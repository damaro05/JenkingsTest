namespace JBC_Connect
{
    /// <summary>
    /// Clase Friend capa de aplicación base (sencilla) para pasar los parámetros del nuevo thread a la clase SearchConnectTCP
    /// </summary>
    /// <remarks></remarks>
    internal class CSearchConnectTCP_Base
    {
        internal CSearchDevicesTCP ProcesoPadre;
        private CSearchConnectTCP InSearchPort;


        #region EVENTS

        //PasaRefHijoEvent
        internal delegate void PasaRefHijoEventHandler(ref CSearchConnectTCP Hijo);
        private PasaRefHijoEventHandler PasaRefHijoEvent;

        internal event PasaRefHijoEventHandler PasaRefHijo
        {
            add
            {
                PasaRefHijoEvent = (PasaRefHijoEventHandler)System.Delegate.Combine(PasaRefHijoEvent, value);
            }
            remove
            {
                PasaRefHijoEvent = (PasaRefHijoEventHandler)System.Delegate.Remove(PasaRefHijoEvent, value);
            }
        }

        #endregion


        internal CSearchConnectTCP_Base(CSearchDevicesTCP RefPadre)
        {
            ProcesoPadre = RefPadre;
        }

        internal void CrearNuevoProceso()
        {
            InSearchPort = new CSearchConnectTCP(ProcesoPadre);
            if (PasaRefHijoEvent != null)
                PasaRefHijoEvent(ref InSearchPort);
        }

    }
}
