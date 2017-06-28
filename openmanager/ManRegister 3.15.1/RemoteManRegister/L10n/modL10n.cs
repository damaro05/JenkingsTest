// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
// End of VB project level imports

using System.Reflection;


namespace RemoteManRegister
{
    public sealed class Localization
    {

        public static string curCulture = "";


        public static string getResStr(string strId)
        {

            return My.Resources.Resources.ResourceManager.GetString(strId);
        }

        public static void changeCulture(string sCultureName)
        {

            if (sCultureName == "en")
            {
                sCultureName = ""; // neutral = English
            }
            curCulture = sCultureName;
            (new Microsoft.VisualBasic.ApplicationServices.ConsoleApplicationBase()).ChangeUICulture(sCultureName);
        }

    }
}
