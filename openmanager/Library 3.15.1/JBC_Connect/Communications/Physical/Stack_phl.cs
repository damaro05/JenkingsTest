// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
// End of VB project level imports

using System.IO.Ports;
using System.Text;
using System.Net;
using System.Net.Sockets;

// Version ráfagas

namespace JBC_Connect
{
    internal class Stack_phl
    {

        #region ENUMERACIONES PUBLICAS

        private enum EnumConnect : int
        {
            NONE,
            USB,
            TCP
        }

        internal enum EnumError : int
        {
            FATAL,
            SENDING_DATA,
            RECEIVING_DATA,
            CLOSED_TCP_CLIENT
        }

        #endregion

        #region VARIABLES
        private EnumConnect m_TypeConnect = EnumConnect.NONE;
        private RoutinesLibrary.IO.SerialPort m_SerialPort_Int;
        private RoutinesLibrary.Net.Protocols.TCP.TCP m_WinSockClient_Int;
        private int m_FrameWindowSizeInBytes_USB = 1;
        #endregion

        #region EVENTOS PUBLICOS
        internal delegate void DataReceivedEventHandler(ref byte[] Data);
        private DataReceivedEventHandler DataReceivedEvent;

        internal event DataReceivedEventHandler DataReceived
        {
            add
            {
                DataReceivedEvent = (DataReceivedEventHandler)System.Delegate.Combine(DataReceivedEvent, value);
            }
            remove
            {
                DataReceivedEvent = (DataReceivedEventHandler)System.Delegate.Remove(DataReceivedEvent, value);
            }
        }

        internal delegate void ErrorConnectEventHandler(EnumError Tipo);
        private ErrorConnectEventHandler ErrorConnectEvent;

        internal event ErrorConnectEventHandler ErrorConnect
        {
            add
            {
                ErrorConnectEvent = (ErrorConnectEventHandler)System.Delegate.Combine(ErrorConnectEvent, value);
            }
            remove
            {
                ErrorConnectEvent = (ErrorConnectEventHandler)System.Delegate.Remove(ErrorConnectEvent, value);
            }
        }

        #endregion

        #region METODOS PUBLICOS
        internal Stack_phl(RoutinesLibrary.IO.SerialPort Port, int FrameWindowSizeInBytes)
        {
            m_SerialPort_Int = new RoutinesLibrary.IO.SerialPort(Port.GetSerialPort);
            m_SerialPort_Int.DataReceived += new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
            m_FrameWindowSizeInBytes_USB = FrameWindowSizeInBytes;
            m_TypeConnect = EnumConnect.USB;
        }

        internal Stack_phl(RoutinesLibrary.Net.Protocols.TCP.TCP WinSock, int FrameWindowSizeInBytes)
        {
            m_WinSockClient_Int = WinSock;
            m_WinSockClient_Int.DataReceived += new RoutinesLibrary.Net.Protocols.TCP.TCP.DataReceivedEventHandler(WinSockClient_Int_DataReceived);
            m_WinSockClient_Int.ClosedConnectionTCP += new RoutinesLibrary.Net.Protocols.TCP.TCP.ClosedConnectionTCPEventHandler(WinSockClient_Int_ClosedConnection);
            m_WinSockClient_Int.BufferSize = FrameWindowSizeInBytes;
            m_TypeConnect = EnumConnect.TCP;
        }

        internal void Int_SendData(byte[] Data)
        {
            try
            {
                if (m_TypeConnect == EnumConnect.USB)
                {
                    m_SerialPort_Int.Write(Data);
                }
                else if (m_TypeConnect == EnumConnect.TCP)
                {
                    m_WinSockClient_Int.SendData(Data);
                }
            }
            catch (Exception)
            {
                Console.WriteLine(" Int_SendData: ");
                // Si no se puede transmitir finalizar la conexión
                if (ErrorConnectEvent != null)
                    ErrorConnectEvent(EnumError.SENDING_DATA);
            }
        }

        internal void Eraser()
        {
            //Debug.Print("StackPhl.Eraser")
            if (m_TypeConnect == EnumConnect.USB)
            {
                m_SerialPort_Int.Dispose();
                m_SerialPort_Int.DataReceived -= new RoutinesLibrary.IO.SerialPort.DataReceivedEventHandler(SerialPort_Int_DataReceived);
                m_SerialPort_Int = null;
            }
            else if (m_TypeConnect == EnumConnect.TCP)
            {
                m_WinSockClient_Int.Dispose();
                m_WinSockClient_Int.DataReceived -= new RoutinesLibrary.Net.Protocols.TCP.TCP.DataReceivedEventHandler(WinSockClient_Int_DataReceived);
                m_WinSockClient_Int.ClosedConnectionTCP -= new RoutinesLibrary.Net.Protocols.TCP.TCP.ClosedConnectionTCPEventHandler(WinSockClient_Int_ClosedConnection);
                m_WinSockClient_Int = null;
            }
        }

        #endregion

        #region METODOS PRIVADOS USB

        private void SerialPort_Int_DataReceived()
        {
            byte[] DataIn = null;
            int Leidos = 0;
            // Tres formas de hacerlo:

            // La primera, el problema es que tenemos que crear un array de 512B
            try
            {
                byte[] DatosInBytes = new byte[m_FrameWindowSizeInBytes_USB + 1];

                //Leidos = SerialPort_Int.Read(DatosInBytes, 0, FrameWindowSizeInBytes_USB) ' NO UTILIZAR
                Leidos = 0;
                while (m_SerialPort_Int.BytesToRead() != 0)
                {
                    DatosInBytes[Leidos] = System.Convert.ToByte(m_SerialPort_Int.ReadByte());
                    Leidos++;
                    if (Leidos > m_FrameWindowSizeInBytes_USB)
                    {
                        break;
                    }
                }
                if (Leidos > 0)
                {
                    DataIn = new byte[Leidos - 1 + 1];
                    Array.Copy(DatosInBytes, DataIn, Leidos);
                    if (DataReceivedEvent != null)
                        DataReceivedEvent(ref DataIn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(" SerialPort_Int_DataReceived: " + ex.Message);
                // Si no se puede recibir finalizar la conexión
                if (ErrorConnectEvent != null)
                    ErrorConnectEvent(EnumError.RECEIVING_DATA);
            }


            // La segunda, falla porque se insertan caracteres intermedios
            //'Dim TextoIn As String
            //'TextoIn = SerialPort_Int.ReadExisting
            //'If TextoIn.Length > 5 Then
            //'    DataOut = Encoding.UTF8.GetBytes(TextoIn)
            //'End If
            //'DataOut = Encoding.UTF8.GetBytes(TextoIn)
            //'RaiseEvent DataReceived(DataOut)

            // La tercera
            //'Dim TextoIn As String
            //'TextoIn = SerialPort_Int.ReadExisting
            //'ReDim DataOut(TextoIn.Length)
            //'Index = 0
            //'For Each c As Char In TextoIn
            //'    DataOut(Index) = CByte(Asc(c))
            //'    Index += 1
            //'Next
            //'RaiseEvent DataReceived(DataOut)
        }

        #endregion

        #region METODOS PRIVADOS TCP
        private void WinSockClient_Int_DataReceived(byte[] data)
        {
            byte[] DataIn = null;
            try
            {
                DataIn = new byte[data.Length - 1 + 1];
                Array.Copy(data, DataIn, data.Length);
                if (DataReceivedEvent != null)
                    DataReceivedEvent(ref DataIn);
            }
            catch (Exception)
            {
                Console.WriteLine(" WinSockClient_Int_DataReceived: ");
                // Si no se puede recibir finalizar la conexión
                if (ErrorConnectEvent != null)
                    ErrorConnectEvent(EnumError.RECEIVING_DATA);
            }
        }

        private void WinSockClient_Int_ClosedConnection()
        {
            Console.WriteLine(" WinSockClient_Int_ClosedConnection: ");
            if (ErrorConnectEvent != null)
                ErrorConnectEvent(EnumError.CLOSED_TCP_CLIENT);
            //Debug.Print("ClosedConnectionTCP en Stack_phl")
        }

        #endregion

    }
}
