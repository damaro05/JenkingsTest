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
    public class dc_EventLog
    {
        [DataMember()]
        public DateTime eventDate;
        [DataMember()]
        public string eventMessage;
        [DataMember()]
        public string eventLevel;
        [DataMember()]
        public string eventApplication;
    }
}
