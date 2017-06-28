// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
	public class CFirmwareStation
	{

		private const int MAX_TIME_TO_DISCONNECT_MICRO = 10; //10 segundos. Retardo de tiempo para marcar un micro como desconectado


		private string m_stationUUID;
		private string m_Model;
		private string m_ModelVersion;
		private string m_ProtocolVersion;
		private string m_HardwareVersion;
		private string m_SoftwareVersion;
		private string m_FileName;
		private DateTime m_TimeMarkToDisconnectMicro = DateTime.MaxValue;


		public string StationUUID
		{
			get
			{
				return m_stationUUID;
			}
			set
			{
				m_stationUUID = value;
			}
		}

		public string Model
		{
			get
			{
				return m_Model;
			}
			set
			{
				m_Model = value;
			}
		}

		public string ModelVersion
		{
			get
			{
				return m_ModelVersion;
			}
			set
			{
				m_ModelVersion = value;
			}
		}

		public string ProtocolVersion
		{
			get
			{
				return m_ProtocolVersion;
			}
			set
			{
				m_ProtocolVersion = value;
			}
		}

		public string HardwareVersion
		{
			get
			{
				return m_HardwareVersion;
			}
			set
			{
				m_HardwareVersion = value;
			}
		}

		public string SoftwareVersion
		{
			get
			{
				return m_SoftwareVersion;
			}
			set
			{
				m_SoftwareVersion = value;
			}
		}

		public string FileName
		{
			get
			{
				return m_FileName;
			}
			set
			{
				m_FileName = value;
			}
		}

		public void SetTimeMarkConnected()
		{
			m_TimeMarkToDisconnectMicro = DateTime.Now;
		}

		public bool IsDisconnectedMicro()
		{
			try
			{
				return m_TimeMarkToDisconnectMicro.AddSeconds(MAX_TIME_TO_DISCONNECT_MICRO) < DateTime.Now;
			}
			catch (Exception)
			{
				return true;
			}
		}
	}
}
