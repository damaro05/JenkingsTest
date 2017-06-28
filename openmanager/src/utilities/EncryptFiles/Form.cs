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
using DataJBC;
// End of VB project level imports

using System.IO;


namespace EncryptFiles
{
	
	
	public partial class Form
	{
		public Form()
		{
			InitializeComponent();
			
			//Added to support default instance behavour in C#
			if (defaultInstance == null)
				defaultInstance = this;
		}
		
#region Default Instance
		
		private static Form defaultInstance;
		
		/// <summary>
		/// Added by the VB.Net to C# Converter to support default instance behavour in C#
		/// </summary>
		public static Form Default
		{
			get
			{
				if (defaultInstance == null)
				{
					defaultInstance = new Form();
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
		
#region UI operations
		
		/// <summary>
		/// Select the files to encrypt/decrypt
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void butSingleFileOrigin_Click(object sender, EventArgs e)
		{
			this.textBoxbutSingleFileOrigin.Clear();
			
			OpenFileDialog openFileDialog1 = new OpenFileDialog();
			openFileDialog1.InitialDirectory = "c:\\";
			openFileDialog1.Filter = "txt files (*.txt)|*.txt|firmware files (*.S19;*.hex)|*.S19;*.hex|jbc files (*.jbc)|*.jbc|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 2;
			openFileDialog1.RestoreDirectory = true;
			openFileDialog1.Multiselect = true;
			
			if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				bool bFirst = true;
				
				foreach (var file in openFileDialog1.FileNames)
				{
					//Salto de linea
					if (bFirst)
					{
						bFirst = false;
					}
					else
					{
						this.textBoxbutSingleFileOrigin.Text += "\r\n";
					}
					
					this.textBoxbutSingleFileOrigin.Text += file;
				}
			}
		}
		
		/// <summary>
		/// Select the destination folter to encrypt/decrypt
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void butSingleFileDestination_Click(object sender, EventArgs e)
		{
			this.textBoxbutSingleFileDestination.Clear();
			
			FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
			folderBrowserDialog1.RootFolder = Environment.SpecialFolder.DesktopDirectory;
			
			if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
			{
				this.textBoxbutSingleFileDestination.Text = folderBrowserDialog1.SelectedPath;
			}
		}
		
		/// <summary>
		/// Execute the encrypt/decrypt process
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void butExecute_Click(object sender, EventArgs e)
		{
			Cursor = Cursors.WaitCursor;
			this.butExecute.Enabled = false;
			this.textBoxResult.Clear();
			
			string[] originFiles = this.textBoxbutSingleFileOrigin.Lines;
			string destPath = this.textBoxbutSingleFileDestination.Text;
			
			
			//Check destination folder
			if (!Directory.Exists(destPath))
			{
				this.textBoxResult.Text += "Destination folder does not exist!";
				
			}
			else
			{
				foreach (string originFile in originFiles)
				{
					//Check origin file
					if (!File.Exists(originFile))
					{
						this.textBoxResult.Text += "Source file does not exist : " + originFile;
					}
					else
					{
						
						bool bOk = false;
						string destFile = "";
						
						this.textBoxResult.Text += "File: " + originFile + "\r\n";
						
						//Encrypt file
						if (this.radioButtonEncrypt.Checked)
						{
							destFile = Path.Combine(destPath, Path.GetFileName(originFile) +".jbc");
							
							this.textBoxResult.Text += "Initiating encryption... ";
							bOk = EncryptFile(originFile, destFile);
							
							//Decrypt file
						}
						else
						{
							destFile = Path.Combine(destPath, Path.GetFileNameWithoutExtension(Path.GetFileName(originFile)));
							
							this.textBoxResult.Text += "Initiating decryption... ";
							bOk = DecryptFile(originFile, destFile);
						}
						
						if (bOk)
						{
							this.textBoxResult.Text += "Operation success";
						}
						else
						{
							this.textBoxResult.Text += "Operation error";
						}
						
					}
					
					this.textBoxResult.Text += "\r\n" + "\r\n";
				}
			}
			
			this.butExecute.Enabled = true;
			Cursor = Cursors.Arrow;
		}
		
#endregion
		
		
		/// <summary>
		/// Encrypt a file
		/// </summary>
		/// <param name="originFile">Origin file</param>
		/// <param name="destFile">Destination file</param>
		/// <returns>True if the operation is successful</returns>
		private bool EncryptFile(string originFile, string destFile)
		{
			bool bOk = false;
			
			try
			{
				string fileReader = (new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.ReadAllText(originFile);
				byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
				
				byte[] encrypted = RoutinesLibrary.Security.AES.EncryptStringToBytes_AES(fileReader, Key, IV);
				(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllBytes(destFile, encrypted, false);
				
				bOk = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			
			return bOk;
		}
		
		/// <summary>
		/// Decrypt a file
		/// </summary>
		/// <param name="originFile">Origin file</param>
		/// <param name="destFile">Destination file</param>
		/// <returns>True if the operation is successful</returns>
		private bool DecryptFile(string originFile, string destFile)
		{
			bool bOk = false;
			
			try
			{
				byte[] fileReader = File.ReadAllBytes(originFile);
                byte[] Key = JBC_encryption.JBC_ENCRYPTION_KEY;
                byte[] IV = JBC_encryption.JBC_ENCRYPTION_IV;
				
				string decrypted = System.Convert.ToString(RoutinesLibrary.Security.AES.DecryptStringFromBytes_AES(fileReader, Key, IV));
				(new Microsoft.VisualBasic.Devices.ServerComputer()).FileSystem.WriteAllText(destFile, decrypted, false);
				
				bOk = true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			
			return bOk;
		}
		
	}
	
}
