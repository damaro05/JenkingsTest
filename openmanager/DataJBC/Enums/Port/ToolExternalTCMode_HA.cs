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
    /// Tool external TC mode
    /// </summary>
    /// <remarks></remarks>
    [FlagsAttribute()]
    public enum ToolExternalTCMode_HA : byte
    {
        REGULATION = 0,
        PROTECTION = 1
    }
}

