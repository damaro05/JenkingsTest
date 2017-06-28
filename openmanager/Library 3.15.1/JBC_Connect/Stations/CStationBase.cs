// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;
using System.IO;
using DataJBC;

namespace JBC_Connect
{
	internal class CStationBase
	{

		public enum Protocol
		{
			Protocol_undefined = 0,
			Protocol_01 = 1,
			Protocol_02 = 2
		}


		//Check parameters constants
		protected const int MAX_LENGTH_DEVICENAME = 16; // Como máximo el nombre del equipo puede tener 16 caracteres, en caso contrario se trunca

		//Update data constants
		protected const int TIME_UPDATE_DATA = 1000; //1000 miliseconds
		protected const int HIGH_SPEED_UPDATE_DATA = 2;
		protected const int MEDIUM_SPEED_UPDATE_DATA = 5;
		protected const int SLOW_SPEED_UPDATE_DATA = 60;

		//Initialization data constants
		protected const int RETRIES_CHECK_UUID_INITIALIZED = 10;
		protected const int TIME_CHECK_DATA_INITIALIZED = 500; //500 miliseconds

		//Transaction ID
		private const int MAX_ENDED_TRANSACTIONS = 20;


		protected byte m_StationNumDevice; //Station Address

		//Protocol
		protected Protocol m_FrameProtocol = Protocol.Protocol_undefined;
		protected Protocol m_CommandProtocol = Protocol.Protocol_undefined;

		//Communication
		protected CCommunicationChannel m_ComChannel;

		//Continuous mode configured in New() in derived class
		protected CContinuousModeStatus m_startContModeStatus = new CContinuousModeStatus();

		//Transaction ID
		private List<uint> m_EndedTransactions = new List<uint>();

		//Update data
		protected Thread m_ThreadUpdateData;
		protected bool m_ThreadUpdateDataAlive = true;
		protected int m_ContUpdateDataHigh = 0;
		protected int m_ContUpdateDataMedium = 0;
		protected int m_ContUpdateDataSlow = 0;

		//Substations
		protected Thread m_ThreadSearchSubStations;
		protected bool m_ThreadSearchSubStationsAlive = true;

		//Initialization data
		protected bool m_IsDataInitialized = false;
		protected uint m_IdTransactionDataInitialized = UInt32.MaxValue;
		protected Thread m_ThreadCheckDataInitialized;
		protected bool m_ThreadCheckDataInitializedAlive = true;

		//Update Firmware
		protected CUpdateFirmware01 m_UpdateFirmware01;
		protected CUpdateFirmware02 m_UpdateFirmware02;


		public CStationBase()
		{
		}

		public Protocol FrameProtocol
		{
			get
			{
				return m_FrameProtocol;
			}
		}

		public Protocol CommandProtocol
		{
			get
			{
				return m_CommandProtocol;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return m_IsDataInitialized;
			}
		}

		public void AddEndedTransaction(uint transactionID)
		{
			m_EndedTransactions.Add(transactionID);
			if (m_EndedTransactions.Count > MAX_ENDED_TRANSACTIONS)
			{
				m_EndedTransactions.RemoveAt(0);
			}
		}

		public bool QueryEndedTransaction(uint transactionID)
		{
			return m_EndedTransactions.Contains(transactionID);
		}

	}
}
