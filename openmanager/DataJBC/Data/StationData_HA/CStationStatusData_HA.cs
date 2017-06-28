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
	public class CStationStatusData_HA : ICloneable
	{

		public ControlModeConnection ControlMode = ControlModeConnection.MONITOR;
		public OnOff RemoteMode;
		public StationError ErrorStation = StationError.NO_ERROR;
		//Continuous mode
		public CContinuousModeStatus ContinuousModeStatus = new CContinuousModeStatus();
		//Only for Remote Manager
		public string ControlModeUserName = "";

		public dynamic Clone()
		{
			CStationStatusData_SOLD cls_Clonado = new CStationStatusData_SOLD();
			cls_Clonado.ControlMode = this.ControlMode;
			cls_Clonado.RemoteMode = this.RemoteMode;
			cls_Clonado.ErrorStation = this.ErrorStation;
			//Continuous mode
			cls_Clonado.ContinuousModeStatus = (CContinuousModeStatus) (this.ContinuousModeStatus.Clone());
			//Only for Remote Manager
			cls_Clonado.ControlModeUserName = this.ControlModeUserName;

			return cls_Clonado;
		}

	}
}
