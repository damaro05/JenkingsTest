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
    public class DataSerie
    {

        private int m_time;
        private double m_value;

        public int Time
        {
            get
            {
                return m_time;
            }
            set
            {
                m_time = value;
            }
        }

        public double Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
            }
        }

    }
}
