// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Runtime.Serialization;

namespace JBC_Connect
{
	public class dc_EnumConstJBC
	{

		[DataContract()]public enum dc_StationType
		{
			[EnumMember()]UNKNOWN = 0,
				[EnumMember()]SOLD = 1,
				[EnumMember()]HA = 2,
				[EnumMember()]SF = 3
		}

		[DataContract()]public enum dc_StationControllerConnType
		{
			[EnumMember()]USB = 0,
				[EnumMember()]ETH = 1
		}

		[DataContract()]public enum dc_StationControllerAction
		{
			[EnumMember()]StartSearch = 0,
				[EnumMember()]StopSearch = 1,
				[EnumMember()]InitialSearchOn = 2,
				[EnumMember()]InitialSearchOff = 3
		}

		[DataContract()]public enum dc_Port
		{
			[EnumMember()]NUM_1 = 0,
				[EnumMember()]NUM_2 = 1,
				[EnumMember()]NUM_3 = 2,
				[EnumMember()]NUM_4 = 3,
				[EnumMember()]NO_PORT = -1
		}

		[DataContract()]public enum dc_OnOff
		{
			[EnumMember()]_OFF = 0,
				[EnumMember()]_ON = 1
		}

		[DataContract()]public enum dc_FaultError
		{
			[EnumMember()]NoError = 0,
				[EnumMember()]NotControlledError = 1,
				[EnumMember()]StationNotFound = 2,
				[EnumMember()]StationError = 3,
				[EnumMember()]ToolError = 4,
				[EnumMember()]NotValidTool = 5,
				[EnumMember()]FunctionNotSupported = 6
		}

		[DataContract()]public enum dc_StationError
		{
			[EnumMember()]OK = 0,
				[EnumMember()]STOPOVERLOAD_TRAFO = 1, // sold
				[EnumMember()]WRONGSENSOR_TRAFO = 2, // sold
				[EnumMember()]MEMORY = 3,
				[EnumMember()]MAINSFREQUENCY = 4,
				[EnumMember()]STATION_MODEL = 5, // desold
				[EnumMember()]NOT_MCU_TOOLS = 6 // desold
		}

		[DataContract()]public enum dc_ToolStatus
		{
			[EnumMember()]DESOLDER = 16,
				[EnumMember()]HIBERNATION = 4,
				[EnumMember()]EXTRACTOR = 8,
				[EnumMember()]SLEEP = 2,
				[EnumMember()]STAND = 1,
				[EnumMember()]NONE = 0
		}

		// Desol
		[DataContract()]public enum dc_ToolStatus_HA
		{
			[EnumMember()]STAND = 128,
				[EnumMember()]PEDAL_PRESSED = 64,
				[EnumMember()]PEDAL_CONNECTED = 32,
				[EnumMember()]SUCTION_REQUESTED = 16,
				[EnumMember()]SUCTION = 8,
				[EnumMember()]COOLING = 4,
				[EnumMember()]HEATER_REQUESTED = 2,
				[EnumMember()]HEATER = 1,
				[EnumMember()]NONE = 0
		}

		[DataContract()]public enum dc_GenericStationTools
		{
			[EnumMember()]NOTOOL = 0,
				[EnumMember()]T210 = 1,
				[EnumMember()]T245 = 2,
				[EnumMember()]PA = 3,
				[EnumMember()]HT = 4,
				[EnumMember()]DS = 5,
				[EnumMember()]DR = 6,
				[EnumMember()]NT105 = 7, // 01/11/2014 Se cambia NT205 por NT105
				[EnumMember()]NP105 = 8,
				// Desold (en la estación son 1, 2, 3, ..., en la DLL local se usa GetGenericToolFromInternal y GetInternalToolFromGeneric para enviar y recibir de la estación))
				[EnumMember()]JT = 31,
				[EnumMember()]TE = 32,
				[EnumMember()]PHS = 33,
				[EnumMember()]PHB = 34
		}

		[DataContract()]public enum dc_ToolError
		{
			[EnumMember()]OK = 0,
				[EnumMember()]SHORTCIRCUIT = 1,
				[EnumMember()]SHORTCIRCUIT_NR = 2,
				[EnumMember()]OPENCIRCUIT = 3,
				[EnumMember()]NOTOOL = 4,
				[EnumMember()]WRONGTOOL = 5,
				[EnumMember()]DETECTIONTOOL = 6,
				[EnumMember()]MAXPOWER = 7,
				[EnumMember()]STOPOVERLOAD_MOS = 8,
				// desol
				[EnumMember()]AIR_PUMP_ERROR = 21,
				[EnumMember()]PROTECION_TC_HIGH = 22,
				[EnumMember()]REGULATION_TC_HIGH = 23,
				[EnumMember()]EXTERNAL_TC_MISSING = 24,
				[EnumMember()]SELECTED_TEMP_NOT_REACHED = 25,
				[EnumMember()]HIGH_HEATER_INTENSITY = 26,
				[EnumMember()]LOW_HEATER_RESISTANCE = 27,
				[EnumMember()]WRONG_HEATER = 28,
				[EnumMember()]NOTOOL_HA = 29,
				[EnumMember()]DETECTIONTOOL_HA = 30
		}

		// Desol HA
		[DataContract()]
			public enum dc_PortProfileMode
			{
				[EnumMember()]
					MANUAL = 0,
				[EnumMember()]
					PROFILE = 1
			}

		// Desol HA
		[DataContract()]
			public enum dc_PedalAction
			{
				[EnumMember()]
					NONE = 0,
				[EnumMember()]
					PULSE = 1,
				[EnumMember()]
					HOLD_DOWN = 2
			}

		// Desol HA
		[DataContract()]
			public enum dc_HeaterStatus_HA
			{
				[EnumMember()]
					HEATER_OFF = 0,
				[EnumMember()]
					HEATER_ON = 1,
				[EnumMember()]
					COOLING = 2
			}

		// Desol HA
		[DataContract()]
			public enum dc_SuctionStatus_HA
			{
				[EnumMember()]
					HEATER_OFF = 0,
				[EnumMember()]
					HEATER_ON = 1
			}

		//Desold HA
		[DataContract()]
			public enum dc_ExternalTCMode_HA
			{
				[EnumMember()]
					REGULATION = 0,
				[EnumMember()]
					PROTECTION = 1
			}

		[DataContract()]
			public enum dc_SpeedContinuousMode
			{
				[EnumMember()]
					OFF = 0,
				[EnumMember()]
					T_10mS = 1,
				[EnumMember()]
					T_20mS = 2,
				[EnumMember()]
					T_50mS = 3,
				[EnumMember()]
					T_100mS = 4,
				[EnumMember()]
					T_200mS = 5,
				[EnumMember()]
					T_500mS = 6,
				[EnumMember()]
					T_1000mS = 7
			}

		[DataContract()]
			public enum dc_Idioma
			{
				[EnumMember()]
					I_Ingles = 0,
				[EnumMember()]
					I_Espanol = 1,
				[EnumMember()]
					I_Frances = 2,
				[EnumMember()]
					I_Aleman = 3,
				[EnumMember()]
					I_Italiano = 4,
				// añadidos en protocolo 02
				[EnumMember()]
					I_Portugues = 5,
				[EnumMember()]
					I_Chino = 6,
				[EnumMember()]
					I_Japones = 7,
				[EnumMember()]
					I_Coreano = 8,
				[EnumMember()]
					I_Ruso = 9
			}

		[DataContract()]
			public enum dc_Lang
			{
				// Protocolo 01
				[EnumMember()]
					en = 0,
				[EnumMember()]
					es = 1,
				[EnumMember()]
					fr = 2,
				[EnumMember()]
					de = 3,
				[EnumMember()]
					it = 4,
				// añadidos en protocolo 02
				[EnumMember()]
					pt = 5,
				[EnumMember()]
					zh = 6, // chino
				[EnumMember()]
					ja = 7,
				[EnumMember()]
					ko = 8,
				[EnumMember()]
					ru = 9
			}

		[DataContract()]
			public enum dc_TimeSleep
			{
				[EnumMember()]
					MINUTE_0 = 0,
				[EnumMember()]
					MINUTE_1 = 1,
				[EnumMember()]
					MINUTE_2 = 2,
				[EnumMember()]
					MINUTE_3 = 3,
				[EnumMember()]
					MINUTE_4 = 4,
				[EnumMember()]
					MINUTE_5 = 5,
				[EnumMember()]
					MINUTE_6 = 6,
				[EnumMember()]
					MINUTE_7 = 7,
				[EnumMember()]
					MINUTE_8 = 8,
				[EnumMember()]
					MINUTE_9 = 9,
				[EnumMember()]
					NO_SLEEP = 0xFFFF
			}

		[DataContract()]
			public enum dc_TimeHibernation
			{
				[EnumMember()]
					MINUTE_0 = 0,
				[EnumMember()]
					MINUTE_5 = 5,
				[EnumMember()]
					MINUTE_10 = 10,
				[EnumMember()]
					MINUTE_15 = 15,
				[EnumMember()]
					MINUTE_20 = 20,
				[EnumMember()]
					MINUTE_25 = 25,
				[EnumMember()]
					MINUTE_30 = 30,
				[EnumMember()]
					MINUTE_35 = 35,
				[EnumMember()]
					MINUTE_40 = 40,
				[EnumMember()]
					MINUTE_45 = 45,
				[EnumMember()]
					MINUTE_50 = 50,
				[EnumMember()]
					MINUTE_55 = 55,
				[EnumMember()]
					MINUTE_60 = 60,
				[EnumMember()]
					NO_HIBERNATION = 0xFFFF
			}

		[DataContract()]
			public enum dc_ToolTemperatureLevels
			{
				[EnumMember()]
					NO_LEVELS = 0xFF,
				[EnumMember()]
					FIRST_LEVEL = 0x0,
				[EnumMember()]
					SECOND_LEVEL = 0x1,
				[EnumMember()]
					THIRD_LEVEL = 0x2
			}

		[DataContract()]
			public enum dc_UpdateState
			{
				[EnumMember()]
					Finished = 0,
				[EnumMember()]
					Updating = 1,
				[EnumMember()]
					Failed = 2
			}

		[DataContract()]
			public enum dc_RobotParity
			{
				[EnumMember()]
					NONE = 0,
				[EnumMember()]
					EVEN = 1,
				[EnumMember()]
					ODD = 2
			}

		[DataContract()]
			public enum dc_RobotProtocol
			{
				[EnumMember()]
					RS232 = 0,
				[EnumMember()]
					RS485 = 1
			}

		[DataContract()]
			public enum dc_RobotSpeed
			{
				[EnumMember()]
					BPS_1200 = 0,
				[EnumMember()]
					BPS_2400 = 1,
				[EnumMember()]
					BPS_4800 = 2,
				[EnumMember()]
					BPS_9600 = 3,
				[EnumMember()]
					BPS_19200 = 4,
				[EnumMember()]
					BPS_38400 = 5,
				[EnumMember()]
					BPS_57600 = 6,
				[EnumMember()]
					BPS_115200 = 7,
				[EnumMember()]
					BPS_230400 = 8,
				[EnumMember()]
					BPS_250000 = 9,
				[EnumMember()]
					BPS_460800 = 10,
				[EnumMember()]
					BPS_500000 = 11
			}

		[DataContract()]
			public enum dc_RobotStop
			{
				[EnumMember()]
					BITS_1 = 1,
				[EnumMember()]
					BITS_2 = 2
			}

		[DataContract()]
			public enum dc_PeripheralType
			{
				[EnumMember()]
					PD = 0,
				[EnumMember()]
					MS = 1,
				[EnumMember()]
					MN = 2,
				[EnumMember()]
					FS = 3,
				[EnumMember()]
					MV = 4,
				[EnumMember()]
					NO_TYPE = -1
			}

		[DataContract()]
			public enum dc_PeripheralFunction
			{
				[EnumMember()]
					SLEEP = 0,
				[EnumMember()]
					EXTRACTOR = 1,
				[EnumMember()]
					MODUL = 2,
				[EnumMember()]
					NO_FUNCTION = -1
			}

		[DataContract()]
			public enum dc_PeripheralActivation
			{
				[EnumMember()]
					PRESSED = 0,
				[EnumMember()]
					PULLED = 1,
				[EnumMember()]
					NO_FUNCTION = -1
			}

		[DataContract()]
			public enum dc_PeripheralStatusPD
			{
				[EnumMember()]
					SHORT_CIRCUIT = 0,
				[EnumMember()]
					OPEN_CIRCUIT = 1,
				[EnumMember()]
					OK = 2
			}

		[DataContract()]
			public enum dc_ControlModeConnection
			{
				[EnumMember()]
					MONITOR = 0,
				[EnumMember()]
					CONTROL = 1,
				[EnumMember()]
					BLOCK_CONTROL = 2,
				[EnumMember()]
					ROBOT = 3
			}

		[DataContract()]
			public enum dc_MessageType
			{
				[EnumMember()]
					INFORMATION_MESSAGE = 0,
				[EnumMember()]
					WARNING_MESSAGE = 1,
				[EnumMember()]
					ERROR_MESSAGE = 2
			}

			[DataContract()]public enum dc_LengthUnit
		{
			[EnumMember()]INCHES = 0,
				[EnumMember()]MILLIMETERS = 1
		}

		[DataContract()]public enum dc_SpeedUnit
		{
			[EnumMember()]INCHES_PER_SECOND = 0,
				[EnumMember()]MILLIMETERS_PER_SECOND = 1
		}

		[DataContract()]public enum dc_DispenserMode_SF
		{
			[EnumMember()]CONTINUOUS = 1,
				[EnumMember()]DISCONTINUOUS = 2,
				[EnumMember()]PROGRAM = 3,
				[EnumMember()]CONCATENATION = 4
		}

		[DataContract()]public enum dc_FeedingDirection_SF
		{
			[EnumMember()]FORWARD = 0,
				[EnumMember()]BACKWARD = 1
		}

	}
}

