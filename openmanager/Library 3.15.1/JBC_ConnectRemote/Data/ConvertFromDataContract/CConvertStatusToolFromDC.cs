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
using OnOff = DataJBC.OnOff;
// End of VB project level imports


namespace JBC_ConnectRemote
{


    internal class CConvertStatusToolFromDC
    {

        public static void CopyData(CToolStatusData_SOLD statusTool,
                CPortData_SOLD infoPort,
                dc_StatusTool dcStatusTool)
        {

            statusTool.ConnectedTool = (GenericStationTools) dcStatusTool.ConnectedTool;
            statusTool.ActualTemp[0].UTI = dcStatusTool.ActualTemp.UTI;
            statusTool.Current_mA[0] = 0; // FALTA saber si se usa
            statusTool.Desold_OnOff = (OnOff) dcStatusTool.Desold_OnOff;
            statusTool.Extractor_OnOff = (OnOff) dcStatusTool.Extractor_OnOff;

            // servicio: S (sleep) or H (hibernation)
            switch (dcStatusTool.FutureMode)
            {
                case "S":
                    statusTool.FutureMode_Tool = ToolFutureMode.Sleep;
                    break;
                case "H":
                    statusTool.FutureMode_Tool = ToolFutureMode.Hibernation;
                    break;
                default:
                    statusTool.FutureMode_Tool = ToolFutureMode.NoFutureMode;
                    break;
            }

            statusTool.Hiber_OnOff = (OnOff) dcStatusTool.Hiber_OnOff;

            statusTool.Peripherals = new CPeripheralData[dcStatusTool.Peripheral.Count()];
            for (var i = 0; i <= dcStatusTool.Peripheral.Count() - 1; i++)
            {
                statusTool.Peripherals[(int) i] = new CPeripheralData(statusTool.Peripherals.Count());
                CConvertPeripheralFromDC.CopyData(statusTool.Peripherals[(int) i], dcStatusTool.Peripheral[i]);
            }

            statusTool.Power_x_Mil[0] = dcStatusTool.Power_x_Mil;
            statusTool.Sleep_OnOff = (OnOff) dcStatusTool.Sleep_OnOff;
            statusTool.Stand_OnOff = (OnOff) dcStatusTool.Stand_OnOff;
            statusTool.StatusRemoteMode.Sleep_OnOff = (OnOff) dcStatusTool.StatusRemoteMode_Sleep_OnOff;
            statusTool.StatusRemoteMode.Desold_OnOff = (OnOff) dcStatusTool.StatusRemoteMode_Desold_OnOff;
            statusTool.StatusRemoteMode.Extractor_OnOff = (OnOff) dcStatusTool.StatusRemoteMode_Extractor_OnOff;
            statusTool.Temp_MOS.UTI = dcStatusTool.Temp_MOS.UTI;
            statusTool.TimeToSleepHibern = dcStatusTool.TimeToSleepHibern;
            statusTool.ToolError = (ToolError) dcStatusTool.ToolError;

            infoPort.ToolStatus.SelectedTemp.UTI = dcStatusTool.PortSelectedTemp.UTI;
        }

        public static void CopyData_HA(CToolStatusData_HA statusTool,
                CPortData_HA infoPort,
                dc_StatusTool_HA dcStatusTool)
        {


            statusTool.ConnectedTool = (GenericStationTools) dcStatusTool.ConnectedTool;
            statusTool.ActualTemp.UTI = dcStatusTool.ActualTemp.UTI;
            statusTool.ActualExtTemp.UTI = dcStatusTool.ActualExtTemp.UTI;
            statusTool.ProtectionTC_Temp.UTI = dcStatusTool.ProtectionTC_Temp.UTI;
            statusTool.Power_x_Mil = dcStatusTool.Power_x_Mil;
            statusTool.Flow_x_Mil = dcStatusTool.Flow_x_Mil;
            statusTool.TimeToStop = dcStatusTool.TimeToStop;

            statusTool.Stand_OnOff = (OnOff) dcStatusTool.Stand_OnOff;
            statusTool.PedalStatus_OnOff = (OnOff) dcStatusTool.PedalStatus_OnOff;
            statusTool.PedalConnected_OnOff = (OnOff) dcStatusTool.PedalConnected_OnOff;
            statusTool.SuctionRequestedStatus_OnOff = (OnOff) dcStatusTool.SuctionRequestedStatus_OnOff;
            statusTool.SuctionStatus_OnOff = (OnOff) dcStatusTool.SuctionStatus_OnOff;
            statusTool.HeaterRequestedStatus_OnOff = (OnOff) dcStatusTool.HeaterRequestedStatus_OnOff;
            statusTool.HeaterStatus_OnOff = (OnOff) dcStatusTool.HeaterStatus_OnOff;
            statusTool.CoolingStatus_OnOff = (OnOff) dcStatusTool.CoolingStatus_OnOff;
            statusTool.ToolError = (ToolError) dcStatusTool.ToolError;

            // port selected temps/flow
            infoPort.ToolStatus.SelectedTemp.UTI = dcStatusTool.PortSelectedTemp.UTI;
            infoPort.ToolStatus.SelectedExtTemp.UTI = dcStatusTool.PortSelectedExtTemp.UTI;
            infoPort.ToolStatus.SelectedFlow_x_Mil = dcStatusTool.PortSelectedFlow_x_mil;
        }

        public static void CopyData_SF(CPortData_SF infoPort, JBCService.dc_StatusTool_SF dcStatusTool)
        {

            infoPort.ToolStatus.EnabledPort = (OnOff) dcStatusTool.EnabledPort;
            infoPort.ToolStatus.DispenserMode = (DispenserMode_SF) dcStatusTool.DispenserMode;
            infoPort.ToolStatus.Speed.InMillimetersPerSecond(dcStatusTool.Speed.MillimetersPerSecond);
            infoPort.ToolStatus.Length.InMillimeters(dcStatusTool.Length.Millimeters);
            infoPort.ToolStatus.FeedingState = (OnOff) dcStatusTool.FeedingState;
            infoPort.ToolStatus.FeedingPercent = dcStatusTool.FeedingPercent;
            infoPort.ToolStatus.FeedingLength.InMillimeters(dcStatusTool.FeedingLength.Millimeters);
            infoPort.ToolStatus.CurrentProgramStep = dcStatusTool.CurrentProgramStep;
        }

    }

}
