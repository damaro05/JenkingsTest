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
    public class dc_ContinuousModeStatus
    {
        [DataMember()]
        public bool port1 = false;
        [DataMember()]
        public bool port2 = false;
        [DataMember()]
        public bool port3 = false;
        [DataMember()]
        public bool port4 = false;
        [DataMember()]
        public dc_EnumConstJBC.dc_SpeedContinuousMode Speed = dc_EnumConstJBC.dc_SpeedContinuousMode.OFF;
    }
}
