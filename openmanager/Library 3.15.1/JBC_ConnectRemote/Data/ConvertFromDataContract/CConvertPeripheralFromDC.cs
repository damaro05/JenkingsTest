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
    public class CConvertPeripheralFromDC
    {

        public static void CopyData(CPeripheralData peripheralData,
                                    dc_PeripheralInfo dcPeripheralInfo)
        {

            peripheralData.ID = dcPeripheralInfo.ID;
            peripheralData.Version = dcPeripheralInfo.Version;
            peripheralData.Hash_MCU_UID = dcPeripheralInfo.Hash_MCU_UID;
            peripheralData.DateTime = dcPeripheralInfo.DateTime;
            peripheralData.Type = (CPeripheralData.PeripheralType)dcPeripheralInfo.Type;
            peripheralData.PortAttached = (Port)((Port)dcPeripheralInfo.PortAttached);
            peripheralData.WorkFunction = (CPeripheralData.PeripheralFunction)dcPeripheralInfo.WorkFunction;
            peripheralData.ActivationMode = (CPeripheralData.PeripheralActivation)dcPeripheralInfo.ActivationMode;
            peripheralData.DelayTime = dcPeripheralInfo.DelayTime;
            peripheralData.StatusActive = (OnOff)dcPeripheralInfo.StatusActive;
            peripheralData.StatusPD = (CPeripheralData.PeripheralStatusPD)dcPeripheralInfo.StatusPD;
        }

    }
}
