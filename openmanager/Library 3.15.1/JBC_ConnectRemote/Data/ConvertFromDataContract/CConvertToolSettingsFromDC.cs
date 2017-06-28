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


    internal class CConvertToolSettingsFromDC
    {

        public static void CopyData(CToolSettingsData_SOLD toolSettings,
                CPortData_SOLD infoPort,
                dc_PortToolSettings dcToolSettings)
        {

            toolSettings.Tool = (GenericStationTools) dcToolSettings.Tool;
            toolSettings.AdjustTemp.UTI = dcToolSettings.AdjustTemp.UTI;
            CConvertCartridgeFromDC.CopyData(toolSettings.Cartridge, dcToolSettings.Cartridge);
            toolSettings.FixedTemp = new CTemperature(dcToolSettings.FixedTemp.UTI);
            toolSettings.FixedTemp_OnOff = (DataJBC.OnOff) dcToolSettings.FixedTemp_OnOff;
            CConvertTempLevelsFromDC.CopyData(toolSettings.Levels, dcToolSettings.Levels);
            toolSettings.SleepTemp = new CTemperature(dcToolSettings.SleepTemp.UTI);
            toolSettings.SleepTime = (ToolTimeSleep) ((ToolTimeSleep) dcToolSettings.SleepTime);
            toolSettings.SleepTimeOnOff = (DataJBC.OnOff) dcToolSettings.SleepTimeOnOff;
            toolSettings.HiberTime = (ToolTimeHibernation) ((ToolTimeHibernation) dcToolSettings.HiberTime);
            toolSettings.HiberTimeOnOff = (DataJBC.OnOff) dcToolSettings.HiberTimeOnOff;
        }

        public static void CopyData_HA(CToolSettingsData_HA toolSettings,
                CPortData_HA infoPort,
                dc_PortToolSettings_HA dcToolSettings)
        {

            toolSettings.Tool = (GenericStationTools)dcToolSettings.Tool;
            CConvertTempLevelsFromDC.CopyData_HA(toolSettings.Levels, dcToolSettings.Levels);
            toolSettings.AdjustTemp.UTI = dcToolSettings.AdjustTemp.UTI;
            toolSettings.TimeToStop = dcToolSettings.TimeToStop;
            toolSettings.ExternalTCMode = (ToolExternalTCMode_HA) dcToolSettings.ExternalTCMode;
            toolSettings.StartMode_ToolButton = (DataJBC.OnOff) dcToolSettings.StartMode_ToolButton;
            toolSettings.StartMode_Pedal = (PedalAction) dcToolSettings.StartMode_Pedal;

            infoPort.ToolStatus.ProfileMode = (DataJBC.OnOff)dcToolSettings.PortProfileMode;
        }

    }

}
