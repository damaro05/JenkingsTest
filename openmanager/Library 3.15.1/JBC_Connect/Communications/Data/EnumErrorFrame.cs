// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBC_Connect
{
    public enum EnumErrorFrame : int
    {
        ErrorType = 0,
        Opcode = 1,
        Port = 2,
        Tool = 3,
        Sequence = 4
    }
}
