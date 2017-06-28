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
using DataJBC;

namespace JBCTraceControlLocalSrv
{
    public class TracePortData : ICloneable
    {

        public Port port = Port.NO_PORT;
        public int temperature = 0;
        public int power = 0;
        public byte status = (byte)0;
        public GenericStationTools tool = GenericStationTools.NO_TOOL;
        // HT hot air stations
        public int flow = 0;
        public int tempTC1 = 0;
        public int tempTC2 = 0;
        public int timetostop = 0;

        public TracePortData(Port _port = default(Port))
        {
            port = _port;
        }

        public dynamic Clone()
        {
            TracePortData c_Clonado = new TracePortData();
            c_Clonado.port = this.port;
            c_Clonado.temperature = this.temperature;
            c_Clonado.power = this.power;
            c_Clonado.status = this.status;
            c_Clonado.tool = this.tool;
            // HT hot air stations
            c_Clonado.flow = this.flow;
            c_Clonado.tempTC1 = this.tempTC1;
            c_Clonado.tempTC2 = this.tempTC2;
            c_Clonado.timetostop = this.timetostop;

            return c_Clonado;
        }

    }
}
