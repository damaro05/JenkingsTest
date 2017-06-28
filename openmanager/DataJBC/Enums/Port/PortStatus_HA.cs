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
    /// List of work mode in Desolder stations
    /// </summary>
    /// <remarks></remarks>
    internal enum PortWorkMode_HA : byte
    {
        MANUAL = 0,
        PROFILE = 1
    }

    /// <summary>
    /// List of heater status in Desolder stations
    /// </summary>
    /// <remarks></remarks>
    internal enum PortHeaterStatus_HA : byte
    {
        HEATER_OFF = 0,
        HEATER_ON = 1,
        COOLING = 2
    }

    /// <summary>
    /// List of suction status in Desolder stations
    /// </summary>
    /// <remarks></remarks>
    internal enum PortSuctionStatus_HA : byte
    {
        SUCTION_OFF = 0,
        SUCTION_ON = 1
    }
}
