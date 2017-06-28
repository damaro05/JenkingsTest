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
    /// Feeding direction
    /// </summary>
    /// <remarks></remarks>
    [FlagsAttribute()]
    public enum FeedingDirection_SF : byte
    {
        FORWARD = 0,
        BACKWARD = 1
    }
}

