// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;

using JBC_Connect;

namespace RoutinesJBC
{
    sealed class ExceptionsRoutines
    {

        internal static string errServiceErrorId = "errServiceError";
        internal static string errNotControlledId = "errNotControlled";
        internal static string errStationNotFoundId = "errStationNotFound";
        internal static string errNotValidToolId = "errNotValidTool";
        internal static string errFunctionNotSupportedId = "errFunctionNotSupported";

        /// <summary>
        /// Genera el objeto FaultException(Of faultError) a partir de una Exception no controlada
        /// </summary>
        /// <param name="_ex">Exception capturada</param>
        /// <param name="_operation">Nombre de la funciÃ³n en la cual se producjo el error</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static FaultException<faultError> getFaultEx(Exception _ex, string _operation)
        {
            faultError err = detailFaultEx(_ex, _operation);
            return new FaultException<faultError>(err, "Not controlled error."); // reason = Not controlled error
        }

        /// <summary>
        /// Genera el objeto FaultException(Of faultError) a partir de un error controlado
        /// </summary>
        /// <param name="_code">CÃ³digo del error</param>
        /// <param name="_message">Texto del error</param>
        /// <param name="_operation">Nombre de la funciÃ³n en la cual se producjo el error</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal static FaultException<faultError> getFaultEx(dc_EnumConstJBC.dc_FaultError _code, string _message, string _operation)
        {
            faultError err = detailFaultEx(_code, _message, _operation);
            return new FaultException<faultError>(err, "Service error"); // reason = Service error
        }

        // custom error detail from Exception
        internal static faultError detailFaultEx(Exception _ex, string _operation)
        {
            faultError err = new faultError();
            if (_ex != null)
            {
                string sErrMsg = _ex.Message;
                if (_ex.InnerException != null)
                {
                    sErrMsg = sErrMsg + " (" + _ex.InnerException.Message + ")";
                }
                err.Code = dc_EnumConstJBC.dc_FaultError.NotControlledError;
                err.Message = sErrMsg;
                err.Operation = _operation;
            }
            return err;
        }

        // custom error detail
        internal static faultError detailFaultEx(dc_EnumConstJBC.dc_FaultError _code, string _message, string _operation)
        {
            faultError err = new faultError();
            err.Code = _code;
            err.Message = _message;
            err.Operation = _operation;
            return err;
        }

    }
}
