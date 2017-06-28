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
    internal class CTempLevelsData_HA : ICloneable
    {

        public OnOff LevelsOnOff;
        public ToolTemperatureLevels LevelsTempSelect;
        public CTemperature[] LevelsTemp;
        public int[] LevelsFlow;
        public CTemperature[] LevelsExtTemp;
        public OnOff[] LevelsTempOnOff;


        public CTempLevelsData_HA(int NumLevels)
        {
            LevelsOnOff = OnOff._OFF;
            LevelsTempSelect = (ToolTemperatureLevels)0;
            LevelsTemp = new CTemperature[NumLevels - 1 + 1];
            LevelsFlow = new int[NumLevels - 1 + 1];
            LevelsExtTemp = new CTemperature[NumLevels - 1 + 1];
            LevelsTempOnOff = new OnOff[NumLevels - 1 + 1];
            for (int index = 0; index <= NumLevels - 1; index++)
            {
                LevelsTemp[index] = new CTemperature();
                LevelsExtTemp[index] = new CTemperature();
                LevelsFlow[index] = 0;
                LevelsTempOnOff[index] = OnOff._OFF;
            }
        }

        public int NumLevels()
        {
            return LevelsTemp.Length;
        }

        public dynamic Clone()
        {
            CTempLevelsData_HA cls_TempLevels_Clonado = new CTempLevelsData_HA(NumLevels());
            cls_TempLevels_Clonado.LevelsOnOff = this.LevelsOnOff;
            cls_TempLevels_Clonado.LevelsTempSelect = this.LevelsTempSelect;
            for (int index = 0; index <= NumLevels() - 1; index++)
            {
                cls_TempLevels_Clonado.LevelsTemp[index] = this.LevelsTemp[index];
                cls_TempLevels_Clonado.LevelsExtTemp[index] = this.LevelsExtTemp[index];
                cls_TempLevels_Clonado.LevelsFlow[index] = this.LevelsFlow[index];
                cls_TempLevels_Clonado.LevelsTempOnOff[index] = this.LevelsTempOnOff[index];
            }

            return cls_TempLevels_Clonado;
        }

    }
}
