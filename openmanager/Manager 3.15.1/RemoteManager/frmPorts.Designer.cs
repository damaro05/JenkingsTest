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
partial class frmPorts : System.Windows.Forms.Form
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
            this.SuspendLayout();
            //
            //frmPorts
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(frmPorts_FormClosed);
            lblData1.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort1);
            pcbTool1.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort1);
            lblData2.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort2);
            pcbTool2.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort2);
            lblData3.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort3);
            pcbTool3.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort3);
            lblData4.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort4);
            pcbTool4.MouseUp += new System.Windows.Forms.MouseEventHandler(MouseUpPort4);
            lblData1.MouseEnter += new System.EventHandler(MouseEnterPort1);
            pcbTool1.MouseEnter += new System.EventHandler(MouseEnterPort1);
            lblData2.MouseEnter += new System.EventHandler(MouseEnterPort2);
            pcbTool2.MouseEnter += new System.EventHandler(MouseEnterPort2);
            lblData3.MouseEnter += new System.EventHandler(MouseEnterPort3);
            pcbTool3.MouseEnter += new System.EventHandler(MouseEnterPort3);
            lblData4.MouseEnter += new System.EventHandler(MouseEnterPort4);
            pcbTool4.MouseEnter += new System.EventHandler(MouseEnterPort4);
            lblData1.MouseLeave += new System.EventHandler(MouseLeavePort1);
            pcbTool1.MouseLeave += new System.EventHandler(MouseLeavePort1);
            lblData2.MouseLeave += new System.EventHandler(MouseLeavePort2);
            pcbTool2.MouseLeave += new System.EventHandler(MouseLeavePort2);
            lblData3.MouseLeave += new System.EventHandler(MouseLeavePort3);
            pcbTool3.MouseLeave += new System.EventHandler(MouseLeavePort3);
            lblData4.MouseLeave += new System.EventHandler(MouseLeavePort4);
            pcbTool4.MouseLeave += new System.EventHandler(MouseLeavePort4);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 247);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "frmPorts";
            this.ShowIcon = false;
            this.Text = "Ports";
            this.ResumeLayout(false);

        }
    }
}
