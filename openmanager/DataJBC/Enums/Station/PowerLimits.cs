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
    /// List of maximum and minimum station power limits. Depending on the
    /// station model the maximum is different. A value of 0 in the maximum
    /// means that there's no maximum value for the model.
    /// </summary>
    /// <remarks></remarks>
    public enum PowerLimits : int
    {
        DM_MAX = 160,
        DD_MAX = 150,
        DI_MAX = 130,
        HD_MAX = 0, // Sin lÃ­mite
        CD_CF_MAX = 130,
        CS_CV_MAX = 0, // Sin lÃ­mite, a un DS se le puede dar como mÃ¡ximo 40W
        CP_MAX = 0, // Sin lÃ­mite, a un PA se le puede dar como mÃ¡ximo 2x40W
        NA_MAX = 0, // Sin lÃ­mite, intrinsecamente da menos de 30W
        JT_MAX = 0, // Sin lÃ­mite
        MIN = 30
    }
}

