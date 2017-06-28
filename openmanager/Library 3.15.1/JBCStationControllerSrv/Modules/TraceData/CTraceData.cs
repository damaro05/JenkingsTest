using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

using JBC_Connect;
using DataJBC;

namespace JBCStationControllerSrv
{
	public class CTraceData
	{

		private const int THREAD_SLEEP_TRACE_DATA = 1000; // 1 segundo
		private const SpeedContinuousMode SPEED_TRACE_DATA = SpeedContinuousMode.T_100mS;
		private const int FILE_MAX_SEQUENCE = 600; // cantidad de datos en 1 archivo de trace
		private const int FILE_CHUNK_SIZE = 10 * 1024; //10kb. The buffer size by default is set to 64kb
		private const string TRACE_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
		private const string FILE_TIME_FORMAT = "yyyyMMddHHmmss";
		private const string TRACE_FILE_EXTENSION = "json";
		private const string TRACE_TEMP_FILE_EXTENSION = "json.tmp";

	
		private Hashtable m_htStationUUID2QueueID = new Hashtable(); //<stationUUID, QueueID>
		private RoutinesLibrary.Data.Structures.MultiDimensionalHashtable m_mdhtStationUUID2PortData = new RoutinesLibrary.Data.Structures.MultiDimensionalHashtable(); //<stationUUID, <port, trace data>>
		private RoutinesLibrary.Data.Structures.MultiDimensionalHashtable m_mdhtStationUUID2PortNumSequence = new RoutinesLibrary.Data.Structures.MultiDimensionalHashtable(); //<stationUUID, <port, num data sequence>>

		private string m_folderData; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
		private Thread m_ThreadTraceData;
		private bool m_IsAliveThreadTraceData = true;


#region Constructor / destructor

		public CTraceData()
		{
			// VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
			m_folderData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles), "JBC\\JBC Station Controller Service\\TraceData");


			//Create folder to store the trace data file
			if (!(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.DirectoryExists(m_folderData))
			{
				Directory.CreateDirectory(m_folderData);
			}

			RepairBrokenFiles();

			//Start trace data
			m_ThreadTraceData = new Thread(new ThreadStart(TraceData));
			m_ThreadTraceData.IsBackground = true;
			m_ThreadTraceData.Start();

		}

		public void Dispose()
		{
			m_IsAliveThreadTraceData = false;
			RepairBrokenFiles(); //FIXME. in thread
		}

#endregion


#region Start / stop

		public bool StartTraceData(string UUID, Port portNbr)
		{
			bool bOk = false;

			//Add port to list data
			if (!m_mdhtStationUUID2PortData.Contains(UUID, portNbr))
			{
				m_mdhtStationUUID2PortData.Add(UUID, portNbr, new TracePortData(portNbr));
				bOk = true;
			}

			//Add port to list num sequence
			if (!m_mdhtStationUUID2PortNumSequence.Contains(UUID, portNbr))
			{
				m_mdhtStationUUID2PortNumSequence.Add(UUID, portNbr, 0);
			}

			//Start station continuous mode for the desired station
			if (!m_htStationUUID2QueueID.Contains(UUID))
			{
				m_htStationUUID2QueueID.Add(UUID, (uint) (DLLConnection.jbc.StartContinuousMode(UUID, SPEED_TRACE_DATA)));
			}

			return true;
		}

		public bool StopTraceData(string UUID)
		{
			bool bOk = false;

			if (m_htStationUUID2QueueID.Contains(UUID))
			{

				//Stop continuous mode
				DLLConnection.jbc.StopContinuousMode(UUID, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])));

				//Listado de puertos que estamos trazando para la estación
				foreach (Port port in m_mdhtStationUUID2PortData.RowKeys(UUID))
				{

					//Cierre y renombre del fichero con id único
					if (File.Exists(TempPathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), port)))
					{
						(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(TempPathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), port), "]}", true);
						(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(TempPathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), port),
								TracePathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), port, DateTime.Now));
					}
				}

				m_htStationUUID2QueueID.Remove(UUID);
				m_mdhtStationUUID2PortData.RemoveRow(UUID);
				m_mdhtStationUUID2PortNumSequence.RemoveRow(UUID);

				bOk = true;
			}

			return bOk;
		}

		public bool StopTraceData(string UUID, Port portNbr)
		{
			bool bOk = System.Convert.ToBoolean(m_mdhtStationUUID2PortData.Contains(UUID, portNbr));

			m_mdhtStationUUID2PortData.Remove(UUID, portNbr);
			m_mdhtStationUUID2PortNumSequence.Remove(UUID, portNbr);

			if (bOk)
			{
				//Cierre y renombre del fichero con id único
				if (File.Exists(TempPathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), portNbr)))
				{
					(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(TempPathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), portNbr), "]}", true);
					(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(TempPathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), portNbr),
							TracePathFilename(m_folderData, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])), portNbr, DateTime.Now));
				}
			}

			//Si no existe ningún puerto más que se esté trazando
			if (!m_mdhtStationUUID2PortData.ContainsRow(UUID))
			{

				//Stop continuous mode
				DLLConnection.jbc.StopContinuousMode(UUID, System.Convert.ToUInt32(System.Convert.ToUInt32(m_htStationUUID2QueueID[UUID])));

				m_htStationUUID2QueueID.Remove(UUID);
			}

			return bOk;
		}

#endregion


#region Files

		public List<string> GetListRecordedDataFiles()
		{
			List<string> listRecordedData = new List<string>();

			DirectoryInfo di = new DirectoryInfo(m_folderData);
			foreach (var fi in di.GetFiles("*." + TRACE_FILE_EXTENSION))
			{
				listRecordedData.Add(fi.Name);
			}

			return listRecordedData;
		}

		public dc_TraceDataSequence GetRecordedData(string fileName, int nSequence)
		{
			dc_TraceDataSequence traceDataSequence = new dc_TraceDataSequence();
			byte[] bytes = new byte[1];

			try
			{
				if (File.Exists(Path.Combine(m_folderData, fileName.Trim())))
				{
					FileStream fileStream = new FileStream(Path.Combine(m_folderData, fileName.Trim()), FileMode.Open, FileAccess.Read);
					BinaryReader binaryReader = new BinaryReader(fileStream);
					long seekPos = binaryReader.BaseStream.Seek((nSequence - 1) * FILE_CHUNK_SIZE, SeekOrigin.Begin);

					bytes = binaryReader.ReadBytes(FILE_CHUNK_SIZE);
					fileStream.Close();

					traceDataSequence.final = bytes.Length < FILE_CHUNK_SIZE;
					traceDataSequence.sequence = nSequence;
					traceDataSequence.bytes = bytes;
				}
				else
				{
					traceDataSequence.sequence = -1;
				}
			}
			catch (Exception ex)
			{
				traceDataSequence.sequence = -1;
				LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
			}

			return traceDataSequence;
		}

		public bool DeleteRecordedDataFile(string fileName)
		{
			bool bOk = false;

			if (File.Exists(Path.Combine(m_folderData, fileName)))
			{
				File.Delete(Path.Combine(m_folderData, fileName));
				bOk = true;
			}

			return bOk;
		}

		private void RepairBrokenFiles()
		{
			//Move temporary files and check it
			DirectoryInfo dInfo = new DirectoryInfo(m_folderData);
			foreach (var fInfo in dInfo.GetFiles("*." + TRACE_TEMP_FILE_EXTENSION))
			{
				CloseJSONFile(System.Convert.ToString(fInfo.FullName));

				(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(Path.Combine(m_folderData, System.Convert.ToString(fInfo.Name)), 
						Path.Combine(m_folderData, System.Convert.ToString(fInfo.Name.Replace("." + TRACE_TEMP_FILE_EXTENSION, ""))) + "_" + DateTime.Now.ToString(FILE_TIME_FORMAT) +"." + TRACE_FILE_EXTENSION);
			}
		}

		private void CloseJSONFile(string fileName)
		{
			FileInfo fi = new FileInfo(fileName);
			FileStream fs = default(FileStream);
			byte[] b = new byte[1];

			//Check last character for correct format
			fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
			fs.Seek(-1, SeekOrigin.End);
			if (fs.Read(b, b.Length - 1, b.Length) > 0)
			{
				if (b[0] != '}')
				{
					fs.Write(System.Text.Encoding.UTF8.GetBytes("}"), 0, 1);
				}
			}
			fs.Close();
		}

		private string TracePathFilename(string sFolderData, uint queueID, Port portNbr, DateTime datetimeData)
		{
			return Path.Combine(sFolderData, (queueID).ToString()) + "_" + System.Convert.ToString(portNbr) + "_" + datetimeData.ToString(FILE_TIME_FORMAT) +"." + TRACE_FILE_EXTENSION;
		}

		private string TempPathFilename(string sFolderData, uint queueID, Port portNbr)
		{
			return Path.Combine(sFolderData, (queueID).ToString()) + "_" + System.Convert.ToString(portNbr) +"." + TRACE_TEMP_FILE_EXTENSION;
		}

#endregion


#region Trace

		private void TraceData()
		{
			ArrayList listStationsUUID = new ArrayList();
			stContinuousModeData_SOLD contData_SOLD = default(stContinuousModeData_SOLD); // solder data
			stContinuousModeData_HA contData_HA = new stContinuousModeData_HA(); // hot air data
			stContinuousModePort_SOLD contDataPort = new stContinuousModePort_SOLD(); // solder data
			stContinuousModePort_HA contDataPort_HA = new stContinuousModePort_HA(); // hot air data

			while (m_IsAliveThreadTraceData)
			{
				// update stations list
				listStationsUUID.Clear();
				listStationsUUID.AddRange(m_htStationUUID2QueueID.Keys);

				//Iteramos en todas las estaciones
				foreach (string stationUUID in listStationsUUID)
				{
					uint queueID = (uint) (m_htStationUUID2QueueID[stationUUID]);

					try
					{
						if (DLLConnection.jbc.StationExists(stationUUID))
						{
							eStationType stationType = DLLConnection.jbc.GetStationType(stationUUID);
							int dataLength = DLLConnection.jbc.GetContinuousModeDataCount(stationUUID, queueID);

							//Iteramos en todos los datos recogidos
							for (int i = 0; i <= dataLength - 1; i++)
							{

								bool bDataExists = false;
								switch (stationType)
								{
									case eStationType.SOLD:
										contData_SOLD = DLLConnection.jbc.GetContinuousModeNextData_SOLD(stationUUID, queueID);
										if (contData_SOLD.data != null)
										{
											bDataExists = true;
										}
										break;
									case eStationType.HA:
										contData_HA = DLLConnection.jbc.GetContinuousModeNextData_HA(stationUUID, queueID);
										if (contData_HA.data != null)
										{
											bDataExists = true;
										}
										break;
								}

								if (bDataExists)
								{
									Hashtable portJSONData = new Hashtable();

									foreach (Port port in m_mdhtStationUUID2PortData.RowKeys(stationUUID))
									{
										portJSONData[port] = "";

										//Cabecera de datos si secuencia es cero
										if (System.Convert.ToInt32(m_mdhtStationUUID2PortNumSequence.Item(stationUUID, port)) == 0)
										{
											// obtener la velocidad de la cola
											SpeedContinuousMode queueCaptureSpeed = DLLConnection.jbc.GetContinuousModeDeliverySpeed(stationUUID, queueID);
											CSpeedContMode speedcm = new CSpeedContMode();
											int frequency = speedcm.SpeedFromEnum(queueCaptureSpeed);

											portJSONData[port] = "{" + "\r\n" + "\"uuid\":\"" + stationUUID + "\"," +
												"\r\n" + "\"port\":" + System.Convert.ToString(port) + "," +
												"\r\n" + "\"time\":\"" + DateTime.Now.ToString(TRACE_TIME_FORMAT) + "\"," +
												"\r\n" + "\"name\":\"" + DLLConnection.jbc.GetStationName(stationUUID) + "\"," +
												"\r\n" + "\"type\":\"" + stationType.ToString() + "\"," +
												"\r\n" + "\"model\":\"" + DLLConnection.jbc.GetStationModel(stationUUID) + "\"," +
												"\r\n" + "\"modeltype\":\"" + DLLConnection.jbc.GetStationModelType(stationUUID) + "\"," +
												"\r\n" + "\"modelversion\":\"" + DLLConnection.jbc.GetStationModelVersion(stationUUID).ToString() + "\"," +
												"\r\n" + "\"software\":\"" + DLLConnection.jbc.GetStationSWversion(stationUUID) + "\"," +
												"\r\n" + "\"hardware\":\"" + DLLConnection.jbc.GetStationHWversion(stationUUID) + "\"," +
												"\r\n" + "\"interval\":" + System.Convert.ToString(frequency) + "," +
												"\r\n" + "\"data\":[";
										}
									}

									// cantidad de puertos
									int iDataPortCount = 0;
									switch (stationType)
									{
										case eStationType.SOLD:
											iDataPortCount = contData_SOLD.data.Length;
											break;
										case eStationType.HA:
											iDataPortCount = contData_HA.data.Length;
											break;
									}

									// datos de los puertos
									for (var x = 0; x <= iDataPortCount - 1; x++)
									{
										Port readingPort = Port.NO_PORT;

										switch (stationType)
										{
											case eStationType.SOLD:
												contDataPort = contData_SOLD.data[x];
												readingPort = contDataPort.port;
												break;
											case eStationType.HA:
												contDataPort_HA = contData_HA.data[x];
												readingPort = contDataPort_HA.port;
												break;
										}

										//Copiamos los datos del registro anterior para comparar al crear el json
										Hashtable antListPortData = new Hashtable();

										foreach (Port port in m_mdhtStationUUID2PortData.RowKeys(stationUUID))
										{
											antListPortData.Add(port, ((TracePortData) (m_mdhtStationUUID2PortData.Item(stationUUID, port))).Clone());
										}

										//Si el puerto lo estamos trazando
										if (m_mdhtStationUUID2PortData.Contains(stationUUID, readingPort))
										{
											TracePortData portData = default(TracePortData);

											switch (stationType)
											{
												case eStationType.SOLD:
													portData = (TracePortData) (m_mdhtStationUUID2PortData.Item(stationUUID, readingPort));
													portData.port = readingPort;
													portData.temperature = System.Convert.ToInt32(contDataPort.temperature.UTI);
													portData.power = System.Convert.ToInt32(contDataPort.power);
													portData.status = System.Convert.ToByte(contDataPort.status);
													portData.tool = DLLConnection.jbc.GetPortToolID(stationUUID, portData.port);
													break;
												case eStationType.HA:
													portData = (TracePortData) (m_mdhtStationUUID2PortData.Item(stationUUID, readingPort));
													portData.port = readingPort;
													portData.temperature = contDataPort_HA.temperature.UTI;
													portData.power = contDataPort_HA.power;
													portData.status = System.Convert.ToByte(contDataPort_HA.status);
													portData.tool = DLLConnection.jbc.GetPortToolID(stationUUID, portData.port);
													// HA
													portData.flow = contDataPort_HA.flow;
													portData.tempTC1 = contDataPort_HA.externalTC1_Temp.UTI;
													portData.tempTC2 = contDataPort_HA.externalTC2_Temp.UTI;
													portData.timetostop = contDataPort_HA.timeToStop;
													break;
											}

											//Coma separador de entradas
											if (System.Convert.ToInt32(m_mdhtStationUUID2PortNumSequence.Item(stationUUID, readingPort)) != 0)
											{
												portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + "," + "\r\n";
											}

											//Escribimos número de secuencia
											portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + "{\"n\":" + System.Convert.ToString(m_mdhtStationUUID2PortNumSequence.Item(stationUUID, readingPort));

											//Notificar tool en la primera entrada de datos o cada vez que haya un cambio
											if (m_mdhtStationUUID2PortNumSequence.Item(stationUUID, readingPort) == 0 |
													portData.tool != ((TracePortData) (antListPortData[readingPort])).tool)
											{
												portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"o\":" + System.Convert.ToString(portData.tool);
											}

											//Si no hay tool, no notificar el status, temperatura ni power
											if (portData.tool != GenericStationTools.NO_TOOL)
											{

												//Notificar status en la primera entrada de datos o cada vez que haya un cambio
												if (System.Convert.ToInt32(m_mdhtStationUUID2PortNumSequence.Item(stationUUID, readingPort)) == 0 |
														portData.status != ((TracePortData) (antListPortData[readingPort])).status)
												{
													portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"s\":" + System.Convert.ToString(portData.status);
												}

												// temperature
												portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"t\":" + System.Convert.ToString(portData.temperature);

												//Si status es extractor o hibernation no notificar el power
												if (portData.status != (byte)ToolStatus.EXTRACTOR &
														portData.status != (byte)ToolStatus.HIBERNATION)
												{
													portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"w\":" + System.Convert.ToString(portData.power);
												}

												// HA
												if (stationType == eStationType.HA)
												{
													// flow
													portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"f\":" + System.Convert.ToString(portData.flow);

													// time to stop
													portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"ts\":" + System.Convert.ToString(portData.timetostop);

													if (portData.tempTC1 > 0)
													{
														portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"x1\":" + System.Convert.ToString(portData.tempTC1);
													}

													if (portData.tempTC2 > 0)
													{
														portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + ", \"x2\":" + System.Convert.ToString(portData.tempTC2);
													}
												}
											}

											portJSONData[readingPort] = (portJSONData[readingPort]).ToString() + "}";
											(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(TempPathFilename(m_folderData, queueID, readingPort), (portJSONData[readingPort]).ToString(), true);
										}
									}

									//Incrementar numero de secuencia
									ArrayList listPorts = new ArrayList();
									listPorts.AddRange(m_mdhtStationUUID2PortNumSequence.RowKeys(stationUUID));

									foreach (Port port in listPorts)
									{
										m_mdhtStationUUID2PortNumSequence.Add(stationUUID, port, (System.Convert.ToInt32(m_mdhtStationUUID2PortNumSequence.Item(stationUUID, port))) + 1);

										//Final de fichero
										if (System.Convert.ToInt32(m_mdhtStationUUID2PortNumSequence.Item(stationUUID, port)) == FILE_MAX_SEQUENCE)
										{
											m_mdhtStationUUID2PortNumSequence.Add(stationUUID, port, 0);

											//Cierre y renombre del fichero con id único
											(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(TempPathFilename(m_folderData, queueID, port), "]}", true);
											(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(TempPathFilename(m_folderData, queueID, port),
													TracePathFilename(m_folderData, queueID, port, DateTime.Now));
										}
									}
								}
							}
						}
						else
						{
							// Stop trace data
							m_htStationUUID2QueueID.Remove(stationUUID);
							m_mdhtStationUUID2PortData.RemoveRow(stationUUID);
							m_mdhtStationUUID2PortNumSequence.RemoveRow(stationUUID);

							ArrayList listPorts = new ArrayList();
							listPorts.AddRange(m_mdhtStationUUID2PortNumSequence.RowKeys(stationUUID));

							foreach (Port port in listPorts)
							{
								//close and rename file with unique name
								if (File.Exists(TempPathFilename(m_folderData, queueID, port)))
								{
									(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(TempPathFilename(m_folderData, queueID, port), "]}", true);
								}
								if (File.Exists(TempPathFilename(m_folderData, queueID, port)))
								{
									(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(TempPathFilename(m_folderData, queueID, port),
											TracePathFilename(m_folderData, queueID, port, DateTime.Now));
								}
							}
						}

					}
					catch (Exception)
					{
						// error
						//Stop trace data
						m_htStationUUID2QueueID.Remove(stationUUID);
						m_mdhtStationUUID2PortData.RemoveRow(stationUUID);
						m_mdhtStationUUID2PortNumSequence.RemoveRow(stationUUID);

						ArrayList listPorts = new ArrayList();
						listPorts.AddRange(m_mdhtStationUUID2PortNumSequence.RowKeys(stationUUID));

						foreach (Port port in listPorts)
						{
							//close and rename file with unique name
							if (File.Exists(TempPathFilename(m_folderData, queueID, port)))
							{
								(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(TempPathFilename(m_folderData, queueID, port), "]}", true);
								(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.MoveFile(TempPathFilename(m_folderData, queueID, port),
										TracePathFilename(m_folderData, queueID, port, DateTime.Now));
							}
						}
					}
				}

				Thread.Sleep(THREAD_SLEEP_TRACE_DATA);
			}
		}

#endregion


	}
}
