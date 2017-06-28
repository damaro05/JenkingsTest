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

using System.IO;

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Manage the update operations
    /// </summary>
    public class CUpdatesManager
    {

        private CLocalData m_localData;

        private CVersionFileParser m_versionFileParser;
        private CScheduleUpdates m_scheduleUpdates;

        //Files to update services and stations
        private CSystemFilesManager m_systemFilesManager;

        //Communications
        private CComUpdaterService m_comUpdaterService;
        private CComRemoteManager m_comRemoteManager;
        private CComWebManager m_comWebManager;

        private bool m_isUpdating = false;


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="localData">Local data reference</param>
        public CUpdatesManager(CLocalData localData)
        {
            m_localData = localData;
            m_versionFileParser = new CVersionFileParser(m_localData);
            m_scheduleUpdates = new CScheduleUpdates(m_localData);
            m_scheduleUpdates.Event_UpdateSystem += UpdateSystem;
            m_systemFilesManager = new CSystemFilesManager(m_localData);
            m_comUpdaterService = new CComUpdaterService();
            m_comRemoteManager = new CComRemoteManager();
            m_comWebManager = new CComWebManager();

            //Descargar los archivos de actualización si no están descargados
            DownloadSwUpdate();

            //Enviar comprobación de actualización al Web Manager
            string ipWebManager = m_localData.GetWebManagerUri();
            if (!string.IsNullOrEmpty(ipWebManager))
            {
                m_comWebManager.StartUpdate(ipWebManager);
            }
        }

        /// <summary>
        /// Release resources
        /// </summary>
        public void Dispose()
        {
            m_localData = null;
            m_versionFileParser.Dispose();
            m_scheduleUpdates.Dispose();
            m_systemFilesManager.Dispose();
            m_comUpdaterService.Dispose();
            m_comRemoteManager.Dispose();
            m_comWebManager.Dispose();
        }

        /// <summary>
        /// Checks if exists an update form JBC's server
        /// </summary>
        /// <returns>Update information</returns>
        public stSwVersion GetInfoNewUpdate()
        {
            stSwVersion infoUpdateSw = new stSwVersion();

            //descargar el archivo de versiones del remote server
            m_systemFilesManager.DownloadFile(System.Convert.ToString(My.Settings.Default.VersionFileName));

            //obtener la información del archivo de versiones
            stSwVersion remoteSwVersion = m_versionFileParser.GetInfoLastSwVersion();

            //obtener la información de las versiones instaladas en la red local
            stSwVersion currentSwVersion = m_localData.GetSwInfo();

            //obtiene la fecha de la última actualización
            infoUpdateSw.lastUpdateDate = m_localData.GetLastUpdateDate();

            //Comprobar si hay actualizacion software del StationController
            if (remoteSwVersion.stationControllerSwDate > currentSwVersion.stationControllerSwDate)
            {
                infoUpdateSw.stationControllerSwAvailable = true;
                infoUpdateSw.stationControllerSwDate = remoteSwVersion.stationControllerSwDate;
                infoUpdateSw.stationControllerSwVersion = remoteSwVersion.stationControllerSwVersion;
                infoUpdateSw.stationControllerSwUrl = remoteSwVersion.stationControllerSwUrl;
            }
            else
            {
                infoUpdateSw.stationControllerSwAvailable = false;
            }

            //Comprobar si hay actualizacion software del RemoteManager
            if (remoteSwVersion.remoteManagerSwDate > currentSwVersion.remoteManagerSwDate)
            {
                infoUpdateSw.remoteManagerSwAvailable = true;
                infoUpdateSw.remoteManagerSwDate = remoteSwVersion.remoteManagerSwDate;
                infoUpdateSw.remoteManagerSwVersion = remoteSwVersion.remoteManagerSwVersion;
                infoUpdateSw.remoteManagerSwUrl = remoteSwVersion.remoteManagerSwUrl;
            }
            else
            {
                infoUpdateSw.remoteManagerSwAvailable = false;
            }

            //Comprobar si hay actualizacion software del HostController
            if (remoteSwVersion.hostControllerSwDate > currentSwVersion.hostControllerSwDate)
            {
                infoUpdateSw.hostControllerSwAvailable = true;
                infoUpdateSw.hostControllerSwDate = remoteSwVersion.hostControllerSwDate;
                infoUpdateSw.hostControllerSwVersion = remoteSwVersion.hostControllerSwVersion;
                infoUpdateSw.hostControllerSwUrl = remoteSwVersion.hostControllerSwUrl;
            }
            else
            {
                infoUpdateSw.hostControllerSwAvailable = false;
            }

            //Comprobar si hay actualizacion software del WebManager
            if (remoteSwVersion.webManagerSwDate > currentSwVersion.webManagerSwDate)
            {
                infoUpdateSw.webManagerSwAvailable = true;
                infoUpdateSw.webManagerSwDate = remoteSwVersion.webManagerSwDate;
                infoUpdateSw.webManagerSwVersion = remoteSwVersion.webManagerSwVersion;
                infoUpdateSw.webManagerSwUrl = remoteSwVersion.webManagerSwUrl;
            }
            else
            {
                infoUpdateSw.webManagerSwAvailable = false;
            }

            return infoUpdateSw;
        }

        /// <summary>
        /// Updates the system with the latest software version from the JBC's server
        /// </summary>
        public void UpdateSystem()
        {

            //Si está actualizando no recibir peticiones
            if (m_isUpdating)
            {
                return;
            }
            m_isUpdating = true;

            //Actualiza la fecha de la última actualización del software
            m_localData.SetLastUpdateDate();

            stSwVersion infoUpdateSw = GetInfoNewUpdate();

            //Comprobar si hay actualizacion software del StationController
            if (infoUpdateSw.stationControllerSwAvailable)
            {

                //Descargar la versión de software y actualizar DB
                m_systemFilesManager.DownloadFile(infoUpdateSw.stationControllerSwUrl);
                m_localData.SetStationControllerSwInfo(infoUpdateSw.stationControllerSwVersion, infoUpdateSw.stationControllerSwDate, infoUpdateSw.stationControllerSwUrl);
            }

            //Comprobar si hay actualizacion software del RemoteManager
            if (infoUpdateSw.remoteManagerSwAvailable)
            {

                //Descargar la versión de software y actualizar DB
                m_systemFilesManager.DownloadFile(infoUpdateSw.remoteManagerSwUrl);
                m_localData.SetRemoteManagerSwInfo(infoUpdateSw.remoteManagerSwVersion, infoUpdateSw.remoteManagerSwDate, infoUpdateSw.remoteManagerSwUrl);
            }

            //Comprobar si hay actualizacion software del WebManager
            if (infoUpdateSw.webManagerSwAvailable)
            {

                //Descargar la versión de software y actualizar DB
                m_systemFilesManager.DownloadFile(infoUpdateSw.webManagerSwUrl);
                m_localData.SetWebManagerSwInfo(infoUpdateSw.webManagerSwVersion, infoUpdateSw.webManagerSwDate, infoUpdateSw.webManagerSwUrl);
            }

            //Si hay actualización del WebManager pero no del HostController
            //Ya que si hay actualización del HostController, éste se reiniciará y comprobará si se tiene que actualizar
            if (infoUpdateSw.webManagerSwAvailable && !infoUpdateSw.hostControllerSwAvailable)
            {

                //Descargar la versión de software y actualizar DB
                m_systemFilesManager.DownloadFile(infoUpdateSw.webManagerSwUrl);
                m_localData.SetWebManagerSwInfo(infoUpdateSw.webManagerSwVersion, infoUpdateSw.webManagerSwDate, infoUpdateSw.webManagerSwUrl);

                //Enviar comprobación de actualización al Web Manager
                string ipWebManager = m_localData.GetWebManagerUri().ToString();
                if (!string.IsNullOrEmpty(ipWebManager))
                {
                    m_comWebManager.StartUpdate(ipWebManager);
                }
            }

            //Comprobar si hay actualizacion software del HostController
            if (infoUpdateSw.hostControllerSwAvailable)
            {

                //Descargar la versión de software, no actualiza ls DB para que al reiniciar el HostController ejecute los scripts sql de actualización
                string hostControllerSwUpdatePath = m_systemFilesManager.DownloadFile(infoUpdateSw.hostControllerSwUrl);

                //Actualizar el HostController
                m_comUpdaterService.SendUpdateSw(hostControllerSwUpdatePath, System.Net.Dns.GetHostName());
            }

            m_isUpdating = false;
        }

        /// <summary>
        /// Check if the latest update software version is downloaded and downloaded it otherwise
        /// </summary>
        public void DownloadSwUpdate()
        {

            stSwVersion currentSwVersion = m_localData.GetSwInfo(); //obtener la información de las versiones instaladas en la red local
            bool bDownloadStationController = !File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), currentSwVersion.stationControllerSwUrl));
            bool bDownloadRemoteManager = !File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), currentSwVersion.remoteManagerSwUrl));
            bool bDownloadHostController = !File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), currentSwVersion.hostControllerSwUrl));
            bool bDownloadWebManager = !File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), currentSwVersion.webManagerSwUrl));

            if (bDownloadStationController || bDownloadRemoteManager || bDownloadHostController || bDownloadWebManager)
            {
                m_systemFilesManager.DownloadFile(System.Convert.ToString(My.Settings.Default.VersionFileName));

                if (bDownloadStationController)
                {
                    string stationControllerSwUrl = m_versionFileParser.GetStationControllerSwUrl(currentSwVersion.stationControllerSwVersion);
                    string sFilePath = m_systemFilesManager.DownloadFile(stationControllerSwUrl);
                    LoggerModule.logger.Info(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". StationController downloaded software url:" + sFilePath);
                    m_localData.SetStationControllerSwUrl(stationControllerSwUrl);
                }

                if (bDownloadRemoteManager)
                {
                    string remoteManagerSwUrl = m_versionFileParser.GetRemoteManagerSwUrl(currentSwVersion.remoteManagerSwVersion);
                    string sFilePath = m_systemFilesManager.DownloadFile(remoteManagerSwUrl);
                    LoggerModule.logger.Info(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". RemoteManager downloaded software url:" + sFilePath);
                    m_localData.SetRemoteManagerSwUrl(remoteManagerSwUrl);
                }

                if (bDownloadHostController)
                {
                    string hostControllerSwUrl = m_versionFileParser.GetHostControllerSwUrl(currentSwVersion.hostControllerSwVersion);
                    string sFilePath = m_systemFilesManager.DownloadFile(hostControllerSwUrl);
                    LoggerModule.logger.Info(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". HostController downloaded software url:" + sFilePath);
                    m_localData.SetHostControllerSwUrl(hostControllerSwUrl);
                }

                if (bDownloadWebManager)
                {
                    string webManagerSwUrl = m_versionFileParser.GetWebManagerSwUrl(currentSwVersion.webManagerSwVersion);
                    string sFilePath = m_systemFilesManager.DownloadFile(webManagerSwUrl);
                    LoggerModule.logger.Info(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". WebManager downloaded software url:" + sFilePath);
                    m_localData.SetWebManagerSwUrl(webManagerSwUrl);
                }
            }
        }


        #region Station Controller

        /// <summary>
        /// Check the Station Controller software version and update it if necessary
        /// </summary>
        /// <param name="swVersion">Station Controller software version</param>
        /// <param name="ipStationController">Station Controller IP</param>
        public void CheckUpdateStationController(string swVersion, string ipStationController)
        {
            if (m_localData.GetStationControllerSwVersion() != swVersion)
            {
                string urlStationControllerSw = Path.Combine(m_localData.GetSystemFilesFolderLocation(), m_localData.GetStationControllerSwUrl());
                m_comUpdaterService.SendUpdateSw(urlStationControllerSw, ipStationController);
            }
        }

        #endregion


        #region Remote Manager

        /// <summary>
        /// Gets a part of the update's package for a Remote Manager
        /// </summary>
        /// <param name="nSequence">Part number of the update package</param>
        /// <returns>Update package to send</returns>
        public dc_UpdateRemoteManager GetUpdateRemoteManager(int nSequence)
        {
            return m_comRemoteManager.GetFileUpdateRemoteManager(nSequence, Path.Combine(m_localData.GetSystemFilesFolderLocation(), m_localData.GetRemoteManagerSwUrl()));
        }

        #endregion


        #region Web Manager

        /// <summary>
        /// Gets a portion of the update to be sent to the WebManager
        /// </summary>
        /// <param name="nSequence">Sequence number</param>
        /// <param name="ip">Address web manager</param>
        /// <returns>Update data</returns>
        public dc_UpdateWebManager GetUpdateWebManager(int nSequence, string ip)
        {
            return m_comWebManager.GetFileUpdateWebManager(nSequence, Path.Combine(m_localData.GetSystemFilesFolderLocation(), m_localData.GetWebManagerSwUrl()), ip);
        }

        /// <summary>
        /// Start the Web Manager update process
        /// </summary>
        public void SendStartUpdateWebManager()
        {
            string ipWebManager = m_localData.GetWebManagerUri().ToString();
            if (!string.IsNullOrEmpty(ipWebManager))
            {
                m_comWebManager.StartUpdate(ipWebManager);
            }
        }

        #endregion


        #region Schedule update

        #region Schedule specific time

        /// <summary>
        /// Get the scheduled time to start an update in a concrete date
        /// </summary>
        /// <returns>Scheduled time</returns>
        public dc_InfoUpdateSpecificTime GetUpdateSpecificTime()
        {
            return m_scheduleUpdates.GetUpdateSpecificTime();
        }

        /// <summary>
        /// Set the scheduled time to start a update in a concrete date
        /// </summary>
        /// <param name="infoUpdateSpecificTime">Scheduled time</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetUpdateSpecificTime(dc_InfoUpdateSpecificTime infoUpdateSpecificTime)
        {
            return m_scheduleUpdates.SetUpdateSpecificTime(infoUpdateSpecificTime);
        }

        #endregion


        #region Schedule periodic time

        /// <summary>
        /// Get the scheduled time of periodic updates
        /// </summary>
        /// <returns>Scheduled periodic updates</returns>
        public dc_InfoUpdatePeriodicTime GetUpdatePeriodicTime()
        {
            return m_scheduleUpdates.GetUpdatePeriodicTime();
        }

        /// <summary>
        /// Set the scheduled time of periodic updates
        /// </summary>
        /// <param name="infoUpdatePeriodicTime">Scheduled time</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetUpdatePeriodicTime(dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime)
        {
            return m_scheduleUpdates.SetScheduleUpdatePeriodic(infoUpdatePeriodicTime);
        }

        /// <summary>
        /// Get the information of periodic updates check
        /// </summary>
        /// <returns>Information of periodic updates check</returns>
        public dc_InfoCheckPeriodicTime GetCheckPeriodicTime()
        {
            return m_scheduleUpdates.GetCheckPeriodicTime();
        }

        /// <summary>
        /// Set periodic updates check
        /// </summary>
        /// <param name="infoCheckPeriodicTime">Periodic updates check configuration</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetCheckPeriodicTime(dc_InfoCheckPeriodicTime infoCheckPeriodicTime)
        {
            return m_scheduleUpdates.SetCheckPeriodicTime(infoCheckPeriodicTime);
        }

        #endregion

        #endregion


        #region Update notification

        /// <summary>
        /// Get the configuration information update notifications
        /// </summary>
        /// <returns>Configuration information update notifications</returns>
        public dc_UpdateNotifications GetUpdateNotifications()
        {
            return m_localData.GetUpdateNotifications();
        }

        /// <summary>
        /// Set the configuration information update notifications
        /// </summary>
        /// <param name="updateNotifications">Configuration information update notifications</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetUpdateNotifications(dc_UpdateNotifications updateNotifications)
        {
            return m_localData.SetUpdateNotifications(updateNotifications);
        }

        #endregion

    }
}
