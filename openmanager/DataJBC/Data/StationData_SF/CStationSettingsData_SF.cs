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
	public class CStationSettingsData_SF : ICloneable
	{

		//Parameters
		public string Name = "";
		public OnOff Beep = OnOff._OFF;
		public OnOff PINEnabled = OnOff._ON;
		public string PIN = "";
		public CLength.LengthUnit LengthUnit;
		public OnOff StationLocked = OnOff._OFF;

		//Programs
		public byte SelectedProgram;
		public CProgramDispenserData_SF[] Programs = new CProgramDispenserData_SF[35];
		public byte[] ConcatenateProgramList = new byte[35];		

		//Communications
		public CRobotData Robot = new CRobotData();


		public dynamic Clone()
		{
			CStationSettingsData_SF cls_Clonado = new CStationSettingsData_SF();
			cls_Clonado.Name = this.Name;
			cls_Clonado.Beep = this.Beep;
			cls_Clonado.PINEnabled = this.PINEnabled;
			cls_Clonado.PIN = this.PIN;
			cls_Clonado.LengthUnit = this.LengthUnit;
			cls_Clonado.StationLocked = this.StationLocked;
			cls_Clonado.Robot = (CRobotData) (this.Robot.Clone());

			return cls_Clonado;
		}

	}
}
