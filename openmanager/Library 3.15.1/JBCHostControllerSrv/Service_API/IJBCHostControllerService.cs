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

using System.ServiceModel;
using System.Runtime.Serialization;
using JBCHostControllerSrv.JBCStationControllerService;

namespace JBCHostControllerSrv
{
    [ServiceContract(Namespace = "http://JBCHostControllerSrv")]
        public interface IJBCHostControllerService
        {

#region HostController Service Management

        [OperationContract(), FaultContract(typeof(faultError))]
            dc_HostController_Info GetHostControllerInfo();

#endregion


#region System Files

            [OperationContract(), FaultContract(typeof(faultError))]
                bool IsAvailableRemoteServerDownload();

            [OperationContract(), FaultContract(typeof(faultError))]
                bool SetAvailableRemoteServerDownload(bool active);

            [OperationContract(), FaultContract(typeof(faultError))]
                string GetUserFilesLocalFolderLocation();

            [OperationContract(), FaultContract(typeof(faultError))]
                bool SetUserFilesLocalFolderLocation(string path);

#endregion


#region Updates

#region Update operations

            //Comprueba si existe una actualización disponible en el servidor remoto
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_InfoUpdateSoftware CheckUpdate();

            //Actualiza el sistema con la versión disponible en el servidor remoto
            [OperationContract(IsOneWay = true)]
                void UpdateSystem();

#endregion


#region Schedule update

#region Specific schedule

            //Devuelve la información de la actualización específica programada
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_InfoUpdateSpecificTime GetUpdateSpecificTime();

            //Programa una actualización específica
            [OperationContract(), FaultContract(typeof(faultError))]
                bool SetUpdateSpecificTime(dc_InfoUpdateSpecificTime infoUpdateSpecificTime);

#endregion

#region Periodic schedule

            //Devuelve la información de la actualización periódica programada
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_InfoUpdatePeriodicTime GetUpdatePeriodicTime();

            //Programa una actualización periódica
            [OperationContract(), FaultContract(typeof(faultError))]
                bool SetUpdatePeriodicTime(dc_InfoUpdatePeriodicTime infoUpdatePeriodicTime);

#endregion

#region Check periodic

            //Devuelve la información de la comprobación periódica programada
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_InfoCheckPeriodicTime GetCheckPeriodicTime();

            //Guarda la información de la comprobación periódica programada
            [OperationContract(), FaultContract(typeof(faultError))]
                bool SetCheckPeriodicTime(dc_InfoCheckPeriodicTime infoCheckPeriodicTime);

#endregion

#endregion


#region Update notification

            //Devuelve la información de la configuración de las notificaciones de actualización
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_UpdateNotifications GetUpdateNotifications();

            //Guarda la información de la configuración de las notificaciones de actualización
            [OperationContract(), FaultContract(typeof(faultError))]
                bool SetUpdateNotifications(dc_UpdateNotifications updateNotifications);

#endregion


#region Services Open Manager

#region Station Controller

            //Comprueba si el Station Controller se tiene que actualizar y lo actualiza en caso necesario
            [OperationContract(IsOneWay = true)]
                void CheckUpdateConnectedStationController(string swVersion);

#endregion

#region Remote Manager

            //Comprueba si el RemoteManager se tiene que actualizar
            [OperationContract(), FaultContract(typeof(faultError))]
                bool CheckUpdateConnectedRemoteManager(string swVersion);

            //Envia la porción de la secuencia del programa
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_UpdateRemoteManager GetFileUpdateRemoteManager(int nSequence);

#endregion

#region Web Manager

            //Comprueba si el WebManager se tiene que actualizar
            [OperationContract(), FaultContract(typeof(faultError))]
                bool CheckUpdateConnectedWebManager(string swVersion);

            //Envia la porción de la secuencia del programa
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_UpdateWebManager GetFileUpdateWebManager(int nSequence);

#endregion

#endregion

#endregion


#region Update firmware

            //Devuelve la información de las versiones software disponibles para un modelo de estación
            [OperationContract(), FaultContract(typeof(faultError))]
                List<dc_FirmwareStation> GetInfoUpdateFirmware(dc_FirmwareStation infoUpdateFirmware);

            //Envia la porción de la secuencia del programa
            [OperationContract(), FaultContract(typeof(faultError))]
                dc_UpdateFirmware GetFileUpdateFirmware(int nSequence, string urlFirmwareSw);

#endregion


#region Services Open Manager

#region Station Controller

            //Obtiene la versión de software del StationController en local
            [OperationContract(), FaultContract(typeof(faultError))]
                string GetStationControllerSwVersion();

#endregion

#region Host Controller

            //Obtiene la versión de software del HostController en local
            [OperationContract(), FaultContract(typeof(faultError))]
                string GetHostControllerSwVersion();

#endregion

#region Web Manager

            //Obtiene la versión de software del WebManager en local
            [OperationContract(), FaultContract(typeof(faultError))]
                string GetWebManagerSwVersion();

            //Guardala uri del Web Manager
            [OperationContract(), FaultContract(typeof(faultError))]
                void SetWebManagerUri(Uri webManagerUri);

#endregion

#endregion


#region Log Events

            [OperationContract(), FaultContract(typeof(faultError))]
                void RegisterEventLog(List<dc_EventLog> eventLog);

#endregion

        }
}
