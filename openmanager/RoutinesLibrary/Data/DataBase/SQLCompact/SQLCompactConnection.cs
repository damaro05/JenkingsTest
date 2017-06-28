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

using System.Data.SqlServerCe;
using System.IO;


namespace RoutinesLibrary.Data.DataBase.SQLCompact
{
					
	public class SQLCompactConnection : SQLDataProvider
	{
						
		private System.Data.SqlServerCe.SqlCeConnection m_sqlConnection;
		private string m_sqlConnectionString = "";
		private string m_sqlDB = "";
						
						
		//@Brief Constructor de la clase.  Inicializa la conexión SQL
		//@Param[in] SQLCeDB Archivo de base de datos
		//@Param[in] SQLCeParams Parámetros adicionales de conexión
		public SQLCompactConnection(string SQLCeDB, string SQLCeParams = "")
		{
			m_sqlDB = SQLCeDB;
			m_sqlConnectionString = "Data Source='" + SQLCeDB + "'";
			if (SQLCeParams != "")
			{
				m_sqlConnectionString = m_sqlConnectionString + ";" + SQLCeParams;
			}
			m_sqlConnection = new SqlCeConnection(m_sqlConnectionString);
		}
						
		public void Dispose()
		{
			m_sqlConnection.Dispose();
		}
						
		//@Brief Crea una BD
		public bool CreateDB()
		{
			// delete if exists
			try
			{
				System.IO.File.Delete(m_sqlDB);
			}
			catch (Exception)
			{
				// Already exists. Cannot delete.
				return false;
			}
							
			// create blank DB
			try
			{
				SqlCeEngine engine = new SqlCeEngine(m_sqlConnectionString);
				engine.CreateDatabase();
			}
			catch (Exception)
			{
				// Cannot create database
				return false;
			}
							
			return true;
		}
						
		public bool TableExists(string table)
		{
			bool bOk = false;
			string value = "";
							
			try
			{
				string sCommand = GetTableExistsQuery(table);
				SqlCeCommand sqlCommand = new SqlCeCommand(sCommand, m_sqlConnection);
								
				m_sqlConnection.Open();
				IDataReader sqlReader = sqlCommand.ExecuteReader();
				bOk = GetDataReader(sqlReader, ref value);
								
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
			finally
			{
				m_sqlConnection.Close();
			}
							
			return bOk;
		}
						
		//@Brief Execute a select query in the data base
		//@Param[in] key Field key
		//@Param[in] table Name of the data base table to execute the query
		//@Param[out] value Query result
		//@Return Boolean True if the operation is successful
		public bool SelectQuery(string key, string table, ref string value)
		{
			bool bOk = false;
			try
			{
				string sCommand = GetSelectQuery(key, table);
				SqlCeCommand sqlCommand = new SqlCeCommand(sCommand, m_sqlConnection);
								
				m_sqlConnection.Open();
				IDataReader sqlReader = sqlCommand.ExecuteReader();
				bOk = GetDataReader(sqlReader, ref value);
								
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
			finally
			{
				m_sqlConnection.Close();
			}
							
			return bOk;
		}
						
		//@Brief Execute a select query in the data base
		//@Param[in] keyList List of keys to get values
		//@Param[in] table Name of the data base table to execute the query
		//@Param[out] valueList List of result values of the query
		//@Return Boolean True if the operation is successful
		public bool SelectQuery(string[] keyList, string table, ref string[] valueList)
		{
			bool bOk = false;
			try
			{
				string sCommand = GetSelectQuery(keyList, table);
				SqlCeCommand sqlCommand = new SqlCeCommand(sCommand, m_sqlConnection);
								
				m_sqlConnection.Open();
				IDataReader sqlReader = sqlCommand.ExecuteReader();
				bOk = GetDataReader(sqlReader, ref valueList);
								
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
			finally
			{
				m_sqlConnection.Close();
			}
							
			return bOk;
		}
						
		public bool InsertQuery(string[] keyList, string table, ref string[] valueList)
		{
			bool bOk = false;
			try
			{
				string sCommand = GetInsertQuery(keyList, table, valueList);
				SqlCeCommand sqlCommand = new SqlCeCommand(sCommand, m_sqlConnection);
								
				m_sqlConnection.Open();
				int sqlResult = sqlCommand.ExecuteNonQuery();
				bOk = sqlResult == 1;
								
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
			finally
			{
				m_sqlConnection.Close();
			}
							
			return bOk;
		}
						
		//@Brief Execute a update query in the data base
		//@Param[in] key Field key
		//@Param[in] table Name of the data base table to execute the query
		//@Param[out] value Query result
		//@Return Boolean True if the operation is successful
		public bool UpdateQuery(string key, string table, string value)
		{
			bool bOk = false;
			try
			{
				string sCommand = GetUpdateQuery(key, table, value);
				SqlCeCommand sqlCommand = new SqlCeCommand(sCommand, m_sqlConnection);
								
				m_sqlConnection.Open();
				int sqlResult = sqlCommand.ExecuteNonQuery();
				bOk = sqlResult == 1;
								
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
			finally
			{
				m_sqlConnection.Close();
			}
							
			return bOk;
		}
						
		//@Brief Execute a update query in the data base
		//@Param[in] keyList List of keys
		//@Param[in] table Name of the data base table to execute the query
		//@Param[out] valueList List of values
		//@Return Boolean True if the operation is successful
		public bool UpdateQuery(string[] keyList, string table, string[] valueList)
		{
			bool bOk = false;
			try
			{
				string sCommand = GetUpdateQuery(keyList, table, valueList);
				SqlCeCommand sqlCommand = new SqlCeCommand(sCommand, m_sqlConnection);
								
				m_sqlConnection.Open();
				int sqlResult = sqlCommand.ExecuteNonQuery();
				bOk = sqlResult == 1;
								
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
			finally
			{
				m_sqlConnection.Close();
			}
							
			return bOk;
		}
						
		//@Brief Execute a non sql query in the data base
		//@Param[in] query SQL query to execute
		//@Return Boolean True if the operation is successful
		public bool ExecuteQuery(string query)
		{
							
			bool bOk = false;
			try
			{
				string sCommand = query;
				SqlCeCommand sqlCommand = new SqlCeCommand(sCommand, m_sqlConnection);
								
				m_sqlConnection.Open();
				int sqlResult = sqlCommand.ExecuteNonQuery();
				bOk = sqlResult > 0;
								
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
			finally
			{
				m_sqlConnection.Close();
			}
							
			return bOk;
		}
						
		public bool ExecuteFileScript(string scriptPath)
		{
			bool bOk = false;
							
			if (File.Exists(scriptPath))
			{
								
				StreamReader objReader = new StreamReader(scriptPath);
				string sTextLine = "";
				string sqlSentence = "";
								
				bOk = true;
								
				//Leemos el documento line by line
				while (objReader.Peek() != -1)
				{
					sTextLine = objReader.ReadLine();
									
					//Fin de una sentencia sql
					if (sTextLine == "GO")
					{
						bOk = bOk && ExecuteQuery(sqlSentence);
						sqlSentence = "";
						goto continueDo;
					}
									
					//Concatenamos la sentencia
					sqlSentence += sTextLine;
continueDo:
					1.GetHashCode() ; //VBConversions note: C# requires an executable line here, so a dummy line was added.
				}
								
				objReader.Close();
			}
							
			return bOk;
		}
						
	}
					
}
