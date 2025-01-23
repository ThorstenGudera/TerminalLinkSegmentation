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
            cbFindDestPoint = new CheckBox();
            numSrcPtSurround = new NumericUpDown();
            cbOrdinaryDraw = new CheckBox();
            toolStripStatusLabel5 = new ToolStripStatusLabel();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            ToolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel6 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            panel3 = new Panel();
            panel1 = new Panel();
            cbShowCustPic = new CheckBox();
            btnSaveHLC1 = new Button();
            label14 = new Label();
            numOpacityOrdDraw = new NumericUpDown();
            btnLoadEdited = new Button();
            btnHSL2 = new Button();
            cbFindInOrig = new CheckBox();
            btnColorsHSL = new Button();
            btnColorsRGB = new Button();
            pictureBox1 = new PictureBox();
            numGamma = new NumericUpDown();
            numMaxPixelDist = new NumericUpDown();
            label12 = new Label();
            label10 = new Label();
            panel5 = new Panel();
            cbLoadFCache = new CheckBox();
            rbShiftC = new RadioButton();
            rbShiftMP = new RadioButton();
            cbShiftImage = new CheckBox();
            cbDiffCol = new CheckBox();
            label13 = new Label();
            numExtendRegion = new NumericUpDown();
            cbBlackBG = new CheckBox();
            cbWholeRegionPic = new CheckBox();
            cbUseCustomReBlendPic = new CheckBox();
            label9 = new Label();
            btnReBlend = new Button();
            btnLoadCustomPenStrokesPic = new Button();
            panel2 = new Panel();
            label8 = new Label();
            label7 = new Label();
            cbClickMode = new CheckBox();
            btnFindDestPoint = new Button();
            label6 = new Label();
            btnSaveStrokes = new Button();
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
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            colorDialog1 = new ColorDialog();
            ((System.ComponentModel.ISupportInitialize)numSrcPtSurround).BeginInit();
            statusStrip1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOpacityOrdDraw).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numGamma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numMaxPixelDist).BeginInit();
            panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numExtendRegion).BeginInit();
            panel2.SuspendLayout();
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
            helplineRulerCtrl1.Size = new Size(757, 664);
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
            Panel4.Location = new Point(1150, 220);
            Panel4.Margin = new Padding(4, 3, 4, 3);
            Panel4.Name = "Panel4";
            Panel4.Size = new Size(197, 2);
            Panel4.TabIndex = 509;
            // 
            // Panel8
            // 
            Panel8.BackColor = SystemColors.ActiveCaptionText;
            Panel8.Location = new Point(13, 220);
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
            cmbZoom.Location = new Point(1397, 79);
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
            cbBGColor.Location = new Point(1303, 12);
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
            button10.Location = new Point(1397, 44);
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
            button8.Location = new Point(1300, 44);
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
            btnUndo.Location = new Point(1299, 111);
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
            btnRedo.Location = new Point(1398, 111);
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
            btnCancel.Location = new Point(1397, 187);
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
            btnOK.Location = new Point(1303, 187);
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
            button3.Location = new Point(2700, 13);
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
            button2.Location = new Point(1397, 10);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 91;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // cbFindDestPoint
            // 
            cbFindDestPoint.AutoSize = true;
            cbFindDestPoint.Location = new Point(63, 94);
            cbFindDestPoint.Name = "cbFindDestPoint";
            cbFindDestPoint.Size = new Size(234, 19);
            cbFindDestPoint.TabIndex = 768;
            cbFindDestPoint.Text = "find dest point - width around src point";
            toolTip1.SetToolTip(cbFindDestPoint, "Please note, that all pixels of the rectangle around the Src point \r\nmust be contained in the Dest image.\r\nAlso make sure, there's enough color-change inside the rectangle.");
            cbFindDestPoint.UseVisualStyleBackColor = true;
            cbFindDestPoint.CheckedChanged += cbFindDestPoint_CheckedChanged;
            // 
            // numSrcPtSurround
            // 
            numSrcPtSurround.Location = new Point(300, 91);
            numSrcPtSurround.Margin = new Padding(4, 3, 4, 3);
            numSrcPtSurround.Minimum = new decimal(new int[] { 20, 0, 0, 0 });
            numSrcPtSurround.Name = "numSrcPtSurround";
            numSrcPtSurround.Size = new Size(56, 23);
            numSrcPtSurround.TabIndex = 766;
            toolTip1.SetToolTip(numSrcPtSurround, "Please note, that all pixels of the rectangle around the Src point \r\nmust be contained in the Dest image.\r\nAlso make sure, there's enough color-change inside the rectangle.");
            numSrcPtSurround.Value = new decimal(new int[] { 50, 0, 0, 0 });
            numSrcPtSurround.ValueChanged += numSrcPtSurround_ValueChanged;
            // 
            // cbOrdinaryDraw
            // 
            cbOrdinaryDraw.AutoSize = true;
            cbOrdinaryDraw.Enabled = false;
            cbOrdinaryDraw.Location = new Point(181, 44);
            cbOrdinaryDraw.Name = "cbOrdinaryDraw";
            cbOrdinaryDraw.Size = new Size(99, 19);
            cbOrdinaryDraw.TabIndex = 784;
            cbOrdinaryDraw.Text = "OrdinaryDraw";
            toolTip1.SetToolTip(cbOrdinaryDraw, "just draw the src, no poisson drawing");
            cbOrdinaryDraw.UseVisualStyleBackColor = true;
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new Size(19, 34);
            toolStripStatusLabel5.Text = "    ";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, ToolStripStatusLabel2, toolStripStatusLabel5, toolStripStatusLabel6, toolStripProgressBar1, toolStripStatusLabel4, toolStripStatusLabel3 });
            statusStrip1.Location = new Point(0, 893);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1500, 39);
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
            // toolStripStatusLabel6
            // 
            toolStripStatusLabel6.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel6.Name = "toolStripStatusLabel6";
            toolStripStatusLabel6.Size = new Size(23, 34);
            toolStripStatusLabel6.Text = "    ";
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
            panel1.AutoScroll = true;
            panel1.Controls.Add(cbShowCustPic);
            panel1.Controls.Add(btnSaveHLC1);
            panel1.Controls.Add(cbOrdinaryDraw);
            panel1.Controls.Add(label14);
            panel1.Controls.Add(numOpacityOrdDraw);
            panel1.Controls.Add(btnLoadEdited);
            panel1.Controls.Add(btnHSL2);
            panel1.Controls.Add(cbFindInOrig);
            panel1.Controls.Add(btnColorsHSL);
            panel1.Controls.Add(btnColorsRGB);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(numGamma);
            panel1.Controls.Add(numMaxPixelDist);
            panel1.Controls.Add(label12);
            panel1.Controls.Add(label10);
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(cbClickMode);
            panel1.Controls.Add(cbFindDestPoint);
            panel1.Controls.Add(btnFindDestPoint);
            panel1.Controls.Add(numSrcPtSurround);
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
            panel1.Size = new Size(1500, 229);
            panel1.TabIndex = 247;
            // 
            // cbShowCustPic
            // 
            cbShowCustPic.AutoSize = true;
            cbShowCustPic.Location = new Point(1037, 193);
            cbShowCustPic.Name = "cbShowCustPic";
            cbShowCustPic.Size = new Size(116, 19);
            cbShowCustPic.TabIndex = 786;
            cbShowCustPic.Text = "show custom pic";
            cbShowCustPic.UseVisualStyleBackColor = true;
            cbShowCustPic.CheckedChanged += cbShowCustPic_CheckedChanged;
            // 
            // btnSaveHLC1
            // 
            btnSaveHLC1.Location = new Point(400, 146);
            btnSaveHLC1.Name = "btnSaveHLC1";
            btnSaveHLC1.Size = new Size(88, 28);
            btnSaveHLC1.TabIndex = 785;
            btnSaveHLC1.Text = "save HLC1";
            btnSaveHLC1.UseVisualStyleBackColor = true;
            btnSaveHLC1.Click += btnSaveHLC1_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Enabled = false;
            label14.Location = new Point(282, 45);
            label14.Name = "label14";
            label14.Size = new Size(48, 15);
            label14.TabIndex = 783;
            label14.Text = "Opacity";
            // 
            // numOpacityOrdDraw
            // 
            numOpacityOrdDraw.DecimalPlaces = 2;
            numOpacityOrdDraw.Enabled = false;
            numOpacityOrdDraw.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numOpacityOrdDraw.Location = new Point(336, 43);
            numOpacityOrdDraw.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numOpacityOrdDraw.Name = "numOpacityOrdDraw";
            numOpacityOrdDraw.Size = new Size(61, 23);
            numOpacityOrdDraw.TabIndex = 782;
            numOpacityOrdDraw.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // btnLoadEdited
            // 
            btnLoadEdited.Enabled = false;
            btnLoadEdited.Location = new Point(273, 146);
            btnLoadEdited.Name = "btnLoadEdited";
            btnLoadEdited.Size = new Size(121, 28);
            btnLoadEdited.TabIndex = 781;
            btnLoadEdited.Text = "load edited Src Pic";
            btnLoadEdited.UseVisualStyleBackColor = true;
            btnLoadEdited.Click += btnLoadEdited_Click;
            // 
            // btnHSL2
            // 
            btnHSL2.Location = new Point(1195, 187);
            btnHSL2.Name = "btnHSL2";
            btnHSL2.Size = new Size(88, 28);
            btnHSL2.TabIndex = 780;
            btnHSL2.Text = "HSL";
            btnHSL2.UseVisualStyleBackColor = true;
            btnHSL2.Click += btnHSL2_Click;
            // 
            // cbFindInOrig
            // 
            cbFindInOrig.AutoSize = true;
            cbFindInOrig.Location = new Point(382, 121);
            cbFindInOrig.Name = "cbFindInOrig";
            cbFindInOrig.Size = new Size(122, 19);
            cbFindInOrig.TabIndex = 779;
            cbFindInOrig.Text = "find in Orig Image";
            cbFindInOrig.UseVisualStyleBackColor = true;
            // 
            // btnColorsHSL
            // 
            btnColorsHSL.Location = new Point(367, 187);
            btnColorsHSL.Name = "btnColorsHSL";
            btnColorsHSL.Size = new Size(88, 28);
            btnColorsHSL.TabIndex = 777;
            btnColorsHSL.Text = "HSL";
            btnColorsHSL.UseVisualStyleBackColor = true;
            btnColorsHSL.Click += btnColorsHSL_Click;
            // 
            // btnColorsRGB
            // 
            btnColorsRGB.Location = new Point(273, 187);
            btnColorsRGB.Name = "btnColorsRGB";
            btnColorsRGB.Size = new Size(88, 28);
            btnColorsRGB.TabIndex = 778;
            btnColorsRGB.Text = "RGB";
            btnColorsRGB.UseVisualStyleBackColor = true;
            btnColorsRGB.Click += btnColorsRGB_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(1187, 10);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(100, 100);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 776;
            pictureBox1.TabStop = false;
            pictureBox1.DoubleClick += pictureBox1_DoubleClick;
            // 
            // numGamma
            // 
            numGamma.DecimalPlaces = 2;
            numGamma.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numGamma.Location = new Point(741, 75);
            numGamma.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            numGamma.Name = "numGamma";
            numGamma.Size = new Size(61, 23);
            numGamma.TabIndex = 774;
            numGamma.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // numMaxPixelDist
            // 
            numMaxPixelDist.Location = new Point(741, 46);
            numMaxPixelDist.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numMaxPixelDist.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numMaxPixelDist.Name = "numMaxPixelDist";
            numMaxPixelDist.Size = new Size(61, 23);
            numMaxPixelDist.TabIndex = 775;
            numMaxPixelDist.Value = new decimal(new int[] { 127, 0, 0, 0 });
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(660, 77);
            label12.Name = "label12";
            label12.Size = new Size(48, 15);
            label12.TabIndex = 772;
            label12.Text = "gamma";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(660, 48);
            label10.Name = "label10";
            label10.Size = new Size(75, 15);
            label10.TabIndex = 773;
            label10.Text = "maxPixelDist";
            // 
            // panel5
            // 
            panel5.BorderStyle = BorderStyle.FixedSingle;
            panel5.Controls.Add(cbLoadFCache);
            panel5.Controls.Add(rbShiftC);
            panel5.Controls.Add(rbShiftMP);
            panel5.Controls.Add(cbShiftImage);
            panel5.Controls.Add(cbDiffCol);
            panel5.Controls.Add(label13);
            panel5.Controls.Add(numExtendRegion);
            panel5.Controls.Add(cbBlackBG);
            panel5.Controls.Add(cbWholeRegionPic);
            panel5.Controls.Add(cbUseCustomReBlendPic);
            panel5.Controls.Add(label9);
            panel5.Controls.Add(btnReBlend);
            panel5.Controls.Add(btnLoadCustomPenStrokesPic);
            panel5.Location = new Point(822, 11);
            panel5.Name = "panel5";
            panel5.Size = new Size(350, 174);
            panel5.TabIndex = 771;
            // 
            // cbLoadFCache
            // 
            cbLoadFCache.AutoSize = true;
            cbLoadFCache.Location = new Point(260, 118);
            cbLoadFCache.Name = "cbLoadFCache";
            cbLoadFCache.Size = new Size(85, 19);
            cbLoadFCache.TabIndex = 775;
            cbLoadFCache.Text = "fromCache";
            cbLoadFCache.UseVisualStyleBackColor = true;
            // 
            // rbShiftC
            // 
            rbShiftC.AutoSize = true;
            rbShiftC.Location = new Point(214, 144);
            rbShiftC.Name = "rbShiftC";
            rbShiftC.Size = new Size(65, 19);
            rbShiftC.TabIndex = 774;
            rbShiftC.Text = "custom";
            rbShiftC.UseVisualStyleBackColor = true;
            // 
            // rbShiftMP
            // 
            rbShiftMP.AutoSize = true;
            rbShiftMP.Checked = true;
            rbShiftMP.Location = new Point(103, 144);
            rbShiftMP.Name = "rbShiftMP";
            rbShiftMP.Size = new Size(105, 19);
            rbShiftMP.TabIndex = 773;
            rbShiftMP.TabStop = true;
            rbShiftMP.Text = "mapped Points";
            rbShiftMP.UseVisualStyleBackColor = true;
            // 
            // cbShiftImage
            // 
            cbShiftImage.AutoSize = true;
            cbShiftImage.Location = new Point(15, 145);
            cbShiftImage.Name = "cbShiftImage";
            cbShiftImage.Size = new Size(82, 19);
            cbShiftImage.TabIndex = 772;
            cbShiftImage.Text = "shift pic to";
            cbShiftImage.UseVisualStyleBackColor = true;
            // 
            // cbDiffCol
            // 
            cbDiffCol.AutoSize = true;
            cbDiffCol.Location = new Point(194, 60);
            cbDiffCol.Name = "cbDiffCol";
            cbDiffCol.Size = new Size(103, 19);
            cbDiffCol.TabIndex = 771;
            cbDiffCol.Text = "different Color";
            cbDiffCol.UseVisualStyleBackColor = true;
            cbDiffCol.CheckedChanged += cbDiffCol_CheckedChanged;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Enabled = false;
            label13.Location = new Point(224, 36);
            label13.Name = "label13";
            label13.Size = new Size(43, 15);
            label13.TabIndex = 770;
            label13.Text = "extend";
            // 
            // numExtendRegion
            // 
            numExtendRegion.Enabled = false;
            numExtendRegion.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numExtendRegion.Location = new Point(271, 34);
            numExtendRegion.Name = "numExtendRegion";
            numExtendRegion.Size = new Size(61, 23);
            numExtendRegion.TabIndex = 769;
            numExtendRegion.ValueChanged += numExtendRegion_ValueChanged;
            // 
            // cbBlackBG
            // 
            cbBlackBG.AutoSize = true;
            cbBlackBG.Location = new Point(7, 60);
            cbBlackBG.Name = "cbBlackBG";
            cbBlackBG.Size = new Size(181, 19);
            cbBlackBG.TabIndex = 768;
            cbBlackBG.Text = "redraw om black background";
            cbBlackBG.UseVisualStyleBackColor = true;
            cbBlackBG.CheckedChanged += cbBlackBG_CheckedChanged;
            // 
            // cbWholeRegionPic
            // 
            cbWholeRegionPic.AutoSize = true;
            cbWholeRegionPic.Location = new Point(7, 30);
            cbWholeRegionPic.Name = "cbWholeRegionPic";
            cbWholeRegionPic.Size = new Size(188, 19);
            cbWholeRegionPic.TabIndex = 767;
            cbWholeRegionPic.Text = "region surrounding penstrokes";
            cbWholeRegionPic.UseVisualStyleBackColor = true;
            cbWholeRegionPic.CheckedChanged += cbWholeRegionPic_CheckedChanged;
            // 
            // cbUseCustomReBlendPic
            // 
            cbUseCustomReBlendPic.AutoSize = true;
            cbUseCustomReBlendPic.Location = new Point(7, 90);
            cbUseCustomReBlendPic.Name = "cbUseCustomReBlendPic";
            cbUseCustomReBlendPic.Size = new Size(162, 19);
            cbUseCustomReBlendPic.TabIndex = 765;
            cbUseCustomReBlendPic.Text = "use custon Penstrokes pic";
            cbUseCustomReBlendPic.UseVisualStyleBackColor = true;
            cbUseCustomReBlendPic.CheckedChanged += cbUseCustomReBlendPic_CheckedChanged;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(3, 9);
            label9.Name = "label9";
            label9.Size = new Size(130, 15);
            label9.TabIndex = 764;
            label9.Text = "Re-Blend all Penstrokes";
            // 
            // btnReBlend
            // 
            btnReBlend.Location = new Point(244, 3);
            btnReBlend.Name = "btnReBlend";
            btnReBlend.Size = new Size(88, 28);
            btnReBlend.TabIndex = 756;
            btnReBlend.Text = "Go";
            btnReBlend.UseVisualStyleBackColor = true;
            btnReBlend.Click += btnReBlend_Click;
            // 
            // btnLoadCustomPenStrokesPic
            // 
            btnLoadCustomPenStrokesPic.Location = new Point(244, 84);
            btnLoadCustomPenStrokesPic.Name = "btnLoadCustomPenStrokesPic";
            btnLoadCustomPenStrokesPic.Size = new Size(88, 28);
            btnLoadCustomPenStrokesPic.TabIndex = 756;
            btnLoadCustomPenStrokesPic.Text = "load";
            btnLoadCustomPenStrokesPic.UseVisualStyleBackColor = true;
            btnLoadCustomPenStrokesPic.Click += btnLoadCustomPenStrokesPic_Click;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label8);
            panel2.Controls.Add(label7);
            panel2.Location = new Point(41, 119);
            panel2.Name = "panel2";
            panel2.Size = new Size(216, 64);
            panel2.TabIndex = 770;
            panel2.Visible = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(5, 22);
            label8.Name = "label8";
            label8.Size = new Size(169, 30);
            label8.TabIndex = 0;
            label8.Text = "right MouseButton: Add point \r\nand draw path";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(5, 2);
            label7.Name = "label7";
            label7.Size = new Size(199, 15);
            label7.TabIndex = 0;
            label7.Text = "left MouseButton: Add point to path";
            // 
            // cbClickMode
            // 
            cbClickMode.AutoSize = true;
            cbClickMode.Enabled = false;
            cbClickMode.Location = new Point(225, 18);
            cbClickMode.Name = "cbClickMode";
            cbClickMode.Size = new Size(79, 19);
            cbClickMode.TabIndex = 769;
            cbClickMode.Text = "ClickDraw";
            cbClickMode.UseVisualStyleBackColor = true;
            cbClickMode.CheckedChanged += cbClickMode_CheckedChanged;
            // 
            // btnFindDestPoint
            // 
            btnFindDestPoint.Location = new Point(367, 88);
            btnFindDestPoint.Name = "btnFindDestPoint";
            btnFindDestPoint.Size = new Size(88, 28);
            btnFindDestPoint.TabIndex = 767;
            btnFindDestPoint.Text = "Find";
            btnFindDestPoint.UseVisualStyleBackColor = true;
            btnFindDestPoint.Click += btnFindDestPoint_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(527, 153);
            label6.Name = "label6";
            label6.Size = new Size(175, 15);
            label6.TabIndex = 765;
            label6.Text = "save a pic with all drawn strokes";
            // 
            // btnSaveStrokes
            // 
            btnSaveStrokes.Location = new Point(714, 146);
            btnSaveStrokes.Name = "btnSaveStrokes";
            btnSaveStrokes.Size = new Size(88, 28);
            btnSaveStrokes.TabIndex = 764;
            btnSaveStrokes.Text = "Go";
            btnSaveStrokes.UseVisualStyleBackColor = true;
            btnSaveStrokes.Click += btnSaveStrokes_Click;
            // 
            // btnScreenBlend
            // 
            btnScreenBlend.Location = new Point(473, 186);
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
            label5.Location = new Point(902, 193);
            label5.Name = "label5";
            label5.Size = new Size(48, 15);
            label5.TabIndex = 762;
            label5.Text = "Opacity";
            // 
            // cbOverlay
            // 
            cbOverlay.AutoSize = true;
            cbOverlay.Location = new Point(795, 192);
            cbOverlay.Name = "cbOverlay";
            cbOverlay.Size = new Size(101, 19);
            cbOverlay.TabIndex = 761;
            cbOverlay.Text = "overlay src pic";
            cbOverlay.UseVisualStyleBackColor = true;
            cbOverlay.CheckedChanged += cbOverlay_CheckedChanged;
            // 
            // btnLoadOrig
            // 
            btnLoadOrig.Location = new Point(117, 186);
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
            numOpacity.Location = new Point(956, 191);
            numOpacity.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numOpacity.Name = "numOpacity";
            numOpacity.Size = new Size(61, 23);
            numOpacity.TabIndex = 759;
            numOpacity.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            numOpacity.ValueChanged += numOpacity_ValueChanged;
            // 
            // button12
            // 
            button12.Location = new Point(13, 186);
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
            numLowerWeight.Location = new Point(583, 77);
            numLowerWeight.Name = "numLowerWeight";
            numLowerWeight.Size = new Size(61, 23);
            numLowerWeight.TabIndex = 755;
            numLowerWeight.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // numUpperWeight
            // 
            numUpperWeight.DecimalPlaces = 2;
            numUpperWeight.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numUpperWeight.Location = new Point(583, 46);
            numUpperWeight.Name = "numUpperWeight";
            numUpperWeight.Size = new Size(61, 23);
            numUpperWeight.TabIndex = 755;
            numUpperWeight.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(473, 79);
            label4.Name = "label4";
            label4.Size = new Size(101, 15);
            label4.TabIndex = 754;
            label4.Text = "lower Img Weight";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(473, 48);
            label3.Name = "label3";
            label3.Size = new Size(103, 15);
            label3.TabIndex = 754;
            label3.Text = "upper Img Weight";
            // 
            // cmbAlg
            // 
            cmbAlg.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAlg.FormattingEnabled = true;
            cmbAlg.Items.AddRange(new object[] { "AddB", "NormalB", "MaxB", "FixedWeightB", "NormalDynamicWeightB", "AddDynamicWeightB" });
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
            rbDest.Location = new Point(250, 68);
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
            rbSrc.Location = new Point(192, 68);
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
            cbAuto.Location = new Point(124, 69);
            cbAuto.Name = "cbAuto";
            cbAuto.Size = new Size(52, 19);
            cbAuto.TabIndex = 749;
            cbAuto.Text = "Auto";
            cbAuto.UseVisualStyleBackColor = true;
            // 
            // cbSetPts
            // 
            cbSetPts.AutoSize = true;
            cbSetPts.Location = new Point(41, 69);
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
            cbDraw.CheckedChanged += cbDraw_CheckedChanged;
            // 
            // numPenSize
            // 
            numPenSize.DecimalPlaces = 2;
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
            label1.Location = new Point(1329, 82);
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
            splitContainer1.Location = new Point(0, 229);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(helplineRulerCtrl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(helplineRulerCtrl2);
            splitContainer1.Size = new Size(1500, 664);
            splitContainer1.SplitterDistance = 757;
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
            helplineRulerCtrl2.Size = new Size(739, 664);
            helplineRulerCtrl2.TabIndex = 249;
            helplineRulerCtrl2.Zoom = 1F;
            helplineRulerCtrl2.ZoomSetManually = false;
            helplineRulerCtrl2.DBPanelDblClicked += helplineRulerCtrl2_DBPanelDblClicked;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // colorDialog1
            // 
            colorDialog1.AnyColor = true;
            colorDialog1.FullOpen = true;
            // 
            // frmPoissonDraw
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1500, 932);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(panel3);
            Controls.Add(statusStrip1);
            Name = "frmPoissonDraw";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmPoissonDraw";
            FormClosing += Form1_FormClosing;
            Load += frmPoissonDraw_Load;
            ((System.ComponentModel.ISupportInitialize)numSrcPtSurround).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numOpacityOrdDraw).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numGamma).EndInit();
            ((System.ComponentModel.ISupportInitialize)numMaxPixelDist).EndInit();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numExtendRegion).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
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
        private CheckBox cbFindDestPoint;
        internal Button btnFindDestPoint;
        private NumericUpDown numSrcPtSurround;
        internal System.ComponentModel.BackgroundWorker backgroundWorker2;
        private CheckBox cbClickMode;
        private Panel panel2;
        private Label label8;
        private Label label7;
        private Panel panel5;
        private CheckBox cbUseCustomReBlendPic;
        private Label label9;
        internal Button btnReBlend;
        internal Button btnLoadCustomPenStrokesPic;
        private NumericUpDown numGamma;
        private NumericUpDown numMaxPixelDist;
        private Label label12;
        private Label label10;
        private CheckBox cbWholeRegionPic;
        private PictureBox pictureBox1;
        private CheckBox cbBlackBG;
        private Label label13;
        private NumericUpDown numExtendRegion;
        private CheckBox cbDiffCol;
        internal ColorDialog colorDialog1;
        private Button btnColorsHSL;
        private Button btnColorsRGB;
        private CheckBox cbFindInOrig;
        private Button btnHSL2;
        private Button btnLoadEdited;
        private CheckBox cbOrdinaryDraw;
        private Label label14;
        private NumericUpDown numOpacityOrdDraw;
        private Button btnSaveHLC1;
        private ToolStripStatusLabel toolStripStatusLabel6;
        private RadioButton rbShiftC;
        private RadioButton rbShiftMP;
        private CheckBox cbShiftImage;
        private CheckBox cbLoadFCache;
        private CheckBox cbShowCustPic;
    }
}