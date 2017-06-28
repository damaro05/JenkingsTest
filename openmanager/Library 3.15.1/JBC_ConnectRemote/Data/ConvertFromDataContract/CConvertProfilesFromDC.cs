// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
using JBC_ConnectRemote.JBCService;
using DataJBC;
// End of VB project level imports


namespace JBC_ConnectRemote
{
    public class CConvertProfilesFromDC
    {

        public static void CopyData(CProfileData_HA profileData,
                                    dc_Profile_HA dcProfile)
        {

            profileData.Name = dcProfile.Name;
            profileData.Data = new byte[dcProfile.Data.Length - 1 + 1];
            Array.Copy(dcProfile.Data, profileData.Data, System.Convert.ToInt32(dcProfile.Data.Length));
        }

    }
}
