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
    public partial class uControlNotificationState_NoConnection
    {
        public uControlNotificationState_NoConnection()
        {
            InitializeComponent();
        }

        public delegate void ReconnectHostControllerEventHandler();
        private ReconnectHostControllerEventHandler ReconnectHostControllerEvent;

        public event ReconnectHostControllerEventHandler ReconnectHostController
        {
            add
            {
                ReconnectHostControllerEvent = (ReconnectHostControllerEventHandler)System.Delegate.Combine(ReconnectHostControllerEvent, value);
            }
            remove
            {
                ReconnectHostControllerEvent = (ReconnectHostControllerEventHandler)System.Delegate.Remove(ReconnectHostControllerEvent, value);
            }
        }



        public void ReLoadTexts()
        {

            this.labelTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_NoConnectionTitleId);
            this.labelSubTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_NoConnectionExplanationId);
            this.butReconnect.Text = Localization.getResStr(Configuration.updatesNotificationState_NoConnectionReconnectId);
        }

        public void butReconnect_Click(object sender, EventArgs e)
        {
            if (ReconnectHostControllerEvent != null)
                ReconnectHostControllerEvent();
        }

    }
}
