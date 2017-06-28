Option Strict On
Option Explicit On

Imports System.Data.SQLite


Namespace RoutinesLibrary
    Namespace Data
        Namespace DataBase
            Namespace SQLite

                Public Class SQLiteConnection
                    Inherits SQLDataProvider

                    Private m_sqlConnection As SQLiteConnection
                    Private m_sqlConnectionString As String = ""
                    Private m_sqlDB As String = ""

                    '@Brief Constructor de la clase. Inicializa la conexión SQL
                    '@Param[in] SQLiteDB Archivo de base de datos
                    '@Param[in] SQLiteParams parámetros adicionales de conexión
                    Public Sub New(ByVal SQLiteDB As String, Optional ByVal SQLiteParams As String = "")
                        m_sqlDB = SQLiteDB
                        m_sqlConnectionString = "Data Source='" & SQLiteDB & "'"
                        If SQLiteParams <> "" Then m_sqlConnectionString = m_sqlConnectionString & ";" & SQLiteParams
                        m_sqlConnection = New SQLiteConnection(m_sqlConnectionString)
                    End Sub

                    '@Brief Crea una BD en blanco
                    Public Function CreateDB() As Boolean
                        ' delete if exists
                        Try
                            IO.File.Delete(m_sqlDB)
                        Catch ex As Exception
                            ' Already exists. Cannot delete.
                            Return False
                        End Try

                        ' create blank DB
                        Try
                            SQLiteConnection.CreateFile(m_sqlDB)
                        Catch ex As Exception
                            ' Cannot create database 
                            Return False
                        End Try
                        Return True
                    End Function

                    Public Sub Dispose()
                        m_sqlConnection.Dispose()
                    End Sub

                    '@Brief Execute a select query in the data base
                    '@Param[in] key Field key
                    '@Param[in] table Name of the data base table to execute the query
                    '@Param[out] value Query result
                    '@Return Boolean True if the operation is successful
                    Public Function SelectQuery(ByVal key As String, ByVal table As String, ByRef value As String) As Boolean

                        Dim bOk As Boolean = False
                        Try
                            Dim sCommand As String = GetSelectQuery(key, table)
                            Dim sqlCommand As SQLiteCommand = New SQLiteCommand(sCommand, m_sqlConnection)

                            m_sqlConnection.Open()
                            Dim sqlReader As IDataReader = sqlCommand.ExecuteReader()
                            bOk = GetDataReader(sqlReader, value)

                        Catch ex As Exception
                            Throw New System.Exception(System.Reflection.MethodInfo.GetCurrentMethod.ToString & " . Error: " & ex.Message)
                        Finally
                            m_sqlConnection.Close()
                        End Try

                        Return bOk
                    End Function

                    '@Brief Execute a select query in the data base
                    '@Param[in] keyList List of keys to get values
                    '@Param[in] table Name of the data base table to execute the query
                    '@Param[out] valueList List of result values of the query
                    '@Return Boolean True if the operation is successful
                    Public Function SelectQuery(ByVal keyList As String(), ByVal table As String, ByRef valueList As String()) As Boolean

                        Dim bOk As Boolean = False
                        Try
                            Dim sCommand As String = GetSelectQuery(keyList, table)
                            Dim sqlCommand As SQLiteCommand = New SQLiteCommand(sCommand, m_sqlConnection)

                            m_sqlConnection.Open()
                            Dim sqlReader As IDataReader = sqlCommand.ExecuteReader()
                            bOk = GetDataReader(sqlReader, valueList)

                        Catch ex As Exception
                            Throw New System.Exception(System.Reflection.MethodInfo.GetCurrentMethod.ToString & " . Error: " & ex.Message)
                        Finally
                            m_sqlConnection.Close()
                        End Try

                        Return bOk
                    End Function

                    '@Brief Execute a update query in the data base
                    '@Param[in] key Field key
                    '@Param[in] table Name of the data base table to execute the query
                    '@Param[out] value Query result
                    '@Return Boolean True if the operation is successful
                    Public Function UpdateQuery(ByVal key As String, ByVal table As String, ByRef value As String) As Boolean

                        Dim bOk As Boolean = False
                        Try
                            Dim sCommand As String = GetUpdateQuery(key, table, value)
                            Dim sqlCommand As SQLiteCommand = New SQLiteCommand(sCommand, m_sqlConnection)

                            m_sqlConnection.Open()
                            Dim sqlResult As Integer = sqlCommand.ExecuteNonQuery()
                            bOk = (sqlResult = 1)

                        Catch ex As Exception
                            Throw New System.Exception(System.Reflection.MethodInfo.GetCurrentMethod.ToString & " . Error: " & ex.Message)
                        Finally
                            m_sqlConnection.Close()
                        End Try

                        Return bOk
                    End Function

                    '@Brief Execute a update query in the data base
                    '@Param[in] keyList List of keys
                    '@Param[in] table Name of the data base table to execute the query
                    '@Param[out] valueList List of values
                    '@Return Boolean True if the operation is successful
                    Public Function UpdateQuery(ByVal keyList As String(), ByVal table As String, ByRef valueList As String()) As Boolean

                        Dim bOk As Boolean = False
                        Try
                            Dim sCommand As String = GetUpdateQuery(keyList, table, valueList)
                            Dim sqlCommand As SQLiteCommand = New SQLiteCommand(sCommand, m_sqlConnection)

                            m_sqlConnection.Open()
                            Dim sqlResult As Integer = sqlCommand.ExecuteNonQuery()
                            bOk = (sqlResult = 1)

                        Catch ex As Exception
                            Throw New System.Exception(System.Reflection.MethodInfo.GetCurrentMethod.ToString & " . Error: " & ex.Message)
                        Finally
                            m_sqlConnection.Close()
                        End Try

                        Return bOk
                    End Function

                    '@Brief Execute a non sql query in the data base
                    '@Param[in] query SQL query to execute
                    '@Return Boolean True if the operation is successful
                    Public Function ExecuteQuery(ByVal query As String) As Boolean

                        Dim bOk As Boolean = False
                        Try
                            Dim sCommand As String = query
                            Dim sqlCommand As SQLiteCommand = New SQLiteCommand(sCommand, m_sqlConnection)

                            m_sqlConnection.Open()
                            Dim sqlResult As Integer = sqlCommand.ExecuteNonQuery()
                            bOk = (sqlResult > 0)

                        Catch ex As Exception
                            Throw New System.Exception(System.Reflection.MethodInfo.GetCurrentMethod.ToString & " . Error: " & ex.Message)
                        Finally
                            m_sqlConnection.Close()
                        End Try

                        Return bOk
                    End Function

                End Class

            End Namespace
        End Namespace
    End Namespace
End Namespace
