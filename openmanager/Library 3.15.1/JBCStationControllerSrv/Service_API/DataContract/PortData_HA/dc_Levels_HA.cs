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
    public class dc_Levels_HA
    {
        [DataMember()]
        public int LevelsCount = 3;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff LevelsOnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public byte LevelsTempSelect = (byte)0;
        [DataMember()]
        public dc_getTemperature[] LevelsTemp = new dc_getTemperature[] { };
        [DataMember()]
        public int[] LevelsFlow;
        [DataMember()]
        public dc_getTemperature[] LevelsExtTemp = new dc_getTemperature[] { };
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff[] LevelsTempOnOff = new dc_EnumConstJBC.dc_OnOff[] { };

    }
}
