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
    internal class CCountersData_SOLD : ICloneable
    {

        public int ContPlugMinutes = 0; // Minutos la estación enchufada
        public int ContWorkMinutes = 0; // Minutos el puerto trabajando
        public int ContSleepMinutes = 0; // Minutos el puerto en sleep
        public int ContHiberMinutes = 0; // Minutos el puerto hibernando
        public int ContIdleMinutes = 0; // Minutos el puerto sin herramienta, se puede calcular: ContPlugMinutes - (ContWorkMinutes + ContSleepMinutes + ContHiberMinutes)
        public int ContSleepCycles = 0; // Ciclos de sleep
        public int ContDesoldCycles = 0; // Ciclos de desoldador, pulsaciones de aspiración


        public dynamic Clone()
        {
            CCountersData_SOLD cls_Counters_Clonado = new CCountersData_SOLD();
            cls_Counters_Clonado.ContPlugMinutes = this.ContPlugMinutes;
            cls_Counters_Clonado.ContWorkMinutes = this.ContWorkMinutes;
            cls_Counters_Clonado.ContSleepMinutes = this.ContSleepMinutes;
            cls_Counters_Clonado.ContHiberMinutes = this.ContHiberMinutes;
            cls_Counters_Clonado.ContIdleMinutes = this.ContIdleMinutes;
            cls_Counters_Clonado.ContSleepCycles = this.ContSleepCycles;
            cls_Counters_Clonado.ContDesoldCycles = this.ContDesoldCycles;

            return cls_Counters_Clonado;
        }

    }
}
