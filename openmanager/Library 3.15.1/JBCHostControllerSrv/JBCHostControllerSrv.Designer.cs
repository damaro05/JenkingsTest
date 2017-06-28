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

using System.ServiceProcess;

namespace JBCHostControllerSrv
{
[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()] public
partial class JBCLocalServerSrv : System.ServiceProcess.ServiceBase
{
		
	//UserService reemplaza a Dispose para limpiar la lista de componentes.
	[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
	{
		try
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
		}
		finally
		{
			base.Dispose(disposing);
		}
	}
		
	// Punto de entrada principal del proceso
	[MTAThread()][System.Diagnostics.DebuggerNonUserCode()]public 
	static void Main()
	{
		System.ServiceProcess.ServiceBase[] ServicesToRun = null;
			
		// Puede que más de un servicio de NT se ejecute con el mismo proceso. Para agregar
		// otro servicio a este proceso, cambie la siguiente línea para
		// crear un segundo objeto de servicio. Por ejemplo,
		//
		//   ServicesToRun = New System.ServiceProcess.ServiceBase () {New Service1, New MySecondUserService}
		//
		ServicesToRun = new System.ServiceProcess.ServiceBase[] {new JBCLocalServerSrv()};
			
		System.ServiceProcess.ServiceBase.Run(ServicesToRun);
	}
		
	//Requerido por el Diseñador de componentes
	private System.ComponentModel.Container components = null;
		
	// NOTA: el Diseñador de componentes requiere el siguiente procedimiento
	// Se puede modificar utilizando el Diseñador de componentes.
	// No lo modifique con el editor de código.
	[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
	{
		components = new System.ComponentModel.Container();
		this.ServiceName = "Service1";
	}
		
	}
}
