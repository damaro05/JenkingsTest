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
    public partial class uControlNotificationState_Warning
    {
        public uControlNotificationState_Warning()
        {
            InitializeComponent();
        }

        public delegate void UpdateSystemEventHandler();
        private UpdateSystemEventHandler UpdateSystemEvent;

        public event UpdateSystemEventHandler UpdateSystem
        {
            add
            {
                UpdateSystemEvent = (UpdateSystemEventHandler)System.Delegate.Combine(UpdateSystemEvent, value);
            }
            remove
            {
                UpdateSystemEvent = (UpdateSystemEventHandler)System.Delegate.Remove(UpdateSystemEvent, value);
            }
        }



        public void ReLoadTexts()
        {

            this.labelTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_WarningTitleId);
            this.labelStationControllerVersionTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_WarningStationControllerVersionId);
            this.labelRemoteManagerVersionTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_WarningRemoteManagerVersionId);
            this.labelHostControllerVersionTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_WarningHostControllerVersionId);
            this.labelWebManagerVersionTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_WarningWebManagerVersionId);
            this.butUpdate.Text = Localization.getResStr(Configuration.updatesNotificationState_WarningInstallUpdatesId);
        }

        public void butUpdate_Click(object sender, EventArgs e)
        {
            if (UpdateSystemEvent != null)
                UpdateSystemEvent();
        }

    }
}
