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
using DataJBC;


namespace RemoteManager
{
    public partial class TabPanels
    {
        private List<Panel> pages;
        private Rectangle pageArea;

        public string CurrentPage
        {
            set
            {
                System.Windows.Forms.Control[] aControls = this.Controls.Find(value, true);
                if (aControls.Length > 0)
                {
                    if (selectedPage != null)
                    {
                        if (selectedPage.Name != value)
                        {
                            selectedPage.Visible = false;
                        }
                    }
                    //aControls(0).BringToFront()
                    aControls[0].Visible = true;
                    selectedPage = (Panel)aControls[0];
                }
            }
            get
            {
                if (selectedPage != null)
                {
                    return selectedPage.Name;
                }
                else
                {
                    return "";
                }
            }
        }

        public bool EnableCtrls
        {
            set
            {
                Cursor cur;
                if (value)
                {
                    cur = Cursors.Hand;
                }
                else
                {
                    cur = Cursors.Default;
                }
                //pcbSubstract.Cursor = cur
                //pcbAdd.Cursor = cur
                //lblLvl1.Cursor = cur
                //lblLvl2.Cursor = cur
                //lblLvl3.Cursor = cur

                enabledControls = value;
            }
            get
            {
                return enabledControls;
            }
        }

        //Private selectedPageName As String
        private Panel selectedPage;
        private bool enabledControls = false;

        // Public Events
        public delegate void work_ClickControlEventHandler(Control ctrl);
        private work_ClickControlEventHandler work_ClickControlEvent;

        public event work_ClickControlEventHandler work_ClickControl
        {
            add
            {
                work_ClickControlEvent = (work_ClickControlEventHandler)System.Delegate.Combine(work_ClickControlEvent, value);
            }
            remove
            {
                work_ClickControlEvent = (work_ClickControlEventHandler)System.Delegate.Remove(work_ClickControlEvent, value);
            }
        }

        public delegate void toolSettingsTool_CheckedChangedEventHandler(Control ctrl);
        private toolSettingsTool_CheckedChangedEventHandler toolSettingsTool_CheckedChangedEvent;

        public event toolSettingsTool_CheckedChangedEventHandler toolSettingsTool_CheckedChanged
        {
            add
            {
                toolSettingsTool_CheckedChangedEvent = (toolSettingsTool_CheckedChangedEventHandler)System.Delegate.Combine(toolSettingsTool_CheckedChangedEvent, value);
            }
            remove
            {
                toolSettingsTool_CheckedChangedEvent = (toolSettingsTool_CheckedChangedEventHandler)System.Delegate.Remove(toolSettingsTool_CheckedChangedEvent, value);
            }
        }

        public delegate void toolSettingsPort_CheckedChangedEventHandler(Control ctrl);
        private toolSettingsPort_CheckedChangedEventHandler toolSettingsPort_CheckedChangedEvent;

        public event toolSettingsPort_CheckedChangedEventHandler toolSettingsPort_CheckedChanged
        {
            add
            {
                toolSettingsPort_CheckedChangedEvent = (toolSettingsPort_CheckedChangedEventHandler)System.Delegate.Combine(toolSettingsPort_CheckedChangedEvent, value);
            }
            remove
            {
                toolSettingsPort_CheckedChangedEvent = (toolSettingsPort_CheckedChangedEventHandler)System.Delegate.Remove(toolSettingsPort_CheckedChangedEvent, value);
            }
        }

        public delegate void conf_ClickControlEventHandler(Control ctrl);
        private conf_ClickControlEventHandler conf_ClickControlEvent;

        public event conf_ClickControlEventHandler conf_ClickControl
        {
            add
            {
                conf_ClickControlEvent = (conf_ClickControlEventHandler)System.Delegate.Combine(conf_ClickControlEvent, value);
            }
            remove
            {
                conf_ClickControlEvent = (conf_ClickControlEventHandler)System.Delegate.Remove(conf_ClickControlEvent, value);
            }
        }

        public delegate void reset_ClickControlEventHandler(Control ctrl);
        private reset_ClickControlEventHandler reset_ClickControlEvent;

        public event reset_ClickControlEventHandler reset_ClickControl
        {
            add
            {
                reset_ClickControlEvent = (reset_ClickControlEventHandler)System.Delegate.Combine(reset_ClickControlEvent, value);
            }
            remove
            {
                reset_ClickControlEvent = (reset_ClickControlEventHandler)System.Delegate.Remove(reset_ClickControlEvent, value);
            }
        }

        public delegate void countersPort_CheckedChangedEventHandler(Control ctrl);
        private countersPort_CheckedChangedEventHandler countersPort_CheckedChangedEvent;

        public event countersPort_CheckedChangedEventHandler countersPort_CheckedChanged
        {
            add
            {
                countersPort_CheckedChangedEvent = (countersPort_CheckedChangedEventHandler)System.Delegate.Combine(countersPort_CheckedChangedEvent, value);
            }
            remove
            {
                countersPort_CheckedChangedEvent = (countersPort_CheckedChangedEventHandler)System.Delegate.Remove(countersPort_CheckedChangedEvent, value);
            }
        }

        public delegate void countersType_CheckedChangedEventHandler(string sType);
        private countersType_CheckedChangedEventHandler countersType_CheckedChangedEvent;

        public event countersType_CheckedChangedEventHandler countersType_CheckedChanged
        {
            add
            {
                countersType_CheckedChangedEvent = (countersType_CheckedChangedEventHandler)System.Delegate.Combine(countersType_CheckedChangedEvent, value);
            }
            remove
            {
                countersType_CheckedChangedEvent = (countersType_CheckedChangedEventHandler)System.Delegate.Remove(countersType_CheckedChangedEvent, value);
            }
        }

        public delegate void countersResetPortPartialCounters_ClickEventHandler();
        private countersResetPortPartialCounters_ClickEventHandler countersResetPortPartialCounters_ClickEvent;

        public event countersResetPortPartialCounters_ClickEventHandler countersResetPortPartialCounters_Click
        {
            add
            {
                countersResetPortPartialCounters_ClickEvent = (countersResetPortPartialCounters_ClickEventHandler)System.Delegate.Combine(countersResetPortPartialCounters_ClickEvent, value);
            }
            remove
            {
                countersResetPortPartialCounters_ClickEvent = (countersResetPortPartialCounters_ClickEventHandler)System.Delegate.Remove(countersResetPortPartialCounters_ClickEvent, value);
            }
        }

        public delegate void graphPlots_DropDownEventHandler(Control ctrl); // for filling register windows
        private graphPlots_DropDownEventHandler graphPlots_DropDownEvent;

        public event graphPlots_DropDownEventHandler graphPlots_DropDown
        {
            add
            {
                graphPlots_DropDownEvent = (graphPlots_DropDownEventHandler)System.Delegate.Combine(graphPlots_DropDownEvent, value);
            }
            remove
            {
                graphPlots_DropDownEvent = (graphPlots_DropDownEventHandler)System.Delegate.Remove(graphPlots_DropDownEvent, value);
            }
        }

        public delegate void graphAddSeries_ClickEventHandler(Control ctrl);
        private graphAddSeries_ClickEventHandler graphAddSeries_ClickEvent;

        public event graphAddSeries_ClickEventHandler graphAddSeries_Click
        {
            add
            {
                graphAddSeries_ClickEvent = (graphAddSeries_ClickEventHandler)System.Delegate.Combine(graphAddSeries_ClickEvent, value);
            }
            remove
            {
                graphAddSeries_ClickEvent = (graphAddSeries_ClickEventHandler)System.Delegate.Remove(graphAddSeries_ClickEvent, value);
            }
        }

        public delegate void stationSettings_CheckedChangedEventHandler(string sType);
        private stationSettings_CheckedChangedEventHandler stationSettings_CheckedChangedEvent;

        public event stationSettings_CheckedChangedEventHandler stationSettings_CheckedChanged
        {
            add
            {
                stationSettings_CheckedChangedEvent = (stationSettings_CheckedChangedEventHandler)System.Delegate.Combine(stationSettings_CheckedChangedEvent, value);
            }
            remove
            {
                stationSettings_CheckedChangedEvent = (stationSettings_CheckedChangedEventHandler)System.Delegate.Remove(stationSettings_CheckedChangedEvent, value);
            }
        }


        public TabPanels(string pageInitial, GenericStationTools[] SupportedTools)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();

            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;

            // build TableLayoutPanelTools
            // evitar que al dar nombre a un boton, ya exista.
            for (var i = TableLayoutPanelTools.ColumnCount - 1; i >= 0; i--)
            {
                RadioButton rb = (RadioButton)(TableLayoutPanelTools.GetControlFromPosition(System.Convert.ToInt32(i), 0));
                if (i < SupportedTools.Length)
                {
                    rb.Name = "rbToolSettings_" + SupportedTools[i].ToString();
                    rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(SupportedTools[i].ToString() + "_mini");
                    ToolTip1.SetToolTip(rb, SupportedTools[i].ToString());
                }
                else
                {
                    rb.Name = "rbToolSettings_" + i.ToString();
                    rb.Visible = false;
                }
            }

            pages = new List<Panel>();

            foreach (Control ctrl in this.Controls)
            {
                if (ctrl.Name.Substring(0, 4) == "page")
                {
                    ctrl.BackColor = Color.Transparent;
                    ctrl.Size = pageWork.Size;
                    ctrl.Location = new Point(0, 0);
                    ctrl.Visible = false;
                    pages.Add((Panel)ctrl);
                }
            }

            // relocate those panel in pageWork
            //pageDummy
            lblNoTool.Parent = pageWork;
            lblToolNeeded.Parent = pageWork;
            lblNoTool.Visible = false;
            lblToolNeeded.Visible = false;
            //pageDummy2
            panelWorkSleep.Parent = pageWork;
            panelWorkSleep.Visible = false;
            //pageDummy3
            panelWorkPageProfilePlot.Parent = pageWork_HA;
            labelWorkProfile_HA_HotAirTempTitle.Parent = pageWork_HA;
            labelWorkProfile_HA_HotAirTemp.Parent = pageWork_HA;
            labelWorkProfile_HA_ExtTCTempTitle.Parent = pageWork_HA;
            labelWorkProfile_HA_ExtTCTemp.Parent = pageWork_HA;
            labelWorkProfile_HA_AirFlowTitle.Parent = pageWork_HA;
            labelWorkProfile_HA_AirFlow.Parent = pageWork_HA;
            labelWorkProfile_HA_Status.Parent = pageWork_HA;
            panelWorkPageProfilePlot.Visible = false;
            labelWorkProfile_HA_HotAirTempTitle.Visible = false;
            labelWorkProfile_HA_HotAirTemp.Visible = false;
            labelWorkProfile_HA_ExtTCTempTitle.Visible = false;
            labelWorkProfile_HA_ExtTCTemp.Visible = false;
            labelWorkProfile_HA_AirFlowTitle.Visible = false;
            labelWorkProfile_HA_AirFlow.Visible = false;
            labelWorkProfile_HA_Status.Visible = false;


            this.Size = pageWork.Size;
            this.BackColor = Color.Transparent;
            pageArea = new Rectangle(0, 0, this.Width, this.Height);

            if (pageInitial != "")
            {
                CurrentPage = pageInitial;
            }
            this.EnableCtrls = false;

        }
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (pages != null)
            {
                foreach (Panel page in pages)
                {
                    page.Size = new Size(this.Width, this.Height);
                }
            }

        }

        // work page
        // Port selection ----------------------------------------------------------------------
        public void pcbTool_MouseEnter(object sender, System.EventArgs e)
        {
            // HighLight
            pcbTool.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("ToolSelected_big");
        }

        public void pcbTool_MouseLeave(object sender, System.EventArgs e)
        {
            // UnHighLight
            pcbTool.Image = null;
        }

        public void pcbTool_Click(System.Object sender, System.EventArgs e)
        {
            if (work_ClickControlEvent != null)
                work_ClickControlEvent((Control)sender);
        }

        //
        // station settings page
        //
        public void rbTabSettings_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                if (ReferenceEquals(((RadioButton)sender), rbEthernetConf))
                {
                    if (stationSettings_CheckedChangedEvent != null)
                        stationSettings_CheckedChangedEvent("Eth");
                }
                else if (ReferenceEquals(((RadioButton)sender), rbRobotConf))
                {
                    if (stationSettings_CheckedChangedEvent != null)
                        stationSettings_CheckedChangedEvent("Rbt");
                }
                else
                {
                    if (stationSettings_CheckedChangedEvent != null)
                        stationSettings_CheckedChangedEvent("Gen");
                }
            }
        }

        // tool settings page
        public void rbPort_MouseEnter(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("Port" + Configuration.myGetRadioButtonPortNbr(rb).ToString() + "miniMouseOver");
                rb.Refresh();
            }
        }

        public void rbPort_MouseLeave(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject("Port" + Configuration.myGetRadioButtonPortNbr(rb).ToString() + "mini");
                rb.Refresh();
            }
        }


        public void rbToolSettings_MouseEnter(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(Configuration.myGetRadioButtonToolName(rb) + "_miniMouseOver");
                rb.Refresh();
            }
        }

        public void rbToolSettings_MouseLeave(System.Object sender, System.EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked)
            {
                rb.Image = (Image)My.Resources.Resources.ResourceManager.GetObject(Configuration.myGetRadioButtonToolName(rb) + "_mini");
                rb.Refresh();
            }
        }

        public void rbToolSettings_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (toolSettingsTool_CheckedChangedEvent != null)
                toolSettingsTool_CheckedChangedEvent((Control)sender);
        }

        public void rbPortTools_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (toolSettingsPort_CheckedChangedEvent != null)
                toolSettingsPort_CheckedChangedEvent((Control)sender);
        }

        // load save settings page

        public void butConf_Click(System.Object sender, System.EventArgs e)
        {
            if (conf_ClickControlEvent != null)
                conf_ClickControlEvent((Control)sender);
        }

        // reset page

        public void butResetProceed_Click(System.Object sender, System.EventArgs e)
        {
            if (reset_ClickControlEvent != null)
                reset_ClickControlEvent((Control)sender);
        }

        // counters page

        public void rbPortCounters_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (countersPort_CheckedChangedEvent != null)
                countersPort_CheckedChangedEvent((Control)sender);
        }

        public void rbCountersType_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (((RadioButton)sender).Checked)
            {
                if (ReferenceEquals(((RadioButton)sender), rbPartialCounters))
                {
                    if (countersType_CheckedChangedEvent != null)
                        countersType_CheckedChangedEvent("P");
                }
                else
                {
                    if (countersType_CheckedChangedEvent != null)
                        countersType_CheckedChangedEvent("G");
                }
            }
        }

        public void butResetPartialCounters_Click(System.Object sender, System.EventArgs e)
        {
            if (countersResetPortPartialCounters_ClickEvent != null)
                countersResetPortPartialCounters_ClickEvent();
        }

        // graphics page

        public void cbxGraphPlots_DropDown(System.Object sender, System.EventArgs e)
        {
            if (graphPlots_DropDownEvent != null)
                graphPlots_DropDownEvent((Control)sender);
        }

        public void butGraphAddSeries_Click(System.Object sender, System.EventArgs e)
        {
            if (graphAddSeries_ClickEvent != null)
                graphAddSeries_ClickEvent((Control)sender);
        }

    }
}
