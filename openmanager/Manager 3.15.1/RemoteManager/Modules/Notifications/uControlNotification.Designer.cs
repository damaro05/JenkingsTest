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
    [global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public
partial class uControlNotification : System.Windows.Forms.UserControl
    {

        //UserControl overrides dispose to clean up the component list.
        [System.Diagnostics.DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        //Required by the Windows Form Designer
        private System.ComponentModel.Container components = null;

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.
        //Do not modify it using the code editor.
        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.button_left = new System.Windows.Forms.Button();
            this.button_left.Click += new System.EventHandler(this.button_left_Click);
            this.button_right = new System.Windows.Forms.Button();
            this.button_right.Click += new System.EventHandler(this.button_right_Click);
            this.imgNotif = new System.Windows.Forms.PictureBox();
            this.textBox_page = new System.Windows.Forms.TextBox();
            this.button_close = new System.Windows.Forms.Button();
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            this.textDescription = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)this.imgNotif).BeginInit();
            this.SuspendLayout();
            //
            //button_left
            //
            this.button_left.BackColor = System.Drawing.Color.Transparent;
            this.button_left.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_left.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_left.FlatAppearance.BorderSize = 0;
            this.button_left.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_left.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_left.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_left.Image = My.Resources.Resources.arrow_left;
            this.button_left.Location = new System.Drawing.Point(12, 25);
            this.button_left.Margin = new System.Windows.Forms.Padding(0);
            this.button_left.Name = "button_left";
            this.button_left.Size = new System.Drawing.Size(30, 30);
            this.button_left.TabIndex = 1;
            this.button_left.UseVisualStyleBackColor = false;
            //
            //button_right
            //
            this.button_right.BackColor = System.Drawing.Color.Transparent;
            this.button_right.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_right.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_right.FlatAppearance.BorderSize = 0;
            this.button_right.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_right.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_right.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_right.Image = My.Resources.Resources.arrow_right;
            this.button_right.Location = new System.Drawing.Point(548, 25);
            this.button_right.Margin = new System.Windows.Forms.Padding(0);
            this.button_right.Name = "button_right";
            this.button_right.Size = new System.Drawing.Size(30, 30);
            this.button_right.TabIndex = 2;
            this.button_right.UseVisualStyleBackColor = false;
            //
            //imgNotif
            //
            this.imgNotif.BackColor = System.Drawing.Color.Transparent;
            this.imgNotif.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.imgNotif.ErrorImage = null;
            this.imgNotif.InitialImage = null;
            this.imgNotif.Location = new System.Drawing.Point(58, 25);
            this.imgNotif.Margin = new System.Windows.Forms.Padding(0);
            this.imgNotif.Name = "imgNotif";
            this.imgNotif.Size = new System.Drawing.Size(30, 30);
            this.imgNotif.TabIndex = 7;
            this.imgNotif.TabStop = false;
            //
            //textBox_page
            //
            this.textBox_page.BackColor = System.Drawing.SystemColors.Control;
            this.textBox_page.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox_page.Enabled = false;
            this.textBox_page.Font = new System.Drawing.Font("Verdana", (float)(8.0F));
            this.textBox_page.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.textBox_page.Location = new System.Drawing.Point(4, 4);
            this.textBox_page.Name = "textBox_page";
            this.textBox_page.ReadOnly = true;
            this.textBox_page.Size = new System.Drawing.Size(53, 13);
            this.textBox_page.TabIndex = 9;
            //
            //button_close
            //
            this.button_close.BackColor = System.Drawing.Color.Transparent;
            this.button_close.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button_close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button_close.FlatAppearance.BorderSize = 0;
            this.button_close.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.button_close.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.button_close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_close.Image = My.Resources.Resources.BlackClose;
            this.button_close.Location = new System.Drawing.Point(566, 2);
            this.button_close.Margin = new System.Windows.Forms.Padding(0);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(20, 20);
            this.button_close.TabIndex = 10;
            this.button_close.UseVisualStyleBackColor = false;
            //
            //textDescription
            //
            this.textDescription.Location = new System.Drawing.Point(103, 13);
            this.textDescription.Name = "textDescription";
            this.textDescription.Size = new System.Drawing.Size(410, 54);
            this.textDescription.TabIndex = 11;
            this.textDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            //uControlNotification
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF((float)(8.0F), (float)(14.0F));
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.textBox_page);
            this.Controls.Add(this.imgNotif);
            this.Controls.Add(this.button_right);
            this.Controls.Add(this.button_left);
            this.Font = new System.Drawing.Font("Verdana", (float)(9.0F));
            this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "uControlNotification";
            this.Size = new System.Drawing.Size(590, 80);
            ((System.ComponentModel.ISupportInitialize)this.imgNotif).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.Button button_left;
        internal System.Windows.Forms.Button button_right;
        internal System.Windows.Forms.PictureBox imgNotif;
        internal System.Windows.Forms.TextBox textBox_page;
        internal System.Windows.Forms.Button button_close;
        internal System.Windows.Forms.Label textDescription;

    }
}
