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

namespace JBCTraceControlLocalSrv
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
