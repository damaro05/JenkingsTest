// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBCHostControllerSrv
{
    public class CEventLogRecorder
    {

        private CLocalData m_localData;


        public CEventLogRecorder(CLocalData localData)
        {
            m_localData = localData;
        }

        public void RegisterEventLog(List<dc_EventLog> eventLog)
        {
            string softwareVersion = m_localData.GetSwInfo().stationControllerSwVersion;

            foreach (dc_EventLog eventLogEntry in eventLog)
            {
                m_localData.SetRegisterEventLog(eventLogEntry.eventDate, softwareVersion, eventLogEntry.eventLevel, eventLogEntry.eventMessage, eventLogEntry.eventApplication);
            }
        }

    }
}
