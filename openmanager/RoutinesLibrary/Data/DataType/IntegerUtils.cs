using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RoutinesLibrary.Data.DataType
{

    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// More information for rounding procedures: https://support.microsoft.com/en-us/kb/196652
    /// </remarks>
    public class IntegerUtils
    {

        public static bool KeyIsNumber(char cKey)
        {
            bool returnValue = false;
            // only numbers, backspace and negative
            returnValue = false;
            if ("0123456789-".IndexOf(cKey.ToString()) + 1 > 0 || cKey == Microsoft.VisualBasic.Strings.ChrW((System.Int32)Keys.Back))
            {
                returnValue = true;
            }
            return returnValue;
        }

        public static int RoundNumber(int nValue, int nStep)
        {
            return System.Convert.ToInt32(Math.Round((double)nValue / nStep) * nStep);
        }

        public static int RoundNumber(int nValue, int nStep, int nMin, int nMax)
        {
            nValue = RoundNumber(nValue, nStep);
            if (nValue < nMin)
            {
                return nMin;
            }
            if (nValue > nMax)
            {
                return nMax;
            }
            return nValue;
        }

        /// <summary>
        /// Convert a byte array to integer
        /// </summary>
        /// <param name="address">Byte array to convert</param>
        /// <param name="dataInBigEndian">True if the byte array is in Big Endian</param>
        /// <returns>Converted byte array</returns>
        public static int BytesToInt(byte[] address, bool dataInBigEndian)
        {
            List<byte> correctBytes = new List<byte>();

            correctBytes.AddRange(address);

            if ((BitConverter.IsLittleEndian && dataInBigEndian) || (!BitConverter.IsLittleEndian && !dataInBigEndian))
            {
                correctBytes.Reverse();
            }

            return BitConverter.ToInt32(correctBytes.ToArray(), 0);
        }

    }

}
