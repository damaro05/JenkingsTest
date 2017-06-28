// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports
using DataJBC;

namespace JBC_Connect
{


	public class CStationElement
	{
		internal string UUID = "";
		internal string ParentUUID = "";
		internal StationState State = StationState.Unknown;
		internal eStationType StationType = eStationType.UNKNOWN;
		internal CStation_SOLD Station_SOLD = null; // if soldering station
		internal CStation_HA Station_HA = null; // if hot air desoldering station
		internal CStation_SF Station_SF = null; // if soldering feeder station
		internal CStation_FE Station_FE = null; // if fume extractor station
	}

}
