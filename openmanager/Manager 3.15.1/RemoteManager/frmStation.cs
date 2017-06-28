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


namespace RemoteManager
{
    public partial class frmStation
    {

        //#region Default Instance

        //    private static frmStation defaultInstance;

        //    /// <summary>
        //    /// Added by the VB.Net to C# Converter to support default instance behavour in C#
        //    /// </summary>
        //    public static frmStation Default
        //    {
        //        get
        //        {
        //            if (defaultInstance == null)
        //            {
        //                defaultInstance = new frmStation();
        //                defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
        //            }

        //            return defaultInstance;
        //        }
        //        set
        //        {
        //            defaultInstance = value;
        //        }
        //    }

        //    static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        //    {
        //        defaultInstance = null;
        //    }

        //#endregion

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

        public frmStation(JBC_API_Remote jbcRef, ManRegister regRef, long ID, ListViewItem item)
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

            //Ocultar el icono de periféricos si la estación no los soporta
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

            tabPages = new TabPanels("pageWork", jbc.GetStationTools(myID));
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
            createPeripheralPage();
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
            loadTextsPeripheralPage();
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
                toolSettingsParams = true;
                m_updatePeripheralParams = true;
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
                        toolSettingsParams = true; // force update
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
                        tabPages.CurrentPage = "pageWork";
                        break;
                    case "iconStationSettings":
                        m_updateStationSettingsParams = true; // force update
                        tabPages.CurrentPage = "pageStationSettings";
                        break;
                    case "iconToolSettings":
                        toolSettingsParams = true; // force update
                        tabPages.CurrentPage = "pageToolSettings";
                        break;
                    case "iconPeripheral":
                        m_updatePeripheralParams = true; // force update
                        tabPages.CurrentPage = "pagePeripheral";
                        break;
                    case "iconLoadSaveSettings":
                        tabPages.CurrentPage = "pageLoadSaveSettings";
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
                    m_updatePeripheralParams = true;
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
                    case "pageWork":
                        updateWorkPage();
                        break;
                    case "pageStationSettings":
                        updateStationSettingsPage();
                        break;
                    case "pageToolSettings":
                        updateToolSettingsPage();
                        break;
                    case "pagePeripheral":
                        updatePeripheralPage();
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

                //Depending on the tag value reseting the timer
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
        private TempPanels tempPages;

        // Creates the work page controls
        private void createWorkPage()
        {

            tempPages = new TempPanels("");
            tempPages.ClickControl += onClickControlTemp;
            tabPages.PanelTemps.Controls.Add(tempPages);
            loadTextsWorkPage();
            myResetPowerTrack();

            // Updating the work parametters
            WorkParams = true;
        }

        private void loadTextsWorkPage()
        {

            tabPages.lblTitlePwr.Text = Localization.getResStr(Configuration.workPowerId);
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

                case "lblLvl1":
                    // change temp level from work page
                    lvl = ToolTemperatureLevels.FIRST_LEVEL;
                    await jbc.SetPortToolSelectedTempLevelsAsync(myID, Port, jbc.GetPortToolID(myID, Port), lvl);
                    toolSettingsParams = true; // update tool params page also
                    break;

                case "lblLvl2":
                    // change temp level from work page
                    lvl = ToolTemperatureLevels.SECOND_LEVEL;
                    await jbc.SetPortToolSelectedTempLevelsAsync(myID, Port, jbc.GetPortToolID(myID, Port), lvl);
                    toolSettingsParams = true; // update tool params page also
                    break;

                case "lblLvl3":
                    // change temp level from work page
                    lvl = ToolTemperatureLevels.THIRD_LEVEL;
                    await jbc.SetPortToolSelectedTempLevelsAsync(myID, Port, jbc.GetPortToolID(myID, Port), lvl);
                    toolSettingsParams = true; // update tool params page also
                    break;

            }
            WorkParams = true;

        }

        // Port selection ----------------------------------------------------------------------

        private void onWorkClickControl(Control ctrl)
        {

            switch (ctrl.Name)
            {
                case "pcbTool":
                case "butPort":
                    // Stopping the update timer
                    tmr.Stop();

                    // Launching the tool selection port
                    frmPorts portWin = new frmPorts(myID, jbc);
                    //portWin.MdiParent = Me.MdiParent
                    portWin.selectedPort = Port;
                    portWin.ShowDialog();
                    Port = portWin.selectedPort;

                    // update work page
                    WorkParams = true;

                    // Restarting the update timer
                    tmr.Start();
                    break;


            }

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
            tabPages.pictTrackPowerColor.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("power_" + iPower.ToString());
        }

        private void myResetPowerTrack()
        {
            mySetPowerScale(0);
        }
        #endregion

        private async void updateWorkPage()
        {
            CTemperature temp = default(CTemperature);
            CTemperature tempAdjust = default(CTemperature);

            // Checking if the station is initialized
            if (jbc.GetPortCount(myID) <= 0)
            {
                return;
            }

            // Retrieve remote data
            await jbc.UpdatePortStatusAsync(myID, Port);

            // If no tempPages page selected, force update all data, checking what page to show
            if (tempPages.CurrentPage == "")
            {
                WorkParams = true;
            }

            GenericStationTools toolWork = default(GenericStationTools);
            toolWork = jbc.GetPortToolID(myID, Port);

            //
            // NO TOOL
            //
            if (toolWork == GenericStationTools.NO_TOOL)
            {
                //show/hide only once (change from "Tool" to "No tool")
                if (!tabPages.lblNoTool.Visible)
                {
                    // hide work data
                    tabPages.panelPower.Visible = false;
                    tabPages.PanelTemps.Visible = false;
                    tabPages.lblToolTemp.Visible = false;
                    tabPages.lblToolTempUnits.Visible = false;
                    tabPages.lblToolTempAdjust.Visible = false;
                    tabPages.pcbTool.Visible = false;
                    // hide sleep data
                    tabPages.panelWorkSleep.Visible = false;
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
                // tool
                tabPages.pcbTool.BackgroundImage = (Image)My.Resources.Resources.ResourceManager.GetObject(toolWork.ToString() + "_big");

                // status
                string sTFTStatus = "";
                string sTFTStand = "";
                string sTFTDelay = "";
                bool stand = false;
                bool sleep = false;
                bool hiber = false;
                bool extractor = false;
                bool desolder = false;
                ToolFutureMode futureMode;
                stand = jbc.GetPortToolStandStatus(myID, Port) == OnOff._ON;
                sleep = jbc.GetPortToolSleepStatus(myID, Port) == OnOff._ON;
                hiber = jbc.GetPortToolHibernationStatus(myID, Port) == OnOff._ON;
                extractor = jbc.GetPortToolExtractorStatus(myID, Port) == OnOff._ON;
                desolder = jbc.GetPortToolDesolderStatus(myID, Port) == OnOff._ON;
                int iDelay = 0;
                iDelay = jbc.GetPortToolTimeToFutureMode(myID, Port);
                //Debug.Print("Port={0} Stand:{1} Sleep:{2} Hiber:{3} Extractor:{4} Desolder:{5} Delay: {6} FutureMode:{7}", (Port + 1).ToString, stand.ToString, sleep.ToString, hiber.ToString, extractor.ToString, desolder.ToString, iDelay.ToString, jbc.GetPortToolFutureMode(myID, Port))
                if (iDelay > 0)
                {
                    // if Delay, show status as Sleep and Tool in the stand
                    sTFTStatus = Localization.getResStr(Configuration.PortsSleepId);
                    sTFTStand = Localization.getResStr(Configuration.PortsStandId);
                    futureMode = jbc.GetPortToolFutureMode(myID, Port);
                    if (futureMode == ToolFutureMode.Sleep)
                    {
                        sTFTStatus = "";
                        sTFTDelay = string.Format(Localization.getResStr(Configuration.PortsDelayToSleepId), Configuration.myGetFormatedDelay(iDelay));
                    }
                    else if (futureMode == ToolFutureMode.Hibernation)
                    {
                        // puede estar en hibernación, pero estar la hibernación desactivada, entonces devuelve FFFF en Delay
                        if (iDelay != System.Convert.ToInt32(ToolTimeHibernation.NO_HIBERNATION))
                        {
                            sTFTDelay = string.Format(Localization.getResStr(Configuration.PortsDelayToHiberId), Configuration.myGetFormatedDelay(iDelay));
                        }
                    }
                }
                else if (stand)
                {
                    sTFTStatus = "";
                    sTFTStand = Localization.getResStr(Configuration.PortsStandId);
                }
                else if (sleep)
                {
                    sTFTStatus = Localization.getResStr(Configuration.PortsSleepId);
                    sTFTStand = Localization.getResStr(Configuration.PortsStandId);
                }
                else if (hiber)
                {
                    sTFTStatus = Localization.getResStr(Configuration.PortsHiberId);
                    sTFTStand = Localization.getResStr(Configuration.PortsStandId);
                }
                else if (extractor)
                {
                    // 24/07/2015 el texto de status debe ser Hibernación, cuando está en Extractor
                    sTFTStatus = Localization.getResStr(Configuration.PortsHiberId);
                    sTFTStand = Localization.getResStr(Configuration.PortsExtractorId);
                }
                else if (desolder)
                {
                    sTFTStatus = Localization.getResStr(Configuration.PortsDesolderId);
                    sTFTStand = "";
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
                    tabPages.pcbTool.Visible = true;
                    tabPages.PanelTemps.Visible = true;

                    bFromNoTool = true;
                }

                // actual temp
                temp = jbc.GetPortToolActualTemp(myID, Port);
                tempAdjust = jbc.GetPortToolAdjustTemp(myID, Port, toolWork);

                //
                // STAND
                //
                if (stand || sleep || hiber || extractor || iDelay > 0)
                {
                    // Show sleep page

                    // show/hide only once
                    if (!tabPages.panelWorkSleep.Visible || bFromNoTool)
                    {
                        // work data
                        tabPages.panelPower.Visible = false;
                        tabPages.lblToolTemp.Visible = false;
                        tabPages.lblToolTempUnits.Visible = false;
                        tabPages.lblToolTempAdjust.Visible = false;
                        // sleep data
                        tabPages.panelWorkSleep.Visible = true;
                    }

                    tabPages.labWorkSleepStatus.Text = sTFTStatus;
                    tabPages.labWorkSleepStand.Text = sTFTStand;
                    tabPages.labWorkSleepTemp.Text = string.Format(Localization.getResStr(Configuration.PortsActualTempId), Configuration.convertTempToString(temp, true, true).ToString());
                    tabPages.labWorkSleepDelay.Text = sTFTDelay;
                }
                else
                {
                    //
                    // WORK
                    //

                    // show/hide only once
                    if (tabPages.panelWorkSleep.Visible || bFromNoTool)
                    {
                        // sleep data
                        tabPages.panelWorkSleep.Visible = false;
                        // work data
                        tabPages.panelPower.Visible = true;
                        tabPages.lblToolTemp.Visible = true;
                        tabPages.lblToolTempUnits.Visible = true;
                        tabPages.lblToolTempAdjust.Visible = true;
                        tabPages.PanelTemps.Visible = true;
                    }

                    tabPages.lblToolTemp.Text = Configuration.convertTempToString(temp, false, true);
                    tabPages.lblToolTempUnits.Text = Configuration.Tunits;
                    tabPages.lblToolTempAdjust.Text = "";
                    if (tempAdjust.UTI != 0)
                    {
                        if (tempAdjust.UTI > 0)
                        {
                            tabPages.lblToolTempAdjust.Text = "+";
                        }
                        tabPages.lblToolTempAdjust.Text += Configuration.convertTempAdjToString(tempAdjust, false, System.Convert.ToString(true));
                    }
                    // Power bar
                    pwr = System.Convert.ToInt32(jbc.GetPortToolActualPower(myID, Port) / 10);
                    tabPages.lblPwr.Text = pwr.ToString() + " " + Configuration.pwrUnitsPercentStr;
                    mySetPowerScale(pwr);
                }
            }

            tabPages.butPort.Text = string.Format(Localization.getResStr(Configuration.workPortId), Port.ToString().Replace("NUM_", ""));

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

            if (WorkParams)
            {
                // Params updated
                WorkParams = false;

                // default=Manual
                // user can change selected temp for the working port, also if no tool selected
                bool bFixed = false;
                bool bLevel = false;

                // Label desired temperature
                temp = jbc.GetPortToolSelectedTemp(myID, Port);
                if (temp != null)
                {
                    if (temp.isValid())
                    {
                        tempPages.lblDesTemp.Text = Localization.getResStr(Configuration.workTempSelectionId) + ": " + Configuration.convertTempToString(temp, true, true);
                    }
                }

                if (toolWork != GenericStationTools.NO_TOOL)
                {
                    //
                    // Levels
                    //
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

                    //
                    // Level fixed
                    //
                    bFixed = false;
                    if (!features.TempLevelsWithStatus)
                    {
                        temp = jbc.GetPortToolFixTemp(myID, Port, toolWork);
                        if (!Equals(temp, null))
                        {
                            if (temp.isValid())
                            {
                                tempPages.lblFixed.Text = Localization.getResStr(Configuration.workTempFixedId) + ": " + Configuration.convertTempToString(temp, true, true);
                                bFixed = true;
                            }
                            else
                            {
                                tempPages.lblFixed.Text = Localization.getResStr(Configuration.workTempFixedId) + ": " + Configuration.invalidTempStr;
                            }
                        }
                        else
                        {
                            tempPages.lblFixed.Text = Localization.getResStr(Configuration.workTempFixedId) + ": " + Configuration.invalidTempStr;
                        }
                    }

                    //
                    // Get info status periph
                    //
                    if (features.Peripherals)
                    {
                        CPeripheralData[] portPeripheralsData = jbc.GetPortPeripheral(myID, Port);

                        if (portPeripheralsData.Length > 0)
                        {
                            switch (portPeripheralsData[0].Type)
                            {
                                case CPeripheralData.PeripheralType.FS:
                                    tabPages.txtStatusPeriph_1.Text = "FS";
                                    break;
                                case CPeripheralData.PeripheralType.MN:
                                    tabPages.txtStatusPeriph_1.Text = "MN";
                                    break;
                                case CPeripheralData.PeripheralType.MS:
                                    tabPages.txtStatusPeriph_1.Text = "MS";
                                    break;
                                case CPeripheralData.PeripheralType.MV:
                                    tabPages.txtStatusPeriph_1.Text = "MV";
                                    break;
                                default:
                                    tabPages.txtStatusPeriph_1.Text = "PD";
                                    break;
                            }

                            switch (portPeripheralsData[0].StatusActive)
                            {
                                case OnOff._ON:
                                    tabPages.txtStatusPeriph_1.ForeColor = Color.FromArgb(238, 160, 74);
                                    break;
                                default:
                                    tabPages.txtStatusPeriph_1.ForeColor = Color.FromArgb(64, 64, 64);
                                    break;
                            }

                            if (portPeripheralsData.Length > 1)
                            {
                                switch (portPeripheralsData[1].Type)
                                {
                                    case CPeripheralData.PeripheralType.FS:
                                        tabPages.txtStatusPeriph_2.Text = "FS";
                                        break;
                                    case CPeripheralData.PeripheralType.MN:
                                        tabPages.txtStatusPeriph_2.Text = "MN";
                                        break;
                                    case CPeripheralData.PeripheralType.MS:
                                        tabPages.txtStatusPeriph_2.Text = "MS";
                                        break;
                                    case CPeripheralData.PeripheralType.MV:
                                        tabPages.txtStatusPeriph_2.Text = "MV";
                                        break;
                                    default:
                                        tabPages.txtStatusPeriph_2.Text = "PD";
                                        break;
                                }

                                switch (portPeripheralsData[1].StatusActive)
                                {
                                    case OnOff._ON:
                                        tabPages.txtStatusPeriph_2.ForeColor = Color.FromArgb(238, 160, 74);
                                        break;
                                    default:
                                        tabPages.txtStatusPeriph_2.ForeColor = Color.FromArgb(64, 64, 64);
                                        break;
                                }

                                tabPages.txtStatusPeriph_2.Visible = true;
                            }
                            else
                            {
                                tabPages.txtStatusPeriph_2.Visible = false;
                            }

                            if (!tabPages.panelWorkPeriphStatus.Visible)
                            {
                                tabPages.panelWorkPeriphStatus.Visible = true;
                            }
                        }
                        else
                        {
                            if (tabPages.panelWorkPeriphStatus.Visible)
                            {
                                tabPages.panelWorkPeriphStatus.Visible = false;
                            }
                        }
                    }
                }

                if (bFixed)
                {
                    tempPages.CurrentPage = "pageTempFixed";
                }
                else if (bLevel)
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

            if (features.DisplaySettings)
            {
                stationParamsTable.addParam(Configuration.stnTunitsId, Localization.getResStr(Configuration.stnTunitsId), ParamTable.cInputType.SWITCH, tempUnits, tempUnitsText);
                stationParamsTable.addParam(Configuration.stnN2Id, Localization.getResStr(Configuration.stnN2Id), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
                stationParamsTable.addParam(Configuration.stnHelpId, Localization.getResStr(Configuration.stnHelpId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
                stationParamsTable.addParam(Configuration.stnBeepId, Localization.getResStr(Configuration.stnBeepId), ParamTable.cInputType.SWITCH, aOnOff, OnOffText);
            }

            // 13/01/2014
            //stationParamsTable.addParam(stnPwrLimitId, getResStr(stnPwrLimitId), ParamTable.cInputType.NUMBER, pwropt)
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

            if (features.DisplaySettings)
            {
                stationParamsTable.setText(Configuration.stnTunitsId, Localization.getResStr(Configuration.stnTunitsId), tempUnitsText);
                stationParamsTable.setText(Configuration.stnN2Id, Localization.getResStr(Configuration.stnN2Id), OnOffText);
                stationParamsTable.setText(Configuration.stnHelpId, Localization.getResStr(Configuration.stnHelpId), OnOffText);
                stationParamsTable.setText(Configuration.stnBeepId, Localization.getResStr(Configuration.stnBeepId), OnOffText);
            }

            //13/02/2014
            //stationParamsTable.setText(stnPwrLimitId, getResStr(stnPwrLimitId), pwropt)
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

                temp = jbc.GetStationMaxTemp(myID);
                stationParamsTable.setValue(Configuration.stnTmaxId, Configuration.convertTempToString(temp, false, true));
                temp = jbc.GetStationMinTemp(myID);
                stationParamsTable.setValue(Configuration.stnTminId, Configuration.convertTempToString(temp, false, true));

                if (features.DisplaySettings)
                {
                    CTemperature.TemperatureUnit units = jbc.GetStationTempUnits(myID);
                    stationParamsTable.setValue(Configuration.stnTunitsId, units.ToString());
                    stationParamsTable.setValue(Configuration.stnBeepId, jbc.GetStationBeep(myID).ToString());
                    stationParamsTable.setValue(Configuration.stnHelpId, jbc.GetStationHelpText(myID).ToString());
                    stationParamsTable.setValue(Configuration.stnN2Id, jbc.GetStationN2Mode(myID).ToString());
                }

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


                //13/01/2014
                //Dim pwr As Integer = jbc.GetStationPowerLimit(myID)
                //stationParamsTable.setValue(stnPwrLimitId, pwr.ToString())

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
            if (name == Configuration.stnTunitsId)
            {
                await jbc.SetStationTempUnitsAsync(myID, (CTemperature.TemperatureUnit)Int32.Parse(value));
            }
            //13/01/2014
            //If name = stnPwrLimitId Then jbc.SetStationPowerLimit(myID, Convert.ToInt32(value))
            if (name == Configuration.stnBeepId)
            {
                await jbc.SetStationBeepAsync(myID, (OnOff)int.Parse(value));
            }
            if (name == Configuration.stnHelpId)
            {
                await jbc.SetStationHelpTextAsync(myID, (OnOff)int.Parse(value));
            }
            if (name == Configuration.stnN2Id)
            {
                await jbc.SetStationN2ModeAsync(myID, (OnOff)int.Parse(value));
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
        private bool toolSettingsParams = false;

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
            string[] tempoptFix = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(auxMinTemp, false, true), Configuration.convertTempToString(auxMaxTemp, false, true), "0" };
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
            if (!features.TempLevelsWithStatus)
            {
                toolParamsTable.addParam(Configuration.toolFixTempId, Localization.getResStr(Configuration.toolFixTempId), ParamTable.cInputType.NUMBER, tempoptFix, null);
            }

            string[] tempLvlsValues = Configuration.getTempLevels(Configuration.arrOption.VALUES, features.TempLevelsWithStatus);
            string[] tempLvls = Configuration.getTempLevels(Configuration.arrOption.TEXTS, features.TempLevelsWithStatus);

            string[] sleepDelaysValues = Configuration.getSleepDelays(Configuration.arrOption.VALUES, features.DelayWithStatus);
            string[] sleepDelays = Configuration.getSleepDelays(Configuration.arrOption.TEXTS, features.DelayWithStatus);

            string[] hiberDelaysValues = Configuration.getHiberDelays(Configuration.arrOption.VALUES, features.DelayWithStatus);
            string[] hiberDelays = Configuration.getHiberDelays(Configuration.arrOption.TEXTS, features.DelayWithStatus);

            if (features.Cartridges)
            {
                toolParamsTable.addParam(Configuration.toolCartridgeId, Localization.getResStr(Configuration.toolCartridgeId), ParamTable.cInputType.BUTTON, null, null, features.Cartridges);
            }

            toolParamsTable.addParam(Configuration.toolSelectedTempLvlId, Localization.getResStr(Configuration.toolSelectedTempLvlId), ParamTable.cInputType.SWITCH, tempLvlsValues, tempLvls, features.TempLevelsWithStatus);
            toolParamsTable.addParam(Configuration.toolTempLvl1Id, "    " + Localization.getResStr(Configuration.toolTempLvl1Id), ParamTable.cInputType.NUMBER, tempopt, null, features.TempLevelsWithStatus, 3);
            toolParamsTable.addParam(Configuration.toolTempLvl2Id, "    " + Localization.getResStr(Configuration.toolTempLvl2Id), ParamTable.cInputType.NUMBER, tempopt, null, features.TempLevelsWithStatus, 3);
            toolParamsTable.addParam(Configuration.toolTempLvl3Id, "    " + Localization.getResStr(Configuration.toolTempLvl3Id), ParamTable.cInputType.NUMBER, tempopt, null, features.TempLevelsWithStatus, 3);

            toolParamsTable.addParam(Configuration.toolSleepTempId, Localization.getResStr(Configuration.toolSleepTempId), ParamTable.cInputType.NUMBER, tempopt, null, false, 3);
            toolParamsTable.addParam(Configuration.toolSleepDelayId, Localization.getResStr(Configuration.toolSleepDelayId), ParamTable.cInputType.DROPLIST, sleepDelaysValues, sleepDelays, features.DelayWithStatus);

            toolParamsTable.addParam(Configuration.toolHibernationDelayId, Localization.getResStr(Configuration.toolHibernationDelayId), ParamTable.cInputType.DROPLIST, hiberDelaysValues, hiberDelays, features.DelayWithStatus);
            toolParamsTable.addParam(Configuration.toolAdjustTempId, Localization.getResStr(Configuration.toolAdjustTempId), ParamTable.cInputType.NUMBER, tempoptAdj, null, false, 4);

            toolParamsTable.ptyDefaultRowHeight = 22;

            // Adding the table to the page panel
            tabPages.pageToolSettings.Controls.Add(toolParamsTable);
            ToolTipIcons.SetToolTip(iconToolSettings, Localization.getResStr(Configuration.toolTabHintId));

            tabPages.lblSelectedTool.Text = Localization.getResStr(Configuration.toolSelectedToolId);

            //Forcing update
            toolSettingsParams = true;
        }

        private void loadTextsToolSettingsPage()
        {

            // update texts
            CTemperature auxMaxTemp = jbc.GetStationFeatures(myID).MaxTemp; // getMaxTemp(jbc.GetStationModel(myID))
            CTemperature auxMinTemp = jbc.GetStationFeatures(myID).MinTemp;
            string[] tempopt = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(auxMinTemp, false, true), Configuration.convertTempToString(auxMaxTemp, false, true) };
            string[] tempoptFix = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempToString(auxMinTemp, false, true), Configuration.convertTempToString(auxMaxTemp, false, true), "0" };
            string[] tempoptAdj = new string[] { Configuration.sReplaceTag + " " + Configuration.Tunits, Configuration.convertTempAdjToString(Configuration.tempMinAdj, false), Configuration.convertTempAdjToString(Configuration.tempMaxAdj, false) };
            string[] tempLvls = Configuration.getTempLevels(Configuration.arrOption.TEXTS, features.TempLevelsWithStatus);
            string[] sleepDelays = Configuration.getSleepDelays(Configuration.arrOption.TEXTS, features.DelayWithStatus);
            string[] hiberDelays = Configuration.getHiberDelays(Configuration.arrOption.TEXTS, features.DelayWithStatus);

            //toolParamsTable.setText(toolSelectedTempId, getResStr(toolSelectedTempId), tempopt)
            if (!features.TempLevelsWithStatus)
            {
                toolParamsTable.setText(Configuration.toolFixTempId, Localization.getResStr(Configuration.toolFixTempId), tempoptFix);
            }
            toolParamsTable.setText(Configuration.toolSelectedTempLvlId, Localization.getResStr(Configuration.toolSelectedTempLvlId), tempLvls);
            toolParamsTable.setText(Configuration.toolTempLvl1Id, "    " + Localization.getResStr(Configuration.toolTempLvl1Id), tempopt);
            toolParamsTable.setText(Configuration.toolTempLvl2Id, "    " + Localization.getResStr(Configuration.toolTempLvl2Id), tempopt);
            toolParamsTable.setText(Configuration.toolTempLvl3Id, "    " + Localization.getResStr(Configuration.toolTempLvl3Id), tempopt);
            toolParamsTable.setText(Configuration.toolSleepTempId, Localization.getResStr(Configuration.toolSleepTempId), tempopt);
            toolParamsTable.setText(Configuration.toolSleepDelayId, Localization.getResStr(Configuration.toolSleepDelayId), sleepDelays);
            toolParamsTable.setText(Configuration.toolHibernationDelayId, Localization.getResStr(Configuration.toolHibernationDelayId), hiberDelays);
            toolParamsTable.setText(Configuration.toolAdjustTempId, Localization.getResStr(Configuration.toolAdjustTempId), tempoptAdj);
            if (features.Cartridges)
            {
                toolParamsTable.setText(Configuration.toolCartridgeId, Localization.getResStr(Configuration.toolCartridgeId), null);
            }

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
                    if (Configuration.myControlExists(this, "rbToolSettings_" + toolConnectedToolPort.ToString(), ref ctrl))
                    {
                        rb.BackgroundImage = null;
                    }
                }
                // show port in connected tool
                toolConnectedToolPort = jbc.GetPortToolID(myID, toolCurPort);
                if (toolConnectedToolPort != GenericStationTools.NO_TOOL)
                {
                    if (Configuration.myControlExists(this, "rbToolSettings_" + toolConnectedToolPort.ToString(), ref ctrl))
                    {
                        //CType(rb, RadioButton).BackgroundImage = My.Resources.ToolConnected_mini
                        rb.BackgroundImage = (Image)My.Resources.Resources.ResourceManager.GetObject("ToolConnected_mini_" + iClickedPort.ToString());
                    }
                }
                toolSettingsParams = true;
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
                toolSettingsParams = true;
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
                    toolSettingsParams = true;
                }
            }

            //If update necessary
            if (toolSettingsParams)
            {

                // Retrieve remote data
                await jbc.UpdatePortToolSettingsAsync(myID, toolCurPort, toolCurTool);

                //toolParamsTable.setValue(toolSelectedTempId, convertTempToString(jbc.GetPortToolSelectedTemp(myID, toolCurPort)))
                CTemperature temp = default(CTemperature);
                if (!features.TempLevelsWithStatus)
                {
                    temp = jbc.GetPortToolFixTemp(myID, toolCurPort, toolCurTool);
                    if (temp != null)
                    {
                        if (temp.isValid())
                        {
                            toolParamsTable.setValue(Configuration.toolFixTempId, Configuration.convertTempToString(temp, false, true));
                        }
                        else
                        {
                            toolParamsTable.setValue(Configuration.toolFixTempId, Configuration.noDataStr);
                        }
                    }
                    else
                    {
                        toolParamsTable.setValue(Configuration.toolFixTempId, Configuration.noDataStr);
                    }
                }

                // levels
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

                toolParamsTable.setValue(Configuration.toolSleepTempId, Configuration.convertTempToString(jbc.GetPortToolSleepTemp(myID, toolCurPort, toolCurTool), false, true));
                toolParamsTable.setValue(Configuration.toolSleepDelayId, Convert.ToString(jbc.GetPortToolSleepDelay(myID, toolCurPort, toolCurTool)));
                if (features.DelayWithStatus)
                {
                    toolParamsTable.setCheck(Configuration.toolSleepDelayId, jbc.GetPortToolSleepDelayEnabled(myID, toolCurPort, toolCurTool) == OnOff._ON);
                }
                toolParamsTable.setValue(Configuration.toolHibernationDelayId, Convert.ToString(jbc.GetPortToolHibernationDelay(myID, toolCurPort, toolCurTool)));
                if (features.DelayWithStatus)
                {
                    toolParamsTable.setCheck(Configuration.toolHibernationDelayId, jbc.GetPortToolHibernationDelayEnabled(myID, toolCurPort, toolCurTool) == OnOff._ON);
                }
                // toolParamsTable.setValue(toolAdjustTempId, convertTempToString(jbc.GetPortToolAdjustTemp(myID, toolCurPort, toolCurTool), False))
                // do not convert UTI->Celsius or Fahrenheit if UTI = 0
                temp = jbc.GetPortToolAdjustTemp(myID, toolCurPort, toolCurTool);
                if (temp.UTI == 0)
                {
                    toolParamsTable.setValue(Configuration.toolAdjustTempId, "0");
                }
                else
                {
                    toolParamsTable.setValue(Configuration.toolAdjustTempId, Configuration.convertTempAdjToString(temp, false));
                }

                //cartridge
                if (features.Cartridges)
                {
                    bool bCartridgeOnOff = jbc.GetPortToolCartridgeEnabled(myID, toolCurPort, toolCurTool) == OnOff._ON;
                    ushort usCartridgeNbr = jbc.GetPortToolCartridge(myID, toolCurPort, toolCurTool);
                    toolParamsTable.setValue(Configuration.toolCartridgeId, Strings.Format(usCartridgeNbr, "000"));
                    toolParamsTable.setCheck(Configuration.toolCartridgeId, bCartridgeOnOff);
                }

                //Update done
                toolSettingsParams = false;
            }
        }

        private async void toolParamsTable_NewValue(string name, string value)
        {
            //Some value has been changed, sending it to the station
            //If name = "toolSelectedTemp" Then jbc.SetPortToolSelectedTemp(myID, toolCurPort, convertStringToTemp(value))
            if (name == Configuration.toolFixTempId)
            {
                if (value == "" || value == "0" || value == Configuration.noDataStr)
                {
                    await jbc.SetPortToolFixTempAsync(myID, toolCurPort, toolCurTool, OnOff._OFF);
                }
                else
                {
                    await jbc.SetPortToolFixTempAsync(myID, toolCurPort, toolCurTool, Configuration.convertStringToTemp(value));
                }
                WorkParams = true; // update work page also
            }
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
            if (name == Configuration.toolSleepTempId)
            {
                await jbc.SetPortToolSleepTempAsync(myID, toolCurPort, toolCurTool, Configuration.convertStringToTemp(value));
            }
            if (name == Configuration.toolSleepDelayId)
            {
                await jbc.SetPortToolSleepDelayAsync(myID, toolCurPort, toolCurTool, (ToolTimeSleep)int.Parse(value));
            }
            if (name == Configuration.toolHibernationDelayId)
            {
                await jbc.SetPortToolHibernationDelayAsync(myID, toolCurPort, toolCurTool, (ToolTimeHibernation)int.Parse(value));
            }
            if (name == Configuration.toolAdjustTempId)
            {
                CTemperature tempAdj = Configuration.convertStringToTempAdj(value);
                await jbc.SetPortToolAdjustTempAsync(myID, toolCurPort, toolCurTool, tempAdj);
            }

            //Updating
            toolSettingsParams = true;
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
            if (name == Configuration.toolCartridgeId)
            {
                int iSelected = 0;
                try
                {
                    iSelected = int.Parse(toolParamsTable.getValue(Configuration.toolCartridgeId));
                    CCartridgeData cartridge = new CCartridgeData();
                    cartridge.CartridgeNbr = System.Convert.ToUInt16(iSelected);
                    cartridge.CartridgeOnOff = onoff;
                    await jbc.SetPortToolCartridgeAsync(myID, toolCurPort, toolCurTool, cartridge);
                    WorkParams = true; // update work page also
                }
                catch (Exception)
                {
                }
            }
            if (name == Configuration.toolSleepDelayId)
            {
                await jbc.SetPortToolSleepDelayEnabledAsync(myID, toolCurPort, toolCurTool, onoff);
            }
            if (name == Configuration.toolHibernationDelayId)
            {
                await jbc.SetPortToolHibernationDelayEnabledAsync(myID, toolCurPort, toolCurTool, onoff);
            }

            //Updating
            toolSettingsParams = true;
        }

        private async void toolParamsTable_ButtonClicked(string name, string value)
        {
            //Some value is clicked (defined as button to be processed out of the paramater list)
            if (name == Configuration.toolCartridgeId)
            {
                // create and show dialog for cartridge form
                int iSelected = 0;
                try
                {
                    iSelected = int.Parse(value);
                }
                catch (Exception)
                {
                    iSelected = 0;
                }
                frmCartridges frmCart = new frmCartridges(toolCurTool, iSelected, jbc.GetStationModel(myID));
                if (mode == (int)ControlModeConnection.CONTROL)
                {
                    frmCart.butOk.Enabled = true;
                }
                else
                {
                    frmCart.butOk.Enabled = false;
                }

                tmr.Stop();
                if (frmCart.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        iSelected = int.Parse(frmCart.tbCartridgeNumber.Text);
                        OnOff onoff = OnOff._OFF;
                        if (toolParamsTable.getCheck(Configuration.toolCartridgeId))
                        {
                            onoff = OnOff._ON;
                        }
                        CCartridgeData cartridge = new CCartridgeData();
                        cartridge.CartridgeNbr = System.Convert.ToUInt16(iSelected);
                        cartridge.CartridgeOnOff = onoff;
                        await jbc.SetPortToolCartridgeAsync(myID, toolCurPort, toolCurTool, cartridge);
                        //Updating
                        toolSettingsParams = true;
                    }
                    catch (Exception)
                    {
                    }
                }
                frmCart.Dispose();
                tmr.Start();

            }
        }

        #endregion

        #region PERIPHERAL PAGE

        private int ASCII_A_LOWER_CODE = 97;

        private bool m_updatePeripheralParams = false;
        private CPeripheralData[] m_peripherals;
        private int m_selectedConfigPeripheral = 0; //Peripheral to show un right panel
        private bool m_skipChangesPeripheralEvent = false; //Skip if a input event is raised
        private int m_lastMonitorMode = -1;


        private void createPeripheralPage()
        {
            if (features.Peripherals)
            {
                tabPages.labelPeripheralPort2.Visible = jbc.GetPortCount(myID) > 1;
                tabPages.labelPeripheralPort3.Visible = jbc.GetPortCount(myID) > 2;
                tabPages.labelPeripheralPort4.Visible = jbc.GetPortCount(myID) > 3;

                tabPages.inputFunctionPeripheral.SelectedValueChanged += ConfigPeripheral_Change;
                tabPages.inputActivationPeripheral.SelectedValueChanged += ConfigPeripheral_Change;
                tabPages.inputTimePeripheral.TextChanged += ConfigPeripheral_Change;

                loadTextsPeripheralPage();

                //Forcing update
                m_updatePeripheralParams = false;
            }
            else
            {
                tabPages.lblNoPeripheralSupported.Visible = true;
            }

            ToolTipIcons.SetToolTip(iconPeripheral, Localization.getResStr(Configuration.peripheralTabHintId));
        }

        private void loadTextsPeripheralPage()
        {
            tabPages.lblNoPeripheral.Text = Localization.getResStr(Configuration.peripheralNoPeripheralId).ToUpper();
            tabPages.lblNoPeripheralSupported.Text = Localization.getResStr(Configuration.peripheralNoSupportedId).ToUpper();

            tabPages.textFunctionPeripheral.Text = Localization.getResStr(Configuration.peripheralFunctionId);

            tabPages.inputFunctionPeripheral.Items.Clear();
            tabPages.inputFunctionPeripheral.Items.Add(Localization.getResStr(Configuration.peripheralSleepId));
            tabPages.inputFunctionPeripheral.Items.Add(Localization.getResStr(Configuration.peripheralExtractorId));
            tabPages.inputFunctionPeripheral.Items.Add(Localization.getResStr(Configuration.peripheralModuleId));

            tabPages.textActivationPeripheral.Text = Localization.getResStr(Configuration.peripheralActivationId);

            tabPages.inputActivationPeripheral.Items.Clear();
            tabPages.inputActivationPeripheral.Items.Add(Localization.getResStr(Configuration.peripheralPressedId));
            tabPages.inputActivationPeripheral.Items.Add(Localization.getResStr(Configuration.peripheralReleasedId));

            tabPages.textTimePeripheral.Text = Localization.getResStr(Configuration.peripheralTimeId);
            tabPages.inputErrorTimePeripheral.Text = Localization.getResStr(Configuration.peripheralErrorTimeId);
        }

        private async void updatePeripheralPage()
        {
            if (!features.Peripherals)
            {
                return;
            }

            //
            // Mode has been changed
            //
            if (mode != m_lastMonitorMode)
            {
                if (mode == (int)ControlModeConnection.MONITOR)
                {
                    tabPages.inputFunctionPeripheral.Enabled = false;
                    tabPages.inputActivationPeripheral.Enabled = false;
                    tabPages.inputTimePeripheral.Enabled = false;

                    CheckBox ChkBox = null;
                    foreach (object xObject in tabPages.tlpPeripherals.Controls)
                    {
                        if (xObject is CheckBox)
                        {
                            ChkBox = (CheckBox)xObject;
                            ChkBox.Enabled = false;
                        }
                    }
                }
                else if (mode == (int)ControlModeConnection.CONTROL)
                {
                    tabPages.inputFunctionPeripheral.Enabled = true;
                    tabPages.inputActivationPeripheral.Enabled = true;
                    tabPages.inputTimePeripheral.Enabled = true;

                    CheckBox ChkBox = null;
                    foreach (object xObject in tabPages.tlpPeripherals.Controls)
                    {
                        if (xObject is CheckBox)
                        {
                            ChkBox = (CheckBox)xObject;
                            ChkBox.Enabled = true;
                        }
                    }
                }

                m_lastMonitorMode = mode;
            }

            //
            // Only if parameter update required
            //
            if (m_updatePeripheralParams)
            {

                bool bFixHeight = false;

                // Retrieve remote data
                await jbc.UpdatePeripheralsAsync(myID);
                m_peripherals = jbc.GetPeripherals(myID);
                if (ReferenceEquals(m_peripherals, null))
                {
                    return;
                }

                if (m_peripherals.Count() == 0)
                {
                    tabPages.lblNoPeripheral.Visible = true;
                    tabPages.tlpPeripherals.Visible = false;
                    tabPages.panelConfigPeripheral.Visible = false;
                }
                else
                {
                    tabPages.lblNoPeripheral.Visible = false;
                    tabPages.tlpPeripherals.Visible = true;
                    tabPages.panelConfigPeripheral.Visible = true;
                }

                //
                //Crear elementos
                //
                while (tabPages.tlpPeripherals.RowCount - 1 < m_peripherals.Count())
                {
                    tabPages.tlpPeripherals.RowCount++;

                    int nActRow = tabPages.tlpPeripherals.RowCount - 1;

                    //Name
                    LinkLabel labelName = new LinkLabel();
                    labelName.LinkClicked += ConfigurationPeripheral_Click;
                    labelName.LinkBehavior = LinkBehavior.NeverUnderline;
                    labelName.TextAlign = ContentAlignment.MiddleLeft;
                    labelName.Font = new Font(labelName.Font.Name, 11, labelName.Font.Style, labelName.Font.Unit);
                    labelName.LinkColor = Color.FromArgb(64, 64, 64);
                    labelName.VisitedLinkColor = Color.FromArgb(64, 64, 64);
                    labelName.ActiveLinkColor = Color.FromArgb(238, 160, 74);
                    labelName.MouseEnter += LabelName_MouseEnter;
                    labelName.MouseLeave += LabelName_MouseLeave;
                    tabPages.tlpPeripherals.Controls.Add(labelName, 0, nActRow);

                    //Remove Margins
                    tabPages.tlpPeripherals.GetControlFromPosition(0, nActRow).Margin = new Padding(0);

                    //Ports 1, 2, 3, 4
                    CheckBox cbPort1 = new CheckBox();
                    cbPort1.CheckAlign = ContentAlignment.MiddleCenter;
                    tabPages.tlpPeripherals.Controls.Add(cbPort1, 1, nActRow);
                    cbPort1.Enabled = mode == (int)ControlModeConnection.CONTROL;
                    cbPort1.CheckedChanged += CheckBoxPortPeripheral_Click;
                    tabPages.tlpPeripherals.GetControlFromPosition(1, nActRow).Margin = new Padding(0);

                    if (jbc.GetPortCount(myID) > 1)
                    {
                        CheckBox cbPort2 = new CheckBox();
                        cbPort2.CheckAlign = ContentAlignment.MiddleCenter;
                        tabPages.tlpPeripherals.Controls.Add(cbPort2, 2, nActRow);
                        cbPort2.Enabled = mode == (int)ControlModeConnection.CONTROL;
                        cbPort2.CheckedChanged += CheckBoxPortPeripheral_Click;
                        tabPages.tlpPeripherals.GetControlFromPosition(2, nActRow).Margin = new Padding(0);

                        if (jbc.GetPortCount(myID) > 2)
                        {
                            CheckBox cbPort3 = new CheckBox();
                            cbPort3.CheckAlign = ContentAlignment.MiddleCenter;
                            tabPages.tlpPeripherals.Controls.Add(cbPort3, 3, nActRow);
                            cbPort3.Enabled = mode == (int)ControlModeConnection.CONTROL;
                            cbPort3.CheckedChanged += CheckBoxPortPeripheral_Click;
                            tabPages.tlpPeripherals.GetControlFromPosition(3, nActRow).Margin = new Padding(0);

                            if (jbc.GetPortCount(myID) > 3)
                            {
                                CheckBox cbPort4 = new CheckBox();
                                cbPort4.CheckAlign = ContentAlignment.MiddleCenter;
                                tabPages.tlpPeripherals.Controls.Add(cbPort4, 4, nActRow);
                                cbPort4.Enabled = mode == (int)ControlModeConnection.CONTROL;
                                cbPort4.CheckedChanged += CheckBoxPortPeripheral_Click;
                                tabPages.tlpPeripherals.GetControlFromPosition(4, nActRow).Margin = new Padding(0);
                            }
                        }
                    }

                    bFixHeight = true;
                }

                //
                //Borrar elementos
                //
                while (tabPages.tlpPeripherals.RowCount - 1 > m_peripherals.Count())
                {
                    RoutinesLibrary.UI.TableLayoutPanelUtils.RemoveRow(tabPages.tlpPeripherals, tabPages.tlpPeripherals.RowCount - 1);
                    bFixHeight = true;
                }

                //Ajustar altura elementos
                if (bFixHeight)
                {
                    bool bFirstRowStyle = true;
                    foreach (RowStyle style in tabPages.tlpPeripherals.RowStyles)
                    {
                        style.SizeType = SizeType.Absolute;
                        style.Height = 23;
                    }
                    tabPages.tlpPeripherals.RowStyles[0].Height = 46;
                }

                //Cambiar el selected por si el elemento mostrado se ha borrado
                if (m_peripherals.Count() <= m_selectedConfigPeripheral)
                {
                    m_selectedConfigPeripheral = 0;
                }

                //
                //Actualizar elementos
                //
                int nCountMN = 0;
                int nCountMS = 0;
                int nCountFS = 0;
                int nCountMV = 0;
                int nCountPD = 0;

                for (var i = 0; i <= m_peripherals.Count() - 1; i++)
                {
                    //Name
                    string strNamePeriph = "";

                    if (m_peripherals[(int)i].Type == CPeripheralData.PeripheralType.MN)
                    {
                        strNamePeriph += "MN " + System.Convert.ToString(Strings.Chr(ASCII_A_LOWER_CODE + nCountMN));
                        nCountMN++;
                    }
                    else if (m_peripherals[(int)i].Type == CPeripheralData.PeripheralType.MS)
                    {
                        strNamePeriph += "MS " + System.Convert.ToString(Strings.Chr(ASCII_A_LOWER_CODE + nCountMS));
                        nCountMS++;
                    }
                    else if (m_peripherals[(int)i].Type == CPeripheralData.PeripheralType.FS)
                    {
                        strNamePeriph += "FS " + System.Convert.ToString(Strings.Chr(ASCII_A_LOWER_CODE + nCountFS));
                        nCountFS++;
                    }
                    else if (m_peripherals[(int)i].Type == CPeripheralData.PeripheralType.MV)
                    {
                        strNamePeriph += "MV " + System.Convert.ToString(Strings.Chr(ASCII_A_LOWER_CODE + nCountMV));
                        nCountFS++;
                    }
                    else if (m_peripherals[(int)i].Type == CPeripheralData.PeripheralType.PD)
                    {
                        strNamePeriph += "PD " + System.Convert.ToString(Strings.Chr(ASCII_A_LOWER_CODE + nCountPD));
                        nCountPD++;
                    }
                    else
                    {
                        strNamePeriph += "";
                    }

                    if (tabPages.tlpPeripherals.GetControlFromPosition(0, System.Convert.ToInt32(i + 1)).Text != strNamePeriph)
                    {
                        tabPages.tlpPeripherals.GetControlFromPosition(0, System.Convert.ToInt32(i + 1)).Text = strNamePeriph;
                    }

                    //Port check
                    m_skipChangesPeripheralEvent = true;
                    if (((CheckBox)(tabPages.tlpPeripherals.GetControlFromPosition(1, System.Convert.ToInt32(i + 1)))).Checked != (m_peripherals[(int)i].PortAttached == Port.NUM_1))
                    {
                        ((CheckBox)(tabPages.tlpPeripherals.GetControlFromPosition(1, System.Convert.ToInt32(i + 1)))).Checked = m_peripherals[(int)i].PortAttached == Port.NUM_1;
                    }

                    if (jbc.GetPortCount(myID) > 1)
                    {
                        ((CheckBox)(tabPages.tlpPeripherals.GetControlFromPosition(2, System.Convert.ToInt32(i + 1)))).Checked = m_peripherals[(int)i].PortAttached == Port.NUM_2;

                        if (jbc.GetPortCount(myID) > 2)
                        {
                            ((CheckBox)(tabPages.tlpPeripherals.GetControlFromPosition(3, System.Convert.ToInt32(i + 1)))).Checked = m_peripherals[(int)i].PortAttached == Port.NUM_3;

                            if (jbc.GetPortCount(myID) > 3)
                            {
                                ((CheckBox)(tabPages.tlpPeripherals.GetControlFromPosition(4, System.Convert.ToInt32(i + 1)))).Checked = m_peripherals[(int)i].PortAttached == Port.NUM_4;
                            }
                        }

                    }
                    m_skipChangesPeripheralEvent = false;
                }

                //
                //Panel lateral de configuración
                //
                LoadPanelConfigurationPeripheral();

                tabPages.tlpPeripherals.Refresh();

                // Update done
                m_updatePeripheralParams = false;
            }
        }

        private void ConfigurationPeripheral_Click(object sender, System.EventArgs e)
        {

            int row = tabPages.tlpPeripherals.GetCellPosition((Control)sender).Row;
            m_selectedConfigPeripheral = row - 1;

            LoadPanelConfigurationPeripheral();
        }

        private void LoadPanelConfigurationPeripheral()
        {

            if (m_peripherals.Count() <= m_selectedConfigPeripheral | m_selectedConfigPeripheral < 0)
            {
                return;
            }

            //Name
            if (tabPages.textNamePeripheral.Text != tabPages.tlpPeripherals.GetControlFromPosition(0, m_selectedConfigPeripheral + 1).Text)
            {
                tabPages.textNamePeripheral.Text = tabPages.tlpPeripherals.GetControlFromPosition(0, m_selectedConfigPeripheral + 1).Text;
            }

            //Type
            if (m_peripherals[m_selectedConfigPeripheral].Type == CPeripheralData.PeripheralType.PD)
            {
                tabPages.textTypePeripheral.Text = Localization.getResStr(Configuration.peripheralPedalId);
            }
            else if (m_peripherals[m_selectedConfigPeripheral].Type == CPeripheralData.PeripheralType.MS)
            {
                tabPages.textTypePeripheral.Text = Localization.getResStr(Configuration.peripheralElectricDesId);
            }
            else if (m_peripherals[m_selectedConfigPeripheral].Type == CPeripheralData.PeripheralType.MN)
            {
                tabPages.textTypePeripheral.Text = Localization.getResStr(Configuration.peripheralNitrogenId);
            }
            else if (m_peripherals[m_selectedConfigPeripheral].Type == CPeripheralData.PeripheralType.FS)
            {
                tabPages.textTypePeripheral.Text = Localization.getResStr(Configuration.peripheralFumeExtId);
            }
            else if (m_peripherals[m_selectedConfigPeripheral].Type == CPeripheralData.PeripheralType.MV)
            {
                tabPages.textTypePeripheral.Text = Localization.getResStr(Configuration.peripheralPneumaticDesId);
            }
            else
            {
                tabPages.textTypePeripheral.Text = "";
            }

            //Make visible the right panel
            if (CPeripheralData.PeripheralType.IsDefined(typeof(CPeripheralData.PeripheralType), m_peripherals[m_selectedConfigPeripheral].Type) && m_peripherals[m_selectedConfigPeripheral].Type == CPeripheralData.PeripheralType.PD)
            {
                tabPages.panelConfigParamPeripheral.Visible = true;

                m_skipChangesPeripheralEvent = true;
                //Function
                if ((tabPages.inputFunctionPeripheral.SelectedIndex != (int)m_peripherals[m_selectedConfigPeripheral].WorkFunction) &&
                    !tabPages.inputFunctionPeripheral.Focused &&
                    CPeripheralData.PeripheralFunction.IsDefined(typeof(CPeripheralData.PeripheralFunction), m_peripherals[m_selectedConfigPeripheral].WorkFunction))
                {
                    tabPages.inputFunctionPeripheral.SelectedIndex = (System.Int32)(m_peripherals[m_selectedConfigPeripheral].WorkFunction);
                }
                //Activation
                if (tabPages.inputActivationPeripheral.SelectedIndex != (int)m_peripherals[m_selectedConfigPeripheral].ActivationMode &&
                    !tabPages.inputActivationPeripheral.Focused &&
                    CPeripheralData.PeripheralActivation.IsDefined(typeof(CPeripheralData.PeripheralActivation), m_peripherals[m_selectedConfigPeripheral].ActivationMode))
                {
                    tabPages.inputActivationPeripheral.SelectedIndex = (System.Int32)(m_peripherals[m_selectedConfigPeripheral].ActivationMode);
                }
                //Time
                if (tabPages.inputTimePeripheral.Text != string.Format("{0}.{1}", m_peripherals[m_selectedConfigPeripheral].DelayTime / 10, m_peripherals[m_selectedConfigPeripheral].DelayTime % 10) && !tabPages.inputTimePeripheral.Focused)
                {
                    tabPages.inputTimePeripheral.Text = string.Format("{0}.{1}", m_peripherals[m_selectedConfigPeripheral].DelayTime / 10, m_peripherals[m_selectedConfigPeripheral].DelayTime % 10);
                    tabPages.inputErrorTimePeripheral.Visible = false;
                }
                m_skipChangesPeripheralEvent = false;
            }
            else
            {
                tabPages.panelConfigParamPeripheral.Visible = false;
            }
        }

        private async void CheckBoxPortPeripheral_Click(object sender, System.EventArgs e)
        {

            if (m_skipChangesPeripheralEvent)
            {
                return;
            }

            int row = tabPages.tlpPeripherals.GetCellPosition((Control)sender).Row;
            int column = tabPages.tlpPeripherals.GetCellPosition((Control)sender).Column;

            if (((CheckBox)sender).Checked == false)
            {
                m_peripherals[row - 1].PortAttached = Port.NO_PORT;
            }
            else
            {
                m_peripherals[row - 1].PortAttached = (Port)(column - 1);
            }

            await jbc.SetPeripheralAsync(myID, m_peripherals[row - 1]);
        }

        private async void ConfigPeripheral_Change(object sender, System.EventArgs e)
        {

            if (m_skipChangesPeripheralEvent)
            {
                return;
            }

            //Check time
            if (tabPages.inputTimePeripheral.Text.Length < 1)
            {
                return;
            }

            short nTime = (short)(-1);
            double fTime = 0;
            if (double.TryParse(tabPages.inputTimePeripheral.Text.Replace(".", ","), out fTime))
            {
                if (fTime < short.MaxValue & fTime > short.MinValue)
                {
                    nTime = (short)(fTime * 10);
                }
            }

            if (nTime < 0 | nTime > 99)
            {
                tabPages.inputErrorTimePeripheral.Visible = true;
                return;
            }
            else
            {
                tabPages.inputErrorTimePeripheral.Visible = false;
            }

            m_peripherals[m_selectedConfigPeripheral].WorkFunction = (CPeripheralData.PeripheralFunction)tabPages.inputFunctionPeripheral.SelectedIndex;
            m_peripherals[m_selectedConfigPeripheral].ActivationMode = (CPeripheralData.PeripheralActivation)tabPages.inputActivationPeripheral.SelectedIndex;
            m_peripherals[m_selectedConfigPeripheral].DelayTime = nTime;

            await jbc.SetPeripheralAsync(myID, m_peripherals[m_selectedConfigPeripheral]);
        }

        // Mouse over color enabled
        private void LabelName_MouseEnter(object sender, System.EventArgs e)
        {
            ((LinkLabel)sender).LinkColor = Color.FromArgb(238, 160, 74);
        }

        // Mouse over color disabled
        private void LabelName_MouseLeave(object sender, System.EventArgs e)
        {
            ((LinkLabel)sender).LinkColor = Color.FromArgb(64, 64, 64);
        }

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
            switch (ctrl.Name)
            {
                case "butConfLoad":
                    //Only if control mode
                    if (mode == (int)ControlModeConnection.CONTROL)
                    {
                        var ldgXmlLoad = new OpenFileDialog();
                        ldgXmlLoad.Filter = " (*.xml)|*.xml";
                        if (ldgXmlLoad.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            //Loading
                            xmlLoad(ldgXmlLoad.FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Localization.getResStr(Configuration.confMustControlModeId));
                    }
                    break;

                case "butConfSave":
                    //Only if control mode
                    if (mode == (int)ControlModeConnection.CONTROL)
                    {
                        var sdgXmlSave = new SaveFileDialog();
                        sdgXmlSave.Filter = " (*.xml)|*.xml";
                        if (sdgXmlSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            //Saving
                            xmlSave(sdgXmlSave.FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show(Localization.getResStr(Configuration.confMustControlModeId));
                    }
                    break;

            }
        }

        private void xmlSave(string filename)
        {
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            // get settings from station in xml format
            xmlDoc = Configuration.confGetFromStation(myID, jbc, false);
            if (xmlDoc != null)
            {
                // save xml
                RoutinesLibrary.Data.Xml.XMLUtils.SaveToFile(xmlDoc, filename);
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
                Configuration.confSetToStation(xmlDoc, myID, jbc, iTargetPorts);
            }
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
            counterParamsTable.addParam(Configuration.counterSleepMinutesId, Localization.getResStr(Configuration.counterSleepMinutesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterHiberMinutesId, Localization.getResStr(Configuration.counterHiberMinutesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterNoToolMinutesId, Localization.getResStr(Configuration.counterNoToolMinutesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterSleepCyclesId, Localization.getResStr(Configuration.counterSleepCyclesId), ParamTable.cInputType.FIX, null, null);
            counterParamsTable.addParam(Configuration.counterDesolderCyclesId, Localization.getResStr(Configuration.counterDesolderCyclesId), ParamTable.cInputType.FIX, null, null);

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
            counterParamsTable.setText(Configuration.counterSleepMinutesId, Localization.getResStr(Configuration.counterSleepMinutesId), null);
            counterParamsTable.setText(Configuration.counterHiberMinutesId, Localization.getResStr(Configuration.counterHiberMinutesId), null);
            counterParamsTable.setText(Configuration.counterNoToolMinutesId, Localization.getResStr(Configuration.counterNoToolMinutesId), null);
            counterParamsTable.setText(Configuration.counterSleepCyclesId, Localization.getResStr(Configuration.counterSleepCyclesId), null);
            counterParamsTable.setText(Configuration.counterDesolderCyclesId, Localization.getResStr(Configuration.counterDesolderCyclesId), null);

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
                    counterCurPort = (Port)(System.Enum.Parse(typeof(Port), "NUM_" + Configuration.myGetRadioButtonPortNbr(rb).ToString()));
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
                counterParamsTable.setValue(Configuration.counterSleepMinutesId, Configuration.myGetStrFromMinutes(jbc.GetPortToolSleepMinutes(myID, counterCurPort, counterCurType)));
                counterParamsTable.setValue(Configuration.counterHiberMinutesId, Configuration.myGetStrFromMinutes(jbc.GetPortToolHibernationMinutes(myID, counterCurPort, counterCurType)));
                counterParamsTable.setValue(Configuration.counterNoToolMinutesId, Configuration.myGetStrFromMinutes(jbc.GetPortToolIdleMinutes(myID, counterCurPort, counterCurType)));
                counterParamsTable.setValue(Configuration.counterSleepCyclesId, System.Convert.ToString(jbc.GetPortToolSleepCycles(myID, counterCurPort, counterCurType).ToString()));
                counterParamsTable.setValue(Configuration.counterDesolderCyclesId, System.Convert.ToString(jbc.GetPortToolDesolderCycles(myID, counterCurPort, counterCurType).ToString()));

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

        #region UPDATER PAGE

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
