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
    public class dc_ContinuousModePort_HA
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_Port port = dc_EnumConstJBC.dc_Port.NO_PORT;
        [DataMember()]
        public dc_getTemperature temperature = new dc_getTemperature();
        [DataMember()]
        public int flow = 0;
        [DataMember()]
        public int power = 0;
        [DataMember()]
        public dc_getTemperature ext1Temp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature ext2Temp = new dc_getTemperature();
        [DataMember()]
        public int timeToStop = 0;
        [DataMember()]
        public dc_EnumConstJBC.dc_ToolStatus_HA status = dc_EnumConstJBC.dc_ToolStatus_HA.NONE;
    }
}
