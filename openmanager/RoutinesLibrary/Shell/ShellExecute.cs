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

using System.Runtime.InteropServices;


namespace RoutinesLibrary.Shell
{
		
	/// <summary>
	/// This class provides a mechanism to interactuate with the shell command
	/// </summary>
	public class ShellExecute
	{
			
		private const int SE_ERR_FNF = 2;
		private const int SE_ERR_PNF = 3;
		private const int SE_ERR_ACCESSDENIED = 5;
		private const int SE_ERR_OOM = 8;
		private const int SE_ERR_DLLNOTFOUND = 32;
		private const int SE_ERR_SHARE = 26;
		private const int SE_ERR_ASSOCINCOMPLETE = 27;
		private const int SE_ERR_DDETIMEOUT = 28;
		private const int SE_ERR_DDEFAIL = 29;
		private const int SE_ERR_DDEBUSY = 30;
		private const int SE_ERR_NOASSOC = 31;
		private const int ERROR_BAD_FORMAT = 11;
		private const int SW_SHOWNOACTIVATE = 4;
			
		[DllImport("shell32.dll",EntryPoint="ShellExecuteA", ExactSpelling=true, CharSet=CharSet.Ansi, SetLastError=true)]
		private static extern long ShellExecuteA(long hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, long nShowCmd);
			
		[DllImport("user32", ExactSpelling=true, CharSet=CharSet.Ansi, SetLastError=true)]
		private static extern long GetDesktopWindow();
			
		/// <summary>
		/// Open a document
		/// </summary>
		/// <param name="docName">Document path</param>
		/// <param name="msgError">Message error</param>
		/// <returns>True if the operation was succesful</returns>
		public static bool OpenDocument(string docName, ref string msgError)
		{
			msgError = "";
            long result = System.Convert.ToInt64(ShellExecuteA(System.Convert.ToInt64(GetDesktopWindow()), "open", docName, "", "", SW_SHOWNOACTIVATE));
				
			if (result < 32)
			{
				//There was an error
				switch (result)
				{
					case SE_ERR_FNF:
						msgError = "File not found";
						break;
					case SE_ERR_PNF:
						msgError = "Path not found";
						break;
					case SE_ERR_ACCESSDENIED:
						msgError = "Access denied";
						break;
					case SE_ERR_OOM:
						msgError = "Out of memory";
						break;
					case SE_ERR_DLLNOTFOUND:
						msgError = "DLL not found";
						break;
					case SE_ERR_SHARE:
						msgError = "A sharing violation occurred";
						break;
					case SE_ERR_ASSOCINCOMPLETE:
						msgError = "Incomplete or invalid file association";
						break;
					case SE_ERR_DDETIMEOUT:
						msgError = "DDE Time out";
						break;
					case SE_ERR_DDEFAIL:
						msgError = "DDE transaction failed";
						break;
					case SE_ERR_DDEBUSY:
						msgError = "DDE busy";
						break;
					case SE_ERR_NOASSOC:
						msgError = "No association for file extension";
						break;
					case ERROR_BAD_FORMAT:
						msgError = "Invalid EXE file or error in EXE image";
						break;
					default:
						msgError = "Unknown error";
						break;
				}
					
				return false;
			}
				
			return true;
		}
			
	}
		
}
