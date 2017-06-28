// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

namespace LibraryTest
{
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public
    partial class Form1 : System.Windows.Forms.Form
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

        //NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
        //Se puede modificar usando el Diseñador de Windows Forms.
        //No lo modifique con el editor de código.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.type_0 = new System.Windows.Forms.Label();
            this.butSendCommand = new System.Windows.Forms.Button();
            this.cbLogPort = new System.Windows.Forms.ComboBox();
            this.field_1 = new System.Windows.Forms.TextBox();
            this.butClearLog = new System.Windows.Forms.Button();
            this.cbLogTool = new System.Windows.Forms.ComboBox();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.SplitContainer2 = new System.Windows.Forms.SplitContainer();
            this.labCommandProtocol = new System.Windows.Forms.Label();
            this.lbLogOrders = new System.Windows.Forms.ListBox();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tpSendCommand = new System.Windows.Forms.TabPage();
            this.butControlMode = new System.Windows.Forms.Button();
            this.Label16 = new System.Windows.Forms.Label();
            this.tbOpCodeExtended = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.tbSendDataDefault = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.numSendTimes = new System.Windows.Forms.NumericUpDown();
            this.labTargetDevice = new System.Windows.Forms.Label();
            this.tlpInput = new System.Windows.Forms.TableLayoutPanel();
            this.type_9 = new System.Windows.Forms.Label();
            this.type_8 = new System.Windows.Forms.Label();
            this.type_7 = new System.Windows.Forms.Label();
            this.type_6 = new System.Windows.Forms.Label();
            this.type_5 = new System.Windows.Forms.Label();
            this.type_4 = new System.Windows.Forms.Label();
            this.type_3 = new System.Windows.Forms.Label();
            this.type_12 = new System.Windows.Forms.Label();
            this.type_2 = new System.Windows.Forms.Label();
            this.type_11 = new System.Windows.Forms.Label();
            this.type_1 = new System.Windows.Forms.Label();
            this.type_10 = new System.Windows.Forms.Label();
            this.field_12 = new System.Windows.Forms.TextBox();
            this.field_11 = new System.Windows.Forms.TextBox();
            this.field_10 = new System.Windows.Forms.TextBox();
            this.field_0 = new System.Windows.Forms.TextBox();
            this.field_2 = new System.Windows.Forms.TextBox();
            this.field_3 = new System.Windows.Forms.TextBox();
            this.field_4 = new System.Windows.Forms.TextBox();
            this.field_5 = new System.Windows.Forms.TextBox();
            this.field_6 = new System.Windows.Forms.TextBox();
            this.field_7 = new System.Windows.Forms.TextBox();
            this.field_9 = new System.Windows.Forms.TextBox();
            this.field_8 = new System.Windows.Forms.TextBox();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.type_25 = new System.Windows.Forms.Label();
            this.type_13 = new System.Windows.Forms.Label();
            this.field_13 = new System.Windows.Forms.TextBox();
            this.type_14 = new System.Windows.Forms.Label();
            this.field_14 = new System.Windows.Forms.TextBox();
            this.type_15 = new System.Windows.Forms.Label();
            this.field_15 = new System.Windows.Forms.TextBox();
            this.type_16 = new System.Windows.Forms.Label();
            this.field_16 = new System.Windows.Forms.TextBox();
            this.type_17 = new System.Windows.Forms.Label();
            this.field_17 = new System.Windows.Forms.TextBox();
            this.type_18 = new System.Windows.Forms.Label();
            this.field_18 = new System.Windows.Forms.TextBox();
            this.type_19 = new System.Windows.Forms.Label();
            this.field_19 = new System.Windows.Forms.TextBox();
            this.field_20 = new System.Windows.Forms.TextBox();
            this.field_21 = new System.Windows.Forms.TextBox();
            this.field_22 = new System.Windows.Forms.TextBox();
            this.field_23 = new System.Windows.Forms.TextBox();
            this.field_24 = new System.Windows.Forms.TextBox();
            this.type_20 = new System.Windows.Forms.Label();
            this.type_21 = new System.Windows.Forms.Label();
            this.type_22 = new System.Windows.Forms.Label();
            this.type_23 = new System.Windows.Forms.Label();
            this.type_24 = new System.Windows.Forms.Label();
            this.field_25 = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.cbTempType = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.numTargetDevice = new System.Windows.Forms.NumericUpDown();
            this.Label1 = new System.Windows.Forms.Label();
            this.tbSendData = new System.Windows.Forms.TextBox();
            this.tbOpCode = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.tbReceiveData = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.tbAction = new System.Windows.Forms.TextBox();
            this.tpUpdater = new System.Windows.Forms.TabPage();
            this.tbProcessedS19 = new System.Windows.Forms.TextBox();
            this.cbSaveS19ToProcess = new System.Windows.Forms.CheckBox();
            this.cbProgDelay = new System.Windows.Forms.CheckBox();
            this.nudProgDelay = new System.Windows.Forms.NumericUpDown();
            this.cbSendEndProgram = new System.Windows.Forms.CheckBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.tbBorrarMemoriaFlash = new System.Windows.Forms.TextBox();
            this.cbUpdaterTestOnly = new System.Windows.Forms.CheckBox();
            this.labUpdaterProgress = new System.Windows.Forms.Label();
            this.butUpdaterCancel = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.butStepStart = new System.Windows.Forms.Button();
            this.butStepGo = new System.Windows.Forms.Button();
            this.butStepNext = new System.Windows.Forms.Button();
            this.butUpdater = new System.Windows.Forms.Button();
            this.butSearchFile = new System.Windows.Forms.Button();
            this.Label13 = new System.Windows.Forms.Label();
            this.tbProgramFile = new System.Windows.Forms.TextBox();
            this.tpFiles = new System.Windows.Forms.TabPage();
            this.butFiles_SelectedFile = new System.Windows.Forms.Button();
            this.butFiles_SelectFile = new System.Windows.Forms.Button();
            this.butFiles_DeleteFile = new System.Windows.Forms.Button();
            this.butFiles_SendFile = new System.Windows.Forms.Button();
            this.butFiles_GetFile = new System.Windows.Forms.Button();
            this.tbFiles_Extension = new System.Windows.Forms.TextBox();
            this.lbFiles_List = new System.Windows.Forms.ListBox();
            this.butFiles_ListFiles = new System.Windows.Forms.Button();
            this.tpBCC = new System.Windows.Forms.TabPage();
            this.Label20 = new System.Windows.Forms.Label();
            this.nupTargetDevice = new System.Windows.Forms.NumericUpDown();
            this.Label19 = new System.Windows.Forms.Label();
            this.butCheckFrame = new System.Windows.Forms.Button();
            this.tbHexaBytes = new System.Windows.Forms.TextBox();
            this.tpUID = new System.Windows.Forms.TabPage();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.rdGUIDType = new System.Windows.Forms.RadioButton();
            this.rdMACType = new System.Windows.Forms.RadioButton();
            this.butStationToMemory = new System.Windows.Forms.Button();
            this.butMemoryToStation = new System.Windows.Forms.Button();
            this.tbConvertedUID = new System.Windows.Forms.TextBox();
            this.tbStationUID = new System.Windows.Forms.TextBox();
            this.Label18 = new System.Windows.Forms.Label();
            this.tbMemoryUID = new System.Windows.Forms.TextBox();
            this.butNewUID = new System.Windows.Forms.Button();
            this.Label17 = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.gbUSB = new System.Windows.Forms.GroupBox();
            this.cbSerialPorts = new System.Windows.Forms.ComboBox();
            this.butConnectUSB = new System.Windows.Forms.Button();
            this.Label5 = new System.Windows.Forms.Label();
            this.gbEthernet = new System.Windows.Forms.GroupBox();
            this.butConnectEth = new System.Windows.Forms.Button();
            this.cbEndPoints = new System.Windows.Forms.ComboBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.cbViewSYNReceivingData = new System.Windows.Forms.CheckBox();
            this.cbUseOrders = new System.Windows.Forms.ComboBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.butClipboard = new System.Windows.Forms.Button();
            this.OpenFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.timerReadContMode = new System.Windows.Forms.Timer(this.components);
            this.cbLogWrap = new System.Windows.Forms.CheckBox();
            this.timerProgDelay = new System.Windows.Forms.Timer(this.components);
            this.cbViewRAW = new System.Windows.Forms.CheckBox();
            this.groupPortConf = new System.Windows.Forms.GroupBox();
            this.butSerialPortConfigDefaults = new System.Windows.Forms.Button();
            this.Label21 = new System.Windows.Forms.Label();
            this.cbPort_StopBits = new System.Windows.Forms.ComboBox();
            this.Label22 = new System.Windows.Forms.Label();
            this.cbPort_Parity = new System.Windows.Forms.ComboBox();
            this.Label23 = new System.Windows.Forms.Label();
            this.nudPort_DataBits = new System.Windows.Forms.NumericUpDown();
            this.Label24 = new System.Windows.Forms.Label();
            this.cbPort_Speed = new System.Windows.Forms.ComboBox();
            this.SaveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.rbConnectUSB_StartPC = new System.Windows.Forms.RadioButton();
            this.rbConnectUSB_StartStation = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer2)).BeginInit();
            this.SplitContainer2.Panel1.SuspendLayout();
            this.SplitContainer2.Panel2.SuspendLayout();
            this.SplitContainer2.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.tpSendCommand.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSendTimes)).BeginInit();
            this.tlpInput.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetDevice)).BeginInit();
            this.tpUpdater.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudProgDelay)).BeginInit();
            this.GroupBox1.SuspendLayout();
            this.tpFiles.SuspendLayout();
            this.tpBCC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupTargetDevice)).BeginInit();
            this.tpUID.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.gbUSB.SuspendLayout();
            this.gbEthernet.SuspendLayout();
            this.groupPortConf.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort_DataBits)).BeginInit();
            this.GroupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(195, 63);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(26, 13);
            this.Label3.TabIndex = 15;
            this.Label3.Text = "Port";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(345, 63);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(28, 13);
            this.Label2.TabIndex = 14;
            this.Label2.Text = "Tool";
            // 
            // type_0
            // 
            this.type_0.AutoSize = true;
            this.type_0.Location = new System.Drawing.Point(3, 0);
            this.type_0.Name = "type_0";
            this.type_0.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_0.Size = new System.Drawing.Size(31, 19);
            this.type_0.TabIndex = 13;
            this.type_0.Text = "Type";
            // 
            // butSendCommand
            // 
            this.butSendCommand.Location = new System.Drawing.Point(16, 441);
            this.butSendCommand.Name = "butSendCommand";
            this.butSendCommand.Size = new System.Drawing.Size(119, 23);
            this.butSendCommand.TabIndex = 9;
            this.butSendCommand.Text = "Send Command";
            this.butSendCommand.UseVisualStyleBackColor = true;
            this.butSendCommand.Click += new System.EventHandler(this.butLogSend_Click);
            // 
            // cbLogPort
            // 
            this.cbLogPort.FormattingEnabled = true;
            this.cbLogPort.Items.AddRange(new object[] {
            "No Port",
            "1",
            "2",
            "3",
            "4"});
            this.cbLogPort.Location = new System.Drawing.Point(227, 60);
            this.cbLogPort.Name = "cbLogPort";
            this.cbLogPort.Size = new System.Drawing.Size(94, 21);
            this.cbLogPort.TabIndex = 4;
            // 
            // field_1
            // 
            this.field_1.Location = new System.Drawing.Point(83, 23);
            this.field_1.Name = "field_1";
            this.field_1.Size = new System.Drawing.Size(131, 20);
            this.field_1.TabIndex = 8;
            // 
            // butClearLog
            // 
            this.butClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butClearLog.Location = new System.Drawing.Point(1088, 810);
            this.butClearLog.Name = "butClearLog";
            this.butClearLog.Size = new System.Drawing.Size(75, 23);
            this.butClearLog.TabIndex = 10;
            this.butClearLog.Text = "Clear Log";
            this.butClearLog.UseVisualStyleBackColor = true;
            this.butClearLog.Click += new System.EventHandler(this.butClearLog_Click);
            // 
            // cbLogTool
            // 
            this.cbLogTool.FormattingEnabled = true;
            this.cbLogTool.Location = new System.Drawing.Point(379, 60);
            this.cbLogTool.Name = "cbLogTool";
            this.cbLogTool.Size = new System.Drawing.Size(106, 21);
            this.cbLogTool.TabIndex = 5;
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer1.BackColor = System.Drawing.Color.PeachPuff;
            this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainer1.Location = new System.Drawing.Point(3, 2);
            this.SplitContainer1.Name = "SplitContainer1";
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.SplitContainer1.Panel1.Controls.Add(this.SplitContainer2);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.SplitContainer1.Panel2.Controls.Add(this.tbLog);
            this.SplitContainer1.Panel2.Controls.Add(this.gbUSB);
            this.SplitContainer1.Panel2.Controls.Add(this.gbEthernet);
            this.SplitContainer1.Size = new System.Drawing.Size(1174, 802);
            this.SplitContainer1.SplitterDistance = 508;
            this.SplitContainer1.SplitterWidth = 8;
            this.SplitContainer1.TabIndex = 11;
            // 
            // SplitContainer2
            // 
            this.SplitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SplitContainer2.BackColor = System.Drawing.Color.PeachPuff;
            this.SplitContainer2.Location = new System.Drawing.Point(3, 5);
            this.SplitContainer2.Name = "SplitContainer2";
            this.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer2.Panel1
            // 
            this.SplitContainer2.Panel1.BackColor = System.Drawing.SystemColors.Control;
            this.SplitContainer2.Panel1.Controls.Add(this.labCommandProtocol);
            this.SplitContainer2.Panel1.Controls.Add(this.lbLogOrders);
            // 
            // SplitContainer2.Panel2
            // 
            this.SplitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.SplitContainer2.Panel2.Controls.Add(this.TabControl1);
            this.SplitContainer2.Size = new System.Drawing.Size(502, 794);
            this.SplitContainer2.SplitterDistance = 278;
            this.SplitContainer2.TabIndex = 29;
            // 
            // labCommandProtocol
            // 
            this.labCommandProtocol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labCommandProtocol.Location = new System.Drawing.Point(3, 259);
            this.labCommandProtocol.Name = "labCommandProtocol";
            this.labCommandProtocol.Size = new System.Drawing.Size(464, 14);
            this.labCommandProtocol.TabIndex = 5;
            this.labCommandProtocol.Text = "Command protocol:";
            // 
            // lbLogOrders
            // 
            this.lbLogOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLogOrders.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLogOrders.FormattingEnabled = true;
            this.lbLogOrders.HorizontalScrollbar = true;
            this.lbLogOrders.ItemHeight = 14;
            this.lbLogOrders.Location = new System.Drawing.Point(3, 3);
            this.lbLogOrders.Name = "lbLogOrders";
            this.lbLogOrders.Size = new System.Drawing.Size(496, 242);
            this.lbLogOrders.TabIndex = 4;
            this.lbLogOrders.SelectedValueChanged += new System.EventHandler(this.lbLogOrders_SelectedValueChanged);
            // 
            // TabControl1
            // 
            this.TabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl1.Controls.Add(this.tpSendCommand);
            this.TabControl1.Controls.Add(this.tpUpdater);
            this.TabControl1.Controls.Add(this.tpFiles);
            this.TabControl1.Controls.Add(this.tpBCC);
            this.TabControl1.Controls.Add(this.tpUID);
            this.TabControl1.Location = new System.Drawing.Point(3, 3);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(499, 506);
            this.TabControl1.TabIndex = 33;
            // 
            // tpSendCommand
            // 
            this.tpSendCommand.Controls.Add(this.butControlMode);
            this.tpSendCommand.Controls.Add(this.Label16);
            this.tpSendCommand.Controls.Add(this.tbOpCodeExtended);
            this.tpSendCommand.Controls.Add(this.Label15);
            this.tpSendCommand.Controls.Add(this.tbSendDataDefault);
            this.tpSendCommand.Controls.Add(this.Label6);
            this.tpSendCommand.Controls.Add(this.numSendTimes);
            this.tpSendCommand.Controls.Add(this.labTargetDevice);
            this.tpSendCommand.Controls.Add(this.tlpInput);
            this.tpSendCommand.Controls.Add(this.TableLayoutPanel1);
            this.tpSendCommand.Controls.Add(this.Label3);
            this.tpSendCommand.Controls.Add(this.Label8);
            this.tpSendCommand.Controls.Add(this.Label12);
            this.tpSendCommand.Controls.Add(this.cbTempType);
            this.tpSendCommand.Controls.Add(this.Label2);
            this.tpSendCommand.Controls.Add(this.Label4);
            this.tpSendCommand.Controls.Add(this.numTargetDevice);
            this.tpSendCommand.Controls.Add(this.butSendCommand);
            this.tpSendCommand.Controls.Add(this.Label1);
            this.tpSendCommand.Controls.Add(this.cbLogPort);
            this.tpSendCommand.Controls.Add(this.tbSendData);
            this.tpSendCommand.Controls.Add(this.cbLogTool);
            this.tpSendCommand.Controls.Add(this.tbOpCode);
            this.tpSendCommand.Controls.Add(this.Label7);
            this.tpSendCommand.Controls.Add(this.tbReceiveData);
            this.tpSendCommand.Controls.Add(this.Label9);
            this.tpSendCommand.Controls.Add(this.tbAction);
            this.tpSendCommand.Location = new System.Drawing.Point(4, 22);
            this.tpSendCommand.Name = "tpSendCommand";
            this.tpSendCommand.Padding = new System.Windows.Forms.Padding(3);
            this.tpSendCommand.Size = new System.Drawing.Size(491, 480);
            this.tpSendCommand.TabIndex = 0;
            this.tpSendCommand.Text = "Send Commands";
            this.tpSendCommand.UseVisualStyleBackColor = true;
            // 
            // butControlMode
            // 
            this.butControlMode.Location = new System.Drawing.Point(333, 107);
            this.butControlMode.Name = "butControlMode";
            this.butControlMode.Size = new System.Drawing.Size(131, 23);
            this.butControlMode.TabIndex = 37;
            this.butControlMode.Text = "Set Control Mode";
            this.butControlMode.UseVisualStyleBackColor = true;
            this.butControlMode.Click += new System.EventHandler(this.butControlMode_Click);
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Location = new System.Drawing.Point(97, 63);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(52, 13);
            this.Label16.TabIndex = 36;
            this.Label16.Text = "Extended";
            // 
            // tbOpCodeExtended
            // 
            this.tbOpCodeExtended.Location = new System.Drawing.Point(151, 60);
            this.tbOpCodeExtended.MaxLength = 5;
            this.tbOpCodeExtended.Name = "tbOpCodeExtended";
            this.tbOpCodeExtended.Size = new System.Drawing.Size(35, 20);
            this.tbOpCodeExtended.TabIndex = 35;
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Location = new System.Drawing.Point(12, 113);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(67, 13);
            this.Label15.TabIndex = 34;
            this.Label15.Text = "Default Data";
            // 
            // tbSendDataDefault
            // 
            this.tbSendDataDefault.Location = new System.Drawing.Point(85, 110);
            this.tbSendDataDefault.MaxLength = 3;
            this.tbSendDataDefault.Name = "tbSendDataDefault";
            this.tbSendDataDefault.Size = new System.Drawing.Size(204, 20);
            this.tbSendDataDefault.TabIndex = 33;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(6, 13);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(88, 13);
            this.Label6.TabIndex = 21;
            this.Label6.Text = "Send Data Fields";
            // 
            // numSendTimes
            // 
            this.numSendTimes.Location = new System.Drawing.Point(162, 443);
            this.numSendTimes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numSendTimes.Name = "numSendTimes";
            this.numSendTimes.Size = new System.Drawing.Size(50, 20);
            this.numSendTimes.TabIndex = 29;
            this.numSendTimes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // labTargetDevice
            // 
            this.labTargetDevice.AutoSize = true;
            this.labTargetDevice.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labTargetDevice.Location = new System.Drawing.Point(295, 87);
            this.labTargetDevice.Name = "labTargetDevice";
            this.labTargetDevice.Size = new System.Drawing.Size(30, 13);
            this.labTargetDevice.TabIndex = 32;
            this.labTargetDevice.Text = "H00";
            // 
            // tlpInput
            // 
            this.tlpInput.ColumnCount = 2;
            this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.15596F));
            this.tlpInput.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.84404F));
            this.tlpInput.Controls.Add(this.type_9, 0, 9);
            this.tlpInput.Controls.Add(this.type_8, 0, 8);
            this.tlpInput.Controls.Add(this.type_7, 0, 7);
            this.tlpInput.Controls.Add(this.type_6, 0, 6);
            this.tlpInput.Controls.Add(this.type_5, 0, 5);
            this.tlpInput.Controls.Add(this.type_4, 0, 4);
            this.tlpInput.Controls.Add(this.type_3, 0, 3);
            this.tlpInput.Controls.Add(this.type_12, 0, 12);
            this.tlpInput.Controls.Add(this.type_2, 0, 2);
            this.tlpInput.Controls.Add(this.type_11, 0, 11);
            this.tlpInput.Controls.Add(this.type_1, 0, 1);
            this.tlpInput.Controls.Add(this.type_10, 0, 10);
            this.tlpInput.Controls.Add(this.type_0, 0, 0);
            this.tlpInput.Controls.Add(this.field_12, 1, 12);
            this.tlpInput.Controls.Add(this.field_11, 1, 11);
            this.tlpInput.Controls.Add(this.field_10, 1, 10);
            this.tlpInput.Controls.Add(this.field_0, 1, 0);
            this.tlpInput.Controls.Add(this.field_1, 1, 1);
            this.tlpInput.Controls.Add(this.field_2, 1, 2);
            this.tlpInput.Controls.Add(this.field_3, 1, 3);
            this.tlpInput.Controls.Add(this.field_4, 1, 4);
            this.tlpInput.Controls.Add(this.field_5, 1, 5);
            this.tlpInput.Controls.Add(this.field_6, 1, 6);
            this.tlpInput.Controls.Add(this.field_7, 1, 7);
            this.tlpInput.Controls.Add(this.field_9, 1, 9);
            this.tlpInput.Controls.Add(this.field_8, 1, 8);
            this.tlpInput.Location = new System.Drawing.Point(20, 154);
            this.tlpInput.Name = "tlpInput";
            this.tlpInput.RowCount = 13;
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpInput.Size = new System.Drawing.Size(218, 281);
            this.tlpInput.TabIndex = 7;
            // 
            // type_9
            // 
            this.type_9.AutoSize = true;
            this.type_9.Location = new System.Drawing.Point(3, 180);
            this.type_9.Name = "type_9";
            this.type_9.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_9.Size = new System.Drawing.Size(31, 19);
            this.type_9.TabIndex = 31;
            this.type_9.Text = "Type";
            // 
            // type_8
            // 
            this.type_8.AutoSize = true;
            this.type_8.Location = new System.Drawing.Point(3, 160);
            this.type_8.Name = "type_8";
            this.type_8.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_8.Size = new System.Drawing.Size(31, 19);
            this.type_8.TabIndex = 29;
            this.type_8.Text = "Type";
            // 
            // type_7
            // 
            this.type_7.AutoSize = true;
            this.type_7.Location = new System.Drawing.Point(3, 140);
            this.type_7.Name = "type_7";
            this.type_7.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_7.Size = new System.Drawing.Size(31, 19);
            this.type_7.TabIndex = 27;
            this.type_7.Text = "Type";
            // 
            // type_6
            // 
            this.type_6.AutoSize = true;
            this.type_6.Location = new System.Drawing.Point(3, 120);
            this.type_6.Name = "type_6";
            this.type_6.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_6.Size = new System.Drawing.Size(31, 19);
            this.type_6.TabIndex = 25;
            this.type_6.Text = "Type";
            // 
            // type_5
            // 
            this.type_5.AutoSize = true;
            this.type_5.Location = new System.Drawing.Point(3, 100);
            this.type_5.Name = "type_5";
            this.type_5.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_5.Size = new System.Drawing.Size(31, 19);
            this.type_5.TabIndex = 23;
            this.type_5.Text = "Type";
            // 
            // type_4
            // 
            this.type_4.AutoSize = true;
            this.type_4.Location = new System.Drawing.Point(3, 80);
            this.type_4.Name = "type_4";
            this.type_4.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_4.Size = new System.Drawing.Size(31, 19);
            this.type_4.TabIndex = 21;
            this.type_4.Text = "Type";
            // 
            // type_3
            // 
            this.type_3.AutoSize = true;
            this.type_3.Location = new System.Drawing.Point(3, 60);
            this.type_3.Name = "type_3";
            this.type_3.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_3.Size = new System.Drawing.Size(31, 19);
            this.type_3.TabIndex = 19;
            this.type_3.Text = "Type";
            // 
            // type_12
            // 
            this.type_12.AutoSize = true;
            this.type_12.Location = new System.Drawing.Point(3, 240);
            this.type_12.Name = "type_12";
            this.type_12.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_12.Size = new System.Drawing.Size(31, 19);
            this.type_12.TabIndex = 17;
            this.type_12.Text = "Type";
            // 
            // type_2
            // 
            this.type_2.AutoSize = true;
            this.type_2.Location = new System.Drawing.Point(3, 40);
            this.type_2.Name = "type_2";
            this.type_2.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_2.Size = new System.Drawing.Size(31, 19);
            this.type_2.TabIndex = 17;
            this.type_2.Text = "Type";
            // 
            // type_11
            // 
            this.type_11.AutoSize = true;
            this.type_11.Location = new System.Drawing.Point(3, 220);
            this.type_11.Name = "type_11";
            this.type_11.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_11.Size = new System.Drawing.Size(31, 19);
            this.type_11.TabIndex = 15;
            this.type_11.Text = "Type";
            // 
            // type_1
            // 
            this.type_1.AutoSize = true;
            this.type_1.Location = new System.Drawing.Point(3, 20);
            this.type_1.Name = "type_1";
            this.type_1.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_1.Size = new System.Drawing.Size(31, 19);
            this.type_1.TabIndex = 15;
            this.type_1.Text = "Type";
            // 
            // type_10
            // 
            this.type_10.AutoSize = true;
            this.type_10.Location = new System.Drawing.Point(3, 200);
            this.type_10.Name = "type_10";
            this.type_10.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_10.Size = new System.Drawing.Size(31, 19);
            this.type_10.TabIndex = 13;
            this.type_10.Text = "Type";
            // 
            // field_12
            // 
            this.field_12.Location = new System.Drawing.Point(83, 243);
            this.field_12.Name = "field_12";
            this.field_12.Size = new System.Drawing.Size(131, 20);
            this.field_12.TabIndex = 19;
            // 
            // field_11
            // 
            this.field_11.Location = new System.Drawing.Point(83, 223);
            this.field_11.Name = "field_11";
            this.field_11.Size = new System.Drawing.Size(131, 20);
            this.field_11.TabIndex = 18;
            // 
            // field_10
            // 
            this.field_10.Location = new System.Drawing.Point(83, 203);
            this.field_10.Name = "field_10";
            this.field_10.Size = new System.Drawing.Size(131, 20);
            this.field_10.TabIndex = 17;
            // 
            // field_0
            // 
            this.field_0.Location = new System.Drawing.Point(83, 3);
            this.field_0.Name = "field_0";
            this.field_0.Size = new System.Drawing.Size(131, 20);
            this.field_0.TabIndex = 7;
            // 
            // field_2
            // 
            this.field_2.Location = new System.Drawing.Point(83, 43);
            this.field_2.Name = "field_2";
            this.field_2.Size = new System.Drawing.Size(131, 20);
            this.field_2.TabIndex = 9;
            // 
            // field_3
            // 
            this.field_3.Location = new System.Drawing.Point(83, 63);
            this.field_3.Name = "field_3";
            this.field_3.Size = new System.Drawing.Size(131, 20);
            this.field_3.TabIndex = 10;
            // 
            // field_4
            // 
            this.field_4.Location = new System.Drawing.Point(83, 83);
            this.field_4.Name = "field_4";
            this.field_4.Size = new System.Drawing.Size(131, 20);
            this.field_4.TabIndex = 11;
            // 
            // field_5
            // 
            this.field_5.Location = new System.Drawing.Point(83, 103);
            this.field_5.Name = "field_5";
            this.field_5.Size = new System.Drawing.Size(131, 20);
            this.field_5.TabIndex = 12;
            // 
            // field_6
            // 
            this.field_6.Location = new System.Drawing.Point(83, 123);
            this.field_6.Name = "field_6";
            this.field_6.Size = new System.Drawing.Size(131, 20);
            this.field_6.TabIndex = 13;
            // 
            // field_7
            // 
            this.field_7.Location = new System.Drawing.Point(83, 143);
            this.field_7.Name = "field_7";
            this.field_7.Size = new System.Drawing.Size(131, 20);
            this.field_7.TabIndex = 14;
            // 
            // field_9
            // 
            this.field_9.Location = new System.Drawing.Point(83, 183);
            this.field_9.Name = "field_9";
            this.field_9.Size = new System.Drawing.Size(131, 20);
            this.field_9.TabIndex = 16;
            // 
            // field_8
            // 
            this.field_8.Location = new System.Drawing.Point(83, 163);
            this.field_8.Name = "field_8";
            this.field_8.Size = new System.Drawing.Size(131, 20);
            this.field_8.TabIndex = 15;
            // 
            // TableLayoutPanel1
            // 
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37.15596F));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 62.84404F));
            this.TableLayoutPanel1.Controls.Add(this.type_25, 0, 12);
            this.TableLayoutPanel1.Controls.Add(this.type_13, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.field_13, 1, 0);
            this.TableLayoutPanel1.Controls.Add(this.type_14, 0, 1);
            this.TableLayoutPanel1.Controls.Add(this.field_14, 1, 1);
            this.TableLayoutPanel1.Controls.Add(this.type_15, 0, 2);
            this.TableLayoutPanel1.Controls.Add(this.field_15, 1, 2);
            this.TableLayoutPanel1.Controls.Add(this.type_16, 0, 3);
            this.TableLayoutPanel1.Controls.Add(this.field_16, 1, 3);
            this.TableLayoutPanel1.Controls.Add(this.type_17, 0, 4);
            this.TableLayoutPanel1.Controls.Add(this.field_17, 1, 4);
            this.TableLayoutPanel1.Controls.Add(this.type_18, 0, 5);
            this.TableLayoutPanel1.Controls.Add(this.field_18, 1, 5);
            this.TableLayoutPanel1.Controls.Add(this.type_19, 0, 6);
            this.TableLayoutPanel1.Controls.Add(this.field_19, 1, 6);
            this.TableLayoutPanel1.Controls.Add(this.field_20, 1, 7);
            this.TableLayoutPanel1.Controls.Add(this.field_21, 1, 8);
            this.TableLayoutPanel1.Controls.Add(this.field_22, 1, 9);
            this.TableLayoutPanel1.Controls.Add(this.field_23, 1, 10);
            this.TableLayoutPanel1.Controls.Add(this.field_24, 1, 11);
            this.TableLayoutPanel1.Controls.Add(this.type_20, 0, 7);
            this.TableLayoutPanel1.Controls.Add(this.type_21, 0, 8);
            this.TableLayoutPanel1.Controls.Add(this.type_22, 0, 9);
            this.TableLayoutPanel1.Controls.Add(this.type_23, 0, 10);
            this.TableLayoutPanel1.Controls.Add(this.type_24, 0, 11);
            this.TableLayoutPanel1.Controls.Add(this.field_25, 1, 12);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(250, 156);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 13;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(218, 279);
            this.TableLayoutPanel1.TabIndex = 8;
            // 
            // type_25
            // 
            this.type_25.AutoSize = true;
            this.type_25.Location = new System.Drawing.Point(3, 240);
            this.type_25.Name = "type_25";
            this.type_25.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_25.Size = new System.Drawing.Size(31, 19);
            this.type_25.TabIndex = 42;
            this.type_25.Text = "Type";
            // 
            // type_13
            // 
            this.type_13.AutoSize = true;
            this.type_13.Location = new System.Drawing.Point(3, 0);
            this.type_13.Name = "type_13";
            this.type_13.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_13.Size = new System.Drawing.Size(31, 19);
            this.type_13.TabIndex = 19;
            this.type_13.Text = "Type";
            // 
            // field_13
            // 
            this.field_13.Location = new System.Drawing.Point(83, 3);
            this.field_13.Name = "field_13";
            this.field_13.Size = new System.Drawing.Size(131, 20);
            this.field_13.TabIndex = 20;
            // 
            // type_14
            // 
            this.type_14.AutoSize = true;
            this.type_14.Location = new System.Drawing.Point(3, 20);
            this.type_14.Name = "type_14";
            this.type_14.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_14.Size = new System.Drawing.Size(31, 19);
            this.type_14.TabIndex = 21;
            this.type_14.Text = "Type";
            // 
            // field_14
            // 
            this.field_14.Location = new System.Drawing.Point(83, 23);
            this.field_14.Name = "field_14";
            this.field_14.Size = new System.Drawing.Size(131, 20);
            this.field_14.TabIndex = 21;
            // 
            // type_15
            // 
            this.type_15.AutoSize = true;
            this.type_15.Location = new System.Drawing.Point(3, 40);
            this.type_15.Name = "type_15";
            this.type_15.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_15.Size = new System.Drawing.Size(31, 19);
            this.type_15.TabIndex = 23;
            this.type_15.Text = "Type";
            // 
            // field_15
            // 
            this.field_15.Location = new System.Drawing.Point(83, 43);
            this.field_15.Name = "field_15";
            this.field_15.Size = new System.Drawing.Size(131, 20);
            this.field_15.TabIndex = 22;
            // 
            // type_16
            // 
            this.type_16.AutoSize = true;
            this.type_16.Location = new System.Drawing.Point(3, 60);
            this.type_16.Name = "type_16";
            this.type_16.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_16.Size = new System.Drawing.Size(31, 19);
            this.type_16.TabIndex = 25;
            this.type_16.Text = "Type";
            // 
            // field_16
            // 
            this.field_16.Location = new System.Drawing.Point(83, 63);
            this.field_16.Name = "field_16";
            this.field_16.Size = new System.Drawing.Size(131, 20);
            this.field_16.TabIndex = 23;
            // 
            // type_17
            // 
            this.type_17.AutoSize = true;
            this.type_17.Location = new System.Drawing.Point(3, 80);
            this.type_17.Name = "type_17";
            this.type_17.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_17.Size = new System.Drawing.Size(31, 19);
            this.type_17.TabIndex = 27;
            this.type_17.Text = "Type";
            // 
            // field_17
            // 
            this.field_17.Location = new System.Drawing.Point(83, 83);
            this.field_17.Name = "field_17";
            this.field_17.Size = new System.Drawing.Size(131, 20);
            this.field_17.TabIndex = 24;
            // 
            // type_18
            // 
            this.type_18.AutoSize = true;
            this.type_18.Location = new System.Drawing.Point(3, 100);
            this.type_18.Name = "type_18";
            this.type_18.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_18.Size = new System.Drawing.Size(31, 19);
            this.type_18.TabIndex = 29;
            this.type_18.Text = "Type";
            // 
            // field_18
            // 
            this.field_18.Location = new System.Drawing.Point(83, 103);
            this.field_18.Name = "field_18";
            this.field_18.Size = new System.Drawing.Size(131, 20);
            this.field_18.TabIndex = 25;
            // 
            // type_19
            // 
            this.type_19.AutoSize = true;
            this.type_19.Location = new System.Drawing.Point(3, 120);
            this.type_19.Name = "type_19";
            this.type_19.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_19.Size = new System.Drawing.Size(31, 19);
            this.type_19.TabIndex = 31;
            this.type_19.Text = "Type";
            // 
            // field_19
            // 
            this.field_19.Location = new System.Drawing.Point(83, 123);
            this.field_19.Name = "field_19";
            this.field_19.Size = new System.Drawing.Size(131, 20);
            this.field_19.TabIndex = 26;
            // 
            // field_20
            // 
            this.field_20.Location = new System.Drawing.Point(83, 143);
            this.field_20.Name = "field_20";
            this.field_20.Size = new System.Drawing.Size(131, 20);
            this.field_20.TabIndex = 32;
            // 
            // field_21
            // 
            this.field_21.Location = new System.Drawing.Point(83, 163);
            this.field_21.Name = "field_21";
            this.field_21.Size = new System.Drawing.Size(131, 20);
            this.field_21.TabIndex = 33;
            // 
            // field_22
            // 
            this.field_22.Location = new System.Drawing.Point(83, 183);
            this.field_22.Name = "field_22";
            this.field_22.Size = new System.Drawing.Size(131, 20);
            this.field_22.TabIndex = 34;
            // 
            // field_23
            // 
            this.field_23.Location = new System.Drawing.Point(83, 203);
            this.field_23.Name = "field_23";
            this.field_23.Size = new System.Drawing.Size(131, 20);
            this.field_23.TabIndex = 35;
            // 
            // field_24
            // 
            this.field_24.Location = new System.Drawing.Point(83, 223);
            this.field_24.Name = "field_24";
            this.field_24.Size = new System.Drawing.Size(131, 20);
            this.field_24.TabIndex = 36;
            // 
            // type_20
            // 
            this.type_20.AutoSize = true;
            this.type_20.Location = new System.Drawing.Point(3, 140);
            this.type_20.Name = "type_20";
            this.type_20.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_20.Size = new System.Drawing.Size(31, 19);
            this.type_20.TabIndex = 37;
            this.type_20.Text = "Type";
            // 
            // type_21
            // 
            this.type_21.AutoSize = true;
            this.type_21.Location = new System.Drawing.Point(3, 160);
            this.type_21.Name = "type_21";
            this.type_21.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_21.Size = new System.Drawing.Size(31, 19);
            this.type_21.TabIndex = 38;
            this.type_21.Text = "Type";
            // 
            // type_22
            // 
            this.type_22.AutoSize = true;
            this.type_22.Location = new System.Drawing.Point(3, 180);
            this.type_22.Name = "type_22";
            this.type_22.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_22.Size = new System.Drawing.Size(31, 19);
            this.type_22.TabIndex = 39;
            this.type_22.Text = "Type";
            // 
            // type_23
            // 
            this.type_23.AutoSize = true;
            this.type_23.Location = new System.Drawing.Point(3, 200);
            this.type_23.Name = "type_23";
            this.type_23.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_23.Size = new System.Drawing.Size(31, 19);
            this.type_23.TabIndex = 40;
            this.type_23.Text = "Type";
            // 
            // type_24
            // 
            this.type_24.AutoSize = true;
            this.type_24.Location = new System.Drawing.Point(3, 220);
            this.type_24.Name = "type_24";
            this.type_24.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.type_24.Size = new System.Drawing.Size(31, 19);
            this.type_24.TabIndex = 41;
            this.type_24.Text = "Type";
            // 
            // field_25
            // 
            this.field_25.Location = new System.Drawing.Point(83, 243);
            this.field_25.Name = "field_25";
            this.field_25.Size = new System.Drawing.Size(131, 20);
            this.field_25.TabIndex = 43;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(201, 138);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(83, 13);
            this.Label8.TabIndex = 24;
            this.Label8.Text = "Data to be send";
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Location = new System.Drawing.Point(163, 86);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(75, 13);
            this.Label12.TabIndex = 31;
            this.Label12.Text = "Target Device";
            // 
            // cbTempType
            // 
            this.cbTempType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTempType.FormattingEnabled = true;
            this.cbTempType.Items.AddRange(new object[] {
            "UTI",
            "Celsious",
            "Faherenheit"});
            this.cbTempType.Location = new System.Drawing.Point(321, 443);
            this.cbTempType.Margin = new System.Windows.Forms.Padding(4);
            this.cbTempType.Name = "cbTempType";
            this.cbTempType.Size = new System.Drawing.Size(124, 21);
            this.cbTempType.TabIndex = 10;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(262, 446);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(48, 13);
            this.Label4.TabIndex = 25;
            this.Label4.Text = "Temp as";
            // 
            // numTargetDevice
            // 
            this.numTargetDevice.Location = new System.Drawing.Point(244, 84);
            this.numTargetDevice.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numTargetDevice.Name = "numTargetDevice";
            this.numTargetDevice.Size = new System.Drawing.Size(45, 20);
            this.numTargetDevice.TabIndex = 30;
            this.numTargetDevice.ValueChanged += new System.EventHandler(this.numTargetDevice_ValueChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(7, 63);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(46, 13);
            this.Label1.TabIndex = 19;
            this.Label1.Text = "OpCode";
            // 
            // tbSendData
            // 
            this.tbSendData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSendData.Location = new System.Drawing.Point(118, 10);
            this.tbSendData.Name = "tbSendData";
            this.tbSendData.Size = new System.Drawing.Size(367, 20);
            this.tbSendData.TabIndex = 0;
            this.tbSendData.Leave += new System.EventHandler(this.tbSendData_Leave);
            // 
            // tbOpCode
            // 
            this.tbOpCode.Location = new System.Drawing.Point(54, 60);
            this.tbOpCode.MaxLength = 5;
            this.tbOpCode.Name = "tbOpCode";
            this.tbOpCode.Size = new System.Drawing.Size(35, 20);
            this.tbOpCode.TabIndex = 3;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(6, 37);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(111, 13);
            this.Label7.TabIndex = 23;
            this.Label7.Text = "Response Data Fields";
            // 
            // tbReceiveData
            // 
            this.tbReceiveData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbReceiveData.Location = new System.Drawing.Point(118, 34);
            this.tbReceiveData.Name = "tbReceiveData";
            this.tbReceiveData.Size = new System.Drawing.Size(367, 20);
            this.tbReceiveData.TabIndex = 1;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(9, 87);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(37, 13);
            this.Label9.TabIndex = 28;
            this.Label9.Text = "Action";
            // 
            // tbAction
            // 
            this.tbAction.Location = new System.Drawing.Point(54, 83);
            this.tbAction.MaxLength = 3;
            this.tbAction.Name = "tbAction";
            this.tbAction.Size = new System.Drawing.Size(75, 20);
            this.tbAction.TabIndex = 6;
            // 
            // tpUpdater
            // 
            this.tpUpdater.Controls.Add(this.tbProcessedS19);
            this.tpUpdater.Controls.Add(this.cbSaveS19ToProcess);
            this.tpUpdater.Controls.Add(this.cbProgDelay);
            this.tpUpdater.Controls.Add(this.nudProgDelay);
            this.tpUpdater.Controls.Add(this.cbSendEndProgram);
            this.tpUpdater.Controls.Add(this.Label14);
            this.tpUpdater.Controls.Add(this.tbBorrarMemoriaFlash);
            this.tpUpdater.Controls.Add(this.cbUpdaterTestOnly);
            this.tpUpdater.Controls.Add(this.labUpdaterProgress);
            this.tpUpdater.Controls.Add(this.butUpdaterCancel);
            this.tpUpdater.Controls.Add(this.GroupBox1);
            this.tpUpdater.Controls.Add(this.butUpdater);
            this.tpUpdater.Controls.Add(this.butSearchFile);
            this.tpUpdater.Controls.Add(this.Label13);
            this.tpUpdater.Controls.Add(this.tbProgramFile);
            this.tpUpdater.Location = new System.Drawing.Point(4, 22);
            this.tpUpdater.Name = "tpUpdater";
            this.tpUpdater.Padding = new System.Windows.Forms.Padding(3);
            this.tpUpdater.Size = new System.Drawing.Size(491, 480);
            this.tpUpdater.TabIndex = 1;
            this.tpUpdater.Text = "Updater";
            this.tpUpdater.UseVisualStyleBackColor = true;
            // 
            // tbProcessedS19
            // 
            this.tbProcessedS19.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProcessedS19.Location = new System.Drawing.Point(20, 211);
            this.tbProcessedS19.Multiline = true;
            this.tbProcessedS19.Name = "tbProcessedS19";
            this.tbProcessedS19.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbProcessedS19.Size = new System.Drawing.Size(461, 46);
            this.tbProcessedS19.TabIndex = 39;
            // 
            // cbSaveS19ToProcess
            // 
            this.cbSaveS19ToProcess.AutoSize = true;
            this.cbSaveS19ToProcess.Location = new System.Drawing.Point(20, 188);
            this.cbSaveS19ToProcess.Name = "cbSaveS19ToProcess";
            this.cbSaveS19ToProcess.Size = new System.Drawing.Size(125, 17);
            this.cbSaveS19ToProcess.TabIndex = 38;
            this.cbSaveS19ToProcess.Text = "Save processed S19";
            this.cbSaveS19ToProcess.UseVisualStyleBackColor = true;
            // 
            // cbProgDelay
            // 
            this.cbProgDelay.AutoSize = true;
            this.cbProgDelay.Location = new System.Drawing.Point(20, 160);
            this.cbProgDelay.Name = "cbProgDelay";
            this.cbProgDelay.Size = new System.Drawing.Size(282, 17);
            this.cbProgDelay.TabIndex = 37;
            this.cbProgDelay.Text = "Milisegundos de espera entre envíos de programación";
            this.cbProgDelay.UseVisualStyleBackColor = true;
            this.cbProgDelay.CheckedChanged += new System.EventHandler(this.cbProgDelay_CheckedChanged);
            // 
            // nudProgDelay
            // 
            this.nudProgDelay.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.nudProgDelay.Location = new System.Drawing.Point(305, 159);
            this.nudProgDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudProgDelay.Name = "nudProgDelay";
            this.nudProgDelay.Size = new System.Drawing.Size(78, 20);
            this.nudProgDelay.TabIndex = 35;
            this.nudProgDelay.ValueChanged += new System.EventHandler(this.nudProgDelay_ValueChanged);
            // 
            // cbSendEndProgram
            // 
            this.cbSendEndProgram.AutoSize = true;
            this.cbSendEndProgram.Checked = true;
            this.cbSendEndProgram.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSendEndProgram.Location = new System.Drawing.Point(98, 42);
            this.cbSendEndProgram.Name = "cbSendEndProgram";
            this.cbSendEndProgram.Size = new System.Drawing.Size(115, 17);
            this.cbSendEndProgram.TabIndex = 34;
            this.cbSendEndProgram.Text = "Send End Program";
            this.cbSendEndProgram.UseVisualStyleBackColor = true;
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Location = new System.Drawing.Point(17, 137);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(212, 13);
            this.Label14.TabIndex = 33;
            this.Label14.Text = "Datos a enviar al Borrar memoria flash (22h)";
            // 
            // tbBorrarMemoriaFlash
            // 
            this.tbBorrarMemoriaFlash.Location = new System.Drawing.Point(235, 134);
            this.tbBorrarMemoriaFlash.Name = "tbBorrarMemoriaFlash";
            this.tbBorrarMemoriaFlash.Size = new System.Drawing.Size(205, 20);
            this.tbBorrarMemoriaFlash.TabIndex = 32;
            this.tbBorrarMemoriaFlash.Text = "02:PSE_LED_01:1234567:7654321";
            // 
            // cbUpdaterTestOnly
            // 
            this.cbUpdaterTestOnly.AutoSize = true;
            this.cbUpdaterTestOnly.Location = new System.Drawing.Point(13, 42);
            this.cbUpdaterTestOnly.Name = "cbUpdaterTestOnly";
            this.cbUpdaterTestOnly.Size = new System.Drawing.Size(71, 17);
            this.cbUpdaterTestOnly.TabIndex = 31;
            this.cbUpdaterTestOnly.Text = "Test Only";
            this.cbUpdaterTestOnly.UseVisualStyleBackColor = true;
            // 
            // labUpdaterProgress
            // 
            this.labUpdaterProgress.AutoSize = true;
            this.labUpdaterProgress.Location = new System.Drawing.Point(17, 308);
            this.labUpdaterProgress.Name = "labUpdaterProgress";
            this.labUpdaterProgress.Size = new System.Drawing.Size(16, 13);
            this.labUpdaterProgress.TabIndex = 30;
            this.labUpdaterProgress.Text = "...";
            // 
            // butUpdaterCancel
            // 
            this.butUpdaterCancel.Enabled = false;
            this.butUpdaterCancel.Location = new System.Drawing.Point(399, 84);
            this.butUpdaterCancel.Name = "butUpdaterCancel";
            this.butUpdaterCancel.Size = new System.Drawing.Size(75, 23);
            this.butUpdaterCancel.TabIndex = 29;
            this.butUpdaterCancel.Text = "Cancel";
            this.butUpdaterCancel.UseVisualStyleBackColor = true;
            this.butUpdaterCancel.Click += new System.EventHandler(this.butUpdaterCancel_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.butStepStart);
            this.GroupBox1.Controls.Add(this.butStepGo);
            this.GroupBox1.Controls.Add(this.butStepNext);
            this.GroupBox1.Location = new System.Drawing.Point(92, 65);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(291, 53);
            this.GroupBox1.TabIndex = 28;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Step by Step";
            // 
            // butStepStart
            // 
            this.butStepStart.Location = new System.Drawing.Point(6, 19);
            this.butStepStart.Name = "butStepStart";
            this.butStepStart.Size = new System.Drawing.Size(93, 23);
            this.butStepStart.TabIndex = 24;
            this.butStepStart.Text = "Step by Step";
            this.butStepStart.UseVisualStyleBackColor = true;
            this.butStepStart.Click += new System.EventHandler(this.butUpdater_Click);
            // 
            // butStepGo
            // 
            this.butStepGo.Enabled = false;
            this.butStepGo.Location = new System.Drawing.Point(208, 19);
            this.butStepGo.Name = "butStepGo";
            this.butStepGo.Size = new System.Drawing.Size(75, 23);
            this.butStepGo.TabIndex = 27;
            this.butStepGo.Text = "Go";
            this.butStepGo.UseVisualStyleBackColor = true;
            this.butStepGo.Click += new System.EventHandler(this.butStepGo_Click);
            // 
            // butStepNext
            // 
            this.butStepNext.Enabled = false;
            this.butStepNext.Location = new System.Drawing.Point(117, 19);
            this.butStepNext.Name = "butStepNext";
            this.butStepNext.Size = new System.Drawing.Size(75, 23);
            this.butStepNext.TabIndex = 26;
            this.butStepNext.Text = "Next Step";
            this.butStepNext.UseVisualStyleBackColor = true;
            this.butStepNext.Click += new System.EventHandler(this.butStepNext_Click);
            // 
            // butUpdater
            // 
            this.butUpdater.Location = new System.Drawing.Point(12, 84);
            this.butUpdater.Name = "butUpdater";
            this.butUpdater.Size = new System.Drawing.Size(74, 23);
            this.butUpdater.TabIndex = 25;
            this.butUpdater.Text = "Process all";
            this.butUpdater.UseVisualStyleBackColor = true;
            this.butUpdater.Click += new System.EventHandler(this.butUpdater_Click);
            // 
            // butSearchFile
            // 
            this.butSearchFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.butSearchFile.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("butSearchFile.BackgroundImage")));
            this.butSearchFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.butSearchFile.Location = new System.Drawing.Point(458, 8);
            this.butSearchFile.Name = "butSearchFile";
            this.butSearchFile.Size = new System.Drawing.Size(23, 23);
            this.butSearchFile.TabIndex = 23;
            this.butSearchFile.UseVisualStyleBackColor = true;
            this.butSearchFile.Click += new System.EventHandler(this.butSearchFile_Click);
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Location = new System.Drawing.Point(5, 13);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(65, 13);
            this.Label13.TabIndex = 22;
            this.Label13.Text = "Program File";
            // 
            // tbProgramFile
            // 
            this.tbProgramFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProgramFile.Location = new System.Drawing.Point(83, 10);
            this.tbProgramFile.Name = "tbProgramFile";
            this.tbProgramFile.Size = new System.Drawing.Size(369, 20);
            this.tbProgramFile.TabIndex = 0;
            // 
            // tpFiles
            // 
            this.tpFiles.Controls.Add(this.butFiles_SelectedFile);
            this.tpFiles.Controls.Add(this.butFiles_SelectFile);
            this.tpFiles.Controls.Add(this.butFiles_DeleteFile);
            this.tpFiles.Controls.Add(this.butFiles_SendFile);
            this.tpFiles.Controls.Add(this.butFiles_GetFile);
            this.tpFiles.Controls.Add(this.tbFiles_Extension);
            this.tpFiles.Controls.Add(this.lbFiles_List);
            this.tpFiles.Controls.Add(this.butFiles_ListFiles);
            this.tpFiles.Location = new System.Drawing.Point(4, 22);
            this.tpFiles.Name = "tpFiles";
            this.tpFiles.Padding = new System.Windows.Forms.Padding(3);
            this.tpFiles.Size = new System.Drawing.Size(491, 480);
            this.tpFiles.TabIndex = 2;
            this.tpFiles.Text = "Files";
            this.tpFiles.UseVisualStyleBackColor = true;
            // 
            // butFiles_SelectedFile
            // 
            this.butFiles_SelectedFile.Location = new System.Drawing.Point(142, 165);
            this.butFiles_SelectedFile.Name = "butFiles_SelectedFile";
            this.butFiles_SelectedFile.Size = new System.Drawing.Size(124, 23);
            this.butFiles_SelectedFile.TabIndex = 7;
            this.butFiles_SelectedFile.Text = "Current Selected File";
            this.butFiles_SelectedFile.UseVisualStyleBackColor = true;
            this.butFiles_SelectedFile.Click += new System.EventHandler(this.butFiles_SelectedFile_Click);
            // 
            // butFiles_SelectFile
            // 
            this.butFiles_SelectFile.Location = new System.Drawing.Point(142, 194);
            this.butFiles_SelectFile.Name = "butFiles_SelectFile";
            this.butFiles_SelectFile.Size = new System.Drawing.Size(124, 23);
            this.butFiles_SelectFile.TabIndex = 6;
            this.butFiles_SelectFile.Text = "Select File";
            this.butFiles_SelectFile.UseVisualStyleBackColor = true;
            this.butFiles_SelectFile.Click += new System.EventHandler(this.butFiles_SelectFile_Click);
            // 
            // butFiles_DeleteFile
            // 
            this.butFiles_DeleteFile.Location = new System.Drawing.Point(142, 114);
            this.butFiles_DeleteFile.Name = "butFiles_DeleteFile";
            this.butFiles_DeleteFile.Size = new System.Drawing.Size(124, 23);
            this.butFiles_DeleteFile.TabIndex = 5;
            this.butFiles_DeleteFile.Text = "Delete Selected File";
            this.butFiles_DeleteFile.UseVisualStyleBackColor = true;
            this.butFiles_DeleteFile.Click += new System.EventHandler(this.butFiles_DeleteFile_Click);
            // 
            // butFiles_SendFile
            // 
            this.butFiles_SendFile.Location = new System.Drawing.Point(142, 85);
            this.butFiles_SendFile.Name = "butFiles_SendFile";
            this.butFiles_SendFile.Size = new System.Drawing.Size(124, 23);
            this.butFiles_SendFile.TabIndex = 4;
            this.butFiles_SendFile.Text = "Add File";
            this.butFiles_SendFile.UseVisualStyleBackColor = true;
            this.butFiles_SendFile.Click += new System.EventHandler(this.butFiles_SendFile_Click);
            // 
            // butFiles_GetFile
            // 
            this.butFiles_GetFile.Location = new System.Drawing.Point(142, 56);
            this.butFiles_GetFile.Name = "butFiles_GetFile";
            this.butFiles_GetFile.Size = new System.Drawing.Size(124, 23);
            this.butFiles_GetFile.TabIndex = 3;
            this.butFiles_GetFile.Text = "Get File";
            this.butFiles_GetFile.UseVisualStyleBackColor = true;
            this.butFiles_GetFile.Click += new System.EventHandler(this.butFiles_GetFile_Click);
            // 
            // tbFiles_Extension
            // 
            this.tbFiles_Extension.Location = new System.Drawing.Point(86, 29);
            this.tbFiles_Extension.Name = "tbFiles_Extension";
            this.tbFiles_Extension.Size = new System.Drawing.Size(50, 20);
            this.tbFiles_Extension.TabIndex = 2;
            this.tbFiles_Extension.Text = "JPF";
            // 
            // lbFiles_List
            // 
            this.lbFiles_List.FormattingEnabled = true;
            this.lbFiles_List.Location = new System.Drawing.Point(16, 56);
            this.lbFiles_List.Name = "lbFiles_List";
            this.lbFiles_List.Size = new System.Drawing.Size(120, 264);
            this.lbFiles_List.TabIndex = 1;
            // 
            // butFiles_ListFiles
            // 
            this.butFiles_ListFiles.Location = new System.Drawing.Point(16, 27);
            this.butFiles_ListFiles.Name = "butFiles_ListFiles";
            this.butFiles_ListFiles.Size = new System.Drawing.Size(64, 23);
            this.butFiles_ListFiles.TabIndex = 0;
            this.butFiles_ListFiles.Text = "List Files";
            this.butFiles_ListFiles.UseVisualStyleBackColor = true;
            this.butFiles_ListFiles.Click += new System.EventHandler(this.butFiles_ListFiles_Click);
            // 
            // tpBCC
            // 
            this.tpBCC.Controls.Add(this.Label20);
            this.tpBCC.Controls.Add(this.nupTargetDevice);
            this.tpBCC.Controls.Add(this.Label19);
            this.tpBCC.Controls.Add(this.butCheckFrame);
            this.tpBCC.Controls.Add(this.tbHexaBytes);
            this.tpBCC.Location = new System.Drawing.Point(4, 22);
            this.tpBCC.Name = "tpBCC";
            this.tpBCC.Padding = new System.Windows.Forms.Padding(3);
            this.tpBCC.Size = new System.Drawing.Size(491, 480);
            this.tpBCC.TabIndex = 4;
            this.tpBCC.Text = "BCC";
            this.tpBCC.UseVisualStyleBackColor = true;
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Location = new System.Drawing.Point(6, 49);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(165, 13);
            this.Label20.TabIndex = 4;
            this.Label20.Text = "Target Device to check (decimal)";
            // 
            // nupTargetDevice
            // 
            this.nupTargetDevice.Location = new System.Drawing.Point(194, 47);
            this.nupTargetDevice.Name = "nupTargetDevice";
            this.nupTargetDevice.Size = new System.Drawing.Size(49, 20);
            this.nupTargetDevice.TabIndex = 3;
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.Location = new System.Drawing.Point(6, 21);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(61, 13);
            this.Label19.TabIndex = 2;
            this.Label19.Text = "Hexa Bytes";
            // 
            // butCheckFrame
            // 
            this.butCheckFrame.Location = new System.Drawing.Point(9, 81);
            this.butCheckFrame.Name = "butCheckFrame";
            this.butCheckFrame.Size = new System.Drawing.Size(108, 23);
            this.butCheckFrame.TabIndex = 1;
            this.butCheckFrame.Text = "Check Frame";
            this.butCheckFrame.UseVisualStyleBackColor = true;
            this.butCheckFrame.Click += new System.EventHandler(this.butCalcBCC_Click);
            // 
            // tbHexaBytes
            // 
            this.tbHexaBytes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHexaBytes.Location = new System.Drawing.Point(89, 18);
            this.tbHexaBytes.Name = "tbHexaBytes";
            this.tbHexaBytes.Size = new System.Drawing.Size(384, 20);
            this.tbHexaBytes.TabIndex = 0;
            // 
            // tpUID
            // 
            this.tpUID.Controls.Add(this.GroupBox2);
            this.tpUID.Controls.Add(this.butStationToMemory);
            this.tpUID.Controls.Add(this.butMemoryToStation);
            this.tpUID.Controls.Add(this.tbConvertedUID);
            this.tpUID.Controls.Add(this.tbStationUID);
            this.tpUID.Controls.Add(this.Label18);
            this.tpUID.Controls.Add(this.tbMemoryUID);
            this.tpUID.Controls.Add(this.butNewUID);
            this.tpUID.Controls.Add(this.Label17);
            this.tpUID.Location = new System.Drawing.Point(4, 22);
            this.tpUID.Name = "tpUID";
            this.tpUID.Padding = new System.Windows.Forms.Padding(3);
            this.tpUID.Size = new System.Drawing.Size(491, 480);
            this.tpUID.TabIndex = 3;
            this.tpUID.Text = "UID - Hex";
            this.tpUID.UseVisualStyleBackColor = true;
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.rdGUIDType);
            this.GroupBox2.Controls.Add(this.rdMACType);
            this.GroupBox2.Location = new System.Drawing.Point(155, 10);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(170, 49);
            this.GroupBox2.TabIndex = 8;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "UID Type";
            // 
            // rdGUIDType
            // 
            this.rdGUIDType.AutoSize = true;
            this.rdGUIDType.Location = new System.Drawing.Point(95, 19);
            this.rdGUIDType.Name = "rdGUIDType";
            this.rdGUIDType.Size = new System.Drawing.Size(52, 17);
            this.rdGUIDType.TabIndex = 1;
            this.rdGUIDType.Text = "GUID";
            this.rdGUIDType.UseVisualStyleBackColor = true;
            // 
            // rdMACType
            // 
            this.rdMACType.AutoSize = true;
            this.rdMACType.Checked = true;
            this.rdMACType.Location = new System.Drawing.Point(15, 19);
            this.rdMACType.Name = "rdMACType";
            this.rdMACType.Size = new System.Drawing.Size(48, 17);
            this.rdMACType.TabIndex = 0;
            this.rdMACType.TabStop = true;
            this.rdMACType.Text = "MAC";
            this.rdMACType.UseVisualStyleBackColor = true;
            // 
            // butStationToMemory
            // 
            this.butStationToMemory.Location = new System.Drawing.Point(246, 253);
            this.butStationToMemory.Name = "butStationToMemory";
            this.butStationToMemory.Size = new System.Drawing.Size(173, 23);
            this.butStationToMemory.TabIndex = 7;
            this.butStationToMemory.Text = "Convert Station To Memory";
            this.butStationToMemory.UseVisualStyleBackColor = true;
            this.butStationToMemory.Click += new System.EventHandler(this.butStationToMemory_Click);
            // 
            // butMemoryToStation
            // 
            this.butMemoryToStation.Location = new System.Drawing.Point(53, 253);
            this.butMemoryToStation.Name = "butMemoryToStation";
            this.butMemoryToStation.Size = new System.Drawing.Size(173, 23);
            this.butMemoryToStation.TabIndex = 6;
            this.butMemoryToStation.Text = "Convert Memory to Station";
            this.butMemoryToStation.UseVisualStyleBackColor = true;
            this.butMemoryToStation.Click += new System.EventHandler(this.butMemoryToStation_Click);
            // 
            // tbConvertedUID
            // 
            this.tbConvertedUID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConvertedUID.Location = new System.Drawing.Point(53, 282);
            this.tbConvertedUID.Multiline = true;
            this.tbConvertedUID.Name = "tbConvertedUID";
            this.tbConvertedUID.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbConvertedUID.Size = new System.Drawing.Size(383, 107);
            this.tbConvertedUID.TabIndex = 5;
            // 
            // tbStationUID
            // 
            this.tbStationUID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStationUID.Location = new System.Drawing.Point(145, 152);
            this.tbStationUID.Multiline = true;
            this.tbStationUID.Name = "tbStationUID";
            this.tbStationUID.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbStationUID.Size = new System.Drawing.Size(325, 84);
            this.tbStationUID.TabIndex = 4;
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Location = new System.Drawing.Point(15, 155);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(124, 13);
            this.Label18.TabIndex = 3;
            this.Label18.Text = "Station UID (Hexa bytes)";
            // 
            // tbMemoryUID
            // 
            this.tbMemoryUID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbMemoryUID.Location = new System.Drawing.Point(145, 65);
            this.tbMemoryUID.Multiline = true;
            this.tbMemoryUID.Name = "tbMemoryUID";
            this.tbMemoryUID.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbMemoryUID.Size = new System.Drawing.Size(325, 81);
            this.tbMemoryUID.TabIndex = 2;
            // 
            // butNewUID
            // 
            this.butNewUID.Location = new System.Drawing.Point(18, 23);
            this.butNewUID.Name = "butNewUID";
            this.butNewUID.Size = new System.Drawing.Size(123, 23);
            this.butNewUID.TabIndex = 1;
            this.butNewUID.Text = "Generate New UID";
            this.butNewUID.UseVisualStyleBackColor = true;
            this.butNewUID.Click += new System.EventHandler(this.butNewUID_Click);
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Location = new System.Drawing.Point(15, 68);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(66, 13);
            this.Label17.TabIndex = 0;
            this.Label17.Text = "Memory UID";
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLog.Location = new System.Drawing.Point(7, 61);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbLog.Size = new System.Drawing.Size(599, 735);
            this.tbLog.TabIndex = 0;
            this.tbLog.WordWrap = false;
            // 
            // gbUSB
            // 
            this.gbUSB.Controls.Add(this.cbSerialPorts);
            this.gbUSB.Controls.Add(this.butConnectUSB);
            this.gbUSB.Controls.Add(this.Label5);
            this.gbUSB.Location = new System.Drawing.Point(15, 5);
            this.gbUSB.Name = "gbUSB";
            this.gbUSB.Size = new System.Drawing.Size(280, 50);
            this.gbUSB.TabIndex = 26;
            this.gbUSB.TabStop = false;
            this.gbUSB.Text = "USB";
            // 
            // cbSerialPorts
            // 
            this.cbSerialPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSerialPorts.FormattingEnabled = true;
            this.cbSerialPorts.Location = new System.Drawing.Point(144, 17);
            this.cbSerialPorts.Margin = new System.Windows.Forms.Padding(4);
            this.cbSerialPorts.Name = "cbSerialPorts";
            this.cbSerialPorts.Size = new System.Drawing.Size(124, 21);
            this.cbSerialPorts.TabIndex = 1;
            this.cbSerialPorts.DropDown += new System.EventHandler(this.cbSerialPorts_DropDown);
            // 
            // butConnectUSB
            // 
            this.butConnectUSB.Location = new System.Drawing.Point(7, 15);
            this.butConnectUSB.Name = "butConnectUSB";
            this.butConnectUSB.Size = new System.Drawing.Size(88, 24);
            this.butConnectUSB.TabIndex = 0;
            this.butConnectUSB.Text = "Connect";
            this.butConnectUSB.UseVisualStyleBackColor = true;
            this.butConnectUSB.Click += new System.EventHandler(this.butConnectUSB_Click);
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(108, 20);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(29, 13);
            this.Label5.TabIndex = 20;
            this.Label5.Text = "Port:";
            // 
            // gbEthernet
            // 
            this.gbEthernet.Controls.Add(this.butConnectEth);
            this.gbEthernet.Controls.Add(this.cbEndPoints);
            this.gbEthernet.Controls.Add(this.Label11);
            this.gbEthernet.Location = new System.Drawing.Point(301, 5);
            this.gbEthernet.Name = "gbEthernet";
            this.gbEthernet.Size = new System.Drawing.Size(337, 50);
            this.gbEthernet.TabIndex = 27;
            this.gbEthernet.TabStop = false;
            this.gbEthernet.Text = "Ethernet";
            // 
            // butConnectEth
            // 
            this.butConnectEth.Location = new System.Drawing.Point(6, 16);
            this.butConnectEth.Name = "butConnectEth";
            this.butConnectEth.Size = new System.Drawing.Size(88, 23);
            this.butConnectEth.TabIndex = 23;
            this.butConnectEth.Text = "Connect";
            this.butConnectEth.UseVisualStyleBackColor = true;
            this.butConnectEth.Click += new System.EventHandler(this.butConnectEth_Click);
            // 
            // cbEndPoints
            // 
            this.cbEndPoints.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEndPoints.FormattingEnabled = true;
            this.cbEndPoints.Location = new System.Drawing.Point(177, 18);
            this.cbEndPoints.Margin = new System.Windows.Forms.Padding(4);
            this.cbEndPoints.Name = "cbEndPoints";
            this.cbEndPoints.Size = new System.Drawing.Size(153, 21);
            this.cbEndPoints.TabIndex = 24;
            this.cbEndPoints.DropDownClosed += new System.EventHandler(this.cbEndPoints_DropDown);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Location = new System.Drawing.Point(112, 21);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(48, 13);
            this.Label11.TabIndex = 25;
            this.Label11.Text = "Address:";
            // 
            // cbViewSYNReceivingData
            // 
            this.cbViewSYNReceivingData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbViewSYNReceivingData.AutoSize = true;
            this.cbViewSYNReceivingData.Location = new System.Drawing.Point(926, 812);
            this.cbViewSYNReceivingData.Name = "cbViewSYNReceivingData";
            this.cbViewSYNReceivingData.Size = new System.Drawing.Size(74, 17);
            this.cbViewSYNReceivingData.TabIndex = 9;
            this.cbViewSYNReceivingData.Text = "View SYN";
            this.cbViewSYNReceivingData.UseVisualStyleBackColor = true;
            // 
            // cbUseOrders
            // 
            this.cbUseOrders.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbUseOrders.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbUseOrders.FormattingEnabled = true;
            this.cbUseOrders.Items.AddRange(new object[] {
            "Automatic",
            "Protocol 01",
            "Protocol 02"});
            this.cbUseOrders.Location = new System.Drawing.Point(921, 839);
            this.cbUseOrders.Name = "cbUseOrders";
            this.cbUseOrders.Size = new System.Drawing.Size(160, 21);
            this.cbUseOrders.TabIndex = 21;
            // 
            // Label10
            // 
            this.Label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Label10.AutoSize = true;
            this.Label10.Location = new System.Drawing.Point(857, 842);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(58, 13);
            this.Label10.TabIndex = 22;
            this.Label10.Text = "Use orders";
            // 
            // butClipboard
            // 
            this.butClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.butClipboard.Location = new System.Drawing.Point(1088, 839);
            this.butClipboard.Name = "butClipboard";
            this.butClipboard.Size = new System.Drawing.Size(75, 23);
            this.butClipboard.TabIndex = 28;
            this.butClipboard.Text = "To Clipboard";
            this.butClipboard.UseVisualStyleBackColor = true;
            this.butClipboard.Click += new System.EventHandler(this.butClipboard_Click);
            // 
            // OpenFileDialog1
            // 
            this.OpenFileDialog1.FileName = "OpenFileDialog1";
            this.OpenFileDialog1.Filter = "S19|*.s19|Hex|*.hex|All|*.*";
            // 
            // cbLogWrap
            // 
            this.cbLogWrap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbLogWrap.AutoSize = true;
            this.cbLogWrap.Location = new System.Drawing.Point(1006, 813);
            this.cbLogWrap.Name = "cbLogWrap";
            this.cbLogWrap.Size = new System.Drawing.Size(76, 17);
            this.cbLogWrap.TabIndex = 30;
            this.cbLogWrap.Text = "Wrap Text";
            this.cbLogWrap.UseVisualStyleBackColor = true;
            this.cbLogWrap.CheckedChanged += new System.EventHandler(this.cbLogWrap_CheckedChanged);
            // 
            // timerProgDelay
            // 
            this.timerProgDelay.Interval = 1000;
            this.timerProgDelay.Tick += new System.EventHandler(this.timerProgDelay_Tick);
            // 
            // cbViewRAW
            // 
            this.cbViewRAW.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbViewRAW.AutoSize = true;
            this.cbViewRAW.Location = new System.Drawing.Point(841, 812);
            this.cbViewRAW.Name = "cbViewRAW";
            this.cbViewRAW.Size = new System.Drawing.Size(74, 17);
            this.cbViewRAW.TabIndex = 31;
            this.cbViewRAW.Text = "View Raw";
            this.cbViewRAW.UseVisualStyleBackColor = true;
            // 
            // groupPortConf
            // 
            this.groupPortConf.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupPortConf.Controls.Add(this.butSerialPortConfigDefaults);
            this.groupPortConf.Controls.Add(this.Label21);
            this.groupPortConf.Controls.Add(this.cbPort_StopBits);
            this.groupPortConf.Controls.Add(this.Label22);
            this.groupPortConf.Controls.Add(this.cbPort_Parity);
            this.groupPortConf.Controls.Add(this.Label23);
            this.groupPortConf.Controls.Add(this.nudPort_DataBits);
            this.groupPortConf.Controls.Add(this.Label24);
            this.groupPortConf.Controls.Add(this.cbPort_Speed);
            this.groupPortConf.Location = new System.Drawing.Point(9, 813);
            this.groupPortConf.Name = "groupPortConf";
            this.groupPortConf.Size = new System.Drawing.Size(672, 47);
            this.groupPortConf.TabIndex = 36;
            this.groupPortConf.TabStop = false;
            this.groupPortConf.Text = "Serial Port";
            // 
            // butSerialPortConfigDefaults
            // 
            this.butSerialPortConfigDefaults.Location = new System.Drawing.Point(568, 13);
            this.butSerialPortConfigDefaults.Name = "butSerialPortConfigDefaults";
            this.butSerialPortConfigDefaults.Size = new System.Drawing.Size(93, 23);
            this.butSerialPortConfigDefaults.TabIndex = 42;
            this.butSerialPortConfigDefaults.Text = "Set Defaults";
            this.butSerialPortConfigDefaults.UseVisualStyleBackColor = true;
            this.butSerialPortConfigDefaults.Click += new System.EventHandler(this.butSerialPortConfigDefaults_Click);
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Location = new System.Drawing.Point(411, 18);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(49, 13);
            this.Label21.TabIndex = 41;
            this.Label21.Text = "Stop Bits";
            // 
            // cbPort_StopBits
            // 
            this.cbPort_StopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPort_StopBits.FormattingEnabled = true;
            this.cbPort_StopBits.Location = new System.Drawing.Point(466, 15);
            this.cbPort_StopBits.Name = "cbPort_StopBits";
            this.cbPort_StopBits.Size = new System.Drawing.Size(76, 21);
            this.cbPort_StopBits.TabIndex = 40;
            this.cbPort_StopBits.SelectedIndexChanged += new System.EventHandler(this.SerialPortConfigChanged);
            // 
            // Label22
            // 
            this.Label22.AutoSize = true;
            this.Label22.Location = new System.Drawing.Point(273, 18);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(33, 13);
            this.Label22.TabIndex = 39;
            this.Label22.Text = "Parity";
            // 
            // cbPort_Parity
            // 
            this.cbPort_Parity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPort_Parity.FormattingEnabled = true;
            this.cbPort_Parity.Location = new System.Drawing.Point(312, 15);
            this.cbPort_Parity.Name = "cbPort_Parity";
            this.cbPort_Parity.Size = new System.Drawing.Size(76, 21);
            this.cbPort_Parity.TabIndex = 38;
            this.cbPort_Parity.SelectedIndexChanged += new System.EventHandler(this.SerialPortConfigChanged);
            // 
            // Label23
            // 
            this.Label23.AutoSize = true;
            this.Label23.Location = new System.Drawing.Point(160, 19);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(50, 13);
            this.Label23.TabIndex = 37;
            this.Label23.Text = "Data Bits";
            // 
            // nudPort_DataBits
            // 
            this.nudPort_DataBits.Location = new System.Drawing.Point(216, 16);
            this.nudPort_DataBits.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudPort_DataBits.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudPort_DataBits.Name = "nudPort_DataBits";
            this.nudPort_DataBits.Size = new System.Drawing.Size(39, 20);
            this.nudPort_DataBits.TabIndex = 36;
            this.nudPort_DataBits.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.nudPort_DataBits.ValueChanged += new System.EventHandler(this.SerialPortConfigChanged);
            // 
            // Label24
            // 
            this.Label24.AutoSize = true;
            this.Label24.Location = new System.Drawing.Point(6, 19);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(58, 13);
            this.Label24.TabIndex = 35;
            this.Label24.Text = "Baud Rate";
            // 
            // cbPort_Speed
            // 
            this.cbPort_Speed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPort_Speed.FormattingEnabled = true;
            this.cbPort_Speed.Location = new System.Drawing.Point(70, 16);
            this.cbPort_Speed.Name = "cbPort_Speed";
            this.cbPort_Speed.Size = new System.Drawing.Size(76, 21);
            this.cbPort_Speed.TabIndex = 34;
            this.cbPort_Speed.SelectedIndexChanged += new System.EventHandler(this.SerialPortConfigChanged);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GroupBox3.Controls.Add(this.rbConnectUSB_StartPC);
            this.GroupBox3.Controls.Add(this.rbConnectUSB_StartStation);
            this.GroupBox3.Location = new System.Drawing.Point(687, 808);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(148, 56);
            this.GroupBox3.TabIndex = 28;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "USB Handshake";
            // 
            // rbConnectUSB_StartPC
            // 
            this.rbConnectUSB_StartPC.AutoSize = true;
            this.rbConnectUSB_StartPC.Location = new System.Drawing.Point(17, 36);
            this.rbConnectUSB_StartPC.Name = "rbConnectUSB_StartPC";
            this.rbConnectUSB_StartPC.Size = new System.Drawing.Size(69, 17);
            this.rbConnectUSB_StartPC.TabIndex = 23;
            this.rbConnectUSB_StartPC.Text = "Starts PC";
            this.rbConnectUSB_StartPC.UseVisualStyleBackColor = true;
            // 
            // rbConnectUSB_StartStation
            // 
            this.rbConnectUSB_StartStation.AutoSize = true;
            this.rbConnectUSB_StartStation.Checked = true;
            this.rbConnectUSB_StartStation.Location = new System.Drawing.Point(17, 18);
            this.rbConnectUSB_StartStation.Name = "rbConnectUSB_StartStation";
            this.rbConnectUSB_StartStation.Size = new System.Drawing.Size(126, 17);
            this.rbConnectUSB_StartStation.TabIndex = 22;
            this.rbConnectUSB_StartStation.TabStop = true;
            this.rbConnectUSB_StartStation.Text = "Normal (starts station)";
            this.rbConnectUSB_StartStation.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1181, 873);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.groupPortConf);
            this.Controls.Add(this.cbViewRAW);
            this.Controls.Add(this.cbLogWrap);
            this.Controls.Add(this.butClipboard);
            this.Controls.Add(this.cbViewSYNReceivingData);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.cbUseOrders);
            this.Controls.Add(this.SplitContainer1);
            this.Controls.Add(this.butClearLog);
            this.MinimumSize = new System.Drawing.Size(950, 700);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JBC Library Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.SplitContainer2.Panel1.ResumeLayout(false);
            this.SplitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer2)).EndInit();
            this.SplitContainer2.ResumeLayout(false);
            this.TabControl1.ResumeLayout(false);
            this.tpSendCommand.ResumeLayout(false);
            this.tpSendCommand.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSendTimes)).EndInit();
            this.tlpInput.ResumeLayout(false);
            this.tlpInput.PerformLayout();
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetDevice)).EndInit();
            this.tpUpdater.ResumeLayout(false);
            this.tpUpdater.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudProgDelay)).EndInit();
            this.GroupBox1.ResumeLayout(false);
            this.tpFiles.ResumeLayout(false);
            this.tpFiles.PerformLayout();
            this.tpBCC.ResumeLayout(false);
            this.tpBCC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupTargetDevice)).EndInit();
            this.tpUID.ResumeLayout(false);
            this.tpUID.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.gbUSB.ResumeLayout(false);
            this.gbUSB.PerformLayout();
            this.gbEthernet.ResumeLayout(false);
            this.gbEthernet.PerformLayout();
            this.groupPortConf.ResumeLayout(false);
            this.groupPortConf.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudPort_DataBits)).EndInit();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label type_0;
        internal System.Windows.Forms.Button butSendCommand;
        internal System.Windows.Forms.ComboBox cbLogPort;
        internal System.Windows.Forms.TextBox field_1;
        internal System.Windows.Forms.Button butClearLog;
        internal System.Windows.Forms.ComboBox cbLogTool;
        internal System.Windows.Forms.SplitContainer SplitContainer1;
        internal System.Windows.Forms.CheckBox cbViewSYNReceivingData;
        internal System.Windows.Forms.Button butConnectUSB;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.ComboBox cbSerialPorts;
        internal System.Windows.Forms.ComboBox cbTempType;
        internal System.Windows.Forms.TableLayoutPanel tlpInput;
        internal System.Windows.Forms.Label type_9;
        internal System.Windows.Forms.Label type_8;
        internal System.Windows.Forms.Label type_7;
        internal System.Windows.Forms.Label type_6;
        internal System.Windows.Forms.Label type_5;
        internal System.Windows.Forms.Label type_4;
        internal System.Windows.Forms.Label type_3;
        internal System.Windows.Forms.Label type_2;
        internal System.Windows.Forms.Label type_1;
        internal System.Windows.Forms.TextBox field_0;
        internal System.Windows.Forms.TextBox field_2;
        internal System.Windows.Forms.TextBox field_3;
        internal System.Windows.Forms.TextBox field_4;
        internal System.Windows.Forms.TextBox field_5;
        internal System.Windows.Forms.TextBox field_6;
        internal System.Windows.Forms.TextBox field_7;
        internal System.Windows.Forms.TextBox field_9;
        internal System.Windows.Forms.TextBox field_8;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.TextBox tbSendData;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox tbOpCode;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.TextBox tbReceiveData;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Label type_19;
        internal System.Windows.Forms.Label type_18;
        internal System.Windows.Forms.Label type_17;
        internal System.Windows.Forms.Label type_16;
        internal System.Windows.Forms.Label type_15;
        internal System.Windows.Forms.Label type_14;
        internal System.Windows.Forms.Label type_13;
        internal System.Windows.Forms.Label type_12;
        internal System.Windows.Forms.Label type_11;
        internal System.Windows.Forms.Label type_10;
        internal System.Windows.Forms.TextBox field_10;
        internal System.Windows.Forms.TextBox field_11;
        internal System.Windows.Forms.TextBox field_12;
        internal System.Windows.Forms.TextBox field_13;
        internal System.Windows.Forms.TextBox field_14;
        internal System.Windows.Forms.TextBox field_15;
        internal System.Windows.Forms.TextBox field_16;
        internal System.Windows.Forms.TextBox field_17;
        internal System.Windows.Forms.TextBox field_19;
        internal System.Windows.Forms.TextBox field_18;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.TextBox tbAction;
        internal System.Windows.Forms.SplitContainer SplitContainer2;
        internal System.Windows.Forms.ListBox lbLogOrders;
        internal System.Windows.Forms.ComboBox cbUseOrders;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.ComboBox cbEndPoints;
        internal System.Windows.Forms.Button butConnectEth;
        internal System.Windows.Forms.GroupBox gbUSB;
        internal System.Windows.Forms.GroupBox gbEthernet;
        internal System.Windows.Forms.NumericUpDown numSendTimes;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.NumericUpDown numTargetDevice;
        internal System.Windows.Forms.Button butClipboard;
        internal System.Windows.Forms.Label labCommandProtocol;
        internal System.Windows.Forms.Label labTargetDevice;
        internal System.Windows.Forms.TabControl TabControl1;
        internal System.Windows.Forms.TabPage tpSendCommand;
        internal System.Windows.Forms.TabPage tpUpdater;
        internal System.Windows.Forms.Button butSearchFile;
        internal System.Windows.Forms.Label Label13;
        internal System.Windows.Forms.TextBox tbProgramFile;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Button butStepStart;
        internal System.Windows.Forms.Button butStepGo;
        internal System.Windows.Forms.Button butStepNext;
        internal System.Windows.Forms.Button butUpdater;
        internal System.Windows.Forms.Button butUpdaterCancel;
        internal System.Windows.Forms.Label labUpdaterProgress;
        internal System.Windows.Forms.CheckBox cbUpdaterTestOnly;
        internal System.Windows.Forms.Label Label14;
        internal System.Windows.Forms.TextBox tbBorrarMemoriaFlash;
        internal System.Windows.Forms.TabPage tpFiles;
        internal System.Windows.Forms.Timer timerReadContMode;
        internal System.Windows.Forms.TextBox tbLog;
        internal System.Windows.Forms.CheckBox cbSendEndProgram;
        internal System.Windows.Forms.CheckBox cbLogWrap;
        internal System.Windows.Forms.NumericUpDown nudProgDelay;
        internal System.Windows.Forms.Timer timerProgDelay;
        internal System.Windows.Forms.CheckBox cbProgDelay;
        internal System.Windows.Forms.Label Label15;
        internal System.Windows.Forms.TextBox tbSendDataDefault;
        internal System.Windows.Forms.Label Label16;
        internal System.Windows.Forms.TextBox tbOpCodeExtended;
        internal System.Windows.Forms.Button butControlMode;
        internal System.Windows.Forms.TabPage tpUID;
        internal System.Windows.Forms.TextBox tbStationUID;
        internal System.Windows.Forms.Label Label18;
        internal System.Windows.Forms.TextBox tbMemoryUID;
        internal System.Windows.Forms.Button butNewUID;
        internal System.Windows.Forms.Label Label17;
        internal System.Windows.Forms.Button butStationToMemory;
        internal System.Windows.Forms.Button butMemoryToStation;
        internal System.Windows.Forms.TextBox tbConvertedUID;
        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.RadioButton rdGUIDType;
        internal System.Windows.Forms.RadioButton rdMACType;
        internal System.Windows.Forms.TabPage tpBCC;
        internal System.Windows.Forms.Label Label19;
        internal System.Windows.Forms.Button butCheckFrame;
        internal System.Windows.Forms.TextBox tbHexaBytes;
        internal System.Windows.Forms.Label Label20;
        internal System.Windows.Forms.NumericUpDown nupTargetDevice;
        internal System.Windows.Forms.CheckBox cbViewRAW;
        internal System.Windows.Forms.CheckBox cbSaveS19ToProcess;
        internal System.Windows.Forms.TextBox tbProcessedS19;
        internal System.Windows.Forms.GroupBox groupPortConf;
        internal System.Windows.Forms.Label Label21;
        internal System.Windows.Forms.ComboBox cbPort_StopBits;
        internal System.Windows.Forms.Label Label22;
        internal System.Windows.Forms.ComboBox cbPort_Parity;
        internal System.Windows.Forms.Label Label23;
        internal System.Windows.Forms.NumericUpDown nudPort_DataBits;
        internal System.Windows.Forms.Label Label24;
        internal System.Windows.Forms.ComboBox cbPort_Speed;
        internal System.Windows.Forms.Button butSerialPortConfigDefaults;
        internal System.Windows.Forms.Button butFiles_ListFiles;
        internal System.Windows.Forms.TextBox tbFiles_Extension;
        internal System.Windows.Forms.ListBox lbFiles_List;
        internal System.Windows.Forms.SaveFileDialog SaveFileDialog1;
        internal System.Windows.Forms.Button butFiles_GetFile;
        internal System.Windows.Forms.Button butFiles_SendFile;
        internal System.Windows.Forms.Button butFiles_DeleteFile;
        internal System.Windows.Forms.GroupBox GroupBox3;
        internal System.Windows.Forms.RadioButton rbConnectUSB_StartPC;
        internal System.Windows.Forms.RadioButton rbConnectUSB_StartStation;
        internal System.Windows.Forms.Button butFiles_SelectFile;
        internal System.Windows.Forms.Button butFiles_SelectedFile;
        internal System.Windows.Forms.Label type_25;
        internal System.Windows.Forms.TextBox field_20;
        internal System.Windows.Forms.TextBox field_21;
        internal System.Windows.Forms.TextBox field_22;
        internal System.Windows.Forms.TextBox field_23;
        internal System.Windows.Forms.TextBox field_24;
        internal System.Windows.Forms.Label type_20;
        internal System.Windows.Forms.Label type_21;
        internal System.Windows.Forms.Label type_22;
        internal System.Windows.Forms.Label type_23;
        internal System.Windows.Forms.Label type_24;
        internal System.Windows.Forms.TextBox field_25;
        private System.ComponentModel.IContainer components;
    }
}
