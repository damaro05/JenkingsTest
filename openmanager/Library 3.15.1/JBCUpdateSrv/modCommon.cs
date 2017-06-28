// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.ServiceModel.Description;

namespace JBCUpdaterSrv
{
    sealed class modCommon
    {

        // servicio WCF
        public static ServiceHost serviceHost = null;

        // string resources
        internal static string errServiceErrorId = "errServiceError";
        internal static string errNotControlledId = "errNotControlled";
        internal static string errStationNotFoundId = "errStationNotFound";
        internal static string errNotValidToolId = "errNotValidTool";
        // events
        public const string evSource = "JBC Updater Service App";
        public const string evSourceLog = "Application";
        public const string evMachine = ".";

        public const string evNewId = "evNew";
        public const string evStartingId = "evStarting";
        public const string evStartedId = "evStarted";
        public const string evStoppingId = "evStopping";
        public const string evStoppedId = "evStopped";
        public const string evErrorCreatingId = "evErrorCreating";

    }
}
