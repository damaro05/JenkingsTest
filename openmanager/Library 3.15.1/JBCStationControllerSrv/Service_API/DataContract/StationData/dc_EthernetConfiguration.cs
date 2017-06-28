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
using System.Net;

using JBC_Connect;

namespace JBCStationControllerSrv
{
    [DataContract()]
    public class dc_EthernetConfiguration
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff DHCP = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public IPAddress IP;
        [DataMember()]
        public IPAddress Mask;
        [DataMember()]
        public IPAddress Gateway;
        [DataMember()]
        public IPAddress DNS1;
        [DataMember()]
        public IPAddress DNS2;
        [DataMember()]
        public ushort Port;
    }
}
