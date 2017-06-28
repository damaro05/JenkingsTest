using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Configuration.Install;
using System.ServiceModel;
using System.ServiceProcess;

using RoutinesJBC;
using DataJBC;


namespace JBCStationControllerSrv
{
    /// <summary>
    /// Creates a Station Controller windows service on the local machine
    /// </summary>
    public class JBCStationControllerService : ServiceBase
    {
        public const string WINDOWS_SERVICE_NAME = "JBCStationControllerService";


        private ServiceHost m_serviceHost = null;
        private RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPHost m_discoveryServiceHost;


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
        public JBCStationControllerService()
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
            LoggerModule.logger.Info(My.Resources.Resources.evStarting);

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
                foreach (string argument in Environment.GetCommandLineArgs())
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
#if DEBUG_STATIONCONTROLLER && DEBUG
					My.Settings.Default.SearchUSB = true;
					My.Settings.Default.SearchETH = true;

					string[] args = new string[1];
					JBCStationControllerService serv = new JBCStationControllerService();
					serv.OnStart(args);
					while (true)
					{
						try
						{
                            System.Threading.Thread.Sleep(1000);
						}
						catch (Exception)
						{
						}
					}
					serv.OnStop();
#endif
                    break;
                }
            }
            else
            {
                // crea el servicio windows
                ServiceBase.Run(new JBCStationControllerService());
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
            //System.Diagnostics.Debugger.Launch();
            // initialize DLL and start USB and Eth searches
            DLLConnection.CDLLConnectionController.InitDLL();

            //Initialize WCF service
            if (RoutinesLibrary.Net.WCF.WCFServiceServer.OpenWCFServiceBasic(ref sErr,
                                                                             ref m_serviceHost,
                                                                             typeof(CImplIJBCStationControllerService),
                                                                             typeof(IJBCStationControllerService),
                                                                             "JBCStationControllerSrv/service"))
            {
                // initialize discovery service. other services can discovery this one
                m_discoveryServiceHost = new RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPHost(SearcherServicesData.MESSAGE_STATIONCONTROLLER_DISCOVERY_REQUEST,
                                                                                               SearcherServicesData.MESSAGE_STATIONCONTROLLER_DISCOVERY_RESPONSE,
                                                                                               SearcherServicesData.PORT_STATIONCONTROLLER_DISCOVERY,
                                                                                               m_serviceHost.BaseAddresses[0].ToString());

                LoggerModule.logger.Info(My.Resources.Resources.evStarted);
            }
            else
            {
                LoggerModule.logger.Error(string.Format("{0} ({1})", My.Resources.Resources.evErrorCreating, sErr));
            }
        }

        /// <summary>
        /// Stop the Windows service
        /// </summary>
        protected override void OnStop()
        {
            LoggerModule.logger.Info(My.Resources.Resources.evStopping);

            m_discoveryServiceHost.Dispose();
            RoutinesLibrary.Net.WCF.WCFServiceServer.CloseWCFService(ref m_serviceHost);
            DLLConnection.CDLLConnectionController.ReleaseDLL();

            LoggerModule.logger.Info(My.Resources.Resources.evStopped);
        }

        protected override void OnCustomCommand(int command)
        {
            LoggerModule.logger.Info(Strings.Format("Custom Command: {0}", command.ToString()));

            bool bSave = false;

            try
            {
                switch (command)
                {
                    case (int)enumCommand.startETH:
                        if (DLLConnection.jbc != null)
                        {
                            if (!DLLConnection.jbc.isSearching(SearchMode.ETH))
                            {
                                LoggerModule.logger.Info(My.Resources.Resources.evSearchingETHStations);
                                DLLConnection.jbc.StartSearch(SearchMode.ETH);
                            }
                        }
                        break;
                    case (int)enumCommand.startUSB:
                        if (DLLConnection.jbc != null)
                        {
                            if (!DLLConnection.jbc.isSearching(SearchMode.USB))
                            {
                                LoggerModule.logger.Info(My.Resources.Resources.evSearchingUSBStations);
                                DLLConnection.jbc.StartSearch(SearchMode.USB);
                            }
                        }
                        break;
                    case (int)enumCommand.stopETH:
                        if (DLLConnection.jbc != null)
                        {
                            if (DLLConnection.jbc.isSearching(SearchMode.ETH))
                            {
                                LoggerModule.logger.Info(My.Resources.Resources.evStopSearchingETHStations);
                                DLLConnection.jbc.StopSearch(SearchMode.ETH);
                            }
                        }
                        break;
                    case (int)enumCommand.stopUSB:
                        if (DLLConnection.jbc != null)
                        {
                            if (DLLConnection.jbc.isSearching(SearchMode.USB))
                            {
                                LoggerModule.logger.Info(My.Resources.Resources.evStopSearchingUSBStations);
                                DLLConnection.jbc.StopSearch(SearchMode.USB);
                            }
                        }
                        break;
                    case (int)enumCommand.initALL:
                        My.Settings.Default.SearchETH = true;
                        My.Settings.Default.SearchUSB = true;
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingETH, My.Resources.Resources.evOn));
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingUSB, My.Resources.Resources.evOn));
                        bSave = true;
                        break;
                    case (int)enumCommand.initUSB:
                        My.Settings.Default.SearchETH = false;
                        My.Settings.Default.SearchUSB = true;
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingETH, My.Resources.Resources.evOff));
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingUSB, My.Resources.Resources.evOn));
                        bSave = true;
                        break;
                    case (int)enumCommand.initETH:
                        My.Settings.Default.SearchETH = true;
                        My.Settings.Default.SearchUSB = false;
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingETH, My.Resources.Resources.evOn));
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingUSB, My.Resources.Resources.evOff));
                        bSave = true;
                        break;
                    case (int)enumCommand.initNONE:
                        My.Settings.Default.SearchETH = false;
                        My.Settings.Default.SearchUSB = false;
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingETH, My.Resources.Resources.evOff));
                        LoggerModule.logger.Info(string.Format(My.Resources.Resources.evSetSearchingUSB, My.Resources.Resources.evOff));
                        bSave = true;
                        break;
                }

                if (bSave)
                {
                    My.Settings.Default.Save();
                }

            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(string.Format("{0} ({1})", My.Resources.Resources.evErrorOnCustomCommand, ex.Message));
            }
        }

    }
}
