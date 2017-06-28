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
    /// <remarks>The port is soldering station</remarks>
    internal class CPortData_SOLD
    {

        //Port status
        public CToolStatusData_SOLD ToolStatus = new CToolStatusData_SOLD();

        //Configured parameters
        public CToolSettingsData_SOLD[] ToolSettings;

        //Counters
        public CCountersData_SOLD Counters = new CCountersData_SOLD();
        public CCountersData_SOLD PartialCounters = new CCountersData_SOLD();


        public CPortData_SOLD(int NumTool, int NumLevels)
        {
            ToolSettings = new CToolSettingsData_SOLD[NumTool - 1 + 1];
            for (int index = 0; index <= NumTool - 1; index++)
            {
                ToolSettings[index] = new CToolSettingsData_SOLD(NumLevels);
            }
        }

    }
}
