using System;
using System.Net;
using DataJBC;


namespace JBC_Connect
{
    /// <summary>
    /// Clase para la busqueda de nuevos equipos
    /// </summary>
    /// <remarks></remarks>
    internal class CSearchConnectTCP : CSearchConnect
    {
        private const int MS_WAIT_SEARCH = 3000; //3s para una nueva busqueda por todas las estaciones descubiertas por UDP
        private const int MS_WAIT_NAK = 2000; //Espera como máximo 10 envios, El equipo que se conecta debe estar enviando NAK's cada 100ms

        private const int iRefreshUDPinWaitSearch = 2; // contador al que debe llegar para actualizar la lista desde UPD
        private const int MAX_REINTENTOS_SOBRE_ENDPOINT_SOLICITADO = 5;


        private CStationtableTCP discoveredStations; // lista de end points TCP
        private int IndexStation = -1;
        private int iRefreshUDPCount = 0;
        private bool bRefreshUDPForce = false;
        private int m_reintentos;
        private RoutinesLibrary.Net.Protocols.TCP.TCP WinSockIn = null;
        private CSearchUDP searchStationsUPD = null;
        private bool bExternalSearcherUDP = false;
        private string EndPointSearch;

        private int iReintentosSobreEndPointSolicitado = 0;
        private object m_LockTimer = new object();
        private byte[] m_DataIn;


        #region EVENTS

        //NewConnectionEvent
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

        //NoConexionEvent
        internal delegate void NoConexionEventHandler(string sEndPoint);
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

        #endregion


        internal CSearchConnectTCP(CSearchDevicesTCP RefPadre)
        {
            MS_WAIT_ACK = 2000;
            MS_WAIT_FIRMWARE = 2000;

            discoveredStations = new CStationtableTCP();
            m_StatusConnect = StatusConnect.StopSearch;
            m_Timer_Search.Interval = MS_WAIT_SEARCH;
            m_Timer_Search.AutoReset = false; // se debe activar de nuevo al salir del evento Elapsed
            m_Timer_Search.Enabled = false;
            m_Timer_Search.Elapsed += Timer_Search_Elapsed;
            m_Timer_Search.Start();
            m_reintentos = 0;
        }

        internal void Eraser()
        {
            StopSearch();
            m_Timer_Search.Dispose();
            m_Timer_Search = null;

            if (!bExternalSearcherUDP)
            {
                searchStationsUPD.Eraser();
                searchStationsUPD = null;
            }
            discoveredStations.Reset();
            discoveredStations = null;
        }

        internal void StartSearch(string EndPoint, CSearchUDP _externalSearcherUDP)
        {
            EndPointSearch = EndPoint.Trim();
            if (ReferenceEquals(searchStationsUPD, null))
            {
                // por si se vuelve a llamar a StartSearch
                if (_externalSearcherUDP != null)
                {
                    bExternalSearcherUDP = true;
                    searchStationsUPD = _externalSearcherUDP;
                }
                else
                {
                    bExternalSearcherUDP = false;
                    searchStationsUPD = new CSearchUDP();
                }
                searchStationsUPD.Refresh += searchStationsUPD_Refresh;
            }

            iRefreshUDPCount = 0;
            bRefreshUDPForce = true;
            if (!searchStationsUPD.IsSearching())
            {
                StartUDPDiscover();
            }
            m_StatusConnect = StatusConnect.WaitSearch;
            m_Timer_Search.Interval = MS_WAIT_SEARCH;
            m_Timer_Search.Enabled = true;
        }

        internal void StopSearch()
        {
            iRefreshUDPCount = -1;
            bRefreshUDPForce = false;
            if (searchStationsUPD.IsSearching())
            {
                StopUDPDiscover();
            }
            m_StatusConnect = StatusConnect.StopSearch;
            m_Timer_Search.Interval = MS_WAIT_SEARCH;
            m_Timer_Search.Enabled = false;
            if (WinSockIn != null)
            {
                try
                {
                    CloseWinSock(ref WinSockIn);
                    WinSockIn = null;
                }
                catch (Exception)
                {
                }
            }
        }

        private void StartUDPDiscover()
        {
            searchStationsUPD.StartSearch();
        }

        private void StopUDPDiscover()
        {
            searchStationsUPD.StopSearch();
        }

        public void Timer_Search_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Este timer se desactiva siempre al entrar y se debe hacer un start siempre qu se salga
            bool bRestartTimer = true;
            string sError = "";

            lock (m_LockTimer)
            {
                StatusConnect StatusConnect_New = new StatusConnect();
                IPEndPoint NumEndPoint = default(IPEndPoint);

                switch (m_StatusConnect)
                {
                    case StatusConnect.StopSearch:
                        // si se solicitó un puerto
                        if (!string.IsNullOrEmpty(EndPointSearch))
                        {
                            bRestartTimer = false;
                        }
                        break;

                    case StatusConnect.WaitSearch:
                        // iRefreshUDPCount = -1 Detenida la actualización
                        if (iRefreshUDPCount >= 0)
                        {
                            iRefreshUDPCount++;
                        }
                        if (iRefreshUDPCount >= iRefreshUDPinWaitSearch || bRefreshUDPForce)
                        {
                            myRefreshStationList();
                            iRefreshUDPCount = 0;
                        }

                        IndexStation = discoveredStations.Number - 1;

                        StatusConnect_New = StatusConnect.Search;
                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                        break;

                    case StatusConnect.Search:
                        try
                        {
                            if (!string.IsNullOrEmpty(EndPointSearch))
                            {
                                string[] aIPPort = EndPointSearch.Split(':');
                                WinSockIn = new RoutinesLibrary.Net.Protocols.TCP.TCP(new IPEndPoint(IPAddress.Parse(aIPPort[0].Trim()), System.Convert.ToInt32(ushort.Parse(aIPPort[1].Trim()))));
                                WinSockIn.ClosedConnectionTCP += new RoutinesLibrary.Net.Protocols.TCP.TCP.ClosedConnectionTCPEventHandler(WinSockIn_FinishConnection);
                                WinSockIn.DataReceived += new RoutinesLibrary.Net.Protocols.TCP.TCP.DataReceivedEventHandler(WinSockIn_DataReceived);
                                if (WinSockIn.Connect(ref sError))
                                {
                                    m_Timer_Search.Interval = MS_WAIT_NAK;
                                    StatusConnect_New = StatusConnect.WaitNAKorHS;
                                    m_FrameDataIn01.Reset(); // preparar buffer de entrada
                                    m_FrameDataOut01.Reset(); // preparar buffer de salida
                                    m_FrameDataIn02.Reset(); // preparar buffer de entrada
                                    m_FrameDataOut02.Reset(); // preparar buffer de salida
                                    m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_undefined;
                                }
                                else
                                {
                                    //Error. Se prepara para una nueva busqueda
                                    StatusConnect_New = StatusConnect.Search;
                                    if (m_Timer_Search != null)
                                    {
                                        m_Timer_Search.Interval = MS_NEW_SEARCH; //Genera un nuevo evento lo antes posible
                                    }
                                    CloseWinSock(ref WinSockIn); // se elimina el puerto creado
                                }

                            }
                            else if (IndexStation < 0)
                            {
                                StatusConnect_New = StatusConnect.WaitSearch;
                                m_Timer_Search.Interval = MS_WAIT_SEARCH;
                            }
                            else
                            {
                                // define the next indexstation
                                while (IndexStation >= 0)
                                {
                                    if (discoveredStations.get_Connected(IndexStation))
                                    {
                                        IndexStation--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                if (IndexStation < 0)
                                {
                                    StatusConnect_New = StatusConnect.WaitSearch;
                                    m_Timer_Search.Interval = MS_WAIT_SEARCH;
                                }
                                else
                                {
                                    NumEndPoint = discoveredStations.GetStation(IndexStation).StationData.IPEndPointValue;
                                    WinSockIn = new RoutinesLibrary.Net.Protocols.TCP.TCP(new IPEndPoint(NumEndPoint.Address, System.Convert.ToInt32((ushort)NumEndPoint.Port)));
                                    WinSockIn.ClosedConnectionTCP += new RoutinesLibrary.Net.Protocols.TCP.TCP.ClosedConnectionTCPEventHandler(WinSockIn_FinishConnection);
                                    WinSockIn.DataReceived += new RoutinesLibrary.Net.Protocols.TCP.TCP.DataReceivedEventHandler(WinSockIn_DataReceived);
                                    if (WinSockIn.Connect(ref sError))
                                    {
                                        //Debug.Print("SearchTCP WinSockIn.Connect true: " & NumEndPoint.ToString)
                                        m_Timer_Search.Interval = MS_WAIT_NAK;
                                        StatusConnect_New = StatusConnect.WaitNAKorHS;
                                        m_FrameDataIn01.Reset(); // preparar buffer de entrada
                                        m_FrameDataOut01.Reset(); // preparar buffer de salida
                                        m_FrameDataIn02.Reset(); // preparar buffer de entrada
                                        m_FrameDataOut02.Reset(); // preparar buffer de salida
                                        m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_undefined;
                                    }
                                    else
                                    {
                                        //Debug.Print("SearchTCP WinSockIn.Connect false: " & NumEndPoint.ToString)
                                        //Error. Se prepara para una nueva busqueda
                                        StatusConnect_New = StatusConnect.Search;
                                        if (m_Timer_Search != null)
                                        {
                                            m_Timer_Search.Interval = MS_NEW_SEARCH; //Genera un nuevo evento lo antes posible
                                        }
                                        CloseWinSock(ref WinSockIn); // se elimina el puerto creado
                                    }
                                    IndexStation--;
                                }

                            }
                        }
                        catch (Exception)
                        {
                            //Debug.Print("SearchTCP Exception in Search: " & ex.Message)
                            //Error. Se prepara para una nueva busqueda
                            StatusConnect_New = StatusConnect.Search;
                            if (m_Timer_Search != null)
                            {
                                m_Timer_Search.Interval = MS_NEW_SEARCH; //Genera un nuevo evento lo antes posible
                            }
                            CloseWinSock(ref WinSockIn); // se elimina el puerto creado
                            IndexStation--;
                        }
                        break;

                    case StatusConnect.WaitNAKorHS:
                    case StatusConnect.WaitFW:
                    case StatusConnect.WaitHS:
                    case StatusConnect.WaitACK:
                    case StatusConnect.WaitNum:
                        //TimeOut. Se prepara para una nueva busqueda
                        //Debug.Print("SearchTCP TimeOut en StatusConnect: " & StatusConnect.ToString)
                        StatusConnect_New = StatusConnect.Search;
                        m_Timer_Search.Interval = MS_NEW_SEARCH; //Genera un nuevo evento lo antes posible
                        CloseWinSock(ref WinSockIn); // se elimina el puerto creado

                        // si se solicitó una dirección específica, reintentar y luego parar
                        if (!string.IsNullOrEmpty(EndPointSearch))
                        {
                            if (iReintentosSobreEndPointSolicitado >= MAX_REINTENTOS_SOBRE_ENDPOINT_SOLICITADO)
                            {
                                if (NoConexionEvent != null)
                                    NoConexionEvent(EndPointSearch);
                                m_StatusConnect = StatusConnect.StopSearch;
                                // no rearranca el timer
                                bRestartTimer = false;
                            }
                            else
                            {
                                iReintentosSobreEndPointSolicitado++;
                            }
                        }
                        else
                        {
                            IndexStation--;
                        }
                        break;
                }

                m_reintentos += 1;
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

        private void WinSockIn_DataReceived(byte[] _DataIn)
        {
            lock (m_LockParseDataReceived)
            {
                m_DataIn = _DataIn;
                ParseDataReceived();
            }
        }

        protected override void SendBytes(byte[] _bytes)
        {
            WinSockIn.SendData(_bytes);
        }

        protected override byte[] ReadBytes(int numBytes = 0)
        {
            if (numBytes < 1 | numBytes > m_DataIn.Length)
            {
                return m_DataIn;
            }

            byte[] retBytes = new byte[numBytes];
            Array.Copy(m_DataIn, retBytes, retBytes.Length);

            return retBytes;
        }

        protected override void RaiseNewConnection(CStationBase.Protocol commandProtocol, string sStationModel, string SoftwareVersion, string HardwareVersion)
        {
            // Cambia de referencia al puerto porque la anterior ya ha sido asignada
            RoutinesLibrary.Net.Protocols.TCP.TCP WinSock_Connected = WinSockIn;
            WinSockIn = null;

            //Cuando una estacion con un k60 esta apagada pero el usb esta conectado, sigue alimentandose.
            if (CheckStationModel(sStationModel))
            {
                //Genera el evento Nueva conexion, de protocolo de conexión/trama 02
                CConnectionData connectionData = new CConnectionData();
                connectionData.Mode = SearchMode.ETH;
                connectionData.pWinSock = WinSock_Connected;
                connectionData.PCNumDevice = m_pcNumDevice;
                connectionData.StationNumDevice = m_stationNumDevice;
                connectionData.FrameProtocol = m_Conection_FrameProtocol;
                connectionData.CommandProtocol = commandProtocol;
                connectionData.StationModel = sStationModel;
                connectionData.SoftwareVersion = SoftwareVersion;
                connectionData.HardwareVersion = HardwareVersion;

                if (NewConnectionEvent != null)
                    NewConnectionEvent(ref connectionData);
            }
            else
            {
                CloseWinSock(ref WinSock_Connected);
                m_Timer_Search.Stop();
                m_Timer_Search.Interval = MS_NEW_SEARCH;
                m_Timer_Search.Start();
            }
        }

        /// <summary>
        /// Metodo para cerrar la conexión
        /// </summary>
        /// <remarks></remarks>
        protected override void CloseConnection()
        {
            CloseWinSock(ref WinSockIn);
        }

        private void Event_NewConnection(ref CConnectionData connectionData)
        {
            // marca como conectado
            //Debug.Print("SearchTCP - NuevaConexion evento propio de SearchTCP")
            int idx = discoveredStations.ExistsStation(connectionData.pWinSock.HostEndPoint);
            if (idx >= 0)
            {
                discoveredStations.set_Connected(idx, true);
            }
        }

        private void myRefreshStationList()
        {
            IPEndPoint NumEndPoint = default(IPEndPoint);

            // set all stations not discovered
            for (int i = 0; i <= discoveredStations.GetTable.Count - 1; i++)
            {
                discoveredStations.set_Discovered(i, false);
            }

            // add or set discovered stations
            CStationConnectionData[] UDPStations = searchStationsUPD.GetDiscoveredStations();
            int idx = 0;
            foreach (CStationConnectionData stnData in UDPStations)
            {
                NumEndPoint = stnData.IPEndPointValue;
                idx = discoveredStations.ExistsStation(NumEndPoint);
                if (idx >= 0)
                {
                    discoveredStations.set_Discovered(idx, true);
                }
                else
                {
                    discoveredStations.AddStation(NumEndPoint, stnData);
                }
            }

            // delete stations not in discovered table
            for (int i = discoveredStations.GetTable.Count - 1; i >= 0; i--)
            {
                if (discoveredStations.get_Discovered(i) == false)
                {
                    discoveredStations.RemoveStation(i);
                }
            }

            iRefreshUDPCount = 0;
        }

        private void WinSockIn_FinishConnection()
        {
            //Error al recibir en SearchTCP. Se prepara para una nueva busqueda
            //Debug.Print("CloseConnectionTCP evento de WinSockIn en SearchTCP")
            m_StatusConnect = StatusConnect.Search;
            if (m_Timer_Search != null)
            {
                m_Timer_Search.Interval = MS_NEW_SEARCH; //Genera un nuevo evento lo antes posible
            }
            CloseWinSock(ref WinSockIn); // se elimina el puerto creado
        }

        private void searchStationsUPD_Refresh()
        {
            bRefreshUDPForce = true;
        }

        private void CloseWinSock(ref RoutinesLibrary.Net.Protocols.TCP.TCP _WinSock)
        {
            try
            {
                if (_WinSock != null)
                {
                    // marca como no conectado
                    int idx = discoveredStations.ExistsStation(_WinSock.HostEndPoint);
                    if (idx >= 0)
                    {
                        //Debug.Print("SearchTCP - CloseWinSock - marca como no conectada")
                        discoveredStations.set_Connected(idx, false);
                        //discoveredStations.StationWinSock(idx) = Nothing
                    }
                    _WinSock.Dispose();
                    _WinSock = null;
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
