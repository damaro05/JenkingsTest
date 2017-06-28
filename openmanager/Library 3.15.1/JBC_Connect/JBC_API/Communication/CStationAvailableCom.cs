// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Threading;

namespace JBC_Connect
{
    /// <summary>
    /// Relaciona un canal de comunicaciones con un UUID de estación
    /// </summary>
    /// <remarks>
    /// Se utilizan dos HashTable (consulta de datos bidireccional) para ganar eficiencia en el acceso a datos
    /// </remarks>
    internal class CStationAvailableCom
    {

        private Hashtable m_htPhy2ListStationUUID = new Hashtable(); //Relaciona un canal de comunicaciones con una lista de UUID de estaciones
        private Hashtable m_htStationUUID2Phy = new Hashtable(); //Relaciona un UUID de estación con un canal de comunicaciones
        private Hashtable m_htStationUUID2Address = new Hashtable(); //Relaciona un UUID de estación con un address de estación
        private Mutex m_mutexRouterTables = new Mutex();


        public void Dispose()
        {
            m_mutexRouterTables.WaitOne();

            //Eliminar todos los canales de comunicación
            foreach (DictionaryEntry item in m_htPhy2ListStationUUID)
            {
                ((CCommunicationChannel)item.Key).Dispose();
            }
            m_htPhy2ListStationUUID.Clear();
            m_htPhy2ListStationUUID = null;

            m_htStationUUID2Phy.Clear();
            m_htStationUUID2Phy = null;

            m_htStationUUID2Address.Clear();
            m_htStationUUID2Address = null;

            m_mutexRouterTables.ReleaseMutex();
        }

        public void AddUUIDStation(string stationUUID, byte address, ref CCommunicationChannel PhysicalChannel)
        {
            m_mutexRouterTables.WaitOne();

            //Comprobamos si el canal de comunicación existe
            if (m_htPhy2ListStationUUID.Contains(PhysicalChannel))
            {

                //Añadimos la relación Physical -> Station UUID
                List<string> listStationUUID = (List<string>)(m_htPhy2ListStationUUID[PhysicalChannel]);
                listStationUUID.Add(stationUUID);
                m_htPhy2ListStationUUID[PhysicalChannel] = listStationUUID;

                //Si no existe el canal de comunicación lo añadimos
            }
            else
            {

                //Añadimos la relación Physical -> Station UUID
                List<string> listStationUUID = new List<string>();
                listStationUUID.Add(stationUUID);
                m_htPhy2ListStationUUID.Add(PhysicalChannel, listStationUUID);
            }

            //Añadimos la relación Station UUID -> Physical
            m_htStationUUID2Phy.Add(stationUUID, PhysicalChannel);

            //Añadimos la relación Station UUID -> Address
            m_htStationUUID2Address.Add(stationUUID, address);

            m_mutexRouterTables.ReleaseMutex();
        }

        public void RemoveIDStation(string stationUUID)
        {
            m_mutexRouterTables.WaitOne();

            //Comprobamos si el Station UUID existe
            if (m_htStationUUID2Phy.Contains(stationUUID))
            {

                CCommunicationChannel PhysicalChannel = (CCommunicationChannel)(m_htStationUUID2Phy[stationUUID]);

                //Borramos la relación Station UUID -> Physical
                m_htStationUUID2Phy.Remove(stationUUID);

                //Borramos la relación Station UUID -> Address
                m_htStationUUID2Address.Remove(stationUUID);

                //Borramos la relación Physical -> Station ID
                List<string> listStationUUID = (List<string>)(m_htPhy2ListStationUUID[PhysicalChannel]);
                listStationUUID.Remove(stationUUID);

                if (listStationUUID.Count > 0)
                {
                    m_htPhy2ListStationUUID[PhysicalChannel] = listStationUUID;
                }
                else
                {
                    m_htPhy2ListStationUUID.Remove(PhysicalChannel);

                    //Como el canal de comunicaciones no tiene ninguna estación asociada lo eliminamos
                    PhysicalChannel.Dispose();
                    PhysicalChannel = null;
                }
            }

            m_mutexRouterTables.ReleaseMutex();
        }

        public bool CheckAddressComChannel(string stationParentUUID, byte address)
        {
            bool bExists = false;
            m_mutexRouterTables.WaitOne();

            //Comprobamos si el Station UUID existe
            if (m_htStationUUID2Phy.Contains(stationParentUUID))
            {

                //Obtenemos el canal de comunicación de la estación
                CCommunicationChannel PhysicalChannel = (CCommunicationChannel)(m_htStationUUID2Phy[stationParentUUID]);

                //Obtenemos todas las estaciones del canal de comunicación
                List<string> listStationUUID = (List<string>)(m_htPhy2ListStationUUID[PhysicalChannel]);

                //Comprobamos si alguna estación está asociada al address
                foreach (string stationUUID in listStationUUID)
                {
                    if (System.Convert.ToByte(m_htStationUUID2Address[stationUUID]) == address)
                    {
                        bExists = true;
                        break;
                    }
                }
            }
            m_mutexRouterTables.ReleaseMutex();

            return bExists;
        }

        public void GetComChannel(string stationUUID, ref CCommunicationChannel comChannel)
        {
            m_mutexRouterTables.WaitOne();

            if (m_htStationUUID2Phy.Contains(stationUUID))
            {
                comChannel = (CCommunicationChannel)(m_htStationUUID2Phy[stationUUID]);
            }

            m_mutexRouterTables.ReleaseMutex();
        }

        public List<string> GetListStationSameComChannel(string stationUUID)
        {
            List<string> listStationUUID = new List<string>();
            m_mutexRouterTables.WaitOne();

            //Comprobamos si el Station UUID existe
            if (m_htStationUUID2Phy.Contains(stationUUID))
            {

                //Obtenemos el canal de comunicación de la estación
                CCommunicationChannel PhysicalChannel = (CCommunicationChannel)(m_htStationUUID2Phy[stationUUID]);

                //Obtenemos todas las estaciones del canal de comunicación
                listStationUUID = (List<string>)(m_htPhy2ListStationUUID[PhysicalChannel]);
            }
            m_mutexRouterTables.ReleaseMutex();

            return listStationUUID;
        }

    }
}
