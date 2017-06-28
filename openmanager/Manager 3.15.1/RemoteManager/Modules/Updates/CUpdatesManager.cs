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

using System.IO;
using System.ServiceModel;
using System.Threading;
using RemoteManager.HostControllerServiceReference;
using JBC_ConnectRemote;


namespace RemoteManager
{
    public class CUpdatesManager
    {

        private const int TIME_CHECK_UPDATE = 15 * 60 * 1000; //15 minutes


        //Forms
        private frmMain m_frmMain;
        private frmUpdates m_frmUpdates = null;
        private frmUpdatesReInstall m_frmReInstall = null;

        //JBC connect
        private JBC_API_Remote m_jbc;

        //Communications
        private CComHostController m_comHostController;

        private bool m_isUpdating = false;


        //Automatic check updates
        private Thread m_ThreadAutomaticCheckUpdate;


        public delegate void CancelUpdatedReInstallEventHandler();
        private CancelUpdatedReInstallEventHandler CancelUpdatedReInstallEvent;

        public event CancelUpdatedReInstallEventHandler CancelUpdatedReInstall
        {
            add
            {
                CancelUpdatedReInstallEvent = (CancelUpdatedReInstallEventHandler)System.Delegate.Combine(CancelUpdatedReInstallEvent, value);
            }
            remove
            {
                CancelUpdatedReInstallEvent = (CancelUpdatedReInstallEventHandler)System.Delegate.Remove(CancelUpdatedReInstallEvent, value);
            }
        }

        public delegate void UpdateAvailableEventHandler();
        private UpdateAvailableEventHandler UpdateAvailableEvent;

        public event UpdateAvailableEventHandler UpdateAvailable
        {
            add
            {
                UpdateAvailableEvent = (UpdateAvailableEventHandler)System.Delegate.Combine(UpdateAvailableEvent, value);
            }
            remove
            {
                UpdateAvailableEvent = (UpdateAvailableEventHandler)System.Delegate.Remove(UpdateAvailableEvent, value);
            }
        }



        public CUpdatesManager(frmMain frmMain, CComHostController comHostController, JBC_API_Remote jbc)
        {
            m_frmMain = frmMain;
            m_jbc = jbc;
            m_comHostController = comHostController;

            //Initialize automatic check updates
            m_ThreadAutomaticCheckUpdate = new Thread(new System.Threading.ThreadStart(AutomaticCheckUpdate));
            m_ThreadAutomaticCheckUpdate.IsBackground = true;
            m_ThreadAutomaticCheckUpdate.Start();
        }

        public void ReLoadTexts()
        {
            if (m_frmUpdates != null)
            {
                m_frmUpdates.ReLoadTexts();
            }

            if (m_frmReInstall != null)
            {
                m_frmReInstall.ReLoadTexts();
            }
        }

        public void ShowFormUpdates()
        {
            if (ReferenceEquals(m_frmUpdates, null))
            {
                m_frmUpdates = new frmUpdates(m_comHostController, m_jbc);
                m_frmUpdates.MdiParent = m_frmMain;
                m_frmUpdates.Dock = DockStyle.None;

                m_frmUpdates.Show();
            }
            else
            {
                if (m_frmUpdates.Visible)
                {
                    m_frmUpdates.Show();
                }
                else
                {
                    m_frmUpdates.Hide();
                }
            }
        }

        public void ShowFormReInstall()
        {

            //Close update configuration panel
            if (m_frmUpdates != null)
            {
                m_frmUpdates.Close();
                m_frmUpdates = null;
            }

            if (ReferenceEquals(m_frmReInstall, null))
            {
                m_frmReInstall = new frmUpdatesReInstall(m_comHostController);
                m_frmReInstall.MdiParent = m_frmMain;
                m_frmReInstall.cancelUpdatedReInstall += Event_CancelUpdateReInstall;
            }

            m_frmReInstall.Show();
        }

        private void Event_CancelUpdateReInstall()
        {
            if (m_frmReInstall != null)
            {
                m_frmReInstall.Close();
                m_frmReInstall = null;
            }

            if (CancelUpdatedReInstallEvent != null)
                CancelUpdatedReInstallEvent();
        }

        private void AutomaticCheckUpdate()
        {
            bool bContinue = true;

            while (bContinue)
            {

                dc_InfoCheckPeriodicTime infoCheckPeriodic = m_comHostController.GetCheckPeriodicTime();
                if (infoCheckPeriodic.available)
                {

                    dc_InfoUpdateSoftware infoUpdate = m_comHostController.CheckUpdate();
                    if (infoUpdate.stationControllerSwAvailable || infoUpdate.remoteManagerSwAvailable || infoUpdate.hostControllerSwAvailable || infoUpdate.webManagerSwAvailable)
                    {
                        //Only raise event once
                        bContinue = false;
                        if (UpdateAvailableEvent != null)
                            UpdateAvailableEvent();
                    }
                }

                Thread.Sleep(TIME_CHECK_UPDATE);
            }
        }

    }
}
