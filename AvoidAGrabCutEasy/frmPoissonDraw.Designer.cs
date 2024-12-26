using HelplineRulerControl;

namespace AvoidAGrabCutEasy
{
    partial class frmPoissonDraw
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
            try
            {
                if (this._tb != null)
                    this._tb.Dispose();

            }
            catch { }

            try
            {
                if (this.helplineRulerCtrl1.Bmp != null)
                    this.helplineRulerCtrl1.Bmp.Dispose();

            }
            catch { }

            try
            {
                if (this.helplineRulerCtrl2.Bmp != null)
                    this.helplineRulerCtrl2.Bmp.Dispose();

            }
            catch { }

            try
            {
                if (this._bmpBU != null)
                    this._bmpBU.Dispose();

            }
            catch { }

            try
            {
                if (this._bmpOrg != null)
                    this._bmpOrg.Dispose();

            }
            catch { }

            try
            {
                if (this._bmpDraw != null)
                    this._bmpDraw.Dispose();

            }
            catch { }


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
            openFileDialog1 = new OpenFileDialog();
            helplineRulerCtrl1 = new HelplineRulerCtrl();
            saveFileDialog1 = new SaveFileDialog();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            Panel4 = new Panel();
            Panel8 = new Panel();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            button10 = new Button();
            button8 = new Button();
            btnUndo = new Button();
            btnRedo = new Button();
            btnCancel = new Button();
            btnOK = new Button();
            button3 = new Button();
            button2 = new Button();
            toolTip1 = new ToolTip(components);
            btnGo = new Button();
            toolStripStatusLabel5 = new ToolStripStatusLabel();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            ToolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            panel3 = new Panel();
            panel1 = new Panel();
            btnScreenBlend = new Button();
            label5 = new Label();
            cbOverlay = new CheckBox();
            btnLoadOrig = new Button();
            numOpacity = new NumericUpDown();
            button12 = new Button();
            numLowerWeight = new NumericUpDown();
            numUpperWeight = new NumericUpDown();
            label4 = new Label();
            label3 = new Label();
            cmbAlg = new ComboBox();
            label2 = new Label();
            btnNewPath = new Button();
            rbDest = new RadioButton();
            rbSrc = new RadioButton();
            cbAuto = new CheckBox();
            cbSetPts = new CheckBox();
            label11 = new Label();
            cbDraw = new CheckBox();
            numPenSize = new NumericUpDown();
            label1 = new Label();
            Timer3 = new System.Windows.Forms.Timer(components);
            splitContainer1 = new SplitContainer();
            helplineRulerCtrl2 = new HelplineRulerCtrl();
            label6 = new Label();
            btnSaveStrokes = new Button();
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOpacity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numLowerWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numUpperWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPenSize).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png";
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
            helplineRulerCtrl1.Margin = new Padding(5, 3, 5, 3);
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(606, 622);
            helplineRulerCtrl1.TabIndex = 249;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // Panel4
            // 
            Panel4.BackColor = SystemColors.ActiveCaptionText;
            Panel4.Location = new Point(1150, 180);
            Panel4.Margin = new Padding(4, 3, 4, 3);
            Panel4.Name = "Panel4";
            Panel4.Size = new Size(197, 2);
            Panel4.TabIndex = 509;
            // 
            // Panel8
            // 
            Panel8.BackColor = SystemColors.ActiveCaptionText;
            Panel8.Location = new Point(13, 180);
            Panel8.Margin = new Padding(4, 3, 4, 3);
            Panel8.Name = "Panel8";
            Panel8.Size = new Size(1107, 2);
            Panel8.TabIndex = 507;
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(1098, 82);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 313;
            cmbZoom.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(1003, 12);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 211;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += CheckBox12_CheckedChanged;
            // 
            // button10
            // 
            button10.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(1097, 44);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 106;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(1000, 44);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 102;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // btnUndo
            // 
            btnUndo.Enabled = false;
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(1000, 114);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 98;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // btnRedo
            // 
            btnRedo.Enabled = false;
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(1099, 114);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 99;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(1097, 147);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 100;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(1003, 147);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 101;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += Button28_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.FlatStyle = FlatStyle.System;
            button3.ForeColor = SystemColors.ControlText;
            button3.Location = new Point(2400, 13);
            button3.Margin = new Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new Size(88, 27);
            button3.TabIndex = 97;
            button3.Text = "Open";
            button3.Visible = false;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(1097, 10);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 91;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // btnGo
            // 
            btnGo.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGo.Location = new Point(878, 147);
            btnGo.Margin = new Padding(4, 3, 4, 3);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(88, 27);
            btnGo.TabIndex = 522;
            btnGo.Text = "Go";
            toolTip1.SetToolTip(btnGo, "5");
            btnGo.UseVisualStyleBackColor = true;
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new Size(19, 34);
            toolStripStatusLabel5.Text = "    ";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, ToolStripStatusLabel2, toolStripStatusLabel5, toolStripProgressBar1, toolStripStatusLabel4, toolStripStatusLabel3 });
            statusStrip1.Location = new Point(0, 811);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1200, 39);
            statusStrip1.TabIndex = 246;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("Segoe UI", 16F);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(63, 34);
            toolStripStatusLabel1.Text = "Hallo";
            // 
            // ToolStripStatusLabel2
            // 
            ToolStripStatusLabel2.AutoSize = false;
            ToolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            ToolStripStatusLabel2.Name = "ToolStripStatusLabel2";
            ToolStripStatusLabel2.Size = new Size(100, 34);
            ToolStripStatusLabel2.Text = "    ";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(467, 33);
            toolStripProgressBar1.Visible = false;
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Font = new Font("Segoe UI", 16F);
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(63, 34);
            toolStripStatusLabel4.Text = "Hallo";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.AutoSize = false;
            toolStripStatusLabel3.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(100, 34);
            toolStripStatusLabel3.Text = "    ";
            // 
            // panel3
            // 
            panel3.Location = new Point(0, 0);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(233, 28);
            panel3.TabIndex = 248;
            // 
            // panel1
            // 
            panel1.Controls.Add(label6);
            panel1.Controls.Add(btnSaveStrokes);
            panel1.Controls.Add(btnScreenBlend);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(cbOverlay);
            panel1.Controls.Add(btnLoadOrig);
            panel1.Controls.Add(numOpacity);
            panel1.Controls.Add(button12);
            panel1.Controls.Add(numLowerWeight);
            panel1.Controls.Add(numUpperWeight);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(cmbAlg);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(btnNewPath);
            panel1.Controls.Add(rbDest);
            panel1.Controls.Add(rbSrc);
            panel1.Controls.Add(cbAuto);
            panel1.Controls.Add(cbSetPts);
            panel1.Controls.Add(label11);
            panel1.Controls.Add(cbDraw);
            panel1.Controls.Add(numPenSize);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(btnGo);
            panel1.Controls.Add(Panel4);
            panel1.Controls.Add(Panel8);
            panel1.Controls.Add(cmbZoom);
            panel1.Controls.Add(cbBGColor);
            panel1.Controls.Add(button10);
            panel1.Controls.Add(button8);
            panel1.Controls.Add(btnUndo);
            panel1.Controls.Add(btnRedo);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnOK);
            panel1.Controls.Add(button3);
            panel1.Controls.Add(button2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1200, 189);
            panel1.TabIndex = 247;
            // 
            // btnScreenBlend
            // 
            btnScreenBlend.Location = new Point(274, 130);
            btnScreenBlend.Name = "btnScreenBlend";
            btnScreenBlend.Size = new Size(88, 28);
            btnScreenBlend.TabIndex = 763;
            btnScreenBlend.Text = "ScreenBlend";
            btnScreenBlend.UseVisualStyleBackColor = true;
            btnScreenBlend.Click += btnScreenBlend_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(744, 153);
            label5.Name = "label5";
            label5.Size = new Size(48, 15);
            label5.TabIndex = 762;
            label5.Text = "Opacity";
            // 
            // cbOverlay
            // 
            cbOverlay.AutoSize = true;
            cbOverlay.Location = new Point(637, 152);
            cbOverlay.Name = "cbOverlay";
            cbOverlay.Size = new Size(101, 19);
            cbOverlay.TabIndex = 761;
            cbOverlay.Text = "overlay src pic";
            cbOverlay.UseVisualStyleBackColor = true;
            cbOverlay.CheckedChanged += cbOverlay_CheckedChanged;
            // 
            // btnLoadOrig
            // 
            btnLoadOrig.Location = new Point(156, 130);
            btnLoadOrig.Name = "btnLoadOrig";
            btnLoadOrig.Size = new Size(88, 28);
            btnLoadOrig.TabIndex = 760;
            btnLoadOrig.Text = "load Orig Pic";
            btnLoadOrig.UseVisualStyleBackColor = true;
            // 
            // numOpacity
            // 
            numOpacity.DecimalPlaces = 2;
            numOpacity.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numOpacity.Location = new Point(798, 151);
            numOpacity.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numOpacity.Name = "numOpacity";
            numOpacity.Size = new Size(61, 23);
            numOpacity.TabIndex = 759;
            numOpacity.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            numOpacity.ValueChanged += numOpacity_ValueChanged;
            // 
            // button12
            // 
            button12.Location = new Point(13, 130);
            button12.Name = "button12";
            button12.Size = new Size(88, 28);
            button12.TabIndex = 756;
            button12.Text = "load Src Pic";
            button12.UseVisualStyleBackColor = true;
            button12.Click += button12_Click;
            // 
            // numLowerWeight
            // 
            numLowerWeight.DecimalPlaces = 2;
            numLowerWeight.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numLowerWeight.Location = new Point(771, 54);
            numLowerWeight.Name = "numLowerWeight";
            numLowerWeight.Size = new Size(61, 23);
            numLowerWeight.TabIndex = 755;
            numLowerWeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numUpperWeight
            // 
            numUpperWeight.DecimalPlaces = 2;
            numUpperWeight.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numUpperWeight.Location = new Point(583, 54);
            numUpperWeight.Name = "numUpperWeight";
            numUpperWeight.Size = new Size(61, 23);
            numUpperWeight.TabIndex = 755;
            numUpperWeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(664, 56);
            label4.Name = "label4";
            label4.Size = new Size(101, 15);
            label4.TabIndex = 754;
            label4.Text = "lower Img Weight";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(473, 56);
            label3.Name = "label3";
            label3.Size = new Size(103, 15);
            label3.TabIndex = 754;
            label3.Text = "upper Img Weight";
            // 
            // cmbAlg
            // 
            cmbAlg.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAlg.FormattingEnabled = true;
            cmbAlg.Items.AddRange(new object[] { "AddB", "NormalB", "MaxB", "FixedWeightB" });
            cmbAlg.Location = new Point(540, 15);
            cmbAlg.Name = "cmbAlg";
            cmbAlg.Size = new Size(121, 23);
            cmbAlg.TabIndex = 753;
            cmbAlg.SelectedIndexChanged += cmbAlg_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(473, 19);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 752;
            label2.Text = "Algorithm";
            // 
            // btnNewPath
            // 
            btnNewPath.Location = new Point(338, 12);
            btnNewPath.Name = "btnNewPath";
            btnNewPath.Size = new Size(88, 28);
            btnNewPath.TabIndex = 751;
            btnNewPath.Text = "new Path";
            btnNewPath.UseVisualStyleBackColor = true;
            btnNewPath.Click += btnNewPath_Click;
            // 
            // rbDest
            // 
            rbDest.AutoSize = true;
            rbDest.Location = new Point(250, 48);
            rbDest.Name = "rbDest";
            rbDest.Size = new Size(59, 19);
            rbDest.TabIndex = 750;
            rbDest.Text = "DestPt";
            rbDest.UseVisualStyleBackColor = true;
            // 
            // rbSrc
            // 
            rbSrc.AutoSize = true;
            rbSrc.Checked = true;
            rbSrc.Location = new Point(192, 48);
            rbSrc.Name = "rbSrc";
            rbSrc.Size = new Size(52, 19);
            rbSrc.TabIndex = 750;
            rbSrc.TabStop = true;
            rbSrc.Text = "SrcPt";
            rbSrc.UseVisualStyleBackColor = true;
            // 
            // cbAuto
            // 
            cbAuto.AutoSize = true;
            cbAuto.Location = new Point(124, 49);
            cbAuto.Name = "cbAuto";
            cbAuto.Size = new Size(52, 19);
            cbAuto.TabIndex = 749;
            cbAuto.Text = "Auto";
            cbAuto.UseVisualStyleBackColor = true;
            // 
            // cbSetPts
            // 
            cbSetPts.AutoSize = true;
            cbSetPts.Location = new Point(41, 49);
            cbSetPts.Name = "cbSetPts";
            cbSetPts.Size = new Size(77, 19);
            cbSetPts.TabIndex = 749;
            cbSetPts.Text = "set Points";
            cbSetPts.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(13, 19);
            label11.Name = "label11";
            label11.Size = new Size(47, 15);
            label11.TabIndex = 748;
            label11.Text = "PenSize";
            // 
            // cbDraw
            // 
            cbDraw.AutoSize = true;
            cbDraw.Location = new Point(163, 18);
            cbDraw.Name = "cbDraw";
            cbDraw.Size = new Size(53, 19);
            cbDraw.TabIndex = 746;
            cbDraw.Text = "Draw";
            cbDraw.UseVisualStyleBackColor = true;
            // 
            // numPenSize
            // 
            numPenSize.DecimalPlaces = 2;
            numPenSize.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numPenSize.Location = new Point(77, 15);
            numPenSize.Margin = new Padding(4, 3, 4, 3);
            numPenSize.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numPenSize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPenSize.Name = "numPenSize";
            numPenSize.Size = new Size(70, 23);
            numPenSize.TabIndex = 745;
            numPenSize.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(1030, 85);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(58, 15);
            label1.TabIndex = 691;
            label1.Text = "Set Zoom";
            // 
            // Timer3
            // 
            Timer3.Interval = 500;
            Timer3.Tick += Timer3_Tick;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 189);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(helplineRulerCtrl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(helplineRulerCtrl2);
            splitContainer1.Size = new Size(1200, 622);
            splitContainer1.SplitterDistance = 606;
            splitContainer1.TabIndex = 250;
            // 
            // helplineRulerCtrl2
            // 
            helplineRulerCtrl2.Bmp = null;
            helplineRulerCtrl2.Dock = DockStyle.Fill;
            helplineRulerCtrl2.DontDoLayout = false;
            helplineRulerCtrl2.DontHandleDoubleClick = false;
            helplineRulerCtrl2.DontPaintBaseImg = false;
            helplineRulerCtrl2.DontProcDoubleClick = false;
            helplineRulerCtrl2.DrawModeClipped = false;
            helplineRulerCtrl2.DrawPixelated = false;
            helplineRulerCtrl2.IgnoreZoom = false;
            helplineRulerCtrl2.Location = new Point(0, 0);
            helplineRulerCtrl2.Margin = new Padding(5, 3, 5, 3);
            helplineRulerCtrl2.MoveHelpLinesOnResize = false;
            helplineRulerCtrl2.Name = "helplineRulerCtrl2";
            helplineRulerCtrl2.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl2.Size = new Size(590, 622);
            helplineRulerCtrl2.TabIndex = 249;
            helplineRulerCtrl2.Zoom = 1F;
            helplineRulerCtrl2.ZoomSetManually = false;
            helplineRulerCtrl2.DBPanelDblClicked += helplineRulerCtrl2_DBPanelDblClicked;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(637, 114);
            label6.Name = "label6";
            label6.Size = new Size(178, 15);
            label6.TabIndex = 765;
            label6.Text = "save a pic with all drawn striokes";
            // 
            // btnSaveStrokes
            // 
            btnSaveStrokes.Location = new Point(824, 107);
            btnSaveStrokes.Name = "btnSaveStrokes";
            btnSaveStrokes.Size = new Size(88, 28);
            btnSaveStrokes.TabIndex = 764;
            btnSaveStrokes.Text = "Go";
            btnSaveStrokes.UseVisualStyleBackColor = true;
            btnSaveStrokes.Click += btnSaveStrokes_Click;
            // 
            // frmPoissonDraw
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 850);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(panel3);
            Controls.Add(statusStrip1);
            Name = "frmPoissonDraw";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmPoissonDraw";
            Load += frmPoissonDraw_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOpacity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numLowerWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numUpperWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPenSize).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private OpenFileDialog openFileDialog1;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private SaveFileDialog saveFileDialog1;
        internal System.ComponentModel.BackgroundWorker backgroundWorker1;
        internal Panel Panel4;
        internal Panel Panel8;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        private Button button10;
        private Button button8;
        private Button btnUndo;
        private Button btnRedo;
        private Button btnCancel;
        private Button btnOK;
        internal Button button3;
        internal Button button2;
        private ToolTip toolTip1;
        internal Button btnGo;
        private ToolStripStatusLabel toolStripStatusLabel5;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        internal ToolStripStatusLabel ToolStripStatusLabel2;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel4;
        internal ToolStripStatusLabel toolStripStatusLabel3;
        private Panel panel3;
        internal Panel panel1;
        internal System.Windows.Forms.Timer Timer3;
        internal Label label1;
        private SplitContainer splitContainer1;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl2;
        private NumericUpDown numLowerWeight;
        private NumericUpDown numUpperWeight;
        private Label label4;
        private Label label3;
        private ComboBox cmbAlg;
        private Label label2;
        private Button btnNewPath;
        private RadioButton rbDest;
        private RadioButton rbSrc;
        private CheckBox cbAuto;
        private CheckBox cbSetPts;
        private Label label11;
        private CheckBox cbDraw;
        private NumericUpDown numPenSize;
        private Button button12;
        private Label label5;
        private CheckBox cbOverlay;
        internal Button btnLoadOrig;
        private NumericUpDown numOpacity;
        private Button btnScreenBlend;
        private Label label6;
        internal Button btnSaveStrokes;
    }
}