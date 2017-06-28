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
    public class dc_RobotConfiguration
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff Status = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_RobotProtocol Protocol = dc_EnumConstJBC.dc_RobotProtocol.RS232;
        [DataMember()]
        public ushort Address = System.Convert.ToUInt16(0);
        [DataMember()]
        public dc_EnumConstJBC.dc_RobotSpeed Speed = dc_EnumConstJBC.dc_RobotSpeed.BPS_1200;
        [DataMember()]
        public ushort DataBits = System.Convert.ToUInt16(8);
        [DataMember()]
        public dc_EnumConstJBC.dc_RobotStop StopBits = dc_EnumConstJBC.dc_RobotStop.BITS_1;
        [DataMember()]
        public dc_EnumConstJBC.dc_RobotParity Parity = dc_EnumConstJBC.dc_RobotParity.NONE;
    }
}
