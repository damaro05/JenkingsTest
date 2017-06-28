// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;

namespace DataJBC
{
    // station queue of continuous mode data (used in JBC_Connect)
    internal class CContinuousModeQueueStation_SOLD
{
	//Inherits CContinuousModeQueue
		
	private struct stAcumPortData
	{
		public int temperatureUTI;
		public int power;
	}
		
	public uint queueID;
	public ulong userSequence;
	public int lastFrameSequence;
	public List<stContinuousModeData_SOLD> dataList;
		
	internal object syncObj = new object();
	// datos para velocidad de captura
	// la estación debería enviar los datos a 10 ms y de todos los puertos
	private SpeedContinuousMode m_stationSpeed;
	private SpeedContinuousMode m_captureSpeed;
	private int m_captureMSCount = 0;
	private int m_captureWindow = 1;
	private stAcumPortData[] m_acumData = new stAcumPortData[4];
		
	public SpeedContinuousMode captureSpeed
	{
		// cannot change capture speed
		get
		{
			return m_captureSpeed;
		}
	}
	public SpeedContinuousMode stationSpeed
	{
		get
		{
			return m_stationSpeed;
		}
	}
		
	public CContinuousModeQueueStation_SOLD(uint newQueueID, SpeedContinuousMode captureSpeed, SpeedContinuousMode stationSpeed)
	{
		//MyBase.New(newQueueID)
		queueID = newQueueID;
		clearSequences();
		dataList = new List<stContinuousModeData_SOLD>();
		m_captureSpeed = captureSpeed;
		m_stationSpeed = stationSpeed;
		m_captureMSCount = 0;
		calculateCaptureWindow();
	}
		
	private void clearSequences()
	{
		userSequence = 0;
		lastFrameSequence = 255;
	}
		
	public void ClearData()
	{
		lock(syncObj)
		{
			// remove data from queue buffer
			if (dataList != null)
			{
				dataList.Clear();
			}
		}
	}
		
	public void DisposeData()
	{
		lock(syncObj)
		{
			// dispose data from queue buffer
			if (dataList != null)
			{
				dataList.Clear();
			}
			dataList = null;
		}
	}
		
	private void calculateCaptureWindow()
	{
		// calculate capture window
		CSpeedContMode speedContMode = new CSpeedContMode();
		int capSpeed = speedContMode.SpeedFromEnum(m_captureSpeed);
		int stnSpeed = speedContMode.SpeedFromEnum(m_stationSpeed);
		m_captureWindow = capSpeed / stnSpeed;
	}
		
	public void AddData(stContinuousModeData_SOLD data) // Overloads
	{
		// data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
		lock(syncObj)
		{
			int frameSequence = 0;
				
			m_captureMSCount++;
			// cumulate
			for (var i = 0; i <= data.data.Length - 1; i++)
			{
				m_acumData[(int) i].power += data.data[(int) i].power;
				m_acumData[(int) i].temperatureUTI += data.data[(int) i].temperature.UTI;
			}
			if (m_captureMSCount == m_captureWindow)
			{
				// set sequence for data (with current user sequence)
				userSequence = userSequence + System.Convert.ToUInt64(1);
				data.sequence = userSequence;
					
				for (var i = 0; i <= data.data.Length - 1; i++)
				{
					data.data[(int) i].power = System.Convert.ToInt32(m_acumData[(int) i].power / m_captureWindow);
					data.data[(int) i].temperature = new CTemperature(System.Convert.ToInt32(m_acumData[(int) i].temperatureUTI / m_captureWindow));
					m_acumData[(int) i].power = 0;
					m_acumData[(int) i].temperatureUTI = 0;
				}
					
				dataList.Add(data);
				m_captureMSCount = 0;
			}
				
		}
	}
		
	public int DataCount()
	{
		lock(syncObj)
		{
			try
			{
				return dataList.Count;
			}
			catch (Exception)
			{
					
			}
			return 0;
		}
	}
		
	public stContinuousModeData_SOLD GetData()
	{
		// check data.data.length = 0 on return for error???
		lock(syncObj)
		{
			stContinuousModeData_SOLD data = new stContinuousModeData_SOLD();
			try
			{
				// get out data from trace buffer
				if (dataList.Count > 0)
				{
					data = dataList[0];
					dataList.RemoveAt(0);
				}
			}
			catch (Exception)
			{
					
			}
			return data;
		}
	}
}


    // queue of continuous mode data (used in JBC_ConnectRemote)
    internal class CContinuousModeQueue
    {

        public uint queueID;
        public ulong userSequence;
        public int lastFrameSequence;
        public List<stContinuousModeData_SOLD> dataList;

        internal object syncObj = new object();

        public CContinuousModeQueue(uint newQueueID)
        {
            queueID = newQueueID;
            clearSequences();
            dataList = new List<stContinuousModeData_SOLD>();
        }

        private void clearSequences()
        {
            userSequence = 0;
            lastFrameSequence = 255;
        }

        public void ClearData()
        {
            lock (syncObj)
            {
                // remove data from queue buffer
                if (dataList != null)
                {
                    dataList.Clear();
                }
            }
        }

        public void DisposeData()
        {
            lock (syncObj)
            {
                // dispose data from queue buffer
                if (dataList != null)
                {
                    dataList.Clear();
                }
                dataList = null;
            }
        }

        public void AddData(stContinuousModeData_SOLD data)
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            lock (syncObj)
            {
                int frameSequence = 0;

                //save the frame sequence number (to calculate next usersequence)
                frameSequence = (int)data.sequence;

                // set sequence for data (with current user sequence)
                data.sequence = userSequence;
                //updating the next user sequence and lastFrameSequence
                if (frameSequence > lastFrameSequence)
                {
                    userSequence = userSequence + System.Convert.ToUInt64(frameSequence - lastFrameSequence);
                }
                if (frameSequence < lastFrameSequence)
                {
                    userSequence = userSequence + System.Convert.ToUInt64(frameSequence - lastFrameSequence + 256);
                }
                lastFrameSequence = frameSequence;
                //Dim curSequence As Integer = CInt(data.sequence)
                dataList.Add(data);
            }
        }

        public int DataCount()
        {
            lock (syncObj)
            {
                try
                {
                    return dataList.Count;
                }
                catch (Exception)
                {

                }
                return 0;
            }
        }

        public stContinuousModeData_SOLD GetData()
        {
            // check data.data.length = 0 on return for error???
            lock (syncObj)
            {
                stContinuousModeData_SOLD data = new stContinuousModeData_SOLD();
                try
                {
                    // get out data from trace buffer
                    if (dataList.Count > 0)
                    {
                        data = dataList[0];
                        dataList.RemoveAt(0);
                    }
                }
                catch (Exception)
                {

                }
                return data;
            }
        }

    }
}
