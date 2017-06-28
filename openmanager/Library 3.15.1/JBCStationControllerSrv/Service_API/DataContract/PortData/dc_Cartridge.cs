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
    public class dc_Cartridge
    {
        [DataMember()]
        public ushort CartridgeNbr = System.Convert.ToUInt16(0);
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff CartridgeOnOff = dc_EnumConstJBC.dc_OnOff._OFF;
    }
}
