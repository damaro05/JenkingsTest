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
using System.Configuration.Install;
using System.ServiceProcess;


namespace JBCStationControllerSrv
{
    // Provide the ProjectInstaller class which allows
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class CProjectInstaller : Installer
    {

        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public CProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;

            service = new ServiceInstaller();
            service.ServiceName = JBCStationControllerService.WINDOWS_SERVICE_NAME;
            service.Description = "JBC Service to control connected stations by exposing JBC Connect API thru WCF";
            service.StartType = ServiceStartMode.Automatic;

            Installers.Add(process);
            Installers.Add(service);
        }

    }
}
