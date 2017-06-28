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


namespace RoutinesLibrary.UI
{
		
	public class TreeViewUtils
	{
			
		public static TreeNode AddNode(TreeNode oParentNode, string nodeName, string nodeText)
		{
			// creates a node and, optionally, add to parent
			var oNewNode = new TreeNode();
			oNewNode.Name = nodeName;
			oNewNode.Text = nodeText;
			oNewNode.Tag = null;
			if (oParentNode != null)
			{
				oParentNode.Nodes.Add(oNewNode);
			}
			return oNewNode;
		}
			
		public static TreeNode AddNode(TreeView oParentNode, string nodeName, string nodeText)
		{
			// creates a node and, optionally, add to parent
			var oNewNode = new TreeNode();
			oNewNode.Name = nodeName;
			oNewNode.Text = nodeText;
			oNewNode.Tag = null;
			if (oParentNode != null)
			{
				oParentNode.Nodes.Add(oNewNode);
			}
			return oNewNode;
		}
			
		public static void DeleteNode(ref TreeNode tnode)
		{
			// removes from treeview
			tnode.Remove();
			if (tnode.Tag is Collection)
			{
				((Collection) tnode.Tag).Clear();
			}
			// removes and nothing children
			foreach (TreeNode tnodeChild in tnode.Nodes)
			{
				TreeNode temp_tnode = tnodeChild;
				DeleteNode(ref temp_tnode);
			}
			// nothing the node
			tnode = null;
		}
			
		public static void SetAttrib(TreeNode oNode, string attribName, object attribValue)
		{
			// attributes are saved in treenode Tag property as a Collection of key/value pairs
            Collection attcoll = (Collection)oNode.Tag;
			if (ReferenceEquals(attcoll, null))
			{
				attcoll = new Collection();
			}
			// remove, if exists
			if (attcoll.Contains(attribName))
			{
				attcoll.Remove(attribName);
			}
			attcoll.Add(attribValue, attribName, null, null);
			oNode.Tag = attcoll;
		}
			
		public static dynamic GetAttrib(TreeNode oNode, string attribName)
		{
			// attributes are saved in treenode Tag property as a Collection of key/value pairs
            Collection attcoll = (Collection)oNode.Tag;
			if (ReferenceEquals(attcoll, null))
			{
				return null;
			}
			else
			{
				if (!attcoll.Contains(attribName))
				{
					return "";
				}
				else
				{
					return attcoll[attribName];
				}
			}
		}
			
		public static void RemoveAttrib(TreeNode oNode, string attribName)
		{
            Collection attcoll = (Collection)oNode.Tag;
			if (attcoll != null)
			{
				attcoll.Remove(attribName);
				oNode.Tag = attcoll;
			}
		}
			
		public static TreeNode[] SearchFromKey(string sKey, TreeNodeCollection nodes, bool bSubDirs)
		{
			TreeNode[] aFoundNodes = null;
			if (nodes != null)
			{
				aFoundNodes = nodes.Find(sKey, bSubDirs);
				return aFoundNodes;
			}
			else
			{
				aFoundNodes = new TreeNode[0];
				return aFoundNodes;
			}
		}
			
		public static void ExpandParent(TreeNode node, bool bParents)
		{
			if (node.Parent != null)
			{
				node.Parent.Expand();
				if (bParents && node.Parent.Parent != null)
				{
					ExpandParent(node.Parent, bParents);
				}
			}
		}
			
	}
		
}
