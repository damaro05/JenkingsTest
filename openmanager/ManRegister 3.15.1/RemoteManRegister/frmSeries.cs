// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using JBC_ConnectRemote;
// End of VB project level imports

namespace RemoteManRegister
{
    public partial class frmSeries
    {

        //defining constant flags for the procedure to do of the modified serie
        public enum cSerieProcedure
        {
            ADD,
            EDIT,
            REMOVE,
            NONE
        }

        //defining the serie data structure
        public struct tSerieProcedure
        {
            public cSerieProcedure type;
            public string name;
            public string newName;
            public ulong stationID;
            public int port;
            public int magnitude;
            public Color clr;
            public bool showPoints;
            public bool showLines;
            public string legend;
        }

        //defining the list of added, edited or removed series
        private List<tSerieProcedure> series = new List<tSerieProcedure>();

        private frmMainRegister frmMainReg;
        private List<ManRegGlobal.cListStation> stationList = new List<ManRegGlobal.cListStation>();
        private bool bUpdatingStations = false;
        private ToolTip tooltipSerie = new ToolTip();
        private JBC_API_Remote jbc = null;

        // only permits this station
        public string sSelectStationID = "";

        public frmSeries(frmMainRegister pMainReg, JBC_API_Remote _jbc)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();
            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;
            frmMainReg = pMainReg;
            jbc = _jbc;

        }

        public void frmSeries_Load(System.Object sender, System.EventArgs e)
        {
            ReloadTexts();

            //loading the current series
            loadCurrentCreatedSeries();

            //updating the connected stations
            updateConnectedStations();

            //updating the serie list
            updateSerieListBox();

            //if there are series selecting one by default, otherwise setting default values
            if (lbxSeries.Items.Count > 0)
            {
                lbxSeries.SelectedIndex = 0;
            }
            //Else
            //If cbxStation.Items.Count > 0 Then cbxStation.SelectedIndex = 0
            cbxPort.SelectedIndex = 0;
            cbxAxis.SelectedIndex = 0;
            pcbColor.BackColor = Color.FromName(System.Convert.ToString(My.Settings.Default.serieColors[(lbxSeries.Items.Count) % My.Settings.Default.serieColors.Count]));
            chbLine.Checked = true;
            chbPoints.Checked = false;
            txtName.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId) + frmMainReg.tempCnt.ToString();

        }

        public void ReloadTexts()
        {
            Text = Localization.getResStr(ManRegGlobal.regMnuConfigSeriesId);

            GroupBoxListOfSeries.Text = Localization.getResStr(ManRegGlobal.regSeriesListOfSeriesId);
            GroupBoxSerieData.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieDataId);
            lblName.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieNameId);

            lblStation.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieStationId);
            lblPort.Text = Localization.getResStr(ManRegGlobal.regSeriesSeriePortId);
            lblMagnitude.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieMagnitudeId);
            lblColor.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorId);

            butColor.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);
            chbPoints.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieShowPointsId);
            chbLine.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieShowLineId);
            butAdd.Text = Localization.getResStr(ManRegGlobal.regAddId);
            //butEdit.Text = getResStr(regModifyId)
            butRemove.Text = Localization.getResStr(ManRegGlobal.regRemoveId);

            butOK.Text = Localization.getResStr(ManRegGlobal.regButOkId);
            butCancel.Text = Localization.getResStr(ManRegGlobal.regButCancelId);

            cbxAxis.Items.Clear();
            cbxAxis.Items.Add(string.Format(Localization.getResStr(ManRegGlobal.regSeriesMagnitudeTempId), frmMainReg.plot.myTempUnits));
            cbxAxis.Items.Add(Localization.getResStr(ManRegGlobal.regSeriesMagnitudePowerId));
            cbxAxis.Items.Add(Localization.getResStr(ManRegGlobal.regSeriesMagnitudeFlowId));

        }

        private void updateConnectedStations()
        {
            bUpdatingStations = true;
            bool bSelectStationIDFound = false;
            bool bSelectValueIDFound = false;
            string selectedValue = System.Convert.ToString(cbxStation.SelectedValue);

            // Loading the connected stations
            //For Each stn As cListStation In stationList
            //    stn = Nothing
            //Next
            stationList.Clear();
            foreach (ManRegGlobal.cConnectedStation connStn in ManRegGlobal.connectedStations)
            {
                ManRegGlobal.cListStation stn = new ManRegGlobal.cListStation(connStn.ID, jbc.GetStationName(long.Parse(connStn.ID)), jbc.GetStationModel(long.Parse(connStn.ID)));
                stationList.Add(stn);
                if (connStn.ID == sSelectStationID)
                {
                    bSelectStationIDFound = true;
                }
                if (connStn.ID == selectedValue)
                {
                    bSelectValueIDFound = true;
                }
            }
            if (!bSelectStationIDFound)
            {
                sSelectStationID = "";
            }

            cbxStation.DataSource = null;
            cbxStation.DataSource = stationList;
            cbxStation.DisplayMember = "Text";
            cbxStation.ValueMember = "ID";

            // Restoring the selected item
            try
            {
                if (!string.IsNullOrEmpty(sSelectStationID))
                {
                    cbxStation.SelectedValue = sSelectStationID;
                }
                else if (bSelectValueIDFound)
                {
                    cbxStation.SelectedValue = selectedValue;
                }
                else
                {
                    cbxStation.SelectedIndex = -1;
                }
            }
            catch (Exception)
            {
                cbxStation.SelectedIndex = -1;
            }

            bUpdatingStations = false;
        }

        private void updateSerieListBox()
        {
            //filling the serie list with the current procedures
            lbxSeries.Items.Clear();
            foreach (tSerieProcedure proc in series)
            {
                if (proc.type != cSerieProcedure.REMOVE)
                {
                    lbxSeries.Items.Add(proc.newName);
                }
            }
        }

        private void loadCurrentCreatedSeries()
        {
            //getting the current series in the plot
            List<string> list = new List<string>();
            frmMainReg.plot.getListOfSerieName(list);

            //for each serie gettin its data
            tSerieProcedure proc = new tSerieProcedure();
            foreach (string s in list)
            {
                proc.name = s;
                proc.newName = s;
                proc.type = cSerieProcedure.NONE;
                frmMainReg.plot.getSerieConfig(s, ref proc.stationID, ref proc.port, ref proc.magnitude, ref proc.clr, ref proc.showPoints, ref proc.showLines, ref proc.legend);
                series.Add(proc);
            }
        }

        private void setSerieProcedure(cSerieProcedure proc)
        {
            //creating a new serie procedure structure
            tSerieProcedure procedure = new tSerieProcedure();

            //filling common fields
            procedure.type = proc;
            //procedure.stationID = Convert.ToUInt64(trim(cbxStation.SelectedItem.ToString.Split("-")(0)))
            procedure.stationID = Convert.ToUInt64(cbxStation.SelectedValue);
            procedure.port = System.Convert.ToInt32(cbxPort.SelectedItem);
            if (((string)cbxAxis.SelectedItem).Contains(string.Format(Localization.getResStr(ManRegGlobal.regSeriesMagnitudeTempId), frmMainReg.plot.myTempUnits)))
            {
                procedure.magnitude = frmMainRegister.TEMPERATURE;
            }
            if (((string)cbxAxis.SelectedItem).Contains(Localization.getResStr(ManRegGlobal.regSeriesMagnitudePowerId)))
            {
                procedure.magnitude = frmMainRegister.POWER;
            }
            if (((string)cbxAxis.SelectedItem).Contains(Localization.getResStr(ManRegGlobal.regSeriesMagnitudeFlowId)))
            {
                procedure.magnitude = frmMainRegister.FLOW;
            }
            procedure.clr = pcbColor.BackColor;
            procedure.showPoints = chbPoints.Checked;
            procedure.showLines = chbLine.Checked;

            //depending on the desired procedure
            if (proc == cSerieProcedure.ADD)
            {
                procedure.name = txtName.Text;
                procedure.newName = txtName.Text;
                procedure.legend = txtName.Text;
                series.Add(procedure);
            }
            else if (proc == cSerieProcedure.EDIT)
            {
                //looking for the serie in the list of series
                int i = getSerieIndex(System.Convert.ToString(lbxSeries.SelectedItem));
                if (i != -1)
                {
                    procedure.name = System.Convert.ToString(series[i].name);
                    procedure.newName = txtName.Text;
                    procedure.legend = txtName.Text;
                    if (series[i].type == cSerieProcedure.ADD)
                    {
                        procedure.type = cSerieProcedure.ADD;
                    }
                    series.RemoveAt(i);
                    series.Add(procedure);
                }
            }
            else if (proc == cSerieProcedure.REMOVE)
            {
                //looking for the serie in the list of series
                int i = getSerieIndex(System.Convert.ToString(lbxSeries.SelectedItem));
                if (i != -1)
                {
                    procedure.name = System.Convert.ToString(series[i].name);
                    procedure.newName = System.Convert.ToString(series[i].newName);
                    procedure.legend = System.Convert.ToString(series[i].legend);
                    if (series[i].type != cSerieProcedure.ADD)
                    {
                        series.RemoveAt(i);
                        series.Add(procedure);
                    }
                    else
                    {
                        series.RemoveAt(i);
                    }
                }
            }

        }

        private void applySerieProcs()
        {
            try
            {
                //for every serie proc in the list doing its job
                foreach (tSerieProcedure proc in series)
                {
                    //depending on the proc
                    if (proc.type == cSerieProcedure.ADD)
                    {
                        //adding a serie
                        frmMainReg.plot.addSerie(proc.newName, proc.stationID, proc.port, proc.magnitude, proc.clr, proc.showPoints, proc.showLines, proc.legend);
                    }
                    else if (proc.type == cSerieProcedure.EDIT)
                    {
                        //editing a serie
                        frmMainReg.plot.editSerie(proc.name, proc.stationID, proc.port, proc.magnitude, proc.clr, proc.showPoints, proc.showLines, proc.legend, proc.newName);
                    }
                    else if (proc.type == cSerieProcedure.REMOVE)
                    {
                        //removing a serie
                        frmMainReg.plot.removeSerie(proc.name);
                    }
                    else if (proc.type == cSerieProcedure.NONE)
                    {
                        //nothing to do
                    }
                }
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        private void loadSelectedSerie()
        {
            //if there are series and one is selected
            tooltipSerie.RemoveAll();
            tooltipSerie.BackColor = Color.FromKnownColor(KnownColor.Info);
            tooltipSerie.ForeColor = Color.FromKnownColor(KnownColor.InfoText);
            if (lbxSeries.Items.Count > 0 & lbxSeries.SelectedIndex >= 0)
            {
                //looking for the selected serie
                int i = getSerieIndex(System.Convert.ToString(lbxSeries.SelectedItem));

                if (butEdit.Visible)
                {
                    // Modify button is visible, change values to modify
                    //loading teh current list box selected serie data
                    txtName.Text = System.Convert.ToString(series[i].newName);
                    cbxStation.SelectedValue = series[i].stationID.ToString();
                    cbxPort.SelectedItem = series[i].port.ToString();
                    if (series[i].magnitude == frmMainRegister.TEMPERATURE)
                    {
                        cbxAxis.SelectedIndex = 0;
                    }
                    if (series[i].magnitude == frmMainRegister.POWER)
                    {
                        cbxAxis.SelectedIndex = 1;
                    }
                    if (series[i].magnitude == frmMainRegister.FLOW)
                    {
                        cbxAxis.SelectedIndex = 2;
                    }
                    pcbColor.BackColor = series[i].clr;
                    chbPoints.Checked = System.Convert.ToBoolean(series[i].showPoints);
                    chbLine.Checked = System.Convert.ToBoolean(series[i].showLines);
                }
                else
                {
                    // show selected serie data in tooltip when not butEdit button
                    int idxStation = getIndexOfStation(series[i].stationID);
                    string sStation = "";
                    string sMag = "";
                    string sShow = "";
                    if (idxStation >= 0)
                    {
                        sStation = System.Convert.ToString(stationList[idxStation].Text);
                    }
                    if (series[i].magnitude == frmMainRegister.TEMPERATURE)
                    {
                        sMag = string.Format(Localization.getResStr(ManRegGlobal.regSeriesMagnitudeTempId), frmMainReg.plot.myTempUnits);
                    }
                    if (series[i].magnitude == frmMainRegister.POWER)
                    {
                        sMag = Localization.getResStr(ManRegGlobal.regSeriesMagnitudePowerId);
                    }
                    if (series[i].magnitude == frmMainRegister.FLOW)
                    {
                        sMag = Localization.getResStr(ManRegGlobal.regSeriesMagnitudeFlowId);
                    }
                    if (series[i].showPoints)
                    {
                        sShow = sShow.Trim() + " " + Localization.getResStr(ManRegGlobal.regSeriesSerieShowPointsId);
                    }
                    if (series[i].showLines)
                    {
                        sShow = sShow.Trim() + " " + Localization.getResStr(ManRegGlobal.regSeriesSerieShowLineId);
                    }
                    tooltipSerie.BackColor = series[i].clr;
                    tooltipSerie.ForeColor = myInvertedColor(series[i].clr);
                    tooltipSerie.SetToolTip(lbxSeries, Localization.getResStr(ManRegGlobal.regSeriesSerieStationId) + ": " + sStation + " - " + Localization.getResStr(ManRegGlobal.regSeriesSeriePortId) + ": " + series[i].port.ToString() + " - " + Localization.getResStr(ManRegGlobal.regSeriesSerieMagnitudeId) + ": " + sMag + " - " + sShow);
                }

            }
        }

        private Color myInvertedColor(Color col)
        {
            Color colInv = new Color();
            //colInv = Color.FromArgb(col.ToArgb Xor &HFFFFFF00)
            colInv = Color.FromArgb(col.A,
                255 - col.R, 255 - col.G, 255 - col.B);
            //colInv = Color.FromArgb((CInt(col.R) + 128) Mod 256, _
            //                        (CInt(col.G) + 128) Mod 256, _
            //                        (CInt(col.B) + 128) Mod 256)
            return colInv;
        }


        private int getIndexOfStation(ulong stationID)
        {
            ManRegGlobal.cListStation stn;
            if (stationList.Count > 0)
            {
                for (var i = 0; i <= stationList.Count - 1; i++)
                {
                    stn = stationList[System.Convert.ToInt32(i)];
                    if (ulong.Parse(stn.ID) == stationID)
                    {
                        return System.Convert.ToInt32(i);
                    }
                }
            }
            return -1;
        }

        private bool checkUserData(cSerieProcedure proc)
        {
            try
            {
                //if editing or removing , checking there's a selected serie
                if (proc == cSerieProcedure.EDIT | proc == cSerieProcedure.REMOVE)
                {
                    if (lbxSeries.SelectedIndex == -1)
                    {
                        throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_NO_SERIE_SELECTED_TO_EDIT_OR_REMOVE));
                    }
                }


                //checking the serie name
                if (txtName.Text.Length == 0)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_EMPTY_NAME));
                }

                //if adding, the serie name must not be repeated
                if (proc == cSerieProcedure.ADD)
                {
                    foreach (string s in lbxSeries.Items)
                    {
                        if (s == txtName.Text)
                        {
                            throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_REPEATED_NAME));
                        }
                    }
                }

                //checking there's a station selected if not removing
                if (proc != cSerieProcedure.REMOVE)
                {
                    if (cbxStation.Items.Count == 0 | cbxStation.SelectedIndex == -1)
                    {
                        throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_NO_STATION_SELECTED));
                    }
                }

                //all fine, indicating
                return true;
            }
            catch (CerrorRegister err)
            {
                //bad parametters, showing the error and indicating
                err.showError();
                return false;
            }
        }

        private int getSerieIndex(string name)
        {
            //looking for the serie
            bool found = false;
            int cnt = 0;
            while (!found && cnt < series.Count)
            {
                if (name == series[cnt].newName)
                {
                    found = true;
                }
                if (name != series[cnt].newName)
                {
                    cnt++;
                }
            }

            //search result
            if (found)
            {
                return cnt;
            }
            else
            {
                return -1;
            }
        }

        public void lbxSeries_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            //loading the selected serie
            loadSelectedSerie();
        }

        public void cbxStation_DropDown(dynamic sender, System.EventArgs e)
        {
            sender.SuspendLayout();
            //updating the list of connected stations
            updateConnectedStations();
            sender.ResumeLayout();
        }

        public void butAdd_Click(System.Object sender, System.EventArgs e)
        {
            //checking user data and in case is correct adding
            if (checkUserData(cSerieProcedure.ADD))
            {
                setSerieProcedure(cSerieProcedure.ADD);
                updateSerieListBox();
                if (cbxAxis.SelectedIndex == 0)
                {
                    frmMainReg.tempCnt++;
                    txtName.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId) + frmMainReg.tempCnt.ToString();
                }
                else if (cbxAxis.SelectedIndex == 1)
                {
                    frmMainReg.pwrCnt++;
                    txtName.Text = Localization.getResStr(ManRegGlobal.gralPowerId) + frmMainReg.pwrCnt.ToString();
                }
                else if (cbxAxis.SelectedIndex == 2)
                {
                    frmMainReg.flowCnt++;
                    txtName.Text = Localization.getResStr(ManRegGlobal.gralFlowId) + frmMainReg.flowCnt.ToString();
                }
                txtName.Focus();
                txtName.SelectAll();

                //reseting the default color
                pcbColor.BackColor = Color.FromName(System.Convert.ToString(My.Settings.Default.serieColors[(lbxSeries.Items.Count) % My.Settings.Default.serieColors.Count]));
            }
        }

        public void butEdit_Click(System.Object sender, System.EventArgs e)
        {
            //checking user data and in case is correct editing it
            if (checkUserData(cSerieProcedure.EDIT))
            {
                setSerieProcedure(cSerieProcedure.EDIT);
                updateSerieListBox();
                lbxSeries.SelectedIndex = lbxSeries.Items.Count - 1;
            }
        }

        public void butRemove_Click(System.Object sender, System.EventArgs e)
        {
            //checking user data and in case is correct removing it
            if (checkUserData(cSerieProcedure.REMOVE))
            {
                int selected = lbxSeries.SelectedIndex;
                setSerieProcedure(cSerieProcedure.REMOVE);
                updateSerieListBox();
                if (selected < lbxSeries.Items.Count)
                {
                    lbxSeries.SelectedIndex = selected;
                }
                if (selected >= lbxSeries.Items.Count)
                {
                    lbxSeries.SelectedIndex = lbxSeries.Items.Count - 1;
                }
                if (lbxSeries.SelectedIndex == -1)
                {
                    loadSelectedSerie(); // clear data to show
                }
            }
        }

        public void butOK_Click(System.Object sender, System.EventArgs e)
        {
            sSelectStationID = "";
            //applying procedures
            applySerieProcs();
            frmMainReg.reDraw();
            series.Clear();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public void butCancel_Click(System.Object sender, System.EventArgs e)
        {
            sSelectStationID = "";
            series.Clear();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public void frmSeries_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            butCancel_Click(null, null);
        }

        public void butColor_Click(System.Object sender, System.EventArgs e)
        {
            //showing the color dialog
            if (clrColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pcbColor.BackColor = clrColor.Color;
            }
        }

        public void cbxAxis_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ((txtName.Text.Length == (int)(Localization.getResStr(ManRegGlobal.gralTemperatureId).Length + 1) && txtName.Text.Contains(Localization.getResStr(ManRegGlobal.gralTemperatureId))) ||
                    (txtName.Text.Length == (int)(Localization.getResStr(ManRegGlobal.gralPowerId).Length + 1) && txtName.Text.Contains(Localization.getResStr(ManRegGlobal.gralPowerId))) ||
                    (txtName.Text.Length == (int)(Localization.getResStr(ManRegGlobal.gralFlowId).Length + 1) && txtName.Text.Contains(Localization.getResStr(ManRegGlobal.gralFlowId))))
            {
                if (cbxAxis.SelectedIndex == 0)
                {
                    txtName.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId) + frmMainReg.tempCnt.ToString();
                }
                else if (cbxAxis.SelectedIndex == 1)
                {
                    txtName.Text = Localization.getResStr(ManRegGlobal.gralPowerId) + frmMainReg.pwrCnt.ToString();
                }
                else if (cbxAxis.SelectedIndex == 2)
                {
                    txtName.Text = Localization.getResStr(ManRegGlobal.gralFlowId) + frmMainReg.pwrCnt.ToString();
                }
            }
            txtName.Focus();
            txtName.SelectAll();
        }
    }
}
