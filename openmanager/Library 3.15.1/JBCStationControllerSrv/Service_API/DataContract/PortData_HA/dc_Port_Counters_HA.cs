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
	
	
	[DataContract()]public class dc_Port_Counters_HA
	{
		[DataMember()]public JBC_Connect.dc_EnumConstJBC.dc_Port portNbr = JBC_Connect.dc_EnumConstJBC.dc_Port.NO_PORT;
		[DataMember()]public dc_Counters_HA GlobalCounters = new dc_Counters_HA();
		[DataMember()]public dc_Counters_HA PartialCounters = new dc_Counters_HA();
	}
	
}
