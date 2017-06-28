// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBCUpdaterSrv
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
