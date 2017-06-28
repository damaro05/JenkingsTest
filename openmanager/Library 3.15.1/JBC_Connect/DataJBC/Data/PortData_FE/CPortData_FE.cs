// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports


namespace DataJBC
{
	
	
	/// <summary>
	/// Provides status information and configuration of the port
	/// </summary>
	/// <remarks>The port is a soldering feeder station</remarks>
	internal class CPortData_FE
	{
		
		//Port status
		public CToolStatusData_FE ToolStatus = new CToolStatusData_FE();
		
		//Counters
		public CCountersData_FE Counters = new CCountersData_FE();
		public CCountersData_FE PartialCounters = new CCountersData_FE();
		
	}
	
}
