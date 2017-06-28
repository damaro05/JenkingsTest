// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports


namespace RoutinesLibrary.Data.DataBase
{
		
	/// <summary>
	/// Provides methods to construct sql queries and get data from queries
	/// </summary>
	public class SQLDataProvider
	{
			
		/// <summary>
		/// Get query to consult if a table exists
		/// </summary>
		/// <param name="table">Table name to consult</param>
		/// <returns>Query</returns>
		public string GetTableExistsQuery(string table)
		{
			return "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '" + table + "'";
		}
			
		/// <summary>
		/// Get a select query with a single key
		/// </summary>
		/// <param name="key">Field key</param>
		/// <param name="table">Table name to consult</param>
		/// <returns>Query</returns>
		public string GetSelectQuery(string key, string table)
		{
			return "SELECT " + key + " FROM " + table;
		}
			
		/// <summary>
		/// Get a select query with multiple keys
		/// </summary>
		/// <param name="keyList">List of keys to get values</param>
		/// <param name="table">Table name to consult</param>
		/// <returns>Query</returns>
		public string GetSelectQuery(string[] keyList, string table)
		{
			string sCommand = "SELECT ";
				
			if (keyList.Length == 0)
			{
				sCommand += "*";
			}
			else
			{
				bool bFirst = true;
				foreach (string key in keyList)
				{
					if (bFirst)
					{
						bFirst = false;
					}
					else
					{
						sCommand += ",";
					}
					sCommand += key;
				}
			}
				
			sCommand += " FROM " + table;
				
			return sCommand;
		}
			
		public string GetInsertQuery(string[] keyList, string table, string[] valueList)
		{
			string sCommand = "INSERT INTO " + table;
				
			bool bFirst = true;
			foreach (string key in keyList)
			{
				if (bFirst)
				{
					bFirst = false;
					sCommand += " (";
				}
				else
				{
					sCommand += ",";
				}
				sCommand += key;
			}
			sCommand += ")";
				
			bFirst = true;
			foreach (string value in valueList)
			{
				if (bFirst)
				{
					bFirst = false;
					sCommand += " VALUES ('";
				}
				else
				{
					sCommand += "','";
				}
				sCommand += value;
			}
			sCommand += "')";
				
			return sCommand;
		}
			
		/// <summary>
		/// Get a update query with a single key
		/// </summary>
		/// <param name="key">Field key</param>
		/// <param name="table">Table name to consult</param>
		/// <param name="value">Value to update</param>
		/// <returns>Query</returns>
		public string GetUpdateQuery(string key, string table, string value)
		{
			return "UPDATE " + table + " SET " + key + "='" + value + "'";
		}
			
		/// <summary>
		/// Get a update query with multiple keys
		/// </summary>
		/// <param name="keyList">List of keys to update values</param>
		/// <param name="table">Table name to consult</param>
		/// <param name="valueList">Value list to update</param>
		/// <returns>Query</returns>
		public string GetUpdateQuery(string[] keyList, string table, string[] valueList)
		{
				
			//Comprobar el numero de parametros recibidos
			if (keyList.Length == 0)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: number of arguments must be greater than 0."));
			}
				
			if (keyList.Length != valueList.Length)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: number of key list and value list arguments differents."));
			}
				
			string sCommand = "UPDATE " + table + " SET ";
				
			bool bFirst = true;
			for (int i = 0; i <= keyList.Length - 1; i++)
			{
				if (bFirst)
				{
					bFirst = false;
				}
				else
				{
					sCommand += ",";
				}
				sCommand += keyList[i] + "='" + valueList[i] + "'";
			}
				
			return sCommand;
		}
			
		/// <summary>
		///  Get data of a query
		/// </summary>
		/// <param name="sqlReader">Data reader</param>
		/// <param name="value">Data result</param>
		/// <returns>True if the operation was succesful</returns>
		public bool GetDataReader(IDataReader sqlReader, ref string value)
		{
			bool bOk = false;
				
			while (sqlReader.Read())
			{
				value = System.Convert.ToString(sqlReader.GetValue(0).ToString());
				bOk = true;
			}
			sqlReader.Close();
				
			return bOk;
		}
			
		/// <summary>
		///  Get data list of a query
		/// </summary>
		/// <param name="sqlReader">Data reader</param>
		/// <param name="valueList">List of data result</param>
		/// <returns>True if the operation was succesful</returns>
		public bool GetDataReader(IDataReader sqlReader, ref string[] valueList)
		{
			bool bOk = false;
				
			while (sqlReader.Read())
			{
				valueList = new string[sqlReader.FieldCount - 1 + 1];
				for (int i = 0; i <= sqlReader.FieldCount - 1; i++)
				{
					valueList[i] = System.Convert.ToString(sqlReader.GetValue(i).ToString());
				}
				bOk = true;
			}
			sqlReader.Close();
				
			return bOk;
		}
			
	}
		
}
