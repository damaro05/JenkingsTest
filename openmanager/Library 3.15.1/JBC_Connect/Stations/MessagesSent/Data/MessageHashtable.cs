// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports


// 12/03/2013 #edu# Se añade Stand_OnOff en cls_StatusTool
// 22/03/2013 Added ToolTemperatureLevels Enum #edu#
// 07/01/2014 Se añaden las herramientas de la estación Nano a la lista GenericStationTools
// 01/11/2014 Se cambia NT205 por NT105
// 17/02/2015 Se añade EndedTransactions As New List(Of UInteger), maxEndedTransactions, addEndedTransaction y queryEndedTransaction a la clase cls_Station_Sold
// 24/07/2015 Estación PSE se define sin ráfagas. Además la versión nueva de DME, también se define sin ráfagas.
// 24/07/2015 Falta implementar: Se añade herramienta T470 = 9. No está implementada en la estación, pero pertenece a las combinaciones T245+HD o T245+HDE = T470

namespace JBC_Connect
{
    internal class MessageHashtable
    {

        private Hashtable MessageTableInt = new Hashtable(); //Aqui se guarda todas las ordenes pendiente

        public struct Message
        {
            public byte Command;
            public byte Device;
            public byte[] Datos;
            public DateTime Hora;
        }

        public MessageHashtable()
        {
            MessageTableInt.Clear();
        }

        public MessageHashtable(uint NumStream, Message NewMessage)
        {
            MessageTableInt.Clear();
            // Si es cero hay un error y no guardar
            if (NumStream != 0)
            {
                PutMessage(NumStream, NewMessage);
            }
        }

        // Guarda una variable Message
        public void PutMessage(uint NumStream, Message NewMessage)
        {
            if (NumStream == 0) // Si el número es cero es que no hay conexión no guardar y esperar mensaje de error
            {
                return;
            }
            NewMessage.Hora = DateTime.Now; // también se guarda la hora en la que se guardo
            MessageTableInt.Add(NumStream, NewMessage);
        }

        // Elimina un mensaje de la colección
        public void RemoveMessage(uint NumStream)
        {
            if (MessageTableInt.Contains(NumStream))
            {
                MessageTableInt.Remove(NumStream);
            }
        }

        // Lee no borra un mensaje
        public bool ReadMessage(uint NumStream, ref Message retMessage)
        {
            if (MessageTableInt.ContainsKey(NumStream))
            {
                retMessage = (Message)(MessageTableInt[NumStream]);
                return true;
            }
            else
            {
                return false;
            }
        }

        // Devuelve el número de mensajes almacenados
        public int Count
        {
            get
            {
                return MessageTableInt.Count;
            }
        }

        // Devuelve la colección de mensajes almacenados
        public Hashtable GetTable
        {
            get
            {
                return MessageTableInt;
            }
        }

        // Borra toda la tabla
        public void Reset()
        {
            MessageTableInt.Clear();
        }

    }
}
