namespace AvoidAGrabCutEasy
{
    partial class frmChainCode
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            label19 = new Label();
            numWHScribbles = new NumericUpDown();
            panel5 = new Panel();
            btnColor = new Button();
            label8 = new Label();
            btnRemLastScribbles2 = new Button();
            cbDraw = new CheckBox();
            numDraw = new NumericUpDown();
            panel3 = new Panel();
            btnRemLastScribbles = new Button();
            cbErase = new CheckBox();
            numErase = new NumericUpDown();
            panel2 = new Panel();
            numReplace = new NumericUpDown();
            btnReplace = new Button();
            label3 = new Label();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            button10 = new Button();
            button8 = new Button();
            button2 = new Button();
            btnRedo = new Button();
            btnUndo = new Button();
            panel4 = new Panel();
            numChainTolerance = new NumericUpDown();
            button1 = new Button();
            label4 = new Label();
            label5 = new Label();
            panel1 = new Panel();
            numDivisor = new NumericUpDown();
            btnGrad = new Button();
            cmbGradMode = new ComboBox();
            label2 = new Label();
            label1 = new Label();
            btnCancel = new Button();
            btnOK = new Button();
            splitContainer2 = new SplitContainer();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            label7 = new Label();
            label6 = new Label();
            cbSelSingleClick = new CheckBox();
            btnSelNone = new Button();
            btnClearPaths = new Button();
            btnSelAll = new Button();
            cbRestrict = new CheckBox();
            numRestrict = new NumericUpDown();
            checkedListBox1 = new CheckedListBox();
            saveFileDialog1 = new SaveFileDialog();
            toolTip1 = new ToolTip(components);
            Timer3 = new System.Windows.Forms.Timer(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            colorDialog1 = new ColorDialog();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numWHScribbles).BeginInit();
            panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDraw).BeginInit();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numErase).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numReplace).BeginInit();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numChainTolerance).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDivisor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRestrict).BeginInit();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1, toolStripStatusLabel2 });
            statusStrip1.Location = new Point(0, 737);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1148, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 16);
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(118, 17);
            toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(label19);
            splitContainer1.Panel1.Controls.Add(numWHScribbles);
            splitContainer1.Panel1.Controls.Add(panel5);
            splitContainer1.Panel1.Controls.Add(panel3);
            splitContainer1.Panel1.Controls.Add(panel2);
            splitContainer1.Panel1.Controls.Add(Label20);
            splitContainer1.Panel1.Controls.Add(cmbZoom);
            splitContainer1.Panel1.Controls.Add(cbBGColor);
            splitContainer1.Panel1.Controls.Add(button10);
            splitContainer1.Panel1.Controls.Add(button8);
            splitContainer1.Panel1.Controls.Add(button2);
            splitContainer1.Panel1.Controls.Add(btnRedo);
            splitContainer1.Panel1.Controls.Add(btnUndo);
            splitContainer1.Panel1.Controls.Add(panel4);
            splitContainer1.Panel1.Controls.Add(panel1);
            splitContainer1.Panel1.Controls.Add(btnCancel);
            splitContainer1.Panel1.Controls.Add(btnOK);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1148, 737);
            splitContainer1.SplitterDistance = 181;
            splitContainer1.TabIndex = 1;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(843, 143);
            label19.Margin = new Padding(4, 0, 4, 0);
            label19.Name = "label19";
            label19.Size = new Size(37, 15);
            label19.TabIndex = 684;
            label19.Text = "width";
            // 
            // numWHScribbles
            // 
            numWHScribbles.Location = new Point(888, 140);
            numWHScribbles.Margin = new Padding(4, 3, 4, 3);
            numWHScribbles.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numWHScribbles.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numWHScribbles.Name = "numWHScribbles";
            numWHScribbles.Size = new Size(52, 23);
            numWHScribbles.TabIndex = 683;
            numWHScribbles.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // panel5
            // 
            panel5.Controls.Add(btnColor);
            panel5.Controls.Add(label8);
            panel5.Controls.Add(btnRemLastScribbles2);
            panel5.Controls.Add(cbDraw);
            panel5.Controls.Add(numDraw);
            panel5.Location = new Point(342, 90);
            panel5.Name = "panel5";
            panel5.Size = new Size(314, 77);
            panel5.TabIndex = 682;
            // 
            // btnColor
            // 
            btnColor.Location = new Point(6, 36);
            btnColor.Name = "btnColor";
            btnColor.Size = new Size(75, 23);
            btnColor.TabIndex = 654;
            btnColor.Text = "Color";
            btnColor.UseVisualStyleBackColor = true;
            btnColor.Click += btnColor_Click;
            // 
            // label8
            // 
            label8.BackColor = Color.Black;
            label8.BorderStyle = BorderStyle.FixedSingle;
            label8.Location = new Point(122, 39);
            label8.Name = "label8";
            label8.Size = new Size(88, 23);
            label8.TabIndex = 653;
            label8.Text = "    ";
            // 
            // btnRemLastScribbles2
            // 
            btnRemLastScribbles2.Location = new Point(244, 4);
            btnRemLastScribbles2.Margin = new Padding(4, 3, 4, 3);
            btnRemLastScribbles2.Name = "btnRemLastScribbles2";
            btnRemLastScribbles2.Size = new Size(63, 27);
            btnRemLastScribbles2.TabIndex = 652;
            btnRemLastScribbles2.Text = "rem last";
            btnRemLastScribbles2.UseVisualStyleBackColor = true;
            btnRemLastScribbles2.Click += btnRemLastScribbles2_Click;
            // 
            // cbDraw
            // 
            cbDraw.AutoSize = true;
            cbDraw.Location = new Point(6, 8);
            cbDraw.Name = "cbDraw";
            cbDraw.Size = new Size(107, 19);
            cbDraw.TabIndex = 651;
            cbDraw.Text = "Draw w. Mouse";
            cbDraw.UseVisualStyleBackColor = true;
            cbDraw.CheckedChanged += cbDraw_CheckedChanged;
            // 
            // numDraw
            // 
            numDraw.Location = new Point(122, 7);
            numDraw.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numDraw.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDraw.Name = "numDraw";
            numDraw.Size = new Size(88, 23);
            numDraw.TabIndex = 650;
            numDraw.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // panel3
            // 
            panel3.Controls.Add(btnRemLastScribbles);
            panel3.Controls.Add(cbErase);
            panel3.Controls.Add(numErase);
            panel3.Location = new Point(14, 123);
            panel3.Name = "panel3";
            panel3.Size = new Size(314, 44);
            panel3.TabIndex = 682;
            // 
            // btnRemLastScribbles
            // 
            btnRemLastScribbles.Location = new Point(244, 4);
            btnRemLastScribbles.Margin = new Padding(4, 3, 4, 3);
            btnRemLastScribbles.Name = "btnRemLastScribbles";
            btnRemLastScribbles.Size = new Size(63, 27);
            btnRemLastScribbles.TabIndex = 652;
            btnRemLastScribbles.Text = "rem last";
            btnRemLastScribbles.UseVisualStyleBackColor = true;
            btnRemLastScribbles.Click += btnRemLastScribbles_Click;
            // 
            // cbErase
            // 
            cbErase.AutoSize = true;
            cbErase.Location = new Point(6, 8);
            cbErase.Name = "cbErase";
            cbErase.Size = new Size(107, 19);
            cbErase.TabIndex = 651;
            cbErase.Text = "Erase w. Mouse";
            cbErase.UseVisualStyleBackColor = true;
            cbErase.CheckedChanged += cbErase_CheckedChanged;
            // 
            // numErase
            // 
            numErase.Location = new Point(119, 6);
            numErase.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numErase.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numErase.Name = "numErase";
            numErase.Size = new Size(88, 23);
            numErase.TabIndex = 650;
            numErase.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // panel2
            // 
            panel2.Controls.Add(numReplace);
            panel2.Controls.Add(btnReplace);
            panel2.Controls.Add(label3);
            panel2.Location = new Point(12, 73);
            panel2.Name = "panel2";
            panel2.Size = new Size(314, 44);
            panel2.TabIndex = 682;
            // 
            // numReplace
            // 
            numReplace.Location = new Point(119, 7);
            numReplace.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numReplace.Name = "numReplace";
            numReplace.Size = new Size(88, 23);
            numReplace.TabIndex = 650;
            numReplace.Value = new decimal(new int[] { 71, 0, 0, 0 });
            // 
            // btnReplace
            // 
            btnReplace.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReplace.ForeColor = SystemColors.ControlText;
            btnReplace.Location = new Point(221, 4);
            btnReplace.Margin = new Padding(4, 3, 4, 3);
            btnReplace.Name = "btnReplace";
            btnReplace.Size = new Size(88, 27);
            btnReplace.TabIndex = 647;
            btnReplace.Text = "Go";
            btnReplace.UseVisualStyleBackColor = true;
            btnReplace.Click += btnReplace_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 11);
            label3.Name = "label3";
            label3.Size = new Size(78, 15);
            label3.TabIndex = 0;
            label3.Text = "Set BG Transp";
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(978, 90);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 681;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(1047, 87);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 680;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(961, 15);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 679;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // button10
            // 
            button10.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(1047, 47);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 678;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(952, 47);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 677;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(1047, 10);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 676;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // btnRedo
            // 
            btnRedo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(852, 10);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 651;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(756, 10);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 650;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // panel4
            // 
            panel4.BorderStyle = BorderStyle.FixedSingle;
            panel4.Controls.Add(numChainTolerance);
            panel4.Controls.Add(button1);
            panel4.Controls.Add(label4);
            panel4.Controls.Add(label5);
            panel4.Location = new Point(342, 10);
            panel4.Name = "panel4";
            panel4.Size = new Size(312, 70);
            panel4.TabIndex = 649;
            // 
            // numChainTolerance
            // 
            numChainTolerance.Location = new Point(167, 6);
            numChainTolerance.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numChainTolerance.Name = "numChainTolerance";
            numChainTolerance.Size = new Size(88, 23);
            numChainTolerance.TabIndex = 650;
            numChainTolerance.Value = new decimal(new int[] { 254, 0, 0, 0 });
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.ForeColor = SystemColors.ControlText;
            button1.Location = new Point(218, 35);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(88, 27);
            button1.TabIndex = 647;
            button1.Text = "Go";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(104, 8);
            label4.Name = "label4";
            label4.Size = new Size(57, 15);
            label4.TabIndex = 1;
            label4.Text = "Tolerance";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(5, 8);
            label5.Name = "label5";
            label5.Size = new Size(84, 15);
            label5.TabIndex = 0;
            label5.Text = "GetChainCode";
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(numDivisor);
            panel1.Controls.Add(btnGrad);
            panel1.Controls.Add(cmbGradMode);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(14, 10);
            panel1.Name = "panel1";
            panel1.Size = new Size(312, 57);
            panel1.TabIndex = 649;
            // 
            // numDivisor
            // 
            numDivisor.Location = new Point(116, 29);
            numDivisor.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numDivisor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDivisor.Name = "numDivisor";
            numDivisor.Size = new Size(88, 23);
            numDivisor.TabIndex = 650;
            numDivisor.Value = new decimal(new int[] { 16, 0, 0, 0 });
            // 
            // btnGrad
            // 
            btnGrad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnGrad.ForeColor = SystemColors.ControlText;
            btnGrad.Location = new Point(218, 25);
            btnGrad.Margin = new Padding(4, 3, 4, 3);
            btnGrad.Name = "btnGrad";
            btnGrad.Size = new Size(88, 27);
            btnGrad.TabIndex = 647;
            btnGrad.Text = "Go";
            btnGrad.UseVisualStyleBackColor = true;
            btnGrad.Click += btnGrad_Click;
            // 
            // cmbGradMode
            // 
            cmbGradMode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGradMode.FormattingEnabled = true;
            cmbGradMode.Location = new Point(83, 4);
            cmbGradMode.Name = "cmbGradMode";
            cmbGradMode.Size = new Size(121, 23);
            cmbGradMode.TabIndex = 2;
            cmbGradMode.SelectedIndexChanged += cmbGradMode_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(5, 33);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 1;
            label2.Text = "Divisor";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(5, 8);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 0;
            label1.Text = "GradMode";
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(1047, 136);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 647;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(954, 136);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 648;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(helplineRulerCtrl1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(label7);
            splitContainer2.Panel2.Controls.Add(label6);
            splitContainer2.Panel2.Controls.Add(cbSelSingleClick);
            splitContainer2.Panel2.Controls.Add(btnSelNone);
            splitContainer2.Panel2.Controls.Add(btnClearPaths);
            splitContainer2.Panel2.Controls.Add(btnSelAll);
            splitContainer2.Panel2.Controls.Add(cbRestrict);
            splitContainer2.Panel2.Controls.Add(numRestrict);
            splitContainer2.Panel2.Controls.Add(checkedListBox1);
            splitContainer2.Size = new Size(1148, 552);
            splitContainer2.SplitterDistance = 844;
            splitContainer2.TabIndex = 0;
            // 
            // helplineRulerCtrl1
            // 
            helplineRulerCtrl1.Bmp = null;
            helplineRulerCtrl1.Dock = DockStyle.Fill;
            helplineRulerCtrl1.DontDoLayout = false;
            helplineRulerCtrl1.DontHandleDoubleClick = false;
            helplineRulerCtrl1.DontPaintBaseImg = false;
            helplineRulerCtrl1.DontProcDoubleClick = false;
            helplineRulerCtrl1.DrawModeClipped = false;
            helplineRulerCtrl1.IgnoreZoom = false;
            helplineRulerCtrl1.Location = new Point(0, 0);
            helplineRulerCtrl1.Margin = new Padding(4, 3, 4, 3);
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(844, 552);
            helplineRulerCtrl1.TabIndex = 0;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(143, 261);
            label7.Name = "label7";
            label7.Size = new Size(16, 15);
            label7.TabIndex = 651;
            label7.Text = "...";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(13, 261);
            label6.Name = "label6";
            label6.Size = new Size(16, 15);
            label6.TabIndex = 651;
            label6.Text = "...";
            // 
            // cbSelSingleClick
            // 
            cbSelSingleClick.AutoSize = true;
            cbSelSingleClick.Location = new Point(12, 322);
            cbSelSingleClick.Name = "cbSelSingleClick";
            cbSelSingleClick.Size = new Size(131, 19);
            cbSelSingleClick.TabIndex = 1;
            cbSelSingleClick.Text = "SelectOnSingleClick";
            cbSelSingleClick.UseVisualStyleBackColor = true;
            cbSelSingleClick.CheckedChanged += cbSelSingleClick_CheckedChanged;
            // 
            // btnSelNone
            // 
            btnSelNone.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelNone.ForeColor = SystemColors.ControlText;
            btnSelNone.Location = new Point(199, 347);
            btnSelNone.Margin = new Padding(4, 3, 4, 3);
            btnSelNone.Name = "btnSelNone";
            btnSelNone.Size = new Size(88, 27);
            btnSelNone.TabIndex = 647;
            btnSelNone.Text = "UnselectAll";
            btnSelNone.UseVisualStyleBackColor = true;
            btnSelNone.Click += btnSelNone_Click;
            // 
            // btnClearPaths
            // 
            btnClearPaths.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearPaths.ForeColor = SystemColors.ControlText;
            btnClearPaths.Location = new Point(13, 380);
            btnClearPaths.Margin = new Padding(4, 3, 4, 3);
            btnClearPaths.Name = "btnClearPaths";
            btnClearPaths.Size = new Size(88, 27);
            btnClearPaths.TabIndex = 647;
            btnClearPaths.Text = "ClearPaths";
            btnClearPaths.UseVisualStyleBackColor = true;
            btnClearPaths.Click += btnClearPaths_Click;
            // 
            // btnSelAll
            // 
            btnSelAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelAll.ForeColor = SystemColors.ControlText;
            btnSelAll.Location = new Point(12, 347);
            btnSelAll.Margin = new Padding(4, 3, 4, 3);
            btnSelAll.Name = "btnSelAll";
            btnSelAll.Size = new Size(88, 27);
            btnSelAll.TabIndex = 647;
            btnSelAll.Text = "SelectAll";
            btnSelAll.UseVisualStyleBackColor = true;
            btnSelAll.Click += btnSelAll_Click;
            // 
            // cbRestrict
            // 
            cbRestrict.AutoSize = true;
            cbRestrict.Location = new Point(12, 297);
            cbRestrict.Name = "cbRestrict";
            cbRestrict.Size = new Size(158, 19);
            cbRestrict.TabIndex = 1;
            cbRestrict.Text = "restrict amount chains to";
            cbRestrict.UseVisualStyleBackColor = true;
            // 
            // numRestrict
            // 
            numRestrict.Location = new Point(198, 296);
            numRestrict.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numRestrict.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRestrict.Name = "numRestrict";
            numRestrict.Size = new Size(88, 23);
            numRestrict.TabIndex = 650;
            numRestrict.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // checkedListBox1
            // 
            checkedListBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(13, 14);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(273, 238);
            checkedListBox1.TabIndex = 0;
            checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // Timer3
            // 
            Timer3.Interval = 500;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // colorDialog1
            // 
            colorDialog1.AnyColor = true;
            colorDialog1.FullOpen = true;
            // 
            // frmChainCode
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1148, 759);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Name = "frmChainCode";
            Text = "frmChainCode";
            FormClosing += frmChainCode_FormClosing;
            Load += frmChainCode_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numWHScribbles).EndInit();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDraw).EndInit();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numErase).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numReplace).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numChainTolerance).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDivisor).EndInit();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numRestrict).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private CheckedListBox checkedListBox1;
        private Panel panel1;
        private NumericUpDown numDivisor;
        private ComboBox cmbGradMode;
        private Label label2;
        private Label label1;
        private Button btnCancel;
        private Button btnOK;
        private Button btnGrad;
        private Button btnRedo;
        private Button btnUndo;
        internal Label Label20;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        private Button button10;
        private Button button8;
        internal Button button2;
        private SaveFileDialog saveFileDialog1;
        private ToolTip toolTip1;
        internal System.Windows.Forms.Timer Timer3;
        private Panel panel2;
        private NumericUpDown numReplace;
        private Button btnReplace;
        private Label label3;
        private Panel panel3;
        private NumericUpDown numErase;
        private CheckBox cbErase;
        private Panel panel4;
        private NumericUpDown numChainTolerance;
        private Button button1;
        private Label label4;
        private Label label5;
        private CheckBox cbSelSingleClick;
        private Button btnSelNone;
        private Button btnClearPaths;
        private Button btnSelAll;
        private CheckBox cbRestrict;
        private NumericUpDown numRestrict;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private Label label7;
        private Label label6;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private Label label19;
        internal NumericUpDown numWHScribbles;
        private Button btnRemLastScribbles;
        private Panel panel5;
        private Button btnColor;
        private Label label8;
        private Button btnRemLastScribbles2;
        private CheckBox cbDraw;
        private NumericUpDown numDraw;
        internal ColorDialog colorDialog1;
    }
}