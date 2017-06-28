// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Runtime.Serialization;

namespace JBCStationControllerSrv
{
    [DataContract()]
    public class dc_FirmwareStation
    {
        [DataMember()]
        public string stationUUID;
        [DataMember()]
        public string model;
        [DataMember()]
        public string modelVersion;
        [DataMember()]
        public string protocolVersion;
        [DataMember()]
        public string hardwareVersion;
        [DataMember()]
        public string softwareVersion;
        [DataMember()]
        public string fileName;
        [DataMember()]
        public string language;
    }
}
