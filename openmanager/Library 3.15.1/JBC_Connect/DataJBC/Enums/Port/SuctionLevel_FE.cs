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
	
	
	/// <summary>
	/// Suction level
	/// </summary>
	/// <remarks></remarks>
	[FlagsAttribute()]public enum SuctionLevel_FE : byte
	{
		HIGH = 1,
		MEDIUM = 2,
		LOW = 3,
		CUSTOM = 4
	}
	
}
