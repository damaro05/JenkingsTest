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
    /// List of possible tool errors
    /// </summary>
    /// <remarks></remarks>
    public enum ToolError : byte
    {
        // soldering station
        NO_ERROR = 0,
        SHORTCIRCUIT = 1,
        SHORTCIRCUIT_NR = 2,
        OPENCIRCUIT = 3,
        NO_TOOL = 4,
        WRONGTOOL = 5,
        DETECTIONTOOL = 6,
        MAXPOWER = 7,
        STOPOVERLOAD_MOS = 8,
        // HA desoldering station (internal code is code - 20)
        AIR_PUMP_ERROR = 21,
        PROTECION_TC_HIGH = 22,
        REGULATION_TC_HIGH = 23,
        EXTERNAL_TC_MISSING = 24,
        SELECTED_TEMP_NOT_REACHED = 25,
        HIGH_HEATER_INTENSITY = 26,
        LOW_HEATER_RESISTANCE = 27,
        WRONG_HEATER = 28,
        NOTOOL_HA = 29,
        DETECTIONTOOL_HA = 30
    }
}

