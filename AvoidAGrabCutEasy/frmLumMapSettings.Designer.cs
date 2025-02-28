namespace AvoidAGrabCutEasy
{
    partial class frmLumMapSettings
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
            btnCancel = new Button();
            btnOK = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            numF1 = new NumericUpDown();
            numTh = new NumericUpDown();
            numF2 = new NumericUpDown();
            label4 = new Label();
            numExp1 = new NumericUpDown();
            label5 = new Label();
            numExp2 = new NumericUpDown();
            label6 = new Label();
            numThMultiplier = new NumericUpDown();
            cbAuto = new CheckBox();
            splitContainer1 = new SplitContainer();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            cbAppSettingsOnly = new CheckBox();
            button10 = new Button();
            button8 = new Button();
            button2 = new Button();
            btnRedo = new Button();
            btnUndo = new Button();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            rbApp = new RadioButton();
            rbImage = new RadioButton();
            groupBox4 = new GroupBox();
            numPostBlurKrrnl = new NumericUpDown();
            cbPostBlur = new CheckBox();
            label22 = new Label();
            label10 = new Label();
            numIGGDivisor = new NumericUpDown();
            label21 = new Label();
            label18 = new Label();
            numIGGAlpha = new NumericUpDown();
            btnInvGaussGrad = new Button();
            numIGGKernel = new NumericUpDown();
            rbMorph = new RadioButton();
            rbIGG = new RadioButton();
            groupBox3 = new GroupBox();
            label16 = new Label();
            label15 = new Label();
            numDistWeight = new NumericUpDown();
            btnBlur = new Button();
            numKernel = new NumericUpDown();
            groupBox2 = new GroupBox();
            pictureBox1 = new PictureBox();
            cbSecondColor = new CheckBox();
            label8 = new Label();
            numValDst2 = new NumericUpDown();
            numValSrc2 = new NumericUpDown();
            label7 = new Label();
            label17 = new Label();
            numValDst = new NumericUpDown();
            numValSrc = new NumericUpDown();
            btnColors = new Button();
            panel1 = new Panel();
            groupBox1 = new GroupBox();
            label9 = new Label();
            rbGreaterThan = new RadioButton();
            rbLessThan = new RadioButton();
            cbDoFirstMult = new CheckBox();
            cbDoSecondMult = new CheckBox();
            toolTip1 = new ToolTip(components);
            saveFileDialog1 = new SaveFileDialog();
            backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            Timer3 = new System.Windows.Forms.Timer(components);
            backgroundWorker4 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)numF1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTh).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numF2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numExp1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numExp2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numThMultiplier).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            statusStrip1.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPostBlurKrrnl).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIGGDivisor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIGGAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIGGKernel).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDistWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numKernel).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numValDst2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numValSrc2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numValDst).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numValSrc).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(326, 758);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 732;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(229, 758);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 733;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 45);
            label1.Name = "label1";
            label1.Size = new Size(44, 15);
            label1.TabIndex = 734;
            label1.Text = "factor1";
            toolTip1.SetToolTip(label1, "Greater factors lead to a result with less pixels displayed.");
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 105);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 734;
            label2.Text = "threshold";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 157);
            label3.Name = "label3";
            label3.Size = new Size(44, 15);
            label3.TabIndex = 734;
            label3.Text = "factor2";
            toolTip1.SetToolTip(label3, "Greater factors lead to a result with less pixels displayed.");
            // 
            // numF1
            // 
            numF1.DecimalPlaces = 4;
            numF1.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numF1.Location = new Point(80, 43);
            numF1.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numF1.Name = "numF1";
            numF1.Size = new Size(88, 23);
            numF1.TabIndex = 735;
            toolTip1.SetToolTip(numF1, "Greater factors lead to a result with less pixels displayed.");
            numF1.Value = new decimal(new int[] { 25, 0, 0, 65536 });
            // 
            // numTh
            // 
            numTh.DecimalPlaces = 4;
            numTh.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numTh.Location = new Point(80, 103);
            numTh.Name = "numTh";
            numTh.Size = new Size(88, 23);
            numTh.TabIndex = 735;
            numTh.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // numF2
            // 
            numF2.DecimalPlaces = 4;
            numF2.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numF2.Location = new Point(80, 154);
            numF2.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numF2.Name = "numF2";
            numF2.Size = new Size(88, 23);
            numF2.TabIndex = 735;
            toolTip1.SetToolTip(numF2, "Greater factors lead to a result with less pixels displayed.");
            numF2.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(193, 45);
            label4.Name = "label4";
            label4.Size = new Size(63, 15);
            label4.TabIndex = 734;
            label4.Text = "exponent1";
            // 
            // numExp1
            // 
            numExp1.DecimalPlaces = 4;
            numExp1.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numExp1.Location = new Point(283, 43);
            numExp1.Name = "numExp1";
            numExp1.Size = new Size(88, 23);
            numExp1.TabIndex = 735;
            numExp1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(193, 157);
            label5.Name = "label5";
            label5.Size = new Size(63, 15);
            label5.TabIndex = 734;
            label5.Text = "exponent2";
            // 
            // numExp2
            // 
            numExp2.DecimalPlaces = 4;
            numExp2.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numExp2.Location = new Point(283, 154);
            numExp2.Name = "numExp2";
            numExp2.Size = new Size(88, 23);
            numExp2.TabIndex = 735;
            numExp2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(193, 105);
            label6.Name = "label6";
            label6.Size = new Size(86, 15);
            label6.TabIndex = 734;
            label6.Text = "multiplier 10^-";
            // 
            // numThMultiplier
            // 
            numThMultiplier.DecimalPlaces = 4;
            numThMultiplier.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numThMultiplier.Location = new Point(283, 103);
            numThMultiplier.Name = "numThMultiplier";
            numThMultiplier.Size = new Size(88, 23);
            numThMultiplier.TabIndex = 735;
            numThMultiplier.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // cbAuto
            // 
            cbAuto.AutoSize = true;
            cbAuto.Location = new Point(80, 127);
            cbAuto.Name = "cbAuto";
            cbAuto.Size = new Size(50, 19);
            cbAuto.TabIndex = 736;
            cbAuto.Text = "auto";
            toolTip1.SetToolTip(cbAuto, "Split the real range of values at the specified threshold ");
            cbAuto.UseVisualStyleBackColor = true;
            cbAuto.CheckedChanged += cbAuto_CheckedChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(helplineRulerCtrl1);
            splitContainer1.Panel1.Controls.Add(statusStrip1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(cbAppSettingsOnly);
            splitContainer1.Panel2.Controls.Add(button10);
            splitContainer1.Panel2.Controls.Add(button8);
            splitContainer1.Panel2.Controls.Add(button2);
            splitContainer1.Panel2.Controls.Add(btnRedo);
            splitContainer1.Panel2.Controls.Add(btnUndo);
            splitContainer1.Panel2.Controls.Add(Label20);
            splitContainer1.Panel2.Controls.Add(cmbZoom);
            splitContainer1.Panel2.Controls.Add(cbBGColor);
            splitContainer1.Panel2.Controls.Add(rbApp);
            splitContainer1.Panel2.Controls.Add(rbImage);
            splitContainer1.Panel2.Controls.Add(groupBox4);
            splitContainer1.Panel2.Controls.Add(groupBox3);
            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Panel2.Controls.Add(groupBox1);
            splitContainer1.Panel2.Controls.Add(btnOK);
            splitContainer1.Panel2.Controls.Add(btnCancel);
            splitContainer1.Size = new Size(1255, 797);
            splitContainer1.SplitterDistance = 824;
            splitContainer1.TabIndex = 737;
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
            helplineRulerCtrl1.DrawPixelated = false;
            helplineRulerCtrl1.IgnoreZoom = false;
            helplineRulerCtrl1.Location = new Point(0, 0);
            helplineRulerCtrl1.Margin = new Padding(4, 3, 4, 3);
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(824, 762);
            helplineRulerCtrl1.TabIndex = 1;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripProgressBar1, toolStripStatusLabel3 });
            statusStrip1.Location = new Point(0, 762);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(824, 35);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("Segoe UI", 15.75F);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(61, 30);
            toolStripStatusLabel1.Text = "Hallo";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.AutoSize = false;
            toolStripStatusLabel2.Font = new Font("Segoe UI", 15.75F);
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(49, 30);
            toolStripStatusLabel2.Text = "    ";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(250, 29);
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Font = new Font("Segoe UI", 15.75F);
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(61, 30);
            toolStripStatusLabel3.Text = "Hallo";
            // 
            // cbAppSettingsOnly
            // 
            cbAppSettingsOnly.AutoSize = true;
            cbAppSettingsOnly.Location = new Point(13, 597);
            cbAppSettingsOnly.Name = "cbAppSettingsOnly";
            cbAppSettingsOnly.Size = new Size(174, 19);
            cbAppSettingsOnly.TabIndex = 750;
            cbAppSettingsOnly.Text = "do Application settings only";
            toolTip1.SetToolTip(cbAppSettingsOnly, "Dont set the image, but the factors and threshold");
            cbAppSettingsOnly.UseVisualStyleBackColor = true;
            // 
            // button10
            // 
            button10.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(319, 677);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 748;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(224, 677);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 747;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(318, 602);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 746;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // btnRedo
            // 
            btnRedo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRedo.Enabled = false;
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(320, 640);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 745;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUndo.Enabled = false;
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(224, 640);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 744;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(162, 723);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 743;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(230, 719);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 742;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(18, 721);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 741;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // rbApp
            // 
            rbApp.AutoSize = true;
            rbApp.Checked = true;
            rbApp.Location = new Point(18, 372);
            rbApp.Name = "rbApp";
            rbApp.Size = new Size(86, 19);
            rbApp.TabIndex = 740;
            rbApp.TabStop = true;
            rbApp.Text = "Application";
            rbApp.UseVisualStyleBackColor = true;
            rbApp.CheckedChanged += rbApp_CheckedChanged;
            // 
            // rbImage
            // 
            rbImage.AutoSize = true;
            rbImage.Location = new Point(18, 12);
            rbImage.Name = "rbImage";
            rbImage.Size = new Size(58, 19);
            rbImage.TabIndex = 740;
            rbImage.Text = "Image";
            rbImage.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(numPostBlurKrrnl);
            groupBox4.Controls.Add(cbPostBlur);
            groupBox4.Controls.Add(label22);
            groupBox4.Controls.Add(label10);
            groupBox4.Controls.Add(numIGGDivisor);
            groupBox4.Controls.Add(label21);
            groupBox4.Controls.Add(label18);
            groupBox4.Controls.Add(numIGGAlpha);
            groupBox4.Controls.Add(btnInvGaussGrad);
            groupBox4.Controls.Add(numIGGKernel);
            groupBox4.Controls.Add(rbMorph);
            groupBox4.Controls.Add(rbIGG);
            groupBox4.Location = new Point(13, 207);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(402, 132);
            groupBox4.TabIndex = 739;
            groupBox4.TabStop = false;
            groupBox4.Text = "3) InvGaussGrad";
            // 
            // numPostBlurKrrnl
            // 
            numPostBlurKrrnl.Location = new Point(177, 93);
            numPostBlurKrrnl.Margin = new Padding(4, 3, 4, 3);
            numPostBlurKrrnl.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numPostBlurKrrnl.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numPostBlurKrrnl.Name = "numPostBlurKrrnl";
            numPostBlurKrrnl.Size = new Size(52, 23);
            numPostBlurKrrnl.TabIndex = 688;
            numPostBlurKrrnl.Value = new decimal(new int[] { 7, 0, 0, 0 });
            // 
            // cbPostBlur
            // 
            cbPostBlur.AutoSize = true;
            cbPostBlur.Checked = true;
            cbPostBlur.CheckState = CheckState.Checked;
            cbPostBlur.Location = new Point(19, 96);
            cbPostBlur.Name = "cbPostBlur";
            cbPostBlur.Size = new Size(73, 19);
            cbPostBlur.TabIndex = 694;
            cbPostBlur.Text = "post Blur";
            cbPostBlur.UseVisualStyleBackColor = true;
            cbPostBlur.CheckedChanged += cbSecondColor_CheckedChanged;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(132, 58);
            label22.Name = "label22";
            label22.Size = new Size(38, 15);
            label22.TabIndex = 695;
            label22.Text = "Alpha";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(132, 97);
            label10.Name = "label10";
            label10.Size = new Size(40, 15);
            label10.TabIndex = 685;
            label10.Text = "Kernel";
            // 
            // numIGGDivisor
            // 
            numIGGDivisor.Location = new Point(295, 55);
            numIGGDivisor.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numIGGDivisor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numIGGDivisor.Name = "numIGGDivisor";
            numIGGDivisor.Size = new Size(52, 23);
            numIGGDivisor.TabIndex = 690;
            numIGGDivisor.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(246, 58);
            label21.Name = "label21";
            label21.Size = new Size(43, 15);
            label21.TabIndex = 694;
            label21.Text = "Divisor";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(19, 58);
            label18.Name = "label18";
            label18.Size = new Size(40, 15);
            label18.TabIndex = 693;
            label18.Text = "Kernel";
            // 
            // numIGGAlpha
            // 
            numIGGAlpha.Location = new Point(177, 56);
            numIGGAlpha.Margin = new Padding(4, 3, 4, 3);
            numIGGAlpha.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numIGGAlpha.Name = "numIGGAlpha";
            numIGGAlpha.Size = new Size(52, 23);
            numIGGAlpha.TabIndex = 691;
            numIGGAlpha.Value = new decimal(new int[] { 101, 0, 0, 0 });
            // 
            // btnInvGaussGrad
            // 
            btnInvGaussGrad.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnInvGaussGrad.ForeColor = SystemColors.ControlText;
            btnInvGaussGrad.Location = new Point(305, 92);
            btnInvGaussGrad.Margin = new Padding(4, 3, 4, 3);
            btnInvGaussGrad.Name = "btnInvGaussGrad";
            btnInvGaussGrad.Size = new Size(65, 27);
            btnInvGaussGrad.TabIndex = 689;
            btnInvGaussGrad.Text = "InvGG";
            btnInvGaussGrad.UseVisualStyleBackColor = true;
            btnInvGaussGrad.Click += btnInvGaussGrad_Click;
            // 
            // numIGGKernel
            // 
            numIGGKernel.Location = new Point(70, 56);
            numIGGKernel.Margin = new Padding(4, 3, 4, 3);
            numIGGKernel.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numIGGKernel.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numIGGKernel.Name = "numIGGKernel";
            numIGGKernel.Size = new Size(52, 23);
            numIGGKernel.TabIndex = 692;
            numIGGKernel.Value = new decimal(new int[] { 27, 0, 0, 0 });
            // 
            // rbMorph
            // 
            rbMorph.AutoSize = true;
            rbMorph.Location = new Point(132, 22);
            rbMorph.Name = "rbMorph";
            rbMorph.Size = new Size(151, 19);
            rbMorph.TabIndex = 740;
            rbMorph.Text = "Morphological Gradient";
            rbMorph.UseVisualStyleBackColor = true;
            // 
            // rbIGG
            // 
            rbIGG.AutoSize = true;
            rbIGG.Checked = true;
            rbIGG.Location = new Point(19, 22);
            rbIGG.Name = "rbIGG";
            rbIGG.Size = new Size(97, 19);
            rbIGG.TabIndex = 740;
            rbIGG.TabStop = true;
            rbIGG.Text = "InvGaussGrad";
            rbIGG.UseVisualStyleBackColor = true;
            rbIGG.CheckedChanged += rbIGG_CheckedChanged;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label16);
            groupBox3.Controls.Add(label15);
            groupBox3.Controls.Add(numDistWeight);
            groupBox3.Controls.Add(btnBlur);
            groupBox3.Controls.Add(numKernel);
            groupBox3.Location = new Point(13, 138);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(402, 59);
            groupBox3.TabIndex = 739;
            groupBox3.TabStop = false;
            groupBox3.Text = "2) Blur";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(132, 26);
            label16.Name = "label16";
            label16.Size = new Size(64, 15);
            label16.TabIndex = 689;
            label16.Text = "distWeight";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(19, 26);
            label15.Name = "label15";
            label15.Size = new Size(40, 15);
            label15.TabIndex = 685;
            label15.Text = "Kernel";
            // 
            // numDistWeight
            // 
            numDistWeight.Location = new Point(203, 22);
            numDistWeight.Margin = new Padding(4, 3, 4, 3);
            numDistWeight.Maximum = new decimal(new int[] { 444, 0, 0, 0 });
            numDistWeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDistWeight.Name = "numDistWeight";
            numDistWeight.Size = new Size(52, 23);
            numDistWeight.TabIndex = 687;
            toolTip1.SetToolTip(numDistWeight, "max color dist to keep blurring.\r\n\"Edge weight\" - bigger blurs more edges\r\nvalue range from 1 to 443, \r\n444 is blurs without efge test");
            numDistWeight.Value = new decimal(new int[] { 101, 0, 0, 0 });
            // 
            // btnBlur
            // 
            btnBlur.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBlur.ForeColor = SystemColors.ControlText;
            btnBlur.Location = new Point(273, 18);
            btnBlur.Margin = new Padding(4, 3, 4, 3);
            btnBlur.Name = "btnBlur";
            btnBlur.Size = new Size(65, 27);
            btnBlur.TabIndex = 686;
            btnBlur.Text = "Blur";
            btnBlur.UseVisualStyleBackColor = true;
            btnBlur.Click += btnBlur_Click;
            // 
            // numKernel
            // 
            numKernel.Location = new Point(70, 22);
            numKernel.Margin = new Padding(4, 3, 4, 3);
            numKernel.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numKernel.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numKernel.Name = "numKernel";
            numKernel.Size = new Size(52, 23);
            numKernel.TabIndex = 688;
            numKernel.Value = new decimal(new int[] { 127, 0, 0, 0 });
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(pictureBox1);
            groupBox2.Controls.Add(cbSecondColor);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(numValDst2);
            groupBox2.Controls.Add(numValSrc2);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(label17);
            groupBox2.Controls.Add(numValDst);
            groupBox2.Controls.Add(numValSrc);
            groupBox2.Controls.Add(btnColors);
            groupBox2.Location = new Point(13, 40);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(402, 92);
            groupBox2.TabIndex = 739;
            groupBox2.TabStop = false;
            groupBox2.Text = "1) Colors [optional, not done in automatic methods]";
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(359, 8);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(40, 40);
            pictureBox1.TabIndex = 695;
            pictureBox1.TabStop = false;
            // 
            // cbSecondColor
            // 
            cbSecondColor.AutoSize = true;
            cbSecondColor.Location = new Point(19, 56);
            cbSecondColor.Name = "cbSecondColor";
            cbSecondColor.Size = new Size(125, 19);
            cbSecondColor.TabIndex = 694;
            cbSecondColor.Text = "map a 2nd color X:";
            cbSecondColor.UseVisualStyleBackColor = true;
            cbSecondColor.CheckedChanged += cbSecondColor_CheckedChanged;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(214, 58);
            label8.Name = "label8";
            label8.Size = new Size(31, 15);
            label8.TabIndex = 693;
            label8.Text = "to Y:";
            // 
            // numValDst2
            // 
            numValDst2.Enabled = false;
            numValDst2.Location = new Point(251, 55);
            numValDst2.Margin = new Padding(4, 3, 4, 3);
            numValDst2.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numValDst2.Name = "numValDst2";
            numValDst2.Size = new Size(52, 23);
            numValDst2.TabIndex = 691;
            numValDst2.Value = new decimal(new int[] { 164, 0, 0, 0 });
            // 
            // numValSrc2
            // 
            numValSrc2.Enabled = false;
            numValSrc2.Location = new Point(155, 55);
            numValSrc2.Margin = new Padding(4, 3, 4, 3);
            numValSrc2.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numValSrc2.Name = "numValSrc2";
            numValSrc2.Size = new Size(52, 23);
            numValSrc2.TabIndex = 692;
            numValSrc2.Value = new decimal(new int[] { 148, 0, 0, 0 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(132, 26);
            label7.Name = "label7";
            label7.Size = new Size(31, 15);
            label7.TabIndex = 689;
            label7.Text = "to Y:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(19, 26);
            label17.Name = "label17";
            label17.Size = new Size(44, 15);
            label17.TabIndex = 689;
            label17.Text = "Map X:";
            // 
            // numValDst
            // 
            numValDst.Location = new Point(170, 24);
            numValDst.Margin = new Padding(4, 3, 4, 3);
            numValDst.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numValDst.Name = "numValDst";
            numValDst.Size = new Size(52, 23);
            numValDst.TabIndex = 687;
            numValDst.Value = new decimal(new int[] { 148, 0, 0, 0 });
            // 
            // numValSrc
            // 
            numValSrc.Location = new Point(70, 24);
            numValSrc.Margin = new Padding(4, 3, 4, 3);
            numValSrc.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numValSrc.Name = "numValSrc";
            numValSrc.Size = new Size(52, 23);
            numValSrc.TabIndex = 688;
            numValSrc.Value = new decimal(new int[] { 62, 0, 0, 0 });
            // 
            // btnColors
            // 
            btnColors.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnColors.ForeColor = SystemColors.ControlText;
            btnColors.Location = new Point(313, 52);
            btnColors.Margin = new Padding(4, 3, 4, 3);
            btnColors.Name = "btnColors";
            btnColors.Size = new Size(65, 27);
            btnColors.TabIndex = 686;
            btnColors.Text = "Colors";
            toolTip1.SetToolTip(btnColors, "Create a cardinal spline (curve) of color mappings, by specifying one or two colors.");
            btnColors.UseVisualStyleBackColor = true;
            btnColors.Click += btnColors_Click;
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDarkDark;
            panel1.Location = new Point(13, 357);
            panel1.Name = "panel1";
            panel1.Size = new Size(400, 2);
            panel1.TabIndex = 738;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(rbGreaterThan);
            groupBox1.Controls.Add(rbLessThan);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(numF1);
            groupBox1.Controls.Add(cbDoFirstMult);
            groupBox1.Controls.Add(cbDoSecondMult);
            groupBox1.Controls.Add(cbAuto);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(numExp2);
            groupBox1.Controls.Add(numExp1);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(numF2);
            groupBox1.Controls.Add(numTh);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(numThMultiplier);
            groupBox1.Location = new Point(12, 401);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(403, 186);
            groupBox1.TabIndex = 737;
            groupBox1.TabStop = false;
            groupBox1.Text = "Application Settings";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(153, 128);
            label9.Name = "label9";
            label9.Size = new Size(102, 15);
            label9.TabIndex = 738;
            label9.Text = "factor2 values are ";
            // 
            // rbGreaterThan
            // 
            rbGreaterThan.AutoSize = true;
            rbGreaterThan.Location = new Point(321, 126);
            rbGreaterThan.Name = "rbGreaterThan";
            rbGreaterThan.Size = new Size(47, 19);
            rbGreaterThan.TabIndex = 737;
            rbGreaterThan.Text = "> th";
            rbGreaterThan.UseVisualStyleBackColor = true;
            // 
            // rbLessThan
            // 
            rbLessThan.AutoSize = true;
            rbLessThan.Checked = true;
            rbLessThan.Location = new Point(267, 126);
            rbLessThan.Name = "rbLessThan";
            rbLessThan.Size = new Size(47, 19);
            rbLessThan.TabIndex = 737;
            rbLessThan.TabStop = true;
            rbLessThan.Text = "< th";
            rbLessThan.UseVisualStyleBackColor = true;
            // 
            // cbDoFirstMult
            // 
            cbDoFirstMult.AutoSize = true;
            cbDoFirstMult.Checked = true;
            cbDoFirstMult.CheckState = CheckState.Checked;
            cbDoFirstMult.Location = new Point(8, 18);
            cbDoFirstMult.Name = "cbDoFirstMult";
            cbDoFirstMult.Size = new Size(87, 19);
            cbDoFirstMult.TabIndex = 736;
            cbDoFirstMult.Text = "Do 1st mult";
            cbDoFirstMult.UseVisualStyleBackColor = true;
            cbDoFirstMult.CheckedChanged += cbDoFirstMult_CheckedChanged;
            // 
            // cbDoSecondMult
            // 
            cbDoSecondMult.AutoSize = true;
            cbDoSecondMult.Checked = true;
            cbDoSecondMult.CheckState = CheckState.Checked;
            cbDoSecondMult.Location = new Point(8, 78);
            cbDoSecondMult.Name = "cbDoSecondMult";
            cbDoSecondMult.Size = new Size(92, 19);
            cbDoSecondMult.TabIndex = 736;
            cbDoSecondMult.Text = "Do 2nd mult";
            cbDoSecondMult.UseVisualStyleBackColor = true;
            cbDoSecondMult.CheckedChanged += cbDoSecondMult_CheckedChanged;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // backgroundWorker3
            // 
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.WorkerSupportsCancellation = true;
            backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            backgroundWorker3.ProgressChanged += backgroundWorker3_ProgressChanged;
            backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
            // 
            // Timer3
            // 
            Timer3.Interval = 500;
            Timer3.Tick += Timer3_Tick;
            // 
            // backgroundWorker4
            // 
            backgroundWorker4.WorkerReportsProgress = true;
            backgroundWorker4.WorkerSupportsCancellation = true;
            backgroundWorker4.DoWork += backgroundWorker4_DoWork;
            backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
            backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
            // 
            // frmLumMapSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1255, 797);
            Controls.Add(splitContainer1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmLumMapSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmLumMapSettings";
            FormClosing += frmLumMapSettings_FormClosing;
            Load += frmLumMapSettings_Load;
            ((System.ComponentModel.ISupportInitialize)numF1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTh).EndInit();
            ((System.ComponentModel.ISupportInitialize)numF2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numExp1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numExp2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numThMultiplier).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPostBlurKrrnl).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIGGDivisor).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIGGAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIGGKernel).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numDistWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numKernel).EndInit();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numValDst2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numValSrc2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numValDst).EndInit();
            ((System.ComponentModel.ISupportInitialize)numValSrc).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnCancel;
        private Button btnOK;
        private Label label1;
        private Label label2;
        private Label label3;
        internal NumericUpDown numF1;
        internal NumericUpDown numTh;
        internal NumericUpDown numF2;
        private Label label4;
        internal NumericUpDown numExp1;
        private Label label5;
        internal NumericUpDown numExp2;
        private Label label6;
        internal NumericUpDown numThMultiplier;
        internal CheckBox cbAuto;
        private SplitContainer splitContainer1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private GroupBox groupBox1;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private Panel panel1;
        private GroupBox groupBox4;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private RadioButton rbApp;
        private RadioButton rbImage;
        internal Label Label20;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        private Label label7;
        private Label label17;
        internal NumericUpDown numValDst;
        internal NumericUpDown numValSrc;
        private Button btnColors;
        private Label label16;
        private Label label15;
        internal NumericUpDown numDistWeight;
        private Button btnBlur;
        internal NumericUpDown numKernel;
        private Label label22;
        private NumericUpDown numIGGDivisor;
        private Label label21;
        private Label label18;
        internal NumericUpDown numIGGAlpha;
        private Button btnInvGaussGrad;
        internal NumericUpDown numIGGKernel;
        private Button button10;
        private Button button8;
        internal Button button2;
        private Button btnRedo;
        private Button btnUndo;
        private ToolTip toolTip1;
        private SaveFileDialog saveFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker3;
        internal System.Windows.Forms.Timer Timer3;
        private System.ComponentModel.BackgroundWorker backgroundWorker4;
        internal CheckBox cbSecondColor;
        private Label label8;
        internal NumericUpDown numValDst2;
        internal NumericUpDown numValSrc2;
        internal CheckBox cbAppSettingsOnly;
        private PictureBox pictureBox1;
        private Label label9;
        internal RadioButton rbGreaterThan;
        internal RadioButton rbLessThan;
        internal NumericUpDown numPostBlurKrrnl;
        internal CheckBox cbPostBlur;
        private Label label10;
        private RadioButton rbMorph;
        private RadioButton rbIGG;
        internal CheckBox cbDoSecondMult;
        internal CheckBox cbDoFirstMult;
    }
}