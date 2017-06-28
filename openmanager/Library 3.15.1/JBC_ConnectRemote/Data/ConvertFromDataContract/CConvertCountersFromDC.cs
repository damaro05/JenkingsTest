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


    internal class CConvertCountersFromDC
    {

        public static void CopyData(CCountersData_SOLD counters, 
                dc_Counters dcCounters)
        {

            counters.ContPlugMinutes = dcCounters.ContPlugMinutes;
            counters.ContWorkMinutes = dcCounters.ContWorkMinutes;
            counters.ContSleepMinutes = dcCounters.ContSleepMinutes;
            counters.ContHiberMinutes = dcCounters.ContHiberMinutes;
            counters.ContIdleMinutes = dcCounters.ContIdleMinutes;
            counters.ContSleepCycles = dcCounters.ContSleepCycles;
            counters.ContDesoldCycles = dcCounters.ContDesoldCycles;
        }

        public static void CopyData_HA(CCountersData_HA counters, 
                dc_Counters_HA dcCounters)
        {

            counters.ContPlugMinutes = dcCounters.ContPlugMinutes;
            counters.ContWorkMinutes = dcCounters.ContWorkMinutes;
            counters.ContWorkCycles = dcCounters.ContWorkCycles;
            counters.ContSuctionCycles = dcCounters.ContSuctionCycles;
        }

        public static void CopyData_SF(CCountersData_SF counters, 
                dc_Counters_SF dcCounters)
        {

            counters.ContTinLength = dcCounters.ContTinLength;
            counters.ContPlugMinutes = dcCounters.ContPlugMinutes;
            counters.ContWorkMinutes = dcCounters.ContWorkMinutes;
            counters.ContWorkCycles = dcCounters.ContWorkCycles;
        }

    }


}
