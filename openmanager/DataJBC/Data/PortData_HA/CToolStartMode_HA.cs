// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Data;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace DataJBC
{
	public class CToolStartMode_HA : ICloneable
	{

		protected OnOff m_ToolButton = OnOff._OFF;
		protected PedalAction m_Pedal = PedalAction.NONE;


		public CToolStartMode_HA()
		{
		}

		public CToolStartMode_HA(byte bits)
		{
			fromByte(bits);
		}

		public void fromByte(byte bits)
		{
			m_ToolButton = (OnOff)(bits & 0x1);
			if (((OnOff)((bits & 0x4) >> 2)) == OnOff._ON)
			{
				m_Pedal = PedalAction.PULSE;
			}
			else if (((OnOff)((bits & 0x8) >> 3)) == OnOff._ON)
			{
				m_Pedal = PedalAction.HOLD_DOWN;
			}
			else
			{
				m_Pedal = PedalAction.NONE;
			}
		}

		public byte toByte()
		{
			byte byt = (byte)0;
			if (m_ToolButton == OnOff._ON)
			{
				byt += (byte)ToolStartMode_HA.TOOL_BUTTON;
			}
			switch (m_Pedal)
			{
				case PedalAction.PULSE:
					byt += (byte)ToolStartMode_HA.PEDAL_PULSE;
					break;
				case PedalAction.HOLD_DOWN:
					byt += (byte)ToolStartMode_HA.PEDAL_HOLD_DOWN;
					break;
			}
			return byt;
		}

		public OnOff ToolButton
		{
			get
			{
				return m_ToolButton;
			}
			set
			{
				m_ToolButton = value;
			}
		}

		public PedalAction Pedal
		{
			get
			{
				return m_Pedal;
			}
			set
			{
				m_Pedal = value;
			}
		}

		public dynamic Clone()
		{
			CToolStartMode_HA cls_Clonado = new CToolStartMode_HA();
			cls_Clonado.ToolButton = this.ToolButton;
			cls_Clonado.Pedal = this.Pedal;

			return cls_Clonado;
		}

	}
}
