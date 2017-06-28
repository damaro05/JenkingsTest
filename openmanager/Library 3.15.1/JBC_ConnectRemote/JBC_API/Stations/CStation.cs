// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;


namespace JBC_ConnectRemote
{


    /// <summary>
    /// This class is the station comunications and data container.
    /// Is designed to allow the creation of multiple ControlStack
    /// classes with events.
    /// </summary>
    /// <remarks></remarks>
    public abstract class CStation
    {

        public enum EnumProtocol
        {
            Protocol_undefined = 0,
            Protocol_01 = 1,
            Protocol_02 = 2
        }

        public enum StationState
        {
            Unknown,
            Connected,
            Disconnected
        }


        protected string myUUID;
        protected JBC_API_Remote myAPI;
        protected EnumProtocol myServiceProtocol;
        internal EndpointAddress remoteAddress;

        protected StationState State;
        internal Remote_Stack stack = null;

        public string UUID
        {
            get
            {
                return myUUID;
            }
        }

        public EnumProtocol ServiceProtocol
        {
            get
            {
                return myServiceProtocol;
            }
        }

    }

}
