using System;
using System.Collections.Generic;

namespace JBC_Connect
{
    //Protocolo de conexión versión 01
    // se añade el pedido de FIRMWARE
    //EQUIPO                     PC
    //--------------------------------
    //   NAK ----------------->
    //       <-----------------  SYN
    //   ACK ----------------->
    //       <-----------------  ACK
    //   #num----------------->
    //       <-----------------  ACK
    //       <-----------------  M_FIRMWARE (21)
    //   M_FIRMWARE (21) ----->          (datos: PROTOCOLVERSION : STATION : SOFTWARE : HARDWARE)
    //--------------------------------

    //Protocolo de conexión versión 02: frame de handshaking
    // FRAME
    //Start  -   Cabecera                                -   Información	-   Check	-   Stop
    //1byte	-   1byte	1byte	1byte	1byte	1byte	-   #cant bytes	-   1byte	-   1byte
    //STX	-   Origen	Destino	UID=FDh	Comando	#cant	-   Datos	    -   BCC	    -   ETX
    //                   #numdevice              datos       #numdevice

    //EQUIPO                         PC
    //----------------------------------
    //   M_HS (00) ----------->          (handshake - datos: #num)
    //       <-----------------  M_HS (00) con ACK en datos
    //       <-----------------  M_FIRMWARE (21)
    //   M_FIRMWARE (21) ----->          (datos: PROTOCOLVERSION : STATION : SOFTWARE : HARDWARE)
    //----------------------------------


    internal abstract class CSearchConnect
    {
        protected const byte ASCII_STX = 0x2;
        protected const byte ASCII_ACK = 0x6;
        protected const byte ASCII_DLE = 0x10; // para verificar DLE-STX, DLE-ETX, DLE-DLE
        protected const byte ASCII_NAK = 0x15;
        protected const byte ASCII_SYN = 0x16;

        protected const int MS_NEW_SEARCH = 1; //Espera como máximo 1ms (lo mínimo)para generar un nuevo evento por tiempo

        private List<byte> STATION_ADDRESS_AVAILABLES = new List<byte>() { 0x0, 0x10 };

        protected int MS_WAIT_ACK; //Espera como máximo 100ms, aunque la respuesta ha de ser inmediata
        protected int MS_WAIT_FIRMWARE; //Espera como máximo 100ms


        protected enum StatusConnect : byte
        {
            StopSearch, // estado sin buscar nuevas conexiones
            WaitSearch, // estado de espera antes de comenzar una nueva busqueda de puertos
            Search, // estado inicial epera a que toque una nueva busqueda por tiempo
            WaitNAKorHS, // espera un caracter NAK (protocolo 1) o un inicio de trama (protocolo 2)
            WaitACKofHS, // espera un caracter ACK o NAK por un Handshake iniciado desde el PC (protocolo 2)
            RetryHS, // volver a intentar una conexión Handshake desde el PC
                     // protocolo de conexión 1
            WaitACK, // espera un caracter ACK
            WaitNum, // espera un caracter con el número del PC NumDevice
                     // protocolo de conexión 2
            WaitHS, // espera más trama HS (protocolo 2)
            WaitFW // espera una trama firmware
        }

        // para protocolo de conexión 1
        protected enum EnumReceived : byte
        {
            Null,
            NAK, // se ha recibido todo NAK
            ACK, // Ha llegado todo NAK y el último es ACK
            Other // Ha llegado algo que no es NAK o ACK
        }


        protected StatusConnect m_StatusConnect = new StatusConnect();
        // protocolo de conexión utilizado (NAK=01 o tramas=02)
        protected CStationBase.Protocol m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_undefined;

        protected byte m_pcNumDevice;
        protected byte m_stationNumDevice;

        protected CFrameData m_FrameDataIn01 = new CFrameData(CStationBase.Protocol.Protocol_01); // buffer de entrada trama protocolo 01
        protected CFrameData m_FrameDataOut01 = new CFrameData(CStationBase.Protocol.Protocol_01); // buffer de salida trama protocolo 01
        protected CFrameData m_FrameDataIn02 = new CFrameData(CStationBase.Protocol.Protocol_02); // buffer de entrada trama protocolo 02
        protected CFrameData m_FrameDataOut02 = new CFrameData(CStationBase.Protocol.Protocol_02); // buffer de salida trama protocolo 02
        protected CDllFifoMessages m_MessageFIFOIn = new CDllFifoMessages();

        protected System.Timers.Timer m_Timer_Search = new System.Timers.Timer();

        protected object m_LockParseDataReceived = new object();


        protected abstract void SendBytes(byte[] _bytes);
        protected abstract byte[] ReadBytes(int numBytes = 0);
        protected abstract void RaiseNewConnection(CStationBase.Protocol commandProtocol, string sStationModel, string SoftwareVersion, string HardwareVersion);
        protected abstract void CloseConnection();

        protected void ParseDataReceived()
        {
            byte[] DataIn = null;
            byte[] DataOut = null;
            bool Conecta = false;
            EnumReceived ReceivedWaitACK = default(EnumReceived);
            string sStationModel = "";
            int iErrors = 0; //Unused
            string sCommandProtocol = "";
            string SoftwareVersion = "";
            string HardwareVersion = "";

            switch (m_StatusConnect)
            {
                case StatusConnect.WaitNAKorHS:
                case StatusConnect.WaitHS:
                    // esperanndo NAK de protocolo 01 o inicio de trama handshake (DLE-STX) de protocolo 02

                    DataIn = ReadBytes();

                    //
                    // Conexión protocolo 02 (por tramas)
                    //

                    // Nuevo protocolo 2: puede venir una trama de handshaking
                    // verificar si viene una trama y cargar en FrameDatain
                    if (((DataIn.Length > 0 && DataIn[0] == ASCII_DLE) & m_StatusConnect == StatusConnect.WaitNAKorHS) ||
                        (m_StatusConnect == StatusConnect.WaitHS))
                    {
                        m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_02;

                        // seguir leyendo en buffer de entrada
                        iErrors = m_FrameDataIn02.DecodeCheckReceivedData(DataIn, -1, ref m_MessageFIFOIn);

                        // ha recibido un frame
                        if (m_MessageFIFOIn.Number > 0)
                        {
                            // verificar trama Handshake y obtener número de dispositivo del PC asignado por la estación
                            // si conecta = false -> frames leídos sin handshaking
                            Conecta = false;
                            foreach (CDllFifoMessages.MessageDll message in m_MessageFIFOIn.GetTable)
                            {
                                // FID a recibir FDh
                                // Control Handshake 00h
                                // El número de dispositivo asignado vendrá tanto en TargetDevice como en los datos
                                // Filtramos las direcciones origen
                                if (message.Command == (byte)EnumCommandFrame_02_SOLD.M_HS &&
                                    message.FID == 0xFD &&
                                    STATION_ADDRESS_AVAILABLES.Contains(message.SourceDevice))
                                {
                                    m_pcNumDevice = message.TargetDevice;
                                    m_stationNumDevice = message.SourceDevice;
                                    Conecta = true;
                                    break;
                                }
                            }

                        }
                        else if (m_FrameDataIn02.GetFrame().Length > 0)
                        {
                            // ha recibido parte de un frame, seguir leyendo
                            m_StatusConnect = StatusConnect.WaitHS;
                            break;
                        }
                        else
                        {
                            Conecta = false;
                        }


                    //
                    // Conexión protocolo 01
                    //
                    }
                    else
                    {
                        foreach (byte byt in DataIn)
                        {
                            if (byt == ASCII_NAK)
                            {
                                // protocolo conexión 01
                                Conecta = true;
                                m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_01;
                            }
                            else
                            {
                                // si no es protocolo conexión 02, todos han de ser NAK
                                m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_undefined;
                                Conecta = false;
                                break;
                            }
                        }
                    }

                    //Conexion no válida
                    if (Conecta == false)
                    {
                        CloseConnection();
                        m_StatusConnect = StatusConnect.Search;

                        m_Timer_Search.Stop();
                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                        m_Timer_Search.Start();
                        break;
                    }

                    // conexión válida -> responder, según protocolo
                    try
                    {
                        switch (m_Conection_FrameProtocol)
                        {
                            case CStationBase.Protocol.Protocol_01:
                                // send SYN
                                SendBytes(new byte[] { ASCII_SYN });

                                m_Timer_Search.Interval = MS_WAIT_ACK;
                                m_StatusConnect = StatusConnect.WaitACK;
                                break;

                            case CStationBase.Protocol.Protocol_02:
                                // responder con Handshake y ACK en Datos
                                // 04/04/2016 Limpiar buffer de entrada para la próxima consulta
                                m_MessageFIFOIn.Reset(); // inicializa los mensajes de entrada
                                m_FrameDataIn02.Reset(); // inicializa buffer de entrada

                                DataOut = new byte[1];
                                DataOut[0] = (byte)EnumCommandFrame_02_SOLD.M_ACK;
                                // XOR porque es respuesta
                                m_FrameDataOut02.EncodeFrame(DataOut, (byte)(m_pcNumDevice ^ 0x80), m_stationNumDevice, (byte)EnumCommandFrame_02_SOLD.M_HS, (byte)(0xFD));
                                SendBytes(m_FrameDataOut02.GetStuffedFrame());

                                // enviar solicitud de firmware
                                DataOut = new byte[0];
                                byte FIDSent_1 = (byte)237;
                                // 26/06/2014 El FIRMWARE se pide siempre al Device 0 (en protocolo 02), porque en las touch el HANDSHAKE lo emite el device COM y devuelve COM
                                //FrameDataOut02.EncodeFrame(DataOut, pcNumDevice, stationNumDevice, Stack_apl_JBC.EnumCommandFrame.M_FIRMWARE, FIDSent)
                                // 05/11/2014 Se vuelve a enviar al device de conexión y se analiza en WaitFW si se debe enviar al 00
                                //FrameDataOut02.EncodeFrame(DataOut, pcNumDevice, 0, Stack_apl_JBC_BURST.EnumCommandFrame.M_FIRMWARE, FIDSent)
                                m_FrameDataOut02.EncodeFrame(DataOut, m_pcNumDevice, m_stationNumDevice, (byte)EnumCommandFrame_02_SOLD.M_FIRMWARE, FIDSent_1);
                                SendBytes(m_FrameDataOut02.GetStuffedFrame());

                                m_Timer_Search.Interval = MS_WAIT_FIRMWARE;
                                m_StatusConnect = StatusConnect.WaitFW;
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        CloseConnection();
                        m_StatusConnect = StatusConnect.Search;
                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                    }
                    break;

                case StatusConnect.WaitACKofHS:
                    // esperanndo ACK (o NAK) de protocolo 02 de un handshake emitido por el PC
                    DataIn = ReadBytes();

                    //
                    // Respuesta handshake de PC protocolo 02
                    //

                    // verificar si viene una trama de respuesta de Handshake y cargar en FrameDatain
                    if (DataIn.Length > 0 && DataIn[0] == ASCII_DLE)
                    {
                        m_Conection_FrameProtocol = CStationBase.Protocol.Protocol_02;

                        // seguir leyendo en buffer de entrada
                        iErrors = m_FrameDataIn02.DecodeCheckReceivedData(DataIn, -1, ref m_MessageFIFOIn);

                        // ha recibido un frame
                        if (m_MessageFIFOIn.Number > 0)
                        {
                            // verificar trama respuesta de Handshake
                            // si conecta = false -> frames leídos sin handshaking
                            Conecta = false;
                            foreach (CDllFifoMessages.MessageDll message in m_MessageFIFOIn.GetTable)
                            {
                                // FID a recibir FDh
                                // Control Handshake 00h
                                // El número de dispositivo asignado vendrá tanto en TargetDevice como en los datos
                                // Filtramos las direcciones origen
                                if (message.Command == (byte)EnumCommandFrame_02_SOLD.M_HS & message.FID == 0xFD)
                                {
                                    if (message.Datos[0] == (byte)EnumCommandFrame_02_SOLD.M_ACK)
                                    {
                                        m_pcNumDevice = message.TargetDevice;
                                        m_stationNumDevice = message.SourceDevice;
                                        Conecta = true;
                                        break;
                                    }
                                    else
                                    {
                                        Conecta = false;
                                    }
                                }
                            }
                        }
                        else if (m_FrameDataIn02.GetFrame().Length > 0)
                        {
                            // ha recibido parte de un frame, seguir leyendo
                            m_StatusConnect = StatusConnect.WaitACKofHS;
                            break;
                        }
                        else
                        {
                            Conecta = false;
                        }
                    }

                    //Conexion no válida
                    if (Conecta == false)
                    {
                        CloseConnection();
                        m_StatusConnect = StatusConnect.Search;

                        m_Timer_Search.Stop();
                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                        m_Timer_Search.Start();
                        break;
                    }

                    // conexión válida -> solicitar FIRMWARE
                    try
                    {
                        switch (m_Conection_FrameProtocol)
                        {
                            case CStationBase.Protocol.Protocol_02:
                                // enviar solicitud de firmware
                                m_MessageFIFOIn.Reset(); // inicializa los mensajes de entrada
                                m_FrameDataIn02.Reset(); // inicializa buffer de entrada

                                // enviar solicitud de firmware
                                DataOut = new byte[0];
                                byte FIDSent_2 = (byte)237;
                                m_FrameDataOut02.EncodeFrame(DataOut, m_pcNumDevice, m_stationNumDevice, (byte)EnumCommandFrame_02_SOLD.M_FIRMWARE, FIDSent_2);
                                SendBytes(m_FrameDataOut02.GetStuffedFrame());

                                m_Timer_Search.Interval = MS_WAIT_FIRMWARE;
                                m_StatusConnect = StatusConnect.WaitFW;
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        CloseConnection();
                        m_StatusConnect = StatusConnect.Search;
                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                    }
                    break;

                case StatusConnect.WaitFW:
                    // se espera la respuesta a la solicitud de FIRMWARE (para los 2 protocolos de conexión 01 y 02)
                    DataIn = ReadBytes();

                    // esperando trama de firmware del protocolo de conexión 02
                    if (m_Conection_FrameProtocol == CStationBase.Protocol.Protocol_02)
                    {
                        // quizás checkear devices
                        iErrors = m_FrameDataIn02.DecodeCheckReceivedData(DataIn, -1, ref m_MessageFIFOIn);
                        if (m_MessageFIFOIn.Number > 0)
                        {
                            // ha recibido un frame
                            // analizar trama y obtener dato station model y protocolo
                            // si conecta = false -> frames leídos sin FIRMWARE
                            Conecta = false;
                            foreach (CDllFifoMessages.MessageDll message in m_MessageFIFOIn.GetTable)
                            {
                                // Control Firmware
                                // analizar trama y obtener dato station model
                                if (message.Command == (byte)EnumCommandFrame_02_SOLD.M_FIRMWARE)
                                {
                                    string sFirmw = System.Text.Encoding.UTF8.GetString(message.Datos);
                                    string[] aFirmw = sFirmw.Split(':');

                                    if (aFirmw.Length < 2)
                                    {
                                        m_StatusConnect = StatusConnect.Search;
                                        m_Timer_Search.Stop();
                                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                                        m_Timer_Search.Start();
                                        break;
                                    }

                                    sCommandProtocol = aFirmw[0].Trim();
                                    sStationModel = aFirmw[1].Trim(); // format: model_modeltype_modelversion
                                    SoftwareVersion = aFirmw[2].Trim();
                                    HardwareVersion = aFirmw[3].Trim();

                                    CStationBase.Protocol commandProtocol = default(CStationBase.Protocol);
                                    switch (sCommandProtocol)
                                    {
                                        case "01":
                                            commandProtocol = CStationBase.Protocol.Protocol_01;
                                            break;
                                        case "02":
                                            commandProtocol = CStationBase.Protocol.Protocol_02;
                                            break;
                                        default:
                                            commandProtocol = CStationBase.Protocol.Protocol_01;
                                            break;
                                    }

                                    // ver si el FIRMWARE me ha devuelto como modelo COM_DTE_01, por lo que es una DME
                                    // no actualizada y debo pedir el FIRMWARE (y continuar) con el devide H00
                                    if (commandProtocol == CStationBase.Protocol.Protocol_02 & sStationModel.Substring(0, 3) == "COM")
                                    {
                                        // OJO, borrar lista de mensajes de entrada para recibir de nuevo el M_FIRMWARE
                                        // si no lo hacemos, se queda el mensaje anterior en la lista y se lee el primero que encuentra (el mismo)
                                        // 04/04/2016 Limpiar buffer de entrada para la prÃ³xima consulta
                                        m_MessageFIFOIn.Reset(); // inicializa los mensajes de entrada
                                        m_FrameDataIn02.Reset(); // inicializa buffer de entrada

                                        // volver a enviar solicitud de firmware pero al device 0
                                        DataOut = new byte[0];
                                        byte FIDSent = (byte)(message.FID + 1);
                                        m_stationNumDevice = (byte)0;
                                        m_FrameDataOut02.EncodeFrame(DataOut, m_pcNumDevice, m_stationNumDevice, (byte)EnumCommandFrame_02_SOLD.M_FIRMWARE, FIDSent);
                                        SendBytes(m_FrameDataOut02.GetStuffedFrame());

                                        m_Timer_Search.Interval = MS_WAIT_FIRMWARE;
                                        m_StatusConnect = StatusConnect.WaitFW;
                                    }
                                    else
                                    {
                                        //Se prepara para una nueva conexión
                                        m_StatusConnect = StatusConnect.Search;

                                        if (m_Timer_Search != null)
                                        {
                                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                                        }

                                        // nueva conexión
                                        RaiseNewConnection(commandProtocol, sStationModel, SoftwareVersion, HardwareVersion);
                                    }

                                    break;
                                }
                            }
                        }
                        else if (m_FrameDataIn02.GetFrame().Length > 0)
                        {
                            // ha recibido parte de un frame, seguir leyendo
                            m_StatusConnect = StatusConnect.WaitFW;
                            break;
                        }

                        // esperando trama de firmware del protocolo de conexión 01
                    }
                    else if (m_Conection_FrameProtocol == CStationBase.Protocol.Protocol_01)
                    {
                        // quizás checkear devices
                        iErrors = m_FrameDataIn01.DecodeCheckReceivedData(DataIn, -1, ref m_MessageFIFOIn);
                        if (m_MessageFIFOIn.Number > 0)
                        {
                            // ha recibido un frame
                            // analizar trama y obtener dato station model y protocolo
                            // si conecta = false -> frames leídos sin FIRMWARE
                            Conecta = false;
                            foreach (CDllFifoMessages.MessageDll message in m_MessageFIFOIn.GetTable)
                            {
                                // Control Firmware
                                // analizar trama y obtener dato station model y protocolo
                                if (message.Command == (byte)EnumCommandFrame_01_SOLD.M_FIRMWARE)
                                {
                                    string sFirmw = System.Text.Encoding.UTF8.GetString(message.Datos);
                                    string[] aFirmw = sFirmw.Split(':');

                                    if (aFirmw.Length < 3)
                                    {
                                        m_StatusConnect = StatusConnect.Search;
                                        m_Timer_Search.Stop();
                                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                                        m_Timer_Search.Start();

                                        break;
                                    }

                                    sCommandProtocol = aFirmw[0].Trim();
                                    sStationModel = aFirmw[1].Trim(); // format: model_modeltype?_modelversion
                                                                      // 01/06/2016 Edu Se detecta que en protocolo 01, al grabar un UID de 20 caracteres, añade los últimos 7 también al Software del string de FIRMWARE,
                                                                      // por lo tanto se toman sólo los 7 primeros caracteres del software y del hardware
                                    SoftwareVersion = aFirmw[2].Trim().Substring(0, 7);
                                    HardwareVersion = aFirmw[3].Trim().Substring(0, 7);

                                    CStationBase.Protocol commandProtocol = default(CStationBase.Protocol);
                                    switch (sCommandProtocol)
                                    {
                                        case "01":
                                            commandProtocol = CStationBase.Protocol.Protocol_01;
                                            break;
                                        case "02":
                                            commandProtocol = CStationBase.Protocol.Protocol_02;
                                            break;
                                        default:
                                            commandProtocol = CStationBase.Protocol.Protocol_01;
                                            break;
                                    }

                                    //Se prepara para una nueva conexión
                                    m_StatusConnect = StatusConnect.Search;

                                    if (m_Timer_Search != null)
                                    {
                                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                                    }

                                    RaiseNewConnection(commandProtocol, sStationModel, SoftwareVersion, HardwareVersion);

                                    break;
                                }
                            }

                        }
                        else if (m_FrameDataIn01.GetFrame().Length > 0)
                        {
                            // ha recibido parte de un frame, seguir leyendo
                            m_StatusConnect = StatusConnect.WaitFW;
                            break;
                        }
                    }
                    break;

                case StatusConnect.WaitACK:
                    // Protocol 01: waiting ACK
                    // se espera la respuesta a la solicitud de FIRMWARE (para los 2 protocolos de conexión 01 y 02)

                    DataIn = ReadBytes();
                    ReceivedWaitACK = EnumReceived.Null;

                    foreach (byte byt in DataIn)
                    {
                        if (byt == ASCII_ACK)
                        {
                            ReceivedWaitACK = EnumReceived.ACK;
                        }
                        else if (byt == ASCII_NAK)
                        {
                            ReceivedWaitACK = EnumReceived.NAK;
                        }
                        else //Todos han de ser ACK o NAK
                        {
                            ReceivedWaitACK = EnumReceived.Other;
                            break;
                        }
                    }

                    switch (ReceivedWaitACK)
                    {
                        case EnumReceived.Null: // si no ha llegado nada
                            break;
                        // seguir esperando
                        case EnumReceived.ACK: // si todo lo que ha llegado son NAK menos el último que es un ACK
                            try
                            {
                                SendBytes(new byte[] { ASCII_ACK });
                            }
                            catch (Exception)
                            {
                                CloseConnection();
                                m_StatusConnect = StatusConnect.Search;
                                m_Timer_Search.Interval = MS_NEW_SEARCH;
                                break;
                            }

                            m_StatusConnect = StatusConnect.WaitNum;
                            m_Timer_Search.Interval = MS_WAIT_ACK;
                            break;
                        case EnumReceived.NAK: // si todos son NAK salir y esperar más caracteres
                            break;
                        // seguir esperando
                        case EnumReceived.Other: // si ha llegado algo inesperado
                            CloseConnection();
                            m_StatusConnect = StatusConnect.Search;
                            m_Timer_Search.Interval = MS_NEW_SEARCH;
                            break;
                        default:
                            break;
                    }

                    break;

                case StatusConnect.WaitNum:
                    // ProtocolO de conexión 01, se espera el número del PC (device)
                    // se espera sólo el número del PC

                    m_pcNumDevice = System.Convert.ToByte(ReadBytes(1)[0]);
                    m_stationNumDevice = (byte)0;
                    try
                    {
                        SendBytes(new byte[] { ASCII_ACK });
                    }
                    catch (Exception)
                    {
                        CloseConnection();
                        m_StatusConnect = StatusConnect.Search;
                        m_Timer_Search.Interval = MS_NEW_SEARCH;
                        break;
                    }

                    // enviar solicitud de firmware
                    DataOut = new byte[0];
                    m_FrameDataOut01.EncodeFrame(DataOut, m_pcNumDevice, m_stationNumDevice, (byte)EnumCommandFrame_01_SOLD.M_FIRMWARE);
                    SendBytes(m_FrameDataOut01.GetStuffedFrame());
                    m_Timer_Search.Interval = MS_WAIT_FIRMWARE;
                    m_StatusConnect = StatusConnect.WaitFW;
                    m_FrameDataIn01.Reset(); // inicializa buffer de entrada
                    break;
            }
        }

        protected bool CheckStationModel(string sStationModel)
        {
            foreach (char c in sStationModel)
            {
                if (c < '/' || c > 'z')
                {
                    return false;
                }
            }
            return true;
        }

    }
}
