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
    /// List of continuous mode data speed.
    /// </summary>
    /// <remarks></remarks>
    public enum SpeedContinuousMode : byte
    {
        OFF = 0,
        T_10mS = 1,
        T_20mS = 2,
        T_50mS = 3,
        T_100mS = 4,
        T_200mS = 5,
        T_500mS = 6,
        T_1000mS = 7
    }
}

