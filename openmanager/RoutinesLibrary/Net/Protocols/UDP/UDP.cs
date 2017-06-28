// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Net;
using System.Net.Sockets;
using System.Threading;

    namespace RoutinesLibrary.Net.Protocols.UDP
    {

        /// <summary>
        /// Provides UDP communication
        /// </summary>
        public class UDP
        {

            private UdpClient m_Socket; //UDP socket
            private ushort m_Port; //UDP port
            private IPEndPoint m_RemoteEndPoint; //Remote machine endpoint to send messages

            private bool m_Activated = false; //Connection active
            private Thread m_ThreadProcessDataReceived;

            private RoutinesLibrary.Net.InformationNetworkInterface.NetworkInterfaceAddress[] m_NetworkInterfaceAddress; //Network interfaces to send messages


            /// <summary>
            /// Raises an event with the data received
            /// </summary>
            /// <param name="Data">Data received</param>
            /// <param name="RemoteEndPoint">Ip address of the remote computer that sent the message</param>
            public delegate void DataReceivedEventHandler(byte[] Data, IPEndPoint RemoteEndPoint);
            private DataReceivedEventHandler DataReceivedEvent;

            public event DataReceivedEventHandler DataReceived
            {
                add
                {
                    DataReceivedEvent = (DataReceivedEventHandler)System.Delegate.Combine(DataReceivedEvent, value);
                }
                remove
                {
                    DataReceivedEvent = (DataReceivedEventHandler)System.Delegate.Remove(DataReceivedEvent, value);
                }
            }


            #region CONSTRUCTORS

            /// <summary>
            /// Class constructor
            /// </summary>
            /// <param name="_Port">UDP port</param>
            /// <param name="_remoteEndPoint">Remote EndPoint to connect</param>
            public UDP(ushort _Port, IPEndPoint _remoteEndPoint = default(IPEndPoint))
            {
                Port = _Port;
                RemoteEndPoint = _remoteEndPoint;
            }

            /// <summary>
            /// Stop communications and release resources
            /// </summary>
            public void Dispose()
            {
                Deactivate();
            }

            #endregion

            #region PROPERTIES

            /// <summary>
            /// Get or set the UDP port
            /// </summary>
            /// <value>UDP port</value>
            /// <returns>UDP port</returns>
            public ushort Port
            {
                get
                {
                    ushort returnValue = default(ushort);
                    returnValue = m_Port;
                    return returnValue;
                }
                set
                {
                    bool activatedSocket = m_Activated;

                    if (activatedSocket)
                    {
                        Deactivate();
                    }
                    m_Port = value;
                    if (activatedSocket)
                    {
                        Activate();
                    }
                }
            }

            /// <summary>
            /// Get or set the remote EndPoint to send messages
            /// </summary>
            /// <value>Remote EndPoint</value>
            /// <returns>Remote EndPoint</returns>
            public IPEndPoint RemoteEndPoint
            {
                get
                {
                    IPEndPoint returnValue = default(IPEndPoint);
                    returnValue = m_RemoteEndPoint;
                    return returnValue;
                }
                set
                {
                    m_RemoteEndPoint = value;
                }
            }

            #endregion

            #region PUBLIC METHODS

            #region Start / Stop

            /// <summary>
            /// Activate communication
            /// </summary>
            public void Activate()
            {
                m_Activated = true;

                m_Socket = new UdpClient(new IPEndPoint(IPAddress.Any, Port)); //el udpclient también hace bind
                m_Socket.EnableBroadcast = true;
                m_Socket.Ttl = (short)10;

                //Listen receive data
                m_ThreadProcessDataReceived = new Thread(new System.Threading.ThreadStart(Process_DataReceive));
                m_ThreadProcessDataReceived.Name = "DataReceive_UDP";
                m_ThreadProcessDataReceived.Start();

                //Obtiene todos los puertos del PC IPv4
                m_NetworkInterfaceAddress = RoutinesLibrary.Net.InformationNetworkInterface.GetNetworkInterfaceAddress();
            }

            /// <summary>
            /// Deactivated communications
            /// </summary>
            public void Deactivate()
            {
                m_Activated = false;

                if (m_Socket != null)
                {
                    m_Socket.Close();
                    m_Socket = null;
                }
            }

            #endregion

            #region Send

            /// <summary>
            /// Send message to remote computer
            /// </summary>
            /// <param name="data">Message to send</param>
            public void Send(byte[] data)
            {
                if (!m_Activated)
                {
                    return;
                }

                if (ReferenceEquals(RemoteEndPoint, null))
                {
                    throw (new System.Exception("Remote EndPoint nos defined"));
                }

                try
                {
                    m_Socket.Send(data, data.Length, RemoteEndPoint);
                }
                catch (Exception ex)
                {
                    throw (new System.Exception(ex.Message));
                }
            }

            /// <summary>
            /// Send message to broadcast
            /// </summary>
            /// <param name="data">Message to send</param>
            /// <param name="portSend">Destination port</param>
            public void SendBroadcast(byte[] data, ushort portSend)
            {
                if (!m_Activated)
                {
                    return;
                }

                IPEndPoint addrEndPoint = default(IPEndPoint);
                try
                {
                    //Send to each address of net adapter
                    for (int i = 0; i <= m_NetworkInterfaceAddress.Length - 1; i++)
                    {
                        addrEndPoint = new IPEndPoint(m_NetworkInterfaceAddress[i].Broadcast, portSend);
                        m_Socket.Send(data, data.Length, addrEndPoint);
                    }

                    //Send to all
                    addrEndPoint = new IPEndPoint(new IPAddress(new byte[] { 255, 255, 255, 255 }), portSend);
                    m_Socket.Send(data, data.Length, addrEndPoint);

                }
                catch (Exception ex)
                {
                    throw (new System.Exception(ex.Message));
                }
            }

            #endregion

            #endregion

            #region PRIVATE METHODS

            /// <summary>
            /// Listen incomming messages
            /// </summary>
            private void Process_DataReceive()
            {
                while (m_Activated)
                {
                    IPEndPoint requestRemoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] dataBytes = null;

                    try
                    {
                        dataBytes = m_Socket.Receive(ref requestRemoteIPEndPoint);
                        if (DataReceivedEvent != null)
                            DataReceivedEvent(dataBytes, requestRemoteIPEndPoint);

                    }
                    catch (SocketException ex)
                    {
                        if (m_Activated)
                        {
                            throw (new System.Exception(ex.Message));
                        }
                    }
                }
            }

            #endregion

        }

    }

