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
    public class dc_Features
    {
        [DataMember()]
        public bool Alarms = false;
        [DataMember()]
        public bool AllToolsSamePortSettings = false;
        [DataMember()]
        public bool Cartridges = false;
        [DataMember()]
        public bool DelayWithStatus = false;
        [DataMember()]
        public bool DisplaySettings = false;
        [DataMember()]
        public bool Ethernet = false;
        [DataMember()]
        public bool FirmwareUpdate = false;
        [DataMember()]
        public dc_getTemperature MaxTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature MinTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature ExtTCMaxTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature ExtTCMinTemp = new dc_getTemperature();
        [DataMember()]
        public int MaxFlow = 0;
        [DataMember()]
        public int MinFlow = 0;
        [DataMember()]
        public bool PartialCounters = false;
        [DataMember()]
        public bool Peripherals = false;
        [DataMember()]
        public bool Robot = false;
        [DataMember()]
        public bool TempLevelsWithStatus = false;
        [DataMember()]
        public bool TempLevels = true; // en la primera versión de HA vendrá a false
    }
}
