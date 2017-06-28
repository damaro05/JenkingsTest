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
using System.ServiceModel;
using System.Threading;
using System.Timers;
using JBCHostControllerSrv.JBCUpdaterService;

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Manage the update process of a service
    /// </summary>
    public class CComUpdaterService
    {

        private const int CHUNK_SIZE = 10 * 1024; //10kb. The buffer size by default is set to 64kb
        private const int WAIT_READY_UPDATER_TRIES = 18;
        private const int WAIT_READY_UPDATER_TIME = 10 * 1000; //10 seconds
        private const int TIME_RETRY_UPDATE = 10 * 60 * 1000; //10 minutes


        private Collection m_pendingUpdates;
        private Thread m_ThreadPendingUpdates;
        private bool m_IsAliveThreadPendingUpdates = true;
        private static Semaphore m_mutexPendingUpdates = new Semaphore(1, 1);


        /// <summary>
        /// Class constructor
        /// </summary>
        public CComUpdaterService()
        {
            m_pendingUpdates = new Collection();

            m_ThreadPendingUpdates = new Thread(new System.Threading.ThreadStart(PendingUpdates));
            m_ThreadPendingUpdates.IsBackground = true;
            m_ThreadPendingUpdates.Start();
        }

        /// <summary>
        /// Release class resources
        /// </summary>
        public void Dispose()
        {
            m_IsAliveThreadPendingUpdates = false;
            m_pendingUpdates.Clear();
            m_pendingUpdates = null;
        }

        /// <summary>
        /// Install an update on a windows service
        /// </summary>
        /// <param name="pathSw">Update file path</param>
        /// <param name="ip">Widnows service's IP</param>
        /// <returns>True if the update operation was successful</returns>
        public bool SendUpdateSw(string pathSw, string ip)
        {

            bool bOk = false;
            UriBuilder uri = new UriBuilder("http", ip, 8000, "/JBCUpdaterSrv/service");
            EndpointAddress ep = new EndpointAddress(uri.ToString());

            try
            {
                //Comprobamos de que el elemento no esté en la cola
                m_mutexPendingUpdates.WaitOne();
                bool bExists = m_pendingUpdates.Contains(ep.ToString());
                m_mutexPendingUpdates.Release();
                if (bExists)
                {
                    return bOk;
                }

                //Espera a que el updater pueda recibir peticiones
                if (WaitReadyUpdater(ep))
                {
                    if (SendFile(ep, pathSw))
                    {
                        SendInitUpdateCommand(ep);

                        //Guardamos el elemento enviado
                        CUpdaterServicePending stPendingCom = new CUpdaterServicePending(ep, pathSw);
                        m_mutexPendingUpdates.WaitOne();
                        m_pendingUpdates.Add(stPendingCom, ep.ToString(), null, null); //utilizamos el endpoint como key list
                        m_mutexPendingUpdates.Release();

                        bOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }

            return bOk;
        }

        /// <summary>
        /// Wait until the windows service is ready to accept updates
        /// </summary>
        /// <param name="ep">Windows service endpoint</param>
        /// <returns>True if the windows service is ready to accept updates</returns>
        private bool WaitReadyUpdater(EndpointAddress ep)
        {

            bool bReady = false;
            int nTries = WAIT_READY_UPDATER_TRIES;
            int nTimeWait = WAIT_READY_UPDATER_TIME;

            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            JBCUpdaterServiceClient serviceClient = new JBCUpdaterServiceClient(binding, ep);

            try
            {
                serviceClient.Open();
                do
                {
                    if (serviceClient.StateUpdate() == dc_EnumConstJBCdc_UpdateState.Updating)
                    {
                        nTries--;
                        Thread.Sleep(nTimeWait);
                    }
                    else
                    {
                        bReady = true;
                    }
                } while (nTries > 0 && !bReady);

                if (nTries == 0)
                {
                    LoggerModule.logger.Warn(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Time exceded");
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            finally
            {
                serviceClient.Close();
            }

            return bReady;
        }

        /// <summary>
        /// Send update file to windows service
        /// </summary>
        /// <param name="endpoint">Windows service's endpoint</param>
        /// <param name="pathSw">Update file path</param>
        /// <returns>True if the file is sent successful</returns>
        private bool SendFile(EndpointAddress endpoint, string pathSw)
        {
            bool bOk = false;

            //Open connection
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            UriBuilder uri = new UriBuilder(endpoint.Uri.Scheme, endpoint.Uri.Host, endpoint.Uri.Port, "/JBCUpdaterSrv/service");
            EndpointAddress updateEndpoint = new EndpointAddress(uri.ToString());
            JBCUpdaterServiceClient serviceClient = new JBCUpdaterServiceClient(binding, updateEndpoint);

            serviceClient.Open();

            //Input file
            int nSequence = 1;
            FileStream fileStream = new FileStream(pathSw, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            byte[] bytes = null;

            do
            {
                bytes = binaryReader.ReadBytes(CHUNK_SIZE);
                if (bytes.Length > 0)
                {
                    int nTries = 3;
                    bOk = false;

                    while (nTries > 0 && !bOk)
                    {
                        bOk = serviceClient.ReceiveFile(nSequence, bytes) == nSequence;
                        if (!bOk)
                        {
                            System.Threading.Thread.Sleep(3000); //Esperamos un tiempo antes de intentar volver a enviar el paquete
                        }

                        nTries--;
                    }
                }

                if (!bOk)
                {
                    break;
                }
                nSequence++;
            } while (bytes.Length > 0);

            //Close file and connection
            fileStream.Close();
            serviceClient.Close();

            if (!bOk)
            {
                LoggerModule.logger.Warn(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: can't send data");
            }

            return bOk;
        }

        /// <summary>
        /// Send updater command to windows service
        /// </summary>
        /// <param name="ep">Windows service's endpoint</param>
        private void SendInitUpdateCommand(EndpointAddress ep)
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            JBCUpdaterServiceClient serviceClient = new JBCUpdaterServiceClient(binding, ep);

            serviceClient.Open();
            serviceClient.InitUpdate();
            serviceClient.Close();
        }

        /// <summary>
        /// Periodically check the status of update processes and resend the update file if necessary
        /// </summary>
        private void PendingUpdates()
        {
            while (m_IsAliveThreadPendingUpdates)
            {


                m_mutexPendingUpdates.WaitOne();
                var pendingUpdatesFinal = m_pendingUpdates;

                foreach (CUpdaterServicePending pendingCom in m_pendingUpdates)
                {

                    //Obtenemos el estado de la actualización
                    BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    JBCUpdaterServiceClient serviceClient = new JBCUpdaterServiceClient(binding, pendingCom.endPoint);
                    serviceClient.Open();
                    dc_EnumConstJBCdc_UpdateState state = serviceClient.StateUpdate();
                    serviceClient.Close();

                    //
                    // Success operation
                    //
                    if (state == dc_EnumConstJBCdc_UpdateState.Finished)
                    {
                        pendingUpdatesFinal.Remove(pendingCom.endPoint.ToString());

                        //
                        // Fail or Updating
                        //
                    }
                    else
                    {
                        if (pendingCom.nTriesRemainingUpdate == 0)
                        {
                            pendingUpdatesFinal.Remove(pendingCom.endPoint.ToString());
                            //TODO send error message
                        }
                        else
                        {
                            pendingCom.DecrementRemainingUpdate();
                            pendingUpdatesFinal.Remove(pendingCom.endPoint.ToString());
                            pendingUpdatesFinal.Add(pendingCom, pendingCom.endPoint.ToString(), null, null);

                            if (SendFile(pendingCom.endPoint, pendingCom.sUrlSendFile))
                            {
                                SendInitUpdateCommand(pendingCom.endPoint);
                            }
                            else
                            {
                                pendingUpdatesFinal.Remove(pendingCom.endPoint.ToString());
                                //TODO send error message
                            }
                        }
                    }
                }

                m_pendingUpdates = pendingUpdatesFinal;
                m_mutexPendingUpdates.Release();

                Thread.Sleep(TIME_RETRY_UPDATE);
            }
        }

    }
}
