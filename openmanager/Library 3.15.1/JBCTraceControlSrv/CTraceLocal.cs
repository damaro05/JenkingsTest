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

using System.ServiceModel;
using System.IO;
using System.Management;
using JBC_Connect;
using DataJBC;
using RoutinesJBC;



namespace JBCTraceControlLocalSrv
{
    public class CTraceLocal
    {
        private const int TIME_START = 2000;
        private const int TIME_STOP_SERVICE = 5000;
        private const int TIME_COPY_FILES = 2000;
        private const int TIME_DISCOVER = 5000;
        private const SpeedContinuousMode DEFAULT_CAPTURE_SPEED = SpeedContinuousMode.T_100mS;
        private const string USB_TRACE_FOLDER_NAME = "TraceFiles"; // folder in USB
        private const string COPIED_TRACE_FOLDER_NAME = "CopiedTraceFiles"; // folder inside m_TraceFolder to move already copied files to USB
        private const string LOG_TIME_FORMAT = "yyyyMMddHHmmss";
        private const string TRACE_LOG_EXTENSION = "log";
        private const string TRACE_CONTROL_LOG_NAME = "JBCTraceControl";

        public int FileMaxSequence = 6000; // Maximum Sequence for Trace File

        private ManagementEventWatcher watcher = new ManagementEventWatcher();

        public enum eStatus
        {
            none,
            tracing,
            onerror
        }

        private JBC_API jbc = null;

        private System.Timers.Timer m_TimerStart;
        private System.Timers.Timer m_TimerDiscover;
        private System.Timers.Timer m_TimerCopyFiles;

        private bool bClosing = false;
        private bool m_isTracing = false;
        private Dictionary<long, CTraceStation> m_listTracingStations = new Dictionary<long, CTraceStation>();
        private object obj_listTracingStations = new object();
        private string m_TraceFolder = "";
        private string m_TraceControlLogFolder; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private List<string> filesToMove;
        private string m_USBTraceFolder = "";

        private CTraceData m_Trace;

        public class CTraceStation
        {
            public long ID = -1;
            public string UID = "";
            public eStatus status = eStatus.none;
            public string Name = "";
            public string Model = "";
            public string ModelType = "";
            public string ModelVersion = "";
            public string Software = "";
            public string Hardware = "";
            public string COM = "";

            public CTraceStation()
            {
                ID = -1;
                UID = "";
                status = eStatus.none;
            }

            public CTraceStation(long _ID, string _UID)
            {
                ID = _ID;
                UID = _UID;
                status = eStatus.none;
            }
        }

        public CTraceLocal(JBC_API _jbc, int _FileMaxSequence = 0)
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            m_TraceControlLogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\JBC Station Controller Service\\TraceData");


            LoggerModule.InitLogger();

            jbc = _jbc;
            FileMaxSequence = _FileMaxSequence;

            m_Trace = new CTraceData(6000);

            //CheckForIllegalCrossThreadCalls = False

            //butStartTrace.Enabled = True
            //butStopTrace.Enabled = False
            //nudFileMaxSequence.Enabled = butStartTrace.Enabled

            m_TimerStart = new System.Timers.Timer();
            m_TimerStart.Elapsed += TimerStart_Elapsed;
            m_TimerStart.AutoReset = false;
            m_TimerStart.Interval = TIME_START;
            m_TimerStart.Stop();

            m_TimerDiscover = new System.Timers.Timer();
            m_TimerDiscover.Elapsed += TimerDiscover_Elapsed;
            m_TimerDiscover.AutoReset = false;
            m_TimerDiscover.Interval = TIME_DISCOVER;
            m_TimerDiscover.Stop();

            m_TimerCopyFiles = new System.Timers.Timer();
            m_TimerCopyFiles.Elapsed += TimerCopyFiles_Elapsed;
            m_TimerCopyFiles.AutoReset = false;
            m_TimerCopyFiles.Interval = TIME_COPY_FILES;
            m_TimerCopyFiles.Stop();

            m_TimerStart.Start();
        }

        public void Dispose()
        {

            // stop watcher
            if (watcher != null)
            {
                // parar watching
                watcher.Stop();
            }

            stopTrace();
            stopSearching();
            m_Trace.close();

        }

        private void TimerStart_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_TimerStart.Stop();

            // iniciar watch de cambio de unidades
            watchDrives();

            if (!startTrace())
            {
                playSound(eSounds.TraceError, USE_SPEAKER);
                playSound(eSounds.TraceError, USE_SPEAKER);
                playSound(eSounds.TraceError, USE_SPEAKER);
            }

        }

        public bool startTrace()
        {
            // start discovering stations

            log("---");
            if (ReferenceEquals(jbc, null))
            {
                if (!startSearching())
                {
                    log("# Error in startTrace");
                    return false;
                }
            }

            playSound(eSounds.TraceStart, USE_SPEAKER);

            try
            {
                //Dim sPath As String = JBC_SC.GetTraceFolder()
                string sPath = m_Trace.GetFolderData();
                m_TraceFolder = sPath;
                log("Trace Folder: " + sPath);
            }
            catch (Exception)
            {
                log("# Error in GetTraceFolder)");
                m_TraceFolder = "";
                stopSearching();
                return false;
            }

            m_isTracing = true;
            //butStartTrace.Enabled = False
            //butStopTrace.Enabled = True
            //nudFileMaxSequence.Enabled = butStartTrace.Enabled
            log("Trace started.");

            m_TimerDiscover.Start();

            return true;
        }

        public void restart()
        {
            stopTrace();
            m_Trace.close();
            m_Trace = new CTraceData(FileMaxSequence);
            if (!startTrace())
            {
                playSound(eSounds.TraceError, USE_SPEAKER);
                playSound(eSounds.TraceError, USE_SPEAKER);
                playSound(eSounds.TraceError, USE_SPEAKER);
            }
        }

        private bool startTraceStation(long id)
        {
            CTraceStation stn = null;
            bool bOk = true;

            if (!m_listTracingStations.ContainsKey(id))
            {
                //Try
                //    info = JBC_SC.GetStationInfo(id)
                //Catch ex As Exception
                //    log("  Error al querer obtener info de la estación id: '" & id.ToString & "'")
                //    bOk = False
                //End Try
                //Try
                //    settings = JBC_SC.GetStationSettings(id)
                //Catch ex As Exception
                //    log("  Error al querer obtener settings de la estación id: '" & id.ToString & "'")
                //    bOk = False
                //End Try

                if (bOk)
                {
                    stn = getStationData(id);
                    log("  Start trace station '" + showStationData(stn) + "'");
                    if (string.IsNullOrEmpty(stn.UID))
                    {
                        log("     UID IS MISSING, CANNOT TRACE.");
                    }
                    m_listTracingStations.Add(id, stn);

                    try
                    {
                        if (!string.IsNullOrEmpty(stn.UID))
                        {
                            //JBC_SC.SetTraceSpeed(info.InternalID, SpeedContinuousMode.T_100mS)
                            //JBC_SC.StartTrace(info.InternalID)
                            //m_Trace.SetTraceSpeed(InternalID, DEFAULT_CAPTURE_SPEED)
                            m_Trace.TraceSpeed(stn.UID, SpeedContinuousMode.T_10mS);
                            m_Trace.StartTraceData(stn.UID);
                            stn.status = eStatus.tracing;
                        }
                    }
                    catch (Exception)
                    {
                        log("# Error trying to trace station: '" + showStationData(stn) + "'");
                        return false;
                    }

                }
            }
            else
            {
                return false;
            }

            return true;

        }

        public void stopTrace()
        {
            // stop discovering, and stop trace and remove stations from list

            m_TimerDiscover.Stop();

            // stop tracing all stations
            if (jbc != null)
            {
                List<long> keys = new List<long>();
                keys.AddRange(m_listTracingStations.Keys);
                foreach (long id in keys)
                {
                    string stationData = showStationData(getStationData(id));
                    if (!stopTraceStation(id))
                    {
                        //log("# Error trying to stop trace for station (stopTrace):" & stationData & "")
                    }
                }
            }
            m_isTracing = false;
            //butStartTrace.Enabled = True
            //butStopTrace.Enabled = False
            //nudFileMaxSequence.Enabled = butStartTrace.Enabled
            log("Trace stopped.");
        }

        private bool stopTraceStation(long id)
        {
            string stationData = "";
            // get data from list (may be station does not exist in jbc dll)
            if (m_listTracingStations.ContainsKey(id))
            {
                try
                {
                    // gets data
                    CTraceStation stn = m_listTracingStations[id];
                    stationData = showStationData(stn);
                    // remove from list
                    m_listTracingStations.Remove(id);
                    // stop trace in UID
                    if (stn.UID.Trim() != "")
                    {
                        //JBC_SC.StopTrace(stn.UID)
                        m_Trace.StopTraceData(stn.UID);
                        log("  Stop trace for station: " + stationData + "");
                    }
                }
                catch (Exception)
                {
                    return false;
                    //					log("# Error trying to stop trace for station (stopTrace): " + stationData + "");
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void TimerDiscover_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_TimerDiscover.Stop();
            if (discoverStations())
            {
                m_TimerDiscover.Start();
            }
        }

        private bool discoverStations()
        {
            // discover new stations and trace them

            long[] listStn = null;
            CTraceStation stn = null;

            //Dim info As JBCService1.dc_Station_Sold_Info = Nothing
            //Dim settings As JBCService1.dc_Station_Sold_Settings = Nothing

            bool bOk = true;

            try
            {
                logFile("Searching stations..."); // only to file
                listStn = jbc.getStationList();
            }
            catch (Exception)
            {
                log("# Error trying to list stations (GetStationList)");
                stopSearching();
                return false;
            }

            foreach (long id in listStn)
            {
                // look if it is tracing and start
                startTraceStation(id);
            }

            return true;

        }

        private string showStationData(CTraceStation stn)
        {
            string stationData = "";
            try
            {
                stationData = "id:" + stn.ID.ToString();
                stationData += " name:" + stn.Name + " (" + stn.COM + ")";
                stationData += " model:" + stn.Model;
                stationData += " SW/HW:" + stn.Software + "/" + stn.Hardware;
                stationData += " UID:[" + stn.UID + "]";
            }
            catch (Exception)
            {

            }
            return stationData;
        }

        private CTraceStation getStationData(long id)
        {
            CTraceStation stn = new CTraceStation();
            if (id < 0)
            {
                return stn;
            }
            try
            {
                string InternalID = jbc.GetStationInternalUID(id); // info.InternalID
                stn.ID = id;
                stn.UID = InternalID;
                stn.COM = jbc.GetStationCOM(id);
                stn.Name = jbc.GetStationName(id); // settings.Name
                stn.Model = jbc.GetStationModel(id); // info.Model
                stn.ModelType = jbc.GetStationModelType(id); // info.ModelType
                stn.ModelVersion = System.Convert.ToString(jbc.GetStationModelVersion(id)); // info.ModelVersion.ToString
                stn.Software = jbc.GetStationSWversion(id); // info.Version_Software
                stn.Hardware = jbc.GetStationHWversion(id); // info.Version_Hardware
            }
            catch (Exception)
            {

            }
            return stn;
        }

        private bool startSearching()
        {
            try
            {
                if (jbc != null)
                {
                    jbc.NewStationConnected += event_StationConnected;
                    jbc.StationDisconnected += event_StationDisconnected;
                    jbc.UserError += event_UserError;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void event_StationConnected(long stationID)
        {
            CTraceStation stn = getStationData(stationID);
            log("Connected: " + showStationData(stn));
        }

        public void event_StationDisconnected(long stationID)
        {
            string stationData = "Station ID: " + stationID.ToString();
            if (m_listTracingStations.ContainsKey(stationID))
            {
                stationData = showStationData(m_listTracingStations[stationID]);
            }
            log("Disconnected: " + stationData);
            stopTraceStation(stationID);
        }

        public void event_UserError(long stationID, Cerror err)
        {
            log("# Error in station ID: " + stationID.ToString() + " Msg: " + err.GetMsg());
            log("#   Ex: " + err.Message);

        }

        private void stopSearching()
        {
            try
            {
                if (jbc != null)
                {
                    jbc.NewStationConnected -= event_StationConnected;
                    jbc.StationDisconnected -= event_StationDisconnected;
                    jbc.UserError -= event_UserError;
                }
            }
            catch (Exception)
            {
            }
        }


        #region Move files from tracing to USB

        private void TimerCopyFiles_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            m_TimerCopyFiles.Stop();
            string sBackupPath = Path.Combine(m_TraceFolder, COPIED_TRACE_FOLDER_NAME);
            if (!Directory.Exists(m_USBTraceFolder))
            {
                try
                {
                    Directory.CreateDirectory(m_USBTraceFolder);
                }
                catch (Exception)
                {
                    log("# Could not create directory '" + m_USBTraceFolder + "'");
                    m_USBTraceFolder = "";
                    m_insertedUSBDriveName = "";
                }
            }
            if (!Directory.Exists(sBackupPath))
            {
                try
                {
                    Directory.CreateDirectory(sBackupPath);
                }
                catch (Exception)
                {
                    log("# Could not create backup directory '" + sBackupPath + "'");
                }
            }
            if (Directory.Exists(m_USBTraceFolder))
            {
                moveTraceFiles();
            }

        }

        private int moveTraceFiles()
        {
            string sSourcePathFilename = "";
            string sTargetPathFilename = "";
            string sBackupPathfilename = "";
            int iCount = 0;
            if (string.IsNullOrEmpty(m_USBTraceFolder))
            {
                return iCount;
            }

            // move trace files
            filesToMove = m_Trace.GetListRecordedDataFiles();
            string[] files = filesToMove.ToArray();

            // copy trace files and log
            log("Copying " + (files.Length).ToString() + " trace data files...");
            playSound(eSounds.CopyStart, USE_SPEAKER);
            foreach (string fi in files)
            {
                sSourcePathFilename = Path.Combine(m_TraceFolder, fi);
                sTargetPathFilename = Path.Combine(m_USBTraceFolder, fi);
                sBackupPathfilename = Path.Combine(m_TraceFolder, COPIED_TRACE_FOLDER_NAME, fi);
                if (System.IO.File.Exists(sSourcePathFilename))
                {
                    try
                    {
                        if (isDriveConnected(m_insertedUSBDriveName))
                        {
                            // copy to USB
                            System.IO.File.Copy(sSourcePathFilename, sTargetPathFilename, true);
                            iCount++;

                            // move to backup
                            try
                            {
                                // move to backup
                                if (Directory.Exists(Path.Combine(m_TraceFolder, COPIED_TRACE_FOLDER_NAME)))
                                {
                                    System.IO.File.Move(sSourcePathFilename, sBackupPathfilename);
                                }
                            }
                            catch (Exception)
                            {
                                log("# Could not move '" + sSourcePathFilename + "' to '" + sBackupPathfilename + "'");
                            }
                        }
                    }
                    catch (Exception)
                    {
                        log("# Could not copy '" + sSourcePathFilename + "' to '" + sTargetPathFilename + "'");
                    }

                }
            }

            log("Copied " + iCount.ToString() + " trace data files.");

            // move JBCTraceControl log file
            sSourcePathFilename = logTraceControlPathFilename();
            sTargetPathFilename = Path.Combine(m_USBTraceFolder, TRACE_CONTROL_LOG_NAME + "_" + DateTime.Now.ToString(LOG_TIME_FORMAT) + "." + TRACE_LOG_EXTENSION);
            if (System.IO.File.Exists(sSourcePathFilename))
            {
                try
                {
                    if (isDriveConnected(m_insertedUSBDriveName))
                    {
                        System.IO.File.Move(sSourcePathFilename, sTargetPathFilename);
                        iCount++;
                    }
                }
                catch (Exception)
                {
                    log("# Could not move '" + sSourcePathFilename + "' to '" + sTargetPathFilename + "'");
                }
            }

            playSound(eSounds.CopyEnd, USE_SPEAKER);

            return iCount;
        }

        #endregion

        #region Log

        private void log(string sText)
        {
            logFile(sText);
        }

        private void logFile(string sText)
        {
            //If Not IO.Directory.Exists(logTraceControlPathFilename) Then
            //    IO.Directory.CreateDirectory(logTraceControlPathFilename)
            //End If
            (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(logTraceControlPathFilename(), "[" + DateTime.Now.ToString(LOG_TIME_FORMAT) + "] " + sText + "\r\n", true);
        }

        private string logTraceControlPathFilename()
        {
            return Path.Combine(m_TraceControlLogFolder, TRACE_CONTROL_LOG_NAME + "." + TRACE_LOG_EXTENSION);
        }

        #endregion

        #region USB
        private string m_insertedUSBDriveName = "";

        private void watchDrives()
        {
            // initiate drive watcher
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2 or EventType = 3");
            // asignar evento
            //AddHandler watcher.EventArrived, AddressOf watcher_EventArrived
            // definir evento
            watcher.Query = query;
            // comenzar watching
            watcher.Start();
            //watcher.WaitForNextEvent()
        }

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject wmiObj = e.NewEvent;
            string sInfo = "";
            string sDrive = wmiObj.Properties["DriveName"].Value.ToString(); // DriveName tiene el formato X:
                                                                             // EventType = 2 -> inserted
                                                                             // EventType = 3 -> removed
            string sEventType = wmiObj.Properties["EventType"].Value.ToString();
            // watch only removable


            switch (sEventType)
            {
                case "2":
                    if (getDriveType(sDrive) == DriveType.Removable)
                    {
                        sInfo = "Inserted " + sDrive;
                        //playSound(eSounds.DriveInserted, USE_SPEAKER)
                        if (string.IsNullOrEmpty(m_insertedUSBDriveName))
                        {
                            m_insertedUSBDriveName = sDrive;
                            m_USBTraceFolder = sDrive + "\\" + USB_TRACE_FOLDER_NAME;
                            m_TimerCopyFiles.Start();
                        }
                    }
                    break;

                case "3":
                    sInfo = "Removed " + sDrive;
                    //playSound(eSounds.DriveRemoved, USE_SPEAKER)
                    if (sDrive == m_insertedUSBDriveName)
                    {
                        m_insertedUSBDriveName = "";
                        m_USBTraceFolder = "";
                    }
                    break;
            }
            log(sInfo);
        }

        private System.IO.DriveType getDriveType(string sDriveName)
        {
            sDriveName = sDriveName.Replace("\\", "").ToUpper();
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                // drive.Name tiene el formato X:\
                string drivename = drive.Name.Replace("\\", "").ToUpper();
                if (drivename == sDriveName)
                {
                    return drive.DriveType;
                }
            }
            return System.IO.DriveType.Unknown;
        }

        private bool isDriveConnected(string sDriveName)
        {
            sDriveName = sDriveName.Replace("\\", "").ToUpper();
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                // drive.Name tiene el formato X:\
                string drivename = drive.Name.Replace("\\", "").ToUpper();
                if (drivename == sDriveName)
                {
                    return true;
                }
            }
            return false;
        }

        private void showDrives(bool bUSBOnly)
        {
            bool bSome = false;
            string sInfo = "";
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            foreach (System.IO.DriveInfo drive in drives)
            {
                if (bUSBOnly)
                {
                    switch (drive.DriveType)
                    {
                        case System.IO.DriveType.Removable:
                            bSome = true;
                            sInfo = "Detected " + drive.Name;
                            log(sInfo);
                            break;
                    }
                }
                else
                {
                    bSome = true;
                    sInfo = string.Format("Name: {0} Type: {1}", drive.Name, drive.DriveType.ToString()); // drive.Name tiene el formato X:\
                    log(sInfo);
                }
            }
            if (bSome)
            {
                log("---");
            }
        }

        #endregion

        #region Sounds
        private const bool USE_SPEAKER = true;

        private enum eSounds
        {
            TraceStart,
            TraceEnd,
            TraceError,
            DriveInserted,
            DriveRemoved,
            CopyStart,
            CopyEnd
        }

        private void playSound(eSounds type, bool bSpeaker)
        {
            // Console.Beep(frequency, duration)
            // The frequency ranges between 37 and 32767, although the higher tones are not audible by the human ear.
            // The duration is 1000 for one second.
            switch (type)
            {
                case eSounds.TraceStart:
                    if (bSpeaker)
                    {
                        Console.Beep(800, 1000);
                    }
                    else
                    {
                        System.Media.SystemSounds.Beep.Play();
                    }
                    break;
                case eSounds.TraceEnd:
                    if (bSpeaker)
                    {
                        Console.Beep(800, 250);
                    }
                    else
                    {
                        System.Media.SystemSounds.Beep.Play();
                    }
                    break;
                case eSounds.TraceError:
                    if (bSpeaker)
                    {
                        Console.Beep(800, 3000);
                    }
                    else
                    {
                        System.Media.SystemSounds.Exclamation.Play();
                    }
                    break;
                case eSounds.DriveInserted:
                    if (bSpeaker)
                    {
                        Console.Beep(800, 500);
                    }
                    else
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                    break;
                case eSounds.DriveRemoved:
                    if (bSpeaker)
                    {
                        Console.Beep(800, 250);
                    }
                    else
                    {
                        System.Media.SystemSounds.Asterisk.Play();
                    }
                    break;
                case eSounds.CopyStart:
                    if (bSpeaker)
                    {
                        Console.Beep(800, 500);
                    }
                    else
                    {
                        System.Media.SystemSounds.Hand.Play();
                    }
                    break;
                case eSounds.CopyEnd:
                    if (bSpeaker)
                    {
                        Console.Beep(800, 1000);
                    }
                    else
                    {
                        System.Media.SystemSounds.Beep.Play();
                    }
                    break;
            }

        }

        #endregion

    }
}
