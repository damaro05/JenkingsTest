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
    //FIXME: Remove this class
    [DataContract()]
    public class dc_ConfigurationPortStation
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_GenericStationTools Tool = dc_EnumConstJBC.dc_GenericStationTools.NOTOOL;
        [DataMember()]
        public int PortSelectedTemp = 0;
        [DataMember()]
        public int AdjustTemp = 0;
    }
}
