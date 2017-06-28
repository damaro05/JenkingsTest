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

using System.IO;
using System.Security.Cryptography;




namespace JBC_Connect
{
	namespace RoutinesLibrary.Security
	{
		
		public class SHA256
		{
			
			//@Brief Comprueba el hash de un archivom con SHA256
			//@Param[in] checksum Valor del hash a comparar
			//@Param[in] sFilePath Fichero a comprobar el hash
			//@Return Boolean True si la validación es correcta
			public static bool CheckFile_SHA256(string checksum, string sFilePath)
			{
				
				bool bOK = false;
				
				if (System.IO.File.Exists(sFilePath))
				{
					try
					{
						System.Security.Cryptography.SHA256 mySHA256 = SHA256Managed.Create();
						byte[] hashValue = null;
						FileInfo fInfo = new FileInfo(sFilePath);
						FileStream fileStream = fInfo.Open(FileMode.Open);
						
						fileStream.Position = 0;
						hashValue = mySHA256.ComputeHash(fileStream);
						fileStream.Close();
						
						bOK = checksum == RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToString(hashValue);
					}
					catch (Exception ex)
					{
						throw (new Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error: " + ex.Message));
					}
				}
				
				return bOK;
			}
			
		}
		
	}
	
}
