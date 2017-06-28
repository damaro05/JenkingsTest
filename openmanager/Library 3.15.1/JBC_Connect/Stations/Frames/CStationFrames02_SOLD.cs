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
	internal class CStationFrames02_SOLD : CStationFrames
	{

		protected CStationData_SOLD m_StationData; //Información de la estación
		protected CPortData_SOLD[] m_PortData; //Información del puerto
		protected CContinuousModeQueueListStation_SOLD m_traceList; //Información del modo continuo

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

		//Parameters changes events
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



		public CStationFrames02_SOLD(CStationData_SOLD _StationData, CPortData_SOLD[] _PortData, CContinuousModeQueueListStation_SOLD _traceList, CCommunicationChannel _ComChannel, byte _StationNumDevice)
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_DEVICEID;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_DEVICEID;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_CLEARMEMFLASH;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_SENDMEMADDRESS;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_SENDMEMDATA;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_ENDPROGR;

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
		internal void ReadInfoPort(Port Port)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_INF_PORT;

			SendMessage(Datos, Command);
		}

#endregion


#region Niveles de Temperatura 0x33 0x34

		/// <summary>
		/// Le pide al Equipo los niveles de temperatura
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_LEVELSTEMPS;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo los niveles de temperatura
		/// </summary>
		/// <remarks></remarks>
		public void WriteLevelsTemps(Port Port, GenericStationTools Tool, OnOff LevelsOnOff, ToolTemperatureLevels LevelSelected, OnOff Level1OnOff, CTemperature Level1Temp, OnOff Level2OnOff, CTemperature Level2Temp, OnOff Level3OnOff, CTemperature Level3Temp)
		{

			// LevelsOnOff(1B) + LevelSelected(1B) + Level1OnOff(1B) + Level1Temp(2B) + Level2OnOff(1B) + Level2Temp(2B) + Level3OnOff(1B) + Level3Temp(2B) + Port(1B) + Tool(1B)

			//Datos
			byte[] ValueB = null;

			// check levels to be sent to station to avoid NACKs
			CTempLevelsData_SOLD levels = new CTempLevelsData_SOLD(3);
			levels.LevelsOnOff = LevelsOnOff;
			levels.LevelsTempSelect = LevelSelected;
			levels.LevelsTempOnOff[0] = Level1OnOff;
			levels.LevelsTemp[0].UTI = Level1Temp.UTI;
			levels.LevelsTempOnOff[1] = Level2OnOff;
			levels.LevelsTemp[1].UTI = Level2Temp.UTI;
			levels.LevelsTempOnOff[2] = Level3OnOff;
			levels.LevelsTemp[2].UTI = Level3Temp.UTI;
			CheckTempLevelsSetting(levels);


			byte[] Datos = new byte[13];
			Datos[0] = (byte)levels.LevelsOnOff;
			Datos[1] = (byte)levels.LevelsTempSelect;

			Datos[2] = (byte)levels.LevelsTempOnOff[0];
			ValueB = BitConverter.GetBytes(levels.LevelsTemp[0].UTI);
			Datos[3] = ValueB[0];
			Datos[4] = ValueB[1];

			Datos[5] = (byte)levels.LevelsTempOnOff[1];
			ValueB = BitConverter.GetBytes(levels.LevelsTemp[1].UTI);
			Datos[6] = ValueB[0];
			Datos[7] = ValueB[1];

			Datos[8] = (byte)levels.LevelsTempOnOff[2];
			ValueB = BitConverter.GetBytes(levels.LevelsTemp[2].UTI);
			Datos[9] = ValueB[0];
			Datos[10] = ValueB[1];

			Datos[11] = (byte)Port;
			Datos[12] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_LEVELSTEMPS;

			SendMessage(Datos, Command);
		}

#endregion


#region Retardo Sleep 0x40 0x41

		/// <summary>
		/// Le pide al Equipo retardo en la entrada del sleep
		/// </summary>
		/// <remarks></remarks>
		public void ReadSleepDelay(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPDELAY;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo retardo en la entrada del sleep
		/// </summary>
		/// <remarks></remarks>
		public void WriteSleepDelay(Port Port, GenericStationTools Tool, ToolTimeSleep value, OnOff onoff)
		{
			// Value(1B) + onoff(1B) + PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)value;
			Datos[1] = (byte)onoff;
			Datos[2] = (byte)Port;
			Datos[3] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPDELAY;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura Sleep 0x42 0x43

		/// <summary>
		/// Le pide al Equipo la temperatura de Sleep en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadSleepTemp(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura de Sleep en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteSleepTemp(Port Port, GenericStationTools Tool, CTemperature temperature)
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Retardo Hibernación 0x44 0x45

		/// <summary>
		/// Le pide al Equipo retardo en la entrada de la hibernación
		/// </summary>
		/// <remarks></remarks>
		public void ReadHiberDelay(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_HIBERDELAY;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo retardo en la entrada de la hibernación
		/// </summary>
		/// <remarks></remarks>
		public void WriteHiberDelay(Port Port, GenericStationTools Tool, ToolTimeHibernation value, OnOff onoff)
		{
			// Value(1B) + onoff(1B) + PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)value;
			Datos[1] = (byte)onoff;
			Datos[2] = (byte)Port;
			Datos[3] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_HIBERDELAY;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura Ajuste 0x46 0x47

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_AJUSTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura de Ajuste en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteAjustTemp(Port Port, GenericStationTools Tool, CTemperature temperature)
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_AJUSTTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Cartucho 0x48 0x49

		/// <summary>
		/// Le pide al Equipo el cartucho del puerto + herramienta
		/// </summary>
		/// <remarks></remarks>
		public void ReadCartridge(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_CARTRIDGE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el cartucho para el puerto + herramienta
		/// </summary>
		/// <remarks></remarks>
		public void WriteCartridge(Port Port, GenericStationTools Tool, CCartridgeData cartridge)
		{
			// OnOff(1B) + cartridge(2B) + adjust300(2B) + adjust400(2B) + grupo(1B) + familia(1B) + PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[11];
			byte[] ValueB = null;

			// onoff
			Datos[0] = System.Convert.ToByte(cartridge.CartridgeOnOff);

			// cartridge
			ValueB = BitConverter.GetBytes(cartridge.CartridgeNbr);
			Datos[1] = ValueB[0];
			Datos[2] = ValueB[1];

			// adjust300
			ValueB = BitConverter.GetBytes(cartridge.CartridgeAdj300);
			Datos[3] = ValueB[0];
			Datos[4] = ValueB[1];

			// adjust400
			ValueB = BitConverter.GetBytes(cartridge.CartridgeAdj400);
			Datos[5] = ValueB[0];
			Datos[6] = ValueB[1];

			// grupo
			Datos[7] = cartridge.CartridgeGroup;

			// familia
			Datos[8] = cartridge.CartridgeFamily;

			// port and tool
			Datos[9] = (byte)Port;
			Datos[10] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_CARTRIDGE;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_SELECTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura Seleccionada en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteSelectTemp(Port Port, CTemperature temperature)
		{
			// Value(2B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[3];
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[0] = ValueB[0];
			Datos[1] = ValueB[1];
			Datos[2] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_SELECTTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura de la punta 0x52

		/// <summary>
		/// Le pide al Equipo la temperatura de la punta en UTI
		/// </summary>
		/// <remarks></remarks>
		internal void ReadTipTemp(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_TIPTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Intensidad del cartucho 0x53

		/// <summary>
		/// Le pide al Equipo la intensidad del cartucho en mA
		/// </summary>
		/// <remarks></remarks>
		internal void ReadCurrent(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_CURRENT;

			SendMessage(Datos, Command);
		}

#endregion


#region Potencia del cartucho 0x54

		/// <summary>
		/// Le pide al Equipo la potencia del cartucho en tanto por mil
		/// </summary>
		/// <remarks></remarks>
		internal void ReadPower(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_POWER;

			SendMessage(Datos, Command);
		}

#endregion


#region Herramienta conectada 0x55

		/// <summary>
		/// Le pide al Equipo la potencia la herramienta conectada
		/// </summary>
		/// <remarks></remarks>
		internal void ReadConnectTool(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_CONNECTTOOL;

			SendMessage(Datos, Command);
		}

#endregion


#region Error Herramienta 0x56

		/// <summary>
		/// Le pide al Equipo el error de la herramienta conectada
		/// </summary>
		/// <remarks></remarks>
		internal void ReadToolError(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_TOOLERROR;

			SendMessage(Datos, Command);
		}

#endregion


#region Estado Herramienta 0x57 0x58

		/// <summary>
		/// Le pide al Equipo el estado de la herramienta conectada
		/// </summary>
		/// <remarks></remarks>
		public void ReadStatusTool(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_STATUSTOOL;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Cambia el estado de la herramienta
		/// </summary>
		/// <remarks></remarks>
		public void WriteStatusTool(Port Port, OnOff Stand_OnOff, OnOff Extractor_OnOff, OnOff Desold_OnOff)
		{
			// STATUS(1B) + PORT(1B)

			//Datos
			byte[] Datos = new byte[2];
			byte Status = (byte)0;

			if (Extractor_OnOff == OnOff._ON)
			{
				Status = (byte)(Status + 0x8);
			}
			else if (Stand_OnOff == OnOff._ON)
			{
				Status = (byte)(Status + 0x1);
			}

			if (Desold_OnOff == OnOff._ON)
			{
				Status = (byte)(Status + 0x10);
			}

			Datos[0] = Status;
			Datos[1] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_STATUSTOOL;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura MOS 0x59

		/// <summary>
		/// Le pide al Equipo la temperatura del MOS del puerto indicado en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadMOSTemp(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_MOSTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Tiempo restante Sleep/Extractor 0x5A

		/// <summary>
		/// Le pide al Equipo el tiempo que falta para entrar en sleep o en extractor
		/// </summary>
		/// <remarks></remarks>
		internal void ReadDelayTime(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_DELAYTIME;

			SendMessage(Datos, Command);
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_REMOTEMODE;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_REMOTEMODE;

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

			if (m_StationData.Info.Features.ContinuousModeDevice11h)
			{
				byte Command = (byte)EnumCommandFrameDSPIC33.M_R_CONTIMODE;
				SendMessage(Datos, Command, (byte)EnumAddress.CONTINUOUS_MODE_DSPIC33_ADDRESS);
			}
			else
			{
				byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_CONTIMODE;
				SendMessage(Datos, Command);
			}
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

			if (m_StationData.Info.Features.ContinuousModeDevice11h)
			{
				byte Command = (byte)EnumCommandFrameDSPIC33.M_W_CONTIMODE;
				SendMessage(Datos, Command, (byte)EnumAddress.CONTINUOUS_MODE_DSPIC33_ADDRESS);
			}
			else
			{
				byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_CONTIMODE;
				SendMessage(Datos, Command);
			}
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_TEMPUNIT;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_TEMPUNIT;

			SendMessage(Datos, Command);
		}

#endregion


#region Maxima Temperatura 0xA2 0xA3

		/// <summary>
		/// Le pide al Equipo la temperatura máxima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadMaxTemp()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_MAXTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura máxima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteMaxTemp(CTemperature temperature)
		{
			// Value(2B)

			//Datos
			byte[] Datos = null;
			Datos = BitConverter.GetBytes(temperature.UTI);
			Array.Resize(ref Datos, 2);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_MAXTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Minima Temperatura 0xA4 0xA5

		/// <summary>
		/// Le pide al Equipo la temperatura mínima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadMinTemp()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_MINTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura mínima seleccionable por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteMinTemp(CTemperature temperature)
		{
			// Value(2B)

			//Datos
			byte[] Datos = null;
			Datos = BitConverter.GetBytes(temperature.UTI);
			Array.Resize(ref Datos, 2);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_MINTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Modo Nitrogeno 0xA6 0xA7

		/// <summary>
		/// Le pide al Equipo el modo Nitrógeno
		/// </summary>
		/// <remarks></remarks>
		public void ReadN2Mode()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_NITROMODE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el modo Nitrógeno
		/// </summary>
		/// <remarks></remarks>
		public void WriteN2Mode(OnOff mode)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(mode);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_NITROMODE;

			SendMessage(Datos, Command);
		}

#endregion


#region Help Text 0xA8 0xA9

		/// <summary>
		/// Le pide al Equipo el Help Text
		/// </summary>
		/// <remarks></remarks>
		public void ReadHelpText()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_HELPTEXT;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el modo Nitrógeno
		/// </summary>
		/// <remarks></remarks>
		public void WriteHelpText(OnOff help)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(help);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_HELPTEXT;

			SendMessage(Datos, Command);
		}

#endregion


#region Power limit 0xAA 0xAB

		/// <summary>
		/// Le pide al Equipo el Límite de potencia
		/// </summary>
		/// <remarks></remarks>
		public void ReadPowerLimit()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_POWERLIM;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el Límite de potencia
		/// </summary>
		/// <remarks></remarks>
		public void WritePowerLimit(int powerLimit)
		{
			// Value(2B)

			//Datos
			byte[] Datos = null;
			Datos = BitConverter.GetBytes(powerLimit);
			Array.Resize(ref Datos, 2);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_POWERLIM;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_PIN;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Permite configurar el PIN del equipo conectado
		/// </summary>
		/// <param name="newPIN"></param>
		/// <remarks></remarks>
		public void WriteDevicePIN(string newPIN)
		{
			// Value(4b)

			//Datos
			byte[] Datos = null;
			Datos = Encoding.UTF8.GetBytes(newPIN);
			Array.Resize(ref Datos, 4);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_PIN;

			SendMessage(Datos, Command);
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_STATERROR;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura Trafo 0xAF

		/// <summary>
		/// Le pide al Equipo la temperatura del transformador en UTI
		/// </summary>
		/// <remarks></remarks>
		internal void ReadTrafoTemp()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_TRAFOTEMP;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_RESETSTATION;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_DEVICENAME;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_DEVICENAME;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_BEEP;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_BEEP;

			SendMessage(Datos, Command);
		}

#endregion


#region Parameters Locked 0xBB 0xBC

		/// <summary>
		/// Le pide al Equipo el estado del Bloqueo de Parámetros
		/// </summary>
		/// <remarks></remarks>
		internal void ReadParametersLocked()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_PARAMETERSLOCKED;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Bloqueo de parámetros
		/// </summary>
		/// <remarks></remarks>
		public void WriteParametersLocked(OnOff value)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(value);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_PARAMETERSLOCKED;

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Counters

#region Minutos Conectado 0xC0 0xD0

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
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_PLUGTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_PLUGTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Conectado 0xC1 0xD1

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
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_PLUGTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_PLUGTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Trabajando 0xC2 0xD2

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
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_WORKTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_WORKTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Trabajando 0xC3 0xD3

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
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_WORKTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_WORKTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sleep 0xC4 0xD4

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto en sleep
		/// </summary>
		/// <remarks></remarks>
		public void ReadSleepTime(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sleep 0xC5 0xD5

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto en sleep
		/// </summary>
		/// <remarks></remarks>
		public void WriteSleepTime(Port Port, CounterTypes eType, int value)
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
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Hibernando 0xC6 0xD6

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		public void ReadHiberTime(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_HIBERTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_HIBERTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Hibernando 0xC7 0xD7

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		public void WriteHiberTime(Port Port, CounterTypes eType, int value)
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
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_HIBERTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_HIBERTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sin herramienta 0xC8 0xD8

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		public void ReadIdleTime(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_NOTOOLTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_NOTOOLTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sin herramienta 0xC9 0xD9

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		public void WriteIdleTime(Port Port, CounterTypes eType, int value)
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
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_NOTOOLTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_NOTOOLTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos Sleep 0xCA 0xDA

		/// <summary>
		/// Le pide al Equipo los ciclos de sleep del puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadSleepCycles(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos Sleep 0xCB 0xDB

		/// <summary>
		/// Guarda en el Equipo los ciclos de sleep del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteSleepCycles(Port Port, CounterTypes eType, int value)
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
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos desoldador 0xCC 0xDC

		/// <summary>
		/// Le pide al Equipo los ciclos de desoldador del puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadDesoldCycles(Port Port, CounterTypes eType)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_DESOLCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_DESOLCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos desoldador 0xCD 0xDD

		/// <summary>
		/// Guarda en el Equipo los ciclos de desoldador del puerto
		/// </summary>
		/// <remarks></remarks>
		public void WriteDesoldCycles(Port Port, CounterTypes eType, int value)
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
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_DESOLCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_DESOLCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Communications

#region USB/ETH Connection Mode 0xE0 0xE1 0xE9 0xEA

		/// <summary>
		/// Lee del equipo el estado de la conexión USB o ETH:
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
			if (m_ComChannel.Mode() == SearchMode.USB)
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_USB_CONNECTSTATUS;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_R_ETH_CONNECTSTATUS;
			}

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el equipo el estado de la conexión USB o ETH:
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
			if (m_ComChannel.Mode() == SearchMode.USB)
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_USB_CONNECTSTATUS;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_SOLD.M_W_ETH_CONNECTSTATUS;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Configuración ETH 0xE7 0xE8

		public void ReadEthernetConfiguration()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_ETH_TCPIPCONFIG;

			SendMessage(Datos, Command);
		}

		public void WriteEthernetConfiguration(CEthernetData ethernetData)
		{
			//Datos
			byte[] Datos = new byte[23];

			//Get data
			byte[] IP = ethernetData.IP.GetAddressBytes();
			byte[] Mask = ethernetData.Mask.GetAddressBytes();
			byte[] Gateway = ethernetData.Gateway.GetAddressBytes();
			byte[] DNS1 = ethernetData.DNS1.GetAddressBytes();
			byte[] DNS2 = ethernetData.DNS2.GetAddressBytes();

			//little endian
			Array.Reverse(IP);
			Array.Reverse(Mask);
			Array.Reverse(Gateway);
			Array.Reverse(DNS1);
			Array.Reverse(DNS2);

			Datos[0] = System.Convert.ToByte(ethernetData.DHCP);
			Array.Copy(IP, 0, Datos, 1, 4);
			Array.Copy(Mask, 0, Datos, 5, 4);
			Array.Copy(Gateway, 0, Datos, 9, 4);
			Array.Copy(DNS1, 0, Datos, 13, 4);
			Array.Copy(DNS2, 0, Datos, 17, 4);
			Array.Copy(BitConverter.GetBytes(ethernetData.Port), 0, Datos, 21, 2);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_ETH_TCPIPCONFIG;

			SendMessage(Datos, Command);
		}

#endregion


#region Configuración Robot 0xF0 0xF1

		public void ReadRobotConfiguration()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_RBT_CONNCONFIG;

			SendMessage(Datos, Command);
		}

		public void WriteRobotConfiguration(CRobotData robotData)
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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_RBT_CONNCONFIG;

			SendMessage(Datos, Command);
		}

#endregion


#region Estado Robot 0xF2 0xF3

		public void ReadRobotStatus()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_RBT_CONNECTSTATUS;

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
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_RBT_CONNECTSTATUS;

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Peripherals

#region Número de periféricos conectados 0xF9

		internal void ReadPeripheralCount()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_PERIPHCOUNT;

			SendMessage(Datos, Command);
		}

#endregion


#region Configuración periféricos 0xFA 0xFB

		public void ReadPeripheralConfiguration(int id)
		{
			//Datos
			byte[] Datos = BitConverter.GetBytes(id);
			Array.Resize(ref Datos, 1);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_PERIPHCONFIG;

			SendMessage(Datos, Command);
		}

		public void WritePeripheralConfiguration(int id, CPeripheralData peripheralData)
		{
			//Datos

			//Check data
			if (Encoding.UTF8.GetBytes(peripheralData.Hash_MCU_UID).Length < 4 || Encoding.UTF8.GetBytes(peripheralData.DateTime).Length < 14)
			{
				return;
			}

			byte[] Datos = new byte[31];

			Datos[0] = System.Convert.ToByte((peripheralData.Version / 10) + 48);
			Datos[1] = System.Convert.ToByte((peripheralData.Version % 10) + 48);
			Array.Copy(Encoding.UTF8.GetBytes(peripheralData.Hash_MCU_UID), 0, Datos, 2, 4);
			Array.Copy(Encoding.UTF8.GetBytes(peripheralData.DateTime), 0, Datos, 6, 14);

			if (peripheralData.Type == CPeripheralData.PeripheralType.FS)
			{
				Array.Copy(Encoding.UTF8.GetBytes("FS"), 0, Datos, 20, 2);
			}
			else if (peripheralData.Type == CPeripheralData.PeripheralType.MS)
			{
				Array.Copy(Encoding.UTF8.GetBytes("MS"), 0, Datos, 20, 2);
			}
			else if (peripheralData.Type == CPeripheralData.PeripheralType.MN)
			{
				Array.Copy(Encoding.UTF8.GetBytes("MN"), 0, Datos, 20, 2);
			}
			else if (peripheralData.Type == CPeripheralData.PeripheralType.MV)
			{
				Array.Copy(Encoding.UTF8.GetBytes("MV"), 0, Datos, 20, 2);
			}
			else
			{
				Array.Copy(Encoding.UTF8.GetBytes("PD"), 0, Datos, 20, 2);
			}

			if (peripheralData.PortAttached == Port.NUM_1)
			{
				Array.Copy(Encoding.UTF8.GetBytes("00"), 0, Datos, 22, 2);
			}
			else if (peripheralData.PortAttached == Port.NUM_2)
			{
				Array.Copy(Encoding.UTF8.GetBytes("01"), 0, Datos, 22, 2);
			}
			else if (peripheralData.PortAttached == Port.NUM_3)
			{
				Array.Copy(Encoding.UTF8.GetBytes("02"), 0, Datos, 22, 2);
			}
			else if (peripheralData.PortAttached == Port.NUM_4)
			{
				Array.Copy(Encoding.UTF8.GetBytes("03"), 0, Datos, 22, 2);
			}
			else
			{
				Array.Copy(Encoding.UTF8.GetBytes("05"), 0, Datos, 22, 2);
			}

			if (peripheralData.WorkFunction == CPeripheralData.PeripheralFunction.Extractor)
			{
				Array.Copy(Encoding.UTF8.GetBytes("EX"), 0, Datos, 24, 2);
			}
			else if (peripheralData.WorkFunction == CPeripheralData.PeripheralFunction.Modul)
			{
				Array.Copy(Encoding.UTF8.GetBytes("MO"), 0, Datos, 24, 2);
			}
			else
			{
				Array.Copy(Encoding.UTF8.GetBytes("SL"), 0, Datos, 24, 2);
			}

			if (peripheralData.ActivationMode == CPeripheralData.PeripheralActivation.Pulled)
			{
				Array.Copy(Encoding.UTF8.GetBytes("PL"), 0, Datos, 26, 2);
			}
			else
			{
				Array.Copy(Encoding.UTF8.GetBytes("PS"), 0, Datos, 26, 2);
			}

			Datos[28] = System.Convert.ToByte((peripheralData.DelayTime / 10) + 48);
			Datos[29] = System.Convert.ToByte((peripheralData.DelayTime % 10) + 48);
			Datos[30] = (byte)id;

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_W_PERIPHCONFIG;

			SendMessage(Datos, Command);
		}

#endregion


#region Estado periféricos 0xFC

		internal void ReadPeripheralStatus(int id)
		{
			//Datos
			byte[] Datos = BitConverter.GetBytes(id);
			Array.Resize(ref Datos, 1);

			//Command
			byte Command = (byte)EnumCommandFrame_02_SOLD.M_R_PERIPHSTATUS;

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

					//Para la DME TOUCH el continuous viene del dspic33(0x11)
				}
				else if (m_StationData.Info.Features.ContinuousModeDevice11h && Origen == (byte)EnumAddress.CONTINUOUS_MODE_DSPIC33_ADDRESS)
				{

					//Del dspic33 el comando recibido no es el esperado
					if (Command == (byte)EnumCommandFrameDSPIC33.M_I_CONTIMODE)
					{
						Command = (byte)EnumCommandFrame_02_SOLD.M_I_CONTIMODE;
					}
					else if (Command == (byte)EnumCommandFrameDSPIC33.M_R_CONTIMODE)
					{
						Command = (byte)EnumCommandFrame_02_SOLD.M_R_CONTIMODE;
					}
					else if (Command == (byte)EnumCommandFrameDSPIC33.M_W_CONTIMODE)
					{
						Command = (byte)EnumCommandFrame_02_SOLD.M_W_CONTIMODE;
					}

					//Del dspic33 la dirección no es la esperada
					Origen = m_StationNumDevice;

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
			else if (Command != (byte)EnumCommandFrame_02_SOLD.M_HS &
					Command != (byte)EnumCommandFrame_02_SOLD.M_I_CONTIMODE &
					Command != (byte)EnumCommandFrame_02_SOLD.M_R_CONTIMODE &
					Command != (byte)EnumCommandFrame_02_SOLD.M_W_CONTIMODE &
					Command != (byte)EnumCommandFrame_02_SOLD.M_FIRMWARE)
			{
				return;
			}


			//
			//Command
			//
			if (Command == (byte)EnumCommandFrame_02_SOLD.M_HS)
			{
				//Si se recibe una trama de handshake de otra dirección, es posible que se trate de otra estación
				if (Origen != m_StationNumDevice)
				{
					ReadDeviceVersions(Origen);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_EOT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_ACK)
			{
				if (EndedTransactionEvent != null)
					EndedTransactionEvent(Numstream);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_NACK)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_SYN)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_DEVICEID)
			{
				clsStationUID stationUUID = new clsStationUID(Datos);
				m_StationData.Info.UUID = stationUUID.UID;
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_DEVICEID)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_RESET)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_FIRMWARE)
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

					//El micro está conectado, actualizamos la marca de tiempo
					if (m_StationData.Info.StationMicros.Contains(Origen))
					{
						((CFirmwareStation)(m_StationData.Info.StationMicros[Origen])).SetTimeMarkConnected();
						((CFirmwareStation)(m_StationData.Info.StationMicros[Origen])).SoftwareVersion = versionSw; //actualizamos el software version ya que ha podido cambiar por una actualización

						//Guardar información del micro
					}
					else
					{
						CFirmwareStation micro = new CFirmwareStation();
						micro.StationUUID = m_StationData.Info.UUID;
						micro.Model = m_StationData.Info.Model;
						micro.HardwareVersion = versionHw;
						micro.SoftwareVersion = versionSw;
						micro.SetTimeMarkConnected();
						m_StationData.Info.StationMicros.Add(Origen, micro);
					}

				}

			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_CLEARMEMFLASH)
			{
				if (ClearingFlashFinishedEvent != null)
					ClearingFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_SENDMEMADDRESS)
			{
				if (AddressMemoryFlashFinishedEvent != null)
					AddressMemoryFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_SENDMEMDATA)
			{
				if (DataMemoryFlashFinishedEvent != null)
					DataMemoryFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_ENDPROGR)
			{
				if (EndProgFinishedEvent != null)
					EndProgFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_INF_PORT)
			{

				if (Datos.Length != 12 & Datos.Length != 14)
				{
					return;
				}

				// en Datos(12) viene el port
				m_PortData[Puerto].ToolStatus.ConnectedTool = GetGenericToolFromInternal(Datos[0]); // 07/01/2014 edu
				m_PortData[Puerto].ToolStatus.ToolError = (ToolError)(Datos[1]);
				m_PortData[Puerto].ToolStatus.ActualTemp[0].UTI = BitConverter.ToUInt16(Datos, 2);
				m_PortData[Puerto].ToolStatus.ActualTemp[1].UTI = BitConverter.ToUInt16(Datos, 4);
				//Debug.Print(String.Format("Tool {0} M_INF_PORT: Puerto {1} 2: {2:X}={3} 4: {4:X}={5}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Datos(2), BitConverter.ToUInt16(Datos, 2).ToString, Datos(4), BitConverter.ToUInt16(Datos, 4)))
				m_PortData[Puerto].ToolStatus.Current_mA[0] = BitConverter.ToUInt16(Datos, 6);
				m_PortData[Puerto].ToolStatus.Current_mA[1] = BitConverter.ToUInt16(Datos, 8);
				m_PortData[Puerto].ToolStatus.Power_x_Mil[0] = BitConverter.ToUInt16(Datos, 6);
				m_PortData[Puerto].ToolStatus.Power_x_Mil[1] = BitConverter.ToUInt16(Datos, 8);
				//Debug.Print(String.Format("Tool {0} M_INF_PORT: Puerto {1} ToUInt16(Datos, 6) {2} ToUInt16(Datos, 8): {3}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(0).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(1).ToString))
				//Debug.Print(String.Format("Tool {0} M_INF_PORT: Puerto {1} 6: {2:X} 7: {3:X} 8: {4:X} 9: {5:X}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Datos(6), Datos(7), Datos(8), Datos(9)))
				m_PortData[Puerto].ToolStatus.Stand_OnOff = (OnOff)(Datos[10] & 0x1);
				m_PortData[Puerto].ToolStatus.Sleep_OnOff = (OnOff)((Datos[10] & 0x2) >> 1);
				m_PortData[Puerto].ToolStatus.Hiber_OnOff = (OnOff)((Datos[10] & 0x4) >> 2);
				m_PortData[Puerto].ToolStatus.Extractor_OnOff = (OnOff)((Datos[10] & 0x8) >> 3);
				m_PortData[Puerto].ToolStatus.Desold_OnOff = (OnOff)((Datos[10] & 0x10) >> 4);
				//Debug.Print("M_INF_PORT - Puerto: {0} Estado: {1}", Puerto.ToString, bytesToBinary(Datos(10)))

				if (Datos.Length == 14)
				{

					m_StationData.Info.Features.ChangesStatusInformation = true;

					//Temperatura seleccionada modificado
					if ((Datos[11] & 0x1) > 0)
					{
						if (Changed_SelectedTemperatureEvent != null)
							Changed_SelectedTemperatureEvent();
					}

					//Parámetros estación modificados
					if ((Datos[11] & 0x2) > 0)
					{
						if (Changed_StationParametersEvent != null)
							Changed_StationParametersEvent();
					}

					//Parámetros herramienta puerto 0 modificados
					if ((Datos[11] & 0x4) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(0);
					}

					//Parámetros herramienta puerto 1 modificados
					if ((Datos[11] & 0x8) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(1);
					}

					//Parámetros herramienta puerto 2 modificados
					if ((Datos[11] & 0x10) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(2);
					}

					//Parámetros herramienta puerto 3 modificados
					if ((Datos[11] & 0x20) > 0)
					{
						if (Changed_ToolParamEvent != null)
							Changed_ToolParamEvent(3);
					}

					//Counters modificados
					if ((Datos[11] & 0x80) > 0)
					{
						if (Changed_CountersEvent != null)
							Changed_CountersEvent();
					}
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_LEVELSTEMPS)
			{
				if (Datos.Length != 13)
				{
					return;
				}

				// en Datos(11) y Datos(12) vienen el port y la tool
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsOnOff = (OnOff)(Datos[0]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempSelect = (ToolTemperatureLevels)(Datos[1]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempOnOff[0] = (OnOff)(Datos[2]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[0].UTI = BitConverter.ToUInt16(Datos, 3);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempOnOff[1] = (OnOff)(Datos[5]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[1].UTI = BitConverter.ToUInt16(Datos, 6);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempOnOff[2] = (OnOff)(Datos[8]);
				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[2].UTI = BitConverter.ToUInt16(Datos, 9);
				// Check temp levels. Stations do not check levels data until tool is connected
				//checkTempLevels(CType(Puerto, Port), Info_Port(Puerto).ToolParam.ToolSettings(Tool).Tool)
				//Debug.Print(String.Format("M_R_LEVELTEMP : {0} ", Info_Port(Puerto).ToolParam.ToolSettings(Tool).LevelsTempSelect.ToString))
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_LEVELSTEMPS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPDELAY)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				// en Datos(2) y Datos(3) vienen el port y la tool
				// en la enumeración NO_SLEEP está definido como FFFFh (viene del protocolo 01, lo controlamos por si acaso)
				if (Datos[0] == 0xFF)
				{
					m_PortData[Puerto].ToolSettings[Tool].SleepTimeOnOff = OnOff._OFF;
				}
				else
				{
					m_PortData[Puerto].ToolSettings[Tool].SleepTime = (ToolTimeSleep)((ToolTimeSleep)(Datos[0]));
					m_PortData[Puerto].ToolSettings[Tool].SleepTimeOnOff = (OnOff)(Datos[1]);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPDELAY)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPTEMP)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				// en Datos(2) y Datos(3) vienen el port y la tool
				m_PortData[Puerto].ToolSettings[Tool].SleepTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_HIBERDELAY)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				// en Datos(2) y Datos(3) vienen el port y la tool
				// en la enumeración NO_HIBERNATION está definido como FFFFh (viene del protocolo 01, lo controlamos por si acaso)
				if (Datos[0] == 0xFF)
				{
					m_PortData[Puerto].ToolSettings[Tool].HiberTimeOnOff = OnOff._OFF;
				}
				else
				{
					m_PortData[Puerto].ToolSettings[Tool].HiberTime = (ToolTimeHibernation)((ToolTimeHibernation)(Datos[0]));
					m_PortData[Puerto].ToolSettings[Tool].HiberTimeOnOff = (OnOff)(Datos[1]);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_HIBERDELAY)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_AJUSTTEMP)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				// en Datos(2) y Datos(3) vienen el port y la tool
				m_PortData[Puerto].ToolSettings[Tool].AdjustTemp.UTI = BitConverter.ToInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_AJUSTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_CARTRIDGE)
			{
				if (Datos.Length != 11)
				{
					return;
				}

				// en Datos(9) viene el port, en Datos(10) la Tool
				m_PortData[Puerto].ToolSettings[Tool].Cartridge.CartridgeOnOff = (OnOff)(Datos[0]);
				m_PortData[Puerto].ToolSettings[Tool].Cartridge.CartridgeNbr = BitConverter.ToUInt16(Datos, 1);
				m_PortData[Puerto].ToolSettings[Tool].Cartridge.CartridgeAdj300 = BitConverter.ToInt16(Datos, 3);
				m_PortData[Puerto].ToolSettings[Tool].Cartridge.CartridgeAdj400 = BitConverter.ToInt16(Datos, 5);
				m_PortData[Puerto].ToolSettings[Tool].Cartridge.CartridgeGroup = Datos[7];
				m_PortData[Puerto].ToolSettings[Tool].Cartridge.CartridgeFamily = Datos[8];
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_CARTRIDGE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_SELECTTEMP)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(3) viene el port
				m_PortData[Puerto].ToolStatus.SelectedTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_SELECTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_TIPTEMP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en Datos(4) viene el port
				m_PortData[Puerto].ToolStatus.ActualTemp[0].UTI = BitConverter.ToUInt16(Datos, 0);
				m_PortData[Puerto].ToolStatus.ActualTemp[1].UTI = BitConverter.ToUInt16(Datos, 2);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_CURRENT)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en Datos(4) viene el port
				m_PortData[Puerto].ToolStatus.Current_mA[0] = BitConverter.ToUInt16(Datos, 0);
				m_PortData[Puerto].ToolStatus.Current_mA[1] = BitConverter.ToUInt16(Datos, 2);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_POWER)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en Datos(4) viene el port
				m_PortData[Puerto].ToolStatus.Power_x_Mil[0] = BitConverter.ToUInt16(Datos, 0);
				m_PortData[Puerto].ToolStatus.Power_x_Mil[1] = BitConverter.ToUInt16(Datos, 2);
				//Debug.Print(String.Format("Tool {0} M_R_POWER: Puerto {1} ToUInt16(Datos, 0) {2} ToUInt16(Datos, 2): {3}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(0).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(1).ToString))
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_CONNECTTOOL)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el port
				m_PortData[Puerto].ToolStatus.ConnectedTool = GetGenericToolFromInternal(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_TOOLERROR)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el port
				m_PortData[Puerto].ToolStatus.ToolError = (ToolError)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_STATUSTOOL)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				// en Datos(1) viene el puerto
				m_PortData[Puerto].ToolStatus.Stand_OnOff = (OnOff)(Datos[0] & 0x1);
				m_PortData[Puerto].ToolStatus.Sleep_OnOff = (OnOff)((Datos[0] & 0x2) >> 1);
				m_PortData[Puerto].ToolStatus.Hiber_OnOff = (OnOff)((Datos[0] & 0x4) >> 2);
				m_PortData[Puerto].ToolStatus.Extractor_OnOff = (OnOff)((Datos[0] & 0x8) >> 3);
				m_PortData[Puerto].ToolStatus.Desold_OnOff = (OnOff)((Datos[0] & 0x10) >> 4);
				//Debug.Print("M_R_STATUSTOOL - Puerto: {0} Estado: {1}", Puerto.ToString, bytesToBinary(Datos(0)))
				// StatusRemote
				m_PortData[Puerto].ToolStatus.StatusRemoteMode.Sleep_OnOff = m_PortData[Puerto].ToolStatus.Stand_OnOff | m_PortData[Puerto].ToolStatus.Sleep_OnOff | m_PortData[Puerto].ToolStatus.Hiber_OnOff;
				m_PortData[Puerto].ToolStatus.StatusRemoteMode.Extractor_OnOff = m_PortData[Puerto].ToolStatus.Extractor_OnOff;
				m_PortData[Puerto].ToolStatus.StatusRemoteMode.Desold_OnOff = m_PortData[Puerto].ToolStatus.Desold_OnOff;
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_MOSTEMP)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				// en Datos(2) viene el puerto
				m_PortData[Puerto].ToolStatus.Temp_MOS.UTI = System.Convert.ToInt32(BitConverter.ToUInt16(Datos, 0));
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_DELAYTIME)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				// en Datos(3) viene el puerto
				m_PortData[Puerto].ToolStatus.TimeToSleepHibern = System.Convert.ToInt32(BitConverter.ToUInt16(Datos, 0));
				// puede venir FFFF
				if (m_PortData[Puerto].ToolStatus.TimeToSleepHibern == 0xFFFF)
				{
					m_PortData[Puerto].ToolStatus.TimeToSleepHibern = 0;
				}
				m_PortData[Puerto].ToolStatus.FutureMode_Tool = (ToolFutureMode)(Datos[2]);
				//Debug.Print("M_R_DELAYTIME - Puerto: {0} TimeToSleepHibern: {1} FutureMode: {2}", Puerto.ToString, Info_Port(Puerto).ToolStatus.TimeToSleepHibern.ToString, CType(Info_Port(Puerto).ToolStatus.FutureMode, FutureMode).ToString)
				// si el estado es work, poner time to sleep/hiber a cero
				//(tienen que arreglarlo los de MODPOW, porque me sigue enviando delay establecido en Sleep, hasta que cambie de estado a sleep o hiber)
				if (m_PortData[Puerto].ToolStatus.Stand_OnOff == OnOff._ON ||
						m_PortData[Puerto].ToolStatus.Sleep_OnOff == OnOff._ON ||
						m_PortData[Puerto].ToolStatus.Hiber_OnOff == OnOff._ON ||
						m_PortData[Puerto].ToolStatus.Desold_OnOff == OnOff._ON ||
						m_PortData[Puerto].ToolStatus.Extractor_OnOff == OnOff._OFF)
				{
					m_PortData[Puerto].ToolStatus.TimeToSleepHibern = 0;
				}

				if (m_PortData[Puerto].ToolStatus.TimeToSleepHibern == 0)
				{
					m_PortData[Puerto].ToolStatus.FutureMode_Tool = ToolFutureMode.NoFutureMode;
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_REMOTEMODE)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Status.RemoteMode = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_REMOTEMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_CONTIMODE)
			{
				if (Datos.Length != 2 & Datos.Length != 3)
				{
					return;
				}

				if (Origen == m_StationNumDevice)
				{
					//m_StationData.ContinuousModeSpeed = CType(Datos(0), SpeedContinuousMode)
					//m_StationData.ContinuousModePorts = Datos(1)
					m_StationData.Status.ContinuousModeStatus.speed = (SpeedContinuousMode) (Datos[0]);
					m_StationData.Status.ContinuousModeStatus.setPortsFromByte(Datos[1]);

				}
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_CONTIMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_I_CONTIMODE)
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
					int iPortDataBlock = 0;
					// 25/05/2015 en protocolo 2 de las DDE y NAE: viene un byte adicional entre las potencias y el estado
					if (m_StationData.Info.Features.ContinuosuModeDataExtraByte)
					{
						iPortDataBlock = 11;
					}
					else
					{
						iPortDataBlock = 10;
					}


					//checking the recieved data length with the number of ports
					if (Datos.Length >= 1 + 1 * iPortDataBlock)
					{
						//getting the ports data
						stContinuousModeData_SOLD data = new stContinuousModeData_SOLD();
						data.data = new stContinuousModePort_SOLD[nPorts - 1 + 1];
						int offset = 0;
						int val1 = 0;
						int val2 = 0;
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
								val2 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 3]) << 8) | System.Convert.ToInt32(Datos[offset + 2]));

								if (m_PortData[(int)data.data[i].port].ToolStatus.ConnectedTool == GenericStationTools.HT |
										m_PortData[(int)data.data[i].port].ToolStatus.ConnectedTool == GenericStationTools.PA)
								{
									data.data[i].temperature = new CTemperature(System.Convert.ToInt32((val1 + val2) / 2));
								}
								else
								{
									data.data[i].temperature = new CTemperature(val1);
								}


								//
								// power
								//
								offset = 1 + i * iPortDataBlock + 4;

								val1 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 1]) << 8) | System.Convert.ToInt32(Datos[offset]));
								val2 = System.Convert.ToInt32((System.Convert.ToInt32(Datos[offset + 3]) << 8) | System.Convert.ToInt32(Datos[offset + 2]));

								if (m_PortData[(int)data.data[i].port].ToolStatus.ConnectedTool == GenericStationTools.HT |
										m_PortData[(int)data.data[i].port].ToolStatus.ConnectedTool == GenericStationTools.PA)
								{
									data.data[i].power = System.Convert.ToInt32((val1 + val2) / 2);
								}
								else
								{
									data.data[i].power = val1;
								}


								//
								// status
								//
								offset = 1 + i * iPortDataBlock + 4 + 4;
								//Public Enum ToolStatus
								// antes Protocol_01
								//DESOLDER = 8
								//EXTRACTOR = 4
								//HIBERNATION = 2
								//SLEEP = 1
								//NONE = 0
								// ahora Protocol_02
								//DESOLDER = 16
								//EXTRACTOR = 8
								//HIBERNATION = 4
								//SLEEP = 2
								//STAND = 1
								//NONE = 0

								// 25/05/2015 en protocolo 2 de las DDE/NAE: viene un byte adicional entre las potencias y el estado
								// 02/09/2015 ademÃ¡s viene mal el estado de la herramienta (viene un 4 y luego un tres cuando se quita del stand)
								// La información en vez de obtenerse del modo continuo se obtiene del tool status
								if (m_StationData.Info.Features.ContinuosuModeDataExtraByte)
								{
									offset++;
									if (m_PortData[(int)data.data[i].port].ToolStatus.Stand_OnOff == OnOff._ON)
									{
										data.data[i].status = ToolStatus.STAND;
									}
									else if (m_PortData[(int)data.data[i].port].ToolStatus.Sleep_OnOff == OnOff._ON)
									{
										data.data[i].status = ToolStatus.SLEEP;
									}
									else if (m_PortData[(int)data.data[i].port].ToolStatus.Hiber_OnOff == OnOff._ON)
									{
										data.data[i].status = ToolStatus.HIBERNATION;
									}
									else if (m_PortData[(int)data.data[i].port].ToolStatus.Extractor_OnOff == OnOff._ON)
									{
										data.data[i].status = ToolStatus.HIBERNATION;
									}
									else
									{
										data.data[i].status = ToolStatus.NONE;
									}

								}
								else
								{
									// bits en protocolo 02:  Desoldador:4 Extractor:3 HibernaciÃ³n:2 Sleep:1 Stand:0
									if ((Datos[offset] & 0x1) == (byte)ToolStatus.STAND)
									{
										//Console.WriteLine("      STAND")
										data.data[i].status = ToolStatus.STAND;
									}
									else if ((Datos[offset] & 0x2) == (byte)ToolStatus.SLEEP)
									{
										//Console.WriteLine("      SLEEP")
										data.data[i].status = ToolStatus.SLEEP;
									}
									else if ((Datos[offset] & 0x4) == (byte)ToolStatus.HIBERNATION)
									{
										//Console.WriteLine("      HIBERNATION")
										data.data[i].status = ToolStatus.HIBERNATION;
									}
									else if ((Datos[offset] & 0x8) == (byte)ToolStatus.EXTRACTOR)
									{
										//Console.WriteLine("      EXTRACTOR")
										data.data[i].status = ToolStatus.EXTRACTOR;
									}
									else
									{
										//Console.WriteLine("      NONE")
										data.data[i].status = ToolStatus.NONE;
									}
								}

								if ((Datos[offset] & 0x10) > 0)
								{
									data.data[i].desoldering = OnOff._ON;
								}
								else
								{
									data.data[i].desoldering = OnOff._OFF;
								}

								//Debug.Print("CONT MODE STATUS prot 02: {0}", data.data(i).status.ToString)
							}
						}

						//adding the continuous data to the list (OLD - only one buffer)
						//data.sequence = userSequence
						//dataList.Add(data)
						//If dataList.Count > JBC_API.CONTINUOUS_MODE_QUEUE_MAX_LENGTH Then dataList.RemoveAt(0)
						//'getting the sequence number
						//Dim curSequence As Integer = CInt(Datos(0))
						//'updating the user sequence
						//If curSequence > lastSequence Then userSequence = userSequence + CType(curSequence - lastSequence, ULong)
						//If curSequence < lastSequence Then userSequence = userSequence + CType(curSequence - lastSequence + 256, ULong)
						//lastSequence = curSequence

						// adding the continuous data to all current buffers (NEW - multi-buffer)
						// passing frame sequence in data.sequence (user sequence is calculated inside traceList class)
						data.sequence = Datos[0];
						m_traceList.AddData(data);
					}
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_TEMPUNIT)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.Unit = (CTemperature.TemperatureUnit)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_TEMPUNIT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_MAXTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.MaxTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_MAXTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_MINTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.MinTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_MINTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_NITROMODE)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.N2Mode = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_NITROMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_HELPTEXT)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.HelpText = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_HELPTEXT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_POWERLIM)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.PowerLimit = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_POWERLIM)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_BEEP)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.Beep = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_BEEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_PARAMETERSLOCKED)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.ParametersLocked = (OnOff) (Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_PARAMETERSLOCKED)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_LANGUAGE)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.Language = GetLanguageFromLangText(Encoding.UTF8.GetString(Datos));
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_LANGUAGE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_PIN)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_StationData.Settings.PIN = Encoding.UTF8.GetString(Datos);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_PIN)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_STATERROR)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Status.ErrorStation = (StationError)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_TRAFOTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Status.TempTRAFO.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_RESETSTATION)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_DEVICENAME)
			{
				string TextoName = Encoding.UTF8.GetString(Datos);
				m_StationData.Settings.Name = TextoName;
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_DEVICENAME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_PLUGTIME)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContPlugMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_PLUGTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_WORKTIME)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContWorkMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_WORKTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPTIME)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContSleepMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_HIBERTIME)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContHiberMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_HIBERTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_NOTOOLTIME)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContIdleMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_NOTOOLTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPCYCLES)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContSleepCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPCYCLES)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_DESOLCYCLES)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].Counters.ContDesoldCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_DESOLCYCLES)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_PLUGTIMEP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContPlugMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_PLUGTIMEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_WORKTIMEP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContWorkMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_WORKTIMEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPTIMEP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContSleepMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPTIMEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_HIBERTIMEP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContHiberMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_HIBERTIMEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_NOTOOLTIMEP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContIdleMinutes = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_NOTOOLTIMEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_SLEEPCYCLESP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContSleepCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_SLEEPCYCLESP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_DESOLCYCLESP)
			{
				if (Datos.Length != 5)
				{
					return;
				}

				// en byte(4) viene el puerto
				m_PortData[Puerto].PartialCounters.ContDesoldCycles = BitConverter.ToInt32(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_DESOLCYCLESP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_USB_CONNECTSTATUS)
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
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_USB_CONNECTSTATUS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_ETH_TCPIPCONFIG)
			{
				if (Datos.Length != 23)
				{
					return;
				}

				byte[] byteIP = new byte[4];
				sDatos = Encoding.UTF8.GetString(Datos);

				m_StationData.Settings.Ethernet.DHCP = (OnOff)(Datos[0]);

				//little endian
				byteIP[0] = Datos[4];
				byteIP[1] = Datos[3];
				byteIP[2] = Datos[2];
				byteIP[3] = Datos[1];
				m_StationData.Settings.Ethernet.IP = new IPAddress(byteIP);

				byteIP[0] = Datos[8];
				byteIP[1] = Datos[7];
				byteIP[2] = Datos[6];
				byteIP[3] = Datos[5];
				m_StationData.Settings.Ethernet.Mask = new IPAddress(byteIP);

				byteIP[0] = Datos[12];
				byteIP[1] = Datos[11];
				byteIP[2] = Datos[10];
				byteIP[3] = Datos[9];
				m_StationData.Settings.Ethernet.Gateway = new IPAddress(byteIP);

				byteIP[0] = Datos[16];
				byteIP[1] = Datos[15];
				byteIP[2] = Datos[14];
				byteIP[3] = Datos[13];
				m_StationData.Settings.Ethernet.DNS1 = new IPAddress(byteIP);

				byteIP[0] = Datos[20];
				byteIP[1] = Datos[19];
				byteIP[2] = Datos[18];
				byteIP[3] = Datos[17];
				m_StationData.Settings.Ethernet.DNS2 = new IPAddress(byteIP);

				m_StationData.Settings.Ethernet.Port = BitConverter.ToUInt16(Datos, 21);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_ETH_TCPIPCONFIG)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_ETH_CONNECTSTATUS)
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
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_ETH_CONNECTSTATUS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_RBT_CONNCONFIG)
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
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_RBT_CONNCONFIG)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_RBT_CONNECTSTATUS)
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
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_W_RBT_CONNECTSTATUS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_PERIPHCOUNT)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				int count = Datos[0];

				//Crear elementos
				while (m_StationData.Peripherals.Count < count)
				{
					m_StationData.Peripherals.Add(new CPeripheralData(m_StationData.Peripherals.Count));
				}

				//Borrar elementos
				m_StationData.Peripherals.RemoveRange(count, m_StationData.Peripherals.Count - count);
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_PERIPHCONFIG)
			{
				if (Datos.Length != 31)
				{
					return;
				}

				int index = Datos[30];

				//Comprobar si el elemento esta indexado
				if (index >= m_StationData.Peripherals.Count)
				{
					return;
				}

				sDatos = Encoding.UTF8.GetString(Datos);

				short nVersion = 0;
				short.TryParse(sDatos.Substring(0, 2), out nVersion);
				m_StationData.Peripherals[index].Version = nVersion;

				m_StationData.Peripherals[index].Hash_MCU_UID = sDatos.Substring(2, 4);
				m_StationData.Peripherals[index].DateTime = sDatos.Substring(6, 14);

				switch (sDatos.Substring(20, 2))
				{
					case "PD":
						m_StationData.Peripherals[index].Type = CPeripheralData.PeripheralType.PD;
						break;
					case "MS":
						m_StationData.Peripherals[index].Type = CPeripheralData.PeripheralType.MS;
						break;
					case "MN":
						m_StationData.Peripherals[index].Type = CPeripheralData.PeripheralType.MN;
						break;
					case "FS":
						m_StationData.Peripherals[index].Type = CPeripheralData.PeripheralType.FS;
						break;
					case "MV":
						m_StationData.Peripherals[index].Type = CPeripheralData.PeripheralType.MV;
						break;
					default:
						m_StationData.Peripherals[index].Type = CPeripheralData.PeripheralType.NO_TYPE;
						break;
				}

				switch (sDatos.Substring(22, 2))
				{
					case "00":
						m_StationData.Peripherals[index].PortAttached = Port.NUM_1;
						break;
					case "01":
						m_StationData.Peripherals[index].PortAttached = Port.NUM_2;
						break;
					case "02":
						m_StationData.Peripherals[index].PortAttached = Port.NUM_3;
						break;
					case "03":
						m_StationData.Peripherals[index].PortAttached = Port.NUM_4;
						break;
					default:
						m_StationData.Peripherals[index].PortAttached = Port.NO_PORT;
						break;
				}

				switch (sDatos.Substring(24, 2))
				{
					case "SL":
						m_StationData.Peripherals[index].WorkFunction = CPeripheralData.PeripheralFunction.Sleep;
						break;
					case "EX":
						m_StationData.Peripherals[index].WorkFunction = CPeripheralData.PeripheralFunction.Extractor;
						break;
					case "MO":
						m_StationData.Peripherals[index].WorkFunction = CPeripheralData.PeripheralFunction.Modul;
						break;
					default:
						m_StationData.Peripherals[index].WorkFunction = CPeripheralData.PeripheralFunction.NO_FUNCTION;
						break;
				}

				switch (sDatos.Substring(26, 2))
				{
					case "PS":
						m_StationData.Peripherals[index].ActivationMode = CPeripheralData.PeripheralActivation.Pressed;
						break;
					case "PL":
						m_StationData.Peripherals[index].ActivationMode = CPeripheralData.PeripheralActivation.Pulled;
						break;
					default:
						m_StationData.Peripherals[index].ActivationMode = CPeripheralData.PeripheralActivation.NO_FUNCTION;
						break;
				}

				short nDelayTime = 0;
				short.TryParse(sDatos.Substring(28, 2), out nDelayTime);
				m_StationData.Peripherals[index].DelayTime = nDelayTime;
			}
			else if (Command == (byte)EnumCommandFrame_02_SOLD.M_R_PERIPHSTATUS)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				int index = Datos[2];

				//Comprobar si el elemento esta indexado
				if (index >= m_StationData.Peripherals.Count)
				{
					return;
				}

				m_StationData.Peripherals[index].StatusActive = (OnOff)(Datos[0]);

				if (Convert.ToChar(Datos[1]) == 'C')
				{
					m_StationData.Peripherals[index].StatusPD = CPeripheralData.PeripheralStatusPD.CC;
				}
				else if (Convert.ToChar(Datos[1]) == 'O')
				{
					m_StationData.Peripherals[index].StatusPD = CPeripheralData.PeripheralStatusPD.OC;
				}
				else if (Convert.ToChar(Datos[1]) == 'K')
				{
					m_StationData.Peripherals[index].StatusPD = CPeripheralData.PeripheralStatusPD.OK;
				}
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
		private bool CheckTempLevelsSetting(CTempLevelsData_SOLD levels)
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
			switch (m_StationData.Info.Model)
			{
				case "NA":
					// 01/11/2014 Se cambia NT205 por NT105
					if (Dato == (byte)Tools_NanoStation.NOTOOL)
					{
						return GenericStationTools.NO_TOOL;
					}
					else if (Dato == (byte)Tools_NanoStation.NT105)
					{
						return GenericStationTools.NT105;
					}
					else if (Dato == (byte)Tools_NanoStation.NP105)
					{
						return GenericStationTools.NP105;
					}
					else
					{
						return ((GenericStationTools)Dato);
					}
					break;
			}
			return ((GenericStationTools)Dato);
		}

		private byte GetInternalToolFromGeneric(GenericStationTools Tool)
		{
			return System.Convert.ToByte(Tool);
		}

		private ToolError GetToolErrorFromInternal(byte myerror)
		{
			return ((ToolError)myerror);
		}

#endregion

	}
}
