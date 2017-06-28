// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using JBC_Connect;
using DataJBC;
// End of VB project level imports

using Microsoft.VisualBasic.CompilerServices;

namespace UnitsConfiguration
{
	
	
	/// <summary>
	/// This program is used to change the units from stations
	/// </summary>
	/// <remarks>The two systems of units used are: ºC/mm and ºF/in</remarks>
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
		
		private enum Sound
		{
			_OK,
			_ERROR,
			_NOSUPPORTED
		}
		
		
		private const string INTERNATIONAL_SYSTEM_UNITS_TEXT = "ºC / mm";
		private const string ALTERNATIVE_SYSTEM_UNITS_TEXT = "ºF / in";
		
		private const string OPERATION_OK = "Configurado";
		private const string OPERATION_ERROR = "Error";
		
		private const string NO_CONNECTION = "No conexión";
		private const string NO_SUPPORTED = "No soportado";
		
		private Color COLOR_OK = Color.FromArgb(64, 64, 64);
		private Color COLOR_ERROR = Color.Red;
		
		
		private JBC_API m_jbc = new JBC_API();
		private long m_stationID = -1;
		private string m_stationModel = "";
		private CTemperature.TemperatureUnit m_stationUnits;
		
		private int m_connectedStations = 0;
		private int m_configuredStations = 0;
		private CTemperature.TemperatureUnit m_selectedUnits = CTemperature.TemperatureUnit.Celsius;
		
		
		public void Form1_Load(System.Object sender, System.EventArgs e)
		{
			//Start search
			m_jbc.StartSearch();
		}
		
		public void Form1_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			m_jbc.Close();
			ProjectData.EndApp();
		}
		
#region Connected / Disconnected
		
		public void Connected(long stationID)
		{
			m_stationID = stationID;
			m_stationModel = m_jbc.GetStationModel(m_stationID);
			m_connectedStations++;
			
			//Get control
			m_jbc.SetControlMode(m_stationID, ControlModeConnection.CONTROL);
			
			bool supportedStation = m_jbc.GetStationFeatures(m_stationID).DisplaySettings;
			
			if (supportedStation)
			{
				m_jbc.SetStationTempUnits(m_stationID, m_selectedUnits);
				int maxRetries = 15;
				uint transactionID = m_jbc.SetTransaction(m_stationID);
				
				while (maxRetries > 0)
				{
					if (m_jbc.QueryTransaction(m_stationID, transactionID))
					{
						break;
					}
					maxRetries--;
					
					System.Threading.Thread.Sleep(100);
				}
				m_stationUnits = m_jbc.GetStationTempUnits(m_stationID);
				
				if (m_selectedUnits == m_stationUnits)
				{
					m_configuredStations++;
				}
			}
			
			//Refresh UI
			RefreshUI(supportedStation);
		}
		
		public void Disconnected(long stationid)
		{
			m_stationID = -1;
			m_stationModel = NO_CONNECTION;
			
			//Refresh UI
			RefreshUI();
		}
		
#endregion
		
#region UI
		
		public void UnitsChange_CheckedChanged(object sender, EventArgs e)
		{
			if (this.rbInternationalUnits.Checked)
			{
				m_selectedUnits = CTemperature.TemperatureUnit.Celsius;
				this.labelUnitsChange.Text = INTERNATIONAL_SYSTEM_UNITS_TEXT;
			}
			else
			{
				m_selectedUnits = CTemperature.TemperatureUnit.Fahrenheit;
				this.labelUnitsChange.Text = ALTERNATIVE_SYSTEM_UNITS_TEXT;
			}
		}
		
		private void RefreshUI(bool supported = false)
		{
			//required as long as this method is called from a diferent thread
			if (this.InvokeRequired)
			{
				this.Invoke((Action) (() => RefreshUI(supported)));
				return;
			}
			
			//Station model
			this.labelStationModel.Text = m_stationModel;
			
			//Station disconnected
			if (m_stationID < 0)
			{
				this.imgStatus.Visible = false;
				this.labelStatus.Visible = false;
				this.labelStationModel.ForeColor = COLOR_ERROR;
				
				//Station not supported
			}
			else if (!supported)
			{
				this.imgStatus.Visible = false;
				this.labelStatus.Visible = true;
				this.labelStatus.Text = NO_SUPPORTED;
				this.labelStatus.ForeColor = COLOR_OK;
				this.labelStationModel.ForeColor = COLOR_OK;
				this.textBoxLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + m_stationModel + " - " + NO_SUPPORTED + "\r\n");
				PlaySound(Sound._NOSUPPORTED);
				
				//Operation OK
			}
			else if (m_stationUnits == m_selectedUnits)
			{
				this.imgStatus.Visible = true;
                this.imgStatus.Image = My.Resources.Resources.ok;
				this.labelStatus.Visible = true;
				this.labelStatus.Text = OPERATION_OK;
				this.labelStatus.ForeColor = COLOR_OK;
				this.labelStationModel.ForeColor = COLOR_OK;
				this.textBoxLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + m_stationModel + " - " + OPERATION_OK + "\r\n");
				PlaySound(Sound._OK);
				
				//Operation KO
			}
			else
			{
				this.imgStatus.Visible = true;
                this.imgStatus.Image = My.Resources.Resources.error;
				this.labelStatus.Visible = true;
				this.labelStatus.Text = OPERATION_ERROR;
				this.labelStatus.ForeColor = COLOR_ERROR;
				this.labelStationModel.ForeColor = COLOR_OK;
				this.textBoxLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " - " + m_stationModel + " - " + OPERATION_ERROR + "\r\n");
				PlaySound(Sound._ERROR);
			}
			
			this.labelTotalStations.Text = (m_connectedStations).ToString();
			this.labelTotalConfigurations.Text = (m_configuredStations).ToString();
		}
		
		private void PlaySound(Sound type)
		{
			// Console.Beep(frequency, duration)
			// The frequency ranges between 37 and 32767, although the higher tones are not audible by the human ear.
			// The duration is 1000 for one second.
			switch (type)
			{
				case Sound._OK:
					Console.Beep(800, 500);
					break;
				case Sound._ERROR:
					Console.Beep(800, 2500);
					break;
				case Sound._NOSUPPORTED:
					Console.Beep(800, 200);
					break;
			}
		}
		
		public void butResetCounters_Click(object sender, EventArgs e)
		{
			m_connectedStations = 0;
			m_configuredStations = 0;
			
			this.labelTotalStations.Text = (m_connectedStations).ToString();
			this.labelTotalConfigurations.Text = (m_configuredStations).ToString();
		}
		
#endregion
		
	}
	
}
