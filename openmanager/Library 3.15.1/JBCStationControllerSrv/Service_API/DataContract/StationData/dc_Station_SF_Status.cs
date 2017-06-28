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
	
	
	[DataContract()]public class dc_Station_SF_Status
	{
		[DataMember()]public JBC_Connect.dc_EnumConstJBC.dc_ControlModeConnection ControlMode = JBC_Connect.dc_EnumConstJBC.dc_ControlModeConnection.MONITOR;
		[DataMember()]public string ControlModeUserName = "";
		[DataMember()]public JBC_Connect.dc_EnumConstJBC.dc_StationError StationError = JBC_Connect.dc_EnumConstJBC.dc_StationError.OK;
	}
	
}
