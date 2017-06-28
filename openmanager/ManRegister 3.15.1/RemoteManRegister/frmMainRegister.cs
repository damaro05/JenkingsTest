// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using JBC_ConnectRemote;

// End of VB project level imports

//using JBC_API_Remote;
//using RemoteManRegister.Cplot;

// 09/12/2013 Se añade Farhenheit (1.66.75.1)
// 22/04/2014 Se cambia la ubicación de TEMPLATE_DIRECTORY y se mueven los archivos a la nueva ubicación
// 09/04/2015 Environment.TickCount And Int32.MaxValue: se añade 'And Int32.MaxValue' para que devuelva siempre un positivo
//       TickCount cycles between Int32.MinValue, which is a negative number, and Int32.MaxValue once every 49.8 days.
//       This removes the sign bit to yield a nonnegative number that cycles between zero and Int32.MaxValue once every 24.9 days.


namespace RemoteManRegister
{
    public partial class frmMainRegister
    {

        //#region Default Instance

        //    private static frmMainRegister defaultInstance;

        //    /// <summary>
        //    /// Added by the VB.Net to C# Converter to support default instance behavour in C#
        //    /// </summary>
        //    public static frmMainRegister Default
        //    {
        //        get
        //        {
        //            if (defaultInstance == null)
        //            {
        //                defaultInstance = new frmMainRegister();
        //                defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
        //            }

        //            return defaultInstance;
        //        }
        //        set
        //        {
        //            defaultInstance = value;
        //        }
        //    }

        //    static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        //    {
        //        defaultInstance = null;
        //    }

        //#endregion
        // forms
        public frmWait frmWaitReg;
        public frmSeries frmSeriesReg;
        public frmOptions frmOptionsReg;
        public frmAxisGrid frmAxisGridReg;
        public frmWizard1 frmWizard1Reg;
        public frmWizard2 frmWizard2Reg;
        public frmTemplate frmTemplateReg;

        //constants
        public const System.Int32 TEMPERATURE = Cplot.TEMPERATURE;
        public const System.Int32 POWER = Cplot.POWER;
        public const System.Int32 FLOW = Cplot.FLOW;
        public const System.UInt64 NO_COM = Cplot.NO_STATION_ID;
        public const int MAX_SERIE_NUM = 12;
        // 22/04/2014 Se cambia la ubicación de TEMPLATE_DIRECTORY
        public string TEMPLATE_DIRECTORY_OLD; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        public string TEMPLATE_DIRECTORY; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

        //defining the default name constant strings
        //Friend Const tempText As String = "Temperature"
        //Friend Const pwrText As String = "Power"

        //defining the default series name counter
        internal int tempCnt = 0;
        internal int pwrCnt = 0;
        internal int flowCnt = 0;

        public enum cAction
        {
            A_PLAY = 0,
            A_PAUSE = 1,
            A_STOP = 2,
            A_RECORD = 3
        }
        public enum cBckWorks
        {
            BCK_IMPORT_CSV,
            BCK_EXPORT_CSV,
            BCK_EXPORT_LBR,
            BCK_IMPORT_LBR
        }
        public enum cZoomType
        {
            XY,
            X,
            Y
        }

        private cAction pauseFrom;

        //structures
        public struct tBckParams
        {
            public cBckWorks whatToDo;
            public string fileName;
        }

        public struct tAxisAndGrid
        {
            public string tempUnits; // #edu#
            public double Tmin;
            public double Tmax;
            public double Pmin;
            public double Pmax;
            public double Pstep;
            public double Tstep;
            public int rulerAxis;
            public double timeRange;
            public double timeStep;
            public double timeMax;
            public double timeMin;
        }

        public struct tConfig
        {
            public tAxisAndGrid axisAndGrid;
        }

        //global var's
        public tConfig config;

        public Cplot plot;

        private bool init = false;

        private cAction status;

        private Timer tmrPlot;
        private ulong lastTick;
        private ulong recordTime;
        private ulong playTime;

        private Timer tmrTrigger;
        public ulong triggerStationID = UInt64.MaxValue;
        public int triggerPort;

        private int timeBarWidth;

        private Cplot.tBitmapCustomCbk bmpCustomCbk;

        private cZoomType zoomType;
        private bool zoomStatus = false;
        private Point zoomP1;
        private Point zoomP2;
        private System.Boolean zoomInit = false;
        private System.Boolean zoomFinish = false;

        internal bool coordStatus = false;
        private int coordTime;
        private bool coordSelected = false;
        private System.Windows.Forms.Form frmCoords = new System.Windows.Forms.Form();
        private int myFrmID;
        private JBC_API_Remote jbc = null;

        private string defaultTempUnits; // temp units when created the window

        //--load and dispose methods--
        public frmMainRegister(int frmID, string currentTempUnits, JBC_API_Remote _jbc)
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            TEMPLATE_DIRECTORY_OLD = Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + "\\JBC\\LabRegister\\Templates\\";
            TEMPLATE_DIRECTORY = RoutinesLibrary.Data.DataType.StringUtils.chkSlash(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles)) + "JBC\\Manager\\Templates\\";


            // Llamada necesaria para el diseñador.
            InitializeComponent();

            //Added to support default instance behavour in C#
            //if (defaultInstance == null)
            //    defaultInstance = this;

            try
            {
                jbc = _jbc;
                jbc.StationDisconnected += JBC_StationDisconnected;
                myFrmID = frmID;
                defaultTempUnits = currentTempUnits;

                var versionDLL = System.Reflection.Assembly.GetAssembly(typeof(frmMainRegister)).GetName().Version.ToString();
                Text = Text + " - " + versionDLL;

                // load text based on current culture
                ReloadTexts();

                //Setting the timer for the plot refresh
                tmrPlot = new Timer();
                tmrPlot.Tick += tmr_Tick;
                tmrPlot.Interval = 100;

                //Setting the trigger timer
                tmrTrigger = new Timer();
                tmrTrigger.Tick += tmrTrigger_Tick;
                tmrTrigger.Interval = 100;
                triggerStationID = UInt64.MaxValue;
                triggerPort = 1;
                lblTrigger.Tag = -1;

                //Creating the template directory if don't exists
                // 22/04/2014 Se cambia la ubicación de TEMPLATE_DIRECTORY
                //(it should be created and files moved at installation, with everyone permision of modify)
                if (!Microsoft.VisualBasic.FileIO.FileSystem.DirectoryExists(TEMPLATE_DIRECTORY))
                {
                    // copy files from OLD location (may be cannot move-delete with user permissions)
                    Microsoft.VisualBasic.FileIO.FileSystem.CreateDirectory(TEMPLATE_DIRECTORY);
                    if (Microsoft.VisualBasic.FileIO.FileSystem.DirectoryExists(TEMPLATE_DIRECTORY_OLD))
                    {
                        foreach (string sfile in Microsoft.VisualBasic.FileIO.FileSystem.GetFiles(TEMPLATE_DIRECTORY_OLD))
                        {
                            Microsoft.VisualBasic.FileIO.FileSystem.CopyFile(sfile, TEMPLATE_DIRECTORY + System.IO.Path.GetFileName(sfile));
                        }
                    }
                }

                //Creating the custom bitmap for the XY and zoom tools
                bmpCustomCbk = new Cplot.tBitmapCustomCbk(bitmapTools);
                frmCoords.Dispose();

                pcbMain.AllowDrop = true;

                //Creating the main plot
                plot = new Cplot(pcbMain, myFrmID, jbc);
                plot.setCustomCbk(bmpCustomCbk);
                loadSettings();
                applyAxisConfiguration();
                PictureBox null_PictureBox = null;
                plot.draw(true, ref null_PictureBox);

                //Indicating initialized the form
                init = true;

                //Starting the trigger timer
                tmrTrigger.Start();

                //Reseting plot for the first time, to initialize status and var's
                newPlot();
            }
            catch (CerrorRegister err)
            {
                err.showError();
                return;
            }
        }

        public void frmMain_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            // 13/01/2014 Stop continuous mode, if recording
            plot.stopContinuousMode();
            //finishing the memory and saving the settings
            plot.dispose();
            saveSettings();
            tmrTrigger.Stop();
            tmrPlot.Stop();
            if (ClosedFrmEvent != null)
                ClosedFrmEvent(myFrmID);
        }

        private void JBC_StationDisconnected(long stationID)
        {
            List<ulong> stnList = new List<ulong>();
            plot.getListOfSerieStationID(stnList);
            if (stnList.Contains((ulong)stationID))
            {
                CancelDrawing();
            }
        }

        #region Public Events
        public delegate void ClosedFrmEventHandler(int frmID);
        private ClosedFrmEventHandler ClosedFrmEvent;

        public event ClosedFrmEventHandler ClosedFrm
        {
            add
            {
                ClosedFrmEvent = (ClosedFrmEventHandler)System.Delegate.Combine(ClosedFrmEvent, value);
            }
            remove
            {
                ClosedFrmEvent = (ClosedFrmEventHandler)System.Delegate.Remove(ClosedFrmEvent, value);
            }
        }


        #endregion

        #region Public functions

        public void ReloadTexts()
        {
            FileToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuFileId);
            NewToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuFileNewId) + "...";
            OpenToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuFileOpenId) + "...";
            SaveAsToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuFileSaveAsId) + "...";
            PrintToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuFilePrintId) + "...";
            ExportToCSVToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuFileExportToCSVId) + "...";

            ConfigurationToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigId);
            WizardToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigWizardId) + "...";
            SerieToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigSeriesId) + "...";
            AxisGridToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigAxisId) + "...";
            OptionsToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigOptionsId) + "...";
            TitleToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigTitleId) + "...";
            TemplatesToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigTemplatesId);
            LoadToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigTemplatesLoadId) + "...";
            SaveToolStripMenuItem.Text = Localization.getResStr(ManRegGlobal.regMnuConfigTemplatesSaveId) + "...";

            tsbXY.Text = Localization.getResStr(ManRegGlobal.regStripCoordinatesId);
            tsbXY.ToolTipText = Localization.getResStr(ManRegGlobal.regStripCoordinatesId);

            tsbZooms.Text = Localization.getResStr(ManRegGlobal.regStripZoomId);
            tsbZooms.ToolTipText = Localization.getResStr(ManRegGlobal.regStripZoomId);

            tsbDefaultZoom.Text = Localization.getResStr(ManRegGlobal.regStripDefaultZoomId);
            tsbDefaultZoom.ToolTipText = Localization.getResStr(ManRegGlobal.regStripDefaultZoomId);

            lblTrigger.ToolTipText = Localization.getResStr(ManRegGlobal.regStripTriggerId);
            updateTriggerLabel();

            tsbResetTrigger.Text = Localization.getResStr(ManRegGlobal.regStripResetTriggerId);
            tsbResetTrigger.ToolTipText = Localization.getResStr(ManRegGlobal.regStripResetTriggerId);

            tsbPlay.Text = Localization.getResStr(ManRegGlobal.regStripPlayId);
            tsbPlay.ToolTipText = Localization.getResStr(ManRegGlobal.regStripPlayId);

            tsbPause.Text = Localization.getResStr(ManRegGlobal.regStripPauseId);
            tsbPause.ToolTipText = Localization.getResStr(ManRegGlobal.regStripPauseId);

            tsbStop.Text = Localization.getResStr(ManRegGlobal.regStripStopId);
            tsbStop.ToolTipText = Localization.getResStr(ManRegGlobal.regStripStopId);

            tsbRecord.Text = Localization.getResStr(ManRegGlobal.regStripRecordId);
            tsbRecord.ToolTipText = Localization.getResStr(ManRegGlobal.regStripRecordId);

            lblStatus.ToolTipText = Localization.getResStr(ManRegGlobal.regStripStatusId);
            updateStatusLabel();

            // children forms
            if (frmWaitReg != null)
            {
                frmWaitReg.ReloadTexts();
            }
            if (frmSeriesReg != null)
            {
                frmSeriesReg.ReloadTexts();
            }
            if (frmOptionsReg != null)
            {
                frmOptionsReg.ReloadTexts();
            }
            if (frmAxisGridReg != null)
            {
                frmAxisGridReg.ReloadTexts();
            }
            if (frmWizard1Reg != null)
            {
                frmWizard1Reg.ReloadTexts();
            }
            if (frmWizard2Reg != null)
            {
                frmWizard2Reg.ReloadTexts();
            }
            if (frmTemplateReg != null)
            {
                frmTemplateReg.ReloadTexts();
            }


        }

        public void reDraw(bool initPlot = false)
        {
            try
            {
                if (init)
                {
                    PictureBox null_PictureBox = null;
                    plot.draw(initPlot, ref null_PictureBox);
                }
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        public frmSeries myInitFormSeries()
        {
            if (ReferenceEquals(frmSeriesReg, null))
            {
                frmSeriesReg = new frmSeries(this, jbc);
                frmSeriesReg.Owner = this;
            }
            return frmSeriesReg;
        }

        public void CancelDrawing()
        {
            plot.myStop();
            tmrPlot.Stop();
            action(cAction.A_STOP);
        }

        #endregion


        //--private functions--
        private void newPlot()
        {
            action(cAction.A_STOP);
            playTime = 0;
            recordTime = 0;
            zoomFinish = false;
            zoomInit = false;
            zoomStatus = false;
            activeCoordStatus(false);
            setMediaBar(playTime, recordTime);
            plot.clearSeries();
            plot.myTitle = "NO TITLE";
            reDraw();
        }

        private void loadSettings()
        {
            //setting last desktop location and size
            if (My.Settings.Default.AppMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                int x = System.Convert.ToInt32(My.Settings.Default.AppPosition.X);
                int y = System.Convert.ToInt32(My.Settings.Default.AppPosition.Y);
                this.SetDesktopBounds(150, 150, System.Convert.ToInt32(My.Settings.Default.AppSize.Width), System.Convert.ToInt32(My.Settings.Default.AppSize.Height));
                foreach (Screen sc in Screen.AllScreens)
                {
                    if (x < sc.Bounds.X + sc.Bounds.Width & x > sc.Bounds.X & y < sc.Bounds.Y + sc.Bounds.Height & y > sc.Bounds.Y)
                    {
                        this.SetDesktopBounds(x, y, System.Convert.ToInt32(My.Settings.Default.AppSize.Width), System.Convert.ToInt32(My.Settings.Default.AppSize.Height));
                    }
                }
            }

            //loading last configuration
            config.axisAndGrid.tempUnits = defaultTempUnits;
            if (config.axisAndGrid.tempUnits == ManRegGlobal.tempunitCELSIUS)
            {
                config.axisAndGrid.Tmin = System.Convert.ToDouble(My.Settings.Default.Tmin);
                config.axisAndGrid.Tmax = System.Convert.ToDouble(My.Settings.Default.Tmax);
                config.axisAndGrid.Tstep = System.Convert.ToDouble(My.Settings.Default.Tstep);
            }
            if (config.axisAndGrid.tempUnits == ManRegGlobal.tempunitFAHRENHEIT)
            {
                config.axisAndGrid.Tmin = System.Convert.ToDouble(My.Settings.Default.TminF);
                config.axisAndGrid.Tmax = System.Convert.ToDouble(My.Settings.Default.TmaxF);
                config.axisAndGrid.Tstep = System.Convert.ToDouble(My.Settings.Default.TstepF);
            }

            config.axisAndGrid.Pmin = System.Convert.ToDouble(My.Settings.Default.Pmin);
            config.axisAndGrid.Pmax = System.Convert.ToDouble(My.Settings.Default.Pmax);
            config.axisAndGrid.Pstep = System.Convert.ToDouble(My.Settings.Default.Pstep);
            config.axisAndGrid.rulerAxis = System.Convert.ToInt32(My.Settings.Default.rulerAxis);
            config.axisAndGrid.timeRange = System.Convert.ToDouble(My.Settings.Default.timeRange);
            config.axisAndGrid.timeStep = System.Convert.ToDouble(My.Settings.Default.timeStep);
            config.axisAndGrid.timeMin = System.Convert.ToDouble(My.Settings.Default.timeMin);
            config.axisAndGrid.timeMax = System.Convert.ToDouble(My.Settings.Default.timeMax);
            plot.myBckGnd = My.Settings.Default.bckGnd;
            plot.myTempAxisClr = My.Settings.Default.axisTempClr;
            plot.myPwrAxisClr = My.Settings.Default.axisPwrClr;
            plot.myTimeAxisClr = My.Settings.Default.axisTimeClr;
            plot.myGridClr = My.Settings.Default.gridClr;
            plot.myTextClr = My.Settings.Default.textClr;
            plot.myTitleClr = My.Settings.Default.titleClr;
            plot.myLineWidth = System.Convert.ToInt32(My.Settings.Default.lineWidth);
            plot.myPointWidth = System.Convert.ToInt32(My.Settings.Default.pointWidth);
        }

        private void saveSettings()
        {
            //storing the current form position and size
            if (this.WindowState == FormWindowState.Maximized)
            {
                My.Settings.Default.AppMaximized = true;
            }
            else
            {
                My.Settings.Default.AppMaximized = false;
                My.Settings.Default.AppPosition = this.Location;
                My.Settings.Default.AppSize = this.Size;
            }

            //saving current configuration
            if (config.axisAndGrid.tempUnits == ManRegGlobal.tempunitCELSIUS)
            {
                My.Settings.Default.Tmin = config.axisAndGrid.Tmin;
                My.Settings.Default.Tmax = config.axisAndGrid.Tmax;
                My.Settings.Default.Tstep = config.axisAndGrid.Tstep;
            }
            if (config.axisAndGrid.tempUnits == ManRegGlobal.tempunitFAHRENHEIT)
            {
                My.Settings.Default.TminF = config.axisAndGrid.Tmin;
                My.Settings.Default.TmaxF = config.axisAndGrid.Tmax;
                My.Settings.Default.TstepF = config.axisAndGrid.Tstep;
            }
            My.Settings.Default.Pmin = config.axisAndGrid.Pmin;
            My.Settings.Default.Pmax = config.axisAndGrid.Pmax;
            My.Settings.Default.Pstep = config.axisAndGrid.Pstep;
            My.Settings.Default.rulerAxis = config.axisAndGrid.rulerAxis;
            My.Settings.Default.timeRange = config.axisAndGrid.timeRange;
            My.Settings.Default.timeStep = config.axisAndGrid.timeStep;
            My.Settings.Default.timeMin = config.axisAndGrid.timeMin;
            My.Settings.Default.timeMax = config.axisAndGrid.timeMax;
            My.Settings.Default.bckGnd = plot.myBckGnd;
            My.Settings.Default.axisTempClr = plot.myTempAxisClr;
            My.Settings.Default.axisPwrClr = plot.myPwrAxisClr;
            My.Settings.Default.axisTimeClr = plot.myTimeAxisClr;
            My.Settings.Default.gridClr = plot.myGridClr;
            My.Settings.Default.textClr = plot.myTextClr;
            My.Settings.Default.titleClr = plot.myTitleClr;
            My.Settings.Default.lineWidth = plot.myLineWidth;
            My.Settings.Default.pointWidth = plot.myPointWidth;
            //saving
            My.Settings.Default.Save();
        }

        private void applyAxisConfiguration(bool byRange = false)
        {
            plot.myTempUnits = config.axisAndGrid.tempUnits;
            plot.configTempAxis(config.axisAndGrid.Tmin, config.axisAndGrid.Tmax, config.axisAndGrid.Tstep);
            plot.configPwrAxis(config.axisAndGrid.Pmin, config.axisAndGrid.Pmax, config.axisAndGrid.Pstep);
            if (byRange)
            {
                plot.configTimeAxis(config.axisAndGrid.timeRange, config.axisAndGrid.timeStep);
            }
            if (!byRange)
            {
                plot.configTimeAxis(config.axisAndGrid.timeMin, config.axisAndGrid.timeMax, config.axisAndGrid.timeStep);
            }
            plot.myRulerAxis = config.axisAndGrid.rulerAxis;
        }

        private void action(cAction ac)
        {
            try
            {
                if (ac == cAction.A_PAUSE)
                {
                    if (status == cAction.A_PLAY | status == cAction.A_RECORD)
                    {
                        pauseFrom = status;
                        plot.myPause();
                        status = cAction.A_PAUSE;
                        lblStatus.ForeColor = Color.Black;
                        lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusPauseId); // "PAUSE"
                        tmrPlot.Stop();
                    }
                    else if (status == cAction.A_PAUSE)
                    {
                        //depending on the previous state
                        if (pauseFrom == cAction.A_PLAY)
                        {
                            action(cAction.A_PLAY);
                        }
                        if (pauseFrom == cAction.A_RECORD)
                        {
                            action(cAction.A_RECORD);
                        }
                    }
                }
                if (ac == cAction.A_PLAY)
                {
                    //it must be at least one time range of recorded time in order to reproduce.
                    if (recordTime > Convert.ToUInt64(config.axisAndGrid.timeRange * 1000))
                    {
                        //in case coord status is active, deactivating it
                        if (coordStatus)
                        {
                            activeCoordStatus(false);
                        }

                        //setting initial play time
                        if (status != cAction.A_PAUSE | (status == cAction.A_PAUSE & pauseFrom == cAction.A_RECORD))
                        {
                            playTime = 0;
                        }
                        lastTick = (ulong)(Environment.TickCount & int.MaxValue);
                        plot.myPlay();
                        status = cAction.A_PLAY;
                        lblStatus.ForeColor = Color.Black;
                        lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusPlayId); // "PLAY"
                        tmrPlot.Start();
                    }
                }
                if (ac == cAction.A_RECORD)
                {
                    //in case coord status is active, deactivating it
                    if (coordStatus)
                    {
                        activeCoordStatus(false);
                    }

                    bool proceed = true;

                    // 13/01/2014 start continuous mode and check if plotting same stations in another window
                    if (!plot.startContinuousMode())
                    {
                        proceed = false;
                        Interaction.MsgBox(Localization.getResStr(ManRegGlobal.regPlottingInAnotherWindowID), MsgBoxStyle.OkOnly, Localization.getResStr(ManRegGlobal.gralWarningId));
                    }

                    //setting initial play time
                    if (proceed)
                    {
                        if (status != cAction.A_PAUSE | (status == cAction.A_PAUSE & pauseFrom == cAction.A_PLAY))
                        {
                            //remembering the user to save the file
                            if (recordTime != 0 && plot.myTrigger == Cplot.cTrigger.TRG_MANUAL)
                            {
                                proceed = Interaction.MsgBox(Localization.getResStr(ManRegGlobal.regDataLostId), MsgBoxStyle.OkCancel, Localization.getResStr(ManRegGlobal.gralWarningId)) == MsgBoxResult.Ok;
                            }
                            if (proceed)
                            {
                                recordTime = 0;
                                playTime = 0;
                                lastTick = (ulong)(Environment.TickCount & int.MaxValue); // se añade And Int32.MaxValue para que devuelva siempre un positivo
                            }
                        }
                    }

                    //recording status
                    if (proceed)
                    {
                        plot.myRecord();
                        status = cAction.A_RECORD;
                        lblStatus.ForeColor = Color.Red;
                        lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusRecordId); // "RECORD"
                        tmrPlot.Start();
                        // 13/01/2014
                        //tsbPlay.Enabled = False
                        //tsbPause.Enabled = False
                        //tsbStop.Enabled = True
                        //tsbRecord.Enabled = False
                    }
                }

                if (ac == cAction.A_STOP)
                {
                    // 13/01/2014 stop continuous mode and check if plotting same stations in another window
                    if (status == cAction.A_RECORD)
                    {
                        plot.stopContinuousMode();
                        // 13/01/2014
                        //tsbPlay.Enabled = True
                        //tsbPause.Enabled = True
                        //tsbStop.Enabled = False
                        //tsbRecord.Enabled = True
                    }

                    //stopping playing or recording
                    plot.myStop();
                    status = cAction.A_STOP;
                    lblStatus.ForeColor = Color.Black;
                    lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusStopId); // "STOP"
                    tmrPlot.Stop();
                    playTime = 0;
                    setMediaBar(playTime, recordTime);
                }
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        private void updateStatusLabel()
        {
            switch (status)
            {
                case cAction.A_PAUSE:
                    lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusPauseId); // "PAUSE"
                    break;
                case cAction.A_PLAY:
                    lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusPlayId); // "PLAY"
                    break;
                case cAction.A_RECORD:
                    lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusRecordId); // "RECORD"
                    break;
                case cAction.A_STOP:
                    lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusStopId);
                    break;
                default:
                    lblStatus.Text = Localization.getResStr(ManRegGlobal.regStripStatusStopId);
                    break;
            }
        }
        //--------------------------------------------------------
        private void printConfig()
        {
            prntPlot.DefaultPageSettings.Landscape = true;
            prntPlot.DocumentName = this.Text;
            prntPlot.OriginAtMargins = false;
        }

        private void printCurrentPlot()
        {
            if (status == cAction.A_PLAY | status == cAction.A_RECORD)
            {
                action(cAction.A_PAUSE);
            }
            printConfig();
            PrintDialog1.AllowPrintToFile = true;
            PrintDialog1.UseEXDialog = true;
            PrintDialog1.Document = prntPlot;

            //printDialog = True
            if (PrintDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //printFromPreView = False
                prntPlot.Print();
            }
            //printDialog = False
        }
        //Private preViewDialog As Boolean = False
        //Private printDialog As Boolean = False
        //Private printFromPreView As Boolean = False
        private void printPreviewCurrentPlot()
        {
            if (status != cAction.A_PLAY & status != cAction.A_RECORD)
            {
                printConfig();
                PrintPreviewDialog1.Document = prntPlot;
                PrintPreviewDialog1.WindowState = FormWindowState.Maximized;
                //preViewDialog = True
                PrintPreviewDialog1.ShowDialog();
                // preViewDialog = False
                //printFromPreView = False
            }
        }

        //---------------------------------------------------------
        private ulong calculateTimeBar()
        {
            int mouseX = this.PointToClient(frmMainRegister.MousePosition).X;
            ulong time = default(ulong);
            if (mouseX < pcbMediaBar.Bounds.X)
            {
                time = 0;
            }
            else
            {
                time = (ulong)(Convert.ToDouble(recordTime) * Convert.ToDouble(mouseX - pcbMediaBar.Bounds.X) / Convert.ToDouble(timeBarWidth));
                if (time > recordTime)
                {
                    time = recordTime;
                }
            }
            return time;
        }

        private void timeBarClick()
        {
            if (status != cAction.A_RECORD & recordTime > 0)
            {
                playTime = calculateTimeBar();

                //adjusting the playtime value to consider the window range
                ulong endTime = default(ulong);
                if (recordTime > Convert.ToUInt64(config.axisAndGrid.timeRange * 1000))
                {
                    endTime = recordTime - Convert.ToUInt64(config.axisAndGrid.timeRange * 1000);
                }
                if (recordTime <= Convert.ToUInt64(config.axisAndGrid.timeRange * 1000))
                {
                    endTime = 0;
                }
                if (playTime > endTime)
                {
                    playTime = endTime;
                }
                setMediaBar(playTime, recordTime);
                moveToTime(Convert.ToDouble(playTime) / 1000.0);
            }
        }

        // VBConversions Note: Former VB static variables moved to class level because they aren't supported in C#.
        private ulong timeBarMouseMove_time = 0;

        private void timeBarMouseMove()
        {
            // static ulong time = 0; VBConversions Note: Static variable moved to class level and renamed timeBarMouseMove_time. Local static variables are not supported in C#.
            if (status != cAction.A_RECORD & recordTime > 0)
            {
                ulong newTime = calculateTimeBar();
                if (Math.Round((double)newTime, 3) != Math.Round((double)timeBarMouseMove_time, 3))
                {
                    timeBarMouseMove_time = newTime;
                    string timeStr = (timeBarMouseMove_time / 3600000).ToString("00") + ":" +
                                     ((timeBarMouseMove_time % 3600000) / 60000).ToString("00") + ":" +
                                     (((timeBarMouseMove_time % 3600000) % 60000) / 1000).ToString("00");
                    ttpPlayTime.SetToolTip(pcbMediaBar, timeStr);
                }
            }
        }

        private void moveToTime(double time)
        {
            try
            {
                double min = 0;
                double max = 0;
                min = time; //- config.axisAndGrid.timeRange / 2
                max = time + config.axisAndGrid.timeRange; // / 2
                plot.configTimeAxis(min, max, config.axisAndGrid.timeStep);
                PictureBox temp_pcb = null;
                plot.draw(true, ref temp_pcb, true, false);
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        //time values in seconds
        private void setMediaBar(ulong playTime, ulong totalTime)
        {
            //generating the strings for the play time and the record time
            string playStr = (playTime / 3600000).ToString("00") + ":" + ((playTime % 3600000) / 60000).ToString("00") + ":" + (((playTime % 3600000) % 60000) / 1000).ToString("00");
            string totalStr = (totalTime / 3600000).ToString("00") + ":" + ((totalTime % 3600000) / 60000).ToString("00") + ":" + (((totalTime % 3600000) % 60000) / 1000).ToString("00");

            //creating the image
            Cbuffer buff = new Cbuffer(pcbMediaBar.Width, pcbMediaBar.Height);
            buff.draw.Clear(this.BackColor);

            //measuring the time strings
            SizeF size = buff.draw.MeasureString(playStr + " // " + totalStr, this.Font);

            //drawing the play time bar
            int barMargin = 3;
            timeBarWidth = System.Convert.ToInt32(buff.bmp.Width - size.Width - 2 * barMargin);
            int posLeft = 0;
            int posRight = 0;
            if (totalTime > Convert.ToUInt64(config.axisAndGrid.timeRange * 1000))
            {
                posLeft = System.Convert.ToInt32((Convert.ToDouble(playTime) / Convert.ToDouble(totalTime)) * timeBarWidth);
                posRight = System.Convert.ToInt32((Convert.ToDouble(playTime + Convert.ToUInt64(config.axisAndGrid.timeRange * 1000)) / Convert.ToDouble(totalTime)) * timeBarWidth);
            }
            else
            {
                posLeft = 0;
                posRight = 0;
            }
            buff.draw.DrawLine(Pens.Black, barMargin, buff.bmp.Height / 2, timeBarWidth, buff.bmp.Height / 2);
            buff.draw.FillRectangle(Brushes.Black, barMargin + posLeft - 2, 0, 4, buff.bmp.Height);
            buff.draw.FillRectangle(Brushes.Black, barMargin + posRight - 2, 0, 4, buff.bmp.Height);

            //drawing the time strings
            buff.draw.DrawString(playStr + " // " + totalStr, this.Font, Brushes.Black, timeBarWidth + 2 * barMargin, buff.bmp.Height / 2 - size.Height / 2);

            //drawing on the media picture box
            pcbMediaBar.Image = buff.bmp;
        }
        //--------------------------------------------------------------

        private void calculateZoom(Point p1, Point p2)
        {
            //assigning to local var's the current axis configuration
            tAxisAndGrid axis = new tAxisAndGrid();
            plot.getAxisConfig("temp", ref axis.Tmin, ref axis.Tmax, ref axis.Tstep);
            plot.getAxisConfig("pwr", ref axis.Pmin, ref axis.Pmax, ref axis.Pstep);
            plot.getAxisConfig("time_val", ref axis.timeMin, ref axis.timeMax, ref axis.timeStep);
            axis.timeRange = axis.timeMax - axis.timeMin;

            //converting the pixels into points in the temp and pwr axis
            PointF pt = new PointF();
            pt = plot.convertPixToPoint(p1, TEMPERATURE);
            if (zoomType == cZoomType.XY | zoomType == cZoomType.Y)
            {
                axis.Tmax = pt.Y;
            }
            if (zoomType == cZoomType.XY | zoomType == cZoomType.X)
            {
                axis.timeMin = pt.X;
            }
            pt = plot.convertPixToPoint(p2, TEMPERATURE);
            if (zoomType == cZoomType.XY | zoomType == cZoomType.Y)
            {
                axis.Tmin = pt.Y;
            }
            if (zoomType == cZoomType.XY | zoomType == cZoomType.X)
            {
                axis.timeMax = pt.X;
            }
            pt = plot.convertPixToPoint(p1, POWER);
            if (zoomType == cZoomType.XY | zoomType == cZoomType.Y)
            {
                axis.Pmax = pt.Y;
            }
            pt = plot.convertPixToPoint(p2, POWER);
            if (zoomType == cZoomType.XY | zoomType == cZoomType.Y)
            {
                axis.Pmin = pt.Y;
            }
            if (zoomType == cZoomType.XY | zoomType == cZoomType.X)
            {
                axis.timeRange = axis.timeMax - axis.timeMin;
            }

            //assigning the new axis configuration
            plot.configTempAxis(axis.Tmin, axis.Tmax, axis.Tstep);
            plot.configPwrAxis(axis.Pmin, axis.Pmax, axis.Pstep);
            if (status == cAction.A_RECORD | (status == cAction.A_PAUSE & pauseFrom == cAction.A_RECORD))
            {
                plot.configTimeAxis(axis.timeRange, axis.timeStep);
            }
            else
            {
                if (zoomType == cZoomType.XY | zoomType == cZoomType.X)
                {
                    plot.configTimeAxis(axis.timeMin, axis.timeMax, axis.timeStep);
                }
                if (zoomType == cZoomType.XY | zoomType == cZoomType.Y)
                {
                    plot.configTimeAxis(axis.timeRange, axis.timeStep);
                }
            }
        }

        private void bitmapTools(ref Cbuffer buf, ref Rectangle pRect)
        {
            //resizing the buffer
            buf.resize(pcbMain.Width, pcbMain.Height);

            //clearing teh background
            buf.draw.Clear(Color.Transparent);

            //if proper drawing the zoom rectangle
            if (zoomStatus && zoomInit && !zoomFinish)
            {
                //the zoom line or rectangle is drawn in the oposite color of the background
                Pen clr = new Pen(Color.FromArgb(255, System.Convert.ToInt32(255 - plot.myBckGnd.R), System.Convert.ToInt32(255 - plot.myBckGnd.G), System.Convert.ToInt32(255 - plot.myBckGnd.B)), 1);
                if (zoomType == cZoomType.XY)
                {
                    buf.draw.DrawRectangle(clr, new Rectangle(zoomP1.X, zoomP1.Y, zoomP2.X - zoomP1.X, zoomP2.Y - zoomP1.Y));
                }
                else if (zoomType == cZoomType.Y)
                {
                    buf.draw.DrawLine(clr, zoomP1.X, zoomP1.Y, zoomP1.X, zoomP2.Y);
                }
                else if (zoomType == cZoomType.X)
                {
                    buf.draw.DrawLine(clr, zoomP1.X, zoomP1.Y, zoomP2.X, zoomP1.Y);
                }
            }

            //updating the coord viewer if necessary
            if (coordSelected && (coordTime <= pRect.Right & coordTime >= pRect.X))
            {
                buf.draw.DrawLine(Pens.Black, coordTime, pRect.Bottom, coordTime, pRect.Y);
            }
        }

        private void activeCoordStatus(bool active)
        {
            //activating or deactivating the coord status
            tsbXY.Checked = active;
            coordStatus = active;
            coordSelected = false;

            //if coord status is unsetted disposing the coord's form
            if (!coordStatus && !frmCoords.IsDisposed)
            {
                frmCoords.Dispose();
            }

            //if coord status is setted forcing the pause status in case play or record
            if (coordStatus && (status == cAction.A_PLAY | status == cAction.A_RECORD))
            {
                action(cAction.A_PAUSE);
            }

            //setting a default selection time if activating
            if (active)
            {
                coordTime = System.Convert.ToInt32((double)pcbMain.Width / 2);
            }
            coordSelected = true;
            setFormCoords();

            //updating the plot
            reDraw();
        }

        private void fomCoords_Dispose(System.Object sender, System.EventArgs e)
        {
            activeCoordStatus(false);
        }

        private void createFormCoords()
        {
            //creating the form
            frmCoords = new Form();
            frmCoords.Disposed += fomCoords_Dispose;
            frmCoords.Name = "frmCoords";
            frmCoords.Owner = this;
            frmCoords.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowOnly;
            frmCoords.AutoSize = true;
            frmCoords.MinimizeBox = false;
            frmCoords.MaximizeBox = false;
            frmCoords.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            frmCoords.StartPosition = FormStartPosition.Manual;
            frmCoords.Icon = this.Icon;
            frmCoords.TopMost = true;
            frmCoords.Location = new Point(System.Convert.ToInt32(this.Location.X + (double)this.Width / 2), System.Convert.ToInt32(this.Location.Y + (double)this.Height / 2));
            frmCoords.Size = new Size(150, 20);
            frmCoords.Hide();
        }

        internal void setFormCoords()
        {
            //creating the form if not yet created
            if (frmCoords.IsDisposed)
            {
                createFormCoords();
            }

            //setting the time value
            double timeSec = plot.convertPixToPoint(new Point(coordTime, 0), Cplot.TEMPERATURE).X;
            string timeStr = (Math.Truncate(timeSec) / 3600).ToString("00") + ":" + ((Math.Truncate(timeSec) % 3600) / 60).ToString("00") + ":" + ((Math.Truncate(timeSec) % 3600) % 60).ToString("00") + ":" + System.Convert.ToString(((timeSec - Convert.ToDouble(Math.Truncate(timeSec))) * 100).ToString("00"));
            frmCoords.Text = Localization.getResStr(ManRegGlobal.regCoordTimeId) + " " + timeStr;

            //getting all the series ID's
            List<string> IDlst = new List<string>();
            plot.getListOfSerieName(IDlst);

            //generating the series labels
            Label lblSerie = null;
            Label lblValue = null;
            Color clr = new Color();
            ulong station = UInt64.MaxValue;
            string legend = "";
            int mag = 0;
            int port = 0;
            bool pts = false;
            bool lines = false;
            int ySerie = 0;
            ySerie = 0;
            foreach (string s in IDlst)
            {
                //setting the serie name label
                plot.getSerieConfig(s, ref station, ref port, ref mag, ref clr, ref pts, ref lines, ref legend);
                if (frmCoords.Controls.Find("lbl" + s, true).Length == 0)
                {
                    lblSerie = new Label();
                    lblSerie.Name = "lbl" + s;
                    lblSerie.AutoSize = true;
                    lblSerie.ForeColor = clr;
                    lblSerie.Font = new Font("MS Sans Seriff", 10, FontStyle.Bold);
                    lblSerie.Text = s + ":";
                    lblSerie.Location = new Point(0, ySerie);

                    //adding the label to the form
                    frmCoords.Controls.Add(lblSerie);
                }
                else if (frmCoords.Controls.Find("lbl" + s, true).Length > 0)
                {
                    lblSerie = (Label)(frmCoords.Controls["lbl" + s]);
                }

                //setting the serie value label
                if (frmCoords.Controls.Find("lbl" + s + "Value", true).Length == 0)
                {
                    lblValue = new Label();
                    lblValue.Name = "lbl" + s + "Value";
                    lblValue.AutoSize = true;
                    lblValue.Font = new Font("MS Sans Seriff", 10);
                    lblValue.ForeColor = clr;
                    lblValue.Location = new Point(lblSerie.Width, ySerie);
                    lblValue.Text = System.Convert.ToString(Math.Round(plot.getSerieValueInPos(coordTime, s), 1));

                    //adding the ñabel to the form
                    frmCoords.Controls.Add(lblValue);
                }
                else if (frmCoords.Controls.Find("lbl" + s + "Value", true).Length > 0)
                {
                    lblValue = (Label)(frmCoords.Controls["lbl" + s + "Value"]);
                    lblValue.Text = System.Convert.ToString(Math.Round(plot.getSerieValueInPos(coordTime, s), 1));
                }

                //preparing the next y coordinate
                ySerie = ySerie + lblSerie.Height;
            }

            //showing the form
            if (!frmCoords.Visible)
            {
                frmCoords.Show();
                this.Focus();
            }

            //updating the plot
            reDraw();
        }

        private void generateCSV()
        {
            //setting the proper status
            action(cAction.A_STOP);

            //opening the save file dialog
            sfdSave.Filter = Localization.getResStr(ManRegGlobal.filterCommaSeparatedId);
            if (sfdSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //showing the wait form
                int index = sfdSave.FileName.LastIndexOf("\\") + 1;
                if (ReferenceEquals(frmWaitReg, null))
                {
                    frmWaitReg = new frmWait();
                    frmWaitReg.Owner = this;
                }
                frmWaitReg.lblWaitMessage.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgGeneratingFileId), sfdSave.FileName.Substring(index));
                frmWaitReg.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgGeneratingExtId), "CSV");
                frmWaitReg.Show();
                this.Enabled = false;

                //exporting the file on background work
                tBckParams param = new tBckParams();
                param.whatToDo = cBckWorks.BCK_EXPORT_CSV;
                param.fileName = sfdSave.FileName;
                bckLongProc.RunWorkerAsync(param);
            }
        }

        private void generateLBR()
        {
            //setting the proper status
            action(cAction.A_STOP);

            //opening the save file dialog
            sfdSave.Filter = Localization.getResStr(ManRegGlobal.filterLabRegisterId);
            if (sfdSave.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //showing the wait form
                int index = sfdSave.FileName.LastIndexOf("\\") + 1;
                if (ReferenceEquals(frmWaitReg, null))
                {
                    frmWaitReg = new frmWait();
                    frmWaitReg.Owner = this;
                }
                frmWaitReg.lblWaitMessage.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgGeneratingFileId), sfdSave.FileName.Substring(index));
                frmWaitReg.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgGeneratingExtId), "LBR");
                frmWaitReg.Show();
                this.Enabled = false;

                //exporting the file on background work
                tBckParams param = new tBckParams();
                param.whatToDo = cBckWorks.BCK_EXPORT_LBR;
                param.fileName = sfdSave.FileName;
                bckLongProc.RunWorkerAsync(param);
            }
        }

        private void loadCSV()
        {
            //setting the proper status
            action(cAction.A_STOP);

            //opening the load file dialog
            ofdOpen.Filter = Localization.getResStr(ManRegGlobal.filterCommaSeparatedId);
            if (ofdOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadCSVFile(ofdOpen.FileName);
            }
        }

        private void loadCSVFile(string sFileName)
        {
            //creating a new plot
            newPlot();

            //showing the wait form
            int index = sFileName.LastIndexOf("\\") + 1;
            if (ReferenceEquals(frmWaitReg, null))
            {
                frmWaitReg = new frmWait();
                frmWaitReg.Owner = this;
            }
            frmWaitReg.lblWaitMessage.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgLoadingFileId), sFileName.Substring(index));
            frmWaitReg.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgLoadingExtId), "CSV");
            frmWaitReg.Show();
            this.Enabled = false;

            //importing the file on background work
            tBckParams param = new tBckParams();
            param.whatToDo = cBckWorks.BCK_IMPORT_CSV;
            param.fileName = sFileName;
            bckLongProc.RunWorkerAsync(param);

        }

        private void loadLBR()
        {
            //setting the proper status
            action(cAction.A_STOP);

            //opening the load file dialog
            ofdOpen.Filter = Localization.getResStr(ManRegGlobal.filterLabRegisterId);
            if (ofdOpen.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                loadLBRFile(ofdOpen.FileName);
            }
        }

        private void loadLBRFile(string sFileName)
        {
            //creating a new plot
            newPlot();

            //showing the wait form
            int index = sFileName.LastIndexOf("\\") + 1;
            if (ReferenceEquals(frmWaitReg, null))
            {
                frmWaitReg = new frmWait();
                frmWaitReg.Owner = this;
            }
            frmWaitReg.lblWaitMessage.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgLoadingFileId), sFileName.Substring(index));
            frmWaitReg.Text = string.Format(Localization.getResStr(ManRegGlobal.regMsgLoadingExtId), "LBR");
            frmWaitReg.Show();
            this.Enabled = false;

            //importing the file on background work
            tBckParams param = new tBckParams();
            param.whatToDo = cBckWorks.BCK_IMPORT_LBR;
            param.fileName = sFileName;
            bckLongProc.RunWorkerAsync(param);

        }

        private void generateTemplate()
        {
            //setting the proper status
            action(cAction.A_STOP);

            //getting the file name
            string name = Interaction.InputBox(Localization.getResStr(ManRegGlobal.regTemplateEnterNameId), Localization.getResStr(ManRegGlobal.regTemplateId), Localization.getResStr(ManRegGlobal.regTemplateDefaultNameId));
            if (!string.IsNullOrEmpty(name))
            {
                plot.saveAsTPT(TEMPLATE_DIRECTORY + "\\" + name + "." + ManRegGlobal.templateExtension);
            }
        }

        //--widgets and form methods--
        private void tmr_Tick(object sender, System.EventArgs e)
        {
            //incrementing the corresponding time
            ulong elapsedTime = (ulong)(Environment.TickCount & int.MaxValue) - lastTick;
            if (status == cAction.A_RECORD)
            {
                recordTime = recordTime + elapsedTime;
            }
            if (status == cAction.A_PLAY)
            {
                ulong endTime = default(ulong);
                if (recordTime > Convert.ToUInt64(config.axisAndGrid.timeRange * 1000))
                {
                    endTime = recordTime - Convert.ToUInt64(config.axisAndGrid.timeRange * 1000);
                }
                if (recordTime <= Convert.ToUInt64(config.axisAndGrid.timeRange * 1000))
                {
                    endTime = 0;
                }
                if (playTime >= endTime)
                {
                    action(cAction.A_PAUSE);
                }
                else
                {
                    playTime = playTime + elapsedTime;
                }
            }

            //checking current internall plot status
            if (plot.getPlotStatus() == Cplot.cStatus.STATUS_PLAY & status != cAction.A_PLAY)
            {
                action(cAction.A_PLAY);
            }
            if (plot.getPlotStatus() == Cplot.cStatus.STATUS_STOP & status != cAction.A_STOP)
            {
                action(cAction.A_STOP);
            }
            if (plot.getPlotStatus() == Cplot.cStatus.STATUS_RECORD & status != cAction.A_RECORD)
            {
                action(cAction.A_RECORD);
            }
            if ((plot.getPlotStatus() == Cplot.cStatus.STATUS_PAUSE_FROM_PLAY | plot.getPlotStatus() == Cplot.cStatus.STATUS_PAUSE_FROM_RECORD) & status != cAction.A_PAUSE)
            {
                action(cAction.A_PAUSE);
            }

            //checking API error
            if (ManRegGlobal.checkAPIerror())
            {
                action(cAction.A_STOP);
            }

            //updating the media
            setMediaBar(playTime, recordTime);

            //setting the last tick time
            lastTick = (ulong)(Environment.TickCount & int.MaxValue);
        }

        public void SerieToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            myInitFormSeries();
            frmSeriesReg.sSelectStationID = "";
            frmSeriesReg.butEdit.Visible = true;
            frmSeriesReg.cbxStation.Enabled = true;
            frmSeriesReg.ShowDialog();
        }

        public void AxisGridToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            if (ReferenceEquals(frmAxisGridReg, null))
            {
                frmAxisGridReg = new frmAxisGrid(this);
                frmAxisGridReg.Owner = this;
            }
            if (frmAxisGridReg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //applying the selected configuration
                    applyAxisConfiguration();
                    PictureBox null_PictureBox = null;
                    plot.draw(true, ref null_PictureBox);
                }
                catch (CerrorRegister err)
                {
                    err.showError();
                }
            }
        }

        public void Form1_Resize(object sender, System.EventArgs e)
        {
            reDraw();
            setMediaBar(playTime, recordTime);
        }

        public void Form1_ResizeEnd(object sender, System.EventArgs e)
        {
            reDraw();
            setMediaBar(playTime, recordTime);
        }

        public void Form1_SizeChanged(object sender, System.EventArgs e)
        {
            reDraw();
            setMediaBar(playTime, recordTime);
        }

        public void tsbPlay_Click(System.Object sender, System.EventArgs e)
        {
            action(cAction.A_PLAY);
        }

        public void tsbPause_Click(System.Object sender, System.EventArgs e)
        {
            action(cAction.A_PAUSE);
        }

        public void tsbStop_Click(System.Object sender, System.EventArgs e)
        {
            action(cAction.A_STOP);
        }

        public void tsbRecord_Click(System.Object sender, System.EventArgs e)
        {
            action(cAction.A_RECORD);
        }

        public void PrintToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            printCurrentPlot();
        }

        //Private Sub ToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        //    printPreviewCurrentPlot()
        //End Sub

        public void prntPlot_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //printing the picture box
            PictureBox auxPcb = new PictureBox();
            auxPcb.Visible = false;
            auxPcb.Width = e.MarginBounds.Width;
            auxPcb.Height = e.MarginBounds.Height;
            Color prevClr = plot.myBckGnd;
            plot.myBckGnd = Color.White;
            plot.draw(false, ref auxPcb);
            plot.myBckGnd = prevClr;
            e.Graphics.DrawImage(auxPcb.Image, e.MarginBounds.X, e.MarginBounds.Y);
            //If preViewDialog And printFromPreView And Not printDialog Then
            //    e.Cancel = True
            //    printCurrentPlot()
            //ElseIf preViewDialog Then
            //    printFromPreView = True
            //    e.Graphics.DrawImage(auxPcb.Image, e.MarginBounds.X, e.MarginBounds.Y)
            //Else
            //    e.Graphics.DrawImage(auxPcb.Image, e.MarginBounds.X, e.MarginBounds.Y)
            //End If
        }

        public void pcbMediaBar_Click(object sender, System.EventArgs e)
        {
            timeBarClick();
        }

        public void NewToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            newPlot();
        }

        public void ExportToCSVToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            generateCSV();
        }

        public void pcbMain_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //checking it's a right click
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                defaultZoom();
            }
        }

        public void pcbMain_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (zoomStatus)
            {
                //initializing the zoom rectangle
                zoomP1 = e.Location;
                zoomInit = true;
            }
        }

        // VBConversions Note: Former VB static variables moved to class level because they aren't supported in C#.
        private Point pcbMain_MouseMove_lastPos = new Point(0, 0);

        public void pcbMain_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // static Point lastPos = new Point(0, 0); VBConversions Note: Static variable moved to class level and renamed pcbMain_MouseMove_lastPos. Local static variables are not supported in C#.
            if (zoomStatus & e.Button == System.Windows.Forms.MouseButtons.Left && (Math.Abs(e.X - pcbMain_MouseMove_lastPos.X) > 2 || Math.Abs(e.Y - pcbMain_MouseMove_lastPos.Y) > 2))
            {
                //setting the second point
                pcbMain_MouseMove_lastPos = e.Location;
                zoomP2 = e.Location;
                reDraw();
            }
        }

        public void pcbMain_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (zoomStatus && zoomInit)
            {
                //finishing the zoom rectangle
                zoomP2 = e.Location;
                zoomFinish = true;
                zoomInit = false;
                zoomStatus = false;
                Cursor = Cursors.Default;
                if (zoomP2.X > zoomP1.X | zoomP2.Y > zoomP1.Y)
                {
                    calculateZoom(zoomP1, zoomP2);
                }
                reDraw();
            }

            if (coordStatus)
            {
                //getting the pixel x coord
                coordTime = e.X;
                coordSelected = true;
                setFormCoords();
            }
        }

        public void tsbXY_Click(System.Object sender, System.EventArgs e)
        {
            //setting or unsetting the coord status
            activeCoordStatus(!tsbXY.Checked);
        }

        public void pcbMediaBar_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            timeBarMouseMove();
        }

        public void TitleToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            //changing the plot title
            string newTitle = Interaction.InputBox(Localization.getResStr(ManRegGlobal.regEnterPlotTitleId), Localization.getResStr(ManRegGlobal.regPlotTitleId), System.Convert.ToString(plot.myTitle));
            if (!string.IsNullOrEmpty(newTitle))
            {
                plot.myTitle = newTitle;
                reDraw();
            }
        }

        public void OpenToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            loadLBR();
        }

        public void bckLongProc_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                //parsing the params
                tBckParams param = (tBckParams)e.Argument;

                //depending on the argument value
                if (param.whatToDo == cBckWorks.BCK_IMPORT_CSV)
                {
                    plot.importFromCSV(param.fileName);
                }
                else if (param.whatToDo == cBckWorks.BCK_EXPORT_CSV)
                {
                    plot.exportToCSV(param.fileName);
                }
                else if (param.whatToDo == cBckWorks.BCK_EXPORT_LBR)
                {
                    plot.saveToLBR(param.fileName);
                }
                else if (param.whatToDo == cBckWorks.BCK_IMPORT_LBR)
                {
                    plot.loadFromLBR(param.fileName);
                }

                e.Result = e.Argument;
            }
            catch (CerrorRegister err)
            {
                err.showError();
            }
        }

        public void bckLongProc_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //parsing the params
            tBckParams param = (tBckParams)e.Result;

            if (param.whatToDo == cBckWorks.BCK_EXPORT_CSV |
                param.whatToDo == cBckWorks.BCK_IMPORT_CSV |
                param.whatToDo == cBckWorks.BCK_EXPORT_LBR |
                param.whatToDo == cBckWorks.BCK_IMPORT_LBR)
            {
                //hiding the wait form
                frmWaitReg.Hide();
                this.Enabled = true;

                //setting the record time
                recordTime = Convert.ToUInt64(plot.getSerieLastTimeValue()) * 1000;
                setMediaBar(0, recordTime);

                //updating the plot area
                reDraw();
            }
        }

        public void WizardToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            if (ReferenceEquals(frmWizard1Reg, null))
            {
                frmWizard1Reg = new frmWizard1(this, jbc);
                frmWizard1Reg.Owner = this;
                frmWizard2Reg = new frmWizard2(this, jbc);
                frmWizard2Reg.Owner = this;
            }
            if (frmWizard1Reg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    //applying the selected configuration
                    applyAxisConfiguration();
                    PictureBox null_PictureBox = null;
                    plot.draw(true, ref null_PictureBox);
                }
                catch (CerrorRegister err)
                {
                    err.showError();
                }
            }
        }

        public void SaveAsToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            generateLBR();
        }

        private void SaveToolStripMenuItem1_Click(System.Object sender, System.EventArgs e)
        {

        }

        public void OptionsToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            if (ReferenceEquals(frmOptionsReg, null))
            {
                frmOptionsReg = new frmOptions(this, jbc);
                frmOptionsReg.Owner = this;
            }
            frmOptionsReg.ShowDialog();
        }

        private void initZoom()
        {
            //initializing the zoom status
            if (!zoomStatus)
            {
                Cursor = Cursors.Cross;
            }
            if (zoomStatus)
            {
                Cursor = Cursors.Default;
            }
            zoomStatus = !zoomStatus;
            zoomInit = false;
            zoomFinish = false;
        }

        private void defaultZoom()
        {
            zoomFinish = true;
            zoomInit = false;
            zoomStatus = false;
            Cursor = Cursors.Default;
            applyAxisConfiguration(false);
            reDraw();
        }

        public void tsbZooms_ButtonClick(System.Object sender, System.EventArgs e)
        {
            initZoom();
        }

        public void tsbDefaultZoom_Click(System.Object sender, System.EventArgs e)
        {
            defaultZoom();
        }

        public void XYToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            //setting the zoom type to XY
            zoomType = cZoomType.XY;
            tsbZooms.Image = XYToolStripMenuItem.Image;
            initZoom();
        }

        public void XToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            //setting the zoom type to X
            zoomType = cZoomType.X;
            tsbZooms.Image = XToolStripMenuItem.Image;
            initZoom();
        }

        public void YToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            //setting the zoom type to Y
            zoomType = cZoomType.Y;
            tsbZooms.Image = YToolStripMenuItem.Image;
            initZoom();
        }

        private async void tmrTrigger_Tick(object sender, System.EventArgs e)
        {
            bool init = false;
            bool finish = false;

            //updating label if required
            if ((Cplot.cTrigger)lblTrigger.Tag != plot.myTrigger)
            {
                updateTriggerLabel();
                lblTrigger.Tag = plot.myTrigger;
            }

            //checking trigger condition
            Cplot.tCheckTrigger resCheckTrigger = await plot.checkTrigger(triggerStationID, triggerPort);
            init = resCheckTrigger.init;
            finish = resCheckTrigger.finish;

            //Debug.Print(String.Format("Init:{0} Finish:{1} triggerStationID:{2} triggerPort:{3}", init, finish, triggerStationID, triggerPort))
            if (init)
            {
                action(cAction.A_RECORD);
            }
            if (finish)
            {
                action(cAction.A_STOP);
            }

            //setting the Reset trigger button
            if (plot.myTrigger == Cplot.cTrigger.TRG_SINGLE)
            {
                tsbResetTrigger.Enabled = !plot.myTriggerReady;
            }
            else
            {
                tsbResetTrigger.Enabled = false;
            }
        }

        private void updateTriggerLabel()
        {
            if (plot != null)
            {
                lblTrigger.Text = ManRegGlobal.getTriggerText(plot.myTrigger);
            }
        }

        public void tsbResetTrigger_Click(System.Object sender, System.EventArgs e)
        {
            if (plot.myTriggerReady == false)
            {
                plot.myTriggerReady = true;
                tsbResetTrigger.Enabled = false;
            }
        }

        public void LoadToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            if (ReferenceEquals(frmTemplateReg, null))
            {
                frmTemplateReg = new frmTemplate(this, jbc);
                frmTemplateReg.Owner = this;
            }
            frmTemplateReg.ShowDialog();
        }

        public void SaveToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            generateTemplate();
        }

        // Drag and Drop station
        public void pcbMain_DragEnter(System.Object sender, System.Windows.Forms.DragEventArgs e)
        {
            string sData = "";
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // If the data is a file, display the copy cursor.
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                // If the data is a station ID, display the copy cursor.
                sData = System.Convert.ToString(e.Data.GetData(DataFormats.Text));
                if (sData.IndexOf("ID=") + 1 > 0)
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        public void pcbMain_DragDrop(System.Object sender, System.Windows.Forms.DragEventArgs e)
        {
            string sData = "";
            ulong myID = default(ulong);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // load file
                string[] aFiles = (string[])(e.Data.GetData(DataFormats.FileDrop));
                string sFileName = "";
                sFileName = aFiles[0];
                if (sFileName.ToLower().IndexOf(".lbr") >= 0)
                {
                    loadLBRFile(sFileName);
                }
                else if (sFileName.ToLower().IndexOf(".csv") >= 0)
                {
                    loadCSVFile(sFileName);
                }
                else
                {
                    MessageBox.Show(Localization.getResStr(ManRegGlobal.regMsgExtNotValidId));
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                // If the data is a station ID, display the copy cursor.
                sData = System.Convert.ToString(e.Data.GetData(DataFormats.Text));
                if (sData.IndexOf("ID=") + 1 > 0)
                {
                    sData = sData.Replace("ID=", "");
                    try
                    {
                        myID = Convert.ToUInt32(sData);
                        myInitFormSeries();

                        Control oControl = null;
                        // only add for this station
                        if (ManRegGlobal.myControlExists(frmSeriesReg, "cbxStation", ref oControl))
                        {
                            ((ComboBox)oControl).Enabled = false;
                        }
                        frmSeriesReg.sSelectStationID = myID.ToString();

                        // do not permit modifying
                        if (ManRegGlobal.myControlExists(frmSeriesReg, "butEdit", ref oControl))
                        {
                            ((Button)oControl).Visible = false;
                        }

                        frmSeriesReg.ShowDialog();

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Error dragging station.");

                    }
                }
            }


        }
    }
}
