// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Diagnostics;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Linq;
// End of VB project level imports

using System.ServiceModel;
using System.Runtime.Serialization;

using JBC_Connect;
using DataJBC;

namespace JBCStationControllerSrv
{
	[ServiceContract(Namespace = "http://JBCStationControllerSrv")]
		public interface IJBCStationControllerService
		{

        #region StationController Service Management

        /// <summary>
        /// Get station controller information </summary>
        /// <returns>dc_StationController_Info class that contains station controller name, identifier and software version</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_StationController_Info GetStationControllerInfo();

        /// <summary>
        /// Set searching configuration and searching actions.
        /// If the action identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="action">The station action to be done</param>
        /// <param name="conntype">Indicates the connection type (USB or Ethernet)</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void StationControllerSearch(dc_EnumConstJBC.dc_StationControllerAction action, dc_EnumConstJBC.dc_StationControllerConnType conntype);

        /// <summary>
        /// Detecting if the station controller is actually searching 
        /// </summary>
        /// <param name="conntype">Indicates the connection type (USB or Ethernet)</param>
        /// <returns>True if the station is currently searching</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool StationControllerIsSearching(dc_EnumConstJBC.dc_StationControllerConnType conntype);

        #endregion

        #region Station List Operations

        /// <summary>
        /// Get number of stations found 
        /// </summary>
        /// <returns>An integer that is the number of stations connected</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				int GetStationCount();

        /// <summary>
        /// Get list of stations connected
        /// </summary>
        /// <returns>A string list that is the number of stations identifier connected</returns>
	    [OperationContract(), FaultContract(typeof(faultError))]
				string[] GetStationList();

        #endregion

        #region Station Info Operations

        /// <summary>
        /// Get specific station information 
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Station_Sold_Info class that contains the station information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Station_Sold_Info GetStationInfo(string stationUUID);

        //<OperationContract(), FaultContract(GetType(faultError))> _
        //Function GetStationType(ByVal stationUUID As String) As dc_EnumConstJBC.dc_StationType
        #endregion

        #region Station Status Operations

        /// <summary>
        /// Get status of specific soldering station
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Station_Sold_Status class that contains the station status</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Station_Sold_Status GetStationStatus(string stationUUID);

        /// <summary>
        /// Get status of the current selected Hot Air Desoldering station
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Station_HA_Status class that contains the Hot Air Desoldering station status</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Station_HA_Status GetStationStatus_HA(string stationUUID);

        /// <summary>
        /// Get status of the current selected Soldering Feeder station
        /// </summary>
        /// <remarks>Soldering Feeder stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Station_SF_Status class that contains the Soldering Feeder station status</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Station_SF_Status GetStationStatus_SF(string stationUUID);

        /// <summary>
        /// Get current control mode of the station
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>An integer that is the current control mode</returns>
		[OperationContract(), FaultContract(typeof(faultError))]
				dc_EnumConstJBC.dc_ControlModeConnection GetControlMode(string stationUUID);
     
        /// <summary>
        /// Enable (CONTROL) or disable (MONITOR) control mode of the station indicated
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="mode">The control mode of the desired station</param>
        /// <param name="userName">The user name</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetControlMode(string stationUUID, dc_EnumConstJBC.dc_ControlModeConnection mode, string userName);

        /// <summary>
        /// Set remote mode status on or off.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="onoff">A On/Off flag that switches the remote mode state</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetRemoteMode(string stationUUID, dc_EnumConstJBC.dc_OnOff onoff);

        /// <summary>
        /// Upgrade remote mode status of the station indicated.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
		[OperationContract(), FaultContract(typeof(faultError))]
				void KeepControlMode(string stationUUID);
			
        //Pendiente de implementación
			[OperationContract(), FaultContract(typeof(faultError))]
				bool ShowMessage(string stationUUID, string message, JBC_Connect.dc_EnumConstJBC.dc_MessageType type);


        #endregion

        #region Port Status Operations

        /// <summary>
        /// Get port information on the soldering station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <returns>A dc_StatusTool class that contains the port information of a specific soldering station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_StatusTool GetPortStatus(string stationUUID, dc_EnumConstJBC.dc_Port portNbr);

        /// <summary>
        /// Get port information on the current selected Hot Air Desoldering station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <returns>A dc_StatusTool_HA class that contains the port information of a specific Hot Air Desoldering station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_StatusTool_HA GetPortStatus_HA(string stationUUID, dc_EnumConstJBC.dc_Port portNbr);

        /// <summary>
        /// Get port information on the Soldering Feeder station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering Feeder stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <returns>A dc_StatusTool_SF class that contains the port information of a specific Soldering Feeder station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_StatusTool_SF GetPortStatus_SF(string stationUUID, JBC_Connect.dc_EnumConstJBC.dc_Port portNbr);

        /// <summary>
        /// Set ON or OFF the stand status, extractor mode and desolder mode of the soldering station 
        /// If the station identifier or the port identifier are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="sleep">The identifier of the stand status</param>
        /// <param name="extractor">A On/Off flag that switches the extractor mode</param>
        /// <param name="desolder">A On/Off flag that switches the desolder mode</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortStatusTool(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_OnOff sleep, dc_EnumConstJBC.dc_OnOff extractor,
				dc_EnumConstJBC.dc_OnOff desolder);

        /// <summary>
        /// Set ON or OFF the heater status and suction status of the Hot Air Desoldering station
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="heater">A On/Off flag that switches the heater status</param>
        /// <param name="suction">A On/Off flag that switches the suction status</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortStatusTool_HA(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_OnOff heater,
				dc_EnumConstJBC.dc_OnOff suction);

        /// <summary>
        /// Set Enable or Disable the port on a specific station 
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="enabled">A On/Off flag that switches the port state</param>
        /// <returns>True if the operation has been done correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool SetEnabledPort(string stationUUID, JBC_Connect.dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_OnOff enabled);

        #endregion

        #region Station Settings Operations

        /// <summary>
        /// Get settings information on the soldering station
        /// </summary>
        /// <remarks>Soldering station only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Station_Sold_Settings class that contains the station settings information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Station_Sold_Settings GetStationSettings(string stationUUID);

        /// <summary>
        /// Get settings information on the current selected Hot Air Desoldering station
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Station_HA_Settings class that contains the station settings information of a specific Hot Air Desoldering station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Station_HA_Settings GetStationSettings_HA(string stationUUID);

        /// <summary>
        /// Get settings information on the current selected Soldering Feeder station
        /// </summary>
        /// <remarks>Soldering Feeder stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Station_SF_Settings class that contains the station settings information of a specific Soldering Feeder station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Station_SF_Settings GetStationSettings_SF(string stationUUID);

        /// <summary>
        /// Get station PIN identifier of the current selected station
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A string that contains the station PIN identifier</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				string GetStationPIN(string stationUUID);

        /// <summary>
        /// Set station name of the current selected station
        /// If the station identifier is not correct or the indicated name is empty an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newName">The desired new station name</param>        
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationName(string stationUUID, string newName);

        /// <summary>
        /// Set station PIN of the current selected station
        /// If the station identifier is not correct or the indicated PIN is empty an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newPIN">The desired new station PIN</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationPIN(string stationUUID, string newPIN);

        /// <summary>
        /// Set station temperature units of the current selected station 
        /// If the station identifier is not correct or the temperature is out of range an erro event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newTempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationTempUnit(string stationUUID, string newTempUnit);

        /// <summary>
        /// Set inches or millimeters as a station length units
        /// If the station identifier or the length units are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newLengthUnit">The desired new station length units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationLengthUnit(string stationUUID, dc_EnumConstJBC.dc_LengthUnit newLengthUnit);

        /// <summary>
        /// Set minimum temperature of the current selected station
        /// If the station identifier is not correct or the temperature is out of range an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newTemp">The desired new minimum station temperature</param>
        /// <param name="tempUnit">The desired station temperature length units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationMinTemp(string stationUUID, int newTemp, string tempUnit);

        /// <summary>
        /// Set maximum temperature of the current selected station
        /// If the station identifier is not correct or the temperature is out of range an error event is thrown
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newTemp">The desired new maximum station temperature</param>
        /// <param name="tempUnit">The desired station temperature length units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationMaxTemp(string stationUUID, int newTemp, string tempUnit);

        /// <summary>
        /// Set the N2 mode status to the current selected soldering station
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="onoff">A On/Off flag that switches the N2 mode</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationN2Mode(string stationUUID, dc_EnumConstJBC.dc_OnOff onoff);

        /// <summary>
        /// Set help text status of the current selected station
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="onoff">A On/Off flag that switches the station help text status</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationHelpText(string stationUUID, dc_EnumConstJBC.dc_OnOff onoff);

        /// <summary>
        /// Set beep status of the current selected station 
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="onoff">A On/Off flag that switches the station beep status</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationBeep(string stationUUID, dc_EnumConstJBC.dc_OnOff onoff);

        /// <summary>
        /// Set lock status of the current selected station
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="locked">A On/Off flag that switches the lock status</param>
        /// <param name="message">the desired message to be shown</param>
        /// <param name="timeout">The desired locking time</param>
        /// <param name="dataEntry">Enable or Disable data entry</param>
        /// <returns>True if the station has been locked successfully</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool SetStationLocked(string stationUUID, dc_EnumConstJBC.dc_OnOff locked, string message, uint timeout, bool dataEntry);

        /// <summary>
        /// Set power limit of the current selected soldering station
        /// If the station identifier is not correct or the temperature is out of range an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newPowerLimit">The desired new station power limit</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationPowerLimit(string stationUUID, int newPowerLimit);

        /// <summary>
        /// Set minimum external temperature of the current station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newTemp">The desired new station temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        // HA desoldering
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationMinExtTemp(string stationUUID, int newTemp, string tempUnit);

        /// <summary>
        /// Set maximum external temperature of the current station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newTemp">The desired new station temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationMaxExtTemp(string stationUUID, int newTemp, string tempUnit);

        /// <summary>
        /// Set minimum flow of the current selected Hot Air Desoldering station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering station</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newFlow">The desired new station flow</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationMinFlow(string stationUUID, int newFlow);

        /// <summary>
        /// Set maximum flow of the current selected Hot Air Desoldering station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="newFlow">The desired new station flow</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationMaxFlow(string stationUUID, int newFlow);

        /// <summary>
        /// Set Enable or Disable the station PIN of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="onoff">A On/Off flag that switches the station PIN</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationPINEnabled(string stationUUID, dc_EnumConstJBC.dc_OnOff onoff);

        /// <summary>
        /// Set station program list of the current selected station
        /// </summary>
        /// <remarks>Soldering Feeder station only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="programList">The desired list of programs</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationConcatenateProgramList(string stationUUID, byte[] programList);

        /// <summary>
        /// Set a specific program of the current selected station
        /// </summary>
        /// <remarks>Soldering Feeder station only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="nbrProgram">The identifier of the desired station program</param>
        /// <param name="programDC">The desired content of the station program</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetStationProgram(string stationUUID, byte nbrProgram, dc_ProgramDispenserData_SF programDC);

        /// <summary>
        /// Delete a specific program of the current selected station
        /// </summary>
        /// <remarks>Soldering Feeder station only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="nbrProgram">The identifier of the desired station program</param>
        [OperationContract(), FaultContract(typeof(faultError))]
			void DeleteStationProgram(string stationUUID, byte nbrProgram);

        #endregion

        #region Port/tool Settings Operations

        /// <summary>
        /// Get tool settings on the soldering station
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A dc_PortToolSettings class that contains the tool settings information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_PortToolSettings GetPortToolSettings(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool);

        /// <summary>
        /// Get tool settings on the Hot Air Desoldering station 
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <returns>A dc_PortToolSettings_HA class that contains the tool settings configuration</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_PortToolSettings_HA GetPortToolSettings_HA(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool);

        /// <summary>
        /// Set temperature of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="temp">The desired new tool temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSelectedTemp(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, int temp, string tempUnit);

        /// <summary>
        /// Set temperature of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="signStep">The desired new tool temperature increment. it may be positive or negative</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSelectedTempStep(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, int signStep, string tempUnit);

        /// <summary>
        /// Set flow of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="flow">The desired new tool flow</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSelectedFlow(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, int flow);

        /// <summary>
        /// Set external temperature of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed </remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="temp">The desired new tool temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSelectedExtTemp(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, int temp, string tempUnit);

        /// <summary>
        /// Set external temperature of the current selected station tool  
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="signStep">The desired new tool temperature increment. it may be positive or negative</param>
        /// <param name="tempUnit">The desired temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSelectedExtTempStep(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, int signStep, string tempUnit);

        /// <summary>
        /// Set fixed temperature of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="temp">The desired new tool temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolFixTemp(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool,
					int temp, string tempUnit);

        // temp levels
        /// <summary>
        /// Set selection levels and temperature levels of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="LevelsOnOff">A On/Off flag that switches the station levels</param>
        /// <param name="LevelSelected">Selected temperature level</param>
        /// <param name="Level1OnOff">Enable or Disable the first level</param>
        /// <param name="Level1Temp">First level temperature</param>
        /// <param name="Level2OnOff">Enable or Disable the second level</param>
        /// <param name="Level2Temp">Second level temperature</param>
        /// <param name="Level3OnOff">Enable or Disable the third level</param>
        /// <param name="Level3Temp">Third level temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolLevels(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff LevelsOnOff, dc_EnumConstJBC.dc_ToolTemperatureLevels LevelSelected, dc_EnumConstJBC.dc_OnOff Level1OnOff, int Level1Temp, dc_EnumConstJBC.dc_OnOff Level2OnOff, int Level2Temp, dc_EnumConstJBC.dc_OnOff Level3OnOff, int Level3Temp, string tempUnit);

        /// <summary>
        /// Set selection levels, temperature and flow levels of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="LevelsOnOff">A On/Off flag that switches the station levels</param>
        /// <param name="LevelSelected">Selected temperature level</param>
        /// <param name="Level1OnOff">Enable or Disable the first level</param>
        /// <param name="Level1Temp">First level temperature</param>
        /// <param name="Level1Flow">First level flow</param>
        /// <param name="Level1ExtTemp">First level external temperature</param>
        /// <param name="Level2OnOff">Enable or Disable the second level</param>
        /// <param name="Level2Temp">Second level temperature</param>
        /// <param name="Level2Flow">Second level flow</param>
        /// <param name="Level2ExtTemp">Second level external temperature</param>
        /// <param name="Level3OnOff">Enable or Disable the third level</param>
        /// <param name="Level3Temp">Third level temperature</param>
        /// <param name="Level3Flow">Third level flow</param>
        /// <param name="Level3ExtTemp">Third level external temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolLevels_HA(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff LevelsOnOff, dc_EnumConstJBC.dc_ToolTemperatureLevels LevelSelected, dc_EnumConstJBC.dc_OnOff Level1OnOff, int Level1Temp, int Level1Flow, int Level1ExtTemp, dc_EnumConstJBC.dc_OnOff Level2OnOff, int Level2Temp, int Level2Flow, int Level2ExtTemp, dc_EnumConstJBC.dc_OnOff Level3OnOff, int Level3Temp, int Level3Flow, int Level3ExtTemp, string tempUnit);

        /// <summary>
        /// Get temperature level status of the current selected station tool
        /// If the station or prot identifier are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="LevelsOnOff">A On/Off flag that switches the station levels</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSelectedLevelEnabled(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff LevelsOnOff);

        /// <summary>
        /// Set level of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="LevelSelected">Selected temperature level</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSelectedLevel(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels LevelSelected);

        /// <summary>
        /// Set Enable or Disable the tool level on a specific station
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="Level">The identifier of the desired level</param>
        /// <param name="OnOff">The desired level status</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolTempLevelEnabled(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, dc_EnumConstJBC.dc_OnOff OnOff);

        /// <summary>
        /// Set temperature level of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="Level">The identifier of the desired level</param>
        /// <param name="LevelTemp">The desired new level temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolTempLevel(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, int LevelTemp, string tempUnit);

        /// <summary>
        /// Set external temperature level of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="Level">The identifier of the desired level</param>
        /// <param name="LevelTemp">The desired new level temperature</param>
        /// <param name="tempUnit">The desired new temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolExtTempLevel(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, int LevelTemp, string tempUnit);

        /// <summary>
        /// Set flow level of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="Level">The identifier of the desired level</param>
        /// <param name="LevelFlow">The desired new level flow</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolFlowLevel(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ToolTemperatureLevels Level, int LevelFlow);

        /// <summary>
        /// Set Enable or Disable the profile mode on a specific station
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="OnOff">The desired profile mode</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolProfileMode(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_OnOff OnOff);

        // end temp levels
        /// <summary>
        /// Set sleep delay of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="delay">The desired sleep delay</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSleepDelay(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_TimeSleep delay);

        /// <summary>
        /// Set Enable or Disable the sleep delay mode on a specific station tool
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="OnOff">A On/Off flag that switches the tool sleep delay mode</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSleepDelayEnabled(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff OnOff);

        /// <summary>
        /// Set hibernation delay of the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="delay">The desired tool delay</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolHibernationDelay(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_TimeHibernation delay);

        /// <summary>
        /// Set Enable of Disable the hibernation delay mode on a specific station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="OnOff">A On/Off flag that switches the tool hibernation delay</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolHibernationDelayEnabled(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff OnOff);

        /// <summary>
        /// Set sleeping temperature of the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="temp">the desired new tool temperature</param>
        /// <param name="tempUnit">The desired temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolSleepTemp(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, int temp, string tempUnit);

        /// <summary>
        /// Set temperature adjustments of the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <param name="stationUUID">Th identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="temp">The desired tool temperature</param>
        /// <param name="tempUnit">The desired temperature units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolAdjustTemp(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, int temp, string tempUnit);

        /// <summary>
        /// Set cartridge of the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="cartridge">The desired cartridge</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolCartridge(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_Cartridge cartridge);

        /// <summary>
        /// Set starting mode of the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="toolButton">the desaired tool button status</param>
        /// <param name="pedalAction">The desired pedal action</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolStartMode(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_OnOff toolButton, dc_EnumConstJBC.dc_PedalAction pedalAction);

        /// <summary>
        /// Set stop timer of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="timetostop">The desired time to stop</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolTimeToStop(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, int timetostop);

        /// <summary>
        /// Set working mode of the external TC in the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="tool">The identifier of the desired tool</param>
        /// <param name="mode">The desired external TC mode</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortToolExternalTCMode(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, dc_EnumConstJBC.dc_GenericStationTools tool, dc_EnumConstJBC.dc_ExternalTCMode_HA mode);

        /// <summary>
        /// Set dispensing mode of the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown. 
        /// </summary>
        /// <remarks>Soldering Feeder stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbrp">The identifier of the desired port</param>
        /// <param name="dispenserMode">The desired dispender mode</param>
        /// <param name="nbrProgram">The identifier of the desired station program</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortDispenserMode(string stationUUID, dc_EnumConstJBC.dc_Port portNbrp, dc_EnumConstJBC.dc_DispenserMode_SF dispenserMode, byte nbrProgram);

        /// <summary>
        /// Set feeder length of the current selected station tool
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering Feeder stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="length">The desired length</param>
        /// <param name="lengthUnit">The desired length units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortLength(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, double length, dc_EnumConstJBC.dc_LengthUnit lengthUnit);

        /// <summary>
        /// Set feeder speed of the current selected station tool 
        /// If the station or port identifier are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering Feeder stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <param name="speed">The desired speed</param>
        /// <param name="speedUnit">The desired speed units</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPortSpeed(string stationUUID, dc_EnumConstJBC.dc_Port portNbr, double speed, dc_EnumConstJBC.dc_SpeedUnit speedUnit);

        #endregion

        #region Counters Operations

        // get
        /// <summary>
        /// Get port counters of the current selected station
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <returns>A dc_Port_Counters class that contains the port counters information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Port_Counters GetPortCounters(string stationUUID, dc_EnumConstJBC.dc_Port portNbr);

        /// <summary>
        /// Get port counters of the current selected Hot Air Desoldering station
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <returns>A dc_Port_Counters_HA class that contains the port counters information of the Hot Air Desoldering station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Port_Counters_HA GetPortCounters_HA(string stationUUID, dc_EnumConstJBC.dc_Port portNbr);

        /// <summary>
        /// Get port counters of the current selected Soldering Feeder station
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering Feeder stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        /// <returns>A dc_Port_Counters_SF class that contains the port counters information of the Soldering Feeder station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Port_Counters_SF GetPortCounters_SF(string stationUUID, dc_EnumConstJBC.dc_Port portNbr);

        // set
        /// <summary>
        /// Set partial counters of the current selected station 
        /// If the station or port identifiers are not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="portNbr">The identifier of the desired port</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void ResetPortPartialCounters(string stationUUID, dc_EnumConstJBC.dc_Port portNbr);

        #endregion

        #region Continuous Mode Operations

        // FALTA: implemetar queue speed a nivel wcf
        /// <summary>
        /// Gets the current continuous data transmission mode status of the indicated station.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_ContinuousModeStatus object with the current status</returns>
        /// <remarks></remarks>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_ContinuousModeStatus GetContinuousMode(string stationUUID);

        /// <summary>
        /// Gets the current continuous mode data transmissions pending to be got from the internal FIFO queue of the indicated station.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks></remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="traceID">The queue ID returned by StartContinuousMode</param>
        /// <returns>An integer that is the queue length</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				int GetContinuousModeDataCount(string stationUUID, uint traceID);

        /// <summary>
        /// Starts a new continuous data queue instance on the indicated station and returns a queue ID.
        /// The desired transmission speed and ports will be the ones defined in SetContinuousMode Method.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A new queue id of the continuous mode to be used when retrieving the data</returns>
        /// <remarks></remarks>
        [OperationContract(), FaultContract(typeof(faultError))]
				uint StartContinuousMode(string stationUUID);

        /// <summary>
        /// Stops and clear the current continuous mode data transmission of the indicated queue.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="traceID">The queue ID returned by StartContinuousMode</param>
        /// <remarks></remarks>
        [OperationContract(), FaultContract(typeof(faultError))]
				void StopContinuousMode(string stationUUID, uint traceID);

        /// <summary>
        /// Gets the next continuous mode data in the internal FIFO queue from the station. It is the oldest transmission.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="traceID">The queue ID returned by StartContinuousMode</param>
        /// <returns>A dc_ContinuousModeData object that is the oldest transmission in the queue</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_ContinuousModeData GetContinuousModeNextData(string stationUUID, uint traceID);

        /// <summary>
        /// Gets the next continuous mode data in the internal FIFO queue from the station. It is the oldest transmission.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="traceID">The queue ID returned by StartContinuousMode</param>
        /// <returns>A dc_ContinuousModeData_HA object that is the oldest transmission in the queue</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_ContinuousModeData_HA GetContinuousModeNextData_HA(string stationUUID, uint traceID);

        /// <summary>
        /// Gets the next continuous mode data chunck in the internal FIFO queue from the station. It is the oldest transmission.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="traceID">The queue ID returned by StartContinuousMode</param>
        /// <param name="iChunk">The chunk ID returned by StartContinuousMode</param>
        /// <returns>A dc_ContinuousModeData array that contains the oldest transmissions in the queue</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_ContinuousModeData[] GetContinuousModeNextDataChunk(string stationUUID, uint traceID, int iChunk);

        /// <summary>
        /// Gets the next continuous mode data chunck in the internal FIFO queue from the station. It is the oldest transmission.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="traceID">The queue ID returned by StartContinuousMode</param>
        /// <param name="iChunk">The chunk ID returned by StartContinuousMode</param>
        /// <returns>A dc_ContinuousModeData_HA array that contains the oldest transmissions in the queue</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_ContinuousModeData_HA[] GetContinuousModeNextDataChunk_HA(string stationUUID, uint traceID, int iChunk);

        /// <summary>
        /// Defines the speed and ports to be used in continuous data transmission mode of the indicated station.
        /// The desired transmission speed ( period ) and at least one port must be indicated when defining.
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="speed">The desired speed ( period ) for the transmissions</param>
        /// <param name="portA">First desired port to be monitorized</param>
        /// <param name="portB">Second desired port to be monitorized</param>
        /// <param name="portC">Third desired port to be monitorized</param>
        /// <param name="portD">Fourth desired port to be monitorized</param>
        /// <remarks></remarks>	
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetContinuousMode(string stationUUID, dc_EnumConstJBC.dc_SpeedContinuousMode speed, dc_EnumConstJBC.dc_Port portA = default(dc_EnumConstJBC.dc_Port), dc_EnumConstJBC.dc_Port portB = default(dc_EnumConstJBC.dc_Port), dc_EnumConstJBC.dc_Port portC = default(dc_EnumConstJBC.dc_Port), dc_EnumConstJBC.dc_Port portD = default(dc_EnumConstJBC.dc_Port));

        #endregion

        #region Communication Operations

        /// <summary>
        /// Get Ethernet configuration of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_EthernetConfiguration class that contains the network connection information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_EthernetConfiguration GetEthernetConfiguration(string stationUUID);

        /// <summary>
        /// Get Robot configuration of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_RobotConfiguration class that contains the robot mode configuration</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_RobotConfiguration GetRobotConfiguration(string stationUUID);

        /// <summary>
        /// Set Ethernet configuration of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="ethernetConfiguration">The desired ethernet configuration mode</param> 
        [OperationContract(), FaultContract(typeof(faultError))]
                void SetEthernetConfiguration(string stationUUID, dc_EthernetConfiguration ethernetConfiguration);

        /// <summary>
        /// Set Robot configuration of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="robotConfiguration">The desired robot mode configuration</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetRobotConfiguration(string stationUUID, dc_RobotConfiguration robotConfiguration);

        #endregion

        #region Peripheral Operations

        /// <summary>
        /// Get information about the peripherals of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_PeripheralInfo array that contains the peripheral information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_PeripheralInfo[] GetAllPeripheralInfo(string stationUUID);

        /// <summary>
        /// Set peripheral information of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Soldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="peripheralInfo">The desired information on the peripheral station</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetPeripheralInfo(string stationUUID, dc_PeripheralInfo peripheralInfo);

        #endregion

        #region Profile Operations

        // get
        /// <summary>
        /// Get profile list of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A dc_Profile_HA array that contains the profile information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_Profile_HA[] GetAllProfiles_HA(string stationUUID);

        /// <summary>
        /// Get the current selected profile station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>The identifier of the current selected station</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				string GetSelectedProfile_HA(string stationUUID);

        // set
        /// <summary>
        /// Set profile of the current selected station 
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="profile">The desired profile information</param>
        /// <returns>True if the profile has been set correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool SetProfile_HA(string stationUUID, dc_Profile_HA profile);

        // delete
        /// <summary>
        /// Delete profile of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="profileName">The identifier of the desired profile</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void DeleteProfile_HA(string stationUUID, string profileName);

        // sync operation
        /// <summary>
        /// Synchronize the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SyncProfiles_HA(string stationUUID);

        /// <summary>
        /// Get synchronization state of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <remarks>Hot Air Desoldering stations only allowed</remarks>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>True if the files have been synchronized (read/write)</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool SyncFinishedProfiles_HA(string stationUUID);

        #endregion

        #region Other Operations

        // set
        /// <summary>
        /// Set a transaction when the transaction event is done (TransactionFinished) of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <returns>A transaction ID in order to identify the transaction when the transaction event is done (TransactionFinished event)</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				uint SetTransaction(string stationUUID);

        /// <summary>
        /// Detecting if a transaction of the current selected station has finished
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="transactionID">The identifier of the desired transaction</param>
        /// <returns>True if the transaction has finished</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool QueryEndedTransaction(string stationUUID, uint transactionID);

        /// <summary>
        /// Set default parameters value of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void SetDefaultStationParams(string stationUUID);

        /// <summary>
        /// Restart the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void ResetStation(string stationUUID);

        #endregion

        #region Update Firmware

        /// <summary>
        /// Get list of stations pending of update 
        /// </summary>
        /// <returns>A list of pending stations</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				List<string> GetStationListUpdating();

        /// <summary>
        /// Update list of stations pending of update
        /// </summary>
        /// <param name="listStationsToUpdate">The desired list of stations</param>
        [OperationContract(), FaultContract(typeof(faultError))]
				void UpdateStations(List<dc_FirmwareStation> listStationsToUpdate);

        #endregion

        #region Traceability

        /// <summary>
        /// Enable the traceability module
        /// </summary>
        /// <param name="ServerCode">The desired traceability server code</param>
        /// <param name="Ip">The desired traceability server IP</param>
        /// <param name="Port">The desired traceability server Port</param>
        /// <remarks>Enables forwarding of all events</remarks>
        /// <returns>True if the module has been started correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool StartTraceability(string ServerCode, string Ip, ushort Port);

        /// <summary>
        /// Disable the traceability module
        /// </summary>
        /// <returns>True if the module has been stopped correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool StopTraceability();

        //Session
        /// <summary>
        /// Create a new user session associated with the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="p">the identifier of the desired station port</param>
        /// <param name="userCode">The user identifier</param>
        /// <param name="userName">The user name</param>
        /// <param name="inputDeviceID">The identifier of the keyboard input</param>
        /// <returns>True if the station is not associated yet and the operation was successful</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool NewUserSession(string stationUUID, dc_EnumConstJBC.dc_Port p, string userCode, string userName, string inputDeviceID);

        /// <summary>
        /// Close and delete user session associated with the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="p">The identifier of the desired station port</param>
        /// <returns>True if the user session has been closed correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool CloseUserSession(string stationUUID, dc_EnumConstJBC.dc_Port p);

        /// <summary>
        /// Get user Id associated with the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="p">The identifier of the desired station port</param>
        /// <returns>The user identifier if there is a station associated with</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				string GetAuthenticatedUser(string stationUUID, dc_EnumConstJBC.dc_Port p);

        //Date and Time
        /// <summary>
        /// Get the current date time
        /// </summary>
        /// <returns>A current long date time</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				long GetDate();

        /// <summary>
        /// Set the current date time 
        /// </summary>
        /// <param name="newBinaryDate">The desired date time</param>
        /// <returns>True if the current date has been set correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool SetDate(long newBinaryDate);

        //Station configuration
        /// <summary>
        /// Set configuration of the current selected station
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="p">The identifier of the desired station port</param>
        /// <param name="configuration">The desired configuration</param>
        /// <returns>True if the configuration has been set correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool LoadConfigurationPortStation(string stationUUID, dc_EnumConstJBC.dc_Port p, dc_ConfigurationPortStation configuration);


        //Recorded data
        /// <summary>
        /// Start recording traceability data
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="p">The identifier of the desired station port</param>
        /// <returns>True if the recording has started correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool StartRecordData(string stationUUID, dc_EnumConstJBC.dc_Port p);

        /// <summary>
        /// Stop recording traceability data 
        /// If the station identifier is not correct an error event is thrown.
        /// </summary>
        /// <param name="stationUUID">The identifier of the desired station</param>
        /// <param name="p">The identifier of the desired station port</param>
        /// <returns>True if the recording has stopped correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool StopRecordData(string stationUUID, dc_EnumConstJBC.dc_Port p);

        /// <summary>
        /// Get list of recorded data files
        /// </summary>
        /// <returns>A list of recorded files/returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				List<string> GetListRecordedDataFiles();

        /// <summary>
        /// Get recorded data of the current selected file
        /// </summary>
        /// <param name="fileName">The desired file name</param>
        /// <param name="nSequence">The desired number of sequence</param>
        /// <returns>A dc_TraceDataSequence class that contains the traceability data information</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				dc_TraceDataSequence GetRecordedData(string fileName, int nSequence);

        /// <summary>
        /// Delete recorded data file
        /// </summary>
        /// <param name="fileName">The desired file name</param>
        /// <returns>True if the file has been deleted correctly</returns>
        [OperationContract(), FaultContract(typeof(faultError))]
				bool DeleteRecordedDataFile(string fileName);

#endregion

		}
}
