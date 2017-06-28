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
	internal class CToolStatusData_HA : ICloneable
	{

		//Temperature, power and flow
		public CTemperature ActualTemp = new CTemperature(0); // Air temperature
		public CTemperature ActualExtTemp = new CTemperature(0);
		public CTemperature ProtectionTC_Temp = new CTemperature(0);
		public int Power_x_Mil;
		public int Flow_x_Mil;

		//Tool
		public GenericStationTools ConnectedTool = new GenericStationTools();

		//Status
		public OnOff Stand_OnOff;
		public OnOff PedalStatus_OnOff;
		public OnOff PedalConnected_OnOff;
		public OnOff SuctionRequestedStatus_OnOff;
		public OnOff SuctionStatus_OnOff;
		public OnOff HeaterRequestedStatus_OnOff;
		// heater y cooling son excluyentes
		public OnOff HeaterStatus_OnOff;
		public OnOff CoolingStatus_OnOff;

		//Status change
		public int TimeToStop;

		public OnOff EnabledPort = OnOff._OFF;

		//Selected parameters
		public CTemperature SelectedTemp = new CTemperature();
		public CTemperature SelectedExtTemp = new CTemperature();
		public int SelectedFlow_x_Mil = 0;
		public OnOff ProfileMode = OnOff._OFF;

		//Error
		public ToolError ToolError = new ToolError();


		public dynamic Clone()
		{
			CToolStatusData_HA cls_StatusTool_Clonado = new CToolStatusData_HA();
			cls_StatusTool_Clonado.ActualTemp.UTI = this.ActualTemp.UTI;
			cls_StatusTool_Clonado.ActualExtTemp.UTI = this.ActualExtTemp.UTI;
			cls_StatusTool_Clonado.ProtectionTC_Temp.UTI = this.ProtectionTC_Temp.UTI;
			cls_StatusTool_Clonado.Power_x_Mil = this.Power_x_Mil;
			cls_StatusTool_Clonado.Flow_x_Mil = this.Flow_x_Mil;
			cls_StatusTool_Clonado.ConnectedTool = this.ConnectedTool;
			cls_StatusTool_Clonado.Stand_OnOff = this.Stand_OnOff;
			cls_StatusTool_Clonado.PedalStatus_OnOff = this.PedalStatus_OnOff;
			cls_StatusTool_Clonado.PedalConnected_OnOff = this.PedalConnected_OnOff;
			cls_StatusTool_Clonado.SuctionRequestedStatus_OnOff = this.SuctionRequestedStatus_OnOff;
			cls_StatusTool_Clonado.SuctionStatus_OnOff = this.SuctionStatus_OnOff;
			cls_StatusTool_Clonado.HeaterRequestedStatus_OnOff = this.HeaterRequestedStatus_OnOff;
			cls_StatusTool_Clonado.HeaterStatus_OnOff = this.HeaterStatus_OnOff;
			cls_StatusTool_Clonado.CoolingStatus_OnOff = this.CoolingStatus_OnOff;
			cls_StatusTool_Clonado.TimeToStop = this.TimeToStop;
			cls_StatusTool_Clonado.ToolError = this.ToolError;

			return cls_StatusTool_Clonado;
		}

	}
}
