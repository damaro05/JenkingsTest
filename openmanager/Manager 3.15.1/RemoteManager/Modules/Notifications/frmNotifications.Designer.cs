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
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public
partial class frmNotifications : System.Windows.Forms.Form
    {

        //Form overrides dispose to clean up the component list.
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer
        private System.ComponentModel.Container components = null;

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.
        //Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.uControlNotification = new uControlNotification();
            base.MouseMove += new System.Windows.Forms.MouseEventHandler(frmNotifications_MouseMove);
            this.uControlNotification.Click_ButtonLeft += new uControlNotification.Click_ButtonLeftEventHandler(this.frmNotifications_Click_ButtonLeft);
            this.uControlNotification.Click_ButtonRight += new uControlNotification.Click_ButtonRightEventHandler(this.frmNotifications_Click_ButtonRight);
            this.uControlNotification.Click_ButtonClose += new uControlNotification.Click_ButtonCloseEventHandler(this.frmNotifications_Click_ButtonClose);
            this.uControlNotificationPermanent = new uControlNotification();
            this.uControlNotificationPermanent.Click_ButtonLeft += new uControlNotification.Click_ButtonLeftEventHandler(this.frmNotificationsPermanent_Click_ButtonLeft);
            this.uControlNotificationPermanent.Click_ButtonRight += new uControlNotification.Click_ButtonRightEventHandler(this.frmNotificationsPermanent_Click_ButtonRight);
            this.uControlNotificationPermanent.Click_ButtonClose += new uControlNotification.Click_ButtonCloseEventHandler(this.frmNotificationsPermanent_Click_ButtonClose);
            this.SuspendLayout();
            //
            //uControlNotification
            //
            this.uControlNotification.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.uControlNotification.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.uControlNotification.Location = new System.Drawing.Point(0, 83);
            this.uControlNotification.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.uControlNotification.Name = "uControlNotification";
            this.uControlNotification.Size = new System.Drawing.Size(590, 80);
            this.uControlNotification.TabIndex = 1;
            //
            //uControlNotificationPermanent
            //
            this.uControlNotificationPermanent.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.uControlNotificationPermanent.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.uControlNotificationPermanent.Location = new System.Drawing.Point(0, 0);
            this.uControlNotificationPermanent.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.uControlNotificationPermanent.Name = "uControlNotificationPermanent";
            this.uControlNotificationPermanent.Size = new System.Drawing.Size(590, 80);
            this.uControlNotificationPermanent.TabIndex = 0;
            //
            //frmNotifications
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(590, 163);
            this.Controls.Add(this.uControlNotification);
            this.Controls.Add(this.uControlNotificationPermanent);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmNotifications";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Notifications";
            this.TopMost = true;
            this.ResumeLayout(false);

        }
        internal uControlNotification uControlNotificationPermanent;
        internal uControlNotification uControlNotification;
    }
}
