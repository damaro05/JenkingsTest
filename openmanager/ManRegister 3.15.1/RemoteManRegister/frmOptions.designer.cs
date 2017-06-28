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
    partial class frmOptions : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.lblTempAxisColor = new System.Windows.Forms.Label();
            base.Load += new System.EventHandler(frmOptions_Load);
            this.pcbTempClr = new System.Windows.Forms.PictureBox();
            this.butTempClr = new System.Windows.Forms.Button();
            this.butTempClr.Click += new System.EventHandler(this.butTempClr_Click);
            this.butPwrClr = new System.Windows.Forms.Button();
            this.butPwrClr.Click += new System.EventHandler(this.butPwrClr_Click);
            this.pcbPwrClr = new System.Windows.Forms.PictureBox();
            this.lblPowerAxisColor = new System.Windows.Forms.Label();
            this.butTimeClr = new System.Windows.Forms.Button();
            this.butTimeClr.Click += new System.EventHandler(this.butTimeClr_Click);
            this.pcbTimeClr = new System.Windows.Forms.PictureBox();
            this.lblTimeAxisColor = new System.Windows.Forms.Label();
            this.butGridClr = new System.Windows.Forms.Button();
            this.butGridClr.Click += new System.EventHandler(this.butGridClr_Click);
            this.pcbGridClr = new System.Windows.Forms.PictureBox();
            this.lblGridDivColor = new System.Windows.Forms.Label();
            this.butTextClr = new System.Windows.Forms.Button();
            this.butTextClr.Click += new System.EventHandler(this.butTextClr_Click);
            this.pcbTextClr = new System.Windows.Forms.PictureBox();
            this.lblSeriesTextColor = new System.Windows.Forms.Label();
            this.butBckGndClr = new System.Windows.Forms.Button();
            this.butBckGndClr.Click += new System.EventHandler(this.butBckGndClr_Click);
            this.pcbBckGndClr = new System.Windows.Forms.PictureBox();
            this.lblBackgroundColor = new System.Windows.Forms.Label();
            this.cdlgColor = new System.Windows.Forms.ColorDialog();
            this.butDefaultColor = new System.Windows.Forms.Button();
            this.butDefaultColor.Click += new System.EventHandler(this.butDefaultColor_Click);
            this.gbColors = new System.Windows.Forms.GroupBox();
            this.butTitleClr = new System.Windows.Forms.Button();
            this.butTitleClr.Click += new System.EventHandler(this.butTitleClr_Click);
            this.pcbTitleClr = new System.Windows.Forms.PictureBox();
            this.lblTitleTextColor = new System.Windows.Forms.Label();
            this.gbPlotStartSide = new System.Windows.Forms.GroupBox();
            this.rdbRightStart = new System.Windows.Forms.RadioButton();
            this.rdbRightStart.CheckedChanged += new System.EventHandler(this.rdbRightStart_CheckedChanged);
            this.rdbLeftStart = new System.Windows.Forms.RadioButton();
            this.rdbLeftStart.CheckedChanged += new System.EventHandler(this.rdbLeftStart_CheckedChanged);
            this.gbTriggerType = new System.Windows.Forms.GroupBox();
            this.lblTriggerAuto = new System.Windows.Forms.Label();
            this.lblTriggerAuto.Click += new System.EventHandler(this.Label14_Click);
            this.lblTriggerManual = new System.Windows.Forms.Label();
            this.lblTriggerSingle = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblStation = new System.Windows.Forms.Label();
            this.cbxPort = new System.Windows.Forms.ComboBox();
            this.cbxPort.SelectedIndexChanged += new System.EventHandler(this.cbxPort_SelectedIndexChanged);
            this.cbxStation = new System.Windows.Forms.ComboBox();
            this.cbxStation.SelectedIndexChanged += new System.EventHandler(this.cbxCOM_SelectedIndexChanged);
            this.cbxStation.SelectedValueChanged += new System.EventHandler(this.cbxStation_SelectedValueChanged);
            this.rdbTriggerManual = new System.Windows.Forms.RadioButton();
            this.rdbTriggerManual.CheckedChanged += new System.EventHandler(this.rdbTriggerManual_CheckedChanged);
            this.rdbTriggerSingle = new System.Windows.Forms.RadioButton();
            this.rdbTriggerSingle.CheckedChanged += new System.EventHandler(this.rdbTriggerSingle_CheckedChanged);
            this.rdbTriggerAuto = new System.Windows.Forms.RadioButton();
            this.rdbTriggerAuto.CheckedChanged += new System.EventHandler(this.rdbTriggerAuto_CheckedChanged);
            this.butApply = new System.Windows.Forms.Button();
            this.butApply.Click += new System.EventHandler(this.butApply_Click);
            this.butCancel = new System.Windows.Forms.Button();
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            this.gbPlotLinesAndPoints = new System.Windows.Forms.GroupBox();
            this.nudPointWidth = new System.Windows.Forms.NumericUpDown();
            this.nudPointWidth.ValueChanged += new System.EventHandler(this.nudPointWidth_ValueChanged);
            this.lblPointWidth = new System.Windows.Forms.Label();
            this.nudLineWidth = new System.Windows.Forms.NumericUpDown();
            this.nudLineWidth.ValueChanged += new System.EventHandler(this.nudLineWidth_ValueChanged);
            this.lblLineWidth = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)this.pcbTempClr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbPwrClr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbTimeClr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbGridClr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbTextClr).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbBckGndClr).BeginInit();
            this.gbColors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbTitleClr).BeginInit();
            this.gbPlotStartSide.SuspendLayout();
            this.gbTriggerType.SuspendLayout();
            this.gbPlotLinesAndPoints.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.nudPointWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.nudLineWidth).BeginInit();
            this.SuspendLayout();
            //
            //lblTempAxisColor
            //
            this.lblTempAxisColor.AutoSize = true;
            this.lblTempAxisColor.Location = new System.Drawing.Point(10, 28);
            this.lblTempAxisColor.Name = "lblTempAxisColor";
            this.lblTempAxisColor.Size = new System.Drawing.Size(88, 13);
            this.lblTempAxisColor.TabIndex = 0;
            this.lblTempAxisColor.Text = "Temperature axis";
            //
            //pcbTempClr
            //
            this.pcbTempClr.Location = new System.Drawing.Point(133, 19);
            this.pcbTempClr.Name = "pcbTempClr";
            this.pcbTempClr.Size = new System.Drawing.Size(33, 29);
            this.pcbTempClr.TabIndex = 1;
            this.pcbTempClr.TabStop = false;
            //
            //butTempClr
            //
            this.butTempClr.Location = new System.Drawing.Point(172, 23);
            this.butTempClr.Name = "butTempClr";
            this.butTempClr.Size = new System.Drawing.Size(75, 23);
            this.butTempClr.TabIndex = 2;
            this.butTempClr.Text = "Select...";
            this.butTempClr.UseVisualStyleBackColor = true;
            //
            //butPwrClr
            //
            this.butPwrClr.Location = new System.Drawing.Point(172, 58);
            this.butPwrClr.Name = "butPwrClr";
            this.butPwrClr.Size = new System.Drawing.Size(75, 23);
            this.butPwrClr.TabIndex = 5;
            this.butPwrClr.Text = "Select...";
            this.butPwrClr.UseVisualStyleBackColor = true;
            //
            //pcbPwrClr
            //
            this.pcbPwrClr.Location = new System.Drawing.Point(133, 54);
            this.pcbPwrClr.Name = "pcbPwrClr";
            this.pcbPwrClr.Size = new System.Drawing.Size(33, 29);
            this.pcbPwrClr.TabIndex = 4;
            this.pcbPwrClr.TabStop = false;
            //
            //lblPowerAxisColor
            //
            this.lblPowerAxisColor.AutoSize = true;
            this.lblPowerAxisColor.Location = new System.Drawing.Point(10, 63);
            this.lblPowerAxisColor.Name = "lblPowerAxisColor";
            this.lblPowerAxisColor.Size = new System.Drawing.Size(58, 13);
            this.lblPowerAxisColor.TabIndex = 3;
            this.lblPowerAxisColor.Text = "Power axis";
            //
            //butTimeClr
            //
            this.butTimeClr.Location = new System.Drawing.Point(172, 93);
            this.butTimeClr.Name = "butTimeClr";
            this.butTimeClr.Size = new System.Drawing.Size(75, 23);
            this.butTimeClr.TabIndex = 8;
            this.butTimeClr.Text = "Select...";
            this.butTimeClr.UseVisualStyleBackColor = true;
            //
            //pcbTimeClr
            //
            this.pcbTimeClr.Location = new System.Drawing.Point(133, 89);
            this.pcbTimeClr.Name = "pcbTimeClr";
            this.pcbTimeClr.Size = new System.Drawing.Size(33, 29);
            this.pcbTimeClr.TabIndex = 7;
            this.pcbTimeClr.TabStop = false;
            //
            //lblTimeAxisColor
            //
            this.lblTimeAxisColor.AutoSize = true;
            this.lblTimeAxisColor.Location = new System.Drawing.Point(10, 98);
            this.lblTimeAxisColor.Name = "lblTimeAxisColor";
            this.lblTimeAxisColor.Size = new System.Drawing.Size(51, 13);
            this.lblTimeAxisColor.TabIndex = 6;
            this.lblTimeAxisColor.Text = "Time axis";
            //
            //butGridClr
            //
            this.butGridClr.Location = new System.Drawing.Point(172, 128);
            this.butGridClr.Name = "butGridClr";
            this.butGridClr.Size = new System.Drawing.Size(75, 23);
            this.butGridClr.TabIndex = 11;
            this.butGridClr.Text = "Select...";
            this.butGridClr.UseVisualStyleBackColor = true;
            //
            //pcbGridClr
            //
            this.pcbGridClr.Location = new System.Drawing.Point(133, 124);
            this.pcbGridClr.Name = "pcbGridClr";
            this.pcbGridClr.Size = new System.Drawing.Size(33, 29);
            this.pcbGridClr.TabIndex = 10;
            this.pcbGridClr.TabStop = false;
            //
            //lblGridDivColor
            //
            this.lblGridDivColor.AutoSize = true;
            this.lblGridDivColor.Location = new System.Drawing.Point(10, 133);
            this.lblGridDivColor.Name = "lblGridDivColor";
            this.lblGridDivColor.Size = new System.Drawing.Size(69, 13);
            this.lblGridDivColor.TabIndex = 9;
            this.lblGridDivColor.Text = "Grid divisions";
            //
            //butTextClr
            //
            this.butTextClr.Location = new System.Drawing.Point(172, 163);
            this.butTextClr.Name = "butTextClr";
            this.butTextClr.Size = new System.Drawing.Size(75, 23);
            this.butTextClr.TabIndex = 14;
            this.butTextClr.Text = "Select...";
            this.butTextClr.UseVisualStyleBackColor = true;
            //
            //pcbTextClr
            //
            this.pcbTextClr.Location = new System.Drawing.Point(133, 159);
            this.pcbTextClr.Name = "pcbTextClr";
            this.pcbTextClr.Size = new System.Drawing.Size(33, 29);
            this.pcbTextClr.TabIndex = 13;
            this.pcbTextClr.TabStop = false;
            //
            //lblSeriesTextColor
            //
            this.lblSeriesTextColor.AutoSize = true;
            this.lblSeriesTextColor.Location = new System.Drawing.Point(10, 168);
            this.lblSeriesTextColor.Name = "lblSeriesTextColor";
            this.lblSeriesTextColor.Size = new System.Drawing.Size(56, 13);
            this.lblSeriesTextColor.TabIndex = 12;
            this.lblSeriesTextColor.Text = "Series text";
            //
            //butBckGndClr
            //
            this.butBckGndClr.Location = new System.Drawing.Point(172, 198);
            this.butBckGndClr.Name = "butBckGndClr";
            this.butBckGndClr.Size = new System.Drawing.Size(75, 23);
            this.butBckGndClr.TabIndex = 17;
            this.butBckGndClr.Text = "Select...";
            this.butBckGndClr.UseVisualStyleBackColor = true;
            //
            //pcbBckGndClr
            //
            this.pcbBckGndClr.Location = new System.Drawing.Point(133, 194);
            this.pcbBckGndClr.Name = "pcbBckGndClr";
            this.pcbBckGndClr.Size = new System.Drawing.Size(33, 29);
            this.pcbBckGndClr.TabIndex = 16;
            this.pcbBckGndClr.TabStop = false;
            //
            //lblBackgroundColor
            //
            this.lblBackgroundColor.AutoSize = true;
            this.lblBackgroundColor.Location = new System.Drawing.Point(10, 203);
            this.lblBackgroundColor.Name = "lblBackgroundColor";
            this.lblBackgroundColor.Size = new System.Drawing.Size(65, 13);
            this.lblBackgroundColor.TabIndex = 15;
            this.lblBackgroundColor.Text = "Background";
            //
            //butDefaultColor
            //
            this.butDefaultColor.Location = new System.Drawing.Point(27, 284);
            this.butDefaultColor.Name = "butDefaultColor";
            this.butDefaultColor.Size = new System.Drawing.Size(199, 28);
            this.butDefaultColor.TabIndex = 18;
            this.butDefaultColor.Text = "Restore defaults";
            this.butDefaultColor.UseVisualStyleBackColor = true;
            //
            //gbColors
            //
            this.gbColors.Controls.Add(this.butTitleClr);
            this.gbColors.Controls.Add(this.pcbTitleClr);
            this.gbColors.Controls.Add(this.lblTitleTextColor);
            this.gbColors.Controls.Add(this.butGridClr);
            this.gbColors.Controls.Add(this.butDefaultColor);
            this.gbColors.Controls.Add(this.lblTempAxisColor);
            this.gbColors.Controls.Add(this.butBckGndClr);
            this.gbColors.Controls.Add(this.pcbTempClr);
            this.gbColors.Controls.Add(this.pcbBckGndClr);
            this.gbColors.Controls.Add(this.butTempClr);
            this.gbColors.Controls.Add(this.lblBackgroundColor);
            this.gbColors.Controls.Add(this.lblPowerAxisColor);
            this.gbColors.Controls.Add(this.butTextClr);
            this.gbColors.Controls.Add(this.pcbPwrClr);
            this.gbColors.Controls.Add(this.pcbTextClr);
            this.gbColors.Controls.Add(this.butPwrClr);
            this.gbColors.Controls.Add(this.lblSeriesTextColor);
            this.gbColors.Controls.Add(this.lblTimeAxisColor);
            this.gbColors.Controls.Add(this.pcbTimeClr);
            this.gbColors.Controls.Add(this.pcbGridClr);
            this.gbColors.Controls.Add(this.butTimeClr);
            this.gbColors.Controls.Add(this.lblGridDivColor);
            this.gbColors.Location = new System.Drawing.Point(12, 12);
            this.gbColors.Name = "gbColors";
            this.gbColors.Size = new System.Drawing.Size(258, 328);
            this.gbColors.TabIndex = 19;
            this.gbColors.TabStop = false;
            this.gbColors.Text = "Colors";
            //
            //butTitleClr
            //
            this.butTitleClr.Location = new System.Drawing.Point(172, 233);
            this.butTitleClr.Name = "butTitleClr";
            this.butTitleClr.Size = new System.Drawing.Size(75, 23);
            this.butTitleClr.TabIndex = 21;
            this.butTitleClr.Text = "Select...";
            this.butTitleClr.UseVisualStyleBackColor = true;
            //
            //pcbTitleClr
            //
            this.pcbTitleClr.Location = new System.Drawing.Point(133, 229);
            this.pcbTitleClr.Name = "pcbTitleClr";
            this.pcbTitleClr.Size = new System.Drawing.Size(33, 29);
            this.pcbTitleClr.TabIndex = 20;
            this.pcbTitleClr.TabStop = false;
            //
            //lblTitleTextColor
            //
            this.lblTitleTextColor.AutoSize = true;
            this.lblTitleTextColor.Location = new System.Drawing.Point(10, 238);
            this.lblTitleTextColor.Name = "lblTitleTextColor";
            this.lblTitleTextColor.Size = new System.Drawing.Size(47, 13);
            this.lblTitleTextColor.TabIndex = 19;
            this.lblTitleTextColor.Text = "Title text";
            //
            //gbPlotStartSide
            //
            this.gbPlotStartSide.Controls.Add(this.rdbRightStart);
            this.gbPlotStartSide.Controls.Add(this.rdbLeftStart);
            this.gbPlotStartSide.Location = new System.Drawing.Point(277, 13);
            this.gbPlotStartSide.Name = "gbPlotStartSide";
            this.gbPlotStartSide.Size = new System.Drawing.Size(219, 62);
            this.gbPlotStartSide.TabIndex = 20;
            this.gbPlotStartSide.TabStop = false;
            this.gbPlotStartSide.Text = "Plot start side";
            //
            //rdbRightStart
            //
            this.rdbRightStart.AutoSize = true;
            this.rdbRightStart.Location = new System.Drawing.Point(7, 40);
            this.rdbRightStart.Name = "rdbRightStart";
            this.rdbRightStart.Size = new System.Drawing.Size(117, 17);
            this.rdbRightStart.TabIndex = 1;
            this.rdbRightStart.TabStop = true;
            this.rdbRightStart.Text = "Plot starts from right";
            this.rdbRightStart.UseVisualStyleBackColor = true;
            //
            //rdbLeftStart
            //
            this.rdbLeftStart.AutoSize = true;
            this.rdbLeftStart.Location = new System.Drawing.Point(7, 16);
            this.rdbLeftStart.Name = "rdbLeftStart";
            this.rdbLeftStart.Size = new System.Drawing.Size(111, 17);
            this.rdbLeftStart.TabIndex = 0;
            this.rdbLeftStart.TabStop = true;
            this.rdbLeftStart.Text = "Plot starts from left";
            this.rdbLeftStart.UseVisualStyleBackColor = true;
            //
            //gbTriggerType
            //
            this.gbTriggerType.Controls.Add(this.lblTriggerAuto);
            this.gbTriggerType.Controls.Add(this.lblTriggerManual);
            this.gbTriggerType.Controls.Add(this.lblTriggerSingle);
            this.gbTriggerType.Controls.Add(this.lblPort);
            this.gbTriggerType.Controls.Add(this.lblStation);
            this.gbTriggerType.Controls.Add(this.cbxPort);
            this.gbTriggerType.Controls.Add(this.cbxStation);
            this.gbTriggerType.Controls.Add(this.rdbTriggerManual);
            this.gbTriggerType.Controls.Add(this.rdbTriggerSingle);
            this.gbTriggerType.Controls.Add(this.rdbTriggerAuto);
            this.gbTriggerType.Location = new System.Drawing.Point(277, 81);
            this.gbTriggerType.Name = "gbTriggerType";
            this.gbTriggerType.Size = new System.Drawing.Size(219, 243);
            this.gbTriggerType.TabIndex = 21;
            this.gbTriggerType.TabStop = false;
            this.gbTriggerType.Text = "Trigger";
            //
            //lblTriggerAuto
            //
            this.lblTriggerAuto.Location = new System.Drawing.Point(27, 36);
            this.lblTriggerAuto.Name = "lblTriggerAuto";
            this.lblTriggerAuto.Size = new System.Drawing.Size(186, 28);
            this.lblTriggerAuto.TabIndex = 27;
            this.lblTriggerAuto.Text = "It starts when tool in hand";
            //
            //lblTriggerManual
            //
            this.lblTriggerManual.Location = new System.Drawing.Point(27, 150);
            this.lblTriggerManual.Name = "lblTriggerManual";
            this.lblTriggerManual.Size = new System.Drawing.Size(186, 30);
            this.lblTriggerManual.TabIndex = 26;
            this.lblTriggerManual.Text = "It starts and finishes by using the buttons";
            //
            //lblTriggerSingle
            //
            this.lblTriggerSingle.Location = new System.Drawing.Point(27, 86);
            this.lblTriggerSingle.Name = "lblTriggerSingle";
            this.lblTriggerSingle.Size = new System.Drawing.Size(186, 41);
            this.lblTriggerSingle.TabIndex = 25;
            this.lblTriggerSingle.Text = "It starts when tool in hand and finish when time window is reached";
            //
            //lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(7, 218);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 6;
            this.lblPort.Text = "Port";
            //
            //lblStation
            //
            this.lblStation.AutoSize = true;
            this.lblStation.Location = new System.Drawing.Point(6, 196);
            this.lblStation.Name = "lblStation";
            this.lblStation.Size = new System.Drawing.Size(40, 13);
            this.lblStation.TabIndex = 5;
            this.lblStation.Text = "Station";
            //
            //cbxPort
            //
            this.cbxPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPort.FormattingEnabled = true;
            this.cbxPort.Items.AddRange(new object[] { "1", "2", "3", "4" });
            this.cbxPort.Location = new System.Drawing.Point(65, 215);
            this.cbxPort.Name = "cbxPort";
            this.cbxPort.Size = new System.Drawing.Size(56, 21);
            this.cbxPort.TabIndex = 4;
            //
            //cbxStation
            //
            this.cbxStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStation.FormattingEnabled = true;
            this.cbxStation.Location = new System.Drawing.Point(65, 193);
            this.cbxStation.Name = "cbxStation";
            this.cbxStation.Size = new System.Drawing.Size(148, 21);
            this.cbxStation.TabIndex = 3;
            //
            //rdbTriggerManual
            //
            this.rdbTriggerManual.AutoSize = true;
            this.rdbTriggerManual.Location = new System.Drawing.Point(7, 130);
            this.rdbTriggerManual.Name = "rdbTriggerManual";
            this.rdbTriggerManual.Size = new System.Drawing.Size(63, 17);
            this.rdbTriggerManual.TabIndex = 2;
            this.rdbTriggerManual.TabStop = true;
            this.rdbTriggerManual.Text = "Manual:";
            this.rdbTriggerManual.UseVisualStyleBackColor = true;
            //
            //rdbTriggerSingle
            //
            this.rdbTriggerSingle.AutoSize = true;
            this.rdbTriggerSingle.Location = new System.Drawing.Point(7, 67);
            this.rdbTriggerSingle.Name = "rdbTriggerSingle";
            this.rdbTriggerSingle.Size = new System.Drawing.Size(57, 17);
            this.rdbTriggerSingle.TabIndex = 1;
            this.rdbTriggerSingle.TabStop = true;
            this.rdbTriggerSingle.Text = "Single:";
            this.rdbTriggerSingle.UseVisualStyleBackColor = true;
            //
            //rdbTriggerAuto
            //
            this.rdbTriggerAuto.AutoSize = true;
            this.rdbTriggerAuto.Location = new System.Drawing.Point(7, 17);
            this.rdbTriggerAuto.Name = "rdbTriggerAuto";
            this.rdbTriggerAuto.Size = new System.Drawing.Size(50, 17);
            this.rdbTriggerAuto.TabIndex = 0;
            this.rdbTriggerAuto.TabStop = true;
            this.rdbTriggerAuto.Text = "Auto:";
            this.rdbTriggerAuto.UseVisualStyleBackColor = true;
            //
            //butApply
            //
            this.butApply.Location = new System.Drawing.Point(12, 365);
            this.butApply.Name = "butApply";
            this.butApply.Size = new System.Drawing.Size(75, 23);
            this.butApply.TabIndex = 22;
            this.butApply.Text = "Accept";
            this.butApply.UseVisualStyleBackColor = true;
            //
            //butCancel
            //
            this.butCancel.Location = new System.Drawing.Point(93, 365);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 23);
            this.butCancel.TabIndex = 23;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            //
            //gbPlotLinesAndPoints
            //
            this.gbPlotLinesAndPoints.Controls.Add(this.nudPointWidth);
            this.gbPlotLinesAndPoints.Controls.Add(this.lblPointWidth);
            this.gbPlotLinesAndPoints.Controls.Add(this.nudLineWidth);
            this.gbPlotLinesAndPoints.Controls.Add(this.lblLineWidth);
            this.gbPlotLinesAndPoints.Location = new System.Drawing.Point(276, 330);
            this.gbPlotLinesAndPoints.Name = "gbPlotLinesAndPoints";
            this.gbPlotLinesAndPoints.Size = new System.Drawing.Size(219, 64);
            this.gbPlotLinesAndPoints.TabIndex = 24;
            this.gbPlotLinesAndPoints.TabStop = false;
            this.gbPlotLinesAndPoints.Text = "Plot lines and points";
            //
            //nudPointWidth
            //
            this.nudPointWidth.Location = new System.Drawing.Point(118, 38);
            this.nudPointWidth.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudPointWidth.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            this.nudPointWidth.Name = "nudPointWidth";
            this.nudPointWidth.Size = new System.Drawing.Size(48, 20);
            this.nudPointWidth.TabIndex = 3;
            this.nudPointWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudPointWidth.Value = new decimal(new int[] { 3, 0, 0, 0 });
            //
            //lblPointWidth
            //
            this.lblPointWidth.AutoSize = true;
            this.lblPointWidth.Location = new System.Drawing.Point(6, 40);
            this.lblPointWidth.Name = "lblPointWidth";
            this.lblPointWidth.Size = new System.Drawing.Size(81, 13);
            this.lblPointWidth.TabIndex = 2;
            this.lblPointWidth.Text = "Point width (pix)";
            //
            //nudLineWidth
            //
            this.nudLineWidth.Location = new System.Drawing.Point(118, 15);
            this.nudLineWidth.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.nudLineWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.nudLineWidth.Name = "nudLineWidth";
            this.nudLineWidth.Size = new System.Drawing.Size(48, 20);
            this.nudLineWidth.TabIndex = 1;
            this.nudLineWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nudLineWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            //lblLineWidth
            //
            this.lblLineWidth.AutoSize = true;
            this.lblLineWidth.Location = new System.Drawing.Point(6, 17);
            this.lblLineWidth.Name = "lblLineWidth";
            this.lblLineWidth.Size = new System.Drawing.Size(77, 13);
            this.lblLineWidth.TabIndex = 0;
            this.lblLineWidth.Text = "Line width (pix)";
            //
            //frmOptions
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 406);
            this.Controls.Add(this.gbPlotLinesAndPoints);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butApply);
            this.Controls.Add(this.gbTriggerType);
            this.Controls.Add(this.gbPlotStartSide);
            this.Controls.Add(this.gbColors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Name = "frmOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Options";
            ((System.ComponentModel.ISupportInitialize)this.pcbTempClr).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbPwrClr).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbTimeClr).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbGridClr).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbTextClr).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbBckGndClr).EndInit();
            this.gbColors.ResumeLayout(false);
            this.gbColors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbTitleClr).EndInit();
            this.gbPlotStartSide.ResumeLayout(false);
            this.gbPlotStartSide.PerformLayout();
            this.gbTriggerType.ResumeLayout(false);
            this.gbTriggerType.PerformLayout();
            this.gbPlotLinesAndPoints.ResumeLayout(false);
            this.gbPlotLinesAndPoints.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.nudPointWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.nudLineWidth).EndInit();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Label lblTempAxisColor;
        internal System.Windows.Forms.PictureBox pcbTempClr;
        internal System.Windows.Forms.Button butTempClr;
        internal System.Windows.Forms.Button butPwrClr;
        internal System.Windows.Forms.PictureBox pcbPwrClr;
        internal System.Windows.Forms.Label lblPowerAxisColor;
        internal System.Windows.Forms.Button butTimeClr;
        internal System.Windows.Forms.PictureBox pcbTimeClr;
        internal System.Windows.Forms.Label lblTimeAxisColor;
        internal System.Windows.Forms.Button butGridClr;
        internal System.Windows.Forms.PictureBox pcbGridClr;
        internal System.Windows.Forms.Label lblGridDivColor;
        internal System.Windows.Forms.Button butTextClr;
        internal System.Windows.Forms.PictureBox pcbTextClr;
        internal System.Windows.Forms.Label lblSeriesTextColor;
        internal System.Windows.Forms.Button butBckGndClr;
        internal System.Windows.Forms.PictureBox pcbBckGndClr;
        internal System.Windows.Forms.Label lblBackgroundColor;
        internal System.Windows.Forms.ColorDialog cdlgColor;
        internal System.Windows.Forms.Button butDefaultColor;
        internal System.Windows.Forms.GroupBox gbColors;
        internal System.Windows.Forms.GroupBox gbPlotStartSide;
        internal System.Windows.Forms.RadioButton rdbLeftStart;
        internal System.Windows.Forms.RadioButton rdbRightStart;
        internal System.Windows.Forms.GroupBox gbTriggerType;
        internal System.Windows.Forms.RadioButton rdbTriggerManual;
        internal System.Windows.Forms.RadioButton rdbTriggerSingle;
        internal System.Windows.Forms.RadioButton rdbTriggerAuto;
        internal System.Windows.Forms.Label lblPort;
        internal System.Windows.Forms.Label lblStation;
        internal System.Windows.Forms.ComboBox cbxPort;
        internal System.Windows.Forms.ComboBox cbxStation;
        internal System.Windows.Forms.Button butApply;
        internal System.Windows.Forms.Button butCancel;
        internal System.Windows.Forms.GroupBox gbPlotLinesAndPoints;
        internal System.Windows.Forms.Label lblLineWidth;
        internal System.Windows.Forms.NumericUpDown nudLineWidth;
        internal System.Windows.Forms.NumericUpDown nudPointWidth;
        internal System.Windows.Forms.Label lblPointWidth;
        internal System.Windows.Forms.Button butTitleClr;
        internal System.Windows.Forms.PictureBox pcbTitleClr;
        internal System.Windows.Forms.Label lblTitleTextColor;
        internal System.Windows.Forms.Label lblTriggerSingle;
        internal System.Windows.Forms.Label lblTriggerAuto;
        internal System.Windows.Forms.Label lblTriggerManual;
    }
}
