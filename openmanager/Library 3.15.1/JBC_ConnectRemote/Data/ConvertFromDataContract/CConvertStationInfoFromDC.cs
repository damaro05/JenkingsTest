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


    public class CConvertStationInfoFromDC
    {

        public static void CopyData(CStationInfoData stationInfo,
                dc_Station_Sold_Info dcStationInfo)
        {

            stationInfo.UUID = dcStationInfo.UUID;
            stationInfo.ParentUUID = dcStationInfo.ParentUUID;
            stationInfo.StationType = (eStationType) dcStationInfo.StationType;
            stationInfo.Protocol = dcStationInfo.Protocol;
            stationInfo.COM = dcStationInfo.COM;
            stationInfo.ConnectionType = dcStationInfo.ConnectionType;
            CConvertFeaturesFromDC.CopyData(stationInfo.Features, dcStationInfo.Features);

            for (var i = 0; i <= dcStationInfo.InfoUpdateFirmware.Length - 1; i++)
            {
                CFirmwareStation stationMicro = new CFirmwareStation();
                CConvertFirmwareVersionFromDC.CopyData(stationMicro, dcStationInfo.InfoUpdateFirmware[i]);
                stationInfo.StationMicros.Add(i, stationMicro);
            }

            stationInfo.Model = dcStationInfo.Model;
            stationInfo.ModelType = dcStationInfo.ModelType;
            stationInfo.ModelVersion = dcStationInfo.ModelVersion;
            stationInfo.PortCount = dcStationInfo.PortCount;

            stationInfo.SupportedTools = new GenericStationTools[dcStationInfo.SupportedTools.Length - 1 + 1];
            for (var i = 0; i <= dcStationInfo.SupportedTools.Length - 1; i++)
            {
                stationInfo.SupportedTools[(int) i] = (GenericStationTools) (dcStationInfo.SupportedTools[i]);
            }

            stationInfo.TempLevelsCount = dcStationInfo.TempLevelsCount;
            stationInfo.Version_Software = dcStationInfo.Version_Software;
            stationInfo.Version_Hardware = dcStationInfo.Version_Hardware;
        }

    }
}
