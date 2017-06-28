using System;

namespace JBC_Connect
{
    internal class CStationInfoTCP
    {
        public CStationConnectionData StationData;
        public DateTime HourAdded;
        public bool bDiscovered;
        public int iAttemptsTCP;
        public bool bConnectedTCP;


        public CStationInfoTCP(int NumConnectIn)
        {
            StationData = new CStationConnectionData(NumConnectIn);
        }

        public CStationInfoTCP(CStationConnectionData StationIn)
        {
            StationData = StationIn;
        }

    }
}
