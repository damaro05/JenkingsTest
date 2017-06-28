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
    /// Dispenser mode
    /// </summary>
    /// <remarks></remarks>
    [FlagsAttribute()]
    public enum DispenserMode_SF : byte
    {
        CONTINUOUS = 1,
        DISCONTINUOUS = 2,
        PROGRAM = 3,
        CONCATENATION = 4
    }
}

