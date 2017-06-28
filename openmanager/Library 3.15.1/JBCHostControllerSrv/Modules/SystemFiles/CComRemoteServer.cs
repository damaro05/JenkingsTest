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
    /// This class provides connection to the JBC's FTP server
    /// </summary>
    /// <remarks></remarks>
    public class CComRemoteServer
    {

        private string m_FTPUser; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private string m_FTPPassword; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private RoutinesLibrary.Net.Protocols.FTP.FTP m_ftp;


        /// <summary>
        /// Class constructor
        /// </summary>
        public CComRemoteServer()
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            m_FTPUser = System.Convert.ToString(My.Settings.Default.FTPUser);
            m_FTPPassword = System.Convert.ToString(My.Settings.Default.FTPPwd);

            m_ftp = new RoutinesLibrary.Net.Protocols.FTP.FTP();
        }

        /// <summary>
        /// Deletes all resources
        /// </summary>
        public void Dispose()
        {
            m_ftp = null;
        }

        /// <summary>
        /// Download a file from JBC's server
        /// </summary>
        /// <param name="fileName">File name to download</param>
        /// <param name="downloadFolder">Folder path to download the file</param>
        /// <returns>Full path of the downloaded file</returns>
        public string DownloadFileFromRemoteServer(string fileName, string downloadFolder)
        {
            var sReturn = "";

            string sFTPFileUrl = Path.Combine(System.Convert.ToString(My.Settings.Default.FTPUrl), fileName);
            sFTPFileUrl = sFTPFileUrl.Replace("\\", "/");
            string sLocalFileUrl = Path.Combine(downloadFolder, fileName);

            try
            {
                if (m_ftp.DownloadFileFTP(sFTPFileUrl, m_FTPUser, m_FTPPassword, sLocalFileUrl))
                {
                    sReturn = sLocalFileUrl;
                }
                else
                {
                    LoggerModule.logger.Warn(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + "Error: Can't download file: " + fileName);
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + ". Error: " + ex.Message + " - Can't download file: " + fileName);
            }

            return sReturn;
        }

    }
}
