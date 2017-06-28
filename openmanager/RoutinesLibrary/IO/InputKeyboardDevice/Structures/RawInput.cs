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
    public struct RAWINPUT
    {
        [FieldOffset(0)]
        public RAWINPUTHEADER header;
        [FieldOffset(16)]
        public RAWMOUSE mouse;
        [FieldOffset(16)]
        public RAWKEYBOARD keyboard;
        [FieldOffset(16)]
        public RAWHID hid;
    }
}
