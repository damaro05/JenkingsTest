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
    public sealed class LoggerModule
    {

        internal static log4net.ILog logger;


        internal static void InitLogger()
        {
            logger = log4net.LogManager.GetLogger("GeneralLogger");
        }

    }
}
