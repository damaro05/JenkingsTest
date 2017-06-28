// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
// End of VB project level imports

namespace RemoteManRegister
{
    public class CerrorRegister : ApplicationException
    {

        //Error constants
        public const int NO_ERROR = 0;

        public const int ERROR_SERIE_BAD_POINT_TIME = 100;

        public const int ERROR_PLOT_BAD_MAGNITUDE_PARAM = 200;
        public const int ERROR_PLOT_BAD_PORT_PARAM = 201;
        public const int ERROR_PLOT_SERIE_NAME_REPEATED = 202;
        public const int ERROR_PLOT_SERIE_NOT_FOUND = 203;
        public const int ERROR_PLOT_SERIE_ENTRY_NOT_FOUND = 204;
        public const int ERROR_PLOT_ENTRY_NOT_SERIE = 205;
        public const int ERROR_PLOT_LOAD_LBR = 206;

        public const int ERROR_COM_GET_DATA = 300;
        public const int ERROR_COM_OPEN = 301;
        public const int ERROR_COM_CLOSE = 302;

        public const int ERROR_GUI_AXIS_BAD_RANGE_VALUES = 400;
        public const int ERROR_GUI_SERIE_REPEATED_NAME = 401;
        public const int ERROR_GUI_SERIE_EMPTY_NAME = 402;
        public const int ERROR_GUI_SERIE_NO_STATION_CONNECTED = 403;
        public const int ERROR_GUI_SERIE_EMPTY_LIST = 404;
        public const int ERROR_GUI_SERIE_NO_STATION_SELECTED = 405;
        public const int ERROR_GUI_SERIE_NO_SERIE_SELECTED_TO_EDIT_OR_REMOVE = 406;

        //Error strings
        private const string ERROR_STRINGS = "100|Point time value is lower than previous one.|" +
            "200|Bad magnitude value. It must be Temperature or Power.|" +
            "201|Bad port number. It must be from 1 to 4.|" +
            "202|The serie name already exists.|" +
            "203|The desired serie does not exist.|" +
            "204|Point time value is lower than previous ones.|" +
            "205|Point time value is lower than previous ones.|" +
            "206|Problem loading the .lbr. The file is corrupted.|" +
            "300|Comunications error.|" +
            "301|Port open error.|" +
            "302|Port close error.|" +
            "400|The range values indicated are incoherent.|" +
            "401|The serie name already exists.|" +
            "402|The name has not been introduced.|" +
            "403|There must be an station connected.|" +
            "404|There's any serie.|" +
            "405|Select a station for the serie.|" +
            "406|No serie selected to be edited or removed.";

        //global vars
        private int myCode;
        private string myText;

        //functions
        public CerrorRegister(int code)
        {
            //setting the code
            myCode = code;

            //loading the Error strings
            string[] str = ERROR_STRINGS.Split("|".ToCharArray());
            for (int i = 0; i <= str.Length - 1; i += 2)
            {
                if (Convert.ToInt32(str[i]) == myCode)
                {
                    myText = str[i + 1];
                    break;
                }
            }
        }

        public void showError()
        {
            Interaction.MsgBox("ERROR! - code: " + System.Convert.ToString(myCode) + "\r\n" + myText, MsgBoxStyle.Critical, null);
        }
    }
}
