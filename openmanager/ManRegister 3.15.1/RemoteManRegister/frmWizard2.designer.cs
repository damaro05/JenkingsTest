// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
// End of VB project level imports

namespace RemoteManRegister
{
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public
    partial class frmWizard2 : System.Windows.Forms.Form
    {

        //Form reemplaza a Dispose para limpiar la lista de componentes.
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

        //Requerido por el Diseñador de Windows Forms
        private System.ComponentModel.Container components = null;

        //NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
        //Se puede modificar usando el Diseñador de Windows Forms.
        //No lo modifique con el editor de código.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWizard2));
            this.lblInfo = new System.Windows.Forms.Label();
            this.Load += new System.EventHandler(frmWizard2_Load);
            this.gbxContent = new System.Windows.Forms.GroupBox();
            this.lblSeconds1 = new System.Windows.Forms.Label();
            this.lblAxisTimeWindow = new System.Windows.Forms.Label();
            this.cbxTimeRange = new System.Windows.Forms.ComboBox();
            this.lbxPower = new System.Windows.Forms.ListBox();
            this.lbxPower.SelectedIndexChanged += new System.EventHandler(this.lbxPower_SelectedIndexChanged);
            this.lbxTemperature = new System.Windows.Forms.ListBox();
            this.lbxTemperature.SelectedIndexChanged += new System.EventHandler(this.lbxTemperature_SelectedIndexChanged);
            this.butFinish = new System.Windows.Forms.Button();
            this.butFinish.Click += new System.EventHandler(this.butFinish_Click);
            this.butPrev = new System.Windows.Forms.Button();
            this.butPrev.Click += new System.EventHandler(this.butPrev_Click);
            this.gbGridStep = new System.Windows.Forms.GroupBox();
            this.lblSeconds2 = new System.Windows.Forms.Label();
            this.cbxTimeStep = new System.Windows.Forms.ComboBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.rdbPower = new System.Windows.Forms.RadioButton();
            this.rdbPower.CheckedChanged += new System.EventHandler(this.rdbPower_CheckedChanged);
            this.rdbTemperature = new System.Windows.Forms.RadioButton();
            this.rdbTemperature.CheckedChanged += new System.EventHandler(this.rdbTemperature_CheckedChanged);
            this.lblPwrAdjust = new System.Windows.Forms.Label();
            this.cbxPwrStep = new System.Windows.Forms.ComboBox();
            this.cbxPwrStep.SelectedIndexChanged += new System.EventHandler(this.cbxPwrStep_SelectedIndexChanged);
            this.lblTempAdjust = new System.Windows.Forms.Label();
            this.cbxTempStep = new System.Windows.Forms.ComboBox();
            this.cbxTempStep.SelectedIndexChanged += new System.EventHandler(this.cbxTempStep_SelectedIndexChanged);
            this.rbAxisTempFahrenheit = new System.Windows.Forms.RadioButton();
            this.rbAxisTempFahrenheit.CheckedChanged += new System.EventHandler(this.rbAxisTemp_CheckedChanged);
            this.rbAxisTempCelsius = new System.Windows.Forms.RadioButton();
            this.rbAxisTempCelsius.CheckedChanged += new System.EventHandler(this.rbAxisTemp_CheckedChanged);
            this.gbxContent.SuspendLayout();
            this.gbGridStep.SuspendLayout();
            this.SuspendLayout();
            //
            //lblInfo
            //
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(374, 30);
            this.lblInfo.TabIndex = 5;
            this.lblInfo.Text = "Label1";
            //
            //gbxContent
            //
            this.gbxContent.Controls.Add(this.rbAxisTempFahrenheit);
            this.gbxContent.Controls.Add(this.rbAxisTempCelsius);
            this.gbxContent.Controls.Add(this.lblSeconds1);
            this.gbxContent.Controls.Add(this.lblAxisTimeWindow);
            this.gbxContent.Controls.Add(this.cbxTimeRange);
            this.gbxContent.Controls.Add(this.lbxPower);
            this.gbxContent.Controls.Add(this.lbxTemperature);
            this.gbxContent.Location = new System.Drawing.Point(12, 42);
            this.gbxContent.Name = "gbxContent";
            this.gbxContent.Size = new System.Drawing.Size(374, 122);
            this.gbxContent.TabIndex = 4;
            this.gbxContent.TabStop = false;
            this.gbxContent.Text = "Axis range";
            //
            //lblSeconds1
            //
            this.lblSeconds1.Location = new System.Drawing.Point(205, 96);
            this.lblSeconds1.Name = "lblSeconds1";
            this.lblSeconds1.Size = new System.Drawing.Size(100, 13);
            this.lblSeconds1.TabIndex = 16;
            this.lblSeconds1.Text = "(seconds)";
            //
            //lblAxisTimeWindow
            //
            this.lblAxisTimeWindow.Location = new System.Drawing.Point(6, 96);
            this.lblAxisTimeWindow.Name = "lblAxisTimeWindow";
            this.lblAxisTimeWindow.Size = new System.Drawing.Size(127, 13);
            this.lblAxisTimeWindow.TabIndex = 15;
            this.lblAxisTimeWindow.Text = "Time Window";
            this.lblAxisTimeWindow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //cbxTimeRange
            //
            this.cbxTimeRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeRange.FormattingEnabled = true;
            this.cbxTimeRange.Location = new System.Drawing.Point(139, 93);
            this.cbxTimeRange.Name = "cbxTimeRange";
            this.cbxTimeRange.Size = new System.Drawing.Size(60, 21);
            this.cbxTimeRange.TabIndex = 14;
            //
            //lbxPower
            //
            this.lbxPower.FormattingEnabled = true;
            this.lbxPower.Location = new System.Drawing.Point(236, 18);
            this.lbxPower.Name = "lbxPower";
            this.lbxPower.Size = new System.Drawing.Size(132, 69);
            this.lbxPower.TabIndex = 13;
            //
            //lbxTemperature
            //
            this.lbxTemperature.FormattingEnabled = true;
            this.lbxTemperature.Location = new System.Drawing.Point(6, 19);
            this.lbxTemperature.Name = "lbxTemperature";
            this.lbxTemperature.Size = new System.Drawing.Size(132, 69);
            this.lbxTemperature.TabIndex = 12;
            //
            //butFinish
            //
            this.butFinish.Location = new System.Drawing.Point(289, 276);
            this.butFinish.Name = "butFinish";
            this.butFinish.Size = new System.Drawing.Size(97, 23);
            this.butFinish.TabIndex = 6;
            this.butFinish.Text = "Finish";
            this.butFinish.UseVisualStyleBackColor = true;
            //
            //butPrev
            //
            this.butPrev.Location = new System.Drawing.Point(12, 276);
            this.butPrev.Name = "butPrev";
            this.butPrev.Size = new System.Drawing.Size(97, 23);
            this.butPrev.TabIndex = 7;
            this.butPrev.Text = "Previous";
            this.butPrev.UseVisualStyleBackColor = true;
            //
            //gbGridStep
            //
            this.gbGridStep.Controls.Add(this.lblSeconds2);
            this.gbGridStep.Controls.Add(this.cbxTimeStep);
            this.gbGridStep.Controls.Add(this.lblTime);
            this.gbGridStep.Controls.Add(this.rdbPower);
            this.gbGridStep.Controls.Add(this.rdbTemperature);
            this.gbGridStep.Controls.Add(this.lblPwrAdjust);
            this.gbGridStep.Controls.Add(this.cbxPwrStep);
            this.gbGridStep.Controls.Add(this.lblTempAdjust);
            this.gbGridStep.Controls.Add(this.cbxTempStep);
            this.gbGridStep.Location = new System.Drawing.Point(12, 170);
            this.gbGridStep.Name = "gbGridStep";
            this.gbGridStep.Size = new System.Drawing.Size(374, 100);
            this.gbGridStep.TabIndex = 8;
            this.gbGridStep.TabStop = false;
            this.gbGridStep.Text = "Grid step";
            //
            //lblSeconds2
            //
            this.lblSeconds2.Location = new System.Drawing.Point(156, 68);
            this.lblSeconds2.Name = "lblSeconds2";
            this.lblSeconds2.Size = new System.Drawing.Size(100, 13);
            this.lblSeconds2.TabIndex = 31;
            this.lblSeconds2.Text = "(seconds)";
            //
            //cbxTimeStep
            //
            this.cbxTimeStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeStep.FormattingEnabled = true;
            this.cbxTimeStep.Location = new System.Drawing.Point(96, 65);
            this.cbxTimeStep.Name = "cbxTimeStep";
            this.cbxTimeStep.Size = new System.Drawing.Size(54, 21);
            this.cbxTimeStep.TabIndex = 30;
            //
            //lblTime
            //
            this.lblTime.Location = new System.Drawing.Point(22, 68);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(68, 13);
            this.lblTime.TabIndex = 29;
            this.lblTime.Text = "Time";
            //
            //rdbPower
            //
            this.rdbPower.AutoSize = true;
            this.rdbPower.Location = new System.Drawing.Point(6, 43);
            this.rdbPower.Name = "rdbPower";
            this.rdbPower.Size = new System.Drawing.Size(55, 17);
            this.rdbPower.TabIndex = 28;
            this.rdbPower.TabStop = true;
            this.rdbPower.Text = "Power";
            this.rdbPower.UseVisualStyleBackColor = true;
            //
            //rdbTemperature
            //
            this.rdbTemperature.AutoSize = true;
            this.rdbTemperature.Location = new System.Drawing.Point(6, 20);
            this.rdbTemperature.Name = "rdbTemperature";
            this.rdbTemperature.Size = new System.Drawing.Size(85, 17);
            this.rdbTemperature.TabIndex = 27;
            this.rdbTemperature.TabStop = true;
            this.rdbTemperature.Text = "Temperature";
            this.rdbTemperature.UseVisualStyleBackColor = true;
            //
            //lblPwrAdjust
            //
            this.lblPwrAdjust.Location = new System.Drawing.Point(156, 41);
            this.lblPwrAdjust.Name = "lblPwrAdjust";
            this.lblPwrAdjust.Size = new System.Drawing.Size(80, 21);
            this.lblPwrAdjust.TabIndex = 26;
            this.lblPwrAdjust.Text = "lblPwrAdjust";
            this.lblPwrAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //cbxPwrStep
            //
            this.cbxPwrStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPwrStep.FormattingEnabled = true;
            this.cbxPwrStep.Location = new System.Drawing.Point(97, 42);
            this.cbxPwrStep.Name = "cbxPwrStep";
            this.cbxPwrStep.Size = new System.Drawing.Size(53, 21);
            this.cbxPwrStep.TabIndex = 25;
            //
            //lblTempAdjust
            //
            this.lblTempAdjust.Location = new System.Drawing.Point(156, 18);
            this.lblTempAdjust.Name = "lblTempAdjust";
            this.lblTempAdjust.Size = new System.Drawing.Size(80, 21);
            this.lblTempAdjust.TabIndex = 24;
            this.lblTempAdjust.Text = "lblTempAdjust";
            this.lblTempAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //cbxTempStep
            //
            this.cbxTempStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTempStep.FormattingEnabled = true;
            this.cbxTempStep.Location = new System.Drawing.Point(97, 19);
            this.cbxTempStep.Name = "cbxTempStep";
            this.cbxTempStep.Size = new System.Drawing.Size(53, 21);
            this.cbxTempStep.TabIndex = 23;
            //
            //rbAxisTempFahrenheit
            //
            this.rbAxisTempFahrenheit.AutoSize = true;
            this.rbAxisTempFahrenheit.Location = new System.Drawing.Point(145, 42);
            this.rbAxisTempFahrenheit.Name = "rbAxisTempFahrenheit";
            this.rbAxisTempFahrenheit.Size = new System.Drawing.Size(35, 17);
            this.rbAxisTempFahrenheit.TabIndex = 21;
            this.rbAxisTempFahrenheit.Text = "ºF";
            this.rbAxisTempFahrenheit.UseVisualStyleBackColor = true;
            //
            //rbAxisTempCelsius
            //
            this.rbAxisTempCelsius.AutoSize = true;
            this.rbAxisTempCelsius.Checked = true;
            this.rbAxisTempCelsius.Location = new System.Drawing.Point(144, 19);
            this.rbAxisTempCelsius.Name = "rbAxisTempCelsius";
            this.rbAxisTempCelsius.Size = new System.Drawing.Size(36, 17);
            this.rbAxisTempCelsius.TabIndex = 20;
            this.rbAxisTempCelsius.TabStop = true;
            this.rbAxisTempCelsius.Text = "ºC";
            this.rbAxisTempCelsius.UseVisualStyleBackColor = true;
            //
            //frmWizard2
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 309);
            this.Controls.Add(this.gbGridStep);
            this.Controls.Add(this.butPrev);
            this.Controls.Add(this.butFinish);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.gbxContent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Name = "frmWizard2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Wizard";
            this.gbxContent.ResumeLayout(false);
            this.gbxContent.PerformLayout();
            this.gbGridStep.ResumeLayout(false);
            this.gbGridStep.PerformLayout();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Label lblInfo;
        internal System.Windows.Forms.GroupBox gbxContent;
        internal System.Windows.Forms.Button butFinish;
        internal System.Windows.Forms.Button butPrev;
        internal System.Windows.Forms.ComboBox cbxTimeRange;
        internal System.Windows.Forms.ListBox lbxPower;
        internal System.Windows.Forms.ListBox lbxTemperature;
        internal System.Windows.Forms.GroupBox gbGridStep;
        internal System.Windows.Forms.ComboBox cbxTimeStep;
        internal System.Windows.Forms.Label lblTime;
        internal System.Windows.Forms.RadioButton rdbPower;
        internal System.Windows.Forms.RadioButton rdbTemperature;
        internal System.Windows.Forms.Label lblPwrAdjust;
        internal System.Windows.Forms.ComboBox cbxPwrStep;
        internal System.Windows.Forms.Label lblTempAdjust;
        internal System.Windows.Forms.ComboBox cbxTempStep;
        internal System.Windows.Forms.Label lblAxisTimeWindow;
        internal System.Windows.Forms.Label lblSeconds1;
        internal System.Windows.Forms.Label lblSeconds2;
        internal System.Windows.Forms.RadioButton rbAxisTempFahrenheit;
        internal System.Windows.Forms.RadioButton rbAxisTempCelsius;
    }
}
