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
		public class dc_ProgramDispenserData_SF
		{
		[DataMember()]
			public JBC_Connect.dc_EnumConstJBC.dc_OnOff Enabled = JBC_Connect.dc_EnumConstJBC.dc_OnOff._OFF;
		[DataMember()]
			public string Name;
		[DataMember()]
			public ushort Length_1;
		[DataMember()]
			public ushort Speed_1;
		[DataMember()]
			public ushort Length_2;
		[DataMember()]
			public ushort Speed_2;
		[DataMember()]
			public ushort Length_3;
		[DataMember()]
			public ushort Speed_3;
	}
	
}
