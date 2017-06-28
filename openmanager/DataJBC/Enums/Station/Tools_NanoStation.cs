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

    public enum Tools_NanoStation : byte // Este tipo enumerado es el que luego se usa en la enumeraciÃ³n Station
    {
        NOTOOL = 0,
        NT105 = 1, // Nano Soldador ' 01/11/2014 Se cambia NT205 por NT105
        NP105 = 3 // Nano Pinza
    }
}
