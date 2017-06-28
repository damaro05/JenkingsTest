// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

using System.Net;
using System.Text;
using Microsoft.VisualBasic.CompilerServices;
using System.Threading;
using JBC_Connect;
using DataJBC;
using RoutinesJBC;

// Programa para probar los protocolos de las estaciones
// Se utilizan los stacks sin modificar (apl, dll y phl)
// Dentro de los stacks existen "#if LibraryTest then" que habilitan los eventos de
// aviso de envío y recepción de los frames enteros. En este programa, en "My Project" se
// define la constante de compilación (tanto en debug como en release) "LibraryTest=true"
// 08/06/2015 Se añaden 2 tramas HBB y HBC - Read and Write Partameters Locked
// 06/08/2015 Se actualizan tramas del protocolo 02 según documento "Protocolo PS_v01.91.docx"
// 20/10/2015 Se actualizan tramas del protocolo 02 según documento "Protocolo PS_v02.20.docx"
// 20/10/2015 Se añade protocolo de JT desoladora según documento "Protocolo JTSE-PC_v0.2.pdf"
// 07/02/2017 Se añade protocolo de SF dispensador de estaño según docuemnto "Protocolo SF-PC_v1 1.docx"

namespace LibraryTest
{
    public partial class Form1
    {
        public Form1()
        {
            InitializeComponent();

            //Added to support default instance behavour in C#
            if (defaultInstance == null)
                defaultInstance = this;
        }

        #region Default Instance

        private static Form1 defaultInstance;

        /// <summary>
        /// Added by the VB.Net to C# Converter to support default instance behavour in C#
        /// </summary>
        public static Form1 Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new Form1();
                    defaultInstance.FormClosed += new FormClosedEventHandler(defaultInstance_FormClosed);
                }

                return defaultInstance;
            }
            set
            {
                defaultInstance = value;
            }
        }

        static void defaultInstance_FormClosed(object sender, FormClosedEventArgs e)
        {
            defaultInstance = null;
        }

        #endregion

        private List<cValueText3> listOrders = new List<cValueText3>();
        private List<cValueText2> listTools = new List<cValueText2>();
        private bool bLoadingCommands;

        private SearchDevicesUSB StationSearcherUSB = null;
        private CSearchDevicesTCP StationSearcherTCP = null;
        private CCommunicationChannel stackApl = null;
        private System.IO.Ports.SerialPort mySerialPort = null;
        private RoutinesLibrary.IO.SerialPortConfig mySerialPortConfig = new RoutinesLibrary.IO.SerialPortConfig();
        private bool bUpdateConfigFromControls = false;
        private RoutinesLibrary.Net.Protocols.TCP.TCP myWinSock = null;
        private CStationBase.Protocol myFrameProtocol;
        private CStationBase.Protocol myCommandProtocol;

        private CStationtableTCP discoveredStations;
        private CSearchUDP searchStationsUPD;
        private CFeaturesData features;

        // updater
        private string sUpdaterLastPathFile = "";
        // files
        private string sFilesLastPathFile = "";


        #region Enumerations and structures
        const string TConnect = "Connect";
        const string TDisconnect = "Disconnect";
        const string TControlMode = "Set Control Mode";
        const string TMonitorMode = "Set Monitor Mode";

        //Public Structure strOrder
        //    Public sOrder As String
        //End Structure

        #endregion

        public void Form1_Load(System.Object sender, System.EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            if (My.Settings.Default.UpgradeSettings)
            {
                My.Settings.Default.Upgrade();
                My.Settings.Default.UpgradeSettings = false;
                My.Settings.Default.Save();
            }

            //tbProgramFile.Text = "C:\MIDORI\Desarr\JBC\Laboratorio\Packer\Micros\Programs\HT6486\SBL (SEMILLA BOOTLOADER)\DDR\9996771\9996771.s19"
            //tbProgramFile.Text = "C:\MIDORI\Desarr\JBC\Laboratorio\Varios\Pruebas MicroPrograms\Application.afx.S19"
            loadConfigData();
            setControlsFromPortConfig();

            butConnectUSB.Text = TConnect;
            butConnectEth.Text = TConnect;
            butSendCommand.Enabled = false;
            cbUseOrders.Enabled = !butSendCommand.Enabled;
            myLoadSerialPorts();
            myLoadOrders_02();
            cbLogPort.SelectedIndex = 0;
            cbTempType.SelectedIndex = 1;
            myLoadtools();
            cbUseOrders.SelectedIndex = 0; // automatic
                                           // list of stations discovered by UPD
            discoveredStations = new CStationtableTCP();
            createMySearcherUDP();
            // updater
            updaterUI(enumUpdaterState.initial);

            this.ClearingFlashInProgress += new Form1.ClearingFlashInProgressEventHandler(event_ClearingFlashInProgress);
            this.ClearingFlashFinished += new Form1.ClearingFlashFinishedEventHandler(event_ClearingFlashFinished);
            this.AddressMemoryFlashFinished += new Form1.AddressMemoryFlashFinishedEventHandler(event_WrittenFlashAddress);
            this.DataMemoryFlashFinished += new Form1.DataMemoryFlashFinishedEventHandler(event_WrittenFlashData);
            this.UpdateMicroFirmwareFinished += new Form1.UpdateMicroFirmwareFinishedEventHandler(event_UpdateMicroFirmwareFinished);
            this.FileCount += new Form1.FileCountEventHandler(event_FileCount);
            this.FileName += new Form1.FileNameEventHandler(event_FileName);
            this.StartReadingFile += new Form1.StartReadingFileEventHandler(event_StartReadingFile);
            this.BlockReadingFile += new Form1.BlockReadingFileEventHandler(event_BlockReadingFile);
            this.EndReadingFile += new Form1.EndReadingFileEventHandler(event_EndReadingFile);
            this.StartWritingFile += new Form1.StartWritingFileEventHandler(event_StartWritingFile);
            this.BlockWritingFile += new Form1.BlockWritingFileEventHandler(event_BlockWritingFile);
            this.EndWritingFile += new Form1.EndWritingFileEventHandler(event_EndWritingFile);
            this.DeletedFileName += new Form1.DeletedFileNameEventHandler(event_DeletedFileName);
            this.CurrentSelectedFileName += new Form1.CurrentSelectedFileNameEventHandler(event_CurrentFileName);
        }

        public void Form1_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            eraseMySearcherUDP();
            eraseSearcherTCP();
            eraseSearcherUSB();
            eraseStack();

            saveConfigData();
        }

        private void createMySearcherUDP()
        {
            searchStationsUPD = new CSearchUDP();
            searchStationsUPD.StartSearch();
        }

        private void eraseMySearcherUDP()
        {
            if (searchStationsUPD != null)
            {
                searchStationsUPD.Eraser();
                searchStationsUPD = null;
            }
        }

        private void eraseStack()
        {
            if (stackApl != null)
            {
                stackApl.Dispose();
                stackApl = null;
                stackApl.SentFrame += stackApl_SentFrame;
                stackApl.ResponseFrame += stackApl_ResponseFrame;
                stackApl.SentRawData += stackApl_SentRawData;
                stackApl.ResponseRawData += stackApl_ResponseRawData;
                stackApl.MessageResponse += stackApl_Response;
                stackApl.ConnectionError += stackApl_ConnectError;
            }
        }

        private void loadConfigData()
        {
            tbProgramFile.Text = "";
            tbBorrarMemoriaFlash.Text = "";
            tbProgramFile.Text = System.Convert.ToString(My.Settings.Default.updLastProgramFile);
            tbBorrarMemoriaFlash.Text = System.Convert.ToString(My.Settings.Default.updLastDataToSend);
            sUpdaterLastPathFile = tbProgramFile.Text;
            try
            {
                sFilesLastPathFile = Strings.Trim(System.Convert.ToString(My.Settings.Default.filesLastPathFile));
            }
            catch (Exception)
            {
            }
            setPortConfigFromSettings();
        }

        private void saveConfigData()
        {
            My.Settings.Default.updLastProgramFile = tbProgramFile.Text;
            My.Settings.Default.updLastDataToSend = tbBorrarMemoriaFlash.Text;
            My.Settings.Default.filesLastPathFile = sFilesLastPathFile;
            setSettingsFromPortConfig();
            My.Settings.Default.Save();
        }

        private void setControlsFromPortConfig()
        {
            int idx = 0;
            int idxSel = 0;
            string sText = "";

            bUpdateConfigFromControls = false;

            cbPort_Speed.Items.Clear();
            idxSel = -1;
            foreach (var entry in System.Enum.GetValues(typeof(CRobotData.RobotSpeed)))
            {
                sText = entry.ToString().Replace("bps_", "");
                idx = cbPort_Speed.Items.Add(sText);
                if (int.Parse(sText) == mySerialPortConfig.BaudRate)
                {
                    idxSel = idx;
                }
            }
            if (idxSel == -1)
            {
                idxSel = cbPort_Speed.Items.Add(mySerialPortConfig.BaudRate.ToString());
            }
            cbPort_Speed.SelectedIndex = idxSel;

            cbPort_Parity.Items.Clear();
            idxSel = -1;
            foreach (var entry in System.Enum.GetValues(typeof(System.IO.Ports.Parity)))
            {
                sText = entry.ToString();
                idx = cbPort_Parity.Items.Add(sText);
                if (sText == mySerialPortConfig.Parity.ToString())
                {
                    idxSel = idx;
                }
            }
            cbPort_Parity.SelectedIndex = idxSel;

            cbPort_StopBits.Items.Clear();
            idxSel = -1;
            foreach (var entry in System.Enum.GetValues(typeof(System.IO.Ports.StopBits)))
            {
                sText = entry.ToString();
                idx = cbPort_StopBits.Items.Add(sText);
                if (sText == mySerialPortConfig.StopBits.ToString())
                {
                    idxSel = idx;
                }
            }
            cbPort_StopBits.SelectedIndex = idxSel;

            nudPort_DataBits.Value = mySerialPortConfig.DataBits;

            bUpdateConfigFromControls = true;
        }

        private void setPortConfigFromControls()
        {
            string sText = "";

            sText = System.Convert.ToString(cbPort_Speed.Items[cbPort_Speed.SelectedIndex]);
            mySerialPortConfig.BaudRate = int.Parse(sText);

            sText = System.Convert.ToString(cbPort_Parity.Items[cbPort_Parity.SelectedIndex]);
            mySerialPortConfig.Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), sText);

            sText = System.Convert.ToString(cbPort_StopBits.Items[cbPort_StopBits.SelectedIndex]);
            mySerialPortConfig.StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), sText);

            mySerialPortConfig.DataBits = (int)nudPort_DataBits.Value;

        }

        private void setPortConfigFromSettings()
        {
            try
            {
                mySerialPortConfig.BaudRate = System.Convert.ToInt32(My.Settings.Default.serialPort_Baudios);
                mySerialPortConfig.Parity = (System.IO.Ports.Parity)Enum.Parse(typeof(System.IO.Ports.Parity), My.Settings.Default.serialPort_Parity);
                mySerialPortConfig.StopBits = (System.IO.Ports.StopBits)Enum.Parse(typeof(System.IO.Ports.StopBits), My.Settings.Default.serialPort_StopBits);
                mySerialPortConfig.DataBits = System.Convert.ToInt32(My.Settings.Default.serialPort_DataBits);
            }
            catch (Exception)
            {
            }
        }

        private void setSettingsFromPortConfig()
        {
            try
            {
                My.Settings.Default.serialPort_Baudios = mySerialPortConfig.BaudRate.ToString();
                My.Settings.Default.serialPort_Parity = mySerialPortConfig.Parity.ToString();
                My.Settings.Default.serialPort_StopBits = mySerialPortConfig.StopBits.ToString();
                My.Settings.Default.serialPort_DataBits = mySerialPortConfig.DataBits.ToString();
            }
            catch (Exception)
            {
            }
        }

        public void SerialPortConfigChanged(object sender, EventArgs e)
        {
            if (bUpdateConfigFromControls)
            {
                setPortConfigFromControls();
            }
        }

        public void butSerialPortConfigDefaults_Click(object sender, EventArgs e)
        {
            mySerialPortConfig.SetDefaults();
            setControlsFromPortConfig();
        }

        public void butConnectUSB_Click(System.Object sender, System.EventArgs e)
        {
            eraseStack();
            eraseSearcherUSB();
            eraseSearcherTCP();

            if (butConnectUSB.Text == TConnect)
            {
                if (cbSerialPorts.SelectedIndex == 0)
                {
                    StationSearcherUSB = new SearchDevicesUSB("", mySerialPortConfig, rbConnectUSB_StartPC.Checked);
                    StationSearcherUSB.NewConnection += StationSearcherUSB_NewConnection;
                    StationSearcherUSB.NoConnectionOnPort += StationSearcherUSB_NoConnection;
                    StationSearcherUSB.DataSentRawDataSearch += StationSearcherUSB_DataSentRawDataSearch;
                    StationSearcherUSB.DataReceivedRawDataSearch += StationSearcherUSB_DataReceivedRawDataSearch;
                }
                else
                {
                    StationSearcherUSB = new SearchDevicesUSB(cbSerialPorts.Text, mySerialPortConfig, rbConnectUSB_StartPC.Checked);
                    StationSearcherUSB.NewConnection += StationSearcherUSB_NewConnection;
                    StationSearcherUSB.NoConnectionOnPort += StationSearcherUSB_NoConnection;
                    StationSearcherUSB.DataSentRawDataSearch += StationSearcherUSB_DataSentRawDataSearch;
                    StationSearcherUSB.DataReceivedRawDataSearch += StationSearcherUSB_DataReceivedRawDataSearch;
                }
                if (rbConnectUSB_StartPC.Checked)
                {
                    butConnectUSB.Text = TDisconnect;
                    butConnectUSB.Enabled = true; // habilitar disconnect
                }
                else
                {
                    butConnectUSB.Enabled = false; // espera a conectar para habilitar disconnect
                }
            }
            else
            {
                butConnectUSB.Text = TConnect;
                butSendCommand.Enabled = false;
                cbUseOrders.Enabled = !butSendCommand.Enabled;
                butConnectEth.Enabled = true;
            }

        }

        public void butConnectEth_Click(System.Object sender, System.EventArgs e)
        {
            eraseStack();
            eraseSearcherTCP();
            eraseSearcherUSB();

            if (butConnectEth.Text == TConnect)
            {
                if (cbEndPoints.SelectedIndex == 0)
                {
                    StationSearcherTCP = new CSearchDevicesTCP("", searchStationsUPD);
                    StationSearcherTCP.NewConnection += StationSearcherTCP_NewConnection;
                    StationSearcherTCP.NoConnectionOnEndPoint += StationSearcherTCP_NoConnection;
                }
                else
                {
                    StationSearcherTCP = new CSearchDevicesTCP(cbEndPoints.Text, searchStationsUPD);
                    StationSearcherTCP.NewConnection += StationSearcherTCP_NewConnection;
                    StationSearcherTCP.NoConnectionOnEndPoint += StationSearcherTCP_NoConnection;
                }
                butConnectEth.Enabled = false;
            }
            else
            {
                butConnectEth.Text = TConnect;
                butSendCommand.Enabled = false;
                cbUseOrders.Enabled = !butSendCommand.Enabled;
                butConnectUSB.Enabled = true;
            }
        }

        public void butControlMode_Click(object sender, EventArgs e)
        {
            // control or monitor
            if (butControlMode.Text == TControlMode)
            {
                setMode(true);
                butControlMode.Text = TMonitorMode;
            }
            else
            {
                setMode(false);
                butControlMode.Text = TControlMode;
            }
        }

        private void setMode(bool bControl)
        {
            string sHexaCommand = "";
            string sData = "";
            string sCM = "";
            byte[] SendData = null;
            byte[] PortAndTool = null;
            byte[] tempData = null;
            byte Command = 0;

            switch (myCommandProtocol)
            {
                case CStationBase.Protocol.Protocol_01:
                    if (bControl)
                    {
                        sCM = "C";
                    }
                    else
                    {
                        sCM = "M";
                    }
                    if (mySerialPort != null)
                    {
                        sHexaCommand = "&H1F";
                        sData = sCM;
                    }
                    else
                    {
                        return;
                    }
                    break;
                case CStationBase.Protocol.Protocol_02:
                    if (bControl)
                    {
                        sCM = "C";
                    }
                    else
                    {
                        sCM = "M";
                    }
                    sData = Environment.MachineName + ":" + sCM;
                    if (mySerialPort != null)
                    {
                        sHexaCommand = "&HE1"; // USB
                    }
                    else if (myWinSock != null)
                    {
                        sHexaCommand = "&HEA"; // ETH
                    }
                    else
                    {
                        return;
                    }
                    break;
            }
            Command = Convert.ToByte(Conversion.Val(sHexaCommand));
            PortAndTool = new byte[] { };
            SendData = new byte[0];
            tempData = System.Text.Encoding.ASCII.GetBytes(sData);
            Array.Resize(ref SendData, (tempData.Length + PortAndTool.Length) - 1 + 1);
            if (tempData.Length > 0)
            {
                Array.Copy(tempData, SendData, tempData.Length);
            }
            if (stackApl != null)
            {
                uint NumMessage;
                MessageHashtable.Message MessageInt = new MessageHashtable.Message();
                MessageInt.Datos = new byte[SendData.Length - 1 + 1];
                Array.Copy(SendData, MessageInt.Datos, SendData.Length);
                MessageInt.Command = Command;
                MessageInt.Device = (byte)numTargetDevice.Value;
                NumMessage = stackApl.Send(MessageInt.Datos, MessageInt.Command, MessageInt.Device);
            }

        }

        public void butClearLog_Click(System.Object sender, System.EventArgs e)
        {
            tbLog.Clear();
        }

        public void cbLogWrap_CheckedChanged(object sender, EventArgs e)
        {
            tbLog.WordWrap = ((CheckBox)sender).Checked;
        }

        public void butClipboard_Click(System.Object sender, System.EventArgs e)
        {
            Clipboard.SetDataObject(tbLog.Text);
        }

        private object LockLogAdd = new object();
        public void LogAdd(string sText)
        {
            lock (LockLogAdd)
            {
                tbLog.AppendText(sText + "\r\n");
                tbLog.SelectionStart = tbLog.Text.Length;
                Application.DoEvents();
            }
        }

        public void cbSerialPorts_DropDown(System.Object sender, System.EventArgs e)
        {
            myLoadSerialPorts();
        }

        public void cbEndPoints_DropDown(System.Object sender, System.EventArgs e)
        {
            myLoadEndPoints();
        }

        public void butLogSend_Click(System.Object sender, System.EventArgs e)
        {
            filesCurrentOperation = FilesOperation.Manual;
            sendCommandManual();
        }

        private void sendCommandManual()
        {
            string sHexaOpCode = "";
            string sHexaOpCodeExtended = "";
            string sTool = "";
            string sPortNbr = "";
            byte[] SendData = null;
            byte[] PortAndTool = null;
            byte[] tempData = null;
            byte OpCode = 0;
            byte OpCodeExtended = (byte)0;

            // port and tool
            //sHexaCommand = lbLogOrders.SelectedValue
            sHexaOpCode = tbOpCode.Text.Trim();
            OpCode = Convert.ToByte(Conversion.Val(sHexaOpCode));
            sHexaOpCodeExtended = tbOpCodeExtended.Text.Trim();
            if (!string.IsNullOrEmpty(sHexaOpCodeExtended))
            {
                OpCodeExtended = Convert.ToByte(Conversion.Val(sHexaOpCodeExtended));
            }
            PortAndTool = new byte[] { };
            if (cbLogPort.Enabled)
            {
                sPortNbr = Strings.Trim(System.Convert.ToString(cbLogPort.SelectedIndex - 1));
                if (cbLogPort.SelectedIndex == 0)
                {
                    MessageBox.Show("Port missing.");
                    return;
                }
                Array.Resize(ref PortAndTool, 1);
                PortAndTool[0] = Convert.ToByte(cbLogPort.SelectedIndex - 1);
                if (cbLogTool.Enabled)
                {
                    Array.Resize(ref PortAndTool, 2);
                    sTool = System.Convert.ToString(cbLogTool.SelectedValue);
                    if (string.IsNullOrEmpty(sTool) || sTool == "0")
                    {
                        MessageBox.Show("Tool missing.");
                        return;
                    }
                    PortAndTool[1] = Convert.ToByte(int.Parse(sTool));
                }
            }

            SendData = new byte[0];

            // build data
            tempData = new byte[0];
            myDataFromFields(ref tempData);

            // join
            if (PortAndTool.Length > 0 | tempData.Length > 0)
            {
                if (!string.IsNullOrEmpty(sHexaOpCodeExtended))
                {
                    Array.Resize(ref SendData, (tempData.Length + PortAndTool.Length + 2) - 1 + 1);
                }
                else
                {
                    Array.Resize(ref SendData, (tempData.Length + PortAndTool.Length) - 1 + 1);
                }
            }

            switch (myCommandProtocol)
            {
                case CStationBase.Protocol.Protocol_01:
                    // port and tool go before data
                    if (PortAndTool.Length > 1)
                    {
                        // port and tool
                        SendData[0] = PortAndTool[0]; // port
                        SendData[1] = PortAndTool[1]; // tool
                        if (tempData.Length > 0)
                        {
                            Array.Copy(tempData, 0, SendData, 2, tempData.Length);
                        }
                    }
                    else if (PortAndTool.Length > 0)
                    {
                        SendData[0] = PortAndTool[0]; // port
                        if (tempData.Length > 0)
                        {
                            Array.Copy(tempData, 0, SendData, 1, tempData.Length);
                        }
                    }
                    else
                    {
                        if (tempData.Length > 0)
                        {
                            Array.Copy(tempData, 0, SendData, 0, tempData.Length);
                        }
                    }
                    break;

                case CStationBase.Protocol.Protocol_02:
                    if (!string.IsNullOrEmpty(sHexaOpCodeExtended))
                    {
                        SendData[0] = OpCode;
                        SendData[1] = OpCodeExtended;
                        if (tempData.Length > 0)
                        {
                            Array.Copy(tempData, 0, SendData, 2, tempData.Length);
                        }
                    }
                    else
                    {
                        if (tempData.Length > 0)
                        {
                            Array.Copy(tempData, SendData, tempData.Length);
                        }
                    }
                    // port and tool go after data
                    if (PortAndTool.Length > 1)
                    {
                        // port and tool
                        SendData[SendData.Length - 1] = PortAndTool[1]; // tool
                        SendData[SendData.Length - 2] = PortAndTool[0]; // port
                    }
                    else if (PortAndTool.Length > 0)
                    {
                        SendData[SendData.Length - 1] = PortAndTool[0]; // port
                    }
                    break;
            }

            if (stackApl != null)
            {
                uint NumMessage;
                MessageHashtable.Message MessageInt = new MessageHashtable.Message();
                MessageInt.Datos = new byte[SendData.Length - 1 + 1];
                Array.Copy(SendData, MessageInt.Datos, SendData.Length);
                if (!string.IsNullOrEmpty(sHexaOpCodeExtended))
                {
                    MessageInt.Command = (byte)(0xFF); // extended
                }
                else
                {
                    MessageInt.Command = OpCode;
                }
                MessageInt.Device = (byte)numTargetDevice.Value;
                for (var i = 1; i <= numSendTimes.Value; i++)
                {
                    NumMessage = stackApl.Send(MessageInt.Datos, MessageInt.Command, MessageInt.Device);
                }
            }
        }

        private void SendMessage(byte[] Datos, byte command, byte targetaddress = 0, bool delayedResponse = false)
        {
            if (targetaddress == 0)
            {
                targetaddress = (byte)numTargetDevice.Value;
            }
            stackApl.Send(Datos, command, targetaddress, delayedResponse);
        }

        public void numTargetDevice_ValueChanged(System.Object sender, System.EventArgs e)
        {
            labTargetDevice.Text = string.Format("H{0:X2}", (int)numTargetDevice.Value);
        }

        public void lbLogOrders_SelectedValueChanged(System.Object sender, System.EventArgs e)
        {
            if (bLoadingCommands)
            {
                return;
            }
            sFindValue3 = System.Convert.ToString(lbLogOrders.SelectedValue);
            cValueText3 itemSelectedOrder = default(cValueText3);
            itemSelectedOrder = listOrders.Find(myFindValue3);
            switch (itemSelectedOrder.Value.Length)
            {
                case 4:
                    // normal
                    tbOpCode.Text = itemSelectedOrder.Value;
                    tbOpCodeExtended.Text = "";
                    break;
                case 6:
                    // extended
                    tbOpCode.Text = itemSelectedOrder.Value.Substring(0, 4);
                    tbOpCodeExtended.Text = itemSelectedOrder.Value.Substring(0, 2) + itemSelectedOrder.Value.Substring(4, 2);
                    break;
            }
            tbAction.Text = itemSelectedOrder.Type;
            tbSendData.Text = itemSelectedOrder.DataSend;
            tbSendDataDefault.Text = itemSelectedOrder.DataSendDefault;
            tbReceiveData.Text = itemSelectedOrder.DataResponse;
            myShowInputFields();

        }

        private bool selectLogOrders(string order)
        {
            cValueText3 itemOrder;
            for (int i = 0; i <= lbLogOrders.Items.Count - 1; i++)
            {
                itemOrder = (cValueText3)lbLogOrders.Items[i];
                if (itemOrder.Value == order)
                {
                    lbLogOrders.SelectedIndex = System.Convert.ToInt32(i);
                    return true;
                }
            }
            return false;

        }

        public void tbSendData_Leave(System.Object sender, System.EventArgs e)
        {
            myShowInputFields();
        }

        private void myShowInputFields()
        {
            Label oLab = null;
            TextBox oField = null;
            string[] aDataTypeSend = null;
            string[] aDataDefaultSend = null;

            if (!tbAction.Text.Contains("p"))
            {
                cbLogPort.Enabled = false;
            }
            else
            {
                cbLogPort.Enabled = true;
            }
            if (!tbAction.Text.Contains("t"))
            {
                cbLogTool.Enabled = false;
            }
            else
            {
                cbLogTool.Enabled = true;
            }

            tbSendData.Text = tbSendData.Text.Trim();
            tbSendDataDefault.Text = tbSendDataDefault.Text.Trim();
            if (tbSendData.Text != "")
            {
                aDataTypeSend = tbSendData.Text.Split('-');
            }
            else
            {
                aDataTypeSend = new string[0];
            }
            if (tbSendDataDefault.Text != "")
            {
                aDataDefaultSend = tbSendDataDefault.Text.Split('-');
            }
            else
            {
                aDataDefaultSend = new string[0];
            }

            for (int i = 0; i <= 19; i++)
            {
                if (i < aDataTypeSend.Length)
                {
                    if (myControlExists(this, "type_" + i.ToString(), ref oLab))
                    {
                        if (aDataTypeSend[i].Substring(0, 1) == "S")
                        {
                            aDataTypeSend[i] = System.Convert.ToString(aDataTypeSend[i].Replace("S", "Ascii "));
                        }
                        else if (aDataTypeSend[i].Substring(0, 1) == "N")
                        {
                            aDataTypeSend[i] = System.Convert.ToString(aDataTypeSend[i].Replace("N", "Number "));
                        }
                        else if (aDataTypeSend[i].Substring(0, 1) == "B")
                        {
                            aDataTypeSend[i] = System.Convert.ToString(aDataTypeSend[i].Replace("B", "Binary "));
                        }
                        else if (aDataTypeSend[i].Substring(0, 1) == "X")
                        {
                            aDataTypeSend[i] = System.Convert.ToString(aDataTypeSend[i].Replace("X", "Hexa "));
                        }
                        else if (aDataTypeSend[i].Substring(0, 1) == "T")
                        {
                            aDataTypeSend[i] = System.Convert.ToString(aDataTypeSend[i].Replace("T", "Temp. "));
                        }
                        oLab.Text = aDataTypeSend[(int)i];
                    }
                    if (myControlExists(this, "field_" + i.ToString(), ref oField))
                    {
                        oField.Text = "";
                        oField.Enabled = true;
                        if (aDataDefaultSend.Length > i)
                        {
                            oField.Text = aDataDefaultSend[i];
                        }
                    }

                }
                else
                {
                    if (myControlExists(this, "type_" + i.ToString(), ref oLab))
                    {
                        oLab.Text = "";
                    }
                    if (myControlExists(this, "field_" + i.ToString(), ref oField))
                    {
                        oField.Text = "";
                        oField.Enabled = false;
                    }
                }
            }

        }

        // others routines
        private bool myControlExists(Form frm, string sControlName, ref Label myControl)
        {
            bool returnValue = false;
            Control[] aControls = frm.Controls.Find(sControlName, true);
            returnValue = false;
            if (aControls.Length > 0)
            {
                myControl = (Label) aControls[0];
                returnValue = true;
            }
            return returnValue;
        }

        private bool myControlExists(Form frm, string sControlName, ref TextBox myControl)
        {
            bool returnValue = false;
            Control[] aControls = frm.Controls.Find(sControlName, true);
            returnValue = false;
            if (aControls.Length > 0)
            {
                myControl = (TextBox) aControls[0];
                returnValue = true;
            }
            return returnValue;
        }

        #region SearchDevices and Persistent Conection USB

        private void StationSearcherUSB_NewConnection(ref CConnectionData connectionData)
        {
            string sLog = string.Format("New connection - Serial port: {0} - PC device assigned: {1} - FrameProtocol: {2} - CommandProtocol: {3} - Station model: {4}", connectionData.pSerialPort.GetSerialPort.PortName, connectionData.PCNumDevice.ToString(), connectionData.FrameProtocol.ToString(), connectionData.CommandProtocol.ToString(), connectionData.StationModel);
            LogAdd(sLog);
            //Debug.Print(sLog)
            mySerialPort = connectionData.pSerialPort.GetSerialPort;
            numTargetDevice.Value = connectionData.StationNumDevice;
            string sCommandProtocol = "";
            switch (cbUseOrders.SelectedIndex)
            {
                case 0:
                    //Automatic
                    myFrameProtocol = connectionData.FrameProtocol;
                    myCommandProtocol = connectionData.CommandProtocol;
                    sCommandProtocol = Strings.Format(System.Convert.ToInt32(connectionData.CommandProtocol), "00");
                    break;
                case 1:
                    myFrameProtocol = connectionData.FrameProtocol;
                    // force command protocol 01
                    myCommandProtocol = CStationBase.Protocol.Protocol_01;
                    sCommandProtocol = "01";
                    break;
                case 2:
                    myFrameProtocol = connectionData.FrameProtocol;
                    // force command protocol 02
                    myCommandProtocol = CStationBase.Protocol.Protocol_02;
                    sCommandProtocol = "02";
                    break;
                default:
                    myFrameProtocol = connectionData.FrameProtocol;
                    myCommandProtocol = connectionData.CommandProtocol;
                    sCommandProtocol = Strings.Format(System.Convert.ToInt32(connectionData.CommandProtocol), "00");
                    break;
            }

            CModelData stationModelData = new CModelData(connectionData.StationModel);
            var sModel = stationModelData.Model;
            string sModelType = stationModelData.ModelType;
            int iModelVersion = stationModelData.ModelVersion;

            CStationsConfiguration confStation = new CStationsConfiguration(sModel);
            int iPorts = confStation.Ports;
            eStationType stationType = confStation.StationType;
            GenericStationTools[] ToolSoportadas = confStation.SupportedTools;
            myLoadtools(ToolSoportadas, stationType);

            features = new CFeaturesData(sModel, sModelType, iModelVersion, sCommandProtocol);
            EnumFrameFlowControl FlowControl = EnumFrameFlowControl.ONE_TO_ONE;
            if (features.BurstMessages)
            {
                FlowControl = EnumFrameFlowControl.BURST;
            }

            labCommandProtocol.Text = "Command protocol: ";
            switch (myCommandProtocol)
            {
                case CStationBase.Protocol.Protocol_01:
                    labCommandProtocol.Text = labCommandProtocol.Text + "01";
                    myLoadOrders_01();
                    stackApl = new CCommunicationChannel(connectionData);
                    stackApl.SentFrame += stackApl_SentFrame;
                    stackApl.ResponseFrame += stackApl_ResponseFrame;
                    stackApl.SentRawData += stackApl_SentRawData;
                    stackApl.ResponseRawData += stackApl_ResponseRawData;
                    stackApl.MessageResponse += stackApl_Response;
                    stackApl.ConnectionError += stackApl_ConnectError;
                    stackApl.Initialize(FlowControl == EnumFrameFlowControl.BURST, connectionData.StationNumDevice, connectionData.FrameProtocol, connectionData.CommandProtocol);
                    //Initialize address communication channel
                    stackApl.AddStack(connectionData.StationNumDevice);
                    break;

                case CStationBase.Protocol.Protocol_02:
                    labCommandProtocol.Text = labCommandProtocol.Text + "02";
                    if (stationType == eStationType.HA)
                    {
                        myLoadOrders_HA_02();
                    }
                    else if (stationType == eStationType.SF)
                    {
                        myLoadOrders_SF_02();
                    }
                    else
                    {
                        myLoadOrders_02();
                    }
                    stackApl = new CCommunicationChannel(connectionData);
                    stackApl.SentFrame += stackApl_SentFrame;
                    stackApl.ResponseFrame += stackApl_ResponseFrame;
                    stackApl.SentRawData += stackApl_SentRawData;
                    stackApl.ResponseRawData += stackApl_ResponseRawData;
                    stackApl.MessageResponse += stackApl_Response;
                    stackApl.ConnectionError += stackApl_ConnectError;
                    stackApl.Initialize(FlowControl == EnumFrameFlowControl.BURST, connectionData.StationNumDevice, connectionData.FrameProtocol, connectionData.CommandProtocol);
                    //Initialize address communication channel
                    stackApl.AddStack(connectionData.StationNumDevice);
                    break;

            }

            eraseSearcherUSB();

            butConnectUSB.Enabled = true;
            butConnectUSB.Text = TDisconnect;
            butConnectEth.Enabled = false;
            butSendCommand.Enabled = true;
            cbUseOrders.Enabled = !butSendCommand.Enabled;
        }

        private void StationSearcherUSB_NoConnection(string sSerialPort)
        {
            LogAdd(string.Format("No connection on serial port: {0}", sSerialPort));
            eraseSearcherUSB();
            butConnectUSB.Enabled = true;
            butConnectUSB.Text = TConnect;
            butConnectEth.Enabled = true;
            butSendCommand.Enabled = false;
            cbUseOrders.Enabled = !butSendCommand.Enabled;

        }

        private void eraseSearcherUSB()
        {
            if (StationSearcherUSB != null)
            {
                StationSearcherUSB.NewConnection -= StationSearcherUSB_NewConnection;
                StationSearcherUSB.NoConnectionOnPort -= StationSearcherUSB_NoConnection;
                StationSearcherUSB.DataSentRawDataSearch -= StationSearcherUSB_DataSentRawDataSearch;
                StationSearcherUSB.DataReceivedRawDataSearch -= StationSearcherUSB_DataReceivedRawDataSearch;
                StationSearcherUSB.Eraser();
                StationSearcherUSB = null;
            }
        }

        private void StationSearcherUSB_DataSentRawDataSearch(ref byte[] RawData)
        {
            string sText = "";
            sText = RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToHexa(RawData, "-");
            LogAdd("--- Sent RAW (Handshake) [" + sText + "]");
        }

        private void StationSearcherUSB_DataReceivedRawDataSearch(ref byte[] RawData)
        {
            string sText = "";
            sText = RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToHexa(RawData, "-");
            LogAdd("--- Received RAW (Handshake)  [" + sText + "]");
        }
        #endregion

        #region SearchDevices and Persistent Conection TCP

        private void StationSearcherTCP_NewConnection(ref CConnectionData connectionData)
        {
            string sLog = string.Format("New connection - WinSock: {0} - PC device assigned: {1} - FrameProtocol: {2} - CommandProtocol: {3} - Station model: {4}", connectionData.pWinSock.HostEndPoint.ToString(), connectionData.PCNumDevice.ToString(), connectionData.FrameProtocol.ToString(), connectionData.CommandProtocol.ToString(), connectionData.StationModel);
            LogAdd(sLog);
            //Debug.Print(sLog)
            myWinSock = connectionData.pWinSock;
            numTargetDevice.Value = connectionData.StationNumDevice;
            string sCommandProtocol = "";
            switch (cbUseOrders.SelectedIndex)
            {
                case 0:
                    //Automatic
                    myFrameProtocol = connectionData.FrameProtocol;
                    myCommandProtocol = connectionData.CommandProtocol;
                    sCommandProtocol = Strings.Format(System.Convert.ToInt32(connectionData.CommandProtocol), "00");
                    break;
                case 1:
                    myFrameProtocol = connectionData.FrameProtocol;
                    // force command protocol 01
                    myCommandProtocol = CStationBase.Protocol.Protocol_01;
                    sCommandProtocol = "01";
                    break;
                case 2:
                    myFrameProtocol = connectionData.FrameProtocol;
                    // force command protocol 02
                    myCommandProtocol = CStationBase.Protocol.Protocol_02;
                    sCommandProtocol = "02";
                    break;
                default:
                    myFrameProtocol = connectionData.FrameProtocol;
                    myCommandProtocol = connectionData.CommandProtocol;
                    sCommandProtocol = Strings.Format(System.Convert.ToInt32(connectionData.CommandProtocol), "00");
                    break;
            }

            string[] ArrayModelData = connectionData.StationModel.Split('_');
            string sModel = ArrayModelData[0].Trim();
            string sModelType = "";
            int iModelVersion = 0;
            if (ArrayModelData.Length > 1)
            {
                sModelType = ArrayModelData[1].Trim();
            }
            if (ArrayModelData.Length > 2)
            {
                if (Information.IsNumeric(ArrayModelData[2]))
                {
                    iModelVersion = int.Parse(ArrayModelData[2]);
                }
            }
            features = new CFeaturesData(sModel, sModelType, iModelVersion, sCommandProtocol);
            EnumFrameFlowControl FlowControl = EnumFrameFlowControl.ONE_TO_ONE;
            if (features.BurstMessages)
            {
                FlowControl = EnumFrameFlowControl.BURST;
            }

            labCommandProtocol.Text = "Command protocol: ";
            switch (myCommandProtocol)
            {
                case CStationBase.Protocol.Protocol_01:
                    labCommandProtocol.Text = labCommandProtocol.Text + "01";
                    myLoadOrders_01();
                    stackApl = new CCommunicationChannel(connectionData);
                    stackApl.SentFrame += stackApl_SentFrame;
                    stackApl.ResponseFrame += stackApl_ResponseFrame;
                    stackApl.SentRawData += stackApl_SentRawData;
                    stackApl.ResponseRawData += stackApl_ResponseRawData;
                    stackApl.MessageResponse += stackApl_Response;
                    stackApl.ConnectionError += stackApl_ConnectError;
                    stackApl.Initialize(FlowControl == EnumFrameFlowControl.BURST, connectionData.StationNumDevice, connectionData.FrameProtocol, connectionData.CommandProtocol);
                    //Initialize address communication channel
                    stackApl.AddStack(connectionData.StationNumDevice);
                    break;
                case CStationBase.Protocol.Protocol_02:
                    labCommandProtocol.Text = labCommandProtocol.Text + "02";
                    myLoadOrders_02();
                    stackApl = new CCommunicationChannel(connectionData);
                    stackApl.SentFrame += stackApl_SentFrame;
                    stackApl.ResponseFrame += stackApl_ResponseFrame;
                    stackApl.SentRawData += stackApl_SentRawData;
                    stackApl.ResponseRawData += stackApl_ResponseRawData;
                    stackApl.MessageResponse += stackApl_Response;
                    stackApl.ConnectionError += stackApl_ConnectError;
                    stackApl.Initialize(FlowControl == EnumFrameFlowControl.BURST, connectionData.StationNumDevice, connectionData.FrameProtocol, connectionData.CommandProtocol);
                    //Initialize address communication channel
                    stackApl.AddStack(connectionData.StationNumDevice);
                    break;
            }

            eraseSearcherTCP();
            //createMySearcherUDP()
            butConnectEth.Enabled = true;
            butConnectEth.Text = TDisconnect;
            butConnectUSB.Enabled = false;
            butSendCommand.Enabled = true;
            cbUseOrders.Enabled = !butSendCommand.Enabled;

            // updater
            updaterUI(enumUpdaterState.initial);
        }

        private void StationSearcherTCP_NoConnection(string sEndPoint)
        {
            LogAdd(string.Format("No connection on address: {0}", sEndPoint));
            eraseSearcherTCP();
            //createMySearcherUDP()
            butConnectEth.Enabled = true;
            butConnectEth.Text = TConnect;
            butConnectUSB.Enabled = true;
            butSendCommand.Enabled = false;
            cbUseOrders.Enabled = !butSendCommand.Enabled;
        }

        private void eraseSearcherTCP()
        {
            if (StationSearcherTCP != null)
            {
                StationSearcherTCP.Dispose();
                StationSearcherTCP = null;
                StationSearcherTCP.NewConnection += StationSearcherTCP_NewConnection;
                StationSearcherTCP.NoConnectionOnEndPoint += StationSearcherTCP_NoConnection;
            }
        }

        #endregion

        #region Stack
        private void stackApl_SentFrame(byte[] Frame)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CCommunicationChannel.SentFrameEventHandler(stackApl_SentFrame), new object[] { Frame });
                return;
            }

            string sFrameEdited = "";
            string sDataEdited = "";
            string sText = "";
            string sText2 = "";
            string sData = "";
            int iCommand = myFrameEdited(Frame, tbSendData.Text.Trim(), ref sFrameEdited, ref sDataEdited);
            if (iCommand == -1 || string.IsNullOrEmpty(sFrameEdited))
            {
                sText = string.Format("Sent frame (hexa) cannot be edited - Data: {0}", sFrameEdited);
            }
            else if (iCommand != (int)EnumCommandFrame_02_SOLD.M_SYN | cbViewSYNReceivingData.Checked)
            {
                sText = string.Format("Sent frame (hexa) - Data: {0}", sFrameEdited);
                if (!string.IsNullOrEmpty(sDataEdited))
                {
                    sText2 = string.Format("Sent data: {0}", sDataEdited);
                }
                else
                {
                    sText2 = "";
                }
            }
            if (!string.IsNullOrEmpty(sText))
            {
                LogAdd(sText);
            }
            if (!string.IsNullOrEmpty(sText2))
            {
                LogAdd(sText2);
            }
        }

        private void stackApl_ResponseFrame(byte[] Frame)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CCommunicationChannel.ResponseFrameEventHandler(stackApl_ResponseFrame), new object[] { Frame });
                return;
            }

            string sFrameEdited = "";
            string sDataEdited = "";
            string sText = "";
            string sText2 = "";
            string sData = "";
            int iCommand = myFrameEdited(Frame, tbReceiveData.Text.Trim(), ref sFrameEdited, ref sDataEdited);
            if (iCommand == -1 || string.IsNullOrEmpty(sFrameEdited))
            {
                sText = string.Format("Received frame (hexa) cannot be edited - Data: {0}", sFrameEdited);
            }
            else if (iCommand != (int)EnumCommandFrame_02_SOLD.M_SYN | cbViewSYNReceivingData.Checked)
            {
                sText = string.Format("Received frame (hexa) - Data: {0}", sFrameEdited);
                if (!string.IsNullOrEmpty(sDataEdited))
                {
                    sText2 = string.Format("Edited data: {0}", sDataEdited);
                }
                else
                {
                    sText2 = "";
                }
            }
            if (!string.IsNullOrEmpty(sText))
            {
                LogAdd(sText);
            }
            if (!string.IsNullOrEmpty(sText2))
            {
                LogAdd(sText2);
            }
            //If sText <> "" Or sText2 <> "" Then LogAdd("")


        }

        private void stackApl_SentRawData(byte[] RawData)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CCommunicationChannel.SentRawDataEventHandler(stackApl_SentRawData), new object[] { RawData });
                return;
            }
            string sText = "";
            if (cbViewRAW.Checked)
            {
                sText = RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToHexa(RawData, "-");
                LogAdd("--- Sent RAW [" + sText + "]");
            }
        }

        private void stackApl_ResponseRawData(byte[] RawData)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CCommunicationChannel.ResponseRawDataEventHandler(stackApl_ResponseRawData), new object[] { RawData });
                return;
            }
            string sText = "";
            if (cbViewRAW.Checked)
            {
                sText = RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToHexa(RawData, "-");
                LogAdd("--- Received RAW [" + sText + "]");
            }
        }

        private void stackApl_Response(uint NumStream, byte[] Datos, byte Command, byte Origen)
        {
            //LogAdd(String.Format("Received data - Num: {0} - Data: {1} - Command: {2}", NumStream.ToString, myBytesToHexa(Datos), Command.ToString))
            // updater y files
            byte[] bytes = null;
            List<byte> listBytes = new List<byte>();

            // updater
            if (Command == (byte)EnumCommandFrame_02_HA.M_CLEARING)
            {
                if (ClearingFlashInProgressEvent != null)
                    ClearingFlashInProgressEvent(Datos);
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_CLEARMEMFLASH)
            {
                if (ClearingFlashFinishedEvent != null)
                    ClearingFlashFinishedEvent(Datos);
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_SENDMEMADDRESS)
            {
                if (AddressMemoryFlashFinishedEvent != null)
                    AddressMemoryFlashFinishedEvent(Datos);
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_SENDMEMDATA)
            {
                if (DataMemoryFlashFinishedEvent != null)
                    DataMemoryFlashFinishedEvent(Datos);
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_ENDPROGR)
            {
                if (EndProgFinishedEvent != null)
                    EndProgFinishedEvent(Datos);

                // files
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_READSTARTFILE)
            {
                if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
                {
                    if (StartReadingFileEvent != null)
                        StartReadingFileEvent(true, BitConverter.ToInt32(Datos, 1));
                }
                else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
                {
                    if (StartReadingFileEvent != null)
                        StartReadingFileEvent(false, 0);
                }
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_READFILEBLOCK)
            {
                bytes = new byte[Datos.Length - 4 - 1 + 1];
                Array.Copy(Datos, 4, bytes, 0, bytes.Length);
                if (BlockReadingFileEvent != null) // sequence, data
                    BlockReadingFileEvent(BitConverter.ToInt32(Datos, 0), bytes);
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_READENDOFFILE)
            {
                if (EndReadingFileEvent != null)
                    EndReadingFileEvent();
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_WRITESTARTFILE)
            {
                if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
                {
                    if (StartWritingFileEvent != null)
                        StartWritingFileEvent(true);
                }
                else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
                {
                    if (StartWritingFileEvent != null)
                        StartWritingFileEvent(false);
                }
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_WRITEFILEBLOCK)
            {
                if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
                {
                    if (BlockWritingFileEvent != null)
                        BlockWritingFileEvent(BitConverter.ToInt32(Datos, 1), true);
                }
                else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
                {
                    if (BlockWritingFileEvent != null)
                        BlockWritingFileEvent(BitConverter.ToInt32(Datos, 1), false);
                }
                else
                {
                    if (BlockWritingFileEvent != null)
                        BlockWritingFileEvent(BitConverter.ToInt32(Datos, 1), false);
                }
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_WRITEENDOFFILE)
            {
                if (EndWritingFileEvent != null)
                    EndWritingFileEvent();
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_R_FILESCOUNT)
            {
                if (FileCountEvent != null)
                    FileCountEvent(BitConverter.ToInt32(Datos, 0));
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_R_GETFILENAME)
            {
                listBytes.Clear();
                foreach (byte byt in Datos)
                {
                    listBytes.Add(byt);
                }
                if (FileNameEvent != null)
                    FileNameEvent(Encoding.ASCII.GetString(listBytes.ToArray()));
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_DELETEFILE)
            {
                if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
                {
                    if (DeletedFileNameEvent != null)
                        DeletedFileNameEvent(true);
                }
                else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
                {
                    if (DeletedFileNameEvent != null)
                        DeletedFileNameEvent(false);
                }
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_R_SELECTEDFILENAME)
            {
                listBytes.Clear();
                foreach (byte byt in Datos)
                {
                    listBytes.Add(byt);
                }
                if (CurrentSelectedFileNameEvent != null)
                    CurrentSelectedFileNameEvent(Encoding.ASCII.GetString(listBytes.ToArray()));
            }
            else if (Command == (byte)EnumCommandFrame_02_HA.M_W_SELECTFILE)
            {
                if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_ACK)
                {
                    if (SelectedFileEvent != null)
                        SelectedFileEvent(true);
                }
                else if (Datos[0] == (byte)EnumCommandFrame_02_HA.M_NACK)
                {
                    if (SelectedFileEvent != null)
                        SelectedFileEvent(false);
                }
            }
        }

        private void stackApl_ConnectError(EnumConnectError TypeError, byte address, byte command)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CCommunicationChannel.ConnectionErrorEventHandler(stackApl_ConnectError), new object[] { TypeError, address, command });
                return;
            }

            //Station connection error. Depending on the error type
            LogAdd(string.Format("Error {0} - Disconnected ", TypeError.ToString()));
            //station comunication lost. Releasing the stack memory
            eraseStack();
            eraseSearcherUSB();
            eraseSearcherTCP();
            butConnectUSB.Enabled = true;
            butConnectUSB.Text = TConnect;
            butConnectEth.Enabled = true;
            butConnectEth.Text = TConnect;
            butSendCommand.Enabled = false;
            cbUseOrders.Enabled = !butSendCommand.Enabled;
            mySerialPort = null;
            myWinSock = null;
            updaterUI(enumUpdaterState.initial);
        }
        #endregion

        private int myFrameEdited(byte[] _array, string sDataTypes, ref string sFrameEdited, ref string sDataEdited)
        {
            int returnValue = 0;
            // return Command. -1 = Command not detected
            string sFrame = "";
            string sHex = "";
            int iDataIdx = -1;
            string[] aDataType = null;
            string sFormat = "";
            int iLen = 0;
            byte[] aData = null;
            CTemperature auxTemp = new CTemperature();

            returnValue = -1;
            sFrameEdited = "";
            sDataEdited = "";
            aData = new byte[0];

            switch (myFrameProtocol)
            {
                case CStationBase.Protocol.Protocol_01:
                    for (var i = 0; i <= _array.Length - 1; i++)
                    {
                        if (((int)i == 0) || (i == (_array.Length - 1)))
                        {
                            // STX, ETX
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                        else if (((int)i == 1) || ((int)i == 2))
                        {
                            // source, target devices
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                        else if ((int)i == 3)
                        {
                            // command
                            returnValue = _array[i];
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                        else if ((int)i == 4)
                        {
                            // data length
                            sHex = _array[i].ToString("X").PadLeft(2, '0') + ":";
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-{";
                            }
                            sFrame += sHex;
                        }
                        else if (i == (_array.Length - 2))
                        {
                            // BCC
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "}-";
                            }
                            sFrame += sHex;
                        }
                        else if (i > 4)
                        {
                            // data
                            iDataIdx++;
                            Array.Resize(ref aData, iDataIdx + 1);
                            aData[iDataIdx] = _array[i];

                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (sFrame.Substring(sFrame.Length - 1, 1) != ":")
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                    }
                    break;

                case CStationBase.Protocol.Protocol_02:
                    for (var i = 0; i <= _array.Length - 1; i++)
                    {
                        if (((int)i == 0) || (i == (_array.Length - 1)))
                        {
                            // STX, ETX
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                        else if (((int)i == 1) || ((int)i == 2))
                        {
                            // source, target devices
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                        else if ((int)i == 3)
                        {
                            // FID
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                        else if ((int)i == 4)
                        {
                            // command
                            returnValue = _array[i];
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                        else if ((int)i == 5)
                        {
                            // data length
                            sHex = _array[i].ToString("X").PadLeft(2, '0') + ":";
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "-{";
                            }
                            sFrame += sHex;
                        }
                        else if (i == (_array.Length - 2))
                        {
                            // BCC
                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (!string.IsNullOrEmpty(sFrame))
                            {
                                sFrame += "}-";
                            }
                            sFrame += sHex;
                        }
                        else if (i > 5)
                        {
                            // data
                            iDataIdx++;
                            Array.Resize(ref aData, iDataIdx + 1);
                            aData[iDataIdx] = _array[i];

                            sHex = System.Convert.ToString(_array[i].ToString("X").PadLeft(2, '0'));
                            if (sFrame.Substring(sFrame.Length - 1, 1) != ":")
                            {
                                sFrame += "-";
                            }
                            sFrame += sHex;
                        }
                    }
                    break;

            }

            // build sDataEdited

            if (aData.Length > 0 && returnValue != -1 && returnValue != (int)EnumCommandFrame_02_SOLD.M_SYN)
            {

                if (sDataTypes != "")
                {
                    aDataType = sDataTypes.Split('-');
                }
                else
                {
                    aDataType = new string[0];
                }

                iDataIdx = 0;

                // extended opcode
                if (tbOpCodeExtended.Text != "")
                {
                    sDataEdited += "Ext:" + aData[0].ToString("X").PadLeft(2, '0') + "-" +
                        aData[1].ToString("X").PadLeft(2, '0');
                    iDataIdx = 2;
                }

                for (var x = 0; x <= aDataType.Length - 1; x++)
                {
                    sFormat = aDataType[(int)x].Substring(0, 1);

                    // if no more data, exit for
                    if (iDataIdx > aData.Length - 1)
                    {
                        break;
                    }

                    switch (sFormat)
                    {
                        case "S":
                            // ascii data
                            if (aDataType[(int)x].Length > 1)
                            {
                                iLen = int.Parse(aDataType[(int)x].Substring(1));
                            }
                            else
                            {
                                iLen = aData.Length;
                            }
                            if (sDataEdited != "")
                            {
                                sDataEdited += "-";
                            }
                            if (iLen > aData.Length - iDataIdx)
                            {
                                iLen = aData.Length - iDataIdx;
                            }
                            sDataEdited = sDataEdited + System.Text.Encoding.UTF8.GetString(aData, iDataIdx, iLen);
                            iDataIdx += iLen;
                            break;
                        case "X":
                            // show as hexa bytes
                            if (aDataType[(int)x].Length > 1)
                            {
                                iLen = int.Parse(aDataType[(int)x].Substring(1));
                            }
                            else
                            {
                                iLen = aData.Length;
                            }
                            for (var i = iDataIdx; i <= iDataIdx + iLen - 1; i++)
                            {
                                if (sDataEdited != "")
                                {
                                    sDataEdited += "-";
                                }
                                sDataEdited += System.Convert.ToString(aData[(int)i].ToString("X").PadLeft(2, '0'));
                            }
                            iDataIdx += iLen;
                            break;

                        case "T":
                            // temperature
                            iLen = int.Parse(aDataType[(int)x].Substring(1));
                            if (sDataEdited != "")
                            {
                                sDataEdited += "-";
                            }
                            switch (iLen)
                            {
                                case 1:
                                    auxTemp.UTI = aData[iDataIdx];
                                    break;
                                case 2:
                                    auxTemp.UTI = BitConverter.ToUInt16(aData, iDataIdx);
                                    break;
                                case 4:
                                    auxTemp.UTI = BitConverter.ToInt32(aData, iDataIdx);
                                    break;
                            }
                            switch (cbTempType.SelectedIndex)
                            {
                                case 0:
                                    //UTI
                                    sDataEdited += auxTemp.UTI.ToString();
                                    break;
                                case 1:
                                    //Celsius
                                    switch (auxTemp.UTI)
                                    {
                                        case 0:
                                        case 0xFF:
                                        case 0xFFFF:
                                            sDataEdited += auxTemp.UTI.ToString();
                                            break;
                                        default:
                                            sDataEdited += auxTemp.ToCelsius().ToString();
                                            break;
                                    }
                                    break;
                                case 2:
                                    //Fahrenheit
                                    switch (auxTemp.UTI)
                                    {
                                        case 0:
                                        case 0xFF:
                                        case 0xFFFF:
                                            sDataEdited += auxTemp.UTI.ToString();
                                            break;
                                        default:
                                            sDataEdited += auxTemp.ToFahrenheit().ToString();
                                            break;
                                    }
                                    break;
                            }
                            iDataIdx += iLen;
                            break;

                        case "N":
                            // number
                            iLen = int.Parse(aDataType[(int)x].Substring(1));
                            if (sDataEdited != "")
                            {
                                sDataEdited += "-";
                            }
                            switch (iLen)
                            {
                                case 1:
                                    sDataEdited += aData[iDataIdx].ToString();
                                    break;
                                case 2:
                                    sDataEdited += BitConverter.ToUInt16(aData, iDataIdx).ToString();
                                    break;
                                case 4:
                                    sDataEdited += BitConverter.ToUInt32(aData, iDataIdx).ToString();
                                    break;
                            }
                            iDataIdx += iLen;
                            break;

                        case "B":
                            // binary
                            iLen = int.Parse(aDataType[(int)x].Substring(1));
                            for (var j = 0; j <= iLen - 1; j++)
                            {
                                if (sDataEdited != "")
                                {
                                    sDataEdited += "-";
                                }
                                sDataEdited += System.Convert.ToString(Convert.ToString(aData[iDataIdx + j], 2).PadLeft(8, '0'));
                            }
                            iDataIdx += iLen;
                            break;

                    }

                }
            }

            sFrameEdited = sFrame;
            return returnValue;
        }

        private int myDataFromFields(ref byte[] Data)
        {
            int returnValue = 0;
            // returns -1 if error

            Label oLab = null;
            TextBox oField = null;
            string[] aDataTypeSend = null;
            string sFormat = "";
            string sFieldData = "";
            int iLen = 0;
            string sLen = "";
            int iDataIdx = -1;
            byte[] tempData = null;
            CTemperature auxTemp = new CTemperature();
            string tempString = "";

            returnValue = 0;
            Data = new byte[0];

            tbSendData.Text = tbSendData.Text.Trim();
            if (tbSendData.Text != "")
            {
                aDataTypeSend = tbSendData.Text.Split('-');
            }
            else
            {
                aDataTypeSend = new string[0];
            }

            iDataIdx = 0;
            for (var x = 0; x <= aDataTypeSend.Length - 1; x++)
            {
                sFormat = aDataTypeSend[(int)x].Substring(0, 1);

                sFieldData = "";
                if (myControlExists(this, "field_" + x.ToString(), ref oField))
                {
                    sFieldData = oField.Text;
                }

                // check missing data. ask to continue (for optional extended data)
                if (string.IsNullOrEmpty(sFieldData))
                {
                    if (!showMissingFieldMsg(System.Convert.ToInt32(x)))
                    {
                        return -1;
                    }
                }

                switch (sFormat)
                {
                    case "S":
                        // input as ascii data
                        if (!string.IsNullOrEmpty(sFieldData))
                        {
                            if (aDataTypeSend[(int)x].Length > 1)
                            {
                                iLen = int.Parse(aDataTypeSend[(int)x].Substring(1));
                            }
                            else
                            {
                                iLen = sFieldData.Length;
                            }
                            tempData = System.Text.Encoding.UTF8.GetBytes(sFieldData.Substring(0, iLen));
                            Array.Resize(ref Data, Data.Length + tempData.Length - 1 + 1);
                            Array.Copy(tempData, 0, Data, iDataIdx, tempData.Length);
                            iDataIdx += iLen;
                        }
                        break;

                    case "X":
                        // input as hexa
                        if (!string.IsNullOrEmpty(sFieldData))
                        {
                            if (aDataTypeSend[(int)x].Length > 1)
                            {
                                iLen = int.Parse(aDataTypeSend[(int)x].Substring(1));
                            }
                            else
                            {
                                iLen = System.Convert.ToInt32((double)sFieldData.Length / 2);
                            }
                            Array.Resize(ref Data, Data.Length + iLen - 1 + 1);
                            sFieldData = sFieldData.Replace("-", "");
                            tempData = RoutinesLibrary.Data.DataType.StringUtils.HexaToByteArray(sFieldData).ToArray();
                            Array.Copy(tempData, 0, Data, iDataIdx, iLen);
                            iDataIdx += iLen;
                        }
                        break;

                    case "T":
                        // input as temperature
                        if (!string.IsNullOrEmpty(sFieldData))
                        {
                            iLen = int.Parse(aDataTypeSend[(int)x].Substring(1));
                            Array.Resize(ref Data, Data.Length + iLen - 1 + 1);
                            switch (cbTempType.SelectedIndex)
                            {
                                case 0:
                                    //input as UTI
                                    auxTemp.UTI = int.Parse(sFieldData);
                                    break;
                                case 1:
                                    //input as Celsius
                                    switch (int.Parse(sFieldData))
                                    {
                                        case 0: //, &HFF, &HFFFF
                                            auxTemp.UTI = int.Parse(sFieldData);
                                            break;
                                        default:
                                            auxTemp.InCelsius(int.Parse(sFieldData));
                                            break;
                                    }
                                    break;
                                case 2:
                                    //input as Fahrenheit
                                    switch (int.Parse(sFieldData))
                                    {
                                        case 0: //, &HFF, &HFFFF
                                            auxTemp.UTI = int.Parse(sFieldData);
                                            break;
                                        default:
                                            auxTemp.InFahrenheit(int.Parse(sFieldData));
                                            break;
                                    }
                                    break;
                            }
                            switch (iLen)
                            {
                                case 1:
                                    tempData = BitConverter.GetBytes(auxTemp.UTI);
                                    Array.Resize(ref tempData, iLen - 1 + 1);
                                    Array.Copy(tempData, 0, Data, iDataIdx, iLen);
                                    break;
                                case 2:
                                    tempData = BitConverter.GetBytes(auxTemp.UTI);
                                    Array.Resize(ref tempData, iLen - 1 + 1);
                                    Array.Copy(tempData, 0, Data, iDataIdx, iLen);
                                    break;
                                case 4:
                                    tempData = BitConverter.GetBytes(auxTemp.UTI);
                                    Array.Resize(ref tempData, iLen - 1 + 1);
                                    Array.Copy(tempData, 0, Data, iDataIdx, iLen);
                                    break;
                            }
                            iDataIdx += iLen;
                        }
                        break;

                    case "N":
                        // number
                        if (!string.IsNullOrEmpty(sFieldData))
                        {
                            iLen = int.Parse(aDataTypeSend[(int)x].Substring(1));
                            Array.Resize(ref Data, Data.Length + iLen - 1 + 1);
                            switch (iLen)
                            {
                                case 1:
                                    tempData = BitConverter.GetBytes(int.Parse(sFieldData));
                                    Array.Resize(ref tempData, iLen - 1 + 1);
                                    Array.Copy(tempData, 0, Data, iDataIdx, iLen);
                                    break;
                                case 2:
                                    tempData = BitConverter.GetBytes(int.Parse(sFieldData));
                                    Array.Resize(ref tempData, iLen - 1 + 1);
                                    Array.Copy(tempData, 0, Data, iDataIdx, iLen);
                                    break;
                                case 4:
                                    tempData = BitConverter.GetBytes(int.Parse(sFieldData));
                                    Array.Resize(ref tempData, iLen - 1 + 1);
                                    Array.Copy(tempData, 0, Data, iDataIdx, iLen);
                                    break;
                            }
                            iDataIdx += iLen;
                        }
                        break;

                    case "B":
                        // binary
                        if (!string.IsNullOrEmpty(sFieldData))
                        {
                            iLen = int.Parse(aDataTypeSend[(int)x].Substring(1));
                            Array.Resize(ref Data, Data.Length + iLen - 1 + 1);
                            for (var j = 0; j <= iLen - 1; j++)
                            {
                                //For j = iDataIdx To iDataIdx + iLen - 1
                                tempString = sFieldData.PadLeft(8, '0').Substring((j * 8) + 1 - 1, 8);
                                tempData = BitConverter.GetBytes(Convert.ToInt32(tempString, 2));
                                Array.Resize(ref tempData, 1);
                                Array.Copy(tempData, 0, Data, iDataIdx + j, 1);
                            }
                            iDataIdx += iLen;
                        }
                        break;

                }

            }

            return returnValue;
        }

        public bool showMissingFieldMsg(int iFieldNbr)
        {
            // return Continue
            return Interaction.MsgBox("Field " + System.Convert.ToString(iFieldNbr) + " is blank. Data will not be sent. Continue anyway?", MsgBoxStyle.YesNo, "Field data is missing") == MsgBoxResult.Yes;
        }

        public void myLoadSerialPorts()
        {
            System.Collections.ObjectModel.ReadOnlyCollection<string> PortsList = default(System.Collections.ObjectModel.ReadOnlyCollection<string>);
            string sText = "";
            string sSelected = "";
            int iSelected = -1;
            if (cbSerialPorts.SelectedIndex >= 0)
            {
                sSelected = System.Convert.ToString(cbSerialPorts.Items[cbSerialPorts.SelectedIndex]);
            }
            cbSerialPorts.Items.Clear();
            cbSerialPorts.Items.Add("Search");
            PortsList = (new Microsoft.VisualBasic.Devices.Computer()).Ports.SerialPortNames;
            foreach (string sport in PortsList)
            {
                cbSerialPorts.Items.Add(sport);
            }
            if (!string.IsNullOrEmpty(sSelected))
            {
                iSelected = cbSerialPorts.Items.IndexOf(sSelected);
            }
            if (iSelected < 0)
            {
                cbSerialPorts.SelectedIndex = 0;
            }
            else
            {
                cbSerialPorts.SelectedIndex = iSelected;
            }
        }

        public void myLoadEndPoints()
        {

            myRefreshStationListUPD();

            string sText = "";
            string sSelected = "";
            int iSelected = -1;
            if (cbEndPoints.SelectedIndex >= 0)
            {
                sSelected = System.Convert.ToString(cbEndPoints.Items[cbEndPoints.SelectedIndex]);
            }
            cbEndPoints.Items.Clear();
            cbEndPoints.Items.Add("Search");
            foreach (CStationInfoTCP ep in discoveredStations.GetTable)
            {
                cbEndPoints.Items.Add(ep.StationData.IPEndPointValue.ToString());
            }
            if (!string.IsNullOrEmpty(sSelected))
            {
                iSelected = cbEndPoints.Items.IndexOf(sSelected);
            }
            if (iSelected < 0)
            {
                cbEndPoints.SelectedIndex = 0;
            }
            else
            {
                cbEndPoints.SelectedIndex = iSelected;
            }
        }

        private void myRefreshStationListUPD()
        {
            IPEndPoint NumEndPoint = default(IPEndPoint);

            // set all stations not discovered
            for (int i = 0; i <= discoveredStations.GetTable.Count - 1; i++)
            {
                discoveredStations.set_Discovered(i, false);
            }

            // add or set discovered stations
            CStationConnectionData[] UDPStations = searchStationsUPD.GetDiscoveredStations();
            int idx = 0;
            foreach (CStationConnectionData stnData in UDPStations)
            {
                NumEndPoint = stnData.IPEndPointValue;
                idx = discoveredStations.ExistsStation(NumEndPoint);
                if (idx >= 0)
                {
                    discoveredStations.set_Discovered(idx, true);
                }
                else
                {
                    discoveredStations.AddStation(NumEndPoint, stnData);
                }
            }

            // delete stations not in discovered table
            for (int i = discoveredStations.GetTable.Count - 1; i >= 0; i--)
            {
                if (discoveredStations.get_Discovered(i) == false)
                {
                    discoveredStations.RemoveStation(i);
                }
            }

        }

        private void myLoadOrders_01()
        {
            // S: String
            // Bx: x Bytes a binario
            // Nx: x Bytes a numérico

            bLoadingCommands = true;
            lbLogOrders.DataSource = null;

            listOrders.Clear();
            listOrders.Add(new cValueText3("&H00", "Null", "W", "", "X1"));
            listOrders.Add(new cValueText3("&H04", "End of transmision", "W", "", ""));
            listOrders.Add(new cValueText3("&H06", "Positive Acknowlegde", "W", "", "X1"));
            listOrders.Add(new cValueText3("&H15", "Negative Acknowlegde", "W", "", ""));
            listOrders.Add(new cValueText3("&H16", "Syncronize", "W", "", "X1"));
            listOrders.Add(new cValueText3("&H1E", "Read Connection Status", "R", "", "S1"));
            listOrders.Add(new cValueText3("&H1F", "Write Connection Status", "W", "S1", ""));
            listOrders.Add(new cValueText3("&H20", "Execute Bootloader", "W", "", "X1"));
            listOrders.Add(new cValueText3("&H21", "Protocol:StationModel:Soft:Hard", "R", "", "S"));
            listOrders.Add(new cValueText3("&H30", "Tool: Read Port Info", "Rp", "", "N1-N1-T2-T2-N2-N2-B1-B1"));
            listOrders.Add(new cValueText3("&H31", "Tool: Read Fixed Temp", "Rpt", "", "T2"));
            listOrders.Add(new cValueText3("&H32", "Tool: Write Fixed Temp", "Wpt", "T2", ""));
            listOrders.Add(new cValueText3("&H33", "Tool: Read Temp Levels", "Rpt", "", "N1"));
            listOrders.Add(new cValueText3("&H34", "Tool: Write Temp Levels", "Wpt", "N1", ""));
            listOrders.Add(new cValueText3("&H35", "Tool: Read Temp Level 1", "Wpt", "", "T2"));
            listOrders.Add(new cValueText3("&H36", "Tool: Write Temp Level 1", "Wpt", "T2", ""));
            listOrders.Add(new cValueText3("&H37", "Tool: Read Temp Level 2", "Wpt", "", "T2"));
            listOrders.Add(new cValueText3("&H38", "Tool: Write Temp Level 2", "Wpt", "T2", ""));
            listOrders.Add(new cValueText3("&H39", "Tool: Read Temp Level 3", "Wpt", "", "T2"));
            listOrders.Add(new cValueText3("&H3A", "Tool: Write Temp Level 3", "Wpt", "T2", ""));

            listOrders.Add(new cValueText3("&H40", "Tool: Read Sleep Delay", "Rpt", "", "N1"));
            listOrders.Add(new cValueText3("&H41", "Tool: Write Sleep Delay", "Wpt", "N1", "X1"));
            listOrders.Add(new cValueText3("&H42", "Tool: Read Sleep Temp", "Rpt", "", "T2"));
            listOrders.Add(new cValueText3("&H43", "Tool: Write Sleep Temp", "Wpt", "T2", "X1"));
            listOrders.Add(new cValueText3("&H44", "Tool: Read Hiber Delay", "Rpt", "", "N1"));
            listOrders.Add(new cValueText3("&H45", "Tool: Write Hiber Delay", "Wpt", "N1", "X1"));
            listOrders.Add(new cValueText3("&H46", "Tool: Read Adjust Temp", "Rpt", "", "T2"));
            listOrders.Add(new cValueText3("&H47", "Tool: Write Adjust Temp", "Wpt", "T2", "X1"));

            listOrders.Add(new cValueText3("&H50", "Tool: Read Selected Temp", "Rp", "", "T2"));
            listOrders.Add(new cValueText3("&H51", "Tool: Write Selected Temp", "Wp", "T2", ""));
            listOrders.Add(new cValueText3("&H52", "Tool: Read tip temp (A:B)", "Rp", "", "T2-T2"));
            listOrders.Add(new cValueText3("&H53", "Tool: Read cartridge current (A:B) (ERROR)", "Rp", "", "N2-N2"));
            listOrders.Add(new cValueText3("&H54", "Tool: Read power (A:B)", "Rp", "", "N2-N2"));
            listOrders.Add(new cValueText3("&H55", "Tool: Read connected tool", "Rp", "", "N1"));
            listOrders.Add(new cValueText3("&H56", "Tool: Read tool error", "Rp", "", "N1"));
            listOrders.Add(new cValueText3("&H57", "Tool: Read tool status (sleep, extractor, etc)", "Rp", "", "B1"));

            listOrders.Add(new cValueText3("&H58", "Read MOS Temp", "Rp", "", "T2"));
            listOrders.Add(new cValueText3("&H59", "Tool: Read Delay Time sleep-extractor (ERROR)", "Rp", "", "N2-S1"));
            listOrders.Add(new cValueText3("&H60", "Read Remote Mode", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H61", "Write Remote Mode", "W", "N1", "X1"));
            listOrders.Add(new cValueText3("&H62", "Read Status Remote Mode", "R", "", "B1"));
            listOrders.Add(new cValueText3("&H63", "Write Status Remote Mode", "W", "B1", "X1"));

            listOrders.Add(new cValueText3("&H80", "Continuos Mode: Read", "R", "", "N1-B1"));
            listOrders.Add(new cValueText3("&H81", "Continuos Mode: Write", "W", "N1-B1", "X1"));
            listOrders.Add(new cValueText3("&H82", "Continuos Mode: Receive Info", "", "", ""));

            listOrders.Add(new cValueText3("&HA0", "Station: Read Temp Units", "R", "", "S1"));
            listOrders.Add(new cValueText3("&HA1", "Station: Write Temp Units", "W", "S1", "X1"));
            listOrders.Add(new cValueText3("&HA2", "Station: Read Max Temp", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HA3", "Station: Write Max Temp", "W", "T2", ""));
            listOrders.Add(new cValueText3("&HA4", "Station: Read Min Temp", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HA5", "Station: Write Min Temp", "W", "T2", ""));
            listOrders.Add(new cValueText3("&HA6", "Station: Read Nitrogen Mode", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HA7", "Station: Write Nitrogen Mode", "W", "N1", ""));
            listOrders.Add(new cValueText3("&HA8", "Station: Read Help Text", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HA9", "Station: Write Help Text", "W", "N1", ""));
            listOrders.Add(new cValueText3("&HAA", "Station: Read power limit", "R", "", "N2"));
            listOrders.Add(new cValueText3("&HAB", "Station: Write power limit", "W", "N2", ""));
            listOrders.Add(new cValueText3("&HAC", "Station: Read PIN", "R", "", "S4"));
            listOrders.Add(new cValueText3("&HAD", "Station: Write PIN", "W", "S4", ""));
            listOrders.Add(new cValueText3("&HAE", "Station: Read error", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HAF", "Station: Read TRAFO Temp", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HB0", "Station: Reset to factory settings", "W", "", "X1"));
            listOrders.Add(new cValueText3("&HB1", "Station: Read Station Name", "R", "", "S"));
            listOrders.Add(new cValueText3("&HB2", "Station: Write Station Name", "W", "S", "X1"));
            listOrders.Add(new cValueText3("&HB3", "Station: Read Beep", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HB4", "Station: Write Beep", "W", "N1", "X1"));
            listOrders.Add(new cValueText3("&HB5", "Station: Read Language", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HB6", "Station: Write Language", "W", "N1", "X1"));
            listOrders.Add(new cValueText3("&HB7", "Station: Read Temp Error TRAFO", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HB8", "Station: Read Temp Error MOS", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HB9", "Station: Read Device ID", "R", "", "X"));
            listOrders.Add(new cValueText3("&HBA", "Station: Write Device ID (max20)", "W", "X", "X1"));
            // 08/06/2015 Se añaden 2 tramas HBB y HBC - Partameters Locked
            listOrders.Add(new cValueText3("&HBB", "Station: Read Partameters Locked", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HBC", "Station: Write Partameters Locked", "W", "N1", "X1"));

            listOrders.Add(new cValueText3("&HC0", "Glob.Counters: Read Plug Time min.", "R", "", "N4-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HC1", "Glob.Counters: Write Plug Time (ERROR)", "W", "N4-N4-N4-N4", ""));
            listOrders.Add(new cValueText3("&HC2", "Glob.Counters: Read Work Time", "R", "", "N4-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HC3", "Glob.Counters: Write Work Time (ERROR)", "W", "N4-N4-N4-N4", ""));
            listOrders.Add(new cValueText3("&HC4", "Glob.Counters: Read Sleep Time", "R", "", "N4-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HC5", "Glob.Counters: Write Sleep Time (ERROR)", "W", "N4-N4-N4-N4", ""));
            listOrders.Add(new cValueText3("&HC6", "Glob.Counters: Read Hiber Time", "R", "", "N4-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HC7", "Glob.Counters: Write Hiber Time (ERROR)", "W", "N4-N4-N4-N4", ""));
            listOrders.Add(new cValueText3("&HC8", "Glob.Counters: Read No Tool Time", "R", "", "N4-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HC9", "Glob.Counters: Write No Tool Time (ERROR)", "W", "N4-N4-N4-N4", ""));
            listOrders.Add(new cValueText3("&HCA", "Glob.Counters: Read Sleep Cycles", "R", "", "N4-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HCB", "Glob.Counters: Write Sleep Cycles (ERROR)", "W", "N4-N4-N4-N4", ""));
            listOrders.Add(new cValueText3("&HCC", "Glob.Counters: Read Desol Cycles", "R", "", "N4-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HCD", "Glob.Counters: Write Desol Cycles (ERROR)", "W", "N4-N4-N4-N4", ""));

            lbLogOrders.ValueMember = "Value";
            lbLogOrders.DisplayMember = "Text";
            lbLogOrders.DataSource = listOrders;

            lbLogOrders.Refresh();

            bLoadingCommands = false;

        }

        private void myLoadOrders_02()
        {
            // S:  string variable
            // Sx: string fijo de x bytes
            // Bx: binario en x bytes
            // Nx: numérico en x bytes
            // Tx: temperatura en x bytes
            // Xx: hexa en x bytes

            bLoadingCommands = true;
            lbLogOrders.DataSource = null;

            listOrders.Clear();
            listOrders.Add(new cValueText3("&H00", "Handshake", "W", "", ""));
            listOrders.Add(new cValueText3("&H04", "End of transmision", "W", "", ""));
            listOrders.Add(new cValueText3("&H06", "Positive Acknowlegde", "W", "", ""));
            listOrders.Add(new cValueText3("&H15", "Negative Acknowlegde", "W", "", ""));
            listOrders.Add(new cValueText3("&H16", "Syncronize", "W", "", ""));
            listOrders.Add(new cValueText3("&H17", "Read StandBy", "R", "", "X"));
            listOrders.Add(new cValueText3("&H18", "Write StandBy", "W", "X", ""));
            listOrders.Add(new cValueText3("&H1C", "Read original UID (MCU digital sign)", "R", "", "S"));
            listOrders.Add(new cValueText3("&H1D", "Discover", "W", "", ""));
            listOrders.Add(new cValueText3("&H1E", "Read UID", "R", "", "X"));
            listOrders.Add(new cValueText3("&H1F", "Write UID (max32)", "W", "X", ""));

            listOrders.Add(new cValueText3("&H20", "Reset Station -> Execute Bootloader", "W", "", ""));
            listOrders.Add(new cValueText3("&H21", "Protocol:StationModel:Soft:Hard", "R", "", "S"));

            listOrders.Add(new cValueText3("&H22", "Boot: Clear Flash Memory (Response several H28 + 1 H22)", "W", "S", ""));
            listOrders.Add(new cValueText3("&H23", "Boot: Send Memory Address ", "W", "N1-X4", "X1-N1"));
            listOrders.Add(new cValueText3("&H24", "Boot: Send Memory Data", "W", "N1-X128", "X1-N1"));
            listOrders.Add(new cValueText3("&H25", "Boot: End Program ", "W", "", ""));
            listOrders.Add(new cValueText3("&H26", "Boot: End Update/Bootloader", "W", "", ""));
            listOrders.Add(new cValueText3("&H27", "Boot: Wait (Continue update)", "W", "", ""));
            listOrders.Add(new cValueText3("&H28", "Boot: Clearing in progress (pages-erased page)", "R", "", "N1-N1")); // response to H22 pages-cleared page
            listOrders.Add(new cValueText3("&H29", "Boot: Force Update", "W", "", ""));

            listOrders.Add(new cValueText3("&H30", "Tool: Read Port Status", "Rp", "", "N1-N1-T2-T2-N2-N2-B1-N1"));
            listOrders.Add(new cValueText3("&H31", "Tool: Reset Configuration Port/Tool (NEW)", "Wpt", "", ""));

            listOrders.Add(new cValueText3("&H33", "Tool: Read Temp Levels", "Rpt", "", "N1-N1-N1-T2-N1-T2-N1-T2-N1-N1"));
            listOrders.Add(new cValueText3("&H34", "Tool: Write Temp Levels", "Wpt", "N1-N1-N1-T2-N1-T2-N1-T2", ""));

            listOrders.Add(new cValueText3("&H40", "Tool: Read Sleep Delay", "Rpt", "", "N1-N1-N1-N1"));
            listOrders.Add(new cValueText3("&H41", "Tool: Write Sleep Delay", "Wpt", "N1-N1", ""));
            listOrders.Add(new cValueText3("&H42", "Tool: Read Sleep Temp", "Rpt", "", "T2-N1-N1"));
            listOrders.Add(new cValueText3("&H43", "Tool: Write Sleep Temp", "Wpt", "T2", ""));
            listOrders.Add(new cValueText3("&H44", "Tool: Read Hiber Delay", "Rpt", "", "N1-N1-N1-N1"));
            listOrders.Add(new cValueText3("&H45", "Tool: Write Hiber Delay", "Wpt", "N1-N1", ""));
            listOrders.Add(new cValueText3("&H46", "Tool: Read Adjust Temp", "Rpt", "", "T2-N1-N1"));
            listOrders.Add(new cValueText3("&H47", "Tool: Write Adjust Temp", "Wpt", "T2", ""));

            listOrders.Add(new cValueText3("&H48", "Tool: Read Cartridge", "Rpt", "", "N1-N2-N2-N2-N1-N1-N1-N1"));
            listOrders.Add(new cValueText3("&H49", "Tool: Write Cartridge", "Wpt", "N1-N2-N2-N2-N1-N1", ""));

            listOrders.Add(new cValueText3("&H50", "Tool: Read Selected Temp", "Rp", "", "T2-N1"));
            listOrders.Add(new cValueText3("&H51", "Tool: Write Selected Temp", "Wp", "T2", ""));
            listOrders.Add(new cValueText3("&H52", "Tool: Read tip temp (A:B)", "Rp", "", "T2-T2-N1"));
            listOrders.Add(new cValueText3("&H53", "Tool: Read cartridge current (A:B)", "Rp", "", "N2-N2-N1"));
            listOrders.Add(new cValueText3("&H54", "Tool: Read power (A:B)", "Rp", "", "N2-N2-N1"));
            listOrders.Add(new cValueText3("&H55", "Tool: Read connected tool", "Rp", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H56", "Tool: Read tool error", "Rp", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H57", "Tool: Read tool status (sleep, extractor, etc)", "Rp", "", "B1-N1"));
            //listOrders.Add(New cValueText3("&H58", "Tool: Write tool status (remote mode only) ", "Wp", "B1", ""))
            listOrders.Add(new cValueText3("&H59", "Read MOS Temp", "Rp", "", "T2-N1"));
            listOrders.Add(new cValueText3("&H5A", "Tool: Read Delay Time sleep-extractor", "Rp", "", "N2-S1-N1"));
            listOrders.Add(new cValueText3("&H5B", "Tool: Light/Sound Warning (F/R-timeon-timeoff-repetitions)", "Wp", "S1-N2-N2-N2", ""));
            listOrders.Add(new cValueText3("&H60", "Read Remote Mode", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H61", "Write Remote Mode", "W", "N1", ""));
            listOrders.Add(new cValueText3("&H62", "Read Tool Status (Remote Mode)", "R", "", "B1"));
            listOrders.Add(new cValueText3("&H63", "Write Tool Status (Remote Mode) Bit 0:Stand 3=Extractor 4:Desold", "W", "B1", ""));

            listOrders.Add(new cValueText3("&H80", "Continuos Mode: Read", "R", "", "N1-B1"));
            listOrders.Add(new cValueText3("&H81", "Continuos Mode: Write (Last = extended mode)", "W", "N1-B1-N1", ""));
            listOrders.Add(new cValueText3("&H82", "Continuos Mode: Receive Info (Last 3 = extended mode)", "", "", "N1-T1-T2-N1-N1-B1-X1-N1-N1-N2"));

            listOrders.Add(new cValueText3("&H83", "Read Max Temp Alarm + Delay", "Rp", "", "T2-N2-N1"));
            listOrders.Add(new cValueText3("&H84", "Write Max Temp Alarm + Delay", "Wp", "T2-N2", ""));
            listOrders.Add(new cValueText3("&H85", "Read Min Temp Alarm + Delay", "Rp", "", "T2-N2-N1"));
            listOrders.Add(new cValueText3("&H86", "Write Min Temp Alarm + Delay", "Wp", "T2-N2", ""));
            listOrders.Add(new cValueText3("&H87", "Read Temp Alarm", "Rp", "", "N1"));

            listOrders.Add(new cValueText3("&H90", "File: Start", "W", "S", ""));
            listOrders.Add(new cValueText3("&H91", "File: Content Block (max 254)", "W", "N1-S", "X1-N1"));
            listOrders.Add(new cValueText3("&H92", "File: End", "W", "", ""));
            listOrders.Add(new cValueText3("&H93", "File: Get List by Extension (Response several H94 + 1 H93)", "W", "S", ""));
            listOrders.Add(new cValueText3("&H94", "File: Info List by Extension", "R", "", "S"));
            listOrders.Add(new cValueText3("&H95", "File: Ask File", "W", "", ""));

            listOrders.Add(new cValueText3("&HA0", "Station: Read Temp Units", "R", "", "S1"));
            listOrders.Add(new cValueText3("&HA1", "Station: Write Temp Units", "W", "S1", "X1"));
            listOrders.Add(new cValueText3("&HA2", "Station: Read Max Temp", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HA3", "Station: Write Max Temp", "W", "T2", ""));
            listOrders.Add(new cValueText3("&HA4", "Station: Read Min Temp", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HA5", "Station: Write Min Temp", "W", "T2", ""));
            listOrders.Add(new cValueText3("&HA6", "Station: Read Station Lock", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HA7", "Station: Write Station Lock", "W", "N1", ""));

            listOrders.Add(new cValueText3("&HAA", "Station: Read power limit", "R", "", "N2"));
            listOrders.Add(new cValueText3("&HAB", "Station: Write power limit", "W", "N2", ""));
            listOrders.Add(new cValueText3("&HAC", "Station: Read PIN", "R", "", "S4"));
            listOrders.Add(new cValueText3("&HAD", "Station: Write PIN", "W", "S4", ""));
            listOrders.Add(new cValueText3("&HAE", "Station: Read error", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HAF", "Station: Read TRAFO Temp", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HB0", "Station: Reset to factory settings (CHANGED)", "W", "N1", "")); // N1 extended mode
            listOrders.Add(new cValueText3("&HB1", "Station: Read Station Name", "R", "", "S"));
            listOrders.Add(new cValueText3("&HB2", "Station: Write Station Name", "W", "S", ""));
            listOrders.Add(new cValueText3("&HB3", "Station: Read Sound Status", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HB4", "Station: Write Sound Status", "W", "N1", ""));
            listOrders.Add(new cValueText3("&HB5", "Station: Read Language", "R", "", "S2"));
            listOrders.Add(new cValueText3("&HB6", "Station: Write Language", "W", "S2", ""));
            listOrders.Add(new cValueText3("&HB7", "Station: Read Temp Error TRAFO", "R", "", "T2"));
            listOrders.Add(new cValueText3("&HB8", "Station: Read Temp Error MOS", "R", "", "T2"));

            listOrders.Add(new cValueText3("&HBB", "Station: Read Date and Time", "R", "", "N2-N1-N1-N1-N1-N1"));
            listOrders.Add(new cValueText3("&HBC", "Station: Write Date and Time", "W", "N2-N1-N1-N1-N1-N1", ""));
            listOrders.Add(new cValueText3("&HBD", "Station: Light/Sound Warning (D/S-timeon-timeoff-repetitions)", "W", "S1-N2-N2-N2", ""));
            listOrders.Add(new cValueText3("&HBE", "Station: Read Display Theme", "R", "", "S"));
            listOrders.Add(new cValueText3("&HBF", "Station: Write Display Theme", "W", "S", ""));

            listOrders.Add(new cValueText3("&HC0", "Glob.Counters: Read Plug Time min.", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC1", "Glob.Counters: Write Plug Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC2", "Glob.Counters: Read Work Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC3", "Glob.Counters: Write Work Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC4", "Glob.Counters: Read Sleep Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC5", "Glob.Counters: Write Sleep Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC6", "Glob.Counters: Read Hiber Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC7", "Glob.Counters: Write Hiber Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC8", "Glob.Counters: Read No Tool Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC9", "Glob.Counters: Write No Tool Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HCA", "Glob.Counters: Read Sleep Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HCB", "Glob.Counters: Write Sleep Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HCC", "Glob.Counters: Read Desol Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HCD", "Glob.Counters: Write Desol Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HCE", "Glob.Counters: Reset (NEW)", "Wp", "", ""));

            listOrders.Add(new cValueText3("&HD0", "Part.Counters: Read Plug Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD1", "Part.Counters: Write Plug Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD2", "Part.Counters: Read Work Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD3", "Part.Counters: Write Work Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD4", "Part.Counters: Read Sleep Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD5", "Part.Counters: Write Sleep Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD6", "Part.Counters: Read Hiber Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD7", "Part.Counters: Write Hiber Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD8", "Part.Counters: Read No Tool Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD9", "Part.Counters: Write No Tool Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HDA", "Part.Counters: Read Sleep Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HDB", "Part.Counters: Write Sleep Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HDC", "Part.Counters: Read Desol Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HDD", "Part.Counters: Write Desol Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HDE", "Part.Counters: Reset (NEW)", "Wp", "", ""));

            listOrders.Add(new cValueText3("&HE0", "USB: Read Connection Status. PCname:c", "R", "", "S"));
            listOrders.Add(new cValueText3("&HE1", "USB: Write Connection Status. PCname:c", "W", "S", "", "libtest:c"));

            listOrders.Add(new cValueText3("&HE3", "FRONTAL: Read Connection Status. PCname:c", "R", "", "S"));
            listOrders.Add(new cValueText3("&HE4", "FRONTAL: Write Connection Status. PCname:c", "W", "S", "", "libtest:c"));

            listOrders.Add(new cValueText3("&HE7", "ETH: Read Configuration", "R", "", "N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N2"));
            listOrders.Add(new cValueText3("&HE8", "ETH: Write Configuration", "W", "N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N2", ""));
            listOrders.Add(new cValueText3("&HE9", "ETH: Read Connection Status. IP:PCname:c", "R", "", "N1-N1-N1-N1-S"));
            listOrders.Add(new cValueText3("&HEA", "ETH: Write Connection Status. PCname:c", "W", "S", "", "libtest:c"));

            listOrders.Add(new cValueText3("&HF0", "ROBOT: Read Connection Configuration", "R", "", "N1-S1-S1-S1-S1-S2"));
            listOrders.Add(new cValueText3("&HF1", "ROBOT: Write Connection Configuration", "W", "N1-S1-S1-S1-S1-S2", ""));
            listOrders.Add(new cValueText3("&HF2", "ROBOT: Read Connection Status", "R", "", "S1"));
            listOrders.Add(new cValueText3("&HF3", "ROBOT: Write Connection Status", "W", "S1", ""));

            listOrders.Add(new cValueText3("&HF9", "PERIPH: Read Number of Peripherals", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HFA", "PERIPH: Read Peripheral Configuration", "R", "N1", "S30-N1"));
            listOrders.Add(new cValueText3("&HFB", "PERIPH: Write Peripheral Configuration", "W", "S30-N1", ""));
            listOrders.Add(new cValueText3("&HFC", "PERIPH: Read Peripheral Status", "R", "N1", "N1-S1"));
            listOrders.Add(new cValueText3("&HFD", "PERIPH: Write Peripheral Status", "W", "", ""));

            // extended opcodes
            listOrders.Add(new cValueText3("&HC000", "EXT Glob.Counters: Read All Counters", "Rp", "", "N1-N4-N4-N4-N4-N4-N4-N4-N4-N1"));
            listOrders.Add(new cValueText3("&HC001", "EXT Glob.Counters: Write All Counters", "Rp", "N1-N4-N4-N4-N4-N4-N4-N4-N4-N1", ""));
            listOrders.Add(new cValueText3("&HC002", "EXT Glob.Counters: Read Plug Time min.", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC003", "EXT Glob.Counters: Write Plug Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC004", "EXT Glob.Counters: Read Work Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC005", "EXT Glob.Counters: Write Work Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC006", "EXT Glob.Counters: Read Sleep Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC007", "EXT Glob.Counters: Write Sleep Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC008", "EXT Glob.Counters: Read Hiber Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC009", "EXT Glob.Counters: Write Hiber Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC00A", "EXT Glob.Counters: Read No Tool Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC00B", "EXT Glob.Counters: Write No Tool Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC00C", "EXT Glob.Counters: Read Sleep Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC00D", "EXT Glob.Counters: Write Sleep Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC00E", "EXT Glob.Counters: Read Desol Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC00F", "EXT Glob.Counters: Write Desol Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC010", "EXT Glob.Counters: Read Sold Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC011", "EXT Glob.Counters: Write Sold Cycles", "Wp", "N4", ""));

            listOrders.Add(new cValueText3("&HD000", "EXT Part.Counters: Read All Counters", "Rp", "", "N1-N4-N4-N4-N4-N4-N4-N4-N4-N1"));
            listOrders.Add(new cValueText3("&HD001", "EXT Part.Counters: Write All Counters", "Rp", "N1-N4-N4-N4-N4-N4-N4-N4-N4-N1", ""));
            listOrders.Add(new cValueText3("&HD002", "EXT Part.Counters: Read Plug Time min.", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD003", "EXT Part.Counters: Write Plug Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD004", "EXT Part.Counters: Read Work Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD005", "EXT Part.Counters: Write Work Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD006", "EXT Part.Counters: Read Sleep Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD007", "EXT Part.Counters: Write Sleep Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD008", "EXT Part.Counters: Read Hiber Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD009", "EXT Part.Counters: Write Hiber Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD00A", "EXT Part.Counters: Read No Tool Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD00B", "EXT Part.Counters: Write No Tool Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD00C", "EXT Part.Counters: Read Sleep Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD00D", "EXT Part.Counters: Write Sleep Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD00E", "EXT Part.Counters: Read Desol Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD00F", "EXT Part.Counters: Write Desol Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD010", "EXT Part.Counters: Read Sold Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD011", "EXT Part.Counters: Write Sold Cycles", "Wp", "N4", ""));

            listOrders.Add(new cValueText3("&H0130", "EXT Sold.Lab: Read Configuration", "Rp", "", ""));
            listOrders.Add(new cValueText3("&H0131", "EXT Sold.Lab: Read Soldering End", "Rp", "", "S1-S1"));

            listOrders.Add(new cValueText3("&H0250", "EXT Trace: Read Barcode Reader Status", "R", "", "N1-N1-N2"));
            listOrders.Add(new cValueText3("&H0251", "EXT Trace: Clear Reader Input Field", "W", "", ""));
            listOrders.Add(new cValueText3("&H0252", "EXT Trace: Read Reader Data (response several H0253 + 1 H0252)", "R", "", ""));
            listOrders.Add(new cValueText3("&H0253", "EXT Trace: Get Reader Data", "R", "", "N1-X"));
            listOrders.Add(new cValueText3("&H0254", "EXT Trace: Read Key Pressed", "R", "", "S"));

            lbLogOrders.ValueMember = "Value";
            lbLogOrders.DisplayMember = "Text";
            lbLogOrders.DataSource = listOrders;

            lbLogOrders.Refresh();

            bLoadingCommands = false;

        }

        private void myLoadOrders_HA_02()
        {
            // S:  string variable
            // Sx: string fijo de x bytes
            // Bx: binario en x bytes
            // Nx: numérico en x bytes
            // Tx: temperatura en x bytes
            // Xx: hexa en x bytes

            bLoadingCommands = true;
            lbLogOrders.DataSource = null;

            listOrders.Clear();
            listOrders.Add(new cValueText3("&H00", "Handshake", "W", "", ""));
            listOrders.Add(new cValueText3("&H04", "End of transmision", "W", "", ""));
            listOrders.Add(new cValueText3("&H06", "Positive Acknowlegde", "W", "", ""));
            listOrders.Add(new cValueText3("&H15", "Negative Acknowlegde", "W", "", ""));
            listOrders.Add(new cValueText3("&H16", "Syncronize", "W", "", ""));
            listOrders.Add(new cValueText3("&H1C", "Read original UID (MCU digital sign)", "R", "", "S"));
            listOrders.Add(new cValueText3("&H1D", "Discover", "W", "", ""));
            listOrders.Add(new cValueText3("&H1E", "Read UID", "R", "", "X"));
            listOrders.Add(new cValueText3("&H1F", "Write UID (max32)", "W", "X", ""));

            listOrders.Add(new cValueText3("&H20", "Reset Station -> Execute Bootloader", "W", "", ""));
            listOrders.Add(new cValueText3("&H21", "Protocol:StationModel:Soft:Hard", "R", "", "S"));

            listOrders.Add(new cValueText3("&H22", "Boot: Clear Flash Memory (Response several H28 + 1 H22)", "W", "S", ""));
            listOrders.Add(new cValueText3("&H23", "Boot: Send Memory Address ", "W", "N1-X4", "X1-N1"));
            listOrders.Add(new cValueText3("&H24", "Boot: Send Memory Data", "W", "N1-X128", "X1-N1"));
            listOrders.Add(new cValueText3("&H25", "Boot: End Program ", "W", "", ""));
            listOrders.Add(new cValueText3("&H26", "Boot: End Update/Bootloader", "W", "", ""));
            listOrders.Add(new cValueText3("&H27", "Boot: Wait (Continue update)", "W", "", ""));
            listOrders.Add(new cValueText3("&H28", "Boot: Clearing in progress (pages-erased page)", "R", "", "N1-N1")); // response to H22 pages-cleared page
            listOrders.Add(new cValueText3("&H29", "Boot: Force Update", "W", "", ""));

            listOrders.Add(new cValueText3("&H30", "Port: Read Port Status", "Rp", "", "N1-N1-T2-T2-N2-N2-N2-B1-N1"));
            listOrders.Add(new cValueText3("&H30", "Port: Read Port Status con TimeToStop (N/A)", "Rp", "", "N1-N1-T2-T2-N2-N2-N2-B1-N1"));
            listOrders.Add(new cValueText3("&H31", "Tool: Reset Configuration Port/Tool (N/A)", "Wpt", "", ""));

            listOrders.Add(new cValueText3("&H33", "Port: Read Work Mode", "Rp", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H34", "Port: Write Work Mode", "Wp", "N1", ""));
            listOrders.Add(new cValueText3("&H35", "Port: Read Heater Status", "Rp", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H36", "Port: Write Heater Status", "Wp", "N1", ""));
            listOrders.Add(new cValueText3("&H37", "Port: Read Suction Status", "Rp", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H38", "Port: Write Suction Status", "Wp", "N1", ""));

            listOrders.Add(new cValueText3("&H40", "Tool: Read Temp/Flow Levels (N/A)", "Rpt", "", "N1-N1-N1-T2-N2-T2-N1-T2-N2-T2-N1-T2-N2-T2-N1-N1"));
            listOrders.Add(new cValueText3("&H41", "Tool: Write Temp/Flow Levels (N/A)", "Wpt", "N1-N1-N1-T2-N2-T2-N1-T2-N2-T2-N1-T2-N2-T2", ""));
            listOrders.Add(new cValueText3("&H42", "Tool: Read Adjust Temp", "Rpt", "", "T2-N1-N1"));
            listOrders.Add(new cValueText3("&H43", "Tool: Write Adjust Temp", "Wpt", "T2", ""));
            listOrders.Add(new cValueText3("&H44", "Tool: Read Time To Stop (seconds)", "Rpt", "", "N2-N1-N1"));
            listOrders.Add(new cValueText3("&H45", "Tool: Write Time To Stop (seconds)", "Wpt", "N2", ""));
            listOrders.Add(new cValueText3("&H46", "Tool: Read Start Mode (N/A)", "Rpt", "", "B1-N1-N1"));
            listOrders.Add(new cValueText3("&H47", "Tool: Write Start Mode (N/A)", "Wpt", "B1", ""));

            listOrders.Add(new cValueText3("&H50", "Port: Read Selected Temp", "Rp", "", "T2-N1"));
            listOrders.Add(new cValueText3("&H51", "Port: Write Selected Temp", "Wp", "T2", ""));
            listOrders.Add(new cValueText3("&H52", "Port: Read Air Temp", "Rp", "", "T2-N1"));
            //listOrders.Add(New cValueText3("&H53", "Port: Read Heater Intensity", "Rp", "", "N2-N1"))
            listOrders.Add(new cValueText3("&H54", "Port: Read Power", "Rp", "", "N2-N1"));
            listOrders.Add(new cValueText3("&H55", "Port: Read Connected Tool", "Rp", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H56", "Port: Read Tool Error", "Rp", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H57", "Port: Read tool status (heater, heater solicitado ,cooling, suction, etc)", "Rp", "", "B1-N1"));

            listOrders.Add(new cValueText3("&H59", "Port: Read Selected Flow", "Rp", "", "N2-N1"));
            listOrders.Add(new cValueText3("&H5A", "Port: Write Selected Flow", "Wp", "N2", ""));
            listOrders.Add(new cValueText3("&H5B", "Port: Read Selected External Temp.", "Rp", "", "T2-N1"));
            listOrders.Add(new cValueText3("&H5C", "Port: Write Selected External Temp.", "Wp", "T2", ""));
            listOrders.Add(new cValueText3("&H5D", "Port: Read Current Flow", "Rp", "", "N2-N1"));
            //listOrders.Add(New cValueText3("&H5E", "Port: Read Current Protect TC Temp.", "Rp", "", "T2-N1"))
            listOrders.Add(new cValueText3("&H5F", "Port: Read Current External TC Temp.", "Rp", "", "T2-N1"));

            listOrders.Add(new cValueText3("&H60", "Read Remote Mode", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H61", "Write Remote Mode", "W", "N1", ""));

            listOrders.Add(new cValueText3("&H80", "Continuos Mode: Read", "R", "", "N1-B1"));
            listOrders.Add(new cValueText3("&H81", "Continuos Mode: Write", "W", "N1-B1", ""));
            listOrders.Add(new cValueText3("&H82", "Continuos Mode: Receive Info", "", "", "N1-T2-N2-N2-T2-T2-N2-B1-B1"));

            listOrders.Add(new cValueText3("&H90", "File: Start Read File", "R", "S12", "X1-N4"));
            listOrders.Add(new cValueText3("&H91", "File: Read Content Block (128 bytes)", "R", "N4", "N4-X128"));
            listOrders.Add(new cValueText3("&H92", "File: End Read File", "W", "", "X1"));
            listOrders.Add(new cValueText3("&H93", "File: Start Write File", "R", "S12-N4", "X1"));
            listOrders.Add(new cValueText3("&H94", "File: Write Content Block (128 bytes)", "R", "N4-X128", "X1-N4"));
            listOrders.Add(new cValueText3("&H95", "File: End Write File", "W", "", "X1"));
            listOrders.Add(new cValueText3("&H96", "File: Get File Count by Extension", "R", "S3", "N4"));
            listOrders.Add(new cValueText3("&H97", "File: Get File Name", "R", "N4", "S12"));
            listOrders.Add(new cValueText3("&H98", "File: Delete File", "W", "S12", "X1"));
            listOrders.Add(new cValueText3("&H9A", "File: Get Selected File", "R", "", "S12"));
            listOrders.Add(new cValueText3("&H9B", "File: Select File", "W", "S12", "X1"));

            listOrders.Add(new cValueText3("&HA0", "Station: Read Temp Units", "R", "", "S1"));
            listOrders.Add(new cValueText3("&HA1", "Station: Write Temp Units", "W", "S1", ""));
            listOrders.Add(new cValueText3("&HA2", "Station: Read Select Temp Limits", "R", "", "T2-T2"));
            listOrders.Add(new cValueText3("&HA3", "Station: Write Select Temp Limits", "W", "T2-T2", ""));
            listOrders.Add(new cValueText3("&HA4", "Station: Read Flow Limits", "R", "", "N2-N2"));
            listOrders.Add(new cValueText3("&HA5", "Station: Write Flow Limits", "W", "N2-N2", ""));
            listOrders.Add(new cValueText3("&HA6", "Station: Read Select External TC Temp. Limits", "R", "", "T2-T2"));
            listOrders.Add(new cValueText3("&HA7", "Station: Write Select External TC Temp. Limits", "W", "T2-T2", ""));
            listOrders.Add(new cValueText3("&HA8", "Station: Read PIN Enabled", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HA9", "Station: Write PIN Enabled", "W", "N1", ""));
            listOrders.Add(new cValueText3("&HAA", "Station: Read Station Locked", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HAB", "Station: Write Station Locked", "W", "N1", ""));
            listOrders.Add(new cValueText3("&HAC", "Station: Read PIN", "R", "", "S4"));
            listOrders.Add(new cValueText3("&HAD", "Station: Write PIN", "W", "S4", ""));
            listOrders.Add(new cValueText3("&HAE", "Station: Read Station Error", "R", "", "N1"));

            listOrders.Add(new cValueText3("&HB0", "Station: Reset to factory settings", "W", "", ""));
            listOrders.Add(new cValueText3("&HB1", "Station: Read Station Name", "R", "", "S"));
            listOrders.Add(new cValueText3("&HB2", "Station: Write Station Name", "W", "S", ""));
            listOrders.Add(new cValueText3("&HB3", "Station: Read Sound Status", "R", "", "N1"));
            listOrders.Add(new cValueText3("&HB4", "Station: Write Sound Status", "W", "N1", ""));
            listOrders.Add(new cValueText3("&HB5", "Station: Read Language (N/A)", "R", "", "S2"));
            listOrders.Add(new cValueText3("&HB6", "Station: Write Language (N/A)", "W", "S2", ""));
            listOrders.Add(new cValueText3("&HBB", "Station: Read Date and Time", "R", "", "N2-N1-N1-N1-N1-N1"));
            listOrders.Add(new cValueText3("&HBC", "Station: Write Date and Time", "W", "N2-N1-N1-N1-N1-N1", ""));

            //listOrders.Add(New cValueText3("&HA6", "Station: Read Time To Stop", "R", "", "N2"))
            //listOrders.Add(New cValueText3("&HA7", "Station: Write Time To Stop", "W", "N2", ""))
            //listOrders.Add(New cValueText3("&HA8", "Station: Read Temp To Stop Cooling", "R", "", "T2"))
            //listOrders.Add(New cValueText3("&HA9", "Station: Write Temp To Stop Cooling", "W", "T2", ""))
            //listOrders.Add(New cValueText3("&HAA", "Station: Read Flow Deviation Limit", "R", "", "N1"))
            //listOrders.Add(New cValueText3("&HAB", "Station: Write Flow Deviation Limit", "W", "N1", ""))
            //listOrders.Add(New cValueText3("&HAC", "Station: Read Protect. TC Temp. Limit", "R", "", "T2"))
            //listOrders.Add(New cValueText3("&HAD", "Station: Write Protect. TC Temp. Limit", "W", "T2", ""))
            //listOrders.Add(New cValueText3("&HAE", "Station: Read Control TC Temp Limit", "R", "", "T2"))
            //listOrders.Add(New cValueText3("&HAF", "Station: Write Control TC Temp Limit", "W", "T2", ""))
            //listOrders.Add(New cValueText3("&HB0", "Station: Read Max. Current to Error", "R", "", "N2")) ' miliamperios
            //listOrders.Add(New cValueText3("&HB1", "Station: Write Max. Current to Error", "W", "N2", "")) ' miliamperios
            //listOrders.Add(New cValueText3("&HB2", "Station: Read Heater Resistance Limits", "R", "", "N2-N2")) ' miliomnios
            //listOrders.Add(New cValueText3("&HB3", "Station: Write Heater Resistance Limits", "W", "N2-N2", "")) ' miliomnios

            listOrders.Add(new cValueText3("&HC0", "Glob.Counters: Read Plug Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC1", "Glob.Counters: Write Plug Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC2", "Glob.Counters: Read Work Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC3", "Glob.Counters: Write Work Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC4", "Glob.Counters: Read Work Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC5", "Glob.Counters: Write Work Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HC6", "Glob.Counters: Read Suction Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HC7", "Glob.Counters: Write Suction Cycles", "Wp", "N4", ""));

            listOrders.Add(new cValueText3("&HD0", "Part.Counters: Read Plug Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD1", "Part.Counters: Write Plug Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD2", "Part.Counters: Read Work Time", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD3", "Part.Counters: Write Work Time", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD4", "Part.Counters: Read Work Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD5", "Part.Counters: Write Work Cycles", "Wp", "N4", ""));
            listOrders.Add(new cValueText3("&HD6", "Part.Counters: Read Suction Cycles", "Rp", "", "N4-N1"));
            listOrders.Add(new cValueText3("&HD7", "Part.Counters: Write Suction Cycles", "Wp", "N4", ""));

            listOrders.Add(new cValueText3("&HE0", "USB: Read Connection Status. PCname:c", "R", "", "S"));
            listOrders.Add(new cValueText3("&HE1", "USB: Write Connection Status. PCname:c", "W", "S", "", "libtest:c"));

            listOrders.Add(new cValueText3("&HE3", "FRONTAL: Read Connection Status. PCname:c", "R", "", "S"));
            listOrders.Add(new cValueText3("&HE4", "FRONTAL: Write Connection Status. PCname:c", "W", "S", "", "libtest:c"));

            listOrders.Add(new cValueText3("&HF0", "ROBOT: Read Connection Configuration", "R", "", "N1-S1-S1-S1-S1-S2"));
            listOrders.Add(new cValueText3("&HF1", "ROBOT: Write Connection Configuration", "W", "N1-S1-S1-S1-S1-S2", ""));
            listOrders.Add(new cValueText3("&HF2", "ROBOT: Read Connection Status", "R", "", "S1"));
            listOrders.Add(new cValueText3("&HF3", "ROBOT: Write Connection Status", "W", "S1", ""));

            lbLogOrders.ValueMember = "Value";
            lbLogOrders.DisplayMember = "Text";
            lbLogOrders.DataSource = listOrders;

            lbLogOrders.Refresh();

            bLoadingCommands = false;

        }

        private void myLoadOrders_SF_02()
        {
            // S:  string variable
            // Sx: string fijo de x bytes
            // Bx: binario en x bytes
            // Nx: numérico en x bytes
            // Tx: temperatura en x bytes
            // Xx: hexa en x bytes

            bLoadingCommands = true;
            lbLogOrders.DataSource = null;

            listOrders.Clear();
            listOrders.Add(new cValueText3("&H00", "Handshake", "W", "", ""));
            listOrders.Add(new cValueText3("&H04", "End of transmision", "W", "", ""));
            listOrders.Add(new cValueText3("&H06", "Positive Acknowlegde", "W", "", ""));
            listOrders.Add(new cValueText3("&H15", "Negative Acknowlegde", "W", "", ""));
            listOrders.Add(new cValueText3("&H16", "Syncronize", "W", "", ""));
            listOrders.Add(new cValueText3("&H1C", "Read original UID (MCU digital sign)", "R", "", "S"));
            listOrders.Add(new cValueText3("&H1D", "Discover", "W", "", ""));
            listOrders.Add(new cValueText3("&H1E", "Read UID", "R", "", "X"));
            listOrders.Add(new cValueText3("&H1F", "Write UID (max32)", "W", "X", ""));

            listOrders.Add(new cValueText3("&H20", "Reset Station -> Execute Bootloader", "W", "", ""));
            listOrders.Add(new cValueText3("&H21", "Protocol:StationModel:Soft:Hard", "R", "", "S"));

            listOrders.Add(new cValueText3("&H22", "Boot: Clear Flash Memory (Response several H28 + 1 H22)", "W", "S", ""));
            listOrders.Add(new cValueText3("&H23", "Boot: Send Memory Address ", "W", "N1-X4", "X1-N1"));
            listOrders.Add(new cValueText3("&H24", "Boot: Send Memory Data", "W", "N1-X128", "X1-N1"));
            listOrders.Add(new cValueText3("&H25", "Boot: End Program ", "W", "", ""));
            listOrders.Add(new cValueText3("&H26", "Boot: End Update/Bootloader", "W", "", ""));
            listOrders.Add(new cValueText3("&H27", "Boot: Wait (Continue update)", "W", "", ""));
            listOrders.Add(new cValueText3("&H28", "Boot: Clearing in progress (pages-erased page)", "R", "", "N1-N1")); // response to H22 pages-cleared page
            listOrders.Add(new cValueText3("&H29", "Boot: Force Update", "W", "", ""));

            listOrders.Add(new cValueText3("&H30", "Tool: Read Mode", "R", "", "N1-N1"));
            listOrders.Add(new cValueText3("&H31", "Tool: Write Mode", "W", "N1-N1", ""));
            listOrders.Add(new cValueText3("&H32", "Tool: Read Program", "R", "N1", "N1-S8-N2-N2-N2-N2-N2-N2"));
            listOrders.Add(new cValueText3("&H33", "Tool: Write Program", "W", "N1-S8-N2-N2-N2-N2-N2-N2", ""));
            listOrders.Add(new cValueText3("&H34", "Tool: Read Program List", "R", "", "N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1"));
            listOrders.Add(new cValueText3("&H35", "Tool: Write Program List", "W", "N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1-N1", ""));
            listOrders.Add(new cValueText3("&H36", "Tool: Read Speed", "R", "", "N2"));
            listOrders.Add(new cValueText3("&H37", "Tool: Write Speed", "W", "N2", ""));
            listOrders.Add(new cValueText3("&H38", "Tool: Read Length", "R", "", "N2"));
            listOrders.Add(new cValueText3("&H39", "Tool: Write Length", "W", "N2", ""));

            listOrders.Add(new cValueText3("&H3A", "Tool: Start Feeding", "W", "N1", ""));
            listOrders.Add(new cValueText3("&H3B", "Tool: Stop Feeding", "W", "", ""));
            listOrders.Add(new cValueText3("&H3C", "Tool: Read Feeding", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H3D", "Tool: Read Backward Mode", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H3E", "Tool: Write Backward Mode", "W", "N1", ""));

            listOrders.Add(new cValueText3("&H50", "Station: Reset to factory settings", "W", "", ""));
            listOrders.Add(new cValueText3("&H51", "Station: Read PIN", "R", "", "S4"));
            listOrders.Add(new cValueText3("&H52", "Station: Write PIN", "W", "S4", ""));
            listOrders.Add(new cValueText3("&H53", "Station: Read Station Locked", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H54", "Station: Write Station Locked", "W", "N1", ""));
            listOrders.Add(new cValueText3("&H55", "Station: Read Beep Status", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H56", "Station: Write Beep Status", "W", "N1", ""));
            listOrders.Add(new cValueText3("&H57", "Station: Read Units", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H58", "Station: Write Units", "W", "N1", ""));

            listOrders.Add(new cValueText3("&H59", "Station: Read Error Code", "R", "", "N1"));
            listOrders.Add(new cValueText3("&H5A", "Station: Reset Error Code", "W", "N1", ""));

            listOrders.Add(new cValueText3("&HC0", "Glob.Counters: Read", "R", "", "N8-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HC1", "Glob.Counters: Reset", "W", "", ""));

            listOrders.Add(new cValueText3("&HC2", "Part.Counters: Read", "R", "", "N8-N4-N4-N4"));
            listOrders.Add(new cValueText3("&HC3", "Part.Counters: Reset", "W", "", ""));

            listOrders.Add(new cValueText3("&HE0", "USB: Read Connection Status. PCname:c", "R", "", "S"));
            listOrders.Add(new cValueText3("&HE1", "USB: Write Connection Status. PCname:c", "W", "S", "", "libtest:c"));

            listOrders.Add(new cValueText3("&HF0", "ROBOT: Read Connection Configuration", "R", "", "N1-S1-S1-S1-S1-S2"));
            listOrders.Add(new cValueText3("&HF1", "ROBOT: Write Connection Configuration", "W", "N1-S1-S1-S1-S1-S2", ""));
            listOrders.Add(new cValueText3("&HF2", "ROBOT: Read Connection Status", "R", "", "S1"));
            listOrders.Add(new cValueText3("&HF3", "ROBOT: Write Connection Status", "W", "S1", ""));

            lbLogOrders.ValueMember = "Value";
            lbLogOrders.DisplayMember = "Text";
            lbLogOrders.DataSource = listOrders;

            lbLogOrders.Refresh();

            bLoadingCommands = false;

        }

        private void myLoadtools()
        {
            listTools.Clear();
            for (int i = 0; i <= 8; i++)
            {
                string sTool = ((GenericStationTools)i).ToString();
                listTools.Add(new cValueText2(i.ToString(), sTool));
            }
            cbLogTool.ValueMember = "Value";
            cbLogTool.DisplayMember = "Text";
            cbLogTool.DataSource = listTools;
        }

        private void myLoadtools(GenericStationTools[] supportedTools, eStationType StationType)
        {
            listTools.Clear();
            cbLogTool.DataSource = null;
            string sTool = "";
            for (var i = 0; i <= supportedTools.Length - 1; i++)
            {
                sTool = supportedTools[i].ToString();
                listTools.Add(new cValueText2((GetInternalToolFromGeneric(supportedTools[i], StationType)).ToString(), sTool));
            }
            sTool = ((GenericStationTools)0).ToString();
            listTools.Insert(0, new cValueText2(0.ToString(), sTool));
            cbLogTool.ValueMember = "Value";
            cbLogTool.DisplayMember = "Text";
            cbLogTool.DataSource = listTools;
        }

        private byte GetInternalToolFromGeneric(GenericStationTools Tool, eStationType StationType)
        {
            switch (StationType)
            {
                case eStationType.HA:
                    // desolder
                    if (Tool != GenericStationTools.NO_TOOL)
                    {
                        return System.Convert.ToByte(Tool - 30);
                    }
                    break;
            }
            return System.Convert.ToByte(Tool);
        }

        private string sFindValue3;
        private bool myFindValue3(cValueText3 item)
        {
            if (item.Value == sFindValue3)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string sFindValue2;
        private bool myFindValue2(cValueText2 item)
        {
            if (item.Value == sFindValue2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public class cValueText3
        {
            private string myValue;
            private string myName;
            private string myType;
            private string myDataSend;
            private string myDataResponse;
            private string myDataSendDefault;
            private string myText;

            public cValueText3(string strValue, string strName, string strType, string strDataSend, string strDataResponse, string strDataSendDefault = "")
            {
                this.myValue = strValue;
                this.myName = strName;
                this.myType = strType;
                this.myDataSend = strDataSend;
                this.myDataResponse = strDataResponse;
                this.myDataSendDefault = strDataSendDefault;
                this.myText = strValue + " - " + strName + " - [" + strType + "] Send:" + strDataSend + " Response:" + strDataResponse + "";
            } //New

            public string Value
            {
                get
                {
                    return myValue;
                }
            }

            public string Name
            {
                get
                {
                    return myName;
                }
            }

            public string Type
            {
                get
                {
                    return myType;
                }
            }

            public string DataSend
            {
                get
                {
                    return myDataSend;
                }
            }

            public string DataSendDefault
            {
                get
                {
                    return myDataSendDefault;
                }
            }

            public string DataResponse
            {
                get
                {
                    return myDataResponse;
                }
            }

            public string Text
            {
                get
                {
                    return myText;
                }
            }

        } //cValueText3

        public class cValueText2
        {
            private string myValue;
            private string myName;
            private string myText;

            public cValueText2(string strValue, string strName)
            {
                this.myValue = strValue;
                this.myName = strName;
                this.myText = strName + " (" + strValue + ")";
            } //New

            public string Value
            {
                get
                {
                    return myValue;
                }
            }

            public string Name
            {
                get
                {
                    return myName;
                }
            }

            public string Text
            {
                get
                {
                    return myText;
                }
            }

        } //cValueText2

        #region Updater

        //Bootloader events
        public delegate void ClearingFlashInProgressEventHandler(byte[] Datos);
        private ClearingFlashInProgressEventHandler ClearingFlashInProgressEvent;

        public event ClearingFlashInProgressEventHandler ClearingFlashInProgress
        {
            add
            {
                ClearingFlashInProgressEvent = (ClearingFlashInProgressEventHandler)System.Delegate.Combine(ClearingFlashInProgressEvent, value);
            }
            remove
            {
                ClearingFlashInProgressEvent = (ClearingFlashInProgressEventHandler)System.Delegate.Remove(ClearingFlashInProgressEvent, value);
            }
        }

        public delegate void ClearingFlashFinishedEventHandler(byte[] Datos);
        private ClearingFlashFinishedEventHandler ClearingFlashFinishedEvent;

        public event ClearingFlashFinishedEventHandler ClearingFlashFinished
        {
            add
            {
                ClearingFlashFinishedEvent = (ClearingFlashFinishedEventHandler)System.Delegate.Combine(ClearingFlashFinishedEvent, value);
            }
            remove
            {
                ClearingFlashFinishedEvent = (ClearingFlashFinishedEventHandler)System.Delegate.Remove(ClearingFlashFinishedEvent, value);
            }
        }

        public delegate void AddressMemoryFlashFinishedEventHandler(byte[] Datos);
        private AddressMemoryFlashFinishedEventHandler AddressMemoryFlashFinishedEvent;

        public event AddressMemoryFlashFinishedEventHandler AddressMemoryFlashFinished
        {
            add
            {
                AddressMemoryFlashFinishedEvent = (AddressMemoryFlashFinishedEventHandler)System.Delegate.Combine(AddressMemoryFlashFinishedEvent, value);
            }
            remove
            {
                AddressMemoryFlashFinishedEvent = (AddressMemoryFlashFinishedEventHandler)System.Delegate.Remove(AddressMemoryFlashFinishedEvent, value);
            }
        }

        public delegate void DataMemoryFlashFinishedEventHandler(byte[] Datos);
        private DataMemoryFlashFinishedEventHandler DataMemoryFlashFinishedEvent;

        public event DataMemoryFlashFinishedEventHandler DataMemoryFlashFinished
        {
            add
            {
                DataMemoryFlashFinishedEvent = (DataMemoryFlashFinishedEventHandler)System.Delegate.Combine(DataMemoryFlashFinishedEvent, value);
            }
            remove
            {
                DataMemoryFlashFinishedEvent = (DataMemoryFlashFinishedEventHandler)System.Delegate.Remove(DataMemoryFlashFinishedEvent, value);
            }
        }

        public delegate void EndProgFinishedEventHandler(byte[] Datos);
        private EndProgFinishedEventHandler EndProgFinishedEvent;

        public event EndProgFinishedEventHandler EndProgFinished
        {
            add
            {
                EndProgFinishedEvent = (EndProgFinishedEventHandler)System.Delegate.Combine(EndProgFinishedEvent, value);
            }
            remove
            {
                EndProgFinishedEvent = (EndProgFinishedEventHandler)System.Delegate.Remove(EndProgFinishedEvent, value);
            }
        }

        public delegate void UpdateMicroFirmwareFinishedEventHandler();
        private UpdateMicroFirmwareFinishedEventHandler UpdateMicroFirmwareFinishedEvent;

        public event UpdateMicroFirmwareFinishedEventHandler UpdateMicroFirmwareFinished
        {
            add
            {
                UpdateMicroFirmwareFinishedEvent = (UpdateMicroFirmwareFinishedEventHandler)System.Delegate.Combine(UpdateMicroFirmwareFinishedEvent, value);
            }
            remove
            {
                UpdateMicroFirmwareFinishedEvent = (UpdateMicroFirmwareFinishedEventHandler)System.Delegate.Remove(UpdateMicroFirmwareFinishedEvent, value);
            }
        }


        private enum enumUpdaterState
        {
            initial,
            goprocess,
            stopstep
        }

        private enum UpdateFirmwareState
        {
            None,
            ClearingFlash,
            SendingAddress,
            SendingData,
            EndProgramming,
            Cancelled
        }

        private CMicroPrograms02 m_cMicro = null;
        private UpdateFirmwareState m_UpdateFirmwareState;
        private CMicroPrograms02.s19rec m_s19rec = null;
        private int m_s19recAntAddress = 0;
        private int iS19RecCount = 0;
        private int iUpdaterCount = 0;
        private byte m_UpdateFirmwareSequence = (byte)0;
        private bool bStepByStep = false;
        private bool bUpdaterCancelled = false;

        public void butSearchFile_Click(System.Object sender, System.EventArgs e)
        {
            OpenFileDialog1.Filter = "S19|*.s19|Hex|*.hex|All|*.*";
            if (tbProgramFile.Text == "")
            {
                OpenFileDialog1.InitialDirectory = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath;
                OpenFileDialog1.FileName = "";
            }
            else
            {
                OpenFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(tbProgramFile.Text);
                OpenFileDialog1.FileName = System.IO.Path.GetFileName(sUpdaterLastPathFile);
            }
            if (OpenFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (OpenFileDialog1.FileName.Trim() != "")
                {
                    tbProgramFile.Text = OpenFileDialog1.FileName;
                    sUpdaterLastPathFile = System.IO.Path.GetDirectoryName(OpenFileDialog1.FileName);
                }
            }
        }

        public void butUpdater_Click(System.Object sender, System.EventArgs e)
        {
            // check selected file
            if (!System.IO.File.Exists(tbProgramFile.Text))
            {
                MessageBox.Show(tbProgramFile.Text + " does not exist.");
                return;
            }

            string sErr = "";
            if (ReferenceEquals(m_cMicro, null))
            {
                m_cMicro = new CMicroPrograms02();
            }
            if (!m_cMicro.Load(tbProgramFile.Text, ref sErr))
            {
                MessageBox.Show(sErr);
                return;
            }

            // save S19 to file
            string sError = "";
            string s19Pathfilename = "";
            tbProcessedS19.Text = "";

            if (cbSaveS19ToProcess.Checked)
            {
                // converted and sorted
                sError = "";
                s19Pathfilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(tbProgramFile.Text), System.IO.Path.GetFileNameWithoutExtension(tbProgramFile.Text) + "_converted_sorted.S19");
                if (!m_cMicro.SaveConvertedAndSorted(s19Pathfilename, ref sError))
                {
                    MessageBox.Show("Error when saving S19: " + sError);
                }
                else
                {
                    tbProcessedS19.AppendText(s19Pathfilename + "\r\n");
                }

                // processed
                sError = "";
                s19Pathfilename = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(tbProgramFile.Text), System.IO.Path.GetFileNameWithoutExtension(tbProgramFile.Text) + "_processed.S19");
                if (!m_cMicro.Save(s19Pathfilename, ref sError))
                {
                    MessageBox.Show("Error when saving S19: " + sError);
                }
                else
                {
                    tbProcessedS19.AppendText(s19Pathfilename + "\r\n");
                }

            }

            // save last process data
            saveConfigData();

            m_s19rec = null;
            m_s19recAntAddress = 0;
            bUpdaterCancelled = false;
            m_UpdateFirmwareState = UpdateFirmwareState.None;

            bStepByStep = false;
            if (((Button)sender).Name == butStepStart.Name)
            {
                bStepByStep = true;
            }
            processUpdater();

        }

        private void processUpdater()
        {
            if (tbBorrarMemoriaFlash.Text == "")
            {
                MessageBox.Show("Faltan los datos a enviar al borrar la memoria flash");
                return;
            }
            m_s19rec = null;

            updaterUI(enumUpdaterState.goprocess);
            iUpdaterCount = 0;
            m_UpdateFirmwareSequence = (byte)0;
            iS19RecCount = m_cMicro.initUpdaterData();
            m_UpdateFirmwareState = UpdateFirmwareState.ClearingFlash;
            LogAdd("Clear flash memory (22h)");

            // send Borrar memoria flash
            if (stackApl != null && !cbUpdaterTestOnly.Checked)
            {
                // Borrar memoria flash
                ClearMemoryFlash(tbBorrarMemoriaFlash.Text, (byte)numTargetDevice.Value);
                // continua en event_ClearingFlashFinished, cuando la estación informa del fin de borrado de memoria
            }
            else
            {
                if (!bStepByStep)
                {
                    sendUpdaterRec(); // go
                }
                else
                {
                    updaterUI(enumUpdaterState.stopstep); // enable "next" button
                }
            }

        }

        public void event_ClearingFlashInProgress(byte[] Datos)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new ClearingFlashInProgressEventHandler(event_ClearingFlashInProgress), new object[] { Datos });
                return;
            }

            int paginas = Datos[0];
            int paginaborrada = Datos[1];
            LogAdd(string.Format("Evento ClearingFlashInProgress (28h) - Pages:{0} Cleared page:{1}", paginas.ToString(), paginaborrada.ToString()));
        }

        public void event_ClearingFlashFinished(byte[] Datos)
        {
            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new ClearingFlashFinishedEventHandler(event_ClearingFlashFinished), new object[] { Datos });
                return;
            }

            LogAdd(string.Format("Evento ClearingFlashFinished (22h)"));

            // continuar con updater
            if (!bStepByStep)
            {
                sendUpdaterRec(); // go
            }
            else
            {
                updaterUI(enumUpdaterState.stopstep); // enable "next" button
            }
        }

        public void event_WrittenFlashAddress(byte[] Datos)
        {
            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new AddressMemoryFlashFinishedEventHandler(event_WrittenFlashAddress), new object[] { Datos });
                return;
            }

            int secuencia = -1;
            string sRet = "";
            if (Datos[0] == ((byte)(0x15)))
            {
                sRet = "NACK";
            }
            else if (Datos[0] == ((byte)(0x6)))
            {
                sRet = "ACK";
            }
            else
            {
                sRet = System.Convert.ToString(Datos[0].ToString("X"));
            }
            if (myCommandProtocol == CStationBase.Protocol.Protocol_02 & Datos.Length > 1)
            {
                secuencia = Datos[1];
            }
            LogAdd(string.Format("Evento AddressMemoryFlashFinished (23h) - Secuencia:{0} {1}", secuencia.ToString(), sRet));

            if (cbProgDelay.Checked && nudProgDelay.Value > 0)
            {
                timerProgDelay.Start();
            }
            else
            {
                if (!bStepByStep)
                {
                    sendUpdaterRec(); // go
                }
                else
                {
                    updaterUI(enumUpdaterState.stopstep); // enable "next" button
                }
            }
        }

        public void event_WrittenFlashData(byte[] Datos)
        {
            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new DataMemoryFlashFinishedEventHandler(event_WrittenFlashData), new object[] { Datos });
                return;
            }

            int secuencia = -1;
            string sRet = "";
            if (Datos[0] == ((byte)(0x15)))
            {
                sRet = "NACK";
            }
            else if (Datos[0] == ((byte)(0x6)))
            {
                sRet = "ACK";
            }
            else
            {
                sRet = System.Convert.ToString(Datos[0].ToString("X"));
            }
            if (myCommandProtocol == CStationBase.Protocol.Protocol_02 & Datos.Length > 1)
            {
                secuencia = Datos[1];
            }
            LogAdd(string.Format("Evento DataMemoryFlashFinished (24h) - Secuencia:{0} {1}", secuencia.ToString(), sRet));

            if (cbProgDelay.Checked && nudProgDelay.Value > 0)
            {
                timerProgDelay.Start();
            }
            else
            {
                if (!bStepByStep)
                {
                    sendUpdaterRec(); // go
                }
                else
                {
                    updaterUI(enumUpdaterState.stopstep); // enable "next" button
                }
            }

        }

        public void event_UpdateMicroFirmwareFinished()
        {
            //required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateMicroFirmwareFinishedEventHandler(event_UpdateMicroFirmwareFinished), new object[] { });
                return;
            }
            LogAdd("End Update.");

        }

        public void nudProgDelay_ValueChanged(object sender, EventArgs e)
        {
            timerProgDelay.Interval = (int)nudProgDelay.Value;
        }

        public void timerProgDelay_Tick(object sender, EventArgs e)
        {
            timerProgDelay.Stop();
            if (!bStepByStep)
            {
                sendUpdaterRec(); // go
            }
            else
            {
                updaterUI(enumUpdaterState.stopstep); // enable "next" button
            }
        }

        public void cbProgDelay_CheckedChanged(object sender, EventArgs e)
        {
            if (cbProgDelay.Checked && nudProgDelay.Value > 0)
            {
                timerProgDelay.Interval = (int)nudProgDelay.Value;
            }
            else
            {
                timerProgDelay.Stop();
            }
        }

        private bool sendUpdaterRec()
        {
            byte[] SendData = null;

            //
            //Pasar al siguiente estado
            //
            if (m_UpdateFirmwareState == UpdateFirmwareState.ClearingFlash |
                    m_UpdateFirmwareState == UpdateFirmwareState.SendingData)
            {
                if (m_UpdateFirmwareState == UpdateFirmwareState.ClearingFlash)
                {
                    m_UpdateFirmwareState = UpdateFirmwareState.SendingAddress;

                    //Inicializa el contador de secuencia
                    m_UpdateFirmwareSequence = (byte)0;
                }

                if (m_s19rec != null)
                {
                    m_s19recAntAddress = RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true); //is BigEndian
                }

                //No hay mas datos para leer
                if (!m_cMicro.getNextUpdaterData(ref m_s19rec))
                {
                    m_UpdateFirmwareState = UpdateFirmwareState.EndProgramming;

                    LogAdd("Send End Program (25h)");
                    EndProgramming((byte)numTargetDevice.Value);

                }

            }
            else if (m_UpdateFirmwareState == UpdateFirmwareState.SendingAddress)
            {
                m_UpdateFirmwareState = UpdateFirmwareState.SendingData;

            }
            else if (m_UpdateFirmwareState == UpdateFirmwareState.EndProgramming)
            {

                m_UpdateFirmwareState = UpdateFirmwareState.None;
            }

            //
            //Enviar dato
            //
            if (m_UpdateFirmwareState == UpdateFirmwareState.SendingData)
            {

                //No es un bloque consecutivo. Enviar address
                if ((m_s19recAntAddress + CMicroPrograms02.BLOCK_MICRO_PROGRAM) < RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true)) //is BigEndian
                {
                    m_UpdateFirmwareState = UpdateFirmwareState.SendingAddress;

                }
                else
                {
                    SendData = new byte[m_s19rec.data.ToArray().Length + 1];
                    SendData[0] = m_UpdateFirmwareSequence;
                    Array.Copy(m_s19rec.data.ToArray(), 0, SendData, 1, m_s19rec.data.ToArray().Length);

                    m_s19recAntAddress = RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true); //is BigEndian

                    LogAdd("Send Data (24h)");
                    DataMemoryFlash(SendData, (byte)numTargetDevice.Value);
                }
            }

            //
            //Enviar address
            //
            if (m_UpdateFirmwareState == UpdateFirmwareState.SendingAddress)
            {

                SendData = new byte[m_s19rec.address.ToArray().Length + 1];
                SendData[0] = m_UpdateFirmwareSequence;
                Array.Copy(m_s19rec.address.ToArray(), 0, SendData, 1, m_s19rec.address.ToArray().Length);

                m_s19recAntAddress = RoutinesLibrary.Data.DataType.IntegerUtils.BytesToInt(m_s19rec.address.ToArray(), true); //is BigEndian

                LogAdd("Send Address (23h)");
                AddressMemoryFlash(SendData, (byte)numTargetDevice.Value);
            }

            //Siguiente secuencia
            if (m_UpdateFirmwareSequence == 255)
            {
                m_UpdateFirmwareSequence = (byte)0;
            }
            else
            {
                m_UpdateFirmwareSequence += 1;
            }

            return true;
        }

        public void butStepNext_Click(System.Object sender, System.EventArgs e)
        {
            sendUpdaterRec();
        }

        public void butStepGo_Click(System.Object sender, System.EventArgs e)
        {
            bStepByStep = !bStepByStep;
            if (bStepByStep)
            {
                updaterUI(enumUpdaterState.stopstep);
                sendUpdaterRec();
            }
            else
            {
                updaterUI(enumUpdaterState.goprocess);
                sendUpdaterRec();
            }

        }

        public void butUpdaterCancel_Click(System.Object sender, System.EventArgs e)
        {
            bUpdaterCancelled = true;
            LogAdd("CANCELADO");
            updaterUI(enumUpdaterState.initial);
        }

        private void updaterUI(enumUpdaterState state)
        {
            switch (state)
            {
                case enumUpdaterState.initial:
                    butUpdater.Enabled = true;
                    butStepStart.Enabled = true;
                    butStepNext.Enabled = false;
                    butStepGo.Enabled = false;
                    butUpdaterCancel.Enabled = false;
                    break;
                case enumUpdaterState.goprocess:
                    butUpdater.Enabled = false;
                    butStepStart.Enabled = false;
                    butStepNext.Enabled = false;
                    butStepGo.Text = "Stop";
                    butStepGo.Enabled = true;
                    butUpdaterCancel.Enabled = true;
                    break;
                case enumUpdaterState.stopstep:
                    butStepNext.Enabled = true;
                    butStepGo.Text = "Go";
                    butStepGo.Enabled = true;
                    butUpdaterCancel.Enabled = true;
                    break;
            }
            Application.DoEvents();
        }


        #region Clear memory flash 0x22

        /// <summary>
        /// Borra la memoria de programa preparada para almacenar el nuevo programa
        /// </summary>
        /// <param name="firmwareName">Indica el tipo de estación que será, el software que tendrá y el hardware que soportará</param>
        /// <param name="address">Dirección destino</param>
        public void ClearMemoryFlash(string firmwareName, byte address)
        {
            //Datos
            var Datos = Encoding.UTF8.GetBytes(firmwareName);

            //Command
            byte Command = (byte)EnumCommandFrame_02_SOLD.M_CLEARMEMFLASH;

            //stackApl.Send(Datos, Command, address, True) 'Delayed response
            SendMessage(Datos, Command, address, true);
        }

        #endregion


        #region Address memory flash 0x23

        /// <summary>
        /// Establece la dirección de memoria a programar
        /// </summary>
        /// <param name="Datos">Dirección de memoria a programar</param>
        /// <param name="address">Dirección destino</param>
        public void AddressMemoryFlash(byte[] Datos, byte address)
        {
            //Command
            byte Command = (byte)EnumCommandFrame_02_SOLD.M_SENDMEMADDRESS;

            //stackApl.Send(Datos, Command, address)
            SendMessage(Datos, Command, address);
        }

        #endregion


        #region Data memory flash 0x24

        /// <summary>
        /// Envía un bloque de datos a programar
        /// </summary>
        /// <param name="Datos">Bloque de datos a programar</param>
        /// <param name="address">Dirección destino</param>
        public void DataMemoryFlash(byte[] Datos, byte address)
        {
            //Command
            byte Command = (byte)EnumCommandFrame_02_SOLD.M_SENDMEMDATA;

            //stackApl.Send(Datos, Command, address)
            SendMessage(Datos, Command, address);
        }

        #endregion


        #region End programming 0x25

        /// <summary>
        /// Fin de programación
        /// </summary>
        /// <param name="address">Dirección destino</param>
        public void EndProgramming(byte address)
        {
            //Datos
            var Datos = new byte[] { };

            //Command
            byte Command = (byte)EnumCommandFrame_02_SOLD.M_ENDPROGR;

            //stackApl.Send(Datos, Command, address)
            SendMessage(Datos, Command, address);
        }

        #endregion


        #endregion

        #region UID
        public void butNewUID_Click(object sender, EventArgs e)
        {
            clsStationUID myUID = default(clsStationUID);
            if (rdMACType.Checked)
            {
                myUID = new clsStationUID(clsStationUID.enumType.MAC);
            }
            else if (rdGUIDType.Checked)
            {
                myUID = new clsStationUID(clsStationUID.enumType.GUIDS);
            }
            else
            {
                myUID = new clsStationUID(clsStationUID.enumType.MAC);
            }
            tbMemoryUID.Text = myUID.UID;
            tbStationUID.Text = RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToHexa(myUID.StationData);
        }

        public void butMemoryToStation_Click(object sender, EventArgs e)
        {
            if (tbMemoryUID.Text != "")
            {
                clsStationUID myUID = new clsStationUID(tbMemoryUID.Text);
                tbConvertedUID.Text = RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToHexa(myUID.StationData);
            }
        }

        public void butStationToMemory_Click(object sender, EventArgs e)
        {
            if (tbStationUID.Text != "")
            {
                clsStationUID myUID = new clsStationUID(RoutinesLibrary.Data.DataType.StringUtils.HexaToByteArray(tbStationUID.Text).ToArray());
                tbConvertedUID.Text = myUID.UID;
            }
        }

        #endregion

        #region Check Frame
        private CDllFifoMessages decodeStuffedData(byte[] stuffedData, CStationBase.Protocol protocol, ref int iErrors)
        {
            iErrors = 0;
            CFrameData frame = default(CFrameData);
            CDllFifoMessages MessageFIFO = new CDllFifoMessages();
            frame = new CFrameData(protocol);
            iErrors = frame.DecodeCheckReceivedData(stuffedData, -1, ref MessageFIFO);
            return MessageFIFO;
        }

        public void butCalcBCC_Click(object sender, EventArgs e)
        {
            string sFieldData = tbHexaBytes.Text;
            int iLen = System.Convert.ToInt32((double)sFieldData.Length / 2);
            byte[] bytes = null;
            sFieldData = sFieldData.Replace("-", "");
            bytes = RoutinesLibrary.Data.DataType.StringUtils.HexaToByteArray(sFieldData).ToArray();
            int iErrors = 0;
            CDllFifoMessages MessageFIFO = decodeStuffedData(bytes, CStationBase.Protocol.Protocol_02, ref iErrors);
            MessageBox.Show("Errors: " + iErrors.ToString());

        }
        #endregion


        #region Files

        private enum FilesOperation
        {
            None,
            ReadingFile_Start,
            ReadingFile_Blocks,
            ReadingFile_End,
            WritingFile_Start,
            WritingFile_Blocks,
            WritingFile_End,
            ListingFiles_Count,
            ListingFiles_Filename,
            DeletingFile,
            SelectedFile,
            SelectingFile,
            Manual
        }

        // Files events
        public delegate void StartReadingFileEventHandler(bool bACK, int bytesCount);
        private StartReadingFileEventHandler StartReadingFileEvent;

        public event StartReadingFileEventHandler StartReadingFile
        {
            add
            {
                StartReadingFileEvent = (StartReadingFileEventHandler)System.Delegate.Combine(StartReadingFileEvent, value);
            }
            remove
            {
                StartReadingFileEvent = (StartReadingFileEventHandler)System.Delegate.Remove(StartReadingFileEvent, value);
            }
        }

        public delegate void BlockReadingFileEventHandler(int sequende, byte[] data);
        private BlockReadingFileEventHandler BlockReadingFileEvent;

        public event BlockReadingFileEventHandler BlockReadingFile
        {
            add
            {
                BlockReadingFileEvent = (BlockReadingFileEventHandler)System.Delegate.Combine(BlockReadingFileEvent, value);
            }
            remove
            {
                BlockReadingFileEvent = (BlockReadingFileEventHandler)System.Delegate.Remove(BlockReadingFileEvent, value);
            }
        }

        public delegate void EndReadingFileEventHandler();
        private EndReadingFileEventHandler EndReadingFileEvent;

        public event EndReadingFileEventHandler EndReadingFile
        {
            add
            {
                EndReadingFileEvent = (EndReadingFileEventHandler)System.Delegate.Combine(EndReadingFileEvent, value);
            }
            remove
            {
                EndReadingFileEvent = (EndReadingFileEventHandler)System.Delegate.Remove(EndReadingFileEvent, value);
            }
        }

        public delegate void StartWritingFileEventHandler(bool bOk);
        private StartWritingFileEventHandler StartWritingFileEvent;

        public event StartWritingFileEventHandler StartWritingFile
        {
            add
            {
                StartWritingFileEvent = (StartWritingFileEventHandler)System.Delegate.Combine(StartWritingFileEvent, value);
            }
            remove
            {
                StartWritingFileEvent = (StartWritingFileEventHandler)System.Delegate.Remove(StartWritingFileEvent, value);
            }
        }

        public delegate void BlockWritingFileEventHandler(int sequende, bool bACK);
        private BlockWritingFileEventHandler BlockWritingFileEvent;

        public event BlockWritingFileEventHandler BlockWritingFile
        {
            add
            {
                BlockWritingFileEvent = (BlockWritingFileEventHandler)System.Delegate.Combine(BlockWritingFileEvent, value);
            }
            remove
            {
                BlockWritingFileEvent = (BlockWritingFileEventHandler)System.Delegate.Remove(BlockWritingFileEvent, value);
            }
        }

        public delegate void EndWritingFileEventHandler();
        private EndWritingFileEventHandler EndWritingFileEvent;

        public event EndWritingFileEventHandler EndWritingFile
        {
            add
            {
                EndWritingFileEvent = (EndWritingFileEventHandler)System.Delegate.Combine(EndWritingFileEvent, value);
            }
            remove
            {
                EndWritingFileEvent = (EndWritingFileEventHandler)System.Delegate.Remove(EndWritingFileEvent, value);
            }
        }

        public delegate void FileCountEventHandler(int count);
        private FileCountEventHandler FileCountEvent;

        public event FileCountEventHandler FileCount
        {
            add
            {
                FileCountEvent = (FileCountEventHandler)System.Delegate.Combine(FileCountEvent, value);
            }
            remove
            {
                FileCountEvent = (FileCountEventHandler)System.Delegate.Remove(FileCountEvent, value);
            }
        }

        public delegate void FileNameEventHandler(string fileName);
        private FileNameEventHandler FileNameEvent;

        public event FileNameEventHandler FileName
        {
            add
            {
                FileNameEvent = (FileNameEventHandler)System.Delegate.Combine(FileNameEvent, value);
            }
            remove
            {
                FileNameEvent = (FileNameEventHandler)System.Delegate.Remove(FileNameEvent, value);
            }
        }

        public delegate void DeletedFileNameEventHandler(bool bACK);
        private DeletedFileNameEventHandler DeletedFileNameEvent;

        public event DeletedFileNameEventHandler DeletedFileName
        {
            add
            {
                DeletedFileNameEvent = (DeletedFileNameEventHandler)System.Delegate.Combine(DeletedFileNameEvent, value);
            }
            remove
            {
                DeletedFileNameEvent = (DeletedFileNameEventHandler)System.Delegate.Remove(DeletedFileNameEvent, value);
            }
        }

        public delegate void CurrentSelectedFileNameEventHandler(string fileName);
        private CurrentSelectedFileNameEventHandler CurrentSelectedFileNameEvent;

        public event CurrentSelectedFileNameEventHandler CurrentSelectedFileName
        {
            add
            {
                CurrentSelectedFileNameEvent = (CurrentSelectedFileNameEventHandler)System.Delegate.Combine(CurrentSelectedFileNameEvent, value);
            }
            remove
            {
                CurrentSelectedFileNameEvent = (CurrentSelectedFileNameEventHandler)System.Delegate.Remove(CurrentSelectedFileNameEvent, value);
            }
        }

        public delegate void SelectedFileEventHandler(bool bACK);
        private SelectedFileEventHandler SelectedFileEvent;

        public event SelectedFileEventHandler SelectedFile
        {
            add
            {
                SelectedFileEvent = (SelectedFileEventHandler)System.Delegate.Combine(SelectedFileEvent, value);
            }
            remove
            {
                SelectedFileEvent = (SelectedFileEventHandler)System.Delegate.Remove(SelectedFileEvent, value);
            }
        }


        private FilesOperation filesCurrentOperation = FilesOperation.None;
        // list
        private int filesListCount = 0;
        private List<string> filesList = new List<string>();
        // get/set
        private string filesCurrentFilename = "";
        private int filesCurrentSequence = 0;
        private int filesCurrentSize = 0;
        private int filesCurrentBlocksToProcess = 0;
        private int filesCurrentCount = 0;
        private List<byte> filesCurrentFileData = new List<byte>();
        const int filesBlockSize = 128;


        public void event_FileCount(int count)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new FileCountEventHandler(event_FileCount), new object[] { count });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }

            filesListCount = count;
            filesList.Clear();
            lbFiles_List.Items.Clear();
            if (filesListCount > 0)
            {
                filesCurrentCount = 1;
                filesCurrentOperation = FilesOperation.ListingFiles_Filename;
                ReadFileName(filesCurrentCount - 1);
            }
            else
            {
                filesCurrentOperation = FilesOperation.None;
            }
        }

        public void event_FileName(string fileName)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new FileNameEventHandler(event_FileName), new object[] { fileName });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }

            if (fileName != "")
            {
                filesList.Add(fileName);
                lbFiles_List.Items.Add(fileName);
            }
            if (filesListCount > filesCurrentCount)
            {
                filesCurrentCount++;
                filesCurrentOperation = FilesOperation.ListingFiles_Filename;
                ReadFileName(filesCurrentCount - 1);
            }
            else
            {
                lbFiles_List.SelectedIndex = 0;
                filesCurrentOperation = FilesOperation.None;
            }
        }

        public void event_StartReadingFile(bool bACK, int bytesCount)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new StartReadingFileEventHandler(event_StartReadingFile), new object[] { bACK, bytesCount });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }
            if (filesCurrentOperation != FilesOperation.ReadingFile_Start)
            {
                return;
            }

            if (!bACK)
            {
                MessageBox.Show(string.Format("Error al comenzar a leer el archivo {0}", filesCurrentFilename));
                filesCurrentOperation = FilesOperation.None;
                return;
            }
            if (bytesCount > 0)
            {
                filesCurrentSize = bytesCount; // NO IMPLEMENTADO, VIENE CERO
                filesCurrentBlocksToProcess = filesCurrentSize / filesBlockSize;
                if (filesCurrentSize % filesBlockSize != 0)
                {
                    filesCurrentBlocksToProcess++;
                }
            }
            else
            {
                filesCurrentSize = -1; // NO IMPLEMENTADO bytesCount, VIENE CERO
                filesCurrentBlocksToProcess = -1;
            }
            filesCurrentSequence = 0;
            filesCurrentCount = 0;
            filesCurrentFileData.Clear();

            filesCurrentOperation = FilesOperation.ReadingFile_Blocks;
            ReadFile_Block(filesCurrentSequence);
        }

        public void event_BlockReadingFile(int sequence, byte[] data)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new BlockReadingFileEventHandler(event_BlockReadingFile), new object[] { sequence, data });
                return;
            }

            List<byte> bytesRead = new List<byte>();

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }
            if (filesCurrentOperation != FilesOperation.ReadingFile_Blocks)
            {
                return;
            }

            // asumimos que no hay más datos por data.length = 0
            if (data.Length > 0)
            {

                // si la secuencia que recibo es menor al bloque que he enviado,
                // no tratar porque puede ser la respuesta de un reintento de envío a nivel más bajo
                if (sequence < filesCurrentSequence)
                {
                    return;
                }

                if (sequence > filesCurrentSequence)
                {
                    MessageBox.Show(string.Format("Sequence error when reading blocks of file {0}", filesCurrentFilename));
                    filesCurrentOperation = FilesOperation.None;
                    return;
                }

                // leído bloque
                filesCurrentCount++;
                // bytes del bloque según tamaño y bloques leídos
                int bytesBlock = -1;
                if (filesCurrentSize == -1)
                {
                    // bytes count NO IMPLEMENTADO en Start Reading File
                    bytesBlock = filesBlockSize;
                }
                else
                {
                    bytesBlock = getBlockDataBytesCount(filesCurrentCount, filesBlockSize, filesCurrentSize);
                }

                // quitar nulos finales, si existen
                //For Each byt As Byte In data
                //    If byt <> &H0 Then
                //        bytesRead.Add(byt)
                //    End If
                //Next
                bytesRead.AddRange(data);

                // controlar si se ha recibido datos más allá de los datos correspondientes al bloque
                // añadir datos
                bytesBlock = Math.Min(bytesBlock, System.Convert.ToInt32(bytesRead.Count));
                for (var i = 0; i <= bytesBlock - 1; i++)
                {
                    filesCurrentFileData.Add(bytesRead[i]);
                }

                // es el último a leer: si lo leído es menor a 128 o llega al tope de bloques a leer
                // si bytes count NO IMPLEMENTADO en Start Reading File, filesCurrentSize y filesCurrentBlocksToProcess viene con -1
                if (bytesBlock < filesBlockSize || (filesCurrentCount == filesCurrentBlocksToProcess & filesCurrentBlocksToProcess != -1))
                {
                    filesCurrentOperation = FilesOperation.ReadingFile_End;
                    ReadFile_End();
                }
                else
                {
                    if (filesCurrentSequence == int.MaxValue)
                    {
                        filesCurrentSequence = 0;
                    }
                    else
                    {
                        filesCurrentSequence++;
                    }
                    filesCurrentOperation = FilesOperation.ReadingFile_Blocks;
                    ReadFile_Block(filesCurrentSequence);
                }
            }
            else
            {
                // fin por data.length = 0
                filesCurrentOperation = FilesOperation.ReadingFile_End;
                ReadFile_End();
            }


        }

        public void event_EndReadingFile()
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new EndReadingFileEventHandler(event_EndReadingFile), new object[] { });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }
            if (filesCurrentOperation != FilesOperation.ReadingFile_End)
            {
                return;
            }

            string sSavedAs = "";
            if (string.IsNullOrEmpty(sFilesLastPathFile))
            {
                SaveFileDialog1.InitialDirectory = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath;
                SaveFileDialog1.FileName = filesCurrentFilename;
            }
            else
            {
                SaveFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(sFilesLastPathFile);
                SaveFileDialog1.FileName = filesCurrentFilename;
            }
            if (SaveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (SaveFileDialog1.FileName.Trim() != "")
                {
                    sSavedAs = SaveFileDialog1.FileName;
                    System.IO.File.WriteAllBytes(sSavedAs, filesCurrentFileData.ToArray());
                    sFilesLastPathFile = SaveFileDialog1.FileName;
                }
            }

            if (!string.IsNullOrEmpty(sSavedAs))
            {
                LogAdd(string.Format("=== File {0} saved in {1}", filesCurrentFilename, sSavedAs));
            }
            else
            {
                LogAdd(string.Format("=== File {0}", filesCurrentFilename));
                LogAdd(RoutinesLibrary.Data.DataType.StringUtils.ByteArrayToHexa(filesCurrentFileData.ToArray(), "-"));
                LogAdd(string.Format("=== End"));
            }
            filesCurrentOperation = FilesOperation.None;

        }

        public void event_StartWritingFile(bool bACK)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new StartWritingFileEventHandler(event_StartWritingFile), new object[] { bACK });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }
            if (filesCurrentOperation != FilesOperation.WritingFile_Start)
            {
                return;
            }

            if (!bACK)
            {
                MessageBox.Show(string.Format("Error al comenzar a enviar el archivo {0}", filesCurrentFilename));
                filesCurrentOperation = FilesOperation.None;
                return;
            }
            filesCurrentSequence = 0;
            filesCurrentBlocksToProcess = filesCurrentSize / filesBlockSize;
            if (filesCurrentSize % filesBlockSize != 0)
            {
                filesCurrentBlocksToProcess++;
            }
            filesCurrentCount = 0;

            byte[] blockData = null;
            int bytesToSend = filesBlockSize;
            if (filesCurrentSize < bytesToSend)
            {
                bytesToSend = filesCurrentSize;
            }
            blockData = new byte[bytesToSend - 1 + 1];
            // copiar los datos a enviar a blockData
            filesCurrentFileData.CopyTo(0, blockData, 0, bytesToSend);
            // enviar primera secuencia
            filesCurrentOperation = FilesOperation.WritingFile_Blocks;
            WriteFile_Block(filesCurrentSequence, blockData);
        }

        private int getBlockDataBytesCount(int iBlockNumber, int iBlockSize, int iTotalSize)
        {
            int bytesPrevious = iBlockSize * (iBlockNumber - 1);
            return Math.Min(iTotalSize - bytesPrevious, iBlockSize);
        }

        public void event_BlockWritingFile(int sequence, bool bACK)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new BlockWritingFileEventHandler(event_BlockWritingFile), new object[] { sequence, bACK });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }
            if (filesCurrentOperation != FilesOperation.WritingFile_Blocks)
            {
                return;
            }

            // si la secuencia que recibo es menor al bloque que he enviado,
            // no tratar porque puede ser la respuesta de un reintento de envío a nivel más bajo
            if (sequence < filesCurrentSequence)
            {
                return;
            }

            if (sequence > filesCurrentSequence)
            {
                MessageBox.Show(string.Format("Error de secuencia al enviar bloques del archivo {0}", filesCurrentFilename));
                filesCurrentOperation = FilesOperation.None;
                return;
            }

            if (!bACK)
            {
                MessageBox.Show(string.Format("Error al enviar bloques del archivo {0}, secuencia {1}", filesCurrentFilename, sequence));
                filesCurrentOperation = FilesOperation.None;
                return;
            }

            filesCurrentCount++; // enviado
                                 // es el último a enviar
            if (filesCurrentCount == filesCurrentBlocksToProcess)
            {
                filesCurrentOperation = FilesOperation.WritingFile_End;
                WriteFile_End();
            }
            else
            {
                if (filesCurrentSequence == int.MaxValue)
                {
                    filesCurrentSequence = 0;
                }
                else
                {
                    filesCurrentSequence++;
                }

                byte[] blockData = null;
                int bytesToSend = filesBlockSize;
                int bytesSent = filesBlockSize * filesCurrentCount;
                int bytesLeft = filesCurrentSize - bytesSent;
                if (bytesLeft < bytesToSend)
                {
                    bytesToSend = bytesLeft;
                }
                blockData = new byte[bytesToSend - 1 + 1];
                // copiar los datos a enviar a blockData
                filesCurrentFileData.CopyTo(bytesSent, blockData, 0, bytesToSend);
                // enviar siguientes datos
                filesCurrentOperation = FilesOperation.WritingFile_Blocks;
                WriteFile_Block(filesCurrentSequence, blockData);
            }
        }

        public void event_EndWritingFile()
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new EndWritingFileEventHandler(event_EndWritingFile), new object[] { });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }
            if (filesCurrentOperation != FilesOperation.WritingFile_End)
            {
                return;
            }

            LogAdd(string.Format("=== File {0} sent to station.", filesCurrentFilename));
            filesCurrentOperation = FilesOperation.None;

            if (Interaction.MsgBox("Update list?", MsgBoxStyle.YesNo, null) == MsgBoxResult.Yes)
            {
                butFiles_ListFiles_Click(null, null);
            }
        }

        //DeletedFileName
        public void event_DeletedFileName(bool bACK)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new DeletedFileNameEventHandler(event_DeletedFileName), new object[] { bACK });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }

            if (!bACK)
            {
                MessageBox.Show(string.Format("Error when trying to delete file {0}", filesCurrentFilename));
                filesCurrentOperation = FilesOperation.None;
                return;
            }
            LogAdd(string.Format("=== Deleted file: {0}.", filesCurrentFilename));
            filesList.Remove(filesCurrentFilename);
            lbFiles_List.Items.Remove(filesCurrentFilename);
            filesCurrentOperation = FilesOperation.None;

        }


        public void event_CurrentFileName(string fileName)
        {
            //'required as long as this method is called from a diferent thread
            if (this.InvokeRequired)
            {
                this.Invoke(new CurrentSelectedFileNameEventHandler(event_CurrentFileName), new object[] { fileName });
                return;
            }

            // si no se realizó desde la pestaña de Files
            if (filesCurrentOperation == FilesOperation.Manual)
            {
                return;
            }
            if (filesCurrentOperation != FilesOperation.SelectedFile)
            {
                return;
            }

            if (fileName != "")
            {
                MessageBox.Show("Current selected file: " + fileName);
                LogAdd(string.Format("=== Current file: {0}.", fileName));
            }
            filesCurrentOperation = FilesOperation.None;
        }

        #region Files methods

        #region Read File 0x90 0x91 0x92

        /// <summary>
        /// Le indica al equipo que va a iniciar la lectura de un archivo
        /// </summary>
        /// <remarks></remarks>
        public void ReadFile_Start(string fileName)
        {
            //Datos
            const int maxLength = 12;
            fileName = fileName.Substring(0, maxLength);
            List<byte> Datos = new List<byte>();
            Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
            //For i = 1 To maxLength - Datos.Count
            //    Datos.Add(&H0) ' rellenar con nulos
            //Next

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_READSTARTFILE;

            SendMessage(Datos.ToArray(), Command);
        }

        /// <summary>
        /// Pide al equipo un bloque de datos de 128 bytes como máximo
        /// </summary>
        /// <remarks></remarks>
        public void ReadFile_Block(int sequence)
        {
            //Datos
            byte[] Datos = new byte[4];
            Datos = BitConverter.GetBytes(sequence);

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_READFILEBLOCK;

            SendMessage(Datos, Command);
        }

        /// <summary>
        /// Informa al equipo el fin de la lectura del archivo
        /// </summary>
        /// <remarks></remarks>
        public void ReadFile_End()
        {
            //Datos
            var Datos = new byte[] { };

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_READENDOFFILE;

            SendMessage(Datos, Command);
        }

        #endregion

        #region Write File 0x93 0x94 0x95

        /// <summary>
        /// Le indica al equipo que va a iniciar la escritura de un archivo
        /// </summary>
        /// <remarks></remarks>
        public void WriteFile_Start(string fileName, int dataLength)
        {
            //Datos
            const int maxLength = 12;
            fileName = fileName.Substring(0, maxLength);
            List<byte> Datos = new List<byte>();
            Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
            //For i = 1 To maxLength - Datos.Count
            //    Datos.Add(&H0) ' rellenar con nulos
            //Next
            Datos.AddRange(BitConverter.GetBytes(dataLength));

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_WRITESTARTFILE;

            SendMessage(Datos: Datos.ToArray(), command: Command, delayedResponse: true);
        }

        /// <summary>
        /// Envía al equipo un bloque de datos de 128 bytes como máximo
        /// </summary>
        /// <remarks></remarks>
        public void WriteFile_Block(int sequence, byte[] blockData)
        {
            //Datos
            const int maxLength = 128;
            List<byte> blockDataBytes = new List<byte>();
            blockDataBytes.AddRange(blockData);
            if (blockDataBytes.Count > maxLength)
            {
                blockDataBytes.RemoveRange(maxLength - 1, blockDataBytes.Count - maxLength);
            }
            List<byte> Datos = new List<byte>();
            Datos.AddRange(BitConverter.GetBytes(sequence));
            Datos.AddRange(blockDataBytes.ToArray());

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_WRITEFILEBLOCK;

            SendMessage(Datos: Datos.ToArray(), command: Command, delayedResponse: true);
        }

        /// <summary>
        /// Informa al equipo el fin de la escritura del archivo
        /// </summary>
        /// <remarks></remarks>
        public void WriteFile_End()
        {
            //Datos
            var Datos = new byte[] { };

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_WRITEENDOFFILE;

            SendMessage(Datos, Command);
        }

        #endregion

        #region Read File Count 0x96
        /// <summary>
        /// Le pide al equipo la cantidad de archivos que existen en la estación
        /// </summary>
        /// <remarks></remarks>
        public void ReadFileCount(string fileExtension)
        {
            //Datos
            const int maxLength = 3;
            fileExtension = fileExtension.Substring(0, maxLength);
            List<byte> Datos = new List<byte>();
            Datos.AddRange(Encoding.ASCII.GetBytes(fileExtension));

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_R_FILESCOUNT;

            SendMessage(Datos.ToArray(), Command);
        }
        #endregion

        #region Read File Name 0x97
        /// <summary>
        /// Le pide al equipo el nombre de un archivo de la estación
        /// </summary>
        /// <remarks></remarks>
        public void ReadFileName(int fileNumber)
        {
            //Datos
            List<byte> Datos = new List<byte>();
            Datos.AddRange(BitConverter.GetBytes(fileNumber));

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_R_GETFILENAME;

            SendMessage(Datos.ToArray(), Command);
        }
        #endregion

        #region Delete File 0x98
        /// <summary>
        /// Le indica al equipo el borrado de un archivo de la estación
        /// </summary>
        /// <remarks></remarks>
        public void DeleteFile(string fileName)
        {
            //Datos
            const int maxLength = 12;
            fileName = fileName.Substring(0, maxLength);
            List<byte> Datos = new List<byte>();
            Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
            //For i = 1 To maxLength - Datos.Count
            //    Datos.Add(&H0) ' rellenar con nulos
            //Next

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_DELETEFILE;

            SendMessage(Datos.ToArray(), Command);
        }
        #endregion

        #region Read Selected File 0x9A
        /// <summary>
        /// Le pide al equipo el nombre del archivo seleccionado en la estación
        /// </summary>
        /// <remarks></remarks>
        public void ReadSelectedFile()
        {
            //Datos
            var Datos = new byte[] { };

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_R_SELECTEDFILENAME;

            SendMessage(Datos, Command);
        }
        #endregion

        #region Write Selected File 0x9B
        /// <summary>
        /// Le pide al equipo que seleccione un archivo en la estación
        /// </summary>
        /// <remarks></remarks>
        public void WriteSelectedFile(string fileName)
        {
            //Datos
            const int maxLength = 12;
            fileName = fileName.Substring(0, maxLength);
            List<byte> Datos = new List<byte>();
            Datos.AddRange(Encoding.ASCII.GetBytes(fileName));
            //For i = 1 To maxLength - Datos.Count
            //    Datos.Add(&H0) ' rellenar con nulos
            //Next

            //Command
            byte Command = (byte)EnumCommandFrame_02_HA.M_W_SELECTFILE;

            SendMessage(Datos: Datos.ToArray(), command: Command, delayedResponse: true);
        }
        #endregion

        #endregion

        private void files_ListFiles(string ext)
        {
            ext = ext.ToUpper();
            filesCurrentOperation = FilesOperation.ListingFiles_Count;
            ReadFileCount(ext);
        }

        private void files_GetFile(string filename)
        {
            filename = filename.ToUpper();
            filesCurrentFilename = filename.ToUpper();
            filesCurrentOperation = FilesOperation.ReadingFile_Start;
            ReadFile_Start(filename);
        }

        private void files_SendFile(string pathfilename)
        {
            string filename = System.IO.Path.GetFileName(pathfilename);
            filename = filename.ToUpper();
            filesCurrentFilename = filename;
            filesCurrentFileData.Clear();
            // read data into Unicode string
            string stringData = System.IO.File.ReadAllText(pathfilename);
            // convert unicode to ASCII to send data
            filesCurrentFileData.AddRange(Encoding.ASCII.GetBytes(stringData));
            filesCurrentSize = System.Convert.ToInt32(filesCurrentFileData.Count);
            filesCurrentOperation = FilesOperation.WritingFile_Start;
            WriteFile_Start(filename, System.Convert.ToInt32(filesCurrentFileData.Count));
        }

        private void files_DeleteFile(string filename)
        {
            filename = filename.ToUpper();
            filesCurrentFilename = filename;
            filesCurrentOperation = FilesOperation.DeletingFile;
            DeleteFile(filename);
        }

        private void files_CurrentSelectedFile()
        {
            filesCurrentOperation = FilesOperation.SelectedFile;
            ReadSelectedFile();
        }

        private void files_SelectFile(string filename)
        {
            filename = filename.ToUpper();
            filesCurrentFilename = filename;
            filesCurrentOperation = FilesOperation.SelectingFile;
            WriteSelectedFile(filename);
        }

        public void butFiles_ListFiles_Click(object sender, EventArgs e)
        {
            string ext = tbFiles_Extension.Text;
            if (ext.Length == 0 | ext.Length > 3)
            {
                MessageBox.Show("Extension should be 3 chars.");
                return;
            }
            LogAdd("=== List Files ===");
            files_ListFiles(ext);
        }

        public void butFiles_GetFile_Click(object sender, EventArgs e)
        {
            string filename = "";
            if (lbFiles_List.Items.Count == 0 | lbFiles_List.SelectedIndex < 0)
            {
                filename = Interaction.InputBox("Manual: entre el nombre del archivo.", "Nombre del archivo");
                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
            }
            else
            {
                filename = System.Convert.ToString(lbFiles_List.Items[lbFiles_List.SelectedIndex]);
            }
            LogAdd("=== Get File === (" + filename + ")");
            files_GetFile(filename);
        }

        public void butFiles_SendFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog1.Filter = "Perfil (*.JPF)|*.JPF|All|*.*";
            if (string.IsNullOrEmpty(sFilesLastPathFile))
            {
                OpenFileDialog1.InitialDirectory = (new Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase()).Info.DirectoryPath;
                OpenFileDialog1.FileName = "";
            }
            else
            {
                OpenFileDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(sFilesLastPathFile);
                OpenFileDialog1.FileName = System.IO.Path.GetFileName(sFilesLastPathFile);
            }
            if (OpenFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (OpenFileDialog1.FileName.Trim() != "")
                {
                    string filename = System.IO.Path.GetFileName(OpenFileDialog1.FileName.Trim());
                    string fileext = System.Convert.ToString(System.IO.Path.GetExtension(OpenFileDialog1.FileName.Trim()).Replace(".", ""));
                    filename = filename.ToUpper();
                    if (filename.Length > 12)
                    {
                        MessageBox.Show("Selected file name should be 12 characters long (DOS name format 8.3)");
                    }
                    else if (fileext.Length == 0 | fileext.Length > 3)
                    {
                        MessageBox.Show("Selected file name extension should be 3 characters long");
                    }
                    else
                    {
                        LogAdd("=== Add File === (" + filename + ")");
                        sFilesLastPathFile = OpenFileDialog1.FileName;
                        files_SendFile(OpenFileDialog1.FileName);
                    }
                }
            }
        }

        public void butFiles_DeleteFile_Click(object sender, EventArgs e)
        {
            string filename = "";
            if (lbFiles_List.Items.Count == 0 | lbFiles_List.SelectedIndex < 0)
            {
                filename = Interaction.InputBox("Manual: entre el nombre del archivo.", "Nombre del archivo");
                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
            }
            else
            {
                filename = System.Convert.ToString(lbFiles_List.Items[lbFiles_List.SelectedIndex]);
            }
            LogAdd("=== Delete File === (" + filename + ")");
            files_DeleteFile(filename);
        }

        public void butFiles_SelectedFile_Click(object sender, EventArgs e)
        {
            LogAdd("=== Current File ===");
            files_CurrentSelectedFile();
        }

        public void butFiles_SelectFile_Click(object sender, EventArgs e)
        {
            string filename = "";
            if (lbFiles_List.Items.Count == 0 | lbFiles_List.SelectedIndex < 0)
            {
                filename = Interaction.InputBox("Manual: entre el nombre del archivo.", "Nombre del archivo");
                if (string.IsNullOrEmpty(filename))
                {
                    return;
                }
            }
            else
            {
                filename = System.Convert.ToString(lbFiles_List.Items[lbFiles_List.SelectedIndex]);
            }
            LogAdd("=== Select File === (" + filename + ")");
            files_SelectFile(filename);
        }

        public class StationFilesManager
        {

        }

        #endregion


    }
}
