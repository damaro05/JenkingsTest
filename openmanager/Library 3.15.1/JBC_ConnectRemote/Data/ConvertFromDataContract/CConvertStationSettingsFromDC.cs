// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
using JBC_ConnectRemote.JBCService;
using DataJBC;
using OnOff = DataJBC.OnOff;
// End of VB project level imports


namespace JBC_ConnectRemote
{


    public class CConvertStationSettingsFromDC
    {

        public static void CopyData(CStationSettingsData_SOLD stationSettings,
                dc_Station_Sold_Settings dcStationSettings)
        {

            stationSettings.Name = dcStationSettings.Name;
            stationSettings.PIN = dcStationSettings.PIN;
            if (dcStationSettings.Unit == "F")
            {
                stationSettings.Unit = CTemperature.TemperatureUnit.Fahrenheit;
            }
            else
            {
                stationSettings.Unit = CTemperature.TemperatureUnit.Celsius;
            }
            stationSettings.MaxTemp = new CTemperature(dcStationSettings.MaxTemp.UTI);
            stationSettings.MinTemp = new CTemperature(dcStationSettings.MinTemp.UTI);
            stationSettings.N2Mode = (OnOff) dcStationSettings.N2Mode;
            stationSettings.HelpText = (OnOff) dcStationSettings.HelpText;
            stationSettings.PowerLimit = dcStationSettings.PowerLimit;
            stationSettings.Beep = (OnOff) dcStationSettings.Beep;
            stationSettings.Language = (Idioma) dcStationSettings.Idioma;

        }

        public static void CopyData_HA(CStationSettingsData_HA stationSettings,
                dc_Station_HA_Settings dcStationSettings)
        {

            stationSettings.Name = dcStationSettings.Name;
            stationSettings.Beep = (OnOff) dcStationSettings.Beep;
            stationSettings.Language = (Idioma) dcStationSettings.Idioma;
            stationSettings.PINEnabled = (OnOff) dcStationSettings.PINEnabled;
            stationSettings.PIN = dcStationSettings.PIN;
            if (dcStationSettings.Unit == "F")
            {
                stationSettings.Unit = CTemperature.TemperatureUnit.Fahrenheit;
            }
            else
            {
                stationSettings.Unit = CTemperature.TemperatureUnit.Celsius;
            }
            stationSettings.StationLocked = (OnOff) dcStationSettings.StationLocked;
            stationSettings.MaxTemp = new CTemperature(dcStationSettings.MaxTemp.UTI);
            stationSettings.MinTemp = new CTemperature(dcStationSettings.MinTemp.UTI);
            stationSettings.MaxFlow = dcStationSettings.MaxFlow;
            stationSettings.MinFlow = dcStationSettings.MinFlow;
            stationSettings.MaxExtTemp = new CTemperature(dcStationSettings.MaxExtTemp.UTI);
            stationSettings.MinExtTemp = new CTemperature(dcStationSettings.MinExtTemp.UTI);

        }


        public static void CopyData_SF(CStationSettingsData_SF stationSettings, 
                    dc_Station_SF_Settings dcStationSettings)
        {

            stationSettings.Name = dcStationSettings.Name;
            stationSettings.Beep = (OnOff) dcStationSettings.Beep;
            stationSettings.PINEnabled = (OnOff) dcStationSettings.PINEnabled;
            stationSettings.PIN = dcStationSettings.PIN;
            stationSettings.LengthUnit = (CLength.LengthUnit) dcStationSettings.LengthUnit;
            stationSettings.StationLocked = (OnOff) dcStationSettings.StationLocked;
            stationSettings.SelectedProgram = dcStationSettings.SelectedProgram;

            stationSettings.Programs = new CProgramDispenserData_SF[dcStationSettings.Programs.Length - 1 + 1];
            for (int i = 0; i <= dcStationSettings.Programs.Length - 1; i++)
            {
                CProgramDispenserData_SF program = new CProgramDispenserData_SF();
                program.Enabled = (OnOff) (dcStationSettings.Programs[i].Enabled);
                program.Name = dcStationSettings.Programs[i].Name;
                program.Length_1 = dcStationSettings.Programs[i].Length_1;
                program.Speed_1 = dcStationSettings.Programs[i].Speed_1;
                program.Length_2 = dcStationSettings.Programs[i].Length_2;
                program.Speed_2 = dcStationSettings.Programs[i].Speed_2;
                program.Length_3 = dcStationSettings.Programs[i].Length_3;
                program.Speed_3 = dcStationSettings.Programs[i].Speed_3;

                stationSettings.Programs[i] = program;
            }

            stationSettings.ConcatenateProgramList = new byte[dcStationSettings.ConcatenateProgramList.Length - 1 + 1];
            Array.Copy(dcStationSettings.ConcatenateProgramList, stationSettings.ConcatenateProgramList, System.Convert.ToInt32(dcStationSettings.ConcatenateProgramList.Length));
        }

    }

}
