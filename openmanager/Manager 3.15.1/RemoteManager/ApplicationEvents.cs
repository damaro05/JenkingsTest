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
    namespace My
    {

        // The following events are available for MyApplication:
        //
        // Startup: Raised when the application starts, before the startup form is created.
        // Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
        // UnhandledException: Raised if the application encounters an unhandled exception.
        // StartupNextInstance: Raised when launching a single-instance application and the application is already active.
        // NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
        partial class MyApplication : global::Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
        {

            protected override bool OnInitialize(System.Collections.ObjectModel.ReadOnlyCollection<string> commandLineArgs)
            {
                this.MinimumSplashScreenDisplayTime = 5500;
                return base.OnInitialize(commandLineArgs);
            }

        }

    }
}
