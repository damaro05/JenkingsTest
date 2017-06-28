// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

using System.ServiceModel;
using System.Threading;
using RemoteManager.HostControllerServiceReference;
using RoutinesJBC;

namespace RemoteManager
{
    public class CComHostController : CSearcherHostControllerServices
    {


        #region FilesLocation

        public bool IsAvailableRemoteServerDownload()
        {
            bool available = true;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    available = serviceClient.IsAvailableRemoteServerDownload();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return available;
        }

        public bool SetAvailableRemoteServerDownload(bool available)
        {
            bool bOk = false;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    bOk = serviceClient.SetAvailableRemoteServerDownload(available);
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return bOk;
        }

        public string GetUserFilesLocalFolderLocation()
        {
            string path = "";

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    path = serviceClient.GetUserFilesLocalFolderLocation();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return path;
        }

        public bool SetUserFilesLocalFolderLocation(string path)
        {
            bool bOk = false;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    bOk = serviceClient.SetUserFilesLocalFolderLocation(path);
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return bOk;
        }

        #endregion


        #region Update

        //@Brief Obtiene la información de la existencia de una actualización disponible
        //@Return dc_InfoUpdateSoftware Información de la actualización
        public dc_InfoUpdateSoftware CheckUpdate()
        {

            dc_InfoUpdateSoftware infoUpdate = new dc_InfoUpdateSoftware();
            infoUpdate.stationControllerSwAvailable = false;
            infoUpdate.remoteManagerSwAvailable = false;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    infoUpdate = serviceClient.CheckUpdate();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return infoUpdate;
        }

        //@Brief Obtiene la última actualización
        public void UpdateSystem()
        {

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    serviceClient.UpdateSystem();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

        }

        //@Brief Obtiene la información de la actualización periódica
        //@Return dc_InfoUpdatePeriodicTime Información de la actualización periódica
        public dc_InfoUpdatePeriodicTime GetUpdatePeriodicTime()
        {

            dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime = new dc_InfoUpdatePeriodicTime();
            infoUpdatePeriodicTime.available = false;
            infoUpdatePeriodicTime.modeDaily = true;
            infoUpdatePeriodicTime.hour = (byte)0;
            infoUpdatePeriodicTime.minute = (byte)0;
            infoUpdatePeriodicTime.weekday = (byte)1;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    infoUpdatePeriodicTime = serviceClient.GetUpdatePeriodicTime();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return infoUpdatePeriodicTime;
        }

        //@Brief Establece la actualización periódica
        //@Param[in] infoUpdatePeriodicTime Información de la actualización periódica
        public void SetUpdatePeriodicTime(dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime)
        {

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceclient = new JBCHostControllerServiceClient(binding, ep);

                    serviceclient.Open();
                    bool bok = serviceclient.SetUpdatePeriodicTime(infoUpdatePeriodicTime);
                    serviceclient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

        }

        //@Brief Obtiene la información de la comprobación periódica
        //@Return dc_InfoCheckPeriodicTime Información de la comprobación periódica
        public dc_InfoCheckPeriodicTime GetCheckPeriodicTime()
        {

            dc_InfoCheckPeriodicTime infoCheckPeriodicTime = new dc_InfoCheckPeriodicTime();
            infoCheckPeriodicTime.available = false;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    infoCheckPeriodicTime = serviceClient.GetCheckPeriodicTime();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return infoCheckPeriodicTime;
        }

        //@Brief Establece la comprobación periódica
        //@Param[in] infoCheckPeriodicTime Información de la comprobación periódica
        public void SetCheckPeriodicTime(dc_InfoCheckPeriodicTime infoCheckPeriodicTime)
        {

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceclient = new JBCHostControllerServiceClient(binding, ep);

                    serviceclient.Open();
                    bool bok = serviceclient.SetCheckPeriodicTime(infoCheckPeriodicTime);
                    serviceclient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

        }

        //@Brief Obtiene la información de la actualización específica
        //@Return dc_InfoUpdateSpecificTime Información de la actualización específica
        public dc_InfoUpdateSpecificTime GetUpdateSpecificTimeInfo()
        {

            dc_InfoUpdateSpecificTime infoUpdateSpecificTime = new dc_InfoUpdateSpecificTime();
            infoUpdateSpecificTime.available = false;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    infoUpdateSpecificTime = serviceClient.GetUpdateSpecificTime();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return infoUpdateSpecificTime;
        }

        //@Brief Establece la actualización específica
        //@Param[in] infoUpdateSpecificTime Información de la actualización específica
        public void SetUpdateSpecificTime(dc_InfoUpdateSpecificTime infoUpdateSpecificTime)
        {

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    serviceClient.SetUpdateSpecificTime(infoUpdateSpecificTime);
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

        }

        //@Brief Comprueba si existe una actualización para el RemoteManager
        //@Paran[in] swVersion Versión del RemoteManager
        //@Return Boolean True si existe una actualización
        public bool CheckUpdateConnectedRemoteManager(string swVersion)
        {

            bool bOk = false;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    bOk = serviceClient.CheckUpdateConnectedRemoteManager(swVersion);
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return bOk;
        }

        //@Brief Obtiene una porción de la actualización
        //@Param[in] nSequence Número de la secuencia
        //@Return dc_UpdateWProgram Estructura con los datos de la actualización
        public dc_UpdateRemoteManager GetFileUpdateRemoteManager(int nSequence)
        {

            dc_UpdateRemoteManager updateRemoteManager = new dc_UpdateRemoteManager();
            updateRemoteManager.sequence = -1;
            updateRemoteManager.final = false;

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    updateRemoteManager = serviceClient.GetFileUpdateRemoteManager(nSequence);
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return updateRemoteManager;
        }

        //@Brief Obtiene la versión del software del StationController
        //@Return String Versión del software del StationController
        public string GetSwStationController()
        {

            string swStationController = "";

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    swStationController = serviceClient.GetStationControllerSwVersion();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return swStationController;
        }

        //@Brief Obtiene la versión del software del HostController
        //@Return String Versión del software del HostController
        public string GetSwHostController()
        {

            string swHostController = "";

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    swHostController = serviceClient.GetHostControllerSwVersion();
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return swHostController;
        }

        #endregion


        #region Update stations

        public List<dc_FirmwareStation> GetInfoUpdateFirmware(dc_FirmwareStation infoUpdateFirmware)
        {
            List<dc_FirmwareStation> listInfoUpdateFirmware = new List<dc_FirmwareStation>();

            try
            {
                m_mutexHostControllerEndpoints.WaitOne();
                foreach (EndpointAddress ep in m_hostControllerEndpoints)
                {
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                    serviceClient.Open();
                    listInfoUpdateFirmware.AddRange(serviceClient.GetInfoUpdateFirmware(infoUpdateFirmware));
                    serviceClient.Close();
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                m_mutexHostControllerEndpoints.ReleaseMutex();
            }

            return listInfoUpdateFirmware;
        }

        #endregion

    }
}
