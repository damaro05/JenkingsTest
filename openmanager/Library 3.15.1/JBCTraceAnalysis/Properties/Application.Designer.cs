// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FormTraceAnalysis
{
    namespace My
    {

        //NOTE: This file is auto-generated; do not modify it directly.  To make changes,
        // or if you encounter build errors in this file, go to the Project Designer
        // (go to Project Properties or double-click the My Project node in
        // Solution Explorer), and make changes on the Application tab.
        //
        public partial class MyApplication : global::Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
        {
            [STAThread]
            static void Main()
            {
                (new MyApplication()).Run(new string[] { });
            }

            [global::System.Diagnostics.DebuggerStepThrough()]
            public MyApplication() : base(global::Microsoft.VisualBasic.ApplicationServices.AuthenticationMode.Windows)
            {



                this.IsSingleInstance = false;
                this.EnableVisualStyles = true;
                this.SaveMySettingsOnExit = true;
                this.ShutdownStyle = global::Microsoft.VisualBasic.ApplicationServices.ShutdownMode.AfterMainFormCloses;
            }

            [global::System.Diagnostics.DebuggerStepThroughAttribute()]
            protected override void OnCreateMainForm()
            {
                this.MainForm = FormTraceAnalysis.Default;
            }
        }
    }
}
