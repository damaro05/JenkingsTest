// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
    internal class CStationData_SOLD : ICloneable
    {

        //Data
        public CStationInfoData Info = new CStationInfoData();
        public CStationSettingsData_SOLD Settings = new CStationSettingsData_SOLD();
        public CStationStatusData_SOLD Status = new CStationStatusData_SOLD();
        public List<CPeripheralData> Peripherals = new List<CPeripheralData>();


        public virtual dynamic Clone()
        {
            CStationData_SOLD cls_Station_Clonado = new CStationData_SOLD();
            cls_Station_Clonado.Info = (CStationInfoData)(this.Info.Clone());
            cls_Station_Clonado.Settings = (CStationSettingsData_SOLD)(this.Settings.Clone());
            cls_Station_Clonado.Status = (CStationStatusData_SOLD)(this.Status.Clone());

            return cls_Station_Clonado;
        }

    }
}
