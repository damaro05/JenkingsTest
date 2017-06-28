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
    public partial class frmAbout
    {

        private CComHostController m_comHostController;
        private string m_swStationController = "";
        private string m_swHostController = "";


        public frmAbout(CComHostController comHostController)
        {
            InitializeComponent();
            m_comHostController = comHostController;
        }

        public void frmAbout_Load(System.Object sender, System.EventArgs e)
        {
            m_swStationController = m_comHostController.GetSwStationController();
            m_swHostController = m_comHostController.GetSwHostController();
            ReLoadTexts();
        }

        public void frmAbout_LostFocus(object sender, System.EventArgs e)
        {
            this.Close();
        }

        public void ReLoadTexts()
        {
            string[] tempLines = textBox.Lines;

            tempLines[1] = Localization.getResStr(Configuration.aboutLineRemoteManagerId) + (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.Version.ToString();
            tempLines[2] = Localization.getResStr(Configuration.aboutLineStationControllerId) + m_swStationController;
            tempLines[3] = Localization.getResStr(Configuration.aboutLineHostControllerId) + m_swHostController;
            tempLines[6] = Localization.getResStr(Configuration.aboutLineRightsId);

            textBox.Lines = tempLines;
        }

    }
}
