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
    public class dc_ContinuousModeData_HA
    {
        [DataMember()]
        public dc_ContinuousModePort_HA[] data = new dc_ContinuousModePort_HA[] { };
        [DataMember()]
        public ulong sequence;
    }
}
