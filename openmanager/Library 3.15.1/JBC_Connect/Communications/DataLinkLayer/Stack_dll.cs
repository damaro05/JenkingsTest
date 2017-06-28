// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO.Ports;
using System.Threading;

namespace JBC_Connect
{
    internal class Stack_dll
    {

        private const int MAX_FRAME_LENGTH = (8 + 255);


        private Stack_phl m_phl;
        private byte m_pcNumDevice;
        private bool m_bErrorConnect = false;
        private CFrameData m_FrameDataOut01; // buffer de salida para protocolo de frame 01
        private CFrameData m_FrameDataIn01; // buffer de entrada para protocolo de frame 01
        private CFrameData m_FrameDataOut02; // buffer de salida para protocolo de frame 02
        private CFrameData m_FrameDataIn02; // buffer de entrada para protocolo de frame 02
        private CDllFifoMessages m_MessageFIFOIn = new CDllFifoMessages(); // lista de mensajes de entrada
        private CDllFifoMessages m_FrameFIFOOut = new CDllFifoMessages(); // lista de frames de salida
        private CStationBase.Protocol m_FrameProtocol;
        private EnumFrameFlowControl m_Flowcontrol;
        private int m_FrameWindow;


        internal delegate void ErrorConnectEventHandler(Stack_phl.EnumError Tipo); // Error fatal se debe finalizar la conexión: Error puerto, No hay respuesta
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

        internal delegate void DataReceivedEventHandler(byte Origen, byte Command, byte[] Data, byte FID, bool Response);
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

        internal delegate void DataReceivedMessagesEventHandler(List<CDllFifoMessages.MessageDll> Messages);
        private DataReceivedMessagesEventHandler DataReceivedMessagesEvent;

        internal event DataReceivedMessagesEventHandler DataReceivedMessages
        {
            add
            {
                DataReceivedMessagesEvent = (DataReceivedMessagesEventHandler)System.Delegate.Combine(DataReceivedMessagesEvent, value);
            }
            remove
            {
                DataReceivedMessagesEvent = (DataReceivedMessagesEventHandler)System.Delegate.Remove(DataReceivedMessagesEvent, value);
            }
        }

#if LibraryTest
	internal delegate void DataReceivedFrameMessagesEventHandler(List<CDllFifoMessages.MessageDll> Messages);
	private DataReceivedFrameMessagesEventHandler DataReceivedFrameMessagesEvent;
		
	internal event DataReceivedFrameMessagesEventHandler DataReceivedFrameMessages
	{
		add
		{
			DataReceivedFrameMessagesEvent = (DataReceivedFrameMessagesEventHandler) System.Delegate.Combine(DataReceivedFrameMessagesEvent, value);
		}
		remove
		{
			DataReceivedFrameMessagesEvent = (DataReceivedFrameMessagesEventHandler) System.Delegate.Remove(DataReceivedFrameMessagesEvent, value);
		}
	}
		
	internal delegate void DataReceivedFrameEventHandler(byte[] Frame);
	private DataReceivedFrameEventHandler DataReceivedFrameEvent;
		
	internal event DataReceivedFrameEventHandler DataReceivedFrame
	{
		add
		{
			DataReceivedFrameEvent = (DataReceivedFrameEventHandler) System.Delegate.Combine(DataReceivedFrameEvent, value);
		}
		remove
		{
			DataReceivedFrameEvent = (DataReceivedFrameEventHandler) System.Delegate.Remove(DataReceivedFrameEvent, value);
		}
	}
		
	internal delegate void DataSentFrameEventHandler(byte[] Frame);
	private DataSentFrameEventHandler DataSentFrameEvent;
		
	internal event DataSentFrameEventHandler DataSentFrame
	{
		add
		{
			DataSentFrameEvent = (DataSentFrameEventHandler) System.Delegate.Combine(DataSentFrameEvent, value);
		}
		remove
		{
			DataSentFrameEvent = (DataSentFrameEventHandler) System.Delegate.Remove(DataSentFrameEvent, value);
		}
	}
		
		
	internal delegate void DataReceivedRawDataEventHandler(byte[] RawData);
	private DataReceivedRawDataEventHandler DataReceivedRawDataEvent;
		
	internal event DataReceivedRawDataEventHandler DataReceivedRawData
	{
		add
		{
			DataReceivedRawDataEvent = (DataReceivedRawDataEventHandler) System.Delegate.Combine(DataReceivedRawDataEvent, value);
		}
		remove
		{
			DataReceivedRawDataEvent = (DataReceivedRawDataEventHandler) System.Delegate.Remove(DataReceivedRawDataEvent, value);
		}
	}
		
	internal delegate void DataSentRawDataEventHandler(byte[] RawData);
	private DataSentRawDataEventHandler DataSentRawDataEvent;
		
	internal event DataSentRawDataEventHandler DataSentRawData
	{
		add
		{
			DataSentRawDataEvent = (DataSentRawDataEventHandler) System.Delegate.Combine(DataSentRawDataEvent, value);
		}
		remove
		{
			DataSentRawDataEvent = (DataSentRawDataEventHandler) System.Delegate.Remove(DataSentRawDataEvent, value);
		}
	}
		
		
#endif


        internal Stack_dll(ref RoutinesLibrary.IO.SerialPort Port, byte _pcNumDevice, CStationBase.Protocol _FrameProtocol, EnumFrameFlowControl _Flowcontrol, int frameWindow)
        {
            m_phl = new Stack_phl(Port, (MAX_FRAME_LENGTH * frameWindow) + MAX_FRAME_LENGTH);
            m_phl.DataReceived += phl_DataReceived;
            m_phl.ErrorConnect += phl_ErrorConnect;

            Initialize(_pcNumDevice, _FrameProtocol, _Flowcontrol, frameWindow);
        }

        internal Stack_dll(ref RoutinesLibrary.Net.Protocols.TCP.TCP WinSock, byte _pcNumDevice, CStationBase.Protocol _FrameProtocol, EnumFrameFlowControl _Flowcontrol, int frameWindow)
        {
            m_phl = new Stack_phl(WinSock, (MAX_FRAME_LENGTH * frameWindow) + MAX_FRAME_LENGTH);
            m_phl.DataReceived += phl_DataReceived;
            m_phl.ErrorConnect += phl_ErrorConnect;

            Initialize(_pcNumDevice, _FrameProtocol, _Flowcontrol, frameWindow);
        }

        private void Initialize(byte _pcNumDevice, CStationBase.Protocol _FrameProtocol, EnumFrameFlowControl _Flowcontrol, int frameWindow)
        {
            m_FrameProtocol = _FrameProtocol;
            m_Flowcontrol = _Flowcontrol;
            m_pcNumDevice = _pcNumDevice;

            m_FrameDataOut01 = new CFrameData(_FrameProtocol);
            m_FrameDataIn01 = new CFrameData(_FrameProtocol);
            m_FrameDataOut02 = new CFrameData(_FrameProtocol);
            m_FrameDataIn02 = new CFrameData(_FrameProtocol);
        }

        internal bool SendData(byte[] Data, byte targetDevice, byte FrameControl, byte FID, bool response = false)
        {
            // envío directo, utilizado con FlowControl.ONE_NO_FID
            if (!m_bErrorConnect)
            {

                byte origen = m_pcNumDevice;
                if (response)
                {
                    origen = (byte)(origen ^ (byte)EnumAddress.MASK_RESPONSE_ADDRESS);
                }

                //'Debug.Print(String.Format("DataSend dll: {0:X}", FrameControl))

                switch (m_FrameProtocol)
                {
                    case CStationBase.Protocol.Protocol_01:
                        if (!m_FrameDataOut01.EncodeFrame(Data, origen, targetDevice, FrameControl))
                        {
                            return false;
                        }
                        m_phl.Int_SendData(m_FrameDataOut01.GetStuffedFrame()); // Transferir datos
#if LibraryTest
					DataSentFrameEvent(m_FrameDataOut01.GetFrame());
					DataSentRawDataEvent(m_FrameDataOut01.GetStuffedFrame());
#endif
                        break;
                    case CStationBase.Protocol.Protocol_02:
                        if (!m_FrameDataOut02.EncodeFrame(Data, origen, targetDevice, FrameControl, FID))
                        {
                            return false;
                        }
                        m_phl.Int_SendData(m_FrameDataOut02.GetStuffedFrame()); // Transferir datos
#if LibraryTest
					DataSentFrameEvent(m_FrameDataOut02.GetFrame());
					DataSentRawDataEvent(m_FrameDataOut02.GetStuffedFrame());
#endif
                        break;
                }

                return true;
            }

            return false;
        }

        internal bool AddDataToSend(byte[] Data, byte targetDevice, byte FrameControl, byte FID)
        {
            // envío por ráfagas (junto con SendAddedData), utilizado con FlowControl.BURST_FID
            if (!m_bErrorConnect)
            {
                //'Debug.Print(String.Format("AddDataToSend dll: {0:X}", FrameControl))
                switch (m_FrameProtocol)
                {
                    case CStationBase.Protocol.Protocol_01:
                        if (!m_FrameDataOut01.AddFrame(Data, m_pcNumDevice, targetDevice, FrameControl, m_FrameFIFOOut))
                        {
                            return false;
                        }
                        break;
                    case CStationBase.Protocol.Protocol_02:
                        if (!m_FrameDataOut02.AddFrame(Data, m_pcNumDevice, targetDevice, FrameControl, m_FrameFIFOOut, FID))
                        {
                            return false;
                        }
                        break;
                }

                return true;
            }

            return false;
        }

        internal int SendAddedData()
        {
            // envío por ráfagas (junto con AddDataToSend), utilizado con FlowControl.BURST_FID
            int iFramesSent = 0;
            try
            {
                if (!m_bErrorConnect)
                {
                    //'Debug.Print(String.Format("DataSend - Frames to send: ", FrameFIFOOut.Number))

                    foreach (CDllFifoMessages.MessageDll Frame in m_FrameFIFOOut.GetTable)
                    {
                        m_phl.Int_SendData(Frame.RawStuffedFrame); // Transferir datos
                        iFramesSent++;
#if LibraryTest
					DataSentFrameEvent(Frame.RawFrame);
#endif
                    }

                    // vaciar lista de frames de salida
                    m_FrameFIFOOut.Reset();
                }
            }
            catch (Exception)
            {
            }

            return iFramesSent;
        }

        internal void Eraser()
        {
            //Debug.Print("StackDll.Eraser")
            m_phl.Eraser();
            m_phl = null;
            //m_phl.DataReceived += phl_DataReceived;
            //m_phl.ErrorConnect += phl_ErrorConnect;
            m_FrameFIFOOut.Reset();
            m_FrameFIFOOut = null;
            m_MessageFIFOIn.Reset(); // #Edu#
            m_MessageFIFOIn = null; // #Edu#
        }


        private object LockReceived = new object();
        private void phl_DataReceived(ref byte[] Data)
        {
            lock (LockReceived)
            {

#if LibraryTest
			DataReceivedRawDataEvent(Data);
#endif

                m_MessageFIFOIn.Reset();

                int iErrors = 0;
                int iUndecodedBytes = 0;

                switch (m_FrameProtocol)
                {
                    case CStationBase.Protocol.Protocol_01:
                        iErrors = m_FrameDataIn01.DecodeCheckReceivedData(Data, m_pcNumDevice, ref m_MessageFIFOIn);
                        iUndecodedBytes = m_FrameDataIn01.CountFrameData();
                        break;
                    case CStationBase.Protocol.Protocol_02:
                        iErrors = m_FrameDataIn02.DecodeCheckReceivedData(Data, m_pcNumDevice, ref m_MessageFIFOIn);
                        iUndecodedBytes = m_FrameDataIn02.CountFrameData();
                        break;

                }

                if (m_Flowcontrol == EnumFrameFlowControl.BURST)
                {
                    // received data as messages
                    List<CDllFifoMessages.MessageDll> Messages = new List<CDllFifoMessages.MessageDll>();
                    foreach (CDllFifoMessages.MessageDll Mess in m_MessageFIFOIn.GetTable)
                    {
                        Messages.Add(Mess);
                    }
#if LibraryTest
				if (DataReceivedFrameMessagesEvent != null)
					DataReceivedFrameMessagesEvent(Messages);
#endif
                    if (DataReceivedMessagesEvent != null)
                        DataReceivedMessagesEvent(Messages);
                }
                else
                {
                    foreach (CDllFifoMessages.MessageDll Mess in m_MessageFIFOIn.GetTable)
                    {
#if LibraryTest
					if (DataReceivedFrameEvent != null) // Frame
						DataReceivedFrameEvent(Mess.RawFrame);
#endif
                        if (DataReceivedEvent != null) // Origen, Comando, Datos(), FID
                        {
                            DataReceivedEvent(Mess.SourceDevice, Mess.Command, Mess.Datos, Mess.FID, Mess.Response);
                        }

                        //Es posible que se haya destruido el objeto antes de que salga del loop (en consecuencia de una actualización)
                        if (ReferenceEquals(m_MessageFIFOIn, null))
                        {
                            break;
                        }
                    }

                }
            }
        }

        private void phl_ErrorConnect(Stack_phl.EnumError Tipo)
        {
            Console.WriteLine(" phl_ErrorConnect: " + Tipo);
            // Se ha recibido un error fatal del puerto serie, se debe finalizar la conexión
            if (!m_bErrorConnect)
            {
                //Debug.Print("phl_ErrorConnect: " & Tipo.ToString)
                if (ErrorConnectEvent != null)
                    ErrorConnectEvent(Tipo);
            }
            m_bErrorConnect = true;
        }

    }
}
