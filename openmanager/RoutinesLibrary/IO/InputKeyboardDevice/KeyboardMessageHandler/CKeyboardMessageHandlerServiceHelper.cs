// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.Windows.Forms;
using System.Threading;

namespace RoutinesLibrary
{
    internal class CKeyboardMessageHandlerServiceHelper
    {

        private Thread messagePump;
        private AutoResetEvent messagePumpRunning = new AutoResetEvent(false);

        public delegate void KeyboardConnectedEventHandler(string deviceName);
        private KeyboardConnectedEventHandler KeyboardConnectedEvent;

        public event KeyboardConnectedEventHandler KeyboardConnected
        {
            add
            {
                KeyboardConnectedEvent = (KeyboardConnectedEventHandler)System.Delegate.Combine(KeyboardConnectedEvent, value);
            }
            remove
            {
                KeyboardConnectedEvent = (KeyboardConnectedEventHandler)System.Delegate.Remove(KeyboardConnectedEvent, value);
            }
        }

        public delegate void KeyboardDisconnectedEventHandler(string deviceName);
        private KeyboardDisconnectedEventHandler KeyboardDisconnectedEvent;

        public event KeyboardDisconnectedEventHandler KeyboardDisconnected
        {
            add
            {
                KeyboardDisconnectedEvent = (KeyboardDisconnectedEventHandler)System.Delegate.Combine(KeyboardDisconnectedEvent, value);
            }
            remove
            {
                KeyboardDisconnectedEvent = (KeyboardDisconnectedEventHandler)System.Delegate.Remove(KeyboardDisconnectedEvent, value);
            }
        }

        public delegate void KeyboardMessageEventHandler(CKeyboardMessage message);
        private KeyboardMessageEventHandler KeyboardMessageEvent;

        public event KeyboardMessageEventHandler KeyboardMessage
        {
            add
            {
                KeyboardMessageEvent = (KeyboardMessageEventHandler)System.Delegate.Combine(KeyboardMessageEvent, value);
            }
            remove
            {
                KeyboardMessageEvent = (KeyboardMessageEventHandler)System.Delegate.Remove(KeyboardMessageEvent, value);
            }
        }



        public CKeyboardMessageHandlerServiceHelper()
        {
            messagePump = new Thread(new System.Threading.ThreadStart(RunMessagePump));
            messagePump.Start();
            messagePumpRunning.WaitOne();
        }

        private void RunMessagePump()
        {
            CKeyboardMessageHandler messageHandler = new CKeyboardMessageHandler();

            messageHandler.KeyboardConnected += event_KeyboardConnected;
            messageHandler.KeyboardDisconnected += event_KeyboardDisconnected;
            messageHandler.KeyboardMessage += event_KeyboardMessage;

            messagePumpRunning.Set();
            Application.Run();
        }

        private void event_KeyboardConnected(string deviceName)
        {
            if (KeyboardConnectedEvent != null)
                KeyboardConnectedEvent(deviceName);
        }

        private void event_KeyboardDisconnected(string deviceName)
        {
            if (KeyboardDisconnectedEvent != null)
                KeyboardDisconnectedEvent(deviceName);
        }

        private void event_KeyboardMessage(CKeyboardMessage message)
        {
            if (KeyboardMessageEvent != null)
                KeyboardMessageEvent(message);
        }

    }
}
