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

using JBC_Connect;

namespace JBCStationControllerSrv
{
    [DataContract()]
    public class dc_Station_HA_Status
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_ControlModeConnection ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.MONITOR;
        [DataMember()]
        public string ControlModeUserName = "";
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff RemoteMode = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_StationError StationError = dc_EnumConstJBC.dc_StationError.OK;
        [DataMember()]
        public dc_ContinuousModeStatus ContinuousModeStatus = new dc_ContinuousModeStatus();
    }
}
