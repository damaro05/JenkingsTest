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
    public class dc_UpdateRemoteManager
    {
        [DataMember()]
        public byte[] bytes;
        [DataMember()]
        public bool final;
        [DataMember()]
        public int sequence;
    }
}