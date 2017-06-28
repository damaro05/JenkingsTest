// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
// End of VB project level imports

namespace RemoteManRegister
{
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public
    partial class frmWait : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmWait));
            this.lblWaitMessage = new System.Windows.Forms.Label();
            base.Load += new System.EventHandler(frmWait_Load);
            this.pgbWait = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            //
            //lblWaitMessage
            //
            this.lblWaitMessage.Location = new System.Drawing.Point(12, 9);
            this.lblWaitMessage.Name = "lblWaitMessage";
            this.lblWaitMessage.Size = new System.Drawing.Size(196, 23);
            this.lblWaitMessage.TabIndex = 0;
            this.lblWaitMessage.Text = "Loading myFile.csv. Please wait...";
            //
            //pgbWait
            //
            this.pgbWait.Location = new System.Drawing.Point(12, 35);
            this.pgbWait.Name = "pgbWait";
            this.pgbWait.Size = new System.Drawing.Size(196, 13);
            this.pgbWait.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pgbWait.TabIndex = 1;
            //
            //frmWait
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(220, 63);
            this.Controls.Add(this.pgbWait);
            this.Controls.Add(this.lblWaitMessage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmWait";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Loading...";
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Label lblWaitMessage;
        internal System.Windows.Forms.ProgressBar pgbWait;
    }
}
