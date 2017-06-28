// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Net;

namespace DataJBC
{
    public class CEthernetData : ICloneable
    {

        private OnOff m_DHCP = OnOff._OFF;
        private IPAddress m_IP = new IPAddress(0);
        private IPAddress m_Mask = new IPAddress(0);
        private IPAddress m_Gateway = new IPAddress(0);
        private IPAddress m_DNS1 = new IPAddress(0);
        private IPAddress m_DNS2 = new IPAddress(0);
        private ushort m_Port = System.Convert.ToUInt16(0);


        public dynamic Clone()
        {
            CEthernetData new_EthernetData = new CEthernetData();
            new_EthernetData.DHCP = this.DHCP;
            new_EthernetData.IP = this.IP;
            new_EthernetData.Mask = this.Mask;
            new_EthernetData.Gateway = this.Gateway;
            new_EthernetData.DNS1 = this.DNS1;
            new_EthernetData.DNS2 = this.DNS2;
            new_EthernetData.Port = this.Port;

            return new_EthernetData;
        }

        public OnOff DHCP
        {
            get
            {
                return m_DHCP;
            }
            set
            {
                if (OnOff.IsDefined(typeof(OnOff), value))
                {
                    m_DHCP = value;
                }
                else
                {
                    m_DHCP = OnOff._OFF;
                }
            }
        }

        public IPAddress IP
        {
            get
            {
                return m_IP;
            }
            set
            {
                m_IP = value;
            }
        }

        public IPAddress Mask
        {
            get
            {
                return m_Mask;
            }
            set
            {
                m_Mask = value;
            }
        }

        public IPAddress Gateway
        {
            get
            {
                return m_Gateway;
            }
            set
            {
                m_Gateway = value;
            }
        }

        public IPAddress DNS1
        {
            get
            {
                return m_DNS1;
            }
            set
            {
                m_DNS1 = value;
            }
        }

        public IPAddress DNS2
        {
            get
            {
                return m_DNS2;
            }
            set
            {
                m_DNS2 = value;
            }
        }

        public ushort Port
        {
            get
            {
                return m_Port;
            }
            set
            {
                m_Port = value;
            }
        }

    }
}
