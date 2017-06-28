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

using System.ServiceModel;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;
using JBC_ConnectRemote.JBCService;
using DataJBC;
using RoutinesJBC;
using SpeedContinuousMode = DataJBC.SpeedContinuousMode;
using Constants = DataJBC.Constants;
using OnOff = DataJBC.OnOff;

namespace JBC_ConnectRemote
{


    internal class CStation_Sold : CStation
    {

        private CContinuousModeQueueList traceList = new CContinuousModeQueueList();
        private SpeedContinuousMode startContModeSpeed = Constants.DEFAULT_STATION_CONTINUOUSMODE_SPEED;
        private byte startContModePorts = (byte) 0;

        internal new Remote_StackSold stack = null;


#region Station Public methods

        public CStation_Sold(string UUID, JBC_API_Remote APIreference, ref JBCStationControllerServiceClient hostService)
        {
            myAPI = APIreference;
            myUUID = UUID;
            remoteAddress = hostService.Endpoint.Address;

            //myServiceProtocol = _ServiceProtocol
            //_ServiceProtocol
            if (EnumProtocol.Protocol_01 == EnumProtocol.Protocol_01)
            {
                stack = new Remote_StackSold_01(ref hostService, UUID);
                stack.ConnectError += stack_ConnectError;
                stack.ReceivedInfoContiMode += stack_ReceivedInfoContiMode;
                ((Remote_StackSold_01) stack).StartStack(); // 26/11/2015
            }
            else
            {
                stack = new Remote_StackSold_01(ref hostService, UUID);
                stack.ConnectError += stack_ConnectError;
                stack.ReceivedInfoContiMode += stack_ReceivedInfoContiMode;
                ((Remote_StackSold_01) stack).StartStack(); // 26/11/2015
            }
        }

        public void Eraser()
        {

            //launching the event for the API class
            //myAPI.launchStationDisconnected(ID)

            //Station comunication lost. Releasing the stack memory
            stack.Eraser();

            //Self-destruction of the class instance
            // FALTA: no destruir objeto, cuando la API esté en formato NET
            // y si destruyo el objeto puedo provocar excepciones
            // Marcar como desconectado en la función launchStationDisconnected
            this.Finalize();
        }

        public int NumPorts
        {
            get
            {
                if (!stack.Equals(null))
                {
                    return stack.Info_Port.Length;
                }
                else
                {
                    return -1;
                }
            }
        }

        public CFeaturesData GetFeatures
        {
            get
            {
                if (!stack.Equals(null))
                {
                    return stack.Info_Station.Info.Features;
                }
                else
                {
                    return null;
                }
            }
        }

#endregion

#region Station Public Continuous mode

        public int UpdateContiModeNextData(uint queueID, int iChunk)
        {
            // get host data into local buffer
            return stack.UpdateContiModeNextData(queueID, iChunk);
        }

        public async Task<int> UpdateContiModeNextDataAsync(uint queueID, int iChunk)
        {
            // get host data into local buffer
            return await stack.UpdateContiModeNextDataAsync(queueID, iChunk);
        }

        public async Task SetContinuousModeAsync(SpeedContinuousMode speed, Port portA = default(Port), Port portB = default(Port), Port portC = default(Port), Port portD = default(Port))
        {
            //dataList.Clear()
            await stack.WriteContiModeAsync(speed, portA, portB, portC, portD);
            //stack.ReadContiMode()
        }

        public async Task<uint> StartContinuousModeAsync()
        {
            // start new queue in host
            uint uiNewTraceID = System.Convert.ToUInt32(await stack.StartContiModeAsync());
            // new local queue to receive data
            traceList.NewQueue(uiNewTraceID);
            // return New traceID
            return uiNewTraceID;
        }

        public async Task StopContinuousModeAsync(uint queueID)
        {
            // stop queue in host
            await stack.StopContiModeAsync(queueID);
            // delete local queue
            traceList.DeleteQueue(queueID);
        }

        public async Task<CContinuousModeStatus> GetContinuousModeAsync()
        {
            // get status from host
            await stack.ReadContiModeAsync();

            CContinuousModeStatus status = new CContinuousModeStatus();
            status.port1 = stack.Info_Station.Status.ContinuousModeStatus.port1;
            status.port2 = stack.Info_Station.Status.ContinuousModeStatus.port2;
            status.port3 = stack.Info_Station.Status.ContinuousModeStatus.port3;
            status.port4 = stack.Info_Station.Status.ContinuousModeStatus.port4;
            status.speed = stack.Info_Station.Status.ContinuousModeStatus.speed;
            return status;
        }

        public int GetContinuousModeDataCount(uint queueID)
        {
            //get local data count
            return traceList.DataCount(queueID);
        }

        public stContinuousModeData_SOLD GetContinuousModeNextData(uint queueID)
        {
            // get local data
            if (GetContinuousModeDataCount(queueID) > 0)
            {
                //Dim data As JBC_API_Remote.tContinuousModeData = dataList(0)
                //dataList.RemoveAt(0)
                stContinuousModeData_SOLD data = traceList.Queue(queueID).GetData();
                return data;
            }
            else
            {
                return new stContinuousModeData_SOLD();
            }
        }

#endregion

#region Station Public Orders

        public async Task SetDefaultStationParamsAsync()
        {
            await stack.SetDefaultStationParamsAsync();
        }

        public void ResetStation()
        {
            stack.DeviceResetAsync();
        }


#region Station Info

        // Leer del host datos de la estación
        public async Task<bool> UpdateStationInfoAsync()
        {
            return await stack.UpdateStationInfoAsync();
        }

        // leer del host información de la estación y devuelve una estructura
        public async Task<CStationInfoData> GetStationInfoAsync()
        {
            await UpdateStationInfoAsync();
            return stack.Info_Station.Info;
        }

        public eStationType GetStationType()
        {
            return stack.Info_Station.Info.StationType;
        }

        public string GetHostName()
        {
            return stack.hostName;
        }

        public string GetStationCom()
        {
            return stack.Info_Station.Info.COM;
        }

        public string GetStationConnectionType()
        {
            return stack.Info_Station.Info.ConnectionType;
        }

        public string GetStationProtocol()
        {
            return stack.Info_Station.Info.Protocol;
        }

        public string GetStationModel()
        {
            return stack.Info_Station.Info.Model;
        }

        public string GetStationModelType()
        {
            return stack.Info_Station.Info.ModelType;
        }

        public int GetStationModelVersion()
        {
            return stack.Info_Station.Info.ModelVersion;
        }

        public string GetStationHWversion()
        {
            return stack.Info_Station.Info.Version_Hardware;
        }

        public string GetStationSWversion()
        {
            return stack.Info_Station.Info.Version_Software;
        }

        public CFirmwareStation[] GetFirmwareVersion()
        {
            List<CFirmwareStation> firmwareVersion = new List<CFirmwareStation>();

            foreach (DictionaryEntry stationMicroEntry in stack.Info_Station.Info.StationMicros)
            {
                firmwareVersion.Add((CFirmwareStation) stationMicroEntry.Value);
            }

            return firmwareVersion.ToArray();
        }

        public GenericStationTools[] GetStationTools()
        {
            //getting the supported tools
            GenericStationTools[] tools = null;
            tools = new GenericStationTools[stack.Info_Port[0].ToolSettings.Length - 1 + 1];
            for (int cnt = 0; cnt <= stack.Info_Port[0].ToolSettings.Length - 1; cnt++)
            {
                if (stack.Info_Port[0].ToolSettings[cnt].Tool != GenericStationTools.NO_TOOL)
                {
                    tools[cnt] = stack.Info_Port[0].ToolSettings[cnt].Tool;
                }
                else
                {
                    Array.Resize(ref tools, tools.Length - 2 + 1);
                }
            }
            return tools;
        }

#endregion

#region Station Status

        // leer del host datos de estado de la estación
        public async Task<bool> UpdateStationStatusAsync()
        {
            return await stack.UpdateStationStatusAsync();
        }

        // leer del host datos de estado de la estación y devuelve una estructura
        //Public Function GetStationStatus() As JBC_ConnectRemote.cls_Station_Sold_Status
        //    UpdateStationStatus()
        //    Return stack.Info_Station.Status
        //End Function

        public ControlModeConnection GetControlMode()
        {
            return stack.Info_Station.Status.ControlMode;
        }

        public async void SetControlModeAsync(ControlModeConnection mode, string userName)
        {
            await stack.WriteConnectStatusAsync(mode, userName);
            stack.ReadConnectStatusAsync();
        }

        public string GetControlModeUserName()
        {
            return stack.Info_Station.Status.ControlModeUserName;
        }

        public void KeepControlMode()
        {
            stack.KeepControlMode();
        }

        public OnOff GetRemoteMode()
        {
            return OnOff._OFF;
            //Return CType(stack.Info_Station.RemoteMode, OnOff)
        }

        public async Task SetRemoteModeAsync(OnOff OnOff)
        {
            if (OnOff == OnOff._ON)
            {
                await stack.WriteRemoteModeAsync(OnOff._ON);
            }
            if (OnOff == OnOff._OFF)
            {
                await stack.WriteRemoteModeAsync(OnOff._OFF);
            }
            //stack.ReadRemoteMode()
        }

        public StationError GetStationError()
        {
            return stack.Info_Station.Status.ErrorStation;
        }

        public CTemperature GetStationTransformerTemp()
        {
            return new CTemperature(stack.Info_Station.Status.TempTRAFO.UTI);
        }

        public CTemperature GetStationTransformerErrorTemp()
        {
            return new CTemperature(stack.Info_Station.Status.TempErrorTRAFO.UTI);
        }

        public CTemperature GetStationMOSerrorTemp()
        {
            return (new CTemperature(stack.Info_Station.Status.TempErrorMOS.UTI));
        }
#endregion

#region Station Settings

        // leer del host configuración de la estación
        public async Task<bool> UpdateStationSettingsAsync()
        {
            return await stack.UpdateStationSettingsAsync();
        }

        public string GetStationName()
        {
            return stack.Info_Station.Settings.Name;
        }

        public async Task SetStationNameAsync(string newName)
        {
            await stack.WriteDeviceNameAsync(newName);
            //stack.ReadDeviceName()
        }

        public string GetStationPIN()
        {
            return stack.Info_Station.Settings.PIN;
        }

        public async Task SetStationPINAsync(string newPIN)
        {
            await stack.WriteDevicePINAsync(newPIN);
            //stack.ReadDevicePIN()
        }

        public CTemperature GetStationMaxTemp()
        {
            return new CTemperature(stack.Info_Station.Settings.MaxTemp.UTI);
        }

        public async Task SetStationMaxTempAsync(CTemperature temperature)
        {
            await stack.WriteMaxTempAsync(new CTemperature(temperature.UTI));
            //stack.ReadMaxTemp() ' ya lo hace el write
        }

        public CTemperature GetStationMinTemp()
        {
            return new CTemperature(stack.Info_Station.Settings.MinTemp.UTI);
        }

        public async Task SetStationMinTempAsync(CTemperature temperature)
        {
            await stack.WriteMinTempAsync(new CTemperature(temperature.UTI));
            //stack.ReadMinTemp() ' ya lo hace el write
        }

        public CTemperature.TemperatureUnit GetStationTempUnits()
        {
            return stack.Info_Station.Settings.Unit;
        }

        public async Task SetStationTempUnitsAsync(CTemperature.TemperatureUnit units)
        {
            await stack.WriteTempUnitAsync(units);
            //stack.ReadTempUnit()
        }

        public OnOff GetStationN2Mode()
        {
            return stack.Info_Station.Settings.N2Mode;
        }

        public async Task SetStationN2ModeAsync(OnOff mode)
        {
            await stack.WriteN2ModeAsync(mode);
            //stack.ReadN2Mode()
        }

        public OnOff GetStationHelpText()
        {
            return stack.Info_Station.Settings.HelpText;
        }

        public async Task SetStationHelpTextAsync(OnOff help)
        {
            await stack.WriteHelpTextAsync(help);
            //stack.ReadHelpText()
        }

        public int GetStationPowerLimit()
        {
            return stack.Info_Station.Settings.PowerLimit;
        }

        public async Task SetStationPowerLimitAsync(int powerLimit)
        {
            await stack.WritePowerLimitAsync(powerLimit);
            //stack.ReadPowerLimit()
        }

        public OnOff GetStationBeep()
        {
            return stack.Info_Station.Settings.Beep;
        }

        public async Task SetStationBeepAsync(OnOff beep)
        {
            await stack.WriteBeepAsync(beep);
            //stack.ReadBeep()
        }

        public async Task<uint> SetTransactionAsync()
        {
            return await stack.SetTransactionAsync();
        }

        public async Task<bool> QueryEndedTransactionAsync(uint transactionID)
        {
            return await stack.QueryEndedTransactionAsync(transactionID);
        }
#endregion

#region Port Status

        // leer del host estado del puerto/herramienta
        public async Task<bool> UpdatePortStatus(Port port)
        {
            return await stack.UpdatePortStatusAsync(port);
        }

        // leer del host estado del puerto/herramienta y devuelve una estructura
        //Public Function GetPortStatus(ByVal port As Port) As JBC_ConnectRemote.cls_StatusTool
        //    UpdatePortStatus(port)
        //    Return stack.Info_Port(port).ToolStatus
        //End Function

        public GenericStationTools GetPortToolID(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.ConnectedTool;
        }

        public CTemperature GetPortToolActualTemp(Port port)
        {
            return new CTemperature(stack.Info_Port[(int)port].ToolStatus.ActualTemp[0].UTI);
        }

        public int GetPortToolActualPower(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Power_x_Mil[0];
        }

        public ToolError GetPortToolError(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.ToolError;
        }

        public int GetPortToolCartridgeCurrent(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Current_mA[0];
        }

        public CTemperature GetPortToolMOStemp(Port port)
        {
            return new CTemperature(stack.Info_Port[(int)port].ToolStatus.Temp_MOS.UTI);
        }

        public ToolFutureMode GetPortToolFutureMode(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.FutureMode_Tool;
        }

        public int GetPortToolTimeToFutureMode(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.TimeToSleepHibern;
        }

        public OnOff GetPortToolStandStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Stand_OnOff;
        }

        public OnOff GetPortToolSleepStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Sleep_OnOff;
        }

        public OnOff GetPortToolHibernationStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Hiber_OnOff;
        }

        public OnOff GetPortToolExtractorStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Extractor_OnOff;
        }

        public OnOff GetPortToolDesolderStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Desold_OnOff;
        }

        public async Task SetPortToolStandStatusAsync(Port port, OnOff OnOff)
        {
            // 17/02/2014 Se modifica: si pido On, debo llamarlo con Extractor = off, porque es prioritario el Extractor y
            // y nunca se pondría en On el stand si he hecho un extractor On anterior
            if (OnOff == OnOff._ON)
            {
                await stack.WriteStatusToolAsync(port, OnOff._ON, OnOff._OFF, stack.Info_Port[(int)port].ToolStatus.Desold_OnOff);
            }
            if (OnOff == OnOff._OFF)
            {
                await stack.WriteStatusToolAsync(port, OnOff._OFF, stack.Info_Port[(int)port].ToolStatus.Extractor_OnOff, stack.Info_Port[(int)port].ToolStatus.Desold_OnOff);
            }
            //stack.ReadStatusTool(CType(port, Port))
        }

        public async Task SetPortToolExtractorStatusAsync(Port port, OnOff OnOff)
        {
            if (OnOff == OnOff._ON)
            {
                await stack.WriteStatusToolAsync(port, stack.Info_Port[(int)port].ToolStatus.Sleep_OnOff, OnOff._ON, stack.Info_Port[(int)port].ToolStatus.Desold_OnOff);
            }
            if (OnOff == OnOff._OFF)
            {
                await stack.WriteStatusToolAsync(port, stack.Info_Port[(int)port].ToolStatus.Sleep_OnOff, OnOff._OFF, stack.Info_Port[(int)port].ToolStatus.Desold_OnOff);
            }
            //stack.ReadStatusTool(CType(port, Port))
        }

        public async Task SetPortToolDesolderStatusAsync(Port port, OnOff OnOff)
        {
            if (OnOff == OnOff._ON)
            {
                await stack.WriteStatusToolAsync(port, stack.Info_Port[(int)port].ToolStatus.Sleep_OnOff, stack.Info_Port[(int)port].ToolStatus.Extractor_OnOff, OnOff._ON);
            }
            if (OnOff == OnOff._OFF)
            {
                await stack.WriteStatusToolAsync(port, stack.Info_Port[(int)port].ToolStatus.Sleep_OnOff, stack.Info_Port[(int)port].ToolStatus.Extractor_OnOff, OnOff._OFF);
            }
            //stack.ReadStatusTool(CType(port, Port))
        }
#endregion

#region Port+Tool

        // leer del host configuración del puerto+herramienta
        public async Task<bool> UpdatePortToolSettingsAsync(Port port, GenericStationTools tool)
        {
            return await stack.UpdatePortToolSettingsAsync(port, tool);
        }

        // leer del host configuración del puerto+herramienta y devuelve una estructura
        //Public Function GetPortToolSettings(ByVal port As Port, ByVal tool As GenericStationTools) As cls_ToolSettings
        //    UpdatePortToolSettings(port, tool)
        //    Return stack.Info_Port(port).ToolSettings(tool)
        //End Function

        public CTemperature GetPortToolSelectedTemp(Port port)
        {
            return new CTemperature(stack.Info_Port[(int)port].ToolStatus.SelectedTemp.UTI);
        }

        public async Task SetPortToolSelectedTempAsync(Port port, CTemperature temperature)
        {
            await stack.WriteSelectTempAsync(port, new CTemperature(temperature.UTI));
            //stack.ReadSelectTemp(CType(port, Port))
        }

        public CTemperature GetPortToolFixTemp(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return new CTemperature(stack.Info_Port[(int)port].ToolSettings[index].FixedTemp.UTI);
            }
            else
            {
                return new CTemperature(0);
            }
        }

        public async Task SetPortToolFixTempAsync(Port port, GenericStationTools tool, CTemperature temperature)
        {
            await stack.WriteFixTempAsync(port, tool, new CTemperature(temperature.UTI));
            //stack.ReadFixTemp(CType(port, Port), CType(tool, GenericStationTools))
        }

        public ToolTemperatureLevels GetPortToolSelectedTempLevels(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].Levels.LevelsTempSelect;
            }
            else
            {
                return ToolTemperatureLevels.NO_LEVELS;
            }
        }

        public OnOff GetPortToolSelectedTempLevelsEnabled(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].Levels.LevelsOnOff;
            }
            return OnOff._OFF;
        }

        public async Task SetPortToolSelectedTempLevelsAsync(Port port, GenericStationTools tool, ToolTemperatureLevels level)
        {
            await stack.WriteSelectedLevelAsync(port, tool, level);
        }

        public async Task SetPortToolSelectedTempLevelsEnabledAsync(Port port, GenericStationTools tool, OnOff onoff)
        {
            await stack.WriteSelectedLevelEnabledAsync(port, tool, onoff);
        }

        public CTemperature GetPortToolTempLevel(Port port, GenericStationTools tool, 
                ToolTemperatureLevels level)
        {
            int index = getToolIndex(tool);
            if (index > -1 && level != ToolTemperatureLevels.NO_LEVELS)
            {
                return new CTemperature(stack.Info_Port[(int)port].ToolSettings[index].Levels.LevelsTemp[(int)level].UTI);
            }
            else
            {
                return new CTemperature(0);
            }
        }

        public OnOff GetPortToolTempLevelEnabled(Port port, GenericStationTools tool, ToolTemperatureLevels level)
        {
            int index = getToolIndex(tool);
            if (index > -1 && level != ToolTemperatureLevels.NO_LEVELS)
            {
                return ((OnOff)(stack.Info_Port[(int)port].ToolSettings[index].Levels.LevelsTempOnOff[(int)level]));
            }
            return OnOff._OFF;
        }

        public async Task SetPortToolTempLevelAsync(Port port, GenericStationTools tool, ToolTemperatureLevels level, CTemperature temperature)
        {
            await stack.WriteLevelTempAsync(port, tool, level, new CTemperature(temperature.UTI));
        }

        public async Task SetPortToolTempLevelEnabledAsync(Port port, GenericStationTools tool, ToolTemperatureLevels level, OnOff onoff)
        {
            await stack.WriteLevelTempEnabledAsync(port, tool, level, onoff);
        }

        public async Task SetPortToolLevelsAsync(Port port, GenericStationTools tool, OnOff LevelsOnOff, ToolTemperatureLevels LevelSelected, OnOff Level1OnOff, CTemperature Level1Temp, OnOff Level2OnOff, CTemperature Level2Temp, OnOff Level3OnOff, CTemperature Level3Temp)
        {
            await stack.WriteLevelsTempsAsync(port, tool, LevelsOnOff, LevelSelected, Level1OnOff, new CTemperature(Level1Temp.UTI), Level2OnOff, new CTemperature(Level2Temp.UTI), Level3OnOff, new CTemperature(Level3Temp.UTI));
        }

        public ToolTimeSleep GetPortToolSleepDelay(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].SleepTime;
            }
            else
            {
                return ToolTimeSleep.NO_SLEEP;
            }
        }

        public OnOff GetPortToolSleepDelayEnabled(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].SleepTimeOnOff;
            }
            return OnOff._OFF;
        }

        public async Task SetPortToolSleepDelayAsync(Port port, GenericStationTools tool, ToolTimeSleep delay)
        {
            await stack.WriteSleepDelayAsync(port, tool, delay);
        }

        public async Task SetPortToolSleepDelayEnabledAsync(Port port, GenericStationTools tool, OnOff onoff)
        {
            await stack.WriteSleepDelayEnabledAsync(port, tool, onoff);
        }

        public CTemperature GetPortToolSleepTemp(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return new CTemperature(stack.Info_Port[(int)port].ToolSettings[index].SleepTemp.UTI);
            }
            else
            {
                return new CTemperature(0);
            }
        }

        public async Task SetPortToolSleepTempAsync(Port port, GenericStationTools tool, CTemperature temperature)
        {
            await stack.WriteSleepTempAsync(port, tool, new CTemperature(temperature.UTI));
        }

        public ToolTimeHibernation GetPortToolHibernationDelay(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].HiberTime;
            }
            else
            {
                return ToolTimeHibernation.NO_HIBERNATION;
            }
        }

        public OnOff GetPortToolHibernationDelayEnabled(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].HiberTimeOnOff;
            }
            else
            {
                return OnOff._OFF;
            }
        }

        public async Task SetPortToolHibernationDelayAsync(Port port, GenericStationTools tool, ToolTimeHibernation delay)
        {
            await stack.WriteHiberDelayAsync(port, tool, delay);
        }

        public async Task SetPortToolHibernationDelayEnabledAsync(Port port, GenericStationTools tool, OnOff onoff)
        {
            await stack.WriteHiberDelayEnabledAsync(port, tool, onoff);
        }

        public CTemperature GetPortToolAdjustTemp(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return new CTemperature(stack.Info_Port[(int)port].ToolSettings[index].AdjustTemp.UTI);
            }
            else
            {
                return new CTemperature(0);
            }
        }

        public async Task SetPortToolAdjustTempAsync(Port port, GenericStationTools tool, CTemperature temperature)
        {
            await stack.WriteAjustTempAsync(port, tool, new CTemperature(temperature.UTI));
        }

        public ushort GetPortToolCartridge(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].Cartridge.CartridgeNbr;
            }
            else
            {
                return System.Convert.ToUInt16(ToolTemperatureLevels.NO_LEVELS);
            }
        }

        public OnOff GetPortToolCartridgeEnabled(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].Cartridge.CartridgeOnOff;
            }
            else
            {
                return OnOff._OFF;
            }
        }

        public async Task SetPortToolCartridgeAsync(Port port, GenericStationTools tool, CCartridgeData cartridge)
        {
            await stack.WriteCartridgeAsync(port, tool, cartridge);
        }

#endregion

#region Counters

        // leer del host los datos de contadores del puerto
        public async Task<bool> UpdatePortCountersAsync(Port port)
        {
            return await stack.UpdatePortCountersAsync(port);
        }

        // Global counters

        public int GetStationPluggedMinutes()
        {
            return stack.Info_Port[0].Counters.ContPlugMinutes;
        }

        public int GetPortToolPluggedMinutes(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContPlugMinutes;
        }

        public int GetPortToolWorkMinutes(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContWorkMinutes;
        }

        public int GetPortToolSleepMinutes(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContSleepMinutes;
        }

        public int GetPortToolHibernationMinutes(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContHiberMinutes;
        }

        public int GetPortToolIdleMinutes(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContIdleMinutes;
        }

        public int GetPortToolSleepCycles(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContSleepCycles;
        }

        public int GetPortToolDesolderCycles(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContDesoldCycles;
        }

        // Partial counters

        public int GetStationPluggedMinutesPartial()
        {
            return stack.Info_Port[0].PartialCounters.ContPlugMinutes;
        }

        public int GetPortToolPluggedMinutesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContPlugMinutes;
        }

        public int GetPortToolWorkMinutesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContWorkMinutes;
        }

        public int GetPortToolSleepMinutesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContSleepMinutes;
        }

        public int GetPortToolHibernationMinutesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContHiberMinutes;
        }

        public int GetPortToolIdleMinutesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContIdleMinutes;
        }

        public int GetPortToolSleepCyclesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContSleepCycles;
        }

        public int GetPortToolDesolderCyclesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContDesoldCycles;
        }

        public void ResetPortToolMinutesPartial(Port port)
        {
            stack.ResetPortToolMinutesPartialAsync(port);
        }

#endregion

#region Communication

        public async Task<bool> UpdateEthernetConfigurationAsync()
        {
            return await stack.UpdateEthernetConfigurationAsync();
        }

        public CEthernetData GetEthernetConfiguration()
        {
            return stack.Info_Station.Settings.Ethernet;
        }

        public async Task SetEthernetConfigurationAsync(CEthernetData ethernetData)
        {
            await stack.WriteEthernetConfigurationAsync(ethernetData);
        }

        public async Task<bool> UpdateRobotConfigurationAsync()
        {
            return await stack.UpdateRobotConfigurationAsync();
        }

        public CRobotData GetRobotConfiguration()
        {
            return stack.Info_Station.Settings.Robot;
        }

        public async Task SetRobotConfigurationAsync(CRobotData robotData)
        {
            await stack.WriteRobotConfigurationAsync(robotData);
        }

#endregion

#region Peripheral

        // leer del host la configuración de los periféricos
        public async Task<bool> UpdatePeripheralAsync()
        {
            return await stack.UpdateAllPeripheralAsync();
        }

        public CPeripheralData[] GetPeripherals()
        {
            return stack.Info_Peripheral.ToArray();
        }

        public CPeripheralData[] GetPortPeripheral(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Peripherals;
        }

        public async Task SetPeripheralAsync(CPeripheralData peripheralData)
        {
            await stack.WritePeripheralAsync(peripheralData);
        }

#endregion

#region Update Firmware

        public void UpdateStationsFirmware(List<CFirmwareStation> stationList)
        {
            stack.UpdateStationsFirmware(stationList);
        }

        public List<string> GetStationListUpdating()
        {
            return stack.GetStationListUpdating();
        }

#endregion

#endregion

#region Station Private events

        private void stack_ConnectError(Remote_StackSold.EnumConnectError ErrorType, string sMsg, string sStackFunction, dc_EnumConstJBCdc_FaultError serviceErrorCode, string serviceOperation)
        {

            Debug.Print("ConnectError en API stack station {0} en host {1} (ejecuta launchStationDisconnected + myAPI.StationSearcherTCP.StationDisconnected + stack.Eraser)", stack.Info_Station.Settings.Name, stack.hostName);

            if (ErrorType == Remote_StackHA.EnumConnectError.WCF_STACK)
            {
                myAPI.launchUserError(UUID, new Cerror(Cerror.cErrorCodes.COMMUNICATION_ERROR, "Communication error: '" + sMsg + "' (function: " + sStackFunction + ").", new byte[1] {0}));
                //cierra las demas estaciones del host
                // 26/11/2015 No
                //myAPI.launchStationComError(stack.Info_Station.hostEndPointAddress)

            }
            else if (ErrorType == Remote_StackHA.EnumConnectError.WCF_SERVICE)
            {
                if (serviceErrorCode == dc_EnumConstJBCdc_FaultError.NotControlledError | serviceErrorCode == dc_EnumConstJBCdc_FaultError.StationNotFound)
                {
                    // 26/11/2015
                    //myAPI.ServicesSearcher.StationDisconnected(stack.Info_Station.hostEndPointAddress, stack.Info_Station.hostStnID)
                    //myAPI.launchStationDisconnected(ID)
                    //Eraser()
                }
            }

            myAPI.launchStationDisconnected(UUID);
            //Eraser()

        }

        private void stack_ReceivedInfoContiMode(stContinuousModeData_SOLD Datos, uint queueID)
        {
            // añadir a buffer local
            traceList.AddData(Datos, queueID);
        }

        //Private Sub stack_TransactionFinished(ByVal transactionID As UInteger) Handles stack.TransactionFinished
        //    'Station received a TransactionFinish event, implemented with M_ACK frame
        //    If (transactionID > 0) Then

        //        'launching the event for the API class
        //        myAPI.launchTransactionFinished(ID, transactionID)
        //    End If
        //End Sub
#endregion

#region Station Private methods

        private int getToolIndex(GenericStationTools tool)
        {
            //looking for the tool in the tool param vector
            int cnt = 0;
            bool found = false;
            while (cnt < stack.Info_Port[0].ToolSettings.Length && !found)
            {
                if (stack.Info_Port[0].ToolSettings[cnt].Tool == tool)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            if (found)
            {
                return cnt;
            }
            return -1;
        }

#endregion

    }

}
