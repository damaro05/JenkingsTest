using System.Threading;

namespace JBCStationControllerSrv
{
    class CStationWorkingEvent
    {
        private const int UPDATE_EVENT = 500;

        private Thread m_ThreadWorkingEvent;
        private bool m_WorkingStations = false;

        #region "Events"

        public delegate void StationWorkingEventHandler(bool working);
        private StationWorkingEventHandler StationWorkingEvent;

        public event StationWorkingEventHandler StationWorking {
            add {
                StationWorkingEvent = (StationWorkingEventHandler)System.Delegate.Combine(StationWorkingEvent, value);
            }
            remove {
                StationWorkingEvent = (StationWorkingEventHandler)System.Delegate.Remove(StationWorkingEvent, value);
            }
        }

        #endregion

        public CStationWorkingEvent()
        {
            m_ThreadWorkingEvent = new Thread(new System.Threading.ThreadStart(SearchWorkingStations));
            m_ThreadWorkingEvent.Start();
        }

        private void NotifyStationWorking(bool stationWorking)
        {
            if(m_WorkingStations != stationWorking)
            {
                m_WorkingStations = stationWorking;
                if(StationWorkingEvent != null)
                {
                    StationWorkingEvent(m_WorkingStations);
                }
            }
        }

        private void SearchWorkingStations()
        {
            do
            {
                bool stationWorking = false;
                string[] stationList = DLLConnection.jbc.GetStationList();

                foreach (string station in stationList)
                {
                    if (DLLConnection.jbc.GetStationConnectionType(station) != "U") continue;
                    if (DLLConnection.jbc.GetStationType(station) != DataJBC.eStationType.SOLD) continue;

                    for (int i = 0; i < DLLConnection.jbc.GetPortCount(station) && !stationWorking; i++)
                    {
                        if (DLLConnection.jbc.GetPortToolID(station, (DataJBC.Port)i) != DataJBC.GenericStationTools.NO_TOOL)
                        {
                            stationWorking = DLLConnection.jbc.GetPortToolHibernationStatus(station, (DataJBC.Port)i) == DataJBC.OnOff._OFF &
                                             DLLConnection.jbc.GetPortToolSleepStatus(station, (DataJBC.Port)i) == DataJBC.OnOff._OFF &
                                             DLLConnection.jbc.GetPortToolStandStatus(station, (DataJBC.Port)i) == DataJBC.OnOff._OFF;
                        }
                    }
                    if (stationWorking) break;
                }

                NotifyStationWorking(stationWorking);
                Thread.Sleep(UPDATE_EVENT);
            } while (true);
        }
    }
}
