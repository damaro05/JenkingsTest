// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.ServiceProcess;
using System.ServiceModel;


namespace RoutinesLibrary.Services
{
		
	/// <summary>
	/// Manages the services installed on the local machine
	/// </summary>
	/// <remarks></remarks>
	public class WindowsServiceManager
	{
			
		public enum ResultInstall
		{
			OK,
			FILE_NO_FOUND,
			TIME_OVER
		}
			
			
		private const int m_nTimeMaxCmd = 2 * 60 * 1000; //espera 2 minutos a tener instalado/desinstalado el servicio
		private const string m_sInstall = "installutil";
		private const string m_sUninstall = "installutil /u";
			
		private Process m_procExecutingInstall = null;
		private Process m_procExecutingUninstall = null;
		private string m_runtimeDirectory;
			
		System.Timers.Timer m_timeoutInstall = new System.Timers.Timer();
		System.Timers.Timer m_timeoutUninstall = new System.Timers.Timer();
			
		public delegate void InstallEventEventHandler(ResultInstall Result);
		private InstallEventEventHandler InstallEventEvent;
			
		public event InstallEventEventHandler InstallEvent
		{
			add
			{
				InstallEventEvent = (InstallEventEventHandler) System.Delegate.Combine(InstallEventEvent, value);
			}
			remove
			{
				InstallEventEvent = (InstallEventEventHandler) System.Delegate.Remove(InstallEventEvent, value);
			}
		}
			
		public delegate void UninstallEventEventHandler(ResultInstall Result);
		private UninstallEventEventHandler UninstallEventEvent;
			
		public event UninstallEventEventHandler UninstallEvent
		{
			add
			{
				UninstallEventEvent = (UninstallEventEventHandler) System.Delegate.Combine(UninstallEventEvent, value);
			}
			remove
			{
				UninstallEventEvent = (UninstallEventEventHandler) System.Delegate.Remove(UninstallEventEvent, value);
			}
		}
			
			
			
		/// <summary>
		/// Class constructor
		/// </summary>
		public WindowsServiceManager()
		{
			m_timeoutInstall.Interval = m_nTimeMaxCmd;
			m_timeoutInstall.AutoReset = false;
			m_timeoutInstall.Stop();
				
			m_timeoutUninstall.Interval = m_nTimeMaxCmd;
			m_timeoutUninstall.AutoReset = false;
			m_timeoutUninstall.Stop();
				
			m_runtimeDirectory = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
		}
			
		/// <summary>
		/// Install a service
		/// </summary>
		/// <param name="Volume">Hard disk volume where is the service file</param>
		/// <param name="path">Service file path</param>
		/// <param name="File">Service file name</param>
		/// <returns>True if the installation was succesful</returns>
		public bool InstallService(string Volume, string path, string File)
		{
				
			bool bOk = false;
			ProcessStartInfo procStartInfo = new ProcessStartInfo();
				
			try
			{
				if (m_procExecutingInstall != null)
				{
					if (!m_procExecutingInstall.HasExited)
					{
						m_procExecutingInstall.Close();
					}
				}
					
				m_timeoutInstall.Start();
					
				procStartInfo.UseShellExecute = false;
				procStartInfo.CreateNoWindow = true;
				procStartInfo.RedirectStandardInput = true;
				procStartInfo.RedirectStandardOutput = true;
				procStartInfo.RedirectStandardError = true;
				procStartInfo.FileName = "cmd.exe";
				procStartInfo.Arguments = "/k " + m_runtimeDirectory.Substring(0, 2) + " & cd \"" + m_runtimeDirectory + "\"";
				procStartInfo.WindowStyle = ProcessWindowStyle.Normal;
				procStartInfo.Verb = "runas"; //add this to prompt for elevation
					
				m_procExecutingInstall = Process.Start(procStartInfo);
				m_procExecutingInstall.BeginOutputReadLine();
					
				// crea el canal StandardInput
				StreamWriter myStreamWriter = m_procExecutingInstall.StandardInput;
					
				// instala el servicio
				myStreamWriter.WriteLine(m_sInstall + " \"" + System.IO.Path.Combine(path, File) + "\"");
					
				// elimina el cmd
				myStreamWriter.WriteLine("exit");
					
				m_procExecutingInstall.WaitForExit();
				m_timeoutInstall.Stop();
				m_procExecutingInstall.Close();
					
				if (InstallEventEvent != null)
					InstallEventEvent(ResultInstall.OK);
				bOk = true;
					
			}
			catch (Exception ex)
			{
				throw (new Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
				if (InstallEventEvent != null)
					InstallEventEvent(ResultInstall.FILE_NO_FOUND);
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Uninstall a service
		/// </summary>
		/// <param name="Volume">Hard disk volume where is the service file</param>
		/// <param name="path">Service file path</param>
		/// <param name="File">Service file name</param>
		/// <returns>True if the uninstallation was succesful</returns>
		public bool UninstallService(string Volume, string path, string File)
		{
				
			bool bOk = false;
			ProcessStartInfo procStartInfo = new ProcessStartInfo();
				
			try
			{
				if (m_procExecutingUninstall != null)
				{
					if (!m_procExecutingUninstall.HasExited)
					{
						m_procExecutingUninstall.Close();
					}
				}
					
				m_timeoutUninstall.Start();
					
				procStartInfo.UseShellExecute = false;
				procStartInfo.CreateNoWindow = true;
				procStartInfo.RedirectStandardInput = true;
				procStartInfo.RedirectStandardOutput = true;
				procStartInfo.RedirectStandardError = true;
				procStartInfo.FileName = "cmd.exe";
				procStartInfo.Arguments = "/k " + m_runtimeDirectory.Substring(0, 2) + " & cd \"" + m_runtimeDirectory + "\"";
				procStartInfo.WindowStyle = ProcessWindowStyle.Normal;
				procStartInfo.Verb = "runas"; //add this to prompt for elevation
					
				m_procExecutingUninstall = Process.Start(procStartInfo);
				m_procExecutingUninstall.BeginOutputReadLine();
					
				// crea el canal StandardInput
				StreamWriter myStreamWriter = m_procExecutingUninstall.StandardInput;
					
				// instala el servicio
				myStreamWriter.WriteLine(m_sUninstall + " \"" + System.IO.Path.Combine(path, File) + "\"");
					
				// elimina el cmd
				myStreamWriter.WriteLine("exit");
					
				m_procExecutingUninstall.WaitForExit();
				m_timeoutUninstall.Stop();
				m_procExecutingUninstall.Close();
					
				if (UninstallEventEvent != null)
					UninstallEventEvent(ResultInstall.OK);
				bOk = true;
					
			}
			catch (Exception ex)
			{
				throw (new Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
				if (UninstallEventEvent != null)
					UninstallEventEvent(ResultInstall.FILE_NO_FOUND);
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Installation time expiration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimeoutInstall_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			m_procExecutingInstall.Close();
			if (InstallEventEvent != null)
				InstallEventEvent(ResultInstall.OK);
		}
			
		/// <summary>
		/// Uninstall time expiration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void TimeoutUninstall_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			m_procExecutingUninstall.Close();
			if (UninstallEventEvent != null)
				UninstallEventEvent(ResultInstall.OK);
		}
			
		/// <summary>
		/// Start a service
		/// </summary>
		/// <param name="serviceName">Service name</param>
		/// <param name="equipo">Machine name where the service is installed</param>
		/// <returns>True if the service was started succesful</returns>
		public bool StartService(string serviceName, string equipo = "")
		{
				
			bool bOk = false;
			try
			{
				ServiceController service = CvtServiceName2Service(serviceName, equipo);
					
				if (ReferenceEquals(service, null))
				{
					throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: el servicio no existe"));
				}
				else
				{
					bOk = StartService(service, equipo);
				}
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Start a service
		/// </summary>
		/// <param name="service">Service controller</param>
		/// <param name="equipo">Machine name where the service is installed</param>
		/// <returns>True if the service was started succesful</returns>
		private bool StartService(ServiceController service, string equipo = "")
		{
				
			bool bOk = false;
			try
			{
				service.Refresh();
				service.Start();
				bOk = StatusCheck(service, ServiceControllerStatus.Running);
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + (" . Error: " + ex.Message)));
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Stop a service
		/// </summary>
		/// <param name="serviceName">Service name</param>
		/// <param name="equipo">Machine name where the service is installed</param>
		/// <returns>True if the service was stopped succesful</returns>
		public bool StopService(string serviceName, string equipo = "")
		{
				
			bool bOk = false;
			try
			{
				ServiceController service = CvtServiceName2Service(serviceName, equipo);
					
				if (ReferenceEquals(service, null))
				{
					throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: el servicio no existe"));
				}
				else
				{
					bOk = StopService(service);
				}
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + (" . Error: " + ex.Message)));
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Stop a service
		/// </summary>
		/// <param name="service">Service controller</param>
		/// <returns>True if the service was stopped succesful</returns>
		private bool StopService(ServiceController service)
		{
				
			bool bOk = false;
			try
			{
				service.Refresh();
				if (service.CanStop)
				{
					service.Stop();
					bOk = StatusCheck(service, ServiceControllerStatus.Stopped);
				}
			}
			catch (Exception ex)
			{
				throw (new System.Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + (" . Error: " + ex.Message)));
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Finds a service by name
		/// </summary>
		/// <param name="service">Service name</param>
		/// <param name="equipo">Machine name where the service is installed</param>
		/// <returns>Service controller</returns>
		private ServiceController CvtServiceName2Service(string service, string equipo = "")
		{
			ServiceController servController = null;
			ServiceController[] services = ServiceController.GetServices(equipo);
				
			for (var i = 0; i <= services.Length - 1; i++)
			{
				if (service == services[(int) i].ServiceName)
				{
					servController = services[(int) i];
					break;
				}
			}
				
			return servController;
		}
			
		/// <summary>
		/// Check if a service is installed
		/// </summary>
		/// <param name="service">Service name</param>
		/// <param name="equipo">Machine name where the service is installed</param>
		/// <returns>True if the service exists</returns>
		public bool ServiceExists(string service, string equipo = "")
		{
				
			bool bOk = false;
			ServiceController[] services = ServiceController.GetServices(equipo);
				
			for (var i = 0; i <= services.Length - 1; i++)
			{
				if (service == services[(int) i].ServiceName)
				{
					bOk = true;
					break;
				}
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Wait to status of a service
		/// </summary>
		/// <param name="serviceName">Service name</param>
		/// <param name="nextServStatus">Desired service status</param>
		/// <param name="equipo">Machine name where the service is installed</param>
		/// <returns>True if the service status is the desired</returns>
		public bool StatusCheck(string serviceName, ServiceControllerStatus nextServStatus, string equipo = "")
		{
				
			bool bOk = false;
			ServiceController service = CvtServiceName2Service(serviceName, equipo);
				
			if (!ReferenceEquals(service, null))
			{
				bOk = StatusCheck(service, nextServStatus);
			}
				
			return bOk;
		}
			
		/// <summary>
		/// Wait to status of a service
		/// </summary>
		/// <param name="service">Service controller</param>
		/// <param name="nextServStatus">Desired service status</param>
		/// <returns>True if the service status is the desired</returns>
		public bool StatusCheck(ServiceController service, ServiceControllerStatus nextServStatus)
		{
				
			service.Refresh();
			do
			{
				System.Threading.Thread.Sleep(1000);
				service.Refresh();
			} while (service.Status.Equals(ServiceControllerStatus.ContinuePending) 
				|| service.Status.Equals(ServiceControllerStatus.PausePending) 
				|| service.Status.Equals(ServiceControllerStatus.StartPending) 
				|| service.Status.Equals(ServiceControllerStatus.StopPending));
				
			return service.Status == nextServStatus;
		}
			
	}
		
}
