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
    /// Defines the recieved data of the continuous mode for a port.
    /// NOTE: Power and flow units are per thousand.
    /// </summary>
    /// <remarks></remarks>
    public struct stContinuousModePort_HA
    {
        public Port port;
        public CTemperature temperature;
        public int flow;
        public int power;
        public CTemperature externalTC1_Temp;
        public CTemperature externalTC2_Temp;
        public int timeToStop;
        public ToolStatus_HA status;
    }
}

