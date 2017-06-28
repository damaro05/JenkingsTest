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
partial class frmMessagePopup : System.Windows.Forms.Form
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
            this.labelMsg = new System.Windows.Forms.Label();
            this.butClose = new System.Windows.Forms.Button();
            this.butClose.Click += new System.EventHandler(this.butClose_Click);
            this.pictureBox_Error = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)this.pictureBox_Error).BeginInit();
            this.SuspendLayout();
            //
            //labelMsg
            //
            this.labelMsg.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.labelMsg.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelMsg.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelMsg.Location = new System.Drawing.Point(80, 31);
            this.labelMsg.Margin = new System.Windows.Forms.Padding(0);
            this.labelMsg.Name = "labelMsg";
            this.labelMsg.Size = new System.Drawing.Size(450, 74);
            this.labelMsg.TabIndex = 4;
            this.labelMsg.Text = "" + System.Convert.ToString(global::Microsoft.VisualBasic.Strings.ChrW(13)) + System.Convert.ToString(global::Microsoft.VisualBasic.Strings.ChrW(10));
            //
            //butClose
            //
            this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butClose.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.butClose.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.butClose.Location = new System.Drawing.Point(217, 155);
            this.butClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(116, 32);
            this.butClose.TabIndex = 4;
            this.butClose.Text = "Cerrar";
            this.butClose.UseVisualStyleBackColor = true;
            //
            //pictureBox_Error
            //
            this.pictureBox_Error.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox_Error.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pictureBox_Error.Image = My.Resources.Resources.error_icon;
            this.pictureBox_Error.Location = new System.Drawing.Point(20, 20);
            this.pictureBox_Error.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox_Error.Name = "pictureBox_Error";
            this.pictureBox_Error.Size = new System.Drawing.Size(30, 30);
            this.pictureBox_Error.TabIndex = 5;
            this.pictureBox_Error.TabStop = false;
            //
            //frmMessagePopup
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(550, 212);
            this.Controls.Add(this.pictureBox_Error);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.labelMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMessagePopup";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "frmMessagePopup";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)this.pictureBox_Error).EndInit();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Label labelMsg;
        internal System.Windows.Forms.Button butClose;
        internal System.Windows.Forms.PictureBox pictureBox_Error;
    }
}
