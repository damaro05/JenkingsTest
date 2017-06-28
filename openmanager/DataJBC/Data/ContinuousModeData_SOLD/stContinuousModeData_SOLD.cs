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
    /// Defines the data returned by the continuous mode transmisions
    /// </summary>
    /// <remarks></remarks>
    public struct stContinuousModeData_SOLD
    {
        public stContinuousModePort_SOLD[] data;
        public ulong sequence;
    }
}
