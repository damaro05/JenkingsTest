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
    public class dc_PortToolSettings_HA
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_Port portNbr = dc_EnumConstJBC.dc_Port.NO_PORT;
        [DataMember()]
        public dc_EnumConstJBC.dc_GenericStationTools Tool = dc_EnumConstJBC.dc_GenericStationTools.NOTOOL;
        [DataMember()]
        public dc_Levels_HA Levels = new dc_Levels_HA();
        [DataMember()]
        public dc_getTemperature AdjustTemp = new dc_getTemperature();
        [DataMember()]
        public int TimeToStop;
        [DataMember()]
        public dc_EnumConstJBC.dc_ExternalTCMode_HA ExternalTCMode = new dc_EnumConstJBC.dc_ExternalTCMode_HA();
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff StartMode_ToolButton = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_PedalAction StartMode_Pedal = dc_EnumConstJBC.dc_PedalAction.NONE;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff PortProfileMode = dc_EnumConstJBC.dc_OnOff._OFF; // false: manual mode - true: profile mode
    }
}
