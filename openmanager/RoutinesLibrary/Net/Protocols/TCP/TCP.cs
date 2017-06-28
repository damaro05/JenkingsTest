// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text;


    namespace RoutinesLibrary.Net.Protocols.TCP
    {

        /// <summary>
        /// Provides TCP communication
        /// </summary>
        public class TCP
        {

            private const int DEFAULT_BUFFER_SIZE = (2 * (256 + 7));


            private TcpClient m_socket = null; //TCP socket
            private NetworkStream m_Stream = null; //Network stream to send and receive data
            private IPEndPoint m_HostIPEndPoint = new IPEndPoint(0, 0); //Remote EndPoint to send and receive messages

            private int m_BufferSize;

            private Thread m_ThreadProcessDataReceived;
            private bool m_IsAliveThread = true;

            /// <summary>
            /// Communicates closed connections
            /// </summary>
            public delegate void ClosedConnectionTCPEventHandler();
            private ClosedConnectionTCPEventHandler ClosedConnectionTCPEvent;

            public event ClosedConnectionTCPEventHandler ClosedConnectionTCP
            {
                add
                {
                    ClosedConnectionTCPEvent = (ClosedConnectionTCPEventHandler)System.Delegate.Combine(ClosedConnectionTCPEvent, value);
                }
                remove
                {
                    ClosedConnectionTCPEvent = (ClosedConnectionTCPEventHandler)System.Delegate.Remove(ClosedConnectionTCPEvent, value);
                }
            }


            /// <summary>
            /// Raises an event with the data received
            /// </summary>
            /// <param name="Data">Data received</param>
            public delegate void DataReceivedEventHandler(byte[] data);
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
            /// <param name="_hostEndPoint">Remote EndPoint to connect</param>
            public TCP(IPEndPoint _hostEndPoint)
            {
                m_HostIPEndPoint = _hostEndPoint;
                m_BufferSize = DEFAULT_BUFFER_SIZE;
            }

            /// <summary>
            /// Stop communications and release resources
            /// </summary>
            public void Dispose()
            {
                m_IsAliveThread = false;
                Close();
            }

            #endregion

            #region PROPERTIES

            /// <summary>
            /// Get the remote EndPoint, both IP and port
            /// </summary>
            /// <value>Remote EndPoint</value>
            /// <returns>Remote EndPoint</returns>
            public IPEndPoint HostEndPoint
            {
                get
                {
                    IPEndPoint returnValue = default(IPEndPoint);
                    returnValue = m_HostIPEndPoint;
                    return returnValue;
                }
            }

            /// <summary>
            /// Get the remote IP
            /// </summary>
            /// <value>Remote IP</value>
            /// <returns>Remote IP</returns>
            public IPAddress HostIP
            {
                get
                {
                    IPAddress returnValue = default(IPAddress);
                    returnValue = m_HostIPEndPoint.Address;
                    return returnValue;
                }
            }

            /// <summary>
            /// Get the remote port
            /// </summary>
            /// <value>Remote port</value>
            /// <returns>Remote port</returns>
            public ushort HostPort
            {
                get
                {
                    ushort returnValue = default(ushort);
                    returnValue = System.Convert.ToUInt16((ushort)m_HostIPEndPoint.Port);
                    return returnValue;
                }
            }

            /// <summary>
            /// Get or set the buffer size to receive messages
            /// </summary>
            /// <value>Buffer size</value>
            /// <returns>Buffer size</returns>
            public int BufferSize
            {
                get
                {
                    int returnValue = 0;
                    returnValue = m_BufferSize;
                    return returnValue;
                }
                set
                {
                    m_BufferSize = value;
                }
            }

            #endregion

            #region METODOS PUBLICOS

            /// <summary>
            /// Connect to remote EndPoint
            /// </summary>
            /// <param name="sError">Error messages if the operation was failed</param>
            /// <returns>True if the operation was successful</returns>
            public bool Connect(ref string sError)
            {
                sError = "";
                m_socket = new TcpClient();

                try
                {
                    m_socket.Connect(HostEndPoint);
                    m_Stream = m_socket.GetStream();
                }
                catch (Exception ex)
                {
                    sError = ex.Message;
                    m_socket.Close();
                    m_socket = null;
                    m_Stream = null;
                    return false;
                }

                //Listen receive data
                m_ThreadProcessDataReceived = new Thread(new System.Threading.ThreadStart(Process_DataReceive));
                m_ThreadProcessDataReceived.Name = "DataReceive_TCP";
                m_ThreadProcessDataReceived.Start();

                return true;
            }

            /// <summary>
            /// Send data to remote computer
            /// </summary>
            /// <param name="Data">Message data</param>
            public void SendData(string Data)
            {
                SendData(Encoding.ASCII.GetBytes(Data));
            }

            /// <summary>
            /// Send data to remote computer
            /// </summary>
            /// <param name="Data">Message data</param>
            public void SendData(byte[] Data)
            {
                if (!(ReferenceEquals(m_Stream, null)))
                {
                    m_Stream.Write(Data, 0, Data.Length);
                }
            }

            #endregion

            #region FUNCIONES PRIVADAS

            /// <summary>
            /// Listen incomming messages
            /// </summary>
            private void Process_DataReceive()
            {
                byte[] buffer = null;
                int readedDataCount = 0;

                while (m_IsAliveThread)
                {
                    try
                    {
                        buffer = new byte[BufferSize - 1 + 1];
                        readedDataCount = m_Stream.Read(buffer, 0, buffer.Length);

                        if (readedDataCount > 0)
                        {
                            byte[] readedData = new byte[readedDataCount - 1 + 1];
                            Array.Copy(buffer, readedData, readedDataCount);

                            if (DataReceivedEvent != null)
                                DataReceivedEvent(readedData);
                        }
                    }
                    catch (Exception)
                    {
                        goto endOfWhileLoop;
                    }
                }
                endOfWhileLoop:

                if (ClosedConnectionTCPEvent != null)
                    ClosedConnectionTCPEvent();
            }

            /// <summary>
            /// Close TCP connection
            /// </summary>
            private void Close()
            {

                //socket shutdown
                try
                {
                    if (m_socket != null)
                    {
                        m_socket.Client.Shutdown(SocketShutdown.Both);
                        m_socket.Client.Close();
                    }
                }
                catch (Exception)
                {
                }

                //close stream
                try
                {
                    if (m_Stream != null)
                    {
                        m_Stream.Close();
                        m_Stream = null;
                    }
                }
                catch (Exception)
                {
                }

                //close tcp
                try
                {
                    if (m_socket != null)
                    {
                        m_socket.Close();
                        m_socket = null;
                    }
                }
                catch (Exception)
                {
                }
            }

            #endregion

        }

    }

