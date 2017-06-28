// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Runtime.Serialization;

namespace JBCHostControllerSrv
{
    [DataContract()]
    public class dc_InfoUpdatePeriodicTime
    {
        [DataMember()]
        public bool available = false;
        [DataMember()]
        public bool modeDaily = false;
        [DataMember()]
        public byte weekday = (byte)1;
        [DataMember()]
        public byte hour = (byte)0;
        [DataMember()]
        public byte minute = (byte)0;
    }
}
