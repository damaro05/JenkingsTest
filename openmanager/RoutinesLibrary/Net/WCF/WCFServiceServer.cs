// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Discovery;


namespace RoutinesLibrary.Net.WCF
{
		
	public class WCFServiceServer
	{
			
		public static bool OpenWCFServiceBasic(ref string sErr,
                                               ref ServiceHost serviceHost,
                                               Type serviceType,
                                               Type serviceInterfaceType,
                                               string serviceName,
                                               int servicePort = 8000)
		{
			sErr = "";
				
			if (serviceHost != null)
			{
				serviceHost.Close();
			}
				
            Uri baseAddress = new Uri("http://" + Environment.MachineName + ":" + System.Convert.ToString(servicePort) + "/" + serviceName);
            serviceHost = new ServiceHost(serviceType, baseAddress);
				
			try
			{
				BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
				serviceHost.AddServiceEndpoint(serviceInterfaceType, binding, "");
					
				// IncludeExceptionDetailInFaults behaviour
				if (serviceHost.Description.Behaviors.Contains(typeof(ServiceDebugBehavior)))
				{
					((ServiceDebugBehavior) (serviceHost.Description.Behaviors[typeof(ServiceDebugBehavior)])).IncludeExceptionDetailInFaults = true;
				}
					
				// Enable metadata exchange (behaviour)
				if (serviceHost.Description.Behaviors.Contains(typeof(ServiceMetadataBehavior)))
				{
					((ServiceMetadataBehavior) (serviceHost.Description.Behaviors[typeof(ServiceMetadataBehavior)])).HttpGetEnabled = true;
				}
				else
				{
					ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
					smb.HttpGetEnabled = true;
					serviceHost.Description.Behaviors.Add(smb);
				}
					
				// DISCOVERY behaviour
				// make the service discoverable by adding the discovery behavior
				if (!serviceHost.Description.Behaviors.Contains(typeof(ServiceDiscoveryBehavior)))
				{
					serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
				}
					
				// DISCOVERY ENDPOINT
				// add the discovery endpoint
				//serviceHost.AddServiceEndpoint(New UdpDiscoveryEndpoint())
					
				// MEX ENDPOINT
				serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
					
				// Open the ServiceHostBase to create listeners and start listening for messages.
				//FIXME . Provoca error
				serviceHost.Open();
					
			}
			catch (Exception ex)
			{
				sErr = ex.Message;
				if (ex.InnerException != null)
				{
					sErr = sErr + " (" + ex.InnerException.Message + ").";
				}
					
				serviceHost.Abort();
				return false;
			}
				
			return true;
		}

        public static bool OpenWCFServiceWSDual(ref string sErr,
                                                ref ServiceHost serviceHost,
                                                Type serviceType,
                                                Type serviceInterfaceType,
                                                string serviceName,
                                                int servicePort = 8000)
		{
			sErr = "";
				
			if (serviceHost != null)
			{
				serviceHost.Close();
			}
				
			List<Uri> baseAddrs = new List<Uri>();
			Uri hostURI = new Uri("http://" + Environment.MachineName + ":" + System.Convert.ToString(servicePort) + "/" + serviceName);
			baseAddrs.Add(hostURI);
			serviceHost = new ServiceHost(serviceType, baseAddrs.ToArray());
				
			try
			{
				WSDualHttpBinding binding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
				binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
				serviceHost.AddServiceEndpoint(serviceInterfaceType, binding, "");
					
				// IncludeExceptionDetailInFaults behaviour
				if (serviceHost.Description.Behaviors.Contains(typeof(ServiceDebugBehavior)))
				{
					((ServiceDebugBehavior) (serviceHost.Description.Behaviors[typeof(ServiceDebugBehavior)])).IncludeExceptionDetailInFaults = true;
				}
					
				// Enable metadata exchange (behaviour)
				if (serviceHost.Description.Behaviors.Contains(typeof(ServiceMetadataBehavior)))
				{
					((ServiceMetadataBehavior) (serviceHost.Description.Behaviors[typeof(ServiceMetadataBehavior)])).HttpGetEnabled = true;
				}
				else
				{
					ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
					smb.HttpGetEnabled = true;
					serviceHost.Description.Behaviors.Add(smb);
				}
					
				// MEX ENDPOINT
				serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
					
				// Open the ServiceHostBase to create listeners and start listening for messages.
				//FIXME . Provoca error
				serviceHost.Open();
					
			}
			catch (Exception ex)
			{
				sErr = ex.Message;
				if (ex.InnerException != null)
				{
					sErr = sErr + " (" + ex.InnerException.Message + ").";
				}
					
				serviceHost.Abort();
				return false;
			}
				
			return true;
		}
			
		public static bool CloseWCFService(ref ServiceHost serviceHost)
		{
				
			if (serviceHost != null)
			{
				try
				{
					serviceHost.Close();
				}
				catch (Exception)
				{
					serviceHost.Abort();
					return false;
				}
					
				serviceHost = null;
			}
				
			return true;
		}
			
	}
		
}
