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
using JBC_ConnectRemote.JBCService;
using DataJBC;
using RoutinesJBC;


namespace JBC_ConnectRemote
{


    internal class CSearchStations
    {

        private const int TIME_DISCOVER = 3000;

        private RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPClient m_searcherServices;
        private System.Timers.Timer m_Timer;
        private bool m_isSearching = false;

        public delegate void DiscoveredStationControllerEventHandler(ref JBCStationControllerServiceClient hostService);
        private DiscoveredStationControllerEventHandler DiscoveredStationControllerEvent;

        public event DiscoveredStationControllerEventHandler DiscoveredStationController
        {
            add
            {
                DiscoveredStationControllerEvent = (DiscoveredStationControllerEventHandler) System.Delegate.Combine(DiscoveredStationControllerEvent, value);
            }
            remove
            {
                DiscoveredStationControllerEvent = (DiscoveredStationControllerEventHandler) System.Delegate.Remove(DiscoveredStationControllerEvent, value);
            }
        }

        public delegate void DiscoveredStationEventHandler(ref JBCStationControllerServiceClient hostService, string UUID, eStationType hostStnType);
        private DiscoveredStationEventHandler DiscoveredStationEvent;

        public event DiscoveredStationEventHandler DiscoveredStation
        {
            add
            {
                DiscoveredStationEvent = (DiscoveredStationEventHandler) System.Delegate.Combine(DiscoveredStationEvent, value);
            }
            remove
            {
                DiscoveredStationEvent = (DiscoveredStationEventHandler) System.Delegate.Remove(DiscoveredStationEvent, value);
            }
        }



        /// <summary>
        /// Class constructor
        /// </summary>
        /// <remarks>Initialize search service</remarks>
        public CSearchStations()
        {
            m_Timer = new System.Timers.Timer();
            m_Timer.Elapsed += Timer_Elapsed;
            m_Timer.AutoReset = false;
            m_Timer.Interval = TIME_DISCOVER;
            m_Timer.Stop();

            // initialize discovery service
            m_searcherServices = new RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPClient(SearcherServicesData.MESSAGE_STATIONCONTROLLER_DISCOVERY_REQUEST,
                    SearcherServicesData.MESSAGE_STATIONCONTROLLER_DISCOVERY_RESPONSE,
                    SearcherServicesData.PORT_STATIONCONTROLLER_DISCOVERY);
            StartSearch();
        }

        /// <summary>
        /// Stop the search and release the resouces
        /// </summary>
        public void Dispose()
        {
            StopSearch();

            m_Timer.Dispose();
            m_Timer = null;
            m_Timer.Elapsed += Timer_Elapsed;
            m_searcherServices.Dispose();
            m_searcherServices = null;

            this.Finalize();
        }

        /// <summary>
        /// Start the services search
        /// </summary>
        public void StartSearch()
        {
            m_isSearching = true;
            m_searcherServices.StartSearch();
            m_Timer.Start();
        }

        /// <summary>
        /// Stop the services search
        /// </summary>
        public void StopSearch()
        {
            m_isSearching = false;
            m_searcherServices.StopSearch();
            m_Timer.Stop();
        }

        /// <summary>
        /// Restart the services search
        /// </summary>
        public void ReStartSearch()
        {
            StopSearch();
            StartSearch();
        }

        /// <summary>
        /// Search timer handler. Discover stations
        /// </summary>
        /// <param name="sender">Objeto que lanza el evento</param>
        /// <param name="e">Parámetros asociados al evento</param>
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            //buscar stations controllers
            List<EndpointAddress> endpointsList = m_searcherServices.GetDiscoveredServices();

            foreach (EndpointAddress endpoint in endpointsList)
            {
                BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

                try
                {
                    JBCStationControllerServiceClient JBC_StationControllerService = new JBCStationControllerServiceClient(binding, endpoint);
                    JBC_StationControllerService.Open();

                    //notificar de un station controller encontrado
                    if (DiscoveredStationControllerEvent != null)
                        DiscoveredStationControllerEvent(ref JBC_StationControllerService);

                    string[] lista = JBC_StationControllerService.GetStationList();
                    for (var i = 0; i <= lista.Length - 1; i++)
                    {
                        //notificar de una estación encontrada
                        dc_Station_Sold_Info stationInfo = JBC_StationControllerService.GetStationInfo(lista[(int) i]);
                        eStationType stationType = (eStationType) ((eStationType) stationInfo.StationType);
                        if (DiscoveredStationEvent != null)
                            DiscoveredStationEvent(ref JBC_StationControllerService, lista[(int) i], stationType);
                    }
                }
                catch (Exception ex)
                {
                    //Este error sucede al desconectar un Station Controller ya que se intenta acceder a un Endpoint donde nadie escucha
                    Debug.WriteLine(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                }
            }

            if (m_isSearching)
            {
                m_Timer.Start();
            }
        }

    }

}
