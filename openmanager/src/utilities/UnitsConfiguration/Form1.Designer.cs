// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using JBC_Connect;
// End of VB project level imports


namespace UnitsConfiguration
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class Form1 : System.Windows.Forms.Form
	{
		
		//Form overrides dispose to clean up the component list.
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
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			this.labelStationModel = new System.Windows.Forms.Label();
			base.Load += new System.EventHandler(Form1_Load);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
			m_jbc.NewStationConnected += new JBC_API.NewStationConnectedEventHandler(Connected);
			m_jbc.StationDisconnected += new JBC_API.StationDisconnectedEventHandler(Disconnected);
			this.labelTotalStationsTitle = new System.Windows.Forms.Label();
			this.labelTotalStations = new System.Windows.Forms.Label();
			this.labelTotalConfigurationsTitle = new System.Windows.Forms.Label();
			this.labelTotalConfigurations = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.imgStatus = new System.Windows.Forms.PictureBox();
			this.textBoxLog = new System.Windows.Forms.TextBox();
			this.labelUnitsChange = new System.Windows.Forms.Label();
			this.rbInternationalUnits = new System.Windows.Forms.RadioButton();
			this.rbInternationalUnits.CheckedChanged += new System.EventHandler(this.UnitsChange_CheckedChanged);
			this.rbAlternativeUnits = new System.Windows.Forms.RadioButton();
			this.rbAlternativeUnits.CheckedChanged += new System.EventHandler(this.UnitsChange_CheckedChanged);
			this.butResetCounters = new System.Windows.Forms.Button();
			this.butResetCounters.Click += new System.EventHandler(this.butResetCounters_Click);
			((System.ComponentModel.ISupportInitialize) this.imgStatus).BeginInit();
			this.SuspendLayout();
			//
			//labelStationModel
			//
			this.labelStationModel.AutoSize = true;
			this.labelStationModel.Font = new System.Drawing.Font("Verdana", (float) (18.0F), System.Drawing.FontStyle.Bold);
			this.labelStationModel.ForeColor = System.Drawing.Color.Red;
			this.labelStationModel.Location = new System.Drawing.Point(600, 40);
			this.labelStationModel.Name = "labelStationModel";
			this.labelStationModel.Size = new System.Drawing.Size(177, 29);
			this.labelStationModel.TabIndex = 1;
			this.labelStationModel.Text = "No conexión";
			//
			//labelTotalStationsTitle
			//
			this.labelTotalStationsTitle.AutoSize = true;
			this.labelTotalStationsTitle.Font = new System.Drawing.Font("Verdana", (float) (18.0F));
			this.labelTotalStationsTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.labelTotalStationsTitle.Location = new System.Drawing.Point(34, 348);
			this.labelTotalStationsTitle.Name = "labelTotalStationsTitle";
			this.labelTotalStationsTitle.Size = new System.Drawing.Size(294, 29);
			this.labelTotalStationsTitle.TabIndex = 2;
			this.labelTotalStationsTitle.Text = "Estaciones conectadas:";
			//
			//labelTotalStations
			//
			this.labelTotalStations.AutoSize = true;
			this.labelTotalStations.Font = new System.Drawing.Font("Verdana", (float) (18.0F));
			this.labelTotalStations.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.labelTotalStations.Location = new System.Drawing.Point(345, 348);
			this.labelTotalStations.Name = "labelTotalStations";
			this.labelTotalStations.Size = new System.Drawing.Size(28, 29);
			this.labelTotalStations.TabIndex = 3;
			this.labelTotalStations.Text = "0";
			//
			//labelTotalConfigurationsTitle
			//
			this.labelTotalConfigurationsTitle.AutoSize = true;
			this.labelTotalConfigurationsTitle.Font = new System.Drawing.Font("Verdana", (float) (18.0F));
			this.labelTotalConfigurationsTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.labelTotalConfigurationsTitle.Location = new System.Drawing.Point(34, 386);
			this.labelTotalConfigurationsTitle.Name = "labelTotalConfigurationsTitle";
			this.labelTotalConfigurationsTitle.Size = new System.Drawing.Size(215, 29);
			this.labelTotalConfigurationsTitle.TabIndex = 4;
			this.labelTotalConfigurationsTitle.Text = "Configuraciones:";
			//
			//labelTotalConfigurations
			//
			this.labelTotalConfigurations.AutoSize = true;
			this.labelTotalConfigurations.Font = new System.Drawing.Font("Verdana", (float) (18.0F));
			this.labelTotalConfigurations.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.labelTotalConfigurations.Location = new System.Drawing.Point(345, 386);
			this.labelTotalConfigurations.Name = "labelTotalConfigurations";
			this.labelTotalConfigurations.Size = new System.Drawing.Size(28, 29);
			this.labelTotalConfigurations.TabIndex = 5;
			this.labelTotalConfigurations.Text = "0";
			//
			//labelStatus
			//
			this.labelStatus.AutoSize = true;
			this.labelStatus.Font = new System.Drawing.Font("Verdana", (float) (28.0F), System.Drawing.FontStyle.Bold);
			this.labelStatus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.labelStatus.Location = new System.Drawing.Point(639, 94);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(74, 46);
			this.labelStatus.TabIndex = 6;
			this.labelStatus.Text = "---";
			this.labelStatus.Visible = false;
			//
			//imgStatus
			//
			this.imgStatus.Image = My.Resources.Resources.ok;
			this.imgStatus.Location = new System.Drawing.Point(605, 102);
			this.imgStatus.Name = "imgStatus";
			this.imgStatus.Size = new System.Drawing.Size(30, 30);
			this.imgStatus.TabIndex = 7;
			this.imgStatus.TabStop = false;
			this.imgStatus.Visible = false;
			//
			//textBoxLog
			//
			this.textBoxLog.Location = new System.Drawing.Point(605, 174);
			this.textBoxLog.Multiline = true;
			this.textBoxLog.Name = "textBoxLog";
			this.textBoxLog.ReadOnly = true;
			this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxLog.Size = new System.Drawing.Size(425, 241);
			this.textBoxLog.TabIndex = 8;
			//
			//labelUnitsChange
			//
			this.labelUnitsChange.Font = new System.Drawing.Font("Verdana", (float) (80.0F));
			this.labelUnitsChange.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.labelUnitsChange.Location = new System.Drawing.Point(40, 87);
			this.labelUnitsChange.Name = "labelUnitsChange";
			this.labelUnitsChange.Size = new System.Drawing.Size(531, 247);
			this.labelUnitsChange.TabIndex = 9;
			this.labelUnitsChange.Text = "ºC / mm";
			this.labelUnitsChange.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//rbInternationalUnits
			//
			this.rbInternationalUnits.AutoSize = true;
			this.rbInternationalUnits.Checked = true;
			this.rbInternationalUnits.Font = new System.Drawing.Font("Verdana", (float) (18.0F));
			this.rbInternationalUnits.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.rbInternationalUnits.Location = new System.Drawing.Point(39, 38);
			this.rbInternationalUnits.Name = "rbInternationalUnits";
			this.rbInternationalUnits.Size = new System.Drawing.Size(118, 33);
			this.rbInternationalUnits.TabIndex = 10;
			this.rbInternationalUnits.TabStop = true;
			this.rbInternationalUnits.Text = "ºC/mm";
			this.rbInternationalUnits.UseVisualStyleBackColor = true;
			//
			//rbAlternativeUnits
			//
			this.rbAlternativeUnits.AutoSize = true;
			this.rbAlternativeUnits.Font = new System.Drawing.Font("Verdana", (float) (18.0F));
			this.rbAlternativeUnits.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.rbAlternativeUnits.Location = new System.Drawing.Point(177, 38);
			this.rbAlternativeUnits.Name = "rbAlternativeUnits";
			this.rbAlternativeUnits.Size = new System.Drawing.Size(106, 33);
			this.rbAlternativeUnits.TabIndex = 11;
			this.rbAlternativeUnits.Text = "ºF / in";
			this.rbAlternativeUnits.UseVisualStyleBackColor = true;
			//
			//butResetCounters
			//
			this.butResetCounters.Cursor = System.Windows.Forms.Cursors.Default;
			this.butResetCounters.Font = new System.Drawing.Font("Verdana", (float) (10.0F));
			this.butResetCounters.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
			this.butResetCounters.Location = new System.Drawing.Point(420, 379);
			this.butResetCounters.Name = "butResetCounters";
			this.butResetCounters.Size = new System.Drawing.Size(151, 36);
			this.butResetCounters.TabIndex = 12;
			this.butResetCounters.Text = "Reset contadores";
			this.butResetCounters.UseVisualStyleBackColor = true;
			//
			//Form1
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (8.0F), (float) (14.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1055, 434);
			this.Controls.Add(this.butResetCounters);
			this.Controls.Add(this.rbAlternativeUnits);
			this.Controls.Add(this.rbInternationalUnits);
			this.Controls.Add(this.labelUnitsChange);
			this.Controls.Add(this.textBoxLog);
			this.Controls.Add(this.imgStatus);
			this.Controls.Add(this.labelStatus);
			this.Controls.Add(this.labelStationModel);
			this.Controls.Add(this.labelTotalConfigurations);
			this.Controls.Add(this.labelTotalConfigurationsTitle);
			this.Controls.Add(this.labelTotalStations);
			this.Controls.Add(this.labelTotalStationsTitle);
			this.Font = new System.Drawing.Font("Verdana", (float) (9.0F));
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "Form1";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Units configuration";
			((System.ComponentModel.ISupportInitialize) this.imgStatus).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
			
		}
		internal System.Windows.Forms.Label labelStationModel;
		internal System.Windows.Forms.Label labelTotalStationsTitle;
		internal System.Windows.Forms.Label labelTotalStations;
		internal System.Windows.Forms.Label labelTotalConfigurationsTitle;
		internal System.Windows.Forms.Label labelTotalConfigurations;
		internal System.Windows.Forms.Label labelStatus;
		internal System.Windows.Forms.PictureBox imgStatus;
		internal System.Windows.Forms.TextBox textBoxLog;
		internal System.Windows.Forms.Label labelUnitsChange;
		internal System.Windows.Forms.RadioButton rbInternationalUnits;
		internal System.Windows.Forms.RadioButton rbAlternativeUnits;
		internal System.Windows.Forms.Button butResetCounters;
		
	}
	
}
