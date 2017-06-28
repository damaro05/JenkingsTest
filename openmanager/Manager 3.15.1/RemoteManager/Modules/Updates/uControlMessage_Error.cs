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
    public partial class uControlMessage_Error
    {
        public uControlMessage_Error()
        {
            InitializeComponent();
        }

        public void uControlMessage_Error_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            CPanelStyle.SetPanelStyle(e, this.ClientRectangle, Color.FromArgb(221, 75, 57));
        }

        public void ReLoadTexts()
        {
            this.labelSubTitle.Text = Localization.getResStr(Configuration.updatesMessageErrorId);
        }

    }
}
