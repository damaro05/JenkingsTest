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
partial class TempPanels : System.Windows.Forms.UserControl
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
            this.pageTempManual = new System.Windows.Forms.Panel();
            this.pcbAdd = new System.Windows.Forms.PictureBox();
            this.pcbAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseDown);
            this.pcbAdd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseUp);
            this.pcbSubstract = new System.Windows.Forms.PictureBox();
            this.pcbSubstract.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseDown);
            this.pcbSubstract.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseUp);
            this.lblDesTemp = new System.Windows.Forms.Label();
            this.pageTempLevels = new System.Windows.Forms.Panel();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLvl3 = new System.Windows.Forms.Label();
            this.lblLvl3.Click += new System.EventHandler(this.lblLvl_Click);
            this.lblLvl2 = new System.Windows.Forms.Label();
            this.lblLvl2.Click += new System.EventHandler(this.lblLvl_Click);
            this.lblLvl1 = new System.Windows.Forms.Label();
            this.lblLvl1.Click += new System.EventHandler(this.lblLvl_Click);
            this.lblTitleLevel = new System.Windows.Forms.Label();
            this.pageTempFixed = new System.Windows.Forms.Panel();
            this.lblFixed = new System.Windows.Forms.Label();
            this.pageTempManual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbAdd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbSubstract).BeginInit();
            this.pageTempLevels.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.pageTempFixed.SuspendLayout();
            this.SuspendLayout();
            //
            //pageTempManual
            //
            this.pageTempManual.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageTempManual.Controls.Add(this.pcbAdd);
            this.pageTempManual.Controls.Add(this.pcbSubstract);
            this.pageTempManual.Controls.Add(this.lblDesTemp);
            this.pageTempManual.Location = new System.Drawing.Point(0, 0);
            this.pageTempManual.Name = "pageTempManual";
            this.pageTempManual.Size = new System.Drawing.Size(302, 99);
            this.pageTempManual.TabIndex = 0;
            //
            //pcbAdd
            //
            this.pcbAdd.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.pcbAdd.Image = My.Resources.Resources.butplus5;
            this.pcbAdd.Location = new System.Drawing.Point(151, 41);
            this.pcbAdd.Name = "pcbAdd";
            this.pcbAdd.Size = new System.Drawing.Size(70, 43);
            this.pcbAdd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pcbAdd.TabIndex = 3;
            this.pcbAdd.TabStop = false;
            //
            //pcbSubstract
            //
            this.pcbSubstract.Image = My.Resources.Resources.butminus5;
            this.pcbSubstract.Location = new System.Drawing.Point(65, 41);
            this.pcbSubstract.Name = "pcbSubstract";
            this.pcbSubstract.Size = new System.Drawing.Size(70, 43);
            this.pcbSubstract.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pcbSubstract.TabIndex = 2;
            this.pcbSubstract.TabStop = false;
            //
            //lblDesTemp
            //
            this.lblDesTemp.BackColor = System.Drawing.Color.Transparent;
            this.lblDesTemp.Font = new System.Drawing.Font("Verdana", (float)(11.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblDesTemp.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblDesTemp.Location = new System.Drawing.Point(3, 0);
            this.lblDesTemp.Name = "lblDesTemp";
            this.lblDesTemp.Size = new System.Drawing.Size(296, 38);
            this.lblDesTemp.TabIndex = 1;
            this.lblDesTemp.Text = "Selected 000 ºC";
            this.lblDesTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageTempLevels
            //
            this.pageTempLevels.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageTempLevels.Controls.Add(this.TableLayoutPanel1);
            this.pageTempLevels.Controls.Add(this.lblTitleLevel);
            this.pageTempLevels.Location = new System.Drawing.Point(317, 0);
            this.pageTempLevels.Name = "pageTempLevels";
            this.pageTempLevels.Size = new System.Drawing.Size(302, 99);
            this.pageTempLevels.TabIndex = 1;
            //
            //TableLayoutPanel1
            //
            this.TableLayoutPanel1.ColumnCount = 3;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)(51.31579F)));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, (float)(48.68421F)));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, (float)(94.0F)));
            this.TableLayoutPanel1.Controls.Add(this.lblLvl3, 2, 0);
            this.TableLayoutPanel1.Controls.Add(this.lblLvl2, 1, 0);
            this.TableLayoutPanel1.Controls.Add(this.lblLvl1, 0, 0);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(36, 31);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, (float)(50.0F)));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(257, 66);
            this.TableLayoutPanel1.TabIndex = 2;
            //
            //lblLvl3
            //
            this.lblLvl3.BackColor = System.Drawing.Color.Transparent;
            this.lblLvl3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLvl3.Font = new System.Drawing.Font("Calibri", (float)(26.25F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblLvl3.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblLvl3.Image = My.Resources.Resources.Border;
            this.lblLvl3.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblLvl3.Location = new System.Drawing.Point(165, 0);
            this.lblLvl3.Name = "lblLvl3";
            this.lblLvl3.Size = new System.Drawing.Size(78, 58);
            this.lblLvl3.TabIndex = 3;
            this.lblLvl3.Text = "000";
            this.lblLvl3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //lblLvl2
            //
            this.lblLvl2.BackColor = System.Drawing.Color.Transparent;
            this.lblLvl2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLvl2.Font = new System.Drawing.Font("Calibri", (float)(26.25F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblLvl2.ForeColor = System.Drawing.Color.White;
            this.lblLvl2.Image = My.Resources.Resources.SelectedBorder;
            this.lblLvl2.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblLvl2.Location = new System.Drawing.Point(86, 0);
            this.lblLvl2.Name = "lblLvl2";
            this.lblLvl2.Size = new System.Drawing.Size(73, 58);
            this.lblLvl2.TabIndex = 2;
            this.lblLvl2.Text = "000";
            this.lblLvl2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //lblLvl1
            //
            this.lblLvl1.BackColor = System.Drawing.Color.Transparent;
            this.lblLvl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblLvl1.Font = new System.Drawing.Font("Calibri", (float)(26.25F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblLvl1.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblLvl1.Image = My.Resources.Resources.Border;
            this.lblLvl1.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblLvl1.Location = new System.Drawing.Point(3, 0);
            this.lblLvl1.Name = "lblLvl1";
            this.lblLvl1.Size = new System.Drawing.Size(77, 58);
            this.lblLvl1.TabIndex = 1;
            this.lblLvl1.Text = "000";
            this.lblLvl1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //lblTitleLevel
            //
            this.lblTitleLevel.BackColor = System.Drawing.Color.Transparent;
            this.lblTitleLevel.Font = new System.Drawing.Font("Verdana", (float)(11.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblTitleLevel.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblTitleLevel.Location = new System.Drawing.Point(10, 4);
            this.lblTitleLevel.Name = "lblTitleLevel";
            this.lblTitleLevel.Size = new System.Drawing.Size(283, 24);
            this.lblTitleLevel.TabIndex = 0;
            this.lblTitleLevel.Text = "Temperature Levels";
            this.lblTitleLevel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pageTempFixed
            //
            this.pageTempFixed.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageTempFixed.Controls.Add(this.lblFixed);
            this.pageTempFixed.Location = new System.Drawing.Point(634, 0);
            this.pageTempFixed.Name = "pageTempFixed";
            this.pageTempFixed.Size = new System.Drawing.Size(302, 99);
            this.pageTempFixed.TabIndex = 2;
            //
            //lblFixed
            //
            this.lblFixed.BackColor = System.Drawing.Color.Transparent;
            this.lblFixed.Font = new System.Drawing.Font("Verdana", (float)(11.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblFixed.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblFixed.Location = new System.Drawing.Point(3, 5);
            this.lblFixed.Name = "lblFixed";
            this.lblFixed.Size = new System.Drawing.Size(296, 40);
            this.lblFixed.TabIndex = 1;
            this.lblFixed.Text = "Fixed Temperature 000 ºC";
            this.lblFixed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //TempPanels
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pageTempFixed);
            this.Controls.Add(this.pageTempLevels);
            this.Controls.Add(this.pageTempManual);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "TempPanels";
            this.Size = new System.Drawing.Size(954, 429);
            this.pageTempManual.ResumeLayout(false);
            this.pageTempManual.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbAdd).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbSubstract).EndInit();
            this.pageTempLevels.ResumeLayout(false);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.pageTempFixed.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Panel pageTempManual;
        internal System.Windows.Forms.Label lblDesTemp;
        internal System.Windows.Forms.PictureBox pcbSubstract;
        internal System.Windows.Forms.PictureBox pcbAdd;
        internal System.Windows.Forms.Panel pageTempLevels;
        internal System.Windows.Forms.Label lblLvl1;
        internal System.Windows.Forms.Label lblTitleLevel;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Label lblLvl3;
        internal System.Windows.Forms.Label lblLvl2;
        internal System.Windows.Forms.Panel pageTempFixed;
        internal System.Windows.Forms.Label lblFixed;

    }
}
