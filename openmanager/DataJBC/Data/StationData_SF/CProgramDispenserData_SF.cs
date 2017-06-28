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
	public class CProgramDispenserData_SF : ICloneable
	{

		public OnOff Enabled = OnOff._OFF;
		public string Name = "";
		public ushort Length_1 = 0;
		public ushort Speed_1 = 0;
		public ushort Length_2 = 0;
		public ushort Speed_2 = 0;
		public ushort Length_3 = 0;
		public ushort Speed_3 = 0;


		public dynamic Clone()
		{
			CProgramDispenserData_SF cls_Clonado = new CProgramDispenserData_SF();
			cls_Clonado.Enabled = this.Enabled;			
			cls_Clonado.Name = this.Name;
			cls_Clonado.Length_1 = this.Length_1;
			cls_Clonado.Speed_1 = this.Speed_1;
			cls_Clonado.Length_2 = this.Length_2;
			cls_Clonado.Speed_2 = this.Speed_2;
			cls_Clonado.Length_3 = this.Length_3;
			cls_Clonado.Speed_3 = this.Speed_3;

			return cls_Clonado;
		}

	}
}
