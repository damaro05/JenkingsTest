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
    public enum EnumFrameFlowControl : int
    {
        ONE_TO_ONE, // envÃ­a un frame y espera respuesta
        BURST // envÃ­a varios frames
    }
}
