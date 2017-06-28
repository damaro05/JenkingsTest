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
    internal class CCountersData_HA : ICloneable
    {

        public int ContPlugMinutes = 0; // Minutos la estación enchufada
        public int ContWorkMinutes = 0; // Minutos el puerto trabajando
        public int ContWorkCycles = 0; // Ciclos del puerto trabajando
        public int ContSuctionCycles = 0; // Ciclos del puerto en succión

        // Minutos el puerto sin herramienta, se puede calcular: ContPlugMinutes - ContWorkMinutes
        public int ContIdleMinutes
        {
            get
            {
                return ContPlugMinutes - (ContWorkMinutes);
            }
        }


        public dynamic Clone()
        {
            CCountersData_HA cls_Counters_Clonado = new CCountersData_HA();
            cls_Counters_Clonado.ContPlugMinutes = this.ContPlugMinutes;
            cls_Counters_Clonado.ContWorkMinutes = this.ContWorkMinutes;
            cls_Counters_Clonado.ContWorkCycles = this.ContWorkCycles;
            cls_Counters_Clonado.ContSuctionCycles = this.ContSuctionCycles;

            return cls_Counters_Clonado;
        }

    }
}
