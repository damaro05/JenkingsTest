// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.ServiceProcess;
using System.ServiceModel;
using System.Threading;
//using JBCUpdaterSrv.dc_EnumConstJBC;
using JBC_Connect;


namespace JBCUpdaterSrv
{
    /// <summary>
    /// This class is responsible for receiving the update packages and reinstall a windows service
    /// </summary>
    /// <remarks></remarks>
    public class JBCUpdaterService : IJBCUpdaterService
    {

        private const int TIME_WAIT_UNLOCK_FILE_SO = 10 * 1000; //10 segundos. Tiempo de reintentos para que el SO desbloquee un fichero
        private const int TRIES_WAIT_UNLOCK_FILE_SO = 10; //Número de reintentos para que el SO desbloquee un fichero
        private const int TIME_WAIT_STOP_SERVICE_SO = 3 * 1000; //3 segundos. Tiempo de reintentos para que el SO actualice el estado de un servicio
        private const int TRIES_WAIT_STOP_SERVICE_SO = 10; //Número de reintentos para que el SO actualice el estado de un servicio

        RoutinesLibrary.Services.WindowsServiceManager m_serviceManager = new RoutinesLibrary.Services.WindowsServiceManager();


        /// <summary>
        /// Class constructor
        /// </summary>
        public JBCUpdaterService()
        {
            LoggerModule.logger.Info("Se crea una instancia de JBCUpdaterService");
        }

        /// <summary>
        /// Receives a packet of bytes and writes it into the update file
        /// </summary>
        /// <param name="nSequence">Packet sequence number</param>
        /// <param name="bytes">Data bytes</param>
        /// <returns>Number of stored sequence, -1 if an error occurred</returns>
        public int ReceiveFile(int nSequence, byte[] bytes)
        {

            int nReturn = nSequence;

            try
            {
                string sTempDir = Path.Combine(Path.GetTempPath(), System.Convert.ToString(My.Settings.Default.TempDirName));
                string sTempFile = Path.Combine(sTempDir, System.Convert.ToString(My.Settings.Default.AppCompressFileName));

                //Si es el primer byte eliminamos los archivos de antiguas actualizaciones
                if (nSequence == 1)
                {
                    if (Directory.Exists(sTempDir))
                    {
                        (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DeleteDirectory(sTempDir, Microsoft.VisualBasic.FileIO.DeleteDirectoryOption.DeleteAllContents);
                    }
                }

                //Creamos una carpeta temporal para descomprimir las actualizaciones
                if (!Directory.Exists(sTempDir))
                {
                    Directory.CreateDirectory(sTempDir);
                }

                (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllBytes(sTempFile, bytes, true);
            }
            catch (Exception ex)
            {
                nReturn = -1;
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }

            return nReturn;
        }

        /// <summary>
        /// Run a system update
        /// </summary>
        public void InitUpdate()
        {

            //Si se esta actualizando rechazar las peticiones a esta rutina
            if (GetUpdateState() == dc_EnumConstJBC.dc_UpdateState.Updating)
            {
                return;
            }

            SetUpdateState(dc_EnumConstJBC.dc_UpdateState.Updating);
            bool bOk = false;

            try
            {
                bOk = UncompressUpdateFile();
                if (bOk)
                {
                    string sServiceName = "";
                    string sAppPath = "";
                    string sAppFileName = "";

                    bOk = GetUpdateInfo(ref sServiceName, ref sAppPath, ref sAppFileName);
                    if (bOk)
                    {

                        string sVolume = sAppPath.Substring(0, 2);

                        //
                        //Eliminar el servicio antiguo
                        //
                        if (m_serviceManager.ServiceExists(sServiceName))
                        {
                            int nTries = TRIES_WAIT_STOP_SERVICE_SO;

                            //Intentamos parar el servicio varias veces por si el SO tiene un retraso en la actualización del estado
                            while (m_serviceManager.StatusCheck(sServiceName, ServiceControllerStatus.Running) && nTries > 0)
                            {
                                bOk = System.Convert.ToBoolean(m_serviceManager.StopService(sServiceName));
                                if (bOk)
                                {
                                    break;
                                }

                                Thread.Sleep(TIME_WAIT_STOP_SERVICE_SO);
                                nTries--;
                            }

                            //No comprobamos que el servicio se haya parado, ya que la información del estado que devuelve el SO puede ser errónea
                            bOk = System.Convert.ToBoolean(m_serviceManager.UninstallService(sVolume, sAppPath, sAppFileName));
                        }

                        //
                        //Instalar el nuevo servicio
                        //
                        if (bOk)
                        {

                            //Copiamos los archivos de la actualización
                            bOk = RoutinesLibrary.IO.Dir.CopyDirectory(Path.Combine(Path.GetTempPath(), System.Convert.ToString(My.Settings.Default.TempDirName)), sAppPath);

                            if (bOk)
                            {
                                bOk = System.Convert.ToBoolean(m_serviceManager.InstallService(sVolume, sAppPath, sAppFileName));
                                if (bOk)
                                {
                                    bOk = System.Convert.ToBoolean(m_serviceManager.StartService(sServiceName));
                                }
                            }
                        }
                        else
                        {
                            LoggerModule.logger.Warn(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". No se va a instalar el nuevo servicio: " + sServiceName + ". No se paró o no se desinstaló correctamente.");
                        }
                    }
                }

                if (bOk)
                {
                    SetUpdateState(dc_EnumConstJBC.dc_UpdateState.Finished);
                }
                else
                {
                    SetUpdateState(dc_EnumConstJBC.dc_UpdateState.Failed);
                }

            }
            catch (Exception ex)
            {
                SetUpdateState(dc_EnumConstJBC.dc_UpdateState.Failed);
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message + ". InnerException: " + ex.InnerException.Message);
            }

            //Borramos los archivos temporales
            (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DeleteDirectory(Path.Combine(Path.GetTempPath(), System.Convert.ToString(My.Settings.Default.TempDirName)), Microsoft.VisualBasic.FileIO.DeleteDirectoryOption.DeleteAllContents);
        }

        /// <summary>
        /// Obtains information of the update file
        /// </summary>
        /// <param name="sServiceName">Service name</param>
        /// <param name="sAppPath">Application folder path</param>
        /// <param name="sAppFileName">Name of the application executable</param>
        /// <returns>True if there is a folder of the application to update</returns>
        /// <remarks>
        /// Example info file:
        ///     JBCStationControllerService
        ///     JBC Station Controller Service
        ///     JBCStationControllerSrv.exe
        /// </remarks>
        private bool GetUpdateInfo(ref string sServiceName, ref string sAppPath, ref string sAppFileName)
        {

            bool bOk = false;
            string sInfoFile = Path.Combine(Path.GetTempPath(), System.Convert.ToString(My.Settings.Default.TempDirName), System.Convert.ToString(My.Settings.Default.UpdateInfoFile));
            string sDirName = "";

            try
            {
                if (File.Exists(sInfoFile))
                {

                    StreamReader objReader = new StreamReader(sInfoFile);
                    string sTextLine = "";
                    int nCont = 1;

                    //Leemos el documento line by line
                    while (objReader.Peek() != -1)
                    {
                        sTextLine = objReader.ReadLine().Trim();

                        if (nCont == 1)
                        {
                            sServiceName = sTextLine;
                        }
                        else if (nCont == 2)
                        {
                            sDirName = sTextLine;
                        }
                        else if (nCont == 3)
                        {
                            sAppFileName = sTextLine;
                        }
                        else
                        {
                            break;
                        }
                        nCont++;
                    }
                    objReader.Close();

                    //FIXME Buscar otra manera de hacerlo de encontrar la ruta de aplicación
                    if ((new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DirectoryExists(Path.Combine("C:\\Program Files\\JBC", sDirName)))
                    {
                        bOk = true;
                        sAppPath = Path.Combine("C:\\Program Files\\JBC", sDirName);
                    }
                    else if ((new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DirectoryExists(Path.Combine("C:\\Program Files (x86)\\JBC", sDirName)))
                    {
                        bOk = true;
                        sAppPath = Path.Combine("C:\\Program Files (x86)\\JBC", sDirName);
                    }
                    else
                    {
                        LoggerModule.logger.Warn(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". No existe la ruta de aplicación");
                    }

                }
                else
                {
                    LoggerModule.logger.Warn(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". No existe el fichero: " + sInfoFile);
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }

            return bOk;
        }

        /// <summary>
        /// Decompress the update file
        /// </summary>
        /// <returns>True if the operation was successful</returns>
        private bool UncompressUpdateFile()
        {

            bool bOk = false;
            string sTempDir = Path.Combine(Path.GetTempPath(), System.Convert.ToString(My.Settings.Default.TempDirName));
            string sTempFile = Path.Combine(sTempDir, System.Convert.ToString(My.Settings.Default.AppCompressFileName));

            if (File.Exists(sTempFile))
            {
                string sTempCompressFile = Path.ChangeExtension(sTempFile, "tar.gz");

                try
                {
                    File.Move(sTempFile, sTempCompressFile);

                    //Descomprimir .tar.gz
                    Stream inStream = File.OpenRead(sTempCompressFile);
                    Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inStream);
                    ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream);

                    tarArchive.ExtractContents(sTempDir);

                    tarArchive.Close();
                    gzipStream.Close();
                    inStream.Close();

                    //Eliminamos el archivo comprimido
                    if (File.Exists(sTempCompressFile))
                    {
                        File.Delete(sTempCompressFile);
                    }

                    bOk = true;
                }
                catch (Exception ex)
                {
                    LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
                }
            }
            else
            {
                LoggerModule.logger.Warn(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". File doesn't exists: " + sTempFile);
            }

            return bOk;
        }

        #region State Update

        /// <summary>
        /// Reports the status of the update
        /// </summary>
        /// <returns>Update status</returns>
        public dc_EnumConstJBC.dc_UpdateState StateUpdate()
        {
            return GetUpdateState();
        }

        /// <summary>
        /// Save the update status
        /// </summary>
        /// <param name="state">New update status</param>
        private void SetUpdateState(dc_EnumConstJBC.dc_UpdateState state)
        {

            string sState = "";
            if (state == dc_EnumConstJBC.dc_UpdateState.Failed)
            {
                sState = dc_EnumConstJBC.dc_UpdateState.Failed.ToString();
            }
            else if (state == dc_EnumConstJBC.dc_UpdateState.Finished)
            {
                sState = dc_EnumConstJBC.dc_UpdateState.Finished.ToString();
            }
            else
            {
                sState = dc_EnumConstJBC.dc_UpdateState.Updating.ToString();
            }

            //El estado de actualización se guarda en un fichero para permitir la concurrencia
            (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\JBC Updater Service", System.Convert.ToString(My.Settings.Default.UpdateStateFile)), sState, false);
        }

        /// <summary>
        /// Gets the status of the update
        /// </summary>
        /// <returns>Status Update</returns>
        private dc_EnumConstJBC.dc_UpdateState GetUpdateState()
        {

            string sFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\JBC Updater Service", System.Convert.ToString(My.Settings.Default.UpdateStateFile));
            dc_EnumConstJBC.dc_UpdateState state = dc_EnumConstJBC.dc_UpdateState.Finished;

            if (File.Exists(sFile))
            {
                StreamReader objReader = new StreamReader(sFile);
                string sTextLine;

                //Leemos el documento line by line
                while (objReader.Peek() != -1)
                {
                    sTextLine = objReader.ReadLine().Trim();

                    if (sTextLine == dc_EnumConstJBC.dc_UpdateState.Updating.ToString())
                    {
                        state = dc_EnumConstJBC.dc_UpdateState.Updating;
                    }
                    else if (sTextLine == dc_EnumConstJBC.dc_UpdateState.Finished.ToString())
                    {
                        state = dc_EnumConstJBC.dc_UpdateState.Finished;
                    }
                    else //Failed
                    {
                        state = dc_EnumConstJBC.dc_UpdateState.Failed;
                    }
                }
                objReader.Close();
            }

            return state;
        }

        #endregion

    }
}
