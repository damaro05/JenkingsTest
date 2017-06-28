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
partial class ProfileParameters : System.Windows.Forms.UserControl
    {

        //UserControl overrides dispose to clean up the component list.
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
            this.pageProfilesParameters = new System.Windows.Forms.Panel();
            this.labelProfileDuration = new System.Windows.Forms.Label();
            this.labelProfileMode = new System.Windows.Forms.Label();
            this.labelProfileName = new System.Windows.Forms.Label();
            this.labelProfileDurationTitle = new System.Windows.Forms.Label();
            this.labelProfileModeTitle = new System.Windows.Forms.Label();
            this.labelProfileNameTitle = new System.Windows.Forms.Label();
            this.button_right = new System.Windows.Forms.Button();
            this.button_right.Click += new System.EventHandler(this.button_right_Click);
            this.button_left = new System.Windows.Forms.Button();
            this.button_left.Click += new System.EventHandler(this.button_left_Click);
            this.labelProfileTotal = new System.Windows.Forms.Label();
            this.labelProfileSplit = new System.Windows.Forms.Label();
            this.labelProfileSelected = new System.Windows.Forms.Label();
            this.pageProfilesParametersEdit = new System.Windows.Forms.Panel();
            this.labelProfileNameEdit = new System.Windows.Forms.Label();
            this.butTimePlus = new RoutinesLibrary.UI.RL_Button();
            this.butTimePlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butTimePlus_Click);
            this.butTimePlus.HoldOn += new RoutinesLibrary.UI.RL_Button.HoldOnEventHandler(this.butTimePlus_Click);
            this.butTimeMinus = new RoutinesLibrary.UI.RL_Button();
            this.butTimeMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butTimeMinus_Click);
            this.butTimeMinus.HoldOn += new RoutinesLibrary.UI.RL_Button.HoldOnEventHandler(this.butTimeMinus_Click);
            this.butAirFlowPlus = new RoutinesLibrary.UI.RL_Button();
            this.butAirFlowPlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butAirFlowPlus_Click);
            this.butAirFlowPlus.HoldOn += new RoutinesLibrary.UI.RL_Button.HoldOnEventHandler(this.butAirFlowPlus_Click);
            this.butAirFlowMinus = new RoutinesLibrary.UI.RL_Button();
            this.butAirFlowMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butAirFlowMinus_Click);
            this.butAirFlowMinus.HoldOn += new RoutinesLibrary.UI.RL_Button.HoldOnEventHandler(this.butAirFlowMinus_Click);
            this.butTemperaturePlus = new RoutinesLibrary.UI.RL_Button();
            this.butTemperaturePlus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butTemperaturePlus_Click);
            this.butTemperaturePlus.HoldOn += new RoutinesLibrary.UI.RL_Button.HoldOnEventHandler(this.butTemperaturePlus_Click);
            this.butTemperatureMinus = new RoutinesLibrary.UI.RL_Button();
            this.butTemperatureMinus.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butTemperatureMinus_Click);
            this.butTemperatureMinus.HoldOn += new RoutinesLibrary.UI.RL_Button.HoldOnEventHandler(this.butTemperatureMinus_Click);
            this.labelPoint = new System.Windows.Forms.Label();
            this.labelTime = new System.Windows.Forms.Label();
            this.labelAirFlow = new System.Windows.Forms.Label();
            this.labelTemperature = new System.Windows.Forms.Label();
            this.labelTimeTitle = new System.Windows.Forms.Label();
            this.labelAirFlowTitle = new System.Windows.Forms.Label();
            this.labelTemperatureTitle = new System.Windows.Forms.Label();
            this.button_rightEdit = new System.Windows.Forms.Button();
            this.button_rightEdit.Click += new System.EventHandler(this.button_rightEdit_Click);
            this.button_leftEdit = new System.Windows.Forms.Button();
            this.button_leftEdit.Click += new System.EventHandler(this.button_leftEdit_Click);
            this.labelPointTotal = new System.Windows.Forms.Label();
            this.labelProfileSplitEdit = new System.Windows.Forms.Label();
            this.labelPointSelected = new System.Windows.Forms.Label();
            this.pageProfilesParameters.SuspendLayout();
            this.pageProfilesParametersEdit.SuspendLayout();
            this.SuspendLayout();
            //
            //pageProfilesParameters
            //
            this.pageProfilesParameters.BackColor = System.Drawing.Color.Transparent;
            this.pageProfilesParameters.Controls.Add(this.labelProfileDuration);
            this.pageProfilesParameters.Controls.Add(this.labelProfileMode);
            this.pageProfilesParameters.Controls.Add(this.labelProfileName);
            this.pageProfilesParameters.Controls.Add(this.labelProfileDurationTitle);
            this.pageProfilesParameters.Controls.Add(this.labelProfileModeTitle);
            this.pageProfilesParameters.Controls.Add(this.labelProfileNameTitle);
            this.pageProfilesParameters.Controls.Add(this.button_right);
            this.pageProfilesParameters.Controls.Add(this.button_left);
            this.pageProfilesParameters.Controls.Add(this.labelProfileTotal);
            this.pageProfilesParameters.Controls.Add(this.labelProfileSplit);
            this.pageProfilesParameters.Controls.Add(this.labelProfileSelected);
            this.pageProfilesParameters.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.pageProfilesParameters.Location = new System.Drawing.Point(0, 0);
            this.pageProfilesParameters.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pageProfilesParameters.Name = "pageProfilesParameters";
            this.pageProfilesParameters.Size = new System.Drawing.Size(506, 92);
            this.pageProfilesParameters.TabIndex = 47;
            //
            //labelProfileDuration
            //
            this.labelProfileDuration.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileDuration.Location = new System.Drawing.Point(341, 66);
            this.labelProfileDuration.Name = "labelProfileDuration";
            this.labelProfileDuration.Size = new System.Drawing.Size(110, 15);
            this.labelProfileDuration.TabIndex = 10;
            this.labelProfileDuration.Text = "---";
            //
            //labelProfileMode
            //
            this.labelProfileMode.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileMode.Location = new System.Drawing.Point(341, 39);
            this.labelProfileMode.Name = "labelProfileMode";
            this.labelProfileMode.Size = new System.Drawing.Size(110, 15);
            this.labelProfileMode.TabIndex = 9;
            this.labelProfileMode.Text = "---";
            //
            //labelProfileName
            //
            this.labelProfileName.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileName.Location = new System.Drawing.Point(341, 12);
            this.labelProfileName.Name = "labelProfileName";
            this.labelProfileName.Size = new System.Drawing.Size(110, 15);
            this.labelProfileName.TabIndex = 8;
            this.labelProfileName.Text = "---";
            //
            //labelProfileDurationTitle
            //
            this.labelProfileDurationTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileDurationTitle.Location = new System.Drawing.Point(215, 66);
            this.labelProfileDurationTitle.Name = "labelProfileDurationTitle";
            this.labelProfileDurationTitle.Size = new System.Drawing.Size(110, 15);
            this.labelProfileDurationTitle.TabIndex = 7;
            this.labelProfileDurationTitle.Text = "Duration";
            //
            //labelProfileModeTitle
            //
            this.labelProfileModeTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileModeTitle.Location = new System.Drawing.Point(215, 39);
            this.labelProfileModeTitle.Name = "labelProfileModeTitle";
            this.labelProfileModeTitle.Size = new System.Drawing.Size(110, 15);
            this.labelProfileModeTitle.TabIndex = 6;
            this.labelProfileModeTitle.Text = "Mode";
            //
            //labelProfileNameTitle
            //
            this.labelProfileNameTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileNameTitle.Location = new System.Drawing.Point(215, 12);
            this.labelProfileNameTitle.Name = "labelProfileNameTitle";
            this.labelProfileNameTitle.Size = new System.Drawing.Size(110, 15);
            this.labelProfileNameTitle.TabIndex = 5;
            this.labelProfileNameTitle.Text = "Profile name";
            //
            //button_right
            //
            this.button_right.BackColor = System.Drawing.Color.Transparent;
            this.button_right.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_right.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_right.FlatAppearance.BorderSize = 0;
            this.button_right.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_right.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_right.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_right.Image = My.Resources.Resources.arrow_right;
            this.button_right.Location = new System.Drawing.Point(98, 66);
            this.button_right.Margin = new System.Windows.Forms.Padding(0);
            this.button_right.Name = "button_right";
            this.button_right.Size = new System.Drawing.Size(15, 15);
            this.button_right.TabIndex = 4;
            this.button_right.UseVisualStyleBackColor = false;
            //
            //button_left
            //
            this.button_left.BackColor = System.Drawing.Color.Transparent;
            this.button_left.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_left.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_left.FlatAppearance.BorderSize = 0;
            this.button_left.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_left.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_left.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_left.Image = My.Resources.Resources.arrow_left;
            this.button_left.Location = new System.Drawing.Point(25, 66);
            this.button_left.Margin = new System.Windows.Forms.Padding(0);
            this.button_left.Name = "button_left";
            this.button_left.Size = new System.Drawing.Size(15, 15);
            this.button_left.TabIndex = 3;
            this.button_left.UseVisualStyleBackColor = false;
            //
            //labelProfileTotal
            //
            this.labelProfileTotal.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileTotal.Location = new System.Drawing.Point(73, 66);
            this.labelProfileTotal.Margin = new System.Windows.Forms.Padding(0);
            this.labelProfileTotal.Name = "labelProfileTotal";
            this.labelProfileTotal.Size = new System.Drawing.Size(25, 15);
            this.labelProfileTotal.TabIndex = 2;
            this.labelProfileTotal.Text = "1";
            this.labelProfileTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //labelProfileSplit
            //
            this.labelProfileSplit.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileSplit.Location = new System.Drawing.Point(61, 66);
            this.labelProfileSplit.Margin = new System.Windows.Forms.Padding(0);
            this.labelProfileSplit.Name = "labelProfileSplit";
            this.labelProfileSplit.Size = new System.Drawing.Size(15, 15);
            this.labelProfileSplit.TabIndex = 1;
            this.labelProfileSplit.Text = "/";
            this.labelProfileSplit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //labelProfileSelected
            //
            this.labelProfileSelected.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileSelected.Location = new System.Drawing.Point(40, 66);
            this.labelProfileSelected.Margin = new System.Windows.Forms.Padding(0);
            this.labelProfileSelected.Name = "labelProfileSelected";
            this.labelProfileSelected.Size = new System.Drawing.Size(25, 15);
            this.labelProfileSelected.TabIndex = 0;
            this.labelProfileSelected.Text = "1";
            this.labelProfileSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //pageProfilesParametersEdit
            //
            this.pageProfilesParametersEdit.BackColor = System.Drawing.Color.Transparent;
            this.pageProfilesParametersEdit.Controls.Add(this.labelProfileNameEdit);
            this.pageProfilesParametersEdit.Controls.Add(this.butTimePlus);
            this.pageProfilesParametersEdit.Controls.Add(this.butTimeMinus);
            this.pageProfilesParametersEdit.Controls.Add(this.butAirFlowPlus);
            this.pageProfilesParametersEdit.Controls.Add(this.butAirFlowMinus);
            this.pageProfilesParametersEdit.Controls.Add(this.butTemperaturePlus);
            this.pageProfilesParametersEdit.Controls.Add(this.butTemperatureMinus);
            this.pageProfilesParametersEdit.Controls.Add(this.labelPoint);
            this.pageProfilesParametersEdit.Controls.Add(this.labelTime);
            this.pageProfilesParametersEdit.Controls.Add(this.labelAirFlow);
            this.pageProfilesParametersEdit.Controls.Add(this.labelTemperature);
            this.pageProfilesParametersEdit.Controls.Add(this.labelTimeTitle);
            this.pageProfilesParametersEdit.Controls.Add(this.labelAirFlowTitle);
            this.pageProfilesParametersEdit.Controls.Add(this.labelTemperatureTitle);
            this.pageProfilesParametersEdit.Controls.Add(this.button_rightEdit);
            this.pageProfilesParametersEdit.Controls.Add(this.button_leftEdit);
            this.pageProfilesParametersEdit.Controls.Add(this.labelPointTotal);
            this.pageProfilesParametersEdit.Controls.Add(this.labelProfileSplitEdit);
            this.pageProfilesParametersEdit.Controls.Add(this.labelPointSelected);
            this.pageProfilesParametersEdit.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.pageProfilesParametersEdit.Location = new System.Drawing.Point(0, 120);
            this.pageProfilesParametersEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pageProfilesParametersEdit.Name = "pageProfilesParametersEdit";
            this.pageProfilesParametersEdit.Size = new System.Drawing.Size(506, 92);
            this.pageProfilesParametersEdit.TabIndex = 48;
            //
            //labelProfileNameEdit
            //
            this.labelProfileNameEdit.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.labelProfileNameEdit.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileNameEdit.Location = new System.Drawing.Point(22, 12);
            this.labelProfileNameEdit.Name = "labelProfileNameEdit";
            this.labelProfileNameEdit.Size = new System.Drawing.Size(187, 15);
            this.labelProfileNameEdit.TabIndex = 24;
            this.labelProfileNameEdit.Text = "---";
            //
            //butTimePlus
            //
            this.butTimePlus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.butTimePlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butTimePlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butTimePlus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butTimePlus.Location = new System.Drawing.Point(464, 61);
            this.butTimePlus.Margin = new System.Windows.Forms.Padding(0);
            this.butTimePlus.Name = "butTimePlus";
            this.butTimePlus.Size = new System.Drawing.Size(34, 25);
            this.butTimePlus.TabIndex = 23;
            this.butTimePlus.Text = "+1";
            this.butTimePlus.UseVisualStyleBackColor = false;
            //
            //butTimeMinus
            //
            this.butTimeMinus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.butTimeMinus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butTimeMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butTimeMinus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butTimeMinus.Location = new System.Drawing.Point(422, 61);
            this.butTimeMinus.Margin = new System.Windows.Forms.Padding(0);
            this.butTimeMinus.Name = "butTimeMinus";
            this.butTimeMinus.Size = new System.Drawing.Size(34, 25);
            this.butTimeMinus.TabIndex = 22;
            this.butTimeMinus.Text = "-1";
            this.butTimeMinus.UseVisualStyleBackColor = false;
            //
            //butAirFlowPlus
            //
            this.butAirFlowPlus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.butAirFlowPlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butAirFlowPlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butAirFlowPlus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butAirFlowPlus.Location = new System.Drawing.Point(464, 34);
            this.butAirFlowPlus.Margin = new System.Windows.Forms.Padding(0);
            this.butAirFlowPlus.Name = "butAirFlowPlus";
            this.butAirFlowPlus.Size = new System.Drawing.Size(34, 25);
            this.butAirFlowPlus.TabIndex = 21;
            this.butAirFlowPlus.Text = "+1";
            this.butAirFlowPlus.UseVisualStyleBackColor = false;
            //
            //butAirFlowMinus
            //
            this.butAirFlowMinus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.butAirFlowMinus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butAirFlowMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butAirFlowMinus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butAirFlowMinus.Location = new System.Drawing.Point(422, 34);
            this.butAirFlowMinus.Margin = new System.Windows.Forms.Padding(0);
            this.butAirFlowMinus.Name = "butAirFlowMinus";
            this.butAirFlowMinus.Size = new System.Drawing.Size(34, 25);
            this.butAirFlowMinus.TabIndex = 20;
            this.butAirFlowMinus.Text = "-1";
            this.butAirFlowMinus.UseVisualStyleBackColor = false;
            //
            //butTemperaturePlus
            //
            this.butTemperaturePlus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.butTemperaturePlus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butTemperaturePlus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butTemperaturePlus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butTemperaturePlus.Location = new System.Drawing.Point(464, 7);
            this.butTemperaturePlus.Margin = new System.Windows.Forms.Padding(0);
            this.butTemperaturePlus.Name = "butTemperaturePlus";
            this.butTemperaturePlus.Size = new System.Drawing.Size(34, 25);
            this.butTemperaturePlus.TabIndex = 19;
            this.butTemperaturePlus.Text = "+5";
            this.butTemperaturePlus.UseVisualStyleBackColor = false;
            //
            //butTemperatureMinus
            //
            this.butTemperatureMinus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.butTemperatureMinus.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butTemperatureMinus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butTemperatureMinus.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butTemperatureMinus.Location = new System.Drawing.Point(422, 7);
            this.butTemperatureMinus.Margin = new System.Windows.Forms.Padding(0);
            this.butTemperatureMinus.Name = "butTemperatureMinus";
            this.butTemperatureMinus.Size = new System.Drawing.Size(34, 25);
            this.butTemperatureMinus.TabIndex = 18;
            this.butTemperatureMinus.Text = "-5";
            this.butTemperatureMinus.UseVisualStyleBackColor = false;
            //
            //labelPoint
            //
            this.labelPoint.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelPoint.Location = new System.Drawing.Point(25, 42);
            this.labelPoint.Name = "labelPoint";
            this.labelPoint.Size = new System.Drawing.Size(88, 18);
            this.labelPoint.TabIndex = 11;
            this.labelPoint.Text = "Point";
            this.labelPoint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //labelTime
            //
            this.labelTime.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelTime.Location = new System.Drawing.Point(341, 66);
            this.labelTime.Name = "labelTime";
            this.labelTime.Size = new System.Drawing.Size(70, 15);
            this.labelTime.TabIndex = 10;
            this.labelTime.Text = "---";
            //
            //labelAirFlow
            //
            this.labelAirFlow.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelAirFlow.Location = new System.Drawing.Point(341, 39);
            this.labelAirFlow.Name = "labelAirFlow";
            this.labelAirFlow.Size = new System.Drawing.Size(70, 15);
            this.labelAirFlow.TabIndex = 9;
            this.labelAirFlow.Text = "---";
            //
            //labelTemperature
            //
            this.labelTemperature.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelTemperature.Location = new System.Drawing.Point(341, 12);
            this.labelTemperature.Name = "labelTemperature";
            this.labelTemperature.Size = new System.Drawing.Size(70, 15);
            this.labelTemperature.TabIndex = 8;
            this.labelTemperature.Text = "---";
            //
            //labelTimeTitle
            //
            this.labelTimeTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelTimeTitle.Location = new System.Drawing.Point(215, 66);
            this.labelTimeTitle.Name = "labelTimeTitle";
            this.labelTimeTitle.Size = new System.Drawing.Size(110, 15);
            this.labelTimeTitle.TabIndex = 7;
            this.labelTimeTitle.Text = "Time";
            //
            //labelAirFlowTitle
            //
            this.labelAirFlowTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelAirFlowTitle.Location = new System.Drawing.Point(215, 39);
            this.labelAirFlowTitle.Name = "labelAirFlowTitle";
            this.labelAirFlowTitle.Size = new System.Drawing.Size(110, 15);
            this.labelAirFlowTitle.TabIndex = 6;
            this.labelAirFlowTitle.Text = "Air flow";
            //
            //labelTemperatureTitle
            //
            this.labelTemperatureTitle.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelTemperatureTitle.Location = new System.Drawing.Point(215, 12);
            this.labelTemperatureTitle.Name = "labelTemperatureTitle";
            this.labelTemperatureTitle.Size = new System.Drawing.Size(110, 15);
            this.labelTemperatureTitle.TabIndex = 5;
            this.labelTemperatureTitle.Text = "Temperature";
            //
            //button_rightEdit
            //
            this.button_rightEdit.BackColor = System.Drawing.Color.Transparent;
            this.button_rightEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_rightEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_rightEdit.FlatAppearance.BorderSize = 0;
            this.button_rightEdit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_rightEdit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_rightEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_rightEdit.Image = My.Resources.Resources.arrow_right;
            this.button_rightEdit.Location = new System.Drawing.Point(98, 66);
            this.button_rightEdit.Margin = new System.Windows.Forms.Padding(0);
            this.button_rightEdit.Name = "button_rightEdit";
            this.button_rightEdit.Size = new System.Drawing.Size(15, 15);
            this.button_rightEdit.TabIndex = 4;
            this.button_rightEdit.UseVisualStyleBackColor = false;
            //
            //button_leftEdit
            //
            this.button_leftEdit.BackColor = System.Drawing.Color.Transparent;
            this.button_leftEdit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_leftEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_leftEdit.FlatAppearance.BorderSize = 0;
            this.button_leftEdit.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_leftEdit.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_leftEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_leftEdit.Image = My.Resources.Resources.arrow_left;
            this.button_leftEdit.Location = new System.Drawing.Point(25, 66);
            this.button_leftEdit.Margin = new System.Windows.Forms.Padding(0);
            this.button_leftEdit.Name = "button_leftEdit";
            this.button_leftEdit.Size = new System.Drawing.Size(15, 15);
            this.button_leftEdit.TabIndex = 3;
            this.button_leftEdit.UseVisualStyleBackColor = false;
            //
            //labelPointTotal
            //
            this.labelPointTotal.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelPointTotal.Location = new System.Drawing.Point(73, 66);
            this.labelPointTotal.Margin = new System.Windows.Forms.Padding(0);
            this.labelPointTotal.Name = "labelPointTotal";
            this.labelPointTotal.Size = new System.Drawing.Size(25, 15);
            this.labelPointTotal.TabIndex = 2;
            this.labelPointTotal.Text = "1";
            this.labelPointTotal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //labelProfileSplitEdit
            //
            this.labelProfileSplitEdit.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelProfileSplitEdit.Location = new System.Drawing.Point(61, 66);
            this.labelProfileSplitEdit.Margin = new System.Windows.Forms.Padding(0);
            this.labelProfileSplitEdit.Name = "labelProfileSplitEdit";
            this.labelProfileSplitEdit.Size = new System.Drawing.Size(15, 15);
            this.labelProfileSplitEdit.TabIndex = 1;
            this.labelProfileSplitEdit.Text = "/";
            this.labelProfileSplitEdit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //labelPointSelected
            //
            this.labelPointSelected.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.labelPointSelected.Location = new System.Drawing.Point(40, 66);
            this.labelPointSelected.Margin = new System.Windows.Forms.Padding(0);
            this.labelPointSelected.Name = "labelPointSelected";
            this.labelPointSelected.Size = new System.Drawing.Size(25, 15);
            this.labelPointSelected.TabIndex = 0;
            this.labelPointSelected.Text = "1";
            this.labelPointSelected.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            //ProfileParameters
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pageProfilesParametersEdit);
            this.Controls.Add(this.pageProfilesParameters);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ProfileParameters";
            this.Size = new System.Drawing.Size(584, 285);
            this.pageProfilesParameters.ResumeLayout(false);
            this.pageProfilesParametersEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Panel pageProfilesParameters;
        internal System.Windows.Forms.Label labelProfileTotal;
        internal System.Windows.Forms.Label labelProfileSplit;
        internal System.Windows.Forms.Label labelProfileSelected;
        internal System.Windows.Forms.Button button_left;
        internal System.Windows.Forms.Button button_right;
        internal System.Windows.Forms.Label labelProfileNameTitle;
        internal System.Windows.Forms.Label labelProfileDuration;
        internal System.Windows.Forms.Label labelProfileMode;
        internal System.Windows.Forms.Label labelProfileName;
        internal System.Windows.Forms.Label labelProfileDurationTitle;
        internal System.Windows.Forms.Label labelProfileModeTitle;
        internal System.Windows.Forms.Panel pageProfilesParametersEdit;
        internal System.Windows.Forms.Label labelTime;
        internal System.Windows.Forms.Label labelAirFlow;
        internal System.Windows.Forms.Label labelTemperature;
        internal System.Windows.Forms.Label labelTimeTitle;
        internal System.Windows.Forms.Label labelAirFlowTitle;
        internal System.Windows.Forms.Label labelTemperatureTitle;
        internal System.Windows.Forms.Button button_rightEdit;
        internal System.Windows.Forms.Button button_leftEdit;
        internal System.Windows.Forms.Label labelPointTotal;
        internal System.Windows.Forms.Label labelProfileSplitEdit;
        internal System.Windows.Forms.Label labelPointSelected;
        internal System.Windows.Forms.Label labelPoint;
        internal RoutinesLibrary.UI.RL_Button butTemperatureMinus;
        internal RoutinesLibrary.UI.RL_Button butTemperaturePlus;
        internal RoutinesLibrary.UI.RL_Button butAirFlowMinus;
        internal RoutinesLibrary.UI.RL_Button butAirFlowPlus;
        internal RoutinesLibrary.UI.RL_Button butTimeMinus;
        internal RoutinesLibrary.UI.RL_Button butTimePlus;
        internal System.Windows.Forms.Label labelProfileNameEdit;

    }
}
