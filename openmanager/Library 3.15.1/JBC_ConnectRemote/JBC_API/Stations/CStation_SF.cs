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

using System.Threading.Tasks;
using JBC_ConnectRemote.JBCService;


namespace JBC_ConnectRemote
{
	
	
	public class CStation_SF : CStation
	{
		
		internal new Remote_StackSF stack = null;
		
		
#region Station Public methods
		
		public CStation_SF(string UUID, JBC_API_Remote APIreference, ref JBCStationControllerServiceClient hostService)
		{
			myAPI = APIreference;
			myUUID = UUID;
			remoteAddress = hostService.Endpoint.Address;
			
			stack = new Remote_StackSF(ref hostService, UUID);
			stack.ConnectError += stack_ConnectError;
			stack.StartStack();
		}
		
		public void Eraser()
		{
			
			//launching the event for the API class
			//myAPI.launchStationDisconnected(ID)
			
			//Station comunication lost. Releasing the stack memory
			stack.Eraser();
			
			//Self-destruction of the class instance
			// FALTA: no destruir objeto, cuando la API esté en formato NET
			// y si destruyo el objeto puedo provocar excepciones
			// Marcar como desconectado en la función launchStationDisconnected
			this.Finalize();
		}
		
		public int NumPorts
		{
			get
			{
				if (!stack.Equals(null))
				{
					return stack.Info_Port.Length;
				}
				else
				{
					return -1;
				}
			}
		}
		
		public JBC_ConnectRemote.CFeaturesData GetFeatures
		{
			get
			{
				if (!stack.Equals(null))
				{
					return stack.Info_Station.Info.Features;
				}
				else
				{
					return null;
				}
			}
		}
		
#endregion
		
#region Station Public Orders
		
		public async Task SetDefaultStationParamsAsync()
		{
			await stack.SetDefaultStationParamsAsync();
		}
		
		public void ResetStation()
		{
			stack.DeviceResetAsync();
		}
		
#region Station Info
		
		// Leer del host datos de la estación
		public async Task<bool> UpdateStationInfoAsync()
		{
			return await stack.UpdateStationInfoAsync();
		}
		
		// leer del host información de la estación y devuelve una estructura
		public async Task<JBC_ConnectRemote.CStationInfoData> GetStationInfoAsync()
		{
			await UpdateStationInfoAsync();
			return stack.Info_Station.Info;
		}
		
		public eStationType GetStationType()
		{
			return stack.Info_Station.Info.StationType;
		}
		
		public string GetHostName()
		{
			return stack.hostName;
		}
		
		public string GetStationCom()
		{
			return stack.Info_Station.Info.COM;
		}
		
		public string GetStationConnectionType()
		{
			return stack.Info_Station.Info.ConnectionType;
		}
		
		public string GetStationProtocol()
		{
			return stack.Info_Station.Info.Protocol;
		}
		
		public string GetStationModel()
		{
			return stack.Info_Station.Info.Model;
		}
		
		public string GetStationModelType()
		{
			return stack.Info_Station.Info.ModelType;
		}
		
		public int GetStationModelVersion()
		{
			return stack.Info_Station.Info.ModelVersion;
		}
		
		public string GetStationHWversion()
		{
			return stack.Info_Station.Info.Version_Hardware;
		}
		
		public string GetStationSWversion()
		{
			return stack.Info_Station.Info.Version_Software;
		}
		
		public CFirmwareStation[] GetFirmwareVersion()
		{
			List<CFirmwareStation> firmwareVersion = new List<CFirmwareStation>();
			
			foreach (DictionaryEntry stationMicroEntry in stack.Info_Station.Info.StationMicros)
			{
				firmwareVersion.Add((CFirmwareStation) stationMicroEntry.Value);
			}
			
			return firmwareVersion.ToArray();
		}
		
		public GenericStationTools[] GetStationTools()
		{
			GenericStationTools[] tools = new GenericStationTools[0];
			return tools;
		}
		
#endregion
		
#region Station Status
		
		// leer del host datos de estado de la estación
		public async Task<bool> UpdateStationStatusAsync()
		{
			return await stack.UpdateStationStatusAsync();
		}
		
		public ControlModeConnection GetControlMode()
		{
			return stack.Info_Station.Status.ControlMode;
		}
		
		public async void SetControlModeAsync(ControlModeConnection mode, string userName)
		{
			await stack.WriteConnectStatusAsync(mode, userName);
			stack.ReadConnectStatusAsync();
		}
		
		public string GetControlModeUserName()
		{
			return stack.Info_Station.Status.ControlModeUserName;
		}
		
		public void KeepControlMode()
		{
			stack.KeepControlMode();
		}
		
		public StationError GetStationError()
		{
			return stack.Info_Station.Status.ErrorStation;
		}
		
#endregion
		
#region Station Settings
		
		// leer del host configuración de la estación
		public async Task<bool> UpdateStationSettingsAsync()
		{
			return await stack.UpdateStationSettingsAsync();
		}
		
		public string GetStationName()
		{
			return stack.Info_Station.Settings.Name;
		}
		
		public async Task SetStationNameAsync(string newName)
		{
			await stack.WriteDeviceNameAsync(newName);
			//stack.ReadDeviceName()
		}
		
		public string GetStationPIN()
		{
			return stack.Info_Station.Settings.PIN;
		}
		
		public async Task SetStationPINAsync(string newPIN)
		{
			await stack.WriteDevicePINAsync(newPIN);
			//stack.ReadDevicePIN()
		}
		
		// PIN enabled
		public OnOff GetStationPINEnabled()
		{
			return stack.Info_Station.Settings.PINEnabled;
		}
		
		public async Task SetStationPINEnabledAsync(OnOff value)
		{
			await stack.WritePINEnabledAsync(value);
		}
		
		public OnOff GetStationBeep()
		{
			return stack.Info_Station.Settings.Beep;
		}
		
		public async Task SetStationBeepAsync(OnOff beep)
		{
			await stack.WriteBeepAsync(beep);
			//stack.ReadBeep()
		}
		
		// length units
		public CLength.LengthUnit GetStationLengthUnits()
		{
			return stack.Info_Station.Settings.LengthUnit;
		}
		
		public async Task SetStationLengthUnitsAsync(CLength.LengthUnit units)
		{
			await stack.WriteLengthUnitAsync(units);
		}
		
		// Station locked
		public OnOff GetStationLocked()
		{
			return stack.Info_Station.Settings.StationLocked;
		}
		
		public async Task SetStationLockedAsync(OnOff value)
		{
			await stack.WriteStationLockedAsync(value);
		}
		
		public async Task<uint> SetTransactionAsync()
		{
			return await stack.SetTransactionAsync();
		}
		
		public async Task<bool> QueryEndedTransactionAsync(uint transactionID)
		{
			return await stack.QueryEndedTransactionAsync(transactionID);
		}
		
		//Programs
		public CProgramDispenserData_SF GetStationProgram(byte nbrProgram)
		{
			return stack.Info_Station.Settings.Programs[nbrProgram - 1];
		}
		
		public async Task SetStationProgram(byte nbrProgram, CProgramDispenserData_SF program)
		{
			await stack.WriteStationProgramAsync(nbrProgram, program);
		}
		
		public byte GetStationSelectedProgram()
		{
			return stack.Info_Station.Settings.SelectedProgram;
		}
		
		public async Task DeleteStationProgram(byte nbrProgram)
		{
			await stack.DeleteStationProgramAsync(nbrProgram);
		}
		
		public byte[] GetStationConcatenateProgramList()
		{
			return stack.Info_Station.Settings.ConcatenateProgramList;
		}
		
		public async Task SetStationConcatenateProgramList(byte[] programList)
		{
			await stack.WriteStationConcatenateProgramListAsync(programList);
		}
		
#endregion
		
#region Port Status
		
		// leer del host estado del puerto/herramienta
		public async Task<bool> UpdatePortStatus(Port port)
		{
			return await stack.UpdatePortStatusAsync(port);
		}
		
		public GenericStationTools GetPortToolID(Port port)
		{
			return GenericStationTools.NO_TOOL;
		}
		
		public DispenserMode_SF GetPortDispenserMode(Port port)
		{
			return stack.Info_Port[port].ToolStatus.DispenserMode;
		}
		
		public async Task SetPortDispenserModeAsync(Port port, DispenserMode_SF dispenserMode, byte nbrProgram)
		{
			await stack.WritePortDispenserModeAsync(port, dispenserMode, nbrProgram);
		}
		
		public CLength GetPortLength(Port port)
		{
			return stack.Info_Port[port].ToolStatus.Length;
		}
		
		public async Task SetPortLengthAsync(Port port, CLength length)
		{
			await stack.WritePortLengthAsync(port, length);
		}
		
		public CSpeed GetPortSpeed(Port port)
		{
			return stack.Info_Port[port].ToolStatus.Speed;
		}
		
		public async Task SetPortSpeedAsync(Port port, CSpeed speed)
		{
			await stack.WritePortSpeedAsync(port, speed);
		}
		
		public OnOff GetPortFeedingState(Port port)
		{
			return stack.Info_Port[port].ToolStatus.FeedingState;
		}
		
#endregion
		
#region Counters
		
		// leer del host los datos de contadores del puerto
		public async Task<bool> UpdatePortCountersAsync(Port port)
		{
			return await stack.UpdatePortCountersAsync(port);
		}
		
		// Global counters
		
		public int GetStationPluggedMinutes()
		{
			return stack.Info_Port[0].Counters.ContPlugMinutes;
		}
		
		public long GetPortToolTinLength(Port port)
		{
			return stack.Info_Port[port].Counters.ContTinLength;
		}
		
		public int GetPortToolPluggedMinutes(Port port)
		{
			return stack.Info_Port[port].Counters.ContPlugMinutes;
		}
		
		public int GetPortToolWorkMinutes(Port port)
		{
			return stack.Info_Port[port].Counters.ContWorkMinutes;
		}
		
		public int GetPortToolWorkCycles(Port port)
		{
			return stack.Info_Port[port].Counters.ContWorkCycles;
		}
		
		public int GetPortToolIdleMinutes(Port port)
		{
			return stack.Info_Port[port].Counters.ContIdleMinutes;
		}
		
		// Partial counters
		
		public int GetStationPluggedMinutesPartial()
		{
			return stack.Info_Port[0].PartialCounters.ContPlugMinutes;
		}
		
		public long GetPortToolTinLengthPartial(Port port)
		{
			return stack.Info_Port[port].PartialCounters.ContTinLength;
		}
		
		public int GetPortToolPluggedMinutesPartial(Port port)
		{
			return stack.Info_Port[port].PartialCounters.ContPlugMinutes;
		}
		
		public int GetPortToolWorkMinutesPartial(Port port)
		{
			return stack.Info_Port[port].PartialCounters.ContWorkMinutes;
		}
		
		public int GetPortToolWorkCyclesPartial(Port port)
		{
			return stack.Info_Port[port].PartialCounters.ContWorkCycles;
		}
		
		public int GetPortToolIdleMinutesPartial(Port port)
		{
			return stack.Info_Port[port].PartialCounters.ContIdleMinutes;
		}
		
		public void ResetPortToolMinutesPartial(Port port)
		{
			stack.ResetPortToolMinutesPartialAsync(port);
		}
		
#endregion
		
#region Communication
		
		public async Task<bool> UpdateRobotConfigurationAsync()
		{
			return await stack.UpdateRobotConfigurationAsync();
		}
		
		public CRobotData GetRobotConfiguration()
		{
			return stack.Info_Station.Settings.Robot;
		}
		
		public async Task SetRobotConfigurationAsync(CRobotData robotData)
		{
			await stack.WriteRobotConfigurationAsync(robotData);
		}
		
#endregion
		
#region Update Firmware
		
		public void UpdateStationsFirmware(List<CFirmwareStation> stationList)
		{
			stack.UpdateStationsFirmware(stationList);
		}
		
		public List<string> GetStationListUpdating()
		{
			return stack.GetStationListUpdating();
		}
		
#endregion
		
#endregion
		
#region Station Private events
		
		private void stack_ConnectError(Remote_StackSold.EnumConnectError ErrorType, string sMsg, string sStackFunction, dc_EnumConstJBCdc_FaultError serviceErrorCode, string serviceOperation)
			{
			
			Debug.Print("ConnectError en API stack station {0} en host {1} (ejecuta launchStationDisconnected + myAPI.StationSearcherTCP.StationDisconnected + stack.Eraser)", 
				stack.Info_Station.Settings.Name, stack.hostName);
			
			if (ErrorType == Remote_StackHA.EnumConnectError.WCF_STACK)
			{
				myAPI.launchUserError(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication error: '" + sMsg + "' (function: " + sStackFunction + ").", new byte[1] {0}));
				
			}
			else if (ErrorType == Remote_StackHA.EnumConnectError.WCF_SERVICE)
			{
				if (serviceErrorCode == dc_EnumConstJBCdc_FaultError.NotControlledError | 
					serviceErrorCode == dc_EnumConstJBCdc_FaultError.StationNotFound)
				{
					// 26/11/2015
					//myAPI.ServicesSearcher.StationDisconnected(stack.Info_Station.hostEndPointAddress, stack.Info_Station.hostStnID)
					//myAPI.launchStationDisconnected(ID)
					//Eraser()
				}
			}
			
			myAPI.launchStationDisconnected(UUID);
		}
		
#endregion
		
	}
	
}
