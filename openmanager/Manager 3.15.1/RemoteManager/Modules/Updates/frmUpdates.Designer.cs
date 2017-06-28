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
partial class frmUpdates : System.Windows.Forms.Form
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
            this.Panel1 = new System.Windows.Forms.Panel();
            this.Load += new System.EventHandler(frmUpdatesDock_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(frmOptions_Paint);
            this.linkLabelUpdateStations = new System.Windows.Forms.LinkLabel();
            this.linkLabelUpdateStations.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelUpdateStations_LinkClicked);
            this.linkLabelConfiguration = new System.Windows.Forms.LinkLabel();
            this.linkLabelConfiguration.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelConfiguration_LinkClicked);
            this.linkLabelSearchUpdates = new System.Windows.Forms.LinkLabel();
            this.linkLabelSearchUpdates.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelOpenManagerUpdates_LinkClicked);
            this.TabControl = new System.Windows.Forms.TabControl();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.labelTextLastUpdateValue = new System.Windows.Forms.Label();
            this.labelTextLastUpdate = new System.Windows.Forms.Label();
            this.UControlNotificationState_NoConnection = new uControlNotificationState_NoConnection();
            this.UControlNotificationState_NoConnection.ReconnectHostController += new uControlNotificationState_NoConnection.ReconnectHostControllerEventHandler(this.butReconnect_Click);
            this.UControlNotificationState_Ok = new uControlNotificationState_Ok();
            this.UControlNotificationState_InitUpdateProcess = new uControlNotificationState_InitUpdateProcess();
            this.UControlNotificationState_Warning = new uControlNotificationState_Warning();
            this.UControlNotificationState_Warning.UpdateSystem += new uControlNotificationState_Warning.UpdateSystemEventHandler(this.butUpdate_Click);
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.imgConfigurationFilesLocation = new System.Windows.Forms.PictureBox();
            this.imgConfigurationFilesLocation.Click += new System.EventHandler(this.imgConfigurationFilesLocation_Click);
            this.textBoxConfigurationFilesLocation = new System.Windows.Forms.TextBox();
            this.textBoxConfigurationFilesLocation.TextChanged += new System.EventHandler(this.comboBoxConfigurationFilesLocation_SelectedIndexChanged);
            this.comboBoxConfigurationFilesLocation = new System.Windows.Forms.ComboBox();
            this.comboBoxConfigurationFilesLocation.SelectedIndexChanged += new System.EventHandler(this.comboBoxConfigurationFilesLocation_SelectedIndexChanged);
            this.labelConfigurationTitleFilesLocation = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.pictureBoxAutomaticUpdates = new System.Windows.Forms.PictureBox();
            this.panelPlanificar = new System.Windows.Forms.Panel();
            this.comboBoxScheduleUpdateHour = new System.Windows.Forms.ComboBox();
            this.comboBoxScheduleUpdateHour.SelectedIndexChanged += new System.EventHandler(this.checkBoxPlanificar_CheckedChanged);
            this.labelTextScheduleAt = new System.Windows.Forms.Label();
            this.dateTimePickerPlanificar = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerPlanificar.ValueChanged += new System.EventHandler(this.checkBoxPlanificar_CheckedChanged);
            this.checkBoxPlanificar = new System.Windows.Forms.CheckBox();
            this.checkBoxPlanificar.CheckedChanged += new System.EventHandler(this.checkBoxPlanificar_CheckedChanged);
            this.labelConfigurationTitleSchedule = new System.Windows.Forms.Label();
            this.lineSeparator2 = new System.Windows.Forms.Label();
            this.labelConfigurationTitleAutomatic = new System.Windows.Forms.Label();
            this.lineSeparator = new System.Windows.Forms.Label();
            this.comboBoxAutomaticUpdateHour = new System.Windows.Forms.ComboBox();
            this.comboBoxAutomaticUpdateHour.SelectedIndexChanged += new System.EventHandler(this.comboBoxConfigurationUpdate_SelectedIndexChanged);
            this.labelTextAutomaticAt = new System.Windows.Forms.Label();
            this.comboBoxConfigurationUpdateDate = new System.Windows.Forms.ComboBox();
            this.comboBoxConfigurationUpdateDate.SelectedIndexChanged += new System.EventHandler(this.comboBoxConfigurationUpdate_SelectedIndexChanged);
            this.labelTextInstallUpdates = new System.Windows.Forms.Label();
            this.comboBoxConfigurationUpdate = new System.Windows.Forms.ComboBox();
            this.comboBoxConfigurationUpdate.SelectedIndexChanged += new System.EventHandler(this.comboBoxConfigurationUpdate_SelectedIndexChanged);
            this.TabPage3 = new System.Windows.Forms.TabPage();
            this.cbUpdateAllStations = new System.Windows.Forms.CheckBox();
            this.cbUpdateAllStations.CheckedChanged += new System.EventHandler(this.CheckedAllStationController);
            this.butUpdateStations = new System.Windows.Forms.Button();
            this.butUpdateStations.Click += new System.EventHandler(this.butUpdateStations_Click);
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.labelStationUpdateTitleUpdateAvailable = new System.Windows.Forms.Label();
            this.labelStationUpdateTitleSoftwareVersion = new System.Windows.Forms.Label();
            this.labelStationUpdateTitleModel = new System.Windows.Forms.Label();
            this.labelStationUpdateTitleName = new System.Windows.Forms.Label();
            this.labelTitle = new System.Windows.Forms.Label();
            this.imgRefresh = new System.Windows.Forms.PictureBox();
            this.imgRefresh.Click += new System.EventHandler(this.imgRefresh_Click);
            this.FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.Panel1.SuspendLayout();
            this.TabControl.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.imgConfigurationFilesLocation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxAutomaticUpdates).BeginInit();
            this.panelPlanificar.SuspendLayout();
            this.TabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.imgRefresh).BeginInit();
            this.SuspendLayout();
            //
            //Panel1
            //
            this.Panel1.BackColor = System.Drawing.SystemColors.Window;
            this.Panel1.Controls.Add(this.linkLabelUpdateStations);
            this.Panel1.Controls.Add(this.linkLabelConfiguration);
            this.Panel1.Controls.Add(this.linkLabelSearchUpdates);
            this.Panel1.Location = new System.Drawing.Point(1, 1);
            this.Panel1.Margin = new System.Windows.Forms.Padding(1, 4, 1, 4);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(226, 564);
            this.Panel1.TabIndex = 0;
            //
            //linkLabelUpdateStations
            //
            this.linkLabelUpdateStations.ActiveLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.linkLabelUpdateStations.AutoSize = true;
            this.linkLabelUpdateStations.LinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.linkLabelUpdateStations.Location = new System.Drawing.Point(20, 64);
            this.linkLabelUpdateStations.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabelUpdateStations.Name = "linkLabelUpdateStations";
            this.linkLabelUpdateStations.Size = new System.Drawing.Size(108, 14);
            this.linkLabelUpdateStations.TabIndex = 2;
            this.linkLabelUpdateStations.TabStop = true;
            this.linkLabelUpdateStations.Text = "Update stations";
            this.linkLabelUpdateStations.VisitedLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            //
            //linkLabelConfiguration
            //
            this.linkLabelConfiguration.ActiveLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.linkLabelConfiguration.AutoSize = true;
            this.linkLabelConfiguration.LinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.linkLabelConfiguration.Location = new System.Drawing.Point(20, 40);
            this.linkLabelConfiguration.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabelConfiguration.Name = "linkLabelConfiguration";
            this.linkLabelConfiguration.Size = new System.Drawing.Size(142, 14);
            this.linkLabelConfiguration.TabIndex = 1;
            this.linkLabelConfiguration.TabStop = true;
            this.linkLabelConfiguration.Text = "Change configuration";
            this.linkLabelConfiguration.VisitedLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            //
            //linkLabelSearchUpdates
            //
            this.linkLabelSearchUpdates.ActiveLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.linkLabelSearchUpdates.AutoSize = true;
            this.linkLabelSearchUpdates.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.linkLabelSearchUpdates.LinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.linkLabelSearchUpdates.Location = new System.Drawing.Point(20, 16);
            this.linkLabelSearchUpdates.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkLabelSearchUpdates.Name = "linkLabelSearchUpdates";
            this.linkLabelSearchUpdates.Size = new System.Drawing.Size(106, 14);
            this.linkLabelSearchUpdates.TabIndex = 0;
            this.linkLabelSearchUpdates.TabStop = true;
            this.linkLabelSearchUpdates.Text = "Search updates";
            this.linkLabelSearchUpdates.VisitedLinkColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            //
            //TabControl
            //
            this.TabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TabControl.Controls.Add(this.TabPage1);
            this.TabControl.Controls.Add(this.TabPage2);
            this.TabControl.Controls.Add(this.TabPage3);
            this.TabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.TabControl.Location = new System.Drawing.Point(241, 55);
            this.TabControl.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(803, 498);
            this.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControl.TabIndex = 1;
            //
            //TabPage1
            //
            this.TabPage1.Controls.Add(this.labelTextLastUpdateValue);
            this.TabPage1.Controls.Add(this.labelTextLastUpdate);
            this.TabPage1.Controls.Add(this.UControlNotificationState_NoConnection);
            this.TabPage1.Controls.Add(this.UControlNotificationState_Ok);
            this.TabPage1.Controls.Add(this.UControlNotificationState_InitUpdateProcess);
            this.TabPage1.Controls.Add(this.UControlNotificationState_Warning);
            this.TabPage1.Location = new System.Drawing.Point(4, 5);
            this.TabPage1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.TabPage1.Size = new System.Drawing.Size(795, 489);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "TabPage1";
            this.TabPage1.UseVisualStyleBackColor = true;
            //
            //labelTextLastUpdateValue
            //
            this.labelTextLastUpdateValue.AutoSize = true;
            this.labelTextLastUpdateValue.Location = new System.Drawing.Point(257, 110);
            this.labelTextLastUpdateValue.Name = "labelTextLastUpdateValue";
            this.labelTextLastUpdateValue.Size = new System.Drawing.Size(44, 14);
            this.labelTextLastUpdateValue.TabIndex = 4;
            this.labelTextLastUpdateValue.Text = "Never";
            //
            //labelTextLastUpdate
            //
            this.labelTextLastUpdate.AutoSize = true;
            this.labelTextLastUpdate.Location = new System.Drawing.Point(4, 110);
            this.labelTextLastUpdate.Name = "labelTextLastUpdate";
            this.labelTextLastUpdate.Size = new System.Drawing.Size(88, 14);
            this.labelTextLastUpdate.TabIndex = 3;
            this.labelTextLastUpdate.Text = "Last update:";
            //
            //UControlNotificationState_NoConnection
            //
            this.UControlNotificationState_NoConnection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UControlNotificationState_NoConnection.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.UControlNotificationState_NoConnection.Location = new System.Drawing.Point(4, 15);
            this.UControlNotificationState_NoConnection.Margin = new System.Windows.Forms.Padding(0);
            this.UControlNotificationState_NoConnection.Name = "UControlNotificationState_NoConnection";
            this.UControlNotificationState_NoConnection.Size = new System.Drawing.Size(500, 127);
            this.UControlNotificationState_NoConnection.TabIndex = 0;
            //
            //UControlNotificationState_Ok
            //
            this.UControlNotificationState_Ok.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UControlNotificationState_Ok.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.UControlNotificationState_Ok.Location = new System.Drawing.Point(4, 15);
            this.UControlNotificationState_Ok.Margin = new System.Windows.Forms.Padding(0);
            this.UControlNotificationState_Ok.Name = "UControlNotificationState_Ok";
            this.UControlNotificationState_Ok.Size = new System.Drawing.Size(500, 75);
            this.UControlNotificationState_Ok.TabIndex = 1;
            this.UControlNotificationState_Ok.Visible = false;
            //
            //UControlNotificationState_InitUpdateProcess
            //
            this.UControlNotificationState_InitUpdateProcess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UControlNotificationState_InitUpdateProcess.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.UControlNotificationState_InitUpdateProcess.Location = new System.Drawing.Point(4, 15);
            this.UControlNotificationState_InitUpdateProcess.Margin = new System.Windows.Forms.Padding(0);
            this.UControlNotificationState_InitUpdateProcess.Name = "UControlNotificationState_InitUpdateProcess";
            this.UControlNotificationState_InitUpdateProcess.Size = new System.Drawing.Size(500, 141);
            this.UControlNotificationState_InitUpdateProcess.TabIndex = 5;
            this.UControlNotificationState_InitUpdateProcess.Visible = false;
            //
            //UControlNotificationState_Warning
            //
            this.UControlNotificationState_Warning.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.UControlNotificationState_Warning.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.UControlNotificationState_Warning.Location = new System.Drawing.Point(4, 15);
            this.UControlNotificationState_Warning.Margin = new System.Windows.Forms.Padding(0);
            this.UControlNotificationState_Warning.Name = "UControlNotificationState_Warning";
            this.UControlNotificationState_Warning.Size = new System.Drawing.Size(500, 224);
            this.UControlNotificationState_Warning.TabIndex = 2;
            this.UControlNotificationState_Warning.Visible = false;
            //
            //TabPage2
            //
            this.TabPage2.Controls.Add(this.imgConfigurationFilesLocation);
            this.TabPage2.Controls.Add(this.textBoxConfigurationFilesLocation);
            this.TabPage2.Controls.Add(this.comboBoxConfigurationFilesLocation);
            this.TabPage2.Controls.Add(this.labelConfigurationTitleFilesLocation);
            this.TabPage2.Controls.Add(this.Label2);
            this.TabPage2.Controls.Add(this.pictureBoxAutomaticUpdates);
            this.TabPage2.Controls.Add(this.panelPlanificar);
            this.TabPage2.Controls.Add(this.checkBoxPlanificar);
            this.TabPage2.Controls.Add(this.labelConfigurationTitleSchedule);
            this.TabPage2.Controls.Add(this.lineSeparator2);
            this.TabPage2.Controls.Add(this.labelConfigurationTitleAutomatic);
            this.TabPage2.Controls.Add(this.lineSeparator);
            this.TabPage2.Controls.Add(this.comboBoxAutomaticUpdateHour);
            this.TabPage2.Controls.Add(this.labelTextAutomaticAt);
            this.TabPage2.Controls.Add(this.comboBoxConfigurationUpdateDate);
            this.TabPage2.Controls.Add(this.labelTextInstallUpdates);
            this.TabPage2.Controls.Add(this.comboBoxConfigurationUpdate);
            this.TabPage2.Location = new System.Drawing.Point(4, 25);
            this.TabPage2.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.TabPage2.Size = new System.Drawing.Size(795, 469);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "TabPage2";
            this.TabPage2.UseVisualStyleBackColor = true;
            //
            //imgConfigurationFilesLocation
            //
            this.imgConfigurationFilesLocation.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgConfigurationFilesLocation.Image = My.Resources.Resources.Folder_16x16;
            this.imgConfigurationFilesLocation.Location = new System.Drawing.Point(588, 314);
            this.imgConfigurationFilesLocation.Name = "imgConfigurationFilesLocation";
            this.imgConfigurationFilesLocation.Size = new System.Drawing.Size(16, 16);
            this.imgConfigurationFilesLocation.TabIndex = 4;
            this.imgConfigurationFilesLocation.TabStop = false;
            //
            //textBoxConfigurationFilesLocation
            //
            this.textBoxConfigurationFilesLocation.Location = new System.Drawing.Point(56, 311);
            this.textBoxConfigurationFilesLocation.Name = "textBoxConfigurationFilesLocation";
            this.textBoxConfigurationFilesLocation.Size = new System.Drawing.Size(526, 22);
            this.textBoxConfigurationFilesLocation.TabIndex = 42;
            //
            //comboBoxConfigurationFilesLocation
            //
            this.comboBoxConfigurationFilesLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxConfigurationFilesLocation.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.comboBoxConfigurationFilesLocation.FormattingEnabled = true;
            this.comboBoxConfigurationFilesLocation.Items.AddRange(new object[] { "Download updates from JBC server", "Specify path of update files" });
            this.comboBoxConfigurationFilesLocation.Location = new System.Drawing.Point(53, 278);
            this.comboBoxConfigurationFilesLocation.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.comboBoxConfigurationFilesLocation.Name = "comboBoxConfigurationFilesLocation";
            this.comboBoxConfigurationFilesLocation.Size = new System.Drawing.Size(551, 22);
            this.comboBoxConfigurationFilesLocation.TabIndex = 41;
            //
            //labelConfigurationTitleFilesLocation
            //
            this.labelConfigurationTitleFilesLocation.AutoSize = true;
            this.labelConfigurationTitleFilesLocation.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelConfigurationTitleFilesLocation.Location = new System.Drawing.Point(4, 248);
            this.labelConfigurationTitleFilesLocation.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelConfigurationTitleFilesLocation.Name = "labelConfigurationTitleFilesLocation";
            this.labelConfigurationTitleFilesLocation.Size = new System.Drawing.Size(94, 14);
            this.labelConfigurationTitleFilesLocation.TabIndex = 39;
            this.labelConfigurationTitleFilesLocation.Text = "Files location";
            //
            //Label2
            //
            this.Label2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.Label2.Location = new System.Drawing.Point(22, 254);
            this.Label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(608, 1);
            this.Label2.TabIndex = 40;
            //
            //pictureBoxAutomaticUpdates
            //
            this.pictureBoxAutomaticUpdates.Location = new System.Drawing.Point(10, 42);
            this.pictureBoxAutomaticUpdates.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBoxAutomaticUpdates.Name = "pictureBoxAutomaticUpdates";
            this.pictureBoxAutomaticUpdates.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxAutomaticUpdates.TabIndex = 38;
            this.pictureBoxAutomaticUpdates.TabStop = false;
            this.pictureBoxAutomaticUpdates.Visible = false;
            //
            //panelPlanificar
            //
            this.panelPlanificar.Controls.Add(this.comboBoxScheduleUpdateHour);
            this.panelPlanificar.Controls.Add(this.labelTextScheduleAt);
            this.panelPlanificar.Controls.Add(this.dateTimePickerPlanificar);
            this.panelPlanificar.Location = new System.Drawing.Point(81, 191);
            this.panelPlanificar.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.panelPlanificar.Name = "panelPlanificar";
            this.panelPlanificar.Size = new System.Drawing.Size(458, 33);
            this.panelPlanificar.TabIndex = 37;
            //
            //comboBoxScheduleUpdateHour
            //
            this.comboBoxScheduleUpdateHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScheduleUpdateHour.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.comboBoxScheduleUpdateHour.FormattingEnabled = true;
            this.comboBoxScheduleUpdateHour.Items.AddRange(new object[] { "0:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" });
            this.comboBoxScheduleUpdateHour.Location = new System.Drawing.Point(236, 4);
            this.comboBoxScheduleUpdateHour.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.comboBoxScheduleUpdateHour.Name = "comboBoxScheduleUpdateHour";
            this.comboBoxScheduleUpdateHour.Size = new System.Drawing.Size(84, 22);
            this.comboBoxScheduleUpdateHour.TabIndex = 38;
            //
            //labelTextScheduleAt
            //
            this.labelTextScheduleAt.AutoSize = true;
            this.labelTextScheduleAt.Location = new System.Drawing.Point(210, 7);
            this.labelTextScheduleAt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTextScheduleAt.Name = "labelTextScheduleAt";
            this.labelTextScheduleAt.Size = new System.Drawing.Size(20, 14);
            this.labelTextScheduleAt.TabIndex = 38;
            this.labelTextScheduleAt.Text = "at";
            //
            //dateTimePickerPlanificar
            //
            this.dateTimePickerPlanificar.Location = new System.Drawing.Point(4, 4);
            this.dateTimePickerPlanificar.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.dateTimePickerPlanificar.Name = "dateTimePickerPlanificar";
            this.dateTimePickerPlanificar.Size = new System.Drawing.Size(200, 22);
            this.dateTimePickerPlanificar.TabIndex = 17;
            //
            //checkBoxPlanificar
            //
            this.checkBoxPlanificar.AutoSize = true;
            this.checkBoxPlanificar.Location = new System.Drawing.Point(53, 163);
            this.checkBoxPlanificar.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.checkBoxPlanificar.Name = "checkBoxPlanificar";
            this.checkBoxPlanificar.Size = new System.Drawing.Size(205, 18);
            this.checkBoxPlanificar.TabIndex = 36;
            this.checkBoxPlanificar.Text = "Schedule automatic updates";
            this.checkBoxPlanificar.UseVisualStyleBackColor = true;
            //
            //labelConfigurationTitleSchedule
            //
            this.labelConfigurationTitleSchedule.AutoSize = true;
            this.labelConfigurationTitleSchedule.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelConfigurationTitleSchedule.Location = new System.Drawing.Point(4, 130);
            this.labelConfigurationTitleSchedule.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelConfigurationTitleSchedule.Name = "labelConfigurationTitleSchedule";
            this.labelConfigurationTitleSchedule.Size = new System.Drawing.Size(123, 14);
            this.labelConfigurationTitleSchedule.TabIndex = 34;
            this.labelConfigurationTitleSchedule.Text = "Schedule updates";
            //
            //lineSeparator2
            //
            this.lineSeparator2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lineSeparator2.Location = new System.Drawing.Point(22, 136);
            this.lineSeparator2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lineSeparator2.Name = "lineSeparator2";
            this.lineSeparator2.Size = new System.Drawing.Size(608, 1);
            this.lineSeparator2.TabIndex = 35;
            //
            //labelConfigurationTitleAutomatic
            //
            this.labelConfigurationTitleAutomatic.AutoSize = true;
            this.labelConfigurationTitleAutomatic.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelConfigurationTitleAutomatic.Location = new System.Drawing.Point(4, 16);
            this.labelConfigurationTitleAutomatic.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelConfigurationTitleAutomatic.Name = "labelConfigurationTitleAutomatic";
            this.labelConfigurationTitleAutomatic.Size = new System.Drawing.Size(129, 14);
            this.labelConfigurationTitleAutomatic.TabIndex = 5;
            this.labelConfigurationTitleAutomatic.Text = "Automatic updates";
            //
            //lineSeparator
            //
            this.lineSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lineSeparator.Location = new System.Drawing.Point(22, 23);
            this.lineSeparator.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lineSeparator.Name = "lineSeparator";
            this.lineSeparator.Size = new System.Drawing.Size(608, 1);
            this.lineSeparator.TabIndex = 33;
            //
            //comboBoxAutomaticUpdateHour
            //
            this.comboBoxAutomaticUpdateHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAutomaticUpdateHour.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.comboBoxAutomaticUpdateHour.FormattingEnabled = true;
            this.comboBoxAutomaticUpdateHour.Items.AddRange(new object[] { "0:00", "1:00", "2:00", "3:00", "4:00", "5:00", "6:00", "7:00", "8:00", "9:00", "10:00", "11:00", "12:00", "13:00", "14:00", "15:00", "16:00", "17:00", "18:00", "19:00", "20:00", "21:00", "22:00", "23:00" });
            this.comboBoxAutomaticUpdateHour.Location = new System.Drawing.Point(397, 79);
            this.comboBoxAutomaticUpdateHour.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.comboBoxAutomaticUpdateHour.Name = "comboBoxAutomaticUpdateHour";
            this.comboBoxAutomaticUpdateHour.Size = new System.Drawing.Size(84, 22);
            this.comboBoxAutomaticUpdateHour.TabIndex = 4;
            //
            //labelTextAutomaticAt
            //
            this.labelTextAutomaticAt.AutoSize = true;
            this.labelTextAutomaticAt.Location = new System.Drawing.Point(369, 82);
            this.labelTextAutomaticAt.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTextAutomaticAt.Name = "labelTextAutomaticAt";
            this.labelTextAutomaticAt.Size = new System.Drawing.Size(20, 14);
            this.labelTextAutomaticAt.TabIndex = 3;
            this.labelTextAutomaticAt.Text = "at";
            //
            //comboBoxConfigurationUpdateDate
            //
            this.comboBoxConfigurationUpdateDate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxConfigurationUpdateDate.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.comboBoxConfigurationUpdateDate.FormattingEnabled = true;
            this.comboBoxConfigurationUpdateDate.Items.AddRange(new object[] { "Everyday", "Every Monday", "Every Tuesday", "Every Wednesday", "Every Thursday", "Every Friday", "Every Saturday", "Every Sunday" });
            this.comboBoxConfigurationUpdateDate.Location = new System.Drawing.Point(197, 79);
            this.comboBoxConfigurationUpdateDate.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.comboBoxConfigurationUpdateDate.Name = "comboBoxConfigurationUpdateDate";
            this.comboBoxConfigurationUpdateDate.Size = new System.Drawing.Size(166, 22);
            this.comboBoxConfigurationUpdateDate.TabIndex = 2;
            //
            //labelTextInstallUpdates
            //
            this.labelTextInstallUpdates.AutoSize = true;
            this.labelTextInstallUpdates.Location = new System.Drawing.Point(53, 82);
            this.labelTextInstallUpdates.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTextInstallUpdates.Name = "labelTextInstallUpdates";
            this.labelTextInstallUpdates.Size = new System.Drawing.Size(138, 14);
            this.labelTextInstallUpdates.TabIndex = 1;
            this.labelTextInstallUpdates.Text = "Install new updates:";
            //
            //comboBoxConfigurationUpdate
            //
            this.comboBoxConfigurationUpdate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxConfigurationUpdate.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.comboBoxConfigurationUpdate.FormattingEnabled = true;
            this.comboBoxConfigurationUpdate.Items.AddRange(new object[] { "Allow Remote Manager install updates (recommended)", "Notify to install updates", "Never check for updates (not recommended)" });
            this.comboBoxConfigurationUpdate.Location = new System.Drawing.Point(53, 46);
            this.comboBoxConfigurationUpdate.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
            this.comboBoxConfigurationUpdate.Name = "comboBoxConfigurationUpdate";
            this.comboBoxConfigurationUpdate.Size = new System.Drawing.Size(551, 22);
            this.comboBoxConfigurationUpdate.TabIndex = 0;
            //
            //TabPage3
            //
            this.TabPage3.Controls.Add(this.cbUpdateAllStations);
            this.TabPage3.Controls.Add(this.butUpdateStations);
            this.TabPage3.Controls.Add(this.flowLayoutPanel);
            this.TabPage3.Controls.Add(this.labelStationUpdateTitleUpdateAvailable);
            this.TabPage3.Controls.Add(this.labelStationUpdateTitleSoftwareVersion);
            this.TabPage3.Controls.Add(this.labelStationUpdateTitleModel);
            this.TabPage3.Controls.Add(this.labelStationUpdateTitleName);
            this.TabPage3.Location = new System.Drawing.Point(4, 5);
            this.TabPage3.Name = "TabPage3";
            this.TabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage3.Size = new System.Drawing.Size(795, 489);
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "TabPage3";
            this.TabPage3.UseVisualStyleBackColor = true;
            //
            //cbUpdateAllStations
            //
            this.cbUpdateAllStations.AutoSize = true;
            this.cbUpdateAllStations.Location = new System.Drawing.Point(4, 26);
            this.cbUpdateAllStations.Name = "cbUpdateAllStations";
            this.cbUpdateAllStations.Size = new System.Drawing.Size(15, 14);
            this.cbUpdateAllStations.TabIndex = 16;
            this.cbUpdateAllStations.UseVisualStyleBackColor = true;
            //
            //butUpdateStations
            //
            this.butUpdateStations.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butUpdateStations.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.butUpdateStations.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.butUpdateStations.Location = new System.Drawing.Point(631, 451);
            this.butUpdateStations.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butUpdateStations.Name = "butUpdateStations";
            this.butUpdateStations.Size = new System.Drawing.Size(157, 25);
            this.butUpdateStations.TabIndex = 15;
            this.butUpdateStations.Text = "Update stations";
            this.butUpdateStations.UseVisualStyleBackColor = true;
            //
            //flowLayoutPanel
            //
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 67);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(792, 364);
            this.flowLayoutPanel.TabIndex = 14;
            this.flowLayoutPanel.WrapContents = false;
            //
            //labelStationUpdateTitleUpdateAvailable
            //
            this.labelStationUpdateTitleUpdateAvailable.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelStationUpdateTitleUpdateAvailable.Location = new System.Drawing.Point(425, 16);
            this.labelStationUpdateTitleUpdateAvailable.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStationUpdateTitleUpdateAvailable.Name = "labelStationUpdateTitleUpdateAvailable";
            this.labelStationUpdateTitleUpdateAvailable.Size = new System.Drawing.Size(180, 33);
            this.labelStationUpdateTitleUpdateAvailable.TabIndex = 13;
            this.labelStationUpdateTitleUpdateAvailable.Text = "Update available";
            this.labelStationUpdateTitleUpdateAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelStationUpdateTitleSoftwareVersion
            //
            this.labelStationUpdateTitleSoftwareVersion.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelStationUpdateTitleSoftwareVersion.Location = new System.Drawing.Point(325, 16);
            this.labelStationUpdateTitleSoftwareVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStationUpdateTitleSoftwareVersion.Name = "labelStationUpdateTitleSoftwareVersion";
            this.labelStationUpdateTitleSoftwareVersion.Size = new System.Drawing.Size(90, 33);
            this.labelStationUpdateTitleSoftwareVersion.TabIndex = 12;
            this.labelStationUpdateTitleSoftwareVersion.Text = "Software version";
            this.labelStationUpdateTitleSoftwareVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelStationUpdateTitleModel
            //
            this.labelStationUpdateTitleModel.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelStationUpdateTitleModel.Location = new System.Drawing.Point(245, 16);
            this.labelStationUpdateTitleModel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStationUpdateTitleModel.Name = "labelStationUpdateTitleModel";
            this.labelStationUpdateTitleModel.Size = new System.Drawing.Size(70, 33);
            this.labelStationUpdateTitleModel.TabIndex = 11;
            this.labelStationUpdateTitleModel.Text = "Model";
            this.labelStationUpdateTitleModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelStationUpdateTitleName
            //
            this.labelStationUpdateTitleName.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelStationUpdateTitleName.Location = new System.Drawing.Point(43, 16);
            this.labelStationUpdateTitleName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelStationUpdateTitleName.Name = "labelStationUpdateTitleName";
            this.labelStationUpdateTitleName.Size = new System.Drawing.Size(192, 33);
            this.labelStationUpdateTitleName.TabIndex = 10;
            this.labelStationUpdateTitleName.Text = "Station name";
            this.labelStationUpdateTitleName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(246, 24);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(214, 18);
            this.labelTitle.TabIndex = 2;
            this.labelTitle.Text = "Open Manager updates";
            //
            //imgRefresh
            //
            this.imgRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.imgRefresh.Image = My.Resources.Resources.refresh;
            this.imgRefresh.Location = new System.Drawing.Point(1015, 21);
            this.imgRefresh.Name = "imgRefresh";
            this.imgRefresh.Size = new System.Drawing.Size(24, 24);
            this.imgRefresh.TabIndex = 3;
            this.imgRefresh.TabStop = false;
            this.imgRefresh.Visible = false;
            //
            //frmUpdates
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1055, 566);
            this.Controls.Add(this.imgRefresh);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.Panel1);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdates";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Updates";
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.TabControl.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.imgConfigurationFilesLocation).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pictureBoxAutomaticUpdates).EndInit();
            this.panelPlanificar.ResumeLayout(false);
            this.panelPlanificar.PerformLayout();
            this.TabPage3.ResumeLayout(false);
            this.TabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.imgRefresh).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.LinkLabel linkLabelSearchUpdates;
        internal System.Windows.Forms.LinkLabel linkLabelConfiguration;
        internal System.Windows.Forms.TabControl TabControl;
        internal System.Windows.Forms.TabPage TabPage2;
        internal System.Windows.Forms.Label labelTitle;
        internal System.Windows.Forms.ComboBox comboBoxConfigurationUpdate;
        internal System.Windows.Forms.Label labelTextInstallUpdates;
        internal System.Windows.Forms.ComboBox comboBoxConfigurationUpdateDate;
        internal System.Windows.Forms.Label labelTextAutomaticAt;
        internal System.Windows.Forms.ComboBox comboBoxAutomaticUpdateHour;
        internal System.Windows.Forms.Label labelConfigurationTitleAutomatic;
        internal System.Windows.Forms.Label labelConfigurationTitleSchedule;
        internal System.Windows.Forms.Label lineSeparator2;
        internal System.Windows.Forms.Label lineSeparator;
        internal System.Windows.Forms.Panel panelPlanificar;
        internal System.Windows.Forms.DateTimePicker dateTimePickerPlanificar;
        internal System.Windows.Forms.CheckBox checkBoxPlanificar;
        internal System.Windows.Forms.Label labelTextScheduleAt;
        internal System.Windows.Forms.ComboBox comboBoxScheduleUpdateHour;
        internal System.Windows.Forms.TabPage TabPage1;
        internal uControlNotificationState_NoConnection UControlNotificationState_NoConnection;
        internal uControlNotificationState_Warning UControlNotificationState_Warning;
        internal uControlNotificationState_Ok UControlNotificationState_Ok;
        internal System.Windows.Forms.Label labelTextLastUpdateValue;
        internal System.Windows.Forms.Label labelTextLastUpdate;
        internal uControlNotificationState_InitUpdateProcess UControlNotificationState_InitUpdateProcess;
        internal System.Windows.Forms.PictureBox pictureBoxAutomaticUpdates;
        internal System.Windows.Forms.LinkLabel linkLabelUpdateStations;
        internal System.Windows.Forms.TabPage TabPage3;
        internal System.Windows.Forms.Label labelStationUpdateTitleUpdateAvailable;
        internal System.Windows.Forms.Label labelStationUpdateTitleSoftwareVersion;
        internal System.Windows.Forms.Label labelStationUpdateTitleModel;
        internal System.Windows.Forms.Label labelStationUpdateTitleName;
        internal System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        internal System.Windows.Forms.Button butUpdateStations;
        internal System.Windows.Forms.CheckBox cbUpdateAllStations;
        internal System.Windows.Forms.PictureBox imgRefresh;
        internal System.Windows.Forms.ComboBox comboBoxConfigurationFilesLocation;
        internal System.Windows.Forms.Label labelConfigurationTitleFilesLocation;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox textBoxConfigurationFilesLocation;
        internal System.Windows.Forms.PictureBox imgConfigurationFilesLocation;
        internal System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1;
    }
}
