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
// End of VB project level imports

namespace RemoteManRegister
{
    public partial class frmAxisGrid
    {

        //defining the axis temperatures for custom ranges
        //Private ReadOnly axisTempCustomValues() As Integer = {25, 50, 75, 100, 125, 150, 175, 200, 225, 250,
        //                                             275, 300, 325, 350, 375, 400, 425, 450, 475, 500}
        private frmMainRegister frmMainReg;
        private string myTempUnits;
        private string myTempUnitsText;

        public frmAxisGrid(frmMainRegister pMainReg)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();
            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;
            frmMainReg = pMainReg;
        }

        public void frmAxisGrid_Load(System.Object sender, System.EventArgs e)
        {
            int idxPwr = 0;
            int idxTemp = 0;
            string str = "";

            ReloadTexts();

            //clearing and loading the other lists
            lbxPower.Items.Clear();
            cbxPwrMin.Items.Clear();
            cbxPwrMax.Items.Clear();
            cbxPwrStep.Items.Clear();
            cbxTimeRange.Items.Clear();
            cbxTimeStep.Items.Clear();

            foreach (string s in My.Settings.Default.AxisPwrRange)
            {
                lbxPower.Items.Add(s);
            }

            foreach (string s in My.Settings.Default.AxisPwrCustom)
            {
                string sTemp = s + " " + System.Convert.ToString(frmMainReg.plot.myPwrUnits);
                cbxPwrMin.Items.Add(sTemp);
                cbxPwrMax.Items.Add(sTemp);
            }
            cbxPwrMin.Items.RemoveAt(cbxPwrMin.Items.Count - 1);
            cbxPwrMax.Items.RemoveAt(0);

            //using the default values to fill the time axis ranges
            foreach (string s in My.Settings.Default.AxisTimeRange)
            {
                cbxTimeRange.Items.Add(s);
                if (Convert.ToDouble(s.Split(" ".ToCharArray())[0]) == frmMainReg.config.axisAndGrid.timeRange)
                {
                    cbxTimeRange.SelectedItem = s;
                }
            }
            if (cbxTimeRange.SelectedIndex < 0)
            {
                cbxTimeRange.SelectedIndex = 0;
            }

            //using the default values to fill the grid steps
            foreach (string s in My.Settings.Default.GridPwrSteps)
            {
                cbxPwrStep.Items.Add(Convert.ToDouble(s));
            }
            foreach (string s in My.Settings.Default.GridTimeStep)
            {
                cbxTimeStep.Items.Add(s);
                if (Convert.ToDouble(s.Split(" ".ToCharArray())[0]) == frmMainReg.config.axisAndGrid.timeStep)
                {
                    cbxTimeStep.SelectedItem = s;
                }
            }

            // set plot temp units
            // load temperature list depending on temp units
            if ((string)frmMainReg.plot.myTempUnits == ManRegGlobal.tempunitCELSIUS)
            {
                myTempUnits = ManRegGlobal.tempunitCELSIUS;
                loadTemperatureLists();
            }
            if ((string)frmMainReg.plot.myTempUnits == ManRegGlobal.tempunitFAHRENHEIT)
            {
                myTempUnits = ManRegGlobal.tempunitFAHRENHEIT;
                loadTemperatureLists();
            }

            //setting the selected indexes initially
            // lists
            str = getStringFromRange(frmMainReg.config.axisAndGrid.Tmin, frmMainReg.config.axisAndGrid.Tmax, myTempUnitsText);
            idxTemp = lbxTemperature.Items.IndexOf(str);
            if (idxTemp < 0)
            {
                idxTemp = 0;
            }
            str = getStringFromRange(frmMainReg.config.axisAndGrid.Pmin, frmMainReg.config.axisAndGrid.Pmax, System.Convert.ToString(frmMainReg.plot.myPwrUnits));
            idxPwr = lbxPower.Items.IndexOf(str);
            if (idxPwr < 0)
            {
                idxPwr = 0;
            }
            lbxTemperature.SelectedIndex = idxTemp;
            lbxPower.SelectedIndex = idxPwr;

            // steps
            idxTemp = cbxTempStep.Items.IndexOf(frmMainReg.config.axisAndGrid.Tstep);
            if (idxTemp < 0)
            {
                idxTemp = 0;
            }
            idxPwr = cbxPwrStep.Items.IndexOf(frmMainReg.config.axisAndGrid.Pstep);
            if (idxPwr < 0)
            {
                idxPwr = 0;
            }
            cbxTempStep.SelectedIndex = idxTemp;
            cbxPwrStep.SelectedIndex = idxPwr;

            //setting default selection for the custom axis combo boxes
            cbxPwrMax.SelectedIndex = 0;
            cbxPwrMin.SelectedIndex = 0;

            // default ruler axis as temp
            rdbTemperature.Checked = true;
            if ((string)frmMainReg.plot.myTempUnits == ManRegGlobal.tempunitCELSIUS)
            {
                rbAxisTempCelsius.Checked = true;
            }
            if ((string)frmMainReg.plot.myTempUnits == ManRegGlobal.tempunitFAHRENHEIT)
            {
                rbAxisTempFahrenheit.Checked = true;
            }

            //setting the selected ruler axis (force change)
            if (frmMainReg.config.axisAndGrid.rulerAxis == frmMainRegister.TEMPERATURE)
            {
                rdbTemperature.Checked = true;
            }
            if (frmMainReg.config.axisAndGrid.rulerAxis == frmMainRegister.POWER)
            {
                rdbPower.Checked = true;
            }
            // force change
            recalculateAxis();

        }

        private void loadTemperatureLists()
        {

            lbxTemperature.Items.Clear();
            cbxTempMin.Items.Clear();
            cbxTempMax.Items.Clear();
            cbxTempStep.Items.Clear();

            if (myTempUnits == ManRegGlobal.tempunitCELSIUS)
            {

                myTempUnitsText = ManRegGlobal.CELSIUS_TEXT;

                //using the default values to fill the axis range list
                foreach (string s in My.Settings.Default.AxisTempRange)
                {
                    lbxTemperature.Items.Add(s);
                }
                //using the default values to fill the custom range combo boxes
                foreach (string s in My.Settings.Default.AxisTempCustom)
                {
                    string sTemp = s + " " + myTempUnits;
                    cbxTempMin.Items.Add(sTemp);
                    cbxTempMax.Items.Add(sTemp);
                }
                //using the default values to fill the grid steps
                foreach (string s in My.Settings.Default.GridTempSteps)
                {
                    cbxTempStep.Items.Add(Convert.ToDouble(s));
                }
            }

            if (myTempUnits == ManRegGlobal.tempunitFAHRENHEIT)
            {

                myTempUnitsText = ManRegGlobal.FAHRENHEIT_TEXT;

                //using the default values to fill the axis range list
                foreach (string s in My.Settings.Default.AxisTempRangeF)
                {
                    lbxTemperature.Items.Add(s);
                }
                //using the default values to fill the custom range combo boxes
                foreach (string s in My.Settings.Default.AxisTempCustomF)
                {
                    string sTemp = s + " " + myTempUnits;
                    cbxTempMin.Items.Add(sTemp);
                    cbxTempMax.Items.Add(sTemp);
                }
                //using the default values to fill the grid steps
                foreach (string s in My.Settings.Default.GridTempStepsF)
                {
                    cbxTempStep.Items.Add(Convert.ToDouble(s));
                }
            }

            cbxTempMin.Items.RemoveAt(cbxTempMin.Items.Count - 1);
            cbxTempMax.Items.RemoveAt(0);

            //setting the selected index
            lbxTemperature.SelectedIndex = 0;
            //setting default selection for the custom axis combo boxes
            cbxTempStep.SelectedIndex = 0;
            cbxTempMax.SelectedIndex = 0;
            cbxTempMin.SelectedIndex = 0;

        }

        public void rbAxisTemp_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (ReferenceEquals(sender, rbAxisTempCelsius) && rbAxisTempCelsius.Checked)
            {
                myTempUnits = ManRegGlobal.tempunitCELSIUS;
                loadTemperatureLists();
                recalculateAxis();
            }
            if (ReferenceEquals(sender, rbAxisTempFahrenheit) && rbAxisTempFahrenheit.Checked)
            {
                myTempUnits = ManRegGlobal.tempunitFAHRENHEIT;
                loadTemperatureLists();
                recalculateAxis();
            }
        }

        public void ReloadTexts()
        {
            Text = Localization.getResStr(ManRegGlobal.regMnuConfigAxisId);

            gbxAxis.Text = Localization.getResStr(ManRegGlobal.regAxisGridAxisId);
            lblAxisTemperature.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId);
            lblAxisPower.Text = Localization.getResStr(ManRegGlobal.gralPowerId);
            butAddTemp.Text = Localization.getResStr(ManRegGlobal.regAddId);
            butAddPwr.Text = Localization.getResStr(ManRegGlobal.regAddId);
            lblAxisTimeWindow.Text = Localization.getResStr(ManRegGlobal.regAxisGridTimeWindowId);

            gbxGrid.Text = Localization.getResStr(ManRegGlobal.regAxisGridGridId);
            rdbTemperature.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId);
            rdbPower.Text = Localization.getResStr(ManRegGlobal.gralPowerId);
            lblTime.Text = Localization.getResStr(ManRegGlobal.regTimeId);
            lblSeconds1.Text = Localization.getResStr(ManRegGlobal.regAxisGridSecondsId);
            lblSeconds2.Text = Localization.getResStr(ManRegGlobal.regAxisGridSecondsId);

            butOK.Text = Localization.getResStr(ManRegGlobal.regButOkId);
            butCancel.Text = Localization.getResStr(ManRegGlobal.regButCancelId);

        }

        public void butAddTemp_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                //generating the string
                string s = cbxTempMin.SelectedItem + " - " + System.Convert.ToString(cbxTempMax.SelectedItem);

                //checking that the range is correct
                double min = 0;
                double max = 0;
                getRangeFromString(s, ref min, ref max);
                if (min >= max)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_GUI_AXIS_BAD_RANGE_VALUES));
                }

                //adding the new range to the list
                lbxTemperature.Items.Add(s);

                //selecting the new range
                lbxTemperature.SelectedIndex = lbxTemperature.Items.IndexOf(s);

                //saving the new range in the settings
                if (myTempUnits == ManRegGlobal.tempunitCELSIUS)
                {
                    My.Settings.Default.AxisTempRange.Add(s);
                }
                if (myTempUnits == ManRegGlobal.tempunitFAHRENHEIT)
                {
                    My.Settings.Default.AxisTempRangeF.Add(s);
                }
                My.Settings.Default.Save();
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        public void butAddPwr_Click(System.Object sender, System.EventArgs e)
        {
            try
            {
                //generating the string
                string s = cbxPwrMin.SelectedItem + " - " + System.Convert.ToString(cbxPwrMax.SelectedItem);

                //checking that the range is correct
                double min = 0;
                double max = 0;
                getRangeFromString(s, ref min, ref max);
                if (min >= max)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_GUI_AXIS_BAD_RANGE_VALUES));
                }

                //adding the new range to the list
                lbxPower.Items.Add(s);

                //selecting the new range
                lbxPower.SelectedIndex = lbxPower.Items.IndexOf(s);

                //saving the new range in the settings
                My.Settings.Default.AxisPwrRange.Add(s);
                My.Settings.Default.Save();
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        public void cbxTempStep_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //selected a new temperature step, caluclating the power step
            if (lbxTemperature.SelectedIndex >= 0 & lbxPower.SelectedIndex >= 0)
            {
                recalculateAxis();
            }
        }

        public void cbxPwrStep_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //selected a new power step, caluclating the temperature step
            if (lbxTemperature.SelectedIndex >= 0 & lbxPower.SelectedIndex >= 0)
            {
                recalculateAxis();
            }
        }

        public void rdbAxisType_CheckedChanged(object sender, System.EventArgs e)
        {
            if (ReferenceEquals(sender, rdbTemperature) && rdbTemperature.Checked)
            {
                recalculateAxis();
            }
            if (ReferenceEquals(sender, rdbPower) && rdbPower.Checked)
            {
                recalculateAxis();
            }
        }

        private void recalculateAxis()
        {
            if (rdbTemperature.Checked)
            {
                rdbPower.Checked = false;
                cbxTempStep.Enabled = true;
                cbxPwrStep.Enabled = false;
                //cbxTempStep.SelectedIndex = 0
                if (lbxTemperature.SelectedIndex >= 0 & lbxPower.SelectedIndex >= 0)
                {
                    double min = 0;
                    double max = 0;
                    getRangeFromString(System.Convert.ToString(lbxTemperature.SelectedItem), ref min, ref max);
                    int nDiv = (int)(Math.Floor(System.Convert.ToDecimal((max - min) / System.Convert.ToDouble(cbxTempStep.SelectedItem))));
                    getRangeFromString(System.Convert.ToString(lbxPower.SelectedItem), ref min, ref max);
                    double pwrStp = (max - min) / Convert.ToDouble(nDiv);

                    //setting the label values with the result
                    lblTempAdjust.Text = cbxTempStep.SelectedItem.ToString() + " " + myTempUnits;
                    lblPwrAdjust.Text = Convert.ToString(Math.Round(pwrStp, 2)) + " " + System.Convert.ToString(frmMainReg.plot.myPwrUnits);
                }
            }
            else
            {
                rdbTemperature.Checked = false;
                cbxPwrStep.Enabled = true;
                cbxTempStep.Enabled = false;
                //cbxPwrStep.SelectedIndex = 0
                if (lbxTemperature.SelectedIndex >= 0 & lbxPower.SelectedIndex >= 0)
                {
                    double min = 0;
                    double max = 0;
                    getRangeFromString(System.Convert.ToString(lbxPower.SelectedItem), ref min, ref max);
                    int nDiv = (int)(Math.Floor(System.Convert.ToDecimal((max - min) / System.Convert.ToDouble(cbxPwrStep.SelectedItem))));
                    getRangeFromString(System.Convert.ToString(lbxTemperature.SelectedItem), ref min, ref max);
                    double tempStp = (max - min) / Convert.ToDouble(nDiv);

                    //setting the label values with the result
                    lblPwrAdjust.Text = cbxPwrStep.SelectedItem.ToString() + " " + System.Convert.ToString(frmMainReg.plot.myPwrUnits);
                    lblTempAdjust.Text = Convert.ToString(Math.Round(tempStp, 2)) + " " + myTempUnitsText;
                }
            }
        }

        public void lbxPower_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (rdbPower.Checked && cbxPwrStep.SelectedItem != null)
            {
                cbxPwrStep_SelectedIndexChanged(lbxPower, new System.EventArgs());
            }
            if (rdbTemperature.Checked && cbxTempStep.SelectedItem != null)
            {
                cbxTempStep_SelectedIndexChanged(lbxPower, new System.EventArgs());
            }
        }

        public void lbxTemperature_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (rdbPower.Checked && cbxPwrStep.SelectedItem != null)
            {
                cbxPwrStep_SelectedIndexChanged(lbxTemperature, new System.EventArgs());
            }
            if (rdbTemperature.Checked && cbxTempStep.SelectedItem != null)
            {
                cbxTempStep_SelectedIndexChanged(lbxTemperature, new System.EventArgs());
            }
        }

        public void butOK_Click(System.Object sender, System.EventArgs e)
        {
            //setting the current selected configuration
            double min = 0;
            double max = 0;

            frmMainReg.config.axisAndGrid.tempUnits = myTempUnits;

            getRangeFromString(System.Convert.ToString(lbxPower.SelectedItem), ref min, ref max);
            frmMainReg.config.axisAndGrid.Pmin = min;
            frmMainReg.config.axisAndGrid.Pmax = max;

            getRangeFromString(System.Convert.ToString(lbxTemperature.SelectedItem), ref min, ref max);
            frmMainReg.config.axisAndGrid.Tmin = min;
            frmMainReg.config.axisAndGrid.Tmax = max;

            frmMainReg.config.axisAndGrid.Pstep = Convert.ToDouble(lblPwrAdjust.Text.Split(" ".ToCharArray())[0]);
            frmMainReg.config.axisAndGrid.Tstep = Convert.ToDouble(lblTempAdjust.Text.Split(" ".ToCharArray())[0]);

            //frmMainReg.config.axisAndGrid.timeRange = Convert.ToDouble(cbxTimeRange.SelectedItem.Split(" ")[0]);
            //frmMainReg.config.axisAndGrid.timeStep = Convert.ToDouble(cbxTimeStep.SelectedItem.Split(" ")[0]);
            frmMainReg.config.axisAndGrid.timeMax = frmMainReg.config.axisAndGrid.timeMin + frmMainReg.config.axisAndGrid.timeRange;

            if (rdbTemperature.Checked)
            {
                frmMainReg.config.axisAndGrid.rulerAxis = frmMainRegister.TEMPERATURE;
            }
            if (rdbPower.Checked)
            {
                frmMainReg.config.axisAndGrid.rulerAxis = frmMainRegister.POWER;
            }

            //returning
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public void butCancel_Click(System.Object sender, System.EventArgs e)
        {
            //returning
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void getRangeFromString(string s, ref double min, ref double max)
        {
            //getting the values from the string
            string[] word = s.Split(" ".ToCharArray());
            min = Convert.ToDouble(word[0]);
            max = Convert.ToDouble(word[3]);
        }

        private string getStringFromRange(double min, double max, string units)
        {
            string s = min.ToString() + " " + units + " - " + max.ToString() + " " + units;
            return s;
        }


    }
}
