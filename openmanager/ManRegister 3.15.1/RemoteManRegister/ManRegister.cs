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
    public class ManRegister
    {

        // for register forms
        public struct tRegister
        {
            public frmMainRegister frm;
            public int frmID;
        }
        public List<tRegister> registerList;

        // register form ID counter
        private int lastFrmID;
        internal JBC_API_Remote jbc;

        public ManRegister(JBC_API_Remote pJBC)
        {
            jbc = pJBC;
            jbc.NewStationConnected += JBC_NewStationConnected;
            jbc.StationDisconnected += JBC_StationDisconnected;
            //jbc.UserError += JBC_UserError;
            registerList = new List<tRegister>();
            lastFrmID = -1;
        }

        public int newFrm(Form frmParent, bool bMDI, string currentTempUnits)
        {

            tRegister myReg = new tRegister();

            lastFrmID++;
            myReg.frm = new frmMainRegister(lastFrmID, currentTempUnits, jbc);
            if (frmParent != null)
            {
                if (bMDI)
                {
                    myReg.frm.MdiParent = frmParent;
                }
                else
                {
                    myReg.frm.Owner = frmParent;
                }
            }
            myReg.frm.ClosedFrm += onClosedFrm;
            myReg.frmID = lastFrmID;
            registerList.Add(myReg);
            return myReg.frmID;
        }

        // handles form closing
        private void onClosedFrm(int frmID)
        {
            removeFrm((ulong)frmID);
        }

        //Public Sub setCulture(ByVal sCulture As String)
        //    changeCulture(sCulture)
        //    ReloadTexts()
        //End Sub

        public void reloadTexts()
        {
            foreach (tRegister reg in registerList)
            {
                reg.frm.ReloadTexts();
            }
        }

        #region JBC_Connect events
        private void JBC_NewStationConnected(long stationID)
        {
            // Adding the station to the list of connected stations
            ManRegGlobal.connectedStations.Add(new ManRegGlobal.cConnectedStation(Convert.ToString(stationID), -1));
        }

        private void JBC_StationDisconnected(long stationID)
        {
            // Removing the station of the list of connected stations
            foreach (ManRegGlobal.cConnectedStation connStn in ManRegGlobal.connectedStations)
            {
                if (long.Parse(connStn.ID) == stationID)
                {
                    // remove station from list
                    ManRegGlobal.connectedStations.Remove(connStn);
                    break;
                }
            }
        }

        //private void JBC_UserError(long stationID, JBC_API_Remote.Cerror err)
        //{
        //    if (err.GetCode() != ManRegGlobal.lastError)
        //    {
        //        ManRegGlobal.lastError = (int) (err.GetCode());
        //        ManRegGlobal.errorCatch = true;
        //        //MsgBox("JBC_Connect API error: " & err.GetMsg(), MsgBoxStyle.Critical)
        //    }
        //}

        #endregion

        #region Form routines
        internal void removeFrm(ulong frmID)
        {
            //getting the station index in the list
            int index = getFrmIndex((int)frmID);
            if (index >= 0)
            {
                if (registerList[index].frm != null)
                {
                    if (!registerList[index].frm.IsDisposed)
                    {
                        registerList[index].frm.Dispose();
                    }
                }
                registerList.RemoveAt(index);
            }
        }

        public Form getFrm(int frmID)
        {
            //looking for the station in the list
            bool found = false;
            int cnt = 0;
            while (cnt < registerList.Count && !found)
            {
                if (registerList[cnt].frmID == frmID)
                {
                    return registerList[cnt].frm;
                }
                else
                {
                    cnt++;
                }
            }
            return null;
        }

        private int getFrmIndex(int frmID)
        {
            //looking for the station in the list
            bool found = false;
            int cnt = 0;
            while (cnt < registerList.Count && !found)
            {
                if (registerList[cnt].frmID == frmID)
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

        #endregion
    }
}
