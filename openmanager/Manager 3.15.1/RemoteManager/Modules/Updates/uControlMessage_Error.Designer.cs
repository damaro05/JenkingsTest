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
partial class uControlMessage_Error : System.Windows.Forms.UserControl
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
            this.Paint += new System.Windows.Forms.PaintEventHandler(uControlMessage_Error_Paint);
            this.SuspendLayout();
            //
            //labelSubTitle
            //
            this.labelSubTitle.AutoSize = true;
            this.labelSubTitle.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelSubTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(221)), System.Convert.ToInt32(System.Convert.ToByte(75)), System.Convert.ToInt32(System.Convert.ToByte(57)));
            this.labelSubTitle.Location = new System.Drawing.Point(25, 15);
            this.labelSubTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelSubTitle.Name = "labelSubTitle";
            this.labelSubTitle.Size = new System.Drawing.Size(338, 14);
            this.labelSubTitle.TabIndex = 8;
            this.labelSubTitle.Text = "An error has occurred during the installation process";
            //
            //uControlMessage_Error
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.MistyRose;
            this.Controls.Add(this.labelSubTitle);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "uControlMessage_Error";
            this.Size = new System.Drawing.Size(500, 44);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Label labelSubTitle;

    }
}
