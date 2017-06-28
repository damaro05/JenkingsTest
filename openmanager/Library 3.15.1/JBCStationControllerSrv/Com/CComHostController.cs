// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;
using System.Threading;
using System.IO;
using HostControllerServiceReference;

using DataJBC;
using RoutinesJBC;


namespace JBCStationControllerSrv
{
    public class CComHostController : CSearcherHostControllerServices
    {

        private const int TIME_SEND_VERSION_HOST_CONTROLLER = 2 * 60 * 1000; //2 minutes
        private const int TIME_SEND_EVENT_LOG_HOST_CONTROLLER = 20 * 60 * 1000; //20 minutes


        private string m_swVersion;
        private Thread m_ThreadVersionSoftware;
        private Thread m_ThreadEventLog;
        private CEventLog m_eventLog = new CEventLog();


        public CComHostController()
        {
            m_swVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


            //Send software version
            m_ThreadVersionSoftware = new Thread(new System.Threading.ThreadStart(SendVersionFileToController));
            m_ThreadVersionSoftware.IsBackground = true;
            m_ThreadVersionSoftware.Start();

            //Send event log
            m_ThreadEventLog = new Thread(new System.Threading.ThreadStart(SendEventLogToController));
            m_ThreadEventLog.IsBackground = true;
            m_ThreadEventLog.Start();

            //Events from JBC_Connect
            DLLConnection.jbc.GetUpdateFirmware += GetUpdateFirmware;
        }

        /// <summary>
        /// Send software version to HostController
        /// </summary>
        private void SendVersionFileToController()
        {
            // inicialmente esperamos 10 segundos para que el searcher tenga tiempo suficiente de realizar su búsqueda
            Thread.Sleep(10000);

            do
            {
                try
                {
                    m_mutexHostControllerEndpoints.WaitOne();
                    foreach (EndpointAddress ep in m_hostControllerEndpoints)
                    {
                        //Open connection
                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                        JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                        serviceClient.Open();
                        serviceClient.CheckUpdateConnectedStationController(m_swVersion);
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

                Thread.Sleep(TIME_SEND_VERSION_HOST_CONTROLLER);
            } while (true);
        }

        private void SendEventLogToController()
        {
            do
            {
                List<dc_EventLog> eventLog = m_eventLog.GetLogEvents();

                try
                {
                    m_mutexHostControllerEndpoints.WaitOne();
                    foreach (EndpointAddress ep in m_hostControllerEndpoints)
                    {
                        //Open connection
                        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                        JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);

                        serviceClient.Open();
                        serviceClient.RegisterEventLog(eventLog.ToArray());
                        serviceClient.Close();
                    }

                    //Updates the date of the last data collection
                    My.Settings.Default.EventLogLastDataCollection = DateTime.Now;
                }
                catch (Exception ex)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                }
                finally
                {
                    m_mutexHostControllerEndpoints.ReleaseMutex();
                }

                Thread.Sleep(TIME_SEND_EVENT_LOG_HOST_CONTROLLER);
            } while (true);
        }

        public void GetUpdateFirmware(ref System.Collections.Generic.List<CFirmwareStation> versionMicros)
        {

            List<CFirmwareStation> versionMicrosHostController = new List<CFirmwareStation>();

            try
            {
                //Carpeta donde guardar la descarga
                string firmwareFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\JBC Station Controller Service\\Firmwares");

                //Crear carpeta temporal
                if (!(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DirectoryExists(firmwareFolder))
                {
                    Directory.CreateDirectory(firmwareFolder);
                }

                //Endpoint HostController
                EndpointAddress ep = null;

                m_mutexHostControllerEndpoints.WaitOne();
                if (m_hostControllerEndpoints.Count > 0)
                {
                    ep = m_hostControllerEndpoints.First();
                }
                m_mutexHostControllerEndpoints.ReleaseMutex();

                if (ep != null)
                {

                    //Open connection
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCHostControllerServiceClient serviceClient = new JBCHostControllerServiceClient(binding, ep);
                    serviceClient.Open();

                    //Recorrer todos los micros
                    for (int i = 0; i <= versionMicros.Count - 1; i++)
                    {

                        //Obtener información del firmware
                        HostControllerServiceReference.dc_FirmwareStation infoUpdateFirmware = new HostControllerServiceReference.dc_FirmwareStation();
                        infoUpdateFirmware.model = System.Convert.ToString(versionMicros.ElementAt(i).Model);
                        infoUpdateFirmware.hardwareVersion = System.Convert.ToString(versionMicros.ElementAt(i).HardwareVersion);

                        List<HostControllerServiceReference.dc_FirmwareStation> listNewInfoUpdateFirmware = new List<HostControllerServiceReference.dc_FirmwareStation>();
                        listNewInfoUpdateFirmware.AddRange(serviceClient.GetInfoUpdateFirmware(infoUpdateFirmware));
                        foreach (HostControllerServiceReference.dc_FirmwareStation dc_firmware in listNewInfoUpdateFirmware)
                        {
                            CFirmwareStation firmware = new CFirmwareStation();
                            firmware.Model = dc_firmware.model;
                            firmware.ModelVersion = dc_firmware.modelVersion;
                            firmware.ProtocolVersion = dc_firmware.protocolVersion;
                            firmware.HardwareVersion = dc_firmware.hardwareVersion;
                            firmware.SoftwareVersion = dc_firmware.softwareVersion;
                            firmware.FileName = Path.Combine(firmwareFolder, dc_firmware.fileName);
                            versionMicrosHostController.Add(firmware);

                            //Si no existe el archivo de firmware
                            if (!File.Exists(firmware.FileName))
                            {

                                bool bOk = false;
                                bool bContinue = false;
                                int nSequence = 1;

                                //Download firmware update
                                do
                                {
                                    int nTries = 3;
                                    bOk = false;
                                    dc_UpdateFirmware updateFirmware = new dc_UpdateFirmware();

                                    while (nTries > 0 && !bOk)
                                    {
                                        updateFirmware = serviceClient.GetFileUpdateFirmware(nSequence, dc_firmware.fileName);
                                        bOk = updateFirmware.sequence == nSequence;
                                        bContinue = !updateFirmware.final;

                                        nTries--;
                                    }

                                    if (!bOk)
                                    {
                                        break;
                                    }
                                    nSequence++;

                                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllBytes(firmware.FileName, updateFirmware.bytes, true);
                                } while (bContinue);
                            }
                        }
                    }

                    serviceClient.Close();
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }

            versionMicros = versionMicrosHostController;
        }

    }
}
