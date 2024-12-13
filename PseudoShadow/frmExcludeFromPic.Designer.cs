namespace PseudoShadow
{
    partial class frmExcludeFromPic
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
            if(this.checkedListBox1.Items.Count > 0)
            {
                for(int i = this.checkedListBox1.Items.Count - 1; i >= 0; i--)
                    ((ExcludedBmpRegion)this.checkedListBox1.Items[i]).Dispose();
            } 

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
            splitContainer2 = new SplitContainer();
            Label3 = new Label();
            numPenWidth = new NumericUpDown();
            btnRemPoint = new Button();
            btnRemSeg = new Button();
            btnNewPath = new Button();
            btnClosePath = new Button();
            btnAdd2 = new Button();
            btnShow2 = new Button();
            btnShow = new Button();
            btnAdd = new Button();
            label5 = new Label();
            label2 = new Label();
            numExceptBounds2 = new NumericUpDown();
            numExceptBounds = new NumericUpDown();
            label4 = new Label();
            label1 = new Label();
            cbDraw = new CheckBox();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            btnUndo = new Button();
            btnRedo = new Button();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            checkedListBox1 = new CheckedListBox();
            Button29 = new Button();
            Button28 = new Button();
            button10 = new Button();
            button8 = new Button();
            button2 = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            toolTip1 = new ToolTip(components);
            saveFileDialog1 = new SaveFileDialog();
            Timer3 = new System.Windows.Forms.Timer(components);
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numPenWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numExceptBounds2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numExceptBounds).BeginInit();
            statusStrip1.SuspendLayout();
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
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(checkedListBox1);
            splitContainer1.Panel2.Controls.Add(Button29);
            splitContainer1.Panel2.Controls.Add(Button28);
            splitContainer1.Panel2.Controls.Add(button10);
            splitContainer1.Panel2.Controls.Add(button8);
            splitContainer1.Panel2.Controls.Add(button2);
            splitContainer1.Size = new Size(1200, 750);
            splitContainer1.SplitterDistance = 958;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(Label3);
            splitContainer2.Panel1.Controls.Add(numPenWidth);
            splitContainer2.Panel1.Controls.Add(btnRemPoint);
            splitContainer2.Panel1.Controls.Add(btnRemSeg);
            splitContainer2.Panel1.Controls.Add(btnNewPath);
            splitContainer2.Panel1.Controls.Add(btnClosePath);
            splitContainer2.Panel1.Controls.Add(btnAdd2);
            splitContainer2.Panel1.Controls.Add(btnShow2);
            splitContainer2.Panel1.Controls.Add(btnShow);
            splitContainer2.Panel1.Controls.Add(btnAdd);
            splitContainer2.Panel1.Controls.Add(label5);
            splitContainer2.Panel1.Controls.Add(label2);
            splitContainer2.Panel1.Controls.Add(numExceptBounds2);
            splitContainer2.Panel1.Controls.Add(numExceptBounds);
            splitContainer2.Panel1.Controls.Add(label4);
            splitContainer2.Panel1.Controls.Add(label1);
            splitContainer2.Panel1.Controls.Add(cbDraw);
            splitContainer2.Panel1.Controls.Add(Label20);
            splitContainer2.Panel1.Controls.Add(cmbZoom);
            splitContainer2.Panel1.Controls.Add(cbBGColor);
            splitContainer2.Panel1.Controls.Add(btnUndo);
            splitContainer2.Panel1.Controls.Add(btnRedo);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(helplineRulerCtrl1);
            splitContainer2.Panel2.Controls.Add(statusStrip1);
            splitContainer2.Size = new Size(958, 750);
            splitContainer2.SplitterDistance = 152;
            splitContainer2.TabIndex = 0;
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new Point(511, 54);
            Label3.Margin = new Padding(4, 0, 4, 0);
            Label3.Name = "Label3";
            Label3.Size = new Size(63, 15);
            Label3.TabIndex = 310;
            Label3.Text = "Pen width:";
            // 
            // numPenWidth
            // 
            numPenWidth.DecimalPlaces = 2;
            numPenWidth.Location = new Point(581, 51);
            numPenWidth.Margin = new Padding(4, 3, 4, 3);
            numPenWidth.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numPenWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numPenWidth.Name = "numPenWidth";
            numPenWidth.Size = new Size(58, 23);
            numPenWidth.TabIndex = 309;
            numPenWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnRemPoint
            // 
            btnRemPoint.Location = new Point(402, 48);
            btnRemPoint.Margin = new Padding(4, 3, 4, 3);
            btnRemPoint.Name = "btnRemPoint";
            btnRemPoint.Size = new Size(88, 27);
            btnRemPoint.TabIndex = 306;
            btnRemPoint.Text = "remPt";
            btnRemPoint.UseVisualStyleBackColor = true;
            btnRemPoint.Click += btnRemPoint_Click;
            // 
            // btnRemSeg
            // 
            btnRemSeg.Location = new Point(306, 48);
            btnRemSeg.Margin = new Padding(4, 3, 4, 3);
            btnRemSeg.Name = "btnRemSeg";
            btnRemSeg.Size = new Size(88, 27);
            btnRemSeg.TabIndex = 308;
            btnRemSeg.Text = "remSeg";
            btnRemSeg.UseVisualStyleBackColor = true;
            btnRemSeg.Click += btnRemSeg_Click;
            // 
            // btnNewPath
            // 
            btnNewPath.Location = new Point(199, 47);
            btnNewPath.Margin = new Padding(4, 3, 4, 3);
            btnNewPath.Name = "btnNewPath";
            btnNewPath.Size = new Size(88, 27);
            btnNewPath.TabIndex = 305;
            btnNewPath.Text = "newPath";
            btnNewPath.UseVisualStyleBackColor = true;
            btnNewPath.Click += btnNewPath_Click;
            // 
            // btnClosePath
            // 
            btnClosePath.Location = new Point(103, 46);
            btnClosePath.Margin = new Padding(4, 3, 4, 3);
            btnClosePath.Name = "btnClosePath";
            btnClosePath.Size = new Size(88, 27);
            btnClosePath.TabIndex = 307;
            btnClosePath.Text = "closePath";
            btnClosePath.UseVisualStyleBackColor = true;
            btnClosePath.Click += btnClosePath_Click;
            // 
            // btnAdd2
            // 
            btnAdd2.Location = new Point(485, 83);
            btnAdd2.Name = "btnAdd2";
            btnAdd2.Size = new Size(75, 23);
            btnAdd2.TabIndex = 300;
            btnAdd2.Text = "add to list";
            btnAdd2.UseVisualStyleBackColor = true;
            btnAdd2.Click += btnAdd2_Click;
            // 
            // btnShow2
            // 
            btnShow2.Location = new Point(372, 83);
            btnShow2.Name = "btnShow2";
            btnShow2.Size = new Size(75, 23);
            btnShow2.TabIndex = 299;
            btnShow2.Text = "show";
            btnShow2.UseVisualStyleBackColor = true;
            btnShow2.Click += btnShow2_Click;
            // 
            // btnShow
            // 
            btnShow.Location = new Point(372, 10);
            btnShow.Name = "btnShow";
            btnShow.Size = new Size(75, 23);
            btnShow.TabIndex = 299;
            btnShow.Text = "show";
            btnShow.UseVisualStyleBackColor = true;
            btnShow.Click += btnShow_Click;
            // 
            // btnAdd
            // 
            btnAdd.Enabled = false;
            btnAdd.Location = new Point(485, 10);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(75, 23);
            btnAdd.TabIndex = 299;
            btnAdd.Text = "add to list";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(254, 87);
            label5.Name = "label5";
            label5.Size = new Size(102, 15);
            label5.TabIndex = 298;
            label5.Text = "px wide boundary";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(254, 15);
            label2.Name = "label2";
            label2.Size = new Size(102, 15);
            label2.TabIndex = 298;
            label2.Text = "px wide boundary";
            // 
            // numExceptBounds2
            // 
            numExceptBounds2.Location = new Point(149, 85);
            numExceptBounds2.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numExceptBounds2.Name = "numExceptBounds2";
            numExceptBounds2.Size = new Size(88, 23);
            numExceptBounds2.TabIndex = 297;
            numExceptBounds2.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // numExceptBounds
            // 
            numExceptBounds.Location = new Point(149, 12);
            numExceptBounds.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numExceptBounds.Name = "numExceptBounds";
            numExceptBounds.Size = new Size(88, 23);
            numExceptBounds.TabIndex = 297;
            numExceptBounds.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(38, 87);
            label4.Name = "label4";
            label4.Size = new Size(69, 15);
            label4.TabIndex = 296;
            label4.Text = "from path a";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 15);
            label1.Name = "label1";
            label1.Size = new Size(96, 15);
            label1.TabIndex = 296;
            label1.Text = "from whole pic a";
            // 
            // cbDraw
            // 
            cbDraw.AutoSize = true;
            cbDraw.Location = new Point(21, 53);
            cbDraw.Name = "cbDraw";
            cbDraw.Size = new Size(77, 19);
            cbDraw.TabIndex = 294;
            cbDraw.Text = "DrawPath";
            cbDraw.UseVisualStyleBackColor = true;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(783, 53);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 293;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(852, 49);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 292;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(872, 88);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 289;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // btnUndo
            // 
            btnUndo.Enabled = false;
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(756, 12);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 285;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // btnRedo
            // 
            btnRedo.Enabled = false;
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(851, 12);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 286;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
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
            helplineRulerCtrl1.Size = new Size(958, 559);
            helplineRulerCtrl1.TabIndex = 1;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripProgressBar1, toolStripStatusLabel2 });
            statusStrip1.Location = new Point(0, 559);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(958, 35);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Font = new Font("Segoe UI", 16F);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(215, 30);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(100, 29);
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Font = new Font("Segoe UI", 16F);
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(215, 30);
            toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // checkedListBox1
            // 
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.Location = new Point(23, 88);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(183, 130);
            checkedListBox1.TabIndex = 292;
            // 
            // Button29
            // 
            Button29.DialogResult = DialogResult.Cancel;
            Button29.Location = new Point(139, 711);
            Button29.Margin = new Padding(4, 3, 4, 3);
            Button29.Name = "Button29";
            Button29.Size = new Size(88, 27);
            Button29.TabIndex = 290;
            Button29.Text = "Cancel";
            Button29.UseVisualStyleBackColor = true;
            // 
            // Button28
            // 
            Button28.DialogResult = DialogResult.OK;
            Button28.Location = new Point(45, 711);
            Button28.Margin = new Padding(4, 3, 4, 3);
            Button28.Name = "Button28";
            Button28.Size = new Size(88, 27);
            Button28.TabIndex = 291;
            Button28.Text = "OK";
            Button28.UseVisualStyleBackColor = true;
            Button28.Click += Button28_Click;
            // 
            // button10
            // 
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(118, 45);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 288;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(23, 45);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 287;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(23, 12);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 284;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // Timer3
            // 
            Timer3.Interval = 1;
            Timer3.Tick += Timer3_Tick;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // frmExcludeFromPic
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 750);
            Controls.Add(splitContainer1);
            Name = "frmExcludeFromPic";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmExcludeFromPic";
            FormClosing += frmExcludeFromPic_FormClosing;
            Load += frmExcludeFromPic_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numPenWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numExceptBounds2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numExceptBounds).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ToolTip toolTip1;
        private SaveFileDialog saveFileDialog1;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        internal System.Windows.Forms.Timer Timer3;
        internal Label Label20;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        private Button btnUndo;
        private Button btnRedo;
        internal Button Button29;
        internal Button Button28;
        private Button button10;
        private Button button8;
        internal Button button2;
        private CheckBox cbDraw;
        private Button btnAdd;
        private Label label2;
        private NumericUpDown numExceptBounds;
        private Label label1;
        private Button btnAdd2;
        internal Label Label3;
        internal NumericUpDown numPenWidth;
        internal Button btnRemPoint;
        internal Button btnRemSeg;
        internal Button btnNewPath;
        internal Button btnClosePath;
        private Button btnShow2;
        private Button btnShow;
        private Label label5;
        private NumericUpDown numExceptBounds2;
        private Label label4;
        private CheckedListBox checkedListBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
    }
}