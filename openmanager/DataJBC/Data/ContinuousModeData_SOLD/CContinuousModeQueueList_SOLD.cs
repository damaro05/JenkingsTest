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
    // list of queues of continuous mode data for station retrieve
    internal class CContinuousModeQueueListStation_SOLD
    {

        private uint lastQueueID;
        private List<CContinuousModeQueueStation_SOLD> queueList;
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

        public CContinuousModeQueueListStation_SOLD(SpeedContinuousMode retrieveSpeed)
        {
            queueList = new List<CContinuousModeQueueStation_SOLD>();
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
                CContinuousModeQueueStation_SOLD queuedata = new CContinuousModeQueueStation_SOLD(lastQueueID, captureSpeed, m_retrieveSpeed);
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
                CContinuousModeQueueStation_SOLD queuedata = new CContinuousModeQueueStation_SOLD(queueID, captureSpeed, m_retrieveSpeed);
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

        public void AddData(stContinuousModeData_SOLD data)
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            //SyncLock syncObjList
            // add continuous mode data in each queue buffer
            foreach (CContinuousModeQueueStation_SOLD dataQueue in queueList)
            {
                dataQueue.AddData(data);
            }
            //End SyncLock
        }

        public void AddData(stContinuousModeData_SOLD data, uint queueID)
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

        public stContinuousModeData_SOLD GetData(uint queueID)
        {
            stContinuousModeData_SOLD data = new stContinuousModeData_SOLD();
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

        public CContinuousModeQueueStation_SOLD Queue(uint queueID)
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
    internal class CContinuousModeQueueList
    {

        private uint lastQueueID;
        private List<CContinuousModeQueue> queueList;
        private object syncObjList = new object();

        public CContinuousModeQueueList()
        {
            queueList = new List<CContinuousModeQueue>();
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
                CContinuousModeQueue queuedata = new CContinuousModeQueue(lastQueueID);
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
                CContinuousModeQueue queuedata = new CContinuousModeQueue(queueID);
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

        public void AddData(stContinuousModeData_SOLD data)
        {
            // data.sequence trae la secuencia del frame, que se convertirá en una secuencia continua
            //SyncLock syncObjList
            // add continuous mode data in each queue buffer
            foreach (CContinuousModeQueue dataQueue in queueList)
            {
                dataQueue.AddData(data);
            }
            //End SyncLock
        }

        public void AddData(stContinuousModeData_SOLD data, uint queueID)
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

        public stContinuousModeData_SOLD GetData(uint queueID)
        {
            stContinuousModeData_SOLD data = new stContinuousModeData_SOLD();
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

        public CContinuousModeQueue Queue(uint queueID)
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
