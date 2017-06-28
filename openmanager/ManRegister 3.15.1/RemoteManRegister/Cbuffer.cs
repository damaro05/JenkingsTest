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
// End of VB project level imports

namespace RemoteManRegister
{
    //defines a buffer where to draw
    public class Cbuffer
    {
        public Bitmap bmp; //The bitmap
        public Graphics draw; //The drawing object of the bitmap

        public Cbuffer(int w, int h)
        {
            if (w > 0 & h > 0)
            {
                bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else
            {
                bmp = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            draw = Graphics.FromImage(bmp);
        }

        public void Dispose()
        {
            bmp.Dispose();
            draw.Dispose();
        }

        public void resize(int w, int h)
        {
            Dispose();
            if (w > 0 & h > 0)
            {
                bmp = new Bitmap(w, h);
            }
            else
            {
                bmp = new Bitmap(1, 1);
            }
            draw = Graphics.FromImage(bmp);
        }
    }
}
