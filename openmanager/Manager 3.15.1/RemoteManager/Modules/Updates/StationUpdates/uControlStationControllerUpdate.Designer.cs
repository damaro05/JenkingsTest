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
partial class uControlStationControllerUpdate : System.Windows.Forms.UserControl
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
            this.cbStationController = new System.Windows.Forms.CheckBox();
            this.FlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStationController = new System.Windows.Forms.LinkLabel();
            this.imgStationController = new System.Windows.Forms.PictureBox();
            this.lineSeparator = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)this.imgStationController).BeginInit();
            this.SuspendLayout();
            //
            //cbStationController
            //
            this.cbStationController.AutoSize = true;
            this.cbStationController.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.cbStationController.Location = new System.Drawing.Point(4, 3);
            this.cbStationController.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbStationController.Name = "cbStationController";
            this.cbStationController.Size = new System.Drawing.Size(15, 14);
            this.cbStationController.TabIndex = 1;
            this.cbStationController.UseVisualStyleBackColor = true;
            //
            //FlowLayoutPanel
            //
            this.FlowLayoutPanel.AutoSize = true;
            this.FlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.FlowLayoutPanel.Location = new System.Drawing.Point(0, 27);
            this.FlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.FlowLayoutPanel.Name = "FlowLayoutPanel";
            this.FlowLayoutPanel.Size = new System.Drawing.Size(610, 1);
            this.FlowLayoutPanel.TabIndex = 2;
            //
            //labelStationController
            //
            this.labelStationController.ActiveLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelStationController.AutoSize = true;
            this.labelStationController.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelStationController.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelStationController.LinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.labelStationController.Location = new System.Drawing.Point(44, 3);
            this.labelStationController.Name = "labelStationController";
            this.labelStationController.Size = new System.Drawing.Size(124, 14);
            this.labelStationController.TabIndex = 3;
            this.labelStationController.TabStop = true;
            this.labelStationController.Text = "Station Controller";
            this.labelStationController.VisitedLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            //
            //imgStationController
            //
            this.imgStationController.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgStationController.Image = My.Resources.Resources.BlackDwnArrow;
            this.imgStationController.Location = new System.Drawing.Point(28, 2);
            this.imgStationController.Margin = new System.Windows.Forms.Padding(0);
            this.imgStationController.Name = "imgStationController";
            this.imgStationController.Size = new System.Drawing.Size(16, 16);
            this.imgStationController.TabIndex = 4;
            this.imgStationController.TabStop = false;
            //
            //lineSeparator
            //
            this.lineSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lineSeparator.Location = new System.Drawing.Point(62, 11);
            this.lineSeparator.Margin = new System.Windows.Forms.Padding(0);
            this.lineSeparator.Name = "lineSeparator";
            this.lineSeparator.Size = new System.Drawing.Size(713, 1);
            this.lineSeparator.TabIndex = 34;
            //
            //uControlStationControllerUpdate
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.imgStationController);
            this.Controls.Add(this.labelStationController);
            this.Controls.Add(this.FlowLayoutPanel);
            this.Controls.Add(this.cbStationController);
            this.Controls.Add(this.lineSeparator);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.Margin = new System.Windows.Forms.Padding(0, 0, 0, 13);
            this.Name = "uControlStationControllerUpdate";
            this.Size = new System.Drawing.Size(775, 41);
            ((System.ComponentModel.ISupportInitialize)this.imgStationController).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.CheckBox cbStationController;
        internal System.Windows.Forms.FlowLayoutPanel FlowLayoutPanel;
        internal System.Windows.Forms.LinkLabel labelStationController;
        internal System.Windows.Forms.PictureBox imgStationController;
        internal System.Windows.Forms.Label lineSeparator;

    }
}
