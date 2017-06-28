// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
// End of VB project level imports

namespace DataJBC
{

    /// <summary>
    /// Station types.
    /// </summary>
    /// <remarks></remarks>
    public enum eStationType : int
    {
        UNKNOWN = 0,
        SOLD = 1, // soldering station
        HA = 2, // hot air desoldering station
        SF = 3, // tin feeder
        FE = 4 // fume extractor
    }
}

