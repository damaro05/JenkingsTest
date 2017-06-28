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
	public enum EnumCommandFrame_02_HA : byte
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
		M_INF_PORT = 0x30, // Solicitud de información del puerto
		M_RESET_PORTTOOL = 0x31, // Reset configuración port+tool
		// Reserva: &H32
		M_R_PROFILEMODE = 0x33, // Leer modo de trabajo de perfil (off=manual/on=perfiles)
		M_W_PROFILEMODE = 0x34, // Guardar modo de trabajo de perfil (off=manual/on=perfiles)
		M_R_HEATERSTATUS = 0x35, // Leer estado calefactor
		M_W_HEATERSTATUS = 0x36, // Guardar estado calefactor
		M_R_SUCTIONSTATUS = 0x37, // Leer estado succión
		M_W_SUCTIONSTATUS = 0x38, // Guardar estado succión
		M_R_EXTTCMODE = 0x39, // Leer modo configuración TC externo
		M_W_EXTTCMODE = 0x3A, // Guardar modo configuración TC externo
		// Reserva: &H3B - &H3F
		M_R_LEVELSTEMPS = 0x40, // Leer niveles de temperatura y caudal
		M_W_LEVELSTEMPS = 0x41, // Guardar niveles de temperatura y caudal
		M_R_AJUSTTEMP = 0x42, // Leer temp de ajuste
		M_W_AJUSTTEMP = 0x43, // Guardar temp de ajuste
		M_R_TIMETOSTOP = 0x44, // Leer tiempo para parar
		M_W_TIMETOSTOP = 0x45, // Guardar tiempo para parar
		M_R_STARTMODE = 0x46, // Leer modo activación
		M_W_STARTMODE = 0x47, // Guardar modo activación
		//Reserva: &H48 - &H4F
		M_R_SELECTTEMP = 0x50, // Leer temp seleccionada
		M_W_SELECTTEMP = 0x51, // Guardar temp seleccionada
		M_R_AIRTEMP = 0x52, // Read air temp
		//Reserva: &H53
		M_R_POWER = 0x54, // Read power
		M_R_CONNECTTOOL = 0x55, // Read connected tool
		M_R_TOOLERROR = 0x56, // Read tool error
		M_R_STATUSTOOL = 0x57, // Read tool status (heater, heater solicitado ,cooling, suction, etc)
		//Reserva: &H58
		M_R_SELECTFLOW = 0x59, // Read selected Flow
		M_W_SELECTFLOW = 0x5A, // Write selected Flow
		M_R_SELECTEXTTEMP = 0x5B, // Read selected External Temp
		M_W_SELECTEXTTEMP = 0x5C, // Write selected External Temp
		M_R_AIRFLOW = 0x5D, // Read current air flow
		//Reserva: &H5E
		M_R_EXTTCTEMP = 0x5F, // Read current external TC Temp
		M_R_REMOTEMODE = 0x60, // Read remote mode
		M_W_REMOTEMODE = 0x61, // Write remote mode
		//Reserva: &H62 - &H7F
		M_R_CONTIMODE = 0x80,
		M_W_CONTIMODE = 0x81,
		M_I_CONTIMODE = 0x82,
		//Reserva: &H83 - &H8F
		// Files
		M_READSTARTFILE = 0x90,
		M_READFILEBLOCK = 0x91,
		M_READENDOFFILE = 0x92,
		M_WRITESTARTFILE = 0x93,
		M_WRITEFILEBLOCK = 0x94,
		M_WRITEENDOFFILE = 0x95,
		M_R_FILESCOUNT = 0x96,
		M_R_GETFILENAME = 0x97,
		M_DELETEFILE = 0x98,
		M_R_SELECTEDFILENAME = 0x9A,
		M_W_SELECTFILE = 0x9B,
		// Station
		M_R_TEMPUNIT = 0xA0, // Leer unidades de temp
		M_W_TEMPUNIT = 0xA1, // Guardar unidades de temp
		M_R_MAXMINTEMP = 0xA2, // Leer límites temp aire (máx:mín)
		M_W_MAXMINTEMP = 0xA3, // Guardar límites temp aire (máx:mín)
		M_R_MAXMINFLOW = 0xA4, // Leer límites caudal aire (máx:mín)
		M_W_MAXMINFLOW = 0xA5, // Guardar límites caudal aire (máx:mín)
		M_R_MAXMINEXTTEMP = 0xA6, // Leer límites temp TC externo (máx:mín)
		M_W_MAXMINEXTTEMP = 0xA7, // Guardar límites temp TC externo (máx:mín)

		M_R_PINENABLED = 0xA8, // Leer PIN enabled
		M_W_PINENABLED = 0xA9, // Guardar PIN enabled
		M_R_STATIONLOCKED = 0xAA, // Leer estación bloqueada
		M_W_STATIONLOCKED = 0xAB, // Guardar estación bloqueada
		M_R_PIN = 0xAC, // Leer PIN
		M_W_PIN = 0xAD, // Guardar PIN
		M_R_STATERROR = 0xAE, // Leer error de la estación
		//Reserva: &HAF
		M_RESETSTATION = 0xB0, // Reset parámetros estación (Valores de fábrica)
		M_R_DEVICENAME = 0xB1, // Leer nombre equipo
		M_W_DEVICENAME = 0xB2, // Guardar nombre equipo
		M_R_BEEP = 0xB3, //
		M_W_BEEP = 0xB4, //
		M_R_LANGUAGE = 0xB5, //
		M_W_LANGUAGE = 0xB6, //
		//Reserva: &HB7 - &HBA
		M_R_DATETIME = 0xBB, // Leer fecha y hora
		M_W_DATETIME = 0xBC, // Guardar fecha y hora
		//Reserva: &HBD
		M_R_THEME = 0xBE, // Leer tema de pantalla (no implementado)
		M_W_THEME = 0xBF, // Guardar tema de pantalla (no implementado)
		// Global Counters
		M_R_PLUGTIME = 0xC0,
		M_W_PLUGTIME = 0xC1,
		M_R_WORKTIME = 0xC2,
		M_W_WORKTIME = 0xC3,
		M_R_WORKCYCLES = 0xC4,
		M_W_WORKCYCLES = 0xC5,
		M_R_SUCTIONCYCLES = 0xC6,
		M_W_SUCTIONCYCLES = 0xC7,
		//Reserva: &HC8 - &HCF
		// Partial Counters
		M_R_PLUGTIMEP = 0xD0,
		M_W_PLUGTIMEP = 0xD1,
		M_R_WORKTIMEP = 0xD2,
		M_W_WORKTIMEP = 0xD3,
		M_R_WORKCYCLESP = 0xD4,
		M_W_WORKCYCLESP = 0xD5,
		M_R_SUCTIONCYCLESP = 0xD6,
		M_W_SUCTIONCYCLESP = 0xD7,
		//Reserva: &HD8 - &HDF
		// USB Connection
		M_R_USB_CONNECTSTATUS = 0xE0, // Leer estado conexión USB
		M_W_USB_CONNECTSTATUS = 0xE1, // Guardar estado conexión USB
		//Reserva: &HE2 - &HEF
		// Robot Connection
		M_R_RBT_CONNCONFIG = 0xF0, // Leer configuración conexión robot
		M_W_RBT_CONNCONFIG = 0xF1, // Guardar configuración conexión robot
		M_R_RBT_CONNECTSTATUS = 0xF2, // Leer estado conexión robot
		M_W_RBT_CONNECTSTATUS = 0xF3 // Guardar estado conexión robot
			//Reserva: &HF4 - &HF8
			//M_R_PERIPHCOUNT = &HF9          ' Leer número de periféricos
			//M_R_PERIPHCONFIG = &HFA         ' Leer configuración periférico
			//M_W_PERIPHCONFIG = &HFB         ' Guardar configuración periférico
			//M_R_PERIPHSTATUS = &HFC         ' Leer estado periférico
			//M_W_PERIPHSTATUS = &HFD         ' Guardar estado periférico
			//Reserva: &HFE
			//Reserva: &HFF                  ' Reservado para extender los códigos
	}
}
