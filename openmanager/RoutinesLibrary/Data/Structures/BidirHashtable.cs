// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports


    namespace RoutinesLibrary.Data.Structures
    {

        public class BidirHashtable
        {

            private Hashtable m_htFwd = new Hashtable();
            private Hashtable m_htBkwd = new Hashtable();


            public void Add(object key, object val)
            {
                m_htFwd.Add(key, val);
                m_htBkwd.Add(val, key);
            }

            public void Remove(object key)
            {
                object val = m_htFwd[key];
                m_htFwd.Remove(key);
                m_htBkwd.Remove(val);
            }

            public void RemoveValue(object val)
            {
                object key = m_htBkwd[val];
                m_htFwd.Remove(key);
                m_htBkwd.Remove(val);
            }

            public void Clear()
            {
                m_htFwd.Clear();
                m_htBkwd.Clear();
            }

            public dynamic Item(object key)
            {
                return m_htFwd[key];
            }

            public dynamic ReverseLookup(object val)
            {
                return m_htBkwd[val];
            }

            public bool Contains(object key)
            {
                return m_htFwd.Contains(key);
            }

            public bool ContainsValue(object val)
            {
                return m_htBkwd.Contains(val);
            }

        }

    }

