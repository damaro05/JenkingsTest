Imports System
Imports System.Reflection
Imports System.Runtime.InteropServices

' La información general sobre un ensamblado se controla mediante el siguiente 
' conjunto de atributos. Cambie estos atributos para modificar la información
' asociada con un ensamblado.

' Revisar los valores de los atributos del ensamblado

<Assembly: AssemblyTitle("JBCHostUpdaterSrv")> 
<Assembly: AssemblyDescription("")> 
<Assembly: AssemblyProduct("JBCHostUpdaterSrv")> 

'El siguiente GUID sirve como identificador de typelib si este proyecto se expone a COM
<Assembly: Guid("2dcd70e5-f20b-435f-a24e-3a7b98b575c4")> 

<Assembly: log4net.Config.XmlConfigurator(ConfigFile:="log4net.config", Watch:=True)>