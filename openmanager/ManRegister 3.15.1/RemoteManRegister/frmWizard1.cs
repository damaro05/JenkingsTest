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
    public partial class frmWizard1
    {

        private frmMainRegister frmMainReg;
        private List<ManRegGlobal.cListStation> stationList = new List<ManRegGlobal.cListStation>();
        public string sSelectStationID = "";
        private bool bUpdatingStations = false;
        private JBC_API_Remote jbc = null;

        public frmWizard1(frmMainRegister pMainReg, JBC_API_Remote _jbc)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();
            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;
            frmMainReg = pMainReg;
            jbc = _jbc;
        }

        public void frmWizard1_Load(object sender, System.EventArgs e)
        {
            ReloadTexts();
            loadStep();
        }

        public void ReloadTexts()
        {
            Text = Localization.getResStr(ManRegGlobal.regMnuConfigWizardId);
            lblInfo.Text = Localization.getResStr(ManRegGlobal.regWiz1InfoId);

            gbxContent.Text = Localization.getResStr(ManRegGlobal.regWizardSeriesAndTitleId);
            lblName.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieNameId);
            lblPort.Text = Localization.getResStr(ManRegGlobal.regSeriesSeriePortId);
            lblMagnitude.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieMagnitudeId);
            lblPlotTitle.Text = Localization.getResStr(ManRegGlobal.regPlotTitleId);
            butAdd.Text = Localization.getResStr(ManRegGlobal.regAddId);
            butRemove.Text = Localization.getResStr(ManRegGlobal.regRemoveId);
            butNext.Text = Localization.getResStr(ManRegGlobal.regNextId);

            cbxAxis.Items.Clear();
            cbxAxis.Items.Add(string.Format(Localization.getResStr(ManRegGlobal.regSeriesMagnitudeTempId), frmMainReg.plot.myTempUnits));
            cbxAxis.Items.Add(Localization.getResStr(ManRegGlobal.regSeriesMagnitudePowerId));

        }

        //Functions
        private void loadStep()
        {
            try
            {
                //setting the label info

                //when loading getting the list of series
                lbxSeries.Items.Clear();
                List<string> list = new List<string>();
                frmMainReg.plot.getListOfSerieName(list);
                foreach (string s in list)
                {
                    lbxSeries.Items.Add(s);
                }

                //selecting default values for widgets
                if (lbxSeries.Items.Count > 0)
                {
                    lbxSeries.SelectedIndex = 0;
                }

                //updating the content
                updateConnectedStations();
                updateControls();
            }
            catch (CerrorRegister err)
            {
                err.showError();
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }

        private void updateControls()
        {
            //there are stations, enabling the widgets
            cbxAxis.Enabled = true;
            cbxPort.Enabled = true;
            cbxStation.Enabled = true;
            txtSetName.Enabled = true;
            butAdd.Enabled = lbxSeries.Items.Count < frmMainRegister.MAX_SERIE_NUM;

            //setting default values
            if (cbxStation.SelectedIndex < 0 & cbxStation.Items.Count > 0)
            {
                cbxStation.SelectedIndex = 0;
            }
            if (cbxPort.SelectedIndex < 0)
            {
                cbxPort.SelectedIndex = 0;
            }
            if (cbxAxis.SelectedIndex < 0)
            {
                cbxAxis.SelectedIndex = 0;
            }

            if (txtTitle.Text == "")
            {
                txtTitle.Text = System.Convert.ToString(frmMainReg.plot.myTitle);
            }
            if (cbxAxis.SelectedIndex == 0)
            {
                txtSetName.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId) + frmMainReg.tempCnt.ToString();
            }
            else if (cbxAxis.SelectedIndex == 1)
            {
                txtSetName.Text = Localization.getResStr(ManRegGlobal.gralPowerId) + frmMainReg.pwrCnt.ToString();
            }
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

        private void addSerie()
        {
            try
            {
                //adding a serie if posible, checking the srie name
                if (cbxStation.Items.Count <= 0)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_NO_STATION_CONNECTED));
                }
                if (txtSetName.Text.Length == 0)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_EMPTY_NAME));
                }

                foreach (string s in lbxSeries.Items)
                {
                    if (s == txtSetName.Text)
                    {
                        throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_REPEATED_NAME));
                    }
                }

                //parametters are correct, adding the serie
                string serieName = txtSetName.Text;
                int mag = 0;
                Color clr = new Color();
                //if (cbxAxis.SelectedItem.contains(string.Format(Localization.getResStr(ManRegGlobal.regSeriesMagnitudeTempId), frmMainReg.plot.myTempUnits)))
                //{
                //    mag = frmMainRegister.TEMPERATURE;
                //    frmMainReg.tempCnt++;
                //}
                //else if (cbxAxis.SelectedItem.contains(Localization.getResStr(ManRegGlobal.regSeriesMagnitudePowerId)))
                //{
                //    mag = frmMainRegister.POWER;
                //    frmMainReg.pwrCnt++;
                //}
                clr = Color.FromName(System.Convert.ToString(My.Settings.Default.serieColors[(lbxSeries.Items.Count) % My.Settings.Default.serieColors.Count]));
                //
                //frmMainReg.plot.addSerie(serieName, Convert.ToUInt64(trim(cbxStation.SelectedItem.ToString.Split("-")(0))), _
                //                      cbxPort.SelectedItem, mag, clr, False, True)
                frmMainReg.plot.addSerie(serieName, Convert.ToUInt64(cbxStation.SelectedValue), System.Convert.ToInt32(cbxPort.SelectedItem), mag, clr, false, true, "");

                //updating the serie list
                lbxSeries.Items.Add(serieName);
                lbxSeries.SelectedItem = serieName;

                //updating the coord's form in case is necessary
                if (frmMainReg.coordStatus)
                {
                    frmMainReg.setFormCoords();
                }

                //redrawing the plot
                PictureBox null_PictureBox = null;
                frmMainReg.plot.draw(true, ref null_PictureBox);
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        private void removeSerie()
        {
            try
            {
                //checking there's a serie selected
                if (lbxSeries.Items.Count <= 0)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_GUI_SERIE_EMPTY_LIST));
                }

                //removing the selected serie
                frmMainReg.plot.removeSerie(System.Convert.ToString(lbxSeries.SelectedItem));

                //removing from the list
                lbxSeries.Items.Remove(lbxSeries.SelectedItem);
                if (lbxSeries.Items.Count > 0)
                {
                    lbxSeries.SelectedIndex = 0;
                }

                //updating the coord's form in case is necessary
                if (frmMainReg.coordStatus)
                {
                    frmMainReg.setFormCoords();
                }

                //redrawing the plot
                PictureBox null_PictureBox = null;
                frmMainReg.plot.draw(true, ref null_PictureBox);
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        //Widget methods
        public void butAdd_Click(object sender, System.EventArgs e)
        {
            addSerie();
            updateControls();
        }

        public void butNext_Click(object sender, System.EventArgs e)
        {
            //setting the title
            frmMainReg.plot.myTitle = txtTitle.Text;
            PictureBox null_PictureBox = null;
            frmMainReg.plot.draw(true, ref null_PictureBox);

            //showing the step 2
            this.Hide();
            frmMainReg.frmWizard2Reg.Location = this.Location;
            if (frmMainReg.frmWizard2Reg.ShowDialog() != System.Windows.Forms.DialogResult.Retry)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                this.Show();
            }
        }

        public void butRemove_Click(System.Object sender, System.EventArgs e)
        {
            removeSerie();
            updateControls();
        }

        public void cbxAxis_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            if ((txtSetName.Text.Length == (int)(Localization.getResStr(ManRegGlobal.gralTemperatureId).Length + 1) && txtSetName.Text.Contains(Localization.getResStr(ManRegGlobal.gralTemperatureId))) || (txtSetName.Text.Length == (int)(Localization.getResStr(ManRegGlobal.gralPowerId).Length + 1) && txtSetName.Text.Contains(Localization.getResStr(ManRegGlobal.gralPowerId))))
            {
                if (cbxAxis.SelectedIndex == 0)
                {
                    txtSetName.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId) + frmMainReg.tempCnt.ToString();
                }
                else if (cbxAxis.SelectedIndex == 1)
                {
                    txtSetName.Text = Localization.getResStr(ManRegGlobal.gralPowerId) + frmMainReg.pwrCnt.ToString();
                }
            }
        }

        public void cbxStation_DropDown(dynamic sender, System.EventArgs e)
        {
            sender.SuspendLayout();
            //updating the list of connected stations
            updateConnectedStations();
            sender.ResumeLayout();
        }

        public void cbxStation_Click(object sender, System.EventArgs e)
        {
            updateControls();
        }

    }
}
