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

namespace RoutinesLibrary
{
    internal class CKeyboardMessageHandler : NativeWindow
    {

        private const int TIME_SEARCH_KEYBOARDS = 3000;

        private CInputKeyboardDevice m_inputKeyboard;
        private System.Timers.Timer m_timerSearchKeyboards;
        private Hashtable m_htKeyboardsMessages = new Hashtable(); //Contiene las teclas pulsadas para cada teclado

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



        public CKeyboardMessageHandler()
        {
            CreateHandle(new CreateParams());

            //Input keyboard
            m_inputKeyboard = new CInputKeyboardDevice(this.Handle);
            m_inputKeyboard.EnumerateDevices();
            m_inputKeyboard.KeyPressed += new CInputKeyboardDevice.DeviceEventHandler(KeyPressed);

            //Refresh keyboard devices
            m_timerSearchKeyboards = new System.Timers.Timer(TIME_SEARCH_KEYBOARDS);
            m_timerSearchKeyboards.Elapsed += RefreshListKeyboards;
            m_timerSearchKeyboards.Start();
        }

        protected override void WndProc(ref Message message)
        {
            if (m_inputKeyboard != null)
            {
                m_inputKeyboard.ProcessMessage(message);
            }

            base.WndProc(ref message);
        }

        private void KeyPressed(object sender, CKeyControlEventArgs e)
        {

            if (m_htKeyboardsMessages.Contains(e.Keyboard.deviceName))
            {
                if (e.Keyboard.key == (int)Keys.Enter)
                {
                    CKeyboardMessage message = new CKeyboardMessage();
                    message.deviceName = e.Keyboard.deviceName;
                    message.message = (m_htKeyboardsMessages[e.Keyboard.deviceName]).ToString();

                    if (KeyboardMessageEvent != null)
                        KeyboardMessageEvent(message);
                    m_htKeyboardsMessages[e.Keyboard.deviceName] = "";
                }
                else
                {
                    m_htKeyboardsMessages[e.Keyboard.deviceName] = (m_htKeyboardsMessages[e.Keyboard.deviceName]).ToString() + e.Keyboard.vKey;
                }
            }
        }

        private void RefreshListKeyboards(object sender, EventArgs e)
        {

            //Refresh list keyboards
            m_inputKeyboard.EnumerateDevices();
            List<string> listKeyboard = m_inputKeyboard.GetDevices();

            //Keyboards connections
            foreach (string deviceInfo in listKeyboard)
            {
                if (!m_htKeyboardsMessages.Contains(deviceInfo))
                {
                    m_htKeyboardsMessages.Add(deviceInfo, "");
                    if (KeyboardConnectedEvent != null)
                        KeyboardConnectedEvent(deviceInfo);
                }
            }

            //Keyboards disconnections
            ArrayList keys = new ArrayList(m_htKeyboardsMessages.Keys);
            foreach (string key in keys)
            {
                if (!listKeyboard.Contains(key))
                {
                    m_htKeyboardsMessages.Remove(key);
                    if (KeyboardDisconnectedEvent != null)
                        KeyboardDisconnectedEvent(key);
                }
            }
        }

    }
}
