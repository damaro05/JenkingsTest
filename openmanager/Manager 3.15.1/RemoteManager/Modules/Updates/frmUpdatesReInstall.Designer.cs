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
partial class frmUpdatesReInstall : System.Windows.Forms.Form
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
            this.labelTitle = new System.Windows.Forms.Label();
            this.Paint += new System.Windows.Forms.PaintEventHandler(frmUpdatesReInstall_Paint);
            this.labelInformation = new System.Windows.Forms.Label();
            this.butUpdate = new System.Windows.Forms.Button();
            this.butUpdate.Click += new System.EventHandler(this.butUpdate_Click);
            this.butClose = new System.Windows.Forms.Button();
            this.butClose.Click += new System.EventHandler(this.butClose_Click);
            this.progressBarUpdate = new System.Windows.Forms.ProgressBar();
            this.labelUpdateStatus = new System.Windows.Forms.Label();
            this.uControlMessage_Error = new uControlMessage_Error();
            this.SuspendLayout();
            //
            //labelTitle
            //
            this.labelTitle.Font = new System.Drawing.Font("Verdana", (float)(24.0F), System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(25, 50);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(350, 158);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "A software update is required";
            //
            //labelInformation
            //
            this.labelInformation.Font = new System.Drawing.Font("Verdana", (float)(11.0F));
            this.labelInformation.Location = new System.Drawing.Point(425, 100);
            this.labelInformation.Name = "labelInformation";
            this.labelInformation.Size = new System.Drawing.Size(350, 150);
            this.labelInformation.TabIndex = 1;
            this.labelInformation.Text = "This version of RemoteManager needs to be updated. This process will close the cu" +
                "rrent application and open the installer. Do you want to install the update on y" +
                "our computer?";
            //
            //butUpdate
            //
            this.butUpdate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butUpdate.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Bold);
            this.butUpdate.Location = new System.Drawing.Point(524, 361);
            this.butUpdate.Name = "butUpdate";
            this.butUpdate.Size = new System.Drawing.Size(116, 32);
            this.butUpdate.TabIndex = 2;
            this.butUpdate.Text = "Update";
            this.butUpdate.UseVisualStyleBackColor = true;
            //
            //butClose
            //
            this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.butClose.Location = new System.Drawing.Point(659, 361);
            this.butClose.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(116, 32);
            this.butClose.TabIndex = 3;
            this.butClose.Text = "Cancel";
            this.butClose.UseVisualStyleBackColor = true;
            //
            //progressBarUpdate
            //
            this.progressBarUpdate.Location = new System.Drawing.Point(25, 290);
            this.progressBarUpdate.Name = "progressBarUpdate";
            this.progressBarUpdate.Size = new System.Drawing.Size(745, 23);
            this.progressBarUpdate.TabIndex = 4;
            this.progressBarUpdate.Visible = false;
            //
            //labelUpdateStatus
            //
            this.labelUpdateStatus.AutoSize = true;
            this.labelUpdateStatus.Location = new System.Drawing.Point(25, 270);
            this.labelUpdateStatus.Name = "labelUpdateStatus";
            this.labelUpdateStatus.Size = new System.Drawing.Size(119, 14);
            this.labelUpdateStatus.TabIndex = 5;
            this.labelUpdateStatus.Text = "Installer progress";
            this.labelUpdateStatus.Visible = false;
            //
            //uControlMessage_Error
            //
            this.uControlMessage_Error.BackColor = System.Drawing.Color.MistyRose;
            this.uControlMessage_Error.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.uControlMessage_Error.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.uControlMessage_Error.Location = new System.Drawing.Point(25, 270);
            this.uControlMessage_Error.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.uControlMessage_Error.Name = "uControlMessage_Error";
            this.uControlMessage_Error.Size = new System.Drawing.Size(745, 44);
            this.uControlMessage_Error.TabIndex = 6;
            this.uControlMessage_Error.Visible = false;
            //
            //frmUpdatesReInstall
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ClientSize = new System.Drawing.Size(779, 401);
            this.ControlBox = false;
            this.Controls.Add(this.labelUpdateStatus);
            this.Controls.Add(this.progressBarUpdate);
            this.Controls.Add(this.butClose);
            this.Controls.Add(this.butUpdate);
            this.Controls.Add(this.labelInformation);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.uControlMessage_Error);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmUpdatesReInstall";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RemoteManager update";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Label labelTitle;
        internal System.Windows.Forms.Label labelInformation;
        internal System.Windows.Forms.Button butUpdate;
        internal System.Windows.Forms.Button butClose;
        internal System.Windows.Forms.ProgressBar progressBarUpdate;
        internal System.Windows.Forms.Label labelUpdateStatus;
        internal uControlMessage_Error uControlMessage_Error;
    }
}
