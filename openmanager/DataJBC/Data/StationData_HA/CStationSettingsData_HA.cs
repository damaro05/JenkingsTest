// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

using Microsoft.VisualBasic.CompilerServices;

namespace DataJBC
{
    public class CStationSettingsData_HA : ICloneable
    {
        public CStationSettingsData_HA()
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            m_StationDatetime = new DateTime(DateTime.MinValue.Ticks);

        }

        //Parameters
        public string Name = "";
        public OnOff Beep = OnOff._OFF;
        public Idioma Language = Idioma.I_Ingles;
        public OnOff PINEnabled = OnOff._ON;
        public string PIN = "";
        public CTemperature.TemperatureUnit Unit = CTemperature.TemperatureUnit.Celsius;
        public OnOff StationLocked = OnOff._OFF;
        private DateTime m_StationDatetime; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

        //Limits
        public CTemperature MaxTemp = new CTemperature();
        public CTemperature MinTemp = new CTemperature();
        public int MaxFlow = 0;
        public int MinFlow = 0;
        public CTemperature MaxExtTemp = new CTemperature();
        public CTemperature MinExtTemp = new CTemperature();

        //Communications
        public CRobotData Robot = new CRobotData();

        //Profiles
        public List<CProfileData_HA> Profiles = new List<CProfileData_HA>();
        public string SelectedProfile = "";


        public byte[] bytesStationDatetime
        {
            get
            {
                List<byte> bytes = new List<byte>();
                byte[] tYear = null;
                tYear = BitConverter.GetBytes(m_StationDatetime.Year);
                Array.Resize(ref tYear, 2);
                bytes.AddRange(tYear);
                bytes.Add((byte)m_StationDatetime.Month);
                bytes.Add((byte)m_StationDatetime.Day);
                bytes.Add((byte)m_StationDatetime.Hour);
                bytes.Add((byte)m_StationDatetime.Minute);
                bytes.Add((byte)m_StationDatetime.Second);
                return bytes.ToArray();
            }
            set
            {
                if (value.Length == 7)
                {
                    byte[] dtYear = new byte[2];
                    try
                    {
                        dtYear[0] = value[0];
                        dtYear[1] = value[1];
                        DateTime myDT = new DateTime(Convert.ToInt32(dtYear), value[2], value[3], value[4], value[5], value[6]);
                        m_StationDatetime = myDT;
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public DateTime dtStationDatetime
        {
            get
            {
                return m_StationDatetime;
            }
            set
            {
                try
                {
                    m_StationDatetime = value;
                }
                catch (Exception)
                {

                }
            }
        }

        public dynamic Clone()
        {
            CStationSettingsData_HA cls_Clonado = new CStationSettingsData_HA();
            cls_Clonado.Name = this.Name;
            cls_Clonado.Beep = this.Beep;
            cls_Clonado.Language = this.Language;
            cls_Clonado.PINEnabled = this.PINEnabled;
            cls_Clonado.PIN = this.PIN;
            cls_Clonado.Unit = this.Unit;
            cls_Clonado.StationLocked = this.StationLocked;
            cls_Clonado.dtStationDatetime = this.dtStationDatetime;
            cls_Clonado.MaxTemp.UTI = this.MaxTemp.UTI;
            cls_Clonado.MinTemp.UTI = this.MinTemp.UTI;
            cls_Clonado.MaxFlow = this.MaxFlow;
            cls_Clonado.MinFlow = this.MinFlow;
            cls_Clonado.MaxExtTemp.UTI = this.MaxExtTemp.UTI;
            cls_Clonado.MinExtTemp.UTI = this.MinExtTemp.UTI;
            cls_Clonado.Robot = (CRobotData)(this.Robot.Clone());
            foreach (CProfileData_HA profile in this.Profiles)
            {
                cls_Clonado.Profiles.Add((CProfileData_HA)(profile.Clone()));
            }
            cls_Clonado.SelectedProfile = this.SelectedProfile;

            return cls_Clonado;
        }

    }
}
