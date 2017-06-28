// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.Threading;
using Microsoft.VisualBasic;


namespace RoutinesLibrary.IO
{
			
	public class Dir
	{
				
		private const int TIME_WAIT_UNLOCK_FILE_SO = 10 * 1000; //10 segundos. Tiempo de reintentos para que el SO desbloquee un fichero
		private const int TRIES_WAIT_UNLOCK_FILE_SO = 10; //Número de reintentos para que el SO desbloquee un fichero
				
				
		public static bool checkDir(string sDir)
		{
			bool bOk = false;
					
			if (sDir == "")
			{
				return bOk;
			}
					
			try
			{
				if (!Directory.Exists(sDir))
				{
					Directory.CreateDirectory(sDir);
				}
				if (Directory.Exists(sDir))
				{
					bOk = true;
				}
			}
			catch (Exception)
			{
			}
					
			return bOk;
		}
				
		/// <summary>
		/// Copy files from the source folder to destination folder overwriting the content
		/// </summary>
		/// <param name="source">Source folder</param>
		/// <param name="destination">Destination folder</param>
		/// <remarks>
		/// This method check if is posible to override a file and retry it
		/// </remarks>
		public static bool CopyDirectory(string source, string destination)
		{
			bool bOk = true;
					
			try
			{
				//Recursively call the CopyUpdateFiles Method for each Directory
				foreach (string subDir in Directory.GetDirectories(source))
				{
					string[] aSubDir = subDir.Split(char.Parse("\\"));
					string destSubDir = Path.Combine(destination, aSubDir[aSubDir.Length - 1]);
							
					//Creamos el directorio destino si no existe
					if (!Directory.Exists(destSubDir))
					{
						Directory.CreateDirectory(destSubDir);
					}
							
					bOk = bOk && CopyDirectory(subDir, destSubDir);
				}
						
				//Go ahead and copy each file in "source" to the "destination" directory
				foreach (string fileDir in Directory.GetFiles(source))
				{
					int nTries = TRIES_WAIT_UNLOCK_FILE_SO;
							
					//Comprobamos que el SO haya cerrado el archivo para poder sobreescrbirlo
					while (nTries > 0)
					{
						if (!File.IsFileOpen(Path.Combine(destination, Path.GetFileName(fileDir))))
						{
							break;
						}
								
						Thread.Sleep(TIME_WAIT_UNLOCK_FILE_SO);
						nTries--;
					}
							
					FileSystem.FileCopy(Path.Combine(source, Path.GetFileName(fileDir)), Path.Combine(destination, Path.GetFileName(fileDir)));
				}
			}
			catch (Exception)
			{
				bOk = false;
			}
					
			return bOk;
		}
				
	}
			
}
