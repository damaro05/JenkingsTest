// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO.Ports;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;
using DataJBC;


namespace JBC_Connect
{
    internal class CCommunicationChannel
    {

        private const int MAX_FRAME_WINDOW = 8; // how many frames to be sent or received for flowcontrol = BURST
        private const byte MAX_FID = 0xEF; // (protocolo 02)


        //Channel
        private SearchMode m_mode;
        private RoutinesLibrary.IO.SerialPort m_pSerialPort;
        private RoutinesLibrary.Net.Protocols.TCP.TCP m_pWinSock;

        //Addesses
        private byte m_sourceAddress;

        private CStationBase.Protocol m_frameProtocol;

        //Flow control
        private int m_frameWindow = 1;
        private EnumFrameFlowControl m_flowControlMethod;

        private Hashtable m_htStackAddressMessages = new Hashtable();
        private static Semaphore m_mutexStackAddressMessages = new Semaphore(1, 1);

        //Number messages
        private uint m_NumberMessage = (uint)0;
        private byte m_FID = (byte)0;

        //DLL
        private Stack_dll m_StackDll;
        private static Semaphore m_mutexStackDll = new Semaphore(1, 1);


        public delegate void MessageResponseEventHandler(uint Numstream, byte[] Datos, byte Command, byte Origen);
        private MessageResponseEventHandler MessageResponseEvent;

        public event MessageResponseEventHandler MessageResponse
        {
            add
            {
                MessageResponseEvent = (MessageResponseEventHandler)System.Delegate.Combine(MessageResponseEvent, value);
            }
            remove
            {
                MessageResponseEvent = (MessageResponseEventHandler)System.Delegate.Remove(MessageResponseEvent, value);
            }
        }

        public delegate void ConnectionErrorEventHandler(EnumConnectError ErrorType, byte address, byte command);
        private ConnectionErrorEventHandler ConnectionErrorEvent;

        public event ConnectionErrorEventHandler ConnectionError
        {
            add
            {
                ConnectionErrorEvent = (ConnectionErrorEventHandler)System.Delegate.Combine(ConnectionErrorEvent, value);
            }
            remove
            {
                ConnectionErrorEvent = (ConnectionErrorEventHandler)System.Delegate.Remove(ConnectionErrorEvent, value);
            }
        }

        public delegate void ResetSendedEventHandler(byte address);
        private ResetSendedEventHandler ResetSendedEvent;

        public event ResetSendedEventHandler ResetSended
        {
            add
            {
                ResetSendedEvent = (ResetSendedEventHandler)System.Delegate.Combine(ResetSendedEvent, value);
            }
            remove
            {
                ResetSendedEvent = (ResetSendedEventHandler)System.Delegate.Remove(ResetSendedEvent, value);
            }
        }

#if LibraryTest
	internal delegate void SentFrameEventHandler(byte[] Frame);
	private SentFrameEventHandler SentFrameEvent;
		
	internal event SentFrameEventHandler SentFrame
	{
		add
		{
			SentFrameEvent = (SentFrameEventHandler) System.Delegate.Combine(SentFrameEvent, value);
		}
		remove
		{
			SentFrameEvent = (SentFrameEventHandler) System.Delegate.Remove(SentFrameEvent, value);
		}
	}
		
	internal delegate void ResponseFrameEventHandler(byte[] Frame);
	private ResponseFrameEventHandler ResponseFrameEvent;
		
	internal event ResponseFrameEventHandler ResponseFrame
	{
		add
		{
			ResponseFrameEvent = (ResponseFrameEventHandler) System.Delegate.Combine(ResponseFrameEvent, value);
		}
		remove
		{
			ResponseFrameEvent = (ResponseFrameEventHandler) System.Delegate.Remove(ResponseFrameEvent, value);
		}
	}
		
	internal delegate void SentRawDataEventHandler(byte[] RawData);
	private SentRawDataEventHandler SentRawDataEvent;
		
	internal event SentRawDataEventHandler SentRawData
	{
		add
		{
			SentRawDataEvent = (SentRawDataEventHandler) System.Delegate.Combine(SentRawDataEvent, value);
		}
		remove
		{
			SentRawDataEvent = (SentRawDataEventHandler) System.Delegate.Remove(SentRawDataEvent, value);
		}
	}
		
	internal delegate void ResponseRawDataEventHandler(byte[] RawData);
	private ResponseRawDataEventHandler ResponseRawDataEvent;
		
	internal event ResponseRawDataEventHandler ResponseRawData
	{
		add
		{
			ResponseRawDataEvent = (ResponseRawDataEventHandler) System.Delegate.Combine(ResponseRawDataEvent, value);
		}
		remove
		{
			ResponseRawDataEvent = (ResponseRawDataEventHandler) System.Delegate.Remove(ResponseRawDataEvent, value);
		}
	}
		
#endif
        private bool StackAddressMessageContains(int key)
        {
            return m_htStackAddressMessages.Contains(key + 1);
        }

        private void StackAddressMessageAdd(int key, object value)
        {
            m_htStackAddressMessages.Add(key + 1, value);
        }

        private CStackMessages StackAddressMessageGet(int key)
        {
            return (CStackMessages)m_htStackAddressMessages[key + 1];
        }

        private void StackAddressMessageRemove(int key)
        {
            m_htStackAddressMessages.Remove(key + 1);
        }

        public CCommunicationChannel(CConnectionData connectionData)
        {
            m_mode = connectionData.Mode;
            m_sourceAddress = connectionData.PCNumDevice;

            if (m_mode == SearchMode.USB)
            {
                m_pSerialPort = connectionData.pSerialPort;
            }
            else if (m_mode == SearchMode.ETH)
            {
                m_pWinSock = connectionData.pWinSock;
            }

            m_frameProtocol = connectionData.FrameProtocol;
        }

        public void Initialize(bool _BurstMessages, byte _StationNumDevice, CStationBase.Protocol _FrameProtocol, CStationBase.Protocol _CommandProtocol)
        {

            if (_BurstMessages)
            {
                m_flowControlMethod = EnumFrameFlowControl.BURST;
                m_frameWindow = MAX_FRAME_WINDOW;
            }
            else
            {
                m_flowControlMethod = EnumFrameFlowControl.ONE_TO_ONE;
            }

            if (m_mode == SearchMode.USB)
            {
                m_StackDll = new Stack_dll(ref m_pSerialPort, m_sourceAddress, _FrameProtocol, m_flowControlMethod, m_frameWindow);
                m_StackDll.DataReceived += DataReceived;
                m_StackDll.DataReceivedMessages += DataReceived_Burst;
                m_StackDll.ErrorConnect += Event_ConnectionError;
#if LibraryTest
			    m_StackDll.DataSentFrame += StackDll_DataSentFrame;
			    m_StackDll.DataReceivedFrame += StackDll_DataReceivedFrame;
			    m_StackDll.DataReceivedFrameMessages += StackDll_DataReceivedFrame_Burst;
			    m_StackDll.DataSentRawData += StackDll_DataSentRawData;
			    m_StackDll.DataReceivedRawData += StackDll_DataReceivedRawData;
#endif
            }
            else if (m_mode == SearchMode.ETH)
            {
                m_StackDll = new Stack_dll(ref m_pWinSock, m_sourceAddress, _FrameProtocol, m_flowControlMethod, m_frameWindow);
                m_StackDll.DataReceived += DataReceived;
                m_StackDll.DataReceivedMessages += DataReceived_Burst;
                m_StackDll.ErrorConnect += Event_ConnectionError;
#if LibraryTest
			    m_StackDll.DataSentFrame += StackDll_DataSentFrame;
			    m_StackDll.DataReceivedFrame += StackDll_DataReceivedFrame;
			    m_StackDll.DataReceivedFrameMessages += StackDll_DataReceivedFrame_Burst;
			    m_StackDll.DataSentRawData += StackDll_DataSentRawData;
			    m_StackDll.DataReceivedRawData += StackDll_DataReceivedRawData;
#endif
            }
        }

        public void Dispose()
        {
            m_mutexStackAddressMessages.WaitOne();

            if (m_htStackAddressMessages != null)
            {
                foreach (DictionaryEntry stack in m_htStackAddressMessages)
                {
                    ((CStackMessages)stack.Value).Dispose();
                }
                m_htStackAddressMessages.Clear();
                m_htStackAddressMessages = null;
            }

            m_mutexStackAddressMessages.Release();

            m_pSerialPort = null;
            m_pWinSock = null;

            m_mutexStackDll.WaitOne();
            if (m_StackDll != null)
            {
#if LibraryTest
			    m_StackDll.DataSentFrame -= StackDll_DataSentFrame;
			    m_StackDll.DataReceivedFrame -= StackDll_DataReceivedFrame;
			    m_StackDll.DataReceivedFrameMessages -= StackDll_DataReceivedFrame_Burst;
			    m_StackDll.DataSentRawData -= StackDll_DataSentRawData;
			    m_StackDll.DataReceivedRawData -= StackDll_DataReceivedRawData;
#endif
                m_StackDll.Eraser();
                m_StackDll = null;
                //m_StackDll.DataReceived += DataReceived;
                //m_StackDll.DataReceivedMessages += DataReceived_Burst;
                //m_StackDll.ErrorConnect += Event_ConnectionError;
            }
            m_mutexStackDll.Release();
        }

        public SearchMode Mode()
        {
            return m_mode;
        }

        public string COMName()
        {
            string name = "";

            // 01/06/2016 Added Edu
            if (m_mode == SearchMode.USB)
            {
                if (m_pSerialPort != null)
                {
                    name = m_pSerialPort.GetSerialPort.PortName;
                }
            }
            else if (m_mode == SearchMode.ETH)
            {
                if (m_pWinSock != null)
                {
                    name = System.Convert.ToString(m_pWinSock.HostIP.ToString());
                }
            }

            return name;
        }

        public string COMType()
        {
            // 01/06/2016 Added Edu
            if (m_mode == SearchMode.USB)
            {
                return "U";
            }
            else if (m_mode == SearchMode.ETH)
            {
                return "E";
            }
            return "";
        }

        public void AddStack(byte stackAddress)
        {
            m_mutexStackAddressMessages.WaitOne();

            //Si no existe la cola la creamos
            if (!StackAddressMessageContains(stackAddress))
            {
                CStackMessages stack = new CStackMessages(stackAddress, m_frameWindow);
                StackAddressMessageAdd(stackAddress, stack);

                stack.SendMessage += SendMessage;
                stack.ConnectionMaintenance += ConnectMaintenance;
                stack.ConnectionTimeOut += ConnectTimeOut;
            }

            m_mutexStackAddressMessages.Release();
        }

        public uint Send(byte[] Datos, byte command, byte address, bool delayedResponse = false)
        {

            //Incrementamos el Number Message
            m_NumberMessage = System.Convert.ToUInt32(m_NumberMessage + 1);
            uint NumMessage = m_NumberMessage;

            //Incrementamos el FID
            if (m_FID >= MAX_FID)
            {
                m_FID = (byte)0;
            }
            m_FID += 1;

            //Creamos el mensaje
            CMessageCom message = new CMessageCom(m_NumberMessage, m_FID, command, Datos, address);
            message.DelayedResponse = delayedResponse;

            CStackMessages stackAddressMessages = null;

            m_mutexStackAddressMessages.WaitOne();

            //Los broadcast los enviamos directamente
            if ((address & (byte)EnumAddress.MASK_BROADCAST_ADDRESS) != (byte)EnumAddress.MASK_BROADCAST_ADDRESS)
            {

                //Si existe la cola lo ponemos en cola, sino, lo enviamos directamente
                if (StackAddressMessageContains(address & (byte)EnumAddress.MASK_STATION_ADDRESS))
                {
                    stackAddressMessages = StackAddressMessageGet(address & (byte)EnumAddress.MASK_STATION_ADDRESS);
                }
            }

            m_mutexStackAddressMessages.Release();

            if (stackAddressMessages != null)
            {
                stackAddressMessages.Add(message);
            }
            else
            {
                SendMessage(message);
            }

            return NumMessage;
        }

        private void SendMessage(CMessageCom message)
        {
            m_mutexStackDll.WaitOne();
            if (m_StackDll != null)
            {
                m_StackDll.SendData(message.Data(), message.Address, message.Command(), message.FID(), message.Response());
            }
            m_mutexStackDll.Release();

            //Se notifica de los Reset para poder continuar con el updater en las estaciones de protocolo 1
            if (message.Command() == (byte)EnumCommandFrame.M_RESET)
            {
                if (ResetSendedEvent != null)
                    ResetSendedEvent(message.Address);
            }
        }

        #region Stack DLL communication

        private void DataReceived(byte Origen, byte Command, byte[] Data, byte FID, bool Response)
        {

            uint NumMessage = (uint)0;
            CStackMessages stackAddressMessages = null;

            //Si es un HandShake respondemos directamente
            if (Command == (byte)EnumCommandFrame.M_NULL)
            {
                if (Response)
                {
                    return;
                }

                byte[] Datos = new byte[1];
                Datos[0] = (byte)EnumCommandFrame.M_ACK;

                CMessageCom message = new CMessageCom((uint)0, (byte)(0xFD), Command, Datos, Origen, true);
                SendMessage(message);

            }
            else if (Command == (byte)EnumCommandFrame.M_NACK)
            {

                m_mutexStackAddressMessages.WaitOne();
                if (m_htStackAddressMessages != null)
                {

                    //Respuesta de una dirección a la que hemos enviado un mensaje
                    if (StackAddressMessageContains(Origen & (byte)EnumAddress.MASK_STATION_ADDRESS))
                    {
                        stackAddressMessages = StackAddressMessageGet(Origen & (byte)EnumAddress.MASK_STATION_ADDRESS);
                    }

                }
                m_mutexStackAddressMessages.Release();

                //Recuperar número de mensaje y eliminarlo de la lista
                if (stackAddressMessages != null)
                {
                    NumMessage = stackAddressMessages.Response(FID, Data[(byte)EnumErrorFrame.Opcode]);
                }
            }
            else
            {


                //FIXME . Modo BURST solo se utiliza para la DME.
                //Para la DME todas las comunicaciones van contra la dirección 00 pero cuando se envía un ACK se tiene que hacer contra la 10, sino, responde NACK
                if (m_flowControlMethod == EnumFrameFlowControl.BURST & Command == (byte)EnumCommandFrame.M_ACK)
                {
                    Origen = (byte)0;
                }

                m_mutexStackAddressMessages.WaitOne();
                if (m_htStackAddressMessages != null)
                {

                    //Respuesta de una dirección a la que hemos enviado un mensaje
                    if (StackAddressMessageContains(Origen & (byte)EnumAddress.MASK_STATION_ADDRESS))
                    {
                        stackAddressMessages = StackAddressMessageGet(Origen & (byte)EnumAddress.MASK_STATION_ADDRESS);
                    }

                }
                m_mutexStackAddressMessages.Release();

                //Recuperar número de mensaje y eliminarlo de la lista
                if (stackAddressMessages != null)
                {
                    NumMessage = stackAddressMessages.Response(FID, Command);
                }
            }
	    
            if (MessageResponseEvent != null)
                MessageResponseEvent(NumMessage, Data, Command, Origen);
        }

        private void DataReceived_Burst(List<CDllFifoMessages.MessageDll> Messages)
        {
            foreach (CDllFifoMessages.MessageDll mess in Messages)
            {
                DataReceived(mess.SourceDevice, mess.Command, mess.Datos, mess.FID, mess.Response);
            }
        }

        private void Event_ConnectionError(Stack_phl.EnumError Tipo)
        {
            //Es necesario lanzar el evento en un nuevo thread ya que los eventos lo ejecuta el mismo thread, y si hay algún mutex se puede producir un 'dead lock'
            System.Threading.Thread t = new System.Threading.Thread(() => ConnectionErrorEvent(EnumConnectError.PHYSICAL, (byte)(0xFF), (byte)0)); //no importa quién provoque el error ya que es para todo el canal de comunicaciones
            t.Start();
        }

        #endregion


        #region Stack messages

        private void ConnectMaintenance(byte address)
        {

            //Incrementamos el Number Message
            m_NumberMessage = System.Convert.ToUInt32(m_NumberMessage + 1);

            //Incrementamos el FID
            if (m_FID >= MAX_FID)
            {
                m_FID = (byte)0;
            }
            m_FID += 1;

            // 11/05/2016 Se cambia a SYN (como originalmente)
            CMessageCom message = new CMessageCom((uint)0, m_FID, (byte)EnumCommandFrame.M_SYN, new byte[] { }, address);
            SendMessage(message);
        }

        private void ConnectTimeOut(byte address, byte command)
        {
            CStackMessages stackAddressMessages = null;

            m_mutexStackAddressMessages.WaitOne();

            if (StackAddressMessageContains(address & (byte)EnumAddress.MASK_STATION_ADDRESS))
            {
                stackAddressMessages = StackAddressMessageGet(address & (byte)EnumAddress.MASK_STATION_ADDRESS);
                StackAddressMessageRemove(address & (byte)EnumAddress.MASK_STATION_ADDRESS);
            }

            m_mutexStackAddressMessages.Release();

            if (stackAddressMessages != null)
            {
                stackAddressMessages.Dispose();
            }

            if (ConnectionErrorEvent != null)
                ConnectionErrorEvent(EnumConnectError.TIME_OUT, address, command);
        }

        #endregion


        #region LibraryTest

#if LibraryTest
		
	private void StackDll_DataSentFrame(byte[] Frame)
	{
		if (SentFrameEvent != null)
			SentFrameEvent(Frame);
	}
		
	private void StackDll_DataReceivedFrame(byte[] Frame)
	{
		if (ResponseFrameEvent != null)
			ResponseFrameEvent(Frame);
	}
		
	private void StackDll_DataReceivedFrame_Burst(List<CDllFifoMessages.MessageDll> Messages)
	{
		foreach (CDllFifoMessages.MessageDll mess in Messages)
		{
			if (ResponseFrameEvent != null)
				ResponseFrameEvent(mess.RawFrame);
		}
	}
		
	private void StackDll_DataSentRawData(byte[] RawData)
	{
		if (SentRawDataEvent != null)
			SentRawDataEvent(RawData);
	}
		
	private void StackDll_DataReceivedRawData(byte[] RawData)
	{
		if (ResponseRawDataEvent != null)
			ResponseRawDataEvent(RawData);
	}
		
		
#endif

        #endregion

    }
}
