// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
	public sealed class Constants
	{

		/// <summary>
		/// The maximum length for the continuous mode data queue
		/// </summary>
		/// <remarks></remarks>
		public const int CONTINUOUS_MODE_QUEUE_MAX_LENGTH = 10000;

		public const int NUM_LEVELS_TEMP = 0x3; // Se definen tres niveles de temperatura por herramienta y puerto
		public const int DEFAULT_TEMP = 350 * 9; // Temperatura por defecto
		public const byte DEFAULT_SLEEP_TIME = 0; // Por defecto no tiene retardo de sleep
		public const byte DEFAULT_HIBERNATION_TIME = 30; // Por defecto 30 minutos de retardo de hibernaciÃ³n
		public const int DEFAULT_TEMP_AJUST = 50 * 9; // Por defecto 50ÂºC
		public const int NO_FIXED_TEMP = 0xFFFF; // Temperatura no fijada (protocolo 01)
		public const byte NO_LEVELS = 0xFF; // Se desactiva todos los niveles (protocolo 01)
		public const int NO_TEMP_LEVEL = 0xFFFF; // Nivel de temperatura desactivado (protocolo 01)
		public const int NO_DELAY = 0xFFFF; // no hay delay hiber porque estÃ¡ desahbilitado (protocolo 02)
		public const int NO_EXT_TC = 0xFFFF; // no hay termopar externo
		public const int MAX_TIME_TO_STOP = 50 * 60; // 50 min
		public const SpeedContinuousMode DEFAULT_STATION_CONTINUOUSMODE_SPEED = SpeedContinuousMode.T_10mS;
		public const string PROFILES_FILE_EXTENSION = "jpf";

	}
}
