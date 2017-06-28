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
using HostControllerServiceReference;

using JBC_Connect;

namespace JBCStationControllerSrv
{
    [DataContract()]
    public class dc_Station_Sold_Info
    {
        [DataMember()]
        public string UUID = "";
        [DataMember()]
        public string ParentUUID = "";
        [DataMember()]
        public dc_EnumConstJBC.dc_StationType StationType = dc_EnumConstJBC.dc_StationType.UNKNOWN; // new
        [DataMember()]
        public string Protocol = "";
        [DataMember()]
        public string COM = "";
        [DataMember()]
        public string ConnectionType = ""; // U=USB E=Ethernet
        [DataMember()]
        public dc_Features Features = new dc_Features();
        [DataMember()]
        public dc_FirmwareStation[] InfoUpdateFirmware = new dc_FirmwareStation[] { };
        [DataMember()]
        public string Model = "";
        [DataMember()]
        public string ModelType = "";
        [DataMember()]
        public int ModelVersion = -1;
        [DataMember()]
        public int PortCount = 0;
        [DataMember()]
        public dc_EnumConstJBC.dc_GenericStationTools[] SupportedTools = new dc_EnumConstJBC.dc_GenericStationTools[] { };
        [DataMember()]
        public int TempLevelsCount = 3;
        [DataMember()]
        public string Version_Software = "";
        [DataMember()]
        public string Version_Hardware = "";
    }
}
