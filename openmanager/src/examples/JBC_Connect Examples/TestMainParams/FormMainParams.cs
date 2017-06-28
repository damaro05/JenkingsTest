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

using JBC_Connect;


namespace TestMainParams
{
	
	public partial class FormMainParams
	{
		public FormMainParams()
		{
			InitializeComponent();
			
			//Added to support default instance behavour in C#
			if (defaultInstance == null)
				defaultInstance = this;
		}
		
#region Default Instance
		
		private static FormMainParams defaultInstance;
		
		/// <summary>
		/// Added by the VB.Net to C# Converter to support default instance behavour in C#
		/// </summary>
		public static FormMainParams Default
		{
			get
			{
				if (defaultInstance == null)
				{
					defaultInstance = new FormMainParams();
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
		
		// For soldering stations
		
		public JBC_API JBC; // The main JBC library class, the one required to manage the station.
		private System.Timers.Timer tmr;
		private List<long> stations = new List<long>(); // The station ID list, used to call the methods of the JBC object.
		private long curStation = long.MaxValue;
		private CFeaturesData curFeatures; // features supported by the station
		
		public void Form1_Load(object sender, System.EventArgs e)
		{
			init();
		}
		
		public void Form1_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			//close and unload API resourses
			JBC.Close();
		}
		
		private void init()
		{
			//creating the API object
			JBC = new JBC_API();
			JBC.NewStationConnected += JBC_NewStationConnected;
			JBC.StationDisconnected += JBC_StationDisconnected;
			JBC.UserError += JBC_UserError;
			CheckForIllegalCrossThreadCalls = false;
			
			// start searching stations
			JBC.StartSearch();
			
			//clearing the combobox
			cbxStations.Items.Clear();
			cbxStations.SelectedIndex = -1;
			
			//disabling the params
			gbxStationParams.Enabled = false;
			gbxStationData.Enabled = false;
			gbxPort1.Enabled = false;
			gbxPort2.Enabled = false;
			gbxPort3.Enabled = false;
			gbxPort4.Enabled = false;
			
			//initial values for the comboboxes
			cbxUnits.SelectedIndex = 0;
			cbxN2.SelectedIndex = 0;
			cbxHelp.SelectedIndex = 0;
			cbxBeep.SelectedIndex = 0;
			
			//configuring timer
			tmr = new System.Timers.Timer();
			tmr.Elapsed += tmr_Elapsed;
			tmr.Interval = 500;
			tmr.AutoReset = false;
		}
		
		private void JBC_NewStationConnected(long stationID)
		{
			//new station connected, adding to the list and updating the combo box
			stations.Add(stationID);
			cbxStations.Items.Add(stationID);
			
			//allowing the combobox selection as long as at least one station is present and selecting the detected station
			if (!cbxStations.Enabled)
			{
				cbxStations.Enabled = true;
			}
			if (!gbxStationParams.Enabled)
			{
				gbxStationParams.Enabled = true;
			}
			if (!gbxStationData.Enabled)
			{
				gbxStationData.Enabled = true;
			}
			
			if (JBC.GetStationType(stationID) != eStationType.SOLD)
			{
				curStation = long.MaxValue;
				string stationModel = JBC.GetStationModel(stationID);
				MessageBox.Show(string.Format("Detected station ID {0} model '{1}' but not a soldering station.", stationID, stationModel));
				return ;
			}
			else
			{
				cbxStations.SelectedItem = stationID;
				curStation = stationID;
				
				//updating the comboboxes
				updateParams();
				if (curFeatures.DisplaySettings)
				{
					if (lblUnits.Text != cbxUnits.SelectedItem.ToString())
					{
						cbxUnits.SelectedItem = lblUnits.Text;
					}
					if (lblN2.Text != cbxN2.SelectedItem.ToString())
					{
						cbxN2.SelectedItem = lblN2.Text;
					}
					if (lblHelp.Text != cbxHelp.SelectedItem.ToString())
					{
						cbxHelp.SelectedItem = lblHelp.Text;
					}
					if (lblBeep.Text != cbxBeep.SelectedItem.ToString())
					{
						cbxBeep.SelectedItem = lblBeep.Text;
					}
				}
				
				//setting the control mode to ON
				JBC.SetControlMode(curStation, ControlModeConnection.CONTROL);
				
				//starting the timer if it's not running
				tmr.Start();
			}
		}
		
		private void JBC_StationDisconnected(long stationID)
		{
			//station disconnected, removing from the list
			stations.Remove(stationID);
			
			//if the disconnected station is the selected one changing its selection
			if (curStation == stationID)
			{
				cbxStations.SelectedIndex = -1;
			}
			
			//removing from the combobox
			cbxStations.Items.Remove(stationID);
			
			//setting the proper combobx state
			if (cbxStations.Items.Count == 0)
			{
				gbxStationParams.Enabled = false;
				gbxStationData.Enabled = false;
				gbxPort1.Enabled = false;
				gbxPort2.Enabled = false;
				gbxPort3.Enabled = false;
				gbxPort4.Enabled = false;
				cbxStations.SelectedIndex = -1;
				tmr.Stop();
			}
		}
		
		private void JBC_UserError(long stationID, JBC_Connect.Cerror err)
		{
			//showing the error and finishing
			MessageBox.Show(err.GetMsg());
		}
		
		private void updateParams()
		{
			//if a station is selected updating all of its parametters
			if (curStation != long.MaxValue)
			{
				
				// get features supported by the station
				curFeatures = JBC.GetStationFeatures(curStation);
				
				//updating station parametters
				lblModel.Text = JBC.GetStationModel(curStation);
				lblSW.Text = JBC.GetStationSWversion(curStation);
				lblError.Text = JBC.GetStationError(curStation).ToString();
				lblName.Text = JBC.GetStationName(curStation);
				CTemperature.TemperatureUnit units = JBC.GetStationTempUnits(curStation);
				
				// if station supports display settings changes
				butUnits.Enabled = curFeatures.DisplaySettings;
				butN2.Enabled = curFeatures.DisplaySettings;
				butHelp.Enabled = curFeatures.DisplaySettings;
				butBeep.Enabled = curFeatures.DisplaySettings;
				if (curFeatures.DisplaySettings)
				{
					lblUnits.Text = units.ToString();
					lblN2.Text = System.Convert.ToString(JBC.GetStationN2Mode(curStation).ToString().Replace("_", ""));
					lblHelp.Text = System.Convert.ToString(JBC.GetStationHelpText(curStation).ToString().Replace("_", ""));
					lblBeep.Text = System.Convert.ToString(JBC.GetStationBeep(curStation).ToString().Replace("_", ""));
				}
				else
				{
					lblUnits.Text = "N/A"; // not supported
					lblN2.Text = "N/A"; // not supported
					lblHelp.Text = "N/A"; // not supported
					lblBeep.Text = "N/A"; // not supported
				}
				if (units == CTemperature.TemperatureUnit.Celsius)
				{
					lblTrafoError.Text = JBC.GetStationTransformerErrorTemp(curStation).ToCelsius().ToString() + " ºC";
					lblMOSerror.Text = JBC.GetStationMOSerrorTemp(curStation).ToCelsius().ToString() + " ºC";
					lblTrafo.Text = JBC.GetStationTransformerTemp(curStation).ToCelsius().ToString() + " ºC";
					lblMaxTemp.Text = JBC.GetStationMaxTemp(curStation).ToRoundCelsius().ToString() + " ºC";
					lblMinTemp.Text = JBC.GetStationMinTemp(curStation).ToRoundCelsius().ToString() + " ºC";
				}
				else if (units == CTemperature.TemperatureUnit.Fahrenheit)
				{
					lblTrafoError.Text = JBC.GetStationTransformerErrorTemp(curStation).ToFahrenheit().ToString() + " ºF";
					lblMOSerror.Text = JBC.GetStationMOSerrorTemp(curStation).ToFahrenheit().ToString() + " ºF";
					lblTrafo.Text = JBC.GetStationTransformerTemp(curStation).ToFahrenheit().ToString() + " ºF";
					lblMaxTemp.Text = JBC.GetStationMaxTemp(curStation).ToRoundFahrenheit().ToString() + " ºF";
					lblMinTemp.Text = JBC.GetStationMinTemp(curStation).ToRoundFahrenheit().ToString() + " ºF";
				}
				
				//clearing all the ports labels and disabling the ports group boxes
				for (int cnt = JBC.GetPortCount(curStation) + 1; cnt <= 4; cnt++)
				{
					Control gbx = this.Controls.Find("gbxPort" + System.Convert.ToString(cnt), true)[0];
					foreach (Control ctrl in gbx.Controls)
					{
						if (ctrl.Name.Contains("lbl"))
						{
							ctrl.Text = "";
						}
					}
					gbx.Enabled = false;
				}
				
				//updating the ports parametters
				for (int cnt = 1; cnt <= JBC.GetPortCount(curStation); cnt++)
				{
					Control port = this.Controls.Find("gbxPort" + System.Convert.ToString(cnt), true)[0];
					Control tool = this.Controls.Find("lblPort" + System.Convert.ToString(cnt) + "Tool", true)[0];
					Control err = this.Controls.Find("lblPort" + System.Convert.ToString(cnt) + "Error", true)[0];
					Control actual = this.Controls.Find("lblPort" + System.Convert.ToString(cnt) + "ActualTemp", true)[0];
					Control selec = this.Controls.Find("lblPort" + System.Convert.ToString(cnt) + "SelecTemp", true)[0];
					CheckBox sleep = (CheckBox) (this.Controls.Find("cbxPort" + System.Convert.ToString(cnt) + "Sleep", true)[0]);
					CheckBox hiber = (CheckBox) (this.Controls.Find("cbxPort" + System.Convert.ToString(cnt) + "Hibernation", true)[0]);
					CheckBox extractor = (CheckBox) (this.Controls.Find("cbxPort" + System.Convert.ToString(cnt) + "Extractor", true)[0]);
					tool.Text = JBC.GetPortToolID(curStation, (Port) (cnt - 1)).ToString();
					if (tool.Text != "")
					{
						port.Enabled = true;
						ToolError aux = JBC.GetPortToolError(curStation, (Port) (cnt - 1));
						if (aux == ToolError.NO_TOOL)
						{
							err.Text = ToolError.NO_ERROR.ToString();
						}
						else
						{
							err.Text = JBC.GetPortToolError(curStation, (Port) (cnt - 1)).ToString();
						}
						if (units == CTemperature.TemperatureUnit.Celsius)
						{
							actual.Text = JBC.GetPortToolActualTemp(curStation, (Port) (cnt - 1)).ToRoundCelsius().ToString() + " ºC";
							selec.Text = JBC.GetPortToolSelectedTemp(curStation, (Port) (cnt - 1)).ToRoundCelsius().ToString() + " ºC";
						}
						else if (units == CTemperature.TemperatureUnit.Fahrenheit)
						{
							actual.Text = JBC.GetPortToolActualTemp(curStation, (Port) (cnt - 1)).ToRoundFahrenheit().ToString() + " ºF";
							selec.Text = JBC.GetPortToolSelectedTemp(curStation, (Port) (cnt - 1)).ToRoundFahrenheit().ToString() + " ºF";
						}
						sleep.Checked = JBC.GetPortToolSleepStatus(curStation, (Port) (cnt - 1)) == OnOff._ON;
						hiber.Checked = JBC.GetPortToolHibernationStatus(curStation, (Port) (cnt - 1)) == OnOff._ON;
						extractor.Checked = JBC.GetPortToolExtractorStatus(curStation, (Port) (cnt - 1)) == OnOff._ON;
					}
				}
			}
		}
		
		public void cbxStations_SelectedIndexChanged(System.Object sender, System.EventArgs e)
		{
			//updating the selected station variable
			if (cbxStations.SelectedIndex >= 0)
			{
				long selectedID = Convert.ToInt32(cbxStations.SelectedItem);
				if (JBC.GetStationType(selectedID) != eStationType.SOLD)
				{
					curStation = long.MaxValue;
					string stationModel = JBC.GetStationModel(selectedID);
					MessageBox.Show(string.Format("Station ID {0} model '{1}' is not a soldering station.", selectedID, stationModel));
					return ;
				}
				curStation = Convert.ToInt32(cbxStations.SelectedItem);
				
				//updating the comboboxes
				updateParams();
				if (lblUnits.Text != cbxUnits.SelectedItem.ToString())
				{
					cbxUnits.SelectedItem = lblUnits.Text;
				}
				if (curFeatures.DisplaySettings)
				{
					if (lblN2.Text != cbxN2.SelectedItem.ToString())
					{
						cbxN2.SelectedItem = lblN2.Text;
					}
					if (lblHelp.Text != cbxHelp.SelectedItem.ToString())
					{
						cbxHelp.SelectedItem = lblHelp.Text;
					}
					if (lblBeep.Text != cbxBeep.SelectedItem.ToString())
					{
						cbxBeep.SelectedItem = lblBeep.Text;
					}
				}
			}
			else
			{
				curStation = long.MaxValue;
			}
		}
		
		private void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			//updating the parametters
			updateParams();
			
			//reseting the timer
			tmr.Start();
		}
		
		private void setName()
		{
			if (curStation != long.MaxValue)
			{
				JBC.SetStationName(curStation, txtName.Text);
				txtName.Text = "";
			}
		}
		
		public void butName_Click(System.Object sender, System.EventArgs e)
		{
			setName();
		}
		
		public void txtName_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setName();
			}
		}
		
		private void setMaxTemp()
		{
			if (curStation != long.MaxValue)
			{
				CTemperature T = new CTemperature(0);
				if (JBC.GetStationTempUnits(curStation) == CTemperature.TemperatureUnit.Celsius)
				{
					T.InCelsius(Convert.ToInt32(txtMaxTemp.Text));
				}
				if (JBC.GetStationTempUnits(curStation) == CTemperature.TemperatureUnit.Fahrenheit)
				{
					T.InFahrenheit(Convert.ToInt32(txtMaxTemp.Text));
				}
				JBC.SetStationMaxTemp(curStation, T);
				txtMaxTemp.Text = "";
			}
		}
		
		public void butMaxTemp_Click(System.Object sender, System.EventArgs e)
		{
			setMaxTemp();
		}
		
		public void txtMaxTemp_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setMaxTemp();
			}
		}
		
		private void setMinTemp()
		{
			if (curStation != long.MaxValue)
			{
				CTemperature T = new CTemperature(0);
				if (JBC.GetStationTempUnits(curStation) == CTemperature.TemperatureUnit.Celsius)
				{
					T.InCelsius(Convert.ToInt32(txtMinTemp.Text));
				}
				if (JBC.GetStationTempUnits(curStation) == CTemperature.TemperatureUnit.Fahrenheit)
				{
					T.InFahrenheit(Convert.ToInt32(txtMinTemp.Text));
				}
				JBC.SetStationMinTemp(curStation, T);
				txtMinTemp.Text = "";
			}
		}
		
		public void butMinTemp_Click(System.Object sender, System.EventArgs e)
		{
			setMinTemp();
		}
		
		public void txtMinTemp_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setMinTemp();
			}
		}
		
		public void butUnits_Click(System.Object sender, System.EventArgs e)
		{
			if (curStation != long.MaxValue)
			{
				if (cbxUnits.SelectedItem.ToString().Contains("CELSIUS"))
				{
					JBC.SetStationTempUnits(curStation, CTemperature.TemperatureUnit.Celsius);
				}
				if (cbxUnits.SelectedItem.ToString().Contains("FAHRENHEIT"))
				{
					JBC.SetStationTempUnits(curStation, CTemperature.TemperatureUnit.Fahrenheit);
				}
			}
		}
		
		public void butN2_Click(System.Object sender, System.EventArgs e)
		{
			if (curStation != long.MaxValue)
			{
				if (cbxN2.SelectedItem.ToString().Contains("ON"))
				{
					JBC.SetStationN2Mode(curStation, OnOff._ON);
				}
				if (cbxN2.SelectedItem.ToString().Contains("OFF"))
				{
					JBC.SetStationN2Mode(curStation, OnOff._OFF);
				}
			}
		}
		
		public void butHelp_Click(System.Object sender, System.EventArgs e)
		{
			if (curStation != long.MaxValue)
			{
				if (cbxHelp.SelectedItem.ToString().Contains("ON"))
				{
					JBC.SetStationHelpText(curStation, OnOff._ON);
				}
				if (cbxHelp.SelectedItem.ToString().Contains("OFF"))
				{
					JBC.SetStationHelpText(curStation, OnOff._OFF);
				}
			}
		}
		
		private void setPwrLimit()
		{
			if (curStation != long.MaxValue)
			{
				//JBC.SetStationPowerLimit(curStation, Convert.ToInt32(txtPwrLimit.Text))
				//txtPwrLimit.Text = ""
			}
		}
		
		private void butPwrLimit_Click(System.Object sender, System.EventArgs e)
		{
			setPwrLimit();
		}
		
		private void txtPwrLimit_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setPwrLimit();
			}
		}
		
		public void butBeep_Click(System.Object sender, System.EventArgs e)
		{
			if (curStation != long.MaxValue)
			{
				if (cbxBeep.SelectedItem.ToString().Contains("ON"))
				{
					JBC.SetStationBeep(curStation, OnOff._ON);
				}
				if (cbxBeep.SelectedItem.ToString().Contains("OFF"))
				{
					JBC.SetStationBeep(curStation, OnOff._OFF);
				}
			}
		}
		
		private void setPortSelecTemp(int port)
		{
			if (curStation != long.MaxValue)
			{
				MaskedTextBox txt = (MaskedTextBox) (this.Controls.Find("txtPort" + System.Convert.ToString(port) + "SelecTemp", true)[0]);
				CTemperature T = new CTemperature(0);
				if (JBC.GetStationTempUnits(curStation) == CTemperature.TemperatureUnit.Celsius)
				{
					T.InCelsius(Convert.ToInt32(txt.Text));
				}
				if (JBC.GetStationTempUnits(curStation) == CTemperature.TemperatureUnit.Fahrenheit)
				{
					T.InFahrenheit(Convert.ToInt32(txt.Text));
				}
				JBC.SetPortToolSelectedTemp(curStation, (Port) (port - 1), T);
				txt.Text = "";
			}
		}
		
		public void butPort1SelecTemp_Click(System.Object sender, System.EventArgs e)
		{
			setPortSelecTemp(1);
		}
		
		public void txtPort1SelecTemp_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setPortSelecTemp(1);
			}
		}
		
		public void butPort2SelecTemp_Click(System.Object sender, System.EventArgs e)
		{
			setPortSelecTemp(2);
		}
		
		public void txtPort2SelecTemp_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setPortSelecTemp(2);
			}
		}
		
		public void butPort3SelecTemp_Click(System.Object sender, System.EventArgs e)
		{
			setPortSelecTemp(3);
		}
		
		public void txtPort3SelecTemp_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setPortSelecTemp(3);
			}
		}
		
		public void butPort4SelecTemp_Click(System.Object sender, System.EventArgs e)
		{
			setPortSelecTemp(4);
		}
		
		public void txtPort4SelecTemp_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar == ControlChars.Cr)
			{
				setPortSelecTemp(4);
			}
		}
		
	}
	
}
