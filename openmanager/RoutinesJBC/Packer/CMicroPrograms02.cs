// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace RoutinesJBC
{
    public class CMicroPrograms02
    {

        public const int BLOCK_MICRO_PROGRAM = 128;


        private class dataBlock
        {
            public int m_Address;
            public byte[] m_Data;

            public dataBlock(int addr, byte[] data)
            {
                m_Address = addr;
                m_Data = data;
            }
        }


        public class s19rec
        {
            public char type;
            public int length;
            public List<byte> address;
            public List<byte> data;
            public List<byte> checksum;
        }

        private List<string> m_aS19Data;
        private List<string> m_aConvertedToS19Data;
        private int m_currentIdx = -1;


        /// <summary>
        /// Class constructor
        /// </summary>
        public CMicroPrograms02()
        {
            m_aS19Data = new List<string>();
            m_aConvertedToS19Data = new List<string>();
        }

        /// <summary>
        /// Load a Hex or S19 program file and convert it to S19 type
        /// </summary>
        /// <param name="sFilename">Path to program file</param>
        /// <param name="sError">Error description if exists</param>
        /// <returns>True if the operation was succesfull</returns>
        public bool Load(string sFilename, ref string sError)
        {
            string[] aData = null;

            // read file
            try
            {
                aData = System.IO.File.ReadAllLines(sFilename, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }

            return LoadFromData(ref aData, ref sError);
        }

        public bool LoadFromData(ref string[] aData, ref string sError)
        {
            bool bConvert = false;
            sError = "";

            // convert to uppercase
            for (var i = 0; i <= aData.Length - 1; i++)
            {
                aData[i] = aData[i].ToUpper();
            }

            // check if it S19 or Hex
            if (aData[0][0] == ':')
            {
                // Hex format -> convert to S19 and sort
                bConvert = true;
            }
            else if (aData[0][0] == 'S')
            {
                //S19 format -> sort
                bConvert = false;
            }
            else
            {
                sError = string.Format("Program is not Hex nor S19 format");
                return false;
            }

            // convert hex to S19
            string[] aLoadedData = null;
            if (bConvert)
            {
                aLoadedData = new string[0];
                if (!myConvertHexToS19(aData, ref aLoadedData, ref sError))
                {
                    return false;
                }
            }
            else
            {
                aLoadedData = new string[aData.Length];
                aData.CopyTo(aLoadedData, 0);
            }

            // add data program sorted
            m_aS19Data.Clear();
            m_aS19Data.AddRange(SortS19(aLoadedData));

            // save converted and sorted for exporting
            m_aConvertedToS19Data.Clear();
            m_aConvertedToS19Data.AddRange(m_aS19Data.ToArray());

            string[] aConvertedData = ConvertS19Data(BLOCK_MICRO_PROGRAM);
            m_aS19Data.Clear();
            m_aS19Data.AddRange(SortS19(aConvertedData));

            return true;
        }

        /// <summary>
        /// Save the S19 program into a file
        /// </summary>
        /// <param name="sFilename">Path to program file</param>
        /// <param name="sError">Error description if exists</param>
        /// <returns>True if the operation was succesfull</returns>
        public bool Save(string sFilename, ref string sError)
        {
            sError = "";

            try
            {
                System.IO.File.WriteAllLines(sFilename, m_aS19Data.ToArray(), System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }

            return true;
        }

        public bool SaveConvertedAndSorted(string sFilename, ref string sError)
        {
            sError = "";

            try
            {
                System.IO.File.WriteAllLines(sFilename, m_aConvertedToS19Data.ToArray(), System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the S19 program
        /// </summary>
        /// <returns>S19 program</returns>
        public string[] GetAllData()
        {
            return m_aS19Data.ToArray();
        }

        /// <summary>
        /// Prepare to a update process
        /// </summary>
        /// <returns>Data record count</returns>
        public int initUpdaterData()
        {
            m_currentIdx = -1;
            return dataRecordCount();
        }

        /// <summary>
        /// Get the data type record count
        /// </summary>
        /// <returns>Data type record count</returns>
        public int dataRecordCount()
        {
            int icount = 0;

            // count records to be returned
            for (var i = 0; i <= m_aS19Data.Count - 1; i++)
            {
                if (checkDataType(getS19RecType(System.Convert.ToInt32(i))))
                {
                    icount++;
                }
            }

            return icount;
        }

        /// <summary>
        /// Get the total record count
        /// </summary>
        /// <returns>Total record count</returns>
        public int recordCount()
        {
            return m_aS19Data.Count;
        }

        /// <summary>
        /// Get the next record data
        /// </summary>
        /// <param name="rec">Record data</param>
        /// <returns>True if the operation was succesfull</returns>
        public bool getNextUpdaterData(ref s19rec rec)
        {
            bool bOk = false;

            do
            {
                m_currentIdx++;
                if (m_currentIdx >= m_aS19Data.Count)
                {
                    return false;
                }

                if (checkDataType(getS19RecType(m_currentIdx)))
                {
                    rec = getS19Rec(m_currentIdx);
                    bOk = true;
                }
            } while (!bOk);

            return true;
        }

        /// <summary>
        /// Check if a type is a data type
        /// </summary>
        /// <param name="type">Type to be checked</param>
        /// <returns>True if is a data type</returns>
        private bool checkDataType(char type)
        {
            switch (type)
            {
                case '1':
                case '2':
                case '3':
                    return true;
                case '7':
                case '8':
                case '9':
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get record type at a specific line
        /// </summary>
        /// <param name="idx">Record line index</param>
        /// <returns>Record type</returns>
        private char getS19RecType(int idx)
        {
            return m_aS19Data[idx][1];
        }

        /// <summary>
        /// Get record at a specific line
        /// </summary>
        /// <param name="idx">Record line index</param>
        /// <returns>Record</returns>
        private s19rec getS19Rec(int idx)
        {
            s19rec rec = new s19rec();
            int lengthHex = 0;
            int addressBytes = 0;
            int posDataHex = 0;
            int dataLengthHex = 0;

            // rec type (0 to 9)
            rec.type = getS19RecType(idx);

            // qty of bytes of address + data + checksum fields
            rec.length = int.Parse(System.Convert.ToString(m_aS19Data[idx].Substring(2, 2)), System.Globalization.NumberStyles.AllowHexSpecifier);
            lengthHex = rec.length * 2;

            switch (rec.type)
            {
                case '0':
                    addressBytes = 2;
                    break;
                case '1':
                    addressBytes = 2; // data address 16 bit
                    break;
                case '2':
                    addressBytes = 3; // data address 24 bit
                    break;
                case '3':
                    addressBytes = 4; // data address 32 bit
                    break;
                case '5':
                    addressBytes = 2; // S1+S2+S3 record count
                    break;
                case '7':
                    addressBytes = 4; // starting address of the program 32 bit
                    break;
                case '8':
                    addressBytes = 3; // starting address of the program 24 bit
                    break;
                case '9':
                    addressBytes = 2; // starting address of the program 16 bit
                    break;
            }

            //Address
            rec.address = convHexToByteArray(System.Convert.ToString(m_aS19Data[idx].Substring(4, addressBytes * 2)));

            //Posición inicial de data
            posDataHex = 2 + 2 + (addressBytes * 2); // Sx + length(2) + address hex length

            //Longitud de data
            dataLengthHex = lengthHex - (addressBytes * 2) - 2; // total hex length - address hex length - checksum(2)

            //Data
            if (dataLengthHex > 0)
            {
                rec.data = convHexToByteArray(System.Convert.ToString(m_aS19Data[idx].Substring(posDataHex, dataLengthHex)));
            }
            else
            {
                rec.data = new List<byte>();
            }

            //Checksum
            rec.checksum = convHexToByteArray(System.Convert.ToString(m_aS19Data[idx].Substring(posDataHex + dataLengthHex, 2)));

            return rec;
        }

        /// <summary>
        /// Convert hexadecimal string to a byte array
        /// </summary>
        /// <param name="sHexPairs">Hexadecimal string</param>
        /// <returns>List of byte</returns>
        private List<byte> convHexToByteArray(string sHexPairs)
        {
            List<byte> bytes = new List<byte>();

            for (var i = 0; i <= sHexPairs.Length - 2; i += 2)
            {
                bytes.Add(Convert.ToByte(sHexPairs.Substring(System.Convert.ToInt32(i), 2), 16));
            }

            return bytes;
        }

        /// <summary>
        /// Convert byte array to hexadecimal string
        /// </summary>
        /// <param name="_array">List of byte</param>
        /// <returns>Hexadecimal string</returns>
        private string convBytesToHexa(byte[] _array)
        {
            string sHexa = "";

            for (var i = 0; i <= _array.Length - 1; i++)
            {
                sHexa += System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
            }

            return sHexa;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="_array"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private string convBytesToHexaEdited(byte[] _array)
        {
            string sHexa = "";

            for (var i = 0; i <= _array.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(sHexa))
                {
                    sHexa += "-";
                }
                sHexa += System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
            }

            return sHexa;
        }

        /// <summary>
        /// Sort a S19 program
        /// </summary>
        /// <param name="sSource">S19 program to be sort</param>
        /// <returns>Sorted program</returns>
        private string[] SortS19(string[] sSource)
        {
            List<string> sSortTarget = new List<string>();
            List<string> sTarget = new List<string>();

            // build array to sort
            const string sSortSep = "#";
            string sRecForSort = "";

            foreach (string Str in sSource)
            {
                sRecForSort = Str.Substring(0, 2) + Str.Substring(4, 8) + sSortSep + Str;
                sSortTarget.Add(sRecForSort);
            }

            sSortTarget.Sort();
            sTarget.Clear();

            foreach (string Str in sSortTarget)
            {
                sRecForSort = Str.Substring(Str.IndexOf(sSortSep) + 1 + sSortSep.Length - 1);
                sTarget.Add(sRecForSort);
            }

            return sTarget.ToArray();
        }

        /// <summary>
        /// Convert an integer to byte array
        /// </summary>
        /// <param name="address">Integer to convert</param>
        /// <param name="dataInBigEndian">True if the final byte array must be in Big Endian</param>
        /// <returns>Converted integer</returns>
        public byte[] intToBytesAddress(int address, bool dataInBigEndian)
        {
            List<byte> correctBytes = new List<byte>();

            correctBytes.AddRange(BitConverter.GetBytes(System.Convert.ToInt32(address)));

            // GetBytes returns BigEndian or LittleEndian depending on BitConverter.IsLittleEndian
            if ((BitConverter.IsLittleEndian && dataInBigEndian) || (!BitConverter.IsLittleEndian && !dataInBigEndian))
            {
                correctBytes.Reverse();
            }

            return correctBytes.ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="address"></param>
        /// <param name="bytesAreBigEndian"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public byte[] getAddressBytesOrder(byte[] address, bool bytesAreBigEndian)
        {
            List<byte> correctBytes = new List<byte>();

            correctBytes.AddRange(address);

            if ((BitConverter.IsLittleEndian && bytesAreBigEndian) || (!BitConverter.IsLittleEndian && !bytesAreBigEndian))
            {
                correctBytes.Reverse();
            }

            return correctBytes.ToArray();
        }

        public string[] ConvertS19Data(int iLengthDataBlock)
        {
            List<string> sTarget = new List<string>();
            int icount = recordCount();
            s19rec rec = default(s19rec);
            int recDecimalAddress = 0;
            int recDataLength = 0;

            List<dataBlock> listDataBlock = new List<dataBlock>();
            List<dataBlock> listAlignedDataBlock = new List<dataBlock>();
            List<byte> currentBlockData = new List<byte>();
            int currentBlockAddress = 0;
            //Dim prevBlockData As New List(Of Byte)
            //Dim prevBlockAddress As Integer = 0
            int nextStartAddress = 0;
            List<byte> bytesTemp = new List<byte>();

            for (var i = 0; i <= icount - 1; i++)
            {
                rec = getS19Rec(System.Convert.ToInt32(i));

                switch (rec.type)
                {
                    case '0':
                        myAddS19Record(sTarget, rec.type, convBytesToHexa(rec.address.ToArray()), convBytesToHexa(rec.data.ToArray()));
                        break;
                    case '1':
                        break;

                    case '2':
                        break;

                    case '3':

                        //dirección y longitud del bloque de datos
                        recDecimalAddress = RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(rec.address.ToArray(), true); //is BigEndian
                        recDataLength = rec.data.Count;

                        //address gap
                        if (recDecimalAddress > nextStartAddress)
                        {

                            //new block. recording cumulative data
                            if (currentBlockData.Count > 0)
                            {
                                dataBlock block = new dataBlock(currentBlockAddress, currentBlockData.ToArray());
                                listDataBlock.Add(block);
                            }

                            //accumulates new data
                            currentBlockData.Clear();
                            currentBlockData.AddRange(rec.data.ToArray());
                            currentBlockAddress = recDecimalAddress;

                            //no gap
                        }
                        else
                        {
                            //add to current block
                            currentBlockData.AddRange(rec.data.ToArray());
                        }

                        //calculate next address, to control gaps
                        nextStartAddress = recDecimalAddress + recDataLength;
                        break;

                    case '5':
                        break;

                    case '7':
                    case '8':
                    case '9':
                        break;

                }
            }

            //add remaining data in last block
            if (currentBlockData.Count > 0)
            {
                dataBlock block = new dataBlock(currentBlockAddress, currentBlockData.ToArray());
                listDataBlock.Add(block);
            }


            //'Align all directions to 128 (iLengthDataBlock)
            //For i = 0 To listDataBlock.Count - 1
            //    If listDataBlock.Item(i).m_Address Mod iLengthDataBlock <> 0 Then

            //        'Calculamos el gap inicial
            //        Dim nInitialGap As Integer = listDataBlock.Item(i).m_Address - ((listDataBlock.Item(i).m_Address \ 128) * 128)

            //        'Rellenamos los datos iniciales con FF
            //        Dim alignData(listDataBlock.Item(i).m_Data.Length + nInitialGap - 1) As Byte
            //        setArrayTo(alignData, &HFF)
            //        Array.Copy(listDataBlock.Item(i).m_Data, 0, alignData, nInitialGap, listDataBlock.Item(i).m_Data.Length)

            //        'Actualizamos el array de datos
            //        listDataBlock.Item(i).m_Data = alignData

            //        'Actualizamos el address
            //        listDataBlock.Item(i).m_Address -= nInitialGap
            //    End If
            //Next

            //Align all directions to 128

            int lastAlignedDataBlock = -1;

            for (var i = 0; i <= listDataBlock.Count - 1; i++)
            {

                lastAlignedDataBlock = listAlignedDataBlock.Count - 1;

                if (listDataBlock[System.Convert.ToInt32(i)].m_Address % iLengthDataBlock != 0)
                {

                    //If listDataBlock.Item(i).m_Address > 486635784 Then
                    //    MsgBox("> 486635784")
                    //End If

                    int nInitialGap = 0;
                    int nEndGap = 0;
                    byte[] alignData = null;
                    int nCopyDataLength = 0;
                    int nTargetBufferToCopyLength = 0;

                    // ver si la dirección está dentro del bloque alineado anterior (múltiplo de 128)
                    bool bAlignBlock = true;
                    if (i > 0 & lastAlignedDataBlock >= 0)
                    {
                        int iPreviousBlockAddress = System.Convert.ToInt32(listAlignedDataBlock[lastAlignedDataBlock].m_Address);
                        int iPreviousBlockDataLength = System.Convert.ToInt32(listAlignedDataBlock[lastAlignedDataBlock].m_Data.Length);
                        int iPreviousBlockAlignedBlocks = iPreviousBlockDataLength / iLengthDataBlock;
                        if (iPreviousBlockDataLength / iLengthDataBlock != (double)iPreviousBlockDataLength / iLengthDataBlock)
                        {
                            iPreviousBlockAlignedBlocks++;
                        }
                        if (listDataBlock[System.Convert.ToInt32(i)].m_Address < iPreviousBlockAddress + (iPreviousBlockAlignedBlocks * iLengthDataBlock))
                        {
                            // copiar parte de los datos al bloque anterior
                            // primero el gap entre el fin de datos y la nueva dirección
                            nEndGap = System.Convert.ToInt32(listDataBlock[System.Convert.ToInt32(i)].m_Address - (iPreviousBlockAddress + iPreviousBlockDataLength));
                            // espacio que tengo para copiar = el total alineado menos los datos existentes y el gap añadido
                            nTargetBufferToCopyLength = (iPreviousBlockAlignedBlocks * iLengthDataBlock) - (iPreviousBlockDataLength + nEndGap);
                            // datos a copiar: mínimo entre espacio destino y datos origen
                            nCopyDataLength = Math.Min(nTargetBufferToCopyLength, System.Convert.ToInt32(listDataBlock[System.Convert.ToInt32(i)].m_Data.Length));
                            //Rellenamos los datos iniciales con FF
                            alignData = new byte[nEndGap + nCopyDataLength - 1 + 1];
                            setArrayTo(ref alignData, (byte)(0xFF));
                            Array.Copy((System.Array)(listDataBlock[System.Convert.ToInt32(i)].m_Data), 0, alignData, nEndGap, nCopyDataLength);
                            // copiar a bloque anterior
                            bytesTemp.Clear();
                            bytesTemp.AddRange(listAlignedDataBlock[lastAlignedDataBlock].m_Data);
                            bytesTemp.AddRange(alignData);
                            listAlignedDataBlock[lastAlignedDataBlock].m_Data = bytesTemp.ToArray();
                            // quitar estos datos del bloque actual y modificar address
                            bytesTemp.Clear();
                            bytesTemp.AddRange(listDataBlock[System.Convert.ToInt32(i)].m_Data);
                            bytesTemp.RemoveRange(0, nCopyDataLength);
                            if (bytesTemp.Count > 0)
                            {
                                listDataBlock[System.Convert.ToInt32(i)].m_Data = bytesTemp.ToArray();
                                // modificar address
                                listDataBlock[System.Convert.ToInt32(i)].m_Address = listDataBlock[System.Convert.ToInt32(i)].m_Address + nCopyDataLength;
                            }
                            else
                            {
                                bAlignBlock = false;
                            }
                        }
                    }

                    if (bAlignBlock)
                    {

                        //Calculamos el gap inicial
                        nInitialGap = System.Convert.ToInt32(listDataBlock[System.Convert.ToInt32(i)].m_Address - ((listDataBlock[System.Convert.ToInt32(i)].m_Address / iLengthDataBlock) * iLengthDataBlock));

                        //Rellenamos los datos iniciales con FF
                        alignData = new byte[listDataBlock[System.Convert.ToInt32(i)].m_Data.Length + nInitialGap - 1 + 1];
                        setArrayTo(ref alignData, (byte)(0xFF));
                        Array.Copy((System.Array)(listDataBlock[System.Convert.ToInt32(i)].m_Data), 0, alignData, nInitialGap, System.Convert.ToInt32(listDataBlock[System.Convert.ToInt32(i)].m_Data.Length));

                        //Actualizamos el array de datos
                        listDataBlock[System.Convert.ToInt32(i)].m_Data = alignData;

                        //Actualizamos el address
                        listDataBlock[System.Convert.ToInt32(i)].m_Address -= nInitialGap;

                        // add aligned block
                        listAlignedDataBlock.Add(listDataBlock[System.Convert.ToInt32(i)]);
                    }

                }
                else
                {
                    // add block, already aligned
                    listAlignedDataBlock.Add(listDataBlock[System.Convert.ToInt32(i)]);
                }
            }

            // build SREC in iDataBlock format
            byte[] newData = null;
            newData = new byte[iLengthDataBlock - 1 + 1];

            int iCopyLength = 0;
            int iDataLengthRemainder = 0;
            int iAddress = 0;

            int nextStartByte = 0;
            int iBlockNumber = 0;
            bool bEnd = false;


            //recorrer todos los bloques de datos
            //For i = 0 To listDataBlock.Count - 1
            for (var i = 0; i <= listAlignedDataBlock.Count - 1; i++)
            {

                //dirección y datos del bloque de datos
                //currentBlockAddress = listDataBlock.Item(i).m_Address
                currentBlockAddress = System.Convert.ToInt32(listAlignedDataBlock[System.Convert.ToInt32(i)].m_Address);
                currentBlockData.Clear();
                //currentBlockData.AddRange(listDataBlock.Item(i).m_Data)
                currentBlockData.AddRange(listAlignedDataBlock[System.Convert.ToInt32(i)].m_Data);

                //If currentBlockAddress > 486635784 Then
                //    MsgBox("> 486635784")
                //End If

                nextStartByte = 0;
                iBlockNumber = 0;
                bEnd = false;

                //loop por el bloque para dividir en direcciones de 128
                do
                {
                    //determinar cuanto queda por copiar
                    iDataLengthRemainder = currentBlockData.Count - nextStartByte;

                    if (iDataLengthRemainder > 0)
                    {

                        //copiamos un bloque entero de datos o el resto
                        if (iDataLengthRemainder > iLengthDataBlock)
                        {
                            iCopyLength = iLengthDataBlock;
                        }
                        else
                        {
                            iCopyLength = iDataLengthRemainder;
                        }

                        //número de bloque a copiar
                        iBlockNumber++;

                        //el buffer de 128 a HFF
                        setArrayTo(ref newData, (byte)(0xFF));

                        //copy data
                        Array.Copy(currentBlockData.ToArray(), nextStartByte, newData, 0, iCopyLength);

                        //la nueva dirección es la dirección base del bloque más la cantidad de bloques copiados
                        iAddress = currentBlockAddress + (iLengthDataBlock * (iBlockNumber - 1));

                        //grabar el bloque
                        myAddS19Record(sTarget, '3', convBytesToHexa(intToBytesAddress(iAddress, true)), convBytesToHexa(newData));

                        //actualizar la siguiente dirección
                        nextStartByte += iCopyLength;
                    }
                    else
                    {
                        bEnd = true;
                    }
                } while (!bEnd);
            }

            return sTarget.ToArray();
        }

        private void setArrayTo(ref byte[] arr, byte value)
        {
            for (var i = 0; i <= arr.Length - 1; i++)
            {
                arr[i] = value;
            }
        }

        #region Convert Hex to S19

        public bool myConvertHexToS19(string[] sSourceData, ref string[] sTargetData, ref string sError)
        {
            bool bConvert = false;

            // Record types written:
            // S0 Block header
            // S3 Data with 32bit address
            // S5 Count of S3 records (optional)
            // S7 End of block for records of type S3
            // 10/04/2014 Convert .hex records to uppercase

            // convert to uppercase
            for (var i = 0; i <= sSourceData.Length - 1; i++)
            {
                sSourceData[i] = sSourceData[i].ToUpper();
            }

            // check if it S19 or Hex
            if (sSourceData[0][0] == ':')
            {
                // Hex format -> convert to S19 and sort
                bConvert = true;
            }
            else if (sSourceData[0][0] == 'S')
            {
                //S19 format -> sort
                bConvert = false;
            }
            else
            {
                sError = string.Format("Data is not Hex nor S19 program data.");
                return false;
            }

            // convert
            uint iDataLength = 0;
            char[] chrByteCount = null;
            char[] chrAddress = null;
            char[] chrRecType = null;
            char[] chrData = null;
            char[] chrCRC = null;
            int iCRCRec = 0;
            int iCRCSum = 0;
            int iCRCCalculated = 0;
            byte[] bytes = null;
            string sDebugMsg = "";
            string sStdMsg = System.Convert.ToString("{0} - Data: '{1}' (line:{2})" + "\r\n");
            char[] charCurrentUpper16BitsAddress = null;
            char[] charCurrent32BitsAddress = null;
            int iS19RecordCount = 0;

            List<string> sTarget = new List<string>();
            List<string> sSortTarget = new List<string>();

            if (bConvert)
            {
                myAddS19Record(sTarget, '0', "0000", "68656C6C6F20202020200000"); // hello

                charCurrentUpper16BitsAddress = new[] { '0', '0', '0', '0' };
                for (var i = 0; i <= sSourceData.Length - 1; i++)
                {
                    if (sSourceData[i].Substring(0, 1) == ":")
                    {
                        // byte count (2 hex) = Data hexs
                        chrByteCount = sSourceData[i].Substring(1, 2).ToCharArray();
                        iDataLength = System.Convert.ToUInt32(Convert.ToUInt16(chrByteCount.ToString(), 16));
                        // address (4 hex)
                        chrAddress = sSourceData[i].Substring(3, 4).ToCharArray();
                        charCurrent32BitsAddress = new char[4];
                        charCurrentUpper16BitsAddress.CopyTo(charCurrent32BitsAddress, 0);
                        chrAddress.CopyTo(charCurrent32BitsAddress, 2);
                        // record type (2 hex)
                        chrRecType = sSourceData[i].Substring(7, 2).ToCharArray();
                        // data
                        chrData = sSourceData[i].Substring(9, (int)(iDataLength * 2)).ToCharArray();
                        // CRC
                        chrCRC = sSourceData[i].Substring(System.Convert.ToInt32(9 + (iDataLength * 2)), 2).ToCharArray();
                        iCRCRec = Convert.ToInt16(chrCRC.ToString(), 16);
                        iCRCSum = 0;
                        int x = 0;
                        for (x = 0; x <= (int)(iDataLength + 4 - 1); x++)
                        {
                            iCRCSum += Convert.ToInt16(System.Convert.ToString(sSourceData[i].Substring((x * 2) + 1, 2)), 16);
                        }

                        // calculate CRC
                        // 1s complement of int = not(int)
                        // 2s complement of int2 = (not(int)) + 1 : int2 = int2 and &HFF
                        // least significant byte
                        bytes = BitConverter.GetBytes(iCRCSum);
                        // 2s complement
                        iCRCCalculated = System.Convert.ToInt32((~(bytes[0])) + 1);
                        iCRCCalculated = iCRCCalculated & 0xFF; // overflow suppression

                        if (iCRCRec != iCRCCalculated)
                        {
                            sError = string.Format("Error in CRC validation in Hex program file. Line number: {0} In record: {1} Calculated: {2}", (i + 1).ToString(), iCRCRec.ToString("X"), iCRCCalculated.ToString("X"));
                            return false;
                        }

                        switch ((chrRecType).ToString())
                        {
                            case "00":
                                // Data record
                                // save 16bit address
                                //myAddS19Record(sTarget, "1", CStr(chrAddress), CStr(chrData))
                                // save 32bit address
                                myAddS19Record(sTarget, '3', (charCurrent32BitsAddress).ToString(), (chrData).ToString());
                                iS19RecordCount++;
                                break;

                            case "01":
                                // End of File record

                                // write S19 record count in address field
                                myAddS19Record(sTarget, '5', iS19RecordCount.ToString("X4"), "");

                                // write S19 end of block record
                                // records type 1
                                //myAddS19Record(sTarget, "9", "0000", "")
                                // records type 3
                                myAddS19Record(sTarget, '7', "00000000", "");
                                goto endOfForLoop;

                            case "02":
                                // Extended Segment Address record
                                sDebugMsg += string.Format(sStdMsg, (chrRecType).ToString(), (chrData).ToString(), (i + 1).ToString());
                                break;
                            case "03":
                                // Start Segment Address record
                                sDebugMsg += string.Format(sStdMsg, (chrRecType).ToString(), (chrData).ToString(), (i + 1).ToString());
                                break;

                            case "04":
                                // Extended Linear Address record
                                sDebugMsg += string.Format(sStdMsg, (chrRecType).ToString(), (chrData).ToString(), (i + 1).ToString());
                                charCurrentUpper16BitsAddress = chrData;
                                break;
                            case "05":
                                // Start Linear Address record
                                sDebugMsg += string.Format(sStdMsg, (chrRecType).ToString(), (chrData).ToString(), (i + 1).ToString());
                                break;

                            default:
                                sError = string.Format("Unknown record type in Hex program file. Line number: {0}", (i + 1).ToString());
                                return false;

                        }

                    }
                }
                endOfForLoop:
                1.GetHashCode(); //VBConversions note: C# requires an executable line here, so a dummy line was added.

            }
            else
            {
                // no conversion
                sTarget.AddRange(sSourceData);

            }

            //sDebugMsg += vbCrLf
            //Dim str As String = "68656C6C6F20202020200000"
            //Dim str2 As String = ""
            //For i = 0 To (str.Length / 2) - 1
            //    str2 += Convert.ToChar(Convert.ToInt16(Mid(str, (i * 2) + 1, 2), 16))
            //Next
            //MsgBox(sDebugMsg + str2)

            sTargetData = new string[sTarget.Count - 1 + 1];
            sTarget.ToArray().CopyTo(sTargetData, 0);
            return true;

        }

        public bool myConvertHexToS19File(string sSourcePathFileName, string sTargetPathFileName, bool bSort, ref string sError)
        {
            string[] sSourceData = System.IO.File.ReadAllLines(sSourcePathFileName, System.Text.Encoding.ASCII);

            string sErr = "";
            string[] sTargetData = null;
            sTargetData = new string[0];
            if (!myConvertHexToS19(sSourceData, ref sTargetData, ref sErr))
            {
                sError = sErr;
                return false;
            }

            if (bSort)
            {
                System.IO.File.WriteAllLines(sTargetPathFileName, SortS19(sTargetData), System.Text.Encoding.ASCII);
            }
            else
            {
                System.IO.File.WriteAllLines(sTargetPathFileName, sTargetData, System.Text.Encoding.ASCII);
            }

            return true;

        }

        public bool myAddS19Record(List<string> sTarget, char sRecType, string sAddressHex, string sDataHex)
        {
            string rec = "";
            // fixed S (1)
            rec += "S";
            // record type (1)
            rec += sRecType.ToString();
            // byte count (that follows): address + data + checksum (2)
            int iByteCount = System.Convert.ToInt32(((double)sAddressHex.Length / 2) + ((double)sDataHex.Length / 2) + 1);
            rec += iByteCount.ToString("X2");
            // address (?)
            rec += sAddressHex;
            // data (?), may be no data
            if (sDataHex.Length > 0)
            {
                rec += sDataHex;
            }

            // CRC (sum of bytes of count + address + data -> least significant byte -> 1's complement)
            int iCRCBytesCount = System.Convert.ToInt32(1 + ((double)sAddressHex.Length / 2) + ((double)sDataHex.Length / 2));
            int iCRCSum = 0;
            for (var x = 0; x <= iCRCBytesCount - 1; x++)
            {
                iCRCSum += Convert.ToInt16(rec.Substring(System.Convert.ToInt32((x * 2) + 2), 2), 16);
            }
            // calculate CRC
            // 1s complement of int = not(int)
            // 2s complement of int2 = (not(int)) + 1 : int2 = int2 and &HFF
            // least significant byte
            byte[] bytes = BitConverter.GetBytes(iCRCSum);
            // 1s complement
            int iCRCCalculated = System.Convert.ToInt32(~bytes[0]);
            rec += iCRCCalculated.ToString("X2");

            sTarget.Add(rec);

            return true;
        }

        #endregion

    }
}
