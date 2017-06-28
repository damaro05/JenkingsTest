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
    public class dc_StatusTool_HA
    {
        // port info
        [DataMember()]
        public dc_EnumConstJBC.dc_Port portNbr = dc_EnumConstJBC.dc_Port.NO_PORT;
        [DataMember()]
        public dc_EnumConstJBC.dc_GenericStationTools ConnectedTool = dc_EnumConstJBC.dc_GenericStationTools.NOTOOL;
        [DataMember()]
        public dc_getTemperature ActualTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature ActualExtTemp = new dc_getTemperature();
        [DataMember()]
        public dc_getTemperature ProtectionTC_Temp = new dc_getTemperature();
        [DataMember()]
        public int Power_x_Mil = 0;
        [DataMember()]
        public int Flow_x_Mil = 0;
        [DataMember()]
        public int TimeToStop = 0;
        // port status
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff Stand_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff PedalStatus_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff PedalConnected_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff SuctionRequestedStatus_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff SuctionStatus_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff HeaterRequestedStatus_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff HeaterStatus_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff CoolingStatus_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
        [DataMember()]
        public dc_EnumConstJBC.dc_ToolError ToolError = dc_EnumConstJBC.dc_ToolError.OK;

        [DataMember()]
        public dc_getTemperature PortSelectedTemp = new dc_getTemperature(); // selected temp for selected port (all tools)
        [DataMember()]
        public dc_getTemperature PortSelectedExtTemp = new dc_getTemperature(); // selected Ext temp for selected port (all tools)
        [DataMember()]
        public int PortSelectedFlow_x_mil = 0; // selected flow for selected port (all tools)
        [DataMember()]
        public dc_EnumConstJBC.dc_OnOff EnabledPort = dc_EnumConstJBC.dc_OnOff._ON;

        // datos que viajan para no consultar también dc_PortToolSettings_HA
        // settings del puerto y la herramienta conectada (si hay alguna)
        [DataMember()]
        public dc_PortToolSettings_HA This_PortToolSettings = new dc_PortToolSettings_HA();
    }
}
