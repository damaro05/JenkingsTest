// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Net.NetworkInformation;

namespace RoutinesJBC
{
    /// <summary>
    /// Clase para generar identificadores únicos para las estaciones
    /// Existen 4 métodos:
    /// 1) Basado en la MAC de la tarjeta del Station Controller (12 caracteres) más un número adicional de 8 caracteres, con un total de 20 caracteres ASCII (32-127),
    /// que puede pasarse en la generación o si se pasa cero, se genera un número aleatorio formado por
    /// month (1byte), day (1byte), hour (1byte), minutes (1byte), seconds (1byte) and miliseconds (string 3), todos en el rango ascii de 32-127
    /// 2) Basado en un GUID con formato xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx y formato para la estación de 16 bytes en rango 0-255
    /// (soportado a partir de la estación PS)
    /// 3) Basado en un GUID con formato xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx y mismo formato para la estación sin los guiones, de 32 bytes ASCII (en rango 32-127)
    /// 4) Personalizado de 20 bytes, donde se añaden 2 bytes en la primera posición que indica que es personalizado
    /// quedando 18 para el código personalizado
    /// En el caso de GUID (16 o 32 bytes)
    /// Se mantiene con formato string: xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx - total 8+4+4+4+12+4hyphen = 36 characters
    /// En la estación se graban bytes: 16 Bytes in 00-FF range o 32 bytes en rango 32-127
    /// En el caso de MAC (20 bytes)
    /// Se mantiene con formato string: 0080C78F6C96+XXXXXXXX (XXXXXXXX is sequence number) - total 12+8=20
    /// En la estación se graban bytes: 0080C78F6C96+XXXXXXXX - total: 12+8=20 (Bytes in 32-127 ascii range)
    /// Resumen:
    /// 16 bytes es GUID en rango 0-255
    /// 32 bytes es GUID en rango 32-127
    /// 20 bytes puede ser MAC+secuencia en rango 32-127 o personalizado si comienza con el prefijo "##"
    /// </summary>
    /// <remarks></remarks>
    public class clsStationUID
    {

        //GUID info
        //Version 4 (random)
        //Version 4 UUIDs use a scheme relying only on random numbers.
        //This algorithm sets the version number (4 bits) as well as two reserved bits.
        //All other bits (the remaining 122 bits) are set using a random or pseudorandom data source.
        //Version 4 UUIDs have the form xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx
        //where x is any hexadecimal digit and y is one of 8, 9, A, or B (e.g., f47ac10b-58cc-4372-a567-0e02b2c3d479).
        // Guid (Constructor) (String)
        //Cadena que contiene un identificador GUID en uno de los siguientes formatos ("d" representa un dígito hexadecimal sin distinción de mayúsculas y minúsculas):
        //32 dígitos contiguos:
        //dddddddddddddddddddddddddddddddd
        //O bien
        //Grupos de 8, 4, 4, 4 y 12 dígitos con guiones entre los grupos.Todo el identificador GUID puede encerrarse de forma opcional entre llaves o paréntesis:
        //dddddddd-dddd-dddd-dddd-dddddddddddd
        //O bien
        //{dddddddd-dddd-dddd-dddd-dddddddddddd}
        //O bien
        //(dddddddd-dddd-dddd-dddd-dddddddddddd)


        public enum enumType
        {
            EMPTY,
            GUIDS, // GUIDS
            GUIDB, // GUIDB
            MAC,
            CUSTOM,
            UNKNOWN
        }

        //Private Const PrefixGUID As String = "%"
        //Private Const PrefixMAC As String = "!"
        private const string PrefixCUSTOM = "##";
        private const int iMACSequenceNumberLength = 8;

        private string m_UID = "";
        private enumType m_Type = enumType.EMPTY;

        public string UID
        {
            get
            {
                return toMemory();
            }
            set
            {
                fromMemory(value);
            }
        }

        public byte[] StationData
        {
            get
            {
                return toStationBytes().ToArray();
            }
            set
            {
                fromStationBytes(value);
            }
        }

        public enumType Type
        {
            get
            {
                return m_Type;
            }
        }

        /// <summary>
        /// Creates object with an empty station UID
        /// </summary>
        /// <remarks></remarks>
        public clsStationUID()
        {
            m_Type = enumType.EMPTY;
            m_UID = "";
        }

        /// <summary>
        /// Creates object and builds a new MAC (NIC MAC Address) or GUID station UID
        /// </summary>
        /// <param name="type">MAC or GUID type</param>
        /// <param name="iSequence">Optional, for MAC type. If zero, sequence will be based on
        /// month (1byte), day (1byte), hour (1byte), minutes (1byte), seconds (1byte) and miliseconds (string)</param>
        /// <remarks></remarks>
        public clsStationUID(enumType type, int iSequence = 0)
        {
            switch (type)
            {
                case enumType.GUIDB:
                    NewGUIDB();
                    break;
                case enumType.GUIDS:
                    NewGUIDS();
                    break;
                case enumType.MAC:
                    NewMAC(iSequence);
                    break;
                case enumType.CUSTOM:
                    m_Type = enumType.EMPTY;
                    m_UID = "";
                    break;
                default:
                    m_Type = enumType.EMPTY;
                    m_UID = "";
                    break;
            }
        }

        /// <summary>
        /// Creates object and builds a new custom station UID
        /// </summary>
        /// <param name="sCustomUID">Custom UID data</param>
        /// <param name="iSequence">Optional, it will be appended to UID if greater than zero</param>
        /// <remarks></remarks>
        public clsStationUID(string sCustomUID, int iSequence)
        {
            if (iSequence < 0)
            {
                iSequence = 0;
            }
            NewCustomUID(sCustomUID, iSequence);
        }

        /// <summary>
        /// Creates object with already created UID from memory
        /// </summary>
        /// <param name="sUid">UID from memory</param>
        /// <remarks></remarks>
        public clsStationUID(string sUid)
        {
            fromMemory(sUid);
        }

        /// <summary>
        /// Creates object with UID data from station bytes
        /// </summary>
        /// <param name="aUid">UID from station as bytes</param>
        /// <remarks></remarks>
        public clsStationUID(byte[] aUid)
        {
            fromStationBytes(aUid);
        }

        /// <summary>
        /// Creates a UID based on a GUID
        /// Memory format: xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx (8+4+4+4+12+4hyphen = 36 characters)
        /// Station format: GUID converted to 16 bytes on range 0-255
        /// </summary>
        /// <remarks></remarks>
        public void NewGUIDB()
        {
            Guid g = Guid.NewGuid();
            m_UID = g.ToString();
            m_Type = enumType.GUIDB;
        }

        /// <summary>
        /// Creates a UID based on a GUID
        /// Memory format: xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx (8+4+4+4+12+4hyphen = 32 characters)
        /// Station format: 32 ASCII bytes on range 32-127 (36 memory format without hyphens)
        /// </summary>
        /// <remarks></remarks>
        public void NewGUIDS()
        {
            Guid g = Guid.NewGuid();
            m_UID = g.ToString().Replace("-", "");
            m_Type = enumType.GUIDS;
        }

        /// <summary>
        /// Creates a UID based on NIC MAC address (format: 0080C78F6C96) plus a sequence number of length 7
        /// Format with sequence number: prefix+0080C78F6C96+0000000 (0000000 is sequence number) - total: 1+12+7=20
        /// If sequence number = 0, then sequence number is built based on month+day (1byte), hour (1byte), minutes (1byte), seconds (1byte) and miliseconds (string)
        /// Format with generated number: prefix+0080C78F6C96+BBBB000 - total: 1+12+7=20 (where B is Byte in 32-127 range)
        ///  byte of month+day(12-1+31-1=41) + 32 = ascii range 32-73
        ///  byte of hour(23) + 32 = ascii range 32-55
        ///  byte of min(59) + 32 = ascii range 32-91
        ///  byte of sec(59) + 32 = ascii range 32-91
        ///  string of milisec(999) = 000-999
        /// </summary>
        /// <param name="iSequence"></param>
        /// <remarks></remarks>
        public void NewMAC(int iSequence = 0)
        {
            string nicId = "";
            string sMAC = GetMacAddress(ref nicId);
            string sSequence = "";
            List<byte> aSequence = new List<byte>();
            //Dim nowdt As New DateTime(Now.Year, 12, 31, 23, 59, 59, 999)
            DateTime nowdt = DateTime.Now;
            if (iSequence == 0)
            {
                aSequence.Add((byte)(nowdt.Month - 1 + 32));
                aSequence.Add((byte)(nowdt.Day - 1 + 32));
                aSequence.Add((byte)(nowdt.Hour + 32));
                aSequence.Add((byte)(nowdt.Minute + 32));
                aSequence.Add((byte)(nowdt.Second + 32));
                aSequence.AddRange(System.Text.Encoding.UTF8.GetBytes(nowdt.Millisecond.ToString().PadLeft(3, '0')));
            }
            else
            {
                aSequence.AddRange(System.Text.Encoding.UTF8.GetBytes(iSequence.ToString().PadLeft(iMACSequenceNumberLength, '0')));
            }
            m_UID = sMAC + System.Text.Encoding.UTF8.GetString(aSequence.ToArray());
            m_Type = enumType.MAC;
        }

        /// <summary>
        /// Custom UID
        /// </summary>
        /// <param name="sUid"></param>
        /// <remarks></remarks>
        public void NewCustomUID(string sUid, int iSequence = 0)
        {
            if (iSequence > 0)
            {
                m_UID = PrefixCUSTOM + sUid + iSequence.ToString();
            }
            else
            {
                m_UID = PrefixCUSTOM + sUid;
            }
            m_Type = enumType.CUSTOM;
        }

        /// <summary>
        /// Used to write the UID to the station
        /// For GUIDB returns GUID.ToByteArray in format 16bytes values: 00-FF
        /// For GUIDS returns GUID.ToString in 32 bytes, without hyphens, values: 32-127
        /// For MAC returns bytes of MAC+Seq, as format 0080C78F6C96+XXXXXXXX (total 20 bytes) values: 32-127 ASCII
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private List<byte> toStationBytes()
        {
            List<byte> bytes = new List<byte>();

            string sPrefix = "";
            string sUID = "";

            switch (m_Type)
            {
                case enumType.GUIDB:
                    // sUID is a GUID
                    // returns GUID.ToByteArray in 16 bytes, values: 0-255
                    sUID = m_UID;
                    Guid g = new Guid(sUID);
                    // guid
                    bytes.AddRange(g.ToByteArray());
                    return bytes;

                case enumType.GUIDS:
                    // sUID is a GUID
                    // returns GUID.ToString in 32 bytes, without hyphens, values: 32-127
                    sUID = m_UID.Replace("-", "");
                    bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(sUID));
                    return bytes;

                case enumType.MAC:
                    // sUID is a NIC MAC + a sequence number
                    // returns bytes of MAC+Seq in string format 0080C78F6C96+XXXXXXXX (total 20 bytes) values: 32-127 ASCII
                    sUID = m_UID;
                    // guid
                    bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(sUID));
                    return bytes;

                case enumType.CUSTOM:
                    // sUID is not a GUID nor MAC
                    bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(m_UID));
                    return bytes;

                case enumType.EMPTY:
                    return bytes;

                default:
                    if (m_UID.Length > 0)
                    {
                        bytes.AddRange(System.Text.Encoding.UTF8.GetBytes(m_UID));
                    }
                    return bytes;
            }

            return bytes;

        }

        /// <summary>
        /// Used to build the UID based on bytes read from station
        /// </summary>
        /// <param name="stationUID"></param>
        /// <remarks></remarks>
        private void fromStationBytes(byte[] stationUID)
        {
            m_UID = "";
            m_Type = enumType.EMPTY;

            if (stationUID.Length < 2)
            {
                return;
            }

            // UID
            List<byte> aUID = new List<byte>();
            // custom prefix
            byte[] aPrefix = new byte[PrefixCUSTOM.Length - 1 + 1];
            Array.Copy(stationUID, 0, aPrefix, 0, aPrefix.Length);
            string sPrefix = System.Text.Encoding.UTF8.GetString(aPrefix);

            if (sPrefix == PrefixCUSTOM)
            {
                m_UID = System.Text.Encoding.UTF8.GetString(stationUID);
                m_Type = enumType.CUSTOM;
            }
            else
            {
                switch (stationUID.Length)
                {
                    case 16:
                        // build GUID based on bytes
                        aUID.AddRange(stationUID);
                        Guid g = new Guid(aUID.ToArray());
                        m_UID = g.ToString();
                        m_Type = enumType.GUIDB;
                        break;

                    case 32:
                        // build GUID based on ASCII bytes (without hyphens)
                        // xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx
                        string sGUID = System.Text.Encoding.UTF8.GetString(stationUID);
                        //For i = 1 To sGUID.Length
                        //    Select Case i
                        //        Case 9, 13, 17, 21
                        //            m_UID += "-"
                        //        Case Else
                        //    End Select
                        //    m_UID += Mid(sGUID, i, 1)
                        //Next
                        m_UID = sGUID;
                        m_Type = enumType.GUIDS;
                        break;

                    case 20:
                        // UID is of format 0080C78F6C96+seq
                        m_UID = System.Text.Encoding.UTF8.GetString(stationUID);
                        m_Type = enumType.MAC;
                        break;

                    default:
                        m_UID = System.Text.Encoding.UTF8.GetString(stationUID);
                        m_Type = enumType.UNKNOWN;
                        break;

                }

            }


        }

        private string toMemory()
        {
            return m_UID;
        }

        private void fromMemory(string memoryUID)
        {
            m_UID = "";
            m_Type = enumType.EMPTY;

            if (memoryUID.Length < 2)
            {
                return;
            }

            // UID
            List<byte> aUID = new List<byte>();
            // custom prefix
            string sPrefix = memoryUID.Substring(0, PrefixCUSTOM.Length);

            if (sPrefix == PrefixCUSTOM)
            {
                m_UID = memoryUID;
                m_Type = enumType.CUSTOM;
            }
            else
            {
                switch (memoryUID.Length)
                {
                    case 36:
                        // build GUID based on bytes
                        // xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx
                        m_UID = memoryUID;
                        m_Type = enumType.GUIDB;
                        break;

                    case 32:
                        // build GUID based on bytes
                        // xxxxxxxxxxxx4xxxyxxxxxxxxxxxxxxx
                        m_UID = memoryUID;
                        m_Type = enumType.GUIDS;
                        break;

                    case 20:
                        // UID is of format 0080C78F6C96+seq
                        m_UID = memoryUID;
                        m_Type = enumType.MAC;
                        break;

                    default:
                        m_UID = memoryUID;
                        m_Type = enumType.UNKNOWN;
                        break;

                }
            }

        }

        /// <summary>
        /// Finds the MAC address of the NIC with maximum speed.
        /// </summary>
        /// <return>The MAC address.</return>
        private string GetMacAddress(ref string sNicId)
        {

            const int MIN_MAC_ADDR_LENGTH = 12;
            string macAddress = string.Empty;
            long maxSpeed = -1;
            sNicId = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string tempMac = System.Convert.ToString(nic.GetPhysicalAddress().ToString());
                if (nic.Speed > maxSpeed && !string.IsNullOrEmpty(tempMac) && tempMac.Length >= MIN_MAC_ADDR_LENGTH)
                {
                    maxSpeed = nic.Speed;
                    sNicId = nic.Id;
                    macAddress = tempMac;
                }
            }
            return macAddress;
        }

    }
}
