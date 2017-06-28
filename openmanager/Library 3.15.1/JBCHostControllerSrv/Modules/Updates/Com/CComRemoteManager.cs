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

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Manage the Remote Manager communication
    /// </summary>
    public class CComRemoteManager
    {

        private const int CHUNK_SIZE = 10 * 1024; //10kb. The buffer size by default is set to 64kb

        /// <summary>
        /// Release resources
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Gets a part of the update's package
        /// </summary>
        /// <param name="nSequence">Part number of the update package</param>
        /// <param name="urlRemoteManagerSw">Update package to send</param>
        /// <returns>Update package data</returns>
        public dc_UpdateRemoteManager GetFileUpdateRemoteManager(int nSequence, string urlRemoteManagerSw)
        {

            dc_UpdateRemoteManager updateRemoteManager = new dc_UpdateRemoteManager();
            byte[] bytes = new byte[1];

            try
            {
                int count = 0;

                FileStream fileStream = new FileStream(urlRemoteManagerSw, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                long seekPos = binaryReader.BaseStream.Seek((nSequence - 1) * CHUNK_SIZE, SeekOrigin.Begin);

                bytes = binaryReader.ReadBytes(CHUNK_SIZE);
                count = bytes.Length;

                fileStream.Close();

                updateRemoteManager.final = count < CHUNK_SIZE;
                updateRemoteManager.sequence = nSequence;
            }
            catch (Exception ex)
            {
                updateRemoteManager.sequence = -1;
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + ". Error: " + ex.Message.ToString());
            }

            updateRemoteManager.bytes = bytes;

            return updateRemoteManager;
        }

    }
}
