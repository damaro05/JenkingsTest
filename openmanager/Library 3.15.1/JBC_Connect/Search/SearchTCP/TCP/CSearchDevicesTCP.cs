using System.Threading;

namespace JBC_Connect
{
    /// <summary>
    /// Clase publica para la conexión nuevos equipos por TCP
    /// </summary>
    /// <remarks></remarks>
    internal class CSearchDevicesTCP
    {
        #region EVENTS

        //NewConnectionEvent
        public delegate void NewConnectionEventHandler(ref CConnectionData connectionData);
        private NewConnectionEventHandler NewConnectionEvent;

        public event NewConnectionEventHandler NewConnection
        {
            add
            {
                NewConnectionEvent = (NewConnectionEventHandler)System.Delegate.Combine(NewConnectionEvent, value);
            }
            remove
            {
                NewConnectionEvent = (NewConnectionEventHandler)System.Delegate.Remove(NewConnectionEvent, value);
            }
        }

        //NoConnectionOnEndPointEvent
        public delegate void NoConnectionOnEndPointEventHandler(string EndPoint);
        private NoConnectionOnEndPointEventHandler NoConnectionOnEndPointEvent;

        public event NoConnectionOnEndPointEventHandler NoConnectionOnEndPoint
        {
            add
            {
                NoConnectionOnEndPointEvent = (NoConnectionOnEndPointEventHandler)System.Delegate.Combine(NoConnectionOnEndPointEvent, value);
            }
            remove
            {
                NoConnectionOnEndPointEvent = (NoConnectionOnEndPointEventHandler)System.Delegate.Remove(NoConnectionOnEndPointEvent, value);
            }
        }

        #endregion


        // Creamos una variable del tipo Thread
        private Thread ThreadHijo1;
        // Creamos una variable de la clase en la que está el Sub que se usará en el Thread
        CSearchConnectTCP_Base InstanciaHijo; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        CSearchConnectTCP NewInstanciaHijo;
        // Otras variables
        private string EndPointSearch = ""; // search on this EndPoint only xxxx.xxxx.xxxx.xxxx:port
        private CSearchUDP externalSearcherUDP;


        public CSearchDevicesTCP(CSearchUDP _externalSearcherUDP)
        {
            Initialize(_externalSearcherUDP);
        }

        public CSearchDevicesTCP(string EndPoint, CSearchUDP _externalSearcherUDP)
        {
            Initialize(_externalSearcherUDP);
            // guardamos el end point solicitado
            EndPointSearch = EndPoint;
        }

        private void Initialize(CSearchUDP _externalSearcherUDP)
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            InstanciaHijo = new CSearchConnectTCP_Base(this);
            InstanciaHijo.PasaRefHijo += InstanciaHijo1_PasaRefHijo;

            externalSearcherUDP = _externalSearcherUDP;

            ThreadHijo1 = new Thread(new System.Threading.ThreadStart(InstanciaHijo.CrearNuevoProceso));
            ThreadHijo1.Name = "SearchConnectTCP_Base";
            ThreadHijo1.SetApartmentState(ApartmentState.STA);
            ThreadHijo1.Start();
        }

        public void Dispose()
        {
            NewInstanciaHijo.Eraser();
            ThreadHijo1.Abort();
            NewInstanciaHijo = null;
            //NewInstanciaHijo.NewConnection += NewInstanciaHijo1_NewConnection;
            //NewInstanciaHijo.NoConexion += NewInstanciaHijo1_NoConexion;
            InstanciaHijo.PasaRefHijo -= InstanciaHijo1_PasaRefHijo;
            InstanciaHijo = null;
        }

        public void StopSearch()
        {
            NewInstanciaHijo.StopSearch();
        }

        private void NewInstanciaHijo1_NewConnection(ref CConnectionData connectionData)
        {
            if (NewConnectionEvent != null)
                NewConnectionEvent(ref connectionData);
        }

        private void NewInstanciaHijo1_NoConexion(string sEndPoint)
        {
            if (NoConnectionOnEndPointEvent != null)
                NoConnectionOnEndPointEvent(sEndPoint);
        }

        // eventos del nuevo hijo
        private void InstanciaHijo1_PasaRefHijo(ref CSearchConnectTCP Hijo)
        {
            NewInstanciaHijo = Hijo;
            NewInstanciaHijo.NewConnection += NewInstanciaHijo1_NewConnection;
            NewInstanciaHijo.NoConexion += NewInstanciaHijo1_NoConexion;
            NewInstanciaHijo.StartSearch(EndPointSearch, externalSearcherUDP);
        }

    }
}
