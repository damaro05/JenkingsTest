// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using JBC_Connect;
using DataJBC;
using RoutinesJBC;
// End of VB project level imports

using Microsoft.Win32;

// Programa para desbloquear los parámetros bloqueados de fábrica
// SW number: 9996978 for CD/CF
namespace JBCAdvancedLevel
{
	
public partial class FormUnlockParams
{
	public FormUnlockParams()
	{
		InitializeComponent();
			
		//Added to support default instance behavour in C#
		if (defaultInstance == null)
			defaultInstance = this;
	}
		
#region Default Instance
		
	private static FormUnlockParams defaultInstance;
		
	/// <summary>
	/// Added by the VB.Net to C# Converter to support default instance behavour in C#
	/// </summary>
	public static FormUnlockParams Default
	{
		get
		{
			if (defaultInstance == null)
			{
				defaultInstance = new FormUnlockParams();
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
	private JBC_API jbc;
	private StationsHashtable stationList;
		
	private const string tLockedParametersStatus = "Basic";
	private const string tLockParameters = "Set to Basic Level";
	private const string tLockParametersHint = "Lock some parameters for this station.";
	private const string tUnlockedParametersStatus = "Advanced";
	private const string tUnlockParameters = "Set to Advanced Level";
	private const string tUnlockParametersHint = "Unlock some advanced parameters for this station.";
	private const string tNotAvailable = "Not available";
	private const string tNotAvailableHint = "This station do not have the Basic/Advanced Level feature.";
	private const string tCol_Sel = "colSelParametersLocked";
	private const string tCol_But = "colButLockUnlock";
	private const string tCol_ID = "colID";
		
	public void FormUnlockParams_Load(System.Object sender, System.EventArgs e)
	{
		CheckForIllegalCrossThreadCalls = false;
			
		SplitContainer1.Panel2Collapsed = true;
			
		stationList = new StationsHashtable();
		jbc = new JBC_API();
		jbc.NewStationConnected += jbc_NewStationConnected;
		jbc.StationDisconnected += jbc_StationDisconnected;
		jbc.UserError += jbc_UserError;
		jbc.StartSearch(SearchMode.USB);
			
	}
		
	public void FormUnlockParams_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
	{
		if (jbc != null)
		{
			jbc.Close();
		}
	}
		
	public void timerStationListData_Tick(System.Object sender, System.EventArgs e)
	{
		// timer to update station list data
			
		((Timer) sender).Stop();
		// activated when a new station is connected (addStation function)
		bool bAllWithData = true;
		foreach (long stnID in stationList.TableIDs)
		{
			tStation stn = stationList.GetStation(stnID);
			if (stn.bStationShowed == false)
			{
				stn.sModel = jbc.GetStationModel(stn.ID);
				stn.sModelType = jbc.GetStationModelType(stn.ID);
				stn.iModelVersion = jbc.GetStationModelVersion(stn.ID);
				stn.sName = jbc.GetStationName(stn.ID);
				stn.sSW = jbc.GetStationSWversion(stn.ID);
				stn.sHW = jbc.GetStationHWversion(stn.ID);
				stn.sStationCOM = jbc.GetStationCOM(stn.ID);
				stn.sProtocol = jbc.GetStationProtocol(stn.ID);
				stn.Features = jbc.GetStationFeatures(stn.ID);
				// TEST ONLU
				//stn.Features.ParamsLockedFrame = True
					
				if (!string.IsNullOrEmpty(stn.sModel)&& !string.IsNullOrEmpty(stn.sSW))
				{
					if (string.IsNullOrEmpty(stn.sName))
					{
						stn.sName = "No Name";
					}
					// show
					LogAdd(string.Format("Connected - Name:{0} - Model:{1} - Type:{2} - Version:{3}", stn.sName, stn.sModel, stn.sModelType, stn.iModelVersion.ToString()));
					LogAdd(string.Format("            SW:{0} - HW:{1} - COM:{2} - Protocol:{3}", stn.sSW, stn.sHW, stn.sStationCOM, stn.sProtocol));
					stn.bStationShowed = true;
						
					addUpdateGrid(stn);
				}
				else
				{
					bAllWithData = false;
				}
				stationList.SetStation(stnID, stn);
			}
		}
		if (!bAllWithData)
		{
			// continue with timer
			((Timer) sender).Start();
		}
			
	}
		
	private void addUpdateGrid(tStation stn)
	{
		int iRow = -1;
		for (var i = 0; i <= gridStations.Rows.Count - 1; i++)
		{
			if ((int) (gridStations.Rows[i].Cells[tCol_ID].Value) == stn.ID)
			{
				iRow = System.Convert.ToInt32(i);
				break;
			}
		}
		if (iRow < 0)
		{
			iRow = gridStations.Rows.Add();
		}
		DataGridViewRow row = gridStations.Rows[iRow];
			
		row.Cells[tCol_ID].Value = stn.ID;
		row.Cells["colName"].Value = stn.sName;
		row.Cells["colModel"].Value = stn.sModel;
		row.Cells["colModelType"].Value = stn.sModelType;
		row.Cells["colModelVersion"].Value = stn.iModelVersion;
		row.Cells["colSW"].Value = stn.sSW;
		row.Cells["colHW"].Value = stn.sHW;
		row.Cells["colCOM"].Value = stn.sStationCOM;
		row.Cells["colProtocol"].Value = stn.sProtocol;
			
		if (stn.Features.ParamsLockedFrame)
		{
			row.Cells[tCol_But].ReadOnly = false;
			row.Cells[tCol_Sel].ReadOnly = true;
				
			CStation_SOLD instanceStn = jbc.Station(stn.ID);
			if (instanceStn.GetStationParametersLocked() == OnOff._ON)
			{
				row.Cells[tCol_But].Value = tUnlockParameters;
				row.Cells[tCol_But].ToolTipText = tUnlockParametersHint;
				row.Cells[tCol_Sel].Value = tLockedParametersStatus;
			}
			else
			{
				row.Cells[tCol_But].Value = tLockParameters;
				row.Cells[tCol_But].ToolTipText = tLockParametersHint;
				row.Cells[tCol_Sel].Value = tUnlockedParametersStatus;
			}
		}
		else
		{
			row.Cells[tCol_But].ReadOnly = true;
			row.Cells[tCol_But].Value = tNotAvailable;
			row.Cells[tCol_But].ToolTipText = tNotAvailableHint;
				
				
			row.Cells[tCol_Sel].ReadOnly = true;
			row.Cells[tCol_Sel].Value = tUnlockedParametersStatus;
		}
			
	}
		
	private void removeGrid(long stationID)
	{
		int iRow = -1;
		for (var i = 0; i <= gridStations.Rows.Count - 1; i++)
		{
			if ((int) (gridStations.Rows[i].Cells[tCol_ID].Value) == stationID)
			{
				iRow = System.Convert.ToInt32(i);
				break;
			}
		}
		if (iRow >= 0)
		{
			gridStations.Rows.RemoveAt(iRow);
		}
	}
		
	private void setParametersLocked(long stationID, bool bLocked)
	{
		CStation_SOLD instanceStn = jbc.Station(stationID);
		instanceStn.SetControlMode(ControlModeConnection.CONTROL);
		if (bLocked)
		{
			instanceStn.SetStationParametersLocked(OnOff._ON);
		}
		else
		{
			instanceStn.SetStationParametersLocked(OnOff._OFF);
		}
        instanceStn.SetControlMode(ControlModeConnection.MONITOR);
	}
		
	public void gridStations_CellClick(System.Object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
	{
		DataGridViewCell cellBut = default(DataGridViewCell);
		DataGridViewCell cellSel;
		if (e.RowIndex < 0)
		{
			return ;
		}
		if (gridStations.Columns[e.ColumnIndex].Name != tCol_But && gridStations.Columns[e.ColumnIndex].Name != tCol_Sel)
		{
			return ;
		}
		cellBut = gridStations.Rows[e.RowIndex].Cells[tCol_But];
		cellSel = gridStations.Rows[e.RowIndex].Cells[tCol_Sel];
		if (cellBut.Value != tLockParameters && cellBut.Value != tUnlockParameters)
		{
			return ;
		}
			
		long stationID = System.Convert.ToInt64(System.Convert.ToUInt32(gridStations.Rows[e.RowIndex].Cells[tCol_ID].Value));
		bool bUnlock = true;
			
		if (gridStations.Columns[e.ColumnIndex].Name == tCol_But)
		{
			if ((string) cellBut.Value == tLockParameters)
			{
				setParametersLocked(stationID, true);
				cellBut.Value = tUnlockParameters;
				cellSel.Value = tLockedParametersStatus;
			}
			else if ((string) cellBut.Value == tUnlockParameters)
			{
				setParametersLocked(stationID, false);
				cellBut.Value = tLockParameters;
				cellSel.Value = tUnlockedParametersStatus;
			}
		}
		if (gridStations.Columns[e.ColumnIndex].Name == tCol_Sel)
		{
			if ((string) cellSel.Value == tUnlockedParametersStatus)
			{
				setParametersLocked(stationID, true);
				cellBut.Value = tUnlockParameters;
				cellSel.Value = tLockedParametersStatus;
			}
			else if ((string) cellSel.Value == tLockedParametersStatus)
			{
				setParametersLocked(stationID, false);
				cellBut.Value = tLockParameters;
				cellSel.Value = tUnlockedParametersStatus;
			}
				
		}
			
	}
		
		
	public void gridStations_CellMouseClick(System.Object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
	{
			
	}
		
	private void jbc_NewStationConnected(long stationID)
	{
		//required as long as this method is called from a diferent thread
		if (this.InvokeRequired)
		{
			this.Invoke(new JBC_API.NewStationConnectedEventHandler(jbc_NewStationConnected), new object[] {stationID});
			return;
		}
			
		//new station connected, adding it
		tStation newStation = addStation(stationID);
			
		// #edu# activate timer to see if data is available to fill the grid for this station
		timerStationListData.Start();
			
	}
		
	private void jbc_StationDisconnected(long stationID)
	{
		//required as long as this method is called from a diferent thread
		if (this.InvokeRequired)
		{
			this.Invoke(new JBC_API.StationDisconnectedEventHandler(jbc_StationDisconnected), new object[] {stationID});
			return;
		}
			
		tStation stn = stationList.GetStation(stationID);
		LogAdd(string.Format("Disconnected - Name:{0} - Model:{1} - Type:{2} - Version:{3}", stn.sName, stn.sModel, stn.sModelType, stn.iModelVersion.ToString()));
			
		rmvStation(stationID);
			
		removeGrid(stationID);
			
	}
		
	private void jbc_UserError(long stationID, Cerror err)
	{
		//For Each stn As tStation In stationList
		//    If stn.ID = stationID Then
		//        ' #edu# show error only if it is found
		//        ' get station data from station list
		//        Dim sStationData As String = ""
		//        Dim model = stn.stationModel
		//        Dim name = stn.stationName
		//        sStationData = getResStr(confStationId) & " " & model & " - " & name
		//        Dim errorCode As JBC_Connect.Cerror.cErrorCodes = err.GetCode()
		//        Dim commErrorCode As JBC_Connect.Cerror.cCommErrorCodes = err.GetCommErrorCode()
		//        Dim sErrorMsg As String = ""
		//        Dim sCommErrorMsg As String = ""
		//        Dim sErrorText As String = ""
		//        Select Case errorCode
		//            Case JBC_Connect.Cerror.cErrorCodes.STATION_ID_NOT_FOUND
		//                sErrorMsg = getResStr(ueSTATION_ID_NOT_FOUND)
		//            Case JBC_Connect.Cerror.cErrorCodes.CONTINUOUS_MODE_ON_WITHOUT_PORTS
		//                sErrorMsg = getResStr(ueCONTINUOUS_MODE_ON_WITHOUT_PORTS)
		//            Case JBC_Connect.Cerror.cErrorCodes.PORT_NOT_IN_RANGE
		//                sErrorMsg = getResStr(uePORT_NOT_IN_RANGE)
		//            Case JBC_Connect.Cerror.cErrorCodes.INVALID_STATION_NAME
		//                sErrorMsg = getResStr(ueINVALID_STATION_NAME)
		//            Case JBC_Connect.Cerror.cErrorCodes.TEMPERATURE_OUT_OF_RANGE
		//                sErrorMsg = getResStr(ueTEMPERATURE_OUT_OF_RANGE)
		//            Case JBC_Connect.Cerror.cErrorCodes.STATION_ID_OVERFLOW
		//                sErrorMsg = getResStr(ueSTATION_ID_OVERFLOW)
		//            Case JBC_Connect.Cerror.cErrorCodes.POWER_LIMIT_OUT_OF_RANGE
		//                sErrorMsg = getResStr(uePOWER_LIMIT_OUT_OF_RANGE)
		//            Case JBC_Connect.Cerror.cErrorCodes.TOOL_NOT_SUPPORTED
		//                sErrorMsg = getResStr(ueTOOL_NOT_SUPPORTED)
		//            Case JBC_Connect.Cerror.cErrorCodes.FUNCTION_NOT_SUPPORTED
		//                sErrorMsg = getResStr(ueFUNCTION_NOT_SUPPORTED)
		//            Case JBC_Connect.Cerror.cErrorCodes.COMMUNICATION_ERROR
		//                sErrorMsg = getResStr(ueCOMMUNICATION_ERROR)
		//        End Select
		//        Select Case commErrorCode
		//            Case JBC_Connect.Cerror.cCommErrorCodes.NO_COMM_ERROR
		//                'sCommErrorMsg = getResStr(ceNO_COMM_ERROR)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.BCC
		//                sCommErrorMsg = getResStr(ceBCC)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.FRAME_FORMAT
		//                sCommErrorMsg = getResStr(ceFRAME_FORMAT)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.OUT_OF_RANGE
		//                sCommErrorMsg = getResStr(ceOUT_OF_RANGE)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.COMMAND_REJECTED
		//                sCommErrorMsg = getResStr(ceCOMMAND_REJECTED)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.CONTROL_MODE_REQUIRED
		//                sCommErrorMsg = getResStr(ceCONTROL_MODE_REQUIRED)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.INCORRECT_SEQUENCY
		//                sCommErrorMsg = getResStr(ceINCORRECT_SEQUENCY)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.FLASH_WRITE_ERROR
		//                sCommErrorMsg = getResStr(ceFLASH_WRITE_ERROR)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.CONTROL_MODE_ALREADY_ACTIVATED
		//                sCommErrorMsg = getResStr(ceCONTROL_MODE_ALREADY_ACTIVATED)
		//            Case JBC_Connect.Cerror.cCommErrorCodes.NOT_VALID_HARDWARE
		//                sCommErrorMsg = getResStr(ceNOT_VALID_HARDWARE)
		//        End Select
		//        If sErrorMsg <> "" Then
		//            If sErrorText <> "" Then sErrorText = sErrorText & vbCrLf
		//            sErrorText = sErrorText & getResStr(ueErrorId) & getResStr(gralValueSeparatorId) & sErrorMsg
		//        End If
		//        If sCommErrorMsg <> "" Then
		//            If sErrorText <> "" Then sErrorText = sErrorText & vbCrLf
		//            sErrorText = sErrorText & getResStr(ceErrorId) & getResStr(gralValueSeparatorId) & sCommErrorMsg
		//        End If
		//        If sErrorText <> "" Then
		//            MsgBox(sErrorText, MsgBoxStyle.OkOnly, sStationData)
		//        End If
		//        Exit For ' #edu 28/05/2014
		//    End If
		//Next
		//' #edu#
		//'MsgBox("Error of station " & stationID & ". Code: " & err.GetCode() & " -> " & err.GetMsg())
	}
		
	public void Label1_DoubleClick(System.Object sender, System.EventArgs e)
	{
		SplitContainer1.Panel2Collapsed = !SplitContainer1.Panel2Collapsed;
	}
		
#region Rutinas
	//Add station to the list
	private tStation addStation(long stationID)
	{
		//Creating the station structure
		tStation dataStation = new tStation(stationID);
			
		//adding the structure to the list
		stationList.AddStation(stationID, dataStation);
			
		//returning the new added station object
		return dataStation;
	}
		
	//Rmv station from the list
	private void rmvStation(long stationID)
	{
		stationList.RemoveStation(stationID);
	}
		
	private object LockLogAdd = new object();
	public void LogAdd(string sText)
	{
		//SyncLock LockLogAdd
		ListBox1.Items.Add(sText);
		//End SyncLock
	}
#endregion
		
#region STATION CLASS
	internal class tStation
	{
		public long ID = -1;
		public string sName = "";
		public string sModel = "";
		public string sModelType = "";
		public int iModelVersion = 0;
		public string sSW = "";
		public string sHW = "";
		public string sStationCOM = "";
		public string sProtocol = "";
		public bool bStationShowed = false;
		public CFeaturesData Features;
		public tStation(long stationID)
		{
			ID = stationID;
		}
	}
#endregion
		
#region STATION LIST HASHTABLE
		
	internal class StationsHashtable
	{
		//Se usa para mantener los servicios descubiertos
		private Hashtable StationsTableInt = new Hashtable();
			
		public StationsHashtable()
		{
			StationsTableInt.Clear();
		}
			
		// Existe la estación
		public bool ExistsStation(long stationID)
		{
			return StationsTableInt.ContainsKey(stationID);
		}
			
		// Añade una estación
		public void AddStation(long stationID, tStation NewStation)
		{
			if (stationID >= 0)
			{
				if (ReferenceEquals(NewStation, null))
				{
					NewStation = new tStation(stationID);
				}
				NewStation.ID = stationID;
				StationsTableInt.Add(stationID, NewStation);
			}
		}
			
		// Lee una estación
		public tStation GetStation(long stationID)
		{
			if (ExistsStation(stationID))
			{
				return ((tStation) (StationsTableInt[stationID]));
			}
			else
			{
				return null;
			}
		}
			
		// modifica una estación
		public bool SetStation(long stationID, tStation ChangedStation)
		{
			if (ExistsStation(stationID))
			{
				StationsTableInt[stationID] = ChangedStation;
				return true;
			}
			else
			{
				return false;
			}
				
		}
			
		// Borra una estación de la colección
		public void RemoveStation(long stationID)
		{
			StationsTableInt.Remove(stationID);
		}
			
		// Devuelve el número de estaciones
		public int Count
		{
			get
			{
				return StationsTableInt.Count;
			}
		}
			
		// Devuelve la colección de IDs de estaciones
		public List<long> TableIDs
		{
			get
			{
				List<long> lista = new List<long>();
				foreach (long stnID in StationsTableInt.Keys)
				{
					lista.Add(stnID);
				}
				return lista;
			}
		}
			
		// Borra toda la tabla
		public void Reset()
		{
			StationsTableInt.Clear();
		}
	}
		
#endregion
		
}
	
	
	
}
