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


namespace DataJBC
{
	
	
	internal class CCountersData_FE : ICloneable
	{
		
		public int ContPlugMinutes = 0; // Minutos conectado
		public int ContIdleMinutes = 0; // Minutos sin trabajar
		public int ContWorkIntakeMinutes = 0; // Minutos el puerto trabajando en work
		public int ContStandIntakeMinutes = 0; // Minutos el puerto trabajando en stand
		public int ContWorkCycles = 0; // Ciclos del puerto trabajando
		
		
		public dynamic Clone()
		{
			CCountersData_FE cls_Counters_Clonado = new CCountersData_FE();
			cls_Counters_Clonado.ContPlugMinutes = this.ContPlugMinutes;
			cls_Counters_Clonado.ContIdleMinutes = this.ContIdleMinutes;
			cls_Counters_Clonado.ContWorkIntakeMinutes = this.ContWorkIntakeMinutes;
			cls_Counters_Clonado.ContStandIntakeMinutes = this.ContStandIntakeMinutes;
			cls_Counters_Clonado.ContWorkCycles = this.ContWorkCycles;
			
			return cls_Counters_Clonado;
		}
		
	}
	
}
