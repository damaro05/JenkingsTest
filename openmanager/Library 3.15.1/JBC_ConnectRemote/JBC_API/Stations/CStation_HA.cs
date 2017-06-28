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
using OnOff = DataJBC.OnOff;
using SpeedContinuousMode = DataJBC.SpeedContinuousMode;




namespace JBC_ConnectRemote
{


    internal class CStation_HA : CStation
    {

        private CContinuousModeQueueList_HA traceList = new CContinuousModeQueueList_HA();
        private SpeedContinuousMode startContModeSpeed = DataJBC.Constants.DEFAULT_STATION_CONTINUOUSMODE_SPEED;
        private byte startContModePorts = (byte) 0;

        internal new Remote_StackHA stack = null;

        public static bool CheckIsModelType(string sModelStation)
        {
            try
            {
                CStationsConfiguration stationConfig = new CStationsConfiguration(sModelStation);
                return stationConfig.StationType == eStationType.HA;
            }
            catch (Exception)
            {
                return false;
            }
        }


#region Station Public methods

        public CStation_HA(string UUID,
                JBC_API_Remote APIreference,
                ref JBCStationControllerServiceClient hostService)
        {
            myAPI = APIreference;
            myUUID = UUID;
            remoteAddress = hostService.Endpoint.Address;

            //myServiceProtocol = _ServiceProtocol
            //_ServiceProtocol
            if (EnumProtocol.Protocol_01 == EnumProtocol.Protocol_01)
            {
                stack = new Remote_StackHA_01(ref hostService, UUID);
                stack.ConnectError += stack_ConnectError;
                stack.ReceivedInfoContiMode += stack_ReceivedInfoContiMode;
                ((Remote_StackHA_01) stack).StartStack(); // 26/11/2015
            }
            else
            {
                stack = new Remote_StackHA_01(ref hostService, UUID);
                stack.ConnectError += stack_ConnectError;
                stack.ReceivedInfoContiMode += stack_ReceivedInfoContiMode;
                ((Remote_StackHA_01) stack).StartStack(); // 26/11/2015
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

        public stContinuousModeData_HA GetContinuousModeNextData(uint queueID)
        {
            // get local data
            if (GetContinuousModeDataCount(queueID) > 0)
            {
                //Dim data As JBC_API_Remote.tContinuousModeData_HA = dataList(0)
                //dataList.RemoveAt(0)
                stContinuousModeData_HA data = traceList.Queue(queueID).GetData();
                return data;
            }
            else
            {
                return new stContinuousModeData_HA();
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


#region Station Info :fet:

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

#region Station Status :fet:

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

#endregion

#region Station Settings :fet:

        // leer del host configuración de la estación
        public async Task<bool> UpdateStationSettingsAsync()
        {
            return await stack.UpdateStationSettingsAsync();
        }

        // leer del host configuración de la estación y devuelve una estructura
        //Public Function GetStationSettings() As JBC_ConnectRemote.cls_Station_Sold_Settings
        //    UpdateStationSettings()
        //    Return stack.Info_Station.Settings
        //End Function

        // name
        public string GetStationName()
        {
            return stack.Info_Station.Settings.Name;
        }

        public async Task SetStationNameAsync(string newName)
        {
            await stack.WriteDeviceNameAsync(newName);
            //stack.ReadDeviceName()
        }

        // PIN
        public string GetStationPIN()
        {
            return stack.Info_Station.Settings.PIN;
        }

        public async Task SetStationPINAsync(string newPIN)
        {
            await stack.WriteDevicePINAsync(newPIN);
            //stack.ReadDevicePIN()
        }

        // max temp
        public CTemperature GetStationMaxTemp()
        {
            return new CTemperature(stack.Info_Station.Settings.MaxTemp.UTI);
        }

        public async Task SetStationMaxTempAsync(CTemperature temperature)
        {
            await stack.WriteMaxTempAsync(new CTemperature(temperature.UTI));
            //stack.ReadMaxTemp() ' ya lo hace el write
        }

        // min temp
        public CTemperature GetStationMinTemp()
        {
            return new CTemperature(stack.Info_Station.Settings.MinTemp.UTI);
        }

        public async Task SetStationMinTempAsync(CTemperature temperature)
        {
            await stack.WriteMinTempAsync(new CTemperature(temperature.UTI));
            //stack.ReadMinTemp() ' ya lo hace el write
        }

        // max ext temp
        public CTemperature GetStationMaxExtTemp()
        {
            return new CTemperature(stack.Info_Station.Settings.MaxExtTemp.UTI);
        }

        public async Task SetStationMaxExtTempAsync(CTemperature temperature)
        {
            await stack.WriteMaxExtTempAsync(new CTemperature(temperature.UTI));
        }

        // min ext temp
        public CTemperature GetStationMinExtTemp()
        {
            return new CTemperature(stack.Info_Station.Settings.MinExtTemp.UTI);
        }

        public async Task SetStationMinExtTempAsync(CTemperature temperature)
        {
            await stack.WriteMinExtTempAsync(new CTemperature(temperature.UTI));
        }

        // max flow
        public int GetStationMaxFlow()
        {
            return stack.Info_Station.Settings.MaxFlow;
        }

        public async Task SetStationMaxFlowAsync(int flow)
        {
            await stack.WriteMaxFlowAsync(flow);
        }

        // min flow
        public int GetStationMinFlow()
        {
            return stack.Info_Station.Settings.MinFlow;
        }

        public async Task SetStationMinFlowAsync(int flow)
        {
            await stack.WriteMinFlowAsync(flow);
        }

        // temp units
        public CTemperature.TemperatureUnit GetStationTempUnits()
        {
            return stack.Info_Station.Settings.Unit;
        }

        public async Task SetStationTempUnitsAsync(CTemperature.TemperatureUnit units)
        {
            await stack.WriteTempUnitAsync(units);
            //stack.ReadTempUnit()
        }

        // beep
        public OnOff GetStationBeep()
        {
            return stack.Info_Station.Settings.Beep;
        }

        public async Task SetStationBeepAsync(OnOff beep)
        {
            await stack.WriteBeepAsync(beep);
            //stack.ReadBeep()
        }

        // PIN enabled
        public OnOff GetStationPINEnabled()
        {
            return stack.Info_Station.Settings.PINEnabled;
        }

        public async Task SetStationPINEnabledAsync(OnOff value)
        {
            await stack.WritePINEnabledAsync(value);
        }

        // Station locked
        public OnOff GetStationLocked()
        {
            return stack.Info_Station.Settings.StationLocked;
        }

        public async Task SetStationLockedAsync(OnOff value)
        {
            await stack.WriteStationLockedAsync(value);
        }

        // transaction
        public async Task<uint> SetTransactionAsync()
        {
            return await stack.SetTransactionAsync();
        }

        public async Task<bool> QueryEndedTransactionAsync(uint transactionID)
        {
            return await stack.QueryEndedTransactionAsync(transactionID);
        }
#endregion

#region Port Status :fet:

        // leer del host estado del puerto/herramienta
        public async Task<bool> UpdatePortStatus(Port port)
        {
            return await stack.UpdatePortStatusAsync(port);
        }

        // leer del host estado del puerto/herramienta y devuelve una estructura
        //Public Function GetPortStatus(ByVal port As Port) As JBC_ConnectRemote.CStatusToolData_HA
        //    UpdatePortStatus(port)
        //    Return stack.Info_Port(port).ToolStatus
        //End Function

        public GenericStationTools GetPortToolID(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.ConnectedTool;
        }

        public CTemperature GetPortToolActualTemp(Port port)
        {
            return new CTemperature(stack.Info_Port[(int)port].ToolStatus.ActualTemp.UTI);
        }

        public CTemperature GetPortToolActualExtTemp(Port port)
        {
            return new CTemperature(stack.Info_Port[(int)port].ToolStatus.ActualExtTemp.UTI);
        }

        public CTemperature GetPortToolProtectionTCTemp(Port port)
        {
            return new CTemperature(stack.Info_Port[(int)port].ToolStatus.ProtectionTC_Temp.UTI);
        }

        public int GetPortToolActualPower(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Power_x_Mil;
        }

        public int GetPortToolActualFlow(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Flow_x_Mil;
        }

        public ToolError GetPortToolError(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.ToolError;
        }

        public int GetPortToolTimeToStop(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.TimeToStop;
        }

        public OnOff GetPortToolStandStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.Stand_OnOff;
        }

        public OnOff GetPortToolPedalStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.PedalStatus_OnOff;
        }

        public OnOff GetPortToolPedalConnected(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.PedalConnected_OnOff;
        }

        public OnOff GetPortToolHeaterRequestedStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.HeaterRequestedStatus_OnOff;
        }

        public OnOff GetPortToolHeaterStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.HeaterStatus_OnOff;
        }

        public OnOff GetPortToolSuctionRequestedStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.SuctionRequestedStatus_OnOff;
        }

        public OnOff GetPortToolSuctionStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.SuctionStatus_OnOff;
        }

        public OnOff GetPortToolCoolingStatus(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.CoolingStatus_OnOff;
        }

        public async Task SetPortToolHeaterStatusAsync(Port port, OnOff OnOff)
        {
            await stack.WriteStatusToolAsync(port, OnOff, stack.Info_Port[(int)port].ToolStatus.SuctionStatus_OnOff);
        }

        public async Task SetPortToolSuctionStatusAsync(Port port, OnOff OnOff)
        {
            await stack.WriteStatusToolAsync(port, stack.Info_Port[(int)port].ToolStatus.HeaterStatus_OnOff, OnOff);
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
        }

        public CTemperature GetPortToolSelectedExtTemp(Port port)
        {
            return new CTemperature(stack.Info_Port[(int)port].ToolStatus.SelectedExtTemp.UTI);
        }

        public async Task SetPortToolSelectedExtTempAsync(Port port, CTemperature temperature)
        {
            await stack.WriteSelectExtTempAsync(port, new CTemperature(temperature.UTI));
        }

        public int GetPortToolSelectedFlow(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.SelectedFlow_x_Mil;
        }

        public async Task SetPortToolSelectedFlowAsync(Port port, int flow)
        {
            await stack.WriteSelectFlowAsync(port, flow);
        }

        public OnOff GetPortToolProfileMode(Port port)
        {
            return stack.Info_Port[(int)port].ToolStatus.ProfileMode;
        }

        public async Task SetPortToolProfileMode(Port port, OnOff onoff)
        {
            await stack.WriteProfileModeAsync(port, onoff);
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

        public async Task SetPortToolSelectedTempLevelsAsync(Port port, GenericStationTools tool, 
                ToolTemperatureLevels level)
        {
            await stack.WriteSelectedLevelAsync(port, tool, level);
        }

        public async Task SetPortToolSelectedTempLevelsEnabledAsync(Port port, GenericStationTools tool, OnOff onoff)
        {
            await stack.WriteSelectedLevelEnabledAsync(port, tool, onoff);
        }

        public CTemperature GetPortToolTempLevel(Port port, GenericStationTools tool, ToolTemperatureLevels level)
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
                return ((OnOff) (stack.Info_Port[(int)port].ToolSettings[index].Levels.LevelsTempOnOff[(int)level]));
            }
            return OnOff._OFF;
        }

        public async Task SetPortToolTempLevelAsync(Port port, GenericStationTools tool, 
                ToolTemperatureLevels level, CTemperature temperature)
        {
            await stack.WriteLevelTempAsync(port, tool, level, new CTemperature(temperature.UTI));
        }

        public async Task SetPortToolTempLevelEnabledAsync(Port port, GenericStationTools tool, ToolTemperatureLevels level, OnOff onoff)
        {
            await stack.WriteLevelTempEnabledAsync(port, tool, level, onoff);
        }

        public async Task SetPortToolLevelsAsync(Port port, GenericStationTools tool, OnOff LevelsOnOff, ToolTemperatureLevels LevelSelected, OnOff Level1OnOff, CTemperature Level1Temp, int Level1Flow, CTemperature Level1ExtTemp, OnOff Level2OnOff, CTemperature Level2Temp, int Level2Flow, CTemperature Level2ExtTemp, OnOff Level3OnOff, CTemperature Level3Temp, int Level3Flow, CTemperature Level3ExtTemp)
        {
            await stack.WriteLevelsTempsAsync(port, tool, LevelsOnOff, LevelSelected, Level1OnOff, new CTemperature(Level1Temp.UTI), Level1Flow, new CTemperature(Level1ExtTemp.UTI), Level2OnOff, new CTemperature(Level2Temp.UTI), Level2Flow, new CTemperature(Level2ExtTemp.UTI), Level3OnOff, new CTemperature(Level3Temp.UTI), Level3Flow, new CTemperature(Level3ExtTemp.UTI));
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

        public int GetPortToolTimeToStop(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].TimeToStop;
            }
            else
            {
                return 0;
            }
        }

        public async Task SetPortToolTimeToStopAsync(Port port, GenericStationTools tool, int value)
        {
            await stack.WriteTimeToStopAsync(port, tool, value);
        }

        public ToolExternalTCMode_HA GetPortToolExternalTCMode(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index != -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].ExternalTCMode;
            }
            else
            {
                return 0;
            }
        }

        public async Task SetPortToolExternalTCModeAsync(Port port, GenericStationTools tool, ToolExternalTCMode_HA mode)
        {
            await stack.WriteExternalTCModeAsync(port, tool, mode);
        }

        public OnOff GetPortToolStartMode_ToolButton(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index > -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].StartMode_ToolButton;
            }
            else
            {
                return OnOff._OFF;
            }
        }

        public PedalAction GetPortToolStartMode_PedalAction(Port port, GenericStationTools tool)
        {
            int index = getToolIndex(tool);
            if (index > -1)
            {
                return stack.Info_Port[(int)port].ToolSettings[index].StartMode_Pedal;
            }
            else
            {
                return PedalAction.NONE;
            }
        }

        public async Task SetPortToolStartModeAsync(Port port, GenericStationTools tool, OnOff toolButton, PedalAction pedalAction)
        {
            await stack.WriteStartModeAsync(port, tool, toolButton, pedalAction);
        }

#endregion

#region Counters

        // leer del host los datos de contadores del puerto
        public async Task<bool> UpdatePortCountersAsync(Port port)
        {
            return await stack.UpdatePortCountersAsync(port);
        }

        // leer del host los datos de contadores del puerto y devolver una estructura
        //Public Function GetPortCounters(ByVal port As Port, ByVal counttype As cls_EnumConstJBC.EnumCounterType) As cls_Counters
        //    UpdatePortCounters(port)
        //    If counttype = cls_EnumConstJBC.EnumCounterType.counterGlobal Then
        //        Return stack.Info_Port(port).Counters
        //    Else
        //        Return stack.Info_Port(port).PartialCounters
        //    End If
        //End Function

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

        public int GetPortToolWorkCycles(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContWorkCycles;
        }

        public int GetPortToolSuctionCycles(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContSuctionCycles;
        }

        public int GetPortToolIdleMinutes(Port port)
        {
            return stack.Info_Port[(int)port].Counters.ContIdleMinutes;
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

        public int GetPortToolWorkCyclesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContWorkCycles;
        }

        public int GetPortToolSuctionCyclesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContSuctionCycles;
        }

        public int GetPortToolIdleMinutesPartial(Port port)
        {
            return stack.Info_Port[(int)port].PartialCounters.ContIdleMinutes;
        }

        public void ResetPortToolMinutesPartial(Port port)
        {
            stack.ResetPortToolMinutesPartialAsync(port);
        }

#endregion

#region Communication

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

#region Profile

        public async Task<bool> UpdateProfilesAsync()
        {
            return await stack.UpdateProfilesAsync();
        }

        public async Task<bool> UpdateSelectedProfileAsync()
        {
            return await stack.UpdateSelectedProfileAsync();
        }

        public int GetProfileCount()
        {
            return stack.Info_Station.Settings.Profiles.Count;
        }

        public byte[] GetProfile(string profileName)
        {
            foreach (CProfileData_HA profile in stack.Info_Station.Settings.Profiles)
            {
                if (profile.Name == profileName)
                {
                    return profile.Data;
                }
            }

            return null;
        }

        public async Task<bool> SetProfileAsync(string profileName, byte[] profileData)
        {
            return await stack.SetProfileAsync(profileName, profileData);
        }

        public string GetSelectedProfile()
        {
            return stack.Info_Station.Settings.SelectedProfile;
        }

        public string GetProfileName(int profileIndex)
        {
            if (profileIndex < stack.Info_Station.Settings.Profiles.Count)
            {
                return stack.Info_Station.Settings.Profiles[profileIndex].Name;
            }

            return null;
        }

        public async Task DeleteProfile(string profileName)
        {
            await stack.DeleteProfile(profileName);
        }

        public async Task SyncProfiles()
        {
            await stack.SyncProfiles();
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

        private void stack_ReceivedInfoContiMode(stContinuousModeData_HA Datos, uint queueID)
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
