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
    /// Manage files used by the application downloading them of JBC's server or specifying a user folder
    /// </summary>
    public class CSystemFilesManager
    {

        private CLocalData m_localData;
        private CComRemoteServer m_comRemoteServer = new CComRemoteServer();


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="localData">Local data reference</param>
        public CSystemFilesManager(CLocalData localData)
        {
            m_localData = localData;
        }

        /// <summary>
        /// Deletes all resources
        /// </summary>
        public void Dispose()
        {
            m_localData = null;
            m_comRemoteServer.Dispose();
        }

        /// <summary>
        /// Download a file from JBC's server or from a user folder
        /// </summary>
        /// <param name="fileName">File name to download</param>
        /// <returns>Full path of the downloaded file</returns>
        public string DownloadFile(string fileName)
        {
            var sReturn = "";
            string downloadFolder = m_localData.GetSystemFilesFolderLocation();

            //Download from Remote Server
            if (m_localData.IsAvailableRemoteServerDownload())
            {
                sReturn = m_comRemoteServer.DownloadFileFromRemoteServer(fileName, downloadFolder);

                //Local file
            }
            else
            {
                sReturn = Path.Combine(downloadFolder, fileName);
            }

            return sReturn;
        }

    }
}
