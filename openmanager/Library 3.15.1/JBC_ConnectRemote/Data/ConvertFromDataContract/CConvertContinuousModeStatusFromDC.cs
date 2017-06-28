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
using SpeedContinuousMode = DataJBC.SpeedContinuousMode;
// End of VB project level imports


namespace JBC_ConnectRemote
{
    public class CConvertContinuousModeStatusFromDC
    {

        public static void CopyData(CContinuousModeStatus continuousModeStatus,
                                    dc_ContinuousModeStatus dcContinuousModeStatus)
        {

            continuousModeStatus.speed = (SpeedContinuousMode)dcContinuousModeStatus.Speed;
            continuousModeStatus.port1 = dcContinuousModeStatus.port1;
            continuousModeStatus.port2 = dcContinuousModeStatus.port2;
            continuousModeStatus.port3 = dcContinuousModeStatus.port3;
            continuousModeStatus.port4 = dcContinuousModeStatus.port4;
        }

    }
}
