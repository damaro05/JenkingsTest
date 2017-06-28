// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Threading;

namespace RoutinesLibrary
{
    /// <summary>
    /// Handles raw input from keyboard devices
    /// </summary>
    public class CInputKeyboardDevice
    {

        //The following constants are defined in Windows.h

        private const int RIDEV_INPUTSINK = 0x100;
        private const int RID_INPUT = 0x10000003;

        private const int FAPPCOMMAND_MASK = 0xF000;
        private const int FAPPCOMMAND_MOUSE = 0x8000;
        private const int FAPPCOMMAND_OEM = 0x1000;

        private const int RIM_TYPEMOUSE = 0;
        private const int RIM_TYPEKEYBOARD = 1;
        private const int RIM_TYPEHID = 2;

        private const int RIDI_DEVICENAME = 0x20000007;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_INPUT = 0xFF;
        private const int VK_OEM_CLEAR = 0xFE;
        private const int VK_LAST_KEY = VK_OEM_CLEAR; //this is a made up value used as a sentinel


        /// <summary>
        /// List of keyboard devices
        /// </summary>
        /// <remarks>
        /// Key: the device handle
        /// Value: the device info class
        /// </remarks>
        private Hashtable m_deviceList = new Hashtable();
        private static Semaphore m_mutexDeviceList = new Semaphore(1, 1);

        /// <summary>
        /// The delegate to handle KeyPressed events
        /// </summary>
        /// <param name="sender">The object sending the event</param>
        /// <param name="e">A set of KeyControlEventArgs information about the key that was pressed and the device it was on</param>
        public delegate void DeviceEventHandler(object sender, CKeyControlEventArgs e);

        /// <summary>
        /// The event raised when InputDevice detects that a key was pressed
        /// </summary>
        private DeviceEventHandler KeyPressedEvent;
        public event DeviceEventHandler KeyPressed
        {
            add
            {
                KeyPressedEvent = (DeviceEventHandler)System.Delegate.Combine(KeyPressedEvent, value);
            }
            remove
            {
                KeyPressedEvent = (DeviceEventHandler)System.Delegate.Remove(KeyPressedEvent, value);
            }
        }



        [DllImport("user32.dll")]
        public static extern uint GetRawInputDeviceList(IntPtr pRawInputDeviceList, ref uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        public static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint uiCommand, IntPtr pData, ref uint pcbSize);

        [DllImport("user32.dll")]
        public static extern bool RegisterRawInputDevices([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0)]RAWINPUTDEVICE[] pRawInputDevices, int uiNumDevices, int cbSize);

        [DllImport("user32.dll")]
        public static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);


        /// <summary>
        /// Class constructor. Registers the raw input devices for the calling window
        /// </summary>
        /// <param name="hwnd">Handle of the window listening for key presses</param>
        public CInputKeyboardDevice(IntPtr hwnd)
        {

            //Create an array of all the raw input devices we want to listen to. In this case, only keyboard devices
            //RIDEV_INPUTSINK determines that the window will continue to receive messages even when it doesn't have the focus
            RAWINPUTDEVICE[] rid = new RAWINPUTDEVICE[1];

            rid[0].usUsagePage = System.Convert.ToUInt16(0x1);
            rid[0].usUsage = System.Convert.ToUInt16(0x6);
            rid[0].dwFlags = RIDEV_INPUTSINK;
            rid[0].hwndTarget = hwnd;

            if (!RegisterRawInputDevices(rid, rid.Length, Marshal.SizeOf(rid[0])))
            {
                throw (new ApplicationException("Failed to register raw input device(s)."));
            }
        }

        /// <summary>
        /// Iterates through the list provided by GetRawInputDeviceList, counting keyboard devices and adding them to deviceList
        /// </summary>
        /// <returns>The number of keyboard devices found</returns>
        public int EnumerateDevices()
        {
            int NumberOfDevices = 0;
            uint deviceCount = (uint)0;
            int dwSize = Marshal.SizeOf(typeof(RAWINPUTDEVICELIST));

            //Get the number of raw input devices in the list, then allocate sufficient memory and get the entire list
            if (GetRawInputDeviceList(IntPtr.Zero, ref deviceCount, (uint)dwSize) == 0)
            {
                IntPtr pRawInputDeviceList = Marshal.AllocHGlobal((int)(dwSize * deviceCount));
                GetRawInputDeviceList(pRawInputDeviceList, ref deviceCount, (uint)dwSize);

                m_mutexDeviceList.WaitOne();
                m_deviceList.Clear();

                //Iterate through the list, discarding undesired items and retrieving further information on keyboard devices
                for (var i = 0; i <= deviceCount - 1; i++)
                {

                    CDeviceInfo dInfo = default(CDeviceInfo);
                    string deviceName = "";
                    uint pcbSize = (uint)0;

                    RAWINPUTDEVICELIST rid = (RAWINPUTDEVICELIST)(Marshal.PtrToStructure(new IntPtr(pRawInputDeviceList.ToInt32() + (dwSize * i)), typeof(RAWINPUTDEVICELIST)));

                    GetRawInputDeviceInfo(rid.hDevice, (uint)RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

                    if (pcbSize > 0)
                    {
                        IntPtr pData = Marshal.AllocHGlobal((int)pcbSize);
                        GetRawInputDeviceInfo(rid.hDevice, (uint)RIDI_DEVICENAME, pData, ref pcbSize);
                        deviceName = (string)Marshal.PtrToStringAnsi(pData);

                        //Drop the "root" keyboard and mouse devices used for Terminal Services and the Remote Desktop
                        if (deviceName.ToUpper().Contains("ROOT"))
                        {
                            continue;
                        }

                        //If the device is identified in the list as a keyboard or HID device, create a DeviceInfo object to store information about it
                        if (rid.dwType == RIM_TYPEKEYBOARD | rid.dwType == RIM_TYPEHID)
                        {
                            dInfo = new CDeviceInfo();

                            dInfo.deviceName = Marshal.PtrToStringAnsi(pData);
                            dInfo.deviceHandle = rid.hDevice;
                            dInfo.deviceType = GetDeviceType(rid.dwType);

                            //Check the Registry to retrieve a more friendly description.
                            string DeviceDesc = ReadReg(deviceName);
                            dInfo.Name = DeviceDesc;

                            //If it isn't already in the list, add it to the deviceList hashtable and increase the NumberOfDevices count
                            if (!m_deviceList.Contains(rid.hDevice))
                            {
                                NumberOfDevices++;
                                m_deviceList.Add(rid.hDevice, dInfo);
                            }
                        }

                        Marshal.FreeHGlobal(pData);
                    }
                }
                m_mutexDeviceList.Release();

                Marshal.FreeHGlobal(pRawInputDeviceList);

                return NumberOfDevices;
            }
            else
            {
                throw (new ApplicationException("An error occurred while retrieving the list of devices."));
            }
        }

        /// <summary>
        /// Get a list of all keyboard devices founded
        /// </summary>
        /// <returns>List of all keyboard devices founded</returns>
        public List<string> GetDevices()
        {
            List<string> listDevices = new List<string>();

            m_mutexDeviceList.WaitOne();
            foreach (DictionaryEntry entryDevice in m_deviceList)
            {
                listDevices.Add(((CDeviceInfo)entryDevice.Value).deviceName);
            }
            m_mutexDeviceList.Release();

            return listDevices;
        }

        /// <summary>
        /// Filters Windows messages for WM_INPUT messages and calls ProcessInputCommand if necessary
        /// </summary>
        /// <param name="message">The Windows message</param>
        public void ProcessMessage(Message message)
        {
            switch (message.Msg)
            {
                case WM_INPUT:
                    ProcessInputCommand(message);
                    break;
            }
        }

        /// <summary>
        /// Converts a RAWINPUTDEVICELIST dwType value to a string describing the device type
        /// </summary>
        /// <param name="device">A dwType value (RIM_TYPEMOUSE, RIM_TYPEKEYBOARD or RIM_TYPEHID)</param>
        /// <returns>A string representation of the input value</returns>
        private string GetDeviceType(int device)
        {
            string deviceType = "";

            switch (device)
            {
                case RIM_TYPEMOUSE:
                    deviceType = "MOUSE";
                    break;
                case RIM_TYPEKEYBOARD:
                    deviceType = "KEYBOARD";
                    break;
                case RIM_TYPEHID:
                    deviceType = "HID";
                    break;
                default:
                    deviceType = "UNKNOWN";
                    break;
            }

            return deviceType;
        }

        /// <summary>
        /// Determines what type of device triggered a WM_INPUT message (Used in the ProcessInputCommand method)
        /// </summary>
        /// <param name="param">The LParam from a WM_INPUT message</param>
        /// <returns>A DeviceType enum value</returns>
        private InputDeviceType GetDevice(int param)
        {
            InputDeviceType deviceType = InputDeviceType.Key;

            try
            {
                switch ((int)(((ushort)(param >> 16)) & FAPPCOMMAND_MASK))
                {
                    case FAPPCOMMAND_OEM:
                        deviceType = InputDeviceType.OEM;
                        break;
                    case FAPPCOMMAND_MOUSE:
                        deviceType = InputDeviceType.Mouse;
                        break;
                    default:
                        deviceType = InputDeviceType.Key;
                        break;
                }
            }
            catch (Exception)
            {

            }

            return deviceType;
        }

        /// <summary>
        /// Reads the Registry to retrieve a friendly description of the device
        /// </summary>
        /// <param name="item">The device name to search for, as provided by GetRawInputDeviceInfo</param>
        /// <returns>The device description stored in the Registry entry's DeviceDesc value</returns>
        private string ReadReg(string item)
        {

            //Example Device Identification string
            //@"\??\ACPI#PNP0303#3&13c0b0c5&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";

            //remove the \??\
            item = item.Substring(4);

            string[] split = item.Split('#');

            string id_01 = split[0]; //ACPI (Class code)
            string id_02 = split[1]; //PNP0303 (SubClass code)
            string id_03 = split[2]; //3&13c0b0c5&0 (Protocol code)
                                     //The final part is the class GUID and is not needed here

            //Open the appropriate key as read-only so no permissions are needed
            RegistryKey OurKey = Registry.LocalMachine;

            string findme = string.Format("System\\CurrentControlSet\\Enum\\{0}\\{1}\\{2}", id_01, id_02, id_03);
            OurKey = OurKey.OpenSubKey(findme, false);

            //Retrieve the desired information and set isKeyboard
            string deviceDesc = (OurKey.GetValue("DeviceDesc")).ToString();

            return deviceDesc;
        }

        /// <summary>
        /// Processes WM_INPUT messages to retrieve information about any keyboard events that occur
        /// </summary>
        /// <param name="message">The WM_INPUT message to process</param>
        private void ProcessInputCommand(Message message)
        {
            uint dwSize = (uint)0;

            //First call to GetRawInputData sets the value of dwSize, which can then be used to allocate the appropriate amount of memory, storing the pointer in "buffer"
            GetRawInputData(message.LParam, (uint)RID_INPUT, IntPtr.Zero, ref dwSize, System.Convert.ToUInt32((uint)(Marshal.SizeOf(typeof(RAWINPUTHEADER)))));

            IntPtr buffer = Marshal.AllocHGlobal((int)dwSize);
            try
            {

                //Check that buffer points to something, and if so, call GetRawInputData again to fill the allocated memory with information about the input
                if (buffer != IntPtr.Zero && GetRawInputData(message.LParam, (uint)RID_INPUT, buffer, ref dwSize, System.Convert.ToUInt32((uint)(Marshal.SizeOf(new RAWINPUTHEADER())))) == dwSize)
                {

                    //Store the message information in "raw", then check that the input comes from a keyboard device before processing it to raise an appropriate KeyPressed event
                    RAWINPUT raw = (RAWINPUT)(Marshal.PtrToStructure(buffer, typeof(RAWINPUT)));

                    if (raw.header.dwType == RIM_TYPEKEYBOARD)
                    {

                        //Filter for Key Down events and then retrieve information about the keystroke
                        if (raw.keyboard.Message == WM_KEYDOWN || raw.keyboard.Message == WM_SYSKEYDOWN)
                        {

                            ushort key = raw.keyboard.VKey;

                            //On most keyboards, "extended" keys such as the arrow or page keys return two codes - the key's own code, and an
                            //"extended key" flag, which translates to 255. This flag isn't useful to us, so it can be disregarded
                            if (key > VK_LAST_KEY)
                            {
                                return;
                            }

                            //Retrieve information about the device and the key that was pressed
                            CDeviceInfo dInfo = null;

                            m_mutexDeviceList.WaitOne();
                            if (m_deviceList.Contains(raw.header.hDevice))
                            {
                                Keys myKey = System.Windows.Forms.Keys.A;

                                dInfo = (CDeviceInfo)(m_deviceList[raw.header.hDevice]);
                                myKey = (Keys)(Enum.Parse(typeof(Keys), Enum.GetName(typeof(Keys), key)));
                                dInfo.vKey = myKey.ToString();
                                dInfo.key = key;
                            }
                            else
                            {
                                string errMessage = string.Format("Handle :{0} was not in hashtable. The device may support more than one handle or usage page, and is probably not a standard keyboard.", raw.header.hDevice);
                                throw (new ApplicationException(errMessage));
                            }
                            m_mutexDeviceList.Release();

                            //If the key that was pressed is valid and there was no problem retrieving information on the device, raise the KeyPressed event
                            if (dInfo != null)
                            {
                                if (KeyPressedEvent != null)
                                    KeyPressedEvent(this, new CKeyControlEventArgs(dInfo, GetDevice(message.LParam.ToInt32())));
                            }
                            else
                            {
                                string errMessage = string.Format("Received Unknown Key: {0}. Possibly an unknown device", key);
                                throw (new ApplicationException(errMessage));
                            }
                        }
                    }
                }

            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

    }
}
