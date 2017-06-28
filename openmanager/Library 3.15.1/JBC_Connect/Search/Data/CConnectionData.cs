using DataJBC;

namespace JBC_Connect
{
    internal class CConnectionData
    {

        public SearchMode Mode;
        public RoutinesLibrary.IO.SerialPort pSerialPort = null;
        public RoutinesLibrary.Net.Protocols.TCP.TCP pWinSock = null;

        public byte PCNumDevice; // Station frame. Source field (1D, 1E)
        public byte StationNumDevice; // Station frame. Target field [10 .. 60]
        public CStationBase.Protocol FrameProtocol;
        public CStationBase.Protocol CommandProtocol;
        public string StationModel;
        public string SoftwareVersion;
        public string HardwareVersion;

    }
}
