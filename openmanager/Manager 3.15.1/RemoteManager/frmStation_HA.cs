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

using System.Net;
//using JBC_API_Remote;
//using RemoteManRegister.ManRegister;
using Telerik.WinControls.UI;
using Telerik.Charting;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Threading;
using JBC_ConnectRemote;
using DataJBC;
using RemoteManRegister;


// 01/10/2013 Para mostrar el porcentaje de potencia se divide con \ (en lugar de /) truncando
//            (al llegar 999 muestra 100% porque / redondea)
//            pwr = jbc.GetPortToolActualPower(myID, Port) \ 10
// 05/12/2013 Se suma o resta 10 al modificar temperatura en Fahrenheit
// 10/12/2013 Al visualizar en Fahrenheit, se cambian los botones a -10 y +10
// 10/12/2013 Se quitan datos del tab de información: MOS Error Temp y Trafo Error Temp
// 20/12/2013 Se modifican los límites de temperatura máximos, según el modelo de estación
// 13/01/2014 Se quita SetStationPowerLimit de la API y del Manager (se mantiene en el protocolo) label: stnPwrLimitId
// 25/02/2014 Se añade el manejo de protocolos (sProtocol)
// 19/07/2014 Tool settings para DME_TCH_01: mismos settings para todas las herramientas de un puerto
// 17/02/2015 Se quita jbc_TransactionFinished, hasta que implementemos callbacks en WCF. Se añade un timer tmrQueryTransaction
// 24/07/2015 el texto de status debe ser Hibernación, cuando está en Extractor
// 16/08/2016 Versión para Hot Air Desoldering station JTSE


namespace RemoteManager
{
    public partial class frmStation_HA
    {

        #region Default Instance

        //private static frmStation_HA defaultInstance;

        ///// <summary>
        ///// Added by the VB.Net to C# Converter to support default instance behavour in C#
        ///// </summary>
        //public static frmStation_HA Default
        //{
        //    get
        //    {
        //        if (defaultInstance == null)
        //        {
        //            defaultInstance = new frmStation_HA();
        //            defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
        //        }

        //        return defaultInstance;
        //    }
        //    set
        //    {
        //        defaultInstance = value;
        //    }
        //}

        //static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    defaultInstance = null;
        //}

        #endregion

        private JBC_API_Remote jbc;
        private ManRegister reg;

        public System.Windows.Forms.Timer tmr;
        private long myID;
        private ListViewItem myItem;
        // timer para transaction
        public System.Windows.Forms.Timer tmrQueryTransaction;
        private System.Int32 iCountQueryTransactionTicks = 0;
        private System.Int32 TICK_QUERY_TRANSACTION = 1000;
        private const int MAX_TICKS_QUERY_TRANSACTION = 3;

        public TabPanels tabPages;

        private Port Port = Port.NUM_1;

        public const int MONITOR_MODE = 0;
        public const int CONTROL_MODE = 1;
        public int mode = MONITOR_MODE;


        // New and Changed Features between model/versions
        private CFeaturesData features;

        // Shared controls between pages
        // NO HAY

        #region Public Events
        public delegate void PlotStationEventHandler(long stationID, int frmID); // frmID = -1 -> new window
        private PlotStationEventHandler PlotStationEvent;

        public event PlotStationEventHandler PlotStation
        {
            add
            {
                PlotStationEvent = (PlotStationEventHandler)System.Delegate.Combine(PlotStationEvent, value);
            }
            remove
            {
                PlotStationEvent = (PlotStationEventHandler)System.Delegate.Remove(PlotStationEvent, value);
            }
        }

        public delegate void StationNameChangedEventHandler(long stationID, string sStationNewName);
        private StationNameChangedEventHandler StationNameChangedEvent;

        public event StationNameChangedEventHandler StationNameChanged
        {
            add
            {
                StationNameChangedEvent = (StationNameChangedEventHandler)System.Delegate.Combine(StationNameChangedEvent, value);
            }
            remove
            {
                StationNameChangedEvent = (StationNameChangedEventHandler)System.Delegate.Remove(StationNameChangedEvent, value);
            }
        }


        #endregion

        public frmStation_HA(JBC_API_Remote jbcRef, ManRegister regRef, long ID, ListViewItem item)
        {

            // Llamada necesaria para el diseñador.
            InitializeComponent();

            //Added to support default instance behavour in C#
            //if (defaultInstance == null)
            //    defaultInstance = this;

            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;

            // initial control mode
            //If CType(item.Tag, tStationDataItemList).bControlMode Then mode = ControlModeConnection.CONTROL Else mode = ControlModeConnection.MONITOR

            // Setting the jbc api object, ID and node
            jbc = jbcRef;
            reg = regRef;
            myID = ID;
            myItem = item;

            // New and Changed Features between model/versions
            features = jbc.GetStationFeatures(myID);

            //Ocultar el icono de periféricos
            if (!features.Peripherals)
            {
                tlpTabs.Controls.Remove(iconPeripheral);
                tlpTabs.SetCellPosition(iconLoadSaveSettings, new TableLayoutPanelCellPosition(System.Convert.ToInt32(tlpTabs.GetCellPosition(iconLoadSaveSettings).Column - 1), 0));
                tlpTabs.SetCellPosition(iconResetSettings, new TableLayoutPanelCellPosition(System.Convert.ToInt32(tlpTabs.GetCellPosition(iconResetSettings).Column - 1), 0));
                tlpTabs.SetCellPosition(iconCounters, new TableLayoutPanelCellPosition(System.Convert.ToInt32(tlpTabs.GetCellPosition(iconCounters).Column - 1), 0));
                tlpTabs.SetCellPosition(iconInfo, new TableLayoutPanelCellPosition(System.Convert.ToInt32(tlpTabs.GetCellPosition(iconInfo).Column - 1), 0));
                tlpTabs.SetCellPosition(iconGraphics, new TableLayoutPanelCellPosition(System.Convert.ToInt32(tlpTabs.GetCellPosition(iconGraphics).Column - 1), 0));
            }
        }

        public void frmStationParams_Load(object sender, System.EventArgs e)
        {

            tabPages = new TabPanels("pageWork_HA", jbc.GetStationTools(myID));
            tabPages.work_ClickControl += onWorkClickControl;
            tabPages.stationSettings_CheckedChanged += onStationSettings_CheckedChanged;
            tabPages.toolSettingsPort_CheckedChanged += onToolSettingsPort_CheckedChanged;
            tabPages.toolSettingsTool_CheckedChanged += onToolSettingsTool_CheckedChanged;
            tabPages.conf_ClickControl += OnConf_ClickControl;
            tabPages.reset_ClickControl += onReset_ClickControl;
            tabPages.countersPort_CheckedChanged += onCountersPort_CheckedChanged;
            tabPages.countersType_CheckedChanged += onCountersType_CheckedChanged;
            tabPages.countersResetPortPartialCounters_Click += onCountersResetPartial_Click;
            tabPages.graphPlots_DropDown += onGraphPlots_DropDown;
            tabPages.graphAddSeries_Click += onGraphAddSeries_Click;
            tabPages.Top = tlpTabs.Bottom;
            tabPages.Left = 1;
            tabPages.Size = new Size(511, 300);
            this.Controls.Add(tabPages);
            iconWork.Checked = true;

            // Setting the initial control mode
            Configuration.paintFormStationControlMode(myID, myItem);

            createWorkPage();
            createStationSettingsPage();
            createToolSettingsPage();
            CreateProfilesPage();
            createLoadAndSavePage();
            createResetSettingsPage();
            createCountersPage();
            createInfoPage();
            createGraphicsPage();

            //Setting the name and the text
            this.Name = "frmStation" + myID.ToString();
            this.Text = "Station - " + myID.ToString() + " - ";

            //Setting the timer to do a first refresh in 500ms as long as the station
            //is not yet ready to be asked for data. THIS IS A DLL BUG
            tmr = new System.Windows.Forms.Timer();
            tmr.Tick += tmr_Tick;
            //#edu#
            tmr.Interval = 500;
            //tmr.Interval = 3000
            tmr.Tag = true;
            tmr.Start();

            tmrQueryTransaction = new System.Windows.Forms.Timer();
            tmrQueryTransaction.Tick += tmrQueryTransaction_Tick;
            tmrQueryTransaction.Interval = TICK_QUERY_TRANSACTION;
            tmrQueryTransaction.Stop();
        }

        public void frmStation_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            // do not destroy, only hide
            this.Hide();
            e.Cancel = true;
        }

        public void ReLoadTexts()
        {
            loadTextsWorkPage();
            loadTextsStationSettingsPage();
            loadTextsToolSettingsPage();
            loadTextsLoadAndSavePage();
            loadTextsResetSettingsPage();
            loadTextsCountersPage();
            loadTextsInfoPage();
            loadTextsGraphicsPage();
        }

        public void RefreshSettingsPages(bool bOnlyCurrent = false)
        {
            // force to refresh settings parameters (for example, when updated station out of this windows)
            if (!bOnlyCurrent)
            {
                // force update
                WorkParams = true;
                m_updateStationSettingsParams = true;
                m_toolSettingsParams = true;
                counterParams = true;
                infoParams = true;
            }
            else
            {
                switch (tabPages.CurrentPage)
                {
                    case "pageStationSettings":
                        m_updateStationSettingsParams = true; // force update
                        break;
                    case "pageToolSettings":
                        m_toolSettingsParams = true; // force update
                        break;
                }
            }
        }

        #region TABS
        public void icon_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (rb.Checked)
            {
                rb.BackgroundImageLayout = ImageLayout.Stretch;
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(rb.Name + "_s");

                switch (rb.Name)
                {
                    case "iconWork":
                        //changeha
                        tabPages.CurrentPage = "pageWork_HA";
                        break;
                    case "iconStationSettings":
                        m_updateStationSettingsParams = true; // force update
                        tabPages.CurrentPage = "pageStationSettings";
                        break;
                    case "iconToolSettings":
                        m_toolSettingsParams = true; // force update
                        tabPages.CurrentPage = "pageToolSettings";
                        break;
                    case "iconProfile":
                        tabPages.CurrentPage = "pageProfiles";
                        break;
                    case "iconLoadSaveSettings":
                        tabPages.CurrentPage = "pageLoadSaveSettings_HA";
                        break;
                    case "iconResetSettings":
                        tabPages.CurrentPage = "pageResetSettings";
                        break;
                    case "iconCounters":
                        tabPages.CurrentPage = "pageCounters";
                        break;
                    case "iconInfo":
                        tabPages.CurrentPage = "pageInfo";
                        break;
                    case "iconGraphics":
                        tabPages.CurrentPage = "pageGraphics";
                        break;
                }
            }
            else
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(rb.Name);
            }
        }

        public void icon_MouseEnter(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.BackgroundImageLayout = ImageLayout.Stretch;
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(rb.Name + "_s");
            }
        }

        public void icon_MouseLeave(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(rb.Name);
            }
        }

        #endregion

        #region MODE
        private void setControlMode(ControlModeConnection desMode)
        {
            // Setting the control mode
            if (desMode == ControlModeConnection.CONTROL)
            {
                Configuration.setStationControlMode(jbc, myID, ref myItem, true);
            }
            else
            {
                Configuration.setStationControlMode(jbc, myID, ref myItem, false);
            }
        }

        public async void cbMode_Click(System.Object sender, System.EventArgs e)
        {

            // comprobar si el robot tiene el control
            if (jbc.GetControlMode(myID) == ControlModeConnection.ROBOT)
            {
                MsgBoxResult responseRbt = Interaction.MsgBox(Localization.getResStr(Configuration.modeRobotLooseControlWarningId), MsgBoxStyle.OkCancel, null);

                if (responseRbt == MsgBoxResult.Ok)
                {
                    CRobotData rbtData = new CRobotData();
                    rbtData.Status = OnOff._OFF;
                    rbtData.Protocol = (CRobotData.RobotProtocol)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtProtocolId));
                    rbtData.Address = System.Convert.ToUInt16(m_robotParamsTable.getValue(Configuration.stnRbtAddressId));
                    rbtData.Speed = (CRobotData.RobotSpeed)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtSpeedId));
                    rbtData.DataBits = System.Convert.ToUInt16(m_robotParamsTable.getValue(Configuration.stnRbtDataBitsId));
                    rbtData.StopBits = (CRobotData.RobotStop)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtStopBitsId));
                    rbtData.Parity = (CRobotData.RobotParity)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtParityId));

                    await jbc.SetRobotConfigurationAsync(myID, rbtData);

                    //Forcing an update
                    m_updateStationSettingsParams = true;
                }
                else
                {
                    return;
                }

                //comprobar si otra estación tiene el control
            }
            else if (jbc.GetControlMode(myID) == ControlModeConnection.BLOCK_CONTROL)
            {
                cbMode.Enabled = false;
                cbMode.Checked = !cbMode.Checked;
                cbMode.Enabled = true;

                MessageBox.Show(Localization.getResStr(Configuration.modeControlWarningId) + jbc.GetControlModeUserName(myID));

                return;
            }

            if (mode == (int)ControlModeConnection.MONITOR)
            {
                setControlMode(ControlModeConnection.CONTROL);
            }
            else
            {
                setControlMode(ControlModeConnection.MONITOR);
            }
        }

        #endregion

        #region TICK

        private object LockTimer = new object();
        // VBConversions Note: Former VB static variables moved to class level because they aren't supported in C#.
        private long tmr_Tick_minuteCounter = 0;
        private long tmr_Tick_sec3Counter = 0;

        private void tmr_Tick(object sender, System.EventArgs e)
        {
            lock (LockTimer)
            {
                // static long minuteCounter = 0; VBConversions Note: Static variable moved to class level and renamed tmr_Tick_minuteCounter. Local static variables are not supported in C#.
                // static long sec3Counter = 0; VBConversions Note: Static variable moved to class level and renamed tmr_Tick_sec3Counter. Local static variables are not supported in C#.

                //#edu#
                tmr.Stop();

                jbc.UpdateStationStatusAsync(myID);

                //#edu#
                var model = myItem.SubItems[Configuration.subItemModelId].Text;
                var name = myItem.Text;
                // Updating the station window title
                if (this.Text != model + " - " + name)
                {
                    this.Text = model + " - " + name;
                }

                //Checking minute counter time
                if (tmr_Tick_minuteCounter >= (double)60000 / tmr.Interval)
                {
                    RefreshSettingsPages(false); // false = all pages
                    tmr_Tick_minuteCounter = 0;
                }

                //Checking 3 seconds counter time
                if (tmr_Tick_sec3Counter >= (double)1500 / tmr.Interval)
                {
                    WorkParams = true;
                    m_updateStationSettingsParams = true;
                    tmr_Tick_sec3Counter = 0;

                    if (jbc.GetControlMode(myID) == ControlModeConnection.CONTROL)
                    {
                        mode = (int)ControlModeConnection.CONTROL;
                        cbMode.Text = Localization.getResStr(Configuration.modeControlModeId);
                        cbMode.Checked = false;

                        Configuration.tStationDataItemList dataItemList = new Configuration.tStationDataItemList();
                        dataItemList = (Configuration.tStationDataItemList)myItem.Tag;
                        dataItemList.bControlMode = true;
                        myItem.Tag = dataItemList;
                        myItem.ImageKey = "Station_unlock";
                        Configuration.paintFormStationControlMode(myID, myItem);

                        //Refresh station control
                        jbc.KeepControlMode(myID);
                    }
                    else
                    {
                        mode = (int)ControlModeConnection.MONITOR;
                        cbMode.Text = Localization.getResStr(Configuration.modeMonitorModeId);
                        cbMode.Checked = true;

                        Configuration.tStationDataItemList dataItemList = new Configuration.tStationDataItemList();
                        dataItemList = (Configuration.tStationDataItemList)myItem.Tag;
                        dataItemList.bControlMode = false;
                        myItem.Tag = dataItemList;
                        myItem.ImageKey = "Station_lock";
                        Configuration.paintFormStationControlMode(myID, myItem);
                    }

                }

                // Updating the page if selected
                switch (tabPages.CurrentPage)
                {
                    case "pageWork_HA":
                        updateWorkPage();
                        break;
                    case "pageStationSettings":
                        updateStationSettingsPage();
                        break;
                    case "pageToolSettings":
                        updateToolSettingsPage();
                        break;
                    case "pageProfiles":
                        updateProfilesPage();
                        break;
                    case "pageResetSettings":
                        updateResetSettingsPage();
                        break;
                    case "pageCounters":
                        updateCountersPage();
                        break;
                    case "pageInfo":
                        updateInfoPage();
                        break;
                }

                // CHECKEAR ERROR ESTACION

                //Incrementing the counters
                tmr_Tick_minuteCounter++;
                tmr_Tick_sec3Counter++;

                // Depending on the tag value reseting the timer
                if ((bool) tmr.Tag)
                {
                    tmr.Start();
                }
                else
                {
                    tmr.Stop();
                }

            }
        }

        #endregion

        #region WORK PAGE
        // Global var's
        private bool WorkParams = false;

        // Work page controls definitions
        private TempPanels_HA tempPages;

        //Plot recorder profile
        private PlotRecorderProfile_HA m_plotRecorderProfile_HA = null;

        // Creates the work page controls
        private void createWorkPage()
        {

            tempPages = new TempPanels_HA("");
            tempPages.ClickControl += onClickControlTemp;
            tabPages.PanelTemps_HA.Controls.Add(tempPages);
            loadTextsWorkPage();
            myResetPowerTrack();

            // Updating the work parametters
            WorkParams = true;
        }

        private void loadTextsWorkPage()
        {

            tabPages.lblTitlePwr_HA.Text = Localization.getResStr(Configuration.workPowerId);
            //tempPages.lblDesTemp.Text = getResStr(workTempSelectionId)
            tempPages.lblTitleLevel.Text = Localization.getResStr(Configuration.workTempLevelsId);
            //tempPages.lblTitleFixed.Text = getResStr(workTempFixedId)
            if (Configuration.Tunits == Configuration.FAHRENHEIT_STR)
            {
                tempPages.pcbSubstract.Image = My.Resources.Resources.butminus10;
                tempPages.pcbAdd.Image = My.Resources.Resources.butplus10;
            }
            else
            {
                tempPages.pcbSubstract.Image = My.Resources.Resources.butminus5;
                tempPages.pcbAdd.Image = My.Resources.Resources.butplus5;
            }
            if (mode == (int)ControlModeConnection.CONTROL)
            {
                cbMode.Text = Localization.getResStr(Configuration.modeControlModeId);
            }
            else if (mode == (int)ControlModeConnection.MONITOR)
            {
                cbMode.Text = Localization.getResStr(Configuration.modeMonitorModeId);
            }
            ToolTipIcons.SetToolTip(iconWork, Localization.getResStr(Configuration.workTabHintId));
        }

        // selected temp and levels from work page ----------------------------------------
        private async void onClickControlTemp(string controlName)
        {
            CTemperature temp = default(CTemperature);
            ToolTemperatureLevels lvl = default(ToolTemperatureLevels);
            CTemperature auxLimitTemp = default(CTemperature);
            CTemperature auxStationLimitTemp = default(CTemperature);
            int flow = 0;
            int auxStationLimitFlow = 0;


            switch (controlName)
            {
                case "pcbAdd":
                    // change selected temp from work page
                    temp = jbc.GetPortToolSelectedTemp(myID, Port);
                    auxLimitTemp = jbc.GetStationFeatures(myID).MaxTemp;
                    auxStationLimitTemp = jbc.GetStationMaxTemp(myID);
                    Configuration.stepTemp(temp, 1);
                    if ((temp.UTI > auxLimitTemp.UTI) || (temp.UTI > auxStationLimitTemp.UTI & auxStationLimitTemp.UTI > 0))
                    {
                        return;
                    }
                    await jbc.SetPortToolSelectedTempAsync(myID, Port, temp);
                    break;

                case "pcbSubstract":
                    // change selected temp from work page
                    temp = jbc.GetPortToolSelectedTemp(myID, Port);
                    auxLimitTemp = jbc.GetStationFeatures(myID).MinTemp;
                    auxStationLimitTemp = jbc.GetStationMinTemp(myID);
                    Configuration.stepTemp(temp, -1);
                    if ((temp.UTI < auxLimitTemp.UTI) || (temp.UTI < auxStationLimitTemp.UTI & auxStationLimitTemp.UTI > 0))
                    {
                        return;
                    }
                    await jbc.SetPortToolSelectedTempAsync(myID, Port, temp);
                    break;

                case "pcbAddFlow":
                    // change selected flow from work page
                    flow = System.Convert.ToInt32(jbc.GetPortToolSelectedFlow(myID, Port) / 10);
                    auxStationLimitFlow = System.Convert.ToInt32(jbc.GetStationMaxFlow(myID) / 10);
                    flow++;
                    if (flow > auxStationLimitFlow & auxStationLimitFlow > 0)
                    {
                        return;
                    }
                    await jbc.SetPortToolSelectedFlowAsync(myID, Port, flow * 10);
                    break;

                case "pcbSubstractFlow":
                    // change selected flow from work page
                    flow = System.Convert.ToInt32(jbc.GetPortToolSelectedFlow(myID, Port) / 10);
                    auxStationLimitFlow = System.Convert.ToInt32(jbc.GetStationMinFlow(myID) / 10);
                    flow--;
                    if (flow < auxStationLimitFlow & auxStationLimitFlow > 0)
                    {
                        return;
                    }
                    await jbc.SetPortToolSelectedFlowAsync(myID, Port, flow * 10);
                    break;

                case "lblLvl1":
                    // change temp level from work page
                    lvl = ToolTemperatureLevels.FIRST_LEVEL;
                    await jbc.SetPortToolSelectedTempLevelsAsync(myID, Port, jbc.GetPortToolID(myID, Port), lvl);
                    m_toolSettingsParams = true; // update tool params page also
                    break;

                case "lblLvl2":
                    // change temp level from work page
                    lvl = ToolTemperatureLevels.SECOND_LEVEL;
                    await jbc.SetPortToolSelectedTempLevelsAsync(myID, Port, jbc.GetPortToolID(myID, Port), lvl);
                    m_toolSettingsParams = true; // update tool params page also
                    break;

                case "lblLvl3":
                    // change temp level from work page
                    lvl = ToolTemperatureLevels.THIRD_LEVEL;
                    await jbc.SetPortToolSelectedTempLevelsAsync(myID, Port, jbc.GetPortToolID(myID, Port), lvl);
                    m_toolSettingsParams = true; // update tool params page also
                    break;

            }
            WorkParams = true;

        }

        // Port selection ----------------------------------------------------------------------

        private void onWorkClickControl(Control ctrl)
        {

        }

        #region Power Scale
        private void mySetPowerScale(int iPercent)
        {
            int iPower = iPercent;
            if (iPower < 99)
            {
                iPower = System.Convert.ToInt32((iPower / 5) * 5);
            }
            else
            {
                iPower = 99;
            }
            //changeha
            tabPages.pictTrackPowerColor_HA.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("power_" + iPower.ToString());
        }

        private void myResetPowerTrack()
        {
            mySetPowerScale(0);
        }
        #endregion

        private async void updateWorkPage()
        {

            // Checking if the station is initialized
            if (jbc.GetPortCount(myID) <= 0)
            {
                return;
            }

            // Retrieve remote data
            await jbc.UpdatePortStatusAsync(myID, Port);
            await jbc.UpdateProfilesAsync(myID);
            await jbc.UpdateSelectedProfileAsync(myID);

            //Control/Monitor mode
            if (mode == (int)ControlModeConnection.MONITOR)
            {
                if (tempPages.EnableCtrls)
                {
                    tempPages.EnableCtrls = false;
                }
            }
            else if (mode == (int)ControlModeConnection.CONTROL)
            {
                if (!tempPages.EnableCtrls)
                {
                    tempPages.EnableCtrls = true;
                }
            }

            //Profile mode ON
            if (jbc.GetPortToolProfileMode(myID, Port) == OnOff._ON)
            {
                if (ReferenceEquals(m_plotRecorderProfile_HA, null))
                {
                    m_plotRecorderProfile_HA = new PlotRecorderProfile_HA(jbc,
                        tabPages.panelWorkPageProfilePlot,
                        tabPages.labelWorkProfile_HA_HotAirTemp,
                        tabPages.labelWorkProfile_HA_ExtTCTemp,
                        tabPages.labelWorkProfile_HA_AirFlow,
                        tabPages.labelWorkProfile_HA_Status,
                        myID, Port);
                }

                if (!m_plotRecorderProfile_HA.IsRunning())
                {
                    //hide work data
                    tabPages.panelPower_HA.Visible = false;
                    tabPages.PanelTemps_HA.Visible = false;
                    tabPages.lblToolTemp_HA.Visible = false;
                    tabPages.lblToolTempUnits_HA.Visible = false;
                    tabPages.lblToolFlow_HA.Visible = false;
                    tabPages.lblToolFlowUnits_HA.Visible = false;
                    tabPages.labWorkPortStatus_HA.Visible = false;
                    //show plot
                    tabPages.panelWorkPageProfilePlot.Visible = true;
                    tabPages.labelWorkProfile_HA_HotAirTempTitle.Visible = true;
                    tabPages.labelWorkProfile_HA_HotAirTemp.Visible = true;
                    tabPages.labelWorkProfile_HA_ExtTCTempTitle.Visible = true;
                    tabPages.labelWorkProfile_HA_ExtTCTemp.Visible = true;
                    tabPages.labelWorkProfile_HA_AirFlowTitle.Visible = true;
                    tabPages.labelWorkProfile_HA_AirFlow.Visible = true;
                    tabPages.labelWorkProfile_HA_Status.Visible = true;
                    //start plot
                    await m_plotRecorderProfile_HA.Start();
                }

                //Profile mode OFF
            }
            else
            {
                if (m_plotRecorderProfile_HA != null)
                {

                    if (m_plotRecorderProfile_HA.IsRunning())
                    {
                        //show work data
                        tabPages.panelPower_HA.Visible = true;
                        tabPages.PanelTemps_HA.Visible = true;
                        tabPages.lblToolTemp_HA.Visible = true;
                        tabPages.lblToolTempUnits_HA.Visible = true;
                        tabPages.lblToolFlow_HA.Visible = true;
                        tabPages.lblToolFlowUnits_HA.Visible = true;
                        tabPages.labWorkPortStatus_HA.Visible = true;
                        //hide plot
                        tabPages.panelWorkPageProfilePlot.Visible = false;
                        tabPages.labelWorkProfile_HA_HotAirTempTitle.Visible = false;
                        tabPages.labelWorkProfile_HA_HotAirTemp.Visible = false;
                        tabPages.labelWorkProfile_HA_ExtTCTempTitle.Visible = false;
                        tabPages.labelWorkProfile_HA_ExtTCTemp.Visible = false;
                        tabPages.labelWorkProfile_HA_AirFlowTitle.Visible = false;
                        tabPages.labelWorkProfile_HA_AirFlow.Visible = false;
                        tabPages.labelWorkProfile_HA_Status.Visible = false;
                        //stop plot
                        m_plotRecorderProfile_HA.Stop();
                    }
                }
                UpdateWorkPageProfileOff();
            }
        }

        private void UpdateWorkPageProfileOff()
        {
            CTemperature temp = default(CTemperature);
            int flow = 0;

            // If no tempPages page selected, force update all data, checking what page to show
            if (tempPages.CurrentPage == "")
            {
                WorkParams = true;
            }

            GenericStationTools toolWork = default(GenericStationTools);
            toolWork = jbc.GetPortToolID(myID, Port);

            bool stopped = false;
            bool stand = false;
            bool pedal = false;
            bool suction = false;
            bool heater = false;
            bool cooling = false;

            //
            // NO TOOL
            //
            if (toolWork == GenericStationTools.NO_TOOL)
            {
                //show/hide only once (change from "Tool" to "No tool")
                if (!tabPages.lblNoTool.Visible)
                {
                    // hide work data
                    tabPages.panelPower_HA.Visible = false;
                    tabPages.PanelTemps_HA.Visible = false;
                    tabPages.lblToolTemp_HA.Visible = false;
                    tabPages.lblToolTempUnits_HA.Visible = false;
                    tabPages.labWorkPortStatus_HA.Visible = false;
                    // hide sleep data
                    //tabPages.panelWorkSleep.Visible = False
                    //show NO TOOL
                    tabPages.lblNoTool.Visible = true;
                    tabPages.lblToolNeeded.Visible = true;
                }
                //data
                tabPages.lblNoTool.Text = Localization.getResStr(Configuration.workNoToolId);
                tabPages.lblToolNeeded.Text = Localization.getResStr(Configuration.workToolNeededId);
            }
            else
            {

                // status
                string sWorkStatus = "";
                string sTFTStatus = "";
                string sTFTStand = "";
                string sTFTDelay = "";
                // changeha
                stand = jbc.GetPortToolStandStatus(myID, Port) == OnOff._ON;
                pedal = jbc.GetPortToolPedalStatus(myID, Port) == OnOff._ON;
                suction = jbc.GetPortToolSuctionStatus(myID, Port) == OnOff._ON;
                heater = jbc.GetPortToolHeaterStatus(myID, Port) == OnOff._ON;
                cooling = jbc.GetPortToolCoolingStatus(myID, Port) == OnOff._ON;
                stopped = false;
                int iTimeToStop = 0;
                //Debug.Print("Port={0} Stand:{1} Sleep:{2} Hiber:{3} Extractor:{4} Desolder:{5} Delay: {6} FutureMode:{7}", (Port + 1).ToString, stand.ToString, sleep.ToString, hiber.ToString, extractor.ToString, desolder.ToString, iDelay.ToString, jbc.GetPortToolFutureMode(myID, Port))
                if (stand)
                {
                    sWorkStatus = Localization.getResStr(Configuration.PortsStopId);
                    sTFTStatus = Localization.getResStr(Configuration.PortsStandId);
                    sTFTStand = Localization.getResStr(Configuration.PortsStandId);
                    stopped = true;
                }
                else if (pedal)
                {
                    sWorkStatus = Localization.getResStr(Configuration.PortsPedalId);
                    sTFTStatus = Localization.getResStr(Configuration.PortsPedalId);
                    sTFTStand = Localization.getResStr(Configuration.PortsPedalId);
                }
                else if (suction)
                {
                    sWorkStatus = Localization.getResStr(Configuration.PortsSuctionId);
                    sTFTStatus = Localization.getResStr(Configuration.PortsSuctionId);
                    sTFTStand = Localization.getResStr(Configuration.PortsSuctionId);
                }
                else if (heater)
                {
                    sWorkStatus = Localization.getResStr(Configuration.PortsHeaterId);
                    sTFTStatus = Localization.getResStr(Configuration.PortsHeaterId);
                    sTFTStand = Localization.getResStr(Configuration.PortsHeaterId);
                }
                else if (cooling)
                {
                    sWorkStatus = Localization.getResStr(Configuration.PortsCoolingId);
                    sTFTStatus = Localization.getResStr(Configuration.PortsCoolingId);
                    sTFTStand = "";
                }
                else
                {
                    sWorkStatus = Localization.getResStr(Configuration.PortsStopId);
                    stopped = true;
                }

                int pwr = 0;
                bool bFromNoTool = false;

                //show/hide only once (change from "No tool" to "Tool")
                if (tabPages.lblNoTool.Visible)
                {
                    //hide
                    tabPages.lblNoTool.Visible = false;
                    tabPages.lblToolNeeded.Visible = false;
                    //show
                    tabPages.PanelTemps_HA.Visible = true;

                    bFromNoTool = true;
                }

                //
                // WORK
                //

                // show/hide only once
                if (bFromNoTool)
                {
                    // sleep data
                    //tabPages.panelWorkSleep.Visible = False
                    // work data
                    tabPages.panelPower_HA.Visible = true;
                    tabPages.lblToolTemp_HA.Visible = true;
                    tabPages.lblToolTempUnits_HA.Visible = true;
                    tabPages.PanelTemps_HA.Visible = true;
                    tabPages.labWorkPortStatus_HA.Visible = true;
                }
                //tabPages.PanelTemps_HA.Location = locationPanelTempsWork

                // actual temp
                if (stopped)
                {
                    tabPages.lblToolTemp_HA.Text = "---";
                }
                else
                {
                    temp = jbc.GetPortToolActualTemp(myID, Port);
                    //Debug.Print("Actual Temp en working UTI: " & temp.UTI.ToString)
                    tabPages.lblToolTemp_HA.Text = Configuration.convertTempToString(temp, false, true);
                }

                // actual flow
                flow = jbc.GetPortToolActualFlow(myID, Port);
                tabPages.lblToolFlow_HA.Text = (flow / 10).ToString();

                tabPages.lblToolTempUnits_HA.Text = Configuration.Tunits;
                // status
                tabPages.labWorkPortStatus_HA.Text = sWorkStatus;
                // Power bar
                // 01/10/2013 se divide con \ truncando el resto
                pwr = System.Convert.ToInt32(jbc.GetPortToolActualPower(myID, Port) / 10);
                tabPages.lblPwr_HA.Text = pwr.ToString() + " " + Configuration.pwrUnitsPercentStr;

                mySetPowerScale(pwr);
            }

            if (WorkParams)
            {
                // Params updated
                WorkParams = false;

                // default=Manual
                // user can change selected temp for the working port, also if no tool selected
                bool bLevel = false;

                // Label desired temperature
                temp = jbc.GetPortToolSelectedTemp(myID, Port);
                tempPages.lblDesTemp.Text = string.Format(Localization.getResStr(Configuration.workTempSelectedId), Configuration.convertTempToString(temp, true, true));

                // Label desired flow
                flow = System.Convert.ToInt32(jbc.GetPortToolSelectedFlow(myID, Port) / 10);
                tempPages.lblDesFlow.Text = string.Format(Localization.getResStr(Configuration.workFlowSelectedId), flow.ToString() + " %");

                if (toolWork != GenericStationTools.NO_TOOL)
                {

                    //
                    // Levels
                    //
                    // ver si están implementados nivels en la HA

                    if (features.TempLevels)
                    {
                        bLevel = jbc.GetPortToolSelectedTempLevelsEnabled(myID, Port, toolWork) == OnOff._ON;
                        ToolTemperatureLevels lvl = jbc.GetPortToolSelectedTempLevels(myID, Port, toolWork);
                        if (bLevel)
                        {
                            if (lvl == ToolTemperatureLevels.NO_LEVELS)
                            {
                                bLevel = false;
                            }
                            else
                            {
                                switch (lvl)
                                {
                                    case ToolTemperatureLevels.FIRST_LEVEL:
                                        tempPages.setSelectedLevel(tempPages.lblLvl1);
                                        break;
                                    case ToolTemperatureLevels.SECOND_LEVEL:
                                        tempPages.setSelectedLevel(tempPages.lblLvl2);
                                        break;
                                    case ToolTemperatureLevels.THIRD_LEVEL:
                                        tempPages.setSelectedLevel(tempPages.lblLvl3);
                                        break;
                                }

                            }

                            bool thisLevel = false;

                            temp = jbc.GetPortToolTempLevel(myID, Port, toolWork, ToolTemperatureLevels.FIRST_LEVEL);
                            thisLevel = jbc.GetPortToolTempLevelEnabled(myID, Port, toolWork, ToolTemperatureLevels.FIRST_LEVEL) == OnOff._ON;
                            if (temp.isValid() && thisLevel)
                            {
                                tempPages.lblLvl1.Text = Configuration.convertTempToString(temp, false, true);
                                tempPages.lblLvl1.Enabled = true;
                            }
                            else
                            {
                                tempPages.lblLvl1.Text = Configuration.invalidTempStr;
                                tempPages.lblLvl1.Enabled = false;
                            }

                            temp = jbc.GetPortToolTempLevel(myID, Port, toolWork, ToolTemperatureLevels.SECOND_LEVEL);
                            thisLevel = jbc.GetPortToolTempLevelEnabled(myID, Port, toolWork, ToolTemperatureLevels.SECOND_LEVEL) == OnOff._ON;
                            if (temp.isValid() && thisLevel)
                            {
                                tempPages.lblLvl2.Text = Configuration.convertTempToString(temp, false, true);
                                tempPages.lblLvl2.Enabled = true;
                            }
                            else
                            {
                                tempPages.lblLvl2.Text = Configuration.invalidTempStr;
                                tempPages.lblLvl2.Enabled = false;

                            }

                            temp = jbc.GetPortToolTempLevel(myID, Port, toolWork, ToolTemperatureLevels.THIRD_LEVEL);
                            thisLevel = jbc.GetPortToolTempLevelEnabled(myID, Port, toolWork, ToolTemperatureLevels.THIRD_LEVEL) == OnOff._ON;
                            if (temp.isValid() && thisLevel)
                            {
                                tempPages.lblLvl3.Text = Configuration.convertTempToString(temp, false, true);
                                tempPages.lblLvl3.Enabled = true;
                            }
                            else
                            {
                                tempPages.lblLvl3.Text = Configuration.invalidTempStr;
                                tempPages.lblLvl3.Enabled = false;
                            }
                        }

                    }
                }

                if (bLevel)
                {
                    tempPages.CurrentPage = "pageTempLevels";
                }
                else
                {
                    tempPages.CurrentPage = "pageTempManual";
                }

            }
            System.Windows.Forms.Application.DoEvents();
        }

        #endregion

        #region STATION SETTINGS PAGE
        // Global var's
        private bool m_updateStationSettingsParams = false;

        // Controls
        private ParamTable stationParamsTable;
        private ParamTable m_ethernetParamsTable;
        private ParamTable m_robotParamsTable;

        // Creates the station settings page
        private void createStationSettingsPage()
        {

            var TOP_MARGIN = 0;
            var LEFT_MARGIN = 5;

            CTemperature auxMaxTemp = jbc.GetStationFeatures(myID).MaxTemp; // getMaxTemp(jbc.GetStationModel(myID))
            CTemperature auxMinTemp = jbc.GetStationFeatures(myID).MinTemp;
            string[] tempopt = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(auxMinTemp, false, true), Configuration.convertTempToString(auxMaxTemp, false, true) };
            string[] flowopt = new string[] { Configuration.sReplaceTag + " " + Configuration.flowUnitsStr, "0", "100" };
            string[] pwropt = new string[] { Configuration.sReplaceTag + " " + Configuration.pwrUnitsStr };


            //
            // Titles
            //
            tabPages.rbGeneralSettings.Text = Localization.getResStr(Configuration.stnGenSettingsId);
            tabPages.rbEthernetConf.Text = Localization.getResStr(Configuration.stnEthConfId);
            tabPages.rbRobotConf.Text = Localization.getResStr(Configuration.stnRbtConfId);


            //
            // General Settings
            //
            // Creating the param table
            stationParamsTable = new ParamTable(300, 140); // default columns width
            stationParamsTable.NewValue += stationParamsTable_NewValue;
            stationParamsTable.Location = new Point(tabPages.pageStationSettings.ClientSize.Width * LEFT_MARGIN / 100, tabPages.pageStationSettings.ClientSize.Height * TOP_MARGIN / 100);
            stationParamsTable.Height = tabPages.pageStationSettings.ClientSize.Height * (100 - TOP_MARGIN - 5) / 100;
            stationParamsTable.Width = tabPages.pageStationSettings.ClientSize.Width * (100 - (LEFT_MARGIN * 2)) / 100;

            // Adding the station parametters
            stationParamsTable.addParam(Configuration.stnNameId, Localization.getResStr(Configuration.stnNameId), ParamTable.cInputType.TEXT, null, null, false, 15);

            string[] aOnOff = new string[] { OnOff._ON.ToString("D"), OnOff._OFF.ToString("D") };
            string[] OnOffText = new string[] { Localization.getResStr(Configuration.ON_STRId), Localization.getResStr(Configuration.OFF_STRId) };

            string[] tempUnits = new string[] { CTemperature.TemperatureUnit.Celsius.ToString("D"), CTemperature.TemperatureUnit.Fahrenheit.ToString("D") };
            string[] tempUnitsText = new string[] { Localization.getResStr(Configuration.stnTunitsCelsiusId), Localization.getResStr(Configuration.stnTunitsFahrenheitId) };

            stationParamsTable.addParam(Configuration.stnTmaxId, Localization.getResStr(Configuration.stnTmaxId), ParamTable.cInputType.NUMBER, tempopt, null, false, 3);
            stationParamsTable.addParam(Configuration.stnTminId, Localization.getResStr(Configuration.stnTminId), ParamTable.cInputType.NUMBER, tempopt, null, false, 3);
            stationParamsTable.addParam(Configuration.stnFlowmaxId, Localization.getResStr(Configuration.stnFlowmaxId), ParamTable.cInputType.NUMBER, flowopt, null, false, 3);
            stationParamsTable.addParam(Configuration.stnFlowminId, Localization.getResStr(Configuration.stnFlowminId), ParamTable.cInputType.NUMBER, flowopt, null, false, 3);
            stationParamsTable.addParam(Configuration.stnTExtmaxId, Localization.getResStr(Configuration.stnTExtmaxId), ParamTable.cInputType.NUMBER, tempopt, null, false, 3);
            stationParamsTable.addParam(Configuration.stnTExtminId, Localization.getResStr(Configuration.stnTExtminId), ParamTable.cInputType.NUMBER, tempopt, null, false, 3);

            stationParamsTable.addParam(Configuration.stnTunitsId, Localization.getResStr(Configuration.stnTunitsId), ParamTable.cInputType.SWITCH, tempUnits, tempUnitsText);
            stationParamsTable.addParam(Configuration.stnBeepId, Localization.getResStr(Configuration.stnBeepId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);

            stationParamsTable.addParam(Configuration.stnPINId, Localization.getResStr(Configuration.stnPINId), ParamTable.cInputType.TEXT, null, null, false, 4);


            //
            // Ethernet configuration
            //
            if (features.Ethernet)
            {
                m_ethernetParamsTable = new ParamTable(300, 140); // default columns width
                m_ethernetParamsTable.NewValue += ethernetParamsTable_NewValue;
                m_ethernetParamsTable.Location = new Point(tabPages.pageStationSettings.ClientSize.Width * LEFT_MARGIN / 100, tabPages.pageStationSettings.ClientSize.Height * TOP_MARGIN / 100);
                m_ethernetParamsTable.Height = tabPages.pageStationSettings.ClientSize.Height * (100 - TOP_MARGIN - 5) / 100;
                m_ethernetParamsTable.Width = tabPages.pageStationSettings.ClientSize.Width * (100 - (LEFT_MARGIN * 2)) / 100;

                string[] ethernetPortValues = Configuration.getEthernetPort(Configuration.arrOption.VALUES);

                // Adding the ethernet parametters
                m_ethernetParamsTable.addParam(Configuration.stnEthDHCPId, Localization.getResStr(Configuration.stnEthDHCPId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
                m_ethernetParamsTable.addParam(Configuration.stnEthIPId, Localization.getResStr(Configuration.stnEthIPId), ParamTable.cInputType.TEXT, null, null);
                m_ethernetParamsTable.addParam(Configuration.stnEthMaskId, Localization.getResStr(Configuration.stnEthMaskId), ParamTable.cInputType.TEXT, null, null);
                m_ethernetParamsTable.addParam(Configuration.stnEthGatewayId, Localization.getResStr(Configuration.stnEthGatewayId), ParamTable.cInputType.TEXT, null, null);
                m_ethernetParamsTable.addParam(Configuration.stnEthDNSId, Localization.getResStr(Configuration.stnEthDNSId), ParamTable.cInputType.TEXT, null, null);
                m_ethernetParamsTable.addParam(Configuration.stnEthPortId, Localization.getResStr(Configuration.stnEthPortId), ParamTable.cInputType.NUMBER, ethernetPortValues, null, false, 5);

                m_ethernetParamsTable.Visible = false;
            }


            //
            // Robot configuration
            //
            if (features.Robot)
            {
                m_robotParamsTable = new ParamTable(300, 140); // default columns width
                m_robotParamsTable.NewValue += robotParamsTable_NewValue;
                m_robotParamsTable.Location = new Point(tabPages.pageStationSettings.ClientSize.Width * LEFT_MARGIN / 100, tabPages.pageStationSettings.ClientSize.Height * TOP_MARGIN / 100);
                m_robotParamsTable.Height = tabPages.pageStationSettings.ClientSize.Height * (100 - TOP_MARGIN - 5) / 100;
                m_robotParamsTable.Width = tabPages.pageStationSettings.ClientSize.Width * (100 - (LEFT_MARGIN * 2)) / 100;

                string[] robotProtocolValues = Configuration.getRobotProtocol(Configuration.arrOption.VALUES);
                string[] robotProtocolTexts = Configuration.getRobotProtocol(Configuration.arrOption.TEXTS);
                string[] robotAddressValues = Configuration.getRobotAddress(Configuration.arrOption.VALUES);
                string[] robotSpeedValues = Configuration.getRobotSpeed(Configuration.arrOption.VALUES);
                string[] robotSpeedTexts = Configuration.getRobotSpeed(Configuration.arrOption.TEXTS);
                string[] robotStopBitsValues = Configuration.getRobotStopBits(Configuration.arrOption.VALUES);
                string[] robotStopBitsTexts = Configuration.getRobotStopBits(Configuration.arrOption.TEXTS);
                string[] robotParityValues = Configuration.getRobotParity(Configuration.arrOption.VALUES);
                string[] robotParityTexts = Configuration.getRobotParity(Configuration.arrOption.TEXTS);

                // Adding the robot parametters
                m_robotParamsTable.addParam(Configuration.stnRbtStatusId, Localization.getResStr(Configuration.stnRbtStatusId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
                m_robotParamsTable.addParam(Configuration.stnRbtProtocolId, Localization.getResStr(Configuration.stnRbtProtocolId), ParamTable.cInputType.DROPLIST, robotProtocolValues, robotProtocolTexts);
                m_robotParamsTable.addParam(Configuration.stnRbtAddressId, Localization.getResStr(Configuration.stnRbtAddressId), ParamTable.cInputType.TEXT, robotAddressValues, null, false, 2);
                m_robotParamsTable.addParam(Configuration.stnRbtSpeedId, Localization.getResStr(Configuration.stnRbtSpeedId), ParamTable.cInputType.DROPLIST, robotSpeedValues, robotSpeedTexts);
                m_robotParamsTable.addParam(Configuration.stnRbtDataBitsId, Localization.getResStr(Configuration.stnRbtDataBitsId), ParamTable.cInputType.FIX, null, null);
                m_robotParamsTable.addParam(Configuration.stnRbtStopBitsId, Localization.getResStr(Configuration.stnRbtStopBitsId), ParamTable.cInputType.DROPLIST, robotStopBitsValues, robotStopBitsTexts);
                m_robotParamsTable.addParam(Configuration.stnRbtParityId, Localization.getResStr(Configuration.stnRbtParityId), ParamTable.cInputType.DROPLIST, robotParityValues, robotParityTexts);

                m_robotParamsTable.Visible = false;
            }

            stationParamsTable.ptyDefaultRowHeight = 22;

            //Add params table to control
            tabPages.panelStationSettings.Controls.Add(stationParamsTable);
            tabPages.panelStationSettings.Controls.Add(m_ethernetParamsTable);
            tabPages.panelStationSettings.Controls.Add(m_robotParamsTable);


            ToolTipIcons.SetToolTip(iconStationSettings, Localization.getResStr(Configuration.stnTabHintId));

            //Show/hide tabs
            tabPages.rbGeneralSettings.Visible = features.Ethernet || features.Robot;
            tabPages.rbEthernetConf.Visible = features.Ethernet;
            tabPages.rbRobotConf.Visible = features.Robot;


            // Indicating update of parametters required
            m_updateStationSettingsParams = true;
        }

        private void loadTextsStationSettingsPage()
        {
            CTemperature auxMaxTemp = jbc.GetStationFeatures(myID).MaxTemp; // getMaxTemp(jbc.GetStationModel(myID))
            CTemperature auxMinTemp = jbc.GetStationFeatures(myID).MinTemp;
            string[] tempopt = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(auxMinTemp, false, true), Configuration.convertTempToString(auxMaxTemp, false, true) };
            string[] flowopt = new string[] { Configuration.sReplaceTag + " " + Configuration.flowUnitsStr, "0", "100" };
            string[] pwropt = new string[] { Configuration.sReplaceTag + " " + Configuration.pwrUnitsStr };
            string[] OnOffText = new string[] { Localization.getResStr(Configuration.ON_STRId), Localization.getResStr(Configuration.OFF_STRId) };
            string[] tempUnitsText = new string[] { Localization.getResStr(Configuration.stnTunitsCelsiusId), Localization.getResStr(Configuration.stnTunitsFahrenheitId) };

            // update texts in the station parametters
            stationParamsTable.setText(Configuration.stnNameId, Localization.getResStr(Configuration.stnNameId), null);


            //
            // Titles
            //
            tabPages.rbGeneralSettings.Text = Localization.getResStr(Configuration.stnGenSettingsId);
            tabPages.rbEthernetConf.Text = Localization.getResStr(Configuration.stnEthConfId);
            tabPages.rbRobotConf.Text = Localization.getResStr(Configuration.stnRbtConfId);


            //
            // General Settings
            //
            stationParamsTable.setText(Configuration.stnTmaxId, Localization.getResStr(Configuration.stnTmaxId), tempopt);
            stationParamsTable.setText(Configuration.stnTminId, Localization.getResStr(Configuration.stnTminId), tempopt);
            stationParamsTable.setText(Configuration.stnFlowmaxId, Localization.getResStr(Configuration.stnFlowmaxId), flowopt);
            stationParamsTable.setText(Configuration.stnFlowminId, Localization.getResStr(Configuration.stnFlowminId), flowopt);
            stationParamsTable.setText(Configuration.stnTExtmaxId, Localization.getResStr(Configuration.stnTExtmaxId), tempopt);
            stationParamsTable.setText(Configuration.stnTExtminId, Localization.getResStr(Configuration.stnTExtminId), tempopt);

            stationParamsTable.setText(Configuration.stnTunitsId, Localization.getResStr(Configuration.stnTunitsId), tempUnitsText);
            stationParamsTable.setText(Configuration.stnBeepId, Localization.getResStr(Configuration.stnBeepId), OnOffText);

            stationParamsTable.setText(Configuration.stnPINId, Localization.getResStr(Configuration.stnPINId), null);

            //
            // Ethernet configuration
            //
            if (features.Ethernet)
            {
                m_ethernetParamsTable.setText(Configuration.stnEthDHCPId, Localization.getResStr(Configuration.stnEthDHCPId), OnOffText);
                m_ethernetParamsTable.setText(Configuration.stnEthIPId, Localization.getResStr(Configuration.stnEthIPId), null);
                m_ethernetParamsTable.setText(Configuration.stnEthMaskId, Localization.getResStr(Configuration.stnEthMaskId), null);
                m_ethernetParamsTable.setText(Configuration.stnEthGatewayId, Localization.getResStr(Configuration.stnEthGatewayId), null);
                m_ethernetParamsTable.setText(Configuration.stnEthDNSId, Localization.getResStr(Configuration.stnEthDNSId), null);
                m_ethernetParamsTable.setText(Configuration.stnEthPortId, Localization.getResStr(Configuration.stnEthPortId), null);
            }


            //
            // Robot configuration
            //
            if (features.Robot)
            {
                string[] robotParityTexts = Configuration.getRobotParity(Configuration.arrOption.TEXTS);

                m_robotParamsTable.setText(Configuration.stnRbtStatusId, Localization.getResStr(Configuration.stnRbtStatusId), OnOffText);
                m_robotParamsTable.setText(Configuration.stnRbtProtocolId, Localization.getResStr(Configuration.stnRbtProtocolId), null);
                m_robotParamsTable.setText(Configuration.stnRbtAddressId, Localization.getResStr(Configuration.stnRbtAddressId), null);
                m_robotParamsTable.setText(Configuration.stnRbtSpeedId, Localization.getResStr(Configuration.stnRbtSpeedId), null);
                m_robotParamsTable.setText(Configuration.stnRbtDataBitsId, Localization.getResStr(Configuration.stnRbtDataBitsId), null);
                m_robotParamsTable.setText(Configuration.stnRbtStopBitsId, Localization.getResStr(Configuration.stnRbtStopBitsId), null);
                m_robotParamsTable.setText(Configuration.stnRbtParityId, Localization.getResStr(Configuration.stnRbtParityId), robotParityTexts);
            }


            ToolTipIcons.SetToolTip(iconStationSettings, Localization.getResStr(Configuration.stnTabHintId));

            // Indicating update of parametters required
            m_updateStationSettingsParams = true;
        }

        private void onStationSettings_CheckedChanged(string sType)
        {
            if (sType == "Eth")
            {
                stationParamsTable.Visible = false;
                if (!ReferenceEquals(m_ethernetParamsTable, null))
                {
                    m_ethernetParamsTable.Visible = true;
                }
                if (!ReferenceEquals(m_robotParamsTable, null))
                {
                    m_robotParamsTable.Visible = false;
                }
            }
            else if (sType == "Rbt")
            {
                stationParamsTable.Visible = false;
                if (!ReferenceEquals(m_ethernetParamsTable, null))
                {
                    m_ethernetParamsTable.Visible = false;
                }
                if (!ReferenceEquals(m_robotParamsTable, null))
                {
                    m_robotParamsTable.Visible = true;
                }
            }
            else //Gen
            {
                stationParamsTable.Visible = true;
                if (!ReferenceEquals(m_ethernetParamsTable, null))
                {
                    m_ethernetParamsTable.Visible = false;
                }
                if (!ReferenceEquals(m_robotParamsTable, null))
                {
                    m_robotParamsTable.Visible = false;
                }
            }

            m_updateStationSettingsParams = true;
        }

        // Updates the station settings page
        private async void updateStationSettingsPage()
        {

            // When mode has been changed
            if (mode == (int)ControlModeConnection.MONITOR)
            {
                stationParamsTable.setValue(Configuration.stnPINId, Configuration.hidePINtext);
                stationParamsTable.inputControlsEnable(false);
                if (!ReferenceEquals(m_ethernetParamsTable, null))
                {
                    m_ethernetParamsTable.inputControlsEnable(false);
                }
                if (!ReferenceEquals(m_robotParamsTable, null))
                {
                    m_robotParamsTable.inputControlsEnable(false);
                }
            }
            else if (mode == (int)ControlModeConnection.CONTROL)
            {
                stationParamsTable.setValue(Configuration.stnPINId, jbc.GetStationPIN(myID));
                stationParamsTable.inputControlsEnable(true);
                if (!ReferenceEquals(m_ethernetParamsTable, null))
                {
                    m_ethernetParamsTable.inputControlsEnable(true);
                }
                if (!ReferenceEquals(m_robotParamsTable, null))
                {
                    m_robotParamsTable.inputControlsEnable(true);
                }
            }

            // Only if parameter update required
            if (m_updateStationSettingsParams)
            {

                //
                // General settings
                //
                await jbc.UpdateStationSettingsAsync(myID);

                // Getting the station parameters
                stationParamsTable.setValue(Configuration.stnNameId, jbc.GetStationName(myID));
                // Update station name in station list
                //myItem.Text = jbc.GetStationName(myID)
                if (StationNameChangedEvent != null)
                    StationNameChangedEvent(myID, jbc.GetStationName(myID));

                CTemperature temp = default(CTemperature);
                int flow = 0;

                temp = jbc.GetStationMaxTemp(myID);
                stationParamsTable.setValue(Configuration.stnTmaxId, Configuration.convertTempToString(temp, false, true));
                temp = jbc.GetStationMinTemp(myID);
                stationParamsTable.setValue(Configuration.stnTminId, Configuration.convertTempToString(temp, false, true));
                temp = jbc.GetStationMaxExtTemp(myID);
                stationParamsTable.setValue(Configuration.stnTExtmaxId, Configuration.convertTempToString(temp, false, true));
                temp = jbc.GetStationMinExtTemp(myID);
                stationParamsTable.setValue(Configuration.stnTExtminId, Configuration.convertTempToString(temp, false, true));
                flow = System.Convert.ToInt32(jbc.GetStationMaxFlow(myID) / 10);
                stationParamsTable.setValue(Configuration.stnFlowmaxId, flow.ToString());
                flow = System.Convert.ToInt32(jbc.GetStationMinFlow(myID) / 10);
                stationParamsTable.setValue(Configuration.stnFlowminId, flow.ToString());

                CTemperature.TemperatureUnit units = jbc.GetStationTempUnits(myID);
                stationParamsTable.setValue(Configuration.stnTunitsId, units.ToString());
                stationParamsTable.setValue(Configuration.stnBeepId, jbc.GetStationBeep(myID).ToString());

                //
                // Ethernet
                //
                if (features.Ethernet && !ReferenceEquals(m_ethernetParamsTable, null))
                {
                    await jbc.UpdateEthernetConfigurationAsync(myID);
                    CEthernetData ethData = jbc.GetEthernetConfiguration(myID);

                    m_ethernetParamsTable.setValue(Configuration.stnEthDHCPId, ethData.DHCP.ToString());
                    m_ethernetParamsTable.setValue(Configuration.stnEthIPId, ethData.IP.ToString());
                    m_ethernetParamsTable.setValue(Configuration.stnEthMaskId, ethData.Mask.ToString());
                    m_ethernetParamsTable.setValue(Configuration.stnEthGatewayId, ethData.Gateway.ToString());
                    m_ethernetParamsTable.setValue(Configuration.stnEthDNSId, ethData.DNS1.ToString());
                    m_ethernetParamsTable.setValue(Configuration.stnEthPortId, ethData.Port.ToString());
                }

                //
                // Robot
                //
                if (features.Robot && !ReferenceEquals(m_robotParamsTable, null))
                {
                    await jbc.UpdateRobotConfigurationAsync(myID);
                    CRobotData rbtData = jbc.GetRobotConfiguration(myID);

                    m_robotParamsTable.setValue(Configuration.stnRbtStatusId, rbtData.Status.ToString());
                    m_robotParamsTable.setValue(Configuration.stnRbtProtocolId, rbtData.Protocol.ToString());
                    m_robotParamsTable.setValue(Configuration.stnRbtAddressId, rbtData.Address.ToString());
                    m_robotParamsTable.setValue(Configuration.stnRbtSpeedId, rbtData.Speed.ToString());
                    m_robotParamsTable.setValue(Configuration.stnRbtDataBitsId, rbtData.DataBits.ToString());
                    m_robotParamsTable.setValue(Configuration.stnRbtStopBitsId, rbtData.StopBits.ToString());
                    m_robotParamsTable.setValue(Configuration.stnRbtParityId, rbtData.Parity.ToString());
                }

                // Update done
                m_updateStationSettingsParams = false;
            }
        }

        private async void stationParamsTable_NewValue(string name, string value)
        {
            //Some value has been changed, sending it to the station
            if (name == Configuration.stnNameId && name != "")
            {
                await jbc.SetStationNameAsync(myID, value);
                if (StationNameChangedEvent != null)
                    StationNameChangedEvent(myID, value);
            }
            if (name == Configuration.stnTminId)
            {
                await jbc.SetStationMinTempAsync(myID, Configuration.convertStringToTemp(value));
            }
            if (name == Configuration.stnTmaxId)
            {
                await jbc.SetStationMaxTempAsync(myID, Configuration.convertStringToTemp(value));
            }
            if (name == Configuration.stnTExtminId)
            {
                await jbc.SetStationMinExtTempAsync(myID, Configuration.convertStringToTemp(value));
            }
            if (name == Configuration.stnTExtmaxId)
            {
                await jbc.SetStationMaxExtTempAsync(myID, Configuration.convertStringToTemp(value));
            }
            if (name == Configuration.stnFlowminId)
            {
                await jbc.SetStationMinFlowAsync(myID, (int.Parse(value)) * 10);
            }
            if (name == Configuration.stnFlowmaxId)
            {
                await jbc.SetStationMaxFlowAsync(myID, (int.Parse(value)) * 10);
            }

            if (name == Configuration.stnTunitsId)
            {
                await jbc.SetStationTempUnitsAsync(myID, (CTemperature.TemperatureUnit)Int32.Parse(value));
            }
            if (name == Configuration.stnBeepId)
            {
                await jbc.SetStationBeepAsync(myID, (OnOff)int.Parse(value));
            }
            if (name == Configuration.stnPINId)
            {
                await jbc.SetStationPINAsync(myID, value);
            }

            //Forcing an update
            m_updateStationSettingsParams = true;
        }

        private async void ethernetParamsTable_NewValue(string name, string value)
        {

            bool bEthOk = true;
            CEthernetData ethData = new CEthernetData();

            ethData.DHCP = (OnOff)Int32.Parse(m_ethernetParamsTable.getValue(Configuration.stnEthDHCPId));

            IPAddress temp_address = ethData.IP;
            IPAddress temp_address2 = ethData.Mask;
            IPAddress temp_address3 = ethData.Gateway;
            IPAddress temp_address4 = ethData.DNS1;
            bEthOk = System.Convert.ToBoolean(IPAddress.TryParse(m_ethernetParamsTable.getValue(Configuration.stnEthIPId), out temp_address) &&
                                              IPAddress.TryParse(m_ethernetParamsTable.getValue(Configuration.stnEthMaskId), out temp_address2) &&
                                              IPAddress.TryParse(m_ethernetParamsTable.getValue(Configuration.stnEthGatewayId), out temp_address3) &&
                                              IPAddress.TryParse(m_ethernetParamsTable.getValue(Configuration.stnEthDNSId), out temp_address4));
            ethData.IP = temp_address;
            ethData.Mask = temp_address2;
            ethData.Gateway = temp_address3;
            ethData.DNS1 = temp_address4;

            ethData.Port = System.Convert.ToUInt16(m_ethernetParamsTable.getValue(Configuration.stnEthPortId));

            if (bEthOk)
            {
                await jbc.SetEthernetConfigurationAsync(myID, ethData);
            }

            //Forcing an update
            m_updateStationSettingsParams = true;
        }

        private async void robotParamsTable_NewValue(string name, string value)
        {

            MsgBoxResult responseRbt = MsgBoxResult.Ok;
            OnOff statusRbt = (OnOff)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtStatusId));

            if (statusRbt == OnOff._ON)
            {
                responseRbt = Interaction.MsgBox(Localization.getResStr(Configuration.modeRobotGetControlWarningId), MsgBoxStyle.OkCancel, null);
            }

            if (responseRbt == MsgBoxResult.Ok)
            {
                CRobotData rbtData = new CRobotData();
                rbtData.Status = statusRbt;
                rbtData.Protocol = (CRobotData.RobotProtocol)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtProtocolId));
                rbtData.Address = System.Convert.ToUInt16(m_robotParamsTable.getValue(Configuration.stnRbtAddressId));
                rbtData.Speed = (CRobotData.RobotSpeed)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtSpeedId));
                rbtData.DataBits = System.Convert.ToUInt16(m_robotParamsTable.getValue(Configuration.stnRbtDataBitsId));
                rbtData.StopBits = (CRobotData.RobotStop)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtStopBitsId));
                rbtData.Parity = (CRobotData.RobotParity)Int32.Parse(m_robotParamsTable.getValue(Configuration.stnRbtParityId));

                await jbc.SetRobotConfigurationAsync(myID, rbtData);

                //Forcing an update
                m_updateStationSettingsParams = true;
            }
        }

        #endregion

        #region TOOL SETTINGS PAGE
        // Global var's
        private bool m_toolSettingsParams = false;

        private bool bAlreadySetConnectedToolsAndPorts = false;
        private GenericStationTools toolCurTool;
        private Port toolCurPort;
        private GenericStationTools toolConnectedToolPort = GenericStationTools.NO_TOOL;

        // Controls
        private ParamTable toolParamsTable;

        // Creates the tool settings page
        private void createToolSettingsPage()
        {
            var TOP_MARGIN = 10;
            var LEFT_MARGIN = 5;

            CTemperature auxMaxTemp = jbc.GetStationFeatures(myID).MaxTemp; // getMaxTemp(jbc.GetStationModel(myID))
            CTemperature auxMinTemp = jbc.GetStationFeatures(myID).MinTemp;
            string[] tempopt = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(auxMinTemp, false, true), Configuration.convertTempToString(auxMaxTemp, false, true) };
            string[] tempoptAdj = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(Configuration.tempMinAdj, false, true), Configuration.convertTempToString(Configuration.tempMaxAdj, false, true) };

            // Creating the param table
            toolParamsTable = new ParamTable(250, 110, 20); // default columns width
            toolParamsTable.NewValue += toolParamsTable_NewValue;
            toolParamsTable.paramTableCheckedChanged += toolParamsTable_CheckedChanged;
            toolParamsTable.paramTableButtonClicked += toolParamsTable_ButtonClicked;
            //toolParamsTable.Location = New Point(tabPages.pageToolSettings.ClientSize.Width * LEFT_MARGIN / 100, TableLayoutPanelTools.Location.Y + TableLayoutPanelTools.Height + 10)
            toolParamsTable.Location = new Point(System.Convert.ToInt32((double)tabPages.pageToolSettings.ClientSize.Width * LEFT_MARGIN / 100), tabPages.lblSelectedTool.Location.Y + tabPages.lblSelectedTool.Height + 5);
            toolParamsTable.Height = tabPages.pageToolSettings.ClientSize.Height - toolParamsTable.Location.Y + 10;
            toolParamsTable.Width = System.Convert.ToInt32((double)tabPages.pageToolSettings.ClientSize.Width * (100 - (LEFT_MARGIN * 4)) / 100);

            // Adding the tool parametters and its input controls
            //toolParamsTable.addParam(toolSelectedTempId, getResStr(toolSelectedTempId), ParamTable.cInputType.NUMBER, tempopt)

            string[] aOnOff = new string[] { OnOff._ON.ToString("D"), OnOff._OFF.ToString("D") };
            string[] OnOffText = new string[] { Localization.getResStr(Configuration.ON_STRId), Localization.getResStr(Configuration.OFF_STRId) };

            string[] tempLvlsValues = Configuration.getTempLevels(Configuration.arrOption.VALUES, features.TempLevelsWithStatus);
            string[] tempLvls = Configuration.getTempLevels(Configuration.arrOption.TEXTS, features.TempLevelsWithStatus);

            string[] timeToStopValues = Configuration.getTimeToStop(Configuration.arrOption.VALUES);

            string[] extTCModeValues = Configuration.getExtTCMode(Configuration.arrOption.VALUES);
            string[] extTCMode = Configuration.getExtTCMode(Configuration.arrOption.TEXTS);

            string[] startModePedalActionValues = Configuration.getStartModePedalActivation(Configuration.arrOption.VALUES);
            string[] startModePedalAction = Configuration.getStartModePedalActivation(Configuration.arrOption.TEXTS);

            //Dim sleepDelaysValues() As String = getSleepDelays(arrOption.VALUES, features.DelayWithStatus)
            //Dim sleepDelays() As String = getSleepDelays(arrOption.TEXTS, features.DelayWithStatus)
            //Dim hiberDelaysValues() As String = getHiberDelays(arrOption.VALUES, features.DelayWithStatus)
            //Dim hiberDelays() As String = getHiberDelays(arrOption.TEXTS, features.DelayWithStatus)
            if (features.TempLevels)
            {
                toolParamsTable.addParam(Configuration.toolSelectedTempLvlId, Localization.getResStr(Configuration.toolSelectedTempLvlId), ParamTable.cInputType.SWITCH, tempLvlsValues, tempLvls, features.TempLevelsWithStatus);
                toolParamsTable.addParam(Configuration.toolTempLvl1Id, "    " + Localization.getResStr(Configuration.toolTempLvl1Id), ParamTable.cInputType.NUMBER, tempopt, null, features.TempLevelsWithStatus, 3);
                toolParamsTable.addParam(Configuration.toolTempLvl2Id, "    " + Localization.getResStr(Configuration.toolTempLvl2Id), ParamTable.cInputType.NUMBER, tempopt, null, features.TempLevelsWithStatus, 3);
                toolParamsTable.addParam(Configuration.toolTempLvl3Id, "    " + Localization.getResStr(Configuration.toolTempLvl3Id), ParamTable.cInputType.NUMBER, tempopt, null, features.TempLevelsWithStatus, 3);
            }
            //toolParamsTable.addParam(toolSleepTempId, getResStr(toolSleepTempId), ParamTable.cInputType.NUMBER, tempopt, Nothing, False, 3)
            //toolParamsTable.addParam(toolSleepDelayId, getResStr(toolSleepDelayId), ParamTable.cInputType.DROPLIST, sleepDelaysValues, sleepDelays, features.DelayWithStatus)
            //toolParamsTable.addParam(toolHibernationDelayId, getResStr(toolHibernationDelayId), ParamTable.cInputType.DROPLIST, hiberDelaysValues, hiberDelays, features.DelayWithStatus)
            toolParamsTable.addParam(Configuration.toolProfilesId, Localization.getResStr(Configuration.toolProfilesId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
            toolParamsTable.addParam(Configuration.toolAdjustTempId, Localization.getResStr(Configuration.toolAdjustTempId), ParamTable.cInputType.NUMBER, tempoptAdj, null, false, 4);
            toolParamsTable.addParam(Configuration.toolTimeToStopId, Localization.getResStr(Configuration.toolTimeToStopId), ParamTable.cInputType.NUMBER, timeToStopValues, null, System.Convert.ToBoolean(null), 2);
            toolParamsTable.addParam(Configuration.toolExtTCModeId, Localization.getResStr(Configuration.toolExtTCModeId), ParamTable.cInputType.DROPLIST, extTCModeValues, extTCMode);
            toolParamsTable.addParam(Configuration.toolExtTCTempId, Localization.getResStr(Configuration.toolExtTCTempId), ParamTable.cInputType.NUMBER, tempopt, null, false, 4);
            toolParamsTable.addParam(Configuration.toolStartModeToolButtonId, Localization.getResStr(Configuration.toolStartModeToolButtonId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
            toolParamsTable.addParam(Configuration.toolStartModeStandOutId, Localization.getResStr(Configuration.toolStartModeStandOutId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
            toolParamsTable.addParam(Configuration.toolStartModePedalActionId, Localization.getResStr(Configuration.toolStartModePedalActionId), ParamTable.cInputType.DROPLIST, startModePedalActionValues, startModePedalAction);

            toolParamsTable.ptyDefaultRowHeight = 22;

            // Adding the table to the page panel
            tabPages.pageToolSettings.Controls.Add(toolParamsTable);
            ToolTipIcons.SetToolTip(iconToolSettings, Localization.getResStr(Configuration.toolTabHintId));

            tabPages.lblSelectedTool.Text = Localization.getResStr(Configuration.toolSelectedToolId);

            //Forcing update
            m_toolSettingsParams = true;
        }

        private void loadTextsToolSettingsPage()
        {

            // update texts
            CTemperature auxMaxTemp = jbc.GetStationFeatures(myID).MaxTemp; // getMaxTemp(jbc.GetStationModel(myID))
            CTemperature auxMinTemp = jbc.GetStationFeatures(myID).MinTemp;
            string[] tempopt = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(auxMinTemp, false, true), Configuration.convertTempToString(auxMaxTemp, false, true) };
            string[] tempoptAdj = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(Configuration.tempMinAdj, false, true), Configuration.convertTempToString(Configuration.tempMaxAdj, false, true) };
            string[] tempLvls = Configuration.getTempLevels(Configuration.arrOption.TEXTS, features.TempLevelsWithStatus);
            string[] sleepDelays = Configuration.getSleepDelays(Configuration.arrOption.TEXTS, features.DelayWithStatus);
            string[] hiberDelays = Configuration.getHiberDelays(Configuration.arrOption.TEXTS, features.DelayWithStatus);
            string[] OnOffText = new string[] { Localization.getResStr(Configuration.ON_STRId), Localization.getResStr(Configuration.OFF_STRId) };

            //toolParamsTable.setText(toolSelectedTempId, getResStr(toolSelectedTempId), tempopt)
            if (features.TempLevels)
            {
                toolParamsTable.setText(Configuration.toolSelectedTempLvlId, Localization.getResStr(Configuration.toolSelectedTempLvlId), tempLvls);
                toolParamsTable.setText(Configuration.toolTempLvl1Id, "    " + Localization.getResStr(Configuration.toolTempLvl1Id), tempopt);
                toolParamsTable.setText(Configuration.toolTempLvl2Id, "    " + Localization.getResStr(Configuration.toolTempLvl2Id), tempopt);
                toolParamsTable.setText(Configuration.toolTempLvl3Id, "    " + Localization.getResStr(Configuration.toolTempLvl3Id), tempopt);
            }
            //toolParamsTable.setText(toolSleepTempId, getResStr(toolSleepTempId), tempopt)
            //toolParamsTable.setText(toolSleepDelayId, getResStr(toolSleepDelayId), sleepDelays)
            //toolParamsTable.setText(toolHibernationDelayId, getResStr(toolHibernationDelayId), hiberDelays)
            toolParamsTable.setText(Configuration.toolProfilesId, Localization.getResStr(Configuration.toolProfilesId), OnOffText);
            toolParamsTable.setText(Configuration.toolAdjustTempId, Localization.getResStr(Configuration.toolAdjustTempId), tempoptAdj);
            toolParamsTable.setText(Configuration.toolTimeToStopId, Localization.getResStr(Configuration.toolTimeToStopId), null);
            toolParamsTable.setText(Configuration.toolExtTCModeId, Localization.getResStr(Configuration.toolExtTCModeId), null);
            toolParamsTable.setText(Configuration.toolExtTCTempId, Localization.getResStr(Configuration.toolExtTCTempId), tempopt);
            toolParamsTable.setText(Configuration.toolStartModeToolButtonId, Localization.getResStr(Configuration.toolStartModeToolButtonId), null);
            toolParamsTable.setText(Configuration.toolStartModeStandOutId, Localization.getResStr(Configuration.toolStartModeStandOutId), null);
            toolParamsTable.setText(Configuration.toolStartModePedalActionId, Localization.getResStr(Configuration.toolStartModePedalActionId), null);

            ToolTipIcons.SetToolTip(iconToolSettings, Localization.getResStr(Configuration.toolTabHintId));
            tabPages.lblSelectedTool.Text = Localization.getResStr(Configuration.toolSelectedToolId) + ": " + toolCurTool.ToString();
        }

        private void onToolSettingsPort_CheckedChanged(Control ctrl)
        {
            RadioButton rb = (RadioButton)ctrl;
            int iClickedPort = Configuration.myGetRadioButtonPortNbr(rb);
            if (rb.Checked)
            {
                // change image for selected port
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("Port" + iClickedPort.ToString() + "miniSelected");
                toolCurPort = (Port)System.Enum.Parse(typeof(Port), "NUM_" + iClickedPort.ToString());

                // change background image for connected tool to selected port
                // previous connected tool
                if (toolConnectedToolPort != GenericStationTools.NO_TOOL)
                {
                    Control rbControl = (Control)rb;
                    if (Configuration.myControlExists(this, "rbToolSettings_" + toolConnectedToolPort.ToString(), ref rbControl))
                    {
                        rbControl.BackgroundImage = null;
                    }
                    rb = (RadioButton)rbControl;
                }
                // show port in connected tool
                toolConnectedToolPort = jbc.GetPortToolID(myID, toolCurPort);
                if (toolConnectedToolPort != GenericStationTools.NO_TOOL)
                {
                    Control rbControl = (Control)rb;
                    if (Configuration.myControlExists(this, "rbToolSettings_" + toolConnectedToolPort.ToString(), ref rbControl))
                    {
                        //CType(rb, RadioButton).BackgroundImage = My.Resources.ToolConnected_mini
                        rbControl.BackgroundImage = (Image)My.Resources.Resources.ResourceManager.GetObject("ToolConnected_mini_" + iClickedPort.ToString());
                    }
                    rb = (RadioButton)rbControl;
                }
                m_toolSettingsParams = true;
            }
            else
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("Port" + iClickedPort.ToString() + "mini");
            }
            rb.Refresh();
        }

        private void onToolSettingsTool_CheckedChanged(Control ctrl)
        {
            RadioButton rb = (RadioButton)ctrl;
            if (rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(Configuration.myGetRadioButtonToolName(rb) + "_miniSelected");
                toolCurTool = (GenericStationTools)System.Enum.Parse(typeof(GenericStationTools), Configuration.myGetRadioButtonToolName(rb));
                if (features.AllToolsSamePortSettings)
                {
                    //tabPages.lblSelectedTool.Text = getResStr(toolAllSameSettingsId)
                    tabPages.lblSelectedTool.Text = "";
                }
                else
                {
                    tabPages.lblSelectedTool.Text = Localization.getResStr(Configuration.toolSelectedToolId) + ": " + toolCurTool.ToString();
                }
                m_toolSettingsParams = true;
            }
            else
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(Configuration.myGetRadioButtonToolName(rb) + "_mini");
            }
            rb.Refresh();
        }

        private async void updateToolSettingsPage()
        {
            // When mode has been changed
            if (mode == (int)ControlModeConnection.MONITOR)
            {
                toolParamsTable.inputControlsEnable(false);
            }
            else if (mode == (int)ControlModeConnection.CONTROL)
            {
                toolParamsTable.inputControlsEnable(true);
            }

            // if station data is available
            if (jbc.GetPortCount(myID) > 0)
            {

                // update availabel tools and ports
                if (!bAlreadySetConnectedToolsAndPorts)
                {

                    //Setting initial tool and port
                    // port
                    toolCurPort = Port; // same default work port
                                        // tool
                    GenericStationTools[] stnTools = jbc.GetStationTools(myID);
                    if (jbc.GetPortToolID(myID, toolCurPort) == GenericStationTools.NO_TOOL)
                    {
                        toolCurTool = stnTools[0]; // set the first supported tool
                    }
                    else
                    {
                        toolCurTool = jbc.GetPortToolID(myID, toolCurPort); // set the connected tool
                    }

                    // select radio buttons
                    Control radio = null;
                    if (Configuration.myControlExists(this, "rbPortTools_" + toolCurPort.ToString().Replace("NUM_", ""), ref radio))
                    {
                        ((RadioButton)radio).Checked = true;
                    }
                    if (Configuration.myControlExists(this, "rbToolSettings_" + toolCurTool.ToString(), ref radio))
                    {
                        ((RadioButton)radio).Checked = true;
                    }

                    // disable unsupported ports
                    int i = jbc.GetPortCount(myID) + 1;
                    while (i <= Configuration.MAX_PORTS)
                    {
                        if (Configuration.myControlExists(this, "rbPortTools_" + i.ToString(), ref radio))
                        {
                            ((RadioButton)radio).Enabled = false;
                        }
                        i++;
                    }

                    // disable unsupported tools
                    foreach (GenericStationTools tool in System.Enum.GetValues(typeof(GenericStationTools)))
                    {
                        if (tool != GenericStationTools.NO_TOOL)
                        {
                            // 19/07/2014 Tool settings para DME_TCH_01: mismos settings para todas las herramientas de un puerto
                            if (!(Array.IndexOf(stnTools, tool) >= 0) || features.AllToolsSamePortSettings)
                            {
                                if (Configuration.myControlExists(this, "rbToolSettings_" + tool.ToString(), ref radio))
                                {
                                    ((RadioButton)radio).Enabled = false;
                                }
                            }
                        }
                    }

                    bAlreadySetConnectedToolsAndPorts = true;
                    m_toolSettingsParams = true;
                }
            }

            //If update necessary
            if (m_toolSettingsParams)
            {

                // Retrieve remote data
                await jbc.UpdatePortToolSettingsAsync(myID, toolCurPort, toolCurTool);

                //toolParamsTable.setValue(toolSelectedTempId, convertTempToString(jbc.GetPortToolSelectedTemp(myID, toolCurPort)))
                CTemperature temp = default(CTemperature);

                // levels
                if (features.TempLevels)
                {
                    // selected level
                    toolParamsTable.setValue(Configuration.toolSelectedTempLvlId, Convert.ToString(jbc.GetPortToolSelectedTempLevels(myID, toolCurPort, toolCurTool)));
                    if (features.TempLevelsWithStatus)
                    {
                        toolParamsTable.setCheck(Configuration.toolSelectedTempLvlId, System.Convert.ToBoolean(jbc.GetPortToolSelectedTempLevelsEnabled(myID, toolCurPort, toolCurTool)));
                    }
                    //Debug.Print("TextId: {0}, Data: {1}", toolSelectedTempLvlId, jbc.GetPortToolSelectedTempLevels(myID, toolCurPort, toolCurTool))

                    // level 1 temp
                    temp = jbc.GetPortToolTempLevel(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.FIRST_LEVEL);
                    if (temp.isValid())
                    {
                        toolParamsTable.setValue(Configuration.toolTempLvl1Id, Configuration.convertTempToString(temp, false, true));
                    }
                    else
                    {
                        toolParamsTable.setValue(Configuration.toolTempLvl1Id, Configuration.noDataStr);
                    }
                    if (features.TempLevelsWithStatus)
                    {
                        toolParamsTable.setCheck(Configuration.toolTempLvl1Id, System.Convert.ToBoolean(jbc.GetPortToolTempLevelEnabled(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.FIRST_LEVEL)));
                    }

                    // level 2 temp
                    temp = jbc.GetPortToolTempLevel(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.SECOND_LEVEL);
                    if (temp.isValid())
                    {
                        toolParamsTable.setValue(Configuration.toolTempLvl2Id, Configuration.convertTempToString(temp, false, true));
                    }
                    else
                    {
                        toolParamsTable.setValue(Configuration.toolTempLvl2Id, Configuration.noDataStr);
                    }
                    if (features.TempLevelsWithStatus)
                    {
                        toolParamsTable.setCheck(Configuration.toolTempLvl2Id, System.Convert.ToBoolean(jbc.GetPortToolTempLevelEnabled(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.SECOND_LEVEL)));
                    }

                    // level 3 temp
                    temp = jbc.GetPortToolTempLevel(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.THIRD_LEVEL);
                    if (temp.isValid())
                    {
                        toolParamsTable.setValue(Configuration.toolTempLvl3Id, Configuration.convertTempToString(temp, false, true));
                    }
                    else
                    {
                        toolParamsTable.setValue(Configuration.toolTempLvl3Id, Configuration.noDataStr);
                    }
                    if (features.TempLevelsWithStatus)
                    {
                        toolParamsTable.setCheck(Configuration.toolTempLvl3Id, System.Convert.ToBoolean(jbc.GetPortToolTempLevelEnabled(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.THIRD_LEVEL)));
                    }

                }

                //toolParamsTable.setValue(toolSleepTempId, convertTempToString(jbc.GetPortToolSleepTemp(myID, toolCurPort, toolCurTool), False, True))
                //toolParamsTable.setValue(toolSleepDelayId, jbc.GetPortToolSleepDelay(myID, toolCurPort, toolCurTool))
                //If features.DelayWithStatus Then toolParamsTable.setCheck(toolSleepDelayId, jbc.GetPortToolSleepDelayEnabled(myID, toolCurPort, toolCurTool))
                //toolParamsTable.setValue(toolHibernationDelayId, jbc.GetPortToolHibernationDelay(myID, toolCurPort, toolCurTool))
                //If features.DelayWithStatus Then toolParamsTable.setCheck(toolHibernationDelayId, jbc.GetPortToolHibernationDelayEnabled(myID, toolCurPort, toolCurTool))
                // do not convert UTI->Celsius or Fahrenheit if UTI = 0
                toolParamsTable.setValue(Configuration.toolProfilesId, Convert.ToString(jbc.GetPortToolProfileMode(myID, toolCurPort)));

                temp = jbc.GetPortToolAdjustTemp(myID, toolCurPort, toolCurTool);
                if (temp.UTI == 0)
                {
                    toolParamsTable.setValue(Configuration.toolAdjustTempId, "0");
                }
                else
                {
                    toolParamsTable.setValue(Configuration.toolAdjustTempId, Configuration.convertTempAdjToString(temp, false));
                }

                toolParamsTable.setValue(Configuration.toolTimeToStopId, Configuration.convertMinuteTimeToString(System.Convert.ToInt32(jbc.GetPortToolTimeToStop(myID, toolCurPort, toolCurTool) / 60))); //el resultado viene en segundos y lo expresamos en minutos
                toolParamsTable.setValue(Configuration.toolExtTCModeId, Convert.ToString(jbc.GetPortToolExternalTCMode(myID, toolCurPort, toolCurTool)));
                toolParamsTable.setValue(Configuration.toolExtTCTempId, Configuration.convertTempToString(jbc.GetPortToolSelectedExtTemp(myID, toolCurPort), false, true));
                toolParamsTable.setValue(Configuration.toolStartModeToolButtonId, Convert.ToString(jbc.GetPortToolStartMode_ToolButton(myID, toolCurPort, toolCurTool)));
                toolParamsTable.setValue(Configuration.toolStartModeStandOutId, Convert.ToString(jbc.GetPortToolStartMode_StandOut(myID, toolCurPort, toolCurTool)));
                toolParamsTable.setValue(Configuration.toolStartModePedalActionId, Convert.ToString(jbc.GetPortToolStartMode_PedalAction(myID, toolCurPort, toolCurTool)));


                //Update done
                m_toolSettingsParams = false;
            }
        }

        private async void toolParamsTable_NewValue(string name, string value)
        {
            //Some value has been changed, sending it to the station
            //If name = "toolSelectedTemp" Then jbc.SetPortToolSelectedTemp(myID, toolCurPort, convertStringToTemp(value))
            if (name == Configuration.toolSelectedTempLvlId)
            {
                await jbc.SetPortToolSelectedTempLevelsAsync(myID, toolCurPort, toolCurTool, (ToolTemperatureLevels)(int.Parse(value)));
                WorkParams = true; // update work page also
            }
            if (name == Configuration.toolTempLvl1Id)
            {
                if (value == "" || value == "0")
                {
                    value = Configuration.noDataStr;
                }
                await jbc.SetPortToolTempLevelAsync(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.FIRST_LEVEL, Configuration.convertStringToTemp(value));
                WorkParams = true; // update work page also
            }
            if (name == Configuration.toolTempLvl2Id)
            {
                if (value == "" || value == "0")
                {
                    value = Configuration.noDataStr;
                }
                await jbc.SetPortToolTempLevelAsync(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.SECOND_LEVEL, Configuration.convertStringToTemp(value));
                WorkParams = true; // update work page also
            }
            if (name == Configuration.toolTempLvl3Id)
            {
                if (value == "" || value == "0")
                {
                    value = Configuration.noDataStr;
                }
                await jbc.SetPortToolTempLevelAsync(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.THIRD_LEVEL, Configuration.convertStringToTemp(value));
                WorkParams = true; // update work page also
            }
            //If name = toolSleepTempId Then Await jbc.SetPortToolSleepTempAsync(myID, toolCurPort, toolCurTool, convertStringToTemp(value))
            //If name = toolSleepDelayId Then Await jbc.SetPortToolSleepDelayAsync(myID, toolCurPort, toolCurTool, value)
            //If name = toolHibernationDelayId Then Await jbc.SetPortToolHibernationDelayAsync(myID, toolCurPort, toolCurTool, value)
            if (name == Configuration.toolProfilesId)
            {
                if (value != "")
                {
                    await jbc.SetPortToolProfileModeAsync(myID, toolCurPort, OnOff._ON);
                }
                else
                {
                    await jbc.SetPortToolProfileModeAsync(myID, toolCurPort, OnOff._OFF);
                }
            }
            if (name == Configuration.toolAdjustTempId)
            {
                CTemperature tempAdj = Configuration.convertStringToTempAdj(value);
                await jbc.SetPortToolAdjustTempAsync(myID, toolCurPort, toolCurTool, tempAdj);
            }
            if (name == Configuration.toolTimeToStopId)
            {
                await jbc.SetPortToolTimeToStopAsync(myID, toolCurPort, toolCurTool, System.Convert.ToInt32(double.Parse(value) * 60)); //el usuario introduce los minutos pero se guarda en segundos
            }
            if (name == Configuration.toolExtTCModeId)
            {
                await jbc.SetPortToolExternalTCModeAsync(myID, toolCurPort, toolCurTool, (ToolExternalTCMode_HA)int.Parse(value));
            }
            if (name == Configuration.toolExtTCTempId)
            {
                await jbc.SetPortToolSelectedExtTempAsync(myID, toolCurPort, Configuration.convertStringToTemp(value));
            }
            if (name == Configuration.toolStartModeToolButtonId)
            {
                await jbc.SetPortToolStartModeAsync(myID,
                                                    toolCurPort,
                                                    toolCurTool,
                                                    (OnOff)int.Parse(value),
                                                    jbc.GetPortToolStartMode_StandOut(myID, toolCurPort, toolCurTool),
                                                    jbc.GetPortToolStartMode_PedalAction(myID, toolCurPort, toolCurTool));
            }
            if (name == Configuration.toolStartModeStandOutId)
            {
                await jbc.SetPortToolStartModeAsync(myID,
                                                    toolCurPort,
                                                    toolCurTool,
                                                    jbc.GetPortToolStartMode_ToolButton(myID, toolCurPort, toolCurTool),
                                                    (OnOff)int.Parse(value),
                                                    jbc.GetPortToolStartMode_PedalAction(myID, toolCurPort, toolCurTool));
            }
            if (name == Configuration.toolStartModePedalActionId)
            {
                await jbc.SetPortToolStartModeAsync(myID,
                                                    toolCurPort,
                                                    toolCurTool,
                                                    jbc.GetPortToolStartMode_ToolButton(myID, toolCurPort, toolCurTool),
                                                    jbc.GetPortToolStartMode_StandOut(myID, toolCurPort, toolCurTool),
                                                    (PedalAction)int.Parse(value));
            }

            //Updating
            m_toolSettingsParams = true;
        }

        private async void toolParamsTable_CheckedChanged(string name, bool value)
        {
            //Some value has been changed, sending it to the station
            // check boxes of temp levels
            OnOff onoff = default(OnOff);
            if (value)
            {
                onoff = OnOff._ON;
            }
            else
            {
                onoff = OnOff._OFF;
            }
            if (name == Configuration.toolSelectedTempLvlId)
            {
                await jbc.SetPortToolSelectedTempLevelsEnabledAsync(myID, toolCurPort, toolCurTool, onoff);
                WorkParams = true; // update work page also
            }
            if (name == Configuration.toolTempLvl1Id)
            {
                await jbc.SetPortToolTempLevelEnabledAsync(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.FIRST_LEVEL, onoff);
                WorkParams = true; // update work page also
            }
            if (name == Configuration.toolTempLvl2Id)
            {
                await jbc.SetPortToolTempLevelEnabledAsync(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.SECOND_LEVEL, onoff);
                WorkParams = true; // update work page also
            }
            if (name == Configuration.toolTempLvl3Id)
            {
                await jbc.SetPortToolTempLevelEnabledAsync(myID, toolCurPort, toolCurTool, ToolTemperatureLevels.THIRD_LEVEL, onoff);
                WorkParams = true; // update work page also
            }
            //If name = toolSleepDelayId Then
            //    Await jbc.SetPortToolSleepDelayEnabledAsync(myID, toolCurPort, toolCurTool, onoff)
            //End If
            //If name = toolHibernationDelayId Then
            //    Await jbc.SetPortToolHibernationDelayEnabledAsync(myID, toolCurPort, toolCurTool, onoff)
            //End If

            //Updating
            m_toolSettingsParams = true;
        }

        private async void toolParamsTable_ButtonClicked(string name, string value)
        {
            //Some value is clicked (defined as button to be processed out of the paramater list)
            //If name = toolCartridgeId Then
            //    ' create and show dialog for cartridge form
            //    Dim iSelected As Integer
            //    Try
            //        iSelected = CInt(value)
            //    Catch ex As Exception
            //        iSelected = 0
            //    End Try
            //    Dim frmCart As frmCartridges = New frmCartridges(toolCurTool, iSelected, jbc.GetStationModel(myID))
            //    If mode = ControlModeConnection.CONTROL Then
            //        frmCart.butOk.Enabled = True
            //    Else
            //        frmCart.butOk.Enabled = False
            //    End If

            //    tmr.Stop()
            //    If frmCart.ShowDialog() = Windows.Forms.DialogResult.OK Then
            //        Try
            //            iSelected = CInt(frmCart.tbCartridgeNumber.Text)
            //            Dim onoff As OnOff = onoff._OFF
            //            If toolParamsTable.getCheck(toolCartridgeId) Then onoff = onoff._ON
            //            Await jbc.SetPortToolCartridgeAsync(myID, toolCurPort, toolCurTool, onoff, iSelected)
            //            'Updating
            //            toolSettingsParams = True
            //        Catch ex As Exception
            //        End Try
            //    End If
            //    frmCart.Dispose()
            //    tmr.Start()

            //End If
        }

        #endregion

        #region PROFILES PAGE

        private ProfileOptions m_profileOptions;
        private ProfileParameters m_profileParameters;
        private JObject m_jsonParse = null;
        private int m_profileSelected;
        private string m_profileSelectedName;
        private int m_pointSelected;
        private ProfileRegulationMode m_profileRegulationMode;
        //Axis
        private CategoricalAxis m_timeAxis = new CategoricalAxis();
        //Series
        private BindingList<DataSerie> m_dataProfileHotAirTemp = new BindingList<DataSerie>();
        private BindingList<DataSerie> m_dataProfileExtTCTemp = new BindingList<DataSerie>();
        private BindingList<DataSerie> m_dataProfileAirFlow = new BindingList<DataSerie>();


        private async void CreateProfilesPage()
        {

            // Retrieve remote data
            await jbc.UpdateProfilesAsync(myID);
            await jbc.UpdateSelectedProfileAsync(myID);

            //Options panel
            m_profileOptions = new ProfileOptions();
            m_profileOptions.ProfileNew += ProfileNew;
            m_profileOptions.ProfileEdit += ProfileEdit;
            m_profileOptions.ProfileDelete += ProfileDelete;
            m_profileOptions.ProfileCopy += ProfileCopy;
            m_profileOptions.ProfileSync += ProfileSync;
            m_profileOptions.AddPoint += AddNewPoint;
            m_profileOptions.RemovePoint += RemovePoint;
            m_profileOptions.SaveEditedProfile += SaveEditedProfile;
            m_profileOptions.CancelEditedProfile += CancelEditedProfile;
            tabPages.panelProfilesOptions.Controls.Add(m_profileOptions);

            //Profile parameters
            m_profileParameters = new ProfileParameters();
            m_profileParameters.ProfileSelectMinus += MinusProfileSelector;
            m_profileParameters.ProfileSelectPlus += PlusProfileSelector;
            m_profileParameters.PointSelectMinus += MinusPointSelector;
            m_profileParameters.PointSelectPlus += PlusPointSelector;
            m_profileParameters.TemperatureMinus += TemperatureMinus;
            m_profileParameters.TemperaturePlus += TemperaturePlus;
            m_profileParameters.AirFlowMinus += AirFlowMinus;
            m_profileParameters.AirFlowPlus += AirFlowPlus;
            m_profileParameters.TimeMinus += TimeMinus;
            m_profileParameters.TimePlus += TimePlus;
            tabPages.panelProfilesParameters.Controls.Add(m_profileParameters);


            m_profileSelected = 0;
            m_profileSelectedName = jbc.GetProfileName(myID, m_profileSelected);
            m_pointSelected = 0;

            //
            //Initialize axis
            //

            //Horizontal axis - Time
            m_timeAxis.LabelFitMode = AxisLabelFitMode.MultiLine;
            m_timeAxis.PlotMode = AxisPlotMode.OnTicks;
            m_timeAxis.ShowLabels = true;
            m_timeAxis.LabelFormatProvider = new LabelTimeFormat();
            m_timeAxis.LabelOffset = 1; //no mostrar el primer label

            //Vertical axis - Temperature
            LinearAxis tempAxis = new LinearAxis();
            tempAxis.AxisType = AxisType.Second;
            tempAxis.Title = "Temp ºC";
            tempAxis.Maximum = 500;
            tempAxis.Minimum = 0;
            tempAxis.MajorStep = 100;

            //Vertical axis - Power
            LinearAxis powerAxis = new LinearAxis();
            powerAxis.HorizontalLocation = AxisHorizontalLocation.Right;
            powerAxis.AxisType = AxisType.Second;
            powerAxis.Title = "Power %";
            powerAxis.Maximum = 100;
            powerAxis.Minimum = 0;
            powerAxis.MajorStep = 20;

            //
            //Initialize grid area
            //

            //Adjust margins
            tabPages.panelProfilesPlot.View.Margin = new Padding(0);

            CartesianArea area = tabPages.panelProfilesPlot.GetArea<CartesianArea>();
            CartesianGrid grid = area.GetGrid<CartesianGrid>();
            grid.DrawHorizontalStripes = true;
            grid.DrawHorizontalFills = false;
            grid.DrawVerticalStripes = true;
            grid.DrawVerticalFills = false;
            grid.ForeColor = Color.DarkGray;

            //
            //Initialize series
            //

            //Profile hot air temp
            FastLineSeries serieProfileHotAirTemp = new FastLineSeries();
            serieProfileHotAirTemp.Name = "Profile hot air temperature";
            serieProfileHotAirTemp.BorderColor = Color.DarkRed;
            serieProfileHotAirTemp.PointSize = new SizeF(0, 0);
            serieProfileHotAirTemp.CategoryMember = "Time";
            serieProfileHotAirTemp.ValueMember = "Value";
            serieProfileHotAirTemp.DataSource = m_dataProfileHotAirTemp;
            serieProfileHotAirTemp.BorderWidth = 1;
            serieProfileHotAirTemp.HorizontalAxis = m_timeAxis;
            serieProfileHotAirTemp.VerticalAxis = tempAxis;
            tabPages.panelProfilesPlot.Series.Add(serieProfileHotAirTemp);
            //Profile ext TC temp
            FastLineSeries serieProfileExtTCTemp = new FastLineSeries();
            serieProfileExtTCTemp.Name = "Profile ext TC temperature";
            serieProfileExtTCTemp.BorderColor = Color.DarkGreen;
            serieProfileExtTCTemp.PointSize = new SizeF(0, 0);
            serieProfileExtTCTemp.CategoryMember = "Time";
            serieProfileExtTCTemp.ValueMember = "Value";
            serieProfileExtTCTemp.DataSource = m_dataProfileExtTCTemp;
            serieProfileExtTCTemp.BorderWidth = 1;
            serieProfileExtTCTemp.HorizontalAxis = m_timeAxis;
            serieProfileExtTCTemp.VerticalAxis = tempAxis;
            tabPages.panelProfilesPlot.Series.Add(serieProfileExtTCTemp);
            //Profile air flow
            FastLineSeries serieProfileAirFlow = new FastLineSeries();
            serieProfileAirFlow.Name = "Profile air flow";
            serieProfileAirFlow.BorderColor = Color.RoyalBlue;
            serieProfileAirFlow.PointSize = new SizeF(0, 0);
            serieProfileAirFlow.CategoryMember = "Time";
            serieProfileAirFlow.ValueMember = "Value";
            serieProfileAirFlow.DataSource = m_dataProfileAirFlow;
            serieProfileAirFlow.BorderWidth = 1;
            serieProfileAirFlow.HorizontalAxis = m_timeAxis;
            serieProfileAirFlow.VerticalAxis = powerAxis;
            tabPages.panelProfilesPlot.Series.Add(serieProfileAirFlow);

            LoadJsonSelectedProfile();
            LoadDataProfile();
        }

        private void updateProfilesPage()
        {

            // When mode has been changed
            if (mode == (int)ControlModeConnection.MONITOR)
            {
                m_profileOptions.butProfilesNew.Enabled = false;
                m_profileOptions.butProfilesEdit.Enabled = false;
                m_profileOptions.butProfilesDelete.Enabled = false;
                m_profileOptions.butProfilesCopy.Enabled = false;

                m_profileOptions.butAddPoint.Enabled = false;
                m_profileOptions.butRemovePoint.Enabled = false;
                m_profileOptions.butSaveEditedProfile.Enabled = false;

                m_profileParameters.butTemperatureMinus.Enabled = false;
                m_profileParameters.butTemperaturePlus.Enabled = false;
                m_profileParameters.butAirFlowMinus.Enabled = false;
                m_profileParameters.butAirFlowPlus.Enabled = false;
                m_profileParameters.butTimeMinus.Enabled = false;
                m_profileParameters.butTimePlus.Enabled = false;

            }
            else if (mode == (int)ControlModeConnection.CONTROL)
            {
                m_profileOptions.butProfilesNew.Enabled = true;
                m_profileOptions.butProfilesEdit.Enabled = true;
                m_profileOptions.butProfilesDelete.Enabled = true;
                m_profileOptions.butProfilesCopy.Enabled = true;

                m_profileOptions.butAddPoint.Enabled = true;
                m_profileOptions.butRemovePoint.Enabled = true;
                m_profileOptions.butSaveEditedProfile.Enabled = true;

                m_profileParameters.butTemperatureMinus.Enabled = true;
                m_profileParameters.butTemperaturePlus.Enabled = true;
                m_profileParameters.butAirFlowMinus.Enabled = true;
                m_profileParameters.butAirFlowPlus.Enabled = true;
                m_profileParameters.butTimeMinus.Enabled = true;
                m_profileParameters.butTimePlus.Enabled = true;
            }
        }

        private void LoadJsonSelectedProfile()
        {
            byte[] profileDataByte = jbc.GetProfile(myID, m_profileSelectedName);
            if (ReferenceEquals(profileDataByte, null))
            {
                return;
            }

            string profileData = System.Text.Encoding.ASCII.GetString(profileDataByte);
            ///'''''''''''''''''''''
            //TEST
            if (m_profileSelected % 2 == 0)
            {
                profileData = "{'_Type':'PROFILE','_V':1,'_':{'Name':'','Desc':'','Mode':0,'Points':[{'ATemp':1350,'ETemp1':0,'ETemp2':0,'AFlow':1000,'Time':200},{'ATemp':2700,'ETemp1':0,'ETemp2':0,'AFlow':1000,'Time':200},{'ATemp':2700,'ETemp1':0,'ETemp2':0,'AFlow':1000,'Time':200}]}}";
            }
            else
            {
                profileData = "{'_Type':'PROFILE','_V':1,'_':{'Name':'','Desc':'','Mode':1,'Points':[{'ATemp':0,'ETemp1':1800,'ETemp2':0,'AFlow':1000,'Time':200},{'ATemp':0,'ETemp1':1350,'ETemp2':0,'AFlow':700,'Time':200},{'ATemp':0,'ETemp1':900,'ETemp2':0,'AFlow':1000,'Time':200}]}}";
            }
            ///'''''''''''''''''''''
            try
            {
                m_jsonParse = JObject.Parse(profileData);
            }
            catch (Exception)
            {
                //TODO . show error message
            }
        }

        private void LoadDataProfile()
        {
            //Si el modo de regulación del perfil está en Air temp, el valor de ext TC es 0
            //Si el modo de regulación del perfil está en Ext TC temp, el valor de Air temp es 0

            if (ReferenceEquals(m_jsonParse, null))
            {
                return;
            }

            m_dataProfileHotAirTemp.Clear();
            m_dataProfileExtTCTemp.Clear();
            m_dataProfileAirFlow.Clear();

            JToken points = m_jsonParse["_"]["Points"];
            m_profileRegulationMode = (ProfileRegulationMode)(System.Convert.ToInt32(m_jsonParse["_"]["Mode"]));

            int windowDivisions = 0;
            int windowTime = 0;

            foreach (var point in points)
            {
                windowDivisions++;
                windowTime += System.Convert.ToInt32(point["Time"]);
            }

            int tick = 0;
            double iniHotAirTemp = 0;
            double endHotAirTemp = 0;
            double iniExtTCTemp = 0;
            double endExtTCTemp = 0;
            double iniAirFlow = 0;
            double endAirFlow = 0;

            foreach (var point in points)
            {
                //Valores del inicio del step
                if (tick == 0)
                {
                    if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
                    {
                        iniHotAirTemp = System.Convert.ToDouble(point["ATemp"]);
                        iniExtTCTemp = -1;
                    }
                    else
                    {
                        iniHotAirTemp = -1;
                        iniExtTCTemp = System.Convert.ToDouble(point["ETemp1"]);
                    }
                    iniAirFlow = System.Convert.ToDouble(point["AFlow"]);

                    //Punto inicial
                    DataSerie objDataSerie = new DataSerie();
                    objDataSerie.Value = CTemperature.ToCelsius(System.Convert.ToInt32(iniHotAirTemp));
                    objDataSerie.Time = tick;
                    m_dataProfileHotAirTemp.Add(objDataSerie);

                    objDataSerie = new DataSerie();
                    objDataSerie.Value = CTemperature.ToCelsius(System.Convert.ToInt32(iniExtTCTemp));
                    objDataSerie.Time = tick;
                    m_dataProfileExtTCTemp.Add(objDataSerie);

                    objDataSerie = new DataSerie();
                    objDataSerie.Value = iniAirFlow / 10;
                    objDataSerie.Time = tick;
                    m_dataProfileAirFlow.Add(objDataSerie);

                    tick = 1;
                }
                else
                {
                    iniHotAirTemp = endHotAirTemp;
                    iniExtTCTemp = endExtTCTemp;
                    iniAirFlow = endAirFlow;
                }

                //Valores del final del step
                if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
                {
                    endHotAirTemp = System.Convert.ToDouble(point["ATemp"]);
                    endExtTCTemp = -1;
                }
                else
                {
                    endHotAirTemp = -1;
                    endExtTCTemp = System.Convert.ToDouble(point["ETemp1"]);
                }
                endAirFlow = System.Convert.ToDouble(point["AFlow"]);

                //Duración del step
                int stepTime = System.Convert.ToInt32(point["Time"]);

                //Calcular el incremento para cada punto del actual step
                double slopeHotAirTemp = (endHotAirTemp - iniHotAirTemp) / stepTime;
                double slopeExtTCTemp = (endExtTCTemp - iniExtTCTemp) / stepTime;
                double slopeAirFlow = (endAirFlow - iniAirFlow) / stepTime;

                //Valores temporales para los puntos
                double tmpHotAirTemp = iniHotAirTemp;
                double tmpExtTCTemp = iniExtTCTemp;
                double tmpAirFlow = iniAirFlow;

                for (int i = 1; i <= System.Convert.ToInt32(point["Time"]); i++)
                {
                    //Incrementar los valores para cada tick
                    tmpHotAirTemp += slopeHotAirTemp;
                    tmpExtTCTemp += slopeExtTCTemp;
                    tmpAirFlow += slopeAirFlow;

                    //Guardar los puntos calculados de la recta
                    DataSerie objDataSerie = new DataSerie();
                    objDataSerie.Value = CTemperature.ToCelsius(System.Convert.ToInt32(tmpHotAirTemp));
                    objDataSerie.Time = tick;
                    m_dataProfileHotAirTemp.Add(objDataSerie);

                    objDataSerie = new DataSerie();
                    objDataSerie.Value = CTemperature.ToCelsius(System.Convert.ToInt32(tmpExtTCTemp));
                    objDataSerie.Time = tick;
                    m_dataProfileExtTCTemp.Add(objDataSerie);

                    objDataSerie = new DataSerie();
                    objDataSerie.Value = tmpAirFlow / 10;
                    objDataSerie.Time = tick;
                    m_dataProfileAirFlow.Add(objDataSerie);

                    tick++;
                }
            }

            //Horizontal axis - Time
            if (windowTime > 0 & windowDivisions > 0)
            {
                m_timeAxis.MajorTickInterval = windowTime / windowDivisions;
            }
            else
            {
                m_timeAxis.MajorTickInterval = 1;
            }

            //Profile parameters
            m_profileParameters.labelProfileTotal.Text = System.Convert.ToString(jbc.GetProfileCount(myID));
            m_profileParameters.labelProfileSelected.Text = System.Convert.ToString(m_profileSelected + 1);
            m_profileParameters.labelProfileName.Text = m_profileSelectedName;
            if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
            {
                m_profileParameters.labelProfileMode.Text = "Air temp";
            }
            else
            {
                m_profileParameters.labelProfileMode.Text = "Ext TC";
            }
            m_profileParameters.labelProfileDuration.Text = LabelTimeFormat.Format(windowTime);
            //Profile parameters - Edit
            m_profileParameters.labelProfileNameEdit.Text = m_profileSelectedName + "/" + m_profileParameters.labelProfileMode.Text;
            m_profileParameters.labelPointTotal.Text = System.Convert.ToString(points.Count());
            m_profileParameters.labelPointSelected.Text = System.Convert.ToString(m_pointSelected + 1);
            if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
            {
                m_profileParameters.labelTemperature.Text = System.Convert.ToString(CTemperature.ToCelsius(System.Convert.ToInt32(points[m_pointSelected]["ATemp"])));
            }
            else
            {
                m_profileParameters.labelTemperature.Text = System.Convert.ToString(CTemperature.ToCelsius(System.Convert.ToInt32(points[m_pointSelected]["ETemp1"])));
            }
            m_profileParameters.labelAirFlow.Text = System.Convert.ToString((System.Convert.ToInt32(points[m_pointSelected]["AFlow"])) / 10);
            m_profileParameters.labelTime.Text = LabelTimeFormat.Format(System.Convert.ToInt32(points[m_pointSelected]["Time"]));
        }

        #region Profile parameters

        #region Navigation

        private void MinusProfileSelector()
        {
            m_profileSelected--;
            if (m_profileSelected < 0)
            {
                m_profileSelected = jbc.GetProfileCount(myID) - 1;
            }
            m_profileSelectedName = jbc.GetProfileName(myID, m_profileSelected);

            LoadJsonSelectedProfile();
            LoadDataProfile();
        }

        private void PlusProfileSelector()
        {
            m_profileSelected++;
            if (m_profileSelected >= jbc.GetProfileCount(myID))
            {
                m_profileSelected = 0;
            }
            m_profileSelectedName = jbc.GetProfileName(myID, m_profileSelected);

            LoadJsonSelectedProfile();
            LoadDataProfile();
        }

        private void MinusPointSelector()
        {
            m_pointSelected--;
            if (m_pointSelected < 0)
            {
                m_pointSelected = System.Convert.ToInt32(m_jsonParse["_"]["Points"].Count() - 1);
            }

            LoadDataProfile();
        }

        private void PlusPointSelector()
        {
            m_pointSelected++;
            if (m_pointSelected >= m_jsonParse["_"]["Points"].Count())
            {
                m_pointSelected = 0;
            }

            LoadDataProfile();
        }

        #endregion

        #region Minus/plus parameters

        private void TemperatureMinus()
        {
            CTemperature temp = default(CTemperature);
            CTemperature minTemp = default(CTemperature);
            CTemperature maxTemp = default(CTemperature);

            if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
            {
                temp = new CTemperature(System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["ATemp"]));
                minTemp = jbc.GetStationMinTemp(myID);
                maxTemp = jbc.GetStationMaxTemp(myID);
            }
            else
            {
                temp = new CTemperature(System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["ETemp1"]));
                minTemp = jbc.GetStationMinExtTemp(myID);
                maxTemp = jbc.GetStationMaxExtTemp(myID);
            }

            if (temp.UTI > minTemp.UTI)
            {
                temp.StepCelsious(-1);
            }
            else
            {
                temp.UTI = maxTemp.UTI;
            }

            if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
            {
                m_jsonParse["_"]["Points"][m_pointSelected]["ATemp"] = temp.UTI;
            }
            else
            {
                m_jsonParse["_"]["Points"][m_pointSelected]["ETemp1"] = temp.UTI;
            }

            LoadDataProfile();
        }

        private void TemperaturePlus()
        {
            CTemperature temp = default(CTemperature);
            CTemperature minTemp = default(CTemperature);
            CTemperature maxTemp = default(CTemperature);

            if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
            {
                temp = new CTemperature(System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["ATemp"]));
                minTemp = jbc.GetStationMinTemp(myID);
                maxTemp = jbc.GetStationMaxTemp(myID);
            }
            else
            {
                temp = new CTemperature(System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["ETemp1"]));
                minTemp = jbc.GetStationMinExtTemp(myID);
                maxTemp = jbc.GetStationMaxExtTemp(myID);
            }

            if (temp.UTI < maxTemp.UTI)
            {
                temp.StepCelsious(1);
            }
            else
            {
                temp.UTI = minTemp.UTI;
            }

            if (m_profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
            {
                m_jsonParse["_"]["Points"][m_pointSelected]["ATemp"] = temp.UTI;
            }
            else
            {
                m_jsonParse["_"]["Points"][m_pointSelected]["ETemp1"] = temp.UTI;
            }

            LoadDataProfile();
        }

        private void AirFlowMinus()
        {
            int flow = System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["AFlow"]);
            if (flow > jbc.GetStationMinFlow(myID))
            {
                flow -= 10;
            }
            else
            {
                flow = jbc.GetStationMaxFlow(myID);
            }
            m_jsonParse["_"]["Points"][m_pointSelected]["AFlow"] = flow;
            LoadDataProfile();
        }

        private void AirFlowPlus()
        {
            int flow = System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["AFlow"]);
            if (flow < jbc.GetStationMaxFlow(myID))
            {
                flow += 10;
            }
            else
            {
                flow = jbc.GetStationMinFlow(myID);
            }
            m_jsonParse["_"]["Points"][m_pointSelected]["AFlow"] = flow;
            LoadDataProfile();
        }

        private void TimeMinus()
        {
            int time = System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["Time"]);
            if (time > 0)
            {
                m_jsonParse["_"]["Points"][m_pointSelected]["Time"] = (System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["Time"])) - 10;
                LoadDataProfile();
            }
        }

        private void TimePlus()
        {
            m_jsonParse["_"]["Points"][m_pointSelected]["Time"] = (System.Convert.ToInt32(m_jsonParse["_"]["Points"][m_pointSelected]["Time"])) + 10;
            LoadDataProfile();
        }

        #endregion

        #endregion

        #region Profile options

        #region Navigation

        private void ProfileNew()
        {
            ProfileNewDialog profileNewDialog = new ProfileNewDialog();

            if (profileNewDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (profileNewDialog.txtboxProfileName.Text == "")
                {
                    MessageBox.Show("Invalid name", "", MessageBoxButtons.OK);
                }
                else
                {
                    m_profileSelectedName = profileNewDialog.txtboxProfileName.Text;
                    if (profileNewDialog.rbModeAirTemp.Checked)
                    {
                        m_profileRegulationMode = ProfileRegulationMode.AIR_TEMP;
                    }
                    else
                    {
                        m_profileRegulationMode = ProfileRegulationMode.EXT_TC_TEMP;
                    }

                    JObject jsonParse = new JObject();
                    JObject jsonProfileData = new JObject();
                    JArray jsonPoints = new JArray();
                    jsonParse.Add("_Type", "PROFILE");
                    jsonParse.Add("_V", "1");
                    jsonParse.Add("_", jsonProfileData);
                    jsonProfileData.Add("Name", m_profileSelectedName);
                    jsonProfileData.Add("Desc", "");
                    jsonProfileData.Add("Mode", Convert.ToString(m_profileRegulationMode));
                    jsonProfileData.Add("Points", jsonPoints);
                    m_jsonParse = jsonParse;

                    AddNewPoint();

                    //Change options
                    m_profileOptions.CurrentPage = "pageProfilesOptionsEdit";
                    m_profileParameters.CurrentPage = "pageProfilesParametersEdit";
                }
            }
        }

        private void ProfileEdit()
        {

            //Change options
            m_profileOptions.CurrentPage = "pageProfilesOptionsEdit";
            m_profileParameters.CurrentPage = "pageProfilesParametersEdit";
        }

        private async void ProfileDelete()
        {
            if (jbc.GetSelectedProfile(myID) == m_profileSelectedName)
            {
                MessageBox.Show("Can not delete profile selected by station", "", MessageBoxButtons.OK);
                return;
            }

            if (MessageBox.Show("Do you want to delete this profile?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Cursor = Cursors.WaitCursor;
                jbc.DeleteProfile(myID, m_profileSelectedName);
                await jbc.UpdateProfilesAsync(myID);
                await jbc.UpdateSelectedProfileAsync(myID);

                m_profileSelected = 0;
                m_profileSelectedName = jbc.GetProfileName(myID, m_profileSelected);

                LoadJsonSelectedProfile();
                LoadDataProfile();
                Cursor = Cursors.Arrow;
            }
        }

        private void ProfileCopy()
        {
            ProfileNewDialog profileNewDialog = new ProfileNewDialog(false);

            if (profileNewDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (profileNewDialog.txtboxProfileName.Text == "")
                {
                    MessageBox.Show("Invalid name", "", MessageBoxButtons.OK);
                }
                else
                {
                    m_profileSelectedName = profileNewDialog.txtboxProfileName.Text;

                    LoadDataProfile();

                    //Change options
                    m_profileOptions.CurrentPage = "pageProfilesOptionsEdit";
                    m_profileParameters.CurrentPage = "pageProfilesParametersEdit";
                }
            }
        }

        private async void ProfileSync()
        {
            Cursor = Cursors.WaitCursor;
            jbc.SyncProfiles(myID);
            await jbc.UpdateProfilesAsync(myID);
            await jbc.UpdateSelectedProfileAsync(myID);

            m_profileSelected = 0;
            m_profileSelectedName = jbc.GetProfileName(myID, m_profileSelected);

            LoadJsonSelectedProfile();
            LoadDataProfile();
            Cursor = Cursors.Arrow;
        }

        #endregion

        #region Edit

        private void AddNewPoint()
        {
            JArray jsonPoints = (JArray)m_jsonParse["_"]["Points"];

            if (jsonPoints.Count >= 16)
            {
                MessageBox.Show("Added maximum number of points", "", MessageBoxButtons.OK);
                return;
            }

            JObject jsonPoint = new JObject();
            if (jsonPoints.Count > 0)
            {
                jsonPoint.Add("ATemp", jsonPoints[jsonPoints.Count - 1]["ATemp"]);
                jsonPoint.Add("ETemp1", jsonPoints[jsonPoints.Count - 1]["ETemp1"]);
                jsonPoint.Add("ETemp2", jsonPoints[jsonPoints.Count - 1]["ETemp2"]);
                jsonPoint.Add("AFlow", jsonPoints[jsonPoints.Count - 1]["AFlow"]);
                jsonPoint.Add("Time", jsonPoints[jsonPoints.Count - 1]["Time"]);
                m_pointSelected = jsonPoints.Count;
            }
            else
            {
                jsonPoint.Add("ATemp", jbc.GetStationMinTemp(myID).UTI);
                jsonPoint.Add("ETemp1", jbc.GetStationMinExtTemp(myID).UTI);
                jsonPoint.Add("ETemp2", 0);
                jsonPoint.Add("AFlow", jbc.GetStationMinFlow(myID));
                jsonPoint.Add("Time", 0);
                m_pointSelected = 0;
            }
            jsonPoints.Add(jsonPoint);

            LoadDataProfile();
        }

        private void RemovePoint()
        {
            //como mínimo 1 punto
            if (m_jsonParse["_"]["Points"].Count() > 1)
            {
                m_jsonParse["_"]["Points"][m_pointSelected].Remove();

                //si hemos eliminado el último elemento
                if (m_jsonParse["_"]["Points"].Count() == m_pointSelected)
                {
                    m_pointSelected--;
                }

                LoadDataProfile();
            }
        }

        private async void SaveEditedProfile()
        {
            Cursor = Cursors.WaitCursor;
            m_profileOptions.CurrentPage = "pageProfilesOptions";
            m_profileParameters.CurrentPage = "pageProfilesParameters";

            //TODO. Save
            await jbc.UpdateProfilesAsync(myID);
            await jbc.UpdateSelectedProfileAsync(myID);

            m_profileSelected = 0;
            m_profileSelectedName = jbc.GetProfileName(myID, m_profileSelected);

            LoadJsonSelectedProfile();
            LoadDataProfile();
            Cursor = Cursors.Arrow;
        }

        private void CancelEditedProfile()
        {
            m_profileOptions.CurrentPage = "pageProfilesOptions";
            m_profileParameters.CurrentPage = "pageProfilesParameters";

            m_profileSelectedName = jbc.GetProfileName(myID, m_profileSelected);
            m_pointSelected = 0;

            LoadJsonSelectedProfile();
            LoadDataProfile();
        }

        #endregion

        #endregion

        #endregion

        #region LOAD AND SAVE SETTINGS PAGE
        //Global var's

        //Controls

        private void createLoadAndSavePage()
        {
            loadTextsLoadAndSavePage();
        }

        private void loadTextsLoadAndSavePage()
        {
            tabPages.lblConfInfo.Text = Localization.getResStr(Configuration.confInfoId);
            tabPages.butConfLoad.Text = Localization.getResStr(Configuration.confLoadId);
            tabPages.butConfSave.Text = Localization.getResStr(Configuration.confSaveId);
            ToolTipIcons.SetToolTip(iconLoadSaveSettings, Localization.getResStr(Configuration.confTabHintId));
        }

        private void OnConf_ClickControl(Control ctrl)
        {

            //Only if control mode
            if (mode != (int)ControlModeConnection.CONTROL)
            {
                MessageBox.Show(Localization.getResStr(Configuration.confMustControlModeId));
                return;
            }

            switch (ctrl.Name)
            {
                case "butConfLoad_HA":
                    var ldgXmlLoad = new OpenFileDialog();
                    ldgXmlLoad.Filter = " (*.xml)|*.xml";
                    if (ldgXmlLoad.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Loading
                        xmlLoad(ldgXmlLoad.FileName);
                    }
                    break;

                case "butConfSave_HA":
                    var sdgXmlSave = new SaveFileDialog();
                    sdgXmlSave.Filter = " (*.xml)|*.xml";
                    if (sdgXmlSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Saving
                        xmlSave(sdgXmlSave.FileName);
                    }
                    break;

                case "butConfLoadProfile_HA":
                    var ldgJPFLoad = new OpenFileDialog();
                    ldgJPFLoad.Filter = " (*.jpf)|*.jpf";
                    ldgJPFLoad.Multiselect = true;
                    if (ldgJPFLoad.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Loading
                        jpfLoad(ldgJPFLoad.FileNames);
                    }
                    break;

                case "butConfSaveProfile_HA":
                    var sdgJPFSave = new FolderBrowserDialog();
                    if (sdgJPFSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //Saving
                        jpfSave(sdgJPFSave.SelectedPath);
                    }
                    break;

            }
        }

        private void xmlLoad(string fileName)
        {
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            string sError = "";
            xmlDoc = RoutinesLibrary.Data.Xml.XMLUtils.LoadFromFile(fileName, ref sError);
            if (xmlDoc != null)
            {
                int[] iTargetPorts = new int[] { 1, 2, 3, 4 };
                Configuration.confSetToStation_HA(xmlDoc, myID, jbc, iTargetPorts);
            }
        }

        private void xmlSave(string filename)
        {
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            // get settings from station in xml format
            xmlDoc = Configuration.confGetFromStation_HA(myID, jbc, false);
            if (xmlDoc != null)
            {
                // save xml
                RoutinesLibrary.Data.Xml.XMLUtils.SaveToFile(xmlDoc, filename);
            }
        }

        private async void jpfLoad(string[] filesName)
        {
            Cursor = Cursors.WaitCursor;
            List<string> filesError = new List<string>();

            foreach (string fileName in filesName)
            {
                string fileData = "";

                //Check file exists
                try
                {
                    fileData = (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.ReadAllText(fileName);
                }
                catch (Exception)
                {
                    filesError.Add(fileName);
                    continue;
                }

                //Check JSON format
                try
                {
                    m_jsonParse = JObject.Parse(fileData);
                }
                catch (Exception)
                {
                    filesError.Add(fileName);
                    continue;
                }

                //TODO . save
            }

            await jbc.UpdateProfilesAsync(myID);
            await jbc.UpdateSelectedProfileAsync(myID);

            Cursor = Cursors.Arrow;

            if (filesError.Count == 0)
            {
                MessageBox.Show("Profiles have been loaded correctly", "", MessageBoxButtons.OK);
            }
            else
            {
                string profilesFailed = "";
                for (int i = 0; i <= filesError.Count - 1; i++)
                {
                    profilesFailed += "\r\n" + filesError[i];
                }

                MessageBox.Show("The following profiles could not be loaded: " + profilesFailed, "", MessageBoxButtons.OK);
            }
        }

        private void jpfSave(string folderName)
        {
            for (int i = 0; i <= jbc.GetProfileCount(myID) - 1; i++)
            {
                string profileName = jbc.GetProfileName(myID, i);
                string profileData = System.Text.Encoding.ASCII.GetString(jbc.GetProfile(myID, profileName));

                (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(System.IO.Path.Combine(folderName, profileName), profileData, false);
            }

            MessageBox.Show("Profiles have been saved correctly", "", MessageBoxButtons.OK);
        }

        #endregion

        #region RESET SETTINGS PAGE
        // Global var's
        private bool resetSettingsParams = false;
        private bool resetOnProcess = false;

        // Creates the reset settings page
        private void createResetSettingsPage()
        {
            loadTextsResetSettingsPage();
        }

        private void loadTextsResetSettingsPage()
        {
            tabPages.lblResetInfo.Text = Localization.getResStr(Configuration.resetInfoId);
            tabPages.butResetProceed.Text = Localization.getResStr(Configuration.resetProceedId);
            ToolTipIcons.SetToolTip(iconResetSettings, Localization.getResStr(Configuration.resetTabHintId));

        }

        private async void onReset_ClickControl(Control ctrl)
        {
            //Only if control mode
            if (mode == (int)ControlModeConnection.CONTROL)
            {
                //Proceeding
                await jbc.DefaultStationParametersAsync(myID);

                //Starting the progress bar
                tabPages.pgbResetBar.Value = 0;
                resetOnProcess = true;
            }
            else
            {
                tmr.Stop();
                MessageBox.Show(Localization.getResStr(Configuration.confMustControlModeId));
                tmr.Start();
            }
        }

        private void updateResetSettingsPage()
        {
            //Updating the progressBar if reset running
            if (resetOnProcess)
            {
                if (tabPages.pgbResetBar.Value + Configuration.resetPgBarInc <= tabPages.pgbResetBar.Maximum)
                {
                    tabPages.pgbResetBar.Value = tabPages.pgbResetBar.Value + Configuration.resetPgBarInc;
                }
                if (tabPages.pgbResetBar.Value + Configuration.resetPgBarInc > tabPages.pgbResetBar.Maximum)
                {
                    tabPages.pgbResetBar.Value = tabPages.pgbResetBar.Maximum;
                }
                if (tabPages.pgbResetBar.Value >= tabPages.pgbResetBar.Maximum)
                {
                    //Finished
                    resetOnProcess = false;
                    Interaction.MsgBox(Localization.getResStr(Configuration.resetSuccessMessageId), MsgBoxStyle.OkOnly, this.Text);
                    tabPages.pgbResetBar.Value = 0;
                }
            }
        }
        #endregion

        #region COUNTERS PAGE
        //Global var's
        private bool counterParams = false;

        private Port counterCurPort;
        private CounterTypes counterCurType = CounterTypes.GLOBAL_COUNTER;

        // Controls
        private ParamTable counterParamsTable;
        private bool bAlreadySetCounterPorts = false;

        //Creates the counters page
        private void createCountersPage()
        {
            var TOP_MARGIN = 20;
            var LEFT_MARGIN = 5;

            // Creating the param table
            counterParamsTable = new ParamTable(240, 180); // default columns width
            counterParamsTable.Location = new Point(System.Convert.ToInt32((double)tabPages.pageCounters.ClientSize.Width * LEFT_MARGIN / 100), System.Convert.ToInt32((double)tabPages.pageCounters.Height * TOP_MARGIN / 100));
            counterParamsTable.Height = tabPages.pageCounters.ClientSize.Height * (100 - TOP_MARGIN - 5) / 100;
            counterParamsTable.Width = tabPages.pageCounters.ClientSize.Width * (100 - (LEFT_MARGIN * 2)) / 100;

            // Adding the tool parametters and its input controls
            counterParamsTable.addParam(Configuration.counterPluggedMinutesId, Localization.getResStr(Configuration.counterPluggedMinutesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterWorkMinutesId, Localization.getResStr(Configuration.counterWorkMinutesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterNoToolMinutesId, Localization.getResStr(Configuration.counterNoToolMinutesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterWorkCyclesId, Localization.getResStr(Configuration.counterWorkCyclesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterSuctionCyclesId, Localization.getResStr(Configuration.counterSuctionCyclesId), ParamTable.cInputType.FIX, null, null);

            //Adding controls to the page
            tabPages.pageCounters.Controls.Add(counterParamsTable);
            ToolTipIcons.SetToolTip(iconCounters, Localization.getResStr(Configuration.counterTabHintId));

            tabPages.rbGlobalCounters.Text = Localization.getResStr(Configuration.counterGlobalId);
            tabPages.rbPartialCounters.Text = Localization.getResStr(Configuration.counterPartialId);
            tabPages.butResetPartialCounters.Text = Localization.getResStr(Configuration.counterResetPartialCountersId);

            tabPages.rbGlobalCounters.Visible = features.PartialCounters;
            tabPages.rbPartialCounters.Visible = features.PartialCounters;
            tabPages.butResetPartialCounters.Visible = features.PartialCounters;

            //Forcing update
            counterParams = true;
        }

        private void loadTextsCountersPage()
        {
            // update texts
            counterParamsTable.setText(Configuration.counterPluggedMinutesId, Localization.getResStr(Configuration.counterPluggedMinutesId), null);
            counterParamsTable.setText(Configuration.counterWorkMinutesId, Localization.getResStr(Configuration.counterWorkMinutesId), null);
            counterParamsTable.setText(Configuration.counterNoToolMinutesId, Localization.getResStr(Configuration.counterNoToolMinutesId), null);
            counterParamsTable.setText(Configuration.counterWorkCyclesId, Localization.getResStr(Configuration.counterWorkCyclesId), null);
            counterParamsTable.setText(Configuration.counterSuctionCyclesId, Localization.getResStr(Configuration.counterSuctionCyclesId), null);

            tabPages.rbGlobalCounters.Text = Localization.getResStr(Configuration.counterGlobalId);
            tabPages.rbPartialCounters.Text = Localization.getResStr(Configuration.counterPartialId);
            tabPages.butResetPartialCounters.Text = Localization.getResStr(Configuration.counterResetPartialCountersId);

            ToolTipIcons.SetToolTip(iconCounters, Localization.getResStr(Configuration.counterTabHintId));
        }

        //Private Sub rbPortCounters_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        private void onCountersPort_CheckedChanged(Control ctrl)
        {
            RadioButton rb = (RadioButton)ctrl;
            if (rb.Checked)
            {
                if (Configuration.myGetRadioButtonPortNbr(rb) <= jbc.GetPortCount(myID))
                {
                    counterCurPort = (Port)System.Enum.Parse(typeof(Port), "NUM_" + Configuration.myGetRadioButtonPortNbr(rb).ToString());
                    rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("Port" + Configuration.myGetRadioButtonPortNbr(rb).ToString() + "miniSelected");
                    counterParams = true;
                }
                else
                {
                    rb.Checked = false;
                }
            }
            else
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("Port" + Configuration.myGetRadioButtonPortNbr(rb).ToString() + "mini");
            }
            rb.Refresh();
        }

        private void onCountersType_CheckedChanged(string sType)
        {
            if (sType == "P")
            {
                counterCurType = CounterTypes.PARTIAL_COUNTER;
            }
            else
            {
                counterCurType = CounterTypes.GLOBAL_COUNTER;
            }
            counterParams = true;
        }

        private uint resetCounterTransactionId = (uint)0;
        private async void onCountersResetPartial_Click()
        {
            //Only if control mode
            if (mode == (int)ControlModeConnection.CONTROL)
            {
                jbc.ResetPortToolStationPartialCounters(myID, counterCurPort);
                // envía set transaction y espera a recibir el evento TransactionFinished para actualizar la página
                // 17/02/2015 hasta que implementemos callsback en WCF, se utiliza un timer y jbc.QueryEndedTransaction
                resetCounterTransactionId = System.Convert.ToUInt32(await jbc.SetTransactionAsync(myID));
                iCountQueryTransactionTicks = 0;
                tmrQueryTransaction.Start();
            }
            else
            {
                tmr.Stop();
                MessageBox.Show(Localization.getResStr(Configuration.confMustControlModeId));
                tmr.Start();
            }
        }

        private async void tmrQueryTransaction_Tick(object sender, System.EventArgs e)
        {
            tmrQueryTransaction.Stop();
            iCountQueryTransactionTicks++;
            if (iCountQueryTransactionTicks > MAX_TICKS_QUERY_TRANSACTION)
            {
                counterParams = true;
            }
            else
            {
                if (await jbc.QueryEndedTransactionAsync(myID, resetCounterTransactionId))
                {
                    counterParams = true;
                }
                else
                {
                    tmrQueryTransaction.Start();
                }
            }
        }

        private async void updateCountersPage()
        {
            // When mode has been changed
            if (mode == (int)ControlModeConnection.MONITOR)
            {
                counterParamsTable.inputControlsEnable(false);
            }
            else if (mode == (int)ControlModeConnection.CONTROL)
            {
                counterParamsTable.inputControlsEnable(true);
            }

            // if station data is available
            if (jbc.GetPortCount(myID) > 0)
            {
                if (!bAlreadySetCounterPorts)
                {

                    //Setting initial port
                    counterCurPort = Port; // same default work port

                    // selecte radio buttons
                    Control radio = null;
                    if (Configuration.myControlExists(this, "rbPortCounters_" + counterCurPort.ToString().Replace("NUM_", ""), ref radio))
                    {
                        ((RadioButton)radio).Checked = true;
                    }

                    // disable unsupported ports
                    int i = jbc.GetPortCount(myID) + 1;
                    while (i <= Configuration.MAX_PORTS)
                    {
                        if (Configuration.myControlExists(this, "rbPortCounters_" + i.ToString(), ref radio))
                        {
                            ((RadioButton)radio).Enabled = false;
                        }
                        i++;
                    }

                    bAlreadySetCounterPorts = true;
                    counterParams = true;
                }

            }

            //If update necessary then updating
            if (counterParams)
            {

                // Retrieve remote data
                await jbc.UpdatePortCountersAsync(myID, counterCurPort);

                counterParamsTable.setValue(Configuration.counterPluggedMinutesId, Configuration.myGetStrFromMinutes(jbc.GetPortToolPluggedMinutes(myID, counterCurPort, counterCurType)));
                counterParamsTable.setValue(Configuration.counterWorkMinutesId, Configuration.myGetStrFromMinutes(jbc.GetPortToolWorkMinutes(myID, counterCurPort, counterCurType)));
                counterParamsTable.setValue(Configuration.counterNoToolMinutesId, Configuration.myGetStrFromMinutes(jbc.GetPortToolIdleMinutes(myID, counterCurPort, counterCurType)));
                counterParamsTable.setValue(Configuration.counterWorkCyclesId, System.Convert.ToString(jbc.GetPortToolWorkCycles(myID, counterCurPort, counterCurType).ToString()));
                counterParamsTable.setValue(Configuration.counterSuctionCyclesId, System.Convert.ToString(jbc.GetPortToolSuctionCycles(myID, counterCurPort, counterCurType).ToString()));

                if (features.PartialCounters & counterCurType == CounterTypes.PARTIAL_COUNTER)
                {
                    tabPages.butResetPartialCounters.Visible = true;
                }
                else
                {
                    tabPages.butResetPartialCounters.Visible = false;
                }

                //Update done
                counterParams = false;
            }
        }

        #endregion

        #region INFO PAGE
        //Global var's
        private bool infoParams = false;

        // Controls
        private ParamTable infoParamsTable;

        //Creates the info page
        private void createInfoPage()
        {
            var TOP_MARGIN = 10;
            var LEFT_MARGIN = 5;

            // Creating the param table
            infoParamsTable = new ParamTable(300, 120); // default columns width
            infoParamsTable.Location = new Point(tabPages.pageInfo.ClientSize.Width * LEFT_MARGIN / 100, tabPages.pageInfo.ClientSize.Height * TOP_MARGIN / 100);
            infoParamsTable.Height = tabPages.pageInfo.ClientSize.Height * (100 - TOP_MARGIN - 5) / 100;
            infoParamsTable.Width = tabPages.pageInfo.ClientSize.Width * (100 - (LEFT_MARGIN * 2)) / 100;

            // Adding the info parametters and its input controls
            infoParamsTable.addParam(Configuration.stnModelId, Localization.getResStr(Configuration.stnModelId), ParamTable.cInputType.FIX, null, null);
            infoParamsTable.addParam(Configuration.stnSWId, Localization.getResStr(Configuration.stnSWId), ParamTable.cInputType.FIX, null, null);
            infoParamsTable.addParam(Configuration.stnProtocolId, Localization.getResStr(Configuration.stnProtocolId), ParamTable.cInputType.FIX, null, null);
            //infoParamsTable.addParam(stnTrafoErrId, getResStr(stnTrafoErrId), ParamTable.cInputType.FIX)
            //infoParamsTable.addParam(stnMOSErrId, getResStr(stnMOSErrId), ParamTable.cInputType.FIX)


            //Adding controls to the page
            tabPages.pageInfo.Controls.Add(infoParamsTable);
            ToolTipIcons.SetToolTip(iconInfo, Localization.getResStr(Configuration.infoTabHintId));

            //Forcing update
            infoParams = true;
        }

        private void loadTextsInfoPage()
        {
            // updating texts in the param table
            infoParamsTable.setText(Configuration.stnModelId, Localization.getResStr(Configuration.stnModelId), null);
            infoParamsTable.setText(Configuration.stnSWId, Localization.getResStr(Configuration.stnSWId), null);
            infoParamsTable.setText(Configuration.stnProtocolId, Localization.getResStr(Configuration.stnProtocolId), null);
            //infoParamsTable.setText(stnTrafoErrId, getResStr(stnTrafoErrId))
            //infoParamsTable.setText(stnMOSErrId, getResStr(stnMOSErrId))
            ToolTipIcons.SetToolTip(iconInfo, Localization.getResStr(Configuration.infoTabHintId));
        }

        private void updateInfoPage()
        {
            // When mode has been changed
            if (mode == (int)ControlModeConnection.MONITOR)
            {
                infoParamsTable.inputControlsEnable(false);
            }
            else if (mode == (int)ControlModeConnection.CONTROL)
            {
                infoParamsTable.inputControlsEnable(true);
            }

            //If update necessary then updating
            if (infoParams)
            {
                infoParamsTable.setValue(Configuration.stnModelId, jbc.GetStationModel(myID));
                infoParamsTable.setValue(Configuration.stnSWId, jbc.GetStationSWversion(myID));
                infoParamsTable.setValue(Configuration.stnProtocolId, jbc.GetStationProtocol(myID));
                //Dim temp As Ctemperature
                //temp = jbc.GetStationTransformerErrorTemp(myID)
                //infoParamsTable.setValue(stnTrafoErrId, convertTempToString(temp, True, True))
                //temp = jbc.GetStationMOSerrorTemp(myID)
                //infoParamsTable.setValue(stnMOSErrId, convertTempToString(temp, True, True))

                //Update done
                infoParams = false;
            }
        }

        #endregion

        #region GRAPHICS PAGE

        private void createGraphicsPage()
        {
            loadTextsGraphicsPage();
        }

        private void loadTextsGraphicsPage()
        {
            //tabPages.labUnderConstruction.Text = getResStr(gralUnderConstructionId)
            ToolTipIcons.SetToolTip(iconGraphics, Localization.getResStr(Configuration.graphTabHintId));

            tabPages.lblGraphInfo.Text = Localization.getResStr(Configuration.graphInfoId);
            tabPages.lblGraphAddToPlot.Text = Localization.getResStr(Configuration.graphAddToPlotId);
            tabPages.butGraphAddSeries.Text = Localization.getResStr(Configuration.graphSeriesAddSeriesId);

        }

        private void onGraphPlots_DropDown(Control ctrl)
        {
            ComboBox combo = (ComboBox)ctrl;
            string currentSelectedPlot = combo.GetItemText(combo.SelectedItem);
            combo.Items.Clear();
            combo.Items.Add(Localization.getResStr(Configuration.graphNewPlotId)); // new plot
            foreach (ManRegister.tRegister plotwnd in reg.registerList)
            {
                combo.Items.Add(plotwnd.frm.plot.myTitle);
            }
            if (!string.IsNullOrEmpty(currentSelectedPlot))
            {
                combo.SelectedIndex = combo.Items.IndexOf(currentSelectedPlot);
            }

        }

        private void onGraphAddSeries_Click(Control ctrl)
        {
            ComboBox combo = tabPages.cbxGraphPlots;
            string currentSelectedPlot = combo.GetItemText(combo.SelectedItem);
            if (currentSelectedPlot == Localization.getResStr(Configuration.graphNewPlotId) || string.IsNullOrEmpty(currentSelectedPlot))
            {
                if (PlotStationEvent != null)
                    PlotStationEvent(myID, -1);
            }
            else
            {
                if (PlotStationEvent != null) // the first index is "new plot"
                    PlotStationEvent(myID, reg.registerList[combo.SelectedIndex - 1].frmID);
            }
        }

        #endregion

        #region Showing Radio Button Images

        private void rbToolSettings_MouseEnter(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(Configuration.myGetRadioButtonToolName(rb) + "_miniMouseOver");
                rb.Refresh();
            }
        }

        private void rbToolSettings_MouseLeave(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(Configuration.myGetRadioButtonToolName(rb) + "_mini");
                rb.Refresh();
            }
        }

        #endregion

        #region JBC API Events

        //Private Sub jbc_TransactionFinished(ByVal stationID As ULong, ByVal transactionID As UInteger) Handles jbc.TransactionFinished
        //    ' NO IMPLEMENTADO HASTA QUE NO TENGAMOS DUAL COMMUNICATION EN WCF (CALLBACKS)
        //    ' se reemplaza por un timer y QueryEndedTransaction
        //    counterParams = True
        //End Sub
        #endregion

        //Private Sub IconTab_Deselected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles IconTab.Deselected
        //    'e.TabPage.BackgroundImage = Nothing
        //End Sub

        //Private Sub IconTab_Selected(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlEventArgs) Handles IconTab.Selected
        //    'e.TabPage.BackgroundImage = My.Resources.Background
        //End Sub

    }
}
