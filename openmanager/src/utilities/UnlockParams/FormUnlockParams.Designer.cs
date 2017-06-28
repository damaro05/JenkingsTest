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


namespace JBCAdvancedLevel
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class FormUnlockParams : System.Windows.Forms.Form
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
			base.Load += new System.EventHandler(FormUnlockParams_Load);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FormUnlockParams_FormClosing);
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUnlockParams));
			this.Label1 = new System.Windows.Forms.Label();
			this.Label1.DoubleClick += new System.EventHandler(this.Label1_DoubleClick);
			this.gridStations = new System.Windows.Forms.DataGridView();
			this.gridStations.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridStations_CellClick);
			this.gridStations.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridStations_CellMouseClick);
			this.timerStationListData = new System.Windows.Forms.Timer(this.components);
			this.timerStationListData.Tick += new System.EventHandler(this.timerStationListData_Tick);
			this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
			this.ListBox1 = new System.Windows.Forms.ListBox();
			this.colSelParametersLocked = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colModelType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colModelVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colSW = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colHW = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colProtocol = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colButLockUnlock = new System.Windows.Forms.DataGridViewButtonColumn();
			((System.ComponentModel.ISupportInitialize) this.gridStations).BeginInit();
			((System.ComponentModel.ISupportInitialize) this.SplitContainer1).BeginInit();
			this.SplitContainer1.Panel1.SuspendLayout();
			this.SplitContainer1.Panel2.SuspendLayout();
			this.SplitContainer1.SuspendLayout();
			this.SuspendLayout();
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(4, 9);
			this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(97, 16);
			this.Label1.TabIndex = 1;
			this.Label1.Text = "Stations Found";
			//
			//gridStations
			//
			this.gridStations.AllowUserToAddRows = false;
			this.gridStations.AllowUserToDeleteRows = false;
			this.gridStations.AllowUserToOrderColumns = true;
			this.gridStations.AllowUserToResizeRows = false;
			this.gridStations.Anchor = (System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.gridStations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridStations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {this.colSelParametersLocked, this.colID, this.colName, this.colModel, this.colModelType, this.colModelVersion, this.colCOM, this.colSW, this.colHW, this.colProtocol, this.colButLockUnlock});
			this.gridStations.Location = new System.Drawing.Point(4, 29);
			this.gridStations.Margin = new System.Windows.Forms.Padding(4);
			this.gridStations.MultiSelect = false;
			this.gridStations.Name = "gridStations";
			this.gridStations.RowHeadersVisible = false;
			this.gridStations.Size = new System.Drawing.Size(674, 115);
			this.gridStations.TabIndex = 0;
			//
			//timerStationListData
			//
			this.timerStationListData.Interval = 1000;
			//
			//SplitContainer1
			//
			this.SplitContainer1.Anchor = (System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.SplitContainer1.Location = new System.Drawing.Point(1, 2);
			this.SplitContainer1.Name = "SplitContainer1";
			this.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			//
			//SplitContainer1.Panel1
			//
			this.SplitContainer1.Panel1.Controls.Add(this.Label1);
			this.SplitContainer1.Panel1.Controls.Add(this.gridStations);
			//
			//SplitContainer1.Panel2
			//
			this.SplitContainer1.Panel2.Controls.Add(this.ListBox1);
			this.SplitContainer1.Size = new System.Drawing.Size(682, 243);
			this.SplitContainer1.SplitterDistance = 148;
			this.SplitContainer1.TabIndex = 2;
			//
			//ListBox1
			//
			this.ListBox1.Anchor = (System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.ListBox1.FormattingEnabled = true;
			this.ListBox1.ItemHeight = 16;
			this.ListBox1.Location = new System.Drawing.Point(3, 3);
			this.ListBox1.Name = "ListBox1";
			this.ListBox1.Size = new System.Drawing.Size(675, 84);
			this.ListBox1.TabIndex = 0;
			//
			//colSelParametersLocked
			//
			this.colSelParametersLocked.HeaderText = "Current Level";
			this.colSelParametersLocked.Name = "colSelParametersLocked";
			this.colSelParametersLocked.ReadOnly = true;
			this.colSelParametersLocked.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			//
			//colID
			//
			this.colID.HeaderText = "ID";
			this.colID.Name = "colID";
			this.colID.ReadOnly = true;
			this.colID.Visible = false;
			this.colID.Width = 50;
			//
			//colName
			//
			this.colName.HeaderText = "Name";
			this.colName.Name = "colName";
			this.colName.ReadOnly = true;
			//
			//colModel
			//
			this.colModel.HeaderText = "Model";
			this.colModel.Name = "colModel";
			this.colModel.ReadOnly = true;
			//
			//colModelType
			//
			this.colModelType.HeaderText = "Model Type";
			this.colModelType.Name = "colModelType";
			this.colModelType.ReadOnly = true;
			this.colModelType.Visible = false;
			//
			//colModelVersion
			//
			this.colModelVersion.HeaderText = "Model Version";
			this.colModelVersion.Name = "colModelVersion";
			this.colModelVersion.ReadOnly = true;
			this.colModelVersion.Visible = false;
			//
			//colCOM
			//
			this.colCOM.HeaderText = "Connection";
			this.colCOM.Name = "colCOM";
			this.colCOM.ReadOnly = true;
			this.colCOM.Visible = false;
			//
			//colSW
			//
			this.colSW.HeaderText = "SW";
			this.colSW.Name = "colSW";
			this.colSW.ReadOnly = true;
			//
			//colHW
			//
			this.colHW.HeaderText = "HW";
			this.colHW.Name = "colHW";
			this.colHW.ReadOnly = true;
			//
			//colProtocol
			//
			this.colProtocol.HeaderText = "Protocol";
			this.colProtocol.Name = "colProtocol";
			this.colProtocol.ReadOnly = true;
			this.colProtocol.Visible = false;
			//
			//colButLockUnlock
			//
			this.colButLockUnlock.HeaderText = "Basic/Advanced Level Setting";
			this.colButLockUnlock.Name = "colButLockUnlock";
			this.colButLockUnlock.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colButLockUnlock.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.colButLockUnlock.Width = 170;
			//
			//FormUnlockParams
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (8.0F), (float) (16.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(686, 248);
			this.Controls.Add(this.SplitContainer1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (9.75F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
			this.Icon = (System.Drawing.Icon) (resources.GetObject("$this.Icon"));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FormUnlockParams";
			this.Text = "JBC Advanced Level Configuration";
			((System.ComponentModel.ISupportInitialize) this.gridStations).EndInit();
			this.SplitContainer1.Panel1.ResumeLayout(false);
			this.SplitContainer1.Panel1.PerformLayout();
			this.SplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.SplitContainer1).EndInit();
			this.SplitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);
			
		}
		internal System.Windows.Forms.Timer timerStationListData;
		internal System.Windows.Forms.DataGridView gridStations;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.SplitContainer SplitContainer1;
		internal System.Windows.Forms.ListBox ListBox1;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colSelParametersLocked;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colID;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colName;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colModel;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colModelType;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colModelVersion;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colCOM;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colSW;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colHW;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colProtocol;
		internal System.Windows.Forms.DataGridViewButtonColumn colButLockUnlock;
		
	}
	
}
