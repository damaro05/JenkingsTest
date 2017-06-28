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
	/// List of temperature limits. In UTI units
	/// </summary>
	/// <remarks></remarks>
	public enum TemperatureLimits : int
	{
		MAX_TEMP = 450 * 9, //Maximum temperature
		MAX_TEMP_HD = 500 * 9, //Maximum temperature for HD
		MIN_TEMP = 90 * 9, //Minimum temperature
		MAX_TEMP_JTSE = 450 * 9, //Maximum temperature for JTSE
		MIN_TEMP_JTSE = 150 * 9, //Minimum temperature for JTSE
		MAX_EXT_TC_TEMP_JTSE = 450 * 9, //Maximum external termocouple temperature for JTSE
		MIN_EXT_TC_TEMP_JTSE = 25 * 9 //Minimum external termocouple temperature for JTSE
	}
}

