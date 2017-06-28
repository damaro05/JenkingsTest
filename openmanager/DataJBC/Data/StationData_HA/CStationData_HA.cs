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
    internal class CStationData_HA : ICloneable
    {

        //Data
        public CStationInfoData Info = new CStationInfoData();
        public CStationSettingsData_HA Settings = new CStationSettingsData_HA();
        public CStationStatusData_HA Status = new CStationStatusData_HA();

        public virtual dynamic Clone()
        {
            CStationData_HA cls_Station_Clonado = new CStationData_HA();
            cls_Station_Clonado.Info = (CStationInfoData)(this.Info.Clone());
            cls_Station_Clonado.Settings = (CStationSettingsData_HA)(this.Settings.Clone());
            cls_Station_Clonado.Status = (CStationStatusData_HA)(this.Status.Clone());

            return cls_Station_Clonado;
        }

    }
}
