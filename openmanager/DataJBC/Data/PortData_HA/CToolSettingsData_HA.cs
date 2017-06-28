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
    /// <remarks>The tool is for a hot air station</remarks>
    internal class CToolSettingsData_HA : ICloneable
    {

        //Temperature configuration
        protected CTemperature m_AdjustTemp = new CTemperature();

        //Tool selected
        protected GenericStationTools m_Tool;

        //Levels data
        protected CTempLevelsData_HA m_Levels;

        //Start/stop configuration
        protected int m_TimeToStop;
        protected CToolStartMode_HA m_StartMode = new CToolStartMode_HA();

        //Ext TC
        protected ToolExternalTCMode_HA m_ExternalTCMode;


        public CToolSettingsData_HA(int NumLevels)
        {
            Initialize(NumLevels);
        }

        protected virtual void Initialize(int NumLevels)
        {
            Levels = new CTempLevelsData_HA(NumLevels);
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

        //Tool selected

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
        public CTempLevelsData_HA Levels
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

        //Start/stop configuration

        public int TimeToStop
        {
            get
            {
                return m_TimeToStop;
            }
            set
            {
                m_TimeToStop = value;
            }
        }

        public CToolStartMode_HA StartMode
        {
            get
            {
                return m_StartMode;
            }
            set
            {
                m_StartMode = value;
            }
        }

        public OnOff StartMode_ToolButton
        {
            get
            {
                return m_StartMode.ToolButton;
            }
            set
            {
                m_StartMode.ToolButton = value;
            }
        }

        public PedalAction StartMode_Pedal
        {
            get
            {
                return m_StartMode.Pedal;
            }
            set
            {
                m_StartMode.Pedal = value;
            }
        }

        //Ext TC

        public ToolExternalTCMode_HA ExternalTCMode
        {
            get
            {
                return m_ExternalTCMode;
            }
            set
            {
                m_ExternalTCMode = value;
            }
        }

        public dynamic Clone()
        {
            CToolSettingsData_HA cls_ToolSettings_Clonado = new CToolSettingsData_HA(m_Levels.NumLevels());
            cls_ToolSettings_Clonado.Levels = (CTempLevelsData_HA)(this.Levels.Clone());
            cls_ToolSettings_Clonado.AdjustTemp = (CTemperature)(this.AdjustTemp.Clone());
            cls_ToolSettings_Clonado.TimeToStop = this.TimeToStop;
            cls_ToolSettings_Clonado.StartMode = (CToolStartMode_HA)(this.StartMode.Clone());
            cls_ToolSettings_Clonado.Tool = this.Tool;
            cls_ToolSettings_Clonado.ExternalTCMode = this.ExternalTCMode;

            return cls_ToolSettings_Clonado;
        }

    }
}
