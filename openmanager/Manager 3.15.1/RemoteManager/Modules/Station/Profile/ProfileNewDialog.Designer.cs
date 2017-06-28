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
partial class ProfileNewDialog : System.Windows.Forms.Form
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
            this.rbModeAirTemp = new System.Windows.Forms.RadioButton();
            this.rbModeExtTCTemp = new System.Windows.Forms.RadioButton();
            this.labelRegulationMode = new System.Windows.Forms.Label();
            this.labelProfileName = new System.Windows.Forms.Label();
            this.txtboxProfileName = new System.Windows.Forms.TextBox();
            this.txtboxProfileName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtboxProfileName_KeyPress);
            this.butOk = new System.Windows.Forms.Button();
            this.butOk.Click += new System.EventHandler(this.butOk_Click);
            this.butCancel = new System.Windows.Forms.Button();
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            this.labelProfileNameHelp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            //rbModeAirTemp
            //
            this.rbModeAirTemp.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeAirTemp.BackColor = System.Drawing.Color.Transparent;
            this.rbModeAirTemp.Checked = true;
            this.rbModeAirTemp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbModeAirTemp.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbModeAirTemp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbModeAirTemp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbModeAirTemp.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.rbModeAirTemp.Location = new System.Drawing.Point(30, 50);
            this.rbModeAirTemp.Name = "rbModeAirTemp";
            this.rbModeAirTemp.Size = new System.Drawing.Size(175, 26);
            this.rbModeAirTemp.TabIndex = 36;
            this.rbModeAirTemp.TabStop = true;
            this.rbModeAirTemp.Text = "Air temp";
            this.rbModeAirTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeAirTemp.UseVisualStyleBackColor = false;
            //
            //rbModeExtTCTemp
            //
            this.rbModeExtTCTemp.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbModeExtTCTemp.BackColor = System.Drawing.Color.Transparent;
            this.rbModeExtTCTemp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbModeExtTCTemp.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbModeExtTCTemp.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbModeExtTCTemp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbModeExtTCTemp.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.rbModeExtTCTemp.Location = new System.Drawing.Point(235, 50);
            this.rbModeExtTCTemp.Name = "rbModeExtTCTemp";
            this.rbModeExtTCTemp.Size = new System.Drawing.Size(175, 26);
            this.rbModeExtTCTemp.TabIndex = 37;
            this.rbModeExtTCTemp.Text = "Ext TC temp";
            this.rbModeExtTCTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbModeExtTCTemp.UseVisualStyleBackColor = false;
            //
            //labelRegulationMode
            //
            this.labelRegulationMode.AutoSize = true;
            this.labelRegulationMode.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelRegulationMode.Location = new System.Drawing.Point(20, 20);
            this.labelRegulationMode.Name = "labelRegulationMode";
            this.labelRegulationMode.Size = new System.Drawing.Size(120, 14);
            this.labelRegulationMode.TabIndex = 38;
            this.labelRegulationMode.Text = "Regulation mode?";
            //
            //labelProfileName
            //
            this.labelProfileName.AutoSize = true;
            this.labelProfileName.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.labelProfileName.Location = new System.Drawing.Point(20, 98);
            this.labelProfileName.Margin = new System.Windows.Forms.Padding(0);
            this.labelProfileName.Name = "labelProfileName";
            this.labelProfileName.Size = new System.Drawing.Size(92, 14);
            this.labelProfileName.TabIndex = 39;
            this.labelProfileName.Text = "Profile name?";
            //
            //txtboxProfileName
            //
            this.txtboxProfileName.Location = new System.Drawing.Point(30, 128);
            this.txtboxProfileName.MaxLength = 8;
            this.txtboxProfileName.Name = "txtboxProfileName";
            this.txtboxProfileName.Size = new System.Drawing.Size(380, 22);
            this.txtboxProfileName.TabIndex = 40;
            //
            //butOk
            //
            this.butOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butOk.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.butOk.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.butOk.Location = new System.Drawing.Point(235, 175);
            this.butOk.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butOk.Name = "butOk";
            this.butOk.Size = new System.Drawing.Size(80, 28);
            this.butOk.TabIndex = 41;
            this.butOk.Text = "OK";
            this.butOk.UseVisualStyleBackColor = true;
            //
            //butCancel
            //
            this.butCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.butCancel.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.butCancel.Location = new System.Drawing.Point(330, 175);
            this.butCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(80, 28);
            this.butCancel.TabIndex = 42;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            //
            //labelProfileNameHelp
            //
            this.labelProfileNameHelp.AutoSize = true;
            this.labelProfileNameHelp.Font = new System.Drawing.Font("Verdana", (float)(7.0F));
            this.labelProfileNameHelp.Location = new System.Drawing.Point(114, 100);
            this.labelProfileNameHelp.Margin = new System.Windows.Forms.Padding(0);
            this.labelProfileNameHelp.Name = "labelProfileNameHelp";
            this.labelProfileNameHelp.Size = new System.Drawing.Size(88, 12);
            this.labelProfileNameHelp.TabIndex = 43;
            this.labelProfileNameHelp.Text = "(Max length 8)";
            //
            //ProfileNewDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 224);
            this.Controls.Add(this.labelProfileNameHelp);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOk);
            this.Controls.Add(this.txtboxProfileName);
            this.Controls.Add(this.labelProfileName);
            this.Controls.Add(this.labelRegulationMode);
            this.Controls.Add(this.rbModeExtTCTemp);
            this.Controls.Add(this.rbModeAirTemp);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProfileNewDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.RadioButton rbModeAirTemp;
        internal System.Windows.Forms.RadioButton rbModeExtTCTemp;
        internal System.Windows.Forms.Label labelRegulationMode;
        internal System.Windows.Forms.Label labelProfileName;
        internal System.Windows.Forms.TextBox txtboxProfileName;
        internal System.Windows.Forms.Button butOk;
        internal System.Windows.Forms.Button butCancel;
        internal System.Windows.Forms.Label labelProfileNameHelp;
    }
}
