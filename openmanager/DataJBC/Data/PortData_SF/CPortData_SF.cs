// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
	/// <summary>
	/// Provides status information and configuration of the port
	/// </summary>
	/// <remarks>The port is a soldering feeder station</remarks>
	internal class CPortData_SF
	{

		//Port status
		public CToolStatusData_SF ToolStatus = new CToolStatusData_SF();
		//Counters
		public CCountersData_SF Counters = new CCountersData_SF();
		public CCountersData_SF PartialCounters = new CCountersData_SF();

	}
}
