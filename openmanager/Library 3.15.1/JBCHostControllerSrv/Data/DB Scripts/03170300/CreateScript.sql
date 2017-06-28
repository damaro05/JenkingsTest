/***
	Tabla de programación de actualizaciones
***/
 CREATE TABLE [schedule] (
        [updatePeriodicAvailable] tinyint DEFAULT '0' NULL,
        [updatePeriodicModeDaily] tinyint DEFAULT '0' NULL,
        [updatePeriodicWeekday] tinyint DEFAULT '1' NULL,
        [updatePeriodicHour] tinyint DEFAULT '0' NULL,
        [updatePeriodicMinute] tinyint DEFAULT '0' NULL,
        [updateSpecificAvailable] tinyint DEFAULT '0' NULL,
        [updateSpecificTime] datetime DEFAULT 'CURRENT_TIMESTAMP' NULL,
        [checkPeriodicAvailable] tinyint DEFAULT '0' NULL,
        [checkPeriodicModeDaily] tinyint DEFAULT '0' NULL,
        [checkPeriodicWeekday] tinyint DEFAULT '1' NULL,
        [checkPeriodicHour] tinyint DEFAULT '0' NULL,
        [checkPeriodicMinute] tinyint DEFAULT '0' NULL
)
GO
/* Valores por omisión */
INSERT INTO [schedule] VALUES('0', '0', '1', '0', '0', '0', '1900-01-01 00:00:00', '0', '0', '1', '0', '0')
GO
/***
	Tabla de información de versiones
***/
CREATE TABLE [info] (
        [stationControllerSwVersion] NVARCHAR(20) NULL,
        [stationControllerSwDate] datetime NULL,
        [stationControllerSwUrl] NVARCHAR(100) NULL,
        [remoteManagerSwVersion] NVARCHAR(20) NULL,
        [remoteManagerSwDate] datetime NULL,
        [remoteManagerSwUrl] NVARCHAR(100) NULL,
        [hostControllerSwVersion] NVARCHAR(20) NULL,
        [hostControllerSwDate] datetime NULL,
        [hostControllerSwUrl] NVARCHAR(100) NULL
)
GO
/* Valores por omisión */
INSERT INTO [info] VALUES ('3.17.3.0', '2016-04-19 00:00:00', '', '3.17.3.0', '2016-04-19 00:00:00', '', '3.17.3.0', '2016-04-19 00:00:00', '')
GO
