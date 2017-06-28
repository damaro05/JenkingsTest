using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using DataJBC;

// 13/05/2014 Añadir la posibilidad de conexiones NAK (protocolo de conexión/frames 01) con órdenes de protocolo 02
//            Para lo cual el tipo de conexión indica el tipo de frames a utilizar (connection/frame protocol).
//            El commando FIRMWARE nos indicará el conjunto y formato de las órdenes a utilizar (command protocol)
// 26/06/2014 El FIRMWARE se pide siempre al Device 0 (en protocolo 02), porque en las touch el HANDSHAKE lo emite el device COM y devuelve COM
// 05/11/2014 Se añade el device de la estación que se conecta en el evento NewConnection
// 05/11/2014 Se vuelve a enviar al device de conexión y se analiza en WaitFW si el FIRMWARE me ha devuelto como modelo COM_DTE_01, por lo que es una DME
//            no actualizada y debo pedir el FIRMWARE (y continuar) con el devide H00


namespace JBC_Connect
{
    /// <summary>
    /// Clase para la busqueda de nuevos equipos
    /// </summary>
    /// <remarks></remarks>
    internal class SearchConnectUSB : CSearchConnect
    {

        private const int MS_WAIT_SEARCH = 1000; //1s para una nueva busqueda por todos los puertos del PC
        private const int MS_WAIT_NAK = 300; //Espera como máximo 3 envios, El equipo que se conecta debe estar enviando NAK's cada 100ms
        private const int MAX_REINTENTOS_SOBRE_PUERTO_SOLICITADO = 5;

        private ReadOnlyCollection<string> m_PortsList; // lista de puertos USB
        private int m_IndexPort;
        private RoutinesLibrary.IO.SerialPort m_SerialPort_Int;
        private string m_PortSearch = "";
        private RoutinesLibrary.IO.SerialPortConfig m_PortConfig = null;
        private bool m_StartHandshake = false;

        private int m_iReintentosSobrePuertoSolicitado = 0;
        private object m_LockTimer = new object();


        #region EVENTS

        //NewConnection
        public delegate void NewConnectionEventHandler(ref CConnectionData connectionData);
        private NewConnectionEventHandler NewConnectionEvent;

        public event NewConnectionEventHandler NewConnection
        {
            add
            {
                NewConnectionEvent = (NewConnectionEventHandler)System.Delegate.Combine(NewConnectionEvent, value);
            }
            remove
            {
                NewConnectionEvent = (NewConnectionEventHandler)System.Delegate.Remove(NewConnectionEvent, value);
            }
        }

        //NoConexion
        internal delegate void NoConexionEventHandler(string sPort);
        private NoConexionEventHandler NoConexionEvent;

        internal event NoConexionEventHandler NoConexion
        {
            add
            {
                NoConexionEvent = (NoConexionEventHandler)System.Delegate.Combine(NoConexionEvent, value);
            }
            remove
            {
                NoConexionEvent = (NoConexionEventHandler)System.Delegate.Remove(NoConexionEvent, value);
            }
        }

#if LibraryTest

        //DataReceivedRawData
	    internal delegate void DataReceivedRawDataEventHandler(ref byte[] RawData);
	    private DataReceivedRawDataEventHandler DataReceivedRawDataEvent;
		
	    internal event DataReceivedRawDataEventHandler DataReceivedRawData
	    {
		    add
		    {
			    DataReceivedRawDataEvent = (DataReceivedRawDataEventHandler) System.Delegate.Combine(DataReceivedRawDataEvent, value);
		    }
		    remove
		    {
			    DataReceivedRawDataEvent = (DataReceivedRawDataEventHandler) System.Delegate.Remove(DataReceivedRawDataEvent, value);
		    }
	    }
		
        //DataSentRawDataEvent
	    internal delegate void DataSentRawDataEventHandler(ref byte[] RawData);
	    private DataSentRawDataEventHandler DataSentRawDataEvent;
		
	    internal event DataSentRawDataEventHandler DataSentRawData
	    {
		    add
		    {
			    DataSentRawDataEvent = (DataSentRawDataEventHandler) System.Delegate.Combine(DataSentRawDataEvent, value);
		    }
		    remove
		    {
			    DataSentRawDataEvent = (DataSentRawDataEventHandler) System.Delegate.Remove(DataSentRawDataEvent, value);
		    }
	    }
		
#endif

        #endregion


        internal SearchConnectUSB(SearchDevicesUSB RefPadre)
        {
            MS_WAIT_ACK = 100;
            MS_WAIT_FIRMWARE = 100;

            m_StatusConnect = StatusConnect.StopSearch;
            m_Timer_Search.Interval = MS_WAIT_SEARCH;
            m_Timer_Search.AutoReset = false;
            m_Timer_Search.Enabled = false;
            m_Timer_Search.Elapsed += Timer_Search_Elapsed;
        }

        internal void Eraser()
        {
            StopSearch();
            m_Timer_Search.Dispose();
            m_Timer_Search.Elapsed -= Timer_Search_Elapsed;
            m_Timer_Search = null;
        }

        internal void StartSearch(string Port, RoutinesLibrary.IO.SerialPortConfig SerialPortConfig, bool bStartHandshake = false)
        {
            m_PortSearch = Port;
            m_PortConfig = SerialPortConfig;
            m_StartHandshake = bStartHandshake;
            m_StatusConnect = StatusConnect.WaitSearch;
            m_Timer_Search.Interval = MS_WAIT_SEARCH;
            m_Timer_Search.Enabled = true;
        }

        private void StopSearch()
        {
            m_Timer_Search.Interval = MS_WAIT_SEARCH;
            m_Timer_Search.Enabled = false;
            m_StatusConnect = StatusConnect.StopSearch;
            if (m_SerialPort_Int != null)
            {
                //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                m_SerialPort_Int.Dispose();
            }
        }

        public void Timer_Search_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Este timer se desactiva siempre al entrar y se debe hacer un start siempre qu se salga
            bool bRestartTimer = true;
            string SearchingPort = "";

            lock (m_LockTimer)
            {
                StatusConnect StatusConnect_New = new StatusConnect();

                switch (m_StatusConnect)
                {
                    case StatusConnect.StopSearch:
                        // si se solicitó un puerto
                        if (!string.IsNullOrEmpty(m_PortSearch) || m_StartHandshake)
                        {
                            bRestartTimer = false;
                        }
                        break;

                    case StatusConnect.WaitSearch:
                        // begin to searching thru the port list
                        if (string.IsNullOrEmpty(m_PortSearch))
                        {
                            string[] portlist = System.IO.Ports.SerialPort.GetPortNames();
                            if (!ReferenceEquals(Type.GetType("Mono.Runtime"), null))
                            {
                                List<string> availablePorts = new List<string>();

                                foreach (string c in portlist)
                                {
                                    if (c.Contains("ttyUSB") || c.Contains("ttyACM"))
                                        availablePorts.Add(c);
                                }

                                portlist = availablePorts.ToArray();
                            }
                            m_PortsList = new ReadOnlyCollection<string>(portlist);
                        }
                        else
                        {
                            List<string> tempList = new List<string>();
                            tempList.Add(m_PortSearch);
                            m_PortsList = new ReadOnlyCollection<string>(tempList);
                            m_iReintentosSobrePuertoSolicitado = 0;
                        }

                        m_IndexPort = m_PortsList.Count - 1;
                        StatusConnect_New = StatusConnect.Search;
                        if (m_Timer_Search != null)
                        {
                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                        }
                        break;

                    case StatusConnect.Search:
                    case StatusConnect.RetryHS:
                        // next in the port list or retry Handshake
                        try
                        {
                            if (m_StatusConnect == StatusConnect.RetryHS)
                            {
                                m_iReintentosSobrePuertoSolicitado++; // aumentar reintentos
                                m_IndexPort++; // reintentar con el índice anterior
                            }
                            if (m_IndexPort < 0)
                            {
                                StatusConnect_New = StatusConnect.WaitSearch;
                                if (m_Timer_Search != null)
                                {
                                    m_Timer_Search.Interval = MS_WAIT_SEARCH;
                                }
                            }
                            else
                            {
                                SerialPort NewSP = new SerialPort();
                                SearchingPort = m_PortsList[m_IndexPort];
                                NewSP.PortName = m_PortsList[m_IndexPort];
                                if (!(RoutinesLibrary.IO.SerialPort.isOpen(NewSP)))
                                {
                                    m_SerialPort_Int = new RoutinesLibrary.IO.SerialPort(NewSP);
                                    m_SerialPort_Int.DataReceived += new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                                    m_SerialPort_Int.ConfigPort(m_PortConfig);
                                    m_IndexPort--;

                                    if (m_Timer_Search != null)
                                    {
                                        m_Timer_Search.Interval = MS_WAIT_NAK;
                                    }

                                    m_MessageFIFOIn.Reset();  // preparar lista de tramas recibidas
                                    m_FrameDataIn01.Reset();  // preparar buffer de entrada
                                    m_FrameDataOut01.Reset(); // preparar buffer de salida
                                    m_FrameDataIn02.Reset();  // preparar buffer de entrada
                                    m_FrameDataOut02.Reset(); // preparar buffer de salida

                                    if (m_StartHandshake)
                                    {
                                        StatusConnect_New = StatusConnect.WaitACKofHS;
                                        m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_02;
                                        SendHandshake();
                                    }
                                    else
                                    {
                                        StatusConnect_New = StatusConnect.WaitNAKorHS;
                                        m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_undefined;
                                    }
                                }
                                else
                                {
                                    //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                                    //TODO: Tiene sentido???
                                    StatusConnect_New = StatusConnect.Search;
                                    m_IndexPort--;
                                    if (m_SerialPort_Int != null)
                                    {
                                        m_SerialPort_Int.Dispose();
                                    }
                                   
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                            //Error. Se prepara para una nueva busqueda
                            StatusConnect_New = StatusConnect.Search;
                            if (m_Timer_Search != null)
                            {
                                m_Timer_Search.Interval = MS_NEW_SEARCH;
                            }
                            //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                            m_SerialPort_Int.Dispose();
                        }
                        break;

                    case StatusConnect.WaitNAKorHS:
                        //TimeOut. Se prepara para una nueva busqueda
                        // HANDSHAKE DE ESTACIÓN
                        StatusConnect_New = StatusConnect.Search;
                        if (m_Timer_Search != null)
                        {
                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                        }
                        //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                        m_SerialPort_Int.Dispose();

                        // si se solicitó un puerto, reintentar 5 veces
                        if (!string.IsNullOrEmpty(m_PortSearch))
                        {
                            if (m_iReintentosSobrePuertoSolicitado >= MAX_REINTENTOS_SOBRE_PUERTO_SOLICITADO)
                            {
                                if (NoConexionEvent != null)
                                    NoConexionEvent(m_PortSearch);
                                StatusConnect_New = StatusConnect.StopSearch;
                                // no rearranca el timer
                                bRestartTimer = false;
                            }
                            else
                            {
                                StatusConnect_New = StatusConnect.RetryHS;
                            }
                        }
                        break;

                    case StatusConnect.WaitACKofHS:
                        //TimeOut. Se prepara para una nueva busqueda
                        // HANDSHAKE DE PC
                        StatusConnect_New = StatusConnect.Search;
                        if (m_Timer_Search != null)
                        {
                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                        }
                        //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                        m_SerialPort_Int.Dispose();

                        // si se solicitó handshake desde el PC, reintentar 5 veces
                        if (m_iReintentosSobrePuertoSolicitado >= MAX_REINTENTOS_SOBRE_PUERTO_SOLICITADO)
                        {
                            // si se especificó un puerto, detener. si no, sigue el StatusConnect.Search
                            if (!string.IsNullOrEmpty(m_PortSearch))
                            {
                                if (NoConexionEvent != null)
                                    NoConexionEvent(m_PortSearch);
                                StatusConnect_New = StatusConnect.StopSearch;
                                // no rearranca el timer
                                bRestartTimer = false;
                            }
                        }
                        else
                        {
                            StatusConnect_New = StatusConnect.RetryHS;
                        }
                        break;

                    case StatusConnect.WaitACK:
                        //TimeOut. Se prepara para una nueva busqueda
                        StatusConnect_New = StatusConnect.Search;
                        if (m_Timer_Search != null)
                        {
                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                        }
                        //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                        m_SerialPort_Int.Dispose();
                        break;

                    case StatusConnect.WaitNum:
                        //TimeOut. Se prepara para una nueva busqueda
                        StatusConnect_New = StatusConnect.Search;
                        if (m_Timer_Search != null)
                        {
                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                        }
                        //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                        m_SerialPort_Int.Dispose();
                        break;

                    case StatusConnect.WaitFW:
                    case StatusConnect.WaitHS:
                        //TimeOut protocolo 2. Se prepara para una nueva busqueda
                        //Debug.Print("Timer_Search_Elapsed-WaitFM timeout")
                        StatusConnect_New = StatusConnect.Search;
                        if (m_Timer_Search != null)
                        {
                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                        }
                        //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                        m_SerialPort_Int.Dispose();
                        break;

                }

                m_StatusConnect = StatusConnect_New;

                try
                {
                    if (bRestartTimer && m_Timer_Search != null)
                    {
                        m_Timer_Search.Start();
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void SendHandshake()
        {
            // enviar handshake (caso de inicio desde el PC)
            byte[] DataOut = null;
            DataOut = new byte[0];
            byte FIDSent = (byte)(0xFD);
            m_FrameDataOut02.Reset(); // preparar buffer de salida
            m_FrameDataOut02.EncodeFrame(DataOut, (byte)(0x19), (byte)(0x10), (byte)EnumCommandFrame_02_SOLD.M_HS, FIDSent);
            SendBytes(m_FrameDataOut02.GetStuffedFrame());
        }

        private void SerialPort_Int_DataReceived()
        {
            lock (m_LockParseDataReceived)
            {
                ParseDataReceived();
            }
        }

        protected override void SendBytes(byte[] _bytes)
        {
            m_SerialPort_Int.Write(_bytes);

#if LibraryTest
		    DataSentRawDataEvent(ref _bytes);
#endif
        }

        protected override byte[] ReadBytes(int numBytes = 0)
        {
            byte[] bytes = null;
            if (numBytes < 1)
            {
                bytes = m_SerialPort_Int.ReadBytesFromPort();
            }
            else
            {
                bytes = m_SerialPort_Int.ReadBytesFromPort(numBytes);
            }

#if LibraryTest
		    DataReceivedRawDataEvent(ref bytes);
#endif

            return bytes;
        }

        protected override void RaiseNewConnection(CStationBase.Protocol commandProtocol, string sStationModel, string SoftwareVersion, string HardwareVersion)
        {

            //Cuando una estacion con un k60 esta apagada pero el usb esta conectado, sigue alimentandose.
            if (CheckStationModel(sStationModel))
            {
                //Genera el evento Nueva conexion, de protocolo de conexión/trama 02
                CConnectionData connectionData = new CConnectionData();
                connectionData.Mode = SearchMode.USB;
                connectionData.pSerialPort = m_SerialPort_Int;
                connectionData.PCNumDevice = m_pcNumDevice;
                connectionData.StationNumDevice = m_stationNumDevice;
                connectionData.FrameProtocol = m_Conection_FrameProtocol;
                connectionData.CommandProtocol = commandProtocol;
                connectionData.StationModel = sStationModel;
                connectionData.SoftwareVersion = SoftwareVersion;
                connectionData.HardwareVersion = HardwareVersion;

                //Cambia de referencia al puerto porque la anterior ya ha sido asignada
                //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                m_SerialPort_Int.Dispose(false);
                m_SerialPort_Int = null;

                if (NewConnectionEvent != null)
                    NewConnectionEvent(ref connectionData);
            }
            else
            {
                //Conexion no válida
                //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                m_SerialPort_Int.Dispose();
                m_Timer_Search.Stop();
                m_Timer_Search.Interval = MS_NEW_SEARCH;
                m_Timer_Search.Start();
            }
        }

        protected override void CloseConnection()
        {
            //m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
            m_SerialPort_Int.Dispose();
        }

    }
}
