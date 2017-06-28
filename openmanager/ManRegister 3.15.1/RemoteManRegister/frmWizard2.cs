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
    public partial class frmWizard2
    {

        private frmMainRegister frmMainReg;
        private string myTempUnits;
        private string myTempUnitsText;
        private JBC_API_Remote jbc = null;

        public frmWizard2(frmMainRegister pMainReg, JBC_API_Remote _jbc)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();
            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;
            frmMainReg = pMainReg;
            jbc = _jbc;
        }

        public void frmWizard2_Load(object sender, System.EventArgs e)
        {
            int idxPwr = 0;
            int idxTemp = 0;
            string str = "";

            ReloadTexts();

            // set plot temp units
            // load temperature list depending on temp units
            if ((string)frmMainReg.plot.myTempUnits == ManRegGlobal.tempunitCELSIUS)
            {
                rbAxisTempCelsius.Checked = true;
                rbAxisTemp_CheckedChanged(rbAxisTempCelsius, null);
            }
            if ((string)frmMainReg.plot.myTempUnits == ManRegGlobal.tempunitFAHRENHEIT)
            {
                rbAxisTempFahrenheit.Checked = true;
                rbAxisTemp_CheckedChanged(rbAxisTempFahrenheit, null);
            }

            //clearing the other lists
            lbxPower.Items.Clear();
            cbxPwrStep.Items.Clear();
            cbxTimeRange.Items.Clear();
            cbxTimeStep.Items.Clear();

            foreach (string s in My.Settings.Default.AxisPwrRange)
            {
                lbxPower.Items.Add(s);
            }

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

            //setting the selected ruler axis
            if (frmMainReg.config.axisAndGrid.rulerAxis == frmMainRegister.TEMPERATURE)
            {
                rdbTemperature.Checked = true;
            }
            if (frmMainReg.config.axisAndGrid.rulerAxis == frmMainRegister.POWER)
            {
                rdbPower.Checked = true;
            }
        }

        public void ReloadTexts()
        {
            Text = Localization.getResStr(ManRegGlobal.regMnuConfigWizardId);
            lblInfo.Text = Localization.getResStr(ManRegGlobal.regWiz2InfoId);

            gbxContent.Text = Localization.getResStr(ManRegGlobal.regWizard2AxisRangeId);
            lblAxisTimeWindow.Text = Localization.getResStr(ManRegGlobal.regAxisGridTimeWindowId);
            gbGridStep.Text = Localization.getResStr(ManRegGlobal.regAxisGridGridStepId);
            rdbTemperature.Text = Localization.getResStr(ManRegGlobal.gralTemperatureId);
            rdbPower.Text = Localization.getResStr(ManRegGlobal.gralPowerId);
            lblTime.Text = Localization.getResStr(ManRegGlobal.regTimeId);
            lblSeconds1.Text = Localization.getResStr(ManRegGlobal.regAxisGridSecondsId);
            lblSeconds2.Text = Localization.getResStr(ManRegGlobal.regAxisGridSecondsId);

            butPrev.Text = Localization.getResStr(ManRegGlobal.regPreviousId);
            butFinish.Text = Localization.getResStr(ManRegGlobal.regFinishId);

        }

        //Functions
        private void loadTemperatureLists()
        {

            lbxTemperature.Items.Clear();
            cbxTempStep.Items.Clear();

            if (myTempUnits == ManRegGlobal.tempunitCELSIUS)
            {
                //using the default values to fill the axis range list
                foreach (string s in My.Settings.Default.AxisTempRange)
                {
                    lbxTemperature.Items.Add(s);
                }
                //using the default values to fill the grid steps
                foreach (string s in My.Settings.Default.GridTempSteps)
                {
                    cbxTempStep.Items.Add(Convert.ToDouble(s));
                }
            }

            if (myTempUnits == ManRegGlobal.tempunitFAHRENHEIT)
            {
                //using the default values to fill the axis range list
                foreach (string s in My.Settings.Default.AxisTempRangeF)
                {
                    lbxTemperature.Items.Add(s);
                }
                //using the default values to fill the grid steps
                foreach (string s in My.Settings.Default.GridTempStepsF)
                {
                    cbxTempStep.Items.Add(Convert.ToDouble(s));
                }
            }

            //setting the selected index
            lbxTemperature.SelectedIndex = 0;
            //setting default selection for the custom axis combo boxes
            cbxTempStep.SelectedIndex = 0;

        }

        public void rbAxisTemp_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (ReferenceEquals(sender, rbAxisTempCelsius) && rbAxisTempCelsius.Checked)
            {
                myTempUnits = ManRegGlobal.tempunitCELSIUS;
                myTempUnitsText = ManRegGlobal.CELSIUS_TEXT;
                loadTemperatureLists();
            }
            if (ReferenceEquals(sender, rbAxisTempFahrenheit) && rbAxisTempFahrenheit.Checked)
            {
                myTempUnits = ManRegGlobal.tempunitFAHRENHEIT;
                myTempUnitsText = ManRegGlobal.FAHRENHEIT_TEXT;
                loadTemperatureLists();
            }
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

        //Windget methods
        public void butPrev_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Retry;
        }

        public void cbxTempStep_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //selected a new temperature step, caluclating the power step
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

        public void cbxPwrStep_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            //selected a new power step, caluclating the temperature step
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
                lblTempAdjust.Text = Convert.ToString(Math.Round(tempStp, 2)) + " " + myTempUnits;
            }
        }

        public void rdbTemperature_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdbTemperature.Checked)
            {
                rdbPower.Checked = false;
                cbxTempStep.Enabled = true;
                cbxPwrStep.Enabled = false;
                //cbxTempStep.SelectedIndex = 0
                cbxTempStep_SelectedIndexChanged(rdbTemperature, null);
            }
        }

        public void rdbPower_CheckedChanged(object sender, System.EventArgs e)
        {
            if (rdbPower.Checked)
            {
                rdbTemperature.Checked = false;
                cbxPwrStep.Enabled = true;
                cbxTempStep.Enabled = false;
                //cbxPwrStep.SelectedIndex = 0
                cbxPwrStep_SelectedIndexChanged(rdbPower, null);
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

        public void butFinish_Click(object sender, System.EventArgs e)
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
    }
}
