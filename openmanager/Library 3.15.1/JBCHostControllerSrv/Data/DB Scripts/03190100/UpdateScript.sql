/***
        Añadir WebManager
***/
ALTER TABLE [info] ADD
        [webManagerSwVersion] NVARCHAR(20) NULL,
        [webManagerSwDate] datetime NULL,
        [webManagerSwUrl] NVARCHAR(100) NULL
GO
/* Valores correctos de versión */
UPDATE [info] SET 
        [stationControllerSwVersion] = '3.19.1.0',
        [stationControllerSwDate] = '2016-10-10 00:00:00',
        [stationControllerSwUrl] = ''
GO
UPDATE [info] SET 
        [remoteManagerSwVersion] = '3.19.1.0',
        [remoteManagerSwDate] = '2016-10-10 00:00:00',
        [remoteManagerSwUrl] = ''
GO
UPDATE [info] SET 
        [hostControllerSwVersion] = '3.19.1.0',
        [hostControllerSwDate] = '2016-10-10 00:00:00',
        [hostControllerSwUrl] = ''
GO
UPDATE [info] SET
        [webManagerSwVersion] = '3.19.1.0',
        [webManagerSwDate] = '2016-10-10 00:00:00',
        [webManagerSwUrl] = ''
GO
/***
        Añadir last check update date
**/
ALTER TABLE [info] ADD
        [lastUpdateDate] datetime NULL
GO
/* Valores por omisión */
UPDATE [info] SET
        [lastUpdateDate] = '1900-01-01 00:00:00'
GO
/***
        Elimimnar check periodic info
***/
ALTER TABLE [schedule]
        DROP COLUMN [checkPeriodicModeDaily]
GO
ALTER TABLE [schedule]
        DROP COLUMN [checkPeriodicWeekday]
GO
ALTER TABLE [schedule]
        DROP COLUMN [checkPeriodicHour]
GO
ALTER TABLE [schedule]
        DROP COLUMN [checkPeriodicMinute]
GO
/***
        Tabla de información de notificaciones de actualizaciones
***/
CREATE TABLE [updateNotifications] (
        [emailAvailable] tinyint DEFAULT '0' NULL,
        [emailAddress] NVARCHAR(100) NULL
)
GO
/* Valores por omisión */
INSERT INTO [updateNotifications] VALUES ('0', '')
GO
