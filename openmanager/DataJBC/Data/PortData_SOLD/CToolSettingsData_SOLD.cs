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
    /// Provides configuration of the tool
    /// </summary>
    /// <remarks>The tool is for a soldering station</remarks>
    internal class CToolSettingsData_SOLD : ICloneable
    {

        //Temperature configuration
        protected CTemperature m_AdjustTemp = new CTemperature();
        protected CTemperature m_FixedTemp = new CTemperature();
        protected OnOff m_FixedTemp_OnOff;

        //Cartridge and tool selected
        protected CCartridgeData m_Cartridge = new CCartridgeData();
        protected GenericStationTools m_Tool;

        //Levels data
        protected CTempLevelsData_SOLD m_Levels;

        //Sleep configuration
        protected CTemperature m_SleepTemp = new CTemperature();
        protected ToolTimeSleep m_SleepTime;
        protected OnOff m_SleepTimeOnOff;

        //Hibernation configuration
        protected ToolTimeHibernation m_HiberTime;
        protected OnOff m_HiberTimeOnOff;


        public CToolSettingsData_SOLD(int NumLevels)
        {
            Initialize(NumLevels);
        }

        protected virtual void Initialize(int NumLevels)
        {
            Levels = new CTempLevelsData_SOLD(NumLevels);
        }

        //Temperature configuration

        public CTemperature AdjustTemp
        {
            get
            {
                return m_AdjustTemp;
            }
            set
            {
                m_AdjustTemp = value;
            }
        }

        public CTemperature FixedTemp
        {
            get
            {
                return m_FixedTemp;
            }
            set
            {
                m_FixedTemp = value;
            }
        }

        public OnOff FixedTemp_OnOff
        {
            get
            {
                return m_FixedTemp_OnOff;
            }
            set
            {
                m_FixedTemp_OnOff = value;
            }
        }

        //Cartridge and tool selected

        public CCartridgeData Cartridge
        {
            get
            {
                return m_Cartridge;
            }
            set
            {
                m_Cartridge = value;
            }
        }

        public GenericStationTools Tool
        {
            get
            {
                return m_Tool;
            }
            set
            {
                m_Tool = value;
            }
        }

        //Levels data

        public CTempLevelsData_SOLD Levels
        {
            get
            {
                return m_Levels;
            }
            set
            {
                m_Levels = value;
            }
        }

        //Sleep configuration

        public CTemperature SleepTemp
        {
            get
            {
                return m_SleepTemp;
            }
            set
            {
                m_SleepTemp = value;
            }
        }

        public ToolTimeSleep SleepTime
        {
            get
            {
                return m_SleepTime;
            }
            set
            {
                m_SleepTime = value;
            }
        }

        public OnOff SleepTimeOnOff
        {
            get
            {
                return m_SleepTimeOnOff;
            }
            set
            {
                m_SleepTimeOnOff = value;
            }
        }

        //Hibernation configuration

        public ToolTimeHibernation HiberTime
        {
            get
            {
                return m_HiberTime;
            }
            set
            {
                if (ToolTimeHibernation.IsDefined(typeof(ToolTimeHibernation), value))
                {
                    m_HiberTime = value;
                }
                else
                {
                    m_HiberTime = (ToolTimeHibernation)((ToolTimeHibernation)(RoutinesLibrary.Data.DataType.IntegerUtils.RoundNumber((System.Int32)value, 5, 0, 60)));
                }
            }
        }

        public OnOff HiberTimeOnOff
        {
            get
            {
                return m_HiberTimeOnOff;
            }
            set
            {
                m_HiberTimeOnOff = value;
            }
        }


        public dynamic Clone()
        {
            CToolSettingsData_SOLD cls_ToolSettings_Clonado = new CToolSettingsData_SOLD(m_Levels.NumLevels());
            cls_ToolSettings_Clonado.AdjustTemp = (CTemperature)(this.AdjustTemp.Clone());
            cls_ToolSettings_Clonado.Cartridge = (CCartridgeData)(this.Cartridge.Clone());
            cls_ToolSettings_Clonado.FixedTemp = (CTemperature)(this.FixedTemp.Clone());
            cls_ToolSettings_Clonado.FixedTemp_OnOff = this.FixedTemp_OnOff;
            cls_ToolSettings_Clonado.HiberTime = this.HiberTime;
            cls_ToolSettings_Clonado.HiberTimeOnOff = this.HiberTimeOnOff;
            cls_ToolSettings_Clonado.Levels = (CTempLevelsData_SOLD)(this.Levels.Clone());
            cls_ToolSettings_Clonado.SleepTemp = (CTemperature)(this.SleepTemp.Clone());
            cls_ToolSettings_Clonado.SleepTime = this.SleepTime;
            cls_ToolSettings_Clonado.SleepTimeOnOff = this.SleepTimeOnOff;
            cls_ToolSettings_Clonado.Tool = this.Tool;

            return cls_ToolSettings_Clonado;
        }

    }
}
