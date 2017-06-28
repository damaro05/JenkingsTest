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
    public partial class TempPanels
    {

        private string selectedPageName;
        private bool enabledControls = false;


        public delegate void ClickControlEventHandler(string name);
        private ClickControlEventHandler ClickControlEvent;

        public event ClickControlEventHandler ClickControl
        {
            add
            {
                ClickControlEvent = (ClickControlEventHandler)System.Delegate.Combine(ClickControlEvent, value);
            }
            remove
            {
                ClickControlEvent = (ClickControlEventHandler)System.Delegate.Remove(ClickControlEvent, value);
            }
        }



        #region Constructor/destructor

        public TempPanels(string pageInitial)
        {
            // Llamada necesaria para el diseñador.
            InitializeComponent();

            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;

            pageTempManual.BackColor = Color.Transparent;
            pageTempManual.Location = new Point(0, 0);

            pageTempLevels.BackColor = Color.Transparent;
            pageTempLevels.Size = pageTempManual.Size;
            pageTempLevels.Location = new Point(0, 0);

            pageTempFixed.BackColor = Color.Transparent;
            pageTempFixed.Size = pageTempManual.Size;
            pageTempFixed.Location = new Point(0, 0);

            if (pageInitial != "")
            {
                CurrentPage = pageInitial;
            }
            this.EnableCtrls = false;

        }

        #endregion

        #region Properties

        public string CurrentPage
        {
            set
            {
                System.Windows.Forms.Control[] aControls = this.Controls.Find(value, true);
                if (aControls.Length > 0)
                {
                    aControls[0].BringToFront();
                    selectedPageName = System.Convert.ToString(aControls[0].Name);
                }
            }
            get
            {
                if (!string.IsNullOrEmpty(selectedPageName))
                {
                    return selectedPageName;
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
                Cursor cur = default(Cursor);
                if (value)
                {
                    cur = Cursors.Hand;
                }
                else
                {
                    cur = Cursors.Default;
                }
                pcbSubstract.Cursor = cur;
                pcbAdd.Cursor = cur;
                lblLvl1.Cursor = cur;
                lblLvl2.Cursor = cur;
                lblLvl3.Cursor = cur;

                enabledControls = value;
            }
            get
            {
                return enabledControls;
            }
        }

        #endregion

        public void setSelectedLevel(Label lblLvlSelected)
        {
            lblLvl1.Image = (Image)(My.Resources.Resources.ResourceManager.GetObject("Border")); // background en blanco
            lblLvl1.ForeColor = Configuration.stnColorTextSel;
            lblLvl2.Image = (Image)(My.Resources.Resources.ResourceManager.GetObject("Border")); // background en blanco
            lblLvl2.ForeColor = Configuration.stnColorTextSel;
            lblLvl3.Image = (Image)(My.Resources.Resources.ResourceManager.GetObject("Border")); // background en blanco
            lblLvl3.ForeColor = Configuration.stnColorTextSel;
            lblLvlSelected.Image = (Image)(My.Resources.Resources.ResourceManager.GetObject("SelectedBorder")); // background en gris
            lblLvlSelected.ForeColor = Color.White;
        }

        // Add and substract picturebox click event --------------------------------------------
        public void pcbAddAndSubs_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!enabledControls)
            {
                return;
            }
            // Casting the sender
            PictureBox pcb = (PictureBox)sender;
            // Moving the control to perform the press effect
            pcb.Location = new Point(pcb.Location.X + 1, pcb.Location.Y + 1);
        }

        public void pcbAddAndSubs_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!enabledControls)
            {
                return;
            }
            // Casting the sender
            PictureBox pcb = (PictureBox)sender;

            // Moving the control to the original position to restore from press effect
            pcb.Location = new Point(pcb.Location.X - 1, pcb.Location.Y - 1);

            if (ClickControlEvent != null)
                ClickControlEvent(pcb.Name);
        }

        // Temperature levels selection --------------------------------------------------------
        public void lblLvl_Click(object sender, System.EventArgs e)
        {
            if (!enabledControls)
            {
                return;
            }
            // Casting the sender
            Label lbl = (Label)sender;
            setSelectedLevel(lbl);
            if (ClickControlEvent != null)
                ClickControlEvent(lbl.Name);
        }

    }
}
