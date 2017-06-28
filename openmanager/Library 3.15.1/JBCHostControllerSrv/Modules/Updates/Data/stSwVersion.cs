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

namespace JBCHostControllerSrv
{
    public struct stSwVersion
    {
        public DateTime lastUpdateDate;
        //StationController
        public bool stationControllerSwAvailable;
        public string stationControllerSwVersion;
        public DateTime stationControllerSwDate;
        public string stationControllerSwUrl;
        //RemoteManager
        public bool remoteManagerSwAvailable;
        public string remoteManagerSwVersion;
        public DateTime remoteManagerSwDate;
        public string remoteManagerSwUrl;
        //HostController
        public bool hostControllerSwAvailable;
        public string hostControllerSwVersion;
        public DateTime hostControllerSwDate;
        public string hostControllerSwUrl;
        //WebManager
        public bool webManagerSwAvailable;
        public string webManagerSwVersion;
        public DateTime webManagerSwDate;
        public string webManagerSwUrl;
    }
}
