// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBCTraceControlLocalSrv
{
    sealed class modL10nData
    {

        internal const string EV_ON_ID = "evOn";
        internal const string EV_OFF_ID = "evOff";
        //Public Const EV_NEW_ID As String = "evNew"
        internal const string EV_STARTING_ID = "evStarting";
        internal const string EV_STARTED_ID = "evStarted";
        internal const string EV_STOPPING_ID = "evStopping";
        internal const string EV_STOPPED_ID = "evStopped";
        internal const string EV_ERROR_CREATING_ID = "evErrorCreating";
        internal const string EV_ERROR_STARTING_DLL_ID = "evErrorStartingDLL";
        internal const string EV_SEARCHING_USB_STATIONS_ID = "evSearchingUSBStations";
        internal const string EV_SEARCHING_ETH_STATIONS_ID = "evSearchingETHStations";
        internal const string EV_STOP_SEARCHING_USB_STATIONS_ID = "evStopSearchingUSBStations";
        internal const string EV_STOP_SEARCHING_ETH_STATIONS_ID = "evStopSearchingETHStations";
        internal const string EV_SEARCHING_NONE_ID = "evSearchingNone";
        internal const string EV_SET_SEARCHING_USB_ID = "evSetSearchingUSB";
        internal const string EV_SET_SEARCHING_ETH_ID = "evSetSearchingETH";
        internal const string EV_ERROR_ON_CUSTOM_COMMAND_ID = "evErrorOnCustomCommand";

    }
}
