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
	internal class CToolStatusData_SOLD : ICloneable
	{

		//Temperature, power and current
		public CTemperature[] ActualTemp;
		public CTemperature Temp_MOS = new CTemperature();
		public int[] Power_x_Mil;
		public int[] Current_mA;

		//Tool
		public GenericStationTools ConnectedTool = new GenericStationTools();

		//Status
		public OnOff Desold_OnOff;
		public OnOff Extractor_OnOff;
		public OnOff Hiber_OnOff;
		public OnOff Sleep_OnOff;
		public OnOff Stand_OnOff; // 12/03/2013 #edu#
		public stc_StatusRemoteMode StatusRemoteMode;

		//Status change
		public int TimeToSleepHibern;
		public ToolFutureMode FutureMode_Tool;

		public OnOff EnabledPort = OnOff._OFF;
		public CTemperature SelectedTemp = new CTemperature();

#if ConnectRemote
		//Peripherals
		public CPeripheralData[] Peripherals; // Periféricos asociados al puerto
#endif

		//Error
		public ToolError ToolError = new ToolError();


		public struct stc_StatusRemoteMode
		{
			public OnOff Sleep_OnOff;
			public OnOff Extractor_OnOff;
			public OnOff Desold_OnOff;
		}


		public CToolStatusData_SOLD()
		{
			ActualTemp = new CTemperature[2];
			ActualTemp[0] = new CTemperature();
			ActualTemp[1] = new CTemperature();
			Power_x_Mil = new int[2];
			Current_mA = new int[2];
		}

		public dynamic Clone()
		{
			CToolStatusData_SOLD cls_StatusTool_Clonado = new CToolStatusData_SOLD();
			cls_StatusTool_Clonado.ActualTemp = this.ActualTemp;
			cls_StatusTool_Clonado.Temp_MOS = (CTemperature)(this.Temp_MOS.Clone());
			cls_StatusTool_Clonado.Power_x_Mil = this.Power_x_Mil;
			cls_StatusTool_Clonado.Current_mA = this.Current_mA;
			cls_StatusTool_Clonado.ConnectedTool = this.ConnectedTool;
			cls_StatusTool_Clonado.Desold_OnOff = this.Desold_OnOff;
			cls_StatusTool_Clonado.Extractor_OnOff = this.Extractor_OnOff;
			cls_StatusTool_Clonado.Hiber_OnOff = this.Hiber_OnOff;
			cls_StatusTool_Clonado.Sleep_OnOff = this.Sleep_OnOff;
			cls_StatusTool_Clonado.Stand_OnOff = this.Stand_OnOff; // 12/03/2013 #edu#
			cls_StatusTool_Clonado.StatusRemoteMode = this.StatusRemoteMode;
			cls_StatusTool_Clonado.TimeToSleepHibern = this.TimeToSleepHibern;
			cls_StatusTool_Clonado.FutureMode_Tool = this.FutureMode_Tool;
			cls_StatusTool_Clonado.ToolError = this.ToolError;

			return cls_StatusTool_Clonado;
		}

	}
}
