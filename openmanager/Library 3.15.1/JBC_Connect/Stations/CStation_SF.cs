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
using Microsoft.VisualBasic;
using DataJBC;
using RoutinesJBC;

namespace JBC_Connect
{


	internal class CStation_SF : CStationBase
	{

		//Data
		private CStationData_SF m_StationData = new CStationData_SF(); //Información de la estación (Dispensador de estaño)
		private CPortData_SF[] m_PortData = new CPortData_SF[0]; //Información del puerto (Dispensador de estaño)

		//Frames
		private CStationFrames02_SF m_Frames_02; // (Dispensador de estaño)


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



		public CStation_SF(byte _StationNumDevice, Protocol _CommandProtocol, Protocol _FrameProtocol, string _StationModel, string _SoftwareVersion, string _HardwareVersion, CCommunicationChannel _ComChannel, string _ParentUUID = "")
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

			//default values for Programs
			for (int i = 0; i <= m_StationData.Settings.Programs.Length - 1; i++)
			{
				m_StationData.Settings.Programs[i] = new CProgramDispenserData_SF();
			}
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
			m_Frames_02 = new CStationFrames02_SF(m_StationData, m_PortData, m_ComChannel, m_StationNumDevice);
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
			m_PortData = new CPortData_SF[iPorts - 1 + 1];

			//Recorremos todos los puertos de la estación
			for (int idxPort = 0; idxPort <= m_PortData.Length - 1; idxPort++)
			{
				m_PortData[idxPort] = new CPortData_SF();
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
				m_Frames_02.WriteToolEnabled(enabled);
				m_Frames_02.ReadToolEnabled();
			}
		}

		public DispenserMode_SF GetPortToolDispenserMode(Port port)
		{
			DispenserMode_SF dispenserMode = DispenserMode_SF.CONTINUOUS;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				dispenserMode = m_PortData[(int)port].ToolStatus.DispenserMode;
			}

			return dispenserMode;
		}

		public void SetPortToolDispenserMode(Port port, DispenserMode_SF mode, byte nbrProgram)
		{
			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));

				//Check program
			}
			else if (nbrProgram > m_StationData.Settings.Programs.Length | nbrProgram <= 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PROGRAM_OUT_OF_RANGE, "Program not in range."));

			}
			else
			{
				m_Frames_02.WriteDispenserMode(mode, nbrProgram);
				m_Frames_02.ReadDispenserMode();
			}
		}

		public CSpeed GetPortToolSpeed(Port port)
		{
			CSpeed speed = new CSpeed();

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				speed = m_PortData[(int)port].ToolStatus.Speed;
			}

			return speed;
		}

		public void SetPortToolSpeed(Port port, CSpeed speed)
		{
			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				m_Frames_02.WriteSpeed(System.Convert.ToUInt16((ushort) (speed.ToMillimetersPerSecond() * 10)));
				m_Frames_02.ReadSpeed();
			}
		}

		public CLength GetPortToolLength(Port port)
		{
			CLength length = new CLength();

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				length = m_PortData[(int)port].ToolStatus.Length;
			}

			return length;
		}

		public void SetPortToolLength(Port port, CLength length)
		{
			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				//decimas de milimetro
				m_Frames_02.WriteLength(System.Convert.ToUInt16((ushort) (length.ToMillimeters() * 10)));
				m_Frames_02.ReadLength();
			}
		}

		public OnOff GetPortToolFeedingState(Port port)
		{
			OnOff feeding = OnOff._OFF;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				feeding = m_PortData[(int)port].ToolStatus.FeedingState;
			}

			return feeding;
		}

		public ushort GetPortToolFeedingPercent(Port port)
		{
			ushort feedingPercent = System.Convert.ToUInt16(0);

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				feedingPercent = m_PortData[(int)port].ToolStatus.FeedingPercent;
			}

			return feedingPercent;
		}

		public CLength GetPortToolFeedingLength(Port port)
		{
			CLength length = new CLength();

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				length = m_PortData[(int)port].ToolStatus.FeedingLength;
			}

			return length;
		}

		public byte GetPortToolCurrentProgramStep(Port port)
		{
			byte programStep = (byte) 0;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				programStep = m_PortData[(int)port].ToolStatus.CurrentProgramStep;
			}

			return programStep;
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

		public void ResetStationError()
		{
			m_Frames_02.ReadResetStationError((byte)m_StationData.Status.ErrorStation);
		}

		public GenericStationTools[] GetStationTools()
		{
			GenericStationTools[] tools = new GenericStationTools[0];
			return tools;
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

		public OnOff GetStationLocked()
		{
			return m_StationData.Settings.StationLocked;
		}

		public void SetStationLocked(OnOff locked)
		{
			m_Frames_02.WriteStationLocked(locked);
			m_Frames_02.ReadStationLocked();
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

		public CLength.LengthUnit GetStationLengthUnits()
		{
			return m_StationData.Settings.LengthUnit;
		}

		public void SetStationLengthUnits(CLength.LengthUnit units)
		{
			m_Frames_02.WriteLengthUnit(units);
			m_Frames_02.ReadLengthUnit();
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

		public OnOff GetStationPINEnabled()
		{
			return m_StationData.Settings.PINEnabled;
		}

		public void SetStationPINEnabled(OnOff _onoff)
		{
			m_Frames_02.WritePINEnabled(_onoff);
			m_Frames_02.ReadPINEnabled();
		}

		public byte GetStationSelectedProgram()
		{
			return m_StationData.Settings.SelectedProgram;
		}

		public CProgramDispenserData_SF GetStationProgram(byte nbrProgram)
		{
			CProgramDispenserData_SF program = null;

			//Check program
			if (nbrProgram > m_StationData.Settings.Programs.Length | nbrProgram <= 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PROGRAM_OUT_OF_RANGE, "Program not in range."));

			}
			else
			{
				program = m_StationData.Settings.Programs[nbrProgram - 1];
			}

			return program;
		}

		public void SetStationProgram(byte nbrProgram, CProgramDispenserData_SF program)
		{

			//Check program
			if (nbrProgram > m_StationData.Settings.Programs.Length | nbrProgram <= 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PROGRAM_OUT_OF_RANGE, "Program not in range."));

			}
			else
			{
				m_Frames_02.WriteProgram(nbrProgram, program);
				m_Frames_02.ReadProgram(nbrProgram);
			}
		}

		public void DeleteStationProgram(byte nbrProgram)
		{

			//Check program
			if (nbrProgram > m_StationData.Settings.Programs.Length | nbrProgram <= 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PROGRAM_OUT_OF_RANGE, "Program not in range."));

			}
			else
			{
				CProgramDispenserData_SF program = new CProgramDispenserData_SF();
				program.Name = "--------";
				program.Length_1 = System.Convert.ToUInt16(0);
				program.Speed_1 = System.Convert.ToUInt16(0);
				program.Length_2 = System.Convert.ToUInt16(0);
				program.Speed_2 = System.Convert.ToUInt16(0);
				program.Length_3 = System.Convert.ToUInt16(0);
				program.Speed_3 = System.Convert.ToUInt16(0);

				m_Frames_02.WriteProgram(nbrProgram, program);
				m_Frames_02.ReadProgram(nbrProgram);
			}
		}

		public byte[] GetStationConcatenateProgramList()
		{
			return m_StationData.Settings.ConcatenateProgramList;
		}

		public void SetStationConcatenateProgramList(byte[] programs)
		{
			m_Frames_02.WriteProgramList(programs);
			m_Frames_02.ReadProgramList();
		}


#endregion


#region Counters

#region Global Counters

		public int GetStationPluggedMinutes()
		{
			return m_PortData[0].Counters.ContPlugMinutes;
		}

		public long GetPortToolTinLength(Port port)
		{
			long counter = 0;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].Counters.ContTinLength;
			}

			return counter;
		}

		public int GetPortToolPluggedMinutes(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
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
			if ((int)port >= NumPorts|| (int)port < 0)
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

		public int GetPortToolWorkCycles(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
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

		internal void ResetStationCounters()
		{
			m_Frames_02.ReadResetCounters(CounterTypes.GLOBAL_COUNTER);
			m_Frames_02.ReadCounters(CounterTypes.GLOBAL_COUNTER);
		}

#endregion


#region Partial Counters

		public int GetStationPluggedMinutesPartial()
		{
			return m_PortData[0].PartialCounters.ContPlugMinutes;
		}

		public long GetPortToolTinLengthPartial(Port port)
		{
			long counter = 0;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(UUID, new Cerror(Cerror.cErrorCodes.PORT_NOT_IN_RANGE, "Port not in range."));
			}
			else
			{
				counter = m_PortData[(int)port].PartialCounters.ContTinLength;
			}

			return counter;
		}

		public int GetPortToolPluggedMinutesPartial(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
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
			if ((int)port >= NumPorts|| (int)port < 0)
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

		public int GetPortToolWorkCyclesPartial(Port port)
		{
			int counter = 0;

			//Check port
			if ((int)port >= NumPorts|| (int)port < 0)
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

		internal void ResetStationCountersPartial()
		{
			m_Frames_02.ReadResetCounters(CounterTypes.PARTIAL_COUNTER);
			m_Frames_02.ReadCounters(CounterTypes.PARTIAL_COUNTER);
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
				//Información que se pide cada 500ms
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

					//Tool Parameters
					UpdateAllToolParam();

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
					UpdateCounters();

					m_ContUpdateDataSlow = SLOW_SPEED_UPDATE_DATA - 1;
				}


				//Para que los PC lentos puedan funcionar en modo contínuo se debe bajar la velocidad
				int timeUpdateData = TIME_UPDATE_DATA;


				//
				//INITIALLY
				//
				if (!m_IsDataInitialized && m_IdTransactionDataInitialized == UInt32.MaxValue)
				{

					//Pedir los micros una única vez
					m_Frames_02.ReadDeviceVersions();

					//Asegurar que se han pedido todos los datos de la estación al inicializar
					m_IdTransactionDataInitialized = SetTransaction();
				}

				Thread.Sleep(timeUpdateData);
			}
		}

		private void UpdateAllInfoPort()
		{
			m_Frames_02.ReadFeeding();
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
			m_Frames_02.ReadDevicePIN();
			m_Frames_02.ReadDeviceName();
			m_Frames_02.ReadPINEnabled();
			m_Frames_02.ReadStationLocked();
			m_Frames_02.ReadBeep();
			m_Frames_02.ReadLengthUnit();
			m_Frames_02.ReadConnectStatus();

			//
			//Programs
			//
			for (int i = 0; i <= m_StationData.Settings.Programs.Length - 1; i++)
			{
				m_Frames_02.ReadProgram((byte) (i + 1));
			}
			m_Frames_02.ReadProgramList();

			//
			//Robot Configuration
			//
			UpdateRobot();
			UpdateRobotStatus();
		}

		private void UpdateAllToolParam()
		{
			m_Frames_02.ReadToolEnabled();
			m_Frames_02.ReadDispenserMode();
			m_Frames_02.ReadSpeed();
			m_Frames_02.ReadLength();
		}

		private void UpdateRobot()
		{
			m_Frames_02.ReadRobotConfiguration();
		}

		private void UpdateRobotStatus()
		{
			m_Frames_02.ReadRobotStatus();
		}

		private void UpdateCounters()
		{
			m_Frames_02.ReadCounters(CounterTypes.GLOBAL_COUNTER);
			m_Frames_02.ReadCounters(CounterTypes.PARTIAL_COUNTER);
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
