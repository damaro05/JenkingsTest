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
    /// Created the DB if not exists and update it making use of the SQL scripts
    /// </summary>
    /// <remarks></remarks>
    public class CDBScripts
    {

        private const string DB_SCRIPTS_FOLDER_PATH = "Data\\DB Scripts\\";
        private const string DB_CREATE_SCRIPT = "CreateScript.sql";
        private const string DB_UPDATE_SCRIPT = "UpdateScript.sql";


        private RoutinesLibrary.Data.DataBase.SQLCompact.SQLCompactConnection m_DBConnection; //Conexión con la base de datos
        private string m_DBScriptsFolderPath; //Path de la carpeta de los scripts de DB
        private string m_FolderNameActVersion; //Nombre de la carpeta de la versión actual


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="DBConnection">Reference to SQL connection</param>
        /// <remarks></remarks>
        public CDBScripts(RoutinesLibrary.Data.DataBase.SQLCompact.SQLCompactConnection DBConnection)
        {
            //Guardamos la conexión con la base de datos
            m_DBConnection = DBConnection;

            //Path de la carpeta de los scripts de DB
            m_DBScriptsFolderPath = Path.Combine((new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath, DB_SCRIPTS_FOLDER_PATH);

            //Nombre de la carpeta de la versión actual. Tiene el formato XXYYZZWW
            string[] aSwVersion = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.Version.ToString().Split('.');
            m_FolderNameActVersion = "";
            foreach (string numSwVersion in aSwVersion)
            {
                if (numSwVersion.Length == 1)
                {
                    m_FolderNameActVersion += "0";
                }
                m_FolderNameActVersion += numSwVersion;
            }
        }

        /// <summary>
        /// Create the DB and execute the SQL scripts depending of the actual software version
        /// </summary>
        /// <returns>True if the DB is created and the scripts executed</returns>
        /// <remarks></remarks>
        public bool CreateDataBase()
        {
            bool bOk = false;

            m_DBConnection.CreateDB();

            //leer directorio según versión y ejecutar script correspondiente
            string sCreateScript = Path.Combine(m_DBScriptsFolderPath, m_FolderNameActVersion, DB_CREATE_SCRIPT);
            if (File.Exists(sCreateScript))
            {
                bOk = m_DBConnection.ExecuteFileScript(sCreateScript);
            }

            return bOk;
        }

        /// <summary>
        /// Update the DB executing all the SQL scripts from the software version stored in the DB to the application software version
        /// </summary>
        /// <param name="DBActVersion"></param>
        /// <returns>True if all the SQL scripts are executed correctly</returns>
        /// <remarks></remarks>
        public bool UpdateDataBase(string DBActVersion)
        {
            bool bOk = false;

            //Nombre de la carpeta de la versión. Tiene el formato XXYYZZWW
            string[] aSwVersion = DBActVersion.Split('.');
            string dirActVersion = "";
            foreach (string numSwVersion in aSwVersion)
            {
                if (numSwVersion.Length == 1)
                {
                    dirActVersion += "0";
                }
                dirActVersion += numSwVersion;
            }

            if (Directory.Exists(m_DBScriptsFolderPath))
            {

                //Extraemos el nombre de las carpetas
                string[] dirsScripts = Directory.GetDirectories(m_DBScriptsFolderPath);
                for (int i = 0; i <= dirsScripts.Length - 1; i++)
                {
                    string[] aSubDir = dirsScripts[i].Split(char.Parse("\\"));
                    dirsScripts[i] = aSubDir[aSubDir.Length - 1];
                }

                //Ejecuta los scripts de actualización desde la versión actual de base de datos hasta la versión actual de software
                Array.Sort(dirsScripts);
                int indexFirstDir = Array.BinarySearch(dirsScripts, dirActVersion) + 1;

                for (int i = indexFirstDir; i <= dirsScripts.Length - 1; i++)
                {
                    string scriptPath = Path.Combine(m_DBScriptsFolderPath, dirsScripts[i], DB_UPDATE_SCRIPT);
                    if (File.Exists(scriptPath))
                    {
                        bOk = bOk && m_DBConnection.ExecuteFileScript(scriptPath);
                    }
                }
            }

            return bOk;
        }

    }
}
