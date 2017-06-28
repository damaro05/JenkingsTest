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
    public class CConvertEthernetConfigurationFromDC
    {

        public static void CopyData(CEthernetData ethernet,
                                    dc_EthernetConfiguration dcEthernet)
        {

            ethernet.DHCP = (OnOff)dcEthernet.DHCP;
            ethernet.IP = dcEthernet.IP;
            ethernet.Mask = dcEthernet.Mask;
            ethernet.Gateway = dcEthernet.Gateway;
            ethernet.DNS1 = dcEthernet.DNS1;
            ethernet.DNS2 = dcEthernet.DNS2;
            ethernet.Port = dcEthernet.Port;
        }

    }
}
