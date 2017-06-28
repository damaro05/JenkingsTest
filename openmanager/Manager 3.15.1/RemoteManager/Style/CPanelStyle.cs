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
    public class CPanelStyle
    {

        public static void SetPanelStyle(ref System.Windows.Forms.PaintEventArgs e, ref Rectangle borderForm)
        {
            SetPanelStyle(e, borderForm, Color.Gray);
        }

        public static void SetPanelStyle(System.Windows.Forms.PaintEventArgs e, Rectangle borderForm, Color borderColor)
        {

            ControlPaint.DrawBorder(e.Graphics, borderForm,
                borderColor, 1, ButtonBorderStyle.Solid,
                borderColor, 1, ButtonBorderStyle.Solid,
                borderColor, 1, ButtonBorderStyle.Solid,
                borderColor, 1, ButtonBorderStyle.Solid);
        }

    }
}
