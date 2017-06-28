// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;
using System.Runtime.Serialization;
using JBC_Connect;

namespace JBCUpdaterSrv
{
    [ServiceContract(Namespace = "http://JBCUpdaterSrv")]
    public interface IJBCUpdaterService
    {

        #region Operation Contracts

        //Recibe un paquete de bytes
        [OperationContract(), FaultContract(typeof(faultError))]
        int ReceiveFile(int nSequence, byte[] bytes);

        //Inicializa la actualización del sistema
        [OperationContract(IsOneWay = true)]
        void InitUpdate();

        //Informa del estado de la actualización
        [OperationContract(), FaultContract(typeof(faultError))]
        dc_EnumConstJBC.dc_UpdateState StateUpdate();

        #endregion

    }
}
