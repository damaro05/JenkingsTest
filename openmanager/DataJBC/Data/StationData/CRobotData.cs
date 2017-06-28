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
    public class CRobotData : ICloneable
    {

        public enum RobotParity : int
        {
            None = 0,
            Even = 1,
            Odd = 2
        }

        public enum RobotProtocol : int
        {
            RS232 = 0,
            RS485 = 1
        }

        public enum RobotSpeed : int
        {
            bps_1200 = 0,
            bps_2400 = 1,
            bps_4800 = 2,
            bps_9600 = 3,
            bps_19200 = 4,
            bps_38400 = 5,
            bps_57600 = 6,
            bps_115200 = 7,
            bps_230400 = 8,
            bps_250000 = 9,
            bps_460800 = 10,
            bps_500000 = 11
        }

        public enum RobotStop : int
        {
            bits_1 = 1,
            bits_2 = 2
        }


        private OnOff m_Status = OnOff._OFF;
        private RobotProtocol m_Protocol = RobotProtocol.RS232;
        private ushort m_Address = System.Convert.ToUInt16(0); // [00 .. 99]
        private RobotSpeed m_Speed = RobotSpeed.bps_1200;
        private ushort m_DataBits = System.Convert.ToUInt16(8); // fix value
        private RobotStop m_StopBits = RobotStop.bits_1;
        private RobotParity m_Parity = RobotParity.None;


        public dynamic Clone()
        {
            CRobotData new_RobotData = new CRobotData();
            new_RobotData.Status = this.Status;
            new_RobotData.Protocol = this.Protocol;
            new_RobotData.Address = this.Address;
            new_RobotData.Speed = this.Speed;
            new_RobotData.DataBits = this.DataBits;
            new_RobotData.StopBits = this.StopBits;
            new_RobotData.Parity = this.Parity;

            return new_RobotData;
        }

        public OnOff Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                if (OnOff.IsDefined(typeof(OnOff), value))
                {
                    m_Status = value;
                }
                else
                {
                    m_Status = OnOff._OFF;
                }
            }
        }

        public RobotProtocol Protocol
        {
            get
            {
                return m_Protocol;
            }
            set
            {
                if (RobotProtocol.IsDefined(typeof(RobotProtocol), value))
                {
                    m_Protocol = value;
                }
                else
                {
                    m_Protocol = RobotProtocol.RS232;
                }
            }
        }

        public ushort Address
        {
            get
            {
                return m_Address;
            }
            set
            {
                if (value >= 0 && value <= 99)
                {
                    m_Address = value;
                }
            }
        }

        public RobotSpeed Speed
        {
            get
            {
                return m_Speed;
            }
            set
            {
                if (RobotSpeed.IsDefined(typeof(RobotSpeed), value))
                {
                    m_Speed = value;
                }
                else
                {
                    m_Speed = RobotSpeed.bps_1200;
                }
            }
        }

        public ushort DataBits
        {
            get
            {
                return m_DataBits;
            }
            set
            {
                m_DataBits = value;
            }
        }

        public RobotStop StopBits
        {
            get
            {
                return m_StopBits;
            }
            set
            {
                if (RobotStop.IsDefined(typeof(RobotStop), value))
                {
                    m_StopBits = value;
                }
                else
                {
                    m_StopBits = RobotStop.bits_1;
                }
            }
        }

        public RobotParity Parity
        {
            get
            {
                return m_Parity;
            }
            set
            {
                if (RobotParity.IsDefined(typeof(RobotParity), value))
                {
                    m_Parity = value;
                }
                else
                {
                    m_Parity = RobotParity.None;
                }
            }
        }

    }
}
