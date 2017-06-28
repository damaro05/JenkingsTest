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
partial class uControlNotificationState_Warning : System.Windows.Forms.UserControl
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
            this.statePanel = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panelStationControllerVersion = new System.Windows.Forms.Panel();
            this.labelStationControllerVersionTitle = new System.Windows.Forms.Label();
            this.labelStationControllerVersionNum = new System.Windows.Forms.Label();
            this.panelRemoteManagerVersion = new System.Windows.Forms.Panel();
            this.labelRemoteManagerVersionTitle = new System.Windows.Forms.Label();
            this.labelRemoteManagerVersionNum = new System.Windows.Forms.Label();
            this.panelHostControllerVersion = new System.Windows.Forms.Panel();
            this.labelHostControllerVersionTitle = new System.Windows.Forms.Label();
            this.labelHostControllerVersionNum = new System.Windows.Forms.Label();
            this.panelWebManagerVersion = new System.Windows.Forms.Panel();
            this.labelWebManagerVersionTitle = new System.Windows.Forms.Label();
            this.labelWebManagerVersionNum = new System.Windows.Forms.Label();
            this.butUpdate = new System.Windows.Forms.Button();
            this.butUpdate.Click += new System.EventHandler(this.butUpdate_Click);
            this.panelStationControllerVersion.SuspendLayout();
            this.panelRemoteManagerVersion.SuspendLayout();
            this.panelHostControllerVersion.SuspendLayout();
            this.panelWebManagerVersion.SuspendLayout();
            this.SuspendLayout();
            //
            //statePanel
            //
            this.statePanel.BackColor = System.Drawing.Color.Orange;
            this.statePanel.Location = new System.Drawing.Point(0, 0);
            this.statePanel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.statePanel.Name = "statePanel";
            this.statePanel.Size = new System.Drawing.Size(20, 224);
            this.statePanel.TabIndex = 0;
            //
            //labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelTitle.Location = new System.Drawing.Point(47, 16);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(263, 18);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "There are updates availables";
            //
            //panelStationControllerVersion
            //
            this.panelStationControllerVersion.Controls.Add(this.labelStationControllerVersionTitle);
            this.panelStationControllerVersion.Controls.Add(this.labelStationControllerVersionNum);
            this.panelStationControllerVersion.Location = new System.Drawing.Point(47, 78);
            this.panelStationControllerVersion.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.panelStationControllerVersion.Name = "panelStationControllerVersion";
            this.panelStationControllerVersion.Size = new System.Drawing.Size(430, 27);
            this.panelStationControllerVersion.TabIndex = 8;
            //
            //labelStationControllerVersionTitle
            //
            this.labelStationControllerVersionTitle.AutoSize = true;
            this.labelStationControllerVersionTitle.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelStationControllerVersionTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelStationControllerVersionTitle.Location = new System.Drawing.Point(21, 5);
            this.labelStationControllerVersionTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelStationControllerVersionTitle.Name = "labelStationControllerVersionTitle";
            this.labelStationControllerVersionTitle.Size = new System.Drawing.Size(189, 14);
            this.labelStationControllerVersionTitle.TabIndex = 3;
            this.labelStationControllerVersionTitle.Text = "Nº version StationController:";
            //
            //labelStationControllerVersionNum
            //
            this.labelStationControllerVersionNum.AutoSize = true;
            this.labelStationControllerVersionNum.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelStationControllerVersionNum.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelStationControllerVersionNum.Location = new System.Drawing.Point(290, 5);
            this.labelStationControllerVersionNum.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelStationControllerVersionNum.Name = "labelStationControllerVersionNum";
            this.labelStationControllerVersionNum.Size = new System.Drawing.Size(51, 14);
            this.labelStationControllerVersionNum.TabIndex = 4;
            this.labelStationControllerVersionNum.Text = "0.0.0.0";
            //
            //panelRemoteManagerVersion
            //
            this.panelRemoteManagerVersion.Controls.Add(this.labelRemoteManagerVersionTitle);
            this.panelRemoteManagerVersion.Controls.Add(this.labelRemoteManagerVersionNum);
            this.panelRemoteManagerVersion.Location = new System.Drawing.Point(47, 48);
            this.panelRemoteManagerVersion.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.panelRemoteManagerVersion.Name = "panelRemoteManagerVersion";
            this.panelRemoteManagerVersion.Size = new System.Drawing.Size(430, 27);
            this.panelRemoteManagerVersion.TabIndex = 9;
            //
            //labelRemoteManagerVersionTitle
            //
            this.labelRemoteManagerVersionTitle.AutoSize = true;
            this.labelRemoteManagerVersionTitle.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelRemoteManagerVersionTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelRemoteManagerVersionTitle.Location = new System.Drawing.Point(21, 5);
            this.labelRemoteManagerVersionTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelRemoteManagerVersionTitle.Name = "labelRemoteManagerVersionTitle";
            this.labelRemoteManagerVersionTitle.Size = new System.Drawing.Size(185, 14);
            this.labelRemoteManagerVersionTitle.TabIndex = 3;
            this.labelRemoteManagerVersionTitle.Text = "Nº version RemoteManager:";
            //
            //labelRemoteManagerVersionNum
            //
            this.labelRemoteManagerVersionNum.AutoSize = true;
            this.labelRemoteManagerVersionNum.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelRemoteManagerVersionNum.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelRemoteManagerVersionNum.Location = new System.Drawing.Point(290, 5);
            this.labelRemoteManagerVersionNum.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelRemoteManagerVersionNum.Name = "labelRemoteManagerVersionNum";
            this.labelRemoteManagerVersionNum.Size = new System.Drawing.Size(51, 14);
            this.labelRemoteManagerVersionNum.TabIndex = 4;
            this.labelRemoteManagerVersionNum.Text = "0.0.0.0";
            //
            //panelHostControllerVersion
            //
            this.panelHostControllerVersion.Controls.Add(this.labelHostControllerVersionTitle);
            this.panelHostControllerVersion.Controls.Add(this.labelHostControllerVersionNum);
            this.panelHostControllerVersion.Location = new System.Drawing.Point(47, 107);
            this.panelHostControllerVersion.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.panelHostControllerVersion.Name = "panelHostControllerVersion";
            this.panelHostControllerVersion.Size = new System.Drawing.Size(430, 27);
            this.panelHostControllerVersion.TabIndex = 10;
            //
            //labelHostControllerVersionTitle
            //
            this.labelHostControllerVersionTitle.AutoSize = true;
            this.labelHostControllerVersionTitle.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelHostControllerVersionTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelHostControllerVersionTitle.Location = new System.Drawing.Point(21, 5);
            this.labelHostControllerVersionTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelHostControllerVersionTitle.Name = "labelHostControllerVersionTitle";
            this.labelHostControllerVersionTitle.Size = new System.Drawing.Size(173, 14);
            this.labelHostControllerVersionTitle.TabIndex = 3;
            this.labelHostControllerVersionTitle.Text = "Nº version HostController:";
            //
            //labelHostControllerVersionNum
            //
            this.labelHostControllerVersionNum.AutoSize = true;
            this.labelHostControllerVersionNum.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelHostControllerVersionNum.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelHostControllerVersionNum.Location = new System.Drawing.Point(290, 5);
            this.labelHostControllerVersionNum.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelHostControllerVersionNum.Name = "labelHostControllerVersionNum";
            this.labelHostControllerVersionNum.Size = new System.Drawing.Size(51, 14);
            this.labelHostControllerVersionNum.TabIndex = 4;
            this.labelHostControllerVersionNum.Text = "0.0.0.0";
            //
            //panelWebManagerVersion
            //
            this.panelWebManagerVersion.Controls.Add(this.labelWebManagerVersionTitle);
            this.panelWebManagerVersion.Controls.Add(this.labelWebManagerVersionNum);
            this.panelWebManagerVersion.Location = new System.Drawing.Point(47, 136);
            this.panelWebManagerVersion.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.panelWebManagerVersion.Name = "panelWebManagerVersion";
            this.panelWebManagerVersion.Size = new System.Drawing.Size(430, 27);
            this.panelWebManagerVersion.TabIndex = 11;
            //
            //labelWebManagerVersionTitle
            //
            this.labelWebManagerVersionTitle.AutoSize = true;
            this.labelWebManagerVersionTitle.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelWebManagerVersionTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelWebManagerVersionTitle.Location = new System.Drawing.Point(21, 5);
            this.labelWebManagerVersionTitle.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelWebManagerVersionTitle.Name = "labelWebManagerVersionTitle";
            this.labelWebManagerVersionTitle.Size = new System.Drawing.Size(166, 14);
            this.labelWebManagerVersionTitle.TabIndex = 3;
            this.labelWebManagerVersionTitle.Text = "Nº version WebManager:";
            //
            //labelWebManagerVersionNum
            //
            this.labelWebManagerVersionNum.AutoSize = true;
            this.labelWebManagerVersionNum.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelWebManagerVersionNum.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelWebManagerVersionNum.Location = new System.Drawing.Point(290, 5);
            this.labelWebManagerVersionNum.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.labelWebManagerVersionNum.Name = "labelWebManagerVersionNum";
            this.labelWebManagerVersionNum.Size = new System.Drawing.Size(51, 14);
            this.labelWebManagerVersionNum.TabIndex = 4;
            this.labelWebManagerVersionNum.Text = "0.0.0.0";
            //
            //butUpdate
            //
            this.butUpdate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butUpdate.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.butUpdate.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.butUpdate.Location = new System.Drawing.Point(340, 183);
            this.butUpdate.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butUpdate.Name = "butUpdate";
            this.butUpdate.Size = new System.Drawing.Size(137, 25);
            this.butUpdate.TabIndex = 12;
            this.butUpdate.Text = "Install updates";
            this.butUpdate.UseVisualStyleBackColor = true;
            //
            //uControlNotificationState_Warning
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.butUpdate);
            this.Controls.Add(this.panelWebManagerVersion);
            this.Controls.Add(this.panelHostControllerVersion);
            this.Controls.Add(this.panelRemoteManagerVersion);
            this.Controls.Add(this.panelStationControllerVersion);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.statePanel);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uControlNotificationState_Warning";
            this.Size = new System.Drawing.Size(500, 224);
            this.panelStationControllerVersion.ResumeLayout(false);
            this.panelStationControllerVersion.PerformLayout();
            this.panelRemoteManagerVersion.ResumeLayout(false);
            this.panelRemoteManagerVersion.PerformLayout();
            this.panelHostControllerVersion.ResumeLayout(false);
            this.panelHostControllerVersion.PerformLayout();
            this.panelWebManagerVersion.ResumeLayout(false);
            this.panelWebManagerVersion.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Panel statePanel;
        internal System.Windows.Forms.Label labelTitle;
        internal System.Windows.Forms.Panel panelStationControllerVersion;
        internal System.Windows.Forms.Label labelStationControllerVersionTitle;
        internal System.Windows.Forms.Label labelStationControllerVersionNum;
        internal System.Windows.Forms.Panel panelRemoteManagerVersion;
        internal System.Windows.Forms.Label labelRemoteManagerVersionTitle;
        internal System.Windows.Forms.Label labelRemoteManagerVersionNum;
        internal System.Windows.Forms.Panel panelHostControllerVersion;
        internal System.Windows.Forms.Label labelHostControllerVersionTitle;
        internal System.Windows.Forms.Label labelHostControllerVersionNum;
        internal System.Windows.Forms.Panel panelWebManagerVersion;
        internal System.Windows.Forms.Label labelWebManagerVersionTitle;
        internal System.Windows.Forms.Label labelWebManagerVersionNum;
        internal System.Windows.Forms.Button butUpdate;

    }
}
