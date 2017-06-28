// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBC_Connect
{
    /// <summary>
    /// Clase para crear una FIFO de frames
    /// </summary>
    /// <remarks></remarks>
    internal class CDllFifoMessages
    {

        public struct MessageDll
        {
            internal byte FID; // número único de trama (protocolo de trama => 02)
            internal byte SourceDevice;
            internal byte TargetDevice;
            internal byte Command;
            internal byte[] Datos;
            internal byte[] RawFrame;
            internal byte[] RawStuffedFrame;
            internal bool Response;
        }

        private Queue m_CollectionMessage = new Queue();


        internal CDllFifoMessages()
        {
        }

        internal CDllFifoMessages(MessageDll message)
        {
            m_CollectionMessage.Enqueue(message);
        }

        // Devuelve el número de mensajes almacenados en la FIFO
        internal int Number
        {
            get
            {
                return m_CollectionMessage.Count;
            }
        }

        // Devuelve la colección de mensajes almacenados
        public Queue GetTable
        {
            get
            {
                return m_CollectionMessage;
            }
        }

        // Borra toda la FIFO
        internal void Reset()
        {
            m_CollectionMessage.Clear();
        }

        // Guarda una variable Message en la FIFO
        internal void PutMessage(MessageDll message)
        {
            m_CollectionMessage.Enqueue(message);
        }

        // Recoge un mensaje de la FIFO
        internal MessageDll GetMessage()
        {
            MessageDll message = new MessageDll();

            if (Number > 0)
            {
                message = (MessageDll)(m_CollectionMessage.Dequeue());
            }
            else
            {
                message.FID = (byte)0;
                message.Command = (byte)0;
                message.SourceDevice = (byte)0;
                message.TargetDevice = (byte)0;
                message.Datos = new byte[] { };
                message.RawFrame = new byte[] { };
                message.RawStuffedFrame = new byte[] { };
            }

            return message;
        }

        // Lee no borra un mensaje de la FIFO
        internal MessageDll ReadMessage()
        {
            MessageDll message = new MessageDll();

            if (Number > 0)
            {
                message = (MessageDll)(m_CollectionMessage.Peek());
            }
            else
            {
                message.FID = (byte)0;
                message.Command = (byte)0;
                message.SourceDevice = (byte)0;
                message.TargetDevice = (byte)0;
                message.Datos = new byte[] { };
                message.RawFrame = new byte[] { };
                message.RawStuffedFrame = new byte[] { };
            }

            return message;
        }

    }
}
