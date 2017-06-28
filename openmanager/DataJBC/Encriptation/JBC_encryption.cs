// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
    sealed internal class JBC_encryption
    {
        internal static byte[] JBC_ENCRYPTION_SALT = new byte[] { (byte)(0x7), (byte)(0x2), (byte)(0xD), (byte)(0x3), (byte)(0x4), (byte)(0x2), (byte)(0x0), (byte)(0x5), (byte)(0x0), (byte)(0xB), (byte)(0xC), (byte)(0x9), (byte)(0x0), (byte)(0x1), (byte)(0x7), (byte)(0x8) };
        internal static byte[] JBC_ENCRYPTION_KEY = new byte[] { (byte)(0x7), (byte)(0xB), (byte)(0xD), (byte)(0xB), (byte)(0x2), (byte)(0x2), (byte)(0x3), (byte)(0x5), (byte)(0x4), (byte)(0xD), (byte)(0xA), (byte)(0xA), (byte)(0x6), (byte)(0x5), (byte)(0xB), (byte)(0xB), (byte)(0xC), (byte)(0x7), (byte)(0x7), (byte)(0x9), (byte)(0xD), (byte)(0x3), (byte)(0x5), (byte)(0xD), (byte)(0x0), (byte)(0x6), (byte)(0x7), (byte)(0xE), (byte)(0x6), (byte)(0x4), (byte)(0x4), (byte)(0x6) };
        internal static byte[] JBC_ENCRYPTION_IV = new byte[] { (byte)(0x6), (byte)(0x3), (byte)(0x8), (byte)(0x4), (byte)(0x5), (byte)(0x0), (byte)(0xC), (byte)(0xC), (byte)(0xA), (byte)(0xA), (byte)(0xA), (byte)(0x3), (byte)(0xF), (byte)(0x6), (byte)(0x4), (byte)(0x8) };
    }
}
