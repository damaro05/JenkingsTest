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
    public partial class frmMessagePopup
    {
        public frmMessagePopup()
        {
            InitializeComponent();
        }

        private Rectangle m_borderForm;


        public void SetMessage(string message)
        {
            labelMsg.Text = message;
            ReLoadTexts();
            this.Visible = true;
        }

        public void frmMessagePopup_Load(object sender, EventArgs e)
        {
            m_borderForm = new Rectangle(0, 0, this.Width, this.Height);
        }

        public void frmUpdatesPopup_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, m_borderForm,
                Color.Gray, 1, ButtonBorderStyle.Solid,
                Color.Gray, 1, ButtonBorderStyle.Solid,
                Color.Gray, 1, ButtonBorderStyle.Solid,
                Color.Gray, 1, ButtonBorderStyle.Solid);
        }

        public void butClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        public void ReLoadTexts()
        {
            butClose.Text = Localization.getResStr(Configuration.dockcloseId);
        }

    }
}
