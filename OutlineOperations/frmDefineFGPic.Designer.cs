namespace OutlineOperations
{
    partial class frmDefineFGPic
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
            btnRedo = new Button();
            btnUndo = new Button();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            toolTip1 = new ToolTip(components);
            helplineRulerCtrl2 = new HelplineRulerControl.HelplineRulerCtrl();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            splitContainer1 = new SplitContainer();
            openFileDialog1 = new OpenFileDialog();
            panel1 = new Panel();
            cbSetOpaque = new CheckBox();
            label3 = new Label();
            cbDraw = new CheckBox();
            numPenW = new NumericUpDown();
            numTH = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            cbOverlay = new CheckBox();
            btnRedoDraw = new Button();
            btnUndoDraw = new Button();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            button10 = new Button();
            button8 = new Button();
            button2 = new Button();
            btnCancel = new Button();
            btnOK = new Button();
            btnCrop = new Button();
            btnGo = new Button();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            panel3 = new Panel();
            toolStripStatusLabel5 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            ToolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            statusStrip1 = new StatusStrip();
            saveFileDialog1 = new SaveFileDialog();
            Timer3 = new System.Windows.Forms.Timer(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            label5 = new Label();
            btnSetMatteFGOpaque = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPenW).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTH).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // btnRedo
            // 
            btnRedo.Enabled = false;
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(1178, 72);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 708;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // btnUndo
            // 
            btnUndo.Enabled = false;
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(1082, 72);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 707;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
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
            helplineRulerCtrl2.HandleMeasureByContainingForm = false;
            helplineRulerCtrl2.IgnoreZoom = false;
            helplineRulerCtrl2.Location = new Point(0, 0);
            helplineRulerCtrl2.Margin = new Padding(5, 3, 5, 3);
            helplineRulerCtrl2.Measure = false;
            helplineRulerCtrl2.MoveHelpLinesOnResize = false;
            helplineRulerCtrl2.Name = "helplineRulerCtrl2";
            helplineRulerCtrl2.PtEnd = new Point(0, 0);
            helplineRulerCtrl2.PtSt = new Point(0, 0);
            helplineRulerCtrl2.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl2.Size = new Size(621, 658);
            helplineRulerCtrl2.TabIndex = 0;
            helplineRulerCtrl2.Zoom = 1F;
            helplineRulerCtrl2.ZoomSetManually = false;
            helplineRulerCtrl2.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
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
            helplineRulerCtrl1.HandleMeasureByContainingForm = false;
            helplineRulerCtrl1.IgnoreZoom = false;
            helplineRulerCtrl1.Location = new Point(0, 0);
            helplineRulerCtrl1.Margin = new Padding(5, 3, 5, 3);
            helplineRulerCtrl1.Measure = false;
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.PtEnd = new Point(0, 0);
            helplineRulerCtrl1.PtSt = new Point(0, 0);
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(653, 658);
            helplineRulerCtrl1.TabIndex = 0;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 159);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(helplineRulerCtrl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(helplineRulerCtrl2);
            splitContainer1.Size = new Size(1279, 658);
            splitContainer1.SplitterDistance = 653;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 231;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png";
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(label5);
            panel1.Controls.Add(btnSetMatteFGOpaque);
            panel1.Controls.Add(cbSetOpaque);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(cbDraw);
            panel1.Controls.Add(numPenW);
            panel1.Controls.Add(numTH);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(cbOverlay);
            panel1.Controls.Add(btnRedoDraw);
            panel1.Controls.Add(btnRedo);
            panel1.Controls.Add(btnUndoDraw);
            panel1.Controls.Add(btnUndo);
            panel1.Controls.Add(Label20);
            panel1.Controls.Add(cmbZoom);
            panel1.Controls.Add(cbBGColor);
            panel1.Controls.Add(button10);
            panel1.Controls.Add(button8);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(btnCancel);
            panel1.Controls.Add(btnOK);
            panel1.Controls.Add(btnCrop);
            panel1.Controls.Add(btnGo);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(1279, 159);
            panel1.TabIndex = 230;
            panel1.MouseDoubleClick += panel1_MouseDoubleClick;
            panel1.MouseDown += panel1_MouseDown;
            // 
            // cbSetOpaque
            // 
            cbSetOpaque.AutoSize = true;
            cbSetOpaque.Location = new Point(453, 50);
            cbSetOpaque.Name = "cbSetOpaque";
            cbSetOpaque.Size = new Size(133, 19);
            cbSetOpaque.TabIndex = 721;
            cbSetOpaque.Text = "set FG alpha opaque";
            cbSetOpaque.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(204, 52);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 714;
            label3.Text = "PenSize";
            // 
            // cbDraw
            // 
            cbDraw.AutoSize = true;
            cbDraw.Location = new Point(23, 51);
            cbDraw.Name = "cbDraw";
            cbDraw.Size = new Size(172, 19);
            cbDraw.TabIndex = 713;
            cbDraw.Text = "Click Draw  boundary to pic";
            cbDraw.UseVisualStyleBackColor = true;
            // 
            // numPenW
            // 
            numPenW.Location = new Point(257, 50);
            numPenW.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numPenW.Name = "numPenW";
            numPenW.Size = new Size(65, 23);
            numPenW.TabIndex = 712;
            numPenW.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // numTH
            // 
            numTH.Location = new Point(222, 17);
            numTH.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numTH.Name = "numTH";
            numTH.Size = new Size(65, 23);
            numTH.TabIndex = 712;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(143, 19);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 711;
            label2.Text = "Threshold";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(23, 19);
            label1.Name = "label1";
            label1.Size = new Size(101, 15);
            label1.TabIndex = 710;
            label1.Text = "Get biggest Chain";
            // 
            // cbOverlay
            // 
            cbOverlay.AutoSize = true;
            cbOverlay.Location = new Point(23, 122);
            cbOverlay.Name = "cbOverlay";
            cbOverlay.Size = new Size(88, 19);
            cbOverlay.TabIndex = 709;
            cbOverlay.Text = "overlay img";
            cbOverlay.UseVisualStyleBackColor = true;
            // 
            // btnRedoDraw
            // 
            btnRedoDraw.ForeColor = SystemColors.ControlText;
            btnRedoDraw.Location = new Point(143, 76);
            btnRedoDraw.Margin = new Padding(4, 3, 4, 3);
            btnRedoDraw.Name = "btnRedoDraw";
            btnRedoDraw.Size = new Size(88, 27);
            btnRedoDraw.TabIndex = 708;
            btnRedoDraw.Text = "RedoDraw";
            btnRedoDraw.UseVisualStyleBackColor = true;
            btnRedoDraw.Click += btnRedoDraw_Click;
            // 
            // btnUndoDraw
            // 
            btnUndoDraw.ForeColor = SystemColors.ControlText;
            btnUndoDraw.Location = new Point(47, 76);
            btnUndoDraw.Margin = new Padding(4, 3, 4, 3);
            btnUndoDraw.Name = "btnUndoDraw";
            btnUndoDraw.Size = new Size(88, 27);
            btnUndoDraw.TabIndex = 707;
            btnUndoDraw.Text = "UndoDraw";
            btnUndoDraw.UseVisualStyleBackColor = true;
            btnUndoDraw.Click += btnUndoDraw_Click;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(876, 14);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 650;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(942, 11);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 649;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(1082, 13);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 648;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // button10
            // 
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(1178, 39);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 647;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(1082, 39);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 646;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(1178, 6);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 645;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(1178, 126);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 643;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(1081, 126);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 644;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += Button28_Click;
            // 
            // btnCrop
            // 
            btnCrop.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCrop.Location = new Point(339, 46);
            btnCrop.Margin = new Padding(4, 3, 4, 3);
            btnCrop.Name = "btnCrop";
            btnCrop.Size = new Size(88, 27);
            btnCrop.TabIndex = 642;
            btnCrop.Text = "Crop";
            btnCrop.UseVisualStyleBackColor = true;
            btnCrop.Click += btnCrop_Click;
            // 
            // btnGo
            // 
            btnGo.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGo.Location = new Point(308, 13);
            btnGo.Margin = new Padding(4, 3, 4, 3);
            btnGo.Name = "btnGo";
            btnGo.Size = new Size(88, 27);
            btnGo.TabIndex = 642;
            btnGo.Text = "Go";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += btnGo_Click;
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Font = new Font("Segoe UI", 16F);
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(63, 34);
            toolStripStatusLabel4.Text = "Hallo";
            // 
            // panel3
            // 
            panel3.Location = new Point(0, 0);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(233, 28);
            panel3.TabIndex = 233;
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new Size(19, 34);
            toolStripStatusLabel5.Text = "    ";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(467, 33);
            toolStripProgressBar1.Visible = false;
            // 
            // ToolStripStatusLabel2
            // 
            ToolStripStatusLabel2.AutoSize = false;
            ToolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            ToolStripStatusLabel2.Name = "ToolStripStatusLabel2";
            ToolStripStatusLabel2.Size = new Size(100, 34);
            ToolStripStatusLabel2.Text = "    ";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("Segoe UI", 16F);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(63, 34);
            toolStripStatusLabel1.Text = "Hallo";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.AutoSize = false;
            toolStripStatusLabel3.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(100, 34);
            toolStripStatusLabel3.Text = "    ";
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, ToolStripStatusLabel2, toolStripProgressBar1, toolStripStatusLabel5, toolStripStatusLabel4, toolStripStatusLabel3 });
            statusStrip1.Location = new Point(0, 817);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1279, 39);
            statusStrip1.TabIndex = 232;
            statusStrip1.Text = "statusStrip1";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // Timer3
            // 
            Timer3.Interval = 500;
            Timer3.Tick += Timer3_Tick;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(748, 50);
            label5.Name = "label5";
            label5.Size = new Size(19, 15);
            label5.TabIndex = 730;
            label5.Text = "    ";
            // 
            // btnSetMatteFGOpaque
            // 
            btnSetMatteFGOpaque.ForeColor = SystemColors.ControlText;
            btnSetMatteFGOpaque.Location = new Point(603, 45);
            btnSetMatteFGOpaque.Margin = new Padding(4, 3, 4, 3);
            btnSetMatteFGOpaque.Name = "btnSetMatteFGOpaque";
            btnSetMatteFGOpaque.Size = new Size(132, 27);
            btnSetMatteFGOpaque.TabIndex = 729;
            btnSetMatteFGOpaque.Text = "Set Matte FG Opaque";
            btnSetMatteFGOpaque.UseVisualStyleBackColor = true;
            btnSetMatteFGOpaque.Visible = false;
            btnSetMatteFGOpaque.Click += btnSetMatteFGOpaque_Click;
            // 
            // frmDefineFGPic
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1279, 856);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Controls.Add(panel3);
            Controls.Add(statusStrip1);
            Name = "frmDefineFGPic";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmDefineFGPic";
            FormClosing += Form1_FormClosing;
            Load += frmDefineFGPic_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numPenW).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTH).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolTip toolTip1;
        private Button btnRedo;
        private Button btnUndo;
        internal System.ComponentModel.BackgroundWorker backgroundWorker2;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl2;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private SplitContainer splitContainer1;
        private OpenFileDialog openFileDialog1;
        private Panel panel1;
        internal Label Label20;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        private Button button10;
        private Button button8;
        internal Button button2;
        private Button btnCancel;
        private Button btnOK;
        private Button btnGo;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private Panel panel3;
        private ToolStripStatusLabel toolStripStatusLabel5;
        private ToolStripProgressBar toolStripProgressBar1;
        internal ToolStripStatusLabel ToolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel1;
        internal ToolStripStatusLabel toolStripStatusLabel3;
        private StatusStrip statusStrip1;
        private SaveFileDialog saveFileDialog1;
        internal System.Windows.Forms.Timer Timer3;
        internal System.ComponentModel.BackgroundWorker backgroundWorker1;
        private CheckBox cbOverlay;
        private Label label3;
        private CheckBox cbDraw;
        private NumericUpDown numPenW;
        private NumericUpDown numTH;
        private Label label2;
        private Label label1;
        private Button btnCrop;
        private Button btnRedoDraw;
        private Button btnUndoDraw;
        internal CheckBox cbSetOpaque;
        private Label label5;
        private Button btnSetMatteFGOpaque;
    }
}