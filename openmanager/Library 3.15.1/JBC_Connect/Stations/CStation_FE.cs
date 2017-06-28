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

using System.Threading;
using DataJBC;
using RoutinesJBC;
using Constants = DataJBC.Constants;


namespace JBC_Connect
{
	
	
	internal class CStation_FE : CStationBase
	{
		
		//Data
		private CStationData_FE m_StationData = new CStationData_FE(); //Información de la estación
		private CPortData_FE[] m_PortData = new CPortData_FE[0]; //Información del puerto
		
		//Frames
		private CStationFrames02_FE m_Frames_02;
		
		
		/// <summary>
		/// When an error occurs this event is launched. Gives a Cerror object with
		/// the error information.
		/// </summary>
		/// <param name="stationUUID">An identifier to the station with error</param>
		/// <param name="err">The Cerror object with the error information</param>
		/// <remarks></remarks>
		public delegate void UserErrorEventHandler(string stationUUID, Cerror err);
		private UserErrorEventHandler UserErrorEvent;
		
		public event UserErrorEventHandler UserError
		{
			add
			{
				UserErrorEvent = (UserErrorEventHandler) System.Delegate.Combine(UserErrorEvent, value);
			}
			remove
			{
				UserErrorEvent = (UserErrorEventHandler) System.Delegate.Remove(UserErrorEvent, value);
			}
		}
		
		public delegate void StationDisconnectedEventHandler(string stationUUID);
		private StationDisconnectedEventHandler StationDisconnectedEvent;
		
		public event StationDisconnectedEventHandler StationDisconnected
		{
			add
			{
				StationDisconnectedEvent = (StationDisconnectedEventHandler) System.Delegate.Combine(StationDisconnectedEvent, value);
			}
			remove
			{
				StationDisconnectedEvent = (StationDisconnectedEventHandler) System.Delegate.Remove(StationDisconnectedEvent, value);
			}
		}
		
		public delegate void InitializedEventHandler(string stationUUID);
		private InitializedEventHandler InitializedEvent;
		
		public event InitializedEventHandler Initialized
		{
			add
			{
				InitializedEvent = (InitializedEventHandler) System.Delegate.Combine(InitializedEvent, value);
			}
			remove
			{
				InitializedEvent = (InitializedEventHandler) System.Delegate.Remove(InitializedEvent, value);
			}
		}
		
		public delegate void Detected_SubStationEventHandler(string stationParentUUID, CConnectionData connectionData);
		private Detected_SubStationEventHandler Detected_SubStationEvent;
		
		public event Detected_SubStationEventHandler Detected_SubStation
		{
			add
			{
				Detected_SubStationEvent = (Detected_SubStationEventHandler) System.Delegate.Combine(Detected_SubStationEvent, value);
			}
			remove
			{
				Detected_SubStationEvent = (Detected_SubStationEventHandler) System.Delegate.Remove(Detected_SubStationEvent, value);
			}
		}
		
		public delegate void UpdateMicroFirmwareFinishedEventHandler(string stationUUID);
		private UpdateMicroFirmwareFinishedEventHandler UpdateMicroFirmwareFinishedEvent;
		
		public event UpdateMicroFirmwareFinishedEventHandler UpdateMicroFirmwareFinished
		{
			add
			{
				UpdateMicroFirmwareFinishedEvent = (UpdateMicroFirmwareFinishedEventHandler) System.Delegate.Combine(UpdateMicroFirmwareFinishedEvent, value);
			}
			remove
			{
				UpdateMicroFirmwareFinishedEvent = (UpdateMicroFirmwareFinishedEventHandler) System.Delegate.Remove(UpdateMicroFirmwareFinishedEvent, value);
			}
		}
		
		
		
		public CStation_FE(byte _StationNumDevice, Protocol _CommandProtocol, Protocol _FrameProtocol, string _StationModel, string _SoftwareVersion, string _HardwareVersion, CCommunicationChannel _ComChannel, string _ParentUUID = "")
			{
			
			m_StationData.Info.ParentUUID = _ParentUUID;
			m_StationNumDevice = _StationNumDevice;
			m_CommandProtocol = _CommandProtocol;
			m_FrameProtocol = _FrameProtocol;
			m_ComChannel = _ComChannel;
			
			m_StationData.Info.Version_Software = _SoftwareVersion;
			m_StationData.Info.Version_Hardware = _HardwareVersion;
			
			//Protocol
			m_StationData.Info.Protocol = Strings.Format(System.Convert.ToInt32(_CommandProtocol), "00");
			
			//Model
			//En protocol 01: model or model_modelversion
			//En protocol 02: model_modeltype_modelversion
			CModelData stationModelData = new CModelData(_StationModel);
			m_StationData.Info.Model = stationModelData.Model;
			m_StationData.Info.ModelType = stationModelData.ModelType;
			m_StationData.Info.ModelVersion = stationModelData.ModelVersion;
			
			//Initialize ports and tools and station type
			InitializeStationPortsToolsType(m_StationData.Info.Model);
			
			//Features
			m_StationData.Info.Features = new CFeaturesData(m_StationData.Info.Model, m_StationData.Info.ModelType, m_StationData.Info.ModelVersion, m_StationData.Info.Protocol);
		}
		
		public void Dispose()
		{
			
			//Terminate threads
			m_ThreadUpdateDataAlive = false;
			m_ThreadSearchSubStationsAlive = false;
			m_ThreadCheckDataInitializedAlive = false;
			
			m_Frames_02.Dispose();
		}
		
		
#region INITIALIZATION
		
		public void InitializeComChannel()
		{
			m_ComChannel.Initialize(m_StationData.Info.Features.BurstMessages, m_StationNumDevice, m_FrameProtocol, m_CommandProtocol);
		}
		
		public string Initialize()
		{
			string UUID = "";
			
			//Initialize address communication channel
			m_ComChannel.AddStack(m_StationNumDevice);
			
			//Initialize station frames
			m_Frames_02 = new CStationFrames02_FE(m_StationData, m_PortData, m_ComChannel, m_StationNumDevice);
			m_Frames_02.Detected_SubStation += Event_Detected_SubStation;
			m_Frames_02.ConnectionError += ConnectionError;
			m_Frames_02.EndedTransaction += AddEndedTransaction;
			
			//Initialize UUID if empty
			m_Frames_02.ReadDeviceUID();
			
			uint transactionID = SetTransaction();
			int retriesCheckUUIDInitialized = RETRIES_CHECK_UUID_INITIALIZED;
			
			while (retriesCheckUUIDInitialized > 0)
			{
				if (QueryEndedTransaction(transactionID))
				{
					if (string.IsNullOrEmpty(m_StationData.Info.UUID))
					{
						SetControlMode(ControlModeConnection.CONTROL);
						SetNewStationUUID();
						SetControlMode(ControlModeConnection.MONITOR);
					}
					UUID = m_StationData.Info.UUID;
					
					break;
				}
				
				retriesCheckUUIDInitialized--;
				Thread.Sleep(100);
			}
			
			//If we do not have the UUID the station was not initialized correctly
			if (!string.IsNullOrEmpty(UUID))
			{
				
				//Initialize update data process
				m_ThreadUpdateData = new Thread(new System.Threading.ThreadStart(UpdateDataProcess));
				m_ThreadUpdateData.IsBackground = true;
				m_ThreadUpdateData.Start();
				
				//Check data initialized
				m_ThreadCheckDataInitialized = new Thread(new System.Threading.ThreadStart(CheckDataInitialized));
				m_ThreadCheckDataInitialized.IsBackground = true;
				m_ThreadCheckDataInitialized.Start();
			}
			
			return UUID;
		}
		
		/// <summary>
		/// Devuelve en 3 variables de la clase la siguiente información:
		/// TypeStation --> el tipo de estación
		/// NumPort --> número de puertos
		/// ToolSoportadas() --> herramientas soportadas
		/// </summary>
		/// <remarks></remarks>
		internal void InitializeStationPortsToolsType(string model)
		{
			CStationsConfiguration confStation = new CStationsConfiguration(model);
			int iPorts = confStation.Ports;
			eStationType stationType = confStation.StationType;
			
			m_StationData.Info.StationType = stationType;
			m_PortData = new CPortData_FE[iPorts - 1 + 1];
			
			//Recorremos todos los puertos de la estación
			for (int idxPort = 0; idxPort <= m_PortData.Length - 1; idxPort++)
			{
				m_PortData[idxPort] = new CPortData_FE();
			}
		}
		
		private void CheckDataInitialized()
		{
			while (m_ThreadCheckDataInitializedAlive)
			{
				
				if (m_IdTransactionDataInitialized != UInt32.MaxValue && QueryEndedTransaction(m_IdTransactionDataInitialized))
				{
					m_IsDataInitialized = true;
					if (InitializedEvent != null)
						InitializedEvent(UUID);
					break;
				}
				
				Thread.Sleep(TIME_CHECK_DATA_INITIALIZED);
			}
		}
		
#endregion
		
		
#region STATION METHODS
		
#region Port
		
		public OnOff GetIntakeActivation(Port Port, ToolStatus_FE intake)
		{
			OnOff activated = OnOff._OFF;
			
			//Check port
			if ((int)Port >= NumPorts|| (int)Port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				if (intake == ToolStatus_FE.WORK)
				{
					activated = m_PortData[(int)Port].ToolStatus.IntakeActivationWork;
				}
				else if (intake == ToolStatus_FE.STAND)
				{
					activated = m_PortData[(int)Port].ToolStatus.IntakeActivationStand;
				}
			}
			
			return activated;
		}
		
		public void SetWorkIntakeActivation(Port Port, OnOff activated)
		{
			//Check port
			if ((int)Port >= NumPorts|| (int)Port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				m_Frames_02.WriteWorkIntakeActivation(Port, activated);
				m_Frames_02.ReadIntakeActivation(Port, ToolStatus_FE.WORK);
			}
		}
		
#endregion
		
		
#region Control mode
		
		public ControlModeConnection GetControlMode()
		{
			return m_StationData.Status.ControlMode;
		}
		
		public void SetControlMode(ControlModeConnection mode)
		{
			m_Frames_02.WriteConnectStatus(mode);
			m_Frames_02.ReadConnectStatus();
		}
		
#endregion
		
		
#region Station methods
		
		public void SetDefaultStationParams()
		{
			m_Frames_02.ReadResetParam();
		}
		
		public eStationType GetStationType()
		{
			return m_StationData.Info.StationType;
		}
		
		public string GetStationProtocol()
		{
			return m_StationData.Info.Protocol;
		}
		
		public string GetStationParentUUID()
		{
			return m_StationData.Info.ParentUUID;
		}
		
		public string GetStationModel()
		{
			return m_StationData.Info.Model;
		}
		
		public string GetStationModelType()
		{
			return m_StationData.Info.ModelType;
		}
		
		public int GetStationModelVersion()
		{
			return m_StationData.Info.ModelVersion;
		}
		
		public string GetStationHWversion()
		{
			return m_StationData.Info.Version_Hardware;
		}
		
		public string GetStationSWversion()
		{
			return m_StationData.Info.Version_Software;
		}
		
		public StationError GetStationError()
		{
			return m_StationData.Status.ErrorStation;
		}
		
		public int NumPorts
		{
			get
			{
				return m_PortData.Length;
			}
		}
		
		public string GetStationName()
		{
			return m_StationData.Settings.Name;
		}
		
		public void SetStationName(string stationName)
		{
			
			if (stationName.Length > MAX_LENGTH_DEVICENAME)
			{
				stationName = stationName.Remove(MAX_LENGTH_DEVICENAME);
			}
			
			m_Frames_02.WriteDeviceName(stationName);
			m_Frames_02.ReadDeviceName();
		}
		
		public string UUID
		{
			get
			{
				return m_StationData.Info.UUID;
			}
		}
		
		public void SetNewStationUUID(int iSequence = 0)
		{
			int iSeqLen = iSequence.ToString().Length;
			if (iSeqLen > 8)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.INVALID_STATION_NAME, "Invalid internal ID - Max. Sequence Length = 8."));
				return ;
			}
			
			// 30/03/2015 Falta habilitar ReadDeviceID (también en ControlStackSold01 y 02) y WriteDeviceID cuando se implemente definitivamente en las estaciones
			// 30/05/2016 Se habilita la lectura y grabación de UID de la estación
			clsStationUID stationUUID = new clsStationUID();
			stationUUID.NewGUIDS();
			// write bytes
			m_Frames_02.WriteDeviceUID(stationUUID.StationData);
			m_Frames_02.ReadDeviceUID();
		}
		
		public string GetStationPIN()
		{
			return m_StationData.Settings.PIN;
		}
		
		public void SetStationPIN(string newPIN)
		{
			if (newPIN.Length == 4)
			{
				m_Frames_02.WriteDevicePIN(newPIN);
				m_Frames_02.ReadDevicePIN();
			}
		}
		
		public OnOff GetContinuousSuction()
		{
			return m_StationData.Settings.ContinuousSuction;
		}
		
		public void SetContinuousSuction(OnOff suction)
		{
			m_Frames_02.WriteContinuousSuction(suction);
			m_Frames_02.ReadContinuousSuction();
		}
		
#endregion
		
		
#region Communications
		
#region Robot
		
		internal CRobotData GetRobotConfiguration()
		{
			return m_StationData.Settings.Robot;
		}
		
		internal void SetRobotConfiguration(CRobotData robotData)
		{
			
			// si la estación está en modo robot no acepta tramas de escritura excepto del status
			if (m_StationData.Settings.Robot.Status == OnOff._OFF)
			{
				m_Frames_02.WriteRobotConfiguration(robotData);
				m_Frames_02.ReadRobotConfiguration();
			}
			m_Frames_02.WriteRobotStatus(robotData.Status);
			m_Frames_02.ReadRobotStatus();
		}
		
		public OnOff GetRobotStatus()
		{
			return m_StationData.Settings.Robot.Status;
		}
		
#endregion
		
#endregion
		
#endregion
		
		
#region STATION FEATURES
		
		public CFeaturesData GetStationFeatures()
		{
			return m_StationData.Info.Features;
		}
		
#endregion
		
		
#region TRANSACTION
		
		public uint SetTransaction()
		{
			return m_Frames_02.MarkACK();
		}
		
#endregion
		
		
#region UPDATE DATA
		
		private void UpdateDataProcess()
		{
			while (m_ThreadUpdateDataAlive)
			{
				
				//
				//VERY HIGH SPEED
				//
				UpdateAllInfoPort();
				
				
				//
				//HIGH SPEED
				//
				if (m_ContUpdateDataHigh > 0)
				{
					m_ContUpdateDataHigh--;
				}
				else
				{
					UpdateStationError();
					
					m_ContUpdateDataHigh = HIGH_SPEED_UPDATE_DATA - 1;
				}
				
				
				//
				//MEDIUM SPEED
				//
				if (m_ContUpdateDataMedium > 0)
				{
					m_ContUpdateDataMedium--;
				}
				else
				{
					//Station Parameters
					UpdateStationParam();
					
					m_ContUpdateDataMedium = MEDIUM_SPEED_UPDATE_DATA - 1;
				}
				
				
				//
				//SLOW SPEED
				//
				if (m_ContUpdateDataSlow > 0)
				{
					m_ContUpdateDataSlow--;
				}
				else
				{
					
					m_ContUpdateDataSlow = SLOW_SPEED_UPDATE_DATA - 1;
				}
				
				
				//
				//INITIALLY
				//
				if (!m_IsDataInitialized && m_IdTransactionDataInitialized == UInt32.MaxValue)
				{
					
					//Asegurar que se han pedido todos los datos de la estación al inicializar
					m_IdTransactionDataInitialized = SetTransaction();
				}
				
				Thread.Sleep(TIME_UPDATE_DATA);
			}
		}
		
		private void UpdateAllInfoPort()
		{
			for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
			{
				m_Frames_02.ReadIntakeActivation((Port) idx, ToolStatus_FE.WORK);
				m_Frames_02.ReadIntakeActivation((Port) idx, ToolStatus_FE.STAND);
			}
			m_Frames_02.ReadContinuousSuction();
		}
		
		private void UpdateStationError()
		{
			m_Frames_02.ReadStationError();
		}
		
		private void UpdateStationParam()
		{
			//
			//General Configuration
			//
			m_Frames_02.ReadConnectStatus();
			
			//
			//Robot Configuration
			//
			UpdateRobot();
			UpdateRobotStatus();
		}
		
		private void UpdateRobot()
		{
			m_Frames_02.ReadRobotConfiguration();
		}
		
		private void UpdateRobotStatus()
		{
			m_Frames_02.ReadRobotStatus();
		}
		
#endregion
		
		
#region SUBSTATIONS
		
		private void Event_Detected_SubStation(CConnectionData connectionData)
		{
			if (Detected_SubStationEvent != null)
				Detected_SubStationEvent(UUID, connectionData);
		}
		
#endregion
		
		
#region STATION COMMUNICATION
		
		private void ConnectionError(EnumConnectError TypeError, byte address, byte command)
		{
			if (TypeError == EnumConnectError.INTERPRET)
			{
				if (UserErrorEvent != null) //TODO. revisar error
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication error in station.", new byte[] {}));
				
			}
			else if (TypeError == EnumConnectError.TIME_OUT)
			{
				if (address == m_StationNumDevice)
				{
					if (UserErrorEvent != null) //TODO. revisar error
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication timeout in station. Command:" + command.ToString("X"), new byte[] {}));
					if (StationDisconnectedEvent != null)
						StationDisconnectedEvent(UUID);
				}
				
			}
			else
			{
				if (StationDisconnectedEvent != null)
					StationDisconnectedEvent(UUID);
			}
		}
		
		public string GetStationCom()
		{
			return m_ComChannel.COMName();
		}
		
		public string GetStationConnectionType()
		{
			return m_ComChannel.COMType();
		}
		
#endregion
		
		
#region UPDATE FIRMWARE
		
		/// <summary>
		/// Get hardware version of all micros in the station
		/// </summary>
		/// <returns>Hardware version of all micros in the station</returns>
		internal List<CFirmwareStation> GetVersionMicrosFirmware()
		{
			List<CFirmwareStation> versionMicros = new List<CFirmwareStation>();
			
			//Add version
			foreach (DictionaryEntry stationMicroEntry in m_StationData.Info.StationMicros)
			{
				CFirmwareStation stationMicro = new CFirmwareStation();
				stationMicro.StationUUID = ((CFirmwareStation) stationMicroEntry.Value).StationUUID;
				stationMicro.Model = ((CFirmwareStation) stationMicroEntry.Value).Model;
				stationMicro.ModelVersion = ((CFirmwareStation) stationMicroEntry.Value).ModelVersion;
				stationMicro.ProtocolVersion = ((CFirmwareStation) stationMicroEntry.Value).ProtocolVersion;
				stationMicro.HardwareVersion = ((CFirmwareStation) stationMicroEntry.Value).HardwareVersion;
				stationMicro.SoftwareVersion = ((CFirmwareStation) stationMicroEntry.Value).SoftwareVersion;
				stationMicro.FileName = ((CFirmwareStation) stationMicroEntry.Value).FileName;
				
				versionMicros.Add(stationMicro);
			}
			
			return versionMicros;
		}
		
		/// <summary>
		/// Initialize the update process of all micros
		/// </summary>
		/// <param name="infoUpdateFirmware">Update firmware information</param>
		internal void UpdateMicrosFirmware(List<CFirmwareStation> infoUpdateFirmware)
		{
			
			//Check feature
			if (!m_StationData.Info.Features.FirmwareUpdate)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Update firmware not supported"));
				
			}
			else
			{
				if (ReferenceEquals(m_UpdateFirmware02, null))
				{
					m_UpdateFirmware02 = new CUpdateFirmware02(UUID, m_Frames_02);
                    //TODO TONI: Check this
                    //m_UpdateFirmware02.UpdateMicroFirmwareFinished += Event_UpdateMicroFirmwareFinished;
				}
				
				m_UpdateFirmware02.UpdateMicrosFirmware(infoUpdateFirmware, m_StationData.Info.StationMicros);
			}
		}
		
#endregion
		
	}
	
}
