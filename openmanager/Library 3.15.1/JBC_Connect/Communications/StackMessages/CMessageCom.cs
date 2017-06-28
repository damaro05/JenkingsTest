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
    /// <summary>
    /// Message used in the communication. Provides information like FID and Number Message
    /// </summary>
    public class CMessageCom
    {

        public const int MAX_MESSAGE_TRIES = 3;

        private uint m_NumberMessage;
        private byte m_FID;
        private byte m_Command;
        private byte[] m_Data;
        private byte m_Address;
        private bool m_Response;
        private int m_TriesRemaining;
        private bool m_DelayedResponse;


        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="_NumberMessage">Number message</param>
        /// <param name="_FID">FID</param>
        /// <param name="_Command">Protocol command</param>
        /// <param name="_Data">Message's data</param>
        /// <param name="_Address">Station address</param>
        /// <param name="_Response">True if the message is a response</param>
        public CMessageCom(uint _NumberMessage, byte _FID, byte _Command, byte[] _Data, byte _Address, bool _Response = false)
        {
            m_NumberMessage = _NumberMessage;
            m_FID = _FID;
            m_Command = _Command;
            m_Data = _Data;
            m_Address = _Address;
            m_Response = _Response;
            m_TriesRemaining = MAX_MESSAGE_TRIES;
            m_DelayedResponse = false;
        }

        /// <summary>
        /// Get the Number Message
        /// </summary>
        /// <returns>Number Message</returns>
        public uint NumberMessage()
        {
            return m_NumberMessage;
        }

        /// <summary>
        /// Get the FID
        /// </summary>
        /// <returns>Message's FID</returns>
        public byte FID()
        {
            return m_FID;
        }

        /// <summary>
        /// Get the command
        /// </summary>
        /// <returns>Protocol command</returns>
        public byte Command()
        {
            return m_Command;
        }

        /// <summary>
        /// Get the Message's data
        /// </summary>
        /// <returns>Byte data</returns>
        public byte[] Data()
        {
            return m_Data;
        }

        /// <summary>
        /// Get/Set the Station Address
        /// </summary>
        /// <value>Destination address</value>
        public byte Address
        {
            get
            {
                return m_Address;
            }
            set
            {
                m_Address = value;
            }
        }

        /// <summary>
        /// Information about if the message is a response
        /// </summary>
        /// <returns>True if the message is a response</returns>
        public bool Response()
        {
            return m_Response;
        }

        /// <summary>
        /// Get tries remaining to resent the message
        /// </summary>
        /// <returns>Tries remaining to resent the message</returns>
        /// <remarks></remarks>
        public int TriesRemaining()
        {
            return m_TriesRemaining;
        }

        /// <summary>
        /// Decrement tries remaining to resent the message
        /// </summary>
        public void DecrementTriesRemaining()
        {
            m_TriesRemaining--;
        }

        /// <summary>
        /// Get/Set if wait a response from the station
        /// </summary>
        /// <value></value>
        public bool DelayedResponse
        {
            get
            {
                return m_DelayedResponse;
            }
            set
            {
                m_DelayedResponse = value;
            }
        }

    }
}
