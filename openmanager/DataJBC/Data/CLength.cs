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
	public class CLength
	{
		public enum LengthUnit
		{
			Inches = 0,
			Millimeters = 1
		}
		
		private const int STEP_LENGTH = 5;


		private double m_millimeters;


		public CLength()
		{
			m_millimeters = 0;
		}

#region Millimeters

		public void InMillimeters(double value)
		{
			m_millimeters = (double) RoutinesLibrary.Data.DataType.IntegerUtils.RoundNumber(System.Convert.ToInt32(value * 10), STEP_LENGTH) / 10;
		}

		public double ToMillimeters()
		{
			return m_millimeters;
		}

#endregion

#region Inches

		public void InInches(double value)
		{
			m_millimeters = (double) RoutinesLibrary.Data.DataType.IntegerUtils.RoundNumber(System.Convert.ToInt32(value * 2.54 * 10), STEP_LENGTH) / 10;
		}

		public double ToInches()
		{
			return m_millimeters / 2.54;
		}

#endregion

	}
}
