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

namespace JBCHostControllerSrv
{
    /// <summary>
    /// Stores information of the service that is being updating
    /// </summary>
    /// <remarks></remarks>
    public class CUpdaterServicePending
    {

        private const int TRIES_UPDATES = 5;

        public EndpointAddress endPoint;
        public string sUrlSendFile;
        public int nTriesRemainingUpdate;


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="ep">Endpoint's service to update</param>
        /// <param name="url">Update package to send to service</param>
        public CUpdaterServicePending(EndpointAddress ep, string url)
        {
            this.endPoint = ep;
            this.sUrlSendFile = url;
            this.nTriesRemainingUpdate = TRIES_UPDATES;
        }

        /// <summary>
        /// Decrement attempts to update
        /// </summary>
        public void DecrementRemainingUpdate()
        {
            nTriesRemainingUpdate--;
        }

    }
}
