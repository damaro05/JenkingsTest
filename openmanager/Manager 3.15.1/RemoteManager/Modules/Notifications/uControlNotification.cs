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
    public partial class uControlNotification
    {
        public uControlNotification()
        {
            InitializeComponent();
        }

        public delegate void Click_ButtonLeftEventHandler();
        private Click_ButtonLeftEventHandler Click_ButtonLeftEvent;

        public event Click_ButtonLeftEventHandler Click_ButtonLeft
        {
            add
            {
                Click_ButtonLeftEvent = (Click_ButtonLeftEventHandler)System.Delegate.Combine(Click_ButtonLeftEvent, value);
            }
            remove
            {
                Click_ButtonLeftEvent = (Click_ButtonLeftEventHandler)System.Delegate.Remove(Click_ButtonLeftEvent, value);
            }
        }

        public delegate void Click_ButtonRightEventHandler();
        private Click_ButtonRightEventHandler Click_ButtonRightEvent;

        public event Click_ButtonRightEventHandler Click_ButtonRight
        {
            add
            {
                Click_ButtonRightEvent = (Click_ButtonRightEventHandler)System.Delegate.Combine(Click_ButtonRightEvent, value);
            }
            remove
            {
                Click_ButtonRightEvent = (Click_ButtonRightEventHandler)System.Delegate.Remove(Click_ButtonRightEvent, value);
            }
        }

        public delegate void Click_ButtonCloseEventHandler();
        private Click_ButtonCloseEventHandler Click_ButtonCloseEvent;

        public event Click_ButtonCloseEventHandler Click_ButtonClose
        {
            add
            {
                Click_ButtonCloseEvent = (Click_ButtonCloseEventHandler)System.Delegate.Combine(Click_ButtonCloseEvent, value);
            }
            remove
            {
                Click_ButtonCloseEvent = (Click_ButtonCloseEventHandler)System.Delegate.Remove(Click_ButtonCloseEvent, value);
            }
        }



        public void button_left_Click(object sender, EventArgs e)
        {
            if (Click_ButtonLeftEvent != null)
                Click_ButtonLeftEvent();
        }

        public void button_right_Click(object sender, EventArgs e)
        {
            if (Click_ButtonRightEvent != null)
                Click_ButtonRightEvent();
        }

        public void button_close_Click(object sender, EventArgs e)
        {
            if (Click_ButtonCloseEvent != null)
                Click_ButtonCloseEvent();
        }

    }
}
