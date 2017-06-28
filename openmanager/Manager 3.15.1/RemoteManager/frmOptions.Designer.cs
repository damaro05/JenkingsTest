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
partial class frmOptions : System.Windows.Forms.Form
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
            System.Windows.Forms.TreeNode TreeNode1 = new System.Windows.Forms.TreeNode("International settings");
            System.Windows.Forms.TreeNode TreeNode2 = new System.Windows.Forms.TreeNode("Units");
            System.Windows.Forms.TreeNode TreeNode3 = new System.Windows.Forms.TreeNode("Notifications");
            System.Windows.Forms.TreeNode TreeNode4 = new System.Windows.Forms.TreeNode("Environment", new System.Windows.Forms.TreeNode[] { TreeNode1, TreeNode2, TreeNode3 });
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
            this.labelLanguages = new System.Windows.Forms.Label();
            base.Load += new System.EventHandler(frmOptions_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(frmOptions_Paint);
            this.comboBoxLanguages = new System.Windows.Forms.ComboBox();
            this.comboBoxLanguages.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguages_SelectedIndexChanged);
            this.labelTemperatureUnit = new System.Windows.Forms.Label();
            this.comboBoxTemperatureUnits = new System.Windows.Forms.ComboBox();
            this.comboBoxTemperatureUnits.SelectedIndexChanged += new System.EventHandler(this.comboBoxTemperatureUnits_SelectedIndexChanged);
            this.TreeView = new System.Windows.Forms.TreeView();
            this.TreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView_AfterSelect);
            this.TabControl = new System.Windows.Forms.TabControl();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.TabPage2 = new System.Windows.Forms.TabPage();
            this.TabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox_Notifications = new System.Windows.Forms.GroupBox();
            this.checkBox_ShowStationNotifications = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowStationNotifications.CheckedChanged += new System.EventHandler(this.checkBox_ShowStationNotifications_CheckedChanged);
            this.checkBox_ShowStationControllerNotifications = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowStationControllerNotifications.CheckedChanged += new System.EventHandler(this.checkBox_ShowStationControllerNotifications_CheckedChanged);
            this.checkBox_ShowErrorNotifications = new System.Windows.Forms.CheckBox();
            this.checkBox_ShowErrorNotifications.CheckedChanged += new System.EventHandler(this.checkBox_ShowErrorNotifications_CheckedChanged);
            this.butClose = new System.Windows.Forms.Button();
            this.butClose.Click += new System.EventHandler(this.butClose_Click);
            this.lineSeparator = new System.Windows.Forms.Label();
            this.TabControl.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.TabPage3.SuspendLayout();
            this.groupBox_Notifications.SuspendLayout();
            this.SuspendLayout();
            //
            //labelLanguages
            //
            this.labelLanguages.AutoSize = true;
            this.labelLanguages.Location = new System.Drawing.Point(6, 14);
            this.labelLanguages.Name = "labelLanguages";
            this.labelLanguages.Size = new System.Drawing.Size(70, 14);
            this.labelLanguages.TabIndex = 15;
            this.labelLanguages.Text = "Language";
            //
            //comboBoxLanguages
            //
            this.comboBoxLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguages.FormattingEnabled = true;
            this.comboBoxLanguages.Items.AddRange(new object[] { "Deutsch", "English", "Español", "日本語" });
            this.comboBoxLanguages.Location = new System.Drawing.Point(121, 11);
            this.comboBoxLanguages.Name = "comboBoxLanguages";
            this.comboBoxLanguages.Size = new System.Drawing.Size(173, 22);
            this.comboBoxLanguages.TabIndex = 15;
            //
            //labelTemperatureUnit
            //
            this.labelTemperatureUnit.AutoSize = true;
            this.labelTemperatureUnit.Location = new System.Drawing.Point(6, 14);
            this.labelTemperatureUnit.Name = "labelTemperatureUnit";
            this.labelTemperatureUnit.Size = new System.Drawing.Size(116, 14);
            this.labelTemperatureUnit.TabIndex = 16;
            this.labelTemperatureUnit.Text = "Temperature unit";
            //
            //comboBoxTemperatureUnits
            //
            this.comboBoxTemperatureUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTemperatureUnits.FormattingEnabled = true;
            this.comboBoxTemperatureUnits.Items.AddRange(new object[] { "Celsius", "Fahrenheit" });
            this.comboBoxTemperatureUnits.Location = new System.Drawing.Point(194, 11);
            this.comboBoxTemperatureUnits.Name = "comboBoxTemperatureUnits";
            this.comboBoxTemperatureUnits.Size = new System.Drawing.Size(173, 22);
            this.comboBoxTemperatureUnits.TabIndex = 17;
            //
            //TreeView
            //
            this.TreeView.BackColor = System.Drawing.SystemColors.Window;
            this.TreeView.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.TreeView.FullRowSelect = true;
            this.TreeView.HideSelection = false;
            this.TreeView.Location = new System.Drawing.Point(12, 12);
            this.TreeView.Name = "TreeView";
            TreeNode1.Name = "mnu_InternationalSettings";
            TreeNode1.Text = "International settings";
            TreeNode2.Name = "mnu_Units";
            TreeNode2.Text = "Units";
            TreeNode3.Name = "mnu_Notifications";
            TreeNode3.Text = "Notifications";
            TreeNode4.Name = "mnu_Environment";
            TreeNode4.Text = "Environment";
            this.TreeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { TreeNode4 });
            this.TreeView.ShowLines = false;
            this.TreeView.Size = new System.Drawing.Size(218, 346);
            this.TreeView.TabIndex = 27;
            //
            //TabControl
            //
            this.TabControl.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.TabControl.Controls.Add(this.TabPage1);
            this.TabControl.Controls.Add(this.TabPage2);
            this.TabControl.Controls.Add(this.TabPage3);
            this.TabControl.ItemSize = new System.Drawing.Size(0, 1);
            this.TabControl.Location = new System.Drawing.Point(250, 12);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(628, 336);
            this.TabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControl.TabIndex = 30;
            //
            //TabPage1
            //
            this.TabPage1.Controls.Add(this.labelLanguages);
            this.TabPage1.Controls.Add(this.comboBoxLanguages);
            this.TabPage1.Location = new System.Drawing.Point(4, 5);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage1.Size = new System.Drawing.Size(620, 327);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "TabPage1";
            this.TabPage1.UseVisualStyleBackColor = true;
            //
            //TabPage2
            //
            this.TabPage2.Controls.Add(this.comboBoxTemperatureUnits);
            this.TabPage2.Controls.Add(this.labelTemperatureUnit);
            this.TabPage2.Location = new System.Drawing.Point(4, 55);
            this.TabPage2.Name = "TabPage2";
            this.TabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage2.Size = new System.Drawing.Size(620, 277);
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "TabPage2";
            this.TabPage2.UseVisualStyleBackColor = true;
            //
            //TabPage3
            //
            this.TabPage3.Controls.Add(this.groupBox_Notifications);
            this.TabPage3.Location = new System.Drawing.Point(4, 55);
            this.TabPage3.Name = "TabPage3";
            this.TabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.TabPage3.Size = new System.Drawing.Size(620, 277);
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "TabPage3";
            this.TabPage3.UseVisualStyleBackColor = true;
            //
            //groupBox_Notifications
            //
            this.groupBox_Notifications.Controls.Add(this.checkBox_ShowStationNotifications);
            this.groupBox_Notifications.Controls.Add(this.checkBox_ShowStationControllerNotifications);
            this.groupBox_Notifications.Controls.Add(this.checkBox_ShowErrorNotifications);
            this.groupBox_Notifications.Location = new System.Drawing.Point(6, 15);
            this.groupBox_Notifications.Name = "groupBox_Notifications";
            this.groupBox_Notifications.Size = new System.Drawing.Size(443, 142);
            this.groupBox_Notifications.TabIndex = 0;
            this.groupBox_Notifications.TabStop = false;
            this.groupBox_Notifications.Text = "Notifications";
            //
            //checkBox_ShowStationNotifications
            //
            this.checkBox_ShowStationNotifications.AutoSize = true;
            this.checkBox_ShowStationNotifications.Location = new System.Drawing.Point(18, 102);
            this.checkBox_ShowStationNotifications.Name = "checkBox_ShowStationNotifications";
            this.checkBox_ShowStationNotifications.Size = new System.Drawing.Size(116, 18);
            this.checkBox_ShowStationNotifications.TabIndex = 25;
            this.checkBox_ShowStationNotifications.Text = "Show stations";
            this.checkBox_ShowStationNotifications.UseVisualStyleBackColor = true;
            //
            //checkBox_ShowStationControllerNotifications
            //
            this.checkBox_ShowStationControllerNotifications.AutoSize = true;
            this.checkBox_ShowStationControllerNotifications.Location = new System.Drawing.Point(18, 67);
            this.checkBox_ShowStationControllerNotifications.Name = "checkBox_ShowStationControllerNotifications";
            this.checkBox_ShowStationControllerNotifications.Size = new System.Drawing.Size(179, 18);
            this.checkBox_ShowStationControllerNotifications.TabIndex = 24;
            this.checkBox_ShowStationControllerNotifications.Text = "Show station controllers";
            this.checkBox_ShowStationControllerNotifications.UseVisualStyleBackColor = true;
            //
            //checkBox_ShowErrorNotifications
            //
            this.checkBox_ShowErrorNotifications.AutoSize = true;
            this.checkBox_ShowErrorNotifications.Location = new System.Drawing.Point(18, 32);
            this.checkBox_ShowErrorNotifications.Name = "checkBox_ShowErrorNotifications";
            this.checkBox_ShowErrorNotifications.Size = new System.Drawing.Size(103, 18);
            this.checkBox_ShowErrorNotifications.TabIndex = 23;
            this.checkBox_ShowErrorNotifications.Text = "Show errors";
            this.checkBox_ShowErrorNotifications.UseVisualStyleBackColor = true;
            //
            //butClose
            //
            this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butClose.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.butClose.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.butClose.Location = new System.Drawing.Point(758, 371);
            this.butClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(116, 32);
            this.butClose.TabIndex = 31;
            this.butClose.Text = "Cerrar";
            this.butClose.UseVisualStyleBackColor = true;
            //
            //lineSeparator
            //
            this.lineSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
            this.lineSeparator.Location = new System.Drawing.Point(250, 357);
            this.lineSeparator.Name = "lineSeparator";
            this.lineSeparator.Size = new System.Drawing.Size(628, 1);
            this.lineSeparator.TabIndex = 32;
            //
            //frmOptions
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(890, 416);
            this.Controls.Add(this.lineSeparator);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.TabControl);
            this.Controls.Add(this.TreeView);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.TabControl.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.TabPage1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.TabPage2.PerformLayout();
            this.TabPage3.ResumeLayout(false);
            this.groupBox_Notifications.ResumeLayout(false);
            this.groupBox_Notifications.PerformLayout();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Label labelLanguages;
        internal System.Windows.Forms.ComboBox comboBoxLanguages;
        internal System.Windows.Forms.Label labelTemperatureUnit;
        internal System.Windows.Forms.ComboBox comboBoxTemperatureUnits;
        internal System.Windows.Forms.TreeView TreeView;
        internal System.Windows.Forms.TabControl TabControl;
        internal System.Windows.Forms.TabPage TabPage1;
        internal System.Windows.Forms.TabPage TabPage2;
        internal System.Windows.Forms.TabPage TabPage3;
        internal System.Windows.Forms.GroupBox groupBox_Notifications;
        internal System.Windows.Forms.CheckBox checkBox_ShowStationNotifications;
        internal System.Windows.Forms.CheckBox checkBox_ShowStationControllerNotifications;
        internal System.Windows.Forms.CheckBox checkBox_ShowErrorNotifications;
        internal System.Windows.Forms.Button butClose;
        internal System.Windows.Forms.Label lineSeparator;
    }
}
