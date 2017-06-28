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

using System.Configuration;
using System.Configuration.Install;
using System.ServiceModel;
using System.ServiceProcess;

namespace JBCTraceControlLocalSrv
{
    public class JBCTraceControlLocalSrv : ServiceBase
    {

        public const string WINDOWS_SERVICE_NAME = "JBCTraceControlLocalService";

        private ServiceHost m_serviceHost = null;
        private CTraceLocal m_TraceLocal = null;

        private enum enumAction
        {
            aUnknown,
            aInstall,
            aUninstall
        }

        private enum enumCommand
        {
            seUnknown = 0,
            aStop = 3,
            aStart = 4,
            aRestart = 5,
            // service custom commands 128 - 256
            startUSB = 128,
            startETH = 129,
            stopUSB = 130,
            stopETH = 131,
            initUSB = 132,
            initETH = 133,
            initALL = 134,
            initNONE = 135
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        public JBCTraceControlLocalSrv()
        {
            //Name the windows service
            ServiceName = WINDOWS_SERVICE_NAME;

            //Settings
            if (My.Settings.Default.UpgradeSettings)
            {
                My.Settings.Default.Upgrade();
                My.Settings.Default.UpgradeSettings = false;
                My.Settings.Default.Save();
            }
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
            LoggerModule.logger.Info(Localization.getResStr(modL10nData.EV_STARTING_ID));

            //It's used in install mode
            if (System.Environment.UserInteractive)
            {

                bool bInstall = false;
                bool bUninstall = false;
                enumAction action = enumAction.aUnknown;
                enumCommand search = enumCommand.seUnknown;
                enumCommand searchInit = enumCommand.seUnknown;
                int timeoutMilliseconds = 30000;
                List<int> listCommands = new List<int>();

                // auto instala o desinstala el servicio windows
                foreach (string argument in (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).CommandLineArgs)
                {
                    switch (argument.ToLower())
                    {
                        case "-install":
                            action = enumAction.aInstall;
                            break;
                        case "-uninstall":
                            action = enumAction.aUninstall;
                            break;
                        case "-stop":
                            listCommands.Add((int)enumCommand.aStop);
                            break;
                        case "-start":
                            listCommands.Add((int)enumCommand.aStart);
                            break;
                        case "-restart":
                            listCommands.Add((int)enumCommand.aRestart);
                            break;
                        case "-initnone":
                            listCommands.Add((int)enumCommand.initNONE);
                            break;
                        case "-initall":
                            listCommands.Add((int)enumCommand.initALL);
                            break;
                        case "-initusb":
                            listCommands.Add((int)enumCommand.initUSB);
                            break;
                        case "-initeth":
                            listCommands.Add((int)enumCommand.initETH);
                            break;
                        case "-startusb":
                            listCommands.Add((int)enumCommand.startUSB);
                            break;
                        case "-starteth":
                            listCommands.Add((int)enumCommand.startETH);
                            break;
                        case "-stopusb":
                            listCommands.Add((int)enumCommand.stopUSB);
                            break;
                        case "-stopeth":
                            listCommands.Add((int)enumCommand.stopETH);
                            break;
                    }
                }

                switch (action)
                {
                    case enumAction.aInstall:
                        try
                        {
                            // install
                            ManagedInstallerClass.InstallHelper(new string[] { System.Reflection.Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception ex)
                        {
                            LoggerModule.logger.Error("Installing service. " + ex.Message);
                            retVal = 1;
                        }
                        break;

                    case enumAction.aUninstall:
                        try
                        {
                            // uninstall
                            ManagedInstallerClass.InstallHelper(new string[] { "/u", System.Reflection.Assembly.GetExecutingAssembly().Location });
                        }
                        catch (Exception ex)
                        {
                            LoggerModule.logger.Error("Uninstalling service. " + ex.Message);
                            retVal = 1;
                        }
                        break;

                    default:
                        ServiceController service = new ServiceController(WINDOWS_SERVICE_NAME);

                        if (service != null)
                        {
                            // it is installed
                            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                            foreach (enumCommand comm in listCommands)
                            {
                                switch (comm)
                                {
                                    case enumCommand.aStart:
                                        try
                                        {
                                            service.Start();
                                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerModule.logger.Error("Starting service. " + ex.Message);
                                            retVal = 1;
                                        }
                                        break;

                                    case enumCommand.aStop:
                                        try
                                        {
                                            service.Stop();
                                            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerModule.logger.Error("Stopping service. " + ex.Message);
                                            retVal = 1;
                                        }
                                        break;

                                    case enumCommand.aRestart:
                                        try
                                        {
                                            if (service.Status == ServiceControllerStatus.Running)
                                            {
                                                service.Stop();
                                                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                                            }
                                            service.Start();
                                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerModule.logger.Error("Restarting service. " + ex.Message);
                                            retVal = 1;
                                        }
                                        break;

                                    case enumCommand.startUSB:
                                    case enumCommand.startETH:
                                    case enumCommand.stopUSB:
                                    case enumCommand.stopETH:
                                        try
                                        {
                                            // should be started for custom commands
                                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                                            service.ExecuteCommand((System.Int32)comm);
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerModule.logger.Error(string.Format("Error trying to custom command {0}: " + ex.Message, comm.ToString()));
                                            retVal = 1;
                                        }
                                        break;

                                    case enumCommand.initALL:
                                    case enumCommand.initETH:
                                    case enumCommand.initUSB:
                                    case enumCommand.initNONE:
                                        try
                                        {
                                            // should be started for custom commands
                                            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                                            service.ExecuteCommand((System.Int32)comm);
                                        }
                                        catch (Exception ex)
                                        {
                                            LoggerModule.logger.Error(string.Format("Error trying to custom command {0}: " + ex.Message, comm.ToString()));
                                            retVal = 1;
                                        }
                                        break;
                                }
                            }
                        }

                        //Poder ejecutar el servicio sin necesidad de instalarlo
#if DEBUG_TRACECONTROLLOCAL && DEBUG
					My.Settings.Default.SearchUSB = true;
					My.Settings.Default.SearchETH = true;
						
					string[] args = new string[1];
					JBCTraceControlLocalSrv serv = new JBCTraceControlLocalSrv();
					serv.OnStart(args);
					while (true)
					{
						Console.Read();
					}
					serv.OnStop();
#endif
                        break;

                }
            }
            else
            {
                // crea el servicio windows
                ServiceBase.Run(new JBCTraceControlLocalSrv());
            }

            return retVal;
        }

        protected override void OnStart(string[] args)
        {
            string sErr = "";

            // initialize DLL and start USB and Eth searches
            if (DLLConnection.CDLLConnectionController.InitDLL() == true)
            {

                // iniciar trace
                m_TraceLocal = new CTraceLocal(DLLConnection.jbc);
                if (m_TraceLocal.startTrace())
                {
                    LoggerModule.logger.Info(Localization.getResStr(modL10nData.EV_STARTED_ID));
                }
                else
                {
                    LoggerModule.logger.Error(string.Format("{0} ({1})", Localization.getResStr(modL10nData.EV_ERROR_CREATING_ID), sErr));
                }

            }

        }

        protected override void OnStop()
        {
            LoggerModule.logger.Info(Localization.getResStr(modL10nData.EV_STOPPING_ID));

            // parar trace
            m_TraceLocal.Dispose();

            // parar Dll
            DLLConnection.CDLLConnectionController.ReleaseDLL();

            LoggerModule.logger.Info(Localization.getResStr(modL10nData.EV_STOPPED_ID));

        }

    }
}
