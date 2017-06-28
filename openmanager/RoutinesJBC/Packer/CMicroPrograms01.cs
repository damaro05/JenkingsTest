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
    internal class CMicroPrograms01
    {

        private enum IntelHexType
        {
            HEX_LINE_DATA_TYPE = 0,
            HEX_LINE_EOF_TYPE = 1,
            HEX_LINE_ADDRESSMSB_TYPE = 4
        }


        private string[] m_textFileFirmware;
        private int m_posFile;
        private string m_addressMSB = "0";


        public void SetTextFileToParse(string[] sTextLines)
        {
            m_textFileFirmware = sTextLines;
            m_posFile = 0;
        }

        public bool GetNextData(ref string address, ref string data)
        {
            address = "";
            data = "";

            string line = "";

            string addressLSB = "";
            string lineData = "";
            int type = -1;

            while ((type != (int)IntelHexType.HEX_LINE_DATA_TYPE) & (type != (int)IntelHexType.HEX_LINE_EOF_TYPE))
            {

                //reading a line
                line = m_textFileFirmware[m_posFile];

                //updating the position counter
                m_posFile++;

                //procesing the line
                ParseHexLine(ref line, ref addressLSB, ref lineData, ref type);
                switch (type)
                {
                    case (int)IntelHexType.HEX_LINE_DATA_TYPE:
                        data = lineData;
                        address = m_addressMSB + addressLSB;
                        break;
                    case (int)IntelHexType.HEX_LINE_ADDRESSMSB_TYPE:
                        m_addressMSB = lineData;
                        break;
                }
            }

            return type == (int)IntelHexType.HEX_LINE_DATA_TYPE;
        }

        public void ParseHexLine(ref string line, ref string addressLSB, ref string linedata, ref int type)
        {
            //erasing the first ':' character
            line = line.Remove(0, 1);

            //getting line data byte count
            string nDataBytes = line.Substring(0, 2);

            //getting line address
            addressLSB = line.Substring(2, 4);

            //getting line record type
            type = int.Parse(line.Substring(7, 1));

            //getting line data
            linedata = line.Substring(8, line.Length - 2 - 8);
        }

    }
}
