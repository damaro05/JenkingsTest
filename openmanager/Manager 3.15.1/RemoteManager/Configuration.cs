// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports

//using JBC_API_Remote;
using Microsoft.VisualBasic.CompilerServices;
using JBC_ConnectRemote;
using DataJBC;
using Constants = DataJBC.Constants;


// 2013/04/15 En archivos xml, se quita el atributo "Number" de "Tool"
// 2013/04/16 En archivos xml, se cambia el atributo "DateTime" por 2 atributos "Date" y "Time"
// 10/12/2013 Se quitan datos del xml: MOS Error Temp y Trafo Error Temp
// 20/12/2013 Se modifican los límites de temperatura máximos, según el modelo de estación
// 13/01/2014 Se quita SetStationPowerLimit de la API y del Manager (se mantiene en el protocolo) node: stnPwrLimitId


namespace RemoteManager
{
    sealed class Configuration
    {
        // estructuras públicas

        public struct tStationDataItemList
        {
            public long ID;
            public bool bControlMode;
        }

        public struct strucStationData
        {
            public long ID;
            public eStationType eStationType;
            public bool bControlMode;
            public string sName;
            public string sModel;
            public string sSW;
            public string sStationCOM;
            public bool bStationNamed;
        }

        // colors
        public static Color stnColorBackground = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(206)), System.Convert.ToInt32(System.Convert.ToByte(210)), System.Convert.ToInt32(System.Convert.ToByte(213)));
        public static Color stnColorText = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
        public static Color stnColorTextSel = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));

        // Variables that end width "Id", refer to a resource string id
        // Some are used also as tags in xml files (export/import station configuration)

        // GENERAL --------------------------------------------
        public static int MAX_PORTS = 4;
        public static string CELSIUS_STR = Strings.Chr(176).ToString() + "C";
        public static string FAHRENHEIT_STR = Strings.Chr(176).ToString() + "F";
        public static string Tunits = Strings.Chr(176).ToString() + "C";

        public static string sTempPath = System.IO.Path.GetTempPath();
        public static string sTempPathSubdir = "JBC\\";
        public static string sDatetimeForFilesFormat = "yyyyMMdd_HHmmss";

        public static string ON_STRId = "ON_STR";
        public static string OFF_STRId = "OFF_STR";
        public static string pwrUnitsStr = "W";
        public static string pwrUnitsPercentStr = "%";
        public static string hidePINtext = "****";
        public static string invalidTempStr = "---";
        public static string noDataStr = "---";
        public static string sReplaceTag = "{0}"; // for replacing values in strings (used in ParamTable, for example)
        public static string flowUnitsStr = "%";

        public static string curViewTempUnits = "";
        public static bool curShowErrorNotifications = true;
        public static bool curShowStationControllerNotifications = false;
        public static bool curShowStationNotifications = false;

        public static CTemperature tempMin = new CTemperature((System.Int32)TemperatureLimits.MIN_TEMP);
        public static CTemperature tempMax = new CTemperature((System.Int32)TemperatureLimits.MAX_TEMP);
        public static CTemperature tempMaxHD = new CTemperature((System.Int32)TemperatureLimits.MAX_TEMP_HD);
        public static CTemperature tempMinAdj = new CTemperature(50 * 9 * -1);
        public static CTemperature tempMaxAdj = new CTemperature(50 * 9);

        public static string gralUnderConstructionId = "gralUnderConstruction";
        public static string gralDoneId = "gralDone";
        public static string gralWarningId = "gralWarning";
        public static string gralNoNameId = "gralNoName";
        public static string gralValueSeparatorId = "gralValueSeparator";
        public static string gralHoursMinutesId = "gralHoursMinutes";
        public static string gralMinutesId = "gralMinutes";
        public static string gralNewGroupId = "gralNewGroup";
        public static string gralStationNoNamedId = "StationNoNamed";
        public static string gralStationNamedId = "StationNamed";
        public static string gralEnabledId = "gralEnabled";
        public static string gralOkId = "gralOk";
        public static string gralCancelId = "gralCancel";

        public static string iconGroupListId = "TreeRoot";
        public static string iconGroupId = "Folder";
        public static string iconStationConnId = "Station_Conn";
        public static string iconStationDisconnId = "Station_Disconn";

        internal static Color colorRoot = Color.Blue;
        internal static Color colorGroup = Color.Gray;
        internal static Color colorStationDisconnected = Color.Firebrick;
        internal static Color colorStationConnected = Color.DarkBlue;

        // FORM MAIN -----------------------------------------
        public static string mnuFileId = "mnuFile";

        public static string mnuEditId = "mnuEdit";

        public static string mnuSupervisorId = "mnuSupervisor";
        public static string mnuLoginSupervisorId = "mnuLoginSupervisor";
        public static string mnuLogoutSupervisorId = "mnuLogoutSupervisor";
        public static string mnuChangePasswordId = "mnuChangePassword";
        public static string supervEnterPasswordId = "supervEnterPassword";
        public static string supervPasswordId = "supervPassword";
        public static string supervMustLoginId = "supervMustLogin";
        public static string supervInvalidPasswordId = "supervInvalidPassword";
        public static string ctrlEnterPINCodeId = "ctrlEnterPINCode";
        public static string ctrlPINCodeId = "ctrlPINCode";
        public static bool bSupervisorLoggedIn = false;

        public static string mnuViewId = "mnuView";
        public static string mnuViewStationListId = "mnuViewStationList";
        public static string mnuViewEventsWindowId = "mnuViewEventsWindow";
        public static string mnuViewWarningId = "mnuViewWarning";

        public static string mnuTempUnitId = "mnuTempUnit";

        public static string mnuToolsId = "mnuTools";
        public static string mnuToolsSettingsManagerId = "mnuToolsSettingsManager";
        public static string mnuToolsRegisterManagerId = "mnuToolsRegisterManager";

        public static string mnuWindowsId = "mnuWindows";
        public static string mnuCascadeId = "mnuCascade";
        public static string mnuCascadeAllId = "mnuCascadeAll";

        public static string mnuHelpId = "mnuHelp";
        public static string mnuUpdatesId = "mnuUpdates";
        public static string mnuAboutId = "mnuAbout";

        public static string gralMinuteId = "gralMinute";
        public static string gralHourId = "gralHour";
        public static string gralDailyId = "gralDaily";
        public static string gralWeeklyId = "gralWeekly";
        public static string gralMondayId = "gralMonday";
        public static string gralTuesdayId = "gralTuesday";
        public static string gralWednesdayId = "gralWednesday";
        public static string gralThursdayId = "gralThursday";
        public static string gralFridayId = "gralFriday";
        public static string gralSaturdayId = "gralSaturday";
        public static string gralSundayId = "gralSunday";

        //Host Controller
        public static string hostControllerConnectedId = "hostControllerConnected";
        public static string hostControllerMultipleConnectedId = "hostControllerMultipleConnected";
        public static string hostControllerNoConnectedId = "hostControllerNoConnected";

        //Update
        public static string updatesNotificationState_InitUpdateProcessTitleId = "updatesNotificationState_InitUpdateProcessTitle";
        public static string updatesNotificationState_InitUpdateProcessExplanationId = "updatesNotificationState_InitUpdateProcessExplanation";
        public static string updatesNotificationState_NoConnectionTitleId = "updatesNotificationState_NoConnectionTitle";
        public static string updatesNotificationState_NoConnectionExplanationId = "updatesNotificationState_NoConnectionExplanation";
        public static string updatesNotificationState_NoConnectionReconnectId = "updatesNotificationState_NoConnectionReconnect";
        public static string updatesNotificationState_OkTitleId = "updatesNotificationState_OkTitle";
        public static string updatesNotificationState_OkExplanationId = "updatesNotificationState_OkExplanation";
        public static string updatesNotificationState_WarningTitleId = "updatesNotificationState_WarningTitle";
        public static string updatesNotificationState_WarningStationControllerVersionId = "updatesNotificationState_WarningStationControllerVersion";
        public static string updatesNotificationState_WarningRemoteManagerVersionId = "updatesNotificationState_WarningRemoteManagerVersion";
        public static string updatesNotificationState_WarningHostControllerVersionId = "updatesNotificationState_WarningHostControllerVersion";
        public static string updatesNotificationState_WarningWebManagerVersionId = "updatesNotificationState_WarningWebManagerVersion";
        public static string updatesNotificationState_WarningInstallUpdatesId = "updatesNotificationState_WarningInstallUpdates";

        public static string updatesMessageErrorId = "updatesMessageError";

        public static string updatesReInstallTitleId = "updatesReInstallTitle";
        public static string updatesReInstallInformationId = "updatesReInstallInformation";
        public static string updatesReInstallUpdateStatusId = "updatesReInstallUpdateStatus";
        public static string updatesReInstallUpdateId = "updatesReInstallUpdate";
        public static string updatesReInstallCancelId = "updatesReInstallCancel";
        public static string updatesReInstallTryAgainId = "updatesReInstallTryAgain";

        public static string updateLastUpdateId = "updateLastUpdate";
        public static string updatesPanelSearchUpdatesId = "updatesPanelSearchUpdates";
        public static string updatesPanelConfigurationUpdatesId = "updatesPanelConfigurationUpdates";
        public static string updatesPanelSearchTitleId = "updatesPanelSearchTitle";
        public static string updatesPanelConfigurationUpdatesTitleId = "updatesPanelConfigurationUpdatesTitle";
        public static string updatesPanelAutomaticUpdatesId = "updatesPanelAutomaticUpdates";
        public static string updatesPanelRButUpdateId = "updatesPanelRButUpdate";
        public static string updatesPanelRButCheckId = "updatesPanelRButCheck";
        public static string updatesPanelRButDisableId = "updatesPanelRButDisable";
        public static string updatesPanelInstallNewUpdatesId = "updatesPanelInstallNewUpdates";

        public static string updatesPanelEveryDayId = "updatesPanelEveryDay";
        public static string updatesPanelEveryMondayId = "updatesPanelEveryMonday";
        public static string updatesPanelEveryTuesdayId = "updatesPanelEveryTuesday";
        public static string updatesPanelEveryWednesdayId = "updatesPanelEveryWednesday";
        public static string updatesPanelEveryThursdayId = "updatesPanelEveryThursday";
        public static string updatesPanelEveryFridayId = "updatesPanelEveryFriday";
        public static string updatesPanelEverySaturdayId = "updatesPanelEverySaturday";
        public static string updatesPanelEverySundayId = "updatesPanelEverySunday";

        public static string updatesPanelAtId = "updatesPanelAt";
        public static string updatesPanelScheduleUpdatesId = "updatesPanelScheduleUpdates";
        public static string updatesPanelAutomaticUpdateScheduleId = "updatesPanelAutomaticUpdateSchedule";

        public static string updatesPanelNeverId = "updatesPanelNever";

        public static string updatesPanelUpdatesStationTitleId = "updatesPanelUpdatesStationTitle";
        public static string updatesPanelStationNotUpdatableId = "updatesPanelStationNotUpdatable";
        public static string updatesPanelStationInProgressId = "updatesPanelStationInProgress";
        public static string updatesPanelStationUpToDateId = "updatesPanelStationUpToDate";
        public static string updatesPanelStationNewSoftwareId = "updatesPanelStationNewSoftware";
        public static string updatesPanelStationNameId = "updatesPanelStationName";
        public static string updatesPanelStationModelId = "updatesPanelStationModel";
        public static string updatesPanelStationSoftwareVersionId = "updatesPanelStationSoftwareVersion";
        public static string updatesPanelStationUpdateAvailableId = "updatesPanelStationUpdateAvailable";

        //About
        public static string aboutLineRemoteManagerId = "aboutLineRemoteManager";
        public static string aboutLineStationControllerId = "aboutLineStationController";
        public static string aboutLineHostControllerId = "aboutLineHostController";
        public static string aboutLineRightsId = "aboutLineRights";

        public static string mnuViewParametersId = "mnuViewParameters";
        public static string mnuPrintCurrentSettingsId = "mnuPrintCurrentSettings";
        public static string mnuUpdateSoftwareId = "mnuUpdateSoftware";
        public static string mnuAddPlotSerieId = "mnuAddPlotSerie";
        public static string mnuNewStationGroupId = "mnuNewStationGroup";
        public static string mnuDeleteTreeItemId = "mnuDeleteTreeItem";
        public static string mnuChangeTreeItemId = "mnuChangeTreeItem";
        public static string mnuReopenStationId = "mnuReopenStation";

        //Options menu
        public static string mnuOptionsId = "mnuOptions";
        public static string mnuLanguagesId = "mnuLanguages";
        public static string mnuLangDEId = "mnuLangDE";
        public static string mnuLangENId = "mnuLangEN";
        public static string mnuLangESId = "mnuLangES";
        public static string mnuLangJAId = "mnuLangJA";
        public static string mnuShowErrorNotificationsId = "mnuShowErrorNotifications";
        public static string mnuShowStationControllerNotificationsId = "mnuShowStationControllerNotifications";
        public static string mnuShowStationNotificationsId = "mnuShowStationNotifications";
        public static string mnuEnvironmentId = "mnuEnvironment";
        public static string mnuInternationalSettingsId = "mnuInternationalSettings";
        public static string mnuUnitsId = "mnuUnits";

        public static string mnuHelpFailedId = "mnuHelpFailed";

        public static string notifNoNotifId = "notifNoNotif";

        // -> Dock panel
        public static string dockStationListId = "dockStationList";
        public static string dockFloatingId = "dockFloating";
        public static string dockTopId = "dockTop";
        public static string dockBottomId = "dockBottom";
        public static string dockLeftId = "dockLeft";
        public static string dockRightId = "dockRight";
        public static string dockAutoHideId = "dockAutoHide";
        public static string dockPanelPosId = "dockPanelPos";
        public static string dockcloseId = "dockClose";
        public static string dockViewTileId = "dockViewTile";
        public static string dockViewDetailsId = "dockViewDetails";
        public static string dockViewTreeListId = "dockViewTreeList";
        public static string dockViewListId = "dockViewList";
        // listview subitems
        public static string subItemModelId = "subItemModel";
        public static string subItemSWId = "subItemSW";
        public static string subItemTypeId = "subItemType";

        // FORM PORTS ----------------------------------------
        public static Font PortsFont = new Font("Microsoft Sans Seriff", 12);
        public static Color PortsTextColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
        public static Font PortsHighLightFont = new Font("Microsoft Sans Seriff", 12, FontStyle.Italic);
        public static Color PortsTextHighLightColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));

        public static string PortsPortTitleId = "portsPortTitle";
        public static string PortsSleepId = "portsSleep";
        public static string PortsHiberId = "portsHiber";
        public static string PortsDesolderId = "portsDesolder";
        public static string PortsExtractorId = "portsExtractor";
        public static string PortsWorkId = "portsWork";
        public static string PortsStandId = "portsStand";
        public static string PortsNoSleepId = "portsNoSleep";
        public static string PortsDelayId = "portsDelay";
        public static string PortsDelayToSleepId = "portsDelayToSleep";
        public static string PortsDelayToHiberId = "portsDelayToHiber";
        public static string PortsNoToolId = "portsNoTool";
        public static string PortsNoPortId = "portsNoPort";
        public static System.String PortsActualTempId = "PortsActualTemp";
        //changeha
        public static System.String PortsActualFlowId = "PortsActualFlow";
        public static System.String PortsCoolingId = "portsCooling";
        public static System.String PortsStopId = "portsStop";
        public static System.String PortsPedalId = "portsPedal";
        public static System.String PortsHeaterId = "portsHeater";
        public static System.String PortsSuctionId = "portsSuction";
        public static System.String PortsTimeToStopId = "portsTimeToStop";

        public static int PortsToolImgWidth = 70;
        public static int PortsToolImgHeight = 80;
        public static int PortsToolDataWidth = 200;
        public static int PortsToolDataHeight = 80;
        public static int PortsHorzMargin = 5;
        public static int PortsVertMargin = 5;

        // ==================================================================
        // FORM STATION
        // ==================================================================
        public static string paramsLimitsErrorId = "paramsLimitsError";
        public static string paramsMinLimitErrorId = "paramsMinLimitError";
        public static string paramsMaxLimitErrorId = "paramsMaxLimitError";
        public static string paramsAllowedValuesId = "paramsAllowedValues";

        //
        // Page work
        //
        public static string modeControlModeId = "modeControlMode";
        public static string modeMonitorModeId = "modeMonitorMode";
        public static string modeRobotLooseControlWarningId = "modeRobotLooseControlWarning";
        public static string modeRobotGetControlWarningId = "modeRobotGetControlWarning";
        public static string modeControlWarningId = "modeControlWarning";
        public static string MinutesId = "txtMinutes";

        public static string workTabHintId = "workTabHint";
        public static string workPortId = "workPort";
        public static string workPowerId = "workPower";
        public static string workTempSelectionId = "workTempSelection";
        public static System.String workTempSelectedId = "workTempSelected"; // HA
        public static System.String workFlowSelectedId = "workFlowSelected"; // HA
        public static string workTempLevelsId = "workTempLevels";
        public static string workTempFixedId = "workTempFixed";
        public static string workNoToolId = "workNoTool";
        public static string workToolNeededId = "workToolNeeded";

        //
        // Page station settings
        //
        // table texts from Resources (used also for xml)
        //titles
        public static string stnGenSettingsId = "stnGenSettings";
        public static string stnEthConfId = "stnEthConf";
        public static string stnRbtConfId = "stnRbtConf";
        // general
        public static string stnNameId = "stnName";
        public static string stnTunitsId = "stnTunits";
        public static string stnPINId = "stnPIN";
        public static string stnTminId = "stnTmin";
        public static string stnTmaxId = "stnTmax";
        public static string stnFlowminId = "stnFlowmin"; // HA
        public static string stnFlowmaxId = "stnFlowmax"; // HA
        public static string stnTExtminId = "stnTExtmin"; // HA
        public static string stnTExtmaxId = "stnTExtmax"; // HA
        public static string stnPwrLimitId = "stnPwrLimit";
        public static string stnBeepId = "stnBeep";
        public static string stnHelpId = "stnHelp";
        public static string stnN2Id = "stnN2";
        // options
        public static string stnTunitsCelsiusId = "stnTunitsCelsius";
        public static string stnTunitsFahrenheitId = "stnTunitsFahrenheit";

        public static string stnTabHintId = "stnTabHint";
        public static string stnStationSettingsId = "stnStationSettings";
        // ethernet
        public static string stnEthDHCPId = "stnEthDHCP";
        public static string stnEthIPId = "stnEthIP";
        public static string stnEthMaskId = "stnEthMask";
        public static string stnEthGatewayId = "stnEthGateway";
        public static string stnEthDNSId = "stnEthDNS";
        public static string stnEthPortId = "stnEthPort";
        // robot
        public static string stnRbtStatusId = "stnRbtStatus";
        public static string stnRbtProtocolId = "stnRbtProtocol";
        public static string stnRbtAddressId = "stnRbtAddress";
        public static string stnRbtSpeedId = "stnRbtSpeed";
        public static string stnRbtDataBitsId = "stnRbtDataBits";
        public static string stnRbtStopBitsId = "stnRbtStopBits";
        public static string stnRbtParityId = "stnRbtParity";
        public static string stnRbtParityNoneID = "stnRbtParityNone";
        public static string stnRbtParityEvenID = "stnRbtParityEven";
        public static string stnRbtParityOddID = "stnRbtParityOdd";

        //
        // Page Info
        //
        public static string stnModelId = "stnModel"; // xml Station attribute

        public static string stnProtocolId = "stnProtocol";
        public static string stnSWId = "stnSW";
        public static string stnTrafoErrId = "stnTrafoErr";
        public static string stnMOSErrId = "stnMOSErr";

        public static string infoTabHintId = "infoTabHint";

        //
        // Page Tool Settings
        //
        public static string toolSelectedToolId = "toolSelectedTool";
        // table texts from Resources (used also for xml)
        public static string toolSelectedTempId = "toolSelectedTemp";
        public static string toolFixTempId = "toolFixTemp";
        public static string toolSelectedTempLvlId = "toolSelectedTempLvl";
        public static string toolTempLvl1Id = "toolTempLvl1";
        public static string toolTempLvl2Id = "toolTempLvl2";
        public static string toolTempLvl3Id = "toolTempLvl3";
        public static string toolSleepTempId = "toolSleepTemp";
        public static string toolSleepDelayId = "toolSleepDelay";
        public static string toolHibernationDelayId = "toolHibernationDelay";
        public static string toolAdjustTempId = "toolAdjustTemp";
        public static string toolCartridgeId = "toolCartridge";
        public static string cartNotFoundId = "cartNotFound";
        public static string toolAllSameSettingsId = "toolAllSameSettings";

        // HA
        public static string toolSelectedAirFlowId = "toolSelectedAirFlow";
        public static string toolProfilesId = "toolProfiles";
        public static string toolTimeToStopId = "toolTimeToStop";
        public static string toolExtTCModeId = "toolExtTCMode";
        public static string toolExtTCTempId = "toolExtTCTemp";
        public static string toolExtTCModeRegulationId = "toolExtTCModeRegulation";
        public static string toolExtTCModeProtectionId = "toolExtTCModeProtection";
        public static string toolStartModeToolButtonId = "toolStartModeToolButton";
        public static string toolStartModeStandOutId = "toolStartModeStandOut";
        public static string toolStartModePedalActionId = "toolStartModePedalAction";
        public static string toolStartModePedalPressOnceId = "toolStartModePedalPressOnce";
        public static string toolStartModePedalPressContId = "toolStartModePedalPressCont";
        // options
        public static string toolNoSelectedTempLvlId = "toolNoSelectedTempLvl";
        public static string toolSelectedTempLvlFirstId = "toolSelectedTempLvlFirst";
        public static string toolSelectedTempLvlSecondId = "toolSelectedTempLvlSecond";
        public static string toolSelectedTempLvlThirdId = "toolSelectedTempLvlThird";
        public static string toolNoSleepDelayId = "toolNoSleepDelay";
        public static string toolNoHibernationDelayId = "toolNoHibernationDelay";

        public static string toolTabHintId = "toolTabHint";
        public static string toolPortsAndToolsId = "toolPortsAndTools";
        public static string toolPortId = "toolPort";
        public static string toolToolId = "toolTool";

        // -> Page load and save
        public static string confInfoId = "confInfo";
        public static string confLoadId = "confLoad";
        public static string confSaveId = "confSave";
        public static string confMustControlModeId = "confMustControlMode";

        public static string confTabHintId = "confTabHint";

        // -> PageResetSettings
        public static string resetInfoId = "resetInfo";
        public static string resetProceedId = "resetProceed";
        public static string resetSuccessMessageId = "resetSuccessMessage";

        public static string resetTabHintId = "resetTabHint";
        public static int resetPgBarInc = 20;

        //
        // Page Peripherals
        //
        public static string peripheralTabHintId = "peripheralTabHint";
        public static string peripheralNoPeripheralId = "peripheralNoPeripheral";

        public static string peripheralNoSupportedId = "peripheralNoSupported";
        public static string peripheralFunctionId = "peripheralFunction";
        public static string peripheralActivationId = "peripheralActivation";
        public static string peripheralTimeId = "peripheralTime";
        public static string peripheralErrorTimeId = "peripheralErrorTime";

        public static string peripheralPedalId = "peripheralPedal";
        public static string peripheralElectricDesId = "peripheralElectricDes";
        public static string peripheralNitrogenId = "peripheralNitrogen";
        public static string peripheralFumeExtId = "peripheralFumeExt";
        public static string peripheralPneumaticDesId = "peripheralPneumaticDes";
        public static string peripheralSleepId = "peripheralSleep";
        public static string peripheralExtractorId = "peripheralExtractor";
        public static string peripheralModuleId = "peripheralModule";
        public static string peripheralPressedId = "peripheralPressed";
        public static string peripheralReleasedId = "peripheralReleased";


        //
        // Page Counters
        //
        public static string counterPluggedMinutesId = "counterPluggedMinutes";
        public static string counterWorkMinutesId = "counterWorkMinutes";
        public static string counterSleepMinutesId = "counterSleepMinutes";
        public static string counterHiberMinutesId = "counterHiberMinutes";
        public static string counterNoToolMinutesId = "counterNoToolMinutes";
        public static string counterSleepCyclesId = "counterSleepCycles";
        public static string counterDesolderCyclesId = "counterDesolderCycles";
        public static string counterWorkCyclesId = "counterWorkCycles"; // HA
        public static string counterSuctionCyclesId = "counterSuctionCycles"; // HA

        // partial counters (sólo para xml)
        //Public counterPartialPluggedMinutesId As String = "partialPluggedMinutes"
        //Public counterPartialWorkMinutesId As String = "partialWorkMinutes"
        //Public counterPartialSleepMinutesId As String = "partialSleepMinutes"
        //Public counterPartialHiberMinutesId As String = "partialHiberMinutes"
        //Public counterPartialNoToolMinutesId As String = "partialNoToolMinutes"
        //Public counterPartialSleepCyclesId As String = "partialSleepCycles"
        //Public counterPartialDesolderCyclesId As String = "partialDesolderCycles"

        public static string counterGlobalId = "counterGlobal"; // text
        public static string counterPartialId = "counterPartial"; // text
        public static string counterResetPartialCountersId = "counterResetPartialCounters";

        public static string counterTabHintId = "counterTabHint";

        // -> Page graphics
        public static string graphTabHintId = "graphTabHint";
        public static string graphNewPlotId = "graphNewPlot";

        public static string graphInfoId = "graphInfo";
        public static string graphAddToPlotId = "graphAddToPlot";
        public static string graphSeriesAddSeriesId = "graphSeriesAddSeries";


        //FORM SETTINGS MANAGER --------------------------------------

        public static frmConfManager frmConfMgr = null;
        public static string sTreeName = "paramTreeCtrl";
        public static string confNoId = "confNo";
        public static string confWarnNoConfTreeId = "confWarnNoConfTree";
        public static string confWarnNoStationSelectedId = "confWarnNoStationSelected";
        public static string confWarnStationsInMonitorModeId = "confWarnStationsInMonitorMode";
        public static string confWarnStationNotConnectedId = "confWarnStationNotConnected";
        public static string confMsgApplyingId = "confMsgApplying";
        public static string confMsgAppliedId = "confMsgApplied";
        public static string confMsgAppliedTimeoutId = "confMsgAppliedTimeout";
        public static string confWarnStationsNotUpdatedId = "confWarnStationsNotUpdated";

        public static string confConfCommentId = "confComment";
        public static string confSourceSettingsId = "confSourceSettings";
        public static string confTargetStationsListId = "confTargetStationsList";
        public static string confStationId = "confStation";
        public static string confTargetPortId = "confTargetPort";
        public static string confViewStationsId = "confViewStations";
        public static string confRefreshStationsId = "confRefreshStations";
        public static string confApplyId = "confApply";
        public static string confVersionId = "confVersion";
        public static string confViewReportId = "confViewReport";
        public static string confFiltersId = "confFilters";
        public static string confFiltersModelId = "confFiltersModel";
        public static string confFiltersNameId = "confFiltersName";
        // tree drop down menu
        public static string confMnuCopyId = "confMnuCopy";
        public static string confMnuPasteId = "confMnuPaste";
        public static string confMnuPasteCheckedId = "confMnuPasteChecked";
        public static string confMnuPasteAndCheckId = "confMnuPasteAndCheck";
        public static string confMnuPasteAndCheckCheckedId = "confMnuPasteAndCheckChecked";
        public static string confMnuViewId = "confMnuView";
        public static string confMnuViewCollapseNodeId = "confMnuViewCollapseNode";
        public static string confMnuViewCollapseAllId = "confMnuViewCollapseAll";
        public static string confMnuViewExpandNodeId = "confMnuViewExpandNode";
        public static string confMnuViewExpandAllId = "confMnuViewExpandAll";
        // hints
        public static string confHintPort2PortId = "confHintPort2Port";
        public static string confHintPort2ToolId = "confHintPort2Tool";
        public static string confHintPort2AllPortsId = "confHintPort2AllPorts";
        public static string confHintTool2PortId = "confHintTool2Port";
        public static string confHintTool2ToolId = "confHintTool2Tool";
        public static string confHintTool2AllPortsId = "confHintTool2AllPorts";
        public static string confHintToolNotCheckedId = "confHintToolNotChecked";


        // LOG ------------------------------------------------------
        public static string logSourceId = "logSource";
        public static string logTargetId = "logTarget";
        public static string logPortsId = "logPorts";
        public static string logLogsId = "logLogs";
        public static string logTargetPortFromSourcePortId = "logTargetPortFromSourcePort";
        public static string logWantToSeeReportId = "logWantToSeeReport";
        // warnings
        public static string logErrNotChangedId = "logErrNotChanged";
        public static string logErrNotExistId = "logErrNotExist";
        public static string logErrPortToolNotChangedId = "logErrPortToolNotChanged";
        public static string logErrPortToolNotExistId = "logErrPortToolNotExist";
        public static string logErrPortNotExistId = "logErrPortNotExist";
        public static string logErrToolNotExistId = "logErrToolNotExist";

        // XML ----------------------------------------------------
        public static string xmlDateTimeFormat = "yyyy-MM-dd HH:mm:ss"; // "yyyy-MM-dd HH:mm:ss UTC:z"
        public static string xmlDateFormat = "yyyy-MM-dd";
        public static string xmlTimeFormat = "HH:mm:ss";
        // xml labels (not settings)
        public static string xmlRootId = "JBCstationParameters"; // xml root node
        public static string xmlCommentId = "Comment"; // xml node, child of root (text confConfCommentId)
        public static string xmlStationId = "Station"; // xml node, child of root (text confStationId) and Group node
        public static string xmlStationSettingsId = "StationSettings"; // xml node, child of Station (text stnStationSettingsId)
        public static string xmlPortsAndToolsId = "PortsAndTools"; // xml node, child  of Station (text toolPortsAndToolsId)
        public static string xmlPortId = "Port"; // xml node, child of PortsAndTools (text toolPortId)
        public static string xmlToolId = "Tool"; // xml node, child of Port (text toolToolId)
        public static string xmlCountersId = "Counters"; // xml node, child of Station (text counterGlobalId)
        public static string xmlPartialCountersId = "PartialCounters"; // xml node, child of Station (text counterPartialId)
        public static string xmlRobotId = "Robot"; // xml node, child of Station (text robotId)
        public static string xmlProfileId = "Profile"; // xml node, child of Station (text profileId)
                                                       // attributes (there are general XML attributes defined in modXMLRutinas.vb file)
        public static string xmlTempUnitsId = "TempUnits"; // xml Station attribute: temperature units in station paramenters for the Station node (text stnTunitsId)
                                                           // XML labels for station list and tree
        public static string xmlStationListId = "StationList"; // xml root for station list xml doc
        public static string xmlDockPanelPositionId = "Position";
        public static string xmlDockPanelLocationId = "Location";
        public static string xmlDockPanelSizeId = "Size";
        public static string xmlDockPanelListId = "List";
        public static string xmlDockPanelListViewId = "ListView";
        public static string xmlDockPanelTreeId = "Tree";
        public static string xmlDockPanelTreeSizeId = "TreeSize";
        public static string xmlGroupListId = "GroupList"; // xml for group list inside xmlStationListId
        public static string xmlGroupId = "Group"; // xml node, child of xmlGroupList or xmlGroup
                                                   // attributes
        public static string xmlGroupNameId = "GroupName"; // xml attribute of xmlGroupId
        public static string xmlConnectedStationIDId = "ConnectedStationID"; // xml attribute of xmlStationId in GroupList
        public static string xmlStationNamedId = "StationNamed"; // xml attribute of xmlStationId in GroupList
                                                                 // xml labels for log
        public static string xmlLogRootId = "JBCConfigLog"; // xml log root node
        public static string xmlLogSourceId = "SourceParams"; // xml log SourceParams node (text confSourceSettingsId)
        public static string xmlLogTargetId = "TargetStations"; // xml log TargetStations node (text confTargetStationsListId)
        public static string xmlLogPortsId = "Ports"; // xml log Ports node (text logPortsId)
        public static string xmlTextFromPortId = "TextFromPort";
        public static string xmlLogsId = "Logs"; // xml logs node
        public static string xmlLogId = "Log"; // xml log node

        // XSL TEXTS -----------------------------------------------
        public static string xslStationConfigurationId = "xslStationConfiguration"; // xsl config title
        public static string xslParameterId = "xslParameter"; // xsl text
        public static string xslValueId = "xslValue"; // xsl text
        public static string xslLogTitleId = "xslLogTitle"; // xsl log title

        //FORM BROWSER --------------------------------------
        public static string browPrintId = "browPrint";
        public static string browPreviewId = "browPreview";
        public static string browPageSetupId = "browPageSetup";

        //FORM EVENTS --------------------------------------
        public static string evEventsId = "evEvents";
        public static string evErrorsId = "evErrors";
        public static string evConnectionsId = "evConnections";
        public static string evStationConnectedId = "evStationConnected";
        public static string evStationDisconnectedId = "evStationDisconnected";
        public static string evStationControllerConnectedId = "evStationControllerConnected";
        public static string evStationControllerDisconnectedId = "evStationControllerDisconnected";

        // USER ERRORS
        public const string ueErrorId = "ueError";
        public const string ueSTATION_ID_NOT_FOUND = "ueSTATION_ID_NOT_FOUND";
        public const string ueCONTINUOUS_MODE_ON_WITHOUT_PORTS = "ueCONTINUOUS_MODE_ON_WITHOUT_PORTS";
        public const string uePORT_NOT_IN_RANGE = "uePORT_NOT_IN_RANGE";
        public const string ueINVALID_STATION_NAME = "ueINVALID_STATION_NAME";
        public const string ueINVALID_STATION_PIN = "ueINVALID_STATION_PIN";
        public const string ueTEMPERATURE_OUT_OF_RANGE = "ueTEMPERATURE_OUT_OF_RANGE";
        public const string ueSTATION_ID_OVERFLOW = "ueSTATION_ID_OVERFLOW";
        public const string uePOWER_LIMIT_OUT_OF_RANGE = "uePOWER_LIMIT_OUT_OF_RANGE";
        public const string ueTOOL_NOT_SUPPORTED = "ueTOOL_NOT_SUPPORTED";
        public const string ueFUNCTION_NOT_SUPPORTED = "ueFUNCTION_NOT_SUPPORTED";
        public const string ueCOMMUNICATION_ERROR = "ueCOMMUNICATION_ERROR";
        // USER COMMUNICATION ERRORS
        public const string ceErrorId = "ceError";
        public const string ceNO_COMM_ERROR = "ceNO_COMM_ERROR";
        public const string ceBCC = "ceBCC";
        public const string ceFRAME_FORMAT = "ceFRAME_FORMAT";
        public const string ceOUT_OF_RANGE = "ceOUT_OF_RANGE";
        public const string ceCOMMAND_REJECTED = "ceCOMMAND_REJECTED";
        public const string ceCONTROL_MODE_REQUIRED = "ceCONTROL_MODE_REQUIRED";
        public const string ceINCORRECT_SEQUENCY = "ceINCORRECT_SEQUENCY";
        public const string ceFLASH_WRITE_ERROR = "ceFLASH_WRITE_ERROR";
        public const string ceCONTROL_MODE_ALREADY_ACTIVATED = "ceCONTROL_MODE_ALREADY_ACTIVATED";
        public const string ceNOT_VALID_HARDWARE = "ceNOT_VALID_HARDWARE";

        #region Customized Cursors
        public static Cursor cursor_switch_plus;
        public static Cursor cursor_switch_minus;
        public static Cursor cursor_hand;
        public static Cursor cursor_enable;

        public static void loadCursors()
        {
            cursor_switch_plus = new Cursor(new System.IO.MemoryStream(My.Resources.Resources.HandPlus));
            cursor_switch_minus = new Cursor(new System.IO.MemoryStream(My.Resources.Resources.HandMinus));
            cursor_hand = new Cursor(new System.IO.MemoryStream(My.Resources.Resources.Hand));
            cursor_enable = new Cursor(new System.IO.MemoryStream(My.Resources.Resources.HandEnable));
        }
        #endregion

        #region General Routines

        // control/monitor mode
        // central procedure that updates mode in station form, dock panel and Configuration form
        public static bool setStationControlMode(JBC_API_Remote jbc, long stationID, ref ListViewItem stationItem, bool bControlMode)
        {
            bool returnValue = false;
            returnValue = true;
            tStationDataItemList dataItemList = new tStationDataItemList();
            //Dim stationID As Long = CType(stationItem.Tag, tStationDataItemList).ID
            //
            // Setting control mode
            //
            if (bControlMode)
            {
                string sCurrentPIN = jbc.GetStationPIN(stationID);
                string sDefaultResponse = "";
                if (sCurrentPIN == "0105")
                {
                    sDefaultResponse = sCurrentPIN;
                }
                else
                {
                    sDefaultResponse = "0000";
                }

                // if supervisor loggedin do not ask for station password
                if (!bSupervisorLoggedIn)
                {
                    string code = Interaction.InputBox(Localization.getResStr(ctrlEnterPINCodeId) + ": ", Localization.getResStr(ctrlPINCodeId), sDefaultResponse);
                    if (string.IsNullOrEmpty(code))
                    {
                        returnValue = false;
                    }
                    else if (code != jbc.GetStationPIN(stationID))
                    {
                        MessageBox.Show(Localization.getResStr(supervInvalidPasswordId));
                        returnValue = false;
                    }
                }

                if (returnValue)
                {
                    // set station
                    jbc.SetControlMode(stationID, ControlModeConnection.CONTROL, Environment.MachineName);
                    // mark stations list
                    dataItemList = (tStationDataItemList)stationItem.Tag;
                    dataItemList.bControlMode = true;
                    stationItem.Tag = dataItemList;
                    stationItem.ImageKey = "Station_unlock";
                    // change form
                    paintFormStationControlMode(stationID, stationItem);
                    // update confmanager form if exists
                    if (frmConfMgr != null)
                    {
                        frmConfMgr.updateStationCheckControl(stationID);
                    }
                }
            }
            else
            {
                //
                // Setting monitor mode
                //
                // set station
                jbc.SetControlMode(stationID, ControlModeConnection.MONITOR, Environment.MachineName);
                // mark stations list
                dataItemList = (tStationDataItemList)stationItem.Tag;
                dataItemList.bControlMode = false;
                stationItem.Tag = dataItemList;
                stationItem.ImageKey = "Station_lock";
                // change form
                paintFormStationControlMode(stationID, stationItem);
                // update confmanager form if exists
                if (frmConfMgr != null)
                {
                    frmConfMgr.updateStationCheckControl(stationID);
                }
            }
            return returnValue;
        }

        public static void paintFormStationControlMode(long stationID, ListViewItem stationItem)
        {
            tStationDataItemList dataItemList = (tStationDataItemList)stationItem.Tag;
            int stsIdx = frmMain.Default.getStationIndex(stationID);
            frmMain.tStation stsElement = frmMain.Default.getStationListElement((int)stationID);
            // change form control mode
            if (stsElement.frm != null)
            {
                if (dataItemList.bControlMode)
                {
                    stsElement.frm.cbMode.Image = My.Resources.Resources.unlock;
                    stsElement.frm.cbMode.Text = Localization.getResStr(modeControlModeId);
                    stsElement.frm.mode = frmStation.CONTROL_MODE;
                }
                else
                {
                    stsElement.frm.cbMode.Image = My.Resources.Resources._lock;
                    stsElement.frm.cbMode.Text = Localization.getResStr(modeMonitorModeId);
                    stsElement.frm.mode = frmStation.MONITOR_MODE;
                }
            }
            if (stsElement.frmHA != null)
            {
                if (dataItemList.bControlMode)
                {
                    stsElement.frmHA.cbMode.Image = My.Resources.Resources.unlock;
                    stsElement.frmHA.cbMode.Text = Localization.getResStr(modeControlModeId);
                    stsElement.frmHA.mode = frmStation_HA.CONTROL_MODE;
                }
                else
                {
                    stsElement.frmHA.cbMode.Image = My.Resources.Resources._lock;
                    stsElement.frmHA.cbMode.Text = Localization.getResStr(modeMonitorModeId);
                    stsElement.frmHA.mode = frmStation_HA.MONITOR_MODE;
                }
            }
        }

        // others routines
        public static bool myControlExists(Form frm, string sControlName, ref System.Windows.Forms.Control myControl)
        {
            bool returnValue = false;
            System.Windows.Forms.Control[] aControls = frm.Controls.Find(sControlName, true);
            returnValue = false;
            if (aControls.Length > 0)
            {
                myControl = aControls[0];
                returnValue = true;
            }
            return returnValue;
        }

        public static int myGetRadioButtonPortNbr(RadioButton rb)
        {
            int returnValue = 0;
            returnValue = -1;
            if (rb.Name.IndexOf("1") >= 0)
            {
                returnValue = 1;
            }
            else if (rb.Name.IndexOf("2") >= 0)
            {
                returnValue = 2;
            }
            else if (rb.Name.IndexOf("3") >= 0)
            {
                returnValue = 3;
            }
            else if (rb.Name.IndexOf("4") >= 0)
            {
                returnValue = 4;
            }
            return returnValue;
        }

        public static string myGetRadioButtonToolName(RadioButton rb)
        {
            string returnValue = "";
            returnValue = rb.Name.Replace("rbToolSettings_", "");
            return returnValue;
        }

        public static string myGetFormatedDelay(int iDelay)
        {
            if (iDelay > 0)
            {
                int iMin = 0;
                int iSecs = 0;
                iMin = Math.DivRem(iDelay, 60, out iSecs);
                return iMin.ToString() + ":" + Strings.Format(iSecs, "00");
            }
            else
            {
                return "";
            }

        }

        public static string myGetStrFromMinutes(int iMinutes)
        {
            int iHrs = iMinutes / 60;
            if (iHrs > 0)
            {
                iMinutes = iMinutes % 60;
                return string.Format(Localization.getResStr(gralHoursMinutesId), iHrs, iMinutes);
            }
            else
            {
                return string.Format(Localization.getResStr(gralMinutesId), iMinutes);
            }
        }

        #endregion

        #region Temperature Functions
        public static string convertTempToString(CTemperature temp, bool bAddunits, bool bRounded, string myTunits = "")
        {
            string ReturnString = "";

            // If temperature is not nothing
            if (!Equals(temp, null))
            {
                if (myTunits == "")
                {
                    myTunits = Tunits; // if blank, use current Tunits value
                }

                if (temp.UTI == Constants.NO_TEMP_LEVEL)
                {
                    ReturnString = noDataStr;
                }
                else if (temp.UTI == 0)
                {
                    ReturnString = "0";
                }
                else
                {
                    // If temperatre has a valid value
                    // Getting the temperature string depending on the desired units
                    if (myTunits == CELSIUS_STR)
                    {
                        if (bRounded)
                        {
                            ReturnString = System.Convert.ToString(temp.ToRoundCelsius().ToString());
                        }
                        else
                        {
                            ReturnString = System.Convert.ToString(temp.ToCelsius().ToString());
                        }
                    }
                    else if (myTunits == FAHRENHEIT_STR)
                    {
                        if (bRounded)
                        {
                            ReturnString = System.Convert.ToString(temp.ToRoundFahrenheit().ToString());
                        }
                        else
                        {
                            ReturnString = System.Convert.ToString(temp.ToFahrenheit().ToString());
                        }
                    }
                    else
                    {
                        myTunits = CELSIUS_STR;
                        ReturnString = System.Convert.ToString(temp.ToCelsius().ToString());
                    }
                }
                if (bAddunits)
                {
                    ReturnString += " " + myTunits;
                }
            }
            else
            {
                ReturnString = invalidTempStr;
            }

            return ReturnString;

        }

        public static CTemperature convertStringToTemp(string temp, string myTunits = "")
        {
            CTemperature ret = new CTemperature(0);
            int aux = 0;

            // if temp string contains a value = 0 (aux = 0) then returns a temperature.UTI = 0

            // If string is not nothing
            if (!Equals(temp, null))
            {
                // If string has a valid value
                try
                {
                    // Setting the temperature value depending on the desired units
                    if (temp.IndexOf(noDataStr) + 1 > 0)
                    {
                        ret.UTI = Constants.NO_TEMP_LEVEL;
                    }
                    else if (temp.IndexOf(CELSIUS_STR) + 1 > 0)
                    {
                        // units come into the string
                        aux = Convert.ToInt32(temp.Replace(CELSIUS_STR, "").Trim());
                        if (aux != 0)
                        {
                            ret.InCelsius(aux);
                        }
                    }
                    else if (temp.IndexOf(FAHRENHEIT_STR) + 1 > 0)
                    {
                        // units come into the string
                        aux = Convert.ToInt32(temp.Replace(FAHRENHEIT_STR, "").Trim());
                        if (aux != 0)
                        {
                            ret.InFahrenheit(aux);
                        }
                    }
                    else
                    {
                        // use units defined in myTunits
                        if (myTunits == "")
                        {
                            myTunits = Tunits; // if blank, use current Tunits value
                        }
                        if (myTunits == CELSIUS_STR)
                        {
                            aux = Convert.ToInt32(temp.Trim());
                            if (aux != 0)
                            {
                                ret.InCelsius(aux);
                            }
                        }
                        else if (myTunits == FAHRENHEIT_STR)
                        {
                            aux = Convert.ToInt32(temp.Trim());
                            if (aux != 0)
                            {
                                ret.InFahrenheit(aux);
                            }
                        }
                        else
                        {
                            aux = Convert.ToInt32(temp.Trim());
                            if (aux != 0)
                            {
                                ret.InCelsius(aux);
                            }
                        }
                    }

                    return ret;
                }
                catch
                {
                    return ret;
                }
            }
            else
            {
                return ret;
            }
        }

        public static string convertTempAdjToString(CTemperature temp, bool bAddunits, string myTunits = "")
        {
            string ReturnString = "";

            // If temperature is not nothing
            if (!Equals(temp, null))
            {
                if (myTunits == "")
                {
                    myTunits = Tunits; // if blank, use current Tunits value
                }

                if (temp.UTI == Constants.NO_TEMP_LEVEL)
                {
                    ReturnString = noDataStr;
                }
                else if (temp.UTI == 0)
                {
                    ReturnString = "0";
                }
                else
                {
                    // If temperatre has a valid value
                    // Getting the temperature string depending on the desired units
                    if (myTunits == CELSIUS_STR)
                    {
                        ReturnString = System.Convert.ToString(temp.ToCelsiusToAdjust().ToString());
                    }
                    else if (myTunits == FAHRENHEIT_STR)
                    {
                        ReturnString = System.Convert.ToString(temp.ToFahrenheitToAdjust().ToString());
                    }
                    else
                    {
                        myTunits = CELSIUS_STR;
                        ReturnString = System.Convert.ToString(temp.ToCelsiusToAdjust().ToString());
                    }
                }
                if (bAddunits)
                {
                    ReturnString += " " + myTunits;
                }
            }
            else
            {
                ReturnString = invalidTempStr;
            }

            return ReturnString;

        }

        public static CTemperature convertStringToTempAdj(string temp, string myTunits = "")
        {
            CTemperature ret = new CTemperature(0);
            int aux = 0;

            // if temp string contains a value = 0 (aux = 0) then returns a temperature.UTI = 0

            // If string is not nothing
            if (!Equals(temp, null))
            {
                // If string has a valid value
                try
                {
                    // Setting the temperature value depending on the desired units
                    if (temp.IndexOf(noDataStr) + 1 > 0)
                    {
                        ret.UTI = 0;
                    }
                    else if (temp.IndexOf(CELSIUS_STR) + 1 > 0)
                    {
                        // units come into the string
                        aux = Convert.ToInt32(temp.Replace(CELSIUS_STR, "").Trim());
                        if (aux != 0)
                        {
                            ret.InCelsiusToAdjust(aux);
                        }
                    }
                    else if (temp.IndexOf(FAHRENHEIT_STR) + 1 > 0)
                    {
                        // units come into the string
                        aux = Convert.ToInt32(temp.Replace(FAHRENHEIT_STR, "").Trim());
                        if (aux != 0)
                        {
                            ret.InFahrenheitToAdjust(aux);
                        }
                    }
                    else
                    {
                        // use units defined in myTunits
                        if (myTunits == "")
                        {
                            myTunits = Tunits; // if blank, use current Tunits value
                        }
                        if (myTunits == CELSIUS_STR)
                        {
                            aux = Convert.ToInt32(temp.Trim());
                            if (aux != 0)
                            {
                                ret.InCelsiusToAdjust(aux);
                            }
                        }
                        else if (myTunits == FAHRENHEIT_STR)
                        {
                            aux = Convert.ToInt32(temp.Trim());
                            if (aux != 0)
                            {
                                ret.InFahrenheitToAdjust(aux);
                            }
                        }
                        else
                        {
                            aux = Convert.ToInt32(temp.Trim());
                            if (aux != 0)
                            {
                                ret.InCelsiusToAdjust(aux);
                            }
                        }
                    }

                    return ret;
                }
                catch
                {
                    return ret;
                }
            }
            else
            {
                return ret;
            }
        }

        private static string convertSTempToSTemp(string temp, string tempTunits, string targettempTunits)
        {
            // convert string temp to string temp
            if (tempTunits == targettempTunits)
            {
                return temp;
            }
            else
            {
                return convertTempToString(convertStringToTemp(temp, tempTunits), false, true, targettempTunits);
            }
        }

        private static string convertSTempAdjToSTempAdj(string temp, string tempTunits, string targettempTunits)
        {
            // convert string temp to string temp
            if (tempTunits == targettempTunits)
            {
                return temp;
            }
            else
            {
                return convertTempAdjToString(convertStringToTempAdj(temp, tempTunits), false, targettempTunits);
            }
        }

        public static bool stepTemp(CTemperature temp, int iStep, string myTunits = "")
        {
            bool bReturn = false;
            int aux = 0;

            // If temperature is not nothing
            if (!Equals(temp, null))
            {

                if (temp.UTI == Constants.NO_TEMP_LEVEL)
                {
                    bReturn = false;
                }
                else if (temp.UTI == 0)
                {
                    bReturn = false;
                }
                else
                {
                    if (myTunits == "")
                    {
                        myTunits = Tunits; // if blank, use current Tunits value
                    }
                    // If temperatre has a valid value
                    // Getting the temperature string depending on the desired units
                    if (myTunits == CELSIUS_STR)
                    {
                        aux = temp.ToRoundCelsius();
                        temp.InCelsius(aux + (iStep * 5));
                        bReturn = true;
                    }
                    else if (myTunits == FAHRENHEIT_STR)
                    {
                        aux = temp.ToRoundFahrenheit();
                        temp.InFahrenheit(aux + (iStep * 10));
                        bReturn = true;
                    }
                    else
                    {
                        temp.InCelsius(temp.ToRoundCelsius() + 5);
                        bReturn = true;
                    }
                }
            }
            else
            {
                bReturn = false;
            }

            return bReturn;

        }

        public static CTemperature getMaxTemp(string sModel)
        {
            switch (sModel)
            {
                case "HD":
                case "HDR":
                    return tempMaxHD;
                default:
                    return tempMax;
            }
        }

        #endregion

        #region Time functions

        public static string convertMinuteTimeToString(int minutes)
        {
            return string.Format(Localization.getResStr(MinutesId), minutes);
        }

        #endregion

        #region Value and Text Arrays

        public enum arrOption : byte
        {
            VALUES = 0,
            TEXTS = 1
        }

        public static string[] getOnOff(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] { OnOff._ON.ToString(), OnOff._OFF.ToString() };
                    break;
                case arrOption.TEXTS:
                    arr = new[] { Localization.getResStr(ON_STRId), Localization.getResStr(OFF_STRId) };
                    break;
            }

            return arr;
        }

        public static string[] getSleepDelays(arrOption opt, bool bWithoutNOValue)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] {ToolTimeSleep.MINUTE_0.ToString(),
                             ToolTimeSleep.MINUTE_1.ToString(),
                             ToolTimeSleep.MINUTE_2.ToString(),
                             ToolTimeSleep.MINUTE_3.ToString(),
                             ToolTimeSleep.MINUTE_4.ToString(),
                             ToolTimeSleep.MINUTE_5.ToString(),
                             ToolTimeSleep.MINUTE_6.ToString(),
                             ToolTimeSleep.MINUTE_7.ToString(),
                             ToolTimeSleep.MINUTE_8.ToString(),
                             ToolTimeSleep.MINUTE_9.ToString()};
                    if (!bWithoutNOValue)
                    {
                        Array.Resize(ref arr, arr.Length + 1);
                        Array.Copy(arr, 0, arr, 1, arr.Length - 1);
                        arr[0] = ToolTimeSleep.NO_SLEEP.ToString();
                    }
                    break;

                case arrOption.TEXTS:
                    arr = new[] {string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_0.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_1.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_2.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_3.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_4.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_5.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_6.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_7.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_8.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeSleep.MINUTE_9.ToString().Replace("MINUTE_", ""))};
                    if (!bWithoutNOValue)
                    {
                        Array.Resize(ref arr, arr.Length + 1);
                        Array.Copy(arr, 0, arr, 1, arr.Length
                            - 1);
                        arr[0] = Localization.getResStr(toolNoSleepDelayId);
                    }
                    break;

            }

            return arr;

        }

        public static string[] getHiberDelays(arrOption opt, bool bWithoutNOValue)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] {ToolTimeHibernation.MINUTE_0.ToString(),
                             ToolTimeHibernation.MINUTE_5.ToString(),
                             ToolTimeHibernation.MINUTE_10.ToString(),
                             ToolTimeHibernation.MINUTE_15.ToString(),
                             ToolTimeHibernation.MINUTE_20.ToString(),
                             ToolTimeHibernation.MINUTE_25.ToString(),
                             ToolTimeHibernation.MINUTE_30.ToString(),
                             ToolTimeHibernation.MINUTE_35.ToString(),
                             ToolTimeHibernation.MINUTE_40.ToString(),
                             ToolTimeHibernation.MINUTE_45.ToString(),
                             ToolTimeHibernation.MINUTE_50.ToString(),
                             ToolTimeHibernation.MINUTE_55.ToString(),
                             ToolTimeHibernation.MINUTE_60.ToString()};
                    if (!bWithoutNOValue)
                    {
                        Array.Resize(ref arr, arr.Length + 1);
                        Array.Copy(arr, 0, arr, 1, arr.Length - 1);
                        arr[0] = ToolTimeHibernation.NO_HIBERNATION.ToString();
                    }
                    break;

                case arrOption.TEXTS:
                    arr = new[] {string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_0.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_5.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_10.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_15.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_20.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_25.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_30.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_35.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_40.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_45.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_50.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_55.ToString().Replace("MINUTE_", "")),
                             string.Format(Localization.getResStr(MinutesId), ToolTimeHibernation.MINUTE_60.ToString().Replace("MINUTE_", ""))};
                    if (!bWithoutNOValue)
                    {
                        Array.Resize(ref arr, arr.Length + 1);
                        Array.Copy(arr, 0, arr, 1, arr.Length - 1);
                        arr[0] = Localization.getResStr(toolNoHibernationDelayId);
                    }
                    break;
            }

            return arr;
        }

        public static string[] getTempLevels(arrOption opt, bool bWithoutNoLevels)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] {ToolTemperatureLevels.FIRST_LEVEL.ToString(),
                             ToolTemperatureLevels.SECOND_LEVEL.ToString(),
                             ToolTemperatureLevels.THIRD_LEVEL.ToString()};
                    if (!bWithoutNoLevels)
                    {
                        Array.Resize(ref arr, arr.Length + 1);
                        Array.Copy(arr, 0, arr, 1, arr.Length - 1);
                        arr[0] = ToolTemperatureLevels.NO_LEVELS.ToString();
                    }
                    break;
                case arrOption.TEXTS:
                    arr = new[] { Localization.getResStr(toolSelectedTempLvlFirstId), Localization.getResStr(toolSelectedTempLvlSecondId), Localization.getResStr(toolSelectedTempLvlThirdId) };
                    if (!bWithoutNoLevels)
                    {
                        Array.Resize(ref arr, arr.Length + 1);
                        Array.Copy(arr, 0, arr, 1, arr.Length - 1);
                        arr[0] = Localization.getResStr(toolNoSelectedTempLvlId);
                    }
                    break;
            }
            return arr;
        }

        public static string[] getEthernetPort(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] { "{0}", Convert.ToString(1), Convert.ToString(65534) };
                    break;
            }

            return arr;
        }

        public static string[] getRobotProtocol(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] {CRobotData.RobotProtocol.RS232.ToString(),
                             CRobotData.RobotProtocol.RS485.ToString()};
                    break;

                case arrOption.TEXTS:
                    arr = new[] { "RS-232", "RS-485" };
                    break;
            }

            return arr;
        }

        public static string[] getRobotAddress(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] { "{0}", Convert.ToString(0), Convert.ToString(99) };
                    break;
            }

            return arr;
        }

        public static string[] getRobotSpeed(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] {CRobotData.RobotSpeed.bps_1200.ToString(),
                             CRobotData.RobotSpeed.bps_2400.ToString(),
                             CRobotData.RobotSpeed.bps_4800.ToString(),
                             CRobotData.RobotSpeed.bps_9600.ToString(),
                             CRobotData.RobotSpeed.bps_19200.ToString(),
                             CRobotData.RobotSpeed.bps_38400.ToString(),
                             CRobotData.RobotSpeed.bps_57600.ToString(),
                             CRobotData.RobotSpeed.bps_115200.ToString(),
                             CRobotData.RobotSpeed.bps_230400.ToString(),
                             CRobotData.RobotSpeed.bps_250000.ToString(),
                             CRobotData.RobotSpeed.bps_460800.ToString(),
                             CRobotData.RobotSpeed.bps_500000.ToString()};
                    break;

                case arrOption.TEXTS:
                    arr = new[] { "1.200 bps", "2.400 bps", "4.800 bps", "9.600 bps", "19.200 bps", "38.400 bps", "57.600 bps", "115.200 bps", "230.400 bps", "250.000 bps", "460.800 bps", "500.000 bps" };
                    break;
            }

            return arr;
        }

        public static string[] getRobotStopBits(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] {CRobotData.RobotStop.bits_1.ToString(),
                             CRobotData.RobotStop.bits_2.ToString()};
                    break;

                case arrOption.TEXTS:
                    arr = new[] { "1 bit", "2 bits" };
                    break;
            }

            return arr;
        }

        public static string[] getRobotParity(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] { CRobotData.RobotParity.None.ToString(),
                              CRobotData.RobotParity.Even.ToString(),
                              CRobotData.RobotParity.Odd.ToString() };
                    break;

                case arrOption.TEXTS:
                    arr = new[] { Localization.getResStr(stnRbtParityNoneID), Localization.getResStr(stnRbtParityEvenID), Localization.getResStr(stnRbtParityOddID) };
                    break;
            }

            return arr;
        }

        public static string[] getTimeToStop(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] { "{0}", Convert.ToString(0), Convert.ToString(60) };
                    break;
            }

            return arr;
        }

        public static string[] getExtTCMode(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] { ToolExternalTCMode_HA.REGULATION.ToString(),
                              ToolExternalTCMode_HA.PROTECTION.ToString() };
                    break;

                case arrOption.TEXTS:
                    arr = new[] {Localization.getResStr(toolExtTCModeRegulationId),
                    Localization.getResStr(toolExtTCModeProtectionId)};
                    break;

            }

            return arr;
        }

        public static string[] getStartModePedalActivation(arrOption opt)
        {
            string[] arr = null;
            switch (opt)
            {
                case arrOption.VALUES:
                    arr = new[] {PedalAction.PULSE.ToString(),
                             PedalAction.HOLD_DOWN.ToString()};
                    break;

                case arrOption.TEXTS:
                    arr = new[] { Localization.getResStr(toolStartModePedalPressOnceId), Localization.getResStr(toolStartModePedalPressContId) };
                    break;

            }

            return arr;
        }

        #endregion

        #region Station Group List Functions

        public static System.Xml.XmlDocument stationlistNewXmlDoc()
        {
            System.Xml.XmlDocument returnValue = default(System.Xml.XmlDocument);
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            System.Xml.XmlNode xmlNodeRoot = default(System.Xml.XmlNode);

            returnValue = null;

            try
            {
                xmlDoc = RoutinesLibrary.Data.Xml.XMLUtils.CreateNewDoc();
                //root for station list
                xmlNodeRoot = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlDoc, xmlStationListId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNodeRoot, ConfigurationXML.xmlDateId, Strings.Format(DateTime.Now, xmlDateFormat));
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNodeRoot, ConfigurationXML.xmlTimeId, Strings.Format(DateTime.Now, xmlTimeFormat));

                //node for group list
                xmlNodeRoot = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlNodeRoot, xmlGroupListId, null);

                returnValue = xmlDoc;

            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        public static bool grouplistXmlToTree(System.Xml.XmlDocument xmlDoc, TreeView oTree)
        {
            bool returnValue = false;
            System.Xml.XmlNode xmlNodeRoot = default(System.Xml.XmlNode);
            returnValue = false;

            try
            {
                oTree.Nodes.Clear();

                // add tree root
                TreeNode tnodeRoot = RoutinesLibrary.UI.TreeViewUtils.AddNode(oTree, xmlGroupListId, Localization.getResStr(dockStationListId));
                // type
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnodeRoot, ConfigurationXML.xmlTypeId, xmlGroupListId); // Type = Group list (root)
                                                                                                                   // icons
                tnodeRoot.ImageKey = iconGroupListId;
                tnodeRoot.SelectedImageKey = iconGroupListId;
                tnodeRoot.ForeColor = colorRoot;

                // group list root node
                xmlNodeRoot = xmlGetGroupListNodeFromXml(xmlDoc);
                if (ReferenceEquals(xmlNodeRoot, null))
                {
                    return returnValue;
                }

                // add stations and groups
                if (xmlNodeRoot.HasChildNodes)
                {
                    foreach (System.Xml.XmlNode xmlChildNode in xmlNodeRoot.ChildNodes)
                    {
                        xmlProcessGroupListXmlNodeToTree(xmlChildNode, tnodeRoot);
                    }
                }

                returnValue = true;

            }
            catch (Exception)
            {

            }
            return returnValue;
        }

        public static bool grouplistTreeToXml(TreeView oTree, System.Xml.XmlDocument xmlDoc)
        {
            bool returnValue = false;
            System.Xml.XmlNode xmlParentNode = default(System.Xml.XmlNode);

            returnValue = false;
            try
            {
                xmlParentNode = xmlGetGroupListNodeFromXml(xmlDoc);

                TreeNode tnodeRoot = oTree.TopNode;
                if (!ReferenceEquals(tnodeRoot, null))
                {
                    foreach (TreeNode tnode in tnodeRoot.Nodes)
                    {
                        xmlProcessGroupListTreeNodeToXml(tnode, xmlParentNode);
                    }
                    returnValue = true;
                }
            }
            catch (Exception ex)
            {
                LoggerModule.logger.Error(System.Reflection.MethodInfo.GetCurrentMethod().Name + ". Error: " + ex.Message);
            }
            return returnValue;
        }

        public static TreeNode grouplistAddGroupNode(TreeNode tnodeParent, string sName)
        {
            TreeNode tnode = default(TreeNode);
            tnode = RoutinesLibrary.UI.TreeViewUtils.AddNode(tnodeParent, sName, sName);
            grouplistSetGroupNode(tnode, sName);
            return tnode;
        }

        public static TreeNode grouplistAddGroupNode(TreeView tnodeParent, string sName)
        {
            TreeNode tnode = default(TreeNode);
            tnode = RoutinesLibrary.UI.TreeViewUtils.AddNode(tnodeParent, sName, sName);
            grouplistSetGroupNode(tnode, sName);
            return tnode;
        }

        public static void grouplistSetGroupNode(TreeNode tnode, string sName)
        {
            tnode.Name = sName;
            tnode.Text = sName;
            // type
            RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, ConfigurationXML.xmlTypeId, xmlGroupId); // Type = Group
                                                                                                       // data
            RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, xmlGroupNameId, sName); // GroupName
                                                                                      // icons
            tnode.ImageKey = iconGroupId;
            tnode.SelectedImageKey = iconGroupId;
            tnode.ForeColor = colorGroup;
        }

        //Public Function grouplistAddStationNode(ByVal tnodeParent As Object, ByVal ID As Long, ByVal sName As String, sModel As String, ByVal bStationNamed As Boolean) As TreeNode
        public static TreeNode grouplistAddStationNode(TreeNode tnodeParent, strucStationData stnData)
        {
            // add node last in parent Nodes
            TreeNode tnode = default(TreeNode);
            tnode = RoutinesLibrary.UI.TreeViewUtils.AddNode(tnodeParent, stnData.sName, stnData.sName + " - " + stnData.sModel);
            grouplistSetStationNode(tnode, stnData);
            return tnode;
        }

        //Public Function grouplistAddStationNodeFirst(ByVal tnodeParent As Object, ByVal ID As Long, ByVal sName As String, sModel As String, ByVal bStationNamed As Boolean) As TreeNode
        public static TreeNode grouplistAddStationNodeFirst(TreeNode tnodeParent, strucStationData stnData)
        {
            // add node first in parent Nodes
            TreeNode tnode = default(TreeNode);
            //tnode = tnodeAddNode(tnodeParent, stnData.sName, stnData.sName & " - " & stnData.sModel)
            tnode = tnodeParent.Nodes.Insert(0, stnData.sName, stnData.sName + " - " + stnData.sModel);
            grouplistSetStationNode(tnode, stnData);
            return tnode;
        }

        //Public Sub grouplistSetStationNode(ByRef tnode As TreeNode, ByVal ID As Long, ByVal sName As String, sModel As String, ByVal bStationNamed As Boolean)
        public static void grouplistSetStationNode(TreeNode tnode, strucStationData stnData)
        {
            try
            {
                tnode.Name = stnData.sName;
                tnode.Text = stnData.sName + " - " + stnData.sModel;
                // udate ID and icons
                // type
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, ConfigurationXML.xmlTypeId, xmlStationId); // Type = Station
                                                                                                             // data
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, stnNameId, stnData.sName); // stnName
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, stnModelId, stnData.sModel); // stnModel
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, xmlConnectedStationIDId, stnData.ID); // Station ID
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, xmlStationNamedId, stnData.bStationNamed.ToString()); // station named or not

                // icons
                if (stnData.ID >= 0)
                {
                    tnode.ImageKey = iconStationConnId;
                    tnode.SelectedImageKey = iconStationConnId;
                    tnode.ForeColor = colorStationConnected;
                }
                else
                {
                    tnode.ImageKey = iconStationDisconnId;
                    tnode.SelectedImageKey = iconStationDisconnId;
                    tnode.ForeColor = colorStationDisconnected;
                }
            }
            catch (Exception)
            {

            }

        }

        //Public Function grouplistConnectedStationNode(ByRef oTree As TreeView, ByVal ID As Long, ByVal sName As String, sModel As String, ByVal bStationNamed As Boolean) As TreeNode
        public static TreeNode grouplistConnectedStationNode(TreeView oTree, strucStationData stnData)
        {
            TreeNode tnode = default(TreeNode);

            // do not add if no name and ID
            if (stnData.sName.Trim() == "")
            {
                return null;
            }
            if (stnData.ID < 0)
            {
                return null;
            }

            // search if exists by ID to update data
            tnode = grouplistGetStationNodeByID(oTree, stnData.ID);
            if (ReferenceEquals(tnode, null))
            {
                if (!stnData.bStationNamed)
                {
                    // do not find because is not named, add to root
                    // tnode = grouplistAddStationNodeFirst(oTree.TopNode, ID, sName, sModel, bStationNamed)
                    tnode = grouplistAddStationNode(oTree.TopNode, stnData);
                }
                else
                {
                    // search by name
                    tnode = grouplistGetStationNodeByName(oTree, stnData.sName, stnData.sModel);
                    if (ReferenceEquals(tnode, null))
                    {
                        // not found, add to root
                        //tnode = grouplistAddStationNodeFirst(oTree.TopNode, stnData)
                        tnode = grouplistAddStationNode(oTree.TopNode, stnData);
                    }
                    else
                    {
                        // found by name
                        grouplistSetStationNode(tnode, stnData);
                    }
                }
            }
            else
            {
                // found by ID
                grouplistSetStationNode(tnode, stnData);
            }

            return tnode;
        }

        public static TreeNode grouplistConnectedStationNameNode(TreeView oTree, long ID, string sName)
        {
            TreeNode tnode = default(TreeNode);
            // change station name
            // do not change if no ID
            if (sName.Trim() == "")
            {
                return null;
            }
            if (ID < 0)
            {
                return null;
            }

            // search if exists by ID to update data
            tnode = grouplistGetStationNodeByID(oTree, ID);
            if (tnode != null)
            {
                // found by ID
                tnode.Name = sName;
                tnode.Text = sName + " - " + RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, stnModelId);
                // data
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, stnNameId, sName); // stnName
                RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, xmlStationNamedId, true.ToString()); // station named or not
            }

            return tnode;
        }

        public static TreeNode grouplistDisconnectedStationNode(TreeView oTree, long ID)
        {
            // do not add if no ID
            if (ID < 0)
            {
                return null;
            }

            TreeNode tnode = grouplistGetStationNodeByID(oTree, ID);
            if (tnode != null)
            {
                if (RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, xmlStationNamedId) == "False")
                {
                    // if no named, delete
                    RoutinesLibrary.UI.TreeViewUtils.DeleteNode(ref tnode);
                }
                else
                {
                    // udate ID to -1 and icons
                    RoutinesLibrary.UI.TreeViewUtils.SetAttrib(tnode, xmlConnectedStationIDId, -1); // Station ID
                                                                                                    // icons
                    tnode.ImageKey = iconStationDisconnId;
                    tnode.SelectedImageKey = iconStationDisconnId;
                    tnode.ForeColor = colorStationDisconnected;
                }
            }

            return tnode;
        }

        public static TreeNode grouplistGetStationNodeByID(TreeView tnodeParent, long ID)
        {
            string sType = "";
            long stnID;
            TreeNode tnodeFound = null;

            foreach (TreeNode tnode in tnodeParent.Nodes)
            {
                sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                if (sType == xmlStationId)
                {
                    stnID = System.Convert.ToInt64(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, xmlConnectedStationIDId));
                    if (stnID == ID)
                    {
                        return tnode;
                    }
                }
                else if ((sType == xmlGroupId) || (sType == xmlGroupListId))
                {
                    if (tnode.Nodes.Count > 0)
                    {
                        tnodeFound = grouplistGetStationNodeByID(tnode, ID);
                        if (tnodeFound != null)
                        {
                            goto endOfForLoop;
                        }
                    }
                }
            }
            endOfForLoop:
            return tnodeFound;
        }

        public static TreeNode grouplistGetStationNodeByID(TreeNode tnodeParent, long ID)
        {
            string sType = "";
            long stnID;
            TreeNode tnodeFound = null;

            foreach (TreeNode tnode in tnodeParent.Nodes)
            {
                sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                if (sType == xmlStationId)
                {
                    stnID = System.Convert.ToInt64(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, xmlConnectedStationIDId));
                    if (stnID == ID)
                    {
                        return tnode;
                    }
                }
                else if ((sType == xmlGroupId) || (sType == xmlGroupListId))
                {
                    if (tnode.Nodes.Count > 0)
                    {
                        tnodeFound = grouplistGetStationNodeByID(tnode, ID);
                        if (tnodeFound != null)
                        {
                            goto endOfForLoop;
                        }
                    }
                }
            }
            endOfForLoop:
            return tnodeFound;
        }

        public static TreeNode grouplistGetStationNodeByName(TreeView tnodeParent, string sName, string sModel = "")
        {
            string sType = "";
            string sStnName;
            string sStnModel;
            TreeNode tnodeFound = null;

            foreach (TreeNode tnode in tnodeParent.Nodes)
            {
                sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                if (sType == xmlStationId)
                {
                    sStnName = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, stnNameId));
                    sStnModel = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, stnModelId));
                    if (sStnName == sName)
                    {
                        if (sModel != "")
                        {
                            if (sStnModel == sModel)
                            {
                                return tnode;
                            }
                        }
                        else
                        {
                            return tnode;
                        }
                    }
                }
                else if ((sType == xmlGroupId) || (sType == xmlGroupListId))
                {
                    if (tnode.Nodes.Count > 0)
                    {
                        tnodeFound = grouplistGetStationNodeByName(tnode, sName, sModel);
                        if (tnodeFound != null)
                        {
                            goto endOfForLoop;
                        }
                    }
                }
            }
            endOfForLoop:
            return tnodeFound;
        }

        public static TreeNode grouplistGetStationNodeByName(TreeNode tnodeParent, string sName, string sModel = "")
        {
            string sType = "";
            string sStnName;
            string sStnModel;
            TreeNode tnodeFound = null;

            foreach (TreeNode tnode in tnodeParent.Nodes)
            {
                sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
                if (sType == xmlStationId)
                {
                    sStnName = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, stnNameId));
                    sStnModel = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, stnModelId));
                    if (sStnName == sName)
                    {
                        if (sModel != "")
                        {
                            if (sStnModel == sModel)
                            {
                                return tnode;
                            }
                        }
                        else
                        {
                            return tnode;
                        }
                    }
                }
                else if ((sType == xmlGroupId) || (sType == xmlGroupListId))
                {
                    if (tnode.Nodes.Count > 0)
                    {
                        tnodeFound = grouplistGetStationNodeByName(tnode, sName, sModel);
                        if (tnodeFound != null)
                        {
                            goto endOfForLoop;
                        }
                    }
                }
            }
            endOfForLoop:
            return tnodeFound;
        }

        public static System.Xml.XmlNode xmlGetStationListNodeFromXml(System.Xml.XmlDocument xmlDoc)
        {
            var sCurrentXmlPath = "/" + xmlStationListId;
            return xmlDoc.SelectSingleNode(sCurrentXmlPath);
        }

        public static System.Xml.XmlNode xmlGetGroupListNodeFromXml(System.Xml.XmlDocument xmlDoc)
        {
            var sCurrentXmlPath = "/" + xmlStationListId + "/" + xmlGroupListId;
            return xmlDoc.SelectSingleNode(sCurrentXmlPath);
        }

        private static void xmlProcessGroupListXmlNodeToTree(System.Xml.XmlNode xmlNode, TreeNode tnodeParent)
        {
            strucStationData stnData = new strucStationData();
            //Dim sName As String
            //Dim sModel As String
            //Dim bStationNamed As Boolean = True
            TreeNode tNode = default(TreeNode);
            if (xmlNode.Name == xmlStationId)
            {
                // station
                stnData.sName = xmlNode.Attributes[stnNameId].Value;
                stnData.sModel = xmlNode.Attributes[stnModelId].Value;
                if (stnData.sName.Trim() == "")
                {
                    stnData.ID = -1;
                    stnData.sName = Localization.getResStr(gralNoNameId);
                    stnData.bStationNamed = false;
                    // do not add no named stations from station group list xml
                    //tNode = grouplistAddStationNode(tnodeParent, stnData)
                }
                else
                {
                    stnData.ID = -1;
                    stnData.bStationNamed = true;
                    tNode = grouplistAddStationNode(tnodeParent, stnData);
                }
            }
            else if (xmlNode.Name == xmlGroupId)
            {
                // folder
                stnData.sName = xmlNode.Attributes[xmlGroupNameId].Value;
                tNode = grouplistAddGroupNode(tnodeParent, stnData.sName);
                if (xmlNode.HasChildNodes)
                {
                    foreach (System.Xml.XmlElement xmlChildNode in xmlNode.ChildNodes)
                    {
                        xmlProcessGroupListXmlNodeToTree(xmlChildNode, tNode);
                    }
                }
            }
        }

        private static void xmlProcessGroupListTreeNodeToXml(TreeNode tnode, System.Xml.XmlNode xmlnodeParent)
        {
            string sType = "";
            string sName = "";
            string sModel = "";
            System.Xml.XmlNode xmlNode = default(System.Xml.XmlNode);
            string sStationNamed;

            sType = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, ConfigurationXML.xmlTypeId));
            if (sType == xmlStationId)
            {
                sName = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, stnNameId));
                sModel = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, stnModelId));
                sStationNamed = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, xmlStationNamedId));
                if (sStationNamed == "False")
                {
                    sName = "";
                }
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlnodeParent, xmlStationId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, stnNameId, sName); // station name in attribute
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, stnModelId, sModel); // station model in attribute
            }
            else if (sType == xmlGroupId)
            {
                sName = System.Convert.ToString(RoutinesLibrary.UI.TreeViewUtils.GetAttrib(tnode, xmlGroupNameId));
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlnodeParent, xmlGroupId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, xmlGroupNameId, sName); // group text in attribute
                if (tnode.Nodes.Count > 0)
                {
                    foreach (TreeNode tnodeChild in tnode.Nodes)
                    {
                        xmlProcessGroupListTreeNodeToXml(tnodeChild, xmlNode);
                    }
                }
            }
        }

        #endregion

        #region Settings Manager Functions

        public static async void confSetToStation(System.Xml.XmlDocument xmlDoc, long myID, JBC_API_Remote jbc, int[] iTargetFromSourcePorts = default(int[]))
        {
            // iTargetFromSourcePorts(targetport-1) contains target/source port correlations:
            //   format: iTargetFromSourcePorts(x) is target port x+1, value is source port
            // iTargetFromSourcePorts(x) = 0 specify no source for x+1 target port
            // if iTargetFromSourcePorts = nothing, use same target/source ports
            // examples:
            //   if iTargetFromSourcePorts(0) = 2 then target port 1 (0+1) from source port 2
            //   iTargetFromSourcePorts(1) = 0 indicates do not save to target port 2

            // it sets only allowed settings

            // New and Changed Features of target station
            //Dim features = New cls_Features(jbc.GetStationModel(myID), jbc.GetStationModelType(myID), jbc.GetStationModelVersion(myID), jbc.GetStationProtocol(myID))
            CFeaturesData features = jbc.GetStationFeatures(myID);

            string configVersion;
            string stationModel;
            string xmlTunits = CELSIUS_STR; // default Celsius
            CTemperature auxTemp = default(CTemperature);
            OnOff auxOnOff = default(OnOff);
            // to send level temps before SetPortToolSelectedTempLevels
            string sTempLvl1 = null;
            string sTempLvl2 = null;
            string sTempLvl3 = null;
            string sSelectedTempLevel = null;
            bool bTempLvl1Enabled = false;
            bool bTempLvl2Enabled = false;
            bool bTempLvl3Enabled = false;
            bool bSelectedTempLevelEnabled = false;
            bool bTempLvl1EnabledExists = false;
            bool bTempLvl2EnabledExists = false;
            bool bTempLvl3EnabledExists = false;
            bool bSelectedTempLevelEnabledExists = false;
            uint iDelay = (uint)0;
            bool bXmlChecked = true;
            bool bXmlCheckedExists = false;
            bool bXmlEnabled = false;
            bool bXmlEnabledExists = false;

            try
            {
                System.Xml.XmlNodeList configJBC = xmlDoc.SelectNodes("/" + xmlRootId);
                if (configJBC.Count > 0)
                {
                    configVersion = System.Convert.ToString(configJBC[0].Attributes[ConfigurationXML.xmlVersionId].Value);
                }

                //Dim stationNode As System.Xml.XmlNodeList = xmlDoc.SelectNodes("/" & xmlRootId & "/" & xmlStationId)
                System.Xml.XmlNode stationNode = xmlGetStationNodeFromXml(xmlDoc);
                if (stationNode != null)
                {
                    stationModel = stationNode.Attributes[stnModelId].Value;
                    xmlTunits = stationNode.Attributes[xmlTempUnitsId].Value;
                }

                System.Xml.XmlNode stationParams = xmlGetStationParamsNodeFromXml(xmlDoc);
                if (stationParams != null)
                {
                    foreach (System.Xml.XmlNode node in stationParams.ChildNodes)
                    {
                        // read "Checked" attribute
                        bXmlCheckedExists = ConfigurationXML.getXmlChecked(node, ref bXmlChecked);
                        // only change if checked or label does not exist in parámeter
                        if (!bXmlCheckedExists || bXmlChecked)
                        {
                            // station parameters
                            if (node.Name == stnNameId)
                            {
                                // station name
                                //jbc.SetStationName(myID, node.InnerText)
                            }
                            else if (node.Name == stnTunitsId)
                            {
                                if (features.DisplaySettings)
                                {
                                    await jbc.SetStationTempUnitsAsync(myID, (CTemperature.TemperatureUnit)(Convert.ToUInt32(Strings.Asc(node.InnerText)))); // C or F
                                }
                            }
                            else if (node.Name == stnTminId)
                            {
                                await jbc.SetStationMinTempAsync(myID, convertStringToTemp(node.InnerText, xmlTunits));
                            }
                            else if (node.Name == stnTmaxId)
                            {
                                await jbc.SetStationMaxTempAsync(myID, convertStringToTemp(node.InnerText, xmlTunits));
                            }
                            else if (node.Name == stnN2Id)
                            {
                                if (features.DisplaySettings)
                                {
                                    await jbc.SetStationN2ModeAsync(myID, (OnOff)(Convert.ToUInt32(node.InnerText)));
                                }
                            }
                            else if (node.Name == stnHelpId)
                            {
                                if (features.DisplaySettings)
                                {
                                    await jbc.SetStationHelpTextAsync(myID, (OnOff)(Convert.ToUInt32(node.InnerText)));
                                }
                            }
                            else if (node.Name == stnBeepId)
                            {
                                if (features.DisplaySettings)
                                {
                                    await jbc.SetStationBeepAsync(myID, (OnOff)(Convert.ToUInt32(node.InnerText)));
                                }
                            }
                            else if (node.Name == stnPwrLimitId)
                            {
                                //13/01/2014
                                //jbc.SetStationPowerLimit(myID, Convert.ToUInt32(node.InnerText))
                            }
                            else if (node.Name == stnPINId)
                            {
                                // PIN
                                //jbc.SetStationPIN(myID, node.InnerText)
                            }

                        } // If Not bXmlCheckedExists Or bXmlChecked
                    }
                }

                // ports and tools
                System.Xml.XmlNode toolPortNodes = xmlGetPortsAndToolsNodeFromXml(xmlDoc);
                if (toolPortNodes != null)
                {
                    System.Xml.XmlNodeList portNodes = toolPortNodes.ChildNodes;
                    //Dim nPorts As Integer = Math.Min(jbc.GetPortCount(myID), portNodes.Count)
                    int nTargetPorts = jbc.GetPortCount(myID);
                    if (ReferenceEquals(iTargetFromSourcePorts, null))
                    {
                        iTargetFromSourcePorts = new int[nTargetPorts + 1];
                        for (var i = 1; i <= nTargetPorts; i++)
                        {
                            iTargetFromSourcePorts[i - 1] = System.Convert.ToInt32(i);
                        }
                    }

                    GenericStationTools[] supportedTools = jbc.GetStationTools(myID);
                    int iSourcePortNbr = 0;
                    int iTargetPortNbr = 0;
                    GenericStationTools curTool = default(GenericStationTools);

                    foreach (System.Xml.XmlNode portNode in portNodes)
                    {
                        iSourcePortNbr = int.Parse(portNode.Attributes[ConfigurationXML.xmlNumberId].Value);
                        // where to copy this source port (may be several target ports)
                        // look for source port in target ports array
                        for (var iTargetIdx = 0; iTargetIdx <= (iTargetFromSourcePorts.Length - 1); iTargetIdx++)
                        {
                            iTargetPortNbr = System.Convert.ToInt32(iTargetIdx + 1);
                            // copy source settings for each target port
                            if (iTargetFromSourcePorts[iTargetIdx] == iSourcePortNbr)
                            {
                                if (iTargetPortNbr <= nTargetPorts & iTargetPortNbr > 0)
                                {

                                    Debug.Print(string.Format("CONF Source port {0} to target port {1}", iSourcePortNbr.ToString(), iTargetPortNbr.ToString()));

                                    foreach (System.Xml.XmlNode portchildnode in portNode.ChildNodes)
                                    {
                                        Debug.Print(string.Format("CONF Source Port {0} PortChildNode {1}", iSourcePortNbr.ToString(), portchildnode.Name));
                                        if (portchildnode.Name == toolSelectedTempId)
                                        {
                                            // read "Checked" attribute
                                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked);
                                            // only change if checked or label does not exist in parameter
                                            if (!bXmlCheckedExists || bXmlChecked)
                                            {
                                                auxTemp = convertStringToTemp(portchildnode.InnerText, xmlTunits);
                                                if (auxTemp.isValid())
                                                {
                                                    await jbc.SetPortToolSelectedTempAsync(myID, (Port)(iTargetPortNbr - 1), auxTemp);
                                                }
                                            }
                                        }
                                        else if (portchildnode.Name == xmlToolId)
                                        {
                                            //curTool = CType(CInt(portchildnode.Attributes.ItemOf(xmlNumberId).Value), JBC_API.GenericStationTools)
                                            curTool = xmlGetToolFromToolNode(portchildnode);
                                            // for update levels
                                            sTempLvl1 = "";
                                            sTempLvl2 = "";
                                            sTempLvl3 = "";
                                            sSelectedTempLevel = "";

                                            foreach (System.Xml.XmlNode toolparamNode in portchildnode.ChildNodes)
                                            {
                                                // read "Checked" attribute
                                                bXmlCheckedExists = ConfigurationXML.getXmlChecked(toolparamNode, ref bXmlChecked);
                                                // only change if checked or label does not exist in parameter
                                                if ((!bXmlCheckedExists) || bXmlChecked)
                                                {
                                                    Debug.Print(string.Format("CONF Source Port {0} Tool {1} Setting {2}", iSourcePortNbr.ToString(), curTool.ToString(), toolparamNode.Name));
                                                    if (toolparamNode.Name == toolFixTempId)
                                                    {
                                                        // fixed temp
                                                        if (!features.TempLevelsWithStatus)
                                                        {
                                                            // if "---", "0" or UTI NO_TEMP_LEVEL, -> Off
                                                            if (toolparamNode.InnerText == noDataStr || toolparamNode.InnerText == "0")
                                                            {
                                                                await jbc.SetPortToolFixTempAsync(myID, (Port)(iTargetPortNbr - 1), curTool, OnOff._OFF);
                                                            }
                                                            else
                                                            {
                                                                auxTemp = convertStringToTemp(toolparamNode.InnerText, xmlTunits);
                                                                if (auxTemp.UTI == Constants.NO_TEMP_LEVEL)
                                                                {
                                                                    await jbc.SetPortToolFixTempAsync(myID, (Port)(iTargetPortNbr - 1), curTool, OnOff._OFF);
                                                                }
                                                                else
                                                                {
                                                                    if (auxTemp.isValid())
                                                                    {
                                                                        await jbc.SetPortToolFixTempAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxTemp);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else if (toolparamNode.Name == toolSelectedTempLvlId)
                                                    {
                                                        sSelectedTempLevel = toolparamNode.InnerText;
                                                        bSelectedTempLevelEnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bSelectedTempLevelEnabled);
                                                    }
                                                    else if (toolparamNode.Name == toolTempLvl1Id)
                                                    {
                                                        sTempLvl1 = toolparamNode.InnerText;
                                                        bTempLvl1EnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bTempLvl1Enabled);
                                                    }
                                                    else if (toolparamNode.Name == toolTempLvl2Id)
                                                    {
                                                        sTempLvl2 = toolparamNode.InnerText;
                                                        bTempLvl2EnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bTempLvl2Enabled);
                                                    }
                                                    else if (toolparamNode.Name == toolTempLvl3Id)
                                                    {
                                                        sTempLvl3 = toolparamNode.InnerText;
                                                        bTempLvl3EnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bTempLvl3Enabled);
                                                    }
                                                    else if (toolparamNode.Name == toolSleepTempId)
                                                    {
                                                        auxTemp = convertStringToTemp(toolparamNode.InnerText, xmlTunits);
                                                        if (auxTemp.isValid())
                                                        {
                                                            await jbc.SetPortToolSleepTempAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxTemp);
                                                        }
                                                    }
                                                    else if (toolparamNode.Name == toolAdjustTempId)
                                                    {
                                                        auxTemp = convertStringToTempAdj(toolparamNode.InnerText, xmlTunits);
                                                        await jbc.SetPortToolAdjustTempAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxTemp);
                                                    }
                                                    else if (toolparamNode.Name == toolSleepDelayId)
                                                    {
                                                        bXmlEnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bXmlEnabled);
                                                        if (bXmlEnabledExists && !features.DelayWithStatus)
                                                        {
                                                            // enabled exists in source, not in target
                                                            if (bXmlEnabled == false)
                                                            {
                                                                // if source enabled tag exists set to false and target has not enable, then set delay to NO_SLEEP
                                                                iDelay = (uint)ToolTimeSleep.NO_SLEEP;
                                                            }
                                                            else
                                                            {
                                                                iDelay = (uint)(Convert.ToInt32(toolparamNode.InnerText));
                                                            }
                                                            await jbc.SetPortToolSleepDelayAsync(myID, (Port)(iTargetPortNbr - 1), curTool, (ToolTimeSleep)iDelay);
                                                        }
                                                        else if (!bXmlEnabledExists && features.DelayWithStatus)
                                                        {
                                                            // enabled does not exists in source, but in target
                                                            if (Convert.ToInt32(toolparamNode.InnerText) == (int)ToolTimeSleep.NO_SLEEP)
                                                            {
                                                                // set to target off and value as default
                                                                //iDelay = JBC_API_Remote.DEFAULT_SLEEP_TIME
                                                                iDelay = System.Convert.ToUInt32(Constants.DEFAULT_SLEEP_TIME);
                                                                auxOnOff = OnOff._OFF;
                                                                //jbc.SetPortToolSleepDelay(myID, CType(iTargetPort - 1, Port), curTool, CType(iDelay, ToolTimeSleep))
                                                                await jbc.SetPortToolSleepDelayEnabledAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxOnOff);
                                                            }
                                                            else
                                                            {
                                                                iDelay = (uint)(Convert.ToInt32(toolparamNode.InnerText));
                                                                auxOnOff = OnOff._ON;
                                                                await jbc.SetPortToolSleepDelayAsync(myID, (Port)(iTargetPortNbr - 1), curTool, (ToolTimeSleep)iDelay);
                                                                await jbc.SetPortToolSleepDelayEnabledAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxOnOff);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            iDelay = (uint)(Convert.ToInt32(toolparamNode.InnerText));
                                                            await jbc.SetPortToolSleepDelayAsync(myID, (Port)(iTargetPortNbr - 1), curTool, (ToolTimeSleep)iDelay);
                                                            if (features.DelayWithStatus)
                                                            {
                                                                if (bXmlEnabled)
                                                                {
                                                                    auxOnOff = OnOff._ON;
                                                                }
                                                                else
                                                                {
                                                                    auxOnOff = OnOff._OFF;
                                                                }
                                                                await jbc.SetPortToolSleepDelayEnabledAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxOnOff);
                                                            }
                                                        }
                                                    }
                                                    else if (toolparamNode.Name == toolHibernationDelayId)
                                                    {
                                                        bXmlEnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bXmlEnabled);
                                                        if (bXmlEnabledExists && !features.DelayWithStatus)
                                                        {
                                                            // enabled exists in source, not in target
                                                            if (bXmlEnabled == false)
                                                            {
                                                                // if source enabled tag exists set to false and target has not enable, then set delay to NO_HIBERNATION
                                                                iDelay = (uint)ToolTimeHibernation.NO_HIBERNATION;
                                                            }
                                                            else
                                                            {
                                                                iDelay = (uint)(Convert.ToInt32(toolparamNode.InnerText));
                                                            }
                                                            await jbc.SetPortToolHibernationDelayAsync(myID, (Port)(iTargetPortNbr - 1), curTool, (ToolTimeHibernation)iDelay);
                                                        }
                                                        else if (!bXmlEnabledExists && features.DelayWithStatus)
                                                        {
                                                            // enabled does not exists in source, but in target
                                                            if (Convert.ToInt32(toolparamNode.InnerText) == (int)ToolTimeHibernation.NO_HIBERNATION)
                                                            {
                                                                // set to target off and value as default
                                                                iDelay = System.Convert.ToUInt32(Constants.DEFAULT_HIBERNATION_TIME);
                                                                auxOnOff = OnOff._OFF;
                                                                //jbc.SetPortToolHibernationDelay(myID, CType(iTargetPort - 1, Port), curTool, CType(iDelay, ToolTimeHibernation))
                                                                await jbc.SetPortToolHibernationDelayEnabledAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxOnOff);
                                                            }
                                                            else
                                                            {
                                                                iDelay = (uint)(Convert.ToInt32(toolparamNode.InnerText));
                                                                auxOnOff = OnOff._ON;
                                                                await jbc.SetPortToolHibernationDelayAsync(myID, (Port)(iTargetPortNbr - 1), curTool, (ToolTimeHibernation)iDelay);
                                                                await jbc.SetPortToolHibernationDelayEnabledAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxOnOff);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            iDelay = (uint)(Convert.ToInt32(toolparamNode.InnerText));
                                                            await jbc.SetPortToolHibernationDelayAsync(myID, (Port)(iTargetPortNbr - 1), curTool, (ToolTimeHibernation)iDelay);
                                                            if (features.DelayWithStatus)
                                                            {
                                                                if (bXmlEnabled)
                                                                {
                                                                    auxOnOff = OnOff._ON;
                                                                }
                                                                else
                                                                {
                                                                    auxOnOff = OnOff._OFF;
                                                                }
                                                                await jbc.SetPortToolHibernationDelayEnabledAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxOnOff);
                                                            }
                                                        }
                                                    }

                                                } // If (Not bXmlCheckedExists) Or bXmlChecked
                                            } // tool child nodes

                                            // Update levels
                                            ToolTemperatureLevels tempLevel = default(ToolTemperatureLevels);
                                            ToolTemperatureLevels selectedLevel = default(ToolTemperatureLevels);
                                            CTemperature auxTemp_L1 = default(CTemperature);
                                            CTemperature auxTemp_L2 = default(CTemperature);
                                            CTemperature auxTemp_L3 = default(CTemperature);
                                            OnOff auxOnOff_L1 = default(OnOff);
                                            OnOff auxOnOff_L2 = default(OnOff);
                                            OnOff auxOnOff_L3 = default(OnOff);
                                            OnOff auxOnOff_selected = default(OnOff);

                                            // si la estación a configurar es "features.TempLevelsWithStatusNotFixed",
                                            // se configuran los datos y se envía sólo una orden al final
                                            // de lo contrario (es decir con protocolo 01) se envían las órdenes separadas
                                            var bActualizarLevelsUnicaOrden = false;

                                            // temp level 1
                                            tempLevel = ToolTemperatureLevels.FIRST_LEVEL;
                                            if (!string.IsNullOrEmpty(sTempLvl1))
                                            {
                                                bXmlEnabledExists = bTempLvl1EnabledExists;
                                                bXmlEnabled = bTempLvl1Enabled;

                                                if (bXmlEnabledExists && !features.TempLevelsWithStatus)
                                                {
                                                    // enabled exists in source, not in target
                                                    if (bXmlEnabled == false)
                                                    {
                                                        // if source enabled tag exists set to false and target has not enable, then set temp to NO_TEMP_LEVEL
                                                        auxTemp_L1 = new CTemperature(Constants.NO_TEMP_LEVEL);
                                                    }
                                                    else
                                                    {
                                                        auxTemp_L1 = convertStringToTemp(System.Convert.ToString(sTempLvl1), xmlTunits);
                                                    }
                                                    // enviar comando
                                                    await jbc.SetPortToolTempLevelAsync(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel, auxTemp_L1);
                                                }
                                                else if (!bXmlEnabledExists && features.TempLevelsWithStatus)
                                                {
                                                    // enviar comando al final
                                                    bActualizarLevelsUnicaOrden = true;
                                                    // enabled does not exists in source, but in target
                                                    if (convertStringToTemp(System.Convert.ToString(sTempLvl1), xmlTunits).UTI == Constants.NO_TEMP_LEVEL)
                                                    {
                                                        // set to target off and value as default
                                                        auxTemp_L1 = jbc.GetPortToolTempLevel(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                                        auxOnOff_L1 = OnOff._OFF;
                                                        //jbc.SetPortToolTempLevel(myID, CType(iTargetPort - 1, Port), curTool, tempLevel, auxTemp_L1)
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L1)
                                                    }
                                                    else
                                                    {
                                                        auxTemp_L1 = convertStringToTemp(System.Convert.ToString(sTempLvl1), xmlTunits);
                                                        auxOnOff_L1 = OnOff._ON;
                                                        //jbc.SetPortToolTempLevel(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxTemp_L1)
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L1)
                                                    }
                                                }
                                                else
                                                {
                                                    auxTemp_L1 = convertStringToTemp(System.Convert.ToString(sTempLvl1), xmlTunits);
                                                    if (features.TempLevelsWithStatus)
                                                    {
                                                        // enviar comando al final
                                                        bActualizarLevelsUnicaOrden = true;
                                                        if (bXmlEnabled)
                                                        {
                                                            auxOnOff_L1 = OnOff._ON;
                                                        }
                                                        else
                                                        {
                                                            auxOnOff_L1 = OnOff._OFF;
                                                        }
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff)
                                                    }
                                                    else
                                                    {
                                                        await jbc.SetPortToolTempLevelAsync(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel, auxTemp_L1);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                auxTemp_L1 = jbc.GetPortToolTempLevel(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                                auxOnOff_L1 = jbc.GetPortToolTempLevelEnabled(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                            }

                                            // temp level 2
                                            tempLevel = ToolTemperatureLevels.SECOND_LEVEL;
                                            if (!string.IsNullOrEmpty(sTempLvl2))
                                            {
                                                bXmlEnabledExists = bTempLvl2EnabledExists;
                                                bXmlEnabled = bTempLvl2Enabled;

                                                if (bXmlEnabledExists && !features.TempLevelsWithStatus)
                                                {
                                                    // enabled exists in source, not in target
                                                    if (bXmlEnabled == false)
                                                    {
                                                        // if source enabled tag exists set to false and target has not enable, then set temp to NO_TEMP_LEVEL
                                                        auxTemp_L2 = new CTemperature(Constants.NO_TEMP_LEVEL);
                                                    }
                                                    else
                                                    {
                                                        auxTemp_L2 = convertStringToTemp(System.Convert.ToString(sTempLvl2), xmlTunits);
                                                    }
                                                    // enviar comando
                                                    await jbc.SetPortToolTempLevelAsync(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel, auxTemp_L2);
                                                }
                                                else if (!bXmlEnabledExists && features.TempLevelsWithStatus)
                                                {
                                                    // enviar comando al final
                                                    bActualizarLevelsUnicaOrden = true;
                                                    // enabled does not exists in source, but in target
                                                    if (convertStringToTemp(System.Convert.ToString(sTempLvl2), xmlTunits).UTI == Constants.NO_TEMP_LEVEL)
                                                    {
                                                        // set to target off and value as default
                                                        auxTemp_L2 = jbc.GetPortToolTempLevel(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                                        auxOnOff_L2 = OnOff._OFF;
                                                        //jbc.SetPortToolTempLevel(myID, CType(iTargetPort - 1, Port), curTool, tempLevel, auxTemp_L2)
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L2)
                                                    }
                                                    else
                                                    {
                                                        auxTemp_L2 = convertStringToTemp(System.Convert.ToString(sTempLvl2), xmlTunits);
                                                        auxOnOff_L2 = OnOff._ON;
                                                        //jbc.SetPortToolTempLevel(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxTemp_L2)
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L2)
                                                    }
                                                }
                                                else
                                                {
                                                    auxTemp_L2 = convertStringToTemp(System.Convert.ToString(sTempLvl2), xmlTunits);
                                                    if (features.TempLevelsWithStatus)
                                                    {
                                                        // enviar comando al final
                                                        bActualizarLevelsUnicaOrden = true;
                                                        if (bXmlEnabled)
                                                        {
                                                            auxOnOff_L2 = OnOff._ON;
                                                        }
                                                        else
                                                        {
                                                            auxOnOff_L2 = OnOff._OFF;
                                                        }
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L2)
                                                    }
                                                    else
                                                    {
                                                        await jbc.SetPortToolTempLevelAsync(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel, auxTemp_L2);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                auxTemp_L2 = jbc.GetPortToolTempLevel(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                                auxOnOff_L2 = jbc.GetPortToolTempLevelEnabled(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                            }

                                            // temp level 3
                                            tempLevel = ToolTemperatureLevels.THIRD_LEVEL;
                                            if (!string.IsNullOrEmpty(sTempLvl3))
                                            {
                                                bXmlEnabledExists = bTempLvl3EnabledExists;
                                                bXmlEnabled = bTempLvl3Enabled;

                                                if (bXmlEnabledExists && !features.TempLevelsWithStatus)
                                                {
                                                    // enabled exists in source, not in target
                                                    if (bXmlEnabled == false)
                                                    {
                                                        // if source enabled tag exists set to false and target has not enable, then set temp to NO_TEMP_LEVEL
                                                        auxTemp_L3 = new CTemperature(Constants.NO_TEMP_LEVEL);
                                                    }
                                                    else
                                                    {
                                                        auxTemp_L3 = convertStringToTemp(System.Convert.ToString(sTempLvl3), xmlTunits);
                                                    }
                                                    // enviar comando
                                                    await jbc.SetPortToolTempLevelAsync(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel, auxTemp_L3);
                                                }
                                                else if (!bXmlEnabledExists && features.TempLevelsWithStatus)
                                                {
                                                    // enviar comando al final
                                                    bActualizarLevelsUnicaOrden = true;
                                                    // enabled does not exists in source, but in target
                                                    if (convertStringToTemp(System.Convert.ToString(sTempLvl3), xmlTunits).UTI == Constants.NO_TEMP_LEVEL)
                                                    {
                                                        // set to target off and value as default
                                                        auxTemp_L3 = jbc.GetPortToolTempLevel(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                                        auxOnOff_L3 = OnOff._OFF;
                                                        //jbc.SetPortToolTempLevel(myID, CType(iTargetPort - 1, Port), curTool, tempLevel, auxTemp_L3)
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L3)
                                                    }
                                                    else
                                                    {
                                                        auxTemp_L3 = convertStringToTemp(System.Convert.ToString(sTempLvl3), xmlTunits);
                                                        auxOnOff_L3 = OnOff._ON;
                                                        //jbc.SetPortToolTempLevel(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxTemp_L3)
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L3)
                                                    }
                                                }
                                                else
                                                {
                                                    auxTemp_L3 = convertStringToTemp(System.Convert.ToString(sTempLvl3), xmlTunits);
                                                    if (features.TempLevelsWithStatus)
                                                    {
                                                        // enviar comando al final
                                                        bActualizarLevelsUnicaOrden = true;
                                                        if (bXmlEnabled)
                                                        {
                                                            auxOnOff_L3 = OnOff._ON;
                                                        }
                                                        else
                                                        {
                                                            auxOnOff_L3 = OnOff._OFF;
                                                        }
                                                        //jbc.SetPortToolTempLevelEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, tempLevel, auxOnOff_L3)
                                                    }
                                                    else
                                                    {
                                                        await jbc.SetPortToolTempLevelAsync(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel, auxTemp_L3);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                auxTemp_L3 = jbc.GetPortToolTempLevel(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                                auxOnOff_L3 = jbc.GetPortToolTempLevelEnabled(myID, (Port)(iTargetPortNbr - 1), curTool, tempLevel);
                                            }

                                            // selected temp level
                                            if (!string.IsNullOrEmpty(sSelectedTempLevel))
                                            {
                                                bXmlEnabledExists = bSelectedTempLevelEnabledExists;
                                                bXmlEnabled = bSelectedTempLevelEnabled;
                                                selectedLevel = ToolTemperatureLevels.NO_LEVELS;
                                                // in xml are the text of the enumeration
                                                //jbc.SetPortToolSelectedTempLevels(myID, CType(iTargetPort - 1, Port), curTool, System.Enum.Parse(GetType(ToolTemperatureLevels), sSelectedTempLevel))
                                                // in xml are the number of the enumeration

                                                if (bXmlEnabledExists && !features.TempLevelsWithStatus)
                                                {
                                                    // enabled exists in source, not in target
                                                    if (bXmlEnabled == false)
                                                    {
                                                        // if source enabled tag exists set to false and target has not enable, then set to NO_LEVELS
                                                        selectedLevel = ToolTemperatureLevels.NO_LEVELS;
                                                    }
                                                    else
                                                    {
                                                        selectedLevel = (ToolTemperatureLevels)(Convert.ToUInt32(sSelectedTempLevel));
                                                    }
                                                    // enviar comando
                                                    await jbc.SetPortToolSelectedTempLevelsAsync(myID, (Port)(iTargetPortNbr - 1), curTool, selectedLevel);
                                                }
                                                else if (!bXmlEnabledExists && features.TempLevelsWithStatus)
                                                {
                                                    // enviar comando al final
                                                    bActualizarLevelsUnicaOrden = true;
                                                    // enabled does not exists in source, but in target
                                                    if (Convert.ToUInt32(sSelectedTempLevel) == (int)ToolTemperatureLevels.NO_LEVELS)
                                                    {
                                                        // set to target off and value as first
                                                        selectedLevel = jbc.GetPortToolSelectedTempLevels(myID, (Port)(iTargetPortNbr - 1), curTool);
                                                        auxOnOff_selected = OnOff._OFF;
                                                        //jbc.SetPortToolSelectedTempLevels(myID, CType(iTargetPort - 1, Port), curTool, selectedLevel)
                                                        //jbc.SetPortToolSelectedTempLevelsEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, auxOnOff_selected)
                                                    }
                                                    else
                                                    {
                                                        selectedLevel = (ToolTemperatureLevels)(Convert.ToUInt32(sSelectedTempLevel));
                                                        auxOnOff_selected = OnOff._ON;
                                                        //jbc.SetPortToolSelectedTempLevels(myID, CType(iTargetPortNbr - 1, Port), curTool, selectedLevel)
                                                        //jbc.SetPortToolSelectedTempLevelsEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, auxOnOff_selected)
                                                    }
                                                }
                                                else
                                                {
                                                    selectedLevel = (ToolTemperatureLevels)(Convert.ToUInt32(sSelectedTempLevel));
                                                    if (features.TempLevelsWithStatus)
                                                    {
                                                        // enviar comando al final
                                                        bActualizarLevelsUnicaOrden = true;
                                                        if (bXmlEnabled)
                                                        {
                                                            auxOnOff_selected = OnOff._ON;
                                                        }
                                                        else
                                                        {
                                                            auxOnOff_selected = OnOff._OFF;
                                                        }
                                                        //jbc.SetPortToolSelectedTempLevelsEnabled(myID, CType(iTargetPortNbr - 1, Port), curTool, auxOnOff_selected)
                                                    }
                                                    else
                                                    {
                                                        await jbc.SetPortToolSelectedTempLevelsAsync(myID, (Port)(iTargetPortNbr - 1), curTool, selectedLevel);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                selectedLevel = jbc.GetPortToolSelectedTempLevels(myID, (Port)(iTargetPortNbr - 1), curTool);
                                                auxOnOff_selected = jbc.GetPortToolSelectedTempLevelsEnabled(myID, (Port)(iTargetPortNbr - 1), curTool);
                                            }

                                            // si hay que actualizar levels del protocolo 02
                                            // hacer sólo un comando
                                            if (bActualizarLevelsUnicaOrden)
                                            {
                                                await jbc.SetPortToolLevelsAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxOnOff_selected, selectedLevel, auxOnOff_L1, auxTemp_L1,
                                                    auxOnOff_L2, auxTemp_L2,
                                                    auxOnOff_L3, auxTemp_L3);
                                            }
                                        } // port child node
                                    } // chidren port node
                                } // If iTargetPort <= nTargetPorts And iTargetPort > 0
                            } // iTargetFromSourcePorts(iTarget) = iSourcePort
                        } // For iTarget = 0 To UBound(iTargetFromSourcePorts)
                    } // portNodes
                }
            }
            catch (Exception ex)
            {
                Interaction.MsgBox("Error setting xml configuration to station: " + ex.Message, MsgBoxStyle.Critical, null);
                return;
            }
        }

        public static async void confSetToStation_HA(System.Xml.XmlDocument xmlDoc, long myID, JBC_API_Remote jbc, int[] iTargetFromSourcePorts = default(int[]))
        {
            // iTargetFromSourcePorts(targetport-1) contains target/source port correlations:
            //   format: iTargetFromSourcePorts(x) is target port x+1, value is source port
            // iTargetFromSourcePorts(x) = 0 specify no source for x+1 target port
            // if iTargetFromSourcePorts = nothing, use same target/source ports
            // examples:
            //   if iTargetFromSourcePorts(0) = 2 then target port 1 (0+1) from source port 2
            //   iTargetFromSourcePorts(1) = 0 indicates do not save to target port 2

            // it sets only allowed settings

            // New and Changed Features of target station
            //Dim features = New cls_Features(jbc.GetStationModel(myID), jbc.GetStationModelType(myID), jbc.GetStationModelVersion(myID), jbc.GetStationProtocol(myID))
            CFeaturesData features = jbc.GetStationFeatures(myID);

            string configVersion;
            string stationModel;
            string xmlTunits = CELSIUS_STR; // default Celsius
            CTemperature auxTemp = default(CTemperature);
            uint iDelay = (uint)0;
            bool bXmlChecked = true;
            bool bXmlCheckedExists = false;
            bool bXmlEnabled = false;
            bool bXmlEnabledExists = false;

            try
            {
                System.Xml.XmlNodeList configJBC = xmlDoc.SelectNodes("/" + xmlRootId);
                if (configJBC.Count > 0)
                {
                    configVersion = System.Convert.ToString(configJBC[0].Attributes[ConfigurationXML.xmlVersionId].Value);
                }

                //Dim stationNode As System.Xml.XmlNodeList = xmlDoc.SelectNodes("/" & xmlRootId & "/" & xmlStationId)
                System.Xml.XmlNode stationNode = xmlGetStationNodeFromXml(xmlDoc);
                if (stationNode != null)
                {
                    stationModel = stationNode.Attributes[stnModelId].Value;
                    xmlTunits = stationNode.Attributes[xmlTempUnitsId].Value;
                }

                System.Xml.XmlNode stationParams = xmlGetStationParamsNodeFromXml(xmlDoc);
                if (stationParams != null)
                {
                    foreach (System.Xml.XmlNode node in stationParams.ChildNodes)
                    {
                        // read "Checked" attribute
                        bXmlCheckedExists = ConfigurationXML.getXmlChecked(node, ref bXmlChecked);
                        // only change if checked or label does not exist in parámeter
                        if (!bXmlCheckedExists || bXmlChecked)
                        {
                            // station parameters
                            if (node.Name == stnNameId)
                            {
                                // station name
                                //jbc.SetStationName(myID, node.InnerText)
                            }
                            else if (node.Name == stnTunitsId)
                            {
                                if (features.DisplaySettings)
                                {
                                    await jbc.SetStationTempUnitsAsync(myID, (CTemperature.TemperatureUnit)(Convert.ToUInt32(Strings.Asc(node.InnerText)))); // C or F
                                }
                            }
                            else if (node.Name == stnTminId)
                            {
                                await jbc.SetStationMinTempAsync(myID, convertStringToTemp(node.InnerText, xmlTunits));
                            }
                            else if (node.Name == stnTmaxId)
                            {
                                await jbc.SetStationMaxTempAsync(myID, convertStringToTemp(node.InnerText, xmlTunits));
                            }
                            else if (node.Name == stnFlowminId)
                            {
                                await jbc.SetStationMinFlowAsync(myID, System.Convert.ToInt32(node.InnerText));
                            }
                            else if (node.Name == stnFlowmaxId)
                            {
                                await jbc.SetStationMaxFlowAsync(myID, System.Convert.ToInt32(node.InnerText));
                            }
                            else if (node.Name == stnTExtminId)
                            {
                                await jbc.SetStationMinExtTempAsync(myID, convertStringToTemp(node.InnerText, xmlTunits));
                            }
                            else if (node.Name == stnTExtmaxId)
                            {
                                await jbc.SetStationMaxExtTempAsync(myID, convertStringToTemp(node.InnerText, xmlTunits));
                            }
                            else if (node.Name == stnBeepId)
                            {
                                if (features.DisplaySettings)
                                {
                                    await jbc.SetStationBeepAsync(myID, (OnOff)(Convert.ToUInt32(node.InnerText)));
                                }
                            }

                        } // If Not bXmlCheckedExists Or bXmlChecked
                    }
                }

                // ports and tools
                System.Xml.XmlNode toolPortNodes = xmlGetPortsAndToolsNodeFromXml(xmlDoc);
                if (toolPortNodes != null)
                {
                    System.Xml.XmlNodeList portNodes = toolPortNodes.ChildNodes;
                    int nTargetPorts = jbc.GetPortCount(myID);
                    if (ReferenceEquals(iTargetFromSourcePorts, null))
                    {
                        iTargetFromSourcePorts = new int[nTargetPorts + 1];
                        for (var i = 1; i <= nTargetPorts; i++)
                        {
                            iTargetFromSourcePorts[i - 1] = System.Convert.ToInt32(i);
                        }
                    }

                    GenericStationTools[] supportedTools = jbc.GetStationTools(myID);
                    int iSourcePortNbr = 0;
                    int iTargetPortNbr = 0;
                    GenericStationTools curTool = default(GenericStationTools);

                    foreach (System.Xml.XmlNode portNode in portNodes)
                    {
                        iSourcePortNbr = int.Parse(portNode.Attributes[ConfigurationXML.xmlNumberId].Value);
                        // where to copy this source port (may be several target ports)
                        // look for source port in target ports array
                        for (var iTargetIdx = 0; iTargetIdx <= (iTargetFromSourcePorts.Length - 1); iTargetIdx++)
                        {
                            iTargetPortNbr = System.Convert.ToInt32(iTargetIdx + 1);
                            // copy source settings for each target port
                            if (iTargetFromSourcePorts[iTargetIdx] == iSourcePortNbr)
                            {
                                if (iTargetPortNbr <= nTargetPorts & iTargetPortNbr > 0)
                                {

                                    Debug.Print(string.Format("CONF Source port {0} to target port {1}", iSourcePortNbr.ToString(), iTargetPortNbr.ToString()));

                                    foreach (System.Xml.XmlNode portchildnode in portNode.ChildNodes)
                                    {
                                        Debug.Print(string.Format("CONF Source Port {0} PortChildNode {1}", iSourcePortNbr.ToString(), portchildnode.Name));
                                        if (portchildnode.Name == toolSelectedTempId)
                                        {
                                            // read "Checked" attribute
                                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked);
                                            // only change if checked or label does not exist in parameter
                                            if (!bXmlCheckedExists || bXmlChecked)
                                            {
                                                auxTemp = convertStringToTemp(portchildnode.InnerText, xmlTunits);
                                                if (auxTemp.isValid())
                                                {
                                                    await jbc.SetPortToolSelectedTempAsync(myID, (Port)(iTargetPortNbr - 1), auxTemp);
                                                }
                                            }
                                        }
                                        else if (portchildnode.Name == toolSelectedAirFlowId)
                                        {
                                            // read "Checked" attribute
                                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked);
                                            // only change if checked or label does not exist in parameter
                                            if (!bXmlCheckedExists || bXmlChecked)
                                            {
                                                await jbc.SetPortToolSelectedFlowAsync(myID, (Port)(iTargetPortNbr - 1), System.Convert.ToInt32(portchildnode.InnerText));
                                            }
                                        }
                                        else if (portchildnode.Name == toolExtTCTempId)
                                        {
                                            // read "Checked" attribute
                                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked);
                                            // only change if checked or label does not exist in parameter
                                            if (!bXmlCheckedExists || bXmlChecked)
                                            {
                                                auxTemp = convertStringToTemp(portchildnode.InnerText, xmlTunits);
                                                if (auxTemp.isValid())
                                                {
                                                    await jbc.SetPortToolSelectedExtTempAsync(myID, (Port)(iTargetPortNbr - 1), auxTemp);
                                                }
                                            }
                                        }
                                        else if (portchildnode.Name == toolProfilesId)
                                        {
                                            // read "Checked" attribute
                                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked);
                                            // only change if checked or label does not exist in parameter
                                            if (!bXmlCheckedExists || bXmlChecked)
                                            {
                                                await jbc.SetPortToolProfileModeAsync(myID, (Port)(iTargetPortNbr - 1), (OnOff)Convert.ToInt32(portchildnode.InnerText));
                                            }
                                        }
                                        else if (portchildnode.Name == xmlToolId)
                                        {
                                            //curTool = CType(CInt(portchildnode.Attributes.ItemOf(xmlNumberId).Value), JBC_API.GenericStationTools)
                                            curTool = xmlGetToolFromToolNode(portchildnode);

                                            foreach (System.Xml.XmlNode toolparamNode in portchildnode.ChildNodes)
                                            {
                                                // read "Checked" attribute
                                                bXmlCheckedExists = ConfigurationXML.getXmlChecked(toolparamNode, ref bXmlChecked);
                                                // only change if checked or label does not exist in parameter
                                                if ((!bXmlCheckedExists) || bXmlChecked)
                                                {
                                                    Debug.Print(string.Format("CONF Source Port {0} Tool {1} Setting {2}", iSourcePortNbr.ToString(), curTool.ToString(), toolparamNode.Name));
                                                    if (toolparamNode.Name == toolAdjustTempId)
                                                    {
                                                        auxTemp = convertStringToTempAdj(toolparamNode.InnerText, xmlTunits);
                                                        await jbc.SetPortToolAdjustTempAsync(myID, (Port)(iTargetPortNbr - 1), curTool, auxTemp);
                                                    }
                                                    else if (toolparamNode.Name == toolTimeToStopId)
                                                    {
                                                        await jbc.SetPortToolTimeToStopAsync(myID, (Port)(iTargetPortNbr - 1), curTool, Convert.ToInt32(toolparamNode.InnerText));
                                                    }
                                                    else if (toolparamNode.Name == toolExtTCModeId)
                                                    {
                                                        await jbc.SetPortToolExternalTCModeAsync(myID, (Port)(iTargetPortNbr - 1), (GenericStationTools)curTool, (ToolExternalTCMode_HA)Convert.ToInt32(toolparamNode.InnerText));
                                                    }
                                                    else if (toolparamNode.Name == toolStartModeToolButtonId)
                                                    {
                                                        await jbc.SetPortToolStartModeAsync(myID,
                                                                                            (Port)(iTargetPortNbr - 1),
                                                                                            curTool,
                                                                                            (OnOff)Convert.ToInt32(toolparamNode.InnerText),
                                                                                            jbc.GetPortToolStartMode_StandOut(myID, (Port)(iTargetPortNbr - 1), curTool),
                                                                                            jbc.GetPortToolStartMode_PedalAction(myID, (Port)(iTargetPortNbr - 1), curTool));
                                                    }
                                                    else if (toolparamNode.Name == toolStartModeStandOutId)
                                                    {
                                                        await jbc.SetPortToolStartModeAsync(myID,
                                                                                            (Port)(iTargetPortNbr - 1),
                                                                                            curTool,
                                                                                            jbc.GetPortToolStartMode_ToolButton(myID, (Port)(iTargetPortNbr - 1), curTool),
                                                                                            (OnOff)Convert.ToInt32(toolparamNode.InnerText),
                                                                                            jbc.GetPortToolStartMode_PedalAction(myID, (Port)(iTargetPortNbr - 1), curTool));
                                                    }
                                                    else if (toolparamNode.Name == toolStartModePedalActionId)
                                                    {
                                                        await jbc.SetPortToolStartModeAsync(myID,
                                                                                            (Port)(iTargetPortNbr - 1),
                                                                                            curTool,
                                                                                            jbc.GetPortToolStartMode_ToolButton(myID, (Port)(iTargetPortNbr - 1), curTool),
                                                                                            jbc.GetPortToolStartMode_StandOut(myID, (Port)(iTargetPortNbr - 1), curTool),
                                                                                            (PedalAction)Convert.ToInt32(toolparamNode.InnerText));
                                                    }

                                                } // If (Not bXmlCheckedExists) Or bXmlChecked
                                            } // tool child nodes
                                        } // port child node
                                    } // chidren port node
                                } // If iTargetPort <= nTargetPorts And iTargetPort > 0
                            } // iTargetFromSourcePorts(iTarget) = iSourcePort
                        } // For iTarget = 0 To UBound(iTargetFromSourcePorts)
                    } // portNodes
                }

                //Robot
                System.Xml.XmlNode robotNode = xmlGetRobotNodeFromXml(xmlDoc);
                if (robotNode != null)
                {
                    CRobotData robotData = jbc.GetRobotConfiguration(myID);

                    foreach (System.Xml.XmlNode robotchildnode in robotNode.ChildNodes)
                    {
                        if (robotchildnode.Name == stnRbtStatusId)
                        {
                            // read "Checked" attribute
                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(robotchildnode, ref bXmlChecked);
                            // only change if checked or label does not exist in parameter
                            if (!bXmlCheckedExists || bXmlChecked)
                            {
                                robotData.Status = (OnOff)Convert.ToInt32(robotchildnode.InnerText);
                            }
                        }
                        else if (robotchildnode.Name == stnRbtProtocolId)
                        {
                            // read "Checked" attribute
                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(robotchildnode, ref bXmlChecked);
                            // only change if checked or label does not exist in parameter
                            if (!bXmlCheckedExists || bXmlChecked)
                            {
                                robotData.Protocol = (CRobotData.RobotProtocol)Convert.ToInt32(robotchildnode.InnerText);
                            }
                        }
                        else if (robotchildnode.Name == stnRbtAddressId)
                        {
                            // read "Checked" attribute
                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(robotchildnode, ref bXmlChecked);
                            // only change if checked or label does not exist in parameter
                            if (!bXmlCheckedExists || bXmlChecked)
                            {
                                robotData.Address = System.Convert.ToUInt16(robotchildnode.InnerText);
                            }
                        }
                        else if (robotchildnode.Name == stnRbtSpeedId)
                        {
                            // read "Checked" attribute
                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(robotchildnode, ref bXmlChecked);
                            // only change if checked or label does not exist in parameter
                            if (!bXmlCheckedExists || bXmlChecked)
                            {
                                robotData.Speed = (CRobotData.RobotSpeed)Convert.ToInt32(robotchildnode.InnerText);
                            }
                        }
                        else if (robotchildnode.Name == stnRbtStopBitsId)
                        {
                            // read "Checked" attribute
                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(robotchildnode, ref bXmlChecked);
                            // only change if checked or label does not exist in parameter
                            if (!bXmlCheckedExists || bXmlChecked)
                            {
                                robotData.StopBits = (CRobotData.RobotStop)Convert.ToInt32(robotchildnode.InnerText);
                            }
                        }
                        else if (robotchildnode.Name == stnRbtParityId)
                        {
                            // read "Checked" attribute
                            bXmlCheckedExists = ConfigurationXML.getXmlChecked(robotchildnode, ref bXmlChecked);
                            // only change if checked or label does not exist in parameter
                            if (!bXmlCheckedExists || bXmlChecked)
                            {
                                robotData.Parity = (CRobotData.RobotParity)Convert.ToInt32(robotchildnode.InnerText);
                            }
                        }
                    }

                    await jbc.SetRobotConfigurationAsync(myID, robotData);
                }

                //Profiles
                System.Xml.XmlNode profileNode = xmlGetProfileNodeFromXml(xmlDoc);
                if (profileNode != null)
                {

                    //Delete profiles
                    for (int i = 0; i <= jbc.GetProfileCount(myID) - 1; i++)
                    {
                        string profileName = jbc.GetProfileName(myID, i);
                        jbc.DeleteProfile(myID, profileName);
                    }

                    foreach (System.Xml.XmlNode profilechildnode in profileNode.ChildNodes)
                    {
                        //TODO . save
                        //profilechildnode.Name
                        //profilechildnode.InnerText
                    }
                }

            }
            catch (Exception ex)
            {
                Interaction.MsgBox("Error setting xml configuration to station: " + ex.Message, MsgBoxStyle.Critical, null);
                return;
            }
        }

        public static System.Xml.XmlDocument confGetFromStation(long myID, JBC_API_Remote jbc, bool bCounters)
        {
            System.Xml.XmlDocument returnValue = default(System.Xml.XmlDocument);
            // it loads all settings to xml
            // specific data format saved in xml:
            //   stnTunitsId: saved as C or F (temp units viewed in the station)
            //   toolSelectedTempLvlId: saved as number of the enumeration
            //   on/off data: saved as 1 or 0 number of enum
            //   temperatures: saved according to the current value of the global Tunits variable (saved as confTempUnitsId attribute)
            // xml layout
            // <JBCstationParameters Version="1" lang="es">
            //   <Station stnModel="DD" TempUnits="°C">
            //       <StationSettings>
            //           station settings
            //       </StationSettings>
            //       <PortsAndTools>
            //           <Port Number="1">
            //              <toolSelectedTemp>350</toolSelectedTemp>
            //               <Tool Number="1" Type="T210">
            //                   <toolFixTemp>0</toolFixTemp>
            //                   ...toolsettings...
            //               </Tool>
            //               <Tool Number="2" Type="T245">
            //                   <toolFixTemp>0</toolFixTemp>
            //                   ...toolsettings...
            //               </Tool>
            //           </Port>
            //           <Port Number="2">
            //               ...
            //           </Port>
            //       </PortsAndTools>
            //       <Counters>
            //           <Port Number="1">
            //               ... counters ...
            //           </Port>
            //       </Counters
            //   </Station>
            // <JBCstationParameters>

            // New and Changed Features of source station
            //Dim features = New cls_Features(jbc.GetStationModel(myID), jbc.GetStationModelType(myID), jbc.GetStationModelVersion(myID), jbc.GetStationProtocol(myID))
            CFeaturesData features = jbc.GetStationFeatures(myID);

            System.Xml.XmlElement xmlRootElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlStationElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlStationSettingsElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlPortsAndToolsElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlPortElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlToolElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlCountersElem = default(System.Xml.XmlElement);
            string xmlTunits = "";
            CTemperature auxTemp = default(CTemperature);
            System.Xml.XmlNode xmlTempNode = default(System.Xml.XmlElement);

            xmlTunits = Tunits; // save temperatures as Tunits in xml data

            try
            {
                System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
                xmlDoc = RoutinesLibrary.Data.Xml.XMLUtils.CreateNewDoc();

                //root
                xmlRootElem = (System.Xml.XmlElement)RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlDoc, xmlRootId);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlVersionId, "2");
                // 2013/04/16 En archivos xml, se cambia el atributo "DateTime" por 2 atributos "Date" y "Time"
                //addAttrib(xmlRootElem, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlDateId, Strings.Format(DateTime.Now, xmlDateFormat));
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlTimeId, Strings.Format(DateTime.Now, xmlTimeFormat));

                //station
                xmlStationElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRootElem, xmlStationId);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlStationElem, stnModelId, jbc.GetStationModel(myID));
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlStationElem, xmlTempUnitsId, xmlTunits);

                //station settings
                xmlStationSettingsElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlStationSettingsId);
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnNameId, jbc.GetStationName(myID));
                if (features.DisplaySettings)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTunitsId, Convert.ToString(Convert.ToChar(jbc.GetStationTempUnits(myID))));
                }
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTmaxId, convertTempToString(jbc.GetStationMaxTemp(myID), false, true, xmlTunits));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTminId, convertTempToString(jbc.GetStationMinTemp(myID), false, true, xmlTunits));
                if (features.DisplaySettings)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnN2Id, Convert.ToString(jbc.GetStationN2Mode(myID)));
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnHelpId, Convert.ToString(jbc.GetStationHelpText(myID)));
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnBeepId, Convert.ToString(jbc.GetStationBeep(myID)));
                }
                // 13/01/2014
                //xmlAddNode(xmlStationSettingsElem, stnPwrLimitId, jbc.GetStationPowerLimit(myID))
                // info settings
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnSWId, jbc.GetStationSWversion(myID));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnProtocolId, jbc.GetStationProtocol(myID));
                //xmlAddNode(xmlStationSettingsElem, stnTrafoErrId, convertTempToString(jbc.GetStationTransformerErrorTemp(myID), False, True, xmlTunits))
                //xmlAddNode(xmlStationSettingsElem, stnMOSErrId, convertTempToString(jbc.GetStationMOSerrorTemp(myID), False, True, xmlTunits))
                // PIN
                //addNode(xmlStationSettingsElem, stnPINId, jbc.GetStationPIN(myID))

                //ports and tools
                xmlPortsAndToolsElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlPortsAndToolsId);

                int nPorts = jbc.GetPortCount(myID);
                GenericStationTools[] tools = jbc.GetStationTools(myID);
                for (int p = 1; p <= nPorts; p++)
                {
                    //port
                    xmlPortElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortsAndToolsElem, xmlPortId);
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlNumberId, p.ToString());
                    //selected Temp
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolSelectedTempId, convertTempToString(jbc.GetPortToolSelectedTemp(myID, (Port)(p - 1)), false, true, xmlTunits));
                    //tools
                    foreach (GenericStationTools t in tools)
                    {
                        // tool
                        xmlToolElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, xmlToolId);
                        // 2013/04/15 Se quita el atributo "Number" de "Tool"
                        //addAttrib(xmlToolElem, xmlNumberId, Convert.ToUInt32(t))
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlToolElem, ConfigurationXML.xmlTypeId, t.ToString());
                        // tool settings
                        if (!features.TempLevelsWithStatus)
                        {
                            auxTemp = jbc.GetPortToolFixTemp(myID, (Port)(p - 1), t);
                            if (auxTemp.UTI == Constants.NO_TEMP_LEVEL | auxTemp.UTI == 0)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolFixTempId, noDataStr);
                            }
                            else
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolFixTempId, convertTempToString(auxTemp, false, true, xmlTunits));
                            }
                        }
                        // selected level
                        // save as ToString of enum
                        //addNode(xmlDoc, xmlToolElem, toolSelectedTempLvlId, jbc.GetPortToolSelectedTempLevels(myID, CType(p - 1, JBC_API.Port), t).ToString)
                        // save as number of enum
                        xmlTempNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolSelectedTempLvlId, Convert.ToString(jbc.GetPortToolSelectedTempLevels(myID, (Port)(p - 1), t)));
                        if (features.TempLevelsWithStatus)
                        {
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlTempNode, ConfigurationXML.xmlEnabledId, Convert.ToBoolean(jbc.GetPortToolSelectedTempLevelsEnabled(myID, (Port)(p - 1), t)).ToString());
                        }
                        // temp level 1
                        xmlTempNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTempLvl1Id, convertTempToString(jbc.GetPortToolTempLevel(myID, (Port)(p - 1), t, ToolTemperatureLevels.FIRST_LEVEL), false, true, xmlTunits));
                        if (features.TempLevelsWithStatus)
                        {
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlTempNode, ConfigurationXML.xmlEnabledId, System.Convert.ToBoolean(jbc.GetPortToolTempLevelEnabled(myID, (Port)(p - 1), t, ToolTemperatureLevels.FIRST_LEVEL)).ToString());
                        }
                        // temp level 2
                        xmlTempNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTempLvl2Id, convertTempToString(jbc.GetPortToolTempLevel(myID, (Port)(p - 1), t, ToolTemperatureLevels.SECOND_LEVEL), false, true, xmlTunits));
                        if (features.TempLevelsWithStatus)
                        {
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlTempNode, ConfigurationXML.xmlEnabledId, System.Convert.ToBoolean(jbc.GetPortToolTempLevelEnabled(myID, (Port)(p - 1), t, ToolTemperatureLevels.SECOND_LEVEL)).ToString());
                        }
                        // temp level 3
                        xmlTempNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTempLvl3Id, convertTempToString(jbc.GetPortToolTempLevel(myID, (Port)(p - 1), t, ToolTemperatureLevels.THIRD_LEVEL), false, true, xmlTunits));
                        if (features.TempLevelsWithStatus)
                        {
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlTempNode, ConfigurationXML.xmlEnabledId, System.Convert.ToBoolean(jbc.GetPortToolTempLevelEnabled(myID, (Port)(p - 1), t, ToolTemperatureLevels.THIRD_LEVEL)).ToString());
                        }
                        // sleep temp
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolSleepTempId, convertTempToString(jbc.GetPortToolSleepTemp(myID, (Port)(p - 1), t), false, true, xmlTunits));
                        // sleep delay
                        xmlTempNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolSleepDelayId, Convert.ToString(jbc.GetPortToolSleepDelay(myID, (Port)(p - 1), t)));
                        if (features.DelayWithStatus)
                        {
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlTempNode, ConfigurationXML.xmlEnabledId, System.Convert.ToBoolean(jbc.GetPortToolSleepDelayEnabled(myID, (Port)(p - 1), t)).ToString());
                        }
                        // hiber delay
                        xmlTempNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolHibernationDelayId, Convert.ToString(jbc.GetPortToolHibernationDelay(myID, (Port)(p - 1), t)));
                        if (features.DelayWithStatus)
                        {
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlTempNode, ConfigurationXML.xmlEnabledId, System.Convert.ToBoolean(jbc.GetPortToolHibernationDelayEnabled(myID, (Port)(p - 1), t)).ToString());
                        }
                        // adjust temp
                        auxTemp = jbc.GetPortToolAdjustTemp(myID, (Port)(p - 1), t);
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolAdjustTempId, convertTempAdjToString(auxTemp, false, xmlTunits));
                    }
                }

                if (bCounters)
                {
                    // global counters
                    xmlCountersElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlCountersId);
                    //addAttrib(xmlCountersElem, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))

                    for (int p = 1; p <= nPorts; p++)
                    {
                        //port
                        xmlPortElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlCountersElem, xmlPortId, null);
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlNumberId, p.ToString());

                        // global counters
                        // minutes
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterPluggedMinutesId, Convert.ToString(jbc.GetPortToolPluggedMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterWorkMinutesId, Convert.ToString(jbc.GetPortToolWorkMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterSleepMinutesId, Convert.ToString(jbc.GetPortToolSleepMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterHiberMinutesId, Convert.ToString(jbc.GetPortToolHibernationMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterNoToolMinutesId, Convert.ToString(jbc.GetPortToolIdleMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        // cycles
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterSleepCyclesId, Convert.ToString(jbc.GetPortToolSleepCycles(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterDesolderCyclesId, Convert.ToString(jbc.GetPortToolDesolderCycles(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                    }

                    // partial counters
                    if (features.PartialCounters)
                    {
                        xmlCountersElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlPartialCountersId, null);
                        //addAttrib(xmlCountersElem, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))

                        for (int p = 1; p <= nPorts; p++)
                        {
                            //port
                            xmlPortElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlCountersElem, xmlPortId, null);
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlNumberId, p.ToString());

                            // partial counters
                            // minutes
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterPluggedMinutesId, Convert.ToString(jbc.GetPortToolPluggedMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterWorkMinutesId, Convert.ToString(jbc.GetPortToolWorkMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterSleepMinutesId, Convert.ToString(jbc.GetPortToolSleepMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterHiberMinutesId, Convert.ToString(jbc.GetPortToolHibernationMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterNoToolMinutesId, Convert.ToString(jbc.GetPortToolIdleMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            // cycles
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterSleepCyclesId, Convert.ToString(jbc.GetPortToolSleepCycles(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterDesolderCyclesId, Convert.ToString(jbc.GetPortToolDesolderCycles(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                        }
                    }
                }

                returnValue = xmlDoc;

            }
            catch (Exception ex)
            {
                returnValue = null;
                Interaction.MsgBox("Error converting station settings to xml: " + ex.Message, MsgBoxStyle.Critical, null);
                return returnValue;
            }
            return returnValue;
        }

        public static System.Xml.XmlDocument confGetFromStation_HA(long myID, JBC_API_Remote jbc, bool bCounters)
        {
            System.Xml.XmlDocument returnValue = default(System.Xml.XmlDocument);
            // it loads all settings to xml
            // specific data format saved in xml:
            //   stnTunitsId: saved as C or F (temp units viewed in the station)
            //   toolSelectedTempLvlId: saved as number of the enumeration
            //   on/off data: saved as 1 or 0 number of enum
            //   temperatures: saved according to the current value of the global Tunits variable (saved as confTempUnitsId attribute)
            // xml layout
            // <JBCstationParameters Version="1" lang="es">
            //   <Station stnModel="DD" TempUnits="°C">
            //       <StationSettings>
            //           station settings
            //       </StationSettings>
            //       <PortsAndTools>
            //           <Port Number="1">
            //              <toolSelectedTemp>350</toolSelectedTemp>
            //               <Tool Number="1" Type="T210">
            //                   <toolFixTemp>0</toolFixTemp>
            //                   ...toolsettings...
            //               </Tool>
            //               <Tool Number="2" Type="T245">
            //                   <toolFixTemp>0</toolFixTemp>
            //                   ...toolsettings...
            //               </Tool>
            //           </Port>
            //           <Port Number="2">
            //               ...
            //           </Port>
            //       </PortsAndTools>
            //       <Counters>
            //           <Port Number="1">
            //               ... counters ...
            //           </Port>
            //       </Counters
            //   </Station>
            // <JBCstationParameters>

            // New and Changed Features of source station
            //Dim features = New cls_Features(jbc.GetStationModel(myID), jbc.GetStationModelType(myID), jbc.GetStationModelVersion(myID), jbc.GetStationProtocol(myID))
            CFeaturesData features = jbc.GetStationFeatures(myID);

            System.Xml.XmlNode xmlRootElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlStationElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlStationSettingsElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlPortsAndToolsElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlPortElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlToolElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlCountersElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlRobotElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlProfileElem = default(System.Xml.XmlElement);
            string xmlTunits = "";
            CTemperature auxTemp = default(CTemperature);

            xmlTunits = Tunits; // save temperatures as Tunits in xml data

            try
            {
                System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
                xmlDoc = RoutinesLibrary.Data.Xml.XMLUtils.CreateNewDoc();

                //root
                xmlRootElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlDoc, xmlRootId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlVersionId, "2");
                // 2013/04/16 En archivos xml, se cambia el atributo "DateTime" por 2 atributos "Date" y "Time"
                //addAttrib(xmlRootElem, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlDateId, Strings.Format(DateTime.Now, xmlDateFormat));
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlTimeId, Strings.Format(DateTime.Now, xmlTimeFormat));

                //station
                xmlStationElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRootElem, xmlStationId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlStationElem, stnModelId, jbc.GetStationModel(myID));
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlStationElem, xmlTempUnitsId, xmlTunits);

                //station settings
                xmlStationSettingsElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlStationSettingsId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnNameId, jbc.GetStationName(myID));
                if (features.DisplaySettings)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTunitsId, System.Convert.ToString(Convert.ToChar(jbc.GetStationTempUnits(myID))));
                }
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTmaxId, convertTempToString(jbc.GetStationMaxTemp(myID), false, true, xmlTunits));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTminId, convertTempToString(jbc.GetStationMinTemp(myID), false, true, xmlTunits));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTExtmaxId, convertTempToString(jbc.GetStationMaxExtTemp(myID), false, true, xmlTunits));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTExtminId, convertTempToString(jbc.GetStationMinExtTemp(myID), false, true, xmlTunits));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnFlowmaxId, System.Convert.ToString(jbc.GetStationMaxFlow(myID)));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnFlowminId, System.Convert.ToString(jbc.GetStationMinFlow(myID)));
                if (features.DisplaySettings)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnBeepId, Convert.ToString(jbc.GetStationBeep(myID)));
                }
                // 13/01/2014
                //xmlAddNode(xmlStationSettingsElem, stnPwrLimitId, jbc.GetStationPowerLimit(myID))
                // info settings
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnSWId, jbc.GetStationSWversion(myID));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnProtocolId, jbc.GetStationProtocol(myID));
                //xmlAddNode(xmlStationSettingsElem, stnTrafoErrId, convertTempToString(jbc.GetStationTransformerErrorTemp(myID), False, True, xmlTunits))
                //xmlAddNode(xmlStationSettingsElem, stnMOSErrId, convertTempToString(jbc.GetStationMOSerrorTemp(myID), False, True, xmlTunits))
                // PIN
                //addNode(xmlStationSettingsElem, stnPINId, jbc.GetStationPIN(myID))

                //ports and tools
                xmlPortsAndToolsElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlPortsAndToolsId, null);

                int nPorts = jbc.GetPortCount(myID);
                GenericStationTools[] tools = jbc.GetStationTools(myID);
                for (int p = 1; p <= nPorts; p++)
                {
                    //port
                    xmlPortElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortsAndToolsElem, xmlPortId, null);
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlNumberId, p.ToString());
                    //selected Temp
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolSelectedTempId, convertTempToString(jbc.GetPortToolSelectedTemp(myID, (Port)(p - 1)), false, true, xmlTunits));
                    //selected Air flow
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolSelectedAirFlowId, Convert.ToString(jbc.GetPortToolSelectedFlow(myID, (Port)(p - 1))));
                    //selected ext temp
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolExtTCTempId, convertTempToString(jbc.GetPortToolSelectedExtTemp(myID, (Port)(p - 1)), false, true, xmlTunits));
                    //profiles mode
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolProfilesId, Convert.ToString(jbc.GetPortToolProfileMode(myID, (Port)(p - 1))));
                    //tools
                    foreach (GenericStationTools t in tools)
                    {
                        // tool
                        xmlToolElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, xmlToolId, null);
                        // 2013/04/15 Se quita el atributo "Number" de "Tool"
                        //addAttrib(xmlToolElem, xmlNumberId, Convert.ToUInt32(t))
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlToolElem, ConfigurationXML.xmlTypeId, t.ToString());
                        // tool settings
                        if (!features.TempLevelsWithStatus)
                        {
                            auxTemp = jbc.GetPortToolFixTemp(myID, (Port)(p - 1), t);
                            if (auxTemp.UTI == Constants.NO_TEMP_LEVEL | auxTemp.UTI == 0)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolFixTempId, noDataStr);
                            }
                            else
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolFixTempId, convertTempToString(auxTemp, false, true, xmlTunits));
                            }
                        }
                        //adjust temp
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolAdjustTempId, convertTempAdjToString(jbc.GetPortToolAdjustTemp(myID, (Port)(p - 1), t), false, xmlTunits));
                        //time to stop
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTimeToStopId, Convert.ToString(jbc.GetPortToolTimeToStop(myID, (Port)(p - 1), t)));
                        //ext TC mode
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolExtTCModeId, Convert.ToString(jbc.GetPortToolExternalTCMode(myID, (Port)(p - 1), t)));
                        //start mode - button
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolStartModeToolButtonId, Convert.ToString(jbc.GetPortToolStartMode_ToolButton(myID, (Port)(p - 1), t)));
                        //start mode - auto
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolStartModeStandOutId, Convert.ToString(jbc.GetPortToolStartMode_StandOut(myID, (Port)(p - 1), t)));
                        //start mode - pedal
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolStartModePedalActionId, Convert.ToString(jbc.GetPortToolStartMode_PedalAction(myID, (Port)(p - 1), t)));
                    }
                }

                if (bCounters)
                {

                    // global counters
                    xmlCountersElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlCountersId);
                    //addAttrib(xmlCountersElem, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))

                    for (int p = 1; p <= nPorts; p++)
                    {
                        //port
                        xmlPortElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlCountersElem, xmlPortId, null);
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlNumberId, p.ToString());

                        // global counters
                        // minutes
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterPluggedMinutesId, System.Convert.ToString(jbc.GetPortToolPluggedMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterWorkMinutesId, System.Convert.ToString(jbc.GetPortToolWorkMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterNoToolMinutesId, System.Convert.ToString(jbc.GetPortToolIdleMinutes(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        // cycles
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterSleepCyclesId, System.Convert.ToString(jbc.GetPortToolWorkCycles(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterDesolderCyclesId, System.Convert.ToString(jbc.GetPortToolSuctionCycles(myID, (Port)(p - 1), CounterTypes.GLOBAL_COUNTER)));
                    }

                    // partial counters
                    if (features.PartialCounters)
                    {
                        xmlCountersElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlPartialCountersId, null);
                        //addAttrib(xmlCountersElem, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))

                        for (int p = 1; p <= nPorts; p++)
                        {
                            //port
                            xmlPortElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlCountersElem, xmlPortId, null);
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlNumberId, p.ToString());

                            // partial counters
                            // minutes
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterPluggedMinutesId, Convert.ToString(jbc.GetPortToolPluggedMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterWorkMinutesId, Convert.ToString(jbc.GetPortToolWorkMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterNoToolMinutesId, Convert.ToString(jbc.GetPortToolIdleMinutes(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            // cycles
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterSleepCyclesId, Convert.ToString(jbc.GetPortToolWorkCycles(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, counterDesolderCyclesId, Convert.ToString(jbc.GetPortToolSuctionCycles(myID, (Port)(p - 1), CounterTypes.PARTIAL_COUNTER)));
                        }
                    }
                }

                //Robot
                xmlRobotElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlRobotId);
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRobotElem, stnRbtStatusId, Convert.ToString(jbc.GetRobotConfiguration(myID).Status));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRobotElem, stnRbtProtocolId, Convert.ToString(jbc.GetRobotConfiguration(myID).Protocol));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRobotElem, stnRbtAddressId, Convert.ToString(jbc.GetRobotConfiguration(myID).Address));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRobotElem, stnRbtSpeedId, Convert.ToString(jbc.GetRobotConfiguration(myID).Speed));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRobotElem, stnRbtDataBitsId, Convert.ToString(jbc.GetRobotConfiguration(myID).DataBits));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRobotElem, stnRbtStopBitsId, Convert.ToString(jbc.GetRobotConfiguration(myID).StopBits));
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRobotElem, stnRbtParityId, Convert.ToString(jbc.GetRobotConfiguration(myID).Parity));

                //Profile
                xmlProfileElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlProfileId);
                for (int i = 0; i <= jbc.GetProfileCount(myID) - 1; i++)
                {
                    string profileName = jbc.GetProfileName(myID, i);
                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlProfileElem, profileName, System.Text.Encoding.ASCII.GetString(jbc.GetProfile(myID, profileName)));
                }

                returnValue = xmlDoc;

            }
            catch (Exception ex)
            {
                returnValue = null;
                Interaction.MsgBox("Error converting station settings to xml: " + ex.Message, MsgBoxStyle.Critical, null);
            }
            return returnValue;
        }

        public static ParamTree confXmlToParamTree(System.Xml.XmlDocument xmlDoc, ref string sComment)
        {
            ParamTree returnValue = default(ParamTree);
            ParamTree oTree = null;
            System.Xml.XmlNode nodeTemp = default(System.Xml.XmlNode);
            string sValue = "";
            TreeNode oStationNode = default(TreeNode);
            TreeNode oParentNode = default(TreeNode);
            TreeNode oPortNode = default(TreeNode);
            TreeNode oToolNode = default(TreeNode);
            TreeNode oTreeNode = default(TreeNode);
            int iPort = 0;
            string sToolName = "";
            GenericStationTools curTool = default(GenericStationTools);
            //Dim auxTemp As Ctemperature
            string xmlTunits = CELSIUS_STR; // default Celsius
            string treeTunits = Tunits; // display units in tree
            bool bXmlChecked = false;
            bool bXmlEnabled = false;
            string sModel = "";

            string[] OnOff = getOnOff(arrOption.VALUES);
            string[] OnOffText = getOnOff(arrOption.TEXTS);

            string[] tempUnits = new string[] { Convert.ToString(CTemperature.TemperatureUnit.Celsius), Convert.ToString(CTemperature.TemperatureUnit.Fahrenheit) };
            string[] tempUnitsText = new string[] { Localization.getResStr(stnTunitsCelsiusId), Localization.getResStr(stnTunitsFahrenheitId) };

            string[] tempLvlsValues = getTempLevels(arrOption.VALUES, false);
            string[] tempLvls = getTempLevels(arrOption.TEXTS, false);

            string[] sleepDelaysValues = getSleepDelays(arrOption.VALUES, false);
            string[] sleepDelays = getSleepDelays(arrOption.TEXTS, false);

            string[] hiberDelaysValues = getHiberDelays(arrOption.VALUES, false);
            string[] hiberDelays = getHiberDelays(arrOption.TEXTS, false);

            string[] PedalActivation = getStartModePedalActivation(arrOption.VALUES);
            string[] PedalActivationText = getStartModePedalActivation(arrOption.TEXTS);

            returnValue = null;

            try
            {
                oTree = new ParamTree(true);

                //Dim configJBC As System.Xml.XmlNodeList = xmlDoc.SelectNodes("/" & confRootId)
                //if configJBC.Count > 0 then configVersion = configJBC(0).Attributes.ItemOf(confVersionId).Value

                sComment = "";
                nodeTemp = xmlDoc.SelectSingleNode("/" + xmlRootId + "/" + xmlCommentId);
                if (nodeTemp != null)
                {
                    sComment = nodeTemp.InnerText;
                }

                // model
                nodeTemp = xmlGetStationParamsNodeFromXml(xmlDoc);
                nodeTemp = nodeTemp.SelectSingleNode(stnModelId);
                if (nodeTemp != null)
                {
                    sModel = nodeTemp.InnerText;
                }

                // temperature limits
                CTemperature auxMaxTemp = getMaxTemp(sModel);
                string[] tempopt = new string[] { sReplaceTag + " " + treeTunits, convertTempToString(tempMin, false, true), convertTempToString(auxMaxTemp, false, true) };
                string[] tempoptFix = new string[] { sReplaceTag + " " + treeTunits, convertTempToString(tempMin, false, true), convertTempToString(auxMaxTemp, false, true), "0", noDataStr };
                string[] tempoptLvl = new string[] { sReplaceTag + " " + treeTunits, convertTempToString(tempMin, false, true), convertTempToString(auxMaxTemp, false, true), "", noDataStr };
                string[] tempoptAdj = new string[] { sReplaceTag + " " + treeTunits, convertTempToString(tempMinAdj, false, true), convertTempToString(tempMaxAdj, false, true), "", "0" };
                string[] pwropt = new string[] { sReplaceTag + " " + pwrUnitsStr };

                // "Station"
                iPort = 0;
                sToolName = "";
                nodeTemp = xmlGetStationNodeFromXml(xmlDoc);
                if (nodeTemp != null)
                {
                    sValue = nodeTemp.Attributes[stnModelId].Value;
                    oStationNode = oTree.addNode(null, stnModelId, iPort, sToolName, Localization.getResStr(stnModelId), sValue, ParamTree.cInputType.FIX_NODE, null, null);
                    if (ConfigurationXML.getXmlChecked(nodeTemp, ref bXmlChecked))
                    {
                        oStationNode.Checked = bXmlChecked;
                    }

                    // get temperature units in xml station node
                    xmlTunits = nodeTemp.Attributes[xmlTempUnitsId].Value;
                    // save xmlUnits in tree node attribute
                    oTree.addNodeAttribute(oStationNode, xmlTempUnitsId, xmlTunits);

                    // "Station/StationSettings"
                    nodeTemp = xmlGetStationParamsNodeFromXml(xmlDoc);
                    if (nodeTemp != null)
                    {
                        sValue = "";
                        oParentNode = oTree.addNode(oStationNode, xmlStationSettingsId, iPort, sToolName, Localization.getResStr(stnStationSettingsId), sValue, ParamTree.cInputType.NO_VALUE, null, null);
                        if (ConfigurationXML.getXmlChecked(nodeTemp, ref bXmlChecked))
                        {
                            oParentNode.Checked = bXmlChecked;
                        }

                        // station parameters
                        foreach (System.Xml.XmlNode node in nodeTemp.ChildNodes)
                        {
                            if (node.Name == stnModelId)
                            {
                                sValue = node.InnerText;
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.FIX, null, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnTunitsId)
                            {
                                //MsgBox("JBC_API.TemperatureUnits.CELSIUS:" & JBC_API.TemperatureUnits.CELSIUS & " " & _
                                //        "Asc(node.InnerText):" & Asc(node.InnerText))
                                sValue = node.InnerText; // in xml: C or F -node.InnerText-
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.SWITCH, tempUnits, tempUnitsText);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnTminId)
                            {
                                sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), false, true, treeTunits);
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnTmaxId)
                            {
                                sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), false, true, treeTunits);
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnTExtminId)
                            {
                                sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), false, true, treeTunits);
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnTExtmaxId)
                            {
                                sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), false, true, treeTunits);
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnFlowminId)
                            {
                                sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), false, true, treeTunits);
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnFlowmaxId)
                            {
                                sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), false, true, treeTunits);
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnBeepId)
                            {
                                sValue = node.InnerText;
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.SWITCH, OnOff, OnOffText);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnHelpId)
                            {
                                sValue = node.InnerText;
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.SWITCH, OnOff, OnOffText);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnN2Id)
                            {
                                sValue = node.InnerText;
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.SWITCH, OnOff, OnOffText);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                            else if (node.Name == stnPwrLimitId)
                            {
                                // 13/01/2014 do not nothing (be careful wuith the case else)
                                //sValue = Convert.ToUInt32(node.InnerText)
                                //oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, getResStr(node.Name), sValue, ParamTree.cInputType.NUMBER, pwropt)
                                //If getXmlChecked(node, bXmlChecked) Then oTreeNode.Checked = bXmlChecked
                            }
                            else if (node.Name == stnTrafoErrId)
                            {
                                //sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), False, True, treeTunits)
                                //oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, getResStr(node.Name), sValue, ParamTree.cInputType.FIX, tempopt)
                                //If getXmlChecked(node, bXmlChecked) Then oTreeNode.Checked = bXmlChecked
                            }
                            else if (node.Name == stnMOSErrId)
                            {
                                //sValue = convertTempToString(convertStringToTemp(node.InnerText, xmlTunits), False, True, treeTunits)
                                //oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, getResStr(node.Name), sValue, ParamTree.cInputType.FIX, tempopt)
                                //If getXmlChecked(node, bXmlChecked) Then oTreeNode.Checked = bXmlChecked
                            }
                            else if (node.Name == stnPINId)
                            {
                                // do not load PIN
                            }
                            else
                            {
                                sValue = node.InnerText;
                                oTreeNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(node.Name), sValue, ParamTree.cInputType.FIX, null, null);
                                if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                                {
                                    oTreeNode.Checked = bXmlChecked;
                                }
                            }
                        }
                    }

                    // "Station/PortsAndTools"
                    nodeTemp = xmlGetPortsAndToolsNodeFromXml(xmlDoc);
                    if (nodeTemp != null)
                    {
                        sValue = "";
                        oParentNode = oTree.addNode(oStationNode, xmlPortsAndToolsId, iPort, sToolName, Localization.getResStr(toolPortsAndToolsId), sValue, ParamTree.cInputType.NO_VALUE, null, null);
                        if (ConfigurationXML.getXmlChecked(nodeTemp, ref bXmlChecked))
                        {
                            oParentNode.Checked = bXmlChecked;
                        }

                        // ports
                        foreach (System.Xml.XmlNode node in nodeTemp.ChildNodes)
                        {
                            iPort = int.Parse(node.Attributes[ConfigurationXML.xmlNumberId].Value);
                            sToolName = "";
                            sValue = (iPort).ToString();
                            oPortNode = oTree.addNode(oParentNode, node.Name, iPort, sToolName, Localization.getResStr(toolPortId), sValue, ParamTree.cInputType.FIX_NODE, null, null);
                            if (ConfigurationXML.getXmlChecked(node, ref bXmlChecked))
                            {
                                oPortNode.Checked = bXmlChecked;
                            }

                            foreach (System.Xml.XmlNode portchildnode in node.ChildNodes)
                            {
                                if (portchildnode.Name == toolSelectedTempId)
                                {
                                    sValue = convertTempToString(convertStringToTemp(portchildnode.InnerText, xmlTunits), false, true, treeTunits);
                                    oTreeNode = oTree.addNode(oPortNode, portchildnode.Name, iPort, sToolName, Localization.getResStr(portchildnode.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                    if (ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked))
                                    {
                                        oTreeNode.Checked = bXmlChecked;
                                    }
                                }
                                else if (portchildnode.Name == toolSelectedAirFlowId)
                                {
                                    sValue = convertTempToString(convertStringToTemp(portchildnode.InnerText, xmlTunits), false, true, treeTunits);
                                    oTreeNode = oTree.addNode(oPortNode, portchildnode.Name, iPort, sToolName, Localization.getResStr(portchildnode.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                    if (ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked))
                                    {
                                        oTreeNode.Checked = bXmlChecked;
                                    }
                                }
                                else if (portchildnode.Name == toolExtTCTempId)
                                {
                                    sValue = convertTempToString(convertStringToTemp(portchildnode.InnerText, xmlTunits), false, true, treeTunits);
                                    oTreeNode = oTree.addNode(oPortNode, portchildnode.Name, iPort, sToolName, Localization.getResStr(portchildnode.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                    if (ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked))
                                    {
                                        oTreeNode.Checked = bXmlChecked;
                                    }
                                }
                                else if (portchildnode.Name == toolProfilesId)
                                {
                                    sValue = portchildnode.InnerText;
                                    oTreeNode = oTree.addNode(oPortNode, portchildnode.Name, iPort, sToolName, Localization.getResStr(portchildnode.Name), sValue, ParamTree.cInputType.SWITCH, OnOff, OnOffText);
                                    if (ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked))
                                    {
                                        oTreeNode.Checked = bXmlChecked;
                                    }
                                }
                                else if (portchildnode.Name == xmlToolId)
                                {
                                    //curTool = CType(CInt(portchildnode.Attributes.ItemOf(xmlNumberId).Value), JBC_API.GenericStationTools)
                                    curTool = xmlGetToolFromToolNode(portchildnode);
                                    sValue = curTool.ToString();
                                    sToolName = curTool.ToString();
                                    oToolNode = oTree.addNode(oPortNode, portchildnode.Name, iPort, sToolName, Localization.getResStr(toolToolId), sValue, ParamTree.cInputType.FIX_NODE, null, null);
                                    if (ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked))
                                    {
                                        oToolNode.Checked = bXmlChecked;
                                    }

                                    foreach (System.Xml.XmlNode toolNode in portchildnode.ChildNodes)
                                    {
                                        if (toolNode.Name == toolFixTempId)
                                        {
                                            if (toolNode.InnerText == noDataStr || toolNode.InnerText == "0")
                                            {
                                                sValue = noDataStr;
                                            }
                                            else
                                            {
                                                sValue = convertTempToString(convertStringToTemp(toolNode.InnerText, xmlTunits), false, true, treeTunits);
                                            }
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.NUMBER, tempoptFix, null);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                        }
                                        else if (toolNode.Name == toolSelectedTempLvlId)
                                        {
                                            // if saved as ToString of enum
                                            //sValue = System.Enum.Parse(GetType(ToolTemperatureLevels), toolNode.InnerText)
                                            // if saved as number of enum
                                            sValue = toolNode.InnerText.Trim();
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.SWITCH, tempLvlsValues, tempLvls);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                            if (ConfigurationXML.getXmlEnabled(toolNode, ref bXmlEnabled))
                                            {
                                                //If bXmlEnabled Then sValue = OnOff(0) Else sValue = OnOff(1)
                                                //oTreeNodeEnabled = oTree.addNode(oTreeNode, xmlEnabledId, iPort, sToolName, getResStr(gralEnabledId), sValue, ParamTree.cInputType.SWITCH, OnOff, OnOffText)
                                                if (bXmlEnabled)
                                                {
                                                    sValue = ParamTree.EnabledTrue;
                                                }
                                                else
                                                {
                                                    sValue = ParamTree.EnabledFalse;
                                                }
                                                oTree.addNodeAttribute(oTreeNode, ParamTree.EnabledAttribName, sValue);
                                            }
                                        }
                                        else if (((toolNode.Name == toolTempLvl1Id) || (toolNode.Name == toolTempLvl2Id)) || (toolNode.Name == toolTempLvl3Id))
                                        {
                                            if (toolNode.InnerText == noDataStr)
                                            {
                                                sValue = noDataStr;
                                            }
                                            else
                                            {
                                                sValue = convertTempToString(convertStringToTemp(toolNode.InnerText, xmlTunits), false, true, treeTunits);
                                            }
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.NUMBER, tempoptLvl, null);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                            if (ConfigurationXML.getXmlEnabled(toolNode, ref bXmlEnabled))
                                            {
                                                if (bXmlEnabled)
                                                {
                                                    sValue = ParamTree.EnabledTrue;
                                                }
                                                else
                                                {
                                                    sValue = ParamTree.EnabledFalse;
                                                }
                                                oTree.addNodeAttribute(oTreeNode, ParamTree.EnabledAttribName, sValue);
                                            }
                                        }
                                        else if (toolNode.Name == toolSleepDelayId)
                                        {
                                            sValue = toolNode.InnerText;
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.SWITCH, sleepDelaysValues, sleepDelays);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                            if (ConfigurationXML.getXmlEnabled(toolNode, ref bXmlEnabled))
                                            {
                                                if (bXmlEnabled)
                                                {
                                                    sValue = ParamTree.EnabledTrue;
                                                }
                                                else
                                                {
                                                    sValue = ParamTree.EnabledFalse;
                                                }
                                                oTree.addNodeAttribute(oTreeNode, ParamTree.EnabledAttribName, sValue);
                                            }
                                        }
                                        else if (toolNode.Name == toolHibernationDelayId)
                                        {
                                            sValue = toolNode.InnerText;
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.SWITCH, hiberDelaysValues, hiberDelays);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                            if (ConfigurationXML.getXmlEnabled(toolNode, ref bXmlEnabled))
                                            {
                                                if (bXmlEnabled)
                                                {
                                                    sValue = ParamTree.EnabledTrue;
                                                }
                                                else
                                                {
                                                    sValue = ParamTree.EnabledFalse;
                                                }
                                                oTree.addNodeAttribute(oTreeNode, ParamTree.EnabledAttribName, sValue);
                                            }
                                        }
                                        else if (toolNode.Name == toolSleepTempId)
                                        {
                                            if (toolNode.InnerText == noDataStr)
                                            {
                                                sValue = noDataStr;
                                            }
                                            else
                                            {
                                                sValue = convertTempToString(convertStringToTemp(toolNode.InnerText, xmlTunits), false, true, treeTunits);
                                            }
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.NUMBER, tempopt, null);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                        }
                                        else if (toolNode.Name == toolAdjustTempId)
                                        {
                                            if (toolNode.InnerText == noDataStr)
                                            {
                                                sValue = noDataStr;
                                            }
                                            else
                                            {
                                                sValue = convertTempAdjToString(convertStringToTempAdj(toolNode.InnerText, xmlTunits), false, treeTunits);
                                            }
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.NUMBER, tempoptAdj, null);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                        }
                                        else if (toolNode.Name == toolTimeToStopId)
                                        {
                                            oTreeNode = oTree.addNode(oToolNode, portchildnode.Name, iPort, sToolName, Localization.getResStr(portchildnode.Name), toolNode.InnerText, ParamTree.cInputType.NUMBER, null, null);
                                            if (ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                        }
                                        else if (toolNode.Name == toolStartModeToolButtonId)
                                        {
                                            sValue = toolNode.InnerText;
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.SWITCH, OnOff, OnOffText);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                        }
                                        else if (toolNode.Name == toolStartModeStandOutId)
                                        {
                                            sValue = toolNode.InnerText;
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.SWITCH, OnOff, OnOffText);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                        }
                                        else if (toolNode.Name == toolStartModePedalActionId)
                                        {
                                            sValue = toolNode.InnerText;
                                            oTreeNode = oTree.addNode(oToolNode, toolNode.Name, iPort, sToolName, Localization.getResStr(toolNode.Name), sValue, ParamTree.cInputType.SWITCH, PedalActivation, PedalActivationText);
                                            if (ConfigurationXML.getXmlChecked(toolNode, ref bXmlChecked))
                                            {
                                                oTreeNode.Checked = bXmlChecked;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Interaction.MsgBox("Error converting xml configuration to tree: " + ex.Message, MsgBoxStyle.Critical, null);
                return returnValue;
            }

            returnValue = oTree;
            return returnValue;
        }

        public static System.Xml.XmlDocument confParamTreeToXml(ParamTree oTree, bool bAddCheckedAttribute, string sComment)
        {
            System.Xml.XmlDocument returnValue = default(System.Xml.XmlDocument);
            System.Xml.XmlNode xmlRootElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlStationElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlStationSettingsElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlPortsAndToolsElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlPortElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlToolElem = default(System.Xml.XmlElement);
            System.Xml.XmlNode xmlNode = default(System.Xml.XmlElement);
            TreeNode node = null;
            TreeNode retNode = null;
            //Dim retNodeEnabled As TreeNode = Nothing
            TreeNode nodePortsAndTools = default(TreeNode);
            TreeNode nodePort = default(TreeNode);
            string sToolName = "";
            string sToolNbr;
            string xmlTunits = Tunits; // default Celsius
            string treeTunits = CELSIUS_STR; // displayed units in tree
            string sValue = "";
            bool bXmlEnabled = false;

            returnValue = null;

            string[] OnOff = getOnOff(arrOption.VALUES);

            xmlTunits = Tunits; // save temperatures as Tunits in xml Station nodes
            if (oTree.getAttribValue(null, 0, "", stnModelId, xmlTempUnitsId, ref sValue))
            {
                treeTunits = sValue; // get temperature units in tree
            }

            //Try
            System.Xml.XmlDocument xmlDoc = default(System.Xml.XmlDocument);
            xmlDoc = RoutinesLibrary.Data.Xml.XMLUtils.CreateNewDoc();

            //root
            xmlRootElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlDoc, xmlRootId);
            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlVersionId, "2");
            // 2013/04/16 En archivos xml, se cambia el atributo "DateTime" por 2 atributos "Date" y "Time"
            //addAttrib(xmlRootElem, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))
            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlDateId, Strings.Format(DateTime.Now, xmlDateFormat));
            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlRootElem, ConfigurationXML.xmlTimeId, Strings.Format(DateTime.Now, xmlTimeFormat));

            //comment
            sComment = sComment.Trim();
            if (sComment != "")
            {
                RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRootElem, xmlCommentId, sComment);
            }

            //station
            xmlStationElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlRootElem, xmlStationId, null);
            if (oTree.getValue(null, 0, "", stnModelId, ref sValue, ref retNode))
            {
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlStationElem, stnModelId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlStationElem, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlStationElem, xmlTempUnitsId, xmlTunits);

            //station settings
            xmlStationSettingsElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlStationSettingsId, null);
            if (oTree.getValue(null, 0, "", stnNameId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnNameId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            //if oTree.getValue(Nothing, 0, "", stnModelId, sValue, retNode) then
            //    xmlNode = addNode(xmlStationSettingsElem, stnModelId, sValue)
            //    If bAddCheckedAttribute Then addAttrib(xmlNode, xmlCheckedId, retNode.Checked.ToString)
            //end if
            if (oTree.getValue(null, 0, "", stnTunitsId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTunitsId, System.Convert.ToString(Convert.ToChar(int.Parse(sValue)))); // save as C or F
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnTminId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTminId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnTmaxId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTmaxId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnTExtminId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTExtminId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnTExtmaxId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnTExtmaxId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnFlowminId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnFlowminId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnFlowmaxId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnFlowmaxId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnN2Id, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnN2Id, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnHelpId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnHelpId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnBeepId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnBeepId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            // 13/01/2014
            //If oTree.getValue(Nothing, 0, "", stnPwrLimitId, sValue, retNode) Then
            //   xmlNode = xmlAddNode(xmlStationSettingsElem, stnPwrLimitId, sValue)
            //   If bAddCheckedAttribute Then xmlAddAttrib(xmlNode, xmlCheckedId, retNode.Checked.ToString)
            //End If
            // do not save PIN
            //If oTree.getValue(Nothing, 0, "", stnPINId, sValue, retNode) Then
            //    xmlNode = addNode(xmlStationSettingsElem, stnPINId, sValue)
            //    If bAddCheckedAttribute Then addAttrib(xmlNode, xmlCheckedId, retNode.Checked.ToString)
            //end if
            // info settings
            if (oTree.getValue(null, 0, "", stnSWId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnSWId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            if (oTree.getValue(null, 0, "", stnProtocolId, ref sValue, ref retNode))
            {
                xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationSettingsElem, stnProtocolId, sValue);
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            //If oTree.getValue(Nothing, 0, "", stnTrafoErrId, sValue, retNode) Then
            //    xmlNode = xmlAddNode(xmlStationSettingsElem, stnTrafoErrId, convertSTempToSTemp(sValue, treeTunits, xmlTunits))
            //    If bAddCheckedAttribute Then xmlAddAttrib(xmlNode, xmlCheckedId, retNode.Checked.ToString)
            //End If
            //If oTree.getValue(Nothing, 0, "", stnMOSErrId, sValue, retNode) Then
            //    xmlNode = xmlAddNode(xmlStationSettingsElem, stnMOSErrId, convertSTempToSTemp(sValue, treeTunits, xmlTunits))
            //    If bAddCheckedAttribute Then xmlAddAttrib(xmlNode, xmlCheckedId, retNode.Checked.ToString)
            //End If

            //ports and tools
            xmlPortsAndToolsElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlStationElem, xmlPortsAndToolsId, null);
            //get tree node
            if (!oTree.getNode(null, 0, "", xmlPortsAndToolsId, ref retNode))
            {
                return returnValue;
            }
            else
            {
                if (bAddCheckedAttribute)
                {
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortsAndToolsElem, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                }
            }
            nodePortsAndTools = retNode;

            int nPorts = nodePortsAndTools.Nodes.Count;
            for (int p = 1; p <= nPorts; p++)
            {
                //port
                // add xml node
                xmlPortElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortsAndToolsElem, xmlPortId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlNumberId, p.ToString());
                // get port node
                if (!oTree.getNode(null, p, "", xmlPortId, ref retNode))
                {
                    return returnValue;
                }
                else
                {
                    if (bAddCheckedAttribute)
                    {
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlPortElem, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                    }
                }
                nodePort = retNode;

                //selected Temp
                if (oTree.getValue(null, p, "", toolSelectedTempId, ref sValue, ref retNode))
                {
                    xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolSelectedTempId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                    if (bAddCheckedAttribute)
                    {
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                    }
                }
                //selected Air flow
                if (oTree.getValue(null, p, "", toolSelectedAirFlowId, ref sValue, ref retNode))
                {
                    xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolSelectedAirFlowId, sValue);
                    if (bAddCheckedAttribute)
                    {
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                    }
                }
                //selected Ext TC Temp
                if (oTree.getValue(null, p, "", toolExtTCTempId, ref sValue, ref retNode))
                {
                    xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolExtTCTempId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                    if (bAddCheckedAttribute)
                    {
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                    }
                }

                //profiles
                if (oTree.getValue(null, p, "", toolProfilesId, ref sValue, ref retNode))
                {
                    xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, toolProfilesId, sValue);
                    if (bAddCheckedAttribute)
                    {
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                    }
                }

                //tools
                foreach (TreeNode nodeTool in nodePort.Nodes)
                {
                    // tools
                    if (((ParamTree.tParam)nodeTool.Tag).nodeName == xmlToolId)
                    {
                        sToolName = ((ParamTree.tParam)nodeTool.Tag).sValue;
                        sToolNbr = (System.Convert.ToInt32(System.Enum.Parse(typeof(GenericStationTools), sToolName))).ToString();

                        xmlToolElem = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlPortElem, xmlToolId, null);
                        // 2013/04/15 Se quita el atributo "Number" de "Tool"
                        //addAttrib(xmlDoc, xmlToolElem, xmlNumberId, sToolNbr)
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlToolElem, ConfigurationXML.xmlTypeId, sToolName);
                        // tool settings
                        if (oTree.getValue(null, p, sToolName, toolFixTempId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolFixTempId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                        // selected temp level
                        if (oTree.getValue(null, p, sToolName, toolSelectedTempLvlId, ref sValue, ref retNode))
                        {
                            // save as ToString of enum
                            // xmlNode = addNode(xmlToolElem, toolSelectedTempLvlId, CType(Convert.ToUInt32(sValue), ToolTemperatureLevels).ToString)
                            // save as number of enum
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolSelectedTempLvlId, Convert.ToUInt32(sValue).ToString());
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                            //If oTree.getValueNodeByName(retNode.Nodes, xmlEnabledId, sValue, retNodeEnabled) Then
                            //    If CType(sValue, Integer) = JBC_API.OnOff._ON Then bXmlEnabled = True Else bXmlEnabled = False
                            //    xmlAddAttrib(xmlNode, xmlEnabledId, bXmlEnabled.ToString)
                            //End If
                            if (oTree.getNodeAttribValue(retNode, ParamTree.EnabledAttribName, ref sValue))
                            {
                                if (sValue == ParamTree.EnabledTrue)
                                {
                                    bXmlEnabled = true;
                                }
                                else
                                {
                                    bXmlEnabled = false;
                                }
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlEnabledId, bXmlEnabled.ToString());
                            }
                        }
                        // temp level 1
                        if (oTree.getValue(null, p, sToolName, toolTempLvl1Id, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTempLvl1Id, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                            if (oTree.getNodeAttribValue(retNode, ParamTree.EnabledAttribName, ref sValue))
                            {
                                if (sValue == ParamTree.EnabledTrue)
                                {
                                    bXmlEnabled = true;
                                }
                                else
                                {
                                    bXmlEnabled = false;
                                }
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlEnabledId, bXmlEnabled.ToString());
                            }
                        }
                        // temp level 2
                        if (oTree.getValue(null, p, sToolName, toolTempLvl2Id, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTempLvl2Id, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                            if (oTree.getNodeAttribValue(retNode, ParamTree.EnabledAttribName, ref sValue))
                            {
                                if (sValue == ParamTree.EnabledTrue)
                                {
                                    bXmlEnabled = true;
                                }
                                else
                                {
                                    bXmlEnabled = false;
                                }
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlEnabledId, bXmlEnabled.ToString());
                            }
                        }
                        // temp level 3
                        if (oTree.getValue(null, p, sToolName, toolTempLvl3Id, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTempLvl3Id, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                            if (oTree.getNodeAttribValue(retNode, ParamTree.EnabledAttribName, ref sValue))
                            {
                                if (sValue == ParamTree.EnabledTrue)
                                {
                                    bXmlEnabled = true;
                                }
                                else
                                {
                                    bXmlEnabled = false;
                                }
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlEnabledId, bXmlEnabled.ToString());
                            }
                        }
                        // sleep temp
                        if (oTree.getValue(null, p, sToolName, toolSleepTempId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolSleepTempId, convertSTempToSTemp(sValue, treeTunits, xmlTunits));
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                        // sleep delay
                        if (oTree.getValue(null, p, sToolName, toolSleepDelayId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolSleepDelayId, sValue);
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                            if (oTree.getNodeAttribValue(retNode, ParamTree.EnabledAttribName, ref sValue))
                            {
                                if (sValue == ParamTree.EnabledTrue)
                                {
                                    bXmlEnabled = true;
                                }
                                else
                                {
                                    bXmlEnabled = false;
                                }
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlEnabledId, bXmlEnabled.ToString());
                            }
                        }
                        // hiber delay
                        if (oTree.getValue(null, p, sToolName, toolHibernationDelayId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolHibernationDelayId, sValue);
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                            if (oTree.getNodeAttribValue(retNode, ParamTree.EnabledAttribName, ref sValue))
                            {
                                if (sValue == ParamTree.EnabledTrue)
                                {
                                    bXmlEnabled = true;
                                }
                                else
                                {
                                    bXmlEnabled = false;
                                }
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlEnabledId, bXmlEnabled.ToString());
                            }
                        }
                        // adjust temp
                        if (oTree.getValue(null, p, sToolName, toolAdjustTempId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolAdjustTempId, convertSTempAdjToSTempAdj(sValue, treeTunits, xmlTunits));
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                        // time to stop
                        if (oTree.getValue(null, p, sToolName, toolTimeToStopId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolTimeToStopId, sValue);
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                        // Ext TC mode
                        if (oTree.getValue(null, p, sToolName, toolExtTCModeId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolExtTCModeId, sValue);
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                        // start mode - tool button
                        if (oTree.getValue(null, p, sToolName, toolStartModeToolButtonId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolStartModeToolButtonId, sValue);
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                        // start mode - automatic
                        if (oTree.getValue(null, p, sToolName, toolStartModeStandOutId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolStartModeStandOutId, sValue);
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                        // start mode - pedal
                        if (oTree.getValue(null, p, sToolName, toolStartModePedalActionId, ref sValue, ref retNode))
                        {
                            xmlNode = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlToolElem, toolStartModePedalActionId, sValue);
                            if (bAddCheckedAttribute)
                            {
                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(xmlNode, ConfigurationXML.xmlCheckedId, retNode.Checked.ToString());
                            }
                        }
                    }
                }
            }

            returnValue = xmlDoc;

            //Catch ex As Exception
            //    confParamTreeToXml = Nothing
            //    MsgBox("Error converting tree settings to xml: " & ex.Message, MsgBoxStyle.Critical)
            //    Exit Function

            //End Try

            return returnValue;
        }

        public static string confSaveConfXml(System.Xml.XmlDocument xmlDoc, string sTitle)
        {
            // return path file name of xml
            string sFilename = "ConfigSettings_" + Strings.Format(DateTime.Now, sDatetimeForFilesFormat);
            string sTempPathJBC = sTempPath + sTempPathSubdir;
            string sXmlPathFileName = sTempPathJBC + sFilename + ".xml";
            string sXslPathFileName = sTempPathJBC + sFilename + ".xsl";
            if (xmlDoc != null)
            {
                if (!System.IO.Directory.Exists(sTempPathJBC))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(sTempPathJBC);
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                // copy de xls file in resources to temptapth
                // load xsl resource file as string
                string temp = My.Resources.Resources.ConfigSettings; // get file as string
                                                                     // replace text
                temp = temp.Replace("#Title#", sTitle);
                temp = temp.Replace("#Parameter#", Localization.getResStr(xslParameterId));
                temp = temp.Replace("#Value#", Localization.getResStr(xslValueId));
                temp = temp.Replace("#_On#", Localization.getResStr(ON_STRId));
                temp = temp.Replace("#_Off#", Localization.getResStr(OFF_STRId));
                // write content
                var myFile = System.IO.File.CreateText(sXslPathFileName);
                myFile.WriteLine(temp);
                myFile.Close();

                // load texts according to current language
                confXlmDocLoadTexts(xmlDoc, sFilename + ".xsl", true);
                // save xml file
                xmlDoc.Save(sXmlPathFileName);
                return sXmlPathFileName;
            }
            else
            {
                return "";
            }
        }

        public static bool confParamTreeReloadTexts(ParamTree oTree)
        {
            bool returnValue = false;
            returnValue = false;
            foreach (TreeNode node in oTree.TreeView1.Nodes)
            {
                confParamTreeNodeAndChildrenReloadText(node);
            }
            returnValue = true;
            return returnValue;
        }

        private static void confParamTreeNodeAndChildrenReloadText(TreeNode node)
        {
            // NOT FINISHED
            //ParamTree.tParam param = node.Tag;
            //// reload node
            //if (param.nodeName == stnModelId)
            //{
            //}
            //else if (param.nodeName == xmlStationId)
            //{
            //}
            //else if (param.nodeName == xmlTempUnitsId)
            //{
            //}
            //else if (param.nodeName == stnNameId)
            //{
            //}
            //else if (param.nodeName == stnTunitsId)
            //{
            //}
            //else if (param.nodeName == stnTminId)
            //{
            //}
            //else if (param.nodeName == stnTmaxId)
            //{
            //}
            //else if (param.nodeName == stnN2Id)
            //{
            //}
            //else if (param.nodeName == stnHelpId)
            //{
            //}
            //else if (param.nodeName == stnBeepId)
            //{
            //}
            //else if (param.nodeName == stnPwrLimitId)
            //{
            //}
            //else if (param.nodeName == stnPINId)
            //{
            //}
            //else if (param.nodeName == stnSWId)
            //{
            //}
            //else if (param.nodeName == stnProtocolId)
            //{
            //}
            //else if (param.nodeName == stnTrafoErrId)
            //{
            //}
            //else if (param.nodeName == stnMOSErrId)
            //{
            //}
            //else if (param.nodeName == xmlPortsAndToolsId)
            //{
            //}
            //else if (param.nodeName == xmlPortId)
            //{
            //}
            //else if (param.nodeName == toolSelectedTempId)
            //{
            //}
            //else if (param.nodeName == xmlToolId)
            //{
            //}
            //else if (param.nodeName == toolFixTempId)
            //{
            //}
            //else if (param.nodeName == toolSelectedTempLvlId)
            //{
            //}
            //else if (param.nodeName == toolTempLvl1Id)
            //{
            //}
            //else if (param.nodeName == toolTempLvl2Id)
            //{
            //}
            //else if (param.nodeName == toolTempLvl3Id)
            //{
            //}
            //else if (param.nodeName == toolSleepTempId)
            //{
            //}
            //else if (param.nodeName == toolSleepDelayId)
            //{
            //}
            //else if (param.nodeName == toolHibernationDelayId)
            //{
            //}
            //else if (param.nodeName == toolAdjustTempId)
            //{
            //}

            if (node.Nodes.Count > 1)
            {
                foreach (TreeNode nodechild in node.Nodes)
                {
                    confParamTreeNodeAndChildrenReloadText(nodechild);
                }
            }
        }

        public static bool confXlmDocLoadTexts(System.Xml.XmlDocument xmlDoc, string sStyleSheet, bool bValueToTextValue)
        {
            bool returnValue = false;
            string sErrorsInNodes = "";
            string xmlTunits = "";

            returnValue = false;
            if (sStyleSheet != "")
            {
                // insert before root node
                System.Xml.XmlProcessingInstruction pi = xmlDoc.CreateProcessingInstruction("xml-stylesheet", string.Format("type='text/xsl' href='{0}'", sStyleSheet));
                System.Xml.XmlNodeList nodeList = xmlDoc.SelectNodes("/*"); // selecciona el primer nodo Element (no el Document)
                if (nodeList.Count > 0)
                {
                    var nodeRoot = nodeList[0];
                    xmlDoc.InsertBefore(pi, nodeRoot);
                }
            }
            foreach (System.Xml.XmlNode node in xmlDoc.ChildNodes)
            {
                confXmlNodeAndChildrenReloadText(node, bValueToTextValue, ref xmlTunits, ref sErrorsInNodes);
            }
            if (!string.IsNullOrEmpty(sErrorsInNodes))
            {
                MessageBox.Show("Errors in nodes: " + sErrorsInNodes);
            }
            returnValue = true;
            return returnValue;
        }

        private static void confXmlNodeAndChildrenReloadText(System.Xml.XmlNode node, bool bValueToTextValue, ref string xmlTunits, ref string sErrorsInNodes)
        {
            string sText = "";
            System.Xml.XmlAttribute attrib = default(System.Xml.XmlAttribute);
            string sId = "";
            string sValue = "";
            string sTextValue = "";
            string sNodeNameForError = "";

            if (node != null)
            {
                sNodeNameForError = node.Name;
            }

            try
            {
                if (node.Name == xmlRootId || node.Name == xmlLogRootId)
                {
                    // sets lang attribute
                    attrib = node.Attributes[ConfigurationXML.xmlLangId];
                    if (ReferenceEquals(attrib, null))
                    {
                        attrib = RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(node, ConfigurationXML.xmlLangId, Localization.curCulture);
                    }
                    else
                    {
                        attrib.Value = Localization.curCulture;
                    }

                }
                else
                {
                    // set text id to retrieve from resources
                    // xml tags with different id for text

                    //conf
                    if (node.Name == xmlCommentId)
                    {
                        sId = confConfCommentId;
                    }
                    else if (node.Name == xmlStationId)
                    {
                        sId = confStationId;
                        // temp units in xml conf
                        if (xmlTunits == "")
                        {
                            attrib = node.Attributes[xmlTempUnitsId];
                            if (attrib != null)
                            {
                                xmlTunits = attrib.Value; // contains ºC or ºF
                            }
                        }
                    }
                    else if (node.Name == xmlStationSettingsId)
                    {
                        sId = stnStationSettingsId;
                    }
                    else if (node.Name == xmlPortsAndToolsId)
                    {
                        sId = toolPortsAndToolsId;
                    }
                    else if (node.Name == xmlPortId)
                    {
                        sId = toolPortId;
                    }
                    else if (node.Name == xmlToolId)
                    {
                        sId = toolToolId;
                    }
                    else if (node.Name == ConfigurationXML.xmlVersionId)
                    {
                        sId = confVersionId;
                    }
                    else if (node.Name == xmlTempUnitsId)
                    {
                        sId = stnTunitsId;
                    }
                    else if (node.Name == xmlCountersId)
                    {
                        sId = counterGlobalId;
                    }
                    else if (node.Name == xmlPartialCountersId)
                    {
                        sId = counterPartialId;
                    }
                    else if (node.Name == ConfigurationXML.xmlEnabledId)
                    {
                        sId = gralEnabledId;

                        // logs
                    }
                    else if (node.Name == xmlLogSourceId)
                    {
                        sId = logSourceId;
                        // temp units in xml log
                        if (xmlTunits == "")
                        {
                            attrib = node.Attributes[xmlTempUnitsId];
                            if (attrib != null)
                            {
                                xmlTunits = attrib.Value; // contains ºC or ºF
                            }
                        }
                    }
                    else if (node.Name == xmlLogTargetId)
                    {
                        sId = logTargetId;
                    }
                    else if (node.Name == xmlLogPortsId)
                    {
                        sId = logPortsId;
                    }
                    else if (node.Name == xmlLogsId)
                    {
                        sId = logLogsId;
                    }
                    else
                    {
                        sId = node.Name;

                        // settings: convert value to text
                        sValue = node.InnerText;
                        if (sValue.Trim() != "")
                        {
                            if (node.Name == stnTunitsId)
                            {
                                // station temp units (contains C or F)
                                sTextValue = string.Format("º{0}", sValue);
                            }
                            else if (((((((((((node.Name == stnTminId) || (node.Name == stnTmaxId)) || (node.Name == toolSelectedTempId)) || (node.Name == toolFixTempId)) || (node.Name == toolTempLvl1Id)) || (node.Name == toolTempLvl2Id)) || (node.Name == toolTempLvl3Id)) || (node.Name == stnTrafoErrId)) || (node.Name == stnMOSErrId)) || (node.Name == toolSleepTempId)) || (node.Name == toolAdjustTempId))
                            {
                                // temperature
                                if (sValue == noDataStr)
                                {
                                    sTextValue = noDataStr;
                                }
                                else
                                {
                                    sTextValue = string.Format("{0} {1}", sValue, xmlTunits);
                                }
                            }
                            else if (node.Name == toolSelectedTempLvlId)
                            {
                                // selected tem level
                                sTextValue = getTextFromValueArray(sValue, getTempLevels(arrOption.VALUES, false), getTempLevels(arrOption.TEXTS, false));
                            }
                            else if (((node.Name == stnN2Id) || (node.Name == stnHelpId)) || (node.Name == stnBeepId))
                            {
                                // on/off
                                sTextValue = getTextFromValueArray(sValue, getOnOff(arrOption.VALUES), getOnOff(arrOption.TEXTS));
                            }
                            else if (node.Name == stnPwrLimitId)
                            {
                                // power
                                sTextValue = string.Format("{0} {1}", sValue, pwrUnitsStr);
                            }
                            else if (node.Name == toolSleepDelayId)
                            {
                                // minutes
                                sTextValue = getTextFromValueArray(sValue, getSleepDelays(arrOption.VALUES, false), getSleepDelays(arrOption.TEXTS, false));
                            }
                            else if (node.Name == toolHibernationDelayId)
                            {
                                // minutes
                                sTextValue = getTextFromValueArray(sValue, getHiberDelays(arrOption.VALUES, false), getHiberDelays(arrOption.TEXTS, false));
                            }
                        }
                    }

                    // get text from resources
                    sText = Localization.getResStr(sId);

                    // add or change "Text" attribute
                    if (!string.IsNullOrEmpty(sText))
                    {
                        sText = RoutinesLibrary.Data.Xml.XMLUtils.ConvertTextToXML(sText);
                        attrib = node.Attributes[ConfigurationXML.xmlTextId];
                        if (ReferenceEquals(attrib, null))
                        {
                            // text attribute do not exists
                            attrib = RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(node, ConfigurationXML.xmlTextId, sText);
                        }
                        else
                        {
                            attrib.Value = sText;
                        }
                    }

                    // add or change "TextValue" attribute
                    if (!string.IsNullOrEmpty(sTextValue))
                    {
                        sTextValue = RoutinesLibrary.Data.Xml.XMLUtils.ConvertTextToXML(sTextValue);
                        // replace innertext by value converted to text
                        // or add a TextValue attribute
                        if (bValueToTextValue)
                        {
                            node.InnerText = sTextValue;
                        }
                        else
                        {
                            attrib = node.Attributes[ConfigurationXML.xmlTextValueId];
                            if (ReferenceEquals(attrib, null))
                            {
                                // TextValue attribute do not exists
                                attrib = RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(node, ConfigurationXML.xmlTextValueId, sTextValue);
                            }
                            else
                            {
                                attrib.Value = sTextValue;
                            }
                        }
                    }

                }

            }
            catch (Exception)
            {
                if (sErrorsInNodes != "")
                {
                    sErrorsInNodes += ",";
                }
                sErrorsInNodes += sNodeNameForError;
            }

            if (node.ChildNodes.Count > 0)
            {
                foreach (System.Xml.XmlNode nodechild in node.ChildNodes)
                {
                    confXmlNodeAndChildrenReloadText(nodechild, bValueToTextValue, ref xmlTunits, ref sErrorsInNodes);
                }
            }
        }

        #endregion

        #region Settings Manager Log functions

        public static bool logLogXMLs(ref System.Xml.XmlDocument xmlLog, System.Xml.XmlDocument xmlDocSrc, System.Xml.XmlDocument xmlDocTrg, int[] iTargetFromSourcePorts = default(int[]))
        {

            //ByVal myIDTrg As ULong, ByVal jbc As JBC_API, _

            // New and Changed Features of target station
            //Dim features = New cls_Features(jbc.GetStationModel(myID), jbc.GetStationModelType(myID), jbc.GetStationModelVersion(myID), jbc.GetStationProtocol(myID))

            // iTargetFromSourcePorts(targetport-1) contains target/source port correlations:
            //   format: iTargetFromSourcePorts(x) is target port x+1, value is source port
            // iTargetFromSourcePorts(x) = 0 specify no source for x+1 target port
            // if iTargetFromSourcePorts = nothing, use same target/source ports
            // examples:
            //   if iTargetFromSourcePorts(0) = 2 then target port 1 (0+1) from source port 2
            //   iTargetFromSourcePorts(1) = 0 indicates do not save to target port 2

            // Log format (xml):
            //
            // <JBCConfigLog Version="1" DateTime="">
            //   <SourceParams>
            //       <StationSettings>
            //           station settings to change
            //       </StationSettings>
            //       <PortsAndTools>
            //           <Port Number="1">
            //              <toolSelectedTemp>350</toolSelectedTemp>
            //               <Tool Number="1" Type="T210">
            //                   <toolFixTemp>0</toolFixTemp>
            //                   ...toolsettings to change...
            //               </Tool>
            //               <Tool Number="2" Type="T245">
            //                   <toolFixTemp>0</toolFixTemp>
            //                   ...toolsettings to change...
            //               </Tool>
            //           </Port>
            //           <Port Number="2">
            //               ...
            //           </Port>
            //       </PortsAndTools>
            //   </SourceParams>
            //   <TargetStations>
            //       <Station stnModel="DD" stnName="zzzzzzzz" TempUnits="°C">
            //           <Ports>
            //               <Port Number"1">source_port</Port>
            //               <Port Number"2">source_port</Port>
            //               <Port Number"3">source_port</Port>
            //               <Port Number"4">source_port</Port>
            //           </Ports>
            //           <Logs>
            //               <Log>xxxxxxxxxxxxxxxxx</Log>
            //           </Logs>
            //       </Station>
            //   </TargetStations>
            // </JBCConfigLog>


            string stationModelSrc = "";
            string stationModelTrg = "";
            string stationNameTrg = "";
            string xmlTunitsSrc = CELSIUS_STR; // default Celsius
            string xmlTunitsTrg = CELSIUS_STR; // default Celsius
            CTemperature auxTemp1 = new CTemperature(0);
            CTemperature auxTemp2 = new CTemperature(0);
            int auxInt1 = 0;
            int auxInt2 = 0;
            bool bXmlChecked = true;
            bool bXmlCheckedExists = false;
            // enabled (protocol 02, in levels, etc)
            bool bXmlEnabled = false;
            bool bXmlEnabledExists = false;
            bool bXmlEnabledTrg = false; // target
            bool bXmlEnabledExistsTrg = false; // target
                                               // xml
                                               // xml params
            string sCurrentXmlPath = "";
            System.Xml.XmlNode nodeTemp = default(System.Xml.XmlNode);
            System.Xml.XmlNode nodeTargetPort = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeSourceTool = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeTargetTool = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeTrg = default(System.Xml.XmlElement);
            // xml log
            string sCurrentLogPath = "";
            System.Xml.XmlNode nodeLogRoot = default(System.Xml.XmlNode);
            System.Xml.XmlNode nodeLogSource = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLogStationSettings = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLogPortsAndTools = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLogPort = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLogTool = default(System.Xml.XmlElement);
            bool bAddSourceParams = false;

            System.Xml.XmlNode nodeLogTarget = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLogTargetStation = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLogPorts = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLogs = default(System.Xml.XmlElement);
            System.Xml.XmlNode nodeLog = default(System.Xml.XmlElement);
            string sValue = "";
            bool bMatch = false;

            try
            {
                // source data
                nodeTemp = xmlGetStationNodeFromXml(xmlDocSrc);
                if (nodeTemp != null)
                {
                    stationModelSrc = nodeTemp.Attributes[stnModelId].Value;
                    xmlTunitsSrc = nodeTemp.Attributes[xmlTempUnitsId].Value;
                }

                // target data
                nodeTemp = xmlGetStationNodeFromXml(xmlDocTrg);
                if (nodeTemp != null)
                {
                    stationModelTrg = nodeTemp.Attributes[stnModelId].Value;
                    xmlTunitsTrg = nodeTemp.Attributes[xmlTempUnitsId].Value;
                }

                // target station name
                sCurrentXmlPath = "/" + xmlRootId + "/" + xmlStationId + "/" + xmlStationSettingsId + "/" + stnNameId;
                nodeTemp = xmlDocTrg.SelectSingleNode(sCurrentXmlPath);
                if (nodeTemp != null)
                {
                    stationNameTrg = nodeTemp.InnerText;
                }

                // log init
                if (ReferenceEquals(xmlLog, null))
                {
                    // if xmlLog is not created yet
                    xmlLog = RoutinesLibrary.Data.Xml.XMLUtils.CreateNewDoc();

                    //root, source and target nodes
                    nodeLogRoot = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(xmlLog, xmlLogRootId);
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogRoot, ConfigurationXML.xmlVersionId, "2");
                    // 2013/04/16 En archivos xml, se cambia el atributo "DateTime" por 2 atributos "Date" y "Time"
                    //addAttrib(nodeLogRoot, xmlDateTimeId, Format(Now(), xmlDateTimeFormat))
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogRoot, ConfigurationXML.xmlDateId, Strings.Format(DateTime.Now, xmlDateFormat));
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogRoot, ConfigurationXML.xmlTimeId, Strings.Format(DateTime.Now, xmlTimeFormat));
                    nodeLogSource = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogRoot, xmlLogSourceId);
                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogSource, xmlTempUnitsId, xmlTunitsSrc); // xml temperature units
                    nodeLogTarget = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogRoot, xmlLogTargetId, null);
                }
                else
                {
                    sCurrentLogPath = "/" + xmlLogRootId;
                    nodeLogRoot = xmlLog.SelectSingleNode(sCurrentLogPath);
                    sCurrentLogPath = "/" + xmlLogRootId + "/" + xmlLogSourceId;
                    nodeLogSource = xmlLog.SelectSingleNode(sCurrentLogPath);
                    sCurrentLogPath = "/" + xmlLogRootId + "/" + xmlLogTargetId;
                    nodeLogTarget = xmlLog.SelectSingleNode(sCurrentLogPath);
                }

                // log source parameters, if does not exist
                sCurrentLogPath = "/" + xmlLogRootId + "/" + xmlLogSourceId + "/" + xmlStationSettingsId;
                nodeLogStationSettings = xmlLog.SelectSingleNode(sCurrentLogPath);
                sCurrentLogPath = "/" + xmlLogRootId + "/" + xmlLogSourceId + "/" + xmlPortsAndToolsId;
                nodeLogPortsAndTools = xmlLog.SelectSingleNode(sCurrentLogPath);
                if (ReferenceEquals(nodeLogStationSettings, null))
                {
                    bAddSourceParams = true;
                    nodeLogStationSettings = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogSource, xmlStationSettingsId, null);
                    nodeLogPortsAndTools = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogSource, xmlPortsAndToolsId, null);
                }

                // log this target station
                nodeLogTargetStation = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogTarget, xmlStationId, null);
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogTargetStation, stnModelId, stationModelTrg); // station model
                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogTargetStation, stnNameId, stationNameTrg); // station name
                                                                                                              //addAttrib(nodeLogTargetStation, xmlTempUnitsId, xmlTunitsTrg) ' xml temperature units
                                                                                                              // target/from source ports node
                nodeLogPorts = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogTargetStation, xmlLogPortsId, null);
                // logs node
                nodeLogs = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogTargetStation, xmlLogsId, null);

                System.Xml.XmlNode stationParamsSrc = xmlGetStationParamsNodeFromXml(xmlDocSrc);
                System.Xml.XmlNode stationParamsTrg = xmlGetStationParamsNodeFromXml(xmlDocTrg);
                if (stationParamsSrc != null)
                {
                    foreach (System.Xml.XmlNode node in stationParamsSrc.ChildNodes)
                    {
                        // read "Checked" attribute
                        bXmlCheckedExists = ConfigurationXML.getXmlChecked(node, ref bXmlChecked);
                        // add source param, if not added yet
                        if (bAddSourceParams)
                        {
                            // add all parameters. not checked parameters = blank
                            if (!bXmlCheckedExists || bXmlChecked)
                            {
                                sValue = node.InnerText;
                            }
                            else
                            {
                                sValue = "";
                            }
                            RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogStationSettings, node.Name, sValue);
                        }
                        // only analize if checked or label does not exist in parameter
                        if (!bXmlCheckedExists || bXmlChecked)
                        {
                            // get target node
                            nodeTrg = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(stationParamsTrg, node.Name);
                            if (ReferenceEquals(nodeTrg, null))
                            {
                                nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrNotExistId), Localization.getResStr(node.Name)));
                            }
                            else
                            {
                                // station parameters
                                if (node.Name == stnNameId)
                                {
                                    // station name
                                }
                                else if (node.Name == stnTunitsId)
                                {
                                    if (((CTemperature.TemperatureUnit)(Convert.ToUInt32(Strings.Asc(node.InnerText)))) != ((CTemperature.TemperatureUnit)(Convert.ToUInt32(Strings.Asc(nodeTrg.InnerText)))))
                                    {
                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrNotChangedId), Localization.getResStr(node.Name)));
                                    }
                                }
                                else if ((node.Name == stnTminId) || (node.Name == stnTmaxId))
                                {
                                    if (convertStringToTemp(node.InnerText, xmlTunitsSrc).UTI !=
                                            convertStringToTemp(nodeTrg.InnerText, xmlTunitsTrg).UTI)
                                    {
                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrNotChangedId), Localization.getResStr(node.Name)));
                                    }
                                }
                                else if (((node.Name == stnN2Id) || (node.Name == stnHelpId)) || (node.Name == stnBeepId))
                                {
                                    if (((OnOff)(Convert.ToUInt32(node.InnerText))) != ((OnOff)(Convert.ToUInt32(nodeTrg.InnerText))))
                                    {
                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrNotChangedId), Localization.getResStr(node.Name)));
                                    }
                                }
                                else if (node.Name == stnPwrLimitId)
                                {
                                    //If Convert.ToUInt32(node.InnerText) <> Convert.ToUInt32(nodeTrg.InnerText) Then
                                    //   nodeLog = xmlAddNode(nodeLogs, xmlLogId, String.Format(getResStr(logErrNotChangedId), getResStr(node.Name)))
                                    //End If
                                }
                                else if (node.Name == stnPINId)
                                {
                                    // PIN
                                    //jbc.SetStationPIN(myID, node.InnerText)
                                }
                            }

                        } // If Not bXmlCheckedExists Or bXmlChecked
                    }
                }

                // ports and tools
                System.Xml.XmlNode toolPortNodesSrc = xmlGetPortsAndToolsNodeFromXml(xmlDocSrc);
                System.Xml.XmlNode toolPortNodesTrg = xmlGetPortsAndToolsNodeFromXml(xmlDocTrg);
                if (toolPortNodesSrc != null)
                {
                    System.Xml.XmlNodeList portNodesSrc = toolPortNodesSrc.ChildNodes;
                    System.Xml.XmlNodeList portNodesTrg = toolPortNodesTrg.ChildNodes;
                    int nTargetPorts = portNodesTrg.Count;
                    if (ReferenceEquals(iTargetFromSourcePorts, null))
                    {
                        iTargetFromSourcePorts = new int[nTargetPorts + 1];
                        for (var i = 1; i <= nTargetPorts; i++)
                        {
                            iTargetFromSourcePorts[i - 1] = System.Convert.ToInt32(i);
                        }
                    }
                    GenericStationTools[] supportedTools = xmlGetStationTools(xmlDocTrg);
                    int iSourcePort = 0;
                    int iTargetPort = 0;
                    GenericStationTools curTool = default(GenericStationTools);

                    // log target/from source ports
                    for (var iTarget = 0; iTarget <= iTargetFromSourcePorts.Length - 1; iTarget++)
                    {
                        nodeTemp = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogPorts, xmlPortId, System.Convert.ToString(iTargetFromSourcePorts[iTarget]));
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeTemp, ConfigurationXML.xmlNumberId, (iTarget + 1).ToString());
                        RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeTemp, xmlTextFromPortId, string.Format(Localization.getResStr(logTargetPortFromSourcePortId), (iTarget + 1).ToString(), iTargetFromSourcePorts[iTarget]));
                    }

                    foreach (System.Xml.XmlNode portNode in portNodesSrc)
                    {
                        iSourcePort = int.Parse(portNode.Attributes[ConfigurationXML.xmlNumberId].Value);

                        // log add source port and tools parameters, if not added yet
                        if (bAddSourceParams)
                        {
                            nodeLogPort = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogPortsAndTools, portNode.Name);
                            RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogPort, ConfigurationXML.xmlNumberId, iSourcePort.ToString());
                            foreach (System.Xml.XmlNode portchildnode in portNode.ChildNodes)
                            {
                                bXmlCheckedExists = ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked);
                                // only if checked or label does not exist in parameter
                                if (portchildnode.Name == toolSelectedTempId)
                                {
                                    sValue = "";
                                    if (!bXmlCheckedExists || bXmlChecked)
                                    {
                                        sValue = portchildnode.InnerText;
                                    }
                                    RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogPort, portchildnode.Name, sValue);
                                }
                                else if (portchildnode.Name == xmlToolId)
                                {
                                    nodeLogTool = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogPort, portchildnode.Name, null);
                                    // 2013/04/15 Se quita el atributo "Number" de "Tool"
                                    //addAttrib(xmlLog, nodeLogTool, xmlNumberId, portchildnode.Attributes.ItemOf(xmlNumberId).Value)
                                    RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLogTool, ConfigurationXML.xmlTypeId, portchildnode.Attributes[ConfigurationXML.xmlTypeId].Value);
                                    foreach (System.Xml.XmlNode toolparamNode in portchildnode.ChildNodes)
                                    {
                                        bXmlCheckedExists = ConfigurationXML.getXmlChecked(toolparamNode, ref bXmlChecked);
                                        // only if checked or label does not exist in parameter
                                        sValue = "";
                                        if (!bXmlCheckedExists || bXmlChecked)
                                        {
                                            sValue = toolparamNode.InnerText;
                                            nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogTool, toolparamNode.Name, sValue);
                                            bXmlEnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bXmlEnabled);
                                            if (bXmlEnabledExists)
                                            {
                                                RoutinesLibrary.Data.Xml.XMLUtils.AddAttrib(nodeLog, ConfigurationXML.xmlEnabledId, bXmlEnabled.ToString());
                                            }
                                        }
                                        else
                                        {
                                            nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogTool, toolparamNode.Name, "");
                                        }
                                    }
                                }
                            }
                        }

                        // where to copy this source port (may be several target ports)
                        // look for source port in target ports array
                        for (var iTarget = 0; iTarget <= iTargetFromSourcePorts.Length - 1; iTarget++)
                        {
                            iTargetPort = System.Convert.ToInt32(iTarget + 1);
                            // copy source settings for each target port
                            if (iTargetFromSourcePorts[iTarget] == iSourcePort)
                            {
                                if (iTargetPort <= nTargetPorts & iTargetPort > 0)
                                {
                                    // get target port node
                                    nodeTargetPort = xmlGetPortNodeFromXml(xmlDocTrg, iTargetPort.ToString());
                                    if (ReferenceEquals(nodeTargetPort, null))
                                    {
                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortNotExistId), iTargetPort.ToString()));
                                    }
                                    else
                                    {
                                        foreach (System.Xml.XmlNode portchildnode in portNode.ChildNodes)
                                        {
                                            if (portchildnode.Name == toolSelectedTempId)
                                            {
                                                // read "Checked" attribute
                                                bXmlCheckedExists = ConfigurationXML.getXmlChecked(portchildnode, ref bXmlChecked);
                                                // only analize if checked or label does not exist in parameter
                                                if (!bXmlCheckedExists || bXmlChecked)
                                                {
                                                    nodeTrg = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(nodeTargetPort, portchildnode.Name);
                                                    if (ReferenceEquals(nodeTrg, null))
                                                    {
                                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotExistId), Localization.getResStr(portchildnode.Name), iTargetPort.ToString(), ""));
                                                    }
                                                    else
                                                    {
                                                        auxTemp1 = convertStringToTemp(portchildnode.InnerText, xmlTunitsSrc);
                                                        auxTemp2 = convertStringToTemp(nodeTrg.InnerText, xmlTunitsTrg);
                                                        if (auxTemp1.UTI != auxTemp1.UTI)
                                                        {
                                                            nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotChangedId), Localization.getResStr(portchildnode.Name), iTargetPort.ToString(), ""));
                                                        }
                                                    }
                                                }
                                            }
                                            else if (portchildnode.Name == xmlToolId)
                                            {
                                                nodeSourceTool = portchildnode;
                                                //curTool = CType(CInt(nodeSourceTool.Attributes.ItemOf(xmlNumberId).Value), JBC_API.GenericStationTools)
                                                curTool = xmlGetToolFromToolNode(nodeSourceTool);
                                                nodeTargetTool = xmlGetToolNodeFromXml(xmlDocTrg, iTargetPort.ToString(), curTool.ToString());
                                                if (ReferenceEquals(nodeTargetTool, null))
                                                {
                                                    // if target doesn't exist
                                                    nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrToolNotExistId), curTool.ToString()));
                                                }
                                                else
                                                {
                                                    foreach (System.Xml.XmlNode toolparamNode in nodeSourceTool.ChildNodes)
                                                    {
                                                        // read "Checked" attribute
                                                        bXmlCheckedExists = ConfigurationXML.getXmlChecked(toolparamNode, ref bXmlChecked);
                                                        // only analize if checked or label does not exist in parameter
                                                        if ((!bXmlCheckedExists) || bXmlChecked)
                                                        {
                                                            nodeTrg = RoutinesLibrary.Data.Xml.XMLUtils.GetFirstChild(nodeTargetTool, toolparamNode.Name);
                                                            if (ReferenceEquals(nodeTrg, null))
                                                            {
                                                                nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotExistId), Localization.getResStr(toolparamNode.Name), iTargetPort.ToString(), curTool.ToString()));
                                                            }
                                                            else
                                                            {
                                                                if (toolparamNode.Name == toolFixTempId)
                                                                {
                                                                    // if "---", "0" or UTI NO_DEFINED_TEMP, -> Off
                                                                    if (toolparamNode.InnerText == noDataStr || toolparamNode.InnerText == "0")
                                                                    {
                                                                        auxTemp1.UTI = Constants.NO_TEMP_LEVEL;
                                                                    }
                                                                    else
                                                                    {
                                                                        auxTemp1 = convertStringToTemp(toolparamNode.InnerText, xmlTunitsSrc);
                                                                    }
                                                                    if (nodeTrg.InnerText == noDataStr || nodeTrg.InnerText == "0")
                                                                    {
                                                                        auxTemp2.UTI = Constants.NO_TEMP_LEVEL;
                                                                    }
                                                                    else
                                                                    {
                                                                        auxTemp2 = convertStringToTemp(nodeTrg.InnerText, xmlTunitsTrg);
                                                                    }

                                                                    if (auxTemp1.UTI != auxTemp2.UTI)
                                                                    {
                                                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotChangedId), Localization.getResStr(toolparamNode.Name), iTargetPort.ToString(), curTool.ToString()));
                                                                    }
                                                                }
                                                                else if (toolparamNode.Name == toolSelectedTempLvlId)
                                                                {
                                                                    bMatch = true;
                                                                    // ver enabled
                                                                    bXmlEnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bXmlEnabled);
                                                                    bXmlEnabledExistsTrg = ConfigurationXML.getXmlEnabled(nodeTrg, ref bXmlEnabledTrg);
                                                                    if (bXmlEnabledExists && !bXmlEnabledExistsTrg)
                                                                    {
                                                                        // enabled en source y no en target
                                                                        if (!bXmlEnabled)
                                                                        {
                                                                            // source no enabled
                                                                            if (((ToolTemperatureLevels)(Convert.ToUInt32(nodeTrg.InnerText))) !=
                                                                                    ToolTemperatureLevels.NO_LEVELS)
                                                                            {
                                                                                // nivel target con nivel -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // source enabled, comparar nivel seleccionado
                                                                            if (((ToolTemperatureLevels)(Convert.ToUInt32(toolparamNode.InnerText))) != ((ToolTemperatureLevels)(Convert.ToUInt32(nodeTrg.InnerText))))
                                                                            {
                                                                                // diferente nivel -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (!bXmlEnabledExists && bXmlEnabledExistsTrg)
                                                                    {
                                                                        // no enabled en source y si en target
                                                                        if (((ToolTemperatureLevels)(Convert.ToUInt32(toolparamNode.InnerText))) == ToolTemperatureLevels.NO_LEVELS)
                                                                        {
                                                                            // si source está NO_LEVELS
                                                                            if (bXmlEnabledTrg)
                                                                            {
                                                                                // si el target está activo -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // si source tiene nivel seleccionado
                                                                            if (!bXmlEnabledTrg)
                                                                            {
                                                                                // si el target no está activo -> log
                                                                                bMatch = false;
                                                                            }
                                                                            else
                                                                            {
                                                                                // si el target está activo, comparar nivel seleccionado
                                                                                if (((ToolTemperatureLevels)(Convert.ToUInt32(toolparamNode.InnerText))) != ((ToolTemperatureLevels)(Convert.ToUInt32(nodeTrg.InnerText))))
                                                                                {
                                                                                    // diferente nivel -> log
                                                                                    bMatch = false;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        // los dos enabled o los dos sin enabled
                                                                        if (bXmlEnabledExists && bXmlEnabledExistsTrg && bXmlEnabled != bXmlEnabledTrg)
                                                                        {
                                                                            // si los dos tienen enabled y son diferentes -> log
                                                                            bMatch = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            // comparar nivel seleccionado
                                                                            if (((ToolTemperatureLevels)(Convert.ToUInt32(toolparamNode.InnerText))) != ((ToolTemperatureLevels)(Convert.ToUInt32(nodeTrg.InnerText))))
                                                                            {
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (!bMatch)
                                                                    {
                                                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotChangedId), Localization.getResStr(toolparamNode.Name), iTargetPort.ToString(), curTool.ToString()));
                                                                    }
                                                                }
                                                                else if (((toolparamNode.Name == toolTempLvl1Id) || (toolparamNode.Name == toolTempLvl2Id)) || (toolparamNode.Name == toolTempLvl3Id))
                                                                {
                                                                    auxTemp1 = convertStringToTemp(toolparamNode.InnerText, xmlTunitsSrc);
                                                                    auxTemp2 = convertStringToTemp(nodeTrg.InnerText, xmlTunitsTrg);
                                                                    // enabled
                                                                    bMatch = true;
                                                                    // ver enabled
                                                                    bXmlEnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bXmlEnabled);
                                                                    bXmlEnabledExistsTrg = ConfigurationXML.getXmlEnabled(nodeTrg, ref bXmlEnabledTrg);
                                                                    if (bXmlEnabledExists && !bXmlEnabledExistsTrg)
                                                                    {
                                                                        // existe enabled en source y no en target
                                                                        if (!bXmlEnabled)
                                                                        {
                                                                            // source not enabled
                                                                            if (auxTemp2.UTI != Constants.NO_TEMP_LEVEL)
                                                                            {
                                                                                // target con temperatura -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // source enabled, comparar temperaturas
                                                                            if (auxTemp1.UTI != auxTemp2.UTI)
                                                                            {
                                                                                // diferente temperatura -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (!bXmlEnabledExists && bXmlEnabledExistsTrg)
                                                                    {
                                                                        // no existe enabled en source y si en target
                                                                        if (auxTemp1.UTI == Constants.NO_TEMP_LEVEL)
                                                                        {
                                                                            // si source está LEVEL_OFF
                                                                            if (bXmlEnabledTrg)
                                                                            {
                                                                                // si el target está activo -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // si source tiene temperatura
                                                                            if (!bXmlEnabledTrg)
                                                                            {
                                                                                // si el target no está activo -> log
                                                                                bMatch = false;
                                                                            }
                                                                            else
                                                                            {
                                                                                // si el target está activo, comparar temperaturas
                                                                                if (auxTemp1.UTI != auxTemp2.UTI)
                                                                                {
                                                                                    // diferente temperatura -> log
                                                                                    bMatch = false;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        // los dos enabled o los dos sin enabled
                                                                        if (bXmlEnabledExists && bXmlEnabledExistsTrg && bXmlEnabled != bXmlEnabledTrg)
                                                                        {
                                                                            // si los dos tienen enabled y son diferentes -> log
                                                                            bMatch = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            // comparar temperaturas
                                                                            if (auxTemp1.UTI != auxTemp2.UTI)
                                                                            {
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (!bMatch)
                                                                    {
                                                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotChangedId), Localization.getResStr(toolparamNode.Name), iTargetPort.ToString(), curTool.ToString()));
                                                                    }
                                                                }
                                                                else if (toolparamNode.Name == toolSleepTempId)
                                                                {
                                                                    auxTemp1 = convertStringToTemp(toolparamNode.InnerText, xmlTunitsSrc);
                                                                    auxTemp2 = convertStringToTemp(nodeTrg.InnerText, xmlTunitsTrg);
                                                                    if (auxTemp1.UTI != auxTemp2.UTI)
                                                                    {
                                                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotChangedId), Localization.getResStr(toolparamNode.Name), iTargetPort.ToString(), curTool.ToString()));
                                                                    }
                                                                }
                                                                else if (toolparamNode.Name == toolAdjustTempId)
                                                                {
                                                                    auxTemp1 = convertStringToTempAdj(toolparamNode.InnerText, xmlTunitsSrc);
                                                                    auxTemp2 = convertStringToTempAdj(nodeTrg.InnerText, xmlTunitsTrg);
                                                                    if (auxTemp1.UTI != auxTemp2.UTI)
                                                                    {
                                                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotChangedId), Localization.getResStr(toolparamNode.Name), iTargetPort.ToString(), curTool.ToString()));
                                                                    }
                                                                }
                                                                else if ((toolparamNode.Name == toolSleepDelayId) || (toolparamNode.Name == toolHibernationDelayId))
                                                                {
                                                                    auxInt1 = Convert.ToInt32(toolparamNode.InnerText);
                                                                    auxInt2 = Convert.ToInt32(nodeTrg.InnerText);
                                                                    // enabled
                                                                    bMatch = true;
                                                                    // ver enabled
                                                                    bXmlEnabledExists = ConfigurationXML.getXmlEnabled(toolparamNode, ref bXmlEnabled);
                                                                    bXmlEnabledExistsTrg = ConfigurationXML.getXmlEnabled(nodeTrg, ref bXmlEnabledTrg);
                                                                    if (bXmlEnabledExists && !bXmlEnabledExistsTrg)
                                                                    {
                                                                        // enabled en source y no en target
                                                                        if (!bXmlEnabled)
                                                                        {
                                                                            // source no enabled
                                                                            if (auxInt2 != System.Convert.ToInt32(ToolTimeSleep.NO_SLEEP))
                                                                            {
                                                                                // target con delay -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // source enabled, comparar delays
                                                                            if (auxInt1 != auxInt2)
                                                                            {
                                                                                // diferente delay -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                    }
                                                                    else if (!bXmlEnabledExists && bXmlEnabledExistsTrg)
                                                                    {
                                                                        // no enabled en source y si en target
                                                                        if (auxInt1 == System.Convert.ToInt32(ToolTimeSleep.NO_SLEEP))
                                                                        {
                                                                            // si source está sin delay
                                                                            if (bXmlEnabledTrg)
                                                                            {
                                                                                // si el target está activo -> log
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            // si source tiene delay
                                                                            if (!bXmlEnabledTrg)
                                                                            {
                                                                                // si el target no está activo -> log
                                                                                bMatch = false;
                                                                            }
                                                                            else
                                                                            {
                                                                                // si el target está activo, comparar delays
                                                                                if (auxInt1 != auxInt2)
                                                                                {
                                                                                    // diferente temperatura -> log
                                                                                    bMatch = false;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        // los dos enabled o los dos sin enabled
                                                                        if (bXmlEnabledExists && bXmlEnabledExistsTrg && bXmlEnabled != bXmlEnabledTrg)
                                                                        {
                                                                            // si los dos tienen enabled y son diferentes -> log
                                                                            bMatch = false;
                                                                        }
                                                                        else
                                                                        {
                                                                            // comparar temperaturas
                                                                            if (auxInt1 != auxInt2)
                                                                            {
                                                                                bMatch = false;
                                                                            }
                                                                        }
                                                                    }
                                                                    if (!bMatch)
                                                                    {
                                                                        nodeLog = RoutinesLibrary.Data.Xml.XMLUtils.AddNode(nodeLogs, xmlLogId, string.Format(Localization.getResStr(logErrPortToolNotChangedId), Localization.getResStr(toolparamNode.Name), iTargetPort.ToString(), curTool.ToString()));
                                                                    }
                                                                }

                                                            }

                                                        } // If (Not bXmlCheckedExists) Or bXmlChecked
                                                    } // tool child nodes
                                                }
                                            }
                                        } // chidren port node
                                    }
                                }
                                else
                                {
                                    // no target port
                                } // If iTargetPort <= nTargetPorts And iTargetPort > 0

                            } // iTargetFromSourcePorts(iTarget) = iSourcePort

                        } // For iTarget = 0 To UBound(iTargetFromSourcePorts)

                    } // portNodes

                }

                return true;

            }
            catch (Exception ex)
            {
                Interaction.MsgBox("Error analizing configuration applied: " + ex.Message, MsgBoxStyle.Critical, null);
                return false;
            }

        }

        public static string confSaveLogXml(System.Xml.XmlDocument xmlDoc, string sTitle)
        {
            // return path file name of xml
            string sFilename = "ConfigSettingsLog_" + Strings.Format(DateTime.Now, sDatetimeForFilesFormat);
            string sTempPathJBC = sTempPath + sTempPathSubdir;
            string sXmlPathFileName = sTempPathJBC + sFilename + ".xml";
            string sXslPathFileName = sTempPathJBC + sFilename + ".xsl";

            if (xmlDoc != null)
            {
                if (!System.IO.Directory.Exists(sTempPathJBC))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(sTempPathJBC);
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                // copy de xls file in resources to temptapth
                // load xsl resource file as string
                string temp = My.Resources.Resources.ConfigSettingsLog; // get file as string
                                                                        // replace text
                temp = temp.Replace("#Title#", sTitle);
                temp = temp.Replace("#Parameter#", Localization.getResStr(xslParameterId));
                temp = temp.Replace("#Value#", Localization.getResStr(xslValueId));
                temp = temp.Replace("#_On#", Localization.getResStr(ON_STRId));
                temp = temp.Replace("#_Off#", Localization.getResStr(OFF_STRId));
                // write content
                var myFile = System.IO.File.CreateText(sXslPathFileName);
                myFile.WriteLine(temp);
                myFile.Close();

                // load texts according to current language
                confXlmDocLoadTexts(xmlDoc, sFilename + ".xsl", true);
                // save xml file
                xmlDoc.Save(sXmlPathFileName);
                return sXmlPathFileName;
            }
            else
            {
                return "";
            }
        }


        #endregion

        #region Settings Manager Routines

        private static System.Xml.XmlNode xmlGetStationNodeFromXml(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode returnValue = default(System.Xml.XmlNode);
            var sCurrentXmlPath = "/" + xmlRootId + "/" + xmlStationId;
            returnValue = xmlDoc.SelectSingleNode(sCurrentXmlPath);
            return returnValue;
        }

        private static System.Xml.XmlNode xmlGetStationParamsNodeFromXml(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode returnValue = default(System.Xml.XmlNode);
            var sCurrentXmlPath = "/" + xmlRootId + "/" + xmlStationId + "/" + xmlStationSettingsId;
            returnValue = xmlDoc.SelectSingleNode(sCurrentXmlPath);
            return returnValue;
        }

        private static System.Xml.XmlNode xmlGetPortsAndToolsNodeFromXml(System.Xml.XmlDocument xmlDoc)
        {
            System.Xml.XmlNode returnValue = default(System.Xml.XmlNode);
            var sCurrentXmlPath = "/" + xmlRootId + "/" + xmlStationId + "/" + xmlPortsAndToolsId;
            returnValue = xmlDoc.SelectSingleNode(sCurrentXmlPath);
            return returnValue;
        }

        private static System.Xml.XmlNode xmlGetRobotNodeFromXml(System.Xml.XmlDocument xmlDoc)
        {
            var sCurrentXmlPath = "/" + xmlRootId + "/" + xmlStationId + "/" + xmlRobotId;
            return xmlDoc.SelectSingleNode(sCurrentXmlPath);
        }

        private static System.Xml.XmlNode xmlGetProfileNodeFromXml(System.Xml.XmlDocument xmlDoc)
        {
            var sCurrentXmlPath = "/" + xmlRootId + "/" + xmlStationId + "/" + xmlProfileId;
            return xmlDoc.SelectSingleNode(sCurrentXmlPath);
        }

        private static System.Xml.XmlNode xmlGetPortNodeFromXml(System.Xml.XmlDocument xmlDoc, string sPortNumber)
        {
            System.Xml.XmlAttribute attrib = default(System.Xml.XmlAttribute);
            System.Xml.XmlNode xmlPortAndTools = xmlGetPortsAndToolsNodeFromXml(xmlDoc);
            if (xmlPortAndTools != null)
            {
                foreach (System.Xml.XmlElement node in xmlPortAndTools.ChildNodes)
                {
                    if (node.Name == xmlPortId)
                    {
                        attrib = node.Attributes[ConfigurationXML.xmlNumberId];
                        if (attrib != null)
                        {
                            if (attrib.Value == sPortNumber)
                            {
                                return node;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static System.Xml.XmlNode xmlGetToolNodeFromXml(System.Xml.XmlDocument xmlDoc, string sPortNumber, string sToolType)
        {
            System.Xml.XmlAttribute attrib = default(System.Xml.XmlAttribute);
            System.Xml.XmlNode xmlPortNode = xmlGetPortNodeFromXml(xmlDoc, sPortNumber);
            if (xmlPortNode != null)
            {
                foreach (System.Xml.XmlElement node in xmlPortNode.ChildNodes)
                {
                    if (node.Name == xmlToolId)
                    {
                        attrib = node.Attributes[ConfigurationXML.xmlTypeId];
                        if (attrib != null)
                        {
                            if (attrib.Value == sToolType)
                            {
                                return node;
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static GenericStationTools xmlGetToolFromToolNode(System.Xml.XmlNode xmlToolNode)
        {
            GenericStationTools returnValue = default(GenericStationTools);
            System.Xml.XmlAttribute attrib = default(System.Xml.XmlAttribute);
            attrib = xmlToolNode.Attributes[ConfigurationXML.xmlTypeId];
            if (attrib != null)
            {
                returnValue = (GenericStationTools)(int.Parse(attrib.Value));
            }
            else
            {
                attrib = xmlToolNode.Attributes[ConfigurationXML.xmlNumberId];
                if (attrib != null)
                {
                    returnValue = (GenericStationTools)(int.Parse(attrib.Value));
                }
            }
            return returnValue;
        }

        private static GenericStationTools[] xmlGetStationTools(System.Xml.XmlDocument xmlDoc)
        {
            //getting the supported tools of xml
            System.Xml.XmlAttribute xmlAttrib = default(System.Xml.XmlAttribute);
            GenericStationTools[] tools = null;
            var sCurrentXmlPath = "/" + xmlRootId + "/" + xmlStationId + "/" + xmlPortsAndToolsId;
            System.Xml.XmlNodeList toolPortNodes = xmlDoc.SelectNodes(sCurrentXmlPath);
            System.Xml.XmlNode port1Node = toolPortNodes[0].ChildNodes[0];
            int cnt = -1;
            string tooltype = "";

            tools = new GenericStationTools[0];
            foreach (System.Xml.XmlElement node in port1Node.ChildNodes)
            {
                if (node.Name == xmlToolId)
                {
                    xmlAttrib = node.Attributes[ConfigurationXML.xmlTypeId];
                    if (xmlAttrib != null)
                    {
                        cnt++;
                        Array.Resize(ref tools, cnt + 1);
                        tooltype = xmlAttrib.Value;
                        tools[cnt] = (GenericStationTools)int.Parse(tooltype);
                    }
                    else
                    {
                        xmlAttrib = node.Attributes[ConfigurationXML.xmlNumberId];
                        if (xmlAttrib != null)
                        {
                            cnt++;
                            Array.Resize(ref tools, cnt + 1);
                            tools[cnt] = (GenericStationTools)int.Parse(xmlAttrib.Value);
                        }
                    }
                }
            }
            return tools;
        }

        public static string getTextFromValueArray(string sValue, string[] arrayValues, string[] arrayTexts)
        {
            int idx = Array.IndexOf(arrayValues, sValue);
            if (idx >= 0)
            {
                return arrayTexts[idx];
            }
            else
            {
                return sValue;
            }
        }

        public static frmBrowser showXML(string sXmlPathFileName, bool bShow)
        {
            // show xml file in browser (using xsl to show texts and formatting)
            frmBrowser frmBrowser = new frmBrowser();
            frmBrowser.WebBrowser1.Navigate("file:" + sXmlPathFileName);
            if (bShow)
            {
                frmBrowser.Show();
                frmBrowser.WindowState = FormWindowState.Normal;
            }
            return frmBrowser;
        }

        #endregion

    }
}
