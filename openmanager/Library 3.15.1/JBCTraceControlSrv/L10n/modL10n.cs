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

using System.Reflection;

namespace JBCTraceControlLocalSrv
{
    internal sealed class Localization
    {
        internal static string curCulture = "";

        internal static string getResStr(string strId)
        {
            return My.Resources.Resources.ResourceManager.GetString(strId);
        }

        internal static void changeCulture(string sCultureName)
        {
            if (sCultureName == "en")
            {
                sCultureName = ""; // neutral = English
            }
            curCulture = sCultureName;
            (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).ChangeUICulture(sCultureName);
        }

    }
}
