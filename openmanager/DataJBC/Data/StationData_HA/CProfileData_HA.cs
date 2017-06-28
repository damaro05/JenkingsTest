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
    public class CProfileData_HA : ICloneable
    {

        public string Name;
        public byte[] Data = new byte[0]; //Raw data in JSON


        public dynamic Clone()
        {
            CProfileData_HA cls_Clonado = new CProfileData_HA();
            cls_Clonado.Name = this.Name;
            cls_Clonado.Data = new byte[this.Data.Length - 1 + 1];
            Array.Copy(this.Data, cls_Clonado.Data, this.Data.Length);

            return cls_Clonado;
        }

    }
}
