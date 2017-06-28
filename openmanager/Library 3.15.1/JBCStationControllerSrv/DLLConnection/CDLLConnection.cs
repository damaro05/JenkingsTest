// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports
using JBC_Connect;
using DataJBC;

namespace JBCStationControllerSrv
{
    sealed class DLLConnection
    {

        internal static JBC_API jbc = null;


        internal class CDLLConnectionController
        {

            /// <summary>
            /// This method initialize the DLL connect and start searching devices
            /// </summary>
            /// <returns>True if the initialization was successful</returns>
            internal static bool InitDLL()
            {
                try
                {
                    if (ReferenceEquals(jbc, null))
                    {
                        jbc = new JBC_API();
                        //USB and Ethernet
                        if (My.Settings.Default.SearchUSB && My.Settings.Default.SearchETH)
                        {
                            LoggerModule.logger.Info(My.Resources.Resources.evSetSearchingUSB);
                            LoggerModule.logger.Info(My.Resources.Resources.evSetSearchingETH);
                            jbc.StartSearch();
                            //USB
                        }
                        else if (My.Settings.Default.SearchUSB)
                        {
                            LoggerModule.logger.Info(My.Resources.Resources.evSetSearchingUSB);
                            jbc.StartSearch(SearchMode.USB);
                            //Ethernet
                        }
                        else if (My.Settings.Default.SearchETH)
                        {
                            LoggerModule.logger.Info(My.Resources.Resources.evSetSearchingETH);
                            jbc.StartSearch(SearchMode.ETH);
                            //Not search
                        }
                        else
                        {
                            LoggerModule.logger.Info(My.Resources.Resources.evSearchingNone);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LoggerModule.logger.Error(string.Format("{0} ({1})", My.Resources.Resources.evErrorStartingDLL, ex.Message));
                    return false;
                }

                return true;
            }

            /// <summary>
            /// This method close the DLL connect
            /// </summary>
            /// <returns>True if the close was successful</returns>
            internal static bool ReleaseDLL()
            {
                try
                {
                    if (jbc != null)
                    {
                        jbc.Close();
                        jbc = null;
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }

        }

    }
}
