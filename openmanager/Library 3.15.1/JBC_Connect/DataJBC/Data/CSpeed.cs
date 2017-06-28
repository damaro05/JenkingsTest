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


namespace DataJBC
{
	
	
	public class CSpeed
	{
		
		public enum SpeedUnit
		{
			InchesPerSecond = 0,
			MillimetersPerSecond = 1
		}
		
		private const int STEP_SPEED = 5;
		
		
		private double m_millimeters;
		
		
		public CSpeed()
		{
			m_millimeters = 0;
		}
		
#region Millimeters
		
		public void InMillimetersPerSecond(double value)
		{
			m_millimeters = (double) RoutinesLibrary.Data.DataType.IntegerUtils.RoundNumber(System.Convert.ToInt32(value * 10), STEP_SPEED) / 10;
		}
		
		public double ToMillimetersPerSecond()
		{
			return m_millimeters;
		}
		
#endregion
		
#region Inches
		
		public void InInchesPerSecond(double value)
		{
			m_millimeters = (double) RoutinesLibrary.Data.DataType.IntegerUtils.RoundNumber(System.Convert.ToInt32(value * 2.54 * 10), STEP_SPEED) / 10;
		}
		
		public double ToInchesPerSecond()
		{
			return m_millimeters / 2.54;
		}
		
#endregion
		
	}
	
}
