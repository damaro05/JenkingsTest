// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace RoutinesLibrary
{
    /// <summary>
    /// Arguments provided by the handler for the KeyPressed event
    /// </summary>
    public class CKeyControlEventArgs : EventArgs
    {

        private CDeviceInfo m_deviceInfo;
        private InputDeviceType m_device;


        public CKeyControlEventArgs(CDeviceInfo dInfo, InputDeviceType device)
        {
            m_deviceInfo = dInfo;
            m_device = device;
        }

        public CDeviceInfo Keyboard
        {
            get
            {
                return m_deviceInfo;
            }
            set
            {
                m_deviceInfo = value;
            }
        }

        public InputDeviceType Device
        {
            get
            {
                return m_device;
            }
            set
            {
                m_device = value;
            }
        }
    }
}
