// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


namespace RemoteManager
{
    public class LabelTimeFormat : IFormatProvider, ICustomFormatter
    {

        public dynamic GetFormat(Type formatType)
        {
            return this;
        }

        public string Format(string format__1, object arg, IFormatProvider formatProvider)
        {
            return Format(System.Convert.ToInt32(arg));
        }

        public static string Format(int iTime)
        {
            string sTime = "";

            //Calculate time values
            int timeInSeconds = iTime / 10;
            int seconds = timeInSeconds % 60;

            var timeInMinutes = timeInSeconds / 60;
            int minutes = timeInMinutes % 60;

            //Compose time string
            sTime += minutes + ":";

            if (seconds < 10)
            {
                sTime += "0";
            }
            sTime += System.Convert.ToString(seconds);

            return sTime;
        }

    }
}
