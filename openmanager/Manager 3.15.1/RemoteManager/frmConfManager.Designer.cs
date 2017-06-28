// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


namespace RemoteManager
{
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public
partial class frmConfManager : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmConfManager));
            this.butConfSave = new System.Windows.Forms.Button();
            base.Load += new System.EventHandler(frmConfManager_Load);
            this.butConfSave.Click += new System.EventHandler(this.butConfSave_Click);
            this.EndApply += new frmConfManager.EndApplyEventHandler(ev_EndApply);
            this.butConfLoad = new System.Windows.Forms.Button();
            this.butConfLoad.Click += new System.EventHandler(this.butConfLoad_Click);
            this.panelTreeView = new System.Windows.Forms.Panel();
            this.panelTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.onTree_DragEnter);
            this.panelTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.onTree_DragDrop);
            this.panel1 = new System.Windows.Forms.Panel();
            this.butPrint = new System.Windows.Forms.Button();
            this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
            this.Panel1Bot = new System.Windows.Forms.Panel();
            this.labConfComment = new System.Windows.Forms.Label();
            this.tbConfComment = new System.Windows.Forms.TextBox();
            this.labConfSourceSettings = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labLogApply = new System.Windows.Forms.Label();
            this.butConfViewReport = new System.Windows.Forms.Button();
            this.butConfViewReport.Click += new System.EventHandler(this.butConfViewReport_Click);
            this.butConfApply = new System.Windows.Forms.Button();
            this.butConfApply.Click += new System.EventHandler(this.butConfApply_Click);
            this.panelStationsView = new System.Windows.Forms.Panel();
            this.tbFilterName = new System.Windows.Forms.TextBox();
            this.labConfNameFilter = new System.Windows.Forms.Label();
            this.tbFilterModel = new System.Windows.Forms.TextBox();
            this.labConfModelFilter = new System.Windows.Forms.Label();
            this.labConfStationFilter = new System.Windows.Forms.Label();
            this.butRefreshStations = new System.Windows.Forms.Button();
            this.butRefreshStations.Click += new System.EventHandler(this.butRefreshStations_Click);
            this.panelStationsList = new System.Windows.Forms.Panel();
            this.tlpStationList = new System.Windows.Forms.TableLayoutPanel();
            this.labTargetPort_4 = new System.Windows.Forms.Label();
            this.port_1_4 = new System.Windows.Forms.NumericUpDown();
            this.labTargetPort_3 = new System.Windows.Forms.Label();
            this.port_1_3 = new System.Windows.Forms.NumericUpDown();
            this.labTargetPort_2 = new System.Windows.Forms.Label();
            this.port_1_2 = new System.Windows.Forms.NumericUpDown();
            this.labTargetPort_1 = new System.Windows.Forms.Label();
            this.cbPort_1_1 = new System.Windows.Forms.ComboBox();
            this.cbST_1 = new System.Windows.Forms.CheckBox();
            this.labStation = new System.Windows.Forms.Label();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.CheckBox1.Click += new System.EventHandler(this.row_checkControlMode_Click);
            this.Label1 = new System.Windows.Forms.Label();
            this.labConfTargetStations = new System.Windows.Forms.Label();
            this.Splitter1 = new System.Windows.Forms.Splitter();
            this.labConfStationTypeFilter = new System.Windows.Forms.Label();
            this.cbFilterType = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.Panel1Bot.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelStationsView.SuspendLayout();
            this.panelStationsList.SuspendLayout();
            this.tlpStationList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.port_1_4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.port_1_3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.port_1_2).BeginInit();
            this.SuspendLayout();
            //
            //butConfSave
            //
            this.butConfSave.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.butConfSave.ForeColor = System.Drawing.Color.DarkBlue;
            this.butConfSave.Location = new System.Drawing.Point(173, 44);
            this.butConfSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butConfSave.Name = "butConfSave";
            this.butConfSave.Size = new System.Drawing.Size(174, 28);
            this.butConfSave.TabIndex = 7;
            this.butConfSave.Text = "Save To File";
            this.butConfSave.UseVisualStyleBackColor = true;
            //
            //butConfLoad
            //
            this.butConfLoad.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.butConfLoad.ForeColor = System.Drawing.Color.DarkBlue;
            this.butConfLoad.Location = new System.Drawing.Point(4, 44);
            this.butConfLoad.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butConfLoad.Name = "butConfLoad";
            this.butConfLoad.Size = new System.Drawing.Size(161, 28);
            this.butConfLoad.TabIndex = 6;
            this.butConfLoad.Text = "Load From File";
            this.butConfLoad.UseVisualStyleBackColor = true;
            //
            //panelTreeView
            //
            this.panelTreeView.AllowDrop = true;
            this.panelTreeView.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.panelTreeView.AutoScroll = true;
            this.panelTreeView.ForeColor = System.Drawing.Color.White;
            this.panelTreeView.Location = new System.Drawing.Point(4, 17);
            this.panelTreeView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelTreeView.Name = "panelTreeView";
            this.panelTreeView.Size = new System.Drawing.Size(368, 359);
            this.panelTreeView.TabIndex = 1;
            //
            //panel1
            //
            this.panel1.Controls.Add(this.butPrint);
            this.panel1.Controls.Add(this.Panel1Bot);
            this.panel1.Controls.Add(this.labConfSourceSettings);
            this.panel1.Controls.Add(this.panelTreeView);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(376, 472);
            this.panel1.TabIndex = 10;
            //
            //butPrint
            //
            this.butPrint.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.butPrint.ForeColor = System.Drawing.Color.DarkBlue;
            this.butPrint.Location = new System.Drawing.Point(270, 382);
            this.butPrint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butPrint.Name = "butPrint";
            this.butPrint.Size = new System.Drawing.Size(102, 28);
            this.butPrint.TabIndex = 11;
            this.butPrint.Text = "Print";
            this.butPrint.UseVisualStyleBackColor = true;
            //
            //Panel1Bot
            //
            this.Panel1Bot.Controls.Add(this.labConfComment);
            this.Panel1Bot.Controls.Add(this.tbConfComment);
            this.Panel1Bot.Controls.Add(this.butConfSave);
            this.Panel1Bot.Controls.Add(this.butConfLoad);
            this.Panel1Bot.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1Bot.Location = new System.Drawing.Point(0, 397);
            this.Panel1Bot.Name = "Panel1Bot";
            this.Panel1Bot.Size = new System.Drawing.Size(376, 75);
            this.Panel1Bot.TabIndex = 9;
            //
            //labConfComment
            //
            this.labConfComment.Dock = System.Windows.Forms.DockStyle.Top;
            this.labConfComment.ForeColor = System.Drawing.Color.DarkBlue;
            this.labConfComment.Location = new System.Drawing.Point(0, 0);
            this.labConfComment.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labConfComment.Name = "labConfComment";
            this.labConfComment.Size = new System.Drawing.Size(376, 17);
            this.labConfComment.TabIndex = 10;
            this.labConfComment.Text = "Comment";
            //
            //tbConfComment
            //
            this.tbConfComment.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.tbConfComment.Location = new System.Drawing.Point(4, 19);
            this.tbConfComment.Name = "tbConfComment";
            this.tbConfComment.Size = new System.Drawing.Size(369, 22);
            this.tbConfComment.TabIndex = 9;
            //
            //labConfSourceSettings
            //
            this.labConfSourceSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.labConfSourceSettings.ForeColor = System.Drawing.Color.DarkBlue;
            this.labConfSourceSettings.Location = new System.Drawing.Point(0, 0);
            this.labConfSourceSettings.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labConfSourceSettings.Name = "labConfSourceSettings";
            this.labConfSourceSettings.Size = new System.Drawing.Size(376, 14);
            this.labConfSourceSettings.TabIndex = 8;
            this.labConfSourceSettings.Text = "Source Settings";
            //
            //panel2
            //
            this.panel2.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panel2.Controls.Add(this.labLogApply);
            this.panel2.Controls.Add(this.butConfViewReport);
            this.panel2.Controls.Add(this.butConfApply);
            this.panel2.Controls.Add(this.panelStationsView);
            this.panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(376, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(614, 472);
            this.panel2.TabIndex = 11;
            //
            //labLogApply
            //
            this.labLogApply.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.labLogApply.ForeColor = System.Drawing.Color.DarkBlue;
            this.labLogApply.Location = new System.Drawing.Point(299, 448);
            this.labLogApply.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labLogApply.Name = "labLogApply";
            this.labLogApply.Size = new System.Drawing.Size(310, 15);
            this.labLogApply.TabIndex = 20;
            this.labLogApply.Text = "Applying to station xxxxx";
            //
            //butConfViewReport
            //
            this.butConfViewReport.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.butConfViewReport.ForeColor = System.Drawing.Color.DarkBlue;
            this.butConfViewReport.Location = new System.Drawing.Point(153, 441);
            this.butConfViewReport.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butConfViewReport.Name = "butConfViewReport";
            this.butConfViewReport.Size = new System.Drawing.Size(128, 28);
            this.butConfViewReport.TabIndex = 8;
            this.butConfViewReport.Text = "View Report";
            this.butConfViewReport.UseVisualStyleBackColor = true;
            //
            //butConfApply
            //
            this.butConfApply.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.butConfApply.ForeColor = System.Drawing.Color.DarkBlue;
            this.butConfApply.Location = new System.Drawing.Point(17, 441);
            this.butConfApply.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butConfApply.Name = "butConfApply";
            this.butConfApply.Size = new System.Drawing.Size(128, 28);
            this.butConfApply.TabIndex = 7;
            this.butConfApply.Text = "Apply";
            this.butConfApply.UseVisualStyleBackColor = true;
            //
            //panelStationsView
            //
            this.panelStationsView.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.panelStationsView.BackColor = System.Drawing.Color.LightSteelBlue;
            this.panelStationsView.Controls.Add(this.cbFilterType);
            this.panelStationsView.Controls.Add(this.labConfStationTypeFilter);
            this.panelStationsView.Controls.Add(this.tbFilterName);
            this.panelStationsView.Controls.Add(this.labConfNameFilter);
            this.panelStationsView.Controls.Add(this.tbFilterModel);
            this.panelStationsView.Controls.Add(this.labConfModelFilter);
            this.panelStationsView.Controls.Add(this.labConfStationFilter);
            this.panelStationsView.Controls.Add(this.butRefreshStations);
            this.panelStationsView.Controls.Add(this.panelStationsList);
            this.panelStationsView.Controls.Add(this.labConfTargetStations);
            this.panelStationsView.Location = new System.Drawing.Point(8, 3);
            this.panelStationsView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelStationsView.Name = "panelStationsView";
            this.panelStationsView.Size = new System.Drawing.Size(602, 435);
            this.panelStationsView.TabIndex = 3;
            //
            //tbFilterName
            //
            this.tbFilterName.Location = new System.Drawing.Point(456, 28);
            this.tbFilterName.Name = "tbFilterName";
            this.tbFilterName.Size = new System.Drawing.Size(83, 22);
            this.tbFilterName.TabIndex = 21;
            //
            //labConfNameFilter
            //
            this.labConfNameFilter.ForeColor = System.Drawing.Color.DarkBlue;
            this.labConfNameFilter.Location = new System.Drawing.Point(383, 31);
            this.labConfNameFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labConfNameFilter.Name = "labConfNameFilter";
            this.labConfNameFilter.Size = new System.Drawing.Size(71, 17);
            this.labConfNameFilter.TabIndex = 22;
            this.labConfNameFilter.Text = "Name:";
            this.labConfNameFilter.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //tbFilterModel
            //
            this.tbFilterModel.Location = new System.Drawing.Point(313, 28);
            this.tbFilterModel.Name = "tbFilterModel";
            this.tbFilterModel.Size = new System.Drawing.Size(58, 22);
            this.tbFilterModel.TabIndex = 17;
            //
            //labConfModelFilter
            //
            this.labConfModelFilter.ForeColor = System.Drawing.Color.DarkBlue;
            this.labConfModelFilter.Location = new System.Drawing.Point(240, 31);
            this.labConfModelFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labConfModelFilter.Name = "labConfModelFilter";
            this.labConfModelFilter.Size = new System.Drawing.Size(71, 17);
            this.labConfModelFilter.TabIndex = 20;
            this.labConfModelFilter.Text = "Model:";
            this.labConfModelFilter.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //labConfStationFilter
            //
            this.labConfStationFilter.AutoSize = true;
            this.labConfStationFilter.ForeColor = System.Drawing.Color.DarkBlue;
            this.labConfStationFilter.Location = new System.Drawing.Point(7, 31);
            this.labConfStationFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labConfStationFilter.Name = "labConfStationFilter";
            this.labConfStationFilter.Size = new System.Drawing.Size(45, 14);
            this.labConfStationFilter.TabIndex = 19;
            this.labConfStationFilter.Text = "Filters";
            //
            //butRefreshStations
            //
            this.butRefreshStations.AutoSize = true;
            this.butRefreshStations.ForeColor = System.Drawing.Color.DarkBlue;
            this.butRefreshStations.Location = new System.Drawing.Point(366, -1);
            this.butRefreshStations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butRefreshStations.Name = "butRefreshStations";
            this.butRefreshStations.Size = new System.Drawing.Size(153, 28);
            this.butRefreshStations.TabIndex = 18;
            this.butRefreshStations.Text = "Refresh";
            this.butRefreshStations.UseVisualStyleBackColor = true;
            //
            //panelStationsList
            //
            this.panelStationsList.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.panelStationsList.AutoScroll = true;
            this.panelStationsList.Controls.Add(this.tlpStationList);
            this.panelStationsList.Location = new System.Drawing.Point(0, 62);
            this.panelStationsList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelStationsList.Name = "panelStationsList";
            this.panelStationsList.Size = new System.Drawing.Size(595, 370);
            this.panelStationsList.TabIndex = 17;
            //
            //tlpStationList
            //
            this.tlpStationList.AutoSize = true;
            this.tlpStationList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpStationList.BackColor = System.Drawing.Color.Transparent;
            this.tlpStationList.ColumnCount = 7;
            this.tlpStationList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(43.0F)));
            this.tlpStationList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(200.0F)));
            this.tlpStationList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(58.0F)));
            this.tlpStationList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(61.0F)));
            this.tlpStationList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(60.0F)));
            this.tlpStationList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(60.0F)));
            this.tlpStationList.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(100.0F)));
            this.tlpStationList.Controls.Add(this.labTargetPort_4, 5, 0);
            this.tlpStationList.Controls.Add(this.port_1_4, 5, 1);
            this.tlpStationList.Controls.Add(this.labTargetPort_3, 4, 0);
            this.tlpStationList.Controls.Add(this.port_1_3, 4, 1);
            this.tlpStationList.Controls.Add(this.labTargetPort_2, 3, 0);
            this.tlpStationList.Controls.Add(this.port_1_2, 3, 1);
            this.tlpStationList.Controls.Add(this.labTargetPort_1, 2, 0);
            this.tlpStationList.Controls.Add(this.cbPort_1_1, 2, 1);
            this.tlpStationList.Controls.Add(this.cbST_1, 1, 1);
            this.tlpStationList.Controls.Add(this.labStation, 1, 0);
            this.tlpStationList.Controls.Add(this.CheckBox1, 0, 1);
            this.tlpStationList.Controls.Add(this.Label1, 6, 1);
            this.tlpStationList.Location = new System.Drawing.Point(4, 3);
            this.tlpStationList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tlpStationList.Name = "tlpStationList";
            this.tlpStationList.RowCount = 2;
            this.tlpStationList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpStationList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(26.0F)));
            this.tlpStationList.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(20.0F)));
            this.tlpStationList.Size = new System.Drawing.Size(582, 76);
            this.tlpStationList.TabIndex = 16;
            //
            //labTargetPort_4
            //
            this.labTargetPort_4.ForeColor = System.Drawing.Color.DarkBlue;
            this.labTargetPort_4.Location = new System.Drawing.Point(426, 0);
            this.labTargetPort_4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labTargetPort_4.Name = "labTargetPort_4";
            this.labTargetPort_4.Size = new System.Drawing.Size(52, 34);
            this.labTargetPort_4.TabIndex = 4;
            this.labTargetPort_4.Text = "Target Port 4";
            this.labTargetPort_4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            //port_1_4
            //
            this.port_1_4.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.port_1_4.Location = new System.Drawing.Point(437, 53);
            this.port_1_4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.port_1_4.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            this.port_1_4.Name = "port_1_4";
            this.port_1_4.Size = new System.Drawing.Size(41, 22);
            this.port_1_4.TabIndex = 11;
            this.port_1_4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.port_1_4.Value = new decimal(new int[] { 2, 0, 0, 0 });
            //
            //labTargetPort_3
            //
            this.labTargetPort_3.ForeColor = System.Drawing.Color.DarkBlue;
            this.labTargetPort_3.Location = new System.Drawing.Point(366, 0);
            this.labTargetPort_3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labTargetPort_3.Name = "labTargetPort_3";
            this.labTargetPort_3.Size = new System.Drawing.Size(52, 34);
            this.labTargetPort_3.TabIndex = 3;
            this.labTargetPort_3.Text = "Target Port 3";
            this.labTargetPort_3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            //port_1_3
            //
            this.port_1_3.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.port_1_3.Location = new System.Drawing.Point(377, 53);
            this.port_1_3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.port_1_3.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            this.port_1_3.Name = "port_1_3";
            this.port_1_3.Size = new System.Drawing.Size(41, 22);
            this.port_1_3.TabIndex = 10;
            this.port_1_3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.port_1_3.Value = new decimal(new int[] { 1, 0, 0, 0 });
            //
            //labTargetPort_2
            //
            this.labTargetPort_2.ForeColor = System.Drawing.Color.DarkBlue;
            this.labTargetPort_2.Location = new System.Drawing.Point(305, 0);
            this.labTargetPort_2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labTargetPort_2.Name = "labTargetPort_2";
            this.labTargetPort_2.Size = new System.Drawing.Size(53, 34);
            this.labTargetPort_2.TabIndex = 2;
            this.labTargetPort_2.Text = "Target Port 2";
            this.labTargetPort_2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            //port_1_2
            //
            this.port_1_2.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.port_1_2.Location = new System.Drawing.Point(317, 53);
            this.port_1_2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.port_1_2.Maximum = new decimal(new int[] { 4, 0, 0, 0 });
            this.port_1_2.Name = "port_1_2";
            this.port_1_2.Size = new System.Drawing.Size(41, 22);
            this.port_1_2.TabIndex = 9;
            this.port_1_2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.port_1_2.Value = new decimal(new int[] { 2, 0, 0, 0 });
            //
            //labTargetPort_1
            //
            this.labTargetPort_1.ForeColor = System.Drawing.Color.DarkBlue;
            this.labTargetPort_1.Location = new System.Drawing.Point(247, 0);
            this.labTargetPort_1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labTargetPort_1.Name = "labTargetPort_1";
            this.labTargetPort_1.Size = new System.Drawing.Size(50, 34);
            this.labTargetPort_1.TabIndex = 1;
            this.labTargetPort_1.Text = "Target Port 1";
            this.labTargetPort_1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            //cbPort_1_1
            //
            this.cbPort_1_1.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.cbPort_1_1.DisplayMember = "1";
            this.cbPort_1_1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPort_1_1.FormattingEnabled = true;
            this.cbPort_1_1.Items.AddRange(new object[] { "No", "1", "2", "3", "4" });
            this.cbPort_1_1.Location = new System.Drawing.Point(251, 53);
            this.cbPort_1_1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cbPort_1_1.Name = "cbPort_1_1";
            this.cbPort_1_1.Size = new System.Drawing.Size(46, 22);
            this.cbPort_1_1.TabIndex = 14;
            //
            //cbST_1
            //
            this.cbST_1.AutoSize = true;
            this.cbST_1.ForeColor = System.Drawing.Color.DarkBlue;
            this.cbST_1.Location = new System.Drawing.Point(47, 57);
            this.cbST_1.Margin = new System.Windows.Forms.Padding(4, 7, 4, 3);
            this.cbST_1.Name = "cbST_1";
            this.cbST_1.Size = new System.Drawing.Size(44, 16);
            this.cbST_1.TabIndex = 6;
            this.cbST_1.Text = "DD";
            this.cbST_1.UseVisualStyleBackColor = true;
            //
            //labStation
            //
            this.labStation.ForeColor = System.Drawing.Color.DarkBlue;
            this.labStation.Location = new System.Drawing.Point(47, 0);
            this.labStation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labStation.Name = "labStation";
            this.labStation.Size = new System.Drawing.Size(131, 25);
            this.labStation.TabIndex = 0;
            this.labStation.Text = "Station";
            //
            //CheckBox1
            //
            this.CheckBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.CheckBox1.BackColor = System.Drawing.Color.Transparent;
            this.CheckBox1.FlatAppearance.BorderSize = 0;
            this.CheckBox1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.CheckBox1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.CheckBox1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.CheckBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CheckBox1.Image = My.Resources.Resources._lock;
            this.CheckBox1.Location = new System.Drawing.Point(0, 50);
            this.CheckBox1.Margin = new System.Windows.Forms.Padding(0);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(25, 26);
            this.CheckBox1.TabIndex = 15;
            this.CheckBox1.UseVisualStyleBackColor = false;
            //
            //Label1
            //
            this.Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label1.Location = new System.Drawing.Point(485, 50);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(94, 26);
            this.Label1.TabIndex = 16;
            this.Label1.Text = "Applied";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //labConfTargetStations
            //
            this.labConfTargetStations.AutoSize = true;
            this.labConfTargetStations.ForeColor = System.Drawing.Color.DarkBlue;
            this.labConfTargetStations.Location = new System.Drawing.Point(7, 6);
            this.labConfTargetStations.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labConfTargetStations.Name = "labConfTargetStations";
            this.labConfTargetStations.Size = new System.Drawing.Size(130, 14);
            this.labConfTargetStations.TabIndex = 1;
            this.labConfTargetStations.Text = "List Target Stations";
            //
            //Splitter1
            //
            this.Splitter1.BackColor = System.Drawing.Color.SteelBlue;
            this.Splitter1.Location = new System.Drawing.Point(376, 0);
            this.Splitter1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Splitter1.Name = "Splitter1";
            this.Splitter1.Size = new System.Drawing.Size(7, 472);
            this.Splitter1.TabIndex = 12;
            this.Splitter1.TabStop = false;
            //
            //labConfStationTypeFilter
            //
            this.labConfStationTypeFilter.ForeColor = System.Drawing.Color.DarkBlue;
            this.labConfStationTypeFilter.Location = new System.Drawing.Point(50, 31);
            this.labConfStationTypeFilter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labConfStationTypeFilter.Name = "labConfStationTypeFilter";
            this.labConfStationTypeFilter.Size = new System.Drawing.Size(71, 17);
            this.labConfStationTypeFilter.TabIndex = 23;
            this.labConfStationTypeFilter.Text = "Type:";
            this.labConfStationTypeFilter.TextAlign = System.Drawing.ContentAlignment.TopRight;
            //
            //cbFilterType
            //
            this.cbFilterType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbFilterType.FormattingEnabled = true;
            this.cbFilterType.Items.AddRange(new object[] { "Soldering", "Hot air" });
            this.cbFilterType.Location = new System.Drawing.Point(128, 28);
            this.cbFilterType.Name = "cbFilterType";
            this.cbFilterType.Size = new System.Drawing.Size(100, 22);
            this.cbFilterType.TabIndex = 24;
            //
            //frmConfManager
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(990, 472);
            this.Controls.Add(this.Splitter1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmConfManager";
            this.Text = "Settings Manager";
            this.panel1.ResumeLayout(false);
            this.Panel1Bot.ResumeLayout(false);
            this.Panel1Bot.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panelStationsView.ResumeLayout(false);
            this.panelStationsView.PerformLayout();
            this.panelStationsList.ResumeLayout(false);
            this.panelStationsList.PerformLayout();
            this.tlpStationList.ResumeLayout(false);
            this.tlpStationList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.port_1_4).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.port_1_3).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.port_1_2).EndInit();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Button butConfSave;
        internal System.Windows.Forms.Button butConfLoad;
        internal System.Windows.Forms.Panel panelTreeView;
        internal System.Windows.Forms.Panel panel1;
        internal System.Windows.Forms.Panel panel2;
        internal System.Windows.Forms.Splitter Splitter1;
        internal System.Windows.Forms.Panel panelStationsView;
        internal System.Windows.Forms.Label labConfTargetStations;
        internal System.Windows.Forms.TableLayoutPanel tlpStationList;
        internal System.Windows.Forms.NumericUpDown port_1_4;
        internal System.Windows.Forms.NumericUpDown port_1_2;
        internal System.Windows.Forms.Label labStation;
        internal System.Windows.Forms.Label labTargetPort_1;
        internal System.Windows.Forms.Label labTargetPort_2;
        internal System.Windows.Forms.Label labTargetPort_3;
        internal System.Windows.Forms.Label labTargetPort_4;
        internal System.Windows.Forms.CheckBox cbST_1;
        internal System.Windows.Forms.NumericUpDown port_1_3;
        internal System.Windows.Forms.Panel panelStationsList;
        internal System.Windows.Forms.ComboBox cbPort_1_1;
        internal System.Windows.Forms.Label labConfSourceSettings;
        internal System.Windows.Forms.Button butConfApply;
        internal System.Windows.Forms.Label labConfComment;
        internal System.Windows.Forms.TextBox tbConfComment;
        internal System.Windows.Forms.Panel Panel1Bot;
        internal System.Windows.Forms.CheckBox CheckBox1;
        internal System.Windows.Forms.Button butRefreshStations;
        internal System.Windows.Forms.Label labConfStationFilter;
        internal System.Windows.Forms.TextBox tbFilterName;
        internal System.Windows.Forms.Label labConfNameFilter;
        internal System.Windows.Forms.TextBox tbFilterModel;
        internal System.Windows.Forms.Label labConfModelFilter;
        internal System.Windows.Forms.Button butPrint;
        internal System.Windows.Forms.Button butConfViewReport;
        internal System.Windows.Forms.Label labLogApply;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Label labConfStationTypeFilter;
        internal System.Windows.Forms.ComboBox cbFilterType;
    }
}
