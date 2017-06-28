// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using JBC_Connect;

namespace JBCStationControllerSrv
{
	public class CUserSession
	{

		private struct PortStation
		{
			public string stationUID;
			public dc_EnumConstJBC.dc_Port portNbr;
		}


		private Hashtable m_htKeyboardUserCode = new Hashtable(); //keyboard <-> userCode
		private Hashtable m_htStationUserCode = new Hashtable(); //station  <-> userCode
		private Hashtable m_htUserCodeUserName = new Hashtable(); //userCode <-> userName


		/// <summary>
		/// Creates a new user session associated with a station
		/// </summary>
		/// <param name="stationUID">Station UID</param>
		/// <param name="portNbr">Port number</param>
		/// <param name="userCode">User ID</param>
		/// <param name="userName">User name</param>
		/// <param name="deviceName">Keyboard input ID</param>
		/// <returns>True if the station is not associated yet and the operation was succesful</returns>
		public bool NewUserSession(string stationUID, dc_EnumConstJBC.dc_Port portNbr, string userCode, string userName, string deviceName)
		{

			PortStation portStation = new PortStation();
			portStation.portNbr = portNbr;
			portStation.stationUID = stationUID;


			//check station is not already associated
			bool isNewAssociated = !m_htStationUserCode.Contains(portStation);

			if (isNewAssociated)
			{

				//keyboard <-> userCode
				if (deviceName != "")
				{
					if (m_htKeyboardUserCode.Contains(deviceName))
					{
						m_htKeyboardUserCode[deviceName] = userCode;
					}
					else
					{
						m_htKeyboardUserCode.Add(deviceName, userCode);
					}
				}

				//station <-> userCode
				m_htStationUserCode.Add(portStation, userCode);

				//userCode <-> userName
				if (m_htUserCodeUserName.Contains(userCode))
				{
					m_htUserCodeUserName[userCode] = userName;
				}
				else
				{
					m_htUserCodeUserName.Add(userCode, userName);
				}
			}

			return isNewAssociated;
		}

		/// <summary>
		/// Delete an user session associated with a station
		/// </summary>
		/// <param name="stationUID">Station UID</param>
		/// <param name="portNbr">Port number</param>
		/// <returns>True if the session exists and it was deleted</returns>
		public bool CloseUserSession(string stationUID, dc_EnumConstJBC.dc_Port portNbr)
		{
			bool bOk = false;

			PortStation portStation = new PortStation();
			portStation.portNbr = portNbr;
			portStation.stationUID = stationUID;

			//keyboard <-> userCode
			//no remove

			//station <-> userCode
			if (m_htStationUserCode.Contains(portStation))
			{
				m_htStationUserCode.Remove(portStation);
				bOk = true;
			}
			
			//userCode <-> userName
			//no remove

			return bOk;
		}

		/// <summary>
		/// Delete the associated keyboard to an user
		/// </summary>
		/// <param name="deviceName">Keyboard input ID</param>
		/// <returns>True if the keyboard exists and it was deleted</returns>
		public bool CloseKeyboardUserSession(string deviceName)
		{
			bool bOk = true;

			if (m_htKeyboardUserCode.Contains(deviceName))
			{
				m_htKeyboardUserCode.Remove(deviceName);
				bOk = true;
			}

			return bOk;
		}

		/// <summary>
		/// Get the user ID associated to a station
		/// </summary>
		/// <param name="stationUID">Station UID</param>
		/// <param name="portNbr">Port number</param>
		/// <returns>User ID. If there is no user associated, return an empty string</returns>
		public string GetAuthenticatedUser(string stationUID, dc_EnumConstJBC.dc_Port portNbr)
		{
			string userCode = "";

			PortStation portStation = new PortStation();
			portStation.portNbr = portNbr;
			portStation.stationUID = stationUID;

			if (m_htStationUserCode.Contains(portStation))
			{
				userCode = (m_htStationUserCode[portStation]).ToString();
			}

			return userCode;
		}

	}
}
