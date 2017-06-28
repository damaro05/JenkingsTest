using System;

namespace JBC_Connect
{
    internal class CConnectionInfo : ICloneable
    {

        public CSearchUDP.EnumConnection ConnectionType;
        public CSearchUDP.EnumStado Status;
        public string NamePC;

        public CConnectionInfo()
        {
            Status = new CSearchUDP.EnumStado();
            ConnectionType = new CSearchUDP.EnumConnection();
        }

        public dynamic Clone()
        {
            CConnectionInfo cls_ConnectInfo_Clonado = new CConnectionInfo();
            cls_ConnectInfo_Clonado.ConnectionType = this.ConnectionType;
            cls_ConnectInfo_Clonado.Status = this.Status;
            cls_ConnectInfo_Clonado.NamePC = this.NamePC;
            return cls_ConnectInfo_Clonado;
        }

    }
}
