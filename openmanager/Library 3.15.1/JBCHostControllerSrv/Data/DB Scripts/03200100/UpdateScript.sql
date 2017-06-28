/***
        Renombrar las tablas
***/
EXEC sp_rename 'schedule', 'updateSchedule'
GO
EXEC sp_rename 'info', 'versionInfo'
GO
EXEC sp_rename 'updateNotifications', 'systemInfo'
GO
/***
        Renombrar columnas de systemInfo
***/
ALTER TABLE [systemInfo] ADD
          [updateNotificationAvailable] tinyint DEFAULT '0' NULL
GO
Update [systemInfo] SET [updateNotificationAvailable] = [emailAvailable]
GO
ALTER TABLE [systemInfo] DROP COLUMN
          [emailAvailable]
GO
ALTER TABLE [systemInfo] ADD
          [updateNotificationEmail] NVARCHAR(100) NULL
GO
Update [systemInfo] SET [updateNotificationEmail] = [emailAddress]
GO
ALTER TABLE [systemInfo] DROP COLUMN
          [emailAddress]
GO
/* Valores correctos de versión */
UPDATE [versionInfo] SET 
        [stationControllerSwVersion] = '3.20.1.0',
        [stationControllerSwDate] = '2017-02-17 00:00:00',
        [stationControllerSwUrl] = ''
GO
UPDATE [versionInfo] SET 
        [remoteManagerSwVersion] = '3.20.1.0',
        [remoteManagerSwDate] = '2017-02-17 00:00:00',
        [remoteManagerSwUrl] = ''
GO
UPDATE [versionInfo] SET 
        [hostControllerSwVersion] = '3.20.1.0',
        [hostControllerSwDate] = '2017-02-17 00:00:00',
        [hostControllerSwUrl] = ''
GO
UPDATE [versionInfo] SET
        [webManagerSwVersion] = '3.20.1.0',
        [webManagerSwDate] = '2017-02-17 00:00:00',
        [webManagerSwUrl] = ''
GO
/***
        Tabla de información de servicios de la red Open Manager
***/
CREATE TABLE [servicesOpenManager] (
        [webManagerUrl] NVARCHAR(100) NULL
)
GO
/* Valores por omisión */
INSERT INTO [servicesOpenManager] VALUES ('')
GO
/***
        Añadir columnas a systemInfo
***/
ALTER TABLE [systemInfo] ADD
        [remoteServerDownloadAvailable] tinyint DEFAULT '1' NULL
GO
ALTER TABLE [systemInfo] ADD
        [remoteServerDownloadLocalFolder] NVARCHAR(200) NULL
GO
/* Valores por omisión */
UPDATE [systemInfo] SET
        [remoteServerDownloadAvailable] = '1'
GO
UPDATE [systemInfo] SET
        [remoteServerDownloadLocalFolder] = ''
GO
/***
        Tabla de información del registro de eventos
***/
CREATE TABLE [eventLog] (
        [date] datetime NULL,
        [softwareVersion] NVARCHAR(20) NULL,
        [loggerLevel] NVARCHAR(20) NULL,
        [message] NVARCHAR(4000) NULL,
        [application] NVARCHAR(50) NULL
)
GO
