// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports


namespace RoutinesLibrary.IO
{
			
	public class File
	{
				
		public static bool IsFileOpen(string filePath)
		{
			bool rtnvalue = false;
					
			try
			{
				System.IO.FileStream fs = System.IO.File.OpenWrite(filePath);
				fs.Close();
			}
			catch (System.IO.IOException)
			{
				rtnvalue = true;
			}
					
			return rtnvalue;
		}
				
	}
			
}
