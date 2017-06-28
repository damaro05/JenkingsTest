// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Runtime.Serialization;

namespace JBC_Connect
{
    [DataContract()]
    public class faultError
    {

        [DataMember()]
        public dc_EnumConstJBC.dc_FaultError Code;
        [DataMember()]
        public string Message;
        [DataMember()]
        public string Operation;

        public faultError()
        {
            Code = dc_EnumConstJBC.dc_FaultError.NoError;
            Message = "";
            Operation = "";
        }

        public faultError(dc_EnumConstJBC.dc_FaultError _Code, string _Message, string _Operation)
        {
            Code = _Code;
            Message = _Message;
            Operation = _Operation;
        }

    }
}
