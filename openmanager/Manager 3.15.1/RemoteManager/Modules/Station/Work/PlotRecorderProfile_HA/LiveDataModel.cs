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

using Telerik.WinControls.UI;
using System.ComponentModel;
using JBC_ConnectRemote;
using DataJBC;


namespace RemoteManager
{
    public class LiveDataModel
    {

        private JBC_API_Remote m_jbcConnect;
        private RadChartView m_radChartView;
        private Label m_labelHotAirTemp;
        private Label m_labelExtTCTemp;
        private Label m_labelAirFlow;
        private Label m_labelStatus;
        private long m_stationID;

        private uint m_queueID;
        private Timer m_timerPrintSeries = new Timer();
        private int m_tickCount;
        private int m_dataPoints;

        private ToolStatus_HA m_toolStatus = ToolStatus_HA.NONE;

        //Series
        public BindingList<DataSerie> m_dataProfileHotAirTemp;
        public BindingList<DataSerie> m_dataProfileExtTCTemp;
        public BindingList<DataSerie> m_dataProfileAirFlow;
        public BindingList<DataSerie> m_dataHotAirTemp;
        public BindingList<DataSerie> m_dataExtTCTemp;
        public BindingList<DataSerie> m_dataAirFlow;
        public BindingList<DataSerie> m_dataVerticalMark;


        public LiveDataModel(JBC_API_Remote jbcConnect, RadChartView _radChartView, Label labelHotAirTemp, Label labelExtTCTemp, Label labelAirFlow, Label labelStatus, long stationID)
        {

            m_jbcConnect = jbcConnect;
            m_radChartView = _radChartView;
            m_labelHotAirTemp = labelHotAirTemp;
            m_labelExtTCTemp = labelExtTCTemp;
            m_labelAirFlow = labelAirFlow;
            m_labelStatus = labelStatus;
            m_stationID = stationID;

            m_timerPrintSeries.Interval = 100;
            m_timerPrintSeries.Tick += Tick_PrintSeries;

            //Data collection
            BindingList<DataSerie> collection = new BindingList<DataSerie>();
            DataProfileHotAirTemp = collection;
            collection = new BindingList<DataSerie>();
            DataProfileExtTCTemp = collection;
            collection = new BindingList<DataSerie>();
            DataProfileAirFlow = collection;
            collection = new BindingList<DataSerie>();
            DataHotAirTemp = collection;
            collection = new BindingList<DataSerie>();
            DataExtTCTemp = collection;
            collection = new BindingList<DataSerie>();
            DataAirFlow = collection;
            collection = new BindingList<DataSerie>();
            DataVerticalMark = collection;
        }

        public int DataPoints
        {
            get
            {
                return m_dataPoints;
            }
            set
            {
                m_dataPoints = value;
            }
        }

        #region Data Property

        public BindingList<DataSerie> DataProfileHotAirTemp
        {
            get
            {
                return m_dataProfileHotAirTemp;
            }
            set
            {
                if (m_dataProfileHotAirTemp != value)
                {
                    m_dataProfileHotAirTemp = value;
                }
            }
        }

        public BindingList<DataSerie> DataProfileExtTCTemp
        {
            get
            {
                return m_dataProfileExtTCTemp;
            }
            set
            {
                if (m_dataProfileExtTCTemp != value)
                {
                    m_dataProfileExtTCTemp = value;
                }
            }
        }

        public BindingList<DataSerie> DataProfileAirFlow
        {
            get
            {
                return m_dataProfileAirFlow;
            }
            set
            {
                if (m_dataProfileAirFlow != value)
                {
                    m_dataProfileAirFlow = value;
                }
            }
        }

        public BindingList<DataSerie> DataHotAirTemp
        {
            get
            {
                return m_dataHotAirTemp;
            }
            set
            {
                if (m_dataHotAirTemp != value)
                {
                    m_dataHotAirTemp = value;
                }
            }
        }

        public BindingList<DataSerie> DataExtTCTemp
        {
            get
            {
                return m_dataExtTCTemp;
            }
            set
            {
                if (m_dataExtTCTemp != value)
                {
                    m_dataExtTCTemp = value;
                }
            }
        }

        public BindingList<DataSerie> DataAirFlow
        {
            get
            {
                return m_dataAirFlow;
            }
            set
            {
                if (m_dataAirFlow != value)
                {
                    m_dataAirFlow = value;
                }
            }
        }

        public BindingList<DataSerie> DataVerticalMark
        {
            get
            {
                return m_dataVerticalMark;
            }
            set
            {
                if (m_dataVerticalMark != value)
                {
                    m_dataVerticalMark = value;
                }
            }
        }

        #endregion

        #region Initialize data

        public void InitializeDataProfileHotAirTemp(double[] pointsProfileHotAirTemp)
        {
            m_dataProfileHotAirTemp.Clear();

            for (int i = 0; i <= DataPoints - 1; i++)
            {
                DataSerie objDataSerie = new DataSerie();
                objDataSerie.Value = CTemperature.ToCelsius(System.Convert.ToInt32(pointsProfileHotAirTemp[i]));
                objDataSerie.Time = i;
                m_dataProfileHotAirTemp.Add(objDataSerie);
            }
        }

        public void InitializeDataProfileExtTCTemp(double[] pointsProfileExtTCTemp)
        {
            m_dataProfileExtTCTemp.Clear();

            for (int i = 0; i <= DataPoints - 1; i++)
            {
                DataSerie objDataSerie = new DataSerie();
                objDataSerie.Value = CTemperature.ToCelsius(System.Convert.ToInt32(pointsProfileExtTCTemp[i]));
                objDataSerie.Time = i;
                m_dataProfileExtTCTemp.Add(objDataSerie);
            }
        }

        public void InitializeDataProfileAirFlow(double[] pointsProfileAirFlow)
        {
            m_dataProfileAirFlow.Clear();

            for (int i = 0; i <= DataPoints - 1; i++)
            {
                DataSerie objDataSerie = new DataSerie();
                objDataSerie.Value = pointsProfileAirFlow[i] / 10;
                objDataSerie.Time = i;
                m_dataProfileAirFlow.Add(objDataSerie);
            }
        }

        private void InitializeDataHotAirTemp()
        {
            m_dataHotAirTemp.Clear();

            for (int i = 0; i <= DataPoints - 1; i++)
            {
                DataSerie objDataSerie = new DataSerie();
                objDataSerie.Value = 0;
                objDataSerie.Time = i;
                m_dataHotAirTemp.Add(objDataSerie);
            }
        }

        private void InitializeDataExtTCTemp()
        {
            m_dataExtTCTemp.Clear();

            for (int i = 0; i <= DataPoints - 1; i++)
            {
                DataSerie objDataSerie = new DataSerie();
                objDataSerie.Value = 0;
                objDataSerie.Time = i;
                m_dataExtTCTemp.Add(objDataSerie);
            }
        }

        private void InitializeDataAirFlow()
        {
            m_dataAirFlow.Clear();

            for (int i = 0; i <= DataPoints - 1; i++)
            {
                DataSerie objDataSerie = new DataSerie();
                objDataSerie.Value = 0;
                objDataSerie.Time = i;
                m_dataAirFlow.Add(objDataSerie);
            }
        }

        private void InitializeDataVerticalMark()
        {
            m_dataVerticalMark.Clear();

            for (int i = 0; i <= DataPoints - 1; i++)
            {
                DataSerie objDataSerie = new DataSerie();
                objDataSerie.Value = 1000;
                objDataSerie.Time = i;
                m_dataVerticalMark.Add(objDataSerie);
            }
        }

        #endregion

        public async System.Threading.Tasks.Task Start()
        {
            await m_jbcConnect.SetContinuousModeAsync(m_stationID, SpeedContinuousMode.T_100mS, Port.NUM_1);
            m_queueID = System.Convert.ToUInt32(await m_jbcConnect.StartContinuousModeAsync(m_stationID));

            //Inicializamos los ticks y empezamos la recogida de datos
            m_tickCount = 0;
            m_timerPrintSeries.Start();
        }

        private void Tick_PrintSeries(object sender, EventArgs e)
        {
            m_jbcConnect.UpdateContinuousModeNextDataChunkAsync(m_stationID, m_queueID, 50);

            //Mientras hayan datos para leer
            while (m_jbcConnect.GetContinuousModeDataCount(m_stationID, m_queueID) > 0)
            {
                stContinuousModeData_HA continuousModeData = m_jbcConnect.GetContinuousModeNextData_HA(m_stationID, m_queueID);

                if (continuousModeData.data != null)
                {

                    //Se está pintando el profile
                    if (continuousModeData.data[0].status == ToolStatus_HA.HEATER)
                    {

                        //Es la primera muestra. Borrar series
                        if (m_toolStatus != ToolStatus_HA.HEATER)
                        {
                            m_toolStatus = ToolStatus_HA.HEATER;
                            InitializeDataHotAirTemp();
                            InitializeDataExtTCTemp();
                            InitializeDataAirFlow();
                            InitializeDataVerticalMark();
                        }

                        if (m_tickCount < DataPoints)
                        {

                            //Pintamos en la gráfica - Hot air temp
                            DataSerie dataSerieHotAirTemp = new DataSerie();
                            dataSerieHotAirTemp.Value = System.Convert.ToDouble(continuousModeData.data[0].temperature.ToCelsius());
                            dataSerieHotAirTemp.Time = m_tickCount;
                            DataHotAirTemp.RemoveAt(0);
                            DataHotAirTemp.Add(dataSerieHotAirTemp);

                            //Pintamos en la gráfica - External TC temp
                            DataSerie dataSerieExtTCTemp = new DataSerie();
                            dataSerieExtTCTemp.Value = System.Convert.ToDouble(continuousModeData.data[0].externalTC1_Temp.ToCelsius());
                            dataSerieExtTCTemp.Time = m_tickCount;
                            DataExtTCTemp.RemoveAt(0);
                            DataExtTCTemp.Add(dataSerieExtTCTemp);

                            //Pintamos en la gráfica - Air flow
                            DataSerie dataSerieAirFlow = new DataSerie();
                            dataSerieAirFlow.Value = System.Convert.ToDouble(continuousModeData.data[0].flow / 10); //viene en x_mil y se representa en x_cien
                            dataSerieAirFlow.Time = m_tickCount;
                            DataAirFlow.RemoveAt(0);
                            DataAirFlow.Add(dataSerieAirFlow);

                            //Pintamos en la gráfica - Vertical mark
                            DataSerie dataSerieVerticalMark = new DataSerie();
                            dataSerieVerticalMark.Value = -1;
                            dataSerieVerticalMark.Time = m_tickCount;
                            DataVerticalMark.RemoveAt(0);
                            DataVerticalMark.Add(dataSerieVerticalMark);

                            //Refrescamos cada medio segundo
                            if (m_tickCount % 5 == 0)
                            {

                                //Pintamos los labels
                                m_labelHotAirTemp.Text = continuousModeData.data[0].temperature.ToRoundCelsius() + "ºC";
                                m_labelExtTCTemp.Text = continuousModeData.data[0].externalTC1_Temp.ToRoundCelsius() + "ºC";
                                m_labelAirFlow.Text = (continuousModeData.data[0].flow / 10) + "%";

                                //Calculates time remaining
                                int timeInSeconds = System.Convert.ToInt32((DataPoints - m_tickCount) / 10);
                                int seconds = timeInSeconds % 60;
                                var timeInMinutes = timeInSeconds / 60;
                                int minutes = timeInMinutes % 60;

                                string sTime = minutes + "m ";
                                if (seconds < 10)
                                {
                                    sTime += "0";
                                }
                                sTime += seconds + "s";

                                m_labelStatus.Text = sTime;
                            }

                            m_tickCount++;
                        }

                    }
                    else if (continuousModeData.data[0].status == ToolStatus_HA.COOLING)
                    {

                        //Se acaba de parar de pintar el profile - Flag de bajada
                        if (m_toolStatus != ToolStatus_HA.COOLING)
                        {
                            m_toolStatus = ToolStatus_HA.COOLING;
                            m_labelStatus.Text = "Cooling";
                            m_tickCount = 0;
                        }

                    }
                    else if (continuousModeData.data[0].status == ToolStatus_HA.NONE)
                    {
                        if (m_toolStatus != ToolStatus_HA.NONE)
                        {
                            m_toolStatus = ToolStatus_HA.NONE;
                            m_labelStatus.Text = "Stop";
                            m_tickCount = 0;
                        }
                    }
                }
            }

        }

    }
}
