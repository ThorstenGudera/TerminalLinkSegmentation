namespace QuickExtract2
{
    partial class frmQuickExtract
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
            Button29 = new Button();
            Button28 = new Button();
            Panel6 = new Panel();
            Panel4 = new Panel();
            Timer3 = new System.Windows.Forms.Timer(components);
            timer1 = new System.Windows.Forms.Timer(components);
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            ToolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            openFileDialog1 = new OpenFileDialog();
            ColorDialog1 = new ColorDialog();
            SaveFileDialog2 = new SaveFileDialog();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            Timer2 = new System.Windows.Forms.Timer(components);
            BackgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            BackgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            panel3 = new Panel();
            toolTip1 = new ToolTip(components);
            saveFileDialog1 = new SaveFileDialog();
            Panel1 = new Panel();
            numWH = new NumericUpDown();
            cbOutline = new CheckBox();
            cbLoadTo = new CheckBox();
            CheckBox18 = new CheckBox();
            Label20 = new Label();
            ComboBox2 = new ComboBox();
            quickExtractingCtrl1 = new QuickExtractingCtrl();
            Panel11 = new Panel();
            Button26 = new Button();
            Button11 = new Button();
            Label40 = new Label();
            Label1 = new Label();
            CheckBox12 = new CheckBox();
            button10 = new Button();
            button8 = new Button();
            button4 = new Button();
            button5 = new Button();
            button2 = new Button();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            timer4 = new System.Windows.Forms.Timer(components);
            statusStrip1.SuspendLayout();
            Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numWH).BeginInit();
            SuspendLayout();
            // 
            // Button29
            // 
            Button29.DialogResult = DialogResult.Cancel;
            Button29.Location = new Point(1416, 75);
            Button29.Margin = new Padding(4, 3, 4, 3);
            Button29.Name = "Button29";
            Button29.Size = new Size(88, 27);
            Button29.TabIndex = 280;
            Button29.Text = "Cancel";
            Button29.UseVisualStyleBackColor = true;
            // 
            // Button28
            // 
            Button28.DialogResult = DialogResult.OK;
            Button28.Location = new Point(1322, 75);
            Button28.Margin = new Padding(4, 3, 4, 3);
            Button28.Name = "Button28";
            Button28.Size = new Size(88, 27);
            Button28.TabIndex = 280;
            Button28.Text = "OK";
            Button28.UseVisualStyleBackColor = true;
            Button28.Click += Button28_Click;
            // 
            // Panel6
            // 
            Panel6.BackColor = SystemColors.ActiveCaptionText;
            Panel6.Location = new Point(1245, 105);
            Panel6.Margin = new Padding(4, 3, 4, 3);
            Panel6.Name = "Panel6";
            Panel6.Size = new Size(257, 2);
            Panel6.TabIndex = 259;
            // 
            // Panel4
            // 
            Panel4.BackColor = SystemColors.ActiveCaptionText;
            Panel4.Location = new Point(1174, 92);
            Panel4.Margin = new Padding(4, 3, 4, 3);
            Panel4.Name = "Panel4";
            Panel4.Size = new Size(2, 58);
            Panel4.TabIndex = 255;
            // 
            // Timer3
            // 
            Timer3.Interval = 500;
            // 
            // timer1
            // 
            timer1.Interval = 50;
            timer1.Tick += timer1_Tick;
            // 
            // statusStrip1
            // 
            statusStrip1.Font = new Font("Segoe UI", 14F);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, ToolStripStatusLabel2, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 831);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1516, 34);
            statusStrip1.TabIndex = 217;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(56, 29);
            toolStripStatusLabel1.Text = "Hallo";
            // 
            // ToolStripStatusLabel2
            // 
            ToolStripStatusLabel2.AutoSize = false;
            ToolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            ToolStripStatusLabel2.Name = "ToolStripStatusLabel2";
            ToolStripStatusLabel2.Size = new Size(190, 29);
            ToolStripStatusLabel2.Text = " ";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(467, 28);
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png";
            // 
            // ColorDialog1
            // 
            ColorDialog1.AnyColor = true;
            ColorDialog1.FullOpen = true;
            // 
            // SaveFileDialog2
            // 
            SaveFileDialog2.DefaultExt = "tggrp";
            SaveFileDialog2.FileName = "File1.tggrp";
            SaveFileDialog2.Filter = "GraphicsPath files (*.tggrp)|*.tggrp";
            SaveFileDialog2.RestoreDirectory = true;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // Timer2
            // 
            Timer2.Interval = 1000;
            Timer2.Tick += Timer2_Tick;
            // 
            // BackgroundWorker2
            // 
            BackgroundWorker2.WorkerReportsProgress = true;
            BackgroundWorker2.WorkerSupportsCancellation = true;
            // 
            // BackgroundWorker3
            // 
            BackgroundWorker3.WorkerReportsProgress = true;
            BackgroundWorker3.WorkerSupportsCancellation = true;
            // 
            // panel3
            // 
            panel3.Location = new Point(0, 0);
            panel3.Margin = new Padding(4, 3, 4, 3);
            panel3.Name = "panel3";
            panel3.Size = new Size(233, 28);
            panel3.TabIndex = 219;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // Panel1
            // 
            Panel1.Controls.Add(numWH);
            Panel1.Controls.Add(cbOutline);
            Panel1.Controls.Add(cbLoadTo);
            Panel1.Controls.Add(CheckBox18);
            Panel1.Controls.Add(Label20);
            Panel1.Controls.Add(ComboBox2);
            Panel1.Controls.Add(quickExtractingCtrl1);
            Panel1.Controls.Add(Button29);
            Panel1.Controls.Add(Button28);
            Panel1.Controls.Add(Panel6);
            Panel1.Controls.Add(Panel11);
            Panel1.Controls.Add(Panel4);
            Panel1.Controls.Add(Button26);
            Panel1.Controls.Add(Button11);
            Panel1.Controls.Add(Label40);
            Panel1.Controls.Add(Label1);
            Panel1.Controls.Add(CheckBox12);
            Panel1.Controls.Add(button10);
            Panel1.Controls.Add(button8);
            Panel1.Controls.Add(button4);
            Panel1.Controls.Add(button5);
            Panel1.Controls.Add(button2);
            Panel1.Dock = DockStyle.Top;
            Panel1.Location = new Point(0, 0);
            Panel1.Margin = new Padding(4, 3, 4, 3);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(1516, 240);
            Panel1.TabIndex = 218;
            // 
            // numWH
            // 
            numWH.Location = new Point(1461, 142);
            numWH.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numWH.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numWH.Name = "numWH";
            numWH.Size = new Size(50, 23);
            numWH.TabIndex = 286;
            numWH.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // cbOutline
            // 
            cbOutline.AutoSize = true;
            cbOutline.Location = new Point(1201, 144);
            cbOutline.Name = "cbOutline";
            cbOutline.Size = new Size(258, 19);
            cbOutline.TabIndex = 285;
            cbOutline.Text = "compute outline to helplineRulerCtrl1 width";
            cbOutline.UseVisualStyleBackColor = true;
            // 
            // cbLoadTo
            // 
            cbLoadTo.AutoSize = true;
            cbLoadTo.Checked = true;
            cbLoadTo.CheckState = CheckState.Checked;
            cbLoadTo.Location = new Point(1189, 119);
            cbLoadTo.Name = "cbLoadTo";
            cbLoadTo.Size = new Size(192, 19);
            cbLoadTo.TabIndex = 285;
            cbLoadTo.Text = "load result to helplineRulerCtrl2";
            cbLoadTo.UseVisualStyleBackColor = true;
            // 
            // CheckBox18
            // 
            CheckBox18.AutoSize = true;
            CheckBox18.Checked = true;
            CheckBox18.CheckState = CheckState.Checked;
            CheckBox18.Location = new Point(1415, 178);
            CheckBox18.Margin = new Padding(4, 3, 4, 3);
            CheckBox18.Name = "CheckBox18";
            CheckBox18.Size = new Size(79, 19);
            CheckBox18.TabIndex = 284;
            CheckBox18.Text = "draw Path";
            CheckBox18.UseVisualStyleBackColor = true;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(1238, 179);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 283;
            Label20.Text = "Set Zoom";
            // 
            // ComboBox2
            // 
            ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            ComboBox2.FormattingEnabled = true;
            ComboBox2.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            ComboBox2.Location = new Point(1307, 175);
            ComboBox2.Margin = new Padding(4, 3, 4, 3);
            ComboBox2.Name = "ComboBox2";
            ComboBox2.Size = new Size(87, 23);
            ComboBox2.TabIndex = 282;
            ComboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            // 
            // quickExtractingCtrl1
            // 
            quickExtractingCtrl1.Location = new Point(4, 3);
            quickExtractingCtrl1.Margin = new Padding(5, 3, 5, 3);
            quickExtractingCtrl1.Name = "quickExtractingCtrl1";
            quickExtractingCtrl1.Size = new Size(1166, 238);
            quickExtractingCtrl1.TabIndex = 281;
            // 
            // Panel11
            // 
            Panel11.BackColor = SystemColors.ActiveCaptionText;
            Panel11.Location = new Point(1228, 44);
            Panel11.Margin = new Padding(4, 3, 4, 3);
            Panel11.Name = "Panel11";
            Panel11.Size = new Size(2, 35);
            Panel11.TabIndex = 255;
            // 
            // Button26
            // 
            Button26.Enabled = false;
            Button26.Location = new Point(1455, 204);
            Button26.Margin = new Padding(4, 3, 4, 3);
            Button26.Name = "Button26";
            Button26.Size = new Size(49, 27);
            Button26.TabIndex = 239;
            Button26.Text = "Go";
            Button26.UseVisualStyleBackColor = true;
            Button26.Click += Button26_Click;
            // 
            // Button11
            // 
            Button11.Location = new Point(1287, 204);
            Button11.Margin = new Padding(4, 3, 4, 3);
            Button11.Name = "Button11";
            Button11.Size = new Size(49, 27);
            Button11.TabIndex = 239;
            Button11.Text = "Go";
            Button11.UseVisualStyleBackColor = true;
            Button11.Click += Button11_Click;
            // 
            // Label40
            // 
            Label40.AutoSize = true;
            Label40.Enabled = false;
            Label40.Location = new Point(1360, 210);
            Label40.Margin = new Padding(4, 0, 4, 0);
            Label40.Name = "Label40";
            Label40.Size = new Size(85, 15);
            Label40.TabIndex = 238;
            Label40.Text = "remove border";
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(1176, 210);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(99, 15);
            Label1.TabIndex = 238;
            Label1.Text = "add 100px border";
            // 
            // CheckBox12
            // 
            CheckBox12.AutoSize = true;
            CheckBox12.Checked = true;
            CheckBox12.CheckState = CheckState.Checked;
            CheckBox12.Location = new Point(1435, 50);
            CheckBox12.Margin = new Padding(4, 3, 4, 3);
            CheckBox12.Name = "CheckBox12";
            CheckBox12.Size = new Size(67, 19);
            CheckBox12.TabIndex = 217;
            CheckBox12.Text = "BG dark";
            CheckBox12.UseVisualStyleBackColor = true;
            CheckBox12.CheckedChanged += CheckBox12_CheckedChanged;
            // 
            // button10
            // 
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(1336, 45);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 216;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(1241, 45);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 215;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button4
            // 
            button4.Enabled = false;
            button4.ForeColor = SystemColors.ControlText;
            button4.Location = new Point(1227, 12);
            button4.Margin = new Padding(4, 3, 4, 3);
            button4.Name = "button4";
            button4.Size = new Size(88, 27);
            button4.TabIndex = 213;
            button4.Text = "Undo";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Enabled = false;
            button5.ForeColor = SystemColors.ControlText;
            button5.Location = new Point(1322, 12);
            button5.Margin = new Padding(4, 3, 4, 3);
            button5.Name = "button5";
            button5.Size = new Size(88, 27);
            button5.TabIndex = 214;
            button5.Text = "Redo";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(1416, 12);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 212;
            button2.Text = "Save";
            button2.Click += button2_Click;
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
            helplineRulerCtrl1.Location = new Point(0, 240);
            helplineRulerCtrl1.Margin = new Padding(5, 3, 5, 3);
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(1516, 591);
            helplineRulerCtrl1.TabIndex = 220;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // timer4
            // 
            timer4.Interval = 50;
            timer4.Tick += timer4_Tick;
            // 
            // frmQuickExtract
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1516, 865);
            Controls.Add(helplineRulerCtrl1);
            Controls.Add(Panel1);
            Controls.Add(panel3);
            Controls.Add(statusStrip1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "frmQuickExtract";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmQuickExtract";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            Panel1.ResumeLayout(false);
            Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numWH).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        internal System.Windows.Forms.Button Button29;
        internal System.Windows.Forms.Button Button28;
        internal System.Windows.Forms.Panel Panel6;
        internal System.Windows.Forms.Panel Panel4;
        internal System.Windows.Forms.Timer Timer3;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.SaveFileDialog SaveFileDialog2;
        internal System.ComponentModel.BackgroundWorker backgroundWorker1;
        internal System.Windows.Forms.Timer Timer2;
        internal System.ComponentModel.BackgroundWorker BackgroundWorker2;
        internal System.ComponentModel.BackgroundWorker BackgroundWorker3;
        private System.Windows.Forms.Panel panel3;
        internal HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.Button Button26;
        internal System.Windows.Forms.Button Button11;
        internal System.Windows.Forms.Label Label40;
        internal System.Windows.Forms.CheckBox CheckBox12;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        internal System.Windows.Forms.Button button2;
        internal System.Windows.Forms.Panel Panel11;
        internal System.Windows.Forms.Label Label1;
        private QuickExtractingCtrl quickExtractingCtrl1;
        internal System.Windows.Forms.CheckBox CheckBox18;
        internal System.Windows.Forms.Label Label20;
        internal System.Windows.Forms.ComboBox ComboBox2;
        public CheckBox cbLoadTo;
        public NumericUpDown numWH;
        public CheckBox cbOutline;
        private System.Windows.Forms.Timer timer4;
    }
}