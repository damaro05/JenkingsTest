// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports




	namespace RoutinesLibrary.Data.Structures
	{
		
		public class MultiDimensionalHashtable
		{
			
			private Hashtable m_internalTable = new Hashtable();
			
			
			public void Clear()
			{
				m_internalTable.Clear();
			}
			
			public void Add(object key, object key2, object val)
			{
				Hashtable htRow = default(Hashtable);
				
				if (m_internalTable.Contains(key))
				{
					htRow = (Hashtable) (m_internalTable[key]);
				}
				else
				{
					htRow = new Hashtable();
					m_internalTable[key] = htRow;
				}
				
				htRow[key2] = val;
			}
			
			public void Remove(object key, object key2)
			{
				if (m_internalTable.Contains(key))
				{
					Hashtable htRow = (Hashtable) (m_internalTable[key]);
					htRow.Remove(key2);
					
					//delete empty row
					if (htRow.Count == 0)
					{
						m_internalTable.Remove(key);
					}
				}
			}
			
			public void RemoveRow(object key)
			{
				m_internalTable.Remove(key);
			}
			
			public dynamic Item(object key, object key2)
			{
				if (m_internalTable.Contains(key))
				{
					if (((Hashtable) (m_internalTable[key])).Contains(key2))
					{
						return ((Hashtable) (m_internalTable[key]))[key2];
					}
				}
				
				return null;
			}
			
			public bool Contains(object key, object key2)
			{
				if (m_internalTable.Contains(key))
				{
					if (((Hashtable) (m_internalTable[key])).Contains(key2))
					{
						return true;
					}
				}
				
				return false;
			}
			
			public bool ContainsRow(object key)
			{
				return m_internalTable.Contains(key);
			}
			
			public ICollection Keys()
			{
				return m_internalTable.Keys;
			}
			
			public ICollection RowKeys(object key)
			{
				if (m_internalTable.Contains(key))
				{
					return ((Hashtable) (m_internalTable[key])).Keys;
				}
				
				return null;
			}
			
		}
		
	}
	
