using System;
using System.Collections.Generic;
using System.Net;

namespace JBC_Connect
{
    internal class CStationtableTCP
    {
        //Se usa para mantener las estaciones descubiertas y conectadas
        private List<CStationInfoTCP> StationTableInt = new List<CStationInfoTCP>();

        public CStationtableTCP()
        {
            StationTableInt.Clear();
        }

        public CStationtableTCP(IPEndPoint NumEndPoint, CStationConnectionData StationData)
        {
            StationTableInt.Clear();
            AddStation(NumEndPoint, StationData);
        }

        public int ExistsStation(IPEndPoint NumEndPoint)
        {
            // Si existe la estación devuelve el índice, si no existe devuelve -1
            int i = 0;
            for (i = 0; i <= StationTableInt.Count - 1; i++)
            {
                if (StationTableInt[i].StationData.IPEndPointValue.Address.ToString() == NumEndPoint.Address.ToString())
                {
                    return i;
                }
            }
            return -1;
        }

        // Guarda una estación nueva
        public void AddStation(IPEndPoint NumEndPoint, CStationConnectionData StationData)
        {
            if (NumEndPoint != null)
            {
                CStationInfoTCP stnInfo = new CStationInfoTCP(StationData);
                stnInfo.HourAdded = DateTime.Now;
                stnInfo.bDiscovered = true;
                stnInfo.bConnectedTCP = false;
                //stnInfo.WinSock = Nothing
                stnInfo.iAttemptsTCP = 0;
                StationTableInt.Add(stnInfo);
            }
        }

        // Guarda una estación info copiada
        public void AddStation(IPEndPoint NumEndPoint, CStationInfoTCP StationInfo)
        {
            if (NumEndPoint != null)
            {
                StationTableInt.Add(StationInfo);
            }
        }

        // Lee no borra una estación
        public CStationInfoTCP GetStation(IPEndPoint NumEndPoint)
        {
            return GetStation(ExistsStation(NumEndPoint));
        }

        public CStationInfoTCP GetStation(int idx)
        {
            if (idx >= 0 & idx <= StationTableInt.Count - 1)
            {
                return StationTableInt[idx];
            }
            return null;
        }

        // Borra una estación de la colección
        public void RemoveStation(IPEndPoint NumEndPoint)
        {
            RemoveStation(ExistsStation(NumEndPoint));
        }

        public void RemoveStation(int idx)
        {
            if (idx >= 0 & idx <= StationTableInt.Count - 1)
            {
                StationTableInt.RemoveAt(idx);
            }
        }

        public bool get_Connected(int idx)
        {
            return StationTableInt[idx].bConnectedTCP;
        }
        public void set_Connected(int idx, bool value)
        {
            StationTableInt[idx].bConnectedTCP = value;
        }

        //Public Property StationWinSock(ByVal idx As Integer) As WinSockClient
        //    Get
        //        Return StationTableInt.Item(idx).WinSock
        //    End Get
        //    Set(ByVal value As WinSockClient)
        //        StationTableInt.Item(idx).WinSock = value
        //    End Set
        //End Property

        public bool get_Discovered(int idx)
        {
            return StationTableInt[idx].bDiscovered;
        }
        public void set_Discovered(int idx, bool value)
        {
            StationTableInt[idx].bDiscovered = value;
        }

        public int get_Attempts(int idx)
        {
            return StationTableInt[idx].iAttemptsTCP;
        }
        public void set_Attempts(int idx, int value)
        {
            StationTableInt[idx].iAttemptsTCP = value;
        }


        // Devuelve el número de estaciones almacenadas
        public int Number
        {
            get
            {
                return StationTableInt.Count;
            }
        }

        // Devuelve la colección de estaciones almacenadas
        public List<CStationInfoTCP> GetTable
        {
            get
            {
                return StationTableInt;
            }
        }

        // Borra toda la tabla
        public void Reset()
        {
            StationTableInt.Clear();
        }

    }
}
