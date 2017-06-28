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
partial class TempPanels_HA : System.Windows.Forms.UserControl
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
            this.pageTempLevels = new System.Windows.Forms.Panel();
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblLvl3 = new System.Windows.Forms.Label();
            this.lblLvl3.Click += new System.EventHandler(this.lblLvl_Click);
            this.lblLvl2 = new System.Windows.Forms.Label();
            this.lblLvl2.Click += new System.EventHandler(this.lblLvl_Click);
            this.lblLvl1 = new System.Windows.Forms.Label();
            this.lblLvl1.Click += new System.EventHandler(this.lblLvl_Click);
            this.lblTitleLevel = new System.Windows.Forms.Label();
            this.pageTempManual = new System.Windows.Forms.Panel();
            this.pcbAddFlow = new System.Windows.Forms.PictureBox();
            this.pcbAddFlow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseDown);
            this.pcbAddFlow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseUp);
            this.pcbSubstractFlow = new System.Windows.Forms.PictureBox();
            this.pcbSubstractFlow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseDown);
            this.pcbSubstractFlow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseUp);
            this.lblDesFlow = new System.Windows.Forms.Label();
            this.pcbAdd = new System.Windows.Forms.PictureBox();
            this.pcbAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseDown);
            this.pcbAdd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseUp);
            this.pcbSubstract = new System.Windows.Forms.PictureBox();
            this.pcbSubstract.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseDown);
            this.pcbSubstract.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcbAddAndSubs_MouseUp);
            this.lblDesTemp = new System.Windows.Forms.Label();
            this.pageTempLevels.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.pageTempManual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbAddFlow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbSubstractFlow).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbAdd).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbSubstract).BeginInit();
            this.SuspendLayout();
            //
            //pageTempLevels
            //
            this.pageTempLevels.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageTempLevels.Controls.Add(this.TableLayoutPanel1);
            this.pageTempLevels.Controls.Add(this.lblTitleLevel);
            this.pageTempLevels.Location = new System.Drawing.Point(370, 4);
            this.pageTempLevels.Name = "pageTempLevels";
            this.pageTempLevels.Size = new System.Drawing.Size(336, 99);
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
            //pageTempManual
            //
            this.pageTempManual.BackColor = System.Drawing.Color.CornflowerBlue;
            this.pageTempManual.Controls.Add(this.pcbAddFlow);
            this.pageTempManual.Controls.Add(this.pcbSubstractFlow);
            this.pageTempManual.Controls.Add(this.lblDesFlow);
            this.pageTempManual.Controls.Add(this.pcbAdd);
            this.pageTempManual.Controls.Add(this.pcbSubstract);
            this.pageTempManual.Controls.Add(this.lblDesTemp);
            this.pageTempManual.Location = new System.Drawing.Point(3, 4);
            this.pageTempManual.Name = "pageTempManual";
            this.pageTempManual.Size = new System.Drawing.Size(336, 99);
            this.pageTempManual.TabIndex = 4;
            //
            //pcbAddFlow
            //
            this.pcbAddFlow.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.pcbAddFlow.Image = My.Resources.Resources.butplus1;
            this.pcbAddFlow.Location = new System.Drawing.Point(263, 41);
            this.pcbAddFlow.Name = "pcbAddFlow";
            this.pcbAddFlow.Size = new System.Drawing.Size(70, 43);
            this.pcbAddFlow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pcbAddFlow.TabIndex = 6;
            this.pcbAddFlow.TabStop = false;
            //
            //pcbSubstractFlow
            //
            this.pcbSubstractFlow.Image = My.Resources.Resources.butminus1;
            this.pcbSubstractFlow.Location = new System.Drawing.Point(187, 41);
            this.pcbSubstractFlow.Name = "pcbSubstractFlow";
            this.pcbSubstractFlow.Size = new System.Drawing.Size(70, 43);
            this.pcbSubstractFlow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pcbSubstractFlow.TabIndex = 5;
            this.pcbSubstractFlow.TabStop = false;
            //
            //lblDesFlow
            //
            this.lblDesFlow.BackColor = System.Drawing.Color.Transparent;
            this.lblDesFlow.Font = new System.Drawing.Font("Verdana", (float)(11.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.lblDesFlow.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            this.lblDesFlow.Location = new System.Drawing.Point(165, 0);
            this.lblDesFlow.Name = "lblDesFlow";
            this.lblDesFlow.Size = new System.Drawing.Size(168, 38);
            this.lblDesFlow.TabIndex = 4;
            this.lblDesFlow.Text = "Caudal 000 %";
            this.lblDesFlow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //pcbAdd
            //
            this.pcbAdd.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.pcbAdd.Image = My.Resources.Resources.butplus5;
            this.pcbAdd.Location = new System.Drawing.Point(82, 41);
            this.pcbAdd.Name = "pcbAdd";
            this.pcbAdd.Size = new System.Drawing.Size(70, 43);
            this.pcbAdd.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pcbAdd.TabIndex = 3;
            this.pcbAdd.TabStop = false;
            //
            //pcbSubstract
            //
            this.pcbSubstract.Image = My.Resources.Resources.butminus5;
            this.pcbSubstract.Location = new System.Drawing.Point(6, 41);
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
            this.lblDesTemp.Size = new System.Drawing.Size(168, 38);
            this.lblDesTemp.TabIndex = 1;
            this.lblDesTemp.Text = "Temp. 000 ºC";
            this.lblDesTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            //TempPanels_HA
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.pageTempManual);
            this.Controls.Add(this.pageTempLevels);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "TempPanels_HA";
            this.Size = new System.Drawing.Size(954, 429);
            this.pageTempLevels.ResumeLayout(false);
            this.TableLayoutPanel1.ResumeLayout(false);
            this.pageTempManual.ResumeLayout(false);
            this.pageTempManual.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbAddFlow).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbSubstractFlow).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbAdd).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbSubstract).EndInit();
            this.ResumeLayout(false);

        }
        internal System.Windows.Forms.Panel pageTempLevels;
        internal System.Windows.Forms.Label lblLvl1;
        internal System.Windows.Forms.Label lblTitleLevel;
        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.Label lblLvl3;
        internal System.Windows.Forms.Label lblLvl2;
        internal System.Windows.Forms.Panel pageTempManual;
        internal System.Windows.Forms.PictureBox pcbAdd;
        internal System.Windows.Forms.PictureBox pcbSubstract;
        internal System.Windows.Forms.Label lblDesTemp;
        internal System.Windows.Forms.PictureBox pcbAddFlow;
        internal System.Windows.Forms.PictureBox pcbSubstractFlow;
        internal System.Windows.Forms.Label lblDesFlow;

    }
}
