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
    public partial class frmOptions
    {

        private Rectangle m_borderForm;
        private bool m_ChangingTexts = false;

        public delegate void ChangedLanguageEventHandler(string lang);
        private ChangedLanguageEventHandler ChangedLanguageEvent;

        public event ChangedLanguageEventHandler ChangedLanguage
        {
            add
            {
                ChangedLanguageEvent = (ChangedLanguageEventHandler)System.Delegate.Combine(ChangedLanguageEvent, value);
            }
            remove
            {
                ChangedLanguageEvent = (ChangedLanguageEventHandler)System.Delegate.Remove(ChangedLanguageEvent, value);
            }
        }

        public delegate void ChangedTemperatureUnitsEventHandler(string temp);
        private ChangedTemperatureUnitsEventHandler ChangedTemperatureUnitsEvent;

        public event ChangedTemperatureUnitsEventHandler ChangedTemperatureUnits
        {
            add
            {
                ChangedTemperatureUnitsEvent = (ChangedTemperatureUnitsEventHandler)System.Delegate.Combine(ChangedTemperatureUnitsEvent, value);
            }
            remove
            {
                ChangedTemperatureUnitsEvent = (ChangedTemperatureUnitsEventHandler)System.Delegate.Remove(ChangedTemperatureUnitsEvent, value);
            }
        }

        public delegate void ChangedShowErrorNotificationsEventHandler(bool show);
        private ChangedShowErrorNotificationsEventHandler ChangedShowErrorNotificationsEvent;

        public event ChangedShowErrorNotificationsEventHandler ChangedShowErrorNotifications
        {
            add
            {
                ChangedShowErrorNotificationsEvent = (ChangedShowErrorNotificationsEventHandler)System.Delegate.Combine(ChangedShowErrorNotificationsEvent, value);
            }
            remove
            {
                ChangedShowErrorNotificationsEvent = (ChangedShowErrorNotificationsEventHandler)System.Delegate.Remove(ChangedShowErrorNotificationsEvent, value);
            }
        }

        public delegate void ChangedShowStationControllerNotificationsEventHandler(bool show);
        private ChangedShowStationControllerNotificationsEventHandler ChangedShowStationControllerNotificationsEvent;

        public event ChangedShowStationControllerNotificationsEventHandler ChangedShowStationControllerNotifications
        {
            add
            {
                ChangedShowStationControllerNotificationsEvent = (ChangedShowStationControllerNotificationsEventHandler)System.Delegate.Combine(ChangedShowStationControllerNotificationsEvent, value);
            }
            remove
            {
                ChangedShowStationControllerNotificationsEvent = (ChangedShowStationControllerNotificationsEventHandler)System.Delegate.Remove(ChangedShowStationControllerNotificationsEvent, value);
            }
        }

        public delegate void ChangedShowStationNotificationsEventHandler(bool show);
        private ChangedShowStationNotificationsEventHandler ChangedShowStationNotificationsEvent;

        public event ChangedShowStationNotificationsEventHandler ChangedShowStationNotifications
        {
            add
            {
                ChangedShowStationNotificationsEvent = (ChangedShowStationNotificationsEventHandler)System.Delegate.Combine(ChangedShowStationNotificationsEvent, value);
            }
            remove
            {
                ChangedShowStationNotificationsEvent = (ChangedShowStationNotificationsEventHandler)System.Delegate.Remove(ChangedShowStationNotificationsEvent, value);
            }
        }



        public frmOptions()
        {
            InitializeComponent();
            InitializeData();
            ReLoadTexts();
        }

        public void frmOptions_Load(object sender, EventArgs e)
        {
            TreeView.Nodes[0].Expand();
            m_borderForm = this.ClientRectangle;
        }

        public void frmOptions_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            CPanelStyle.SetPanelStyle(ref e, ref m_borderForm);
        }

        public void InitializeData()
        {

            //Configure language
            if (Localization.curCulture == "de") //Deutsch
            {
                comboBoxLanguages.SelectedIndex = 0;
            }
            else if (Localization.curCulture == "en") //English
            {
                comboBoxLanguages.SelectedIndex = 1;
            }
            else if (Localization.curCulture == "ja") //Japaness
            {
                comboBoxLanguages.SelectedIndex = 3;
            }
            else //Spanish
            {
                comboBoxLanguages.SelectedIndex = 2;
            }

            //Configure temperature units
            if (Configuration.curViewTempUnits == "C") //Celsius
            {
                comboBoxTemperatureUnits.SelectedIndex = 0;
            }
            else //Fahrenheit
            {
                comboBoxTemperatureUnits.SelectedIndex = 1;
            }

            //Configure show notifications
            checkBox_ShowErrorNotifications.Checked = Configuration.curShowErrorNotifications;
            checkBox_ShowStationControllerNotifications.Checked = Configuration.curShowStationControllerNotifications;
            checkBox_ShowStationNotifications.Checked = Configuration.curShowStationNotifications;

        }

        public void ReLoadTexts()
        {

            //esto bloquea que se lancen los eventos al cambiar los combox
            m_ChangingTexts = true;

            this.Text = Localization.getResStr(Configuration.mnuOptionsId);

            //Language
            labelLanguages.Text = Localization.getResStr(Configuration.mnuLanguagesId);
            int comboBoxSelectedIndex = comboBoxLanguages.SelectedIndex;
            comboBoxLanguages.Items.Clear();
            comboBoxLanguages.Items.Insert(0, Localization.getResStr(Configuration.mnuLangDEId));
            comboBoxLanguages.Items.Insert(1, Localization.getResStr(Configuration.mnuLangENId));
            comboBoxLanguages.Items.Insert(2, Localization.getResStr(Configuration.mnuLangESId));
            comboBoxLanguages.Items.Insert(3, Localization.getResStr(Configuration.mnuLangJAId));
            comboBoxLanguages.SelectedIndex = comboBoxSelectedIndex;

            //Temperature units
            labelTemperatureUnit.Text = Localization.getResStr(Configuration.mnuTempUnitId);

            //Notifications
            groupBox_Notifications.Text = Localization.getResStr(Configuration.mnuViewWarningId);
            checkBox_ShowErrorNotifications.Text = Localization.getResStr(Configuration.mnuShowErrorNotificationsId);
            checkBox_ShowStationControllerNotifications.Text = Localization.getResStr(Configuration.mnuShowStationControllerNotificationsId);
            checkBox_ShowStationNotifications.Text = Localization.getResStr(Configuration.mnuShowStationNotificationsId);

            //Close button
            butClose.Text = Localization.getResStr(Configuration.dockcloseId);

            //Tree
            TreeView.Nodes[0].Text = Localization.getResStr(Configuration.mnuEnvironmentId); //Environment
            TreeView.Nodes[0].Nodes[0].Text = Localization.getResStr(Configuration.mnuInternationalSettingsId); //International settings
            TreeView.Nodes[0].Nodes[1].Text = Localization.getResStr(Configuration.mnuUnitsId); //Units
            TreeView.Nodes[0].Nodes[2].Text = Localization.getResStr(Configuration.mnuViewWarningId); //Notifications

            m_ChangingTexts = false;

        }

        public void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TabControl.SelectedIndex = TreeView.SelectedNode.Index;
        }

        public void butClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Environment

        #region International settings

        public void comboBoxLanguages_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (m_ChangingTexts)
            {
                return;
            }

            if (comboBoxLanguages.SelectedIndex == 0) //Deutsch
            {
                if (ChangedLanguageEvent != null)
                    ChangedLanguageEvent("de");
            }
            else if (comboBoxLanguages.SelectedIndex == 1) //English
            {
                if (ChangedLanguageEvent != null)
                    ChangedLanguageEvent("en");
            }
            else if (comboBoxLanguages.SelectedIndex == 3) //Japaness
            {
                if (ChangedLanguageEvent != null)
                    ChangedLanguageEvent("ja");
            }
            else //Spanish
            {
                if (ChangedLanguageEvent != null)
                    ChangedLanguageEvent("es");
            }
        }

        #endregion

        #region Units

        public void comboBoxTemperatureUnits_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (m_ChangingTexts)
            {
                return;
            }

            if (comboBoxTemperatureUnits.SelectedIndex == 0) //Celsius
            {
                if (ChangedTemperatureUnitsEvent != null)
                    ChangedTemperatureUnitsEvent("C");
            }
            else //Fahrenheit
            {
                if (ChangedTemperatureUnitsEvent != null)
                    ChangedTemperatureUnitsEvent("F");
            }
        }

        #endregion

        #region Notifications

        public void checkBox_ShowErrorNotifications_CheckedChanged(object sender, EventArgs e)
        {
            if (m_ChangingTexts)
            {
                return;
            }
            if (ChangedShowErrorNotificationsEvent != null)
                ChangedShowErrorNotificationsEvent(checkBox_ShowErrorNotifications.Checked);
        }

        public void checkBox_ShowStationControllerNotifications_CheckedChanged(object sender, EventArgs e)
        {
            if (m_ChangingTexts)
            {
                return;
            }
            if (ChangedShowStationControllerNotificationsEvent != null)
                ChangedShowStationControllerNotificationsEvent(checkBox_ShowStationControllerNotifications.Checked);
        }

        public void checkBox_ShowStationNotifications_CheckedChanged(object sender, EventArgs e)
        {
            if (m_ChangingTexts)
            {
                return;
            }
            if (ChangedShowStationNotificationsEvent != null)
                ChangedShowStationNotificationsEvent(checkBox_ShowStationNotifications.Checked);
        }

        #endregion

        #endregion

    }
}
