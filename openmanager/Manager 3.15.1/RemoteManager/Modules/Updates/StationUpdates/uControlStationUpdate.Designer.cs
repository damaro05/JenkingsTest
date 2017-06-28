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
partial class uControlStationUpdate : System.Windows.Forms.UserControl
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
            this.components = new System.ComponentModel.Container();
            this.cbStation = new System.Windows.Forms.CheckBox();
            this.labelName = new System.Windows.Forms.Label();
            this.labelModel = new System.Windows.Forms.Label();
            this.labelSoftwareVersion = new System.Windows.Forms.Label();
            this.labelUpdateAvailable = new System.Windows.Forms.Label();
            this.ToolTipVersion = new System.Windows.Forms.ToolTip(this.components);
            this.cBoxVersion = new System.Windows.Forms.ComboBox();
            this.cBoxVersion.SelectedIndexChanged += new System.EventHandler(this.cBoxVersion_SelectedIndexChanged);
            this.SuspendLayout();
            //
            //cbStation
            //
            this.cbStation.AutoSize = true;
            this.cbStation.Location = new System.Drawing.Point(10, 5);
            this.cbStation.Name = "cbStation";
            this.cbStation.Size = new System.Drawing.Size(15, 14);
            this.cbStation.TabIndex = 0;
            this.cbStation.UseVisualStyleBackColor = true;
            //
            //labelName
            //
            this.labelName.Location = new System.Drawing.Point(43, 5);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(192, 14);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            //
            //labelModel
            //
            this.labelModel.Location = new System.Drawing.Point(245, 5);
            this.labelModel.Name = "labelModel";
            this.labelModel.Size = new System.Drawing.Size(70, 14);
            this.labelModel.TabIndex = 2;
            this.labelModel.Text = "Model";
            this.labelModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelSoftwareVersion
            //
            this.labelSoftwareVersion.Location = new System.Drawing.Point(325, 5);
            this.labelSoftwareVersion.Name = "labelSoftwareVersion";
            this.labelSoftwareVersion.Size = new System.Drawing.Size(90, 14);
            this.labelSoftwareVersion.TabIndex = 3;
            this.labelSoftwareVersion.Text = "9999999";
            this.labelSoftwareVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelUpdateAvailable
            //
            this.labelUpdateAvailable.Location = new System.Drawing.Point(425, 5);
            this.labelUpdateAvailable.Name = "labelUpdateAvailable";
            this.labelUpdateAvailable.Size = new System.Drawing.Size(180, 14);
            this.labelUpdateAvailable.TabIndex = 4;
            this.labelUpdateAvailable.Text = "Not updatable";
            this.labelUpdateAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //ToolTipVersion
            //
            this.ToolTipVersion.AutomaticDelay = 0;
            this.ToolTipVersion.AutoPopDelay = 5000;
            this.ToolTipVersion.InitialDelay = 100;
            this.ToolTipVersion.ReshowDelay = 100;
            //
            //cBoxVersion
            //
            this.cBoxVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBoxVersion.FormattingEnabled = true;
            this.cBoxVersion.Location = new System.Drawing.Point(615, 1);
            this.cBoxVersion.Name = "cBoxVersion";
            this.cBoxVersion.Size = new System.Drawing.Size(150, 22);
            this.cBoxVersion.TabIndex = 5;
            this.cBoxVersion.Tag = "";
            this.cBoxVersion.Visible = false;
            //
            //uControlStationUpdate
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cBoxVersion);
            this.Controls.Add(this.labelUpdateAvailable);
            this.Controls.Add(this.labelSoftwareVersion);
            this.Controls.Add(this.labelModel);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.cbStation);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "uControlStationUpdate";
            this.Size = new System.Drawing.Size(775, 24);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.CheckBox cbStation;
        internal System.Windows.Forms.Label labelName;
        internal System.Windows.Forms.Label labelModel;
        internal System.Windows.Forms.Label labelSoftwareVersion;
        internal System.Windows.Forms.Label labelUpdateAvailable;
        internal System.Windows.Forms.ToolTip ToolTipVersion;
        internal System.Windows.Forms.ComboBox cBoxVersion;

    }
}
