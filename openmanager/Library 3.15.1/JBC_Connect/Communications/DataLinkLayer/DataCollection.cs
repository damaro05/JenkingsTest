// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;

namespace JBC_Connect
{
    /// <summary>
    /// Clase para crear colecciones de bytes
    /// </summary>
    /// <remarks></remarks>
    internal class DataCollection : System.Collections.CollectionBase
    {

        object m_lock = new object();


        internal DataCollection()
        {
            InnerList.Clear();
        }

        internal byte this[int index]
        {
            get
            {
                lock (m_lock)
                {
                    return ((byte)(InnerList[index]));
                }
            }
            set
            {
                lock (m_lock)
                {
                    InnerList[index] = value;
                }
            }
        }

        internal int Add(byte value)
        {
            lock (m_lock)
            {
                return InnerList.Add(value);
            }
        }

        internal void Reset()
        {
            lock (m_lock)
            {
                InnerList.Clear();
            }
        }

        internal byte[] GetArray
        {
            get
            {
                lock (m_lock)
                {
                    byte[] Data = new byte[CountElements - 1 + 1];

                    for (var index = 0; index <= InnerList.Count - 1; index++)
                    {
                        Data[(int)index] = this[System.Convert.ToInt32(index)];
                    }

                    return (Data);
                }
            }
        }

        internal int CountElements
        {
            get
            {
                lock (m_lock)
                {
                    return InnerList.Count;
                }
            }
        }

        internal bool Empty
        {
            get
            {
                lock (m_lock)
                {
                    if (CountElements == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

    }
}
