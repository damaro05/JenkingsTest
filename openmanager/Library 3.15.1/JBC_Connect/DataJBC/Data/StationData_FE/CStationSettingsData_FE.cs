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
	
	
	public class CStationSettingsData_FE : ICloneable
	{
		
		//Parameters
		public string Name = "";
		public OnOff Beep = OnOff._OFF;
		public string PIN = "";
		public OnOff ContinuousSuction = OnOff._OFF;
		
		//Communications
		public CRobotData Robot = new CRobotData();
		
		
		public dynamic Clone()
		{
			CStationSettingsData_FE cls_Clonado = new CStationSettingsData_FE();
			cls_Clonado.Name = this.Name;
			cls_Clonado.Beep = this.Beep;
			cls_Clonado.PIN = this.PIN;
			cls_Clonado.ContinuousSuction = this.ContinuousSuction;
			cls_Clonado.Robot = (CRobotData) (this.Robot.Clone());
			
			return cls_Clonado;
		}
		
	}
	
}
