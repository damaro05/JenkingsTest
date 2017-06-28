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
    /// Class encapsulating the information about a keyboard event, including the device it originated with and what key was pressed
    /// </summary>
    public class CDeviceInfo
    {
        public string deviceName;
        public string deviceType;
        public IntPtr deviceHandle;
        public string Name;
        public string source;
        public ushort key;
        public string vKey;
    }
}
