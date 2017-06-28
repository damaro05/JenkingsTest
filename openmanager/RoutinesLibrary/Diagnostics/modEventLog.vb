'@Brief Modulo encargado del manejo del log de eventos del sistema
'@Details Modificar las constantes de este modulo para cambiar donde se guarda el log
'@Date 29/07/2015

Option Strict On
Option Explicit On


Friend Module modEventLog


    Friend EvLog As EventLog = Nothing


    '@Brief Create an event log
    '@Return Boolean True if the operation is successful
    Friend Function evLogOpen(Optional ByVal evLogName As String = "Application", Optional ByVal evLogMachine As String = ".") As Boolean

        Dim evLogSource As String = My.Application.Info.AssemblyName

        Try
            If Not EventLog.SourceExists(evLogSource, evLogMachine) Then
                EventLog.CreateEventSource(evLogSource, evLogName)
            End If

            If EvLog Is Nothing Then EvLog = New EventLog(evLogName, evLogMachine, evLogSource)

            evLogWrite("Created event log")

        Catch ex As Exception
            EvLog = Nothing
            Return False
        End Try

        Return True
    End Function

    '@Brief Close the event log
    Friend Sub evLogClose()
        If EvLog IsNot Nothing Then
            EvLog.Close()
            EvLog.Dispose()
            EvLog = Nothing
        End If
    End Sub

    '@Brief Write an entry in the log
    '@Param[in] entry String to write
    Friend Sub evLogWrite(ByVal entry As String)

        If EvLog IsNot Nothing Then EvLog.WriteEntry(entry)
    End Sub

End Module
