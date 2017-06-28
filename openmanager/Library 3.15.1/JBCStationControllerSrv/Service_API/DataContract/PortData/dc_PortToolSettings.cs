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
    public class dc_PortToolSettings
    {
        [DataMember()]
        public dc_EnumConstJBC.dc_Port portNbr = dc_EnumConstJBC.dc_Port.NO_PORT;
        [DataMember()]
        public dc_EnumConstJBC.dc_GenericStationTools Tool = dc_EnumConstJBC.dc_GenericStationTools.NOTOOL;
        [DataMember()]
        public dc_getTemperature AdjustTemp = new dc_getTemperature();
        [DataMember()]
        public dc_Cartridge Cartridge = new dc_Cartridge();
        [DataMember()]
        public dc_getTemperature FixedTemp = new dc_getTemperature();
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff FixedTemp_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_Levels Levels = new dc_Levels();
        [DataMember()]
        public dc_getTemperature SleepTemp = new dc_getTemperature();
        [DataMember()]
        public dc_EnumConstJBC.dc_TimeSleep SleepTime = dc_EnumConstJBC.dc_TimeSleep.MINUTE_0;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff SleepTimeOnOff = dc_EnumConstJBC.dc_OnOff._ON;
        [DataMember()]
        public dc_EnumConstJBC.dc_TimeHibernation HiberTime = dc_EnumConstJBC.dc_TimeHibernation.MINUTE_30;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff HiberTimeOnOff = dc_EnumConstJBC.dc_OnOff._ON;
    }
}
