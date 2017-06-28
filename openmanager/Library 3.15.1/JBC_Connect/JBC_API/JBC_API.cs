using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using DataJBC;
using RoutinesJBC;

// 2012/12/12 Changed in all functions (#Edu#):
//   RaiseEvent UserError(stations(index).id, New Cerror(Cerror.cErrorCodes.STATION_ID_NOT_FOUND, "Station ID not found."))
//   RaiseEvent UserError(stationUUID, New Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."))
// 14/02/2013 Added apiInitialControlMode - #edu#
// 12/03/2013 Added GetPortToolStandStatus - #edu#
// 14/03/2013 sub DefaultStationParametters signed as Deprecated (replaced by DefaultStationParameters)
// 15/03/2013 Se implementa un SetTransaction y un RaiseEvent TransactionFinished por M_ACK (MarkACK()).
//   Sirve para saber cuando se han ejecutado las operaciones anteriores #edu#
// 20/03/2013 Permits newTemp to be JBC_API.TEMP_LEVEL_OFF in SetPortToolTempLevel
// 10/12/2013 Se modifica el redondeo de temperatura de UTI -> Fahrenheit (ToRoundFahrenheit)
// 20/12/2013 Se modifican los límites de temperatura máximos, según el modelo de estación
// 07/01/2014 Se añaden las herramientas de la estación Nano a la lista GenericStationTools
// 13/01/2014 Se quita SetStationPowerLimit de la API y del Manager (se mantiene en el protocolo)
// 01/04/2014 Se vuelve a habilitar Get/SetStationPowerLimit
// 10/06/2014 Se añaden las funciones GetStationModelType y GetStationModelVersion
// 27/06/2014 Se añade la función GetStationFeatures
// 01/11/2014 Se cambia NT205 por NT105
// 05/11/2014 Se habilita la conexión con otros devices diferentes a H00
// 20/11/2014 En la conexión con devices diferentes faltaba pasar el stationNumDevice al stack APL para que lo use el SYN (2.14.09.1)
// 19/11/2014 Se añade GetSupportedTools
// 17/02/2915 Se añade QueryTransaction
// 30/03/2015 Falta habilitar ReadDeviceID y WriteDeviceID cuando se implemente definitivamente en las estaciones
// 25/05/2015 Modo continuo: En protocolo 2 viene un byte adicional, que no está en la documentación, entre las potencias y el estado
//            Documentación: por puerto    Temp: 4Bytes Potencia: 4Bytes Estado: 1Byte Reservado: 1Byte
//            La estación envía, por pueto Temp: 4Bytes Potencia: 4Bytes NoDocumentado: 1Byte Estado: 1Byte Reservado: 1Byte
//            Se marcan las DDE y NAE de las versiones erróneas con un feature: ContinuosuModeDataExtraByte
// 08/06/2015 Se añaden 2 tramas HBB y HBC - Read and Write Partameters Locked y features.ParamsLockedFrame
// 01/06/2016 Se implementa la lectura y grabación de Internal UID de las estaciones.
//            Cuando se detecta una estación, si el UID está en blanco, se le genera uno nuevo y se graba.
//            Se detecta que en protocolo 01, al grabar un UID de 20 caracteres, añade los últimos 7 también al Software del string de FIRMWARE,
//            por lo tanto se toman sólo los 7 primeros caracteres del software y del hardware
//            Se implementa GetStationCOM (COMx o IP) y GetStationConnectionType (U=USB, E=Ethernet)

//#PG# Añadido a Programmers's Guide

namespace JBC_Connect
{
	/// <summary>
	/// JBC stations comunications class. A general API with the necessary methods to manage
	/// a JBC station from a computer.
	/// </summary>
	/// <remarks></remarks>
	///
	public class JBC_API
	{
		//Search
		private CSearchManager m_SearchManager;

		//Stations
		public Hashtable StationsList = new Hashtable(); // CStationElement
		private static Mutex m_mutexStationsList = new Mutex();

		//Update Firmware
		private Hashtable m_listStationsPendingUpdate = new Hashtable(); //UUID <-> <hw, sw> , para cada estación se guarda una lista de que software tiene para un hardware especifico
		private string m_StationUpdatingProgress = ""; //UUID de estación que se está actualizando

		//Communication
		private CStationAvailableCom m_StationAvailableCom = new CStationAvailableCom();


        #region EVENTS

		/// <summary>
		/// This event is launched when a new station has been detected
		/// and correctly linked. Gives the identifier required in order to
		/// call the class methods.
		/// </summary>
		/// <param name="stationUUID" >The identifier of the new station connected</param>
		/// <remarks></remarks>
		public delegate void NewStationConnectedEventHandler(string stationUUID);
		private NewStationConnectedEventHandler NewStationConnectedEvent;

		public event NewStationConnectedEventHandler NewStationConnected
		{
			add
			{
				NewStationConnectedEvent = (NewStationConnectedEventHandler) System.Delegate.Combine(NewStationConnectedEvent, value);
			}
			remove
			{
				NewStationConnectedEvent = (NewStationConnectedEventHandler) System.Delegate.Remove(NewStationConnectedEvent, value);
			}
		}


		/// <summary>
		/// This event is launched when a identified station is disconnected or
		/// undetected. Gives the identifier of the disconnected station in order to
		/// manage the disconection in user application.
		/// </summary>
		/// <param name="stationUUID">An identifier to the disconnected station</param>
		/// <remarks></remarks>
		public delegate void StationDisconnectedEventHandler(string stationUUID);
		private StationDisconnectedEventHandler StationDisconnectedEvent;

		public event StationDisconnectedEventHandler StationDisconnected
		{
			add
			{
				StationDisconnectedEvent = (StationDisconnectedEventHandler) System.Delegate.Combine(StationDisconnectedEvent, value);
			}
			remove
			{
				StationDisconnectedEvent = (StationDisconnectedEventHandler) System.Delegate.Remove(StationDisconnectedEvent, value);
			}
		}


		/// <summary>
		/// When an error occurs this event is launched. Gives a Cerror object with
		/// the error information.
		/// </summary>
		/// <param name="stationUUID">An identifier to the station with error</param>
		/// <param name="err">The Cerror object with the error information</param>
		/// <remarks></remarks>
		public delegate void UserErrorEventHandler(string stationUUID, Cerror err);
		private UserErrorEventHandler UserErrorEvent;

		public event UserErrorEventHandler UserError
		{
			add
			{
				UserErrorEvent = (UserErrorEventHandler) System.Delegate.Combine(UserErrorEvent, value);
			}
			remove
			{
				UserErrorEvent = (UserErrorEventHandler) System.Delegate.Remove(UserErrorEvent, value);
			}
		}


		/// <summary>
		/// This event is launched to inform that previous operations are completed.
		/// Use the SetTransaction function to require station to launch this event.
		/// </summary>
		/// <param name="stationUUID">An identifier to the station</param>
		/// <param name="transactionID">Transaction ID received</param>
		/// <remarks></remarks>
		public delegate void TransactionFinishedEventHandler(string stationUUID, uint transactionID);
		private TransactionFinishedEventHandler TransactionFinishedEvent;

		public event TransactionFinishedEventHandler TransactionFinished
		{
			add
			{
				TransactionFinishedEvent = (TransactionFinishedEventHandler) System.Delegate.Combine(TransactionFinishedEvent, value);
			}
			remove
			{
				TransactionFinishedEvent = (TransactionFinishedEventHandler) System.Delegate.Remove(TransactionFinishedEvent, value);
			}
		}


		public delegate void GetUpdateFirmwareEventHandler(ref List<CFirmwareStation> versionMicros);
		private GetUpdateFirmwareEventHandler GetUpdateFirmwareEvent;

		public event GetUpdateFirmwareEventHandler GetUpdateFirmware
		{
			add
			{
				GetUpdateFirmwareEvent = (GetUpdateFirmwareEventHandler) System.Delegate.Combine(GetUpdateFirmwareEvent, value);
			}
			remove
			{
				GetUpdateFirmwareEvent = (GetUpdateFirmwareEventHandler) System.Delegate.Remove(GetUpdateFirmwareEvent, value);
			}
		}

        #endregion


		public void Close()
		{
			StopSearch(SearchMode.ALL);

			m_mutexStationsList.WaitOne();
			ArrayList listStationsUUID = new ArrayList(StationsList.Keys);
			m_mutexStationsList.ReleaseMutex();

			foreach (string stationUUID in listStationsUUID)
			{
				launchStationDisconnected(stationUUID);
			}

			m_StationAvailableCom.Dispose();
		}


#region API STATION METHODS

#region API General station methods


		public int GetStationCount()
		{
			int stnCount = 0;

			m_mutexStationsList.WaitOne();
			foreach (DictionaryEntry entry in StationsList)
			{
				if (((CStationElement) entry.Value).State == StationState.Connected)
				{
					stnCount++;
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stnCount;
		}

		public string[] GetStationList()
		{
			List<string> stns = new List<string>();

			m_mutexStationsList.WaitOne();
			foreach (DictionaryEntry entry in StationsList)
			{
				if (((CStationElement) entry.Value).State == StationState.Connected)
				{
					stns.Add(((CStationElement) entry.Value).UUID);
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stns.ToArray();
		}

		/// <summary>
		/// Returns if station exists.
		/// </summary>
		/// <param name="stationUUID">Station identifier</param>
		/// <remarks></remarks>
		public bool StationExists(string stationUUID)
		{
			bool ret = false;

			m_mutexStationsList.WaitOne();
			ret = StationsList.Contains(stationUUID);
			m_mutexStationsList.ReleaseMutex();

			return ret;
		}

		/// <summary>
		/// Gets station object by Id.
		/// </summary>
		/// <param name="stationUUID">Station identifier</param>
		/// <remarks></remarks>
		internal dynamic Station(string stationUUID)
		{
			object objStation = null;

			m_mutexStationsList.WaitOne();
			if (StationsList.Contains(stationUUID))
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					objStation = ((CStationElement) (StationsList[stationUUID])).Station_SOLD;
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					objStation = ((CStationElement) (StationsList[stationUUID])).Station_HA;
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					objStation = ((CStationElement) (StationsList[stationUUID])).Station_SF;
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					objStation = ((CStationElement) (StationsList[stationUUID])).Station_FE;
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return objStation;
		}

		/// <summary>
		/// Close and reinitialize station object.
		/// </summary>
		/// <param name="stationUUID">Station identifier</param>
		/// <remarks></remarks>
		public bool ResetStation(string stationUUID)
		{
			bool ret = false;

			m_mutexStationsList.WaitOne();
			if (StationsList.Contains(stationUUID))
			{
				ret = true;
			}
			m_mutexStationsList.ReleaseMutex();

			if (ret)
			{
				launchStationDisconnected(stationUUID);
			}

			return ret;
		}

#endregion


#region API Private catchers and launchers events

		/// <summary>
		/// Function used by Cstation class to launch the station disconnected
		/// event.
		/// </summary>
		/// <param name="stationUUID">The station disconnected ID</param>
		/// <remarks></remarks>
		private void launchStationDisconnected(string stationUUID)
		{

			if (StationDisconnectedEvent != null)
				StationDisconnectedEvent(stationUUID);

			m_mutexStationsList.WaitOne();
			if (StationsList.Contains(stationUUID))
			{
				if (((CStationElement) (StationsList[stationUUID])).Station_SOLD != null)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.Dispose();
					((CStationElement) (StationsList[stationUUID])).Station_SOLD = null;
				}

				if (((CStationElement) (StationsList[stationUUID])).Station_HA != null)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.Dispose();
					((CStationElement) (StationsList[stationUUID])).Station_HA = null;
				}

				if (((CStationElement) (StationsList[stationUUID])).Station_SF != null)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.Dispose();
					((CStationElement) (StationsList[stationUUID])).Station_SF = null;
				}

				if (((CStationElement) (StationsList[stationUUID])).Station_FE != null)
				{
					((CStationElement) (StationsList[stationUUID])).Station_FE.Dispose();
					((CStationElement) (StationsList[stationUUID])).Station_FE = null;
				}

				StationsList.Remove(stationUUID);
				m_StationAvailableCom.RemoveIDStation(stationUUID);
			}

			//Cuando se desconecta una estación es posible que sea uno de los micros que se estaban actualizando, hay que volver a lanzar la rutina de actualizaciones pendientes
			if (m_listStationsPendingUpdate.Contains(stationUUID))
			{
				m_listStationsPendingUpdate.Remove(stationUUID);
				m_StationUpdatingProgress = "";
			}

			m_mutexStationsList.ReleaseMutex();

			UpdateNextStation();
		}

		/// <summary>
		/// Function used by Cstation to launch user errors as Cerror.
		/// </summary>
		/// <param name="err">The Cerror class instance of the launched error</param>
		/// <remarks></remarks>
		internal void launchUserError(string stationUUID, Cerror err)
		{
			//Launching the event
			if (UserErrorEvent != null)
				UserErrorEvent(stationUUID, err);
		}

		private void StationInitialized(string stationUUID)
		{
			bool raiseNewConnectionEvent = false;
			m_mutexStationsList.WaitOne();

			if (StationsList.Contains(stationUUID))
			{
				((CStationElement) (StationsList[stationUUID])).State = StationState.Connected;

				//Launching the user event for new station detected
				raiseNewConnectionEvent = true;

				//If is a substation copy parent parameters
				string parentUUID = ((CStationElement) (StationsList[stationUUID])).ParentUUID;

				if (StationsList.Contains(parentUUID))
				{
					if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
					{
						//Copiamos modo control
						((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetControlMode(((CStationElement) (StationsList[parentUUID])).Station_SOLD.GetControlMode());

						//Copiamos si la estación es actualizable
						((CStationElement) (StationsList[stationUUID])).Station_SOLD.Features_FirmwareUpdate = ((CStationElement) (StationsList[parentUUID])).Station_SOLD.Features_FirmwareUpdate;
					}
					else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
					{
					}
					else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
					{
					}
					else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
					{
					}
				}
			}
			m_mutexStationsList.ReleaseMutex();

			//It must be outside the mutex
			if (raiseNewConnectionEvent)
			{
				if (NewStationConnectedEvent != null)
					NewStationConnectedEvent(stationUUID);
			}
		}

#endregion


#region API Port

		public OnOff GetEnabledPort(string stationUUID, Port port)
		{
			OnOff enabled = OnOff._OFF;

			m_mutexStationsList.WaitOne();
			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetEnabledPort(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetEnabledPort(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetEnabledPort(port);
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return enabled;
		}

		public void SetEnabledPort(string stationUUID, Port port, OnOff enabled)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetEnabledPort(port, enabled);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetEnabledPort(port, enabled);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetEnabledPort(port, enabled);
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool connected to the indicated port of the indicated station. If the station or port
		/// identifiers are not correct an error event is thrown. Uses the GenericStationTools constants defined
		/// in this class.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A GenericStationTools object which is the tool identifier.</returns>
		/// <remarks></remarks>
		public GenericStationTools GetPortToolID(string stationUUID, Port port)
		{
			//#PG#
			GenericStationTools genericTool = GenericStationTools.NO_TOOL;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					genericTool = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolID(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					genericTool = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolID(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return genericTool;
		}

		/// <summary>
		/// Gets the tool selected temperature of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A Ctemperature object with the current selected temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolSelectedTemp(string stationUUID, Port port)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSelectedTemp(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSelectedTemp(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the tool selected temperature of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="newTemp">The desired new selected temperature.</param>
		/// <remarks></remarks>
		public void SetPortToolSelectedTemp(string stationUUID, Port port, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolSelectedTemp(port, newTemp);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolSelectedTemp(port, newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool selected air flow of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A per thousand value with the current selected flow.</returns>
		/// <remarks></remarks>
		public int GetPortToolSelectedFlow(string stationUUID, Port port)
		{
			//#PG#
			int ret = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					ret = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSelectedFlow(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return ret;
		}

		/// <summary>
		/// Sets the tool selected air flow of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="newFlow">The desired new selected flow (per thousand).</param>
		/// <remarks></remarks>
		public void SetPortToolSelectedFlow(string stationUUID, Port port, int newFlow)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolSelectedFlow(port, newFlow);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool selected external temperature of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A Ctemperature object with the current selected temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolSelectedExternalTemp(string stationUUID, Port port)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSelectedExternalTemp(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the tool selected external temperature of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="newTemp">The desired new selected temperature.</param>
		/// <remarks></remarks>
		public void SetPortToolSelectedExternalTemp(string stationUUID, Port port, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolSelectedExternalTemp(port, newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets if profile work mode if active, in the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>On or Off</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolProfileMode(string stationUUID, Port port)
		{
			//#PG#
			OnOff ret = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					ret = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolProfileMode(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return ret;
		}

		/// <summary>
		/// Activates or deactivates the profile mode of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="onoff">A OnOff value</param>
		/// <remarks></remarks>
		public void SetPortToolProfileMode(string stationUUID, Port port, OnOff onoff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolProfileMode(port, onoff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

		}

		/// <summary>
		/// Gets the tool actual temperature of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
		/// in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A Ctemperature object with the tool actual temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolActualTemp(string stationUUID, Port port)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolActualTemp(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolActualTemp(port); // air temp
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Gets the tool actual power of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer object with the tool actual power in per thousand units.</returns>
		/// <remarks></remarks>
		public int GetPortToolActualPower(string stationUUID, Port port)
		{
			//#PG#
			int power = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					power = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolActualPower(port));
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					power = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolActualPower(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return power;
		}

		/// <summary>
		/// Gets the tool actual flow of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer object with the tool actual flow in per thousand units.</returns>
		/// <remarks></remarks>
		public int GetPortToolActualFlow(string stationUUID, Port port)
		{
			//#PG#
			int power = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					power = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolActualFlow(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return power;
		}

		/// <summary>
		/// Gets the tool actual external temperature of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
		/// in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A Ctemperature object with the tool external temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolActualExternalTemp(string stationUUID, Port port)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolActualExternalTemp(port); // air temp
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Gets the tool actual protection TC temperature of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
		/// in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A Ctemperature object with the tool protection TC temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolProtectionTCTemp(string stationUUID, Port port)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolProtectionTC_Temp(port); // protection TC air temp
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Gets the tool current error code of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown. Uses the ToolError constants defined
		/// in this class.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A ToolError object which is the current error code of the tool.</returns>
		/// <remarks></remarks>
		public ToolError GetPortToolError(string stationUUID, Port port)
		{
			//#PG#
			ToolError toolErr = ToolError.NO_TOOL;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					toolErr = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolError(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					toolErr = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolError(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return toolErr;
		}

		/// <summary>
		/// Gets the tool actual cartridge current of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer object with the tool actual cartridge current in mA.</returns>
		/// <remarks></remarks>
		private int GetPortToolCartridgeCurrent(string stationUUID, Port port)
		{
			int cartridge = -1;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					cartridge = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolCartridgeCurrent(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return cartridge;
		}

		/// <summary>
		/// Gets the tool actual MOSFET temperature of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown. Uses the Ctemperature class defined
		/// in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A Ctemperature object with the tool actual MOSFET temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolMOStemp(string stationUUID, Port port)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolMOStemp(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Gets the tool future mode of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A ToolFutureMode object with the tool future mode.</returns>
		/// <remarks></remarks>
		public ToolFutureMode GetPortToolFutureMode(string stationUUID, Port port)
		{
			//#PG#
			ToolFutureMode futureMode = ToolFutureMode.NoFutureMode;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					futureMode = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolFutureMode(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return futureMode;
		}

		/// <summary>
		/// Gets the tool remaining time for the future mode of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer which is the remaining time in seconds.</returns>
		/// <remarks></remarks>
		public int GetPortToolTimeToFutureMode(string stationUUID, Port port)
		{
			//#PG#
			int timeFutureMode = -1;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					timeFutureMode = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolTimeToFutureMode(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return timeFutureMode;
		}

		/// <summary>
		/// Gets the current stand mode status of the indicated port of the indicated station.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the stand mode</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolStandStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff standStatus = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					standStatus = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolStandStatus(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					standStatus = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolStandStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return standStatus;
		}

		/// <summary>
		/// Gets the current sleep mode status of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the sleep mode</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolSleepStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff sleepStatus = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					sleepStatus = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return sleepStatus;
		}

		/// <summary>
		/// Gets the current hibernation mode status of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the hibernation mode</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolHibernationStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff hiberStatus = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					hiberStatus = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolHibernationStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return hiberStatus;
		}

		/// <summary>
		/// Gets the current extractor mode status of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the extractor mode</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolExtractorStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff extractorStatus = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					extractorStatus = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolExtractorStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return extractorStatus;
		}

		/// <summary>
		/// Gets the current desolder mode status of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the desolder mode</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolDesolderStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff desolderStatus = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					desolderStatus = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolDesolderStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return desolderStatus;
		}

		/// <summary>
		/// Gets the current pedal status of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the pedal</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolPedalStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff status = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolPedalStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// Gets the current pedal connected status of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the pedal connected status</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolPedalConnectedStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff status = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolPedalConnectedStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// Gets the current suction requested status of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the suction requested status</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolSuctionRequestedStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff status = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSuctionRequestedStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// Gets the current suction status of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the suction status</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolSuctionStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff status = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSuctionStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// Gets the current heater requested status of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the heater requested status</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolHeaterRequestedStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff status = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolHeaterRequestedStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// Gets the current heater status of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the heater status</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolHeaterStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff status = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolHeaterStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// Gets the current heater status of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A OnOff object which is the current status for the heater status</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolCoolingStatus(string stationUUID, Port port)
		{
			//#PG#
			OnOff status = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolCoolingStatus(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// Gets the current time to stop (seconds) of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>A integer which is the current time to stop, in seconds</returns>
		/// <remarks></remarks>
		public int GetPortToolTimeToStopStatus(string stationUUID, Port port)
		{
			//#PG#
			int status = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					status = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolTimeToStopStatus(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return status;
		}

		/// <summary>
		/// This method sets to ON or OFF the indicated port of the indicated station stand status.
		/// Soldering stations only.
		/// Depending on the sleep and hibernation delays will be one of those status.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="OnOff">The desired state for the stand mode</param>
		/// <remarks></remarks>
		public void SetPortToolStandStatus(string stationUUID, Port port, OnOff OnOff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolStandStatus(port, OnOff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// This method sets to ON or OFF the indicated port of the indicated station extractor mode.
		/// Soldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="OnOff">The desired state for the extractor mode</param>
		/// <remarks></remarks>
		public void SetPortToolExtractorStatus(string stationUUID, Port port, OnOff OnOff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolExtractorStatus(port, OnOff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// This method sets to ON or OFF the indicated port of the indicated station desolder mode.
		/// Soldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="OnOff">The desired state for the desolder mode</param>
		/// <remarks></remarks>
		public void SetPortToolDesolderStatus(string stationUUID, Port port, OnOff OnOff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolDesolderStatus(port, OnOff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// This method sets to ON or OFF the indicated port of the indicated station heater status.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="OnOff">The desired state for the heater</param>
		/// <remarks></remarks>
		public void SetPortToolHeaterStatus(string stationUUID, Port port, OnOff OnOff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolHeaterStatus(port, OnOff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// This method sets to ON or OFF the indicated port of the indicated station suction status.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="OnOff">The desired state for the suction status</param>
		/// <remarks></remarks>
		public void SetPortToolSuctionStatus(string stationUUID, Port port, OnOff OnOff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolSuctionStatus(port, OnOff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public DispenserMode_SF GetPortDispenserMode(string stationUUID, Port port)
		{
			DispenserMode_SF dispenserMode = default(DispenserMode_SF);

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					dispenserMode = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolDispenserMode(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return dispenserMode;
		}

		public void SetPortDispenserMode(string stationUUID, Port port, DispenserMode_SF dispenserMode, byte nbrProgram = 0)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetPortToolDispenserMode(port, dispenserMode, nbrProgram);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public CSpeed GetSpeed(string stationUUID, Port port)
		{
			CSpeed speed = new CSpeed();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					speed = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolSpeed(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return speed;
		}

		public void SetSpeed(string stationUUID, Port port, CSpeed speed)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetPortToolSpeed(port, speed);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public CLength GetLength(string stationUUID, Port port)
		{
			CLength length = new CLength();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					length = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolLength(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return length;
		}

		public void SetLength(string stationUUID, Port port, CLength length)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetPortToolLength(port, length);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public OnOff GetFeedingState(string stationUUID, Port port)
		{
			OnOff feedingOn = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					feedingOn = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolFeedingState(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return feedingOn;
		}

		public ushort GetFeedingPercent(string stationUUID, Port port)
		{
			ushort feedingPercent = System.Convert.ToUInt16(0);

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					feedingPercent = System.Convert.ToUInt16(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolFeedingPercent(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return feedingPercent;
		}

		public CLength GetFeedingLength(string stationUUID, Port port)
		{
			CLength length = new CLength();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					length = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolFeedingLength(port);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return length;
		}

		public byte CurrentProgramStep(string stationUUID, Port port)
		{
			byte programStep = (byte) 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					programStep = System.Convert.ToByte(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolCurrentProgramStep(port));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return programStep;
		}

#endregion


#region API Port+Tool

		/// <summary>
		/// Gets the tool fix temperature of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A Ctemperature object with the current fix temperature</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolFixTemp(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			CTemperature temp = new CTemperature(0);

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolFixTemp(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the tool fix temperature of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="newTemp">The desired new fix temperature</param>
		/// <remarks></remarks>
		public void SetPortToolFixTemp(string stationUUID, Port port, GenericStationTools tool, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolFixTemp(port, tool, newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Sets the tool fix temperature of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="newOnOff">To deactivate fix temperature (Off). If set to On, temperature will be set to MAX temp. + MIN temp. / 2</param>
		/// <remarks></remarks>
		public void SetPortToolFixTemp(string stationUUID, Port port, GenericStationTools tool, OnOff newOnOff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolFixTemp(port, tool, newOnOff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool selected temperature level of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A ToolTemperatureLevels object with the current selected temperature level</returns>
		/// <remarks></remarks>
		public ToolTemperatureLevels GetPortToolSelectedTempLevels(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			ToolTemperatureLevels toolTempLevel = ToolTemperatureLevels.NO_LEVELS;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					toolTempLevel = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSelectedTempLevel(port, tool);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					toolTempLevel = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSelectedTempLevel(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return toolTempLevel;
		}

		/// <summary>
		/// Gets status of the selected temperature levels of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A OnOff object which is the current status for the selected temperature levels</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolSelectedTempLevelsEnabled(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			OnOff enabled = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSelectedTempLevelsEnabled(port, tool);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSelectedTempLevelsEnabled(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return enabled;
		}

		/// <summary>
		/// Sets the tool temperature level of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level">The desired new selected temperature level</param>
		/// <remarks></remarks>
		public void SetPortToolSelectedTempLevels(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolSelectedTempLevels(port, tool, level);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolSelectedTempLevels(port, tool, level);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Activates or deactivates the tool temperature levels of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="onoff">On or Off value</param>
		/// <remarks></remarks>
		public void SetPortToolSelectedTempLevelsEnabled(string stationUUID, Port port, GenericStationTools tool, OnOff onoff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolSelectedTempLevelsEnabled(port, tool, onoff);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolSelectedTempLevelsEnabled(port, tool, onoff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool temperature level of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level">The identifier of the desired level</param>
		/// <returns>A Ctemperature object with the temperature of the level</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolTempLevel(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolTempLevel(port, tool, level);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolTempLevel(port, tool, level);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the tool temperature level of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level ">The identifier of the level</param>
		/// <param name="newTemp">The desired new temp for the level</param>
		/// <remarks></remarks>
		public void SetPortToolTempLevel(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolTempLevel(port, tool, level, newTemp);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolTempLevel(port, tool, level, newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool flow level of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level">The identifier of the desired level</param>
		/// <returns>An integer value with the flow of the level</returns>
		/// <remarks></remarks>
		public int GetPortToolFlowLevel(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			//#PG#
			int flow = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					flow = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolFlowLevel(port, tool, level));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return flow;
		}

		/// <summary>
		/// Sets the tool flow level of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level ">The identifier of the level</param>
		/// <param name="newFlow">The desired new flow for the level</param>
		/// <remarks></remarks>
		public void SetPortToolFlowLevel(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level, int newFlow)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolFlowLevel(port, tool, level, newFlow);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the temperature level of the external TC of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level">The identifier of the desired level</param>
		/// <returns>A Ctemperature object with the temperature of the level</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolExternalTempLevel(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolExternalTempLevel(port, tool, level);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the tool temperature level of the external TC of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level ">The identifier of the level</param>
		/// <param name="newTemp">The desired new temp for the level</param>
		/// <remarks></remarks>
		public void SetPortToolExternalTempLevel(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolExternalTempLevel(port, tool, level, newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the work mode of the external TC of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>The external TC mode</returns>
		/// <remarks></remarks>
		public ToolExternalTCMode_HA GetPortToolExternalTCMode(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			ToolExternalTCMode_HA mode = default(ToolExternalTCMode_HA);

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					mode = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolExternalTCMode(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return mode;
		}

		/// <summary>
		/// Sets the work mode of the external TC of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="mode">The desired external TC mode</param>
		/// <remarks></remarks>
		public void SetPortToolExternalTCMode(string stationUUID, Port port, GenericStationTools tool, ToolExternalTCMode_HA mode)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolExternalTCMode(port, tool, mode);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the status of the level of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level">The identifier of the desired level</param>
		/// <returns>A OnOff object with status of the temperature level</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolTempLevelEnabled(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level)
		{
			//#PG#
			OnOff enabled = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolTempLevelEnabled(port, tool, level);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolTempLevelEnabled(port, tool, level);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return enabled;
		}

		/// <summary>
		/// Activates or deactivates the tool level of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="level ">The identifier of the level</param>
		/// <param name="onoff">The desired status for the level</param>
		/// <remarks></remarks>
		public void SetPortToolTempLevelEnabled(string stationUUID, Port port, GenericStationTools tool, ToolTemperatureLevels level, OnOff onoff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolTempLevelEnabled(port, tool, level, onoff);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolTempLevelEnabled(port, tool, level, onoff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Sets the selected level and temperature levels of the indicated port of the indicated station, at once.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="selectedLevelEnabled">Activates or deactivates levels</param>
		/// <param name="selectedLevel">Selected temperature level</param>
		/// <param name="level1Enabled">Activates or deactivates level 1</param>
		/// <param name="tempLevel1">Temperature for level 1</param>
		/// <param name="level2Enabled">Activates or deactivates level 2</param>
		/// <param name="tempLevel2">Temperature for level 2</param>
		/// <param name="level3Enabled">Activates or deactivates level 3</param>
		/// <param name="tempLevel3">Temperature for level 3</param>
		/// <remarks></remarks>
		public void SetPortToolLevels_SOLD(string stationUUID, Port port, GenericStationTools tool, OnOff selectedLevelEnabled, ToolTemperatureLevels selectedLevel, OnOff level1Enabled, CTemperature tempLevel1, OnOff level2Enabled, CTemperature tempLevel2, OnOff level3Enabled, CTemperature tempLevel3)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolLevels(port, tool, 
					selectedLevelEnabled, selectedLevel, 
					level1Enabled, tempLevel1, 
					level2Enabled, tempLevel2, 
					level3Enabled, tempLevel3);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Sets the selected level and temperatures/flow levels of the indicated port of the indicated station, at once.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="selectedLevelEnabled">Activates or deactivates levels</param>
		/// <param name="selectedLevel">Selected temperature level</param>
		/// <param name="level1Enabled">Activates or deactivates level 1</param>
		/// <param name="tempLevel1">Temperature for level 1</param>
		/// <param name="flowLevel1">Flow for level 1</param>
		/// <param name="tempExternalLevel1">Temperature for external TC level 1</param>
		/// <param name="level2Enabled">Activates or deactivates level 2</param>
		/// <param name="tempLevel2">Temperature for level 2</param>
		/// <param name="flowLevel2">Flow for level 2</param>
		/// <param name="tempExternalLevel2">Temperature for external TC level 2</param>
		/// <param name="level3Enabled">Activates or deactivates level 3</param>
		/// <param name="tempLevel3">Temperature for level 3</param>
		/// <param name="flowLevel3">Flow for level 3</param>
		/// <param name="tempExternalLevel3">Temperature for external TC level 3</param>
		/// <remarks></remarks>
		public void SetPortToolLevels_HA(string stationUUID, Port port, GenericStationTools tool, OnOff selectedLevelEnabled, ToolTemperatureLevels selectedLevel, OnOff level1Enabled, CTemperature tempLevel1, int flowLevel1, CTemperature tempExternalLevel1, OnOff level2Enabled, CTemperature tempLevel2, int flowLevel2, CTemperature tempExternalLevel2, OnOff level3Enabled, CTemperature tempLevel3, int flowLevel3, CTemperature tempExternalLevel3)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolLevels(port, tool, 
					selectedLevelEnabled, selectedLevel, 
					level1Enabled, tempLevel1, flowLevel1, tempExternalLevel1, 
					level2Enabled, tempLevel2, flowLevel2, tempExternalLevel2, 
					level3Enabled, tempLevel3, flowLevel3, tempExternalLevel3);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool sleep delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A ToolTimeSleep with the current sleep delay</returns>
		/// <remarks></remarks>
		public ToolTimeSleep GetPortToolSleepDelay(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			ToolTimeSleep timeSleep = ToolTimeSleep.NO_SLEEP;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					timeSleep = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepDelay(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return timeSleep;
		}

		/// <summary>
		/// Gets the status of the tool sleep delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A OnOff with the current status of the sleep delay</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolSleepDelayEnabled(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			OnOff enabled = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepDelayEnabled(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return enabled;
		}

		/// <summary>
		/// Sets the tool sleep delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="delay">The desired delay for the tool</param>
		/// <remarks></remarks>
		public void SetPortToolSleepDelay(string stationUUID, Port port, GenericStationTools tool, ToolTimeSleep delay)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolSleepDelay(port, tool, delay);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Activates or deactivates the tool sleep delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="onoff">Activates or deactivates the sleep delay for the tool</param>
		/// <remarks></remarks>
		public void SetPortToolSleepDelayEnabled(string stationUUID, Port port, GenericStationTools tool, OnOff onoff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolSleepDelayEnabled(port, tool, onoff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool sleep temperature of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A Ctemperature object with the sleep temperature</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolSleepTemp(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepTemp(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the tool sleep temperature of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="newTemp">The desired temperature for the sleep status</param>
		/// <remarks></remarks>
		public void SetPortToolSleepTemp(string stationUUID, Port port, GenericStationTools tool, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolSleepTemp(port, tool, newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool hibernation delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A ToolTimeHibernation with the current hibernation delay</returns>
		/// <remarks></remarks>
		public ToolTimeHibernation GetPortToolHibernationDelay(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			ToolTimeHibernation hibernationDelay = ToolTimeHibernation.NO_HIBERNATION;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					hibernationDelay = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolHibernationDelay(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return hibernationDelay;
		}

		/// <summary>
		/// Gets status of the tool hibernation delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A OnOff with the current stauts of the hibernation delay</returns>
		/// <remarks></remarks>
		public OnOff GetPortToolHibernationDelayEnabled(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			OnOff enabled = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					enabled = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolHibernationDelayEnabled(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return enabled;
		}

		/// <summary>
		/// Sets the tool hibernation delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="delay">The desired delay for the tool</param>
		/// <remarks></remarks>
		public void SetPortToolHibernationDelay(string stationUUID, Port port, GenericStationTools tool, ToolTimeHibernation delay)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolHibernationDelay(port, tool, delay);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Activates or deactivates the tool hibernation delay of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="onoff">Activates or deactivates the hibernation delay for the tool</param>
		/// <remarks></remarks>
		public void SetPortToolHibernationDelayEnabled(string stationUUID, Port port, GenericStationTools tool, OnOff onoff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolHibernationDelayEnabled(port, tool, onoff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool adjust temperature of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A Ctemperature object with the adjust temperature</returns>
		/// <remarks></remarks>
		public CTemperature GetPortToolAdjustTemp(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolAdjustTemp(port, tool);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolAdjustTemp(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the tool adjust temperature of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="newTemp">The desired temperature for the adjust temperature</param>
		/// <remarks></remarks>
		public void SetPortToolAdjustTemp(string stationUUID, Port port, GenericStationTools tool, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolAdjustTemp(port, tool, newTemp);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolAdjustTemp(port, tool, newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the tool time to stop (seconds) of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A value with the time to stop in seconds</returns>
		/// <remarks></remarks>
		public int GetPortToolTimeToStop(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			int ret = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					ret = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolTimeToStop(port, tool));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return ret;
		}

		/// <summary>
		/// Sets the tool time to stop of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="newValue">The desired time to stop</param>
		/// <remarks></remarks>
		public void SetPortToolTimeToStop(string stationUUID, Port port, GenericStationTools tool, int newValue)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolTimeToStop(port, tool, newValue);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the start mode of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>A value indicating the current start mode</returns>
		/// <remarks></remarks>
		public CToolStartMode_HA GetPortToolStartMode(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			CToolStartMode_HA value = new CToolStartMode_HA();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					value = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolStartMode(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return value;
		}

		/// <summary>
		/// Sets the the start mode of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="mode">The desired temperature for the sleep status</param>
		/// <remarks></remarks>
		public void SetPortToolStartMode(string stationUUID, Port port, GenericStationTools tool, CToolStartMode_HA mode)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetPortToolStartMode(port, tool, mode);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the cartridge used in the tool for the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <returns>Cartridge data</returns>
		/// <remarks></remarks>
		public CCartridgeData GetPortToolCartridge(string stationUUID, Port port, GenericStationTools tool)
		{
			CCartridgeData cartridge = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					cartridge = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolCartridge(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return cartridge;
		}

		/// <summary>
		/// Sets the cartridge used in the tool for the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <param name="tool">The identifier of the desired tool</param>
		/// <param name="cartridge">Cartridge data</param>
		/// <remarks></remarks>
		public void SetPortToolCartridge(string stationUUID, Port port, GenericStationTools tool, CCartridgeData cartridge)
		{

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPortToolCartridge(port, tool, cartridge);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// This method resets setting data of the indicated port and tool of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier or the port identifier are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port and tool</param>
		/// <remarks></remarks>
		public void ResetPortToolSettings(string stationUUID, Port port, GenericStationTools tool)
		{
			//#PG#
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.ResetPortToolSettings(port, tool);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

#endregion


#region API Control mode

		/// <summary>
		/// Gets the current control mode status of the indicated station.
		/// If the station identifier is not correct an error event is thrown
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A OnOff object that indicates the current status</returns>
		/// <remarks></remarks>
		public ControlModeConnection GetControlMode(string stationUUID)
		{
			//#PG#
			ControlModeConnection controlMode = ControlModeConnection.MONITOR;

			m_mutexStationsList.WaitOne();
			stationUUID = LookForUpperParentStation(stationUUID);

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					// Check mode Robot
					if (((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetRobotStatus() == OnOff._ON)
					{
						controlMode = ControlModeConnection.ROBOT;

						// Return the stored value if control
					}
					else
					{
						controlMode = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetControlMode();
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					// Check mode Robot
					if (((CStationElement) (StationsList[stationUUID])).Station_HA.GetRobotStatus() == OnOff._ON)
					{
						controlMode = ControlModeConnection.ROBOT;

						// Return the stored value if control
					}
					else
					{
						controlMode = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetControlMode();
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					// Check mode Robot
					if (((CStationElement) (StationsList[stationUUID])).Station_SF.GetRobotStatus() == OnOff._ON)
					{
						controlMode = ControlModeConnection.ROBOT;

						// Return the stored value if control
					}
					else
					{
						controlMode = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetControlMode();
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					// Check mode Robot
					if (((CStationElement) (StationsList[stationUUID])).Station_FE.GetRobotStatus() == OnOff._ON)
					{
						controlMode = ControlModeConnection.ROBOT;

						// Return the stored value if control
					}
					else
					{
						controlMode = ((CStationElement) (StationsList[stationUUID])).Station_FE.GetControlMode();
					}
				}

			}
			m_mutexStationsList.ReleaseMutex();

			return controlMode;
		}

		/// <summary>
		/// This method activates (CONTROL) or deactivates (MONITOR) the indicated station control mode.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="mode">The desired state for the control mode</param>
		/// <remarks></remarks>
		public void SetControlMode(string stationUUID, ControlModeConnection mode)
		{
			//#PG#

			m_mutexStationsList.WaitOne();
			stationUUID = LookForUpperParentStation(stationUUID);

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				//Get all the stations sharing com channel
				List<string> listStationUUID = m_StationAvailableCom.GetListStationSameComChannel(stationUUID);

				foreach (string elementStationUUID in listStationUUID)
				{
					if (((CStationElement) (StationsList[elementStationUUID])).StationType == eStationType.SOLD)
					{

						// Si esta en modo robot no puedes tomar el control
						if (((CStationElement) (StationsList[elementStationUUID])).Station_SOLD.GetRobotStatus() != OnOff._ON)
						{
							((CStationElement) (StationsList[elementStationUUID])).Station_SOLD.SetControlMode(mode);
						}
					}
					else if (((CStationElement) (StationsList[elementStationUUID])).StationType == eStationType.HA)
					{

						// Si esta en modo robot no puedes tomar el control
						if (((CStationElement) (StationsList[elementStationUUID])).Station_HA.GetRobotStatus() != OnOff._ON)
						{
							((CStationElement) (StationsList[elementStationUUID])).Station_HA.SetControlMode(mode);
						}
					}
					else if (((CStationElement) (StationsList[elementStationUUID])).StationType == eStationType.SF)
					{

						// Si esta en modo robot no puedes tomar el control
						if (((CStationElement) (StationsList[elementStationUUID])).Station_SF.GetRobotStatus() != OnOff._ON)
						{
							((CStationElement) (StationsList[elementStationUUID])).Station_SF.SetControlMode(mode);
						}
					}
					else if (((CStationElement) (StationsList[elementStationUUID])).StationType == eStationType.FE)
					{

						// Si esta en modo robot no puedes tomar el control
						if (((CStationElement) (StationsList[elementStationUUID])).Station_FE.GetRobotStatus() != OnOff._ON)
						{
							((CStationElement) (StationsList[elementStationUUID])).Station_FE.SetControlMode(mode);
						}
					}
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the current remote mode status of the indicated station.
		/// If the station identifier is not correct an error event is thrown
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A OnOff object that indicates the current status</returns>
		/// <remarks></remarks>
		public OnOff GetRemoteMode(string stationUUID)
		{
			//#PG#
			OnOff OnOffRemoteMode = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					OnOffRemoteMode = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetRemoteMode();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					OnOffRemoteMode = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetRemoteMode();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return OnOffRemoteMode;
		}

		/// <summary>
		/// This method sets to ON or OFF the indicated station remote mode.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="OnOff">The desired state for the remote mode</param>
		/// <remarks></remarks>
		public void SetRemoteMode(string stationUUID, OnOff OnOff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				//Get all the stations sharing com channel
				List<string> listStationUUID = m_StationAvailableCom.GetListStationSameComChannel(stationUUID);

				foreach (string elementStationUUID in listStationUUID)
				{
					if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
					{
						((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetRemoteMode(OnOff);
					}
					else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
					{
						((CStationElement) (StationsList[stationUUID])).Station_HA.SetRemoteMode(OnOff);
					}
					else
					{
						if (UserErrorEvent != null)
							UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
					}
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

#endregion


#region API Continuous mode

		/// <summary>
		/// This method starts a new continuous data queue instance on the indicated station and returns an ID.
		/// The station transmision speed and ports will be the ones defined in SetContinuousMode Method.
		/// The desired delivery speed for this queue is defined in captureSpeed perameter
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="deliverySpeed">The desired speed for this queue</param>
		/// <returns>A new queue id of the continuous mode to be used when retrieving the data</returns>
		/// <remarks></remarks>
		public uint StartContinuousMode(string stationUUID, SpeedContinuousMode deliverySpeed = default(SpeedContinuousMode))
		{
			//#PG#
			uint queueID = UInt32.MaxValue;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					queueID = System.Convert.ToUInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.StartContinuousMode(deliverySpeed));
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					queueID = System.Convert.ToUInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.StartContinuousMode(deliverySpeed));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return queueID;
		}

		/// <summary>
		/// Stops and clear the current continuous mode data transmisions of the indicated queue.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="queueID">The queue ID returned by StartContinuousMode</param>
		/// <remarks></remarks>
		public void StopContinuousMode(string stationUUID, uint queueID)
		{
			//#PG#

			m_mutexStationsList.WaitOne();
			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.StopContinuousMode(queueID);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.StopContinuousMode(queueID);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the current continuous data transmision mode status of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A CContinuousModeStatus object with the current status</returns>
		/// <remarks></remarks>
		public CContinuousModeStatus GetContinuousMode(string stationUUID)
		{
			//#PG#
			CContinuousModeStatus continuousModeStatus = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					continuousModeStatus = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetContinuousMode();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					continuousModeStatus = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetContinuousMode();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return continuousModeStatus;
		}

		/// <summary>
		/// This method defines the speed and ports to be used in continuous data transmision mode of the indicated station.
		/// The desired transmision speed ( period ) and at least one port must be indicated when defining.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="speed">The desired speed ( period ) for the transmisions</param>
		/// <param name="portA">First desired port to be monitorized</param>
		/// <param name="portB">Second desired port to be monitorized</param>
		/// <param name="portC">Third desired port to be monitorized</param>
		/// <param name="portD">Fourth desired port to be monitorized</param>
		/// <remarks></remarks>
		public void SetContinuousMode(string stationUUID, SpeedContinuousMode speed, Port portA = default(Port), Port portB = default(Port), Port portC = default(Port), Port portD = default(Port))
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				CContinuousModeStatus statusContMode = new CContinuousModeStatus();
				statusContMode.speed = speed;
				statusContMode.setPortsFromEnum(portA, portB, portC, portD);

				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetContinuousMode(statusContMode);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetContinuousMode(statusContMode);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the current continuous mode delivery speed of the queue of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="queueID">The queue ID returned by StartContinuousMode</param>
		/// <returns>The delivery speed of continuous data</returns>
		/// <remarks></remarks>
		public SpeedContinuousMode GetContinuousModeDeliverySpeed(string stationUUID, uint queueID)
		{
			//#PG#
			SpeedContinuousMode ret = SpeedContinuousMode.OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					ret = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetContinuousModeCaptureSpeed(queueID);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					ret = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetContinuousModeCaptureSpeed(queueID);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return ret;
		}

		/// <summary>
		/// Gets the current continuous mode data transmisions pending to be got from the internal FIFO queue of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="queueID">The queue ID returned by StartContinuousMode</param>
		/// <returns>An integer that is the queue length</returns>
		/// <remarks></remarks>
		public int GetContinuousModeDataCount(string stationUUID, uint queueID)
		{
			//#PG#
			int dataCount = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					dataCount = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetContinuousModeDataCount(queueID));
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					dataCount = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetContinuousModeDataCount(queueID));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return dataCount;
		}

		/// <summary>
		/// Gets the next continuous mode data transmison in the internal FIFO queue from the station. It is the oldest transmision.
		/// Soldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="queueID">The queue ID returned by StartContinuousMode</param>
		/// <returns>A tContinuousModeData object that is the oldest transmision in the queue</returns>
		/// <remarks></remarks>
		public stContinuousModeData_SOLD GetContinuousModeNextData_SOLD(string stationUUID, uint queueID)
		{
			//#PG#
			stContinuousModeData_SOLD nextData = new stContinuousModeData_SOLD();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					nextData = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetContinuousModeNextData(queueID);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return nextData;
		}

		/// <summary>
		/// Gets the next continuous mode data transmison in the internal FIFO queue from the station. It is the oldest transmision.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="queueID">The queue ID returned by StartContinuousMode</param>
		/// <returns>A tContinuousModeData object that is the oldest transmision in the queue</returns>
		/// <remarks></remarks>
		public stContinuousModeData_HA GetContinuousModeNextData_HA(string stationUUID, uint queueID)
		{
			//#PG#
			stContinuousModeData_HA nextData = new stContinuousModeData_HA();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
                if (((CStationElement)(StationsList[stationUUID])).StationType == eStationType.HA)
                {
                    nextData = ((CStationElement)(StationsList[stationUUID])).Station_HA.GetContinuousModeNextData(queueID);
                }
                else
                {
                    if (UserErrorEvent != null)
                        UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
                }
			}
			m_mutexStationsList.ReleaseMutex();

			return nextData;
		}

#endregion


#region API Station methods

		public string GetStationParentUUID(string stationUUID)
		{
			string parentUUID = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					parentUUID = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationParentUUID());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					parentUUID = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationParentUUID());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					parentUUID = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationParentUUID());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					parentUUID = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationParentUUID());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return parentUUID;
		}

		public string GetUpperStationParentUUID(string stationUUID)
		{
			return LookForUpperParentStation(stationUUID);
		}

		/// <summary>
		/// Gets the station type of the indicated station (soldering station, hot air desoldering station).
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A value containing the station type.</returns>
		/// <remarks></remarks>
		public eStationType GetStationType(string stationUUID)
		{
			//#PG#
			eStationType stnType = eStationType.UNKNOWN;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				stnType = ((CStationElement) (StationsList[stationUUID])).StationType;
			}
			m_mutexStationsList.ReleaseMutex();

			return stnType;
		}

		/// <summary>
		/// This method restores the default values for all the station parameters.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <remarks></remarks>
		public void DefaultStationParameters(string stationUUID)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetDefaultStationParams();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetDefaultStationParams();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetDefaultStationParams();
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the protocol version of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the protocol version.</returns>
		/// <remarks></remarks>
		public string GetStationProtocol(string stationUUID)
		{
			//#PG#
			string stationProtocol = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationProtocol = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationProtocol());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationProtocol = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationProtocol());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationProtocol = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationProtocol());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationProtocol = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationProtocol());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationProtocol;
		}

		/// <summary>
		/// Gets the Model name of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the Model name.</returns>
		/// <remarks></remarks>
		public string GetStationModel(string stationUUID)
		{
			//#PG#
			string stationModel = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationModel = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationModel());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationModel = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationModel());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationModel = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationModel());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationModel = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationModel());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationModel;
		}

		/// <summary>
		/// Gets the Model type of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the Model type.</returns>
		/// <remarks></remarks>
		public string GetStationModelType(string stationUUID)
		{
			//#PG#
			string stationModelType = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationModelType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationModelType());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationModelType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationModelType());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationModelType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationModelType());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationModelType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationModelType());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationModelType;
		}

		/// <summary>
		/// Gets the version of the Model type of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the Model type.</returns>
		/// <remarks></remarks>
		public int GetStationModelVersion(string stationUUID)
		{
			//#PG#
			int stationModelVersion = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationModelVersion = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationModelVersion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationModelVersion = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationModelVersion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationModelVersion = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationModelVersion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationModelVersion = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationModelVersion());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationModelVersion;
		}

		/// <summary>
		/// Gets the Hardware version of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the Hardware version.</returns>
		/// <remarks></remarks>
		public string GetStationHWversion(string stationUUID)
		{
			//#PG#
			string stationHWVersion = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationHWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationHWversion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationHWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationHWversion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationHWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationHWversion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationHWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationHWversion());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationHWVersion;
		}

		/// <summary>
		/// Gets the Software version of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the Software version.</returns>
		/// <remarks></remarks>
		public string GetStationSWversion(string stationUUID)
		{
			//#PG#
			string stationSWVersion = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationSWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationSWversion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationSWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationSWversion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationSWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationSWversion());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationSWVersion = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationSWversion());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationSWVersion;
		}

		/// <summary>
		/// Gets the current error code of the indicated station. If the station identifier
		/// is not correct an error event is thrown. Uses the cStationError constants defined in this
		/// class.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A cStationError object which is the error code.</returns>
		/// <remarks></remarks>
		public StationError GetStationError(string stationUUID)
		{
			//#PG#
			StationError sError = StationError.NO_ERROR;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					sError = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationError();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					sError = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationError();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					sError = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationError();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					sError = ((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationError();
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return sError;
		}

		/// <summary>
		/// Gets the current transformer temperature of the indicated station.
		/// Soldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// Uses the Ctemperature class defined in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Ctemperature object with the current transformer temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetStationTransformerTemp(string stationUUID)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationTransformerTemp();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}

			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Gets the current transformer error temperature of the indicated station.
		/// Soldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// Uses the Ctemperature class defined in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Ctemperature object with the current transformer temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetStationTransformerErrorTemp(string stationUUID)
		{
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationTransformerErrorTemp();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Gets the current MOSFET error temperature of the indicated station.
		/// Soldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// Uses the Ctemperature class defined in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Ctemperature object with the current transformer temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetStationMOSerrorTemp(string stationUUID)
		{
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationMOSerrorTemp();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Gets the list of supported tools of the indicated station.
		/// If the station identifier is not correct an error event is thrown. Uses the GenericStationTools constants
		/// defined in this class.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A GenericStationTools vector with the supported tools.</returns>
		/// <remarks></remarks>
		public GenericStationTools[] GetStationTools(string stationUUID)
		{
			//#PG#
			GenericStationTools[] genericStationTools = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					genericStationTools = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationTools();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					genericStationTools = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationTools();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					genericStationTools = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationTools();
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return genericStationTools;
		}

		/// <summary>
		/// Gets the number of ports of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>An integer which is the number of ports</returns>
		/// <remarks></remarks>
		public int GetPortCount(string stationUUID)
		{
			//#PG#
			int portCount = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					portCount = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.NumPorts;
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					portCount = ((CStationElement) (StationsList[stationUUID])).Station_HA.NumPorts;
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					portCount = ((CStationElement) (StationsList[stationUUID])).Station_SF.NumPorts;
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					portCount = ((CStationElement) (StationsList[stationUUID])).Station_FE.NumPorts;
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return portCount;
		}

		/// <summary>
		/// Gets the name of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the station name.</returns>
		/// <remarks></remarks>
		public string GetStationName(string stationUUID)
		{
			//#PG#
			string stationName = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationName = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationName());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationName = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationName());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationName = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationName());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationName = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationName());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationName;
		}

		/// <summary>
		/// Sets the indicated name to the indicated station. If the station identifier is incorrect or
		/// the indicated name is an empty string an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newName">The desired station name</param>
		/// <remarks></remarks>
		public void SetStationName(string stationUUID, string newName)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else if (newName.Length == 0)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.INVALID_STATION_NAME, "Invalid name."));
			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationName(newName);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationName(newName);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationName(newName);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					((CStationElement) (StationsList[stationUUID])).Station_FE.SetStationName(newName);
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the PIN of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the station PIN.</returns>
		/// <remarks></remarks>
		public string GetStationPIN(string stationUUID)
		{
			//#PG#
			string PIN = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					PIN = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationPIN());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					PIN = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationPIN());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					PIN = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationPIN());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return PIN;
		}

		/// <summary>
		/// Sets the indicated PIN to the indicated station. If the station identifier is incorrect or
		/// the indicated PIN is an empty string an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newPIN">The desired new PIN</param>
		/// <remarks></remarks>
		public void SetStationPIN(string stationUUID, string newPIN)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else if (newPIN.Length != 4)
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.INVALID_STATION_PIN, "Invalid PIN."));
			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationPIN(newPIN);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationPIN(newPIN);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationPIN(newPIN);
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the status of the PIN of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>The PIN Enabled status.</returns>
		/// <remarks></remarks>
		public OnOff GetStationPINEnabled(string stationUUID)
		{
			//#PG#
			OnOff PINenabled = OnOff._ON;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					PINenabled = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationPINEnabled();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					PINenabled = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationPINEnabled();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return PINenabled;
		}

		/// <summary>
		/// Activates or deactivates the PIN in the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is incorrect an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="onoff">The desired status</param>
		/// <remarks></remarks>
		public void SetStationPINEnabled(string stationUUID, OnOff onoff)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationPINEnabled(onoff);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationPINEnabled(onoff);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		// max/min temp

		/// <summary>
		/// Gets the maximum temperature of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// It uses the Ctemperature class defined in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Ctemperature object with the maximum temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetStationMaxTemp(string stationUUID)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationMaxTemp();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationMaxTemp();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the indicated maximum temperature to the indicated station.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newtemp">The desired new temperature</param>
		/// <remarks></remarks>
		public void SetStationMaxTemp(string stationUUID, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationMaxTemp(newTemp);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationMaxTemp(newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the minimum temperature of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// Uses the Ctemperature class defined in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Ctemperature object with the minimum temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetStationMinTemp(string stationUUID)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationMinTemp();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationMinTemp();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the indicated minimum temperature to the indicated station.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newtemp">The desired new temperature</param>
		/// <remarks></remarks>
		public void SetStationMinTemp(string stationUUID, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationMinTemp(newTemp);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationMinTemp(newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		// max/min external TC temp

		/// <summary>
		/// Gets the maximum external temperature of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// It uses the Ctemperature class defined in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Ctemperature object with the maximum external temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetStationMaxExternalTemp(string stationUUID)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationMaxExtTemp();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the indicated maximum external temperature to the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newtemp">The desired new external temperature</param>
		/// <remarks></remarks>
		public void SetStationMaxExternalTemp(string stationUUID, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationMaxExtTemp(newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the minimum external temperature of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// Uses the Ctemperature class defined in this library.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Ctemperature object with the minimum external temperature.</returns>
		/// <remarks></remarks>
		public CTemperature GetStationMinExternalTemp(string stationUUID)
		{
			//#PG#
			CTemperature temp = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					temp = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationMinExtTemp();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return temp;
		}

		/// <summary>
		/// Sets the indicated minimum external temperature to the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newtemp">The desired new external temperature</param>
		/// <remarks></remarks>
		public void SetStationMinExternalTemp(string stationUUID, CTemperature newTemp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationMinExtTemp(newTemp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		// max/min flow

		/// <summary>
		/// Gets the maximum flow of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A value with the maximum flow.</returns>
		/// <remarks></remarks>
		public int GetStationMaxFlow(string stationUUID)
		{
			//#PG#
			int flow = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					flow = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationMaxFlow());
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return flow;
		}

		/// <summary>
		/// Sets the indicated maximum flow to the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newFlow">The desired new flow</param>
		/// <remarks></remarks>
		public void SetStationMaxFlow(string stationUUID, int newFlow)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationMaxFlow(newFlow);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the minimum flow of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A value with the minimum flow.</returns>
		/// <remarks></remarks>
		public int GetStationMinFlow(string stationUUID)
		{
			//#PG#
			int flow = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					flow = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationMinFlow());
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return flow;
		}

		/// <summary>
		/// Sets the indicated minimum flow to the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newFlow">The desired new flow</param>
		/// <remarks></remarks>
		public void SetStationMinFlow(string stationUUID, int newFlow)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationMinFlow(newFlow);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}


		/// <summary>
		/// Gets the temperature units of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A TemperatureUnits object with the temperature units.</returns>
		/// <remarks></remarks>
		public CTemperature.TemperatureUnit GetStationTempUnits(string stationUUID)
		{
			//#PG#
			CTemperature.TemperatureUnit tempUnit = CTemperature.TemperatureUnit.Celsius;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					tempUnit = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationTempUnits();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					tempUnit = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationTempUnits();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return tempUnit;
		}

		/// <summary>
		/// Sets the indicated temperature units to the indicated station. If the station identifier is incorrect or
		/// the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newUnits">The desired temperature units.</param>
		/// <remarks></remarks>
		public void SetStationTempUnits(string stationUUID, CTemperature.TemperatureUnit newUnits)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationTempUnits(newUnits);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationTempUnits(newUnits);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public CLength.LengthUnit GetStationLengthUnits(string stationUUID)
		{
			CLength.LengthUnit lengthUnit = CLength.LengthUnit.Inches;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					lengthUnit = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationLengthUnits();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return lengthUnit;
		}

		public void SetStationLengthUnits(string stationUUID, CLength.LengthUnit newUnits)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationLengthUnits(newUnits);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the N2 mode status of the indicated station.
		/// Soldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A OnOff object with the current N2 mode status</returns>
		/// <remarks></remarks>
		public OnOff GetStationN2Mode(string stationUUID)
		{
			//#PG#
			OnOff N2Mode = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					N2Mode = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationN2Mode();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return N2Mode;
		}

		/// <summary>
		/// Sets the indicated N2 mode status to the indicated station.
		/// Soldering stations only.
		/// If the station identifier is incorrect or
		/// the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newMode">The desired status for the N2 mode</param>
		/// <remarks></remarks>
		public void SetStationN2Mode(string stationUUID, OnOff newMode)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationN2Mode(newMode);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the Help Text status of the indicated station.
		/// Soldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A OnOff object with the current Help Text status</returns>
		/// <remarks></remarks>
		public OnOff GetStationHelpText(string stationUUID)
		{
			//#PG#
			OnOff helpText = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					helpText = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationHelpText();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return helpText;
		}

		/// <summary>
		/// Sets the indicated Help Text status to the indicated station.
		/// Soldering stations only.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="newHelp ">The desired status for the Help Text</param>
		/// <remarks></remarks>
		public void SetStationHelpText(string stationUUID, OnOff newHelp)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationHelpText(newHelp);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the power limit of the indicated station.
		/// Soldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>An integer object with the power linit in Watts units</returns>
		/// <remarks></remarks>
		public int GetStationPowerLimit(string stationUUID)
		{
			int powerLimit = -1;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					powerLimit = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationPowerLimit());
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return powerLimit;
		}

		/// <summary>
		/// Sets the power limit to the indicated station.
		/// Soldering stations only.
		/// If the station identifier is incorrect or the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="powerLimit">The desired power limit in Watts units.</param>
		/// <remarks></remarks>
		public void SetStationPowerLimit(string stationUUID, int powerLimit)
		{

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationPowerLimit(powerLimit);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the Beep status of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A OnOff object with the current Beep status</returns>
		/// <remarks></remarks>
		public OnOff GetStationBeep(string stationUUID)
		{
			//#PG#
			OnOff beep = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					beep = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationBeep();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					beep = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationBeep();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					beep = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationBeep();
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return beep;
		}

		/// <summary>
		/// Sets the indicated Beep status to the indicated station. If the station identifier is incorrect or
		/// the indicated temperature is out of range an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="beep ">The desired status for the Beep mode</param>
		/// <remarks></remarks>
		public void SetStationBeep(string stationUUID, OnOff beep)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationBeep(beep);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationBeep(beep);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationBeep(beep);
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Gets the Station Locked status of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A OnOff object with the current locked status</returns>
		/// <remarks></remarks>
		public OnOff GetStationLocked(string stationUUID)
		{
			//#PG#
			OnOff locked = OnOff._OFF;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					locked = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationLocked();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					locked = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationLocked();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					locked = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationLocked();
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return locked;
		}

		/// <summary>
		/// Sets the Station Locked status to the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station identifier is incorrect, an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="locked ">The desired status for the station locked status</param>
		/// <remarks></remarks>
		public void SetStationLocked(string stationUUID, OnOff locked)
		{
			//#PG#

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetStationLocked(locked);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetStationLocked(locked);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationLocked(locked);
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public byte GetStationSelectedProgram(string stationUUID)
		{
			byte program = (byte) 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					program = System.Convert.ToByte(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationSelectedProgram());
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return program;
		}

		public byte[] GetStationConcatenateProgramList(string stationUUID)
		{
			byte[] programList = new byte[0];

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					programList = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationConcatenateProgramList();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return programList;
		}

		public void SetStationConcatenateProgramList(string stationUUID, byte[] programList)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationConcatenateProgramList(programList);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public CProgramDispenserData_SF GetStationProgram(string stationUUID, byte nbrProgram)
		{
			CProgramDispenserData_SF program = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					program = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationProgram(nbrProgram);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return program;
		}
		
		public void SetStationProgram(string stationUUID, byte nbrProgram, CProgramDispenserData_SF program)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetStationProgram(nbrProgram, program);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public void DeleteStationProgram(string stationUUID, byte nbrProgram)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.DeleteStationProgram(nbrProgram);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

#endregion


#region API Counters

		/// <summary>
		/// Gets the number of minutes connected of the indicated station.
		/// If the station identifier is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>An integer object with the number of minutes the station is connected</returns>
		/// <remarks></remarks>
		public int GetStationPluggedMinutes(string stationUUID, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationPluggedMinutes());
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationPluggedMinutesPartial());
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationPluggedMinutes());
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationPluggedMinutesPartial());
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationPluggedMinutes());
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationPluggedMinutesPartial());
					}
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the number of minutes connected of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer object with the number of minutes the station is connected</returns>
		/// <remarks></remarks>
		public int GetPortToolPluggedMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolPluggedMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolPluggedMinutesPartial(port));
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolPluggedMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolPluggedMinutesPartial(port));
					}
				}
				else
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolPluggedMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolPluggedMinutesPartial(port));
					}
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the tool working time in minutes of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the working time in minutes</returns>
		/// <remarks></remarks>
		public int GetPortToolWorkMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolWorkMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolWorkMinutesPartial(port));
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolWorkMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolWorkMinutesPartial(port));
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolWorkMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolWorkMinutesPartial(port));
					}
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the tool sleep time in minutes of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the sleep time in minutes</returns>
		/// <remarks></remarks>
		public int GetPortToolSleepMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepMinutesPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the tool hibernation time in minutes of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the hibernation time in minutes</returns>
		/// <remarks></remarks>
		public int GetPortToolHibernationMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolHibernationMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolHibernationMinutesPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the tool idle (no tool) time in minutes of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the idle (no tool) time in minutes</returns>
		/// <remarks></remarks>
		public int GetPortToolIdleMinutes(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolIdleMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolIdleMinutesPartial(port));
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolIdleMinutes(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolIdleMinutesPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the tool number of sleep cycles of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the number of cycles</returns>
		/// <remarks></remarks>
		public int GetPortToolSleepCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepCycles(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolSleepCyclesPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the tool number of desolder cycles of the indicated port of the indicated station.
		/// Soldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the number of cycles</returns>
		/// <remarks></remarks>
		public int GetPortToolDesolderCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolDesolderCycles(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPortToolDesolderCyclesPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}


		/// <summary>
		/// Gets the tool number of work cycles of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the number of cycles</returns>
		/// <remarks></remarks>
		public int GetPortToolWorkCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolWorkCycles(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolWorkCyclesPartial(port));
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolWorkCycles(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolWorkCyclesPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		/// <summary>
		/// Gets the tool number of suction cycles of the indicated port of the indicated station.
		/// Hot Air Desoldering stations only.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <returns>An integer that is the number of cycles</returns>
		/// <remarks></remarks>
		public int GetPortToolSuctionCycles(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			//#PG#
			int counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSuctionCycles(port));
					}
					else
					{
						counter = System.Convert.ToInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.GetPortToolSuctionCyclesPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		public long GetPortToolTinLength(string stationUUID, Port port, CounterTypes counterType = default(CounterTypes))
		{
			long counter = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					if (counterType == CounterTypes.GLOBAL_COUNTER)
					{
						counter = System.Convert.ToInt64(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolTinLength(port));
					}
					else
					{
						counter = System.Convert.ToInt64(((CStationElement) (StationsList[stationUUID])).Station_SF.GetPortToolTinLengthPartial(port));
					}
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return counter;
		}

		private void ResetStationPartialCounters(string stationUUID)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					int numPorts = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.NumPorts;
					if (numPorts > 0)
					{
						((CStationElement) (StationsList[stationUUID])).Station_SOLD.ResetPortToolMinutesPartial(Port.NUM_1);
					}
					if (numPorts > 1)
					{
						((CStationElement) (StationsList[stationUUID])).Station_SOLD.ResetPortToolMinutesPartial(Port.NUM_2);
					}
					if (numPorts > 2)
					{
						((CStationElement) (StationsList[stationUUID])).Station_SOLD.ResetPortToolMinutesPartial(Port.NUM_3);
					}
					if (numPorts > 3)
					{
						((CStationElement) (StationsList[stationUUID])).Station_SOLD.ResetPortToolMinutesPartial(Port.NUM_4);
					}
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.ResetPortToolMinutesPartial(Port.NUM_1);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.ResetStationCountersPartial();
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		/// <summary>
		/// Resets all partial counters of the indicated port of the indicated station.
		/// If the station or port identifiers are not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="port">The identifier of the desired port</param>
		/// <remarks></remarks>
		public void ResetPortToolStationPartialCounters(string stationUUID, Port port)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.ResetPortToolMinutesPartial(port);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.ResetPortToolMinutesPartial(port);
				}
				else
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.ResetStationCountersPartial();
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

#endregion


#region API Communications

#region Ethernet

		/// <summary>
		/// Soldering stations only.
		/// </summary>
		/// <param name="stationUUID"></param>
		/// <returns>CEthernetData structure</returns>
		/// <remarks></remarks>
		public CEthernetData GetEthernetConfiguration(string stationUUID)
		{
			CEthernetData ethernetData = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					ethernetData = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetEthernetConfiguration();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return ethernetData;
		}

		/// <summary>
		/// Soldering stations only.
		/// </summary>
		/// <param name="stationUUID"></param>
		/// <param name="ethernetData"></param>
		/// <remarks></remarks>
		public void SetEthernetConfiguration(string stationUUID, CEthernetData ethernetData)
		{

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetEthernetConfiguration(ethernetData);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

#endregion


#region Robot

		public CRobotData GetRobotConfiguration(string stationUUID)
		{
			CRobotData robotData = null;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					robotData = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetRobotConfiguration();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					robotData = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetRobotConfiguration();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					robotData = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetRobotConfiguration();
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return robotData;
		}

		public void SetRobotConfiguration(string stationUUID, CRobotData robotData)
		{

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetRobotConfiguration(robotData);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SetRobotConfiguration(robotData);
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SF.SetRobotConfiguration(robotData);
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

#endregion

#endregion


#region API Peripherals

		/// <summary>
		/// Gets the peripheral configuration from the indicated station.
		/// Soldering stations only.
		/// </summary>
		/// <param name="stationUUID"></param>
		/// <returns></returns>
		/// <remarks></remarks>
		public List<CPeripheralData> GetPeripheralList(string stationUUID)
		{
			List<CPeripheralData> PeripheralList = new List<CPeripheralData>();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					PeripheralList = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetPeripheralList();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return PeripheralList;
		}

		/// <summary>
		/// Sets the indicated peripheral configuration to the indicated station.
		/// Soldering stations only.
		/// If the station identifier is incorrect, an error is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="peripheralData">The desired peripheral configuration</param>
		/// <remarks></remarks>
		public void SetPeripheralInfo(string stationUUID, CPeripheralData peripheralData)
		{

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetPeripheralConfiguration(peripheralData.ID, peripheralData);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

#endregion


#region API Profiles

		public List<CProfileData_HA> GetProfileList(string stationUUID)
		{
			List<CProfileData_HA> ProfileList = new List<CProfileData_HA>();

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					ProfileList = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetProfileList();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return ProfileList;
		}

		public string GetSelectedProfile(string stationUUID)
		{
			string selectedProfile = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					selectedProfile = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetSelectedProfile());
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return selectedProfile;
		}

		public bool SetProfile(string stationUUID, CProfileData_HA profile)
		{
			bool bOk = false;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					bOk = System.Convert.ToBoolean(((CStationElement) (StationsList[stationUUID])).Station_HA.SetProfile(profile));
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return bOk;
		}

		public void DeleteProfile(string stationUUID, string profileName)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.DeleteProfile(profileName);
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}

		public void SyncProfiles(string stationUUID)
		{
			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					((CStationElement) (StationsList[stationUUID])).Station_HA.SyncProfiles();
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();
		}


		public bool SyncFinishedProfiles(string stationUUID)
		{
			bool bFinished = false;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					bFinished = System.Convert.ToBoolean(((CStationElement) (StationsList[stationUUID])).Station_HA.SyncFinishedProfiles());
				}
				else
				{
					if (UserErrorEvent != null)
						UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED, "Function not supported in this station type."));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return bFinished;
		}

#endregion

#endregion


#region API STATION FEATURES

		/// <summary>
		/// Gets station features by Id.
		/// </summary>
		/// <param name="stationUUID">Station identifier</param>
		/// <remarks></remarks>
		public CFeaturesData GetStationFeatures(string stationUUID)
		{
			CFeaturesData features = null;

			m_mutexStationsList.WaitOne();
			
			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					features = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationFeatures();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					features = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationFeatures();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					features = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationFeatures();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					features = ((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationFeatures();
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return features;
		}

#endregion


#region API TRANSACTION

		/// <summary>
		/// Indicates the station to raise a TransactionFinished event when process this operation,
		/// to know that all previous operations where done
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A Transaction ID, to identify the transaction when receiving TransactionFinished event</returns>
		/// <remarks></remarks>
		public uint SetTransaction(string stationUUID)
		{
			//#PG#
			uint transaction = 0;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					transaction = System.Convert.ToUInt32(((CStationElement) (StationsList[stationUUID])).Station_SOLD.SetTransaction());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					transaction = System.Convert.ToUInt32(((CStationElement) (StationsList[stationUUID])).Station_HA.SetTransaction());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					transaction = System.Convert.ToUInt32(((CStationElement) (StationsList[stationUUID])).Station_SF.SetTransaction());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					transaction = System.Convert.ToUInt32(((CStationElement) (StationsList[stationUUID])).Station_FE.SetTransaction());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return transaction;
		}

		/// <summary>
		/// Ask the station if a Transaction has finished
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <param name="transactionID">The transaction identifier</param>
		/// <returns>True or False</returns>
		/// <remarks></remarks>
		public bool QueryTransaction(string stationUUID, uint transactionID)
		{
			//#PG#
			bool transaction = false;

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					transaction = System.Convert.ToBoolean(((CStationElement) (StationsList[stationUUID])).Station_SOLD.QueryEndedTransaction(transactionID));
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					transaction = System.Convert.ToBoolean(((CStationElement) (StationsList[stationUUID])).Station_HA.QueryEndedTransaction(transactionID));
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					transaction = System.Convert.ToBoolean(((CStationElement) (StationsList[stationUUID])).Station_SF.QueryEndedTransaction(transactionID));
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					transaction = System.Convert.ToBoolean(((CStationElement) (StationsList[stationUUID])).Station_FE.QueryEndedTransaction(transactionID));
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return transaction;
		}

#endregion


#region API Private Methods

		private string LookForUpperParentStation(string stationUUID)
		{
			string retUUID = "";
			
			if (StationsList.Contains(stationUUID))
			{
				retUUID = stationUUID;

				while (StationsList.Contains(((CStationElement) (StationsList[retUUID])).ParentUUID))
				{
					retUUID = ((CStationElement) (StationsList[retUUID])).ParentUUID;
				}
			}

			return retUUID;
		}

		private string LookForChildStation(string stationUUID, List<string> StationsListSearch = default(List<string>))
		{
			string retUUID = "";

			if (StationsList.Contains(stationUUID))
			{
				if (((CStationElement) (StationsList[retUUID])).State == StationState.Connected)
				{
					retUUID = stationUUID;

					bool hasChild = false;

					//Si la busqueda no está acotada a un subconjunto de estaciones utilizamos la lista entera de estaciones
					if (ReferenceEquals(StationsListSearch, null))
					{
						StationsListSearch = new List<string>();
						foreach (CStationElement item in StationsList)
						{
							StationsListSearch.Add(item.UUID);
						}
					}

					//Mientras encontremos una estación hija
					do
					{
						hasChild = false;
						foreach (CStationElement candidateStation in StationsList)
						{

							//Si es una estación hija y está conectada
							if (candidateStation.ParentUUID == stationUUID & candidateStation.State == StationState.Connected)
							{

								//Está en la lista de estaciones en las que buscar
								if (StationsListSearch.Contains(candidateStation.UUID))
								{
									retUUID = candidateStation.UUID;
								}

								stationUUID = candidateStation.UUID;
								hasChild = true;
								break;
							}
						}
					} while (hasChild);
				}
			}

			return retUUID;
		}

#endregion


#region SEARCH MODULE

		/// <summary>
		/// Start searching connected stations.
		/// </summary>
		/// <param name="searchMode">Select the connection type</param>
		/// <remarks></remarks>
		public void StartSearch(SearchMode searchMode = default(SearchMode))
		{
			m_SearchManager = new CSearchManager();
			m_SearchManager.NewConnection += SearchManager_NewConnection;
			m_SearchManager.StartSearch(searchMode);
		}

		/// <summary>
		/// Stop searching connected stations.
		/// </summary>
		/// <param name="searchMode">Select the connection type</param>
		/// <remarks></remarks>
		public void StopSearch(SearchMode searchMode)
		{
			m_SearchManager.StopSearch(searchMode);
		}

		/// <summary>
		/// Ask if it is searching connected stations.
		/// </summary>
		/// <param name="searchMode">Select the connection type</param>
		/// <remarks></remarks>
		public bool isSearching(SearchMode searchMode)
		{
			return m_SearchManager.isSearching(searchMode);
		}

		/// <summary>
		/// Ask if it is searching with any search mode.
		/// </summary>
		/// <remarks></remarks>
		public bool isSearching()
		{
			return m_SearchManager.isSearching();
		}

		/// <summary>
		/// This function handles the Search NewConnection event.
		/// </summary>
		/// <param name="connectionData">Parameters of the new connection</param>
		/// <remarks></remarks>
		private void SearchManager_NewConnection(ref CConnectionData connectionData)
		{

			//
			//Communication
			//
			CCommunicationChannel comChannel = new CCommunicationChannel(connectionData);

			//
			//Station
			//
			string UUID = "";
			CStationElement stationElement = new CStationElement();
			stationElement.State = StationState.Initiating;

			// create station object based on station type

			// get model data (model, model type y model version)
			CModelData stationModelData = new CModelData(connectionData.StationModel);
			// get station type
			CStationsConfiguration confStation = new CStationsConfiguration(stationModelData.Model);
			// save station type to determine which objet to use in all API
			stationElement.StationType = confStation.StationType;

			switch (stationElement.StationType)
			{
				case eStationType.SOLD:
					// create soldering station
					stationElement.Station_SOLD = new CStation_SOLD(connectionData.StationNumDevice,
							connectionData.CommandProtocol,
							connectionData.FrameProtocol,
							connectionData.StationModel,
							connectionData.SoftwareVersion,
							connectionData.HardwareVersion,
							comChannel);

					stationElement.Station_SOLD.StationDisconnected += launchStationDisconnected;
					stationElement.Station_SOLD.UserError += launchUserError;
					stationElement.Station_SOLD.Initialized += StationInitialized;
					stationElement.Station_SOLD.Detected_SubStation += Detected_SubStation;
					stationElement.Station_SOLD.UpdateMicroFirmwareFinished += UpdateStationFinished;

					//Initialize
					stationElement.Station_SOLD.InitializeComChannel();
					UUID = stationElement.Station_SOLD.Initialize();
					break;

				case eStationType.HA:
					// create HA desoldering station
					stationElement.Station_HA = new CStation_HA(connectionData.StationNumDevice,
							connectionData.CommandProtocol,
							connectionData.FrameProtocol,
							connectionData.StationModel,
							connectionData.SoftwareVersion,
							connectionData.HardwareVersion,
							comChannel);

					stationElement.Station_HA.StationDisconnected += launchStationDisconnected;
					stationElement.Station_HA.UserError += launchUserError;
					stationElement.Station_HA.Initialized += StationInitialized;
					stationElement.Station_HA.Detected_SubStation += Detected_SubStation;
					stationElement.Station_HA.UpdateMicroFirmwareFinished += UpdateStationFinished;

					//Initialize
					stationElement.Station_HA.InitializeComChannel();
					UUID = stationElement.Station_HA.Initialize();
					break;

				case eStationType.SF:
					// create soldering feeder station
					stationElement.Station_SF = new CStation_SF(connectionData.StationNumDevice,
							connectionData.CommandProtocol,
							connectionData.FrameProtocol,
							connectionData.StationModel,
							connectionData.SoftwareVersion,
							connectionData.HardwareVersion,
							comChannel);

					stationElement.Station_SF.StationDisconnected += launchStationDisconnected;
					stationElement.Station_SF.UserError += launchUserError;
					stationElement.Station_SF.Initialized += StationInitialized;
					stationElement.Station_SF.Detected_SubStation += Detected_SubStation;
					stationElement.Station_SF.UpdateMicroFirmwareFinished += UpdateStationFinished;

					//Initialize
					stationElement.Station_SF.InitializeComChannel();
					UUID = stationElement.Station_SF.Initialize();
					break;

				case eStationType.FE:
					// create soldering feeder station
					stationElement.Station_FE = new CStation_FE(connectionData.StationNumDevice,
							connectionData.CommandProtocol,
							connectionData.FrameProtocol,
							connectionData.StationModel,
							connectionData.SoftwareVersion,
							connectionData.HardwareVersion,
							comChannel);

					stationElement.Station_FE.StationDisconnected += launchStationDisconnected;
					stationElement.Station_FE.UserError += launchUserError;
					stationElement.Station_FE.Initialized += StationInitialized;
					stationElement.Station_FE.Detected_SubStation += Detected_SubStation;
					stationElement.Station_FE.UpdateMicroFirmwareFinished += UpdateStationFinished;

					//Initialize
					stationElement.Station_FE.InitializeComChannel();
					UUID = stationElement.Station_FE.Initialize();
					break;

			}

			//Add station if is correct initialized
			if (!string.IsNullOrEmpty(UUID))
			{
				m_StationAvailableCom.AddUUIDStation(UUID, connectionData.StationNumDevice, ref comChannel);
				stationElement.UUID = UUID;
				StationsList.Add(UUID, stationElement);

				//Delete station if is not correct initialized
			}
			else
			{
				if (stationElement.Station_SOLD != null)
				{
					stationElement.Station_SOLD.Dispose();
					stationElement.Station_SOLD = null;
				}

				if (stationElement.Station_HA != null)
				{
					stationElement.Station_HA.Dispose();
					stationElement.Station_HA = null;
				}

				if (stationElement.Station_SF != null)
				{
					stationElement.Station_SF.Dispose();
					stationElement.Station_SF = null;
				}

				if (stationElement.Station_FE != null)
				{
					stationElement.Station_FE.Dispose();
					stationElement.Station_FE = null;
				}

				//Delete Communication Channel
				comChannel.Dispose();
			}
		}

#endregion


#region SUBSTATIONS

		private void Detected_SubStation(string stationParentUUID, CConnectionData connectionData)
		{
			m_mutexStationsList.WaitOne();

			if (!m_StationAvailableCom.CheckAddressComChannel(stationParentUUID, connectionData.StationNumDevice))
			{
				if (StationsList.Contains(stationParentUUID))
				{

					//
					//Communication
					//
					CCommunicationChannel comChannel = null;
					m_StationAvailableCom.GetComChannel(stationParentUUID, ref comChannel);

					//
					//Station
					//
					string UUID = "";
					CStationElement stationElement = new CStationElement();
					stationElement.State = StationState.Initiating;
					stationElement.State = StationState.Initiating;

					// create station object based on station type

					// get model data (model, model type y model version)
					CModelData stationModelData = new CModelData(connectionData.StationModel);
					// get station type
					CStationsConfiguration confStation = new CStationsConfiguration(stationModelData.Model);
					// save station type to determine which objet to use in all API
					stationElement.StationType = confStation.StationType;

					switch (stationElement.StationType)
					{
						case eStationType.SOLD:
							// create soldering station
							stationElement.Station_SOLD = new CStation_SOLD(connectionData.StationNumDevice,
									connectionData.CommandProtocol,
									connectionData.FrameProtocol,
									connectionData.StationModel,
									connectionData.SoftwareVersion,
									connectionData.HardwareVersion,
									comChannel,
									stationParentUUID);

							stationElement.Station_SOLD.StationDisconnected += launchStationDisconnected;
							stationElement.Station_SOLD.UserError += launchUserError;
							stationElement.Station_SOLD.Initialized += StationInitialized;
							stationElement.Station_SOLD.Detected_SubStation += Detected_SubStation;
							stationElement.Station_SOLD.UpdateMicroFirmwareFinished += UpdateStationFinished;

							//Initialize
							UUID = stationElement.Station_SOLD.Initialize();
							break;

						case eStationType.HA:
							// create HA desoldering station
							stationElement.Station_HA = new CStation_HA(connectionData.StationNumDevice,
									connectionData.CommandProtocol,
									connectionData.FrameProtocol,
									connectionData.StationModel,
									connectionData.SoftwareVersion,
									connectionData.HardwareVersion,
									comChannel);

							stationElement.Station_HA.StationDisconnected += launchStationDisconnected;
							stationElement.Station_HA.UserError += launchUserError;
							stationElement.Station_HA.Initialized += StationInitialized;
							stationElement.Station_HA.Detected_SubStation += Detected_SubStation;
							stationElement.Station_HA.UpdateMicroFirmwareFinished += UpdateStationFinished;

							//Initialize
							UUID = stationElement.Station_HA.Initialize();
							break;

						case eStationType.SF:
							// create soldering feeder station
							stationElement.Station_SF = new CStation_SF(connectionData.StationNumDevice,
									connectionData.CommandProtocol,
									connectionData.FrameProtocol,
									connectionData.StationModel,
									connectionData.SoftwareVersion,
									connectionData.HardwareVersion,
									comChannel);

							stationElement.Station_SF.StationDisconnected += launchStationDisconnected;
							stationElement.Station_SF.UserError += launchUserError;
							stationElement.Station_SF.Initialized += StationInitialized;
							stationElement.Station_SF.Detected_SubStation += Detected_SubStation;
							stationElement.Station_SF.UpdateMicroFirmwareFinished += UpdateStationFinished;

							//Initialize
							UUID = stationElement.Station_SF.Initialize();
							break;

						case eStationType.FE:
							// create fume extractor station
							stationElement.Station_FE = new CStation_FE(connectionData.StationNumDevice,
									connectionData.CommandProtocol,
									connectionData.FrameProtocol,
									connectionData.StationModel,
									connectionData.SoftwareVersion,
									connectionData.HardwareVersion,
									comChannel);

							stationElement.Station_FE.StationDisconnected += launchStationDisconnected;
							stationElement.Station_FE.UserError += launchUserError;
							stationElement.Station_FE.Initialized += StationInitialized;
							stationElement.Station_FE.Detected_SubStation += Detected_SubStation;
							stationElement.Station_FE.UpdateMicroFirmwareFinished += UpdateStationFinished;

							//Initialize
							UUID = stationElement.Station_FE.Initialize();
							break;

					}

					m_StationAvailableCom.AddUUIDStation(UUID, connectionData.StationNumDevice, ref comChannel);
					stationElement.UUID = UUID;
					StationsList.Add(UUID, stationElement);
				}
			}

			m_mutexStationsList.ReleaseMutex();
		}

#endregion


#region STATION COMMUNICATION

		/// <summary>
		/// Gets the COM port name or Ethernet address of the indicated station. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing the COM port name or Ethernet address. Ex: "COM6" or "192.168.1.132"</returns>
		/// <remarks></remarks>
		public string GetStationCOM(string stationUUID)
		{
			//#PG#
			string stationCOM = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					stationCOM = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationCom());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					stationCOM = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationCom());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					stationCOM = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationCom());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					stationCOM = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationCom());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return stationCOM;
		}

		/// <summary>
		/// Gets the type of connection: U = USB or E = Ethernet. If the station identifier
		/// is not correct an error event is thrown.
		/// </summary>
		/// <param name="stationUUID">The identifier of the desired station</param>
		/// <returns>A string containing U or E</returns>
		/// <remarks></remarks>
		public string GetStationConnectionType(string stationUUID)
		{
			//#PG#
			string connectionType = "";

			m_mutexStationsList.WaitOne();

			if (!StationsList.Contains(stationUUID))
			{
				if (UserErrorEvent != null)
					UserErrorEvent(stationUUID, new Cerror(Cerror.cErrorCodes.STATION_UUID_NOT_FOUND, "Station UUID not found."));

			}
			else
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					connectionType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetStationConnectionType());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					connectionType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_HA.GetStationConnectionType());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					connectionType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_SF.GetStationConnectionType());
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.FE)
				{
					connectionType = System.Convert.ToString(((CStationElement) (StationsList[stationUUID])).Station_FE.GetStationConnectionType());
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return connectionType;
		}

#endregion


#region UPDATE FIRMWARE

		public List<string> GetStationListUpdating()
		{
			List<string> retList = new List<string>();

			m_mutexStationsList.WaitOne();
			foreach (DictionaryEntry entry in m_listStationsPendingUpdate)
			{
				retList.Add(System.Convert.ToString(entry.Key));
			}
			m_mutexStationsList.ReleaseMutex();

			return retList;
		}

		public List<CFirmwareStation> GetVersionMicroFirmware(string stationUUID)
		{
			List<CFirmwareStation> microFirmwares = new List<CFirmwareStation>();

			m_mutexStationsList.WaitOne();

			if (StationsList.Contains(stationUUID))
			{
				if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SOLD)
				{
					microFirmwares = ((CStationElement) (StationsList[stationUUID])).Station_SOLD.GetVersionMicrosFirmware();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.HA)
				{
					microFirmwares = ((CStationElement) (StationsList[stationUUID])).Station_HA.GetVersionMicrosFirmware();
				}
				else if (((CStationElement) (StationsList[stationUUID])).StationType == eStationType.SF)
				{
					microFirmwares = ((CStationElement) (StationsList[stationUUID])).Station_SF.GetVersionMicrosFirmware();
				}
			}
			m_mutexStationsList.ReleaseMutex();

			return microFirmwares;
		}

		public void UpdateStations(List<CFirmwareStation> listStationsToUpdate)
		{

			//Actualizar la lista de estaciones a actualizar
			m_mutexStationsList.WaitOne();
			foreach (CFirmwareStation firmwareUpdate in listStationsToUpdate)
			{
				if (m_listStationsPendingUpdate.Contains(firmwareUpdate.StationUUID))
				{
					List<CFirmwareStation> listFirmware = (List<CFirmwareStation>) (m_listStationsPendingUpdate[firmwareUpdate.StationUUID]);
					listFirmware.Add(firmwareUpdate);
					m_listStationsPendingUpdate[firmwareUpdate.StationUUID] = listFirmware;
				}
				else
				{
					List<CFirmwareStation> listFirmware = new List<CFirmwareStation>();
					listFirmware.Add(firmwareUpdate);
					m_listStationsPendingUpdate.Add(firmwareUpdate.StationUUID, listFirmware);
				}
			}
			m_mutexStationsList.ReleaseMutex();

			UpdateNextStation();
		}

		private void UpdateNextStation()
		{

			//Si hay alguna estación pendiente de actualizar
			if (m_listStationsPendingUpdate.Count > 0 && string.IsNullOrEmpty(m_StationUpdatingProgress))
			{

				m_mutexStationsList.WaitOne();
				List<string> listStationsPendingUpdate = new List<string>();

				//Seleccionamos una estación para actualizarla
				string stationToUpdate = "";
				foreach (DictionaryEntry entry in m_listStationsPendingUpdate)
				{
					stationToUpdate = System.Convert.ToString(entry.Key);
					listStationsPendingUpdate.Add(stationToUpdate);
				}

				//Buscamos la estación hija (búsqueda acotada)
				stationToUpdate = LookForChildStation(stationToUpdate, listStationsPendingUpdate);

				//Si no existe la estación la eliminamos
				if (!StationsList.Contains(stationToUpdate))
				{
					m_listStationsPendingUpdate.Remove(stationToUpdate);
				}
				m_mutexStationsList.ReleaseMutex();

				if (!StationsList.Contains(stationToUpdate))
				{
					UpdateNextStation();
				}
				else
				{
					//ID Station que se está actualizando
					m_StationUpdatingProgress = stationToUpdate;

					if (((CStationElement) (StationsList[stationToUpdate])).StationType == eStationType.SOLD)
					{
						//Versiones de firmware
						List<CFirmwareStation> versionMicrosStation = ((CStationElement) (StationsList[stationToUpdate])).Station_SOLD.GetVersionMicrosFirmware();
						List<CFirmwareStation> versionMicrosToUpdate = new List<CFirmwareStation>();

						//Descargar archivos y obtener información
						if (GetUpdateFirmwareEvent != null)
							GetUpdateFirmwareEvent(ref versionMicrosStation);

						//Buscar en versionMicros los elementos con un hardware que estén en m_listStationsPendingUpdate
						List<CFirmwareStation> listFirmwareStationPendingUpdate = new List<CFirmwareStation>();
						if (m_listStationsPendingUpdate.Contains(stationToUpdate))
						{
							listFirmwareStationPendingUpdate = (List<CFirmwareStation>) (m_listStationsPendingUpdate[stationToUpdate]);
						}

						for (int i = 0; i <= versionMicrosStation.Count - 1; i++)
						{
							for (int j = 0; j <= listFirmwareStationPendingUpdate.Count - 1; j++)
							{
								if (versionMicrosStation[i].SoftwareVersion == listFirmwareStationPendingUpdate[j].SoftwareVersion)
								{
									versionMicrosToUpdate.Add(versionMicrosStation[i]);
									break;
								}
							}
						}

						//Actualizar micros
						((CStationElement) (StationsList[stationToUpdate])).Station_SOLD.UpdateMicrosFirmware(versionMicrosToUpdate);
					}
					else if (((CStationElement) (StationsList[stationToUpdate])).StationType == eStationType.HA)
					{
						//Versiones de firmware
						List<CFirmwareStation> versionMicrosStation = ((CStationElement) (StationsList[stationToUpdate])).Station_HA.GetVersionMicrosFirmware();
						List<CFirmwareStation> versionMicrosToUpdate = new List<CFirmwareStation>();

						//Descargar archivos y obtener información
						if (GetUpdateFirmwareEvent != null)
							GetUpdateFirmwareEvent(ref versionMicrosStation);

						//Buscar en versionMicros los elementos con un hardware que estén en m_listStationsPendingUpdate
						List<CFirmwareStation> listFirmwareStationPendingUpdate = new List<CFirmwareStation>();
						if (m_listStationsPendingUpdate.Contains(stationToUpdate))
						{
							listFirmwareStationPendingUpdate = (List<CFirmwareStation>) (m_listStationsPendingUpdate[stationToUpdate]);
						}

						for (int i = 0; i <= versionMicrosStation.Count - 1; i++)
						{
							for (int j = 0; j <= listFirmwareStationPendingUpdate.Count - 1; j++)
							{
								if (versionMicrosStation[i].SoftwareVersion == listFirmwareStationPendingUpdate[j].SoftwareVersion)
								{
									versionMicrosToUpdate.Add(versionMicrosStation[i]);
									break;
								}
							}
						}

						//Actualizar micros
						((CStationElement) (StationsList[stationToUpdate])).Station_HA.UpdateMicrosFirmware(versionMicrosToUpdate);
					}
					else if (((CStationElement) (StationsList[stationToUpdate])).StationType == eStationType.SF)
					{
						//Versiones de firmware
						List<CFirmwareStation> versionMicrosStation = ((CStationElement) (StationsList[stationToUpdate])).Station_SF.GetVersionMicrosFirmware();
						List<CFirmwareStation> versionMicrosToUpdate = new List<CFirmwareStation>();

						//Descargar archivos y obtener información
						if (GetUpdateFirmwareEvent != null)
							GetUpdateFirmwareEvent(ref versionMicrosStation);

						//Buscar en versionMicros los elementos con un hardware que estén en m_listStationsPendingUpdate
						List<CFirmwareStation> listFirmwareStationPendingUpdate = new List<CFirmwareStation>();
						if (m_listStationsPendingUpdate.Contains(stationToUpdate))
						{
							listFirmwareStationPendingUpdate = (List<CFirmwareStation>) (m_listStationsPendingUpdate[stationToUpdate]);
						}

						for (int i = 0; i <= versionMicrosStation.Count - 1; i++)
						{
							for (int j = 0; j <= listFirmwareStationPendingUpdate.Count - 1; j++)
							{
								if (versionMicrosStation[i].SoftwareVersion == listFirmwareStationPendingUpdate[j].SoftwareVersion)
								{
									versionMicrosToUpdate.Add(versionMicrosStation[i]);
									break;
								}
							}
						}

						//Actualizar micros
						((CStationElement) (StationsList[stationToUpdate])).Station_SF.UpdateMicrosFirmware(versionMicrosToUpdate);
					}
				}
			}
		}

		private void UpdateStationFinished(string stationUUID)
		{
			m_mutexStationsList.WaitOne();
			if (m_listStationsPendingUpdate.Contains(stationUUID))
			{
				m_listStationsPendingUpdate.Remove(stationUUID);
				m_StationUpdatingProgress = "";
			}
			m_mutexStationsList.ReleaseMutex();

			launchStationDisconnected(stationUUID);
		}

#endregion

	}
}
