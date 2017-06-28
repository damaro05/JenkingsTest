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
    public partial class ProfileParameters
    {

        private string m_selectedPageName = "";

        //Navigation
        public delegate void ProfileSelectMinusEventHandler();
        private ProfileSelectMinusEventHandler ProfileSelectMinusEvent;

        public event ProfileSelectMinusEventHandler ProfileSelectMinus
        {
            add
            {
                ProfileSelectMinusEvent = (ProfileSelectMinusEventHandler)System.Delegate.Combine(ProfileSelectMinusEvent, value);
            }
            remove
            {
                ProfileSelectMinusEvent = (ProfileSelectMinusEventHandler)System.Delegate.Remove(ProfileSelectMinusEvent, value);
            }
        }

        public delegate void ProfileSelectPlusEventHandler();
        private ProfileSelectPlusEventHandler ProfileSelectPlusEvent;

        public event ProfileSelectPlusEventHandler ProfileSelectPlus
        {
            add
            {
                ProfileSelectPlusEvent = (ProfileSelectPlusEventHandler)System.Delegate.Combine(ProfileSelectPlusEvent, value);
            }
            remove
            {
                ProfileSelectPlusEvent = (ProfileSelectPlusEventHandler)System.Delegate.Remove(ProfileSelectPlusEvent, value);
            }
        }

        public delegate void PointSelectMinusEventHandler();
        private PointSelectMinusEventHandler PointSelectMinusEvent;

        public event PointSelectMinusEventHandler PointSelectMinus
        {
            add
            {
                PointSelectMinusEvent = (PointSelectMinusEventHandler)System.Delegate.Combine(PointSelectMinusEvent, value);
            }
            remove
            {
                PointSelectMinusEvent = (PointSelectMinusEventHandler)System.Delegate.Remove(PointSelectMinusEvent, value);
            }
        }

        public delegate void PointSelectPlusEventHandler();
        private PointSelectPlusEventHandler PointSelectPlusEvent;

        public event PointSelectPlusEventHandler PointSelectPlus
        {
            add
            {
                PointSelectPlusEvent = (PointSelectPlusEventHandler)System.Delegate.Combine(PointSelectPlusEvent, value);
            }
            remove
            {
                PointSelectPlusEvent = (PointSelectPlusEventHandler)System.Delegate.Remove(PointSelectPlusEvent, value);
            }
        }

        //Minus/plus parameters
        public delegate void TemperatureMinusEventHandler();
        private TemperatureMinusEventHandler TemperatureMinusEvent;

        public event TemperatureMinusEventHandler TemperatureMinus
        {
            add
            {
                TemperatureMinusEvent = (TemperatureMinusEventHandler)System.Delegate.Combine(TemperatureMinusEvent, value);
            }
            remove
            {
                TemperatureMinusEvent = (TemperatureMinusEventHandler)System.Delegate.Remove(TemperatureMinusEvent, value);
            }
        }

        public delegate void TemperaturePlusEventHandler();
        private TemperaturePlusEventHandler TemperaturePlusEvent;

        public event TemperaturePlusEventHandler TemperaturePlus
        {
            add
            {
                TemperaturePlusEvent = (TemperaturePlusEventHandler)System.Delegate.Combine(TemperaturePlusEvent, value);
            }
            remove
            {
                TemperaturePlusEvent = (TemperaturePlusEventHandler)System.Delegate.Remove(TemperaturePlusEvent, value);
            }
        }

        public delegate void AirFlowMinusEventHandler();
        private AirFlowMinusEventHandler AirFlowMinusEvent;

        public event AirFlowMinusEventHandler AirFlowMinus
        {
            add
            {
                AirFlowMinusEvent = (AirFlowMinusEventHandler)System.Delegate.Combine(AirFlowMinusEvent, value);
            }
            remove
            {
                AirFlowMinusEvent = (AirFlowMinusEventHandler)System.Delegate.Remove(AirFlowMinusEvent, value);
            }
        }

        public delegate void AirFlowPlusEventHandler();
        private AirFlowPlusEventHandler AirFlowPlusEvent;

        public event AirFlowPlusEventHandler AirFlowPlus
        {
            add
            {
                AirFlowPlusEvent = (AirFlowPlusEventHandler)System.Delegate.Combine(AirFlowPlusEvent, value);
            }
            remove
            {
                AirFlowPlusEvent = (AirFlowPlusEventHandler)System.Delegate.Remove(AirFlowPlusEvent, value);
            }
        }

        public delegate void TimeMinusEventHandler();
        private TimeMinusEventHandler TimeMinusEvent;

        public event TimeMinusEventHandler TimeMinus
        {
            add
            {
                TimeMinusEvent = (TimeMinusEventHandler)System.Delegate.Combine(TimeMinusEvent, value);
            }
            remove
            {
                TimeMinusEvent = (TimeMinusEventHandler)System.Delegate.Remove(TimeMinusEvent, value);
            }
        }

        public delegate void TimePlusEventHandler();
        private TimePlusEventHandler TimePlusEvent;

        public event TimePlusEventHandler TimePlus
        {
            add
            {
                TimePlusEvent = (TimePlusEventHandler)System.Delegate.Combine(TimePlusEvent, value);
            }
            remove
            {
                TimePlusEvent = (TimePlusEventHandler)System.Delegate.Remove(TimePlusEvent, value);
            }
        }



        public ProfileParameters()
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();

            pageProfilesParameters.Location = new Point(0, 0);
            pageProfilesParametersEdit.Location = new Point(0, 0);

            CurrentPage = pageProfilesParameters.Name;
        }

        public string CurrentPage
        {
            set
            {
                System.Windows.Forms.Control[] aControls = this.Controls.Find(value, true);
                if (aControls.Length > 0)
                {
                    aControls[0].BringToFront();
                    m_selectedPageName = System.Convert.ToString(aControls[0].Name);
                }
            }
            get
            {
                if (!string.IsNullOrEmpty(m_selectedPageName))
                {
                    return m_selectedPageName;
                }
                else
                {
                    return "";
                }
            }
        }

        #region Buttons navigation

        public void button_left_Click(object sender, EventArgs e)
        {
            if (ProfileSelectMinusEvent != null)
                ProfileSelectMinusEvent();
        }

        public void button_right_Click(object sender, EventArgs e)
        {
            if (ProfileSelectPlusEvent != null)
                ProfileSelectPlusEvent();
        }

        public void button_leftEdit_Click(object sender, EventArgs e)
        {
            if (PointSelectMinusEvent != null)
                PointSelectMinusEvent();
        }

        public void button_rightEdit_Click(object sender, EventArgs e)
        {
            if (PointSelectPlusEvent != null)
                PointSelectPlusEvent();
        }

        #endregion

        #region Buttons minus/plus parameters

        public void butTemperatureMinus_Click(object sender, EventArgs e)
        {
            if (TemperatureMinusEvent != null)
                TemperatureMinusEvent();
        }

        public void butTemperaturePlus_Click(object sender, EventArgs e)
        {
            if (TemperaturePlusEvent != null)
                TemperaturePlusEvent();
        }

        public void butAirFlowMinus_Click(object sender, EventArgs e)
        {
            if (AirFlowMinusEvent != null)
                AirFlowMinusEvent();
        }

        public void butAirFlowPlus_Click(object sender, EventArgs e)
        {
            if (AirFlowPlusEvent != null)
                AirFlowPlusEvent();
        }

        public void butTimeMinus_Click(object sender, EventArgs e)
        {
            if (TimeMinusEvent != null)
                TimeMinusEvent();
        }

        public void butTimePlus_Click(object sender, EventArgs e)
        {
            if (TimePlusEvent != null)
                TimePlusEvent();
        }

        #endregion

    }
}
