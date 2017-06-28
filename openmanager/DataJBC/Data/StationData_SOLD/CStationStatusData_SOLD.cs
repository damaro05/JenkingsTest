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
	public class CStationStatusData_SOLD : ICloneable
	{

		public ControlModeConnection ControlMode = ControlModeConnection.MONITOR;
		public OnOff RemoteMode;
		public StationError ErrorStation = StationError.NO_ERROR;
		public CTemperature TempErrorMOS = new CTemperature();
		public CTemperature TempErrorTRAFO = new CTemperature();
		public CTemperature TempTRAFO = new CTemperature();
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
			cls_Clonado.TempErrorMOS.UTI = this.TempErrorMOS.UTI;
			cls_Clonado.TempErrorTRAFO.UTI = this.TempErrorTRAFO.UTI;
			cls_Clonado.TempTRAFO.UTI = this.TempTRAFO.UTI;
			//Continuous mode
			cls_Clonado.ContinuousModeStatus = (CContinuousModeStatus) (this.ContinuousModeStatus.Clone());
			//Only for Remote Manager
			cls_Clonado.ControlModeUserName = this.ControlModeUserName;

			return cls_Clonado;
		}

	}
}
