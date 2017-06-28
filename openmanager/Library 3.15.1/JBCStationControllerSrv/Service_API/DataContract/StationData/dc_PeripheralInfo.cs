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
    public class dc_PeripheralInfo
    {
        [DataMember()]
        public int ID = 0;
        [DataMember()]
        public short Version = (short)0;
        [DataMember()]
        public string Hash_MCU_UID = "";
        [DataMember()]
        public string DateTime = "";
        [DataMember()]
        public dc_EnumConstJBC.dc_PeripheralType Type = dc_EnumConstJBC.dc_PeripheralType.NO_TYPE;
        [DataMember()]
        public dc_EnumConstJBC.dc_Port PortAttached = dc_EnumConstJBC.dc_Port.NO_PORT;
        [DataMember()]
        public dc_EnumConstJBC.dc_PeripheralFunction WorkFunction = dc_EnumConstJBC.dc_PeripheralFunction.NO_FUNCTION;
        [DataMember()]
        public dc_EnumConstJBC.dc_PeripheralActivation ActivationMode = dc_EnumConstJBC.dc_PeripheralActivation.NO_FUNCTION;
        [DataMember()]
        public short DelayTime = (short)0;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff StatusActive = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_PeripheralStatusPD StatusPD = dc_EnumConstJBC.dc_PeripheralStatusPD.OK;
    }
}
