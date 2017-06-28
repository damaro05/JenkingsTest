// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{

	/// <summary>
	/// List of posible tool status
	/// </summary>
	/// <remarks></remarks>
	[FlagsAttribute()]public enum ToolStatus
	{
		//DESOLDER = 8
		//EXTRACTOR = 4
		//HIBERNATION = 2
		//SLEEP = 1
		//NONE = 0
		DESOLDER = 16,
		EXTRACTOR = 8,
		HIBERNATION = 4,
		SLEEP = 2,
		STAND = 1,
		NONE = 0
			// bits en protocolo 01:  Desoldador:3 Extractor:2 HibernaciÃ³n:1 Sleep:0
			// bits en protocolo 02:  Desoldador:4 Extractor:3 HibernaciÃ³n:2 Sleep:1 Stand:0
	}

	/// <summary>
	/// List of posible tool status for desolder station
	/// </summary>
	/// <remarks></remarks>
	[FlagsAttribute()]public enum ToolStatus_HA
	{
		//Bit 7	128 Tool in stand		        (1:en el stand / 0:fuera del stand)
		//Bit 6	64 Pedal status		            (1:pulsado / 0:no pulsado)
		//Bit 5	32 Pedal conectado 		        (1:Pedal conectado / 0:Sin pedal)
		//Bit 4	16 Suction status solicitado 	(1:ON / 0:OFF)
		//Bit 3	8 Suction status real 	        (1:ON / 0:OFF)
		//Bit 2	4 Cooling 			            (1:ON / 0:OFF)
		//Bit 1	2 Heater status solicitado 	    (1:ON / 0:OFF)
		//Bit 0	1 Heater status real 	        (1:ON / 0:OFF)
		STAND = 128,
		PEDAL_PRESSED = 64,
		PEDAL_CONNECTED = 32,
		SUCTION_REQUESTED = 16,
		SUCTION = 8,
		COOLING = 4,
		HEATER_REQUESTED = 2,
		HEATER = 1,
		NONE = 0
	}
		
	/// <summary>
	/// List of posible tool status for fume extractor station
	/// </summary>
	/// <remarks></remarks>
	[FlagsAttribute()]
		public enum ToolStatus_FE
		{
			STAND = 1,
			WORK = 0
		}

}

