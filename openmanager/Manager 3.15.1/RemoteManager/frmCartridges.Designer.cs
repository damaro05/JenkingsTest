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
partial class frmCartridges : System.Windows.Forms.Form
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
            this.pictCart = new System.Windows.Forms.PictureBox();
            base.Load += new System.EventHandler(frmCartridges_Load);
            this.butPrevious = new System.Windows.Forms.Button();
            this.butPrevious.Click += new System.EventHandler(this.butPrevious_Click);
            this.butNext = new System.Windows.Forms.Button();
            this.butNext.Click += new System.EventHandler(this.butNext_Click);
            this.tbCartridgeNumber = new System.Windows.Forms.TextBox();
            this.tbCartridgeNumber.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCartridgeNumber_KeyPress);
            this.butOk = new System.Windows.Forms.Button();
            this.butOk.Click += new System.EventHandler(this.butOk_Click);
            ((System.ComponentModel.ISupportInitialize)this.pictCart).BeginInit();
            this.SuspendLayout();
            //
            //pictCart
            //
            this.pictCart.Location = new System.Drawing.Point(0, -2);
            this.pictCart.Name = "pictCart";
            this.pictCart.Size = new System.Drawing.Size(274, 146);
            this.pictCart.TabIndex = 0;
            this.pictCart.TabStop = false;
            //
            //butPrevious
            //
            this.butPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butPrevious.Location = new System.Drawing.Point(189, 146);
            this.butPrevious.Name = "butPrevious";
            this.butPrevious.Size = new System.Drawing.Size(35, 27);
            this.butPrevious.TabIndex = 1;
            this.butPrevious.Text = "<";
            this.butPrevious.UseVisualStyleBackColor = true;
            //
            //butNext
            //
            this.butNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butNext.Location = new System.Drawing.Point(227, 146);
            this.butNext.Name = "butNext";
            this.butNext.Size = new System.Drawing.Size(35, 27);
            this.butNext.TabIndex = 2;
            this.butNext.Text = ">";
            this.butNext.UseVisualStyleBackColor = true;
            //
            //tbCartridgeNumber
            //
            this.tbCartridgeNumber.Font = new System.Drawing.Font("Verdana", (float)(12.0F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.tbCartridgeNumber.Location = new System.Drawing.Point(118, 146);
            this.tbCartridgeNumber.MaxLength = 3;
            this.tbCartridgeNumber.Name = "tbCartridgeNumber";
            this.tbCartridgeNumber.Size = new System.Drawing.Size(50, 27);
            this.tbCartridgeNumber.TabIndex = 3;
            this.tbCartridgeNumber.Text = "000";
            this.tbCartridgeNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            //butOk
            //
            this.butOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butOk.Location = new System.Drawing.Point(12, 146);
            this.butOk.Name = "butOk";
            this.butOk.Size = new System.Drawing.Size(75, 27);
            this.butOk.TabIndex = 4;
            this.butOk.Text = "Ok";
            this.butOk.UseVisualStyleBackColor = true;
            //
            //frmCartridges
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(273, 174);
            this.Controls.Add(this.butOk);
            this.Controls.Add(this.tbCartridgeNumber);
            this.Controls.Add(this.butNext);
            this.Controls.Add(this.butPrevious);
            this.Controls.Add(this.pictCart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "frmCartridges";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cartridges";
            ((System.ComponentModel.ISupportInitialize)this.pictCart).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.PictureBox pictCart;
        internal System.Windows.Forms.Button butPrevious;
        internal System.Windows.Forms.Button butNext;
        internal System.Windows.Forms.TextBox tbCartridgeNumber;
        internal System.Windows.Forms.Button butOk;
    }
}
