// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Linq;
// End of VB project level imports

namespace JBC_Connect
{
	public enum EnumCommandFrame_02_SF : byte
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
		M_FORCEUPDATE = 0x29, // Forzar update (sólo paras las que se reprograman por fichero)
		//Reserva: &H2A - &H2F
		// Tool
		M_R_DISPENSERMODE = 0x30, // Leer modo de dispensador
		M_W_DISPENSERMODE = 0x31, // Guardar modo de dispensador
		M_R_PROGRAM = 0x32, // Leer programa
		M_W_PROGRAM = 0x33, // Guardar programa
		M_R_PROGRAMLIST = 0x34, // Leer lista de programas
		M_W_PROGRAMLIST = 0x35, // Guardar lista de programas
		M_R_SPEED = 0x36, // Leer speed
		M_W_SPEED = 0x37, // Guardar speed
		M_R_LENGTH = 0x38, // Leer longitud a dispensar
		M_W_LENGTH = 0x39, // Guardar longitud a dispensar
		M_R_STARTFEEDING = 0x3A, // Start feeding
		M_R_STOPTFEEDING = 0x3B, // Stop feeding
		M_R_FEEDING = 0x3C, // Leer estado del dispensador
		M_R_BACKWARDMODE = 0x3D, // Leer backward modo
		M_W_BACKWARDMODE = 0x3E, // Guardar backward modo
		// Reserva: &H3F - &H4F
		// Station
		M_RESETSTATION = 0x50, // Reset parámetros estación (Valores de fábrica)
		M_R_PIN = 0x51, // Leer PIN
		M_W_PIN = 0x52, // Guardar PIN
		M_R_STATIONLOCKED = 0x53, // Leer estación bloqueada
		M_W_STATIONLOCKED = 0x54, // Guardar estación bloqueada
		M_R_BEEP = 0x55, //
		M_W_BEEP = 0x56, //
		M_R_LENGTHUNIT = 0x57, // Leer unidades
		M_W_LENGTHUNIT = 0x58, // Guardar unidades
		M_R_STATERROR = 0x59, //
		M_R_RESETERROR = 0x5A,
		M_R_DEVICENAME = 0x5B,
		M_W_DEVICENAME = 0x5C,
		M_R_TOOLENABLED = 0x5D,
		M_W_TOOLENABLED = 0x5E,
		M_R_PINENABLED = 0x5F,
		M_W_PINENABLED = 0x60,
		//Reserva: &H61 - &HBF
		// Counters
		M_R_COUNTERS = 0xC0,
		M_R_RESETCOUNTERS = 0xC1,
		M_R_COUNTERSP = 0xC2,
		M_R_RESETCOUNTERSP = 0xC3,
		//Reserva: &HC4 - &HDF
		// USB Connection
		M_R_USB_CONNECTSTATUS = 0xE0, // Leer estado conexión USB
		M_W_USB_CONNECTSTATUS = 0xE1, // Guardar estado conexión USB
		//Reserva: &HE2 - &HEF
		// Robot Connection
		M_R_RBT_CONNCONFIG = 0xF0, // Leer configuración conexión robot
		M_W_RBT_CONNCONFIG = 0xF1, // Guardar configuración conexión robot
		M_R_RBT_CONNECTSTATUS = 0xF2, // Leer estado conexión robot
		M_W_RBT_CONNECTSTATUS = 0xF3 // Guardar estado conexión robot
			//Reserva: &HF4 - &HFE
			//Reserva: &HFF                  ' Reservado para extender los códigos
	}
}
