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
using System.ServiceModel.Channels;
using JBCHostControllerSrv.JBCStationControllerService;


namespace JBCHostControllerSrv
{
    //Por defecto, WCF se instancia con concurrency=single. Esto quiere decir que las peticiones se resuelven una por una y no en paralelo.
    //InstanceContexMode=Single implica que una única instancia del servicio será creada para toda la vida del servicio. Esto permite
    //disponer de timers (pe, schedule updates) que solo son creados una única vez.
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single,
            InstanceContextMode = InstanceContextMode.Single)]
        public class CImplIJBCHostControllerService : IJBCHostControllerService
    {

        private CLocalData m_localData;
        private CUpdatesManager m_updatesManager;
        private CUpdatesFirmwareManager m_updaterFirmwareManager;
        private CEventLogRecorder m_eventLogRecorder;


#region HostController Service Management

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <remarks></remarks>
        public CImplIJBCHostControllerService()
        {
            m_localData = new CLocalData();
            m_updatesManager = new CUpdatesManager(m_localData);
            m_updaterFirmwareManager = new CUpdatesFirmwareManager(ref m_localData);
            m_eventLogRecorder = new CEventLogRecorder(m_localData);
        }

        public void Dispose()
        {
            m_localData.Dispose();
            m_updatesManager.Dispose();
            m_updaterFirmwareManager.Dispose();
        }

        public dc_HostController_Info GetHostControllerInfo()
        {
            dc_HostController_Info ret = new dc_HostController_Info();

            try
            {
                ret.PCName = Environment.MachineName;
                ret.PCUID = "";
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (string.IsNullOrEmpty(ret.PCUID))
                    {
                        ret.PCUID = System.Convert.ToString(mo.Properties["ProcessorId"].Value.ToString());
                    }
                }

            }
            catch (FaultException<faultError> faultEx)
            {
                LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
                throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
                throw (ExceptionsRoutines.getFaultEx(ref ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
            }

            return ret;
        }

#endregion


#region System Files

        public bool IsAvailableRemoteServerDownload()
        {
            return m_localData.IsAvailableRemoteServerDownload();
        }

        public bool SetAvailableRemoteServerDownload(bool active)
        {
            return m_localData.SetAvailableRemoteServerDownload(active);
        }

        public string GetUserFilesLocalFolderLocation()
        {
            return m_localData.GetUserFilesLocalFolderLocation();
        }

        public bool SetUserFilesLocalFolderLocation(string path)
        {
            return m_localData.SetUserFilesLocalFolderLocation(path);
        }

#endregion


#region Updates

#region Update operations

        /// <summary>
        /// Check if an update is available on the remote server
        /// </summary>
        /// <returns>Update information</returns>
        public dc_InfoUpdateSoftware CheckUpdate()
        {
            dc_InfoUpdateSoftware dcInfoUpdateSw = new dc_InfoUpdateSoftware();
            stSwVersion infoUpdateSw = m_updatesManager.GetInfoNewUpdate();

            dcInfoUpdateSw.lastUpdateDate = infoUpdateSw.lastUpdateDate;
            //Station Controller
            dcInfoUpdateSw.stationControllerSwAvailable = infoUpdateSw.stationControllerSwAvailable;
            dcInfoUpdateSw.stationControllerSwDate = infoUpdateSw.stationControllerSwDate;
            dcInfoUpdateSw.stationControllerSwVersion = infoUpdateSw.stationControllerSwVersion;
            //Remote Manager
            dcInfoUpdateSw.remoteManagerSwAvailable = infoUpdateSw.remoteManagerSwAvailable;
            dcInfoUpdateSw.remoteManagerSwDate = infoUpdateSw.remoteManagerSwDate;
            dcInfoUpdateSw.remoteManagerSwVersion = infoUpdateSw.remoteManagerSwVersion;
            //Host Controller
            dcInfoUpdateSw.hostControllerSwAvailable = infoUpdateSw.hostControllerSwAvailable;
            dcInfoUpdateSw.hostControllerSwDate = infoUpdateSw.hostControllerSwDate;
            dcInfoUpdateSw.hostControllerSwVersion = infoUpdateSw.hostControllerSwVersion;
            //Web Manager
            dcInfoUpdateSw.webManagerSwAvailable = infoUpdateSw.webManagerSwAvailable;
            dcInfoUpdateSw.webManagerSwDate = infoUpdateSw.webManagerSwDate;
            dcInfoUpdateSw.webManagerSwVersion = infoUpdateSw.webManagerSwVersion;

            return dcInfoUpdateSw;
        }

        /// <summary>
        /// Update the system with the update available on the remote server
        /// </summary>
        public void UpdateSystem()
        {
            m_updatesManager.UpdateSystem();
        }

#endregion


#region Schedule update

#region Specific schedule

        /// <summary>
        /// Returns information about the specific scheduled update
        /// </summary>
        /// <returns>Specific scheduled update configuration</returns>
        public dc_InfoUpdateSpecificTime GetUpdateSpecificTime()
        {
            return m_updatesManager.GetUpdateSpecificTime();
        }

        /// <summary>
        /// Schedule a specific scheduled update
        /// </summary>
        /// <param name="infoUpdateSpecificTime">Specific scheduled update configuration</param>
        /// <returns>True if the update schedule was successful</returns>
        public bool SetUpdateSpecificTime(dc_InfoUpdateSpecificTime infoUpdateSpecificTime)
        {
            return m_updatesManager.SetUpdateSpecificTime(infoUpdateSpecificTime);
        }

#endregion

#region Periodic schedule

        /// <summary>
        /// Returns information about the periodic schedule update
        /// </summary>
        /// <returns>Periodic schedule update configuration</returns>
        public dc_InfoUpdatePeriodicTime GetUpdatePeriodicTime()
        {
            return m_updatesManager.GetUpdatePeriodicTime();
        }

        /// <summary>
        /// Schedule a periodic schedule update
        /// </summary>
        /// <param name="infoUpdatePeriodicTime">Periodic scheduled update configuration</param>
        /// <returns>True if the update schedule was succesful</returns>
        public bool SetUpdatePeriodicTime(dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime)
        {
            return m_updatesManager.SetUpdatePeriodicTime(infoUpdatePeriodicTime);
        }

#endregion

#region Check periodic

        /// <summary>
        /// Returns the data of the periodic update verification
        /// </summary>
        /// <returns>Periodic update verification configuration</returns>
        public dc_InfoCheckPeriodicTime GetCheckPeriodicTime()
        {
            return m_updatesManager.GetCheckPeriodicTime();
        }

        /// <summary>
        /// Schedule a periodic update verification
        /// </summary>
        /// <param name="infoCheckPeriodicTime">Periodic update verification configuration</param>
        /// <returns>True if the update verification schedule was succesful</returns>
        public bool SetCheckPeriodicTime(dc_InfoCheckPeriodicTime infoCheckPeriodicTime)
        {
            return m_updatesManager.SetCheckPeriodicTime(infoCheckPeriodicTime);
        }

#endregion

#endregion


#region Update notification

        /// <summary>
        /// Returns ths data of the update notifications
        /// </summary>
        /// <returns>Configuration information update notifications</returns>
        public dc_UpdateNotifications GetUpdateNotifications()
        {
            return m_updatesManager.GetUpdateNotifications();
        }

        /// <summary>
        /// Set the information update notifications
        /// </summary>
        /// <param name="updateNotifications">Information update notifications</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetUpdateNotifications(dc_UpdateNotifications updateNotifications)
        {
            return m_updatesManager.SetUpdateNotifications(updateNotifications);
        }

#endregion


#region Services Open Manager

#region Station Controller

        /// <summary>
        /// Check if a Station Controller needs updating. And update it if necessary
        /// </summary>
        /// <param name="swVersion">Station Controller software version</param>
        public void CheckUpdateConnectedStationController(string swVersion)
        {
            swVersion = swVersion.Trim();

            //Obtiene los datos del cliente que envia la consulta
            var prop = OperationContext.Current.IncomingMessageProperties;
            var endpoint = (RemoteEndpointMessageProperty)(prop[RemoteEndpointMessageProperty.Name]);
            string ip = endpoint.Address;

            m_updatesManager.CheckUpdateStationController(swVersion, ip);
        }

#endregion

#region Remote Manager

        /// <summary>
        /// Check if a Remote Manager needs updating
        /// </summary>
        /// <param name="swVersion">Remote Manager software version</param>
        /// <returns>True if needs updating</returns>
        public bool CheckUpdateConnectedRemoteManager(string swVersion)
        {
            return swVersion != m_localData.GetRemoteManagerSwVersion();
        }

        /// <summary>
        /// Return an update portion to Remote Manager
        /// </summary>
        /// <param name="nSequence">Sequence number</param>
        /// <returns>Update portion data</returns>
        public dc_UpdateRemoteManager GetFileUpdateRemoteManager(int nSequence)
        {
            return m_updatesManager.GetUpdateRemoteManager(nSequence);
        }

#endregion

#region Web Manager

        /// <summary>
        /// Check if a Web Manager needs updating
        /// </summary>
        /// <param name="swVersion">Web Manager software version</param>
        /// <returns>True if needs updating</returns>
        public bool CheckUpdateConnectedWebManager(string swVersion)
        {
            bool needsUpdate = swVersion != m_localData.GetWebManagerSwVersion();

            //Si no tiene una versión correcta envía una petición al Web Manager de que se actualice
            if (needsUpdate)
            {
                m_updatesManager.SendStartUpdateWebManager();
            }

            return needsUpdate;
        }

        /// <summary>
        /// Return an update portion to Web Manager
        /// </summary>
        /// <param name="nSequence">Sequence number</param>
        /// <returns>Update portion data</returns>
        public dc_UpdateWebManager GetFileUpdateWebManager(int nSequence)
        {

            //Obtiene los datos del cliente que envia la consulta
            var prop = OperationContext.Current.IncomingMessageProperties;
            var endpoint = (RemoteEndpointMessageProperty)(prop[RemoteEndpointMessageProperty.Name]);
            string ip = endpoint.Address;

            return m_updatesManager.GetUpdateWebManager(nSequence, ip);
        }

#endregion

#endregion

#endregion


#region Update firmware

        /// <summary>
        /// Returns information of all available software version for a station model
        /// </summary>
        /// <param name="infoUpdateFirmware"></param>
        /// <returns>Software versions for a station model</returns>
        public List<dc_FirmwareStation> GetInfoUpdateFirmware(dc_FirmwareStation infoUpdateFirmware)
        {
            return m_updaterFirmwareManager.GetInfoUpdateFirmware(infoUpdateFirmware);
        }

        /// <summary>
        /// Return an update portion of station software
        /// </summary>
        /// <param name="nSequence">Sequence number</param>
        /// <param name="urlFirmwareSw">Firmware file</param>
        /// <returns>Update portion data</returns>
        public dc_UpdateFirmware GetFileUpdateFirmware(int nSequence, string urlFirmwareSw)
        {
            return m_updaterFirmwareManager.GetFileUpdateFirmware(nSequence, urlFirmwareSw);
        }

#endregion


#region Services Open Manager

#region Station Controller

        /// <summary>
        /// Return the local Station Controller version
        /// </summary>
        /// <returns>Station Controller version</returns>
        public string GetStationControllerSwVersion()
        {
            return m_localData.GetStationControllerSwVersion();
        }

#endregion

#region Host Controller

        /// <summary>
        /// Return the local Host Controller version
        /// </summary>
        /// <returns>Host Controller version</returns>
        public string GetHostControllerSwVersion()
        {
            return m_localData.GetHostControllerSwVersion();
        }

#endregion

#region Web Manager

        /// <summary>
        /// Return the local Web Manager version
        /// </summary>
        /// <returns>Web Manager version</returns>
        public string GetWebManagerSwVersion()
        {
            return m_localData.GetWebManagerSwVersion();
        }

        /// <summary>
        /// Set Web Manager uri
        /// </summary>
        /// <param name="webManagerUri">Web Manager uri</param>
        public void SetWebManagerUri(Uri webManagerUri)
        {
            m_localData.SetWebManagerUri(webManagerUri);
        }

#endregion

#endregion


#region Log Events

        public void RegisterEventLog(List<dc_EventLog> eventLog)
        {
            m_eventLogRecorder.RegisterEventLog(eventLog);
        }

#endregion

    }
}
