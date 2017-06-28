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


    public class CConvertFeaturesFromDC
    {

        public static void CopyData(CFeaturesData features,
                dc_Features dcFeatures)
        {

            features.Alarms = dcFeatures.Alarms;
            features.AllToolsSamePortSettings = dcFeatures.AllToolsSamePortSettings;
            features.Cartridges = dcFeatures.Cartridges;
            features.DelayWithStatus = dcFeatures.DelayWithStatus;
            features.DisplaySettings = dcFeatures.DisplaySettings;
            features.Ethernet = dcFeatures.Ethernet;
            features.FirmwareUpdate = dcFeatures.FirmwareUpdate;
            features.MaxTemp.UTI = dcFeatures.MaxTemp.UTI;
            features.MinTemp.UTI = dcFeatures.MinTemp.UTI;
            features.ExtTCMaxTemp.UTI = dcFeatures.ExtTCMaxTemp.UTI;
            features.ExtTCMinTemp.UTI = dcFeatures.ExtTCMinTemp.UTI;
            features.MaxFlow = dcFeatures.MaxFlow;
            features.MinFlow = dcFeatures.MinFlow;
            features.PartialCounters = dcFeatures.PartialCounters;
            features.Peripherals = dcFeatures.Peripherals;
            features.Robot = dcFeatures.Robot;
            features.TempLevelsWithStatus = dcFeatures.TempLevelsWithStatus;
            features.TempLevels = dcFeatures.TempLevels;
        }

    }

}
