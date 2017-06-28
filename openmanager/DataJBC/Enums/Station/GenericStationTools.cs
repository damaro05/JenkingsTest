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
    /// List of the diferent tools available by a generic station
    /// </summary>
    /// <remarks></remarks>
    public enum GenericStationTools : byte
    {
        NO_TOOL = 0,
        T210 = 1,
        T245 = 2,
        PA = 3,
        HT = 4,
        DS = 5,
        DR = 6,
        NT105 = 7, // 07/01/2014 edu ' 01/11/2014 Se cambia NT205 por NT105
        NP105 = 8, // 07/01/2014 edu
                   // Desoldadoras (se usa GetGenericToolFromInternal y GetInternalToolFromGeneric para enviar y recibir de la estación)
        JT = 31,
        TE = 32,
        PHS = 33,
        PHB = 34
    }
}

