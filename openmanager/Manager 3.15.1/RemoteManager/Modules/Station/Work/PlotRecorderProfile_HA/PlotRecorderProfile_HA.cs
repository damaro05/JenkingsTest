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

using Telerik.Charting;
using Telerik.WinControls.UI;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using DataJBC;
using JBC_ConnectRemote;


namespace RemoteManager
{
    public class PlotRecorderProfile_HA
    {

        public enum mediaPlayStatus
        {
            Stopped,
            Running
        }


        private int TEMP_MAX = 500; //Temperatura máxima 500ºC
        private int TEMP_MIN = 0; //Temperatura mínima 0ºC
        private int TEMP_STEP = 100; //Cada 100 ºC mostrar separación
        private int POWER_MAX = 100; //Potencia/flow máximo 100%
        private int POWER_MIN = 0; //Potencia/flow mínimo 0%
        private int POWER_STEP = 20; //Cada 20 % mostrar separación


        private JBC_API_Remote m_jbcConnect;
        private RadChartView m_radChartView;
        private Label m_labelHotAirTemp;
        private Label m_labelExtTCTemp;
        private Label m_labelAirFlow;
        private Label m_labelStatus;
        private long m_stationID;
        private Port m_port;

        //Window properties
        private int m_windowTime; //En decimas de segundo
        private int m_windowDivisions;

        //Live data
        private LiveDataModel m_liveData; //Modelo de datos de las series

        //Axis
        private CategoricalAxis m_timeAxis = new CategoricalAxis(); //Eje de tiempo
        private LinearAxis m_tempAxis = new LinearAxis(); //Eje de temperatura
        private LinearAxis m_powerAxis = new LinearAxis(); //Eje de potencia

        //Media state
        private mediaPlayStatus m_mediaPlayStatus = mediaPlayStatus.Stopped;


        public PlotRecorderProfile_HA(JBC_API_Remote jbcConnect, RadChartView _radChartView, Label labelHotAirTemp, Label labelExtTCTemp, Label labelAirFlow, Label labelStatus, long stationID, Port port)
        {

            m_jbcConnect = jbcConnect;
            m_radChartView = _radChartView;
            m_labelHotAirTemp = labelHotAirTemp;
            m_labelExtTCTemp = labelExtTCTemp;
            m_labelAirFlow = labelAirFlow;
            m_labelStatus = labelStatus;
            m_stationID = stationID;
            m_port = port;
            m_liveData = new LiveDataModel(m_jbcConnect, m_radChartView, m_labelHotAirTemp, m_labelExtTCTemp, m_labelAirFlow, m_labelStatus, m_stationID);


            //
            //Initialize axis
            //

            //Horizontal axis - Time
            m_timeAxis.LabelFitMode = AxisLabelFitMode.MultiLine;
            m_timeAxis.PlotMode = AxisPlotMode.OnTicks;
            m_timeAxis.ShowLabels = true;
            m_timeAxis.LabelFormatProvider = new LabelTimeFormat();
            m_timeAxis.LabelOffset = 1; //no mostrar el primer label

            //Vertical axis - Temperature
            m_tempAxis.AxisType = AxisType.Second;
            m_tempAxis.Title = "Temp ºC";
            m_tempAxis.Maximum = TEMP_MAX;
            m_tempAxis.Minimum = TEMP_MIN;
            m_tempAxis.MajorStep = TEMP_STEP;

            //Vertical axis - Power
            m_powerAxis.HorizontalLocation = AxisHorizontalLocation.Right;
            m_powerAxis.AxisType = AxisType.Second;
            m_powerAxis.Title = "Power %";
            m_powerAxis.Maximum = POWER_MAX;
            m_powerAxis.Minimum = POWER_MIN;
            m_powerAxis.MajorStep = POWER_STEP;

            //
            //Initialize grid area
            //

            //Adjust margins
            m_radChartView.View.Margin = new Padding(0);

            CartesianArea area = m_radChartView.GetArea<CartesianArea>();
            CartesianGrid grid = area.GetGrid<CartesianGrid>();
            grid.DrawHorizontalStripes = true;
            grid.DrawHorizontalFills = false;
            grid.DrawVerticalStripes = true;
            grid.DrawVerticalFills = false;
            grid.ForeColor = Color.DarkGray;


            //Se añaden y se quitan dos series vacias a cada eje para que pinte del color correcto el eje de temperatura y de potencia
            FastLineSeries lineSeries = new FastLineSeries();
            lineSeries.BorderColor = Color.Black;
            lineSeries.DataSource = new BindingList<DataSerie>();
            lineSeries.HorizontalAxis = m_timeAxis;
            lineSeries.VerticalAxis = m_tempAxis;
            m_radChartView.Series.Add(lineSeries);

            lineSeries = new FastLineSeries();
            lineSeries.BorderColor = Color.Black;
            lineSeries.DataSource = new BindingList<DataSerie>();
            lineSeries.HorizontalAxis = m_timeAxis;
            lineSeries.VerticalAxis = m_powerAxis;
            m_radChartView.Series.Add(lineSeries);

            m_radChartView.Series.Clear();


            //
            //Initialize series
            //

            //Profile hot air temp
            FastLineSeries serieProfileHotAirTemp = new FastLineSeries();
            serieProfileHotAirTemp.Name = "Profile hot air temperature";
            serieProfileHotAirTemp.LegendTitle = "Profile hot air temperature";
            serieProfileHotAirTemp.BorderColor = Color.LightCoral;
            serieProfileHotAirTemp.PointSize = new SizeF(0, 0);
            serieProfileHotAirTemp.CategoryMember = "Time";
            serieProfileHotAirTemp.ValueMember = "Value";
            serieProfileHotAirTemp.DataSource = m_liveData.DataProfileHotAirTemp;
            serieProfileHotAirTemp.BorderWidth = 1;
            serieProfileHotAirTemp.HorizontalAxis = m_timeAxis;
            serieProfileHotAirTemp.VerticalAxis = m_tempAxis;
            m_radChartView.Series.Add(serieProfileHotAirTemp);
            //Profile ext TC temp
            FastLineSeries serieProfileExtTCTemp = new FastLineSeries();
            serieProfileExtTCTemp.Name = "Profile ext TC temperature";
            serieProfileExtTCTemp.LegendTitle = "Profile ext TC temperature";
            serieProfileExtTCTemp.BorderColor = Color.LightGreen;
            serieProfileExtTCTemp.PointSize = new SizeF(0, 0);
            serieProfileExtTCTemp.CategoryMember = "Time";
            serieProfileExtTCTemp.ValueMember = "Value";
            serieProfileExtTCTemp.DataSource = m_liveData.DataProfileExtTCTemp;
            serieProfileExtTCTemp.BorderWidth = 1;
            serieProfileExtTCTemp.HorizontalAxis = m_timeAxis;
            serieProfileExtTCTemp.VerticalAxis = m_tempAxis;
            m_radChartView.Series.Add(serieProfileExtTCTemp);
            //Profile air flow
            FastLineSeries serieProfileAirFlow = new FastLineSeries();
            serieProfileAirFlow.Name = "Profile air flow";
            serieProfileAirFlow.LegendTitle = "Profile air flow";
            serieProfileAirFlow.BorderColor = Color.DeepSkyBlue;
            serieProfileAirFlow.PointSize = new SizeF(0, 0);
            serieProfileAirFlow.CategoryMember = "Time";
            serieProfileAirFlow.ValueMember = "Value";
            serieProfileAirFlow.DataSource = m_liveData.DataProfileAirFlow;
            serieProfileAirFlow.BorderWidth = 1;
            serieProfileAirFlow.HorizontalAxis = m_timeAxis;
            serieProfileAirFlow.VerticalAxis = m_powerAxis;
            m_radChartView.Series.Add(serieProfileAirFlow);
            //Hot air temp
            FastLineSeries serieHotAirTemp = new FastLineSeries();
            serieHotAirTemp.Name = "Hot air temperature";
            serieHotAirTemp.LegendTitle = "Hot air temperature";
            serieHotAirTemp.BorderColor = Color.DarkRed;
            serieHotAirTemp.PointSize = new SizeF(0, 0);
            serieHotAirTemp.CategoryMember = "Time";
            serieHotAirTemp.ValueMember = "Value";
            serieHotAirTemp.DataSource = m_liveData.DataHotAirTemp;
            serieHotAirTemp.BorderWidth = 1;
            serieHotAirTemp.HorizontalAxis = m_timeAxis;
            serieHotAirTemp.VerticalAxis = m_tempAxis;
            m_radChartView.Series.Add(serieHotAirTemp);
            //Ext TC temp
            FastLineSeries serieExtTCTemp = new FastLineSeries();
            serieExtTCTemp.Name = "Ext TC temperature";
            serieExtTCTemp.LegendTitle = "Ext TC temperature";
            serieExtTCTemp.BorderColor = Color.DarkGreen;
            serieExtTCTemp.PointSize = new SizeF(0, 0);
            serieExtTCTemp.CategoryMember = "Time";
            serieExtTCTemp.ValueMember = "Value";
            serieExtTCTemp.DataSource = m_liveData.DataExtTCTemp;
            serieExtTCTemp.BorderWidth = 1;
            serieExtTCTemp.HorizontalAxis = m_timeAxis;
            serieExtTCTemp.VerticalAxis = m_tempAxis;
            m_radChartView.Series.Add(serieExtTCTemp);
            //Air flow
            FastLineSeries serieAirFlow = new FastLineSeries();
            serieAirFlow.Name = "Air flow";
            serieAirFlow.LegendTitle = "Air flow";
            serieAirFlow.BorderColor = Color.RoyalBlue;
            serieAirFlow.PointSize = new SizeF(0, 0);
            serieAirFlow.CategoryMember = "Time";
            serieAirFlow.ValueMember = "Value";
            serieAirFlow.DataSource = m_liveData.DataAirFlow;
            serieAirFlow.BorderWidth = 1;
            serieAirFlow.HorizontalAxis = m_timeAxis;
            serieAirFlow.VerticalAxis = m_powerAxis;
            m_radChartView.Series.Add(serieAirFlow);
            //Vertical mark serie
            FastLineSeries serieVerticalMark = new FastLineSeries();
            serieVerticalMark.BorderColor = Color.Black;
            serieVerticalMark.PointSize = new SizeF(0, 0);
            serieVerticalMark.CategoryMember = "Time";
            serieVerticalMark.ValueMember = "Value";
            serieVerticalMark.DataSource = m_liveData.DataVerticalMark;
            serieVerticalMark.BorderWidth = 1;
            serieVerticalMark.HorizontalAxis = m_timeAxis;
            serieVerticalMark.VerticalAxis = m_tempAxis;
            m_radChartView.Series.Add(serieVerticalMark);
        }

        public void LoadProfile()
        {
            //Si el modo de regulación del perfil está en Air temp, el valor de ext TC se obtiene del seleccionado en la estación
            //Si el modo de regulación del perfil está en Ext TC temp, el valor de Air temp no se tiene que mostrar

            byte[] profileDataByte = m_jbcConnect.GetProfile(m_stationID, m_jbcConnect.GetSelectedProfile(m_stationID));
            if (ReferenceEquals(profileDataByte, null))
            {
                return;
            }

            string profileData = System.Text.Encoding.ASCII.GetString(profileDataByte);
            JObject jsonParse = (JObject)(JObject.Parse(profileData));
            JToken points = jsonParse["_"]["Points"];
            ProfileRegulationMode profileRegulationMode = (ProfileRegulationMode)(System.Convert.ToInt32(jsonParse["_"]["Mode"]));

            m_windowDivisions = 0;
            m_windowTime = 0;
            foreach (var point in points)
            {
                m_windowDivisions++;
                m_windowTime += System.Convert.ToInt32(point["Time"]);
            }

            double[] pointsProfileHotAirTemp = new double[m_windowTime + 1];
            double[] pointsProfileExtTCTemp = new double[m_windowTime + 1];
            double[] pointsProfileAirFlow = new double[m_windowTime + 1];

            int tick = 0;
            double iniHotAirTemp = 0;
            double endHotAirTemp = 0;
            double iniExtTCTemp = 0;
            double endExtTCTemp = 0;
            double iniAirFlow = 0;
            double endAirFlow = 0;

            foreach (var point in points)
            {
                //Valores del inicio del step
                if (tick == 0)
                {
                    if (profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
                    {
                        iniHotAirTemp = System.Convert.ToDouble(point["ATemp"]);
                        iniExtTCTemp = m_jbcConnect.GetPortToolSelectedExtTemp(m_stationID, m_port).UTI;
                    }
                    else
                    {
                        iniHotAirTemp = -1;
                        iniExtTCTemp = System.Convert.ToDouble(point["ETemp1"]);
                    }
                    iniAirFlow = System.Convert.ToDouble(point["AFlow"]);

                    //Punto inicial
                    pointsProfileHotAirTemp[tick] = iniHotAirTemp;
                    pointsProfileExtTCTemp[tick] = iniExtTCTemp;
                    pointsProfileAirFlow[tick] = iniAirFlow;
                    tick = 1;
                }
                else
                {
                    iniHotAirTemp = endHotAirTemp;
                    iniExtTCTemp = endExtTCTemp;
                    iniAirFlow = endAirFlow;
                }

                //Valores del final del step
                if (profileRegulationMode == ProfileRegulationMode.AIR_TEMP)
                {
                    endHotAirTemp = System.Convert.ToDouble(point["ATemp"]);
                    endExtTCTemp = m_jbcConnect.GetPortToolSelectedExtTemp(m_stationID, m_port).UTI;
                }
                else
                {
                    endHotAirTemp = -1;
                    endExtTCTemp = System.Convert.ToDouble(point["ETemp1"]);
                }
                endAirFlow = System.Convert.ToDouble(point["AFlow"]);

                //Duración del step
                int stepTime = System.Convert.ToInt32(point["Time"]);

                //Calcular el incremento para cada punto del actual step
                double slopeHotAirTemp = (endHotAirTemp - iniHotAirTemp) / stepTime;
                double slopeExtTCTemp = (endExtTCTemp - iniExtTCTemp) / stepTime;
                double slopeAirFlow = (endAirFlow - iniAirFlow) / stepTime;

                //Valores temporales para los puntos
                double tmpHotAirTemp = iniHotAirTemp;
                double tmpExtTCTemp = iniExtTCTemp;
                double tmpAirFlow = iniAirFlow;

                for (int i = 1; i <= System.Convert.ToInt32(point["Time"]); i++)
                {
                    //Incrementar los valores para cada tick
                    tmpHotAirTemp += slopeHotAirTemp;
                    tmpExtTCTemp += slopeExtTCTemp;
                    tmpAirFlow += slopeAirFlow;

                    //Guardar los puntos calculados de la recta
                    pointsProfileHotAirTemp[tick] = tmpHotAirTemp;
                    pointsProfileExtTCTemp[tick] = tmpExtTCTemp;
                    pointsProfileAirFlow[tick] = tmpAirFlow;

                    tick++;
                }
            }

            //Horizontal axis - Time
            m_timeAxis.MajorTickInterval = m_windowTime / m_windowDivisions;

            //Data points
            m_liveData.DataPoints = m_windowTime + 1;

            //Fill Data
            m_liveData.InitializeDataProfileHotAirTemp(pointsProfileHotAirTemp);
            m_liveData.InitializeDataProfileExtTCTemp(pointsProfileExtTCTemp);
            m_liveData.InitializeDataProfileAirFlow(pointsProfileAirFlow);
        }

        public async System.Threading.Tasks.Task Start()
        {
            if (m_mediaPlayStatus == mediaPlayStatus.Stopped)
            {
                m_mediaPlayStatus = mediaPlayStatus.Running;
                LoadProfile();
                await m_liveData.Start();
            }
        }

        public void Stop()
        {
            if (m_mediaPlayStatus == mediaPlayStatus.Running)
            {
                m_mediaPlayStatus = mediaPlayStatus.Stopped;

            }
        }

        public bool IsRunning()
        {
            return m_mediaPlayStatus == mediaPlayStatus.Running;
        }

    }
}
