// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Timers;
using System.ServiceModel;


namespace RoutinesLibrary.Net.DiscoveryUDP
{
	/// <summary>
	/// Provides a mechanism for searching by UDP. This class performs searches periodically
	/// </summary>
	public class DiscoveryUDPClient
	{
			
		//Configuration search
		private string m_MessageRequest;
		private string m_MessageResponse;
		private ushort m_Port;
		private int m_DiscoverInterval;
			
		//UDP socket
		private RoutinesLibrary.Net.Protocols.UDP.UDP m_SockUDP;
			
		//List discovered services
		private List<EndpointAddress> m_ListDiscoveredServices = new List<EndpointAddress>();
		private List<EndpointAddress> m_tempListDiscoveredServices = new List<EndpointAddress>();
		private static Semaphore m_semaphoreListDiscoveredServices = new Semaphore(1, 1);
			
		//Search
		private bool m_SeachActive = false;
		private Thread m_ThreadSearch;
		private bool m_IsAliveThreadSearch = true;
			
			
#region CONSTRUCTORS
			
		/// <summary>
		/// Class constructor. Starts searches automatically
		/// </summary>
		/// <param name="messageRequest">Request message</param>
		/// <param name="messageResponse">Response message</param>
		/// <param name="port">Port to perform searches</param>
		/// <param name="discoverInterval">Time interval to perform searches</param>
		public DiscoveryUDPClient(string messageRequest, string messageResponse, ushort port, int discoverInterval = 3000)
		{
			//Configuration search
			m_MessageRequest = messageRequest;
			m_MessageResponse = messageResponse;
			m_Port = port;
			m_DiscoverInterval = discoverInterval;
				
			//UDP socket
			m_SockUDP = new RoutinesLibrary.Net.Protocols.UDP.UDP(RoutinesLibrary.Net.InformationNetworkInterface.GetPortAvailable());
			//m_SockUDP.DataReceived += new System.EventHandler(this.SockUDP_DataReceived);
            m_SockUDP.DataReceived += new RoutinesLibrary.Net.Protocols.UDP.UDP.DataReceivedEventHandler(SockUDP_DataReceived);
			m_SockUDP.Activate();
				
			//Search
			m_ThreadSearch = new Thread(new System.Threading.ThreadStart(Process_Discover));
			m_ThreadSearch.Name = "Process_Discover_DiscoveryUDPClient";
			m_ThreadSearch.Start();
		}
			
		/// <summary>
		/// Release resources
		/// </summary>
		public void Dispose()
		{
			m_IsAliveThreadSearch = false;
		}
			
#endregion
			
#region PUBLIC METHODS
			
#region Start / Stop
			
		/// <summary>
		/// Starts searches process
		/// </summary>
		public void StartSearch()
		{
			m_SeachActive = true;
		}
			
		/// <summary>
		/// Stop searches process
		/// </summary>
		public void StopSearch()
		{
			m_SeachActive = false;
		}
			
#endregion
			
		/// <summary>
		/// Get the list of discovered services
		/// </summary>
		/// <returns>Discovered services</returns>
		public List<EndpointAddress> GetDiscoveredServices()
		{
			List<EndpointAddress> result = new List<EndpointAddress>();
				
			m_semaphoreListDiscoveredServices.WaitOne();
			foreach (EndpointAddress ep in m_ListDiscoveredServices)
			{
				result.Add(ep);
			}
			m_semaphoreListDiscoveredServices.Release();
				
			return result;
		}
			
#endregion
			
#region PRIVATE METHODS
			
		/// <summary>
		/// Process to perform searches periodically
		/// </summary>
		/// <remarks></remarks>
		private void Process_Discover()
		{
			while (m_IsAliveThreadSearch)
			{
					
				if (m_SeachActive)
				{
						
					//get the list of addresses searched
					m_semaphoreListDiscoveredServices.WaitOne();
					m_ListDiscoveredServices.Clear();
						
					foreach (EndpointAddress ep in m_tempListDiscoveredServices)
					{
						m_ListDiscoveredServices.Add(ep);
					}
						
					m_tempListDiscoveredServices.Clear();
					m_semaphoreListDiscoveredServices.Release();
						
					//send discover frame
					try
					{
						m_SockUDP.SendBroadcast(Encoding.ASCII.GetBytes(m_MessageRequest), m_Port);
					}
					catch (Exception ex)
					{
#if DEBUG
						throw (new System.Exception(ex.Message));
#endif
					}
				}
					
				Thread.Sleep(m_DiscoverInterval);
			}
				
			m_SockUDP.Dispose();
			m_SockUDP = null;
            //m_SockUDP.DataReceived += new System.EventHandler(SockUDP_DataReceived);
            m_SockUDP.DataReceived -= new RoutinesLibrary.Net.Protocols.UDP.UDP.DataReceivedEventHandler(SockUDP_DataReceived);
		}
			
		/// <summary>
		/// Listen response messages and save the EndPoint
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="IP"></param>
		/// <remarks></remarks>
        private void SockUDP_DataReceived(byte[] Data, IPEndPoint IP)
        {
            string strData = System.Text.Encoding.UTF8.GetString(Data);
            string[] aData = Strings.Split(strData, ":", 2);

            //check message response
            if (aData.Count() == 2 && aData[0] == m_MessageResponse)
            {
                EndpointAddress epReceived = new EndpointAddress(aData[1]);

                m_semaphoreListDiscoveredServices.WaitOne();
                if (!m_tempListDiscoveredServices.Contains(epReceived))
                {
                    m_tempListDiscoveredServices.Add(epReceived);
                }
                m_semaphoreListDiscoveredServices.Release();
            }
        }
			
#endregion
			
	}
		
}
