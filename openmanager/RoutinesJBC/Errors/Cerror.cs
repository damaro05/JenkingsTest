// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Collections;
using System.Linq;
// End of VB project level imports

namespace RoutinesJBC
{

	/// <summary>
	/// The general error object used in this library. It is used for storing and showing errors.
	/// </summary>
	/// <remarks></remarks>
	public class Cerror : Exception
	{

		/// <summary>
		/// The list of all posible user error codes
		/// </summary>
		/// <remarks></remarks>
		public enum cErrorCodes
		{
			STATION_UUID_NOT_FOUND = 1,
			CONTINUOUS_MODE_ON_WITHOUT_PORTS = 2,
			PORT_NOT_IN_RANGE = 3,
			INVALID_STATION_NAME = 4,
			INVALID_STATION_PIN = 5,
			TEMPERATURE_OUT_OF_RANGE = 6,
			POWER_LIMIT_OUT_OF_RANGE = 7,
			TOOL_NOT_SUPPORTED = 8,
			FUNCTION_NOT_SUPPORTED = 9,
			COMMUNICATION_ERROR = 10,
			PERIPHERAL_NOT_IN_RANGE = 11,
			FLOW_OUT_OF_RANGE = 12,
			TIME_OUT_OF_RANGE = 13,
			PROGRAM_OUT_OF_RANGE = 14
		}

		/// <summary>
		/// The list of station/PC protocol error codes
		/// </summary>
		/// <remarks></remarks>
		public enum cCommErrorCodes
		{
			NO_COMM_ERROR = 0,
			BCC = 1,
			FRAME_FORMAT = 2,
			OUT_OF_RANGE = 3,
			COMMAND_REJECTED = 4,
			CONTROL_MODE_REQUIRED = 5,
			INCORRECT_SEQUENCY = 6,
			FLASH_WRITE_ERROR = 7,
			CONTROL_MODE_ALREADY_ACTIVATED = 8,
			NOT_VALID_HARDWARE = 9,
			INTERNAL_ERROR = 10
		}

		private cErrorCodes m_Code;
		private cCommErrorCodes m_CommCode;
		//Private m_CommErrorData As cCommErrorCodes
		private byte[] m_CommErrorData;
		private string m_Msg;

		/// <summary>
		/// Creates an error object
		/// </summary>
		/// <param name="code">The code for the error</param>
		/// <param name="msg">The message for the error</param>
		/// <remarks></remarks>
		public Cerror(cErrorCodes code, string msg)
		{
			m_Code = code;
			m_Msg = msg;
			//m_CommErrorData = cCommErrorCodes.NO_COMM_ERROR
		}

		/// <summary>
		/// Creates an error object with communication error code
		/// </summary>
		/// <param name="code">The code for the error</param>
		/// <param name="msg">The message for the error</param>
		/// <remarks></remarks>
		public Cerror(cErrorCodes code, string msg, byte[] CommErrorData)
		{
			//Public Sub New(ByVal code As cErrorCodes, ByVal msg As String, ByVal CommErrorData As cCommErrorCodes)
			m_Code = code;
			m_Msg = msg;
			m_CommErrorData = CommErrorData;
		}

		/// <summary>
		/// Gets the error message.
		/// </summary>
		/// <returns>The error message</returns>
		/// <remarks></remarks>
		public string GetMsg()
		{
			return m_Msg;
		}

		/// <summary>
		/// Gets the error code.
		/// </summary>
		/// <returns>The error code</returns>
		/// <remarks></remarks>
		public cErrorCodes GetCode()
		{
			return m_Code;
		}

		/// <summary>
		/// Gets the error code.
		/// </summary>
		/// <returns>The error code</returns>
		/// <remarks></remarks>
		public cCommErrorCodes GetCommErrorCode()
		{
			if (m_CommErrorData != null && m_CommErrorData.Length > 0)
			{
				return ((cCommErrorCodes)(m_CommErrorData[0]));
			}
			else
			{
				return cCommErrorCodes.NO_COMM_ERROR;
			}
			//    Return m_CommErrorData
		}

	}
}

