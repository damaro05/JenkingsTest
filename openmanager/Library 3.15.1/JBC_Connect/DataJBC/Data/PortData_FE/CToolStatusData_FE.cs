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
	
	
	internal class CToolStatusData_FE
	{
		
		public SuctionLevel_FE SuctionLevel;
		public ushort Flow;
		public ushort Speed;
		public OnOff IntakeActivationWork;
		public OnOff IntakeActivationStand;
		
		//Selected parameters
		public int SelectedFlow_x_Mil = 0;
		
		public int StandIntakes = 0;
		public int SuctionDelayWork;
		public int SuctionDelayStand;
		public int TimeToStopSuctionWork;
		public int TimeToStopSuctionStand;
		
		public PedalAction PedalActivation;
		public PedalMode PedalMode;
		public int FilterStatus;
	}
	
}
