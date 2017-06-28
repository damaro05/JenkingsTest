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
    /// Defines the recieved data of the continuous mode for a port.
    /// NOTE: Power units are per thousand.
    /// </summary>
    /// <remarks></remarks>
    public struct stContinuousModePort_SOLD
    {
        public Port port;
        public CTemperature temperature; //Tool temperature. Average calculated if more than one branch
        public int power; //Tool power. Average calculated if more than one branch
        public ToolStatus status;
        public OnOff desoldering;
    }
}
