// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;
// End of VB project level imports


namespace JBC_Connect
{
	
	
	public enum EnumCommandFrame_02_FE : byte
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
		M_R_SUCTIONLEVEL = 0x30, // Leer nivel de succión
		M_W_SUCTIONLEVEL = 0x31, // Guardar nivel de succión
		M_R_FLOW = 0x32, // Leer caudal de aspiración actual
		M_R_SPEED = 0x33, // Leer velocidad del ventilador actual
		M_R_SELECTFLOW = 0x34, // Leer caudal de aspiración seleccionado
		M_W_SELECTFLOW = 0x35, // Guardar caudal de aspiración seleccionado
		M_R_STANDINTAKES = 0x36, // Leer número de tomas de stand
		M_W_STANDINTAKES = 0x37, // Guardar número de tomas de stand
		M_R_INTAKEACTIVATION = 0x38, // Leer estado de activación de la toma
		M_W_INTAKEACTIVATION = 0x39, // Guardar estado de activación de la toma
		M_R_SUCTIONDELAY = 0x3A, // Leer tiempo de retardo de succión
		M_W_SUCTIONDELAY = 0x3B, // Guardar tiempo de retardo de succión
		M_R_DELAYTIME = 0x3C, // Leer tiempo restante retardo de succión
		M_R_ACTIVATIONPEDAL = 0x3D, // Leer modo de activación
		M_W_ACTIVATIONPEDAL = 0x3E, // Guardar modo de activación
		M_R_PEDALMODE = 0x3F, // Leer modo de pedal
		M_W_PEDALMODE = 0x40, // Guardar modo de pedal
		M_R_FILTERSTATUS = 0x41, // Leer estado del filtro
		M_R_RESETFILTER = 0x42, // Reset del filtro
		//Reserva: &H43 - &H4F
		// Station
		M_RESETSTATION = 0x50, // Reset parámetros estación (Valores de fábrica)
		M_R_PIN = 0x51, // Leer PIN
		M_W_PIN = 0x52, // Guardar PIN
		//Reserva: &H53 - &H54
		M_R_BEEP = 0x55,
		M_W_BEEP = 0x56,
		M_R_CONTINUOUSSUCTION = 0x57,
		M_W_CONTINUOUSSUCTION = 0x58,
		M_R_STATERROR = 0x59, // Leer error
		//Reserva: &H5A
		M_R_DEVICENAME = 0x5B, // Leer nombre equipo
		M_W_DEVICENAME = 0x5C, // Guardar nombre equipo
		//Reserva: &H5D - &HBF
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
