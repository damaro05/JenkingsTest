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
partial class TabPanels : System.Windows.Forms.UserControl
    {

        //UserControl reemplaza a Dispose para limpiar la lista de componentes.
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
            this.components = new System.ComponentModel.Container();
            Telerik.WinControls.UI.CartesianArea CartesianArea1 = new Telerik.WinControls.UI.CartesianArea();
            Telerik.WinControls.UI.CartesianArea CartesianArea2 = new Telerik.WinControls.UI.CartesianArea();
            this.pageWork = new System.Windows.Forms.Panel();
            this.lblToolTempAdjust = new System.Windows.Forms.Label();
            this.panelWorkPeriphStatus = new System.Windows.Forms.Panel();
            this.txtStatusPeriph_2 = new System.Windows.Forms.Label();
            this.txtStatusPeriph_1 = new System.Windows.Forms.Label();
            this.butPort = new System.Windows.Forms.Button();
            this.butPort.Click += new System.EventHandler(this.pcbTool_Click);
            this.lblToolTempUnits = new System.Windows.Forms.Label();
            this.pcbTool = new System.Windows.Forms.PictureBox();
            this.pcbTool.MouseEnter += new System.EventHandler(this.pcbTool_MouseEnter);
            this.pcbTool.MouseLeave += new System.EventHandler(this.pcbTool_MouseLeave);
            this.pcbTool.Click += new System.EventHandler(this.pcbTool_Click);
            this.lblToolTemp = new System.Windows.Forms.Label();
            this.panelPower = new System.Windows.Forms.Panel();
            this.lblPwr = new System.Windows.Forms.Label();
            this.pictTrackPowerColor = new System.Windows.Forms.PictureBox();
            this.lblTitlePwr = new System.Windows.Forms.Label();
            this.PanelTemps = new System.Windows.Forms.Panel();
            this.pageStationSettings = new System.Windows.Forms.Panel();
            this.panelStationSettings = new System.Windows.Forms.Panel();
            this.rbRobotConf = new System.Windows.Forms.RadioButton();
            this.rbRobotConf.CheckedChanged += new System.EventHandler(this.rbTabSettings_CheckedChanged);
            this.rbEthernetConf = new System.Windows.Forms.RadioButton();
            this.rbEthernetConf.CheckedChanged += new System.EventHandler(this.rbTabSettings_CheckedChanged);
            this.rbGeneralSettings = new System.Windows.Forms.RadioButton();
            this.rbGeneralSettings.CheckedChanged += new System.EventHandler(this.rbTabSettings_CheckedChanged);
            this.pageToolSettings = new System.Windows.Forms.Panel();
            this.lblSelectedTool = new System.Windows.Forms.Label();
            this.TableLayoutPanelTools = new System.Windows.Forms.TableLayoutPanel();
            this.rbToolSettings_T5 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T5.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T5.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T5.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.rbToolSettings_T6 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T6.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T6.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T6.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.rbToolSettings_T4 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T4.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T4.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T4.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.rbToolSettings_T1 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T1.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T1.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T1.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.rbToolSettings_T3 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T3.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T3.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T3.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.rbToolSettings_T2 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T2.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T2.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T2.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.rbToolSettings_T7 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T7.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T7.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T7.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.rbToolSettings_T8 = new System.Windows.Forms.RadioButton();
            this.rbToolSettings_T8.MouseEnter += new System.EventHandler(this.rbToolSettings_MouseEnter);
            this.rbToolSettings_T8.MouseLeave += new System.EventHandler(this.rbToolSettings_MouseLeave);
            this.rbToolSettings_T8.CheckedChanged += new System.EventHandler(this.rbToolSettings_CheckedChanged);
            this.TableLayoutPanelPortsTools = new System.Windows.Forms.TableLayoutPanel();
            this.rbPortTools_1 = new System.Windows.Forms.RadioButton();
            this.rbPortTools_1.MouseEnter += new System.EventHandler(this.rbPort_MouseEnter);
            this.rbPortTools_1.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortTools_1.CheckedChanged += new System.EventHandler(this.rbPortTools_CheckedChanged);
            this.rbPortTools_2 = new System.Windows.Forms.RadioButton();
            this.rbPortTools_2.MouseEnter += new System.EventHandler(this.rbPort_MouseEnter);
            this.rbPortTools_2.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortTools_2.CheckedChanged += new System.EventHandler(this.rbPortTools_CheckedChanged);
            this.rbPortTools_3 = new System.Windows.Forms.RadioButton();
            this.rbPortTools_3.MouseEnter += new System.EventHandler(this.rbPort_MouseEnter);
            this.rbPortTools_3.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortTools_3.CheckedChanged += new System.EventHandler(this.rbPortTools_CheckedChanged);
            this.rbPortTools_4 = new System.Windows.Forms.RadioButton();
            this.rbPortTools_4.MouseEnter += new System.EventHandler(this.rbPort_MouseEnter);
            this.rbPortTools_4.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortTools_4.CheckedChanged += new System.EventHandler(this.rbPortTools_CheckedChanged);
            this.pageLoadSaveSettings = new System.Windows.Forms.Panel();
            this.butConfSave = new System.Windows.Forms.Button();
            this.butConfSave.Click += new System.EventHandler(this.butConf_Click);
            this.butConfLoad = new System.Windows.Forms.Button();
            this.butConfLoad.Click += new System.EventHandler(this.butConf_Click);
            this.lblConfInfo = new System.Windows.Forms.Label();
            this.pageResetSettings = new System.Windows.Forms.Panel();
            this.pgbResetBar = new System.Windows.Forms.ProgressBar();
            this.butResetProceed = new System.Windows.Forms.Button();
            this.butResetProceed.Click += new System.EventHandler(this.butResetProceed_Click);
            this.lblResetInfo = new System.Windows.Forms.Label();
            this.pageCounters = new System.Windows.Forms.Panel();
            this.butResetPartialCounters = new System.Windows.Forms.Button();
            this.butResetPartialCounters.Click += new System.EventHandler(this.butResetPartialCounters_Click);
            this.rbPartialCounters = new System.Windows.Forms.RadioButton();
            this.rbPartialCounters.CheckedChanged += new System.EventHandler(this.rbCountersType_CheckedChanged);
            this.rbGlobalCounters = new System.Windows.Forms.RadioButton();
            this.rbGlobalCounters.CheckedChanged += new System.EventHandler(this.rbCountersType_CheckedChanged);
            this.TableLayoutPanelPortsCounters = new System.Windows.Forms.TableLayoutPanel();
            this.rbPortCounters_1 = new System.Windows.Forms.RadioButton();
            this.rbPortCounters_1.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortCounters_1.CheckedChanged += new System.EventHandler(this.rbPortCounters_CheckedChanged);
            this.rbPortCounters_2 = new System.Windows.Forms.RadioButton();
            this.rbPortCounters_2.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortCounters_2.CheckedChanged += new System.EventHandler(this.rbPortCounters_CheckedChanged);
            this.rbPortCounters_3 = new System.Windows.Forms.RadioButton();
            this.rbPortCounters_3.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortCounters_3.CheckedChanged += new System.EventHandler(this.rbPortCounters_CheckedChanged);
            this.rbPortCounters_4 = new System.Windows.Forms.RadioButton();
            this.rbPortCounters_4.MouseLeave += new System.EventHandler(this.rbPort_MouseLeave);
            this.rbPortCounters_4.CheckedChanged += new System.EventHandler(this.rbPortCounters_CheckedChanged);
            this.pageInfo = new System.Windows.Forms.Panel();
            this.pageDummy = new System.Windows.Forms.Panel();
            this.lblNoTool = new System.Windows.Forms.Label();
            this.lblToolNeeded = new System.Windows.Forms.Label();
            this.pageGraphics = new System.Windows.Forms.Panel();
            this.lblGraphInfo = new System.Windows.Forms.Label();
            this.butGraphAddSeries = new System.Windows.Forms.Button();
            this.butGraphAddSeries.Click += new System.EventHandler(this.butGraphAddSeries_Click);
            this.cbxGraphPlots = new System.Windows.Forms.ComboBox();
            this.cbxGraphPlots.DropDown += new System.EventHandler(this.cbxGraphPlots_DropDown);
            this.lblGraphAddToPlot = new System.Windows.Forms.Label();
            this.pageDummy2 = new System.Windows.Forms.Panel();
            this.panelWorkSleep = new System.Windows.Forms.Panel();
            this.labWorkSleepStatus = new System.Windows.Forms.Label();
            this.labWorkSleepDelay = new System.Windows.Forms.Label();
            this.labWorkSleepStand = new System.Windows.Forms.Label();
            this.labWorkSleepTemp = new System.Windows.Forms.Label();
            this.pagePeripheral = new System.Windows.Forms.Panel();
            this.lblNoPeripheralSupported = new System.Windows.Forms.Label();
            this.panelConfigPeripheral = new System.Windows.Forms.Panel();
            this.textTypePeripheral = new System.Windows.Forms.Label();
            this.panelConfigParamPeripheral = new System.Windows.Forms.Panel();
            this.inputErrorTimePeripheral = new System.Windows.Forms.Label();
            this.inputTimePeripheral = new System.Windows.Forms.TextBox();
            this.textFunctionPeripheral = new System.Windows.Forms.Label();
            this.inputFunctionPeripheral = new System.Windows.Forms.ComboBox();
            this.textTimePeripheral = new System.Windows.Forms.Label();
            this.inputActivationPeripheral = new System.Windows.Forms.ComboBox();
            this.textActivationPeripheral = new System.Windows.Forms.Label();
            this.lineSeparator = new System.Windows.Forms.Label();
            this.textNamePeripheral = new System.Windows.Forms.Label();
            this.tlpPeripherals = new System.Windows.Forms.TableLayoutPanel();
            this.labelPeripheralPort3 = new System.Windows.Forms.Label();
            this.labelPeripheralPort1 = new System.Windows.Forms.Label();
            this.labelPeripheralPort2 = new System.Windows.Forms.Label();
            this.labelPeripheralPort4 = new System.Windows.Forms.Label();
            this.lblNoPeripheral = new System.Windows.Forms.Label();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.pageWork_HA = new System.Windows.Forms.Panel();
            this.lblToolFlowUnits_HA = new System.Windows.Forms.Label();
            this.lblToolFlow_HA = new System.Windows.Forms.Label();
            this.labWorkPortStatus_HA = new System.Windows.Forms.Label();
            this.lblToolTempUnits_HA = new System.Windows.Forms.Label();
            this.PanelTemps_HA = new System.Windows.Forms.Panel();
            this.lblToolTemp_HA = new System.Windows.Forms.Label();
            this.panelPower_HA = new System.Windows.Forms.Panel();
            this.lblPwr_HA = new System.Windows.Forms.Label();
            this.pictTrackPowerColor_HA = new System.Windows.Forms.PictureBox();
            this.lblTitlePwr_HA = new System.Windows.Forms.Label();
            this.pageDummy3 = new System.Windows.Forms.Panel();
            this.panelWorkPageProfilePlot = new Telerik.WinControls.UI.RadChartView();
            this.labelWorkProfile_HA_Status = new System.Windows.Forms.Label();
            this.labelWorkProfile_HA_AirFlow = new System.Windows.Forms.Label();
            this.labelWorkProfile_HA_AirFlowTitle = new System.Windows.Forms.Label();
            this.labelWorkProfile_HA_ExtTCTemp = new System.Windows.Forms.Label();
            this.labelWorkProfile_HA_ExtTCTempTitle = new System.Windows.Forms.Label();
            this.labelWorkProfile_HA_HotAirTemp = new System.Windows.Forms.Label();
            this.labelWorkProfile_HA_HotAirTempTitle = new System.Windows.Forms.Label();
            this.pageProfiles = new System.Windows.Forms.Panel();
            this.panelProfilesParameters = new System.Windows.Forms.Panel();
            this.panelProfilesOptions = new System.Windows.Forms.Panel();
            this.lineSeparator2 = new System.Windows.Forms.Label();
            this.panelProfilesPlot = new Telerik.WinControls.UI.RadChartView();
            this.pageLoadSaveSettings_HA = new System.Windows.Forms.Panel();
            this.butConfSaveProfile_HA = new System.Windows.Forms.Button();
            this.butConfSaveProfile_HA.Click += new System.EventHandler(this.butConf_Click);
            this.butConfLoadProfile_HA = new System.Windows.Forms.Button();
            this.butConfLoadProfile_HA.Click += new System.EventHandler(this.butConf_Click);
            this.Label1 = new System.Windows.Forms.Label();
            this.butConfSave_HA = new System.Windows.Forms.Button();
            this.butConfSave_HA.Click += new System.EventHandler(this.butConf_Click);
            this.butConfLoad_HA = new System.Windows.Forms.Button();
            this.butConfLoad_HA.Click += new System.EventHandler(this.butConf_Click);
            this.lblConfInfo_HA = new System.Windows.Forms.Label();
            this.pageWork.SuspendLayout();
            this.panelWorkPeriphStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbTool).BeginInit();
            this.panelPower.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pictTrackPowerColor).BeginInit();
            this.pageStationSettings.SuspendLayout();
            this.pageToolSettings.SuspendLayout();
            this.TableLayoutPanelTools.SuspendLayout();
            this.TableLayoutPanelPortsTools.SuspendLayout();
            this.pageLoadSaveSettings.SuspendLayout();
            this.pageResetSettings.SuspendLayout();
            this.pageCounters.SuspendLayout();
            this.TableLayoutPanelPortsCounters.SuspendLayout();
            this.pageDummy.SuspendLayout();
            this.pageGraphics.SuspendLayout();
            this.pageDummy2.SuspendLayout();
            this.panelWorkSleep.SuspendLayout();
            this.pagePeripheral.SuspendLayout();
            this.panelConfigPeripheral.SuspendLayout();
            this.panelConfigParamPeripheral.SuspendLayout();
            this.tlpPeripherals.SuspendLayout();
            this.pageWork_HA.SuspendLayout();
            this.panelPower_HA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pictTrackPowerColor_HA).BeginInit();
            this.pageDummy3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.panelWorkPageProfilePlot).BeginInit();
            this.pageProfiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.panelProfilesPlot).BeginInit();
            this.pageLoadSaveSettings_HA.SuspendLayout();
            this.SuspendLayout();
            //
            //pageWork
            //
            this.pageWork.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageWork.Controls.Add(this.lblToolTempAdjust);
            this.pageWork.Controls.Add(this.panelWorkPeriphStatus);
            this.pageWork.Controls.Add(this.butPort);
            this.pageWork.Controls.Add(this.lblToolTempUnits);
            this.pageWork.Controls.Add(this.pcbTool);
            this.pageWork.Controls.Add(this.lblToolTemp);
            this.pageWork.Controls.Add(this.panelPower);
            this.pageWork.Controls.Add(this.PanelTemps);
            this.pageWork.Location = new System.Drawing.Point(0, 0);
            this.pageWork.Name = "pageWork";
            this.pageWork.Size = new System.Drawing.Size(512, 314);
            this.pageWork.TabIndex = 0;
            //
            //lblToolTempAdjust
            //
            this.lblToolTempAdjust.BackColor = System.Drawing.Color.Transparent;
            this.lblToolTempAdjust.Font = new System.Drawing.Font("Verdana", (float)(18.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolTempAdjust.Location = new System.Drawing.Point(348, 115);
            this.lblToolTempAdjust.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblToolTempAdjust.Name = "lblToolTempAdjust";
            this.lblToolTempAdjust.Size = new System.Drawing.Size(63, 37);
            this.lblToolTempAdjust.TabIndex = 49;
            this.lblToolTempAdjust.Text = "+0";
            this.lblToolTempAdjust.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //panelWorkPeriphStatus
            //
            this.panelWorkPeriphStatus.Controls.Add(this.txtStatusPeriph_2);
            this.panelWorkPeriphStatus.Controls.Add(this.txtStatusPeriph_1);
            this.panelWorkPeriphStatus.Location = new System.Drawing.Point(16, 253);
            this.panelWorkPeriphStatus.Name = "panelWorkPeriphStatus";
            this.panelWorkPeriphStatus.Size = new System.Drawing.Size(67, 54);
            this.panelWorkPeriphStatus.TabIndex = 48;
            this.panelWorkPeriphStatus.Visible = false;
            //
            //txtStatusPeriph_2
            //
            this.txtStatusPeriph_2.Font = new System.Drawing.Font("Verdana", (float)(11.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.txtStatusPeriph_2.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.txtStatusPeriph_2.Location = new System.Drawing.Point(0, 31);
            this.txtStatusPeriph_2.Name = "txtStatusPeriph_2";
            this.txtStatusPeriph_2.Size = new System.Drawing.Size(67, 17);
            this.txtStatusPeriph_2.TabIndex = 2;
            this.txtStatusPeriph_2.Text = "-";
            this.txtStatusPeriph_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //txtStatusPeriph_1
            //
            this.txtStatusPeriph_1.Font = new System.Drawing.Font("Verdana", (float)(11.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.txtStatusPeriph_1.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.txtStatusPeriph_1.Location = new System.Drawing.Point(0, 5);
            this.txtStatusPeriph_1.Name = "txtStatusPeriph_1";
            this.txtStatusPeriph_1.Size = new System.Drawing.Size(67, 17);
            this.txtStatusPeriph_1.TabIndex = 0;
            this.txtStatusPeriph_1.Text = "-";
            this.txtStatusPeriph_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //butPort
            //
            this.butPort.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butPort.FlatAppearance.BorderSize = 0;
            this.butPort.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.butPort.FlatAppearance.MouseOverBackColor = System.Drawing.Color.White;
            this.butPort.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butPort.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butPort.Location = new System.Drawing.Point(422, 215);
            this.butPort.Name = "butPort";
            this.butPort.Size = new System.Drawing.Size(80, 23);
            this.butPort.TabIndex = 46;
            this.butPort.Text = "Port 0";
            this.butPort.UseVisualStyleBackColor = true;
            //
            //lblToolTempUnits
            //
            this.lblToolTempUnits.BackColor = System.Drawing.Color.Transparent;
            this.lblToolTempUnits.Font = new System.Drawing.Font("Verdana", (float)(25.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolTempUnits.Location = new System.Drawing.Point(348, 74);
            this.lblToolTempUnits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblToolTempUnits.Name = "lblToolTempUnits";
            this.lblToolTempUnits.Size = new System.Drawing.Size(63, 37);
            this.lblToolTempUnits.TabIndex = 44;
            this.lblToolTempUnits.Text = "ºC";
            this.lblToolTempUnits.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //pcbTool
            //
            this.pcbTool.BackColor = System.Drawing.Color.Transparent;
            this.pcbTool.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pcbTool.Location = new System.Drawing.Point(422, 20);
            this.pcbTool.Name = "pcbTool";
            this.pcbTool.Size = new System.Drawing.Size(80, 190);
            this.pcbTool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pcbTool.TabIndex = 41;
            this.pcbTool.TabStop = false;
            //
            //lblToolTemp
            //
            this.lblToolTemp.BackColor = System.Drawing.Color.Transparent;
            this.lblToolTemp.Font = new System.Drawing.Font("Verdana", (float)(66.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolTemp.Location = new System.Drawing.Point(147, 55);
            this.lblToolTemp.Margin = new System.Windows.Forms.Padding(0);
            this.lblToolTemp.Name = "lblToolTemp";
            this.lblToolTemp.Size = new System.Drawing.Size(218, 96);
            this.lblToolTemp.TabIndex = 39;
            this.lblToolTemp.Text = "---";
            this.lblToolTemp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //panelPower
            //
            this.panelPower.BackColor = System.Drawing.Color.Transparent;
            this.panelPower.Controls.Add(this.lblPwr);
            this.panelPower.Controls.Add(this.pictTrackPowerColor);
            this.panelPower.Controls.Add(this.lblTitlePwr);
            this.panelPower.Location = new System.Drawing.Point(5, 22);
            this.panelPower.Name = "panelPower";
            this.panelPower.Size = new System.Drawing.Size(89, 223);
            this.panelPower.TabIndex = 47;
            //
            //lblPwr
            //
            this.lblPwr.BackColor = System.Drawing.Color.Transparent;
            this.lblPwr.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblPwr.Location = new System.Drawing.Point(16, 197);
            this.lblPwr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPwr.Name = "lblPwr";
            this.lblPwr.Size = new System.Drawing.Size(63, 23);
            this.lblPwr.TabIndex = 38;
            this.lblPwr.Text = "000 %";
            this.lblPwr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pictTrackPowerColor
            //
            this.pictTrackPowerColor.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.pictTrackPowerColor.Image = My.Resources.Resources.power_99;
            this.pictTrackPowerColor.Location = new System.Drawing.Point(35, 19);
            this.pictTrackPowerColor.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictTrackPowerColor.Name = "pictTrackPowerColor";
            this.pictTrackPowerColor.Size = new System.Drawing.Size(21, 162);
            this.pictTrackPowerColor.TabIndex = 18;
            this.pictTrackPowerColor.TabStop = false;
            //
            //lblTitlePwr
            //
            this.lblTitlePwr.BackColor = System.Drawing.Color.Transparent;
            this.lblTitlePwr.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblTitlePwr.Location = new System.Drawing.Point(15, 183);
            this.lblTitlePwr.Name = "lblTitlePwr";
            this.lblTitlePwr.Size = new System.Drawing.Size(64, 14);
            this.lblTitlePwr.TabIndex = 43;
            this.lblTitlePwr.Text = "Power";
            this.lblTitlePwr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //PanelTemps
            //
            this.PanelTemps.BackColor = System.Drawing.Color.Transparent;
            this.PanelTemps.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.PanelTemps.Location = new System.Drawing.Point(105, 206);
            this.PanelTemps.Name = "PanelTemps";
            this.PanelTemps.Size = new System.Drawing.Size(302, 99);
            this.PanelTemps.TabIndex = 42;
            //
            //pageStationSettings
            //
            this.pageStationSettings.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageStationSettings.Controls.Add(this.panelStationSettings);
            this.pageStationSettings.Controls.Add(this.rbRobotConf);
            this.pageStationSettings.Controls.Add(this.rbEthernetConf);
            this.pageStationSettings.Controls.Add(this.rbGeneralSettings);
            this.pageStationSettings.Location = new System.Drawing.Point(0, 327);
            this.pageStationSettings.Name = "pageStationSettings";
            this.pageStationSettings.Size = new System.Drawing.Size(512, 314);
            this.pageStationSettings.TabIndex = 1;
            //
            //panelStationSettings
            //
            this.panelStationSettings.Location = new System.Drawing.Point(6, 51);
            this.panelStationSettings.Name = "panelStationSettings";
            this.panelStationSettings.Size = new System.Drawing.Size(500, 255);
            this.panelStationSettings.TabIndex = 38;
            //
            //rbRobotConf
            //
            this.rbRobotConf.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbRobotConf.BackColor = System.Drawing.Color.Transparent;
            this.rbRobotConf.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbRobotConf.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbRobotConf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbRobotConf.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.rbRobotConf.Location = new System.Drawing.Point(349, 13);
            this.rbRobotConf.Name = "rbRobotConf";
            this.rbRobotConf.Size = new System.Drawing.Size(137, 26);
            this.rbRobotConf.TabIndex = 37;
            this.rbRobotConf.Text = "Robot";
            this.rbRobotConf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbRobotConf.UseVisualStyleBackColor = false;
            //
            //rbEthernetConf
            //
            this.rbEthernetConf.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbEthernetConf.BackColor = System.Drawing.Color.Transparent;
            this.rbEthernetConf.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbEthernetConf.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbEthernetConf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbEthernetConf.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.rbEthernetConf.Location = new System.Drawing.Point(187, 13);
            this.rbEthernetConf.Name = "rbEthernetConf";
            this.rbEthernetConf.Size = new System.Drawing.Size(137, 26);
            this.rbEthernetConf.TabIndex = 36;
            this.rbEthernetConf.Text = "Ethernet";
            this.rbEthernetConf.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbEthernetConf.UseVisualStyleBackColor = false;
            //
            //rbGeneralSettings
            //
            this.rbGeneralSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbGeneralSettings.BackColor = System.Drawing.Color.Transparent;
            this.rbGeneralSettings.Checked = true;
            this.rbGeneralSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbGeneralSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbGeneralSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbGeneralSettings.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.rbGeneralSettings.Location = new System.Drawing.Point(25, 13);
            this.rbGeneralSettings.Name = "rbGeneralSettings";
            this.rbGeneralSettings.Size = new System.Drawing.Size(137, 26);
            this.rbGeneralSettings.TabIndex = 35;
            this.rbGeneralSettings.TabStop = true;
            this.rbGeneralSettings.Text = "General Settings";
            this.rbGeneralSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbGeneralSettings.UseVisualStyleBackColor = false;
            //
            //pageToolSettings
            //
            this.pageToolSettings.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageToolSettings.Controls.Add(this.lblSelectedTool);
            this.pageToolSettings.Controls.Add(this.TableLayoutPanelTools);
            this.pageToolSettings.Controls.Add(this.TableLayoutPanelPortsTools);
            this.pageToolSettings.Location = new System.Drawing.Point(531, 328);
            this.pageToolSettings.Name = "pageToolSettings";
            this.pageToolSettings.Size = new System.Drawing.Size(512, 314);
            this.pageToolSettings.TabIndex = 2;
            //
            //lblSelectedTool
            //
            this.lblSelectedTool.BackColor = System.Drawing.Color.Transparent;
            this.lblSelectedTool.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblSelectedTool.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblSelectedTool.Location = new System.Drawing.Point(3, 60);
            this.lblSelectedTool.Name = "lblSelectedTool";
            this.lblSelectedTool.Size = new System.Drawing.Size(417, 15);
            this.lblSelectedTool.TabIndex = 36;
            this.lblSelectedTool.Text = "T210";
            this.lblSelectedTool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //TableLayoutPanelTools
            //
            this.TableLayoutPanelTools.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanelTools.ColumnCount = 8;
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(59.0F)));
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(57.0F)));
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(58.0F)));
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(58.0F)));
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(59.0F)));
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(58.0F)));
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(58.0F)));
            this.TableLayoutPanelTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(23.0F)));
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T5, 4, 0);
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T6, 5, 0);
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T4, 3, 0);
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T1, 0, 0);
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T3, 2, 0);
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T2, 1, 0);
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T7, 6, 0);
            this.TableLayoutPanelTools.Controls.Add(this.rbToolSettings_T8, 7, 0);
            this.TableLayoutPanelTools.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanelTools.Name = "TableLayoutPanelTools";
            this.TableLayoutPanelTools.RowCount = 1;
            this.TableLayoutPanelTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(58.0F)));
            this.TableLayoutPanelTools.Size = new System.Drawing.Size(466, 58);
            this.TableLayoutPanelTools.TabIndex = 35;
            //
            //rbToolSettings_T5
            //
            this.rbToolSettings_T5.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T5.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T5.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T5.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T5.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T5.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T5.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T5.Image = My.Resources.Resources.DS_mini;
            this.rbToolSettings_T5.Location = new System.Drawing.Point(235, 3);
            this.rbToolSettings_T5.Name = "rbToolSettings_T5";
            this.rbToolSettings_T5.Size = new System.Drawing.Size(52, 52);
            this.rbToolSettings_T5.TabIndex = 34;
            this.rbToolSettings_T5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T5.UseVisualStyleBackColor = false;
            //
            //rbToolSettings_T6
            //
            this.rbToolSettings_T6.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T6.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T6.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T6.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T6.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T6.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T6.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T6.Image = My.Resources.Resources.DR_mini;
            this.rbToolSettings_T6.Location = new System.Drawing.Point(294, 3);
            this.rbToolSettings_T6.Name = "rbToolSettings_T6";
            this.rbToolSettings_T6.Size = new System.Drawing.Size(52, 52);
            this.rbToolSettings_T6.TabIndex = 35;
            this.rbToolSettings_T6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T6.UseVisualStyleBackColor = false;
            //
            //rbToolSettings_T4
            //
            this.rbToolSettings_T4.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T4.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T4.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T4.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T4.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T4.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T4.Image = My.Resources.Resources.HT_mini;
            this.rbToolSettings_T4.Location = new System.Drawing.Point(177, 3);
            this.rbToolSettings_T4.Name = "rbToolSettings_T4";
            this.rbToolSettings_T4.Size = new System.Drawing.Size(52, 52);
            this.rbToolSettings_T4.TabIndex = 32;
            this.rbToolSettings_T4.Tag = "";
            this.rbToolSettings_T4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T4.UseVisualStyleBackColor = false;
            //
            //rbToolSettings_T1
            //
            this.rbToolSettings_T1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T1.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T1.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T1.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T1.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T1.Image = My.Resources.Resources.T210_mini;
            this.rbToolSettings_T1.Location = new System.Drawing.Point(3, 3);
            this.rbToolSettings_T1.Name = "rbToolSettings_T1";
            this.rbToolSettings_T1.Size = new System.Drawing.Size(52, 52);
            this.rbToolSettings_T1.TabIndex = 30;
            this.rbToolSettings_T1.Tag = "";
            this.rbToolSettings_T1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T1.UseVisualStyleBackColor = false;
            //
            //rbToolSettings_T3
            //
            this.rbToolSettings_T3.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T3.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T3.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T3.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T3.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T3.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T3.Image = My.Resources.Resources.PA_mini;
            this.rbToolSettings_T3.Location = new System.Drawing.Point(119, 3);
            this.rbToolSettings_T3.Name = "rbToolSettings_T3";
            this.rbToolSettings_T3.Size = new System.Drawing.Size(52, 52);
            this.rbToolSettings_T3.TabIndex = 31;
            this.rbToolSettings_T3.Tag = "";
            this.rbToolSettings_T3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T3.UseVisualStyleBackColor = false;
            //
            //rbToolSettings_T2
            //
            this.rbToolSettings_T2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T2.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T2.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T2.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T2.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T2.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T2.Image = My.Resources.Resources.T245_mini;
            this.rbToolSettings_T2.Location = new System.Drawing.Point(62, 3);
            this.rbToolSettings_T2.Name = "rbToolSettings_T2";
            this.rbToolSettings_T2.Size = new System.Drawing.Size(51, 52);
            this.rbToolSettings_T2.TabIndex = 33;
            this.rbToolSettings_T2.Tag = "";
            this.rbToolSettings_T2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T2.UseVisualStyleBackColor = false;
            //
            //rbToolSettings_T7
            //
            this.rbToolSettings_T7.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T7.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T7.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T7.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T7.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T7.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T7.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T7.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T7.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T7.Image = My.Resources.Resources.NT105_mini;
            this.rbToolSettings_T7.Location = new System.Drawing.Point(352, 3);
            this.rbToolSettings_T7.Name = "rbToolSettings_T7";
            this.rbToolSettings_T7.Size = new System.Drawing.Size(52, 52);
            this.rbToolSettings_T7.TabIndex = 36;
            this.rbToolSettings_T7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T7.UseVisualStyleBackColor = false;
            //
            //rbToolSettings_T8
            //
            this.rbToolSettings_T8.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbToolSettings_T8.BackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbToolSettings_T8.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T8.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbToolSettings_T8.FlatAppearance.BorderSize = 0;
            this.rbToolSettings_T8.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T8.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T8.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbToolSettings_T8.ForeColor = System.Drawing.Color.Transparent;
            this.rbToolSettings_T8.Image = My.Resources.Resources.NP105_mini;
            this.rbToolSettings_T8.Location = new System.Drawing.Point(410, 3);
            this.rbToolSettings_T8.Name = "rbToolSettings_T8";
            this.rbToolSettings_T8.Size = new System.Drawing.Size(52, 52);
            this.rbToolSettings_T8.TabIndex = 37;
            this.rbToolSettings_T8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbToolSettings_T8.UseVisualStyleBackColor = false;
            //
            //TableLayoutPanelPortsTools
            //
            this.TableLayoutPanelPortsTools.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.TableLayoutPanelPortsTools.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanelPortsTools.ColumnCount = 1;
            this.TableLayoutPanelPortsTools.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(47.0F)));
            this.TableLayoutPanelPortsTools.Controls.Add(this.rbPortTools_1, 0, 0);
            this.TableLayoutPanelPortsTools.Controls.Add(this.rbPortTools_2, 0, 1);
            this.TableLayoutPanelPortsTools.Controls.Add(this.rbPortTools_3, 0, 2);
            this.TableLayoutPanelPortsTools.Controls.Add(this.rbPortTools_4, 0, 3);
            this.TableLayoutPanelPortsTools.Location = new System.Drawing.Point(465, 85);
            this.TableLayoutPanelPortsTools.Name = "TableLayoutPanelPortsTools";
            this.TableLayoutPanelPortsTools.RowCount = 4;
            this.TableLayoutPanelPortsTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsTools.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsTools.Size = new System.Drawing.Size(47, 172);
            this.TableLayoutPanelPortsTools.TabIndex = 34;
            //
            //rbPortTools_1
            //
            this.rbPortTools_1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortTools_1.BackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortTools_1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_1.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortTools_1.FlatAppearance.BorderSize = 0;
            this.rbPortTools_1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortTools_1.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortTools_1.Image = My.Resources.Resources.Port1mini;
            this.rbPortTools_1.Location = new System.Drawing.Point(3, 3);
            this.rbPortTools_1.Name = "rbPortTools_1";
            this.rbPortTools_1.Size = new System.Drawing.Size(35, 35);
            this.rbPortTools_1.TabIndex = 30;
            this.rbPortTools_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_1.UseVisualStyleBackColor = false;
            //
            //rbPortTools_2
            //
            this.rbPortTools_2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortTools_2.BackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortTools_2.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_2.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortTools_2.FlatAppearance.BorderSize = 0;
            this.rbPortTools_2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortTools_2.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortTools_2.Image = My.Resources.Resources.Port2mini;
            this.rbPortTools_2.Location = new System.Drawing.Point(3, 45);
            this.rbPortTools_2.Name = "rbPortTools_2";
            this.rbPortTools_2.Size = new System.Drawing.Size(35, 35);
            this.rbPortTools_2.TabIndex = 33;
            this.rbPortTools_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_2.UseVisualStyleBackColor = false;
            //
            //rbPortTools_3
            //
            this.rbPortTools_3.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortTools_3.BackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortTools_3.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_3.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortTools_3.FlatAppearance.BorderSize = 0;
            this.rbPortTools_3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortTools_3.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortTools_3.Image = My.Resources.Resources.Port3mini;
            this.rbPortTools_3.Location = new System.Drawing.Point(3, 87);
            this.rbPortTools_3.Name = "rbPortTools_3";
            this.rbPortTools_3.Size = new System.Drawing.Size(35, 35);
            this.rbPortTools_3.TabIndex = 31;
            this.rbPortTools_3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_3.UseVisualStyleBackColor = false;
            //
            //rbPortTools_4
            //
            this.rbPortTools_4.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortTools_4.BackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortTools_4.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_4.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortTools_4.FlatAppearance.BorderSize = 0;
            this.rbPortTools_4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortTools_4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortTools_4.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortTools_4.Image = My.Resources.Resources.Port4mini;
            this.rbPortTools_4.Location = new System.Drawing.Point(3, 129);
            this.rbPortTools_4.Name = "rbPortTools_4";
            this.rbPortTools_4.Size = new System.Drawing.Size(35, 35);
            this.rbPortTools_4.TabIndex = 32;
            this.rbPortTools_4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortTools_4.UseVisualStyleBackColor = false;
            //
            //pageLoadSaveSettings
            //
            this.pageLoadSaveSettings.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageLoadSaveSettings.Controls.Add(this.butConfSave);
            this.pageLoadSaveSettings.Controls.Add(this.butConfLoad);
            this.pageLoadSaveSettings.Controls.Add(this.lblConfInfo);
            this.pageLoadSaveSettings.Location = new System.Drawing.Point(0, 657);
            this.pageLoadSaveSettings.Name = "pageLoadSaveSettings";
            this.pageLoadSaveSettings.Size = new System.Drawing.Size(512, 314);
            this.pageLoadSaveSettings.TabIndex = 3;
            //
            //butConfSave
            //
            this.butConfSave.BackColor = System.Drawing.Color.White;
            this.butConfSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butConfSave.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butConfSave.Location = new System.Drawing.Point(266, 189);
            this.butConfSave.Name = "butConfSave";
            this.butConfSave.Size = new System.Drawing.Size(165, 26);
            this.butConfSave.TabIndex = 5;
            this.butConfSave.Text = "Save to File";
            this.butConfSave.UseVisualStyleBackColor = false;
            //
            //butConfLoad
            //
            this.butConfLoad.BackColor = System.Drawing.Color.White;
            this.butConfLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butConfLoad.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butConfLoad.Location = new System.Drawing.Point(80, 189);
            this.butConfLoad.Name = "butConfLoad";
            this.butConfLoad.Size = new System.Drawing.Size(165, 26);
            this.butConfLoad.TabIndex = 4;
            this.butConfLoad.Text = "Load from File";
            this.butConfLoad.UseVisualStyleBackColor = false;
            //
            //lblConfInfo
            //
            this.lblConfInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblConfInfo.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblConfInfo.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblConfInfo.Location = new System.Drawing.Point(69, 99);
            this.lblConfInfo.Name = "lblConfInfo";
            this.lblConfInfo.Size = new System.Drawing.Size(374, 76);
            this.lblConfInfo.TabIndex = 3;
            this.lblConfInfo.Text = "Load a configuration file for the station or save the current station configurati" +
                "on.";
            this.lblConfInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageResetSettings
            //
            this.pageResetSettings.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageResetSettings.Controls.Add(this.pgbResetBar);
            this.pageResetSettings.Controls.Add(this.butResetProceed);
            this.pageResetSettings.Controls.Add(this.lblResetInfo);
            this.pageResetSettings.Location = new System.Drawing.Point(531, 987);
            this.pageResetSettings.Name = "pageResetSettings";
            this.pageResetSettings.Size = new System.Drawing.Size(512, 314);
            this.pageResetSettings.TabIndex = 4;
            //
            //pgbResetBar
            //
            this.pgbResetBar.Location = new System.Drawing.Point(76, 208);
            this.pgbResetBar.Name = "pgbResetBar";
            this.pgbResetBar.Size = new System.Drawing.Size(319, 23);
            this.pgbResetBar.TabIndex = 6;
            //
            //butResetProceed
            //
            this.butResetProceed.BackColor = System.Drawing.Color.White;
            this.butResetProceed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butResetProceed.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butResetProceed.Location = new System.Drawing.Point(171, 162);
            this.butResetProceed.Name = "butResetProceed";
            this.butResetProceed.Size = new System.Drawing.Size(113, 26);
            this.butResetProceed.TabIndex = 5;
            this.butResetProceed.Text = "Proceed";
            this.butResetProceed.UseVisualStyleBackColor = false;
            //
            //lblResetInfo
            //
            this.lblResetInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblResetInfo.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblResetInfo.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblResetInfo.Location = new System.Drawing.Point(46, 83);
            this.lblResetInfo.Name = "lblResetInfo";
            this.lblResetInfo.Size = new System.Drawing.Size(374, 76);
            this.lblResetInfo.TabIndex = 4;
            this.lblResetInfo.Text = "Do you want to restore all the station parameters to its default values?";
            this.lblResetInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageCounters
            //
            this.pageCounters.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageCounters.Controls.Add(this.butResetPartialCounters);
            this.pageCounters.Controls.Add(this.rbPartialCounters);
            this.pageCounters.Controls.Add(this.rbGlobalCounters);
            this.pageCounters.Controls.Add(this.TableLayoutPanelPortsCounters);
            this.pageCounters.Location = new System.Drawing.Point(0, 987);
            this.pageCounters.Name = "pageCounters";
            this.pageCounters.Size = new System.Drawing.Size(512, 314);
            this.pageCounters.TabIndex = 5;
            //
            //butResetPartialCounters
            //
            this.butResetPartialCounters.BackColor = System.Drawing.Color.White;
            this.butResetPartialCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butResetPartialCounters.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butResetPartialCounters.Location = new System.Drawing.Point(49, 264);
            this.butResetPartialCounters.Name = "butResetPartialCounters";
            this.butResetPartialCounters.Size = new System.Drawing.Size(361, 26);
            this.butResetPartialCounters.TabIndex = 34;
            this.butResetPartialCounters.Text = "Reset Port Partial Counters";
            this.butResetPartialCounters.UseVisualStyleBackColor = false;
            //
            //rbPartialCounters
            //
            this.rbPartialCounters.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPartialCounters.AutoSize = true;
            this.rbPartialCounters.BackColor = System.Drawing.Color.Transparent;
            this.rbPartialCounters.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbPartialCounters.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbPartialCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPartialCounters.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.rbPartialCounters.Location = new System.Drawing.Point(235, 13);
            this.rbPartialCounters.Name = "rbPartialCounters";
            this.rbPartialCounters.Size = new System.Drawing.Size(121, 26);
            this.rbPartialCounters.TabIndex = 33;
            this.rbPartialCounters.Text = "Partial Counters";
            this.rbPartialCounters.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPartialCounters.UseVisualStyleBackColor = false;
            //
            //rbGlobalCounters
            //
            this.rbGlobalCounters.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbGlobalCounters.AutoSize = true;
            this.rbGlobalCounters.BackColor = System.Drawing.Color.Transparent;
            this.rbGlobalCounters.Checked = true;
            this.rbGlobalCounters.FlatAppearance.CheckedBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbGlobalCounters.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSteelBlue;
            this.rbGlobalCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbGlobalCounters.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.rbGlobalCounters.Location = new System.Drawing.Point(65, 13);
            this.rbGlobalCounters.Name = "rbGlobalCounters";
            this.rbGlobalCounters.Size = new System.Drawing.Size(120, 26);
            this.rbGlobalCounters.TabIndex = 32;
            this.rbGlobalCounters.TabStop = true;
            this.rbGlobalCounters.Text = "Global Counters";
            this.rbGlobalCounters.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbGlobalCounters.UseVisualStyleBackColor = false;
            //
            //TableLayoutPanelPortsCounters
            //
            this.TableLayoutPanelPortsCounters.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.TableLayoutPanelPortsCounters.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanelPortsCounters.ColumnCount = 1;
            this.TableLayoutPanelPortsCounters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(47.0F)));
            this.TableLayoutPanelPortsCounters.Controls.Add(this.rbPortCounters_1, 0, 0);
            this.TableLayoutPanelPortsCounters.Controls.Add(this.rbPortCounters_2, 0, 1);
            this.TableLayoutPanelPortsCounters.Controls.Add(this.rbPortCounters_3, 0, 2);
            this.TableLayoutPanelPortsCounters.Controls.Add(this.rbPortCounters_4, 0, 3);
            this.TableLayoutPanelPortsCounters.Location = new System.Drawing.Point(465, 38);
            this.TableLayoutPanelPortsCounters.Name = "TableLayoutPanelPortsCounters";
            this.TableLayoutPanelPortsCounters.RowCount = 4;
            this.TableLayoutPanelPortsCounters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsCounters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsCounters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsCounters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(42.0F)));
            this.TableLayoutPanelPortsCounters.Size = new System.Drawing.Size(47, 176);
            this.TableLayoutPanelPortsCounters.TabIndex = 31;
            //
            //rbPortCounters_1
            //
            this.rbPortCounters_1.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortCounters_1.BackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortCounters_1.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_1.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortCounters_1.FlatAppearance.BorderSize = 0;
            this.rbPortCounters_1.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortCounters_1.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_1.Image = My.Resources.Resources.Port1mini;
            this.rbPortCounters_1.Location = new System.Drawing.Point(3, 3);
            this.rbPortCounters_1.Name = "rbPortCounters_1";
            this.rbPortCounters_1.Size = new System.Drawing.Size(35, 35);
            this.rbPortCounters_1.TabIndex = 30;
            this.rbPortCounters_1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_1.UseVisualStyleBackColor = false;
            //
            //rbPortCounters_2
            //
            this.rbPortCounters_2.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortCounters_2.BackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortCounters_2.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_2.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortCounters_2.FlatAppearance.BorderSize = 0;
            this.rbPortCounters_2.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortCounters_2.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_2.Image = My.Resources.Resources.Port2mini;
            this.rbPortCounters_2.Location = new System.Drawing.Point(3, 45);
            this.rbPortCounters_2.Name = "rbPortCounters_2";
            this.rbPortCounters_2.Size = new System.Drawing.Size(35, 35);
            this.rbPortCounters_2.TabIndex = 33;
            this.rbPortCounters_2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_2.UseVisualStyleBackColor = false;
            //
            //rbPortCounters_3
            //
            this.rbPortCounters_3.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortCounters_3.BackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortCounters_3.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_3.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortCounters_3.FlatAppearance.BorderSize = 0;
            this.rbPortCounters_3.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortCounters_3.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_3.Image = My.Resources.Resources.Port3mini;
            this.rbPortCounters_3.Location = new System.Drawing.Point(3, 87);
            this.rbPortCounters_3.Name = "rbPortCounters_3";
            this.rbPortCounters_3.Size = new System.Drawing.Size(35, 35);
            this.rbPortCounters_3.TabIndex = 31;
            this.rbPortCounters_3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_3.UseVisualStyleBackColor = false;
            //
            //rbPortCounters_4
            //
            this.rbPortCounters_4.Appearance = System.Windows.Forms.Appearance.Button;
            this.rbPortCounters_4.BackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.rbPortCounters_4.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_4.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.rbPortCounters_4.FlatAppearance.BorderSize = 0;
            this.rbPortCounters_4.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rbPortCounters_4.ForeColor = System.Drawing.Color.Transparent;
            this.rbPortCounters_4.Image = My.Resources.Resources.Port4mini;
            this.rbPortCounters_4.Location = new System.Drawing.Point(3, 129);
            this.rbPortCounters_4.Name = "rbPortCounters_4";
            this.rbPortCounters_4.Size = new System.Drawing.Size(35, 35);
            this.rbPortCounters_4.TabIndex = 32;
            this.rbPortCounters_4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rbPortCounters_4.UseVisualStyleBackColor = false;
            //
            //pageInfo
            //
            this.pageInfo.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageInfo.Location = new System.Drawing.Point(531, 1317);
            this.pageInfo.Name = "pageInfo";
            this.pageInfo.Size = new System.Drawing.Size(512, 314);
            this.pageInfo.TabIndex = 6;
            //
            //pageDummy
            //
            this.pageDummy.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageDummy.Controls.Add(this.lblNoTool);
            this.pageDummy.Controls.Add(this.lblToolNeeded);
            this.pageDummy.Location = new System.Drawing.Point(531, 1647);
            this.pageDummy.Name = "pageDummy";
            this.pageDummy.Size = new System.Drawing.Size(512, 314);
            this.pageDummy.TabIndex = 7;
            //
            //lblNoTool
            //
            this.lblNoTool.BackColor = System.Drawing.Color.Transparent;
            this.lblNoTool.Font = new System.Drawing.Font("Verdana", (float)(26.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblNoTool.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblNoTool.Location = new System.Drawing.Point(42, 24);
            this.lblNoTool.Margin = new System.Windows.Forms.Padding(0);
            this.lblNoTool.Name = "lblNoTool";
            this.lblNoTool.Size = new System.Drawing.Size(374, 119);
            this.lblNoTool.TabIndex = 48;
            this.lblNoTool.Text = "NO TOOL SET";
            this.lblNoTool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //lblToolNeeded
            //
            this.lblToolNeeded.BackColor = System.Drawing.Color.Transparent;
            this.lblToolNeeded.Font = new System.Drawing.Font("Verdana", (float)(20.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolNeeded.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblToolNeeded.Location = new System.Drawing.Point(20, 143);
            this.lblToolNeeded.Margin = new System.Windows.Forms.Padding(0);
            this.lblToolNeeded.Name = "lblToolNeeded";
            this.lblToolNeeded.Size = new System.Drawing.Size(411, 93);
            this.lblToolNeeded.TabIndex = 49;
            this.lblToolNeeded.Text = "TOOL AND CARTRIDGE ARE NEEDED";
            this.lblToolNeeded.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageGraphics
            //
            this.pageGraphics.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageGraphics.Controls.Add(this.lblGraphInfo);
            this.pageGraphics.Controls.Add(this.butGraphAddSeries);
            this.pageGraphics.Controls.Add(this.cbxGraphPlots);
            this.pageGraphics.Controls.Add(this.lblGraphAddToPlot);
            this.pageGraphics.Location = new System.Drawing.Point(0, 1317);
            this.pageGraphics.Name = "pageGraphics";
            this.pageGraphics.Size = new System.Drawing.Size(512, 314);
            this.pageGraphics.TabIndex = 8;
            //
            //lblGraphInfo
            //
            this.lblGraphInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblGraphInfo.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblGraphInfo.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblGraphInfo.Location = new System.Drawing.Point(46, 30);
            this.lblGraphInfo.Name = "lblGraphInfo";
            this.lblGraphInfo.Size = new System.Drawing.Size(374, 76);
            this.lblGraphInfo.TabIndex = 21;
            this.lblGraphInfo.Text = "Select a plot to add series for graphics";
            this.lblGraphInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //butGraphAddSeries
            //
            this.butGraphAddSeries.BackColor = System.Drawing.Color.White;
            this.butGraphAddSeries.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butGraphAddSeries.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butGraphAddSeries.Location = new System.Drawing.Point(137, 222);
            this.butGraphAddSeries.Name = "butGraphAddSeries";
            this.butGraphAddSeries.Size = new System.Drawing.Size(165, 26);
            this.butGraphAddSeries.TabIndex = 20;
            this.butGraphAddSeries.Text = "Add Series";
            this.butGraphAddSeries.UseVisualStyleBackColor = false;
            //
            //cbxGraphPlots
            //
            this.cbxGraphPlots.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxGraphPlots.FormattingEnabled = true;
            this.cbxGraphPlots.Location = new System.Drawing.Point(235, 147);
            this.cbxGraphPlots.Name = "cbxGraphPlots";
            this.cbxGraphPlots.Size = new System.Drawing.Size(209, 22);
            this.cbxGraphPlots.TabIndex = 19;
            //
            //lblGraphAddToPlot
            //
            this.lblGraphAddToPlot.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblGraphAddToPlot.Location = new System.Drawing.Point(22, 150);
            this.lblGraphAddToPlot.Name = "lblGraphAddToPlot";
            this.lblGraphAddToPlot.Size = new System.Drawing.Size(207, 14);
            this.lblGraphAddToPlot.TabIndex = 18;
            this.lblGraphAddToPlot.Text = "Add series to this plot . . . . . . . . . . . . . . . . ";
            //
            //pageDummy2
            //
            this.pageDummy2.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageDummy2.Controls.Add(this.panelWorkSleep);
            this.pageDummy2.Location = new System.Drawing.Point(0, 1651);
            this.pageDummy2.Name = "pageDummy2";
            this.pageDummy2.Size = new System.Drawing.Size(512, 314);
            this.pageDummy2.TabIndex = 9;
            //
            //panelWorkSleep
            //
            this.panelWorkSleep.Controls.Add(this.labWorkSleepStatus);
            this.panelWorkSleep.Controls.Add(this.labWorkSleepDelay);
            this.panelWorkSleep.Controls.Add(this.labWorkSleepStand);
            this.panelWorkSleep.Controls.Add(this.labWorkSleepTemp);
            this.panelWorkSleep.Location = new System.Drawing.Point(30, 24);
            this.panelWorkSleep.Name = "panelWorkSleep";
            this.panelWorkSleep.Size = new System.Drawing.Size(350, 174);
            this.panelWorkSleep.TabIndex = 53;
            this.panelWorkSleep.Visible = false;
            //
            //labWorkSleepStatus
            //
            this.labWorkSleepStatus.BackColor = System.Drawing.Color.Transparent;
            this.labWorkSleepStatus.Font = new System.Drawing.Font("Verdana", (float)(21.75F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.labWorkSleepStatus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labWorkSleepStatus.Location = new System.Drawing.Point(4, 10);
            this.labWorkSleepStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labWorkSleepStatus.Name = "labWorkSleepStatus";
            this.labWorkSleepStatus.Size = new System.Drawing.Size(341, 37);
            this.labWorkSleepStatus.TabIndex = 49;
            this.labWorkSleepStatus.Text = "Sleep";
            //
            //labWorkSleepDelay
            //
            this.labWorkSleepDelay.BackColor = System.Drawing.Color.Transparent;
            this.labWorkSleepDelay.Font = new System.Drawing.Font("Verdana", (float)(15.75F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.labWorkSleepDelay.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labWorkSleepDelay.Location = new System.Drawing.Point(5, 130);
            this.labWorkSleepDelay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labWorkSleepDelay.Name = "labWorkSleepDelay";
            this.labWorkSleepDelay.Size = new System.Drawing.Size(341, 37);
            this.labWorkSleepDelay.TabIndex = 52;
            this.labWorkSleepDelay.Text = "Delay to enter sleep 2:05";
            //
            //labWorkSleepStand
            //
            this.labWorkSleepStand.BackColor = System.Drawing.Color.Transparent;
            this.labWorkSleepStand.Font = new System.Drawing.Font("Verdana", (float)(18.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.labWorkSleepStand.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labWorkSleepStand.Location = new System.Drawing.Point(4, 53);
            this.labWorkSleepStand.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labWorkSleepStand.Name = "labWorkSleepStand";
            this.labWorkSleepStand.Size = new System.Drawing.Size(341, 37);
            this.labWorkSleepStand.TabIndex = 50;
            this.labWorkSleepStand.Text = "Tool in the stand";
            //
            //labWorkSleepTemp
            //
            this.labWorkSleepTemp.BackColor = System.Drawing.Color.Transparent;
            this.labWorkSleepTemp.Font = new System.Drawing.Font("Verdana", (float)(15.75F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.labWorkSleepTemp.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labWorkSleepTemp.Location = new System.Drawing.Point(5, 94);
            this.labWorkSleepTemp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labWorkSleepTemp.Name = "labWorkSleepTemp";
            this.labWorkSleepTemp.Size = new System.Drawing.Size(341, 37);
            this.labWorkSleepTemp.TabIndex = 51;
            this.labWorkSleepTemp.Text = "Actual Temp: 250 ºC";
            //
            //pagePeripheral
            //
            this.pagePeripheral.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pagePeripheral.Controls.Add(this.lblNoPeripheralSupported);
            this.pagePeripheral.Controls.Add(this.panelConfigPeripheral);
            this.pagePeripheral.Controls.Add(this.tlpPeripherals);
            this.pagePeripheral.Controls.Add(this.lblNoPeripheral);
            this.pagePeripheral.Location = new System.Drawing.Point(531, 1981);
            this.pagePeripheral.Name = "pagePeripheral";
            this.pagePeripheral.Size = new System.Drawing.Size(512, 314);
            this.pagePeripheral.TabIndex = 10;
            //
            //lblNoPeripheralSupported
            //
            this.lblNoPeripheralSupported.BackColor = System.Drawing.Color.Transparent;
            this.lblNoPeripheralSupported.Font = new System.Drawing.Font("Verdana", (float)(26.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblNoPeripheralSupported.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblNoPeripheralSupported.Location = new System.Drawing.Point(18, 75);
            this.lblNoPeripheralSupported.Margin = new System.Windows.Forms.Padding(0);
            this.lblNoPeripheralSupported.Name = "lblNoPeripheralSupported";
            this.lblNoPeripheralSupported.Size = new System.Drawing.Size(476, 164);
            this.lblNoPeripheralSupported.TabIndex = 54;
            this.lblNoPeripheralSupported.Text = "NO PERIPHERAL SUPPORTED";
            this.lblNoPeripheralSupported.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNoPeripheralSupported.Visible = false;
            //
            //panelConfigPeripheral
            //
            this.panelConfigPeripheral.Controls.Add(this.textTypePeripheral);
            this.panelConfigPeripheral.Controls.Add(this.panelConfigParamPeripheral);
            this.panelConfigPeripheral.Controls.Add(this.lineSeparator);
            this.panelConfigPeripheral.Controls.Add(this.textNamePeripheral);
            this.panelConfigPeripheral.Location = new System.Drawing.Point(270, 15);
            this.panelConfigPeripheral.Name = "panelConfigPeripheral";
            this.panelConfigPeripheral.Size = new System.Drawing.Size(227, 284);
            this.panelConfigPeripheral.TabIndex = 53;
            this.panelConfigPeripheral.Visible = false;
            //
            //textTypePeripheral
            //
            this.textTypePeripheral.AutoSize = true;
            this.textTypePeripheral.Font = new System.Drawing.Font("Verdana", (float)(10.0F), System.Drawing.FontStyle.Bold);
            this.textTypePeripheral.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.textTypePeripheral.Location = new System.Drawing.Point(45, 38);
            this.textTypePeripheral.Name = "textTypePeripheral";
            this.textTypePeripheral.Size = new System.Drawing.Size(46, 17);
            this.textTypePeripheral.TabIndex = 57;
            this.textTypePeripheral.Text = "Type";
            //
            //panelConfigParamPeripheral
            //
            this.panelConfigParamPeripheral.Controls.Add(this.inputErrorTimePeripheral);
            this.panelConfigParamPeripheral.Controls.Add(this.inputTimePeripheral);
            this.panelConfigParamPeripheral.Controls.Add(this.textFunctionPeripheral);
            this.panelConfigParamPeripheral.Controls.Add(this.inputFunctionPeripheral);
            this.panelConfigParamPeripheral.Controls.Add(this.textTimePeripheral);
            this.panelConfigParamPeripheral.Controls.Add(this.inputActivationPeripheral);
            this.panelConfigParamPeripheral.Controls.Add(this.textActivationPeripheral);
            this.panelConfigParamPeripheral.Location = new System.Drawing.Point(15, 71);
            this.panelConfigParamPeripheral.Name = "panelConfigParamPeripheral";
            this.panelConfigParamPeripheral.Size = new System.Drawing.Size(212, 213);
            this.panelConfigParamPeripheral.TabIndex = 56;
            //
            //inputErrorTimePeripheral
            //
            this.inputErrorTimePeripheral.AutoSize = true;
            this.inputErrorTimePeripheral.ForeColor = System.Drawing.Color.Red;
            this.inputErrorTimePeripheral.Location = new System.Drawing.Point(45, 186);
            this.inputErrorTimePeripheral.Name = "inputErrorTimePeripheral";
            this.inputErrorTimePeripheral.Size = new System.Drawing.Size(167, 14);
            this.inputErrorTimePeripheral.TabIndex = 56;
            this.inputErrorTimePeripheral.Text = "Allowed range [0.0 .. 9.9]";
            this.inputErrorTimePeripheral.Visible = false;
            //
            //inputTimePeripheral
            //
            this.inputTimePeripheral.Location = new System.Drawing.Point(45, 161);
            this.inputTimePeripheral.MaxLength = 3;
            this.inputTimePeripheral.Name = "inputTimePeripheral";
            this.inputTimePeripheral.Size = new System.Drawing.Size(121, 22);
            this.inputTimePeripheral.TabIndex = 55;
            //
            //textFunctionPeripheral
            //
            this.textFunctionPeripheral.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.textFunctionPeripheral.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.textFunctionPeripheral.Location = new System.Drawing.Point(30, 15);
            this.textFunctionPeripheral.Name = "textFunctionPeripheral";
            this.textFunctionPeripheral.Size = new System.Drawing.Size(162, 14);
            this.textFunctionPeripheral.TabIndex = 1;
            this.textFunctionPeripheral.Text = "Function";
            //
            //inputFunctionPeripheral
            //
            this.inputFunctionPeripheral.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputFunctionPeripheral.FormattingEnabled = true;
            this.inputFunctionPeripheral.Items.AddRange(new object[] { "Sleep", "Extractor", "Modul" });
            this.inputFunctionPeripheral.Location = new System.Drawing.Point(45, 37);
            this.inputFunctionPeripheral.Name = "inputFunctionPeripheral";
            this.inputFunctionPeripheral.Size = new System.Drawing.Size(121, 22);
            this.inputFunctionPeripheral.TabIndex = 4;
            //
            //textTimePeripheral
            //
            this.textTimePeripheral.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.textTimePeripheral.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.textTimePeripheral.Location = new System.Drawing.Point(30, 139);
            this.textTimePeripheral.Name = "textTimePeripheral";
            this.textTimePeripheral.Size = new System.Drawing.Size(162, 14);
            this.textTimePeripheral.TabIndex = 3;
            this.textTimePeripheral.Text = "Minimum time (seconds)";
            //
            //inputActivationPeripheral
            //
            this.inputActivationPeripheral.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inputActivationPeripheral.FormattingEnabled = true;
            this.inputActivationPeripheral.Items.AddRange(new object[] { "Pressed", "Pulled" });
            this.inputActivationPeripheral.Location = new System.Drawing.Point(45, 99);
            this.inputActivationPeripheral.Name = "inputActivationPeripheral";
            this.inputActivationPeripheral.Size = new System.Drawing.Size(121, 22);
            this.inputActivationPeripheral.TabIndex = 5;
            //
            //textActivationPeripheral
            //
            this.textActivationPeripheral.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.textActivationPeripheral.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.textActivationPeripheral.Location = new System.Drawing.Point(30, 77);
            this.textActivationPeripheral.Name = "textActivationPeripheral";
            this.textActivationPeripheral.Size = new System.Drawing.Size(162, 14);
            this.textActivationPeripheral.TabIndex = 2;
            this.textActivationPeripheral.Text = "Activation";
            //
            //lineSeparator
            //
            this.lineSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lineSeparator.Location = new System.Drawing.Point(10, 0);
            this.lineSeparator.Name = "lineSeparator";
            this.lineSeparator.Size = new System.Drawing.Size(1, 284);
            this.lineSeparator.TabIndex = 54;
            //
            //textNamePeripheral
            //
            this.textNamePeripheral.AutoSize = true;
            this.textNamePeripheral.Font = new System.Drawing.Font("Verdana", (float)(14.0F), System.Drawing.FontStyle.Bold);
            this.textNamePeripheral.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.textNamePeripheral.Location = new System.Drawing.Point(35, 3);
            this.textNamePeripheral.Name = "textNamePeripheral";
            this.textNamePeripheral.Size = new System.Drawing.Size(122, 23);
            this.textNamePeripheral.TabIndex = 0;
            this.textNamePeripheral.Text = "Peripheral";
            //
            //tlpPeripherals
            //
            this.tlpPeripherals.ColumnCount = 5;
            this.tlpPeripherals.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)(100.0F)));
            this.tlpPeripherals.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(35.0F)));
            this.tlpPeripherals.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(35.0F)));
            this.tlpPeripherals.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(35.0F)));
            this.tlpPeripherals.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(35.0F)));
            this.tlpPeripherals.Controls.Add(this.labelPeripheralPort3, 3, 0);
            this.tlpPeripherals.Controls.Add(this.labelPeripheralPort1, 1, 0);
            this.tlpPeripherals.Controls.Add(this.labelPeripheralPort2, 2, 0);
            this.tlpPeripherals.Controls.Add(this.labelPeripheralPort4, 4, 0);
            this.tlpPeripherals.Location = new System.Drawing.Point(15, 15);
            this.tlpPeripherals.Margin = new System.Windows.Forms.Padding(0);
            this.tlpPeripherals.Name = "tlpPeripherals";
            this.tlpPeripherals.RowCount = 1;
            this.tlpPeripherals.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float)(100.0F)));
            this.tlpPeripherals.Size = new System.Drawing.Size(240, 284);
            this.tlpPeripherals.TabIndex = 52;
            this.tlpPeripherals.Visible = false;
            //
            //labelPeripheralPort3
            //
            this.labelPeripheralPort3.AutoSize = true;
            this.labelPeripheralPort3.Font = new System.Drawing.Font("Verdana", (float)(17.75F), System.Drawing.FontStyle.Bold);
            this.labelPeripheralPort3.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelPeripheralPort3.Location = new System.Drawing.Point(173, 0);
            this.labelPeripheralPort3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.labelPeripheralPort3.Name = "labelPeripheralPort3";
            this.labelPeripheralPort3.Size = new System.Drawing.Size(30, 29);
            this.labelPeripheralPort3.TabIndex = 2;
            this.labelPeripheralPort3.Text = "3";
            this.labelPeripheralPort3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelPeripheralPort1
            //
            this.labelPeripheralPort1.AutoSize = true;
            this.labelPeripheralPort1.Font = new System.Drawing.Font("Verdana", (float)(17.75F), System.Drawing.FontStyle.Bold);
            this.labelPeripheralPort1.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelPeripheralPort1.Location = new System.Drawing.Point(103, 0);
            this.labelPeripheralPort1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.labelPeripheralPort1.Name = "labelPeripheralPort1";
            this.labelPeripheralPort1.Size = new System.Drawing.Size(30, 29);
            this.labelPeripheralPort1.TabIndex = 0;
            this.labelPeripheralPort1.Text = "1";
            this.labelPeripheralPort1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelPeripheralPort2
            //
            this.labelPeripheralPort2.AutoSize = true;
            this.labelPeripheralPort2.Font = new System.Drawing.Font("Verdana", (float)(17.75F), System.Drawing.FontStyle.Bold);
            this.labelPeripheralPort2.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelPeripheralPort2.Location = new System.Drawing.Point(138, 0);
            this.labelPeripheralPort2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.labelPeripheralPort2.Name = "labelPeripheralPort2";
            this.labelPeripheralPort2.Size = new System.Drawing.Size(30, 29);
            this.labelPeripheralPort2.TabIndex = 1;
            this.labelPeripheralPort2.Text = "2";
            this.labelPeripheralPort2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelPeripheralPort4
            //
            this.labelPeripheralPort4.AutoSize = true;
            this.labelPeripheralPort4.Font = new System.Drawing.Font("Verdana", (float)(17.75F), System.Drawing.FontStyle.Bold);
            this.labelPeripheralPort4.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelPeripheralPort4.Location = new System.Drawing.Point(208, 0);
            this.labelPeripheralPort4.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.labelPeripheralPort4.Name = "labelPeripheralPort4";
            this.labelPeripheralPort4.Size = new System.Drawing.Size(30, 29);
            this.labelPeripheralPort4.TabIndex = 3;
            this.labelPeripheralPort4.Text = "4";
            this.labelPeripheralPort4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //lblNoPeripheral
            //
            this.lblNoPeripheral.BackColor = System.Drawing.Color.Transparent;
            this.lblNoPeripheral.Font = new System.Drawing.Font("Verdana", (float)(26.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblNoPeripheral.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblNoPeripheral.Location = new System.Drawing.Point(18, 75);
            this.lblNoPeripheral.Margin = new System.Windows.Forms.Padding(0);
            this.lblNoPeripheral.Name = "lblNoPeripheral";
            this.lblNoPeripheral.Size = new System.Drawing.Size(476, 164);
            this.lblNoPeripheral.TabIndex = 50;
            this.lblNoPeripheral.Text = "NO PERIPHERAL FOUND";
            this.lblNoPeripheral.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageWork_HA
            //
            this.pageWork_HA.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageWork_HA.Controls.Add(this.lblToolFlowUnits_HA);
            this.pageWork_HA.Controls.Add(this.lblToolFlow_HA);
            this.pageWork_HA.Controls.Add(this.labWorkPortStatus_HA);
            this.pageWork_HA.Controls.Add(this.lblToolTempUnits_HA);
            this.pageWork_HA.Controls.Add(this.PanelTemps_HA);
            this.pageWork_HA.Controls.Add(this.lblToolTemp_HA);
            this.pageWork_HA.Controls.Add(this.panelPower_HA);
            this.pageWork_HA.Location = new System.Drawing.Point(531, 0);
            this.pageWork_HA.Name = "pageWork_HA";
            this.pageWork_HA.Size = new System.Drawing.Size(512, 314);
            this.pageWork_HA.TabIndex = 11;
            //
            //lblToolFlowUnits_HA
            //
            this.lblToolFlowUnits_HA.BackColor = System.Drawing.Color.Transparent;
            this.lblToolFlowUnits_HA.Font = new System.Drawing.Font("Verdana", (float)(18.0F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolFlowUnits_HA.Location = new System.Drawing.Point(437, 120);
            this.lblToolFlowUnits_HA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblToolFlowUnits_HA.Name = "lblToolFlowUnits_HA";
            this.lblToolFlowUnits_HA.Size = new System.Drawing.Size(63, 37);
            this.lblToolFlowUnits_HA.TabIndex = 59;
            this.lblToolFlowUnits_HA.Text = "%";
            this.lblToolFlowUnits_HA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //lblToolFlow_HA
            //
            this.lblToolFlow_HA.BackColor = System.Drawing.Color.Transparent;
            this.lblToolFlow_HA.Font = new System.Drawing.Font("Verdana", (float)(48.0F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolFlow_HA.Location = new System.Drawing.Point(276, 82);
            this.lblToolFlow_HA.Margin = new System.Windows.Forms.Padding(0);
            this.lblToolFlow_HA.Name = "lblToolFlow_HA";
            this.lblToolFlow_HA.Size = new System.Drawing.Size(178, 84);
            this.lblToolFlow_HA.TabIndex = 58;
            this.lblToolFlow_HA.Text = "80";
            this.lblToolFlow_HA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //labWorkPortStatus_HA
            //
            this.labWorkPortStatus_HA.BackColor = System.Drawing.Color.Transparent;
            this.labWorkPortStatus_HA.Font = new System.Drawing.Font("Verdana", (float)(14.25F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.labWorkPortStatus_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labWorkPortStatus_HA.Location = new System.Drawing.Point(37, 2);
            this.labWorkPortStatus_HA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labWorkPortStatus_HA.Name = "labWorkPortStatus_HA";
            this.labWorkPortStatus_HA.Size = new System.Drawing.Size(445, 30);
            this.labWorkPortStatus_HA.TabIndex = 57;
            this.labWorkPortStatus_HA.Text = "Sleep";
            this.labWorkPortStatus_HA.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //
            //lblToolTempUnits_HA
            //
            this.lblToolTempUnits_HA.BackColor = System.Drawing.Color.Transparent;
            this.lblToolTempUnits_HA.Font = new System.Drawing.Font("Verdana", (float)(18.0F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolTempUnits_HA.Location = new System.Drawing.Point(233, 102);
            this.lblToolTempUnits_HA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblToolTempUnits_HA.Name = "lblToolTempUnits_HA";
            this.lblToolTempUnits_HA.Size = new System.Drawing.Size(63, 37);
            this.lblToolTempUnits_HA.TabIndex = 54;
            this.lblToolTempUnits_HA.Text = "ºC";
            this.lblToolTempUnits_HA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //PanelTemps_HA
            //
            this.PanelTemps_HA.BackColor = System.Drawing.Color.Transparent;
            this.PanelTemps_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.PanelTemps_HA.Location = new System.Drawing.Point(105, 213);
            this.PanelTemps_HA.Name = "PanelTemps_HA";
            this.PanelTemps_HA.Size = new System.Drawing.Size(337, 99);
            this.PanelTemps_HA.TabIndex = 53;
            //
            //lblToolTemp_HA
            //
            this.lblToolTemp_HA.BackColor = System.Drawing.Color.Transparent;
            this.lblToolTemp_HA.Font = new System.Drawing.Font("Verdana", (float)(48.0F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblToolTemp_HA.Location = new System.Drawing.Point(72, 83);
            this.lblToolTemp_HA.Margin = new System.Windows.Forms.Padding(0);
            this.lblToolTemp_HA.Name = "lblToolTemp_HA";
            this.lblToolTemp_HA.Size = new System.Drawing.Size(178, 83);
            this.lblToolTemp_HA.TabIndex = 51;
            this.lblToolTemp_HA.Text = "300";
            this.lblToolTemp_HA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //panelPower_HA
            //
            this.panelPower_HA.BackColor = System.Drawing.Color.Transparent;
            this.panelPower_HA.Controls.Add(this.lblPwr_HA);
            this.panelPower_HA.Controls.Add(this.pictTrackPowerColor_HA);
            this.panelPower_HA.Controls.Add(this.lblTitlePwr_HA);
            this.panelPower_HA.Location = new System.Drawing.Point(6, 50);
            this.panelPower_HA.Name = "panelPower_HA";
            this.panelPower_HA.Size = new System.Drawing.Size(64, 223);
            this.panelPower_HA.TabIndex = 56;
            //
            //lblPwr_HA
            //
            this.lblPwr_HA.BackColor = System.Drawing.Color.Transparent;
            this.lblPwr_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblPwr_HA.Location = new System.Drawing.Point(1, 197);
            this.lblPwr_HA.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPwr_HA.Name = "lblPwr_HA";
            this.lblPwr_HA.Size = new System.Drawing.Size(59, 23);
            this.lblPwr_HA.TabIndex = 38;
            this.lblPwr_HA.Text = "000 %";
            this.lblPwr_HA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pictTrackPowerColor_HA
            //
            this.pictTrackPowerColor_HA.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.pictTrackPowerColor_HA.Image = My.Resources.Resources.power_99;
            this.pictTrackPowerColor_HA.Location = new System.Drawing.Point(20, 19);
            this.pictTrackPowerColor_HA.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictTrackPowerColor_HA.Name = "pictTrackPowerColor_HA";
            this.pictTrackPowerColor_HA.Size = new System.Drawing.Size(21, 162);
            this.pictTrackPowerColor_HA.TabIndex = 18;
            this.pictTrackPowerColor_HA.TabStop = false;
            //
            //lblTitlePwr_HA
            //
            this.lblTitlePwr_HA.BackColor = System.Drawing.Color.Transparent;
            this.lblTitlePwr_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblTitlePwr_HA.Location = new System.Drawing.Point(0, 183);
            this.lblTitlePwr_HA.Name = "lblTitlePwr_HA";
            this.lblTitlePwr_HA.Size = new System.Drawing.Size(61, 14);
            this.lblTitlePwr_HA.TabIndex = 43;
            this.lblTitlePwr_HA.Text = "Power";
            this.lblTitlePwr_HA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageDummy3
            //
            this.pageDummy3.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageDummy3.Controls.Add(this.panelWorkPageProfilePlot);
            this.pageDummy3.Controls.Add(this.labelWorkProfile_HA_Status);
            this.pageDummy3.Controls.Add(this.labelWorkProfile_HA_AirFlow);
            this.pageDummy3.Controls.Add(this.labelWorkProfile_HA_AirFlowTitle);
            this.pageDummy3.Controls.Add(this.labelWorkProfile_HA_ExtTCTemp);
            this.pageDummy3.Controls.Add(this.labelWorkProfile_HA_ExtTCTempTitle);
            this.pageDummy3.Controls.Add(this.labelWorkProfile_HA_HotAirTemp);
            this.pageDummy3.Controls.Add(this.labelWorkProfile_HA_HotAirTempTitle);
            this.pageDummy3.Location = new System.Drawing.Point(0, 1985);
            this.pageDummy3.Name = "pageDummy3";
            this.pageDummy3.Size = new System.Drawing.Size(512, 314);
            this.pageDummy3.TabIndex = 54;
            //
            //panelWorkPageProfilePlot
            //
            CartesianArea1.ShowGrid = true;
            this.panelWorkPageProfilePlot.AreaDesign = CartesianArea1;
            this.panelWorkPageProfilePlot.BackColor = System.Drawing.Color.Transparent;
            this.panelWorkPageProfilePlot.Location = new System.Drawing.Point(0, 37);
            this.panelWorkPageProfilePlot.Margin = new System.Windows.Forms.Padding(0);
            this.panelWorkPageProfilePlot.Name = "panelWorkPageProfilePlot";
            this.panelWorkPageProfilePlot.Size = new System.Drawing.Size(512, 205);
            this.panelWorkPageProfilePlot.TabIndex = 0;
            this.panelWorkPageProfilePlot.Text = "RadChartView1";
            ((Telerik.WinControls.UI.RadChartElement)(this.panelWorkPageProfilePlot.GetChildAt(0))).BorderBoxStyle = Telerik.WinControls.BorderBoxStyle.SingleBorder;
            ((Telerik.WinControls.UI.RadChartElement)(this.panelWorkPageProfilePlot.GetChildAt(0))).BorderWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelWorkPageProfilePlot.GetChildAt(0))).BorderLeftWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelWorkPageProfilePlot.GetChildAt(0))).BorderTopWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelWorkPageProfilePlot.GetChildAt(0))).BorderRightWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelWorkPageProfilePlot.GetChildAt(0))).BorderBottomWidth = (float)(0.0F);
            //
            //labelWorkProfile_HA_Status
            //
            this.labelWorkProfile_HA_Status.Font = new System.Drawing.Font("Verdana", (float)(10.0F));
            this.labelWorkProfile_HA_Status.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelWorkProfile_HA_Status.Location = new System.Drawing.Point(362, 0);
            this.labelWorkProfile_HA_Status.Name = "labelWorkProfile_HA_Status";
            this.labelWorkProfile_HA_Status.Size = new System.Drawing.Size(150, 26);
            this.labelWorkProfile_HA_Status.TabIndex = 25;
            this.labelWorkProfile_HA_Status.Text = "Stop";
            this.labelWorkProfile_HA_Status.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelWorkProfile_HA_AirFlow
            //
            this.labelWorkProfile_HA_AirFlow.Font = new System.Drawing.Font("Verdana", (float)(14.0F), System.Drawing.FontStyle.Bold);
            this.labelWorkProfile_HA_AirFlow.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelWorkProfile_HA_AirFlow.Location = new System.Drawing.Point(350, 277);
            this.labelWorkProfile_HA_AirFlow.Name = "labelWorkProfile_HA_AirFlow";
            this.labelWorkProfile_HA_AirFlow.Size = new System.Drawing.Size(150, 26);
            this.labelWorkProfile_HA_AirFlow.TabIndex = 24;
            this.labelWorkProfile_HA_AirFlow.Text = "--- ºC";
            this.labelWorkProfile_HA_AirFlow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelWorkProfile_HA_AirFlowTitle
            //
            this.labelWorkProfile_HA_AirFlowTitle.Font = new System.Drawing.Font("Verdana", (float)(10.0F));
            this.labelWorkProfile_HA_AirFlowTitle.ForeColor = System.Drawing.Color.RoyalBlue;
            this.labelWorkProfile_HA_AirFlowTitle.Location = new System.Drawing.Point(350, 252);
            this.labelWorkProfile_HA_AirFlowTitle.Name = "labelWorkProfile_HA_AirFlowTitle";
            this.labelWorkProfile_HA_AirFlowTitle.Size = new System.Drawing.Size(150, 20);
            this.labelWorkProfile_HA_AirFlowTitle.TabIndex = 23;
            this.labelWorkProfile_HA_AirFlowTitle.Text = "Air flow";
            this.labelWorkProfile_HA_AirFlowTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelWorkProfile_HA_ExtTCTemp
            //
            this.labelWorkProfile_HA_ExtTCTemp.Font = new System.Drawing.Font("Verdana", (float)(14.0F), System.Drawing.FontStyle.Bold);
            this.labelWorkProfile_HA_ExtTCTemp.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelWorkProfile_HA_ExtTCTemp.Location = new System.Drawing.Point(180, 277);
            this.labelWorkProfile_HA_ExtTCTemp.Name = "labelWorkProfile_HA_ExtTCTemp";
            this.labelWorkProfile_HA_ExtTCTemp.Size = new System.Drawing.Size(150, 26);
            this.labelWorkProfile_HA_ExtTCTemp.TabIndex = 22;
            this.labelWorkProfile_HA_ExtTCTemp.Text = "--- ºC";
            this.labelWorkProfile_HA_ExtTCTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelWorkProfile_HA_ExtTCTempTitle
            //
            this.labelWorkProfile_HA_ExtTCTempTitle.Font = new System.Drawing.Font("Verdana", (float)(10.0F));
            this.labelWorkProfile_HA_ExtTCTempTitle.ForeColor = System.Drawing.Color.DarkGreen;
            this.labelWorkProfile_HA_ExtTCTempTitle.Location = new System.Drawing.Point(180, 252);
            this.labelWorkProfile_HA_ExtTCTempTitle.Name = "labelWorkProfile_HA_ExtTCTempTitle";
            this.labelWorkProfile_HA_ExtTCTempTitle.Size = new System.Drawing.Size(150, 20);
            this.labelWorkProfile_HA_ExtTCTempTitle.TabIndex = 21;
            this.labelWorkProfile_HA_ExtTCTempTitle.Text = "Ext TC temp";
            this.labelWorkProfile_HA_ExtTCTempTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelWorkProfile_HA_HotAirTemp
            //
            this.labelWorkProfile_HA_HotAirTemp.Font = new System.Drawing.Font("Verdana", (float)(14.0F), System.Drawing.FontStyle.Bold);
            this.labelWorkProfile_HA_HotAirTemp.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelWorkProfile_HA_HotAirTemp.Location = new System.Drawing.Point(10, 277);
            this.labelWorkProfile_HA_HotAirTemp.Name = "labelWorkProfile_HA_HotAirTemp";
            this.labelWorkProfile_HA_HotAirTemp.Size = new System.Drawing.Size(150, 26);
            this.labelWorkProfile_HA_HotAirTemp.TabIndex = 20;
            this.labelWorkProfile_HA_HotAirTemp.Text = "--- ºC";
            this.labelWorkProfile_HA_HotAirTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelWorkProfile_HA_HotAirTempTitle
            //
            this.labelWorkProfile_HA_HotAirTempTitle.Font = new System.Drawing.Font("Verdana", (float)(10.0F));
            this.labelWorkProfile_HA_HotAirTempTitle.ForeColor = System.Drawing.Color.DarkRed;
            this.labelWorkProfile_HA_HotAirTempTitle.Location = new System.Drawing.Point(10, 252);
            this.labelWorkProfile_HA_HotAirTempTitle.Name = "labelWorkProfile_HA_HotAirTempTitle";
            this.labelWorkProfile_HA_HotAirTempTitle.Size = new System.Drawing.Size(150, 20);
            this.labelWorkProfile_HA_HotAirTempTitle.TabIndex = 19;
            this.labelWorkProfile_HA_HotAirTempTitle.Text = "Hot air temp";
            this.labelWorkProfile_HA_HotAirTempTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageProfiles
            //
            this.pageProfiles.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageProfiles.Controls.Add(this.panelProfilesParameters);
            this.pageProfiles.Controls.Add(this.panelProfilesOptions);
            this.pageProfiles.Controls.Add(this.lineSeparator2);
            this.pageProfiles.Controls.Add(this.panelProfilesPlot);
            this.pageProfiles.Location = new System.Drawing.Point(531, 2315);
            this.pageProfiles.Name = "pageProfiles";
            this.pageProfiles.Size = new System.Drawing.Size(512, 314);
            this.pageProfiles.TabIndex = 55;
            //
            //panelProfilesParameters
            //
            this.panelProfilesParameters.Location = new System.Drawing.Point(0, 53);
            this.panelProfilesParameters.Name = "panelProfilesParameters";
            this.panelProfilesParameters.Size = new System.Drawing.Size(512, 98);
            this.panelProfilesParameters.TabIndex = 43;
            //
            //panelProfilesOptions
            //
            this.panelProfilesOptions.Location = new System.Drawing.Point(0, 0);
            this.panelProfilesOptions.Name = "panelProfilesOptions";
            this.panelProfilesOptions.Size = new System.Drawing.Size(512, 52);
            this.panelProfilesOptions.TabIndex = 42;
            //
            //lineSeparator2
            //
            this.lineSeparator2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lineSeparator2.Location = new System.Drawing.Point(10, 52);
            this.lineSeparator2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lineSeparator2.Name = "lineSeparator2";
            this.lineSeparator2.Size = new System.Drawing.Size(492, 1);
            this.lineSeparator2.TabIndex = 41;
            //
            //panelProfilesPlot
            //
            CartesianArea2.ShowGrid = true;
            this.panelProfilesPlot.AreaDesign = CartesianArea2;
            this.panelProfilesPlot.BackColor = System.Drawing.Color.Transparent;
            this.panelProfilesPlot.Location = new System.Drawing.Point(0, 151);
            this.panelProfilesPlot.Margin = new System.Windows.Forms.Padding(0);
            this.panelProfilesPlot.Name = "panelProfilesPlot";
            this.panelProfilesPlot.Size = new System.Drawing.Size(512, 152);
            this.panelProfilesPlot.TabIndex = 1;
            this.panelProfilesPlot.Text = "RadChartView1";
            ((Telerik.WinControls.UI.RadChartElement)(this.panelProfilesPlot.GetChildAt(0))).BorderBoxStyle = Telerik.WinControls.BorderBoxStyle.SingleBorder;
            ((Telerik.WinControls.UI.RadChartElement)(this.panelProfilesPlot.GetChildAt(0))).BorderWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelProfilesPlot.GetChildAt(0))).BorderLeftWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelProfilesPlot.GetChildAt(0))).BorderTopWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelProfilesPlot.GetChildAt(0))).BorderRightWidth = (float)(0.0F);
            ((Telerik.WinControls.UI.RadChartElement)(this.panelProfilesPlot.GetChildAt(0))).BorderBottomWidth = (float)(0.0F);
            //
            //pageLoadSaveSettings_HA
            //
            this.pageLoadSaveSettings_HA.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageLoadSaveSettings_HA.Controls.Add(this.butConfSaveProfile_HA);
            this.pageLoadSaveSettings_HA.Controls.Add(this.butConfLoadProfile_HA);
            this.pageLoadSaveSettings_HA.Controls.Add(this.Label1);
            this.pageLoadSaveSettings_HA.Controls.Add(this.butConfSave_HA);
            this.pageLoadSaveSettings_HA.Controls.Add(this.butConfLoad_HA);
            this.pageLoadSaveSettings_HA.Controls.Add(this.lblConfInfo_HA);
            this.pageLoadSaveSettings_HA.Location = new System.Drawing.Point(531, 657);
            this.pageLoadSaveSettings_HA.Name = "pageLoadSaveSettings_HA";
            this.pageLoadSaveSettings_HA.Size = new System.Drawing.Size(512, 314);
            this.pageLoadSaveSettings_HA.TabIndex = 6;
            //
            //butConfSaveProfile_HA
            //
            this.butConfSaveProfile_HA.BackColor = System.Drawing.Color.White;
            this.butConfSaveProfile_HA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butConfSaveProfile_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butConfSaveProfile_HA.Location = new System.Drawing.Point(265, 264);
            this.butConfSaveProfile_HA.Name = "butConfSaveProfile_HA";
            this.butConfSaveProfile_HA.Size = new System.Drawing.Size(165, 26);
            this.butConfSaveProfile_HA.TabIndex = 8;
            this.butConfSaveProfile_HA.Text = "Save to File";
            this.butConfSaveProfile_HA.UseVisualStyleBackColor = false;
            //
            //butConfLoadProfile_HA
            //
            this.butConfLoadProfile_HA.BackColor = System.Drawing.Color.White;
            this.butConfLoadProfile_HA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butConfLoadProfile_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butConfLoadProfile_HA.Location = new System.Drawing.Point(79, 264);
            this.butConfLoadProfile_HA.Name = "butConfLoadProfile_HA";
            this.butConfLoadProfile_HA.Size = new System.Drawing.Size(165, 26);
            this.butConfLoadProfile_HA.TabIndex = 7;
            this.butConfLoadProfile_HA.Text = "Load from File";
            this.butConfLoadProfile_HA.UseVisualStyleBackColor = false;
            //
            //Label1
            //
            this.Label1.BackColor = System.Drawing.Color.Transparent;
            this.Label1.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.Label1.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.Label1.Location = new System.Drawing.Point(68, 174);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(374, 76);
            this.Label1.TabIndex = 6;
            this.Label1.Text = "Load a profile file for the station or save station profiles.";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //butConfSave_HA
            //
            this.butConfSave_HA.BackColor = System.Drawing.Color.White;
            this.butConfSave_HA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butConfSave_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butConfSave_HA.Location = new System.Drawing.Point(266, 108);
            this.butConfSave_HA.Name = "butConfSave_HA";
            this.butConfSave_HA.Size = new System.Drawing.Size(165, 26);
            this.butConfSave_HA.TabIndex = 5;
            this.butConfSave_HA.Text = "Save to File";
            this.butConfSave_HA.UseVisualStyleBackColor = false;
            //
            //butConfLoad_HA
            //
            this.butConfLoad_HA.BackColor = System.Drawing.Color.White;
            this.butConfLoad_HA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butConfLoad_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butConfLoad_HA.Location = new System.Drawing.Point(80, 108);
            this.butConfLoad_HA.Name = "butConfLoad_HA";
            this.butConfLoad_HA.Size = new System.Drawing.Size(165, 26);
            this.butConfLoad_HA.TabIndex = 4;
            this.butConfLoad_HA.Text = "Load from File";
            this.butConfLoad_HA.UseVisualStyleBackColor = false;
            //
            //lblConfInfo_HA
            //
            this.lblConfInfo_HA.BackColor = System.Drawing.Color.Transparent;
            this.lblConfInfo_HA.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblConfInfo_HA.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblConfInfo_HA.Location = new System.Drawing.Point(69, 18);
            this.lblConfInfo_HA.Name = "lblConfInfo_HA";
            this.lblConfInfo_HA.Size = new System.Drawing.Size(374, 76);
            this.lblConfInfo_HA.TabIndex = 3;
            this.lblConfInfo_HA.Text = "Load a configuration file for the station or save the current station configurati" +
                "on.";
            this.lblConfInfo_HA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //TabPanels
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pageLoadSaveSettings_HA);
            this.Controls.Add(this.pageProfiles);
            this.Controls.Add(this.pageDummy3);
            this.Controls.Add(this.pageWork_HA);
            this.Controls.Add(this.pagePeripheral);
            this.Controls.Add(this.pageDummy2);
            this.Controls.Add(this.pageGraphics);
            this.Controls.Add(this.pageDummy);
            this.Controls.Add(this.pageInfo);
            this.Controls.Add(this.pageCounters);
            this.Controls.Add(this.pageResetSettings);
            this.Controls.Add(this.pageLoadSaveSettings);
            this.Controls.Add(this.pageToolSettings);
            this.Controls.Add(this.pageStationSettings);
            this.Controls.Add(this.pageWork);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "TabPanels";
            this.Size = new System.Drawing.Size(1046, 2700);
            this.pageWork.ResumeLayout(false);
            this.panelWorkPeriphStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pcbTool).EndInit();
            this.panelPower.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pictTrackPowerColor).EndInit();
            this.pageStationSettings.ResumeLayout(false);
            this.pageToolSettings.ResumeLayout(false);
            this.TableLayoutPanelTools.ResumeLayout(false);
            this.TableLayoutPanelPortsTools.ResumeLayout(false);
            this.pageLoadSaveSettings.ResumeLayout(false);
            this.pageResetSettings.ResumeLayout(false);
            this.pageCounters.ResumeLayout(false);
            this.pageCounters.PerformLayout();
            this.TableLayoutPanelPortsCounters.ResumeLayout(false);
            this.pageDummy.ResumeLayout(false);
            this.pageGraphics.ResumeLayout(false);
            this.pageDummy2.ResumeLayout(false);
            this.panelWorkSleep.ResumeLayout(false);
            this.pagePeripheral.ResumeLayout(false);
            this.panelConfigPeripheral.ResumeLayout(false);
            this.panelConfigPeripheral.PerformLayout();
            this.panelConfigParamPeripheral.ResumeLayout(false);
            this.panelConfigParamPeripheral.PerformLayout();
            this.tlpPeripherals.ResumeLayout(false);
            this.tlpPeripherals.PerformLayout();
            this.pageWork_HA.ResumeLayout(false);
            this.panelPower_HA.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pictTrackPowerColor_HA).EndInit();
            this.pageDummy3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.panelWorkPageProfilePlot).EndInit();
            this.pageProfiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.panelProfilesPlot).EndInit();
            this.pageLoadSaveSettings_HA.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Panel pageWork;
        internal System.Windows.Forms.Panel pageStationSettings;
        internal System.Windows.Forms.Label lblToolTempUnits;
        internal System.Windows.Forms.Label lblTitlePwr;
        internal System.Windows.Forms.PictureBox pictTrackPowerColor;
        internal System.Windows.Forms.Panel PanelTemps;
        internal System.Windows.Forms.PictureBox pcbTool;
        internal System.Windows.Forms.Label lblToolTemp;
        internal System.Windows.Forms.Label lblPwr;
        internal System.Windows.Forms.Panel pageToolSettings;
        internal System.Windows.Forms.Label lblSelectedTool;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanelTools;
        internal System.Windows.Forms.RadioButton rbToolSettings_T5;
        internal System.Windows.Forms.RadioButton rbToolSettings_T6;
        internal System.Windows.Forms.RadioButton rbToolSettings_T4;
        internal System.Windows.Forms.RadioButton rbToolSettings_T1;
        internal System.Windows.Forms.RadioButton rbToolSettings_T3;
        internal System.Windows.Forms.RadioButton rbToolSettings_T2;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanelPortsTools;
        internal System.Windows.Forms.RadioButton rbPortTools_1;
        internal System.Windows.Forms.RadioButton rbPortTools_2;
        internal System.Windows.Forms.RadioButton rbPortTools_3;
        internal System.Windows.Forms.RadioButton rbPortTools_4;
        internal System.Windows.Forms.Panel pageLoadSaveSettings;
        internal System.Windows.Forms.Button butConfSave;
        internal System.Windows.Forms.Button butConfLoad;
        internal System.Windows.Forms.Label lblConfInfo;
        internal System.Windows.Forms.Panel pageResetSettings;
        internal System.Windows.Forms.ProgressBar pgbResetBar;
        internal System.Windows.Forms.Button butResetProceed;
        internal System.Windows.Forms.Label lblResetInfo;
        internal System.Windows.Forms.Panel pageCounters;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanelPortsCounters;
        internal System.Windows.Forms.RadioButton rbPortCounters_1;
        internal System.Windows.Forms.RadioButton rbPortCounters_2;
        internal System.Windows.Forms.RadioButton rbPortCounters_3;
        internal System.Windows.Forms.RadioButton rbPortCounters_4;
        internal System.Windows.Forms.Panel pageInfo;
        internal System.Windows.Forms.Button butPort;
        internal System.Windows.Forms.Panel pageDummy;
        internal System.Windows.Forms.Label lblNoTool;
        internal System.Windows.Forms.Label lblToolNeeded;
        internal System.Windows.Forms.Panel panelPower;
        internal System.Windows.Forms.Panel pageGraphics;
        internal System.Windows.Forms.Panel pageDummy2;
        internal System.Windows.Forms.Panel panelWorkSleep;
        internal System.Windows.Forms.Label labWorkSleepStatus;
        internal System.Windows.Forms.Label labWorkSleepDelay;
        internal System.Windows.Forms.Label labWorkSleepStand;
        internal System.Windows.Forms.Label labWorkSleepTemp;
        internal System.Windows.Forms.ComboBox cbxGraphPlots;
        internal System.Windows.Forms.Label lblGraphAddToPlot;
        internal System.Windows.Forms.Button butGraphAddSeries;
        internal System.Windows.Forms.Label lblGraphInfo;
        internal System.Windows.Forms.RadioButton rbToolSettings_T7;
        internal System.Windows.Forms.RadioButton rbToolSettings_T8;
        internal System.Windows.Forms.RadioButton rbPartialCounters;
        internal System.Windows.Forms.RadioButton rbGlobalCounters;
        internal System.Windows.Forms.Button butResetPartialCounters;
        internal System.Windows.Forms.Panel pagePeripheral;
        internal System.Windows.Forms.Label lblNoPeripheral;
        internal System.Windows.Forms.TableLayoutPanel tlpPeripherals;
        internal System.Windows.Forms.Label labelPeripheralPort2;
        internal System.Windows.Forms.Label labelPeripheralPort3;
        internal System.Windows.Forms.Label labelPeripheralPort4;
        internal System.Windows.Forms.Label labelPeripheralPort1;
        internal System.Windows.Forms.Panel panelConfigPeripheral;
        internal System.Windows.Forms.Label textFunctionPeripheral;
        internal System.Windows.Forms.Label textNamePeripheral;
        internal System.Windows.Forms.ComboBox inputActivationPeripheral;
        internal System.Windows.Forms.ComboBox inputFunctionPeripheral;
        internal System.Windows.Forms.Label textTimePeripheral;
        internal System.Windows.Forms.Label textActivationPeripheral;
        internal System.Windows.Forms.Label lineSeparator;
        internal System.Windows.Forms.TextBox inputTimePeripheral;
        internal System.Windows.Forms.Panel panelConfigParamPeripheral;
        internal System.Windows.Forms.Label textTypePeripheral;
        internal System.Windows.Forms.Label inputErrorTimePeripheral;
        internal System.Windows.Forms.Panel panelWorkPeriphStatus;
        internal System.Windows.Forms.Label txtStatusPeriph_1;
        internal System.Windows.Forms.Label txtStatusPeriph_2;
        internal System.Windows.Forms.Label lblNoPeripheralSupported;
        internal System.Windows.Forms.RadioButton rbEthernetConf;
        internal System.Windows.Forms.RadioButton rbGeneralSettings;
        internal System.Windows.Forms.RadioButton rbRobotConf;
        internal System.Windows.Forms.Panel panelStationSettings;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.Panel pageWork_HA;
        internal System.Windows.Forms.Label lblToolFlowUnits_HA;
        internal System.Windows.Forms.Label lblToolFlow_HA;
        internal System.Windows.Forms.Label labWorkPortStatus_HA;
        internal System.Windows.Forms.Label lblToolTempUnits_HA;
        internal System.Windows.Forms.Panel PanelTemps_HA;
        internal System.Windows.Forms.Label lblToolTemp_HA;
        internal System.Windows.Forms.Panel panelPower_HA;
        internal System.Windows.Forms.Label lblPwr_HA;
        internal System.Windows.Forms.PictureBox pictTrackPowerColor_HA;
        internal System.Windows.Forms.Label lblTitlePwr_HA;
        internal System.Windows.Forms.Label lblToolTempAdjust;
        internal System.Windows.Forms.Panel pageDummy3;
        internal Telerik.WinControls.UI.RadChartView panelWorkPageProfilePlot;
        internal System.Windows.Forms.Label labelWorkProfile_HA_HotAirTemp;
        internal System.Windows.Forms.Label labelWorkProfile_HA_HotAirTempTitle;
        internal System.Windows.Forms.Label labelWorkProfile_HA_AirFlow;
        internal System.Windows.Forms.Label labelWorkProfile_HA_AirFlowTitle;
        internal System.Windows.Forms.Label labelWorkProfile_HA_ExtTCTemp;
        internal System.Windows.Forms.Label labelWorkProfile_HA_ExtTCTempTitle;
        internal System.Windows.Forms.Label labelWorkProfile_HA_Status;
        internal System.Windows.Forms.Panel pageProfiles;
        internal Telerik.WinControls.UI.RadChartView panelProfilesPlot;
        internal System.Windows.Forms.Label lineSeparator2;
        internal System.Windows.Forms.Panel panelProfilesOptions;
        internal System.Windows.Forms.Panel panelProfilesParameters;
        internal System.Windows.Forms.Panel pageLoadSaveSettings_HA;
        internal System.Windows.Forms.Button butConfSave_HA;
        internal System.Windows.Forms.Button butConfLoad_HA;
        internal System.Windows.Forms.Label lblConfInfo_HA;
        internal System.Windows.Forms.Button butConfSaveProfile_HA;
        internal System.Windows.Forms.Button butConfLoadProfile_HA;
        internal System.Windows.Forms.Label Label1;

    }
}
