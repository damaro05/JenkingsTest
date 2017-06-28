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


	internal class CStation_SOLD : CStationBase
	{

		//Data
		private CStationData_SOLD m_StationData = new CStationData_SOLD(); //Información de la estación
		private CPortData_SOLD[] m_PortData = new CPortData_SOLD[0]; //Información del puerto

		//Frames
		private CStationFrames01_SOLD m_Frames_01;
		private CStationFrames02_SOLD m_Frames_02;

		// continuous mode queue
		private CContinuousModeQueueListStation_SOLD m_continuousModeQueueList;


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



		public CStation_SOLD(byte _StationNumDevice, Protocol _CommandProtocol, Protocol _FrameProtocol, string _StationModel, string _SoftwareVersion, string _HardwareVersion, CCommunicationChannel _ComChannel, string _ParentUUID = "")
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
			m_continuousModeQueueList = new CContinuousModeQueueListStation_SOLD(m_startContModeStatus.speed);
		}

		public void Dispose()
		{

			//Terminate threads
			m_ThreadUpdateDataAlive = false;
			m_ThreadSearchSubStationsAlive = false;
			m_ThreadCheckDataInitializedAlive = false;

			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.Dispose();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.Dispose();
			}
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
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01 = new CStationFrames01_SOLD(m_StationData, m_PortData, m_continuousModeQueueList, m_ComChannel, m_StationNumDevice);
				m_Frames_01.Changed_StationParameters += UpdateStationParam;
				m_Frames_01.Changed_ToolParam += UpdateToolParam;
				m_Frames_01.Changed_SelectedTemperature += UpdateAllSelectedTemperature;
				m_Frames_01.Changed_Counters += UpdateCounters;
				m_Frames_01.ConnectionError += ConnectionError;
				m_Frames_01.ResetSended += ResetSended;
				m_Frames_01.EndedTransaction += AddEndedTransaction;
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02 = new CStationFrames02_SOLD(m_StationData, m_PortData, m_continuousModeQueueList, m_ComChannel, m_StationNumDevice);
				m_Frames_02.Changed_StationParameters += UpdateStationParam;
				m_Frames_02.Changed_ToolParam += UpdateToolParam;
				m_Frames_02.Changed_SelectedTemperature += UpdateAllSelectedTemperature;
				m_Frames_02.Changed_Counters += UpdateCounters;
				m_Frames_02.Detected_SubStation += Event_Detected_SubStation;
				m_Frames_02.ConnectionError += ConnectionError;
				m_Frames_02.ResetSended += ResetSended;
				m_Frames_02.EndedTransaction += AddEndedTransaction;
			}

			//Initialize UUID if empty
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.ReadDeviceUID();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.ReadDeviceUID();
			}

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
			m_PortData = new CPortData_SOLD[iPorts - 1 + 1];

			//Recorremos todos los puertos de la estación
			for (int idxPort = 0; idxPort <= m_PortData.Length - 1; idxPort++)
			{
				m_PortData[idxPort] = new CPortData_SOLD(ToolSoportadas.Length, Constants.NUM_LEVELS_TEMP);

				//Para cada puerto guardamos la tool
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
						m_Frames_01.WriteSelectTemp(port, temperature);
						m_Frames_01.ReadSelectTemp(port);
					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						m_Frames_02.WriteSelectTemp(port, temperature);
						m_Frames_02.ReadSelectTemp(port);
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
				CTemperature A = new CTemperature(m_PortData[(int)port].ToolStatus.ActualTemp[0].UTI);
				CTemperature B = new CTemperature(m_PortData[(int)port].ToolStatus.ActualTemp[1].UTI);

				if ((GetPortToolID(port) == GenericStationTools.HT |
							GetPortToolID(port) == GenericStationTools.PA) &&
						!m_StationData.Info.Features.SelectedTempTweezersValueCero)
				{
					temp = new CTemperature(System.Convert.ToInt32((A.UTI + B.UTI) / 2));
				}
				else
				{
					temp = new CTemperature(A.UTI);
				}
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
				int A = System.Convert.ToInt32(m_PortData[(int)port].ToolStatus.Power_x_Mil[0]);
				int B = System.Convert.ToInt32(m_PortData[(int)port].ToolStatus.Power_x_Mil[1]);

				if (GetPortToolID(port) == GenericStationTools.HT |
						GetPortToolID(port) == GenericStationTools.PA)
				{
					power = System.Convert.ToInt32((A + B) / 2);
				}
				else
				{
					power = A;
				}
			}

			return power;
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

		public int GetPortToolCartridgeCurrent(Port port)
		{
			int cartridge = -1;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				cartridge = System.Convert.ToInt32(m_PortData[(int)port].ToolStatus.Current_mA[0]);
			}

			return cartridge;
		}

		public CTemperature GetPortToolMOStemp(Port port)
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
				temp = new CTemperature(m_PortData[(int)port].ToolStatus.Temp_MOS.UTI);
			}

			return temp;
		}

		public ToolFutureMode GetPortToolFutureMode(Port port)
		{
			ToolFutureMode futureMode = ToolFutureMode.NoFutureMode;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				futureMode = m_PortData[(int)port].ToolStatus.FutureMode_Tool;
			}

			return futureMode;
		}

		public int GetPortToolTimeToFutureMode(Port port)
		{
			int timeFutureMode = -1;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				timeFutureMode = m_PortData[(int)port].ToolStatus.TimeToSleepHibern;
			}

			return timeFutureMode;
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

		public OnOff GetPortToolSleepStatus(Port port)
		{
			OnOff sleepStatus = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				sleepStatus = m_PortData[(int)port].ToolStatus.Sleep_OnOff;
			}

			return sleepStatus;
		}

		public OnOff GetPortToolHibernationStatus(Port port)
		{
			OnOff hiberStatus = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				hiberStatus = m_PortData[(int)port].ToolStatus.Hiber_OnOff;
			}

			return hiberStatus;
		}

		public OnOff GetPortToolExtractorStatus(Port port)
		{
			OnOff extractorStatus = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				extractorStatus = m_PortData[(int)port].ToolStatus.Extractor_OnOff;
			}

			return extractorStatus;
		}

		public OnOff GetPortToolDesolderStatus(Port port)
		{
			OnOff desolderStatus = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts || port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				desolderStatus = m_PortData[(int)port].ToolStatus.Desold_OnOff;
			}

			return desolderStatus;
		}

		public void SetPortToolStandStatus(Port port, OnOff _OnOff)
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
				// 17/02/2014 Se modifica: si pido On, debo llamarlo con Extractor = off, porque es prioritario el Extractor y
				// y nunca se pondría en On el stand si he hecho un extractor On anterior
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (_OnOff == OnOff._ON)
					{
						m_Frames_01.WriteStatusRemoteMode(port, OnOff._ON, OnOff._OFF, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					if (_OnOff == OnOff._OFF)
					{
						m_Frames_01.WriteStatusRemoteMode(port, OnOff._OFF, m_PortData[(int)port].ToolStatus.Extractor_OnOff, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					m_Frames_01.ReadStatusRemoteMode(port);
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					if (_OnOff == OnOff._ON)
					{
						m_Frames_02.WriteStatusTool(port, OnOff._ON, OnOff._OFF, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					if (_OnOff == OnOff._OFF)
					{
						m_Frames_02.WriteStatusTool(port, OnOff._OFF, m_PortData[(int)port].ToolStatus.Extractor_OnOff, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					m_Frames_02.ReadStatusTool(port);
				}
			}
		}

		public void SetPortToolExtractorStatus(Port port, OnOff _OnOff)
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
					if (_OnOff == OnOff._ON)
					{
						m_Frames_01.WriteStatusRemoteMode(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, OnOff._ON, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					if (_OnOff == OnOff._OFF)
					{
						m_Frames_01.WriteStatusRemoteMode(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, OnOff._OFF, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					m_Frames_01.ReadStatusRemoteMode(port);
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					if (_OnOff == OnOff._ON)
					{
						m_Frames_02.WriteStatusTool(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, OnOff._ON, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					if (_OnOff == OnOff._OFF)
					{
						m_Frames_02.WriteStatusTool(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, OnOff._OFF, m_PortData[(int)port].ToolStatus.Desold_OnOff);
					}
					m_Frames_02.ReadStatusTool(port);
				}
			}
		}

		public void SetPortToolDesolderStatus(Port port, OnOff _OnOff)
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
					if (_OnOff == OnOff._ON)
					{
						m_Frames_01.WriteStatusRemoteMode(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, m_PortData[(int)port].ToolStatus.Extractor_OnOff, OnOff._ON);
					}
					if (_OnOff == OnOff._OFF)
					{
						m_Frames_01.WriteStatusRemoteMode(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, m_PortData[(int)port].ToolStatus.Extractor_OnOff, OnOff._OFF);
					}
					m_Frames_01.ReadStatusRemoteMode(port);
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					if (_OnOff == OnOff._ON)
					{
						m_Frames_02.WriteStatusTool(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, m_PortData[(int)port].ToolStatus.Extractor_OnOff, OnOff._ON);
					}
					if (_OnOff == OnOff._OFF)
					{
						m_Frames_02.WriteStatusTool(port, m_PortData[(int)port].ToolStatus.Sleep_OnOff, m_PortData[(int)port].ToolStatus.Extractor_OnOff, OnOff._OFF);
					}
					m_Frames_02.ReadStatusTool(port);
				}
			}
		}

#endregion


#region Port + Tool

		public CTemperature GetPortToolFixTemp(Port port, GenericStationTools tool)
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

				//Check TempLevels
			}
			else if (m_StationData.Info.Features.TempLevelsWithStatus)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported by this station model."));

			}
			else
			{
				int idx = getToolIndex(tool);
				temp = new CTemperature(m_PortData[(int)port].ToolSettings[idx].FixedTemp.UTI);
			}

			return temp;
		}

		public void SetPortToolFixTemp(Port port, GenericStationTools tool, CTemperature temperature)
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

				//Check TempLevels
			}
			else if (m_StationData.Info.Features.TempLevelsWithStatus)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported by this station model."));

			}
			else
			{

				//Check temperature
				CTemperature auxMaxTemp = GetStationMaxTemp();
				CTemperature auxMinTemp = GetStationMinTemp();

				//el protocolo 2 no debería aceptar una temperatura de NO_TEMP_LEVEL
				if (((temperature.UTI > auxMaxTemp.UTI) || (temperature.UTI < auxMinTemp.UTI)) && temperature.UTI != Constants.NO_FIXED_TEMP)
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
				}
				else
				{
					//Switch protocol
					if (m_CommandProtocol == Protocol.Protocol_01)
					{
						m_Frames_01.WriteFixTemp(port, tool, new CTemperature(temperature.UTI));
						m_Frames_01.ReadFixTemp(port, tool);
					}
				}
			}
		}

		public void SetPortToolFixTemp(Port port, GenericStationTools tool, OnOff newOnOff)
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

				//Check TempLevels
			}
			else if (m_StationData.Info.Features.TempLevelsWithStatus)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported by this station model."));

			}
			else
			{

				//Calculate temperature
				CTemperature newTemp = new CTemperature(System.Convert.ToInt32((double) (Features_MaxTemp().UTI + Features_MinTemp().UTI) / 2));
				if (newOnOff == OnOff._OFF)
				{
					newTemp.UTI = Constants.NO_FIXED_TEMP;
				}
				else
				{
					// si ya tiene una temperatura y se pone a On, no hacer nada
					if (GetPortToolFixTemp(port, tool).UTI != Constants.NO_FIXED_TEMP)
					{
						return;
					}
				}

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteFixTemp(port, tool, newTemp);
					m_Frames_01.ReadFixTemp(port, tool);
				}
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

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTempSelect != ToolTemperatureLevels.NO_LEVELS)
					{
						enabled = OnOff._ON;
					}
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					enabled = m_PortData[(int)port].ToolSettings[idx].Levels.LevelsOnOff;
				}
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

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteSelectLevelTemp(port, tool, level);
					m_Frames_01.ReadSelectLevelTemp(port, tool);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					int idx = getToolIndex(tool);
					CTempLevelsData_SOLD stnToolLevels = (CTempLevelsData_SOLD) (m_PortData[(int)port].ToolSettings[idx].Levels.Clone());
					stnToolLevels.LevelsTempSelect = level;
					m_Frames_02.WriteLevelsTemps(port, tool, stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect, stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2]);
					m_Frames_02.ReadLevelsTemps(port, tool);
					stnToolLevels = null;
				}
			}
		}

		public void SetPortToolSelectedTempLevelsEnabled(Port
				port, GenericStationTools tool, OnOff _onoff)
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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (_onoff == OnOff._OFF)
					{
						m_Frames_01.WriteSelectLevelTemp(port, tool, ToolTemperatureLevels.NO_LEVELS);
					}
					else if (m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTempSelect == ToolTemperatureLevels.NO_LEVELS)
					{
						m_Frames_01.WriteSelectLevelTemp(port, tool, ToolTemperatureLevels.FIRST_LEVEL);
					}
					m_Frames_01.ReadSelectLevelTemp(port, tool);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{

					CTempLevelsData_SOLD stnToolLevels = (CTempLevelsData_SOLD) (m_PortData[(int)port].ToolSettings[idx].Levels.Clone());
					stnToolLevels.LevelsOnOff = _onoff;
					m_Frames_02.WriteLevelsTemps(port, tool, stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect, stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2]);
					m_Frames_02.ReadLevelsTemps(port, tool);
					stnToolLevels = null;
				}
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

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI != 0 && m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI != Constants.NO_TEMP_LEVEL)
					{
						enabled = OnOff._ON;
					}

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					enabled = m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTempOnOff[(int)level];
				}
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

					//Switch protocol
					if (m_CommandProtocol == Protocol.Protocol_01)
					{

						if (level == ToolTemperatureLevels.FIRST_LEVEL)
						{
							m_Frames_01.WriteLevelTemp1(port, tool, temperature);
							m_Frames_01.ReadLevelTemp1(port, tool);

						}
						else if (level == ToolTemperatureLevels.SECOND_LEVEL)
						{
							m_Frames_01.WriteLevelTemp2(port, tool, temperature);
							m_Frames_01.ReadLevelTemp2(port, tool);

						}
						else if (level == ToolTemperatureLevels.THIRD_LEVEL)
						{
							m_Frames_01.WriteLevelTemp3(port, tool, temperature);
							m_Frames_01.ReadLevelTemp3(port, tool);
						}

					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						int idx = getToolIndex(tool);
						CTempLevelsData_SOLD stnToolLevels = (CTempLevelsData_SOLD) (m_PortData[(int)port].ToolSettings[idx].Levels.Clone());

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

						m_Frames_02.WriteLevelsTemps(port, tool, stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect, stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2]);
						m_Frames_02.ReadLevelsTemps(port, tool);
						stnToolLevels = null;
					}
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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{

					//First level
					if (level == ToolTemperatureLevels.FIRST_LEVEL)
					{
						if (onoff == OnOff._OFF)
						{
							m_Frames_01.WriteLevelTemp1(port, tool, new CTemperature(Constants.NO_TEMP_LEVEL));

						}
						else if (m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI == 0 ||
								m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI == Constants.NO_TEMP_LEVEL)
						{
							m_Frames_01.WriteLevelTemp1(port, tool, new CTemperature(Constants.DEFAULT_TEMP));
						}

						m_Frames_01.ReadLevelTemp1(port, tool);

						//Second level
					}
					else if (level == ToolTemperatureLevels.SECOND_LEVEL)
					{
						if (onoff == OnOff._OFF)
						{
							m_Frames_01.WriteLevelTemp2(port, tool, new CTemperature(Constants.NO_TEMP_LEVEL));

						}
						else if (m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI == 0 ||
								m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI == Constants.NO_TEMP_LEVEL)
						{
							m_Frames_01.WriteLevelTemp2(port, tool, new CTemperature(Constants.DEFAULT_TEMP));
						}

						m_Frames_01.ReadLevelTemp2(port, tool);

						//Third level
					}
					else if (level == ToolTemperatureLevels.THIRD_LEVEL)
					{
						if (onoff == OnOff._OFF)
						{
							m_Frames_01.WriteLevelTemp3(port, tool, new CTemperature(Constants.NO_TEMP_LEVEL));

						}
						else if (m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI == 0 ||
								m_PortData[(int)port].ToolSettings[idx].Levels.LevelsTemp[(int)level].UTI == Constants.NO_TEMP_LEVEL)
						{
							m_Frames_01.WriteLevelTemp3(port, tool, new CTemperature(Constants.DEFAULT_TEMP));
						}

						m_Frames_01.ReadLevelTemp3(port, tool);
					}

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					CTempLevelsData_SOLD stnToolLevels = (CTempLevelsData_SOLD) (m_PortData[(int)port].ToolSettings[idx].Levels.Clone());

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

					m_Frames_02.WriteLevelsTemps(port, tool, stnToolLevels.LevelsOnOff, stnToolLevels.LevelsTempSelect, stnToolLevels.LevelsTempOnOff[0], stnToolLevels.LevelsTemp[0], stnToolLevels.LevelsTempOnOff[1], stnToolLevels.LevelsTemp[1], stnToolLevels.LevelsTempOnOff[2], stnToolLevels.LevelsTemp[2]);
					m_Frames_02.ReadLevelsTemps(port, tool);
					stnToolLevels = null;
				}
			}
		}

		public void SetPortToolLevels(Port port, GenericStationTools tool, OnOff LevelsOnOff, ToolTemperatureLevels LevelSelected, OnOff Level1OnOff, CTemperature Level1Temp, OnOff Level2OnOff, CTemperature Level2Temp, OnOff Level3OnOff, CTemperature Level3Temp)
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

				if (((Level1Temp.UTI > auxMaxTemp.UTI) || (Level1Temp.UTI < auxMinTemp.UTI) && Level1Temp.UTI != Constants.NO_TEMP_LEVEL) || ((Level2Temp.UTI > auxMaxTemp.UTI) || (Level2Temp.UTI < auxMinTemp.UTI) && Level2Temp.UTI != Constants.NO_TEMP_LEVEL) || ((Level3Temp.UTI > auxMaxTemp.UTI) || (Level3Temp.UTI < auxMinTemp.UTI) && Level3Temp.UTI != Constants.NO_TEMP_LEVEL))
				{
					if (UserErrorEvent != null)
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE, "Temperature out of range."));
				}
				else
				{
					//Switch protocol
					if (m_CommandProtocol == Protocol.Protocol_01)
					{

						//Levels selected
						if (LevelsOnOff == OnOff._OFF)
						{
							m_Frames_01.WriteSelectLevelTemp(port, tool, ToolTemperatureLevels.NO_LEVELS);
						}
						else
						{
							m_Frames_01.WriteSelectLevelTemp(port, tool, LevelSelected);
						}
						m_Frames_01.ReadSelectLevelTemp(port, tool);

						//First level
						if (Level1OnOff == OnOff._OFF)
						{
							m_Frames_01.WriteLevelTemp1(port, tool, new CTemperature(Constants.NO_TEMP_LEVEL));
						}
						else
						{
							m_Frames_01.WriteLevelTemp1(port, tool, new CTemperature(Level1Temp.UTI));
						}
						m_Frames_01.ReadLevelTemp1(port, tool);

						//Second level
						if (Level2OnOff == OnOff._OFF)
						{
							m_Frames_01.WriteLevelTemp2(port, tool, new CTemperature(Constants.NO_TEMP_LEVEL));
						}
						else
						{
							m_Frames_01.WriteLevelTemp2(port, tool, new CTemperature(Level2Temp.UTI));
						}
						m_Frames_01.ReadLevelTemp2(port, tool);

						//Third level
						if (Level3OnOff == OnOff._OFF)
						{
							m_Frames_01.WriteLevelTemp3(port, tool, new CTemperature(Constants.NO_TEMP_LEVEL));
						}
						else
						{
							m_Frames_01.WriteLevelTemp3(port, tool, new CTemperature(Level3Temp.UTI));
						}
						m_Frames_01.ReadLevelTemp3(port, tool);

					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						m_Frames_02.WriteLevelsTemps(port, tool, LevelsOnOff, LevelSelected, Level1OnOff, new CTemperature(Level1Temp.UTI), Level2OnOff, new CTemperature(Level2Temp.UTI), Level3OnOff, new CTemperature(Level3Temp.UTI));
						m_Frames_02.ReadLevelsTemps(port, tool);
					}
				}
			}
		}

		public ToolTimeSleep GetPortToolSleepDelay(Port port, GenericStationTools tool)
		{
			ToolTimeSleep toolSleep = ToolTimeSleep.NO_SLEEP;

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
				toolSleep = m_PortData[(int)port].ToolSettings[idx].SleepTime;
			}

			return toolSleep;
		}

		public OnOff GetPortToolSleepDelayEnabled(Port port, GenericStationTools tool)
		{
			OnOff sleepDelayEnabled = OnOff._OFF;

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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (m_PortData[(int)port].ToolSettings[idx].SleepTime != ToolTimeSleep.NO_SLEEP)
					{
						sleepDelayEnabled = OnOff._ON;
					}

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					sleepDelayEnabled = m_PortData[(int)port].ToolSettings[idx].SleepTimeOnOff;
				}
			}

			return sleepDelayEnabled;
		}

		public void SetPortToolSleepDelay(Port port, GenericStationTools tool, ToolTimeSleep delay)
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

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteSleepDelay(port, tool, delay);
					m_Frames_01.ReadSleepDelay(port, tool);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					int idx = getToolIndex(tool);
					m_Frames_02.WriteSleepDelay(port, tool, delay, m_PortData[(int)port].ToolSettings[idx].SleepTimeOnOff);
					m_Frames_02.ReadSleepDelay(port, tool);
				}
			}
		}

		public void SetPortToolSleepDelayEnabled(Port port, GenericStationTools tool, OnOff _onoff)
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

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (_onoff == OnOff._OFF)
					{
						m_Frames_01.WriteSleepDelay(port, tool, ToolTimeSleep.NO_SLEEP);
					}
					else
					{
						m_Frames_01.WriteSleepDelay(port, tool, (ToolTimeSleep)Constants.DEFAULT_SLEEP_TIME);
					}
					m_Frames_01.ReadSleepDelay(port, tool);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					int idx = getToolIndex(tool);
					m_Frames_02.WriteSleepDelay(port, tool, m_PortData[(int)port].ToolSettings[idx].SleepTime, _onoff);
					m_Frames_02.ReadSleepDelay(port, tool);
				}
			}
		}

		public CTemperature GetPortToolSleepTemp(Port port, GenericStationTools tool)
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
				temp.UTI = m_PortData[(int)port].ToolSettings[idx].SleepTemp.UTI;
			}

			return temp;
		}

		public void SetPortToolSleepTemp(Port port, GenericStationTools tool, CTemperature temperature)
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
				CTemperature auxMinTemp = Features_MinTemp();

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
						m_Frames_01.WriteSleepTemp(port, tool, temperature);
						m_Frames_01.ReadSleepTemp(port, tool);

					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						m_Frames_02.WriteSleepTemp(port, tool, temperature);
						m_Frames_02.ReadSleepTemp(port, tool);
					}
				}
			}
		}

		public ToolTimeHibernation GetPortToolHibernationDelay(Port port, GenericStationTools tool)
		{
			ToolTimeHibernation toolHibernation = ToolTimeHibernation.NO_HIBERNATION;

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
				toolHibernation = m_PortData[(int)port].ToolSettings[idx].HiberTime;
			}

			return toolHibernation;
		}

		public OnOff GetPortToolHibernationDelayEnabled(Port port, GenericStationTools tool)
		{
			OnOff delayEnabled = OnOff._OFF;

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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (m_PortData[(int)port].ToolSettings[idx].HiberTime != ToolTimeHibernation.NO_HIBERNATION)
					{
						delayEnabled = OnOff._ON;
					}

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					delayEnabled = m_PortData[(int)port].ToolSettings[idx].HiberTimeOnOff;
				}
			}

			return delayEnabled;
		}

		public void SetPortToolHibernationDelay(Port port, GenericStationTools tool, ToolTimeHibernation delay)
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

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteHiberDelay(port, tool, delay);
					m_Frames_01.ReadHiberDelay(port, tool);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					int idx = getToolIndex(tool);
					m_Frames_02.WriteHiberDelay(port, tool, delay, m_PortData[(int)port].ToolSettings[idx].HiberTimeOnOff);
					m_Frames_02.ReadHiberDelay(port, tool);
				}
			}
		}

		public void SetPortToolHibernationDelayEnabled(Port port, GenericStationTools tool, OnOff onoff)
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

				//Switch protocol
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (onoff == OnOff._OFF)
					{
						m_Frames_01.WriteHiberDelay(port, tool, ToolTimeHibernation.NO_HIBERNATION);
					}
					else
					{
						m_Frames_01.WriteHiberDelay(port, tool, (ToolTimeHibernation)Constants.DEFAULT_HIBERNATION_TIME);
					}
					m_Frames_01.ReadHiberDelay(port, tool);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					int idx = getToolIndex(tool);
					m_Frames_02.WriteHiberDelay(port, tool, m_PortData[(int)port].ToolSettings[idx].HiberTime, onoff);
					m_Frames_02.ReadHiberDelay(port, tool);
				}
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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteAjustTemp(port, tool, temperature);
					m_Frames_01.ReadAjustTemp(port, tool);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteAjustTemp(port, tool, temperature);
					m_Frames_02.ReadAjustTemp(port, tool);
				}
			}
		}

		public CCartridgeData GetPortToolCartridge(Port port, GenericStationTools tool)
		{
			CCartridgeData cartridge = null;

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
				if (m_StationData.Info.Features.Cartridges)
				{
					int idx = getToolIndex(tool);
					cartridge = m_PortData[(int)port].ToolSettings[idx].Cartridge;
				}
			}

			return cartridge;
		}

		public void SetPortToolCartridge(Port port, GenericStationTools tool, CCartridgeData cartridge)
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
				if (m_StationData.Info.Features.Cartridges)
				{

					//Calculate correct parameters to cartridge
					if (cartridge.CalculateParametersFromNumber(tool, m_StationData.Info.Model))
					{

						//Switch protocol
						if (m_CommandProtocol == Protocol.Protocol_02)
						{
							int idx = getToolIndex(tool);
							m_Frames_02.WriteCartridge(port, tool, cartridge);
							m_Frames_02.ReadCartridge(port, tool);
						}
					}
				}
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
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.WriteConnectStatus(mode);
				m_Frames_01.ReadConnectStatus();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteConnectStatus(mode);
				m_Frames_02.ReadConnectStatus();
			}
		}

		public OnOff GetRemoteMode()
		{
			return m_StationData.Status.RemoteMode;
		}

		public void SetRemoteMode(OnOff _OnOff)
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.WriteRemoteMode(_OnOff);
				m_Frames_01.ReadRemoteMode();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteRemoteMode(_OnOff);
				m_Frames_02.ReadRemoteMode();
			}
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

			// if continuous mode off, start continuous mode with default mode: all available ports and 10 miliseconds
			if (m_StationData.Status.ContinuousModeStatus.speed == SpeedContinuousMode.OFF)
			{
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteContiMode(m_startContModeStatus.getByteFromPorts(), m_startContModeStatus.speed);
					m_Frames_01.ReadContiMode();
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteContiMode(m_startContModeStatus.getByteFromPorts(), m_startContModeStatus.speed);
					m_Frames_02.ReadContiMode();
				}
			}

			return uiNewTraceID;
		}

		public void StopContinuousMode(uint traceID)
		{
			// delete queue instance
			m_continuousModeQueueList.DeleteQueue(traceID);

			// if no more queues, stop
			if (m_continuousModeQueueList.QueueCount() == 0)
			{
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteContiMode(m_startContModeStatus.getByteFromPorts(), SpeedContinuousMode.OFF);
					m_Frames_01.ReadContiMode();
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteContiMode(m_startContModeStatus.getByteFromPorts(), SpeedContinuousMode.OFF);
					m_Frames_02.ReadContiMode();
				}
			}
		}

		public CContinuousModeStatus GetContinuousMode()
		{
			CContinuousModeStatus status = (CContinuousModeStatus) (m_StationData.Status.ContinuousModeStatus.Clone());

			return status;
		}

		// ports and speed to use in next StartContinuousMode
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

		public stContinuousModeData_SOLD GetContinuousModeNextData(uint traceID)
		{
			return m_continuousModeQueueList.Queue(traceID).GetData();
		}

#endregion


#region Station methods

		public void SetDefaultStationParams()
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.ReadResetParam();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.ReadResetParam();
			}
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

		public CTemperature GetStationTransformerTemp()
		{
			return new CTemperature(m_StationData.Status.TempTRAFO.UTI);
		}

		public CTemperature GetStationTransformerErrorTemp()
		{
			return new CTemperature(m_StationData.Status.TempErrorTRAFO.UTI);
		}

		public CTemperature GetStationMOSerrorTemp()
		{
			return (new CTemperature(m_StationData.Status.TempErrorMOS.UTI));
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

			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.WriteDeviceName(stationName);
				m_Frames_01.ReadDeviceName();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteDeviceName(stationName);
				m_Frames_02.ReadDeviceName();
			}
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
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				stationUUID.NewMAC(iSequence);
				// write bytes
				m_Frames_01.WriteDeviceUID(stationUUID.StationData);
				m_Frames_01.ReadDeviceUID();

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				stationUUID.NewGUIDS();
				// write bytes
				m_Frames_02.WriteDeviceUID(stationUUID.StationData);
				m_Frames_02.ReadDeviceUID();
			}
				
			m_StationData.Info.UUID = stationUUID.UID;
		}

		public string GetStationPIN()
		{
			return m_StationData.Settings.PIN;
		}

		public void SetStationPIN(string newPIN)
		{
			if (newPIN.Length == 4)
			{
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteDevicePIN(newPIN);
					m_Frames_01.ReadDevicePIN();
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteDevicePIN(newPIN);
					m_Frames_02.ReadDevicePIN();
				}
			}
		}

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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteMaxTemp(temperature);
					m_Frames_01.ReadMaxTemp();
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteMaxTemp(temperature);
					m_Frames_02.ReadMaxTemp();
				}
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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteMinTemp(temperature);
					m_Frames_01.ReadMinTemp();
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteMinTemp(temperature);
					m_Frames_02.ReadMinTemp();
				}
			}
		}

		public CTemperature.TemperatureUnit GetStationTempUnits()
		{
			return m_StationData.Settings.Unit;
		}

		public void SetStationTempUnits(CTemperature.TemperatureUnit units)
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.WriteTempUnit(units);
				m_Frames_01.ReadTempUnit();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteTempUnit(units);
				m_Frames_02.ReadTempUnit();
			}
		}

		public OnOff GetStationN2Mode()
		{
			return m_StationData.Settings.N2Mode;
		}

		public void SetStationN2Mode(OnOff mode)
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.WriteN2Mode(mode);
				m_Frames_01.ReadN2Mode();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteN2Mode(mode);
				m_Frames_02.ReadN2Mode();
			}
		}

		public OnOff GetStationHelpText()
		{
			return m_StationData.Settings.HelpText;
		}

		public void SetStationHelpText(OnOff help)
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.WriteHelpText(help);
				m_Frames_01.ReadHelpText();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteHelpText(help);
				m_Frames_02.ReadHelpText();
			}
		}

		public OnOff GetStationParametersLocked()
		{
			return m_StationData.Settings.ParametersLocked;
		}

		public void SetStationParametersLocked(OnOff @lock)
		{

			if (m_StationData.Info.Features.ParamsLockedFrame)
			{
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WriteParametersLocked(@lock);
					m_Frames_01.ReadParametersLocked();

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WriteParametersLocked(@lock);
					m_Frames_02.ReadParametersLocked();
				}
			}
		}

		public int GetStationPowerLimit()
		{
			return m_StationData.Settings.PowerLimit;
		}

		public void SetStationPowerLimit(int powerLimit)
		{

			powerLimit = System.Convert.ToInt32((powerLimit / 10) * 10);

			//Check Power limit
			int max = Features_MaxPowerLimit();

			if (powerLimit < (int) PowerLimits.MIN | (max != 0 & powerLimit > max))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.POWER_LIMIT_OUT_OF_RANGE, "Power limit out of range"));

			}
			else
			{
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.WritePowerLimit(powerLimit);
					m_Frames_01.ReadPowerLimit();
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WritePowerLimit(powerLimit);
					m_Frames_02.ReadPowerLimit();
				}
			}
		}

		public OnOff GetStationBeep()
		{
			return m_StationData.Settings.Beep;
		}

		public void SetStationBeep(OnOff beep)
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.WriteBeep(beep);
				m_Frames_01.ReadBeep();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteBeep(beep);
				m_Frames_02.ReadBeep();
			}
		}

		public OnOff GetStationLocked()
		{
			return m_StationData.Settings.StationLocked;
		}

		public void SetStationLocked(OnOff locked)
		{
			//FIXME
			m_StationData.Settings.StationLocked = locked;
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

		public int GetPortToolSleepMinutes(Port port)
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
				counter = m_PortData[(int)port].Counters.ContSleepMinutes;
			}

			return counter;
		}

		public int GetPortToolHibernationMinutes(Port port)
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
				counter = m_PortData[(int)port].Counters.ContHiberMinutes;
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
				counter = m_PortData[(int)port].Counters.ContIdleMinutes;
			}

			return counter;
		}

		public int GetPortToolSleepCycles(Port port)
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
				counter = m_PortData[(int)port].Counters.ContSleepCycles;
			}

			return counter;
		}

		public int GetPortToolDesolderCycles(Port port)
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
				counter = m_PortData[(int)port].Counters.ContDesoldCycles;
			}

			return counter;
		}

		internal void ResetPortToolMinutesGlobalPorts()
		{
			// added Edu 22/04/2016
			//Check port
			int[] values = new int[] {0, 0, 0, 0};

			//Switch protocol
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				Array.Resize(ref values, NumPorts - 1 + 1);
				// reset minutes
				m_Frames_01.WriteHiberTime(values);
				m_Frames_01.WriteSleepTime(values);
				m_Frames_01.WriteWorkTime(values);
				m_Frames_01.WritePlugTime(values);
				// WriteIdleTime is calculated in station, do not reset
				// In DME_TCH_01 is not calculated
				if (!m_StationData.Info.Features.CounterIdleTimeIsCalculated)
				{
					m_Frames_01.WriteIdleTime(values);
				}
				// reset cycles
				m_Frames_01.WriteSleepCycles(values);
				m_Frames_01.WriteDesoldCycles(values);

				// read minutes
				m_Frames_01.ReadPlugTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadWorkTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadSleepTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadHiberTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadIdleTime(CounterTypes.GLOBAL_COUNTER);
				// read cycles
				m_Frames_01.ReadSleepCycles(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadDesoldCycles(CounterTypes.GLOBAL_COUNTER);

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				for (var Port = 0; Port <= NumPorts - 1; Port++)
				{
					// reset minutes
					m_Frames_02.WriteHiberTime((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
					m_Frames_02.WriteSleepTime((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
					m_Frames_02.WriteWorkTime((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
					m_Frames_02.WritePlugTime((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
					// WriteIdleTime is calculated in station, do not reset
					// In DME_TCH_01 is not calculated
					if (!m_StationData.Info.Features.CounterIdleTimeIsCalculated)
					{
						m_Frames_02.WriteIdleTime((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
					}
					// reset cycles
					m_Frames_02.WriteSleepCycles((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);
					m_Frames_02.WriteDesoldCycles((Port) Port, CounterTypes.GLOBAL_COUNTER, values[(int) Port]);

					// read minutes
					m_Frames_02.ReadPlugTime((Port) Port, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadWorkTime((Port) Port, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadSleepTime((Port) Port, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadHiberTime((Port) Port, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadIdleTime((Port) Port, CounterTypes.GLOBAL_COUNTER);
					// read cycles
					m_Frames_02.ReadSleepCycles((Port) Port, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadDesoldCycles((Port) Port, CounterTypes.GLOBAL_COUNTER);

				}
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

		public int GetPortToolSleepMinutesPartial(Port port)
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
				counter = m_PortData[(int)port].PartialCounters.ContSleepMinutes;
			}

			return counter;
		}

		public int GetPortToolHibernationMinutesPartial(Port port)
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
				counter = m_PortData[(int)port].PartialCounters.ContHiberMinutes;
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
				counter = m_PortData[(int)port].PartialCounters.ContIdleMinutes;
			}

			return counter;
		}

		public int GetPortToolSleepCyclesPartial(Port port)
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
				counter = m_PortData[(int)port].PartialCounters.ContSleepCycles;
			}

			return counter;
		}

		public int GetPortToolDesolderCyclesPartial(Port port)
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
				counter = m_PortData[(int)port].PartialCounters.ContDesoldCycles;
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

				//Switch protocol
				//protocolo 1 no puede hacer reset parciales
				if (m_CommandProtocol == Protocol.Protocol_02)
				{

					//Reset
					m_Frames_02.WriteHiberTime(port, CounterTypes.PARTIAL_COUNTER, 0);
					m_Frames_02.WriteSleepTime(port, CounterTypes.PARTIAL_COUNTER, 0);
					m_Frames_02.WriteWorkTime(port, CounterTypes.PARTIAL_COUNTER, 0);
					m_Frames_02.WritePlugTime(port, CounterTypes.PARTIAL_COUNTER, 0);
					// WriteIdleTime is calculated in station, do not reset
					// In DME_TCH_01 is not calculated
					if (!m_StationData.Info.Features.CounterIdleTimeIsCalculated)
					{
						m_Frames_02.WriteIdleTime(port, CounterTypes.PARTIAL_COUNTER, 0);
					}
					m_Frames_02.WriteSleepCycles(port, CounterTypes.PARTIAL_COUNTER, 0);
					m_Frames_02.WriteDesoldCycles(port, CounterTypes.PARTIAL_COUNTER, 0);

					//Read
					m_Frames_02.ReadPlugTime(port, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadWorkTime(port, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadSleepTime(port, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadHiberTime(port, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadIdleTime(port, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadSleepCycles(port, CounterTypes.PARTIAL_COUNTER);
					m_Frames_02.ReadDesoldCycles(port, CounterTypes.PARTIAL_COUNTER);
				}
			}
		}

#endregion

#endregion


#region Communications

#region Ethernet

		internal CEthernetData GetEthernetConfiguration()
		{
			return m_StationData.Settings.Ethernet;
		}

		internal void SetEthernetConfiguration(CEthernetData ethernetData)
		{

			//Switch protocol
			//protocolo 1 no puede configurar ethernet
			if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.WriteEthernetConfiguration(ethernetData);
				m_Frames_02.ReadEthernetConfiguration();
			}
		}

#endregion


#region Robot

		internal CRobotData GetRobotConfiguration()
		{
			return m_StationData.Settings.Robot;
		}

		internal void SetRobotConfiguration(CRobotData robotData)
		{

			//Switch protocol
			//protocolo 1 no puede configurar robot
			if (m_CommandProtocol == Protocol.Protocol_02)
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
		}

		public OnOff GetRobotStatus()
		{
			return m_StationData.Settings.Robot.Status;
		}

#endregion

#endregion


#region Peripherals

		public List<CPeripheralData> GetPeripheralList()
		{
			return m_StationData.Peripherals;
		}

		public void SetPeripheralConfiguration(int peripheralID, CPeripheralData peripheralData)
		{

			//Check peripherals
			if (m_StationData.Peripherals.Count < peripheralData.ID)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PERIPHERAL_NOT_IN_RANGE, "Peripheral ID not found."));
			}
			else
			{

				//Switch protocol
				//protocolo 1 no soporta peripherals
				if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.WritePeripheralConfiguration(peripheralID, peripheralData);
					m_Frames_02.ReadPeripheralConfiguration(peripheralID);
				}
			}
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
			uint idTransaction = (uint) 0;

			//Switch protocol
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				idTransaction = m_Frames_01.MarkACK();
			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				idTransaction = m_Frames_02.MarkACK();
			}

			return idTransaction;
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
					if (!m_StationData.Info.Features.ChangesStatusInformation || (!m_IsDataInitialized && m_IdTransactionDataInitialized == UInt32.MaxValue))
					{

						//Station Parameters
						UpdateStationParam();

						//Selected temperature
						UpdateAllSelectedTemperature();

						//Tool Parameters
						UpdateAllToolParam();
					}

					//Un cambio en el estado del robot no viene indicado en status information
					UpdateRobotStatus();

					UpdateMOSTemp();
					UpdateTrafoTemp();

					//Peripheral Configuration
					UpdateAllPeripheralConfiguration();

					//Peripheral Status
					UpdateAllPeripheralStatus();

					//Pedir todos los micros de la estación
					if (m_CommandProtocol == Protocol.Protocol_01)
					{
						// se quita por problemas en la DI versión software 9996926 en el comando H21 pedido de software/harware
						//m_Frames_01.ReadDeviceVersions()

					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						//Comprobar si los micros están desconectados
						ArrayList keys = new ArrayList(m_StationData.Info.StationMicros.Keys);
						foreach (byte key in keys)
						{
							if (((CFirmwareStation) (m_StationData.Info.StationMicros[key])).IsDisconnectedMicro())
							{
								m_StationData.Info.StationMicros.Remove(key);
							}
						}

						m_Frames_02.ReadDeviceVersions((byte) (EnumAddress.MASK_STATION_ADDRESS | EnumAddress.MASK_BROADCAST_ADDRESS));
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
						timeUpdateData = (int)((double)TIME_UPDATE_DATA * 1.5);
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

					//Pedir los micros una única vez por problemas en la DI
					if (m_CommandProtocol == Protocol.Protocol_01)
					{
						m_Frames_01.ReadDeviceVersions();
					}

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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.ReadInfoPort((Port) idx);
					//No implementado en versión 01 del protocolo, devuelve NACK
					//m_Frames_01.ReadDelayTime(CType(idx, Port))

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.ReadInfoPort((Port) idx);
					m_Frames_02.ReadDelayTime((Port) idx);
				}
			}
		}

		private void UpdateStationError()
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.ReadStationError();

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.ReadStationError();
			}
		}

		private void UpdateMOSTemp()
		{
			for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
			{
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.ReadMOSTemp((Port) idx);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.ReadMOSTemp((Port) idx);
				}
			}
		}

		private void UpdateTrafoTemp()
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.ReadTrafoTemp();

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.ReadTrafoTemp();
			}
		}

		private void UpdateStationParam()
		{
			//
			//General Configuration
			//
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.ReadDeviceUID(); // added 01/06/2016 Edu
				m_Frames_01.ReadTempUnit();
				m_Frames_01.ReadN2Mode();
				m_Frames_01.ReadHelpText();
				m_Frames_01.ReadPowerLimit();
				m_Frames_01.ReadBeep();
				m_Frames_01.ReadTempErrorTrafo();
				m_Frames_01.ReadTempErrorMOS();
				m_Frames_01.ReadDeviceName();
				m_Frames_01.ReadDevicePIN();
				m_Frames_01.ReadMaxTemp();
				m_Frames_01.ReadMinTemp();
				m_Frames_01.ReadConnectStatus();
				if (m_StationData.Info.Features.ParamsLockedFrame)
				{
					m_Frames_01.ReadParametersLocked();
				}

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				m_Frames_02.ReadDeviceUID(); // added 01/06/2016 Edu
				// 07/09/2016 Edu las PS dan error
				if (m_StationData.Info.Features.DisplaySettings)
				{
					m_Frames_02.ReadTempUnit();
				}
				m_Frames_02.ReadDeviceName();
				m_Frames_02.ReadDevicePIN();
				m_Frames_02.ReadMaxTemp();
				m_Frames_02.ReadMinTemp();
				m_Frames_02.ReadConnectStatus();
				if (m_StationData.Info.Features.ParamsLockedFrame)
				{
					m_Frames_02.ReadParametersLocked();
				}
			}

			//
			//Ethernet Configuration
			//
			UpdateEthernetConfiguration();

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
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				foreach (CToolSettingsData_SOLD ToolIn in m_PortData[idx].ToolSettings)
				{
					if (ToolIn.Tool != GenericStationTools.NO_TOOL)
					{
						m_Frames_01.ReadFixTemp((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadLevelTemp1((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadLevelTemp2((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadLevelTemp3((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadSelectLevelTemp((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadSleepDelay((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadSleepTemp((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadHiberDelay((Port) idx, ToolIn.Tool);
						m_Frames_01.ReadAjustTemp((Port) idx, ToolIn.Tool);
					}
				}

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				foreach (CToolSettingsData_SOLD ToolIn in m_PortData[idx].ToolSettings)
				{
					if (ToolIn.Tool != GenericStationTools.NO_TOOL)
					{
						m_Frames_02.ReadLevelsTemps((Port) idx, ToolIn.Tool);
						m_Frames_02.ReadSleepDelay((Port) idx, ToolIn.Tool);
						m_Frames_02.ReadSleepTemp((Port) idx, ToolIn.Tool);
						m_Frames_02.ReadHiberDelay((Port) idx, ToolIn.Tool);
						m_Frames_02.ReadAjustTemp((Port) idx, ToolIn.Tool);
						if (m_StationData.Info.Features.Cartridges)
						{
							m_Frames_02.ReadCartridge((Port) idx, ToolIn.Tool);
						}
					}
				}
			}
		}

		private void UpdateAllSelectedTemperature()
		{
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
				{
					m_Frames_01.ReadSelectTemp((Port) idx);
				}

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
				{
					m_Frames_02.ReadSelectTemp((Port) idx);
				}
			}
		}

		private void UpdateEthernetConfiguration()
		{
			if (m_StationData.Info.Features.Ethernet)
			{
				if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.ReadEthernetConfiguration();
				}
			}
		}

		private void UpdateRobot()
		{
			if (m_StationData.Info.Features.Robot)
			{
				if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.ReadRobotConfiguration();
				}
			}
		}

		private void UpdateRobotStatus()
		{
			if (m_StationData.Info.Features.Robot)
			{
				if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.ReadRobotStatus();
				}
			}
		}

		private void UpdateAllPeripheralStatus()
		{
			if (m_StationData.Info.Features.Peripherals)
			{
				for (var idx = 0; idx <= m_StationData.Peripherals.Count - 1; idx++)
				{
					if (m_CommandProtocol == Protocol.Protocol_01)
					{
						m_Frames_01.ReadPeripheralStatus(System.Convert.ToInt32(idx));
					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						m_Frames_02.ReadPeripheralStatus(System.Convert.ToInt32(idx));
					}
				}
			}
		}

		private void UpdateAllPeripheralConfiguration()
		{
			if (m_StationData.Info.Features.Peripherals)
			{

				//Count
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					m_Frames_01.ReadPeripheralCount();
				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					m_Frames_02.ReadPeripheralCount();
				}

				//Configuration
				for (var idx = 0; idx <= m_StationData.Peripherals.Count - 1; idx++)
				{
					if (m_CommandProtocol == Protocol.Protocol_01)
					{
						m_Frames_01.ReadPeripheralConfiguration(System.Convert.ToInt32(idx));
					}
					else if (m_CommandProtocol == Protocol.Protocol_02)
					{
						m_Frames_02.ReadPeripheralConfiguration(System.Convert.ToInt32(idx));
					}
				}
			}
		}

		private void UpdateCounters()
		{

			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				m_Frames_01.ReadPlugTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadWorkTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadSleepTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadHiberTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadIdleTime(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadSleepCycles(CounterTypes.GLOBAL_COUNTER);
				m_Frames_01.ReadDesoldCycles(CounterTypes.GLOBAL_COUNTER);

			}
			else if (m_CommandProtocol == Protocol.Protocol_02)
			{
				for (var idx = 0; idx <= m_PortData.Length - 1; idx++)
				{
					// Global
					m_Frames_02.ReadPlugTime((Port) idx, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadWorkTime((Port) idx, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadSleepTime((Port) idx, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadHiberTime((Port) idx, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadIdleTime((Port) idx, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadSleepCycles((Port) idx, CounterTypes.GLOBAL_COUNTER);
					m_Frames_02.ReadDesoldCycles((Port) idx, CounterTypes.GLOBAL_COUNTER);
					//Partial
					if (m_StationData.Info.Features.PartialCounters)
					{
						m_Frames_02.ReadPlugTime((Port) idx, CounterTypes.PARTIAL_COUNTER);
						m_Frames_02.ReadWorkTime((Port) idx, CounterTypes.PARTIAL_COUNTER);
						m_Frames_02.ReadSleepTime((Port) idx, CounterTypes.PARTIAL_COUNTER);
						m_Frames_02.ReadHiberTime((Port) idx, CounterTypes.PARTIAL_COUNTER);
						m_Frames_02.ReadIdleTime((Port) idx, CounterTypes.PARTIAL_COUNTER);
						m_Frames_02.ReadSleepCycles((Port) idx, CounterTypes.PARTIAL_COUNTER);
						m_Frames_02.ReadDesoldCycles((Port) idx, CounterTypes.PARTIAL_COUNTER);
					}
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
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication error in station.", new byte[] {}));

			}
			else if (TypeError == EnumConnectError.TIME_OUT)
			{
				if (address == m_StationNumDevice)
				{
					if (UserErrorEvent != null) //TODO. revisar error
						UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication timeout in station. Command:H" + command.ToString("X"), new byte[] {}));
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

		private void ResetSended(byte address)
		{

			//El reset es posible que sea por que una estación está en proceso de actualización (protocolo 1)
			if (m_CommandProtocol == Protocol.Protocol_01)
			{
				if (m_UpdateFirmware01 != null)
				{
					if (m_UpdateFirmware01.IsMicroUpdatingProgress())
					{
						m_UpdateFirmware01.ContinueUpdating();
					}
				}
			}
		}

		public string GetStationCom()
		{
			// 01/06/2016 Added Edu
			return m_ComChannel.COMName();
		}

		public string GetStationConnectionType()
		{
			// 01/06/2016 Added Edu
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
				if (m_CommandProtocol == Protocol.Protocol_01)
				{
					if (ReferenceEquals(m_UpdateFirmware01, null))
					{
						m_UpdateFirmware01 = new CUpdateFirmware01(UUID, m_Frames_01, m_ComChannel.COMName());
						m_UpdateFirmware01.UpdateMicroFirmwareFinished += Event_UpdateMicroFirmwareFinished;
					}

					m_UpdateFirmware01.UpdateMicrosFirmware(infoUpdateFirmware, m_StationData.Info.StationMicros);

				}
				else if (m_CommandProtocol == Protocol.Protocol_02)
				{
					if (ReferenceEquals(m_UpdateFirmware02, null))
					{
						m_UpdateFirmware02 = new CUpdateFirmware02(UUID, m_Frames_02);
						m_UpdateFirmware02.UpdateMicroFirmwareFinished += Event_UpdateMicroFirmwareFinished;
					}

					m_UpdateFirmware02.UpdateMicrosFirmware(infoUpdateFirmware, m_StationData.Info.StationMicros);
				}
			}
		}

		public void Event_UpdateMicroFirmwareFinished(string _UUID)
		{
			if (UpdateMicroFirmwareFinishedEvent != null)
				UpdateMicroFirmwareFinishedEvent(_UUID);
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
