// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;

namespace JBC_Connect
{
    /// <summary>
    /// Manage the messages to send to a specific address
    /// </summary>
    /// <remarks>Manage maintenance and timeout communication</remarks>
    public class CStackMessages
    {

        private const int TIME_TIMEOUT_MESSAGE = 1500;
        private const int TIME_TIMEOUT_MESSAGE_DELAYED_RESPONSE = 15000;
        private const int TIMER_CONNECT_MAINTENANCE = 500;


        private byte m_address;
        private List<CMessageCom> m_Messages = new List<CMessageCom>();

        private int m_frameWindow;
        private int m_ToSendFrameWindow;

        private Timer m_TimerConnectMaintenance;
        private Timer m_TimerConnectTimeOut;
        private Mutex m_mutexTimer = new Mutex();


        public delegate void SendMessageEventHandler(CMessageCom message);
        private SendMessageEventHandler SendMessageEvent;

        public event SendMessageEventHandler SendMessage
        {
            add
            {
                SendMessageEvent = (SendMessageEventHandler)System.Delegate.Combine(SendMessageEvent, value);
            }
            remove
            {
                SendMessageEvent = (SendMessageEventHandler)System.Delegate.Remove(SendMessageEvent, value);
            }
        }

        public delegate void ConnectionMaintenanceEventHandler(byte address);
        private ConnectionMaintenanceEventHandler ConnectionMaintenanceEvent;

        public event ConnectionMaintenanceEventHandler ConnectionMaintenance
        {
            add
            {
                ConnectionMaintenanceEvent = (ConnectionMaintenanceEventHandler)System.Delegate.Combine(ConnectionMaintenanceEvent, value);
            }
            remove
            {
                ConnectionMaintenanceEvent = (ConnectionMaintenanceEventHandler)System.Delegate.Remove(ConnectionMaintenanceEvent, value);
            }
        }

        public delegate void ConnectionTimeOutEventHandler(byte address, byte command);
        private ConnectionTimeOutEventHandler ConnectionTimeOutEvent;

        public event ConnectionTimeOutEventHandler ConnectionTimeOut
        {
            add
            {
                ConnectionTimeOutEvent = (ConnectionTimeOutEventHandler)System.Delegate.Combine(ConnectionTimeOutEvent, value);
            }
            remove
            {
                ConnectionTimeOutEvent = (ConnectionTimeOutEventHandler)System.Delegate.Remove(ConnectionTimeOutEvent, value);
            }
        }



        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="address">Address to send the messages</param>
        /// <param name="frameWindow">Number of simultaneous messages to send</param>
        public CStackMessages(byte address, int frameWindow)
        {

            m_address = address;
            m_frameWindow = frameWindow;
            m_ToSendFrameWindow = frameWindow;

            //Timeout messages . Stop
            m_TimerConnectTimeOut = new Timer(Elapsed_ConnectTimeOut, null,
                Timeout.Infinite,
                Timeout.Infinite);

            //Connect maintenance . Start
            m_TimerConnectMaintenance = new Timer(Elapsed_ConnectMaintenance, null,
                TIMER_CONNECT_MAINTENANCE,
                TIMER_CONNECT_MAINTENANCE);
        }

        /// <summary>
        /// Dispose all the resources
        /// </summary>
        public void Dispose()
        {
            m_Messages.Clear();
            m_Messages = null;

            m_mutexTimer.WaitOne();

            m_TimerConnectTimeOut.Change(Timeout.Infinite, Timeout.Infinite); //Stop the Timer
            m_TimerConnectTimeOut.Dispose();
            m_TimerConnectTimeOut = null;

            m_TimerConnectMaintenance.Change(Timeout.Infinite, Timeout.Infinite); //Stop the Timer
            m_TimerConnectMaintenance.Dispose();
            m_TimerConnectMaintenance = null;

            m_mutexTimer.ReleaseMutex();
        }

        /// <summary>
        /// Add a new message to the FIFO list
        /// </summary>
        /// <param name="message">New message to send</param>
        public void Add(CMessageCom message)
        {
            m_Messages.Add(message);
            TryToSend();
        }

        /// <summary>
        /// Get the Number Message of a specific sent message
        /// </summary>
        /// <param name="FID">Message's FID</param>
        /// <param name="Command">Message's command</param>
        /// <returns>Number Message</returns>
        /// <remarks>This function use the FID if the frame window is greater than 1, otherwise use the command</remarks>
        public uint Response(byte FID, byte Command)
        {
            uint numMessage = (uint)0;
            CMessageCom message = null;

            if (m_frameWindow > 1)
            {
                for (var i = 0; i <= m_Messages.Count - 1; i++)
                {
                    if (m_Messages[i].FID() == FID)
                    {
                        message = m_Messages[i];
                        m_Messages.RemoveAt(System.Convert.ToInt32(i));
                        break;
                    }
                }
            }
            else
            {
                if (m_Messages.Count > 0)
                {
                    if (m_Messages[0].Command() == Command)
                    {
                        message = m_Messages[0];
                        m_Messages.RemoveAt(0);
                    }
                }
            }

            if (message != null)
            {
                //Desactivamos el timer de timeout ya que no esperamos respuesta
                m_TimerConnectTimeOut.Change(Timeout.Infinite, Timeout.Infinite); //Stop the Timer

                m_ToSendFrameWindow++;
                numMessage = message.NumberMessage();

                //Si no hay mas mensajes para enviar empezamos el mantenimiento de la conexión
                if (m_Messages.Count == 0)
                {
                    m_TimerConnectMaintenance.Change(TIMER_CONNECT_MAINTENANCE, TIMER_CONNECT_MAINTENANCE); //Start the Timer
                }
            }

            TryToSend();

            return numMessage;
        }

        /// <summary>
        /// Send messages until the pending messages to response is equal to the frame window
        /// </summary>
        private void TryToSend()
        {
            while (m_ToSendFrameWindow > 0)
            {
                CMessageCom message = ReadMessage();

                if (ReferenceEquals(message, null))
                {
                    break;
                }
                else
                {
                    //Decrementamos la ventana de mensajes
                    m_ToSendFrameWindow--;

                    //Activamos el timer de timeout
                    m_TimerConnectTimeOut.Change(Timeout.Infinite, Timeout.Infinite); //Stop the Timer
                    if (message.DelayedResponse)
                    {
                        //No desactivamos el mantenimiento de la conexión ya que el tiempo de respuesta es demasiado elevado
                        m_TimerConnectMaintenance.Change(TIMER_CONNECT_MAINTENANCE, TIMER_CONNECT_MAINTENANCE); //Start the Timer

                        m_TimerConnectTimeOut.Change(TIME_TIMEOUT_MESSAGE_DELAYED_RESPONSE, TIME_TIMEOUT_MESSAGE_DELAYED_RESPONSE); //Start the Timer
                    }
                    else
                    {
                        //Desactivamos el mantenimiento de la conexión
                        m_TimerConnectMaintenance.Change(Timeout.Infinite, Timeout.Infinite); //Stop the Timer

                        m_TimerConnectTimeOut.Change(TIME_TIMEOUT_MESSAGE, TIME_TIMEOUT_MESSAGE); //Start the Timer
                    }

                    //FIXME . Modo BURST solo se utiliza para la DME.
                    //Para la DME todas las comunicaciones van contra la dirección 0x00 pero cuando se envía un ACK se tiene que hacer contra la 0x10, sino, responde NACK
                    if (m_frameWindow > 1 & message.Command() == (byte)EnumCommandFrame.M_ACK)
                    {
                        message.Address = (byte)(0x10);
                    }

                    if (SendMessageEvent != null)
                        SendMessageEvent(message);
                }
            }
        }

        /// <summary>
        /// Get the next message to send in the FIFO list
        /// </summary>
        /// <returns>Message to send</returns>
        private CMessageCom ReadMessage()
        {
            CMessageCom message = null;

            //Si está en modo BURST envía el siguiente que todavía no haya enviado
            if (m_frameWindow > 1)
            {
                for (var i = 0; i <= m_Messages.Count - 1; i++)
                {
                    if (m_Messages[i].TriesRemaining() == CMessageCom.MAX_MESSAGE_TRIES)
                    {
                        message = m_Messages[i];
                        message.DecrementTriesRemaining();
                        m_Messages[i] = message;

                        break;
                    }
                }

                //Si está en modo SINGLE envía el primero
            }
            else
            {
                if (m_Messages.Count > 0)
                {
                    if (m_Messages[0].TriesRemaining() == CMessageCom.MAX_MESSAGE_TRIES)
                    {
                        message = m_Messages[0];
                        message.DecrementTriesRemaining();
                        m_Messages[0] = message;
                    }
                }
            }

            return message;
        }

        /// <summary>
        /// Manage a timeout communication. Tries to resend the message. If attempts have been exhausted, raise an event
        /// </summary>
        /// <param name="state">Timer state</param>
        private void Elapsed_ConnectTimeOut(object state)
        {

            m_TimerConnectTimeOut.Change(Timeout.Infinite, Timeout.Infinite); //Stop the Timer

            if (m_Messages.Count > 0)
            {
                CMessageCom message = m_Messages[0];

                if (message.TriesRemaining() > 0)
                {
                    message.DecrementTriesRemaining();
                    m_Messages[0] = message;
                    if (message.DelayedResponse)
                    {
                        m_TimerConnectTimeOut.Change(TIME_TIMEOUT_MESSAGE_DELAYED_RESPONSE, TIME_TIMEOUT_MESSAGE_DELAYED_RESPONSE); //Start the Timer
                    }
                    else
                    {
                        m_TimerConnectTimeOut.Change(TIME_TIMEOUT_MESSAGE, TIME_TIMEOUT_MESSAGE); //Start the Timer
                    }


                    if (SendMessageEvent != null)
                        SendMessageEvent(message);
                }
                else
                {
                    m_Messages.RemoveAt(0);

                    if (ConnectionTimeOutEvent != null)
                        ConnectionTimeOutEvent(m_address, message.Command());
                }
            }
        }

        /// <summary>
        /// Maintenance the communication
        /// </summary>
        /// <param name="state">Timer state</param>
        private void Elapsed_ConnectMaintenance(object state)
        {

            if (ConnectionMaintenanceEvent != null)
                ConnectionMaintenanceEvent(m_address);

            m_mutexTimer.WaitOne();
            //Es necesario comprobarlo ya que es posible que se haya hecho un Dispose y más tarde salte el evento del timer
            if (m_TimerConnectMaintenance != null)
            {
                m_TimerConnectMaintenance.Change(Timeout.Infinite, Timeout.Infinite); //Stop the Timer
                m_TimerConnectMaintenance.Change(TIMER_CONNECT_MAINTENANCE, TIMER_CONNECT_MAINTENANCE); //Start the Timer
            }
            m_mutexTimer.ReleaseMutex();
        }

    }
}
