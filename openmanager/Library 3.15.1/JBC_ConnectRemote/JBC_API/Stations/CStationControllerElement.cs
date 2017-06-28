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


    internal class CStationControllerElement
    {
        public const int TIME_TO_LIFE = 3;

        public EndpointAddress EndPoint;
        public string HostName;
        public string PCUID;
        public int TimeToLife;

        public CStationControllerElement(EndpointAddress ep, string name, string uid)
        {
            EndPoint = ep;
            HostName = name;
            PCUID = uid;
            TimeToLife = TIME_TO_LIFE;
        }
    }

}
