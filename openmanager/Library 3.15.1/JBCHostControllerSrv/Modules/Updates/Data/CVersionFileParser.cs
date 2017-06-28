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
using DataJBC;

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Parse a file with the services version information
    /// </summary>
    /// <remarks>
    ///
    /// -------------------------------------------------------
    /// -------------------- Versions file --------------------
    /// -------------------------------------------------------
    /// Formato:
    /// version(X.Y.Z.W) : date(yyyy/MM/dd) : url
    /// -------------------------------------------------------
    ///
    ///
    /// #stationcontroller
    /// 3.15.1.5 : 2014/01/01 : 444005.jbc
    /// 3.15.1.4 : 2015/01/01 : 444004.jbc
    ///
    /// #remotemanager
    /// 3.15.1.5 : 2014/01/01 : 333005.jbc
    /// 3.15.1.4 : 2015/01/01 : 333004.jbc
    ///
    /// #hostcontroller
    /// 3.15.1.5 : 2014/01/01 : 222005.jbc
    /// 3.15.1.4 : 2015/01/01 : 222004.jbc
    ///
    /// </remarks>
    public class CVersionFileParser
    {

        private const string STATION_CONTROLLER_MARK = "#stationcontroller"; //marca para empezar un bloque de datos del StationController
        private const string REMOTE_MANAGER_MARK = "#remotemanager"; //marca para empezar un bloque de datos del RemoteManager
        private const string HOST_CONTROLLER_MARK = "#hostcontroller"; //marca para empezar un bloque de datos del HostController
        private const string WEB_MANAGER_MARK = "#webmanager"; //marca para empezar un bloque de datos del WebManager
        private const string SEPARATOR = ":"; //caracter separador de una linea
        private const int LINE_ARGS = 3; //numero de campos en una linea


        private CLocalData m_localData;


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="localData">Local data reference</param>
        public CVersionFileParser(CLocalData localData)
        {
            m_localData = localData;
        }

        /// <summary>
        /// Release resources
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Get the last software version from info file
        /// </summary>
        /// <returns>StationController, RemoteManager, HostController and WebManager update information</returns>
        public stSwVersion GetInfoLastSwVersion()
        {

            stSwVersion swVersion = new stSwVersion();
            swVersion.stationControllerSwVersion = "";
            swVersion.stationControllerSwDate = DateTime.Parse("1/1/1900");
            swVersion.stationControllerSwUrl = "";
            swVersion.remoteManagerSwVersion = "";
            swVersion.remoteManagerSwDate = DateTime.Parse("1/1/1900");
            swVersion.remoteManagerSwUrl = "";
            swVersion.hostControllerSwVersion = "";
            swVersion.hostControllerSwDate = DateTime.Parse("1/1/1900");
            swVersion.hostControllerSwUrl = "";
            swVersion.webManagerSwVersion = "";
            swVersion.webManagerSwDate = DateTime.Parse("1/1/1900");
            swVersion.webManagerSwUrl = "";


            if (File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName))))
            {

                bool bBlockStationController = false; //para saber si estamos en un bloque de datos del StationController
                bool bBlockRemoteManager = false; //para saber si estamos en un bloque de datos del RemoteManager
                bool bBlockHostController = false; //para saber si estamos en un bloque de datos del HostController
                bool bBlockWebManager = false; //para saber si estamos en un bloque de datos del WebManager

                //Decrypt
                byte[] fileReader = File.ReadAllBytes(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName)));
                byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
                string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

                //Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10)
                string[] sTextLines = decrypted.Replace("\r", "").Split('\n');

                //Leemos el documento line by line
                foreach (string sTextLine in sTextLines)
                {

                    //Empieza un bloque de StationController
                    if (sTextLine == STATION_CONTROLLER_MARK)
                    {
                        bBlockStationController = true;
                        bBlockRemoteManager = false;
                        bBlockHostController = false;
                        bBlockWebManager = false;

                        //Empieza un bloque de RemoteManager
                    }
                    else if (sTextLine == REMOTE_MANAGER_MARK)
                    {
                        bBlockStationController = false;
                        bBlockRemoteManager = true;
                        bBlockHostController = false;
                        bBlockWebManager = false;

                        //Empieza un bloque de HostController
                    }
                    else if (sTextLine == HOST_CONTROLLER_MARK)
                    {
                        bBlockStationController = false;
                        bBlockRemoteManager = false;
                        bBlockHostController = true;
                        bBlockWebManager = false;

                        //Empieza un bloque de WebManager
                    }
                    else if (sTextLine == WEB_MANAGER_MARK)
                    {
                        bBlockStationController = false;
                        bBlockRemoteManager = false;
                        bBlockHostController = false;
                        bBlockWebManager = true;

                        //Estamos leyendo un bloque de datos
                    }
                    else if (bBlockStationController || bBlockRemoteManager || bBlockHostController || bBlockWebManager)
                    {

                        string[] lineArray = Strings.Split(sTextLine, SEPARATOR, LINE_ARGS);

                        //Comprobar el numero de campos de una linea
                        if (lineArray.Length == LINE_ARGS)
                        {
                            DateTime candidateDate = default(DateTime);

                            if (DateTime.TryParseExact(lineArray[1].Trim(), "yyyy/MM/dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out candidateDate))
                            {

                                //Pertenece a un StationController
                                if (bBlockStationController)
                                {

                                    //La nueva version leida es mas reciente
                                    if (candidateDate > swVersion.stationControllerSwDate)
                                    {
                                        swVersion.stationControllerSwVersion = lineArray[0].Trim();
                                        swVersion.stationControllerSwDate = candidateDate;
                                        swVersion.stationControllerSwUrl = lineArray[2].Trim();
                                    }

                                    //Pertenece a un RemoteManager
                                }
                                else if (bBlockRemoteManager)
                                {

                                    //La nueva version leida es mas reciente
                                    if (candidateDate > swVersion.remoteManagerSwDate)
                                    {
                                        swVersion.remoteManagerSwVersion = lineArray[0].Trim();
                                        swVersion.remoteManagerSwDate = candidateDate;
                                        swVersion.remoteManagerSwUrl = lineArray[2].Trim();
                                    }

                                    //Pertenece a un HostController
                                }
                                else if (bBlockHostController)
                                {

                                    //La nueva version leida es mas reciente
                                    if (candidateDate > swVersion.hostControllerSwDate)
                                    {
                                        swVersion.hostControllerSwVersion = lineArray[0].Trim();
                                        swVersion.hostControllerSwDate = candidateDate;
                                        swVersion.hostControllerSwUrl = lineArray[2].Trim();
                                    }

                                    //Pertenece a un WebManager
                                }
                                else if (bBlockWebManager)
                                {

                                    //La nueva version leida es mas reciente
                                    if (candidateDate > swVersion.webManagerSwDate)
                                    {
                                        swVersion.webManagerSwVersion = lineArray[0].Trim();
                                        swVersion.webManagerSwDate = candidateDate;
                                        swVersion.webManagerSwUrl = lineArray[2].Trim();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return swVersion;
        }

        /// <summary>
        /// Get the StationController url given a version
        /// </summary>
        /// <param name="swVersion">StationController version</param>
        /// <returns>StationController sw url</returns>
        public string GetStationControllerSwUrl(string swVersion)
        {

            swVersion = swVersion.Trim();
            string sStationControllerSwUrl = "";

            if (File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName))))
            {
                bool bBlockStationController = false; //para saber si estamos en un bloque de datos del StationController

                //Decrypt
                byte[] fileReader = File.ReadAllBytes(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName)));
                byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
                string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

                //Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10)
                string[] sTextLines = decrypted.Replace("\r", "").Split('\n');

                //Leemos el documento line by line
                foreach (string sTextLine in sTextLines)
                {

                    //Empieza un bloque de StationController
                    if (sTextLine == STATION_CONTROLLER_MARK)
                    {
                        bBlockStationController = true;

                        //Empieza un bloque de RemoteManager
                    }
                    else if (sTextLine == REMOTE_MANAGER_MARK)
                    {
                        bBlockStationController = false;

                    }
                    else if (sTextLine == HOST_CONTROLLER_MARK)
                    {
                        bBlockStationController = false;

                        //Empieza un bloque de WebManager
                    }
                    else if (sTextLine == WEB_MANAGER_MARK)
                    {
                        bBlockStationController = false;

                        //Estamos leyendo un bloque de StationController
                    }
                    else if (bBlockStationController)
                    {
                        string[] lineArray = Strings.Split(sTextLine, SEPARATOR, LINE_ARGS);

                        //Comprobar el numero de campos de una linea
                        if (lineArray.Length == LINE_ARGS)
                        {
                            if (swVersion == lineArray[0].Trim())
                            {
                                sStationControllerSwUrl = lineArray[2].Trim();
                            }
                        }
                    }
                }
            }

            return sStationControllerSwUrl;
        }

        /// <summary>
        /// Get the RemoteManager url given a version
        /// </summary>
        /// <param name="swVersion">RemoteManager version</param>
        /// <returns>RemoteManager sw url</returns>
        public string GetRemoteManagerSwUrl(string swVersion)
        {

            swVersion = swVersion.Trim();
            string sRemoteManagerSwUrl = "";

            if (File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName))))
            {
                bool bBlockRemoteManager = false; //para saber si estamos en un bloque de datos del RemoteManager

                //Decrypt
                byte[] fileReader = File.ReadAllBytes(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName)));
                byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
                string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

                //Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10)
                string[] sTextLines = decrypted.Replace("\r", "").Split('\n');

                //Leemos el documento line by line
                foreach (string sTextLine in sTextLines)
                {

                    //Empieza un bloque de Station Controller
                    if (sTextLine == STATION_CONTROLLER_MARK)
                    {
                        bBlockRemoteManager = false;

                        //Empieza un bloque de RemoteManager
                    }
                    else if (sTextLine == REMOTE_MANAGER_MARK)
                    {
                        bBlockRemoteManager = true;

                    }
                    else if (sTextLine == HOST_CONTROLLER_MARK)
                    {
                        bBlockRemoteManager = false;

                        //Empieza un bloque de WebManager
                    }
                    else if (sTextLine == WEB_MANAGER_MARK)
                    {
                        bBlockRemoteManager = false;

                        //Estamos leyendo un bloque de RemoteManager
                    }
                    else if (bBlockRemoteManager)
                    {
                        string[] lineArray = Strings.Split(sTextLine, SEPARATOR, LINE_ARGS);

                        //Comprobar el numero de campos de una linea
                        if (lineArray.Length == LINE_ARGS)
                        {
                            if (swVersion == lineArray[0].Trim())
                            {
                                sRemoteManagerSwUrl = lineArray[2].Trim();
                            }
                        }
                    }
                }
            }

            return sRemoteManagerSwUrl;
        }

        /// <summary>
        /// Get the HostController url given a version
        /// </summary>
        /// <param name="swVersion">HostController version</param>
        /// <returns>HostController sw url</returns>
        public string GetHostControllerSwUrl(string swVersion)
        {

            swVersion = swVersion.Trim();
            string sHostControllerSwUrl = "";

            if (File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName))))
            {
                bool bBlockHostController = false; //para saber si estamos en un bloque de datos del HostController

                //Decrypt
                byte[] fileReader = File.ReadAllBytes(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName)));
                byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
                string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

                //Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10)
                string[] sTextLines = decrypted.Replace("\r", "").Split('\n');

                //Leemos el documento line by line
                foreach (string sTextLine in sTextLines)
                {

                    //Empieza un bloque de StationController
                    if (sTextLine == STATION_CONTROLLER_MARK)
                    {
                        bBlockHostController = false;

                        //Empieza un bloque de RemoteManager
                    }
                    else if (sTextLine == REMOTE_MANAGER_MARK)
                    {
                        bBlockHostController = false;

                    }
                    else if (sTextLine == HOST_CONTROLLER_MARK)
                    {
                        bBlockHostController = true;

                        //Empieza un bloque de WebManager
                    }
                    else if (sTextLine == WEB_MANAGER_MARK)
                    {
                        bBlockHostController = false;

                        //Estamos leyendo un bloque de HostController
                    }
                    else if (bBlockHostController)
                    {
                        string[] lineArray = Strings.Split(sTextLine, SEPARATOR, LINE_ARGS);

                        //Comprobar el numero de campos de una linea
                        if (lineArray.Length == LINE_ARGS)
                        {
                            if (swVersion == lineArray[0].Trim())
                            {
                                sHostControllerSwUrl = lineArray[2].Trim();
                            }
                        }
                    }
                }
            }

            return sHostControllerSwUrl;
        }

        /// <summary>
        /// Get the WebManager url given a version
        /// </summary>
        /// <param name="swVersion">WebManager version</param>
        /// <returns>WebManager sw url</returns>
        public string GetWebManagerSwUrl(string swVersion)
        {

            swVersion = swVersion.Trim();
            string sWebManagerSwUrl = "";

            if (File.Exists(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName))))
            {
                bool bBlockWebManager = false; //para saber si estamos en un bloque de datos del WebManager

                //Decrypt
                byte[] fileReader = File.ReadAllBytes(Path.Combine(m_localData.GetSystemFilesFolderLocation(), System.Convert.ToString(My.Settings.Default.VersionFileName)));
                byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
                string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

                //Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10)
                string[] sTextLines = decrypted.Replace("\r", "").Split('\n');

                //Leemos el documento line by line
                foreach (string sTextLine in sTextLines)
                {

                    //Empieza un bloque de Station Controller
                    if (sTextLine == STATION_CONTROLLER_MARK)
                    {
                        bBlockWebManager = false;

                        //Empieza un bloque de RemoteManager
                    }
                    else if (sTextLine == REMOTE_MANAGER_MARK)
                    {
                        bBlockWebManager = false;

                    }
                    else if (sTextLine == HOST_CONTROLLER_MARK)
                    {
                        bBlockWebManager = false;

                        //Empieza un bloque de WebManager
                    }
                    else if (sTextLine == WEB_MANAGER_MARK)
                    {
                        bBlockWebManager = true;

                        //Estamos leyendo un bloque de WebManager
                    }
                    else if (bBlockWebManager)
                    {
                        string[] lineArray = Strings.Split(sTextLine, SEPARATOR, LINE_ARGS);

                        //Comprobar el numero de campos de una linea
                        if (lineArray.Length == LINE_ARGS)
                        {
                            if (swVersion == lineArray[0].Trim())
                            {
                                sWebManagerSwUrl = lineArray[2].Trim();
                            }
                        }
                    }
                }
            }

            return sWebManagerSwUrl;
        }

    }
}
