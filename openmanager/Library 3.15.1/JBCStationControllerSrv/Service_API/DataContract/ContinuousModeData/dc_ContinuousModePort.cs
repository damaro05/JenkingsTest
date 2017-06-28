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

using DataJBC;
using JBC_Connect;
//TODO:Toni

namespace JBCStationControllerSrv
{
    [DataContract()]
    public class dc_ContinuousModePort
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_Port port = dc_EnumConstJBC.dc_Port.NO_PORT;
        [DataMember()]
        public dc_getTemperature temperature = new dc_getTemperature();
        [DataMember()]
        public int power = 0;
        [DataMember()]
        public dc_EnumConstJBC.dc_ToolStatus status = dc_EnumConstJBC.dc_ToolStatus.NONE;
        [DataMember()]
        public OnOff desoldering = OnOff._OFF;
    }
}
