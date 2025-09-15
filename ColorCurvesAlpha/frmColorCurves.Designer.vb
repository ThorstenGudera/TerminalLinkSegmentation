<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmColorCurves
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If Not IsNothing(_bitmap) Then
                _bitmap.Dispose()
            End If
            If Not IsNothing(_bSrc) Then
                _bSrc.Dispose()
            End If
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmColorCurves))
        pictureBox1 = New PictureBox()
        button11 = New Button()
        button10 = New Button()
        panel1 = New Panel()
        GroupBox2 = New GroupBox()
        btnClearBG = New Button()
        btnGetBmp = New Button()
        btnLoadBG = New Button()
        Label2 = New Label()
        Label4 = New Label()
        numericUpDown5 = New NumericUpDown()
        Label3 = New Label()
        Label1 = New Label()
        numericUpDown4 = New NumericUpDown()
        numericUpDown1 = New NumericUpDown()
        btnOrigSize = New Button()
        numericUpDown3 = New NumericUpDown()
        CheckBox5 = New CheckBox()
        numericUpDown2 = New NumericUpDown()
        CheckBox4 = New CheckBox()
        CheckBox1 = New CheckBox()
        GroupBox1 = New GroupBox()
        RadioButton2 = New RadioButton()
        RadioButton1 = New RadioButton()
        Button13 = New Button()
        CheckBox12 = New CheckBox()
        Label11 = New Label()
        GroupBox5 = New GroupBox()
        GroupBox6 = New GroupBox()
        Label12 = New Label()
        NumericUpDown11 = New NumericUpDown()
        Button7 = New Button()
        ListBox4 = New ListBox()
        Button8 = New Button()
        Label13 = New Label()
        Label14 = New Label()
        NumericUpDown12 = New NumericUpDown()
        NumericUpDown13 = New NumericUpDown()
        Label10 = New Label()
        NumericUpDown10 = New NumericUpDown()
        TextBox1 = New TextBox()
        CheckBox3 = New CheckBox()
        Button12 = New Button()
        button9 = New Button()
        panel3 = New DBPanel()
        CheckBox2 = New CheckBox()
        ComboBox2 = New ComboBox()
        CheckBoxPic = New CheckBox()
        panel2 = New Panel()
        Timer1 = New Timer(components)
        StatusStrip1 = New StatusStrip()
        ToolStripStatusLabel1 = New ToolStripStatusLabel()
        ToolStripStatusLabel2 = New ToolStripStatusLabel()
        ColorDialog1 = New ColorDialog()
        openFileDialog1 = New OpenFileDialog()
        CType(pictureBox1, ComponentModel.ISupportInitialize).BeginInit()
        panel1.SuspendLayout()
        GroupBox2.SuspendLayout()
        CType(numericUpDown5, ComponentModel.ISupportInitialize).BeginInit()
        CType(numericUpDown4, ComponentModel.ISupportInitialize).BeginInit()
        CType(numericUpDown1, ComponentModel.ISupportInitialize).BeginInit()
        CType(numericUpDown3, ComponentModel.ISupportInitialize).BeginInit()
        CType(numericUpDown2, ComponentModel.ISupportInitialize).BeginInit()
        GroupBox1.SuspendLayout()
        GroupBox5.SuspendLayout()
        GroupBox6.SuspendLayout()
        CType(NumericUpDown11, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumericUpDown12, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumericUpDown13, ComponentModel.ISupportInitialize).BeginInit()
        CType(NumericUpDown10, ComponentModel.ISupportInitialize).BeginInit()
        panel3.SuspendLayout()
        panel2.SuspendLayout()
        StatusStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' pictureBox1
        ' 
        pictureBox1.Location = New Point(0, 0)
        pictureBox1.Margin = New Padding(4, 3, 4, 3)
        pictureBox1.Name = "pictureBox1"
        pictureBox1.Size = New Size(100, 50)
        pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize
        pictureBox1.TabIndex = 0
        pictureBox1.TabStop = False
        ' 
        ' button11
        ' 
        button11.DialogResult = DialogResult.Cancel
        button11.FlatStyle = FlatStyle.System
        button11.Location = New Point(385, 769)
        button11.Margin = New Padding(4, 3, 4, 3)
        button11.Name = "button11"
        button11.Size = New Size(88, 27)
        button11.TabIndex = 14
        button11.Text = "Cancel"
        ' 
        ' button10
        ' 
        button10.DialogResult = DialogResult.OK
        button10.FlatStyle = FlatStyle.System
        button10.Location = New Point(282, 769)
        button10.Margin = New Padding(4, 3, 4, 3)
        button10.Name = "button10"
        button10.Size = New Size(88, 27)
        button10.TabIndex = 13
        button10.Text = "OK"
        ' 
        ' panel1
        ' 
        panel1.Controls.Add(GroupBox2)
        panel1.Controls.Add(CheckBox4)
        panel1.Controls.Add(CheckBox1)
        panel1.Controls.Add(GroupBox1)
        panel1.Controls.Add(Button13)
        panel1.Controls.Add(CheckBox12)
        panel1.Controls.Add(Label11)
        panel1.Controls.Add(GroupBox5)
        panel1.Controls.Add(Label10)
        panel1.Controls.Add(NumericUpDown10)
        panel1.Controls.Add(TextBox1)
        panel1.Controls.Add(CheckBox3)
        panel1.Controls.Add(Button12)
        panel1.Controls.Add(button11)
        panel1.Controls.Add(button10)
        panel1.Controls.Add(button9)
        panel1.Controls.Add(panel3)
        panel1.Controls.Add(CheckBoxPic)
        panel1.Dock = DockStyle.Left
        panel1.Location = New Point(0, 0)
        panel1.Margin = New Padding(4, 3, 4, 3)
        panel1.Name = "panel1"
        panel1.Size = New Size(487, 809)
        panel1.TabIndex = 0
        ' 
        ' GroupBox2
        ' 
        GroupBox2.Controls.Add(btnClearBG)
        GroupBox2.Controls.Add(btnGetBmp)
        GroupBox2.Controls.Add(btnLoadBG)
        GroupBox2.Controls.Add(Label2)
        GroupBox2.Controls.Add(Label4)
        GroupBox2.Controls.Add(numericUpDown5)
        GroupBox2.Controls.Add(Label3)
        GroupBox2.Controls.Add(Label1)
        GroupBox2.Controls.Add(numericUpDown4)
        GroupBox2.Controls.Add(numericUpDown1)
        GroupBox2.Controls.Add(btnOrigSize)
        GroupBox2.Controls.Add(numericUpDown3)
        GroupBox2.Controls.Add(CheckBox5)
        GroupBox2.Controls.Add(numericUpDown2)
        GroupBox2.Location = New Point(14, 570)
        GroupBox2.Name = "GroupBox2"
        GroupBox2.Size = New Size(371, 182)
        GroupBox2.TabIndex = 215
        GroupBox2.TabStop = False
        GroupBox2.Text = "CurPic"
        ' 
        ' btnClearBG
        ' 
        btnClearBG.Location = New Point(281, 20)
        btnClearBG.Name = "btnClearBG"
        btnClearBG.Size = New Size(75, 23)
        btnClearBG.TabIndex = 743
        btnClearBG.Text = "Clear BG"
        btnClearBG.UseVisualStyleBackColor = True
        ' 
        ' btnGetBmp
        ' 
        btnGetBmp.Location = New Point(195, 20)
        btnGetBmp.Margin = New Padding(4, 3, 4, 3)
        btnGetBmp.Name = "btnGetBmp"
        btnGetBmp.Size = New Size(74, 23)
        btnGetBmp.TabIndex = 742
        btnGetBmp.Text = "GetBmp"
        btnGetBmp.UseVisualStyleBackColor = True
        ' 
        ' btnLoadBG
        ' 
        btnLoadBG.Location = New Point(114, 20)
        btnLoadBG.Name = "btnLoadBG"
        btnLoadBG.Size = New Size(75, 23)
        btnLoadBG.TabIndex = 1
        btnLoadBG.Text = "from File"
        btnLoadBG.UseVisualStyleBackColor = True
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(21, 48)
        Label2.Margin = New Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(56, 15)
        Label2.TabIndex = 216
        Label2.Text = "Location:"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(256, 48)
        Label4.Margin = New Padding(4, 0, 4, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(55, 15)
        Label4.TabIndex = 218
        Label4.Text = "Rotation:"
        ' 
        ' numericUpDown5
        ' 
        numericUpDown5.DecimalPlaces = 2
        numericUpDown5.Location = New Point(270, 70)
        numericUpDown5.Margin = New Padding(4, 3, 4, 3)
        numericUpDown5.Maximum = New Decimal(New Integer() {360, 0, 0, 0})
        numericUpDown5.Minimum = New Decimal(New Integer() {360, 0, 0, Integer.MinValue})
        numericUpDown5.Name = "numericUpDown5"
        numericUpDown5.Size = New Size(86, 23)
        numericUpDown5.TabIndex = 221
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(22, 95)
        Label3.Margin = New Padding(4, 0, 4, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(30, 15)
        Label3.TabIndex = 217
        Label3.Text = "Size:"
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(21, 24)
        Label1.Name = "Label1"
        Label1.Size = New Size(87, 15)
        Label1.TabIndex = 0
        Label1.Text = "Load BG Image"
        ' 
        ' numericUpDown4
        ' 
        numericUpDown4.DecimalPlaces = 2
        numericUpDown4.Location = New Point(143, 117)
        numericUpDown4.Margin = New Padding(4, 3, 4, 3)
        numericUpDown4.Maximum = New Decimal(New Integer() {32767, 0, 0, 0})
        numericUpDown4.Name = "numericUpDown4"
        numericUpDown4.Size = New Size(86, 23)
        numericUpDown4.TabIndex = 219
        ' 
        ' numericUpDown1
        ' 
        numericUpDown1.DecimalPlaces = 2
        numericUpDown1.Location = New Point(22, 70)
        numericUpDown1.Margin = New Padding(4, 3, 4, 3)
        numericUpDown1.Maximum = New Decimal(New Integer() {32767, 0, 0, 0})
        numericUpDown1.Minimum = New Decimal(New Integer() {32768, 0, 0, Integer.MinValue})
        numericUpDown1.Name = "numericUpDown1"
        numericUpDown1.Size = New Size(86, 23)
        numericUpDown1.TabIndex = 223
        ' 
        ' btnOrigSize
        ' 
        btnOrigSize.Location = New Point(268, 113)
        btnOrigSize.Margin = New Padding(4, 3, 4, 3)
        btnOrigSize.Name = "btnOrigSize"
        btnOrigSize.Size = New Size(88, 27)
        btnOrigSize.TabIndex = 224
        btnOrigSize.Text = "OrigSize"
        btnOrigSize.UseVisualStyleBackColor = True
        ' 
        ' numericUpDown3
        ' 
        numericUpDown3.DecimalPlaces = 2
        numericUpDown3.Location = New Point(23, 117)
        numericUpDown3.Margin = New Padding(4, 3, 4, 3)
        numericUpDown3.Maximum = New Decimal(New Integer() {32767, 0, 0, 0})
        numericUpDown3.Name = "numericUpDown3"
        numericUpDown3.Size = New Size(86, 23)
        numericUpDown3.TabIndex = 222
        ' 
        ' CheckBox5
        ' 
        CheckBox5.AutoSize = True
        CheckBox5.Checked = True
        CheckBox5.CheckState = CheckState.Checked
        CheckBox5.Location = New Point(23, 147)
        CheckBox5.Margin = New Padding(4, 3, 4, 3)
        CheckBox5.Name = "CheckBox5"
        CheckBox5.Size = New Size(88, 19)
        CheckBox5.TabIndex = 225
        CheckBox5.Text = "keep aspect"
        CheckBox5.UseVisualStyleBackColor = True
        ' 
        ' numericUpDown2
        ' 
        numericUpDown2.DecimalPlaces = 2
        numericUpDown2.Location = New Point(142, 70)
        numericUpDown2.Margin = New Padding(4, 3, 4, 3)
        numericUpDown2.Maximum = New Decimal(New Integer() {32767, 0, 0, 0})
        numericUpDown2.Minimum = New Decimal(New Integer() {32768, 0, 0, Integer.MinValue})
        numericUpDown2.Name = "numericUpDown2"
        numericUpDown2.Size = New Size(86, 23)
        numericUpDown2.TabIndex = 220
        ' 
        ' CheckBox4
        ' 
        CheckBox4.AutoSize = True
        CheckBox4.Location = New Point(265, 423)
        CheckBox4.Margin = New Padding(4, 3, 4, 3)
        CheckBox4.Name = "CheckBox4"
        CheckBox4.Size = New Size(74, 19)
        CheckBox4.TabIndex = 214
        CheckBox4.Text = "do Alpha"
        CheckBox4.UseVisualStyleBackColor = True
        ' 
        ' CheckBox1
        ' 
        CheckBox1.AutoSize = True
        CheckBox1.Checked = True
        CheckBox1.CheckState = CheckState.Checked
        CheckBox1.Location = New Point(148, 425)
        CheckBox1.Margin = New Padding(4, 3, 4, 3)
        CheckBox1.Name = "CheckBox1"
        CheckBox1.Size = New Size(103, 19)
        CheckBox1.TabIndex = 214
        CheckBox1.Text = "Ignore Alpha 0"
        CheckBox1.UseVisualStyleBackColor = True
        ' 
        ' GroupBox1
        ' 
        GroupBox1.Controls.Add(RadioButton2)
        GroupBox1.Controls.Add(RadioButton1)
        GroupBox1.Location = New Point(14, 517)
        GroupBox1.Margin = New Padding(4, 3, 4, 3)
        GroupBox1.Name = "GroupBox1"
        GroupBox1.Padding = New Padding(4, 3, 4, 3)
        GroupBox1.Size = New Size(225, 47)
        GroupBox1.TabIndex = 2
        GroupBox1.TabStop = False
        GroupBox1.Text = "Mode"
        ' 
        ' RadioButton2
        ' 
        RadioButton2.AutoSize = True
        RadioButton2.Location = New Point(118, 22)
        RadioButton2.Margin = New Padding(4, 3, 4, 3)
        RadioButton2.Name = "RadioButton2"
        RadioButton2.Size = New Size(93, 19)
        RadioButton2.TabIndex = 0
        RadioButton2.Text = "LumToAlpha"
        RadioButton2.UseVisualStyleBackColor = True
        ' 
        ' RadioButton1
        ' 
        RadioButton1.AutoSize = True
        RadioButton1.Checked = True
        RadioButton1.Location = New Point(7, 22)
        RadioButton1.Margin = New Padding(4, 3, 4, 3)
        RadioButton1.Name = "RadioButton1"
        RadioButton1.Size = New Size(100, 19)
        RadioButton1.TabIndex = 0
        RadioButton1.TabStop = True
        RadioButton1.Text = "AlphaToAlpha"
        RadioButton1.UseVisualStyleBackColor = True
        ' 
        ' Button13
        ' 
        Button13.Enabled = False
        Button13.Location = New Point(89, 420)
        Button13.Margin = New Padding(4, 3, 4, 3)
        Button13.Name = "Button13"
        Button13.Size = New Size(38, 27)
        Button13.TabIndex = 213
        Button13.Text = "Col"
        Button13.UseVisualStyleBackColor = True
        ' 
        ' CheckBox12
        ' 
        CheckBox12.AutoSize = True
        CheckBox12.Checked = True
        CheckBox12.CheckState = CheckState.Checked
        CheckBox12.Location = New Point(374, 522)
        CheckBox12.Margin = New Padding(4, 3, 4, 3)
        CheckBox12.Name = "CheckBox12"
        CheckBox12.Size = New Size(67, 19)
        CheckBox12.TabIndex = 212
        CheckBox12.Text = "BG dark"
        CheckBox12.UseVisualStyleBackColor = True
        ' 
        ' Label11
        ' 
        Label11.AutoSize = True
        Label11.Location = New Point(5, 487)
        Label11.Margin = New Padding(4, 0, 4, 0)
        Label11.Name = "Label11"
        Label11.Size = New Size(468, 15)
        Label11.TabIndex = 21
        Label11.Text = "Create new Point with the Right MouseButton. Move the points with Left MouseButton."
        ' 
        ' GroupBox5
        ' 
        GroupBox5.Controls.Add(GroupBox6)
        GroupBox5.Location = New Point(332, 0)
        GroupBox5.Margin = New Padding(4, 3, 4, 3)
        GroupBox5.Name = "GroupBox5"
        GroupBox5.Padding = New Padding(4, 3, 4, 3)
        GroupBox5.Size = New Size(141, 376)
        GroupBox5.TabIndex = 20
        GroupBox5.TabStop = False
        ' 
        ' GroupBox6
        ' 
        GroupBox6.Controls.Add(Label12)
        GroupBox6.Controls.Add(NumericUpDown11)
        GroupBox6.Controls.Add(Button7)
        GroupBox6.Controls.Add(ListBox4)
        GroupBox6.Controls.Add(Button8)
        GroupBox6.Controls.Add(Label13)
        GroupBox6.Controls.Add(Label14)
        GroupBox6.Controls.Add(NumericUpDown12)
        GroupBox6.Controls.Add(NumericUpDown13)
        GroupBox6.Location = New Point(7, 16)
        GroupBox6.Margin = New Padding(4, 3, 4, 3)
        GroupBox6.Name = "GroupBox6"
        GroupBox6.Padding = New Padding(4, 3, 4, 3)
        GroupBox6.Size = New Size(121, 360)
        GroupBox6.TabIndex = 1
        GroupBox6.TabStop = False
        GroupBox6.Text = "alpha"
        ' 
        ' Label12
        ' 
        Label12.Location = New Point(9, 323)
        Label12.Margin = New Padding(4, 0, 4, 0)
        Label12.Name = "Label12"
        Label12.Size = New Size(37, 27)
        Label12.TabIndex = 8
        Label12.Text = "tens"
        Label12.TextAlign = ContentAlignment.TopCenter
        ' 
        ' NumericUpDown11
        ' 
        NumericUpDown11.DecimalPlaces = 1
        NumericUpDown11.Increment = New Decimal(New Integer() {1, 0, 0, 65536})
        NumericUpDown11.Location = New Point(56, 323)
        NumericUpDown11.Margin = New Padding(4, 3, 4, 3)
        NumericUpDown11.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        NumericUpDown11.Name = "NumericUpDown11"
        NumericUpDown11.Size = New Size(47, 23)
        NumericUpDown11.TabIndex = 7
        NumericUpDown11.Value = New Decimal(New Integer() {5, 0, 0, 65536})
        ' 
        ' Button7
        ' 
        Button7.FlatStyle = FlatStyle.System
        Button7.Location = New Point(19, 277)
        Button7.Margin = New Padding(4, 3, 4, 3)
        Button7.Name = "Button7"
        Button7.Size = New Size(88, 27)
        Button7.TabIndex = 6
        Button7.Text = "remove"
        ' 
        ' ListBox4
        ' 
        ListBox4.ItemHeight = 15
        ListBox4.Location = New Point(19, 138)
        ListBox4.Margin = New Padding(4, 3, 4, 3)
        ListBox4.Name = "ListBox4"
        ListBox4.Size = New Size(83, 124)
        ListBox4.TabIndex = 5
        ' 
        ' Button8
        ' 
        Button8.FlatStyle = FlatStyle.System
        Button8.Location = New Point(19, 92)
        Button8.Margin = New Padding(4, 3, 4, 3)
        Button8.Name = "Button8"
        Button8.Size = New Size(88, 27)
        Button8.TabIndex = 4
        Button8.Text = "add"
        ' 
        ' Label13
        ' 
        Label13.Location = New Point(9, 55)
        Label13.Margin = New Padding(4, 0, 4, 0)
        Label13.Name = "Label13"
        Label13.Size = New Size(19, 27)
        Label13.TabIndex = 3
        Label13.Text = "Y"
        Label13.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' Label14
        ' 
        Label14.Location = New Point(9, 28)
        Label14.Margin = New Padding(4, 0, 4, 0)
        Label14.Name = "Label14"
        Label14.Size = New Size(19, 27)
        Label14.TabIndex = 2
        Label14.Text = "X"
        Label14.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' NumericUpDown12
        ' 
        NumericUpDown12.Location = New Point(37, 55)
        NumericUpDown12.Margin = New Padding(4, 3, 4, 3)
        NumericUpDown12.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        NumericUpDown12.Name = "NumericUpDown12"
        NumericUpDown12.Size = New Size(65, 23)
        NumericUpDown12.TabIndex = 1
        ' 
        ' NumericUpDown13
        ' 
        NumericUpDown13.Location = New Point(37, 28)
        NumericUpDown13.Margin = New Padding(4, 3, 4, 3)
        NumericUpDown13.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        NumericUpDown13.Name = "NumericUpDown13"
        NumericUpDown13.Size = New Size(65, 23)
        NumericUpDown13.TabIndex = 0
        ' 
        ' Label10
        ' 
        Label10.AutoSize = True
        Label10.Location = New Point(163, 452)
        Label10.Margin = New Padding(4, 0, 4, 0)
        Label10.Name = "Label10"
        Label10.Size = New Size(58, 15)
        Label10.TabIndex = 20
        Label10.Text = "Tolerance"
        ' 
        ' NumericUpDown10
        ' 
        NumericUpDown10.Location = New Point(234, 451)
        NumericUpDown10.Margin = New Padding(4, 3, 4, 3)
        NumericUpDown10.Maximum = New Decimal(New Integer() {255, 0, 0, 0})
        NumericUpDown10.Name = "NumericUpDown10"
        NumericUpDown10.Size = New Size(65, 23)
        NumericUpDown10.TabIndex = 19
        ' 
        ' TextBox1
        ' 
        TextBox1.Location = New Point(14, 450)
        TextBox1.Margin = New Padding(4, 3, 4, 3)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(116, 23)
        TextBox1.TabIndex = 18
        ' 
        ' CheckBox3
        ' 
        CheckBox3.AutoSize = True
        CheckBox3.Location = New Point(14, 423)
        CheckBox3.Margin = New Padding(4, 3, 4, 3)
        CheckBox3.Name = "CheckBox3"
        CheckBox3.Size = New Size(59, 19)
        CheckBox3.TabIndex = 17
        CheckBox3.Text = "Range"
        CheckBox3.UseVisualStyleBackColor = True
        ' 
        ' Button12
        ' 
        Button12.Location = New Point(148, 395)
        Button12.Margin = New Padding(4, 3, 4, 3)
        Button12.Name = "Button12"
        Button12.Size = New Size(152, 27)
        Button12.TabIndex = 16
        Button12.Text = "Reset Points"
        Button12.UseVisualStyleBackColor = True
        ' 
        ' button9
        ' 
        button9.FlatStyle = FlatStyle.System
        button9.Location = New Point(264, 517)
        button9.Margin = New Padding(4, 3, 4, 3)
        button9.Name = "button9"
        button9.Size = New Size(88, 27)
        button9.TabIndex = 12
        button9.Text = "draw"
        ' 
        ' panel3
        ' 
        panel3.Controls.Add(CheckBox2)
        panel3.Controls.Add(ComboBox2)
        panel3.Location = New Point(-19, 28)
        panel3.Margin = New Padding(4, 3, 4, 3)
        panel3.Name = "panel3"
        panel3.Size = New Size(344, 351)
        panel3.TabIndex = 0
        ' 
        ' CheckBox2
        ' 
        CheckBox2.AutoSize = True
        CheckBox2.Checked = True
        CheckBox2.CheckState = CheckState.Checked
        CheckBox2.Location = New Point(152, 318)
        CheckBox2.Margin = New Padding(4, 3, 4, 3)
        CheckBox2.Name = "CheckBox2"
        CheckBox2.Size = New Size(74, 19)
        CheckBox2.TabIndex = 17
        CheckBox2.Text = "sortArray"
        CheckBox2.UseVisualStyleBackColor = True
        ' 
        ' ComboBox2
        ' 
        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox2.Items.AddRange(New Object() {"All", "red", "green", "blue"})
        ComboBox2.Location = New Point(253, 316)
        ComboBox2.Margin = New Padding(4, 3, 4, 3)
        ComboBox2.Name = "ComboBox2"
        ComboBox2.Size = New Size(65, 23)
        ComboBox2.TabIndex = 16
        ' 
        ' CheckBoxPic
        ' 
        CheckBoxPic.AutoSize = True
        CheckBoxPic.Location = New Point(336, 452)
        CheckBoxPic.Margin = New Padding(4, 3, 4, 3)
        CheckBoxPic.Name = "CheckBoxPic"
        CheckBoxPic.Size = New Size(90, 19)
        CheckBoxPic.TabIndex = 16
        CheckBoxPic.Text = "Pic-Modus2"
        CheckBoxPic.UseVisualStyleBackColor = True
        ' 
        ' panel2
        ' 
        panel2.Controls.Add(pictureBox1)
        panel2.Dock = DockStyle.Fill
        panel2.Location = New Point(487, 0)
        panel2.Margin = New Padding(4, 3, 4, 3)
        panel2.Name = "panel2"
        panel2.Size = New Size(634, 809)
        panel2.TabIndex = 1
        ' 
        ' Timer1
        ' 
        Timer1.Interval = 50
        ' 
        ' StatusStrip1
        ' 
        StatusStrip1.Items.AddRange(New ToolStripItem() {ToolStripStatusLabel1, ToolStripStatusLabel2})
        StatusStrip1.Location = New Point(0, 809)
        StatusStrip1.Name = "StatusStrip1"
        StatusStrip1.Padding = New Padding(1, 0, 16, 0)
        StatusStrip1.Size = New Size(1121, 22)
        StatusStrip1.TabIndex = 17
        StatusStrip1.Text = "StatusStrip1"
        ' 
        ' ToolStripStatusLabel1
        ' 
        ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        ToolStripStatusLabel1.Size = New Size(35, 17)
        ToolStripStatusLabel1.Text = "Hello"
        ' 
        ' ToolStripStatusLabel2
        ' 
        ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        ToolStripStatusLabel2.Size = New Size(16, 17)
        ToolStripStatusLabel2.Text = "   "
        ' 
        ' ColorDialog1
        ' 
        ColorDialog1.AnyColor = True
        ColorDialog1.FullOpen = True
        ' 
        ' openFileDialog1
        ' 
        openFileDialog1.FileName = "openFileDialog1"
        openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png"
        ' 
        ' frmColorCurves
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1121, 831)
        Controls.Add(panel2)
        Controls.Add(panel1)
        Controls.Add(StatusStrip1)
        FormBorderStyle = FormBorderStyle.Fixed3D
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        Name = "frmColorCurves"
        StartPosition = FormStartPosition.CenterScreen
        Text = "ColorCurves"
        CType(pictureBox1, ComponentModel.ISupportInitialize).EndInit()
        panel1.ResumeLayout(False)
        panel1.PerformLayout()
        GroupBox2.ResumeLayout(False)
        GroupBox2.PerformLayout()
        CType(numericUpDown5, ComponentModel.ISupportInitialize).EndInit()
        CType(numericUpDown4, ComponentModel.ISupportInitialize).EndInit()
        CType(numericUpDown1, ComponentModel.ISupportInitialize).EndInit()
        CType(numericUpDown3, ComponentModel.ISupportInitialize).EndInit()
        CType(numericUpDown2, ComponentModel.ISupportInitialize).EndInit()
        GroupBox1.ResumeLayout(False)
        GroupBox1.PerformLayout()
        GroupBox5.ResumeLayout(False)
        GroupBox6.ResumeLayout(False)
        CType(NumericUpDown11, ComponentModel.ISupportInitialize).EndInit()
        CType(NumericUpDown12, ComponentModel.ISupportInitialize).EndInit()
        CType(NumericUpDown13, ComponentModel.ISupportInitialize).EndInit()
        CType(NumericUpDown10, ComponentModel.ISupportInitialize).EndInit()
        panel3.ResumeLayout(False)
        panel3.PerformLayout()
        panel2.ResumeLayout(False)
        panel2.PerformLayout()
        StatusStrip1.ResumeLayout(False)
        StatusStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents button9 As System.Windows.Forms.Button
    Friend WithEvents panel3 As DBPanel
    Friend WithEvents panel2 As System.Windows.Forms.Panel
    Friend WithEvents panel1 As System.Windows.Forms.Panel
    Friend WithEvents button10 As System.Windows.Forms.Button
    Friend WithEvents button11 As System.Windows.Forms.Button
    Friend WithEvents pictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents CheckBoxPic As System.Windows.Forms.CheckBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown11 As System.Windows.Forms.NumericUpDown
    Friend WithEvents Button7 As System.Windows.Forms.Button
    Friend WithEvents ListBox4 As System.Windows.Forms.ListBox
    Friend WithEvents Button8 As System.Windows.Forms.Button
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown12 As System.Windows.Forms.NumericUpDown
    Friend WithEvents NumericUpDown13 As System.Windows.Forms.NumericUpDown
    Friend WithEvents ComboBox2 As System.Windows.Forms.ComboBox
    Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
    Friend WithEvents Button12 As System.Windows.Forms.Button
    Public WithEvents CheckBox3 As System.Windows.Forms.CheckBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Public WithEvents NumericUpDown10 As System.Windows.Forms.NumericUpDown
    Public WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents CheckBox12 As CheckBox
    Friend WithEvents Button13 As Button
    Friend WithEvents ColorDialog1 As ColorDialog
    Friend WithEvents GroupBox1 As GroupBox
    Public WithEvents RadioButton2 As RadioButton
    Public WithEvents RadioButton1 As RadioButton
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Public WithEvents CheckBox1 As CheckBox
    Public WithEvents CheckBox4 As CheckBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnLoadBG As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnGetBmp As Button
    Friend WithEvents btnClearBG As Button
    Private WithEvents openFileDialog1 As OpenFileDialog
    Public WithEvents Label2 As Label
    Public WithEvents Label3 As Label
    Public WithEvents numericUpDown4 As NumericUpDown
    Public WithEvents numericUpDown1 As NumericUpDown
    Public WithEvents numericUpDown3 As NumericUpDown
    Public WithEvents CheckBox5 As CheckBox
    Public WithEvents numericUpDown2 As NumericUpDown
    Public WithEvents Label4 As Label
    Public WithEvents numericUpDown5 As NumericUpDown
    Public WithEvents btnOrigSize As Button
End Class
