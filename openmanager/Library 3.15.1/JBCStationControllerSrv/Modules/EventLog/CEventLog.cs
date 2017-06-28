// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.Text.RegularExpressions;
using HostControllerServiceReference;



/// <summary>
///
/// </summary>
/// <remarks>
/// Example of event entry:
/// *INFO       2017-01-10 11:00:45,102        Service Started [GeneralLogger] [7]
/// </remarks>
namespace JBCStationControllerSrv
{
    public class CEventLog
    {
        public CEventLog()
        {
            m_appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().FullName;
        }

        private const string LOG_FILE = "application.log";
        private const string WARN_MARK = "*WARN";
        private const string ERROR_MARK = "*ERROR";

        private string m_appName; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.


        public List<dc_EventLog> GetLogEvents()
        {
            List<dc_EventLog> logEvents = new List<dc_EventLog>();

            if (File.Exists(LOG_FILE))
            {
                FileStream objStream = new FileStream(LOG_FILE, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                StreamReader objReader = new StreamReader(objStream);
                string sTextLine = "";
                DateTime lastDataCollection = System.Convert.ToDateTime(My.Settings.Default.EventLogLastDataCollection);

                while (objReader.Peek() != -1)
                {
                    sTextLine = objReader.ReadLine();
                    string mark = "";

                    if (sTextLine.Contains(WARN_MARK))
                    {
                        mark = WARN_MARK;
                    }
                    if (sTextLine.Contains(ERROR_MARK))
                    {
                        mark = ERROR_MARK;
                    }

                    //Warning or Error line
                    if (!string.IsNullOrEmpty(mark))
                    {
                        sTextLine = System.Convert.ToString(sTextLine.Replace(mark, "").Trim());
                        DateTime eventDate = DateTime.ParseExact(sTextLine.Substring(0, 23), "yyyy-MM-dd HH:mm:ss,fff", System.Globalization.CultureInfo.InvariantCulture);

                        //Recent event
                        if (eventDate > lastDataCollection)
                        {
                            dc_EventLog logEvent = new dc_EventLog();
                            logEvent.eventLevel = mark.Substring(1);
                            logEvent.eventDate = eventDate;
                            logEvent.eventMessage = System.Convert.ToString(sTextLine.Substring(23).Trim());
                            logEvent.eventApplication = m_appName;

                            logEvents.Add(logEvent);
                        }
                    }
                }

                objReader.Close();
            }

            return logEvents;
        }

    }
}
