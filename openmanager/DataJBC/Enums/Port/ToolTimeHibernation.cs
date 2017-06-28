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
    /// List of possible times before entering the Hibernation mode
    /// </summary>
    /// <remarks></remarks>
    public enum ToolTimeHibernation : ushort
    {
        MINUTE_0 = 0,
        MINUTE_5 = 5,
        MINUTE_10 = 10,
        MINUTE_15 = 15,
        MINUTE_20 = 20,
        MINUTE_25 = 25,
        MINUTE_30 = 30,
        MINUTE_35 = 35,
        MINUTE_40 = 40,
        MINUTE_45 = 45,
        MINUTE_50 = 50,
        MINUTE_55 = 55,
        MINUTE_60 = 60,
        NO_HIBERNATION = 0xFFFF // sÃ³lo protocolo 01 (en protocolo 02 hay un OnOff)
    }
}

