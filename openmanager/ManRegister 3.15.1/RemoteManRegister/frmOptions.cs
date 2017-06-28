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
    public partial class frmOptions
    {
        public struct tParams
        {
            public Color bckGnd;
            public Color tempClr;
            public Color pwrClr;
            public Color timeClr;
            public Color gridClr;
            public Color textClr;
            public Color titleClr;

            public int sideStart;

            public int lineWidth;
            public int pointWidth;

            public int trigger;
            public ulong triggerStationID;
            public int triggerPort;
        }

        private tParams _params = new tParams();

        private frmMainRegister frmMainReg;
        private List<ManRegGlobal.cListStation> stationList = new List<ManRegGlobal.cListStation>();
        private bool bLoadingStations = false;
        private JBC_API_Remote jbc = null;

        public frmOptions(frmMainRegister pMainReg, JBC_API_Remote _jbc)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();
            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;
            frmMainReg = pMainReg;
            jbc = _jbc;
        }

        public void frmOptions_Load(System.Object sender, System.EventArgs e)
        {
            ReloadTexts();
            //loading and setting de parametters
            loadParams();
        }

        public void ReloadTexts()
        {
            Text = Localization.getResStr(ManRegGlobal.regMnuConfigOptionsId);

            gbColors.Text = Localization.getResStr(ManRegGlobal.regOptColorsId);
            butTempClr.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);
            butPwrClr.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);
            butTimeClr.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);
            butGridClr.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);
            butTextClr.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);
            butBckGndClr.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);
            butTitleClr.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieColorSelectId);

            lblTempAxisColor.Text = Localization.getResStr(ManRegGlobal.regOptClrTempAxisId);
            lblPowerAxisColor.Text = Localization.getResStr(ManRegGlobal.regOptClrPowerAxisId);
            lblTimeAxisColor.Text = Localization.getResStr(ManRegGlobal.regOptClrTimeAxisId);
            lblGridDivColor.Text = Localization.getResStr(ManRegGlobal.regOptClrGridDivId);
            lblSeriesTextColor.Text = Localization.getResStr(ManRegGlobal.regOptClrSeriesTextId);
            lblBackgroundColor.Text = Localization.getResStr(ManRegGlobal.regOptClrBackgroundId);
            lblTitleTextColor.Text = Localization.getResStr(ManRegGlobal.regOptClrTitleId);
            butDefaultColor.Text = Localization.getResStr(ManRegGlobal.regOptClrDefaultsId);

            gbPlotStartSide.Text = Localization.getResStr(ManRegGlobal.regOptStartSideId);
            rdbLeftStart.Text = Localization.getResStr(ManRegGlobal.regOptStartSideLeftId);
            rdbRightStart.Text = Localization.getResStr(ManRegGlobal.regOptStartSideRightId);
            gbTriggerType.Text = Localization.getResStr(ManRegGlobal.regStripTriggerId);
            rdbTriggerAuto.Text = Localization.getResStr(ManRegGlobal.regStripTriggerAutoId);
            rdbTriggerSingle.Text = Localization.getResStr(ManRegGlobal.regStripTriggerSingleId);
            rdbTriggerManual.Text = Localization.getResStr(ManRegGlobal.regStripTriggerManualId);
            lblTriggerAuto.Text = Localization.getResStr(ManRegGlobal.regOptTriggerAutoInfoId);
            lblTriggerSingle.Text = Localization.getResStr(ManRegGlobal.regOptTriggerSingleInfoId);
            lblTriggerManual.Text = Localization.getResStr(ManRegGlobal.regOptTriggerManualInfoId);
            lblStation.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieStationId);
            lblPort.Text = Localization.getResStr(ManRegGlobal.regSeriesSeriePortId);

            gbPlotLinesAndPoints.Text = Localization.getResStr(ManRegGlobal.regOptLinesAndPointsId);
            lblLineWidth.Text = Localization.getResStr(ManRegGlobal.regOptLineWidthId);
            lblPointWidth.Text = Localization.getResStr(ManRegGlobal.regOptPointWidthId);

        }

        private void loadParams()
        {
            //setting initial values
            _params.bckGnd = frmMainReg.plot.myBckGnd;
            _params.gridClr = frmMainReg.plot.myGridClr;
            _params.pwrClr = frmMainReg.plot.myPwrAxisClr;
            _params.sideStart = (int)frmMainReg.plot.mySideStart;
            _params.tempClr = frmMainReg.plot.myTempAxisClr;
            _params.textClr = frmMainReg.plot.myTextClr;
            _params.timeClr = frmMainReg.plot.myTimeAxisClr;
            _params.trigger = (int)frmMainReg.plot.myTrigger;
            _params.triggerStationID = frmMainReg.triggerStationID;
            _params.triggerPort = frmMainReg.triggerPort;
            _params.lineWidth = frmMainReg.plot.myLineWidth;
            _params.pointWidth = frmMainReg.plot.myPointWidth;
            _params.titleClr = frmMainReg.plot.myTitleClr;

            //setting widgets
            //setting the current trigger type
            if (frmMainReg.plot.myTrigger == Cplot.cTrigger.TRG_AUTO)
            {
                rdbTriggerAuto.Checked = true;
            }
            if (frmMainReg.plot.myTrigger == Cplot.cTrigger.TRG_MANUAL)
            {
                rdbTriggerManual.Checked = true;
            }
            if (frmMainReg.plot.myTrigger == Cplot.cTrigger.TRG_SINGLE)
            {
                rdbTriggerSingle.Checked = true;
            }

            //loading the stations of the series
            List<ulong> lstSeriesStations = new List<ulong>();
            frmMainReg.plot.getListOfSerieStationID(lstSeriesStations);
            loadSeriesStations(lstSeriesStations);
            //selecting the current station and port
            if (_params.triggerStationID != UInt64.MaxValue)
            {
                //cbxStation.SelectedItem = params.triggerStationID.ToString & " - " & jbc.GetStationName(params.triggerStationID)
                cbxStation.SelectedValue = _params.triggerStationID.ToString();
            }
            else
            {
                if (lstSeriesStations.Count > 0)
                {
                    cbxStation.SelectedIndex = 0;
                    _params.triggerStationID = lstSeriesStations[0];
                }
            }
            cbxPort.SelectedItem = frmMainReg.triggerPort.ToString();

            //setting boxes colors
            pcbTempClr.BackColor = frmMainReg.plot.myTempAxisClr;
            pcbPwrClr.BackColor = frmMainReg.plot.myPwrAxisClr;
            pcbTimeClr.BackColor = frmMainReg.plot.myTimeAxisClr;
            pcbGridClr.BackColor = frmMainReg.plot.myGridClr;
            pcbTextClr.BackColor = frmMainReg.plot.myTextClr;
            pcbBckGndClr.BackColor = frmMainReg.plot.myBckGnd;
            pcbTitleClr.BackColor = frmMainReg.plot.myTitleClr;

            //setting plot side start radio button
            if (frmMainReg.plot.mySideStart == Cplot.cSideStart.START_LEFT)
            {
                rdbLeftStart.Checked = true;
            }
            if (frmMainReg.plot.mySideStart == Cplot.cSideStart.START_RIGHT)
            {
                rdbRightStart.Checked = true;
            }

            //setting the line and points width
            nudLineWidth.Value = frmMainReg.plot.myLineWidth;
            nudPointWidth.Value = frmMainReg.plot.myPointWidth;
        }

        private void loadSeriesStations(List<ulong> listSeriesStations)
        {
            ManRegGlobal.cListStation stn = default(ManRegGlobal.cListStation);
            bLoadingStations = true;
            stationList.Clear();
            foreach (ulong ID in listSeriesStations)
            {
                // search id in connected stations
                bool bConnected = false;
                foreach (ManRegGlobal.cConnectedStation connStn in ManRegGlobal.connectedStations)
                {
                    if (ulong.Parse(connStn.ID) == ID)
                    {
                        bConnected = true;
                        break;
                    }
                }
                if (bConnected)
                {
                    stn = new ManRegGlobal.cListStation(Convert.ToString(ID), jbc.GetStationName((long)ID), jbc.GetStationModel((long)ID));
                    stationList.Add(stn);
                }
            }
            cbxStation.DataSource = null; // para que actualice el combo box
            cbxStation.DataSource = stationList;
            cbxStation.DisplayMember = "Text";
            cbxStation.ValueMember = "ID";
            bLoadingStations = false;
        }

        private void applyParams()
        {
            //setting current values
            frmMainReg.plot.myBckGnd = _params.bckGnd;
            frmMainReg.plot.myGridClr = _params.gridClr;
            frmMainReg.plot.myPwrAxisClr = _params.pwrClr;
            frmMainReg.plot.mySideStart = (Cplot.cSideStart)_params.sideStart;
            frmMainReg.plot.myTempAxisClr = _params.tempClr;
            frmMainReg.plot.myTextClr = _params.textClr;
            frmMainReg.plot.myTimeAxisClr = _params.timeClr;
            frmMainReg.plot.myPointWidth = _params.pointWidth;
            frmMainReg.plot.myLineWidth = _params.lineWidth;
            frmMainReg.plot.myTitleClr = _params.titleClr;

            //updating trigger and its label
            frmMainReg.plot.myTrigger = (Cplot.cTrigger)_params.trigger;
            frmMainReg.triggerStationID = _params.triggerStationID;
            frmMainReg.triggerPort = _params.triggerPort;
        }

        public void butTempClr_Click(System.Object sender, System.EventArgs e)
        {
            if (cdlgColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _params.tempClr = cdlgColor.Color;
                pcbTempClr.BackColor = cdlgColor.Color;
            }
        }

        public void butPwrClr_Click(System.Object sender, System.EventArgs e)
        {
            if (cdlgColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _params.pwrClr = cdlgColor.Color;
                pcbPwrClr.BackColor = cdlgColor.Color;
            }
        }

        public void butTimeClr_Click(System.Object sender, System.EventArgs e)
        {
            if (cdlgColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _params.timeClr = cdlgColor.Color;
                pcbTimeClr.BackColor = cdlgColor.Color;
            }
        }

        public void butGridClr_Click(System.Object sender, System.EventArgs e)
        {
            if (cdlgColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _params.gridClr = cdlgColor.Color;
                pcbGridClr.BackColor = cdlgColor.Color;
            }
        }

        public void butTextClr_Click(System.Object sender, System.EventArgs e)
        {
            if (cdlgColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _params.textClr = cdlgColor.Color;
                pcbTextClr.BackColor = cdlgColor.Color;
            }
        }

        public void butBckGndClr_Click(System.Object sender, System.EventArgs e)
        {
            if (cdlgColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _params.bckGnd = cdlgColor.Color;
                pcbBckGndClr.BackColor = cdlgColor.Color;
            }
        }

        public void butTitleClr_Click(System.Object sender, System.EventArgs e)
        {
            if (cdlgColor.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _params.titleClr = cdlgColor.Color;
                pcbTitleClr.BackColor = cdlgColor.Color;
            }
        }

        public void butDefaultColor_Click(System.Object sender, System.EventArgs e)
        {
            _params.bckGnd = Color.WhiteSmoke;
            _params.pwrClr = Color.Black;
            _params.tempClr = Color.Black;
            _params.timeClr = Color.Black;
            _params.gridClr = Color.FromArgb(255, 200, 200, 200);
            _params.textClr = Color.Black;
            _params.titleClr = Color.Black;
            pcbBckGndClr.BackColor = _params.bckGnd;
            pcbPwrClr.BackColor = _params.pwrClr;
            pcbTempClr.BackColor = _params.tempClr;
            pcbTimeClr.BackColor = _params.timeClr;
            pcbGridClr.BackColor = _params.gridClr;
            pcbTextClr.BackColor = _params.textClr;
            pcbTitleClr.BackColor = _params.titleClr;
        }

        public void rdbLeftStart_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (rdbLeftStart.Checked)
            {
                _params.sideStart = (int)Cplot.cSideStart.START_LEFT;
            }
        }

        public void rdbRightStart_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (rdbRightStart.Checked)
            {
                _params.sideStart = (int)Cplot.cSideStart.START_RIGHT;
            }
        }

        public void cbxCOM_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //If cbxStation.SelectedItem <> Nothing Then params.triggerStationID = Convert.ToUInt64(trim(cbxStation.SelectedItem.ToString.Split("-")(0)))
        }

        public void cbxStation_SelectedValueChanged(System.Object sender, System.EventArgs e)
        {
            if (!bLoadingStations)
            {
                if (cbxStation.SelectedValue != null)
                {
                    _params.triggerStationID = Convert.ToUInt64(cbxStation.SelectedValue);
                }
            }
        }

        public void cbxPort_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            _params.triggerPort = Convert.ToInt32(cbxPort.SelectedItem);
        }

        public void rdbTriggerAuto_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (rdbTriggerAuto.Checked)
            {
                _params.trigger = (int)Cplot.cTrigger.TRG_AUTO;
            }
        }

        public void rdbTriggerSingle_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (rdbTriggerSingle.Checked)
            {
                _params.trigger = (int)Cplot.cTrigger.TRG_SINGLE;
            }
        }

        public void rdbTriggerManual_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (rdbTriggerManual.Checked)
            {
                _params.trigger = (int)Cplot.cTrigger.TRG_MANUAL;
            }
        }

        public void butApply_Click(System.Object sender, System.EventArgs e)
        {
            applyParams();
            frmMainReg.reDraw();

            //exiting the form
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public void butCancel_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        public void nudLineWidth_ValueChanged(System.Object sender, System.EventArgs e)
        {
            _params.lineWidth = (int)nudLineWidth.Value;
        }

        public void nudPointWidth_ValueChanged(System.Object sender, System.EventArgs e)
        {
            _params.pointWidth = (int)nudPointWidth.Value;
        }

        public void Label14_Click(System.Object sender, System.EventArgs e)
        {

        }


    }
}
