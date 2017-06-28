// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;

namespace JBC_Connect
{
	internal class CMessagesSentManager
	{

		//Pending messages constants
		private const int TIME_GARBAGE_EXPIRED_MESSAGE = 10000;
		private const int TIME_WAIT_RESPONSE_SENT_MESSAGE = 30000; //se establece un tiempo máximo de espera sin tener respuesta del equipo de 30s (se mide en ms)


		private MessageHashtable m_SentMessageTable = new MessageHashtable(); //Aqui se guarda todas las ordenes pendientes
		private Thread m_ThreadGarbageMessage;
		private bool m_ThreadGarbageMessageAlive = true;
		private static Mutex m_mutexGarbageMessage = new Mutex();


		public CMessagesSentManager()
		{
			//Initialize garbage collector for older messages
			m_ThreadGarbageMessage = new Thread(new System.Threading.ThreadStart(GarbageExpiredMessage));
			m_ThreadGarbageMessage.IsBackground = true;
			m_ThreadGarbageMessage.Start();
		}

		public void Dispose()
		{
			//Terminate threads
			m_ThreadGarbageMessageAlive = false;
		}

		public void PutMessage(uint NumStream, MessageHashtable.Message NewMessage)
		{
			m_mutexGarbageMessage.WaitOne();
			m_SentMessageTable.PutMessage(NumStream, NewMessage);
			m_mutexGarbageMessage.ReleaseMutex();
		}

		public void RemoveMessage(uint NumStream)
		{
			m_mutexGarbageMessage.WaitOne();
			m_SentMessageTable.RemoveMessage(NumStream);
			m_mutexGarbageMessage.ReleaseMutex();
		}

		public bool ReadMessage(uint NumStream, ref MessageHashtable.Message retMessage)
		{
			m_mutexGarbageMessage.WaitOne();
			bool bOk = m_SentMessageTable.ReadMessage(NumStream, ref retMessage);
			m_mutexGarbageMessage.ReleaseMutex();

			return bOk;
		}

		private void GarbageExpiredMessage()
		{
			while (m_ThreadGarbageMessageAlive)
			{
				m_mutexGarbageMessage.WaitOne();

				List<uint> expiredMessage = new List<uint>();

				foreach (DictionaryEntry Elemento in m_SentMessageTable.GetTable)
				{
					DateTime HoraActual = default(DateTime);
					DateTime HoraLimite = default(DateTime);
					MessageHashtable.Message Mensaje = new MessageHashtable.Message();

					Mensaje = (MessageHashtable.Message)Elemento.Value;
					// 2014/06/13 edu no eliminar los ACK (fin transaction)
					if (Mensaje.Command == (byte)EnumCommandFrame.M_ACK)
					{
						continue;
					}

					HoraActual = DateTime.Now;
					HoraLimite = Mensaje.Hora.AddMilliseconds(TIME_WAIT_RESPONSE_SENT_MESSAGE);

					if (HoraActual > HoraLimite)
					{
						//mensaje a eliminar
						expiredMessage.Add((uint)Elemento.Key);
					}
				}

				//Eliminamos los mensajes expirados
				foreach (uint mess in expiredMessage)
				{
					m_SentMessageTable.RemoveMessage(mess);
				}

				m_mutexGarbageMessage.ReleaseMutex();
				Thread.Sleep(TIME_GARBAGE_EXPIRED_MESSAGE);
			}
		}

	}
}
