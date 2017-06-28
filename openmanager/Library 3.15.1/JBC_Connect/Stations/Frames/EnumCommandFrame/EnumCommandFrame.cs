// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace JBC_Connect
{
    public enum EnumCommandFrame : byte
    {
        M_NULL = 0x0, // handshake
        M_ACK = 0x6,
        M_NACK = 0x15,
        M_SYN = 0x16,
        M_RESET = 0x20, // salta a ejecutar bootloader
        M_FIRMWARE = 0x21 // Leer la versión del software + hardware + protocolo + tipo estación
    }
}
