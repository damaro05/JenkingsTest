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

using System.Runtime.Serialization;

namespace JBCHostControllerSrv
{
    [DataContract()]
    public class dc_InfoUpdateSoftware
    {
        [DataMember()]
        public DateTime lastUpdateDate;
        [DataMember()]
        public bool stationControllerSwAvailable = false;
        [DataMember()]
        public DateTime stationControllerSwDate;
        [DataMember()]
        public string stationControllerSwVersion;
        [DataMember()]
        public bool remoteManagerSwAvailable = false;
        [DataMember()]
        public DateTime remoteManagerSwDate;
        [DataMember()]
        public string remoteManagerSwVersion;
        [DataMember()]
        public bool hostControllerSwAvailable = false;
        [DataMember()]
        public DateTime hostControllerSwDate;
        [DataMember()]
        public string hostControllerSwVersion;
        [DataMember()]
        public bool webManagerSwAvailable = false;
        [DataMember()]
        public DateTime webManagerSwDate;
        [DataMember()]
        public string webManagerSwVersion;
    }
}
