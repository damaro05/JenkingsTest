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

using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.ServiceModel;
using System.ServiceProcess;
using RoutinesJBC;

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Creates a Host Controller windows service on the local machine
    /// </summary>
    public class JBCHostControllerService : ServiceBase
    {

        internal const string WINDOWS_SERVICE_NAME = "JBCHostControllerService";


        private ServiceHost m_serviceHost = null;
        private RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPHost m_discoveryServiceHost;


        /// <summary>
        /// Class constructor
        /// </summary>
        public JBCHostControllerService()
        {
            //Name the windows service
            ServiceName = WINDOWS_SERVICE_NAME;
        }

        /// <summary>
        /// Install or initialize the service
        /// </summary>
        /// <returns>Return 0 is operation is succesfull, 1 otherwise</returns>
        public static int Main()
        {
            int retVal = 0;

            //Initialize the Event Log
            LoggerModule.InitLogger();

            //It's used in install mode
            if (System.Environment.UserInteractive)
            {

                bool bInstall = false;
                bool bUninstall = false;
                bool bStart = false;
                bool bStop = false;
                int timeoutMilliseconds = 3000;

                // auto instala o desinstala el servicio windows
                foreach (string argument in (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).CommandLineArgs)
                {
                    switch (argument.ToLower())
                    {
                        case "-install":
                            bInstall = true;
                            break;
                        case "-uninstall":
                            bUninstall = true;
                            break;
                        case "-stop":
                            bStop = true;
                            break;
                        case "-start":
                            bStart = true;
                            break;
                    }
                }

                // install
                if (bInstall)
                {
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { System.Reflection.Assembly.GetExecutingAssembly().Location });
                    }
                    catch (Exception ex)
                    {
                        LoggerModule.logger.Error("Error trying to install: " + ex.Message);
                        retVal = 1;
                    }

                    // uninstall
                }
                else if (bUninstall)
                {
                    try
                    {
                        ManagedInstallerClass.InstallHelper(new string[] { "/u", System.Reflection.Assembly.GetExecutingAssembly().Location });
                    }
                    catch (Exception ex)
                    {
                        LoggerModule.logger.Error("Error trying to uninstall: " + ex.Message);
                        retVal = 1;
                    }

                    // it is installed
                }
                else
                {
                    ServiceController service = new ServiceController(WINDOWS_SERVICE_NAME);

                    if (service != null)
                    {
                        TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                        if (bStart)
                        {
                            try
                            {
                                service.Start();
                                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                            }
                            catch (Exception ex)
                            {
                                LoggerModule.logger.Error("Error trying to start: " + ex.Message);
                                retVal = 1;
                            }

                        }
                        else if (bStop)
                        {
                            try
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                            }
                            catch (Exception ex)
                            {
                                LoggerModule.logger.Error("Error trying to stop: " + ex.Message);
                                retVal = 1;
                            }
                        }
                    }
                }

                //Poder ejecutar el servicio sin necesidad de instalarlo
#if DEBUG
			string[] args = new string[1]();
			JBCHostControllerService serv = new JBCHostControllerService();
			serv.OnStart(args);
			while (true)
			{
				Console.Read();
			}
			serv.OnStop();
#endif

            }
            else
            {
                // crea el servicio windows
                ServiceBase.Run(new JBCHostControllerService());
            }

            return retVal;
        }

        /// <summary>
        /// Start the Windows service
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            string sErr = "";

            //Initialize WCF service
            if (RoutinesLibrary.Net.WCF.WCFServiceServer.OpenWCFServiceBasic(ref sErr, ref m_serviceHost, typeof(CImplIJBCHostControllerService),
                typeof(IJBCHostControllerService), "JBCHostControllerSrv/service"))
            {
                // initialize discovery service. other services can discovery this one
                m_discoveryServiceHost = new RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPHost(SearcherServicesData.MESSAGE_HOSTCONTROLLER_DISCOVERY_REQUEST,
                    SearcherServicesData.MESSAGE_HOSTCONTROLLER_DISCOVERY_RESPONSE,
                    SearcherServicesData.PORT_HOSTCONTROLLER_DISCOVERY,
                    m_serviceHost.BaseAddresses[0].ToString());
                LoggerModule.logger.Info("Started HostController");
            }
            else
            {
                LoggerModule.logger.Error("Error Starting HostController: " + sErr);
            }
        }

        /// <summary>
        /// Stop the Windows service
        /// </summary>
        protected override void OnStop()
        {
            LoggerModule.logger.Info("Stopping HostController");

            m_discoveryServiceHost.Dispose();
            RoutinesLibrary.Net.WCF.WCFServiceServer.CloseWCFService(ref m_serviceHost);

            LoggerModule.logger.Info("Stopped HostController");
        }

    }
}
