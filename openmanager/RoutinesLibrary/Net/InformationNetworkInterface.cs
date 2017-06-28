// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Net;
using System.Net.NetworkInformation;


    namespace RoutinesLibrary.Net
    {

        /// <summary>
        /// This class provides informartion for network interface
        /// </summary>
        /// <remarks></remarks>
        internal class InformationNetworkInterface
        {

            public struct NetworkInterfaceAddress
            {
                internal IPAddress Address;
                internal IPAddress Mask;
                internal IPAddress Broadcast;
            }


            #region PUBLIC METHODS

            /// <summary>
            /// Get all the network interface addresses
            /// </summary>
            /// <returns></returns>
            /// <remarks></remarks>
            public static NetworkInterfaceAddress[] GetNetworkInterfaceAddress()
            {
                List<NetworkInterfaceAddress> listNetworkInterfaceAddress = new List<NetworkInterfaceAddress>();

                foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    IPInterfaceProperties ipNetworkInterface = netInterface.GetIPProperties();

                    foreach (UnicastIPAddressInformation unicastAddress in ipNetworkInterface.UnicastAddresses)
                    {
                        if ((ReferenceEquals(unicastAddress.IPv4Mask, null) == false) && (unicastAddress.Address.IsIPv6LinkLocal == false))
                        {
                            if (IsValidIPv4(unicastAddress.IPv4Mask) && IsValidIPv4(unicastAddress.Address))
                            {

                                string addressBroadcast = "";
                                byte[] addressMaskByte = ConvertIP2Bytes(unicastAddress.IPv4Mask);
                                byte[] addressByte = ConvertIP2Bytes(unicastAddress.Address);
                                byte[] addressBroadcastByte = new byte[addressByte.Length];

                                for (int index = 0; index < addressByte.Length; index++)
                                {
                                    addressBroadcastByte[index] = (byte)(addressByte[index] | ~addressMaskByte[index]);
                                    addressBroadcast = addressBroadcast + addressBroadcastByte[index].ToString();
                                    if (index != addressByte.Length - 1)
                                    {
                                        addressBroadcast = addressBroadcast + ".";
                                    }
                                }

                                NetworkInterfaceAddress interfaceAddress = new NetworkInterfaceAddress();
                                interfaceAddress.Address = unicastAddress.Address;
                                interfaceAddress.Mask = unicastAddress.IPv4Mask;
                                interfaceAddress.Broadcast = IPAddress.Parse(addressBroadcast);

                                listNetworkInterfaceAddress.Add(interfaceAddress);
                            }
                        }
                    }
                }

                return listNetworkInterfaceAddress.ToArray();
            }

            /// <summary>
            /// Get a port available
            /// </summary>
            /// <param name="minPort">Minimum port</param>
            /// <param name="maxPort">Maximum port</param>
            /// <returns>Port available</returns>
            public static ushort GetPortAvailable(ushort minPort = default(ushort), ushort maxPort = default(ushort))
            {
                ushort port = minPort;
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                IPEndPoint[] ipEndpoint = ipProperties.GetActiveUdpListeners();
                List<int> listBussyPort = new List<int>();

                foreach (IPEndPoint ip in ipEndpoint)
                {
                    listBussyPort.Add(ip.Port);
                }

                while (true)
                {
                    if (!listBussyPort.Contains(port))
                    {
                        return port;
                    }

                    port = System.Convert.ToUInt16(port + 1);
                    if (port > maxPort)
                    {
                        port = minPort;
                    }
                }
            }

            #endregion

            #region PRIVATE METHODS

            /// <summary>
            /// Check if a IP address is v4
            /// </summary>
            /// <param name="addr">IP address</param>
            /// <returns>True if the IP address is v4</returns>
            private static bool IsValidIPv4(IPAddress addr)
            {
                bool bOk = false;

                //check to make sure an ip address was provided
                System.Net.IPAddress temp_address = null;
                if (!string.IsNullOrEmpty(addr.ToString()) && System.Net.IPAddress.TryParse(addr.ToString(), out temp_address))
                {
                    bOk = ConvertIP2Bytes(addr).Length == 4;
                }

                return bOk;
            }

            /// <summary>
            /// Convert IP address to byte array
            /// </summary>
            /// <param name="AddressConver"></param>
            /// <returns>Byte array IP address</returns>
            private static byte[] ConvertIP2Bytes(IPAddress AddressConver)
            {
                try
                {
                    string[] AddressString = AddressConver.ToString().Split(".".ToCharArray());
                    if (AddressString.Length != 4) // Error
                    {
                        return new byte[] { };
                    }
                    byte[] AddressByte = new byte[AddressString.Length - 1 + 1];
                    int Convert = 0;
                    for (int index = 0; index <= AddressString.Length - 1; index++)
                    {
                        Convert = int.Parse(AddressString[index]);
                        if (Convert < 0 | Convert > 255) // Error
                        {
                            return new byte[] { };
                        }
                        AddressByte[index] = (byte)Convert;
                    }
                    return AddressByte;
                }
                catch (Exception)
                {
                    return new byte[] { };
                }
            }

            #endregion

        }

    }

