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
partial class frmEvents : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEvents));
            this.treeEvents = new System.Windows.Forms.TreeView();
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmEvents_FormClosing);
            this.SuspendLayout();
            //
            //treeEvents
            //
            this.treeEvents.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.treeEvents.Location = new System.Drawing.Point(1, 1);
            this.treeEvents.Name = "treeEvents";
            this.treeEvents.Size = new System.Drawing.Size(421, 259);
            this.treeEvents.TabIndex = 0;
            //
            //frmEvents
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 261);
            this.Controls.Add(this.treeEvents);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEvents";
            this.Text = "Eventos";
            this.TopMost = true;
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.TreeView treeEvents;
    }
}
