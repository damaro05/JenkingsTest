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


namespace TestContinuousMode
{
	
	public partial class FormContinuousMode
	{
		public FormContinuousMode()
		{
			InitializeComponent();
			
			//Added to support default instance behavour in C#
			if (defaultInstance == null)
				defaultInstance = this;
		}
		
#region Default Instance
		
		private static FormContinuousMode defaultInstance;
		
		/// <summary>
		/// Added by the VB.Net to C# Converter to support default instance behavour in C#
		/// </summary>
		public static FormContinuousMode Default
		{
			get
			{
				if (defaultInstance == null)
				{
					defaultInstance = new FormContinuousMode();
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
		
		private JBC_API JBC;
		private long ID = long.MaxValue;
		private System.Timers.Timer tmr;
		private uint continuousmodeQueueId = UInt32.MaxValue;
		
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
			//creating the API, the timer and configuring VBasic properly
			JBC = new JBC_API();
			JBC.NewStationConnected += JBC_NewStationConnected;
			JBC.StationDisconnected += JBC_StationDisconnected;
			JBC.UserError += JBC_UserError;
			tmr = new System.Timers.Timer();
			tmr.Elapsed += tmr_Elapsed;
			CheckForIllegalCrossThreadCalls = false;
			
			CSpeedContinuousModeBindingSource.DataSource = typeof(SpeedContinuousMode);
			
			// start searching stations
			JBC.StartSearch();
			
			//configuring the timer
			tmr.Interval = 100;
			tmr.AutoReset = false;
			
			//setting the combobox speed values
			Array items = System.Enum.GetNames(typeof(SpeedContinuousMode));
			foreach (string field in items)
			{
				cbxSpeed.Items.Add(field);
			}
			cbxSpeed.SelectedIndex = 0;
			
			//setting the initial tmr interval
			cbxTick.SelectedIndex = 0;
			tmr.Interval = 1000;
			
			//initially disabling the start and stop buttons
			butStart.Enabled = false;
			butStop.Enabled = false;
		}
		
		private void myStart()
		{
			//starting the continuous mode by calling the API function
			Port p1 = Port.NO_PORT;
			Port p2 = Port.NO_PORT;
			Port p3 = Port.NO_PORT;
			Port p4 = Port.NO_PORT;
			if (chbPort1.Checked)
			{
				p1 = Port.NUM_1;
			}
			if (chbPort2.Checked)
			{
				p2 = Port.NUM_2;
			}
			if (chbPort3.Checked)
			{
				p3 = Port.NUM_3;
			}
			if (chbPort4.Checked)
			{
				p4 = Port.NUM_4;
			}
			//define speed and ports
			JBC.SetContinuousMode(ID, (SpeedContinuousMode) cbxSpeed.SelectedIndex, p1, p2, p3, p4);
			//start continuous mode. it returns a queue id from where to receive the data in
			continuousmodeQueueId = JBC.StartContinuousMode(ID);
			//once a continuous process is started it can be stopped but you cannot restart the same queue
			butStart.Enabled = false;
			butStop.Enabled = true;
			
			//disabling the speed combobox as long as we don't want this to be changed during the
			//monitorization for this application. Doing the same with the port selection checkboxes
			cbxSpeed.Enabled = false;
			chbPort1.Enabled = false;
			chbPort2.Enabled = false;
			chbPort3.Enabled = false;
			chbPort4.Enabled = false;
			
			//starting the application refresh timer
			tmr.Start();
		}
		
		private void myStop()
		{
			//stopping the continuous mode by calling the API function
			JBC.StopContinuousMode(ID, continuousmodeQueueId);
			
			//stopping the application refresh timer
			tmr.Stop();
			
			//once a continuous process is stopped it can be started
			butStart.Enabled = true;
			butStop.Enabled = false;
			
			//enabling the ports and speed controls as long as the monitorization is stopped and setting the speed to the OFF status
			cbxSpeed.SelectedIndex = 0;
			cbxSpeed.Enabled = true;
			chbPort1.Enabled = true;
			chbPort2.Enabled = true;
			chbPort3.Enabled = true;
			chbPort4.Enabled = true;
		}
		
		private void tick()
		{
			//Dim start As ULong = Environment.TickCount
			//Getting all the continuous mode data transmisions pending in the queue, just advancing the queue to get the last value and
			//counting how many transmisions have been done. We only desired one value every timer tick and it is also desired to check
			//the sequence number returned by the API with the real transmision counts to check data lose.
			int transmisionTickCounter = 0;
			JBC_Connect.stContinuousModeData ports = null;
			int countInTick = JBC.GetContinuousModeDataCount(ID, continuousmodeQueueId);
			for (var i = 0; i <= countInTick - 1; i++)
			{
				ports = JBC.GetContinuousModeNextData(ID, continuousmodeQueueId);
				transmisionTickCounter++;
			}
			
			//plotting LAST data if there's any
			if (transmisionTickCounter > 0)
			{
				//plotting the result of ports in the labels
				int portNum = 0;
				for (int cnt = 0; cnt <= ports.data.Length - 1; cnt++)
				{
					if (ports.data[cnt].port == Port.NUM_1)
					{
						portNum = 1;
					}
					if (ports.data[cnt].port == Port.NUM_2)
					{
						portNum = 2;
					}
					if (ports.data[cnt].port == Port.NUM_3)
					{
						portNum = 3;
					}
					if (ports.data[cnt].port == Port.NUM_4)
					{
						portNum = 4;
					}
					this.Controls.Find("lblTemp" + portNum.ToString(), true)[0].Text = ports.data[cnt].temperature.ToCelsius.ToString() + " ºC";
					this.Controls.Find("lblPwr" + portNum.ToString(), true)[0].Text = (ports.data[cnt].power / 10).ToString() + " %";
					this.Controls.Find("lblStatus" + portNum.ToString(), true)[0].Text = ports.data[0].status.ToString();
				}
				
				//plotting the counts
				lblSeq.Text = "Sequence: " + ports.sequence.ToString();
			}
			
			//udating the number of recieved transmisions
			lblTrans.Text = "trans. in tick: " + transmisionTickCounter.ToString();
			
			//restarting the timer
			//Console.WriteLine("AppLoop: " & Environment.TickCount - start)
			tmr.Start();
		}
		
		private void JBC_NewStationConnected(long stationID)
		{
			//if no station already then setting the detected
			if (ID == long.MaxValue)
			{
				if (JBC.GetStationType(stationID) != eStationType.SOLD)
				{
					lblStation.Text = stationID.ToString() + " not a soldering station.";
					return ;
				}
				
				//setting the detected station
				ID = stationID;
				lblStation.Text = "Station: " + ID.ToString();
				
				//setting the station in control mode
				JBC.SetControlMode(ID, ControlModeConnection.CONTROL);
				
				//enabling the start button if a speed is selected and disabling the stop button
				butStart.Enabled = cbxSpeed.SelectedIndex != 0;
				butStop.Enabled = false;
			}
		}
		
		private void JBC_StationDisconnected(long stationID)
		{
			//if the disconnected station is the one beeing monitorized setting the no station detected status
			if (stationID == ID)
			{
				//stopping the timer just in case is running
				tmr.Stop();
				
				//setting ID to a proper value
				ID = long.MaxValue;
				lblStation.Text = "Station: UNDETECTED";
				
				//disabling the start and stop buttons
				butStart.Enabled = false;
				butStop.Enabled = false;
			}
		}
		
		private void JBC_UserError(long stationID, JBC_Connect.Cerror err)
		{
			//showing the error
			MessageBox.Show(err.GetMsg());
		}
		
		public void butStart_Click(object sender, System.EventArgs e)
		{
			myStart();
		}
		
		public void butStop_Click(object sender, System.EventArgs e)
		{
			myStop();
		}
		
		public void cbxSpeed_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//if there's a station connected enabling the start button if a speed has been selected
			if (cbxSpeed.SelectedIndex != 0)
			{
				butStart.Enabled = ID != long.MaxValue;
			}
		}
		
		private void tmr_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			tick();
		}
		
		public void cbxTick_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			//changing the timer interval
			tmr.Interval = System.Convert.ToInt32(cbxTick.SelectedItem);
		}
		
	}
	
}
