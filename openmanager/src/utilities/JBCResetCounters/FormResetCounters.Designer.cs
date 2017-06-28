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


namespace JBCResetCounters
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class FormResetCounters : System.Windows.Forms.Form
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
			base.Load += new System.EventHandler(FormResetCounters_Load);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FormResetCounters_FormClosing);
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormResetCounters));
			this.labStationsFound = new System.Windows.Forms.Label();
			this.labStationsFound.DoubleClick += new System.EventHandler(this.Label1_DoubleClick);
			this.gridStations = new System.Windows.Forms.DataGridView();
			this.gridStations.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridStations_CellClick);
			this.gridStations.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridStations_RowEnter);
			this.gridStations.Sorted += new System.EventHandler(this.gridStations_Sorted);
			this.gridStations.SelectionChanged += new System.EventHandler(this.gridStations_SelectionChanged);
			this.gridStations.CurrentCellChanged += new System.EventHandler(this.gridStations_CurrentCellChanged);
			this.gridStations.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gridStations_CellMouseClick);
			this.colID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colModel = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colModelType = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colModelVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colCOM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colSW = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colHW = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colProtocol = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.colButResetAllCounters = new System.Windows.Forms.DataGridViewButtonColumn();
			this.colButResetPartialCounters = new System.Windows.Forms.DataGridViewButtonColumn();
			this.timerStationListData = new System.Windows.Forms.Timer(this.components);
			this.timerStationListData.Tick += new System.EventHandler(this.timerStationListData_Tick);
			this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
			this.cbProtocol01 = new System.Windows.Forms.CheckBox();
			this.labStationCounters = new System.Windows.Forms.Label();
			this.gridCounters = new System.Windows.Forms.DataGridView();
			this.col_Counter = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Global_Port_1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Global_Port_2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Global_Port_3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Global_Port_4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Partial_Port_1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Partial_Port_2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Partial_Port_3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col_Partial_Port_4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.timerUpdateCounters = new System.Windows.Forms.Timer(this.components);
			this.timerUpdateCounters.Tick += new System.EventHandler(this.timerUpdateCounters_Tick);
			((System.ComponentModel.ISupportInitialize) this.gridStations).BeginInit();
			((System.ComponentModel.ISupportInitialize) this.SplitContainer1).BeginInit();
			this.SplitContainer1.Panel1.SuspendLayout();
			this.SplitContainer1.Panel2.SuspendLayout();
			this.SplitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize) this.gridCounters).BeginInit();
			this.SuspendLayout();
			//
			//labStationsFound
			//
			this.labStationsFound.AutoSize = true;
			this.labStationsFound.Location = new System.Drawing.Point(4, 9);
			this.labStationsFound.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labStationsFound.Name = "labStationsFound";
			this.labStationsFound.Size = new System.Drawing.Size(97, 16);
			this.labStationsFound.TabIndex = 1;
			this.labStationsFound.Text = "Stations Found";
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
			this.gridStations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {this.colID, this.colName, this.colModel, this.colModelType, this.colModelVersion, this.colCOM, this.colSW, this.colHW, this.colProtocol, this.colButResetAllCounters, this.colButResetPartialCounters});
			this.gridStations.Location = new System.Drawing.Point(4, 29);
			this.gridStations.Margin = new System.Windows.Forms.Padding(4);
			this.gridStations.MultiSelect = false;
			this.gridStations.Name = "gridStations";
			this.gridStations.RowHeadersVisible = false;
			this.gridStations.Size = new System.Drawing.Size(1051, 98);
			this.gridStations.TabIndex = 0;
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
			this.colProtocol.Width = 50;
			//
			//colButResetAllCounters
			//
			this.colButResetAllCounters.HeaderText = "Reset All Counters";
			this.colButResetAllCounters.Name = "colButResetAllCounters";
			this.colButResetAllCounters.ReadOnly = true;
			this.colButResetAllCounters.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colButResetAllCounters.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.colButResetAllCounters.Width = 250;
			//
			//colButResetPartialCounters
			//
			this.colButResetPartialCounters.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.colButResetPartialCounters.HeaderText = "Reset Partial Counters";
			this.colButResetPartialCounters.Name = "colButResetPartialCounters";
			this.colButResetPartialCounters.ReadOnly = true;
			this.colButResetPartialCounters.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.colButResetPartialCounters.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
			this.SplitContainer1.Panel1.Controls.Add(this.cbProtocol01);
			this.SplitContainer1.Panel1.Controls.Add(this.labStationsFound);
			this.SplitContainer1.Panel1.Controls.Add(this.gridStations);
			//
			//SplitContainer1.Panel2
			//
			this.SplitContainer1.Panel2.Controls.Add(this.labStationCounters);
			this.SplitContainer1.Panel2.Controls.Add(this.gridCounters);
			this.SplitContainer1.Size = new System.Drawing.Size(1059, 354);
			this.SplitContainer1.SplitterDistance = 131;
			this.SplitContainer1.TabIndex = 2;
			//
			//cbProtocol01
			//
			this.cbProtocol01.AutoSize = true;
			this.cbProtocol01.Location = new System.Drawing.Point(903, 5);
			this.cbProtocol01.Name = "cbProtocol01";
			this.cbProtocol01.Size = new System.Drawing.Size(94, 20);
			this.cbProtocol01.TabIndex = 2;
			this.cbProtocol01.Text = "Protocol 01";
			this.cbProtocol01.UseVisualStyleBackColor = true;
			this.cbProtocol01.Visible = false;
			//
			//labStationCounters
			//
			this.labStationCounters.Dock = System.Windows.Forms.DockStyle.Top;
			this.labStationCounters.Location = new System.Drawing.Point(0, 0);
			this.labStationCounters.Name = "labStationCounters";
			this.labStationCounters.Size = new System.Drawing.Size(1059, 16);
			this.labStationCounters.TabIndex = 1;
			this.labStationCounters.Text = "Counters for Station: ";
			//
			//gridCounters
			//
			this.gridCounters.AllowUserToAddRows = false;
			this.gridCounters.AllowUserToDeleteRows = false;
			this.gridCounters.AllowUserToResizeRows = false;
			this.gridCounters.Anchor = (System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.gridCounters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridCounters.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {this.col_Counter, this.col_Global_Port_1, this.col_Global_Port_2, this.col_Global_Port_3, this.col_Global_Port_4, this.col_Partial_Port_1, this.col_Partial_Port_2, this.col_Partial_Port_3, this.col_Partial_Port_4});
			this.gridCounters.Location = new System.Drawing.Point(3, 26);
			this.gridCounters.Name = "gridCounters";
			this.gridCounters.RowHeadersVisible = false;
			this.gridCounters.Size = new System.Drawing.Size(1053, 190);
			this.gridCounters.TabIndex = 0;
			//
			//col_Counter
			//
			this.col_Counter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.col_Counter.HeaderText = "Counter";
			this.col_Counter.Name = "col_Counter";
			this.col_Counter.ReadOnly = true;
			this.col_Counter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Counter.Width = 60;
			//
			//col_Global_Port_1
			//
			this.col_Global_Port_1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle17.Format = "N0";
			DataGridViewCellStyle17.NullValue = null;
			this.col_Global_Port_1.DefaultCellStyle = DataGridViewCellStyle17;
			this.col_Global_Port_1.HeaderText = "Port 1";
			this.col_Global_Port_1.Name = "col_Global_Port_1";
			this.col_Global_Port_1.ReadOnly = true;
			this.col_Global_Port_1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Global_Port_1.Width = 48;
			//
			//col_Global_Port_2
			//
			this.col_Global_Port_2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle18.Format = "N0";
			DataGridViewCellStyle18.NullValue = null;
			this.col_Global_Port_2.DefaultCellStyle = DataGridViewCellStyle18;
			this.col_Global_Port_2.HeaderText = "Port 2";
			this.col_Global_Port_2.Name = "col_Global_Port_2";
			this.col_Global_Port_2.ReadOnly = true;
			this.col_Global_Port_2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Global_Port_2.Width = 48;
			//
			//col_Global_Port_3
			//
			this.col_Global_Port_3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle19.Format = "N0";
			DataGridViewCellStyle19.NullValue = null;
			this.col_Global_Port_3.DefaultCellStyle = DataGridViewCellStyle19;
			this.col_Global_Port_3.HeaderText = "Port 3";
			this.col_Global_Port_3.Name = "col_Global_Port_3";
			this.col_Global_Port_3.ReadOnly = true;
			this.col_Global_Port_3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Global_Port_3.Width = 48;
			//
			//col_Global_Port_4
			//
			this.col_Global_Port_4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle20.Format = "N0";
			this.col_Global_Port_4.DefaultCellStyle = DataGridViewCellStyle20;
			this.col_Global_Port_4.HeaderText = "Port 4";
			this.col_Global_Port_4.Name = "col_Global_Port_4";
			this.col_Global_Port_4.ReadOnly = true;
			this.col_Global_Port_4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Global_Port_4.Width = 48;
			//
			//col_Partial_Port_1
			//
			this.col_Partial_Port_1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle21.Format = "N0";
			this.col_Partial_Port_1.DefaultCellStyle = DataGridViewCellStyle21;
			this.col_Partial_Port_1.HeaderText = "Partial 1";
			this.col_Partial_Port_1.Name = "col_Partial_Port_1";
			this.col_Partial_Port_1.ReadOnly = true;
			this.col_Partial_Port_1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Partial_Port_1.Width = 62;
			//
			//col_Partial_Port_2
			//
			this.col_Partial_Port_2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle22.Format = "N0";
			this.col_Partial_Port_2.DefaultCellStyle = DataGridViewCellStyle22;
			this.col_Partial_Port_2.HeaderText = "Partial 2";
			this.col_Partial_Port_2.Name = "col_Partial_Port_2";
			this.col_Partial_Port_2.ReadOnly = true;
			this.col_Partial_Port_2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Partial_Port_2.Width = 62;
			//
			//col_Partial_Port_3
			//
			this.col_Partial_Port_3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle23.Format = "N0";
			this.col_Partial_Port_3.DefaultCellStyle = DataGridViewCellStyle23;
			this.col_Partial_Port_3.HeaderText = "Partial 3";
			this.col_Partial_Port_3.Name = "col_Partial_Port_3";
			this.col_Partial_Port_3.ReadOnly = true;
			this.col_Partial_Port_3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Partial_Port_3.Width = 62;
			//
			//col_Partial_Port_4
			//
			this.col_Partial_Port_4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			DataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			DataGridViewCellStyle24.Format = "N0";
			this.col_Partial_Port_4.DefaultCellStyle = DataGridViewCellStyle24;
			this.col_Partial_Port_4.HeaderText = "Partial 4";
			this.col_Partial_Port_4.Name = "col_Partial_Port_4";
			this.col_Partial_Port_4.ReadOnly = true;
			this.col_Partial_Port_4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.col_Partial_Port_4.Width = 62;
			//
			//timerUpdateCounters
			//
			this.timerUpdateCounters.Interval = 2000;
			//
			//FormResetCounters
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (8.0F), (float) (16.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1063, 359);
			this.Controls.Add(this.SplitContainer1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", (float) (9.75F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
			this.Icon = (System.Drawing.Icon) (resources.GetObject("$this.Icon"));
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "FormResetCounters";
			this.Text = "JBC Reset Counter";
			((System.ComponentModel.ISupportInitialize) this.gridStations).EndInit();
			this.SplitContainer1.Panel1.ResumeLayout(false);
			this.SplitContainer1.Panel1.PerformLayout();
			this.SplitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.SplitContainer1).EndInit();
			this.SplitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize) this.gridCounters).EndInit();
			this.ResumeLayout(false);
			
		}
		internal System.Windows.Forms.Timer timerStationListData;
		internal System.Windows.Forms.DataGridView gridStations;
		internal System.Windows.Forms.Label labStationsFound;
		internal System.Windows.Forms.SplitContainer SplitContainer1;
		internal System.Windows.Forms.DataGridView gridCounters;
		internal System.Windows.Forms.Label labStationCounters;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Counter;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Global_Port_1;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Global_Port_2;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Global_Port_3;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Global_Port_4;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Partial_Port_1;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Partial_Port_2;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Partial_Port_3;
		internal System.Windows.Forms.DataGridViewTextBoxColumn col_Partial_Port_4;
		internal System.Windows.Forms.CheckBox cbProtocol01;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colID;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colName;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colModel;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colModelType;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colModelVersion;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colCOM;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colSW;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colHW;
		internal System.Windows.Forms.DataGridViewTextBoxColumn colProtocol;
		internal System.Windows.Forms.DataGridViewButtonColumn colButResetAllCounters;
		internal System.Windows.Forms.DataGridViewButtonColumn colButResetPartialCounters;
		internal System.Windows.Forms.Timer timerUpdateCounters;
		
	}
	
}
