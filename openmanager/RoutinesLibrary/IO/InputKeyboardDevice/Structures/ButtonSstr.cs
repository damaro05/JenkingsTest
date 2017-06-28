// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Runtime.InteropServices;

namespace RoutinesLibrary
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BUTTONSSTR
    {
        [MarshalAs(UnmanagedType.U2)]
        public ushort usButtonFlags;
        [MarshalAs(UnmanagedType.U2)]
        public ushort usButtonData;
    }
}
