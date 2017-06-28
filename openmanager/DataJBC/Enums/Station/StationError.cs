// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
// End of VB project level imports
namespace DataJBC
{

    /// <summary>
    /// List of possible station errors.
    /// </summary>
    /// <remarks></remarks>
    public enum StationError : byte
    {
        NO_ERROR = 0,
        STOPOVERLOAD_TRAFO = 1, // sold
        WRONGSENSOR_TRAFO = 2, // sold
        MEMORY = 3,
        MAINSFREQUENCY = 4,
        STATION_MODEL = 5, // desold
        NOT_MCU_TOOLS = 6 // desold
    }
}

