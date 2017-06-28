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
	
	
	internal class CToolStatusData_SF
	{
		
		public OnOff EnabledPort = OnOff._OFF;
		public DispenserMode_SF DispenserMode;
		public CSpeed Speed = new CSpeed();
		public CLength Length = new CLength();
		public OnOff FeedingState;
		public ushort FeedingPercent;
		public CLength FeedingLength = new CLength();
		public byte CurrentProgramStep;
		
	}
	
}
