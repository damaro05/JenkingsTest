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

using System.Text;
using System.Net;
using Microsoft.VisualBasic.CompilerServices;
using DataJBC;
using RoutinesJBC;


namespace JBC_Connect
{
	
	
	internal class CStationFrames02_FE : CStationFrames
	{
		
		protected CStationData_FE m_StationData; //Información de la estación
		protected CPortData_FE[] m_PortData; //Información del puerto
		
		
		//Substation events
		public delegate void Detected_SubStationEventHandler(CConnectionData connectionData);
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
		
		//Bootloader events
		public delegate void ClearingFlashFinishedEventHandler();
		private ClearingFlashFinishedEventHandler ClearingFlashFinishedEvent;
		
		public event ClearingFlashFinishedEventHandler ClearingFlashFinished
		{
			add
			{
				ClearingFlashFinishedEvent = (ClearingFlashFinishedEventHandler) System.Delegate.Combine(ClearingFlashFinishedEvent, value);
			}
			remove
			{
				ClearingFlashFinishedEvent = (ClearingFlashFinishedEventHandler) System.Delegate.Remove(ClearingFlashFinishedEvent, value);
			}
		}
		
		public delegate void AddressMemoryFlashFinishedEventHandler();
		private AddressMemoryFlashFinishedEventHandler AddressMemoryFlashFinishedEvent;
		
		public event AddressMemoryFlashFinishedEventHandler AddressMemoryFlashFinished
		{
			add
			{
				AddressMemoryFlashFinishedEvent = (AddressMemoryFlashFinishedEventHandler) System.Delegate.Combine(AddressMemoryFlashFinishedEvent, value);
			}
			remove
			{
				AddressMemoryFlashFinishedEvent = (AddressMemoryFlashFinishedEventHandler) System.Delegate.Remove(AddressMemoryFlashFinishedEvent, value);
			}
		}
		
		public delegate void DataMemoryFlashFinishedEventHandler();
		private DataMemoryFlashFinishedEventHandler DataMemoryFlashFinishedEvent;
		
		public event DataMemoryFlashFinishedEventHandler DataMemoryFlashFinished
		{
			add
			{
				DataMemoryFlashFinishedEvent = (DataMemoryFlashFinishedEventHandler) System.Delegate.Combine(DataMemoryFlashFinishedEvent, value);
			}
			remove
			{
				DataMemoryFlashFinishedEvent = (DataMemoryFlashFinishedEventHandler) System.Delegate.Remove(DataMemoryFlashFinishedEvent, value);
			}
		}
		
		public delegate void EndProgFinishedEventHandler();
		private EndProgFinishedEventHandler EndProgFinishedEvent;
		
		public event EndProgFinishedEventHandler EndProgFinished
		{
			add
			{
				EndProgFinishedEvent = (EndProgFinishedEventHandler) System.Delegate.Combine(EndProgFinishedEvent, value);
			}
			remove
			{
				EndProgFinishedEvent = (EndProgFinishedEventHandler) System.Delegate.Remove(EndProgFinishedEvent, value);
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
		
		
		
		public CStationFrames02_FE(CStationData_FE _StationData, CPortData_FE[] _PortData, CCommunicationChannel _ComChannel, byte _StationNumDevice)
			{
			
			m_StationData = _StationData;
			m_PortData = _PortData;
			m_ComChannel = _ComChannel;
			m_ComChannel.MessageResponse += ProcessMessageResponse;
			m_ComChannel.MessageResponse += ProcessMessageResponse;
			m_ComChannel.ConnectionError += ComChannelConnectionError;
			m_ComChannel.ResetSended += ComChannelResetSended;
			m_ComChannel.MessageResponse += ProcessMessageResponse;
			m_ComChannel.MessageResponse += ProcessMessageResponse;
			m_ComChannel.MessageResponse += ProcessMessageResponse;
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
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_DEVICEID;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_DEVICEID;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_CLEARMEMFLASH;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_SENDMEMADDRESS;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_SENDMEMDATA;
			
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
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_ENDPROGR;
			
			SendMessage(Datos, Command, address);
		}
		
#endregion
		
#endregion
		
		
#region Port + Tool
		
#region Suction Level 0x30 0x31
		
		public void ReadSuctionLevel()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_SUCTIONLEVEL;
			
			SendMessage(Datos, Command);
		}
		
		public void WriteSuctionLevel(SuctionLevel_FE level)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)level;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_SUCTIONLEVEL;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Flow 0x32
		
		public void ReadFlow()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_FLOW;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Speed 0x33
		
		public void ReadSpeed()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_SPEED;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Caudal Seleccionado 0x34 0x35
		
		/// <summary>
		/// Le pide al Equipo el caudal Seleccionado
		/// </summary>
		/// <remarks></remarks>
		public void ReadSelectFlow()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_SELECTFLOW;
			
			SendMessage(Datos, Command);
		}
		
		/// <summary>
		/// Guarda en el Equipo el caudal Seleccionado
		/// </summary>
		/// <remarks></remarks>
		public uint WriteSelectFlow(int value)
		{
			// Value(2B)
			
			//Datos
			byte[] Datos = BitConverter.GetBytes(value);
			Array.Resize(ref Datos, 2);
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_SELECTFLOW;
			
			return SendMessage(Datos, Command);
		}
		
#endregion
		
#region Stand intake 0x36 0x37
		
		public void ReadStandIntakes(Port Port)
		{
			// PORT(1B)
			
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_STANDINTAKES;
			
			SendMessage(Datos, Command);
		}
		
		public void WriteStandIntakes(Port Port, int intakes)
		{
            // INTAKES(1B) + PORT(1B)

            //Datos
            byte[] Datos = new byte[2];
            Datos[0] = (byte)intakes;
			Datos[1] = (byte)Port;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_STANDINTAKES;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Intake Activation 0x38 0x39
		
		public void ReadIntakeActivation(Port Port, ToolStatus_FE intake)
		{
			// PORT(1B) + INTAKE(1B)
			
			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = (byte)intake;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_INTAKEACTIVATION;
			
			SendMessage(Datos, Command);
		}
		
		public void WriteWorkIntakeActivation(Port Port, OnOff onoff)
		{
			// PORT(1B) + ONOFF(1B)
			
			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = (byte)onoff;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_INTAKEACTIVATION;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Suction Delay 0x3A 0x3B
		
		public void ReadSuctionDelay(Port Port, ToolStatus_FE intake)
		{
			// PORT(1B) + INTAKE(1B)
			
			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = (byte)intake;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_SUCTIONDELAY;
			
			SendMessage(Datos, Command);
		}
		
		public void WriteSuctionDelay(Port Port, ToolStatus_FE intake, int delay)
		{
            // DELAY(2B) + PORT(1B) + INTAKES(1B)

            //Datos
            byte[] Datos = BitConverter.GetBytes(delay);
            Datos[2] = (byte)Port;
            Datos[3] = (byte)intake;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_SUCTIONDELAY;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Time to close 0x3C
		
		public void ReadTimeToStopSuction(Port Port, ToolStatus_FE intake)
		{
			// PORT(1B) + INTAKE(1B)
			
			//Datos
			byte[] Datos = new byte[2];
			Datos[0] = (byte)Port;
			Datos[1] = (byte)intake;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_DELAYTIME;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Activation pedal 0x3D 0x3E
		
		public void ReadActivationPedal(Port Port)
		{
			// PORT(1B)
			
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_ACTIVATIONPEDAL;
			
			SendMessage(Datos, Command);
		}
		
		public void WriteActivationPedal(Port Port, PedalAction activation)
		{
			// ACTION(1B) + PORT(1B)
			
			//Datos
			byte[] Datos = new byte[2];
			if (activation == PedalAction.HOLD_DOWN)
			{
				Datos[0] = (byte) 0;
			}
			else
			{
				Datos[0] = (byte) 1;
			}
			Datos[1] = (byte)Port;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_ACTIVATIONPEDAL;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Pedal mode 0x3F 0x40
		
		public void ReadPedalMode(Port Port)
		{
			// PORT(1B)
			
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = (byte)Port;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_PEDALMODE;
			
			SendMessage(Datos, Command);
		}
		
		public void WritePedalMode(Port Port, PedalMode mode)
		{
            // MODE(1B) + PORT(1B)

            //Datos
            byte[] Datos = new byte[2];
            Datos[0] = (byte)mode;
			Datos[1] = (byte)Port;
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_PEDALMODE;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Filter status 0x41
		
		public void ReadFilterStatus()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_FILTERSTATUS;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#region Filter reset 0x42
		
		public void ReadResetFilter()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_RESETFILTER;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#endregion
		
		
#region Station method
		
#region Reset Parametros 0x50
		
		public void ReadResetParam()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_RESETSTATION;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
		
#region PIN 0x51 0x52
		
		/// <summary>
		/// Lee del equipo conectado su PIN
		/// </summary>
		/// <remarks></remarks>
		public void ReadDevicePIN()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_PIN;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_PIN;
			
			return SendMessage(Datos, Command);
		}
		
#endregion
		
		
#region Beep 0x55 0x56
		
		/// <summary>
		/// Le pide al Equipo el estado del Beep
		/// </summary>
		/// <remarks></remarks>
		public void ReadBeep()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_BEEP;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_BEEP;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
		
#region Continuous suction 0x57 0x58
		
		/// <summary>
		///
		/// </summary>
		/// <remarks></remarks>
		public void ReadContinuousSuction()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_CONTINUOUSSUCTION;
			
			SendMessage(Datos, Command);
		}
		
		/// <summary>
		///
		/// </summary>
		/// <remarks></remarks>
		public void WriteContinuousSuction(OnOff suction)
		{
			//Datos
			byte[] Datos = new byte[1];
			Datos[0] = System.Convert.ToByte(suction);
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_CONTINUOUSSUCTION;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
		
#region Error estación 0x59
		
		/// <summary>
		/// Le pide al Equipo el error de estación si lo hubiera
		/// </summary>
		/// <remarks></remarks>
		internal void ReadStationError()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_STATERROR;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
		
#region Nombre equipo 0x5B 0x5C
		
		/// <summary>
		/// Lee del equipo conectado su nombre
		/// </summary>
		/// <remarks></remarks>
		public void ReadDeviceName()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_DEVICENAME;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_DEVICENAME;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#endregion
		
		
#region Counters
		
#region Counters 0xC0 0xC1 0xC2 0xC3
		
		public void ReadCounters(CounterTypes eType)
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_FE.M_R_COUNTERS;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_FE.M_R_COUNTERSP;
			}
			
			SendMessage(Datos, Command);
		}
		
		public void ReadResetCounters(CounterTypes eType)
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = 0;
			if (eType == CounterTypes.GLOBAL_COUNTER)
			{
				Command = (byte)EnumCommandFrame_02_FE.M_R_RESETCOUNTERS;
			}
			else
			{
				Command = (byte)EnumCommandFrame_02_FE.M_R_RESETCOUNTERSP;
			}
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
#endregion
		
		
#region Communications
		
#region USB Connection Mode 0xE0 0xE1
		
		/// <summary>
		/// Lee del equipo el estado de la conexión USB o ETH:
		///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
		///   * Control Mode: en este estado la estación sólo es configurable desde el PC
		/// </summary>
		/// <remarks></remarks>
		public void ReadConnectStatus()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_USB_CONNECTSTATUS;
			
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
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_USB_CONNECTSTATUS;
			
			SendMessage(Datos, Command);
		}
		
#endregion
		
		
#region Configuración Robot 0xF0 0xF1
		
		public void ReadRobotConfiguration()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_RBT_CONNCONFIG;
			
			SendMessage(Datos, Command);
		}
		
		public uint WriteRobotConfiguration(CRobotData robotData)
		{
			//Datos
			byte[] Datos = new byte[7];
			
			Datos[0] = System.Convert.ToByte(robotData.Speed);
			Datos[1] = (byte) (robotData.DataBits + 48);
			
			if (robotData.Parity == CRobotData.RobotParity.Odd)
			{
				Datos[2] = (byte) (Strings.AscW("O"));
			}
			else if (robotData.Parity == CRobotData.RobotParity.Even)
			{
				Datos[2] = (byte) (Strings.AscW("E"));
			}
			else
			{
				Datos[2] = (byte) (Strings.AscW("N"));
			}
			
			Datos[3] = System.Convert.ToByte(robotData.StopBits);
			Datos[4] = System.Convert.ToByte(robotData.Protocol);
			//big endian
			Datos[5] = System.Convert.ToByte((robotData.Address / 10) + 48);
			Datos[6] = System.Convert.ToByte((robotData.Address % 10) + 48);
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_RBT_CONNCONFIG;
			
			return SendMessage(Datos, Command);
		}
		
#endregion
		
		
#region Estado Robot 0xF2 0xF3
		
		public void ReadRobotStatus()
		{
			//Datos
			byte[] Datos = new byte[] {};
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_R_RBT_CONNECTSTATUS;
			
			SendMessage(Datos, Command);
		}
		
		public void WriteRobotStatus(OnOff status)
		{
			//Datos
			byte[] Datos = new byte[1];
			
			if (status == OnOff._ON)
			{
				Datos[0] = (byte) (Strings.AscW("C"));
			}
			else
			{
				Datos[0] = (byte) (Strings.AscW("N"));
			}
			
			//Command
			byte Command = (byte)EnumCommandFrame_02_FE.M_W_RBT_CONNECTSTATUS;
			
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
				if (m_StationData.Info.Features.SubStations && (m_StationNumDevice + (int) EnumAddress.NEXT_SUBSTATION_ADDRESS) == Origen)
				{
					
					//Las direcciones que vengan del mismo bloque de estación
				}
				else if ((Origen & (byte)EnumAddress.MASK_STATION_ADDRESS) == m_StationNumDevice)
				{
					
				}
				else
				{
					return ;
				}
			}
			
			
			//
			//Sent message
			//
			MessageHashtable.Message MessageInt = new MessageHashtable.Message();
			int Puerto = 0;
			int Intake = 0;
			string sDatos = "";
			string[] aDatos = null;
			string sTemp = "";
			
			//Recuperar mensaje
			if (m_MessagesSentManager.ReadMessage(Numstream, ref MessageInt))
			{
				m_MessagesSentManager.RemoveMessage(Numstream);
				
				//Mensajes recibidos sin petición. (Firmware -> sub estación)
				//Para la DME touch enviamos las peticiones del modo continuo al dspic33 en vez de a la estación
			}
				else if (Command != (byte)EnumCommandFrame_02_FE.M_HS &
					Command != (byte)EnumCommandFrame_02_FE.M_NACK &
					Command != (byte)EnumCommandFrame_02_FE.M_FIRMWARE)
					{
					return ;
			}
			
			
			//
			//Command
			//
			if (Command == (byte)EnumCommandFrame_02_FE.M_HS)
			{
				//Si se recibe una trama de handshake de otra dirección, es posible que se trate de otra estación
				if (Origen != m_StationNumDevice)
				{
					ReadDeviceVersions(Origen);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_EOT)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_ACK)
			{
				if (EndedTransactionEvent != null)
					EndedTransactionEvent(Numstream);
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_NACK)
			{
				Debug.Print("NACK Error: {0} Sent opcode: H{1:X2}", (Datos[0]).ToString(), Datos[1]);
				if (NACKTransactionEvent != null)
					NACKTransactionEvent(Numstream);
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_SYN)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_DEVICEID)
			{
				clsStationUID stationUUID = new clsStationUID(Datos);
				m_StationData.Info.UUID = stationUUID.UID;
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_W_DEVICEID)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_RESET)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_FIRMWARE)
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
			else if (Command == (byte)EnumCommandFrame_02_FE.M_CLEARMEMFLASH)
			{
				if (ClearingFlashFinishedEvent != null)
					ClearingFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_SENDMEMADDRESS)
			{
				if (AddressMemoryFlashFinishedEvent != null)
					AddressMemoryFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_SENDMEMDATA)
			{
				if (DataMemoryFlashFinishedEvent != null)
					DataMemoryFlashFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_ENDPROGR)
			{
				if (EndProgFinishedEvent != null)
					EndProgFinishedEvent();
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_SUCTIONLEVEL)
			{
				if (Datos.Length != 1)
				{
					return ;
				}
				
				//valor común para todos los puertos
				for (int i = 0; i <= m_PortData.Length - 1; i++)
				{
					m_PortData[i].ToolStatus.SuctionLevel = (SuctionLevel_FE) (Datos[0]);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_W_SUCTIONLEVEL)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_FLOW)
			{
				if (Datos.Length != 2)
				{
					return ;
				}
				
				//valor común para todos los puertos
				for (int i = 0; i <= m_PortData.Length - 1; i++)
				{
					m_PortData[i].ToolStatus.Flow = BitConverter.ToUInt16(Datos, 0);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_SPEED)
			{
				if (Datos.Length != 2)
				{
					return ;
				}
				
				//valor común para todos los puertos
				for (int i = 0; i <= m_PortData.Length - 1; i++)
				{
					m_PortData[i].ToolStatus.Speed = BitConverter.ToUInt16(Datos, 0);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_SELECTFLOW)
			{
				if (Datos.Length != 2)
				{
					return ;
				}
				
				//valor común para todos los puertos
				for (int i = 0; i <= m_PortData.Length - 1; i++)
				{
					m_PortData[i].ToolStatus.SelectedFlow_x_Mil = BitConverter.ToUInt16(Datos, 0);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_W_SELECTFLOW)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_STANDINTAKES)
			{
				if (Datos.Length != 1)
				{
					return ;
				}
				
				m_PortData[Puerto].ToolStatus.StandIntakes = Datos[0];
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_W_STANDINTAKES)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_INTAKEACTIVATION)
			{
				if (Datos.Length != 1)
				{
					return ;
				}
				
				if (Intake == (int) ToolStatus_FE.WORK)
				{
					m_PortData[Puerto].ToolStatus.IntakeActivationWork = (OnOff) (Datos[0]);
					
				}
				else if (Intake == (int) ToolStatus_FE.STAND)
				{
					m_PortData[Puerto].ToolStatus.IntakeActivationStand = (OnOff) (Datos[0]);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_W_INTAKEACTIVATION)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_SUCTIONDELAY)
			{
				if (Datos.Length != 4)
				{
					return ;
				}
				
				if (Intake == (int) ToolStatus_FE.WORK)
				{
					m_PortData[Puerto].ToolStatus.SuctionDelayWork = BitConverter.ToUInt16(Datos, 0);
					
				}
				else if (Intake == (int) ToolStatus_FE.STAND)
				{
					m_PortData[Puerto].ToolStatus.SuctionDelayStand = BitConverter.ToUInt16(Datos, 0);
				}
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_W_SUCTIONDELAY)
			{
			}
			else if (Command == (byte)EnumCommandFrame_02_FE.M_R_DELAYTIME)
			{
				if (Datos.Length != 4)
				{
					return ;
				}
				
				if (Intake == (int) ToolStatus_FE.WORK)
				{
					m_PortData[Puerto].ToolStatus.TimeToStopSuctionWork = BitConverter.ToUInt16(Datos, 0);
					
				}
				else if (Intake == (int) ToolStatus_FE.STAND)
				{
					m_PortData[Puerto].ToolStatus.TimeToStopSuctionStand = BitConverter.ToUInt16(Datos, 0);
				}
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_ACTIVATIONPEDAL)
			{
				if (Datos.Length != 2)
				{
					return ;
				}
				
				if (Datos[0] == 0)
				{
					m_PortData[Puerto].ToolStatus.PedalActivation = PedalAction.HOLD_DOWN;
				}
				else if (Datos[0] == 1)
				{
					m_PortData[Puerto].ToolStatus.PedalActivation = PedalAction.PULSE;
				}
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_ACTIVATIONPEDAL)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_PEDALMODE)
			{
				if (Datos.Length != 2)
				{
					return ;
				}
				
				m_PortData[Puerto].ToolStatus.PedalMode = (PedalMode) (Datos[0]);
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_PEDALMODE)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_FILTERSTATUS)
			{
				if (Datos.Length != 2)
				{
					return ;
				}
				
				//valor común para todos los puertos
				for (int i = 0; i <= m_PortData.Length - 1; i++)
				{
					m_PortData[i].ToolStatus.FilterStatus = BitConverter.ToUInt16(Datos, 0);
				}
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_RESETFILTER)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_RESETSTATION)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_PIN)
			{
				if (Datos.Length != 4)
				{
					return ;
				}
				
				m_StationData.Settings.PIN = Encoding.UTF8.GetString(Datos);
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_PIN)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_BEEP)
			{
				if (Datos.Length != 1)
				{
					return ;
				}
				
				m_StationData.Settings.Beep = (OnOff) (Datos[0]);
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_BEEP)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_CONTINUOUSSUCTION)
			{
				if (Datos.Length != 1)
				{
					return ;
				}
				
				m_StationData.Settings.ContinuousSuction = (OnOff) (Datos[0]);
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_CONTINUOUSSUCTION)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_STATERROR)
			{
				if (Datos.Length != 2)
				{
					return ;
				}
				
				m_StationData.Status.ErrorStation = (StationError) (BitConverter.ToInt16(Datos, 0));
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_DEVICENAME)
			{
				string TextoName = Encoding.UTF8.GetString(Datos);
				m_StationData.Settings.Name = TextoName;
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_DEVICENAME)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_COUNTERS)
			{
				if (Datos.Length != 40)
				{
					return ;
				}
				
				m_PortData[0].Counters.ContPlugMinutes = BitConverter.ToInt32(Datos, 0);
				m_PortData[1].Counters.ContPlugMinutes = BitConverter.ToInt32(Datos, 4);
				m_PortData[0].Counters.ContIdleMinutes = BitConverter.ToInt32(Datos, 8);
				m_PortData[1].Counters.ContIdleMinutes = BitConverter.ToInt32(Datos, 12);
				m_PortData[0].Counters.ContWorkIntakeMinutes = BitConverter.ToInt32(Datos, 16);
				m_PortData[1].Counters.ContWorkIntakeMinutes = BitConverter.ToInt32(Datos, 20);
				m_PortData[0].Counters.ContStandIntakeMinutes = BitConverter.ToInt32(Datos, 24);
				m_PortData[1].Counters.ContStandIntakeMinutes = BitConverter.ToInt32(Datos, 28);
				m_PortData[0].Counters.ContWorkCycles = BitConverter.ToInt32(Datos, 32);
				m_PortData[1].Counters.ContWorkCycles = BitConverter.ToInt32(Datos, 36);
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_RESETCOUNTERS)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_COUNTERSP)
			{
				if (Datos.Length != 40)
				{
					return ;
				}
				
				m_PortData[0].PartialCounters.ContPlugMinutes = BitConverter.ToInt32(Datos, 0);
				m_PortData[1].PartialCounters.ContPlugMinutes = BitConverter.ToInt32(Datos, 4);
				m_PortData[0].PartialCounters.ContIdleMinutes = BitConverter.ToInt32(Datos, 8);
				m_PortData[1].PartialCounters.ContIdleMinutes = BitConverter.ToInt32(Datos, 12);
				m_PortData[0].PartialCounters.ContWorkIntakeMinutes = BitConverter.ToInt32(Datos, 16);
				m_PortData[1].PartialCounters.ContWorkIntakeMinutes = BitConverter.ToInt32(Datos, 20);
				m_PortData[0].PartialCounters.ContStandIntakeMinutes = BitConverter.ToInt32(Datos, 24);
				m_PortData[1].PartialCounters.ContStandIntakeMinutes = BitConverter.ToInt32(Datos, 28);
				m_PortData[0].PartialCounters.ContWorkCycles = BitConverter.ToInt32(Datos, 32);
				m_PortData[1].PartialCounters.ContWorkCycles = BitConverter.ToInt32(Datos, 36);
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_RESETCOUNTERSP)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_USB_CONNECTSTATUS)
			{
				if (Datos.Length == 0)
				{
					return ;
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
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_USB_CONNECTSTATUS)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_RBT_CONNCONFIG)
			{
				if (Datos.Length != 7)
				{
					return ;
				}
				
				sDatos = Encoding.UTF8.GetString(Datos);
				
				m_StationData.Settings.Robot.Speed = ((CRobotData.RobotSpeed) (Datos[0]));
				
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
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_RBT_CONNCONFIG)
			{
			}
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_R_RBT_CONNECTSTATUS)
			{
				if (Datos.Length != 1)
				{
					return ;
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
			else if (Command ==(byte)EnumCommandFrame_02_FE.M_W_RBT_CONNECTSTATUS)
			{
			}
			else
			{
				//No hacer nada
			}
			
		}
		
#endregion
		
	}
	
}
