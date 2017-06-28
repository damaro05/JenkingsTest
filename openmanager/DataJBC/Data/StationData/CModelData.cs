// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
using Microsoft.VisualBasic;
// End of VB project level imports

namespace DataJBC
{
    public class CModelData
    {

        public string Model = "";
        public string ModelType = "";
        public int ModelVersion = 0;


        public CModelData(string sModelString)
        {
            //En protocol 01: model or model_modelversion
            //En protocol 02: model_modeltype_modelversion
            string[] arrModelData = sModelString.Split('_');
            Model = arrModelData[0].Trim();

            if (arrModelData.Length > 1)
            {
                ModelType = arrModelData[1].Trim();

                if (arrModelData.Length > 2)
                {
                    if (Information.IsNumeric(arrModelData[2]))
                    {
                        ModelVersion = int.Parse(arrModelData[2]);
                    }
                }
            }
        }

    }
}
