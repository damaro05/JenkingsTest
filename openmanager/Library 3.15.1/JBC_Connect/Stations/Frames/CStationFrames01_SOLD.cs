// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using Microsoft.VisualBasic;
using DataJBC;
using RoutinesJBC;
using Constants = DataJBC.Constants;


namespace JBC_Connect
{

	internal class CStationFrames01_SOLD : CStationFrames
	{

		protected CStationData_SOLD m_StationData; //Información de la estación
		protected CPortData_SOLD[] m_PortData; //Información del puerto
		protected CContinuousModeQueueListStation_SOLD m_traceList; //Información del modo continuo

		public delegate void Changed_SelectedTemperatureEventHandler();
		private Changed_SelectedTemperatureEventHandler Changed_SelectedTemperatureEvent;

		public event Changed_SelectedTemperatureEventHandler Changed_SelectedTemperature
		{
			add
			{
				Changed_SelectedTemperatureEvent = (Changed_SelectedTemperatureEventHandler) System.Delegate.Combine(Changed_SelectedTemperatureEvent, value);
			}
			remove
			{
				Changed_SelectedTemperatureEvent = (Changed_SelectedTemperatureEventHandler) System.Delegate.Remove(Changed_SelectedTemperatureEvent, value);
			}
		}

		public delegate void Changed_StationParametersEventHandler();
		private Changed_StationParametersEventHandler Changed_StationParametersEvent;

		public event Changed_StationParametersEventHandler Changed_StationParameters
		{
			add
			{
				Changed_StationParametersEvent = (Changed_StationParametersEventHandler) System.Delegate.Combine(Changed_StationParametersEvent, value);
			}
			remove
			{
				Changed_StationParametersEvent = (Changed_StationParametersEventHandler) System.Delegate.Remove(Changed_StationParametersEvent, value);
			}
		}

		public delegate void Changed_ToolParamEventHandler(int port);
		private Changed_ToolParamEventHandler Changed_ToolParamEvent;

		public event Changed_ToolParamEventHandler Changed_ToolParam
		{
			add
			{
				Changed_ToolParamEvent = (Changed_ToolParamEventHandler) System.Delegate.Combine(Changed_ToolParamEvent, value);
			}
			remove
			{
				Changed_ToolParamEvent = (Changed_ToolParamEventHandler) System.Delegate.Remove(Changed_ToolParamEvent, value);
			}
		}

		public delegate void Changed_CountersEventHandler();
		private Changed_CountersEventHandler Changed_CountersEvent;

		public event Changed_CountersEventHandler Changed_Counters
		{
			add
			{
				Changed_CountersEvent = (Changed_CountersEventHandler) System.Delegate.Combine(Changed_CountersEvent, value);
			}
			remove
			{
				Changed_CountersEvent = (Changed_CountersEventHandler) System.Delegate.Remove(Changed_CountersEvent, value);
			}
		}

		public delegate void EndedTransactionEventHandler(uint transactionID);
		private EndedTransactionEventHandler EndedTransactionEvent;

		public event EndedTransactionEventHandler EndedTransaction
		{
			add
			{
				EndedTransactionEvent = (EndedTransactionEventHandler) System.Delegate.Combine(EndedTransactionEvent, value);
			}
			remove
			{
				EndedTransactionEvent = (EndedTransactionEventHandler) System.Delegate.Remove(EndedTransactionEvent, value);
			}
		}



		public CStationFrames01_SOLD(CStationData_SOLD _StationData, CPortData_SOLD[] _PortData, CContinuousModeQueueListStation_SOLD _traceList, CCommunicationChannel _ComChannel, byte _StationNumDevice)
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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_INF_PORT;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura Fija 0x31 0x32

		/// <summary>
		/// Le pide al Equipo la temperatura fijada por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void ReadFixTemp(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_FIXTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura fijada por el equipo en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteFixTemp(Port Port, GenericStationTools Tool, CTemperature temperature)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_FIXTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Nivel Temperatura Seleccionado 0x33 0x34

		/// <summary>
		/// Le pide al Equipo el nivel de temperatura seleccionado
		/// </summary>
		/// <remarks></remarks>
		public void ReadSelectLevelTemp(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_LEVELTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el nivel de temperatura seleccionado
		/// </summary>
		/// <remarks></remarks>
		public void WriteSelectLevelTemp(Port Port, GenericStationTools Tool, ToolTemperatureLevels level)
		{
			// PORT(1B) + TOOL(1B) + Value(1B)

			//Datos
			byte[] Datos = new byte[3];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			Datos[2] = (byte)level;

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_LEVELTEMP;

			SendMessage(Datos, Command);
		}

#endregion


#region Nivel Temperatura 1 0x35 0x36

		/// <summary>
		/// Le pide al Equipo el nivel de temperatura 1
		/// </summary>
		/// <remarks></remarks>
		public void ReadLevelTemp1(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_LEVEL1;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el nivel de temperatura 1
		/// </summary>
		/// <remarks></remarks>
		public void WriteLevelTemp1(Port Port, GenericStationTools Tool, CTemperature temperature)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_LEVEL1;

			SendMessage(Datos, Command);
		}

#endregion


#region Nivel Temperatura 2 0x37 0x38

		/// <summary>
		/// Le pide al Equipo el nivel de temperatura 2
		/// </summary>
		/// <remarks></remarks>
		public void ReadLevelTemp2(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_LEVEL2;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el nivel de temperatura 2
		/// </summary>
		/// <remarks></remarks>
		public void WriteLevelTemp2(Port Port, GenericStationTools Tool, CTemperature temperature)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_LEVEL2;

			SendMessage(Datos, Command);
		}

#endregion


#region Nivel Temperatura 3 0x39 0x3A

		/// <summary>
		/// Le pide al Equipo el nivel de temperatura 3
		/// </summary>
		/// <remarks></remarks>
		public void ReadLevelTemp3(Port Port, GenericStationTools Tool)
		{
			// PORT(1B) + TOOL(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_LEVEL3;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo el nivel de temperatura 3
		/// </summary>
		/// <remarks></remarks>
		public void WriteLevelTemp3(Port Port, GenericStationTools Tool, CTemperature temperature)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_LEVEL3;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPDELAY;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo retardo en la entrada del sleep
		/// </summary>
		/// <remarks></remarks>
		public void WriteSleepDelay(Port Port, GenericStationTools Tool, ToolTimeSleep value)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes((ushort)value);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPDELAY;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura de Sleep en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteSleepTemp(Port Port, GenericStationTools Tool, CTemperature temperature)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_HIBERDELAY;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo retardo en la entrada de la hibernación
		/// </summary>
		/// <remarks></remarks>
		public void WriteHiberDelay(Port Port, GenericStationTools Tool, ToolTimeHibernation value)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes((ushort)value);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_HIBERDELAY;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_AJUSTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura de Ajuste en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteAjustTemp(Port Port, GenericStationTools Tool, CTemperature temperature)
		{
			// PORT(1B) + TOOL(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[4];
			Datos[0] = (byte)Port;
			Datos[1] = GetInternalToolFromGeneric(Tool);
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[2] = ValueB[0];
			Datos[3] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_AJUSTTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_SELECTTEMP;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el Equipo la temperatura Seleccionada en UTI
		/// </summary>
		/// <remarks></remarks>
		public void WriteSelectTemp(Port Port, CTemperature temperature)
		{
			// PORT(1B) + Value(2B)

			//Datos
			byte[] Datos = new byte[3];
			Datos[0] = (byte)Port;
			byte[] ValueB = BitConverter.GetBytes(temperature.UTI);
			Datos[1] = ValueB[0];
			Datos[2] = ValueB[1];

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_SELECTTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_TIPTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_CURRENT;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_POWER;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_CONNECTTOOL;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_TOOLERROR;

			SendMessage(Datos, Command);
		}

#endregion


#region Estado Herramienta 0x57

		/// <summary>
		/// Le pide al Equipo el estado de la herramienta conectada
		/// </summary>
		/// <remarks></remarks>
		internal void ReadStatusTool(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_STATUSTOOL;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_MOSTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_DELAYTIME;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_REMOTEMODE;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_REMOTEMODE;

			SendMessage(Datos, Command);
		}

#endregion


#region Estado Modo Remoto 0x62 0x63

		/// <summary>
		/// Le pide al Equipo que devuelva el valor del estado del modo remoto de un puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadStatusRemoteMode(Port Port)
		{
			// PORT(1B)

			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_STATUSREMOTEMODE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Le pide al Equipo que configure su estado de modo remoto
		/// puerto, Sleep, extractor, desoldador
		/// </summary>
		/// <remarks></remarks>
		public void WriteStatusRemoteMode(Port Port, OnOff Sleep, OnOff Extractor, OnOff Desoldador)
		{
			// PORT(1B) + STATUS(1B)

			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			byte Status = (byte)0;

			if (Extractor == OnOff._ON)
			{
				Status = (byte)(Status + 0x4);
			}
			else if (Sleep == OnOff._ON)
			{
				Status = (byte)(Status + 0x1);
			}

			if (Desoldador == OnOff._ON)
			{
				Status = (byte)(Status + 0x8);
			}

			Datos[1] = Status;

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_STATUSREMOTEMODE;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_CONTIMODE;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_CONTIMODE;

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Control mode

#region Connection Mode 0x1E 0x1F

		/// <summary>
		/// Lee del equipo el estado de la conexión:
		///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
		///   * Control Mode: en este estado la estación sólo es configurable desde el PC
		/// </summary>
		/// <remarks></remarks>
		public void ReadConnectStatus()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_CONNECTSTATUS;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda en el equipo el estado de la conexión:
		///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
		///   * Control Mode: en este estado la estación sólo es configurable desde el PC
		/// </summary>
		/// <remarks></remarks>
		public void WriteConnectStatus(ControlModeConnection mode)
		{
			//Datos
			byte[] Datos = new byte[1];

			if (mode == ControlModeConnection.CONTROL)
			{
				Datos[0] = 0x43; //"C"
			}
			else
			{
				Datos[0] = 0x4D; //"M"
			}

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_CONNECTSTATUS;

			SendMessage(Datos, Command);
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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_TEMPUNIT;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_TEMPUNIT;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_MAXTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_MAXTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_MINTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_MINTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_NITROMODE;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_NITROMODE;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_HELPTEXT;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_HELPTEXT;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_POWERLIM;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_POWERLIM;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_PIN;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_PIN;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_STATERROR;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_TRAFOTEMP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_RESETSTATION;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_DEVICENAME;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_DEVICENAME;

			SendMessage(Datos, Command);
		}

#endregion


#region ID equipo 0xB9 0xBA

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_DEVICEID;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Permite configurar un UID para el equipo conectado
		/// </summary>
		/// <param name="Value">Tamaño máximo 20 bytes entre 32 y 127</param>
		/// <remarks></remarks>
		public void WriteDeviceUID(byte[] Value)
		{
			//Datos
			int iLen = Value.Length;
			if (iLen > 20)
			{
				iLen = 20;
			}
			byte[] Datos = null;
			Datos = new byte[iLen - 1 + 1];
			Array.Copy(Value, Datos, iLen);

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_DEVICEID;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_BEEP;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_BEEP;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura error trafo 0xB7

		/// <summary>
		/// Le pide al Equipo la temperatura de entrada en error de transformador en UTI
		/// </summary>
		/// <remarks></remarks>
		internal void ReadTempErrorMOS()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_TEMPERRORTRAFO;

			SendMessage(Datos, Command);
		}

#endregion


#region Temperatura error MOS 0xB8

		/// <summary>
		/// Le pide al Equipo la temperatura de entrada en error de los transistores MOS en UTI
		/// </summary>
		/// <remarks></remarks>
		internal void ReadTempErrorTrafo()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_TEMPERRORMOS;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_PARAMETERSLOCKED;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_PARAMETERSLOCKED;

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
		public void ReadPlugTime(CounterTypes eType)
		{

			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_PLUGTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_PLUGTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Conectado 0xC1

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del equipo conectado
		/// </summary>
		/// <remarks></remarks>
		internal void WritePlugTime(int[] value)
		{
			// portcount * VALUE(4B)
			//Datos
			byte[] Datos = null;
			Datos = new byte[(m_PortData.Length * 4) - 1 + 1];
			for (var i = 0; i <= m_PortData.Length - 1; i++)
			{
				byte[] ValueB = BitConverter.GetBytes(value[i]);
				Array.Copy(ValueB, 0, Datos, System.Convert.ToInt32(4 * i), 4);
			}
			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_PLUGTIME;

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Trabajando 0xC2 0xD2

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto trabajando
		/// </summary>
		/// <remarks></remarks>
		public void ReadWorkTime(CounterTypes eType)
		{

			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_WORKTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_WORKTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Trabajando 0xC3

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto trabajando
		/// </summary>
		/// <remarks></remarks>
		internal void WriteWorkTime(int[] values)
		{
			// portcount * VALUE(4B)
			//Datos
			byte[] Datos = null;
			Datos = new byte[(m_PortData.Length * 4) - 1 + 1];
			for (var i = 0; i <= m_PortData.Length - 1; i++)
			{
				byte[] ValueB = BitConverter.GetBytes(values[i]);
				Array.Copy(ValueB, 0, Datos, System.Convert.ToInt32(4 * i), 4);
			}

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_WORKTIME;

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sleep 0xC4 0xD4

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto en sleep
		/// </summary>
		/// <remarks></remarks>
		public void ReadSleepTime(CounterTypes eType)
		{

			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sleep 0xC5

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto en sleep
		/// </summary>
		/// <remarks></remarks>
		internal void WriteSleepTime(int[] values)
		{
			// portcount * VALUE(4B)
			//Datos
			byte[] Datos = null;
			Datos = new byte[(m_PortData.Length * 4) - 1 + 1];
			for (var i = 0; i <= m_PortData.Length - 1; i++)
			{
				byte[] ValueB = BitConverter.GetBytes(values[i]);
				Array.Copy(ValueB, 0, Datos, System.Convert.ToInt32(4 * i), 4);
			}

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPTIME;

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Hibernando 0xC6 0xD6

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		public void ReadHiberTime(CounterTypes eType)
		{

			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_HIBERTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_HIBERTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Hibernando 0xC7

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		internal void WriteHiberTime(int[] values)
		{
			// portcount * VALUE(4B)
			//Datos
			byte[] Datos = null;
			Datos = new byte[(m_PortData.Length * 4) - 1 + 1];
			for (var i = 0; i <= m_PortData.Length - 1; i++)
			{
				byte[] ValueB = BitConverter.GetBytes(values[i]);
				Array.Copy(ValueB, 0, Datos, System.Convert.ToInt32(4 * i), 4);
			}
			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_HIBERTIME;

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sin herramienta 0xC8 0xD8

		/// <summary>
		/// Le pide al Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		public void ReadIdleTime(CounterTypes eType)
		{

			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_NOTOOLTIME;
			}
			else
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_NOTOOLTIMEP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Minutos Sin herramienta 0xC9

		/// <summary>
		/// Guarda en el Equipo el tiempo en minutos del puerto en hibernación
		/// </summary>
		/// <remarks></remarks>
		internal void WriteIdleTime(int[] values)
		{
			// portcount * VALUE(4B)
			//Datos
			byte[] Datos = null;
			Datos = new byte[(m_PortData.Length * 4) - 1 + 1];
			for (var i = 0; i <= m_PortData.Length - 1; i++)
			{
				byte[] ValueB = BitConverter.GetBytes(values[i]);
				Array.Copy(ValueB, 0, Datos, System.Convert.ToInt32(4 * i), 4);
			}

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_NOTOOLTIME;

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos Sleep 0xCA 0xDA

		/// <summary>
		/// Le pide al Equipo los ciclos de sleep del puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadSleepCycles(CounterTypes eType)
		{

			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos Sleep 0xCB

		/// <summary>
		/// Guarda en el Equipo los ciclos de sleep del puerto
		/// </summary>
		/// <remarks></remarks>
		internal void WriteSleepCycles(int[] values)
		{
			// portcount * VALUE(4B)
			//Datos
			byte[] Datos = null;
			Datos = new byte[(m_PortData.Length * 4) - 1 + 1];
			for (var i = 0; i <= m_PortData.Length - 1; i++)
			{
				byte[] ValueB = BitConverter.GetBytes(values[i]);
				Array.Copy(ValueB, 0, Datos, System.Convert.ToInt32(4 * i), 4);
			}

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPCYCLES;

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos desoldador 0xCC 0xDC

		/// <summary>
		/// Le pide al Equipo los ciclos de desoldador del puerto
		/// </summary>
		/// <remarks></remarks>
		public void ReadDesoldCycles(CounterTypes eType)
		{

			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_DESOLCYCLES;
			}
			else
			{
				Command = (byte)EnumCommandFrame_01_SOLD.M_R_DESOLCYCLESP;
			}

			SendMessage(Datos, Command);
		}

#endregion


#region Ciclos desoldador 0xCD

		/// <summary>
		/// Guarda en el Equipo los ciclos de desoldador del puerto
		/// </summary>
		/// <remarks></remarks>
		internal void WriteDesoldCycles(int[] values)
		{
			// portcount * VALUE(4B)
			//Datos
			byte[] Datos = null;
			Datos = new byte[(m_PortData.Length * 4) - 1 + 1];
			for (var i = 0; i <= m_PortData.Length - 1; i++)
			{
				byte[] ValueB = BitConverter.GetBytes(values[i]);
				Array.Copy(ValueB, 0, Datos, System.Convert.ToInt32(4 * i), 4);
			}

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_DESOLCYCLES;

			SendMessage(Datos, Command);
		}

#endregion

#endregion


#region Communications

#region Configuración ETH 0xE7 0xE8

		public void ReadEthernetConfiguration()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_ETH_TCPIPCONFIG;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_ETH_TCPIPCONFIG;

			SendMessage(Datos, Command);
		}

#endregion


#region Configuración Robot 0xF0 0xF1

		public void ReadRobotConfiguration()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_RBT_CONNCONFIG;

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
				Datos[2] = 0x4F; //"O"
			}
			else if (robotData.Parity == CRobotData.RobotParity.Even)
			{
				Datos[2] = 0x45; //"E"
			}
			else
			{
				Datos[2] = 0x4E; //"N"
			}

			Datos[3] = System.Convert.ToByte(robotData.StopBits);
			Datos[4] = System.Convert.ToByte(robotData.Protocol);
			//big endian
			Datos[5] = System.Convert.ToByte((robotData.Address / 10) + 48);
			Datos[6] = System.Convert.ToByte((robotData.Address % 10) + 48);

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_RBT_CONNCONFIG;

			SendMessage(Datos, Command);
		}

#endregion


#region Estado Robot 0xF2 0xF3

		public void ReadRobotStatus()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_RBT_CONNECTSTATUS;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_RBT_CONNECTSTATUS;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_PERIPHCOUNT;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_PERIPHCONFIG;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_W_PERIPHCONFIG;

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
			byte Command = (byte)EnumCommandFrame_01_SOLD.M_R_PERIPHSTATUS;

			SendMessage(Datos, Command);
		}

#endregion

#endregion

#endregion

#region DECODE FRAMES

		public void ProcessMessageResponse(uint Numstream, byte[] Datos, byte Command, byte Origen)
		{

			//
			//Sent message
			//
			MessageHashtable.Message MessageInt = new MessageHashtable.Message();
			int Puerto = 0;
			int Tool = 0;

			//recuperar mensaje
			if (m_MessagesSentManager.ReadMessage(Numstream, ref MessageInt))
			{
				m_MessagesSentManager.RemoveMessage(Numstream);

				//obtener puerto y tool
				if (MessageInt.Datos.Length == 1)
				{
					Puerto = MessageInt.Datos[0];
				}
				else if (MessageInt.Datos.Length == 2)
				{
					Puerto = MessageInt.Datos[0];
					Tool = SearchToolArray(GetGenericToolFromInternal(MessageInt.Datos[1]));
				}

				//Mensajes recibidos sin petición
			}
			else if (Command != (byte)EnumCommandFrame_01_SOLD.M_I_CONTIMODE)
			{
				return;
			}


			//
			//Command
			//
			if (Command == (byte)EnumCommandFrame_01_SOLD.M_NULL)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_SYN)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_ACK)
			{
				if (EndedTransactionEvent != null)
					EndedTransactionEvent(Numstream);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_NACK)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_CONNECTSTATUS)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				if (Strings.ChrW(Datos[0]) == 'C')
				{
					m_StationData.Status.ControlMode = ControlModeConnection.CONTROL;
				}
				else
				{
					m_StationData.Status.ControlMode = ControlModeConnection.MONITOR;
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_CONNECTSTATUS)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_RESET)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_FIRMWARE)
			{
				if (Datos.Length == 0)
				{
					return;
				}

				string datosVersions = Encoding.UTF8.GetString(Datos);
				string[] arrayVersions = datosVersions.Split(':');
				if (arrayVersions.Length < 4)
				{
				}

				//protocolo
				m_StationData.Info.Protocol = arrayVersions[0];

				//modelo
				string[] arrayModelData = arrayVersions[1].Split('_');
				m_StationData.Info.Model = arrayModelData[0];
				if (arrayModelData.Length > 1)
				{
					m_StationData.Info.ModelType = arrayModelData[1];
				}
				if (arrayModelData.Length > 2)
				{
					if (Information.IsNumeric(arrayModelData[2]))
					{
						m_StationData.Info.ModelVersion = int.Parse(arrayModelData[2]);
					}
				}

				//soft y hard versions
				// 01/06/2016 Edu Se detecta que en protocolo 01, al grabar un UID de 20 caracteres, añade los últimos 7 también al Software del string de FIRMWARE,
				// por lo tanto se toman sólo los 7 primeros caracteres del software y del hardware
				m_StationData.Info.Version_Software = arrayVersions[2].Trim().Substring(0, 7);
				m_StationData.Info.Version_Hardware = arrayVersions[3].Trim().Substring(0, 7);


				//El micro está conectado, actualizamos la marca de tiempo
				if (m_StationData.Info.StationMicros.Contains(Origen))
				{
					((CFirmwareStation)(m_StationData.Info.StationMicros[Origen])).SetTimeMarkConnected();
					((CFirmwareStation)(m_StationData.Info.StationMicros[Origen])).SoftwareVersion = m_StationData.Info.Version_Software; //actualizamos el software version ya que ha podido cambiar por una actualización

					//Guardar información del micro
				}
				else
				{
					CFirmwareStation micro = new CFirmwareStation();
					micro.StationUUID = m_StationData.Info.UUID;
					micro.Model = m_StationData.Info.Model;
					micro.HardwareVersion = m_StationData.Info.Version_Hardware;
					micro.SoftwareVersion = m_StationData.Info.Version_Software;
					micro.SetTimeMarkConnected();
					m_StationData.Info.StationMicros.Add(Origen, micro);
				}

			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_INF_PORT)
			{
				if (Datos.Length != 12 & Datos.Length != 15)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.ConnectedTool = GetGenericToolFromInternal(Datos[0]);
				//Debug.Print(String.Format("Tool number {0} M_INF_PORT: Puerto {1}", Datos(0).ToString, (Puerto + 1).ToString))
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
				// 12/03/2015 forzar el Stand_OnOff a OFF (en varsión 01 del protocolo se establece con la combinación
				//            TimeToSleepHibern > 0 and FutureMode = Sleep
				m_PortData[Puerto].ToolStatus.Stand_OnOff = OnOff._OFF;
				m_PortData[Puerto].ToolStatus.Sleep_OnOff = (OnOff)(Datos[10] & 0x1);
				m_PortData[Puerto].ToolStatus.Hiber_OnOff = (OnOff)((Datos[10] & 0x2) >> 1);
				m_PortData[Puerto].ToolStatus.Extractor_OnOff = (OnOff)((Datos[10] & 0x4) >> 2);
				m_PortData[Puerto].ToolStatus.Desold_OnOff = (OnOff)((Datos[10] & 0x8) >> 3);

				//
				// Changes
				//

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


				if (Datos.Length == 15)
				{

					// #Edu# cargar datos de TimeToSleepHibern Datos(12 y 13) y FutureMode Datos(14)
					m_PortData[Puerto].ToolStatus.TimeToSleepHibern = System.Convert.ToInt32(BitConverter.ToUInt16(Datos, 12)); // datos 12 y 13
					m_PortData[Puerto].ToolStatus.FutureMode_Tool = (ToolFutureMode)(Datos[14]);
					// #Edu# 12/03/2013 si hay delay y el future mode es Sleep, entonces marcar estado como Stand
					if (m_PortData[Puerto].ToolStatus.TimeToSleepHibern > 0 && m_PortData[Puerto].ToolStatus.FutureMode_Tool == ToolFutureMode.Sleep)
					{
						m_PortData[Puerto].ToolStatus.Sleep_OnOff = OnOff._OFF;
						m_PortData[Puerto].ToolStatus.Stand_OnOff = OnOff._ON;
					}
					//Debug.Print(String.Format("TimeToSleepHibern: {0} FutureMode: {1}", Info_Port(Puerto).ToolStatus.TimeToSleepHibern.ToString, Info_Port(Puerto).ToolStatus.FutureMode.ToString))
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_FIXTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				ushort aux_UShort = BitConverter.ToUInt16(Datos, 0);
				if (aux_UShort == 0xFFFF)
				{
					m_PortData[Puerto].ToolSettings[Tool].FixedTemp_OnOff = OnOff._OFF;
					m_PortData[Puerto].ToolSettings[Tool].FixedTemp.UTI = aux_UShort;
				}
				else
				{
					m_PortData[Puerto].ToolSettings[Tool].FixedTemp_OnOff = OnOff._ON;
					m_PortData[Puerto].ToolSettings[Tool].FixedTemp.UTI = aux_UShort;
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_FIXTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_LEVELTEMP)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTempSelect = (ToolTemperatureLevels)(Datos[0]);
				// 22/03/2013 check temp levels. stations do not check levels data until tool is connected
				// 18/04/2013 checkTempLevels function. se corrige las llamadas porque estaban pasando la enumeración equivocada.
				CheckTempLevels((Port)Puerto, m_PortData[Puerto].ToolSettings[Tool].Tool);
				//Debug.Print(String.Format("M_R_LEVELTEMP : {0} ", Info_Port(Puerto).ToolParam.ToolSettings(Tool).LevelsTempSelect.ToString))
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_LEVELTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_LEVEL1)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[0].UTI = BitConverter.ToUInt16(Datos, 0);
				// 22/03/2013 check temp levels. stations do not check levels data until tool is connected
				// 18/04/2013 checkTempLevels function. se corrige las llamadas porque estaban pasando la enumeración equivocada.
				CheckTempLevels((Port)Puerto, m_PortData[Puerto].ToolSettings[Tool].Tool);
				//Debug.Print(String.Format("M_R_LEVEL1 : {0} ", Info_Port(Puerto).ToolParam.ToolSettings(Tool).LevelsTemp(0).Value.ToString))
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_LEVEL1)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_LEVEL2)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[1].UTI = BitConverter.ToUInt16(Datos, 0);
				// 22/03/2013 check temp levels. stations do not check levels data until tool is connected
				// 18/04/2013 checkTempLevels function. se corrige las llamadas porque estaban pasando la enumeración equivocada.
				CheckTempLevels((Port)Puerto, m_PortData[Puerto].ToolSettings[Tool].Tool);
				//Debug.Print(String.Format("M_R_LEVEL2 : {0} ", Info_Port(Puerto).ToolParam.ToolSettings(Tool).LevelsTemp(1).Value.ToString))
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_LEVEL2)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_LEVEL3)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].Levels.LevelsTemp[2].UTI = BitConverter.ToUInt16(Datos, 0);
				// 22/03/2013 check temp levels. stations do not check levels data until tool is connected
				// 18/04/2013 checkTempLevels function. se corrige las llamadas porque estaban pasando la enumeración equivocada.
				CheckTempLevels((Port)Puerto, m_PortData[Puerto].ToolSettings[Tool].Tool);

				//Debug.Print(String.Format("M_R_LEVEL3 : {0} ", Info_Port(Puerto).ToolParam.ToolSettings(Tool).LevelsTemp(2).Value.ToString))
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_LEVEL3)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPDELAY)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].SleepTime = (ToolTimeSleep)((ToolTimeSleep)(BitConverter.ToUInt16(Datos, 0)));
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPDELAY)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].SleepTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_HIBERDELAY)
			{
				// 2012_12_05 Se modifica para que pueda recibir FFFF (igual que M_R_SLEEPDELAY) - Edu
				//If Datos.Length <> 1 Then
				//    ReDim Preserve Datos(0)
				//End If
				//Info_Port(Puerto).ToolParam.ToolSettings(Tool).HiberTime = CType(Datos(0), ToolTimeHibernation)
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].HiberTime = (ToolTimeHibernation)((ToolTimeHibernation)(BitConverter.ToUInt16(Datos, 0)));
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_HIBERDELAY)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_AJUSTTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolSettings[Tool].AdjustTemp.UTI = BitConverter.ToInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_AJUSTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_SELECTTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.SelectedTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_SELECTTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_TIPTEMP)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.ActualTemp[0].UTI = BitConverter.ToUInt16(Datos, 0);
				m_PortData[Puerto].ToolStatus.ActualTemp[1].UTI = BitConverter.ToUInt16(Datos, 2);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_CURRENT)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.Current_mA[0] = BitConverter.ToUInt16(Datos, 0);
				m_PortData[Puerto].ToolStatus.Current_mA[1] = BitConverter.ToUInt16(Datos, 2);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_POWER)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.Power_x_Mil[0] = BitConverter.ToUInt16(Datos, 0);
				m_PortData[Puerto].ToolStatus.Power_x_Mil[1] = BitConverter.ToUInt16(Datos, 2);
				//Debug.Print(String.Format("Tool {0} M_R_POWER: Puerto {1} ToUInt16(Datos, 0) {2} ToUInt16(Datos, 2): {3}", Info_Port(Puerto).ToolStatus.ConnectedTool.ToString, (Puerto + 1).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(0).ToString, Info_Port(Puerto).ToolStatus.Power_x_Mil(1).ToString))
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_CONNECTTOOL)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.ConnectedTool = GetGenericToolFromInternal(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_TOOLERROR)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.ToolError = (ToolError)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_STATUSTOOL)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.Sleep_OnOff = (OnOff)(Datos[0] & 0x1);
				m_PortData[Puerto].ToolStatus.Hiber_OnOff = (OnOff)((Datos[0] & 0x2) >> 1);
				m_PortData[Puerto].ToolStatus.Extractor_OnOff = (OnOff)((Datos[0] & 0x4) >> 2);
				m_PortData[Puerto].ToolStatus.Desold_OnOff = (OnOff)((Datos[0] & 0x8) >> 3);
				// #Edu# 12/03/2013 si hay delay y el future mode es Sleep, entonces marcar estado como Stand
				if (m_PortData[Puerto].ToolStatus.TimeToSleepHibern > 0 && m_PortData[Puerto].ToolStatus.FutureMode_Tool == ToolFutureMode.Sleep)
				{
					m_PortData[Puerto].ToolStatus.Sleep_OnOff = OnOff._OFF;
					m_PortData[Puerto].ToolStatus.Stand_OnOff = OnOff._ON;
				}
				else
				{
					// 12/03/2015 forzar el Stand_OnOff a OFF (en varsión 01 del protocolo se establece con la combinación
					//            TimeToSleepHibern > 0 and FutureMode = Sleep)
					m_PortData[Puerto].ToolStatus.Stand_OnOff = OnOff._OFF;
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_MOSTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.Temp_MOS.UTI = System.Convert.ToInt32(BitConverter.ToUInt16(Datos, 0));
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_DELAYTIME)
			{
				if (Datos.Length != 3)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.TimeToSleepHibern = System.Convert.ToInt32(BitConverter.ToUInt16(Datos, 0));
				m_PortData[Puerto].ToolStatus.FutureMode_Tool = (ToolFutureMode)(Datos[2]);
				// #Edu# 12/03/2013 si hay delay y el future mode es Sleep, entonces marcar estado como Stand
				if (m_PortData[Puerto].ToolStatus.TimeToSleepHibern > 0 && m_PortData[Puerto].ToolStatus.FutureMode_Tool == ToolFutureMode.Sleep)
				{
					m_PortData[Puerto].ToolStatus.Sleep_OnOff = OnOff._OFF;
					m_PortData[Puerto].ToolStatus.Stand_OnOff = OnOff._ON;
				}
				else
				{
					// 12/03/2015 forzar el Stand_OnOff a OFF (en varsión 01 del protocolo se establece con la combinación
					//            TimeToSleepHibern > 0 and FutureMode = Sleep)
					m_PortData[Puerto].ToolStatus.Stand_OnOff = OnOff._OFF;
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_REMOTEMODE)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Status.RemoteMode = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_REMOTEMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_STATUSREMOTEMODE)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_PortData[Puerto].ToolStatus.StatusRemoteMode.Sleep_OnOff = (OnOff)(Datos[0] & 0x1);
				m_PortData[Puerto].ToolStatus.StatusRemoteMode.Extractor_OnOff = (OnOff)((Datos[0] & 0x4) >> 2);
				m_PortData[Puerto].ToolStatus.StatusRemoteMode.Desold_OnOff = (OnOff)((Datos[0] & 0x8) >> 3);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_STATUSREMOTEMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_CONTIMODE)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				//m_StationData.ContinuousModeSpeed = CType(Datos(0), SpeedContinuousMode)
				//m_StationData.ContinuousModePorts = Datos(1)
				m_StationData.Status.ContinuousModeStatus.speed = (SpeedContinuousMode)(Datos[0]);
				m_StationData.Status.ContinuousModeStatus.setPortsFromByte(Datos[1]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_CONTIMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_I_CONTIMODE)
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
					int iPortDataBlock = 9;

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

								// bits en protocolo 01:  Desoldador:3 Extractor:2 Hibernación:1 Sleep:0
								if ((Datos[offset] & 0x1) == (byte)ToolStatus.SLEEP)
								{
									data.data[i].status = ToolStatus.SLEEP;
								}
								else if ((Datos[offset] & 0x2) == (byte)ToolStatus.HIBERNATION)
								{
									data.data[i].status = ToolStatus.HIBERNATION;
								}
								else if ((Datos[offset] & 0x4) == (byte)ToolStatus.EXTRACTOR)
								{
									data.data[i].status = ToolStatus.EXTRACTOR;
								}
								else
								{
									data.data[i].status = ToolStatus.NONE;
								}

								if ((Datos[offset] & 0x8) > 0)
								{
									data.data[i].desoldering = OnOff._ON;
								}
								else
								{
									data.data[i].desoldering = OnOff._OFF;
								}

								//Debug.Print("CONT MODE STATUS prot 01: {0}", data.data(i).status.ToString)
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
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_TEMPUNIT)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.Unit = (CTemperature.TemperatureUnit)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_TEMPUNIT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_MAXTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.MaxTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_MAXTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_MINTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.MinTemp.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_MINTEMP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_NITROMODE)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.N2Mode = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_NITROMODE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_HELPTEXT)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.HelpText = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_HELPTEXT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_POWERLIM)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Settings.PowerLimit = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_POWERLIM)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_BEEP)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.Beep = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_BEEP)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_PARAMETERSLOCKED)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.ParametersLocked = (OnOff)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_PARAMETERSLOCKED)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_LANGUAGE)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Settings.Language = (Idioma)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_LANGUAGE)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_TEMPERRORTRAFO)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Status.TempErrorTRAFO.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_TEMPERRORMOS)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Status.TempErrorMOS.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_PIN)
			{
				if (Datos.Length != 4)
				{
					return;
				}

				m_StationData.Settings.PIN = Encoding.UTF8.GetString(Datos);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_PIN)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_STATERROR)
			{
				if (Datos.Length != 1)
				{
					return;
				}

				m_StationData.Status.ErrorStation = (StationError)(Datos[0]);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_TRAFOTEMP)
			{
				if (Datos.Length != 2)
				{
					return;
				}

				m_StationData.Status.TempTRAFO.UTI = BitConverter.ToUInt16(Datos, 0);
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_RESETSTATION)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_DEVICENAME)
			{
				string TextoName = Encoding.UTF8.GetString(Datos);
				m_StationData.Settings.Name = TextoName;
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_DEVICENAME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_DEVICEID)
			{
				clsStationUID stationUUID = new clsStationUID(Datos);
				m_StationData.Info.UUID = stationUUID.UID;
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_DEVICEID)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_PLUGTIME)
			{
				for (int index = 0; index <= m_PortData.Length - 1; index++)
				{
					m_PortData[index].Counters.ContPlugMinutes = BitConverter.ToInt32(Datos, 4 * index);
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_PLUGTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_WORKTIME)
			{
				for (int index = 0; index <= m_PortData.Length - 1; index++)
				{
					m_PortData[index].Counters.ContWorkMinutes = BitConverter.ToInt32(Datos, 4 * index);
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_WORKTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPTIME)
			{
				for (int index = 0; index <= m_PortData.Length - 1; index++)
				{
					m_PortData[index].Counters.ContSleepMinutes = BitConverter.ToInt32(Datos, 4 * index);
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_HIBERTIME)
			{
				for (int index = 0; index <= m_PortData.Length - 1; index++)
				{
					m_PortData[index].Counters.ContHiberMinutes = BitConverter.ToInt32(Datos, 4 * index);
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_HIBERTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_NOTOOLTIME)
			{
				for (int index = 0; index <= m_PortData.Length - 1; index++)
				{
					m_PortData[index].Counters.ContIdleMinutes = BitConverter.ToInt32(Datos, 4 * index);
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_NOTOOLTIME)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_SLEEPCYCLES)
			{
				for (int index = 0; index <= m_PortData.Length - 1; index++)
				{
					m_PortData[index].Counters.ContSleepCycles = BitConverter.ToInt32(Datos, 4 * index);
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_SLEEPCYCLES)
			{
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_R_DESOLCYCLES)
			{
				for (int index = 0; index <= m_PortData.Length - 1; index++)
				{
					m_PortData[index].Counters.ContDesoldCycles = BitConverter.ToInt32(Datos, 4 * index);
				}
			}
			else if (Command == (byte)EnumCommandFrame_01_SOLD.M_W_DESOLCYCLES)
			{
			}
			else
			{
				//No hacer nada
			}
		}

#endregion

#region CHECK PARAMETERS

		// 22/03/2013 checkTempLevels function. stations do not check levels data until tool is connected
		private void CheckTempLevels(Port Port, GenericStationTools Tool)
		{

			if (Tool != GenericStationTools.NO_TOOL)
			{
				int ToolIdx = SearchToolArray(Tool);
				bool bSearchForATemp = false;

				if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect == ToolTemperatureLevels.FIRST_LEVEL)
				{
					if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTemp[0].UTI == Constants.NO_TEMP_LEVEL)
					{
						bSearchForATemp = true;
					}
				}
				else if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect == ToolTemperatureLevels.SECOND_LEVEL)
				{
					if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTemp[1].UTI == Constants.NO_TEMP_LEVEL)
					{
						bSearchForATemp = true;
					}
				}
				else if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect == ToolTemperatureLevels.THIRD_LEVEL)
				{
					if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTemp[2].UTI == Constants.NO_TEMP_LEVEL)
					{
						bSearchForATemp = true;
					}
				}
				else if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect == ToolTemperatureLevels.NO_LEVELS)
				{
				}

				if (bSearchForATemp)
				{
					if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTemp[0].UTI != Constants.NO_TEMP_LEVEL)
					{
						m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect = ToolTemperatureLevels.FIRST_LEVEL;
					}
					else if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTemp[1].UTI != Constants.NO_TEMP_LEVEL)
					{
						m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect = ToolTemperatureLevels.SECOND_LEVEL;
					}
					else if (m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTemp[2].UTI != Constants.NO_TEMP_LEVEL)
					{
						m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect = ToolTemperatureLevels.THIRD_LEVEL;
					}
					else
					{
						m_PortData[(int)Port].ToolSettings[ToolIdx].Levels.LevelsTempSelect = ToolTemperatureLevels.NO_LEVELS;
					}
				}
			}
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
