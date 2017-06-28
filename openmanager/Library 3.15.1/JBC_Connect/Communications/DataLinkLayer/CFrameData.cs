// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBC_Connect
{
    /// <summary>
    /// Clase para manejar frames de los protocolos
    /// </summary>
    /// <remarks></remarks>
    internal class CFrameData
    {

        internal enum EnumEstadoIn : int
        {
            NoConnect, // no conectado
            RxWaitingStartDLE, // recepción de escape en el frame start (DLE)
            RxWaitingStartSTX, // recepción de frame start (STX)
            RxReceivingFrame, // recibiendo Frame
            RxReceivingFrameScapeDLE // se ha recibido un escape (DLE) en el Frame
        }

        protected enum EnumFrameField : int
        {
            Control,
            Data
        }


        private const byte STX = 0x2;
        private const byte ETX = 0x3;
        private const byte EOT = 0x4;
        private const byte ACK = 0x6;
        private const byte DLE = 0x10;
        private const byte NAK = 0x15;
        private const byte SYN = 0x16;
        private const byte ESC = 0x1B;

        private byte POS_STX;
        private byte POS_ORIGEN;
        private byte POS_DESTINO;
        private byte POS_FID;
        private byte POS_CONTROL;
        private byte POS_LENGTH;
        private byte POS_DATA;
        private byte POS_ETX_MAS_LENGTH;
        private byte POS_BCC_MAS_LENGTH;
        private int LENGTH_CONTROL;
        private int LENGTH_FRAME;
        private int LENGTH_FRAME_DLE;


        internal EnumEstadoIn StatusIn;

        private CStationBase.Protocol m_FrameProtocol;
        private DataCollection m_myFrameData;
        private DataCollection m_myFrameStuffedData;


        public CFrameData(CStationBase.Protocol FrameProtocol)
        {

            m_FrameProtocol = FrameProtocol;

            if (FrameProtocol == CStationBase.Protocol.Protocol_01)
            {
                POS_CONTROL = (byte)3;
                POS_LENGTH = (byte)4;
                POS_DATA = (byte)5;
                POS_ETX_MAS_LENGTH = (byte)6;
                POS_BCC_MAS_LENGTH = (byte)5;
                LENGTH_CONTROL = 7;

            }
            else if (FrameProtocol == CStationBase.Protocol.Protocol_02)
            {
                POS_FID = (byte)3;
                POS_CONTROL = (byte)4;
                POS_LENGTH = (byte)5;
                POS_DATA = (byte)6;
                POS_ETX_MAS_LENGTH = (byte)7;
                POS_BCC_MAS_LENGTH = (byte)6;
                LENGTH_CONTROL = 8;
            }

            POS_STX = (byte)0;
            POS_ORIGEN = (byte)1;
            POS_DESTINO = (byte)2;
            LENGTH_FRAME = LENGTH_CONTROL + 255;
            LENGTH_FRAME_DLE = 2 * LENGTH_FRAME;


            m_myFrameData = new DataCollection();
            m_myFrameData.Reset();
            m_myFrameStuffedData = new DataCollection();
            m_myFrameStuffedData.Reset();
            SetStatusIn(EnumEstadoIn.RxWaitingStartDLE);
        }


        //Public Sub New(ByVal FrameBytes() As Byte)
        //    myFrameData = New DataCollection
        //    myFrameData.Reset()
        //    For Each value As Byte In FrameBytes
        //        myFrameData.Add(value)
        //    Next
        //    myFrameStuffedData = New DataCollection
        //    myFrameStuffedData.Reset()
        //    SetStatusIn(FrameData.EnumEstadoIn.RxStartDLE)
        //End Sub

        public void SetFrame(byte[] FrameBytes)
        {
            m_myFrameData.Reset();
            foreach (byte value in FrameBytes)
            {
                m_myFrameData.Add(value);
            }
        }

        public void Reset()
        {
            m_myFrameData.Reset();
            m_myFrameStuffedData.Reset();
        }

        internal int Add(byte value)
        {
            return m_myFrameData.Add(value);
        }

        public byte[] GetFrame()
        {
            return m_myFrameData.GetArray;
        }

        public int CountFrameData()
        {
            return m_myFrameData.CountElements;
        }

        public byte[] GetStuffedFrame()
        {
            StuffingData();
            return m_myFrameStuffedData.GetArray;
        }

        public int CountStuffedFrameData()
        {
            return m_myFrameStuffedData.CountElements;
        }

        internal void StuffingData()
        {
            // Montar la trama con stuffing
            byte[] DataInt = m_myFrameData.GetArray;
            m_myFrameStuffedData.Reset();
            for (int index = 0; index <= DataInt.Length - 1; index++)
            {
                if ((index == 0) || (index == (DataInt.Length - 1))) // El primer elemento y el último son de control
                {
                    StuffingByte(m_myFrameStuffedData, DataInt[index], EnumFrameField.Control);
                } // El resto son datos
                else
                {
                    StuffingByte(m_myFrameStuffedData, DataInt[index], EnumFrameField.Data);
                }
            }
        }

        private void StuffingByte(DataCollection FrameStuffedData, byte Data, EnumFrameField Control)
        {
            switch (Control)
            {
                case EnumFrameField.Control: // Los campos de control siempre llevan DLE
                    FrameStuffedData.Add(DLE);
                    FrameStuffedData.Add(Data);
                    break;
                case EnumFrameField.Data: // Los datos sólo llevan DLE si es otro DLE
                    if (Data == DLE)
                    {
                        FrameStuffedData.Add(DLE);
                    }
                    FrameStuffedData.Add(Data);
                    break;
            }
        }

        private byte CalculoBCCFrame()
        {
            return CalculoBCC(m_myFrameData.GetArray);
        }

        public byte CalculoBCC(byte[] Data)
        {
            byte BCC = (byte)0;
            foreach (byte value in Data)
            {
                BCC = (byte)(BCC ^ value);
            }
            return BCC;
        }


        public void SetStatusIn(EnumEstadoIn Status)
        {
            // estado de recepción, si es buffer de recepción
            StatusIn = Status;
        }

        public EnumEstadoIn GetStatusIn()
        {
            // estado de recepción, si es buffer de recepción
            return StatusIn;
        }

        public bool EncodeFrame(byte[] Data, byte sourceDevice, byte targetDevice, byte FrameControl, byte FID = 0)
        {
            try
            {
                int NumElemnt = Data.Length + POS_ETX_MAS_LENGTH;
                byte[] DataInt = new byte[NumElemnt + 1];

                DataInt[POS_STX] = STX;
                DataInt[POS_ORIGEN] = sourceDevice;
                DataInt[POS_DESTINO] = targetDevice;
                if (m_FrameProtocol == CStationBase.Protocol.Protocol_02)
                {
                    DataInt[POS_FID] = FID;
                }
                DataInt[POS_CONTROL] = FrameControl;

                DataInt[POS_LENGTH] = (byte)Data.Length;
                for (int index = 0; index <= Data.Length - 1; index++)
                {
                    DataInt[index + POS_DATA] = Data[index];
                }
                DataInt[Data.Length + POS_BCC_MAS_LENGTH] = (byte)0; // Primero ponemos cero y cuando tengamos el EXT se vuelve a calcular
                DataInt[Data.Length + POS_ETX_MAS_LENGTH] = ETX;
                DataInt[Data.Length + POS_BCC_MAS_LENGTH] = CalculoBCC(DataInt);

                SetFrame(DataInt);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CheckFrame(int targetDeviceToCheck)
        {
            if (m_myFrameData[POS_STX] != STX) // Comienza por STX
            {
                return false;
            }
            if (m_myFrameData.CountElements != (LENGTH_CONTROL + m_myFrameData[POS_LENGTH])) // La longitud es correcta
            {
                return false;
            }
            if (m_myFrameData[m_myFrameData.CountElements - 1] != ETX) // Finaliza con ETX
            {
                return false;
            }
            if (CalculoBCC(m_myFrameData.GetArray) != 0) // el BCC es correcto
            {
                return false;
            }
            return true; // Si todo ha ido bien la trama se considera correcta
        }

        public bool AddFrame(byte[] Data, byte sourceDevice, byte targetDevice, byte FrameControl, CDllFifoMessages FrameFIFO, byte FID = 0)
        {
            if (EncodeFrame(Data, sourceDevice, targetDevice, FrameControl, FID))
            {
                CDllFifoMessages.MessageDll NewFrame = new CDllFifoMessages.MessageDll();
                NewFrame.FID = FID;
                NewFrame.RawFrame = GetFrame();
                NewFrame.RawStuffedFrame = GetStuffedFrame();
                FrameFIFO.PutMessage(NewFrame);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int DecodeCheckReceivedData(byte[] Data, int targetDeviceToCheck, ref CDllFifoMessages MessageFIFO)
        {
            // rutina que analiza los datos recibidos y añade los mensajes a la lista MessageFIFO
            // el buffer se queda con lo que no ha podido generar mensaje y el estado de recepción (StatusIn)
            // devuelve la cantidad de errores producidos
            int iErrors = 0;

            foreach (byte value in Data)
            {
                switch (StatusIn)
                {
                    case EnumEstadoIn.NoConnect:
                        break;
                    case EnumEstadoIn.RxWaitingStartDLE:
                        if (value == DLE)
                        {
                            StatusIn = EnumEstadoIn.RxWaitingStartSTX; //pasa a esperar el STX
                        }
                        break;
                    case EnumEstadoIn.RxWaitingStartSTX:
                        if (value == STX)
                        {
                            StatusIn = EnumEstadoIn.RxReceivingFrame; //continúa recibiendo
                            m_myFrameData.Reset(); // Borra lo anterior guardado
                            m_myFrameData.Add(value); //guarda dato
                        }
                        else if (value == DLE)
                        {
                            // continúa esperando un STX
                        }
                        else
                        {
                            StatusIn = EnumEstadoIn.RxWaitingStartDLE; //Error empieza desde el principio
                        }
                        break;
                    case EnumEstadoIn.RxReceivingFrame:
                        if (value != DLE)
                        {
                            m_myFrameData.Add(value); //guarda y continúa recibiendo
                        }
                        else
                        {
                            StatusIn = EnumEstadoIn.RxReceivingFrameScapeDLE; //Cambia de estado por si es una orden de bajo nivel
                        }
                        break;
                    case EnumEstadoIn.RxReceivingFrameScapeDLE:
                        if (value == DLE)
                        {
                            StatusIn = EnumEstadoIn.RxReceivingFrame; //continúa recibiendo
                            m_myFrameData.Add(value); //guarda y continúa recibiendo
                        }
                        else if (value == ETX)
                        {
                            StatusIn = EnumEstadoIn.RxWaitingStartDLE;
                            m_myFrameData.Add(value); //guarda

                            if (CheckFrame(targetDeviceToCheck) == true) //Trama OK
                            {
                                AddFrameToMessageArray(MessageFIFO);
                            }
                            else
                            {
                                StatusIn = EnumEstadoIn.RxWaitingStartDLE; //Error empieza desde el principio
                                iErrors++;
                            }
                            m_myFrameData.Reset();
                        }
                        else if (value == STX) // Inicio nueva trama
                        {
                            StatusIn = EnumEstadoIn.RxReceivingFrame; //continúa recibiendo
                            m_myFrameData.Reset(); // Borra lo anterior guardado
                            m_myFrameData.Add(value); //guarda dato
                        }
                        else
                        {
                            StatusIn = EnumEstadoIn.RxWaitingStartDLE; //Error empieza desde el principio
                            iErrors++;
                        }
                        break;
                    default:
                        StatusIn = EnumEstadoIn.RxWaitingStartDLE; //Error empieza desde el principio
                        iErrors++;
                        break;
                }
            }

            return iErrors;
        }

        private bool AddFrameToMessageArray(CDllFifoMessages MessageFIFO)
        {
            try
            {
                CDllFifoMessages.MessageDll NewMessage = new CDllFifoMessages.MessageDll();
                byte[] Frame1 = null;
                byte[] Frame2 = new byte[m_myFrameData[POS_LENGTH] - 1 + 1];
                Frame1 = m_myFrameData.GetArray;
                Array.Copy(Frame1, POS_DATA, Frame2, 0, m_myFrameData[POS_LENGTH]);
                NewMessage.SourceDevice = System.Convert.ToByte(Frame1[POS_ORIGEN] & 0x7F);
                NewMessage.TargetDevice = System.Convert.ToByte(Frame1[POS_DESTINO] & 0x7F);
                if (m_FrameProtocol == CStationBase.Protocol.Protocol_01)
                {
                    NewMessage.FID = (byte)0;
                }
                else if (m_FrameProtocol == CStationBase.Protocol.Protocol_02)
                {
                    NewMessage.FID = Frame1[POS_FID];
                }
                NewMessage.Command = Frame1[POS_CONTROL];
                NewMessage.Datos = Frame2;
                NewMessage.Response = (Frame1[POS_ORIGEN] & 0x80) > 0;
                NewMessage.RawFrame = new byte[m_myFrameData.GetArray.Length - 1 + 1];
                Array.Copy(m_myFrameData.GetArray, NewMessage.RawFrame, System.Convert.ToInt32(m_myFrameData.GetArray.Length));
                MessageFIFO.PutMessage(NewMessage);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
