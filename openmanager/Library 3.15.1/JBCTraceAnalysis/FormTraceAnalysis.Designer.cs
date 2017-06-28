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
// End of VB project level imports

namespace FormTraceAnalysis
{
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public
    partial class FormTraceAnalysis : System.Windows.Forms.Form
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
            this.components = new System.ComponentModel.Container();
            base.Load += new System.EventHandler(FormTrace_Load);
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(FormTrace_FormClosing);
            System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle DataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timerStart = new System.Windows.Forms.Timer(this.components);
            this.timerStart.Tick += new System.EventHandler(this.timerStart_Tick);
            this.butReadJson = new System.Windows.Forms.Button();
            this.butReadJson.Click += new System.EventHandler(this.butReadJson_Click);
            this.tbPortData = new System.Windows.Forms.TextBox();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.ds = new System.Data.DataSet();
            this.TraceData = new System.Data.DataTable();
            this.colUID = new System.Data.DataColumn();
            this.colInitialDateTime = new System.Data.DataColumn();
            this.colInterval = new System.Data.DataColumn();
            this.colPort = new System.Data.DataColumn();
            this.colTool = new System.Data.DataColumn();
            this.colToolStatus = new System.Data.DataColumn();
            this.colTempUTI = new System.Data.DataColumn();
            this.colPower1000 = new System.Data.DataColumn();
            this.colSoftware = new System.Data.DataColumn();
            this.colFilename = new System.Data.DataColumn();
            this.colFileSequence = new System.Data.DataColumn();
            this.colSequence = new System.Data.DataColumn();
            this.colPortsDataJson = new System.Data.DataColumn();
            this.colCalculatedDateTime = new System.Data.DataColumn();
            this.StationData = new System.Data.DataTable();
            this.colStationUID = new System.Data.DataColumn();
            this.colStationName = new System.Data.DataColumn();
            this.colStationModel = new System.Data.DataColumn();
            this.colStationModelType = new System.Data.DataColumn();
            this.colStationModelVersion = new System.Data.DataColumn();
            this.colStationHW = new System.Data.DataColumn();
            this.colStationSW = new System.Data.DataColumn();
            this.colStationSelected = new System.Data.DataColumn();
            this.DataColumn1 = new System.Data.DataColumn();
            this.LoadedFiles = new System.Data.DataTable();
            this.colPathFilename = new System.Data.DataColumn();
            this.colLoadedFilename = new System.Data.DataColumn();
            this.colFileUID = new System.Data.DataColumn();
            this.gridTraceData = new System.Windows.Forms.DataGridView();
            this.gridTraceData.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridTraceData_RowEnter);
            this.colgridUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colgridInitialDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colgridInterval = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colgridCalculatedDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colgridFilename = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colgridFileSequence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colgridSequence = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colgridPortsDataJson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingTraceData = new System.Windows.Forms.BindingSource(this.components);
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tpTraceData = new System.Windows.Forms.TabPage();
            this.gbExportSeparator = new System.Windows.Forms.GroupBox();
            this.butExportData = new System.Windows.Forms.Button();
            this.butExportData.Click += new System.EventHandler(this.butExportData_Click);
            this.rbSepSemicolon = new System.Windows.Forms.RadioButton();
            this.rbSepComma = new System.Windows.Forms.RadioButton();
            this.rbSepTab = new System.Windows.Forms.RadioButton();
            this.labRecordCount = new System.Windows.Forms.Label();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.Label2 = new System.Windows.Forms.Label();
            this.tpFilters = new System.Windows.Forms.TabPage();
            this.Label1 = new System.Windows.Forms.Label();
            this.gridStations = new System.Windows.Forms.DataGridView();
            this.gridStations.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridStations_CellValueChanged);
            this.gridStations.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridStations_CellContentClick);
            this.gridStations.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridStations_CellClick);
            this.StationSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.StationUID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationModelDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationModelTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationModelVersionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationHWDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StationSWDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bindingStations = new System.Windows.Forms.BindingSource(this.components);
            this.tpLog = new System.Windows.Forms.TabPage();
            this.tbLog = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)this.ds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.TraceData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.StationData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.LoadedFiles).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.gridTraceData).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.bindingTraceData).BeginInit();
            this.TabControl1.SuspendLayout();
            this.tpTraceData.SuspendLayout();
            this.gbExportSeparator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.SplitContainer1).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.tpFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.gridStations).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.bindingStations).BeginInit();
            this.tpLog.SuspendLayout();
            this.SuspendLayout();
            //
            //timerStart
            //
            this.timerStart.Interval = 500;
            //
            //butReadJson
            //
            this.butReadJson.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.butReadJson.Location = new System.Drawing.Point(8, 374);
            this.butReadJson.Margin = new System.Windows.Forms.Padding(4);
            this.butReadJson.Name = "butReadJson";
            this.butReadJson.Size = new System.Drawing.Size(94, 26);
            this.butReadJson.TabIndex = 4;
            this.butReadJson.Text = "Read Jsons";
            this.butReadJson.UseVisualStyleBackColor = true;
            //
            //tbPortData
            //
            this.tbPortData.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.tbPortData.Location = new System.Drawing.Point(4, 39);
            this.tbPortData.Margin = new System.Windows.Forms.Padding(4);
            this.tbPortData.Multiline = true;
            this.tbPortData.Name = "tbPortData";
            this.tbPortData.ReadOnly = true;
            this.tbPortData.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.tbPortData.Size = new System.Drawing.Size(377, 311);
            this.tbPortData.TabIndex = 5;
            this.tbPortData.WordWrap = false;
            //
            //OpenFileDialog1
            //
            this.OpenFileDialog1.DefaultExt = "json";
            this.OpenFileDialog1.Filter = "Trace Files |*.json| All |*.*";
            this.OpenFileDialog1.Multiselect = true;
            //
            //ds
            //
            this.ds.DataSetName = "TraceDataSet";
            this.ds.Tables.AddRange(new System.Data.DataTable[] { this.TraceData, this.StationData, this.LoadedFiles });
            //
            //TraceData
            //
            this.TraceData.Columns.AddRange(new System.Data.DataColumn[] { this.colUID, this.colInitialDateTime, this.colInterval, this.colPort, this.colTool, this.colToolStatus, this.colTempUTI, this.colPower1000, this.colSoftware, this.colFilename, this.colFileSequence, this.colSequence, this.colPortsDataJson, this.colCalculatedDateTime });
            this.TraceData.TableName = "TraceData";
            //
            //colUID
            //
            this.colUID.ColumnName = "UID";
            //
            //colInitialDateTime
            //
            this.colInitialDateTime.Caption = "InitialDateTime";
            this.colInitialDateTime.ColumnName = "InitialDateTime";
            this.colInitialDateTime.DataType = typeof(DateTime);
            //
            //colInterval
            //
            this.colInterval.ColumnName = "Interval";
            this.colInterval.DataType = typeof(int);
            //
            //colPort
            //
            this.colPort.ColumnName = "Port";
            this.colPort.DataType = typeof(int);
            //
            //colTool
            //
            this.colTool.ColumnName = "Tool";
            this.colTool.DataType = typeof(int);
            //
            //colToolStatus
            //
            this.colToolStatus.ColumnName = "ToolStatus";
            //
            //colTempUTI
            //
            this.colTempUTI.ColumnName = "TempUTI";
            this.colTempUTI.DataType = typeof(int);
            //
            //colPower1000
            //
            this.colPower1000.ColumnName = "Power1000";
            //
            //colSoftware
            //
            this.colSoftware.ColumnName = "Software";
            //
            //colFilename
            //
            this.colFilename.ColumnName = "Filename";
            //
            //colFileSequence
            //
            this.colFileSequence.ColumnName = "FileSequence";
            this.colFileSequence.DataType = typeof(int);
            //
            //colSequence
            //
            this.colSequence.ColumnName = "Sequence";
            this.colSequence.DataType = typeof(int);
            //
            //colPortsDataJson
            //
            this.colPortsDataJson.ColumnName = "PortsDataJson";
            //
            //colCalculatedDateTime
            //
            this.colCalculatedDateTime.ColumnName = "CalculatedDateTime";
            this.colCalculatedDateTime.DataType = typeof(DateTime);
            //
            //StationData
            //
            this.StationData.Columns.AddRange(new System.Data.DataColumn[] { this.colStationUID, this.colStationName, this.colStationModel, this.colStationModelType, this.colStationModelVersion, this.colStationHW, this.colStationSW, this.colStationSelected, this.DataColumn1 });
            this.StationData.TableName = "StationData";
            //
            //colStationUID
            //
            this.colStationUID.ColumnName = "UID";
            //
            //colStationName
            //
            this.colStationName.ColumnName = "StationName";
            //
            //colStationModel
            //
            this.colStationModel.ColumnName = "StationModel";
            //
            //colStationModelType
            //
            this.colStationModelType.ColumnName = "StationModelType";
            //
            //colStationModelVersion
            //
            this.colStationModelVersion.ColumnName = "StationModelVersion";
            //
            //colStationHW
            //
            this.colStationHW.ColumnName = "StationHW";
            //
            //colStationSW
            //
            this.colStationSW.ColumnName = "StationSW";
            //
            //colStationSelected
            //
            this.colStationSelected.ColumnName = "StationSelected";
            this.colStationSelected.DataType = typeof(int);
            this.colStationSelected.DefaultValue = 1;
            //
            //DataColumn1
            //
            this.DataColumn1.ColumnName = "StationType";
            //
            //LoadedFiles
            //
            this.LoadedFiles.Columns.AddRange(new System.Data.DataColumn[] { this.colPathFilename, this.colLoadedFilename, this.colFileUID });
            this.LoadedFiles.TableName = "LoadedFiles";
            //
            //colPathFilename
            //
            this.colPathFilename.ColumnName = "PathFilename";
            //
            //colLoadedFilename
            //
            this.colLoadedFilename.ColumnName = "Filename";
            //
            //colFileUID
            //
            this.colFileUID.ColumnName = "UID";
            //
            //gridTraceData
            //
            this.gridTraceData.AllowUserToAddRows = false;
            this.gridTraceData.AllowUserToDeleteRows = false;
            this.gridTraceData.AllowUserToOrderColumns = true;
            this.gridTraceData.AllowUserToResizeRows = false;
            this.gridTraceData.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.gridTraceData.AutoGenerateColumns = false;
            this.gridTraceData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTraceData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.colgridUID, this.colgridInitialDateTime, this.colgridInterval, this.colgridCalculatedDateTime, this.colgridFilename, this.colgridFileSequence, this.colgridSequence, this.colgridPortsDataJson });
            this.gridTraceData.DataSource = this.bindingTraceData;
            this.gridTraceData.Location = new System.Drawing.Point(5, 5);
            this.gridTraceData.Margin = new System.Windows.Forms.Padding(4);
            this.gridTraceData.Name = "gridTraceData";
            this.gridTraceData.ReadOnly = true;
            this.gridTraceData.RowHeadersVisible = false;
            this.gridTraceData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridTraceData.Size = new System.Drawing.Size(747, 345);
            this.gridTraceData.TabIndex = 6;
            //
            //colgridUID
            //
            this.colgridUID.DataPropertyName = "UID";
            this.colgridUID.HeaderText = "UID";
            this.colgridUID.Name = "colgridUID";
            this.colgridUID.ReadOnly = true;
            this.colgridUID.Width = 150;
            //
            //colgridInitialDateTime
            //
            this.colgridInitialDateTime.DataPropertyName = "InitialDateTime";
            DataGridViewCellStyle1.Format = "dd-MM-yyyy HH:mm:ss";
            this.colgridInitialDateTime.DefaultCellStyle = DataGridViewCellStyle1;
            this.colgridInitialDateTime.HeaderText = "Initial DateTime";
            this.colgridInitialDateTime.Name = "colgridInitialDateTime";
            this.colgridInitialDateTime.ReadOnly = true;
            //
            //colgridInterval
            //
            this.colgridInterval.DataPropertyName = "Interval";
            this.colgridInterval.HeaderText = "Interval";
            this.colgridInterval.Name = "colgridInterval";
            this.colgridInterval.ReadOnly = true;
            this.colgridInterval.Width = 50;
            //
            //colgridCalculatedDateTime
            //
            this.colgridCalculatedDateTime.DataPropertyName = "CalculatedDateTime";
            DataGridViewCellStyle2.Format = "dd-MM-yyyy HH:mm:ss";
            this.colgridCalculatedDateTime.DefaultCellStyle = DataGridViewCellStyle2;
            this.colgridCalculatedDateTime.HeaderText = "Calculated DateTime";
            this.colgridCalculatedDateTime.Name = "colgridCalculatedDateTime";
            this.colgridCalculatedDateTime.ReadOnly = true;
            this.colgridCalculatedDateTime.Visible = false;
            //
            //colgridFilename
            //
            this.colgridFilename.DataPropertyName = "Filename";
            this.colgridFilename.HeaderText = "Filename";
            this.colgridFilename.Name = "colgridFilename";
            this.colgridFilename.ReadOnly = true;
            //
            //colgridFileSequence
            //
            this.colgridFileSequence.DataPropertyName = "FileSequence";
            this.colgridFileSequence.HeaderText = "File Sequence";
            this.colgridFileSequence.Name = "colgridFileSequence";
            this.colgridFileSequence.ReadOnly = true;
            //
            //colgridSequence
            //
            this.colgridSequence.DataPropertyName = "Sequence";
            this.colgridSequence.HeaderText = "Sequence";
            this.colgridSequence.Name = "colgridSequence";
            this.colgridSequence.ReadOnly = true;
            this.colgridSequence.Visible = false;
            //
            //colgridPortsDataJson
            //
            this.colgridPortsDataJson.DataPropertyName = "PortsDataJson";
            this.colgridPortsDataJson.HeaderText = "Ports Data Json";
            this.colgridPortsDataJson.Name = "colgridPortsDataJson";
            this.colgridPortsDataJson.ReadOnly = true;
            this.colgridPortsDataJson.Width = 200;
            //
            //bindingTraceData
            //
            this.bindingTraceData.DataMember = "TraceData";
            this.bindingTraceData.DataSource = this.ds;
            //
            //TabControl1
            //
            this.TabControl1.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.TabControl1.Controls.Add(this.tpTraceData);
            this.TabControl1.Controls.Add(this.tpFilters);
            this.TabControl1.Controls.Add(this.tpLog);
            this.TabControl1.Location = new System.Drawing.Point(1, 5);
            this.TabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(1159, 442);
            this.TabControl1.TabIndex = 7;
            //
            //tpTraceData
            //
            this.tpTraceData.Controls.Add(this.gbExportSeparator);
            this.tpTraceData.Controls.Add(this.labRecordCount);
            this.tpTraceData.Controls.Add(this.SplitContainer1);
            this.tpTraceData.Controls.Add(this.butReadJson);
            this.tpTraceData.Location = new System.Drawing.Point(4, 24);
            this.tpTraceData.Margin = new System.Windows.Forms.Padding(4);
            this.tpTraceData.Name = "tpTraceData";
            this.tpTraceData.Padding = new System.Windows.Forms.Padding(4);
            this.tpTraceData.Size = new System.Drawing.Size(1151, 414);
            this.tpTraceData.TabIndex = 0;
            this.tpTraceData.Text = "Trace Data";
            this.tpTraceData.UseVisualStyleBackColor = true;
            //
            //gbExportSeparator
            //
            this.gbExportSeparator.Controls.Add(this.butExportData);
            this.gbExportSeparator.Controls.Add(this.rbSepSemicolon);
            this.gbExportSeparator.Controls.Add(this.rbSepComma);
            this.gbExportSeparator.Controls.Add(this.rbSepTab);
            this.gbExportSeparator.Location = new System.Drawing.Point(136, 358);
            this.gbExportSeparator.Name = "gbExportSeparator";
            this.gbExportSeparator.Size = new System.Drawing.Size(335, 49);
            this.gbExportSeparator.TabIndex = 10;
            this.gbExportSeparator.TabStop = false;
            //
            //butExportData
            //
            this.butExportData.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.butExportData.Location = new System.Drawing.Point(7, 16);
            this.butExportData.Margin = new System.Windows.Forms.Padding(4);
            this.butExportData.Name = "butExportData";
            this.butExportData.Size = new System.Drawing.Size(94, 26);
            this.butExportData.TabIndex = 17;
            this.butExportData.Tag = ";";
            this.butExportData.Text = "Export Data";
            this.butExportData.UseVisualStyleBackColor = true;
            //
            //rbSepSemicolon
            //
            this.rbSepSemicolon.AutoSize = true;
            this.rbSepSemicolon.Location = new System.Drawing.Point(236, 20);
            this.rbSepSemicolon.Name = "rbSepSemicolon";
            this.rbSepSemicolon.Size = new System.Drawing.Size(84, 19);
            this.rbSepSemicolon.TabIndex = 16;
            this.rbSepSemicolon.Text = "Semicolon";
            this.rbSepSemicolon.UseVisualStyleBackColor = true;
            //
            //rbSepComma
            //
            this.rbSepComma.AutoSize = true;
            this.rbSepComma.Location = new System.Drawing.Point(161, 20);
            this.rbSepComma.Name = "rbSepComma";
            this.rbSepComma.Size = new System.Drawing.Size(69, 19);
            this.rbSepComma.TabIndex = 15;
            this.rbSepComma.Tag = ",";
            this.rbSepComma.Text = "Comma";
            this.rbSepComma.UseVisualStyleBackColor = true;
            //
            //rbSepTab
            //
            this.rbSepTab.AutoSize = true;
            this.rbSepTab.Checked = true;
            this.rbSepTab.Location = new System.Drawing.Point(109, 20);
            this.rbSepTab.Name = "rbSepTab";
            this.rbSepTab.Size = new System.Drawing.Size(46, 19);
            this.rbSepTab.TabIndex = 14;
            this.rbSepTab.TabStop = true;
            this.rbSepTab.Tag = "tab";
            this.rbSepTab.Text = "Tab";
            this.rbSepTab.UseVisualStyleBackColor = true;
            //
            //labRecordCount
            //
            this.labRecordCount.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.labRecordCount.AutoSize = true;
            this.labRecordCount.Location = new System.Drawing.Point(487, 380);
            this.labRecordCount.Name = "labRecordCount";
            this.labRecordCount.Size = new System.Drawing.Size(82, 15);
            this.labRecordCount.TabIndex = 9;
            this.labRecordCount.Text = "Record Count";
            //
            //SplitContainer1
            //
            this.SplitContainer1.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.SplitContainer1.Location = new System.Drawing.Point(3, 3);
            this.SplitContainer1.Name = "SplitContainer1";
            //
            //SplitContainer1.Panel1
            //
            this.SplitContainer1.Panel1.Controls.Add(this.gridTraceData);
            //
            //SplitContainer1.Panel2
            //
            this.SplitContainer1.Panel2.Controls.Add(this.tbPortData);
            this.SplitContainer1.Panel2.Controls.Add(this.Label2);
            this.SplitContainer1.Size = new System.Drawing.Size(1145, 354);
            this.SplitContainer1.SplitterDistance = 756;
            this.SplitContainer1.TabIndex = 8;
            //
            //Label2
            //
            this.Label2.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(3, 20);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(65, 15);
            this.Label2.TabIndex = 7;
            this.Label2.Text = "Ports data:";
            //
            //tpFilters
            //
            this.tpFilters.Controls.Add(this.Label1);
            this.tpFilters.Controls.Add(this.gridStations);
            this.tpFilters.Location = new System.Drawing.Point(4, 24);
            this.tpFilters.Margin = new System.Windows.Forms.Padding(4);
            this.tpFilters.Name = "tpFilters";
            this.tpFilters.Padding = new System.Windows.Forms.Padding(4);
            this.tpFilters.Size = new System.Drawing.Size(1151, 414);
            this.tpFilters.TabIndex = 2;
            this.tpFilters.Text = "Stations";
            this.tpFilters.UseVisualStyleBackColor = true;
            //
            //Label1
            //
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(8, 15);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(51, 15);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "Stations";
            //
            //gridStations
            //
            this.gridStations.AllowUserToAddRows = false;
            this.gridStations.AllowUserToDeleteRows = false;
            this.gridStations.AllowUserToOrderColumns = true;
            this.gridStations.AllowUserToResizeRows = false;
            this.gridStations.AutoGenerateColumns = false;
            this.gridStations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridStations.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { this.StationSelected, this.StationUID, this.StationNameDataGridViewTextBoxColumn, this.StationType, this.StationModelDataGridViewTextBoxColumn, this.StationModelTypeDataGridViewTextBoxColumn, this.StationModelVersionDataGridViewTextBoxColumn, this.StationHWDataGridViewTextBoxColumn, this.StationSWDataGridViewTextBoxColumn });
            this.gridStations.DataSource = this.bindingStations;
            this.gridStations.Location = new System.Drawing.Point(7, 44);
            this.gridStations.Margin = new System.Windows.Forms.Padding(4);
            this.gridStations.Name = "gridStations";
            this.gridStations.RowHeadersVisible = false;
            this.gridStations.Size = new System.Drawing.Size(773, 285);
            this.gridStations.TabIndex = 0;
            //
            //StationSelected
            //
            this.StationSelected.DataPropertyName = "StationSelected";
            this.StationSelected.FalseValue = "0";
            this.StationSelected.HeaderText = "Sel.";
            this.StationSelected.Name = "StationSelected";
            this.StationSelected.TrueValue = "1";
            this.StationSelected.Width = 50;
            //
            //StationUID
            //
            this.StationUID.DataPropertyName = "UID";
            this.StationUID.HeaderText = "UID";
            this.StationUID.Name = "StationUID";
            this.StationUID.ReadOnly = true;
            this.StationUID.Width = 150;
            //
            //StationNameDataGridViewTextBoxColumn
            //
            this.StationNameDataGridViewTextBoxColumn.DataPropertyName = "StationName";
            this.StationNameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.StationNameDataGridViewTextBoxColumn.Name = "StationNameDataGridViewTextBoxColumn";
            this.StationNameDataGridViewTextBoxColumn.ReadOnly = true;
            //
            //StationType
            //
            this.StationType.DataPropertyName = "StationType";
            this.StationType.HeaderText = "Type";
            this.StationType.Name = "StationType";
            this.StationType.ReadOnly = true;
            this.StationType.Width = 50;
            //
            //StationModelDataGridViewTextBoxColumn
            //
            this.StationModelDataGridViewTextBoxColumn.DataPropertyName = "StationModel";
            this.StationModelDataGridViewTextBoxColumn.HeaderText = "Model";
            this.StationModelDataGridViewTextBoxColumn.Name = "StationModelDataGridViewTextBoxColumn";
            this.StationModelDataGridViewTextBoxColumn.ReadOnly = true;
            this.StationModelDataGridViewTextBoxColumn.Width = 60;
            //
            //StationModelTypeDataGridViewTextBoxColumn
            //
            this.StationModelTypeDataGridViewTextBoxColumn.DataPropertyName = "StationModelType";
            this.StationModelTypeDataGridViewTextBoxColumn.HeaderText = "Model Type";
            this.StationModelTypeDataGridViewTextBoxColumn.Name = "StationModelTypeDataGridViewTextBoxColumn";
            this.StationModelTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.StationModelTypeDataGridViewTextBoxColumn.Width = 50;
            //
            //StationModelVersionDataGridViewTextBoxColumn
            //
            this.StationModelVersionDataGridViewTextBoxColumn.DataPropertyName = "StationModelVersion";
            this.StationModelVersionDataGridViewTextBoxColumn.HeaderText = "Model Version";
            this.StationModelVersionDataGridViewTextBoxColumn.Name = "StationModelVersionDataGridViewTextBoxColumn";
            this.StationModelVersionDataGridViewTextBoxColumn.ReadOnly = true;
            this.StationModelVersionDataGridViewTextBoxColumn.Width = 50;
            //
            //StationHWDataGridViewTextBoxColumn
            //
            this.StationHWDataGridViewTextBoxColumn.DataPropertyName = "StationHW";
            this.StationHWDataGridViewTextBoxColumn.HeaderText = "HW";
            this.StationHWDataGridViewTextBoxColumn.Name = "StationHWDataGridViewTextBoxColumn";
            this.StationHWDataGridViewTextBoxColumn.ReadOnly = true;
            //
            //StationSWDataGridViewTextBoxColumn
            //
            this.StationSWDataGridViewTextBoxColumn.DataPropertyName = "StationSW";
            this.StationSWDataGridViewTextBoxColumn.HeaderText = "SW";
            this.StationSWDataGridViewTextBoxColumn.Name = "StationSWDataGridViewTextBoxColumn";
            this.StationSWDataGridViewTextBoxColumn.ReadOnly = true;
            //
            //bindingStations
            //
            this.bindingStations.DataMember = "StationData";
            this.bindingStations.DataSource = this.ds;
            //
            //tpLog
            //
            this.tpLog.Controls.Add(this.tbLog);
            this.tpLog.Location = new System.Drawing.Point(4, 24);
            this.tpLog.Margin = new System.Windows.Forms.Padding(4);
            this.tpLog.Name = "tpLog";
            this.tpLog.Padding = new System.Windows.Forms.Padding(4);
            this.tpLog.Size = new System.Drawing.Size(1151, 414);
            this.tpLog.TabIndex = 1;
            this.tpLog.Text = "Log";
            this.tpLog.UseVisualStyleBackColor = true;
            //
            //tbLog
            //
            this.tbLog.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.tbLog.Location = new System.Drawing.Point(8, 7);
            this.tbLog.Margin = new System.Windows.Forms.Padding(4);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(1017, 371);
            this.tbLog.TabIndex = 0;
            //
            //FormTraceAnalysis
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(7.0F), (float)(15.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1162, 448);
            this.Controls.Add(this.TabControl1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", (float)(9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormTraceAnalysis";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)this.ds).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.TraceData).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.StationData).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.LoadedFiles).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.gridTraceData).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.bindingTraceData).EndInit();
            this.TabControl1.ResumeLayout(false);
            this.tpTraceData.ResumeLayout(false);
            this.tpTraceData.PerformLayout();
            this.gbExportSeparator.ResumeLayout(false);
            this.gbExportSeparator.PerformLayout();
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.SplitContainer1).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.tpFilters.ResumeLayout(false);
            this.tpFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.gridStations).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.bindingStations).EndInit();
            this.tpLog.ResumeLayout(false);
            this.tpLog.PerformLayout();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Timer timerStart;
        internal System.Windows.Forms.Button butReadJson;
        internal System.Windows.Forms.TextBox tbPortData;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        internal System.Data.DataSet ds;
        internal System.Data.DataTable TraceData;
        internal System.Data.DataColumn colUID;
        internal System.Data.DataColumn colInitialDateTime;
        internal System.Data.DataColumn colInterval;
        internal System.Data.DataColumn colPort;
        internal System.Data.DataColumn colTool;
        internal System.Data.DataColumn colToolStatus;
        internal System.Data.DataColumn colTempUTI;
        internal System.Data.DataColumn colPower1000;
        internal System.Data.DataColumn colSoftware;
        internal System.Data.DataTable StationData;
        internal System.Data.DataColumn colStationUID;
        internal System.Data.DataColumn colStationName;
        internal System.Data.DataColumn colStationModel;
        internal System.Data.DataColumn colStationModelType;
        internal System.Data.DataColumn colStationModelVersion;
        internal System.Data.DataColumn colStationHW;
        internal System.Data.DataColumn colStationSW;
        internal System.Windows.Forms.DataGridView gridTraceData;
        internal System.Windows.Forms.BindingSource bindingTraceData;
        internal System.Windows.Forms.TabControl TabControl1;
        internal System.Windows.Forms.TabPage tpTraceData;
        internal System.Windows.Forms.TabPage tpLog;
        internal System.Windows.Forms.TextBox tbLog;
        internal System.Data.DataColumn colFilename;
        internal System.Data.DataColumn colFileSequence;
        internal System.Data.DataColumn colSequence;
        internal System.Data.DataColumn colPortsDataJson;
        internal System.Data.DataTable LoadedFiles;
        internal System.Data.DataColumn colPathFilename;
        internal System.Data.DataColumn colLoadedFilename;
        internal System.Data.DataColumn colFileUID;
        internal System.Data.DataColumn colCalculatedDateTime;
        internal System.Data.DataColumn colStationSelected;
        internal System.Windows.Forms.TabPage tpFilters;
        internal System.Windows.Forms.DataGridView gridStations;
        internal System.Windows.Forms.BindingSource bindingStations;
        internal System.Data.DataColumn DataColumn1;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.SplitContainer SplitContainer1;
        internal System.Windows.Forms.DataGridViewCheckBoxColumn StationSelected;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationUID;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationNameDataGridViewTextBoxColumn;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationType;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationModelDataGridViewTextBoxColumn;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationModelTypeDataGridViewTextBoxColumn;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationModelVersionDataGridViewTextBoxColumn;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationHWDataGridViewTextBoxColumn;
        internal System.Windows.Forms.DataGridViewTextBoxColumn StationSWDataGridViewTextBoxColumn;
        internal System.Windows.Forms.Label labRecordCount;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridUID;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridInitialDateTime;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridInterval;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridCalculatedDateTime;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridFilename;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridFileSequence;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridSequence;
        internal System.Windows.Forms.DataGridViewTextBoxColumn colgridPortsDataJson;
        internal System.Windows.Forms.GroupBox gbExportSeparator;
        internal System.Windows.Forms.Button butExportData;
        internal System.Windows.Forms.RadioButton rbSepSemicolon;
        internal System.Windows.Forms.RadioButton rbSepComma;
        internal System.Windows.Forms.RadioButton rbSepTab;

    }
}
