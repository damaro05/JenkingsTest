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
partial class ProfileOptions : System.Windows.Forms.UserControl
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
            this.pageProfilesOptions = new System.Windows.Forms.Panel();
            this.butProfilesSync = new System.Windows.Forms.Button();
            this.butProfilesSync.Click += new System.EventHandler(this.butProfilesSync_Click);
            this.butProfilesCopy = new System.Windows.Forms.Button();
            this.butProfilesCopy.Click += new System.EventHandler(this.butProfilesCopy_Click);
            this.butProfilesDelete = new System.Windows.Forms.Button();
            this.butProfilesDelete.Click += new System.EventHandler(this.butProfilesDelete_Click);
            this.butProfilesEdit = new System.Windows.Forms.Button();
            this.butProfilesEdit.Click += new System.EventHandler(this.butProfilesEdit_Click);
            this.butProfilesNew = new System.Windows.Forms.Button();
            this.butProfilesNew.Click += new System.EventHandler(this.butProfilesNew_Click);
            this.pageProfilesOptionsEdit = new System.Windows.Forms.Panel();
            this.butCancelEditedProfile = new System.Windows.Forms.Button();
            this.butCancelEditedProfile.Click += new System.EventHandler(this.butCancelEditedProfile_Click);
            this.butSaveEditedProfile = new System.Windows.Forms.Button();
            this.butSaveEditedProfile.Click += new System.EventHandler(this.butSaveEditedProfile_Click);
            this.butRemovePoint = new System.Windows.Forms.Button();
            this.butRemovePoint.Click += new System.EventHandler(this.butRemovePoint_Click);
            this.butAddPoint = new System.Windows.Forms.Button();
            this.butAddPoint.Click += new System.EventHandler(this.butAddPoint_Click);
            this.pageProfilesOptions.SuspendLayout();
            this.pageProfilesOptionsEdit.SuspendLayout();
            this.SuspendLayout();
            //
            //pageProfilesOptions
            //
            this.pageProfilesOptions.BackColor = System.Drawing.Color.Transparent;
            this.pageProfilesOptions.Controls.Add(this.butProfilesSync);
            this.pageProfilesOptions.Controls.Add(this.butProfilesCopy);
            this.pageProfilesOptions.Controls.Add(this.butProfilesDelete);
            this.pageProfilesOptions.Controls.Add(this.butProfilesEdit);
            this.pageProfilesOptions.Controls.Add(this.butProfilesNew);
            this.pageProfilesOptions.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.pageProfilesOptions.Location = new System.Drawing.Point(0, 0);
            this.pageProfilesOptions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pageProfilesOptions.Name = "pageProfilesOptions";
            this.pageProfilesOptions.Size = new System.Drawing.Size(506, 46);
            this.pageProfilesOptions.TabIndex = 46;
            //
            //butProfilesSync
            //
            this.butProfilesSync.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butProfilesSync.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butProfilesSync.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butProfilesSync.Location = new System.Drawing.Point(408, 10);
            this.butProfilesSync.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butProfilesSync.Name = "butProfilesSync";
            this.butProfilesSync.Size = new System.Drawing.Size(90, 26);
            this.butProfilesSync.TabIndex = 4;
            this.butProfilesSync.Text = "Sync";
            this.butProfilesSync.UseVisualStyleBackColor = false;
            //
            //butProfilesCopy
            //
            this.butProfilesCopy.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butProfilesCopy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butProfilesCopy.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butProfilesCopy.Location = new System.Drawing.Point(308, 10);
            this.butProfilesCopy.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butProfilesCopy.Name = "butProfilesCopy";
            this.butProfilesCopy.Size = new System.Drawing.Size(90, 26);
            this.butProfilesCopy.TabIndex = 3;
            this.butProfilesCopy.Text = "Copy";
            this.butProfilesCopy.UseVisualStyleBackColor = false;
            //
            //butProfilesDelete
            //
            this.butProfilesDelete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butProfilesDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butProfilesDelete.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butProfilesDelete.Location = new System.Drawing.Point(208, 10);
            this.butProfilesDelete.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butProfilesDelete.Name = "butProfilesDelete";
            this.butProfilesDelete.Size = new System.Drawing.Size(90, 26);
            this.butProfilesDelete.TabIndex = 2;
            this.butProfilesDelete.Text = "Delete";
            this.butProfilesDelete.UseVisualStyleBackColor = false;
            //
            //butProfilesEdit
            //
            this.butProfilesEdit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butProfilesEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butProfilesEdit.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butProfilesEdit.Location = new System.Drawing.Point(108, 10);
            this.butProfilesEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butProfilesEdit.Name = "butProfilesEdit";
            this.butProfilesEdit.Size = new System.Drawing.Size(90, 26);
            this.butProfilesEdit.TabIndex = 1;
            this.butProfilesEdit.Text = "Edit";
            this.butProfilesEdit.UseVisualStyleBackColor = false;
            //
            //butProfilesNew
            //
            this.butProfilesNew.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butProfilesNew.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butProfilesNew.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butProfilesNew.Location = new System.Drawing.Point(8, 10);
            this.butProfilesNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butProfilesNew.Name = "butProfilesNew";
            this.butProfilesNew.Size = new System.Drawing.Size(90, 26);
            this.butProfilesNew.TabIndex = 0;
            this.butProfilesNew.Text = "New";
            this.butProfilesNew.UseVisualStyleBackColor = false;
            //
            //pageProfilesOptionsEdit
            //
            this.pageProfilesOptionsEdit.BackColor = System.Drawing.Color.Transparent;
            this.pageProfilesOptionsEdit.Controls.Add(this.butCancelEditedProfile);
            this.pageProfilesOptionsEdit.Controls.Add(this.butSaveEditedProfile);
            this.pageProfilesOptionsEdit.Controls.Add(this.butRemovePoint);
            this.pageProfilesOptionsEdit.Controls.Add(this.butAddPoint);
            this.pageProfilesOptionsEdit.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.pageProfilesOptionsEdit.Location = new System.Drawing.Point(0, 80);
            this.pageProfilesOptionsEdit.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pageProfilesOptionsEdit.Name = "pageProfilesOptionsEdit";
            this.pageProfilesOptionsEdit.Size = new System.Drawing.Size(506, 46);
            this.pageProfilesOptionsEdit.TabIndex = 47;
            //
            //butCancelEditedProfile
            //
            this.butCancelEditedProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butCancelEditedProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butCancelEditedProfile.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butCancelEditedProfile.Location = new System.Drawing.Point(380, 10);
            this.butCancelEditedProfile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butCancelEditedProfile.Name = "butCancelEditedProfile";
            this.butCancelEditedProfile.Size = new System.Drawing.Size(104, 26);
            this.butCancelEditedProfile.TabIndex = 3;
            this.butCancelEditedProfile.Text = "Cancel";
            this.butCancelEditedProfile.UseVisualStyleBackColor = false;
            //
            //butSaveEditedProfile
            //
            this.butSaveEditedProfile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butSaveEditedProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butSaveEditedProfile.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butSaveEditedProfile.Location = new System.Drawing.Point(259, 10);
            this.butSaveEditedProfile.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butSaveEditedProfile.Name = "butSaveEditedProfile";
            this.butSaveEditedProfile.Size = new System.Drawing.Size(104, 26);
            this.butSaveEditedProfile.TabIndex = 2;
            this.butSaveEditedProfile.Text = "Save";
            this.butSaveEditedProfile.UseVisualStyleBackColor = false;
            //
            //butRemovePoint
            //
            this.butRemovePoint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butRemovePoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butRemovePoint.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butRemovePoint.Location = new System.Drawing.Point(138, 10);
            this.butRemovePoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butRemovePoint.Name = "butRemovePoint";
            this.butRemovePoint.Size = new System.Drawing.Size(104, 26);
            this.butRemovePoint.TabIndex = 1;
            this.butRemovePoint.Text = "Remove point";
            this.butRemovePoint.UseVisualStyleBackColor = false;
            //
            //butAddPoint
            //
            this.butAddPoint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.butAddPoint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butAddPoint.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.butAddPoint.Location = new System.Drawing.Point(17, 10);
            this.butAddPoint.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butAddPoint.Name = "butAddPoint";
            this.butAddPoint.Size = new System.Drawing.Size(104, 26);
            this.butAddPoint.TabIndex = 0;
            this.butAddPoint.Text = "Add point";
            this.butAddPoint.UseVisualStyleBackColor = false;
            //
            //ProfileOptions
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pageProfilesOptionsEdit);
            this.Controls.Add(this.pageProfilesOptions);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ProfileOptions";
            this.Size = new System.Drawing.Size(584, 193);
            this.pageProfilesOptions.ResumeLayout(false);
            this.pageProfilesOptionsEdit.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Panel pageProfilesOptions;
        internal System.Windows.Forms.Button butProfilesNew;
        internal System.Windows.Forms.Button butProfilesSync;
        internal System.Windows.Forms.Button butProfilesCopy;
        internal System.Windows.Forms.Button butProfilesDelete;
        internal System.Windows.Forms.Button butProfilesEdit;
        internal System.Windows.Forms.Panel pageProfilesOptionsEdit;
        internal System.Windows.Forms.Button butCancelEditedProfile;
        internal System.Windows.Forms.Button butSaveEditedProfile;
        internal System.Windows.Forms.Button butRemovePoint;
        internal System.Windows.Forms.Button butAddPoint;

    }
}
