// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


namespace RemoteManager
{
    public partial class uControlStationUpdate
    {
        public uControlStationUpdate()
        {
            InitializeComponent();
        }

        private Hashtable m_hardwareVersion = new Hashtable(); //HW <-> SW


        public void AddFirmware(string hardware, string software)
        {
            if (m_hardwareVersion.Contains(hardware))
            {
                m_hardwareVersion[hardware] = software;
            }
            else
            {
                m_hardwareVersion.Add(hardware, software);
            }
        }

        public Hashtable GetFirmware()
        {
            return m_hardwareVersion;
        }

        public void cBoxVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string softwareVersion = System.Convert.ToString((this.cBoxVersion.SelectedItem).ToString().Split('(')[0]);

            ArrayList keys = new ArrayList(m_hardwareVersion.Keys);
            foreach (string key in keys)
            {
                m_hardwareVersion[key] = softwareVersion;
            }
        }

    }
}
