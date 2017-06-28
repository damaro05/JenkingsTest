// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Reflection;
using System.Runtime.InteropServices;


// La información general sobre un ensamblado se controla mediante el siguiente
// conjunto de atributos. Cambie estos atributos para modificar la información
// asociada con un ensamblado.

// Revisar los valores de los atributos del ensamblado

[assembly:AssemblyTitle("JBC Station Controller Service")]
[assembly:AssemblyDescription("")]
[assembly:AssemblyProduct("JBC Station Controller Service")]

//El siguiente GUID sirve como identificador de typelib si este proyecto se expone a COM
[assembly:Guid("815e257c-7699-476e-9aed-e50dcefe85e4")]

[assembly:log4net.Config.XmlConfigurator(ConfigFile="log4net.config",Watch=true)]
