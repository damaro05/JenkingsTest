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
using System.Text;


namespace RoutinesLibrary.Net.DiscoveryUDP
{
		
	/// <summary>
	/// Provides a mechanism for searching by UDP. This class waits to receive requests to respond to searches
	/// </summary>
	public class DiscoveryUDPHost
	{
			
		//Configuration search
		private string m_MessageRequest;
		private string m_MessageResponse;
		private ushort m_Port;
		private string m_LocalIPAddress;
			
		//UDP socket
		private RoutinesLibrary.Net.Protocols.UDP.UDP m_SockUDP;
			
			
#region CONSTRUCTORS
			
		/// <summary>
		/// Class constructor. Wait to receive requests to respond to searches
		/// </summary>
		/// <param name="messageRequest">Request message</param>
		/// <param name="messageResponse">Response message</param>
		/// <param name="port">Port to wait searches</param>
		/// <param name="localIPAdress">Local IP address to response</param>
		public DiscoveryUDPHost(string messageRequest, string messageResponse, ushort port, string localIPAdress = "")
		{
			//Configuration search
			m_MessageRequest = messageRequest;
			m_MessageResponse = messageResponse;
			m_Port = port;
			m_LocalIPAddress = localIPAdress;
				
			//UDP socket
			m_SockUDP = new RoutinesLibrary.Net.Protocols.UDP.UDP(m_Port);
            m_SockUDP.DataReceived += new RoutinesLibrary.Net.Protocols.UDP.UDP.DataReceivedEventHandler(this.SockUDP_DataReceived);
			m_SockUDP.Activate();
		}
			
		/// <summary>
		/// Release resources
		/// </summary>
		public void Dispose()
		{
			m_SockUDP.Dispose();
			m_SockUDP = null;
            //m_SockUDP.DataReceived -= new RoutinesLibrary.Net.Protocols.UDP.UDP.DataReceivedEventHandler(this.SockUDP_DataReceived);
		}
			
#endregion
			
#region PRIVATE METHODS
			
		/// <summary>
		/// Listen incomming messages and responses to it
		/// </summary>
		/// <param name="Data">Request message</param>
		/// <param name="IP">Remote request IP address</param>
		private void SockUDP_DataReceived(byte[] Data, IPEndPoint IP)
		{
			string sData = Encoding.UTF8.GetString(Data);
				
			//Comprobar procedencia del mensaje
			if (sData == m_MessageRequest)
			{
					
				//send response
				m_SockUDP.RemoteEndPoint = IP;
				try
				{
					m_SockUDP.Send(Encoding.ASCII.GetBytes(m_MessageResponse + ":" + m_LocalIPAddress));
				}
				catch (Exception ex)
				{
#if DEBUG
					throw (new System.Exception(ex.Message));
#endif
				}
			}
		}
			
#endregion
			
	}
		
}
