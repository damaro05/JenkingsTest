// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;
using System.IO;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;
using DataJBC;
using RoutinesJBC;
using Constants = DataJBC.Constants;



namespace JBC_Connect
{


	internal class CStation_HA : CStationBase
	{

		private int TIME_WAIT_SYNC_PROFILE = 10000; //Tiempo máximo de espera para sincronizar perfiles (lectura) - 10 segundos
		private int LENGTH_PROFILE_FILE = 128;


		//Data
		private CStationData_HA m_StationData = new CStationData_HA(); //Información de la estación (Desoldadora)
		private CPortData_HA[] m_PortData = new CPortData_HA[0]; //Información del puerto (Desoldadora)

		//Frames
		private CStationFrames02_HA m_Frames_02;

		//Continuous mode queue
		private CContinuousModeQueueListStation_HA m_continuousModeQueueList;

		//Files
		private int m_indexProfileManaging = 0; //Indica el index del perfil que se está sincronizando
		private int m_sequenceProfileManaging = 0; //Indica el número de sequencia de perfil que se está sincronizando (lectura/escritura)
		private CProfileData_HA m_dataProfileManaging; //Contiene los datos del perfil que se está guardando
		private bool m_syncFilesLocked = false; //Indica si se están sincronizando los archivos (lectura/escritura): perfiles, etc..


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



		public CStation_HA(byte _StationNumDevice, Protocol _CommandProtocol, Protocol _FrameProtocol, string _StationModel, string _SoftwareVersion, string _HardwareVersion, CCommunicationChannel _ComChannel, string _ParentUUID = "")
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

			// set default continuous mode status
			m_startContModeStatus.speed = Constants.DEFAULT_STATION_CONTINUOUSMODE_SPEED;
			m_startContModeStatus.setAllPortsFromCount(m_PortData.Length);
			// creates continuous mode queue
			m_continuousModeQueueList = new CContinuousModeQueueListStation_HA(m_startContModeStatus.speed);
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
			m_Frames_02 = new CStationFrames02_HA(m_StationData, m_PortData, m_continuousModeQueueList, m_ComChannel, m_StationNumDevice);
			m_Frames_02.FileCount += Event_ResponseFileCount;
			m_Frames_02.FileName += Event_ResponseFileName;
			m_Frames_02.StartReadingFile += Event_ResponseStartReadingFile;
			m_Frames_02.BlockReadingFile += Event_ResponseBlockReadingFile;
			m_Frames_02.EndReadingFile += Event_ResponseEndReadingFile;
			m_Frames_02.StartWritingFile += Event_ResponseStartWritingFile;
			m_Frames_02.BlockWritingFile += Event_ResponseBlockWritingFile;
			m_Frames_02.EndWritingFile += Event_ResponseEndWrintingFile;
			m_Frames_02.Changed_StationParameters += UpdateStationParam;
			m_Frames_02.Changed_ToolParam += UpdateToolParam;
			m_Frames_02.Changed_SelectedTemperature += UpdateAllSelectedTempAndFlow;
			m_Frames_02.Changed_Counters += UpdateCounters;
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
			GenericStationTools[] ToolSoportadas = confStation.SupportedTools;

			m_StationData.Info.StationType = stationType;
			m_PortData = new CPortData_HA[iPorts - 1 + 1];

			//Recorremos todos los puertos de la estación
			for (int idxPort = 0; idxPort <= m_PortData.Length - 1; idxPort++)
			{
				m_PortData[idxPort] = new CPortData_HA(ToolSoportadas.Length, Constants.NUM_LEVELS_TEMP);

				//Para cada puero guardamos la tool
				for (int idxTool = 0; idxTool <= ToolSoportadas.Length - 1; idxTool++)
				{
					m_PortData[idxPort].ToolSettings[idxTool].Tool = ToolSoportadas[idxTool];
				}
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

		public OnOff GetEnabledPort(Port port)
		{
			OnOff enabled = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				enabled = m_PortData[(int)port].ToolStatus.EnabledPort;
			}

			return enabled;
		}

		public void SetEnabledPort(Port port, OnOff enabled)
		{
			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				//FIXME
				m_PortData[(int)port].ToolStatus.EnabledPort = enabled;
			}
		}

		public GenericStationTools GetPortToolID(Port port)
		{
			GenericStationTools genericTool = GenericStationTools.NO_TOOL;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				genericTool = m_PortData[(int)port].ToolStatus.ConnectedTool;
			}

			return genericTool;
		}

		public CTemperature GetPortToolSelectedTemp(Port port)
		{
			CTemperature temp = null;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				temp = new CTemperature(m_PortData[(int)port].ToolStatus.SelectedTemp.UTI);
			}

			return temp;
		}

		public void SetPortToolSelectedTemp(Port port, CTemperature temperature)
		{
			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				//Check temperature
				CTemperature auxMaxTemp = GetStationMaxTemp();
				CTemperature auxMinTemp = GetStationMinTemp();

				//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
				if (((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI)) && temperature.UTI != Constants.NO_TEMP_LEVEL)
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
				}
				else
				{
					//Switch protocol
					if (m_CommandProtocol == Protocol.Protocol_01)
					{
						//m_Frames_01.WriteSelectTemp(port, temperature)
						//m_Frames_01.ReadSelectTemp(port)
					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						m_Frames_02.WriteSelectTemp(port, temperature);
						m_Frames_02.ReadSelectTemp(port);
					}
				}
			}
		}

		public int GetPortToolSelectedFlow(Port port)
		{
			int ret = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				ret = m_PortData[(int)port].ToolStatus.SelectedFlow_x_Mil;
			}

			return ret;
		}

		public void SetPortToolSelectedFlow(Port port, int flow)
		{
			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					//m_Frames_01.WriteSelectTemp(port, temperature)
					//m_Frames_01.ReadSelectTemp(port)
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteSelectFlow(port, flow);
					m_Frames_02.ReadSelectFlow(port);
				}
			}
		}

		public CTemperature GetPortToolSelectedExternalTemp(Port port)
		{
			CTemperature temp = null;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				temp = new CTemperature(m_PortData[(int)port].ToolStatus.SelectedExtTemp.UTI);
			}

			return temp;
		}

		public void SetPortToolSelectedExternalTemp(Port port, CTemperature temperature)
		{
			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				//Check temperature
				CTemperature auxMaxTemp = GetStationMaxExtTemp();
				CTemperature auxMinTemp = GetStationMinExtTemp();

				//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
				if (((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI)) && temperature.UTI != Constants.NO_TEMP_LEVEL)
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
				}
				else
				{
					//Switch protocol
					if (m_CommandProtocol == Protocol.Protocol_01)
					{
						//m_Frames_01.WriteSelectExternalTemp(port, temperature)
						//m_Frames_01.ReadSelectExternalTemp(port)
					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						m_Frames_02.WriteSelectExternalTemp(port, temperature);
						m_Frames_02.ReadSelectExternalTemp(port);
					}
				}
			}
		}

		public CTemperature GetPortToolActualTemp(Port port)
		{
			CTemperature temp = null;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				temp = new CTemperature(m_PortData[(int)port].ToolStatus.ActualTemp.UTI);
			}

			return temp;
		}

		public int GetPortToolActualPower(Port port)
		{
			int power = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				power = m_PortData[(int)port].ToolStatus.Power_x_Mil;
			}

			return power;
		}

		public int GetPortToolActualFlow(Port port)
		{
			int power = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				power = m_PortData[(int)port].ToolStatus.Flow_x_Mil;
			}

			return power;
		}

		public CTemperature GetPortToolProtectionTC_Temp(Port port)
		{
			CTemperature temp = null;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				temp = new CTemperature(m_PortData[(int)port].ToolStatus.ProtectionTC_Temp.UTI);
			}

			return temp;
		}

		public CTemperature GetPortToolActualExternalTemp(Port port)
		{
			CTemperature temp = null;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				temp = new CTemperature(m_PortData[(int)port].ToolStatus.ActualExtTemp.UTI);
			}

			return temp;
		}

		public ToolError GetPortToolError(Port port)
		{
			ToolError toolErr = ToolError.NO_TOOL;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				toolErr = m_PortData[(int)port].ToolStatus.ToolError;
			}

			return toolErr;
		}

		public OnOff GetPortToolStandStatus(Port port)
		{
			OnOff standStatus = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				standStatus = m_PortData[(int)port].ToolStatus.Stand_OnOff;
			}

			return standStatus;
		}

		public OnOff GetPortToolPedalStatus(Port port)
		{
			OnOff status = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.PedalStatus_OnOff;
			}

			return status;
		}

		public OnOff GetPortToolPedalConnectedStatus(Port port)
		{
			OnOff status = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.PedalConnected_OnOff;
			}

			return status;
		}

		public OnOff GetPortToolSuctionRequestedStatus(Port port)
		{
			OnOff status = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.SuctionRequestedStatus_OnOff;
			}

			return status;
		}

		public OnOff GetPortToolSuctionStatus(Port port)
		{
			OnOff status = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.SuctionStatus_OnOff;
			}

			return status;
		}

		public OnOff GetPortToolHeaterRequestedStatus(Port port)
		{
			OnOff status = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.HeaterRequestedStatus_OnOff;
			}

			return status;
		}

		public OnOff GetPortToolHeaterStatus(Port port)
		{
			OnOff status = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.HeaterStatus_OnOff;
			}

			return status;
		}

		public OnOff GetPortToolCoolingStatus(Port port)
		{
			OnOff status = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.CoolingStatus_OnOff;
			}

			return status;
		}

		public int GetPortToolTimeToStopStatus(Port port)
		{
			int status = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				status = m_PortData[(int)port].ToolStatus.TimeToStop;
			}

			return status;
		}

		public OnOff GetPortToolProfileMode(Port port)
		{
			OnOff ret = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				ret = m_PortData[(int)port].ToolStatus.ProfileMode;
			}
			return ret;
		}

		public void SetPortToolProfileMode(Port port, OnOff onoff)
		{
			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				m_Frames_02.WriteProfileMode(port, onoff);
				m_Frames_02.ReadProfileMode(port);
			}
		}

		public void SetPortToolHeaterStatus(Port port, OnOff _OnOff)
		{
			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				if (_OnOff == OnOff._ON)
				{
					m_Frames_02.WriteHeaterStatus(port, PortHeaterStatus_HA.HEATER_ON);
				}
				else
				{
					m_Frames_02.WriteHeaterStatus(port, PortHeaterStatus_HA.HEATER_OFF);
				}
				m_Frames_02.ReadHeaterStatus(port);
			}
		}

		public void SetPortToolSuctionStatus(Port port, OnOff _OnOff)
		{
			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				if (_OnOff == OnOff._ON)
				{
					m_Frames_02.WriteSuctionStatus(port, PortSuctionStatus_HA.SUCTION_ON);
				}
				else
				{
					m_Frames_02.WriteSuctionStatus(port, PortSuctionStatus_HA.SUCTION_OFF);
				}
				m_Frames_02.ReadSuctionStatus(port);
			}
		}


#endregion


#region Port + Tool

		public void ResetPortToolSettings(Port port, GenericStationTools tool)
		{
			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				m_Frames_02.ResetPortTool(port, tool);
			}
		}

		public ToolTemperatureLevels GetPortToolSelectedTempLevel(Port port, GenericStationTools tool)
		{
			ToolTemperatureLevels toolTempLevel = ToolTemperatureLevels.NO_LEVELS;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				toolTempLevel = m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTempSelect;
			}

			return toolTempLevel;
		}

		public OnOff GetPortToolSelectedTempLevelsEnabled(Port port, GenericStationTools tool)
		{
			OnOff enabled = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				enabled = m_PortData[(int)port].ToolSettings[idx].Levels.LevelsOnOff;
			}

			return enabled;
		}

		public void SetPortToolSelectedTempLevels(Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{

				int idx = getToolIndex(tool);
				CTempLevelsData_HA stnToolLevels = (CTempLevelsData_HA)(m_PortData[(int)port].ToolSettings[idx].Levels.Clone());
				stnToolLevels.LevelsTempSelect = level;
				m_Frames_02.WriteLevelsTemps(port, tool,
						stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect,
						stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsFlow[0], stnToolLevels.LevelsExtTemp[0],
						stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsFlow[1], stnToolLevels.LevelsExtTemp[1],
						stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2], stnToolLevels.LevelsFlow[2], stnToolLevels.LevelsExtTemp[2]);
				m_Frames_02.ReadLevelsTemps(port, tool);
				stnToolLevels = null;
			}

		}

		public void SetPortToolSelectedTempLevelsEnabled(Port port, GenericStationTools tool, OnOff _onoff)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);

				CTempLevelsData_HA stnToolLevels = (CTempLevelsData_HA)(m_PortData[(int)port].ToolSettings[idx].Levels.Clone());
				stnToolLevels.LevelsOnOff = _onoff;
				m_Frames_02.WriteLevelsTemps(port, tool,
						stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect,
						stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsFlow[0], stnToolLevels.LevelsExtTemp[0],
						stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsFlow[1], stnToolLevels.LevelsExtTemp[1],
						stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2], stnToolLevels.LevelsFlow[2], stnToolLevels.LevelsExtTemp[2]);
				m_Frames_02.ReadLevelsTemps(port, tool);
				stnToolLevels = null;
			}
		}

		public CTemperature GetPortToolTempLevel(Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			CTemperature temp = new CTemperature(0);

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				temp.UTI = m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI;
			}

			return temp;
		}

		public int GetPortToolFlowLevel(Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			int flow = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				flow = System.Convert.ToInt32(m_PortData[(int)port].ToolSettings[idx].Levels.LevelsFlow[(int)level]);
			}

			return flow;
		}

		public CTemperature GetPortToolExternalTempLevel(Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			CTemperature temp = new CTemperature(0);

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				temp.UTI = m_PortData[(int)port].ToolSettings[idx].Levels.LevelsExtTemp[(int)level].UTI;
			}

			return temp;
		}

		public OnOff GetPortToolTempLevelEnabled(Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			OnOff enabled = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				enabled = m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTempOnOff[(int)level];
			}

			return enabled;
		}

		public void SetPortToolTempLevel(Port port, GenericStationTools tool, ToolTemperatureLevels level, CTemperature temperature)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{

				//Check temperature
				CTemperature auxMaxTemp = GetStationMaxTemp();
				CTemperature auxMinTemp = GetStationMinTemp();

				//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
				if (((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI)) && temperature.UTI != Constants.NO_TEMP_LEVEL)
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
				}
				else
				{
					int idx = getToolIndex(tool);
					CTempLevelsData_HA stnToolLevels = (CTempLevelsData_HA) (m_PortData[(int)port].ToolSettings[idx].Levels.Clone());

					if (level == ToolTemperatureLevels.FIRST_LEVEL)
					{
						stnToolLevels.LevelsTemp[0] = new CTemperature(temperature.UTI);

					}
					else if (level == ToolTemperatureLevels.SECOND_LEVEL)
					{
						stnToolLevels.LevelsTemp[1] = new CTemperature(temperature.UTI);

					}
					else if (level == ToolTemperatureLevels.THIRD_LEVEL)
					{
						stnToolLevels.LevelsTemp[2] = new CTemperature(temperature.UTI);
					}

					m_Frames_02.WriteLevelsTemps(port, tool,
							stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect,
							stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsFlow[0], stnToolLevels.LevelsExtTemp[0],
							stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsFlow[1], stnToolLevels.LevelsExtTemp[1],
							stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2], stnToolLevels.LevelsFlow[2], stnToolLevels.LevelsExtTemp[2]);
					m_Frames_02.ReadLevelsTemps(port, tool);
					stnToolLevels = null;
				}

			}
		}

		public void SetPortToolFlowLevel(Port port, GenericStationTools tool, ToolTemperatureLevels level, int flow)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				if ((flow > 1000) || (flow < 0))
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FLOW_OUT_OF_RANGE, "Flow out of range."));
				}
				else
				{
					int idx = getToolIndex(tool);
					CTempLevelsData_HA stnToolLevels = (CTempLevelsData_HA)(m_PortData[(int)port].ToolSettings[idx].Levels.Clone());

					if (level == ToolTemperatureLevels.FIRST_LEVEL)
					{
						stnToolLevels.LevelsFlow[0] = flow;

					}
					else if (level == ToolTemperatureLevels.SECOND_LEVEL)
					{
						stnToolLevels.LevelsFlow[1] = flow;

					}
					else if (level == ToolTemperatureLevels.THIRD_LEVEL)
					{
						stnToolLevels.LevelsFlow[2] = flow;
					}

					m_Frames_02.WriteLevelsTemps(port, tool,
							stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect,
							stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsFlow[0], stnToolLevels.LevelsExtTemp[0],
							stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsFlow[1], stnToolLevels.LevelsExtTemp[1],
							stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2], stnToolLevels.LevelsFlow[2], stnToolLevels.LevelsExtTemp[2]);
					m_Frames_02.ReadLevelsTemps(port, tool);
					stnToolLevels = null;
				}

			}
		}

		public void SetPortToolExternalTempLevel(Port port, GenericStationTools tool, ToolTemperatureLevels level, CTemperature temperature)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{

				//Check temperature
				CTemperature auxMaxTemp = GetStationMaxTemp();
				CTemperature auxMinTemp = GetStationMinTemp();

				//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
				if (((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI)) && temperature.UTI != Constants.NO_TEMP_LEVEL)
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
				}
				else
				{
					int idx = getToolIndex(tool);
					CTempLevelsData_HA stnToolLevels = (CTempLevelsData_HA)(m_PortData[(int)port].ToolSettings[idx].Levels.Clone());

					if (level == ToolTemperatureLevels.FIRST_LEVEL)
					{
						stnToolLevels.LevelsExtTemp[0] = new CTemperature(temperature.UTI);

					}
					else if (level == ToolTemperatureLevels.SECOND_LEVEL)
					{
						stnToolLevels.LevelsExtTemp[1] = new CTemperature(temperature.UTI);

					}
					else if (level == ToolTemperatureLevels.THIRD_LEVEL)
					{
						stnToolLevels.LevelsExtTemp[2] = new CTemperature(temperature.UTI);
					}

					m_Frames_02.WriteLevelsTemps(port, tool,
							stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect,
							stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsFlow[0], stnToolLevels.LevelsExtTemp[0],
							stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsFlow[1], stnToolLevels.LevelsExtTemp[1],
							stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2], stnToolLevels.LevelsFlow[2], stnToolLevels.LevelsExtTemp[2]);
					m_Frames_02.ReadLevelsTemps(port, tool);
					stnToolLevels = null;
				}

			}
		}

		public void SetPortToolTempLevelEnabled(Port port, GenericStationTools tool, ToolTemperatureLevels level, OnOff onoff)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);

				//Switch protocol

				CTempLevelsData_HA stnToolLevels = (CTempLevelsData_HA)(m_PortData[(int)port].ToolSettings[idx].Levels.Clone());

				if (level == ToolTemperatureLevels.FIRST_LEVEL)
				{
					stnToolLevels.LevelsTempOnOff[0] = onoff;

				}
				else if (level == ToolTemperatureLevels.SECOND_LEVEL)
				{
					stnToolLevels.LevelsTempOnOff[1] = onoff;

				}
				else if (level == ToolTemperatureLevels.THIRD_LEVEL)
				{
					stnToolLevels.LevelsTempOnOff[2] = onoff;
				}

				m_Frames_02.WriteLevelsTemps(port, tool,
						stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect,
						stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsFlow[0], stnToolLevels.LevelsExtTemp[0],
						stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsFlow[1], stnToolLevels.LevelsExtTemp[1],
						stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2], stnToolLevels.LevelsFlow[2], stnToolLevels.LevelsExtTemp[2]);
				m_Frames_02.ReadLevelsTemps(port, tool);
				stnToolLevels = null;
			}
		}

		public void SetPortToolLevels(Port port, GenericStationTools tool, OnOff LevelsOnOff, ToolTemperatureLevels LevelSelected, OnOff Level1OnOff, CTemperature Level1Temp, int flowLevel1, CTemperature tempExternalLevel1, OnOff Level2OnOff, CTemperature Level2Temp, int flowLevel2, CTemperature tempExternalLevel2, OnOff Level3OnOff, CTemperature Level3Temp, int flowLevel3, CTemperature tempExternalLevel3)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{

				//Check temperature
				CTemperature auxMaxTemp = GetStationMaxTemp();
				CTemperature auxMinTemp = GetStationMinTemp();

				if (((Level1Temp.UTI > auxMaxTemp.UTI) || (Level1Temp.UTI < auxMinTemp.UTI) && Level1Temp.UTI != Constants.NO_TEMP_LEVEL) ||
						((Level2Temp.UTI > auxMaxTemp.UTI) || (Level2Temp.UTI < auxMinTemp.UTI) && Level2Temp.UTI != Constants.NO_TEMP_LEVEL) ||
						((Level3Temp.UTI > auxMaxTemp.UTI) || (Level3Temp.UTI < auxMinTemp.UTI) && Level3Temp.UTI != Constants.NO_TEMP_LEVEL))
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
				}
				else
				{
					m_Frames_02.WriteLevelsTemps(port, tool,
							LevelsOnOff, LevelSelected,
							Level1OnOff, new CTemperature(Level1Temp.UTI), flowLevel1, new CTemperature(tempExternalLevel1.UTI),
							Level2OnOff, new CTemperature(Level2Temp.UTI), flowLevel2, new CTemperature(tempExternalLevel2.UTI),
							Level3OnOff, new CTemperature(Level3Temp.UTI), flowLevel3, new CTemperature(tempExternalLevel3.UTI));
					m_Frames_02.ReadLevelsTemps(port, tool);
				}

			}
		}

		public ToolExternalTCMode_HA GetPortToolExternalTCMode(Port port, GenericStationTools tool)
		{
			ToolExternalTCMode_HA mode = default(ToolExternalTCMode_HA);

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				mode = m_PortData[(int)port].ToolSettings[idx].ExternalTCMode;
			}

			return mode;
		}

		public void SetPortToolExternalTCMode(Port port, GenericStationTools tool, ToolExternalTCMode_HA mode)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				m_Frames_02.WriteExternalTCMode(port, tool, mode);
				m_Frames_02.ReadExternalTCMode(port, tool);
			}
		}

		public int GetPortToolTimeToStop(Port port, GenericStationTools tool)
		{
			int value = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				value = m_PortData[(int)port].ToolSettings[idx].TimeToStop;
			}

			return value;
		}

		public void SetPortToolTimeToStop(Port port, GenericStationTools tool, int value)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

				//Check TimeToStop
			}
			else if (value < 0 | value > Constants.MAX_TIME_TO_STOP)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TIME_OUT_OF_RANGE, "Time not in range."));

			}
			else
			{
				m_Frames_02.WriteTimeToStop(port, tool, value);
				m_Frames_02.ReadTimeToStop(port, tool);
			}
		}

		public CTemperature GetPortToolAdjustTemp(Port port, GenericStationTools tool)
		{
			CTemperature temp = new CTemperature(0);

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				temp.UTI = m_PortData[(int)port].ToolSettings[idx].AdjustTemp.UTI;
			}

			return temp;
		}

		public void SetPortToolAdjustTemp(Port port, GenericStationTools tool, CTemperature temperature)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

				// 10/12/2012 Se cambia verificación de límites (antes MIN_TEMP Y MAX_TEMP) - Edu
			}
			else if (temperature.UTI > Constants.DEFAULT_TEMP_AJUST | temperature.UTI < (Constants.DEFAULT_TEMP_AJUST * -1))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));

			}
			else
			{

				//Switch protocol
				m_Frames_02.WriteAjustTemp(port, tool, temperature);
				m_Frames_02.ReadAjustTemp(port, tool);
			}
		}

		public CToolStartMode_HA GetPortToolStartMode(Port port, GenericStationTools tool)
		{
			CToolStartMode_HA value = new CToolStartMode_HA();

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				value = m_PortData[(int)port].ToolSettings[idx].StartMode;
			}

			return value;
		}

		public void SetPortToolStartMode(Port port, GenericStationTools tool, CToolStartMode_HA value)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check tool
			}
			else if (!isToolSupported(tool))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TOOL_NOT_SUPPORTED, "Tool not supported by this station model."));

			}
			else
			{
				m_Frames_02.WriteStartMode(port, tool, value);
				m_Frames_02.ReadStartMode(port, tool);
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

		public OnOff GetRemoteMode()
		{
			return m_StationData.Status.RemoteMode;
		}

		public void SetRemoteMode(OnOff _OnOff)
		{
			m_Frames_02.WriteRemoteMode(_OnOff);
			m_Frames_02.ReadRemoteMode();
		}

#endregion


#region Continuous mode

		public uint StartContinuousMode(SpeedContinuousMode captureSpeed = default(SpeedContinuousMode))
		{
			// if continuous mode off, set station speed in queue list
			if (m_StationData.Status.ContinuousModeStatus.speed == SpeedContinuousMode.OFF)
			{
				m_continuousModeQueueList.retrieveSpeed = m_startContModeStatus.speed;
			}

			// new queue
			if (captureSpeed == SpeedContinuousMode.OFF)
			{
				captureSpeed = ((CContinuousModeStatus) (m_startContModeStatus.Clone())).speed;
			}
			uint uiNewTraceID = m_continuousModeQueueList.NewQueue(captureSpeed);

			// if continuous mode off, start continuous mode
			if (m_StationData.Status.ContinuousModeStatus.speed == SpeedContinuousMode.OFF)
			{
				m_Frames_02.WriteContiMode(m_startContModeStatus.getByteFromPorts(), m_startContModeStatus.speed);
				m_Frames_02.ReadContiMode();
			}

			return uiNewTraceID;
		}

		public void StopContinuousMode(uint traceID)
		{
			// delete trace instance
			m_continuousModeQueueList.DeleteQueue(traceID);

			// if no more traces, stop
			if (m_continuousModeQueueList.QueueCount() == 0)
			{
				m_Frames_02.WriteContiMode(m_startContModeStatus.getByteFromPorts(), SpeedContinuousMode.OFF);
				m_Frames_02.ReadContiMode();
			}
		}

		public CContinuousModeStatus GetContinuousMode()
		{
			CContinuousModeStatus status = (CContinuousModeStatus) (m_StationData.Status.ContinuousModeStatus.Clone());
			return status;
		}

		public void SetContinuousMode(CContinuousModeStatus statusContMode)
		{
			//Check speed and ports
			if (statusContMode.speed != SpeedContinuousMode.OFF & statusContMode.getByteFromPorts() == 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.CONTINUOUS_MODE_ON_WITHOUT_PORTS, "At least one port must be indicated when activating continuous mode"));
			}
			else
			{
				if (statusContMode.speed == SpeedContinuousMode.OFF)
				{
					statusContMode.speed = Constants.DEFAULT_STATION_CONTINUOUSMODE_SPEED;
				}
				// save ports and speed to use in next StartContinuousMode
				m_startContModeStatus = statusContMode;
			}
		}

		public SpeedContinuousMode GetContinuousModeCaptureSpeed(uint traceID)
		{
			return m_continuousModeQueueList.CaptureSpeed(traceID);
		}

		public int GetContinuousModeDataCount(uint traceID)
		{
			return m_continuousModeQueueList.DataCount(traceID);
		}

		public stContinuousModeData_HA GetContinuousModeNextData(uint traceID)
		{
			return m_continuousModeQueueList.Queue(traceID).GetData();
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

		public GenericStationTools[] GetStationTools()
		{
			//getting the supported tools
			GenericStationTools[] tools = null;
			tools = new GenericStationTools[m_PortData[0].ToolSettings.Length - 1 + 1];

			for (int cnt = 0; cnt <= m_PortData[0].ToolSettings.Length - 1; cnt++)
			{
				if (m_PortData[0].ToolSettings[cnt].Tool != GenericStationTools.NO_TOOL)
				{
					tools[cnt] = m_PortData[0].ToolSettings[cnt].Tool;
				}
				else
				{
					Array.Resize(ref tools, tools.Length - 2 + 1);
				}
			}

			return tools;
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
				return;
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

		public OnOff GetStationPINEnabled()
		{
			return m_StationData.Settings.PINEnabled;
		}

		public void SetStationPINEnabled(OnOff onoff)
		{
			m_Frames_02.WritePINEnabled(onoff);
			m_Frames_02.ReadPINEnabled();
		}


		// temp
		public CTemperature GetStationMaxTemp()
		{
			return new CTemperature(m_StationData.Settings.MaxTemp.UTI);
		}

		public void SetStationMaxTemp(CTemperature temperature)
		{

			//Check temperature
			CTemperature auxMaxTemp = Features_MaxTemp();
			CTemperature auxMinTemp = Features_MinTemp();

			//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
			if (((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI)) && temperature.UTI != Constants.NO_TEMP_LEVEL)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));

			}
			else
			{
				m_StationData.Settings.MaxTemp.UTI = temperature.UTI;
				m_Frames_02.WriteMaxMinTemp(temperature, GetStationMinTemp());
				m_Frames_02.ReadMaxMinTemp();
			}
		}

		public CTemperature GetStationMinTemp()
		{
			return new CTemperature(m_StationData.Settings.MinTemp.UTI);
		}

		public void SetStationMinTemp(CTemperature temperature)
		{

			//Check temperature
			CTemperature auxMaxTemp = Features_MaxTemp();
			CTemperature auxMinTemp = Features_MinTemp();

			//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
			if (((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI)) && temperature.UTI != Constants.NO_TEMP_LEVEL)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));

			}
			else
			{
				m_StationData.Settings.MinTemp.UTI = temperature.UTI;
				m_Frames_02.WriteMaxMinTemp(GetStationMaxTemp(), temperature);
				m_Frames_02.ReadMaxMinTemp();
			}
		}

		public void SetStationMaxMinTemp(CTemperature temperatureMax, CTemperature temperatureMin)
		{

			//Check temperature
			CTemperature auxMaxTemp = Features_MaxTemp();
			CTemperature auxMinTemp = Features_MinTemp();

			//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
			if ((temperatureMax.UTI > auxMaxTemp.UTI) || (temperatureMax.UTI < auxMinTemp.UTI) ||
					(temperatureMin.UTI > auxMaxTemp.UTI) || (temperatureMin.UTI < auxMinTemp.UTI))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));

			}
			else
			{
				m_Frames_02.WriteMaxMinTemp(temperatureMax, temperatureMin);
				m_Frames_02.ReadMaxMinTemp();
			}
		}

		// flow
		public int GetStationMaxFlow()
		{
			return m_StationData.Settings.MaxFlow;
		}

		public void SetStationMaxFlow(int flow)
		{

			if ((flow > 1000) || (flow < 0))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FLOW_OUT_OF_RANGE, "Flow out of range."));

			}
			else
			{
				m_StationData.Settings.MaxFlow = flow;
				m_Frames_02.WriteMaxMinFlow(flow, GetStationMinFlow());
				m_Frames_02.ReadMaxMinFlow();
			}
		}

		public int GetStationMinFlow()
		{
			return m_StationData.Settings.MinFlow;
		}

		public void SetStationMinFlow(int flow)
		{

			if ((flow > 1000) || (flow < 0))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FLOW_OUT_OF_RANGE, "Flow out of range."));

			}
			else
			{
				m_StationData.Settings.MinFlow = flow;
				m_Frames_02.WriteMaxMinFlow(GetStationMaxFlow(), flow);
				m_Frames_02.ReadMaxMinFlow();
			}
		}

		public void SetStationMaxMinFlow(int flowMax, int flowMin)
		{

			if ((flowMax > 1000) || (flowMax < 0) ||
					(flowMin > 1000) || (flowMin < 0))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FLOW_OUT_OF_RANGE, "Flow out of range."));

			}
			else
			{
				m_Frames_02.WriteMaxMinFlow(flowMax, flowMin);
				m_Frames_02.ReadMaxMinFlow();
			}
		}

		// external temp
		public CTemperature GetStationMaxExtTemp()
		{
			return new CTemperature(m_StationData.Settings.MaxExtTemp.UTI);
		}

		public void SetStationMaxExtTemp(CTemperature temperature)
		{

			//Check temperature
			CTemperature auxMaxTemp = m_StationData.Info.Features.ExtTCMaxTemp;
			CTemperature auxMinTemp = m_StationData.Info.Features.ExtTCMinTemp;

			if ((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));

			}
			else
			{
				m_StationData.Settings.MaxExtTemp.UTI = temperature.UTI;
				m_Frames_02.WriteMaxMinExternalTemp(temperature, GetStationMinExtTemp());
				m_Frames_02.ReadMaxMinExternalTemp();
			}
		}

		public CTemperature GetStationMinExtTemp()
		{
			return new CTemperature(m_StationData.Settings.MinExtTemp.UTI);
		}

		public void SetStationMinExtTemp(CTemperature temperature)
		{

			//Check temperature
			CTemperature auxMaxTemp = m_StationData.Info.Features.ExtTCMaxTemp;
			CTemperature auxMinTemp = m_StationData.Info.Features.ExtTCMinTemp;

			if ((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));

			}
			else
			{
				m_StationData.Settings.MinExtTemp.UTI = temperature.UTI;
				m_Frames_02.WriteMaxMinExternalTemp(GetStationMaxExtTemp(), temperature);
				m_Frames_02.ReadMaxMinExternalTemp();
			}
		}

		public CTemperature.TemperatureUnit GetStationTempUnits()
		{
			return m_StationData.Settings.Unit;
		}

		public void SetStationTempUnits(CTemperature.TemperatureUnit units)
		{
			m_Frames_02.WriteTempUnit(units);
			m_Frames_02.ReadTempUnit();
		}

		public OnOff GetStationBeep()
		{
			return m_StationData.Settings.Beep;
		}

		public void SetStationBeep(OnOff beep)
		{
			m_Frames_02.WriteBeep(beep);
			m_Frames_02.ReadBeep();
		}

		public OnOff GetStationLocked()
		{
			return m_StationData.Settings.StationLocked;
		}

		public void SetStationLocked(OnOff locked)
		{
			m_Frames_02.WriteStationLocked(locked);
			m_Frames_02.ReadStationLocked();
		}

#endregion


#region File methods

		private void Event_ResponseFileCount(int count)
		{
			m_StationData.Settings.Profiles.Clear();
			for (int i = 0; i <= count - 1; i++)
			{
				m_StationData.Settings.Profiles.Add(new CProfileData_HA());
			}
			m_indexProfileManaging = 0;

			//La lista está vacia así que empezamos actualizando el primer fichero
			if (count > 0)
			{
				m_Frames_02.ReadFileName(m_indexProfileManaging);
			}
		}

		private void Event_ResponseFileName(string fileName)
		{
			m_StationData.Settings.Profiles[m_indexProfileManaging].Name = fileName;
			m_Frames_02.ReadFile_Start(fileName);
		}

		//reading
		
		private void Event_ResponseStartReadingFile(bool bACK, int bytesCount)
		{
			if (!bACK)
			{
				return;
			}

			//Leer primer bloque
			m_sequenceProfileManaging = 0;
			m_Frames_02.ReadFile_Block(m_sequenceProfileManaging);
		}

		private void Event_ResponseBlockReadingFile(int sequence, byte[] data)
		{
			if (data.Length > 0)
			{
				int startIndex = System.Convert.ToInt32(m_StationData.Settings.Profiles[m_indexProfileManaging].Data.Length);
				Array.Resize(ref m_StationData.Settings.Profiles[m_indexProfileManaging].Data, m_StationData.Settings.Profiles[m_indexProfileManaging].Data.Length + data.Length - 1 + 1);
				Array.Copy(data, 0, (System.Array) (m_StationData.Settings.Profiles[m_indexProfileManaging].Data), startIndex, data.Length);

				//Leer siguiente bloque
				m_sequenceProfileManaging++;
				m_Frames_02.ReadFile_Block(m_sequenceProfileManaging);
			}
			else
			{
				m_Frames_02.ReadFile_End();
			}
		}

		private void Event_ResponseEndReadingFile()
		{
			m_indexProfileManaging++;

			//Leemos el siguiente profile
			if (m_indexProfileManaging < m_StationData.Settings.Profiles.Count)
			{
				m_Frames_02.ReadFileName(m_indexProfileManaging);
			}
			else
			{
				m_syncFilesLocked = false;
			}
		}

		//writing

		private void Event_ResponseStartWritingFile(bool bACK)
		{
			if (!bACK)
			{
				return ;
			}

			//Escribir primer bloque
			m_sequenceProfileManaging = 0;
			ProcessWriteFile();
		}

		private void Event_ResponseBlockWritingFile(int sequence, bool bACK)
		{
			if (!bACK)
			{
				return ;
			}

			m_sequenceProfileManaging++;
			ProcessWriteFile();
		}

		private void ProcessWriteFile()
		{
			int dataLength = 0;
			if (LENGTH_PROFILE_FILE * (m_sequenceProfileManaging + 1) < m_dataProfileManaging.Data.Length)
			{
				dataLength = LENGTH_PROFILE_FILE;
			}
			else
			{
				dataLength = m_dataProfileManaging.Data.Length - LENGTH_PROFILE_FILE * m_sequenceProfileManaging;
			}

			if (dataLength > 0)
			{
				byte[] data = new byte[dataLength - 1 + 1];
				Array.Copy(m_dataProfileManaging.Data, m_sequenceProfileManaging * LENGTH_PROFILE_FILE, data, 0, dataLength);
				m_Frames_02.WriteFile_Block(m_sequenceProfileManaging, data);
			}
			else
			{
				m_Frames_02.WriteFile_End();
			}
		}

		private void Event_ResponseEndWrintingFile()
		{
			m_syncFilesLocked = false;
		}

#endregion


#region Counters

#region Global Counters

		public int GetStationPluggedMinutes()
		{
			return m_PortData[0].Counters.ContPlugMinutes;
		}

		public int GetPortToolPluggedMinutes(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].Counters.ContPlugMinutes;
			}

			return counter;
		}

		public int GetPortToolWorkMinutes(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].Counters.ContWorkMinutes;
			}

			return counter;
		}

		public int GetPortToolIdleMinutes(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].Counters.ContIdleMinutes; // calculado
			}

			return counter;
		}

		public int GetPortToolWorkCycles(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].Counters.ContWorkCycles;
			}

			return counter;
		}

		public int GetPortToolSuctionCycles(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].Counters.ContSuctionCycles;
			}

			return counter;
		}

		internal void ResetPortToolMinutesGlobalPorts()
		{
			// added Edu 22/04/2016
			//Check port
			int[] values = new int[] {0, 0, 0, 0};

			for (var Port = 0; Port <= NumPorts - 1; Port++)
			{
				// reset minutes
				m_Frames_02.WriteWorkTime((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
				m_Frames_02.WritePlugTime((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
				// WriteIdleTime is calculated in software, do not reset
				// reset cycles
				m_Frames_02.WriteWorkCycles((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
				m_Frames_02.WriteSuctionCycles((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);

				// read minutes
				m_Frames_02.ReadPlugTime((Port) Port, CounterTypes.GLOBAL_COUNTER);
				m_Frames_02.ReadWorkTime((Port) Port, CounterTypes.GLOBAL_COUNTER);
				// read cycles
				m_Frames_02.ReadWorkCycles((Port) Port, CounterTypes.GLOBAL_COUNTER);
				m_Frames_02.ReadSuctionCycles((Port) Port, CounterTypes.GLOBAL_COUNTER);

			}
		}

#endregion


#region Partial Counters

		public int GetStationPluggedMinutesPartial()
		{
			return m_PortData[0].PartialCounters.ContPlugMinutes;
		}

		public int GetPortToolPluggedMinutesPartial(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].PartialCounters.ContPlugMinutes;
			}

			return counter;
		}

		public int GetPortToolWorkMinutesPartial(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].PartialCounters.ContWorkMinutes;
			}

			return counter;
		}

		public int GetPortToolIdleMinutesPartial(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].PartialCounters.ContIdleMinutes; // calculated
			}

			return counter;
		}

		public int GetPortToolWorkCyclesPartial(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].PartialCounters.ContWorkCycles;
			}

			return counter;
		}

		public int GetPortToolSuctionCyclesPartial(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].PartialCounters.ContSuctionCycles;
			}

			return counter;
		}

		public void ResetPortToolMinutesPartial(Port port)
		{

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{

				//Reset
				m_Frames_02.WriteWorkTime(port, CounterTypes.PARTIAL_COUNTER, 0);
				m_Frames_02.WritePlugTime(port, CounterTypes.PARTIAL_COUNTER, 0);
				// WriteIdleTime is calculated in software, do not reset
				m_Frames_02.WriteWorkCycles(port, CounterTypes.PARTIAL_COUNTER, 0);
				m_Frames_02.WriteSuctionCycles(port, CounterTypes.PARTIAL_COUNTER, 0);

				//Read
				m_Frames_02.ReadPlugTime(port, CounterTypes.PARTIAL_COUNTER);
				m_Frames_02.ReadWorkTime(port, CounterTypes.PARTIAL_COUNTER);
				m_Frames_02.ReadWorkCycles(port, CounterTypes.PARTIAL_COUNTER);
				m_Frames_02.ReadSuctionCycles(port, CounterTypes.PARTIAL_COUNTER);
			}
		}

#endregion

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


#region Profiles


		public List<CProfileData_HA> GetProfileList()
		{
			return m_StationData.Settings.Profiles;
		}

		public string GetSelectedProfile()
		{
			return m_StationData.Settings.SelectedProfile;
		}
	
		public bool SetProfile(CProfileData_HA profile)
		{
			if (m_syncFilesLocked)
			{
				return false;
			}

			m_syncFilesLocked = true;
			m_dataProfileManaging = profile;
			m_Frames_02.WriteFile_Start(m_dataProfileManaging.Name, m_dataProfileManaging.Data.Length);

			return true;
		}

		public void DeleteProfile(string profileName)
		{
			m_Frames_02.DeleteFile(profileName);
		}

		public void SyncProfiles()
		{
			if (m_syncFilesLocked)
			{
				return;
			}

			m_syncFilesLocked = true;
			m_Frames_02.ReadFileCount(Constants.PROFILES_FILE_EXTENSION);
		}

		public bool SyncFinishedProfiles()
		{
			return !m_syncFilesLocked;
		}

#endregion

#endregion


#region STATION FEATURES

		public CFeaturesData GetStationFeatures()
		{
			return m_StationData.Info.Features;
		}

		public CTemperature Features_MaxTemp()
		{
			return m_StationData.Info.Features.MaxTemp;
		}

		public CTemperature Features_MinTemp()
		{
			return m_StationData.Info.Features.MinTemp;
		}

		public int Features_MaxPowerLimit()
		{
			return m_StationData.Info.Features.MaxPowerLimit;
		}

		public bool Features_TempLevelsWithStatus()
		{
			return m_StationData.Info.Features.TempLevelsWithStatus;
		}

		public bool Features_FirmwareUpdate
		{
			get
			{
				return m_StationData.Info.Features.FirmwareUpdate;
			}
			set
			{
				m_StationData.Info.Features.FirmwareUpdate = value;
			}
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
				//Información que se pide cada 500ms, como la temperatura de la punta
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
					if (!m_StationData.Info.Features.ChangesStatusInformation || !m_IsDataInitialized)
					{

						//Station Parameters
						UpdateStationParam();

						//Selected temperature
						UpdateAllSelectedTempAndFlow();

						//Tool Parameters
						UpdateAllToolParam();
					}

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
					UpdateCounters();

					m_ContUpdateDataSlow = SLOW_SPEED_UPDATE_DATA - 1;
				}


				//Para que los PC lentos puedan funcionar en modo contínuo se debe bajar la velocidad
				int timeUpdateData = 0;
				switch (m_StationData.Status.ContinuousModeStatus.speed)
				{
					case SpeedContinuousMode.T_10mS:
						timeUpdateData = TIME_UPDATE_DATA * 2;
						break;
					case SpeedContinuousMode.T_20mS:
						timeUpdateData = (int)((double) TIME_UPDATE_DATA * 1.5);
						break;
					case SpeedContinuousMode.T_50mS:
						timeUpdateData = TIME_UPDATE_DATA * 1;
						break;
					default:
						timeUpdateData = TIME_UPDATE_DATA;
						break;
				}


				//
				//INITIALLY
				//
				if (!m_IsDataInitialized && m_IdTransactionDataInitialized == UInt32.MaxValue)
				{

					//Pedimos todos los perfiles
					m_Frames_02.ReadFileCount(Constants.PROFILES_FILE_EXTENSION);

					//Asegurar que se han pedido todos los datos de la estación al inicializar
					m_IdTransactionDataInitialized = SetTransaction();
				}

				Thread.Sleep(timeUpdateData);
			}
		}

		private void UpdateAllInfoPort()
		{
			for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
			{
				m_Frames_02.ReadInfoPort((Port) idx);
				m_Frames_02.ReadExternalAirTemp((Port) idx);
			}
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
			m_Frames_02.ReadDeviceName();
			m_Frames_02.ReadDevicePIN();
			m_Frames_02.ReadMaxMinTemp();
			m_Frames_02.ReadMaxMinFlow();
			m_Frames_02.ReadMaxMinExternalTemp();
			m_Frames_02.ReadPINEnabled();
			m_Frames_02.ReadStationLocked();
			m_Frames_02.ReadSelectedFile();
			m_Frames_02.ReadTempUnit();
			m_Frames_02.ReadBeep();
			//m_Frames_02.ReadLanguage() ' FALTA y en la estación no responde. quizás no haga falta implementarlo
			//m_Frames_02.ReadDateAndTime() ' FALTA y en la estación devuelve ASCII, quizás no haga falta implementarlo
			m_Frames_02.ReadConnectStatus();

			//
			//Robot Configuration
			//
			UpdateRobot();
		}

		private void UpdateAllToolParam()
		{
			for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
			{
				UpdateToolParam(System.Convert.ToInt32(idx));
			}
		}

		private void UpdateToolParam(int idx)
		{
			foreach (CToolSettingsData_HA ToolIn in m_PortData[idx].ToolSettings)
			{
				if (ToolIn.Tool != GenericStationTools.NO_TOOL)
				{
					// la versión JTSE_CAP_01 probablemente no saldrá con niveles
					if (m_StationData.Info.Features.TempLevels)
					{
						m_Frames_02.ReadLevelsTemps((Port) idx, ToolIn.Tool);
					}
					m_Frames_02.ReadAjustTemp((Port) idx, ToolIn.Tool);
					m_Frames_02.ReadTimeToStop((Port) idx, ToolIn.Tool);
					m_Frames_02.ReadExternalTCMode((Port) idx, ToolIn.Tool);
					m_Frames_02.ReadStartMode((Port) idx, ToolIn.Tool);
				}
			}
		}

		private void UpdateAllSelectedTempAndFlow()
		{
			for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
			{
				m_Frames_02.ReadSelectTemp((Port)idx);
				m_Frames_02.ReadSelectFlow((Port)idx);
				m_Frames_02.ReadSelectExternalTemp((Port)idx);
				m_Frames_02.ReadProfileMode((Port)idx);
			}
		}

		private void UpdateRobot()
		{
			if (m_StationData.Info.Features.Robot)
			{
				m_Frames_02.ReadRobotConfiguration();
				m_Frames_02.ReadRobotStatus();
			}
		}

		private void UpdateCounters()
		{
			for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
			{
				// Global
				m_Frames_02.ReadPlugTime((Port) idx, CounterTypes.GLOBAL_COUNTER);
				m_Frames_02.ReadWorkTime((Port) idx, CounterTypes.GLOBAL_COUNTER);
				m_Frames_02.ReadWorkCycles((Port) idx, CounterTypes.GLOBAL_COUNTER);
				m_Frames_02.ReadSuctionCycles((Port) idx, CounterTypes.GLOBAL_COUNTER);
				//Partial
				if (m_StationData.Info.Features.PartialCounters)
				{
					m_Frames_02.ReadPlugTime((Port) idx, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadWorkTime((Port) idx, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadWorkCycles((Port) idx, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadSuctionCycles((Port) idx, CounterTypes.PARTIAL_COUNTER);
				}
			}

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
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication error in station.", new byte[] { }));

			}
			else if (TypeError == EnumConnectError.TIME_OUT)
			{
				if (address == m_StationNumDevice)
				{
					if (UserErrorEvent != null) //TODO. revisar error
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication timeout in station. Command:" + command.ToString("X"), new byte[] { }));
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
				stationMicro.Model = ((CFirmwareStation)stationMicroEntry.Value).Model;
				stationMicro.ModelVersion = ((CFirmwareStation)stationMicroEntry.Value).ModelVersion;
				stationMicro.ProtocolVersion = ((CFirmwareStation)stationMicroEntry.Value).ProtocolVersion;
				stationMicro.HardwareVersion = ((CFirmwareStation)stationMicroEntry.Value).HardwareVersion;
				stationMicro.SoftwareVersion = ((CFirmwareStation)stationMicroEntry.Value).SoftwareVersion;
				stationMicro.FileName = ((CFirmwareStation)stationMicroEntry.Value).FileName;

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


#region Station Private methods

		private int getToolIndex(GenericStationTools tool)
		{
			//looking for the tool in the tool param vector
			int cnt = 0;

			while (cnt < m_PortData[0].ToolSettings.Length)
			{
				if (m_PortData[0].ToolSettings[cnt].Tool == tool)
				{
					return cnt;
				}
				cnt++;
			}

			return -1;
		}

		private bool isToolSupported(GenericStationTools tool)
		{
			return getToolIndex(tool) != -1;
		}

#endregion

	}

}
