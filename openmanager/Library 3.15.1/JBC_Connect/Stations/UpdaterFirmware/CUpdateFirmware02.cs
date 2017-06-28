// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using DataJBC;
using RoutinesJBC;

namespace JBC_Connect
{
	internal class CUpdateFirmware02
	{

		internal enum UpdateFirmwareState
		{
			None,
				ClearingFlash,
				SendingAddress,
				SendingData,
				EndProgramming
		}

		private string m_UUID;
		private UpdateFirmwareState m_UpdateFirmwareState = UpdateFirmwareState.None; //Indica el estado de actualización de un micro
		private CMicroPrograms02 m_MicroPrograms = new CMicroPrograms02(); //Gestor del fichero de actualización
		private CMicroPrograms02.s19rec m_s19rec = null; //Record del fichero de actualización
		private int m_s19recAntAddress = 0; //Anterior dirección de memoria actualizada
		private byte m_UpdateFirmwareSequence = (byte)0; //Número de secuencia
		private Hashtable m_MicrosPendingUpdate = new Hashtable(); //Micros pendientes de actualizar
		private List<byte> m_MicrosPendingReset = new List<byte>(); //Micros actualizados y pendientes de hacer un reset
		private short m_MicroUpdatingProgress = (short)(-1); //Dirección del micro que se está actualizando
		private List<CFirmwareStation> m_InfoUpdateFirmware; //Información de las versiones de firmware a actualizar
		private CStationFrames02_SOLD m_FramesSOLD_02 = null;
		private CStationFrames02_HA m_FramesHA_02 = null; // soporte de estaciones HA (Hot Air)
		private CStationFrames02_SF m_FramesSF_02 = null; // soporte de estaciones SF (Soldering feeder)
		private CStationFrames02_FE m_FramesFE_02 = null; // soporte de estaciones FE (Fume extractor)

		public delegate void UpdateMicroFirmwareFinishedEventHandler(string stationUUID);
		private UpdateMicroFirmwareFinishedEventHandler UpdateMicroFirmwareFinishedEvent;

		public event UpdateMicroFirmwareFinishedEventHandler UpdateMicroFirmwareFinished
		{
			add
			{
				UpdateMicroFirmwareFinishedEvent = (UpdateMicroFirmwareFinishedEventHandler)System.Delegate.Combine(UpdateMicroFirmwareFinishedEvent, value);
			}
			remove
			{
				UpdateMicroFirmwareFinishedEvent = (UpdateMicroFirmwareFinishedEventHandler)System.Delegate.Remove(UpdateMicroFirmwareFinishedEvent, value);
			}
		}


		public CUpdateFirmware02(string stationUUID, CStationFrames02_SOLD frames_02)
		{
			m_UUID = stationUUID;
			m_FramesSOLD_02 = frames_02;
			m_FramesSOLD_02.ClearingFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesSOLD_02.AddressMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesSOLD_02.DataMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesSOLD_02.EndProgFinished += Event_UpdateFirmwareStepFinished;
		}

		public CUpdateFirmware02(string stationUUID, CStationFrames02_HA frames_02)
		{
			m_UUID = stationUUID;
			m_FramesHA_02 = frames_02;
			m_FramesHA_02.ClearingFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesHA_02.AddressMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesHA_02.DataMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesHA_02.EndProgFinished += Event_UpdateFirmwareStepFinished;
		}

		public CUpdateFirmware02(string stationUUID, CStationFrames02_SF frames_02)
		{
			m_UUID = stationUUID;
			m_FramesSF_02 = frames_02;
			m_FramesSF_02.ClearingFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesSF_02.AddressMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesSF_02.DataMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesSF_02.EndProgFinished += Event_UpdateFirmwareStepFinished;
		}

		public CUpdateFirmware02(string stationUUID, CStationFrames02_FE frames_02)
		{
			m_UUID = stationUUID;
			m_FramesFE_02 = frames_02;
			m_FramesFE_02.ClearingFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesFE_02.AddressMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesFE_02.DataMemoryFlashFinished += Event_UpdateFirmwareStepFinished;
			m_FramesFE_02.EndProgFinished += Event_UpdateFirmwareStepFinished;
		}

		/// <summary>
		/// Initialize the update process of all micros
		/// </summary>
		/// <param name="infoUpdateFirmware">Update firmware information</param>
		internal void UpdateMicrosFirmware(List<CFirmwareStation> infoUpdateFirmware, Hashtable stationMicros)
		{

			//Check updating in progress
			if (m_MicroUpdatingProgress == -1)
			{

				//Copy info update firmware
				m_InfoUpdateFirmware = infoUpdateFirmware;

				//Copy micros pending update
				m_MicrosPendingUpdate.Clear();
				foreach (DictionaryEntry stationMicroEntry in stationMicros)
				{
					m_MicrosPendingUpdate.Add(stationMicroEntry.Key, stationMicroEntry.Value);
				}

				//Init list micros updated and pending reset
				m_MicrosPendingReset.Clear();

				UpdateNextMicro();
			}
		}

		/// <summary>
		/// Initialize the update process of one micro
		/// </summary>
		private void UpdateNextMicro()
		{

			//
			//Select next micro to update
			//
			CFirmwareStation infoUpdateFirmware = null;
			m_MicroUpdatingProgress = (short)(-1);

			//Recorrer la lista de micros para encontrar el micro actualizable con la dirección más alta
			foreach (DictionaryEntry microPendingUpdateEntry in m_MicrosPendingUpdate)
			{

				//Recorrer la lista de firmwares disponibles
				foreach (CFirmwareStation infoUpdateFirmwareEntry in m_InfoUpdateFirmware)
				{

					//Coincide el modelo de estación
					if (((CFirmwareStation)microPendingUpdateEntry.Value).Model == infoUpdateFirmwareEntry.Model)
					{

						//Coincide la versión de hardware
						if (((CFirmwareStation)microPendingUpdateEntry.Value).HardwareVersion == infoUpdateFirmwareEntry.HardwareVersion)
						{

							//Hemos encontrado un micro a actualizar
							//Comprobar que se vaya a cambiar la versión de software
							if (((CFirmwareStation)microPendingUpdateEntry.Value).SoftwareVersion != infoUpdateFirmwareEntry.SoftwareVersion)
							{

								//Comprobar que dirección es más alta
								if (m_MicroUpdatingProgress < System.Convert.ToByte(microPendingUpdateEntry.Key))
								{
									infoUpdateFirmware = infoUpdateFirmwareEntry;
									m_MicroUpdatingProgress = System.Convert.ToByte(microPendingUpdateEntry.Key);
								}
							}

							//Siguiente micro a buscar
							break;
						}
					}
				}
			}

			//
			//Actualizar micro
			//

			//No hay más micros para actualizar
			if (m_MicroUpdatingProgress == -1)
			{

				//Enviar Reset a todos los micros
				foreach (byte microAddress in m_MicrosPendingReset)
				{
					if (m_FramesSOLD_02 != null)
					{
						m_FramesSOLD_02.DeviceReset(microAddress);
					}
					if (m_FramesHA_02 != null)
					{
						m_FramesHA_02.DeviceReset(microAddress);
					}
					if (m_FramesSF_02 != null)
					{
						m_FramesSF_02.DeviceReset(microAddress);
					}
					if (m_FramesFE_02 != null)
					{
						m_FramesFE_02.DeviceReset(microAddress);
					}
				}

				m_MicrosPendingUpdate = null;
				if (UpdateMicroFirmwareFinishedEvent != null)
					UpdateMicroFirmwareFinishedEvent(m_UUID);
			}
			else
			{

				//Initialize variables
				m_s19rec = null;
				m_s19recAntAddress = 0;

				string sErr = "";

				//Decrypt
				byte[] fileReader = File.ReadAllBytes(infoUpdateFirmware.FileName);
				byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
				byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
				string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

				//Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10) and remove empty lines
				string[] sTextLines = decrypted.Replace("\r", "").Split((char[])(new[] { '\n' }), StringSplitOptions.RemoveEmptyEntries);


				//Cargamos el archivo de firmware
				if (m_MicroPrograms.LoadFromData(ref sTextLines, ref sErr))
				{
					m_MicroPrograms.initUpdaterData();

					//Actualiza el estado
					m_UpdateFirmwareState = UpdateFirmwareState.ClearingFlash;

					//Borrar memoria flash
					if (m_FramesSOLD_02 != null)
					{
						m_FramesSOLD_02.ClearMemoryFlash(infoUpdateFirmware.ProtocolVersion + ":" +
								infoUpdateFirmware.ModelVersion + ":" +
								infoUpdateFirmware.SoftwareVersion + ":" +
								infoUpdateFirmware.HardwareVersion + ":B", (byte)m_MicroUpdatingProgress);
					}
					if (m_FramesHA_02 != null)
					{
						m_FramesHA_02.ClearMemoryFlash(infoUpdateFirmware.ProtocolVersion + ":" +
								infoUpdateFirmware.ModelVersion + ":" +
								infoUpdateFirmware.SoftwareVersion + ":" +
								infoUpdateFirmware.HardwareVersion + ":B", (byte)m_MicroUpdatingProgress);
					}
					if (m_FramesSF_02 != null)
					{
						m_FramesSF_02.ClearMemoryFlash(infoUpdateFirmware.ProtocolVersion + ":" +
								infoUpdateFirmware.ModelVersion + ":" +
								infoUpdateFirmware.SoftwareVersion + ":" +
								infoUpdateFirmware.HardwareVersion + ":B", (byte)m_MicroUpdatingProgress);
					}
					if (m_FramesFE_02 != null)
					{
						m_FramesFE_02.ClearMemoryFlash(infoUpdateFirmware.ProtocolVersion + ":" +
								infoUpdateFirmware.ModelVersion + ":" +
								infoUpdateFirmware.SoftwareVersion + ":" +
								infoUpdateFirmware.HardwareVersion + ":B", (byte) m_MicroUpdatingProgress);
					}

					//No se puede cargar el archivo de firmware probamos con el siguiente micro
				}
				else
				{
					m_MicrosPendingUpdate.Remove((byte)m_MicroUpdatingProgress);
					UpdateNextMicro();
				}
			}
		}

		/// <summary>
		/// Next step in the update firmware process for 1 micro. Send address, data bytes or end program
		/// </summary>
		/// <remarks></remarks>
		private void Event_UpdateFirmwareStepFinished()
		{

			//Sequence (1 byte) + address data bytes
			byte[] SendData = null;

			//
			//Pasar al siguiente estado
			//
			if (m_UpdateFirmwareState == UpdateFirmwareState.ClearingFlash |
					m_UpdateFirmwareState == UpdateFirmwareState.SendingData)
			{
				if (m_UpdateFirmwareState == UpdateFirmwareState.ClearingFlash)
				{
					m_UpdateFirmwareState = UpdateFirmwareState.SendingAddress;

					//Inicializa el contador de secuencia
					m_UpdateFirmwareSequence = (byte)0;
				}

				if (m_s19rec != null)
				{
					m_s19recAntAddress = RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true); //is BigEndian
				}

				//No hay mas datos para leer
				if (!m_MicroPrograms.getNextUpdaterData(ref m_s19rec))
				{
					m_UpdateFirmwareState = UpdateFirmwareState.EndProgramming;

					if (m_FramesSOLD_02 != null)
					{
						m_FramesSOLD_02.EndProgramming((byte)m_MicroUpdatingProgress);
					}
					if (m_FramesHA_02 != null)
					{
						m_FramesHA_02.EndProgramming((byte)m_MicroUpdatingProgress);
					}
					if (m_FramesSF_02 != null)
					{
						m_FramesSF_02.EndProgramming((byte)m_MicroUpdatingProgress);
					}
					if (m_FramesFE_02 != null)
					{
						m_FramesFE_02.EndProgramming((byte) m_MicroUpdatingProgress);
					}
				}

			}
			else if (m_UpdateFirmwareState == UpdateFirmwareState.SendingAddress)
			{
				m_UpdateFirmwareState = UpdateFirmwareState.SendingData;

			}
			else if (m_UpdateFirmwareState == UpdateFirmwareState.EndProgramming)
			{

				m_UpdateFirmwareState = UpdateFirmwareState.None;

				//Añadir micro actualizado
				m_MicrosPendingReset.Add((byte)m_MicroUpdatingProgress);

				//Actualizar siguiente micro
				m_MicrosPendingUpdate.Remove((byte)m_MicroUpdatingProgress);
				UpdateNextMicro();
			}

			//
			//Enviar dato
			//
			if (m_UpdateFirmwareState == UpdateFirmwareState.SendingData)
			{

				//No es un bloque consecutivo. Enviar address
				if ((m_s19recAntAddress + CMicroPrograms02.BLOCK_MICRO_PROGRAM) < RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true)) //is BigEndian
				{
					m_UpdateFirmwareState = UpdateFirmwareState.SendingAddress;

				}
				else
				{
					SendData = new byte[m_s19rec.data.ToArray().Length + 1];
					SendData[0] = m_UpdateFirmwareSequence;
					Array.Copy(m_s19rec.data.ToArray(), 0, SendData, 1, m_s19rec.data.ToArray().Length);

					m_s19recAntAddress = RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true); //is BigEndian

					if (m_FramesSOLD_02 != null)
					{
						m_FramesSOLD_02.DataMemoryFlash(SendData, (byte)m_MicroUpdatingProgress);
					}
					if (m_FramesHA_02 != null)
					{
						m_FramesHA_02.DataMemoryFlash(SendData, (byte)m_MicroUpdatingProgress);
					}
					if (m_FramesSF_02 != null)
					{
						m_FramesSF_02.DataMemoryFlash(SendData, (byte)m_MicroUpdatingProgress);
					}
					if (m_FramesFE_02 != null)
					{
						m_FramesFE_02.DataMemoryFlash(SendData, (byte) m_MicroUpdatingProgress);
					}
				}
			}

			//
			//Enviar address
			//
			if (m_UpdateFirmwareState == UpdateFirmwareState.SendingAddress)
			{

				SendData = new byte[m_s19rec.address.ToArray().Length + 1];
				SendData[0] = m_UpdateFirmwareSequence;
				Array.Copy(m_s19rec.address.ToArray(), 0, SendData, 1, m_s19rec.address.ToArray().Length);

				m_s19recAntAddress = RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true); //is BigEndian

				if (m_FramesSOLD_02 != null)
				{
					m_FramesSOLD_02.AddressMemoryFlash(SendData, (byte)m_MicroUpdatingProgress);
				}
				if (m_FramesHA_02 != null)
				{
					m_FramesHA_02.AddressMemoryFlash(SendData, (byte)m_MicroUpdatingProgress);
				}
				if (m_FramesSF_02 != null)
				{
					m_FramesSF_02.AddressMemoryFlash(SendData, (byte)m_MicroUpdatingProgress);
				}
				if (m_FramesFE_02 != null)
				{
					m_FramesFE_02.AddressMemoryFlash(SendData, (byte) m_MicroUpdatingProgress);
				}
			}

			//Siguiente secuencia
			if (m_UpdateFirmwareSequence == 255)
			{
				m_UpdateFirmwareSequence = (byte)0;
			}
			else
			{
				m_UpdateFirmwareSequence += 1;
			}

		}

	}
}
