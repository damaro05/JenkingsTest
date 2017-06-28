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
partial class uControlNotificationState_NoConnection : System.Windows.Forms.UserControl
    {

        //UserControl overrides dispose to clean up the component list.
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
            this.labelSubTitle = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.statePanel = new System.Windows.Forms.Panel();
            this.butReconnect = new System.Windows.Forms.Button();
            this.butReconnect.Click += new System.EventHandler(this.butReconnect_Click);
            this.SuspendLayout();
            //
            //labelSubTitle
            //
            this.labelSubTitle.AutoSize = true;
            this.labelSubTitle.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelSubTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelSubTitle.Location = new System.Drawing.Point(47, 48);
            this.labelSubTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSubTitle.Name = "labelSubTitle";
            this.labelSubTitle.Size = new System.Drawing.Size(149, 14);
            this.labelSubTitle.TabIndex = 5;
            this.labelSubTitle.Text = "Review the connection";
            //
            //labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelTitle.Location = new System.Drawing.Point(47, 16);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(400, 18);
            this.labelTitle.TabIndex = 4;
            this.labelTitle.Text = "There is no connection to the Host Controller";
            //
            //statePanel
            //
            this.statePanel.BackColor = System.Drawing.Color.Gray;
            this.statePanel.Location = new System.Drawing.Point(0, 0);
            this.statePanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.statePanel.Name = "statePanel";
            this.statePanel.Size = new System.Drawing.Size(20, 127);
            this.statePanel.TabIndex = 3;
            //
            //butReconnect
            //
            this.butReconnect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butReconnect.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.butReconnect.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.butReconnect.Location = new System.Drawing.Point(340, 82);
            this.butReconnect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butReconnect.Name = "butReconnect";
            this.butReconnect.Size = new System.Drawing.Size(137, 25);
            this.butReconnect.TabIndex = 13;
            this.butReconnect.Text = "Reconnect";
            this.butReconnect.UseVisualStyleBackColor = true;
            //
            //uControlNotificationState_NoConnection
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.butReconnect);
            this.Controls.Add(this.labelSubTitle);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.statePanel);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uControlNotificationState_NoConnection";
            this.Size = new System.Drawing.Size(500, 127);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Label labelSubTitle;
        internal System.Windows.Forms.Label labelTitle;
        internal System.Windows.Forms.Panel statePanel;
        internal System.Windows.Forms.Button butReconnect;

    }
}
