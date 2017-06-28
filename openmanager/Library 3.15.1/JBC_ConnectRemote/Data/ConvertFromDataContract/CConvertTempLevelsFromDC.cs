// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
using JBC_ConnectRemote.JBCService;
using DataJBC;

// End of VB project level imports


namespace JBC_ConnectRemote
{
    internal class CConvertTempLevelsFromDC
    {

        public static void CopyData(CTempLevelsData_SOLD levels,
                                    dc_Levels dcLevels)
        {

            for (var i = 0; i <= levels.NumLevels() - 1; i++)
            {
                levels.LevelsTemp[(int)i] = new CTemperature(dcLevels.LevelsTemp[i].UTI);
                levels.LevelsTempOnOff[(int)i] = (DataJBC.OnOff)(dcLevels.LevelsTempOnOff[i]);
            }

            levels.LevelsOnOff = (DataJBC.OnOff)dcLevels.LevelsOnOff;
            levels.LevelsTempSelect = (ToolTemperatureLevels)dcLevels.LevelsTempSelect;
        }

        public static void CopyData_HA(CTempLevelsData_HA levels,
                                       dc_Levels_HA dcLevels)
        {

            for (var i = 0; i <= levels.NumLevels() - 1; i++)
            {
                levels.LevelsTemp[(int)i] = new CTemperature(dcLevels.LevelsTemp[i].UTI);
                levels.LevelsFlow[(int)i] = dcLevels.LevelsFlow[i];
                levels.LevelsExtTemp[(int)i] = new CTemperature(dcLevels.LevelsExtTemp[i].UTI);
                levels.LevelsTempOnOff[(int)i] = (DataJBC.OnOff)(dcLevels.LevelsTempOnOff[i]);
            }
            levels.LevelsOnOff = (DataJBC.OnOff)dcLevels.LevelsOnOff;
            levels.LevelsTempSelect = (ToolTemperatureLevels)dcLevels.LevelsTempSelect;
        }

    }
}
