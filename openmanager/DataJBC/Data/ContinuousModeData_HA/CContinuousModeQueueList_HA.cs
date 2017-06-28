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
    // list of queues of continuous mode data
    internal class CContinuousModeQueueListStation_HA
    {

        private uint lastQueueID;
        private List<CContinuousModeQueueStation_HA> queueList;
        private object syncObjList = new object();
        private SpeedContinuousMode m_retrieveSpeed;

        public SpeedContinuousMode retrieveSpeed
        {
            get
            {
                return m_retrieveSpeed;
            }
            set
            {
                m_retrieveSpeed = value;
            }
        }

        public CContinuousModeQueueListStation_HA(SpeedContinuousMode retrieveSpeed)
        {
            queueList = new List<CContinuousModeQueueStation_HA>();
            lastQueueID = UInt32.MaxValue;
            m_retrieveSpeed = retrieveSpeed;
        }

        public uint NewQueue(SpeedContinuousMode captureSpeed)
        {
            lock (syncObjList)
            {
                if (lastQueueID >= UInt32.MaxValue - 1)
                {
                    lastQueueID = (uint)0;
                }
                else
                {
                    lastQueueID += System.Convert.ToUInt32((uint)1);
                }
                CContinuousModeQueueStation_HA queuedata = new CContinuousModeQueueStation_HA(lastQueueID, captureSpeed, m_retrieveSpeed);
                queueList.Add(queuedata);
                return lastQueueID;
            }
        }

        // se crea una nueva cola con el ID de cola del Host
        public uint NewQueue(uint queueID, SpeedContinuousMode captureSpeed)
        {
            lock (syncObjList)
            {
                lastQueueID = queueID;
                CContinuousModeQueueStation_HA queuedata = new CContinuousModeQueueStation_HA(queueID, captureSpeed, m_retrieveSpeed);
                queueList.Add(queuedata);
                return queueID;
            }
        }

        public void DeleteQueue(uint queueID)
        {
            //SyncLock syncObjList
            int idx = getIdx(queueID);
            if (idx >= 0)
            {
                queueList[idx].ClearData();
                // dispose ???
                queueList[idx].DisposeData();
                queueList.RemoveAt(idx);
            }
            //End SyncLock
        }

        public int QueueCount()
        {
            return queueList.Count;
        }

        public void AddData(stContinuousModeData_HA data)
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            //SyncLock syncObjList
            // add continuous mode data in each queue buffer
            foreach (CContinuousModeQueueStation_HA dataQueue in queueList)
            {
                dataQueue.AddData(data);
            }
            //End SyncLock
        }

        public void AddData(stContinuousModeData_HA data, uint queueID)
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            //SyncLock syncObjList
            // add continuous mode data in buffer
            int idx = getIdx(queueID);
            if (idx >= 0)
            {
                queueList[idx].dataList.Add(data);
            }
            //End SyncLock
        }

        private int getIdx(uint queueID)
        {
            for (var i = 0; i <= queueList.Count - 1; i++)
            {
                if (queueList[System.Convert.ToInt32(i)].queueID == queueID)
                {
                    return System.Convert.ToInt32(i);
                }
            }
            return -1;
        }

        public SpeedContinuousMode CaptureSpeed(uint queueID)
        {
            try
            {
                int idx = getIdx(queueID);
                if (idx >= 0)
                {
                    return queueList[idx].captureSpeed;
                }
            }
            catch (Exception)
            {

            }
            return SpeedContinuousMode.OFF;
        }

        public int DataCount(uint queueID)
        {
            try
            {
                int idx = getIdx(queueID);
                if (idx >= 0)
                {
                    return queueList[idx].DataCount();
                }
            }
            catch (Exception)
            {

            }
            return 0;
        }

        public stContinuousModeData_HA GetData(uint queueID)
        {
            stContinuousModeData_HA data = new stContinuousModeData_HA();
            try
            {
                int idx = getIdx(queueID);
                if (idx >= 0)
                {
                    // get out data from trace buffer
                    data = queueList[idx].GetData();
                }
            }
            catch (Exception)
            {

            }
            return data;
        }

        public CContinuousModeQueueStation_HA Queue(uint queueID)
        {
            int idx = getIdx(queueID);
            if (idx >= 0)
            {
                return queueList[idx];
            }
            return null;
        }

    }

    // list of queues of continuous mode data
    internal class CContinuousModeQueueList_HA
    {

        private uint lastQueueID;
        private List<CContinuousModeQueue_HA> queueList;
        private object syncObjList = new object();


        public CContinuousModeQueueList_HA()
        {
            queueList = new List<CContinuousModeQueue_HA>();
            lastQueueID = UInt32.MaxValue;
        }

        public uint NewQueue()
        {
            lock (syncObjList)
            {
                if (lastQueueID >= UInt32.MaxValue - 1)
                {
                    lastQueueID = (uint)0;
                }
                else
                {
                    lastQueueID += System.Convert.ToUInt32((uint)1);
                }
                CContinuousModeQueue_HA queuedata = new CContinuousModeQueue_HA(lastQueueID);
                queueList.Add(queuedata);
                return lastQueueID;
            }
        }

        // se crea una nueva cola con el ID de cola del Host
        public uint NewQueue(uint queueID)
        {
            lock (syncObjList)
            {
                lastQueueID = queueID;
                CContinuousModeQueue_HA queuedata = new CContinuousModeQueue_HA(queueID);
                queueList.Add(queuedata);
                return queueID;
            }
        }

        public void DeleteQueue(uint queueID)
        {
            //SyncLock syncObjList
            int idx = getIdx(queueID);
            if (idx >= 0)
            {
                queueList[idx].ClearData();
                // dispose ???
                queueList[idx].DisposeData();
                queueList.RemoveAt(idx);
            }
            //End SyncLock
        }

        public int QueueCount()
        {
            return queueList.Count;
        }

        public void AddData(stContinuousModeData_HA data)
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            //SyncLock syncObjList
            // add continuous mode data in each queue buffer
            foreach (CContinuousModeQueue_HA dataQueue in queueList)
            {
                dataQueue.AddData(data);
            }
            //End SyncLock
        }

        public void AddData(stContinuousModeData_HA data, uint queueID)
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            //SyncLock syncObjList
            // add continuous mode data in buffer
            int idx = getIdx(queueID);
            if (idx >= 0)
            {
                queueList[idx].dataList.Add(data);
            }
            //End SyncLock
        }

        private int getIdx(uint queueID)
        {
            for (var i = 0; i <= queueList.Count - 1; i++)
            {
                if (queueList[System.Convert.ToInt32(i)].queueID == queueID)
                {
                    return System.Convert.ToInt32(i);
                }
            }
            return -1;
        }

        public int DataCount(uint queueID)
        {
            try
            {
                int idx = getIdx(queueID);
                if (idx >= 0)
                {
                    return queueList[idx].DataCount();
                }
            }
            catch (Exception)
            {

            }
            return 0;
        }

        public stContinuousModeData_HA GetData(uint queueID)
        {
            stContinuousModeData_HA data = new stContinuousModeData_HA();
            try
            {
                int idx = getIdx(queueID);
                if (idx >= 0)
                {
                    // get out data from trace buffer
                    data = queueList[idx].GetData();
                }
            }
            catch (Exception)
            {

            }
            return data;
        }

        public CContinuousModeQueue_HA Queue(uint queueID)
        {
            int idx = getIdx(queueID);
            if (idx >= 0)
            {
                return queueList[idx];
            }
            return null;
        }

    }
}
