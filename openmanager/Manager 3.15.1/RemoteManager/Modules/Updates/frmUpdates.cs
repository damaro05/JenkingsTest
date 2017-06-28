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

using System.Globalization;
using RemoteManager.HostControllerServiceReference;
using JBC_ConnectRemote;
using DataJBC;


namespace RemoteManager
{
    public partial class frmUpdates
    {

        private const int TEXTS_SEPARATION = 6; //Distancia entre textos y controles de formulario
        private const int NOTIFICATION_UPDATE_WARNING_DISTANCE_ROWS = 30; //Distancia entre filas
        private const int NOTIFICATION_UPDATE_WARNING_DISTANCE_BUTTON = 17; //Distancia del boton actualizar
        private const int NOTIFICATION_UPDATE_WARNING_HEIGHT = 104; //Altura del contenedor
        private const int NOTIFICATION_UPDATE_WARNING_DISTANCE_LAST_UPDATE = 20; //Distancia con el texto "Last Update"


        //JBC connect
        private JBC_API_Remote m_jbc;

        //Communications
        private CComHostController m_comHostController;

        //Updates information
        private dc_InfoUpdateSoftware m_infoUpdateSoftware;
        private dc_InfoUpdatePeriodicTime m_infoUpdatePeriodicTime;
        private dc_InfoCheckPeriodicTime m_infoCheckPeriodicTime;
        private dc_InfoUpdateSpecificTime m_infoUpdateSpecificTime;
        private bool m_isAvailableRemoteServerDownload = true;
        private string m_filesFolderLocation = "";

        //UI bussy
        bool m_formRefreshingValues = false; //Cuando está refrescando el formulario no hacer caso de los eventos que se generan


        public frmUpdates(CComHostController comHostController, JBC_API_Remote jbc)
        {
            InitializeComponent();
            m_jbc = jbc;
            m_comHostController = comHostController;
            ReLoadTexts();
        }

        public void frmUpdatesDock_Load(object sender, System.EventArgs e)
        {

            //Remove underline in sidebar panel
            this.linkLabelSearchUpdates.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelConfiguration.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.linkLabelUpdateStations.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;

            //Actualizar los datos de software
            LoadUpdateSoftware();
            RefreshUpdateSoftware();
        }

        public void frmOptions_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Rectangle temp_borderForm = this.ClientRectangle;
            CPanelStyle.SetPanelStyle(ref e, ref temp_borderForm);
        }

        public void ReLoadTexts()
        {

            //Esto evita que se procesen los eventos al cambiar los combobox
            m_formRefreshingValues = true;


            //
            //Panel
            //
            this.linkLabelSearchUpdates.Text = Localization.getResStr(Configuration.updatesPanelSearchUpdatesId);
            this.linkLabelConfiguration.Text = Localization.getResStr(Configuration.updatesPanelConfigurationUpdatesId);
            this.linkLabelUpdateStations.Text = Localization.getResStr(Configuration.updatesPanelUpdatesStationTitleId);

            //Title
            if (TabControl.SelectedIndex == 0)
            {
                this.labelTitle.Text = Localization.getResStr(Configuration.updatesPanelSearchTitleId);
            }
            else if (TabControl.SelectedIndex == 1)
            {
                this.labelTitle.Text = Localization.getResStr(Configuration.updatesPanelConfigurationUpdatesTitleId);
            }
            else
            {
                this.labelTitle.Text = Localization.getResStr(Configuration.updatesPanelUpdatesStationTitleId);
            }


            //
            //Search updates
            //
            this.UControlNotificationState_InitUpdateProcess.ReLoadTexts();
            this.UControlNotificationState_NoConnection.ReLoadTexts();
            this.UControlNotificationState_Ok.ReLoadTexts();
            this.UControlNotificationState_Warning.ReLoadTexts();

            this.labelTextLastUpdate.Text = Localization.getResStr(Configuration.updateLastUpdateId);


            //
            //Change configuration
            //
            this.labelConfigurationTitleAutomatic.Text = Localization.getResStr(Configuration.updatesPanelAutomaticUpdatesId);

            int comboBoxConfigurationUpdateSelectedIndex = this.comboBoxConfigurationUpdate.SelectedIndex;
            this.comboBoxConfigurationUpdate.Items.Clear();
            this.comboBoxConfigurationUpdate.Items.Insert(0, Localization.getResStr(Configuration.updatesPanelRButUpdateId));
            this.comboBoxConfigurationUpdate.Items.Insert(1, Localization.getResStr(Configuration.updatesPanelRButCheckId));
            this.comboBoxConfigurationUpdate.Items.Insert(2, Localization.getResStr(Configuration.updatesPanelRButDisableId));
            this.comboBoxConfigurationUpdate.SelectedIndex = comboBoxConfigurationUpdateSelectedIndex;

            this.labelTextInstallUpdates.Text = Localization.getResStr(Configuration.updatesPanelInstallNewUpdatesId);

            int comboBoxConfigurationUpdateDateSelectedIndex = this.comboBoxConfigurationUpdateDate.SelectedIndex;
            this.comboBoxConfigurationUpdateDate.Items.Clear();
            this.comboBoxConfigurationUpdateDate.Items.Insert(0, Localization.getResStr(Configuration.updatesPanelEveryDayId));
            this.comboBoxConfigurationUpdateDate.Items.Insert(1, Localization.getResStr(Configuration.updatesPanelEveryMondayId));
            this.comboBoxConfigurationUpdateDate.Items.Insert(2, Localization.getResStr(Configuration.updatesPanelEveryTuesdayId));
            this.comboBoxConfigurationUpdateDate.Items.Insert(3, Localization.getResStr(Configuration.updatesPanelEveryWednesdayId));
            this.comboBoxConfigurationUpdateDate.Items.Insert(4, Localization.getResStr(Configuration.updatesPanelEveryThursdayId));
            this.comboBoxConfigurationUpdateDate.Items.Insert(5, Localization.getResStr(Configuration.updatesPanelEveryFridayId));
            this.comboBoxConfigurationUpdateDate.Items.Insert(6, Localization.getResStr(Configuration.updatesPanelEverySaturdayId));
            this.comboBoxConfigurationUpdateDate.Items.Insert(7, Localization.getResStr(Configuration.updatesPanelEverySundayId));
            this.comboBoxConfigurationUpdateDate.SelectedIndex = comboBoxConfigurationUpdateDateSelectedIndex;

            this.labelTextAutomaticAt.Text = Localization.getResStr(Configuration.updatesPanelAtId);
            this.labelConfigurationTitleSchedule.Text = Localization.getResStr(Configuration.updatesPanelScheduleUpdatesId);
            this.checkBoxPlanificar.Text = Localization.getResStr(Configuration.updatesPanelAutomaticUpdateScheduleId);
            this.labelTextScheduleAt.Text = Localization.getResStr(Configuration.updatesPanelAtId);

            //Calculamos la separación de los elementos
            this.comboBoxConfigurationUpdateDate.Location = new Point(this.labelTextInstallUpdates.Location.X + this.labelTextInstallUpdates.Size.Width + TEXTS_SEPARATION, this.comboBoxConfigurationUpdateDate.Location.Y);
            this.labelTextAutomaticAt.Location = new Point(this.comboBoxConfigurationUpdateDate.Location.X + this.comboBoxConfigurationUpdateDate.Size.Width + TEXTS_SEPARATION, this.labelTextAutomaticAt.Location.Y);
            this.comboBoxAutomaticUpdateHour.Location = new Point(this.labelTextAutomaticAt.Location.X + this.labelTextAutomaticAt.Size.Width + TEXTS_SEPARATION, this.comboBoxAutomaticUpdateHour.Location.Y);

            this.labelTextScheduleAt.Location = new Point(this.dateTimePickerPlanificar.Location.X + this.dateTimePickerPlanificar.Size.Width + TEXTS_SEPARATION, this.labelTextScheduleAt.Location.Y);
            this.comboBoxScheduleUpdateHour.Location = new Point(this.labelTextScheduleAt.Location.X + this.labelTextScheduleAt.Size.Width + TEXTS_SEPARATION, this.comboBoxScheduleUpdateHour.Location.Y);


            //
            //Update stations
            //
            this.labelStationUpdateTitleName.Text = Localization.getResStr(Configuration.updatesPanelStationNameId);
            this.labelStationUpdateTitleModel.Text = Localization.getResStr(Configuration.updatesPanelStationModelId);
            this.labelStationUpdateTitleSoftwareVersion.Text = Localization.getResStr(Configuration.updatesPanelStationSoftwareVersionId);
            this.labelStationUpdateTitleUpdateAvailable.Text = Localization.getResStr(Configuration.updatesPanelStationUpdateAvailableId);

            this.butUpdateStations.Text = Localization.getResStr(Configuration.updatesPanelUpdatesStationTitleId);


            m_formRefreshingValues = false;
        }


        #region Sidebar panel

        public void linkLabelOpenManagerUpdates_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            //Actualizar los datos de software
            LoadUpdateSoftware();
            RefreshUpdateSoftware();

            //Tab index
            TabControl.SelectedIndex = 0;

            //Title
            this.labelTitle.Text = Localization.getResStr(Configuration.updatesPanelSearchTitleId);

            //Icons
            this.imgRefresh.Hide();
        }

        public void linkLabelConfiguration_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            //Actualizar los datos de configuración
            LoadAutomaticUpdateConfiguration();
            LoadScheduleUpdateConfiguration();
            LoadFilesLocationConfiguration();
            RefreshUpdateConfiguration();

            //Tab index
            TabControl.SelectedIndex = 1;

            //Title
            this.labelTitle.Text = Localization.getResStr(Configuration.updatesPanelConfigurationUpdatesTitleId);

            //Icons
            this.imgRefresh.Hide();
        }

        public void linkLabelUpdateStations_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            this.cbUpdateAllStations.CheckState = CheckState.Unchecked;

            //Actualizar los datos de estaciones
            RefreshUpdateStations();

            //Tab index
            TabControl.SelectedIndex = 2;

            //Title
            this.labelTitle.Text = Localization.getResStr(Configuration.updatesPanelUpdatesStationTitleId);

            //Icons
            this.imgRefresh.Show();
        }

        #endregion


        #region Tab 1 - Update information


        private void RefreshUpdateSoftware()
        {

            this.UControlNotificationState_InitUpdateProcess.Hide();

            int locationLastUpdateText = this.UControlNotificationState_NoConnection.Location.Y + NOTIFICATION_UPDATE_WARNING_DISTANCE_LAST_UPDATE;

            if (!m_comHostController.HostAvailable())
            {
                this.UControlNotificationState_NoConnection.Show();
                this.UControlNotificationState_Ok.Hide();
                this.UControlNotificationState_Warning.Hide();

                locationLastUpdateText += this.UControlNotificationState_NoConnection.Size.Height;

            }
            else if (!m_infoUpdateSoftware.stationControllerSwAvailable &&
                !m_infoUpdateSoftware.remoteManagerSwAvailable &&
                !m_infoUpdateSoftware.hostControllerSwAvailable &&
                !m_infoUpdateSoftware.webManagerSwAvailable)
            {
                this.UControlNotificationState_NoConnection.Hide();
                this.UControlNotificationState_Ok.Show();
                this.UControlNotificationState_Warning.Hide();

                locationLastUpdateText += this.UControlNotificationState_Ok.Size.Height;

            }
            else
            {
                this.UControlNotificationState_NoConnection.Hide();
                this.UControlNotificationState_Ok.Hide();
                this.UControlNotificationState_Warning.Show();

                int visibleRows = 0;

                //RemoteManager
                if (!m_infoUpdateSoftware.remoteManagerSwAvailable)
                {
                    this.UControlNotificationState_Warning.panelRemoteManagerVersion.Hide();
                }
                else
                {
                    this.UControlNotificationState_Warning.labelRemoteManagerVersionNum.Text = m_infoUpdateSoftware.remoteManagerSwVersion;
                    this.UControlNotificationState_Warning.panelRemoteManagerVersion.Show();

                    visibleRows++;
                }

                //StationController
                if (!m_infoUpdateSoftware.stationControllerSwAvailable)
                {
                    this.UControlNotificationState_Warning.panelStationControllerVersion.Hide();
                }
                else
                {
                    this.UControlNotificationState_Warning.labelStationControllerVersionNum.Text = m_infoUpdateSoftware.stationControllerSwVersion;
                    this.UControlNotificationState_Warning.panelStationControllerVersion.Show();

                    this.UControlNotificationState_Warning.panelStationControllerVersion.Location =
                        new System.Drawing.Point(this.UControlNotificationState_Warning.panelRemoteManagerVersion.Location.X,
                        this.UControlNotificationState_Warning.panelRemoteManagerVersion.Location.Y + NOTIFICATION_UPDATE_WARNING_DISTANCE_ROWS * visibleRows);

                    visibleRows++;
                }

                //HostController
                if (!m_infoUpdateSoftware.hostControllerSwAvailable)
                {
                    this.UControlNotificationState_Warning.panelHostControllerVersion.Hide();
                }
                else
                {
                    this.UControlNotificationState_Warning.labelHostControllerVersionNum.Text = m_infoUpdateSoftware.hostControllerSwVersion;
                    this.UControlNotificationState_Warning.panelHostControllerVersion.Show();

                    this.UControlNotificationState_Warning.panelHostControllerVersion.Location =
                        new System.Drawing.Point(this.UControlNotificationState_Warning.panelRemoteManagerVersion.Location.X,
                        this.UControlNotificationState_Warning.panelRemoteManagerVersion.Location.Y + NOTIFICATION_UPDATE_WARNING_DISTANCE_ROWS * visibleRows);

                    visibleRows++;
                }

                //WebManager
                if (!m_infoUpdateSoftware.webManagerSwAvailable)
                {
                    this.UControlNotificationState_Warning.panelWebManagerVersion.Hide();
                }
                else
                {
                    this.UControlNotificationState_Warning.labelWebManagerVersionNum.Text = m_infoUpdateSoftware.webManagerSwVersion;
                    this.UControlNotificationState_Warning.panelWebManagerVersion.Show();

                    this.UControlNotificationState_Warning.panelWebManagerVersion.Location =
                        new System.Drawing.Point(this.UControlNotificationState_Warning.panelRemoteManagerVersion.Location.X,
                        this.UControlNotificationState_Warning.panelRemoteManagerVersion.Location.Y + NOTIFICATION_UPDATE_WARNING_DISTANCE_ROWS * visibleRows);

                    visibleRows++;
                }

                //Button update
                this.UControlNotificationState_Warning.butUpdate.Location =
                    new System.Drawing.Point(this.UControlNotificationState_Warning.butUpdate.Location.X,
                    this.UControlNotificationState_Warning.panelRemoteManagerVersion.Location.Y + NOTIFICATION_UPDATE_WARNING_DISTANCE_ROWS * visibleRows + NOTIFICATION_UPDATE_WARNING_DISTANCE_BUTTON);

                //Container
                this.UControlNotificationState_Warning.Size =
                    new System.Drawing.Size(this.UControlNotificationState_Warning.Size.Width,
                    NOTIFICATION_UPDATE_WARNING_HEIGHT + NOTIFICATION_UPDATE_WARNING_DISTANCE_ROWS * visibleRows);

                locationLastUpdateText += this.UControlNotificationState_Warning.Size.Height;
            }


            //Colocar los textos de "búsqueda de última actualización"
            this.labelTextLastUpdate.Location = new Point(this.labelTextLastUpdate.Location.X, locationLastUpdateText);
            this.labelTextLastUpdateValue.Location = new Point(this.labelTextLastUpdateValue.Location.X, locationLastUpdateText);
        }

        private void LoadUpdateSoftware()
        {

            Cursor = Cursors.WaitCursor;

            //Pide los datos al HostController
            m_infoUpdateSoftware = m_comHostController.CheckUpdate();

            //Actualizar fecha última búsqueda de actualización
            if (m_comHostController.HostAvailable())
            {
                if (m_infoUpdateSoftware.lastUpdateDate.Year <= 1970)
                {
                    this.labelTextLastUpdateValue.Text = Localization.getResStr(Configuration.updatesPanelNeverId);
                }
                else
                {
                    this.labelTextLastUpdateValue.Text = m_infoUpdateSoftware.lastUpdateDate.ToString("HH:mm tt dd-MM-yyyy", new CultureInfo("en-US"));
                }
            }

            Cursor = Cursors.Arrow;
        }

        public void butUpdate_Click()
        {
            m_comHostController.UpdateSystem();

            this.UControlNotificationState_NoConnection.Hide();
            this.UControlNotificationState_Ok.Hide();
            this.UControlNotificationState_Warning.Hide();
            this.UControlNotificationState_InitUpdateProcess.Show();

            int locationLastUpdateText = this.UControlNotificationState_InitUpdateProcess.Location.Y +
                NOTIFICATION_UPDATE_WARNING_DISTANCE_LAST_UPDATE +
                this.UControlNotificationState_InitUpdateProcess.Size.Height;

            this.labelTextLastUpdate.Location = new Point(this.labelTextLastUpdate.Location.X, locationLastUpdateText);
            this.labelTextLastUpdateValue.Location = new Point(this.labelTextLastUpdateValue.Location.X, locationLastUpdateText);
        }

        public void butReconnect_Click()
        {

            //Actualizar los datos de software
            LoadUpdateSoftware();
            RefreshUpdateSoftware();
        }

        #endregion


        #region Tab 2 - Update configuration

        private void RefreshUpdateConfiguration()
        {

            m_formRefreshingValues = true;

            if (!m_comHostController.HostAvailable())
            {

                //Si no hay HostController deshabilitar todas las opciones
                this.comboBoxConfigurationUpdate.Enabled = false;
                this.labelTextInstallUpdates.Enabled = false;
                this.comboBoxConfigurationUpdateDate.Enabled = false;
                this.labelTextAutomaticAt.Enabled = false;
                this.comboBoxAutomaticUpdateHour.Enabled = false;
                this.checkBoxPlanificar.Enabled = false;
                this.dateTimePickerPlanificar.Enabled = false;
                this.labelTextScheduleAt.Enabled = false;
                this.comboBoxScheduleUpdateHour.Enabled = false;
                this.comboBoxConfigurationFilesLocation.Enabled = false;
                this.textBoxConfigurationFilesLocation.Enabled = false;
                this.imgConfigurationFilesLocation.Enabled = false;

            }
            else
            {

                //Si hay HostController habilitar todas las opciones
                this.comboBoxConfigurationUpdate.Enabled = true;
                this.labelTextInstallUpdates.Enabled = true;
                this.comboBoxConfigurationUpdateDate.Enabled = true;
                this.labelTextAutomaticAt.Enabled = true;
                this.comboBoxAutomaticUpdateHour.Enabled = true;
                this.checkBoxPlanificar.Enabled = true;
                this.dateTimePickerPlanificar.Enabled = true;
                this.labelTextScheduleAt.Enabled = true;
                this.comboBoxScheduleUpdateHour.Enabled = true;
                this.comboBoxConfigurationFilesLocation.Enabled = true;
                this.textBoxConfigurationFilesLocation.Enabled = true;
                this.imgConfigurationFilesLocation.Enabled = true;


                //
                //Update periodic
                //
                bool enablePeriodic = false;
                int updateDay = 0;
                int updateHour = 0;

                //Actualización automática
                if (m_infoUpdatePeriodicTime.available)
                {
                    this.comboBoxConfigurationUpdate.SelectedIndex = 0;
                    this.pictureBoxAutomaticUpdates.Visible = true;
                    this.pictureBoxAutomaticUpdates.Image = My.Resources.Resources.ok_icon;
                    enablePeriodic = true;

                    //Día de la semana
                    if (!m_infoUpdatePeriodicTime.modeDaily)
                    {
                        updateDay = m_infoUpdatePeriodicTime.weekday;
                    }
                    //Hora
                    updateHour = m_infoUpdatePeriodicTime.hour;

                    //Notificación automática
                }
                else if (m_infoCheckPeriodicTime.available)
                {
                    this.comboBoxConfigurationUpdate.SelectedIndex = 1;
                    this.pictureBoxAutomaticUpdates.Visible = false;

                    //No actualización
                }
                else
                {
                    this.comboBoxConfigurationUpdate.SelectedIndex = 2;
                    this.pictureBoxAutomaticUpdates.Visible = true;
                    this.pictureBoxAutomaticUpdates.Image = My.Resources.Resources.warning_icon;
                }

                //Habilitar opciones
                this.labelTextInstallUpdates.Enabled = enablePeriodic;
                this.comboBoxConfigurationUpdateDate.Enabled = enablePeriodic;
                this.labelTextAutomaticAt.Enabled = enablePeriodic;
                this.comboBoxAutomaticUpdateHour.Enabled = enablePeriodic;
                //Actualizar datos
                this.comboBoxConfigurationUpdateDate.SelectedIndex = updateDay;
                this.comboBoxAutomaticUpdateHour.SelectedIndex = updateHour;


                //
                //Schedule
                //
                bool enableSchedule = false;

                if (m_infoUpdateSpecificTime.available)
                {
                    enableSchedule = true;
                }

                //Habilitar opciones
                this.checkBoxPlanificar.Checked = enableSchedule;
                this.dateTimePickerPlanificar.Enabled = enableSchedule;
                this.labelTextScheduleAt.Enabled = enableSchedule;
                this.comboBoxScheduleUpdateHour.Enabled = enableSchedule;
                //Actualizar datos
                if (m_infoUpdateSpecificTime.time < DateTime.Now)
                {
                    this.dateTimePickerPlanificar.Value = DateTime.Now;
                }
                else
                {
                    this.dateTimePickerPlanificar.Value = m_infoUpdateSpecificTime.time;
                }
                this.comboBoxScheduleUpdateHour.SelectedIndex = m_infoUpdateSpecificTime.time.Hour;


                //
                //Files location
                //
                if (m_isAvailableRemoteServerDownload)
                {
                    this.comboBoxConfigurationFilesLocation.SelectedIndex = 0;
                    this.textBoxConfigurationFilesLocation.Enabled = false;
                }
                else
                {
                    this.comboBoxConfigurationFilesLocation.SelectedIndex = 1;
                    this.textBoxConfigurationFilesLocation.Enabled = true;
                }
                this.textBoxConfigurationFilesLocation.Text = m_filesFolderLocation;
            }

            m_formRefreshingValues = false;
        }

        #region Automatic updates

        private void LoadAutomaticUpdateConfiguration()
        {

            //Pide los datos al HostController
            m_infoUpdatePeriodicTime = m_comHostController.GetUpdatePeriodicTime();
            m_infoCheckPeriodicTime = m_comHostController.GetCheckPeriodicTime();
        }

        public void comboBoxConfigurationUpdate_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (m_formRefreshingValues)
            {
                return;
            }

            SetUpdatePeriodicTime();
            SetCheckPeriodicTime();
            LoadAutomaticUpdateConfiguration();
            RefreshUpdateConfiguration();
        }

        private void SetUpdatePeriodicTime()
        {

            dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime = new dc_InfoUpdatePeriodicTime();
            infoUpdatePeriodicTime.available = this.comboBoxConfigurationUpdate.SelectedIndex == 0;
            infoUpdatePeriodicTime.modeDaily = this.comboBoxConfigurationUpdateDate.SelectedIndex == 0;
            infoUpdatePeriodicTime.hour = (byte)this.comboBoxAutomaticUpdateHour.SelectedIndex;
            infoUpdatePeriodicTime.minute = (byte)0;
            infoUpdatePeriodicTime.weekday = (byte)this.comboBoxConfigurationUpdateDate.SelectedIndex;

            m_comHostController.SetUpdatePeriodicTime(infoUpdatePeriodicTime);
        }

        private void SetCheckPeriodicTime()
        {

            dc_InfoCheckPeriodicTime infoCheckPeriodicTime = new dc_InfoCheckPeriodicTime();
            infoCheckPeriodicTime.available = this.comboBoxConfigurationUpdate.SelectedIndex == 1;

            m_comHostController.SetCheckPeriodicTime(infoCheckPeriodicTime);
        }

        #endregion


        #region Schedule update

        private void LoadScheduleUpdateConfiguration()
        {

            //Pide los datos al HostController
            m_infoUpdateSpecificTime = m_comHostController.GetUpdateSpecificTimeInfo();
        }

        public void checkBoxPlanificar_CheckedChanged(object sender, EventArgs e)
        {

            if (m_formRefreshingValues)
            {
                return;
            }

            SetUpdateSpecificTime();
            LoadScheduleUpdateConfiguration();
            RefreshUpdateConfiguration();
        }

        private void SetUpdateSpecificTime()
        {

            dc_InfoUpdateSpecificTime infoUpdateSpecificTime = new dc_InfoUpdateSpecificTime();
            infoUpdateSpecificTime.available = this.checkBoxPlanificar.Checked;
            DateTime datePicker = dateTimePickerPlanificar.Value;
            System.DateTime datePlanificar = new System.DateTime(datePicker.Year, datePicker.Month, datePicker.Day, comboBoxScheduleUpdateHour.SelectedIndex, 0, 0);
            infoUpdateSpecificTime.time = datePlanificar;

            m_comHostController.SetUpdateSpecificTime(infoUpdateSpecificTime);
        }

        #endregion


        #region FilesLocation

        private void LoadFilesLocationConfiguration()
        {

            //Pide los datos al HostController
            m_isAvailableRemoteServerDownload = m_comHostController.IsAvailableRemoteServerDownload();
            m_filesFolderLocation = m_comHostController.GetUserFilesLocalFolderLocation();
        }

        public void comboBoxConfigurationFilesLocation_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (m_formRefreshingValues)
            {
                return;
            }

            SetFileLocationConfiguration();
            LoadFilesLocationConfiguration();
            RefreshUpdateConfiguration();
        }

        public void imgConfigurationFilesLocation_Click(object sender, EventArgs e)
        {
            if (FolderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxConfigurationFilesLocation.Text = FolderBrowserDialog1.SelectedPath;
            }
        }

        private void SetFileLocationConfiguration()
        {
            m_comHostController.SetAvailableRemoteServerDownload(this.comboBoxConfigurationFilesLocation.SelectedIndex == 0);
            m_comHostController.SetUserFilesLocalFolderLocation(this.textBoxConfigurationFilesLocation.Text);
        }

        #endregion

        #endregion


        #region Tab 3 - Update stations

        public void imgRefresh_Click(object sender, EventArgs e)
        {
            RefreshUpdateStations();
        }

        private void RefreshUpdateStations()
        {

            Cursor = Cursors.WaitCursor;

            bool existsUpdates = false;

            //Borramos todos los elementos
            this.flowLayoutPanel.Controls.Clear();

            //Lista de estaciones que se están actualizando
            List<long> stationListUpdating = new List<long>();

            //Recorremos todas las estaciones
            long[] stationList = m_jbc.GetStationList();
            foreach (long stationID in stationList)
            {

                //Nombre del Station Controller
                string hostName = m_jbc.GetStationHostName(stationID);

                //Si el Station Controller no está en la lista lo añadimos
                if (!this.flowLayoutPanel.Controls.ContainsKey(hostName))
                {

                    //Panel de Station Controller
                    uControlStationControllerUpdate rowStationController = new uControlStationControllerUpdate();
                    rowStationController.labelStationController.Text = hostName;
                    rowStationController.Name = hostName;
                    rowStationController.labelStationController.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
                    rowStationController.labelStationController.Click += ExpandStationControllerListUpdates_LinkLabel;
                    rowStationController.imgStationController.Click += ExpandStationControllerListUpdates_Arrow;
                    rowStationController.cbStationController.CheckedChanged += CheckedStationControllerListUpdates;

                    this.flowLayoutPanel.Controls.Add(rowStationController);

                    //Reorder Station Controllers
                    for (int i = 0; i <= this.flowLayoutPanel.Controls.Count - 1; i++)
                    {
                        if (double.Parse(this.flowLayoutPanel.Controls[i].Name) > double.Parse(hostName))
                        {
                            this.flowLayoutPanel.Controls.SetChildIndex(rowStationController, i);
                            break;
                        }
                    }

                    //Guardamos las estaciones pendientes de actualizar que tiene
                    stationListUpdating.AddRange(m_jbc.GetStationListUpdating(stationID));
                }

                //Obtener el control UI
                uControlStationControllerUpdate controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[hostName]);

                //Panel de estación
                uControlStationUpdate rowStation = new uControlStationUpdate();
                rowStation.Name = (stationID).ToString();
                rowStation.labelName.Text = m_jbc.GetStationName(stationID);
                if (rowStation.labelName.Text == "")
                {
                    rowStation.labelName.Text = Localization.getResStr(Configuration.gralNoNameId);
                }
                rowStation.labelModel.Text = m_jbc.GetStationModel(stationID);
                rowStation.labelSoftwareVersion.Text = m_jbc.GetStationSWversion(stationID);

                //No se puede actualizar este modelo
                if (!m_jbc.GetStationFeatures(stationID).FirmwareUpdate)
                {
                    rowStation.labelUpdateAvailable.Text = Localization.getResStr(Configuration.updatesPanelStationNotUpdatableId);
                    rowStation.cbStation.Enabled = false;

                    //Se está actualizando
                }
                else if (stationListUpdating.Contains(stationID))
                {
                    rowStation.labelUpdateAvailable.Text = Localization.getResStr(Configuration.updatesPanelStationInProgressId);
                    rowStation.labelUpdateAvailable.ForeColor = Color.Orange;
                    rowStation.cbStation.Enabled = false;

                    //Comprobamos si se puede actualizar
                }
                else
                {
                    bool isUpToDate = true;
                    string textTooltip = "";

                    //Obtenemos todos los firmware de la estación
                    CFirmwareStation[] infoUpdateFirmwareStation = m_jbc.GetFirmwareVersion(stationID);

                    //Recorremote todos los firmwares de la estación
                    for (var i = 0; i <= infoUpdateFirmwareStation.Count() - 1; i++)
                    {
                        dc_FirmwareStation dcInfoUpdateFirmware = new dc_FirmwareStation();
                        dcInfoUpdateFirmware.model = infoUpdateFirmwareStation[(int)i].Model;
                        dcInfoUpdateFirmware.hardwareVersion = infoUpdateFirmwareStation[(int)i].HardwareVersion;

                        //Obtenemos la información de firmware
                        List<dc_FirmwareStation> listNewInfoUpdateFirmware = m_comHostController.GetInfoUpdateFirmware(dcInfoUpdateFirmware);

                        textTooltip += infoUpdateFirmwareStation[(int)i].SoftwareVersion;

                        //Si hay mas de una posibilidad (CD - english, chinese)
                        if (listNewInfoUpdateFirmware.Count > 1)
                        {

                            if (listNewInfoUpdateFirmware[0].softwareVersion != infoUpdateFirmwareStation[(int)i].SoftwareVersion || listNewInfoUpdateFirmware[1].softwareVersion != infoUpdateFirmwareStation[(int)i].SoftwareVersion)
                            {
                                isUpToDate = false;
                                textTooltip += " " + System.Convert.ToString(Strings.ChrW(0x2192)) + " " + listNewInfoUpdateFirmware[0].softwareVersion + "/" + listNewInfoUpdateFirmware[1].softwareVersion;
                                rowStation.cBoxVersion.Items.Add(listNewInfoUpdateFirmware[0].softwareVersion + "(" + listNewInfoUpdateFirmware[0].language + ")");
                                rowStation.cBoxVersion.Items.Add(listNewInfoUpdateFirmware[1].softwareVersion + "(" + listNewInfoUpdateFirmware[1].language + ")");
                                rowStation.cBoxVersion.SelectedIndex = 0;
                                rowStation.cBoxVersion.Visible = true;

                                rowStation.AddFirmware(infoUpdateFirmwareStation[(int)i].HardwareVersion, System.Convert.ToString(listNewInfoUpdateFirmware[0].softwareVersion));
                            }
                        }
                        else if (listNewInfoUpdateFirmware.Count > 0)
                        {
                            //Existe una actualización
                            if (listNewInfoUpdateFirmware[0].softwareVersion != infoUpdateFirmwareStation[(int)i].SoftwareVersion)
                            {
                                isUpToDate = false;
                                textTooltip += " " + System.Convert.ToString(Strings.ChrW(0x2192)) + " " + listNewInfoUpdateFirmware[0].softwareVersion;

                                rowStation.AddFirmware(infoUpdateFirmwareStation[(int)i].HardwareVersion, System.Convert.ToString(listNewInfoUpdateFirmware[0].softwareVersion));
                            }
                        }
                        textTooltip += "\r\n";
                    }

                    if (isUpToDate)
                    {
                        rowStation.labelUpdateAvailable.Text = Localization.getResStr(Configuration.updatesPanelStationUpToDateId);
                        rowStation.cbStation.Enabled = false;
                    }
                    else
                    {
                        rowStation.labelUpdateAvailable.Text = Localization.getResStr(Configuration.updatesPanelStationNewSoftwareId);
                        rowStation.labelUpdateAvailable.ForeColor = Color.Orange;
                        rowStation.cbStation.Enabled = true;

                        //Tooltip to software version
                        rowStation.ToolTipVersion.SetToolTip(rowStation.labelUpdateAvailable, textTooltip);

                        existsUpdates = true;
                    }
                }

                rowStation.cbStation.CheckedChanged += CheckedStationListUpdates;

                controlStationControler.FlowLayoutPanel.Controls.Add(rowStation);

                //Reorder Stations
                for (int i = 0; i <= controlStationControler.FlowLayoutPanel.Controls.Count - 1; i++)
                {
                    if (double.Parse(((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[i])).labelName.Text) > double.Parse(m_jbc.GetStationName(stationID)))
                    {
                        controlStationControler.FlowLayoutPanel.Controls.SetChildIndex(rowStation, i);
                        break;
                    }
                }
            }

            //Si todas las estaciones de un Station Controller no se pueden actualizar, deshabilitamos el check box
            for (int i = 0; i <= this.flowLayoutPanel.Controls.Count - 1; i++)
            {
                bool allStationDisabled = true;

                //Obtener el control
                uControlStationControllerUpdate controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[i]);

                for (int j = 0; j <= controlStationControler.FlowLayoutPanel.Controls.Count - 1; j++)
                {
                    allStationDisabled = allStationDisabled && !((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).cbStation.Enabled;
                    if (!allStationDisabled)
                    {
                        break;
                    }
                }

                controlStationControler.cbStationController.Enabled = !allStationDisabled;
            }


            this.butUpdateStations.Enabled = existsUpdates;
            this.cbUpdateAllStations.Enabled = existsUpdates;

            Cursor = Cursors.Arrow;
        }

        private void ExpandStationControllerListUpdates_Arrow(object sender, System.EventArgs e)
        {
            string hostName = ((PictureBox)sender).Parent.Name;
            ExpandStationControllerListUpdates(hostName);
        }

        private void ExpandStationControllerListUpdates_LinkLabel(object sender, System.EventArgs e)
        {
            string hostName = ((LinkLabel)sender).Parent.Name;
            ExpandStationControllerListUpdates(hostName);
        }

        private void ExpandStationControllerListUpdates(string hostName)
        {
            if (this.flowLayoutPanel.Controls.ContainsKey(hostName))
            {

                //Obtener el control
                uControlStationControllerUpdate controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[hostName]);

                //Ocultar/mostrar el listado de estaciones
                if (controlStationControler.FlowLayoutPanel.Visible)
                {
                    controlStationControler.FlowLayoutPanel.Visible = false;
                    controlStationControler.imgStationController.Image = My.Resources.Resources.BlackRightArrow;
                }
                else
                {
                    controlStationControler.FlowLayoutPanel.Visible = true;
                    controlStationControler.imgStationController.Image = My.Resources.Resources.BlackDwnArrow;
                }
            }
        }

        public void CheckedAllStationController(object sender, EventArgs e)
        {

            if (m_formRefreshingValues)
            {
                return;
            }
            m_formRefreshingValues = true;

            bool cbCheck = this.cbUpdateAllStations.Checked;

            //Recorremos todos los Station Controller
            for (int i = 0; i <= this.flowLayoutPanel.Controls.Count - 1; i++)
            {

                //Obtener el control
                uControlStationControllerUpdate controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[i]);

                //Recorremos todas las estaciones que tiene
                for (int j = 0; j <= controlStationControler.FlowLayoutPanel.Controls.Count - 1; j++)
                {
                    if (((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).cbStation.Enabled)
                    {
                        ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).cbStation.Checked = cbCheck;
                    }
                }

                if (controlStationControler.cbStationController.Enabled)
                {
                    controlStationControler.cbStationController.Checked = cbCheck;
                }
            }

            m_formRefreshingValues = false;
        }

        private void CheckedStationControllerListUpdates(object sender, System.EventArgs e)
        {

            if (m_formRefreshingValues)
            {
                return;
            }
            m_formRefreshingValues = true;

            string hostName = ((CheckBox)sender).Parent.Name;

            if (this.flowLayoutPanel.Controls.ContainsKey(hostName))
            {

                //Obtener el control
                uControlStationControllerUpdate controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[hostName]);

                bool cbCheck = controlStationControler.cbStationController.Checked;

                //Check/uncheck all stations
                for (int i = 0; i <= controlStationControler.FlowLayoutPanel.Controls.Count - 1; i++)
                {
                    if (((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[i])).cbStation.Enabled)
                    {
                        ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[i])).cbStation.Checked = cbCheck;
                    }
                }

                //Check if is necessary check all station checkbox
                bool anyChecked = false;
                bool anyUnChecked = false;

                //Get if exists any check/uncheck station controller
                for (int i = 0; i <= this.flowLayoutPanel.Controls.Count - 1; i++)
                {
                    if (((uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[i])).cbStationController.Enabled)
                    {
                        anyChecked = anyChecked || ((uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[i])).cbStationController.Checked;
                        anyUnChecked = anyUnChecked || !((uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[i])).cbStationController.Checked;
                        if (anyChecked && anyUnChecked)
                        {
                            break;
                        }
                    }
                }

                //Update the all stations check
                if (anyChecked && anyUnChecked)
                {
                    this.cbUpdateAllStations.CheckState = CheckState.Indeterminate;
                }
                else if (anyChecked)
                {
                    this.cbUpdateAllStations.CheckState = CheckState.Checked;
                }
                else
                {
                    this.cbUpdateAllStations.CheckState = CheckState.Unchecked;
                }
            }

            m_formRefreshingValues = false;
        }

        private void CheckedStationListUpdates(object sender, System.EventArgs e)
        {

            if (m_formRefreshingValues)
            {
                return;
            }
            m_formRefreshingValues = true;

            string hostName = ((CheckBox)sender).Parent.Parent.Parent.Name;

            if (this.flowLayoutPanel.Controls.ContainsKey(hostName))
            {

                //Obtener el control
                uControlStationControllerUpdate controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[hostName]);

                bool anyChecked = false;
                bool anyUnChecked = false;

                //Get if exists any check/uncheck station
                for (int i = 0; i <= controlStationControler.FlowLayoutPanel.Controls.Count - 1; i++)
                {
                    if (((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[i])).cbStation.Enabled)
                    {
                        anyChecked = anyChecked || ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[i])).cbStation.Checked;
                        anyUnChecked = anyUnChecked || !((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[i])).cbStation.Checked;
                        if (anyChecked && anyUnChecked)
                        {
                            break;
                        }
                    }
                }

                //Update the station controller check
                if (anyChecked && anyUnChecked)
                {
                    controlStationControler.cbStationController.CheckState = CheckState.Indeterminate;
                }
                else if (anyChecked)
                {
                    controlStationControler.cbStationController.CheckState = CheckState.Checked;
                }
                else
                {
                    controlStationControler.cbStationController.CheckState = CheckState.Unchecked;
                }


                //Check if is necessary check all station checkbox
                anyChecked = false;
                anyUnChecked = false;

                //Get if exists any check/uncheck station controller
                for (int i = 0; i <= this.flowLayoutPanel.Controls.Count - 1; i++)
                {

                    //Obtener el control
                    controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[i]);

                    if (controlStationControler.cbStationController.Enabled)
                    {
                        anyChecked = anyChecked || controlStationControler.cbStationController.Checked | controlStationControler.cbStationController.CheckState == CheckState.Indeterminate;
                        anyUnChecked = anyUnChecked || !controlStationControler.cbStationController.Checked | controlStationControler.cbStationController.CheckState == CheckState.Indeterminate;
                        if (anyChecked && anyUnChecked)
                        {
                            break;
                        }
                    }
                }

                //Update the all stations check
                if (anyChecked && anyUnChecked)
                {
                    this.cbUpdateAllStations.CheckState = CheckState.Indeterminate;
                }
                else if (anyChecked)
                {
                    this.cbUpdateAllStations.CheckState = CheckState.Checked;
                }
                else
                {
                    this.cbUpdateAllStations.CheckState = CheckState.Unchecked;
                }
            }

            m_formRefreshingValues = false;
        }

        public void butUpdateStations_Click(object sender, EventArgs e)
        {

            //Recorremos todos los Station Controller
            for (int i = 0; i <= this.flowLayoutPanel.Controls.Count - 1; i++)
            {
                List<CFirmwareStation> stationsToUpdate = new List<CFirmwareStation>();
                long stationID = -1;

                //Obtener el control UI
                uControlStationControllerUpdate controlStationControler = (uControlStationControllerUpdate)(this.flowLayoutPanel.Controls[i]);

                //Recorremos todas las estaciones del Station Controller
                for (int j = 0; j <= controlStationControler.FlowLayoutPanel.Controls.Count - 1; j++)
                {
                    if (((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).cbStation.Checked)
                    {

                        foreach (DictionaryEntry firmwareEntry in ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).GetFirmware())
                        {
                            //Añadir estación a la lista de estaciones para actualizar
                            CFirmwareStation firmware = new CFirmwareStation();
                            firmware.StationID = long.Parse(((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).Name);
                            firmware.SoftwareVersion = (firmwareEntry.Value).ToString();
                            firmware.HardwareVersion = (firmwareEntry.Key).ToString();

                            stationsToUpdate.Add(firmware);
                            stationID = firmware.StationID;
                        }

                        //Modificar texto
                        ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).labelUpdateAvailable.Text = Localization.getResStr(Configuration.updatesPanelStationInProgressId);
                        ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).labelUpdateAvailable.ForeColor = Color.Orange;
                        ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).cbStation.Checked = false;
                        ((uControlStationUpdate)(controlStationControler.FlowLayoutPanel.Controls[j])).cbStation.Enabled = false;
                    }
                }

                //Si tiene alguna estación marcada para actualizar
                if (stationsToUpdate.Count > 0)
                {
                    m_jbc.UpdateStationsFirmware(stationID, stationsToUpdate);
                }
            }
        }

        #endregion

    }
}
