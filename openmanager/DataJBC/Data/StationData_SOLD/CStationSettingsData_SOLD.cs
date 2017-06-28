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
	public class CStationSettingsData_SOLD : ICloneable
	{

		//Parameters
		public string Name = "";
		public OnOff Beep = OnOff._OFF;
		public OnOff HelpText = OnOff._OFF;
		public Idioma Language = Idioma.I_Ingles;
		public OnOff N2Mode = OnOff._OFF;
		public string PIN = "";
		public CTemperature.TemperatureUnit Unit = CTemperature.TemperatureUnit.Celsius;
		public OnOff StationLocked = OnOff._OFF;
		public OnOff ParametersLocked = OnOff._OFF;

		//Limits
		public CTemperature MaxTemp = new CTemperature();
		public CTemperature MinTemp = new CTemperature();
		public int PowerLimit = 0;

		//Communications
		public CEthernetData Ethernet = new CEthernetData();
		public CRobotData Robot = new CRobotData();


		public dynamic Clone()
		{
			CStationSettingsData_SOLD cls_Clonado = new CStationSettingsData_SOLD();
			cls_Clonado.Name = this.Name;
			cls_Clonado.Beep = this.Beep;
			cls_Clonado.HelpText = this.HelpText;
			cls_Clonado.Language = this.Language;
			cls_Clonado.N2Mode = this.N2Mode;
			cls_Clonado.PIN = this.PIN;
			cls_Clonado.Unit = this.Unit;
			cls_Clonado.StationLocked = this.StationLocked;
			cls_Clonado.MaxTemp.UTI = this.MaxTemp.UTI;
			cls_Clonado.MinTemp.UTI = this.MinTemp.UTI;
			cls_Clonado.PowerLimit = this.PowerLimit;
			cls_Clonado.Ethernet = (CEthernetData)(this.Ethernet.Clone());
			cls_Clonado.Robot = (CRobotData)(this.Robot.Clone());

			return cls_Clonado;
		}

	}
}
