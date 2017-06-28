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
partial class ParamTable : System.Windows.Forms.UserControl
    {

        //UserControl reemplaza a Dispose para limpiar la lista de componentes.
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
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.MouseLeave += new System.EventHandler(ParamTable_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(ParamTable_MouseUp);
            base.KeyDown += new System.Windows.Forms.KeyEventHandler(ParamTable_KeyDown);
            base.KeyUp += new System.Windows.Forms.KeyEventHandler(ParamTable_KeyUp);
            this.tbEdit = new System.Windows.Forms.TextBox();
            this.tbEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbEdit_KeyPress);
            this.tbEdit.Validating += new System.ComponentModel.CancelEventHandler(this.tbEdit_Validating);
            this.PanelTable = new System.Windows.Forms.Panel();
            this.TableLayoutPanel1.SuspendLayout();
            this.PanelTable.SuspendLayout();
            this.SuspendLayout();
            //
            //TableLayoutPanel1
            //
            this.TableLayoutPanel1.AutoSize = true;
            this.TableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.TableLayoutPanel1.ColumnCount = 3;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(204.0F)));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(66.0F)));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(20.0F)));
            this.TableLayoutPanel1.Controls.Add(this.tbEdit, 1, 0);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel1.Size = new System.Drawing.Size(290, 31);
            this.TableLayoutPanel1.TabIndex = 1;
            //
            //tbEdit
            //
            this.tbEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbEdit.Location = new System.Drawing.Point(207, 3);
            this.tbEdit.Name = "tbEdit";
            this.tbEdit.Size = new System.Drawing.Size(53, 20);
            this.tbEdit.TabIndex = 3;
            this.tbEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            //PanelTable
            //
            this.PanelTable.AutoScroll = true;
            this.PanelTable.BackColor = System.Drawing.Color.Transparent;
            this.PanelTable.Controls.Add(this.TableLayoutPanel1);
            this.PanelTable.Location = new System.Drawing.Point(0, 0);
            this.PanelTable.Margin = new System.Windows.Forms.Padding(0);
            this.PanelTable.Name = "PanelTable";
            this.PanelTable.Size = new System.Drawing.Size(311, 148);
            this.PanelTable.TabIndex = 2;
            //
            //ParamTable
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.PanelTable);
            this.Name = "ParamTable";
            this.Size = new System.Drawing.Size(337, 200);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel1.PerformLayout();
            this.PanelTable.ResumeLayout(false);
            this.PanelTable.PerformLayout();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Panel PanelTable;
        internal System.Windows.Forms.TextBox tbEdit;

    }
}
