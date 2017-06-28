// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Reflection;
using System.Runtime.InteropServices;


// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

// Review the values of the assembly attributes

[assembly:AssemblyTitle("JBCTraceControlLocalSrv")]
[assembly:AssemblyDescription("")]
[assembly:AssemblyProduct("JBCTraceControlLocalSrv")]

//The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly:Guid("23ae6869-9c4a-4700-b34e-89bee54cd75f")]

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
