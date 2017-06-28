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
using JBC_ConnectRemote;
// End of VB project level imports

using System.Threading.Tasks;

// 20/10/2014 Se modifica getSleepStatus para devuelva sleep también en hibernation y stand
// 25/03/2015 Uso de multiples colas de modo continuo

namespace RemoteManRegister
{
    public class Cstation
    {
        //Constants definitions
        //-> Port list constants
        private const int MAX_PORTS = 4;

        //-> get data constants
        public const int TEMPERATURE = 0;
        public const int INTENSITY = 2;
        public const int POWER = 1;
        public const int FLOW = 3;

        //-> Temperature filter constants
        private const int TEMP_FILTER_SIZE = 1;

        //type definitions
        public struct tPort
        {
            public Port number;
            public GenericStationTools tool;
            public int[] T;
            public int nTrans;
            public int I;
            public int P;
            public int F; // hadesold flow
            public ToolStatus status;
            public ToolStatus_HA status_HA; // hadesold
        }

        //Global var's
        //Private WithEvents serial As System.IO.Ports.SerialPort
        private tPort[] portList = new tPort[MAX_PORTS - 1 + 1];
        private ulong ID = UInt64.MaxValue;
        //hadesold
        private eStationType StationType = eStationType.SOLD;
        private List<byte> byteList = new List<byte>();
        private bool lockList = false;
        private uint uiContModeQueueID; // 25/03/2015 Uso de multiples colas de modo continuo: queueID de esta instancia de estación

        private JBC_API_Remote jbc = null;

        //Properties
        public dynamic myStationID
        {
            get
            {
                return ID;
            }
        }

        public dynamic myStationType
        {
            get
            {
                return StationType;
            }
        }

        // 25/03/2015 Uso de multiples colas de modo continuo
        public uint myContModeQueueID
        {
            get
            {
                return uiContModeQueueID;
            }
            set
            {
                uiContModeQueueID = value;
            }
        }

        //Public functions
        public void init(ulong stationID, eStationType _stationType, JBC_API_Remote _jbc)
        {
            jbc = _jbc;
            //setting the ID
            ID = stationID;
            StationType = _stationType;
            uiContModeQueueID = UInt32.MaxValue;

            //initializing the port's structures
            for (int cnt = 0; cnt <= MAX_PORTS - 1; cnt++)
            {
                portList[cnt].I = 0;
                portList[cnt].number = Port.NO_PORT;
                portList[cnt].P = 0;
                portList[cnt].F = 0; // hadesold flow
                portList[cnt].status = ToolStatus.NONE;
                portList[cnt].status_HA = ToolStatus_HA.NONE;
                portList[cnt].T = new int[TEMP_FILTER_SIZE];
                for (int i = 0; i <= TEMP_FILTER_SIZE - 1; i++)
                {
                    portList[cnt].T[i] = 0;
                }
                portList[cnt].nTrans = 0;
                portList[cnt].tool = GenericStationTools.NO_TOOL;
            }

            // Setting station control mode
            //JBC.SetControlMode(ID, JBC_API.OnOff._ON)

            // Setting the station in continuous mode for all of its ports
            // 13/01/2014 relocated to CPlot class
            // 25/03/2015 relocated here again due to multiples queues managing (see startStationContinuousMode)
            //Dim count As Integer = jbc.GetPortCount(ID)
            //Dim a As Integer = JBC_API.Port.NO_PORT
            //Dim b As Integer = JBC_API.Port.NO_PORT
            //Dim c As Integer = JBC_API.Port.NO_PORT
            //Dim d As Integer = JBC_API.Port.NO_PORT
            //If count >= 1 Then a = JBC_API.Port.NUM_1
            //If count >= 2 Then b = JBC_API.Port.NUM_2
            //If count >= 3 Then c = JBC_API.Port.NUM_3
            //If count >= 4 Then d = JBC_API.Port.NUM_4
            //jbc.SetContinuousMode(ID, JBC_API.SpeedContinuousMode.T_10mS, a, b, c, d)
        }

        public void finish()
        {
            // Stopping station continuous mode
            // 13/01/2014 relocated to CPlot class
            //jbc.SetContinuousMode(ID, JBC_API.SpeedContinuousMode.OFF)
            // 25/03/2015 relocated here again due to multiples queues managing
            stopStationContinuousMode();
        }

        public async void startStationContinuousMode(SpeedContinuousMode speed = default(SpeedContinuousMode))
        {
            // start new continuous mode queue for all of its ports
            int count = jbc.GetPortCount((long)ID);
            int a = (int)Port.NO_PORT;
            int b = (int)Port.NO_PORT;
            int c = (int)Port.NO_PORT;
            int d = (int)Port.NO_PORT;
            if (count >= 1)
            {
                a = (int)Port.NUM_1;
            }
            if (count >= 2)
            {
                b = (int)Port.NUM_2;
            }
            if (count >= 3)
            {
                c = (int)Port.NUM_3;
            }
            if (count >= 4)
            {
                d = (int)Port.NUM_4;
            }
            try
            {
                await jbc.SetContinuousModeAsync((long)ID, speed, (Port)a, (Port)b, (Port)c, (Port)d);
                uiContModeQueueID = System.Convert.ToUInt32(await jbc.StartContinuousModeAsync((long)ID));
                //Debug.Print("startStationContinuousMode")
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cstation::startStationContinuousMode . error:" + ex.Message);
            }
        }

        public async void stopStationContinuousMode()
        {
            try
            {
                // Stopping station continuous mode queue
                //Debug.Print("stopStationContinuousMode")
                if (uiContModeQueueID != UInt32.MaxValue)
                {
                    await jbc.StopContinuousModeAsync((long)ID, uiContModeQueueID);
                    uiContModeQueueID = UInt32.MaxValue;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cstation::stopStationContinuousMode . error:" + ex.Message);
            }
        }

        public async Task<int> getData(int port, int dataType, int value)
        {
            //Public Function getData(ByVal port As Integer, ByVal dataType As Integer, ByVal value As Integer) As Integer
            //getting the required data
            await updatePortsAsync();
            //retunring the data
            if (dataType == TEMPERATURE)
            {
                // Applying the temperature filter to the new value
                int sum = 0;
                for (int i = 0; i <= TEMP_FILTER_SIZE - 1; i++)
                {
                    sum = sum + portList[port - 1].T[i];
                }
                value = System.Convert.ToInt32((double)sum / TEMP_FILTER_SIZE);
            }
            if (dataType == INTENSITY)
            {
                value = portList[port - 1].I;
            }
            if (dataType == POWER)
            {
                value = portList[port - 1].P;
            }
            if (dataType == FLOW)
            {
                value = portList[port - 1].F;
            }
            return value;
        }

        public async Task<bool> getSleepStatus(int port)
        {
            bool isSleeping = false;
            try
            {
                // devuelve si el modo continuo está activado
                // y si está en sleep, que sirve para empezar el gráfico en las modalidades Auto y Single
                CContinuousModeStatus cmStatus = await jbc.GetContinuousModeAsync((long)ID);
                if (cmStatus.speed == SpeedContinuousMode.OFF)
                {
                    // si el modo continuo está desactivado, obtener el sleep del estado del puerto
                    // get remote WCF data
                    await jbc.UpdatePortStatusAsync((long)ID, (Port)(port - 1));
                    // check port status
                    if (StationType == eStationType.SOLD)
                    {
                        if (jbc.GetPortToolSleepStatus((long)ID, (Port)(port - 1)) == OnOff._ON ||
                            jbc.GetPortToolHibernationStatus((long)ID, (Port)(port - 1)) == OnOff._ON ||
                            jbc.GetPortToolStandStatus((long)ID, (Port)(port - 1)) == OnOff._ON)
                        {
                            isSleeping = true;
                        }
                        else
                        {
                            isSleeping = false;
                        }
                    }
                    if (StationType == eStationType.HA)
                    {
                        if (jbc.GetPortToolHeaterStatus((long)ID, (Port)(port - 1)) == OnOff._ON)
                        {
                            isSleeping = false;
                        }
                        else
                        {
                            isSleeping = true;
                        }
                    }
                    //Debug.Print("isSleeping cont mode OFF: {0}", isSleeping.ToString)
                }
                else
                {
                    // si el modo continuo está activado, se obtiene el sleep de los datos del modo continuo
                    //getting the required data
                    await updatePortsAsync();
                    //retunring the data
                    if (StationType == eStationType.SOLD)
                    {
                        //isSleeping = (portList(port - 1).status = JBC_API.ToolStatus.SLEEP)
                        if (portList[port - 1].status == ToolStatus.SLEEP |
                                portList[port - 1].status == ToolStatus.HIBERNATION |
                                portList[port - 1].status == ToolStatus.STAND)
                        {
                            isSleeping = true;
                        }
                        else
                        {
                            isSleeping = false;
                        }
                    }
                    if (StationType == eStationType.HA)
                    {
                        if (portList[port - 1].status_HA == ToolStatus_HA.HEATER | portList[port - 1].status_HA == ToolStatus_HA.HEATER_REQUESTED)
                        {
                            isSleeping = false;
                        }
                        else
                        {
                            isSleeping = true;
                        }
                    }
                    //Debug.Print("isSleeping cont mode ON: {0}", isSleeping.ToString)
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Cstation::getSleepStatus . error:" + ex.Message);
                isSleeping = false;
            }

            return isSleeping;
        }

        //Private functions
        private bool updatePorts()
        {
            try
            {
                jbc.UpdateContinuousModeNextDataChunk((long)ID, uiContModeQueueID, 50);

                // Initializing the ports magnitudes to 0 to calculate the median
                //Debug.Print("DATA: " & JBC.GetContinuousModeDataCount(ID))
                if (jbc.GetContinuousModeDataCount((long)ID, uiContModeQueueID) > 0)
                {
                    for (int cnt = 0; cnt <= MAX_PORTS - 1; cnt++)
                    {
                        portList[cnt].P = 0;
                        portList[cnt].F = 0;

                        // Displacement of the temperature values in the filter
                        for (int i = 0; i <= TEMP_FILTER_SIZE - 1 - 1; i++)
                        {
                            portList[cnt].T[i] = portList[cnt].T[i + 1];
                        }
                        portList[cnt].T[TEMP_FILTER_SIZE - 1] = 0;
                        //portList(cnt).nTrans = 0
                    }
                }

                // Getting all the transmisions from the station and calculating the median temperature and power. Power units are in per thousand and
                // is necessary to pass them into per hundred
                int transmisionCounter = 0;

                stContinuousModeData_SOLD ports = new stContinuousModeData_SOLD();
                stContinuousModeData_HA ports_HA = new stContinuousModeData_HA(); // hadesold
                while (jbc.GetContinuousModeDataCount((long)ID, uiContModeQueueID) > 0)
                {
                    // sold
                    if (StationType == eStationType.SOLD)
                    {
                        ports = jbc.GetContinuousModeNextData((long)ID, uiContModeQueueID);
                        if (ports.data != null)
                        {
                            foreach (stContinuousModePort_SOLD dt in ports.data)
                            {
                                // portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) + dt.temperature.ToCelsius
                                // get data in UTI
                                portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] + dt.temperature.UTI;
                                //If dt.temperature.ToCelsius <> 0 Then portList(dt.port).nTrans = portList(dt.port).nTrans + 1
                                portList[(int)dt.port].I = 0;
                                portList[(int)dt.port].P = System.Convert.ToInt32(portList[(int)dt.port].P + dt.power);
                            }
                            transmisionCounter++;
                        }
                    }
                    // hadesold
                    if (StationType == eStationType.HA)
                    {
                        ports_HA = jbc.GetContinuousModeNextData_HA((long)ID, uiContModeQueueID);
                        if (ports_HA.data != null)
                        {
                            foreach (stContinuousModePort_HA dt in ports_HA.data)
                            {
                                // portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) + dt.temperature.ToCelsius
                                // get data in UTI
                                portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] + dt.temperature.UTI;
                                //If dt.temperature.ToCelsius <> 0 Then portList(dt.port).nTrans = portList(dt.port).nTrans + 1
                                portList[(int)dt.port].I = 0;
                                portList[(int)dt.port].P = System.Convert.ToInt32(portList[(int)dt.port].P + dt.power);
                                portList[(int)dt.port].F = System.Convert.ToInt32(portList[(int)dt.port].F + dt.flow);
                            }
                            transmisionCounter++;
                        }
                    }

                }

                if (transmisionCounter > 0)
                {
                    if (StationType == eStationType.SOLD)
                    {
                        foreach (stContinuousModePort_SOLD dt in ports.data)
                        {
                            portList[(int)dt.port].number = dt.port + 1;
                            portList[(int)dt.port].status = dt.status;
                            //If portList(dt.port).nTrans <> 0 Then portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) / portList(dt.port).nTrans
                            portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] / transmisionCounter;
                            portList[(int)dt.port].P = System.Convert.ToInt32(((double)portList[(int)dt.port].P / 10) / transmisionCounter);
                            portList[(int)dt.port].I = 0;
                        }
                    }
                    if (StationType == eStationType.HA)
                    {
                        foreach (stContinuousModePort_HA dt in ports_HA.data)
                        {
                            portList[(int)dt.port].number = dt.port + 1;
                            portList[(int)dt.port].status = (ToolStatus)dt.status;
                            //If portList(dt.port).nTrans <> 0 Then portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) / portList(dt.port).nTrans
                            portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] / transmisionCounter;
                            portList[(int)dt.port].P = System.Convert.ToInt32(((double)portList[(int)dt.port].P / 10) / transmisionCounter);
                            portList[(int)dt.port].F = System.Convert.ToInt32(((double)portList[(int)dt.port].F / 10) / transmisionCounter); // hadesold
                            portList[(int)dt.port].I = 0;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cstation::updatePorts . error:" + ex.Message);
                return false;
            }
        }

        //Private functions
        private async System.Threading.Tasks.Task<bool> updatePortsAsync()
        {
            try
            {
                await jbc.UpdateContinuousModeNextDataChunkAsync((long)ID, uiContModeQueueID, 50);

                // Initializing the ports magnitudes to 0 to calculate the median
                //Debug.Print("DATA: " & JBC.GetContinuousModeDataCount(ID))
                if (jbc.GetContinuousModeDataCount((long)ID, uiContModeQueueID) > 0)
                {
                    for (int cnt = 0; cnt <= MAX_PORTS - 1; cnt++)
                    {
                        portList[cnt].P = 0;
                        portList[cnt].F = 0;

                        // Displacement of the temperature values in the filter
                        for (int i = 0; i <= TEMP_FILTER_SIZE - 1 - 1; i++)
                        {
                            portList[cnt].T[i] = portList[cnt].T[i + 1];
                        }
                        portList[cnt].T[TEMP_FILTER_SIZE - 1] = 0;
                        //portList(cnt).nTrans = 0
                    }
                }

                // Getting all the transmisions from the station and calculating the median temperature and power. Power units are in per thousand and
                // is necessary to pass them into per hundred
                int transmisionCounter = 0;

                stContinuousModeData_SOLD ports = new stContinuousModeData_SOLD();
                stContinuousModeData_HA ports_HA = new stContinuousModeData_HA(); // hadesold
                while (jbc.GetContinuousModeDataCount((long)ID, uiContModeQueueID) > 0)
                {
                    // sold
                    if (StationType == eStationType.SOLD)
                    {
                        ports = jbc.GetContinuousModeNextData((long)ID, uiContModeQueueID);
                        if (ports.data != null)
                        {
                            foreach (stContinuousModePort_SOLD dt in ports.data)
                            {
                                // portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) + dt.temperature.ToCelsius
                                // get data in UTI
                                portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] + dt.temperature.UTI;
                                //If dt.temperature.ToCelsius <> 0 Then portList(dt.port).nTrans = portList(dt.port).nTrans + 1
                                portList[(int)dt.port].I = 0;
                                portList[(int)dt.port].P = System.Convert.ToInt32(portList[(int)dt.port].P + dt.power);
                            }
                            transmisionCounter++;
                        }
                    }
                    // hadesold
                    if (StationType == eStationType.HA)
                    {
                        ports_HA = jbc.GetContinuousModeNextData_HA((long)ID, uiContModeQueueID);
                        if (ports_HA.data != null)
                        {
                            foreach (stContinuousModePort_HA dt in ports_HA.data)
                            {
                                // portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) + dt.temperature.ToCelsius
                                // get data in UTI
                                portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] + dt.temperature.UTI;
                                //If dt.temperature.ToCelsius <> 0 Then portList(dt.port).nTrans = portList(dt.port).nTrans + 1
                                portList[(int)dt.port].I = 0;
                                portList[(int)dt.port].P = System.Convert.ToInt32(portList[(int)dt.port].P + dt.power);
                                portList[(int)dt.port].F = System.Convert.ToInt32(portList[(int)dt.port].F + dt.flow);
                            }
                            transmisionCounter++;
                        }
                    }

                }

                if (transmisionCounter > 0)
                {
                    if (StationType == eStationType.SOLD)
                    {
                        foreach (stContinuousModePort_SOLD dt in ports.data)
                        {
                            portList[(int)dt.port].number = dt.port + 1;
                            portList[(int)dt.port].status = dt.status;
                            //If portList(dt.port).nTrans <> 0 Then portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) / portList(dt.port).nTrans
                            portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] / transmisionCounter;
                            portList[(int)dt.port].P = System.Convert.ToInt32(((double)portList[(int)dt.port].P / 10) / transmisionCounter);
                            portList[(int)dt.port].I = 0;
                        }
                    }
                    if (StationType == eStationType.HA)
                    {
                        foreach (stContinuousModePort_HA dt in ports_HA.data)
                        {
                            portList[(int)dt.port].number = dt.port + 1;
                            portList[(int)dt.port].status = (ToolStatus)dt.status;
                            //If portList(dt.port).nTrans <> 0 Then portList(dt.port).T(TEMP_FILTER_SIZE - 1) = portList(dt.port).T(TEMP_FILTER_SIZE - 1) / portList(dt.port).nTrans
                            portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] = portList[(int)dt.port].T[TEMP_FILTER_SIZE - 1] / transmisionCounter;
                            portList[(int)dt.port].P = System.Convert.ToInt32(((double)portList[(int)dt.port].P / 10) / transmisionCounter);
                            portList[(int)dt.port].F = System.Convert.ToInt32(((double)portList[(int)dt.port].F / 10) / transmisionCounter); // hadesold
                            portList[(int)dt.port].I = 0;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cstation::updatePorts . error:" + ex.Message);
                return false;
            }
        }
    }
}
