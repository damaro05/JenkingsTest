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
	
	
	[DataContract()]public class dc_Station_SF_Settings
	{
		public dc_Station_SF_Settings()
		{
			// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
			LengthUnit = dc_EnumConstJBC.dc_LengthUnit.MILLIMETERS;
			
		}
		[DataMember()]public string Name = "";
		[DataMember()]public JBC_Connect.dc_EnumConstJBC.dc_OnOff Beep = JBC_Connect.dc_EnumConstJBC.dc_OnOff._OFF;
		[DataMember()]public JBC_Connect.dc_EnumConstJBC.dc_OnOff PINEnabled = JBC_Connect.dc_EnumConstJBC.dc_OnOff._ON;
		[DataMember()]public string PIN = "";
		[DataMember()]public dc_EnumConstJBC.dc_LengthUnit LengthUnit; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
		[DataMember()]public dc_EnumConstJBC.dc_OnOff StationLocked = JBC_Connect.dc_EnumConstJBC.dc_OnOff._OFF;
		//Programs
		[DataMember()]public byte SelectedProgram = (byte) 0;
		[DataMember()]public dc_ProgramDispenserData_SF[] Programs;
		[DataMember()]public byte[] ConcatenateProgramList;
	}
	
}
