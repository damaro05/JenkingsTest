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
    public partial class frmBrowser
    {
        public frmBrowser()
        {
            InitializeComponent();
        }

        public void frmBrowser_Load(System.Object sender, System.EventArgs e)
        {
            ReLoadTexts();
        }

        // --- Localization
        public void ReLoadTexts()
        {
            // buttons
            butPrint.Text = Localization.getResStr(Configuration.browPrintId);
            butPrintPreview.Text = Localization.getResStr(Configuration.browPreviewId);
            butPageSetup.Text = Localization.getResStr(Configuration.browPageSetupId);

        }
        public void butPrintPreview_Click(System.Object sender, System.EventArgs e)
        {
            WebBrowser1.ShowPrintPreviewDialog();
        }

        public void butPrint_Click(System.Object sender, System.EventArgs e)
        {
            WebBrowser1.ShowPrintDialog();
        }

        public void butPageSetup_Click(System.Object sender, System.EventArgs e)
        {
            WebBrowser1.ShowPageSetupDialog();
        }
    }
}
