using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using DataJBC;
using JBC_Connect;
using RoutinesJBC;
using RoutinesLibrary;
using Constants = DataJBC.Constants;

using System.Management;


// Fault error without custom error
//Throw New faultexception("Error in " & System.Reflection.MethodBase.GetCurrentMethod().Name & " (" & ex.Message & ")", _
//New FaultCode("NotControlledError"))
// Manejo de errores en las funciones
// - Si se produce un error controlado, se lanza una FaultException con estructura customizada: FaultException(Of faultError).
// - Todas las funciones contienen un try general con 2 catchs:
//   un catch para reenviar errores controlados lanzados dentro del try general: catch FaultException(Of faultError)
//   y luego un catch general, para enviar los errores no controlador: catch Exception

namespace JBCStationControllerSrv
{
	//Por defecto, WCF se instancia con concurrency=single. Esto quiere decir que las peticiones se resuelven una por una y no en paralelo.
	//InstanceContexMode=Single implica que una única instancia del servicio será creada para toda la vida del servicio. Esto permite
	//disponer de timers (pe, schedule updates) que solo son creados una única vez.
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single,
			InstanceContextMode = InstanceContextMode.Single)]
		public class CImplIJBCStationControllerService : IJBCStationControllerService
	{

		private CComHostController m_comHostController = new CComHostController();
		//Private m_inputKeyboardDevice As New CKeyboardMessageHandlerServiceHelper
		private CUserSession m_userSession = new CUserSession();
		private CStationRemoteControl m_stationRemoteControl = new CStationRemoteControl();
		private CTraceData m_TraceData = new CTraceData();
        private CStationWorkingEvent m_stationWorkingEvent;
        private CFAEAutomaticWorking m_FAEAutomaticWorking;


#region StationController Service Management

		public CImplIJBCStationControllerService()
		{
			//Events from JBC_Connect
			DLLConnection.jbc.NewStationConnected += Event_StationConnected;
			DLLConnection.jbc.StationDisconnected += Event_StationDisconnected;

			//Events from Input Keyboard
			//AddHandler m_inputKeyboardDevice.KeyboardDisconnected, AddressOf KeyboardDisconnected
			//AddHandler m_inputKeyboardDevice.KeyboardMessage, AddressOf KeyboardMessage

            if (My.Settings.Default.EnableStationWorkingEvent)
            {
                m_stationWorkingEvent = new CStationWorkingEvent();
                m_FAEAutomaticWorking = new CFAEAutomaticWorking();
                m_stationWorkingEvent.StationWorking += m_FAEAutomaticWorking.StationWorking;
            }
		}

		public dc_StationController_Info GetStationControllerInfo()
		{
			dc_StationController_Info ret = new dc_StationController_Info();
			Console.WriteLine(" GetStationControllerInfo: ");
			try
			{
				ret.PCName = Environment.MachineName;
				ret.SwVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

				ret.PCUID = "";
				ManagementClass mc = new ManagementClass("Win32_Processor");
				ManagementObjectCollection moc = mc.GetInstances();
				foreach (ManagementObject mo in moc)
				{
					if (string.IsNullOrEmpty(ret.PCUID))
					{
						ret.PCUID = System.Convert.ToString(mo.Properties["ProcessorId"].Value.ToString());
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			Console.WriteLine("    PCName: " + ret.PCName);
			Console.WriteLine("    SwVersion: " + ret.SwVersion);
			return ret;
		}

		public void StationControllerSearch(dc_EnumConstJBC.dc_StationControllerAction action, dc_EnumConstJBC.dc_StationControllerConnType conntype)
		{
			try
			{
				if (action == dc_EnumConstJBC.dc_StationControllerAction.StartSearch)
				{
					if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.USB)
					{
						DLLConnection.jbc.StartSearch(SearchMode.USB);
					}
					else if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.ETH)
					{
						DLLConnection.jbc.StartSearch(SearchMode.ETH);
					}
				}
				else if (action == dc_EnumConstJBC.dc_StationControllerAction.StopSearch)
				{
					if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.USB)
					{
						DLLConnection.jbc.StopSearch(SearchMode.USB);
					}
					else if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.ETH)
					{
						DLLConnection.jbc.StopSearch(SearchMode.ETH);
					}
				}
				else if (action == dc_EnumConstJBC.dc_StationControllerAction.InitialSearchOn)
				{
					if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.USB)
					{
						My.Settings.Default.SearchUSB = true;
					}
					else if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.ETH)
					{
						My.Settings.Default.SearchETH = true;
					}
					My.Settings.Default.Save();
				}
				else if (action == dc_EnumConstJBC.dc_StationControllerAction.InitialSearchOff)
				{
					if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.USB)
					{
						My.Settings.Default.SearchUSB = false;
					}
					else if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.ETH)
					{
						My.Settings.Default.SearchETH = false;
					}
					My.Settings.Default.Save();
				}
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
			}
		}

		public bool StationControllerIsSearching(dc_EnumConstJBC.dc_StationControllerConnType conntype)
		{
			try
			{
				if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.USB)
				{
					return DLLConnection.jbc.isSearching(SearchMode.USB);
				}
				else if (conntype == dc_EnumConstJBC.dc_StationControllerConnType.ETH)
				{
					return DLLConnection.jbc.isSearching(SearchMode.ETH);
				}
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
			}

			return false;
		}

		public long GetDate()
		{
			return DateTime.Now.ToBinary();
		}

		public bool SetDate(long newBinaryDate)
		{
			bool bOk = false;

			try
			{
				DateTime newDate = DateTime.FromBinary(newBinaryDate);
				Microsoft.VisualBasic.DateAndTime.TimeOfDay = newDate;
				Microsoft.VisualBasic.DateAndTime.DateString = newDate.ToString("M-d-yyyy");
				bOk = true;
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

#endregion

#region Station List

		public int GetStationCount()
		{
			int ret = -1;
			Console.WriteLine(" GetStationCount: ");
			try
			{
				ret = DLLConnection.jbc.GetStationCount();
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
			Console.WriteLine("     Count: " + ret);
			return ret;
		}

		public string[] GetStationList()
		{
			Console.WriteLine(" GetStationList: ");
			string[] ret = new string[] { };

			try
			{
				ret = DLLConnection.jbc.GetStationList();
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
			Console.WriteLine("    Length: " + ret.Length);
			foreach(string elem in ret)
			{
				Console.WriteLine("      ID: " + elem);
			}

			return ret;
		}

#endregion

#region Station Info :fet:

		public dc_Station_Sold_Info GetStationInfo(string UUID)
		{
			Console.WriteLine(" GetStationInfo: ");
			dc_Station_Sold_Info ret = new dc_Station_Sold_Info();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret.UUID = UUID;
					ret.StationType = (dc_EnumConstJBC.dc_StationType)(DLLConnection.jbc.GetStationType(UUID));
					ret.ParentUUID = DLLConnection.jbc.GetStationParentUUID(UUID);
					ret.COM = DLLConnection.jbc.GetStationCOM(UUID);
					ret.ConnectionType = DLLConnection.jbc.GetStationConnectionType(UUID);
					ret.Model = DLLConnection.jbc.GetStationModel(UUID);
					ret.ModelType = DLLConnection.jbc.GetStationModelType(UUID);
					ret.ModelVersion = DLLConnection.jbc.GetStationModelVersion(UUID);
					ret.PortCount = DLLConnection.jbc.GetPortCount(UUID);
					ret.Protocol = DLLConnection.jbc.GetStationProtocol(UUID);
					ret.TempLevelsCount = 3;
					ret.Version_Hardware = DLLConnection.jbc.GetStationHWversion(UUID);
					ret.Version_Software = DLLConnection.jbc.GetStationSWversion(UUID);

					// supported tools
					GenericStationTools[] tools = DLLConnection.jbc.GetStationTools(UUID);
					ret.SupportedTools = new dc_EnumConstJBC.dc_GenericStationTools[tools.Length - 1 + 1];
					for (var i = 0; i <= tools.Length - 1; i++)
					{
						ret.SupportedTools[(int) i] = (dc_EnumConstJBC.dc_GenericStationTools)(tools[(int)i]);
					}

					// features
					CFeaturesData features = DLLConnection.jbc.GetStationFeatures(UUID);
					ret.Features.Alarms = features.Alarms;
					ret.Features.AllToolsSamePortSettings = features.AllToolsSamePortSettings;
					ret.Features.Cartridges = features.Cartridges;
					ret.Features.DelayWithStatus = features.DelayWithStatus;
					ret.Features.DisplaySettings = features.DisplaySettings;
					ret.Features.Ethernet = features.Ethernet;
					ret.Features.FirmwareUpdate = features.FirmwareUpdate;
					ret.Features.MaxTemp = convTempToStruc(features.MaxTemp);
					ret.Features.MinTemp = convTempToStruc(features.MinTemp);
					ret.Features.ExtTCMaxTemp = convTempToStruc(features.ExtTCMaxTemp);
					ret.Features.ExtTCMinTemp = convTempToStruc(features.ExtTCMinTemp);
					ret.Features.MaxFlow = System.Convert.ToInt32(features.MaxFlow);
					ret.Features.MinFlow = System.Convert.ToInt32(features.MinFlow);
					ret.Features.PartialCounters = features.PartialCounters;
					ret.Features.Peripherals = features.Peripherals;
					ret.Features.Robot = features.Robot;
					ret.Features.TempLevelsWithStatus = features.TempLevelsWithStatus;
					ret.Features.TempLevels = features.TempLevels;

					//info update firmware
					List<CFirmwareStation> updateFirmware = DLLConnection.jbc.GetVersionMicroFirmware(UUID);
					ret.InfoUpdateFirmware = new dc_FirmwareStation[updateFirmware.Count - 1 + 1];
					for (var i = 0; i <= updateFirmware.Count - 1; i++)
					{
						ret.InfoUpdateFirmware[(int)i] = new dc_FirmwareStation();
						ret.InfoUpdateFirmware[(int)i].stationUUID = updateFirmware[i].StationUUID;
						ret.InfoUpdateFirmware[(int)i].model = System.Convert.ToString(updateFirmware[i].Model);
						ret.InfoUpdateFirmware[(int)i].hardwareVersion = System.Convert.ToString(updateFirmware[i].HardwareVersion);
						ret.InfoUpdateFirmware[(int)i].softwareVersion = System.Convert.ToString(updateFirmware[i].SoftwareVersion);
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
			Console.WriteLine("    ID: " + ret.UUID);
			return ret;
		}

#endregion

#region Station Status :fet:
		// get

		// Soldering stations only
		public dc_Station_Sold_Status GetStationStatus(string UUID)
		{
			Console.WriteLine(" GetStationStatus: ");
			dc_Station_Sold_Status ret = new dc_Station_Sold_Status();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					//Remote control mode
					ControlModeConnection controlMode = DLLConnection.jbc.GetControlMode(UUID);
					if (controlMode == ControlModeConnection.MONITOR)
					{
						ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.MONITOR;

					}
					else if (controlMode == ControlModeConnection.ROBOT)
					{
						ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.ROBOT;

					}
					else if (controlMode == ControlModeConnection.CONTROL)
					{

						//Obtiene los datos del cliente que envia la consulta
						var prop = OperationContext.Current.IncomingMessageProperties;
						var endpointRemote = (RemoteEndpointMessageProperty)(prop[RemoteEndpointMessageProperty.Name]);
						string ipRemote = endpointRemote.Address;

						if (m_stationRemoteControl.IsRemoteControl(UUID, ipRemote))
						{
							ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.CONTROL;
						}
						else
						{
							ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.BLOCK_CONTROL;
						}
					}

					ret.ControlModeUserName = m_stationRemoteControl.UserNameRemoteControl(UUID);
					ret.RemoteMode = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetRemoteMode(UUID));
					ret.StationError = (dc_EnumConstJBC.dc_StationError)(DLLConnection.jbc.GetStationError(UUID));
					ret.TempErrorMOS = convTempToStruc(DLLConnection.jbc.GetStationMOSerrorTemp(UUID));
					ret.TempErrorTRAFO = convTempToStruc(DLLConnection.jbc.GetStationTransformerErrorTemp(UUID));
					ret.TRAFOTemp = convTempToStruc(DLLConnection.jbc.GetStationTransformerTemp(UUID));

					CContinuousModeStatus cm = DLLConnection.jbc.GetContinuousMode(UUID);
					ret.ContinuousModeStatus.port1 = cm.port1;
					ret.ContinuousModeStatus.port2 = cm.port2;
					ret.ContinuousModeStatus.port3 = cm.port3;
					ret.ContinuousModeStatus.port4 = cm.port4;
					ret.ContinuousModeStatus.Speed = (dc_EnumConstJBC.dc_SpeedContinuousMode)cm.speed;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
			return ret;
		}

		// Hot Air desoldering stations only
		public dc_Station_HA_Status GetStationStatus_HA(string UUID)
		{
			dc_Station_HA_Status ret = new dc_Station_HA_Status();
			Console.WriteLine(" GetStationStatus_HA: ");
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					//Remote control mode
					ControlModeConnection controlMode = DLLConnection.jbc.GetControlMode(UUID);
					if (controlMode == ControlModeConnection.MONITOR)
					{
						ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.MONITOR;

					}
					else if (controlMode == ControlModeConnection.ROBOT)
					{
						ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.ROBOT;

					}
					else if (controlMode == ControlModeConnection.CONTROL)
					{

						//Obtiene los datos del cliente que envia la consulta
						var prop = OperationContext.Current.IncomingMessageProperties;
						var endpointRemote = (RemoteEndpointMessageProperty)(prop[RemoteEndpointMessageProperty.Name]);
						string ipRemote = endpointRemote.Address;

						if (m_stationRemoteControl.IsRemoteControl(UUID, ipRemote))
						{
							ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.CONTROL;
						}
						else
						{
							ret.ControlMode = dc_EnumConstJBC.dc_ControlModeConnection.BLOCK_CONTROL;
						}
					}

					ret.ControlModeUserName = m_stationRemoteControl.UserNameRemoteControl(UUID);
					ret.RemoteMode = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetRemoteMode(UUID));
					ret.StationError = (dc_EnumConstJBC.dc_StationError)(DLLConnection.jbc.GetStationError(UUID));

					CContinuousModeStatus cm = DLLConnection.jbc.GetContinuousMode(UUID);
					ret.ContinuousModeStatus.port1 = cm.port1;
					ret.ContinuousModeStatus.port2 = cm.port2;
					ret.ContinuousModeStatus.port3 = cm.port3;
					ret.ContinuousModeStatus.port4 = cm.port4;
	ret.ContinuousModeStatus.Speed = (dc_EnumConstJBC.dc_SpeedContinuousMode)cm.speed;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		// Soldering feeder stations
		public dc_Station_SF_Status GetStationStatus_SF(string UUID)
		{
			dc_Station_SF_Status ret = new dc_Station_SF_Status();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					//Remote control mode
					ControlModeConnection controlMode = DLLConnection.jbc.GetControlMode(UUID);
					if (controlMode == ControlModeConnection.MONITOR)
					{
						ret.ControlMode = JBC_Connect.dc_EnumConstJBC.dc_ControlModeConnection.MONITOR;

					}
					else if (controlMode == ControlModeConnection.ROBOT)
					{
						ret.ControlMode = JBC_Connect.dc_EnumConstJBC.dc_ControlModeConnection.ROBOT;

					}
					else if (controlMode == ControlModeConnection.CONTROL)
					{

						//Obtiene los datos del cliente que envia la consulta
						var prop = OperationContext.Current.IncomingMessageProperties;
						var endpointRemote = (RemoteEndpointMessageProperty) (prop[RemoteEndpointMessageProperty.Name]);
						string ipRemote = endpointRemote.Address;

						if (m_stationRemoteControl.IsRemoteControl(UUID, ipRemote))
						{
							ret.ControlMode = JBC_Connect.dc_EnumConstJBC.dc_ControlModeConnection.CONTROL;
						}
						else
						{
							ret.ControlMode = JBC_Connect.dc_EnumConstJBC.dc_ControlModeConnection.BLOCK_CONTROL;
						}
					}

					ret.ControlModeUserName = m_stationRemoteControl.UserNameRemoteControl(UUID);
					ret.StationError = (JBC_Connect.dc_EnumConstJBC.dc_StationError) (DLLConnection.jbc.GetStationError(UUID));
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}


		public dc_EnumConstJBC.dc_ControlModeConnection GetControlMode(string UUID)
		{
			dc_EnumConstJBC.dc_ControlModeConnection ret = dc_EnumConstJBC.dc_ControlModeConnection.MONITOR;
			Console.WriteLine(" GetControlMode: "+ UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					//Remote control mode
					ControlModeConnection controlMode = DLLConnection.jbc.GetControlMode(UUID);
					if (controlMode == ControlModeConnection.MONITOR)
					{
						ret = dc_EnumConstJBC.dc_ControlModeConnection.MONITOR;

					}
					else if (controlMode == ControlModeConnection.ROBOT)
					{
						ret = dc_EnumConstJBC.dc_ControlModeConnection.ROBOT;

					}
					else if (controlMode == ControlModeConnection.CONTROL)
					{

						//Obtiene los datos del cliente que envia la consulta
						var prop = OperationContext.Current.IncomingMessageProperties;
						var endpointRemote = (RemoteEndpointMessageProperty)(prop[RemoteEndpointMessageProperty.Name]);
						string ipRemote = endpointRemote.Address;

						if (m_stationRemoteControl.IsRemoteControl(UUID, ipRemote))
						{
							ret = dc_EnumConstJBC.dc_ControlModeConnection.CONTROL;
						}
						else
						{
							ret = dc_EnumConstJBC.dc_ControlModeConnection.BLOCK_CONTROL;
						}
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		// set

		public void SetControlMode(string UUID, dc_EnumConstJBC.dc_ControlModeConnection mode, string userName)
		{
			Console.WriteLine(" SetControlMode: ");
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					//Obtiene los datos del cliente que envia la consulta
					var prop = OperationContext.Current.IncomingMessageProperties;
					var endpointRemote = (RemoteEndpointMessageProperty)(prop[RemoteEndpointMessageProperty.Name]);
					string ipRemote = endpointRemote.Address;

					if (mode == dc_EnumConstJBC.dc_ControlModeConnection.CONTROL)
					{
						if (m_stationRemoteControl.SetRemoteControl(UUID, ipRemote, userName))
						{
							DLLConnection.jbc.SetControlMode(UUID, ControlModeConnection.CONTROL);
						}
					}
					else
					{
						if (m_stationRemoteControl.RemoveRemoteControl(UUID, ipRemote))
						{
							DLLConnection.jbc.SetControlMode(UUID, ControlModeConnection.MONITOR);
						}
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetRemoteMode(string UUID, dc_EnumConstJBC.dc_OnOff onoff)
		{
			Console.WriteLine(" SetRemoteMode: ");
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					if (onoff == dc_EnumConstJBC.dc_OnOff._ON)
					{
						DLLConnection.jbc.SetRemoteMode(UUID, OnOff._ON);
					}
					else
					{
						DLLConnection.jbc.SetRemoteMode(UUID, OnOff._OFF);
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void KeepControlMode(string UUID)
		{
			Console.WriteLine(" KeepControlMode: ");
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					//Obtiene los datos del cliente que envia la consulta
					var prop = OperationContext.Current.IncomingMessageProperties;
					var endpointRemote = (RemoteEndpointMessageProperty)(prop[RemoteEndpointMessageProperty.Name]);
					string ipRemote = endpointRemote.Address;

					m_stationRemoteControl.KeepControlMode(UUID, ipRemote);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public bool ShowMessage(string UUID, string message, JBC_Connect.dc_EnumConstJBC.dc_MessageType type)
		{
			bool bOk = false;

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					//TODO
					bOk = true;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

#endregion

#region Port Status :fet:

		// get

		// Soldering Station only
		public dc_StatusTool GetPortStatus(string UUID, dc_EnumConstJBC.dc_Port p)
		{
			dc_StatusTool ret = new dc_StatusTool();
			Console.WriteLine(" GetPortStatus: " + UUID );
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret.portNbr = p;
					ret.ActualTemp = convTempToStruc(DLLConnection.jbc.GetPortToolActualTemp(UUID, (Port)p));
					ret.ConnectedTool = (dc_EnumConstJBC.dc_GenericStationTools)(DLLConnection.jbc.GetPortToolID(UUID, (Port)p));
					ret.Desold_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolDesolderStatus(UUID, (Port)p));
					ret.Extractor_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolExtractorStatus(UUID, (Port)p));
					if (DLLConnection.jbc.GetPortToolFutureMode(UUID, (Port)p) == ToolFutureMode.Sleep)
					{
						ret.FutureMode = "S";
					}
					else if (DLLConnection.jbc.GetPortToolFutureMode(UUID, (Port)p) == ToolFutureMode.Hibernation)
					{
						ret.FutureMode = "H";
					}
					else
					{
						ret.FutureMode = "";
					}
					ret.Hiber_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolHibernationStatus(UUID, (Port)p));
					ret.PortSelectedTemp = convTempToStruc(DLLConnection.jbc.GetPortToolSelectedTemp(UUID, (Port)p));
					ret.EnabledPort = (JBC_Connect.dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetEnabledPort(UUID, (Port) p));
					ret.Power_x_Mil = DLLConnection.jbc.GetPortToolActualPower(UUID, (Port)p);
					ret.Sleep_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSleepStatus(UUID, (Port)p));
					ret.Stand_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolStandStatus(UUID, (Port)p));
					ret.Temp_MOS = convTempToStruc(DLLConnection.jbc.GetPortToolMOStemp(UUID, (Port)p));
					ret.TimeToSleepHibern = DLLConnection.jbc.GetPortToolTimeToFutureMode(UUID, (Port)p);
					ret.ToolError = (dc_EnumConstJBC.dc_ToolError)(DLLConnection.jbc.GetPortToolError(UUID, (Port)p));

					if (ret.ConnectedTool != dc_EnumConstJBC.dc_GenericStationTools.NOTOOL)
					{
						ret.This_PortToolSettings.portNbr = p;
						ret.This_PortToolSettings.Tool = ret.ConnectedTool;
						// fixed
						CTemperature temp = DLLConnection.jbc.GetPortToolFixTemp(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool);
						if (temp.UTI == Constants.NO_FIXED_TEMP | temp.UTI == 0)
						{
							ret.This_PortToolSettings.FixedTemp_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
						}
						else
						{
							ret.This_PortToolSettings.FixedTemp_OnOff = dc_EnumConstJBC.dc_OnOff._ON;
						}
						ret.This_PortToolSettings.FixedTemp = convTempToStruc(temp);
						// levels
						ret.This_PortToolSettings.Levels.LevelsCount = 3;
						ret.This_PortToolSettings.Levels.LevelsOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSelectedTempLevelsEnabled(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						ret.This_PortToolSettings.Levels.LevelsTempSelect = (byte)DLLConnection.jbc.GetPortToolSelectedTempLevels(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool);
						ret.This_PortToolSettings.Levels.LevelsTempOnOff = new dc_EnumConstJBC.dc_OnOff[ret.This_PortToolSettings.Levels.LevelsCount - 1 + 1];
						ret.This_PortToolSettings.Levels.LevelsTemp = new dc_getTemperature[ret.This_PortToolSettings.Levels.LevelsCount - 1 + 1];
						for (var i = 0; i <= ret.This_PortToolSettings.Levels.LevelsCount - 1; i++)
						{
							ret.This_PortToolSettings.Levels.LevelsTempOnOff[(int)i] = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolTempLevelEnabled(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool, (ToolTemperatureLevels)i));
							ret.This_PortToolSettings.Levels.LevelsTemp[(int)i] = convTempToStruc(DLLConnection.jbc.GetPortToolTempLevel(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool, (ToolTemperatureLevels)i));
						}

						ret.This_PortToolSettings.SleepTemp = new dc_getTemperature();
						ret.This_PortToolSettings.SleepTime = (dc_EnumConstJBC.dc_TimeSleep)(DLLConnection.jbc.GetPortToolSleepDelay(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						ret.This_PortToolSettings.SleepTimeOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSleepDelayEnabled(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						ret.This_PortToolSettings.HiberTime = (dc_EnumConstJBC.dc_TimeHibernation)(DLLConnection.jbc.GetPortToolHibernationDelay(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						ret.This_PortToolSettings.HiberTimeOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolHibernationDelayEnabled(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						ret.This_PortToolSettings.AdjustTemp = convAdjustTempToStruc(DLLConnection.jbc.GetPortToolAdjustTemp(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						// cartridge
						CCartridgeData cartridge = DLLConnection.jbc.GetPortToolCartridge(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool);
						if (cartridge != null)
						{
							ret.This_PortToolSettings.Cartridge.CartridgeNbr = cartridge.CartridgeNbr;
							ret.This_PortToolSettings.Cartridge.CartridgeOnOff = (dc_EnumConstJBC.dc_OnOff)cartridge.CartridgeOnOff;
						}
					}

					//
					//Peripherals
					//
					List<dc_PeripheralInfo> peripheralList = new List<dc_PeripheralInfo>();

					List<CPeripheralData> peripheralDataList = DLLConnection.jbc.GetPeripheralList(UUID);
					foreach (CPeripheralData peripheralData in peripheralDataList)
					{
						if ((Port)p == peripheralData.PortAttached)
						{
							dc_PeripheralInfo retPeripheral = new dc_PeripheralInfo();

							retPeripheral.ID = peripheralData.ID;
							retPeripheral.Version = peripheralData.Version;
							retPeripheral.Hash_MCU_UID = peripheralData.Hash_MCU_UID;
							retPeripheral.DateTime = peripheralData.DateTime;
							retPeripheral.Type = (dc_EnumConstJBC.dc_PeripheralType)peripheralData.Type;
							retPeripheral.PortAttached = (dc_EnumConstJBC.dc_Port)peripheralData.PortAttached;
							retPeripheral.WorkFunction = (dc_EnumConstJBC.dc_PeripheralFunction)peripheralData.WorkFunction;
							retPeripheral.ActivationMode = (dc_EnumConstJBC.dc_PeripheralActivation)peripheralData.ActivationMode;
							retPeripheral.DelayTime = peripheralData.DelayTime;
							retPeripheral.StatusActive = (dc_EnumConstJBC.dc_OnOff)peripheralData.StatusActive;
							retPeripheral.StatusPD = (dc_EnumConstJBC.dc_PeripheralStatusPD)peripheralData.StatusPD;

							peripheralList.Add(retPeripheral);
						}
					}
					ret.Peripheral = peripheralList.ToArray();

				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
			Console.WriteLine("    Status: "+ ret.Hiber_OnOff);
			return ret;
		}

		// Hot Air Desoldering Station only
		public dc_StatusTool_HA GetPortStatus_HA(string UUID, dc_EnumConstJBC.dc_Port p)
		{
			Console.WriteLine(" GetPortStatus_HA: ");
			dc_StatusTool_HA ret = new dc_StatusTool_HA();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret.portNbr = p;
					ret.ConnectedTool = (dc_EnumConstJBC.dc_GenericStationTools)(DLLConnection.jbc.GetPortToolID(UUID, (Port)p));
					ret.ActualTemp = convTempToStruc(DLLConnection.jbc.GetPortToolActualTemp(UUID, (Port)p));
					ret.ProtectionTC_Temp = convTempToStruc(DLLConnection.jbc.GetPortToolProtectionTCTemp(UUID, (Port)p));
					ret.ActualExtTemp = convTempToStruc(DLLConnection.jbc.GetPortToolActualExternalTemp(UUID, (Port)p));
					ret.Power_x_Mil = DLLConnection.jbc.GetPortToolActualPower(UUID, (Port)p);
					ret.Flow_x_Mil = DLLConnection.jbc.GetPortToolActualFlow(UUID, (Port)p);
					ret.TimeToStop = DLLConnection.jbc.GetPortToolTimeToStopStatus(UUID, (Port)p);

					ret.Stand_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolStandStatus(UUID, (Port)p));
					ret.PedalConnected_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolPedalConnectedStatus(UUID, (Port)p));
					ret.PedalStatus_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolPedalStatus(UUID, (Port)p));
					ret.HeaterRequestedStatus_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolHeaterRequestedStatus(UUID, (Port)p));
					ret.HeaterStatus_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolHeaterStatus(UUID, (Port)p));
					ret.SuctionRequestedStatus_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSuctionRequestedStatus(UUID, (Port)p));
					ret.SuctionStatus_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSuctionStatus(UUID, (Port)p));
					ret.CoolingStatus_OnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolCoolingStatus(UUID, (Port)p));

					ret.PortSelectedTemp = convTempToStruc(DLLConnection.jbc.GetPortToolSelectedTemp(UUID, (Port)p));
					ret.PortSelectedExtTemp = convTempToStruc(DLLConnection.jbc.GetPortToolSelectedExternalTemp(UUID, (Port)p));
					ret.PortSelectedFlow_x_mil = DLLConnection.jbc.GetPortToolSelectedFlow(UUID, (Port)p);
					ret.ToolError = (dc_EnumConstJBC.dc_ToolError)(DLLConnection.jbc.GetPortToolError(UUID, (Port)p));
					ret.EnabledPort = (JBC_Connect.dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetEnabledPort(UUID, (Port) p));

					if (ret.ConnectedTool != dc_EnumConstJBC.dc_GenericStationTools.NOTOOL)
					{
						ret.This_PortToolSettings.portNbr = p;
						ret.This_PortToolSettings.Tool = ret.ConnectedTool;
						ret.This_PortToolSettings.PortProfileMode = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolProfileMode(UUID, (Port)p));

						// levels
						ret.This_PortToolSettings.Levels.LevelsCount = 3;
						ret.This_PortToolSettings.Levels.LevelsOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSelectedTempLevelsEnabled(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						ret.This_PortToolSettings.Levels.LevelsTempSelect = (byte)DLLConnection.jbc.GetPortToolSelectedTempLevels(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool);
						ret.This_PortToolSettings.Levels.LevelsTempOnOff = new dc_EnumConstJBC.dc_OnOff[ret.This_PortToolSettings.Levels.LevelsCount - 1 + 1];
						ret.This_PortToolSettings.Levels.LevelsTemp = new dc_getTemperature[ret.This_PortToolSettings.Levels.LevelsCount - 1 + 1];
						ret.This_PortToolSettings.Levels.LevelsFlow = new int[ret.This_PortToolSettings.Levels.LevelsCount - 1 + 1];
						ret.This_PortToolSettings.Levels.LevelsExtTemp = new dc_getTemperature[ret.This_PortToolSettings.Levels.LevelsCount - 1 + 1];
						for (var i = 0; i <= ret.This_PortToolSettings.Levels.LevelsCount - 1; i++)
						{
							ret.This_PortToolSettings.Levels.LevelsTempOnOff[(int)i] = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolTempLevelEnabled(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool, (ToolTemperatureLevels)i));
							ret.This_PortToolSettings.Levels.LevelsTemp[(int)i] = convTempToStruc(DLLConnection.jbc.GetPortToolTempLevel(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool, (ToolTemperatureLevels)i));
							ret.This_PortToolSettings.Levels.LevelsExtTemp[(int)i] = convTempToStruc(DLLConnection.jbc.GetPortToolExternalTempLevel(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool, (ToolTemperatureLevels)i));
							ret.This_PortToolSettings.Levels.LevelsFlow[(int)i] = DLLConnection.jbc.GetPortToolFlowLevel(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool, (ToolTemperatureLevels)i);
						}

						ret.This_PortToolSettings.AdjustTemp = convAdjustTempToStruc(DLLConnection.jbc.GetPortToolAdjustTemp(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						ret.This_PortToolSettings.TimeToStop = DLLConnection.jbc.GetPortToolTimeToStop(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool);
						ret.This_PortToolSettings.ExternalTCMode = (dc_EnumConstJBC.dc_ExternalTCMode_HA)(DLLConnection.jbc.GetPortToolExternalTCMode(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool));
						CToolStartMode_HA startMode = DLLConnection.jbc.GetPortToolStartMode(UUID, (Port)p, (GenericStationTools)ret.ConnectedTool);
						ret.This_PortToolSettings.StartMode_ToolButton = (dc_EnumConstJBC.dc_OnOff)startMode.ToolButton;
						ret.This_PortToolSettings.StartMode_Pedal = (dc_EnumConstJBC.dc_PedalAction)startMode.Pedal;
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		// Soldering Feeder Station only
		public dc_StatusTool_SF GetPortStatus_SF(string UUID, JBC_Connect.dc_EnumConstJBC.dc_Port p)
		{
			dc_StatusTool_SF ret = new dc_StatusTool_SF();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret.portNbr = p;
					ret.EnabledPort = (dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetEnabledPort(UUID, (Port) p));
					ret.DispenserMode = (dc_EnumConstJBC.dc_DispenserMode_SF) (DLLConnection.jbc.GetPortDispenserMode(UUID, (Port) p));
					ret.Speed = convSpeedToStruc(DLLConnection.jbc.GetSpeed(UUID, (Port) p));
					ret.Length = convLengthToStruc(DLLConnection.jbc.GetLength(UUID, (Port) p));
					ret.FeedingState = (JBC_Connect.dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetFeedingState(UUID, (Port) p));
					ret.FeedingPercent = System.Convert.ToUInt16(DLLConnection.jbc.GetFeedingPercent(UUID, (Port) p));
					ret.FeedingLength = convLengthToStruc(DLLConnection.jbc.GetFeedingLength(UUID, (Port) p));
					ret.CurrentProgramStep = System.Convert.ToByte(DLLConnection.jbc.CurrentProgramStep(UUID, (Port) p));
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		// set

		// Soldering Station only
		public void SetPortStatusTool(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_OnOff stand, dc_EnumConstJBC.dc_OnOff extractor,
				dc_EnumConstJBC.dc_OnOff desolder)
		{
			Console.WriteLine(" SetPortStatusTool: "+ UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolStandStatus(UUID, (Port)p, (OnOff)stand);
					DLLConnection.jbc.SetPortToolExtractorStatus(UUID, (Port)p, (OnOff)extractor);
					DLLConnection.jbc.SetPortToolDesolderStatus(UUID, (Port)p, (OnOff)desolder);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air Desoldering Station only
		public void SetPortStatusTool_HA(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_OnOff heater,
				dc_EnumConstJBC.dc_OnOff suction)
		{
			Console.WriteLine(" SetPortStatusTool_HA: "+UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolHeaterStatus(UUID, (Port)p, (OnOff)heater);
					DLLConnection.jbc.SetPortToolSuctionStatus(UUID, (Port)p, (OnOff)suction);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

				public bool SetEnabledPort(string UUID, dc_EnumConstJBC.dc_Port p, JBC_Connect.dc_EnumConstJBC.dc_OnOff enabled)
		{
			bool bOk = false;

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					m_TraceData.StopTraceData(UUID, (Port) p);
					DLLConnection.jbc.SetEnabledPort(UUID, (Port) p, (OnOff) enabled);

					//stop record data
					if (enabled == JBC_Connect.dc_EnumConstJBC.dc_OnOff._OFF)
					{
						StopRecordData(UUID, p);
					}
					bOk = true;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

#endregion

#region Station Settings :fet:
		// get

		// Soldering stations only
		public dc_Station_Sold_Settings GetStationSettings(string UUID)
		{
			dc_Station_Sold_Settings ret = new dc_Station_Sold_Settings();
			Console.WriteLine(" GetStationSettings: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret.Name = DLLConnection.jbc.GetStationName(UUID);
					ret.PIN = DLLConnection.jbc.GetStationPIN(UUID);
					ret.Beep = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetStationBeep(UUID));
					ret.StationLocked = (JBC_Connect.dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetStationLocked(UUID));
					ret.MaxTemp = convTempToStruc(DLLConnection.jbc.GetStationMaxTemp(UUID));
					ret.MinTemp = convTempToStruc(DLLConnection.jbc.GetStationMinTemp(UUID));
					if (DLLConnection.jbc.GetStationTempUnits(UUID) == CTemperature.TemperatureUnit.Fahrenheit)
					{
						ret.Unit = "F";
					}
					else
					{
						ret.Unit = "C";
					}
					ret.N2Mode = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetStationN2Mode(UUID));
					ret.HelpText = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetStationHelpText(UUID));
					ret.PowerLimit = DLLConnection.jbc.GetStationPowerLimit(UUID);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
			Console.WriteLine("   Settings: "+ret.Name);
			return ret;
		}

		// Hot Air Desoldering stations only
		public dc_Station_HA_Settings GetStationSettings_HA(string UUID)
		{
			dc_Station_HA_Settings ret = new dc_Station_HA_Settings();
			Console.WriteLine(" GetStationSettings_HA: "+ UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret.Name = DLLConnection.jbc.GetStationName(UUID);
					ret.PIN = DLLConnection.jbc.GetStationPIN(UUID);
					ret.Beep = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetStationBeep(UUID));
					ret.MaxTemp = convTempToStruc(DLLConnection.jbc.GetStationMaxTemp(UUID));
					ret.MinTemp = convTempToStruc(DLLConnection.jbc.GetStationMinTemp(UUID));
					ret.MaxExtTemp = convTempToStruc(DLLConnection.jbc.GetStationMaxExternalTemp(UUID));
					ret.MinExtTemp = convTempToStruc(DLLConnection.jbc.GetStationMinExternalTemp(UUID));
					ret.MaxFlow = DLLConnection.jbc.GetStationMaxFlow(UUID);
					ret.MinFlow = DLLConnection.jbc.GetStationMinFlow(UUID);
					if (DLLConnection.jbc.GetStationTempUnits(UUID) == CTemperature.TemperatureUnit.Fahrenheit)
					{
						ret.Unit = "F";
					}
					else
					{
						ret.Unit = "C";
					}
					ret.PINEnabled = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetStationPINEnabled(UUID));
					ret.StationLocked = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetStationLocked(UUID));
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
			Console.WriteLine("   Settings: "+ ret.Name);
			return ret;
		}

		public dc_Station_SF_Settings GetStationSettings_SF(string UUID)
		{
			dc_Station_SF_Settings ret = new dc_Station_SF_Settings();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret.Name = DLLConnection.jbc.GetStationName(UUID);
					ret.Beep = (JBC_Connect.dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetStationBeep(UUID));
					ret.PINEnabled = (JBC_Connect.dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetStationPINEnabled(UUID));
					ret.PIN = DLLConnection.jbc.GetStationPIN(UUID);
					ret.LengthUnit = (dc_EnumConstJBC.dc_LengthUnit) (DLLConnection.jbc.GetStationLengthUnits(UUID));
					ret.StationLocked = (dc_EnumConstJBC.dc_OnOff) (DLLConnection.jbc.GetStationLocked(UUID));

					//Programs
					ret.SelectedProgram = System.Convert.ToByte(DLLConnection.jbc.GetStationSelectedProgram(UUID));
					ret.Programs = new dc_ProgramDispenserData_SF[35];
					for (byte i = 1; i <= 35; i++)
					{
						CProgramDispenserData_SF program = DLLConnection.jbc.GetStationProgram(UUID, i);
						ret.Programs[i - 1] = new dc_ProgramDispenserData_SF();
						ret.Programs[i - 1].Enabled = (JBC_Connect.dc_EnumConstJBC.dc_OnOff) program.Enabled;
						ret.Programs[i - 1].Name = System.Convert.ToString(program.Name);
						ret.Programs[i - 1].Length_1 = System.Convert.ToUInt16(program.Length_1);
						ret.Programs[i - 1].Speed_1 = System.Convert.ToUInt16(program.Speed_1);
						ret.Programs[i - 1].Length_2 = System.Convert.ToUInt16(program.Length_2);
						ret.Programs[i - 1].Speed_2 = System.Convert.ToUInt16(program.Speed_2);
						ret.Programs[i - 1].Length_3 = System.Convert.ToUInt16(program.Length_3);
						ret.Programs[i - 1].Speed_3 = System.Convert.ToUInt16(program.Speed_3);
					}
					ret.ConcatenateProgramList = DLLConnection.jbc.GetStationConcatenateProgramList(UUID);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		public string GetStationPIN(string UUID)
		{
			Console.WriteLine(" GetStationPIN: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					return DLLConnection.jbc.GetStationPIN(UUID);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return "";
		}

		// set

		public void SetStationName(string UUID, string newName)
		{
			Console.WriteLine(" SetStationName: "+ UUID + " " + newName);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationName(UUID, newName);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationPIN(string UUID, string newPIN)
		{
			Console.WriteLine(" SetStationPIN: " + UUID + " " + newPIN);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationPIN(UUID, newPIN);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationTempUnit(string UUID, string newTempUnit)
		{
			Console.WriteLine(" SetStationTempUnit: " + UUID + " " + newTempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					if (newTempUnit == "F")
					{
						DLLConnection.jbc.SetStationTempUnits(UUID, CTemperature.TemperatureUnit.Fahrenheit);
					}
					else
					{
						DLLConnection.jbc.SetStationTempUnits(UUID, CTemperature.TemperatureUnit.Celsius);
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationLengthUnit(string UUID, dc_EnumConstJBC.dc_LengthUnit newLengthUnit)
		{
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationLengthUnits(UUID, (CLength.LengthUnit) newLengthUnit);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationMinTemp(string UUID, int temp, string tempUnit)
		{
			Console.WriteLine(" SetStationMinTemp: " + UUID +" "+ temp +" " + tempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newTemp = new CTemperature(0);
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetStationMinTemp(UUID, newTemp);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationMaxTemp(string UUID, int temp, string tempUnit)
		{
			Console.WriteLine(" SetStationMaxTemp: " + UUID + " " + temp + " " + tempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newTemp = new CTemperature(0);
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetStationMaxTemp(UUID, newTemp);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetStationMinExtTemp(string UUID, int temp, string tempUnit)
		{
			Console.WriteLine(" SetStationMinExtTemp: " + UUID + " " + temp + " " + tempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newTemp = new CTemperature(0);
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetStationMinExternalTemp(UUID, newTemp);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetStationMaxExtTemp(string UUID, int temp, string tempUnit)
		{
			Console.WriteLine(" SetStationMaxExtTemp: " + UUID + " " + temp + " " + tempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newTemp = new CTemperature(0);
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetStationMaxExternalTemp(UUID, newTemp);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetStationMinFlow(string UUID, int newPowerLimit)
		{
			Console.WriteLine(" SetStationMinFlow: " + UUID + " " + newPowerLimit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationMinFlow(UUID, newPowerLimit);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetStationMaxFlow(string UUID, int newPowerLimit)
		{
			Console.WriteLine(" SetStationMaxFlow: " + UUID + " " + newPowerLimit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationMaxFlow(UUID, newPowerLimit);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Soldering stations only
		public void SetStationN2Mode(string UUID, dc_EnumConstJBC.dc_OnOff onoff)
		{
			Console.WriteLine(" SetStationN2Mode: " + UUID + " " + onoff);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationN2Mode(UUID, (OnOff)onoff);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Soldering stations only
		public void SetStationHelpText(string UUID, dc_EnumConstJBC.dc_OnOff onoff)
		{
			Console.WriteLine(" SetStationHelpText: " + UUID + " " + onoff);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationHelpText(UUID, (OnOff)onoff);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationBeep(string UUID, dc_EnumConstJBC.dc_OnOff onoff)
		{
			Console.WriteLine(" SetStationBeep: " + UUID + " " + onoff);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationBeep(UUID, (OnOff)onoff);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public bool SetStationLocked(string UUID, dc_EnumConstJBC.dc_OnOff locked, string message, uint timeout, bool dataEntry)
		{
			bool bOk = false;

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					m_TraceData.StopTraceData(UUID);
					DLLConnection.jbc.SetStationLocked(UUID, (OnOff) locked);
					//FIXME: Remove this lines. Only test
					if (locked == JBC_Connect.dc_EnumConstJBC.dc_OnOff._ON)
					{
						m_TraceData.StopTraceData(UUID);
					}
					//END FIXME
					bOk = true;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

		// Soldering stations only
		public void SetStationPowerLimit(string UUID, int newPowerLimit)
		{
			Console.WriteLine(" SetStationPwerLimir: " + UUID + " " + newPowerLimit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationPowerLimit(UUID, newPowerLimit);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air Desoldering stations only
		public void SetStationPINEnabled(string UUID, dc_EnumConstJBC.dc_OnOff onoff)
		{
			Console.WriteLine(" SetStationPINEnabled: " + UUID + " " + onoff);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationPINEnabled(UUID, (OnOff)onoff);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationConcatenateProgramList(string UUID, byte[] programList)
		{
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetStationConcatenateProgramList(UUID, programList);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetStationProgram(string UUID, byte nbrProgram, dc_ProgramDispenserData_SF programDC)
		{
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CProgramDispenserData_SF programData = new CProgramDispenserData_SF();
					programData.Enabled = (OnOff) programDC.Enabled;
					programData.Name = programDC.Name;
					programData.Length_1 = programDC.Length_1;
					programData.Speed_1 = programDC.Speed_1;
					programData.Length_2 = programDC.Length_2;
					programData.Speed_2 = programDC.Speed_2;
					programData.Length_3 = programDC.Length_3;
					programData.Speed_3 = programDC.Speed_3;
					DLLConnection.jbc.SetStationProgram(UUID, nbrProgram, programData);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void DeleteStationProgram(string UUID, byte nbrProgram)
		{
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.DeleteStationProgram(UUID, nbrProgram);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

#endregion

#region Port/Tool Settings :fet:

		// get

		// soldering stations only
		public dc_PortToolSettings GetPortToolSettings(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool)
		{
			dc_PortToolSettings ret = new dc_PortToolSettings();
			Console.WriteLine(" GetPortToolSettings: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
					// check port and tool
				}
				else if (tool == dc_EnumConstJBC.dc_GenericStationTools.NOTOOL)
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.NotValidTool, My.Resources.Resources.errNotValidTool,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
					// FALTAN MAS VALIDACIONES
				}
				else
				{
					ret.portNbr = p;
					ret.AdjustTemp = convAdjustTempToStruc(DLLConnection.jbc.GetPortToolAdjustTemp(UUID, (Port)p, (GenericStationTools)tool));

					// cartridge
					CCartridgeData cartridge = DLLConnection.jbc.GetPortToolCartridge(UUID, (Port)p, (GenericStationTools)tool);
					if (cartridge != null)
					{
						ret.Cartridge.CartridgeNbr = cartridge.CartridgeNbr;
						ret.Cartridge.CartridgeOnOff = (dc_EnumConstJBC.dc_OnOff)cartridge.CartridgeOnOff;
					}

					// fixed
					CTemperature temp = DLLConnection.jbc.GetPortToolFixTemp(UUID, (Port)p, (GenericStationTools)tool);
					if (temp.UTI == Constants.NO_FIXED_TEMP | temp.UTI == 0)
					{
						ret.FixedTemp_OnOff = dc_EnumConstJBC.dc_OnOff._OFF;
					}
					else
					{
						ret.FixedTemp_OnOff = dc_EnumConstJBC.dc_OnOff._ON;
					}
					ret.FixedTemp = convTempToStruc(temp);

					ret.HiberTime = (dc_EnumConstJBC.dc_TimeHibernation)(DLLConnection.jbc.GetPortToolHibernationDelay(UUID, (Port)p, (GenericStationTools)tool));
					ret.HiberTimeOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolHibernationDelayEnabled(UUID, (Port)p, (GenericStationTools)tool));

					// levels
					ret.Levels.LevelsCount = 3;
					ret.Levels.LevelsOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSelectedTempLevelsEnabled(UUID, (Port)p, (GenericStationTools)tool));
					ret.Levels.LevelsTempSelect = (byte)DLLConnection.jbc.GetPortToolSelectedTempLevels(UUID, (Port)p, (GenericStationTools)tool);
					ret.Levels.LevelsTempOnOff = new dc_EnumConstJBC.dc_OnOff[ret.Levels.LevelsCount - 1 + 1];
					ret.Levels.LevelsTemp = new dc_getTemperature[ret.Levels.LevelsCount - 1 + 1];
					for (var i = 0; i <= ret.Levels.LevelsCount - 1; i++)
					{
						ret.Levels.LevelsTempOnOff[(int)i] = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolTempLevelEnabled(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)i));
						ret.Levels.LevelsTemp[(int)i] = convTempToStruc(DLLConnection.jbc.GetPortToolTempLevel(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)i));
					}

					ret.SleepTemp = convTempToStruc(DLLConnection.jbc.GetPortToolSleepTemp(UUID, (Port)p, (GenericStationTools)tool), true);
					ret.SleepTime = (dc_EnumConstJBC.dc_TimeSleep)(DLLConnection.jbc.GetPortToolSleepDelay(UUID, (Port)p, (GenericStationTools)tool));
					ret.SleepTimeOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSleepDelayEnabled(UUID, (Port)p, (GenericStationTools)tool));

					// settings for port / tool
					ret.Tool = tool;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		// Hot Air desoldering stations only
		public dc_PortToolSettings_HA GetPortToolSettings_HA(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool)
		{
			dc_PortToolSettings_HA ret = new dc_PortToolSettings_HA();
			Console.WriteLine(" GetPortToolSettings_HA: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
					// check port and tool
				}
				else if (tool == dc_EnumConstJBC.dc_GenericStationTools.NOTOOL)
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.NotValidTool, My.Resources.Resources.errNotValidTool,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
					// FALTAN MAS VALIDACIONES
				}
				else
				{
					ret.portNbr = p;
					ret.AdjustTemp = convAdjustTempToStruc(DLLConnection.jbc.GetPortToolAdjustTemp(UUID, (Port)p, (GenericStationTools)tool));
					ret.TimeToStop = DLLConnection.jbc.GetPortToolTimeToStop(UUID, (Port)p, (GenericStationTools)tool);
					ret.ExternalTCMode = (dc_EnumConstJBC.dc_ExternalTCMode_HA)(DLLConnection.jbc.GetPortToolExternalTCMode(UUID, (Port)p, (GenericStationTools)tool));
					CToolStartMode_HA startmode = DLLConnection.jbc.GetPortToolStartMode(UUID, (Port)p, (GenericStationTools)tool);
					ret.StartMode_ToolButton = (dc_EnumConstJBC.dc_OnOff)startmode.ToolButton;
					ret.StartMode_Pedal = (dc_EnumConstJBC.dc_PedalAction)startmode.Pedal;

					// levels
					ret.Levels.LevelsCount = 3;
					ret.Levels.LevelsOnOff = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolSelectedTempLevelsEnabled(UUID, (Port)p, (GenericStationTools)tool));
					ret.Levels.LevelsTempSelect = (byte)DLLConnection.jbc.GetPortToolSelectedTempLevels(UUID, (Port)p, (GenericStationTools)tool);
					ret.Levels.LevelsTempOnOff = new dc_EnumConstJBC.dc_OnOff[ret.Levels.LevelsCount - 1 + 1];
					ret.Levels.LevelsTemp = new dc_getTemperature[ret.Levels.LevelsCount - 1 + 1];
					ret.Levels.LevelsExtTemp = new dc_getTemperature[ret.Levels.LevelsCount - 1 + 1];
					ret.Levels.LevelsFlow = new int[ret.Levels.LevelsCount - 1 + 1];
					for (var i = 0; i <= ret.Levels.LevelsCount - 1; i++)
					{
						ret.Levels.LevelsTempOnOff[(int)i] = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolTempLevelEnabled(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)i));
						ret.Levels.LevelsTemp[(int)i] = convTempToStruc(DLLConnection.jbc.GetPortToolTempLevel(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)i));
						ret.Levels.LevelsExtTemp[(int)i] = convTempToStruc(DLLConnection.jbc.GetPortToolExternalTempLevel(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)i));
						ret.Levels.LevelsFlow[(int)i] = DLLConnection.jbc.GetPortToolFlowLevel(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)i);
					}

					ret.PortProfileMode = (dc_EnumConstJBC.dc_OnOff)(DLLConnection.jbc.GetPortToolProfileMode(UUID, (Port)p));

					// settings for port / tool
					ret.Tool = tool;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		// set

		public void SetPortToolSelectedTemp(string UUID, dc_EnumConstJBC.dc_Port p, int temp, string tempUnit)
		{
			CTemperature newTemp = new CTemperature(0);
			Console.WriteLine(" SetPortToolSelectedTemp: " + UUID + " " + temp);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetPortToolSelectedTemp(UUID, (Port)p, newTemp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortToolSelectedTempStep(string UUID, dc_EnumConstJBC.dc_Port p, int iStep, string tempUnit)
		{
			// iStep may be positive or negative (for ex: 1, -1, 2, -2, etc)
			Console.WriteLine(" SetPortToolSelectedTempStep: " + UUID + " " + iStep + " " + tempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					var temp = DLLConnection.jbc.GetPortToolSelectedTemp(UUID, (Port)p);
					int aux = 0;
					if (tempUnit == "C")
					{
						aux = temp.ToRoundCelsius();
						temp.InCelsius(aux + (iStep * 5));
					}
					else if (tempUnit == "F")
					{
						aux = temp.ToRoundFahrenheit();
						temp.InFahrenheit(aux + (iStep * 10));
					}
					else
					{
						aux = temp.ToRoundCelsius();
						temp.InCelsius(aux + (iStep * 5));
					}
					DLLConnection.jbc.SetPortToolSelectedTemp(UUID, (Port)p, temp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolSelectedExtTemp(string UUID, dc_EnumConstJBC.dc_Port p, int temp, string tempUnit)
		{
			CTemperature newTemp = new CTemperature(0);
			Console.WriteLine(" SetPortToolSelectedExtTemp: " + UUID + " " + temp + " " + tempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetPortToolSelectedExternalTemp(UUID, (Port)p, newTemp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolSelectedExtTempStep(string UUID, dc_EnumConstJBC.dc_Port p, int iStep, string tempUnit)
		{
			Console.WriteLine(" SetPortToolSelectedExtTempStep: " + UUID + " " + iStep + " " + tempUnit);
			// iStep may be positive or negative (for ex: 1, -1, 2, -2, etc)
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					var temp = DLLConnection.jbc.GetPortToolSelectedExternalTemp(UUID, (Port)p);
					int aux = 0;
					if (tempUnit == "C")
					{
						aux = temp.ToRoundCelsius();
						temp.InCelsius(aux + (iStep * 5));
					}
					else if (tempUnit == "F")
					{
						aux = temp.ToRoundFahrenheit();
						temp.InFahrenheit(aux + (iStep * 10));
					}
					else
					{
						aux = temp.ToRoundCelsius();
						temp.InCelsius(aux + (iStep * 5));
					}
					DLLConnection.jbc.SetPortToolSelectedExternalTemp(UUID, (Port)p, temp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolSelectedFlow(string UUID, dc_EnumConstJBC.dc_Port p, int flow)
		{
			Console.WriteLine(" SetPortToolSelectedFlow: " + UUID + " " + flow);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolSelectedFlow(UUID, (Port)p, flow);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolFixTemp(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool,
				int temp, string tempUnit)
		{
			CTemperature newTemp = new CTemperature(0);
			Console.WriteLine(" SetPortToolFixTemp: " + UUID + " " + temp + " " + tempUnit);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetPortToolFixTemp(UUID, (Port)p, (GenericStationTools)tool, newTemp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolLevels(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff LevelsOnOff, dc_EnumConstJBC.dc_ToolTemperatureLevels LevelSelected, dc_EnumConstJBC.dc_OnOff Level1OnOff, int Level1Temp, dc_EnumConstJBC.dc_OnOff Level2OnOff, int Level2Temp, dc_EnumConstJBC.dc_OnOff Level3OnOff, int Level3Temp, string tempUnit)
		{
			Console.WriteLine(" SetPortToolLevels: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newLevel1Temp = new CTemperature(0);
					CTemperature newLevel2Temp = new CTemperature(0);
					CTemperature newLevel3Temp = new CTemperature(0);

					if (tempUnit == "F")
					{
						newLevel1Temp.InFahrenheit(Level1Temp);
						newLevel2Temp.InFahrenheit(Level2Temp);
						newLevel3Temp.InFahrenheit(Level3Temp);
					}
					else if (tempUnit == "C")
					{
						newLevel1Temp.InCelsius(Level1Temp);
						newLevel2Temp.InCelsius(Level2Temp);
						newLevel3Temp.InCelsius(Level3Temp);
					}
					else
					{
						newLevel1Temp.UTI = Level1Temp;
						newLevel2Temp.UTI = Level2Temp;
						newLevel3Temp.UTI = Level3Temp;
					}
					DLLConnection.jbc.SetPortToolLevels_SOLD(UUID, (Port)p, (GenericStationTools)tool, (OnOff)LevelsOnOff, (ToolTemperatureLevels)LevelSelected, (OnOff)Level1OnOff, newLevel1Temp, (OnOff)Level2OnOff, newLevel2Temp, (OnOff)Level3OnOff, newLevel3Temp);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolLevels_HA(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff LevelsOnOff, dc_EnumConstJBC.dc_ToolTemperatureLevels LevelSelected, dc_EnumConstJBC.dc_OnOff Level1OnOff, int Level1Temp, int Level1Flow, int Level1ExtTemp, dc_EnumConstJBC.dc_OnOff Level2OnOff, int Level2Temp, int Level2Flow, int Level2ExtTemp, dc_EnumConstJBC.dc_OnOff Level3OnOff, int Level3Temp, int Level3Flow, int Level3ExtTemp, string tempUnit)
		{
			Console.WriteLine(" SetPortToolsLeves_HA: " + UUID );
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newLevel1Temp = new CTemperature(0);
					CTemperature newLevel2Temp = new CTemperature(0);
					CTemperature newLevel3Temp = new CTemperature(0);
					CTemperature newLevel1ExtTemp = new CTemperature(0);
					CTemperature newLevel2ExtTemp = new CTemperature(0);
					CTemperature newLevel3ExtTemp = new CTemperature(0);

					if (tempUnit == "F")
					{
						newLevel1Temp.InFahrenheit(Level1Temp);
						newLevel2Temp.InFahrenheit(Level2Temp);
						newLevel3Temp.InFahrenheit(Level3Temp);
						newLevel1ExtTemp.InFahrenheit(Level1ExtTemp);
						newLevel2ExtTemp.InFahrenheit(Level2ExtTemp);
						newLevel3ExtTemp.InFahrenheit(Level3ExtTemp);
					}
					else if (tempUnit == "C")
					{
						newLevel1Temp.InCelsius(Level1Temp);
						newLevel2Temp.InCelsius(Level2Temp);
						newLevel3Temp.InCelsius(Level3Temp);
						newLevel1ExtTemp.InCelsius(Level1ExtTemp);
						newLevel2ExtTemp.InCelsius(Level2ExtTemp);
						newLevel3ExtTemp.InCelsius(Level3ExtTemp);
					}
					else
					{
						newLevel1Temp.UTI = Level1Temp;
						newLevel2Temp.UTI = Level2Temp;
						newLevel3Temp.UTI = Level3Temp;
						newLevel1ExtTemp.UTI = Level1ExtTemp;
						newLevel2ExtTemp.UTI = Level2ExtTemp;
						newLevel3ExtTemp.UTI = Level3ExtTemp;
					}
					DLLConnection.jbc.SetPortToolLevels_HA(UUID, (Port)p, (GenericStationTools)tool, (OnOff)LevelsOnOff, (ToolTemperatureLevels)LevelSelected, (OnOff)Level1OnOff, newLevel1Temp, Level1Flow, newLevel1ExtTemp, (OnOff)Level2OnOff, newLevel2Temp, Level2Flow, newLevel2ExtTemp, (OnOff)Level3OnOff, newLevel3Temp, Level3Flow, newLevel3ExtTemp);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortToolSelectedLevelEnabled(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff LevelsOnOff)
		{
			Console.WriteLine(" SetPortToolSelectedLevelEnabled: " + UUID );
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolSelectedTempLevelsEnabled(UUID, (Port)p, (GenericStationTools)tool, (OnOff)LevelsOnOff);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortToolSelectedLevel(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels LevelSelected)
		{
			Console.WriteLine(" SetPortToolSelectedLevel: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolSelectedTempLevels(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)LevelSelected);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortToolTempLevelEnabled(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, dc_EnumConstJBC.dc_OnOff OnOff)
		{
			Console.WriteLine(" SetPortToolTempLevelEnabled: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolTempLevelEnabled(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)Level, (OnOff)OnOff);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortToolTempLevel(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, int LevelTemp, string tempUnit)
		{
			Console.WriteLine(" SetPortToolTempLevel: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newTemp = new CTemperature(0);
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(LevelTemp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(LevelTemp);
					}
					else
					{
						newTemp.UTI = LevelTemp;
					}
					DLLConnection.jbc.SetPortToolTempLevel(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)Level, newTemp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolExtTempLevel(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, int LevelTemp, string tempUnit)
		{
			Console.WriteLine(" SetPortToolExtTempLevel: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CTemperature newTemp = new CTemperature(0);
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(LevelTemp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(LevelTemp);
					}
					else
					{
						newTemp.UTI = LevelTemp;
					}
					DLLConnection.jbc.SetPortToolExternalTempLevel(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)Level, newTemp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolFlowLevel(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, int LevelFlow)
		{
			Console.WriteLine(" SetPortToolFlowLevel: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolFlowLevel(UUID, (Port)p, (GenericStationTools)tool, (ToolTemperatureLevels)Level, LevelFlow);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolProfileMode(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_OnOff onoff)
		{
			Console.WriteLine(" SetPortToolProfileMode: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolProfileMode(UUID, (Port)p, (OnOff)onoff);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolSleepDelay(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_TimeSleep delay)
		{
			Console.WriteLine(" SetPortToolSleepDelay: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolSleepDelay(UUID, (Port)p, (GenericStationTools)tool, (ToolTimeSleep)delay);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolSleepDelayEnabled(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff OnOff)
		{
			Console.WriteLine(" SetPortToolSleepDelayEnabled: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolSleepDelayEnabled(UUID, (Port)p, (GenericStationTools)tool, (OnOff)OnOff);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolHibernationDelay(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_TimeHibernation delay)
		{
			Console.WriteLine(" SetPortToolHibernationDelay: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolHibernationDelay(UUID, (Port)p, (GenericStationTools)tool, (ToolTimeHibernation)delay);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolHibernationDelayEnabled(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff OnOff)
		{
			Console.WriteLine(" SetPortToolHibernationDelayEnabled: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolHibernationDelayEnabled(UUID, (Port)p, (GenericStationTools)tool, (OnOff)OnOff);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolSleepTemp(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, int temp, string tempUnit)
		{
			Console.WriteLine(" SetPortToolSleepTemp: " + UUID);
			CTemperature newTemp = new CTemperature(0);

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					if (tempUnit == "F")
					{
						newTemp.InFahrenheit(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsius(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetPortToolSleepTemp(UUID, (Port)p, (GenericStationTools)tool, newTemp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortToolAdjustTemp(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, int temp, string tempUnit)
		{
			CTemperature newTemp = new CTemperature(0);
			Console.WriteLine(" SetPortToolAdjustTemp: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					if (tempUnit == "F")
					{
						newTemp.InFahrenheitToAdjust(temp);
					}
					else if (tempUnit == "C")
					{
						newTemp.InCelsiusToAdjust(temp);
					}
					else
					{
						newTemp.UTI = temp;
					}
					DLLConnection.jbc.SetPortToolAdjustTemp(UUID, (Port)p, (GenericStationTools)tool, newTemp);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// soldering stations only
		public void SetPortToolCartridge(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_Cartridge cartridge)
		{
			Console.WriteLine(" SetPortToolCartridge: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CCartridgeData cartridgeData = new CCartridgeData();
					cartridgeData.CartridgeNbr = cartridge.CartridgeNbr;
					cartridgeData.CartridgeOnOff = (OnOff)cartridge.CartridgeOnOff;
					cartridgeData.CartridgeFamily = cartridgeData.CartridgeFamily;
					DLLConnection.jbc.SetPortToolCartridge(UUID, (Port)p, (GenericStationTools)tool, cartridgeData);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolStartMode(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff toolButton, dc_EnumConstJBC.dc_PedalAction pedalAction)
		{
			Console.WriteLine(" SetPortToolStartMode: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CToolStartMode_HA startmode = new CToolStartMode_HA();
					startmode.ToolButton = (OnOff)toolButton;
					startmode.Pedal = (PedalAction)pedalAction;
					DLLConnection.jbc.SetPortToolStartMode(UUID, (Port)p, (GenericStationTools)tool, startmode);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolTimeToStop(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, int timetostop)
		{
			Console.WriteLine(" SetPortToolTimeToStop: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolTimeToStop(UUID, (Port)p, (GenericStationTools)tool, timetostop);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		// Hot Air desoldering stations only
		public void SetPortToolExternalTCMode(string UUID, dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ExternalTCMode_HA mode)
		{
			Console.WriteLine(" SetPortToolExternalTCMode: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortToolExternalTCMode(UUID, (Port)p, (GenericStationTools)tool, (ToolExternalTCMode_HA)mode);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		
		public void SetPortDispenserMode(string UUID, JBC_Connect.dc_EnumConstJBC.dc_Port p, dc_EnumConstJBC.dc_DispenserMode_SF mode, byte nbrProgram)
		{
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetPortDispenserMode(UUID, (Port) p, (DispenserMode_SF) mode, nbrProgram);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortLength(string UUID, dc_EnumConstJBC.dc_Port p, double lengthValue, dc_EnumConstJBC.dc_LengthUnit lengthUnit)
		{
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CLength length = new CLength();
					if (lengthUnit == dc_EnumConstJBC.dc_LengthUnit.INCHES)
					{
						length.InInches(lengthValue);
					}
					else
					{
						length.InMillimeters(lengthValue);
					}
					DLLConnection.jbc.SetLength(UUID, (Port) p, length);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetPortSpeed(string UUID, JBC_Connect.dc_EnumConstJBC.dc_Port p, double speedValue, dc_EnumConstJBC.dc_SpeedUnit speedUnit)
		{
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SF))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CSpeed speed = new CSpeed();
					if (speedUnit == dc_EnumConstJBC.dc_SpeedUnit.INCHES_PER_SECOND)
					{
						speed.InInchesPerSecond(speedValue);
					}
					else
					{
						speed.InMillimetersPerSecond(speedValue);
					}
					DLLConnection.jbc.SetSpeed(UUID, (Port) p, speed);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

#endregion

#region Counters :fet:

		// get

		public dc_Port_Counters GetPortCounters(string UUID, dc_EnumConstJBC.dc_Port p)
		{
			dc_Port_Counters ret = new dc_Port_Counters();
			Console.WriteLine(" GetPortCounters: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CounterTypes counttype = default(CounterTypes);
					ret.portNbr = p;

					counttype = CounterTypes.GLOBAL_COUNTER;
					ret.GlobalCounters.ContPlugMinutes = DLLConnection.jbc.GetPortToolPluggedMinutes(UUID, (Port)p, counttype);
					ret.GlobalCounters.ContWorkMinutes = DLLConnection.jbc.GetPortToolWorkMinutes(UUID, (Port)p, counttype);
					ret.GlobalCounters.ContIdleMinutes = DLLConnection.jbc.GetPortToolIdleMinutes(UUID, (Port)p, counttype);
					ret.GlobalCounters.ContSleepMinutes = DLLConnection.jbc.GetPortToolSleepMinutes(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContHiberMinutes = DLLConnection.jbc.GetPortToolHibernationMinutes(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContSleepCycles = DLLConnection.jbc.GetPortToolSleepCycles(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContDesoldCycles = DLLConnection.jbc.GetPortToolDesolderCycles(UUID, (Port) p, counttype);

					counttype = CounterTypes.PARTIAL_COUNTER;
					ret.PartialCounters.ContPlugMinutes = DLLConnection.jbc.GetPortToolPluggedMinutes(UUID, (Port)p, counttype);
					ret.PartialCounters.ContWorkMinutes = DLLConnection.jbc.GetPortToolWorkMinutes(UUID, (Port)p, counttype);
					ret.PartialCounters.ContIdleMinutes = DLLConnection.jbc.GetPortToolIdleMinutes(UUID, (Port)p, counttype);
					ret.PartialCounters.ContSleepMinutes = DLLConnection.jbc.GetPortToolSleepMinutes(UUID, (Port) p, counttype);
					ret.PartialCounters.ContHiberMinutes = DLLConnection.jbc.GetPortToolHibernationMinutes(UUID, (Port) p, counttype);
					ret.PartialCounters.ContSleepCycles = DLLConnection.jbc.GetPortToolSleepCycles(UUID, (Port) p, counttype);
					ret.PartialCounters.ContDesoldCycles = DLLConnection.jbc.GetPortToolDesolderCycles(UUID, (Port) p, counttype);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		
		public dc_Port_Counters_HA GetPortCounters_HA(string UUID, JBC_Connect.dc_EnumConstJBC.dc_Port p)
		{
			dc_Port_Counters_HA ret = new dc_Port_Counters_HA();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CounterTypes counttype = default(CounterTypes);
					ret.portNbr = p;

					counttype = CounterTypes.GLOBAL_COUNTER;
					ret.GlobalCounters.ContPlugMinutes = DLLConnection.jbc.GetPortToolPluggedMinutes(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContWorkMinutes = DLLConnection.jbc.GetPortToolWorkMinutes(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContWorkCycles = DLLConnection.jbc.GetPortToolWorkCycles(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContSuctionCycles = DLLConnection.jbc.GetPortToolSuctionCycles(UUID, (Port) p, counttype);

					counttype = CounterTypes.PARTIAL_COUNTER;
					ret.PartialCounters.ContPlugMinutes = DLLConnection.jbc.GetPortToolPluggedMinutes(UUID, (Port) p, counttype);
					ret.PartialCounters.ContWorkMinutes = DLLConnection.jbc.GetPortToolWorkMinutes(UUID, (Port) p, counttype);
					ret.PartialCounters.ContWorkCycles = DLLConnection.jbc.GetPortToolWorkCycles(UUID, (Port) p, counttype);
					ret.PartialCounters.ContSuctionCycles = DLLConnection.jbc.GetPortToolSuctionCycles(UUID, (Port) p, counttype);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		public dc_Port_Counters_SF GetPortCounters_SF(string UUID, dc_EnumConstJBC.dc_Port p)
		{
			dc_Port_Counters_SF ret = new dc_Port_Counters_SF();

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CounterTypes counttype = default(CounterTypes);
					ret.portNbr = p;

					counttype = CounterTypes.GLOBAL_COUNTER;
					ret.GlobalCounters.ContTinLength = System.Convert.ToInt64(DLLConnection.jbc.GetPortToolTinLength(UUID, (Port) p, counttype));
					ret.GlobalCounters.ContPlugMinutes = DLLConnection.jbc.GetPortToolPluggedMinutes(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContWorkMinutes = DLLConnection.jbc.GetPortToolWorkMinutes(UUID, (Port) p, counttype);
					ret.GlobalCounters.ContWorkCycles = DLLConnection.jbc.GetPortToolWorkCycles(UUID, (Port) p, counttype);

					counttype = CounterTypes.PARTIAL_COUNTER;
					ret.PartialCounters.ContTinLength = System.Convert.ToInt64(DLLConnection.jbc.GetPortToolTinLength(UUID, (Port) p, counttype));
					ret.PartialCounters.ContPlugMinutes = DLLConnection.jbc.GetPortToolPluggedMinutes(UUID, (Port) p, counttype);
					ret.PartialCounters.ContWorkMinutes = DLLConnection.jbc.GetPortToolWorkMinutes(UUID, (Port) p, counttype);
					ret.PartialCounters.ContWorkCycles = DLLConnection.jbc.GetPortToolWorkCycles(UUID, (Port) p, counttype);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}


		// set

		public void ResetPortPartialCounters(string UUID, dc_EnumConstJBC.dc_Port p)
		{
			Console.WriteLine(" ResetPortPartialCounters: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.ResetPortToolStationPartialCounters(UUID, (Port)p);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

#endregion

#region Continuous Mode :fet:

		// get
		public dc_ContinuousModeStatus GetContinuousMode(string UUID)
		{
			dc_ContinuousModeStatus ret = new dc_ContinuousModeStatus();
			Console.WriteLine(" GetContinuousMode: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CContinuousModeStatus cm = DLLConnection.jbc.GetContinuousMode(UUID);
					ret.port1 = cm.port1;
					ret.port2 = cm.port2;
					ret.port3 = cm.port3;
					ret.port4 = cm.port4;
					ret.Speed = (dc_EnumConstJBC.dc_SpeedContinuousMode)cm.speed;
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}

		public uint StartContinuousMode(string UUID)
		{
			uint ret = (uint)0;
			Console.WriteLine(" StartContinuousMode: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret = DLLConnection.jbc.StartContinuousMode(UUID);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}


		public void StopContinuousMode(string UUID, uint queueID)
		{
			Console.WriteLine(" StopContinuousMode: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.StopContinuousMode(UUID, queueID);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}


		public int GetContinuousModeDataCount(string UUID, uint queueID)
		{
			int ret = 0;
			Console.WriteLine(" GetContinuousModeDataCount: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					ret = DLLConnection.jbc.GetContinuousModeDataCount(UUID, queueID);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}


		public dc_ContinuousModeData GetContinuousModeNextData(string UUID, uint queueID)
		{
			dc_ContinuousModeData ret = new dc_ContinuousModeData();
			Console.WriteLine(" GetContinuousModeNextData: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					var iCount = DLLConnection.jbc.GetContinuousModeDataCount(UUID, queueID);
					if (iCount <= 0)
					{
						return ret;
					}
					stContinuousModeData_SOLD contdata = DLLConnection.jbc.GetContinuousModeNextData_SOLD(UUID, queueID);
					ret.sequence = contdata.sequence;
					ret.data = new dc_ContinuousModePort[contdata.data.Length - 1 + 1];
					for (var i = 0; i <= contdata.data.Length - 1; i++)
					{
						dc_ContinuousModePort retDataPort = new dc_ContinuousModePort();
						retDataPort.port = (dc_EnumConstJBC.dc_Port)(contdata.data[i].port);
						retDataPort.temperature = convTempToStruc(contdata.data[i].temperature, false);
						retDataPort.power = System.Convert.ToInt32(contdata.data[i].power);
						retDataPort.status = (dc_EnumConstJBC.dc_ToolStatus)(contdata.data[i].status);
						retDataPort.desoldering = contdata.data[i].desoldering;
						ret.data[(int)i] = retDataPort;
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}


		public dc_ContinuousModeData_HA GetContinuousModeNextData_HA(string UUID, uint queueID)
		{
			dc_ContinuousModeData_HA ret = new dc_ContinuousModeData_HA();
			Console.WriteLine(" GetContinuousModeNextData_HA: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					var iCount = DLLConnection.jbc.GetContinuousModeDataCount(UUID, queueID);
					if (iCount <= 0)
					{
						return ret;
					}
					stContinuousModeData_HA contdata = DLLConnection.jbc.GetContinuousModeNextData_HA(UUID, queueID);
					ret.sequence = contdata.sequence;
					ret.data = new dc_ContinuousModePort_HA[contdata.data.Length - 1 + 1];
					for (var i = 0; i <= contdata.data.Length - 1; i++)
					{
						dc_ContinuousModePort_HA retDataPort = new dc_ContinuousModePort_HA();
						retDataPort.port = (dc_EnumConstJBC.dc_Port)(contdata.data[i].port);
						retDataPort.temperature = convTempToStruc(contdata.data[i].temperature, false);
						retDataPort.ext1Temp = convTempToStruc(contdata.data[i].externalTC1_Temp, false);
						retDataPort.ext2Temp = convTempToStruc(contdata.data[i].externalTC2_Temp, false);
						retDataPort.power = System.Convert.ToInt32(contdata.data[i].power);
						retDataPort.flow = System.Convert.ToInt32(contdata.data[i].flow);
						retDataPort.timeToStop = System.Convert.ToInt32(contdata.data[i].timeToStop);
						retDataPort.status = (dc_EnumConstJBC.dc_ToolStatus_HA)(contdata.data[i].status);
						ret.data[(int)i] = retDataPort;
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return ret;
		}


		public dc_ContinuousModeData[] GetContinuousModeNextDataChunk(string UUID, uint queueID, int iChunk)
		{
			List<dc_ContinuousModeData> retList = new List<dc_ContinuousModeData>();
			Console.WriteLine(" GetContinuousModeNextDataChunk: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					var iCount = DLLConnection.jbc.GetContinuousModeDataCount(UUID, queueID);
					if (iCount <= 0 | iChunk <= 0)
					{
						return retList.ToArray();
					}
					if (iCount < iChunk)
					{
						iChunk = iCount;
					}
					for (var x = 0; x <= iChunk - 1; x++)
					{
						stContinuousModeData_SOLD contdata = DLLConnection.jbc.GetContinuousModeNextData_SOLD(UUID, queueID);
						dc_ContinuousModeData retData = new dc_ContinuousModeData();
						retData.sequence = contdata.sequence;
						retData.data = new dc_ContinuousModePort[contdata.data.Length - 1 + 1];
						for (var i = 0; i <= contdata.data.Length - 1; i++)
						{
							dc_ContinuousModePort retDataPort = new dc_ContinuousModePort();
							retDataPort.port = (dc_EnumConstJBC.dc_Port)(contdata.data[i].port);
							retDataPort.temperature = convTempToStruc(contdata.data[i].temperature, false);
							retDataPort.power = System.Convert.ToInt32(contdata.data[i].power);
							retDataPort.status = (dc_EnumConstJBC.dc_ToolStatus)(contdata.data[i].status);
							retDataPort.desoldering = contdata.data[i].desoldering;
							retData.data[(int)i] = retDataPort;
						}
						retList.Add(retData);
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return retList.ToArray();
		}


		public dc_ContinuousModeData_HA[] GetContinuousModeNextDataChunk_HA(string UUID, uint queueID, int iChunk)
		{
			List<dc_ContinuousModeData_HA> retList = new List<dc_ContinuousModeData_HA>();
			Console.WriteLine(" GetContinuousModeNextDataChunk_HA: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					var iCount = DLLConnection.jbc.GetContinuousModeDataCount(UUID, queueID);
					if (iCount <= 0 | iChunk <= 0)
					{
						return retList.ToArray();
					}
					if (iCount < iChunk)
					{
						iChunk = iCount;
					}
					for (var x = 0; x <= iChunk - 1; x++)
					{
						stContinuousModeData_HA contdata = DLLConnection.jbc.GetContinuousModeNextData_HA(UUID, queueID);
						dc_ContinuousModeData_HA retData = new dc_ContinuousModeData_HA();
						retData.sequence = contdata.sequence;
						retData.data = new dc_ContinuousModePort_HA[contdata.data.Length - 1 + 1];
						for (var i = 0; i <= contdata.data.Length - 1; i++)
						{
							dc_ContinuousModePort_HA retDataPort = new dc_ContinuousModePort_HA();
							retDataPort.port = (dc_EnumConstJBC.dc_Port)(contdata.data[i].port);
							retDataPort.temperature = convTempToStruc(contdata.data[i].temperature, false);
							retDataPort.ext1Temp = convTempToStruc(contdata.data[i].externalTC1_Temp, false);
							retDataPort.ext2Temp = convTempToStruc(contdata.data[i].externalTC2_Temp, false);
							retDataPort.power = System.Convert.ToInt32(contdata.data[i].power);
							retDataPort.flow = System.Convert.ToInt32(contdata.data[i].flow);
							retDataPort.timeToStop = System.Convert.ToInt32(contdata.data[i].timeToStop);
							retDataPort.status = (dc_EnumConstJBC.dc_ToolStatus_HA)(contdata.data[i].status);
							retData.data[(int)i] = retDataPort;
						}
						retList.Add(retData);
					}
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return retList.ToArray();
		}


		// set
		public void SetContinuousMode(string UUID, dc_EnumConstJBC.dc_SpeedContinuousMode speed, dc_EnumConstJBC.dc_Port portA = default(dc_EnumConstJBC.dc_Port), dc_EnumConstJBC.dc_Port portB = default(dc_EnumConstJBC.dc_Port), dc_EnumConstJBC.dc_Port portC = default(dc_EnumConstJBC.dc_Port), dc_EnumConstJBC.dc_Port portD = default(dc_EnumConstJBC.dc_Port))
		{
			Console.WriteLine(" SetContinuousMode: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SetContinuousMode(UUID, (SpeedContinuousMode)speed, (Port)portA, (Port)portB, (Port)portC, (Port)portD);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

#endregion

#region Commucation Operations :fet:
		// Soldering stations only
		public dc_EthernetConfiguration GetEthernetConfiguration(string UUID)
		{
			dc_EthernetConfiguration retEth = new dc_EthernetConfiguration();
			Console.WriteLine(" GetEthernetConfiguration: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CEthernetData ethernetData = DLLConnection.jbc.GetEthernetConfiguration(UUID);
					retEth.DHCP = (dc_EnumConstJBC.dc_OnOff)ethernetData.DHCP;
					retEth.IP = ethernetData.IP;
					retEth.Mask = ethernetData.Mask;
					retEth.Gateway = ethernetData.Gateway;
					retEth.DNS1 = ethernetData.DNS1;
					retEth.DNS2 = ethernetData.DNS2;
					retEth.Port = ethernetData.Port;
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return retEth;
		}

		// Soldering stations only
		public void SetEthernetConfiguration(string UUID, dc_EthernetConfiguration ethernetConfiguration)
		{
			Console.WriteLine(" SetEthernetConfiguration: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CEthernetData ethData = new CEthernetData();
					ethData.DHCP = (OnOff)ethernetConfiguration.DHCP;
					ethData.IP = ethernetConfiguration.IP;
					ethData.Mask = ethernetConfiguration.Mask;
					ethData.Gateway = ethernetConfiguration.Gateway;
					ethData.DNS1 = ethernetConfiguration.DNS1;
					ethData.DNS2 = ethernetConfiguration.DNS2;
					ethData.Port = ethernetConfiguration.Port;

					DLLConnection.jbc.SetEthernetConfiguration(UUID, ethData);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public dc_RobotConfiguration GetRobotConfiguration(string UUID)
		{
			dc_RobotConfiguration retRbt = new dc_RobotConfiguration();
			Console.WriteLine(" GetRobotConfiguration: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CRobotData robotData = DLLConnection.jbc.GetRobotConfiguration(UUID);
					retRbt.Status = (dc_EnumConstJBC.dc_OnOff)robotData.Status;
					retRbt.Protocol = (dc_EnumConstJBC.dc_RobotProtocol)robotData.Protocol;
					retRbt.Address = robotData.Address;
					retRbt.Speed = (dc_EnumConstJBC.dc_RobotSpeed)robotData.Speed;
					retRbt.DataBits = robotData.DataBits;
					retRbt.StopBits = (dc_EnumConstJBC.dc_RobotStop)robotData.StopBits;
					retRbt.Parity = (dc_EnumConstJBC.dc_RobotParity)robotData.Parity;
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return retRbt;
		}

		public void SetRobotConfiguration(string stationUUID, dc_RobotConfiguration robotConfiguration)
		{
			Console.WriteLine(" SetRobotConfiguration: " + stationUUID);
			try
			{
				if (!myChkStn(stationUUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CRobotData robotData = new CRobotData();
					robotData.Status = (OnOff)robotConfiguration.Status;
					robotData.Protocol = (CRobotData.RobotProtocol)robotConfiguration.Protocol;
					robotData.Address = robotConfiguration.Address;
					robotData.Speed = (CRobotData.RobotSpeed)robotConfiguration.Speed;
					robotData.DataBits = robotConfiguration.DataBits;
					robotData.StopBits = (CRobotData.RobotStop)robotConfiguration.StopBits;
					robotData.Parity = (CRobotData.RobotParity)robotConfiguration.Parity;

					DLLConnection.jbc.SetRobotConfiguration(stationUUID, robotData);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

#endregion

#region Peripheral Operations :fet:
		// Soldering stations only
		public dc_PeripheralInfo[] GetAllPeripheralInfo(string UUID)
		{
			List<dc_PeripheralInfo> retList = new List<dc_PeripheralInfo>();
			Console.WriteLine(" GetAllPeripheralInfo: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					List<CPeripheralData> peripheralDataList = DLLConnection.jbc.GetPeripheralList(UUID);
					foreach (CPeripheralData peripheralData in peripheralDataList)
					{
						dc_PeripheralInfo retPeripheral = new dc_PeripheralInfo();

						retPeripheral.ID = peripheralData.ID;
						retPeripheral.Version = peripheralData.Version;
						retPeripheral.Hash_MCU_UID = peripheralData.Hash_MCU_UID;
						retPeripheral.DateTime = peripheralData.DateTime;
						retPeripheral.Type = (dc_EnumConstJBC.dc_PeripheralType)peripheralData.Type;
						retPeripheral.PortAttached = (dc_EnumConstJBC.dc_Port)peripheralData.PortAttached;
						retPeripheral.WorkFunction = (dc_EnumConstJBC.dc_PeripheralFunction)peripheralData.WorkFunction;
						retPeripheral.ActivationMode = (dc_EnumConstJBC.dc_PeripheralActivation)peripheralData.ActivationMode;
						retPeripheral.DelayTime = peripheralData.DelayTime;
						retPeripheral.StatusActive = (dc_EnumConstJBC.dc_OnOff)peripheralData.StatusActive;
						retPeripheral.StatusPD = (dc_EnumConstJBC.dc_PeripheralStatusPD)peripheralData.StatusPD;

						retList.Add(retPeripheral);
					}
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return retList.ToArray();
		}

		// Soldering stations only
		public void SetPeripheralInfo(string stationUUID, dc_PeripheralInfo peripheralInfo)
		{
			Console.WriteLine(" SetPeripheralInfo: " + stationUUID);
			try
			{
				if (!myChkStn(stationUUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(stationUUID, dc_EnumConstJBC.dc_StationType.SOLD))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CPeripheralData peripheralData = new CPeripheralData();
					peripheralData.ID = peripheralInfo.ID;
					peripheralData.Version = peripheralInfo.Version;
					peripheralData.Hash_MCU_UID = peripheralInfo.Hash_MCU_UID;
					peripheralData.DateTime = peripheralInfo.DateTime;
					peripheralData.Type = (CPeripheralData.PeripheralType)peripheralInfo.Type;
					peripheralData.PortAttached = (Port)peripheralInfo.PortAttached;
					peripheralData.WorkFunction = (CPeripheralData.PeripheralFunction)peripheralInfo.WorkFunction;
					peripheralData.ActivationMode = (CPeripheralData.PeripheralActivation)peripheralInfo.ActivationMode;
					peripheralData.DelayTime = peripheralInfo.DelayTime;

					DLLConnection.jbc.SetPeripheralInfo(stationUUID, peripheralData);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

#endregion

#region Profile Operations

		public dc_Profile_HA[] GetAllProfiles_HA(string UUID)
		{
			List<dc_Profile_HA> retList = new List<dc_Profile_HA>();
			Console.WriteLine(" GetAllProfiles_HA: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					List<CProfileData_HA> profileDataList = DLLConnection.jbc.GetProfileList(UUID);
					foreach (CProfileData_HA profileData in profileDataList)
					{
						dc_Profile_HA retProfile = new dc_Profile_HA();

						retProfile.Name = profileData.Name;
						retProfile.Data = profileData.Data;

						retList.Add(retProfile);
					}
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return retList.ToArray();
		}

		public string GetSelectedProfile_HA(string UUID)
		{
			Console.WriteLine(" GetSelectedProfile_HA: " + UUID);
			string selectedProfile = "";

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					selectedProfile = DLLConnection.jbc.GetSelectedProfile(UUID);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return selectedProfile;
		}

		public bool SetProfile_HA(string UUID, dc_Profile_HA profile)
		{
            bool bOk = false;
			Console.WriteLine(" SetProfile_HA: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					CProfileData_HA profileData = new CProfileData_HA();
					profileData.Name = profile.Name;
					profileData.Data = profile.Data;
					bOk = DLLConnection.jbc.SetProfile(UUID, profileData);

				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
            return bOk;
		}

		public void DeleteProfile_HA(string UUID, string profileName)
		{
			Console.WriteLine(" DeleteProfile_HA: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.DeleteProfile(UUID, profileName);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SyncProfiles_HA(string UUID)
		{
			Console.WriteLine(" SyncProfiles_HA: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.SyncProfiles(UUID);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public bool SyncFinishedProfiles_HA(string UUID)
		{
			bool bFinished = false;

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else if (!myChkStnType(UUID, JBC_Connect.dc_EnumConstJBC.dc_StationType.HA))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.FunctionNotSupported, My.Resources.Resources.errFunctionNotSupported,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					bFinished = System.Convert.ToBoolean(DLLConnection.jbc.SyncFinishedProfiles(UUID));
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bFinished;
		}

#endregion

#region Others :fet:

		// set

		public uint SetTransaction(string UUID)
		{
			Console.WriteLine(" SetTransaction: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					return DLLConnection.jbc.SetTransaction(UUID);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public bool QueryEndedTransaction(string UUID, uint transactionID)
		{
			Console.WriteLine(" QueryEndedTransaction: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					return DLLConnection.jbc.QueryTransaction(UUID, transactionID);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void SetDefaultStationParams(string UUID)
		{
			Console.WriteLine(" SetDefaultStationParams: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.DefaultStationParameters(UUID);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

		public void ResetStation(string UUID)
		{
			Console.WriteLine(" ResetStation: " + UUID);
			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					DLLConnection.jbc.ResetStation(UUID);
				}
			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation + ". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}
		}

#endregion

#region Update Firmware

		public List<string> GetStationListUpdating()
		{
			Console.WriteLine(" GetStationListUpdating: ");
			return DLLConnection.jbc.GetStationListUpdating();
		}

		public void UpdateStations(List<dc_FirmwareStation> listStationsToUpdate)
		{
			Console.WriteLine(" UpdateStations: ");
			List<CFirmwareStation> listFirmwareUpdate = new List<CFirmwareStation>();

			foreach (dc_FirmwareStation stationToUpdate in listStationsToUpdate)
			{
				CFirmwareStation firmwareUpdate = new CFirmwareStation();
				firmwareUpdate.StationUUID = stationToUpdate.stationUUID;
				firmwareUpdate.Model = stationToUpdate.model;
				firmwareUpdate.SoftwareVersion = stationToUpdate.softwareVersion;
				firmwareUpdate.HardwareVersion = stationToUpdate.hardwareVersion;
				listFirmwareUpdate.Add(firmwareUpdate);
			}

			DLLConnection.jbc.UpdateStations(listFirmwareUpdate);
		}

#endregion

#region Rutines

		private bool myChkStn(string stationUUID)
		{
			//Console.WriteLine(" myChkStn: "+ stationID);
			// checks in JBC_Connect exists and also the station ID
			if (ReferenceEquals(DLLConnection.jbc, null))
			{
				return false;
			}

			return DLLConnection.jbc.StationExists(stationUUID);
		}

		private bool myChkStnType(string stationUUID, dc_EnumConstJBC.dc_StationType stationType)
		{
			// checks in JBC_Connect exists and also the station ID
			if (ReferenceEquals(DLLConnection.jbc, null))
			{
				return false;
			}
			return ((dc_EnumConstJBC.dc_StationType)(DLLConnection.jbc.GetStationType(stationUUID))) == stationType;
		}

		private dc_getTemperature convTempToStruc(CTemperature temp, bool bRounded = false)
		{
			var strTemp = new dc_getTemperature();

			if (temp != null)
			{
				strTemp.UTI = temp.UTI;

				if (temp.UTI == 0 | temp.UTI == Constants.NO_FIXED_TEMP | temp.UTI == Constants.NO_TEMP_LEVEL)
				{
					strTemp.Celsius = temp.UTI;
					strTemp.Fahrenheit = temp.UTI;
				}
				else
				{
					if (bRounded)
					{
						strTemp.Celsius = temp.ToRoundCelsius();
						strTemp.Fahrenheit = temp.ToRoundFahrenheit();
					}
					else
					{
						strTemp.Celsius = temp.ToCelsius();
						strTemp.Fahrenheit = temp.ToFahrenheit();
					}
				}
			}

			return strTemp;
		}

		private dc_getTemperature convAdjustTempToStruc(CTemperature temp)
		{
			var strTemp = new dc_getTemperature();
			strTemp.UTI = temp.UTI;

			if (temp.UTI == 0 | temp.UTI == Constants.NO_FIXED_TEMP | temp.UTI == Constants.NO_TEMP_LEVEL)
			{
				strTemp.Celsius = temp.UTI;
				strTemp.Fahrenheit = temp.UTI;
			}
			else
			{
				strTemp.Celsius = temp.ToCelsiusToAdjust();
				strTemp.Fahrenheit = temp.ToFahrenheitToAdjust();
			}

			return strTemp;
		}

		private dc_Length convLengthToStruc(CLength length)
		{
			var strLength = new dc_Length();

			if (length != null)
			{
				strLength.Inches = length.ToInches();
				strLength.Millimeters = length.ToMillimeters();
			}

			return strLength;
		}

		private dc_Speed convSpeedToStruc(CSpeed speed)
		{
			var strSpeed = new dc_Speed();

			if (speed != null)
			{
				strSpeed.InchesPerSecond = speed.ToInchesPerSecond();
				strSpeed.MillimetersPerSecond = speed.ToMillimetersPerSecond();
			}

			return strSpeed;
		}
#endregion

#region Traceability

		public bool StartTraceability(string ServerCode, string Ip, ushort Port)
		{
			bool bOk = false;

			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". IP: " + Ip +". Port: " + System.Convert.ToString(Port) +". Server Code: " + ServerCode);

			try
			{
				UriBuilder uriTraceManager = new UriBuilder(Ip);
				uriTraceManager.Port = Port;
				if (uriTraceManager.ToString() != null)
				{
					My.Settings.Default.TraceManagerUri = uriTraceManager.ToString() + "JBCTraceController/HostTrace";
					My.Settings.Default.TraceManagerServerCode = ServerCode;
					My.Settings.Default.Save();

					//Notify of all connected stations
					string[] stationsUUID = GetStationList();
					foreach (string stationUUID in stationsUUID)
					{
						Thread workerEvent_StationConnected = default(Thread);
						workerEvent_StationConnected = new Thread(Event_StationConnectedDelayed);
						workerEvent_StationConnected.IsBackground = true;
						workerEvent_StationConnected.Start(stationUUID);
					}

					bOk = true;
				}
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

		public bool StopTraceability()
		{
			bool bOk = true;

			My.Settings.Default.TraceManagerUri = "";
			My.Settings.Default.TraceManagerServerCode = "";
			My.Settings.Default.Save();
			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name);

			return bOk;
		}

#endregion

#region User session

		public bool NewUserSession(string UUID, dc_EnumConstJBC.dc_Port portNbr, string userCode, string userName, string inputDeviceID)
		{
			bool bOk = false;

			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +
					". StationUUID: " + UUID +
					". PortNbr: " + System.Convert.ToString(portNbr) +
					". UserCode: " + userCode +
					". UserName: " + userName +
					". InputDeviceID: " + inputDeviceID);

			//check availability of trace service
			if (My.Settings.Default.TraceManagerUri != "")
			{
				bOk = m_userSession.NewUserSession(UUID, portNbr, userCode, userName, inputDeviceID);
			}

			return bOk;
		}

		public bool CloseUserSession(string UUID, dc_EnumConstJBC.dc_Port portNbr)
		{
			bool bOk = false;

			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". StationUID: " + UUID +". PortNbr: " + System.Convert.ToString(portNbr));

			//check availability of trace service
			try
			{
				if (My.Settings.Default.TraceManagerUri != "")
				{
					BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
					JBCTraceControllerServiceReference.HostTraceClient serviceClient = new JBCTraceControllerServiceReference.HostTraceClient(binding, new EndpointAddress(System.Convert.ToString(My.Settings.Default.TraceManagerUri)));

					serviceClient.Open();
					bOk = serviceClient.CloseUserSession(UUID, (int)portNbr);
					serviceClient.Close();
				}

				bOk = bOk && m_userSession.CloseUserSession(UUID, portNbr);
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

		public string GetAuthenticatedUser(string UUID, dc_EnumConstJBC.dc_Port portNbr)
		{
			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". StationUID: " + UUID +". PortNbr: " + System.Convert.ToString(portNbr));

			return m_userSession.GetAuthenticatedUser(UUID, portNbr);
		}

	//Station configuration
		public bool LoadConfigurationPortStation(string UUID, JBC_Connect.dc_EnumConstJBC.dc_Port portNbr, dc_ConfigurationPortStation configuration)
		{
			bool bOk = false;
			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". StationUID: " + UUID +". PortNbr: " + System.Convert.ToString(portNbr));

			if (m_userSession.GetAuthenticatedUser(UUID, portNbr) != "")
			{
				SetPortToolSelectedTemp(UUID, portNbr, configuration.PortSelectedTemp, "");
				SetPortToolAdjustTemp(UUID, portNbr, configuration.Tool, configuration.AdjustTemp, "");

				m_TraceData.StartTraceData(UUID, (Port) portNbr);
				bOk = true;
			}

			return bOk;
		}

#endregion

#region Record data

		#region Record data

		public bool StartRecordData(string UUID, JBC_Connect.dc_EnumConstJBC.dc_Port portNbr)
		{
			bool bOk = false;

			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". StationUUID: " + UUID +". PortNbr: " + System.Convert.ToString(portNbr));

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					bOk = m_TraceData.StartTraceData(UUID, (Port) portNbr);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

		public bool StopRecordData(string UUID, JBC_Connect.dc_EnumConstJBC.dc_Port portNbr)
		{
			bool bOk = false;

			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". StationUID: " + UUID +". PortNbr: " + System.Convert.ToString(portNbr));

			try
			{
				if (!myChkStn(UUID))
				{
					throw (ExceptionsRoutines.getFaultEx(JBC_Connect.dc_EnumConstJBC.dc_FaultError.StationNotFound, My.Resources.Resources.errStationNotFound,
								System.Reflection.MethodBase.GetCurrentMethod().Name));
				}
				else
				{
					bOk = m_TraceData.StopTraceData(UUID, (Port) portNbr);
				}

			}
			catch (FaultException<faultError> faultEx)
			{
				LoggerModule.logger.Error(faultEx.Detail.Operation +". Error: " + faultEx.Detail.Message);
				throw (new FaultException<faultError>(faultEx.Detail, faultEx.Reason.ToString()));
			}
			catch (Exception ex)
			{
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				throw (ExceptionsRoutines.getFaultEx(ex, System.Reflection.MethodBase.GetCurrentMethod().Name));
			}

			return bOk;
		}

		public List<string> GetListRecordedDataFiles()
		{
			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name);			
			return m_TraceData.GetListRecordedDataFiles();
		}

		public dc_TraceDataSequence GetRecordedData(string fileName, int nSequence)
		{
			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". FileName: " + fileName +". Sequence: " + System.Convert.ToString(nSequence));			
			return m_TraceData.GetRecordedData(fileName, nSequence);
		}

		public bool DeleteRecordedDataFile(string fileName)
		{
			LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". FileName: " + fileName);			
			return m_TraceData.DeleteRecordedDataFile(fileName);
		}

#endregion

#region Stations Events

		private void Event_StationConnectedDelayed(object stationUUID)
		{
			Thread.Sleep(100);
			Event_StationConnected((stationUUID).ToString());
		}


		private void Event_StationConnected(string stationUUID)
			{

			//Traceability CallBack
			if (My.Settings.Default.TraceManagerUri != "")
			{
				try
				{
					BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
					EndpointAddress endPoint = new EndpointAddress(System.Convert.ToString(My.Settings.Default.TraceManagerUri));
					JBCTraceControllerServiceReference.HostTraceClient serviceClient = new JBCTraceControllerServiceReference.HostTraceClient(binding, endPoint);

					LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". StationUUID: " + stationUUID +". Now: " + System.Convert.ToString(DateTime.Now.ToBinary()));

					serviceClient.Open();
					serviceClient.StationConnected(System.Convert.ToString(My.Settings.Default.TraceManagerServerCode), stationUUID, DateTime.Now.ToBinary());
					serviceClient.Close();
				}
				catch (Exception ex)
				{
					LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				}
			}
		}


		private void Event_StationDisconnected(string stationUUID)
		{

			//Traceability CallBack
			if (My.Settings.Default.TraceManagerUri != "")
			{
				try
				{
					BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
					JBCTraceControllerServiceReference.HostTraceClient serviceClient = new JBCTraceControllerServiceReference.HostTraceClient(binding, new EndpointAddress(System.Convert.ToString(My.Settings.Default.TraceManagerUri)));

					LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". StationUUID: " + stationUUID +". Now: " + System.Convert.ToString(DateTime.Now.ToBinary()));

					serviceClient.Open();
					serviceClient.StationDisconnected(System.Convert.ToString(My.Settings.Default.TraceManagerServerCode), stationUUID, DateTime.Now.ToBinary());
					serviceClient.Close();
				}
				catch (Exception ex)
				{
					LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Error: " + ex.Message);
				}
			}
		}

#endregion

#region Keyboard Input Device

		private void KeyboardDisconnected(string deviceName)
		{
			m_userSession.CloseKeyboardUserSession(UniqueKeyboardDeviceName(deviceName));
		}

		private void KeyboardMessage(CKeyboardMessage message)
		{
			string deviceName = UniqueKeyboardDeviceName(message.deviceName);

			//Traceability CallBack
			if (My.Settings.Default.TraceManagerUri != "")
			{
				try
				{
					BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
					JBCTraceControllerServiceReference.HostTraceClient serviceClient = new JBCTraceControllerServiceReference.HostTraceClient(binding, new EndpointAddress(System.Convert.ToString(My.Settings.Default.TraceManagerUri)));

					serviceClient.Open();
					serviceClient.NewDataEntry(System.Convert.ToString(My.Settings.Default.TraceManagerServerCode), message.message, DateTime.Now.ToBinary(), "", (System.Int32)Port.NO_PORT, deviceName);
					serviceClient.Close();

					LoggerModule.logger.Debug(System.Reflection.MethodInfo.GetCurrentMethod().Name +". Message: " + message.message +". Now: " + System.Convert.ToString(DateTime.Now.ToBinary()));					
				}
				catch (Exception ex)
				{
					LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
				}
			}
		}

		private string UniqueKeyboardDeviceName(string deviceName)
		{
			string parsedDeviceName = "";

			string[] splitDeviceName = deviceName.Split('#');
			if (splitDeviceName.Length > 2)
			{
				parsedDeviceName = splitDeviceName[1];

				string[] splitCode = splitDeviceName[2].Split('&');
				if (splitCode.Length > 0)
				{
					parsedDeviceName += "&" + splitCode[1];
				}
			}

			return parsedDeviceName;
		}

#endregion

	}
}
#endregion