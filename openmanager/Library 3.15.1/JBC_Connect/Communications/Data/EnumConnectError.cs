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
    public enum EnumConnectError : int
    {
        TIME_OUT, // ErrorEnumConnectError de timeout --> el disposivo no responde a las peticiones (cable interrumpido, equipo apagado, ...)
        FORMAT, // RecepciÃ³n de tramas con error de formato
        INTERPRET, // Error de interpretaciÃ³n --> el dispositivo rechaza las tramas
        RESET, // Despues de un reset el equipo deja de enviar y el PC lo detecta por tiempo
        PHYSICAL // error capa fÃ­sica
    }
}
