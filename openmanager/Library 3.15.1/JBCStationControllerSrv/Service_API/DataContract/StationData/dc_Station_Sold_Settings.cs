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
    public class dc_Station_Sold_Settings
    {
        [DataMember()]
        public string Name = "";
        [DataMember()]
        public string PIN = "";
        [DataMember()]
        public string Unit = ""; // C or F
        [DataMember()]
        public dc_getTemperature MaxTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature MinTemp = new dc_getTemperature();
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff N2Mode = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff HelpText = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public int PowerLimit = 0;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff Beep = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff StationLocked = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_Idioma Idioma = dc_EnumConstJBC.dc_Idioma.I_Ingles;
    }
}
