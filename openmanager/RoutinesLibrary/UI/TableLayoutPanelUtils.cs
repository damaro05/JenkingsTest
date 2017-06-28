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
		
	/// <summary>
	/// This class provides utils to use TableLayoutPanel
	/// </summary>
	/// <remarks></remarks>
	public class TableLayoutPanelUtils
	{
			
		/// <summary>
		/// Remove a row from a TableLayoutPanel
		/// </summary>
		/// <param name="panel">TableLayoutPanel to remove row</param>
		/// <param name="rowIndex">Row index to remove</param>
		public static void RemoveRow(TableLayoutPanel panel, int rowIndex)
		{
			int columnIndex = 0;
				
			for (columnIndex = 0; columnIndex <= panel.ColumnCount - 1; columnIndex++)
			{
				Control Control = panel.GetControlFromPosition(columnIndex, rowIndex);
				panel.Controls.Remove(Control);
			}
				
			int i = 0;
			for (i = rowIndex + 1; i <= panel.RowCount - 1; i++)
			{
				columnIndex = 0;
				for (columnIndex = 0; columnIndex <= panel.ColumnCount - 1; columnIndex++)
				{
					Control control = panel.GetControlFromPosition(columnIndex, i);
					panel.SetRow(control, i - 1);
				}
			}
				
			panel.RowCount--;
		}
			
	}
		
}
