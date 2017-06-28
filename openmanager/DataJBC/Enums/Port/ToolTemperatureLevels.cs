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

    /// <summary>
    /// List of possible tool temperature levels
    /// </summary>
    /// <remarks></remarks>
    public enum ToolTemperatureLevels : byte
    {
        NO_LEVELS = 0xFF,
        FIRST_LEVEL = 0x0,
        SECOND_LEVEL = 0x1,
        THIRD_LEVEL = 0x2
    }
}

