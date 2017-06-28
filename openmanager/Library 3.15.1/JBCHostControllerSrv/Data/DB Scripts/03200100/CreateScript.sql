/***
	Tabla de programación de actualizaciones
***/
 CREATE TABLE [updateSchedule] (
        [updatePeriodicAvailable] tinyint DEFAULT '0' NULL,
        [updatePeriodicModeDaily] tinyint DEFAULT '0' NULL,
        [updatePeriodicWeekday] tinyint DEFAULT '1' NULL,
        [updatePeriodicHour] tinyint DEFAULT '0' NULL,
        [updatePeriodicMinute] tinyint DEFAULT '0' NULL,
        [updateSpecificAvailable] tinyint DEFAULT '0' NULL,
        [updateSpecificTime] datetime DEFAULT 'CURRENT_TIMESTAMP' NULL,
        [checkPeriodicAvailable] tinyint DEFAULT '0' NULL
)
GO
/* Valores por omisión */
INSERT INTO [updateSchedule] VALUES('0', '0', '1', '0', '0', '0', '1900-01-01 00:00:00', '0')
GO
/***
	Tabla de información de versiones
***/
CREATE TABLE [versionInfo] (
        [lastUpdateDate] datetime NULL,
        [stationControllerSwVersion] NVARCHAR(20) NULL,
        [stationControllerSwDate] datetime NULL,
        [stationControllerSwUrl] NVARCHAR(100) NULL,
        [remoteManagerSwVersion] NVARCHAR(20) NULL,
        [remoteManagerSwDate] datetime NULL,
        [remoteManagerSwUrl] NVARCHAR(100) NULL,
        [hostControllerSwVersion] NVARCHAR(20) NULL,
        [hostControllerSwDate] datetime NULL,
        [hostControllerSwUrl] NVARCHAR(100) NULL,
        [webManagerSwVersion] NVARCHAR(20) NULL,
        [webManagerSwDate] datetime NULL,
        [webManagerSwUrl] NVARCHAR(100) NULL
)
GO
/* Valores por omisión */
INSERT INTO [versionInfo] VALUES ('1900-01-01 00:00:00', '3.20.1.0', '2017-02-17 00:00:00', '', '3.20.1.0', '2017-02-17 00:00:00', '', '3.20.1.0', '2017-02-17 00:00:00', '', '3.20.1.0', '2017-02-17 00:00:00', '')
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
        Tabla de información del sistema
***/
CREATE TABLE [systemInfo] (
        [updateNotificationAvailable] tinyint DEFAULT '0' NULL,
        [updateNotificationEmail] NVARCHAR(100) NULL,
        [remoteServerDownloadAvailable] tinyint DEFAULT '1' NULL,
        [remoteServerDownloadLocalFolder] NVARCHAR(200) NULL
)
GO
/* Valores por omisión */
INSERT INTO [systemInfo] VALUES ('0','','1', '')
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
