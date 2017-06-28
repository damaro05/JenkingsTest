namespace JBC_Connect
{
    /// <summary>
    /// Clase Friend capa de aplicación base (sencilla) para pasar los parámetros del nuevo thread a la clase SearchConnectUSB
    /// </summary>
    /// <remarks></remarks>
    internal class SearchConnectUSB_Base
    {
        #region EVENTS

        //PasaRefHijo
        internal delegate void PasaRefHijoEventHandler(ref SearchConnectUSB Hijo);
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

        internal SearchDevicesUSB ProcesoPadre;
        private SearchConnectUSB InSearchPort;


        internal SearchConnectUSB_Base(SearchDevicesUSB RefPadre)
        {
            ProcesoPadre = RefPadre;
        }

        internal void CrearNuevoProceso()
        {
            InSearchPort = new SearchConnectUSB(ProcesoPadre);
            if (PasaRefHijoEvent != null)
                PasaRefHijoEvent(ref InSearchPort);
        }

    }
}
