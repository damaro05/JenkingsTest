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
    internal class CContinuousModeQueueStation_HA
    {
        //Inherits CContinuousModeQueue_HA

        private struct stAcumPortData
        {
            public int temperatureUTI;
            public int externalTC1_TempUTI;
            public int externalTC2_TempUTI;
            public int power;
            public int flow;
        }

        public uint queueID;
        public ulong userSequence;
        public int lastFrameSequence;
        public List<stContinuousModeData_HA> dataList;

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

        public CContinuousModeQueueStation_HA(uint newQueueID, SpeedContinuousMode captureSpeed, SpeedContinuousMode stationSpeed)
        {
            //MyBase.New(newQueueID)
            queueID = newQueueID;
            clearSequences();
            dataList = new List<stContinuousModeData_HA>();
            m_captureSpeed = captureSpeed;
            m_stationSpeed = stationSpeed;
            m_captureMSCount = 0;
            // calculate capture window
            CSpeedContMode speedContMode = new CSpeedContMode();
            int capSpeed = speedContMode.SpeedFromEnum(m_captureSpeed);
            int stnSpeed = speedContMode.SpeedFromEnum(m_stationSpeed);
            m_captureWindow = capSpeed / stnSpeed;

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

        public void AddData(stContinuousModeData_HA data) // Overloads
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            lock (syncObj)
            {
                int frameSequence = 0;

                m_captureMSCount++;
                // cumulate
                for (var i = 0; i <= data.data.Length - 1; i++)
                {
                    m_acumData[(int)i].power += data.data[(int)i].power;
                    m_acumData[(int)i].flow += data.data[(int)i].flow;
                    m_acumData[(int)i].temperatureUTI += data.data[(int)i].temperature.UTI;
                    m_acumData[(int)i].externalTC1_TempUTI += data.data[(int)i].externalTC1_Temp.UTI;
                    m_acumData[(int)i].externalTC2_TempUTI += data.data[(int)i].externalTC2_Temp.UTI;
                }
                if (m_captureMSCount == m_captureWindow)
                {
                    // set sequence for data
                    userSequence = userSequence + System.Convert.ToUInt64(1);
                    data.sequence = userSequence;

                    for (var i = 0; i <= data.data.Length - 1; i++)
                    {
                        data.data[(int)i].power = System.Convert.ToInt32(m_acumData[(int)i].power / m_captureWindow);
                        data.data[(int)i].flow = System.Convert.ToInt32(m_acumData[(int)i].flow / m_captureWindow);
                        data.data[(int)i].temperature = new CTemperature(System.Convert.ToInt32(m_acumData[(int)i].temperatureUTI / m_captureWindow));
                        data.data[(int)i].externalTC1_Temp = new CTemperature(System.Convert.ToInt32(m_acumData[(int)i].externalTC1_TempUTI / m_captureWindow));
                        data.data[(int)i].externalTC2_Temp = new CTemperature(System.Convert.ToInt32(m_acumData[(int)i].externalTC2_TempUTI / m_captureWindow));
                        m_acumData[(int)i].power = 0;
                        m_acumData[(int)i].flow = 0;
                        m_acumData[(int)i].temperatureUTI = 0;
                        m_acumData[(int)i].externalTC1_TempUTI = 0;
                        m_acumData[(int)i].externalTC2_TempUTI = 0;
                    }

                    dataList.Add(data);
                    m_captureMSCount = 0;
                }

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

        public stContinuousModeData_HA GetData()
        {
            // check data.data.length = 0 on return for error???
            lock (syncObj)
            {
                stContinuousModeData_HA data = new stContinuousModeData_HA();
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

    // queue of continuous mode data
    internal class CContinuousModeQueue_HA
    {

        public uint queueID;
        public ulong userSequence;
        public int lastFrameSequence;
        public List<stContinuousModeData_HA> dataList;

        internal object syncObj = new object();

        public CContinuousModeQueue_HA(uint newQueueID)
        {
            queueID = newQueueID;
            clearSequences();
            dataList = new List<stContinuousModeData_HA>();
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

        public void AddData(stContinuousModeData_HA data)
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

        public stContinuousModeData_HA GetData()
        {
            // check data.data.length = 0 on return for error???
            lock (syncObj)
            {
                stContinuousModeData_HA data = new stContinuousModeData_HA();
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

