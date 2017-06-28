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
using JBC_ConnectRemote;
using DataJBC;


namespace RemoteManager
{
    public partial class frmPorts
    {
        private Port port = Port.NO_PORT;
        private JBC_API_Remote jbc;
        private long myID;
        private int nPorts;

        // Controls
        private PictureBox pcbTool1;
        private PictureBox pcbTool2;
        private PictureBox pcbTool3;
        private PictureBox pcbTool4;

        private Label lblData1;
        private Label lblData2;
        private Label lblData3;
        private Label lblData4;

        private Timer tmr;

        public Port selectedPort
        {
            set
            {
                port = value;
            }
            get
            {
                return port;
            }
        }

        public frmPorts(long ID, JBC_API_Remote jbcRef)
        {

            // Required by the designer
            InitializeComponent();

            // Setting vars
            myID = ID;
            jbc = jbcRef;

            // Getting the number of ports
            nPorts = jbc.GetPortCount(myID);

            // Setting the configuration
            this.ClientSize = new Size((Configuration.PortsToolDataWidth + Configuration.PortsToolImgWidth) + Configuration.PortsHorzMargin * 2,
                nPorts * Math.Max(Configuration.PortsToolImgHeight, Configuration.PortsToolDataHeight) + Configuration.PortsVertMargin * (nPorts + 1));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowIcon = false;
            //Me.BackgroundImage = PortsBackGndImg
            //Me.BackgroundImageLayout = ImageLayout.Center
            this.BackColor = Configuration.stnColorBackground;
            this.Location = new Point(System.Convert.ToInt32(PointToScreen(MousePosition).X - this.Width / 2), System.Convert.ToInt32(PointToScreen(MousePosition).Y - this.Height / 2));
            this.Text = jbc.GetStationModel(myID) + " - " + jbc.GetStationName(myID) + " - " + Localization.getResStr(Configuration.PortsPortTitleId);

            // Creating the controls only if the port is present in the station
            if (nPorts >= 1)
            {
                createPortInstance(0, ref pcbTool1, ref lblData1);
            }
            if (nPorts >= 2)
            {
                createPortInstance(1, ref pcbTool2, ref lblData2);
            }
            if (nPorts >= 3)
            {
                createPortInstance(2, ref pcbTool3, ref lblData3);
            }
            if (nPorts >= 4)
            {
                createPortInstance(3, ref pcbTool4, ref lblData4);
            }

            // Launching the update timer
            tmr = new Timer();
            tmr.Tick += tmr_Tick;
            tmr.Interval = 500;
            tmr.Start();
        }

        private void createPortInstance(int port, ref PictureBox pcb, ref Label lbl)
        {
            // Creating the picture box in the indicated port
            pcb = new PictureBox();
            pcb.Name = "pcb" + port.ToString();
            pcb.Size = new Size(Configuration.PortsToolImgWidth, Configuration.PortsToolImgHeight);
            pcb.Location = new Point(Configuration.PortsHorzMargin, Configuration.PortsVertMargin + port * (Math.Max(Configuration.PortsToolImgHeight, Configuration.PortsToolDataHeight) + Configuration.PortsVertMargin));
            pcb.SizeMode = PictureBoxSizeMode.CenterImage;
            pcb.BackgroundImageLayout = ImageLayout.Center;
            pcb.BackColor = Color.Transparent;

            // Creating the data label in the indicated port
            lbl = new Label();
            lbl.Name = "lbl" + port.ToString();
            lbl.BackColor = Color.Transparent;
            lbl.ForeColor = Configuration.PortsTextColor;
            lbl.Font = Configuration.PortsFont;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.AutoSize = false;
            lbl.Size = new Size(Configuration.PortsToolDataWidth, Configuration.PortsToolDataHeight);
            lbl.Location = new Point(Configuration.PortsHorzMargin + Configuration.PortsToolImgWidth, Configuration.PortsVertMargin + port * (Math.Max(Configuration.PortsToolImgHeight, Configuration.PortsToolDataHeight) + Configuration.PortsVertMargin));

            // Adding the picture box and label to the form
            this.Controls.Add(pcb);
            this.Controls.Add(lbl);
        }

        private void updatePortInstance(int port, PictureBox pcb, Label lbl)
        {

            // Required var's
            bool sleep = false;
            bool hiber = false;
            bool extractor = false;
            bool desolder = false;
            GenericStationTools tool = default(GenericStationTools);
            string status = "";

            // If the port exists
            if (port < nPorts)
            {
                // Setting the tool image
                tool = jbc.GetPortToolID(myID, (Port)port);
                pcb.BackgroundImage = (Image)My.Resources.Resources.ResourceManager.GetObject(tool.ToString() + "_mini");

                // Determinating the tool status
                sleep = jbc.GetPortToolSleepStatus(myID, (Port)port) == OnOff._ON;
                hiber = jbc.GetPortToolHibernationStatus(myID, (Port)port) == OnOff._ON;
                extractor = jbc.GetPortToolExtractorStatus(myID, (Port)port) == OnOff._ON;
                desolder = jbc.GetPortToolDesolderStatus(myID, (Port)port) == OnOff._ON;
                if (sleep)
                {
                    status = Localization.getResStr(Configuration.PortsSleepId);
                }
                else if (hiber)
                {
                    status = Localization.getResStr(Configuration.PortsHiberId);
                }
                else if (extractor)
                {
                    status = Localization.getResStr(Configuration.PortsExtractorId);
                }
                else if (desolder)
                {
                    status = Localization.getResStr(Configuration.PortsDesolderId);
                }
                else if (tool == GenericStationTools.NO_TOOL)
                {
                    status = Localization.getResStr(Configuration.PortsNoToolId);
                }
                else
                {
                    status = Localization.getResStr(Configuration.PortsWorkId);
                }

                // Getting the temperature string
                string tempStr = "";
                CTemperature temp = jbc.GetPortToolActualTemp(myID, (Port)port);
                if (!Equals(temp, null))
                {
                    if (temp.isValid())
                    {
                        if (Configuration.Tunits == Configuration.CELSIUS_STR)
                        {
                            tempStr = temp.ToCelsius().ToString() + Configuration.Tunits;
                        }
                        if (Configuration.Tunits == Configuration.FAHRENHEIT_STR)
                        {
                            tempStr = temp.ToFahrenheit().ToString() + Configuration.Tunits;
                        }
                    }
                }

                // Showing the port data depending on the status
                int textW = System.Convert.ToInt32(lbl.CreateGraphics().MeasureString(Localization.getResStr(Configuration.PortsPortTitleId) + " " + (port + 1).ToString(), Configuration.PortsFont).Width);
                int spaceW = System.Convert.ToInt32(lbl.CreateGraphics().MeasureString(" ", Configuration.PortsFont).Width);
                int nSpaces = System.Convert.ToInt32((lbl.Width - textW) / spaceW);
                string sTool = tool.ToString();
                if (tool == GenericStationTools.NO_TOOL)
                {
                    sTool = " ";
                }
                lbl.Text = new string(' ', nSpaces);
                lbl.Text = lbl.Text + Localization.getResStr(Configuration.PortsPortTitleId) + " " + (port + 1).ToString() + "\r\n";
                lbl.Text = lbl.Text + sTool + "\r\n";
                if (status == Localization.getResStr(Configuration.PortsWorkId))
                {
                    lbl.Text = lbl.Text + tempStr;
                }
                else
                {
                    lbl.Text = lbl.Text + status;
                }
            }
        }

        private void highLightPortInstance(int port, PictureBox pcb, Label lbl, bool highLight)
        {

            // Depending if it's desired to highlight or not
            if (highLight)
            {
                lbl.ForeColor = Configuration.PortsTextHighLightColor;
                lbl.Font = Configuration.PortsHighLightFont;
                pcb.Image = My.Resources.Resources.ToolSelected_mini;
            }
            else
            {
                lbl.ForeColor = Configuration.PortsTextColor;
                lbl.Font = Configuration.PortsFont;
                pcb.Image = null;
            }
        }

        private void tmr_Tick(object sender, System.EventArgs e)
        {

            // Stopping the timer
            tmr.Stop();

            // Updating the ports
            if (nPorts >= 1)
            {
                updatePortInstance(0, pcbTool1, lblData1);
            }
            if (nPorts >= 2)
            {
                updatePortInstance(1, pcbTool2, lblData2);
            }
            if (nPorts >= 3)
            {
                updatePortInstance(2, pcbTool3, lblData3);
            }
            if (nPorts >= 4)
            {
                updatePortInstance(3, pcbTool4, lblData4);
            }

            // Reseting the timer
            tmr.Start();
        }

        // SELECTION ---------------------
        public void frmPorts_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            // End dialog
            this.DialogResult = DialogResult.Cancel;
        }

        public void MouseUpPort1(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // Check it's a left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left && nPorts >= 1)
            {
                selectedPort = Port.NUM_1;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        public void MouseUpPort2(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // Check it's a left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left && nPorts >= 1)
            {
                selectedPort = Port.NUM_2;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        public void MouseUpPort3(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // Check it's a left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left && nPorts >= 1)
            {
                selectedPort = Port.NUM_3;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }

        public void MouseUpPort4(object sender, System.Windows.Forms.MouseEventArgs e)
        {

            // Check it's a left click
            if (e.Button == System.Windows.Forms.MouseButtons.Left && nPorts >= 1)
            {
                selectedPort = Port.NUM_4;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
        }
        // HIGHLIGHT --------------------
        public void MouseEnterPort1(object sender, System.EventArgs e)
        {

            // HighLight Port 1
            if (nPorts >= 1)
            {
                highLightPortInstance(0, pcbTool1, lblData1, true);
            }
        }

        public void MouseEnterPort2(object sender, System.EventArgs e)
        {

            // HighLight Port 2
            if (nPorts >= 2)
            {
                highLightPortInstance(1, pcbTool2, lblData2, true);
            }
        }

        public void MouseEnterPort3(object sender, System.EventArgs e)
        {

            // HighLight Port 3
            if (nPorts >= 3)
            {
                highLightPortInstance(2, pcbTool3, lblData3, true);
            }
        }

        public void MouseEnterPort4(object sender, System.EventArgs e)
        {

            // HighLight Port 4
            if (nPorts >= 4)
            {
                highLightPortInstance(3, pcbTool4, lblData4, true);
            }
        }

        public void MouseLeavePort1(object sender, System.EventArgs e)
        {

            // UnHighLight Port 1
            if (nPorts >= 1)
            {
                highLightPortInstance(0, pcbTool1, lblData1, false);
            }
        }

        public void MouseLeavePort2(object sender, System.EventArgs e)
        {

            // UnHighLight Port 2
            if (nPorts >= 2)
            {
                highLightPortInstance(1, pcbTool2, lblData2, false);
            }
        }

        public void MouseLeavePort3(object sender, System.EventArgs e)
        {

            // UnHighLight Port 3
            if (nPorts >= 3)
            {
                highLightPortInstance(2, pcbTool3, lblData3, false);
            }
        }

        public void MouseLeavePort4(object sender, System.EventArgs e)
        {

            // UnHighLight Port 4
            if (nPorts >= 4)
            {
                highLightPortInstance(3, pcbTool4, lblData4, false);
            }
        }
    }
}
