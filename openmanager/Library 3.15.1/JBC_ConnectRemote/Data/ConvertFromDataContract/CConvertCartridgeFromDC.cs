// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
using JBC_ConnectRemote.JBCService;
using DataJBC;
using OnOff = DataJBC.OnOff;
// End of VB project level imports


namespace JBC_ConnectRemote
{
    public class CConvertCartridgeFromDC
    {

        public static void CopyData(CCartridgeData cartridge,
                                    dc_Cartridge dcCartridge)
        {

            cartridge.CartridgeNbr = dcCartridge.CartridgeNbr;
            cartridge.CartridgeOnOff = (OnOff)dcCartridge.CartridgeOnOff;
        }

    }
}
