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

//Dim bytes1() As Byte = BitConverter.GetBytes(15)     ' Convierte en bytes un nÃºmero
//Dim bytes() As Byte = Encoding.UTF8.GetBytes("Hola Mundo") ' Convierte en bytes un string
// 17/11/2014 Service Protocol 01


namespace JBC_ConnectRemote
{


    internal class Remote_StackSold_01 : Remote_StackSold
    {

        private object LockTimer02 = new object();

        internal bool bSendCommandsFirstTime = true;
        internal object ServiceStackJBC01_Lock = new object(); // Variable compartida para bloquear funciones e impedir reentradas


#region METODOS PUBLICOS

        public Remote_StackSold_01(ref JBCStationControllerServiceClient hostService, string hostStnUUID)
        {

            ServiceProtocol = CStation.EnumProtocol.Protocol_01;
            TimerManagement = new System.Timers.Timer();
            TimerManagement.Elapsed += TimerStatus_Tick;
            TimerManagement.Elapsed += TimerStatus_Tick;

            // Se paran los timer por si acaso hasta que se optenga la conexiÃ³n correcta
            TimerManagement.Interval = TIMER_STATUS_TIME;
            TimerManagement.AutoReset = false; // se debe activar de nuevo al salir del evento Elapsed
            TimerManagement.Stop();
            // Se crea la instancia de la clase Station
            Info_Station = new CStationData_SOLD();

            // Se guarda el servicio y el ID de estaciÃ³n del host
            SaveHostService(hostService, hostStnUUID);

            //Await LoadInitStationInfoAsync()
            //Ejecutar sin Async porque no puedo especificar Await
            LoadInitStationInfo();


        }

        private void LoadInitStationInfo()
        {

            // load station info
            if (UpdateStationInfo())
            {
                // Se crean las instancias de la clase Port para almacenar los datos de puertos
                Info_Port = new CPortData_SOLD[Info_Station.Info.PortCount - 1 + 1];
                for (int index = 0; index <= Info_Station.Info.PortCount - 1; index++)
                {
                    Info_Port[index] = new CPortData_SOLD(Info_Station.Info.SupportedTools.Length, Info_Station.Info.TempLevelsCount);
                    for (int index2 = 0; index2 <= Info_Station.Info.SupportedTools.Length - 1; index2++)
                    {
                        Info_Port[index].ToolSettings[index2].Tool = Info_Station.Info.SupportedTools[index2];
                    }
                }
            }

            UpdateStationSettings();
        }

        private async Task LoadInitStationInfoAsync()
        {

            // load station info
            if (await UpdateStationInfoAsync())
            {
                // Se crean las instancias de la clase Port para almacenar los datos de puertos
                Info_Port = new CPortData_SOLD[Info_Station.Info.PortCount - 1 + 1];
                for (int index = 0; index <= Info_Station.Info.PortCount - 1; index++)
                {
                    Info_Port[index] = new CPortData_SOLD(Info_Station.Info.SupportedTools.Length, Info_Station.Info.TempLevelsCount);
                    for (int index2 = 0; index2 <= Info_Station.Info.SupportedTools.Length - 1; index2++)
                    {
                        Info_Port[index].ToolSettings[index2].Tool = Info_Station.Info.SupportedTools[index2];
                    }
                }
            }

            await UpdateStationSettingsAsync();

        }

        public new void StartStack()
        {
            ContTimer_Sync = 0;

            // Se activa el timer para pedir datos
            TimerManagement.Interval = TIMER_STATUS_TIME;
            TimerManagement.AutoReset = false; // se debe activar de nuevo al salir del evento Elapsed
            TimerManagement.Start();
        }

#endregion

#region METODOS PRIVADOS

#region Routines

        private GenericStationTools GetGenericToolFromInternal(byte Dato)
        {
            return ((GenericStationTools) Dato);
        }

        private byte GetInternalToolFromGeneric(GenericStationTools Tool)
        {
            return System.Convert.ToByte(Tool);
        }

        //Private Sub LoadAllToolTemps()
        //    'leer temperaturas de selecciÃ³n y niveles. Se usa al cambier el mÃ­nimo o mÃ¡ximo de la estaciÃ³n
        //    For index = 0 To Info_Port.Length - 1
        //        ReadSelectTemp(CType(index, cPort))
        //        For Each ToolIn As cls_ToolSettings In Info_Port(0).ToolParam.ToolSettings
        //            If ToolIn.Tool <> GenericStationTools.NOTOOL Then
        //                ReadLevelsTemps(CType(index, cPort), ToolIn.Tool)
        //            End If
        //        Next
        //    Next
        //End Sub

#endregion

#region Timers and Events

        public async void TimerStatus_Tick(object sender, System.EventArgs e)
        {

            //SyncLock LockTimer02
            // dentro de un try, porque se puede haber producido la desconexiÃ³n de la estaciÃ³n
            // y haberse borrado las clases y threads correpondientes
            try
            {
                if (connectErrorStatus != EnumConnectError.NO_ERROR)
                {
                    return ;
                }

                // el UpdateStationStatus se usa para sincronismo
                if (ContTimer_Sync > (TIMER_SYNC_COUNT - 1))
                {
                    await UpdateStationStatusAsync();
                    // obtener datos de modo continuo
                    ContTimer_Sync = 0;
                }
                else
                {
                    ContTimer_Sync++;
                }

                if (bSendCommandsFirstTime)
                {
                    // primera solicitud de todos los valores
                    await LoadStationParamAsync();
                    await LoadAllToolParamAsync();
                    await LoadAllPortStatusAsync();
                    await LoadAllPeripheralAsync();
                    await LoadAllCountersAsync();
                    await LoadEthernetConfigurationAsync();
                    await LoadRobotConfigurationAsync();
                    bSendCommandsFirstTime = false;
                }

            }
            catch (Exception)
            {

            }

            // se vuelve a activar
            if (TimerManagement != null)
            {
                TimerManagement.Start();
            }

            //End SyncLock
        }

#endregion

#endregion

    }

}
