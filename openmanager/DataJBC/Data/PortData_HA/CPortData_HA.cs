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
    /// Provides status information and configuration of the port
    /// </summary>
    /// <remarks>The port is a hot air station</remarks>
    internal class CPortData_HA
    {

        //Port status
        public CToolStatusData_HA ToolStatus = new CToolStatusData_HA();

        //Configured parameters
        public CToolSettingsData_HA[] ToolSettings;

        //Counters
        public CCountersData_HA Counters = new CCountersData_HA();
        public CCountersData_HA PartialCounters = new CCountersData_HA();


        public CPortData_HA(int NumTool, int NumLevels)
        {
            ToolSettings = new CToolSettingsData_HA[NumTool - 1 + 1];
            for (int index = 0; index <= NumTool - 1; index++)
            {
                ToolSettings[index] = new CToolSettingsData_HA(NumLevels);
            }
        }

    }
}
