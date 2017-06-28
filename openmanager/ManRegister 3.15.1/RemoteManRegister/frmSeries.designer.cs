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
    partial class frmSeries : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSeries));
            this.lbxSeries = new System.Windows.Forms.ListBox();
            base.Load += new System.EventHandler(frmSeries_Load);
            this.lbxSeries.SelectedIndexChanged += new System.EventHandler(this.lbxSeries_SelectedIndexChanged);
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmSeries_FormClosing);
            this.GroupBoxListOfSeries = new System.Windows.Forms.GroupBox();
            this.butRemove = new System.Windows.Forms.Button();
            this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
            this.GroupBoxSerieData = new System.Windows.Forms.GroupBox();
            this.butEdit = new System.Windows.Forms.Button();
            this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
            this.chbLine = new System.Windows.Forms.CheckBox();
            this.butColor = new System.Windows.Forms.Button();
            this.butColor.Click += new System.EventHandler(this.butColor_Click);
            this.butAdd = new System.Windows.Forms.Button();
            this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
            this.chbPoints = new System.Windows.Forms.CheckBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.pcbColor = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblMagnitude = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblStation = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.cbxStation = new System.Windows.Forms.ComboBox();
            this.cbxStation.DropDown += new System.EventHandler(this.cbxStation_DropDown);
            this.cbxAxis = new System.Windows.Forms.ComboBox();
            this.cbxAxis.SelectedIndexChanged += new System.EventHandler(this.cbxAxis_SelectedIndexChanged);
            this.cbxPort = new System.Windows.Forms.ComboBox();
            this.butOK = new System.Windows.Forms.Button();
            this.butOK.Click += new System.EventHandler(this.butOK_Click);
            this.butCancel = new System.Windows.Forms.Button();
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            this.clrColor = new System.Windows.Forms.ColorDialog();
            this.GroupBoxListOfSeries.SuspendLayout();
            this.GroupBoxSerieData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbColor).BeginInit();
            this.SuspendLayout();
            //
            //lbxSeries
            //
            this.lbxSeries.FormattingEnabled = true;
            this.lbxSeries.Location = new System.Drawing.Point(6, 19);
            this.lbxSeries.Name = "lbxSeries";
            this.lbxSeries.Size = new System.Drawing.Size(180, 160);
            this.lbxSeries.TabIndex = 0;
            //
            //GroupBoxListOfSeries
            //
            this.GroupBoxListOfSeries.Controls.Add(this.butRemove);
            this.GroupBoxListOfSeries.Controls.Add(this.lbxSeries);
            this.GroupBoxListOfSeries.Location = new System.Drawing.Point(12, 12);
            this.GroupBoxListOfSeries.Name = "GroupBoxListOfSeries";
            this.GroupBoxListOfSeries.Size = new System.Drawing.Size(194, 214);
            this.GroupBoxListOfSeries.TabIndex = 2;
            this.GroupBoxListOfSeries.TabStop = false;
            this.GroupBoxListOfSeries.Text = "List of series";
            //
            //butRemove
            //
            this.butRemove.Location = new System.Drawing.Point(6, 185);
            this.butRemove.Name = "butRemove";
            this.butRemove.Size = new System.Drawing.Size(75, 23);
            this.butRemove.TabIndex = 6;
            this.butRemove.Text = "Remove";
            this.butRemove.UseVisualStyleBackColor = true;
            //
            //GroupBoxSerieData
            //
            this.GroupBoxSerieData.Controls.Add(this.butEdit);
            this.GroupBoxSerieData.Controls.Add(this.chbLine);
            this.GroupBoxSerieData.Controls.Add(this.butColor);
            this.GroupBoxSerieData.Controls.Add(this.butAdd);
            this.GroupBoxSerieData.Controls.Add(this.chbPoints);
            this.GroupBoxSerieData.Controls.Add(this.lblColor);
            this.GroupBoxSerieData.Controls.Add(this.pcbColor);
            this.GroupBoxSerieData.Controls.Add(this.lblName);
            this.GroupBoxSerieData.Controls.Add(this.lblMagnitude);
            this.GroupBoxSerieData.Controls.Add(this.lblPort);
            this.GroupBoxSerieData.Controls.Add(this.lblStation);
            this.GroupBoxSerieData.Controls.Add(this.txtName);
            this.GroupBoxSerieData.Controls.Add(this.cbxStation);
            this.GroupBoxSerieData.Controls.Add(this.cbxAxis);
            this.GroupBoxSerieData.Controls.Add(this.cbxPort);
            this.GroupBoxSerieData.Location = new System.Drawing.Point(212, 12);
            this.GroupBoxSerieData.Name = "GroupBoxSerieData";
            this.GroupBoxSerieData.Size = new System.Drawing.Size(245, 214);
            this.GroupBoxSerieData.TabIndex = 3;
            this.GroupBoxSerieData.TabStop = false;
            this.GroupBoxSerieData.Text = "Serie data";
            //
            //butEdit
            //
            this.butEdit.Location = new System.Drawing.Point(6, 185);
            this.butEdit.Name = "butEdit";
            this.butEdit.Size = new System.Drawing.Size(83, 23);
            this.butEdit.TabIndex = 23;
            this.butEdit.Text = "Modify";
            this.butEdit.UseVisualStyleBackColor = true;
            this.butEdit.Visible = false;
            //
            //chbLine
            //
            this.chbLine.AutoSize = true;
            this.chbLine.Location = new System.Drawing.Point(143, 152);
            this.chbLine.Name = "chbLine";
            this.chbLine.Size = new System.Drawing.Size(72, 17);
            this.chbLine.TabIndex = 20;
            this.chbLine.Text = "Show line";
            this.chbLine.UseVisualStyleBackColor = true;
            //
            //butColor
            //
            this.butColor.Location = new System.Drawing.Point(154, 122);
            this.butColor.Name = "butColor";
            this.butColor.Size = new System.Drawing.Size(83, 22);
            this.butColor.TabIndex = 21;
            this.butColor.Text = "Select...";
            this.butColor.UseVisualStyleBackColor = true;
            //
            //butAdd
            //
            this.butAdd.Location = new System.Drawing.Point(156, 185);
            this.butAdd.Name = "butAdd";
            this.butAdd.Size = new System.Drawing.Size(83, 23);
            this.butAdd.TabIndex = 4;
            this.butAdd.Text = "Add";
            this.butAdd.UseVisualStyleBackColor = true;
            //
            //chbPoints
            //
            this.chbPoints.AutoSize = true;
            this.chbPoints.Location = new System.Drawing.Point(38, 152);
            this.chbPoints.Name = "chbPoints";
            this.chbPoints.Size = new System.Drawing.Size(84, 17);
            this.chbPoints.TabIndex = 19;
            this.chbPoints.Text = "Show points";
            this.chbPoints.UseVisualStyleBackColor = true;
            //
            //lblColor
            //
            this.lblColor.AutoSize = true;
            this.lblColor.Location = new System.Drawing.Point(6, 126);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(31, 13);
            this.lblColor.TabIndex = 18;
            this.lblColor.Text = "Color";
            //
            //pcbColor
            //
            this.pcbColor.Location = new System.Drawing.Point(83, 124);
            this.pcbColor.Name = "pcbColor";
            this.pcbColor.Size = new System.Drawing.Size(65, 18);
            this.pcbColor.TabIndex = 22;
            this.pcbColor.TabStop = false;
            //
            //lblName
            //
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 20);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 17;
            this.lblName.Text = "Name";
            //
            //lblMagnitude
            //
            this.lblMagnitude.AutoSize = true;
            this.lblMagnitude.Location = new System.Drawing.Point(6, 100);
            this.lblMagnitude.Name = "lblMagnitude";
            this.lblMagnitude.Size = new System.Drawing.Size(57, 13);
            this.lblMagnitude.TabIndex = 16;
            this.lblMagnitude.Text = "Magnitude";
            //
            //lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(6, 73);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 15;
            this.lblPort.Text = "Port";
            //
            //lblStation
            //
            this.lblStation.AutoSize = true;
            this.lblStation.Location = new System.Drawing.Point(6, 46);
            this.lblStation.Name = "lblStation";
            this.lblStation.Size = new System.Drawing.Size(40, 13);
            this.lblStation.TabIndex = 14;
            this.lblStation.Text = "Station";
            //
            //txtName
            //
            this.txtName.Location = new System.Drawing.Point(83, 17);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(154, 20);
            this.txtName.TabIndex = 10;
            //
            //cbxStation
            //
            this.cbxStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStation.FormattingEnabled = true;
            this.cbxStation.Location = new System.Drawing.Point(83, 43);
            this.cbxStation.Name = "cbxStation";
            this.cbxStation.Size = new System.Drawing.Size(154, 21);
            this.cbxStation.TabIndex = 13;
            //
            //cbxAxis
            //
            this.cbxAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAxis.FormattingEnabled = true;
            this.cbxAxis.Items.AddRange(new object[] { "Temperature( ºC )", "Power( % )" });
            this.cbxAxis.Location = new System.Drawing.Point(83, 97);
            this.cbxAxis.Name = "cbxAxis";
            this.cbxAxis.Size = new System.Drawing.Size(154, 21);
            this.cbxAxis.TabIndex = 12;
            //
            //cbxPort
            //
            this.cbxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPort.FormattingEnabled = true;
            this.cbxPort.Items.AddRange(new object[] { "1", "2", "3", "4" });
            this.cbxPort.Location = new System.Drawing.Point(83, 70);
            this.cbxPort.Name = "cbxPort";
            this.cbxPort.Size = new System.Drawing.Size(154, 21);
            this.cbxPort.TabIndex = 11;
            //
            //butOK
            //
            this.butOK.Location = new System.Drawing.Point(12, 234);
            this.butOK.Name = "butOK";
            this.butOK.Size = new System.Drawing.Size(75, 23);
            this.butOK.TabIndex = 7;
            this.butOK.Text = "Accept";
            this.butOK.UseVisualStyleBackColor = true;
            //
            //butCancel
            //
            this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butCancel.Location = new System.Drawing.Point(95, 234);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 23);
            this.butCancel.TabIndex = 8;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            //
            //frmSeries
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.butCancel;
            this.ClientSize = new System.Drawing.Size(465, 269);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butOK);
            this.Controls.Add(this.GroupBoxSerieData);
            this.Controls.Add(this.GroupBoxListOfSeries);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.MaximizeBox = false;
            this.Name = "frmSeries";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Series";
            this.GroupBoxListOfSeries.ResumeLayout(false);
            this.GroupBoxSerieData.ResumeLayout(false);
            this.GroupBoxSerieData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbColor).EndInit();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.ListBox lbxSeries;
        internal System.Windows.Forms.GroupBox GroupBoxListOfSeries;
        internal System.Windows.Forms.GroupBox GroupBoxSerieData;
        internal System.Windows.Forms.Button butAdd;
        internal System.Windows.Forms.Button butRemove;
        internal System.Windows.Forms.Label lblName;
        internal System.Windows.Forms.Label lblMagnitude;
        internal System.Windows.Forms.Label lblPort;
        internal System.Windows.Forms.Label lblStation;
        internal System.Windows.Forms.TextBox txtName;
        internal System.Windows.Forms.ComboBox cbxStation;
        internal System.Windows.Forms.ComboBox cbxAxis;
        internal System.Windows.Forms.ComboBox cbxPort;
        internal System.Windows.Forms.CheckBox chbLine;
        internal System.Windows.Forms.Button butColor;
        internal System.Windows.Forms.CheckBox chbPoints;
        internal System.Windows.Forms.Label lblColor;
        internal System.Windows.Forms.PictureBox pcbColor;
        internal System.Windows.Forms.Button butOK;
        internal System.Windows.Forms.Button butCancel;
        internal System.Windows.Forms.ColorDialog clrColor;
        internal System.Windows.Forms.Button butEdit;
    }
}
