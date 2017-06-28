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
    public class dc_TraceDataSequence
    {
        [DataMember()]
        public byte[] bytes;
        [DataMember()]
        public bool final;
        [DataMember()]
        public int sequence;
    }
}
