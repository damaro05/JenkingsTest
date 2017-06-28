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
    public partial class ParamTable
    {

        // Defines the posible input types for the values of the rows
        public enum cInputType
        {
            FIX,
            TEXT,
            NUMBER,
            SWITCH,
            BUTTON, // raise an event to me managed outside paramTable
            DROPLIST
        }

        // Type definitions
        private class tParam
        {
            public string paramName;
            public Label lblText;
            public Button lblValue;
            public ContextMenuStrip dropList;
            public CheckBox lblCheck;
            public cInputType input;
            public bool enabled;
            public string value;
            public int maxLengthValue;
            // if cInputType = NUMBER, optionsValue(0) = string where to parse the value, for example "{0} ºC"
            // if cInputType = NUMBER, optionsValue(1) = minimum integer value
            // if cInputType = NUMBER, optionsValue(2) = maximum integer value
            // if cInputType = NUMBER, optionsValue(3) = allowed values, separated by # (ej: 0#1#3#5)
            // if cInputType = SWITCH, optionsValue contains the values and optionsText contains the text to be showed, if diffrerent from values
            public string[] optionsValue;
            public string[] optionsText; // if text is diferent from optionsValue
        }

        // Constants
        private const int DEFAULT_TEXT_COL_WIDTH = 270;
        private const int DEFAULT_VALUE_COL_WIDTH = 120;
        private const int DEFAULT_CHECK_COL_WIDTH = 10;

        // Properties
        public Font ptyFont
        {
            set
            {
                textFont = value;
                this.Invalidate();
            }
            get
            {
                return textFont;
            }
        }

        public Color ptyFontColor
        {
            set
            {
                textColor = value;
                this.Invalidate();
            }
            get
            {
                return textColor;
            }
        }

        public Font ptyMouseOverFont
        {
            set
            {
                textMouseOverFont = value;
                this.Invalidate();
            }
            get
            {
                return textMouseOverFont;
            }
        }

        public Color ptyMouseOverFontColor
        {
            set
            {
                textMouseOverColor = value;
                this.Invalidate();
            }
            get
            {
                return textMouseOverColor;
            }
        }

        public Color ptyTextRowEditBackColor
        {
            set
            {
                textRowEditedBackColor = value;
                this.Invalidate();
            }
            get
            {
                return textRowEditedBackColor;
            }
        }

        public void set_ptyColumnWidth(int index, int value)
        {
            if (index < TableLayoutPanel1.ColumnCount)
            {
                TableLayoutPanel1.ColumnStyles[index].SizeType = SizeType.Absolute;
                TableLayoutPanel1.ColumnStyles[index].Width = value;
                PanelTable.Width = System.Convert.ToInt32(System.Convert.ToInt32(TableLayoutPanel1.ColumnStyles[0].Width +
                    TableLayoutPanel1.ColumnStyles[1].Width +
                    TableLayoutPanel1.ColumnStyles[2].Width) + 20);
                base.Width = PanelTable.Width;
            }
        }
        public int get_ptyColumnWidth(int index)
        {
            if (index < TableLayoutPanel1.ColumnCount)
            {
                int[] mWidths = TableLayoutPanel1.GetColumnWidths();
                return mWidths[index];
            }
            else
            {
                return 0;
            }
        }

        public void set_ptyRowHeight(int index, int value)
        {
            if (index < TableLayoutPanel1.RowCount)
            {
                TableLayoutPanel1.RowStyles[index].SizeType = SizeType.Absolute;
                TableLayoutPanel1.RowStyles[index].Height = value;
            }
        }
        public int get_ptyRowHeight(int index)
        {
            if (index < TableLayoutPanel1.RowCount)
            {
                int[] mHeights = TableLayoutPanel1.GetRowHeights();
                return mHeights[index];
            }
            else
            {
                return 0;
            }
        }

        public int ptyDefaultRowHeight
        {
            set
            {
                rowHeight = value;
                // set absolute height to all rows except the last, that contains the textbox
                if (TableLayoutPanel1.RowStyles.Count > 1)
                {
                    for (var i = 0; i <= TableLayoutPanel1.RowStyles.Count - 2; i++)
                    {
                        TableLayoutPanel1.RowStyles[System.Convert.ToInt32(i)].SizeType = SizeType.Absolute;
                        TableLayoutPanel1.RowStyles[System.Convert.ToInt32(i)].Height = value;
                    }
                }
            }
            get
            {
                return rowHeight;
            }
        }

        public Control ptyCellControl(string paramName, int indexCol)
        {
            int indexRow = getIndex(paramName);
            if (indexRow < TableLayoutPanel1.RowCount & indexCol < TableLayoutPanel1.ColumnCount)
            {
                switch (indexCol)
                {
                    case 0:
                        return paramList[indexRow].lblText;
                    case 1:
                        return paramList[indexRow].lblValue;
                    case 2:
                        return paramList[indexRow].lblCheck;
                }
            }
            return null;
        }

        // Internall var's and parameters

        private System.Collections.Generic.List<tParam> paramList = new System.Collections.Generic.List<tParam>();

        private Font textFont = new Font("Microsoft Sans Seriff", 10);
        private Color textColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. //Color.WhiteSmoke
        private int rowHeight = 32; // Row height in pixels
        private Color textMouseOverColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. // Color of the text when mouse is over
        private Font textMouseOverFont = new Font("Microsoft Sans Seriff", 10, FontStyle.Bold); // Font of the text when mouse is over
        private Color textRowEditedBackColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors. // Back color for a text row when edited
        private bool bControlDown = false;

        // Public Events
        public delegate void NewValueEventHandler(string paramName, string value);
        private NewValueEventHandler NewValueEvent;

        public event NewValueEventHandler NewValue
        {
            add
            {
                NewValueEvent = (NewValueEventHandler)System.Delegate.Combine(NewValueEvent, value);
            }
            remove
            {
                NewValueEvent = (NewValueEventHandler)System.Delegate.Remove(NewValueEvent, value);
            }
        }

        public delegate void paramTableCheckedChangedEventHandler(string paramName, bool value);
        private paramTableCheckedChangedEventHandler paramTableCheckedChangedEvent;

        public event paramTableCheckedChangedEventHandler paramTableCheckedChanged
        {
            add
            {
                paramTableCheckedChangedEvent = (paramTableCheckedChangedEventHandler)System.Delegate.Combine(paramTableCheckedChangedEvent, value);
            }
            remove
            {
                paramTableCheckedChangedEvent = (paramTableCheckedChangedEventHandler)System.Delegate.Remove(paramTableCheckedChangedEvent, value);
            }
        }

        public delegate void paramTableButtonClickedEventHandler(string paramName, string value);
        private paramTableButtonClickedEventHandler paramTableButtonClickedEvent;

        public event paramTableButtonClickedEventHandler paramTableButtonClicked
        {
            add
            {
                paramTableButtonClickedEvent = (paramTableButtonClickedEventHandler)System.Delegate.Combine(paramTableButtonClickedEvent, value);
            }
            remove
            {
                paramTableButtonClickedEvent = (paramTableButtonClickedEventHandler)System.Delegate.Remove(paramTableButtonClickedEvent, value);
            }
        }


        //Constructor
        public ParamTable(int iTextColWidth, int iValueColWidth, int iCheckColWidth = 0)
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            textColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)), System.Convert.ToInt32(System.Convert.ToByte(80)));
            textMouseOverColor = System.Drawing.Color.FromArgb(System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)), System.Convert.ToInt32(System.Convert.ToByte(64)));
            textRowEditedBackColor = Color.FromArgb(150, Color.LightGray);


            // Llamada necesaria para el diseñador.
            InitializeComponent();

            // Agregue cualquier inicialización después de la llamada a InitializeComponent().
            this.DoubleBuffered = true;

            tbEdit.Visible = false; // ocultar campo de edición

            // Settings the columns width and title
            TableLayoutPanel1.ColumnCount = 3;
            TableLayoutPanel1.RowCount = 1;
            TableLayoutPanel1.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            int[] colW = new int[TableLayoutPanel1.ColumnCount - 1 + 1];
            // load defaults
            colW[0] = DEFAULT_TEXT_COL_WIDTH;
            colW[1] = DEFAULT_VALUE_COL_WIDTH;
            colW[2] = DEFAULT_CHECK_COL_WIDTH;
            // change defaults
            if (iTextColWidth > 0)
            {
                colW[0] = iTextColWidth;
            }
            if (iValueColWidth > 0)
            {
                colW[1] = iValueColWidth;
            }
            if (iCheckColWidth > 0)
            {
                colW[2] = iCheckColWidth;
            }
            // apply all
            for (var i = 0; i <= TableLayoutPanel1.ColumnCount - 1; i++)
            {
                set_ptyColumnWidth(System.Convert.ToInt32(i), colW[(int)i]);
            }

        }

        // Adds a param to the table
        public void addParam(string paramName, string text, cInputType input, string[] options, string[] optionsText, bool bShowCheckBox = false, int maxLength = 0)
        {
            // Creating a new tParam
            tParam param = new tParam();

            // Initializing it
            param.paramName = paramName;

            // param title
            param.lblText = new Label();
            param.lblText.Name = "lblName" + paramName;
            param.lblText.ForeColor = textColor;
            //param.lblText.AutoSize = True
            param.lblText.Text = text;
            param.lblText.Margin = new Padding(3, 3, 3, 0);
            param.lblText.Padding = new Padding(0);

            param.lblText.TextAlign = ContentAlignment.TopLeft; // MiddleLeft
            param.lblText.BorderStyle = System.Windows.Forms.BorderStyle.None; // Windows.Forms.BorderStyle.FixedSingle

            param.lblText.MouseEnter += Param_MouseEnter;
            param.lblText.MouseLeave += Param_MouseLeave;
            //AddHandler param.lblText.MouseUp, AddressOf ParamTable_MouseUp

            // param data
            //param.lblValue = New Label
            //param.lblValue.BorderStyle = Windows.Forms.BorderStyle.None ' Windows.Forms.BorderStyle.FixedSingle
            param.lblValue = new Button();
            param.lblValue.Name = "lblValue" + paramName;
            param.lblValue.FlatStyle = FlatStyle.Flat;
            param.lblValue.ForeColor = textColor;
            param.lblValue.Text = Configuration.noDataStr;
            param.lblValue.Margin = new Padding(0);
            param.lblValue.Padding = new Padding(0);
            param.lblValue.Height = 28;
            param.lblValue.TextAlign = ContentAlignment.TopRight; // MiddleRight

            param.dropList = new ContextMenuStrip();
            param.maxLengthValue = maxLength;

            if (input == cInputType.BUTTON)
            {
                param.lblValue.FlatAppearance.BorderSize = 0;
                param.lblValue.FlatAppearance.BorderColor = Color.LightSteelBlue;
                param.lblValue.FlatAppearance.MouseDownBackColor = Color.LightSteelBlue;
                param.lblValue.FlatAppearance.MouseOverBackColor = Color.LightSteelBlue;
                param.lblValue.Cursor = Cursors.Hand;
                param.lblValue.Click += lblBut_Click;
            }
            else
            {
                param.lblValue.FlatAppearance.BorderSize = 0;
                param.lblValue.FlatAppearance.MouseDownBackColor = Color.Transparent;
                param.lblValue.FlatAppearance.MouseOverBackColor = Color.Transparent;
                //param.lblValue.AutoSize = True
                param.lblValue.MouseEnter += Param_MouseEnter;
                param.lblValue.MouseLeave += Param_MouseLeave;
                param.lblValue.MouseUp += ParamTable_MouseUp;
                param.lblValue.KeyDown += ParamTable_KeyDown;
                param.lblValue.KeyUp += ParamTable_KeyUp;
                if (input == cInputType.DROPLIST)
                {
                    param.dropList.Items.Clear();
                    foreach (string optionEl in optionsText)
                    {
                        param.dropList.Items.Add(optionEl);
                    }

                    param.dropList.ItemClicked += dropList_ItemClicked;
                }
            }

            // param checkbox
            if (bShowCheckBox)
            {
                param.lblCheck = new CheckBox();
                param.lblCheck.Name = "lblCheck" + paramName;
                param.lblCheck.Text = "";
                param.lblCheck.MouseClick += lblCheck_CheckedChanged;
            }
            else
            {
                param.lblCheck = null;
            }

            // Setting the input
            param.input = input;
            param.value = Configuration.noDataStr;
            param.enabled = true;
            param.optionsValue = options;
            param.optionsText = optionsText;

            // Adding it to the list
            paramList.Add(param);

            // Adding the row
            TableLayoutPanel1.RowCount++;
            TableLayoutPanel1.SetRow(tbEdit, TableLayoutPanel1.RowCount - 1); // move edit control to the las row
            TableLayoutPanel1.Controls.Add(param.lblText, 0, TableLayoutPanel1.RowCount - 2);
            TableLayoutPanel1.Controls.Add(param.lblValue, 1, TableLayoutPanel1.RowCount - 2);
            if (param.lblCheck != null)
            {
                TableLayoutPanel1.Controls.Add(param.lblCheck, 2, TableLayoutPanel1.RowCount - 2);
            }
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            param.lblText.Width = get_ptyColumnWidth(0);
            param.lblValue.Width = get_ptyColumnWidth(1);

        }

        // Gets a param value
        public string getValue(string paramName)
        {
            int index = getIndex(paramName);
            if (index != -1)
            {
                return paramList[index].value;
            }
            else
            {
                return "";
            }
        }

        // Gets a param check
        public bool getCheck(string paramName)
        {
            int index = getIndex(paramName);
            if (index != -1)
            {
                if (paramList[index].lblCheck != null)
                {
                    return paramList[index].lblCheck.Checked;
                }
            }
            return false;
        }

        // Sets a param value
        public void setValue(string paramName, string value)
        {
            int index = getIndex(paramName);
            if (index != -1)
            {

                if (paramList[index].input == cInputType.NUMBER & paramList[index].optionsValue != null)
                {
                    // NUMBER with parsing string
                    if (paramList[index].optionsValue[0] != "")
                    {
                        paramList[index].lblValue.Text = paramList[index].optionsValue[0].Replace(Configuration.sReplaceTag, value);
                    }
                    else
                    {
                        paramList[index].lblValue.Text = value;
                    }
                }
                else if ((paramList[index].input == cInputType.SWITCH | paramList[index].input == cInputType.DROPLIST) && paramList[index].optionsText != null)
                {
                    // SWITCH and DROPLIST with separated text and values options
                    int idx = Array.IndexOf((System.Array)(paramList[index].optionsValue), value);
                    if (idx >= 0)
                    {
                        paramList[index].lblValue.Text = paramList[index].optionsText[idx];
                    }
                    else
                    {
                        paramList[index].lblValue.Text = value;
                    }
                }
                else
                {
                    // ANY OTHER
                    paramList[index].lblValue.Text = value;
                }

                paramList[index].value = value;
            }
        }

        // Sets a param text
        public void setText(string paramName, string valueText, string[] optionsText)
        {
            int index = getIndex(paramName);
            if (index != -1)
            {
                paramList[index].lblText.Text = valueText;
                // redisplay data containing localized text (formatted NUMBER and SWITCH)
                if (paramList[index].input == cInputType.NUMBER & optionsText != null)
                {
                    // NUMBER with parsing string
                    paramList[index].optionsValue = optionsText;
                    // redisplay data if it is formatted text
                    if (paramList[index].optionsValue[0] != "")
                    {
                        paramList[index].lblValue.Text = paramList[index].optionsValue[0].Replace(Configuration.sReplaceTag, paramList[index].value);
                    }
                }
                else if (paramList[index].input == cInputType.SWITCH & optionsText != null)
                {
                    // SWITCH with separated text and values options
                    paramList[index].optionsText = optionsText;
                    // redisplay selected data
                    int idx = Array.IndexOf((System.Array)(paramList[index].optionsValue), paramList[index].value);
                    if (idx >= 0)
                    {
                        paramList[index].lblValue.Text = paramList[index].optionsText[idx];
                    }
                    else
                    {
                        paramList[index].lblValue.Text = paramList[index].value;
                    }
                }
                else if (paramList[index].input == cInputType.DROPLIST & optionsText != null)
                {
                    // DROPLIST with separated text and values options
                    paramList[index].optionsText = optionsText;

                    //change droplist elements
                    for (int i = 0; i <= paramList[index].dropList.Items.Count - 1; i++)
                    {
                        paramList[index].dropList.Items[i].Text = optionsText[i];
                    }
                }
            }
        }

        // Sets a param check
        public void setCheck(string paramName, bool value)
        {
            int index = getIndex(paramName);
            if (index != -1)
            {
                if (paramList[index].lblCheck != null)
                {
                    paramList[index].lblCheck.Checked = value;
                }
            }
        }

        // Enables/disables all the input controls
        public void inputControlsEnable(bool status)
        {
            // Setting the desired enable status for all the controls
            for (int i = 0; i <= paramList.Count - 1; i++)
            {
                paramList[i].enabled = status;
                if (paramList[i].lblCheck != null)
                {
                    paramList[i].lblCheck.Enabled = status;
                }
            }
        }

        // Returns the indicated parameter position
        private int getIndex(string paramName)
        {
            int cnt = 0;

            while (cnt < paramList.Count)
            {
                if (paramList[cnt].paramName == paramName)
                {
                    return cnt;
                }
                cnt++;
            }

            return -1;
        }

        // Returns the row position of a point
        //Private Function getRowFromPoint(ByVal p As Point) As Integer
        //    Dim lH As Long = 0
        //    Dim mHeights() As Int32 = TableLayoutPanel1.GetRowHeights
        //    getRowFromPoint = -1
        //    For i = 0 To TableLayoutPanel1.RowCount - 1
        //        lH += mHeights(i)
        //        If lH > p.Y Then
        //            getRowFromPoint = i
        //            Exit For
        //        End If
        //    Next
        //End Function

        // Drawing event
        private int mouseOverIndex = -1;
        private int curInputIndex = -1;

        // resize event
        protected override void OnResize(System.EventArgs e)
        {
            PanelTable.Size = base.Size;
            // when resizing control, resize PanelTable to leave space for vertical scroll
            PanelTable.Width = PanelTable.Width - 20;
            base.OnResize(e);
        }

        // Mouse events for selection and modification of parameters
        //Private Sub ParamTable_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        private void Param_MouseEnter(System.Object sender, System.EventArgs e)
        {
            //Debug.Print("Param_MouseEnter")
            bool bDoFocus = true;
            int iRow = -1;
            // 29/07/2015 no pintar ni cambiar foco cuando estoy editando un valor
            if (curInputIndex >= 0)
            {
                // no cambiar focus para que no haga validating del tbEdit
                bDoFocus = false;
            }

            if (ReferenceEquals(((Control)sender).Parent, TableLayoutPanel1))
            {
                iRow = TableLayoutPanel1.GetRow((Control)sender);
                //Debug.Print("iRow=" & iRow.ToString & " - mouseOverIndex=" & mouseOverIndex.ToString)
                //If iRow < 0 Then Exit Sub
                if (iRow != mouseOverIndex)
                {
                    // set previous labels
                    if (mouseOverIndex >= 0)
                    {
                        paramList[mouseOverIndex].lblText.Font = textFont;
                        paramList[mouseOverIndex].lblText.ForeColor = textColor;
                        paramList[mouseOverIndex].lblText.BackColor = Color.Transparent;
                        paramList[mouseOverIndex].lblValue.Font = textFont;
                        paramList[mouseOverIndex].lblValue.ForeColor = textColor;
                        paramList[mouseOverIndex].lblValue.BackColor = Color.Transparent;
                        paramList[mouseOverIndex].lblValue.Cursor = Cursors.Default;
                    }
                    paramList[iRow].lblText.Font = textMouseOverFont;
                    paramList[iRow].lblText.ForeColor = textMouseOverColor;
                    paramList[iRow].lblText.BackColor = Color.Transparent;
                    paramList[iRow].lblValue.Font = textMouseOverFont;
                    paramList[iRow].lblValue.ForeColor = textMouseOverColor;
                    paramList[iRow].lblValue.BackColor = Color.Transparent;
                    if (paramList[iRow].input == cInputType.SWITCH & paramList[iRow].enabled)
                    {
                        // set focus to get key events
                        if (bDoFocus)
                        {
                            paramList[iRow].lblValue.Focus();
                        }
                        if (bControlDown)
                        {
                            paramList[iRow].lblValue.Cursor = Configuration.cursor_switch_minus; // Cursors.PanSouth
                        }
                        else
                        {
                            paramList[iRow].lblValue.Cursor = Configuration.cursor_switch_plus; // Cursors.PanNorth
                        }
                    }
                    else if (paramList[iRow].input != cInputType.FIX & paramList[iRow].enabled)
                    {
                        if (bDoFocus)
                        {
                            paramList[iRow].lblValue.Focus();
                        }
                        paramList[iRow].lblValue.Cursor = Configuration.cursor_hand;
                    }
                    else
                    {
                        if (bDoFocus)
                        {
                            paramList[iRow].lblValue.Focus();
                        }
                        paramList[iRow].lblValue.Cursor = Cursors.Default;
                    }

                }
                mouseOverIndex = TableLayoutPanel1.GetRow((Control)sender);
            }

        }

        public void ParamTable_MouseLeave(object sender, System.EventArgs e)
        {
            //Debug.Print("ParamTable_MouseLeave")
            //PanelTable.Focus()
        }

        private void Param_MouseLeave(object sender, System.EventArgs e)
        {
            //Debug.Print("Param_MouseLeave")
            // When mouse leaves then no row is selected
            if (mouseOverIndex >= 0)
            {
                paramList[mouseOverIndex].lblText.Font = textFont;
                paramList[mouseOverIndex].lblText.ForeColor = textColor;
                paramList[mouseOverIndex].lblText.BackColor = Color.Transparent;
                paramList[mouseOverIndex].lblValue.Font = textFont;
                paramList[mouseOverIndex].lblValue.ForeColor = textColor;
                paramList[mouseOverIndex].lblValue.BackColor = Color.Transparent;
                paramList[mouseOverIndex].lblValue.Cursor = Cursors.Default;
                mouseOverIndex = -1;
            }
        }

        public void ParamTable_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //Debug.Print("ParamTable_MouseUp")

            // if already editing, close previous edit
            //If curInputIndex >= 0 Then
            //    Dim sErr As String = ""
            //    If myValidateEdit(sErr) Then
            //        myEditClose()
            //    Else
            //        MsgBox(sErr)
            //        Return
            //    End If
            //End If

            // If clicked over a row then editing its value
            if (mouseOverIndex != -1) // And sender.GetType Is GetType(Button)
            {
                // Depending on the input type
                tParam param = paramList[mouseOverIndex];
                //Console.WriteLine(param.enabled)
                if (param.enabled)
                {
                    if ((param.input == cInputType.TEXT | param.input == cInputType.NUMBER) && (e.Button == System.Windows.Forms.MouseButtons.Left))
                    {
                        myEditRow(mouseOverIndex);
                    }
                    else if (param.input == cInputType.SWITCH)
                    {
                        // Moving to the next option
                        int curOption = Array.IndexOf(param.optionsValue, param.value);
                        // found value
                        if (curOption >= 0)
                        {
                            if (e.Button == System.Windows.Forms.MouseButtons.Left && !bControlDown) //
                            {
                                curOption++;
                                if (curOption > param.optionsValue.Length - 1)
                                {
                                    curOption = 0;
                                }
                                setValue(param.paramName, param.optionsValue[curOption]);
                                //param.lblValue.Text = param.optionsValue((curOption + 1) Mod param.optionsValue.Length)
                                if (NewValueEvent != null)
                                    NewValueEvent(param.paramName, param.optionsValue[curOption]);
                            }
                            if (e.Button == System.Windows.Forms.MouseButtons.Left && bControlDown)
                            {
                                curOption--;
                                if (curOption < 0)
                                {
                                    curOption = param.optionsValue.Length - 1;
                                }
                                setValue(param.paramName, param.optionsValue[curOption]);
                                //param.lblValue.Text = param.optionsValue(curOption)
                                if (NewValueEvent != null)
                                    NewValueEvent(param.paramName, param.optionsValue[curOption]);
                            }
                        }
                        else
                        {
                            setValue(param.paramName, param.optionsValue[0]);
                        }
                    }
                    else if (param.input == cInputType.DROPLIST)
                    {
                        param.dropList.Visible = true;
                        param.dropList.Show(param.lblValue, 0, param.lblValue.Height);
                    }
                }
                else
                {
                    curInputIndex = -1;
                }
            }
            else
            {
                curInputIndex = -1;
            }

            //Redrawing
            this.Invalidate();
        }

        public void ParamTable_KeyDown(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control)
            {
                bControlDown = true;
                if (mouseOverIndex > -1)
                {
                    if (paramList[mouseOverIndex].lblValue.Cursor == Configuration.cursor_switch_plus)
                    {
                        paramList[mouseOverIndex].lblValue.Cursor = Configuration.cursor_switch_minus;
                    }
                }
            }
        }

        public void ParamTable_KeyUp(System.Object sender, System.Windows.Forms.KeyEventArgs e)
        {
            bControlDown = false;
            if (mouseOverIndex > -1)
            {
                if (paramList[mouseOverIndex].lblValue.Cursor == Configuration.cursor_switch_minus)
                {
                    paramList[mouseOverIndex].lblValue.Cursor = Configuration.cursor_switch_plus;
                }
            }
        }

        private void lblCheck_CheckedChanged(System.Object sender, System.Windows.Forms.MouseEventArgs e)
        {
            int idx = TableLayoutPanel1.GetRow((Control)sender);
            tParam param = paramList[idx];
            if (paramTableCheckedChangedEvent != null)
                paramTableCheckedChangedEvent(param.paramName, ((CheckBox)sender).Checked);
        }

        private void dropList_ItemClicked(System.Object sender, ToolStripItemClickedEventArgs e) //e As System.Windows.Forms.MouseEventArgs)
        {
            for (var i = 0; i <= paramList.Count - 1; i++)
            {
                if (paramList.ElementAt(i).dropList.Equals(sender))
                {
                    setValue(System.Convert.ToString(paramList.ElementAt(i).paramName), System.Convert.ToString(paramList.ElementAt(i).optionsValue[paramList.ElementAt(System.Convert.ToInt32(i)).dropList.Items.IndexOf(e.ClickedItem)]));
                    if (NewValueEvent != null)
                        NewValueEvent(paramList.ElementAt(i).paramName, paramList.ElementAt(i).optionsValue[paramList.ElementAt(i).dropList.Items.IndexOf(e.ClickedItem)]);
                    break;
                }
            }
        }

        private void lblBut_Click(System.Object sender, System.EventArgs e)
        {
            int idx = TableLayoutPanel1.GetRow((Control)sender);
            tParam param = paramList[idx];
            if (paramTableButtonClickedEvent != null)
                paramTableButtonClickedEvent(param.paramName, param.value);
        }

        private void myEditRow(int iRow)
        {
            tParam param = paramList[iRow];
            // if already editing, close previous edit
            if (curInputIndex >= 0)
            {
                myEditClose();
            }
            curInputIndex = iRow;
            tbEdit.MaxLength = param.maxLengthValue;
            tbEdit.Size = param.lblValue.Size;
            tbEdit.Margin = param.lblValue.Margin;
            tbEdit.Text = "";
            if (param.input == cInputType.NUMBER)
            {
                if (param.value == Configuration.noDataStr)
                {
                    tbEdit.Text = "";
                }
                else
                {
                    tbEdit.Text = param.value;
                }
            }
            else
            {
                if (param.value == Configuration.noDataStr)
                {
                    tbEdit.Text = "";
                }
                else
                {
                    tbEdit.Text = param.lblValue.Text;
                }
            }
            // send label to last row, first column
            param.lblValue.Visible = false;
            TableLayoutPanel1.SetCellPosition(param.lblValue, new TableLayoutPanelCellPosition(0, TableLayoutPanel1.RowCount - 1));
            // position edit control (in second column) in current row
            TableLayoutPanel1.SetRow(tbEdit, curInputIndex);
            //tbEdit.Text = tbEdit.Location.X.ToString & ";" & tbEdit.Location.Y.ToString & " " & _
            //              param.lblValue.Location.X.ToString & ";" & param.lblValue.Location.Y.ToString
            tbEdit.Visible = true;
            tbEdit.Focus();
            tbEdit.SelectionLength = tbEdit.TextLength;
            tbEdit.BringToFront();

        }

        private void myEditClose()
        {
            if (curInputIndex != -1)
            {
                tParam param = paramList[curInputIndex];
                if (param.value != tbEdit.Text && !(param.value == Configuration.noDataStr && tbEdit.Text == ""))
                {
                    setValue(param.paramName, tbEdit.Text);
                    if (NewValueEvent != null)
                        NewValueEvent(param.paramName, param.value);
                }
                // send edit control to the last row (same second column)
                tbEdit.Visible = false;
                TableLayoutPanel1.SetRow(tbEdit, TableLayoutPanel1.RowCount - 1);
                // re-position label to the editin row, second column
                TableLayoutPanel1.SetCellPosition(param.lblValue, new TableLayoutPanelCellPosition(1, curInputIndex));
                param.lblValue.Visible = true;
                curInputIndex = -1;
            }
        }

        public void tbEdit_KeyPress(System.Object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            tParam param = default(tParam);
            if (curInputIndex >= 0)
            {
                param = paramList[curInputIndex];
                if (e.KeyChar == Microsoft.VisualBasic.Strings.ChrW((System.Int32)Keys.Enter))
                {
                    tbEdit.Parent.Focus();
                }
                else if (e.KeyChar == Microsoft.VisualBasic.Strings.ChrW((System.Int32)Keys.Escape))
                {
                    tbEdit.Text = param.value.ToString();
                }
                else
                {
                    if (param.input == cInputType.NUMBER)
                    {
                        // only numbers and backspace
                        e.Handled = !RoutinesLibrary.Data.DataType.IntegerUtils.KeyIsNumber(e.KeyChar);
                    }
                }
            }
        }

        public void tbEdit_Validating(System.Object sender, System.ComponentModel.CancelEventArgs e)
        {
            string sErr = "";
            if (curInputIndex >= 0)
            {
                if (myValidateEdit(ref sErr))
                {
                    myEditClose();
                }
                else
                {
                    MessageBox.Show(sErr);
                    e.Cancel = true;
                }
            }
        }

        private bool myValidateEdit(ref string sErr)
        {
            tParam param = default(tParam);
            int iValueMin = 0;
            int iValueMax = 0;
            string sValueMin = "";
            string sValueMax = "";
            string sAllowedValues = "";
            bool bValueExists = false;
            string sValue = "";
            sErr = "";

            if (curInputIndex < 0)
            {
                return true;
            }

            param = paramList[curInputIndex];
            sValue = tbEdit.Text.Trim();

            // check allowed values and limits, if any
            if (param.optionsValue != null)
            {
                // min value
                if (param.optionsValue.Length > 1)
                {
                    try
                    {
                        sValueMin = param.optionsValue[1].Trim();
                        iValueMin = int.Parse(sValueMin);
                    }
                    catch (Exception)
                    {
                        iValueMin = 0;
                    }
                }
                // max value
                if (param.optionsValue.Length > 2)
                {
                    try
                    {
                        sValueMax = param.optionsValue[2].Trim();
                        iValueMax = int.Parse(sValueMax);
                    }
                    catch (Exception)
                    {
                        iValueMax = 0;
                    }
                }
                // allowed values
                if (param.optionsValue.Length > 3)
                {
                    sAllowedValues = param.optionsValue[3].Trim();
                }

                // if allowed values (and no limits)
                if (!string.IsNullOrEmpty(sAllowedValues))
                {
                    if (("#" + sAllowedValues + "#").IndexOf("#" + sValue + "#") + 1 > 0)
                    {
                        bValueExists = true;
                    }
                    // value not allowed and no limits defined
                    if (!bValueExists && string.IsNullOrEmpty(sValueMin) && string.IsNullOrEmpty(sValueMax))
                    {
                        sErr = string.Format(Localization.getResStr(Configuration.paramsAllowedValuesId), sAllowedValues);
                        return false;
                    }
                }

                // if limits
                try
                {
                    if (!bValueExists && (!string.IsNullOrEmpty(sValueMin) || !string.IsNullOrEmpty(sValueMax)))
                    {
                        if (!string.IsNullOrEmpty(sValueMin) && !string.IsNullOrEmpty(sValueMax))
                        {
                            if (int.Parse(sValue) < iValueMin || int.Parse(sValue) > iValueMax)
                            {
                                sErr = string.Format(Localization.getResStr(Configuration.paramsLimitsErrorId), iValueMin.ToString(), iValueMax.ToString());
                                return false;
                            }
                        }
                        else if (!string.IsNullOrEmpty(sValueMin))
                        {
                            if (int.Parse(sValue) < iValueMin)
                            {
                                sErr = string.Format(Localization.getResStr(Configuration.paramsMinLimitErrorId), iValueMin.ToString());
                                return false;
                            }
                        }
                        else if (!string.IsNullOrEmpty(sValueMax))
                        {
                            if (int.Parse(sValue) < iValueMin || int.Parse(tbEdit.Text) > iValueMax)
                            {
                                sErr = string.Format(Localization.getResStr(Configuration.paramsMaxLimitErrorId), iValueMax.ToString());
                                return false;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }
            }

            return true;

        }


    }
}
