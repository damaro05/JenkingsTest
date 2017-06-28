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
using System.ServiceModel;
using JBCStationControllerService;



/// <summary>
/// This example shows how to communicate with the Station Controller and change the basic parameters
/// </summary>
/// <remarks></remarks>
public partial class FormMainParams
{
	
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
	
	private JBCStationControllerServiceClient _JBC_StationControllerService = null;
	private long _stationID = -1;
	private dc_Station_Sold_Info _stationInfo;
	private dc_Station_Sold_Status _stationStatus;
	private dc_Station_Sold_Settings _stationSettings;
	private Timer _tmr;
	
	
	public FormMainParams()
	{
		
		// This call is required by the designer.
		InitializeComponent();
		
		//Added to support default instance behavour in C#
		if (defaultInstance == null)
			defaultInstance = this;
		
		//default combobox item selection
		cbxUnits.SelectedIndex = 0;
		cbxN2.SelectedIndex = 0;
		cbxHelp.SelectedIndex = 0;
		cbxBeep.SelectedIndex = 0;
		
		//refresh station parameters
		_tmr = new Timer();
		_tmr.Tick += tmr_Elapsed;
		_tmr.Interval = 1000; //1 second
		_tmr.Start();
	}
	
	public void butConnect_Click(object sender, EventArgs e)
	{
		//clear station list
		cbxStations.Items.Clear();
		cbxStations.SelectedIndex = -1;
		_stationID = -1;
		
		//disable pannels
		gbxStationData.Enabled = false;
		gbxStationParams.Enabled = false;
		gbxPort1.Enabled = false;
		gbxPort2.Enabled = false;
		gbxPort3.Enabled = false;
		gbxPort4.Enabled = false;
		
		//connect to service
		if (ReferenceEquals(_JBC_StationControllerService, null))
		{
			try
			{
				BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
				EndpointAddress endpoint = new EndpointAddress(txtStationControllerURL.Text);
				_JBC_StationControllerService = new JBCStationControllerServiceClient(binding, endpoint);
				
				//update station controller status
				lblConnectStatus.Text = "Connected";
				butConnect.Text = "Disconnect";
				
				//update station list
				foreach (long stationID in _JBC_StationControllerService.GetStationList())
				{
					cbxStations.Items.Add(stationID);
				}
				
			}
			catch (Exception)
			{
				_JBC_StationControllerService = null;
				
				//update station controller status
				lblConnectStatus.Text = "Error connecting";
				butConnect.Text = "Connect";
			}
			
			//disconnect from service
		}
		else
		{
			_JBC_StationControllerService.Close();
			_JBC_StationControllerService = null;
			
			//update station controller status
			lblConnectStatus.Text = "Disconnected";
			butConnect.Text = "Connect";
		}
	}
	
	public void cbxStations_SelectedIndexChanged(System.Object sender, System.EventArgs e)
	{
		//updating the selected station variable
		if (cbxStations.SelectedIndex >= 0)
		{
			_stationID = Convert.ToInt32(cbxStations.SelectedItem);
			
			dc_Station_Sold_Info stationInfo = _JBC_StationControllerService.GetStationInfo(_stationID);
			
			if (stationInfo.StationType != dc_EnumConstJBCdc_StationType.SOLD)
			{
				cbxStations.SelectedIndex = -1;
				_stationID = -1;
				MessageBox.Show(string.Format("Station ID {0} model '{1}' is not a soldering station.", _stationID, stationInfo.Model));
				return ;
			}
			
			//enable pannels
			gbxStationData.Enabled = true;
			gbxStationParams.Enabled = true;
			
			//it is necessary to take the control mode to modify the parameters of the station
			_JBC_StationControllerService.SetControlMode(_stationID, dc_EnumConstJBCdc_ControlModeConnection.CONTROL, Environment.MachineName);
		}
	}
	
	private void tmr_Elapsed(object sender, EventArgs e)
	{
		if (_stationID >= 0)
		{
			
			//is necessary to maintain control mode
			_JBC_StationControllerService.KeepControlMode(_stationID);
			
			//get station information
			_stationInfo = _JBC_StationControllerService.GetStationInfo(_stationID);
			_stationStatus = _JBC_StationControllerService.GetStationStatus(_stationID);
			_stationSettings = _JBC_StationControllerService.GetStationSettings(_stationID);
			
			//update station data
			lblModel.Text = _stationInfo.Model;
			lblSW.Text = _stationInfo.Version_Software;
			if (_stationSettings.Unit == "C")
			{
				lblTrafo.Text = _stationStatus.TRAFOTemp.Celsius.ToString() + " ºC";
				lblTrafoError.Text = _stationStatus.TempErrorTRAFO.Celsius.ToString() + " ºC";
				lblMOSerror.Text = _stationStatus.TempErrorMOS.Celsius.ToString() + " ºC";
			}
			else
			{
				lblTrafo.Text = _stationStatus.TRAFOTemp.Fahrenheit.ToString() + " ºF";
				lblTrafoError.Text = _stationStatus.TempErrorTRAFO.Fahrenheit.ToString() + " ºF";
				lblMOSerror.Text = _stationStatus.TempErrorMOS.Fahrenheit.ToString() + " ºF";
			}
			lblError.Text = _stationStatus.StationError.ToString();
			
			//update station params
			lblName.Text = _stationSettings.Name;
			if (_stationSettings.Unit == "C")
			{
				lblMaxTemp.Text = _stationSettings.MaxTemp.Celsius.ToString() + " ºC";
				lblMinTemp.Text = _stationSettings.MinTemp.Celsius.ToString() + " ºC";
			}
			else
			{
				lblMaxTemp.Text = _stationSettings.MaxTemp.Fahrenheit.ToString() + " ºF";
				lblMinTemp.Text = _stationSettings.MinTemp.Fahrenheit.ToString() + " ºF";
			}
			//if station supports display settings changes
			if (_stationInfo.Features.DisplaySettings)
			{
				lblUnits.Text = _stationSettings.Unit.ToString();
				lblN2.Text = _stationSettings.N2Mode.ToString().Replace("_", "");
				lblHelp.Text = _stationSettings.HelpText.ToString().Replace("_", "");
				lblBeep.Text = _stationSettings.Beep.ToString().Replace("_", "");
				
				butUnits.Enabled = true;
				butN2.Enabled = true;
				butHelp.Enabled = true;
				butBeep.Enabled = true;
			}
			else
			{
				lblUnits.Text = "N/A"; // not supported
				lblN2.Text = "N/A"; // not supported
				lblHelp.Text = "N/A"; // not supported
				lblBeep.Text = "N/A"; // not supported
				
				butUnits.Enabled = false;
				butN2.Enabled = false;
				butHelp.Enabled = false;
				butBeep.Enabled = false;
			}
			
			//update ports information
			for (var i = 1; i <= _stationInfo.PortCount; i++)
			{
				this.Controls.Find("gbxPort" + System.Convert.ToString(i), true)[0].Enabled = true;
				dc_StatusTool statusTool = _JBC_StationControllerService.GetPortStatus(_stationID, (dc_EnumConstJBCdc_Port) (i - 1));
				
				Control tool = this.Controls.Find("lblPort" + System.Convert.ToString(i) + "Tool", true)[0];
				Control actualTemp = this.Controls.Find("lblPort" + System.Convert.ToString(i) + "ActualTemp", true)[0];
				Control selecTemp = this.Controls.Find("lblPort" + System.Convert.ToString(i) + "SelecTemp", true)[0];
				Control err = this.Controls.Find("lblPort" + System.Convert.ToString(i) + "Error", true)[0];
				Control status = this.Controls.Find("lblPort" + System.Convert.ToString(i) + "Status", true)[0];
				tool.Text = (statusTool.ConnectedTool).ToString();
				if (_stationSettings.Unit == "C")
				{
					actualTemp.Text = statusTool.ActualTemp.Celsius.ToString() + " ºC";
					selecTemp.Text = statusTool.PortSelectedTemp.Celsius.ToString() + " ºC";
				}
				else
				{
					actualTemp.Text = statusTool.ActualTemp.Fahrenheit.ToString() + " ºF";
					selecTemp.Text = statusTool.PortSelectedTemp.Fahrenheit.ToString() + " ºF";
				}
				err.Text = statusTool.ToolError.ToString();
				if (statusTool.Extractor_OnOff == dc_EnumConstJBCdc_OnOff._ON)
				{
					status.Text = "Extractor";
				}
				else if (statusTool.Hiber_OnOff == dc_EnumConstJBCdc_OnOff._ON)
				{
					status.Text = "Hibernation";
				}
				else if (statusTool.Sleep_OnOff == dc_EnumConstJBCdc_OnOff._ON)
				{
					status.Text = "Sleep";
				}
				else if (statusTool.Stand_OnOff == dc_EnumConstJBCdc_OnOff._ON)
				{
					status.Text = "Stand";
				}
				else
				{
					status.Text = "Work";
				}
			}
			for (var i = _stationInfo.PortCount + 1; i <= 4; i++)
			{
				this.Controls.Find("gbxPort" + System.Convert.ToString(i), true)[0].Enabled = false;
			}
		}
		
		_tmr.Start();
	}
	
	public void butName_Click(object sender, EventArgs e)
	{
		_JBC_StationControllerService.SetStationName(_stationID, txtName.Text);
	}
	
	public void butMaxTemp_Click(object sender, EventArgs e)
	{
		_JBC_StationControllerService.SetStationMaxTemp(_stationID, int.Parse(txtMaxTemp.Text), _stationSettings.Unit);
	}
	
	public void butMinTemp_Click(object sender, EventArgs e)
	{
		_JBC_StationControllerService.SetStationMinTemp(_stationID, int.Parse(txtMinTemp.Text), _stationSettings.Unit);
	}
	
	public void butUnits_Click(object sender, EventArgs e)
	{
		if (cbxUnits.SelectedItem.ToString().Contains("CELSIUS"))
		{
			_JBC_StationControllerService.SetStationTempUnit(_stationID, "C");
		}
		if (cbxUnits.SelectedItem.ToString().Contains("FAHRENHEIT"))
		{
			_JBC_StationControllerService.SetStationTempUnit(_stationID, "F");
		}
	}
	
	public void butN2_Click(object sender, EventArgs e)
	{
		if (cbxN2.SelectedItem.ToString().Contains("ON"))
		{
			_JBC_StationControllerService.SetStationN2Mode(_stationID, dc_EnumConstJBCdc_OnOff._ON);
		}
		if (cbxN2.SelectedItem.ToString().Contains("OFF"))
		{
			_JBC_StationControllerService.SetStationN2Mode(_stationID, dc_EnumConstJBCdc_OnOff._OFF);
		}
	}
	
	public void butHelp_Click(object sender, EventArgs e)
	{
		if (cbxHelp.SelectedItem.ToString().Contains("ON"))
		{
			_JBC_StationControllerService.SetStationHelpText(_stationID, dc_EnumConstJBCdc_OnOff._ON);
		}
		if (cbxHelp.SelectedItem.ToString().Contains("OFF"))
		{
			_JBC_StationControllerService.SetStationHelpText(_stationID, dc_EnumConstJBCdc_OnOff._OFF);
		}
	}
	
	public void butBeep_Click(object sender, EventArgs e)
	{
		if (cbxBeep.SelectedItem.ToString().Contains("ON"))
		{
			_JBC_StationControllerService.SetStationBeep(_stationID, dc_EnumConstJBCdc_OnOff._ON);
		}
		if (cbxBeep.SelectedItem.ToString().Contains("OFF"))
		{
			_JBC_StationControllerService.SetStationBeep(_stationID, dc_EnumConstJBCdc_OnOff._OFF);
		}
	}
	
	public void butPort1SelecTemp_Click(object sender, EventArgs e)
	{
		setPortSelecTemp(1);
	}
	
	public void butPort2SelecTemp_Click(object sender, EventArgs e)
	{
		setPortSelecTemp(2);
	}
	
	public void butPort3SelecTemp_Click(object sender, EventArgs e)
	{
		setPortSelecTemp(3);
	}
	
	public void butPort4SelecTemp_Click(object sender, EventArgs e)
	{
		setPortSelecTemp(4);
	}
	
	private void setPortSelecTemp(int port)
	{
		int temperature = System.Convert.ToInt32(this.Controls.Find("txtPort" + System.Convert.ToString(port) + "SelecTemp", true)[0].Text);
		_JBC_StationControllerService.SetPortToolSelectedTemp(_stationID, (dc_EnumConstJBCdc_Port) (port - 1), temperature, _stationSettings.Unit);
	}
	
}
