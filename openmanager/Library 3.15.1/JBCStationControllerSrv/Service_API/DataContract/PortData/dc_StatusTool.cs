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
		public class dc_StatusTool
		{
			[DataMember()]
				public dc_EnumConstJBC.dc_Port portNbr = dc_EnumConstJBC.dc_Port.NO_PORT;
			[DataMember()]
				public dc_EnumConstJBC.dc_GenericStationTools ConnectedTool = dc_EnumConstJBC.dc_GenericStationTools.NOTOOL;
			[DataMember()]
				public dc_getTemperature ActualTemp = new dc_getTemperature();
			//<DataMember()> _
			//Public Current_mA As Integer = 0
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff Desold_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff Extractor_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public string FutureMode = ""; // S (sleep) or H (hibernation)
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff Hiber_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public dc_PeripheralInfo[] Peripheral = new dc_PeripheralInfo[] { };
			[DataMember()]
				public int Power_x_Mil = 0;
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff Sleep_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff Stand_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff StatusRemoteMode_Sleep_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff StatusRemoteMode_Extractor_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public dc_EnumConstJBC.dc_OnOff StatusRemoteMode_Desold_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
			[DataMember()]
				public dc_getTemperature Temp_MOS = new dc_getTemperature();
			[DataMember()]
				public int TimeToSleepHibern = 0;
			[DataMember()]
				public dc_EnumConstJBC.dc_ToolError ToolError = dc_EnumConstJBC.dc_ToolError.OK;

			[DataMember()]
				public dc_getTemperature PortSelectedTemp = new dc_getTemperature(); // selected temp for selected port (all tools)
			[DataMember()]
				public JBC_Connect.dc_EnumConstJBC.dc_OnOff EnabledPort = JBC_Connect.dc_EnumConstJBC.dc_OnOff._ON;
			
			// datos que viajan para no consultar también dc_PortToolSettings
			// settings del puerto y la herramienta conectada (si hay alguna)
			[DataMember()]
				public dc_PortToolSettings This_PortToolSettings = new dc_PortToolSettings();
		}
}
