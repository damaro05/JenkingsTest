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
    public enum EnumCommandFrame_02_SOLD : byte
    {
        // Generic
        M_HS = 0x0, // handshake
                    //Reserva: &H01 - &H03
        M_EOT = 0x4,
        //Reserva: &H05
        M_ACK = 0x6,
        //Reserva: &H07 - &H14
        M_NACK = 0x15,
        M_SYN = 0x16,
        //Reserva: &H17 - &H1B
        M_R_DEVICEIDORIGINAL = 0x1C, // Leer UID original del MCU
        M_R_DISCOVER = 0x1D, // ### Trama Discover
        M_R_DEVICEID = 0x1E, // Leer UID
        M_W_DEVICEID = 0x1F, // Guardar UID
        M_RESET = 0x20, // salta a ejecutar bootloader
        M_FIRMWARE = 0x21, // Leer la versión del software + hardware + protocolo + tipo estación
                           // bootloader
        M_CLEARMEMFLASH = 0x22, // Borra memoria flash de programa menos bootloader e inicializa la programación
        M_SENDMEMADDRESS = 0x23, // Envía dirección memoria flash
        M_SENDMEMDATA = 0x24, // Envía datos memoria flash
        M_ENDPROGR = 0x25, // Finaliza la programación
        M_ENDUPD = 0x26, // Finaliza bootloader -> ejecuta programa
        M_CONTINUEUPD = 0x27, // Continuar en bootloader (wait)
        M_CLEARING = 0x28, // Borrado en progreso
                           //Reserva: &H29 - &H2F
                           // Tool
        M_INF_PORT = 0x30, // Solicitud de información del puerto
                           // Reserva: &H31 - &H32      ' FIXTEMP en 01
        M_R_LEVELSTEMPS = 0x33, // Leer niveles de temperatura
        M_W_LEVELSTEMPS = 0x34, // Guardar niveles de temperatura
                                // Reserva: &H35 - &H3F      ' M_R_LEVELx en 01
        M_R_SLEEPDELAY = 0x40,
        M_W_SLEEPDELAY = 0x41,
        M_R_SLEEPTEMP = 0x42,
        M_W_SLEEPTEMP = 0x43,
        M_R_HIBERDELAY = 0x44,
        M_W_HIBERDELAY = 0x45,
        M_R_AJUSTTEMP = 0x46,
        M_W_AJUSTTEMP = 0x47,
        M_R_CARTRIDGE = 0x48, // Read cartridge+onoff
        M_W_CARTRIDGE = 0x49, // Write cartridge+onoff
                              //Reserva: &H4A - &H4F
        M_R_SELECTTEMP = 0x50,
        M_W_SELECTTEMP = 0x51,
        M_R_TIPTEMP = 0x52, // Read tip temp (A:B)
        M_R_CURRENT = 0x53, // Read cartridge current (A:B)
        M_R_POWER = 0x54, // Read power (A:B)
        M_R_CONNECTTOOL = 0x55, // Read connected tool
        M_R_TOOLERROR = 0x56, // Read tool error
        M_R_STATUSTOOL = 0x57, // Read tool status (sleep, extractor,…)
        M_W_STATUSTOOL = 0x58, // Write tool status (sleep, extractor,…), remote mode only
        M_R_MOSTEMP = 0x59, // Read MOS temp
        M_R_DELAYTIME = 0x5A, // Read time delay sleep-extractor
                              //Reserva: &H5B - &H5F
        M_R_REMOTEMODE = 0x60, // Read remote mode
        M_W_REMOTEMODE = 0x61, // Write remote mode
                               //Reserva: &H62 - &H79
        M_R_CONTIMODE = 0x80,
        M_W_CONTIMODE = 0x81,
        M_I_CONTIMODE = 0x82,
        M_R_ALARMMAXTEMP = 0x83, // Leer temperatura máxima de alarma + retardo
        M_W_ALARMMAXTEMP = 0x84, // Guardar temperatura máxima de alarma + retardo
        M_R_ALARMMINTEMP = 0x85, // Leer temperatura mínima de alarma + retardo
        M_W_ALARMMINTEMP = 0x86, // Guardar temperatura mínima de alarma + retardo
        M_R_ALARMTEMP = 0x87, // Leer alarma de temperatura
                              //Reserva: &H88 - &H9F
                              // Station
        M_R_TEMPUNIT = 0xA0, //
        M_W_TEMPUNIT = 0xA1, //
        M_R_MAXTEMP = 0xA2, // Leer temperatura máxima
        M_W_MAXTEMP = 0xA3, // Guardar temperatura máxima
        M_R_MINTEMP = 0xA4, // Leer temperatura mínima
        M_W_MINTEMP = 0xA5, // Guardar temperatura mínima
        M_R_NITROMODE = 0xA6, //
        M_W_NITROMODE = 0xA7, //
        M_R_HELPTEXT = 0xA8, //
        M_W_HELPTEXT = 0xA9, //
        M_R_POWERLIM = 0xAA, // Leer power limit
        M_W_POWERLIM = 0xAB, // Guardar power limit
        M_R_PIN = 0xAC, // Leer PIN
        M_W_PIN = 0xAD, // Guardar PIN
        M_R_STATERROR = 0xAE, // Leer error
        M_R_TRAFOTEMP = 0xAF, // Leer temperatura del transformador
        M_RESETSTATION = 0xB0, // Reset parámetros estación (Valores de fábrica)
        M_R_DEVICENAME = 0xB1, // Leer nombre equipo
        M_W_DEVICENAME = 0xB2, // Guardar nombre equipo
        M_R_BEEP = 0xB3, //
        M_W_BEEP = 0xB4, //
        M_R_LANGUAGE = 0xB5, //
        M_W_LANGUAGE = 0xB6, //
        M_R_SCREENBRIGHTNESS = 0xB7, //
        M_W_SCREENBRIGHTNESS = 0xB8, //
        M_R_SCREENSAVER = 0xB9, //
        M_W_SCREENSAVER = 0xBA, //
        M_R_PARAMETERSLOCKED = 0xBB, // 08/06/2015 Read Partameters Locked
        M_W_PARAMETERSLOCKED = 0xBC, // 08/06/2015 Write Partameters Locked
                                     //Reserva: &HBD - &HBF
                                     // Global Counters
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
        //Reserva: &HCE - &HCF
        // Partial Counters (new)
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
        //Reserva: &HDE - &HDF
        // USB Connection
        M_R_USB_CONNECTSTATUS = 0xE0, // Leer estado conexión USB
        M_W_USB_CONNECTSTATUS = 0xE1, // Guardar estado conexión USB
                                      //Reserva: &HE2 - &HE6
                                      // Ethernet Connection
        M_R_ETH_TCPIPCONFIG = 0xE7, // Leer configuración TCP/IP
        M_W_ETH_TCPIPCONFIG = 0xE8, // Guardar configuración TCP/IP
        M_R_ETH_CONNECTSTATUS = 0xE9, // Leer estado conexión TCP/IP
        M_W_ETH_CONNECTSTATUS = 0xEA, // Guardar estado conexión TCP/IP
                                      //Reserva: &HEB - &HEF
                                      // Robot Connection
        M_R_RBT_CONNCONFIG = 0xF0, // Leer configuración conexión robot
        M_W_RBT_CONNCONFIG = 0xF1, // Guardar configuración conexión robot
        M_R_RBT_CONNECTSTATUS = 0xF2, // Leer estado conexión robot
        M_W_RBT_CONNECTSTATUS = 0xF3, // Guardar estado conexión robot
                                      //Reserva: &HF4 - &HF8
        M_R_PERIPHCOUNT = 0xF9, // Leer número de periféricos
        M_R_PERIPHCONFIG = 0xFA, // Leer configuración periférico
        M_W_PERIPHCONFIG = 0xFB, // Guardar configuración periférico
        M_R_PERIPHSTATUS = 0xFC, // Leer estado periférico
        M_W_PERIPHSTATUS = 0xFD // Guardar estado periférico
                                //Reserva: &HFE
                                //Reserva: &HFF                  ' Reservado para extender los códigos
    }
}
