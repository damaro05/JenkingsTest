// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
using DataJBC;
// End of VB project level imports


namespace JBC_ConnectRemote
{


    internal class CStationElement
    {
        internal string UUID = "";
        public CStation.StationState State;
        public eStationType StationType = eStationType.UNKNOWN;
        internal CStation_Sold Station_SOLD = null; // if soldering station
        internal CStation_HA Station_HA = null; // if hot air desoldering station
        internal CStation_SF Station_SF = null; // if soldering feeder station
    }

}
