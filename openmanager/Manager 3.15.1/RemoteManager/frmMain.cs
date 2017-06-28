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

using Microsoft.Win32;
using System.IO;
using System.ServiceModel;
//using JBC_API_Remote;
//using RemoteManager.frmNotifications;
using RemoteManager.HostControllerServiceReference;
//using RemoteManRegister.ManRegister;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.CompilerServices;

using JBC_ConnectRemote;
using DataJBC;
using RemoteManRegister;
using RoutinesJBC;

// Tiene el menú de supervisor deshabilitado
// 05/09/2013 Se añaden rutinas para AutoScale (no habilitado)
// 22/04/2014 Se cambia la ubicación del archivo xml con la lista de estaciones en árbol (xmlStationGroupsFileName)


namespace RemoteManager
{
    public delegate void InitUpdateEventHandler(object sender);


    public partial class frmMain
    {

        #region Default Instance

        private static frmMain defaultInstance;

        /// <summary>
        /// Added by the VB.Net to C# Converter to support default instance behavour in C#
        /// </summary>
        public static frmMain Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new frmMain();
                    defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
                }

                return defaultInstance;
            }
            set
            {
                defaultInstance = value;
            }
        }

        static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        {
            defaultInstance = null;
        }

        #endregion
        // 22/04/2014
        private string APPDATA_DIRECTORY_OLD; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private string APPDATA_DIRECTORY; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

        public class tStation
        {
            public ListViewItem item = null;
            public long ID = -1;
            public eStationType StationType;
            public frmStation frm = null;
            public frmStation_HA frmHA = null;
            public bool bItemDataShowed;
            public string stationCOM = "";
            public bool bStationNamed = false;
            public tStation()
            {

            }
        }

        private JBC_API_Remote m_jbcConnect;
        private ManRegister reg;

        private List<tStation> stationList;
        private ListView m_lsvwStationList;
        private TreeView treevwStationList;
        private frmDockPanel frmStationListDockPanel;
        private ToolStripDropDownMenu mnuStationList;
        private ToolStripDropDownMenu mnuTreeStationList;
        private frmEvents frmEvents = null;
        private CUpdatesManager m_UpdatesManager;
        private frmAbout m_frmAbout = null;
        private frmNotifications m_frmNotifications = null;
        private frmOptions m_frmOptions = null;
        private frmMessagePopup m_frmMessagePopup = null;

        private string sSupervisorPW = "";
        private string sProgramAndOrInstallerVersion;
        private bool bStationsInitialControlMode = false;

        private CComHostController m_comHostController;
        private bool m_updateRequired = false;

        private bool m_stationListSearchingStation = false;


        public frmMain()
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            APPDATA_DIRECTORY_OLD = RoutinesLibrary.Data.DataType.StringUtils.chkSlash((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath);
            APPDATA_DIRECTORY = RoutinesLibrary.Data.DataType.StringUtils.chkSlash(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles)) + "JBC\\Manager\\";

            InitializeComponent();

            //Added to support default instance behavour in C#
            if (defaultInstance == null)
                defaultInstance = this;
            // 05/09/2013 For AutoScale font
            //AddHandler SystemEvents.UserPreferenceChanged, AddressOf SystemEvents_UserPreferenceChanged
            //Me.Font = SystemFonts.IconTitleFont

            CreateMutex(0, 0, "JBCMutexInstallApp");
        }

        //Place in Declarations section:
        [DllImport("kernel32", EntryPoint = "CreateMutexA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int CreateMutex(int lpMutexAttributes, int bInitialOwner, string lpName);


        private void SystemEvents_UserPreferenceChanged(object Sender, UserPreferenceChangedEventArgs e)
        {
            // 05/09/2013 For AutoScale font
            //If e.Category = UserPreferenceCategory.Window Then
            //Me.Font = SystemFonts.IconTitleFont
            //End If
        }

        public void frmMain_Load(object sender, System.EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            // disable main menu
            mspMain.Enabled = false;

            LoggerModule.InitLogger();

            //Notifications
            m_frmNotifications = new frmNotifications();
            m_frmNotifications.NotificationAvailable += Event_NotificationAvailable;
            m_frmNotifications.NotificationUnavailable += Event_NotificationUnavailable;
            m_frmNotifications.MdiParent = this;
            NotificationResize();

            //Initialize communications
            m_comHostController = new CComHostController();
            m_comHostController.SingleHostConnected += SingleHostControllerConnected;
            m_comHostController.MultipleHostConnected += MultipleHostControllerConnected;
            m_comHostController.HostDisconnected += HostControllerDisconnected;

            //Creating the station list
            stationList = new List<tStation>();

            //Creating the station panel
            createStationDockPanel();

            //Showing the station panel
            frmStationListDockPanel.Show();

            //Initialize panels
            m_UpdatesManager = new CUpdatesManager(this, m_comHostController, m_jbcConnect);
            m_UpdatesManager.CancelUpdatedReInstall += CancelUpdateReInstall;
            m_UpdatesManager.UpdateAvailable += UpdateAvailable;

            // view temperature units
            setViewTempUnits(System.Convert.ToString(My.Settings.Default.optTempUnits));

            // language
            Localization.curCulture = System.Convert.ToString(My.Settings.Default.optCulture);
            setCulture(Localization.curCulture);

            // notification
            Configuration.curShowErrorNotifications = System.Convert.ToBoolean(My.Settings.Default.optShowErrorNotifications);
            Configuration.curShowStationControllerNotifications = System.Convert.ToBoolean(My.Settings.Default.optShowStationControllerNotifications);
            Configuration.curShowStationNotifications = System.Convert.ToBoolean(My.Settings.Default.optShowStationNotifications);

            // supervisor password
            // deshabilitar
            mnuSupervisor.Visible = false;
            //sSupervisorPW = My.Settings.SupervisorPassword
            //If sSupervisorPW <> "0105" And sSupervisorPW <> "" Then sSupervisorPW = base64Decode(sSupervisorPW)

            // program version
            sProgramAndOrInstallerVersion = getProgramVersion();
            Text = Text + " - " + sProgramAndOrInstallerVersion;

            // load cursors
            Configuration.loadCursors();

            createEventWindow();

            //loading the jbc dll api
            //#edu#
            m_jbcConnect = new JBC_API_Remote();
            m_jbcConnect.NewStationConnected += jbc_NewStationConnected;
            m_jbcConnect.StationDisconnected += jbc_StationDisconnected;
            m_jbcConnect.HostDiscovered += jbc_HostDiscovered;
            m_jbcConnect.HostDisconnected += jbc_HostDisconnected;
            m_jbcConnect.UserError += jbc_UserError;
            m_jbcConnect.SwIncompatible += SoftwareIncompatible;

            // loading de manRegister API
            reg = new ManRegister(m_jbcConnect);

            //start search
            m_jbcConnect.StartSearch();

            timerApplStartUp.Start();

            this.KeyPreview = true;
            StationListSearchingStation(false);
        }

        public void timerApplStartUp_Tick(System.Object sender, System.EventArgs e)
        {
            // startup timer
            ((Timer)sender).Stop();

            // enable main menu
            mspMain.Enabled = true;

            //Adjust window size
            this.Size = My.Settings.Default.WindowSize;
            this.Location = My.Settings.Default.WindowLocation;
            if (My.Settings.Default.WindowStateMaximized || this.Location.X < 0 | this.Location.Y < 0)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.WindowState = FormWindowState.Normal;
            }
        }

        private string getProgramVersion()
        {
            string returnValue = "";
            RegistryKey regKey = default(RegistryKey);
            string sProgramVersion = "";
            string sInstallerVersion = "";

            // show version in window title
            // exe version
            sProgramVersion = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.Version.ToString();
            // get, if any, program version from installer
            try
            {
                regKey = Registry.LocalMachine.OpenSubKey("Software\\JBC\\JBC Remote Manager\\Settings", false);

                if ((regKey != null) && (Information.VarType(regKey) != VariantType.Null))
                {
                    sInstallerVersion = regKey.GetValue("Version", "").ToString();
                }
            }
            catch (Exception)
            {

            }
            if (!string.IsNullOrEmpty(sInstallerVersion))
            {
                returnValue = sInstallerVersion + " (" + sProgramVersion + ")";
            }
            else
            {
                returnValue = sProgramVersion;
            }

            return returnValue;
        }

        public void frmMain_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {

            // close all children
            foreach (Form frm in this.MdiChildren)
            {
                if (frm != null)
                {
                    frm.Close();
                }
            }

            //Save window size
            My.Settings.Default.WindowStateMaximized = this.WindowState == FormWindowState.Maximized; //Me.WindowState
            My.Settings.Default.WindowLocation = this.Location;
            My.Settings.Default.WindowSize = this.Size;

            My.Settings.Default.optCulture = Localization.curCulture;
            My.Settings.Default.optTempUnits = Configuration.curViewTempUnits;
            My.Settings.Default.optShowErrorNotifications = Configuration.curShowErrorNotifications;
            My.Settings.Default.optShowStationControllerNotifications = Configuration.curShowStationControllerNotifications;
            My.Settings.Default.optShowStationNotifications = Configuration.curShowStationNotifications;
            My.Settings.Default.Save();

            // 05/09/2013 For AutoScale font
            //RemoveHandler SystemEvents.UserPreferenceChanged, AddressOf SystemEvents_UserPreferenceChanged

            // stops jbc processes
            if (m_jbcConnect != null)
            {
                m_jbcConnect.Dispose();
            }

            // close me
            e.Cancel = false;

            ProjectData.EndApp();
        }

        private void SoftwareIncompatible()
        {
            if (m_updateRequired)
            {
                return;
            }

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SoftwareIncompatible()));
                return;
            }

            string swVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            //Comprueba si hay alguna actualización para instalar
            if (m_comHostController.CheckUpdateConnectedRemoteManager(swVersion))
            {
                //Si existe una actualización el remote manager no notificar de futuras incompatibilidades
                m_updateRequired = true;
                if (m_UpdatesManager != null)
                {
                    m_UpdatesManager.ShowFormReInstall();
                }
            }
        }

        #region View Temperature Units

        private void setViewTempUnits(string sTempUnits)
        {

            Configuration.curViewTempUnits = sTempUnits;
            if (sTempUnits == "C")
            {
                Configuration.Tunits = Configuration.CELSIUS_STR;
            }
            else
            {
                Configuration.Tunits = Configuration.FAHRENHEIT_STR;
            }
            // reload texts
            ReLoadStationsTexts();
            // refresh data (some data text has TempIUnits text)
            ReLoadStationsData();
        }

        #endregion

        #region Localization

        private void setCulture(string lang)
        {

            Localization.changeCulture(lang);
            ReLoadTexts();
            if (reg != null)
            {
                reg.reloadTexts();
            }
        }

        private void ReLoadTexts()
        {
            // load main form texts
            loadTextsMain();

            // load stations texts
            ReLoadStationsTexts();
            // refresh data (some data text has culture dependant text)
            ReLoadStationsData();

            // load Configuration Manager texts
            if (Configuration.frmConfMgr != null)
            {
                Configuration.frmConfMgr.ReLoadTexts();
            }

            // load events texts
            if (frmEvents != null)
            {
                frmEvents.ReLoadTexts();
            }

            // load updates texts
            m_UpdatesManager.ReLoadTexts();

            // load options texts
            if (m_frmOptions != null)
            {
                m_frmOptions.ReLoadTexts();
            }

            //Message popup
            if (m_frmMessagePopup != null)
            {
                m_frmMessagePopup.ReLoadTexts();
            }

            //Notification popup
            if (m_frmNotifications != null)
            {
                m_frmNotifications.ReLoadTexts();
            }

        }

        private void ReLoadStationsTexts()
        {
            // load stations texts
            foreach (var elem in stationList)
            {
                if (elem.frm != null)
                {
                    elem.frm.ReLoadTexts();
                }
                if (elem.frmHA != null)
                {
                    elem.frmHA.ReLoadTexts();
                }
            }
        }

        private void ReLoadStationsData()
        {
            // load stations data
            foreach (var elem in stationList)
            {
                if (elem.frm != null)
                {
                    elem.frm.RefreshSettingsPages(false); // all pages
                }
                if (elem.frmHA != null)
                {
                    elem.frmHA.RefreshSettingsPages(false); // all pages
                }
            }
        }

        private void loadTextsMain()
        {
            // main menues
            mnuSupervisor.Text = Localization.getResStr(Configuration.mnuSupervisorId);
            mnuLoginSupervisor.Text = Localization.getResStr(Configuration.mnuLoginSupervisorId);
            mnuLogoutSupervisor.Text = Localization.getResStr(Configuration.mnuLogoutSupervisorId);
            mnuChangePasswordSupervisor.Text = Localization.getResStr(Configuration.mnuChangePasswordId);

            mnuView.Text = Localization.getResStr(Configuration.mnuViewId);
            butViewStationList.Text = Localization.getResStr(Configuration.mnuViewStationListId);
            butViewEvents.Text = Localization.getResStr(Configuration.mnuViewEventsWindowId);
            mnuWarning.Text = Localization.getResStr(Configuration.mnuViewWarningId);

            mnuTools.Text = Localization.getResStr(Configuration.mnuToolsId);
            mnuSettingsManager.Text = Localization.getResStr(Configuration.mnuToolsSettingsManagerId);
            mnuRegisterManager.Text = Localization.getResStr(Configuration.mnuToolsRegisterManagerId);
            mnuOptions.Text = Localization.getResStr(Configuration.mnuOptionsId);

            mnuWindows.Text = Localization.getResStr(Configuration.mnuWindowsId);
            mnuCascade.Text = Localization.getResStr(Configuration.mnuCascadeId);
            mnuCascadeAll.Text = Localization.getResStr(Configuration.mnuCascadeAllId);

            mnuHelp.Text = Localization.getResStr(Configuration.mnuHelpId);
            mnuUpdates.Text = Localization.getResStr(Configuration.mnuUpdatesId);
            mnuAbout.Text = Localization.getResStr(Configuration.mnuAboutId);

            // dock panel menues
            // list menues
            mnuStationList.Items["mnuButViewForm"].Text = Localization.getResStr(Configuration.mnuViewParametersId);
            mnuStationList.Items["mnuButPrintCurrentSettings"].Text = Localization.getResStr(Configuration.mnuPrintCurrentSettingsId);
            mnuStationList.Items["mnuButUpdate"].Text = Localization.getResStr(Configuration.mnuUpdateSoftwareId);
            mnuStationList.Items["mnuButAddSerie"].Text = Localization.getResStr(Configuration.mnuAddPlotSerieId);
            mnuStationList.Items["mnuButControlMode"].Text = Localization.getResStr(Configuration.modeControlModeId);
            mnuStationList.Items["mnuButMonitorMode"].Text = Localization.getResStr(Configuration.modeMonitorModeId);
            mnuStationList.Items["mnuButReopenStation"].Text = Localization.getResStr(Configuration.mnuReopenStationId);
            // tree menues
            mnuTreeStationList.Items["mnuButViewForm"].Text = Localization.getResStr(Configuration.mnuViewParametersId);
            mnuTreeStationList.Items["mnuButPrintCurrentSettings"].Text = Localization.getResStr(Configuration.mnuPrintCurrentSettingsId);
            mnuTreeStationList.Items["mnuButUpdate"].Text = Localization.getResStr(Configuration.mnuUpdateSoftwareId);
            mnuTreeStationList.Items["mnuButAddSerie"].Text = Localization.getResStr(Configuration.mnuAddPlotSerieId);
            mnuTreeStationList.Items["mnuButControlMode"].Text = Localization.getResStr(Configuration.modeControlModeId);
            mnuTreeStationList.Items["mnuButMonitorMode"].Text = Localization.getResStr(Configuration.modeMonitorModeId);
            mnuTreeStationList.Items["mnuButReopenStation"].Text = Localization.getResStr(Configuration.mnuReopenStationId);
            mnuTreeStationList.Items["mnuNewStationGroup"].Text = Localization.getResStr(Configuration.mnuNewStationGroupId);
            mnuTreeStationList.Items["mnuDeleteTreeItem"].Text = Localization.getResStr(Configuration.mnuDeleteTreeItemId);
            mnuTreeStationList.Items["mnuChangeTreeItem"].Text = Localization.getResStr(Configuration.mnuChangeTreeItemId);
            // dock panel texts
            frmStationListDockPanel.loadTextsDockPanel();
            if (!Equals(treevwStationList, null))
            {
                if (treevwStationList.TopNode != null)
                {
                    treevwStationList.TopNode.Text = Localization.getResStr(Configuration.dockStationListId);
                }
            }
        }

        #endregion

        #region Station List Panel

        public void timerStationListData_Tick(System.Object sender, System.EventArgs e)
        {
            // timer to update station list data

            ((Timer)sender).Stop();
            // activated when a new station is connected (addStation function)
            bool bAllWithData = true;
            for (var i = 0; i <= stationList.Count - 1; i++)
            {
                tStation stn = stationList[System.Convert.ToInt32(i)];
                if (stn.bItemDataShowed == false)
                {
                    Configuration.strucStationData stnData = new Configuration.strucStationData();
                    stnData.ID = stn.ID;
                    stnData.eStationType = m_jbcConnect.GetStationType(stn.ID);
                    stn.StationType = m_jbcConnect.GetStationType(stn.ID);
                    stnData.sModel = m_jbcConnect.GetStationModel(stn.ID);
                    stnData.sName = "";
                    stnData.sSW = m_jbcConnect.GetStationSWversion(stn.ID);
                    stnData.sStationCOM = m_jbcConnect.GetStationHostName(stn.ID).ToLower(); // jbc.GetStationCOM(stn.ID)
                    stnData.bStationNamed = true;
                    if (!string.IsNullOrEmpty(stnData.sModel) && !string.IsNullOrEmpty(stnData.sStationCOM))
                    {
                        //If stnData.sModel <> "" And stnData.sSW <> "" Then
                        stnData.sName = System.Convert.ToString(buildStationNameInList(m_jbcConnect.GetStationName(stn.ID)));
                        if (m_jbcConnect.GetStationName(stn.ID).Trim() == "")
                        {
                            stnData.bStationNamed = false;
                        }
                        stn.item.SubItems[Configuration.subItemModelId].Text = stnData.sModel + " [" + stnData.sStationCOM + "]";
                        stn.item.SubItems[Configuration.subItemSWId].Text = stnData.sSW;
                        stn.item.SubItems[Configuration.subItemTypeId].Text = Convert.ToString(stnData.eStationType);
                        // define item.text after subitems texts
                        stn.item.Text = stnData.sName;
                        stn.bItemDataShowed = true;
                        stn.bStationNamed = stnData.bStationNamed;
                        stationList[System.Convert.ToInt32(i)] = stn;
                        // show in tree
                        Configuration.grouplistConnectedStationNode(treevwStationList, stnData);
                    }
                    else
                    {
                        bAllWithData = false;
                    }
                }
            }
            if (!bAllWithData)
            {
                // continue with timer
                ((Timer)sender).Start();
            }
            else
            {
                m_lsvwStationList.Enabled = false;
                m_lsvwStationList.Sort();
                m_lsvwStationList.Enabled = true;
            }

        }

        private dynamic buildStationNameInList(string sStationName)
        {
            if (sStationName.Trim() == "")
            {
                return Localization.getResStr(Configuration.gralNoNameId);
            }
            else
            {
                return sStationName;
            }
        }

        private void SetStationNameListView(long stationID, string sStationNewName)
        {
            int stationIdx = getStationIndex(stationID);
            if (stationIdx > -1)
            {
                tStation station = stationList[stationIdx];
                station.item.Text = System.Convert.ToString(buildStationNameInList(sStationNewName));
            }
        }

        private void createStationDockPanel()
        {
            //Creating the dock panel
            frmStationListDockPanel = new frmDockPanel();
            frmStationListDockPanel.FormClosed += frmStationListDockPanel_FormClosed;
            frmStationListDockPanel.MouseEnter += frmStationListDockPanel.DockPanel_MouseEnter;
            frmStationListDockPanel.MouseLeave += frmStationListDockPanel.DockPanel_MouseLeave;

            // load station list configuration previously saved
            System.Xml.XmlDocument xmlDoc = loadStationListConfig();

            //Setting this form as its MDI parent
            frmStationListDockPanel.MdiParent = this;
            //dplStationList.TopMost = True ' cannot be done in MDI
            //configuring
            frmStationListDockPanel.Text = "";
            frmStationListDockPanel.Dock = DockStyle.Right;

            //the list view control
            m_lsvwStationList = frmStationListDockPanel.lsvwStationList;
            m_lsvwStationList.MouseDoubleClick += lsvwStationList_MouseDoubleClick;
            m_lsvwStationList.MouseClick += lsvwStationList_MouseClick;
            m_lsvwStationList.ItemDrag += DockPanel_ItemDrag;
            //the tree view control
            treevwStationList = frmStationListDockPanel.treevwStationList;
            treevwStationList.MouseDoubleClick += treevwStationList_MouseDoubleClick;
            treevwStationList.BeforeLabelEdit += treevwStationList_BeforeLabelEdit;
            treevwStationList.AfterLabelEdit += treevwStationList_AfterLabelEdit;
            treevwStationList.AfterSelect += treevwStationList_AfterSelect;
            treevwStationList.MouseClick += treevwStationList_MouseClick;
            treevwStationList.ItemDrag += treevwStationList_ItemDrag;
            treevwStationList.DragOver += treevwStationList_DragOver;
            treevwStationList.DragDrop += treevwStationList_DragDrop;

            //adding the station items
            // load xml station tree list (group nodes, and stations by name and model)
            Configuration.grouplistXmlToTree(xmlDoc, treevwStationList);

            // add connected stations (in list and tree)
            foreach (tStation stn in stationList)
            {
                //lsvwStationList.Items.Add(s.item)
                addItem(stn);
            }
            // #edu# activate timer to see if data is available to fill the itemlist for the stations
            timerStationListData.Start();

            // load last saved dock panel configuration
            loadDockPanelConfigFromXml(xmlDoc);

            //creating the context menues for the list and tree views
            mnuStationList = new ToolStripDropDownMenu();
            mnuStationList.ItemClicked += mnuStationList_ItemClicked;
            mnuTreeStationList = new ToolStripDropDownMenu();
            mnuTreeStationList.ItemClicked += mnuTreeStationList_ItemClicked;
            mnuStationList.Name = "mnuStationList";
            mnuTreeStationList.Name = "mnuTreeStationList";

            //creating and adding the menu buttons
            ToolStripItem[] mnuButtons = new ToolStripItem[8];
            mnuButtons[0] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuViewParametersId), null, null, "mnuButViewForm");
            mnuButtons[1] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuPrintCurrentSettingsId), null, null, "mnuButPrintCurrentSettings");
            mnuButtons[2] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuUpdateSoftwareId), null, null, "mnuButUpdate");
            mnuButtons[2].Visible = false;
            mnuButtons[3] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuAddPlotSerieId), null, null, "mnuButAddSerie");
            mnuButtons[4] = new ToolStripSeparator();
            mnuButtons[5] = new ToolStripMenuItem(Localization.getResStr(Configuration.modeControlModeId), null, null, "mnuButControlMode");
            mnuButtons[6] = new ToolStripMenuItem(Localization.getResStr(Configuration.modeMonitorModeId), null, null, "mnuButMonitorMode");
            mnuButtons[7] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuReopenStationId), null, null, "mnuButReopenStation");
            mnuStationList.Items.AddRange(mnuButtons);

            ToolStripItem[] mnuButtonstree = new ToolStripItem[12];
            mnuButtonstree[0] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuViewParametersId), null, null, "mnuButViewForm");
            mnuButtonstree[1] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuPrintCurrentSettingsId), null, null, "mnuButPrintCurrentSettings");
            mnuButtonstree[2] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuUpdateSoftwareId), null, null, "mnuButUpdate");
            mnuButtonstree[2].Visible = false;
            mnuButtonstree[3] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuAddPlotSerieId), null, null, "mnuButAddSerie");
            mnuButtonstree[4] = new ToolStripSeparator();
            mnuButtonstree[5] = new ToolStripMenuItem(Localization.getResStr(Configuration.modeControlModeId), null, null, "mnuButControlMode");
            mnuButtonstree[6] = new ToolStripMenuItem(Localization.getResStr(Configuration.modeMonitorModeId), null, null, "mnuButMonitorMode");
            mnuButtonstree[7] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuReopenStationId), null, null, "mnuButReopenStation");
            mnuButtonstree[8] = new ToolStripSeparator();
            mnuButtonstree[9] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuNewStationGroupId), null, null, "mnuNewStationGroup"); // new group
            mnuButtonstree[10] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuDeleteTreeItemId), null, null, "mnuDeleteTreeItem"); // delete group or not connected station
            mnuButtonstree[11] = new ToolStripMenuItem(Localization.getResStr(Configuration.mnuChangeTreeItemId), null, null, "mnuChangeTreeItem"); // change group name
            mnuTreeStationList.Items.AddRange(mnuButtonstree);
        }

        private System.Xml.XmlDocument loadStationListConfig()
        {
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            // 22/04/2014
            string sFilenameOLD = APPDATA_DIRECTORY_OLD + My.Settings.Default.xmlStationGroupsFileName;
            string sFilename = APPDATA_DIRECTORY + My.Settings.Default.xmlStationGroupsFileName;
            // copy file from OLD location (may be cannot move-delete with user permissions)
            if (System.IO.File.Exists(sFilenameOLD) && (!System.IO.File.Exists(sFilename)))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(sFilenameOLD, sFilename);
            }
            if (System.IO.File.Exists(sFilename))
            {
                string sError = "";
                xmlDoc = RoutinesLibrary.Data.Xml.XMLUtils.LoadFromFile(sFilename, ref sError);
            }
            else
            {
                xmlDoc = Configuration.stationlistNewXmlDoc();
            }
            return xmlDoc;
        }

        private void loadDockPanelConfigFromXml(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode xmlNodeRoot = Configuration.xmlGetStationListNodeFromXml(xmlDoc);
            System.Xml.XmlNode xmlNode = default(System.Xml.XmlNode);
            string sPosition = "";
            string sLocation = "";
            string sSize = "";
            bool bChecked = true;

            // position
            xmlNode = xmlNodeRoot.SelectSingleNode(Configuration.xmlDockPanelPositionId);
            if (xmlNode != null)
            {
                frmStationListDockPanel.Dock = (DockStyle)System.Enum.Parse(typeof(System.Windows.Forms.DockStyle), xmlNode.InnerText);
                sPosition = xmlNode.InnerText;
            }

            object[] arr = null;

            // location
            System.Drawing.Point oLocation = new System.Drawing.Point(0, 0);
            xmlNode = xmlNodeRoot.SelectSingleNode(Configuration.xmlDockPanelLocationId);
            if (xmlNode != null)
            {
                sLocation = xmlNode.InnerText;
                arr = xmlNode.InnerText.Split(',');
                oLocation.X = System.Convert.ToInt32(arr[0]);
                oLocation.Y = System.Convert.ToInt32(arr[1]);
            }

            // size
            System.Drawing.Size oSize = new System.Drawing.Size(0, 0);
            xmlNode = xmlNodeRoot.SelectSingleNode(Configuration.xmlDockPanelSizeId);
            if (xmlNode != null)
            {
                sSize = xmlNode.InnerText;
                arr = xmlNode.InnerText.Split(',');
                oSize.Width = System.Convert.ToInt32(arr[0]);
                oSize.Height = System.Convert.ToInt32(arr[1]);
            }

            if (sPosition == System.Windows.Forms.DockStyle.None.ToString())
            {
                // floating, set size and location
                if (!string.IsNullOrEmpty(sSize))
                {
                    frmStationListDockPanel.Size = oSize;
                }
                if (!string.IsNullOrEmpty(sLocation))
                {
                    frmStationListDockPanel.Left = oLocation.X;
                    frmStationListDockPanel.Top = oLocation.Y;
                }
            }
            else if ((sPosition == System.Windows.Forms.DockStyle.Top.ToString()) || (sPosition == System.Windows.Forms.DockStyle.Bottom.ToString()))
            {
                // set height
                if (!string.IsNullOrEmpty(sSize))
                {
                    frmStationListDockPanel.Height = oSize.Height;
                }
            }
            else if ((sPosition == System.Windows.Forms.DockStyle.Left.ToString()) || (sPosition == System.Windows.Forms.DockStyle.Right.ToString()))
            {
                // set width
                if (!string.IsNullOrEmpty(sSize))
                {
                    frmStationListDockPanel.Width = oSize.Width;
                }
            }

            //list view type
            xmlNode = xmlNodeRoot.SelectSingleNode(Configuration.xmlDockPanelListViewId);
            if (xmlNode != null)
            {
                if ((View)System.Enum.Parse(typeof(View), xmlNode.InnerText) == View.Tile)
                {
                    frmStationListDockPanel.myCheckViewType(frmStationListDockPanel.mnuTile, true);
                }
                else
                {
                    frmStationListDockPanel.myCheckViewType(frmStationListDockPanel.mnuDetails, true);
                }
            }

            //list
            xmlNode = xmlNodeRoot.SelectSingleNode(Configuration.xmlDockPanelListId);
            if (xmlNode != null)
            {
                bChecked = xmlNode.InnerText == "True";
                frmStationListDockPanel.myCheckViewType(frmStationListDockPanel.mnuList, bChecked);
            }

            //tree
            xmlNode = xmlNodeRoot.SelectSingleNode(Configuration.xmlDockPanelTreeId);
            if (xmlNode != null)
            {
                bChecked = xmlNode.InnerText == "True";
                frmStationListDockPanel.myCheckViewType(frmStationListDockPanel.mnuTreeList, bChecked);
            }

            // tree size
            oSize = new System.Drawing.Size(0, 0);
            sSize = "";
            xmlNode = xmlNodeRoot.SelectSingleNode(Configuration.xmlDockPanelTreeSizeId);
            if (xmlNode != null)
            {
                sSize = xmlNode.InnerText;
                arr = xmlNode.InnerText.Split(',');
                oSize.Width = System.Convert.ToInt32(arr[0]);
                oSize.Height = System.Convert.ToInt32(arr[1]);
            }
            // set tree height
            if (!string.IsNullOrEmpty(sSize))
            {
                treevwStationList.Height = oSize.Height;
            }

        }

        private void saveStationListConfig()
        {

            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            // 22/04/2014
            var sFilename = APPDATA_DIRECTORY + My.Settings.Default.xmlStationGroupsFileName;
            System.Xml.XmlNode xmlNodeRoot = default(System.Xml.XmlNode);

            xmlDoc = Configuration.stationlistNewXmlDoc();

            // dock panel config
            xmlNodeRoot = Configuration.xmlGetStationListNodeFromXml(xmlDoc);
            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, Configuration.xmlDockPanelPositionId, frmStationListDockPanel.Dock.ToString());
            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, Configuration.xmlDockPanelLocationId, frmStationListDockPanel.Left.ToString() + "," + frmStationListDockPanel.Top.ToString());
            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, Configuration.xmlDockPanelSizeId, frmStationListDockPanel.Size.Width.ToString() + "," + frmStationListDockPanel.Size.Height.ToString());
            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, Configuration.xmlDockPanelListId, m_lsvwStationList.Visible.ToString());
            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, Configuration.xmlDockPanelListViewId, m_lsvwStationList.View.ToString());
            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, Configuration.xmlDockPanelTreeId, treevwStationList.Visible.ToString());
            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, Configuration.xmlDockPanelTreeSizeId, treevwStationList.Size.Width.ToString() + "," + treevwStationList.Size.Height.ToString());

            // group list
            Configuration.grouplistTreeToXml(treevwStationList, xmlDoc);
            xmlDoc.Save(sFilename);
        }

        private void addItem(tStation stn)
        {
            ListViewItem item = stn.item;

            //if the list is created
            if (!Equals(m_lsvwStationList, null))
            {
                //adding the item
                m_lsvwStationList.Enabled = false;
                m_lsvwStationList.Items.Add(item);
                m_lsvwStationList.Enabled = true;
            }

            //node is adde to treeview in timerApplStartUpAndStationListData.Start() timer
            //called after additem
            //to see if data is available to fill the itemlist for this station
            //If Not Equals(treevwStationList, Nothing) Then
            //    'adding the item
            //    Dim sModel As String = Trim(item.SubItems(subItemModelId).Text)
            //    Dim sName As String = Trim(item.Text)
            //    'Dim sSW As String = item.SubItems(subItemSWId).Text
            //    Dim ID As ULong = stn.ID
            //    Dim bStationNamed As Boolean = stn.bStationNamed
            //    grouplistConnectedStationNode(treevwStationList, ID, sName, sModel, bStationNamed)
            //End If

        }

        private void rmvItem(tStation stn)
        {
            ListViewItem item = stn.item;

            //if the tree is created
            if (!Equals(m_lsvwStationList, null))
            {
                //looking for the node
                m_lsvwStationList.Items.Remove(item);
            }

            //if the treeview is created
            if (!Equals(treevwStationList, null))
            {
                //removing the item
                long ID = stn.ID;
                Configuration.grouplistDisconnectedStationNode(treevwStationList, ID);
            }
        }

        private void myNewGroupSelectedTree()
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            // if there's an item selected
            if (tnode != null)
            {
                string sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                TreeNode tnodeGroup;
                if (sType == Configuration.xmlStationId)
                {
                    // add a sibling group
                    if (ReferenceEquals(tnode.Parent, null))
                    {
                        tnodeGroup = Configuration.grouplistAddGroupNode(tnode.TreeView, Localization.getResStr(Configuration.gralNewGroupId));
                    }
                    else
                    {
                        tnodeGroup = Configuration.grouplistAddGroupNode(tnode.Parent, Localization.getResStr(Configuration.gralNewGroupId));
                    }
                }
                else if ((sType == Configuration.xmlGroupId) || (sType == Configuration.xmlGroupListId))
                {
                    // add a subgroup
                    tnodeGroup = Configuration.grouplistAddGroupNode(tnode, Localization.getResStr(Configuration.gralNewGroupId));
                }
            }
        }

        private void myDeleteTreeItemSelectedTree()
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            if (tnode != null)
            {
                string sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                if (sType == Configuration.xmlStationId)
                {
                    // delete station -> if disconnected, delete, if connected, send to root
                    if (tnode.ImageKey == Configuration.iconStationDisconnId)
                    {
                        RoutinesLibrary.UI.TreeViewUtils.DeleteNode(ref tnode);
                    }
                    else
                    {
                        TreeView tview = tnode.TreeView;
                        tnode.Remove();
                        tview.TopNode.Nodes.Insert(0, tnode);
                    }
                }
                else if (sType == Configuration.xmlGroupId)
                {
                    // delete group, if empty
                    if (tnode.Nodes.Count == 0)
                    {
                        RoutinesLibrary.UI.TreeViewUtils.DeleteNode(ref tnode);
                    }
                }
            }
        }

        private void myEditTreeItemSelectedTree()
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            if (tnode != null)
            {
                string sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                if (sType == Configuration.xmlGroupId)
                {
                    tnode.BeginEdit();
                }
            }
        }

        private void myShowSelectedStationForm()
        {
            if (m_lsvwStationList.SelectedItems.Count > 0)
            {
                long stationID = ((Configuration.tStationDataItemList)(m_lsvwStationList.SelectedItems[0].Tag)).ID;
                myShowStationForm(stationID);
            }
        }

        private void myShowSelectedTreeStationForm()
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            // if there's an item selected
            if (tnode != null)
            {
                string sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                if (sType == Configuration.xmlStationId)
                {
                    long stationID = System.Convert.ToInt64(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, Configuration.xmlConnectedStationIDId));
                    if (stationID >= 0)
                    {
                        myShowStationForm(stationID);
                    }
                }
            }
        }

        private void myShowStationForm(long stationID)
        {
            int stationIdx = getStationIndex(stationID);
            if (stationIdx > -1)
            {
                tStation station = stationList[stationIdx];
                //if the form is disposed creating it and showing it, otherwise selecting it
                switch (station.StationType)
                {
                    case eStationType.SOLD:
                        if (ReferenceEquals(station.frm, null))
                        {
                            station.frm = new frmStation(m_jbcConnect, reg, station.ID, station.item);
                            station.frm.MdiParent = this;
                            station.frm.PlotStation += onPlotStation;
                            station.frm.StationNameChanged += onStationNameChanged;
                            station.frm.Show();
                            stationList[stationIdx] = station;
                        }
                        else if (station.frm.IsDisposed)
                        {
                            station.frm = new frmStation(m_jbcConnect, reg, station.ID, station.item);
                            station.frm.MdiParent = this;
                            station.frm.Show();
                            stationList[stationIdx] = station;
                        }
                        else
                        {
                            station.frm.Show();
                            station.frm.WindowState = FormWindowState.Normal;
                            station.frm.Focus();
                        }
                        break;

                    case eStationType.HA:
                        if (ReferenceEquals(station.frmHA, null))
                        {
                            station.frmHA = new frmStation_HA(m_jbcConnect, reg, station.ID, station.item);
                            station.frmHA.MdiParent = this;
                            station.frmHA.PlotStation += onPlotStation;
                            station.frmHA.StationNameChanged += onStationNameChanged;
                            station.frmHA.Show();
                            stationList[stationIdx] = station;
                        }
                        else if (station.frmHA.IsDisposed)
                        {
                            station.frmHA = new frmStation_HA(m_jbcConnect, reg, station.ID, station.item);
                            station.frmHA.MdiParent = this;
                            station.frmHA.Show();
                            stationList[stationIdx] = station;
                        }
                        else
                        {
                            station.frmHA.Show();
                            station.frmHA.WindowState = FormWindowState.Normal;
                            station.frmHA.Focus();
                        }
                        break;

                }

            }

        }

        private void myPrintSelectedStationSettings()
        {
            long stationID = ((Configuration.tStationDataItemList)(m_lsvwStationList.SelectedItems[0].Tag)).ID;
            myPrintStationSettings(stationID);
        }

        private void myPrintSelectedTreeStationSettings()
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            if (tnode != null)
            {
                long stationID = System.Convert.ToInt64(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, Configuration.xmlConnectedStationIDId));
                if (stationID >= 0)
                {
                    myPrintStationSettings(stationID);
                }
            }
        }

        private void myPrintStationSettings(long stationID)
        {
            int stationIdx = getStationIndex(stationID);
            if (stationIdx > -1)
            {
                confPrintStationSettings(stationID);
            }
        }

        private void myControlModeSelectedStations(bool bControlMode)
        {
            long stationID = ((Configuration.tStationDataItemList)(m_lsvwStationList.SelectedItems[0].Tag)).ID;
            myControlModeStations(stationID, bControlMode);
        }

        private void myControlModeSelectedTreeStations(bool bControlMode)
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            if (tnode != null)
            {
                long stationID = System.Convert.ToInt64(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, Configuration.xmlConnectedStationIDId));
                if (stationID >= 0)
                {
                    myControlModeStations(stationID, bControlMode);
                }
            }
        }

        private void myControlModeStations(long stationID, bool bControlMode)
        {
            int stationIdx = getStationIndex(stationID);
            if (stationIdx > -1)
            {
                tStation station = stationList[stationIdx];
                System.Windows.Forms.ListViewItem temp_stationItem = m_lsvwStationList.SelectedItems[0];
                Configuration.setStationControlMode(m_jbcConnect, station.ID, ref temp_stationItem, bControlMode);
            }

        }

        private void myResetSelectedStation()
        {
            long stationID = ((Configuration.tStationDataItemList)(m_lsvwStationList.SelectedItems[0].Tag)).ID;
            if (stationID >= 0)
            {
                m_jbcConnect.ResetStation(stationID);
            }

        }

        private void myResetSelectedTreeStation()
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            if (tnode != null)
            {
                long stationID = System.Convert.ToInt64(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, Configuration.xmlConnectedStationIDId));
                if (stationID >= 0)
                {
                    m_jbcConnect.ResetStation(stationID);
                }
            }

        }

        //Add station to the list
        private tStation addStation(long stationID)
        {

            StationListSearchingStation(true);

            //Creating the station structure
            tStation dataStation = new tStation();
            Configuration.tStationDataItemList dataList = new Configuration.tStationDataItemList();

            //Setting the station ID
            dataStation.ID = stationID;
            dataStation.bItemDataShowed = false; // do not have texts yet
            dataStation.stationCOM = m_jbcConnect.GetStationHostName(stationID).ToLower(); // jbc.GetStationCOM(stationID)
            dataStation.StationType = m_jbcConnect.GetStationType(stationID);
            dataStation.bStationNamed = true; // default

            // List View
            //Creating the station item and subitems
            //Dim itemFont As New Font("Microsoft Sans Serif", 8.25, FontStyle.Italic)
            dataStation.item = new ListViewItem();
            dataStation.item.Name = "lsvwItemStation" + stationID.ToString();
            ListViewItem.ListViewSubItem[] subItems = new ListViewItem.ListViewSubItem[3];
            subItems[0] = new ListViewItem.ListViewSubItem();
            subItems[1] = new ListViewItem.ListViewSubItem();
            subItems[2] = new ListViewItem.ListViewSubItem();
            subItems[0].Name = Configuration.subItemModelId;
            subItems[1].Name = Configuration.subItemSWId;
            subItems[2].Name = Configuration.subItemTypeId;
            dataStation.item.SubItems.AddRange(subItems);
            // general icon
            //data.item.ImageKey = "Station"
            if (bStationsInitialControlMode)
            {
                dataStation.item.ImageKey = "Station_unlock";
            }
            else
            {
                dataStation.item.ImageKey = "Station_lock";
            }

            //data for station list in dock panel #edu#
            dataList.ID = stationID;
            dataList.bControlMode = bStationsInitialControlMode; // initial monitor mode
            dataStation.item.Tag = dataList;
            // #edu# activate timer to see if data is available to fill the itemlist for this station
            // se pasa a addItem
            //timerApplStartUpAndStationListData.Start()

            //creating the station form
            //#edu# do not creates form at startup
            //data.frm = New frmStation(jbc, data.ID, data.item, bSupervisorLoggedIn)
            //data.frm.MdiParent = Me
            dataStation.frm = null;
            dataStation.frmHA = null;

            //adding the structure to the list
            stationList.Add(dataStation);

            //returning the new added station object
            return dataStation;
        }

        //Rmv station from the list
        private void rmvStation(long stationID)
        {
            //getting the station index in the list
            int index = getStationIndex(stationID);
            if (index >= 0)
            {
                stationList[index].item.Remove();
                if (stationList[index].frm != null)
                {
                    if (!stationList[index].frm.IsDisposed)
                    {
                        stationList[index].frm.tmr.Stop();
                        stationList[index].frm.Dispose();
                        //stationList(index).frm = Nothing
                    }
                }
                if (stationList[index].frmHA != null)
                {
                    if (!stationList[index].frmHA.IsDisposed)
                    {
                        stationList[index].frmHA.tmr.Stop();
                        stationList[index].frmHA.Dispose();
                        //stationList(index).frmHA = Nothing
                    }
                }
                stationList.RemoveAt(index);
            }

            StationListSearchingStation(false);

        }

        private void StationListSearchingStation(bool addedStation)
        {

            if (addedStation) //Added station
            {
                if (m_stationListSearchingStation)
                {
                    if (!Equals(m_lsvwStationList, null))
                    {
                        m_lsvwStationList.Items.Clear();
                    }
                    m_stationListSearchingStation = false;
                }

            }
            else //Removed station
            {
                if (stationList.Count == 0 && !m_stationListSearchingStation)
                {

                    //added searching settings
                    tStation dataStation = new tStation();
                    dataStation.bItemDataShowed = false;
                    dataStation.bStationNamed = true;
                    dataStation.frm = null;
                    dataStation.frmHA = null;
                    dataStation.item = new ListViewItem();
                    dataStation.item.Text = "Searching stations...";

                    addItem(dataStation);
                    m_stationListSearchingStation = true;
                }
            }
        }

        public int getStationIndex(long stationID)
        {
            //looking for the station in the list
            bool found = false;
            int cnt = 0;
            while (cnt < stationList.Count && !found)
            {
                if (stationList[cnt].ID == stationID)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            //returning the search result
            if (found)
            {
                return cnt;
            }
            else
            {
                return -1;
            }
        }

        public tStation getStationListElement(int idx)
        {
            tStation stationElem = default(tStation);

            if (idx < stationList.Count & idx >= 0)
            {
                stationElem = stationList[idx];
            }
            else
            {
                stationElem = new tStation();
            }

            return stationElem;
        }

        #endregion

        #region Station List Panel Events

        // close panel
        private void frmStationListDockPanel_FormClosed(System.Object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            saveStationListConfig();
        }

        // double click
        private void lsvwStationList_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // if left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                myShowSelectedStationForm();
            }
        }

        private void treevwStationList_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // if left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                myShowSelectedTreeStationForm();
            }
        }

        // tree label edit
        private void treevwStationList_BeforeLabelEdit(System.Object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
        {

            if (RoutinesLibrary.UI.TreeViewUtils.GetAttrib(e.Node, ConfigurationXML.xmlTypeId) != Configuration.xmlGroupId)
            {
                e.CancelEdit = true;
            }
        }

        private void treevwStationList_AfterLabelEdit(System.Object sender, System.Windows.Forms.NodeLabelEditEventArgs e)
        {

            // nothing changed
            if (ReferenceEquals(e.Label, null))
            {
                return;
            }
            Configuration.grouplistSetGroupNode(e.Node, e.Label);
        }

        TreeNode tnodeSelected;
        private void treevwStationList_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            tnodeSelected = e.Node;
        }

        // mouse click
        private void lsvwStationList_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // if right click
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // getting the selected item
                System.Windows.Forms.ListView.SelectedListViewItemCollection items = m_lsvwStationList.SelectedItems;

                // if there's an item selected then showing the menu
                if (items.Count >= 1)
                {
                    mnuStationList.Show(m_lsvwStationList.PointToScreen(items[0].Position + items[0].Bounds.Size));
                }
            }
        }

        private void treevwStationList_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // if right click
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                // getting the selected item
                TreeNode tnode = treevwStationList.SelectedNode;
                // if there's an item selected then showing the menu
                if (tnode != null)
                {
                    // node type
                    string sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                    // enable/disable options
                    if (sType == Configuration.xmlGroupListId)
                    {
                        foreach (ToolStripItem item in mnuTreeStationList.Items)
                        {
                            item.Enabled = false;
                        }
                        mnuTreeStationList.Items["mnuNewStationGroup"].Enabled = true;
                    }
                    else if (sType == Configuration.xmlGroupId)
                    {
                        foreach (ToolStripItem item in mnuTreeStationList.Items)
                        {
                            item.Enabled = false;
                        }
                        mnuTreeStationList.Items["mnuNewStationGroup"].Enabled = true;
                        mnuTreeStationList.Items["mnuDeleteTreeItem"].Enabled = true;
                        mnuTreeStationList.Items["mnuChangeTreeItem"].Enabled = true;
                    }
                    else if (sType == Configuration.xmlStationId)
                    {
                        foreach (ToolStripItem item in mnuTreeStationList.Items)
                        {
                            item.Enabled = RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, Configuration.xmlConnectedStationIDId) >= 0;
                        }
                        mnuTreeStationList.Items["mnuNewStationGroup"].Enabled = false;
                        mnuTreeStationList.Items["mnuDeleteTreeItem"].Enabled = !(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, Configuration.xmlConnectedStationIDId) >= 0);
                        mnuTreeStationList.Items["mnuChangeTreeItem"].Enabled = false;
                    }

                    mnuTreeStationList.Show(treevwStationList.PointToScreen(tnode.Bounds.Location + tnode.Bounds.Size));
                }
            }
        }

        // dropdown menues events
        private void mnuStationList_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {

            //depending on the clicked item
            switch (e.ClickedItem.Name)
            {
                case "mnuButViewForm":
                    myShowSelectedStationForm();
                    break;
                case "mnuButPrintCurrentSettings":
                    myPrintSelectedStationSettings();
                    break;
                case "mnuButUpdate":
                    MessageBox.Show(Localization.getResStr(Configuration.mnuUpdateSoftwareId) + ": " + Localization.getResStr(Configuration.gralUnderConstructionId));
                    break;
                case "mnuButAddSerie":
                    myPlotSelectedStation();
                    break;
                case "mnuButControlMode":
                    myControlModeSelectedStations(true);
                    break;
                case "mnuButMonitorMode":
                    myControlModeSelectedStations(false);
                    break;
                case "mnuButReopenStation":
                    myResetSelectedStation();
                    break;
            }
        }

        private void mnuTreeStationList_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {

            //depending on the clicked item
            switch (e.ClickedItem.Name)
            {
                case "mnuButViewForm":
                    myShowSelectedTreeStationForm();
                    break;
                case "mnuButPrintCurrentSettings":
                    myPrintSelectedTreeStationSettings();
                    break;
                case "mnuButUpdate":
                    MessageBox.Show(Localization.getResStr(Configuration.mnuUpdateSoftwareId) + ": " + Localization.getResStr(Configuration.gralUnderConstructionId));
                    break;
                case "mnuButAddSerie":
                    myPlotSelectedTreeStation();
                    break;
                case "mnuButControlMode":
                    myControlModeSelectedTreeStations(true);
                    break;
                case "mnuButMonitorMode":
                    myControlModeSelectedTreeStations(false);
                    break;
                case "mnuButReopenStation":
                    myResetSelectedTreeStation();
                    break;
                case "mnuNewStationGroup":
                    myNewGroupSelectedTree();
                    break;
                case "mnuDeleteTreeItem":
                    myDeleteTreeItemSelectedTree();
                    break;
                case "mnuChangeTreeItem":
                    myEditTreeItemSelectedTree();
                    break;
            }
        }

        // tree list drag/drop events
        private void treevwStationList_ItemDrag(System.Object sender, System.Windows.Forms.ItemDragEventArgs e)
        {

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                //Invocar a la operación de arrastrar y colocar.
                DoDragDrop(e.Item, (System.Windows.Forms.DragDropEffects)(DragDropEffects.Move | DragDropEffects.Copy));
            }
        }

        private void treevwStationList_DragOver(System.Object sender, System.Windows.Forms.DragEventArgs e)
        {

            Point ptCursor = treevwStationList.PointToClient(new Point(e.X, e.Y));
            TreeNode tnodeOver = treevwStationList.GetNodeAt(ptCursor);

            TreeNode tnodeDragging = null;
            e.Effect = DragDropEffects.None;
            if (tnodeOver != null)
            {
                if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode"))
                {
                    // dragging a node
                    tnodeDragging = (TreeNode)(e.Data.GetData("System.Windows.Forms.TreeNode"));
                    // if not the same node
                    if (tnodeDragging != tnodeOver)
                    {
                        // get node type
                        string sTypeDragging = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnodeDragging, ConfigurationXML.xmlTypeId));
                        string sTypeOver = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnodeOver, ConfigurationXML.xmlTypeId));
                        // get is station not named
                        string sStationNamed = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnodeDragging, Configuration.xmlStationNamedId));

                        // can drag any but not root
                        // cannot drag not named stations
                        if (sTypeDragging != Configuration.xmlGroupListId && sStationNamed != "False")
                        {
                            if (sTypeOver == Configuration.xmlStationId)
                            {
                                e.Effect = DragDropEffects.Move;
                            }
                            else
                            {
                                e.Effect = DragDropEffects.Move;
                            }
                        }
                    }
                }
            }
        }

        private void treevwStationList_DragDrop(System.Object sender, System.Windows.Forms.DragEventArgs e)
        {

            var ptCursor = treevwStationList.PointToClient(new Point(e.X, e.Y));
            TreeNode tnodeOver = treevwStationList.GetNodeAt(ptCursor);

            TreeNode nodeToExpand = null;
            TreeNode tnodeDragging = null;

            if (tnodeOver != null)
            {
                if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode"))
                {
                    // dragging a node
                    tnodeDragging = (TreeNode)(e.Data.GetData("System.Windows.Forms.TreeNode"));
                    // get node type
                    string sTypeDragging = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnodeDragging, ConfigurationXML.xmlTypeId));
                    string sTypeOver = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnodeOver, ConfigurationXML.xmlTypeId));

                    if (tnodeDragging != tnodeOver)
                    {
                        if (sTypeDragging != Configuration.xmlGroupListId)
                        {

                            // node to expand
                            if (sTypeOver == Configuration.xmlStationId)
                            {
                                nodeToExpand = tnodeOver.Parent;
                            }
                            else
                            {
                                nodeToExpand = tnodeOver;
                            }

                            if ((sTypeOver == Configuration.xmlGroupId) || (sTypeOver == Configuration.xmlGroupListId))
                            {
                                // move to the last of the children
                                tnodeOver.TreeView.Nodes.Remove(tnodeDragging);
                                // first, child
                                tnodeOver.Nodes.Insert(0, tnodeDragging);
                                // last, child
                                //tnodeOver.Nodes.Insert(tnodeOver.Nodes.IndexOf(tnodeOver.LastNode) + 1, tnodeDragging)
                            }
                            else if (sTypeOver == Configuration.xmlStationId)
                            {
                                // move after station
                                tnodeOver.TreeView.Nodes.Remove(tnodeDragging);
                                // before, sibling
                                tnodeOver.Parent.Nodes.Insert(tnodeOver.Parent.Nodes.IndexOf(tnodeOver), tnodeDragging);
                                // after, sibling
                                //tnodeOver.Parent.Nodes.Insert(tnodeOver.Parent.Nodes.IndexOf(tnodeOver) + 1, tnodeDragging)
                            }

                            // expand
                            if (nodeToExpand != null)
                            {
                                nodeToExpand.Expand();
                                RoutinesLibrary.UI.TreeViewUtils.ExpandParent(nodeToExpand, true);
                            }

                        }
                    }
                }
            }
        }


        #endregion

        #region Plotting
        private void myPlotSelectedStation()
        {
            long stationID = ((Configuration.tStationDataItemList)(m_lsvwStationList.SelectedItems[0].Tag)).ID;
            myPlotStation(stationID, -1);
        }

        private void myPlotSelectedTreeStation()
        {
            TreeNode tnode = treevwStationList.SelectedNode;
            if (tnode != null)
            {
                long stationID = System.Convert.ToInt64(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, Configuration.xmlConnectedStationIDId));
                if (stationID >= 0)
                {
                    myPlotStation(stationID, -1);
                }
            }
        }

        public void myPlotStation(long stationID, int regFrmID)
        {
            int stationIdx = getStationIndex(stationID);
            if (stationIdx > -1)
            {
                if (regFrmID < 0)
                {
                    // new window
                    regFrmID = reg.newFrm(this, System.Convert.ToBoolean(My.Settings.Default.optRegisterMDI), Configuration.Tunits);
                }
                frmMainRegister frm = (frmMainRegister)(reg.getFrm(regFrmID));
                if (frm != null)
                {
                    frm.Show();
                    frmSeries frmSeries = frm.myInitFormSeries();
                    Control oControl = null;

                    // only add for this station
                    if (Configuration.myControlExists(frmSeries, "cbxStation", ref oControl))
                    {
                        ((ComboBox)oControl).Enabled = false;
                    }
                    frmSeries.sSelectStationID = stationID.ToString();

                    // do not permit modifying
                    if (Configuration.myControlExists(frmSeries, "butEdit", ref oControl))
                    {
                        ((Button)oControl).Visible = false;
                    }

                    frm.frmSeriesReg.ShowDialog();
                }
            }

        }

        public void onPlotStation(long stationID, int regFrmID)
        {
            myPlotStation(stationID, regFrmID);
        }

        public void onStationNameChanged(long stationID, string sStationNewName)
        {
            SetStationNameListView(stationID, sStationNewName);
            Configuration.grouplistConnectedStationNameNode(treevwStationList, stationID, sStationNewName);
        }

        #endregion

        #region Drag Funcionality
        private void DockPanel_ItemDrag(System.Object sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            ListViewItem item = default(ListViewItem);
            item = (ListViewItem)e.Item;
            if (item != null)
            {
                //Dim stationID As ULong = Convert.ToUInt64(item.Name.Replace("lsvwItemStation", ""))
                long stationID = ((Configuration.tStationDataItemList)item.Tag).ID;
                DoDragDrop("ID=" + System.Convert.ToString(stationID), DragDropEffects.All);
            }
        }
        #endregion

        #region JBC API Events
        //Private LockTrans As New Object
        private void jbc_NewStationConnected(long stationID)
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new JBC_API_Remote.NewStationConnectedEventHandler(jbc_NewStationConnected), new object[] { stationID });
                return;
            }

            //new station connected, adding it
            tStation newStation = addStation(stationID);

            //adding the node to the list and treenode, if the station list panel is present ( the check is done internally)
            addItem(newStation);
            // #edu# activate timer to see if data is available to fill the itemlist for this station
            timerStationListData.Start();

            // obtain data from api
            string sStationData = "";
            var model = m_jbcConnect.GetStationModel(stationID);
            var name = m_jbcConnect.GetStationName(stationID);
            var stationHost = m_jbcConnect.GetStationHostName(stationID).ToLower();
            sStationData = Localization.getResStr(Configuration.confStationId) + " " + model + " [" + stationHost + "] - " + System.Convert.ToString(buildStationNameInList(name));

            //Add message to Event panel
            frmEvents.addEvent(frmEvents.eventType.TypeConnection, Localization.getResStr(Configuration.evStationConnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + sStationData);

            //Add message to notification
            if (Configuration.curShowStationNotifications)
            {
                frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
                notificationMsg.message = Localization.getResStr(Configuration.evStationConnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + sStationData;
                notificationMsg.type = frmNotifications.typeNotificationMessage.INFORMATION;
                m_frmNotifications.Add(notificationMsg);
            }

            //showing the station form, if created in addStation()
            if (newStation.frm != null)
            {
                newStation.frm.Show();
            }
            if (newStation.frmHA != null)
            {
                newStation.frmHA.Show();
            }

        }

        private void jbc_StationDisconnected(long stationID)
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new JBC_API_Remote.StationDisconnectedEventHandler(jbc_StationDisconnected), new object[] { stationID });
                return;
            }

            //getting the station index
            int index = getStationIndex(stationID);
            if (index >= 0)
            {
                //getting the station object
                tStation oldStation = stationList[index];

                // obtain data from station list
                string sStationData = "";
                var model = oldStation.item.SubItems[Configuration.subItemModelId].Text;
                var name = oldStation.item.Text;
                sStationData = Localization.getResStr(Configuration.confStationId) + " " + model + " - " + name;

                //Add message to Event panel
                frmEvents.addEvent(frmEvents.eventType.TypeConnection, Localization.getResStr(Configuration.evStationDisconnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + sStationData);

                //Add message to notification
                if (Configuration.curShowStationNotifications)
                {
                    frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
                    notificationMsg.message = Localization.getResStr(Configuration.evStationDisconnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + sStationData;
                    notificationMsg.type = frmNotifications.typeNotificationMessage.INFORMATION;
                    m_frmNotifications.Add(notificationMsg);
                }

                //hiding the station
                if (oldStation.frm != null)
                {
                    if (!oldStation.frm.IsDisposed)
                    {
                        oldStation.frm.tmr.Stop();
                        //oldStation.frm.Close()
                        oldStation.frm.Dispose();
                        oldStation.frm = null;
                    }
                }
                if (oldStation.frmHA != null)
                {
                    if (!oldStation.frmHA.IsDisposed)
                    {
                        oldStation.frmHA.tmr.Stop();
                        //oldStation.frmHA.Close()
                        oldStation.frmHA.Dispose();
                        oldStation.frmHA = null;
                    }
                }
                //removing the node from the list and treenode
                rmvItem(oldStation);

                //removing the station object
                rmvStation(stationID);
            }
        }

        private void jbc_HostDiscovered(EndpointAddress stationControllerEndPointAddress, string stationControllerName)
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new JBC_API_Remote.HostDiscoveredEventHandler(jbc_HostDiscovered), new object[] { stationControllerEndPointAddress, stationControllerName });
                return;
            }
            frmEvents.addEvent(frmEvents.eventType.TypeConnection, Localization.getResStr(Configuration.evStationControllerConnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + stationControllerName);

            //Add message to notification
            if (Configuration.curShowStationControllerNotifications)
            {
                frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
                notificationMsg.message = Localization.getResStr(Configuration.evStationControllerConnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + stationControllerName;
                notificationMsg.type = frmNotifications.typeNotificationMessage.INFORMATION;
                m_frmNotifications.Add(notificationMsg);
            }

        }

        private void jbc_HostDisconnected(EndpointAddress stationControllerEndPointAddress, string stationControllerName)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new JBC_API_Remote.HostDisconnectedEventHandler(jbc_HostDisconnected), new object[] { stationControllerEndPointAddress, stationControllerName });
                return;
            }
            frmEvents.addEvent(frmEvents.eventType.TypeConnection, Localization.getResStr(Configuration.evStationControllerDisconnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + stationControllerName);

            //Add message to notification
            if (Configuration.curShowStationControllerNotifications)
            {
                frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
                notificationMsg.message = Localization.getResStr(Configuration.evStationControllerDisconnectedId) + Localization.getResStr(Configuration.gralValueSeparatorId) + stationControllerName;
                notificationMsg.type = frmNotifications.typeNotificationMessage.INFORMATION;
                m_frmNotifications.Add(notificationMsg);
            }

        }

        private void jbc_UserError(long stationID, Cerror err)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new JBC_API_Remote.UserErrorEventHandler(jbc_UserError), new object[] { stationID, err });
                return;
            }

            foreach (tStation s in stationList)
            {
                if (s.ID == stationID)
                {
                    if (s.frm != null)
                    {
                        if (!s.frm.IsDisposed)
                        {
                            if (err.GetCommErrorCode() != Cerror.cCommErrorCodes.NO_COMM_ERROR)
                            {
                                // si es un error de comunicaciones estacion/PC, parar
                                s.frm.tmr.Stop();
                            }
                        }
                    }
                    if (s.frmHA != null)
                    {
                        if (!s.frmHA.IsDisposed)
                        {
                            if (err.GetCommErrorCode() != Cerror.cCommErrorCodes.NO_COMM_ERROR)
                            {
                                // si es un error de comunicaciones, parar
                                s.frmHA.tmr.Stop();
                            }
                        }
                    }
                    // #edu# show error only if it is found
                    // get station data from station list
                    string sStationData = "";
                    var model = s.item.SubItems[Configuration.subItemModelId].Text;
                    var name = s.item.Text;
                    sStationData = Localization.getResStr(Configuration.confStationId) + " " + model + " - " + name;
                    Cerror.cErrorCodes errorCode = err.GetCode();
                    Cerror.cCommErrorCodes commErrorCode = err.GetCommErrorCode();
                    string sErrorMsg = "";
                    string sCommErrorMsg = "";
                    string sErrorText = "";
                    if (errorCode == Cerror.cErrorCodes.STATION_ID_NOT_FOUND)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueSTATION_ID_NOT_FOUND);
                    }
                    else if (errorCode == Cerror.cErrorCodes.CONTINUOUS_MODE_ON_WITHOUT_PORTS)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueCONTINUOUS_MODE_ON_WITHOUT_PORTS);
                    }
                    else if (errorCode == Cerror.cErrorCodes.PORT_NOT_IN_RANGE)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.uePORT_NOT_IN_RANGE);
                    }
                    else if (errorCode == Cerror.cErrorCodes.INVALID_STATION_NAME)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueINVALID_STATION_NAME);
                    }
                    else if (errorCode == Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueTEMPERATURE_OUT_OF_RANGE);
                    }
                    else if (errorCode == Cerror.cErrorCodes.STATION_ID_OVERFLOW)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueSTATION_ID_OVERFLOW);
                    }
                    else if (errorCode == Cerror.cErrorCodes.POWER_LIMIT_OUT_OF_RANGE)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.uePOWER_LIMIT_OUT_OF_RANGE);
                    }
                    else if (errorCode == Cerror.cErrorCodes.TOOL_NOT_SUPPORTED)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueTOOL_NOT_SUPPORTED);
                    }
                    else if (errorCode == Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueFUNCTION_NOT_SUPPORTED);
                    }
                    else if (errorCode == Cerror.cErrorCodes.COMMUNICATION_ERROR)
                    {
                        sErrorMsg = Localization.getResStr(Configuration.ueCOMMUNICATION_ERROR);
                    }
                    if (commErrorCode == Cerror.cCommErrorCodes.NO_COMM_ERROR)
                    {
                        //sCommErrorMsg = getResStr(ceNO_COMM_ERROR)
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.BCC)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceBCC);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.FRAME_FORMAT)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceFRAME_FORMAT);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.OUT_OF_RANGE)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceOUT_OF_RANGE);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.COMMAND_REJECTED)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceCOMMAND_REJECTED);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.CONTROL_MODE_REQUIRED)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceCONTROL_MODE_REQUIRED);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.INCORRECT_SEQUENCY)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceINCORRECT_SEQUENCY);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.FLASH_WRITE_ERROR)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceFLASH_WRITE_ERROR);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.CONTROL_MODE_ALREADY_ACTIVATED)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceCONTROL_MODE_ALREADY_ACTIVATED);
                    }
                    else if (commErrorCode == Cerror.cCommErrorCodes.NOT_VALID_HARDWARE)
                    {
                        sCommErrorMsg = Localization.getResStr(Configuration.ceNOT_VALID_HARDWARE);
                    }
                    if (!string.IsNullOrEmpty(sErrorMsg))
                    {
                        if (!string.IsNullOrEmpty(sErrorText))
                        {
                            sErrorText = sErrorText + "\r\n";
                        }
                        sErrorText = sErrorText + Localization.getResStr(Configuration.ueErrorId) + Localization.getResStr(Configuration.gralValueSeparatorId) + sErrorMsg;
                    }
                    if (!string.IsNullOrEmpty(sCommErrorMsg))
                    {
                        if (!string.IsNullOrEmpty(sErrorText))
                        {
                            sErrorText = sErrorText + "\r\n";
                        }
                        sErrorText = sErrorText + Localization.getResStr(Configuration.ceErrorId) + Localization.getResStr(Configuration.gralValueSeparatorId) + sCommErrorMsg;
                    }
                    if (!string.IsNullOrEmpty(sErrorText))
                    {
                        string msgText = sStationData + " - " + sErrorText;

                        //Add message to frmEvents
                        frmEvents.addEvent(frmEvents.eventType.TypeError, msgText);

                        //Add message to Notifications
                        if (Configuration.curShowErrorNotifications)
                        {
                            frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
                            notificationMsg.message = msgText;
                            notificationMsg.type = frmNotifications.typeNotificationMessage.WARNING;
                            m_frmNotifications.Add(notificationMsg);
                        }

                        //Add message to eventLog
                        LoggerModule.logger.Warn(msgText);

                    }
                    break; // #edu 28/05/2014
                }
            }

            // #edu#
            //MsgBox("Error of station " & stationID & ". Code: " & err.GetCode() & " -> " & err.GetMsg())
        }

        #endregion

        #region Events Window

        private void createEventWindow()
        {
            //Creating the log window
            frmEvents = new frmEvents();

            //Setting this form as its MDI parent
            frmEvents.MdiParent = this;
            frmEvents.Dock = DockStyle.Left;

        }

        public void mnuEvents_Click(System.Object sender, System.EventArgs e)
        {

            if (ReferenceEquals(frmEvents, null))
            {
                createEventWindow();
            }
            if (frmEvents != null)
            {
                if (frmEvents.Visible)
                {
                    frmEvents.Close();
                }
                else
                {
                    frmEvents.Show();
                    //frmEvents.WindowState = FormWindowState.Normal
                }
            }
        }

        #endregion

        #region Settings Manager Related

        private void confShowConfMgr(string fileName)
        {

            Control ctrl = null;
            if (Configuration.frmConfMgr != null)
            {
                if (Configuration.frmConfMgr.IsDisposed)
                {
                    Configuration.frmConfMgr = new frmConfManager(stationList, m_jbcConnect, Configuration.bSupervisorLoggedIn);
                    Configuration.frmConfMgr.MdiParent = this;
                }
            }
            else
            {
                Configuration.frmConfMgr = new frmConfManager(stationList, m_jbcConnect, Configuration.bSupervisorLoggedIn);
                Configuration.frmConfMgr.MdiParent = this;
            }
            if (fileName != "")
            {
                Configuration.frmConfMgr.xmlLoadToTree(fileName);
            }
            Configuration.frmConfMgr.Show();

        }

        private void confPrintStationSettings(long stationID)
        {

            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            Form oForm;
            string sXmlPathFileName = "";

            xmlDoc = Configuration.confGetFromStation(stationID, m_jbcConnect, true);
            if (xmlDoc != null)
            {
                sXmlPathFileName = Configuration.confSaveConfXml(xmlDoc, Localization.getResStr(Configuration.xslStationConfigurationId));
                if (!string.IsNullOrEmpty(sXmlPathFileName))
                {
                    oForm = Configuration.showXML(sXmlPathFileName, true);
                }
            }
        }
        #endregion

        #region Main Menus
        public void butViewStationList_Click(System.Object sender, System.EventArgs e)
        {

            //showing/hiding the panel
            if (frmStationListDockPanel.IsDisposed)
            {
                createStationDockPanel();
                frmStationListDockPanel.Show();
            }
            else
            {
                //frmStationListDockPanel.Dispose()
                frmStationListDockPanel.Close();
            }
        }

        public void ViewToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            butViewStationList.Checked = !frmStationListDockPanel.IsDisposed;
            butViewEvents.Checked = frmEvents.Visible;
        }

        public void mnuSettingsManager_Click(System.Object sender, System.EventArgs e)
        {
            confShowConfMgr("");
        }

        public void mnuRegisterManager_Click(System.Object sender, System.EventArgs e)
        {
            int regID = reg.newFrm(this, System.Convert.ToBoolean(My.Settings.Default.optRegisterMDI), Configuration.Tunits);
            frmMainRegister frm = (frmMainRegister)(reg.getFrm(regID));
            if (frm != null)
            {
                frm.Show();
            }
        }

        public void mnuLayout_Click(System.Object sender, System.EventArgs e)
        {
            FormWindowState wsFrmConf = FormWindowState.Normal;
            ToolStripMenuItem mnu = (ToolStripMenuItem)sender;
            if (Configuration.frmConfMgr != null)
            {
                wsFrmConf = Configuration.frmConfMgr.WindowState;
                Configuration.frmConfMgr.WindowState = FormWindowState.Minimized;
            }
            if (ReferenceEquals(mnu, mnuCascade))
            {
                this.LayoutMdi(MdiLayout.Cascade);
            }
            else if (ReferenceEquals(mnu, mnuCascadeAll))
            {
                foreach (tStation stn in stationList)
                {
                    if (stn.frm != null)
                    {
                        if (stn.frm.WindowState != FormWindowState.Normal)
                        {
                            stn.frm.WindowState = FormWindowState.Normal;
                        }
                    }
                    if (stn.frmHA != null)
                    {
                        if (stn.frmHA.WindowState != FormWindowState.Normal)
                        {
                            stn.frmHA.WindowState = FormWindowState.Normal;
                        }
                    }
                }
                this.LayoutMdi(MdiLayout.Cascade);
            }
            if (Configuration.frmConfMgr != null)
            {
                Configuration.frmConfMgr.WindowState = wsFrmConf;
            }
        }

        public void mnuSupervisor_Click(System.Object sender, System.EventArgs e)
        {

            ToolStripMenuItem mnu = (ToolStripMenuItem)sender;
            string code = "";
            bool bOk = false;
            //Dim config As System.Configuration.Configuration

            if (ReferenceEquals(mnu, mnuLoginSupervisor))
            {
                code = Interaction.InputBox(Localization.getResStr(Configuration.supervEnterPasswordId) + ": ", Localization.getResStr(Configuration.supervPasswordId), "0000");
                if (code == sSupervisorPW)
                {
                    bOk = true;
                }
                if (bOk)
                {
                    // set stations control mode
                    //For Each elem In stationList
                    //    If elem.frm IsNot Nothing Then
                    //        elem.frm.setSupervisorControlMode(True)
                    //    End If
                    //    If elem.frmHA IsNot Nothing Then
                    //        elem.frmHA.setSupervisorControlMode(True)
                    //    End If
                    //Next
                    Configuration.bSupervisorLoggedIn = true;
                }
                else
                {
                    MessageBox.Show(Localization.getResStr(Configuration.supervInvalidPasswordId));
                }
            }
            else if (ReferenceEquals(mnu, mnuLogoutSupervisor))
            {
                // set stations monitor mode
                //For Each elem In stationList
                //    If elem.frm IsNot Nothing Then
                //        elem.frm.setSupervisorControlMode(False)
                //    End If
                //    If elem.frmHA IsNot Nothing Then
                //        elem.frmHA.setSupervisorControlMode(False)
                //    End If
                //Next
                Configuration.bSupervisorLoggedIn = false;
            }
            else if (ReferenceEquals(mnu, mnuChangePasswordSupervisor))
            {
                if (!Configuration.bSupervisorLoggedIn)
                {
                    MessageBox.Show(Localization.getResStr(Configuration.supervMustLoginId));
                }
                else
                {
                    code = Interaction.InputBox(Localization.getResStr(Configuration.supervEnterPasswordId) + ": ", Localization.getResStr(Configuration.supervPasswordId), "0000");
                    if (!string.IsNullOrEmpty(code))
                    {
                        sSupervisorPW = code;
                        // cannot write application config file with system.configuration
                        // so, used xml
                        System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                        string filename = Application.ExecutablePath + ".config";
                        xmlDoc.Load(filename);

                        bool bFound = false;
                        //<applicationSettings>
                        //   <Manager.My.MySettings>
                        //       <setting name="SupervisorPassword" serializeAs="String">
                        //           <value>0105</value>
                        System.Xml.XmlNodeList appSettings = xmlDoc.SelectNodes("/configuration/applicationSettings/Manager.My.MySettings/setting");
                        foreach (System.Xml.XmlNode node in appSettings)
                        {
                            if (node.Attributes["name"].Value == "SupervisorPassword")
                            {
                                foreach (System.Xml.XmlNode nodeChild in node.ChildNodes)
                                {
                                    if (nodeChild.Name == "value")
                                    {
                                        node.ChildNodes[0].InnerText = RoutinesLibrary.Data.DataType.StringUtils.base64Encode(code);
                                        bFound = true;
                                        break;
                                    }
                                }
                            }
                            if (bFound)
                            {
                                break;
                            }
                        }
                        if (bFound)
                        {
                            xmlDoc.Save(filename);
                        }
                        xmlDoc = null;
                    }
                }
            }

        }

        public void mnuUpdates_Click(System.Object sender, System.EventArgs e)
        {

            if (m_updateRequired)
            {
                m_UpdatesManager.ShowFormReInstall();
            }
            else
            {
                m_UpdatesManager.ShowFormUpdates();
            }
        }

        public void mnuAbout_Click(System.Object sender, System.EventArgs e)
        {
            mnuAboutShow();
        }

        private void mnuAboutShow()
        {
            if (ReferenceEquals(m_frmAbout, null))
            {
                CreateAboutPanel();
                m_frmAbout.Show();
            }
            else
            {
                if (m_frmAbout.IsDisposed)
                {
                    CreateAboutPanel();
                    m_frmAbout.Show();
                }
                else
                {
                    m_frmAbout.Close();
                }
            }
        }

        private void CreateAboutPanel()
        {
            m_frmAbout = new frmAbout(m_comHostController);
            m_frmAbout.MdiParent = this.ParentForm;
            m_frmAbout.Text = "";
            m_frmAbout.Dock = DockStyle.None;
        }

        #endregion

        #region Update

        private void CancelUpdateReInstall()
        {
            //show icon notification
            mnuHelp.Image = My.Resources.Resources.update;
            mnuUpdates.ForeColor = Color.OrangeRed;
        }

        private void UpdateAvailable()
        {
            frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
            notificationMsg.message = "Update available"; // FIXME
            notificationMsg.type = frmNotifications.typeNotificationMessage.UPDATE;
            m_frmNotifications.Add(notificationMsg);
        }

        #endregion

        #region Notifications

        public void mnuWarning_Click(object sender, EventArgs e)
        {
            if (!m_frmNotifications.Visible)
            {
                m_frmNotifications.ShowNotifications();
            }
        }

        private void Event_NotificationAvailable()
        {
            mnuWarning.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
        }

        private void Event_NotificationUnavailable()
        {
            mnuWarning.DisplayStyle = ToolStripItemDisplayStyle.Text;
        }

        private void setErrorNotifications(bool show)
        {
            Configuration.curShowErrorNotifications = show;
        }

        private void setStationControllerNotifications(bool show)
        {
            Configuration.curShowStationControllerNotifications = show;
        }

        private void setStationNotifications(bool show)
        {
            Configuration.curShowStationNotifications = show;
        }

        #endregion

        #region Options

        public void mnuOptions_Click(object sender, EventArgs e)
        {
            mnuOptionsShow();
        }

        private void mnuOptionsShow()
        {
            if (ReferenceEquals(m_frmOptions, null))
            {
                CreateOptionsPanel();
                m_frmOptions.Show();
            }
            else
            {
                if (m_frmOptions.IsDisposed)
                {
                    CreateOptionsPanel();
                    m_frmOptions.Show();
                }
                else
                {
                    m_frmOptions.Close();
                }
            }
        }

        private void CreateOptionsPanel()
        {
            m_frmOptions = new frmOptions();
            m_frmOptions.ChangedTemperatureUnits += setViewTempUnits;
            m_frmOptions.ChangedLanguage += setCulture;
            m_frmOptions.ChangedShowErrorNotifications += setErrorNotifications;
            m_frmOptions.ChangedShowStationControllerNotifications += setStationControllerNotifications;
            m_frmOptions.ChangedShowStationNotifications += setStationNotifications;
            m_frmOptions.MdiParent = this;
            m_frmOptions.Dock = DockStyle.None;
        }

        #endregion

        #region Help

        public void mnuRemoteManagerHelp_Click(object sender, EventArgs e)
        {
            string msgError = "";
            bool bOK = System.Convert.ToBoolean(RoutinesLibrary.Shell.ShellExecute.OpenDocument("JBC Remote Manager User's Manual.pdf", ref msgError));

            if (!bOK)
            {
                ShowMessagePopup(Localization.getResStr(Configuration.mnuHelpFailedId) + msgError);
            }
        }

        #endregion

        private void ShowMessagePopup(string message)
        {
            if (ReferenceEquals(m_frmMessagePopup, null) || m_frmMessagePopup.IsDisposed)
            {
                m_frmMessagePopup = new frmMessagePopup();
                m_frmMessagePopup.Load += m_frmMessagePopup.frmMessagePopup_Load;
                m_frmMessagePopup.Paint += m_frmMessagePopup.frmUpdatesPopup_Paint;
                m_frmMessagePopup.MdiParent = this;
            }

            MessagePopupResize();
            m_frmMessagePopup.SetMessage(message);
        }

        #region UI resize

        public void frmMain_GotFocus(object sender, System.EventArgs e)
        {

            if (m_frmNotifications != null && !m_frmNotifications.IsDisposed)
            {
                m_frmNotifications.Activate();
                m_frmNotifications.Refresh();
            }

            if (m_frmMessagePopup != null && !m_frmMessagePopup.IsDisposed)
            {
                m_frmMessagePopup.Activate();
                m_frmMessagePopup.Refresh();
            }

            if (m_frmAbout != null && !m_frmAbout.IsDisposed)
            {
                m_frmAbout.Activate();
                m_frmAbout.Refresh();
            }

        }

        public void frmMain_Resize(System.Object sender, System.EventArgs e)
        {
            NotificationResize();
            MessagePopupResize();
        }

        private void NotificationResize()
        {
            if (!ReferenceEquals(m_frmNotifications, null))
            {
                int x_NotificationForm = System.Convert.ToInt32((double)(this.Width - m_frmNotifications.Width) / 2);
                m_frmNotifications.Location = new Point(x_NotificationForm, 0);
            }
        }

        private void MessagePopupResize()
        {
            if (m_frmMessagePopup != null && !m_frmMessagePopup.IsDisposed)
            {
                int x_MessagePopup = System.Convert.ToInt32((double)(this.Width - m_frmMessagePopup.Width) / 2);
                int y_MessagePopup = System.Convert.ToInt32((double)(this.Height - m_frmMessagePopup.Height) / 2);
                m_frmMessagePopup.Location = new Point(x_MessagePopup, y_MessagePopup);
            }
        }

        #endregion

        #region Communication HostController

        private void SingleHostControllerConnected()
        {

            //Notification
            m_frmNotifications.Remove(frmNotifications.typeNotificationMessage.HOSTCONTROLLER);

            //Status bar
            this.lblStatusStatus.Image = My.Resources.Resources._on;
            this.lblStatusStatus.Text = Localization.getResStr(Configuration.hostControllerConnectedId);
        }

        private void MultipleHostControllerConnected()
        {

            //Notification
            m_frmNotifications.Remove(frmNotifications.typeNotificationMessage.HOSTCONTROLLER);

            frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
            notificationMsg.message = Localization.getResStr(Configuration.hostControllerMultipleConnectedId);
            notificationMsg.type = frmNotifications.typeNotificationMessage.HOSTCONTROLLER;
            m_frmNotifications.Add(notificationMsg);

            //Status bar
            this.lblStatusStatus.Image = My.Resources.Resources._off;
            this.lblStatusStatus.Text = Localization.getResStr(Configuration.hostControllerMultipleConnectedId);
        }

        private void HostControllerDisconnected()
        {

            //Notification
            m_frmNotifications.Remove(frmNotifications.typeNotificationMessage.HOSTCONTROLLER);

            frmNotifications.tNotificationMessage notificationMsg = new frmNotifications.tNotificationMessage();
            notificationMsg.message = Localization.getResStr(Configuration.hostControllerNoConnectedId);
            notificationMsg.type = frmNotifications.typeNotificationMessage.HOSTCONTROLLER;
            m_frmNotifications.Add(notificationMsg);

            //Status bar
            this.lblStatusStatus.Image = My.Resources.Resources._off;
            this.lblStatusStatus.Text = Localization.getResStr(Configuration.hostControllerNoConnectedId);
        }

        #endregion

    }
}
