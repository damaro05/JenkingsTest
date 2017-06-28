// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;
using DataJBC;

namespace JBCStationControllerSrv
{
	public class CStationRemoteControl
	{

		private const int MAX_TIME_CONTROL_MODE_LAST_CONNECTION = 5000;


		private Hashtable m_station2RemoteIPAddress = new Hashtable(); //Station UUID <-> IP
		private Hashtable m_station2RemoteLasConnection = new Hashtable(); //Station UUID <-> Date last connection
		private Hashtable m_station2RemoteUserName = new Hashtable(); //Station UUID <-> User name
		private static Semaphore m_semaphoreStationList = new Semaphore(1, 1);

		private Thread m_ThreadCheckControl;


		public CStationRemoteControl()
		{
			m_ThreadCheckControl = new Thread(new ThreadStart(CheckControl));
			m_ThreadCheckControl.IsBackground = true;
			m_ThreadCheckControl.Start();
		}

		public bool IsRemoteControl(string stationUUID, string ipRemote)
		{
			bool bOk = false;
			stationUUID = DLLConnection.jbc.GetUpperStationParentUUID(stationUUID);

			m_semaphoreStationList.WaitOne();
			if (m_station2RemoteIPAddress.Contains(stationUUID))
			{
				bOk = (m_station2RemoteIPAddress[stationUUID]).ToString() == ipRemote;
			}
			m_semaphoreStationList.Release();

			return bOk;
		}

		public bool SetRemoteControl(string stationUUID, string ipRemote, string userName)
		{
			bool bOk = false;
			stationUUID = DLLConnection.jbc.GetUpperStationParentUUID(stationUUID);

			//Si alguien tiene el control no puedes cogerlo
			m_semaphoreStationList.WaitOne();
			if (!m_station2RemoteIPAddress.Contains(stationUUID))
			{
				bOk = true;
				m_station2RemoteIPAddress.Add(stationUUID, ipRemote);
				m_station2RemoteLasConnection.Add(stationUUID, DateTime.Now);
				m_station2RemoteUserName.Add(stationUUID, userName);
			}
			m_semaphoreStationList.Release();

			return bOk;
		}

		public bool RemoveRemoteControl(string stationUUID, string ipRemote)
		{
			bool bOk = false;
			stationUUID = DLLConnection.jbc.GetUpperStationParentUUID(stationUUID);

			//Si alguien tiene el control no puedes quitarselo
			m_semaphoreStationList.WaitOne();
			if (m_station2RemoteIPAddress.Contains(stationUUID))
			{
				if ((m_station2RemoteIPAddress[stationUUID]).ToString() == ipRemote)
				{
					bOk = true;
					m_station2RemoteIPAddress.Remove(stationUUID);
					m_station2RemoteLasConnection.Remove(stationUUID);
					m_station2RemoteUserName.Remove(stationUUID);
				}
			}
			m_semaphoreStationList.Release();

			return bOk;
		}

		public string UserNameRemoteControl(string stationUUID)
		{
			string name = "";
			stationUUID = DLLConnection.jbc.GetUpperStationParentUUID(stationUUID);

			m_semaphoreStationList.WaitOne();
			if (m_station2RemoteUserName.Contains(stationUUID))
			{
				name = (m_station2RemoteUserName[stationUUID]).ToString();
			}
			m_semaphoreStationList.Release();

			return name;
		}

		public void KeepControlMode(string stationUUID, string ipRemote)
		{
			stationUUID = DLLConnection.jbc.GetUpperStationParentUUID(stationUUID);

			m_semaphoreStationList.WaitOne();
			if (m_station2RemoteIPAddress.Contains(stationUUID))
			{
				if ((m_station2RemoteIPAddress[stationUUID]).ToString() == ipRemote)
				{
					m_station2RemoteLasConnection[stationUUID] = DateTime.Now;
				}
			}
			m_semaphoreStationList.Release();
		}

		private void CheckControl()
		{
			do
			{
				List<string> stationsLooseControl = new List<string>();

				m_semaphoreStationList.WaitOne();
				foreach (DictionaryEntry stationEntry in m_station2RemoteIPAddress)
				{
					try
					{
						string stationUUID = System.Convert.ToString(stationEntry.Key);

						//La estación ya ha dejado de existir
						if (!DLLConnection.jbc.StationExists(stationUUID))
						{
							stationsLooseControl.Add(stationUUID);
						}
						else
						{
							//La estación ha entrado en modo robot
							if (DLLConnection.jbc.GetRobotConfiguration(stationUUID).Status == OnOff._ON)
							{
								stationsLooseControl.Add(stationUUID);

								//Si ha pasado el timeout se pierde el control
							}
							else if ((DateTime.Now - System.Convert.ToDateTime(m_station2RemoteLasConnection[stationUUID])).TotalMilliseconds > MAX_TIME_CONTROL_MODE_LAST_CONNECTION)
							{
								stationsLooseControl.Add(stationUUID);
								DLLConnection.jbc.SetControlMode(stationUUID, ControlModeConnection.MONITOR);
							}
						}
					}
					catch (Exception)
					{
					}
				}

				foreach (string stationUUID in stationsLooseControl)
				{
					m_station2RemoteIPAddress.Remove(stationUUID);
					m_station2RemoteLasConnection.Remove(stationUUID);
					m_station2RemoteUserName.Remove(stationUUID);
				}
				m_semaphoreStationList.Release();

				Thread.Sleep(MAX_TIME_CONTROL_MODE_LAST_CONNECTION);
			} while (true);
		}

	}
}
