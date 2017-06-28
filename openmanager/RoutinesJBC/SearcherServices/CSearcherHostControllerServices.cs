// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

using System.ServiceModel;
using System.Threading;

namespace RoutinesJBC
{
    public class CSearcherHostControllerServices
    {

        //Search HostController
        protected List<EndpointAddress> m_hostControllerEndpoints = new List<EndpointAddress>();
        protected Thread m_ThreadSearcher;
        protected RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPClient m_searcherServices;
        protected static Mutex m_mutexHostControllerEndpoints = new Mutex();

        public delegate void SingleHostConnectedEventHandler();
        private SingleHostConnectedEventHandler SingleHostConnectedEvent;

        public event SingleHostConnectedEventHandler SingleHostConnected
        {
            add
            {
                SingleHostConnectedEvent = (SingleHostConnectedEventHandler)System.Delegate.Combine(SingleHostConnectedEvent, value);
            }
            remove
            {
                SingleHostConnectedEvent = (SingleHostConnectedEventHandler)System.Delegate.Remove(SingleHostConnectedEvent, value);
            }
        }

        public delegate void MultipleHostConnectedEventHandler();
        private MultipleHostConnectedEventHandler MultipleHostConnectedEvent;

        public event MultipleHostConnectedEventHandler MultipleHostConnected
        {
            add
            {
                MultipleHostConnectedEvent = (MultipleHostConnectedEventHandler)System.Delegate.Combine(MultipleHostConnectedEvent, value);
            }
            remove
            {
                MultipleHostConnectedEvent = (MultipleHostConnectedEventHandler)System.Delegate.Remove(MultipleHostConnectedEvent, value);
            }
        }

        public delegate void HostDisconnectedEventHandler();
        private HostDisconnectedEventHandler HostDisconnectedEvent;

        public event HostDisconnectedEventHandler HostDisconnected
        {
            add
            {
                HostDisconnectedEvent = (HostDisconnectedEventHandler)System.Delegate.Combine(HostDisconnectedEvent, value);
            }
            remove
            {
                HostDisconnectedEvent = (HostDisconnectedEventHandler)System.Delegate.Remove(HostDisconnectedEvent, value);
            }
        }



        public CSearcherHostControllerServices()
        {
            m_searcherServices = new RoutinesLibrary.Net.DiscoveryUDP.DiscoveryUDPClient(SearcherServicesData.MESSAGE_HOSTCONTROLLER_DISCOVERY_REQUEST, SearcherServicesData.MESSAGE_HOSTCONTROLLER_DISCOVERY_RESPONSE, SearcherServicesData.PORT_HOSTCONTROLLER_DISCOVERY);
            m_searcherServices.StartSearch();
            
            m_ThreadSearcher = new Thread(new System.Threading.ThreadStart(SearchListHostController));
            m_ThreadSearcher.IsBackground = true;
            m_ThreadSearcher.Start();

        }

        private void SearchListHostController()
        {
            bool firstSearch = true;

            do
            {
                List<EndpointAddress> hostControllerEndpoints = m_searcherServices.GetDiscoveredServices();

                m_mutexHostControllerEndpoints.WaitOne();

                bool singleHostConnected = m_hostControllerEndpoints.Count != 1 & hostControllerEndpoints.Count == 1;
                bool multipleHostConnected = (m_hostControllerEndpoints.Count == 0 | m_hostControllerEndpoints.Count == 1) && hostControllerEndpoints.Count > 1;
                bool hostDisconnected = (firstSearch || m_hostControllerEndpoints.Count == 1) && hostControllerEndpoints.Count == 0;

                m_hostControllerEndpoints = hostControllerEndpoints;
                m_mutexHostControllerEndpoints.ReleaseMutex();

                if (singleHostConnected)
                {
                    if (SingleHostConnectedEvent != null)
                    {
                        SingleHostConnectedEvent();
                    }
                }
                else if (multipleHostConnected)
                {
                    if (MultipleHostConnectedEvent != null)
                    {
                        MultipleHostConnectedEvent();
                    }
                }
                else if (hostDisconnected)
                {
                    if (HostDisconnectedEvent != null)
                    {
                        HostDisconnectedEvent();
                    }
                }

                firstSearch = false;
                Thread.Sleep(5000);
            } while (true);
        }

        public bool HostAvailable()
        {
            return m_hostControllerEndpoints.Count == 1;
        }

    }
}
