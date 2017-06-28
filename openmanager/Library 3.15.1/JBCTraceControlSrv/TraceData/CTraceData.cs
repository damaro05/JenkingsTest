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

using System.Threading;
using System.IO;

using JBC_Connect;
using DataJBC;

namespace JBCTraceControlLocalSrv
{
    public class CTraceData
    {

        private const int DEFAULT_FREQUENCY_TRACE_DATA = 1000; // 1 segundo
                                                               // el default de velocidad de captura es la velocidad de toma de datos definida al iniciar la lista de colas en DLL
        private const int DEFAULT_NUM_SEQUENCE = 600; // cantidad de datos en 1 archivo de trace
        private const string TRACE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
        private const string FILE_TIME_FORMAT = "yyyyMMddHHmmss";
        private const string LOG_TIME_FORMAT = "yyyyMMddHHmmss";
        private const string TRACE_FILE_EXTENSION = "json";
        private const string TRACE_TEMP_FILE_EXTENSION = "json.tmp";
        private const string TRACE_LOG_EXTENSION = "log";
        private const int CHUNK_SIZE = 10 * 1024; //10kb. The buffer size by default is set to 64kb

        private int frequencyTrace = DEFAULT_FREQUENCY_TRACE_DATA;
        private Hashtable m_htStationQueue = new Hashtable(); //stationUID  <-> QueueID
        private Hashtable m_htStationPortsData = new Hashtable(); //stationUID  <-> <port, trace data>
        private Hashtable m_htStationNumSequence = new Hashtable(); //stationUID  <-> Num data sequence
        private Hashtable m_htStationSpeed = new Hashtable(); //stationUID  <-> SpeedContinuousMode
        private Hashtable m_htStationID = new Hashtable(); //stationUID  <-> StationID

        private string m_folderData; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private Thread m_ThreadTraceData;
        private int m_MaxNumSequence = DEFAULT_NUM_SEQUENCE; // cantidad de datos en 1 archivo de trace

        public CTraceData(int iFileMaxNumSequence = 0)
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            m_folderData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\JBC Station Controller Service\\TraceData");

            m_MaxNumSequence = iFileMaxNumSequence;
            start();
        }

        private void start()
        {
            //Create folder to store the trace data file
            if (!(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DirectoryExists(m_folderData))
            {
                Directory.CreateDirectory(m_folderData);
            }

            repairBrokenFiles();

            //Start trace data
            m_ThreadTraceData = new Thread(new System.Threading.ThreadStart(TraceData));
            m_ThreadTraceData.IsBackground = true;
            m_ThreadTraceData.Start();

        }

        public int FileMaxSequence
        {
            get
            {
                return m_MaxNumSequence;
            }
        }

        public void close()
        {
            m_ThreadTraceData.Abort();
            repairBrokenFiles();
        }

        public void repairBrokenFiles()
        {
            //Move temporary files and check it
            DirectoryInfo di = new DirectoryInfo(m_folderData);
            foreach (var fi in di.GetFiles("*." + TRACE_TEMP_FILE_EXTENSION))
            {
                CloseJSONFile(System.Convert.ToString(fi.FullName));

                (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(Path.Combine(m_folderData, System.Convert.ToString(fi.Name)),
                    Path.Combine(m_folderData, System.Convert.ToString(fi.Name.Replace("." + TRACE_TEMP_FILE_EXTENSION, ""))) + "_" + DateTime.Now.ToString(FILE_TIME_FORMAT) + "." + TRACE_FILE_EXTENSION);
            }
        }

        public bool IsTraceStarted(string stationUID)
        {
            long stationID = DLLConnection.jbc.GetStationID(stationUID);
            if (stationID == -1)
            {
                return false;
            }
            if (m_htStationQueue.Contains(stationUID))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public SpeedContinuousMode TraceSpeed(string stationUID, SpeedContinuousMode captureSpeed)
        {
            // defines station queue speed
            // returns queue speed
            // if queue is already started, returns the speed of the started queue
            // setting speed as SpeedContinuousMode.OFF, the queue uses the station physical continuous mode speed
            // if you start a queue without setting SetTraceSpeed, SpeedContinuousMode.OFF is used, so defaults to physical speed (station->DLL)

            long stationID = DLLConnection.jbc.GetStationID(stationUID);
            if (stationID == -1)
            {
                return SpeedContinuousMode.OFF;
            }

            // defines station queue speed

            if (!m_htStationQueue.Contains(stationUID))
            {
                // if not queue started
                if (!m_htStationSpeed.Contains(stationUID))
                {
                    //speed
                    m_htStationSpeed.Add(stationUID, captureSpeed);
                }
                else
                {
                    m_htStationSpeed[stationUID] = captureSpeed;
                }
                return captureSpeed;
            }
            else
            {
                // if queue started
                return ((SpeedContinuousMode)(m_htStationSpeed[stationUID]));
            }
        }

        public void StartTraceData(string stationUID, dc_EnumConstJBC.dc_Port portNbr)
        {
            // start tracing one port

            long stationID = DLLConnection.jbc.GetStationID(stationUID);
            if (stationID == -1)
            {
                return;
            }

            //Start station continuous mode for the desired station and initialize num sequence
            if (!m_htStationQueue.Contains(stationUID))
            {
                //speed
                if (!m_htStationSpeed.Contains(stationUID))
                {
                    m_htStationSpeed.Add(stationUID, SpeedContinuousMode.OFF); // defaults to physical speed
                }

                //Num sequence
                m_htStationNumSequence.Add(stationUID, 0);
                //station ID
                m_htStationID.Add(stationUID, stationID);

                uint queueID = DLLConnection.jbc.StartContinuousMode(stationID, (SpeedContinuousMode)(m_htStationSpeed[stationUID]));

                m_htStationQueue.Add(stationUID, queueID);
            }

            //Add port to list
            Hashtable listPortsData = default(Hashtable);

            if (m_htStationPortsData.Contains(stationUID))
            {
                listPortsData = (Hashtable)(m_htStationPortsData[stationUID]);
            }
            else
            {
                listPortsData = new Hashtable();
            }

            if (!listPortsData.Contains((Port)portNbr))
            {
                listPortsData.Add((Port)portNbr, new TracePortData((Port)portNbr));
                m_htStationPortsData[stationUID] = listPortsData;
            }
            evlogWrite("Added trace port: " + portNbr.ToString(), stationUID);
        }

        public void StartTraceData(string stationUID)
        {
            // start tracing all ports

            long stationID = DLLConnection.jbc.GetStationID(stationUID);

            //Start station continuous mode for the desired station and initialize num sequence
            int countPorts = DLLConnection.jbc.GetPortCount(stationID);
            if (!m_htStationQueue.Contains(stationUID))
            {
                //speed
                if (!m_htStationSpeed.Contains(stationUID))
                {
                    m_htStationSpeed.Add(stationUID, SpeedContinuousMode.OFF); // defaults to physical speed
                }

                //Num sequence
                m_htStationNumSequence.Add(stationUID, 0);
                //station ID
                m_htStationID.Add(stationUID, stationID);

                uint queueID = DLLConnection.jbc.StartContinuousMode(stationID, (SpeedContinuousMode)(m_htStationSpeed[stationUID]));

                m_htStationQueue.Add(stationUID, queueID);
            }

            //Add ports to list
            Hashtable listPortsData = default(Hashtable);

            if (m_htStationPortsData.Contains(stationUID))
            {
                listPortsData = (Hashtable)(m_htStationPortsData[stationUID]);
            }
            else
            {
                listPortsData = new Hashtable();
            }

            for (var p = 0; p <= countPorts - 1; p++)
            {
                if (!listPortsData.Contains((Port)p))
                {
                    listPortsData.Add((Port)p, new TracePortData((Port)p));
                }
            }
            m_htStationPortsData[stationUID] = listPortsData;

            evlogWrite("Added trace all ports: " + countPorts.ToString(), stationUID);
        }

        public void StopTraceData(string stationUID, dc_EnumConstJBC.dc_Port portNbr)
        {
            // stop tracing one port

            if (m_htStationPortsData.Contains(stationUID))
            {
                Hashtable listPortData = (Hashtable)(m_htStationPortsData[stationUID]);

                if (listPortData.Contains((Port)portNbr))
                {
                    listPortData.Remove((Port)portNbr);
                }

                evlogWrite("Stop trace port: " + portNbr.ToString(), stationUID);

                //Comprobar si existe algún puerto mas que se esté trazando
                if (listPortData.Count == 0)
                {
                    long stationID = -1;
                    try
                    {
                        //stationID = jbc.GetStationID(stationUID)
                        stationID = System.Convert.ToInt64(m_htStationID[stationUID]);
                        DLLConnection.jbc.StopContinuousMode(stationID, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationQueue[stationUID])));
                    }
                    catch (Exception)
                    {
                    }

                    m_htStationQueue.Remove(stationUID);
                    m_htStationID.Remove(stationUID);
                    m_htStationPortsData.Remove(stationUID);
                    m_htStationNumSequence.Remove(stationUID);
                    m_htStationSpeed.Remove(stationUID);

                    evlogWrite("Stop trace station.", stationUID);

                    //Cierre y renombre del fichero con id único
                    if (File.Exists(tempPathFilename(m_folderData, stationID)))
                    {
                        (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(tempPathFilename(m_folderData, stationID), "}", true);
                        (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(tempPathFilename(m_folderData, stationID), tracePathFilename(m_folderData, stationID, DateTime.Now));
                    }
                }
            }
        }

        public void StopTraceData(string stationUID)
        {
            // stop tracing all ports

            if (m_htStationPortsData.Contains(stationUID))
            {
                long stationID = -1;
                try
                {
                    //stationID = jbc.GetStationID(stationUID)
                    stationID = System.Convert.ToInt64(m_htStationID[stationUID]);
                    DLLConnection.jbc.StopContinuousMode(stationID, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationQueue[stationUID])));
                }
                catch (Exception)
                {
                }

                m_htStationQueue.Remove(stationUID);
                m_htStationID.Remove(stationUID);
                m_htStationPortsData.Remove(stationUID);
                m_htStationNumSequence.Remove(stationUID);
                m_htStationSpeed.Remove(stationUID);

                evlogWrite("Stop trace all ports for station.", stationUID);

                //Cierre y renombre del fichero con id único
                if (File.Exists(tempPathFilename(m_folderData, stationID)))
                {
                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(tempPathFilename(m_folderData, stationID), "}", true);
                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(tempPathFilename(m_folderData, stationID), tracePathFilename(m_folderData, stationID, DateTime.Now));
                }
            }
        }

        public void StopTraceData()
        {
            // stop tracing all stations
            ArrayList listStationUID = new ArrayList();
            long stationID = -1;
            listStationUID.Clear();
            listStationUID.AddRange(m_htStationQueue.Keys);

            foreach (string stationUID in listStationUID)
            {
                try
                {
                    //stationID = jbc.GetStationID(stationUID)
                    stationID = System.Convert.ToInt64(m_htStationID[stationUID]);
                    DLLConnection.jbc.StopContinuousMode(stationID, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationQueue[stationUID])));
                }
                catch (Exception)
                {
                }

                m_htStationQueue.Remove(stationUID);
                m_htStationID.Remove(stationUID);
                m_htStationPortsData.Remove(stationUID);
                m_htStationNumSequence.Remove(stationUID);
                m_htStationSpeed.Remove(stationUID);

                evlogWrite("Stop trace all ports for station.", stationUID);

                //Cierre y renombre del fichero con id único
                if (File.Exists(tempPathFilename(m_folderData, stationID)))
                {
                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(tempPathFilename(m_folderData, stationID), "}", true);
                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(tempPathFilename(m_folderData, stationID), tracePathFilename(m_folderData, stationID, DateTime.Now));
                }
            }

        }

        public string GetFolderData()
        {
            return m_folderData;
        }

        public List<string> GetListRecordedDataFiles()
        {
            List<string> listRecordedData = new List<string>();

            DirectoryInfo di = new DirectoryInfo(m_folderData);
            foreach (var fi in di.GetFiles("*." + TRACE_FILE_EXTENSION))
            {
                listRecordedData.Add(fi.Name);
            }

            return listRecordedData;
        }

        public CTraceDataSequence GetRecordedData(string fileName, int nSequence)
        {
            CTraceDataSequence traceDataSequence = new CTraceDataSequence();
            byte[] bytes = new byte[1];

            try
            {
                if (File.Exists(Path.Combine(m_folderData, fileName.Trim())))
                {
                    FileStream fileStream = new FileStream(Path.Combine(m_folderData, fileName.Trim()), FileMode.Open, FileAccess.Read);
                    BinaryReader binaryReader = new BinaryReader(fileStream);
                    long seekPos = binaryReader.BaseStream.Seek((nSequence - 1) * CHUNK_SIZE, SeekOrigin.Begin);

                    bytes = binaryReader.ReadBytes(CHUNK_SIZE);
                    fileStream.Close();

                    traceDataSequence.final = bytes.Length < CHUNK_SIZE;
                    traceDataSequence.sequence = nSequence;
                    traceDataSequence.bytes = bytes;
                }
                else
                {
                    traceDataSequence.sequence = -1;
                }
            }
            catch (Exception ex)
            {
                traceDataSequence.sequence = -1;
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + ". Error: " + ex.Message.ToString());
            }

            return traceDataSequence;
        }

        public void DeleteRecordedDataFile(string fileName)
        {
            if (File.Exists(Path.Combine(m_folderData, fileName)))
            {
                File.Delete(Path.Combine(m_folderData, fileName));
            }
        }

        private void evlogWrite(string sText, string stationUID)
        {
            (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(logPathFilename(m_folderData), "[" + DateTime.Now.ToString(LOG_TIME_FORMAT) + "] Station [" + stationUID + "] " + sText + "\r\n", true);
        }

        private void TraceData()
        {
            ArrayList listStationUID = new ArrayList();
            Hashtable listPortData = default(Hashtable);
            ArrayList listPortDataKey = new ArrayList();
            Hashtable antListPortData = new Hashtable();
            stContinuousModeData_SOLD contData = new stContinuousModeData_SOLD(); // solder data
            stContinuousModeData_HA contData_HA = new stContinuousModeData_HA(); // hot air data
            stContinuousModePort_SOLD contDataPort = new stContinuousModePort_SOLD(); // solder data
            stContinuousModePort_HA contDataPort_HA = new stContinuousModePort_HA(); // hot air data

            do
            {
                // update stations
                listStationUID.Clear();
                listStationUID.AddRange(m_htStationQueue.Keys);

                //Iteramos en todas las estaciones
                foreach (string stationUID in listStationUID)
                {
                    long stationID = -1;
                    //Dim stationType As dc_EnumConstJBC.dc_StationType = dc_EnumConstJBC.dc_StationType.UNKNOWN
                    eStationType stationType = eStationType.UNKNOWN;

                    try
                    {
                        stationID = DLLConnection.jbc.GetStationID(stationUID);
                        stationType = DLLConnection.jbc.GetStationType(stationID);

                        if (stationID >= 0)
                        {

                            string JSONData = "";

                            //Listado de puertos que estamos trazando para la estación
                            listPortData = (Hashtable)(m_htStationPortsData[stationUID]);
                            listPortDataKey.Clear();
                            listPortDataKey.AddRange(listPortData.Keys);

                            int dataLength = DLLConnection.jbc.GetContinuousModeDataCount(stationID, (uint)(m_htStationQueue[stationUID]));
                            evlogWrite("Data records to process:" + dataLength.ToString(), stationUID);
                            for (int i = 0; i <= dataLength - 1; i++)
                            {

                                bool bDataExists = false;
                                switch (stationType)
                                {
                                    case eStationType.SOLD:
                                        contData = DLLConnection.jbc.GetContinuousModeNextData_SOLD(stationID, (uint)(m_htStationQueue[stationUID]));
                                        if (contData.data != null)
                                        {
                                            bDataExists = true;
                                        }
                                        break;
                                    case eStationType.HA:
                                        contData_HA = DLLConnection.jbc.GetContinuousModeNextData_HA(stationID, (uint)(m_htStationQueue[stationUID]));
                                        if (contData_HA.data != null)
                                        {
                                            bDataExists = true;
                                        }
                                        break;
                                }

                                if (bDataExists)
                                {

                                    JSONData = "";

                                    //Cabecera de datos si secuencia es cero
                                    if (System.Convert.ToInt32(m_htStationNumSequence[stationUID]) == 0)
                                    {
                                        // obtener la velocidad de la cola
                                        SpeedContinuousMode queueCaptureSpeed = DLLConnection.jbc.GetContinuousModeDeliverySpeed(stationID, (uint)(m_htStationQueue[stationUID]));
                                        // convertirla a integer
                                        CSpeedContMode speedcm = new CSpeedContMode();
                                        int frequency = speedcm.SpeedFromEnum(queueCaptureSpeed);
                                        // obtener datos identificativos
                                        string stnName = DLLConnection.jbc.GetStationName(stationID);
                                        string stnModel = DLLConnection.jbc.GetStationModel(stationID);
                                        string stnModelType = DLLConnection.jbc.GetStationModelType(stationID);
                                        string stnModelVersion = DLLConnection.jbc.GetStationModelVersion(stationID).ToString();
                                        string stnSW = DLLConnection.jbc.GetStationSWversion(stationID);
                                        string stnHW = DLLConnection.jbc.GetStationHWversion(stationID);

                                        JSONData = "{" + "\r\n" + "\"id\":\"" + stationUID + "\"," +
                                            "\r\n" + "\"time\":\"" + DateTime.Now.ToString(TRACE_TIME_FORMAT) + "\"," +
                                            "\r\n" + "\"name\":\"" + stnName + "\"," +
                                            "\r\n" + "\"type\":\"" + stationType.ToString() + "\"," +
                                            "\r\n" + "\"model\":\"" + stnModel + "\"," +
                                            "\r\n" + "\"modeltype\":\"" + stnModelType + "\"," +
                                            "\r\n" + "\"modelversion\":\"" + stnModelVersion + "\"," +
                                            "\r\n" + "\"software\":\"" + stnSW + "\"," +
                                            "\r\n" + "\"hardware\":\"" + stnHW + "\"," +
                                            "\r\n" + "\"interval\":" + System.Convert.ToString(frequency);
                                    }

                                    //Por cada puerto del modo continuo
                                    bool bComma = false;

                                    //Escribimos número de secuencia y datos
                                    JSONData += "," + "\r\n" + "\"" + System.Convert.ToString(m_htStationNumSequence[stationUID]) + "\":[";

                                    // datos de los puertos

                                    // cantidad de puertos
                                    int iDataPortCount = 0;
                                    switch (stationType)
                                    {
                                        case eStationType.SOLD:
                                            iDataPortCount = contData.data.Length;
                                            break;
                                        case eStationType.HA:
                                            iDataPortCount = contData_HA.data.Length;
                                            break;
                                    }

                                    // datos de los puertos
                                    //For Each contDataPort As stContinuousModePort In contData.data
                                    for (var x = 0; x <= iDataPortCount - 1; x++)
                                    {

                                        Port readingPort = Port.NO_PORT;
                                        switch (stationType)
                                        {
                                            case eStationType.SOLD:
                                                contDataPort = contData.data[x];
                                                readingPort = contDataPort.port;
                                                break;
                                            case eStationType.HA:
                                                contDataPort_HA = contData_HA.data[x];
                                                readingPort = contDataPort_HA.port;
                                                break;
                                        }

                                        //Copiamos los datos del registro anterior para comparar al crear el json
                                        TracePortData portData = new TracePortData();
                                        antListPortData.Clear();
                                        foreach (Port portDataKey in listPortDataKey)
                                        {
                                            portData = (TracePortData)(listPortData[portDataKey]);
                                            antListPortData.Add(portDataKey, portData.Clone());
                                        }

                                        //Si el puerto lo estamos trazando
                                        if (listPortData.Contains(readingPort))
                                        {
                                            switch (stationType)
                                            {
                                                case eStationType.SOLD:
                                                    portData = (TracePortData)(((TracePortData)(listPortData[contDataPort.port])).Clone());
                                                    portData.port = contDataPort.port;
                                                    portData.temperature = contDataPort.temperature.UTI;
                                                    portData.power = contDataPort.power;
                                                    portData.status = System.Convert.ToByte(contDataPort.status);
                                                    portData.tool = DLLConnection.jbc.GetPortToolID(stationID, portData.port);
                                                    break;
                                                case eStationType.HA:
                                                    portData = (TracePortData)(((TracePortData)(listPortData[contDataPort_HA.port])).Clone());
                                                    portData.port = contDataPort_HA.port;
                                                    portData.temperature = contDataPort_HA.temperature.UTI;
                                                    portData.power = contDataPort_HA.power;
                                                    portData.status = System.Convert.ToByte(contDataPort_HA.status);
                                                    portData.tool = DLLConnection.jbc.GetPortToolID(stationID, portData.port);
                                                    // HA
                                                    portData.flow = contDataPort_HA.flow;
                                                    portData.tempTC1 = contDataPort_HA.externalTC1_Temp.UTI;
                                                    portData.tempTC2 = contDataPort_HA.externalTC2_Temp.UTI;
                                                    portData.timetostop = contDataPort_HA.timeToStop;
                                                    break;
                                            }

                                            if (bComma)
                                            {
                                                JSONData += ",";
                                            }
                                            bComma = true;

                                            JSONData += "\r\n" + "{\"p\":" + System.Convert.ToString(portData.port);

                                            //Notificar tool en la primera entrada de datos o cada vez que haya un cambio
                                            // LA TOOL en TEXTO
                                            if (System.Convert.ToInt32(m_htStationNumSequence[stationUID]) == 0 | portData.tool != ((TracePortData)(antListPortData[portData.port])).tool)
                                            {
                                                JSONData += ", \"o\":" + System.Convert.ToString(portData.tool) + "";
                                            }

                                            //Si no hay tool, no notificar de status, temperatura ni power
                                            if (portData.tool != GenericStationTools.NO_TOOL)
                                            {

                                                //Notificar status en la primera entrada de datos o cada vez que haya un cambio
                                                if (System.Convert.ToInt32(m_htStationNumSequence[stationUID]) == 0 | portData.status != ((TracePortData)(antListPortData[portData.port])).status)
                                                {
                                                    JSONData += ", \"s\":" + System.Convert.ToString(portData.status);
                                                }

                                                // temperature
                                                JSONData += ", \"t\":" + System.Convert.ToString(portData.temperature);

                                                //Si status es extractor o hibernation no notificar el power
                                                if (portData.status != (byte)ToolStatus.EXTRACTOR & portData.status != (byte)ToolStatus.HIBERNATION)
                                                {
                                                    JSONData += ", \"w\":" + System.Convert.ToString(portData.power);
                                                }

                                                // HA
                                                if (stationType == eStationType.HA)
                                                {
                                                    // flow
                                                    JSONData += ", \"f\":" + System.Convert.ToString(portData.flow);

                                                    // time to stop (en décimas de segundo)
                                                    JSONData += ", \"ts\":" + System.Convert.ToString(portData.timetostop);

                                                    // viene &HFF si no hay termopar externo
                                                    if (portData.tempTC1 > 0 & portData.tempTC1 < 0xFF)
                                                    {
                                                        JSONData += ", \"x1\":" + System.Convert.ToString(portData.tempTC1);
                                                    }

                                                    // viene &HFF si no hay termopar externo
                                                    if (portData.tempTC2 > 0 & portData.tempTC2 < 0xFF)
                                                    {
                                                        JSONData += ", \"x2\":" + System.Convert.ToString(portData.tempTC2);
                                                    }

                                                }
                                            }

                                            JSONData += "}";

                                        }

                                    } // For Each contDataPort As stContinuousModePort In contData.data

                                    JSONData += "\r\n" + "]";
                                    (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(tempPathFilename(m_folderData, stationID), JSONData, true);

                                    //Incrementar numero de secuencia
                                    m_htStationNumSequence[stationUID] = (System.Convert.ToInt32(m_htStationNumSequence[stationUID])) + 1;

                                    //Final de fichero
                                    if (System.Convert.ToInt32(m_htStationNumSequence[stationUID]) == m_MaxNumSequence)
                                    {
                                        m_htStationNumSequence[stationUID] = 0;

                                        //Cierre y renombre del fichero con id único
                                        (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(tempPathFilename(m_folderData, stationID), "}", true);
                                        (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(tempPathFilename(m_folderData, stationID), tracePathFilename(m_folderData, stationID, DateTime.Now));
                                    }

                                } // If contData.data IsNot Nothing Then

                            } // For i As Integer = 0 To dataLength
                        }
                        else
                        {
                            // station ID not found (disconnected?)
                            // Stop trace data
                            m_htStationQueue.Remove(stationUID);
                            m_htStationID.Remove(stationUID);
                            m_htStationPortsData.Remove(stationUID);
                            m_htStationNumSequence.Remove(stationUID);
                            m_htStationSpeed.Remove(stationUID);

                            //close and rename file with unique name
                            if (File.Exists(tempPathFilename(m_folderData, stationID)))
                            {
                                (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(tempPathFilename(m_folderData, stationID), "}", true);
                            }
                            if (File.Exists(tempPathFilename(m_folderData, stationID)))
                            {
                                (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(tempPathFilename(m_folderData, stationID), tracePathFilename(m_folderData, stationID, DateTime.Now));
                            }

                            evlogWrite("Station ID not found. Station disconnected?", stationUID);
                        }

                    }
                    catch (Exception ex)
                    {
                        // error
                        //Stop trace data
                        m_htStationQueue.Remove(stationUID);
                        m_htStationID.Remove(stationUID);
                        m_htStationPortsData.Remove(stationUID);
                        m_htStationNumSequence.Remove(stationUID);
                        m_htStationSpeed.Remove(stationUID);

                        //close and rename file with unique name
                        if (File.Exists(tempPathFilename(m_folderData, stationID)))
                        {
                            (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(tempPathFilename(m_folderData, stationID), "}", true);
                            (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(tempPathFilename(m_folderData, stationID), tracePathFilename(m_folderData, stationID, DateTime.Now));
                        }

                        evlogWrite(System.Reflection.MethodBase.GetCurrentMethod().Name + ". Error: " + ex.Message, stationUID);
                    }
                }

                Thread.Sleep(frequencyTrace);
            } while (true);
        }


        private void CloseJSONFile(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            FileStream fs = default(FileStream);
            byte[] b = new byte[1];

            //Check last character for correct format
            fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(-1, SeekOrigin.End);
            if (fs.Read(b, b.Length - 1, b.Length) > 0)
            {
                if (System.Text.Encoding.UTF8.GetString(b) != "}")
                {
                    fs.Write(System.Text.Encoding.UTF8.GetBytes("}"), 0, 1);
                }
            }
            fs.Close();
        }

        private string tracePathFilename(string sFolderData, long stationID, DateTime datetimeData)
        {
            return Path.Combine(sFolderData, (stationID).ToString()) + "_" + datetimeData.ToString(FILE_TIME_FORMAT) + "." + TRACE_FILE_EXTENSION;
        }

        private string tempPathFilename(string sFolderData, long stationID)
        {
            return Path.Combine(sFolderData, (stationID).ToString()) + "." + TRACE_TEMP_FILE_EXTENSION;
        }

        private string logPathFilename(string sFolderData)
        {
            return Path.Combine(sFolderData, "TraceLog") + "." + TRACE_LOG_EXTENSION;
        }

    }
}
