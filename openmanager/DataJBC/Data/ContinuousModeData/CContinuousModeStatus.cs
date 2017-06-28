// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
    public class CContinuousModeStatus : ICloneable
    {

        public SpeedContinuousMode speed = SpeedContinuousMode.OFF;
        public bool port1 = false;
        public bool port2 = false;
        public bool port3 = false;
        public bool port4 = false;

        public CContinuousModeStatus()
        {

        }

        // ---------------
        // ports related
        // ---------------

        public int portCount()
        {
            return (System.Convert.ToInt32(System.Convert.ToByte(port1) & 0x1)) +
                   (System.Convert.ToInt32(System.Convert.ToByte(port2) & 0x1)) +
                   (System.Convert.ToInt32(System.Convert.ToByte(port3) & 0x1)) +
                   (System.Convert.ToInt32(System.Convert.ToByte(port4) & 0x1));
        }

        public byte getByteFromPorts()
        {
            return System.Convert.ToByte((((0x0 | (System.Convert.ToByte(port1) & 0x1)) | (System.Convert.ToByte(port2) & 0x1) << 1) | (System.Convert.ToByte(port3) & 0x1) << 2) | (System.Convert.ToByte(port4) & 0x1) << 3);
        }

        public void setPortsFromByte(byte ports)
        {
            port1 = (ports & 0x1) == 1;
            port2 = (ports & 0x2) == 2;
            port3 = (ports & 0x4) == 4;
            port4 = (ports & 0x8) == 8;
        }

        public void setPortsFromEnum(Port portA = default(Port), Port portB = default(Port), Port portC = default(Port), Port portD = default(Port))
        {
            port1 = portA == Port.NUM_1 | portB == Port.NUM_1 | portC == Port.NUM_1 | portD == Port.NUM_1;
            port2 = portA == Port.NUM_2 | portB == Port.NUM_2 | portC == Port.NUM_2 | portD == Port.NUM_2;
            port3 = portA == Port.NUM_3 | portB == Port.NUM_3 | portC == Port.NUM_3 | portD == Port.NUM_3;
            port4 = portA == Port.NUM_4 | portB == Port.NUM_4 | portC == Port.NUM_4 | portD == Port.NUM_4;

        }

        public bool selectedPort(Port checkPort)
        {
            return (checkPort == Port.NUM_1 & port1) || (checkPort == Port.NUM_2 & port2) || (checkPort == Port.NUM_3 & port3) || (checkPort == Port.NUM_4 & port4);
        }

        public void setAllPortsFromCount(int count)
        {
            port1 = false;
            port2 = false;
            port3 = false;
            port4 = false;
            if (count >= 1)
            {
                port1 = true;
            }
            if (count >= 2)
            {
                port2 = true;
            }
            if (count >= 3)
            {
                port3 = true;
            }
            if (count >= 4)
            {
                port4 = true;
            }
        }

        public dynamic Clone()
        {
            CContinuousModeStatus cls_Clonado = new CContinuousModeStatus();
            cls_Clonado.port1 = this.port1;
            cls_Clonado.port2 = this.port2;
            cls_Clonado.port3 = this.port3;
            cls_Clonado.port4 = this.port4;
            cls_Clonado.speed = this.speed;

            return cls_Clonado;
        }

    }
}
