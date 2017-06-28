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
    partial class frmTemplate : System.Windows.Forms.Form
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
            base.Load += new System.EventHandler(frmTemplate_Load);
            System.Windows.Forms.TreeNode TreeNode1 = new System.Windows.Forms.TreeNode("Nodo10");
            System.Windows.Forms.TreeNode TreeNode2 = new System.Windows.Forms.TreeNode("Nodo11");
            System.Windows.Forms.TreeNode TreeNode3 = new System.Windows.Forms.TreeNode("<portA>", new System.Windows.Forms.TreeNode[] { TreeNode1, TreeNode2 });
            System.Windows.Forms.TreeNode TreeNode4 = new System.Windows.Forms.TreeNode("Nodo2");
            System.Windows.Forms.TreeNode TreeNode5 = new System.Windows.Forms.TreeNode("Nodo3");
            System.Windows.Forms.TreeNode TreeNode6 = new System.Windows.Forms.TreeNode("Nodo7");
            System.Windows.Forms.TreeNode TreeNode7 = new System.Windows.Forms.TreeNode("<stationA>", new System.Windows.Forms.TreeNode[] { TreeNode3, TreeNode4, TreeNode5, TreeNode6 });
            System.Windows.Forms.TreeNode TreeNode8 = new System.Windows.Forms.TreeNode("Nodo12");
            System.Windows.Forms.TreeNode TreeNode9 = new System.Windows.Forms.TreeNode("Nodo13");
            System.Windows.Forms.TreeNode TreeNode10 = new System.Windows.Forms.TreeNode("Nodo5", new System.Windows.Forms.TreeNode[] { TreeNode8, TreeNode9 });
            System.Windows.Forms.TreeNode TreeNode11 = new System.Windows.Forms.TreeNode("Nodo6");
            System.Windows.Forms.TreeNode TreeNode12 = new System.Windows.Forms.TreeNode("Nodo8");
            System.Windows.Forms.TreeNode TreeNode13 = new System.Windows.Forms.TreeNode("Nodo9");
            System.Windows.Forms.TreeNode TreeNode14 = new System.Windows.Forms.TreeNode("Nodo4", new System.Windows.Forms.TreeNode[] { TreeNode10, TreeNode11, TreeNode12, TreeNode13 });
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTemplate));
            this.lbxTemplateList = new System.Windows.Forms.ListBox();
            this.lbxTemplateList.SelectedIndexChanged += new System.EventHandler(this.lbxTemplateList_SelectedIndexChanged);
            this.lblTemplateList = new System.Windows.Forms.Label();
            this.trvTemplate = new System.Windows.Forms.TreeView();
            this.trvTemplate.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvTemplate_NodeMouseClick);
            this.gbTemplateParams = new System.Windows.Forms.GroupBox();
            this.butCancel = new System.Windows.Forms.Button();
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            this.lblTriggerType = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.lblStation = new System.Windows.Forms.Label();
            this.cbxPortTrigger = new System.Windows.Forms.ComboBox();
            this.cbxStationTrigger = new System.Windows.Forms.ComboBox();
            this.cbxStationTrigger.DropDown += new System.EventHandler(this.cbxStationTrigger_DropDown);
            this.butApply = new System.Windows.Forms.Button();
            this.butApply.Click += new System.EventHandler(this.butApply_Click);
            this.lblTriggerTool = new System.Windows.Forms.Label();
            this.lblSeriesTree = new System.Windows.Forms.Label();
            this.cmsTreeNodeEdit = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsTreeNodeEdit.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsTreeNodeEdit_ItemClicked);
            this.gbTemplateParams.SuspendLayout();
            this.SuspendLayout();
            //
            //lbxTemplateList
            //
            this.lbxTemplateList.FormattingEnabled = true;
            this.lbxTemplateList.Location = new System.Drawing.Point(12, 25);
            this.lbxTemplateList.Name = "lbxTemplateList";
            this.lbxTemplateList.Size = new System.Drawing.Size(178, 238);
            this.lbxTemplateList.TabIndex = 0;
            //
            //lblTemplateList
            //
            this.lblTemplateList.AutoSize = true;
            this.lblTemplateList.Location = new System.Drawing.Point(12, 9);
            this.lblTemplateList.Name = "lblTemplateList";
            this.lblTemplateList.Size = new System.Drawing.Size(70, 13);
            this.lblTemplateList.TabIndex = 1;
            this.lblTemplateList.Text = "Template List";
            //
            //trvTemplate
            //
            this.trvTemplate.Location = new System.Drawing.Point(6, 39);
            this.trvTemplate.Name = "trvTemplate";
            TreeNode1.Name = "Nodo10";
            TreeNode1.Text = "Nodo10";
            TreeNode2.Name = "Nodo11";
            TreeNode2.Text = "Nodo11";
            TreeNode3.Name = "Nodo1";
            TreeNode3.Text = "<portA>";
            TreeNode4.Name = "Nodo2";
            TreeNode4.Text = "Nodo2";
            TreeNode5.Name = "Nodo3";
            TreeNode5.Text = "Nodo3";
            TreeNode6.Name = "Nodo7";
            TreeNode6.Text = "Nodo7";
            TreeNode7.Name = "Nodo0";
            TreeNode7.Text = "<stationA>";
            TreeNode8.Name = "Nodo12";
            TreeNode8.Text = "Nodo12";
            TreeNode9.Name = "Nodo13";
            TreeNode9.Text = "Nodo13";
            TreeNode10.Name = "Nodo5";
            TreeNode10.Text = "Nodo5";
            TreeNode11.Name = "Nodo6";
            TreeNode11.Text = "Nodo6";
            TreeNode12.Name = "Nodo8";
            TreeNode12.Text = "Nodo8";
            TreeNode13.Name = "Nodo9";
            TreeNode13.Text = "Nodo9";
            TreeNode14.Name = "Nodo4";
            TreeNode14.Text = "Nodo4";
            this.trvTemplate.Nodes.AddRange(new System.Windows.Forms.TreeNode[] { TreeNode7, TreeNode14 });
            this.trvTemplate.ShowPlusMinus = false;
            this.trvTemplate.ShowRootLines = false;
            this.trvTemplate.Size = new System.Drawing.Size(164, 206);
            this.trvTemplate.TabIndex = 6;
            //
            //gbTemplateParams
            //
            this.gbTemplateParams.Controls.Add(this.butCancel);
            this.gbTemplateParams.Controls.Add(this.lblTriggerType);
            this.gbTemplateParams.Controls.Add(this.lblPort);
            this.gbTemplateParams.Controls.Add(this.lblStation);
            this.gbTemplateParams.Controls.Add(this.cbxPortTrigger);
            this.gbTemplateParams.Controls.Add(this.cbxStationTrigger);
            this.gbTemplateParams.Controls.Add(this.butApply);
            this.gbTemplateParams.Controls.Add(this.lblTriggerTool);
            this.gbTemplateParams.Controls.Add(this.lblSeriesTree);
            this.gbTemplateParams.Controls.Add(this.trvTemplate);
            this.gbTemplateParams.Location = new System.Drawing.Point(196, 12);
            this.gbTemplateParams.Name = "gbTemplateParams";
            this.gbTemplateParams.Size = new System.Drawing.Size(337, 253);
            this.gbTemplateParams.TabIndex = 7;
            this.gbTemplateParams.TabStop = false;
            this.gbTemplateParams.Text = "Template Parameters";
            //
            //butCancel
            //
            this.butCancel.Location = new System.Drawing.Point(256, 224);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(75, 23);
            this.butCancel.TabIndex = 15;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            //
            //lblTriggerType
            //
            this.lblTriggerType.AutoSize = true;
            this.lblTriggerType.Location = new System.Drawing.Point(187, 45);
            this.lblTriggerType.Name = "lblTriggerType";
            this.lblTriggerType.Size = new System.Drawing.Size(57, 13);
            this.lblTriggerType.TabIndex = 14;
            this.lblTriggerType.Text = "NO_TYPE";
            //
            //lblPort
            //
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(188, 121);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(26, 13);
            this.lblPort.TabIndex = 13;
            this.lblPort.Text = "Port";
            //
            //lblStation
            //
            this.lblStation.AutoSize = true;
            this.lblStation.Location = new System.Drawing.Point(188, 70);
            this.lblStation.Name = "lblStation";
            this.lblStation.Size = new System.Drawing.Size(40, 13);
            this.lblStation.TabIndex = 12;
            this.lblStation.Text = "Station";
            //
            //cbxPortTrigger
            //
            this.cbxPortTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxPortTrigger.FormattingEnabled = true;
            this.cbxPortTrigger.Items.AddRange(new object[] { "1", "2", "3", "4" });
            this.cbxPortTrigger.Location = new System.Drawing.Point(191, 137);
            this.cbxPortTrigger.Name = "cbxPortTrigger";
            this.cbxPortTrigger.Size = new System.Drawing.Size(60, 21);
            this.cbxPortTrigger.TabIndex = 11;
            //
            //cbxStationTrigger
            //
            this.cbxStationTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxStationTrigger.FormattingEnabled = true;
            this.cbxStationTrigger.Location = new System.Drawing.Point(191, 86);
            this.cbxStationTrigger.Name = "cbxStationTrigger";
            this.cbxStationTrigger.Size = new System.Drawing.Size(140, 21);
            this.cbxStationTrigger.TabIndex = 10;
            //
            //butApply
            //
            this.butApply.Location = new System.Drawing.Point(256, 195);
            this.butApply.Name = "butApply";
            this.butApply.Size = new System.Drawing.Size(75, 23);
            this.butApply.TabIndex = 9;
            this.butApply.Text = "Accept";
            this.butApply.UseVisualStyleBackColor = true;
            //
            //lblTriggerTool
            //
            this.lblTriggerTool.AutoSize = true;
            this.lblTriggerTool.Location = new System.Drawing.Point(188, 23);
            this.lblTriggerTool.Name = "lblTriggerTool";
            this.lblTriggerTool.Size = new System.Drawing.Size(40, 13);
            this.lblTriggerTool.TabIndex = 8;
            this.lblTriggerTool.Text = "Trigger";
            //
            //lblSeriesTree
            //
            this.lblSeriesTree.AutoSize = true;
            this.lblSeriesTree.Location = new System.Drawing.Point(6, 23);
            this.lblSeriesTree.Name = "lblSeriesTree";
            this.lblSeriesTree.Size = new System.Drawing.Size(36, 13);
            this.lblSeriesTree.TabIndex = 7;
            this.lblSeriesTree.Text = "Series";
            //
            //cmsTreeNodeEdit
            //
            this.cmsTreeNodeEdit.Name = "cmsTreeNodeEdit";
            this.cmsTreeNodeEdit.Size = new System.Drawing.Size(61, 4);
            //
            //frmTemplate
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(6.0F), (float)(13.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 275);
            this.Controls.Add(this.lblTemplateList);
            this.Controls.Add(this.gbTemplateParams);
            this.Controls.Add(this.lbxTemplateList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = (System.Drawing.Icon)(resources.GetObject("$this.Icon"));
            this.Name = "frmTemplate";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Templates";
            this.gbTemplateParams.ResumeLayout(false);
            this.gbTemplateParams.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.ListBox lbxTemplateList;
        internal System.Windows.Forms.Label lblTemplateList;
        internal System.Windows.Forms.TreeView trvTemplate;
        internal System.Windows.Forms.GroupBox gbTemplateParams;
        internal System.Windows.Forms.Label lblTriggerTool;
        internal System.Windows.Forms.Label lblSeriesTree;
        internal System.Windows.Forms.ContextMenuStrip cmsTreeNodeEdit;
        internal System.Windows.Forms.Button butApply;
        internal System.Windows.Forms.ComboBox cbxPortTrigger;
        internal System.Windows.Forms.ComboBox cbxStationTrigger;
        internal System.Windows.Forms.Label lblPort;
        internal System.Windows.Forms.Label lblStation;
        internal System.Windows.Forms.Label lblTriggerType;
        internal System.Windows.Forms.Button butCancel;
    }
}
