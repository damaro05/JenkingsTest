// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataJBC;

namespace FormTraceAnalysis
{
    public partial class FormTraceAnalysis
    {
        public FormTraceAnalysis()
        {
            //Added to support default instance behavour in C#
            if (defaultInstance == null)
                defaultInstance = this;
        }

        #region Default Instance

        private static FormTraceAnalysis defaultInstance;

        /// <summary>
        /// Added by the VB.Net to C# Converter to support default instance behavour in C#
        /// </summary>
        public static FormTraceAnalysis Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new FormTraceAnalysis();
                    defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
                }

                return defaultInstance;
            }
            set
            {
                defaultInstance = value;
            }
        }

        static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        {
            defaultInstance = null;
        }

        #endregion
        private const string TRACE_FILE_EXTENSION = "json";
        private const string TRACE_TEMP_FILE_EXTENSION = "json.tmp";
        private const string TRACE_LOG_EXTENSION = "log";
        private const string TRACE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const string TRACE_TIME_FORMAT_OLD = "dd/MM/yyyy HH:mm:ss";
        private const string LOG_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const string EXPORT_TIME_FORMAT = "yyyyMMddHHmmss";

        public class CStation
        {
            public string UID;
            public string Name;
            public string Model;
            public string ModelType;
            public string ModelVersion;
            public string Software;
            public string Hardware;

            public CStation(string _UID)
            {
                UID = _UID;
            }
        }

        private DataTable tTrace = null;
        private DataTable tStation = null;
        private DataTable tFiles = null;
        private DataRow rowCurrentFilename = null;
        private DataRow rowCurrentStation = null;
        private DataRow rowCurrentTrace = null;
        private DateTime currentInitialDateTime;
        private int currentInterval;
        private string currentFilename = "";
        private string lastJsonFolder = "";
        private bool bLoadingJsons = false;
        //Private listFilter As New List(Of String)

        public void FormTrace_Load(object sender, EventArgs e)
        {

            if (My.Settings.Default.UpgradeSettings)
            {
                My.Settings.Default.Upgrade();
                My.Settings.Default.UpgradeSettings = false;
                My.Settings.Default.Save();
            }

            timerStart.Start();

        }

        public void FormTrace_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveSettings();
        }

        public void timerStart_Tick(object sender, EventArgs e)
        {
            timerStart.Stop();

            tTrace = ds.Tables["TraceData"];
            tStation = ds.Tables["StationData"];
            tFiles = ds.Tables["LoadedFiles"];

            loadSettings();

            // not sortable
            foreach (DataGridViewColumn col in gridTraceData.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }

        private void loadSettings()
        {
            lastJsonFolder = System.Convert.ToString(My.Settings.Default.lastJsonFolder);
            string settSep = System.Convert.ToString(My.Settings.Default.lastSeparator);
            switch (settSep)
            {
                case ",":
                    rbSepComma.Checked = true;
                    break;
                case ";":
                    rbSepSemicolon.Checked = true;
                    break;
                case "tab":
                    rbSepTab.Checked = true;
                    break;
                default:
                    rbSepSemicolon.Checked = true;
                    break;
            }
        }

        private void saveSettings()
        {
            if (rbSepComma.Checked)
            {
                My.Settings.Default.lastSeparator = ",";
            }
            if (rbSepSemicolon.Checked)
            {
                My.Settings.Default.lastSeparator = ";";
            }
            if (rbSepTab.Checked)
            {
                My.Settings.Default.lastSeparator = "tab";
            }
            My.Settings.Default.Save();
        }

        private void setLastJsonFolder(string folder)
        {
            lastJsonFolder = folder;
            My.Settings.Default.lastJsonFolder = lastJsonFolder;
            saveSettings();
        }

        private void log(string sText)
        {
            tbLog.AppendText("[" + DateTime.Now.ToString(LOG_TIME_FORMAT) + "] " + sText + "\r\n");

        }

        public void gridTraceData_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (bLoadingJsons)
            {
                return;
            }
            int iRow = e.RowIndex;
            string jsonData = System.Convert.ToString(gridTraceData.Rows[iRow].Cells["colgridPortsDataJson"].Value);
            TracePortData[] portsData = getPortsData(jsonData);
            tbPortData.Clear();
            foreach (TracePortData p in portsData)
            {
                tbPortData.AppendText(string.Format("Port:{0} Tool:{1} Status:{2} Temp:{3} Power:{4}" + "\r\n", p.port, p.tool, p.status, p.temperature, p.power));
            }
        }

        #region Filter Stations

        public void gridStations_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (gridStations.Columns.Contains("StationSelected"))
            {
                int iCol = gridStations.Columns["StationSelected"].Index;
                if (e.ColumnIndex == iCol)
                {
                    List<string> UIDS = new List<string>();
                    foreach (DataGridViewRow row in gridStations.Rows)
                    {
                        if ((int)(row.Cells["StationSelected"].Value) == 1)
                        {
                            UIDS.Add(((DataRowView)row.DataBoundItem).Row["UID"].ToString());
                        }
                    }
                    filterTraceDataStations(UIDS.ToArray());
                }
            }
        }

        public void gridStations_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (gridStations.Columns.Contains("StationSelected"))
            {
                int iCol = gridStations.Columns["StationSelected"].Index;
                if (e.ColumnIndex == iCol)
                {
                    gridStations.EndEdit();
                    bindingStations.EndEdit();
                }
            }
        }

        public void gridStations_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void filterTraceDataStations(string[] UIDS)
        {
            Cursor = Cursors.WaitCursor;
            if (UIDS.Length == tStation.Rows.Count)
            {
                bindingTraceData.RemoveFilter();
            }
            else
            {
                string IN_UIDS = "";
                foreach (string uid in UIDS)
                {
                    if (!string.IsNullOrEmpty(IN_UIDS))
                    {
                        IN_UIDS += ",";
                    }
                    IN_UIDS += "'" + uid + "'";
                }
                if (!string.IsNullOrEmpty(IN_UIDS))
                {
                    bindingTraceData.Filter = "UID IN (" + IN_UIDS + ")";
                }
            }
            logTraceInfo();
            Cursor = Cursors.Default;
        }

        #endregion

        #region Load Json files

        public void butReadJson_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lastJsonFolder))
            {
                OpenFileDialog1.InitialDirectory = lastJsonFolder;
            }
            OpenFileDialog1.Multiselect = true;
            if (OpenFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (OpenFileDialog1.FileNames.Length > 0)
                {
                    setLastJsonFolder(System.IO.Path.GetDirectoryName(OpenFileDialog1.FileNames[0]));
                    loadTraceFiles(OpenFileDialog1.FileNames);
                }
            }

        }

        private int loadTraceFiles(string[] FileNames)
        {
            DataRow[] rows = null;
            bool bLoad = true;
            int iCount = 0;

            string jsonData2 = "";
            JObject jsonParse = null;

            Cursor = Cursors.WaitCursor;
            log("Loading " + System.Convert.ToString(FileNames.Length) + " trace files...");
            bLoadingJsons = true;

            foreach (string sPathFilename in FileNames)
            {
                try
                {
                    bLoad = true;

                    currentFilename = System.IO.Path.GetFileName(sPathFilename);
                    rows = tFiles.Select("Filename='" + currentFilename + "'");
                    if (rows.Length == 0)
                    {
                        rowCurrentFilename = tFiles.NewRow();
                        rowCurrentFilename["Filename"] = currentFilename;
                        rowCurrentFilename["PathFilename"] = sPathFilename;
                        tFiles.Rows.Add(rowCurrentFilename);
                    }
                    else
                    {
                        if (Interaction.MsgBox("Already loaded file '" + currentFilename + "'. Replace data?", MsgBoxStyle.YesNo, null) == MsgBoxResult.Yes)
                        {
                            rowCurrentFilename = rows[0];
                            rowCurrentFilename["PathFilename"] = sPathFilename;
                            rowCurrentFilename.AcceptChanges();
                            // delete trace rows already loaded
                            rows = tTrace.Select("Filename='" + currentFilename + "'");
                            foreach (DataRow row in rows)
                            {
                                tTrace.Rows.Remove(row);
                            }
                        }
                        else
                        {
                            bLoad = false;
                        }
                    }

                    if (bLoad)
                    {
                        iCount++;
                        // Imports Newtonsoft.Json.Linq
                        jsonData2 = File.ReadAllText(sPathFilename);
                        jsonParse = JObject.Parse(jsonData2);
                        //Dim results As List(Of JToken) = jsonSearch("sequences").Children().ToList()
                        //Dim results As List(Of JToken) = jsonSearch("").Children().ToList()
                        log("  - " + currentFilename + " - Tags: " + jsonParse.Count.ToString());

                        foreach (JProperty prop in jsonParse.Children())
                        {
                            //log(prop.Name & "-" & prop.Type.ToString & "-" & prop.Value.Type.ToString)
                            loadToken(prop);
                        }
                    }

                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    MessageBox.Show(ex.Message);
                }

            }

            log("Loaded " + System.Convert.ToString(iCount) + " trace files.");

            sortTraceData();

            logTraceInfo();

            bLoadingJsons = false;

            Cursor = Cursors.Default;
            return iCount;

        }

        private void sortTraceData()
        {
            bindingTraceData.Sort = "UID,InitialDateTime,FileSequence";
            log("Sorted trace data.");
        }

        private int getTableRecordCount()
        {
            return ((DataSet)bindingTraceData.DataSource).Tables[bindingTraceData.DataMember].Rows.Count;
        }

        private int getBindingRecordCount()
        {
            return bindingTraceData.Count;
        }

        private void logTraceInfo()
        {
            labRecordCount.Text = "Loaded Trace Records: showing " + System.Convert.ToString(bindingTraceData.Count) + " of " + System.Convert.ToString(((DataSet)bindingTraceData.DataSource).Tables[bindingTraceData.DataMember].Rows.Count);
            log(labRecordCount.Text);
        }

        private void processTables()
        {
            bool bEOF = false;
            log("Processing data...");
            sortTraceData();
            if (bindingTraceData.Count > 0)
            {
                bindingTraceData.MoveFirst();
                while (!bEOF)
                {
                    DataRow row = ((DataRowView)bindingTraceData.Current).Row;
                    int seq = System.Convert.ToInt32(row["FileSequence"]);
                    int intervalMS = System.Convert.ToInt32(row["Interval"]);
                    int MS = (seq + 1) * intervalMS;
                    row["CalculatedDateTime"] = System.Convert.ToDateTime(row["InitialDateTime"]).AddMilliseconds(MS);
                    if (bindingTraceData.Position + 1 < bindingTraceData.Count)
                    {
                        bindingTraceData.MoveNext();
                    }
                    else
                    {
                        bEOF = true;
                    }
                }
            }
            log("Data processed.");

        }

        private void loadToken(JProperty prop)
        {

            DataRow lastRow = null;
            DataRow row = null;
            DataRow[] rows = null;
            string sText = "";

            Application.DoEvents();

            //log(prop.Name & ":" & prop.Value.ToString)
            switch (prop.Name)
            {
                case "id":
                    string UID = prop.Value.ToString();
                    rows = tStation.Select("UID='" + UID + "'");
                    if (rows.Length > 0)
                    {
                        rowCurrentStation = rows[0];
                    }
                    else
                    {
                        rowCurrentStation = tStation.NewRow();
                        rowCurrentStation["UID"] = UID;
                        tStation.Rows.Add(rowCurrentStation);
                    }

                    rowCurrentFilename["UID"] = UID;
                    break;

                case "type":
                    if (Information.IsDBNull(rowCurrentStation["StationType"]))
                    {
                        rowCurrentStation["StationType"] = prop.Value.ToString();
                    }
                    break;

                case "model":
                    if (Information.IsDBNull(rowCurrentStation["StationModel"]))
                    {
                        rowCurrentStation["StationModel"] = prop.Value.ToString();
                    }
                    break;

                case "name":
                    if (Information.IsDBNull(rowCurrentStation["StationName"]))
                    {
                        rowCurrentStation["StationName"] = prop.Value.ToString();
                    }
                    break;

                case "modeltype":
                    if (Information.IsDBNull(rowCurrentStation["StationModelType"]))
                    {
                        rowCurrentStation["StationModelType"] = prop.Value.ToString();
                    }
                    break;

                case "modelversion":
                    if (Information.IsDBNull(rowCurrentStation["StationModelVersion"]))
                    {
                        rowCurrentStation["StationModelVersion"] = prop.Value.ToString();
                    }
                    break;

                case "hardware":
                    if (Information.IsDBNull(rowCurrentStation["StationHW"]))
                    {
                        rowCurrentStation["StationHW"] = prop.Value.ToString();
                    }
                    break;

                case "software":
                    if (Information.IsDBNull(rowCurrentStation["StationSW"]))
                    {
                        rowCurrentStation["StationSW"] = prop.Value.ToString();
                    }
                    break;

                case "time":
                    sText = prop.Value.ToString();
                    try
                    {
                        currentInitialDateTime = DateTime.ParseExact(sText, TRACE_TIME_FORMAT, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        currentInitialDateTime = DateTime.ParseExact(sText, TRACE_TIME_FORMAT_OLD, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    break;

                case "interval":
                    currentInterval = int.Parse(prop.Value.ToString());
                    break;

                default:
                    // sequence
                    if (Information.IsNumeric(prop.Name))
                    {

                        // first sequence
                        if (prop.Name.ToString() == "0")
                        {
                            rowCurrentStation.AcceptChanges();
                        }

                        rowCurrentTrace = tTrace.NewRow();
                        rowCurrentTrace["UID"] = rowCurrentStation["UID"];
                        rowCurrentTrace["InitialDateTime"] = currentInitialDateTime;
                        rowCurrentTrace["Interval"] = currentInterval;
                        rowCurrentTrace["PortsDataJson"] = prop.Value.ToString().Replace("\r\n", "");
                        rowCurrentTrace["Filename"] = currentFilename;
                        rowCurrentTrace["FileSequence"] = int.Parse(prop.Name);
                        tTrace.Rows.Add(rowCurrentTrace);

                        //Select Case prop.Value.Type
                        //    Case JTokenType.Array
                        //        ' obtener el array
                        //        Dim arr As JArray = prop.Value
                        //        ' cda array será un objeto
                        //        For Each elem As JObject In arr
                        //            For Each prop2 As JProperty In elem.Children
                        //                loadToken(prop2)
                        //            Next
                        //        Next

                        //    Case Else

                        //End Select

                    }
                    break;

            }

        }

        private TracePortData[] getPortsData(string jsondata)
        {
            List<TracePortData> data = new List<TracePortData>();

            // parse the array
            JArray arr = (JArray)(JArray.Parse(jsondata));

            foreach (JObject elem in arr)
            {
                // create port data
                TracePortData portdata = new TracePortData();

                foreach (JProperty propPortElem in elem.Children())
                {
                    switch (propPortElem.Name)
                    {

                        case "p":
                            //port
                            portdata.port = (Port)(int.Parse(propPortElem.Value.ToString()));
                            break;
                        case "o":
                            //tool
                            portdata.tool = (GenericStationTools)(int.Parse(propPortElem.Value.ToString()));
                            break;
                        case "s":
                            //status
                            portdata.status = byte.Parse(propPortElem.Value.ToString());
                            break;
                        case "t":
                            //temp
                            portdata.temperature = int.Parse(propPortElem.Value.ToString());
                            break;
                        case "w":
                            //power
                            portdata.power = int.Parse(propPortElem.Value.ToString());
                            break;

                        case "f":
                            //caudal (JTSE)
                            portdata.flow = int.Parse(propPortElem.Value.ToString());
                            break;

                        case "x1":
                            //external TC1 (JTSE)
                            portdata.tempTC1 = int.Parse(propPortElem.Value.ToString());
                            break;

                        case "x2":
                            //external TC1 (JTSE)
                            portdata.tempTC2 = int.Parse(propPortElem.Value.ToString());
                            break;

                        case "ts":
                            //time to stop (JTSE)
                            portdata.timetostop = int.Parse(propPortElem.Value.ToString());
                            break;
                        default:
                            break;


                    }
                }

                // add port data
                data.Add(portdata);
            }

            return data.ToArray();

        }

        #endregion

        #region Export Data

        public void butExportData_Click(object sender, EventArgs e)
        {
            exportData("C:\\Midori\\Desarr\\JBC\\Laboratorio\\Varios\\Ejemlos Register");
        }

        private void exportData(string folder)
        {
            bool bHeader = false;
            string sHeader = "";
            string sRec = "";
            int iMS = 0;
            string currentUID = "";
            string filename = "";
            List<string> listFilenames = new List<string>();
            string sSep = "";

            Cursor = Cursors.WaitCursor;

            if (rbSepComma.Checked)
            {
                sSep = ",";
            }
            if (rbSepSemicolon.Checked)
            {
                sSep = ";";
            }
            if (rbSepTab.Checked)
            {
                sSep = System.Convert.ToString("\t");
            }

            foreach (DataGridViewRow row in gridTraceData.Rows)
            {
                if (row.Cells["colgridUID"].Value != currentUID)
                {
                    currentUID = System.Convert.ToString(row.Cells["colgridUID"].Value);
                    filename = Path.Combine(folder, getNameForFilename(currentUID) + "_" + DateTime.Now.ToString(EXPORT_TIME_FORMAT) + ".csv");
                    if (!listFilenames.Contains(filename))
                    {
                        listFilenames.Add(filename);
                    }
                }
                iMS += System.Convert.ToInt32(row.Cells["colgridInterval"].Value);
                string jsonData = System.Convert.ToString(row.Cells["colgridPortsDataJson"].Value);
                TracePortData[] portsData = getPortsData(jsonData);

                if (!bHeader)
                {
                    sHeader = "Time (ms)";
                }
                sRec = iMS.ToString();
                foreach (TracePortData p in portsData)
                {
                    if (!bHeader)
                    {
                        sHeader += sSep + "Tool_" + ((System.Convert.ToInt32(p.port)) + 1).ToString();
                        sHeader += sSep + "Temp_" + ((System.Convert.ToInt32(p.port)) + 1).ToString() + " (ºC)";
                        sHeader += sSep + "Power_" + ((System.Convert.ToInt32(p.port)) + 1).ToString() + " (%)";
                        sHeader += sSep + "Status_" + ((System.Convert.ToInt32(p.port)) + 1).ToString();
                    }
                    CTemperature temp = new CTemperature(p.temperature);
                    sRec += sSep + p.tool.ToString();
                    sRec += sSep + temp.ToCelsius().ToString();
                    sRec += sSep + (p.power / 10).ToString();
                    sRec += sSep + p.status.ToString();
                }
                if (!bHeader)
                {
                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(filename, sHeader + "\r\n", true);
                    bHeader = true;
                }
                (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(filename, sRec + "\r\n", true);
            }

            foreach (string fil in listFilenames)
            {
                log("Exported to '" + System.IO.Path.GetFileName(fil) + "' in '" + System.IO.Path.GetDirectoryName(fil) + "'");
            }

            Cursor = Cursors.Default;
        }

        private string getNameForFilename(string sName)
        {
            string ret = "";
            const string notAllowed = "\\/:*?\"<>|";
            foreach (char car in sName)
            {
                if (notAllowed.IndexOf(car.ToString()) + 1 == 0)
                {
                    ret += car.ToString();
                }
                else
                {
                    ret += Strings.Asc(car).ToString();
                }
            }

            return ret;
        }

        #endregion



    }
}
