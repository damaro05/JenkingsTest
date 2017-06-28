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
	public class CStationInfoData : ICloneable
	{

		public string UUID = "";
		public string ParentUUID = "";
		public string Protocol = "";

		//Connection
		public string COM = ""; //p.e. COM6 (usb), 192.168.1.3 (ethernet)
		public string ConnectionType = ""; // U=USB E=Ethernet

		public eStationType StationType = eStationType.UNKNOWN;
		public CFeaturesData Features = new CFeaturesData();
		public Hashtable StationMicros = new Hashtable(); //Información de los micros de la estación

		//Firmware
		public string Model = "";
		public string ModelType = "";
		public int ModelVersion = -1;
		public string Version_Hardware = "";
		public string Version_Software = "";

		//Port, tools and temp levels
		public int PortCount = 0;
		public GenericStationTools[] SupportedTools = new GenericStationTools[] { };
		public int TempLevelsCount = 3;


		public dynamic Clone()
		{
			CStationInfoData cls_Clonado = new CStationInfoData();
			cls_Clonado.UUID = this.UUID;
			cls_Clonado.ParentUUID = this.ParentUUID;
			cls_Clonado.Protocol = this.Protocol;

			//Connection
			cls_Clonado.COM = this.COM;
			cls_Clonado.ConnectionType = this.ConnectionType;

			cls_Clonado.StationType = this.StationType;
			cls_Clonado.Features = (CFeaturesData)(this.Features.Clone());

			foreach (DictionaryEntry stationMicroEntry in StationMicros)
			{
				CFirmwareStation stationMicro = new CFirmwareStation();
				stationMicro.StationUUID = ((CFirmwareStation) stationMicroEntry.Value).StationUUID;
				stationMicro.Model = ((CFirmwareStation)stationMicroEntry.Value).Model;
				stationMicro.ModelVersion = ((CFirmwareStation)stationMicroEntry.Value).ModelVersion;
				stationMicro.ProtocolVersion = ((CFirmwareStation)stationMicroEntry.Value).ProtocolVersion;
				stationMicro.HardwareVersion = ((CFirmwareStation)stationMicroEntry.Value).HardwareVersion;
				stationMicro.SoftwareVersion = ((CFirmwareStation)stationMicroEntry.Value).SoftwareVersion;
				stationMicro.FileName = ((CFirmwareStation)stationMicroEntry.Value).FileName;

				cls_Clonado.StationMicros.Add(stationMicroEntry.Key, stationMicro);
			}

			//Firmware
			cls_Clonado.Model = this.Model;
			cls_Clonado.ModelType = this.ModelType;
			cls_Clonado.ModelVersion = this.ModelVersion;
			cls_Clonado.Version_Hardware = this.Version_Hardware;
			cls_Clonado.Version_Software = this.Version_Software;

			//Port, tools and temp levels
			cls_Clonado.PortCount = this.PortCount;
			cls_Clonado.SupportedTools = new GenericStationTools[this.SupportedTools.Length - 1 + 1];
			for (var i = 0; i <= this.SupportedTools.Length - 1; i++)
			{
				cls_Clonado.SupportedTools[(int)i] = this.SupportedTools[(int)i];
			}
			cls_Clonado.TempLevelsCount = this.TempLevelsCount;

			return cls_Clonado;
		}

	}
}
