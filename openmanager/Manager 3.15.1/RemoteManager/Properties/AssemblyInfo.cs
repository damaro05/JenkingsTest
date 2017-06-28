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

using System.Reflection;
using System.Runtime.InteropServices;


// La información general sobre un ensamblado se controla mediante el siguiente
// conjunto de atributos. Cambie estos atributos para modificar la información
// asociada con un ensamblado.

// Revisar los valores de los atributos del ensamblado

[assembly:AssemblyTitle("Remote Manager")]
[assembly:AssemblyDescription("Allows full JBC station PC remote control.")]
[assembly:AssemblyProduct("Remote Manager")]

//El siguiente GUID sirve como identificador de typelib si este proyecto se expone a COM
[assembly:Guid("66f9d6f7-8005-4ecc-953a-116359dc58b6")]

[assembly:log4net.Config.XmlConfigurator(ConfigFile="log4net.config",Watch=true)]
