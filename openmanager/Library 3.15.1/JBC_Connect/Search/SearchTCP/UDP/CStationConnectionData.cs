using System;
using System.Net;

namespace JBC_Connect
{
    internal class CStationConnectionData : ICloneable
    {

        public IPEndPoint IPEndPointValue;
        public string ProtocolVersion;
        public string StationModel;
        public string SoftVersion;
        public string HardVersion;
        public CConnectionInfo[] Connection;
        public int ConnectionsCount = 0;


        public CStationConnectionData(int ConnectionsCountIn)
        {
            ConnectionsCount = ConnectionsCountIn;
            IPEndPointValue = new IPEndPoint(0, 0);
            Connection = new CConnectionInfo[ConnectionsCount - 1 + 1];
            for (int index = 0; index <= ConnectionsCount - 1; index++)
            {
                Connection[index] = new CConnectionInfo();
            }
        }

        public dynamic Clone()
        {
            CStationConnectionData cls_StationInfo_Clonado = new CStationConnectionData(ConnectionsCount);
            cls_StationInfo_Clonado.IPEndPointValue.Address = this.IPEndPointValue.Address;
            cls_StationInfo_Clonado.IPEndPointValue.Port = this.IPEndPointValue.Port;
            cls_StationInfo_Clonado.ProtocolVersion = this.ProtocolVersion;
            cls_StationInfo_Clonado.StationModel = this.StationModel;
            cls_StationInfo_Clonado.SoftVersion = this.SoftVersion;
            cls_StationInfo_Clonado.HardVersion = this.HardVersion;
            cls_StationInfo_Clonado.Connection = new CConnectionInfo[this.Connection.Length - 1 + 1];
            for (var index = 0; index <= this.Connection.Length - 1; index++)
            {
                cls_StationInfo_Clonado.Connection[(int)index] = (CConnectionInfo)(this.Connection[(int)index].Clone());
            }

            return cls_StationInfo_Clonado;
        }

    }
}
