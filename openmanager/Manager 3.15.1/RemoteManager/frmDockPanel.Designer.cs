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
partial class frmDockPanel : System.Windows.Forms.Form
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
            base.Load += new System.EventHandler(frmDockPanel_Load);
            base.Move += new System.EventHandler(frmDockPanel_Move);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDockPanel));
            System.Windows.Forms.TreeNode TreeNode1 = new System.Windows.Forms.TreeNode("di-lab - DI - 9999999");
            System.Windows.Forms.TreeNode TreeNode2 = new System.Windows.Forms.TreeNode("dd-name - DD - 8888888");
            System.Windows.Forms.TreeNode TreeNode3 = new System.Windows.Forms.TreeNode("Estaciones sold", new System.Windows.Forms.TreeNode[] { TreeNode2 });
            System.Windows.Forms.TreeNode TreeNode4 = new System.Windows.Forms.TreeNode("Station List", new System.Windows.Forms.TreeNode[] { TreeNode1, TreeNode3 });
            this.tspFormHeader = new System.Windows.Forms.ToolStrip();
            this.tspFormHeader.MouseEnter += new System.EventHandler(this.DockPanel_MouseEnter);
            this.tspFormHeader.MouseLeave += new System.EventHandler(this.DockPanel_MouseLeave);
            this.tspFormHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tspFormHeader_MouseDown);
            this.tspFormHeader.MouseLeave += new System.EventHandler(this.tspFormHeader_MouseLeave);
            this.tspFormHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tspFormHeader_MouseMove);
            this.butClose = new System.Windows.Forms.ToolStripButton();
            this.butClose.Click += new System.EventHandler(this.butClose_Click);
            this.butAutoHide = new System.Windows.Forms.ToolStripButton();
            this.butAutoHide.Click += new System.EventHandler(this.butAutoHide_Click);
            this.butWinPos = new System.Windows.Forms.ToolStripDropDownButton();
            this.mnuFloating = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFloating.Click += new System.EventHandler(this.mnuWinPos_Click);
            this.ToolStripMenuItemSep1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTop = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTop.Click += new System.EventHandler(this.mnuWinPos_Click);
            this.mnuBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBottom.Click += new System.EventHandler(this.mnuWinPos_Click);
            this.mnuBottom.Click += new System.EventHandler(this.mnuWinPos_Click);
            this.mnuLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLeft.Click += new System.EventHandler(this.mnuWinPos_Click);
            this.mnuRight = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRight.Click += new System.EventHandler(this.mnuWinPos_Click);
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuList = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuList.Click += new System.EventHandler(this.mnuViewType_Click);
            this.mnuTile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTile.Click += new System.EventHandler(this.mnuViewType_Click);
            this.mnuDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDetails.Click += new System.EventHandler(this.mnuViewType_Click);
            this.mnuTreeList = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreeList.Click += new System.EventHandler(this.mnuViewType_Click);
            this.lblHeader = new System.Windows.Forms.ToolStripLabel();
            this.lblHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tspFormHeader_MouseDown);
            this.lblHeader.MouseLeave += new System.EventHandler(this.tspFormHeader_MouseLeave);
            this.lblHeader.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tspFormHeader_MouseMove);
            this.pnlFormArea = new System.Windows.Forms.Panel();
            this.pnlFormArea.MouseEnter += new System.EventHandler(this.DockPanel_MouseEnter);
            this.pnlFormArea.MouseLeave += new System.EventHandler(this.DockPanel_MouseLeave);
            this.splitStationList = new System.Windows.Forms.Splitter();
            this.lsvwStationList = new System.Windows.Forms.ListView();
            this.lsvwStationList.MouseEnter += new System.EventHandler(this.DockPanel_MouseEnter);
            this.lsvwStationList.MouseLeave += new System.EventHandler(this.DockPanel_MouseLeave);
            this.colName = (System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader());
            this.colModel = (System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader());
            this.colSW = (System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader());
            this.imgStationList = new System.Windows.Forms.ImageList(this.components);
            this.treevwStationList = new System.Windows.Forms.TreeView();
            this.imgStationListTree = new System.Windows.Forms.ImageList(this.components);
            this.tspFormHeader.SuspendLayout();
            this.pnlFormArea.SuspendLayout();
            this.SuspendLayout();
            //
            //tspFormHeader
            //
            this.tspFormHeader.BackColor = System.Drawing.Color.Wheat;
            this.tspFormHeader.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tspFormHeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.butClose, this.butAutoHide, this.butWinPos, this.lblHeader });
            this.tspFormHeader.Location = new System.Drawing.Point(0, 0);
            this.tspFormHeader.Name = "tspFormHeader";
            this.tspFormHeader.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tspFormHeader.Size = new System.Drawing.Size(242, 25);
            this.tspFormHeader.TabIndex = 0;
            this.tspFormHeader.Text = "ToolStrip1";
            //
            //butClose
            //
            this.butClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.butClose.Image = My.Resources.Resources.BlackClose;
            this.butClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.butClose.Name = "butClose";
            this.butClose.Size = new System.Drawing.Size(23, 22);
            this.butClose.Text = "Close";
            //
            //butAutoHide
            //
            this.butAutoHide.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.butAutoHide.Image = My.Resources.Resources.BlackClip;
            this.butAutoHide.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.butAutoHide.Name = "butAutoHide";
            this.butAutoHide.Size = new System.Drawing.Size(23, 22);
            this.butAutoHide.Text = "Hide";
            this.butAutoHide.ToolTipText = "Auto Hide On/Off";
            //
            //butWinPos
            //
            this.butWinPos.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.None;
            this.butWinPos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuFloating, this.ToolStripMenuItemSep1, this.mnuTop, this.mnuBottom, this.mnuLeft, this.mnuRight, this.ToolStripMenuItem1, this.mnuList, this.mnuTreeList });
            this.butWinPos.Image = (System.Drawing.Image)(resources.GetObject("butWinPos.Image"));
            this.butWinPos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.butWinPos.Name = "butWinPos";
            this.butWinPos.Size = new System.Drawing.Size(13, 22);
            this.butWinPos.Text = "Position";
            this.butWinPos.ToolTipText = "Panel Position";
            //
            //mnuFloating
            //
            this.mnuFloating.Name = "mnuFloating";
            this.mnuFloating.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuFloating.Size = new System.Drawing.Size(123, 22);
            this.mnuFloating.Text = "Floating";
            //
            //ToolStripMenuItemSep1
            //
            this.ToolStripMenuItemSep1.Name = "ToolStripMenuItemSep1";
            this.ToolStripMenuItemSep1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ToolStripMenuItemSep1.Size = new System.Drawing.Size(120, 6);
            //
            //mnuTop
            //
            this.mnuTop.Name = "mnuTop";
            this.mnuTop.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuTop.Size = new System.Drawing.Size(123, 22);
            this.mnuTop.Text = "Top";
            //
            //mnuBottom
            //
            this.mnuBottom.Name = "mnuBottom";
            this.mnuBottom.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuBottom.Size = new System.Drawing.Size(123, 22);
            this.mnuBottom.Text = "Bottom";
            //
            //mnuLeft
            //
            this.mnuLeft.Name = "mnuLeft";
            this.mnuLeft.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuLeft.Size = new System.Drawing.Size(123, 22);
            this.mnuLeft.Text = "Left";
            //
            //mnuRight
            //
            this.mnuRight.Name = "mnuRight";
            this.mnuRight.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuRight.Size = new System.Drawing.Size(123, 22);
            this.mnuRight.Text = "Right";
            //
            //ToolStripMenuItem1
            //
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(120, 6);
            //
            //mnuList
            //
            this.mnuList.Checked = true;
            this.mnuList.CheckOnClick = true;
            this.mnuList.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuList.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuTile, this.mnuDetails });
            this.mnuList.Name = "mnuList";
            this.mnuList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuList.Size = new System.Drawing.Size(123, 22);
            this.mnuList.Text = "List";
            //
            //mnuTile
            //
            this.mnuTile.Checked = true;
            this.mnuTile.CheckOnClick = true;
            this.mnuTile.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuTile.Name = "mnuTile";
            this.mnuTile.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuTile.Size = new System.Drawing.Size(117, 22);
            this.mnuTile.Text = "Icons";
            //
            //mnuDetails
            //
            this.mnuDetails.CheckOnClick = true;
            this.mnuDetails.Name = "mnuDetails";
            this.mnuDetails.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuDetails.Size = new System.Drawing.Size(117, 22);
            this.mnuDetails.Text = "Details";
            //
            //mnuTreeList
            //
            this.mnuTreeList.CheckOnClick = true;
            this.mnuTreeList.Name = "mnuTreeList";
            this.mnuTreeList.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.mnuTreeList.Size = new System.Drawing.Size(123, 22);
            this.mnuTreeList.Text = "Tree";
            //
            //lblHeader
            //
            this.lblHeader.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblHeader.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblHeader.Size = new System.Drawing.Size(60, 22);
            this.lblHeader.Text = "Station List";
            //
            //pnlFormArea
            //
            this.pnlFormArea.BackColor = System.Drawing.Color.Wheat;
            this.pnlFormArea.Controls.Add(this.splitStationList);
            this.pnlFormArea.Controls.Add(this.lsvwStationList);
            this.pnlFormArea.Controls.Add(this.treevwStationList);
            this.pnlFormArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFormArea.Location = new System.Drawing.Point(0, 25);
            this.pnlFormArea.Name = "pnlFormArea";
            this.pnlFormArea.Size = new System.Drawing.Size(242, 268);
            this.pnlFormArea.TabIndex = 1;
            //
            //splitStationList
            //
            this.splitStationList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitStationList.Location = new System.Drawing.Point(0, 137);
            this.splitStationList.Name = "splitStationList";
            this.splitStationList.Size = new System.Drawing.Size(242, 5);
            this.splitStationList.TabIndex = 2;
            this.splitStationList.TabStop = false;
            //
            //lsvwStationList
            //
            this.lsvwStationList.Alignment = System.Windows.Forms.ListViewAlignment.SnapToGrid;
            this.lsvwStationList.BackColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(206)), System.Convert.ToInt32(System.Convert.ToByte(210)), System.Convert.ToInt32(System.Convert.ToByte(213)));
            this.lsvwStationList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsvwStationList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.colName, this.colModel, this.colSW });
            this.lsvwStationList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsvwStationList.LargeImageList = this.imgStationList;
            this.lsvwStationList.Location = new System.Drawing.Point(0, 0);
            this.lsvwStationList.MultiSelect = false;
            this.lsvwStationList.Name = "lsvwStationList";
            this.lsvwStationList.Size = new System.Drawing.Size(242, 142);
            this.lsvwStationList.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lsvwStationList.TabIndex = 0;
            this.lsvwStationList.UseCompatibleStateImageBehavior = false;
            this.lsvwStationList.View = System.Windows.Forms.View.Tile;
            //
            //colName
            //
            this.colName.Text = "Name";
            //
            //colModel
            //
            this.colModel.Text = "Model";
            //
            //colSW
            //
            this.colSW.Text = "Software Version";
            //
            //imgStationList
            //
            this.imgStationList.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgStationList.ImageStream"));
            this.imgStationList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgStationList.Images.SetKeyName(0, "Station");
            this.imgStationList.Images.SetKeyName(1, "Station_unlock");
            this.imgStationList.Images.SetKeyName(2, "Station_lock");
            //
            //treevwStationList
            //
            this.treevwStationList.AllowDrop = true;
            this.treevwStationList.BackColor = System.Drawing.SystemColors.Info;
            this.treevwStationList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.treevwStationList.Font = new System.Drawing.Font("Microsoft Sans Serif", (float)(8.25F), System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.treevwStationList.HideSelection = false;
            this.treevwStationList.ImageKey = "Station_Disconn";
            this.treevwStationList.ImageList = this.imgStationListTree;
            this.treevwStationList.Indent = 19;
            this.treevwStationList.ItemHeight = 18;
            this.treevwStationList.LabelEdit = true;
            this.treevwStationList.Location = new System.Drawing.Point(0, 142);
            this.treevwStationList.Name = "treevwStationList";
            TreeNode1.ForeColor = System.Drawing.Color.Firebrick;
            TreeNode1.ImageKey = "Station_Disconn";
            TreeNode1.Name = "Nodo0";
            TreeNode1.SelectedImageKey = "Station_Disconn";
            TreeNode1.Text = "di-lab - DI - 9999999";
            TreeNode2.ImageKey = "Station_Conn";
            TreeNode2.Name = "Nodo2";
            TreeNode2.SelectedImageKey = "Station_Conn";
            TreeNode2.Text = "dd-name - DD - 8888888";
            TreeNode3.ForeColor = System.Drawing.Color.Gray;
            TreeNode3.ImageKey = "Folder";
            TreeNode3.Name = "Nodo1";
            TreeNode3.SelectedImageKey = "Folder";
            TreeNode3.Text = "Estaciones sold";
            TreeNode4.ImageKey = "TreeRoot";
            TreeNode4.Name = "Nodo0";
            TreeNode4.SelectedImageKey = "TreeRoot";
            TreeNode4.Text = "Station List";
            this.treevwStationList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { TreeNode4 });
            this.treevwStationList.SelectedImageIndex = 0;
            this.treevwStationList.Size = new System.Drawing.Size(242, 126);
            this.treevwStationList.TabIndex = 1;
            //
            //imgStationListTree
            //
            this.imgStationListTree.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgStationListTree.ImageStream"));
            this.imgStationListTree.TransparentColor = System.Drawing.Color.Transparent;
            this.imgStationListTree.Images.SetKeyName(0, "Folder");
            this.imgStationListTree.Images.SetKeyName(1, "Station_Conn");
            this.imgStationListTree.Images.SetKeyName(2, "Station_Disconn");
            this.imgStationListTree.Images.SetKeyName(3, "TreeRoot");
            //
            //frmDockPanel
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 293);
            this.ControlBox = false;
            this.Controls.Add(this.pnlFormArea);
            this.Controls.Add(this.tspFormHeader);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDockPanel";
            this.Text = " ";
            this.TopMost = true;
            this.tspFormHeader.ResumeLayout(false);
            this.tspFormHeader.PerformLayout();
            this.pnlFormArea.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.ToolStrip tspFormHeader;
        internal System.Windows.Forms.ToolStripButton butAutoHide;
        internal System.Windows.Forms.ToolStripDropDownButton butWinPos;
        internal System.Windows.Forms.ToolStripMenuItem mnuFloating;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItemSep1;
        internal System.Windows.Forms.ToolStripMenuItem mnuTop;
        internal System.Windows.Forms.ToolStripMenuItem mnuBottom;
        internal System.Windows.Forms.ToolStripMenuItem mnuLeft;
        internal System.Windows.Forms.ToolStripMenuItem mnuRight;
        internal System.Windows.Forms.Panel pnlFormArea;
        internal System.Windows.Forms.ListView lsvwStationList;
        internal System.Windows.Forms.ColumnHeader colName;
        internal System.Windows.Forms.ColumnHeader colModel;
        internal System.Windows.Forms.ColumnHeader colSW;
        internal System.Windows.Forms.ToolStripButton butClose;
        internal System.Windows.Forms.ToolStripLabel lblHeader;
        internal System.Windows.Forms.ImageList imgStationList;
        internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
        internal System.Windows.Forms.TreeView treevwStationList;
        internal System.Windows.Forms.ImageList imgStationListTree;
        internal System.Windows.Forms.Splitter splitStationList;
        internal System.Windows.Forms.ToolStripMenuItem mnuList;
        internal System.Windows.Forms.ToolStripMenuItem mnuTile;
        internal System.Windows.Forms.ToolStripMenuItem mnuDetails;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreeList;
    }
}
