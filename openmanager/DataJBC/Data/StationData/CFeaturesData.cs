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

	public class CFeaturesData : ICloneable
	{

		public bool Alarms = false;
		public bool AllToolsSamePortSettings = false;
		public bool Cartridges = false;
		public bool DelayWithStatus = false;
		public bool DisplaySettings = false; // TempUnits, N2Mode, HelpText, Beep
		public bool Ethernet = false;
		public bool FirmwareUpdate = false;
		public CTemperature MaxTemp = new CTemperature(0);
		public CTemperature MinTemp = new CTemperature(0);
		public CTemperature ExtTCMaxTemp = new CTemperature(0);
		public CTemperature ExtTCMinTemp = new CTemperature(0);
		public int MaxFlow = 0;
		public int MinFlow = 0;
		public int MaxPowerLimit = 0;
		public bool PartialCounters = false;
		public bool Peripherals = false;
		public bool Robot = false;
		public bool SubStations = false;
		public bool TempLevelsWithStatus = false;
		public bool TempLevels = true;
		// privates to JBC_Connect project
		internal bool BurstMessages = false;
		internal bool ChangesStatusInformation = false;
		internal bool ContinuosuModeDataExtraByte = false; // erroneous extra byte for DDE y NAE
		internal bool ContinuousModeDevice11h = false;
		internal bool CounterIdleTimeIsCalculated = true; // do not reset with M_W_NOTOOLTIMEP command (WriteIdleTime())
		internal bool ParamsLockedFrame = false;
		internal bool SelectedTempTweezersValueCero = false; //erroneous tempB = 0 for DME touch


		public CFeaturesData()
		{
			InitDefault();
		}

		public CFeaturesData(CFeaturesData data)
		{
			InitDefault();
			this.Alarms = data.Alarms;
			this.AllToolsSamePortSettings = data.AllToolsSamePortSettings;
			this.Cartridges = data.Cartridges;
			this.DelayWithStatus = data.DelayWithStatus;
			this.DisplaySettings = data.DisplaySettings;
			this.Ethernet = data.Ethernet;
			this.FirmwareUpdate = data.FirmwareUpdate;
			this.MaxTemp.UTI = data.MaxTemp.UTI;
			this.MinTemp.UTI = data.MinTemp.UTI;
			this.ExtTCMaxTemp.UTI = data.ExtTCMaxTemp.UTI;
			this.ExtTCMinTemp.UTI = data.ExtTCMinTemp.UTI;
			this.MaxFlow = data.MaxFlow;
			this.MinFlow = data.MinFlow;
			this.MaxPowerLimit = data.MaxPowerLimit;
			this.PartialCounters = data.PartialCounters;
			this.Peripherals = data.Peripherals;
			this.Robot = data.Robot;
			this.SubStations = data.SubStations;
			this.TempLevelsWithStatus = data.TempLevelsWithStatus;
			this.TempLevels = data.TempLevels;
		}

		public CFeaturesData(string sModel, string sModelType, int sModelVersion, string sCommandProtocol)
		{
			InitDefault();
			SetModel(sModel, sModelType, sModelVersion, sCommandProtocol);
		}

		private void InitDefault()
		{
			Alarms = false;
			AllToolsSamePortSettings = false;
			Cartridges = false;
			DelayWithStatus = false;
			DisplaySettings = true;
			Ethernet = false;
			FirmwareUpdate = false;
			MaxTemp = new CTemperature((int)TemperatureLimits.MAX_TEMP);
			MinTemp = new CTemperature((int)TemperatureLimits.MIN_TEMP);
			ExtTCMaxTemp = new CTemperature(0);
			ExtTCMinTemp = new CTemperature(0);
			MaxFlow = 0;
			MinFlow = 0;
			MaxPowerLimit = 0;
			PartialCounters = false;
			Peripherals = false;
			Robot = false;
			SubStations = false;
			TempLevelsWithStatus = false;
			TempLevels = true; // implementado niveles de temperatura (y caudal en HA) no implementado en HA modelversion CAP 01
			// privates to JBC_Connect project
			BurstMessages = false;
			ContinuosuModeDataExtraByte = false;
			ContinuousModeDevice11h = false;
			CounterIdleTimeIsCalculated = true;
			ParamsLockedFrame = false; // implementada trama de ParameterLocked
			SelectedTempTweezersValueCero = false;
		}

		public void SetModel(string sModel, string sModelType, int iModelVersion, string sCommandProtocol)
		{

			switch (sModel)
			{
				case "HD":
				case "HDR":
					MaxTemp.UTI = (int)TemperatureLimits.MAX_TEMP_HD;
					break;
				case "JTSE":
					MaxTemp.UTI = (int)TemperatureLimits.MAX_TEMP_JTSE;
					MinTemp.UTI = (int)TemperatureLimits.MIN_TEMP_JTSE;
					ExtTCMaxTemp.UTI = (int)TemperatureLimits.MAX_EXT_TC_TEMP_JTSE;
					ExtTCMinTemp.UTI = (int)TemperatureLimits.MIN_EXT_TC_TEMP_JTSE;
					MaxFlow = (int)FlowLimits.MAX_FLOW;
					MinFlow = (int)FlowLimits.MIN_FLOW;
					break;
				case "DD":
				case "DDR":
					MaxPowerLimit = (int)PowerLimits.DD_MAX;
					break;
				case "DM":
					MaxPowerLimit = (int)PowerLimits.DM_MAX;
					break;
				case "DI":
					MaxPowerLimit = (int)PowerLimits.DI_MAX;
					break;
				case "CD_CF":
				case "CD/CF":
					MaxPowerLimit = (int)PowerLimits.CD_CF_MAX;
					break;
			}


			switch (sCommandProtocol)
			{
				case "02":
					Alarms = true;
					Cartridges = false;
					DelayWithStatus = true;
					DisplaySettings = false;
					PartialCounters = true;
					Robot = true;
					TempLevelsWithStatus = true;
					TempLevels = true;

					switch (sModel)
					{
						case "DME":
							CounterIdleTimeIsCalculated = false; // reset this counter also
							switch (sModelType)
							{
								case "TCH":
									switch (iModelVersion)
									{
										case 1:
											// primera versión
											AllToolsSamePortSettings = true;
											BurstMessages = true;
											ContinuousModeDevice11h = true; // hex 11
											CounterIdleTimeIsCalculated = false; // reset this counter also
											DelayWithStatus = false;
											SelectedTempTweezersValueCero = true;
											break;
										case 2:
											Cartridges = true;
											Ethernet = true;
											Peripherals = true;
											SubStations = true;
											break;
									}
									break;
							}
							break;
						case "PSE":
							Cartridges = true;
							CounterIdleTimeIsCalculated = false; // reset this counter also
							Ethernet = true;
							FirmwareUpdate = true;
							Peripherals = true;
							SubStations = true;
							DisplaySettings = false; // 07/09/2016 no tienen display
							break;
						case "SM":
							Robot = false;
							break;
						case "DDE":
							switch (sModelType)
							{
								case "CAP":
									switch (iModelVersion)
									{
										case 1:
											ContinuosuModeDataExtraByte = true;
											break;
									}
									break;
							}
							break;
						case "NAE":
							switch (sModelType)
							{
								case "CAP":
									switch (iModelVersion)
									{
										case 1:
										case 2:
											ContinuosuModeDataExtraByte = true;
											break;
									}
									break;
							}
							break;
						case "JTSE":
							Alarms = false;
							DelayWithStatus = false;
							FirmwareUpdate = true;
							switch (sModelType)
							{
								case "CAP":
									switch (iModelVersion)
									{
										case 1:
											// primera versión
											TempLevels = false; // no implementado niveles
											DisplaySettings = true;
											break;
									}
									break;
							}
							break;
					}
					break;

				case "01":
					ChangesStatusInformation = true;
					FirmwareUpdate = true;

					switch (sModel)
					{
						case "CD/CF":
							switch (iModelVersion)
							{
								case 2:
									ParamsLockedFrame = true;
									break;
							}
							break;
						case "DDR":
							Robot = true;
							break;
						case "HDR":
							Robot = true;
							break;
					}
					break;
				default:
					break;
			}
		}

		public dynamic Clone()
		{
			CFeaturesData cls_Clonado = new CFeaturesData(this);
			return cls_Clonado;
		}

	}
}

