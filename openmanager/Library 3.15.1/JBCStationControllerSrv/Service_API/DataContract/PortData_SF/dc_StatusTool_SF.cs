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
	
	
	[DataContract()]public class dc_StatusTool_SF
	{
		public dc_StatusTool_SF()
		{
			// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
			DispenserMode = dc_EnumConstJBC.dc_DispenserMode_SF.CONTINUOUS;
			
		}
		[DataMember()]public JBC_Connect.dc_EnumConstJBC.dc_Port portNbr = JBC_Connect.dc_EnumConstJBC.dc_Port.NO_PORT;
		[DataMember()]public dc_EnumConstJBC.dc_OnOff EnabledPort = JBC_Connect.dc_EnumConstJBC.dc_OnOff._ON;
		[DataMember()]public dc_EnumConstJBC.dc_DispenserMode_SF DispenserMode; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
		[DataMember()]public dc_Speed Speed = new dc_Speed();
		[DataMember()]public dc_Length Length = new dc_Length();
		[DataMember()]public JBC_Connect.dc_EnumConstJBC.dc_OnOff FeedingState = JBC_Connect.dc_EnumConstJBC.dc_OnOff._OFF;
		[DataMember()]public ushort FeedingPercent = System.Convert.ToUInt16(0);
		[DataMember()]public dc_Length FeedingLength = new dc_Length();
		[DataMember()]public byte CurrentProgramStep = (byte) 0;
	}
	
}
