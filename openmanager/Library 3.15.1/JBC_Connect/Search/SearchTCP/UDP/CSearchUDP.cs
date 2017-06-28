using System;
using System.Collections;
using System.Net;
using System.Text;
using RoutinesJBC;

namespace JBC_Connect
{
    internal class CSearchUDP
    {
        private const int TIMER_SEARCH_INTERVAL = 5000;
        private const int TIMER_DELETE_CONNECT = (5 * TIMER_SEARCH_INTERVAL);

        // "JBC:01:DD:1234567:9876543:m:eth:PC01"
        private const int POS_CONNECTION_TEXT = 0;
        private const int POS_PROTOCOL_VERSION = 1;
        private const int POS_STATION_TYPE = 2;
        private const int POS_SOFT_VERSION = 3;
        private const int POS_HARD_VERSION = 4;
        private const int POS_CONN_TYPE = 5;
        private const int INC_CONN_TYPE = 0;
        private const int INC_CONN_STATUS = 1;
        private const int INC_CONN_NAME_PC = 2;
        private const int INC_CONN_NEXT = (INC_CONN_NAME_PC + 1);

        public enum EnumStado : int
        {
            NO_CONNECT,
            MONITOR_MODE,
            CONTROL_MODE
        }

        public enum EnumConnection : int
        {
            NONE,
            USB,
            ETH,
            ROBOT
        }

        private RoutinesLibrary.Net.Protocols.UDP.UDP m_SockUDP;
        private System.Timers.Timer m_Timer;
        private CStationHashtableUDP m_StationHashTable;
        private bool ChangeTable;
        private bool Searching = false;
        private object LockTimer = new object();
        private object LockReceived = new object();


        #region "Events"

        //RefreshEvent
        public delegate void RefreshEventHandler();
        private RefreshEventHandler RefreshEvent;

        public event RefreshEventHandler Refresh
        {
            add
            {
                RefreshEvent = (RefreshEventHandler)System.Delegate.Combine(RefreshEvent, value);
            }
            remove
            {
                RefreshEvent = (RefreshEventHandler)System.Delegate.Remove(RefreshEvent, value);
            }
        }

        #endregion


        public CSearchUDP()
        {
            m_SockUDP = new RoutinesLibrary.Net.Protocols.UDP.UDP(SearcherServicesData.PORT_STATION_DISCOVERY);
            m_SockUDP.DataReceived += new RoutinesLibrary.Net.Protocols.UDP.UDP.DataReceivedEventHandler(m_SockUDP_DataReceived);
            m_Timer = new System.Timers.Timer();
            m_Timer.Elapsed += m_Timer_Elapsed;
            m_Timer.AutoReset = false; // se debe activar de nuevo al salir del evento Elapsed
            m_Timer.Stop();
            m_StationHashTable = new CStationHashtableUDP();
            ChangeTable = false;
        }

        public void StartSearch()
        {
            m_SockUDP.Activate();
            m_Timer.Interval = TIMER_SEARCH_INTERVAL;
            m_Timer.Start();
            Searching = true;
        }

        public void StopSearch()
        {
            Searching = false;
            m_Timer.Stop();
            m_SockUDP.Deactivate();
        }

        public void Eraser()
        {
            Searching = false;
            StopSearch();
            m_Timer.Dispose();
            m_Timer = null;
            m_Timer.Elapsed += m_Timer_Elapsed;
            m_SockUDP.Dispose();
        }

        public bool IsSearching()
        {
            return Searching;
        }

        public CStationConnectionData[] GetDiscoveredStations()
        {
            CStationConnectionData[] Stations = null;
            int Index = 0;

            Stations = new CStationConnectionData[m_StationHashTable.Number];
            foreach (DictionaryEntry tableStation in m_StationHashTable.GetTable)
            {
                // No hace falta porque se guarda por duplicado
                //Stations(Index).IPEndPoint = CType(tableStation.Key, IPEndPoint)
                Stations[Index] = (CStationConnectionData)(((CStationInfoUDP)tableStation.Value).StationInfo.Clone());
                Index++;
            }
            return Stations;
        }

        private void m_Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (LockTimer)
            {
                bool Cont = false;
                // Continuamente se buscan estaciones JBC
                m_SockUDP.SendBroadcast(Encoding.ASCII.GetBytes(SearcherServicesData.MESSAGE_STATION_DISCOVERY_REQUEST), SearcherServicesData.PORT_STATION_DISCOVERY);
                // Chequea todas las estaciones
                do
                {
                    try
                    {
                        foreach (DictionaryEntry tableStation in m_StationHashTable.GetTable)
                        {
                            DateTime HoraActual = default(DateTime);
                            DateTime HoraLimite = default(DateTime);
                            // Hora actual
                            HoraActual = DateTime.Now;
                            CStationInfoUDP Station = (CStationInfoUDP)tableStation.Value;
                            IPEndPoint IDTerminal = (IPEndPoint)tableStation.Key;
                            HoraLimite = Station.HourCreated.AddMilliseconds(TIMER_DELETE_CONNECT);
                            if (HoraActual > HoraLimite)
                            {
                                // Ha superado el tiempo de espera sin que la estación responda, se ha de eliminar de la tabla
                                m_StationHashTable.RemoveStation(IDTerminal); //eliminamos la estación
                                ChangeTable = true;
                            }
                        }
                        Cont = false;
                    }
                    catch (Exception)
                    {
                        // Si se produce la excepción debe volverse a repetir ya que se ha quitado un elemento y pueden haber más
                        Cont = true;
                    }
                } while (Cont);
                // Cada vez que pasa el tiempo del timer se informa si han habido cambios en la tabla
                // tanto de añadir nuevas estaciones como de quitat estaciones
                // no se puede informar en "DataReceived" porque se podría activar el evento varias veces seguidad
                if (ChangeTable)
                {
                    ChangeTable = false;
                    if (RefreshEvent != null)
                        RefreshEvent();
                }
                m_Timer.Start(); //Este timer se desactiva siempre al entrar y se debe hacer un start siempre qu se salga
            }
        }

        private void m_SockUDP_DataReceived(byte[] Data, IPEndPoint IDTerminal)
        {
            lock (LockReceived)
            {
                CStationInfoUDP Station = default(CStationInfoUDP);
                string StringData = System.Text.Encoding.UTF8.GetString(Data);
                //Debug.Print("UDP data: " & StringData)
                string[] SplitStringData = StringData.Split(':');
                if (SplitStringData.Length >= (POS_CONN_TYPE + INC_CONN_NAME_PC + 1) && SplitStringData[POS_CONNECTION_TEXT].ToUpperInvariant() == SearcherServicesData.MESSAGE_STATION_DISCOVERY_REQUEST)
                {
                    int NumConnection = System.Convert.ToInt32((SplitStringData.Length - POS_CONN_TYPE) / INC_CONN_NEXT);
                    if (m_StationHashTable.ExistsStation(IDTerminal))
                    {
                        // Si ya existe se quita y se vuelve a añadir,
                        // para renovar su hora para el control de bajas de equipos
                        // y los datos de conexión actualizados
                        Station = m_StationHashTable.GetStation(IDTerminal);
                        m_StationHashTable.RemoveStation(IDTerminal);
                    }
                    else
                    {
                        // Si no existe crear la estación
                        Station = new CStationInfoUDP(NumConnection - 1);
                        Station.StationInfo.IPEndPointValue = IDTerminal;
                        Station.StationInfo.ProtocolVersion = SplitStringData[POS_PROTOCOL_VERSION];
                        Station.StationInfo.StationModel = SplitStringData[POS_STATION_TYPE];
                        Station.StationInfo.SoftVersion = SplitStringData[POS_SOFT_VERSION];
                        Station.StationInfo.HardVersion = SplitStringData[POS_HARD_VERSION];
                        ChangeTable = true;
                    }
                    // actualizar datos de conexión
                    int count = 0;
                    for (int index = POS_CONN_TYPE; index <= (NumConnection * INC_CONN_NEXT) + POS_CONN_TYPE - 1; index += INC_CONN_NEXT)
                    {
                        if (SplitStringData[index + INC_CONN_TYPE].Length > 0 &&
                            SplitStringData[index + INC_CONN_STATUS].Length == 1 &&
                            SplitStringData[index + INC_CONN_NAME_PC].Length > 0)
                        {
                            // Se comprueba una mínima de integridad
                            if (!(System.Enum.IsDefined(typeof(EnumConnection), SplitStringData[index + INC_CONN_TYPE].ToUpper())))
                            {
                                // Si no existe el tipo de conexión considerar no conectado
                                Array.Resize(ref Station.StationInfo.Connection, Station.StationInfo.Connection.Length - 2 + 1);
                                continue;
                            }

                            //"N" --> sin conexión; "M" --> conectado Monitor Mode; "C" --> conectado Control Mode
                            switch (char.ToUpper(System.Convert.ToChar(SplitStringData[index + INC_CONN_STATUS][0])))
                            {
                                case 'N':
                                    Station.StationInfo.Connection[count].Status = EnumStado.NO_CONNECT;
                                    Station.StationInfo.Connection[count].ConnectionType = (CSearchUDP.EnumConnection)((EnumConnection)(System.Enum.Parse(typeof(EnumConnection), SplitStringData[index + INC_CONN_TYPE].ToUpper())));
                                    Station.StationInfo.Connection[count].NamePC = "";
                                    break;
                                case 'M':
                                    Station.StationInfo.Connection[count].Status = EnumStado.MONITOR_MODE;
                                    Station.StationInfo.Connection[count].ConnectionType = (CSearchUDP.EnumConnection)((EnumConnection)(System.Enum.Parse(typeof(EnumConnection), SplitStringData[index + INC_CONN_TYPE].ToUpper())));
                                    Station.StationInfo.Connection[count].NamePC = SplitStringData[index + INC_CONN_NAME_PC];
                                    break;
                                case 'C':
                                    Station.StationInfo.Connection[count].Status = EnumStado.CONTROL_MODE;
                                    Station.StationInfo.Connection[count].ConnectionType = (CSearchUDP.EnumConnection)((EnumConnection)(System.Enum.Parse(typeof(EnumConnection), SplitStringData[index + INC_CONN_TYPE].ToUpper())));
                                    Station.StationInfo.Connection[count].NamePC = SplitStringData[index + INC_CONN_NAME_PC];
                                    break;
                                default:
                                    // esto no se ha de producir, si pasa no añadir nada
                                    Array.Resize(ref Station.StationInfo.Connection, Station.StationInfo.Connection.Length - 1);
                                    continue;
                            }
                        }
                        count++;
                    }
                    // Se guarda la estación detectada
                    m_StationHashTable.PutStation(IDTerminal, Station);
                }
            }
        }

    }
}
