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


namespace RemoteManager
{
    public partial class ProfileNewDialog
    {

        public ProfileNewDialog(bool showRegulationMode = false)
        {

            // This call is required by the designer.
            InitializeComponent();

            if (!showRegulationMode)
            {
                //hide option
                this.labelRegulationMode.Visible = false;
                this.rbModeAirTemp.Visible = false;
                this.rbModeExtTCTemp.Visible = false;

                //Corrected height elements
                int heightCorrection = this.labelProfileName.Location.Y - this.labelRegulationMode.Location.Y;

                this.labelProfileName.Location = new Point(this.labelProfileName.Location.X, this.labelProfileName.Location.Y - heightCorrection);
                this.labelProfileNameHelp.Location = new Point(this.labelProfileNameHelp.Location.X, this.labelProfileNameHelp.Location.Y - heightCorrection);
                this.txtboxProfileName.Location = new Point(this.txtboxProfileName.Location.X, this.txtboxProfileName.Location.Y - heightCorrection);
                this.butCancel.Location = new Point(this.butCancel.Location.X, this.butCancel.Location.Y - heightCorrection);
                this.butOk.Location = new Point(this.butOk.Location.X, this.butOk.Location.Y - heightCorrection);

                this.Height -= heightCorrection;
            }
        }

        public void txtboxProfileName_KeyPress(object sender, KeyPressEventArgs e)
        {

            //13 = enter
            if (Strings.Asc(e.KeyChar) == 13)
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }

            //27 = esc
            if (Strings.Asc(e.KeyChar) == 27)
            {
                DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }

            //8 = backspace
            if (Strings.Asc(e.KeyChar) != 8)
            {
                //65 = A; 90 = Z; 97 = a; 122 = z
                if (!(e.KeyChar >= Convert.ToChar("A") && e.KeyChar <= Convert.ToChar("Z")) &&
                    !(e.KeyChar >= Convert.ToChar("a") && e.KeyChar <= Convert.ToChar("z")) &&
                    !(e.KeyChar >= Convert.ToChar("0") && e.KeyChar <= Convert.ToChar("9")))
                {
                    e.Handled = true;
                }
            }
        }

        public void butOk_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        public void butCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

    }
}
