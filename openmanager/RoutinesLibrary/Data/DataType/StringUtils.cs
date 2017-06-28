using System;
using System.Collections.Generic;

namespace RoutinesLibrary.Data.DataType
{

    public class StringUtils
    {

        public static string ByteArrayToString(byte[] array)
        {
            string sString = "";

            for (var i = 0; i <= array.Length - 1; i++)
            {
                sString += string.Format("{0:X2}", array[i]);
            }

            return sString;
        }

        public static string base64Encode(string sData)
        {
            try
            {
                byte[] encData_byte = new byte[sData.Length - 1 + 1];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(sData);
                string encodedData = Convert.ToBase64String(encData_byte);
                return (encodedData);

            }
            catch (Exception ex)
            {
                throw (new Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error when encoding: " + ex.Message));
            }
        }

        public static string base64Decode(string sData)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecode_byte = Convert.FromBase64String(sData);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount - 1 + 1];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char.ToString().ToCharArray(), 0);
                string result = new String(decoded_char);
                return result;

            }
            catch (Exception ex)
            {
                throw (new Exception(System.Reflection.MethodInfo.GetCurrentMethod().ToString() + " . Error when decoding: " + ex.Message));
            }
        }

        public static string chkSlash(string sText)
        {
            if (sText.Substring(sText.Length - 1, 1) != "\\")
            {
                sText += "\\";
            }
            return sText;
        }

        public static string ByteArrayToBinary(byte[] array)
        {
            string sString = "";

            for (var i = 0; i <= array.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(sString))
                {
                    sString += "-";
                }
                sString += System.Convert.ToString(Convert.ToString(array[i], 2).PadLeft(8, '0'));
            }

            return sString;
        }

        public static string ByteToBinary(byte byt)
        {
            return Convert.ToString(byt, 2).PadLeft(8, '0');
        }

        public static List<byte> HexaToByteArray(string sHexPairs)
        {
            List<byte> bytes = new List<byte>();
            sHexPairs = sHexPairs.Replace("-", "");
            for (var i = 0; i <= sHexPairs.Length - 2; i += 2)
            {
                bytes.Add(Convert.ToByte(sHexPairs.Substring(System.Convert.ToInt32(i), 2), 16));
            }
            return bytes;
        }

        public static string ByteArrayToHexa(byte[] _array, string _separator = "")
        {
            string sHexa = "";
            for (var i = 0; i <= _array.Length - 1; i++)
            {
                if (!string.IsNullOrEmpty(sHexa))
                {
                    sHexa += _separator;
                }
                sHexa += System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
            }
            return sHexa;
        }

        public static byte[] StringToByte(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }

    }

}

