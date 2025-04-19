namespace AvoidAGrabCutEasy
{
    partial class frmExtendOutlinesExtended
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
            splitContainer1 = new SplitContainer();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel5 = new ToolStripStatusLabel();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            cbKeepRightAngles = new CheckBox();
            label7 = new Label();
            label6 = new Label();
            cbSelSingleClick = new CheckBox();
            btnSelNone = new Button();
            btnClearPaths = new Button();
            btnSelAll = new Button();
            cbRestrict = new CheckBox();
            numRestrict = new NumericUpDown();
            label1 = new Label();
            numJRem2 = new NumericUpDown();
            numJRem1 = new NumericUpDown();
            label43 = new Label();
            label44 = new Label();
            checkedListBox1 = new CheckedListBox();
            btnRedo = new Button();
            button1 = new Button();
            btnUndo = new Button();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            button10 = new Button();
            button8 = new Button();
            button2 = new Button();
            btnCancel = new Button();
            btnOK = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            Timer3 = new System.Windows.Forms.Timer(components);
            toolTip1 = new ToolTip(components);
            saveFileDialog1 = new SaveFileDialog();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRestrict).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numJRem2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numJRem1).BeginInit();
            SuspendLayout();
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
            splitContainer1.Panel2.Controls.Add(cbKeepRightAngles);
            splitContainer1.Panel2.Controls.Add(label7);
            splitContainer1.Panel2.Controls.Add(label6);
            splitContainer1.Panel2.Controls.Add(cbSelSingleClick);
            splitContainer1.Panel2.Controls.Add(btnSelNone);
            splitContainer1.Panel2.Controls.Add(btnClearPaths);
            splitContainer1.Panel2.Controls.Add(btnSelAll);
            splitContainer1.Panel2.Controls.Add(cbRestrict);
            splitContainer1.Panel2.Controls.Add(numRestrict);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(numJRem2);
            splitContainer1.Panel2.Controls.Add(numJRem1);
            splitContainer1.Panel2.Controls.Add(label43);
            splitContainer1.Panel2.Controls.Add(label44);
            splitContainer1.Panel2.Controls.Add(checkedListBox1);
            splitContainer1.Panel2.Controls.Add(btnRedo);
            splitContainer1.Panel2.Controls.Add(button1);
            splitContainer1.Panel2.Controls.Add(btnUndo);
            splitContainer1.Panel2.Controls.Add(Label20);
            splitContainer1.Panel2.Controls.Add(cmbZoom);
            splitContainer1.Panel2.Controls.Add(cbBGColor);
            splitContainer1.Panel2.Controls.Add(button10);
            splitContainer1.Panel2.Controls.Add(button8);
            splitContainer1.Panel2.Controls.Add(button2);
            splitContainer1.Panel2.Controls.Add(btnCancel);
            splitContainer1.Panel2.Controls.Add(btnOK);
            splitContainer1.Size = new Size(1200, 760);
            splitContainer1.SplitterDistance = 848;
            splitContainer1.TabIndex = 0;
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
            helplineRulerCtrl1.Size = new Size(848, 716);
            helplineRulerCtrl1.TabIndex = 0;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripProgressBar1, toolStripStatusLabel5, toolStripStatusLabel4 });
            statusStrip1.Location = new Point(0, 716);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(848, 44);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(41, 39);
            toolStripStatusLabel1.Text = "    ";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.AutoSize = false;
            toolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(100, 39);
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 38);
            // 
            // toolStripStatusLabel5
            // 
            toolStripStatusLabel5.Name = "toolStripStatusLabel5";
            toolStripStatusLabel5.Size = new Size(19, 39);
            toolStripStatusLabel5.Text = "    ";
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Font = new Font("Segoe UI", 15.75F);
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(37, 39);
            toolStripStatusLabel4.Text = "    ";
            // 
            // cbKeepRightAngles
            // 
            cbKeepRightAngles.AutoSize = true;
            cbKeepRightAngles.Checked = true;
            cbKeepRightAngles.CheckState = CheckState.Checked;
            cbKeepRightAngles.Location = new Point(42, 529);
            cbKeepRightAngles.Name = "cbKeepRightAngles";
            cbKeepRightAngles.Size = new Size(115, 19);
            cbKeepRightAngles.TabIndex = 673;
            cbKeepRightAngles.Text = "keepRightAngles";
            cbKeepRightAngles.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(138, 302);
            label7.Name = "label7";
            label7.Size = new Size(16, 15);
            label7.TabIndex = 671;
            label7.Text = "...";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(22, 302);
            label6.Name = "label6";
            label6.Size = new Size(16, 15);
            label6.TabIndex = 672;
            label6.Text = "...";
            // 
            // cbSelSingleClick
            // 
            cbSelSingleClick.AutoSize = true;
            cbSelSingleClick.Location = new Point(22, 358);
            cbSelSingleClick.Name = "cbSelSingleClick";
            cbSelSingleClick.Size = new Size(131, 19);
            cbSelSingleClick.TabIndex = 665;
            cbSelSingleClick.Text = "SelectOnSingleClick";
            cbSelSingleClick.UseVisualStyleBackColor = true;
            cbSelSingleClick.CheckedChanged += cbSelSingleClick_CheckedChanged;
            // 
            // btnSelNone
            // 
            btnSelNone.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelNone.ForeColor = SystemColors.ControlText;
            btnSelNone.Location = new Point(245, 383);
            btnSelNone.Margin = new Padding(4, 3, 4, 3);
            btnSelNone.Name = "btnSelNone";
            btnSelNone.Size = new Size(88, 27);
            btnSelNone.TabIndex = 667;
            btnSelNone.Text = "UnselectAll";
            btnSelNone.UseVisualStyleBackColor = true;
            btnSelNone.Click += btnSelNone_Click;
            // 
            // btnClearPaths
            // 
            btnClearPaths.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearPaths.ForeColor = SystemColors.ControlText;
            btnClearPaths.Location = new Point(82, 416);
            btnClearPaths.Margin = new Padding(4, 3, 4, 3);
            btnClearPaths.Name = "btnClearPaths";
            btnClearPaths.Size = new Size(88, 27);
            btnClearPaths.TabIndex = 668;
            btnClearPaths.Text = "ClearPaths";
            btnClearPaths.UseVisualStyleBackColor = true;
            btnClearPaths.Click += btnClearPaths_Click;
            // 
            // btnSelAll
            // 
            btnSelAll.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSelAll.ForeColor = SystemColors.ControlText;
            btnSelAll.Location = new Point(81, 383);
            btnSelAll.Margin = new Padding(4, 3, 4, 3);
            btnSelAll.Name = "btnSelAll";
            btnSelAll.Size = new Size(88, 27);
            btnSelAll.TabIndex = 669;
            btnSelAll.Text = "SelectAll";
            btnSelAll.UseVisualStyleBackColor = true;
            btnSelAll.Click += btnSelAll_Click;
            // 
            // cbRestrict
            // 
            cbRestrict.AutoSize = true;
            cbRestrict.Location = new Point(22, 333);
            cbRestrict.Name = "cbRestrict";
            cbRestrict.Size = new Size(158, 19);
            cbRestrict.TabIndex = 666;
            cbRestrict.Text = "restrict amount chains to";
            cbRestrict.UseVisualStyleBackColor = true;
            // 
            // numRestrict
            // 
            numRestrict.Location = new Point(186, 332);
            numRestrict.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numRestrict.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRestrict.Name = "numRestrict";
            numRestrict.Size = new Size(88, 23);
            numRestrict.TabIndex = 670;
            numRestrict.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(24, 510);
            label1.Name = "label1";
            label1.Size = new Size(138, 15);
            label1.TabIndex = 664;
            label1.Text = "Process selected outlines";
            // 
            // numJRem2
            // 
            numJRem2.Location = new Point(220, 468);
            numJRem2.Margin = new Padding(4, 3, 4, 3);
            numJRem2.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numJRem2.Name = "numJRem2";
            numJRem2.Size = new Size(56, 23);
            numJRem2.TabIndex = 660;
            // 
            // numJRem1
            // 
            numJRem1.Location = new Point(79, 468);
            numJRem1.Margin = new Padding(4, 3, 4, 3);
            numJRem1.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numJRem1.Name = "numJRem1";
            numJRem1.Size = new Size(56, 23);
            numJRem1.TabIndex = 661;
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.Location = new Point(24, 472);
            label43.Margin = new Padding(4, 0, 4, 0);
            label43.Name = "label43";
            label43.Size = new Size(47, 15);
            label43.TabIndex = 662;
            label43.Text = "remove";
            // 
            // label44
            // 
            label44.AutoSize = true;
            label44.Location = new Point(160, 472);
            label44.Margin = new Padding(4, 0, 4, 0);
            label44.Name = "label44";
            label44.Size = new Size(46, 15);
            label44.TabIndex = 663;
            label44.Text = "extend ";
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(24, 12);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(252, 274);
            checkedListBox1.TabIndex = 659;
            checkedListBox1.SelectedIndexChanged += checkedListBox1_SelectedIndexChanged;
            // 
            // btnRedo
            // 
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(188, 565);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 657;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // button1
            // 
            button1.ForeColor = SystemColors.ControlText;
            button1.Location = new Point(188, 504);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(88, 27);
            button1.TabIndex = 658;
            button1.Text = "Go";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // btnUndo
            // 
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(92, 565);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 658;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(125, 680);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 656;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(189, 677);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 655;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(113, 619);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 654;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // button10
            // 
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(188, 644);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 653;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(93, 644);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 652;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(188, 615);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 651;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(188, 721);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 647;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(95, 721);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 648;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // Timer3
            // 
            Timer3.Interval = 500;
            Timer3.Tick += Timer3_Tick;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // frmExtendOutlinesExtended
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1200, 760);
            Controls.Add(splitContainer1);
            Name = "frmExtendOutlinesExtended";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmExtendOutlinesExtended";
            FormClosing += frmProcOutline_FormClosing;
            Load += frmExtendOutlinesExtended_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRestrict).EndInit();
            ((System.ComponentModel.ISupportInitialize)numJRem2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numJRem1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private Button btnCancel;
        private Button btnOK;
        internal Label Label20;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        private Button button10;
        private Button button8;
        internal Button button2;
        private Button btnRedo;
        private Button btnUndo;
        private CheckedListBox checkedListBox1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel5;
        private ToolStripStatusLabel toolStripStatusLabel4;
        internal System.ComponentModel.BackgroundWorker backgroundWorker1;
        internal System.Windows.Forms.Timer Timer3;
        private ToolTip toolTip1;
        private SaveFileDialog saveFileDialog1;
        private NumericUpDown numJRem2;
        private NumericUpDown numJRem1;
        private Label label43;
        private Label label44;
        private Label label1;
        private Button button1;
        private Label label7;
        private Label label6;
        private CheckBox cbSelSingleClick;
        private Button btnSelNone;
        private Button btnClearPaths;
        private Button btnSelAll;
        private CheckBox cbRestrict;
        private NumericUpDown numRestrict;
        internal System.ComponentModel.BackgroundWorker backgroundWorker2;
        private CheckBox cbKeepRightAngles;
    }
}