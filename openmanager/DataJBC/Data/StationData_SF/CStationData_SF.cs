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
    internal class CStationData_SF : ICloneable
    {

        //Data
        public CStationInfoData Info = new CStationInfoData();
        public CStationSettingsData_SF Settings = new CStationSettingsData_SF();
        public CStationStatusData_SF Status = new CStationStatusData_SF();


        public virtual dynamic Clone()
        {
            CStationData_SF cls_Station_Clonado = new CStationData_SF();
            cls_Station_Clonado.Info = (CStationInfoData)(this.Info.Clone());
            cls_Station_Clonado.Settings = (CStationSettingsData_SF)(this.Settings.Clone());
            cls_Station_Clonado.Status = (CStationStatusData_SF)(this.Status.Clone());

            return cls_Station_Clonado;
        }

    }
}
