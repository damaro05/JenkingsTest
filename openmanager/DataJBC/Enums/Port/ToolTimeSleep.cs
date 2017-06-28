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
    /// List of possible times before enter Sleep mode
    /// </summary>
    /// <remarks></remarks>
    public enum ToolTimeSleep : ushort
    {
        MINUTE_0 = 0,
        MINUTE_1 = 1,
        MINUTE_2 = 2,
        MINUTE_3 = 3,
        MINUTE_4 = 4,
        MINUTE_5 = 5,
        MINUTE_6 = 6,
        MINUTE_7 = 7,
        MINUTE_8 = 8,
        MINUTE_9 = 9,
        NO_SLEEP = 0xFFFF // sÃ³lo protocolo 01 (en protocolo 02 hay un OnOff)
    }
}

