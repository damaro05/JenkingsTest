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
partial class ParamTree : System.Windows.Forms.UserControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParamTree));
            this.tbEdit = new System.Windows.Forms.TextBox();
            this.tbEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbEdit_KeyPress);
            this.tbEdit.Validating += new System.ComponentModel.CancelEventHandler(this.tbEdit_Validating);
            this.PanelTable = new System.Windows.Forms.Panel();
            this.TreeView1 = new System.Windows.Forms.TreeView();
            this.TreeView1.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.TreeView1_BeforeCheck);
            this.TreeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterCheck);
            this.TreeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeView1_AfterSelect);
            this.TreeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TreeView1_KeyDown);
            this.TreeView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TreeView1_KeyUp);
            this.TreeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TreeView1_MouseMove);
            this.TreeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseClick);
            this.mnuContextMenuTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuTreeCopyNode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreeCopyNode.Click += new System.EventHandler(this.mnuTreeCopyNode_Click);
            this.mnuTreePasteNode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreePasteNode.Click += new System.EventHandler(this.mnuTreePasteNode_Click);
            this.mnuTreePasteNodeChecked = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreePasteNodeChecked.Click += new System.EventHandler(this.mnuTreePasteNode_Click);
            this.mnuTreeView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreeExpandNodeAndChildren = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreeExpandNodeAndChildren.Click += new System.EventHandler(this.mnuTreeExpandNodeAndChildren_Click);
            this.mnuTreeCollapseNodeAndChildren = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreeCollapseNodeAndChildren.Click += new System.EventHandler(this.mnuTreeCollapseNodeAndChildren_Click);
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuTreeExpandAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreeExpandAll.Click += new System.EventHandler(this.mnuTreeExpandAll_Click);
            this.mnuTreeCollapseAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTreeCollapseAll.Click += new System.EventHandler(this.mnuTreeCollapseAll_Click);
            this.ImageListParamTree = new System.Windows.Forms.ImageList(this.components);
            this.PanelTable.SuspendLayout();
            this.mnuContextMenuTree.SuspendLayout();
            this.SuspendLayout();
            //
            //tbEdit
            //
            this.tbEdit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbEdit.Location = new System.Drawing.Point(3, 167);
            this.tbEdit.Name = "tbEdit";
            this.tbEdit.Size = new System.Drawing.Size(53, 20);
            this.tbEdit.TabIndex = 3;
            this.tbEdit.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            //
            //PanelTable
            //
            this.PanelTable.AutoScroll = true;
            this.PanelTable.BackColor = System.Drawing.Color.Transparent;
            this.PanelTable.Controls.Add(this.TreeView1);
            this.PanelTable.Location = new System.Drawing.Point(0, 0);
            this.PanelTable.Margin = new System.Windows.Forms.Padding(0);
            this.PanelTable.Name = "PanelTable";
            this.PanelTable.Size = new System.Drawing.Size(311, 148);
            this.PanelTable.TabIndex = 2;
            //
            //TreeView1
            //
            this.TreeView1.BackColor = System.Drawing.Color.LightSteelBlue;
            this.TreeView1.CheckBoxes = true;
            this.TreeView1.ContextMenuStrip = this.mnuContextMenuTree;
            this.TreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TreeView1.Font = new System.Drawing.Font("Microsoft Sans Serif", (float)(8.25F), System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, System.Convert.ToByte(0));
            this.TreeView1.ForeColor = System.Drawing.Color.DarkBlue;
            this.TreeView1.ImageIndex = 0;
            this.TreeView1.ImageList = this.ImageListParamTree;
            this.TreeView1.Location = new System.Drawing.Point(0, 0);
            this.TreeView1.Name = "TreeView1";
            this.TreeView1.SelectedImageIndex = 0;
            this.TreeView1.Size = new System.Drawing.Size(311, 148);
            this.TreeView1.TabIndex = 4;
            //
            //mnuContextMenuTree
            //
            this.mnuContextMenuTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuTreeCopyNode, this.mnuTreePasteNode, this.mnuTreePasteNodeChecked, this.mnuTreeView });
            this.mnuContextMenuTree.Name = "mnuContextMenuTree";
            this.mnuContextMenuTree.Size = new System.Drawing.Size(225, 92);
            //
            //mnuTreeCopyNode
            //
            this.mnuTreeCopyNode.Name = "mnuTreeCopyNode";
            this.mnuTreeCopyNode.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C);
            this.mnuTreeCopyNode.Size = new System.Drawing.Size(224, 22);
            this.mnuTreeCopyNode.Text = "Copy";
            //
            //mnuTreePasteNode
            //
            this.mnuTreePasteNode.Name = "mnuTreePasteNode";
            this.mnuTreePasteNode.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V);
            this.mnuTreePasteNode.Size = new System.Drawing.Size(224, 22);
            this.mnuTreePasteNode.Text = "Paste";
            //
            //mnuTreePasteNodeChecked
            //
            this.mnuTreePasteNodeChecked.Name = "mnuTreePasteNodeChecked";
            this.mnuTreePasteNodeChecked.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M);
            this.mnuTreePasteNodeChecked.Size = new System.Drawing.Size(224, 22);
            this.mnuTreePasteNodeChecked.Text = "Paste Checked Only ";
            //
            //mnuTreeView
            //
            this.mnuTreeView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuTreeExpandNodeAndChildren, this.mnuTreeCollapseNodeAndChildren, this.ToolStripSeparator1, this.mnuTreeExpandAll, this.mnuTreeCollapseAll });
            this.mnuTreeView.Name = "mnuTreeView";
            this.mnuTreeView.Size = new System.Drawing.Size(224, 22);
            this.mnuTreeView.Text = "View";
            //
            //mnuTreeExpandNodeAndChildren
            //
            this.mnuTreeExpandNodeAndChildren.Name = "mnuTreeExpandNodeAndChildren";
            this.mnuTreeExpandNodeAndChildren.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Right);
            this.mnuTreeExpandNodeAndChildren.Size = new System.Drawing.Size(246, 22);
            this.mnuTreeExpandNodeAndChildren.Text = "Expand Selected Node";
            //
            //mnuTreeCollapseNodeAndChildren
            //
            this.mnuTreeCollapseNodeAndChildren.Name = "mnuTreeCollapseNodeAndChildren";
            this.mnuTreeCollapseNodeAndChildren.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Left);
            this.mnuTreeCollapseNodeAndChildren.Size = new System.Drawing.Size(246, 22);
            this.mnuTreeCollapseNodeAndChildren.Text = "Collapse Selected Node";
            //
            //ToolStripSeparator1
            //
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(243, 6);
            //
            //mnuTreeExpandAll
            //
            this.mnuTreeExpandAll.Name = "mnuTreeExpandAll";
            this.mnuTreeExpandAll.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down);
            this.mnuTreeExpandAll.Size = new System.Drawing.Size(246, 22);
            this.mnuTreeExpandAll.Text = "Expand All";
            //
            //mnuTreeCollapseAll
            //
            this.mnuTreeCollapseAll.Name = "mnuTreeCollapseAll";
            this.mnuTreeCollapseAll.ShortcutKeys = (System.Windows.Forms.Keys)(System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Up);
            this.mnuTreeCollapseAll.Size = new System.Drawing.Size(246, 22);
            this.mnuTreeCollapseAll.Text = "Collapse All";
            //
            //ImageListParamTree
            //
            this.ImageListParamTree.ImageStream = (System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageListParamTree.ImageStream"));
            this.ImageListParamTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ImageListParamTree.Images.SetKeyName(0, "_none");
            this.ImageListParamTree.Images.SetKeyName(1, "_on");
            this.ImageListParamTree.Images.SetKeyName(2, "_off");
            //
            //ParamTree
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.PanelTable);
            this.Controls.Add(this.tbEdit);
            this.Name = "ParamTree";
            this.Size = new System.Drawing.Size(337, 200);
            this.PanelTable.ResumeLayout(false);
            this.mnuContextMenuTree.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Panel PanelTable;
        internal System.Windows.Forms.TextBox tbEdit;
        internal System.Windows.Forms.TreeView TreeView1;
        internal System.Windows.Forms.ContextMenuStrip mnuContextMenuTree;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreeView;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreeExpandAll;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreeCollapseAll;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreeExpandNodeAndChildren;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreeCollapseNodeAndChildren;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreeCopyNode;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreePasteNode;
        internal System.Windows.Forms.ToolStripMenuItem mnuTreePasteNodeChecked;
        internal System.Windows.Forms.ImageList ImageListParamTree;

    }
}
