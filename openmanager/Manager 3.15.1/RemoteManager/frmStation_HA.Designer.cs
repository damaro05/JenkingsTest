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
partial class frmStation_HA : System.Windows.Forms.Form
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
            this.components = new System.ComponentModel.Container();
            base.Load += new System.EventHandler(frmStationParams_Load);
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmStation_FormClosing);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmStation_HA));
            this.cbMode = new System.Windows.Forms.CheckBox();
            this.cbMode.Click += new System.EventHandler(this.cbMode_Click);
            this.tlpTabs = new System.Windows.Forms.TableLayoutPanel();
            this.iconGraphics = new System.Windows.Forms.RadioButton();
            this.iconGraphics.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconGraphics.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconGraphics.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconInfo = new System.Windows.Forms.RadioButton();
            this.iconInfo.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconInfo.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconInfo.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconCounters = new System.Windows.Forms.RadioButton();
            this.iconCounters.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconCounters.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconCounters.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconResetSettings = new System.Windows.Forms.RadioButton();
            this.iconResetSettings.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconResetSettings.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconResetSettings.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconLoadSaveSettings = new System.Windows.Forms.RadioButton();
            this.iconLoadSaveSettings.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconLoadSaveSettings.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconLoadSaveSettings.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconPeripheral = new System.Windows.Forms.RadioButton();
            this.iconPeripheral.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconPeripheral.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconPeripheral.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconProfile = new System.Windows.Forms.RadioButton();
            this.iconProfile.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconToolSettings = new System.Windows.Forms.RadioButton();
            this.iconToolSettings.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconToolSettings.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconToolSettings.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconStationSettings = new System.Windows.Forms.RadioButton();
            this.iconStationSettings.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconStationSettings.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconStationSettings.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.iconWork = new System.Windows.Forms.RadioButton();
            this.iconWork.CheckedChanged += new System.EventHandler(this.icon_CheckedChanged);
            this.iconWork.MouseEnter += new System.EventHandler(this.icon_MouseEnter);
            this.iconWork.MouseLeave += new System.EventHandler(this.icon_MouseLeave);
            this.ToolTipIcons = new System.Windows.Forms.ToolTip(this.components);
            this.tlpTabs.SuspendLayout();
            this.SuspendLayout();
            //
            //cbMode
            //
            this.cbMode.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.cbMode.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMode.BackColor = System.Drawing.Color.Transparent;
            this.cbMode.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMode.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbMode.FlatAppearance.BorderSize = 0;
            this.cbMode.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.cbMode.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cbMode.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cbMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbMode.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(0)), System.Convert.ToInt32(System.Convert.ToByte(122)), System.Convert.ToInt32(System.Convert.ToByte(255)));
            this.cbMode.Image = My.Resources.Resources._lock;
            this.cbMode.Location = new System.Drawing.Point(492, -1);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(66, 57);
            this.cbMode.TabIndex = 33;
            this.cbMode.Text = "Monitor Mode";
            this.cbMode.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.cbMode.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.cbMode.UseVisualStyleBackColor = false;
            //
            //tlpTabs
            //
            this.tlpTabs.BackColor = System.Drawing.Color.Transparent;
            this.tlpTabs.ColumnCount = 10;
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(50.0F)));
            this.tlpTabs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(10.0F)));
            this.tlpTabs.Controls.Add(this.iconGraphics, 9, 0);
            this.tlpTabs.Controls.Add(this.iconInfo, 8, 0);
            this.tlpTabs.Controls.Add(this.iconCounters, 7, 0);
            this.tlpTabs.Controls.Add(this.iconResetSettings, 6, 0);
            this.tlpTabs.Controls.Add(this.iconLoadSaveSettings, 5, 0);
            this.tlpTabs.Controls.Add(this.iconPeripheral, 4, 0);
            this.tlpTabs.Controls.Add(this.iconProfile, 3, 0);
            this.tlpTabs.Controls.Add(this.iconToolSettings, 2, 0);
            this.tlpTabs.Controls.Add(this.iconStationSettings, 1, 0);
            this.tlpTabs.Controls.Add(this.iconWork, 0, 0);
            this.tlpTabs.Location = new System.Drawing.Point(1, 1);
            this.tlpTabs.Name = "tlpTabs";
            this.tlpTabs.RowCount = 1;
            this.tlpTabs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, (float)(58.0F)));
            this.tlpTabs.Size = new System.Drawing.Size(505, 57);
            this.tlpTabs.TabIndex = 36;
            //
            //iconGraphics
            //
            this.iconGraphics.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconGraphics.BackColor = System.Drawing.Color.Transparent;
            this.iconGraphics.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconGraphics.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconGraphics.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconGraphics.FlatAppearance.BorderSize = 0;
            this.iconGraphics.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconGraphics.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconGraphics.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconGraphics.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconGraphics.ForeColor = System.Drawing.Color.Transparent;
            this.iconGraphics.Image = My.Resources.Resources.iconGraphics;
            this.iconGraphics.Location = new System.Drawing.Point(450, 0);
            this.iconGraphics.Margin = new System.Windows.Forms.Padding(0);
            this.iconGraphics.Name = "iconGraphics";
            this.iconGraphics.Size = new System.Drawing.Size(43, 46);
            this.iconGraphics.TabIndex = 37;
            this.iconGraphics.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconGraphics.UseVisualStyleBackColor = false;
            //
            //iconInfo
            //
            this.iconInfo.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconInfo.BackColor = System.Drawing.Color.Transparent;
            this.iconInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconInfo.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconInfo.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconInfo.FlatAppearance.BorderSize = 0;
            this.iconInfo.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconInfo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconInfo.ForeColor = System.Drawing.Color.Transparent;
            this.iconInfo.Image = My.Resources.Resources.iconInfo;
            this.iconInfo.Location = new System.Drawing.Point(400, 0);
            this.iconInfo.Margin = new System.Windows.Forms.Padding(0);
            this.iconInfo.Name = "iconInfo";
            this.iconInfo.Size = new System.Drawing.Size(46, 46);
            this.iconInfo.TabIndex = 36;
            this.iconInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconInfo.UseVisualStyleBackColor = false;
            //
            //iconCounters
            //
            this.iconCounters.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconCounters.BackColor = System.Drawing.Color.Transparent;
            this.iconCounters.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconCounters.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconCounters.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconCounters.FlatAppearance.BorderSize = 0;
            this.iconCounters.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconCounters.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconCounters.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconCounters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconCounters.ForeColor = System.Drawing.Color.Transparent;
            this.iconCounters.Image = My.Resources.Resources.iconCounters;
            this.iconCounters.Location = new System.Drawing.Point(350, 0);
            this.iconCounters.Margin = new System.Windows.Forms.Padding(0);
            this.iconCounters.Name = "iconCounters";
            this.iconCounters.Size = new System.Drawing.Size(46, 46);
            this.iconCounters.TabIndex = 35;
            this.iconCounters.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconCounters.UseVisualStyleBackColor = false;
            //
            //iconResetSettings
            //
            this.iconResetSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconResetSettings.BackColor = System.Drawing.Color.Transparent;
            this.iconResetSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconResetSettings.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconResetSettings.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconResetSettings.FlatAppearance.BorderSize = 0;
            this.iconResetSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconResetSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconResetSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconResetSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconResetSettings.ForeColor = System.Drawing.Color.Transparent;
            this.iconResetSettings.Image = My.Resources.Resources.iconResetSettings;
            this.iconResetSettings.Location = new System.Drawing.Point(300, 0);
            this.iconResetSettings.Margin = new System.Windows.Forms.Padding(0);
            this.iconResetSettings.Name = "iconResetSettings";
            this.iconResetSettings.Size = new System.Drawing.Size(46, 46);
            this.iconResetSettings.TabIndex = 34;
            this.iconResetSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconResetSettings.UseVisualStyleBackColor = false;
            //
            //iconLoadSaveSettings
            //
            this.iconLoadSaveSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconLoadSaveSettings.BackColor = System.Drawing.Color.Transparent;
            this.iconLoadSaveSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconLoadSaveSettings.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconLoadSaveSettings.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconLoadSaveSettings.FlatAppearance.BorderSize = 0;
            this.iconLoadSaveSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconLoadSaveSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconLoadSaveSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconLoadSaveSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconLoadSaveSettings.ForeColor = System.Drawing.Color.Transparent;
            this.iconLoadSaveSettings.Image = My.Resources.Resources.iconLoadSaveSettings;
            this.iconLoadSaveSettings.Location = new System.Drawing.Point(250, 0);
            this.iconLoadSaveSettings.Margin = new System.Windows.Forms.Padding(0);
            this.iconLoadSaveSettings.Name = "iconLoadSaveSettings";
            this.iconLoadSaveSettings.Size = new System.Drawing.Size(46, 46);
            this.iconLoadSaveSettings.TabIndex = 32;
            this.iconLoadSaveSettings.Tag = "";
            this.iconLoadSaveSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconLoadSaveSettings.UseVisualStyleBackColor = false;
            //
            //iconPeripheral
            //
            this.iconPeripheral.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconPeripheral.BackColor = System.Drawing.Color.Transparent;
            this.iconPeripheral.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconPeripheral.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconPeripheral.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconPeripheral.FlatAppearance.BorderSize = 0;
            this.iconPeripheral.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconPeripheral.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconPeripheral.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconPeripheral.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconPeripheral.ForeColor = System.Drawing.Color.Transparent;
            this.iconPeripheral.Image = My.Resources.Resources.iconProfile;
            this.iconPeripheral.Location = new System.Drawing.Point(200, 0);
            this.iconPeripheral.Margin = new System.Windows.Forms.Padding(0);
            this.iconPeripheral.Name = "iconPeripheral";
            this.iconPeripheral.Size = new System.Drawing.Size(46, 46);
            this.iconPeripheral.TabIndex = 37;
            this.iconPeripheral.Tag = "";
            this.iconPeripheral.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconPeripheral.UseVisualStyleBackColor = false;
            //
            //iconProfile
            //
            this.iconProfile.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconProfile.BackColor = System.Drawing.Color.Transparent;
            this.iconProfile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconProfile.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconProfile.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconProfile.FlatAppearance.BorderSize = 0;
            this.iconProfile.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconProfile.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconProfile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconProfile.ForeColor = System.Drawing.Color.Transparent;
            this.iconProfile.Image = My.Resources.Resources.iconProfile;
            this.iconProfile.Location = new System.Drawing.Point(150, 0);
            this.iconProfile.Margin = new System.Windows.Forms.Padding(0);
            this.iconProfile.Name = "iconProfile";
            this.iconProfile.Size = new System.Drawing.Size(46, 46);
            this.iconProfile.TabIndex = 38;
            this.iconProfile.Tag = "";
            this.iconProfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconProfile.UseVisualStyleBackColor = false;
            //
            //iconToolSettings
            //
            this.iconToolSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconToolSettings.BackColor = System.Drawing.Color.Transparent;
            this.iconToolSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconToolSettings.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconToolSettings.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconToolSettings.FlatAppearance.BorderSize = 0;
            this.iconToolSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconToolSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconToolSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconToolSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconToolSettings.ForeColor = System.Drawing.Color.Transparent;
            this.iconToolSettings.Image = My.Resources.Resources.iconToolSettings;
            this.iconToolSettings.Location = new System.Drawing.Point(100, 0);
            this.iconToolSettings.Margin = new System.Windows.Forms.Padding(0);
            this.iconToolSettings.Name = "iconToolSettings";
            this.iconToolSettings.Size = new System.Drawing.Size(46, 46);
            this.iconToolSettings.TabIndex = 31;
            this.iconToolSettings.Tag = "";
            this.iconToolSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconToolSettings.UseVisualStyleBackColor = false;
            //
            //iconStationSettings
            //
            this.iconStationSettings.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconStationSettings.BackColor = System.Drawing.Color.Transparent;
            this.iconStationSettings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.iconStationSettings.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconStationSettings.FlatAppearance.BorderColor = System.Drawing.Color.CornflowerBlue;
            this.iconStationSettings.FlatAppearance.BorderSize = 0;
            this.iconStationSettings.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconStationSettings.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconStationSettings.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconStationSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconStationSettings.ForeColor = System.Drawing.Color.Transparent;
            this.iconStationSettings.Image = My.Resources.Resources.iconStationSettings;
            this.iconStationSettings.Location = new System.Drawing.Point(50, 0);
            this.iconStationSettings.Margin = new System.Windows.Forms.Padding(0);
            this.iconStationSettings.Name = "iconStationSettings";
            this.iconStationSettings.Size = new System.Drawing.Size(46, 46);
            this.iconStationSettings.TabIndex = 33;
            this.iconStationSettings.Tag = "";
            this.iconStationSettings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconStationSettings.UseVisualStyleBackColor = false;
            //
            //iconWork
            //
            this.iconWork.Appearance = System.Windows.Forms.Appearance.Button;
            this.iconWork.BackColor = System.Drawing.Color.Transparent;
            this.iconWork.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.iconWork.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconWork.FlatAppearance.BorderSize = 0;
            this.iconWork.FlatAppearance.CheckedBackColor = System.Drawing.Color.Transparent;
            this.iconWork.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.iconWork.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.iconWork.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.iconWork.ForeColor = System.Drawing.Color.Transparent;
            this.iconWork.Image = My.Resources.Resources.iconWork;
            this.iconWork.Location = new System.Drawing.Point(0, 0);
            this.iconWork.Margin = new System.Windows.Forms.Padding(0);
            this.iconWork.Name = "iconWork";
            this.iconWork.Size = new System.Drawing.Size(46, 46);
            this.iconWork.TabIndex = 30;
            this.iconWork.Tag = "";
            this.iconWork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.iconWork.UseVisualStyleBackColor = false;
            //
            //frmStation_HA
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(206)), System.Convert.ToInt32(System.Convert.ToByte(210)), System.Convert.ToInt32(System.Convert.ToByte(213)));
            this.ClientSize = new System.Drawing.Size(566, 378);
            this.Controls.Add(this.cbMode);
            this.Controls.Add(this.tlpTabs);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "frmStation_HA";
            this.ShowIcon = false;
            this.Text = "frmStationParams";
            this.tlpTabs.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.CheckBox cbMode;
        internal System.Windows.Forms.TableLayoutPanel tlpTabs;
        internal System.Windows.Forms.RadioButton iconResetSettings;
        internal System.Windows.Forms.RadioButton iconCounters;
        internal System.Windows.Forms.RadioButton iconLoadSaveSettings;
        internal System.Windows.Forms.RadioButton iconWork;
        internal System.Windows.Forms.RadioButton iconToolSettings;
        internal System.Windows.Forms.RadioButton iconStationSettings;
        internal System.Windows.Forms.RadioButton iconInfo;
        internal System.Windows.Forms.ToolTip ToolTipIcons;
        internal System.Windows.Forms.RadioButton iconGraphics;
        internal System.Windows.Forms.RadioButton iconPeripheral;
        internal System.Windows.Forms.RadioButton iconProfile;
    }
}
