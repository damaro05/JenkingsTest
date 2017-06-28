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
    public enum EnumCommandFrame_01_SOLD : byte
    {
        // Generic
        M_NULL = 0x0, // handshake
                      //Reserva: &H01 - &H03
        M_EOT = 0x4,
        //Reserva: &H05
        M_ACK = 0x6,
        //Reserva: &H07 - &H14
        M_NACK = 0x15,
        M_SYN = 0x16,
        //Reserva: &H17 - &H1D
        M_R_CONNECTSTATUS = 0x1E,
        M_W_CONNECTSTATUS = 0x1F,
        M_RESET = 0x20, // salta a ejecutar bootloader
        M_FIRMWARE = 0x21, // Leer la versión del software + hardware + protocolo + tipo estación
                           //Reserva: &H22 - &H2F
        M_INF_PORT = 0x30,
        M_R_FIXTEMP = 0x31,
        M_W_FIXTEMP = 0x32,
        M_R_LEVELTEMP = 0x33,
        M_W_LEVELTEMP = 0x34,
        M_R_LEVEL1 = 0x35,
        M_W_LEVEL1 = 0x36,
        M_R_LEVEL2 = 0x37,
        M_W_LEVEL2 = 0x38,
        M_R_LEVEL3 = 0x39,
        M_W_LEVEL3 = 0x3A,
        //M_R_RESER1 = &H3B
        //M_W_RESER1 = &H3C
        //M_R_RESER2 = &H3D
        //M_W_RESER2 = &H3E
        //Reserva = &H3F
        M_R_SLEEPDELAY = 0x40,
        M_W_SLEEPDELAY = 0x41,
        M_R_SLEEPTEMP = 0x42,
        M_W_SLEEPTEMP = 0x43,
        M_R_HIBERDELAY = 0x44,
        M_W_HIBERDELAY = 0x45,
        M_R_AJUSTTEMP = 0x46,
        M_W_AJUSTTEMP = 0x47,
        //Reserva: &H48 - &H4F
        M_R_SELECTTEMP = 0x50,
        M_W_SELECTTEMP = 0x51,
        M_R_TIPTEMP = 0x52,
        M_R_CURRENT = 0x53,
        M_R_POWER = 0x54,
        M_R_CONNECTTOOL = 0x55,
        M_R_TOOLERROR = 0x56,
        M_R_STATUSTOOL = 0x57,
        M_R_MOSTEMP = 0x58,
        M_R_DELAYTIME = 0x59,
        //Reserva: &H5A - &H5F
        M_R_REMOTEMODE = 0x60,
        M_W_REMOTEMODE = 0x61,
        M_R_STATUSREMOTEMODE = 0x62,
        M_W_STATUSREMOTEMODE = 0x63,
        //Reserva: &H64 - &H7F
        M_R_CONTIMODE = 0x80,
        M_W_CONTIMODE = 0x81,
        M_I_CONTIMODE = 0x82,
        //Reserva: &H83 - &H9F
        M_R_TEMPUNIT = 0xA0,
        M_W_TEMPUNIT = 0xA1,
        M_R_MAXTEMP = 0xA2,
        M_W_MAXTEMP = 0xA3,
        M_R_MINTEMP = 0xA4,
        M_W_MINTEMP = 0xA5,
        M_R_NITROMODE = 0xA6,
        M_W_NITROMODE = 0xA7,
        M_R_HELPTEXT = 0xA8,
        M_W_HELPTEXT = 0xA9,
        M_R_POWERLIM = 0xAA,
        M_W_POWERLIM = 0xAB,
        M_R_PIN = 0xAC,
        M_W_PIN = 0xAD,
        M_R_STATERROR = 0xAE,
        M_R_TRAFOTEMP = 0xAF,
        M_RESETSTATION = 0xB0,
        M_R_DEVICENAME = 0xB1,
        M_W_DEVICENAME = 0xB2,
        M_R_BEEP = 0xB3,
        M_W_BEEP = 0xB4,
        M_R_LANGUAGE = 0xB5,
        M_W_LANGUAGE = 0xB6,
        M_R_TEMPERRORTRAFO = 0xB7,
        M_R_TEMPERRORMOS = 0xB8,
        M_R_DEVICEID = 0xB9, // #Edu# 05/03/2013
        M_W_DEVICEID = 0xBA, // #Edu# 05/03/2013
        M_R_PARAMETERSLOCKED = 0xBB, // 08/06/2015 Read Partameters Locked
        M_W_PARAMETERSLOCKED = 0xBC, // 08/06/2015 Write Partameters Locked
                                     //Reserva: &HBD - &HBF
        M_R_PLUGTIME = 0xC0,
        M_W_PLUGTIME = 0xC1,
        M_R_WORKTIME = 0xC2,
        M_W_WORKTIME = 0xC3,
        M_R_SLEEPTIME = 0xC4,
        M_W_SLEEPTIME = 0xC5,
        M_R_HIBERTIME = 0xC6,
        M_W_HIBERTIME = 0xC7,
        M_R_NOTOOLTIME = 0xC8,
        M_W_NOTOOLTIME = 0xC9,
        M_R_SLEEPCYCLES = 0xCA,
        M_W_SLEEPCYCLES = 0xCB,
        M_R_DESOLCYCLES = 0xCC,
        M_W_DESOLCYCLES = 0xCD,
        M_R_PLUGTIMEP = 0xD0,
        M_W_PLUGTIMEP = 0xD1,
        M_R_WORKTIMEP = 0xD2,
        M_W_WORKTIMEP = 0xD3,
        M_R_SLEEPTIMEP = 0xD4,
        M_W_SLEEPTIMEP = 0xD5,
        M_R_HIBERTIMEP = 0xD6,
        M_W_HIBERTIMEP = 0xD7,
        M_R_NOTOOLTIMEP = 0xD8,
        M_W_NOTOOLTIMEP = 0xD9,
        M_R_SLEEPCYCLESP = 0xDA,
        M_W_SLEEPCYCLESP = 0xDB,
        M_R_DESOLCYCLESP = 0xDC,
        M_W_DESOLCYCLESP = 0xDD,
        M_R_ETH_TCPIPCONFIG = 0xE7, // Leer configuración TCP/IP
        M_W_ETH_TCPIPCONFIG = 0xE8, // Guardar configuración TCP/IP
        M_R_RBT_CONNCONFIG = 0xF0, // Leer configuración conexión robot
        M_W_RBT_CONNCONFIG = 0xF1, // Guardar configuración conexión robot
        M_R_RBT_CONNECTSTATUS = 0xF2, // Leer estado conexión robot
        M_W_RBT_CONNECTSTATUS = 0xF3, // Guardar estado conexión robot
        M_R_PERIPHCOUNT = 0xF9, // Leer número de periféricos
        M_R_PERIPHCONFIG = 0xFA, // Leer configuración periférico
        M_W_PERIPHCONFIG = 0xFB, // Guardar configuración periférico
        M_R_PERIPHSTATUS = 0xFC, // Leer estado periférico
        M_W_PERIPHSTATUS = 0xFD // Guardar estado periférico
    }
}
