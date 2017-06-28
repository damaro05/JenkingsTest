// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


namespace TestContinuousMode
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class FormContinuousMode : System.Windows.Forms.Form
	{
		
		//Form reemplaza a Dispose para limpiar la lista de componentes.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
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
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.Load += new System.EventHandler(Form1_Load);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormContinuousMode));
			this.lblStation = new System.Windows.Forms.Label();
			this.chbPort1 = new System.Windows.Forms.CheckBox();
			this.lblTemp1 = new System.Windows.Forms.Label();
			this.lblStatus1 = new System.Windows.Forms.Label();
			this.lblPwr1 = new System.Windows.Forms.Label();
			this.lblPwr2 = new System.Windows.Forms.Label();
			this.lblStatus2 = new System.Windows.Forms.Label();
			this.lblTemp2 = new System.Windows.Forms.Label();
			this.lblPwr3 = new System.Windows.Forms.Label();
			this.lblStatus3 = new System.Windows.Forms.Label();
			this.lblTemp3 = new System.Windows.Forms.Label();
			this.lblPwr4 = new System.Windows.Forms.Label();
			this.lblStatus4 = new System.Windows.Forms.Label();
			this.lblTemp4 = new System.Windows.Forms.Label();
			this.Label14 = new System.Windows.Forms.Label();
			this.Label15 = new System.Windows.Forms.Label();
			this.Label16 = new System.Windows.Forms.Label();
			this.chbPort4 = new System.Windows.Forms.CheckBox();
			this.chbPort3 = new System.Windows.Forms.CheckBox();
			this.chbPort2 = new System.Windows.Forms.CheckBox();
			this.cbxSpeed = new System.Windows.Forms.ComboBox();
			this.cbxSpeed.SelectedIndexChanged += new System.EventHandler(this.cbxSpeed_SelectedIndexChanged);
			this.CSpeedContinuousModeBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.butStart = new System.Windows.Forms.Button();
			this.butStart.Click += new System.EventHandler(this.butStart_Click);
			this.butStop = new System.Windows.Forms.Button();
			this.butStop.Click += new System.EventHandler(this.butStop_Click);
			this.lblSeq = new System.Windows.Forms.Label();
			this.lblTrans = new System.Windows.Forms.Label();
			this.cbxTick = new System.Windows.Forms.ComboBox();
			this.cbxTick.SelectedIndexChanged += new System.EventHandler(this.cbxTick_SelectedIndexChanged);
			this.Label1 = new System.Windows.Forms.Label();
			this.Label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize) this.CSpeedContinuousModeBindingSource).BeginInit();
			this.SuspendLayout();
			//
			//lblStation
			//
			this.lblStation.AutoSize = true;
			this.lblStation.Location = new System.Drawing.Point(13, 13);
			this.lblStation.Name = "lblStation";
			this.lblStation.Size = new System.Drawing.Size(120, 13);
			this.lblStation.TabIndex = 1;
			this.lblStation.Text = "Station: UNDETECTED";
			//
			//chbPort1
			//
			this.chbPort1.AutoSize = true;
			this.chbPort1.Location = new System.Drawing.Point(16, 59);
			this.chbPort1.Name = "chbPort1";
			this.chbPort1.Size = new System.Drawing.Size(51, 17);
			this.chbPort1.TabIndex = 2;
			this.chbPort1.Text = "Port1";
			this.chbPort1.UseVisualStyleBackColor = true;
			//
			//lblTemp1
			//
			this.lblTemp1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblTemp1.Location = new System.Drawing.Point(73, 56);
			this.lblTemp1.Name = "lblTemp1";
			this.lblTemp1.Size = new System.Drawing.Size(100, 20);
			this.lblTemp1.TabIndex = 3;
			this.lblTemp1.Text = "0 ºC";
			this.lblTemp1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblStatus1
			//
			this.lblStatus1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblStatus1.Location = new System.Drawing.Point(271, 56);
			this.lblStatus1.Name = "lblStatus1";
			this.lblStatus1.Size = new System.Drawing.Size(100, 20);
			this.lblStatus1.TabIndex = 4;
			this.lblStatus1.Text = "NONE";
			this.lblStatus1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblPwr1
			//
			this.lblPwr1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPwr1.Location = new System.Drawing.Point(172, 56);
			this.lblPwr1.Name = "lblPwr1";
			this.lblPwr1.Size = new System.Drawing.Size(100, 20);
			this.lblPwr1.TabIndex = 5;
			this.lblPwr1.Text = "0.0 %";
			this.lblPwr1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblPwr2
			//
			this.lblPwr2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPwr2.Location = new System.Drawing.Point(172, 79);
			this.lblPwr2.Name = "lblPwr2";
			this.lblPwr2.Size = new System.Drawing.Size(100, 20);
			this.lblPwr2.TabIndex = 8;
			this.lblPwr2.Text = "0.0 %";
			this.lblPwr2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblStatus2
			//
			this.lblStatus2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblStatus2.Location = new System.Drawing.Point(271, 79);
			this.lblStatus2.Name = "lblStatus2";
			this.lblStatus2.Size = new System.Drawing.Size(100, 20);
			this.lblStatus2.TabIndex = 7;
			this.lblStatus2.Text = "NONE";
			this.lblStatus2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblTemp2
			//
			this.lblTemp2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblTemp2.Location = new System.Drawing.Point(73, 79);
			this.lblTemp2.Name = "lblTemp2";
			this.lblTemp2.Size = new System.Drawing.Size(100, 20);
			this.lblTemp2.TabIndex = 6;
			this.lblTemp2.Text = "0 ºC";
			this.lblTemp2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblPwr3
			//
			this.lblPwr3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPwr3.Location = new System.Drawing.Point(172, 102);
			this.lblPwr3.Name = "lblPwr3";
			this.lblPwr3.Size = new System.Drawing.Size(100, 20);
			this.lblPwr3.TabIndex = 11;
			this.lblPwr3.Text = "0.0 %";
			this.lblPwr3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblStatus3
			//
			this.lblStatus3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblStatus3.Location = new System.Drawing.Point(271, 102);
			this.lblStatus3.Name = "lblStatus3";
			this.lblStatus3.Size = new System.Drawing.Size(100, 20);
			this.lblStatus3.TabIndex = 10;
			this.lblStatus3.Text = "NONE";
			this.lblStatus3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblTemp3
			//
			this.lblTemp3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblTemp3.Location = new System.Drawing.Point(73, 102);
			this.lblTemp3.Name = "lblTemp3";
			this.lblTemp3.Size = new System.Drawing.Size(100, 20);
			this.lblTemp3.TabIndex = 9;
			this.lblTemp3.Text = "0 ºC";
			this.lblTemp3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblPwr4
			//
			this.lblPwr4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPwr4.Location = new System.Drawing.Point(172, 125);
			this.lblPwr4.Name = "lblPwr4";
			this.lblPwr4.Size = new System.Drawing.Size(100, 20);
			this.lblPwr4.TabIndex = 14;
			this.lblPwr4.Text = "0.0 %";
			this.lblPwr4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblStatus4
			//
			this.lblStatus4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblStatus4.Location = new System.Drawing.Point(271, 125);
			this.lblStatus4.Name = "lblStatus4";
			this.lblStatus4.Size = new System.Drawing.Size(100, 20);
			this.lblStatus4.TabIndex = 13;
			this.lblStatus4.Text = "NONE";
			this.lblStatus4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblTemp4
			//
			this.lblTemp4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblTemp4.Location = new System.Drawing.Point(73, 125);
			this.lblTemp4.Name = "lblTemp4";
			this.lblTemp4.Size = new System.Drawing.Size(100, 20);
			this.lblTemp4.TabIndex = 12;
			this.lblTemp4.Text = "0 ºC";
			this.lblTemp4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label14
			//
			this.Label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Label14.Location = new System.Drawing.Point(172, 37);
			this.Label14.Name = "Label14";
			this.Label14.Size = new System.Drawing.Size(100, 20);
			this.Label14.TabIndex = 17;
			this.Label14.Text = "Power";
			this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label15
			//
			this.Label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Label15.Location = new System.Drawing.Point(271, 37);
			this.Label15.Name = "Label15";
			this.Label15.Size = new System.Drawing.Size(100, 20);
			this.Label15.TabIndex = 16;
			this.Label15.Text = "Status";
			this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label16
			//
			this.Label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Label16.Location = new System.Drawing.Point(73, 37);
			this.Label16.Name = "Label16";
			this.Label16.Size = new System.Drawing.Size(100, 20);
			this.Label16.TabIndex = 15;
			this.Label16.Text = "Temperature";
			this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//chbPort4
			//
			this.chbPort4.AutoSize = true;
			this.chbPort4.Location = new System.Drawing.Point(16, 128);
			this.chbPort4.Name = "chbPort4";
			this.chbPort4.Size = new System.Drawing.Size(51, 17);
			this.chbPort4.TabIndex = 18;
			this.chbPort4.Text = "Port4";
			this.chbPort4.UseVisualStyleBackColor = true;
			//
			//chbPort3
			//
			this.chbPort3.AutoSize = true;
			this.chbPort3.Location = new System.Drawing.Point(16, 105);
			this.chbPort3.Name = "chbPort3";
			this.chbPort3.Size = new System.Drawing.Size(51, 17);
			this.chbPort3.TabIndex = 19;
			this.chbPort3.Text = "Port3";
			this.chbPort3.UseVisualStyleBackColor = true;
			//
			//chbPort2
			//
			this.chbPort2.AutoSize = true;
			this.chbPort2.Location = new System.Drawing.Point(16, 82);
			this.chbPort2.Name = "chbPort2";
			this.chbPort2.Size = new System.Drawing.Size(51, 17);
			this.chbPort2.TabIndex = 20;
			this.chbPort2.Text = "Port2";
			this.chbPort2.UseVisualStyleBackColor = true;
			//
			//cbxSpeed
			//
			this.cbxSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxSpeed.FormattingEnabled = true;
			this.cbxSpeed.Location = new System.Drawing.Point(190, 180);
			this.cbxSpeed.Name = "cbxSpeed";
			this.cbxSpeed.Size = new System.Drawing.Size(100, 21);
			this.cbxSpeed.TabIndex = 21;
			//
			//CSpeedContinuousModeBindingSource
			//
			this.CSpeedContinuousModeBindingSource.DataSource = typeof(JBC_Connect.SpeedContinuousMode);
			//
			//butStart
			//
			this.butStart.Location = new System.Drawing.Point(296, 149);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(75, 23);
			this.butStart.TabIndex = 22;
			this.butStart.Text = "Start";
			this.butStart.UseVisualStyleBackColor = true;
			//
			//butStop
			//
			this.butStop.Location = new System.Drawing.Point(296, 178);
			this.butStop.Name = "butStop";
			this.butStop.Size = new System.Drawing.Size(75, 23);
			this.butStop.TabIndex = 23;
			this.butStop.Text = "Stop";
			this.butStop.UseVisualStyleBackColor = true;
			//
			//lblSeq
			//
			this.lblSeq.Location = new System.Drawing.Point(139, 9);
			this.lblSeq.Name = "lblSeq";
			this.lblSeq.Size = new System.Drawing.Size(126, 20);
			this.lblSeq.TabIndex = 24;
			this.lblSeq.Text = "Sequence: 0";
			this.lblSeq.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblTrans
			//
			this.lblTrans.Location = new System.Drawing.Point(271, 9);
			this.lblTrans.Name = "lblTrans";
			this.lblTrans.Size = new System.Drawing.Size(100, 20);
			this.lblTrans.TabIndex = 25;
			this.lblTrans.Text = "tck: 1000 / tr: 0";
			this.lblTrans.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//cbxTick
			//
			this.cbxTick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxTick.FormattingEnabled = true;
			this.cbxTick.Items.AddRange(new object[] {"1000", "500", "100", "50"});
			this.cbxTick.Location = new System.Drawing.Point(84, 180);
			this.cbxTick.Name = "cbxTick";
			this.cbxTick.Size = new System.Drawing.Size(100, 21);
			this.cbxTick.TabIndex = 26;
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(92, 159);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(86, 13);
			this.Label1.TabIndex = 27;
			this.Label1.Text = "Refresh tick (ms)";
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(196, 159);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(88, 13);
			this.Label2.TabIndex = 28;
			this.Label2.Text = "Trans period (ms)";
			//
			//Form1
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (6.0F), (float) (13.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(382, 211);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.cbxTick);
			this.Controls.Add(this.lblTrans);
			this.Controls.Add(this.lblSeq);
			this.Controls.Add(this.butStop);
			this.Controls.Add(this.butStart);
			this.Controls.Add(this.cbxSpeed);
			this.Controls.Add(this.chbPort2);
			this.Controls.Add(this.chbPort3);
			this.Controls.Add(this.chbPort4);
			this.Controls.Add(this.Label14);
			this.Controls.Add(this.Label15);
			this.Controls.Add(this.Label16);
			this.Controls.Add(this.lblPwr4);
			this.Controls.Add(this.lblStatus4);
			this.Controls.Add(this.lblTemp4);
			this.Controls.Add(this.lblPwr3);
			this.Controls.Add(this.lblStatus3);
			this.Controls.Add(this.lblTemp3);
			this.Controls.Add(this.lblPwr2);
			this.Controls.Add(this.lblStatus2);
			this.Controls.Add(this.lblTemp2);
			this.Controls.Add(this.lblPwr1);
			this.Controls.Add(this.lblStatus1);
			this.Controls.Add(this.lblTemp1);
			this.Controls.Add(this.chbPort1);
			this.Controls.Add(this.lblStation);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = (System.Drawing.Icon) (resources.GetObject("$this.Icon"));
			this.Name = "Form1";
			this.Text = "JBC - Continuous mode test";
			((System.ComponentModel.ISupportInitialize) this.CSpeedContinuousModeBindingSource).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
			
		}
		internal System.Windows.Forms.Label lblStation;
		internal System.Windows.Forms.CheckBox chbPort1;
		internal System.Windows.Forms.Label lblTemp1;
		internal System.Windows.Forms.Label lblStatus1;
		internal System.Windows.Forms.Label lblPwr1;
		internal System.Windows.Forms.Label lblPwr2;
		internal System.Windows.Forms.Label lblStatus2;
		internal System.Windows.Forms.Label lblTemp2;
		internal System.Windows.Forms.Label lblPwr3;
		internal System.Windows.Forms.Label lblStatus3;
		internal System.Windows.Forms.Label lblTemp3;
		internal System.Windows.Forms.Label lblPwr4;
		internal System.Windows.Forms.Label lblStatus4;
		internal System.Windows.Forms.Label lblTemp4;
		internal System.Windows.Forms.Label Label14;
		internal System.Windows.Forms.Label Label15;
		internal System.Windows.Forms.Label Label16;
		internal System.Windows.Forms.CheckBox chbPort4;
		internal System.Windows.Forms.CheckBox chbPort3;
		internal System.Windows.Forms.CheckBox chbPort2;
		internal System.Windows.Forms.ComboBox cbxSpeed;
		internal System.Windows.Forms.BindingSource CSpeedContinuousModeBindingSource;
		internal System.Windows.Forms.Button butStart;
		internal System.Windows.Forms.Button butStop;
		internal System.Windows.Forms.Label lblSeq;
		internal System.Windows.Forms.Label lblTrans;
		internal System.Windows.Forms.ComboBox cbxTick;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label Label2;
		
	}
	
}
