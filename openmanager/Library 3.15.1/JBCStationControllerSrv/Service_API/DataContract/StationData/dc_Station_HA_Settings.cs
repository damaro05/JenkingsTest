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
    public class dc_Station_HA_Settings
    {
        [DataMember()]
        public string Name = "";
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff Beep = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_Idioma Idioma = dc_EnumConstJBC.dc_Idioma.I_Ingles;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff PINEnabled = dc_EnumConstJBC.dc_OnOff._ON;
        [DataMember()]
        public string PIN = "";
        [DataMember()]
        public string Unit = ""; // C or F
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff StationLocked = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_getTemperature MaxTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature MinTemp = new dc_getTemperature();
        [DataMember()]
        public int MaxFlow = 0;
        [DataMember()]
        public int MinFlow = 0;
        [DataMember()]
        public dc_getTemperature MaxExtTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature MinExtTemp = new dc_getTemperature();
        //<DataMember()> _
        //Public StationDatetime As DateTime

    }
}
