// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.IO.Ports;
using System.Text;
using Microsoft.VisualBasic;
using DataJBC;
using RoutinesJBC;

namespace JBC_Connect
{
	internal class CUpdateFirmware01
	{

		private const int MIN_ADDRESS = 0x1400;
		private const int MAX_ADDRESS = 0x7FFFF;
		private const int BLOCK_WIDTH = 2;
		private const int BLOCK_HEIGHT = 128;

		private const string END_OF_BOOT_ADDRESS = "FFFFFFFF";
		private const string VERSION_CODE_FILL_CHAR = " ";
		private const char ACK = 'S';

		private const char ADDRESS_HEADER = 'X';
		private const char DATA_HEADER = 'Y';
		private const char MICRO_FINISH_WRITE_HEADER = 'Z';
		private const char HARDWARE_HEADER = 'W';
		private const char SOFTWARE_HEADER = 'V';

		private const int ADDRESS_LENGTH = 8;
		private const int DATA_LENGTH = 16;
		private const int MICRO_FINISH_WRITE_LENGTH = 0;
		private const int HARDWARE_LENGTH = 32;
		private const int SOFTWARE_LENGTH = 32;

		private const int RESPONSE_STATION_TIMEOUT = 100;
		private const int ERASE_RESPONSE_STATION_TIMEOUT = 5000;

		private int MAX_RETRY_COMMAND = 20;


		//micro memory structure
		private class tBlock
		{
			public string address;
			public string[] data;
			public int written;
		}


		private SerialPort m_SerialPort;
		private string m_ComName;
		private string m_UUID;
		private Hashtable m_MicrosPendingUpdate = new Hashtable(); //Micros pendientes de actualizar
		private List<byte> m_MicrosPendingReset = new List<byte>(); //Micros actualizados y pendientes de hacer un reset
		private short m_MicroUpdatingProgress = (short)(-1); //Dirección del micro que se está actualizando
		private CFirmwareStation m_infoUpdateFirmwareProgress = null; //Información de la versión del micro que se está actualizando
		private List<CFirmwareStation> m_InfoUpdateFirmware; //Información de las versiones de firmware a actualizar
		private CStationFrames01_SOLD m_Frames_01;

		private int m_numBlocks;
		private tBlock[] m_memory;
		private int m_nunBlocksWritten;
		private CMicroPrograms01 m_hexManager = new CMicroPrograms01();


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



		public CUpdateFirmware01(string stationUUID, CStationFrames01_SOLD frames_01, string _ComName)
		{
			m_UUID = stationUUID;
			m_Frames_01 = frames_01;
			m_ComName = _ComName;
		}

		internal bool IsMicroUpdatingProgress()
		{
			return m_MicroUpdatingProgress != -1;
		}

		/// <summary>
		/// Initialize the update process
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
			m_infoUpdateFirmwareProgress = null;
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
									m_infoUpdateFirmwareProgress = infoUpdateFirmwareEntry;
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

				m_MicrosPendingUpdate = null;
				if (UpdateMicroFirmwareFinishedEvent != null)
					UpdateMicroFirmwareFinishedEvent(m_UUID);
			}
			else
			{

				//Obtener el modo control
				m_Frames_01.WriteConnectStatus(ControlModeConnection.CONTROL);

				//Enviar reset
				m_Frames_01.DeviceReset();
			}
		}

		internal void ContinueUpdating()
		{

			//Deshabilitar comunicación
			m_Frames_01.DeleteComChannel();

			//Decrypt
			byte[] fileReader = File.ReadAllBytes(m_infoUpdateFirmwareProgress.FileName);
			byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
			byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
			string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));

			//Carriage Return (0x0D AKA Char 13) and Line Feed (0x0A AKA Char 10) and remove empty lines
			string[] sTextLines = decrypted.Replace("\r", "").Split((char[])(new[] { '\n' }), StringSplitOptions.RemoveEmptyEntries);

			//Crear una nueva comunicación con parámetros distintos
			m_SerialPort = new SerialPort();
			m_SerialPort.PortName = m_ComName;
			//CSerialPort.ConfigPort(m_SerialPort, 500000, Parity.None)
			RoutinesLibrary.IO.SerialPort.ConfigPort(m_SerialPort, new RoutinesLibrary.IO.SerialPortConfig(500000, Parity.None));
			m_SerialPort.ReadTimeout = RESPONSE_STATION_TIMEOUT;

			//trying to read the device versions
			if (LinkPort())
			{

				//configuring the download process
				ConfigMicroMemory();
				FillMicroMemory(sTextLines);

				//starting the download procedure
				DownloadFirmware();

				//Close serial port
				m_SerialPort.Close();
				m_SerialPort.Dispose();
			}

			if (UpdateMicroFirmwareFinishedEvent != null)
				UpdateMicroFirmwareFinishedEvent(m_UUID);
		}

		private bool LinkPort()
		{
			bool linked = false;
			int retryTimes = MAX_RETRY_COMMAND;

			//trying to read the ACK during an ACK timeout
			while (retryTimes > 0 && !linked)
			{
				retryTimes--;

				//Hardware version
				if (Read(HARDWARE_LENGTH, HARDWARE_HEADER))
				{

					//Software version
					if (Read(SOFTWARE_LENGTH, SOFTWARE_HEADER))
					{
						linked = true;
					}
				}
			}

			return linked;
		}

		private void ConfigMicroMemory()
		{

			//Calculating the number of blocks
			m_numBlocks = System.Convert.ToInt32((MAX_ADDRESS + 1 - MIN_ADDRESS) / BLOCK_HEIGHT);

			//creating the micro memory structure
			m_memory = new tBlock[m_numBlocks - 1 + 1];
			for (int i = 0; i <= m_numBlocks - 1; i++)
			{
				m_memory[i] = new tBlock();
				m_memory[i].address = Conversion.Hex(MIN_ADDRESS + (i * BLOCK_HEIGHT));
				m_memory[i].written = 0;
				m_memory[i].data = new string[BLOCK_HEIGHT - 1 + 1];

				//assigning all the micro memory to 0xFF
				for (int row = 0; row <= BLOCK_HEIGHT - 1; row++)
				{
					m_memory[i].data[row] = new string('F', BLOCK_WIDTH * 2);
				}
			}
		}

		private void FillMicroMemory(string[] sTextLines)
		{

			//initializing the blocks written counter
			m_nunBlocksWritten = 0;

			//setting file content to parse
			m_hexManager.SetTextFileToParse(sTextLines);

			string address = "";
			string data = "";
			int addressValue = 0;
			int block = 0;
			int row = 0;
			int nRows = 0;
			int writtenRows = 0;
			int dataPos = 0;

			while (m_hexManager.GetNextData(ref address, ref data))
			{

				//getting the address value, it is divided by two because of the hex file are designed to work with 8 bit address
				//but we are working with 16 bit address. See "HEX files para dspic33.doc" to understand
				addressValue = System.Convert.ToInt32(Convert.ToInt32(address, 16) / 2);

				//checking the address is in the correct range
				if (addressValue <= MAX_ADDRESS & addressValue >= MIN_ADDRESS)
				{

					//calculating to which block the data is for
					block = System.Convert.ToInt32((addressValue - MIN_ADDRESS) / BLOCK_HEIGHT);

					//checking block as written and incrementing the counter if necessary
					if (m_memory[block].written == 0)
					{
						m_memory[block].written = 1;
						m_nunBlocksWritten++;
					}

					//calculating the row in the block
					row = addressValue - Convert.ToInt32(m_memory[block].address, 16);

					//calculating the number of rows to write
					nRows = System.Convert.ToInt32(data.Length / (BLOCK_WIDTH * 2));
					writtenRows = 0;

					//writting the rows
					dataPos = 0;
					while (writtenRows != nRows)
					{

						//checking if has passed to the next block
						if (row == BLOCK_HEIGHT)
						{
							//data is in the next block
							block++;

							//marking and counting next block as written, if necessary
							if (m_memory[block].written == 0)
							{
								m_memory[block].written = 1;
								m_nunBlocksWritten++;
							}

							//setting the row to 0, it is first block row
							row = 0;
						}

						//writting data in the row
						m_memory[block].data[row] = data.Substring(dataPos, BLOCK_WIDTH * 2);
						row++;
						writtenRows++;
						dataPos += BLOCK_WIDTH * 2;
					}
				}
				else
				{
					if (addressValue > MAX_ADDRESS)
					{
						//expected address in the hex file, nothing to report
					}
					else if (addressValue < MIN_ADDRESS)
					{
						//throw error
					}
				}
			}
		}

		private void DownloadFirmware()
		{
			int i = 0;

			//sending the hardware version
			string rawHWversion = m_infoUpdateFirmwareProgress.HardwareVersion;
			i = rawHWversion.Length;
			while (i < HARDWARE_LENGTH)
			{
				rawHWversion += VERSION_CODE_FILL_CHAR;
				i++;
			}
			if (!Write(System.Convert.ToString(HARDWARE_HEADER + rawHWversion)))
			{
				return;
			}

			//sending the software version
			string rawFirmwareVersion = m_infoUpdateFirmwareProgress.SoftwareVersion;
			i = rawFirmwareVersion.Length;
			while (i < SOFTWARE_LENGTH)
			{
				rawFirmwareVersion += VERSION_CODE_FILL_CHAR;
				i++;
			}
			if (!Write(System.Convert.ToString(SOFTWARE_HEADER + rawFirmwareVersion)))
			{
				return;
			}

			//waiting for micro to reset the memory
			m_SerialPort.ReadTimeout = ERASE_RESPONSE_STATION_TIMEOUT;
			if (!Read(MICRO_FINISH_WRITE_LENGTH, MICRO_FINISH_WRITE_HEADER))
			{
				return;
			}
			m_SerialPort.ReadTimeout = RESPONSE_STATION_TIMEOUT;

			//downloading the firmware
			for (i = 0; i <= m_numBlocks - 1; i++)
			{

				//sending blocks that has been written
				if (m_memory[i].written == 1)
				{
					SendBlock(i);

					//waiting for micro to write the block
					if (!Read(MICRO_FINISH_WRITE_LENGTH, MICRO_FINISH_WRITE_HEADER))
					{
						return;
					}
				}
			}

			//sending end of bootload address
			SendAddress(END_OF_BOOT_ADDRESS);
		}

		private void SendBlock(int block)
		{

			//sending the block address
			SendAddress(m_memory[block].address);

			//sending all the block data
			string data = "";
			int nDataTransmisions = System.Convert.ToInt32((BLOCK_HEIGHT * BLOCK_WIDTH) / DATA_LENGTH);
			int rowsPerDataTrans = DATA_LENGTH / BLOCK_WIDTH;
			int row = 0;
			int lastRow = 0;

			for (int trans = 0; trans <= nDataTransmisions - 1; trans++)
			{
				data = "";

				//building the data string
				row = trans * rowsPerDataTrans;
				lastRow = row + rowsPerDataTrans;
				while (row < lastRow)
				{
					data += System.Convert.ToString(m_memory[block].data[row]);
					row++;
				}

				//sending the data trans
				SendData(data, false);
			}
		}

		private void SendAddress(string address)
		{

			int i = address.Length;
			while (i < ADDRESS_LENGTH)
			{
				address = "0" + address;
				i++;
			}

			//creating the address string by adding the header
			address = System.Convert.ToString(ADDRESS_HEADER + address);

			//sending the string
			Write(address);
		}

		private void SendData(string data, bool ackResponse = false)
		{

			//creating the data string by adding the header
			data = System.Convert.ToString(DATA_HEADER + data);

			//sending the string
			Write(data);
		}

		private bool Write(string str)
		{

			//adding the checksum
			byte[] Datos = Encoding.UTF8.GetBytes(str);
			Array.Resize(ref Datos, Datos.Length + 2);
			Datos[Datos.Length - 2] = Convert.ToByte(CalculateChk(str));

			//generating the buffer to write
			Datos[Datos.Length - 1] = Convert.ToByte('\0');

			//writting the string
			int retryTimes = MAX_RETRY_COMMAND;

			while (retryTimes > 0)
			{
				m_SerialPort.Write(Datos, 0, Datos.Length);
				retryTimes--;

				if (ReadAck())
				{
					break;
				}
			}

			return retryTimes > 0;
		}

		private char CalculateChk(string str)
		{

			//calculating the sum of all chars in the string
			int totalSum = 0;
			for (int i = 0; i <= str.Length - 1; i++)
			{
				totalSum += Strings.Asc(str[i]);
			}

			//getting the sum last byte
			totalSum = totalSum & 0xFF;

			//setting the bit 5 to 1, bit 0 is LSB
			//0x20 -> 0010 0000
			totalSum = totalSum | 0x20;

			//returning the chk
			return Strings.Chr(totalSum);
		}

		private bool ReadAck()
		{

			//initializing the read ack to a incorrect value
			char cAck = 'P';

			//trying to read an ACK
			try
			{
				cAck = Strings.ChrW(m_SerialPort.ReadChar());
			}
			catch (Exception)
			{
			}

			return cAck == ACK;
		}

		private bool Read(int strLength, char header)
		{

			//add 1 byte for chk
			int bytesToRead = strLength + 1;

			bool success = false;
			bool headerFound = false;
			int retryTimes = MAX_RETRY_COMMAND;

			while (retryTimes > 0 && !success)
			{
				retryTimes--;

				//check header
				if (!headerFound)
				{

					//read char
					byte readedByte = (byte)(m_SerialPort.ReadByte());

					//header found
					if (Convert.ToChar(readedByte) == header)
					{
						headerFound = true;
					}

					//data
				}
				else
				{
					byte[] dataBytes = new byte[bytesToRead + 1];
					dataBytes[0] = Convert.ToByte(header);
					byte[] dataIn = RoutinesLibrary.IO.SerialPort.ReadBytesFromPort(m_SerialPort, bytesToRead);
					Buffer.BlockCopy(dataIn, 0, dataBytes, 1, dataIn.Length);

					//Checksum
					if (VerifyChk(dataBytes))
					{
						success = true;
					}
					else
					{
						headerFound = false;
					}
				}
			}

			//checking the read operation result
			if (success)
			{

				//comunication has been succesfull, sending ACK
				m_SerialPort.Write(ACK.ToString());
			}

			return success;
		}

		private bool VerifyChk(byte[] dataBytes)
		{
			bool chkValid = false;

			//check length
			if (dataBytes.Length > 0)
			{

				//calculating the sum of all chars in the string
				int totalSum = 0;
				for (var i = 0; i <= dataBytes.Length - 2; i++)
				{
					totalSum += dataBytes[i];
				}

				//getting the sum last byte
				//setting the bit 5 to 1, bit 0 is LSB (0x20 -> 0010 0000)
				char chk = Strings.ChrW(System.Convert.ToInt32((totalSum & 0xFF) | 0x20));
				chkValid = Convert.ToChar(dataBytes[dataBytes.Length - 1]) == chk;
			}

			return chkValid;
		}

	}
}
