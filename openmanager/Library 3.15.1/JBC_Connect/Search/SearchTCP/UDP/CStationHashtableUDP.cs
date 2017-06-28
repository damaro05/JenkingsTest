using System;
using System.Collections;
using System.Net;

namespace JBC_Connect
{
    internal class CStationHashtableUDP
    {
        //Se usa para mantener las estaciones descubiertas UDP
        private Hashtable StationTableInt = new Hashtable();


        public CStationHashtableUDP()
        {
            StationTableInt.Clear();
        }

        public CStationHashtableUDP(IPEndPoint NumEndPoint, CStationInfoUDP NewStation)
        {
            StationTableInt.Clear();
            PutStation(NumEndPoint, NewStation);
        }

        // Existe la estación
        public bool ExistsStation(IPEndPoint NumEndPoint)
        {
            return StationTableInt.ContainsKey(NumEndPoint);
        }

        // Guarda una estación
        public void PutStation(IPEndPoint NumEndPoint, CStationInfoUDP NewStation)
        {
            if (NumEndPoint != null) // Si no existe no guardar y esperar mensaje de error
            {
                NewStation.HourCreated = DateTime.Now; // también se guarda la hora en la que se guardó
                StationTableInt.Add(NumEndPoint, NewStation);
            }
        }

        // Lee no borra una estación
        public CStationInfoUDP GetStation(IPEndPoint NumEndPoint)
        {
            return ((CStationInfoUDP)(StationTableInt[NumEndPoint]));
        }

        // Borra una estación de la colección
        public void RemoveStation(IPEndPoint NumEndPoint)
        {
            StationTableInt.Remove(NumEndPoint);
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
        public Hashtable GetTable
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
