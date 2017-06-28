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
// End of VB project level imports

using Microsoft.Win32;
using DataJBC;
using JBC_Connect;
using RoutinesJBC;

// Programa para borrar contadores globales y/o parciales
namespace JBCResetCounters
{
	
	public partial class FormResetCounters
	{
		public FormResetCounters()
		{
			InitializeComponent();
			
			//Added to support default instance behavour in C#
			if (defaultInstance == null)
				defaultInstance = this;
		}
		
#region Default Instance
		
		private static FormResetCounters defaultInstance;
		
		/// <summary>
		/// Added by the VB.Net to C# Converter to support default instance behavour in C#
		/// </summary>
		public static FormResetCounters Default
		{
			get
			{
				if (defaultInstance == null)
				{
					defaultInstance = new FormResetCounters();
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
		
		//Private Const tResetPartialCountersHint = "Reset Partial counters."
		//Private Const tResetGlobalCountersHint = "Reset counters."
		//Private Const tResetGlobalAndPartialCountersHint = "Reset Global and Partial counters."
		
		//Private Const tNotAvailable = "Not available"
		//Private Const tNotAvailableHint = "This station do not allow partial counters."
		//Private Const tNotProtocol01Hint = "Cannot reset protocol 01 counters."
		
		private const string tCol_But_All = "colButResetAllCounters";
		private const string tCol_But_Partial = "colButResetPartialCounters";
		private const string tCol_ID = "colID";
		private const string tCol_StationName = "colName";
		
		private const string tCol_Global = "col_Global_Port_";
		private const string tCol_Partial = "col_Partial_Port_";
		
		private bool bShowCountersFirstTime = false;
		private uint uiTransact = UInt32.MaxValue;
		private int iStationIDTransact = -1;
		
		private enum rowCounter
		{
			Plugged = 0,
			Work = 1,
			Idle = 2,
			Sleep = 3,
			Hiber = 4,
			SleepC = 5,
			DesoldC = 6
		}
		
		public void FormResetCounters_Load(System.Object sender, System.EventArgs e)
		{
			CheckForIllegalCrossThreadCalls = false;
			
			stationList = new StationsHashtable();
			jbc = new JBC_API();
			jbc.NewStationConnected += jbc_NewStationConnected;
			jbc.StationDisconnected += jbc_StationDisconnected;
			jbc.UserError += jbc_UserError;
			jbc.StartSearch(SearchMode.USB);
			
			loadTexts("es");
			
		}
		
		public void FormResetCounters_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			if (jbc != null)
			{
				jbc.Close();
			}
		}
		
		private void loadTexts(string sLang)
		{
			Localization.changeCulture(sLang);
			
			this.Text = Localization.getResStr("tApplicTitle");
			labStationsFound.Text = Localization.getResStr("tStationsFound");
			labStationCounters.Text = Localization.getResStr("tStationCounters");
			foreach (DataGridViewColumn col in gridStations.Columns)
			{
				col.HeaderText = Localization.getResStr("t" + col.Name);
			}
			
			//gridCounters.Columns("").HeaderText = ""
			foreach (DataGridViewColumn col in gridCounters.Columns)
			{
				string[] colName = col.Name.Split('_');
				switch (colName[1])
				{
					case "Global":
						col.HeaderText = string.Format(Localization.getResStr("tGlobalCounterPort"), int.Parse(colName[3]));
						break;
					case "Partial":
						col.HeaderText = string.Format(Localization.getResStr("tPartialCounterPort"), int.Parse(colName[3]));
						break;
					case "Counter":
						col.HeaderText = Localization.getResStr("tCounter");
						break;
				}
			}
			
			initGridCounters();
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
				if (stn != null)
				{
					if (stn.bStationShowed == false)
					{
						CStation_SOLD objStation = jbc.Station(stnID);
						LogAdd(string.Format("Station {0} is initialized: {1} ", stnID.ToString(), objStation.IsInitialized.ToString()));
						
						stn.sModel = jbc.GetStationModel(stn.ID);
						stn.sModelType = jbc.GetStationModelType(stn.ID);
						stn.iModelVersion = jbc.GetStationModelVersion(stn.ID);
						stn.sName = jbc.GetStationName(stn.ID);
						stn.sSW = jbc.GetStationSWversion(stn.ID);
						stn.sHW = jbc.GetStationHWversion(stn.ID);
						stn.sStationCOM = jbc.GetStationCOM(stn.ID);
						stn.sProtocol = jbc.GetStationProtocol(stn.ID);
						stn.Features = jbc.GetStationFeatures(stn.ID);
						
						LogAdd(string.Format("Station {0} exists: {1} ", stn.ID.ToString(), jbc.StationExists(stn.ID).ToString()));
						LogAdd(string.Format("Connected - Name:{0} - Model:{1} - Type:{2} - Version:{3}", stn.sName, stn.sModel, stn.sModelType, stn.iModelVersion.ToString()));
						LogAdd(string.Format("            SW:{0} - HW:{1} - COM:{2} - Protocol:{3}", stn.sSW, stn.sHW, stn.sStationCOM, stn.sProtocol));
						
						//If stn.sModel <> "" And stn.sSW <> "" Then
						if (!string.IsNullOrEmpty(stn.sModel))
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
			
			if (stn.Features.PartialCounters)
			{
				row.Cells[tCol_But_Partial].Value = Localization.getResStr("tResetPartialCountersHint");
				row.Cells[tCol_But_Partial].ToolTipText = Localization.getResStr("tResetPartialCountersHint");
				row.Cells[tCol_But_Partial].ReadOnly = false;
				
				row.Cells[tCol_But_All].Value = Localization.getResStr("tResetAllCounters");
				row.Cells[tCol_But_All].ToolTipText = Localization.getResStr("tResetGlobalAndPartialCountersHint");
				row.Cells[tCol_But_All].ReadOnly = false;
			}
			else
			{
				row.Cells[tCol_But_Partial].Value = Localization.getResStr("tNotAvailable");
				row.Cells[tCol_But_Partial].ToolTipText = Localization.getResStr("tNotAvailableHint");
				row.Cells[tCol_But_Partial].ReadOnly = true;
				
				if (stn.sProtocol == "01")
				{
					row.Cells[tCol_But_All].Value = Localization.getResStr("tNotAvailable");
					row.Cells[tCol_But_All].ToolTipText = Localization.getResStr("tNotProtocol01Hint");
					row.Cells[tCol_But_All].ReadOnly = true;
				}
				else
				{
					row.Cells[tCol_But_All].Value = Localization.getResStr("tResetAllCounter");
					row.Cells[tCol_But_All].ToolTipText = Localization.getResStr("tResetGlobalCountersHint");
					row.Cells[tCol_But_All].ReadOnly = false;
				}
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
		
		private uint resetCounters(long stationID, bool bPartial, bool bGlobal)
		{
			// devuelve el ID de transacción para saber cuando ha finalizado
			CStation_SOLD instanceStn = jbc.Station(stationID);
			uint transact = (uint) 0;
			instanceStn.SetControlMode(ControlModeConnection.CONTROL);
			if (bPartial && instanceStn.GetStationFeatures().PartialCounters)
			{
				for (var i = 0; i <= instanceStn.NumPorts - 1; i++)
				{
					instanceStn.ResetPortToolMinutesPartial((Port)i);
				}
			}
			if (bGlobal)
			{
				instanceStn.ResetPortToolMinutesGlobalPorts();
			}
			transact = instanceStn.SetTransaction();
            instanceStn.SetControlMode(ControlModeConnection.MONITOR);
			instanceStn = null;
			return transact;
		}
		
		private void initGridCounters()
		{
			gridCounters.Rows.Clear();
			
			gridCounters.Rows.Add(7);
			gridCounters.Rows[(int)rowCounter.Plugged].Cells["col_Counter"].Value = Localization.getResStr("tCounterPlugged");
            gridCounters.Rows[(int)rowCounter.Work].Cells["col_Counter"].Value = Localization.getResStr("tCounterWork");
            gridCounters.Rows[(int)rowCounter.Idle].Cells["col_Counter"].Value = Localization.getResStr("tCounterIdle");
            gridCounters.Rows[(int)rowCounter.Sleep].Cells["col_Counter"].Value = Localization.getResStr("tCounterSleep");
            gridCounters.Rows[(int)rowCounter.Hiber].Cells["col_Counter"].Value = Localization.getResStr("tCounterHiber");
            gridCounters.Rows[(int)rowCounter.SleepC].Cells["col_Counter"].Value = Localization.getResStr("tCounterSleepCycles");
            gridCounters.Rows[(int)rowCounter.DesoldC].Cells["col_Counter"].Value = Localization.getResStr("tCounterDesolCycles");
			foreach (DataGridViewColumn col in gridCounters.Columns)
			{
				string[] colName = col.Name.Split('_');
				if (colName[1] == "Global" || colName[1] == "Partial")
				{
					col.Visible = false;
				}
			}
		}
		
		private void showSelectedStationCounters()
		{
			int iRowIndex = gridStations.SelectedCells[0].RowIndex;
			long stationID = System.Convert.ToInt32(gridStations.Rows[iRowIndex].Cells[tCol_ID].Value);
			labStationCounters.Text = string.Format(Localization.getResStr("tCountersForStation"), gridStations.Rows[iRowIndex].Cells[tCol_StationName].Value.ToString());
			showCounters(stationID);
		}
		
		public void timerUpdateCounters_Tick(object sender, EventArgs e)
		{
			// la actualización del grid de los contadores se hace con un timer
			// porque la API no está grabando en memoria y las tramas de lectura posteriores
			// a la escritura tardan en ejecutarse. Se usa una transacción.
			CStation_SOLD instanceStn = jbc.Station(iStationIDTransact);
			if (instanceStn != null)
			{
				timerUpdateCounters.Stop();
				if (instanceStn.QueryEndedTransaction(uiTransact))
				{
					Cursor = Cursors.Default;
					showSelectedStationCounters();
				}
				else
				{
					timerUpdateCounters.Start();
				}
			}
			else
			{
				Cursor = Cursors.Default;
				showSelectedStationCounters();
			}
		}
		
		private void showCounters(long stationID)
		{
			int ports = jbc.GetPortCount(stationID);
			bool bHasPartial = jbc.GetStationFeatures(stationID).PartialCounters;
			bool bVisible = true;
			foreach (DataGridViewColumn col in gridCounters.Columns)
			{
				col.Visible = false;
			}
			foreach (DataGridViewColumn col in gridCounters.Columns)
			{
				bVisible = true;
				string[] colName = col.Name.Split('_');
				if (colName[1] == "Global" || colName[1] == "Partial")
				{
					if (int.Parse(colName[3]) > ports)
					{
						bVisible = false;
					}
					if (colName[1] == "Partial")
					{
						if (!bHasPartial)
						{
							bVisible = false;
						}
					}
				}
				col.Visible = bVisible;
			}
			
			for (var i = 0; i <= ports - 1; i++)
			{
                gridCounters.Rows[(int)rowCounter.Plugged].Cells[tCol_Global + (i + 1).ToString()].Value = jbc.GetPortToolPluggedMinutes(stationID, (Port)i).ToString();
                gridCounters.Rows[(int)rowCounter.Work].Cells[tCol_Global + (i + 1).ToString()].Value = jbc.GetPortToolWorkMinutes(stationID, (Port)i).ToString();
                gridCounters.Rows[(int)rowCounter.Idle].Cells[tCol_Global + (i + 1).ToString()].Value = jbc.GetPortToolIdleMinutes(stationID, (Port)i).ToString();
                gridCounters.Rows[(int)rowCounter.Sleep].Cells[tCol_Global + (i + 1).ToString()].Value = jbc.GetPortToolSleepMinutes(stationID, (Port)i).ToString();
                gridCounters.Rows[(int)rowCounter.Hiber].Cells[tCol_Global + (i + 1).ToString()].Value = jbc.GetPortToolHibernationMinutes(stationID, (Port)i).ToString();
                gridCounters.Rows[(int)rowCounter.SleepC].Cells[tCol_Global + (i + 1).ToString()].Value = jbc.GetPortToolSleepCycles(stationID, (Port)i).ToString();
                gridCounters.Rows[(int)rowCounter.DesoldC].Cells[tCol_Global + (i + 1).ToString()].Value = jbc.GetPortToolDesolderCycles(stationID, (Port)i).ToString();
				if (bHasPartial)
				{
                    gridCounters.Rows[(int)rowCounter.Plugged].Cells[tCol_Partial + (i + 1).ToString()].Value = jbc.GetPortToolPluggedMinutes(stationID, (Port)i, CounterTypes.PARTIAL_COUNTER).ToString();
                    gridCounters.Rows[(int)rowCounter.Work].Cells[tCol_Partial + (i + 1).ToString()].Value = jbc.GetPortToolWorkMinutes(stationID, (Port)i, CounterTypes.PARTIAL_COUNTER).ToString();
                    gridCounters.Rows[(int)rowCounter.Idle].Cells[tCol_Partial + (i + 1).ToString()].Value = jbc.GetPortToolIdleMinutes(stationID, (Port)i, CounterTypes.PARTIAL_COUNTER).ToString();
                    gridCounters.Rows[(int)rowCounter.Sleep].Cells[tCol_Partial + (i + 1).ToString()].Value = jbc.GetPortToolSleepMinutes(stationID, (Port)i, CounterTypes.PARTIAL_COUNTER).ToString();
                    gridCounters.Rows[(int)rowCounter.Hiber].Cells[tCol_Partial + (i + 1).ToString()].Value = jbc.GetPortToolHibernationMinutes(stationID, (Port)i, CounterTypes.PARTIAL_COUNTER).ToString();
                    gridCounters.Rows[(int)rowCounter.SleepC].Cells[tCol_Partial + (i + 1).ToString()].Value = jbc.GetPortToolSleepCycles(stationID, (Port)i, CounterTypes.PARTIAL_COUNTER).ToString();
                    gridCounters.Rows[(int)rowCounter.DesoldC].Cells[tCol_Partial + (i + 1).ToString()].Value = jbc.GetPortToolDesolderCycles(stationID, (Port)i, CounterTypes.PARTIAL_COUNTER).ToString();
				}
			}
			LogAdd("showCounters stationId API:" + stationID.ToString() + " gridCounters.Rows(rowCounter.Plugged parcial 1):" + jbc.GetPortToolPluggedMinutes(stationID, 0, CounterTypes.PARTIAL_COUNTER).ToString());
			LogAdd("showCounters stationId grid:" + stationID.ToString() + " gridCounters.Rows(rowCounter.Plugged parcial 1):" + System.Convert.ToString(gridCounters.Rows[(int)rowCounter.Plugged].Cells[tCol_Partial + "1"].Value));
			gridCounters.Refresh();
			gridCounters.Update();
		}
		
		public void gridStations_CellClick(System.Object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
		{
			
			if (e.RowIndex < 0)
			{
				return ;
			}
			
			long stationID = System.Convert.ToInt64(System.Convert.ToUInt32(gridStations.Rows[e.RowIndex].Cells[tCol_ID].Value));
			
			if (gridStations.Columns[e.ColumnIndex].Name != tCol_But_All && gridStations.Columns[e.ColumnIndex].Name != tCol_But_Partial)
			{
				return ;
			}
			
			CStation_SOLD instanceStn = jbc.Station(stationID);
			
			switch (gridStations.Columns[e.ColumnIndex].Name)
			{
				case tCol_But_All:
					if (instanceStn.CommandProtocol != CStationBase.Protocol.Protocol_01 | cbProtocol01.Checked)
					{
						if (Interaction.MsgBox(Localization.getResStr("tConfirmAction"), MsgBoxStyle.YesNo, Localization.getResStr("tResetAllCounters")) == MsgBoxResult.Yes)
						{
							uiTransact = resetCounters(stationID, true, true);
							iStationIDTransact = (int) stationID;
							// la actualización del grid de los contadores se hace con un timer
							// porque la API no está grabando en memoria y las tramas de lectura posteriores
							// a la escritura tardan en ejecutarse. Se usa una transacción.
							Cursor = Cursors.WaitCursor;
							timerUpdateCounters.Start();
						}
					}
					break;
				case tCol_But_Partial:
					if (instanceStn.GetStationFeatures().PartialCounters)
					{
						if (Interaction.MsgBox(Localization.getResStr("tConfirmAction"), MsgBoxStyle.YesNo, Localization.getResStr("tResetPartialCounters")) == MsgBoxResult.Yes)
						{
							uiTransact = resetCounters(stationID, true, false);
							iStationIDTransact = (int) stationID;
							// la actualización del grid de los contadores se hace con un timer
							// porque la API no está grabando en memoria y las tramas de lectura posteriores
							// a la escritura tardan en ejecutarse. Se usa una transacción.
							Cursor = Cursors.WaitCursor;
							timerUpdateCounters.Start();
						}
					}
					break;
				default:
					return ;
			}
			
		}
		
		public void gridStations_RowEnter(object sender, DataGridViewCellEventArgs e)
		{
			
		}
		
		public void gridStations_Sorted(object sender, EventArgs e)
		{
			showSelectedStationCounters();
		}
		
		public void gridStations_SelectionChanged(object sender, EventArgs e)
		{
			
		}
		
		public void gridStations_CurrentCellChanged(object sender, EventArgs e)
		{
			
		}
		
		public void gridStations_CellMouseClick(System.Object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
		{
			showSelectedStationCounters();
			//Dim stationID As Long = CInt(gridStations.Rows(e.RowIndex).Cells(tCol_ID).Value)
			//labStationCounters.Text = "Station Counters: " & gridStations.Rows(e.RowIndex).Cells(tCol_StationName).Value
			//showCounters(stationID)
		}
		
		private void jbc_NewStationConnected(long stationID)
		{
			//required as long as this method is called from a diferent thread
			if (this.InvokeRequired)
			{
				this.Invoke(new JBC_API.NewStationConnectedEventHandler(jbc_NewStationConnected), new object[] {stationID});
				return;
			}
			
			//Threading.Thread.Sleep(4000)
			
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
			if (stn != null)
			{
				LogAdd(string.Format("Disconnected - Name:{0} - Model:{1} - Type:{2} - Version:{3}", stn.sName, stn.sModel, stn.sModelType, stn.iModelVersion.ToString()));
			}
			
			rmvStation(stationID);
			
			removeGrid(stationID);
			
		}
		
		private void jbc_UserError(long stationID, Cerror err)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new JBC_API.UserErrorEventHandler(jbc_UserError), new object[] {stationID, err});
				return;
			}
			Cerror.cErrorCodes errorCode = err.GetCode();
			Cerror.cCommErrorCodes commErrorCode = err.GetCommErrorCode();
			string sErrorMsg = "";
			string sCommErrorMsg = "";
			string sErrorText = "";
			sErrorMsg = System.Convert.ToString(errorCode.ToString());
			sCommErrorMsg = System.Convert.ToString(commErrorCode.ToString());
			LogAdd(string.Format("Error: {0} Error Comm: {1}", sErrorMsg, sCommErrorMsg));
			LogAdd(string.Format("Error of station {0} code: {1} msg: {2}", stationID.ToString(), err.GetCode(), err.GetMsg()));
		}
		
		public void Label1_DoubleClick(System.Object sender, System.EventArgs e)
		{
			//SplitContainer1.Panel2Collapsed = Not SplitContainer1.Panel2Collapsed
			if (FormLog.Default.Visible)
			{
				FormLog.Default.Hide();
			}
			else
			{
				FormLog.Default.Show();
			}
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
			FormLog.Default.ListBox1.Items.Add(sText);
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
