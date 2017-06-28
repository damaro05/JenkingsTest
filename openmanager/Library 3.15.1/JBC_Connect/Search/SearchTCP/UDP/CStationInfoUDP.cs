using System;

namespace JBC_Connect
{
    internal class CStationInfoUDP
    {
        public CStationConnectionData StationInfo;
        public DateTime HourCreated;


        public CStationInfoUDP(int NumConnectIn)
        {
            StationInfo = new CStationConnectionData(NumConnectIn);
        }

        public CStationInfoUDP(CStationConnectionData StationIn)
        {
            StationInfo = StationIn;
        }

    }
}
