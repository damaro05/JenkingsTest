// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace RoutinesJBC
{
    sealed class SearcherServicesData
    {

        //Station discovery
        internal const ushort PORT_STATION_DISCOVERY = 64667;
        internal const string MESSAGE_STATION_DISCOVERY_REQUEST = "JBC_STATIONS_SEARCH";

        //Station Controller discovery
        internal const ushort PORT_STATIONCONTROLLER_DISCOVERY = 64668;
        internal const string MESSAGE_STATIONCONTROLLER_DISCOVERY_REQUEST = "STATION_CONTROLLER_DISCOVERY";
        internal const string MESSAGE_STATIONCONTROLLER_DISCOVERY_RESPONSE = "STATION_CONTROLLER_DISCOVERY_RESPONSE";

        //Host Controller discovery
        internal const ushort PORT_HOSTCONTROLLER_DISCOVERY = 64669;
        internal const string MESSAGE_HOSTCONTROLLER_DISCOVERY_REQUEST = "HOST_CONTROLLER_DISCOVERY";
        internal const string MESSAGE_HOSTCONTROLLER_DISCOVERY_RESPONSE = "HOST_CONTROLLER_DISCOVERY_RESPONSE";

    }
}
