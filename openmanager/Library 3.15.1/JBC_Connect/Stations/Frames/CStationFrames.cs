// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Text;
using System.Threading;
using DataJBC;


namespace JBC_Connect
{
	internal class CStationFrames
	{

		protected byte m_StationNumDevice; //Station Address

		//Communication
		protected CCommunicationChannel m_ComChannel;

		//Pending messages
		protected CMessagesSentManager m_MessagesSentManager = new CMessagesSentManager(); //Aqui se guarda todas las ordenes pendientes


		public delegate void ConnectionErrorEventHandler(EnumConnectError ErrorType, byte address, byte command);
		private ConnectionErrorEventHandler ConnectionErrorEvent;

		public event ConnectionErrorEventHandler ConnectionError
		{
			add
			{
				ConnectionErrorEvent = (ConnectionErrorEventHandler)System.Delegate.Combine(ConnectionErrorEvent, value);
			}
			remove
			{
				ConnectionErrorEvent = (ConnectionErrorEventHandler)System.Delegate.Remove(ConnectionErrorEvent, value);
			}
		}

		public delegate void ResetSendedEventHandler(byte address);
		private ResetSendedEventHandler ResetSendedEvent;

		public event ResetSendedEventHandler ResetSended
		{
			add
			{
				ResetSendedEvent = (ResetSendedEventHandler)System.Delegate.Combine(ResetSendedEvent, value);
			}
			remove
			{
				ResetSendedEvent = (ResetSendedEventHandler)System.Delegate.Remove(ResetSendedEvent, value);
			}
		}



		public void Dispose()
		{
			m_MessagesSentManager.Dispose();
			m_ComChannel = null;
		}

		public void DeleteComChannel()
		{
			m_ComChannel.Dispose();
			m_ComChannel = null;
		}

#region CODE FRAMES

#region MarkACK 0x06

		/// <summary>
		/// Envía un M_ACK para que la estación devuelva un M_ACK. Se devuelve el número de mensaje.
		/// Cuando la estación recibe un M_ACK, se genera un Evento de confirmación con el número de mensaje.
		/// Se utiliza para confirmar que se han ejecutado las operaciones anteriores
		/// </summary>
		/// <remarks></remarks>
		public uint MarkACK()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame.M_ACK;

			return SendMessage(Datos, Command);
		}

#endregion


#region Reset 0x20

		/// <summary>
		/// Guarda el puerto utilizado y crea la instancia de la pila de comunicaciones
		/// </summary>
		public void DeviceReset()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame.M_RESET;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Guarda el puerto utilizado y crea la instancia de la pila de comunicaciones
		/// </summary>
		/// <param name="address">Dirección destino</param>
		public void DeviceReset(byte address)
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame.M_RESET;

			SendMessage(Datos, Command, address);
		}

#endregion


#region Versión 0x21

		/// <summary>
		/// Lee del equipo conectado la vesión del hard y del soft, el modelo y el protocolo
		/// </summary>
		/// <remarks></remarks>
		internal void ReadDeviceVersions()
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame.M_FIRMWARE;

			SendMessage(Datos, Command);
		}

		/// <summary>
		/// Lee del equipo conectado la vesión del hard y del soft, el modelo y el protocolo
		/// </summary>
		/// <remarks>Posibilidad de encontrar estaciones conectadas a otras estaciones (p.e. DME -> PSE -> PSE -> PSE)</remarks>
		internal void ReadDeviceVersions(byte address)
		{
			//Datos
			byte[] Datos = new byte[] { };

			//Command
			byte Command = (byte)EnumCommandFrame.M_FIRMWARE;

			SendMessage(Datos, Command, address);
		}

#endregion

#endregion

#region CHECK PARAMETERS

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

#endregion

#region RUTINAS

		protected Idioma GetLanguageFromLangText(string sLang)
		{
			switch (sLang)
			{
				case "EN": //Inglés
					return Idioma.I_Ingles;
				case "ES": //Español
					return Idioma.I_Espanol;
				case "DE": //Alemán
					return Idioma.I_Aleman;
				case "FR": //Francés
					return Idioma.I_Frances;
				case "IT": //Italiano
					return Idioma.I_Italiano;
				case "PT": //Portugués
					return Idioma.I_Portugues;
				case "ZH": //Chino
					return Idioma.I_Chino;
				case "JA": //Japonés
					return Idioma.I_Japones;
				case "KO": //Coreano
					return Idioma.I_Coreano;
				case "RU": //Ruso
					return Idioma.I_Ruso;
				default:
					return Idioma.I_Ingles;
			}
		}

		protected string GetLangTextFromLanguage(Idioma lang)
		{
			switch (lang)
			{
				case Idioma.I_Ingles:
					return "EN"; //Inglés
				case Idioma.I_Espanol:
					return "ES"; //Español
				case Idioma.I_Aleman:
					return "DE"; //Alemán
				case Idioma.I_Frances:
					return "FR"; //Francés
				case Idioma.I_Italiano:
					return "IT"; //Italiano
				case Idioma.I_Portugues:
					return "PT"; //Portugués
				case Idioma.I_Chino:
					return "ZH"; //Chino
				case Idioma.I_Japones:
					return "JA"; //Japonés
				case Idioma.I_Coreano:
					return "KO"; //Coreano
				case Idioma.I_Ruso:
					return "RU"; //Ruso
				default:
					return "EN";
			}
		}

#endregion

#region COMMUNICATION

#if Station_UnitTest

		internal uint SendMessage_Test(byte[] Datos, byte command)
		{
			//By default: station address
			return SendMessage(Datos, command, m_StationNumDevice);
		}

#endif

		protected uint SendMessage(byte[] Datos, byte command, bool delayedResponse = false)
		{
			//By default: station address
			return SendMessage(Datos, command, m_StationNumDevice, delayedResponse);
		}

		protected uint SendMessage(byte[] Datos, byte command, byte targetAddress, bool delayedResponse = false)
		{

			//Send message
			uint NumMessage = (uint)0;

			if (m_ComChannel != null)
			{
				NumMessage = m_ComChannel.Send(Datos, command, targetAddress, delayedResponse);

				//Save sended message
				MessageHashtable.Message MessageInt = new MessageHashtable.Message();
				MessageInt.Datos = Datos;
				MessageInt.Command = command;
				MessageInt.Device = targetAddress;
				m_MessagesSentManager.PutMessage(NumMessage, MessageInt);
			}

			return NumMessage;
		}

		public void ComChannelConnectionError(EnumConnectError ErrorType, byte address, byte command)
		{
			if (ConnectionErrorEvent != null)
				ConnectionErrorEvent(ErrorType, address, command);
		}

		public void ComChannelResetSended(byte address)
		{
			if (ResetSendedEvent != null)
				ResetSendedEvent(address);
		}

#endregion

	}
}
