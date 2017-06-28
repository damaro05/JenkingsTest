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
using System.Runtime.InteropServices;


// La información general sobre un ensamblado se controla mediante el siguiente
// conjunto de atributos. Cambie estos atributos para modificar la información
// asociada con un ensamblado.

// Revisar los valores de los atributos del ensamblado

[assembly:AssemblyTitle("JBC Host Controller Service")]
[assembly:AssemblyDescription("Allows full JBC station PC remote control.")]
[assembly:AssemblyProduct("JBC Host Controller Service")]

//El siguiente GUID sirve como identificador de typelib si este proyecto se expone a COM
[assembly:Guid("6a2f8dd0-b2eb-40ec-8191-d26cdc096886")]

[assembly:log4net.Config.XmlConfigurator(ConfigFile="log4net.config",Watch=true)]
