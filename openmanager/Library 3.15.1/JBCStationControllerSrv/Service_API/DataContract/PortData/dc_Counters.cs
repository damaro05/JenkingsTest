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
    public class dc_Counters
    {
        [DataMember()]
        public int ContPlugMinutes = 0;
        [DataMember()]
        public int ContWorkMinutes = 0;
        [DataMember()]
        public int ContSleepMinutes = 0;
        [DataMember()]
        public int ContHiberMinutes = 0;
        [DataMember()]
        public int ContIdleMinutes = 0;
        [DataMember()]
        public int ContSleepCycles = 0;
        [DataMember()]
        public int ContDesoldCycles = 0;
    }
}
