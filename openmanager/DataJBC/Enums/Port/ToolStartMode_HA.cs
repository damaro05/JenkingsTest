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
    /// Tool start modes
    /// </summary>
    /// <remarks></remarks>
    [FlagsAttribute()]
    public enum ToolStartMode_HA : byte
    {
        NONE = 0,
        TOOL_BUTTON = 1,
        STAND_OUT = 2,
        PEDAL_PULSE = 4,
        PEDAL_HOLD_DOWN = 8
    }
}

