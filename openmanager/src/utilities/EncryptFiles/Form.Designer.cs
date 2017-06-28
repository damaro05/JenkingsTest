// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


namespace EncryptFiles
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class Form : System.Windows.Forms.Form
	{
		
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
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
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			this.lineSeparator = new System.Windows.Forms.Label();
			this.butSingleFileOrigin = new System.Windows.Forms.Button();
			this.butSingleFileOrigin.Click += new System.EventHandler(this.butSingleFileOrigin_Click);
			this.labelEncryptFiles = new System.Windows.Forms.Label();
			this.textBoxbutSingleFileOrigin = new System.Windows.Forms.TextBox();
			this.labelFilesOrigin = new System.Windows.Forms.Label();
			this.labelFilesDestination = new System.Windows.Forms.Label();
			this.textBoxbutSingleFileDestination = new System.Windows.Forms.TextBox();
			this.butSingleFileDestination = new System.Windows.Forms.Button();
			this.butSingleFileDestination.Click += new System.EventHandler(this.butSingleFileDestination_Click);
			this.butExecute = new System.Windows.Forms.Button();
			this.butExecute.Click += new System.EventHandler(this.butExecute_Click);
			this.Panel1 = new System.Windows.Forms.Panel();
			this.radioButtonDecrypt = new System.Windows.Forms.RadioButton();
			this.radioButtonEncrypt = new System.Windows.Forms.RadioButton();
			this.textBoxResult = new System.Windows.Forms.TextBox();
			this.Panel1.SuspendLayout();
			this.SuspendLayout();
			//
			//lineSeparator
			//
			this.lineSeparator.BackColor = System.Drawing.SystemColors.ControlDark;
			this.lineSeparator.Location = new System.Drawing.Point(60, 42);
			this.lineSeparator.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.lineSeparator.Name = "lineSeparator";
			this.lineSeparator.Size = new System.Drawing.Size(814, 1);
			this.lineSeparator.TabIndex = 33;
			//
			//butSingleFileOrigin
			//
			this.butSingleFileOrigin.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butSingleFileOrigin.Font = new System.Drawing.Font("Verdana", (float) (9.0F));
			this.butSingleFileOrigin.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
			this.butSingleFileOrigin.Location = new System.Drawing.Point(764, 94);
			this.butSingleFileOrigin.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.butSingleFileOrigin.Name = "butSingleFileOrigin";
			this.butSingleFileOrigin.Size = new System.Drawing.Size(110, 27);
			this.butSingleFileOrigin.TabIndex = 34;
			this.butSingleFileOrigin.Text = "Select file";
			this.butSingleFileOrigin.UseVisualStyleBackColor = true;
			//
			//labelEncryptFiles
			//
			this.labelEncryptFiles.AutoSize = true;
			this.labelEncryptFiles.Font = new System.Drawing.Font("Verdana", (float) (9.0F), System.Drawing.FontStyle.Bold);
			this.labelEncryptFiles.Location = new System.Drawing.Point(20, 35);
			this.labelEncryptFiles.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.labelEncryptFiles.Name = "labelEncryptFiles";
			this.labelEncryptFiles.Size = new System.Drawing.Size(89, 14);
			this.labelEncryptFiles.TabIndex = 35;
			this.labelEncryptFiles.Text = "Encrypt files";
			//
			//textBoxbutSingleFileOrigin
			//
			this.textBoxbutSingleFileOrigin.Location = new System.Drawing.Point(63, 97);
			this.textBoxbutSingleFileOrigin.Multiline = true;
			this.textBoxbutSingleFileOrigin.Name = "textBoxbutSingleFileOrigin";
			this.textBoxbutSingleFileOrigin.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxbutSingleFileOrigin.Size = new System.Drawing.Size(693, 75);
			this.textBoxbutSingleFileOrigin.TabIndex = 36;
			//
			//labelFilesOrigin
			//
			this.labelFilesOrigin.AutoSize = true;
			this.labelFilesOrigin.Location = new System.Drawing.Point(60, 75);
			this.labelFilesOrigin.Name = "labelFilesOrigin";
			this.labelFilesOrigin.Size = new System.Drawing.Size(78, 14);
			this.labelFilesOrigin.TabIndex = 37;
			this.labelFilesOrigin.Text = "Origin files:";
			//
			//labelFilesDestination
			//
			this.labelFilesDestination.AutoSize = true;
			this.labelFilesDestination.Location = new System.Drawing.Point(60, 190);
			this.labelFilesDestination.Name = "labelFilesDestination";
			this.labelFilesDestination.Size = new System.Drawing.Size(124, 14);
			this.labelFilesDestination.TabIndex = 40;
			this.labelFilesDestination.Text = "Destination folder:";
			//
			//textBoxbutSingleFileDestination
			//
			this.textBoxbutSingleFileDestination.Location = new System.Drawing.Point(63, 212);
			this.textBoxbutSingleFileDestination.Multiline = true;
			this.textBoxbutSingleFileDestination.Name = "textBoxbutSingleFileDestination";
			this.textBoxbutSingleFileDestination.Size = new System.Drawing.Size(693, 22);
			this.textBoxbutSingleFileDestination.TabIndex = 39;
			//
			//butSingleFileDestination
			//
			this.butSingleFileDestination.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butSingleFileDestination.Font = new System.Drawing.Font("Verdana", (float) (9.0F));
			this.butSingleFileDestination.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
			this.butSingleFileDestination.Location = new System.Drawing.Point(764, 209);
			this.butSingleFileDestination.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.butSingleFileDestination.Name = "butSingleFileDestination";
			this.butSingleFileDestination.Size = new System.Drawing.Size(110, 27);
			this.butSingleFileDestination.TabIndex = 38;
			this.butSingleFileDestination.Text = "Select folder";
			this.butSingleFileDestination.UseVisualStyleBackColor = true;
			//
			//butExecute
			//
			this.butExecute.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butExecute.Font = new System.Drawing.Font("Verdana", (float) (9.0F));
			this.butExecute.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
			this.butExecute.Location = new System.Drawing.Point(23, 431);
			this.butExecute.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
			this.butExecute.Name = "butExecute";
			this.butExecute.Size = new System.Drawing.Size(110, 27);
			this.butExecute.TabIndex = 41;
			this.butExecute.Text = "Execute";
			this.butExecute.UseVisualStyleBackColor = true;
			//
			//Panel1
			//
			this.Panel1.Controls.Add(this.radioButtonDecrypt);
			this.Panel1.Controls.Add(this.radioButtonEncrypt);
			this.Panel1.Location = new System.Drawing.Point(23, 372);
			this.Panel1.Name = "Panel1";
			this.Panel1.Size = new System.Drawing.Size(194, 43);
			this.Panel1.TabIndex = 42;
			//
			//radioButtonDecrypt
			//
			this.radioButtonDecrypt.AutoSize = true;
			this.radioButtonDecrypt.Location = new System.Drawing.Point(109, 12);
			this.radioButtonDecrypt.Name = "radioButtonDecrypt";
			this.radioButtonDecrypt.Size = new System.Drawing.Size(73, 18);
			this.radioButtonDecrypt.TabIndex = 1;
			this.radioButtonDecrypt.Text = "Decrypt";
			this.radioButtonDecrypt.UseVisualStyleBackColor = true;
			//
			//radioButtonEncrypt
			//
			this.radioButtonEncrypt.AutoSize = true;
			this.radioButtonEncrypt.Checked = true;
			this.radioButtonEncrypt.Location = new System.Drawing.Point(15, 12);
			this.radioButtonEncrypt.Name = "radioButtonEncrypt";
			this.radioButtonEncrypt.Size = new System.Drawing.Size(72, 18);
			this.radioButtonEncrypt.TabIndex = 0;
			this.radioButtonEncrypt.TabStop = true;
			this.radioButtonEncrypt.Text = "Encrypt";
			this.radioButtonEncrypt.UseVisualStyleBackColor = true;
			//
			//textBoxResult
			//
			this.textBoxResult.Location = new System.Drawing.Point(273, 303);
			this.textBoxResult.Multiline = true;
			this.textBoxResult.Name = "textBoxResult";
			this.textBoxResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBoxResult.Size = new System.Drawing.Size(601, 155);
			this.textBoxResult.TabIndex = 43;
			//
			//Form
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (8.0F), (float) (14.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(898, 470);
			this.Controls.Add(this.textBoxResult);
			this.Controls.Add(this.Panel1);
			this.Controls.Add(this.butExecute);
			this.Controls.Add(this.labelFilesDestination);
			this.Controls.Add(this.textBoxbutSingleFileDestination);
			this.Controls.Add(this.butSingleFileDestination);
			this.Controls.Add(this.labelFilesOrigin);
			this.Controls.Add(this.textBoxbutSingleFileOrigin);
			this.Controls.Add(this.labelEncryptFiles);
			this.Controls.Add(this.butSingleFileOrigin);
			this.Controls.Add(this.lineSeparator);
			this.Font = new System.Drawing.Font("Verdana", (float) (9.0F));
			this.ForeColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
			this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.Name = "Form";
			this.Text = "Encrypt Files";
			this.Panel1.ResumeLayout(false);
			this.Panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
			
		}
		internal System.Windows.Forms.Label lineSeparator;
		internal System.Windows.Forms.Button butSingleFileOrigin;
		internal System.Windows.Forms.Label labelEncryptFiles;
		internal System.Windows.Forms.TextBox textBoxbutSingleFileOrigin;
		internal System.Windows.Forms.Label labelFilesOrigin;
		internal System.Windows.Forms.Label labelFilesDestination;
		internal System.Windows.Forms.TextBox textBoxbutSingleFileDestination;
		internal System.Windows.Forms.Button butSingleFileDestination;
		internal System.Windows.Forms.Button butExecute;
		internal System.Windows.Forms.Panel Panel1;
		internal System.Windows.Forms.RadioButton radioButtonDecrypt;
		internal System.Windows.Forms.RadioButton radioButtonEncrypt;
		internal System.Windows.Forms.TextBox textBoxResult;
		
	}
	
}
