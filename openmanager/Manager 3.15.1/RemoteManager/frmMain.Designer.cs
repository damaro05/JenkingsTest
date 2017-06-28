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
partial class frmMain : System.Windows.Forms.Form
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

        //Requerido por el DiseÃ±ador de Windows Forms
        private System.ComponentModel.Container components = null;

        //NOTA: el DiseÃ±ador de Windows Forms necesita el siguiente procedimiento
        //Se puede modificar usando el DiseÃ±ador de Windows Forms.
        //No lo modifique con el editor de cÃ³digo.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Load += new System.EventHandler(frmMain_Load);
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmMain_FormClosing);
            base.GotFocus += new System.EventHandler(frmMain_GotFocus);
            base.Resize += new System.EventHandler(frmMain_Resize);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mspMain = new System.Windows.Forms.MenuStrip();
            this.mnuSupervisor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoginSupervisor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoginSupervisor.Click += new System.EventHandler(this.mnuSupervisor_Click);
            this.mnuLogoutSupervisor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogoutSupervisor.Click += new System.EventHandler(this.mnuSupervisor_Click);
            this.mnuChangePasswordSupervisor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChangePasswordSupervisor.Click += new System.EventHandler(this.mnuSupervisor_Click);
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView.Click += new System.EventHandler(this.ViewToolStripMenuItem_Click);
            this.butViewStationList = new System.Windows.Forms.ToolStripMenuItem();
            this.butViewStationList.Click += new System.EventHandler(this.butViewStationList_Click);
            this.butViewEvents = new System.Windows.Forms.ToolStripMenuItem();
            this.butViewEvents.Click += new System.EventHandler(this.mnuEvents_Click);
            this.mnuWarning = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWarning.Click += new System.EventHandler(this.mnuWarning_Click);
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsManager = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettingsManager.Click += new System.EventHandler(this.mnuSettingsManager_Click);
            this.mnuRegisterManager = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRegisterManager.Click += new System.EventHandler(this.mnuRegisterManager_Click);
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptions.Click += new System.EventHandler(this.mnuOptions_Click);
            this.mnuWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCascade = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCascade.Click += new System.EventHandler(this.mnuLayout_Click);
            this.mnuCascadeAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCascadeAll.Click += new System.EventHandler(this.mnuLayout_Click);
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoteManagerHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoteManagerHelp.Click += new System.EventHandler(this.mnuRemoteManagerHelp_Click);
            this.mnuUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUpdates.Click += new System.EventHandler(this.mnuUpdates_Click);
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            this.stsStatusData = new System.Windows.Forms.StatusStrip();
            this.lblStatusStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.imgTreeNodes = new System.Windows.Forms.ImageList(this.components);
            this.timerApplStartUp = new System.Windows.Forms.Timer(this.components);
            this.timerApplStartUp.Tick += new System.EventHandler(this.timerApplStartUp_Tick);
            this.timerStationListData = new System.Windows.Forms.Timer(this.components);
            this.timerStationListData.Tick += new System.EventHandler(this.timerStationListData_Tick);
            this.mspMain.SuspendLayout();
            this.stsStatusData.SuspendLayout();
            this.SuspendLayout();
            //
            //mspMain
            //
            this.mspMain.BackColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(247)), System.Convert.ToInt32(System.Convert.ToByte(247)), System.Convert.ToInt32(System.Convert.ToByte(247)));
            this.mspMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuSupervisor, this.mnuView, this.mnuTools, this.mnuWindows, this.mnuHelp });
            this.mspMain.Location = new System.Drawing.Point(0, 0);
            this.mspMain.Name = "mspMain";
            this.mspMain.Size = new System.Drawing.Size(632, 24);
            this.mspMain.TabIndex = 1;
            this.mspMain.Text = "MenuStrip1";
            //
            //mnuSupervisor
            //
            this.mnuSupervisor.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuLoginSupervisor, this.mnuLogoutSupervisor, this.mnuChangePasswordSupervisor });
            this.mnuSupervisor.Name = "mnuSupervisor";
            this.mnuSupervisor.Size = new System.Drawing.Size(74, 20);
            this.mnuSupervisor.Text = "Supervisor";
            //
            //mnuLoginSupervisor
            //
            this.mnuLoginSupervisor.Name = "mnuLoginSupervisor";
            this.mnuLoginSupervisor.Size = new System.Drawing.Size(168, 22);
            this.mnuLoginSupervisor.Text = "Login";
            //
            //mnuLogoutSupervisor
            //
            this.mnuLogoutSupervisor.Name = "mnuLogoutSupervisor";
            this.mnuLogoutSupervisor.Size = new System.Drawing.Size(168, 22);
            this.mnuLogoutSupervisor.Text = "Logout";
            //
            //mnuChangePasswordSupervisor
            //
            this.mnuChangePasswordSupervisor.Name = "mnuChangePasswordSupervisor";
            this.mnuChangePasswordSupervisor.Size = new System.Drawing.Size(168, 22);
            this.mnuChangePasswordSupervisor.Text = "Change password";
            //
            //mnuView
            //
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.butViewStationList, this.butViewEvents, this.mnuWarning });
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(44, 20);
            this.mnuView.Text = "View";
            //
            //butViewStationList
            //
            this.butViewStationList.Checked = true;
            this.butViewStationList.CheckOnClick = true;
            this.butViewStationList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.butViewStationList.Name = "butViewStationList";
            this.butViewStationList.ShortcutKeyDisplayString = "";
            this.butViewStationList.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S);
            this.butViewStationList.Size = new System.Drawing.Size(193, 22);
            this.butViewStationList.Text = "Station list";
            //
            //butViewEvents
            //
            this.butViewEvents.CheckOnClick = true;
            this.butViewEvents.Name = "butViewEvents";
            this.butViewEvents.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E);
            this.butViewEvents.Size = new System.Drawing.Size(193, 22);
            this.butViewEvents.Text = "Events window";
            //
            //mnuWarning
            //
            this.mnuWarning.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.mnuWarning.Image = My.Resources.Resources.warning_icon;
            this.mnuWarning.Name = "mnuWarning";
            this.mnuWarning.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N);
            this.mnuWarning.Size = new System.Drawing.Size(193, 22);
            this.mnuWarning.Text = "Notifications";
            //
            //mnuTools
            //
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuSettingsManager, this.mnuRegisterManager, this.ToolStripSeparator1, this.mnuOptions });
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M);
            this.mnuTools.Size = new System.Drawing.Size(48, 20);
            this.mnuTools.Text = "Tools";
            //
            //mnuSettingsManager
            //
            this.mnuSettingsManager.Name = "mnuSettingsManager";
            this.mnuSettingsManager.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M);
            this.mnuSettingsManager.Size = new System.Drawing.Size(211, 22);
            this.mnuSettingsManager.Text = "Settings manager";
            //
            //mnuRegisterManager
            //
            this.mnuRegisterManager.Name = "mnuRegisterManager";
            this.mnuRegisterManager.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G);
            this.mnuRegisterManager.Size = new System.Drawing.Size(211, 22);
            this.mnuRegisterManager.Text = "Graphics";
            //
            //ToolStripSeparator1
            //
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(208, 6);
            //
            //mnuOptions
            //
            this.mnuOptions.Image = My.Resources.Resources.options_selected;
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.ShowShortcutKeys = false;
            this.mnuOptions.Size = new System.Drawing.Size(211, 22);
            this.mnuOptions.Text = "Options";
            //
            //mnuWindows
            //
            this.mnuWindows.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuCascade, this.mnuCascadeAll });
            this.mnuWindows.Name = "mnuWindows";
            this.mnuWindows.Size = new System.Drawing.Size(68, 20);
            this.mnuWindows.Text = "Windows";
            //
            //mnuCascade
            //
            this.mnuCascade.BackColor = System.Drawing.SystemColors.Control;
            this.mnuCascade.Name = "mnuCascade";
            this.mnuCascade.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C);
            this.mnuCascade.Size = new System.Drawing.Size(221, 22);
            this.mnuCascade.Text = "Cascade";
            //
            //mnuCascadeAll
            //
            this.mnuCascadeAll.Name = "mnuCascadeAll";
            this.mnuCascadeAll.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A);
            this.mnuCascadeAll.Size = new System.Drawing.Size(221, 22);
            this.mnuCascadeAll.Text = "Cascade all windows";
            //
            //mnuHelp
            //
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuRemoteManagerHelp, this.mnuUpdates, this.ToolStripSeparator2, this.mnuAbout });
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "Help";
            //
            //mnuRemoteManagerHelp
            //
            this.mnuRemoteManagerHelp.Name = "mnuRemoteManagerHelp";
            this.mnuRemoteManagerHelp.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.mnuRemoteManagerHelp.Size = new System.Drawing.Size(210, 22);
            this.mnuRemoteManagerHelp.Text = "Remote Manager help";
            //
            //mnuUpdates
            //
            this.mnuUpdates.Name = "mnuUpdates";
            this.mnuUpdates.ShowShortcutKeys = false;
            this.mnuUpdates.Size = new System.Drawing.Size(210, 22);
            this.mnuUpdates.Text = "Updates";
            //
            //ToolStripSeparator2
            //
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(207, 6);
            //
            //mnuAbout
            //
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(210, 22);
            this.mnuAbout.Text = "About Remote Manager";
            //
            //stsStatusData
            //
            this.stsStatusData.BackColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(247)), System.Convert.ToInt32(System.Convert.ToByte(247)), System.Convert.ToInt32(System.Convert.ToByte(247)));
            this.stsStatusData.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatusStatus });
            this.stsStatusData.Location = new System.Drawing.Point(0, 424);
            this.stsStatusData.Name = "stsStatusData";
            this.stsStatusData.Size = new System.Drawing.Size(632, 22);
            this.stsStatusData.TabIndex = 5;
            this.stsStatusData.Text = "Status";
            //
            //lblStatusStatus
            //
            this.lblStatusStatus.Image = My.Resources.Resources._off;
            this.lblStatusStatus.Name = "lblStatusStatus";
            this.lblStatusStatus.Size = new System.Drawing.Size(55, 17);
            this.lblStatusStatus.Text = "Status";
            //
            //imgTreeNodes
            //
            this.imgTreeNodes.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgTreeNodes.ImageStream"));
            this.imgTreeNodes.TransparentColor = System.Drawing.Color.Transparent;
            this.imgTreeNodes.Images.SetKeyName(0, "TreeNodeExpanded.png");
            this.imgTreeNodes.Images.SetKeyName(1, "TreeNodeCollapsed.png");
            this.imgTreeNodes.Images.SetKeyName(2, "TreeNodeData.png");
            //
            //timerApplStartUp
            //
            this.timerApplStartUp.Interval = 5000;
            //
            //timerStationListData
            //
            this.timerStationListData.Interval = 1000;
            //
            //frmMain
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.stsStatusData);
            this.Controls.Add(this.mspMain);
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.mspMain;
            this.MinimumSize = new System.Drawing.Size(640, 480);
            this.Name = "frmMain";
            this.Text = "JBC - Remote Manager";
            this.mspMain.ResumeLayout(false);
            this.mspMain.PerformLayout();
            this.stsStatusData.ResumeLayout(false);
            this.stsStatusData.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.MenuStrip mspMain;
        internal System.Windows.Forms.ToolStripMenuItem mnuView;
        internal System.Windows.Forms.ToolStripMenuItem butViewStationList;
        internal System.Windows.Forms.StatusStrip stsStatusData;
        internal System.Windows.Forms.ToolStripStatusLabel lblStatusStatus;
        internal System.Windows.Forms.ImageList imgTreeNodes;
        internal System.Windows.Forms.ToolStripMenuItem mnuWindows;
        internal System.Windows.Forms.ToolStripMenuItem mnuCascade;
        internal System.Windows.Forms.ToolStripMenuItem mnuSupervisor;
        internal System.Windows.Forms.ToolStripMenuItem mnuLoginSupervisor;
        internal System.Windows.Forms.ToolStripMenuItem mnuLogoutSupervisor;
        internal System.Windows.Forms.ToolStripMenuItem mnuChangePasswordSupervisor;
        internal System.Windows.Forms.ToolStripMenuItem mnuTools;
        internal System.Windows.Forms.ToolStripMenuItem mnuSettingsManager;
        internal System.Windows.Forms.Timer timerApplStartUp;
        internal System.Windows.Forms.ToolStripMenuItem mnuCascadeAll;
        internal System.Windows.Forms.ToolStripMenuItem mnuRegisterManager;
        internal System.Windows.Forms.Timer timerStationListData;
        internal System.Windows.Forms.ToolStripMenuItem butViewEvents;
        internal System.Windows.Forms.ToolStripMenuItem mnuHelp;
        internal System.Windows.Forms.ToolStripMenuItem mnuUpdates;
        internal System.Windows.Forms.ToolStripMenuItem mnuAbout;
        internal System.Windows.Forms.ToolStripMenuItem mnuWarning;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem mnuOptions;
        internal System.Windows.Forms.ToolStripMenuItem mnuRemoteManagerHelp;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;

    }
}
