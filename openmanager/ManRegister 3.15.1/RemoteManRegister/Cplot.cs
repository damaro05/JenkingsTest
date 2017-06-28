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
using DataJBC;
// End of VB project level imports

// 06/08/2013 #edu# Template serie data: station model added. tTPTserie.station current format "ID - stnname - stnmodel"
// 09/12/2013 Se añade Farhenheit (1.66.75.1)
// 13/01/2014 Rounding to 1 decimal when exporting to CSV (temperature grades and power %)
// 13/01/2014 New LBR file format
// 25/03/2015 Uso de multiples colas de modo continuo
// 09/04/2015 Environment.TickCount And Int32.MaxValue: se añade 'And Int32.MaxValue' para que devuelva siempre un positivo
//       TickCount cycles between Int32.MinValue, which is a negative number, and Int32.MaxValue once every 49.8 days.
//       This removes the sign bit to yield a nonnegative number that cycles between zero and Int32.MaxValue once every 24.9 days.

namespace RemoteManRegister
{
    public class Cplot
    {
        //type definitions
        public struct tTPTserie
        {
            public string name;
            public int mag;
            public string station;
            public int port;
            public Color clr;
        }

        public struct tTPTmain
        {
            public string title;

            public tAxis temp;
            public tAxis pwr;
            public tAxis time;

            public string tempUnits; // #edu#

            public cTrigger trigger;

            public cSideStart sideStart;

            public List<tTPTserie> series;
        }

        public struct tAxis
        {
            public double min;
            public double max;
            public double stp;
            public double range;
        }

        public struct tCheckTrigger
        {
            public bool init;
            public bool finish;
        }

        public struct tEntry
        {
            public string serieID;
            public int port;
            public int magnitude;
        }

        public struct tStation
        {
            public bool noStation;
            public Cstation station;
            public List<tEntry> entry;
        }

        //Constants
        public const System.Int32 TEMPERATURE = Cstation.TEMPERATURE;
        public const System.Int32 POWER = Cstation.POWER;
        public const System.Int32 FLOW = Cstation.FLOW;
        public const System.UInt64 NO_STATION_ID = UInt64.MaxValue; //Special station ID for series of no station, usually when loading a plot

        private const int TOP_MARGIN = 70;
        private const int LEFT_MARGIN = 70;
        private const int RIGHT_MARGIN = 70;
        private const int BOTTOM_MARGIN = 30;

        private const int TITLE_TEXT_SIZE = 15;
        private const int LEGEND_TEXT_SIZE = 10;
        private const int LEGEND_LINE_WIDTH = 10;
        private const int AXIS_WIDTH = 2;
        private const int GRID_NUM_MARK_WIDTH = 5;
        private const int NUMBER_TEXT_SIZE = 10;
        private const int UNITS_FONT_SIZE = 10;

        private const int POINT_FILTER_GRADE = 6;
        private const int POINT_DIAMETER = 6;
        private const int LINE_WIDTH = 1;

        private const int TIMER_INTERVAL = 50; // redondea a 16 o 32

        public enum cStatus
        {
            STATUS_PLAY,
            STATUS_PAUSE_FROM_PLAY,
            STATUS_PAUSE_FROM_RECORD,
            STATUS_STOP,
            STATUS_RECORD
        }

        public enum cSideStart
        {
            START_RIGHT,
            START_LEFT
        }

        public enum cTrigger
        {
            TRG_AUTO,
            TRG_SINGLE,
            TRG_MANUAL
        }


        //Global var's
        private string title;
        private Color titleClr;
        private Color bckGnd;
        private Color axisTempClr;
        private Color axisPwrClr;
        private Color axisTimeClr;
        private Color gridClr;
        private Color textClr;
        private bool showLegend;
        private string fontName;
        private cSideStart sideStart;
        private int lineWidth;
        private int pointWidth;

        private cStatus status;

        private string tempUnits;
        private string pwrUnits; // power and flow units %
                                 //Private flowUnits As String

        private tAxis tempAxis;
        private tAxis pwrAxis;
        private tAxis timeAxis;
        private tAxis[] auxTimeAxis = new tAxis[2];
        private int rulerAxis;

        private Rectangle plotRect; //the plot area rectangle
        private Cbuffer areaBMP; //where the plot area margins are drawn
        private Cbuffer gridBMP; //the grid bitmap
        private Cbuffer plotBMP; //the plot area bitmap
        private Cbuffer customBMP; //a bitmap exteriorly accessible for user custom drawing
        private Cbuffer[] auxPlotBMP = new Cbuffer[2]; //the vector of buffers for the RT plot scrolling
        private Cbuffer fullBMP; //the bitmap where the composition of all others will be done

        private Timer tmr; //the timer for the RT plot process, add and show serie points
        private ulong startTime; //the initial time of the sequence
        private ulong lastTickTime; //the last time the timer "ticks"
        private double totalTimeOffset; //the total time offset for when user moves the time window.
        private ulong initPause; //the time instant when the pause has been set
        private bool tmr_TickStopped = false; //true if the timer was stopped inside the tmr_Tick function

        private cTrigger trigger; //the current trigger type
        private bool trieggerReady; //when single or auto trigger must detect only once until it is stopped

        private PictureBox pcbPlot; //the picturebox object where to draw reference
                                    //Private GFX As Graphics                    'the graphics object associated to the picturebox
        private int myFrmID; //ID of the form containing the plot, to control duplicate station plotting
        private JBC_API_Remote jbc = null;

        private List<Cserie> series = new List<Cserie>();
        private List<tStation> stations = new List<tStation>();

        //Properties
        public dynamic myTitle
        {
            get
            {
                return title;
            }
            set
            {
                title = System.Convert.ToString(value);
            }
        }
        public Color myTitleClr
        {
            get
            {
                return titleClr;
            }
            set
            {
                titleClr = value;
            }
        }
        public dynamic myShowLegend
        {
            get
            {
                return showLegend;
            }
            set
            {
                showLegend = System.Convert.ToBoolean(value);
            }
        }
        public dynamic myFontName
        {
            get
            {
                return fontName;
            }
            set
            {
                fontName = System.Convert.ToString(value);
            }
        }
        public dynamic myRulerAxis
        {
            get
            {
                return rulerAxis;
            }
            set
            {
                rulerAxis = System.Convert.ToInt32(value);
            }
        }
        public dynamic myTempUnits
        {
            get
            {
                return tempUnits;
            }
            set
            {
                tempUnits = System.Convert.ToString(value);
            }
        }
        public dynamic myPwrUnits
        {
            get
            {
                return pwrUnits;
            }
            set
            {
                pwrUnits = System.Convert.ToString(value);
            }
        }
        public Color myBckGnd
        {
            get
            {
                return bckGnd;
            }
            set
            {
                bckGnd = value;
            }
        }
        public Color myTempAxisClr
        {
            get
            {
                return axisTempClr;
            }
            set
            {
                axisTempClr = value;
            }
        }
        public Color myPwrAxisClr
        {
            get
            {
                return axisPwrClr;
            }
            set
            {
                axisPwrClr = value;
            }
        }
        public Color myTimeAxisClr
        {
            get
            {
                return axisTimeClr;
            }
            set
            {
                axisTimeClr = value;
            }
        }
        public Color myGridClr
        {
            get
            {
                return gridClr;
            }
            set
            {
                gridClr = value;
            }
        }
        public Color myTextClr
        {
            get
            {
                return textClr;
            }
            set
            {
                textClr = value;
            }
        }
        public cSideStart mySideStart
        {
            get
            {
                return sideStart;
            }
            set
            {
                sideStart = value;
            }
        }
        public cTrigger myTrigger
        {
            get
            {
                return trigger;
            }
            set
            {
                trigger = value;
            }
        }
        public bool myTriggerReady
        {
            get
            {
                return trieggerReady;
            }
            set
            {
                trieggerReady = value;
            }
        }
        public int myLineWidth
        {
            get
            {
                return lineWidth;
            }
            set
            {
                lineWidth = value;
            }
        }

        public int frmID
        {
            get
            {
                return myFrmID;
            }
        }

        public int myPointWidth
        {
            get
            {
                return pointWidth;
            }
            set
            {
                pointWidth = value;
            }
        }

        //callback declarations
        public delegate void tBitmapCustomCbk(ref Cbuffer buf, ref Rectangle pRect);
        private tBitmapCustomCbk bmpCustom;

        //Public functions
        public Cplot(PictureBox pcb, int frmID, JBC_API_Remote _jbc)
        {
            //assigning the picturebox object and creating its graphics
            pcbPlot = pcb;
            //GFX = pcbPlot.CreateGraphics()
            jbc = _jbc;
            myFrmID = frmID;

            //creating the bitmaps
            areaBMP = new Cbuffer(1, 1);
            gridBMP = new Cbuffer(1, 1);
            plotBMP = new Cbuffer(1, 1);
            customBMP = new Cbuffer(1, 1);
            auxPlotBMP[0] = new Cbuffer(1, 1);
            auxPlotBMP[1] = new Cbuffer(1, 1);
            fullBMP = new Cbuffer(1, 1);

            //creating the timers
            tmr = new Timer();
            tmr.Tick += tmr_Tick;

            //default values
            tmr.Interval = TIMER_INTERVAL;
            status = cStatus.STATUS_STOP;
            title = "Untitled";
            showLegend = true;
            fontName = "MS Sans Seriff";
            rulerAxis = TEMPERATURE;
            tempUnits = ManRegGlobal.tempunitCELSIUS; // edu
            pwrUnits = "%";
            configTempAxis(0, 400, 25);
            configPwrAxis(-10, 110, 10);
            configTimeAxis(-10, 0, 1);
            setDefaultColors();
            lineWidth = LINE_WIDTH;
            pointWidth = POINT_DIAMETER;
            sideStart = cSideStart.START_LEFT;
            trigger = cTrigger.TRG_MANUAL;
            trieggerReady = true;
        }

        public void dispose()
        {
            foreach (Cserie s in series)
            {
                s.dispose();
            }
            series.Clear();
            foreach (tStation s in stations)
            {
                if (!s.noStation)
                {
                    s.station.finish();
                }
            }
            stations.Clear();
        }

        public bool startContinuousMode()
        {
            //Dim lstSeriesStations As New List(Of ULong)
            //getListOfSerieStationID(lstSeriesStations)

            //' check if there is a station already plotting in another window
            //For Each connStn As cConnectedStation In connectedStations
            //    If lstSeriesStations.Contains(connStn.ID) Then
            //        ' if station plotting from another window, cannot plot
            //        If connStn.frmPlottingID >= 0 And connStn.frmPlottingID <> myFrmID Then
            //            Return False
            //        End If
            //    End If
            //Next

            //For Each stationId As ULong In lstSeriesStations
            //    startStationContinuousMode(stationId, speed)
            //Next

            // start continuous mode queue instances
            foreach (tStation s in stations)
            {
                if (!s.noStation)
                {
                    s.station.startStationContinuousMode();
                }
            }

            return true;
        }

        //Private Sub startStationContinuousMode(ByVal ID As ULong, ByVal speed As JBC_API_Remote.cSpeedContinuousMode)
        //    ' Setting the station in continuous mode for all of its ports
        //    Dim count As Integer = jbc.GetPortCount(ID)
        //    Dim a As Integer = JBC_API_Remote.cPort.NO_PORT
        //    Dim b As Integer = JBC_API_Remote.cPort.NO_PORT
        //    Dim c As Integer = JBC_API_Remote.cPort.NO_PORT
        //    Dim d As Integer = JBC_API_Remote.cPort.NO_PORT
        //    If count >= 1 Then a = JBC_API_Remote.cPort.NUM_1
        //    If count >= 2 Then b = JBC_API_Remote.cPort.NUM_2
        //    If count >= 3 Then c = JBC_API_Remote.cPort.NUM_3
        //    If count >= 4 Then d = JBC_API_Remote.cPort.NUM_4
        //    jbc.SetContinuousMode(ID, speed, a, b, c, d)
        //    'Debug.Print("startStationContinuousMode")
        //    ' mark as plotting
        //    For Each connStn As cConnectedStation In connectedStations
        //        If connStn.ID = ID Then
        //            connStn.frmPlottingID = myFrmID
        //            Exit For
        //        End If
        //    Next
        //End Sub

        public bool stopContinuousMode()
        {
            //Dim lstSeriesStations As New List(Of ULong)
            //Dim lstSeriesStationsConnected As New List(Of ULong)
            //getListOfSerieStationID(lstSeriesStations)

            //' check if there is a station already plotting in another window
            //For Each connStn As cConnectedStation In connectedStations
            //    If lstSeriesStations.Contains(connStn.ID) Then
            //        lstSeriesStationsConnected.Add(connStn.ID)
            //        ' if station plotting from another window, cannot stop plotting
            //        If connStn.frmPlottingID >= 0 And connStn.frmPlottingID <> myFrmID Then
            //            Return False
            //        End If
            //    End If
            //Next

            //' stop continuous mode only for connected stations
            //For Each stationId As ULong In lstSeriesStationsConnected
            //    stopStationContinuousMode(stationId)
            //Next

            // stop continuous mode queue instances
            foreach (tStation s in stations)
            {
                if (!s.noStation)
                {
                    s.station.stopStationContinuousMode();
                }
            }

            return true;
        }

        //Private Sub stopStationContinuousMode(ByVal ID As ULong)
        //     Stopping station continuous mode
        //    Debug.Print("stopStationContinuousMode")
        //    jbc.SetContinuousMode(ID, JBC_API_Remote.cSpeedContinuousMode.OFF)
        //     mark as not plotting
        //    For Each connStn As cConnectedStation In connectedStations
        //        If connStn.ID = ID Then
        //            connStn.frmPlottingID = -1
        //            Exit For
        //        End If
        //    Next
        //End Sub

        // -- Series --
        public void addSerie(string serieName, ulong stationID, int port, int mag, Color clr, bool pts, bool lines, string legend)
        {

            //checking parametters are correct
            if (mag != POWER & mag != TEMPERATURE & mag != FLOW)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_BAD_MAGNITUDE_PARAM));
            }
            if (port < 1 | port > 4)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_BAD_PORT_PARAM));
            }
            foreach (Cserie s in series)
            {
                if ((string)s.myID == serieName)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_SERIE_NAME_REPEATED));
                }
            }

            //creating the station and the entry
            createStationAndEntry(stationID, serieName, port, mag);

            //creating the serie
            Cserie serie = new Cserie(serieName, mag);
            serie.myColor = clr;
            serie.myShowLines = lines;
            serie.myShowPoints = pts;
            if (legend != "")
            {
                serie.myLegend = legend;
            }
            series.Add(serie);
        }

        public void editSerie(string name, ulong stationID, int port, int mag, Color clr, bool pts, bool lines, string legend = "", string newName = "")
        {
            //looking for the serie
            int i = getSerieIndex(name);

            //getting its entry to erase it and creting a new one
            int station = 0;
            int entry = 0;
            getStationEntry(name, ref station, ref entry);
            stations[station].entry.RemoveAt(entry);
            if (newName != "")
            {
                series[i].myID = newName;
                createStationAndEntry(stationID, newName, port, mag);
            }
            else
            {
                createStationAndEntry(stationID, name, port, mag);
            }

            //updating the serie configuration
            series[i].myAxis = mag;
            if (legend != "")
            {
                series[i].myLegend = legend;
            }
            series[i].myShowLines = lines;
            series[i].myShowPoints = pts;
            series[i].myColor = clr;
        }

        public void removeSerie(string name)
        {
            //looking for the serie
            int i = getSerieIndex(name);

            //removing the serie
            series[i].dispose();
            series.RemoveAt(i);

            //looking for the entry
            int station = 0;
            int entry = 0;
            getStationEntry(name, ref station, ref entry);

            //removing the entry
            stations[station].entry.RemoveAt(entry);

            //if the station has no more entries erasing it
            if (stations[station].entry.Count == 0)
            {
                if (!stations[station].noStation)
                {
                    stations[station].station.finish();
                }
                stations.RemoveAt(station);
            }
        }

        public void removeSeriesWithStation(ulong stationID)
        {
            int station = 0;
            int entry = 0;
            List<string> removeList = new List<string>();
            foreach (Cserie s in series)
            {
                getStationEntry(System.Convert.ToString(s.myID), ref station, ref entry);
                if (stations[station].station.myStationID == stationID)
                {
                    removeList.Add(s.myID);
                }
            }
            foreach (string s in removeList)
            {
                removeSerie(s);
            }
        }

        public void clearSeries()
        {
            while (series.Count > 0)
            {
                removeSerie(System.Convert.ToString(series[0].myID));
            }
        }

        public void getListOfSerieName(List<string> lst)
        {
            lst.Clear();
            foreach (Cserie s in series)
            {
                lst.Add(s.myID);
            }
        }

        public void getSerieConfig(string name, ref ulong stationID, ref int port, ref int magnitude, ref Color clr, ref bool pts, ref bool lines, ref string legend)
        {
            //looking for the serie
            int i = getSerieIndex(name);
            int j = 0;
            int k = 0;
            getStationEntry(name, ref j, ref k);
            //getting the serie values
            if (stations[j].noStation)
            {
                stationID = NO_STATION_ID;
            }
            else
            {
                stationID = stations[j].station.myStationID;
            }
            port = System.Convert.ToInt32(stations[j].entry[k].port);
            magnitude = System.Convert.ToInt32(stations[j].entry[k].magnitude);
            clr = series[i].myColor;
            pts = System.Convert.ToBoolean(series[i].myShowPoints);
            lines = System.Convert.ToBoolean(series[i].myShowLines);
            legend = System.Convert.ToString(series[i].myLegend);
        }

        public double getSerieLastTimeValue()
        {
            if (series.Count > 0)
            {
                double max = System.Convert.ToDouble(series[0].myLastTime);
                foreach (Cserie s in series)
                {
                    if (System.Convert.ToDouble(s.myLastTime) > max)
                    {
                        max = System.Convert.ToDouble(s.myLastTime);
                    }
                }
                return max;
            }
            else
            {
                return 0.0;
            }
        }

        public void getListOfSerieStationID(List<ulong> lst)
        {
            lst.Clear();
            foreach (tStation s in stations)
            {
                if (!s.noStation)
                {
                    lst.Add(s.station.myStationID);
                }
            }
        }

        private void createStationAndEntry(ulong stationID, string serieName, int port, int mag)
        {
            //looking if the station ID is already created
            bool found = false;
            int i = 0;
            while (i <= stations.Count - 1 && !found)
            {
                if (stations[i].noStation)
                {
                    if (stationID == NO_STATION_ID)
                    {
                        found = true;
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    if (stations[i].station.myStationID == stationID)
                    {
                        found = true;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            if (!found)
            {
                //the station is not created, creating it
                tStation station = new tStation();
                if (stationID != NO_STATION_ID)
                {
                    eStationType stationType = jbc.GetStationType((long)stationID); // hadesold
                    station.station = new Cstation();
                    station.station.init(stationID, stationType, jbc); // hadesold
                    station.noStation = false;
                }
                else
                {
                    station.noStation = true;
                }
                station.entry = new List<tEntry>();
                stations.Add(station);
                i = stations.Count - 1;
            }

            //adding an entry to the proper station
            tEntry entry = new tEntry();
            entry.magnitude = mag;
            entry.port = port;
            entry.serieID = serieName;
            stations[i].entry.Add(entry);
        }

        // -- Axis --
        public void configTempAxis(double min, double max, double stp)
        {
            tempAxis.min = min;
            tempAxis.max = max;
            tempAxis.stp = stp;
        }

        public void configPwrAxis(double min, double max, double stp)
        {
            pwrAxis.min = min;
            pwrAxis.max = max;
            pwrAxis.stp = stp;
        }

        public void configTimeAxis(double min, double max, double stp)
        {
            timeAxis.min = min;
            timeAxis.max = max;
            timeAxis.stp = stp;
            timeAxis.range = max - min;
        }

        public void configTimeAxis(double range, double stp)
        {
            timeAxis.range = range;
            timeAxis.stp = stp;
            timeAxis.min = timeAxis.max - range;
        }

        public void getAxisConfig(string axis, ref double min, ref double max, ref double stp)
        {
            if (axis == "temp")
            {
                min = tempAxis.min;
                max = tempAxis.max;
                stp = tempAxis.stp;
            }
            else if (axis == "pwr")
            {
                min = pwrAxis.min;
                max = pwrAxis.max;
                stp = pwrAxis.stp;
            }
            else if (axis == "time_val")
            {
                min = timeAxis.min;
                max = timeAxis.max;
                stp = timeAxis.stp;
            }
            else if (axis == "time_range")
            {
                min = timeAxis.max - timeAxis.range;
                max = timeAxis.max;
                stp = timeAxis.stp;
            }
        }

        //-- Tool/utils --
        //the pixel must be in the picturebox rectangle coordinates
        public PointF convertPixToPoint(Point pix, int axis)
        {
            //converting the pixel to the plot rectangle reference
            pix.X = pix.X - plotRect.X;
            pix.Y = pix.Y - plotRect.Y;

            //converting the point
            PointF pt = new PointF();
            if (axis == TEMPERATURE)
            {
                pt = pixelToPoint(pix, tempAxis, timeAxis);
            }
            else if (axis == POWER)
            {
                pt = pixelToPoint(pix, pwrAxis, timeAxis);
            }
            else if (axis == FLOW)
            {
                pt = pixelToPoint(pix, pwrAxis, timeAxis); // edu: use power axis
            }

            //returning the result
            return pt;
        }

        //the pixel X-Coord must be in the picturebox rectangle coordinates
        public double getSerieValueInPos(int xPix, string serieID)
        {
            //getting the time value of the pixel x coord
            Point pix = new Point(xPix - plotRect.X, 0);
            double time = pixelToPoint(pix, tempAxis, timeAxis).X;

            //getting the points around the time value
            System.Collections.Generic.List<PointF> lst = new System.Collections.Generic.List<PointF>();
            series[getSerieIndex(serieID)].getPoints(lst, time - 1, time + 1, myTempUnits, POINT_FILTER_GRADE);

            //if no points, returning a default 0 value
            if (lst.Count == 0)
            {
                return 0;
            }

            //searching the point immediately superior to the time value
            if (lst[0].X <= time)
            {
                int cnt = 0;
                bool found = false;
                while (cnt < lst.Count && !found)
                {
                    if (lst[cnt].X >= time)
                    {
                        found = true;
                    }
                    else
                    {
                        cnt++;
                    }
                }

                //search result
                if (!found)
                {
                    return lst.Last().Y;
                }
                if (found && cnt == 0)
                {
                    return lst.First().Y;
                }
                return ((time - lst[cnt - 1].X) / (lst[cnt].X - lst[cnt - 1].X)) * (lst[cnt].Y - lst[cnt - 1].Y) + lst[cnt - 1].Y;
            }
            else
            {
                return 0;
            }
        }

        // -- Drawing --
        public void setCustomCbk(tBitmapCustomCbk cbk)
        {
            bmpCustom = cbk;
        }

        public void draw(bool initAuxPlotBitmaps, ref PictureBox pcb, bool offset = false, bool plot = false)
        {
            //resizing the graphics object
            //GFX.Dispose()
            //GFX = pcbPlot.CreateGraphics()

            //if passed another picture box, drawing on it
            //Dim oldPcb As PictureBox = Nothing
            //If Not ReferenceEquals(pcb, Nothing) Then
            //    oldPcb = pcbPlot
            //    pcbPlot = pcb
            //End If

            //calculating the plot areas
            if (!ReferenceEquals(pcb, null))
            {
                definePlotRectangle(pcb);
            }
            else
            {
                definePlotRectangle(pcbPlot);
            }
            // definePlotRectangle()

            //setting the aux plot bitmaps and time axis
            if (initAuxPlotBitmaps)
            {
                initializeAuxPlotBitmaps(offset);
            }

            //bitmapping the area, grid and plot
            if (!ReferenceEquals(pcb, null))
            {
                bitmapArea(pcb);
                bitmapGrid(pcb);
                bitmapPlot(pcb);
            }
            else
            {
                bitmapArea(pcbPlot);
                bitmapGrid(pcbPlot);
                bitmapPlot(pcbPlot);
            }
            //bitmapArea()
            //bitmapGrid()
            //bitmapPlot()

            //bitmapping the custom buffer
            bmpCustom.Invoke(ref customBMP, ref plotRect);

            //resizing the full bitmap
            if (!ReferenceEquals(pcb, null))
            {
                fullBMP.resize(pcb.Width, pcb.Height);
            }
            else
            {
                fullBMP.resize(pcbPlot.Width, pcbPlot.Height);
            }
            //fullBMP.resize(pcbPlot.Width, pcbPlot.Height)

            //flipping to the picturebox
            if (!ReferenceEquals(pcb, null))
            {
                flip(pcb);
            }
            else
            {
                flip(pcbPlot);
            }
            //flip()

            //restoring the proper picturebox if necessary
            //If Not ReferenceEquals(pcb, Nothing) Then
            //    pcbPlot = oldPcb
            //End If
        }

        public void setDefaultColors()
        {
            bckGnd = Color.WhiteSmoke;
            axisPwrClr = Color.Black;
            axisTempClr = Color.Black;
            axisTimeClr = Color.Black;
            gridClr = Color.FromArgb(255, 200, 200, 200);
            titleClr = Color.Black;
            textClr = Color.Black;
        }

        //-- Plot control --
        public void myRecord()
        {
            if (status != cStatus.STATUS_PAUSE_FROM_RECORD)
            {
                //new record process, clearing the series
                int station = 0;
                int entry = 0;
                List<string> removeList = new List<string>();
                foreach (Cserie s in series)
                {
                    s.clear();
                    getStationEntry(System.Convert.ToString(s.myID), ref station, ref entry);
                    if (stations[station].noStation)
                    {
                        removeList.Add(s.myID);
                    }
                }
                foreach (string s in removeList)
                {
                    removeSerie(s);
                }

                //initializing the plot if not comming from pause status
                if (sideStart == cSideStart.START_RIGHT)
                {
                    timeAxis.min = System.Convert.ToDouble(-timeAxis.range);
                    timeAxis.max = 0;
                }
                else if (sideStart == cSideStart.START_LEFT)
                {
                    timeAxis.min = 0;
                    timeAxis.max = timeAxis.range;
                }

                //setting initial values
                startTime = (ulong)(Environment.TickCount & int.MaxValue);
                lastTickTime = (ulong)(Environment.TickCount & int.MaxValue);
                totalTimeOffset = 0;

                //redrawing
                PictureBox null_PictureBox = null;
                draw(true, ref null_PictureBox);
            }
            tmr.Start();
            status = cStatus.STATUS_RECORD;
        }

        public void myPlay()
        {
            if (status != cStatus.STATUS_PAUSE_FROM_PLAY)
            {
                //redrawing the plot for reseting and initializing it
                //If sideStart = cSideStart.START_RIGHT Then
                //    timeAxis.min = 0 - timeAxis.range
                //    timeAxis.max = 0
                //ElseIf sideStart = cSideStart.START_LEFT Then
                //    timeAxis.min = 0
                //    timeAxis.max = 0 + timeAxis.range
                //End If

                //setting the initial time axis values
                timeAxis.min = 0;
                timeAxis.max = 0 + timeAxis.range;

                //setting initial values
                startTime = (ulong)(Environment.TickCount & int.MaxValue);
                lastTickTime = (ulong)(Environment.TickCount & int.MaxValue);
                totalTimeOffset = 0;

                //redrawing
                PictureBox temp_pcb = null;
                draw(true, ref temp_pcb, true, false);
            }
            else
            {
                //to avoid time scroll jumps
                lastTickTime = (ulong)(Environment.TickCount & int.MaxValue);

                //to avoid the plot to continue with a false total time applying an offset
                //Dim curTime As ULong = Environment.TickCount And Int32.MaxValue
                //totalTimeOffset = Convert.ToDouble((curTime - initPause)) / 1000.0
            }
            tmr.Start();
            status = cStatus.STATUS_PLAY;
        }

        public void myPause()
        {
            tmr_TickStopped = false;
            tmr.Stop();
            initPause = (ulong)(Environment.TickCount & int.MaxValue) - (ulong)(totalTimeOffset * 1000);
            if (status == cStatus.STATUS_RECORD)
            {
                status = cStatus.STATUS_PAUSE_FROM_RECORD;
            }
            if (status == cStatus.STATUS_PLAY)
            {
                status = cStatus.STATUS_PAUSE_FROM_PLAY;
            }
        }

        public void myStop()
        {
            //stopping the timer
            tmr_TickStopped = false;
            tmr.Stop();
            status = cStatus.STATUS_STOP;

            //redrawing the plot at time 0
            timeAxis.min = 0;
            timeAxis.max = timeAxis.range;
            PictureBox null_PictureBox = null;
            draw(true, ref null_PictureBox);
        }

        public cStatus getPlotStatus()
        {
            return status;
        }

        //-- Files --
        #region Files

        public void saveToLBR(string file)
        {
            System.IO.FileStream fWrite = default(System.IO.FileStream);
            try
            {
                //if the file already exists, deleting it
                if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(file))
                {
                    Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file);
                }

                //opening the file
                fWrite = new System.IO.FileStream(file, System.IO.FileMode.CreateNew);

            }
            catch (Exception ex)
            {
                Interaction.MsgBox(string.Format(Localization.getResStr(ManRegGlobal.regCannotCreateFileId), file, ex.Message), MsgBoxStyle.OkOnly, Localization.getResStr(ManRegGlobal.gralWarningId));
                return;
            }

            saveToLBR_Version2(fWrite);

        }

        public void saveToLBR_Version1(System.IO.FileStream fWrite)
        {
            // save temperature in Celsius

            //getting the double size
            double p = 0;
            byte sizeOfDouble = (byte)(System.Runtime.InteropServices.Marshal.SizeOf(p));

            //writting the first byte as the size of a double
            fWrite.WriteByte(sizeOfDouble);

            //writting the first line as the plot title
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(title);
            fWrite.WriteByte((byte)byteArray.Length);
            fWrite.Write(byteArray, 0, byteArray.Length);

            //writting the series ID, axis and color. And getting the largest serie
            fWrite.WriteByte((byte)series.Count);
            foreach (Cserie serie in series)
            {
                //serie ID (serie name)
                byteArray = System.Text.Encoding.Unicode.GetBytes((char[])serie.myID);
                fWrite.WriteByte((byte)byteArray.Length);
                fWrite.Write(byteArray, 0, byteArray.Length);

                //serie Axis
                fWrite.WriteByte(System.Convert.ToByte(serie.myAxis));

                //serie color
                fWrite.WriteByte(serie.myColor.R);
                fWrite.WriteByte(serie.myColor.G);
                fWrite.WriteByte(serie.myColor.B);
            }

            //getting the serie with the maximum number of points, initializing the point reader and writting the series point count
            int largestSerie = 0;
            byte[] byteUint64 = new byte[8];
            for (int i = 0; i <= series.Count - 1; i++)
            {
                if (series[largestSerie].myPointCount < series[i].myPointCount)
                {
                    largestSerie = i;
                }
                series[i].initPointIndexReader();
                byteUint64 = BitConverter.GetBytes(System.Convert.ToBoolean(series[i].myPointCount));
                fWrite.Write(byteUint64, 0, byteUint64.Length);
            }

            //writting all the series points
            PointF pt = new PointF();
            PointF mainPt = new PointF();
            byte[] byteSingle = new byte[System.Runtime.InteropServices.Marshal.SizeOf(pt.Y) - 1 + 1];
            for (int i = 0; i < (int)series[largestSerie].myPointCount; i++)
            {
                //writting the time value
                series[largestSerie].getNextPoint(pt, false, ManRegGlobal.tempunitCELSIUS, 1);
                byteSingle = BitConverter.GetBytes(pt.X);
                fWrite.Write(byteSingle, 0, byteSingle.Length);

                //getting the largest serie point
                mainPt = pt;

                //writting the diferent serie values
                for (int j = 0; j <= series.Count - 1; j++)
                {
                    if (j != largestSerie)
                    {
                        if ((int)(series[largestSerie].myPointCount - series[j].myPointCount) <= i)
                        {
                            // 13/01/2014 version 1 write temp values in Celsius
                            series[j].getNextPoint(pt, false, ManRegGlobal.tempunitCELSIUS, 1);
                            byteSingle = BitConverter.GetBytes(pt.Y);
                            fWrite.Write(byteSingle, 0, byteSingle.Length);
                        }
                    }
                    else
                    {
                        byteSingle = BitConverter.GetBytes(mainPt.Y);
                        fWrite.Write(byteSingle, 0, byteSingle.Length);
                    }
                }
            }

            //closing the file
            fWrite.Close();
        }

        public void saveToLBR_Version2(System.IO.FileStream fWrite)
        {
            // same as format 1 and temperature in UTI

            //getting the double size
            double p = 0;
            byte sizeOfDouble = (byte)(System.Runtime.InteropServices.Marshal.SizeOf(p) - 1);

            //writting the first byte as the size of a double
            fWrite.WriteByte(sizeOfDouble);

            //writting the first line as the plot title
            byte[] byteArray = System.Text.Encoding.Unicode.GetBytes(title);
            fWrite.WriteByte((byte)byteArray.Length);
            fWrite.Write(byteArray, 0, byteArray.Length);

            //writting the series ID, axis and color. And getting the largest serie
            fWrite.WriteByte((byte)series.Count);
            foreach (Cserie serie in series)
            {
                //serie ID (serie name)
                byteArray = System.Text.Encoding.Unicode.GetBytes((char[])serie.myID);
                fWrite.WriteByte((byte)byteArray.Length);
                fWrite.Write(byteArray, 0, byteArray.Length);

                //serie Axis
                fWrite.WriteByte(System.Convert.ToByte(serie.myAxis));

                //serie color
                fWrite.WriteByte(serie.myColor.R);
                fWrite.WriteByte(serie.myColor.G);
                fWrite.WriteByte(serie.myColor.B);
            }

            //getting the serie with the maximum number of points, initializing the point reader and writting the series point count
            int largestSerie = 0;
            byte[] byteUint64 = new byte[8];
            for (int i = 0; i <= series.Count - 1; i++)
            {
                if (series[largestSerie].myPointCount < series[i].myPointCount)
                {
                    largestSerie = i;
                }
                series[i].initPointIndexReader();
                byteUint64 = BitConverter.GetBytes(System.Convert.ToBoolean(series[i].myPointCount));
                fWrite.Write(byteUint64, 0, byteUint64.Length);
            }

            //writting all the series points
            PointF pt = new PointF();
            PointF mainPt = new PointF();
            byte[] byteSingle = new byte[System.Runtime.InteropServices.Marshal.SizeOf(pt.Y) - 1 + 1];
            for (int i = 0; i < (int)series[largestSerie].myPointCount; i++)
            {
                //writting the time value
                series[largestSerie].getNextPoint(pt, false, ManRegGlobal.tempunitUTI, 1);
                byteSingle = BitConverter.GetBytes(pt.X);
                fWrite.Write(byteSingle, 0, byteSingle.Length);

                //getting the largest serie point
                mainPt = pt;

                //writting the diferent serie values
                for (int j = 0; j <= series.Count - 1; j++)
                {
                    if (j != largestSerie)
                    {
                        if ((int)(series[largestSerie].myPointCount - series[j].myPointCount) <= i)
                        {
                            // 13/01/2014 if axis is temp, save in LBR as UTI
                            series[j].getNextPoint(pt, false, ManRegGlobal.tempunitUTI, 1);
                            byteSingle = BitConverter.GetBytes(pt.Y);
                            fWrite.Write(byteSingle, 0, byteSingle.Length);
                        }
                    }
                    else
                    {
                        byteSingle = BitConverter.GetBytes(mainPt.Y);
                        fWrite.Write(byteSingle, 0, byteSingle.Length);
                    }
                }
            }

            //closing the file
            fWrite.Close();
        }

        public void saveToLBR_Version3(System.IO.FileStream fWrite)
        {
            // ********************
            // ***** PENDING ******
            // ********************
            // new file format and temperature in UTI

            List<byte> bytesList = new List<byte>();
            string sData = "";
            string sEndOfText = '\0' + "\r";

            // header
            bytesList.Clear();
            sData = "JBC Soldering;" + Strings.Format(DateTime.Now, ManRegGlobal.sDatetimeForLBRFormat) + ";R01.6675;lbr_v03";
            bytesList.AddRange(System.Text.Encoding.Unicode.GetBytes(sData.ToCharArray(), 0, sData.Length));
            bytesList.AddRange(System.Text.Encoding.ASCII.GetBytes(sEndOfText.ToCharArray(), 0, sEndOfText.Length));
            fWrite.Write(bytesList.ToArray(), 0, bytesList.Count);

            // plot title
            bytesList.Clear();
            bytesList.AddRange(System.Text.Encoding.Unicode.GetBytes(title.ToCharArray(), 0, title.Length));
            bytesList.AddRange(System.Text.Encoding.ASCII.GetBytes(sEndOfText.ToCharArray(), 0, sEndOfText.Length));
            fWrite.Write(bytesList.ToArray(), 0, bytesList.Count);

            //writting the series ID, axis and color. And getting the largest serie
            // series count
            fWrite.WriteByte((byte)series.Count);
            // base time

            foreach (Cserie serie in series)
            {
                //serie ID (serie name)
                bytesList.Clear();
                bytesList.AddRange(System.Text.Encoding.Unicode.GetBytes((char[])serie.myID, 0, System.Convert.ToInt32(serie.myID.Length)));
                bytesList.AddRange(System.Text.Encoding.ASCII.GetBytes(sEndOfText.ToCharArray(), 0, sEndOfText.Length));
                fWrite.Write(bytesList.ToArray(), 0, bytesList.Count);

                //serie Axis
                fWrite.WriteByte(System.Convert.ToByte(serie.myAxis));

                //serie color
                fWrite.WriteByte(serie.myColor.R);
                fWrite.WriteByte(serie.myColor.G);
                fWrite.WriteByte(serie.myColor.B);
            }

            //getting the serie with the maximum number of points, initializing the point reader and writting the series point count
            int largestSerie = 0;
            byte[] byteUint64 = new byte[8];
            for (int i = 0; i <= series.Count - 1; i++)
            {
                if (series[largestSerie].myPointCount < series[i].myPointCount)
                {
                    largestSerie = i;
                }
                series[i].initPointIndexReader();
                byteUint64 = BitConverter.GetBytes(System.Convert.ToBoolean(series[i].myPointCount));
                fWrite.Write(byteUint64, 0, byteUint64.Length);
            }

            //writting all the series points
            PointF pt = new PointF();
            PointF mainPt = new PointF();
            byte[] byteSingle = new byte[System.Runtime.InteropServices.Marshal.SizeOf(pt.Y) - 1 + 1];
            for (int i = 0; i < (int)series[largestSerie].myPointCount; i++)
            {
                //writting the time value
                series[largestSerie].getNextPoint(pt, false, ManRegGlobal.tempunitUTI, 1);
                byteSingle = BitConverter.GetBytes(pt.X);
                fWrite.Write(byteSingle, 0, byteSingle.Length);

                //getting the largest serie point
                mainPt = pt;

                //writting the diferent serie values
                for (int j = 0; j <= series.Count - 1; j++)
                {
                    if (j != largestSerie)
                    {
                        if ((int)(series[largestSerie].myPointCount - series[j].myPointCount) <= i)
                        {
                            // 13/01/2014 if axis is temp, save in LBR as UTI
                            series[j].getNextPoint(pt, false, ManRegGlobal.tempunitUTI, 1);
                            byteSingle = BitConverter.GetBytes(pt.Y);
                            fWrite.Write(byteSingle, 0, byteSingle.Length);
                        }
                    }
                    else
                    {
                        byteSingle = BitConverter.GetBytes(mainPt.Y);
                        fWrite.Write(byteSingle, 0, byteSingle.Length);
                    }
                }
            }

            //closing the file
            fWrite.Close();
        }

        public void loadFromLBR(string file)
        {
            System.IO.FileStream fRead = default(System.IO.FileStream);

            try
            {
                //opening the file
                fRead = new System.IO.FileStream(file, System.IO.FileMode.Open);
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(string.Format(Localization.getResStr(ManRegGlobal.regCannotOpenFileId), file, ex.Message), MsgBoxStyle.OkOnly, Localization.getResStr(ManRegGlobal.gralWarningId));
                return;
            }

            //reading the file double size
            byte sizeOfDouble = (byte)(fRead.ReadByte());
            double p = 0;
            byte sizeOfDoubleVersion1 = (byte)(System.Runtime.InteropServices.Marshal.SizeOf(p));
            byte sizeOfDoubleVersion2 = (byte)(System.Runtime.InteropServices.Marshal.SizeOf(p) - 1);

            if (sizeOfDouble == sizeOfDoubleVersion1)
            {
                loadFromLBR_Version1(fRead);
            }
            else if (sizeOfDouble == sizeOfDoubleVersion2)
            {
                loadFromLBR_Version2(fRead);
            }
            else
            {
                Interaction.MsgBox(string.Format(Localization.getResStr(ManRegGlobal.regCannotOpenFileId), file, Localization.getResStr(ManRegGlobal.regNotLBRFileId)), MsgBoxStyle.OkOnly, Localization.getResStr(ManRegGlobal.gralWarningId));
                return;
            }

        }

        public void loadFromLBR_Version1(System.IO.FileStream fRead)
        {
            //reading temperatures in Celsius

            //reading the plot title
            byte titleByteLength = (byte)(fRead.ReadByte());
            byte[] byteArray = new byte[titleByteLength - 1 + 1];
            if (fRead.Read(byteArray, 0, titleByteLength) != titleByteLength)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_LOAD_LBR));
            }
            title = System.Text.Encoding.Unicode.GetString(byteArray);

            //reading the series ID, axis and color.
            byte numSeries = (byte)(fRead.ReadByte());
            List<string> serieIDlist = new List<string>();
            for (int i = 0; i <= numSeries - 1; i++)
            {
                //serie ID and axis
                byteArray = new byte[fRead.ReadByte() - 1 + 1];
                if (fRead.Read(byteArray, 0, byteArray.Length) != byteArray.Length)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_LOAD_LBR));
                }
                serieIDlist.Add(System.Text.Encoding.Unicode.GetString(byteArray));
                addSerie(serieIDlist[i], NO_STATION_ID, 1, fRead.ReadByte(), Color.FromArgb(255, fRead.ReadByte(), fRead.ReadByte(), fRead.ReadByte()), false, true, "");
            }

            //reading the series point count and determinating the largest one
            ulong[] ptCount = new ulong[numSeries - 1 + 1];
            byte[] byteUint64 = new byte[8];
            int largestSerie = 0;
            for (int i = 0; i <= numSeries - 1; i++)
            {
                if (fRead.Read(byteUint64, 0, byteUint64.Length) != byteUint64.Length)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_LOAD_LBR));
                }
                ptCount[i] = BitConverter.ToUInt64(byteUint64, 0);
                if (ptCount[i] > ptCount[largestSerie])
                {
                    largestSerie = i;
                }
            }

            //reading the series values
            foreach (Cserie s in series)
            {
                s.initLoadPointWriter();
            }
            PointF pt = new PointF();
            try
            {

                byte[] byteSingle = new byte[System.Runtime.InteropServices.Marshal.SizeOf(pt.Y) - 1 + 1];
                CTemperature auxTemp = new CTemperature(0);
                for (int i = 0; i < (int)ptCount[largestSerie]; i++)
                {
                    //reading the time value
                    fRead.Read(byteSingle, 0, byteSingle.Length);
                    pt.X = BitConverter.ToSingle(byteSingle, 0);

                    //reading the diferent serie values
                    for (int j = 0; j <= numSeries - 1; j++)
                    {
                        if (System.Convert.ToInt32(ptCount[largestSerie] - ptCount[j]) <= i)
                        {
                            fRead.Read(byteSingle, 0, byteSingle.Length);
                            pt.Y = BitConverter.ToSingle(byteSingle, 0);
                            // 13/01/2014 reading temperatures in Celsius
                            series[j].addLoadPoint(pt.X, pt.Y, ManRegGlobal.tempunitCELSIUS);
                        }
                    }
                }
                foreach (Cserie s in series)
                {
                    s.finishLoadPointWriter();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cplot::loadFromLBR_Version1 . error:" + ex.Message);
                //TODO. write in event log
            }

            //closing the file
            fRead.Close();
        }

        public void loadFromLBR_Version2(System.IO.FileStream fRead)
        {
            //like version 1 of file format and temperarure in UTI

            //reading the plot title
            byte titleByteLength = (byte)(fRead.ReadByte());
            byte[] byteArray = new byte[titleByteLength - 1 + 1];
            if (fRead.Read(byteArray, 0, titleByteLength) != titleByteLength)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_LOAD_LBR));
            }
            title = System.Text.Encoding.Unicode.GetString(byteArray);

            //reading the series ID, axis and color.
            byte numSeries = (byte)(fRead.ReadByte());
            List<string> serieIDlist = new List<string>();
            for (int i = 0; i <= numSeries - 1; i++)
            {
                //serie ID and axis
                byteArray = new byte[fRead.ReadByte() - 1 + 1];
                if (fRead.Read(byteArray, 0, byteArray.Length) != byteArray.Length)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_LOAD_LBR));
                }
                serieIDlist.Add(System.Text.Encoding.Unicode.GetString(byteArray));
                addSerie(serieIDlist[i], NO_STATION_ID, 1, fRead.ReadByte(), Color.FromArgb(255, fRead.ReadByte(), fRead.ReadByte(), fRead.ReadByte()), false, true, "");
            }

            //reading the series point count and determinating the largest one
            ulong[] ptCount = new ulong[numSeries - 1 + 1];
            byte[] byteUint64 = new byte[8];
            int largestSerie = 0;
            for (int i = 0; i <= numSeries - 1; i++)
            {
                if (fRead.Read(byteUint64, 0, byteUint64.Length) != byteUint64.Length)
                {
                    throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_LOAD_LBR));
                }
                ptCount[i] = BitConverter.ToUInt64(byteUint64, 0);
                if (ptCount[i] > ptCount[largestSerie])
                {
                    largestSerie = i;
                }
            }

            //reading the series values
            foreach (Cserie s in series)
            {
                s.initLoadPointWriter();
            }
            PointF pt = new PointF();
            byte[] byteSingle = new byte[System.Runtime.InteropServices.Marshal.SizeOf(pt.Y) - 1 + 1];
            for (int i = 0; i < (int)ptCount[largestSerie]; i++)
            {
                //reading the time value
                fRead.Read(byteSingle, 0, byteSingle.Length);
                pt.X = BitConverter.ToSingle(byteSingle, 0);

                //reading the diferent serie values
                for (int j = 0; j <= numSeries - 1; j++)
                {
                    if (System.Convert.ToInt32(ptCount[largestSerie] - ptCount[j]) <= i)
                    {
                        fRead.Read(byteSingle, 0, byteSingle.Length);
                        pt.Y = BitConverter.ToSingle(byteSingle, 0);
                        // 13/01/2014 reading temperatures in UTI
                        series[j].addLoadPoint(pt.X, pt.Y, ManRegGlobal.tempunitUTI);
                    }
                }
            }
            foreach (Cserie s in series)
            {
                s.finishLoadPointWriter();
            }
            //closing the file
            fRead.Close();
        }

        public void exportToCSV(string file)
        {
            //if the file already exists, deleting it
            if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(file))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file);
            }

            //opening the file
            System.IO.StreamWriter fWrite = new System.IO.StreamWriter(file);

            //saving the series data in the file, writting the titles line
            fWrite.WriteLine(myTitle);

            //writting the legends line and getting all the series points
            string line = "Time (s);";
            string axis = "";
            Color clr = new Color();
            string clrStr = "";
            int cnt = 0;
            int maxPoints = 0;
            int largestSerie = 0;
            foreach (Cserie s in series)
            {
                // 13/01/2014 ºC o ªF
                if ((int)s.myAxis == TEMPERATURE)
                {
                    if ((string)myTempUnits == ManRegGlobal.tempunitCELSIUS)
                    {
                        axis = "(" + ManRegGlobal.tempunitCELSIUS + ")";
                    }
                    if ((string)myTempUnits == ManRegGlobal.tempunitFAHRENHEIT)
                    {
                        axis = "(" + ManRegGlobal.tempunitFAHRENHEIT + ")";
                    }
                }
                if ((int)s.myAxis == POWER)
                {
                    axis = "(%)";
                }
                if ((int)s.myAxis == FLOW)
                {
                    axis = "(%F)";
                }
                clr = s.myColor;
                clrStr = "(" + clr.R.ToString() + "-" + clr.G.ToString() + "-" + clr.B.ToString() + ")";
                line = line + System.Convert.ToString(s.myID) + "|" + axis + clrStr + ";";
                s.initPointIndexReader();
                if (maxPoints < (int)s.myPointCount)
                {
                    maxPoints = (int)s.myPointCount;
                    largestSerie = cnt;
                }
                cnt++;
            }
            fWrite.WriteLine(line);

            //writting all the serie's points
            int offset = 0;
            PointF mainPt = new PointF();
            PointF pt = new PointF();
            for (int i = 0; i <= maxPoints - 1; i++)
            {
                series[largestSerie].getNextPoint(mainPt, false, myTempUnits, POINT_FILTER_GRADE);
                line = mainPt.X.ToString() + ";";
                for (int j = 0; j <= series.Count - 1; j++)
                {
                    offset = i - (int)(series[largestSerie].myPointCount - series[j].myPointCount);
                    if (offset >= 0)
                    {
                        if (j != largestSerie)
                        {
                            // 13/01/2014 if axis is temperature, get points in plotter temp units
                            series[j].getNextPoint(pt, false, myTempUnits, POINT_FILTER_GRADE);
                            //line = line & pt.Y & ";"
                            // 13/01/2014 rounding to 1 decimal when exporting to CSV
                            line = line + Math.Round(pt.Y, 1).ToString() + ";";
                        }
                        else
                        {
                            //line = line & mainPt.Y & ";"
                            // 13/01/2014 rounding to 1 decimal when exporting to CSV
                            line = line + Math.Round(mainPt.Y, 1).ToString() + ";";
                        }

                    }
                }
                fWrite.WriteLine(line);
            }

            //closing the file
            fWrite.Close();
        }

        public void importFromCSV(string file)
        {
            //opening the file
            System.IO.StreamReader fRead = Microsoft.VisualBasic.FileIO.FileSystem.OpenTextFileReader(file);

            //preparing for reading the lines
            string line = "";

            //reading the first line, the title
            line = fRead.ReadLine();
            myTitle = line;

            //reading the second line with the series ID's for adding them. Avoiding the first one which is the time value
            line = fRead.ReadLine();
            string[] serieLst = line.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            string[] serieParams = new string[4];
            string[] serieID = new string[serieLst.Count() - 2 + 1];
            int R = 0;
            int G = 0;
            int B = 0;
            string tempUnitsCSV = "";
            for (int i = 1; i <= serieLst.Count() - 1; i++)
            {
                serieParams[0] = System.Convert.ToString(serieLst[i].Substring(0, System.Convert.ToInt32(serieLst[i].LastIndexOf("|"))));
                serieParams[1] = System.Convert.ToString(serieLst[i].Substring(System.Convert.ToInt32(serieLst[i].LastIndexOf("|") + 1), System.Convert.ToInt32(serieLst[i].LastIndexOf("(") - (serieLst[i].LastIndexOf("|") + 1))));
                serieParams[2] = System.Convert.ToString(serieLst[i].Substring(System.Convert.ToInt32(serieLst[i].LastIndexOf("(") + 1), System.Convert.ToInt32(serieLst[i].LastIndexOf(")") - (serieLst[i].LastIndexOf("(") + 1))));
                R = Convert.ToInt16(serieParams[2].Substring(0, System.Convert.ToInt32(serieParams[2].IndexOf("-"))));
                G = Convert.ToInt16(serieParams[2].Substring(System.Convert.ToInt32(serieParams[2].IndexOf("-") + 1), System.Convert.ToInt32(serieParams[2].LastIndexOf("-") - (serieParams[2].IndexOf("-") + 1))));
                B = Convert.ToInt16(serieParams[2].Substring(System.Convert.ToInt32(serieParams[2].LastIndexOf("-") + 1)));
                // 13/01/2014 if axis is temperature, define temp units to load point below
                if (serieParams[1] == "(" + ManRegGlobal.tempunitCELSIUS + ")" || serieParams[1] == "(" + ManRegGlobal.CELSIUS_TEXT + ")")
                {
                    // 13/01/2014 temperature units in Celsius
                    tempUnitsCSV = ManRegGlobal.tempunitCELSIUS;
                    addSerie(serieParams[0], NO_STATION_ID, 1, TEMPERATURE, Color.FromArgb(255, R, G, B), false, true, "");
                }
                else if (serieParams[1] == "(" + ManRegGlobal.tempunitFAHRENHEIT + ")")
                {
                    // 13/01/2014 temperature units in Fahrenheit
                    tempUnitsCSV = ManRegGlobal.tempunitFAHRENHEIT;
                    addSerie(serieParams[0], NO_STATION_ID, 1, TEMPERATURE, Color.FromArgb(255, R, G, B), false, true, "");
                }
                else if (serieParams[1] == "(%)")
                {
                    addSerie(serieParams[0], NO_STATION_ID, 1, POWER, Color.FromArgb(255, R, G, B), false, true, "");
                }
                else if (serieParams[1] == "(%F)")
                {
                    addSerie(serieParams[0], NO_STATION_ID, 1, FLOW, Color.FromArgb(255, R, G, B), false, true, "");
                }
                serieID[i - 1] = serieParams[0];
            }

            //reading all the remaining lines with the points of the series
            string[] points = null;
            int cnt = 0;
            double time = 0;
            while (!fRead.EndOfStream)
            {
                line = fRead.ReadLine();
                points = line.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                //the first value is the time
                time = Convert.ToDouble(points[0]);

                //adding the points to the series
                cnt = 1;
                while (cnt < points.Count())
                {
                    // 13/01/2014 if axis is temperature, add points converting temperature to UTI values
                    series[getSerieIndex(serieID[cnt - 1])].addPoint(time, Convert.ToDouble(points[cnt]), tempUnitsCSV);
                    cnt++;
                }
            }

            //closing the file
            fRead.Close();
        }

        public void saveAsTPT(string file)
        {
            //if the file already exists, deleting it
            if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(file))
            {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(file);
            }

            //opening the file
            System.IO.StreamWriter fWrite = new System.IO.StreamWriter(file);

            //saving the series data in the file, writting the titles line
            // 13/01/2014 se ha añadido las unidades de temperatura detrás del título
            fWrite.WriteLine(myTitle + "###" + System.Convert.ToString(myTempUnits));

            //writting the axis configuration
            string line = "";
            tAxis axis = new tAxis();
            for (int i = 0; i <= 2; i++)
            {
                line = "";
                switch (i)
                {
                    case 0:
                        axis = tempAxis;
                        line = axis.min + ";" + System.Convert.ToString(axis.max) + ";" + System.Convert.ToString(axis.range) + ";" + System.Convert.ToString(axis.stp);
                        break;
                    case 1:
                        axis = pwrAxis;
                        line = axis.min + ";" + System.Convert.ToString(axis.max) + ";" + System.Convert.ToString(axis.range) + ";" + System.Convert.ToString(axis.stp);
                        break;
                    case 2:
                        if (i == 2)
                        {
                            axis = timeAxis;
                        }
                        line = axis.min + ";" + System.Convert.ToString(axis.max) + ";" + System.Convert.ToString(axis.range) + ";" + System.Convert.ToString(axis.stp);
                        break;
                }
                fWrite.WriteLine(line);
            }
            fWrite.WriteLine(myRulerAxis);

            //writting the trigger
            fWrite.WriteLine(myTrigger);

            //writting the plot start side
            fWrite.WriteLine(mySideStart);

            //writting the series name, mag, station, port, color
            int station = 0;
            int entry = 0;
            string stnName = "";
            string stnModel = "";

            try
            {
                foreach (Cserie s in series)
                {
                    getStationEntry(System.Convert.ToString(s.myID), ref station, ref entry);
                    // 06/08/2013 #edu# station model added. Current format "ID - stnname - stnmodel"
                    stnName = jbc.GetStationName(System.Convert.ToInt64(stations[station].station.myStationID)).Trim();
                    if (string.IsNullOrEmpty(stnName))
                    {
                        stnName = Localization.getResStr(ManRegGlobal.regSeriesSerieStationNoNameId);
                    }
                    stnModel = jbc.GetStationModel(System.Convert.ToInt64(stations[station].station.myStationID));
                    line = s.myID + ";" +
                           stations[station].entry[entry].magnitude + ";" +
                           stations[station].station.myStationID + " - " +
                           stnName + " - " +
                           stnModel + ";" +
                           stations[station].entry[entry].port + ";" +
                           System.Convert.ToString(s.myColor.R) + "-" +
                           System.Convert.ToString(s.myColor.G) + "-" +
                           System.Convert.ToString(s.myColor.B);
                    fWrite.WriteLine(line);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cplot::saveAsTPT . error:" + ex.Message);
                //TODO . write in event log
            }

            //closing the file stream
            fWrite.Close();
        }

        public void loadTPT(string file, tTPTmain template)
        {
            //opening the file
            // 13/01/2014 se ha añadido las unidades de temperatura detrás del título
            System.IO.StreamReader fRead = new System.IO.StreamReader(file);

            //reading the title
            // 13/01/2014 se ha añadido las unidades de temperatura detrás del título
            string sTitle = fRead.ReadLine();
            string[] arr = sTitle.Split("###".ToCharArray()[0]);
            template.title = arr[0];
            if (arr.GetUpperBound(0) > 0)
            {
                template.tempUnits = arr[1];
            }
            else
            {
                template.tempUnits = ManRegGlobal.tempunitCELSIUS;
            }

            //reading the axis configuration
            string[] @params = null;
            for (int i = 0; i <= 2; i++)
            {
                @params = fRead.ReadLine().Split(";".ToCharArray());
                if (i == 0)
                {
                    template.temp.min = Convert.ToDouble(@params[0]);
                    template.temp.max = Convert.ToDouble(@params[1]);
                    template.temp.range = Convert.ToDouble(@params[2]);
                    template.temp.stp = Convert.ToDouble(@params[3]);
                }
                else if (i == 1)
                {
                    template.pwr.min = Convert.ToDouble(@params[0]);
                    template.pwr.max = Convert.ToDouble(@params[1]);
                    template.pwr.range = Convert.ToDouble(@params[2]);
                    template.pwr.stp = Convert.ToDouble(@params[3]);
                }
                else if (i == 2)
                {
                    template.time.min = Convert.ToDouble(@params[0]);
                    template.time.max = Convert.ToDouble(@params[1]);
                    template.time.range = Convert.ToDouble(@params[2]);
                    template.time.stp = Convert.ToDouble(@params[3]);
                }
            }
            myRulerAxis = Convert.ToInt32(fRead.ReadLine());

            //reading the trigger
            template.trigger = (cTrigger)(Convert.ToInt32(fRead.ReadLine()));

            //reading the side start
            template.sideStart = (cSideStart)(Convert.ToInt32(fRead.ReadLine()));

            //reading the series name, mag, station, port, color
            template.series = new List<tTPTserie>();
            string line = fRead.ReadLine();
            tTPTserie serie = new tTPTserie();
            string[] clr = null;
            while (!(string.IsNullOrEmpty(line) && fRead.EndOfStream))
            {
                @params = line.Split(";".ToCharArray());
                serie.name = @params[0];
                serie.mag = Convert.ToInt32(@params[1]);
                // 06/08/2013 #edu# station model added. Current format "ID - stnname - stnmodel"
                serie.station = @params[2];
                serie.port = Convert.ToInt32(@params[3]);
                clr = @params[4].Split("-".ToCharArray());
                serie.clr = Color.FromArgb(255, Convert.ToInt32(clr[0]), Convert.ToInt32(clr[1]), Convert.ToInt32(clr[2]));
                template.series.Add(serie);
                line = fRead.ReadLine();
            }

            //closing the stream
            fRead.Close();
        }

        public void applyTPT(tTPTmain template)
        {
            //setting the title
            myTitle = template.title;
            // 13/01/2014 se ha añadido las unidades de temperatura
            myTempUnits = template.tempUnits;

            //setting the axis
            tempAxis = template.temp;
            pwrAxis = template.pwr;
            timeAxis = template.time;

            //trigger
            myTrigger = template.trigger;

            //side start
            mySideStart = template.sideStart;

            //series
            foreach (tTPTserie s in template.series)
            {
                // 06/08/2013 #edu# station model added. Template current format "ID - stnname - stnmodel"
                addSerie(s.name, Convert.ToUInt64(s.station.Split("-".ToCharArray())[0].Trim()), s.port, s.mag, s.clr, false, true, "");
            }

            //redrawing the plot
            PictureBox null_PictureBox = null;
            draw(true, ref null_PictureBox);
        }
        #endregion

        //-- trigger --
        public async System.Threading.Tasks.Task<tCheckTrigger> checkTrigger(ulong stationID, int port)
        {

            tCheckTrigger ret = new tCheckTrigger();

            //declaring previous status
            bool wasSleeping = true;

            //checking for the station
            bool found = false;
            int cnt = 0;
            while (cnt <= stations.Count - 1 && !found)
            {
                if (!stations[cnt].noStation)
                {
                    if (stations[cnt].station.myStationID == stationID)
                    {
                        found = true;
                    }
                    else
                    {
                        cnt++;
                    }
                }
                else
                {
                    cnt++;
                }
            }

            //search result
            if (found)
            {
                //looking if the tool is sleeping
                bool isSleeping = System.Convert.ToBoolean(await stations[cnt].station.getSleepStatus(port));
                //Debug.Print("isSleeping {0} - cont mode: {1}", isSleeping.ToString, isContMode.ToString)

                //proceeding with the trigger detection
                if (trigger == cTrigger.TRG_MANUAL)
                {
                    ret.init = false;
                    ret.finish = false;
                }
                else
                {
                    //single only detect when trigger is ready
                    if (trigger == cTrigger.TRG_SINGLE)
                    {
                        ret.init = wasSleeping && !isSleeping && trieggerReady;

                        //if is true setting to false to accomplish only one trigger untill user wants to detect another
                        if (ret.init && trieggerReady)
                        {
                            trieggerReady = false;
                        }

                    }
                    else if (trigger == cTrigger.TRG_AUTO)
                    {
                        ret.init = wasSleeping && !isSleeping;
                    }

                    //finish is defined for the single trigger
                    if (trigger == cTrigger.TRG_SINGLE)
                    {
                        //waiting until startTime has been initialized by launching the tmr
                        if (startTime > 0)
                        {
                            ret.finish = (timeAxis.range <= Convert.ToDouble((Environment.TickCount & int.MaxValue) - (int)startTime) / 1000.0) && (status == cStatus.STATUS_RECORD);
                        }
                        else
                        {
                            ret.finish = false;
                        }
                    }
                    else
                    {
                        ret.finish = false;
                    }
                }

                //updating the wasSleeping var
                wasSleeping = isSleeping;
            }
            else
            {
                ret.init = false;
                ret.finish = false;
            }

            return ret;
        }

        //Private functions
        private int getSerieIndex(string name)
        {
            //looking for the serie
            bool found = false;
            int i = 0;
            while (i <= series.Count - 1 && !found)
            {
                if (series[i].myID == name)
                {
                    found = true;
                }
                else
                {
                    i++;
                }
            }

            //search result
            if (!found)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_SERIE_NOT_FOUND));
            }

            //retunrning the index
            return i;
        }

        private void getStationEntry(string name, ref int station, ref int entry)
        {
            //searching the serie name in all of the entries of every station
            int s = 0;
            int e = 0;
            bool found = false;
            s = 0;
            while (s <= stations.Count - 1 && !found)
            {
                e = 0;
                while (e <= stations[s].entry.Count - 1 && !found)
                {
                    if (stations[s].entry[e].serieID == name)
                    {
                        found = true;
                    }
                    else
                    {
                        e++;
                    }
                }
                if (!found)
                {
                    s++;
                }
            }

            //search result
            if (!found)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_PLOT_SERIE_ENTRY_NOT_FOUND));
            }

            //assigning the result
            station = s;
            entry = e;
        }

        private void definePlotRectangle(PictureBox pcb)
        {
            //defining the plot rectangle
            plotRect.X = LEFT_MARGIN;
            plotRect.Y = TOP_MARGIN;
            plotRect.Width = pcb.Width - LEFT_MARGIN - RIGHT_MARGIN;
            plotRect.Height = pcb.Height - TOP_MARGIN - BOTTOM_MARGIN;
        }

        private void bitmapArea(PictureBox pcb)
        {
            //resizing the bitmap
            areaBMP.resize(pcb.Width, pcb.Height);

            //clearing teh background
            areaBMP.draw.Clear(bckGnd);

            //preparing the font for the title
            Font fnt = new Font(fontName, TITLE_TEXT_SIZE);

            //writting the title
            SizeF size = areaBMP.draw.MeasureString(title, fnt);
            areaBMP.draw.DrawString(title, fnt, new SolidBrush(titleClr), (float)((double)pcb.Width / 2 - size.Width / 2), 2);

            //if serie's legends are desired
            if (showLegend)
            {
                //preparing the font for the serie's legends
                fnt.Dispose();
                fnt = new Font(fontName, LEGEND_TEXT_SIZE);

                //writting the legends
                Pen pn = default(Pen);
                string axis = "";
                int entry = 0;
                int station = 0;
                string stationAndPort = "";
                Color clr = new Color();
                int yStart = System.Convert.ToInt32(size.Height);
                int wMax = 0;
                int x = 0;
                int y = 0;
                x = plotRect.X;
                y = yStart;
                for (int i = 0; i <= series.Count - 1; i++)
                {
                    //setting coordinates
                    y = (int)(yStart + (i % 2) * size.Height);
                    x = x + wMax * (1 - (i % 2));

                    //reseting the last two legends max width
                    if (i % 2 == 0)
                    {
                        wMax = 0;
                    }

                    if (series[i].myAxis == POWER)
                    {
                        axis = "(" + pwrUnits + ")"; //axis = "(%)"
                    }
                    if (series[i].myAxis == FLOW)
                    {
                        axis = "(" + pwrUnits + ")"; //axis = "(%)"
                    }
                    if (series[i].myAxis == TEMPERATURE)
                    {
                        axis = "(" + tempUnits + ")"; //axis = "(T)"
                    }
                    getStationEntry(System.Convert.ToString(series[i].myID), ref station, ref entry);
                    stationAndPort = ""; // "[" & stations(station).com.myPortName & " - " & stations(station).entry(entry).port & "]"
                    size = areaBMP.draw.MeasureString(axis + stationAndPort + " " + series[i].myLegend, fnt);
                    if (x + size.Width + LEGEND_LINE_WIDTH < plotRect.X + plotRect.Width & y + size.Height < plotRect.Y)
                    {
                        //the legend fits, writting it
                        clr = series[i].myColor;
                        pn = new Pen(clr, 2); // #edu#
                        areaBMP.draw.DrawLine(pn, x, System.Convert.ToInt32(y + size.Height / 2), x + LEGEND_LINE_WIDTH, System.Convert.ToInt32(y + size.Height / 2));
                        areaBMP.draw.DrawString(axis + stationAndPort + " " + series[i].myLegend, fnt, new SolidBrush(textClr), x + LEGEND_LINE_WIDTH, y);
                    }

                    //updating the max width
                    if (size.Width + LEGEND_LINE_WIDTH > wMax)
                    {
                        wMax = LEGEND_LINE_WIDTH + (int)size.Width;
                    }
                }
            }

        }

        private void bitmapGrid(PictureBox pcb)
        {
            //resizing the grid bitmap
            gridBMP.resize(pcb.Width, pcb.Height);

            //clearing the grid bitmap
            gridBMP.draw.Clear(Color.Transparent);

            //drawing the horizontal divisions and numbers
            // calculating steps, depending on the rulerAxis selected
            double tempStp = 0;
            double pwrStp = 0;
            if (rulerAxis == TEMPERATURE)
            {
                tempStp = tempAxis.stp;
                pwrStp = (pwrAxis.max - pwrAxis.min) / (Math.Floor((tempAxis.max - tempAxis.min) / tempAxis.stp));
            }
            else if (rulerAxis == POWER)
            {
                pwrStp = pwrAxis.stp;
                tempStp = (tempAxis.max - tempAxis.min) / (Math.Floor((double)((pwrAxis.max - pwrAxis.min) / pwrAxis.stp)));
            }

            Point pix = new Point();
            SizeF size = new SizeF();
            Font fnt = new Font(fontName, NUMBER_TEXT_SIZE);
            SolidBrush brsh = new SolidBrush(bckGnd);
            //Dim col As Color = Color.FromArgb(255, 200, 200, 200)
            double temp = 0;
            double pwr = 0;
            // drawing temp and pwr scale, depending on the rulerAxis selected
            if (rulerAxis == TEMPERATURE)
            {
                //temp = Math.Ceiling(tempAxis.min / tempAxis.stp) * tempAxis.stp
                //pwr = Math.Ceiling(pwrAxis.min / pwrAxis.stp) * pwrAxis.stp
                temp = tempAxis.min;
                pwr = pwrAxis.min;
                while (temp <= tempAxis.max)
                {
                    //drawing the line
                    // pixel base on temperature
                    pix = pointToPixel(new PointF(0, (float)temp), tempAxis, timeAxis);
                    gridBMP.draw.DrawLine(new Pen(gridClr), plotRect.X - GRID_NUM_MARK_WIDTH, plotRect.Y + pix.Y, plotRect.X + plotRect.Width + GRID_NUM_MARK_WIDTH, plotRect.Y + pix.Y);

                    //drawing the numbers, as long as the gridBMP has transparent bckgnd setting a non transparent bckgnd for the numbers
                    //to be drawn properly
                    // temp
                    size = gridBMP.draw.MeasureString(Convert.ToString(Math.Round(temp, 1)), fnt);
                    gridBMP.draw.FillRectangle(brsh, new Rectangle(System.Convert.ToInt32(plotRect.X - GRID_NUM_MARK_WIDTH - size.Width), System.Convert.ToInt32(plotRect.Y + pix.Y - size.Height / 2), System.Convert.ToInt32(size.Width), System.Convert.ToInt32(size.Height)));
                    gridBMP.draw.DrawString(Convert.ToString(Math.Round(temp, 1)), fnt, new SolidBrush(axisTempClr), plotRect.X - GRID_NUM_MARK_WIDTH - size.Width, plotRect.Y + pix.Y - size.Height / 2);
                    // pwr
                    size = gridBMP.draw.MeasureString(Convert.ToString(Math.Round(pwr, 1)), fnt);
                    gridBMP.draw.FillRectangle(brsh, new Rectangle(plotRect.X + plotRect.Width + GRID_NUM_MARK_WIDTH, System.Convert.ToInt32(plotRect.Y + pix.Y - size.Height / 2), System.Convert.ToInt32(size.Width), System.Convert.ToInt32(size.Height)));
                    gridBMP.draw.DrawString(Convert.ToString(Math.Round(pwr, 1)), fnt, new SolidBrush(axisPwrClr), plotRect.X + plotRect.Width + GRID_NUM_MARK_WIDTH, plotRect.Y + pix.Y - size.Height / 2);
                    temp = temp + tempStp;
                    pwr = pwr + pwrStp;
                }
            }
            else if (rulerAxis == POWER)
            {
                //pwr = Math.Ceiling(pwrAxis.min / pwrAxis.stp) * pwrAxis.stp
                //temp = Math.Ceiling(tempAxis.min / tempAxis.stp) * tempAxis.stp
                pwr = pwrAxis.min;
                temp = tempAxis.min;
                while (pwr <= pwrAxis.max)
                {
                    // pixel base on power
                    pix = pointToPixel(new PointF(0, (float)pwr), pwrAxis, timeAxis);
                    gridBMP.draw.DrawLine(new Pen(gridClr), plotRect.X - GRID_NUM_MARK_WIDTH, plotRect.Y + pix.Y, plotRect.X + plotRect.Width + GRID_NUM_MARK_WIDTH, plotRect.Y + pix.Y);

                    //drawing the numbers, as long as the gridBMP has transparent bckgnd setting a non transparent bckgnd for the numbers
                    //to be drawn properly
                    // temp
                    size = gridBMP.draw.MeasureString(Convert.ToString(Math.Round(temp, 1)), fnt);
                    gridBMP.draw.FillRectangle(brsh, new Rectangle(System.Convert.ToInt32(plotRect.X - GRID_NUM_MARK_WIDTH - size.Width), System.Convert.ToInt32(plotRect.Y + pix.Y - size.Height / 2), System.Convert.ToInt32(size.Width), System.Convert.ToInt32(size.Height)));
                    gridBMP.draw.DrawString(Convert.ToString(Math.Round(temp, 1)), fnt, new SolidBrush(axisTempClr), plotRect.X - GRID_NUM_MARK_WIDTH - size.Width, plotRect.Y + pix.Y - size.Height / 2);
                    // pwr
                    size = gridBMP.draw.MeasureString(Convert.ToString(Math.Round(pwr, 1)), fnt);
                    gridBMP.draw.FillRectangle(brsh, new Rectangle(plotRect.X + plotRect.Width + GRID_NUM_MARK_WIDTH, System.Convert.ToInt32(plotRect.Y + pix.Y - size.Height / 2), System.Convert.ToInt32(size.Width), System.Convert.ToInt32(size.Height)));
                    gridBMP.draw.DrawString(Convert.ToString(Math.Round(pwr, 1)), fnt, new SolidBrush(axisPwrClr), plotRect.X + plotRect.Width + GRID_NUM_MARK_WIDTH, plotRect.Y + pix.Y - size.Height / 2);
                    pwr = pwr + pwrStp;
                    temp = temp + tempStp;
                }

            }

            //drawing the vertical divisions
            double time = Math.Ceiling(timeAxis.min / timeAxis.stp) * timeAxis.stp;
            while (time <= timeAxis.max)
            {
                //drawing the line
                pix = pointToPixel(new PointF((float)time, 0), tempAxis, timeAxis);
                gridBMP.draw.DrawLine(new Pen(gridClr), plotRect.X + pix.X, plotRect.Y, plotRect.X + pix.X, plotRect.Y + plotRect.Height);
                time = time + timeAxis.stp;
            }

            //drawing the three axis
            //Dim pn As New Pen(Color.Black, AXIS_WIDTH)
            gridBMP.draw.DrawLine(new Pen(axisTempClr, AXIS_WIDTH), plotRect.X, plotRect.Y, plotRect.X, plotRect.Y + plotRect.Height);
            gridBMP.draw.DrawLine(new Pen(axisPwrClr, AXIS_WIDTH), plotRect.X + plotRect.Width, plotRect.Y, plotRect.X + plotRect.Width, plotRect.Y + plotRect.Height);
            gridBMP.draw.DrawLine(new Pen(axisTimeClr, AXIS_WIDTH), plotRect.X, plotRect.Y + plotRect.Height, plotRect.X + plotRect.Width, plotRect.Y + plotRect.Height);

            //drawing the axis units and time seconds per division
            string str = "";
            fnt.Dispose();
            fnt = new Font(fontName, UNITS_FONT_SIZE, FontStyle.Bold);

            // seconds per tick
            str = string.Format(Localization.getResStr(ManRegGlobal.regSecondsPerTickId), timeAxis.stp.ToString());
            size = gridBMP.draw.MeasureString(str, fnt);
            gridBMP.draw.FillRectangle(brsh, new Rectangle(System.Convert.ToInt32(plotRect.X + plotRect.Width - size.Width), plotRect.Y + plotRect.Height + 5, System.Convert.ToInt32(size.Width), System.Convert.ToInt32(size.Height)));
            gridBMP.draw.DrawString(str, fnt, new SolidBrush(axisTimeClr), plotRect.X + plotRect.Width - size.Width, plotRect.Y + plotRect.Height + 5);

            // temperature units
            str = tempUnits;
            size = gridBMP.draw.MeasureString(str, fnt);
            gridBMP.draw.FillRectangle(brsh, new Rectangle(5, plotRect.Y, System.Convert.ToInt32(size.Width), System.Convert.ToInt32(size.Height)));
            gridBMP.draw.DrawString(str, fnt, new SolidBrush(axisTempClr), 5, plotRect.Y);

            // power units
            str = pwrUnits;
            size = gridBMP.draw.MeasureString(str, fnt);
            gridBMP.draw.FillRectangle(brsh, new Rectangle(System.Convert.ToInt32(pcb.Width - 5 - size.Width), plotRect.Y, System.Convert.ToInt32(size.Width), System.Convert.ToInt32(size.Height)));
            gridBMP.draw.DrawString(str, fnt, new SolidBrush(axisPwrClr), pcb.Width - 5 - size.Width, plotRect.Y);
        }

        //initializes the scrolling bitmaps depending on the time axis
        private void bitmapPlot(PictureBox pcb)
        {
            //resizing the plot bitmap and the RTplot bitmaps and clearing them
            plotBMP.resize(plotRect.Width, plotRect.Height);
            plotBMP.draw.Clear(Color.Transparent);

            //plotting the serie points
            List<PointF> pts = new List<PointF>();
            Point curPix = new Point();
            Point prevPix = new Point();
            tAxis axis = new tAxis();
            Color clr = new Color();
            foreach (Cserie s in series)
            {
                pts.Clear();
                // 13/01/2014 added myTempUnits to retrieve temperature points converted from UTI
                s.getPoints(pts, timeAxis.min, timeAxis.max, System.Convert.ToString(myTempUnits), POINT_FILTER_GRADE);

                //setting the serie axis
                if ((int)s.myAxis == TEMPERATURE)
                {
                    axis = tempAxis;
                }
                if ((int)s.myAxis == POWER)
                {
                    axis = pwrAxis;
                }
                if ((int)s.myAxis == FLOW)
                {
                    axis = pwrAxis;
                }

                //if the serie has points
                if (pts.Count > 0)
                {
                    //converting and plotting the points
                    prevPix = pointToPixel(pts[0], axis, timeAxis);
                    foreach (PointF p in pts)
                    {
                        //converting points to pixels
                        curPix = pointToPixel(p, axis, timeAxis);
                        if (curPix.Y > plotBMP.bmp.Height)
                        {
                            curPix.Y = plotBMP.bmp.Height;
                        }
                        if (curPix.Y < 0)
                        {
                            curPix.Y = 0;
                        }

                        //plotting the line and point
                        clr = s.myColor;
                        if (s.myShowLines)
                        {
                            plotBMP.draw.DrawLine(new Pen(clr, lineWidth), prevPix, curPix);
                        }
                        if (s.myShowPoints)
                        {
                            plotBMP.draw.FillEllipse(new SolidBrush(clr), curPix.X - pointWidth / 2, curPix.Y - pointWidth / 2, pointWidth, pointWidth);
                        }

                        //setting the curPix as the prevPix
                        prevPix = curPix;
                    }
                }
            }
        }

        private void initializeAuxPlotBitmaps(bool offset = false)
        {
            //resizing
            foreach (Cbuffer bmp in auxPlotBMP)
            {
                bmp.resize(plotRect.Width, plotRect.Height);
                bmp.draw.Clear(Color.Transparent);
            }

            //configuring the RT plot bitmaps time axis. The 0 RTplot is the first shown
            //and the 1 is the second one.
            for (int i = 0; i <= auxTimeAxis.Length - 1; i++)
            {
                auxTimeAxis[i].range = timeAxis.range;
                auxTimeAxis[i].stp = timeAxis.stp;
                if (i == 0)
                {
                    auxTimeAxis[i].min = timeAxis.min;
                    auxTimeAxis[i].max = timeAxis.max;
                    //If sideStart = cSideStart.START_LEFT Then
                    //    auxTimeAxis(i).min = timeAxis.min
                    //    auxTimeAxis(i).max = timeAxis.max
                    //ElseIf sideStart = cSideStart.START_RIGHT Then
                    //    auxTimeAxis(i).min = timeAxis.max
                    //    auxTimeAxis(i).max = timeAxis.max + timeAxis.range
                    //End If
                }
                else if (i == 1)
                {
                    auxTimeAxis[i].min = timeAxis.max;
                    auxTimeAxis[i].max = timeAxis.max + timeAxis.range;
                    //If sideStart = cSideStart.START_LEFT Then
                    //    auxTimeAxis(i).min = timeAxis.max
                    //    auxTimeAxis(i).max = timeAxis.max + timeAxis.range
                    //ElseIf sideStart = cSideStart.START_RIGHT Then
                    //    auxTimeAxis(i).min = timeAxis.max + timeAxis.range
                    //    auxTimeAxis(i).max = timeAxis.max + 2 * timeAxis.range
                    //End If
                }
                //auxPlotBMP(i).draw.DrawLine(Pens.Green, 0, 0, 0, auxPlotBMP(i).bmp.Height)
            }

            //calculating the total time
            //Dim totalTime As Double = Convert.ToDouble((Environment.TickCount And Int32.MaxValue) - startTime) / 1000.0

            //if play or record the totalTime counter can be outside the visible window,
            //changing the start time to make it stay just at the right or left of the time axis window.
            //It can occur when time axis external changes by the user.
            //If offset And status = cStatus.STATUS_PLAY Then
            //    If sideStart = cSideStart.START_LEFT Then totalTimeOffset = (totalTime - timeAxis.min)
            //    If sideStart = cSideStart.START_RIGHT Then totalTimeOffset = (totalTime - timeAxis.max)
            //End If

            //drawing the current points of the series if exists from the min window time to the totalTime
            //drawAuxPlotBitmaps(timeAxis.min, totalTime - totalTimeOffset)
            drawAuxPlotBitmaps(timeAxis.min, timeAxis.max + timeAxis.range);
        }

        //flips the areaBMP, gridBMP and plotBMP to the GFX object
        private void flip(PictureBox pcb)
        {
            //flipping the bitmaps to the full bitmap
            fullBMP.draw.Clear(bckGnd);
            fullBMP.draw.DrawImage(areaBMP.bmp, 0, 0);
            fullBMP.draw.DrawImage(gridBMP.bmp, 0, 0);
            fullBMP.draw.DrawImage(plotBMP.bmp, plotRect.X, plotRect.Y);
            fullBMP.draw.DrawImage(customBMP.bmp, 0, 0);

            //switching the full bitmap to the picture box
            pcb.Image = fullBMP.bmp;
            //pcbPlot.Image = fullBMP.bmp
            //GFX.DrawImage(fullBMP.bmp, 0, 0)
        }

        //the returned value is referenced to the plot rectangle.
        private Point pointToPixel(PointF pt, tAxis axis, tAxis time)
        {
            //defining the pixel
            Point pix = new Point();

            //calculating the x and y coordinate
            pix.X = System.Convert.ToInt32(((pt.X - time.min) / (time.max - time.min)) * plotRect.Width);
            pix.Y = System.Convert.ToInt32(((pt.Y - axis.min) / (axis.max - axis.min)) * plotRect.Height);

            //the y axis is inverted
            pix.Y = plotRect.Height - pix.Y;

            //returning the result
            return pix;
        }

        //the passed pixel must be in the plot rectangle reference
        private PointF pixelToPoint(Point pix, tAxis axis, tAxis time)
        {
            //defining the point
            PointF pt = new PointF();

            //inverting the pixel y coordinate
            pix.Y = plotRect.Height - pix.Y;

            //calculating the x and y coordinate
            pt.X = (float)(((Convert.ToDouble(pix.X) / Convert.ToDouble(plotRect.Width)) * (time.max - time.min)) + time.min);
            pt.Y = (float)(((Convert.ToDouble(pix.Y) / Convert.ToDouble(plotRect.Height)) * (axis.max - axis.min)) + axis.min);

            //returning the result
            return pt;
        }

        private async void tmr_Tick(object sender, System.EventArgs e)
        {
            tmr_TickStopped = true;
            tmr.Stop();
            ulong elapsedTime = (ulong)(DateTime.Now.Ticks);


            //getting the current time
            ulong curTime = (ulong)(Environment.TickCount & int.MaxValue);

            //calculating the total time
            double totalTime = Convert.ToDouble(curTime - startTime) / 1000.0;

            //calculating the scroll time
            double scrollTime = Convert.ToDouble(curTime - lastTickTime) / 1000.0;
            //If scrollTime > Convert.ToDouble(TIMER_INTERVAL) / 1000 * 1.1 Then Debug.Print("scrollTime: " & scrollTime)

            //depending on the status
            if (status == cStatus.STATUS_RECORD)
            {
                await tmrWhenRecord(totalTime, scrollTime);
            }
            if (status == cStatus.STATUS_PLAY)
            {
                tmrWhenPlay(scrollTime);
            }

            //switching the aux plots to the main plot
            plotBMP.draw.Clear(Color.Transparent);
            int x0 = pointToPixel(new PointF((float)(auxTimeAxis[0].min), 0), tempAxis, timeAxis).X;
            int x1 = pointToPixel(new PointF((float)(auxTimeAxis[1].min), 0), tempAxis, timeAxis).X;
            plotBMP.draw.DrawImage(auxPlotBMP[0].bmp, x0, 0);
            plotBMP.draw.DrawImage(auxPlotBMP[1].bmp, x1, 0);

            //flip buffers
            flip(pcbPlot);

            //updating the last tick time
            lastTickTime = curTime;


            elapsedTime = (ulong)(((ulong)DateTime.Now.Ticks - elapsedTime) / 10000);
            int newInterval = (int)(TIMER_INTERVAL - elapsedTime);

            if (newInterval < 1)
            {
                tmr.Interval = 1;
            }
            else
            {
                tmr.Interval = newInterval;
            }

            if (tmr_TickStopped)
            {
                tmr.Start();
            }
            tmr_TickStopped = false;
        }

        private async System.Threading.Tasks.Task tmrWhenRecord(double totalTime, double scrollTime)
        {
            //if required adding points to the series
            if (status == cStatus.STATUS_RECORD)
            {
                await addPointsToSeries(totalTime);
            }

            //if required refreshing the plot

            //applying the scroll to the time axis if necessary
            double tMin = 0;
            double tMax = 0;
            if (totalTime - totalTimeOffset > timeAxis.max | totalTime - totalTimeOffset < timeAxis.min)
            {
                timeAxis.min = timeAxis.min + scrollTime;
                timeAxis.max = timeAxis.max + scrollTime;
                tMin = timeAxis.max - scrollTime;
                tMax = timeAxis.max;
            }
            else if (totalTime - totalTimeOffset < timeAxis.max & totalTime - totalTimeOffset >= timeAxis.min)
            {
                tMin = totalTime - totalTimeOffset - scrollTime;
                tMax = totalTime - totalTimeOffset;
            }
            //Debug.Print("tMin:" & tMin & " tMax:" & tMax & " total:" & totalTime & " offset:" & totalTimeOffset & " axismin:" & timeAxis.min & _
            //                  " axismax:" & timeAxis.max)
            //scrolling the auxiliar plot bmp's time axis
            Font fnt = new Font(fontName, NUMBER_TEXT_SIZE);
            //Dim size As SizeF
            for (int i = 0; i <= 1; i++)
            {
                if (timeAxis.min >= auxTimeAxis[i].max)
                {
                    //the plot is left passed, setting it at the right side
                    auxTimeAxis[i].min = auxTimeAxis[(i + 1) % 2].max;
                    auxTimeAxis[i].max = System.Convert.ToDouble(auxTimeAxis[(i + 1) % 2].max + timeAxis.range);
                    auxPlotBMP[i].draw.Clear(Color.Transparent);
                    //auxPlotBMP(i).draw.DrawLine(Pens.Green, 0, 0, 0, auxPlotBMP(i).bmp.Height)
                    //Size = auxPlotBMP(i).draw.MeasureString(totalTime, fnt)
                    //auxPlotBMP(i).draw.FillRectangle(New SolidBrush(bckGnd), 2, 2, Size.Width - 4, Size.Height - 4)
                    //auxPlotBMP(i).draw.DrawString(totalTime, fnt, Brushes.Green, 0, 0)
                }
            }

            //plotting the aux plot bitmaps
            drawAuxPlotBitmaps(tMin, tMax);
        }

        private void tmrWhenPlay(double scrollTime)
        {
            //applying the scroll
            timeAxis.min = timeAxis.min + scrollTime;
            timeAxis.max = timeAxis.max + scrollTime;

            //updating the aux plots
            for (int i = 0; i <= 1; i++)
            {
                if (timeAxis.min >= auxTimeAxis[i].max)
                {
                    //the plot is left passed, setting it at the right side
                    auxTimeAxis[i].min = auxTimeAxis[(i + 1) % 2].max;
                    auxTimeAxis[i].max = System.Convert.ToDouble(auxTimeAxis[(i + 1) % 2].max + timeAxis.range);
                    auxPlotBMP[i].draw.Clear(Color.Transparent);

                    //drawing all the points inside the aux plot moved
                    drawAuxPlotBitmaps(auxTimeAxis[i].min, auxTimeAxis[i].max);
                }
            }
        }

        //Private Async Sub addPointsToSeries(ByVal time As Double)
        private async System.Threading.Tasks.Task addPointsToSeries(double time)
        {
            try
            {
                int i = 0;
                int value = 0;
                foreach (tStation station in stations)
                {

                    //if the station has a real station associated
                    if (!station.noStation)
                    {
                        //if the station is already connected
                        bool bConnected = false;
                        foreach (ManRegGlobal.cConnectedStation connStn in ManRegGlobal.connectedStations)
                        {
                            if (connStn.ID == station.station.myStationID.ToString())
                            {
                                bConnected = true;
                            }
                        }
                        if (bConnected)
                        {
                            //station already connected
                            foreach (tEntry entry in station.entry)
                            {
                                //getting the corresponding serie
                                i = getSerieIndex(entry.serieID);
                                value = System.Convert.ToInt32(await station.station.getData(entry.port, entry.magnitude, value));
                                series[i].addPoint(time, value, ManRegGlobal.tempunitUTI); // 13/01/2014 getData returns UTI values if magnitude is temperature
                            }
                        }
                        else
                        {
                            //The station is not present, stopping the record
                            myStop();
                        }
                    }
                }
            }
            catch (CerrorRegister err)
            {
                err.showError();
                myStop();
            }
            catch (Exception ex)
            {
                Console.WriteLine("CPlot::addPointsToSeries . Exception:" + ex.Message);
            }
        }

        private void drawAuxPlotBitmaps(double tMin, double tMax)
        {
            //plotting the points of the series in the new time interval
            List<PointF> pts = new List<PointF>();
            Point curPix = new Point();
            Point prevPix = new Point();
            tAxis axis = new tAxis();
            Color clr = new Color();
            try
            {
            }
            catch (Exception)
            {
            }
            foreach (Cserie s in series)
            {
                pts.Clear();

                //getting the time interval points
                s.getPoints(pts, tMin, tMax, System.Convert.ToString(myTempUnits), POINT_FILTER_GRADE);

                //setting the serie axis
                if ((int)s.myAxis == TEMPERATURE)
                {
                    axis = tempAxis;
                }
                if ((int)s.myAxis == POWER)
                {
                    axis = pwrAxis;
                }
                if ((int)s.myAxis == FLOW)
                {
                    axis = pwrAxis;
                }

                //setting the color
                clr = s.myColor;

                //if the serie has points
                if (pts.Count > 0)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        //converting and plotting the points
                        prevPix = pointToPixel(pts[0], axis, auxTimeAxis[j]);
                        foreach (PointF p in pts)
                        {
                            //converting points to pixels
                            curPix = pointToPixel(p, axis, auxTimeAxis[j]);
                            if (curPix.Y > auxPlotBMP[j].bmp.Height)
                            {
                                curPix.Y = auxPlotBMP[j].bmp.Height;
                            }
                            if (curPix.Y < 0)
                            {
                                curPix.Y = 0;
                            }

                            //plotting the line and point
                            if (s.myShowLines)
                            {
                                auxPlotBMP[j].draw.DrawLine(new Pen(clr, lineWidth), prevPix, curPix);
                            }
                            if (s.myShowPoints)
                            {
                                auxPlotBMP[j].draw.FillEllipse(new SolidBrush(clr), curPix.X - pointWidth / 2, curPix.Y - pointWidth / 2, pointWidth, pointWidth);
                            }

                            //setting the curPix as the prevPix
                            prevPix = curPix;
                        }
                    }
                }
            }
        }

    }
}
