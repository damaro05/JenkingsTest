// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ComponentModel;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;

namespace JBCUpdaterSrv
{
    // Clase para la creación de un servicio windows en el host
    public class JBCUpdater : ServiceBase
    {

        // nombre del servicio de windows
        public const string cWindowsServiceName = "JBCUpdaterService";


        public JBCUpdater()
        {
            // Name the Windows Service
            ServiceName = cWindowsServiceName;
        }

        private string[] args { get; set; }

        public static int Main()
        {
            LoggerModule.InitLogger();

            int retVal = 0;

            if (System.Environment.UserInteractive)
            {
                bool bInstall = false;
                bool bUninstall = false;
                bool bStart = false;
                bool bStop = false;
                int timeoutMilliseconds = 3000;

                //Dim bSilent As Boolean = True
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
                }
                else
                {
                    ServiceController service = new ServiceController(cWindowsServiceName);
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

#if DEBUG
				string[] args = new string[1]();
				JBCUpdater serv = new JBCUpdater();
				serv.OnStart(args);
				while (true)
				{
					Console.Read();
				}
				serv.OnStop();
#endif

                }
            }
            else
            {
                // crea el servicio windows
                ServiceBase.Run(new JBCUpdater());
            }

            return retVal;

        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {


            LoggerModule.logger.Info(Localization.getResStr(modCommon.evStartingId));

            string sErr = "";

            if (RoutinesLibrary.Net.WCF.WCFServiceServer.OpenWCFServiceBasic(ref sErr, ref modCommon.serviceHost, typeof(JBCUpdaterService),
                typeof(IJBCUpdaterService), "JBCUpdaterSrv/service"))
            {
                LoggerModule.logger.Info(Localization.getResStr(modCommon.evStartedId));
            }
            else
            {
                LoggerModule.logger.Error(string.Format("{0} ({1})", Localization.getResStr(modCommon.evErrorCreatingId), sErr));
            }
        }

        // Stop the Windows service
        protected override void OnStop()
        {
            LoggerModule.logger.Info(Localization.getResStr(modCommon.evStoppingId));
            RoutinesLibrary.Net.WCF.WCFServiceServer.CloseWCFService(ref modCommon.serviceHost);
            LoggerModule.logger.Info(Localization.getResStr(modCommon.evStoppedId));
        }

    }


    // Provide the ProjectInstaller class which allows
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {

        private ServiceProcessInstaller process;
        private ServiceInstaller service;


        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = JBCUpdater.cWindowsServiceName;
            service.Description = "JBC Service to update any other JBC service by exposing JBC Connect API thru WCF";
            service.StartType = ServiceStartMode.Automatic;
            Installers.Add(process);
            Installers.Add(service);
        }

    }
}
