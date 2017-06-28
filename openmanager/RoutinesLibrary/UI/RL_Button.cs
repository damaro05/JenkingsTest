// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


namespace RoutinesLibrary.UI
{
		
	public class RL_Button : System.Windows.Forms.Button
	{
			
		private int HOLD_ON_START = 300;
		private int HOLD_ON_INTERVAL = 100;
			
		private Timer m_timer = new Timer();
		private uint m_ticks;
			
		public delegate void HoldOnEventHandler(object sender, EventArgs e);
		private HoldOnEventHandler HoldOnEvent;
			
		public event HoldOnEventHandler HoldOn
		{
			add
			{
				HoldOnEvent = (HoldOnEventHandler) System.Delegate.Combine(HoldOnEvent, value);
			}
			remove
			{
				HoldOnEvent = (HoldOnEventHandler) System.Delegate.Remove(HoldOnEvent, value);
			}
		}
			
			
			
		public RL_Button()
		{
			m_timer.Interval = HOLD_ON_INTERVAL;
		}
			
		private void Event_MouseDown(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_ticks = (uint) 0;
			m_timer.Start();
		}
			
		private void Event_MouseUp(System.Object sender, System.Windows.Forms.MouseEventArgs e)
		{
			m_timer.Stop();
		}
			
		private void HoldOn_Tick(System.Object sender, System.EventArgs e)
		{
			m_ticks = System.Convert.ToUInt32(m_ticks + 1);
			if (m_ticks * HOLD_ON_INTERVAL >= HOLD_ON_START)
			{
				e = new EventArgs();
				if (HoldOnEvent != null)
					HoldOnEvent(this, e);
			}
		}
			
	}
		
}
