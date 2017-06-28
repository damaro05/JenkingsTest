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


namespace JBC_Connect
{
	
	
	public class CStationStatusData_FE : ICloneable
	{
		
		public ControlModeConnection ControlMode = ControlModeConnection.MONITOR;
		public StationError ErrorStation = StationError.NO_ERROR;
		//Only for Remote Manager
		public string ControlModeUserName = "";
		
		public dynamic Clone()
		{
			CStationStatusData_FE cls_Clonado = new CStationStatusData_FE();
			cls_Clonado.ControlMode = this.ControlMode;
			cls_Clonado.ErrorStation = this.ErrorStation;
			//Only for Remote Manager
			cls_Clonado.ControlModeUserName = this.ControlModeUserName;
			
			return cls_Clonado;
		}
		
	}
	
}
