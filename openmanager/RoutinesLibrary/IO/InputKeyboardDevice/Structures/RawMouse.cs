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
    [StructLayout(LayoutKind.Explicit)]
    public struct RAWMOUSE
    {
        [MarshalAs(UnmanagedType.U2)]
        [FieldOffset(0)]
        public ushort usFlags;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(4)]
        public uint ulButtons;
        [FieldOffset(4)]
        public BUTTONSSTR buttonsStr;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(8)]
        public uint ulRawButtons;
        [FieldOffset(12)]
        public int lLastX;
        [FieldOffset(16)]
        public int lLastY;
        [MarshalAs(UnmanagedType.U4)]
        [FieldOffset(20)]
        public uint ulExtraInformation;
    }
}
