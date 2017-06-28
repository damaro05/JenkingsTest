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

using System.IO;
using System.IO.Ports;
using System.Text;
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading.Tasks;
using JBC_ConnectRemote.JBCService;
using DataJBC;
using OnOff = DataJBC.OnOff;
using SpeedContinuousMode = DataJBC.SpeedContinuousMode;
using Constants = DataJBC.Constants;


namespace JBC_ConnectRemote
{


    internal abstract class Remote_StackHA : Remote_Stack
    {

        public CPortData_HA[] Info_Port; // Se crea una variable de la estructura que almacena la información del puerto
        public CStationData_HA Info_Station; // Se crea una variable de la estructura que almacena la información de la estación
        public List<CPeripheralData> Info_Peripheral = new List<CPeripheralData>(); // Se crea una variable de la estructura que almacena la información de los periféricos
        public string hostName;
        private string m_hostStnUUID;

        private JBCStationControllerServiceClient m_hostService;

        internal int ContTimer_Sync = 0; // Cuenta el número de veces que se activa el evento del timer
        internal int ContTimer_1s = 0; // Cuenta el número de veces que se activa el evento del timer
        internal int ContTimer_60s = 0; // Cuenta el número de veces que se activa el evento del timer
        //Friend controlStackInitialControlMode As Boolean

        public delegate void ConnectErrorEventHandler(EnumConnectError ErrorType, string sMsg, string sStackFunction, dc_EnumConstJBCdc_FaultError serviceErrorCode, 
                string serviceOperation);
        private ConnectErrorEventHandler ConnectErrorEvent;

        public event ConnectErrorEventHandler ConnectError
        {
            add
            {
                ConnectErrorEvent = (ConnectErrorEventHandler) System.Delegate.Combine(ConnectErrorEvent, value);
            }
            remove
            {
                ConnectErrorEvent = (ConnectErrorEventHandler) System.Delegate.Remove(ConnectErrorEvent, value);
            }
        }

        public delegate void ReceivedInfoContiModeEventHandler(stContinuousModeData_HA Datos, uint queueID);
        private ReceivedInfoContiModeEventHandler ReceivedInfoContiModeEvent;

        public event ReceivedInfoContiModeEventHandler ReceivedInfoContiMode
        {
            add
            {
                ReceivedInfoContiModeEvent = (ReceivedInfoContiModeEventHandler) System.Delegate.Combine(ReceivedInfoContiModeEvent, value);
            }
            remove
            {
                ReceivedInfoContiModeEvent = (ReceivedInfoContiModeEventHandler) System.Delegate.Remove(ReceivedInfoContiModeEvent, value);
            }
        }

        public delegate void TransactionFinishedEventHandler(uint transactionID);
        private TransactionFinishedEventHandler TransactionFinishedEvent;

        public event TransactionFinishedEventHandler TransactionFinished
        {
            add
            {
                TransactionFinishedEvent = (TransactionFinishedEventHandler) System.Delegate.Combine(TransactionFinishedEvent, value);
            }
            remove
            {
                TransactionFinishedEvent = (TransactionFinishedEventHandler) System.Delegate.Remove(TransactionFinishedEvent, value);
            }
        }


#region METODOS PUBLICOS

        public Remote_StackHA()
        {
            connectErrorStatus = EnumConnectError.NO_ERROR;
        }

#endregion

#region METODOS PRIVADOS DE EVENTOS EN CLASES DERIVADAS

        protected void RaiseEventError(FaultException<faultError> faultEx, string sStackFunction)
        {
            connectErrorStatus = EnumConnectError.WCF_SERVICE;
            if (ConnectErrorEvent != null)
                ConnectErrorEvent(EnumConnectError.WCF_SERVICE, faultEx.Detail.Message, sStackFunction, (dc_EnumConstJBCdc_FaultError) faultEx.Detail.Code, faultEx.Detail.Operation);
        }

        protected void RaiseEventError(Exception ex, string sStackFunction)
        {
            string sError = ex.Message;
            if (ex.InnerException != null)
            {
                sError = sError + " (" + ex.InnerException.Message + ")";
            }
            connectErrorStatus = EnumConnectError.WCF_STACK;
            if (ConnectErrorEvent != null)
                ConnectErrorEvent(EnumConnectError.WCF_STACK, sError, sStackFunction, 0, "");
        }

        protected void RaiseEventReceivedInfoContiMode(stContinuousModeData_HA Datos, uint queueID)
        {
            // añadir datos leídos al buffer local
            if (ReceivedInfoContiModeEvent != null)
                ReceivedInfoContiModeEvent(Datos, queueID);
        }

        protected void RaiseEventTransactionFinished(uint NumStream)
        {
            // NO SE UTILIZA, implementar cuando se implemente WCF Callbacks
            if (TransactionFinishedEvent != null)
                TransactionFinishedEvent(NumStream);
        }

#endregion

#region METODOS PRIVADOS

#region Routines

        private int SearchToolArray(GenericStationTools Tool)
        {
            int index = 0;
            //busca tool en parámetros
            for (index = 0; index <= Info_Port[0].ToolSettings.Length - 1; index++)
            {
                if (Tool == Info_Port[0].ToolSettings[index].Tool)
                {
                    break;
                }
            }
            return index;
        }

        protected async Task LoadStationParamAsync()
        {
            await UpdateStationSettingsAsync();
            await UpdateStationStatusAsync();
        }

        protected async Task LoadAllPortStatusAsync()
        {
            for (var index = 0; index <= Info_Port.Length - 1; index++)
            {
                await UpdatePortStatusAsync((Port) index);
            }
        }

        protected async Task LoadRobotConfigurationAsync()
        {
            await UpdateRobotConfigurationAsync();
        }

        protected async Task LoadAllToolParamAsync()
        {
            for (var index = 0; index <= Info_Port.Length - 1; index++)
            {
                foreach (CToolSettingsData_HA ToolIn in Info_Port[0].ToolSettings)
                {
                    if (ToolIn.Tool != GenericStationTools.NO_TOOL)
                    {
                        await UpdatePortToolSettingsAsync((Port) index, ToolIn.Tool);
                    }
                }
            }
        }

        protected async Task LoadAllCountersAsync()
        {
            for (var index = 0; index <= Info_Port.Length - 1; index++)
            {
                await UpdatePortCountersAsync((Port) index);
            }
        }

#endregion

#region Save Service and host station ID
        /// <summary>
        /// Guarda el servicio y el ID de la estación del host
        /// </summary>
        /// <remarks></remarks>
        internal new void SaveHostService(JBCStationControllerServiceClient hostService, string hostStnUUID)
        {
            // Guarda el número de dispositivo dentro del protocolo
            m_hostStnUUID = hostStnUUID;
            m_hostService = hostService;
        }
#endregion

#endregion

#region COMMANDS

#region Station Info :fet:
        /// <summary>
        /// Lee del host la información estática (modelo, puertos, vesión del hard y del soft, etc) de la estación
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<bool> UpdateStationInfoAsync()
        {
            // ' SyncLock ServiceStackJBC01_Lock

            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_StationController_Info hostInfo = await m_hostService.GetStationControllerInfoAsync();
                hostName = hostInfo.PCName;
                dc_Station_Sold_Info data = await m_hostService.GetStationInfoAsync(m_hostStnUUID);
                CConvertStationInfoFromDC.CopyData(Info_Station.Info, data);

                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // ' End SyncLock
        }

        /// <summary>
        /// Lee del host la información estática (modelo, puertos, vesión del hard y del soft, etc) de la estación
        /// </summary>
        /// <remarks></remarks>
        internal new bool UpdateStationInfo()
        {
            // ' SyncLock ServiceStackJBC01_Lock

            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_StationController_Info hostInfo = m_hostService.GetStationControllerInfo();
                hostName = hostInfo.PCName;
                dc_Station_Sold_Info data = m_hostService.GetStationInfo(m_hostStnUUID);
                CConvertStationInfoFromDC.CopyData(Info_Station.Info, data);

                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // ' End SyncLock
        }

#endregion

#region Station Status :fet:

        /// <summary>
        /// Lee del host la información de estado de la estación
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<bool> UpdateStationStatusAsync()
        {
            //SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_Station_HA_Status data = await m_hostService.GetStationStatus_HAAsync(m_hostStnUUID);
                CConvertStationStatusFromDC.CopyData_HA(Info_Station.Status, data);
                CConvertContinuousModeStatusFromDC.CopyData(Info_Station.Status.ContinuousModeStatus, data.ContinuousModeStatus);

                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            //End SyncLock
        }

#region Station Control Mode

        /// <summary>
        /// Lee del equipo el estado de la conexión USB o ETH:
        ///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
        ///   * Control Mode: en este estado la estación sólo es configurable desde el PC
        /// </summary>
        /// <remarks></remarks>
        internal new async void ReadConnectStatusAsync()
        {
            // ' SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                Info_Station.Status.ControlMode = (ControlModeConnection) (await m_hostService.GetControlModeAsync(m_hostStnUUID));

                // FALTA actualizar tipo y nombre estación
                // FLATA obtener estados de USB y Ethernet
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

        /// <summary>
        /// Guarda en el equipo el estado de la conexión USB o ETH:
        ///   * Monitor Mode: sólo se puede monitorizar la estación no cambiar ningún valor (por defecto es Monitor Mode)
        ///   * Control Mode: en este estado la estación sólo es configurable desde el PC
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteConnectStatusAsync(ControlModeConnection mode, string userName)
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                await m_hostService.SetControlModeAsync(m_hostStnUUID, (dc_EnumConstJBCdc_ControlModeConnection) mode, userName);
                Info_Station.Status.ControlMode = (ControlModeConnection) (await m_hostService.GetControlModeAsync(m_hostStnUUID));
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            // End SyncLock
        }

        internal void KeepControlMode()
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                m_hostService.KeepControlMode(m_hostStnUUID);
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            // End SyncLock
        }

#endregion

#region Station Remote Mode

        /// <summary>
        /// Le pide al Equipo que pase o salga del modo remoto
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteRemoteModeAsync(OnOff value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetRemoteModeAsync(m_hostStnUUID, (dc_EnumConstJBCdc_OnOff) value);
                Info_Station.Status.RemoteMode = value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            // End SyncLock
        }

#endregion

#endregion

#region Station Settings :fet:

#region Get Station Settings :fet:

        /// <summary>
        /// Lee del host la información de configuración de la estación
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<bool> UpdateStationSettingsAsync()
        {
            // SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_Station_HA_Settings data = await m_hostService.GetStationSettings_HAAsync(m_hostStnUUID);
                CConvertStationSettingsFromDC.CopyData_HA(Info_Station.Settings, data);

                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

        /// <summary>
        /// Lee del host la información de configuración de la estación
        /// </summary>
        /// <remarks></remarks>
        internal new bool UpdateStationSettings()
        {
            // SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_Station_HA_Settings data = m_hostService.GetStationSettings_HA(m_hostStnUUID);
                CConvertStationSettingsFromDC.CopyData_HA(Info_Station.Settings, data);

                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

#endregion

#region Set Station Name

        /// <summary>
        /// Permite configurar el nombre del equipo conectado
        /// </summary>
        /// <param name="Value">Tamaño máximo del string 16</param>
        /// <remarks></remarks>
        public new async Task WriteDeviceNameAsync(string Value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationNameAsync(m_hostStnUUID, Value);
                Info_Station.Settings.Name = Value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set PIN

        /// <summary>
        /// Permite configurar el PIN del equipo conectado
        /// </summary>
        /// <param name="Value"></param>
        /// <remarks></remarks>
        public new async Task WriteDevicePINAsync(string Value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                // si no es dato válido no hacer nada
                if (Value.Length != 4)
                {
                    return ;
                }

                await m_hostService.SetStationPINAsync(m_hostStnUUID, Value);
                Info_Station.Settings.PIN = Value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Max Temp :fet:

        /// <summary>
        /// Guarda en el Equipo la temperatura máxima seleccionable por el equipo en UTI
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteMaxTempAsync(CTemperature value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationMaxTempAsync(m_hostStnUUID, value.UTI, "U"); // in UTI
                Info_Station.Settings.MaxTemp.UTI = value.UTI;
                await LoadAllToolParamAsync(); // lee los datos de temperatura que se pudieron cambiar por cambiar el máximo
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Min Temp :fet:

        /// <summary>
        /// Guarda en el Equipo la temperatura mínima seleccionable por el equipo en UTI
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteMinTempAsync(CTemperature value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationMinTempAsync(m_hostStnUUID, value.UTI, "U"); // in UTI
                Info_Station.Settings.MinTemp.UTI = value.UTI;
                await LoadAllToolParamAsync(); // lee los datos de temperatura que se pudieron cambiar por cambiar el mínimo
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Max Flow :fet:

        /// <summary>
        /// Guarda en el Equipo el caudal máximo seleccionable por el equipo en por 1000
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteMaxFlowAsync(int value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationMaxFlowAsync(m_hostStnUUID, value); // en por 1000
                Info_Station.Settings.MaxFlow = value;
                await LoadAllToolParamAsync(); // lee los datos de caudal que se pudieron cambiar por cambiar el máximo
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Min Flow :fet:

        /// <summary>
        /// Guarda en el Equipo el caudal mínimo seleccionable por el equipo en por mil
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteMinFlowAsync(int value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationMinFlowAsync(m_hostStnUUID, value); // en por 1000
                Info_Station.Settings.MinFlow = value;
                await LoadAllToolParamAsync(); // lee los datos de caudal que se pudieron cambiar por cambiar el mínimo
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Max External Temp :fet:

        /// <summary>
        /// Guarda en el Equipo la temperatura máxima seleccionable por el equipo en UTI
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteMaxExtTempAsync(CTemperature value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationMaxExtTempAsync(m_hostStnUUID, value.UTI, "U"); // in UTI
                Info_Station.Settings.MaxExtTemp.UTI = value.UTI;
                await LoadAllToolParamAsync(); // lee los datos de temperatura que se pudieron cambiar por cambiar el máximo
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Min External Temp :fet:

        /// <summary>
        /// Guarda en el Equipo la temperatura mínima seleccionable por el equipo en UTI
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteMinExtTempAsync(CTemperature value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationMinExtTempAsync(m_hostStnUUID, value.UTI, "U"); // in UTI
                Info_Station.Settings.MinExtTemp.UTI = value.UTI;
                await LoadAllToolParamAsync(); // lee los datos de temperatura que se pudieron cambiar por cambiar el mínimo
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Units

        /// <summary>
        /// Guarda en el Equipo las unidades de representación de temperaturas
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteTempUnitAsync(CTemperature.TemperatureUnit value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                string sUnit = "";
                if (value == CTemperature.TemperatureUnit.Fahrenheit)
                {
                    sUnit = "F";
                }
                else
                {
                    sUnit = "C";
                }

                await m_hostService.SetStationTempUnitAsync(m_hostStnUUID, sUnit);
                Info_Station.Settings.Unit = value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Beep

        /// <summary>
        /// Guarda en el Equipo el Límite de potencia
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteBeepAsync(OnOff value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationBeepAsync(m_hostStnUUID, (dc_EnumConstJBCdc_OnOff) value);
                Info_Station.Settings.Beep = value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set PIN Enabled :fet:

        /// <summary>
        /// Guarda en el Equipo la activación del PIN
        /// </summary>
        /// <remarks></remarks>
        public new async Task WritePINEnabledAsync(OnOff value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationPINEnabledAsync(m_hostStnUUID, (dc_EnumConstJBCdc_OnOff) value);
                Info_Station.Settings.PINEnabled = value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Station Locked :fet:

        /// <summary>
        /// Guarda en el Equipo la activación del PIN
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteStationLockedAsync(OnOff value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetStationLockedAsync(m_hostStnUUID, (dc_EnumConstJBCdc_OnOff) value, "", (uint) 0, false);
                Info_Station.Settings.StationLocked = value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#endregion

#region Port Status :fet:

#region Get Port Status :fet:
        /// <summary>
        /// Lee del host la información de estado del puerto indicado
        /// Además obtiene la configuración de puerto/herramienta si hay una conectada, y actualiza los datos
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<bool> UpdatePortStatusAsync(Port Port)
        {
            // SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_StatusTool_HA data = await m_hostService.GetPortStatus_HAAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port);

                // port status
                CConvertStatusToolFromDC.CopyData_HA(Info_Port[(int)Port].ToolStatus, Info_Port[(int)Port], data);

                // port/current tool settings (if any connected)
                if (((GenericStationTools) data.ConnectedTool) != GenericStationTools.NO_TOOL)
                {
                    int idxTool = SearchToolArray((GenericStationTools) data.ConnectedTool);
                    CConvertToolSettingsFromDC.CopyData_HA(Info_Port[(int)Port].ToolSettings[idxTool], Info_Port[(int)Port], data.This_PortToolSettings);
                }
                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            return ret;
            // End SyncLock
        }

#endregion

#region Set Remote Mode Status :fet: TODO

        /// <summary>
        /// Le pide al Equipo que configure el estado de la herramienta en modo remoto
        /// puerto, heater, suction
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteStatusToolAsync(Port Port, OnOff Heater, OnOff Suction)
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                if (connectErrorStatus != EnumConnectError.NO_ERROR)
                {
                    return ;
                }
                await m_hostService.SetPortStatusTool_HAAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_OnOff) Heater, (dc_EnumConstJBCdc_OnOff) Suction);
                Info_Port[(int)Port].ToolStatus.HeaterStatus_OnOff = Heater;
                Info_Port[(int)Port].ToolStatus.SuctionStatus_OnOff = Suction;

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#endregion

#region Port/Tool Settings :fet:

#region Get Port/Tool Settings :fet:

        /// <summary>
        /// Lee del host la información de configuración del puerto y herramienta indicados
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<bool> UpdatePortToolSettingsAsync(Port Port, GenericStationTools Tool)
        {
            // SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_PortToolSettings_HA data = await m_hostService.GetPortToolSettings_HAAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool);

                // port/tool settings
                int idxTool = SearchToolArray(Tool);
                CConvertToolSettingsFromDC.CopyData_HA(Info_Port[(int)Port].ToolSettings[idxTool], Info_Port[(int)Port], data);
                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

#endregion

#region Set Selected Temp :fet:

        /// <summary>
        /// Guarda en el Equipo la temperatura Seleccionada en UTI
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteSelectTempAsync(Port Port, CTemperature value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetPortToolSelectedTempAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, value.UTI, "U"); // in UTI

                Info_Port[(int)Port].ToolStatus.SelectedTemp.UTI = value.UTI;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Selected Ext Temp :fet:

        /// <summary>
        /// Guarda en el Equipo la temperatura externa fijada por el equipo en UTI
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteSelectExtTempAsync(Port Port, CTemperature value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetPortToolSelectedExtTempAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, value.UTI, "U"); // in UTI

                Info_Port[(int)Port].ToolStatus.SelectedExtTemp.UTI = value.UTI;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Selected Flow x mil :fet:

        /// <summary>
        /// Guarda en el Equipo el caudal fijado por el equipo en por mil
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteSelectFlowAsync(Port Port, int value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetPortToolSelectedFlowAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, value); // en por 1000
                Info_Port[(int)Port].ToolStatus.SelectedFlow_x_Mil = value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Profile Mode

        /// <summary>
        /// Guarda en el Equipo el modo de perfil fijado por el equipo
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteProfileModeAsync(Port Port, OnOff onoff)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetPortToolProfileModeAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_OnOff) onoff);
                Info_Port[(int)Port].ToolStatus.ProfileMode = onoff;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Temp Levels :fet: TODO

        /// <summary>
        /// Guarda en el Equipo los niveles de temperatura
        /// </summary>
        /// <remarks></remarks>
        public async Task WriteLevelsTempsAsync(Port Port, GenericStationTools Tool, OnOff LevelsOnOff, ToolTemperatureLevels LevelSelected, OnOff Level1OnOff, CTemperature Level1Temp, int Level1Flow, CTemperature Level1ExtTemp, OnOff Level2OnOff, CTemperature Level2Temp, int Level2Flow, CTemperature Level2ExtTemp, OnOff Level3OnOff, CTemperature Level3Temp, int Level3Flow, CTemperature Level3ExtTemp)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                // check levels to be sent to station to avoid NACKs
                CTempLevelsData_HA levels = new CTempLevelsData_HA(3);
                levels.LevelsOnOff = LevelsOnOff;
                levels.LevelsTempSelect = LevelSelected;

                levels.LevelsTempOnOff[0] = Level1OnOff;
                levels.LevelsTemp[0].UTI = Level1Temp.UTI;
                levels.LevelsExtTemp[0].UTI = Level1ExtTemp.UTI;
                levels.LevelsFlow[0] = Level1Flow;

                levels.LevelsTempOnOff[1] = Level2OnOff;
                levels.LevelsTemp[1].UTI = Level2Temp.UTI;
                levels.LevelsExtTemp[1].UTI = Level2ExtTemp.UTI;
                levels.LevelsFlow[1] = Level2Flow;

                levels.LevelsTempOnOff[2] = Level3OnOff;
                levels.LevelsTemp[2].UTI = Level3Temp.UTI;
                levels.LevelsExtTemp[2].UTI = Level3ExtTemp.UTI;
                levels.LevelsFlow[2] = Level3Flow;
                // FALTA TODO
                checkTempLevelsSetting(levels);

                await m_hostService.SetPortToolLevels_HAAsync(m_hostStnUUID,
                                                              (dc_EnumConstJBCdc_Port)Port,
                                                              (dc_EnumConstJBCdc_GenericStationTools)Tool,
                                                              (dc_EnumConstJBCdc_OnOff)levels.LevelsOnOff,
                                                              (dc_EnumConstJBCdc_ToolTemperatureLevels)levels.LevelsTempSelect,
                                                              (dc_EnumConstJBCdc_OnOff)(levels.LevelsTempOnOff[0]),
                                                              levels.LevelsTemp[0].UTI,
                                                              levels.LevelsFlow[0],
                                                              levels.LevelsExtTemp[0].UTI,
                                                              (dc_EnumConstJBCdc_OnOff)(levels.LevelsTempOnOff[1]),
                                                              levels.LevelsTemp[1].UTI,
                                                              levels.LevelsFlow[1],
                                                              levels.LevelsExtTemp[1].UTI,
                                                              (dc_EnumConstJBCdc_OnOff)(levels.LevelsTempOnOff[2]),
                                                              levels.LevelsTemp[2].UTI,
                                                              levels.LevelsFlow[2],
                                                              levels.LevelsExtTemp[2].UTI,  // in UTI
                                                              "U");

                int idxTool = SearchToolArray(Tool);
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsOnOff = levels.LevelsOnOff;
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempSelect = levels.LevelsTempSelect;

                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempOnOff[0] = levels.LevelsTempOnOff[0];
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTemp[0].UTI = levels.LevelsTemp[0].UTI;
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsFlow[0] = levels.LevelsFlow[0];
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsExtTemp[0].UTI = levels.LevelsExtTemp[0].UTI;

                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempOnOff[1] = levels.LevelsTempOnOff[1];
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTemp[1].UTI = levels.LevelsTemp[1].UTI;
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsFlow[1] = levels.LevelsFlow[1];
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsExtTemp[1].UTI = levels.LevelsExtTemp[1].UTI;

                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempOnOff[2] = levels.LevelsTempOnOff[2];
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTemp[2].UTI = levels.LevelsTemp[2].UTI;
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsFlow[2] = levels.LevelsFlow[2];
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsExtTemp[2].UTI = levels.LevelsExtTemp[2].UTI;

                //ReadLevelsTemps(Port, Tool)

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock

        }

        public async Task WriteSelectedLevelEnabledAsync(Port Port, GenericStationTools Tool, OnOff onoff)
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                await m_hostService.SetPortToolSelectedLevelEnabledAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, (dc_EnumConstJBCdc_OnOff) onoff);

                int idxTool = SearchToolArray(Tool);
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsOnOff = onoff;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

        public async Task WriteSelectedLevelAsync(Port Port, GenericStationTools Tool, ToolTemperatureLevels level)
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                await m_hostService.SetPortToolSelectedLevelAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, (dc_EnumConstJBCdc_ToolTemperatureLevels) level);

                int idxTool = SearchToolArray(Tool);
                Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempSelect = level;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

        public async Task WriteLevelTempAsync(Port Port, GenericStationTools Tool, ToolTemperatureLevels level, CTemperature levelTemp)
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                await m_hostService.SetPortToolTempLevelAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, (dc_EnumConstJBCdc_ToolTemperatureLevels) level,  // in UTI
                        levelTemp.UTI, "U");

                int idxTool = SearchToolArray(Tool);
                switch (level)
                {
                    case ToolTemperatureLevels.FIRST_LEVEL:
                        Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTemp[0].UTI = levelTemp.UTI;
                        break;
                    case ToolTemperatureLevels.SECOND_LEVEL:
                        Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTemp[1].UTI = levelTemp.UTI;
                        break;
                    case ToolTemperatureLevels.THIRD_LEVEL:
                        Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTemp[2].UTI = levelTemp.UTI;
                        break;
                    case ToolTemperatureLevels.NO_LEVELS:
                        break;

                }
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

        public async Task WriteLevelTempEnabledAsync(Port Port, GenericStationTools Tool, ToolTemperatureLevels level, OnOff onoff)
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                await m_hostService.SetPortToolTempLevelEnabledAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, (dc_EnumConstJBCdc_ToolTemperatureLevels) level, (dc_EnumConstJBCdc_OnOff) onoff);

                int idxTool = SearchToolArray(Tool);
                switch (level)
                {
                    case ToolTemperatureLevels.FIRST_LEVEL:
                        Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempOnOff[0] = onoff;
                        break;
                    case ToolTemperatureLevels.SECOND_LEVEL:
                        Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempOnOff[1] = onoff;
                        break;
                    case ToolTemperatureLevels.THIRD_LEVEL:
                        Info_Port[(int)Port].ToolSettings[idxTool].Levels.LevelsTempOnOff[2] = onoff;
                        break;
                    case ToolTemperatureLevels.NO_LEVELS:
                        break;

                }
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Time To Stop :fet:

        /// <summary>
        /// Guarda en el Equipo retardo para parar
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteTimeToStopAsync(Port Port, GenericStationTools Tool, int value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetPortToolTimeToStopAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, value);

                int idxTool = SearchToolArray(Tool);
                Info_Port[(int)Port].ToolSettings[idxTool].TimeToStop = value;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set External TC mode

        /// <summary>
        /// Guarda en el Equipo el modo del TC externo
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteExternalTCModeAsync(Port Port, GenericStationTools Tool, ToolExternalTCMode_HA mode)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetPortToolExternalTCModeAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, (dc_EnumConstJBCdc_ExternalTCMode_HA) mode);

                int idxTool = SearchToolArray(Tool);
                Info_Port[(int)Port].ToolSettings[idxTool].ExternalTCMode = mode;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Start Mode :fet:

        /// <summary>
        /// Guarda en el equipo el modo de activación
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteStartModeAsync(Port Port, GenericStationTools Tool, OnOff toolButton, PedalAction pedalAction)
        {
            // SyncLock ServiceStackJBC01_Lock
            try
            {
                await m_hostService.SetPortToolStartModeAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, (dc_EnumConstJBCdc_OnOff) toolButton, (dc_EnumConstJBCdc_PedalAction) pedalAction);

                int idxTool = SearchToolArray(Tool);
                Info_Port[(int)Port].ToolSettings[idxTool].StartMode_ToolButton = toolButton;
                Info_Port[(int)Port].ToolSettings[idxTool].StartMode_Pedal = pedalAction;

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Set Adjust Temp :fet:

        /// <summary>
        /// Guarda en el Equipo la temperatura de Ajuste en UTI
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteAjustTempAsync(Port Port, GenericStationTools Tool, CTemperature value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetPortToolAdjustTempAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port, (dc_EnumConstJBCdc_GenericStationTools) Tool, value.UTI, "U"); // in UTI

                int idxTool = SearchToolArray(Tool);
                Info_Port[(int)Port].ToolSettings[idxTool].AdjustTemp.UTI = value.UTI;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#endregion

#region Continuous Mode :fet:

        /// <summary>
        /// Crea una nueva cola de modo continuo en el Host y devuelve el ID de la cola.
        /// Si el modo continuo de la estación está detenido, comienza el modo continuo.
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<uint> StartContiModeAsync()
        {
            // SyncLock ServiceStackJBC01_Lock
            uint ret = (uint) 0;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                ret = System.Convert.ToUInt32(await m_hostService.StartContinuousModeAsync(m_hostStnUUID));
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

        /// <summary>
        /// Cierra una cola de modo continuo en el Host. Si no hay más colas, se detiene el modo continuo de la estación
        /// </summary>
        /// <remarks></remarks>
        internal new async Task StopContiModeAsync(uint queueID)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.StopContinuousModeAsync(m_hostStnUUID, queueID);
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

        /// <summary>
        /// Carga desde el Host los datos de modo continuo de una cola de modo continuo
        /// Solicita la cantidad de datos a leer (iChunk).
        /// Devuelve la cantidad de datos leídos.
        /// </summary>
        /// <remarks></remarks>
        internal new int UpdateContiModeNextData(uint queueID, int iChunk)
        {
            // SyncLock ServiceStackJBC01_Lock
            int ret = 0;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_ContinuousModeData_HA[] datos = m_hostService.GetContinuousModeNextDataChunk_HA(m_hostStnUUID, queueID, iChunk);

                ret = datos.Length;
                for (var i = 0; i <= ret - 1; i++)
                {
                    stContinuousModeData_HA dato = new stContinuousModeData_HA();
                    dato.data = new stContinuousModePort_HA[datos[(int) i].data.Length - 1 + 1];
                    dato.sequence = datos[(int) i].sequence;
                    for (var x = 0; x <= datos[(int) i].data.Length - 1; x++)
                    {
                        dato.data[(int) x].port = (Port) ((Port) (datos[(int) i].data[x].port));
                        dato.data[(int) x].power = datos[(int) i].data[x].power;
                        dato.data[(int) x].temperature = new CTemperature(datos[(int) i].data[x].temperature.UTI);
                        dato.data[(int) x].externalTC1_Temp = new CTemperature(datos[(int) i].data[x].ext1Temp.UTI);
                        dato.data[(int) x].externalTC2_Temp = new CTemperature(datos[(int) i].data[x].ext2Temp.UTI);
                        dato.data[(int) x].flow = datos[(int) i].data[x].flow;
                        dato.data[(int) x].timeToStop = datos[(int) i].data[x].timeToStop;
                        dato.data[(int) x].status = (ToolStatus_HA) (datos[(int) i].data[x].status);
                    }
                    RaiseEventReceivedInfoContiMode(dato, queueID);
                }
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

        internal new async Task<int> UpdateContiModeNextDataAsync(uint queueID, int iChunk)
        {
            // SyncLock ServiceStackJBC01_Lock
            int ret = 0;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_ContinuousModeData_HA[] datos = await m_hostService.GetContinuousModeNextDataChunk_HAAsync(m_hostStnUUID, queueID, iChunk);
                ret = datos.Length;
                for (var i = 0; i <= ret - 1; i++)
                {
                    stContinuousModeData_HA dato = new stContinuousModeData_HA();
                    dato.data = new stContinuousModePort_HA[datos[(int) i].data.Length - 1 + 1];
                    dato.sequence = datos[(int) i].sequence;
                    for (var x = 0; x <= datos[(int) i].data.Length - 1; x++)
                    {
                        dato.data[(int) x].port = (Port) ((Port) (datos[(int) i].data[x].port));
                        dato.data[(int) x].power = datos[(int) i].data[x].power;
                        dato.data[(int) x].temperature = new CTemperature(datos[(int) i].data[x].temperature.UTI);
                        dato.data[(int) x].externalTC1_Temp = new CTemperature(datos[(int) i].data[x].ext1Temp.UTI);
                        dato.data[(int) x].externalTC2_Temp = new CTemperature(datos[(int) i].data[x].ext2Temp.UTI);
                        dato.data[(int) x].flow = datos[(int) i].data[x].flow;
                        dato.data[(int) x].timeToStop = datos[(int) i].data[x].timeToStop;
                        dato.data[(int) x].status = (ToolStatus_HA) (datos[(int) i].data[x].status);
                    }
                    RaiseEventReceivedInfoContiMode(dato, queueID);
                }
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

        /// <summary>
        /// Le pide al Equipo información de la velocidad y los puertos configurados para el modo continuo en la estación
        /// Si la estación no está en modo continuo, devolverá 0 en la velocidad (speed)
        /// </summary>
        /// <remarks></remarks>
        internal new async Task ReadContiModeAsync()
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                dc_ContinuousModeStatus data = await m_hostService.GetContinuousModeAsync(m_hostStnUUID);
                CConvertContinuousModeStatusFromDC.CopyData(Info_Station.Status.ContinuousModeStatus, data);

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

        /// <summary>
        /// Define los puertos y la velocidad del modo continuo que se utilizarán en el próximo arranque del modo continuo.
        /// El arranque del modo continuo de la estación se produce al crear la primera cola con StartContMode y
        /// finaliza cuando no existan más colas al borrar la última con StopContMode
        /// Si la estación ya está en modo continuo, estos valores no tendrán ningún efecto inmediato, sólo se guardarán para el próximo arranque
        /// </summary>
        /// <remarks></remarks>
        public new async Task WriteContiModeAsync(SpeedContinuousMode Speed, Port portA = default(Port), Port portB = default(Port), Port portC = default(Port), Port portD = default(Port))
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                // set in host
                await m_hostService.SetContinuousModeAsync(m_hostStnUUID, (dc_EnumConstJBCdc_SpeedContinuousMode) Speed, (dc_EnumConstJBCdc_Port) portA, (dc_EnumConstJBCdc_Port) portB, (dc_EnumConstJBCdc_Port) portC, (dc_EnumConstJBCdc_Port) portD);

                // save in local
                bool port1 = portA == Port.NUM_1 | portB == Port.NUM_1 | portC == Port.NUM_1 | portD == Port.NUM_1;
                bool port2 = portA == Port.NUM_2 | portB == Port.NUM_2 | portC == Port.NUM_2 | portD == Port.NUM_2;
                bool port3 = portA == Port.NUM_3 | portB == Port.NUM_3 | portC == Port.NUM_3 | portD == Port.NUM_3;
                bool port4 = portA == Port.NUM_4 | portB == Port.NUM_4 | portC == Port.NUM_4 | portD == Port.NUM_4;

                Info_Station.Status.ContinuousModeStatus.speed = Speed;
                Info_Station.Status.ContinuousModeStatus.port1 = port1;
                Info_Station.Status.ContinuousModeStatus.port2 = port2;
                Info_Station.Status.ContinuousModeStatus.port3 = port3;
                Info_Station.Status.ContinuousModeStatus.port4 = port4;

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Counters :fet:

#region Update Port Counters :fet:

        /// <summary>
        /// Lee del host la información de contadores del puerto indicado
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<bool> UpdatePortCountersAsync(Port Port)
        {
            // SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_Port_Counters_HA data = await m_hostService.GetPortCounters_HAAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) Port);
                CConvertCountersFromDC.CopyData_HA(Info_Port[(int)Port].Counters, data.GlobalCounters);
                CConvertCountersFromDC.CopyData_HA(Info_Port[(int)Port].PartialCounters, data.PartialCounters);
                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

#endregion

#region Reset Counters :fet:

        /// <summary>
        /// Pone a cero los contadores parciales del puerto
        /// </summary>
        /// <remarks></remarks>
        public new async void ResetPortToolMinutesPartialAsync(Port port)
        {
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.ResetPortPartialCountersAsync(m_hostStnUUID, (dc_EnumConstJBCdc_Port) port);
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

        }

#endregion

#endregion

#region Communication :fet:

        /// <summary>
        /// Lee del host la información de robot de la estación
        /// </summary>
        /// <remarks></remarks>
        internal new async Task<bool> UpdateRobotConfigurationAsync()
        {
            //SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_RobotConfiguration rbtConf = await m_hostService.GetRobotConfigurationAsync(m_hostStnUUID);
                CConvertRobotConfigurationFromDC.CopyData(Info_Station.Settings.Robot, rbtConf);

                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            //End SyncLock
        }

        /// <summary>
        /// Permite configurar el robot de la estación
        /// </summary>
        /// <param name="Value">Configuración del robot</param>
        /// <remarks></remarks>
        public new async Task WriteRobotConfigurationAsync(CRobotData Value)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                dc_RobotConfiguration rbtConfigurationDC = new dc_RobotConfiguration();
                rbtConfigurationDC.Status = (dc_EnumConstJBCdc_OnOff) ((dc_EnumConstJBCdc_OnOff) Value.Status);
                rbtConfigurationDC.Protocol = (dc_EnumConstJBCdc_RobotProtocol) ((dc_EnumConstJBCdc_RobotProtocol) Value.Protocol);
                rbtConfigurationDC.Address = Value.Address;
                rbtConfigurationDC.Speed = (dc_EnumConstJBCdc_RobotSpeed) ((dc_EnumConstJBCdc_RobotSpeed) Value.Speed);
                rbtConfigurationDC.DataBits = Value.DataBits;
                rbtConfigurationDC.StopBits = (dc_EnumConstJBCdc_RobotStop) ((dc_EnumConstJBCdc_RobotStop) Value.StopBits);
                rbtConfigurationDC.Parity = (dc_EnumConstJBCdc_RobotParity) ((dc_EnumConstJBCdc_RobotParity) Value.Parity);

                await m_hostService.SetRobotConfigurationAsync(m_hostStnUUID, rbtConfigurationDC);
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Profile

        internal new async Task<bool> UpdateProfilesAsync()
        {
            //SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_Profile_HA[] profilesData = await m_hostService.GetAllProfiles_HAAsync(m_hostStnUUID);

                Info_Station.Settings.Profiles.Clear();
                for (int i = 0; i <= profilesData.Length - 1; i++)
                {
                    CProfileData_HA profile = new CProfileData_HA();
                    Info_Station.Settings.Profiles.Add(profile);
                    CConvertProfilesFromDC.CopyData(profile, profilesData[i]);
                }

                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            //End SyncLock
        }

        internal new async Task<bool> UpdateSelectedProfileAsync()
        {
            //SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                Info_Station.Settings.SelectedProfile = System.Convert.ToString(await m_hostService.GetSelectedProfile_HAAsync(m_hostStnUUID));
                ret = true;
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            //End SyncLock
        }

        internal async Task<bool> SetProfileAsync(string profileName, byte[] profileData)
        {
            //SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ret;
            }

            try
            {
                dc_Profile_HA profile = new dc_Profile_HA();
                profile.Name = profileName;
                if (System.IO.Path.GetExtension(profile.Name).ToLower()!="." + Constants.PROFILES_FILE_EXTENSION.ToLower())
                {
                    profile.Name += "." + Constants.PROFILES_FILE_EXTENSION;
                }

                profile.Data = profileData;
                ret = System.Convert.ToBoolean(await m_hostService.SetProfile_HAAsync(m_hostStnUUID, profile));

                //wait until sync profiles finish
                while (!m_hostService.SyncFinishedProfiles_HA(m_hostStnUUID))
                {
                    System.Threading.Thread.Sleep(1000);
                }

                //get all profiles
                await UpdateProfilesAsync();

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError((System.ServiceModel.FaultException`1) faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            //End SyncLock
        }

        internal async Task DeleteProfile(string profileName)
        {
            //SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                m_hostService.DeleteProfile_HA(m_hostStnUUID, profileName);

                //wait until sync profiles finish
                while (!m_hostService.SyncFinishedProfiles_HA(m_hostStnUUID))
                {
                    System.Threading.Thread.Sleep(1000);
                }

                //get all profiles
                await UpdateProfilesAsync();

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            //End SyncLock
        }

        internal async Task SyncProfiles()
        {
            //SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                m_hostService.SyncProfiles_HA(m_hostStnUUID);

                //wait until sync profiles finish
                while (!m_hostService.SyncFinishedProfiles_HA(m_hostStnUUID))
                {
                    System.Threading.Thread.Sleep(1000);
                }

                //get all profiles
                await UpdateProfilesAsync();

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            //End SyncLock
        }

#endregion

#region Station Messages Transaction
        /// <summary>
        /// Se inicia una transacción
        /// SetTransaction, en la JBC Connect DLL, envía un mensaje M_ACK para que la estación devuelva un M_ACK. Se devuelve el número de mensaje.
        /// Cuando la estación recibe un M_ACK, genera un Evento de confirmación con el número de mensaje.
        /// Se utiliza para confirmar que se han ejecutado las operaciones anteriores
        /// </summary>
        /// <remarks></remarks>
        public new async Task<uint> SetTransactionAsync()
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return 0;
            }

            uint ret = 0;

            try
            {
                ret = System.Convert.ToUInt32(await m_hostService.SetTransactionAsync(m_hostStnUUID));
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return ret;
            // End SyncLock
        }

        /// <summary>
        /// Se consulta si ha finalizado la transacción.
        /// SetTransaction, en la JBC Connect DLL, envía un mensaje M_ACK para que la estación devuelva un M_ACK. Se devuelve el número de mensaje.
        /// Cuando la estación recibe un M_ACK, genera un Evento de confirmación con el número de mensaje.
        /// Se utiliza para confirmar que se han ejecutado las operaciones anteriores
        /// </summary>
        /// <remarks></remarks>
        public new async Task<bool> QueryEndedTransactionAsync(uint transactionID)
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return false;
            }

            try
            {
                return await m_hostService.QueryEndedTransactionAsync(m_hostStnUUID, transactionID);
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return false;
            // End SyncLock
        }

#endregion

#region Reset Station

        /// <summary>
        /// Close and reinitialize station
        /// </summary>
        /// <remarks></remarks>
        public new async void DeviceResetAsync()
        {
            // SyncLock ServiceStackJBC01_Lock
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.ResetStationAsync(m_hostStnUUID);
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // End SyncLock
        }

#endregion

#region Reset Station Parameters

        /// <summary>
        /// Le pide que resetee todos los parámetros de estación y que deje el equipo con la configuración de fábrica
        /// </summary>
        /// <remarks></remarks>
        public new async Task SetDefaultStationParamsAsync()
        {
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                await m_hostService.SetDefaultStationParamsAsync(m_hostStnUUID);
            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            // Después del reset pide de nuevo todos los parámetros
            await LoadStationParamAsync();
            await LoadAllToolParamAsync();
        }

#endregion

#region Verificación de Niveles de Temperatura

        // 22/03/2013 checkTempLevels function. stations do not check levels data until tool is connected
        // TODO
        private bool checkTempLevelsSetting(CTempLevelsData_HA levels)
        {
            bool bOk = true; // si se devuelve bOk = false entonces se ha modificado algo
            bool bSearchForATemp = false;
            bool bFoundATemp = false;
            if (levels.LevelsOnOff == OnOff._ON)
            {
                // si está activado el TempsLevels, ver si es coherente la selección de temperaturas

                // si el nivel seleccionado no tiene temperatura o está desactivado, buscar una temperatura activa
                // (en protocolo 02, creo que siempre hay una temperatura)
                switch (levels.LevelsTempSelect)
                {
                    case ToolTemperatureLevels.FIRST_LEVEL:
                        if (levels.LevelsTemp[0].UTI == Constants.NO_TEMP_LEVEL |
                                levels.LevelsTempOnOff[0] == OnOff._OFF)
                        {
                            bSearchForATemp = true;
                        }
                        break;
                    case ToolTemperatureLevels.SECOND_LEVEL:
                        if (levels.LevelsTemp[1].UTI == Constants.NO_TEMP_LEVEL |
                                levels.LevelsTempOnOff[1] == OnOff._OFF)
                        {
                            bSearchForATemp = true;
                        }
                        break;
                    case ToolTemperatureLevels.THIRD_LEVEL:
                        if (levels.LevelsTemp[2].UTI == Constants.NO_TEMP_LEVEL |
                                levels.LevelsTempOnOff[2] == OnOff._OFF)
                        {
                            bSearchForATemp = true;
                        }
                        break;
                    case ToolTemperatureLevels.NO_LEVELS:
                        // si no hay nivel seleccionado, buscar una temperatura (si no la estación devuelve NACK)
                        bSearchForATemp = true;
                        break;
                    default:
                        bSearchForATemp = true;
                        break;
                }

                if (bSearchForATemp)
                {
                    bOk = false; // changed some value
                    // buscar entre las temperaturas activas con temperatura válida
                    if (levels.LevelsTemp[0].UTI != Constants.NO_TEMP_LEVEL &
                            levels.LevelsTempOnOff[0] == OnOff._ON)
                    {
                        levels.LevelsTempSelect = ToolTemperatureLevels.FIRST_LEVEL;
                        bFoundATemp = true;
                    }
                    else if (levels.LevelsTemp[1].UTI != Constants.NO_TEMP_LEVEL &
                            levels.LevelsTempOnOff[1] == OnOff._ON)
                    {
                        levels.LevelsTempSelect = ToolTemperatureLevels.SECOND_LEVEL;
                        bFoundATemp = true;
                    }
                    else if (levels.LevelsTemp[2].UTI != Constants.NO_TEMP_LEVEL &
                            levels.LevelsTempOnOff[2] == OnOff._ON)
                    {
                        levels.LevelsTempSelect = ToolTemperatureLevels.THIRD_LEVEL;
                        bFoundATemp = true;
                    }
                    if (!bFoundATemp)
                    {
                        // si no se ha encontrado una temperatura activa y válida
                        // buscar entre las temperaturas válidas, aunque no estén activas
                        if (levels.LevelsTemp[0].UTI != Constants.NO_TEMP_LEVEL)
                        {
                            levels.LevelsTempSelect = ToolTemperatureLevels.FIRST_LEVEL;
                            levels.LevelsTempOnOff[0] = OnOff._ON;
                            bFoundATemp = true;
                        }
                        else if (levels.LevelsTemp[1].UTI != Constants.NO_TEMP_LEVEL)
                        {
                            levels.LevelsTempSelect = ToolTemperatureLevels.SECOND_LEVEL;
                            levels.LevelsTempOnOff[1] = OnOff._ON;
                            bFoundATemp = true;
                        }
                        else if (levels.LevelsTemp[2].UTI != Constants.NO_TEMP_LEVEL)
                        {
                            levels.LevelsTempSelect = ToolTemperatureLevels.THIRD_LEVEL;
                            levels.LevelsTempOnOff[2] = OnOff._ON;
                            bFoundATemp = true;
                        }
                        else
                        {
                            levels.LevelsTempSelect = ToolTemperatureLevels.FIRST_LEVEL;
                            levels.LevelsTemp[0].UTI = Constants.DEFAULT_TEMP;
                            levels.LevelsTempOnOff[0] = OnOff._ON;
                        }
                    }
                }
            }
            return bOk;
        }

#endregion

#region Update Firmware

        internal void UpdateStationsFirmware(List<CFirmwareStation> stationList)
        {
            //SyncLock ServiceStackJBC01_Lock
            bool ret = false;
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return ;
            }

            try
            {
                List<dc_FirmwareStation> dcStationList = new List<dc_FirmwareStation>();
                foreach (CFirmwareStation firmware in stationList)
                {
                    dc_FirmwareStation dcFirmwareUpdate = new dc_FirmwareStation();
                    dcFirmwareUpdate.stationUUID = firmware.StationUUID;
                    dcFirmwareUpdate.softwareVersion = firmware.SoftwareVersion;
                    dcFirmwareUpdate.hardwareVersion = firmware.HardwareVersion;

                    dcStationList.Add(dcFirmwareUpdate);
                }

                m_hostService.UpdateStations(dcStationList.ToArray());

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            //End SyncLock
        }

        internal List<string> GetStationListUpdating()
        {
            //SyncLock ServiceStackJBC01_Lock
            List<string> stationListUpdating = new List<string>();
            if (connectErrorStatus != EnumConnectError.NO_ERROR)
            {
                return stationListUpdating;
            }

            try
            {
                stationListUpdating.AddRange(m_hostService.GetStationListUpdating());

            }
            catch (FaultException<faultError> faultEx)
            {
                RaiseEventError(faultEx, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                RaiseEventError(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
            //End SyncLock

            return stationListUpdating;
        }

#endregion

#endregion

    }

}
