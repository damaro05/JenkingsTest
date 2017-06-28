// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

using System.Threading.Tasks;
using System.Threading;
//using JBC_ConnectRemote.JBC_API_Remote;
using Microsoft.VisualBasic.CompilerServices;
using JBC_ConnectRemote;

// 19/09/2013 se añade "If rowIdx >= 0 Then" en función updateStationCheckControl de frmConfManager.vb
//            porque da error si cambio el control mode pero la estación no está listada en Settings Manager


namespace RemoteManager
{
    public partial class frmConfManager
    {

        private struct tConfStation
        {
            public ListViewItem dockitem;
            public long ID;
            //Public frm As frmStation
            public string stationCOM;
            public int iListRowStn;
            public bool bActive;
            public bool bListed;
        }

        private struct tApplyStation
        {
            public long ID;
            public int[] aTargetFromSourcePorts; // source/target ports matching for this station
            public bool bConfirmedApply; // TransactionFinished event received
            public string stnName;
            public System.Windows.Forms.Timer stnTimer;
            public int iListRow;
            public uint uiTransaction;
        }

        private struct tStationRow
        {
            public int iListRow;
            public bool bShow;
            public CheckBox checkStation;
            public CheckBox checkControlMode;
            public ComboBox[] comboPort;
            public Label labelMsg;
        }

        const int TIME_REFRESH_STATION_LIST = 5000;


        private JBC_API_Remote jbc;
        private List<frmMain.tStation> mainStationList;
        private List<tConfStation> confStationList;
        private List<tApplyStation> applyStationList;
        private int iApplied = 0;
        private List<tStationRow> rowList;
        private bool bGroupedStationList = false;
        private ParamTree oTree = null;
        private int layoutRowHeight = 26;
        private bool bLoggedIn;
        private int iCurrentRowList = 0;
        private System.Xml.XmlDocument xmlDocToApply;
        private System.Xml.XmlDocument xmlLog;
        private string sLastLogXmlPathFileName;
        //Private lApplyTimeOut As Long = 60000
        private long lApplyTimeOut = 2000;
        private int lApplyTimeOutCount = 30;
        private Thread m_ThreadRefreshStationList;


        public frmConfManager(List<frmMain.tStation> stationListRef, JBC_API_Remote jbcRef, bool bSuperLoggedIn)
        {
            // Required by the designer
            InitializeComponent();

            mainStationList = stationListRef;
            jbc = jbcRef;
            bLoggedIn = bSuperLoggedIn;

        }

        public void frmConfManager_Load(System.Object sender, System.EventArgs e)
        {
            cbFilterType.SelectedIndex = 0;

            clearDesignRowsControls();
            confStationList = new List<tConfStation>();
            rowList = new List<tStationRow>();
            applyStationList = new List<tApplyStation>();
            // get station list
            int iAdded = 0;
            int iInactives = 0;
            updateStationList(ref iAdded, ref iInactives);
            ReLoadTexts();
            butConfViewReport.Enabled = false;
            labLogApply.Text = "";

            m_ThreadRefreshStationList = new Thread(new System.Threading.ThreadStart(RefreshStationList));
            m_ThreadRefreshStationList.IsBackground = true;
            m_ThreadRefreshStationList.Start();
        }

        // --- Localization
        public void ReLoadTexts()
        {
            // titles
            labConfSourceSettings.Text = Localization.getResStr(Configuration.confSourceSettingsId);
            labConfTargetStations.Text = Localization.getResStr(Configuration.confTargetStationsListId);
            labConfComment.Text = Localization.getResStr(Configuration.confConfCommentId);

            // list
            labStation.Text = Localization.getResStr(Configuration.confStationId);
            labConfStationFilter.Text = Localization.getResStr(Configuration.confFiltersId);
            labConfModelFilter.Text = Localization.getResStr(Configuration.confFiltersModelId);
            labConfNameFilter.Text = Localization.getResStr(Configuration.confFiltersNameId);
            Control labControl = null;
            for (var i = 1; i <= 4; i++)
            {
                if (Configuration.myControlExists(this, "labTargetPort_" + i.ToString(), ref labControl))
                {
                    ((Label)labControl).Text = string.Format(Localization.getResStr(Configuration.confTargetPortId), i.ToString());
                }
            }

            // buttons
            butConfLoad.Text = Localization.getResStr(Configuration.confLoadId);
            butConfSave.Text = Localization.getResStr(Configuration.confSaveId);
            butPrint.Text = Localization.getResStr(Configuration.browPrintId);
            if (rowList.Count == 0)
            {
                butRefreshStations.Text = Localization.getResStr(Configuration.confViewStationsId);
            }
            else
            {
                butRefreshStations.Text = Localization.getResStr(Configuration.confRefreshStationsId);
            }
            butConfApply.Text = Localization.getResStr(Configuration.confApplyId);
            butConfViewReport.Text = Localization.getResStr(Configuration.confViewReportId);

            // tree
            Control ctrl = null;
            if (Configuration.myControlExists(this, Configuration.sTreeName, ref ctrl))
            {
                ((ParamTree)ctrl).ReLoadTexts();
            }

        }

        #region Source Settings (tree)

        public void butConfLoad_Click(System.Object sender, System.EventArgs e)
        {
            OpenFileDialog ldgXmlLoad = new OpenFileDialog();
            ldgXmlLoad.Filter = " (*.xml)|*.xml";
            if (ldgXmlLoad.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Loading
                xmlLoadToTree(ldgXmlLoad.FileName);
            }

        }

        public bool xmlLoadToTree(string filename)
        {
            bool returnValue = false;
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            Control ctrl = null;
            returnValue = false;
            string sError = "";
            xmlDoc = RoutinesLibrary.Data.Xml.XMLUtils.LoadFromFile(filename, ref sError);
            if (xmlDoc != null)
            {
                returnValue = xmlXmlToTree(xmlDoc);
            }
            return returnValue;
        }

        public bool xmlXmlToTree(System.Xml.XmlDocument xmlDoc)
        {
            bool returnValue = false;
            Control ctrl = null;
            string sComment = "";
            returnValue = false;
            if (xmlDoc != null)
            {
                if (Configuration.myControlExists(this, Configuration.sTreeName, ref ctrl))
                {
                    this.Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
                oTree = Configuration.confXmlToParamTree(xmlDoc, ref sComment);
                if (oTree != null)
                {
                    oTree.Name = Configuration.sTreeName;
                    oTree.Dock = DockStyle.Fill;
                    oTree.EditMode = true;
                    oTree.AllowDrop = true;
                    oTree.DragDrop += onTree_DragDrop;
                    oTree.DragEnter += onTree_DragEnter;
                    this.panelTreeView.Controls.Add(oTree);
                    this.Update();
                    tbConfComment.Text = sComment;
                    returnValue = true;
                }
            }
            return returnValue;
        }

        public void butConfSave_Click(System.Object sender, System.EventArgs e)
        {
            SaveFileDialog ldgXmlSave = new SaveFileDialog();
            Control ctrl = null;
            if (Configuration.myControlExists(this, Configuration.sTreeName, ref ctrl))
            {
                ldgXmlSave.Filter = " (*.xml)|*.xml";
                if (ldgXmlSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Saving
                    xmlSaveFromTree(ldgXmlSave.FileName);
                }
            }
            else
            {
                MessageBox.Show(Localization.getResStr(Configuration.confWarnNoConfTreeId));
            }
        }

        private void xmlSaveFromTree(string filename)
        {
            Control ctrl = null;
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            if (Configuration.myControlExists(this, Configuration.sTreeName, ref ctrl))
            {
                xmlDoc = Configuration.confParamTreeToXml((ParamTree)ctrl, true, tbConfComment.Text);
                if (xmlDoc != null)
                {
                    xmlDoc.Save(filename);
                }
            }
        }

        public void butPrint_Click(System.Object sender, System.EventArgs e)
        {
            Control ctrl = null;
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            Form oForm;
            string sXmlPathFileName = "";
            if (Configuration.myControlExists(this, Configuration.sTreeName, ref ctrl))
            {
                xmlDoc = Configuration.confParamTreeToXml((ParamTree)ctrl, true, tbConfComment.Text);
                if (xmlDoc != null)
                {
                    sXmlPathFileName = Configuration.confSaveConfXml(xmlDoc, Localization.getResStr(Configuration.xslStationConfigurationId));
                    if (!string.IsNullOrEmpty(sXmlPathFileName))
                    {
                        oForm = Configuration.showXML(sXmlPathFileName, true);
                    }
                }
            }
            else
            {
                MessageBox.Show(Localization.getResStr(Configuration.confWarnNoConfTreeId));
            }
        }

        public void onTree_DragEnter(System.Object sender, System.Windows.Forms.DragEventArgs e)
        {
            string sData = "";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // If the data is a file, display the copy cursor.
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                // If the data is a station ID, display the copy cursor.
                sData = System.Convert.ToString(e.Data.GetData(DataFormats.Text));
                if (sData.IndexOf("ID=") + 1 > 0)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        public void onTree_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string sData = "";
            long myID = 0;
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // load file
                string[] aFiles = (string[])(e.Data.GetData(DataFormats.FileDrop));
                string sFileName = "";
                sFileName = aFiles[0];
                if (!xmlLoadToTree(sFileName))
                {
                    MessageBox.Show("Cannot load configuration file.");
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                // If the data is a station ID, display the copy cursor.
                sData = System.Convert.ToString(e.Data.GetData(DataFormats.Text));
                if (sData.IndexOf("ID=") + 1 > 0)
                {
                    sData = sData.Replace("ID=", "");
                    try
                    {
                        myID = Convert.ToUInt32(sData);
                        // get settings from station in xml format
                        xmlDoc = Configuration.confGetFromStation(myID, jbc, false);
                        if (xmlDoc != null)
                        {
                            if (!xmlXmlToTree(xmlDoc))
                            {
                                MessageBox.Show("Cannot show station settings.");
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }

        }

        #endregion

        #region Target Stations List

        public void set_layColumnWidth(int index, int value)
        {
            if (index < tlpStationList.ColumnCount)
            {
                tlpStationList.ColumnStyles[index].SizeType = SizeType.Absolute;
                tlpStationList.ColumnStyles[index].Width = value;
            }
        }
        public int get_layColumnWidth(int index)
        {
            if (index < tlpStationList.ColumnCount)
            {
                int[] mWidths = tlpStationList.GetColumnWidths();
                return mWidths[index];
            }
            else
            {
                return 0;
            }
        }

        public void set_layRowHeight(int index, int value)
        {
            if (index < tlpStationList.RowCount)
            {
                tlpStationList.RowStyles[index].SizeType = SizeType.Absolute;
                tlpStationList.RowStyles[index].Height = value;
            }
        }
        public int get_layRowHeight(int index)
        {
            if (index < tlpStationList.RowCount)
            {
                int[] mHeights = tlpStationList.GetRowHeights();
                return mHeights[index];
            }
            else
            {
                return 0;
            }
        }

        public int layDefaultRowHeight
        {
            set
            {
                layoutRowHeight = value;
                foreach (RowStyle row in tlpStationList.RowStyles)
                {
                    row.SizeType = SizeType.Absolute;
                    row.Height = value;
                }
            }
            get
            {
                return layoutRowHeight;
            }
        }

        private int getStationIndex(long stationID)
        {
            //looking for the station in the list
            bool found = false;
            int cnt = 0;
            while (cnt < confStationList.Count && !found)
            {
                if (confStationList[cnt].ID == stationID)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            //returning the search result
            if (found)
            {
                return cnt;
            }
            else
            {
                return -1;
            }
        }

        private int getApplyStationIndex(long stationID)
        {
            //looking for the station in the list
            bool found = false;
            int cnt = 0;
            while (cnt < applyStationList.Count && !found)
            {
                if (applyStationList[cnt].ID == stationID)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            //returning the search result
            if (found)
            {
                return cnt;
            }
            else
            {
                return -1;
            }
        }

        private int getMainStationIndex(long stationID)
        {
            //looking for the station in the list
            bool found = false;
            int cnt = 0;
            while (cnt < mainStationList.Count && !found)
            {
                if (mainStationList[cnt].ID == stationID)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            //returning the search result
            if (found)
            {
                return cnt;
            }
            else
            {
                return -1;
            }
        }

        private List<long> getStationsIDByListRow(int iListRow)
        {
            //looking for the stations in the list that have the iListRow number
            // cannot have iRow = 0
            List<long> list = new List<long>();
            if (iListRow == 0)
            {
                return list;
            }
            int cnt = 0;
            while (cnt < confStationList.Count)
            {
                if (confStationList[cnt].iListRowStn == iListRow)
                {
                    list.Add(confStationList[cnt].ID);
                }
                cnt++;
            }

            return list;
        }

        private int getRowIndex(int iListRow)
        {
            //looking for the row in the rows list
            bool found = false;
            int cnt = 0;
            while (cnt < rowList.Count && !found)
            {
                if (rowList[cnt].iListRow == iListRow)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            //returning the search result
            if (found)
            {
                return cnt;
            }
            else
            {
                return -1;
            }
        }

        public void butRefreshStations_Click(System.Object sender, System.EventArgs e)
        {
            int iRow = 0;
            tConfStation stn = new tConfStation();
            bool bListFilter = false;

            // update station list
            int iAdded = 0;
            int iInactives = 0;
            updateStationList(ref iAdded, ref iInactives);

            // add rows for new stations
            // delete rows for deleted stations
            for (var i = 0; i <= confStationList.Count - 1; i++)
            {
                stn = confStationList[System.Convert.ToInt32(i)];

                if (stn.bActive)
                {
                    // filters
                    bListFilter = true;
                    if (!myCheckFilter(Strings.LCase(stn.dockitem.SubItems[Configuration.subItemTypeId].Text), System.Convert.ToString(cbFilterType.SelectedIndex + 1)))
                    {
                        bListFilter = false;
                    }
                    if (!myCheckFilter(Strings.LCase(stn.dockitem.SubItems[Configuration.subItemModelId].Text), tbFilterModel.Text.ToLower()))
                    {
                        bListFilter = false;
                    }
                    if (!myCheckFilter(stn.dockitem.Text.ToLower(), tbFilterName.Text.ToLower()))
                    {
                        bListFilter = false;
                    }

                    // list the station
                    if (bListFilter)
                    {
                        // if not listed yet, add the station to the list row
                        if (!stn.bListed)
                        {
                            // add row (returned iRow is the index added)
                            iRow = addRowList(stn);
                            // save index in station
                            stn.iListRowStn = iRow;
                            stn.bListed = true;
                        }
                        else
                        {
                            // if listed, show
                            showRowList(stn.iListRowStn, true);
                        }
                    }
                    else
                    {
                        // if listed, hide from list
                        if (stn.bListed)
                        {
                            showRowList(stn.iListRowStn, false);
                        }
                    }
                }
                else
                {
                    // not active stations
                    // if listed, remove from list
                    if (stn.bListed)
                    {
                        removeRowList(stn.iListRowStn);
                    }
                    stn.bListed = false;
                    stn.iListRowStn = 0;
                }
                confStationList[System.Convert.ToInt32(i)] = stn;
            }

            // fill checkboxes and comoboboxes
            setTargetPorts();

            if (rowList.Count == 0)
            {
                butRefreshStations.Text = Localization.getResStr(Configuration.confViewStationsId);
            }
            else
            {
                butRefreshStations.Text = Localization.getResStr(Configuration.confRefreshStationsId);
            }

        }

        private bool myCheckFilter(string sText, string sFilter)
        {
            bool returnValue = false;
            returnValue = true;
            if (sFilter != "")
            {
                if (sFilter.EndsWith("*") && sFilter.StartsWith("*") && sFilter.Length > 2)
                {
                    if (!sText.Contains(sFilter.Substring(1, sFilter.Length - 2)))
                    {
                        returnValue = false;
                    }
                }
                else if (sFilter.EndsWith("*") && sFilter.Length > 1)
                {
                    if (!sText.StartsWith(sFilter.Substring(0, sFilter.Length - 1)))
                    {
                        returnValue = false;
                    }
                }
                else if (sFilter.StartsWith("*") && sFilter.Length > 1)
                {
                    if (!sText.EndsWith(sFilter.Substring(1, sFilter.Length - 1)))
                    {
                        returnValue = false;
                    }
                }
                else
                {
                    if (!sText.Contains(sFilter))
                    {
                        returnValue = false;
                    }
                }
            }
            return returnValue;
        }

        private int updateStationList(ref int iAdded, ref int iDeleted)
        {
            // returns total active stations
            int stationIdx = -1;
            tConfStation confStn = new tConfStation();
            int iActive = 0;

            iAdded = 0;
            iDeleted = 0;

            // mark all stations as not active
            for (var i = 0; i <= confStationList.Count - 1; i++)
            {
                confStn = confStationList[System.Convert.ToInt32(i)];
                confStn.bActive = false;
                confStationList[System.Convert.ToInt32(i)] = confStn;
            }

            // update stations
            foreach (var stnMain in mainStationList)
            {
                // search in local list
                stationIdx = getStationIndex(System.Convert.ToInt64(stnMain.ID));
                if (stationIdx == -1)
                {
                    //if not found, search with name+model+serialport+SW (station reconnected)
                    for (var i = 0; i <= confStationList.Count - 1; i++)
                    {
                        confStn = confStationList[System.Convert.ToInt32(i)];
                        if (confStn.stationCOM == stnMain.stationCOM &&
                                confStn.dockitem.Text == stnMain.item.Text &&
                                confStn.dockitem.SubItems[Configuration.subItemModelId].Text == stnMain.item.SubItems[Configuration.subItemModelId].Text &&
                                confStn.dockitem.SubItems[Configuration.subItemSWId].Text == stnMain.item.SubItems[Configuration.subItemSWId].Text &&
                                confStn.dockitem.SubItems[Configuration.subItemTypeId].Text == stnMain.item.SubItems[Configuration.subItemTypeId].Text)
                        {
                            // same station with new iD
                            confStn.ID = System.Convert.ToInt64(stnMain.ID);
                            // form
                            //confStn.frm = stnMain.frm
                            // save control mode of this window
                            bool bControlMode = ((Configuration.tStationDataItemList)confStn.dockitem.Tag).bControlMode;
                            // copy item from reconnected station
                            confStn.dockitem = stnMain.item;
                            // change list item
                            confStationList[System.Convert.ToInt32(i)] = confStn;
                            // set mode same as this window
                            Configuration.setStationControlMode(jbc, confStn.ID, ref confStn.dockitem, bControlMode);
                            // get idx
                            stationIdx = getStationIndex(System.Convert.ToInt64(stnMain.ID));
                            break;
                        }
                    }
                }
                // if not found, add new
                if (stationIdx == -1)
                {
                    confStn = new tConfStation();
                    confStn.ID = System.Convert.ToInt64(stnMain.ID);
                    confStn.dockitem = stnMain.item;
                    //confStn.frm = stnMain.frm
                    confStn.stationCOM = System.Convert.ToString(stnMain.stationCOM);
                    confStn.bActive = true;
                    confStn.bListed = false;
                    confStationList.Add(confStn);
                    iActive++;
                    iAdded++;
                }
                else
                {
                    confStn = confStationList[stationIdx];
                    confStn.bActive = true;
                    confStationList[stationIdx] = confStn;
                    iActive++;
                }
            }

            for (var i = 0; i <= confStationList.Count - 1; i++)
            {
                confStn = confStationList[System.Convert.ToInt32(i)];
                if (!confStn.bActive)
                {
                    iDeleted++;
                }
                //confStationList.Item(i) = confStn
            }

            return iActive;

        }

        private int addRowList(tConfStation stn)
        {
            tStationRow row = new tStationRow();
            int iPorts = 0;

            // index being added
            iCurrentRowList++;
            var iIndexRowBase1 = iCurrentRowList;
            row.iListRow = iIndexRowBase1;
            row.bShow = true;

            // mode
            row.checkControlMode = new CheckBox();
            row.checkControlMode.AutoSize = false;
            row.checkControlMode.Size = new Size(25, 25);
            row.checkControlMode.Name = "checkControlMode_" + iIndexRowBase1.ToString();
            row.checkControlMode.Appearance = Appearance.Button;
            row.checkControlMode.BackColor = Color.Transparent;
            row.checkControlMode.FlatStyle = FlatStyle.Flat;
            row.checkControlMode.FlatAppearance.BorderSize = 0;
            row.checkControlMode.FlatAppearance.CheckedBackColor = Color.Transparent;
            row.checkControlMode.FlatAppearance.MouseDownBackColor = Color.Transparent;
            row.checkControlMode.FlatAppearance.MouseOverBackColor = Color.Transparent;
            row.checkControlMode.Margin = new Padding(0);
            row.checkControlMode.Cursor = Cursors.Hand;

            if (((Configuration.tStationDataItemList)stn.dockitem.Tag).bControlMode)
            {
                // control mode
                row.checkControlMode.Image = My.Resources.Resources.unlock;
                row.checkControlMode.Checked = true;
            }
            else
            {
                // monitor mode
                row.checkControlMode.Image = My.Resources.Resources._lock;
                row.checkControlMode.Checked = false;
            }
            row.checkControlMode.Click += row_checkControlMode_Click;

            // station
            row.checkStation = new CheckBox();
            row.checkStation.Name = "checkStn_" + iIndexRowBase1.ToString();
            row.checkStation.AutoSize = true;
            row.checkStation.Text = stn.dockitem.SubItems[Configuration.subItemModelId].Text + " - " + stn.dockitem.Text;
            row.checkStation.Margin = new Padding(4, 3, 4, 3);

            // ports
            iPorts = jbc.GetPortCount(stn.ID);
            ComboBox[] ports = new ComboBox[11];
            row.comboPort = ports;
            for (var i = 1; i <= iPorts; i++)
            {
                row.comboPort[i - 1] = new ComboBox();
                row.comboPort[i - 1].Name = "comboPort_" + iIndexRowBase1.ToString() + "_ " + i.ToString();
                row.comboPort[i - 1].DropDownStyle = ComboBoxStyle.DropDownList;
                row.comboPort[i - 1].Anchor = (int)AnchorStyles.Top + AnchorStyles.Right;
                row.comboPort[i - 1].Width = 46;
                row.comboPort[i - 1].Height = 22;
                //tlpStationList.Controls.Add(row.comboPort(i - 1), i + 1, tlpStationList.RowCount - 1)
                //tlpStationList.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            }

            // message label
            row.labelMsg = new Label();
            row.labelMsg.Name = "labelMsg_" + iIndexRowBase1.ToString();
            row.labelMsg.AutoSize = false;
            row.labelMsg.Dock = DockStyle.Fill;
            row.labelMsg.TextAlign = ContentAlignment.MiddleLeft;

            rowList.Add(row);

            refreshRowsControls();

            return iIndexRowBase1;
        }

        private void setTargetPorts()
        {
            // insert source selected ports in target ports combo boxes
            List<TreeNode> nodes = null;
            List<string> ports = new List<string>();
            string sDefaultPort = "";
            string sCurrentPort = "";
            bool bCurrentPortIsValid = false;

            // get selected ports in source
            if (oTree != null)
            {
                oTree.getNodesName(null, "Port", ref nodes);
                if (nodes != null)
                {
                    foreach (TreeNode node in nodes)
                    {
                        if (node.Checked)
                        {
                            ports.Add(((ParamTree.tParam)node.Tag).iPort.ToString());
                        }
                    }
                    ports.Sort();
                }
            }

            // insert ports and 'No' in target ports
            // set default port in each combo box
            int i = 0;
            foreach (tStationRow row in rowList)
            {
                i = 0;
                while (row.comboPort[i] != null)
                {
                    sDefaultPort = "";
                    sCurrentPort = row.comboPort[i].Text;
                    row.comboPort[i].Items.Clear();
                    bCurrentPortIsValid = false;
                    foreach (string port in ports)
                    {
                        row.comboPort[i].Items.Add(port);
                        // get the default port based on source ports
                        if ((i + 1).ToString() == port)
                        {
                            sDefaultPort = port;
                        }
                        // if there is a selected port, see if it is already valid
                        if (port == sCurrentPort)
                        {
                            bCurrentPortIsValid = true;
                        }
                    }
                    row.comboPort[i].Items.Add(Localization.getResStr(Configuration.confNoId));
                    if (bCurrentPortIsValid || sCurrentPort == Localization.getResStr(Configuration.confNoId))
                    {
                        row.comboPort[i].SelectedIndex = System.Convert.ToInt32(row.comboPort[i].Items.IndexOf(sCurrentPort));
                    }
                    else if (!string.IsNullOrEmpty(sDefaultPort))
                    {
                        row.comboPort[i].SelectedIndex = System.Convert.ToInt32(row.comboPort[i].Items.IndexOf(sDefaultPort));
                    }
                    else
                    {
                        row.comboPort[i].SelectedIndex = System.Convert.ToInt32(row.comboPort[i].Items.IndexOf(Localization.getResStr(Configuration.confNoId)));
                    }

                    i++;
                }
            }

        }

        private void clearRowList()
        {
            // clear rowList and table layout panel controls
            if (rowList != null)
            {
                if (rowList.Count > 0)
                {
                    for (var i = rowList.Count - 1; i >= 0; i--)
                    {
                        removeRowList(System.Convert.ToInt32(rowList[i].iListRow));
                    }
                }
            }
        }

        private void showRowList(int iListRow, bool bShow)
        {
            // set show
            int iRowIdx = getRowIndex(iListRow);
            if (iRowIdx > -1)
            {
                tStationRow row = rowList[iRowIdx];
                row.bShow = bShow;
                rowList[iRowIdx] = row;
                // relocate controls
                refreshRowsControls();
            }
        }

        private void removeRowList(int iListRow)
        {
            int iRowIdx = -1;

            // remove row and table layout panel controls
            iRowIdx = getRowIndex(iListRow);
            if (iRowIdx > -1)
            {
                // dispose objects
                tStationRow row = rowList[iRowIdx];

                // control mode (check box)
                tlpStationList.Controls.Remove(row.checkControlMode);
                row.checkControlMode.Dispose();
                row.checkControlMode = null;

                // station selection (check box)
                tlpStationList.Controls.Remove(row.checkStation);
                row.checkStation.Dispose();
                row.checkStation = null;

                // ports (combo boxes)
                foreach (ComboBox port in row.comboPort)
                {
                    if (port != null)
                    {
                        tlpStationList.Controls.Remove(port);
                        port.Dispose();
                        // port = null; VBConversions Warning: A foreach variable can't be assigned to in C#.
                    }
                }

                // message label
                row.labelMsg.Dispose();
                row.labelMsg = null;

                // remove row
                rowList.RemoveAt(iRowIdx);
                // relocate controls
                refreshRowsControls();
            }
        }

        private void refreshRowsControls()
        {
            tStationRow row = new tStationRow();
            int itlpRow = 0; // table layout panel controls row index

            // refresh table layout panel controls based on rowList (bShow = true)
            if (rowList != null)
            {
                if (rowList.Count > 0)
                {
                    //rowList.Sort()
                    for (var iRowIdx = 0; iRowIdx <= rowList.Count - 1; iRowIdx++)
                    {
                        row = rowList[System.Convert.ToInt32(iRowIdx)];
                        if (row.bShow)
                        {
                            // new row
                            itlpRow++;
                            // adjust tpl.count, if growing
                            if (itlpRow >= tlpStationList.RowCount)
                            {
                                tlpStationList.RowCount++;
                            }
                            if (tlpStationList.Contains(row.checkControlMode))
                            {
                                tlpStationList.SetRow(row.checkControlMode, itlpRow);
                                tlpStationList.SetRow(row.checkStation, itlpRow);
                                foreach (ComboBox port in row.comboPort)
                                {
                                    if (port != null)
                                    {
                                        tlpStationList.SetRow(port, itlpRow);
                                    }
                                }
                                tlpStationList.SetRow(row.labelMsg, itlpRow);
                            }
                            else
                            {
                                tlpStationList.Controls.Add(row.checkControlMode, 0, itlpRow);
                                tlpStationList.Controls.Add(row.checkStation, 1, itlpRow);
                                for (var i = 0; i <= row.comboPort.Count() - 1; i++)
                                {
                                    if (row.comboPort[(int)i] != null)
                                    {
                                        tlpStationList.Controls.Add(row.comboPort[(int)i], System.Convert.ToInt32(i + 2), itlpRow);
                                        tlpStationList.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                                    }
                                }
                                tlpStationList.Controls.Add(row.labelMsg, 6, itlpRow);
                                set_layRowHeight(itlpRow, 26);
                            }
                        }
                        else
                        {
                            // not show
                            if (tlpStationList.Contains(row.checkControlMode))
                            {
                                tlpStationList.Controls.Remove(row.checkControlMode);
                                tlpStationList.Controls.Remove(row.checkStation);
                                foreach (ComboBox port in row.comboPort)
                                {
                                    if (port != null)
                                    {
                                        tlpStationList.Controls.Remove(port);
                                    }
                                }
                                tlpStationList.Controls.Remove(row.labelMsg);
                            }
                        }
                    }
                    // adjust tpl row count, if shrinking
                    tlpStationList.RowCount = itlpRow;
                }
            }
            else
            {
                tlpStationList.RowCount = 1;
            }
        }

        private void clearDesignRowsControls()
        {
            // clear all controls in table layout panel (do not clear titles)
            Control ctrl = default(Control);
            if (tlpStationList.RowCount > 1)
            {
                for (var iRow = 1; iRow <= tlpStationList.RowCount - 1; iRow++)
                {
                    for (var iCol = 0; iCol <= tlpStationList.ColumnCount - 1; iCol++)
                    {
                        ctrl = tlpStationList.GetControlFromPosition(System.Convert.ToInt32(iCol), System.Convert.ToInt32(iRow));
                        if (ctrl != null)
                        {
                            tlpStationList.Controls.Remove(ctrl);
                            //ctrl.Dispose()
                            //ctrl = Nothing
                        }
                    }
                }
                tlpStationList.RowCount = 1;
            }
        }

        private void RefreshStationList()
        {
            do
            {
                for (var i = 0; i <= confStationList.Count - 1; i++)
                {
                    updateStationCheckControl(System.Convert.ToInt64(confStationList[System.Convert.ToInt32(i)].ID));
                }
                Thread.Sleep(TIME_REFRESH_STATION_LIST);
            } while (true);
        }

        #endregion

        #region Set Station Mode
        public void row_checkControlMode_Click(System.Object sender, System.EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            bool bOk;
            tStationRow row = getRowFromModeControlName(cb.Name);
            long stationID = System.Convert.ToInt64(getStationsIDByListRow(row.iListRow)[0]);
            int stationIdx = getStationIndex(stationID);

            //comprobar si tiene el control
            if (jbc.GetControlModeUserName(stationID) != "" && jbc.GetControlModeUserName(stationID) != Environment.MachineName)
            {
                cb.Enabled = false;
                cb.Checked = !cb.Checked;
                cb.Enabled = true;

                MessageBox.Show("La estación está siendo controlada por: " + jbc.GetControlModeUserName(stationID));

                return;
            }

            // cb.Checked changed when clicked
            if (!cb.Checked)
            {
                // change to monitor mode
                // central procedure that updates mode in station form, dock panel and this form (thru updateStationCheckControl(stationID) function
                ListViewItem temp_stationItem = confStationList[stationIdx].dockitem;
                bOk = Configuration.setStationControlMode(jbc, System.Convert.ToInt64(confStationList[stationIdx].ID), ref temp_stationItem, false);
            }
            else
            {
                // change to control mode
                // central procedure that updates mode in station form, dock panel and this form (thru updateStationCheckControl(stationID) function
                ListViewItem temp_stationItem2 = confStationList[stationIdx].dockitem;
                bOk = Configuration.setStationControlMode(jbc, System.Convert.ToInt64(confStationList[stationIdx].ID), ref temp_stationItem2, true);
            }
        }

        private tStationRow getRowFromModeControlName(string cbname)
        {
            foreach (tStationRow row in rowList)
            {
                if (row.checkControlMode.Name == cbname)
                {
                    return row;
                }
            }
            return new tStationRow();
        }

        public void updateStationCheckControl(long stationID)
        {

            int stationIdx = 0;
            int stationMainIdx = 0;
            int rowIdx = 0;
            tConfStation confStn = new tConfStation();
            bool bControlMode = false;
            stationIdx = getStationIndex(stationID);
            if (stationIdx > -1)
            {
                confStn = confStationList[stationIdx];
                stationMainIdx = getMainStationIndex(stationID);
                if (stationMainIdx > -1)
                {
                    confStn.dockitem.Tag = mainStationList[stationMainIdx].item.Tag;
                    bControlMode = ((Configuration.tStationDataItemList)confStn.dockitem.Tag).bControlMode;
                    confStationList[stationIdx] = confStn;
                    rowIdx = getRowIndex(System.Convert.ToInt32(confStationList[stationIdx].iListRowStn));
                    if (rowIdx >= 0)
                    {
                        rowList[rowIdx].checkControlMode.Checked = bControlMode;
                        if (bControlMode)
                        {
                            rowList[rowIdx].checkControlMode.Image = My.Resources.Resources.unlock;
                        }
                        else
                        {
                            rowList[rowIdx].checkControlMode.Image = My.Resources.Resources._lock;
                            ;
                        }
                    }
                }
            }
        }

        #endregion

        #region Apply Configuration

        public async void butConfApply_Click(System.Object sender, System.EventArgs e)
        {
            Control ctrl = null;
            int[] aTargetFromSourcePorts = null;
            int i = 0;
            int iSourcePort = 0;
            int iTargetPort;
            string sSourcePort = "";
            List<long> stnList = default(List<long>);
            int stnIndex = 0;
            bool bStationChecked = false;
            bool bApply = false;
            string sMsg = "";

            bApply = true;
            sMsg = "";
            applyStationList.Clear();
            iApplied = 0;

            // check if there is a configuration tree loaded
            if (!Configuration.myControlExists(this, Configuration.sTreeName, ref ctrl))
            {
                if (!string.IsNullOrEmpty(sMsg))
                {
                    sMsg += System.Convert.ToString("\r\n" + "\r\n");
                }
                sMsg += Localization.getResStr(Configuration.confWarnNoConfTreeId);
                bApply = false;
            }

            // check if there is some selected station
            bStationChecked = false;
            foreach (tStationRow row in rowList)
            {
                if (row.bShow && row.checkStation.Checked)
                {
                    bStationChecked = true;
                }
            }
            if (!bStationChecked)
            {
                if (!string.IsNullOrEmpty(sMsg))
                {
                    sMsg += System.Convert.ToString("\r\n" + "\r\n");
                }
                sMsg += Localization.getResStr(Configuration.confWarnNoStationSelectedId);
                bApply = false;
            }

            // errors
            if (!string.IsNullOrEmpty(sMsg))
            {
                Interaction.MsgBox(sMsg, MsgBoxStyle.Exclamation, Localization.getResStr(Configuration.gralWarningId));
                return;
            }

            sMsg = "";

            // check monitor mode in selected stations
            bStationChecked = false;
            foreach (tStationRow row in rowList)
            {
                if (row.bShow && row.checkStation.Checked && (!row.checkControlMode.Checked))
                {
                    bStationChecked = true;
                }
            }
            if (bStationChecked)
            {
                if (!string.IsNullOrEmpty(sMsg))
                {
                    sMsg += System.Convert.ToString("\r\n" + "\r\n");
                }
                sMsg += Localization.getResStr(Configuration.confWarnStationsInMonitorModeId);
                if (Interaction.MsgBox(sMsg, MsgBoxStyle.YesNo, Localization.getResStr(Configuration.gralWarningId)) == MsgBoxResult.No)
                {
                    return;
                }
            }
            // clear messages
            foreach (tStationRow row in rowList)
            {
                if (row.bShow)
                {
                    row.labelMsg.Text = "";
                }
            }
            if (bApply)
            {
                xmlDocToApply = Configuration.confParamTreeToXml((ParamTree)ctrl, true, "");
                if (xmlDocToApply != null)
                {

                    // new log
                    if (xmlLog != null)
                    {
                        xmlLog = null;
                    }

                    // update station list
                    int iAdded = 0;
                    int iInactives = 0;
                    updateStationList(ref iAdded, ref iInactives);

                    // process each selected station
                    foreach (tStationRow row in rowList)
                    {

                        if (row.bShow && row.checkStation.Checked && row.checkControlMode.Checked)
                        {

                            // create ports correlation array
                            aTargetFromSourcePorts = new int[0];
                            i = 0;
                            while (row.comboPort[i] != null)
                            {
                                // row combos are target ports
                                iTargetPort = i + 1;
                                Array.Resize(ref aTargetFromSourcePorts, i + 1);
                                aTargetFromSourcePorts[i] = 0;
                                // each combo contains which source port to use
                                sSourcePort = System.Convert.ToString(row.comboPort[i].Items[row.comboPort[i].SelectedIndex]);
                                try
                                {
                                    if (sSourcePort != Localization.getResStr(Configuration.confNoId))
                                    {
                                        iSourcePort = int.Parse(sSourcePort);
                                        aTargetFromSourcePorts[i] = iSourcePort;
                                    }
                                }
                                catch (Exception)
                                {
                                }
                                i++;
                            }

                            // get stations associated to this row
                            // future implementation may be 1 grouped station type for many stations
                            stnList = getStationsIDByListRow(row.iListRow);

                            foreach (long stnID in stnList)
                            {
                                stnIndex = getStationIndex(stnID);
                                if (stnIndex > -1)
                                {
                                    // check if station is listed and active
                                    if (confStationList[stnIndex].bListed && getMainStationIndex(stnID) > -1)
                                    {
                                        // add apply station
                                        // timer
                                        System.Windows.Forms.Timer applyStnTimer = new System.Windows.Forms.Timer();
                                        applyStnTimer.Tag = stnID.ToString() + ":" + lApplyTimeOutCount.ToString(); // station ID y timer count in tag property
                                        applyStnTimer.Interval = (int)lApplyTimeOut;
                                        applyStnTimer.Tick += timerStnApply_Tick;

                                        // station
                                        tApplyStation applyStn = new tApplyStation();
                                        applyStn.ID = stnID;
                                        applyStn.iListRow = row.iListRow;
                                        applyStn.stnTimer = applyStnTimer;
                                        applyStn.aTargetFromSourcePorts = aTargetFromSourcePorts;
                                        applyStn.bConfirmedApply = false;
                                        applyStn.stnName = System.Convert.ToString(confStationList[stnIndex].dockitem.Text);
                                        applyStationList.Add(applyStn);
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format(Localization.getResStr(Configuration.confWarnStationNotConnectedId),
                                            confStationList[stnIndex].dockitem.Text + " - " +
                                            confStationList[stnIndex].dockitem.SubItems[Configuration.subItemModelId].Text));
                                    }
                                }
                            }

                        } // If row.bShow And row.checkStation.Checked

                    }

                    if (applyStationList.Count > 0)
                    {
                        idxCurrentStnApply = -1;
                        while (await applyNext() == true)
                        {
                        }
                    }

                } // If xmlDoc IsNot Nothing

            } // if bApply

        }

        int idxCurrentStnApply = -1;
        private async Task<bool> applyNext()
        {
            if (idxCurrentStnApply < applyStationList.Count - 1)
            {

                idxCurrentStnApply++;
                tApplyStation applyStn = applyStationList[idxCurrentStnApply];

                // applying message
                var idx = getRowIndex(applyStn.iListRow);
                rowList[idx].labelMsg.Text = Localization.getResStr(Configuration.confMsgApplyingId);

                // update station
                Configuration.confSetToStation(xmlDocToApply, applyStn.ID, jbc, applyStn.aTargetFromSourcePorts);
                iApplied++;

                // transaction for this station
                // se envía Transaction para saber cuando acaba de actualizar
                // en jbc.TransactionFinished se para el timer (o por timeout)
                applyStn.uiTransaction = System.Convert.ToUInt32(await jbc.SetTransactionAsync(applyStn.ID));

                applyStationList[idxCurrentStnApply] = applyStn;

                // wait for timeout
                applyStn.stnTimer.Start();

                return true;
            }
            else
            {
                return false;
            }
        }

        //' finished applying station
        //Private Sub jbc_TransactionFinished(ByVal stationID As ULong, ByVal transactionID As UInteger) Handles jbc.TransactionFinished
        //    ' NO IMPLEMENTADO HASTA QUE NO TENGAMOS DUAL COMMUNICATION EN WCF (CALLBACKS)

        //    'required as long as this method is called from a diferent thread
        //    If Me.InvokeRequired Then
        //        Me.Invoke(New TransactionFinishedEventHandler(AddressOf invoke_TransactionFinished), New Object() {stationID, transactionID})
        //        Exit Sub
        //    End If
        //End Sub

        private object LockTrans = new object();
        //Private Sub invoke_TransactionFinished(ByVal stationID As ULong, ByVal transactionID As UInteger)
        //    SyncLock LockTrans
        //        ' NO IMPLEMENTADO HASTA QUE NO TENGAMOS DUAL COMMUNICATION EN WCF (CALLBACKS)

        //        ''required as long as this method is called from a diferent thread
        //        'If Me.InvokeRequired Then
        //        '    Me.Invoke(New TransactionFinishedEventHandler(AddressOf jbc_TransactionFinished), New Object() {stationID, transactionID})
        //        '    Exit Sub
        //        'End If

        //        Dim stationApplyIdx As Integer = getApplyStationIndex(stationID)
        //        Dim applyStn As tApplyStation = applyStationList(stationApplyIdx)
        //        ' dispose timer
        //        If applyStn.stnTimer IsNot Nothing Then
        //            applyStn.stnTimer.Stop()
        //            applyStn.stnTimer.Dispose()
        //            applyStn.stnTimer = Nothing
        //        End If
        //        ' applied message
        //        Dim idx = getRowIndex(applyStn.iListRow)
        //        rowList.Item(idx).labelMsg.Text = getResStr(confMsgAppliedId)
        //        ' mark as applied
        //        applyStn.bConfirmedApply = True

        //        applyStationList(stationApplyIdx) = applyStn

        //        If checkEndApply() Then
        //            RaiseEvent EndApply()
        //        End If

        //    End SyncLock
        //End Sub

        // timeout applying station
        private async void timerStnApply_Tick(System.Object sender, System.EventArgs e)
        {

            //SyncLock LockTrans
            ((System.Windows.Forms.Timer)sender).Stop();
            //MsgBox("Timeout when applying configurations to station '" & applyStationList.Item(idxCurrentStnApply).stnName & "'")

            // id de estación viene en el Tag del Timer
            //Dim stationID = CUInt(CType(sender, Timer).Tag)
            string sStationAndCount = System.Convert.ToString(((System.Windows.Forms.Timer)sender).Tag);
            string[] arr = sStationAndCount.Split(':');
            uint stationID = System.Convert.ToUInt32(uint.Parse(arr[0]));
            int iTimerCounter = int.Parse(arr[1]);
            iTimerCounter--;

            int stationApplyIdx = getApplyStationIndex(stationID);
            tApplyStation applyStn = applyStationList[stationApplyIdx];

            if (iTimerCounter <= 0)
            {

                // station apply timeout

                // dispose timer
                if (applyStn.stnTimer != null)
                {
                    applyStn.stnTimer.Stop();
                    applyStn.stnTimer.Dispose();
                    applyStn.stnTimer = null;
                }
                // timeout message
                var idx = getRowIndex(applyStn.iListRow);
                rowList[idx].labelMsg.Text = Localization.getResStr(Configuration.confMsgAppliedTimeoutId);

                applyStationList[stationApplyIdx] = applyStn;

                if (checkEndApply())
                {
                    if (EndApplyEvent != null)
                        EndApplyEvent();
                }

            }
            else
            {
                // query finish Transaction
                if (await jbc.QueryEndedTransactionAsync(stationID, applyStn.uiTransaction))
                {

                    // transaction finished

                    // dispose timer
                    if (applyStn.stnTimer != null)
                    {
                        applyStn.stnTimer.Stop();
                        applyStn.stnTimer.Dispose();
                        applyStn.stnTimer = null;
                    }
                    // applied message
                    var idx = getRowIndex(applyStn.iListRow);
                    rowList[idx].labelMsg.Text = Localization.getResStr(Configuration.confMsgAppliedId);

                    // mark as applied
                    applyStn.bConfirmedApply = true;

                    applyStationList[stationApplyIdx] = applyStn;

                    if (checkEndApply())
                    {
                        if (EndApplyEvent != null)
                            EndApplyEvent();
                    }

                }
                else
                {
                    // continue timer for timeout
                    ((System.Windows.Forms.Timer)sender).Tag = stationID.ToString() + ":" + iTimerCounter.ToString();
                    ((System.Windows.Forms.Timer)sender).Start();
                }

            }

            //End SyncLock
        }

        private bool checkEndApply()
        {
            bool bEndApplying = true;
            foreach (var applyStn in applyStationList)
            {
                if (applyStn.stnTimer != null)
                {
                    bEndApplying = false;
                }
            }
            return bEndApplying;
        }

        private delegate void EndApplyEventHandler();
        private EndApplyEventHandler EndApplyEvent;

        private event EndApplyEventHandler EndApply
        {
            add
            {
                EndApplyEvent = (EndApplyEventHandler)System.Delegate.Combine(EndApplyEvent, value);
            }
            remove
            {
                EndApplyEvent = (EndApplyEventHandler)System.Delegate.Remove(EndApplyEvent, value);
            }
        }

        public void ev_EndApply()
        {
            int stationIdx = 0;
            System.Xml.XmlDocument xmlDocTarget = default(System.Xml.XmlDocument);

            // update data in forms, if any open
            foreach (var applyStn in applyStationList)
            {
                stationIdx = getStationIndex(System.Convert.ToInt64(applyStn.ID));
                // force to refresh current station settings page
                int idx = getMainStationIndex(System.Convert.ToInt64(confStationList[stationIdx].ID));
                frmMain.tStation stnListElem = mainStationList[idx];
                if (stnListElem.frm != null)
                {
                    stnListElem.frm.RefreshSettingsPages(true); // only current
                }
                if (stnListElem.frmHA != null)
                {
                    stnListElem.frmHA.RefreshSettingsPages(true); // only current
                }
            }

            // check if all stations applied
            bool bAllApplied = true;
            foreach (var applyStn in applyStationList)
            {
                if (!applyStn.bConfirmedApply)
                {
                    bAllApplied = false;
                }
            }

            if (!bAllApplied)
            {
                MessageBox.Show(Localization.getResStr(Configuration.confWarnStationsNotUpdatedId));
            }

            // CUIDADO, antes de comparar actualizar estaciones porque se puede haber ido la conexión
            int iAdded = 0;
            int iDeleted = 0;
            updateStationList(ref iAdded, ref iDeleted);

            // comparar datos aplicados y actuales
            foreach (var applyStn in applyStationList)
            {
                // check if connected
                int stnIndex = getStationIndex(System.Convert.ToInt64(applyStn.ID));
                if (stnIndex >= 0 && confStationList[stnIndex].bActive)
                {
                    xmlDocTarget = Configuration.confGetFromStation(System.Convert.ToInt64(applyStn.ID), jbc, false);
                    Configuration.logLogXMLs(ref xmlLog, xmlDocToApply, xmlDocTarget, applyStn.aTargetFromSourcePorts);
                }

            }
            sLastLogXmlPathFileName = Configuration.confSaveLogXml(xmlLog, Localization.getResStr(Configuration.xslLogTitleId));

            // ver informe final
            butConfViewReport.Enabled = true;
            if (Interaction.MsgBox(Localization.getResStr(Configuration.logWantToSeeReportId), MsgBoxStyle.YesNo, Localization.getResStr(Configuration.gralDoneId)) == MsgBoxResult.Yes)
            {
                seeLogReport();
            }

        }

        private void seeLogReport()
        {
            Form oForm;
            if (!string.IsNullOrEmpty(sLastLogXmlPathFileName))
            {
                oForm = Configuration.showXML(sLastLogXmlPathFileName, true);
            }
        }

        public void butConfViewReport_Click(System.Object sender, System.EventArgs e)
        {
            Form oForm;
            if (!string.IsNullOrEmpty(sLastLogXmlPathFileName))
            {
                oForm = Configuration.showXML(sLastLogXmlPathFileName, true);
            }
        }

        #endregion

    }
}
