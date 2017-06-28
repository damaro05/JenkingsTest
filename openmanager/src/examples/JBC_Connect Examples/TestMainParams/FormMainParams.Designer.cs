// VBConversions Note: VB project level imports
using System.Collections.Generic;
using System;
using System.Linq;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Windows.Forms;
// End of VB project level imports


namespace TestMainParams
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class FormMainParams : System.Windows.Forms.Form
	{
		
		//Form reemplaza a Dispose para limpiar la lista de componentes.
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
		
		//Requerido por el Diseñador de Windows Forms
		private System.ComponentModel.Container components = null;
		
		//NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
		//Se puede modificar usando el Diseñador de Windows Forms.
		//No lo modifique con el editor de código.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMainParams));
			this.Label2 = new System.Windows.Forms.Label();
			this.Load += new System.EventHandler(Form1_Load);
			base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
			this.cbxStations = new System.Windows.Forms.ComboBox();
			this.cbxStations.SelectedIndexChanged += new System.EventHandler(this.cbxStations_SelectedIndexChanged);
			this.Label8 = new System.Windows.Forms.Label();
			this.lblName = new System.Windows.Forms.Label();
			this.txtName = new System.Windows.Forms.TextBox();
			this.txtName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtName_KeyPress);
			this.butName = new System.Windows.Forms.Button();
			this.butName.Click += new System.EventHandler(this.butName_Click);
			this.Label4 = new System.Windows.Forms.Label();
			this.Label11 = new System.Windows.Forms.Label();
			this.lblTrafoError = new System.Windows.Forms.Label();
			this.Label13 = new System.Windows.Forms.Label();
			this.lblMOSerror = new System.Windows.Forms.Label();
			this.Label15 = new System.Windows.Forms.Label();
			this.lblSW = new System.Windows.Forms.Label();
			this.Label17 = new System.Windows.Forms.Label();
			this.lblError = new System.Windows.Forms.Label();
			this.Label6 = new System.Windows.Forms.Label();
			this.lblMaxTemp = new System.Windows.Forms.Label();
			this.butMaxTemp = new System.Windows.Forms.Button();
			this.butMaxTemp.Click += new System.EventHandler(this.butMaxTemp_Click);
			this.Label10 = new System.Windows.Forms.Label();
			this.lblMinTemp = new System.Windows.Forms.Label();
			this.butMinTemp = new System.Windows.Forms.Button();
			this.butMinTemp.Click += new System.EventHandler(this.butMinTemp_Click);
			this.Label20 = new System.Windows.Forms.Label();
			this.lblUnits = new System.Windows.Forms.Label();
			this.butUnits = new System.Windows.Forms.Button();
			this.butUnits.Click += new System.EventHandler(this.butUnits_Click);
			this.cbxUnits = new System.Windows.Forms.ComboBox();
			this.cbxN2 = new System.Windows.Forms.ComboBox();
			this.Label22 = new System.Windows.Forms.Label();
			this.lblN2 = new System.Windows.Forms.Label();
			this.butN2 = new System.Windows.Forms.Button();
			this.butN2.Click += new System.EventHandler(this.butN2_Click);
			this.cbxHelp = new System.Windows.Forms.ComboBox();
			this.Label24 = new System.Windows.Forms.Label();
			this.lblHelp = new System.Windows.Forms.Label();
			this.butHelp = new System.Windows.Forms.Button();
			this.butHelp.Click += new System.EventHandler(this.butHelp_Click);
			this.cbxBeep = new System.Windows.Forms.ComboBox();
			this.Label28 = new System.Windows.Forms.Label();
			this.lblBeep = new System.Windows.Forms.Label();
			this.butBeep = new System.Windows.Forms.Button();
			this.butBeep.Click += new System.EventHandler(this.butBeep_Click);
			this.gbxStationParams = new System.Windows.Forms.GroupBox();
			this.txtMinTemp = new System.Windows.Forms.MaskedTextBox();
			this.txtMinTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinTemp_KeyPress);
			this.txtMaxTemp = new System.Windows.Forms.MaskedTextBox();
			this.txtMaxTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxTemp_KeyPress);
			this.lblModel = new System.Windows.Forms.Label();
			this.Label5 = new System.Windows.Forms.Label();
			this.gbxPort1 = new System.Windows.Forms.GroupBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.cbxPort1Extractor = new System.Windows.Forms.CheckBox();
			this.cbxPort1Hibernation = new System.Windows.Forms.CheckBox();
			this.cbxPort1Sleep = new System.Windows.Forms.CheckBox();
			this.lblPort1Error = new System.Windows.Forms.Label();
			this.Label19 = new System.Windows.Forms.Label();
			this.txtPort1SelecTemp = new System.Windows.Forms.MaskedTextBox();
			this.txtPort1SelecTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort1SelecTemp_KeyPress);
			this.butPort1SelecTemp = new System.Windows.Forms.Button();
			this.butPort1SelecTemp.Click += new System.EventHandler(this.butPort1SelecTemp_Click);
			this.lblPort1SelecTemp = new System.Windows.Forms.Label();
			this.Label16 = new System.Windows.Forms.Label();
			this.Label9 = new System.Windows.Forms.Label();
			this.lblPort1ActualTemp = new System.Windows.Forms.Label();
			this.Label3 = new System.Windows.Forms.Label();
			this.lblPort1Tool = new System.Windows.Forms.Label();
			this.lblTrafo = new System.Windows.Forms.Label();
			this.gbxPort3 = new System.Windows.Forms.GroupBox();
			this.Label14 = new System.Windows.Forms.Label();
			this.cbxPort3Extractor = new System.Windows.Forms.CheckBox();
			this.cbxPort3Hibernation = new System.Windows.Forms.CheckBox();
			this.cbxPort3Sleep = new System.Windows.Forms.CheckBox();
			this.lblPort3Error = new System.Windows.Forms.Label();
			this.Label12 = new System.Windows.Forms.Label();
			this.txtPort3SelecTemp = new System.Windows.Forms.MaskedTextBox();
			this.txtPort3SelecTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort3SelecTemp_KeyPress);
			this.butPort3SelecTemp = new System.Windows.Forms.Button();
			this.butPort3SelecTemp.Click += new System.EventHandler(this.butPort3SelecTemp_Click);
			this.lblPort3SelecTemp = new System.Windows.Forms.Label();
			this.Label18 = new System.Windows.Forms.Label();
			this.Label21 = new System.Windows.Forms.Label();
			this.lblPort3ActualTemp = new System.Windows.Forms.Label();
			this.Label25 = new System.Windows.Forms.Label();
			this.lblPort3Tool = new System.Windows.Forms.Label();
			this.gbxPort2 = new System.Windows.Forms.GroupBox();
			this.Label7 = new System.Windows.Forms.Label();
			this.cbxPort2Extractor = new System.Windows.Forms.CheckBox();
			this.cbxPort2Hibernation = new System.Windows.Forms.CheckBox();
			this.cbxPort2Sleep = new System.Windows.Forms.CheckBox();
			this.lblPort2Error = new System.Windows.Forms.Label();
			this.Label30 = new System.Windows.Forms.Label();
			this.txtPort2SelecTemp = new System.Windows.Forms.MaskedTextBox();
			this.txtPort2SelecTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort2SelecTemp_KeyPress);
			this.butPort2SelecTemp = new System.Windows.Forms.Button();
			this.butPort2SelecTemp.Click += new System.EventHandler(this.butPort2SelecTemp_Click);
			this.lblPort2SelecTemp = new System.Windows.Forms.Label();
			this.Label32 = new System.Windows.Forms.Label();
			this.Label33 = new System.Windows.Forms.Label();
			this.lblPort2ActualTemp = new System.Windows.Forms.Label();
			this.Label35 = new System.Windows.Forms.Label();
			this.lblPort2Tool = new System.Windows.Forms.Label();
			this.gbxPort4 = new System.Windows.Forms.GroupBox();
			this.Label23 = new System.Windows.Forms.Label();
			this.cbxPort4Extractor = new System.Windows.Forms.CheckBox();
			this.cbxPort4Hibernation = new System.Windows.Forms.CheckBox();
			this.cbxPort4Sleep = new System.Windows.Forms.CheckBox();
			this.lblPort4Error = new System.Windows.Forms.Label();
			this.Label38 = new System.Windows.Forms.Label();
			this.txtPort4SelecTemp = new System.Windows.Forms.MaskedTextBox();
			this.txtPort4SelecTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPort4SelecTemp_KeyPress);
			this.butPort4SelecTemp = new System.Windows.Forms.Button();
			this.butPort4SelecTemp.Click += new System.EventHandler(this.butPort4SelecTemp_Click);
			this.lblPort4SelecTemp = new System.Windows.Forms.Label();
			this.Label40 = new System.Windows.Forms.Label();
			this.Label41 = new System.Windows.Forms.Label();
			this.lblPort4ActualTemp = new System.Windows.Forms.Label();
			this.Label43 = new System.Windows.Forms.Label();
			this.lblPort4Tool = new System.Windows.Forms.Label();
			this.gbxStationData = new System.Windows.Forms.GroupBox();
			this.gbxStationParams.SuspendLayout();
			this.gbxPort1.SuspendLayout();
			this.gbxPort3.SuspendLayout();
			this.gbxPort2.SuspendLayout();
			this.gbxPort4.SuspendLayout();
			this.gbxStationData.SuspendLayout();
			this.SuspendLayout();
			//
			//Label2
			//
			this.Label2.AutoSize = true;
			this.Label2.Location = new System.Drawing.Point(570, 28);
			this.Label2.Name = "Label2";
			this.Label2.Size = new System.Drawing.Size(74, 13);
			this.Label2.TabIndex = 5;
			this.Label2.Text = "Select station:";
			//
			//cbxStations
			//
			this.cbxStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxStations.FormattingEnabled = true;
			this.cbxStations.Items.AddRange(new object[] {"0 - COM10 - StationName"});
			this.cbxStations.Location = new System.Drawing.Point(574, 44);
			this.cbxStations.Name = "cbxStations";
			this.cbxStations.Size = new System.Drawing.Size(65, 21);
			this.cbxStations.TabIndex = 4;
			//
			//Label8
			//
			this.Label8.Location = new System.Drawing.Point(6, 16);
			this.Label8.Name = "Label8";
			this.Label8.Size = new System.Drawing.Size(60, 20);
			this.Label8.TabIndex = 8;
			this.Label8.Text = "Name:";
			this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblName
			//
			this.lblName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblName.Location = new System.Drawing.Point(72, 16);
			this.lblName.Name = "lblName";
			this.lblName.Size = new System.Drawing.Size(100, 20);
			this.lblName.TabIndex = 9;
			this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//txtName
			//
			this.txtName.Location = new System.Drawing.Point(215, 17);
			this.txtName.Name = "txtName";
			this.txtName.Size = new System.Drawing.Size(100, 20);
			this.txtName.TabIndex = 10;
			this.txtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			//
			//butName
			//
			this.butName.Location = new System.Drawing.Point(178, 15);
			this.butName.Name = "butName";
			this.butName.Size = new System.Drawing.Size(31, 23);
			this.butName.TabIndex = 11;
			this.butName.Text = "<=";
			this.butName.UseVisualStyleBackColor = true;
			//
			//Label4
			//
			this.Label4.Location = new System.Drawing.Point(178, 16);
			this.Label4.Name = "Label4";
			this.Label4.Size = new System.Drawing.Size(66, 20);
			this.Label4.TabIndex = 12;
			this.Label4.Text = "Trafo temp:";
			this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//Label11
			//
			this.Label11.Location = new System.Drawing.Point(350, 16);
			this.Label11.Name = "Label11";
			this.Label11.Size = new System.Drawing.Size(60, 20);
			this.Label11.TabIndex = 14;
			this.Label11.Text = "Trafo error:";
			this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblTrafoError
			//
			this.lblTrafoError.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTrafoError.Location = new System.Drawing.Point(416, 16);
			this.lblTrafoError.Name = "lblTrafoError";
			this.lblTrafoError.Size = new System.Drawing.Size(100, 20);
			this.lblTrafoError.TabIndex = 15;
			this.lblTrafoError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label13
			//
			this.Label13.Location = new System.Drawing.Point(350, 36);
			this.Label13.Name = "Label13";
			this.Label13.Size = new System.Drawing.Size(60, 20);
			this.Label13.TabIndex = 16;
			this.Label13.Text = "MOS error:";
			this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblMOSerror
			//
			this.lblMOSerror.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblMOSerror.Location = new System.Drawing.Point(416, 36);
			this.lblMOSerror.Name = "lblMOSerror";
			this.lblMOSerror.Size = new System.Drawing.Size(100, 20);
			this.lblMOSerror.TabIndex = 17;
			this.lblMOSerror.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label15
			//
			this.Label15.Location = new System.Drawing.Point(6, 36);
			this.Label15.Name = "Label15";
			this.Label15.Size = new System.Drawing.Size(60, 20);
			this.Label15.TabIndex = 18;
			this.Label15.Text = "SW:";
			this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblSW
			//
			this.lblSW.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblSW.Location = new System.Drawing.Point(72, 36);
			this.lblSW.Name = "lblSW";
			this.lblSW.Size = new System.Drawing.Size(100, 20);
			this.lblSW.TabIndex = 19;
			this.lblSW.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label17
			//
			this.Label17.Location = new System.Drawing.Point(178, 36);
			this.Label17.Name = "Label17";
			this.Label17.Size = new System.Drawing.Size(60, 20);
			this.Label17.TabIndex = 20;
			this.Label17.Text = "Error:";
			this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblError
			//
			this.lblError.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblError.Location = new System.Drawing.Point(244, 36);
			this.lblError.Name = "lblError";
			this.lblError.Size = new System.Drawing.Size(100, 20);
			this.lblError.TabIndex = 21;
			this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label6
			//
			this.Label6.Location = new System.Drawing.Point(6, 43);
			this.Label6.Name = "Label6";
			this.Label6.Size = new System.Drawing.Size(60, 20);
			this.Label6.TabIndex = 26;
			this.Label6.Text = "Max temp:";
			this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblMaxTemp
			//
			this.lblMaxTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblMaxTemp.Location = new System.Drawing.Point(72, 43);
			this.lblMaxTemp.Name = "lblMaxTemp";
			this.lblMaxTemp.Size = new System.Drawing.Size(100, 20);
			this.lblMaxTemp.TabIndex = 27;
			this.lblMaxTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//butMaxTemp
			//
			this.butMaxTemp.Location = new System.Drawing.Point(178, 42);
			this.butMaxTemp.Name = "butMaxTemp";
			this.butMaxTemp.Size = new System.Drawing.Size(31, 23);
			this.butMaxTemp.TabIndex = 29;
			this.butMaxTemp.Text = "<=";
			this.butMaxTemp.UseVisualStyleBackColor = true;
			//
			//Label10
			//
			this.Label10.Location = new System.Drawing.Point(6, 69);
			this.Label10.Name = "Label10";
			this.Label10.Size = new System.Drawing.Size(60, 20);
			this.Label10.TabIndex = 30;
			this.Label10.Text = "Min temp:";
			this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblMinTemp
			//
			this.lblMinTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblMinTemp.Location = new System.Drawing.Point(72, 69);
			this.lblMinTemp.Name = "lblMinTemp";
			this.lblMinTemp.Size = new System.Drawing.Size(100, 20);
			this.lblMinTemp.TabIndex = 31;
			this.lblMinTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//butMinTemp
			//
			this.butMinTemp.Location = new System.Drawing.Point(178, 68);
			this.butMinTemp.Name = "butMinTemp";
			this.butMinTemp.Size = new System.Drawing.Size(31, 23);
			this.butMinTemp.TabIndex = 33;
			this.butMinTemp.Text = "<=";
			this.butMinTemp.UseVisualStyleBackColor = true;
			//
			//Label20
			//
			this.Label20.Location = new System.Drawing.Point(338, 16);
			this.Label20.Name = "Label20";
			this.Label20.Size = new System.Drawing.Size(60, 20);
			this.Label20.TabIndex = 34;
			this.Label20.Text = "Units:";
			this.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblUnits
			//
			this.lblUnits.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblUnits.Location = new System.Drawing.Point(404, 16);
			this.lblUnits.Name = "lblUnits";
			this.lblUnits.Size = new System.Drawing.Size(100, 20);
			this.lblUnits.TabIndex = 35;
			this.lblUnits.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//butUnits
			//
			this.butUnits.Location = new System.Drawing.Point(510, 15);
			this.butUnits.Name = "butUnits";
			this.butUnits.Size = new System.Drawing.Size(31, 23);
			this.butUnits.TabIndex = 37;
			this.butUnits.Text = "<=";
			this.butUnits.UseVisualStyleBackColor = true;
			//
			//cbxUnits
			//
			this.cbxUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxUnits.FormattingEnabled = true;
			this.cbxUnits.Items.AddRange(new object[] {"CELSIUS", "FAHRENHEIT"});
			this.cbxUnits.Location = new System.Drawing.Point(547, 17);
			this.cbxUnits.Name = "cbxUnits";
			this.cbxUnits.Size = new System.Drawing.Size(100, 21);
			this.cbxUnits.TabIndex = 38;
			//
			//cbxN2
			//
			this.cbxN2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxN2.FormattingEnabled = true;
			this.cbxN2.Items.AddRange(new object[] {"ON", "OFF"});
			this.cbxN2.Location = new System.Drawing.Point(547, 42);
			this.cbxN2.Name = "cbxN2";
			this.cbxN2.Size = new System.Drawing.Size(100, 21);
			this.cbxN2.TabIndex = 42;
			//
			//Label22
			//
			this.Label22.Location = new System.Drawing.Point(338, 41);
			this.Label22.Name = "Label22";
			this.Label22.Size = new System.Drawing.Size(60, 20);
			this.Label22.TabIndex = 39;
			this.Label22.Text = "N2 mode:";
			this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblN2
			//
			this.lblN2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblN2.Location = new System.Drawing.Point(404, 41);
			this.lblN2.Name = "lblN2";
			this.lblN2.Size = new System.Drawing.Size(100, 20);
			this.lblN2.TabIndex = 40;
			this.lblN2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//butN2
			//
			this.butN2.Location = new System.Drawing.Point(510, 40);
			this.butN2.Name = "butN2";
			this.butN2.Size = new System.Drawing.Size(31, 23);
			this.butN2.TabIndex = 41;
			this.butN2.Text = "<=";
			this.butN2.UseVisualStyleBackColor = true;
			//
			//cbxHelp
			//
			this.cbxHelp.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxHelp.FormattingEnabled = true;
			this.cbxHelp.Items.AddRange(new object[] {"ON", "OFF"});
			this.cbxHelp.Location = new System.Drawing.Point(547, 69);
			this.cbxHelp.Name = "cbxHelp";
			this.cbxHelp.Size = new System.Drawing.Size(100, 21);
			this.cbxHelp.TabIndex = 46;
			//
			//Label24
			//
			this.Label24.Location = new System.Drawing.Point(338, 68);
			this.Label24.Name = "Label24";
			this.Label24.Size = new System.Drawing.Size(60, 20);
			this.Label24.TabIndex = 43;
			this.Label24.Text = "Help text:";
			this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblHelp
			//
			this.lblHelp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblHelp.Location = new System.Drawing.Point(404, 68);
			this.lblHelp.Name = "lblHelp";
			this.lblHelp.Size = new System.Drawing.Size(100, 20);
			this.lblHelp.TabIndex = 44;
			this.lblHelp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//butHelp
			//
			this.butHelp.Location = new System.Drawing.Point(510, 67);
			this.butHelp.Name = "butHelp";
			this.butHelp.Size = new System.Drawing.Size(31, 23);
			this.butHelp.TabIndex = 45;
			this.butHelp.Text = "<=";
			this.butHelp.UseVisualStyleBackColor = true;
			//
			//cbxBeep
			//
			this.cbxBeep.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbxBeep.FormattingEnabled = true;
			this.cbxBeep.Items.AddRange(new object[] {"ON", "OFF"});
			this.cbxBeep.Location = new System.Drawing.Point(547, 95);
			this.cbxBeep.Name = "cbxBeep";
			this.cbxBeep.Size = new System.Drawing.Size(100, 21);
			this.cbxBeep.TabIndex = 54;
			//
			//Label28
			//
			this.Label28.Location = new System.Drawing.Point(338, 94);
			this.Label28.Name = "Label28";
			this.Label28.Size = new System.Drawing.Size(60, 20);
			this.Label28.TabIndex = 51;
			this.Label28.Text = "Beep:";
			this.Label28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblBeep
			//
			this.lblBeep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblBeep.Location = new System.Drawing.Point(404, 94);
			this.lblBeep.Name = "lblBeep";
			this.lblBeep.Size = new System.Drawing.Size(100, 20);
			this.lblBeep.TabIndex = 52;
			this.lblBeep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//butBeep
			//
			this.butBeep.Location = new System.Drawing.Point(510, 93);
			this.butBeep.Name = "butBeep";
			this.butBeep.Size = new System.Drawing.Size(31, 23);
			this.butBeep.TabIndex = 53;
			this.butBeep.Text = "<=";
			this.butBeep.UseVisualStyleBackColor = true;
			//
			//gbxStationParams
			//
			this.gbxStationParams.Controls.Add(this.txtMinTemp);
			this.gbxStationParams.Controls.Add(this.txtMaxTemp);
			this.gbxStationParams.Controls.Add(this.cbxBeep);
			this.gbxStationParams.Controls.Add(this.Label28);
			this.gbxStationParams.Controls.Add(this.lblBeep);
			this.gbxStationParams.Controls.Add(this.butBeep);
			this.gbxStationParams.Controls.Add(this.cbxHelp);
			this.gbxStationParams.Controls.Add(this.Label24);
			this.gbxStationParams.Controls.Add(this.Label8);
			this.gbxStationParams.Controls.Add(this.lblHelp);
			this.gbxStationParams.Controls.Add(this.butName);
			this.gbxStationParams.Controls.Add(this.butHelp);
			this.gbxStationParams.Controls.Add(this.txtName);
			this.gbxStationParams.Controls.Add(this.cbxN2);
			this.gbxStationParams.Controls.Add(this.lblName);
			this.gbxStationParams.Controls.Add(this.Label22);
			this.gbxStationParams.Controls.Add(this.lblN2);
			this.gbxStationParams.Controls.Add(this.butN2);
			this.gbxStationParams.Controls.Add(this.cbxUnits);
			this.gbxStationParams.Controls.Add(this.Label20);
			this.gbxStationParams.Controls.Add(this.butMaxTemp);
			this.gbxStationParams.Controls.Add(this.lblUnits);
			this.gbxStationParams.Controls.Add(this.butUnits);
			this.gbxStationParams.Controls.Add(this.lblMaxTemp);
			this.gbxStationParams.Controls.Add(this.Label10);
			this.gbxStationParams.Controls.Add(this.Label6);
			this.gbxStationParams.Controls.Add(this.lblMinTemp);
			this.gbxStationParams.Controls.Add(this.butMinTemp);
			this.gbxStationParams.Location = new System.Drawing.Point(12, 82);
			this.gbxStationParams.Name = "gbxStationParams";
			this.gbxStationParams.Size = new System.Drawing.Size(656, 126);
			this.gbxStationParams.TabIndex = 55;
			this.gbxStationParams.TabStop = false;
			this.gbxStationParams.Text = "Station Params";
			//
			//txtMinTemp
			//
			this.txtMinTemp.Location = new System.Drawing.Point(215, 70);
			this.txtMinTemp.Mask = "999";
			this.txtMinTemp.Name = "txtMinTemp";
			this.txtMinTemp.Size = new System.Drawing.Size(100, 20);
			this.txtMinTemp.TabIndex = 56;
			this.txtMinTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			//
			//txtMaxTemp
			//
			this.txtMaxTemp.Location = new System.Drawing.Point(215, 44);
			this.txtMaxTemp.Mask = "999";
			this.txtMaxTemp.Name = "txtMaxTemp";
			this.txtMaxTemp.Size = new System.Drawing.Size(100, 20);
			this.txtMaxTemp.TabIndex = 55;
			this.txtMaxTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			//
			//lblModel
			//
			this.lblModel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblModel.Location = new System.Drawing.Point(72, 16);
			this.lblModel.Name = "lblModel";
			this.lblModel.Size = new System.Drawing.Size(100, 20);
			this.lblModel.TabIndex = 59;
			this.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label5
			//
			this.Label5.Location = new System.Drawing.Point(6, 16);
			this.Label5.Name = "Label5";
			this.Label5.Size = new System.Drawing.Size(60, 20);
			this.Label5.TabIndex = 58;
			this.Label5.Text = "Model:";
			this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//gbxPort1
			//
			this.gbxPort1.Controls.Add(this.Label1);
			this.gbxPort1.Controls.Add(this.cbxPort1Extractor);
			this.gbxPort1.Controls.Add(this.cbxPort1Hibernation);
			this.gbxPort1.Controls.Add(this.cbxPort1Sleep);
			this.gbxPort1.Controls.Add(this.lblPort1Error);
			this.gbxPort1.Controls.Add(this.Label19);
			this.gbxPort1.Controls.Add(this.txtPort1SelecTemp);
			this.gbxPort1.Controls.Add(this.butPort1SelecTemp);
			this.gbxPort1.Controls.Add(this.lblPort1SelecTemp);
			this.gbxPort1.Controls.Add(this.Label16);
			this.gbxPort1.Controls.Add(this.Label9);
			this.gbxPort1.Controls.Add(this.lblPort1ActualTemp);
			this.gbxPort1.Controls.Add(this.Label3);
			this.gbxPort1.Controls.Add(this.lblPort1Tool);
			this.gbxPort1.Location = new System.Drawing.Point(12, 214);
			this.gbxPort1.Name = "gbxPort1";
			this.gbxPort1.Size = new System.Drawing.Size(326, 119);
			this.gbxPort1.TabIndex = 56;
			this.gbxPort1.TabStop = false;
			this.gbxPort1.Text = "Port 1";
			//
			//Label1
			//
			this.Label1.Location = new System.Drawing.Point(6, 96);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(60, 20);
			this.Label1.TabIndex = 65;
			this.Label1.Text = "Status:";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//cbxPort1Extractor
			//
			this.cbxPort1Extractor.AutoCheck = false;
			this.cbxPort1Extractor.AutoSize = true;
			this.cbxPort1Extractor.Location = new System.Drawing.Point(217, 99);
			this.cbxPort1Extractor.Name = "cbxPort1Extractor";
			this.cbxPort1Extractor.Size = new System.Drawing.Size(68, 17);
			this.cbxPort1Extractor.TabIndex = 64;
			this.cbxPort1Extractor.Text = "Extractor";
			this.cbxPort1Extractor.UseVisualStyleBackColor = true;
			//
			//cbxPort1Hibernation
			//
			this.cbxPort1Hibernation.AutoCheck = false;
			this.cbxPort1Hibernation.AutoSize = true;
			this.cbxPort1Hibernation.Location = new System.Drawing.Point(131, 99);
			this.cbxPort1Hibernation.Name = "cbxPort1Hibernation";
			this.cbxPort1Hibernation.Size = new System.Drawing.Size(80, 17);
			this.cbxPort1Hibernation.TabIndex = 63;
			this.cbxPort1Hibernation.Text = "Hibernation";
			this.cbxPort1Hibernation.UseVisualStyleBackColor = true;
			//
			//cbxPort1Sleep
			//
			this.cbxPort1Sleep.AutoCheck = false;
			this.cbxPort1Sleep.AutoSize = true;
			this.cbxPort1Sleep.Location = new System.Drawing.Point(72, 99);
			this.cbxPort1Sleep.Name = "cbxPort1Sleep";
			this.cbxPort1Sleep.Size = new System.Drawing.Size(53, 17);
			this.cbxPort1Sleep.TabIndex = 62;
			this.cbxPort1Sleep.Text = "Sleep";
			this.cbxPort1Sleep.UseVisualStyleBackColor = true;
			//
			//lblPort1Error
			//
			this.lblPort1Error.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort1Error.Location = new System.Drawing.Point(72, 76);
			this.lblPort1Error.Name = "lblPort1Error";
			this.lblPort1Error.Size = new System.Drawing.Size(100, 20);
			this.lblPort1Error.TabIndex = 61;
			this.lblPort1Error.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label19
			//
			this.Label19.Location = new System.Drawing.Point(6, 76);
			this.Label19.Name = "Label19";
			this.Label19.Size = new System.Drawing.Size(60, 20);
			this.Label19.TabIndex = 60;
			this.Label19.Text = "Error:";
			this.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//txtPort1SelecTemp
			//
			this.txtPort1SelecTemp.Location = new System.Drawing.Point(215, 57);
			this.txtPort1SelecTemp.Mask = "999";
			this.txtPort1SelecTemp.Name = "txtPort1SelecTemp";
			this.txtPort1SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.txtPort1SelecTemp.TabIndex = 59;
			this.txtPort1SelecTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			//
			//butPort1SelecTemp
			//
			this.butPort1SelecTemp.Location = new System.Drawing.Point(178, 55);
			this.butPort1SelecTemp.Name = "butPort1SelecTemp";
			this.butPort1SelecTemp.Size = new System.Drawing.Size(31, 23);
			this.butPort1SelecTemp.TabIndex = 58;
			this.butPort1SelecTemp.Text = "<=";
			this.butPort1SelecTemp.UseVisualStyleBackColor = true;
			//
			//lblPort1SelecTemp
			//
			this.lblPort1SelecTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort1SelecTemp.Location = new System.Drawing.Point(72, 56);
			this.lblPort1SelecTemp.Name = "lblPort1SelecTemp";
			this.lblPort1SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort1SelecTemp.TabIndex = 57;
			this.lblPort1SelecTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label16
			//
			this.Label16.Location = new System.Drawing.Point(6, 56);
			this.Label16.Name = "Label16";
			this.Label16.Size = new System.Drawing.Size(70, 20);
			this.Label16.TabIndex = 56;
			this.Label16.Text = "Selec temp:";
			this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//Label9
			//
			this.Label9.Location = new System.Drawing.Point(6, 36);
			this.Label9.Name = "Label9";
			this.Label9.Size = new System.Drawing.Size(60, 20);
			this.Label9.TabIndex = 18;
			this.Label9.Text = "Act temp:";
			this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort1ActualTemp
			//
			this.lblPort1ActualTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort1ActualTemp.Location = new System.Drawing.Point(72, 36);
			this.lblPort1ActualTemp.Name = "lblPort1ActualTemp";
			this.lblPort1ActualTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort1ActualTemp.TabIndex = 19;
			this.lblPort1ActualTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label3
			//
			this.Label3.Location = new System.Drawing.Point(6, 16);
			this.Label3.Name = "Label3";
			this.Label3.Size = new System.Drawing.Size(60, 20);
			this.Label3.TabIndex = 16;
			this.Label3.Text = "Tool:";
			this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort1Tool
			//
			this.lblPort1Tool.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort1Tool.Location = new System.Drawing.Point(72, 16);
			this.lblPort1Tool.Name = "lblPort1Tool";
			this.lblPort1Tool.Size = new System.Drawing.Size(100, 20);
			this.lblPort1Tool.TabIndex = 17;
			this.lblPort1Tool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//lblTrafo
			//
			this.lblTrafo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblTrafo.Location = new System.Drawing.Point(244, 16);
			this.lblTrafo.Name = "lblTrafo";
			this.lblTrafo.Size = new System.Drawing.Size(100, 20);
			this.lblTrafo.TabIndex = 13;
			this.lblTrafo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//gbxPort3
			//
			this.gbxPort3.Controls.Add(this.Label14);
			this.gbxPort3.Controls.Add(this.cbxPort3Extractor);
			this.gbxPort3.Controls.Add(this.cbxPort3Hibernation);
			this.gbxPort3.Controls.Add(this.cbxPort3Sleep);
			this.gbxPort3.Controls.Add(this.lblPort3Error);
			this.gbxPort3.Controls.Add(this.Label12);
			this.gbxPort3.Controls.Add(this.txtPort3SelecTemp);
			this.gbxPort3.Controls.Add(this.butPort3SelecTemp);
			this.gbxPort3.Controls.Add(this.lblPort3SelecTemp);
			this.gbxPort3.Controls.Add(this.Label18);
			this.gbxPort3.Controls.Add(this.Label21);
			this.gbxPort3.Controls.Add(this.lblPort3ActualTemp);
			this.gbxPort3.Controls.Add(this.Label25);
			this.gbxPort3.Controls.Add(this.lblPort3Tool);
			this.gbxPort3.Location = new System.Drawing.Point(12, 339);
			this.gbxPort3.Name = "gbxPort3";
			this.gbxPort3.Size = new System.Drawing.Size(326, 119);
			this.gbxPort3.TabIndex = 57;
			this.gbxPort3.TabStop = false;
			this.gbxPort3.Text = "Port 3";
			//
			//Label14
			//
			this.Label14.Location = new System.Drawing.Point(6, 96);
			this.Label14.Name = "Label14";
			this.Label14.Size = new System.Drawing.Size(60, 20);
			this.Label14.TabIndex = 69;
			this.Label14.Text = "Status:";
			this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//cbxPort3Extractor
			//
			this.cbxPort3Extractor.AutoCheck = false;
			this.cbxPort3Extractor.AutoSize = true;
			this.cbxPort3Extractor.Location = new System.Drawing.Point(217, 99);
			this.cbxPort3Extractor.Name = "cbxPort3Extractor";
			this.cbxPort3Extractor.Size = new System.Drawing.Size(68, 17);
			this.cbxPort3Extractor.TabIndex = 68;
			this.cbxPort3Extractor.Text = "Extractor";
			this.cbxPort3Extractor.UseVisualStyleBackColor = true;
			//
			//cbxPort3Hibernation
			//
			this.cbxPort3Hibernation.AutoCheck = false;
			this.cbxPort3Hibernation.AutoSize = true;
			this.cbxPort3Hibernation.Location = new System.Drawing.Point(131, 99);
			this.cbxPort3Hibernation.Name = "cbxPort3Hibernation";
			this.cbxPort3Hibernation.Size = new System.Drawing.Size(80, 17);
			this.cbxPort3Hibernation.TabIndex = 67;
			this.cbxPort3Hibernation.Text = "Hibernation";
			this.cbxPort3Hibernation.UseVisualStyleBackColor = true;
			//
			//cbxPort3Sleep
			//
			this.cbxPort3Sleep.AutoCheck = false;
			this.cbxPort3Sleep.AutoSize = true;
			this.cbxPort3Sleep.Location = new System.Drawing.Point(72, 99);
			this.cbxPort3Sleep.Name = "cbxPort3Sleep";
			this.cbxPort3Sleep.Size = new System.Drawing.Size(53, 17);
			this.cbxPort3Sleep.TabIndex = 66;
			this.cbxPort3Sleep.Text = "Sleep";
			this.cbxPort3Sleep.UseVisualStyleBackColor = true;
			//
			//lblPort3Error
			//
			this.lblPort3Error.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort3Error.Location = new System.Drawing.Point(72, 76);
			this.lblPort3Error.Name = "lblPort3Error";
			this.lblPort3Error.Size = new System.Drawing.Size(100, 20);
			this.lblPort3Error.TabIndex = 61;
			this.lblPort3Error.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label12
			//
			this.Label12.Location = new System.Drawing.Point(6, 76);
			this.Label12.Name = "Label12";
			this.Label12.Size = new System.Drawing.Size(60, 20);
			this.Label12.TabIndex = 60;
			this.Label12.Text = "Error:";
			this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//txtPort3SelecTemp
			//
			this.txtPort3SelecTemp.Location = new System.Drawing.Point(215, 57);
			this.txtPort3SelecTemp.Mask = "999";
			this.txtPort3SelecTemp.Name = "txtPort3SelecTemp";
			this.txtPort3SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.txtPort3SelecTemp.TabIndex = 59;
			this.txtPort3SelecTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			//
			//butPort3SelecTemp
			//
			this.butPort3SelecTemp.Location = new System.Drawing.Point(178, 55);
			this.butPort3SelecTemp.Name = "butPort3SelecTemp";
			this.butPort3SelecTemp.Size = new System.Drawing.Size(31, 23);
			this.butPort3SelecTemp.TabIndex = 58;
			this.butPort3SelecTemp.Text = "<=";
			this.butPort3SelecTemp.UseVisualStyleBackColor = true;
			//
			//lblPort3SelecTemp
			//
			this.lblPort3SelecTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort3SelecTemp.Location = new System.Drawing.Point(72, 56);
			this.lblPort3SelecTemp.Name = "lblPort3SelecTemp";
			this.lblPort3SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort3SelecTemp.TabIndex = 57;
			this.lblPort3SelecTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label18
			//
			this.Label18.Location = new System.Drawing.Point(6, 56);
			this.Label18.Name = "Label18";
			this.Label18.Size = new System.Drawing.Size(70, 20);
			this.Label18.TabIndex = 56;
			this.Label18.Text = "Selec temp:";
			this.Label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//Label21
			//
			this.Label21.Location = new System.Drawing.Point(6, 36);
			this.Label21.Name = "Label21";
			this.Label21.Size = new System.Drawing.Size(60, 20);
			this.Label21.TabIndex = 18;
			this.Label21.Text = "Act temp:";
			this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort3ActualTemp
			//
			this.lblPort3ActualTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort3ActualTemp.Location = new System.Drawing.Point(72, 36);
			this.lblPort3ActualTemp.Name = "lblPort3ActualTemp";
			this.lblPort3ActualTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort3ActualTemp.TabIndex = 19;
			this.lblPort3ActualTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label25
			//
			this.Label25.Location = new System.Drawing.Point(6, 16);
			this.Label25.Name = "Label25";
			this.Label25.Size = new System.Drawing.Size(60, 20);
			this.Label25.TabIndex = 16;
			this.Label25.Text = "Tool:";
			this.Label25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort3Tool
			//
			this.lblPort3Tool.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort3Tool.Location = new System.Drawing.Point(72, 16);
			this.lblPort3Tool.Name = "lblPort3Tool";
			this.lblPort3Tool.Size = new System.Drawing.Size(100, 20);
			this.lblPort3Tool.TabIndex = 17;
			this.lblPort3Tool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//gbxPort2
			//
			this.gbxPort2.Controls.Add(this.Label7);
			this.gbxPort2.Controls.Add(this.cbxPort2Extractor);
			this.gbxPort2.Controls.Add(this.cbxPort2Hibernation);
			this.gbxPort2.Controls.Add(this.cbxPort2Sleep);
			this.gbxPort2.Controls.Add(this.lblPort2Error);
			this.gbxPort2.Controls.Add(this.Label30);
			this.gbxPort2.Controls.Add(this.txtPort2SelecTemp);
			this.gbxPort2.Controls.Add(this.butPort2SelecTemp);
			this.gbxPort2.Controls.Add(this.lblPort2SelecTemp);
			this.gbxPort2.Controls.Add(this.Label32);
			this.gbxPort2.Controls.Add(this.Label33);
			this.gbxPort2.Controls.Add(this.lblPort2ActualTemp);
			this.gbxPort2.Controls.Add(this.Label35);
			this.gbxPort2.Controls.Add(this.lblPort2Tool);
			this.gbxPort2.Location = new System.Drawing.Point(344, 214);
			this.gbxPort2.Name = "gbxPort2";
			this.gbxPort2.Size = new System.Drawing.Size(324, 119);
			this.gbxPort2.TabIndex = 58;
			this.gbxPort2.TabStop = false;
			this.gbxPort2.Text = "Port 2";
			//
			//Label7
			//
			this.Label7.Location = new System.Drawing.Point(6, 96);
			this.Label7.Name = "Label7";
			this.Label7.Size = new System.Drawing.Size(60, 20);
			this.Label7.TabIndex = 69;
			this.Label7.Text = "Status:";
			this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//cbxPort2Extractor
			//
			this.cbxPort2Extractor.AutoCheck = false;
			this.cbxPort2Extractor.AutoSize = true;
			this.cbxPort2Extractor.Location = new System.Drawing.Point(217, 99);
			this.cbxPort2Extractor.Name = "cbxPort2Extractor";
			this.cbxPort2Extractor.Size = new System.Drawing.Size(68, 17);
			this.cbxPort2Extractor.TabIndex = 68;
			this.cbxPort2Extractor.Text = "Extractor";
			this.cbxPort2Extractor.UseVisualStyleBackColor = true;
			//
			//cbxPort2Hibernation
			//
			this.cbxPort2Hibernation.AutoCheck = false;
			this.cbxPort2Hibernation.AutoSize = true;
			this.cbxPort2Hibernation.Location = new System.Drawing.Point(131, 99);
			this.cbxPort2Hibernation.Name = "cbxPort2Hibernation";
			this.cbxPort2Hibernation.Size = new System.Drawing.Size(80, 17);
			this.cbxPort2Hibernation.TabIndex = 67;
			this.cbxPort2Hibernation.Text = "Hibernation";
			this.cbxPort2Hibernation.UseVisualStyleBackColor = true;
			//
			//cbxPort2Sleep
			//
			this.cbxPort2Sleep.AutoCheck = false;
			this.cbxPort2Sleep.AutoSize = true;
			this.cbxPort2Sleep.Location = new System.Drawing.Point(72, 99);
			this.cbxPort2Sleep.Name = "cbxPort2Sleep";
			this.cbxPort2Sleep.Size = new System.Drawing.Size(53, 17);
			this.cbxPort2Sleep.TabIndex = 66;
			this.cbxPort2Sleep.Text = "Sleep";
			this.cbxPort2Sleep.UseVisualStyleBackColor = true;
			//
			//lblPort2Error
			//
			this.lblPort2Error.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort2Error.Location = new System.Drawing.Point(72, 76);
			this.lblPort2Error.Name = "lblPort2Error";
			this.lblPort2Error.Size = new System.Drawing.Size(100, 20);
			this.lblPort2Error.TabIndex = 61;
			this.lblPort2Error.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label30
			//
			this.Label30.Location = new System.Drawing.Point(6, 76);
			this.Label30.Name = "Label30";
			this.Label30.Size = new System.Drawing.Size(60, 20);
			this.Label30.TabIndex = 60;
			this.Label30.Text = "Error:";
			this.Label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//txtPort2SelecTemp
			//
			this.txtPort2SelecTemp.Location = new System.Drawing.Point(215, 57);
			this.txtPort2SelecTemp.Mask = "999";
			this.txtPort2SelecTemp.Name = "txtPort2SelecTemp";
			this.txtPort2SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.txtPort2SelecTemp.TabIndex = 59;
			this.txtPort2SelecTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			//
			//butPort2SelecTemp
			//
			this.butPort2SelecTemp.Location = new System.Drawing.Point(178, 55);
			this.butPort2SelecTemp.Name = "butPort2SelecTemp";
			this.butPort2SelecTemp.Size = new System.Drawing.Size(31, 23);
			this.butPort2SelecTemp.TabIndex = 58;
			this.butPort2SelecTemp.Text = "<=";
			this.butPort2SelecTemp.UseVisualStyleBackColor = true;
			//
			//lblPort2SelecTemp
			//
			this.lblPort2SelecTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort2SelecTemp.Location = new System.Drawing.Point(72, 56);
			this.lblPort2SelecTemp.Name = "lblPort2SelecTemp";
			this.lblPort2SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort2SelecTemp.TabIndex = 57;
			this.lblPort2SelecTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label32
			//
			this.Label32.Location = new System.Drawing.Point(6, 56);
			this.Label32.Name = "Label32";
			this.Label32.Size = new System.Drawing.Size(70, 20);
			this.Label32.TabIndex = 56;
			this.Label32.Text = "Selec temp:";
			this.Label32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//Label33
			//
			this.Label33.Location = new System.Drawing.Point(6, 36);
			this.Label33.Name = "Label33";
			this.Label33.Size = new System.Drawing.Size(60, 20);
			this.Label33.TabIndex = 18;
			this.Label33.Text = "Act temp:";
			this.Label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort2ActualTemp
			//
			this.lblPort2ActualTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort2ActualTemp.Location = new System.Drawing.Point(72, 36);
			this.lblPort2ActualTemp.Name = "lblPort2ActualTemp";
			this.lblPort2ActualTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort2ActualTemp.TabIndex = 19;
			this.lblPort2ActualTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label35
			//
			this.Label35.Location = new System.Drawing.Point(6, 16);
			this.Label35.Name = "Label35";
			this.Label35.Size = new System.Drawing.Size(60, 20);
			this.Label35.TabIndex = 16;
			this.Label35.Text = "Tool:";
			this.Label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort2Tool
			//
			this.lblPort2Tool.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort2Tool.Location = new System.Drawing.Point(72, 16);
			this.lblPort2Tool.Name = "lblPort2Tool";
			this.lblPort2Tool.Size = new System.Drawing.Size(100, 20);
			this.lblPort2Tool.TabIndex = 17;
			this.lblPort2Tool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//gbxPort4
			//
			this.gbxPort4.Controls.Add(this.Label23);
			this.gbxPort4.Controls.Add(this.cbxPort4Extractor);
			this.gbxPort4.Controls.Add(this.cbxPort4Hibernation);
			this.gbxPort4.Controls.Add(this.cbxPort4Sleep);
			this.gbxPort4.Controls.Add(this.lblPort4Error);
			this.gbxPort4.Controls.Add(this.Label38);
			this.gbxPort4.Controls.Add(this.txtPort4SelecTemp);
			this.gbxPort4.Controls.Add(this.butPort4SelecTemp);
			this.gbxPort4.Controls.Add(this.lblPort4SelecTemp);
			this.gbxPort4.Controls.Add(this.Label40);
			this.gbxPort4.Controls.Add(this.Label41);
			this.gbxPort4.Controls.Add(this.lblPort4ActualTemp);
			this.gbxPort4.Controls.Add(this.Label43);
			this.gbxPort4.Controls.Add(this.lblPort4Tool);
			this.gbxPort4.Location = new System.Drawing.Point(344, 339);
			this.gbxPort4.Name = "gbxPort4";
			this.gbxPort4.Size = new System.Drawing.Size(324, 119);
			this.gbxPort4.TabIndex = 59;
			this.gbxPort4.TabStop = false;
			this.gbxPort4.Text = "Port 4";
			//
			//Label23
			//
			this.Label23.Location = new System.Drawing.Point(6, 96);
			this.Label23.Name = "Label23";
			this.Label23.Size = new System.Drawing.Size(60, 20);
			this.Label23.TabIndex = 69;
			this.Label23.Text = "Status:";
			this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//cbxPort4Extractor
			//
			this.cbxPort4Extractor.AutoCheck = false;
			this.cbxPort4Extractor.AutoSize = true;
			this.cbxPort4Extractor.Location = new System.Drawing.Point(217, 99);
			this.cbxPort4Extractor.Name = "cbxPort4Extractor";
			this.cbxPort4Extractor.Size = new System.Drawing.Size(68, 17);
			this.cbxPort4Extractor.TabIndex = 68;
			this.cbxPort4Extractor.Text = "Extractor";
			this.cbxPort4Extractor.UseVisualStyleBackColor = true;
			//
			//cbxPort4Hibernation
			//
			this.cbxPort4Hibernation.AutoCheck = false;
			this.cbxPort4Hibernation.AutoSize = true;
			this.cbxPort4Hibernation.Location = new System.Drawing.Point(131, 99);
			this.cbxPort4Hibernation.Name = "cbxPort4Hibernation";
			this.cbxPort4Hibernation.Size = new System.Drawing.Size(80, 17);
			this.cbxPort4Hibernation.TabIndex = 67;
			this.cbxPort4Hibernation.Text = "Hibernation";
			this.cbxPort4Hibernation.UseVisualStyleBackColor = true;
			//
			//cbxPort4Sleep
			//
			this.cbxPort4Sleep.AutoCheck = false;
			this.cbxPort4Sleep.AutoSize = true;
			this.cbxPort4Sleep.Location = new System.Drawing.Point(72, 99);
			this.cbxPort4Sleep.Name = "cbxPort4Sleep";
			this.cbxPort4Sleep.Size = new System.Drawing.Size(53, 17);
			this.cbxPort4Sleep.TabIndex = 66;
			this.cbxPort4Sleep.Text = "Sleep";
			this.cbxPort4Sleep.UseVisualStyleBackColor = true;
			//
			//lblPort4Error
			//
			this.lblPort4Error.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort4Error.Location = new System.Drawing.Point(72, 76);
			this.lblPort4Error.Name = "lblPort4Error";
			this.lblPort4Error.Size = new System.Drawing.Size(100, 20);
			this.lblPort4Error.TabIndex = 61;
			this.lblPort4Error.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label38
			//
			this.Label38.Location = new System.Drawing.Point(6, 76);
			this.Label38.Name = "Label38";
			this.Label38.Size = new System.Drawing.Size(60, 20);
			this.Label38.TabIndex = 60;
			this.Label38.Text = "Error:";
			this.Label38.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//txtPort4SelecTemp
			//
			this.txtPort4SelecTemp.Location = new System.Drawing.Point(215, 57);
			this.txtPort4SelecTemp.Mask = "999";
			this.txtPort4SelecTemp.Name = "txtPort4SelecTemp";
			this.txtPort4SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.txtPort4SelecTemp.TabIndex = 59;
			this.txtPort4SelecTemp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			//
			//butPort4SelecTemp
			//
			this.butPort4SelecTemp.Location = new System.Drawing.Point(178, 55);
			this.butPort4SelecTemp.Name = "butPort4SelecTemp";
			this.butPort4SelecTemp.Size = new System.Drawing.Size(31, 23);
			this.butPort4SelecTemp.TabIndex = 58;
			this.butPort4SelecTemp.Text = "<=";
			this.butPort4SelecTemp.UseVisualStyleBackColor = true;
			//
			//lblPort4SelecTemp
			//
			this.lblPort4SelecTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort4SelecTemp.Location = new System.Drawing.Point(72, 56);
			this.lblPort4SelecTemp.Name = "lblPort4SelecTemp";
			this.lblPort4SelecTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort4SelecTemp.TabIndex = 57;
			this.lblPort4SelecTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label40
			//
			this.Label40.Location = new System.Drawing.Point(6, 56);
			this.Label40.Name = "Label40";
			this.Label40.Size = new System.Drawing.Size(70, 20);
			this.Label40.TabIndex = 56;
			this.Label40.Text = "Selec temp:";
			this.Label40.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//Label41
			//
			this.Label41.Location = new System.Drawing.Point(6, 36);
			this.Label41.Name = "Label41";
			this.Label41.Size = new System.Drawing.Size(60, 20);
			this.Label41.TabIndex = 18;
			this.Label41.Text = "Act temp:";
			this.Label41.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort4ActualTemp
			//
			this.lblPort4ActualTemp.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort4ActualTemp.Location = new System.Drawing.Point(72, 36);
			this.lblPort4ActualTemp.Name = "lblPort4ActualTemp";
			this.lblPort4ActualTemp.Size = new System.Drawing.Size(100, 20);
			this.lblPort4ActualTemp.TabIndex = 19;
			this.lblPort4ActualTemp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//Label43
			//
			this.Label43.Location = new System.Drawing.Point(6, 16);
			this.Label43.Name = "Label43";
			this.Label43.Size = new System.Drawing.Size(60, 20);
			this.Label43.TabIndex = 16;
			this.Label43.Text = "Tool:";
			this.Label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			//
			//lblPort4Tool
			//
			this.lblPort4Tool.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.lblPort4Tool.Location = new System.Drawing.Point(72, 16);
			this.lblPort4Tool.Name = "lblPort4Tool";
			this.lblPort4Tool.Size = new System.Drawing.Size(100, 20);
			this.lblPort4Tool.TabIndex = 17;
			this.lblPort4Tool.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			//
			//gbxStationData
			//
			this.gbxStationData.Controls.Add(this.lblModel);
			this.gbxStationData.Controls.Add(this.Label11);
			this.gbxStationData.Controls.Add(this.Label17);
			this.gbxStationData.Controls.Add(this.lblError);
			this.gbxStationData.Controls.Add(this.Label5);
			this.gbxStationData.Controls.Add(this.Label15);
			this.gbxStationData.Controls.Add(this.lblSW);
			this.gbxStationData.Controls.Add(this.Label13);
			this.gbxStationData.Controls.Add(this.lblMOSerror);
			this.gbxStationData.Controls.Add(this.lblTrafoError);
			this.gbxStationData.Controls.Add(this.lblTrafo);
			this.gbxStationData.Controls.Add(this.Label4);
			this.gbxStationData.Location = new System.Drawing.Point(12, 12);
			this.gbxStationData.Name = "gbxStationData";
			this.gbxStationData.Size = new System.Drawing.Size(525, 64);
			this.gbxStationData.TabIndex = 60;
			this.gbxStationData.TabStop = false;
			this.gbxStationData.Text = "Station Data";
			//
			//FormMainParams
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF((float) (6.0F), (float) (13.0F));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(679, 470);
			this.Controls.Add(this.gbxStationData);
			this.Controls.Add(this.gbxPort4);
			this.Controls.Add(this.gbxPort2);
			this.Controls.Add(this.gbxPort3);
			this.Controls.Add(this.gbxPort1);
			this.Controls.Add(this.gbxStationParams);
			this.Controls.Add(this.Label2);
			this.Controls.Add(this.cbxStations);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = (System.Drawing.Icon) (resources.GetObject("$this.Icon"));
			this.Name = "FormMainParams";
			this.Text = "JBC - JBC Connect library test";
			this.gbxStationParams.ResumeLayout(false);
			this.gbxStationParams.PerformLayout();
			this.gbxPort1.ResumeLayout(false);
			this.gbxPort1.PerformLayout();
			this.gbxPort3.ResumeLayout(false);
			this.gbxPort3.PerformLayout();
			this.gbxPort2.ResumeLayout(false);
			this.gbxPort2.PerformLayout();
			this.gbxPort4.ResumeLayout(false);
			this.gbxPort4.PerformLayout();
			this.gbxStationData.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
			
		}
		internal System.Windows.Forms.Label Label2;
		internal System.Windows.Forms.Label Label8;
		internal System.Windows.Forms.Label lblName;
		internal System.Windows.Forms.TextBox txtName;
		internal System.Windows.Forms.Button butName;
		internal System.Windows.Forms.Label Label4;
		internal System.Windows.Forms.Label Label11;
		internal System.Windows.Forms.Label lblTrafoError;
		internal System.Windows.Forms.Label Label13;
		internal System.Windows.Forms.Label lblMOSerror;
		internal System.Windows.Forms.Label Label15;
		internal System.Windows.Forms.Label lblSW;
		internal System.Windows.Forms.Label Label17;
		internal System.Windows.Forms.Label lblError;
		internal System.Windows.Forms.Label Label6;
		internal System.Windows.Forms.Label lblMaxTemp;
		internal System.Windows.Forms.Button butMaxTemp;
		internal System.Windows.Forms.Label Label10;
		internal System.Windows.Forms.Label lblMinTemp;
		internal System.Windows.Forms.Button butMinTemp;
		internal System.Windows.Forms.Label Label20;
		internal System.Windows.Forms.Label lblUnits;
		internal System.Windows.Forms.Button butUnits;
		internal System.Windows.Forms.ComboBox cbxUnits;
		internal System.Windows.Forms.ComboBox cbxN2;
		internal System.Windows.Forms.Label Label22;
		internal System.Windows.Forms.Label lblN2;
		internal System.Windows.Forms.Button butN2;
		internal System.Windows.Forms.ComboBox cbxHelp;
		internal System.Windows.Forms.Label Label24;
		internal System.Windows.Forms.Label lblHelp;
		internal System.Windows.Forms.Button butHelp;
		internal System.Windows.Forms.ComboBox cbxBeep;
		internal System.Windows.Forms.Label Label28;
		internal System.Windows.Forms.Label lblBeep;
		internal System.Windows.Forms.Button butBeep;
		internal System.Windows.Forms.GroupBox gbxStationParams;
		internal System.Windows.Forms.MaskedTextBox txtMinTemp;
		internal System.Windows.Forms.MaskedTextBox txtMaxTemp;
		private System.Windows.Forms.ComboBox cbxStations;
		internal System.Windows.Forms.Label lblModel;
		internal System.Windows.Forms.Label Label5;
		internal System.Windows.Forms.GroupBox gbxPort1;
		internal System.Windows.Forms.Label Label3;
		internal System.Windows.Forms.Label lblPort1Tool;
		internal System.Windows.Forms.Label Label9;
		internal System.Windows.Forms.Label lblPort1ActualTemp;
		internal System.Windows.Forms.Label lblTrafo;
		internal System.Windows.Forms.MaskedTextBox txtPort1SelecTemp;
		internal System.Windows.Forms.Button butPort1SelecTemp;
		internal System.Windows.Forms.Label lblPort1SelecTemp;
		internal System.Windows.Forms.Label Label16;
		internal System.Windows.Forms.Label lblPort1Error;
		internal System.Windows.Forms.Label Label19;
		internal System.Windows.Forms.GroupBox gbxPort3;
		internal System.Windows.Forms.Label lblPort3Error;
		internal System.Windows.Forms.Label Label12;
		internal System.Windows.Forms.MaskedTextBox txtPort3SelecTemp;
		internal System.Windows.Forms.Button butPort3SelecTemp;
		internal System.Windows.Forms.Label lblPort3SelecTemp;
		internal System.Windows.Forms.Label Label18;
		internal System.Windows.Forms.Label Label21;
		internal System.Windows.Forms.Label lblPort3ActualTemp;
		internal System.Windows.Forms.Label Label25;
		internal System.Windows.Forms.Label lblPort3Tool;
		internal System.Windows.Forms.GroupBox gbxPort2;
		internal System.Windows.Forms.Label lblPort2Error;
		internal System.Windows.Forms.Label Label30;
		internal System.Windows.Forms.MaskedTextBox txtPort2SelecTemp;
		internal System.Windows.Forms.Button butPort2SelecTemp;
		internal System.Windows.Forms.Label lblPort2SelecTemp;
		internal System.Windows.Forms.Label Label32;
		internal System.Windows.Forms.Label Label33;
		internal System.Windows.Forms.Label lblPort2ActualTemp;
		internal System.Windows.Forms.Label Label35;
		internal System.Windows.Forms.Label lblPort2Tool;
		internal System.Windows.Forms.GroupBox gbxPort4;
		internal System.Windows.Forms.Label lblPort4Error;
		internal System.Windows.Forms.Label Label38;
		internal System.Windows.Forms.MaskedTextBox txtPort4SelecTemp;
		internal System.Windows.Forms.Button butPort4SelecTemp;
		internal System.Windows.Forms.Label lblPort4SelecTemp;
		internal System.Windows.Forms.Label Label40;
		internal System.Windows.Forms.Label Label41;
		internal System.Windows.Forms.Label lblPort4ActualTemp;
		internal System.Windows.Forms.Label Label43;
		internal System.Windows.Forms.Label lblPort4Tool;
		internal System.Windows.Forms.GroupBox gbxStationData;
		internal System.Windows.Forms.CheckBox cbxPort1Sleep;
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.CheckBox cbxPort1Extractor;
		internal System.Windows.Forms.CheckBox cbxPort1Hibernation;
		internal System.Windows.Forms.Label Label7;
		internal System.Windows.Forms.CheckBox cbxPort2Extractor;
		internal System.Windows.Forms.CheckBox cbxPort2Hibernation;
		internal System.Windows.Forms.CheckBox cbxPort2Sleep;
		internal System.Windows.Forms.Label Label14;
		internal System.Windows.Forms.CheckBox cbxPort3Extractor;
		internal System.Windows.Forms.CheckBox cbxPort3Hibernation;
		internal System.Windows.Forms.CheckBox cbxPort3Sleep;
		internal System.Windows.Forms.Label Label23;
		internal System.Windows.Forms.CheckBox cbxPort4Extractor;
		internal System.Windows.Forms.CheckBox cbxPort4Hibernation;
		internal System.Windows.Forms.CheckBox cbxPort4Sleep;
		
	}
	
}
