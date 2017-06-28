using DataJBC;

namespace JBC_Connect
{
    internal class CSearchManager
    {

        private SearchDevicesUSB m_StationSearcherUSB = null;
        private CSearchDevicesTCP m_StationSearcherTCP = null;

        #region EVENTS

        //NewConnection
        internal delegate void NewConnectionEventHandler(ref CConnectionData connectionData);
        private NewConnectionEventHandler NewConnectionEvent;

        internal event NewConnectionEventHandler NewConnection
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

        #endregion


        /// <summary>
        /// Start searching connected stations.
        /// </summary>
        /// <param name="searchMode">Select the connection type</param>
        /// <remarks></remarks>
        internal void StartSearch(SearchMode searchMode = default(SearchMode))
        {
            switch (searchMode)
            {
                case SearchMode.ALL:
                    m_StationSearcherUSB = new SearchDevicesUSB();
                    m_StationSearcherUSB.NewConnection += StationSearcherUSB_NewConnection;
                    m_StationSearcherTCP = new CSearchDevicesTCP(null);
                    m_StationSearcherTCP.NewConnection += StationSearcherTCP_NewConnection;
                    break;

                case SearchMode.USB:
                    m_StationSearcherUSB = new SearchDevicesUSB();
                    m_StationSearcherUSB.NewConnection += StationSearcherUSB_NewConnection;
                    break;

                case SearchMode.ETH:
                    m_StationSearcherTCP = new CSearchDevicesTCP(null);
                    m_StationSearcherTCP.NewConnection += StationSearcherTCP_NewConnection;
                    break;
            }
        }

        /// <summary>
        /// Stop searching connected stations.
        /// </summary>
        /// <param name="searchMode">Select the connection type</param>
        /// <remarks></remarks>
        internal void StopSearch(SearchMode searchMode)
        {
            switch (searchMode)
            {
                case SearchMode.ALL:
                    if (m_StationSearcherUSB != null)
                    {
                        m_StationSearcherUSB.Eraser();
                        m_StationSearcherUSB.NewConnection -= StationSearcherUSB_NewConnection;
                        m_StationSearcherUSB = null;
                    }
                    if (m_StationSearcherTCP != null)
                    {
                        m_StationSearcherTCP.Dispose();
                        m_StationSearcherTCP.NewConnection -= StationSearcherTCP_NewConnection;
                        m_StationSearcherTCP = null;
                    }
                    break;

                case SearchMode.USB:
                    if (m_StationSearcherUSB != null)
                    {
                        m_StationSearcherUSB.Eraser();
                        m_StationSearcherUSB.NewConnection -= StationSearcherUSB_NewConnection;
                        m_StationSearcherUSB = null;
                    }
                    break;

                case SearchMode.ETH:
                    if (m_StationSearcherTCP != null)
                    {
                        m_StationSearcherTCP.Dispose();
                        m_StationSearcherTCP.NewConnection -= StationSearcherTCP_NewConnection;
                        m_StationSearcherTCP = null;
                    }
                    break;
            }
        }

        /// <summary>
        /// Ask if it is searching connected stations.
        /// </summary>
        /// <param name="searchMode">Select the connection type</param>
        /// <remarks></remarks>
        internal bool isSearching(SearchMode searchMode)
        {
            switch (searchMode)
            {
                case SearchMode.ALL:
                    return (m_StationSearcherUSB != null) && (m_StationSearcherTCP != null);

                case SearchMode.USB:
                    return m_StationSearcherUSB != null;

                case SearchMode.ETH:
                    return m_StationSearcherTCP != null;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Ask if it is searching using any search mode.
        /// </summary>
        /// <remarks></remarks>
        internal bool isSearching()
        {
            return (m_StationSearcherUSB != null) || (m_StationSearcherTCP != null);
        }

        private void StationSearcherUSB_NewConnection(ref CConnectionData connectionData)
        {
            if (NewConnectionEvent != null)
                NewConnectionEvent(ref connectionData);
        }

        private void StationSearcherTCP_NewConnection(ref CConnectionData connectionData)
        {
            if (NewConnectionEvent != null)
                NewConnectionEvent(ref connectionData);
        }

    }
}
