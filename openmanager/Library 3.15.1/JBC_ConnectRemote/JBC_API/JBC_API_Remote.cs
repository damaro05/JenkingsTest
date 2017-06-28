// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;
using System.Threading.Tasks;
using System.Threading;
using JBC_ConnectRemote.JBCService;
using DataJBC;
using RoutinesJBC;
using OnOff = DataJBC.OnOff;
using SpeedContinuousMode = DataJBC.SpeedContinuousMode;
using Constants = DataJBC.Constants;

// 01/11/2014 Se cambia NT205 por NT105
// 17/02/2015 Se añade QueryEndedTransaction y se quita el evento TransactionFinished (hasta que se implemente callbacks en nuestro servicio WCF)
//            Se quita launchTransactionFinished


namespace JBC_ConnectRemote
{


    /// <summary>
    /// JBC stations comunications class. A general API with the necessary methods to manage
    /// JBC stations thru the network.
    /// </summary>
    /// <remarks></remarks>
    public class JBC_API_Remote
    {

        private const int TIMER_STATION_CONTROLLER_TIME_TO_LIFE = 4000;

        //Search
        private CSearchStations m_ServicesSearcher;
        
        //Stations
        private Hashtable m_stations = new Hashtable(); // CStationElement

        //Stations Controller
        private List<CStationControllerElement> m_stationControllerList = new List<CStationControllerElement>();
        private System.Timers.Timer m_TimerStationControllerTimeToLife;
        private object m_lockStaionControllerList = new object();


        /// <summary>
        /// This event is launched when a new station has been detected
        /// and correctly linked. Gives the identifier required in order to
        /// call the class methods.
        /// </summary>
        /// <param name="stationUUID" >The identifier of the new station connected</param>
        /// <remarks></remarks>
        public delegate void NewStationConnectedEventHandler(string stationUUID);
        private NewStationConnectedEventHandler NewStationConnectedEvent;

        public event NewStationConnectedEventHandler NewStationConnected
        {
            add
            {
                NewStationConnectedEvent = (NewStationConnectedEventHandler) System.Delegate.Combine(NewStationConnectedEvent, value);
            }
            remove
            {
                NewStationConnectedEvent = (NewStationConnectedEventHandler) System.Delegate.Remove(NewStationConnectedEvent, value);
            }
        }


        /// <summary>
        /// This event is launched when a identified station is disconnected or
        /// undetected. Gives the identifier of the disconnected station in order to
        /// manage the disconection in user application.
        /// </summary>
        /// <param name="stationUUID">An identifier to the disconnected station</param>
        /// <remarks></remarks>
        public delegate void StationDisconnectedEventHandler(string stationUUID);
        private StationDisconnectedEventHandler StationDisconnectedEvent;

        public event StationDisconnectedEventHandler StationDisconnected
        {
            add
            {
                StationDisconnectedEvent = (StationDisconnectedEventHandler) System.Delegate.Combine(StationDisconnectedEvent, value);
            }
            remove
            {
                StationDisconnectedEvent = (StationDisconnectedEventHandler) System.Delegate.Remove(StationDisconnectedEvent, value);
            }
        }


        public delegate void SwIncompatibleEventHandler();
        private SwIncompatibleEventHandler SwIncompatibleEvent;

        public event SwIncompatibleEventHandler SwIncompatible
        {
            add
            {
                SwIncompatibleEvent = (SwIncompatibleEventHandler) System.Delegate.Combine(SwIncompatibleEvent, value);
            }
            remove
            {
                SwIncompatibleEvent = (SwIncompatibleEventHandler) System.Delegate.Remove(SwIncompatibleEvent, value);
            }
        }


        public delegate void HostDiscoveredEventHandler(EndpointAddress hostEndPointAddress, string hostName);
        private HostDiscoveredEventHandler HostDiscoveredEvent;

        public event HostDiscoveredEventHandler HostDiscovered
        {
            add
            {
                HostDiscoveredEvent = (HostDiscoveredEventHandler) System.Delegate.Combine(HostDiscoveredEvent, value);
            }
            remove
            {
                HostDiscoveredEvent = (HostDiscoveredEventHandler) System.Delegate.Remove(HostDiscoveredEvent, value);
            }
        }


        public delegate void HostDisconnectedEventHandler(EndpointAddress hostEndPointAddress, string hostName);
        private HostDisconnectedEventHandler HostDisconnectedEvent;

        public event HostDisconnectedEventHandler HostDisconnected
        {
            add
            {
                HostDisconnectedEvent = (HostDisconnectedEventHandler) System.Delegate.Combine(HostDisconnectedEvent, value);
            }
            remove
            {
                HostDisconnectedEvent = (HostDisconnectedEventHandler) System.Delegate.Remove(HostDisconnectedEvent, value);
            }
        }


        /// <summary>
        /// When an error occurs this event is launched. Gives a Cerror object with
        /// the error information.
        /// </summary>
        /// <param name="stationUUID">An identifier to the station with error</param>
        /// <param name="err">The Cerror object with the error information</param>
        /// <remarks></remarks>
        public delegate void UserErrorEventHandler(string stationUUID, Cerror err);
        private UserErrorEventHandler UserErrorEvent;

        public event UserErrorEventHandler UserError
        {
            add
            {
                UserErrorEvent = (UserErrorEventHandler) System.Delegate.Combine(UserErrorEvent, value);
            }
            remove
            {
                UserErrorEvent = (UserErrorEventHandler) System.Delegate.Remove(UserErrorEvent, value);
            }
        }



        /// <summary>
        /// Creates the memory (use "StartSearch" to initiate searching connected stations).
        /// </summary>
        /// <remarks></remarks>
        public JBC_API_Remote()
        {
            m_TimerStationControllerTimeToLife = new System.Timers.Timer();
            m_TimerStationControllerTimeToLife.Elapsed += CheckStationControllerTimeToLife;
            m_TimerStationControllerTimeToLife.AutoReset = false;
            m_TimerStationControllerTimeToLife.Interval = TIMER_STATION_CONTROLLER_TIME_TO_LIFE;
            m_TimerStationControllerTimeToLife.Start();
        }

        /// <summary>
        /// Close and release resources and threads
        /// </summary>
        /// <remarks></remarks>
        public void Dispose()
        {
            // stop discover
            StopSearch();
            m_ServicesSearcher.Dispose();
            m_ServicesSearcher = null;
            m_ServicesSearcher.DiscoveredStation += Event_DiscoveredStation;
            m_ServicesSearcher.DiscoveredStationController += Event_DiscoveredStationController;

            ArrayList listStationsUUID = new ArrayList(m_stations.Keys);
            foreach (string stationUUID in listStationsUUID)
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.stack.Eraser();
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.stack = null;
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD = null;
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.stack.Eraser();
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.stack = null;
                    ((CStationElement) (m_stations[stationUUID])).Station_HA = null;
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.stack.Eraser();
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.stack = null;
                    ((CStationElement) (m_stations[stationUUID])).Station_SF = null;
                }

                m_stations.Remove(stationUUID);
            }
        }


#region API STATION METHODS

#region API General station methods

        /// <summary>
        /// Returns quantity of stations connected to all discovered JBC Station Controllers
        /// </summary>
        /// <remarks></remarks>
        public int GetStationCount()
        {
            int stnCount = 0;
            foreach (DictionaryEntry entry in m_stations)
            {
                if (((CStationElement) entry.Value).State == CStation.StationState.Connected)
                {
                    stnCount++;
                }
            }
            return stnCount;
        }

        /// <summary>
        /// Returns a list of stations IDs connected to all discovered JBC Station Controllers
        /// </summary>
        /// <remarks></remarks>
        public string[] GetStationList()
        {
            List<string> stns = new List<string>();
            foreach (DictionaryEntry entry in m_stations)
            {
                if (((CStationElement) entry.Value).State == CStation.StationState.Connected)
                {
                    stns.Add(((CStationElement) entry.Value).UUID);
                }
            }
            return stns.ToArray();
        }

        /// <summary>
        /// Close and reconnect station object.
        /// </summary>
        /// <param name="stationUUID">Station identifier</param>
        /// <remarks></remarks>
        public bool ResetStation(string stationUUID)
        {
            if (m_stations.Contains(stationUUID))
            {
                // send commend to reset station in station controller
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.ResetStation();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.ResetStation();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.ResetStation();
                }

                launchStationDisconnected(stationUUID);
                return true;
            }
            else
            {
                return false;
            }
        }

#endregion

#endregion


#region API STATION FEATURES
 
        /// <summary>
        /// Gets station features by Id.
        /// </summary>
        /// <param name="stationUUID">Station identifier</param>
        /// <remarks></remarks>
        public CFeaturesData GetStationFeatures(string stationUUID)
        {
            if (m_stations.Contains(stationUUID))
            {
                CFeaturesData feat = null;

                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    feat = ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures;
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    feat = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures;
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    feat = ((CStationElement) (m_stations[stationUUID])).Station_SF.GetFeatures;
                }

                CFeaturesData retFeat = (CFeaturesData) (feat.Clone());
                return retFeat;
            }
            else
            {
                return null;
            }
        }

#endregion


#region SEARCH MODULE

        /// <summary>
        /// Starts searching JBC Station Controllers with connected stations
        /// </summary>
        /// <remarks></remarks>
        public void StartSearch()
        {
            // initiate discover de WCF hosts services
            m_ServicesSearcher = new CSearchStations();
            m_ServicesSearcher.DiscoveredStation += Event_DiscoveredStation;
            m_ServicesSearcher.DiscoveredStationController += Event_DiscoveredStationController;
        }

        /// <summary>
        /// Stops searching JBC Station Controllers with connected stations
        /// </summary>
        /// <remarks></remarks>
        public void StopSearch()
        {
            // stop discover de WCF hosts services
            m_ServicesSearcher.StopSearch();
        }

#endregion


#region API Public Continuous mode :fet:
        /// <summary>
        /// This method activates or deactivates the continuous data transmision mode of the indicated station.
        /// The desired transmision speed ( period ) and at least one port must be indicated when activating.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="speed">The desired speed ( period ) for the transmisions</param>
        /// <param name="portA">First desired port to be monitorized</param>
        /// <param name="portB">Second desired port to be monitorized</param>
        /// <param name="portC">Third desired port to be monitorized</param>
        /// <param name="portD">Fourth desired port to be monitorized</param>
        /// <remarks></remarks>
        public async Task SetContinuousModeAsync(string stationUUID, SpeedContinuousMode speed, Port portA = default(Port), Port portB = default(Port), Port portC = default(Port), Port portD = default(Port))
        {

            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

            }
            else if (speed != SpeedContinuousMode.OFF & portA == Port.NO_PORT & portB == Port.NO_PORT & portC == Port.NO_PORT & portD == Port.NO_PORT)
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.CONTINUOUS_MODE_ON_WITHOUT_PORTS, "At least one port must be indicated when activating continuous mode"));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetContinuousModeAsync(speed, portA, portB, portC, portD);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetContinuousModeAsync(speed, portA, portB, portC, portD);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the current continuous data transmision mode status of the indicated station.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A tContinuousModeStatus object with the current status</returns>
        /// <remarks></remarks>
        public async Task<CContinuousModeStatus> GetContinuousModeAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetContinuousModeAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.GetContinuousModeAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return default(Task<CContinuousModeStatus>);
        }


        /// <summary>
        /// This method starts a new continuous data queue instance on the indicated station and returns an ID.
        /// The desired transmision speed ( period ) and ports will be the ones defined in SetContinuousMode Method.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A new queue id of the continuous mode to be used when retrieving the data</returns>
        /// <remarks></remarks>
        public async Task<uint> StartContinuousModeAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found"));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.StartContinuousModeAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.StartContinuousModeAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        public async Task StopContinuousModeAsync(string stationUUID, uint queueID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found"));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.StopContinuousModeAsync(queueID);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.StopContinuousModeAsync(queueID);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }

            }
        }


        /// <summary>
        /// Gets the current continuous mode data transmisions pending to be got from the internal FIFO queue of the indicated station.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>An integer that is the queue length</returns>
        /// <remarks></remarks>
        public int GetContinuousModeDataCount(string stationUUID, uint queueID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found"));
                return 0;
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetContinuousModeDataCount(queueID);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetContinuousModeDataCount(queueID);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the next continuous mode data transmison in the internal FIFO queue. It is the oldest transmision.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A tContinuousModeData object that is the oldest transmision in the queue</returns>
        /// <remarks></remarks>
        public stContinuousModeData_SOLD GetContinuousModeNextData(string stationUUID, uint queueID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetContinuousModeNextData(queueID);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the next continuous mode data transmison in the internal FIFO queue. It is the oldest transmision.
        /// HA desoldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A tContinuousModeData object that is the oldest transmision in the queue</returns>
        /// <remarks></remarks>
        public stContinuousModeData_HA GetContinuousModeNextData_HA(string stationUUID, uint queueID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetContinuousModeNextData(queueID);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the next chunk of continuous mode data from the queue of the JBC Station Controller and add them to the local internal FIFO queue.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="iChunk">Quantity of data to be read</param>
        /// <returns>Actual quantity read</returns>
        /// <remarks></remarks>
        public int UpdateContinuousModeNextDataChunk(string stationUUID, uint queueID, int iChunk)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateContiModeNextData(queueID, iChunk);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateContiModeNextData(queueID, iChunk);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        public async Task<int> UpdateContinuousModeNextDataChunkAsync(string stationUUID, uint queueID, int iChunk)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateContiModeNextDataAsync(queueID, iChunk);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateContiModeNextDataAsync(queueID, iChunk);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return default(Task<int>);
        }
#endregion

#region API General Orders :fet:

        /// <summary>
        /// This method restores the default values for all the station parameters.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <remarks></remarks>
        public async Task DefaultStationParametersAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetDefaultStationParamsAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetDefaultStationParamsAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetDefaultStationParamsAsync();
                }
            }
        }

        /// <summary>
        /// Indicates the station to raise a TransactionFinished event
        /// to know that all previous operations where done
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Transaction ID, to identify the transaction when receiving TransactionFinished event</returns>
        /// <remarks></remarks>
        public async Task<uint> SetTransactionAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetTransactionAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetTransactionAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetTransactionAsync();
                }
            }
            return default(Task<uint>);
        }

        /// <summary>
        /// Query the station if the transaction has finished
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Transaction ID, to identify the transaction when receiving TransactionFinished event</returns>
        /// <remarks></remarks>
        public async Task<bool> QueryEndedTransactionAsync(string stationUUID, uint transactionID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.QueryEndedTransactionAsync(transactionID);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.QueryEndedTransactionAsync(transactionID);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.QueryEndedTransactionAsync(transactionID);
                }
            }
            return default(Task<bool>);
        }

#endregion

#region API Station Info :fet:

        /// <summary>
        /// Retrieves from JBC Station Controller information data about the indicated station.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <remarks></remarks>
        public async Task<bool> UpdateStationInfoAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateStationInfoAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateStationInfoAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.UpdateStationInfoAsync();
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the name of the station controller machine that controls the station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the Host name."</returns>
        /// <remarks></remarks>
        public string GetStationHostName(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetHostName();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetHostName();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetHostName();
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the station type of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A value indication the station type</returns>
        /// <remarks></remarks>
        public eStationType GetStationType(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                return ((CStationElement) (m_stations[stationUUID])).StationType;
            }
            return eStationType.UNKNOWN;
        }

        /// <summary>
        /// Gets the COM port name or Ethernet address of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the COM port name or Ethernet address. Ex: "COM6" or "192.168.1.132"</returns>
        /// <remarks></remarks>
        public string GetStationCOM(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationCom();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationCom();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationCom();
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the type of connection: U = USB or E = Ethernet. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing U or E</returns>
        /// <remarks></remarks>
        public string GetStationConnectionType(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationConnectionType();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationConnectionType();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationConnectionType();
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the protocol version of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the protocol version.</returns>
        /// <remarks></remarks>
        public string GetStationProtocol(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationProtocol();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationProtocol();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationProtocol();
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the Model name of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the Model name.</returns>
        /// <remarks></remarks>
        public string GetStationModel(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationModel();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationModel();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationModel();
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the Model type of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the Model type.</returns>
        /// <remarks></remarks>
        public string GetStationModelType(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationModelType();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationModelType();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationModelType();
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the version of the Model type of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the Model type.</returns>
        /// <remarks></remarks>
        public int GetStationModelVersion(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationModelVersion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationModelVersion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationModelVersion();
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the Hardware version of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the Hardware version.</returns>
        /// <remarks></remarks>
        public string GetStationHWversion(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationHWversion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationHWversion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationHWversion();
                }
            }
            return "";
        }

        /// <summary>
        /// Gets the Software version of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the Software version.</returns>
        /// <remarks></remarks>
        public string GetStationSWversion(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationSWversion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationSWversion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationSWversion();
                }
            }
            return "";
        }

        public CFirmwareStation[] GetFirmwareVersion(string stationUUID)
        {
            CFirmwareStation[] retUpdateFirmware = null;

            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    retUpdateFirmware = ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFirmwareVersion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    retUpdateFirmware = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetFirmwareVersion();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    retUpdateFirmware = ((CStationElement) (m_stations[stationUUID])).Station_SF.GetFirmwareVersion();
                }
            }

            return retUpdateFirmware;
        }

        /// <summary>
        /// Gets the list of supported tools of the indicated station. If the station identifier
        /// is not correct an error event is thrown. Uses the GenericStationTools constants
        /// defined in this class.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A GenericStationTools vector with the supported tools.</returns>
        /// <remarks></remarks>
        public GenericStationTools[] GetStationTools(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationTools();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationTools();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationTools();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the number of ports of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>An integer which is the number of ports</returns>
        /// <remarks></remarks>
        public int GetPortCount(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.NumPorts;
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.NumPorts;
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.NumPorts;
                }
            }
            return 0;
        }
#endregion

#region API Station Status :fet:

        /// <summary>
        /// Retrieves from host status data about the indicated station.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <remarks></remarks>
        public async Task<bool> UpdateStationStatusAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateStationStatusAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateStationStatusAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.UpdateStationStatusAsync();
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the current control mode status of the indicated station.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A OnOff object that indicates the current status</returns>
        /// <remarks></remarks>
        public ControlModeConnection GetControlMode(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return null;
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetControlMode();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetControlMode();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetControlMode();
                }
            }
        }

        /// <summary>
        /// This method sets to ON or OFF the indicated station control mode.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="mode">The desired state for the control mode</param>
        /// <param name="userName">The user name for the control mode</param>
        /// <remarks></remarks>
        public void SetControlMode(string stationUUID, ControlModeConnection mode, string userName)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetControlModeAsync(mode, userName);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.SetControlModeAsync(mode, userName);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.SetControlModeAsync(mode, userName);
                }
            }
        }

        public string GetControlModeUserName(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetControlModeUserName();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetControlModeUserName();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetControlModeUserName();
                }
            }
            return null;
        }

        public void KeepControlMode(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.KeepControlMode();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.KeepControlMode();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.KeepControlMode();
                }
            }
        }

        /// <summary>
        /// Gets the current remote mode status of the indicated station.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A OnOff object that indicates the current status</returns>
        /// <remarks></remarks>
        public OnOff GetRemoteMode(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetRemoteMode();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetRemoteMode();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// This method sets to ON or OFF the indicated station remote mode.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="OnOff">The desired state for the remote mode</param>
        /// <remarks></remarks>
        public async void SetRemoteModeAsync(string stationUUID, OnOff OnOff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetRemoteModeAsync(OnOff);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetRemoteModeAsync(OnOff);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the current error code of the indicated station. If the station identifier
        /// is not correct an error event is thrown. Uses the cStationError constants defined in this
        /// class.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A cStationError object which is the error code.</returns>
        /// <remarks></remarks>
        public StationError GetStationError(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationError();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationError();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationError();
                }
            }
            return StationError.NO_ERROR;
        }

        /// <summary>
        /// Gets the current transformer temperature of the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// Uses the Ctemperature class defined in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Ctemperature object with the current transformer temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetStationTransformerTemp(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationTransformerTemp();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current transformer error temperature of the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// Uses the Ctemperature class defined in this
        /// library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Ctemperature object with the current transformer temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetStationTransformerErrorTemp(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationTransformerErrorTemp();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current MOSFET error temperature of the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// Uses the Ctemperature class defined in this
        /// library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Ctemperature object with the current transformer temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetStationMOSerrorTemp(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationMOSerrorTemp();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

#endregion

#region API Station Settings :fet:

        /// <summary>
        /// Retrieves from JBC Station Controller the station settings about the indicated station.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <remarks></remarks>
        public async Task<bool> UpdateStationSettingsAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateStationSettingsAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateStationSettingsAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.UpdateStationSettingsAsync();
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the name of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the station name.</returns>
        /// <remarks></remarks>
        public string GetStationName(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationName();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationName();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationName();
                }
            }
            return "";
        }

        /// <summary>
        /// Sets the indicated name to the indicated station. If the station identifier is incorrect or
        /// the indicated name is an empty string an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newName">The desired station name</param>
        /// <remarks></remarks>
        public async Task SetStationNameAsync(string stationUUID, string newName)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else if (newName.Length == 0)
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.INVALID_STATION_NAME, "Invalid name."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationNameAsync(newName);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationNameAsync(newName);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationNameAsync(newName);
                }
            }
        }

        /// <summary>
        /// Gets the PIN of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string containing the station PIN.</returns>
        /// <remarks></remarks>
        public string GetStationPIN(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationPIN();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationPIN();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationPIN();
                }
            }
            return "";
        }

        /// <summary>
        /// Sets the indicated PIN to the indicated station. If the station identifier is incorrect or
        /// the indicated PIN is an empty string an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newPIN">The desired new PIN</param>
        /// <remarks></remarks>
        public async Task SetStationPINAsync(string stationUUID, string newPIN)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else if (newPIN.Length != 4)
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.INVALID_STATION_PIN, "Invalid PIN."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationPINAsync(newPIN);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationPINAsync(newPIN);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationPINAsync(newPIN);
                }
            }
        }

        /// <summary>
        /// Gets the maximum temperature of the indicated station. If the station identifier
        /// is not correct an error event is thrown. Uses the Ctemperature class defined in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Ctemperature object with the maximum temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetStationMaxTemp(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationMaxTemp();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMaxTemp();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the indicated maximum temperature to the indicated station. If the station identifier is incorrect or
        /// the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newtemp">The desired new temperature</param>
        /// <remarks></remarks>
        public async Task SetStationMaxTempAsync(string stationUUID, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                CTemperature auxMaxTemp = new CTemperature();
                CTemperature auxMinTemp = new CTemperature();
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MinTemp.UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MinTemp.UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI))
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationMaxTempAsync(newTemp);
                    }
                    else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationMaxTempAsync(newTemp);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the minimum temperature of the indicated station. If the station identifier
        /// is not correct an error event is thrown. Uses the Ctemperature class defined in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Ctemperature object with the minimum temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetStationMinTemp(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationMinTemp();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMinTemp();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the indicated minimum temperature to the indicated station. If the station identifier is incorrect or
        /// the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newtemp">The desired new temperature</param>
        /// <remarks></remarks>
        public async Task SetStationMinTempAsync(string stationUUID, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                CTemperature auxMaxTemp = new CTemperature();
                CTemperature auxMinTemp = new CTemperature();
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MinTemp.UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MinTemp.UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI))
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationMinTempAsync(newTemp);
                    }
                    else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationMinTempAsync(newTemp);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the maximum external temperature of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// Uses the Ctemperature class defined in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Ctemperature object with the maximum temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetStationMaxExtTemp(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMaxExtTemp();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the indicated maximum external temperature to the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is incorrect or
        /// the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newtemp">The desired new temperature</param>
        /// <remarks></remarks>
        public async Task SetStationMaxExtTempAsync(string stationUUID, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                CTemperature auxMaxTemp = new CTemperature();
                CTemperature auxMinTemp = new CTemperature();
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.ExtTCMaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.ExtTCMinTemp.UTI);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI))
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationMaxExtTempAsync(newTemp);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the minimum external temperature of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// Uses the Ctemperature class defined in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A Ctemperature object with the minimum temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetStationMinExtTemp(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMinExtTemp();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the indicated minimum external temperature to the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is incorrect or
        /// the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newtemp">The desired new temperature</param>
        /// <remarks></remarks>
        public async Task SetStationMinExtTempAsync(string stationUUID, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                CTemperature auxMaxTemp = new CTemperature();
                CTemperature auxMinTemp = new CTemperature();
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.ExtTCMaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.ExtTCMinTemp.UTI);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI))
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationMinExtTempAsync(newTemp);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the maximum flow of the indicated station, in per thousand.
        /// Hot Air desoldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A value in per thousand.</returns>
        /// <remarks></remarks>
        public int GetStationMaxFlow(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMaxFlow();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Sets the indicated maximum flow to the indicated station, in per thousand.
        /// Hot Air desoldering stations only
        /// If the station identifier is incorrect an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newFlow">The desired new flow</param>
        /// <remarks></remarks>
        public async Task SetStationMaxFlowAsync(string stationUUID, int newFlow)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                int auxMaxFlow = 0;
                int auxMinFlow = 0;
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxFlow = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MaxFlow;
                    auxMinFlow = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MinFlow;
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newFlow > auxMaxFlow) || (newFlow < auxMinFlow))
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FLOW_OUT_OF_RANGE, "Flow out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationMaxFlowAsync(newFlow);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the minimum flow of the indicated station, in per thousand.
        /// Hot Air desoldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A value in per thousand.</returns>
        /// <remarks></remarks>
        public int GetStationMinFlow(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMinFlow();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Sets the indicated minimum flow to the indicated station, in per thousand.
        /// Hot Air desoldering stations only
        /// If the station identifier is incorrect an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newFlow">The desired new flow</param>
        /// <remarks></remarks>
        public async Task SetStationMinFlowAsync(string stationUUID, int newFlow)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                int auxMaxFlow = 0;
                int auxMinFlow = 0;
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxFlow = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MaxFlow;
                    auxMinFlow = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MinFlow;
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newFlow > auxMaxFlow) || (newFlow < auxMinFlow))
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FLOW_OUT_OF_RANGE, "Flow out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationMinFlowAsync(newFlow);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the temperature units of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A TemperatureUnits object with the temperature units.</returns>
        /// <remarks></remarks>
        public CTemperature.TemperatureUnit GetStationTempUnits(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationTempUnits();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationTempUnits();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return CTemperature.TemperatureUnit.Celsius;
        }

        /// <summary>
        /// Sets the indicated temperature units to the indicated station. If the station identifier is incorrect or
        /// the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newUnits">The desired temperature units.</param>
        /// <remarks></remarks>
        public async Task SetStationTempUnitsAsync(string stationUUID, CTemperature.TemperatureUnit newUnits)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationTempUnitsAsync(newUnits);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationTempUnitsAsync(newUnits);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the length units of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A LengthUnits object with the length units.</returns>
        /// <remarks></remarks>
        public CLength.LengthUnit GetStationLengthUnits(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationLengthUnits();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Sets the indicated length units to the indicated station. If the station identifier is incorrect or
        /// the indicated length is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newUnits">The desired length units.</param>
        /// <remarks></remarks>
        public async Task SetStationLengthUnitsAsync(string stationUUID, CLength.LengthUnit newUnits)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationLengthUnitsAsync(newUnits);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the N2 mode status of the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A OnOff object with the current N2 mode status</returns>
        /// <remarks></remarks>
        public OnOff GetStationN2Mode(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationN2Mode();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return OnOff._OFF;
        }

        /// <summary>
        /// Sets the indicated N2 mode status to the indicated station.
        /// Soldering stations only
        /// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newMode">The desired status for the N2 mode</param>
        /// <remarks></remarks>
        public async Task SetStationN2ModeAsync(string stationUUID, OnOff newMode)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationN2ModeAsync(newMode);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the Help Text status of the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A OnOff object with the current Help Text status</returns>
        /// <remarks></remarks>
        public OnOff GetStationHelpText(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationHelpText();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return OnOff._OFF;
        }

        /// <summary>
        /// Sets the indicated Help Text status to the indicated station.
        /// Soldering stations only
        /// If the station identifier is incorrect or
        /// the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newHelp ">The desired status for the Help Text</param>
        /// <remarks></remarks>
        public async Task SetStationHelpTextAsync(string stationUUID, OnOff newHelp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationHelpTextAsync(newHelp);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        // 13/01/2014 Se quita GetStationPowerLimit de la API y del Manager (se mantiene en el protocolo)
        // 01/04/2014  Se vuelve a habilitar GetStationPowerLimit
        /// <summary>
        /// Gets the power limit of the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>An integer object with the power linit in Watts units</returns>
        /// <remarks></remarks>
        public int GetStationPowerLimit(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationPowerLimit();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        // 13/01/2014  Se quita SetStationPowerLimit de la API y del Manager (se mantiene en el protocolo)
        // 01/04/2014  Se vuelve a habilitar SetStationPowerLimit
        /// <summary>
        /// Sets the power limit to the indicated station.
        /// Soldering stations only
        /// If the station identifier is incorrect an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="powerLimit">The desired power limit in Watts units.</param>
        /// <remarks></remarks>
        public async void SetStationPowerLimitAsync(string stationUUID, int powerLimit)
        {
            string model = System.Convert.ToString(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationModel());
            int max = 0;

            switch (model)
            {
                case "DD":
                case "DDR":
                    max = (int) PowerLimits.DD_MAX;
                    break;
                case "DM":
                    max = (int) PowerLimits.DM_MAX;
                    break;
                case "DI":
                    max = (int) PowerLimits.DI_MAX;
                    break;
                case "CD_CF":
                case "CD/CF":
                    max = (int) PowerLimits.CD_CF_MAX;
                    break;
            }

            powerLimit = System.Convert.ToInt32((powerLimit / 10) * 10);

            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (powerLimit < (int) PowerLimits.MIN | (max != 0 & powerLimit > max))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.POWER_LIMIT_OUT_OF_RANGE, "Power limit out of range"));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationPowerLimitAsync(powerLimit);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the Beep status of the indicated station. If the station identifier
        /// is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A OnOff object with the current Beep status</returns>
        /// <remarks></remarks>
        public OnOff GetStationBeep(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationBeep();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationBeep();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationBeep();
                }
            }
            return OnOff._OFF;
        }

        /// <summary>
        /// Sets the indicated Beep status to the indicated station. If the station identifier is incorrect or
        /// the indicated temperature is out of range an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="beep ">The desired status for the Beep mode</param>
        /// <remarks></remarks>
        public async Task SetStationBeepAsync(string stationUUID, OnOff beep)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetStationBeepAsync(beep);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationBeepAsync(beep);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationBeepAsync(beep);
                }
            }
        }

        /// <summary>
        /// Gets the PIN Enabled status of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A OnOff object with the current status</returns>
        /// <remarks></remarks>
        public OnOff GetStationPINEnabled(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationPINEnabled();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationPINEnabled();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return OnOff._OFF;
        }

        /// <summary>
        /// Sets the indicated PIN Enabled status to the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is incorrect an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="value">The desired status</param>
        /// <remarks></remarks>
        public async Task SetStationPINEnabledAsync(string stationUUID, OnOff value)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationPINEnabledAsync(value);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationPINEnabledAsync(value);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the Station Locked status of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A OnOff object with the current status</returns>
        /// <remarks></remarks>
        public OnOff GetStationLocked(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationLocked();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationLocked();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return OnOff._OFF;
        }

        /// <summary>
        /// Sets the Station Locked status to the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier is incorrect an error is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="value">The desired status</param>
        /// <remarks></remarks>
        public async Task SetStationLockedAsync(string stationUUID, OnOff value)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetStationLockedAsync(value);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationLockedAsync(value);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        //Programs
        public CProgramDispenserData_SF GetStationProgram(string stationUUID, byte nbrProgram)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationProgram(nbrProgram);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public Task SetStationProgram(string stationUUID, byte nbrProgram, CProgramDispenserData_SF program)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationProgram(nbrProgram, program);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public byte GetStationSelectedProgram(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationSelectedProgram();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return 0;
        }

        public Task DeleteStationProgram(string stationUUID, byte nbrProgram)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.DeleteStationProgram(nbrProgram);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public byte[] GetStationConcatenateProgramList(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationConcatenateProgramList();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public async Task SetStationConcatenateProgramList(string stationUUID, byte[] programList)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetStationConcatenateProgramList(programList);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

#endregion

#region API Port Status :fet:

        /// <summary>
        /// Retrieves from JBC Station Controller the port status for the indicated station and port.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <remarks></remarks>
        public async Task<bool> UpdatePortStatusAsync(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdatePortStatus(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdatePortStatus(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.UpdatePortStatus(port);
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the tool connected to the indicated port of the indicated station. If the station or port
        /// identifiers are not correct an error event is thrown. Uses the GenericStationTools constants defined
        /// in this class.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A GenericStationTools object which is the tool identifier.</returns>
        /// <remarks></remarks>
        public GenericStationTools GetPortToolID(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return GenericStationTools.NO_TOOL;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolID(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolID(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolID(port);
                }
            }
            return GenericStationTools.NO_TOOL;
        }

        /// <summary>
        /// Gets the tool actual temperature of the indicated port of the indicated station. If the station or port
        /// identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
        /// in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A Ctemperature object with the tool actual temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolActualTemp(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return null;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range. Port: " + Port.ToString()));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolActualTemp(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolActualTemp(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the tool actual external temperature of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
        /// in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A Ctemperature object with the tool actual external temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolActualExtTemp(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return null;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range. Port: " + Port.ToString()));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolActualExtTemp(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the tool protection TC temperature of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
        /// in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A Ctemperature object with the temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolProtectionTCTemp(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return null;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range. Port: " + Port.ToString()));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolProtectionTCTemp(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the tool actual power of the indicated port of the indicated station. If the station or port
        /// identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer object with the tool actual power in per thousand units.</returns>
        /// <remarks></remarks>
        public int GetPortToolActualPower(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return 0;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolActualPower(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolActualPower(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the tool actual flow of the indicated port of the indicated station, in per thousand.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer with the tool actual flow in per thousand units.</returns>
        /// <remarks></remarks>
        public int GetPortToolActualFlow(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return 0;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolActualFlow(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the time to stop of the indicated port of the indicated station, in seconds.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer with the time to stop in seconds.</returns>
        /// <remarks></remarks>
        public int GetPortToolTimeToStopStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return 0;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolTimeToStop(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the tool current error code of the indicated port of the indicated station. If the station or port
        /// identifiers are not correct an error event is thrown. Uses the ToolError constants defined
        /// in this class.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A ToolError object which is the current error code of the tool.</returns>
        /// <remarks></remarks>
        public ToolError GetPortToolError(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolError(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolError(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return ToolError.NO_TOOL;
        }

        /// <summary>
        /// Gets the tool actual cartridge current of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer object with the tool actual cartridge current in mA.</returns>
        /// <remarks></remarks>
        private int GetPortToolCartridgeCurrent(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolCartridgeCurrent(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the tool actual MOSFET temperature of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
        /// in this library.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A Ctemperature object with the tool actual MOSFET temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolMOStemp(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolMOStemp(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the tool future mode of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A ToolFutureMode object with the tool future mode.</returns>
        /// <remarks></remarks>
        public ToolFutureMode GetPortToolFutureMode(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolFutureMode(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the tool remaining time for the future mode of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer which is the remaining time in seconds.</returns>
        /// <remarks></remarks>
        public int GetPortToolTimeToFutureMode(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolTimeToFutureMode(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the current stand mode status of the indicated port of the indicated station.
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status for the stand mode</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolStandStatus(string stationUUID, Port port)
        {
            // 12/03/2013 Added GetPortToolStandStatus - #edu#
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolStandStatus(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolStandStatus(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current sleep mode status of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status for the sleep mode</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolSleepStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current hibernation mode status of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status for the hibernation mode</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolHibernationStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolHibernationStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current extractor mode status of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status for the extractor mode</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolExtractorStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolExtractorStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current desolder mode status of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status for the desolder mode</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolDesolderStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolDesolderStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current pedal status of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolPedalStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolPedalStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the current pedal connected status of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolPedalConnected(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolPedalConnected(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the suction requested status of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolSuctionRequestedStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSuctionRequestedStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the suction status of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolSuctionStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSuctionStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the heater requested status of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolHeaterRequestedStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolHeaterRequestedStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the heater status of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolHeaterStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolHeaterStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the cooling status of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A OnOff object which is the current status</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolCoolingStatus(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolCoolingStatus(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        // set status

        /// <summary>
        /// This method sets to ON or OFF the indicated port of the indicated station stand status.
        /// Soldering stations only
        /// Depending on the sleep and hibernation delays will be one of those status.
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="OnOff">The desired state for the stand mode</param>
        /// <remarks></remarks>
        public async void SetPortToolStandStatusAsync(string stationUUID, Port port, OnOff OnOff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolStandStatusAsync(port, OnOff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// This method sets to ON or OFF the indicated port of the indicated station extractor mode.
        /// Soldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="OnOff">The desired state for the extractor mode</param>
        /// <remarks></remarks>
        public async void SetPortToolExtractorStatusAsync(string stationUUID, Port port, OnOff OnOff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolExtractorStatusAsync(port, OnOff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// This method sets to ON or OFF the indicated port of the indicated station desolder mode.
        /// Soldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="OnOff">The desired state for the desolder mode</param>
        /// <remarks></remarks>
        public async void SetPortToolDesolderStatusAsync(string stationUUID, Port port, OnOff OnOff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolDesolderStatusAsync(port, OnOff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// This method sets to ON or OFF the indicated port of the indicated station heater mode.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="OnOff">The desired state for the heater mode</param>
        /// <remarks></remarks>
        public async void SetPortToolHeaterStatusAsync(string stationUUID, Port port, OnOff OnOff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolHeaterStatusAsync(port, OnOff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// This method sets to ON or OFF the indicated port of the indicated station heater mode.
        /// Hot Air desoldering stations only
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="OnOff">The desired state for the heater mode</param>
        /// <remarks></remarks>
        public async void SetPortToolSuctionStatusAsync(string stationUUID, Port port, OnOff OnOff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolSuctionStatusAsync(port, OnOff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

#endregion

#region API Port+Tool Settings :fet:

        /// <summary>
        /// Retrieves from JBC Station Controller the port and tool settings for the indicated station.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <remarks></remarks>
        public async Task<bool> UpdatePortToolSettingsAsync(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdatePortToolSettingsAsync(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdatePortToolSettingsAsync(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the selected tool temperature of the indicated port of the indicated station. If the station or port
        /// identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A Ctemperature object with the current selected temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolSelectedTemp(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSelectedTemp(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSelectedTemp(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the selected tool temperature of the indicated port of the indicated station. If the station or port
        /// identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="newTemp">The desired new selected temperature.</param>
        /// <remarks></remarks>
        public async Task SetPortToolSelectedTempAsync(string stationUUID, Port port, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                CTemperature auxMaxTemp = new CTemperature();
                CTemperature auxMinTemp = new CTemperature();
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MinTemp.UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MaxTemp.UTI);
                    auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MinTemp.UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI))
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolSelectedTempAsync(port, newTemp);
                    }
                    else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolSelectedTempAsync(port, newTemp);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the selected tool external temperature of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A Ctemperature object with the current selected external temperature.</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolSelectedExtTemp(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSelectedExtTemp(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the selected tool external temperature of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="newTemp">The desired new selected externaltemperature.</param>
        /// <remarks></remarks>
        public async Task SetPortToolSelectedExtTempAsync(string stationUUID, Port port, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    CTemperature auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMaxExtTemp().UTI);
                    CTemperature auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMinExtTemp().UTI);
                    if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI))
                    {
                        if (UserErrorEvent != null)
                            UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                    }
                    else
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolSelectedExtTempAsync(port, newTemp);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the selected tool flow of the indicated port of the indicated station, in per thousand.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A integer with the current flow in per thousand.</returns>
        /// <remarks></remarks>
        public int GetPortToolSelectedFlow(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSelectedFlow(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Sets the selected tool flow of the indicated port of the indicated station, in per thousand.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="newFlow">The desired new selected flow, in per thousand.</param>
        /// <remarks></remarks>
        public async Task SetPortToolSelectedFlowAsync(string stationUUID, Port port, int newFlow)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    if ((newFlow > 1000) || (newFlow < 0))
                    {
                        if (UserErrorEvent != null)
                            UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FLOW_OUT_OF_RANGE, "Flow out of range."));
                    }
                    else
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolSelectedFlowAsync(port, newFlow);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the selected tool profile mode of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>The state of the profile mode</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolProfileMode(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolProfileMode(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the selected tool profile mode of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="onoff">The desired state for the profile mode</param>
        /// <remarks></remarks>
        public async Task SetPortToolProfileModeAsync(string stationUUID, Port port, OnOff onoff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolProfileMode(port, onoff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the tool fix temperature of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A Ctemperature object with the current fix temperature</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolFixTemp(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else if (((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.TempLevelsWithStatus == true)
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolFixTemp(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the tool fix temperature of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="newTemp">The desired new fix temperature</param>
        /// <remarks></remarks>
        public async Task SetPortToolFixTempAsync(string stationUUID, Port port, GenericStationTools tool, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else if (((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.TempLevelsWithStatus == true)
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    CTemperature auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MaxTemp.UTI);
                    CTemperature auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MinTemp.UTI);
                    if (((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI)) && newTemp.UTI != Constants.NO_FIXED_TEMP)
                    {
                        if (UserErrorEvent != null)
                            UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                    }
                    else
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolFixTempAsync(port, tool, newTemp);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Activates or deactivates the tool fix temperature of the indicated port of the indicated station.
        ///  Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="newOnOff">To deactivate fix temperature (Off). If set to On, temperature will be set to MAX temp. + MIN temp. / 2</param>
        /// <remarks></remarks>
        public async Task SetPortToolFixTempAsync(string stationUUID, Port port, GenericStationTools tool, OnOff newOnOff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else if (((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.TempLevelsWithStatus == true)
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    CTemperature newTemp = new CTemperature(System.Convert.ToInt32(System.Convert.ToInt32((int) TemperatureLimits.MAX_TEMP + TemperatureLimits.MIN_TEMP) / 2));
                    if (newOnOff == OnOff._OFF)
                    {
                        newTemp.UTI = Constants.NO_FIXED_TEMP;
                    }
                    else
                    {
                        // si ya tiene una temperatura y se pone a On, no hacer nada
                        if (((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolFixTemp(port, tool).UTI != Constants.NO_FIXED_TEMP)
                        {
                            return ;
                        }
                    }
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolFixTempAsync(port, tool, newTemp);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the tool selected temperature level of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A ToolTemperatureLevels object with the current selected temperature level</returns>
        /// <remarks></remarks>
        public ToolTemperatureLevels GetPortToolSelectedTempLevels(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSelectedTempLevels(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSelectedTempLevels(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets status of the selected temperature levels of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A OnOff object which is the current status for the selected temperature levels</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolSelectedTempLevelsEnabled(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
                return null;

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
                return null;

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));
                return null;

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSelectedTempLevelsEnabled(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSelectedTempLevelsEnabled(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Sets the tool temperature level of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="level">The desired new selected temperature level</param>
        /// <remarks></remarks>
        public async Task SetPortToolSelectedTempLevelsAsync(string stationUUID, Port port, GenericStationTools tool, 
                ToolTemperatureLevels level)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolSelectedTempLevelsAsync(port, tool, level);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolSelectedTempLevelsAsync(port, tool, level);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Activates or deactivates the tool temperature levels of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="onoff">On or Off value</param>
        /// <remarks></remarks>
        public async Task SetPortToolSelectedTempLevelsEnabledAsync(string stationUUID, Port port, GenericStationTools tool, 
                OnOff onoff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolSelectedTempLevelsEnabledAsync(port, tool, onoff);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolSelectedTempLevelsEnabledAsync(port, tool, onoff);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the tool temperature level of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="level">The identifier of the desired level</param>
        /// <returns>A Ctemperature object with the temperature of the level</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolTempLevel(string stationUUID, Port port, GenericStationTools tool, 
                ToolTemperatureLevels level)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolTempLevel(port, tool, level);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolTempLevel(port, tool, level);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the status of the temperature level of the indicated port of the indicated station. If the station or port
        /// identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="level">The identifier of the desired level</param>
        /// <returns>A OnOff object with status of the temperature level</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolTempLevelEnabled(string stationUUID, Port port, GenericStationTools tool, 
                ToolTemperatureLevels level)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolTempLevelEnabled(port, tool, level);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolTempLevelEnabled(port, tool, level);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the tool temperature level of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="level ">The identifier of the level</param>
        /// <param name="newTemp">The desired new temp for the level</param>
        /// <remarks></remarks>
        public async Task SetPortToolTempLevelAsync(string stationUUID, Port port, GenericStationTools tool, 
                ToolTemperatureLevels level, CTemperature newTemp)
        {
            // 20/03/2013 permits newTemp to be JBC_API.TEMP_LEVEL_OFF #edu#
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                CTemperature auxMaxTemp = new CTemperature();
                CTemperature auxMinTemp = new CTemperature();
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    // 13/08/2014 GetStationMaxTemp and GetStationMinTemp added to Range
                    auxMaxTemp.UTI = Math.Min(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MaxTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationMaxTemp().UTI);
                    auxMinTemp.UTI = Math.Max(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MinTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationMinTemp().UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    auxMaxTemp.UTI = Math.Min(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MaxTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMaxTemp().UTI);
                    auxMinTemp.UTI = Math.Max(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MinTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMinTemp().UTI);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
                if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI) && newTemp.UTI != Constants.NO_TEMP_LEVEL)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                }
                else
                {
                    if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolTempLevelAsync(port, tool, level, newTemp);
                    }
                    else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolTempLevelAsync(port, tool, level, newTemp);
                    }
                }
            }
        }

        /// <summary>
        /// Activates or deactivates the tool temperature level of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="level ">The identifier of the level</param>
        /// <param name="onoff">The desired status for the level</param>
        /// <remarks></remarks>
        public async Task SetPortToolTempLevelEnabledAsync(string stationUUID, Port port, GenericStationTools tool, 
                ToolTemperatureLevels level, OnOff onoff)
        {
            // 20/03/2013 permits newTemp to be JBC_API.TEMP_LEVEL_OFF #edu#
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolTempLevelEnabledAsync(port, tool, level, onoff);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolTempLevelEnabledAsync(port, tool, level, onoff);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Sets the selected level and temperature levels of the indicated port of the indicated station, at once.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="selectedLevelEnabled">Activates or deactivates levels</param>
        /// <param name="selectedLevel">Selected temperature level</param>
        /// <param name="level1Enabled">Activates or deactivates level 1</param>
        /// <param name="tempLevel1">Temperature for level 1</param>
        /// <param name="level2Enabled">Activates or deactivates level 2</param>
        /// <param name="tempLevel2">Temperature for level 2</param>
        /// <param name="level3Enabled">Activates or deactivates level 3</param>
        /// <param name="tempLevel3">Temperature for level 3</param>
        /// <remarks></remarks>
        public async Task SetPortToolLevelsAsync(string stationUUID, Port port, GenericStationTools tool, OnOff 
                selectedLevelEnabled, ToolTemperatureLevels selectedLevel, OnOff level1Enabled, CTemperature tempLevel1, OnOff level2Enabled, CTemperature tempLevel2, OnOff level3Enabled, CTemperature tempLevel3)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    // 13/08/2014 Check ranges
                    bool bOutOfRange = false;
                    CTemperature auxMaxTemp = new CTemperature(Math.Min(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MaxTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationMaxTemp().UTI));
                    CTemperature auxMinTemp = new CTemperature(Math.Max(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MinTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationMinTemp().UTI));
                    // level 1 temp
                    if ((tempLevel1.UTI > auxMaxTemp.UTI) || (tempLevel1.UTI < auxMinTemp.UTI) && tempLevel1.UTI != Constants.NO_TEMP_LEVEL)
                    {
                        bOutOfRange = true;
                    }
                    // level 2 temp
                    if ((tempLevel2.UTI > auxMaxTemp.UTI) || (tempLevel2.UTI < auxMinTemp.UTI) && tempLevel2.UTI != Constants.NO_TEMP_LEVEL)
                    {
                        bOutOfRange = true;
                    }
                    // level 3 temp
                    if ((tempLevel3.UTI > auxMaxTemp.UTI) || (tempLevel3.UTI < auxMinTemp.UTI) && tempLevel3.UTI != Constants.NO_TEMP_LEVEL)
                    {
                        bOutOfRange = true;
                    }
                    if (bOutOfRange)
                    {
                        if (UserErrorEvent != null)
                            UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                    }
                    else
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolLevelsAsync(port, tool, selectedLevelEnabled, selectedLevel, level1Enabled, tempLevel1,
                                level2Enabled, tempLevel2, level3Enabled, tempLevel3);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Sets the selected level and temperatures/flow levels of the indicated port of the indicated station, at once.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="selectedLevelEnabled">Activates or deactivates levels</param>
        /// <param name="selectedLevel">Selected temperature level</param>
        /// <param name="level1Enabled">Activates or deactivates level 1</param>
        /// <param name="tempLevel1">Temperature for level 1</param>
        /// <param name="flowLevel1">Flow for level 1</param>
        /// <param name="tempExtLevel1">External Temperature for level 1</param>
        /// <param name="level2Enabled">Activates or deactivates level 2</param>
        /// <param name="tempLevel2">Temperature for level 2</param>
        /// <param name="flowLevel2">Flow for level 2</param>
        /// <param name="tempExtLevel2">External Temperature for level 2</param>
        /// <param name="level3Enabled">Activates or deactivates level 3</param>
        /// <param name="tempLevel3">Temperature for level 3</param>
        /// <param name="flowLevel3">Flow for level 3</param>
        /// <param name="tempExtLevel3">External Temperature for level 3</param>
        /// <remarks></remarks>
        public async Task SetPortToolLevelsAsync(string stationUUID, Port port, GenericStationTools tool, OnOff 
                selectedLevelEnabled, ToolTemperatureLevels selectedLevel, OnOff level1Enabled, CTemperature tempLevel1, int flowLevel1, CTemperature tempExtLevel1, OnOff level2Enabled, CTemperature tempLevel2, int flowLevel2, CTemperature tempExtLevel2, OnOff level3Enabled, CTemperature tempLevel3, int flowLevel3, CTemperature tempExtLevel3)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    // 13/08/2014 Check ranges
                    bool bOutOfRange = false;
                    CTemperature auxMaxTemp = new CTemperature(Math.Min(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MaxTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMaxTemp().UTI));
                    CTemperature auxMinTemp = new CTemperature(Math.Max(((CStationElement) (m_stations[stationUUID])).Station_HA.GetFeatures.MinTemp.UTI, ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationMinTemp().UTI));
                    // level 1 temp
                    if ((tempLevel1.UTI > auxMaxTemp.UTI) || (tempLevel1.UTI < auxMinTemp.UTI) && tempLevel1.UTI != Constants.NO_TEMP_LEVEL)
                    {
                        bOutOfRange = true;
                    }
                    // level 2 temp
                    if ((tempLevel2.UTI > auxMaxTemp.UTI) || (tempLevel2.UTI < auxMinTemp.UTI) && tempLevel2.UTI != Constants.NO_TEMP_LEVEL)
                    {
                        bOutOfRange = true;
                    }
                    // level 3 temp
                    if ((tempLevel3.UTI > auxMaxTemp.UTI) || (tempLevel3.UTI < auxMinTemp.UTI) && tempLevel3.UTI != Constants.NO_TEMP_LEVEL)
                    {
                        bOutOfRange = true;
                    }
                    if (bOutOfRange)
                    {
                        if (UserErrorEvent != null)
                            UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                    }
                    else
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolLevelsAsync(port, tool, selectedLevelEnabled, selectedLevel,
                                level1Enabled, tempLevel1, flowLevel1, tempExtLevel1,
                                level2Enabled, tempLevel2, flowLevel2, tempExtLevel2,
                                level3Enabled, tempLevel3, flowLevel3, tempExtLevel3);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the tool sleep delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A ToolTimeSleep with the current sleep delay</returns>
        /// <remarks></remarks>
        public ToolTimeSleep GetPortToolSleepDelay(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepDelay(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the status of the tool sleep delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A OnOff with the current status of the sleep delay</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolSleepDelayEnabled(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepDelayEnabled(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the tool sleep delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="delay">The desired delay for the tool</param>
        /// <remarks></remarks>
        public async Task SetPortToolSleepDelayAsync(string stationUUID, Port port, GenericStationTools tool, ToolTimeSleep delay)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolSleepDelayAsync(port, tool, delay);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Activates or deactivates the tool sleep delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="onoff">Activates or deactivates the sleep delay for the tool</param>
        /// <remarks></remarks>
        public async Task SetPortToolSleepDelayEnabledAsync(string stationUUID, Port port, GenericStationTools tool, OnOff onoff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolSleepDelayEnabledAsync(port, tool, onoff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the tool sleep temperature of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A Ctemperature object with the sleep temperature</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolSleepTemp(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepTemp(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the tool sleep temperature of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="newTemp">The desired temperature for the sleep status</param>
        /// <remarks></remarks>
        public async Task SetPortToolSleepTempAsync(string stationUUID, Port port, GenericStationTools tool, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    CTemperature auxMaxTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MaxTemp.UTI);
                    CTemperature auxMinTemp = new CTemperature(((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetFeatures.MinTemp.UTI);
                    if ((newTemp.UTI > auxMaxTemp.UTI) || (newTemp.UTI < auxMinTemp.UTI))
                    {
                        if (UserErrorEvent != null)
                            UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
                    }
                    else
                    {
                        await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolSleepTempAsync(port, tool, newTemp);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the tool hibernation delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A ToolTimeHibernation with the current hibernation delay</returns>
        /// <remarks></remarks>
        public ToolTimeHibernation GetPortToolHibernationDelay(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolHibernationDelay(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets status of the tool hibernation delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A OnOff with the current stauts of the hibernation delay</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolHibernationDelayEnabled(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolHibernationDelayEnabled(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the tool hibernation delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="delay">The desired delay for the tool</param>
        /// <remarks></remarks>
        public async Task SetPortToolHibernationDelayAsync(string stationUUID, Port port, GenericStationTools tool, ToolTimeHibernation delay)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolHibernationDelayAsync(port, tool, delay);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Activates or deactivates the tool hibernation delay of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="onoff">Activates or deactivates the hibernation delay for the tool</param>
        /// <remarks></remarks>
        public async Task SetPortToolHibernationDelayEnabledAsync(string stationUUID, Port port, GenericStationTools tool, OnOff onoff)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolHibernationDelayEnabledAsync(port, tool, onoff);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the tool adjust temperature of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A Ctemperature object with the adjust temperature</returns>
        /// <remarks></remarks>
        public CTemperature GetPortToolAdjustTemp(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolAdjustTemp(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolAdjustTemp(port, tool);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the tool adjust temperature of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="newTemp">The desired temperature for the sleep status</param>
        /// <remarks></remarks>
        public async Task SetPortToolAdjustTempAsync(string stationUUID, Port port, GenericStationTools tool, CTemperature newTemp)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
                // 10/12/2012 Se cambia verificación de límites (antes MIN_TEMP Y MAX_TEMP) - Edu

            }
            else if (newTemp.UTI > Constants.DEFAULT_TEMP_AJUST | newTemp.UTI < (Constants.DEFAULT_TEMP_AJUST * -1))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolAdjustTempAsync(port, tool, newTemp);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolAdjustTempAsync(port, tool, newTemp);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the cartridge used in the tool for the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A cartridge number</returns>
        /// <remarks></remarks>
        public ushort GetPortToolCartridge(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolCartridge(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return default(ushort);
        }

        /// <summary>
        /// Returns if cartridge selection is active for the tool for the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A OnOff object with status of the cartridge number for the tool</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolCartridgeEnabled(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolCartridgeEnabled(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the cartridge used in the tool for the indicated port of the indicated station and activates or deactivates cartridge selection.
        /// Soldering sations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="cartridge">Cartridge data</param>
        /// <remarks></remarks>
        public async Task SetPortToolCartridgeAsync(string stationUUID, Port port, GenericStationTools tool, CCartridgeData cartridge)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPortToolCartridgeAsync(port, tool, cartridge);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the time to stop, in seconds, defined for tool for the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>Time to stop value in seconds</returns>
        /// <remarks></remarks>
        public int GetPortToolTimeToStop(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolTimeToStop(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Sets the time to stop for the tool for the indicated port of the indicated station, in seconds.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="value">Time to stop value, in seconds</param>
        /// <remarks></remarks>
        public async Task SetPortToolTimeToStopAsync(string stationUUID, Port port, GenericStationTools tool, int value)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolTimeToStopAsync(port, tool, value);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the external TC mode defined for tool for the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>External TC mode</returns>
        /// <remarks></remarks>
        public ToolExternalTCMode_HA GetPortToolExternalTCMode(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolExternalTCMode(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the external TC mode for the tool for the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="mode">External TC mode</param>
        /// <remarks></remarks>
        public async Task SetPortToolExternalTCModeAsync(string stationUUID, Port port, GenericStationTools tool, ToolExternalTCMode_HA mode)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolExternalTCModeAsync(port, tool, mode);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Gets the button start mode status, defined for tool for the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>On or off</returns>
        /// <remarks></remarks>
        public OnOff GetPortToolStartMode_ToolButton(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolStartMode_ToolButton(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the pedal action for the start mode status, defined for tool for the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>Pedal action</returns>
        /// <remarks></remarks>
        public PedalAction GetPortToolStartMode_PedalAction(string stationUUID, Port port, GenericStationTools tool)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolStartMode_PedalAction(port, tool);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the time to stop for the tool for the indicated port of the indicated station, in seconds.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="ToolButton">Tool starts when button is pressed</param>
        /// <param name="PedalAction">Tool starts when pedal action take effect</param>
        /// <remarks></remarks>
        public async Task SetPortToolStartModeAsync(string stationUUID, Port port, GenericStationTools tool, OnOff ToolButton, PedalAction PedalAction)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else if (!isToolSupported(stationUUID, tool))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetPortToolStartModeAsync(port, tool, ToolButton, PedalAction);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        public DispenserMode_SF GetPortDispenserMode(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortDispenserMode(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public async Task SetPortDispenserMode(string stationUUID, Port port, DispenserMode_SF dispenserMode, byte nbrProgram = 0)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetPortDispenserModeAsync(port, dispenserMode, nbrProgram);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        public CLength GetPortLength(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortLength(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public async Task SetPortLength(string stationUUID, Port port, CLength length)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetPortLengthAsync(port, length);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        public CSpeed GetPortSpeed(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortSpeed(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public async Task SetPortSpeed(string stationUUID, Port port, CSpeed speed)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetPortSpeedAsync(port, speed);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        public OnOff GetPortFeedingState(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortFeedingState(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

#endregion

#region API Counters :fet:

        /// <summary>
        /// Retrieves from JBC Station Controller the counters for the indicated station and port.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <remarks></remarks>
        public async Task<bool> UpdatePortCountersAsync(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdatePortCountersAsync(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdatePortCountersAsync(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.UpdatePortCountersAsync(port);
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the number of minutes connected of the indicated station.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>An integer object with the number of minutes the station is connected</returns>
        /// <remarks></remarks>
        public int GetStationPluggedMinutes(string stationUUID, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationPluggedMinutes();
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationPluggedMinutesPartial();
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationPluggedMinutes();
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationPluggedMinutesPartial();
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationPluggedMinutes();
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationPluggedMinutesPartial();
                    }
                }
            }
            return 0;
        }

        public long GetPortToolTinLength(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolTinLength(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolTinLengthPartial(port);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the number of minutes connected of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer object with the number of minutes the station is connected</returns>
        /// <remarks></remarks>
        public int GetPortToolPluggedMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolPluggedMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolPluggedMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolPluggedMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolPluggedMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolPluggedMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolPluggedMinutesPartial(port);
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the tool working time in minutes of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the working time in minutes</returns>
        /// <remarks></remarks>
        public int GetPortToolWorkMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolWorkMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolWorkMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolWorkMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolWorkMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolWorkMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolWorkMinutesPartial(port);
                    }
                }

            }
            return -1;
        }

        /// <summary>
        /// Gets the tool sleep time in minutes of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the sleep time in minutes</returns>
        /// <remarks></remarks>
        public int GetPortToolSleepMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the tool hibernation time in minutes of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the hibernation time in minutes</returns>
        /// <remarks></remarks>
        public int GetPortToolHibernationMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolHibernationMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolHibernationMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the tool idle (no tool) time in minutes of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the idle (no tool) time in minutes</returns>
        /// <remarks></remarks>
        public int GetPortToolIdleMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolIdleMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolIdleMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolIdleMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolIdleMinutesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolIdleMinutes(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolIdleMinutesPartial(port);
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the tool number of sleep cycles of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the number of cycles</returns>
        /// <remarks></remarks>
        public int GetPortToolSleepCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepCycles(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolSleepCyclesPartial(port);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the tool number of desolder cycles of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the number of cycles</returns>
        /// <remarks></remarks>
        public int GetPortToolDesolderCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolDesolderCycles(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortToolDesolderCyclesPartial(port);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the tool number of work cycles of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the number of cycles</returns>
        /// <remarks></remarks>
        public int GetPortToolWorkCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolWorkCycles(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolWorkCyclesPartial(port);
                    }
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolWorkCycles(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetPortToolWorkCyclesPartial(port);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the tool number of suction cycles of the indicated port of the indicated station.
        /// Hot Air desoldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>An integer that is the number of cycles</returns>
        /// <remarks></remarks>
        public int GetPortToolSuctionCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    if (counterType == CounterTypes.GLOBAL_COUNTER)
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSuctionCycles(port);
                    }
                    else
                    {
                        return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetPortToolSuctionCyclesPartial(port);
                    }
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return -1;
        }

        /// <summary>
        /// Resets all partial counters of the indicated port of the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <remarks></remarks>
        public void ResetPortToolStationPartialCounters(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else if (!isPortSupported(stationUUID, port))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.ResetPortToolMinutesPartial(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.ResetPortToolMinutesPartial(port);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.ResetPortToolMinutesPartial(port);
                }
            }
        }

#endregion

#region API Communications :fet:

        /// <summary>
        /// Retrieves from JBC Station Controller the ethernet configuration about the indicated station.
        /// Soldering statins only
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <remarks></remarks>
        public async Task<bool> UpdateEthernetConfigurationAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateEthernetConfigurationAsync();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return false;
        }

        /// <summary>
        /// Returns ethernet configuration about the indicated station.
        /// Soldering statins only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A CEthernetData object with ethernet configuration</returns>
        /// <remarks></remarks>
        public CEthernetData GetEthernetConfiguration(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetEthernetConfiguration();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the ethernet configuration of the indicated port of the indicated station. If the station
        /// Soldering statins only
        /// identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="ethernetData">The desired new ethernet configuration</param>
        /// <remarks></remarks>
        public async Task SetEthernetConfigurationAsync(string stationUUID, CEthernetData ethernetData)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetEthernetConfigurationAsync(ethernetData);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        /// <summary>
        /// Retrieves from JBC Station Controller the robot configuration about the indicated station.
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <remarks></remarks>
        public async Task<bool> UpdateRobotConfigurationAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateRobotConfigurationAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateRobotConfigurationAsync();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SF.UpdateRobotConfigurationAsync();
                }
            }
            return false;
        }

        /// <summary>
        /// Returns robot configuration about the indicated station.
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A CRobotData object with robot configuration</returns>
        /// <remarks></remarks>
        public CRobotData GetRobotConfiguration(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetRobotConfiguration();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetRobotConfiguration();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SF.GetRobotConfiguration();
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the robot configuration of the indicated port of the indicated station. If the station
        /// identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="robotData">The desired new robot configuration</param>
        /// <remarks></remarks>
        public async Task SetRobotConfigurationAsync(string stationUUID, CRobotData robotData)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetRobotConfigurationAsync(robotData);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetRobotConfigurationAsync(robotData);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SF.SetRobotConfigurationAsync(robotData);
                }
            }
        }

#endregion

#region API Peripheral :fet:

        /// <summary>
        /// Retrieves from JBC Station Controller the peripheral information about the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <remarks></remarks>
        public async Task<bool> UpdatePeripheralsAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdatePeripheralAsync();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return false;
        }

        /// <summary>
        /// Returns peripheral information about the indicated station.
        /// Soldering stations only
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A CPeripheralData object with peripherals information</returns>
        /// <remarks></remarks>
        public CPeripheralData[] GetPeripherals(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPeripherals();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves from JBC Station Controller the peripheral information about the indicated station and the indicated port.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="port">The identifier of the desired port</param>
        /// <returns>A CPeripheralData object with peripherals information</returns>
        /// <remarks></remarks>
        public CPeripheralData[] GetPortPeripheral(string stationUUID, Port port)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetPortPeripheral(port);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
            return null;
        }

        /// <summary>
        /// Sets the peripheral configuration of the indicated port of the indicated station.
        /// Soldering stations only
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="peripheralData">The desired new peripheral configuration</param>
        /// <remarks></remarks>
        public async Task SetPeripheralAsync(string stationUUID, CPeripheralData peripheralData)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_SOLD.SetPeripheralAsync(peripheralData);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

#endregion

#region API Profile

        public async Task<bool> UpdateProfilesAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateProfilesAsync();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return false;
        }

        public async Task<bool> UpdateSelectedProfileAsync(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateSelectedProfileAsync();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return false;
        }

        public int GetProfileCount(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetProfileCount();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return 0;
        }

        public byte[] GetProfile(string stationUUID, string profileName)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetProfile(profileName);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public async Task<bool> SetProfileAsync(string stationUUID, string profileName, byte[] profileData)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return await ((CStationElement) (m_stations[stationUUID])).Station_HA.SetProfileAsync(profileName, profileData);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return false;
        }

        public string GetSelectedProfile(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetSelectedProfile();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public string GetProfileName(string stationUUID, int profileIndex)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    return ((CStationElement) (m_stations[stationUUID])).Station_HA.GetProfileName(profileIndex);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }

            return null;
        }

        public async Task DeleteProfile(string stationUUID, string profileName)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.DeleteProfile(profileName);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

        public async Task SyncProfiles(string stationUUID)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));
            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    await ((CStationElement) (m_stations[stationUUID])).Station_HA.SyncProfiles();
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED,
                                    string.Format("Function '{0}' not supported in this station.", System.Reflection.MethodBase.GetCurrentMethod().Name)));
                }
            }
        }

#endregion

#region API Private catchers and launchers events

        /// <summary>
        /// This function handles the SearchServicesWCF class DiscoveredStation event.
        /// </summary>
        /// <param name="hostService">The host service detected</param>
        /// <param name="stationUUID">The station ID discovered</param>
        /// <remarks></remarks>
        private void Event_DiscoveredStation(ref JBCStationControllerServiceClient hostService, string stationUUID, eStationType hostStnType)
        {

            // 26/11/2015 Ver si ya está en la lista de estaciones
            if (!m_stations.Contains(stationUUID))
            {

                //Comprobamos si el station controller está detectado
                bool stationControllerExists = false;
                lock(m_lockStaionControllerList)
                {
                    for (int i = 0; i <= m_stationControllerList.Count - 1; i++)
                    {
                        if (m_stationControllerList[i].EndPoint == hostService.Endpoint.Address)
                        {
                            stationControllerExists = true;
                            break;
                        }
                    }
                }
                if (!stationControllerExists)
                {
                    return ;
                }

                //Creamos la nueva estación
                CStationElement station = new CStationElement();
                station.UUID = stationUUID;
                station.StationType = hostStnType;
                station.State = CStation.StationState.Connected;
                switch (hostStnType)
                {
                    case eStationType.SOLD:
                        station.Station_SOLD = new CStation_Sold(stationUUID, this, ref hostService);
                        break;
                    case eStationType.HA:
                        station.Station_HA = new CStation_HA(stationUUID, this, ref hostService);
                        break;
                    case eStationType.SF:
                        station.Station_SF = new CStation_SF(stationUUID, this, ref hostService);
                        break;
                }

                //Adding the new station to the list of stations
                m_stations.Add(stationUUID, station);

                //Launching the user event for new station detected
                if (NewStationConnectedEvent != null)
                    NewStationConnectedEvent(stationUUID);
            }
        }

        private void Event_DiscoveredStationController(ref JBCStationControllerServiceClient hostService)
        {
            try
            {
                dc_StationController_Info hostInfo = hostService.GetStationControllerInfo();

                if (CheckStationControllerVersionSoftware(hostService, hostInfo.SwVersion))
                {
                    bool bNewStationController = true;

                    //Check if exists and restart time to life count
                    lock(m_lockStaionControllerList)
                    {
                        for (int i = 0; i <= m_stationControllerList.Count - 1; i++)
                        {
                            if (m_stationControllerList[i].PCUID == hostInfo.PCUID)
                            {
                                m_stationControllerList[i].TimeToLife = CStationControllerElement.TIME_TO_LIFE;
                                bNewStationController = false;
                                break;
                            }
                        }

                        if (bNewStationController)
                        {
                            m_stationControllerList.Add(new CStationControllerElement(hostService.Endpoint.Address, hostInfo.PCName, hostInfo.PCUID));
                            if (HostDiscoveredEvent != null)
                                HostDiscoveredEvent(hostService.Endpoint.Address, hostInfo.PCName);
                        }
                    }
                }
                else
                {
                    if (SwIncompatibleEvent != null)
                        SwIncompatibleEvent();
                }

            }
            catch (Exception)
            {
                //TODO. write in event log
            }
        }

        private bool CheckStationControllerVersionSoftware(JBCStationControllerServiceClient hostService, string hostSw)
        {
            bool bOk = false;

            string[] aHostSw = hostSw.Split('.');
            string[] remoteSw = (new Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase()).Info.Version.ToString().Split('.');

            if (aHostSw.Length == 4 & remoteSw.Length == 4)
            {
                if (int.Parse(aHostSw[0]) == int.Parse(remoteSw[0]) && int.Parse(aHostSw[1]) == int.Parse(remoteSw[1]) && int.Parse(aHostSw[3]) == int.Parse(remoteSw[3]))
                {
                    bOk = true;
                }
            }

            return bOk;
        }

        private void CheckStationControllerTimeToLife(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock(m_lockStaionControllerList)
            {

                for (int i = m_stationControllerList.Count - 1; i >= 0; i--)
                {
                    m_stationControllerList[i].TimeToLife--;

                    if (m_stationControllerList[i].TimeToLife <= 0)
                    {
                        //Intentamos establecer comunicación con el Station Controller antes de eliminarlo
                        bool bStationControllerExists = true;
                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                        JBCStationControllerServiceClient JBC_StationControllerService = new JBCStationControllerServiceClient(binding, System.Convert.ToString(m_stationControllerList[i].EndPoint));
                        JBC_StationControllerService.Open();

                        try
                        {
                            JBC_StationControllerService.GetStationList();
                        }
                        catch (Exception)
                        {
                            bStationControllerExists = false;
                        }

                        if (bStationControllerExists)
                        {
                            m_stationControllerList[i].TimeToLife = CStationControllerElement.TIME_TO_LIFE;
                        }
                        else
                        {
                            if (HostDisconnectedEvent != null)
                                HostDisconnectedEvent(m_stationControllerList[i].EndPoint, m_stationControllerList[i].HostName);
                            m_stationControllerList.RemoveAt(i);
                        }
                    }
                }
            }

            m_TimerStationControllerTimeToLife.Start();
        }


        //Private Sub StationSearcherWCF_SwIncompatible() Handles m_ServicesSearcher.SwIncompatible
        //    RaiseEvent SwIncompatible()
        //End Sub

        /// <summary>
        /// Function used by Cstation class to launch the station disconnected
        /// event.
        /// </summary>
        /// <param name="stationUUID">The station disconnected ID</param>
        /// <remarks></remarks>
        internal void launchStationDisconnected(string stationUUID)
        {

            //Launching the event
            if (StationDisconnectedEvent != null)
                StationDisconnectedEvent(stationUUID);

            //Eliminating the station from the list and freeing resources
            //Only mark as Disconnected
            if (m_stations.Contains(stationUUID))
            {
                // mark as disconneted
                CStationElement strStation = (CStationElement) (m_stations[stationUUID]);
                strStation.State = CStation.StationState.Disconnected;
                m_stations[stationUUID] = strStation;
                // libera stack y objeto
                // 26/11/2015 habilitado
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.Eraser();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.Eraser();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.Eraser();
                }
                // Remove from list
                // 26/11/2015 habilitado
                m_stations.Remove(stationUUID);
            }
        }

        /// <summary>
        /// Function used by Cstation to launch user errors as Cerror.
        /// </summary>
        /// <param name="err">The Cerror class instance of the launched error</param>
        /// <remarks></remarks>
        internal void launchUserError(string stationUUID, Cerror err)
        {
            //Launching the event
            if (UserErrorEvent != null)
                UserErrorEvent(stationUUID, err);
        }

#endregion

#region API Private Methods :fet:

        private bool isToolSupported(string stationUUID, GenericStationTools tool)
        {
            //getting the supported tools if not already gotten
            GenericStationTools[] tools = null;

            if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
            {
                tools = ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationTools();
            }
            else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
            {
                tools = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationTools();
            }
            else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
            {
                tools = ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationTools();
            }

            //looking for the passed tool in the list
            bool found = false;
            int cnt = 0;
            while (cnt < tools.Length && !found)
            {
                if (tools[cnt] == tool)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            //returning the found value
            return found;
        }

        private bool isPortSupported(string stationUUID, Port port)
        {
            if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
            {
                if (port >= ((CStationElement) (m_stations[stationUUID])).Station_SOLD.NumPorts | port < 0)
                {
                    return false;
                }
            }
            else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
            {
                if (port >= ((CStationElement) (m_stations[stationUUID])).Station_HA.NumPorts | port < 0)
                {
                    return false;
                }
            }
            else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
            {
                if (port >= ((CStationElement) (m_stations[stationUUID])).Station_SF.NumPorts | port < 0)
                {
                    return false;
                }
            }
            return true;
        }

#endregion

#region API Update firmware

        public void UpdateStationsFirmware(string stationUUID, List<CFirmwareStation> stationList)
        {
            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SOLD.UpdateStationsFirmware(stationList);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_HA.UpdateStationsFirmware(stationList);
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    ((CStationElement) (m_stations[stationUUID])).Station_SF.UpdateStationsFirmware(stationList);
                }

            }
        }

        public List<string> GetStationListUpdating(string stationUUID)
        {
            List<string> stationListUpdating = new List<string>();

            if (!m_stations.Contains(stationUUID))
            {
                if (UserErrorEvent != null)
                    UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station ID not found."));

            }
            else
            {
                if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SOLD)
                {
                    stationListUpdating = ((CStationElement) (m_stations[stationUUID])).Station_SOLD.GetStationListUpdating();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.HA)
                {
                    stationListUpdating = ((CStationElement) (m_stations[stationUUID])).Station_HA.GetStationListUpdating();
                }
                else if (((CStationElement) (m_stations[stationUUID])).StationType == eStationType.SF)
                {
                    stationListUpdating = ((CStationElement) (m_stations[stationUUID])).Station_SF.GetStationListUpdating();
                }
            }

            return stationListUpdating;
        }

#endregion

    }

}
