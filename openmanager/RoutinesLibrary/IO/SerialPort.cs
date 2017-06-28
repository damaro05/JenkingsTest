// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO.Ports;
using System.Threading;


    namespace RoutinesLibrary.IO
    {

        public class SerialPortConfig
        {

            public int BaudRate;
            public int DataBits;
            public StopBits StopBits;
            public Parity Parity;
            public Handshake Handshake;
            public int WriteBufferSize;
            public int ReadBufferSize;

            public SerialPortConfig()
            {
                SetDefaults();
            }

            public SerialPortConfig(int _baud, Parity _parity)
            {
                SetDefaults();
                BaudRate = _baud;
                Parity = _parity;
            }

            public void SetDefaults()
            {
                BaudRate = 500000;
                DataBits = 8;
                StopBits = System.IO.Ports.StopBits.One;
                Parity = System.IO.Ports.Parity.Even;
                Handshake = System.IO.Ports.Handshake.None;
                WriteBufferSize = 512;
                ReadBufferSize = 512;
            }

        }

        public class SerialPort
        {

            private System.IO.Ports.SerialPort m_serialPort;
            private Thread m_ThreadProcessDataReceived;
            private bool m_IsAliveThread = true;

            private static Hashtable m_openPorts = new Hashtable();

            public delegate void DataReceivedEventHandler();
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



            #region Constructors / Destructors

            /// <summary>
            /// Class constructor. Start listen on port
            /// </summary>
            /// <param name="_serialPort"></param>
            /// <remarks></remarks>
            public SerialPort(System.IO.Ports.SerialPort _serialPort)
            {
                m_serialPort = _serialPort;

                //Listen received data
                m_ThreadProcessDataReceived = new Thread(new System.Threading.ThreadStart(Process_DataReceived));
                m_ThreadProcessDataReceived.Name = "DataReceived_SerialPort";
                m_ThreadProcessDataReceived.Start();
            }

            /// <summary>
            /// Release resources
            /// </summary>
            /// <param name="releaseSerialPort">Indicates if the serial port will be close</param>
            public void Dispose(bool releaseSerialPort = true)
            {
                m_IsAliveThread = false;
                DataReceivedEvent = null;

                if (releaseSerialPort)
                {
                    try
                    {
                        if (m_serialPort != null && m_serialPort.IsOpen)
                        {
                            closePort(m_serialPort);
                            
                            m_serialPort.Dispose();
                            m_serialPort = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                    }
                }
            }

            #endregion

            #region Properties

            public System.IO.Ports.SerialPort GetSerialPort
            {
                get
                {
                    return m_serialPort;
                }
            }

            #endregion

            #region Configuration

            public void ConfigPort(SerialPortConfig _serialPortConfig = default(SerialPortConfig))
            {
                ConfigPort(m_serialPort, _serialPortConfig);
            }

            private static void openPort(System.IO.Ports.SerialPort _serialPort)
            {
                _serialPort.Open();
                m_openPorts.Add(_serialPort.PortName, _serialPort);
            }

            private static void closePort(System.IO.Ports.SerialPort _serialPort)
            {
                _serialPort.Close();
                m_openPorts.Remove(_serialPort.PortName);
            }

            public static bool isOpen(System.IO.Ports.SerialPort _serialPort)
            {
                bool isOpen = false;
                //return isOpen;
                if(m_openPorts.Contains(_serialPort.PortName))
                {
                    System.IO.Ports.SerialPort storedPort = (System.IO.Ports.SerialPort)m_openPorts[_serialPort.PortName];
                    isOpen = storedPort.IsOpen;
                    if(!isOpen)
                    {
                        closePort(storedPort);
                    }
                }
                return isOpen;
            }

            public static bool ConfigPort(System.IO.Ports.SerialPort _serialPort, SerialPortConfig _serialPortConfig = default(SerialPortConfig))
            {

                Console.WriteLine(" Port: " + _serialPort.PortName);
                if (_serialPortConfig == null)
                {
                    _serialPortConfig = new SerialPortConfig();
                }

                bool portOpened = false;
                try
                {
                    _serialPort.DataBits = _serialPortConfig.DataBits;
                    _serialPort.StopBits = _serialPortConfig.StopBits;
                    _serialPort.Parity = _serialPortConfig.Parity;
                    _serialPort.Handshake = _serialPortConfig.Handshake;
                    _serialPort.WriteBufferSize = _serialPortConfig.WriteBufferSize;
                    _serialPort.ReadBufferSize = _serialPortConfig.ReadBufferSize;
                    _serialPort.Encoding = System.Text.Encoding.GetEncoding(28591);

                //Causes an error if it is being used
                    openPort(_serialPort);
                    _serialPort.DiscardInBuffer();
                    _serialPort.DiscardOutBuffer();
                   
                    portOpened = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                }

                if (portOpened)
                {
                    try
                    {
                        //Causes error in Mono for non-standard baud rate
                        _serialPort.BaudRate = _serialPortConfig.BaudRate;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                        ForceSetBaudRate(_serialPort.PortName, _serialPortConfig.BaudRate);
                    }
                }

                return portOpened;
            }

            #endregion

            #region Read / Write

            public int BytesToRead()
            {
                int bytesToRead = 0;
                try
                {
                    bytesToRead = m_serialPort.BytesToRead;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                }
                return bytesToRead;
            }

            public byte ReadByte()
            {
                byte readByte = 0;
                try
                {
                    readByte = (byte)m_serialPort.ReadByte();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                }
                return readByte;
            }

            public byte[] ReadBytesFromPort(int iMaxBuffer = 512)
            {
                return ReadBytesFromPort(m_serialPort, iMaxBuffer);
            }

            public static byte[] ReadBytesFromPort(System.IO.Ports.SerialPort _serialPort, int iMaxBuffer = 512)
            {
                byte[] DataIn = null;
                int index = 0;

                try
                {
                    byte[] DatosInBytes = new byte[iMaxBuffer];

                    while (_serialPort.BytesToRead != 0)
                    {
                        DatosInBytes[index] = (byte)(_serialPort.ReadByte());
                        index++;
                        if (index >= iMaxBuffer - 1)
                        {
                            break;
                        }
                    }

                    DataIn = new byte[index];
                    if (index > 0)
                    {
                        Array.Copy(DatosInBytes, DataIn, index);
                    }

                    return DataIn;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                    DataIn = new byte[0];
                    return DataIn;
                }
            }

            public void Write(byte[] _bytes)
            {
                try
                {
                    m_serialPort.Write(_bytes, 0, _bytes.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                }
            }

            #endregion

            #region Private methods

            private void Process_DataReceived()
            {
                while (m_IsAliveThread)
                {
                    try
                    {
                        if (m_serialPort.IsOpen)
                        {
                            //Causes an error if it is being used
                            if (m_serialPort.BytesToRead > 0)
                            {
                                if (DataReceivedEvent != null)
                                    DataReceivedEvent();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                    }

                    Thread.Sleep(10);
                }
            }


            private static void ForceSetBaudRate(string portName, int baudRate)
            {
                if (ReferenceEquals(Type.GetType("Mono.Runtime"), null))
                {
                    return;
                }
                //It is not mono === not linux!
                string arg = string.Format("stty -F {0} speed {1}", portName, baudRate);
                var proc = new System.Diagnostics.Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = "sudo";
                proc.StartInfo.Arguments = arg;

                proc.Start();
                proc.WaitForExit();
            }

            #endregion

        }

    }

