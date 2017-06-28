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

using Microsoft.VisualBasic.CompilerServices;

// FALTA: usar iconos para seleccionar parámetros enabled


namespace RemoteManager
{
    public partial class ParamTree
    {

        // Defines the posible input types for the values of the nodes
        public enum cInputType
        {
            FIX, // fixed text setting, with replaceable value, cannot check/modify (gray font)
            TEXT, // text setting, can check/modify
            NUMBER, // text with replaceable number setting, can check/modify
            SWITCH, // list of text/values setting, can check/modify
            NO_VALUE, // no value close to node name, cannot modify
            FIX_NODE // fix value for node, cannot modify
        }

        public const string EnabledAttribName = "Enabled";
        public const string EnabledTrue = "True";
        public const string EnabledFalse = "False";
        private const int adjustLeftPixelsCursor = 4; // ajustar porque el cursor (la mano) no está a la izquierda


        // Type definitions
        // esta estructura se guarda en cada nodo en la propiedad Tag
        public class tParam
        {
            public string nodeName;
            public int iPort;
            public string sToolName;
            public string sText;
            public cInputType input;
            public bool enabled;
            public bool check;
            public string sValue;
            // if cInputType = NUMBER or FIX, optionsValue(0) = string where to parse the value, for example "{0} ºC"
            // if cInputType = NUMBER, optionsValue(1) = minimum integer value
            // if cInputType = NUMBER, optionsValue(2) = maximum integer value
            // if cInputType = NUMBER, optionsValue(3) = allowed values, separated by # (ej: 0#1#3#5)
            // if cInputType = NUMBER, optionsValue(4) = value if blank, value is edit text box in blanck (ej: ---)
            // if cInputType = SWITCH, optionsValue contains the values and optionsText contains the text to be showed, if diffrerent from values
            public string[] optionsValue;
            public string[] optionsText; // if text is diferent from optionsValue
            public string[] attribParams;
            public string[] attribValues;
        }

        // Properties
        public bool EditMode
        {
            set
            {
                bEditMode = value;
            }
            get
            {
                return bEditMode;
            }
        }

        // Internal var's and parameters
        private bool bShowTreeView = false;

        private Color textColorLocked; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private int rowHeight = 32;
        private bool bControlDown = false;
        private bool bMnuPasteSettingsAndChecks = false;

        // Public Events
        public delegate void NewValueEventHandler(string paramName, string value);
        private NewValueEventHandler NewValueEvent;

        public event NewValueEventHandler NewValue
        {
            add
            {
                NewValueEvent = (NewValueEventHandler)System.Delegate.Combine(NewValueEvent, value);
            }
            remove
            {
                NewValueEvent = (NewValueEventHandler)System.Delegate.Remove(NewValueEvent, value);
            }
        }

        public delegate void LogEventHandler(string sText);
        private LogEventHandler LogEvent;

        public event LogEventHandler Log
        {
            add
            {
                LogEvent = (LogEventHandler)System.Delegate.Combine(LogEvent, value);
            }
            remove
            {
                LogEvent = (LogEventHandler)System.Delegate.Remove(LogEvent, value);
            }
        }


        //Constructor
        public ParamTree(bool bpShowTreeView)
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            textColorLocked = Color.DarkViolet;
            colorLastCopied = Color.CornflowerBlue;


            // Llamada necesaria para el diseñador.
            InitializeComponent();

            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;

            mnuContextMenuTree.KeyDown += mnuContextMenuTree_KeyDown;
            mnuContextMenuTree.KeyUp += mnuContextMenuTree_KeyUp;

            ReLoadTexts();

            tbEdit.Visible = false; // ocultar campo de edición
            bShowTreeView = bpShowTreeView;
        }

        public void ReLoadTexts()
        {
            // menues
            // tree view
            mnuTreeView.Text = Localization.getResStr(Configuration.confMnuViewId);
            mnuTreeCollapseAll.Text = Localization.getResStr(Configuration.confMnuViewCollapseAllId);
            mnuTreeExpandAll.Text = Localization.getResStr(Configuration.confMnuViewExpandAllId);
            mnuTreeCollapseNodeAndChildren.Text = Localization.getResStr(Configuration.confMnuViewCollapseNodeId);
            mnuTreeExpandNodeAndChildren.Text = Localization.getResStr(Configuration.confMnuViewExpandNodeId);
            // copy/paste nodes
            mnuTreeCopyNode.Text = Localization.getResStr(Configuration.confMnuCopyId);
            mnuTreePasteNode.Text = Localization.getResStr(Configuration.confMnuPasteId);
            mnuTreePasteNodeChecked.Text = Localization.getResStr(Configuration.confMnuPasteCheckedId);

        }

        // Adds a param to the table
        public TreeNode addNode(TreeNode oParentNode, string nodeName, int iPort, string sToolName, string text, string value, cInputType input, string[] options, string[] optionsText)
        {
            TreeNode returnValue = default(TreeNode);
            // Creating a new tParam
            tParam param = new tParam();

            // Initializing it
            param.nodeName = nodeName;
            param.iPort = iPort;
            param.sToolName = sToolName;
            param.sText = text;
            if (value == "")
            {
                value = Configuration.noDataStr;
            }
            param.sValue = value;
            // Setting the input
            param.input = input;
            param.enabled = true;
            param.optionsValue = options;
            param.optionsText = optionsText;
            // add node
            var oNewNode = new TreeNode();
            oNewNode.Tag = param;
            oNewNode.ImageKey = "_none";
            oNewNode.StateImageKey = "_none";
            oNewNode.SelectedImageKey = "_none";
            updateNodeTextBasedOnParam(oNewNode);
            if (input == cInputType.FIX)
            {
                oNewNode.ForeColor = textColorLocked;
            }

            if (oParentNode != null)
            {
                oParentNode.Nodes.Add(oNewNode);
            }
            else
            {
                TreeView1.Nodes.Add(oNewNode);
            }

            returnValue = oNewNode;
            return returnValue;
        }

        public void addNodeAttribute(TreeNode oNode, string attribName, string attribValue)
        {
            tParam param = (tParam)oNode.Tag;
            if (ReferenceEquals(param.attribParams, null))
            {
                param.attribParams = new string[1];
                param.attribValues = new string[1];
            }
            else
            {
                Array.Resize(ref param.attribParams, param.attribParams.Count() + 1);
                Array.Resize(ref param.attribValues, param.attribValues.Count() + 1);
            }
            param.attribParams[param.attribParams.Count() - 1] = attribName;
            param.attribValues[param.attribParams.Count() - 1] = attribValue;
            if (attribName == EnabledAttribName)
            {
                updateNodeTextBasedOnParam(oNode);
            }
        }

        public bool getNode(TreeNodeCollection oNodes, int iPort, string sToolName, string nodeName, ref TreeNode retNode, bool bSubNodes = false)
        {
            retNode = null;
            if (ReferenceEquals(oNodes, null))
            {
                oNodes = TreeView1.Nodes;
            }
            foreach (TreeNode node in oNodes)
            {
                if (((tParam)node.Tag).iPort == iPort && ((tParam)node.Tag).sToolName == sToolName && ((tParam)node.Tag).nodeName == nodeName)
                {
                    retNode = node;
                    return true;
                }
                else
                {
                    if (bSubNodes && node.Nodes.Count > 0)
                    {
                        if (getNode(node.Nodes, iPort, sToolName, nodeName, ref retNode, bSubNodes))
                        {
                            return true;
                        }
                    }
                }
            }
            retNode = null;
            return false;
        }

        public bool getNodeByName(TreeNodeCollection oNodes, string nodeName, ref TreeNode retNode, bool bSubNodes = false)
        {
            retNode = null;
            if (ReferenceEquals(oNodes, null))
            {
                oNodes = TreeView1.Nodes;
            }
            foreach (TreeNode node in oNodes)
            {
                if (((tParam)node.Tag).nodeName == nodeName)
                {
                    retNode = node;
                    return true;
                }
                else
                {
                    if (bSubNodes && node.Nodes.Count > 0)
                    {
                        if (getNodeByName(node.Nodes, nodeName, ref retNode, bSubNodes))
                        {
                            return true;
                        }
                    }
                }
            }
            retNode = null;
            return false;
        }

        public bool getNodesName(TreeNodeCollection oNodes, string nodeName, ref List<TreeNode> retNodes, bool bSubNodes = false)
        {

            bool bFound = false;
            retNodes = new List<TreeNode>();

            if (ReferenceEquals(oNodes, null))
            {
                oNodes = TreeView1.Nodes;
            }
            foreach (TreeNode node in oNodes)
            {
                if (((tParam)node.Tag).nodeName == nodeName)
                {
                    retNodes.Add(node);
                    bFound = true;
                }
                else
                {
                    if (bSubNodes && node.Nodes.Count > 0)
                    {
                        if (getNodesName(node.Nodes, nodeName, ref retNodes, bSubNodes))
                        {
                            bFound = true;
                        }
                    }
                }
            }
            return bFound;
        }

        // Gets a param value
        public bool getValue(TreeNodeCollection oNodes, int iPort, string sToolName, string nodeName, ref string retValue, ref TreeNode retNode, bool bSubNodes = false)
        {

            TreeNode node = null;
            if (getNode(oNodes, iPort, sToolName, nodeName, ref node, bSubNodes))
            {
                retValue = ((tParam)node.Tag).sValue;
                retNode = node;
                return true;
            }
            retValue = "";
            retNode = node;
            return false;
        }

        public bool getValueNodeByName(TreeNodeCollection oNodes, string nodeName, ref string retValue, ref TreeNode retNode, bool bSubNodes = false)
        {

            TreeNode node = null;
            if (getNodeByName(oNodes, nodeName, ref node, bSubNodes))
            {
                retValue = ((tParam)node.Tag).sValue;
                retNode = node;
                return true;
            }
            retValue = "";
            retNode = node;
            return false;
        }

        // Gets a attribute value
        public bool getAttribValue(TreeNodeCollection oNodes, int iPort, string sToolName, string nodeName, string attribName, ref string retAttribValue, bool bSubNodes = false)
        {

            TreeNode node = null;
            if (getNode(oNodes, iPort, sToolName, nodeName, ref node, bSubNodes))
            {
                if (((tParam)node.Tag).attribParams != null)
                {
                    for (var i = 0; i <= ((tParam)node.Tag).attribParams.Count() - 1; i++)
                    {
                        if (((tParam)node.Tag).attribParams[(int)i] == attribName)
                        {
                            retAttribValue = System.Convert.ToString(((tParam)node.Tag).attribValues[(int)i]);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool getNodeAttribValue(TreeNode node, string attribName, ref string retAttribValue)
        {
            if (node != null)
            {
                if (((tParam)node.Tag).attribParams != null)
                {
                    for (var i = 0; i <= ((tParam)node.Tag).attribParams.Count() - 1; i++)
                    {
                        if (((tParam)node.Tag).attribParams[(int)i] == attribName)
                        {
                            retAttribValue = System.Convert.ToString(((tParam)node.Tag).attribValues[(int)i]);
                            return true;
                        }
                    }
                }
            }
            retAttribValue = "";
            return false;
        }

        // Sets a param value (use getNode to search for)
        public void setNodeValue(TreeNode node, string sValue)
        {
            ((tParam)node.Tag).sValue = sValue;
            updateNodeTextBasedOnParam(node);
        }

        // Sets an attribute value (use getNode to search for)
        public void setNodeAttribValue(TreeNode node, string attribName, string sValue)
        {
            if (((tParam)node.Tag).attribParams != null)
            {
                for (var i = 0; i <= ((tParam)node.Tag).attribParams.Count() - 1; i++)
                {
                    if (((tParam)node.Tag).attribParams[(int)i] == attribName)
                    {
                        ((tParam)node.Tag).attribValues[(int)i] = sValue;
                        if (attribName == EnabledAttribName)
                        {
                            updateNodeTextBasedOnParam(node);
                        }
                    }
                }
            }
        }

        // Sets a param text (use getNode to search for)
        public void setNodeText(TreeNode node, string sText, string[] optionsText, bool bSubNodes = false)
        {
            ((tParam)node.Tag).sText = sText;
            if ((((tParam)node.Tag).input == cInputType.NUMBER | ((tParam)node.Tag).input == cInputType.FIX | ((tParam)node.Tag).input == cInputType.FIX_NODE) && optionsText != null)
            {
                // NUMBER with parsing string
                ((tParam)node.Tag).optionsValue = optionsText;
            }
            else if (((tParam)node.Tag).input == cInputType.SWITCH & optionsText != null)
            {
                // SWITCH with separated text and values options
                ((tParam)node.Tag).optionsText = optionsText;
            }
            updateNodeTextBasedOnParam(node);
        }

        private void updateNodeTextBasedOnParam(TreeNode node)
        {
            if ((((tParam)node.Tag).input == cInputType.NUMBER |
                 ((tParam)node.Tag).input == cInputType.FIX |
                 ((tParam)node.Tag).input == cInputType.FIX_NODE) && ((tParam)node.Tag).optionsValue != null)
            {
                // NUMBER or FIX with parsing string
                // modify if value is blank and there is a value for replace it
                if (((tParam)node.Tag).sValue == "" && ((tParam)node.Tag).optionsValue.Length > 4)
                {
                    if (((tParam)node.Tag).optionsValue[4] != "")
                    {
                        ((tParam)node.Tag).sValue = System.Convert.ToString(((tParam)node.Tag).optionsValue[4]);
                    }
                }
                if (((tParam)node.Tag).optionsValue[0] != "")
                {
                    node.Text = ((tParam)node.Tag).sText + Localization.getResStr(Configuration.gralValueSeparatorId) + ((tParam)node.Tag).optionsValue[0].Replace(Configuration.sReplaceTag, ((tParam)node.Tag).sValue);
                }
                else
                {
                    node.Text = ((tParam)node.Tag).sText + Localization.getResStr(Configuration.gralValueSeparatorId) + ((tParam)node.Tag).sValue;
                }
            }
            else if (((tParam)node.Tag).input == cInputType.SWITCH & ((tParam)node.Tag).optionsText != null)
            {
                // SWITCH with separated text and values options
                int idx = Array.IndexOf(((tParam)node.Tag).optionsValue, ((tParam)node.Tag).sValue);
                if (idx >= 0)
                {
                    node.Text = ((tParam)node.Tag).sText + Localization.getResStr(Configuration.gralValueSeparatorId) + ((tParam)node.Tag).optionsText[idx];
                }
                else
                {
                    node.Text = ((tParam)node.Tag).sText + Localization.getResStr(Configuration.gralValueSeparatorId) + ((tParam)node.Tag).sValue;
                }
            }
            else if (((tParam)node.Tag).input == cInputType.NO_VALUE)
            {
                node.Text = ((tParam)node.Tag).sText;
            }
            else
            {
                // ANY OTHER
                // modify if value is blank and there is a value for replace it
                if (((tParam)node.Tag).optionsValue != null)
                {
                    if (((tParam)node.Tag).sValue == "" && ((tParam)node.Tag).optionsValue.Length > 4)
                    {
                        if (((tParam)node.Tag).optionsValue[4] != "")
                        {
                            ((tParam)node.Tag).sValue = System.Convert.ToString(((tParam)node.Tag).optionsValue[4]);
                        }
                    }
                }
                node.Text = ((tParam)node.Tag).sText + Localization.getResStr(Configuration.gralValueSeparatorId) + ((tParam)node.Tag).sValue;
            }

            // show enabled icon
            string sEnabledState = "";
            string sImageKey = "";
            if (getNodeAttribValue(node, EnabledAttribName, ref sEnabledState))
            {
                switch (sEnabledState)
                {
                    case EnabledTrue:
                        sImageKey = "_on";
                        break;
                    case EnabledFalse:
                        sImageKey = "_off";
                        break;
                    default:
                        sImageKey = "_none";
                        break;
                }
                node.ImageKey = sImageKey;
                node.SelectedImageKey = sImageKey;
                node.StateImageKey = sImageKey;
            }

        }

        // clear data
        public void clearData(TreeNodeCollection oNodes = default(TreeNodeCollection))
        {
            if (ReferenceEquals(oNodes, null))
            {
                oNodes = TreeView1.Nodes;
            }
            oNodes.Clear();
        }

        private bool bEditMode = false;
        private TreeNode curEditingNode = null; // current editing node
        private TreeNode curInputNode = null; // node being edited with edit control
        private TreeNode nextInputNode = null;
        private bool bNodeCheck = false; // clicked node but over check box
        private TreeNode nodeLastCopied;
        private Color colorLastCopied; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private bool bPasteOnlyChecked = true;

        // resize event
        protected override void OnResize(System.EventArgs e)
        {
            PanelTable.Size = base.Size;
            // when resizing control, resize PanelTable to leave space for vertical scroll
            PanelTable.Width = base.Width - 5;
            base.OnResize(e);

        }

        private void mySetNodeCheckChildren(TreeNode node, bool bChecked, bool bChildren)
        {
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode childnode in node.Nodes)
                {
                    childnode.Checked = bChecked;
                    if (bChildren && childnode.Nodes.Count > 0)
                    {
                        mySetNodeCheckChildren(childnode, bChecked, bChildren);
                    }
                }
            }
        }

        private void mySetNodeCheckParent(TreeNode node, bool bChecked, bool bParents)
        {
            bool bSomeChecked = false;
            if (node.Parent != null)
            {
                if (!bChecked)
                {
                    // analize if there is any sibling node checked
                    if (node.Parent.Nodes.Count > 0)
                    {
                        foreach (TreeNode childnode in node.Parent.Nodes)
                        {
                            if (childnode.Checked)
                            {
                                bSomeChecked = true;
                            }
                        }
                    }
                    node.Parent.Checked = bSomeChecked;
                    if (bParents && node.Parent.Parent != null)
                    {
                        mySetNodeCheckParent(node.Parent, bSomeChecked, bParents);
                    }
                }
                else
                {
                    // as node is checked, check parent
                    node.Parent.Checked = true;
                    if (bParents && node.Parent.Parent != null)
                    {
                        mySetNodeCheckParent(node.Parent, bChecked, bParents);
                    }
                }
            }
        }

        bool bIsCheckingChildrenAndParents = false;
        bool bIsMovingOverImage = false;

        public void TreeView1_BeforeCheck(System.Object sender, System.Windows.Forms.TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            // cannot check FIX settings (FIX nodes with no children)
            if (((tParam)node.Tag).input == cInputType.FIX & node.Nodes.Count == 0)
            {
                e.Cancel = true;
            }
            if (bIsMovingOverImage)
            {
                e.Cancel = true;
            }
        }

        public void TreeView1_AfterCheck(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            bNodeCheck = true;
            TreeNode node = e.Node;
            //RaiseEvent Log("AfterCheck: " & e.Node.Checked.ToString)
            if (!bIsCheckingChildrenAndParents)
            {
                bIsCheckingChildrenAndParents = true;
                mySetNodeCheckChildren(node, node.Checked, true);
                mySetNodeCheckParent(node, node.Checked, true);
                bIsCheckingChildrenAndParents = false;
            }
        }

        public void TreeView1_AfterSelect(System.Object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            //if changed selected, edit next click in this node
            curEditingNode = e.Node;

            //if there is a copied node, analize and set menues
            tParam paramTarget = (tParam)curEditingNode.Tag;
            mnuTreeCopyNode.Enabled = false;
            mnuTreePasteNode.Enabled = false;
            mnuTreePasteNodeChecked.Enabled = false;
            // copy
            if (paramTarget.nodeName == Configuration.xmlPortId || paramTarget.nodeName == Configuration.xmlToolId)
            {
                mnuTreeCopyNode.Enabled = true;
            }
            // paste
            if (nodeLastCopied != null)
            {
                pasteNodeChildrenValuesChecked(nodeLastCopied, curEditingNode, true, false, false);
            }
        }

        private void mnuContextMenuTree_KeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control)
            {
                bMnuPasteSettingsAndChecks = !bMnuPasteSettingsAndChecks;
                // actúa como switch
                if (bMnuPasteSettingsAndChecks)
                {
                    mnuTreePasteNode.Text = Localization.getResStr(Configuration.confMnuPasteAndCheckId);
                    mnuTreePasteNodeChecked.Text = Localization.getResStr(Configuration.confMnuPasteAndCheckCheckedId);
                }
                else
                {
                    mnuTreePasteNode.Text = Localization.getResStr(Configuration.confMnuPasteId);
                    mnuTreePasteNodeChecked.Text = Localization.getResStr(Configuration.confMnuPasteCheckedId);
                }
            }
        }

        private void mnuContextMenuTree_KeyUp(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //bMnuPasteSettingsAndChecks = False
            //mnuTreePasteNode.Text = getResStr(confMnuPasteId)
            //mnuTreePasteNodeChecked.Text = getResStr(confMnuPasteCheckedId)
        }

        public void TreeView1_KeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control)
            {
                bControlDown = true;
                if (((TreeView)sender).Cursor == Configuration.cursor_switch_plus)
                {
                    ((TreeView)sender).Cursor = Configuration.cursor_switch_minus;
                }
            }
        }

        public void TreeView1_KeyUp(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bControlDown = false;
            if (((TreeView)sender).Cursor == Configuration.cursor_switch_minus)
            {
                ((TreeView)sender).Cursor = Configuration.cursor_switch_plus;
            }
        }

        public void TreeView1_MouseMove(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            TreeNode node = ((TreeView)sender).GetNodeAt(e.Location);
            bIsMovingOverImage = false;
            if (node != null)
            {
                tParam param = (tParam)node.Tag;
                Rectangle tmpBounds = node.Bounds;
                tmpBounds.Offset(adjustLeftPixelsCursor * -1, 0);
                // change cursor, only if mouse into node text bounds
                if (tmpBounds.Contains(e.X, e.Y))
                {
                    if (param.input == cInputType.SWITCH)
                    {
                        if (bControlDown)
                        {
                            ((TreeView)sender).Cursor = Configuration.cursor_switch_minus;
                        }
                        else
                        {
                            ((TreeView)sender).Cursor = Configuration.cursor_switch_plus;
                        }
                    }
                    else if (param.input != cInputType.FIX & param.input != cInputType.FIX_NODE & param.input != cInputType.NO_VALUE)
                    {
                        ((TreeView)sender).Cursor = Configuration.cursor_hand;
                    }
                    else
                    {
                        ((TreeView)sender).Cursor = Cursors.Default;
                    }
                }
                else
                {
                    // zona de icono
                    tmpBounds.Offset((ImageListParamTree.ImageSize.Width) * -1, 0);
                    tmpBounds.Width = ImageListParamTree.ImageSize.Width;
                    if (tmpBounds.Contains(e.X, e.Y))
                    {
                        bIsMovingOverImage = true; // si está en zona de icono, no dejar seleccionar checked (BeforeCheck)
                        if (node.ImageKey == "_none")
                        {
                            ((TreeView)sender).Cursor = Cursors.Default;
                        }
                        else
                        {
                            ((TreeView)sender).Cursor = Configuration.cursor_enable;
                        }
                    }
                    else
                    {
                        ((TreeView)sender).Cursor = Cursors.Default;
                    }
                }
            }
            else
            {
                ((TreeView)sender).Cursor = Cursors.Default;
            }

        }

        public void TreeView1_NodeMouseClick(System.Object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e)
        {
            TreeNode node = e.Node;
            bool bEditThisNode = false;

            // if already selected and not clicked checked/unchecked
            if (node.IsSelected && !bNodeCheck)
            {
                if (ReferenceEquals(curEditingNode, null))
                {
                    curEditingNode = node;
                }
                else
                {
                    if (node.Equals(curEditingNode))
                    {
                        bEditThisNode = true;
                    }
                    else
                    {
                        curEditingNode = node;
                    }
                }

                // if already editing, close previous edit
                if (curInputNode != null)
                {
                    // close editing
                    myEditClose();
                }

            }
            // force node check mark = false
            bNodeCheck = false;

            if (bEditMode && bEditThisNode)
            {
                // editar texto o enabled

                Rectangle tmpBounds = node.Bounds;
                tmpBounds.Offset(adjustLeftPixelsCursor * -1, 0);
                if (e.X >= tmpBounds.Left)
                {
                    // en zona de texto

                    // Depending on the input type
                    tParam param = (tParam)node.Tag;
                    //Console.WriteLine(param.enabled)
                    if (param.enabled)
                    {
                        if ((param.input == cInputType.TEXT | param.input == cInputType.NUMBER) && (e.Button == System.Windows.Forms.MouseButtons.Left))
                        {
                            myEditNode(node);
                        }
                        else if (param.input == cInputType.SWITCH)
                        {
                            // Moving to the next option
                            int curOption = Array.IndexOf(param.optionsValue, param.sValue);
                            // found value
                            if (curOption >= 0)
                            {
                                if (e.Button == System.Windows.Forms.MouseButtons.Left && !bControlDown)
                                {
                                    curOption++;
                                    if (curOption > param.optionsValue.Length - 1)
                                    {
                                        curOption = 0;
                                    }
                                    param.sValue = param.optionsValue[curOption];
                                    updateNodeTextBasedOnParam(node);
                                }
                                if (e.Button == System.Windows.Forms.MouseButtons.Left && bControlDown)
                                {
                                    curOption--;
                                    if (curOption < 0)
                                    {
                                        curOption = param.optionsValue.Length - 1;
                                    }
                                    param.sValue = param.optionsValue[curOption];
                                    updateNodeTextBasedOnParam(node);
                                }
                            }
                            else
                            {
                                param.sValue = param.optionsValue[0];
                                updateNodeTextBasedOnParam(node);
                            }
                        }
                    }
                    else
                    {
                        curInputNode = null;
                    }

                }
                else
                {
                    // zona de icono enabled
                    tmpBounds.Offset((ImageListParamTree.ImageSize.Width) * -1, 0);
                    tmpBounds.Width = ImageListParamTree.ImageSize.Width;
                    string sAttribValue = "";
                    if (tmpBounds.Contains(e.X, e.Y))
                    {
                        switch (node.ImageKey)
                        {
                            case "_on":
                                sAttribValue = EnabledFalse;
                                break;
                            case "_off":
                                sAttribValue = EnabledTrue;
                                break;
                            default:
                                break;
                        }
                        if (!string.IsNullOrEmpty(sAttribValue))
                        {
                            setNodeAttribValue(node, EnabledAttribName, sAttribValue);
                            updateNodeTextBasedOnParam(node);
                        }
                    }
                }

            }

            // force selected node to select node when clic the right button
            TreeView1.SelectedNode = e.Node;

        }

        private void myEditNode(TreeNode node)
        {
            tParam param = (tParam)node.Tag;
            // if already editing, close previous edit
            if (curInputNode != null)
            {
                myEditClose();
            }
            curInputNode = node;
            tbEdit.Text = "";
            if (param.input == cInputType.NUMBER)
            {
                if (param.sValue == Configuration.noDataStr)
                {
                    tbEdit.Text = "";
                }
                else
                {
                    tbEdit.Text = param.sValue;
                }
            }
            else
            {
                if (param.sValue == Configuration.noDataStr)
                {
                    tbEdit.Text = "";
                }
                else
                {
                    tbEdit.Text = param.sValue;
                }
            }
            tbEdit.Top = node.Bounds.Top;
            tbEdit.Left = node.Bounds.Right;
            tbEdit.Visible = true;
            tbEdit.SelectionLength = tbEdit.TextLength;
            tbEdit.BringToFront();
            //tbEdit.Focus()

        }

        private void myEditClose()
        {
            if (curInputNode != null)
            {
                tParam param = (tParam)curInputNode.Tag;
                if (param.sValue != tbEdit.Text && !(param.sValue == Configuration.noDataStr && tbEdit.Text == ""))
                {
                    param.sValue = tbEdit.Text;
                    updateNodeTextBasedOnParam(curInputNode);
                }
                tbEdit.Visible = false;
                curInputNode = null;
            }
        }

        public void tbEdit_KeyPress(System.Object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            tParam param = default(tParam);
            if (curInputNode != null)
            {
                param = (tParam)curInputNode.Tag;
                if (e.KeyChar == Microsoft.VisualBasic.Strings.ChrW((System.Int32)Keys.Enter))
                {
                    myEditClose();
                }
                else if (e.KeyChar == Microsoft.VisualBasic.Strings.ChrW((System.Int32)Keys.Escape))
                {
                    if (param.sValue == Configuration.noDataStr)
                    {
                        tbEdit.Text = "";
                    }
                    else
                    {
                        tbEdit.Text = param.sValue;
                    }
                }
                else
                {
                    if (param.input == cInputType.NUMBER)
                    {
                        // only numbers and backspace
                        e.Handled = !RoutinesLibrary.Data.DataType.IntegerUtils.KeyIsNumber(e.KeyChar);
                    }
                }
            }
        }

        public void tbEdit_Validating(System.Object sender, System.ComponentModel.CancelEventArgs e)
        {
            tParam param = default(tParam);
            int iValueMin = 0;
            int iValueMax = 0;
            string sValueMin = "";
            string sValueMax = "";
            string sAllowedValues = "";
            bool bValueExists = false;
            string sValue = "";

            if (curInputNode != null)
            {
                param = (tParam)curInputNode.Tag;
            }
            else
            {
                return;
            }
            sValue = Strings.Trim(((TextBox)sender).Text);

            // check allowed values and limits, if any
            if (param.optionsValue != null)
            {
                // min value
                if (param.optionsValue.Length > 1)
                {
                    try
                    {
                        sValueMin = param.optionsValue[1].Trim();
                        iValueMin = int.Parse(sValueMin);
                    }
                    catch (Exception)
                    {
                        iValueMin = 0;
                    }
                }
                // max value
                if (param.optionsValue.Length > 2)
                {
                    try
                    {
                        sValueMax = param.optionsValue[2].Trim();
                        iValueMax = int.Parse(param.optionsValue[2]);
                    }
                    catch (Exception)
                    {
                        iValueMax = 0;
                    }
                }
                // allowed values
                if (param.optionsValue.Length > 3)
                {
                    sAllowedValues = param.optionsValue[3].Trim();
                }

                // if allowed values (and no limits)
                if (!string.IsNullOrEmpty(sAllowedValues))
                {
                    if (("#" + sAllowedValues + "#").IndexOf("#" + sValue + "#") + 1 > 0)
                    {
                        bValueExists = true;
                    }
                    // value not allowed and no limits defined
                    if (!bValueExists && string.IsNullOrEmpty(sValueMin) && string.IsNullOrEmpty(sValueMax))
                    {
                        MessageBox.Show(string.Format(Localization.getResStr(Configuration.paramsAllowedValuesId), sAllowedValues));
                        e.Cancel = true;
                        return;
                    }
                }

                // if limits
                try
                {
                    if (!bValueExists && (!string.IsNullOrEmpty(sValueMin) || !string.IsNullOrEmpty(sValueMax)))
                    {
                        if (!string.IsNullOrEmpty(sValueMin) && !string.IsNullOrEmpty(sValueMax))
                        {
                            if (int.Parse(sValue) < iValueMin || int.Parse(sValue) > iValueMax)
                            {
                                MessageBox.Show(string.Format(Localization.getResStr(Configuration.paramsLimitsErrorId), iValueMin.ToString(), iValueMax.ToString()));
                                e.Cancel = true;
                                return;
                            }
                        }
                        else if (!string.IsNullOrEmpty(sValueMin))
                        {
                            if (int.Parse(sValue) < iValueMin)
                            {
                                MessageBox.Show(string.Format(Localization.getResStr(Configuration.paramsMinLimitErrorId), iValueMin.ToString()));
                                e.Cancel = true;
                                return;
                            }
                        }
                        else if (!string.IsNullOrEmpty(sValueMax))
                        {
                            if (int.Parse(sValue) < iValueMin || int.Parse(((TextBox)sender).Text) > iValueMax)
                            {
                                MessageBox.Show(string.Format(Localization.getResStr(Configuration.paramsMaxLimitErrorId), iValueMax.ToString()));
                                e.Cancel = true;
                                return;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            }
            //myEditClose()
        }


        public void mnuTreeExpandAll_Click(System.Object sender, System.EventArgs e)
        {
            TreeView1.ExpandAll();
        }

        public void mnuTreeCollapseAll_Click(System.Object sender, System.EventArgs e)
        {
            TreeView1.CollapseAll();
        }

        public void mnuTreeExpandNodeAndChildren_Click(System.Object sender, System.EventArgs e)
        {
            TreeView1.SelectedNode.ExpandAll();
        }

        public void mnuTreeCollapseNodeAndChildren_Click(System.Object sender, System.EventArgs e)
        {
            TreeView1.SelectedNode.Collapse();
        }

        public void mnuTreeCopyNode_Click(System.Object sender, System.EventArgs e)
        {
            TreeNode node = TreeView1.SelectedNode;
            Font myfont = null;

            nodeLastCopied = node;
            nodeLastCopied.BackColor = colorLastCopied;
            //myfont = New Font(TreeView1.Font.FontFamily, TreeView1.Font.Size, FontStyle.Bold And FontStyle.Italic)
            //nodeLastCopied.NodeFont = myfont

        }

        public void mnuTreePasteNode_Click(System.Object sender, System.EventArgs e)
        {
            TreeNode node = TreeView1.SelectedNode;
            //If bMnuPasteSettingsAndChecks Then MsgBox("SettingsAndChecks")
            if (nodeLastCopied != null)
            {
                if (ReferenceEquals(((ToolStripMenuItem)sender), mnuTreePasteNode))
                {
                    pasteNodeChildrenValuesChecked(nodeLastCopied, node, false, false, bMnuPasteSettingsAndChecks);
                }
                else if (ReferenceEquals(((ToolStripMenuItem)sender), mnuTreePasteNodeChecked))
                {
                    pasteNodeChildrenValuesChecked(nodeLastCopied, node, false, true, bMnuPasteSettingsAndChecks);
                }
            }
        }

        private void pasteNodeChildrenValuesChecked(TreeNode parentSource, TreeNode parentTarget, bool bSetPasteMenus, bool bOnlyChecked, bool bSetChecked)
        {
            List<TreeNode> retNodes = null;
            TreeNode parentSourceClone = (TreeNode)parentSource.Clone();
            tParam paramParentSrc = (tParam)parentSourceClone.Tag;
            tParam paramParentTrg = (tParam)parentTarget.Tag;
            tParam paramToolSrc = null;
            tParam paramToolTrg = null;
            tParam paramSettingSrc = null;
            string sSourceToolName = "";
            string sTargetToolName = "";
            // opciones:
            // Source: port
            // port->port (same port settings to same port settings and same tools to same tools): Paste from tools and settings of Port {0} to Port {1}
            // port->tool: from same tool in parent port to target tool: Paste from Tool {0} settings of Port {1} to Tool {2} of Port {3}
            // port->all other ports (portsandtools): Paste from tools and settings of Port {0} to all other Ports

            // Source: tool
            // tool->port: tool to same tool in target port: Paste from Tool {0} settings of Port {1} to Tool {2} in Port {3}
            // tool->tool (any tool): Paste from Tool {0} settings of Port {1} to Tool {2} in Port {3}
            // tool->same tool in all other ports (portsandtools): Paste from Tool {0} settings of Port {1} to Tool {2} in all other Ports

            // Mark all tool settings as this tool in this port/in all ports
            // con control o Alt se podría seleccionar el mismo tool setting en todo el puerto

            if (bSetPasteMenus)
            {
                mnuTreePasteNode.ToolTipText = "";
                mnuTreePasteNodeChecked.ToolTipText = mnuTreePasteNode.ToolTipText;
            }

            if (paramParentSrc.nodeName == Configuration.xmlPortId)
            {
                // source: port
                if (paramParentTrg.nodeName == Configuration.xmlPortId)
                {
                    // port->port (same port settings to same port settings and same tools to same tools)
                    if (bSetPasteMenus)
                    {
                        // set menues
                        // Paste from tools and settings of Port {0} to Port {1}
                        mnuTreePasteNode.ToolTipText = string.Format(Localization.getResStr(Configuration.confHintPort2PortId), paramParentSrc.iPort.ToString(), paramParentTrg.iPort.ToString());
                        mnuTreePasteNodeChecked.ToolTipText = mnuTreePasteNode.ToolTipText;
                        mnuTreePasteNode.Enabled = true;
                        mnuTreePasteNodeChecked.Enabled = true;
                    }
                    else
                    {
                        // do it
                        pastePort2Port(parentSourceClone, parentTarget, bOnlyChecked, bSetChecked);
                    }
                }
                else if (paramParentTrg.nodeName == Configuration.xmlToolId)
                {
                    // port->tool: from same tool in parent port to target tool
                    if (bSetPasteMenus)
                    {
                        // set menues
                        // Paste from Tool {0} settings of Port {1} to Tool {2} of Port {3}
                        mnuTreePasteNode.ToolTipText = string.Format(Localization.getResStr(Configuration.confHintPort2ToolId), paramParentTrg.sToolName.ToString(), paramParentSrc.iPort.ToString(), paramParentTrg.sToolName.ToString(), paramParentTrg.iPort.ToString());
                        mnuTreePasteNodeChecked.ToolTipText = mnuTreePasteNode.ToolTipText;
                        mnuTreePasteNode.Enabled = true;
                        mnuTreePasteNodeChecked.Enabled = true;
                    }
                    else
                    {
                        // do it
                        // get target tool name
                        sTargetToolName = paramParentTrg.sToolName;
                        // search same tool in source port
                        foreach (TreeNode nodeToolSrc in parentSourceClone.Nodes)
                        {
                            paramToolSrc = (tParam)nodeToolSrc.Tag;
                            if (paramToolSrc.nodeName == Configuration.xmlToolId && paramToolSrc.sToolName == sTargetToolName)
                            {
                                // checked tool only
                                if (nodeToolSrc.Checked || !bOnlyChecked)
                                {
                                    pasteTool2Tool(nodeToolSrc, parentTarget, bOnlyChecked, bSetChecked);
                                }
                                else
                                {
                                    // Source tool {0} in port {1} is not checked
                                    MessageBox.Show(string.Format(Localization.getResStr(Configuration.confHintToolNotCheckedId), paramToolSrc.sToolName, paramToolSrc.iPort.ToString()));
                                }
                                break;
                            }
                        }
                    }
                }
                else if (paramParentTrg.nodeName == Configuration.xmlPortsAndToolsId)
                {
                    // port->all other ports
                    if (bSetPasteMenus)
                    {
                        // Paste from tools and settings of Port {0} to all other Ports
                        mnuTreePasteNode.ToolTipText = string.Format(Localization.getResStr(Configuration.confHintPort2AllPortsId), paramParentSrc.iPort.ToString());
                        mnuTreePasteNodeChecked.ToolTipText = mnuTreePasteNode.ToolTipText;
                        mnuTreePasteNode.Enabled = true;
                        mnuTreePasteNodeChecked.Enabled = true;
                    }
                    else
                    {
                        // do it
                        // target ports
                        foreach (TreeNode nodePortTrg in parentTarget.Nodes)
                        {
                            // do not process source port
                            if (nodePortTrg != parentSourceClone)
                            {
                                pastePort2Port(parentSourceClone, nodePortTrg, bOnlyChecked, bSetChecked);
                            }
                        }
                    }
                }
            }
            else if (paramParentSrc.nodeName == Configuration.xmlToolId)
            {
                // source: tool
                if (paramParentTrg.nodeName == Configuration.xmlPortId)
                {
                    // tool->port: tool to same tool in target port
                    if (bSetPasteMenus)
                    {
                        // set menues
                        // Paste from Tool {0} settings of Port {1} to Tool {2} in Port {3}
                        mnuTreePasteNode.ToolTipText = string.Format(Localization.getResStr(Configuration.confHintTool2PortId), paramParentSrc.sToolName.ToString(), paramParentSrc.iPort.ToString(), paramParentSrc.sToolName.ToString(), paramParentTrg.iPort.ToString());
                        mnuTreePasteNodeChecked.ToolTipText = mnuTreePasteNode.ToolTipText;
                        mnuTreePasteNode.Enabled = true;
                        mnuTreePasteNodeChecked.Enabled = true;
                    }
                    else
                    {
                        // do it
                        // get source tool name
                        sSourceToolName = paramParentSrc.sToolName;
                        // checked tool only
                        if (parentSourceClone.Checked || !bOnlyChecked)
                        {
                            // search same tool in target port
                            foreach (TreeNode nodeToolTrg in parentTarget.Nodes)
                            {
                                paramToolTrg = (tParam)nodeToolTrg.Tag;
                                if (paramToolTrg.nodeName == Configuration.xmlToolId && paramToolTrg.sToolName == sSourceToolName)
                                {
                                    pasteTool2Tool(parentSourceClone, nodeToolTrg, bOnlyChecked, bSetChecked);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Source tool {0} in port {1} is not checked.
                            MessageBox.Show(string.Format(Localization.getResStr(Configuration.confHintToolNotCheckedId), paramParentSrc.sToolName, paramParentSrc.iPort.ToString()));
                        }
                    }
                }
                else if (paramParentTrg.nodeName == Configuration.xmlToolId)
                {
                    // tool->tool (any tool)
                    if (bSetPasteMenus)
                    {
                        // set menues
                        // Paste from Tool {0} settings of Port {1} to Tool {2} in Port {3}
                        mnuTreePasteNode.ToolTipText = string.Format(Localization.getResStr(Configuration.confHintTool2ToolId), paramParentSrc.sToolName.ToString(), paramParentSrc.iPort.ToString(), paramParentTrg.sToolName.ToString(), paramParentTrg.iPort.ToString());
                        mnuTreePasteNodeChecked.ToolTipText = mnuTreePasteNode.ToolTipText;
                        mnuTreePasteNode.Enabled = true;
                        mnuTreePasteNodeChecked.Enabled = true;
                    }
                    else
                    {
                        pasteTool2Tool(parentSourceClone, parentTarget, bOnlyChecked, bSetChecked);
                    }
                }
                else if (paramParentTrg.nodeName == Configuration.xmlPortsAndToolsId)
                {
                    // tool->same tool in all other ports
                    if (bSetPasteMenus)
                    {
                        // set menues
                        // Paste from Tool {0} settings of Port {1} to Tool {2} in all other Ports
                        mnuTreePasteNode.ToolTipText = string.Format(Localization.getResStr(Configuration.confHintTool2AllPortsId), paramParentSrc.sToolName.ToString(), paramParentSrc.iPort.ToString(), paramParentSrc.sToolName.ToString());
                        mnuTreePasteNodeChecked.ToolTipText = mnuTreePasteNode.ToolTipText;
                        mnuTreePasteNode.Enabled = true;
                        mnuTreePasteNodeChecked.Enabled = true;
                    }
                    else
                    {
                        // do it
                        sSourceToolName = paramParentSrc.sToolName;
                        // target ports
                        foreach (TreeNode nodePortTrg in parentTarget.Nodes)
                        {
                            // do not process source port (source tool.parent)
                            if (nodePortTrg != parentSourceClone.Parent)
                            {
                                foreach (TreeNode nodeToolTrg in nodePortTrg.Nodes)
                                {
                                    paramToolTrg = (tParam)nodeToolTrg.Tag;
                                    if (paramToolTrg.nodeName == Configuration.xmlToolId && paramToolTrg.sToolName == sSourceToolName)
                                    {
                                        pasteTool2Tool(parentSourceClone, nodeToolTrg, bOnlyChecked, bSetChecked);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        private void pastePort2Port(TreeNode nodePortSrc, TreeNode nodePortTrg, bool bOnlyChecked, bool bSetChecked)
        {
            List<TreeNode> retNodes = null;
            tParam paramToolSrc = null;
            tParam paramToolTrg = null;
            tParam paramSettingSrc = null;
            string sValue = "";
            string sEnabledStatus = "";

            foreach (TreeNode nodeToolSrc in nodePortSrc.Nodes)
            {
                paramToolSrc = (tParam)nodeToolSrc.Tag;
                if (paramToolSrc.nodeName == Configuration.xmlToolId)
                {
                    // tool
                    // checked tool only
                    if (nodeToolSrc.Checked || !bOnlyChecked)
                    {
                        // search same tool node in target port
                        if (getNodesName(nodePortTrg.Nodes, paramToolSrc.nodeName, ref retNodes, false))
                        {
                            foreach (TreeNode nodeToolTrg in retNodes)
                            {
                                paramToolTrg = (tParam)nodeToolTrg.Tag;
                                if (paramToolSrc.sToolName == paramToolTrg.sToolName)
                                {
                                    pasteTool2Tool(nodeToolSrc, nodeToolTrg, bOnlyChecked, bSetChecked);
                                    break;
                                }
                            }
                        }
                    } // tool checked only
                }
                else
                {
                    // port setting
                    if (getNodesName(nodePortTrg.Nodes, paramToolSrc.nodeName, ref retNodes, false))
                    {
                        if (retNodes.Count > 0)
                        {
                            // checked setting only
                            if (nodeToolSrc.Checked || !bOnlyChecked)
                            {
                                // value
                                sValue = ((tParam)nodeToolSrc.Tag).sValue;
                                setNodeValue(retNodes[0], sValue);
                                // enabled
                                sEnabledStatus = "";
                                if (getNodeAttribValue(nodeToolSrc, EnabledAttribName, ref sEnabledStatus))
                                {
                                    setNodeAttribValue(retNodes[0], EnabledAttribName, sEnabledStatus);
                                }
                                // copy checked status
                                if (bSetChecked)
                                {
                                    retNodes[0].Checked = nodeToolSrc.Checked;
                                }
                            } // port setting checked only
                        }
                    }
                }
            }
        }

        private void pasteTool2Tool(TreeNode nodeToolSrc, TreeNode nodeToolTrg, bool bOnlyChecked, bool bSetChecked)
        {
            List<TreeNode> retNodes = null;
            bool bCopy = true;
            tParam paramSettingSrc = null;
            string sValue = "";
            string sEnabledStatus = "";

            //If paramParentSrc.sToolName <> paramParentTrg.sToolName Then
            //    If MsgBox(String.Format("Source Tool {0} is not the same type as Target {1}. ¿Paste anyway?", paramParentSrc.sToolName, paramParentTrg.sToolName), MsgBoxStyle.YesNo) = MsgBoxResult.No Then
            //        bCopy = False
            //    End If
            //End If

            if (bCopy)
            {
                foreach (TreeNode nodeSetting in nodeToolSrc.Nodes)
                {
                    paramSettingSrc = (tParam)nodeSetting.Tag;
                    if (getNodesName(nodeToolTrg.Nodes, paramSettingSrc.nodeName, ref retNodes, false))
                    {
                        if (retNodes.Count > 0)
                        {
                            // checked tool settings only
                            if (nodeSetting.Checked || !bOnlyChecked)
                            {
                                // value
                                sValue = ((tParam)nodeSetting.Tag).sValue;
                                setNodeValue(retNodes[0], sValue);
                                // enabled
                                sEnabledStatus = "";
                                if (getNodeAttribValue(nodeSetting, EnabledAttribName, ref sEnabledStatus))
                                {
                                    setNodeAttribValue(retNodes[0], EnabledAttribName, sEnabledStatus);
                                }
                                // copy checked status
                                if (bSetChecked)
                                {
                                    retNodes[0].Checked = nodeSetting.Checked;
                                }
                            }
                        }
                    }
                }
            }

        }

    }
}
