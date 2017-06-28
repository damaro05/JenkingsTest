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
partial class frmBrowser : System.Windows.Forms.Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBrowser));
            this.WebBrowser1 = new System.Windows.Forms.WebBrowser();
            base.Load += new System.EventHandler(frmBrowser_Load);
            this.Panel1 = new System.Windows.Forms.Panel();
            this.butPageSetup = new System.Windows.Forms.Button();
            this.butPageSetup.Click += new System.EventHandler(this.butPageSetup_Click);
            this.butPrint = new System.Windows.Forms.Button();
            this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
            this.butPrintPreview = new System.Windows.Forms.Button();
            this.butPrintPreview.Click += new System.EventHandler(this.butPrintPreview_Click);
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            //
            //WebBrowser1
            //
            this.WebBrowser1.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.WebBrowser1.Location = new System.Drawing.Point(0, 37);
            this.WebBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser1.Name = "WebBrowser1";
            this.WebBrowser1.Size = new System.Drawing.Size(632, 410);
            this.WebBrowser1.TabIndex = 0;
            //
            //Panel1
            //
            this.Panel1.Controls.Add(this.butPageSetup);
            this.Panel1.Controls.Add(this.butPrint);
            this.Panel1.Controls.Add(this.butPrintPreview);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(632, 38);
            this.Panel1.TabIndex = 1;
            //
            //butPageSetup
            //
            this.butPageSetup.Location = new System.Drawing.Point(232, 8);
            this.butPageSetup.Name = "butPageSetup";
            this.butPageSetup.Size = new System.Drawing.Size(104, 23);
            this.butPageSetup.TabIndex = 2;
            this.butPageSetup.Text = "Page Setup";
            this.butPageSetup.UseVisualStyleBackColor = true;
            //
            //butPrint
            //
            this.butPrint.Location = new System.Drawing.Point(122, 8);
            this.butPrint.Name = "butPrint";
            this.butPrint.Size = new System.Drawing.Size(104, 23);
            this.butPrint.TabIndex = 1;
            this.butPrint.Text = "Print";
            this.butPrint.UseVisualStyleBackColor = true;
            //
            //butPrintPreview
            //
            this.butPrintPreview.Location = new System.Drawing.Point(12, 8);
            this.butPrintPreview.Name = "butPrintPreview";
            this.butPrintPreview.Size = new System.Drawing.Size(104, 23);
            this.butPrintPreview.TabIndex = 0;
            this.butPrintPreview.Text = "Preview";
            this.butPrintPreview.UseVisualStyleBackColor = true;
            //
            //frmBrowser
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 446);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.WebBrowser1);
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Name = "frmBrowser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JBC Manager";
            this.Panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.WebBrowser WebBrowser1;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.Button butPageSetup;
        internal System.Windows.Forms.Button butPrint;
        internal System.Windows.Forms.Button butPrintPreview;
    }
}
