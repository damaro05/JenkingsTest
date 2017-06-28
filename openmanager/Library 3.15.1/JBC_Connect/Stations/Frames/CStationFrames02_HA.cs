// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Text;
using System.Net;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;
using DataJBC;
using RoutinesJBC;
using Constants = DataJBC.Constants;



namespace JBC_Connect
{
	internal class CStationFrames02_HA : CStationFrames
	{

		protected CStationData_HA m_StationData; //Información de la estación
		protected CPortData_HA[] m_PortData; //Información del puerto
		protected CContinuousModeQueueListStation_HA m_traceList; //Información del modo continuo


		//Substation events
		public delegate void Detected_SubStationEventHandler(CConnectionData connectionData);
		private Detected_SubStationEventHandler Detected_SubStationEvent;

		public event Detected_SubStationEventHandler Detected_SubStation
		{
			add
			{
				Detected_SubStationEvent = (Detected_SubStationEventHandler)System.Delegate.Combine(Detected_SubStationEvent, value);
			}
			remove
			{
				Detected_SubStationEvent = (Detected_SubStationEventHandler)System.Delegate.Remove(Detected_SubStationEvent, value);
			}
		}

		//Parameters changes events (FALTA)
		public delegate void Changed_SelectedTemperatureEventHandler();
		private Changed_SelectedTemperatureEventHandler Changed_SelectedTemperatureEvent;

		public event Changed_SelectedTemperatureEventHandler Changed_SelectedTemperature
		{
			add
			{
				Changed_SelectedTemperatureEvent = (Changed_SelectedTemperatureEventHandler)System.Delegate.Combine(Changed_SelectedTemperatureEvent, value);
			}
			remove
			{
				Changed_SelectedTemperatureEvent = (Changed_SelectedTemperatureEventHandler)System.Delegate.Remove(Changed_SelectedTemperatureEvent, value);
			}
		}

		public delegate void Changed_StationParametersEventHandler();
		private Changed_StationParametersEventHandler Changed_StationParametersEvent;

		public event Changed_StationParametersEventHandler Changed_StationParameters
		{
			add
			{
				Changed_StationParametersEvent = (Changed_StationParametersEventHandler)System.Delegate.Combine(Changed_StationParametersEvent, value);
			}
			remove
			{
				Changed_StationParametersEvent = (Changed_StationParametersEventHandler)System.Delegate.Remove(Changed_StationParametersEvent, value);
			}
		}

		public delegate void Changed_ToolParamEventHandler(int port);
		private Changed_ToolParamEventHandler Changed_ToolParamEvent;

		public event Changed_ToolParamEventHandler Changed_ToolParam
		{
			add
			{
				Changed_ToolParamEvent = (Changed_ToolParamEventHandler)System.Delegate.Combine(Changed_ToolParamEvent, value);
			}
			remove
			{
				Changed_ToolParamEvent = (Changed_ToolParamEventHandler)System.Delegate.Remove(Changed_ToolParamEvent, value);
			}
		}

		public delegate void Changed_CountersEventHandler();
		private Changed_CountersEventHandler Changed_CountersEvent;

		public event Changed_CountersEventHandler Changed_Counters
		{
			add
			{
				Changed_CountersEvent = (Changed_CountersEventHandler)System.Delegate.Combine(Changed_CountersEvent, value);
			}
			remove
			{
				Changed_CountersEvent = (Changed_CountersEventHandler)System.Delegate.Remove(Changed_CountersEvent, value);
			}
		}

		//Bootloader events
		public delegate void ClearingFlashFinishedEventHandler();
		private ClearingFlashFinishedEventHandler ClearingFlashFinishedEvent;

		public event ClearingFlashFinishedEventHandler ClearingFlashFinished
		{
			add
			{
				ClearingFlashFinishedEvent = (ClearingFlashFinishedEventHandler)System.Delegate.Combine(ClearingFlashFinishedEvent, value);
			}
			remove
			{
				ClearingFlashFinishedEvent = (ClearingFlashFinishedEventHandler)System.Delegate.Remove(ClearingFlashFinishedEvent, value);
			}
		}

		public delegate void AddressMemoryFlashFinishedEventHandler();
		private AddressMemoryFlashFinishedEventHandler AddressMemoryFlashFinishedEvent;

		public event AddressMemoryFlashFinishedEventHandler AddressMemoryFlashFinished
		{
			add
			{
				AddressMemoryFlashFinishedEvent = (AddressMemoryFlashFinishedEventHandler)System.Delegate.Combine(AddressMemoryFlashFinishedEvent, value);
			}
			remove
			{
				AddressMemoryFlashFinishedEvent = (AddressMemoryFlashFinishedEventHandler)System.Delegate.Remove(AddressMemoryFlashFinishedEvent, value);
			}
		}

		public delegate void DataMemoryFlashFinishedEventHandler();
		private DataMemoryFlashFinishedEventHandler DataMemoryFlashFinishedEvent;

		public event DataMemoryFlashFinishedEventHandler DataMemoryFlashFinished
		{
			add
			{
				DataMemoryFlashFinishedEvent = (DataMemoryFlashFinishedEventHandler)System.Delegate.Combine(DataMemoryFlashFinishedEvent, value);
			}
			remove
			{
				DataMemoryFlashFinishedEvent = (DataMemoryFlashFinishedEventHandler)System.Delegate.Remove(DataMemoryFlashFinishedEvent, value);
			}
		}

		public delegate void EndProgFinishedEventHandler();
		private EndProgFinishedEventHandler EndProgFinishedEvent;

		public event EndProgFinishedEventHandler EndProgFinished
		{
			add
			{
				EndProgFinishedEvent = (EndProgFinishedEventHandler)System.Delegate.Combine(EndProgFinishedEvent, value);
			}
			remove
			{
				EndProgFinishedEvent = (EndProgFinishedEventHandler)System.Delegate.Remove(EndProgFinishedEvent, value);
			}
		}

		// Files events
		public delegate void StartReadingFileEventHandler(bool bACK, int bytesCount);
		private StartReadingFileEventHandler StartReadingFileEvent;

		public event StartReadingFileEventHandler StartReadingFile
		{
			add
			{
				StartReadingFileEvent = (StartReadingFileEventHandler)System.Delegate.Combine(StartReadingFileEvent, value);
			}
			remove
			{
				StartReadingFileEvent = (StartReadingFileEventHandler)System.Delegate.Remove(StartReadingFileEvent, value);
			}
		}

		public delegate void BlockReadingFileEventHandler(int sequence, byte[] data);
		private BlockReadingFileEventHandler BlockReadingFileEvent;

		public event BlockReadingFileEventHandler BlockReadingFile
		{
			add
			{
				BlockReadingFileEvent = (BlockReadingFileEventHandler)System.Delegate.Combine(BlockReadingFileEvent, value);
			}
			remove
			{
				BlockReadingFileEvent = (BlockReadingFileEventHandler)System.Delegate.Remove(BlockReadingFileEvent, value);
			}
		}

		public delegate void EndReadingFileEventHandler();
		private EndReadingFileEventHandler EndReadingFileEvent;

		public event EndReadingFileEventHandler EndReadingFile
		{
			add
			{
				EndReadingFileEvent = (EndReadingFileEventHandler)System.Delegate.Combine(EndReadingFileEvent, value);
			}
			remove
			{
				EndReadingFileEvent = (EndReadingFileEventHandler)System.Delegate.Remove(EndReadingFileEvent, value);
			}
		}

		public delegate void StartWritingFileEventHandler(bool bOk);
		private StartWritingFileEventHandler StartWritingFileEvent;

		public event StartWritingFileEventHandler StartWritingFile
		{
			add
			{
				StartWritingFileEvent = (StartWritingFileEventHandler)System.Delegate.Combine(StartWritingFileEvent, value);
			}
			remove
			{
				StartWritingFileEvent = (StartWritingFileEventHandler)System.Delegate.Remove(StartWritingFileEvent, value);
			}
		}

		public delegate void BlockWritingFileEventHandler(int sequence, bool bACK);
		private BlockWritingFileEventHandler BlockWritingFileEvent;

		public event BlockWritingFileEventHandler BlockWritingFile
		{
			add
			{
				BlockWritingFileEvent = (BlockWritingFileEventHandler)System.Delegate.Combine(BlockWritingFileEvent, value);
			}
			remove
			{
				BlockWritingFileEvent = (BlockWritingFileEventHandler)System.Delegate.Remove(BlockWritingFileEvent, value);
			}
		}

		public delegate void EndWritingFileEventHandler();
		private EndWritingFileEventHandler EndWritingFileEvent;

		public event EndWritingFileEventHandler EndWritingFile
		{
			add
			{
				EndWritingFileEvent = (EndWritingFileEventHandler)System.Delegate.Combine(EndWritingFileEvent, value);
			}
			remove
			{
				EndWritingFileEvent = (EndWritingFileEventHandler)System.Delegate.Remove(EndWritingFileEvent, value);
			}
		}

		public delegate void FileCountEventHandler(int count);
		private FileCountEventHandler FileCountEvent;

		public event FileCountEventHandler FileCount
		{
			add
			{
				FileCountEvent = (FileCountEventHandler)System.Delegate.Combine(FileCountEvent, value);
			}
			remove
			{
				FileCountEvent = (FileCountEventHandler)System.Delegate.Remove(FileCountEvent, value);
			}
		}

		public delegate void FileNameEventHandler(string fileName);
		private FileNameEventHandler FileNameEvent;

		public event FileNameEventHandler FileName
		{
			add
			{
				FileNameEvent = (FileNameEventHandler)System.Delegate.Combine(FileNameEvent, value);
			}
			remove
			{
				FileNameEvent = (FileNameEventHandler)System.Delegate.Remove(FileNameEvent, value);
			}
		}

		public delegate void DeletedFileNameEventHandler(bool bACK);
		private DeletedFileNameEventHandler DeletedFileNameEvent;

		public event DeletedFileNameEventHandler DeletedFileName
		{
			add
			{
				DeletedFileNameEvent = (DeletedFileNameEventHandler)System.Delegate.Combine(DeletedFileNameEvent, value);
			}
			remove
			{
				DeletedFileNameEvent = (DeletedFileNameEventHandler)System.Delegate.Remove(DeletedFileNameEvent, value);
			}
		}

		public delegate void SelectedFileNameEventHandler(string fileName);
		private SelectedFileNameEventHandler SelectedFileNameEvent;

		public event SelectedFileNameEventHandler SelectedFileName
		{
			add
			{
				SelectedFileNameEvent = (SelectedFileNameEventHandler)System.Delegate.Combine(SelectedFileNameEvent, value);
			}
			remove
			{
				SelectedFileNameEvent = (SelectedFileNameEventHandler)System.Delegate.Remove(SelectedFileNameEvent, value);
			}
		}

		public delegate void SelectedFileEventHandler(bool bACK);
		private SelectedFileEventHandler SelectedFileEvent;

		public event SelectedFileEventHandler SelectedFile
		{
			add
			{
				SelectedFileEvent = (SelectedFileEventHandler)System.Delegate.Combine(SelectedFileEvent, value);
			}
			remove
			{
				SelectedFileEvent = (SelectedFileEventHandler)System.Delegate.Remove(SelectedFileEvent, value);
			}
		}


		public delegate void EndedTransactionEventHandler(uint transactionID);
		private EndedTransactionEventHandler EndedTransactionEvent;

		public event EndedTransactionEventHandler EndedTransaction
		{
			add
			{
				EndedTransactionEvent = (EndedTransactionEventHandler)System.Delegate.Combine(EndedTransactionEvent, value);
			}
			remove
			{
				EndedTransactionEvent = (EndedTransactionEventHandler)System.Delegate.Remove(EndedTransactionEvent, value);
			}
		}

		public delegate void NACKTransactionEventHandler(uint transactionID);
		private NACKTransactionEventHandler NACKTransactionEvent;

		public event NACKTransactionEventHandler NACKTransaction
		{
			add
			{
				NACKTransactionEvent = (NACKTransactionEventHandler) System.Delegate.Combine(NACKTransactionEvent, value);
			}
			remove
			{
				NACKTransactionEvent = (NACKTransactionEventHandler) System.Delegate.Remove(NACKTransactionEvent, value);
			}
		}



		public CStationFrames02_HA(CStationData_HA _StationData, CPortData_HA[] _PortData, CContinuousModeQueueListStation_HA _traceList, CCommunicationChannel _ComChannel, byte _StationNumDevice)
		{

			m_StationData = _StationData;
			m_PortData = _PortData;
			m_traceList = _traceList;
			m_ComChannel = _ComChannel;
			m_ComChannel.MessageResponse += ProcessMessageResponse;
			m_ComChannel.ConnectionError += ComChannelConnectionError;
			m_ComChannel.ResetSended += ComChannelResetSended;
			m_StationNumDevice = _StationNumDevice;
		}


#region CODE FRAMES

#region Generic

#region UID equipo 0x1E 0x1F

		/// <summary>
		/// Lee del equipo conectado su UID
		/// Puede estar en blanco
		/// </summary>
		/// <remarks></remarks>
		internal void ReadDeviceUID()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_DEVICEID;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Permite configurar un UID para el equipo conectado
		/// </summary>
		/// <param name="Value">Tamaño máximo 32 bytes</param>
		/// <remarks></remarks>
		public void WriteDeviceUID(byte[] Value)
		{
			//Datos
			int iLen = Value.Length;
			if (iLen > 32)
			{
				iLen = 32;
			}
			byte[] Datos = null;
			Datos = new byte[iLen - 1 + 1];
			Array.Copy(Value, Datos, iLen);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_DEVICEID;

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Bootloader

#region Clear memory flash 0x22

		/// <summary>
		/// Borra la memoria de programa preparada para almacenar el nuevo programa
		/// </summary>
		/// <param name="firmwareName">Indica el tipo de estación que será, el software que tendrá y el hardware que soportará</param>
		/// <param name="address">Dirección destino</param>
		public void ClearMemoryFlash(string firmwareName, byte address)
		{
			//Datos
			byte[] Datos = Encoding.UTF8.GetBytes(firmwareName);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_CLEARMEMFLASH;

			SendMessage(Datos, Command, address, true); //Delayed response
		}

#endregion


#region Address memory flash 0x23

		/// <summary>
		/// Establece la dirección de memoria a programar
		/// </summary>
		/// <param name="Datos">Dirección de memoria a programar</param>
		/// <param name="address">Dirección destino</param>
		public void AddressMemoryFlash(byte[] Datos, byte address)
		{
			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_SENDMEMADDRESS;

			SendMessage(Datos, Command, address);
		}

#endregion


#region Data memory flash 0x24

		/// <summary>
		/// Envía un bloque de datos a programar
		/// </summary>
		/// <param name="Datos">Bloque de datos a programar</param>
		/// <param name="address">Dirección destino</param>
		public void DataMemoryFlash(byte[] Datos, byte address)
		{
			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_SENDMEMDATA;

			SendMessage(Datos, Command, address);
		}

#endregion


#region End programming 0x25

		/// <summary>
		/// Fin de programación
		/// </summary>
		/// <param name="address">Dirección destino</param>
		public void EndProgramming(byte address)
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_ENDPROGR;

			SendMessage(Datos, Command, address);
		}

#endregion

#endregion


#region Port + Tool

#region Información del puerto 0x30

		/// <summary>
		/// Le pide al Equipo la información del puerto indicado
		/// </summary>
		/// <remarks></remarks>
		internal uint ReadInfoPort(Port Port)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_INF_PORT;

			return SendMessage(Datos, Command);
		}

#endregion


#region Reset puerto/herramienta 0x31

		/// <summary>
		/// Reset de la configuración puerto/herramienta especificada
		/// </summary>
		/// <remarks></remarks>
		internal void ResetPortTool(Port Port, GenericStationTools Tool)
		{
			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_RESET_PORTTOOL;

			SendMessage(Datos, Command);
		}

#endregion


#region Modo de trabajo 0x33 0x34

		/// <summary>
		/// Le pide al Equipo el modo de trabajo, manual o perfil, del puerto
		/// </summary>
		/// <remarks></remarks>
		internal void ReadProfileMode(Port Port)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_PROFILEMODE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el modo de trabajo de perfil, on o off, del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteProfileMode(Port Port, OnOff onoff)
		{
			// mode(1B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[2];
			if (onoff == OnOff._ON)
			{
				Datos[0] = (byte)PortWorkMode_HA.PROFILE;
			}
			else
			{
				Datos[0] = (byte)PortWorkMode_HA.MANUAL;
			}
			Datos[1] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_PROFILEMODE;

			SendMessage(Datos, Command);
		}
#endregion


#region Estado del calefactor 0x35 0x36

		/// <summary>
		/// Le pide al Equipo el estado del calefactor del puerto
		/// </summary>
		/// <remarks></remarks>
		internal void ReadHeaterStatus(Port Port)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_HEATERSTATUS;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el estado del calefactor del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteHeaterStatus(Port Port, PortHeaterStatus_HA status)
		{
			// status(1B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)status;
			Datos[1] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_HEATERSTATUS;

			SendMessage(Datos, Command);
		}
#endregion


#region Estado succión 0x37 0x38

		/// <summary>
		/// Le pide al Equipo el estado de succión del puerto
		/// </summary>
		/// <remarks></remarks>
		internal void ReadSuctionStatus(Port Port)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_SUCTIONSTATUS;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el estado de succión del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteSuctionStatus(Port Port, PortSuctionStatus_HA status)
		{
			// status(1B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)status;
			Datos[1] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_SUCTIONSTATUS;

			SendMessage(Datos, Command);
		}

#endregion


#region Modo configuración TC externo 0x39 0x3A

		/// <summary>
		/// Le pide al Equipo el modo de configuración del TC externo
		/// </summary>
		/// <param name="Port"></param>
		/// <param name="Tool"></param>
		internal void ReadExternalTCMode(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_EXTTCMODE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el modo de configuración del TC externo
		/// </summary>
		/// <remarks></remarks>
		internal void WriteExternalTCMode(Port Port, GenericStationTools Tool, ToolExternalTCMode_HA mode)
		{
			// Mode(1B) + PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[3];
			Datos[0] = (byte)mode;
			Datos[1] = (byte)Port;
			Datos[2] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_EXTTCMODE;

			SendMessage(Datos, Command);
		}

#endregion


#region Niveles de Temperatura/Caudal 0x40 0x41

		/// <summary>
		/// Le pide al Equipo los niveles de temperatura y caudal
		/// </summary>
		/// <remarks></remarks>
		public void ReadLevelsTemps(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_LEVELSTEMPS;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo los niveles de temperatura y caudal
		/// </summary>
		/// <remarks></remarks>
		public void WriteLevelsTemps(Port Port, GenericStationTools Tool, OnOff LevelsOnOff, ToolTemperatureLevels LevelSelected, OnOff Level1OnOff, CTemperature Level1Temp, int Level1Flow, CTemperature Level1ExtTemp, OnOff Level2OnOff, CTemperature Level2Temp, int Level2Flow, CTemperature Level2ExtTemp, OnOff Level3OnOff, CTemperature Level3Temp, int Level3Flow, CTemperature Level3ExtTemp)
		{

			//LevelsOnOff(1B) + LevelSelected(1B) +
			//Level1OnOff(1B) + Level1Temp(2B) + Level1Flow(2B) + Level1ExtTemp(2B) +
			//Level2OnOff(1B) + Level2Temp(2B) + Level2Flow(2B) + Level2ExtTemp(2B) +
			//Level3OnOff(1B) + Level3Temp(2B) + Level3Flow(2B) + Level3ExtTemp(2B) +
			//Port(1B) + Tool(1B)

			//Datos
			byte[] ValueB = null;

			// check levels to be sent to station to avoid NACKs
			CTempLevelsData_HA levels = new CTempLevelsData_HA(3);
			levels.LevelsOnOff = LevelsOnOff;
			levels.LevelsTempSelect = LevelSelected;

			levels.LevelsTempOnOff[0] = Level1OnOff;
			levels.LevelsTemp[0].UTI = Level1Temp.UTI;
			levels.LevelsExtTemp[0].UTI = Level1ExtTemp.UTI;
			levels.LevelsFlow[0] = Level1Flow;

			levels.LevelsTempOnOff[1] = Level2OnOff;
			levels.LevelsTemp[1].UTI = Level2Temp.UTI;
			levels.LevelsExtTemp[1].UTI = Level2ExtTemp.UTI;
			levels.LevelsFlow[1] = Level2Flow;

			levels.LevelsTempOnOff[2] = Level3OnOff;
			levels.LevelsTemp[2].UTI = Level3Temp.UTI;
			levels.LevelsExtTemp[2].UTI = Level3ExtTemp.UTI;
			levels.LevelsFlow[2] = Level3Flow;

			CheckTempLevelsSetting(levels);

			byte[] Datos = new byte[25];
			Datos[0] = (byte)levels.LevelsOnOff;
			Datos[1] = (byte)levels.LevelsTempSelect;

			Datos[2] = (byte)levels.LevelsTempOnOff[0];
			ValueB = BitConverter.GetBytes(System.Convert.ToBoolean(levels.LevelsTemp[0].UTI));
			Datos[3] = ValueB[0];
			Datos[4] = ValueB[1];
			ValueB = BitConverter.GetBytes(levels.LevelsFlow[0]);
			Datos[5] = ValueB[0];
			Datos[6] = ValueB[1];
			ValueB = BitConverter.GetBytes(System.Convert.ToBoolean(levels.LevelsExtTemp[0].UTI));
			Datos[7] = ValueB[0];
			Datos[8] = ValueB[1];

			Datos[9] = (byte)levels.LevelsTempOnOff[1];
			ValueB = BitConverter.GetBytes(System.Convert.ToBoolean(levels.LevelsTemp[1].UTI));
			Datos[10] = ValueB[0];
			Datos[11] = ValueB[1];
			ValueB = BitConverter.GetBytes(levels.LevelsFlow[1]);
			Datos[12] = ValueB[0];
			Datos[13] = ValueB[1];
			ValueB = BitConverter.GetBytes(levels.LevelsExtTemp[1].UTI);
			Datos[14] = ValueB[0];
			Datos[15] = ValueB[1];

			Datos[16] = (byte)levels.LevelsTempOnOff[2];
			ValueB = BitConverter.GetBytes(System.Convert.ToBoolean(levels.LevelsTemp[2].UTI));
			Datos[17] = ValueB[0];
			Datos[18] = ValueB[1];
			ValueB = BitConverter.GetBytes(levels.LevelsFlow[2]);
			Datos[19] = ValueB[0];
			Datos[20] = ValueB[1];
			ValueB = BitConverter.GetBytes(System.Convert.ToBoolean(levels.LevelsExtTemp[2].UTI));
			Datos[21] = ValueB[0];
			Datos[22] = ValueB[1];

			Datos[23] = (byte)Port;
			Datos[24] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_LEVELSTEMPS;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura Ajuste 0x42 0x43

		/// <summary>
		/// Le pide al Equipo la temperatura de Ajuste en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadAjustTemp(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_AJUSTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura de Ajuste en UTI
		/// </summary>
		/// <remarks></remarks>
		public uint WriteAjustTemp(Port Port, GenericStationTools Tool, CTemperature temperature)
		{
			// Value(2B) + PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[4];
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			Datos[2] = (byte)Port;
			Datos[3] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_AJUSTTEMP;

			return SendMessage(Datos, Command);
		}

#endregion


#region Tiempo para detención 0x44 0x45

		/// <summary>
		/// Le pide al Equipo el tiempo para detención
		/// </summary>
		/// <remarks></remarks>
		public void ReadTimeToStop(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_TIMETOSTOP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo retardo en la entrada del sleep
		/// </summary>
		/// <remarks></remarks>
		public uint WriteTimeToStop(Port Port, GenericStationTools Tool, int value)
		{
			// Value(1B) + onoff(1B) + PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[4];
			byte[] ValueB = BitConverter.GetBytes(value);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			Datos[2] = (byte)Port;
			Datos[3] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_TIMETOSTOP;

			return SendMessage(Datos, Command);
		}

#endregion


#region Modo de activación 0x46 0x47

		/// <summary>
		/// Le pide al Equipo el modo de activación de la herramienta del puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadStartMode(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_STARTMODE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el modo de activación de la herramienta del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteStartMode(Port Port, GenericStationTools Tool, CToolStartMode_HA mode)
		{
			// Value(1B) + PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[3];
			Datos[0] = mode.toByte();
			Datos[1] = (byte)Port;
			Datos[2] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_STARTMODE;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura Seleccionada 0x50 0x51

		/// <summary>
		/// Le pide al Equipo la temperatura Seleccionada en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadSelectTemp(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_SELECTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura Seleccionada en UTI
		/// </summary>
		/// <remarks></remarks>
		public uint WriteSelectTemp(Port Port, CTemperature temperature)
		{
			// Value(2B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[3];
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			Datos[2] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_SELECTTEMP;

			return SendMessage(Datos, Command);
		}

#endregion


#region Temperatura del aire (termopar de regulación) 0x52

		/// <summary>
		/// Le pide al Equipo la temperatura del aire en UTI (termopar de regulación)
		/// </summary>
		/// <remarks></remarks>
		internal uint ReadAirTemp(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_AIRTEMP;

			return SendMessage(Datos, Command);
		}

#endregion


#region Potencia de la herramienta 0x54

		/// <summary>
		/// Le pide al Equipo la potencia de la herramienta en tanto por mil
		/// </summary>
		/// <remarks></remarks>
		internal uint ReadPower(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_POWER;

			return SendMessage(Datos, Command);
		}

#endregion


#region Herramienta conectada 0x55

		/// <summary>
		/// Le pide al Equipo la herramienta conectada
		/// </summary>
		/// <remarks></remarks>
		internal uint ReadConnectTool(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_CONNECTTOOL;

			return SendMessage(Datos, Command);
		}

#endregion


#region Error Herramienta 0x56

		/// <summary>
		/// Le pide al Equipo el error de la herramienta conectada
		/// </summary>
		/// <remarks></remarks>
		internal uint ReadToolError(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_TOOLERROR;

			return SendMessage(Datos, Command);
		}

#endregion


#region Estado Herramienta 0x57

		/// <summary>
		/// Le pide al Equipo el estado de la herramienta conectada
		/// </summary>
		/// <remarks></remarks>
		public uint ReadStatusTool(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_STATUSTOOL;

			return SendMessage(Datos, Command);
		}

#endregion


#region Caudal Seleccionado 0x59 0x5A

		/// <summary>
		/// Le pide al Equipo el caudal Seleccionado
		/// </summary>
		/// <remarks></remarks>
		public void ReadSelectFlow(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_SELECTFLOW;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el caudal Seleccionado
		/// </summary>
		/// <remarks></remarks>
		public uint WriteSelectFlow(Port Port, int value)
		{
			// Value(2B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[3];
			byte[] ValueB = BitConverter.GetBytes(value);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			Datos[2] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_SELECTFLOW;

			return SendMessage(Datos, Command);
		}

#endregion


#region Temperatura Externa Seleccionada 0x5B 0x5C

		/// <summary>
		/// Le pide al Equipo la temperatura Seleccionada en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadSelectExternalTemp(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_SELECTEXTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura Seleccionada en UTI
		/// </summary>
		/// <remarks></remarks>
		public uint WriteSelectExternalTemp(Port Port, CTemperature temperature)
		{
			// Value(2B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[3];
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			Datos[2] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_SELECTEXTTEMP;

			return SendMessage(Datos, Command);
		}

#endregion


#region Caudal del aire 0x5D

		/// <summary>
		/// Le pide al Equipo el caudal actual
		/// </summary>
		/// <remarks></remarks>
		public uint ReadAirFlow(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_AIRFLOW;

			return SendMessage(Datos, Command);
		}


#endregion


#region Temperatura del aire del TC externo 0x5F

		/// <summary>
		/// Le pide al Equipo la temperatura del aire del TC externo
		/// </summary>
		/// <remarks></remarks>
		public uint ReadExternalAirTemp(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_EXTTCTEMP;

			return SendMessage(Datos, Command);
		}


#endregion

#endregion


#region Control mode

#region Modo Remoto 0x60 0x61

		/// <summary>
		/// Le pide al Equipo que devuelva el valor del modo remoto
		/// </summary>
		/// <remarks></remarks>
		public void ReadRemoteMode()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_REMOTEMODE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Le pide al Equipo que salga del modo remoto
		/// </summary>
		/// <remarks></remarks>
		public void WriteRemoteMode(OnOff _OnOff)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(_OnOff);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_REMOTEMODE;

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Continuous mode

#region Modo Continuo 0x80 0x81

		/// <summary>
		/// Le pide al Equipo información de la velocidad y los puertos configurados para
		/// el modo continuo
		/// </summary>
		/// <remarks></remarks>
		public void ReadContiMode()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_CONTIMODE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Le pide al Equipo que los puertos activados entren en modo continuo a la velocidad seleccionada
		/// si se quiere desactivar se ha de seleccionar speed OFF
		/// a partir de la activación la estación comenzará a enviar información para su monitorización
		/// Ports On/Off:
		///    bit 0: port1
		///    bit 1: port2
		///    bit 2: port3
		///    bit 3: port4
		/// </summary>
		/// <remarks></remarks>
		public void WriteContiMode(byte startContModePorts, SpeedContinuousMode startContModeSpeed)
		{
			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = System.Convert.ToByte(startContModeSpeed);
			Datos[1] = startContModePorts;

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_CONTIMODE;

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Files methods

#region Read File 0x90 0x91 0x92

		/// <summary>
		/// Le indica al equipo que va a iniciar la lectura de un archivo
		/// </summary>
		/// <remarks></remarks>
		public void ReadFile_Start(string fileName)
		{
			//Datos
			const int maxLength = 12;
			fileName = fileName.Substring(0, maxLength);
			List<byte> Datos = new List<byte>();
			Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
			for (var i = 1; i <= maxLength - Datos.Count; i++)
			{
				Datos.Add(0x0); // rellenar con nulos
			}

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_READSTARTFILE;

			SendMessage(Datos.ToArray(), Command);
		}

		/// <summary>
		/// Pide al equipo un bloque de datos de 128 bytes como máximo
		/// </summary>
		/// <remarks></remarks>
		public void ReadFile_Block(int sequence)
		{
			//Datos
			byte[] Datos = new byte[4];
			Datos = BitConverter.GetBytes(sequence);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_READFILEBLOCK;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Informa al equipo el fin de la lectura del archivo
		/// </summary>
		/// <remarks></remarks>
		public void ReadFile_End()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_READENDOFFILE;

			SendMessage(Datos, Command);
		}

#endregion

#region Write File 0x93 0x94 0x95

		/// <summary>
		/// Le indica al equipo que va a iniciar la escritura de un archivo
		/// </summary>
		/// <remarks></remarks>
		public void WriteFile_Start(string fileName, int length)
		{
			//Datos
			const int maxLength = 12;
			fileName = fileName.Substring(0, maxLength);
			List<byte> Datos = new List<byte>();
			Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
			for (var i = 1; i <= maxLength - Datos.Count; i++)
			{
				Datos.Add(0x0); // rellenar con nulos
			}
			Datos.AddRange(BitConverter.GetBytes(length));

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_WRITESTARTFILE;

			SendMessage(Datos.ToArray(), Command, true); //Delayed response
		}

		/// <summary>
		/// Envía al equipo un bloque de datos de 128 bytes como máximo
		/// </summary>
		/// <remarks></remarks>
		public void WriteFile_Block(int sequence, byte[] blockData)
		{
			//Datos
			const int maxLength = 128;
			List<byte> blockDataBytes = new List<byte>();
			blockDataBytes.AddRange(blockData);
			if (blockDataBytes.Count > maxLength)
			{
				blockDataBytes.RemoveRange(maxLength - 1, blockDataBytes.Count - maxLength);
			}
			List<byte> Datos = new List<byte>();
			Datos.AddRange(BitConverter.GetBytes(sequence));
			Datos.AddRange(blockDataBytes.ToArray());

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_WRITEFILEBLOCK;

			SendMessage(Datos.ToArray(), Command);
		}

		/// <summary>
		/// Informa al equipo el fin de la escritura del archivo
		/// </summary>
		/// <remarks></remarks>
		public void WriteFile_End()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_WRITEENDOFFILE;

			SendMessage(Datos, Command);
		}

#endregion

#region Read File Count 0x96

		/// <summary>
		/// Le pide al equipo la cantidad de archivos que existen en la estación
		/// </summary>
		/// <remarks></remarks>
		public void ReadFileCount(string fileExtension)
		{
			//Datos
			const int maxLength = 3;
			fileExtension = fileExtension.Substring(0, maxLength);
			List<byte> Datos = new List<byte>();
			Datos.AddRange(Encoding.ASCII.GetBytes(fileExtension));

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_FILESCOUNT;

			SendMessage(Datos.ToArray(), Command);
		}

#endregion

#region Read File Name 0x97
		/// <summary>
		/// Le pide al equipo el nombre de un archivo de la estación
		/// </summary>
		/// <remarks></remarks>
		public void ReadFileName(int fileNumber)
		{
			//Datos
			List<byte> Datos = new List<byte>();
			Datos.AddRange(BitConverter.GetBytes(fileNumber));

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_GETFILENAME;

			SendMessage(Datos.ToArray(), Command);
		}
#endregion

#region Delete File 0x98
		/// <summary>
		/// Le pide al equipo el nombre de un archivo de la estación
		/// </summary>
		/// <remarks></remarks>
		public void DeleteFile(string fileName)
		{
			//Datos
			const int maxLength = 12;
			fileName = fileName.Substring(0, maxLength);
			List<byte> Datos = new List<byte>();
			Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
			for (var i = 1; i <= maxLength - Datos.Count; i++)
			{
				Datos.Add(0x0); // rellenar con nulos
			}

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_DELETEFILE;

			SendMessage(Datos.ToArray(), Command);
		}
#endregion

#region Read Selected File 0x9A
		/// <summary>
		/// Le pide al equipo el nombre del archivo seleccionado en la estación
		/// </summary>
		/// <remarks></remarks>
		public void ReadSelectedFile()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_SELECTEDFILENAME;

			SendMessage(Datos, Command);
		}
#endregion

#region Select File 0x9B
		/// <summary>
		/// Le pide al equipo que selecciones un archivo en la estación
		/// </summary>
		/// <remarks></remarks>
		public void SelectFile(string fileName)
		{
			//Datos
			const int maxLength = 12;
			fileName = fileName.Substring(0, maxLength);
			List<byte> Datos = new List<byte>();
			Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
			for (var i = 1; i <= maxLength - Datos.Count; i++)
			{
				Datos.Add(0x0); // rellenar con nulos
			}

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_SELECTFILE;

			SendMessage(Datos.ToArray(), Command);
		}
#endregion

#endregion


#region Station methods

#region Unidades 0xA0 0xA1

		/// <summary>
		/// Le pide al Equipo las unidades de representación de temperaturas
		/// </summary>
		/// <remarks></remarks>
		public void ReadTempUnit()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_TEMPUNIT;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo las unidades de representación de temperaturas
		/// </summary>
		/// <remarks></remarks>
		public void WriteTempUnit(CTemperature.TemperatureUnit units)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(units);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_TEMPUNIT;

			SendMessage(Datos, Command);
		}

#endregion


#region Maxima/Mínima Temperatura 0xA2 0xA3

		/// <summary>
		/// Le pide al Equipo la temperatura máxima y mínima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadMaxMinTemp()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_MAXMINTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura máxima y mínima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public uint WriteMaxMinTemp(CTemperature maximumTemp, CTemperature minimumTemp)
		{
			// Value(2B)+Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			byte[] ValueB = BitConverter.GetBytes(maximumTemp.UTI);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			ValueB = BitConverter.GetBytes(minimumTemp.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_MAXMINTEMP;

			return SendMessage(Datos, Command);
		}

#endregion


#region Maximo/Mínimo Caudal 0xA4 0xA5

		/// <summary>
		/// Le pide al Equipo el caudal máximo y mínimo seleccionable por el equipo
		/// </summary>
		/// <remarks></remarks>
		public void ReadMaxMinFlow()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_MAXMINFLOW;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el caudal máximo y mínimo seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public uint WriteMaxMinFlow(int maximumFlow, int minimumFlow)
		{
			// Value(2B)+Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			byte[] ValueB = BitConverter.GetBytes(maximumFlow);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			ValueB = BitConverter.GetBytes(minimumFlow);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_MAXMINFLOW;

			return SendMessage(Datos, Command);
		}

#endregion


#region Maxima/Mínima Temperatura Externa 0xA6 0xA7

		/// <summary>
		/// Le pide al Equipo la temperatura máxima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadMaxMinExternalTemp()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_MAXMINEXTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura máxima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public uint WriteMaxMinExternalTemp(CTemperature maximumTemp, CTemperature minimumTemp)
		{
			// Value(2B)+Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			byte[] ValueB = BitConverter.GetBytes(maximumTemp.UTI);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			ValueB = BitConverter.GetBytes(minimumTemp.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_MAXMINEXTTEMP;

			return SendMessage(Datos, Command);
		}

#endregion


#region PIN habilitado 0xA8 0xA9

		/// <summary>
		/// Le pide al Equipo si está habilitado el PIN
		/// </summary>
		/// <remarks></remarks>
		public void ReadPINEnabled()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_PINENABLED;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo si se activa o no el PIN
		/// </summary>
		/// <remarks></remarks>
		public void WritePINEnabled(OnOff _onoff)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(_onoff);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_PINENABLED;

			SendMessage(Datos, Command);
		}

#endregion


#region Estación bloqueada 0xAA 0xAB

		/// <summary>
		/// Le pide al Equipo el estado de bloqueo de la estación
		/// </summary>
		/// <remarks></remarks>
		public void ReadStationLocked()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_STATIONLOCKED;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el estado de bloqueo de la estación
		/// </summary>
		/// <remarks></remarks>
		public void WriteStationLocked(OnOff _onoff)
		{
			// Value(2B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(_onoff);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_STATIONLOCKED;

			SendMessage(Datos, Command);
		}

#endregion


#region PIN 0xAC 0xAD

		/// <summary>
		/// Lee del equipo conectado su PIN
		/// </summary>
		/// <remarks></remarks>
		public void ReadDevicePIN()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_PIN;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Permite configurar el PIN del equipo conectado
		/// </summary>
		/// <param name="newPIN"></param>
		/// <remarks></remarks>
		public uint WriteDevicePIN(string newPIN)
		{
			// Value(4b)

			//Datos
			byte[] Datos = null;
			Datos = Encoding.UTF8.GetBytes(newPIN);
			Array.Resize(ref Datos, 4);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_PIN;

			return SendMessage(Datos, Command);
		}

#endregion


#region Error estación 0xAE

		/// <summary>
		/// Le pide al Equipo el error de estación si lo hubiera
		/// </summary>
		/// <remarks></remarks>
		internal void ReadStationError()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_STATERROR;

			SendMessage(Datos, Command);
		}

#endregion


#region Reset Parametros 0xB0

		/// <summary>
		/// Le pide que resetee todos los parámetros de estación y que deje el equipo con la configuración de fábrica
		/// </summary>
		/// <remarks></remarks>
		public void ReadResetParam()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_RESETSTATION;

			SendMessage(Datos, Command);
		}

#endregion


#region Nombre equipo 0xB1 0xB2

		/// <summary>
		/// Lee del equipo conectado su nombre
		/// </summary>
		/// <remarks></remarks>
		public void ReadDeviceName()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_DEVICENAME;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Permite configurar el nombre del equipo conectado
		/// </summary>
		/// <param name="stationName">Tamaño máximo del string 16</param>
		/// <remarks></remarks>
		public void WriteDeviceName(string stationName)
		{
			//Datos
			byte[] Datos = Encoding.UTF8.GetBytes(stationName);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_DEVICENAME;

			SendMessage(Datos, Command);
		}

#endregion


#region Beep 0xB3 0xB4

		/// <summary>
		/// Le pide al Equipo el estado del Beep
		/// </summary>
		/// <remarks></remarks>
		public void ReadBeep()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_BEEP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el Límite de potencia
		/// </summary>
		/// <remarks></remarks>
		public void WriteBeep(OnOff beep)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(beep);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_BEEP;

			SendMessage(Datos, Command);
		}

#endregion

		// falta language
		// falta datetime

#endregion


#region Counters

#region Minutos Conectado 0xC0 0xC1 0xD0 0xD1

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos que la estación está conectada
		/// (aunque no sea necesario se hace por puerto, aunque todos los puertos tengan el mismo valor)
		/// </summary>
		/// <remarks></remarks>
		public void ReadPlugTime(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_PLUGTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_PLUGTIMEP;
			}

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del equipo conectado (por puerto)
		/// </summary>
		/// <remarks></remarks>
		public void WritePlugTime(Port Port, CounterTypes eType, int value)
		{
			// VALUE(4B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[5];
			byte[] ValueB = BitConverter.GetBytes(value);
			Array.Copy(ValueB, 0, Datos, 0, 4);
			Datos[4] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_PLUGTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_PLUGTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Trabajando 0xC2 0xC3 0xD2 0xD3

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto trabajando
		/// </summary>
		/// <remarks></remarks>
		public void ReadWorkTime(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_WORKTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_WORKTIMEP;
			}

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto trabajando
		/// </summary>
		/// <remarks></remarks>
		public void WriteWorkTime(Port Port, CounterTypes eType, int value)
		{
			// VALUE(4B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[5];
			byte[] ValueB = BitConverter.GetBytes(value);
			Array.Copy(ValueB, 0, Datos, 0, 4);
			Datos[4] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_WORKTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_WORKTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos de trabajo 0xC4 0xC5 0xD4 0xD5

		/// <summary>
		/// Le pide al Equipo los ciclos de trabajo del puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadWorkCycles(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_WORKCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_WORKCYCLESP;
			}

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo los ciclos de trabajo del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteWorkCycles(Port Port, CounterTypes eType, int value)
		{
			// VALUE(4B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[5];
			byte[] ValueB = BitConverter.GetBytes(value);
			Array.Copy(ValueB, 0, Datos, 0, 4);
			Datos[4] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_WORKCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_WORKCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos succión 0xC6 0xC7 0xD6 0xD7

		/// <summary>
		/// Le pide al Equipo los ciclos de succión del puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadSuctionCycles(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_SUCTIONCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_R_SUCTIONCYCLESP;
			}

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo los ciclos de succión del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteSuctionCycles(Port Port, CounterTypes eType, int value)
		{
			// VALUE(4B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[5];
			byte[] ValueB = BitConverter.GetBytes(value);
			Array.Copy(ValueB, 0, Datos, 0, 4);
			Datos[4] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_SUCTIONCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_HA.M_W_SUCTIONCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Communications

#region USB Connection Mode 0xE0 0xE1

		/// <summary>
		/// Lee del equipo el estado de la conexión USB:
		///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
		///   * Control Mode: en este estado la estación sólo es configurable desde el PC
		/// </summary>
		/// <remarks></remarks>
		public void ReadConnectStatus()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			Command = (byte)EnumCommandFrame_02_HA.M_R_USB_CONNECTSTATUS;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el equipo el estado de la conexión USB:
		///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
		///   * Control Mode: en este estado la estación sólo es configurable desde el PC
		/// Además envía el nombre del PC
		/// </summary>
		/// <remarks></remarks>
		public void WriteConnectStatus(ControlModeConnection mode, string sMachineName = "")
		{
			//Datos
			byte[] Datos = null;
			string sDatos = "";

			if (sMachineName == "")
			{
				sMachineName = Environment.MachineName;
			}
			sDatos = sMachineName + ":";

			if (mode == ControlModeConnection.CONTROL)
			{
				sDatos += "C";
			}
			else
			{
				sDatos += "M";
			}
			Datos = Encoding.UTF8.GetBytes(sDatos);

			//Command
			byte Command = 0;
			Command = (byte)EnumCommandFrame_02_HA.M_W_USB_CONNECTSTATUS;

			SendMessage(Datos, Command);
		}

#endregion


#region Configuración Robot 0xF0 0xF1

		public void ReadRobotConfiguration()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_RBT_CONNCONFIG;

			SendMessage(Datos, Command);
		}

		public uint WriteRobotConfiguration(CRobotData robotData)
		{
			//Datos
			byte[] Datos = new byte[7];

			Datos[0] = System.Convert.ToByte(robotData.Speed);
			Datos[1] = (byte)(robotData.DataBits + 48);

			if (robotData.Parity == CRobotData.RobotParity.Odd)
			{
				Datos[2] = (byte)(Strings.AscW("O"));
			}
			else if (robotData.Parity == CRobotData.RobotParity.Even)
			{
				Datos[2] = (byte)(Strings.AscW("E"));
			}
			else
			{
				Datos[2] = (byte)(Strings.AscW("N"));
			}

			Datos[3] = System.Convert.ToByte(robotData.StopBits);
			Datos[4] = System.Convert.ToByte(robotData.Protocol);
			//big endian
			Datos[5] = System.Convert.ToByte((robotData.Address / 10) + 48);
			Datos[6] = System.Convert.ToByte((robotData.Address % 10) + 48);

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_RBT_CONNCONFIG;

			return SendMessage(Datos, Command);
		}

#endregion


#region Estado Robot 0xF2 0xF3

		public void ReadRobotStatus()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_R_RBT_CONNECTSTATUS;

			SendMessage(Datos, Command);
		}

		public void WriteRobotStatus(OnOff status)
		{
			//Datos
			byte[] Datos = new byte[1];

			if (status == OnOff._ON)
			{
				Datos[0] = (byte)(Strings.AscW("C"));
			}
			else
			{
				Datos[0] = (byte)(Strings.AscW("N"));
			}

			//Command
			byte Command = (byte)EnumCommandFrame_02_HA.M_W_RBT_CONNECTSTATUS;

			SendMessage(Datos, Command);
		}

#endregion

#endregion

#endregion


#region DECODE FRAMES

		public void ProcessMessageResponse(uint Numstream, byte[] Datos, byte Command, byte Origen)
		{

			//
			//Check source address
			//
			if (Origen != m_StationNumDevice)
			{
				//Para las sub estaciones sólo escuchamos a la siguiente dirección (DME(0x10) -> PSE(0x20) -> PSE (0x30))
				if (m_StationData.Info.Features.SubStations && (m_StationNumDevice + (int)EnumAddress.NEXT_SUBSTATION_ADDRESS) == Origen)
				{

					//Las direcciones que vengan del mismo bloque de estación
				}
				else if ((Origen & (byte)EnumAddress.MASK_STATION_ADDRESS) == m_StationNumDevice)
				{

				}
				else
				{
					return;
				}
			}

			//
			//Sent message
			//
			MessageHashtable.Message MessageInt = new MessageHashtable.Message();
			int Puerto = 0;
			int Tool = 0;
			string sDatos = "";
			string[] aDatos = null;
			string sTemp = "";
			byte[] bytes = null;
			List<byte> listBytes = new List<byte>();

			//Recuperar mensaje
			if (m_MessagesSentManager.ReadMessage(Numstream, ref MessageInt))
			{
				m_MessagesSentManager.RemoveMessage(Numstream);

				//Obtener puerto y tool
				if (MessageInt.Datos.Length == 1)
				{
					Puerto = MessageInt.Datos[0];
				}
				else if (MessageInt.Datos.Length == 2)
				{
					Puerto = MessageInt.Datos[0];
					Tool = SearchToolArray(GetGenericToolFromInternal(MessageInt.Datos[1]));
				}

				//Mensajes recibidos sin petición. (Firmware -> sub estación)
				//Para la DME touch enviamos las peticiones del modo continuo al dspic33 en vez de a la estación
			}
			else if (Command != (byte)EnumCommandFrame_02_HA.M_HS &&
					Command != (byte)EnumCommandFrame_02_HA.M_NACK &&
					Command != (byte)EnumCommandFrame_02_HA.M_I_CONTIMODE &&
					Command != (byte)EnumCommandFrame_02_HA.M_R_CONTIMODE &&
					Command != (byte)EnumCommandFrame_02_HA.M_W_CONTIMODE &&
					Command != (byte)EnumCommandFrame_02_HA.M_FIRMWARE)
			{
				return;
			}


			//
			//Command
			//
			if (Command == (byte)EnumCommandFrame_02_HA.M_HS)
			{
				//Si se recibe una trama de handshake de otra dirección, es posible que se trate de otra estación
				if (Origen != m_StationNumDevice)
				{
					ReadDeviceVersions(Origen);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_EOT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_ACK)
			{
				if (EndedTransactionEvent != null)
					EndedTransactionEvent(Numstream);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_NACK)
			{
				Debug.Print("NACK Error: {0} Sent opcode: H{1:X2}", (Datos[0]).ToString(), Datos[1]);
				if (NACKTransactionEvent != null)
					NACKTransactionEvent(Numstream);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_SYN)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_RESET)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_FIRMWARE)
			{
				if (Datos.Length == 0)
				{
				}

				string datosVersions = Encoding.UTF8.GetString(Datos);
				string[] arrayVersions = datosVersions.Split(':');
				if (arrayVersions.Length < 4)
				{
				}

				//protocolo
				string protocolo = arrayVersions[0];

				//modelo
				string model = arrayVersions[1];
				string[] arrayModelData = arrayVersions[1].Split('_');
				string modelModel = arrayModelData[0];

				string modelType = "";
				if (arrayModelData.Length > 1)
				{
					modelType = arrayModelData[1];
				}

				int modelVersion = -1;
				if (arrayModelData.Length > 2)
				{
					if (Information.IsNumeric(arrayModelData[2]))
					{
						modelVersion = int.Parse(arrayModelData[2]);
					}
				}

				//soft y hard versions
				string versionSw = arrayVersions[2];
				string versionHw = arrayVersions[3];

				//La trama viene del micro principal
				if (Origen == m_StationNumDevice)
				{
					m_StationData.Info.Protocol = protocolo;
					m_StationData.Info.Model = modelModel;
					m_StationData.Info.ModelType = modelType;
					m_StationData.Info.ModelVersion = modelVersion;
					m_StationData.Info.Version_Software = versionSw;
					m_StationData.Info.Version_Hardware = versionHw;

					//La trama viene de una estación conectada a otra estación (p.e. DME -> PSE -> PSE -> PSE)
				}
				else if (((Origen & (byte)EnumAddress.MASK_STATION_ADDRESS) == Origen) && CheckStationModel(model))
				{
					CConnectionData connectionData = new CConnectionData();
					//connectionData.Mode -> no es necesario por que será el mismo que el padre
					//connectionData.pSerialPort -> no es necesario por que será el mismo que el padre
					//connectionData.PCNumDevice -> no es necesario por que será el mismo que el padre
					connectionData.StationNumDevice = Origen;
					//connectionData.FrameProtocol = -> no es necesario por que será el mismo que el padre
					//connectionData.CommandProtocol -> no es necesario por que será el mismo que el padre
					connectionData.StationModel = model;
					connectionData.SoftwareVersion = versionSw;
					connectionData.HardwareVersion = versionHw;

					if (Detected_SubStationEvent != null)
						Detected_SubStationEvent(connectionData);
				}


				//Guardamos la información del micro
				if ((Origen & (byte)EnumAddress.MASK_STATION_ADDRESS) == m_StationNumDevice)
				{

					//Guardar información del micro
					if (!m_StationData.Info.StationMicros.Contains(Origen))
					{
						CFirmwareStation micro = new CFirmwareStation();
						micro.Model = m_StationData.Info.Model;
						micro.HardwareVersion = versionHw;
						micro.SoftwareVersion = versionSw;
						m_StationData.Info.StationMicros.Add(Origen, micro);
					}
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_CLEARMEMFLASH)
			{
				if (ClearingFlashFinishedEvent != null)
					ClearingFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_SENDMEMADDRESS)
			{
				if (AddressMemoryFlashFinishedEvent != null)
					AddressMemoryFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_SENDMEMDATA)
			{
				if (DataMemoryFlashFinishedEvent != null)
					DataMemoryFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_ENDPROGR)
			{
				if (EndProgFinishedEvent != null)
					EndProgFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_INF_PORT)
			{

				if (Datos.Length != 14 & Datos.Length != 16)
				{
					return;
				}

				// en Datos(13) viene el port
				m_PortData[Puerto].ToolStatus.ConnectedTool = GetGenericToolFromInternal(Datos[0]);
				m_PortData[Puerto].ToolStatus.ToolError = GetToolErrorFromInternal(Datos[1]);
				m_PortData[Puerto].ToolStatus.ActualTemp.UTI = BitConverter.ToUInt16(Datos, 2);
				m_PortData[Puerto].ToolStatus.ProtectionTC_Temp.UTI = BitConverter.ToUInt16(Datos, 4);
				//Debug.Print(String.Format("Tool {0} M_INF_PORT: Puerto {1} 2: {2:X}={3} 4: {4:X}={5}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Datos(2), BitConverter.ToUInt16(Datos, 2).ToString, Datos(4), BitConverter.ToUInt16(Datos, 4)))
				m_PortData[Puerto].ToolStatus.Power_x_Mil = BitConverter.ToUInt16(Datos, 6);
				m_PortData[Puerto].ToolStatus.Flow_x_Mil = BitConverter.ToUInt16(Datos, 8);
				m_PortData[Puerto].ToolStatus.TimeToStop = BitConverter.ToUInt16(Datos, 10);
				// tool status
				byte byteStatus = Datos[12];
				m_PortData[Puerto].ToolStatus.HeaterStatus_OnOff = (OnOff)(byteStatus & 0x1);
				m_PortData[Puerto].ToolStatus.HeaterRequestedStatus_OnOff = (OnOff)((byteStatus & 0x2) >> 1);
				m_PortData[Puerto].ToolStatus.CoolingStatus_OnOff = (OnOff)((byteStatus & 0x4) >> 2);
				m_PortData[Puerto].ToolStatus.SuctionStatus_OnOff = (OnOff)((byteStatus & 0x8) >> 3);
				m_PortData[Puerto].ToolStatus.SuctionRequestedStatus_OnOff = (OnOff)((byteStatus & 0x10) >> 4);
				m_PortData[Puerto].ToolStatus.PedalConnected_OnOff = (OnOff)((byteStatus & 0x20) >> 5);
				m_PortData[Puerto].ToolStatus.PedalStatus_OnOff = (OnOff)((byteStatus & 0x40) >> 6);
				m_PortData[Puerto].ToolStatus.Stand_OnOff = (OnOff)((byteStatus & 0x80) >> 7);

				//Debug.Print(String.Format("Tool {0} M_INF_PORT: Puerto {1} ToUInt16(Datos, 6) {2} ToUInt16(Datos, 8): {3}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(0).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(1).ToString))
				//Debug.Print(String.Format("Tool {0} M_INF_PORT: Puerto {1} 6: {2:X} 7: {3:X} 8: {4:X} 9: {5:X}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Datos(6), Datos(7), Datos(8), Datos(9)))
				//Debug.Print("M_INF_PORT - Puerto: {0} Estado: {1}", Puerto.ToString, bytesToBinary(byteStatus))

				if (Datos.Length == 16)
				{

					m_StationData.Info.Features.ChangesStatusInformation = true;

					//Temperatura seleccionada modificado
					if ((Datos[15] & 0x1) > 0)
					{
						if (Changed_SelectedTemperatureEvent != null)
							Changed_SelectedTemperatureEvent();
					}

					//Parámetros estación modificados
					if ((Datos[15] & 0x2) > 0)
					{
						if (Changed_StationParametersEvent != null)
							Changed_StationParametersEvent();
					}

					//Parámetros herramienta puerto 0 modificados
					if ((Datos[15] & 0x4) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(0);
					}

					//Parámetros herramienta puerto 1 modificados
					if ((Datos[15] & 0x8) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(1);
					}

					//Parámetros herramienta puerto 2 modificados
					if ((Datos[15] & 0x10) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(2);
					}

					//Parámetros herramienta puerto 3 modificados
					if ((Datos[15] & 0x20) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(3);
					}

					//Counters modificados
					if ((Datos[15] & 0x80) > 0)
					{
						if (Changed_CountersEvent != null)
							Changed_CountersEvent();
					}
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_PROFILEMODE)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) vienen el port
				if (((PortWorkMode_HA)(Datos[0])) == PortWorkMode_HA.PROFILE)
				{
					m_PortData[Puerto].ToolStatus.ProfileMode = OnOff._ON;
				}
				else
				{
					m_PortData[Puerto].ToolStatus.ProfileMode = OnOff._OFF;
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_HEATERSTATUS)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el port
				if (((PortHeaterStatus_HA)(Datos[0])) == PortHeaterStatus_HA.HEATER_ON)
				{
					m_PortData[Puerto].ToolStatus.HeaterStatus_OnOff = OnOff._ON;
					m_PortData[Puerto].ToolStatus.CoolingStatus_OnOff = OnOff._OFF;
				}
				else if (((PortHeaterStatus_HA)(Datos[0])) == PortHeaterStatus_HA.HEATER_OFF)
				{
					m_PortData[Puerto].ToolStatus.HeaterStatus_OnOff = OnOff._OFF;
					m_PortData[Puerto].ToolStatus.CoolingStatus_OnOff = OnOff._OFF;
				}
				else if (((PortHeaterStatus_HA)(Datos[0])) == PortHeaterStatus_HA.COOLING)
				{
					m_PortData[Puerto].ToolStatus.CoolingStatus_OnOff = OnOff._ON;
					m_PortData[Puerto].ToolStatus.HeaterStatus_OnOff = OnOff._OFF;
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SUCTIONSTATUS)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el port
				if (((PortSuctionStatus_HA)(Datos[0])) == PortSuctionStatus_HA.SUCTION_ON)
				{
					m_PortData[Puerto].ToolStatus.SuctionStatus_OnOff = OnOff._ON;
				}
				else if (((PortSuctionStatus_HA)(Datos[0])) == PortSuctionStatus_HA.SUCTION_OFF)
				{
					m_PortData[Puerto].ToolStatus.SuctionStatus_OnOff = OnOff._OFF;
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SUCTIONSTATUS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_EXTTCMODE)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(1) y Datos(2) vienen el port y la tool
				m_PortData[Puerto].ToolSettings[Tool].ExternalTCMode = (ToolExternalTCMode_HA)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_EXTTCMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_LEVELSTEMPS)
			{
				if (Datos.Length != 25)
				{
					return;
				}

				// en Datos(23) y Datos(24) vienen el port y la tool
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsOnOff = (OnOff)(Datos[0]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempSelect = (ToolTemperatureLevels)(Datos[1]);

				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempOnOff[0] = (OnOff)(Datos[2]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[0].UTI = BitConverter.ToUInt16(Datos, 3);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsFlow[0] = BitConverter.ToUInt16(Datos, 5);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsExtTemp[0].UTI = BitConverter.ToUInt16(Datos, 7);

				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempOnOff[1] = (OnOff)(Datos[9]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[1].UTI = BitConverter.ToUInt16(Datos, 10);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsFlow[1] = BitConverter.ToUInt16(Datos, 12);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsExtTemp[1].UTI = BitConverter.ToUInt16(Datos, 14);

				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempOnOff[2] = (OnOff)(Datos[16]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[2].UTI = BitConverter.ToUInt16(Datos, 17);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsFlow[2] = BitConverter.ToUInt16(Datos, 19);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsExtTemp[2].UTI = BitConverter.ToUInt16(Datos, 21);

				// Check temp levels. Stations do not check levels data until tool is connected
				// FALTA REVISAR
				//checkTempLevels(CType(Puerto, Port), Info_Port(Puerto).ToolParam.ToolSettings(Tool).Tool)
				//Debug.Print(String.Format("M_R_LEVELTEMP : {0} ", Info_Port(Puerto).ToolParam.ToolSettings(Tool).LevelsTempSelect.ToString))
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_LEVELSTEMPS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_AJUSTTEMP)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				// en Datos(2) y Datos(3) vienen el port y la tool
				m_PortData[Puerto].ToolSettings[Tool].AdjustTemp.UTI = BitConverter.ToInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_AJUSTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_TIMETOSTOP)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				// en Datos(2) y Datos(3) vienen el port y la tool
				m_PortData[Puerto].ToolSettings[Tool].TimeToStop = BitConverter.ToInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_TIMETOSTOP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_STARTMODE)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(1) y Datos(2) vienen el port y la tool
				CToolStartMode_HA startMode = new CToolStartMode_HA(Datos[0]);
				m_PortData[Puerto].ToolSettings[Tool].StartMode = startMode;
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_STARTMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SELECTTEMP)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el port
				m_PortData[Puerto].ToolStatus.SelectedTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SELECTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_AIRTEMP)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el port
				m_PortData[Puerto].ToolStatus.ActualTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_POWER)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el port
				m_PortData[Puerto].ToolStatus.Power_x_Mil = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_CONNECTTOOL)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el port
				m_PortData[Puerto].ToolStatus.ConnectedTool = GetGenericToolFromInternal(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_TOOLERROR)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el port
				m_PortData[Puerto].ToolStatus.ToolError = GetToolErrorFromInternal(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_STATUSTOOL)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el puerto
				m_PortData[Puerto].ToolStatus.HeaterStatus_OnOff = (OnOff)(Datos[0] & 0x1);
				m_PortData[Puerto].ToolStatus.HeaterRequestedStatus_OnOff = (OnOff)((Datos[0] & 0x2) >> 1);
				m_PortData[Puerto].ToolStatus.CoolingStatus_OnOff = (OnOff)((Datos[0] & 0x4) >> 2);
				m_PortData[Puerto].ToolStatus.SuctionStatus_OnOff = (OnOff)((Datos[0] & 0x8) >> 3);
				m_PortData[Puerto].ToolStatus.SuctionRequestedStatus_OnOff = (OnOff)((Datos[0] & 0x10) >> 4);
				m_PortData[Puerto].ToolStatus.PedalConnected_OnOff = (OnOff)((Datos[0] & 0x20) >> 5);
				m_PortData[Puerto].ToolStatus.PedalStatus_OnOff = (OnOff)((Datos[0] & 0x40) >> 6);
				m_PortData[Puerto].ToolStatus.Stand_OnOff = (OnOff)((Datos[0] & 0x80) >> 7);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SELECTFLOW)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el port
				m_PortData[Puerto].ToolStatus.SelectedFlow_x_Mil = BitConverter.ToUInt16(Datos, 0);
				//Debug.Print(String.Format("M_R_SELECTFLOW : {0} ", m_PortData(Puerto).SelectedFlow_x_Mil.ToString))
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SELECTFLOW)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SELECTEXTTEMP)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el port
				m_PortData[Puerto].ToolStatus.SelectedExtTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SELECTEXTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_AIRFLOW)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el port
				m_PortData[Puerto].ToolStatus.Flow_x_Mil = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_EXTTCTEMP)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el port
				int temp = BitConverter.ToUInt16(Datos, 0);
				if (temp == Constants.NO_EXT_TC)
				{
					temp = 0;
				}
				m_PortData[Puerto].ToolStatus.ActualExtTemp.UTI = temp;
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_REMOTEMODE)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Status.RemoteMode = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_REMOTEMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_CONTIMODE)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				if (Origen == m_StationNumDevice)
				{
					//m_StationData.ContinuousModeSpeed = CType(Datos(0), SpeedContinuousMode)
					//m_StationData.ContinuousModePorts = Datos(1)
					m_StationData.Status.ContinuousModeStatus.speed = (SpeedContinuousMode)(Datos[0]);
					m_StationData.Status.ContinuousModeStatus.setPortsFromByte(Datos[1]);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_CONTIMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_I_CONTIMODE)
			{

				if (Origen == m_StationNumDevice)
				{
					//Dim start As ULong = CType(Environment.TickCount, ULong)
					//getting the active ports
					CContinuousModeStatus status = (CContinuousModeStatus) (m_StationData.Status.ContinuousModeStatus.Clone());
					//status.port1 = ((m_StationData.ContinuousModePorts And &H1) = 1)
					//status.port2 = ((m_StationData.ContinuousModePorts And &H2) = 2)
					//status.port3 = ((m_StationData.ContinuousModePorts And &H4) = 4)
					//status.port4 = ((m_StationData.ContinuousModePorts And &H8) = 8)
					//status.speed = m_StationData.ContinuousModeSpeed
					//Dim nPorts As Integer = CInt(CByte(status.port1) And &H1) + CInt(CByte(status.port2) And &H1) + CInt(CByte(status.port3) And &H1) + CInt(CByte(status.port4) And &H1)
					int nPorts = status.portCount();
					Port[] ports = new Port[nPorts - 1 + 1];
					bool[] aux = new bool[] { status.port1, status.port2, status.port3, status.port4 };
					int k = 0;
					for (int i = 0; i <= 3; i++)
					{
						if (aux[i])
						{
							ports[k] = (Port)((Port)i);
						}
						k++;
					}

					// length of data
					int iPortDataBlock = 14;

					//checking the recieved data length with the number of ports
					if (Datos.Length >= 1 + 1 * iPortDataBlock)
					{
						//getting the ports data
						stContinuousModeData_HA data = new stContinuousModeData_HA();
						data.data = new stContinuousModePort_HA[nPorts - 1 + 1];
						int offset = 0;
						int val1 = 0;
						//Dim byte1 As Byte
						for (int i = 0; i <= nPorts - 1; i++)
						{
							data.data[i].port = ports[i];
							offset = 1 + i * iPortDataBlock;
							if (offset < Datos.Length)
							{

								//
								// temperature
								//
								val1 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 1]) << 8) | System.Convert.ToInt32(Datos[offset]));
								data.data[i].temperature = new CTemperature(val1);

								//
								// flow
								//
								offset = 1 + i * iPortDataBlock + 2;
								val1 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 1]) << 8) | System.Convert.ToInt32(Datos[offset]));
								data.data[i].flow = val1;

								//
								// power
								//
								offset = 1 + i * iPortDataBlock + 2 + 2;
								val1 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 1]) << 8) | System.Convert.ToInt32(Datos[offset]));
								data.data[i].power = val1;

								//
								// external TC1 temp
								//
								offset = 1 + i * iPortDataBlock + 2 + 2 + 2;
								val1 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 1]) << 8) | System.Convert.ToInt32(Datos[offset]));
								if (val1 == Constants.NO_EXT_TC)
								{
									val1 = 0;
								}
								data.data[i].externalTC1_Temp = new CTemperature(val1);

								//
								// external TC2 temp
								//
								offset = 1 + i * iPortDataBlock + 2 + 2 + 2 + 2;
								val1 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 1]) << 8) | System.Convert.ToInt32(Datos[offset]));
								if (val1 == Constants.NO_EXT_TC)
								{
									val1 = 0;
								}
								data.data[i].externalTC2_Temp = new CTemperature(val1);

								//
								// time to stop
								//
								offset = 1 + i * iPortDataBlock + 2 + 2 + 2 + 2 + 2;
								val1 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 1]) << 8) | System.Convert.ToInt32(Datos[offset]));
								data.data[i].timeToStop = val1;

								//
								// status
								//
								offset = 1 + i * iPortDataBlock + 2 + 2 + 2 + 2 + 2 + 2;
								//Public Enum ToolStatus_HA
								//	Bit 7	Tool in stand
								//	Bit 6	Pedal status
								//	Bit 5	Pedal conectado
								//	Bit 4	Suction status solicitado
								//	Bit 3	Suction status real
								//	Bit 2	Cooling
								//	Bit 1	Heater status solicitado
								//	Bit 0	Heater status real

								if ((Datos[offset] & 0x1) == (byte)ToolStatus_HA.HEATER)
								{
									//Console.WriteLine("      HEATER")
									data.data[i].status = ToolStatus_HA.HEATER;
								}
								else if ((Datos[offset] & 0x2) == (byte)ToolStatus_HA.HEATER_REQUESTED)
								{
									//Console.WriteLine("      HEATER REQUESTED")
									data.data[i].status = ToolStatus_HA.HEATER_REQUESTED;
								}
								else if ((Datos[offset] & 0x4) == (byte)ToolStatus_HA.COOLING)
								{
									//Console.WriteLine("      COOLING")
									data.data[i].status = ToolStatus_HA.COOLING;
								}
								else if ((Datos[offset] & 0x8) == (byte)ToolStatus_HA.SUCTION)
								{
									//Console.WriteLine("      SUCTION")
									data.data[i].status = ToolStatus_HA.SUCTION;
								}
								else if ((Datos[offset] & 0x16) == (byte)ToolStatus_HA.SUCTION_REQUESTED)
								{
									//Console.WriteLine("      SUCTION REQUESTED")
									data.data[i].status = ToolStatus_HA.SUCTION_REQUESTED;
								}
								else if ((Datos[offset] & 0x32) == (byte)ToolStatus_HA.PEDAL_CONNECTED)
								{
									//Console.WriteLine("      PEDAL CONNECTED")
									data.data[i].status = ToolStatus_HA.PEDAL_CONNECTED;
								}
								else if ((Datos[offset] & 0x64) == (byte)ToolStatus_HA.PEDAL_PRESSED)
								{
									//Console.WriteLine("      PEDAL PRESSED")
									data.data[i].status = ToolStatus_HA.PEDAL_PRESSED;
								}
								else if ((Datos[offset] & 0x128) == (byte)ToolStatus_HA.STAND)
								{
									//Console.WriteLine("      STAND")
									data.data[i].status = ToolStatus_HA.STAND;
								}
								else
								{
									//Console.WriteLine("      NONE")
									data.data[i].status = ToolStatus_HA.NONE;
								}

								//Debug.Print("CONT MODE STATUS prot 02: {0}", data.data(i).status.ToString)
							}
						}

						// adding the continuous data to all current buffers (multi-buffer)
						// passing frame sequence in data.sequence (user sequence is calculated inside traceList class)
						data.sequence = Datos[0];
						m_traceList.AddData(data);
					}
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_READSTARTFILE)
			{
				if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
				{
					if (StartReadingFileEvent != null)
						StartReadingFileEvent(true, BitConverter.ToInt32(Datos, 1));
				}
				else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
				{
					if (StartReadingFileEvent != null)
						StartReadingFileEvent(false, 0);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_READFILEBLOCK)
			{
				bytes = new byte[Datos.Length - 4 - 1 + 1];
				Array.Copy(Datos, 4, bytes, 0, bytes.Length);
				if (BlockReadingFileEvent != null) // sequence, data
					BlockReadingFileEvent(BitConverter.ToInt32(Datos, 0), bytes);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_READENDOFFILE)
			{
				if (EndReadingFileEvent != null)
					EndReadingFileEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_WRITESTARTFILE)
			{
				if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
				{
					if (StartWritingFileEvent != null)
						StartWritingFileEvent(true);
				}
				else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
				{
					if (StartWritingFileEvent != null)
						StartWritingFileEvent(false);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_WRITEFILEBLOCK)
			{
				if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
				{
					if (BlockWritingFileEvent != null)
						BlockWritingFileEvent(BitConverter.ToInt32(Datos, 0), true);
				}
				else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
				{
					if (BlockWritingFileEvent != null)
						BlockWritingFileEvent(BitConverter.ToInt32(Datos, 0), false);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_WRITEENDOFFILE)
			{
				if (EndWritingFileEvent != null)
					EndWritingFileEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_FILESCOUNT)
			{
				if (FileCountEvent != null)
					FileCountEvent(BitConverter.ToInt32(Datos, 0));
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_GETFILENAME)
			{
				listBytes.Clear();
				foreach (byte byt in Datos)
				{
					if (byt != 0x0)
					{
						listBytes.Add(byt);
					}
				}
				if (FileNameEvent != null)
					FileNameEvent(Encoding.ASCII.GetString(listBytes.ToArray()));
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_DELETEFILE)
			{
				if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
				{
					if (DeletedFileNameEvent != null)
						DeletedFileNameEvent(true);
				}
				else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
				{
					if (DeletedFileNameEvent != null)
						DeletedFileNameEvent(false);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SELECTEDFILENAME)
			{
				listBytes.Clear();
				foreach (byte byt in Datos)
				{
					if (byt != 0x0)
					{
						listBytes.Add(byt);
					}
				}
				m_StationData.Settings.SelectedProfile = Encoding.ASCII.GetString(listBytes.ToArray());
				if (SelectedFileNameEvent != null)
					SelectedFileNameEvent(Encoding.ASCII.GetString(listBytes.ToArray()));
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SELECTFILE)
			{
				if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
				{
					if (SelectedFileEvent != null)
						SelectedFileEvent(true);
				}
				else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
				{
					if (SelectedFileEvent != null)
						SelectedFileEvent(false);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_TEMPUNIT)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.Unit = (CTemperature.TemperatureUnit)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_TEMPUNIT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_MAXMINTEMP)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_StationData.Settings.MaxTemp.UTI = BitConverter.ToUInt16(Datos, 0);
				m_StationData.Settings.MinTemp.UTI = BitConverter.ToUInt16(Datos, 2);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_MAXMINTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_MAXMINFLOW)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_StationData.Settings.MaxFlow = BitConverter.ToUInt16(Datos, 0);
				m_StationData.Settings.MinFlow = BitConverter.ToUInt16(Datos, 2);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_MAXMINFLOW)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_MAXMINEXTTEMP)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_StationData.Settings.MaxExtTemp.UTI = BitConverter.ToUInt16(Datos, 0);
				m_StationData.Settings.MinExtTemp.UTI = BitConverter.ToUInt16(Datos, 2);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_MAXMINEXTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_BEEP)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.Beep = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_BEEP)
			{
				//Case EnumCommandFrame_02_HA.M_R_PARAMETERSLOCKED
				//    If Datos.Length <> 1 Then Return

				//    m_StationData.ParametersLocked = CType(Datos(0), OnOff)

				//Case EnumCommandFrame_02_HA.M_W_PARAMETERSLOCKED
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_LANGUAGE)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.Language = GetLanguageFromLangText(Encoding.UTF8.GetString(Datos));
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_LANGUAGE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_PIN)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_StationData.Settings.PIN = Encoding.UTF8.GetString(Datos);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_PIN)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_STATERROR)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Status.ErrorStation = (StationError)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_RESETSTATION)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_DEVICENAME)
			{
				string TextoName = Encoding.UTF8.GetString(Datos);
				m_StationData.Settings.Name = TextoName;
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_DEVICENAME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_DEVICEID)
			{
				clsStationUID stationUUID = new clsStationUID(Datos);
				m_StationData.Info.UUID = stationUUID.UID;
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_DEVICEID)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_PLUGTIME)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContPlugMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_PLUGTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_WORKTIME)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContWorkMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_WORKTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_WORKCYCLES)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContWorkCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_WORKCYCLES)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SUCTIONCYCLES)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContSuctionCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SUCTIONCYCLES)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_PLUGTIMEP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContPlugMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_PLUGTIMEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_WORKTIMEP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContWorkMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_WORKTIMEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_WORKCYCLESP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContWorkCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_WORKCYCLESP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SUCTIONCYCLESP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContSuctionCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SUCTIONCYCLESP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_USB_CONNECTSTATUS)
			{
				if (Datos.Length == 0)
				{
					return;
				}

				sDatos = Encoding.UTF8.GetString(Datos);
				if (sDatos.IndexOf(':') >= 0)
				{
					aDatos = sDatos.Split(':');
					sTemp = aDatos[1].Trim();
				}
				else
				{
					sTemp = sDatos.Trim();
				}

				if (sTemp.ToUpper() == "C")
				{
					m_StationData.Status.ControlMode = ControlModeConnection.CONTROL;
				}
				else
				{
					m_StationData.Status.ControlMode = ControlModeConnection.MONITOR;
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_USB_CONNECTSTATUS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_RBT_CONNCONFIG)
			{
				if (Datos.Length != 7)
				{
					return;
				}

				sDatos = Encoding.UTF8.GetString(Datos);

				m_StationData.Settings.Robot.Speed = (CRobotData.RobotSpeed)Datos[0];

				//Data bits in ASCII or number
				if (Datos[1] >= 0 && Datos[1] <= 9)
				{
					m_StationData.Settings.Robot.DataBits = System.Convert.ToUInt16(Datos[1]);
				}
				else
				{
					m_StationData.Settings.Robot.DataBits = System.Convert.ToUInt16(System.Convert.ToUInt16(Datos[1] - 48));
				}

				switch (sDatos.Substring(2, 1))
				{
					case "E":
					case "e":
						m_StationData.Settings.Robot.Parity = CRobotData.RobotParity.Even;
						break;
					case "O":
					case "o":
						m_StationData.Settings.Robot.Parity = CRobotData.RobotParity.Odd;
						break;
					default:
						m_StationData.Settings.Robot.Parity = CRobotData.RobotParity.None;
						break;
				}

				if (Datos[3] == 2 || sDatos.Substring(3, 1) == "2")
				{
					m_StationData.Settings.Robot.StopBits = CRobotData.RobotStop.bits_2;
				}
				else
				{
					m_StationData.Settings.Robot.StopBits = CRobotData.RobotStop.bits_1;
				}

				if (Datos[4] == 1 || sDatos.Substring(4, 1) == "1")
				{
					m_StationData.Settings.Robot.Protocol = CRobotData.RobotProtocol.RS485;
				}
				else
				{
					m_StationData.Settings.Robot.Protocol = CRobotData.RobotProtocol.RS232;
				}

				//big endian
				ushort nID = default(ushort);
				//ID in ASCII or number
				if (Datos[5] >= 0 && Datos[5] <= 9)
				{
					nID = System.Convert.ToUInt16(System.Convert.ToUInt16(Datos[5] * 10));
					nID += System.Convert.ToUInt16(Datos[6]);
				}
				else
				{
					nID = System.Convert.ToUInt16(System.Convert.ToUInt16((Datos[5] - 48) * 10));
					nID += System.Convert.ToUInt16(System.Convert.ToUInt16(Datos[6] - 48));
				}
				//UInt16.TryParse(sDatos.Substring(5, 2), nID)
				m_StationData.Settings.Robot.Address = nID;
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_RBT_CONNCONFIG)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_R_RBT_CONNECTSTATUS)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				sDatos = Encoding.UTF8.GetString(Datos);

				switch (sDatos)
				{
					case "C":
					case "c":
						m_StationData.Settings.Robot.Status = OnOff._ON;
						break;
					default:
						m_StationData.Settings.Robot.Status = OnOff._OFF;
						break;
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_HA.M_W_RBT_CONNECTSTATUS)
			{
			}
			else
			{
				//No hacer nada
			}
		}

#endregion


#region CHECK PARAMETERS

		// Verificación de Niveles de Temperatura
		// 22/03/2013 checkTempLevels function. stations do not check levels data until tool is connected
		// (FALTA terminar el check levels porque en Desoldador incluye flow y external temp)
		private bool CheckTempLevelsSetting(CTempLevelsData_HA levels)
		{
			bool bOk = true; // si se devuelve bOk = false entonces se ha modificado algo
			bool bSearchForATemp = false;
			bool bFoundATemp = false;
			if (levels.LevelsOnOff == OnOff._ON)
			{
				// si está activado el TempsLevels, ver si es coherente la selección de temperaturas

				// si el nivel seleccionado no tiene temperatura o está desactivado, buscar una temperatura activa
				// (en protocolo 02, creo que siempre hay una temperatura)
				switch (levels.LevelsTempSelect)
				{
					case ToolTemperatureLevels.FIRST_LEVEL:
						if (levels.LevelsTemp[0].UTI == Constants.NO_TEMP_LEVEL |
								levels.LevelsTempOnOff[0] == OnOff._OFF)
						{
							bSearchForATemp = true;
						}
						break;
					case ToolTemperatureLevels.SECOND_LEVEL:
						if (levels.LevelsTemp[1].UTI == Constants.NO_TEMP_LEVEL |
								levels.LevelsTempOnOff[1] == OnOff._OFF)
						{
							bSearchForATemp = true;
						}
						break;
					case ToolTemperatureLevels.THIRD_LEVEL:
						if (levels.LevelsTemp[2].UTI == Constants.NO_TEMP_LEVEL |
								levels.LevelsTempOnOff[2] == OnOff._OFF)
						{
							bSearchForATemp = true;
						}
						break;
					case ToolTemperatureLevels.NO_LEVELS:
						// si no hay nivel seleccionado, buscar una temperatura (si no la estación devuelve NACK)
						bSearchForATemp = true;
						break;
					default:
						bSearchForATemp = true;
						break;
				}

				if (bSearchForATemp)
				{
					bOk = false; // changed some value
					// buscar entre las temperaturas activas con temperatura válida
					if (levels.LevelsTemp[0].UTI != Constants.NO_TEMP_LEVEL &
							levels.LevelsTempOnOff[0] == OnOff._ON)
					{
						levels.LevelsTempSelect = ToolTemperatureLevels.FIRST_LEVEL;
						bFoundATemp = true;
					}
					else if (levels.LevelsTemp[1].UTI != Constants.NO_TEMP_LEVEL &
							levels.LevelsTempOnOff[1] == OnOff._ON)
					{
						levels.LevelsTempSelect = ToolTemperatureLevels.SECOND_LEVEL;
						bFoundATemp = true;
					}
					else if (levels.LevelsTemp[2].UTI != Constants.NO_TEMP_LEVEL &
							levels.LevelsTempOnOff[2] == OnOff._ON)
					{
						levels.LevelsTempSelect = ToolTemperatureLevels.THIRD_LEVEL;
						bFoundATemp = true;
					}

					if (!bFoundATemp)
					{
						// si no se ha encontrado una temperatura activa y válida
						// buscar entre las temperaturas válidas, aunque no estén activas
						if (levels.LevelsTemp[0].UTI != Constants.NO_TEMP_LEVEL)
						{
							levels.LevelsTempSelect = ToolTemperatureLevels.FIRST_LEVEL;
							levels.LevelsTempOnOff[0] = OnOff._ON;
							bFoundATemp = true;
						}
						else if (levels.LevelsTemp[1].UTI != Constants.NO_TEMP_LEVEL)
						{
							levels.LevelsTempSelect = ToolTemperatureLevels.SECOND_LEVEL;
							levels.LevelsTempOnOff[1] = OnOff._ON;
							bFoundATemp = true;
						}
						else if (levels.LevelsTemp[2].UTI != Constants.NO_TEMP_LEVEL)
						{
							levels.LevelsTempSelect = ToolTemperatureLevels.THIRD_LEVEL;
							levels.LevelsTempOnOff[2] = OnOff._ON;
							bFoundATemp = true;
						}
						else
						{
							levels.LevelsTempSelect = ToolTemperatureLevels.FIRST_LEVEL;
							levels.LevelsTemp[0].UTI = Constants.DEFAULT_TEMP;
							levels.LevelsTempOnOff[0] = OnOff._ON;
						}
					}
				}
			}

			return bOk;
		}

#endregion


#region RUTINAS

		protected int SearchToolArray(GenericStationTools Tool)
		{
			int index = 0;

			//busca tool en parámetros
			for (index = 0; index <= m_PortData[0].ToolSettings.Length - 1; index++)
			{
				if (Tool == m_PortData[0].ToolSettings[index].Tool)
				{
					break;
				}
			}

			return index;
		}

		private GenericStationTools GetGenericToolFromInternal(byte Dato)
		{
			switch (m_StationData.Info.StationType)
			{
				case eStationType.HA:
					// desolder
					if (Dato > 0)
					{
						return ((GenericStationTools)(Dato + 30));
					}
					break;
			}
			return ((GenericStationTools)Dato);
		}

		private byte GetInternalToolFromGeneric(GenericStationTools Tool)
		{
			switch (m_StationData.Info.StationType)
			{
				case eStationType.HA:
					// desolder
					if (Tool != GenericStationTools.NO_TOOL)
					{
						return System.Convert.ToByte(Tool - 30);
					}
					break;
			}
			return System.Convert.ToByte(Tool);
		}

		private ToolError GetToolErrorFromInternal(byte myerror)
		{
			switch (m_StationData.Info.StationType)
			{
				case eStationType.HA:
					// desolder
					if (myerror > (int)ToolError.NO_ERROR)
					{
						return ((ToolError)(myerror + 20));
					}
					break;
			}
			return ((ToolError)myerror);
		}
#endregion

	}
}
