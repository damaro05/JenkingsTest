// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using JBC_ConnectRemote;
// End of VB project level imports

using Microsoft.VisualBasic.CompilerServices;

namespace RemoteManRegister
{
    public partial class frmTemplate
    {
        //Private stationLst As New List(Of String)
        private System.Windows.Forms.TreeNode nodeEdited;
        private Cplot.tTPTmain template = new Cplot.tTPTmain();

        private frmMainRegister frmMainReg;
        private List<ManRegGlobal.cListStation> stationList = new List<ManRegGlobal.cListStation>();
        private bool bUpdatingStations = false;
        private JBC_API_Remote jbc = null;

        public frmTemplate(frmMainRegister pMainReg, JBC_API_Remote _jbc)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();
            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;
            frmMainReg = pMainReg;
            jbc = _jbc;
        }

        public void frmTemplate_Load(System.Object sender, System.EventArgs e)
        {
            ReloadTexts();

            //loading the template list
            loadTemplateList();

            //clearing the tree
            trvTemplate.Nodes.Clear();

            //disabling the apply button
            butApply.Enabled = false;

            //selecting by default the port 1 for the trigger
            cbxPortTrigger.SelectedIndex = 0;
        }

        public void ReloadTexts()
        {
            Text = Localization.getResStr(ManRegGlobal.regMnuConfigSeriesId);

            lblTemplateList.Text = Localization.getResStr(ManRegGlobal.regTemplateListId);
            gbTemplateParams.Text = Localization.getResStr(ManRegGlobal.regTemplateParamsId);
            lblSeriesTree.Text = Localization.getResStr(ManRegGlobal.regTemplateSeriesDataId);
            lblTriggerTool.Text = Localization.getResStr(ManRegGlobal.regTemplateTriggerId);
            lblStation.Text = Localization.getResStr(ManRegGlobal.regSeriesSerieStationId);
            lblPort.Text = Localization.getResStr(ManRegGlobal.regSeriesSeriePortId);
            butApply.Text = Localization.getResStr(ManRegGlobal.regButOkId);
            butCancel.Text = Localization.getResStr(ManRegGlobal.regButCancelId);

        }

        public void trvTemplate_NodeMouseClick(object sender, System.Windows.Forms.TreeNodeMouseClickEventArgs e)
        {
            ToolStripItem tsItem;
            trvTemplate.ExpandAll();
            nodeEdited = e.Node;
            if (e.Node.Level == 1)
            {
                //port node, filling the context menu strip with the ports numbers
                cmsTreeNodeEdit.Items.Clear();
                for (int i = 1; i <= 4; i++)
                {
                    tsItem = cmsTreeNodeEdit.Items.Add(System.Convert.ToString(i));
                    tsItem.Tag = i;
                }
                cmsTreeNodeEdit.Show(trvTemplate, e.X, e.Y);
            }

            if (e.Node.Level == 0)
            {
                //station node, filling the context menu strip with the available stations and showing it
                cmsTreeNodeEdit.Items.Clear();
                //loadAvailableStations()
                updateConnectedStations();
                foreach (ManRegGlobal.cListStation stn in stationList)
                {
                    tsItem = cmsTreeNodeEdit.Items.Add(stn.Text);
                    tsItem.Tag = stn;
                }
                cmsTreeNodeEdit.Show(trvTemplate, e.X, e.Y);
            }
        }

        public void cmsTreeNodeEdit_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {
            //setting the edited node new value
            nodeEdited.Text = e.ClickedItem.Text;
            // saving station object or port number in tag
            nodeEdited.Tag = e.ClickedItem.Tag;
        }

        public void lbxTemplateList_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            //if selected index is correct loading the template
            if (lbxTemplateList.SelectedIndex >= 0)
            {
                string temp_file = frmMainReg.TEMPLATE_DIRECTORY + "\\" + System.Convert.ToString(lbxTemplateList.SelectedItem);
                loadTemplate(ref temp_file);
            }
        }

        public void butApply_Click(System.Object sender, System.EventArgs e)
        {
            applyTemplate();

            //exiting the template form
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        public void butCancel_Click(System.Object sender, System.EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        //Private Sub cbxStationTrigger_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbxStationTrigger.Click
        //loadAvailableStations()
        //cbxStationTrigger.Items.Clear()
        //For Each s As String In stationLst
        //    cbxStationTrigger.Items.Add(s)
        //Next
        //End Sub

        public void cbxStationTrigger_DropDown(dynamic sender, System.EventArgs e)
        {
            sender.SuspendLayout();
            //updating the list of connected stations
            updateConnectedStations();
            sender.ResumeLayout();
        }

        //Private Sub loadAvailableStations()
        //    ' Loading the connected stations
        //    stationLst.Clear()
        //    For Each ID As ULong In connectedStations
        //        stationLst.Add(ID.ToString & " - " & jbc.GetStationName(ID))
        //    Next

        //End Sub

        private void updateConnectedStations()
        {
            bUpdatingStations = true;
            string selectedValue = System.Convert.ToString(cbxStationTrigger.SelectedValue);

            // Loading the connected stations
            //foreach (ManRegGlobal.cListStation stn in stationList)
            //{
            //    stn = null;
            //}
            stationList.Clear();
            foreach (ManRegGlobal.cConnectedStation connStn in ManRegGlobal.connectedStations)
            {
                ManRegGlobal.cListStation stn = new ManRegGlobal.cListStation(connStn.ID, jbc.GetStationName(long.Parse(connStn.ID)), jbc.GetStationModel(long.Parse(connStn.ID)));
                stationList.Add(stn);
            }
            cbxStationTrigger.DataSource = stationList;
            cbxStationTrigger.DisplayMember = "Text";
            cbxStationTrigger.ValueMember = "ID";

            // Restoring the selected item
            try
            {
                cbxStationTrigger.SelectedValue = selectedValue;
            }
            catch (Exception)
            {
                cbxStationTrigger.SelectedIndex = -1;
            }

            bUpdatingStations = false;
        }

        private void loadTemplateList()
        {
            if (Microsoft.VisualBasic.FileIO.FileSystem.DirectoryExists(frmMainReg.TEMPLATE_DIRECTORY))
            {
                lbxTemplateList.Items.Clear();
                foreach (string f in Microsoft.VisualBasic.FileIO.FileSystem.GetFiles(frmMainReg.TEMPLATE_DIRECTORY))
                {
                    lbxTemplateList.Items.Add(f.Substring(f.LastIndexOf("\\") + 1));
                }
            }
        }

        private void loadTemplate(ref string file)
        {
            //loading the template
            frmMainReg.plot.loadTPT(file, template);

            //clearing the tree
            trvTemplate.Nodes.Clear();

            //setting the template tree
            System.Windows.Forms.TreeNode node = default(System.Windows.Forms.TreeNode);
            int station = 0;
            int port = 0;
            foreach (Cplot.tTPTserie s in template.series)
            {
                //if the serie station don't exist creating it
                // 06/08/2013 #edu# station model added in template. Current format "ID - stnname - stnmodel"
                // do not match with node text
                string[] seriestn = s.station.Split("-".ToCharArray());
                Array.Resize(ref seriestn, 3);
                ManRegGlobal.cListStation stn = new ManRegGlobal.cListStation(seriestn[0].Trim(), seriestn[1].Trim(), seriestn[2].Trim());
                if (!lookForNode(trvTemplate.Nodes, stn.Text, ref station))
                {
                    node = trvTemplate.Nodes.Add(stn.Text);
                    node.Tag = stn;
                    node.ForeColor = Color.Black;
                    node = node.Nodes.Add(System.Convert.ToString(s.port));
                    node.ForeColor = Color.Black;
                    node.Tag = s.port;
                    node = node.Nodes.Add(s.name);
                    node.ForeColor = s.clr;
                }
                else
                {
                    if (!lookForNode(trvTemplate.Nodes[station].Nodes, System.Convert.ToString(s.port), ref port))
                    {
                        node = trvTemplate.Nodes[station].Nodes.Add(System.Convert.ToString(s.port));
                        node.ForeColor = Color.Black;
                        node = node.Nodes.Add(s.name);
                        node.ForeColor = s.clr;
                    }
                    else
                    {
                        node = trvTemplate.Nodes[station].Nodes[port].Nodes.Add(s.name);
                        node.ForeColor = s.clr;
                    }
                }
            }

            //updating the tree
            trvTemplate.ExpandAll();
            trvTemplate.Update();

            //indicating the trigger type
            Cplot.cTrigger triggerType = template.trigger;
            lblTriggerType.Text = ManRegGlobal.getTriggerText(triggerType);

            //enabling the apply button
            butApply.Enabled = true;
        }

        private void applyTemplate()
        {
            //before being able to aply a template the selected stations must be connected
            if (checkSelectedStations())
            {
                //setting the series list for the template from the tree nodes
                int cnt = 0;
                bool found = false;
                Cplot.tTPTserie serie = new Cplot.tTPTserie();
                string station = "";
                foreach (System.Windows.Forms.TreeNode stationNode in trvTemplate.Nodes)
                {
                    foreach (System.Windows.Forms.TreeNode portNode in stationNode.Nodes)
                    {
                        foreach (System.Windows.Forms.TreeNode serieNode in portNode.Nodes)
                        {
                            //serie node, getting its position on the template list
                            found = false;
                            cnt = 0;
                            while (cnt < template.series.Count && !found)
                            {
                                if (template.series[cnt].name == serieNode.Text)
                                {
                                    found = true;
                                }
                                if (template.series[cnt].name != serieNode.Text)
                                {
                                    cnt++;
                                }
                            }

                            //assiging the new station and port
                            if (found)
                            {
                                serie.clr = template.series[cnt].clr;
                                serie.mag = System.Convert.ToInt32(template.series[cnt].mag);
                                serie.name = System.Convert.ToString(template.series[cnt].name);
                                serie.port = Convert.ToInt32(portNode.Text);
                                // 06/08/2013 #edu# station model added in template. Current format "ID - stnname - stnmodel"
                                station = ((ManRegGlobal.cListStation)stationNode.Tag).ID + " - " +
                                          ((ManRegGlobal.cListStation)stationNode.Tag).Name + " - " +
                                          ((ManRegGlobal.cListStation)stationNode.Tag).Model;
                                serie.station = station;
                                template.series.RemoveAt(cnt);
                                template.series.Add(serie);
                            }
                        }
                    }
                }

                //setting the trigger tool
                if (cbxStationTrigger.SelectedIndex == -1 | cbxStationTrigger.Items.Count == 0)
                {
                    frmMainReg.triggerStationID = UInt64.MaxValue;
                }
                else
                {
                    frmMainReg.triggerStationID = Convert.ToUInt64(cbxStationTrigger.SelectedValue);
                }
                frmMainReg.triggerPort = Convert.ToInt32(cbxPortTrigger.SelectedItem);

                //applying the template
                frmMainReg.plot.applyTPT(template);

                //storing the axis config
                frmMainReg.plot.getAxisConfig("temp", ref frmMainReg.config.axisAndGrid.Tmin, ref frmMainReg.config.axisAndGrid.Tmax, ref frmMainReg.config.axisAndGrid.Tstep);
                frmMainReg.plot.getAxisConfig("pwr", ref frmMainReg.config.axisAndGrid.Pmin, ref frmMainReg.config.axisAndGrid.Pmax, ref frmMainReg.config.axisAndGrid.Pstep);
                frmMainReg.plot.getAxisConfig("time_val", ref frmMainReg.config.axisAndGrid.timeMin, ref frmMainReg.config.axisAndGrid.timeMax, ref frmMainReg.config.axisAndGrid.timeStep);
                frmMainReg.config.axisAndGrid.timeRange = frmMainReg.config.axisAndGrid.timeMax - frmMainReg.config.axisAndGrid.timeMin;
            }
            else
            {
                //indicating the selected stations are not correct
                Interaction.MsgBox("Not all the template required stations have been selected.", MsgBoxStyle.Critical, null);
            }
        }

        private bool lookForNode(System.Windows.Forms.TreeNodeCollection lst, string node, ref int index)
        {
            //looking for the passed node
            bool found = false;
            int cnt = 0;
            while (cnt < lst.Count && !found)
            {
                if (node == lst[cnt].Text)
                {
                    found = true;
                }
                else
                {
                    cnt++;
                }
            }

            //returning the search result
            index = cnt;
            return found;
        }

        private bool checkSelectedStations()
        {
            //loading the current available stations
            //loadAvailableStations()
            updateConnectedStations();

            //checking all the 0 level nodes, it is the station nodes
            bool correct = true;
            bool found = false;
            int cnt = 0;
            foreach (System.Windows.Forms.TreeNode node in trvTemplate.Nodes)
            {
                if (node.Level == 0)
                {
                    found = false;
                    cnt = 0;
                    while (cnt < stationList.Count && !found)
                    {
                        if (ReferenceEquals(node.Tag, null))
                        {
                            // comparing by text
                            if (stationList[cnt].Text == node.Text)
                            {
                                found = true;
                            }
                            if (stationList[cnt].Text != node.Text)
                            {
                                cnt++;
                            }
                        }
                        else
                        {
                            // comparing by class cListStation (in Tag property of the node)
                            // #edu# comparing by station name; if blank, by id
                            if (Strings.Trim(((ManRegGlobal.cListStation)node.Tag).Name) == "")
                            {
                                if (stationList[cnt].ID == ((ManRegGlobal.cListStation)node.Tag).ID)
                                {
                                    found = true;
                                }
                                if (stationList[cnt].ID != ((ManRegGlobal.cListStation)node.Tag).ID)
                                {
                                    cnt++;
                                }
                            }
                            else
                            {
                                if (stationList[cnt].Name == ((ManRegGlobal.cListStation)node.Tag).Name)
                                {
                                    found = true;
                                }
                                if (stationList[cnt].Name != ((ManRegGlobal.cListStation)node.Tag).Name)
                                {
                                    cnt++;
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


    }
}
