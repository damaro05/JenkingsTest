// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

using System.IO;
using System.Threading;
using System.ServiceModel;
using RemoteManager.HostControllerServiceReference;
using Microsoft.VisualBasic.CompilerServices;


namespace RemoteManager
{
    public partial class frmUpdatesReInstall
    {

        private const int PROGRESS_BAR_UPDATE_CHUNK = 300;


        //Communications
        private CComHostController m_comHostController;


        public delegate void cancelUpdatedReInstallEventHandler();
        private cancelUpdatedReInstallEventHandler cancelUpdatedReInstallEvent;

        public event cancelUpdatedReInstallEventHandler cancelUpdatedReInstall
        {
            add
            {
                cancelUpdatedReInstallEvent = (cancelUpdatedReInstallEventHandler)System.Delegate.Combine(cancelUpdatedReInstallEvent, value);
            }
            remove
            {
                cancelUpdatedReInstallEvent = (cancelUpdatedReInstallEventHandler)System.Delegate.Remove(cancelUpdatedReInstallEvent, value);
            }
        }



        public frmUpdatesReInstall(CComHostController comHostController)
        {
            InitializeComponent();
            ReLoadTexts();
            m_comHostController = comHostController;
        }

        public void frmUpdatesReInstall_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            System.Drawing.Rectangle temp_borderForm = this.ClientRectangle;
            CPanelStyle.SetPanelStyle(ref e, ref temp_borderForm);
        }

        public void ReLoadTexts()
        {

            this.labelTitle.Text = Localization.getResStr(Configuration.updatesReInstallTitleId);
            this.labelInformation.Text = Localization.getResStr(Configuration.updatesReInstallInformationId);
            this.labelUpdateStatus.Text = Localization.getResStr(Configuration.updatesReInstallUpdateStatusId);
            this.butUpdate.Text = Localization.getResStr(Configuration.updatesReInstallUpdateId);
            this.butClose.Text = Localization.getResStr(Configuration.updatesReInstallCancelId);
            this.uControlMessage_Error.ReLoadTexts();
        }

        private void ErrorUpdate()
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => ErrorUpdate()));
                return;
            }

            //Ocultar progress bar
            this.labelUpdateStatus.Visible = false;
            this.progressBarUpdate.Visible = false;

            //Habilitar controles
            this.butUpdate.Text = Localization.getResStr(Configuration.updatesReInstallTryAgainId);
            this.butUpdate.Enabled = true;
            this.butClose.Enabled = true;

            //Mostrar mensaje de error
            this.uControlMessage_Error.Visible = true;
            this.uControlMessage_Error.Refresh();
        }

        private void SetProgressBarValue(int value)
        {

            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke((Action)(() => SetProgressBarValue(value)));
                return;
            }

            if (value < PROGRESS_BAR_UPDATE_CHUNK - 1)
            {
                this.progressBarUpdate.Value = value;
                this.progressBarUpdate.Refresh();
            }
        }

        public void butUpdate_Click(System.Object sender, System.EventArgs e)
        {

            //Ocultar mensaje de error
            this.uControlMessage_Error.Visible = false;

            //Set progress bar values
            this.labelUpdateStatus.Visible = true;
            this.progressBarUpdate.Visible = true;
            this.progressBarUpdate.Minimum = 0;
            this.progressBarUpdate.Maximum = PROGRESS_BAR_UPDATE_CHUNK;
            this.progressBarUpdate.Value = 0;
            Refresh();

            //Deshabilitar controles
            this.butUpdate.Enabled = false;
            this.butClose.Enabled = false;

            //Creamos un thread para desbloquear la UI mientras se comunica con el HostController
            Thread WorkerProcessUpdate = new Thread(new System.Threading.ThreadStart(ProcessUpdate));
            WorkerProcessUpdate.IsBackground = true;
            WorkerProcessUpdate.Start();
        }

        public void butClose_Click(System.Object sender, System.EventArgs e)
        {
            if (cancelUpdatedReInstallEvent != null)
                cancelUpdatedReInstallEvent();
        }

        private void ProcessUpdate()
        {

            bool bOk = false;

            try
            {
                bool bContinue = false;
                int nSequence = 1;

                //Carpeta temporal donde guardar la descarga
                string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\Manager", System.Convert.ToString(My.Settings.Default.TempUpdateFolder));

                //Borra la carpeta temporal
                if ((new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DirectoryExists(tempFolder))
                {
                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DeleteDirectory(tempFolder, Microsoft.VisualBasic.FileIO.DeleteDirectoryOption.DeleteAllContents);
                }

                //Crear carpeta temporal
                Directory.CreateDirectory(tempFolder);

                //Download the update
                do
                {
                    //Actualizar progress bar
                    SetProgressBarValue(nSequence);

                    int nTries = 3;
                    bOk = false;
                    dc_UpdateRemoteManager updateRemoteManager = new dc_UpdateRemoteManager();

                    while (nTries > 0 && !bOk)
                    {
                        updateRemoteManager = m_comHostController.GetFileUpdateRemoteManager(nSequence);
                        bOk = updateRemoteManager.sequence == nSequence;
                        bContinue = !updateRemoteManager.final;

                        nTries--;
                    }

                    if (!bOk)
                    {
                        break;
                    }
                    nSequence++;

                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllBytes(Path.Combine(tempFolder, System.Convert.ToString(My.Settings.Default.AppCompressFileName)), updateRemoteManager.bytes, true);
                } while (bContinue);

                if (bOk)
                {
                    bOk = UncompressUpdateFile();

                    //Descomprime la actualización
                    if (bOk)
                    {
                        Interaction.Shell(Directory.GetFiles(tempFolder, "*.exe").First());

                        //Auto cierra la aplicación
                        ProjectData.EndApp();
                    }
                }
            }
            catch (Exception)
            {
                bOk = false;
            }

            if (!bOk)
            {
                ErrorUpdate();
            }
        }

        //@Brief Descomprime y comprueba el archivo de actualización
        //@Return Boolean True si la operación se ha realizado correctamente
        private bool UncompressUpdateFile()
        {

            bool bOk = false;
            string tempFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\Manager", System.Convert.ToString(My.Settings.Default.TempUpdateFolder));
            string filePath = Path.Combine(tempFolder, System.Convert.ToString(My.Settings.Default.AppCompressFileName));

            if (File.Exists(filePath))
            {
                string newFile = Path.ChangeExtension(filePath, "tar.gz");

                try
                {
                    File.Move(filePath, newFile);

                    //Descomprimir .tar.gz
                    Stream inStream = File.OpenRead(newFile);
                    Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(inStream);
                    ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream);

                    tarArchive.ExtractContents(tempFolder);

                    tarArchive.Close();
                    gzipStream.Close();
                    inStream.Close();

                    //Comprobar que existe el .exe
                    bOk = Directory.GetFiles(tempFolder, "*.exe").Length > 0;

                }
                catch (Exception)
                {
                }
            }

            return bOk;
        }

    }
}
