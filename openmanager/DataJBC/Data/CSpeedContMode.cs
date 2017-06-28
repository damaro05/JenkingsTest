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

	public class CSpeedContMode
	{

		private const string PREFIX = "T_";
		private const string SUFFIX = "mS";


		public int SpeedFromEnum(SpeedContinuousMode speed)
		{
			int ret = 0;
			string str = System.Convert.ToString(speed.ToString());

			if (str.StartsWith("T_"))
			{
				str = str.Replace(PREFIX, "");
				str = str.Replace(SUFFIX, "");
				ret = int.Parse(str);
			}

			return ret;
		}

		public SpeedContinuousMode EnumFromSpeed(int speed)
		{
			SpeedContinuousMode ret = SpeedContinuousMode.OFF;
			string str = PREFIX + speed.ToString() + SUFFIX;

			if (((IList)(Enum.GetNames(typeof(SpeedContinuousMode)))).Contains(str))
			{
				ret = (SpeedContinuousMode)(Enum.Parse(typeof(SpeedContinuousMode), str));
			}

			return ret;
		}

	}
}

