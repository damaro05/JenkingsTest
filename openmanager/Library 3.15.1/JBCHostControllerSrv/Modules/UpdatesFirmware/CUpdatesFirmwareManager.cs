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
using JBCHostControllerSrv.JBCStationControllerService;
using DataJBC;

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Parse a file with the station firmware version information
    /// </summary>
    /// <remarks>
    ///
    /// ---------------------------------------------------------------------------
    /// -------------------------- Versions firmware file -------------------------
    /// ---------------------------------------------------------------------------
    /// Formato:
    /// MODEL: name
    /// HW: hardware_version
    /// SW: software_version : model_version : protocol_version : file_ftp_location : date [: language]
    /// ---------------------------------------------------------------------------
    ///
    ///
    /// MODEL: PSE
    /// HW: 0012408C
    /// SW: 8886030 : PSE_LED_02 : 02 : 8886030.S19 : 2016/09/15
    ///
    /// </remarks>
    public class CUpdatesFirmwareManager
    {

        private const string SEPARATOR = ":"; //caracter separador de una linea

        private const string MODEL_MARK = "MODEL"; //marca de datos de modelo
        private const int MODEL_LINE_ARGS = 2; //numero de campos en una linea de modelo
        private const string HARDWARE_MARK = "HW"; //marca de datos de hardware
        private const int HARDWARE_LINE_ARGS = 2; //numero de campos en una linea de hardware
        private const string HARDWARE_ALL_VERSION = "ALL"; //marca para incluir todas las versiones de hardware de un modelo
        private const string SOFTWARE_MARK = "SW"; //marca de datos de software
        private const int SOFTWARE_LINE_ARGS = 7; //numero de campos en una linea de software
        private const int SOFTWARE_LINE_ARGS_NO_LANG = 6; //numero de campos en una linea de software sin idioma
        private const string SOFTWARE_DEFAULT_LANGUAGE = "English"; //idioma por defecto (utilizado como key) para las estaciones que no tienen diferentes idiomas

        private const int CHUNK_SIZE = 10 * 1024; //10kb. The buffer size by default is set to 64kb


        //Files to update services and stations
        private CSystemFilesManager m_systemFilesManager;


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="localData">Local data reference</param>
        public CUpdatesFirmwareManager(ref CLocalData localData)
        {
            m_systemFilesManager = new CSystemFilesManager(localData);
        }

        /// <summary>
        /// Release resources
        /// </summary>
        public void Dispose()
        {
            m_systemFilesManager.Dispose();
        }

        /// <summary>
        /// Returns the station latest firmware version given a station model and a hardware version
        /// </summary>
        /// <param name="requestInfoUpdateFirmware">Station model and hardware version information</param>
        /// <returns>Station firmware information</returns>
        public List<dc_FirmwareStation> GetInfoUpdateFirmware(dc_FirmwareStation requestInfoUpdateFirmware)
        {

            //Descargar el archivo de versiones del remote server
            string sFilePath = m_systemFilesManager.DownloadFile(System.Convert.ToString(My.Settings.Default.VersionFirmwareFileName));

            Hashtable htInfoUpdateFirmware = new Hashtable(); //Language -> dc_FirmwareVersion
            Hashtable htLatestFirmwareDate = new Hashtable(); //Language -> Date

            if (File.Exists(sFilePath))
            {

                bool bFoundModel = false;
                bool bFoundHw = false;

                //Decrypt
                byte[] fileReader = File.ReadAllBytes(sFilePath);
                byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
                string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

                //Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10)
                string[] sTextLines = decrypted.Replace("\r", "").Split('\n');

                //Leemos el documento line by line
                foreach (string sTextLine in sTextLines)
                {

                    //Buscar modelo
                    if (!bFoundModel)
                    {

                        //Es una línea de modelo
                        if (sTextLine.IndexOf(MODEL_MARK) == 0)
                        {
                            string[] lineArray = Strings.Split(sTextLine, SEPARATOR, MODEL_LINE_ARGS);

                            //Comprobar el numero de campos de una linea
                            if (lineArray.Length == MODEL_LINE_ARGS)
                            {

                                //Modelo encontrado
                                if (lineArray[1].Trim() == requestInfoUpdateFirmware.model)
                                {
                                    bFoundModel = true;
                                }
                            }
                        }


                        //Buscar hardware
                    }
                    else if (bFoundModel && !bFoundHw)
                    {

                        //Es una línea de hardware
                        if (sTextLine.IndexOf(HARDWARE_MARK) == 0)
                        {
                            string[] lineArray = Strings.Split(sTextLine, SEPARATOR, HARDWARE_LINE_ARGS);

                            //Comprobar el numero de campos de una linea
                            if (lineArray.Length == HARDWARE_LINE_ARGS)
                            {

                                //Hardware encontrado
                                if (lineArray[1].Trim() == requestInfoUpdateFirmware.hardwareVersion || lineArray[1].Trim() == HARDWARE_ALL_VERSION)
                                {
                                    bFoundModel = true;
                                    bFoundHw = true;
                                }
                            }

                            //Si es una línea de modelo salir
                        }
                        else if (sTextLine.IndexOf(MODEL_MARK) == 0)
                        {
                            break;
                        }

                        //Buscar software
                    }
                    else if (bFoundModel && bFoundHw)
                    {

                        //Es una línea de software
                        if (sTextLine.IndexOf(SOFTWARE_MARK) == 0)
                        {
                            string[] lineArray = Strings.Split(sTextLine, SEPARATOR, SOFTWARE_LINE_ARGS);

                            //Comprobar el numero de campos de una linea
                            if (lineArray.Length >= SOFTWARE_LINE_ARGS_NO_LANG)
                            {

                                //Si es una estación multidioma seleccionamos el idioma
                                string language = SOFTWARE_DEFAULT_LANGUAGE;
                                if (lineArray.Length == SOFTWARE_LINE_ARGS)
                                {
                                    language = lineArray[6].Trim();
                                }

                                //Elemento nuevo
                                if (!htInfoUpdateFirmware.Contains(language))
                                {
                                    dc_FirmwareStation newInfoUpdateFirmware = new dc_FirmwareStation();
                                    newInfoUpdateFirmware.stationUUID = requestInfoUpdateFirmware.stationUUID;
                                    newInfoUpdateFirmware.model = requestInfoUpdateFirmware.model;
                                    newInfoUpdateFirmware.hardwareVersion = requestInfoUpdateFirmware.hardwareVersion;
                                    newInfoUpdateFirmware.softwareVersion = lineArray[1].Trim();
                                    newInfoUpdateFirmware.modelVersion = lineArray[2].Trim();
                                    newInfoUpdateFirmware.protocolVersion = lineArray[3].Trim();
                                    newInfoUpdateFirmware.fileName = lineArray[4].Trim();
                                    newInfoUpdateFirmware.language = language;

                                    htInfoUpdateFirmware.Add(language, newInfoUpdateFirmware);
                                    htLatestFirmwareDate.Add(language, lineArray[5].Trim());

                                    //Elemento existente. Comprobar si la fecha es mas reciente
                                }
                                else if (Convert.ToDateTime((htLatestFirmwareDate[language]).ToString()) < Convert.ToDateTime(lineArray[5].Trim()))
                                {
                                    dc_FirmwareStation newInfoUpdateFirmware = (dc_FirmwareStation)(htInfoUpdateFirmware[language]);
                                    newInfoUpdateFirmware.softwareVersion = lineArray[1].Trim();
                                    newInfoUpdateFirmware.modelVersion = lineArray[2].Trim();
                                    newInfoUpdateFirmware.protocolVersion = lineArray[3].Trim();
                                    newInfoUpdateFirmware.fileName = lineArray[4].Trim();

                                    htInfoUpdateFirmware[language] = newInfoUpdateFirmware;
                                    htLatestFirmwareDate[language] = lineArray[5].Trim();
                                }
                            }

                            //No es una línea de software, salir
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            //Valores a retornar. Convertir de hashtable a list
            List<dc_FirmwareStation> retInfoUpdateFirmware = new List<dc_FirmwareStation>();
            foreach (DictionaryEntry infoUpdateFirmwareEntry in htInfoUpdateFirmware)
            {
                retInfoUpdateFirmware.Add((dc_FirmwareStation)infoUpdateFirmwareEntry.Value);
            }

            return retInfoUpdateFirmware;
        }

        /// <summary>
        /// Gets a part of the update's package
        /// </summary>
        /// <param name="nSequence">Part number of the update package</param>
        /// <param name="urlFirmwareSw">Update update file path</param>
        /// <returns>Update package data</returns>
        public dc_UpdateFirmware GetFileUpdateFirmware(int nSequence, string urlFirmwareSw)
        {

            dc_UpdateFirmware updateFirmware = new dc_UpdateFirmware();
            updateFirmware.sequence = -1;
            byte[] bytes = new byte[1];

            string sLocalFileUrl = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                    "JBC\\JBC Host Controller Service", urlFirmwareSw);

            try
            {
                //Descargar el archivo de actualizaciones del remote server
                if (!File.Exists(sLocalFileUrl))
                {
                    m_systemFilesManager.DownloadFile(urlFirmwareSw);
                }

                int count = 0;

                FileStream fileStream = new FileStream(sLocalFileUrl, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                long seekPos = binaryReader.BaseStream.Seek((nSequence - 1) * CHUNK_SIZE, SeekOrigin.Begin);

                bytes = binaryReader.ReadBytes(CHUNK_SIZE);
                count = bytes.Length;

                fileStream.Close();

                updateFirmware.final = count < CHUNK_SIZE;
                updateFirmware.sequence = nSequence;
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + ". Error: " + ex.Message.ToString());
            }

            updateFirmware.bytes = bytes;

            return updateFirmware;
        }

    }
}
