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


namespace RemoteManager
{
    public partial class frmDockPanel
    {
        public frmDockPanel()
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            floatSizeToDock = this.Size;
            leftSizeToDock = this.Size;
            rightSizeToDock = this.Size;
            bottomSizeToDock = this.Size;
            topSizeToDock = this.Size;

            InitializeComponent();
        }
        private Size floatSizeToDock; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private Size leftSizeToDock; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private Size rightSizeToDock; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private Size bottomSizeToDock; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
        private Size topSizeToDock; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.

        public void frmDockPanel_Load(System.Object sender, System.EventArgs e)
        {
            butAutoHide.Tag = false;
            loadTextsDockPanel();
            myArrangeListPanels();
        }

        public void loadTextsDockPanel()
        {
            lblHeader.Text = Localization.getResStr(Configuration.dockStationListId);
            mnuFloating.Text = Localization.getResStr(Configuration.dockFloatingId);
            mnuTop.Text = Localization.getResStr(Configuration.dockTopId);
            mnuBottom.Text = Localization.getResStr(Configuration.dockBottomId);
            mnuLeft.Text = Localization.getResStr(Configuration.dockLeftId);
            mnuRight.Text = Localization.getResStr(Configuration.dockRightId);

            mnuList.Text = Localization.getResStr(Configuration.dockViewListId);
            mnuTreeList.Text = Localization.getResStr(Configuration.dockViewTreeListId);
            mnuTile.Text = Localization.getResStr(Configuration.dockViewTileId);
            mnuDetails.Text = Localization.getResStr(Configuration.dockViewDetailsId);

            butAutoHide.ToolTipText = Localization.getResStr(Configuration.dockAutoHideId);
            butWinPos.ToolTipText = Localization.getResStr(Configuration.dockPanelPosId);
            butClose.ToolTipText = Localization.getResStr(Configuration.dockcloseId);
        }

        public void butClose_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        //WINDOW POSITION FUNCTIONALITY

        public void mnuWinPos_Click(System.Object sender, System.EventArgs e)
        {
            //getting on the caller
            ToolStripMenuItem caller = (ToolStripMenuItem)sender;
            mySetWinPos(caller);
        }

        private void mySetWinPos(ToolStripMenuItem caller)
        {
            //saving the size of current dock
            if (this.Dock == DockStyle.None)
            {
                floatSizeToDock = this.Size;
            }
            else if (this.Dock == DockStyle.Top)
            {
                topSizeToDock = this.Size;
            }
            else if (this.Dock == DockStyle.Bottom)
            {
                bottomSizeToDock = this.Size;
            }
            else if (this.Dock == DockStyle.Left)
            {
                leftSizeToDock = this.Size;
            }
            else if (this.Dock == DockStyle.Right)
            {
                rightSizeToDock = this.Size;
            }

            //depending on the caller
            if (caller.Name == mnuFloating.Name)
            {
                this.Size = floatSizeToDock;
                this.Dock = DockStyle.None;
                this.Text = " ";
            }
            else if (caller.Name == mnuTop.Name)
            {
                this.Size = topSizeToDock;
                this.Dock = DockStyle.Top;
                this.Text = "";
            }
            else if (caller.Name == mnuBottom.Name)
            {
                this.Size = bottomSizeToDock;
                this.Dock = DockStyle.Bottom;
                this.Text = "";
            }
            else if (caller.Name == mnuLeft.Name)
            {
                this.Size = leftSizeToDock;
                this.Dock = DockStyle.Left;
                this.Text = "";
            }
            else if (caller.Name == mnuRight.Name)
            {
                this.Size = rightSizeToDock;
                this.Dock = DockStyle.Right;
                this.Text = "";
            }
        }

        public void myCheckViewType(ToolStripMenuItem mnu, bool bChecked)
        {
            mnu.Checked = bChecked;
            mySetViewType(mnu);
        }

        public void mnuViewType_Click(System.Object sender, System.EventArgs e)
        {
            //getting on the caller
            ToolStripMenuItem caller = (ToolStripMenuItem)sender;
            mySetViewType(caller);
        }

        private void mySetViewType(ToolStripMenuItem caller)
        {
            //MsgBox("List " & mnuList.Checked.ToString & " Tree " & mnuTreeList.Checked.ToString)
            if (caller.Name == mnuList.Name)
            {
                // si se deseleccionan los 2, seleccionar tree
                if (!mnuList.Checked && !mnuTreeList.Checked)
                {
                    mnuTreeList.Checked = true;
                }
            }
            else if (caller.Name == mnuTreeList.Name)
            {
                // si se deseleccionan los 2, seleccionar list
                if (!mnuList.Checked && !mnuTreeList.Checked)
                {
                    mnuList.Checked = true;
                }
            }
            else if (caller.Name == mnuTile.Name)
            {
                // si se deseleccionan los 2 y está seleccionado el tree, deseleccionar Lista, pero luego cambiar al contrario (omisión)
                if (!mnuTile.Checked && !mnuDetails.Checked && mnuTreeList.Checked)
                {
                    mnuList.Checked = false;
                }
                else
                {
                    mnuList.Checked = true;
                }
                mnuDetails.Checked = !mnuTile.Checked;
            }
            else if (caller.Name == mnuDetails.Name)
            {
                // si se deseleccionan los 2 y está seleccionado el tree, deseleccionar Lista, pero luego cambiar al contrario (omisión)
                if (!mnuTile.Checked && !mnuDetails.Checked && mnuTreeList.Checked)
                {
                    mnuList.Checked = false;
                }
                else
                {
                    mnuList.Checked = true;
                }
                mnuTile.Checked = !mnuDetails.Checked;
            }
            else
            {
                mnuList.Checked = true;
                mnuTile.Checked = true;
                mnuDetails.Checked = false;
            }
            if (mnuDetails.Checked)
            {
                lsvwStationList.View = View.Details;
            }
            if (mnuTile.Checked)
            {
                lsvwStationList.View = View.Tile;
            }

            myArrangeListPanels();

        }

        private void myArrangeListPanels()
        {
            try
            {
                if (mnuList.Checked && mnuTreeList.Checked)
                {
                    treevwStationList.Dock = DockStyle.None;
                    splitStationList.Dock = DockStyle.Left;
                    lsvwStationList.Dock = DockStyle.None;

                    lsvwStationList.Visible = true;
                    splitStationList.Visible = true;
                    treevwStationList.Visible = true;

                    treevwStationList.Dock = DockStyle.Bottom;
                    splitStationList.Dock = DockStyle.Bottom;
                    lsvwStationList.Dock = DockStyle.Fill;

                }
                else if (mnuList.Checked)
                {
                    treevwStationList.Dock = DockStyle.None;
                    splitStationList.Dock = DockStyle.Left;
                    lsvwStationList.Dock = DockStyle.None;

                    splitStationList.Visible = false;
                    treevwStationList.Visible = false;
                    lsvwStationList.Visible = true;

                    lsvwStationList.Dock = DockStyle.Fill;

                }
                else if (mnuTreeList.Checked)
                {
                    treevwStationList.Dock = DockStyle.None;
                    splitStationList.Dock = DockStyle.Left;
                    lsvwStationList.Dock = DockStyle.None;

                    lsvwStationList.Visible = false;
                    splitStationList.Visible = false;
                    treevwStationList.Visible = true;

                    treevwStationList.Dock = DockStyle.Fill;

                }

            }
            catch (Exception)
            {

            }
        }

        //FORM AUTOHIDE FUNCTIONALITY

        private Size prevSizeToAutoHide;
        private bool isCollapsed = false;

        public void butAutoHide_Click(System.Object sender, System.EventArgs e)
        {
            //if the form is docked
            if (this.Dock != DockStyle.None)
            {

                //setting the image of the autohide button and getting the previous size to autohide
                if (System.Convert.ToBoolean(butAutoHide.Tag) == false)
                {
                    prevSizeToAutoHide = this.Size;
                    butAutoHide.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    butAutoHide.Tag = true; // autohide selected
                }
                else
                {
                    butAutoHide.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    butAutoHide.Tag = false; // autohide not selected
                }

            }
        }

        public void DockPanel_MouseEnter(object sender, System.EventArgs e)
        {
            //if the autohide mode is active then showing the form
            if (System.Convert.ToBoolean(butAutoHide.Tag) == true && isCollapsed)
            {
                autoHideExpandForm();
            }
        }

        public void DockPanel_MouseLeave(object sender, System.EventArgs e)
        {
            //collpsed the header enter event is the most probably event.
            //to avoid the mouse leave events when moving to a control inside the form checking mouse position
            if (!this.DisplayRectangle.Contains(this.PointToClient(MousePosition)))
            {
                //if autohide mode is active then hiding the form
                if (System.Convert.ToBoolean(butAutoHide.Tag) == true && !isCollapsed)
                {
                    autoHideCollapseForm();
                }
            }
        }

        private void autoHideCollapseForm()
        {
            //indicating that is collapsed
            isCollapsed = true;

            //depending on the dock resizing the form
            if (this.Dock == DockStyle.Top | this.Dock == DockStyle.Bottom | this.Dock == DockStyle.Fill)
            {
                this.Height = 20;
            }
            else if (this.Dock == DockStyle.Left | this.Dock == DockStyle.Right)
            {
                this.Width = 20;
                this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            }

            lsvwStationList.Visible = false;
            treevwStationList.Visible = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            //hidding the header buttons
            butClose.Visible = false;
            butAutoHide.Visible = false;
            butWinPos.Visible = false;

            this.Refresh();

        }

        private void autoHideExpandForm()
        {
            //indicating the form is not collapsed
            isCollapsed = false;

            lsvwStationList.Visible = true;
            treevwStationList.Visible = true;
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;

            //resizing the form depending on the dock
            if (this.Dock == DockStyle.Top | this.Dock == DockStyle.Bottom | this.Dock == DockStyle.Fill)
            {
                this.Height = prevSizeToAutoHide.Height;
            }
            else if (this.Dock == DockStyle.Left | this.Dock == DockStyle.Right)
            {
                this.Width = prevSizeToAutoHide.Width;
            }

            //showing the header buttons
            butClose.Visible = true;
            butAutoHide.Visible = true;
            butWinPos.Visible = true;

            this.Refresh();
        }

        // MOVE FORM FUNCTIONALITY

        private Point oldLocation;

        public void tspFormHeader_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //if the left button is pressed and the form is not docked which means it's floating
            if (e.Button == System.Windows.Forms.MouseButtons.Left && this.Dock == System.Windows.Forms.DockStyle.None)
            {
                //setting the offset of the mouse from the form top left corner, it is the tspHeader left corner
                oldLocation = e.Location;
            }
        }

        public void tspFormHeader_MouseLeave(object sender, System.EventArgs e)
        {
            //if the left button is pressed and the form is not docked which means it's floating
            if (MouseButtons == System.Windows.Forms.MouseButtons.Left && this.Dock == System.Windows.Forms.DockStyle.None)
            {
                //the mouse has exit the header with the left button pressed, this means the mouse has moved faster than the form
                //this.Location = this.Location - (this.PointToScreen(oldLocation) - MousePosition);
            }
        }

        public void tspFormHeader_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //if the left button is pressed and the form is not docked which means it's floating
            if (e.Button == System.Windows.Forms.MouseButtons.Left & this.Dock == System.Windows.Forms.DockStyle.None)
            {
                //changing the form location
                //this.Location = this.Location - (oldLocation - e.Location);
            }
        }


        public void frmDockPanel_Move(System.Object sender, System.EventArgs e)
        {
            //lblHeader.Text = Me.Location.ToString
        }
    }
}
