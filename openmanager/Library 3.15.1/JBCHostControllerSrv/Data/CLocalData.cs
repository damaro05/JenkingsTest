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
using System.Threading;

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Provide access to the local data
    /// </summary>
    public class CLocalData
    {

        private const string CONST_TRUE = "1";
        private const string CONST_FALSE = "0";

        private RoutinesLibrary.Data.DataBase.SQLCompact.SQLCompactConnection m_DBConnection;
        private static Semaphore m_semaphoreData = new Semaphore(1, 1);


        /// <summary>
        /// Class constructor. Created the DB if not exists or update it
        /// </summary>
        public CLocalData()
        {

            //Create connection with the Data Base and execute the necesary sql sentences
            string sDBPathFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                "JBC\\JBC Host Controller Service", System.Convert.ToString(
                My.Settings.Default.SQLCeDB));
            m_DBConnection = new RoutinesLibrary.Data.DataBase.SQLCompact.SQLCompactConnection(sDBPathFilename);

            CDBScripts DBScripts = new CDBScripts(m_DBConnection);

            if (!System.IO.File.Exists(sDBPathFilename))
            {
                DBScripts.CreateDataBase();
            }
            else
            {
                DBScripts.UpdateDataBase(GetHostControllerSwVersion());
            }
        }

        /// <summary>
        /// Releases resources
        /// </summary>
        public void Dispose()
        {
            m_DBConnection.Dispose();
        }

        #region System Files

        /// <summary>
        /// Get the availability of the connection to the Remote Server
        /// </summary>
        /// <returns>True if the Host Controller is configure to connect to the Remote Server</returns>
        public bool IsAvailableRemoteServerDownload()
        {
            bool availableRemoteServerDownload = false;

            string table = "systemInfo";
            string[] keyList = new string[1];
            keyList[0] = "remoteServerDownloadAvailable";
            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    availableRemoteServerDownload = bool.Parse(valueList[0]);
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return availableRemoteServerDownload;
        }

        /// <summary>
        /// Configure the availability of the connection to the Remote Server
        /// </summary>
        /// <param name="active">Indicates if the connection to the Remote Server will be active</param>
        /// <returns>True if the configuration was succesul</returns>
        public bool SetAvailableRemoteServerDownload(bool active)
        {
            bool bRet = false;

            string table = "systemInfo";
            string[] keyList = new string[1];
            keyList[0] = "remoteServerDownloadAvailable";

            string[] valueList = new string[1];
            if (active)
            {
                valueList[0] = CONST_TRUE;
            }
            else
            {
                valueList[0] = CONST_FALSE;
            }

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Gets the path for manual updates instalations
        /// </summary>
        /// <returns>Path for manual updates instalations</returns>
        public string GetUserFilesLocalFolderLocation()
        {
            string folderLocation = "";

            string table = "systemInfo";
            string[] keyList = new string[1];
            keyList[0] = "remoteServerDownloadLocalFolder";
            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    folderLocation = valueList[0];
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return folderLocation;
        }

        /// <summary>
        /// Sets the path for manual updates instalations
        /// </summary>
        /// <returns>True if the operation was succeful</returns>
        public bool SetUserFilesLocalFolderLocation(string path)
        {
            bool bRet = false;

            string table = "systemInfo";
            string[] keyList = new string[1];
            keyList[0] = "remoteServerDownloadLocalFolder";

            string[] valueList = new string[1];
            valueList[0] = path;

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Gets the path of the downloaded updates packages
        /// </summary>
        /// <returns>Path for manual updates instalations</returns>
        public string GetSystemFilesFolderLocation()
        {
            string systemFilesFolderLocation = "";

            //Si está habilitado la descarga automática desde el servidor ftp de JBC, devolver la ruta de descarga
            if (IsAvailableRemoteServerDownload())
            {
                systemFilesFolderLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\JBC Host Controller Service");

                //Sino, la carpeta elegida por el usuario
            }
            else
            {
                systemFilesFolderLocation = GetUserFilesLocalFolderLocation();
            }

            return systemFilesFolderLocation;
        }

        #endregion


        #region Updates

        #region Schedule update

        /// <summary>
        /// Get the periodic updates information
        /// </summary>
        /// <returns>Configuration of the periodic updates</returns>
        public dc_InfoUpdatePeriodicTime GetPeriodicTimeInfo()
        {

            dc_InfoUpdatePeriodicTime periodicTimeInfo = new dc_InfoUpdatePeriodicTime();
            periodicTimeInfo.available = false;
            periodicTimeInfo.modeDaily = false;
            periodicTimeInfo.weekday = (byte)1;
            periodicTimeInfo.hour = (byte)0;
            periodicTimeInfo.minute = (byte)0;

            string table = "updateSchedule";
            string[] keyList = new string[5];
            keyList[0] = "updatePeriodicAvailable";
            keyList[1] = "updatePeriodicModeDaily";
            keyList[2] = "updatePeriodicWeekday";
            keyList[3] = "updatePeriodicHour";
            keyList[4] = "updatePeriodicMinute";
            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    periodicTimeInfo.available = bool.Parse(valueList[0]);
                    periodicTimeInfo.modeDaily = bool.Parse(valueList[1]);
                    periodicTimeInfo.weekday = byte.Parse(valueList[2]);
                    periodicTimeInfo.hour = byte.Parse(valueList[3]);
                    periodicTimeInfo.minute = byte.Parse(valueList[4]);
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return periodicTimeInfo;
        }

        /// <summary>
        /// Set the periodic updates information
        /// </summary>
        /// <param name="infoUpdatePeriodicTime">Periodic updates information</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetPeriodicTimeInfo(dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime)
        {
            bool bRet = false;

            string table = "updateSchedule";
            string[] keyList = new string[5];
            keyList[0] = "updatePeriodicAvailable";
            keyList[1] = "updatePeriodicModeDaily";
            keyList[2] = "updatePeriodicWeekday";
            keyList[3] = "updatePeriodicHour";
            keyList[4] = "updatePeriodicMinute";

            string[] valueList = new string[5];
            if (infoUpdatePeriodicTime.available)
            {
                valueList[0] = CONST_TRUE;
            }
            else
            {
                valueList[0] = CONST_FALSE;
            }
            if (infoUpdatePeriodicTime.modeDaily)
            {
                valueList[1] = CONST_TRUE;
            }
            else
            {
                valueList[1] = CONST_FALSE;
            }
            valueList[2] = (infoUpdatePeriodicTime.weekday).ToString();
            valueList[3] = (infoUpdatePeriodicTime.hour).ToString();
            valueList[4] = (infoUpdatePeriodicTime.minute).ToString();

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Get the periodic updates check information
        /// </summary>
        /// <returns>Configuration of the periodic updates check</returns>
        public dc_InfoCheckPeriodicTime GetCheckPeriodicTimeInfo()
        {

            dc_InfoCheckPeriodicTime periodicTimeInfo = new dc_InfoCheckPeriodicTime();
            periodicTimeInfo.available = false;

            string table = "updateSchedule";
            string[] keyList = new string[1];
            keyList[0] = "checkPeriodicAvailable";
            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    periodicTimeInfo.available = bool.Parse(valueList[0]);
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return periodicTimeInfo;
        }

        /// <summary>
        /// Set the periodic updates check information
        /// </summary>
        /// <param name="infoCheckPeriodicTime">Periodic updates check information</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetCheckPeriodicTimeInfo(dc_InfoCheckPeriodicTime infoCheckPeriodicTime)
        {
            bool bRet = false;

            string table = "updateSchedule";
            string[] keyList = new string[1];
            keyList[0] = "checkPeriodicAvailable";

            string[] valueList = new string[1];
            if (infoCheckPeriodicTime.available)
            {
                valueList[0] = CONST_TRUE;
            }
            else
            {
                valueList[0] = CONST_FALSE;
            }

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Get the specific updates information
        /// </summary>
        /// <returns>Configuration of the specific update</returns>
        public dc_InfoUpdateSpecificTime GetSpecificTimeInfo()
        {

            dc_InfoUpdateSpecificTime specificTimeInfo = new dc_InfoUpdateSpecificTime();
            specificTimeInfo.available = false;
            specificTimeInfo.time = DateTime.Now;

            string table = "updateSchedule";
            string[] keyList = new string[2];
            keyList[0] = "updateSpecificAvailable";
            keyList[1] = "updateSpecificTime";
            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    specificTimeInfo.available = bool.Parse(valueList[0]);
                    specificTimeInfo.time = DateTime.Parse(valueList[1]);
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return specificTimeInfo;
        }

        /// <summary>
        /// Set the specific updates information
        /// </summary>
        /// <param name="infoUpdateSpecificTime">Specific updates information</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetSpecificTimeInfo(dc_InfoUpdateSpecificTime infoUpdateSpecificTime)
        {
            bool bRet = false;

            string table = "updateSchedule";
            string[] keyList = new string[2];
            keyList[0] = "updateSpecificAvailable";
            keyList[1] = "updateSpecificTime";

            string[] valueList = new string[2];
            if (infoUpdateSpecificTime.available)
            {
                valueList[0] = CONST_TRUE;
            }
            else
            {
                valueList[0] = CONST_FALSE;
            }
            valueList[1] = infoUpdateSpecificTime.time.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Set on/off the specific updates
        /// </summary>
        /// <param name="available">True if the specific updates in turn on</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetUpdateSpecificAvailable(bool available)
        {
            bool bRet = false;

            string table = "updateSchedule";
            string key = "updateSpecificAvailable";
            string value = "";
            if (available)
            {
                value = CONST_TRUE;
            }
            else
            {
                value = CONST_FALSE;
            }

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.UpdateQuery(key, table, value))
                {
                    bRet = true;
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        #endregion


        #region Update notification

        /// <summary>
        /// Get the configuration information update notifications
        /// </summary>
        /// <returns>Configuration information update notifications</returns>
        public dc_UpdateNotifications GetUpdateNotifications()
        {

            dc_UpdateNotifications updateNotifications = new dc_UpdateNotifications();
            updateNotifications.emailAvailable = false;
            updateNotifications.emailAddress = "";

            string table = "systemInfo";
            string[] keyList = new string[2];
            keyList[0] = "updateNotificationAvailable";
            keyList[1] = "updateNotificationEmail";
            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    updateNotifications.emailAvailable = bool.Parse(valueList[0]);
                    updateNotifications.emailAddress = valueList[1];
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return updateNotifications;
        }

        /// <summary>
        /// Set the configuration information update notifications
        /// </summary>
        /// <param name="updateNotifications">Configuration information update notifications</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetUpdateNotifications(dc_UpdateNotifications updateNotifications)
        {
            bool bRet = false;

            string table = "systemInfo";
            string[] keyList = new string[2];
            keyList[0] = "updateNotificationAvailable";
            keyList[1] = "updateNotificationEmail";

            string[] valueList = new string[2];
            if (updateNotifications.emailAvailable)
            {
                valueList[0] = CONST_TRUE;
            }
            else
            {
                valueList[0] = CONST_FALSE;
            }
            valueList[1] = updateNotifications.emailAddress;

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        #endregion


        #region Software version

        /// <summary>
        /// Get software information installed in the local network
        /// </summary>
        /// <returns>Software information installed in the local network</returns>
        public stSwVersion GetSwInfo()
        {

            stSwVersion swVersion = new stSwVersion();
            swVersion.stationControllerSwVersion = "";
            swVersion.stationControllerSwDate = DateTime.Parse("1/1/1900");
            swVersion.stationControllerSwUrl = "";

            swVersion.remoteManagerSwVersion = "";
            swVersion.remoteManagerSwDate = DateTime.Parse("1/1/1900");
            swVersion.remoteManagerSwUrl = "";

            swVersion.hostControllerSwVersion = "";
            swVersion.hostControllerSwDate = DateTime.Parse("1/1/1900");
            swVersion.hostControllerSwUrl = "";

            swVersion.webManagerSwVersion = "";
            swVersion.webManagerSwDate = DateTime.Parse("1/1/1900");
            swVersion.webManagerSwUrl = "";

            string table = "versionInfo";
            string[] keyList = new string[8];
            keyList[0] = "stationControllerSwVersion";
            keyList[1] = "remoteManagerSwVersion";
            keyList[2] = "hostControllerSwVersion";
            keyList[3] = "webManagerSwVersion";
            keyList[4] = "stationControllerSwDate";
            keyList[5] = "remoteManagerSwDate";
            keyList[6] = "hostControllerSwDate";
            keyList[7] = "webManagerSwDate";
            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    swVersion.stationControllerSwVersion = valueList[0];
                    swVersion.remoteManagerSwVersion = valueList[1];
                    swVersion.hostControllerSwVersion = valueList[2];
                    swVersion.webManagerSwVersion = valueList[3];
                    swVersion.stationControllerSwDate = DateTime.Parse(valueList[4]);
                    swVersion.remoteManagerSwDate = DateTime.Parse(valueList[5]);
                    swVersion.hostControllerSwDate = DateTime.Parse(valueList[6]);
                    swVersion.webManagerSwDate = DateTime.Parse(valueList[7]);
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return swVersion;
        }

        /// <summary>
        /// Get date of last update
        /// </summary>
        /// <returns>Date of the last update</returns>
        public DateTime GetLastUpdateDate()
        {
            DateTime retDate = new DateTime();

            string table = "versionInfo";
            string[] keyList = new string[1];
            keyList[0] = "lastUpdateDate";

            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    retDate = DateTime.Parse(valueList[0]);
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return retDate;
        }

        /// <summary>
        /// Set date of last update
        /// </summary>
        /// <returns>True if the operation was successful</returns>
        public bool SetLastUpdateDate()
        {
            bool bRet = false;

            string table = "versionInfo";
            string[] keyList = new string[1];
            keyList[0] = "lastUpdateDate";
            string[] valueList = new string[1];
            valueList[0] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }


        #region Station Controller

        /// <summary>
        /// Set the Station Controller software information
        /// </summary>
        /// <param name="version">Software version</param>
        /// <param name="dtDate">Software date release</param>
        /// <param name="url">Software path in the local machine</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetStationControllerSwInfo(string version, DateTime dtDate, string url)
        {
            bool bRet = false;

            string table = "versionInfo";
            string[] keyList = new string[3];
            keyList[0] = "stationControllerSwVersion";
            keyList[1] = "stationControllerSwDate";
            keyList[2] = "stationControllerSwUrl";
            string[] valueList = new string[3];
            valueList[0] = version;
            valueList[1] = dtDate.ToString("yyyy-MM-dd HH:mm:ss");
            valueList[2] = url;

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Get the Station Controller software version
        /// </summary>
        /// <returns>Software version</returns>
        public string GetStationControllerSwVersion()
        {

            string key = "stationControllerSwVersion";
            string value = "";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.SelectQuery(key, table, ref value))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return value;
        }

        /// <summary>
        /// Get the Station Controller software path in the local machine
        /// </summary>
        /// <returns>Software path in the local machine</returns>
        public string GetStationControllerSwUrl()
        {

            string key = "stationControllerSwUrl";
            string value = "";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.SelectQuery(key, table, ref value))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return value;
        }

        /// <summary>
        /// Set the Station Controller software path in the local machine
        /// </summary>
        /// <param name="url">Software path in the local machine</param>
        public void SetStationControllerSwUrl(string url)
        {

            string key = "stationControllerSwUrl";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.UpdateQuery(key, table, url))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }
        }

        #endregion


        #region Host Controller

        /// <summary>
        /// Get the Host Controller software version
        /// </summary>
        /// <returns>Software version</returns>
        public string GetHostControllerSwVersion()
        {

            string key = "hostControllerSwVersion";
            string value = "";
            string table = "versionInfo";
            string oldTable = "info"; //retrocompatibilidad, el nombre de la tabla se cambió

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.TableExists(table))
                {
                    if (!m_DBConnection.SelectQuery(key, table, ref value))
                    {
                        LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                    }
                }
                else
                {
                    if (!m_DBConnection.SelectQuery(key, oldTable, ref value))
                    {
                        LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                    }
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return value;
        }

        /// <summary>
        /// Set the Host Controller software path in the local machine
        /// </summary>
        /// <param name="url">Software path in the local machine</param>
        public void SetHostControllerSwUrl(string url)
        {

            string key = "hostControllerSwUrl";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.UpdateQuery(key, table, url))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }
        }

        #endregion


        #region Remote Manager

        /// <summary>
        /// Set the Remote Manager software information
        /// </summary>
        /// <param name="version">Software version</param>
        /// <param name="dtDate">Software date release</param>
        /// <param name="url">Software path in the local machine</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetRemoteManagerSwInfo(string version, DateTime dtDate, string url)
        {
            bool bRet = false;

            string table = "versionInfo";
            string[] keyList = new string[3];
            keyList[0] = "remoteManagerSwVersion";
            keyList[1] = "remoteManagerSwDate";
            keyList[2] = "remoteManagerSwUrl";
            string[] valueList = new string[3];
            valueList[0] = version;
            valueList[1] = dtDate.ToString("yyyy-MM-dd HH:mm:ss");
            valueList[2] = url;

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Get the Remote Manager software version
        /// </summary>
        /// <returns>Software version</returns>
        public string GetRemoteManagerSwVersion()
        {

            string key = "remoteManagerSwVersion";
            string value = "";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.SelectQuery(key, table, ref value))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return value;
        }

        /// <summary>
        /// Get the Remote Manager software path in the local machine
        /// </summary>
        /// <returns>Software path in the local machine</returns>
        public string GetRemoteManagerSwUrl()
        {

            string key = "remoteManagerSwUrl";
            string value = "";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.SelectQuery(key, table, ref value))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return value;
        }

        /// <summary>
        /// Set the Remote Manager software path in the local machine
        /// </summary>
        /// <param name="url">Software path in the local machine</param>
        public void SetRemoteManagerSwUrl(string url)
        {

            string key = "remoteManagerSwUrl";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.UpdateQuery(key, table, url))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }
        }

        #endregion


        #region Web Manager

        /// <summary>
        /// Set Web Manager software information
        /// </summary>
        /// <param name="version">Software version</param>
        /// <param name="dtDate">Software date release</param>
        /// <param name="url">Software path in the local machine</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetWebManagerSwInfo(string version, DateTime dtDate, string url)
        {
            bool bRet = false;

            string table = "versionInfo";
            string[] keyList = new string[3];
            keyList[0] = "webManagerSwVersion";
            keyList[1] = "webManagerSwDate";
            keyList[2] = "webManagerSwUrl";
            string[] valueList = new string[3];
            valueList[0] = version;
            valueList[1] = dtDate.ToString("yyyy-MM-dd HH:mm:ss");
            valueList[2] = url;

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        /// <summary>
        /// Get Web Manager software version
        /// </summary>
        /// <returns>Software version</returns>
        public string GetWebManagerSwVersion()
        {

            string key = "webManagerSwVersion";
            string value = "";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.SelectQuery(key, table, ref value))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return value;
        }

        /// <summary>
        /// Get Web Manager software path in the local machine
        /// </summary>
        /// <returns>Software path in the local machine</returns>
        public string GetWebManagerSwUrl()
        {

            string key = "webManagerSwUrl";
            string value = "";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.SelectQuery(key, table, ref value))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return value;
        }

        /// <summary>
        /// Set Web Manager software path in the local machine
        /// </summary>
        /// <param name="url">Software path in the local machine</param>
        public void SetWebManagerSwUrl(string url)
        {

            string key = "webManagerSwUrl";
            string table = "versionInfo";

            try
            {
                m_semaphoreData.WaitOne();
                if (!m_DBConnection.UpdateQuery(key, table, url))
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }
        }

        #endregion

        #endregion

        #endregion


        #region Services Open Manager

        /// <summary>
        /// Get the Web Manager URI installed
        /// </summary>
        /// <returns>Web Manager URI installed</returns>
        public string GetWebManagerUri()
        {
            string webManagerUri = "";

            string table = "servicesOpenManager";
            string[] keyList = new string[1];
            keyList[0] = "webManagerUrl";

            string[] valueList = new string[1];

            try
            {
                m_semaphoreData.WaitOne();
                if (m_DBConnection.SelectQuery(keyList, table, ref valueList))
                {
                    webManagerUri = valueList[0];
                }
                else
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Select Query");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return webManagerUri;
        }

        /// <summary>
        /// Set the Web Manager URI installed
        /// </summary>
        /// <param name="webManagerUri">Web Manager URI installed</param>
        /// <returns>True if the operation was successful</returns>
        public bool SetWebManagerUri(Uri webManagerUri)
        {
            bool bRet = false;

            string table = "servicesOpenManager";
            string[] keyList = new string[1];
            keyList[0] = "webManagerUrl";

            string[] valueList = new string[1];
            valueList[0] = webManagerUri.ToString();

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.UpdateQuery(keyList, table, valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Update Query");
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        #endregion


        #region Log Events

        public bool SetRegisterEventLog(DateTime eventDate, string softwareVersion, string eventLevel, string eventMessage, string eventApplication)
        {
            bool bRet = false;

            string table = "eventLog";
            string[] keyList = new string[5];
            keyList[0] = "date";
            keyList[1] = "softwareVersion";
            keyList[2] = "loggerLevel";
            keyList[3] = "message";
            keyList[4] = "application";
            string[] valueList = new string[5];
            valueList[0] = eventDate.ToString();
            valueList[1] = softwareVersion;
            valueList[2] = eventLevel;
            valueList[3] = eventMessage;
            valueList[4] = eventApplication;

            try
            {
                m_semaphoreData.WaitOne();
                bRet = m_DBConnection.InsertQuery(keyList, table, ref valueList);

                if (!bRet)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error with DB Insert Query");
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_semaphoreData.Release();
            }

            return bRet;
        }

        #endregion

    }
}
