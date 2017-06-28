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
    public partial class uControlNotificationState_InitUpdateProcess
    {
        public uControlNotificationState_InitUpdateProcess()
        {
            InitializeComponent();
        }

        public void ReLoadTexts()
        {
            this.labelTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_InitUpdateProcessTitleId);
            this.labelSubTitle.Text = Localization.getResStr(Configuration.updatesNotificationState_InitUpdateProcessExplanationId);
        }
    }
}
