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

using System.ServiceModel;
using JBC_ConnectRemote.JBCService;
using System.Threading.Tasks;


namespace JBC_ConnectRemote
{
	
	
	internal class Remote_StackSF : Remote_Stack
	{
		
		public CPortData_SF[] Info_Port;
		public CStationData_SF Info_Station;
		public string hostName;
		private string m_hostStnUUID;
		
		private JBC_ConnectRemote.JBCService.JBCStationControllerServiceClient m_hostService;
		
		internal int ContTimer_Sync = 0; // Cuenta el número de veces que se activa el evento del timer
		internal int ContTimer_1s = 0; // Cuenta el número de veces que se activa el evento del timer
		internal int ContTimer_60s = 0; // Cuenta el número de veces que se activa el evento del timer
		
		internal bool bSendCommandsFirstTime = true;
		
		public delegate void ConnectErrorEventHandler(EnumConnectError ErrorType, string sMsg, string sStackFunction, dc_EnumConstJBCdc_FaultError serviceErrorCode, 
			string serviceOperation);
		private ConnectErrorEventHandler ConnectErrorEvent;
		
		public event ConnectErrorEventHandler ConnectError
		{
			add
			{
				ConnectErrorEvent = (ConnectErrorEventHandler) System.Delegate.Combine(ConnectErrorEvent, value);
			}
			remove
			{
				ConnectErrorEvent = (ConnectErrorEventHandler) System.Delegate.Remove(ConnectErrorEvent, value);
			}
		}
		
		
		
#region METODOS PUBLICOS
		
		public Remote_StackSF(ref JBCStationControllerServiceClient hostService, string hostStnUUID)
		{
			connectErrorStatus = EnumConnectError.NO_ERROR;
			
			ServiceProtocol = CStation.EnumProtocol.Protocol_01;
			TimerManagement = new System.Timers.Timer();
			TimerManagement.Elapsed += TimerStatus_Tick;
			TimerManagement.Elapsed += TimerStatus_Tick;
			TimerManagement.Elapsed += TimerStatus_Tick;
			
			// Se paran los timer por si acaso hasta que se optenga la conexión correcta
			TimerManagement.Interval = TIMER_STATUS_TIME;
			TimerManagement.AutoReset = false; // se debe activar de nuevo al salir del evento Elapsed
			TimerManagement.Stop();
			// Se crea la instancia de la clase Station
			Info_Station = new CStationData_SF();
			
			// Se guarda el servicio y el ID de estación del host
			SaveHostService(hostService, hostStnUUID);
			
			//Await LoadInitStationInfoAsync()
			//Ejecutar sin Async porque no puedo especificar Await
			LoadInitStationInfo();
		}
		
		private void LoadInitStationInfo()
		{
			
			// load station info
			if (UpdateStationInfo())
			{
				// Se crean las instancias de la clase Port para almacenar los datos de puertos
				Info_Port = new CPortData_SF[Info_Station.Info.PortCount - 1 + 1];
				for (int index = 0; index <= Info_Station.Info.PortCount - 1; index++)
				{
					Info_Port[index] = new CPortData_SF();
				}
			}
			
			UpdateStationSettings();
		}
		
		public new void StartStack()
		{
			ContTimer_Sync = 0;
			
			// Se activa el timer para pedir datos
			TimerManagement.Interval = TIMER_STATUS_TIME;
			TimerManagement.AutoReset = false; // se debe activar de nuevo al salir del evento Elapsed
			TimerManagement.Start();
		}
		
#endregion
		
#region METODOS PRIVADOS DE EVENTOS EN CLASES DERIVADAS
		
		protected void RaiseEventError(Exception ex, string sStackFunction)
		{
			string sError = ex.Message;
			if (ex.InnerException != null)
			{
				sError = sError + " (" + ex.InnerException.Message + ")";
			}
			connectErrorStatus = EnumConnectError.WCF_STACK;
			if (ConnectErrorEvent != null)
				ConnectErrorEvent(EnumConnectError.WCF_STACK, sError, sStackFunction, 0, "");
		}
		
#endregion
		
#region METODOS PRIVADOS
		
#region Routines
		
		protected async Task LoadStationParamAsync()
		{
			await UpdateStationSettingsAsync();
			await UpdateStationStatusAsync();
		}
		
		protected async Task LoadAllPortStatusAsync()
		{
			for (var index = 0; index <= Info_Port.Length - 1; index++)
			{
				await UpdatePortStatusAsync((Port) index);
			}
		}
		
		protected async Task LoadRobotConfigurationAsync()
		{
			await UpdateRobotConfigurationAsync();
		}
		
		protected async Task LoadAllCountersAsync()
		{
			for (var index = 0; index <= Info_Port.Length - 1; index++)
			{
				await UpdatePortCountersAsync((Port) index);
			}
		}
		
#endregion
		
#region Timers and Events
		
		public async void TimerStatus_Tick(object sender, System.EventArgs e)
		{
			
			//SyncLock LockTimer02
			// dentro de un try, porque se puede haber producido la desconexión de la estación
			// y haberse borrado las clases y threads correpondientes
			try
			{
				if (connectErrorStatus != EnumConnectError.NO_ERROR)
				{
					return ;
				}
				
				// el UpdateStationStatus se usa para sincronismo
				if (ContTimer_Sync > (TIMER_SYNC_COUNT - 1))
				{
					await UpdateStationStatusAsync();
					// obtener datos de modo continuo
					ContTimer_Sync = 0;
				}
				else
				{
					ContTimer_Sync++;
				}
				
				if (bSendCommandsFirstTime)
				{
					// primera solicitud de todos los valores
					await LoadStationParamAsync();
					await LoadAllPortStatusAsync();
					await LoadAllCountersAsync();
					await LoadRobotConfigurationAsync();
					bSendCommandsFirstTime = false;
				}
				
			}
			catch (Exception)
			{
				
			}
			
			// se vuelve a activar
			if (TimerManagement != null)
			{
				TimerManagement.Start();
			}
			
			//End SyncLock
		}
		
#endregion
		
#region Save Service and host station ID
		
		/// <summary>
		/// Guarda el servicio y el ID de la estación del host
		/// </summary>
		/// <remarks></remarks>
		internal new void SaveHostService(JBCStationControllerServiceClient hostService, string hostStnUUID)
		{
			// Guarda el número de dispositivo dentro del protocolo
			m_hostStnUUID = hostStnUUID;
			m_hostService = hostService;
		}
		
#endregion
		
#endregion
		
#region COMMANDS
		
#region Station Info
		
		/// <summary>
		/// Lee del host la información estática (modelo, puertos, vesión del hard y del soft, etc) de la estación
		/// </summary>
		/// <remarks></remarks>
		internal new async Task<bool> UpdateStationInfoAsync()
		{
			// ' SyncLock ServiceStackJBC01_Lock
			
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_StationController_Info hostInfo = await m_hostService.GetStationControllerInfoAsync();
				hostName = hostInfo.PCName;
				dc_Station_Sold_Info data = await m_hostService.GetStationInfoAsync(m_hostStnUUID); // 26/11/2015 se añade await y async
				CConvertStationInfoFromDC.CopyData(Info_Station.Info, data);
				
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			// ' End SyncLock
		}
		
		internal new bool UpdateStationInfo()
		{
			// ' SyncLock ServiceStackJBC01_Lock
			
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_StationController_Info hostInfo = m_hostService.GetStationControllerInfo();
				hostName = hostInfo.PCName;
				dc_Station_Sold_Info data = m_hostService.GetStationInfo(m_hostStnUUID);
				CConvertStationInfoFromDC.CopyData(Info_Station.Info, data);
				
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			// ' End SyncLock
		}
		
#endregion
		
#region Station Status
		
		/// <summary>
		/// Lee del host la información de estado de la estación
		/// </summary>
		/// <remarks></remarks>
		internal new async Task<bool> UpdateStationStatusAsync()
		{
			//SyncLock ServiceStackJBC01_Lock
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_Station_SF_Status data = await m_hostService.GetStationStatus_SFAsync(m_hostStnUUID);
				CConvertStationStatusFromDC.CopyData_SF(Info_Station.Status, data);
				
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			//End SyncLock
		}
		
#region Station Control Mode
		
		/// <summary>
		/// Lee del equipo el estado de la conexión USB o ETH:
		///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
		///   * Control Mode: en este estado la estación sólo es configurable desde el PC
		/// </summary>
		/// <remarks></remarks>
		internal new async void ReadConnectStatusAsync()
		{
			// ' SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				Info_Station.Status.ControlMode = (ControlModeConnection) (await m_hostService.GetControlModeAsync(m_hostStnUUID));
				
				// FALTA actualizar tipo y nombre estación
				// FLATA obtener estados de USB y Ethernet
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
		/// <summary>
		/// Guarda en el equipo el estado de la conexión USB o ETH:
		///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
		///   * Control Mode: en este estado la estación sólo es configurable desde el PC
		/// </summary>
		/// <remarks></remarks>
		public new async Task WriteConnectStatusAsync(ControlModeConnection mode, string userName)
		{
			// SyncLock ServiceStackJBC01_Lock
			try
			{
				await m_hostService.SetControlModeAsync(m_hostStnUUID, (JBC_ConnectRemote.JBCService.dc_EnumConstJBCdc_ControlModeConnection) mode, userName);
				Info_Station.Status.ControlMode = (ControlModeConnection) (await m_hostService.GetControlModeAsync(m_hostStnUUID));
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			// End SyncLock
		}
		
		internal void KeepControlMode()
		{
			// SyncLock ServiceStackJBC01_Lock
			try
			{
				m_hostService.KeepControlMode(m_hostStnUUID);
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			// End SyncLock
		}
		
#endregion
		
#endregion
		
#region Station Settings
		
#region Get Station Settings
		
		/// <summary>
		/// Lee del host la información de configuración de la estación
		/// </summary>
		/// <remarks></remarks>
		internal new async Task<bool> UpdateStationSettingsAsync()
		{
			// SyncLock ServiceStackJBC01_Lock
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_Station_SF_Settings data = await m_hostService.GetStationSettings_SFAsync(m_hostStnUUID);
				CConvertStationSettingsFromDC.CopyData_SF(Info_Station.Settings, data);
				
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			// End SyncLock
		}
		
		/// <summary>
		/// Lee del host la información de configuración de la estación
		/// </summary>
		/// <remarks></remarks>
		internal new bool UpdateStationSettings()
		{
			// SyncLock ServiceStackJBC01_Lock
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_Station_SF_Settings data = m_hostService.GetStationSettings_SF(m_hostStnUUID);
				CConvertStationSettingsFromDC.CopyData_SF(Info_Station.Settings, data);
				
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			// End SyncLock
		}
		
#endregion
		
#region Set Station Name
		
		/// <summary>
		/// Permite configurar el nombre del equipo conectado
		/// </summary>
		/// <param name="Value">Tamaño máximo del string 16</param>
		/// <remarks></remarks>
		public new async Task WriteDeviceNameAsync(string Value)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetStationNameAsync(m_hostStnUUID, Value);
				Info_Station.Settings.Name = Value;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Set PIN
		
		/// <summary>
		/// Permite configurar el PIN del equipo conectado
		/// </summary>
		/// <param name="Value"></param>
		/// <remarks></remarks>
		public new async Task WriteDevicePINAsync(string Value)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				// si no es dato válido no hacer nada
				if (Value.Length != 4)
				{
					return ;
				}
				
				await m_hostService.SetStationPINAsync(m_hostStnUUID, Value);
				Info_Station.Settings.PIN = Value;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Set PIN Enabled :fet:
		
		/// <summary>
		/// Guarda en el Equipo la activación del PIN
		/// </summary>
		/// <remarks></remarks>
		public new async Task WritePINEnabledAsync(OnOff value)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetStationPINEnabledAsync(m_hostStnUUID, (JBCService.dc_EnumConstJBCdc_OnOff) value);
				Info_Station.Settings.PINEnabled = value;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Set Units
		
		/// <summary>
		/// Guarda en el Equipo las unidades de representación de longitud
		/// </summary>
		/// <remarks></remarks>
		public new async Task WriteLengthUnitAsync(CLength.LengthUnit value)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetStationLengthUnitAsync(m_hostStnUUID, (dc_EnumConstJBCdc_LengthUnit) value);
				Info_Station.Settings.LengthUnit = value;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Set Beep
		
		/// <summary>
		/// Guarda en el Equipo el Límite de potencia
		/// </summary>
		/// <remarks></remarks>
		public new async Task WriteBeepAsync(OnOff value)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetStationBeepAsync(m_hostStnUUID, (JBCService.dc_EnumConstJBCdc_OnOff) value);
				Info_Station.Settings.Beep = value;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Set Station Locked :fet:
		
		/// <summary>
		/// Guarda en el Equipo la activación del PIN
		/// </summary>
		/// <remarks></remarks>
		public new async Task WriteStationLockedAsync(OnOff value)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetStationLockedAsync(m_hostStnUUID, (JBCService.dc_EnumConstJBCdc_OnOff) value, "", (uint) 0, false);
				Info_Station.Settings.StationLocked = value;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Program
		
		public new async Task WriteStationProgramAsync(byte nbrProgram, CProgramDispenserData_SF program)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				dc_ProgramDispenserData_SF programDC = new dc_ProgramDispenserData_SF();
				programDC.Name = program.Name;
				programDC.Length_1 = program.Length_1;
				programDC.Speed_1 = program.Speed_1;
				programDC.Length_2 = program.Length_2;
				programDC.Speed_2 = program.Speed_2;
				programDC.Length_3 = program.Length_3;
				programDC.Speed_3 = program.Speed_3;
				await m_hostService.SetStationProgramAsync(m_hostStnUUID, nbrProgram, programDC);
				Info_Station.Settings.Programs[nbrProgram] = program;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
		public new async Task DeleteStationProgramAsync(byte nbrProgram)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.DeleteStationProgramAsync(m_hostStnUUID, nbrProgram);
				Info_Station.Settings.Programs[nbrProgram].Enabled = OnOff._OFF;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Program List
		
		public new async Task WriteStationConcatenateProgramListAsync(byte[] programList)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetStationConcatenateProgramListAsync(m_hostStnUUID, programList);
				Info_Station.Settings.ConcatenateProgramList = programList;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#endregion
		
#region Port Status
		
#region Get Port Status
		
		/// <summary>
		/// Lee del host la información de estado del puerto indicado
		/// Además obtiene la configuración de puerto/herramienta si hay una conectada, y actualiza los datos
		/// </summary>
		/// <remarks></remarks>
		internal new async Task<bool> UpdatePortStatusAsync(Port Port)
		{
			// SyncLock ServiceStackJBC01_Lock
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_StatusTool_SF data = await m_hostService.GetPortStatus_SFAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port);
				
				// port status
				CConvertStatusToolFromDC.CopyData_SF(Info_Port[Port], data);
				
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			return ret;
			// End SyncLock
		}
		
#endregion
		
#region Dispenser mode
		
		public new async Task WritePortDispenserModeAsync(Port port, DispenserMode_SF dispenserMode, byte nbrProgram)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetPortDispenserModeAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) port, (dc_EnumConstJBCdc_DispenserMode_SF) dispenserMode, nbrProgram);
				Info_Port[port].ToolStatus.DispenserMode = dispenserMode;
				Info_Station.Settings.SelectedProgram = nbrProgram;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Length
		
		public new async Task WritePortLengthAsync(Port port, CLength length)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetPortLengthAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) port, length.ToMillimeters(), dc_EnumConstJBCdc_LengthUnit.MILLIMETERS);
				Info_Port[port].ToolStatus.Length = length;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Speed
		
		public new async Task WritePortSpeedAsync(Port port, CSpeed speed)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetPortSpeedAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) port, speed.ToMillimetersPerSecond(), dc_EnumConstJBCdc_SpeedUnit.MILLIMETERS_PER_SECOND);
				Info_Port[port].ToolStatus.Speed = speed;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#endregion
		
#region Counters
		
#region Update Port Counters
		
		/// <summary>
		/// Lee del host la información de contadores del puerto indicado
		/// </summary>
		/// <remarks></remarks>
		internal new async Task<bool> UpdatePortCountersAsync(Port Port)
		{
			// SyncLock ServiceStackJBC01_Lock
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_Port_Counters_SF data = await m_hostService.GetPortCounters_SFAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port);
				CConvertCountersFromDC.CopyData_SF(Info_Port[Port].Counters, data.GlobalCounters);
				CConvertCountersFromDC.CopyData_SF(Info_Port[Port].PartialCounters, data.PartialCounters);
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			// End SyncLock
		}
		
#endregion
		
#region Reset Counters
		
		/// <summary>
		/// Pone a cero los contadores parciales del puerto
		/// </summary>
		/// <remarks></remarks>
		public new async void ResetPortToolMinutesPartialAsync(Port port)
		{
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.ResetPortPartialCountersAsync(m_hostStnUUID, (JBCService.dc_EnumConstJBCdc_Port) port);
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
		}
		
#endregion
		
#endregion
		
#region Communication
		
		/// <summary>
		/// Lee del host la información de robot de la estación
		/// </summary>
		/// <remarks></remarks>
		internal new async Task<bool> UpdateRobotConfigurationAsync()
		{
			//SyncLock ServiceStackJBC01_Lock
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ret;
			}
			
			try
			{
				dc_RobotConfiguration rbtConf = await m_hostService.GetRobotConfigurationAsync(m_hostStnUUID);
				CConvertRobotConfigurationFromDC.CopyData(Info_Station.Settings.Robot, rbtConf);
				
				ret = true;
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			//End SyncLock
		}
		
		/// <summary>
		/// Permite configurar el robot de la estación
		/// </summary>
		/// <param name="Value">Configuración del robot</param>
		/// <remarks></remarks>
		public new async Task WriteRobotConfigurationAsync(CRobotData Value)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				dc_RobotConfiguration rbtConfigurationDC = new dc_RobotConfiguration();
				rbtConfigurationDC.Status = (JBCService.dc_EnumConstJBCdc_OnOff) ((dc_EnumConstJBCdc_OnOff) Value.Status);
				rbtConfigurationDC.Protocol = (JBCService.dc_EnumConstJBCdc_RobotProtocol) ((dc_EnumConstJBCdc_RobotProtocol) Value.Protocol);
				rbtConfigurationDC.Address = Value.Address;
				rbtConfigurationDC.Speed = (JBCService.dc_EnumConstJBCdc_RobotSpeed) ((dc_EnumConstJBCdc_RobotSpeed) Value.Speed);
				rbtConfigurationDC.DataBits = Value.DataBits;
				rbtConfigurationDC.StopBits = (JBCService.dc_EnumConstJBCdc_RobotStop) ((dc_EnumConstJBCdc_RobotStop) Value.StopBits);
				rbtConfigurationDC.Parity = (JBCService.dc_EnumConstJBCdc_RobotParity) ((dc_EnumConstJBCdc_RobotParity) Value.Parity);
				
				await m_hostService.SetRobotConfigurationAsync(m_hostStnUUID, rbtConfigurationDC);
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Station Messages Transaction
		
		/// <summary>
		/// Se inicia una transacción
		/// SetTransaction, en la JBC Connect DLL, envía un mensaje M_ACK para que la estación devuelva un M_ACK. Se devuelve el número de mensaje.
		/// Cuando la estación recibe un M_ACK, genera un Evento de confirmación con el número de mensaje.
		/// Se utiliza para confirmar que se han ejecutado las operaciones anteriores
		/// </summary>
		/// <remarks></remarks>
		public new async Task<uint> SetTransactionAsync()
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return 0;
			}
			
			uint ret = null;
			
			try
			{
				ret = System.Convert.ToUInt32(await m_hostService.SetTransactionAsync(m_hostStnUUID));
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return ret;
			// End SyncLock
		}
		
		/// <summary>
		/// Se consulta si ha finalizado la transacción.
		/// SetTransaction, en la JBC Connect DLL, envía un mensaje M_ACK para que la estación devuelva un M_ACK. Se devuelve el número de mensaje.
		/// Cuando la estación recibe un M_ACK, genera un Evento de confirmación con el número de mensaje.
		/// Se utiliza para confirmar que se han ejecutado las operaciones anteriores
		/// </summary>
		/// <remarks></remarks>
		public new async Task<bool> QueryEndedTransactionAsync(uint transactionID)
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return false;
			}
			
			try
			{
				return await m_hostService.QueryEndedTransactionAsync(m_hostStnUUID, transactionID);
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			return false;
			// End SyncLock
		}
		
#endregion
		
#region Reset Station
		
		/// <summary>
		/// Close and reinitialize station
		/// </summary>
		/// <remarks></remarks>
		public new async void DeviceResetAsync()
		{
			// SyncLock ServiceStackJBC01_Lock
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.ResetStationAsync(m_hostStnUUID);
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// End SyncLock
		}
		
#endregion
		
#region Reset Station Parameters
		
		/// <summary>
		/// Le pide que resetee todos los parámetros de estación y que deje el equipo con la configuración de fábrica
		/// </summary>
		/// <remarks></remarks>
		public new async Task SetDefaultStationParamsAsync()
		{
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				await m_hostService.SetDefaultStationParamsAsync(m_hostStnUUID);
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			
			// Después del reset pide de nuevo todos los parámetros
			await LoadStationParamAsync();
		}
		
#endregion
		
#region Update Firmware
		
		internal void UpdateStationsFirmware(List<CFirmwareStation> stationList)
		{
			//SyncLock ServiceStackJBC01_Lock
			bool ret = false;
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return ;
			}
			
			try
			{
				List<dc_FirmwareStation> dcStationList = new List<dc_FirmwareStation>();
				foreach (CFirmwareStation firmware in stationList)
				{
					dc_FirmwareStation dcFirmwareUpdate = new dc_FirmwareStation();
					dcFirmwareUpdate.stationUUID = firmware.StationUUID;
					dcFirmwareUpdate.softwareVersion = firmware.SoftwareVersion;
					dcFirmwareUpdate.hardwareVersion = firmware.HardwareVersion;
					
					dcStationList.Add(dcFirmwareUpdate);
				}
				
				m_hostService.UpdateStations(dcStationList.ToArray());
				
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			//End SyncLock
		}
		
		internal List<string> GetStationListUpdating()
		{
			//SyncLock ServiceStackJBC01_Lock
			List<string> stationListUpdating = new List<string>();
			if (connectErrorStatus != EnumConnectError.NO_ERROR)
			{
				return stationListUpdating;
			}
			
			try
			{
				stationListUpdating.AddRange(m_hostService.GetStationListUpdating());
				
			}
			catch (FaultException<faultError> faultEx)
			{
				RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			catch (Exception ex)
			{
				RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
			}
			//End SyncLock
			
			return stationListUpdating;
		}
		
#endregion
		
#endregion
		
	}
	
}
