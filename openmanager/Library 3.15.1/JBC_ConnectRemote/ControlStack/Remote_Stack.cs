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


namespace JBC_ConnectRemote
{
    public abstract class Remote_Stack
    {

        public enum EnumConnectError : int
        {
            NO_ERROR,
            TIME_OUT, // ErrorEnumConnectError de timeout --> el disposivo no responde a las peticiones (cable interrumpido, equipo apagado, ...)
            WCF_STACK, // Error de enlace WCF
            WCF_SERVICE, // Error en la función WCF
            RESET, // Despues de un reset el equipo deja de enviar y el PC lo detecta por tiempo
            DISCONNECTED // Desconectado normal
        }

        // NO IMPLEMENTADO
        public enum EnumFrameFlowControl : int
        {
            MANUAL, // actualiza los datos de la estación la API
            AUTOMATIC // mantiene actualizados los datos de la estación (gran volumen de datos por red)
        }


        internal const int TIMER_1S = 2; // Se mide en unidades de STATUS_TIME. cadencia en la petición de parámetros que cambian rápidamente
        internal const int TIMER_60S = 120; // Se mide en unidades de STATUS_TIME. cadencia en la petición del resto de parámetros del equipo que cambian lentamente

        internal const int MAX_LENGTH_DEVICENAME = 16; // Como máximo el nombre del equipo puede tener 16 caracteres, en caso contrario se trunca
        internal const int MAX_LENGTH_DEVICEID = 32; // Como máximo el ID del equipo puede tener 32 caracteres, en caso contrario se trunca
        internal const int MAX_TIME_WAIT = 5000; // se establece un tiempo máximo de espera sin tener respuesta del equipo de 5s (se mide en ms)
        internal const int TIMER_STATUS_TIME = 500; // cadencia base en la petición del estado del equipo y sincronismo
        internal const int TIMER_SYNC_COUNT = 6; // veces STATUS_TIME, para enviar una consulta de sincronismo

        internal System.Timers.Timer TimerManagement;

        internal CStation.EnumProtocol ServiceProtocol;
        internal EnumConnectError connectErrorStatus = EnumConnectError.NO_ERROR;


        public void StopCom()
        {
            if (TimerManagement != null)
            {
                TimerManagement.Stop();
            }
        }

        public void Eraser()
        {
            StopCom(); // #Edu#
            if (TimerManagement != null)
            {
                TimerManagement.Dispose();
            }
            TimerManagement = null; // #Edu#
        }

        public void StartStack()
        {
        }

    }
}
