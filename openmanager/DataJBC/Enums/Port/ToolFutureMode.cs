// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{

    /// <summary>
    /// List of station modes.
    /// </summary>
    /// <remarks></remarks>
    public enum ToolFutureMode : byte
    {
        Sleep = 0x53, //'S'
        Hibernation = 0x48, //'H'
        NoFutureMode = 0x4E //'N'
    }
}

