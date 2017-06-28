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
    partial class frmWizard1 : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWizard1));
            this.butNext = new System.Windows.Forms.Button();
            this.Load += new System.EventHandler(frmWizard1_Load);
            this.butNext.Click += new System.EventHandler(this.butNext_Click);
            this.gbxContent = new System.Windows.Forms.GroupBox();
            this.butRemove = new System.Windows.Forms.Button();
            this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
            this.butAdd = new System.Windows.Forms.Button();
            this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
            this.lblPlotTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lbxSeries = new System.Windows.Forms.ListBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblMagnitude = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblStation = new System.Windows.Forms.Label();
            this.txtSetName = new System.Windows.Forms.TextBox();
            this.cbxStation = new System.Windows.Forms.ComboBox();
            this.cbxStation.DropDown += new System.EventHandler(this.cbxStation_DropDown);
            this.cbxStation.Click += new System.EventHandler(this.cbxStation_Click);
            this.cbxAxis = new System.Windows.Forms.ComboBox();
            this.cbxAxis.SelectedIndexChanged += new System.EventHandler(this.cbxAxis_SelectedIndexChanged);
            this.cbxPort = new System.Windows.Forms.ComboBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.gbxContent.SuspendLayout();
            this.SuspendLayout();
            //
            //butNext
            //
            this.butNext.Location = new System.Drawing.Point(283, 276);
            this.butNext.Name = "butNext";
            this.butNext.Size = new System.Drawing.Size(97, 23);
            this.butNext.TabIndex = 0;
            this.butNext.Text = "Next";
            this.butNext.UseVisualStyleBackColor = true;
            //
            //gbxContent
            //
            this.gbxContent.Controls.Add(this.butRemove);
            this.gbxContent.Controls.Add(this.butAdd);
            this.gbxContent.Controls.Add(this.lblPlotTitle);
            this.gbxContent.Controls.Add(this.txtTitle);
            this.gbxContent.Controls.Add(this.lbxSeries);
            this.gbxContent.Controls.Add(this.lblName);
            this.gbxContent.Controls.Add(this.lblMagnitude);
            this.gbxContent.Controls.Add(this.lblPort);
            this.gbxContent.Controls.Add(this.lblStation);
            this.gbxContent.Controls.Add(this.txtSetName);
            this.gbxContent.Controls.Add(this.cbxStation);
            this.gbxContent.Controls.Add(this.cbxAxis);
            this.gbxContent.Controls.Add(this.cbxPort);
            this.gbxContent.Location = new System.Drawing.Point(12, 74);
            this.gbxContent.Name = "gbxContent";
            this.gbxContent.Size = new System.Drawing.Size(374, 196);
            this.gbxContent.TabIndex = 2;
            this.gbxContent.TabStop = false;
            this.gbxContent.Text = "Series And Title";
            //
            //butRemove
            //
            this.butRemove.Location = new System.Drawing.Point(6, 120);
            this.butRemove.Name = "butRemove";
            this.butRemove.Size = new System.Drawing.Size(75, 23);
            this.butRemove.TabIndex = 23;
            this.butRemove.Text = "Remove";
            this.butRemove.UseVisualStyleBackColor = true;
            //
            //butAdd
            //
            this.butAdd.Location = new System.Drawing.Point(293, 126);
            this.butAdd.Name = "butAdd";
            this.butAdd.Size = new System.Drawing.Size(75, 23);
            this.butAdd.TabIndex = 21;
            this.butAdd.Text = "Add";
            this.butAdd.UseVisualStyleBackColor = true;
            //
            //lblPlotTitle
            //
            this.lblPlotTitle.AutoSize = true;
            this.lblPlotTitle.Location = new System.Drawing.Point(6, 152);
            this.lblPlotTitle.Name = "lblPlotTitle";
            this.lblPlotTitle.Size = new System.Drawing.Size(47, 13);
            this.lblPlotTitle.TabIndex = 20;
            this.lblPlotTitle.Text = "Plot title:";
            //
            //txtTitle
            //
            this.txtTitle.Location = new System.Drawing.Point(6, 168);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(362, 20);
            this.txtTitle.TabIndex = 19;
            //
            //lbxSeries
            //
            this.lbxSeries.FormattingEnabled = true;
            this.lbxSeries.Location = new System.Drawing.Point(6, 19);
            this.lbxSeries.Name = "lbxSeries";
            this.lbxSeries.Size = new System.Drawing.Size(120, 95);
            this.lbxSeries.TabIndex = 18;
            //
            //lblName
            //
            this.lblName.Location = new System.Drawing.Point(132, 103);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(57, 13);
            this.lblName.TabIndex = 17;
            this.lblName.Text = "Name";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //lblMagnitude
            //
            this.lblMagnitude.Location = new System.Drawing.Point(132, 76);
            this.lblMagnitude.Name = "lblMagnitude";
            this.lblMagnitude.Size = new System.Drawing.Size(57, 13);
            this.lblMagnitude.TabIndex = 16;
            this.lblMagnitude.Text = "Magnitude";
            this.lblMagnitude.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //lblPort
            //
            this.lblPort.Location = new System.Drawing.Point(132, 49);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(57, 13);
            this.lblPort.TabIndex = 15;
            this.lblPort.Text = "Port";
            this.lblPort.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //lblStation
            //
            this.lblStation.Location = new System.Drawing.Point(132, 22);
            this.lblStation.Name = "lblStation";
            this.lblStation.Size = new System.Drawing.Size(57, 13);
            this.lblStation.TabIndex = 14;
            this.lblStation.Text = "Station";
            this.lblStation.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //txtSetName
            //
            this.txtSetName.Location = new System.Drawing.Point(195, 100);
            this.txtSetName.Name = "txtSetName";
            this.txtSetName.Size = new System.Drawing.Size(173, 20);
            this.txtSetName.TabIndex = 10;
            //
            //cbxStation
            //
            this.cbxStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStation.FormattingEnabled = true;
            this.cbxStation.Location = new System.Drawing.Point(195, 19);
            this.cbxStation.Name = "cbxStation";
            this.cbxStation.Size = new System.Drawing.Size(173, 21);
            this.cbxStation.TabIndex = 13;
            //
            //cbxAxis
            //
            this.cbxAxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAxis.FormattingEnabled = true;
            this.cbxAxis.Items.AddRange(new object[] { "Temperature( ºC )", "Power( % )" });
            this.cbxAxis.Location = new System.Drawing.Point(195, 73);
            this.cbxAxis.Name = "cbxAxis";
            this.cbxAxis.Size = new System.Drawing.Size(173, 21);
            this.cbxAxis.TabIndex = 12;
            //
            //cbxPort
            //
            this.cbxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPort.FormattingEnabled = true;
            this.cbxPort.Items.AddRange(new object[] { "1", "2", "3", "4" });
            this.cbxPort.Location = new System.Drawing.Point(195, 46);
            this.cbxPort.Name = "cbxPort";
            this.cbxPort.Size = new System.Drawing.Size(173, 21);
            this.cbxPort.TabIndex = 11;
            //
            //lblInfo
            //
            this.lblInfo.Location = new System.Drawing.Point(12, 9);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(374, 62);
            this.lblInfo.TabIndex = 3;
            this.lblInfo.Text = "Label1";
            //
            //frmWizard1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 309);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.gbxContent);
            this.Controls.Add(this.butNext);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Name = "frmWizard1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Wizard";
            this.gbxContent.ResumeLayout(false);
            this.gbxContent.PerformLayout();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Button butNext;
        internal System.Windows.Forms.GroupBox gbxContent;
        internal System.Windows.Forms.Label lblInfo;
        internal System.Windows.Forms.Label lblName;
        internal System.Windows.Forms.Label lblMagnitude;
        internal System.Windows.Forms.Label lblPort;
        internal System.Windows.Forms.Label lblStation;
        internal System.Windows.Forms.TextBox txtSetName;
        internal System.Windows.Forms.ComboBox cbxStation;
        internal System.Windows.Forms.ComboBox cbxAxis;
        internal System.Windows.Forms.ComboBox cbxPort;
        internal System.Windows.Forms.ListBox lbxSeries;
        internal System.Windows.Forms.Button butAdd;
        internal System.Windows.Forms.Label lblPlotTitle;
        internal System.Windows.Forms.TextBox txtTitle;
        internal System.Windows.Forms.Button butRemove;
    }
}
