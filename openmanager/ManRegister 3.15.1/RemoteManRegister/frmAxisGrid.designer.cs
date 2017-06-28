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
    partial class frmAxisGrid : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAxisGrid));
            this.gbxGrid = new System.Windows.Forms.GroupBox();
            base.Load += new System.EventHandler(frmAxisGrid_Load);
            this.lblSeconds2 = new System.Windows.Forms.Label();
            this.cbxTimeStep = new System.Windows.Forms.ComboBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.rdbPower = new System.Windows.Forms.RadioButton();
            this.rdbPower.CheckedChanged += new System.EventHandler(this.rdbAxisType_CheckedChanged);
            this.rdbTemperature = new System.Windows.Forms.RadioButton();
            this.rdbTemperature.CheckedChanged += new System.EventHandler(this.rdbAxisType_CheckedChanged);
            this.lblPwrAdjust = new System.Windows.Forms.Label();
            this.cbxPwrStep = new System.Windows.Forms.ComboBox();
            this.cbxPwrStep.SelectedIndexChanged += new System.EventHandler(this.cbxPwrStep_SelectedIndexChanged);
            this.lblTempAdjust = new System.Windows.Forms.Label();
            this.cbxTempStep = new System.Windows.Forms.ComboBox();
            this.cbxTempStep.SelectedIndexChanged += new System.EventHandler(this.cbxTempStep_SelectedIndexChanged);
            this.gbxAxis = new System.Windows.Forms.GroupBox();
            this.rbAxisTempFahrenheit = new System.Windows.Forms.RadioButton();
            this.rbAxisTempFahrenheit.CheckedChanged += new System.EventHandler(this.rbAxisTemp_CheckedChanged);
            this.rbAxisTempCelsius = new System.Windows.Forms.RadioButton();
            this.rbAxisTempCelsius.CheckedChanged += new System.EventHandler(this.rbAxisTemp_CheckedChanged);
            this.lblSeconds1 = new System.Windows.Forms.Label();
            this.cbxTimeRange = new System.Windows.Forms.ComboBox();
            this.lblAxisTimeWindow = new System.Windows.Forms.Label();
            this.butAddPwr = new System.Windows.Forms.Button();
            this.butAddPwr.Click += new System.EventHandler(this.butAddPwr_Click);
            this.cbxPwrMax = new System.Windows.Forms.ComboBox();
            this.cbxPwrMin = new System.Windows.Forms.ComboBox();
            this.butAddTemp = new System.Windows.Forms.Button();
            this.butAddTemp.Click += new System.EventHandler(this.butAddTemp_Click);
            this.cbxTempMax = new System.Windows.Forms.ComboBox();
            this.cbxTempMin = new System.Windows.Forms.ComboBox();
            this.lblAxisPower = new System.Windows.Forms.Label();
            this.lblAxisTemperature = new System.Windows.Forms.Label();
            this.lbxPower = new System.Windows.Forms.ListBox();
            this.lbxPower.SelectedIndexChanged += new System.EventHandler(this.lbxPower_SelectedIndexChanged);
            this.lbxTemperature = new System.Windows.Forms.ListBox();
            this.lbxTemperature.SelectedIndexChanged += new System.EventHandler(this.lbxTemperature_SelectedIndexChanged);
            this.butOK = new System.Windows.Forms.Button();
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            this.butCancel = new System.Windows.Forms.Button();
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            this.gbxGrid.SuspendLayout();
            this.gbxAxis.SuspendLayout();
            this.SuspendLayout();
            //
            //gbxGrid
            //
            this.gbxGrid.Controls.Add(this.lblSeconds2);
            this.gbxGrid.Controls.Add(this.cbxTimeStep);
            this.gbxGrid.Controls.Add(this.lblTime);
            this.gbxGrid.Controls.Add(this.rdbPower);
            this.gbxGrid.Controls.Add(this.rdbTemperature);
            this.gbxGrid.Controls.Add(this.lblPwrAdjust);
            this.gbxGrid.Controls.Add(this.cbxPwrStep);
            this.gbxGrid.Controls.Add(this.lblTempAdjust);
            this.gbxGrid.Controls.Add(this.cbxTempStep);
            this.gbxGrid.Location = new System.Drawing.Point(14, 240);
            this.gbxGrid.Name = "gbxGrid";
            this.gbxGrid.Size = new System.Drawing.Size(251, 101);
            this.gbxGrid.TabIndex = 3;
            this.gbxGrid.TabStop = false;
            this.gbxGrid.Text = "Grid";
            //
            //lblSeconds2
            //
            this.lblSeconds2.Location = new System.Drawing.Point(160, 70);
            this.lblSeconds2.Name = "lblSeconds2";
            this.lblSeconds2.Size = new System.Drawing.Size(85, 13);
            this.lblSeconds2.TabIndex = 18;
            this.lblSeconds2.Text = "(seconds)";
            //
            //cbxTimeStep
            //
            this.cbxTimeStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeStep.FormattingEnabled = true;
            this.cbxTimeStep.Location = new System.Drawing.Point(100, 67);
            this.cbxTimeStep.Name = "cbxTimeStep";
            this.cbxTimeStep.Size = new System.Drawing.Size(54, 21);
            this.cbxTimeStep.TabIndex = 12;
            //
            //lblTime
            //
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(26, 70);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(30, 13);
            this.lblTime.TabIndex = 11;
            this.lblTime.Text = "Time";
            //
            //rdbPower
            //
            this.rdbPower.AutoSize = true;
            this.rdbPower.Location = new System.Drawing.Point(10, 43);
            this.rdbPower.Name = "rdbPower";
            this.rdbPower.Size = new System.Drawing.Size(55, 17);
            this.rdbPower.TabIndex = 5;
            this.rdbPower.TabStop = true;
            this.rdbPower.Text = "Power";
            this.rdbPower.UseVisualStyleBackColor = true;
            //
            //rdbTemperature
            //
            this.rdbTemperature.AutoSize = true;
            this.rdbTemperature.Location = new System.Drawing.Point(10, 19);
            this.rdbTemperature.Name = "rdbTemperature";
            this.rdbTemperature.Size = new System.Drawing.Size(85, 17);
            this.rdbTemperature.TabIndex = 4;
            this.rdbTemperature.TabStop = true;
            this.rdbTemperature.Text = "Temperature";
            this.rdbTemperature.UseVisualStyleBackColor = true;
            //
            //lblPwrAdjust
            //
            this.lblPwrAdjust.Location = new System.Drawing.Point(160, 41);
            this.lblPwrAdjust.Name = "lblPwrAdjust";
            this.lblPwrAdjust.Size = new System.Drawing.Size(80, 21);
            this.lblPwrAdjust.TabIndex = 3;
            this.lblPwrAdjust.Text = "lblPwrAdjust";
            this.lblPwrAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //cbxPwrStep
            //
            this.cbxPwrStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPwrStep.FormattingEnabled = true;
            this.cbxPwrStep.Location = new System.Drawing.Point(101, 42);
            this.cbxPwrStep.Name = "cbxPwrStep";
            this.cbxPwrStep.Size = new System.Drawing.Size(53, 21);
            this.cbxPwrStep.TabIndex = 2;
            //
            //lblTempAdjust
            //
            this.lblTempAdjust.Location = new System.Drawing.Point(160, 17);
            this.lblTempAdjust.Name = "lblTempAdjust";
            this.lblTempAdjust.Size = new System.Drawing.Size(80, 21);
            this.lblTempAdjust.TabIndex = 1;
            this.lblTempAdjust.Text = "lblTempAdjust";
            this.lblTempAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //cbxTempStep
            //
            this.cbxTempStep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTempStep.FormattingEnabled = true;
            this.cbxTempStep.Location = new System.Drawing.Point(101, 18);
            this.cbxTempStep.Name = "cbxTempStep";
            this.cbxTempStep.Size = new System.Drawing.Size(53, 21);
            this.cbxTempStep.TabIndex = 0;
            //
            //gbxAxis
            //
            this.gbxAxis.Controls.Add(this.rbAxisTempFahrenheit);
            this.gbxAxis.Controls.Add(this.rbAxisTempCelsius);
            this.gbxAxis.Controls.Add(this.lblSeconds1);
            this.gbxAxis.Controls.Add(this.cbxTimeRange);
            this.gbxAxis.Controls.Add(this.lblAxisTimeWindow);
            this.gbxAxis.Controls.Add(this.butAddPwr);
            this.gbxAxis.Controls.Add(this.cbxPwrMax);
            this.gbxAxis.Controls.Add(this.cbxPwrMin);
            this.gbxAxis.Controls.Add(this.butAddTemp);
            this.gbxAxis.Controls.Add(this.cbxTempMax);
            this.gbxAxis.Controls.Add(this.cbxTempMin);
            this.gbxAxis.Controls.Add(this.lblAxisPower);
            this.gbxAxis.Controls.Add(this.lblAxisTemperature);
            this.gbxAxis.Controls.Add(this.lbxPower);
            this.gbxAxis.Controls.Add(this.lbxTemperature);
            this.gbxAxis.Location = new System.Drawing.Point(14, 11);
            this.gbxAxis.Name = "gbxAxis";
            this.gbxAxis.Size = new System.Drawing.Size(379, 223);
            this.gbxAxis.TabIndex = 2;
            this.gbxAxis.TabStop = false;
            this.gbxAxis.Text = "Axis";
            //
            //rbAxisTempFahrenheit
            //
            this.rbAxisTempFahrenheit.AutoSize = true;
            this.rbAxisTempFahrenheit.Font = new System.Drawing.Font("Verdana", (float)(8.25F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.rbAxisTempFahrenheit.Location = new System.Drawing.Point(138, 13);
            this.rbAxisTempFahrenheit.Name = "rbAxisTempFahrenheit";
            this.rbAxisTempFahrenheit.Size = new System.Drawing.Size(37, 17);
            this.rbAxisTempFahrenheit.TabIndex = 19;
            this.rbAxisTempFahrenheit.Text = "ºF";
            this.rbAxisTempFahrenheit.UseVisualStyleBackColor = true;
            //
            //rbAxisTempCelsius
            //
            this.rbAxisTempCelsius.AutoSize = true;
            this.rbAxisTempCelsius.Checked = true;
            this.rbAxisTempCelsius.Font = new System.Drawing.Font("Verdana", (float)(8.25F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.rbAxisTempCelsius.Location = new System.Drawing.Point(90, 13);
            this.rbAxisTempCelsius.Name = "rbAxisTempCelsius";
            this.rbAxisTempCelsius.Size = new System.Drawing.Size(40, 17);
            this.rbAxisTempCelsius.TabIndex = 18;
            this.rbAxisTempCelsius.TabStop = true;
            this.rbAxisTempCelsius.Text = "ºC";
            this.rbAxisTempCelsius.UseVisualStyleBackColor = true;
            //
            //lblSeconds1
            //
            this.lblSeconds1.Location = new System.Drawing.Point(210, 190);
            this.lblSeconds1.Name = "lblSeconds1";
            this.lblSeconds1.Size = new System.Drawing.Size(100, 13);
            this.lblSeconds1.TabIndex = 17;
            this.lblSeconds1.Text = "(seconds)";
            //
            //cbxTimeRange
            //
            this.cbxTimeRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTimeRange.FormattingEnabled = true;
            this.cbxTimeRange.Location = new System.Drawing.Point(144, 187);
            this.cbxTimeRange.Name = "cbxTimeRange";
            this.cbxTimeRange.Size = new System.Drawing.Size(60, 21);
            this.cbxTimeRange.TabIndex = 11;
            //
            //lblAxisTimeWindow
            //
            this.lblAxisTimeWindow.Location = new System.Drawing.Point(22, 190);
            this.lblAxisTimeWindow.Name = "lblAxisTimeWindow";
            this.lblAxisTimeWindow.Size = new System.Drawing.Size(116, 13);
            this.lblAxisTimeWindow.TabIndex = 10;
            this.lblAxisTimeWindow.Text = "Time Window";
            this.lblAxisTimeWindow.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //butAddPwr
            //
            this.butAddPwr.Location = new System.Drawing.Point(320, 137);
            this.butAddPwr.Name = "butAddPwr";
            this.butAddPwr.Size = new System.Drawing.Size(53, 23);
            this.butAddPwr.TabIndex = 9;
            this.butAddPwr.Text = "Add";
            this.butAddPwr.UseVisualStyleBackColor = true;
            //
            //cbxPwrMax
            //
            this.cbxPwrMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPwrMax.FormattingEnabled = true;
            this.cbxPwrMax.Location = new System.Drawing.Point(256, 137);
            this.cbxPwrMax.Name = "cbxPwrMax";
            this.cbxPwrMax.Size = new System.Drawing.Size(58, 21);
            this.cbxPwrMax.TabIndex = 8;
            //
            //cbxPwrMin
            //
            this.cbxPwrMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPwrMin.FormattingEnabled = true;
            this.cbxPwrMin.Location = new System.Drawing.Point(192, 137);
            this.cbxPwrMin.Name = "cbxPwrMin";
            this.cbxPwrMin.Size = new System.Drawing.Size(58, 21);
            this.cbxPwrMin.TabIndex = 7;
            //
            //butAddTemp
            //
            this.butAddTemp.Location = new System.Drawing.Point(133, 137);
            this.butAddTemp.Name = "butAddTemp";
            this.butAddTemp.Size = new System.Drawing.Size(53, 23);
            this.butAddTemp.TabIndex = 6;
            this.butAddTemp.Text = "Add";
            this.butAddTemp.UseVisualStyleBackColor = true;
            //
            //cbxTempMax
            //
            this.cbxTempMax.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTempMax.FormattingEnabled = true;
            this.cbxTempMax.Location = new System.Drawing.Point(71, 137);
            this.cbxTempMax.Name = "cbxTempMax";
            this.cbxTempMax.Size = new System.Drawing.Size(58, 21);
            this.cbxTempMax.TabIndex = 5;
            //
            //cbxTempMin
            //
            this.cbxTempMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTempMin.FormattingEnabled = true;
            this.cbxTempMin.Location = new System.Drawing.Point(10, 137);
            this.cbxTempMin.Name = "cbxTempMin";
            this.cbxTempMin.Size = new System.Drawing.Size(58, 21);
            this.cbxTempMin.TabIndex = 4;
            //
            //lblAxisPower
            //
            this.lblAxisPower.AutoSize = true;
            this.lblAxisPower.Location = new System.Drawing.Point(194, 17);
            this.lblAxisPower.Name = "lblAxisPower";
            this.lblAxisPower.Size = new System.Drawing.Size(37, 13);
            this.lblAxisPower.TabIndex = 3;
            this.lblAxisPower.Text = "Power";
            //
            //lblAxisTemperature
            //
            this.lblAxisTemperature.AutoSize = true;
            this.lblAxisTemperature.Location = new System.Drawing.Point(7, 16);
            this.lblAxisTemperature.Name = "lblAxisTemperature";
            this.lblAxisTemperature.Size = new System.Drawing.Size(67, 13);
            this.lblAxisTemperature.TabIndex = 2;
            this.lblAxisTemperature.Text = "Temperature";
            //
            //lbxPower
            //
            this.lbxPower.FormattingEnabled = true;
            this.lbxPower.Location = new System.Drawing.Point(191, 36);
            this.lbxPower.Name = "lbxPower";
            this.lbxPower.Size = new System.Drawing.Size(182, 95);
            this.lbxPower.TabIndex = 1;
            //
            //lbxTemperature
            //
            this.lbxTemperature.FormattingEnabled = true;
            this.lbxTemperature.Location = new System.Drawing.Point(10, 36);
            this.lbxTemperature.Name = "lbxTemperature";
            this.lbxTemperature.Size = new System.Drawing.Size(175, 95);
            this.lbxTemperature.TabIndex = 0;
            //
            //butOK
            //
            this.butOK.Location = new System.Drawing.Point(309, 289);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 23);
            this.butOK.TabIndex = 4;
            this.butOK.Text = "OK";
            this.butOK.UseVisualStyleBackColor = true;
            //
            //butCancel
            //
            this.butCancel.Location = new System.Drawing.Point(309, 318);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 23);
            this.butCancel.TabIndex = 5;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            //
            //frmAxisGrid
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 356);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.gbxGrid);
            this.Controls.Add(this.gbxAxis);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.MaximizeBox = false;
            this.Name = "frmAxisGrid";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Axis & Grid";
            this.gbxGrid.ResumeLayout(false);
            this.gbxGrid.PerformLayout();
            this.gbxAxis.ResumeLayout(false);
            this.gbxAxis.PerformLayout();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.GroupBox gbxGrid;
        internal System.Windows.Forms.Label lblPwrAdjust;
        internal System.Windows.Forms.ComboBox cbxPwrStep;
        internal System.Windows.Forms.Label lblTempAdjust;
        internal System.Windows.Forms.ComboBox cbxTempStep;
        internal System.Windows.Forms.GroupBox gbxAxis;
        internal System.Windows.Forms.Button butAddPwr;
        internal System.Windows.Forms.ComboBox cbxPwrMax;
        internal System.Windows.Forms.ComboBox cbxPwrMin;
        internal System.Windows.Forms.Button butAddTemp;
        internal System.Windows.Forms.ComboBox cbxTempMax;
        internal System.Windows.Forms.ComboBox cbxTempMin;
        internal System.Windows.Forms.Label lblAxisPower;
        internal System.Windows.Forms.Label lblAxisTemperature;
        internal System.Windows.Forms.ListBox lbxPower;
        internal System.Windows.Forms.ListBox lbxTemperature;
        internal System.Windows.Forms.Button butOK;
        internal System.Windows.Forms.Button butCancel;
        internal System.Windows.Forms.RadioButton rdbPower;
        internal System.Windows.Forms.RadioButton rdbTemperature;
        internal System.Windows.Forms.Label lblAxisTimeWindow;
        internal System.Windows.Forms.ComboBox cbxTimeStep;
        internal System.Windows.Forms.Label lblTime;
        internal System.Windows.Forms.ComboBox cbxTimeRange;
        internal System.Windows.Forms.Label lblSeconds2;
        internal System.Windows.Forms.Label lblSeconds1;
        internal System.Windows.Forms.RadioButton rbAxisTempFahrenheit;
        internal System.Windows.Forms.RadioButton rbAxisTempCelsius;
    }
}
