// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
    public class CPeripheralData : ICloneable
    {

        public enum PeripheralType : int
        {
            PD = 0,
            MS = 1,
            MN = 2,
            FS = 3,
            MV = 4,
            NO_TYPE = -1
        }

        public enum PeripheralStatusPD
        {
            CC = 0, //C <- corto circuito
            OC = 1, //O <- circuito abierto
            OK = 2 //K <- ok
        }

        public enum PeripheralFunction : int
        {
            Sleep = 0,
            Extractor = 1,
            Modul = 2,
            NO_FUNCTION = -1
        }

        public enum PeripheralActivation : int
        {
            Pressed = 0,
            Pulled = 1,
            NO_FUNCTION = -1
        }


        private int m_ID = 0;

        //Configuration
        private short m_version = (short)0;
        private string m_Hash_MCU_UID = "";
        private string m_dateTime = "";
        private PeripheralType m_type = PeripheralType.NO_TYPE;
        private Port m_portAttached = Port.NO_PORT;
        private PeripheralFunction m_workFunction = PeripheralFunction.NO_FUNCTION;
        private PeripheralActivation m_activationMode = PeripheralActivation.Pressed;
        private short m_delayTime = (short)0;

        //Status
        private OnOff m_statusActive = OnOff._OFF;
        private PeripheralStatusPD m_statusPD = PeripheralStatusPD.OK;


        public CPeripheralData(int nID = 0)
        {
            ID = nID;
        }

        public dynamic Clone()
        {
            CPeripheralData new_PeripheralData = new CPeripheralData();
            new_PeripheralData.Version = this.Version;
            new_PeripheralData.Hash_MCU_UID = this.Hash_MCU_UID;
            new_PeripheralData.DateTime = this.DateTime;
            new_PeripheralData.Type = this.Type;
            new_PeripheralData.PortAttached = this.PortAttached;
            new_PeripheralData.WorkFunction = this.WorkFunction;
            new_PeripheralData.ActivationMode = this.ActivationMode;
            new_PeripheralData.DelayTime = this.DelayTime;

            return new_PeripheralData;
        }

        public int ID
        {
            get
            {
                return m_ID;
            }
            set
            {
                m_ID = value;
            }
        }

        #region Configuration

        public short Version
        {
            get
            {
                return m_version;
            }
            set
            {
                m_version = value;
            }
        }

        public string Hash_MCU_UID
        {
            get
            {
                return m_Hash_MCU_UID;
            }
            set
            {
                m_Hash_MCU_UID = value;
            }
        }

        public string DateTime
        {
            get
            {
                return m_dateTime;
            }
            set
            {
                m_dateTime = value;
            }
        }

        public PeripheralType Type
        {
            get
            {
                return m_type;
            }
            set
            {
                if (PeripheralType.IsDefined(typeof(PeripheralType), value))
                {
                    m_type = value;
                }
                else
                {
                    m_type = PeripheralType.NO_TYPE;
                }
            }
        }

        public Port PortAttached
        {
            get
            {
                return m_portAttached;
            }
            set
            {
                if (Port.IsDefined(typeof(Port), value))
                {
                    m_portAttached = value;
                }
                else
                {
                    m_portAttached = Port.NO_PORT;
                }
            }
        }

        public PeripheralFunction WorkFunction
        {
            get
            {
                return m_workFunction;
            }
            set
            {
                if (PeripheralFunction.IsDefined(typeof(PeripheralFunction), value))
                {
                    m_workFunction = value;
                }
                else
                {
                    m_workFunction = PeripheralFunction.NO_FUNCTION;
                }
            }
        }

        public PeripheralActivation ActivationMode
        {
            get
            {
                return m_activationMode;
            }
            set
            {
                if (PeripheralActivation.IsDefined(typeof(PeripheralActivation), value))
                {
                    m_activationMode = value;
                }
                else
                {
                    m_activationMode = PeripheralActivation.Pressed;
                }
            }
        }

        public short DelayTime
        {
            get
            {
                return m_delayTime;
            }
            set
            {
                m_delayTime = value;
            }
        }

        #endregion

        #region Status

        public OnOff StatusActive
        {
            get
            {
                return m_statusActive;
            }
            set
            {
                if (OnOff.IsDefined(typeof(OnOff), value))
                {
                    m_statusActive = value;
                }
                else
                {
                    m_statusActive = OnOff._OFF;
                }
            }
        }


        public PeripheralStatusPD StatusPD
        {
            get
            {
                return m_statusPD;
            }
            set
            {
                if (PeripheralStatusPD.IsDefined(typeof(PeripheralStatusPD), value))
                {
                    m_statusPD = value;
                }
            }
        }

        #endregion

    }
}
