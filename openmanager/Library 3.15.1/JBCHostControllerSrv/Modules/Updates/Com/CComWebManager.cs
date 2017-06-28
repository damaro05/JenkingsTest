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
using System.Net;
using System.Threading;

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Manage Web Manager communication
    /// </summary>
    public class CComWebManager
    {

        private const int CHUNK_SIZE = 10 * 1024; //10kb. The buffer size by default is set to 64kb
        private const int WAIT_REQUEST_START_UPDATE = 2000; //Wait 2 seconds before send request


        private Thread m_ThreadRequestStartUpdate;


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
        /// <param name="urlWebManagerSw">Update package to send</param>
        /// <param name="ip">Web Manager IP to send the update</param>
        /// <returns>Update package data</returns>
        public dc_UpdateWebManager GetFileUpdateWebManager(int nSequence, string urlWebManagerSw, string ip)
        {

            dc_UpdateWebManager updateWebManager = new dc_UpdateWebManager();
            byte[] bytes = new byte[1];

            try
            {
                int count = 0;

                FileStream fileStream = new FileStream(urlWebManagerSw, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                long seekPos = binaryReader.BaseStream.Seek((nSequence - 1) * CHUNK_SIZE, SeekOrigin.Begin);

                bytes = binaryReader.ReadBytes(CHUNK_SIZE);
                count = bytes.Length;

                fileStream.Close();

                updateWebManager.final = count < CHUNK_SIZE;
                updateWebManager.sequence = nSequence;
            }
            catch (Exception ex)
            {
                updateWebManager.sequence = -1;
                LoggerModule.logger.Error("CComWebManager::GetFileUpdateWebManager . Error: " + ex.Message.ToString());
            }

            updateWebManager.bytes = bytes;

            //Send start update process
            if (updateWebManager.final)
            {
                StartUpdate(ip);
            }

            return updateWebManager;
        }

        /// <summary>
        /// Start the Web Manager update process in a thread
        /// </summary>
        /// <param name="ip">Web Manager IP</param>
        public void StartUpdate(string ip)
        {
            m_ThreadRequestStartUpdate = new Thread(new System.Threading.ParameterizedThreadStart(RequestStartUpdate));
            m_ThreadRequestStartUpdate.IsBackground = true;
            m_ThreadRequestStartUpdate.Start(ip);
        }

        /// <summary>
        /// Start the Web Manager update process
        /// </summary>
        /// <param name="parameters">Web Manager IP</param>
        private void RequestStartUpdate(object parameters)
        {
            Thread.Sleep(WAIT_REQUEST_START_UPDATE);

            var ip = System.Convert.ToString(parameters);
            UriBuilder uri = new UriBuilder("http", ip, 80, System.Convert.ToString(My.Settings.Default.WebManagerUpdateUrl));

            WebRequest wrGETURL = WebRequest.Create(uri.ToString());
            wrGETURL.GetResponse().GetResponseStream();
        }

    }
}
