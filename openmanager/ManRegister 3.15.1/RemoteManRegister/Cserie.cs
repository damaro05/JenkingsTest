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
using DataJBC;
// End of VB project level imports


// 09/04/2015 Environment.TickCount And Int32.MaxValue: se añade 'And Int32.MaxValue' para que devuelva siempre un positivo
//       TickCount cycles between Int32.MinValue, which is a negative number, and Int32.MaxValue once every 49.8 days.
//       This removes the sign bit to yield a nonnegative number that cycles between zero and Int32.MaxValue once every 24.9 days.
namespace RemoteManRegister
{
    public class Cserie
    {
        //Constants
        private const int NUM_POINTS_IN_MEMORY = 30000;

        //Global var's
        private string ID; //Serie unique ID (serie name)
        private int axis; //The axis where the serie is associated, used by the Cplotter ( tipically Temperature or Power)
        private System.Drawing.Color clr; //The serie color
        private bool showPoints; //Show the points
        private bool showLines; //Show the linear interpolation
        private string legend; //The serie legend text

        private string tempFile; //The serie temp file where to store the points
        private double lastTimeInFile; //Indicates the time of the last point saved in the temp file

        private System.Collections.Generic.List<System.Drawing.PointF> points; //The serie list of points ( time(s), magnitude(<user decide>))
        private ulong nPoints; //The total number of points

        private System.IO.StreamReader fReadPoint; //The stream reader for the getNextpoint()
        private bool initReadPoint = false; //Determinates if the reader is opened

        private System.IO.StreamWriter fWriteLoad; //The stream writer for the addLoadPoint()
        private bool initWriteLoad = false; //Determinates if the writer is opened

        //Properties
        public dynamic myID
        {
            get
            {
                return ID;
            }
            set
            {
                ID = System.Convert.ToString(value);
            }
        }
        public dynamic myAxis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = System.Convert.ToInt32(value);
            }
        }
        public System.Drawing.Color myColor
        {
            get
            {
                return clr;
            }
            set
            {
                clr = value;
            }
        }
        public dynamic myShowPoints
        {
            get
            {
                return showPoints;
            }
            set
            {
                showPoints = System.Convert.ToBoolean(value);
            }
        }
        public dynamic myShowLines
        {
            get
            {
                return showLines;
            }
            set
            {
                showLines = System.Convert.ToBoolean(value);
            }
        }
        public dynamic myLegend
        {
            get
            {
                return legend;
            }
            set
            {
                legend = System.Convert.ToString(value);
            }
        }
        public dynamic myLastTime
        {
            get
            {
                if (lastTimeInFile >= 0)
                {
                    return lastTimeInFile;
                }
                return 0;
            }
        }
        public ulong myPointCount
        {
            get
            {
                return nPoints;
            }
        }

        //Public functions
        public Cserie(string pID, int pAxis)
        {
            //assigning the values
            myID = pID;
            myAxis = pAxis;

            //creating the point list
            points = new List<System.Drawing.PointF>();
            nPoints = 0;

            //creating the temp file
            tempFile = Microsoft.VisualBasic.FileIO.FileSystem.GetTempFileName();
            lastTimeInFile = -1;

            //default values
            clr = System.Drawing.Color.Red;
            showPoints = false;
            showLines = true;
            legend = ID;
        }

        public void dispose()
        {
            points.Clear();
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(tempFile);
        }

        public void addPoint(double time, double value, string tempUnitsIfTemp)
        {
            //checking the point time is higher than the highest time
            if (lastTimeInFile >= time)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_SERIE_BAD_POINT_TIME));
            }

            try
            {
                // 13/01/2014 convert temperature to UTI, if required
                if ((int)myAxis == Cplot.TEMPERATURE)
                {
                    CTemperature auxTemp = new CTemperature(0);
                    if (tempUnitsIfTemp == ManRegGlobal.tempunitCELSIUS)
                    {
                        auxTemp.InCelsius(System.Convert.ToInt32(value));
                        value = auxTemp.UTI;
                    }
                    else if (tempUnitsIfTemp == ManRegGlobal.tempunitFAHRENHEIT)
                    {
                        auxTemp.InFahrenheit(System.Convert.ToInt32(value));
                        value = auxTemp.UTI;
                    }
                }

                //point time is correct, adding it
                //Dim initTime As ULong = Environment.TickCount And Int32.MaxValue
                string text = time.ToString() + ";" + value.ToString();
                System.IO.StreamWriter fWrite = new System.IO.StreamWriter(tempFile, true);
                fWrite.WriteLine(text);
                fWrite.Flush();
                fWrite.Close();
                //If (Environment.TickCount And Int32.MaxValue) - initTime > 0 Then Debug.Print("saveFile: " & (Environment.TickCount And Int32.MaxValue) - initTime)

                //adjusting the point list to the end of the sequence and adding the point
                //initTime = Environment.TickCount And Int32.MaxValue
                if (points.Count > 0)
                {
                    if (Math.Round(lastTimeInFile, 3) > Math.Round(System.Convert.ToDouble(points.Last().X), 3))
                    {
                        //Debug.Print("times: " & lastTimeInFile & " - " & points.Last.X)
                        //point list is not at the end of the sequence
                        loadPointsFromFile(-1, lastTimeInFile);
                    }
                }
                //If (Environment.TickCount  And Int32.MaxValue) - initTime > 0 Then Debug.Print("loadFile: " & (Environment.TickCount And Int32.MaxValue) - initTime)

                //adding the point to the list of points
                points.Add(new System.Drawing.PointF((float)time, (float)value));
                nPoints++;

                //setting the last point time in the file
                lastTimeInFile = time;

                //deleting the precious points if memory exceded
                if (points.Count > NUM_POINTS_IN_MEMORY)
                {
                    points.RemoveRange(0, points.Count - NUM_POINTS_IN_MEMORY);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cserie::addPoint . error:" + ex.Message);
            }
        }

        public void initLoadPointWriter()
        {
            fWriteLoad = new System.IO.StreamWriter(tempFile, true);
            initWriteLoad = true;
        }

        public void addLoadPoint(double time, double value, string tempUnitsIfTemp)
        {
            //checking the point time is higher than the highest time
            // 13/01/2014 if temperature, value should be UTI
            if (lastTimeInFile >= time)
            {
                throw (new CerrorRegister(CerrorRegister.ERROR_SERIE_BAD_POINT_TIME));
            }

            try
            {
                // 13/01/2014 convert temperature to UTI, if required
                if ((int)myAxis == Cplot.TEMPERATURE)
                {
                    CTemperature auxTemp = new CTemperature(0);
                    if (tempUnitsIfTemp == ManRegGlobal.tempunitCELSIUS)
                    {
                        auxTemp.InCelsius(System.Convert.ToInt32(value));
                        value = auxTemp.UTI;
                    }
                    else if (tempUnitsIfTemp == ManRegGlobal.tempunitFAHRENHEIT)
                    {
                        auxTemp.InFahrenheit(System.Convert.ToInt32(value));
                        value = auxTemp.UTI;
                    }
                }

                //adding the point to the list of points
                points.Add(new System.Drawing.PointF((float)time, (float)value));
                nPoints++;

                //setting the last point time in the file
                lastTimeInFile = time;

                //if the point list is at its maximum capacity, writting into the temp file
                if (points.Count >= NUM_POINTS_IN_MEMORY)
                {
                    string text = "";
                    foreach (System.Drawing.PointF p in points)
                    {
                        text = p.X.ToString() + ";" + p.Y.ToString();
                        fWriteLoad.WriteLine(text);
                    }
                    points.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cserie::addLoadPoint . error:" + ex.Message);
            }
        }

        public void finishLoadPointWriter()
        {
            //setting all the remaining points in the point list
            string text = "";
            foreach (System.Drawing.PointF p in points)
            {
                text = p.X.ToString() + ";" + p.Y.ToString();
                fWriteLoad.WriteLine(text);
            }
            points.Clear();

            //closing the writer
            fWriteLoad.Close();
            initWriteLoad = false;

            //adjusting the point list to the end of the sequence and adding the point
            loadPointsFromFile(-1, lastTimeInFile);
        }

        public void getPoints(List<System.Drawing.PointF> lst, double tMin, double tMax, string tempUnitsIfTemp, int filterGrade = 0)
        {
            //if the points aren't lodaed, loading them
            if (points.Count > 0)
            {
                if ((Math.Round(System.Convert.ToDouble(points[0].X), 3) > Math.Round(tMin, 3) && Math.Round(System.Convert.ToDouble(points[0].X), 3) > 0) || (Math.Round(System.Convert.ToDouble(points.Last().X), 3) < Math.Round(tMax, 3) && Math.Round(lastTimeInFile, 3) > Math.Round(System.Convert.ToDouble(points.Last().X), 3)))
                {
                    //FileIO.FileSystem.WriteAllText("E:\Trabajo JBC\JBCstationPlotter\out.txt", _
                    //                               "tMin: " & tMin & "; tMax: " & tMax & "; last: " & _
                    //                               points.Last.X & "; first: " & points(0).X & "; file: " & lastTimeInFile & vbCrLf, True)
                    ulong initTime = (ulong)(Environment.TickCount & int.MaxValue);
                    loadPointsFromFile(tMin, tMax);
                    //If (Environment.TickCount And Int32.MaxValue) - initTime > 0 Then Debug.Print("getPoints: " & (Environment.TickCount And Int32.MaxValue) - initTime)
                }

                //determinating the index range for the raw points
                int first = 0;
                int last = 0;
                getTimeIntervalPointIndex(ref first, ref last, tMin, tMax);

                //getting the previous point of the interval if posible. It's to allow intervals concatenation
                if (first > 0)
                {
                    first--;
                }

                //the point list has the proper points loaded, filtering the points
                int i = 0;
                int j = 0;
                double sum = 0;
                double result = 0;
                float y = 0;
                lst.Clear();
                try
                {
                    for (i = first; i <= last; i++)
                    {
                        if (i - filterGrade + 1 >= 0)
                        {
                            //filter can be applied
                            sum = 0;
                            for (j = 0; j <= filterGrade - 1; j++)
                            {
                                if ((int)myAxis == Cplot.TEMPERATURE)
                                {
                                    CTemperature auxTemp = new CTemperature(System.Convert.ToInt32(points[i - j].Y));
                                    if (tempUnitsIfTemp == ManRegGlobal.tempunitCELSIUS)
                                    {
                                        y = auxTemp.ToCelsius();
                                        //Debug.Print(String.Format("GetPoints as Celsius: UTI: {0} Celsius {1}", points.Item(i - j).Y.ToString, y.ToString))
                                    }
                                    else if (tempUnitsIfTemp == ManRegGlobal.tempunitFAHRENHEIT)
                                    {
                                        y = auxTemp.ToFahrenheit();
                                        //Debug.Print(String.Format("GetPoints as Fahrenheit: UTI: {0} Fahr {1}", points.Item(i - j).Y.ToString, y.ToString))
                                    }
                                    else
                                    {
                                        y = System.Convert.ToSingle(points[i - j].Y);
                                        //Debug.Print(String.Format("GetPoints as UTI: {0}", points.Item(i - j).Y.ToString))
                                    }
                                }
                                else
                                {
                                    y = System.Convert.ToSingle(points[i - j].Y);
                                }
                                sum = sum + y;
                            }
                            result = sum / filterGrade;

                            //assigning the point
                            lst.Add(new System.Drawing.PointF(System.Convert.ToSingle(points[i].X), (float)result));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Cserie::getPoints . error:" + ex.Message);
                }
            }
        }

        public void getAllPoints(ref List<System.Drawing.PointF> lst, string tempUnitsIfTemp, int filterGrade)
        {
            //getting all the points, it is from time 0 to last time in file
            getPoints(lst, 0, lastTimeInFile, tempUnitsIfTemp, filterGrade);
        }

        List<double> filterMemo = new List<double>();
        public void initPointIndexReader()
        {
            //opening the temp file from the beggining
            fReadPoint = new System.IO.StreamReader(tempFile);
            initReadPoint = true;
            filterMemo.Clear();
        }

        public void getNextPoint(System.Drawing.PointF pt, bool last, string tempUnitsIfTemp, int filterGrade)
        {
            try
            {
                //reading the current line
                if (initReadPoint)
                {
                    string line = fReadPoint.ReadLine();
                    if (line.Length > 0)
                    {
                        pt.X = (float)(Convert.ToDouble(line.Substring(0, line.IndexOf(";"))));
                        pt.Y = (float)(Convert.ToDouble(line.Substring(line.IndexOf(";") + 1, line.Length - (line.IndexOf(";") + 1))));

                        // 13/04/2014 convert UTI to Celsius or Faharenheit
                        // do here to convert from UTI before filtering (result may be decimal)
                        if ((int)myAxis == Cplot.TEMPERATURE)
                        {
                            // convert UTI values to requested temp unit. if requested temp unit is blank or UTI, return UTI value
                            CTemperature auxTemp = new CTemperature(System.Convert.ToInt32(pt.Y));
                            if (tempUnitsIfTemp == ManRegGlobal.tempunitCELSIUS)
                            {
                                pt.Y = auxTemp.ToCelsius();
                            }
                            else if (tempUnitsIfTemp == ManRegGlobal.tempunitFAHRENHEIT)
                            {
                                pt.Y = auxTemp.ToFahrenheit();
                            }
                        }

                        //Add raw point
                        filterMemo.Add(pt.Y);

                        //Clearing the old points
                        while (filterMemo.Count > filterGrade)
                        {
                            filterMemo.RemoveAt(0);
                        }

                        //Filtering
                        if (filterMemo.Count >= filterGrade)
                        {
                            pt.Y = 0;
                            for (int i = filterMemo.Count - 1; i >= 0; i--)
                            {
                                pt.Y = pt.Y + (float)filterMemo[i];
                            }
                            pt.Y = pt.Y / filterGrade;
                        }

                        // 13/04/2014 convert UTI to Celsius or Faharenheit
                        // do here to convert from UTI after filtering (result will be always integer)
                        //If myAxis = Cplot.TEMPERATURE Then
                        //    ' convert UTI values to requested temp unit. if requested temp unit is blank or UTI, return UTI value
                        //    Dim auxTemp As JBC_ConnectRemote.Ctemperature = New JBC_ConnectRemote.Ctemperature(pt.Y)
                        //    Select Case tempUnitsIfTemp
                        //        Case tempunitCELSIUS
                        //            pt.Y = auxTemp.ToCelsius
                        //        Case tempunitFAHRENHEIT
                        //            pt.Y = auxTemp.ToFahrenheit
                        //    End Select
                        //End If
                    }
                    else
                    {
                        fReadPoint.Close();
                        initReadPoint = false;
                    }

                    //if the end of stream achieved
                    if (fReadPoint.EndOfStream)
                    {
                        fReadPoint.Close();
                        initReadPoint = false;
                    }
                }

                //if last point desired
                if (last)
                {
                    fReadPoint.Close();
                    initReadPoint = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cserie::getNextPoint . error:" + ex.Message);
            }
        }

        public void clear()
        {
            points.Clear();
            nPoints = 0;
            Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(tempFile);

            //reopening the file
            tempFile = Microsoft.VisualBasic.FileIO.FileSystem.GetTempFileName();
            lastTimeInFile = -1;
        }

        //Private functions
        private void getTimeIntervalPointIndex(ref int first, ref int last, double tMin, double tMax)
        {
            //looking for the point index range that compound the indicated time interval
            bool foundFirst = false;
            bool foundLast = false;
            int i = points.Count - 1;
            while (i >= 0 && (!foundFirst || !foundLast))
            {
                if (points[i].X <= tMax && !foundLast)
                {
                    //found the last point of the range
                    last = i;
                    foundLast = true;
                }
                if (points[i].X < tMin && !foundFirst)
                {
                    //found the first point of the range
                    first = i + 1;
                    foundFirst = true;
                }

                //decrementing the counter
                i--;
            }

            //checking search results
            if (!foundLast)
            {
                //bad time interval, no points inside
                first = -1;
                last = -1;
            }
            if (!foundFirst)
            {
                //the time interval min value is lower than the initial point
                first = 0;
            }
        }

        private void loadPointsFromFile(double tMin, double tMax)
        {
            //loading the indicated range of points
            if (initReadPoint)
            {
                fReadPoint.Close();
            }
            points.Clear();
            System.IO.StreamReader file = Microsoft.VisualBasic.FileIO.FileSystem.OpenTextFileReader(tempFile);
            bool done = false;
            string line = "";
            System.Drawing.PointF pt = new System.Drawing.PointF();
            while (!file.EndOfStream && !done)
            {
                line = file.ReadLine();
                pt.X = (float)(Convert.ToDouble(line.Substring(0, line.IndexOf(";"))));
                pt.Y = (float)(Convert.ToDouble(line.Substring(line.IndexOf(";") + 1, line.Length - (line.IndexOf(";") + 1))));

                //depending on the indicated range
                if (tMin == -1)
                {
                    points.Add(pt);
                    if (points.Count > NUM_POINTS_IN_MEMORY)
                    {
                        points.RemoveRange(0, points.Count - NUM_POINTS_IN_MEMORY);
                    }
                    if (pt.X >= tMax)
                    {
                        done = true;
                    }
                }
                else if (tMax == -1)
                {
                    if (pt.X >= tMin)
                    {
                        points.Add(pt);
                        if (points.Count >= NUM_POINTS_IN_MEMORY)
                        {
                            done = true;
                        }
                    }
                }
                else
                {
                    if (pt.X >= tMin && (pt.X <= tMax | points.Count < NUM_POINTS_IN_MEMORY))
                    {
                        points.Add(pt);
                    }
                    else if (pt.X > tMax)
                    {
                        done = true;
                    }
                }

            }
            file.Close();
        }
    }
}
