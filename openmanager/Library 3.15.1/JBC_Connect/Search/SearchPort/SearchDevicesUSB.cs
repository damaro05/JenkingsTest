using System.Threading;


namespace JBC_Connect
{
    /// <summary>
    /// Clase publica para la busqueda de nuevos equipos por USB (puerto serie)
    /// </summary>
    /// <remarks></remarks>
    internal class SearchDevicesUSB
    {

        #region EVENTS

        //NewConnection
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

        //NoConnectionOnPort
        public delegate void NoConnectionOnPortEventHandler(string Port);
        private NoConnectionOnPortEventHandler NoConnectionOnPortEvent;

        public event NoConnectionOnPortEventHandler NoConnectionOnPort
        {
            add
            {
                NoConnectionOnPortEvent = (NoConnectionOnPortEventHandler)System.Delegate.Combine(NoConnectionOnPortEvent, value);
            }
            remove
            {
                NoConnectionOnPortEvent = (NoConnectionOnPortEventHandler)System.Delegate.Remove(NoConnectionOnPortEvent, value);
            }
        }

#if LibraryTest

        //DataReceivedRawDataSearch
	    internal delegate void DataReceivedRawDataSearchEventHandler(ref byte[] RawData);
	    private DataReceivedRawDataSearchEventHandler DataReceivedRawDataSearchEvent;
		
	    internal event DataReceivedRawDataSearchEventHandler DataReceivedRawDataSearch
	    {
		    add
		    {
			    DataReceivedRawDataSearchEvent = (DataReceivedRawDataSearchEventHandler) System.Delegate.Combine(DataReceivedRawDataSearchEvent, value);
		    }
		    remove
		    {
			    DataReceivedRawDataSearchEvent = (DataReceivedRawDataSearchEventHandler) System.Delegate.Remove(DataReceivedRawDataSearchEvent, value);
		    }
	    }
		
        //DataSentRawDataSearchEvent
	    internal delegate void DataSentRawDataSearchEventHandler(ref byte[] RawData);
	    private DataSentRawDataSearchEventHandler DataSentRawDataSearchEvent;
		
	    internal event DataSentRawDataSearchEventHandler DataSentRawDataSearch
	    {
		    add
		    {
			    DataSentRawDataSearchEvent = (DataSentRawDataSearchEventHandler) System.Delegate.Combine(DataSentRawDataSearchEvent, value);
		    }
		    remove
		    {
			    DataSentRawDataSearchEvent = (DataSentRawDataSearchEventHandler) System.Delegate.Remove(DataSentRawDataSearchEvent, value);
		    }
	    }
		
#endif

        #endregion

        #region VARIABLES
        // Creamos una variable del tipo Thread
        private Thread ThreadHijo1;
        // Creamos una variable de la clase en la que está el Sub que se usará en el Thread
        SearchConnectUSB_Base InstanciaHijo;
        SearchConnectUSB NewInstanciaHijo;
        // Otras variables
        private string PortSearch = ""; // search on this port only
        private RoutinesLibrary.IO.SerialPortConfig PortConfig = null; // serial port configuration (nothing = defaults)
        private bool StartHandshake = false; // PC starts handshake
                                             //Private NumDevice As Byte
        #endregion

        #region METODOS PUBLICOS

        public SearchDevicesUSB()
        {
            Initialize();
        }

        public SearchDevicesUSB(string Port, RoutinesLibrary.IO.SerialPortConfig SerialPortConfig = default(RoutinesLibrary.IO.SerialPortConfig), bool bStartHandshake = false)
        {
            PortSearch = Port;
            PortConfig = SerialPortConfig;
            StartHandshake = bStartHandshake;

            Initialize();
        }

        private void Initialize()
        {
            InstanciaHijo = new SearchConnectUSB_Base(this);
            InstanciaHijo.PasaRefHijo += InstanciaHijo1_PasaRefHijo;

            // Asignamos el Sub que queremos usar al crear una nueva instancia de la clase del tipo Thread
            ThreadHijo1 = new Thread(new System.Threading.ThreadStart(InstanciaHijo.CrearNuevoProceso));
            ThreadHijo1.Name = "SearchConnectUSB_Base";
            ThreadHijo1.SetApartmentState(ApartmentState.STA);
            ThreadHijo1.Start();
        }

        public void Eraser()
        {
            NewInstanciaHijo.NewConnection -= NewInstanciaHijo1_NewConnection;
            NewInstanciaHijo.NoConexion -= NewInstanciaHijo1_NoConexion;
            NewInstanciaHijo.Eraser();
            ThreadHijo1.Abort();

#if LibraryTest
		    NewInstanciaHijo.DataSentRawData -= NewInstanciaHijo1_DataSentRawData;
		    NewInstanciaHijo.DataReceivedRawData -= NewInstanciaHijo1_DataReceivedRawData;
#endif
            NewInstanciaHijo = null;
            InstanciaHijo.PasaRefHijo -= InstanciaHijo1_PasaRefHijo;
            InstanciaHijo = null;
        }

        #endregion

        #region METODOS PRIVADOS

        private void NewInstanciaHijo1_NewConnection(ref CConnectionData connectionData)
        {
            if (NewConnectionEvent != null)
            {
                NewConnectionEvent(ref connectionData);
            }
        }

        private void NewInstanciaHijo1_NoConexion(string sPort)
        {
            if (NoConnectionOnPortEvent != null)
            {
                NoConnectionOnPortEvent(sPort);
            }
        }

#if LibraryTest
	private void NewInstanciaHijo1_DataSentRawData(ref byte[] RawData)
	{
		DataSentRawDataSearchEvent(ref RawData);
	}
		
	private void NewInstanciaHijo1_DataReceivedRawData(ref byte[] RawData)
	{
        DataReceivedRawDataSearchEvent(ref RawData);
	}
#endif

        // eventos del nuevo hijo
        private void InstanciaHijo1_PasaRefHijo(ref SearchConnectUSB Hijo)
        {
            NewInstanciaHijo = Hijo;
            NewInstanciaHijo.NewConnection += NewInstanciaHijo1_NewConnection;
            NewInstanciaHijo.NoConexion += NewInstanciaHijo1_NoConexion;
#if LibraryTest
		NewInstanciaHijo.DataSentRawData += NewInstanciaHijo1_DataSentRawData;
		NewInstanciaHijo.DataReceivedRawData += NewInstanciaHijo1_DataReceivedRawData;
#endif
            NewInstanciaHijo.StartSearch(PortSearch, PortConfig, StartHandshake);
        }

        #endregion

    }
}
