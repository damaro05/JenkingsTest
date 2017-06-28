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
    partial class frmMainRegister : System.Windows.Forms.Form
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
            this.components = new System.ComponentModel.Container();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frmMain_FormClosing);
            this.Resize += new System.EventHandler(Form1_Resize);
            this.ResizeEnd += new System.EventHandler(Form1_ResizeEnd);
            this.SizeChanged += new System.EventHandler(Form1_SizeChanged);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMainRegister));
            this.tsMedia = new System.Windows.Forms.ToolStrip();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbXY = new System.Windows.Forms.ToolStripButton();
            this.tsbXY.Click += new System.EventHandler(this.tsbXY_Click);
            this.tsbZooms = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbZooms.ButtonClick += new System.EventHandler(this.tsbZooms_ButtonClick);
            this.XYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.XYToolStripMenuItem.Click += new System.EventHandler(this.XYToolStripMenuItem_Click);
            this.XToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.XToolStripMenuItem.Click += new System.EventHandler(this.XToolStripMenuItem_Click);
            this.YToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.YToolStripMenuItem.Click += new System.EventHandler(this.YToolStripMenuItem_Click);
            this.tsbDefaultZoom = new System.Windows.Forms.ToolStripButton();
            this.tsbDefaultZoom.Click += new System.EventHandler(this.tsbDefaultZoom_Click);
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.lblTrigger = new System.Windows.Forms.ToolStripLabel();
            this.tsbResetTrigger = new System.Windows.Forms.ToolStripButton();
            this.tsbResetTrigger.Click += new System.EventHandler(this.tsbResetTrigger_Click);
            this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPlay = new System.Windows.Forms.ToolStripButton();
            this.tsbPlay.Click += new System.EventHandler(this.tsbPlay_Click);
            this.tsbPause = new System.Windows.Forms.ToolStripButton();
            this.tsbPause.Click += new System.EventHandler(this.tsbPause_Click);
            this.tsbStop = new System.Windows.Forms.ToolStripButton();
            this.tsbStop.Click += new System.EventHandler(this.tsbStop_Click);
            this.tsbRecord = new System.Windows.Forms.ToolStripButton();
            this.tsbRecord.Click += new System.EventHandler(this.tsbRecord_Click);
            this.lblStatus = new System.Windows.Forms.ToolStripLabel();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewToolStripMenuItem.Click += new System.EventHandler(this.NewToolStripMenuItem_Click);
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            this.SaveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsToolStripMenuItem.Click += new System.EventHandler(this.SaveAsToolStripMenuItem_Click);
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.PrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PrintToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItem_Click);
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ExportToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExportToCSVToolStripMenuItem.Click += new System.EventHandler(this.ExportToCSVToolStripMenuItem_Click);
            this.ConfigurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WizardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WizardToolStripMenuItem.Click += new System.EventHandler(this.WizardToolStripMenuItem_Click);
            this.SerieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SerieToolStripMenuItem.Click += new System.EventHandler(this.SerieToolStripMenuItem_Click);
            this.AxisGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AxisGridToolStripMenuItem.Click += new System.EventHandler(this.AxisGridToolStripMenuItem_Click);
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OptionsToolStripMenuItem.Click += new System.EventHandler(this.OptionsToolStripMenuItem_Click);
            this.TitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TitleToolStripMenuItem.Click += new System.EventHandler(this.TitleToolStripMenuItem_Click);
            this.TemplatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadToolStripMenuItem.Click += new System.EventHandler(this.LoadToolStripMenuItem_Click);
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            this.prntPlot = new System.Drawing.Printing.PrintDocument();
            this.prntPlot.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.prntPlot_PrintPage);
            this.PrintDialog1 = new System.Windows.Forms.PrintDialog();
            this.PrintPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.sfdSave = new System.Windows.Forms.SaveFileDialog();
            this.ofdOpen = new System.Windows.Forms.OpenFileDialog();
            this.ttpPlayTime = new System.Windows.Forms.ToolTip(this.components);
            this.bckLongProc = new System.ComponentModel.BackgroundWorker();
            this.bckLongProc.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bckLongProc_DoWork);
            this.bckLongProc.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bckLongProc_RunWorkerCompleted);
            this.pcbMediaBar = new System.Windows.Forms.PictureBox();
            this.pcbMediaBar.Click += new System.EventHandler(this.pcbMediaBar_Click);
            this.pcbMediaBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pcbMediaBar_MouseMove);
            this.pcbMain = new System.Windows.Forms.PictureBox();
            this.pcbMain.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pcbMain_MouseClick);
            this.pcbMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pcbMain_MouseDown);
            this.pcbMain.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pcbMain_MouseMove);
            this.pcbMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pcbMain_MouseUp);
            this.pcbMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.pcbMain_DragEnter);
            this.pcbMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.pcbMain_DragDrop);
            this.tsMedia.SuspendLayout();
            this.mnuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbMediaBar).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbMain).BeginInit();
            this.SuspendLayout();
            //
            //tsMedia
            //
            this.tsMedia.Dock = System.Windows.Forms.DockStyle.None;
            this.tsMedia.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsMedia.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.ToolStripSeparator4, this.tsbXY, this.tsbZooms, this.tsbDefaultZoom, this.ToolStripSeparator1, this.lblTrigger, this.tsbResetTrigger, this.ToolStripSeparator6, this.tsbPlay, this.tsbPause, this.tsbStop, this.tsbRecord, this.lblStatus });
            this.tsMedia.Location = new System.Drawing.Point(151, 8);
            this.tsMedia.Name = "tsMedia";
            this.tsMedia.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsMedia.Size = new System.Drawing.Size(390, 25);
            this.tsMedia.TabIndex = 3;
            this.tsMedia.Text = "Media";
            //
            //ToolStripSeparator4
            //
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            //
            //tsbXY
            //
            this.tsbXY.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbXY.Image = (System.Drawing.Image)(resources.GetObject("tsbXY.Image"));
            this.tsbXY.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbXY.Name = "tsbXY";
            this.tsbXY.Size = new System.Drawing.Size(23, 22);
            this.tsbXY.Text = "Coordinates";
            //
            //tsbZooms
            //
            this.tsbZooms.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbZooms.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.XYToolStripMenuItem, this.XToolStripMenuItem, this.YToolStripMenuItem });
            this.tsbZooms.Image = (System.Drawing.Image)(resources.GetObject("tsbZooms.Image"));
            this.tsbZooms.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.tsbZooms.Name = "tsbZooms";
            this.tsbZooms.Size = new System.Drawing.Size(32, 22);
            this.tsbZooms.Text = "Zoom";
            //
            //XYToolStripMenuItem
            //
            this.XYToolStripMenuItem.Image = (System.Drawing.Image)(resources.GetObject("XYToolStripMenuItem.Image"));
            this.XYToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.XYToolStripMenuItem.Name = "XYToolStripMenuItem";
            this.XYToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.XYToolStripMenuItem.Text = "XY";
            //
            //XToolStripMenuItem
            //
            this.XToolStripMenuItem.Image = (System.Drawing.Image)(resources.GetObject("XToolStripMenuItem.Image"));
            this.XToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.XToolStripMenuItem.Name = "XToolStripMenuItem";
            this.XToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.XToolStripMenuItem.Text = "X";
            //
            //YToolStripMenuItem
            //
            this.YToolStripMenuItem.Image = (System.Drawing.Image)(resources.GetObject("YToolStripMenuItem.Image"));
            this.YToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.White;
            this.YToolStripMenuItem.Name = "YToolStripMenuItem";
            this.YToolStripMenuItem.Size = new System.Drawing.Size(97, 22);
            this.YToolStripMenuItem.Text = "Y";
            //
            //tsbDefaultZoom
            //
            this.tsbDefaultZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDefaultZoom.Image = (System.Drawing.Image)(resources.GetObject("tsbDefaultZoom.Image"));
            this.tsbDefaultZoom.ImageTransparentColor = System.Drawing.Color.White;
            this.tsbDefaultZoom.Name = "tsbDefaultZoom";
            this.tsbDefaultZoom.Size = new System.Drawing.Size(23, 22);
            this.tsbDefaultZoom.Text = "Default zoom";
            //
            //ToolStripSeparator1
            //
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            //
            //lblTrigger
            //
            this.lblTrigger.AutoSize = false;
            this.lblTrigger.Name = "lblTrigger";
            this.lblTrigger.Size = new System.Drawing.Size(85, 22);
            this.lblTrigger.Text = "TRG_MANUAL";
            this.lblTrigger.ToolTipText = "Trigger";
            //
            //tsbResetTrigger
            //
            this.tsbResetTrigger.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResetTrigger.Image = (System.Drawing.Image)(resources.GetObject("tsbResetTrigger.Image"));
            this.tsbResetTrigger.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResetTrigger.Name = "tsbResetTrigger";
            this.tsbResetTrigger.Size = new System.Drawing.Size(23, 22);
            this.tsbResetTrigger.Text = "Reset trigger detection";
            //
            //ToolStripSeparator6
            //
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            this.ToolStripSeparator6.Size = new System.Drawing.Size(6, 25);
            //
            //tsbPlay
            //
            this.tsbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPlay.Image = (System.Drawing.Image)(resources.GetObject("tsbPlay.Image"));
            this.tsbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPlay.Name = "tsbPlay";
            this.tsbPlay.Size = new System.Drawing.Size(23, 22);
            this.tsbPlay.Text = "Play";
            this.tsbPlay.ToolTipText = "Play";
            //
            //tsbPause
            //
            this.tsbPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPause.Image = (System.Drawing.Image)(resources.GetObject("tsbPause.Image"));
            this.tsbPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPause.Name = "tsbPause";
            this.tsbPause.Size = new System.Drawing.Size(23, 22);
            this.tsbPause.Text = "Pause";
            this.tsbPause.ToolTipText = "Pause";
            //
            //tsbStop
            //
            this.tsbStop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStop.Image = (System.Drawing.Image)(resources.GetObject("tsbStop.Image"));
            this.tsbStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStop.Name = "tsbStop";
            this.tsbStop.Size = new System.Drawing.Size(23, 22);
            this.tsbStop.Text = "Stop";
            this.tsbStop.ToolTipText = "Stop";
            //
            //tsbRecord
            //
            this.tsbRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRecord.Image = (System.Drawing.Image)(resources.GetObject("tsbRecord.Image"));
            this.tsbRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRecord.Name = "tsbRecord";
            this.tsbRecord.Size = new System.Drawing.Size(23, 22);
            this.tsbRecord.Text = "Record";
            this.tsbRecord.ToolTipText = "Record";
            //
            //lblStatus
            //
            this.lblStatus.AutoSize = false;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 22);
            this.lblStatus.Text = "RECORD";
            this.lblStatus.ToolTipText = "Status";
            //
            //mnuMain
            //
            this.mnuMain.AllowMerge = false;
            this.mnuMain.Dock = System.Windows.Forms.DockStyle.None;
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.FileToolStripMenuItem, this.ConfigurationToolStripMenuItem });
            this.mnuMain.Location = new System.Drawing.Point(13, 9);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.mnuMain.Size = new System.Drawing.Size(127, 24);
            this.mnuMain.TabIndex = 4;
            this.mnuMain.Text = "Menu";
            //
            //FileToolStripMenuItem
            //
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.NewToolStripMenuItem, this.OpenToolStripMenuItem, this.SaveAsToolStripMenuItem, this.ToolStripSeparator2, this.PrintToolStripMenuItem, this.ToolStripSeparator3, this.ExportToCSVToolStripMenuItem });
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.FileToolStripMenuItem.Text = "File";
            //
            //NewToolStripMenuItem
            //
            this.NewToolStripMenuItem.Name = "NewToolStripMenuItem";
            this.NewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.NewToolStripMenuItem.Text = "New...";
            //
            //OpenToolStripMenuItem
            //
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.OpenToolStripMenuItem.Text = "Open...";
            //
            //SaveAsToolStripMenuItem
            //
            this.SaveAsToolStripMenuItem.Name = "SaveAsToolStripMenuItem";
            this.SaveAsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.SaveAsToolStripMenuItem.Text = "Save as...";
            //
            //ToolStripSeparator2
            //
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(149, 6);
            //
            //PrintToolStripMenuItem
            //
            this.PrintToolStripMenuItem.Name = "PrintToolStripMenuItem";
            this.PrintToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.PrintToolStripMenuItem.Text = "Print...";
            //
            //ToolStripSeparator3
            //
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            //
            //ExportToCSVToolStripMenuItem
            //
            this.ExportToCSVToolStripMenuItem.Name = "ExportToCSVToolStripMenuItem";
            this.ExportToCSVToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ExportToCSVToolStripMenuItem.Text = "Export to CSV";
            //
            //ConfigurationToolStripMenuItem
            //
            this.ConfigurationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.WizardToolStripMenuItem, this.SerieToolStripMenuItem, this.AxisGridToolStripMenuItem, this.OptionsToolStripMenuItem, this.TitleToolStripMenuItem, this.TemplatesToolStripMenuItem });
            this.ConfigurationToolStripMenuItem.Name = "ConfigurationToolStripMenuItem";
            this.ConfigurationToolStripMenuItem.Size = new System.Drawing.Size(84, 20);
            this.ConfigurationToolStripMenuItem.Text = "Configuration";
            //
            //WizardToolStripMenuItem
            //
            this.WizardToolStripMenuItem.Name = "WizardToolStripMenuItem";
            this.WizardToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.WizardToolStripMenuItem.Text = "Wizard...";
            //
            //SerieToolStripMenuItem
            //
            this.SerieToolStripMenuItem.Name = "SerieToolStripMenuItem";
            this.SerieToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.SerieToolStripMenuItem.Text = "Series...";
            //
            //AxisGridToolStripMenuItem
            //
            this.AxisGridToolStripMenuItem.Name = "AxisGridToolStripMenuItem";
            this.AxisGridToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.AxisGridToolStripMenuItem.Text = "Axis and Grid...";
            //
            //OptionsToolStripMenuItem
            //
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.OptionsToolStripMenuItem.Text = "Options...";
            //
            //TitleToolStripMenuItem
            //
            this.TitleToolStripMenuItem.Name = "TitleToolStripMenuItem";
            this.TitleToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.TitleToolStripMenuItem.Text = "Title...";
            //
            //TemplatesToolStripMenuItem
            //
            this.TemplatesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.LoadToolStripMenuItem, this.SaveToolStripMenuItem });
            this.TemplatesToolStripMenuItem.Name = "TemplatesToolStripMenuItem";
            this.TemplatesToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.TemplatesToolStripMenuItem.Text = "Templates";
            //
            //LoadToolStripMenuItem
            //
            this.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem";
            this.LoadToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.LoadToolStripMenuItem.Text = "Load...";
            //
            //SaveToolStripMenuItem
            //
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.SaveToolStripMenuItem.Text = "Save...";
            //
            //prntPlot
            //
            //
            //PrintDialog1
            //
            this.PrintDialog1.UseEXDialog = true;
            //
            //PrintPreviewDialog1
            //
            this.PrintPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.PrintPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.PrintPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.PrintPreviewDialog1.Enabled = true;
            this.PrintPreviewDialog1.Icon = (System.Drawing.Icon)(resources.GetObject("PrintPreviewDialog1.Icon"));
            this.PrintPreviewDialog1.Name = "PrintPreviewDialog1";
            this.PrintPreviewDialog1.Visible = false;
            //
            //ofdOpen
            //
            this.ofdOpen.FileName = "OpenFileDialog1";
            //
            //bckLongProc
            //
            //
            //pcbMediaBar
            //
            this.pcbMediaBar.Anchor = (System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.pcbMediaBar.Location = new System.Drawing.Point(513, 8);
            this.pcbMediaBar.Name = "pcbMediaBar";
            this.pcbMediaBar.Size = new System.Drawing.Size(405, 23);
            this.pcbMediaBar.TabIndex = 5;
            this.pcbMediaBar.TabStop = false;
            //
            //pcbMain
            //
            this.pcbMain.Anchor = (System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            this.pcbMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pcbMain.Location = new System.Drawing.Point(13, 37);
            this.pcbMain.Name = "pcbMain";
            this.pcbMain.Size = new System.Drawing.Size(905, 513);
            this.pcbMain.TabIndex = 0;
            this.pcbMain.TabStop = false;
            //
            //frmMainRegister
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(930, 566);
            this.Controls.Add(this.pcbMediaBar);
            this.Controls.Add(this.tsMedia);
            this.Controls.Add(this.mnuMain);
            this.Controls.Add(this.pcbMain);
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.MainMenuStrip = this.mnuMain;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "frmMainRegister";
            this.Text = "Register Manager";
            this.tsMedia.ResumeLayout(false);
            this.tsMedia.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)this.pcbMediaBar).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.pcbMain).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.PictureBox pcbMain;
        internal System.Windows.Forms.ToolStrip tsMedia;
        internal System.Windows.Forms.ToolStripButton tsbPlay;
        internal System.Windows.Forms.ToolStripButton tsbPause;
        internal System.Windows.Forms.ToolStripButton tsbStop;
        internal System.Windows.Forms.ToolStripButton tsbRecord;
        internal System.Windows.Forms.MenuStrip mnuMain;
        internal System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem NewToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SaveAsToolStripMenuItem;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
        internal System.Windows.Forms.ToolStripMenuItem ConfigurationToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem WizardToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SerieToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AxisGridToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem OptionsToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem PrintToolStripMenuItem;
        internal System.Windows.Forms.ToolStripLabel lblStatus;
        internal System.Drawing.Printing.PrintDocument prntPlot;
        internal System.Windows.Forms.PrintDialog PrintDialog1;
        internal System.Windows.Forms.PrintPreviewDialog PrintPreviewDialog1;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
        internal System.Windows.Forms.PictureBox pcbMediaBar;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
        internal System.Windows.Forms.ToolStripMenuItem ExportToCSVToolStripMenuItem;
        internal System.Windows.Forms.SaveFileDialog sfdSave;
        internal System.Windows.Forms.OpenFileDialog ofdOpen;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
        internal System.Windows.Forms.ToolStripButton tsbXY;
        internal System.Windows.Forms.ToolTip ttpPlayTime;
        internal System.Windows.Forms.ToolStripMenuItem TitleToolStripMenuItem;
        internal System.ComponentModel.BackgroundWorker bckLongProc;
        internal System.Windows.Forms.ToolStripSplitButton tsbZooms;
        internal System.Windows.Forms.ToolStripMenuItem XYToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem XToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem YToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem TemplatesToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem LoadToolStripMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        internal System.Windows.Forms.ToolStripButton tsbDefaultZoom;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        internal System.Windows.Forms.ToolStripLabel lblTrigger;
        internal System.Windows.Forms.ToolStripButton tsbResetTrigger;

    }
}
