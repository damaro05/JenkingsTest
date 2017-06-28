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


    public class CConvertStationStatusFromDC
    {

        public static void CopyData(CStationStatusData_SOLD stationStatus,
                dc_Station_Sold_Status dcStationStatus)
        {

            stationStatus.ControlMode = (ControlModeConnection) dcStationStatus.ControlMode;
            stationStatus.ControlModeUserName = dcStationStatus.ControlModeUserName;
            stationStatus.ErrorStation = (StationError) dcStationStatus.StationError;
            stationStatus.TempTRAFO.UTI = dcStationStatus.TRAFOTemp.UTI;
            stationStatus.TempErrorTRAFO.UTI = dcStationStatus.TempErrorTRAFO.UTI;
            stationStatus.TempErrorMOS.UTI = dcStationStatus.TempErrorMOS.UTI;
        }

        public static void CopyData_HA(CStationStatusData_HA stationStatus,
                dc_Station_HA_Status dcStationStatus)
        {

            stationStatus.ControlMode = (ControlModeConnection) dcStationStatus.ControlMode;
            stationStatus.ControlModeUserName = dcStationStatus.ControlModeUserName;
            stationStatus.ErrorStation = (StationError) dcStationStatus.StationError;
        }

        public static void CopyData_SF(CStationStatusData_SF stationStatus, 
                dc_Station_SF_Status dcStationStatus)
        {

            stationStatus.ControlMode = (ControlModeConnection) dcStationStatus.ControlMode;
            stationStatus.ControlModeUserName = dcStationStatus.ControlModeUserName;
            stationStatus.ErrorStation = (StationError) dcStationStatus.StationError;
        }

    }

}
