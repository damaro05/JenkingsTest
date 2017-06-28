// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports


namespace JBC_Connect
{
	
	
	internal class CStationData_FE : ICloneable
	{
		
		//Data
		public CStationInfoData Info = new CStationInfoData();
		public CStationSettingsData_FE Settings = new CStationSettingsData_FE();
		public CStationStatusData_FE Status = new CStationStatusData_FE();
		
		
		public virtual dynamic Clone()
		{
			CStationData_FE cls_Station_Clonado = new CStationData_FE();
			
			return cls_Station_Clonado;
		}
		
	}
	
}
