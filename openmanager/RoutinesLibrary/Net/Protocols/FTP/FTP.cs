// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Net;
using System.IO;
using System.Text;


    namespace RoutinesLibrary.Net.Protocols.FTP
    {

        /// <summary>
        /// This class provides FTP communication
        /// </summary>
        public class FTP
        {

            /// <summary>
            /// Download file
            /// </summary>
            /// <param name="sFTPFileUrl">File to download</param>
            /// <param name="sFTPUser">FTP user access credentials</param>
            /// <param name="sFTPPwd">FTP password access credentials</param>
            /// <param name="sLocalFileUrl">Path to store the downloaded file</param>
            /// <returns>True if the download operation was succesful</returns>
            public bool DownloadFileFTP(string sFTPFileUrl, string sFTPUser, string sFTPPwd, string sLocalFileUrl)
            {
                bool bOk = false;

                try
                {
                    FtpWebRequest FtpRequest = (FtpWebRequest)(FtpWebRequest.Create(sFTPFileUrl));
                    NetworkCredential cr = new NetworkCredential(sFTPUser, sFTPPwd);
                    FtpRequest.Credentials = cr;
                    FtpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                    FtpRequest.UseBinary = true;

                    FtpWebResponse FtpResponse = (FtpWebResponse)(FtpRequest.GetResponse());
                    Stream stream = FtpResponse.GetResponseStream();
                    byte[] buffer = new byte[2049];
                    FileStream fs = new FileStream(sLocalFileUrl, FileMode.Create);
                    int readCount = stream.Read(buffer, 0, buffer.Length);

                    while (readCount > 0)
                    {
                        fs.Write(buffer, 0, readCount);
                        readCount = stream.Read(buffer, 0, buffer.Length);
                    }

                    fs.Close();
                    bOk = File.Exists(sLocalFileUrl);
                }
                catch (Exception ex)
                {
                    throw (new Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " - Error: " + ex.Message));
                }

                return bOk;
            }

            /// <summary>
            /// Updaload file
            /// </summary>
            /// <param name="sFTPFileUrl">File name to store in the remote server</param>
            /// <param name="sFTPUser">FTP user access credentials</param>
            /// <param name="sFTPPwd">FTP password access credentials</param>
            /// <param name="sLocalFileUrl">File path to upload</param>
            /// <returns>True if the upload operation was succesful</returns>
            public bool UploadFileFTP(string sFTPFileUrl, string sFTPUser, string sFTPPwd, string sLocalFileUrl)
            {
                bool bOk = false;

                if (File.Exists(sLocalFileUrl))
                {
                    FtpWebRequest FtpRequest = (FtpWebRequest)(FtpWebRequest.Create(sFTPFileUrl));
                    NetworkCredential cr = new NetworkCredential(sFTPUser, sFTPPwd);
                    FtpRequest.Credentials = cr;
                    FtpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                    StreamReader reader = new StreamReader(sLocalFileUrl);
                    byte[] res = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                    reader.Close();

                    try
                    {
                        Stream requestStream = FtpRequest.GetRequestStream();
                        requestStream.Write(res, 0, res.Length);
                        requestStream.Close();

                        bOk = true;
                    }
                    catch (Exception ex)
                    {
                        throw (new Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " - Error: " + ex.Message));
                    }
                }

                return bOk;
            }

        }

    }

