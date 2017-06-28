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


    public class CConvertFirmwareVersionFromDC
    {

        public static void CopyData(CFirmwareStation firmwareVersion,
                dc_FirmwareStation dcFirmwareVersion)
        {

            firmwareVersion.StationUUID = dcFirmwareVersion.stationUUID;
            firmwareVersion.Model = dcFirmwareVersion.model;
            firmwareVersion.ModelVersion = dcFirmwareVersion.modelVersion;
            firmwareVersion.ProtocolVersion = dcFirmwareVersion.protocolVersion;
            firmwareVersion.HardwareVersion = dcFirmwareVersion.hardwareVersion;
            firmwareVersion.SoftwareVersion = dcFirmwareVersion.softwareVersion;
            firmwareVersion.FileName = dcFirmwareVersion.fileName;
        }

    }

}
