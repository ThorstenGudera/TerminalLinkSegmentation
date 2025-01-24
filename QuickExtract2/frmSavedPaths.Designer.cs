namespace QuickExtract2
{
    partial class frmSavedPaths
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
            OpenFileDialog2 = new OpenFileDialog();
            ColorDialog1 = new ColorDialog();
            SaveFileDialog2 = new SaveFileDialog();
            Button9 = new Button();
            Label4 = new Label();
            CheckBox3 = new CheckBox();
            Panel2 = new Panel();
            Label2 = new Label();
            Button7 = new Button();
            Button6 = new Button();
            CheckBox2 = new CheckBox();
            Button8 = new Button();
            Button5 = new Button();
            Label1 = new Label();
            CheckBox1 = new CheckBox();
            Button4 = new Button();
            Button3 = new Button();
            ListBox1 = new ListBox();
            NumericUpDown1 = new NumericUpDown();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            Label3 = new Label();
            Button2 = new Button();
            Panel1 = new Panel();
            cbRestrict = new CheckBox();
            numRestrict = new NumericUpDown();
            btnReadPath = new Button();
            numChainTolerance = new NumericUpDown();
            label5 = new Label();
            label6 = new Label();
            CheckBox4 = new CheckBox();
            CheckBox12 = new CheckBox();
            Button13 = new Button();
            Button1 = new Button();
            toolTip1 = new ToolTip(components);
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown1).BeginInit();
            Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numRestrict).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numChainTolerance).BeginInit();
            SuspendLayout();
            // 
            // OpenFileDialog2
            // 
            OpenFileDialog2.FileName = "File1";
            OpenFileDialog2.Filter = "GraphicsPath-Files (*.tggrp)|*.tggrp";
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
            // Button9
            // 
            Button9.Location = new Point(114, 9);
            Button9.Margin = new Padding(4, 3, 4, 3);
            Button9.Name = "Button9";
            Button9.Size = new Size(110, 27);
            Button9.TabIndex = 11;
            Button9.Text = "unselect ListBox";
            Button9.UseVisualStyleBackColor = true;
            Button9.Click += Button9_Click;
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Enabled = false;
            Label4.Location = new Point(15, 165);
            Label4.Margin = new Padding(4, 0, 4, 0);
            Label4.Name = "Label4";
            Label4.Size = new Size(110, 15);
            Label4.TabIndex = 10;
            Label4.Text = "Add CurPath to List";
            // 
            // CheckBox3
            // 
            CheckBox3.AutoSize = true;
            CheckBox3.Checked = true;
            CheckBox3.CheckState = CheckState.Checked;
            CheckBox3.Location = new Point(139, 333);
            CheckBox3.Margin = new Padding(4, 3, 4, 3);
            CheckBox3.Name = "CheckBox3";
            CheckBox3.Size = new Size(78, 19);
            CheckBox3.TabIndex = 9;
            CheckBox3.Text = "save copy";
            CheckBox3.UseVisualStyleBackColor = true;
            // 
            // Panel2
            // 
            Panel2.Controls.Add(Button9);
            Panel2.Controls.Add(Label4);
            Panel2.Controls.Add(CheckBox3);
            Panel2.Controls.Add(Label2);
            Panel2.Controls.Add(Button7);
            Panel2.Controls.Add(Button6);
            Panel2.Controls.Add(CheckBox2);
            Panel2.Controls.Add(Button8);
            Panel2.Controls.Add(Button5);
            Panel2.Controls.Add(Label1);
            Panel2.Controls.Add(CheckBox1);
            Panel2.Controls.Add(Button4);
            Panel2.Controls.Add(Button3);
            Panel2.Controls.Add(ListBox1);
            Panel2.Dock = DockStyle.Right;
            Panel2.Location = new Point(849, 0);
            Panel2.Margin = new Padding(4, 3, 4, 3);
            Panel2.Name = "Panel2";
            Panel2.Size = new Size(233, 576);
            Panel2.TabIndex = 14;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Location = new Point(43, 384);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(162, 60);
            Label2.TabIndex = 8;
            Label2.Text = "You need to set the path you \r\nwant to work with to CurPath\r\nCurPath will be displayed in \r\nthe Parent form. ";
            // 
            // Button7
            // 
            Button7.Location = new Point(43, 487);
            Button7.Margin = new Padding(4, 3, 4, 3);
            Button7.Name = "Button7";
            Button7.Size = new Size(88, 27);
            Button7.TabIndex = 7;
            Button7.Text = "Load Path";
            Button7.UseVisualStyleBackColor = true;
            Button7.Click += Button7_Click;
            // 
            // Button6
            // 
            Button6.Location = new Point(136, 487);
            Button6.Margin = new Padding(4, 3, 4, 3);
            Button6.Name = "Button6";
            Button6.Size = new Size(88, 27);
            Button6.TabIndex = 7;
            Button6.Text = "Save Path";
            Button6.UseVisualStyleBackColor = true;
            Button6.Click += Button6_Click;
            // 
            // CheckBox2
            // 
            CheckBox2.AutoSize = true;
            CheckBox2.Location = new Point(43, 15);
            CheckBox2.Margin = new Padding(4, 3, 4, 3);
            CheckBox2.Name = "CheckBox2";
            CheckBox2.Size = new Size(69, 19);
            CheckBox2.TabIndex = 6;
            CheckBox2.Text = "CurPath";
            CheckBox2.UseVisualStyleBackColor = true;
            CheckBox2.CheckedChanged += CheckBox2_CheckedChanged;
            // 
            // Button8
            // 
            Button8.Enabled = false;
            Button8.Location = new Point(136, 159);
            Button8.Margin = new Padding(4, 3, 4, 3);
            Button8.Name = "Button8";
            Button8.Size = new Size(88, 27);
            Button8.TabIndex = 5;
            Button8.Text = "Add";
            Button8.UseVisualStyleBackColor = true;
            Button8.Click += Button8_Click;
            // 
            // Button5
            // 
            Button5.Location = new Point(43, 328);
            Button5.Margin = new Padding(4, 3, 4, 3);
            Button5.Name = "Button5";
            Button5.Size = new Size(88, 27);
            Button5.TabIndex = 5;
            Button5.Text = "go";
            Button5.UseVisualStyleBackColor = true;
            Button5.Click += Button5_Click;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(72, 358);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(106, 15);
            Label1.TabIndex = 4;
            Label1.Text = "swap with CurPath";
            // 
            // CheckBox1
            // 
            CheckBox1.AutoSize = true;
            CheckBox1.Checked = true;
            CheckBox1.CheckState = CheckState.Checked;
            CheckBox1.Location = new Point(138, 261);
            CheckBox1.Margin = new Padding(4, 3, 4, 3);
            CheckBox1.Name = "CheckBox1";
            CheckBox1.Size = new Size(80, 19);
            CheckBox1.TabIndex = 3;
            CheckBox1.Text = "shift 50 px";
            CheckBox1.UseVisualStyleBackColor = true;
            // 
            // Button4
            // 
            Button4.Location = new Point(43, 256);
            Button4.Margin = new Padding(4, 3, 4, 3);
            Button4.Name = "Button4";
            Button4.Size = new Size(88, 27);
            Button4.TabIndex = 2;
            Button4.Text = "Clone";
            Button4.UseVisualStyleBackColor = true;
            Button4.Click += Button4_Click;
            // 
            // Button3
            // 
            Button3.Location = new Point(43, 211);
            Button3.Margin = new Padding(4, 3, 4, 3);
            Button3.Name = "Button3";
            Button3.Size = new Size(88, 27);
            Button3.TabIndex = 1;
            Button3.Text = "Delete";
            Button3.UseVisualStyleBackColor = true;
            Button3.Click += Button3_Click;
            // 
            // ListBox1
            // 
            ListBox1.FormattingEnabled = true;
            ListBox1.ItemHeight = 15;
            ListBox1.Location = new Point(43, 43);
            ListBox1.Margin = new Padding(4, 3, 4, 3);
            ListBox1.Name = "ListBox1";
            ListBox1.Size = new Size(139, 109);
            ListBox1.TabIndex = 0;
            ListBox1.SelectedIndexChanged += ListBox1_SelectedIndexChanged;
            // 
            // NumericUpDown1
            // 
            NumericUpDown1.DecimalPlaces = 2;
            NumericUpDown1.Location = new Point(544, 20);
            NumericUpDown1.Margin = new Padding(4, 3, 4, 3);
            NumericUpDown1.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            NumericUpDown1.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            NumericUpDown1.Name = "NumericUpDown1";
            NumericUpDown1.Size = new Size(58, 23);
            NumericUpDown1.TabIndex = 246;
            NumericUpDown1.Value = new decimal(new int[] { 2, 0, 0, 0 });
            NumericUpDown1.ValueChanged += NumericUpDown1_ValueChanged;
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
            helplineRulerCtrl1.Size = new Size(1082, 576);
            helplineRulerCtrl1.TabIndex = 15;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new Point(474, 23);
            Label3.Margin = new Padding(4, 0, 4, 0);
            Label3.Name = "Label3";
            Label3.Size = new Size(63, 15);
            Label3.TabIndex = 247;
            Label3.Text = "Pen width:";
            // 
            // Button2
            // 
            Button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Button2.DialogResult = DialogResult.Cancel;
            Button2.Location = new Point(974, 43);
            Button2.Margin = new Padding(4, 3, 4, 3);
            Button2.Name = "Button2";
            Button2.Size = new Size(88, 27);
            Button2.TabIndex = 9;
            Button2.Text = "Cancel";
            Button2.UseVisualStyleBackColor = true;
            // 
            // Panel1
            // 
            Panel1.Controls.Add(cbRestrict);
            Panel1.Controls.Add(numRestrict);
            Panel1.Controls.Add(btnReadPath);
            Panel1.Controls.Add(numChainTolerance);
            Panel1.Controls.Add(label5);
            Panel1.Controls.Add(label6);
            Panel1.Controls.Add(CheckBox4);
            Panel1.Controls.Add(CheckBox12);
            Panel1.Controls.Add(Button13);
            Panel1.Controls.Add(Label3);
            Panel1.Controls.Add(NumericUpDown1);
            Panel1.Controls.Add(Button1);
            Panel1.Controls.Add(Button2);
            Panel1.Dock = DockStyle.Bottom;
            Panel1.Location = new Point(0, 576);
            Panel1.Margin = new Padding(4, 3, 4, 3);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(1082, 84);
            Panel1.TabIndex = 13;
            // 
            // cbRestrict
            // 
            cbRestrict.AutoSize = true;
            cbRestrict.Checked = true;
            cbRestrict.CheckState = CheckState.Checked;
            cbRestrict.Location = new Point(42, 53);
            cbRestrict.Name = "cbRestrict";
            cbRestrict.Size = new Size(158, 19);
            cbRestrict.TabIndex = 655;
            cbRestrict.Text = "restrict amount chains to";
            cbRestrict.UseVisualStyleBackColor = true;
            // 
            // numRestrict
            // 
            numRestrict.Location = new Point(207, 52);
            numRestrict.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numRestrict.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numRestrict.Name = "numRestrict";
            numRestrict.Size = new Size(52, 23);
            numRestrict.TabIndex = 656;
            numRestrict.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // btnReadPath
            // 
            btnReadPath.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnReadPath.ForeColor = SystemColors.ControlText;
            btnReadPath.Location = new Point(207, 17);
            btnReadPath.Margin = new Padding(4, 3, 4, 3);
            btnReadPath.Name = "btnReadPath";
            btnReadPath.Size = new Size(88, 27);
            btnReadPath.TabIndex = 654;
            btnReadPath.Text = "Go";
            btnReadPath.UseVisualStyleBackColor = true;
            btnReadPath.Click += btnReadPath_Click;
            // 
            // numChainTolerance
            // 
            numChainTolerance.Location = new Point(148, 20);
            numChainTolerance.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numChainTolerance.Name = "numChainTolerance";
            numChainTolerance.Size = new Size(52, 23);
            numChainTolerance.TabIndex = 653;
            numChainTolerance.Value = new decimal(new int[] { 254, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(85, 22);
            label5.Name = "label5";
            label5.Size = new Size(57, 15);
            label5.TabIndex = 652;
            label5.Text = "Tolerance";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(22, 22);
            label6.Name = "label6";
            label6.Size = new Size(57, 15);
            label6.TabIndex = 651;
            label6.Text = "ReadPath";
            toolTip1.SetToolTip(label6, "ReadPathFromPicture");
            // 
            // CheckBox4
            // 
            CheckBox4.AutoSize = true;
            CheckBox4.Location = new Point(709, 23);
            CheckBox4.Margin = new Padding(4, 3, 4, 3);
            CheckBox4.Name = "CheckBox4";
            CheckBox4.Size = new Size(117, 19);
            CheckBox4.TabIndex = 250;
            CheckBox4.Text = "color for Curpath";
            CheckBox4.UseVisualStyleBackColor = true;
            // 
            // CheckBox12
            // 
            CheckBox12.AutoSize = true;
            CheckBox12.Checked = true;
            CheckBox12.CheckState = CheckState.Checked;
            CheckBox12.Location = new Point(372, 21);
            CheckBox12.Margin = new Padding(4, 3, 4, 3);
            CheckBox12.Name = "CheckBox12";
            CheckBox12.Size = new Size(67, 19);
            CheckBox12.TabIndex = 249;
            CheckBox12.Text = "BG dark";
            CheckBox12.UseVisualStyleBackColor = true;
            CheckBox12.CheckedChanged += CheckBox12_CheckedChanged;
            // 
            // Button13
            // 
            Button13.Location = new Point(614, 17);
            Button13.Margin = new Padding(4, 3, 4, 3);
            Button13.Name = "Button13";
            Button13.Size = new Size(88, 27);
            Button13.TabIndex = 248;
            Button13.Text = "Pen color";
            Button13.UseVisualStyleBackColor = true;
            Button13.Click += Button13_Click;
            // 
            // Button1
            // 
            Button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Button1.DialogResult = DialogResult.OK;
            Button1.Location = new Point(880, 43);
            Button1.Margin = new Padding(4, 3, 4, 3);
            Button1.Name = "Button1";
            Button1.Size = new Size(88, 27);
            Button1.TabIndex = 8;
            Button1.Text = "OK";
            Button1.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // frmSavedPaths
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1082, 660);
            Controls.Add(Panel2);
            Controls.Add(helplineRulerCtrl1);
            Controls.Add(Panel1);
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "frmSavedPaths";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmSavedPaths";
            Load += Form8_Load;
            Panel2.ResumeLayout(false);
            Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)NumericUpDown1).EndInit();
            Panel1.ResumeLayout(false);
            Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numRestrict).EndInit();
            ((System.ComponentModel.ISupportInitialize)numChainTolerance).EndInit();
            ResumeLayout(false);
        }

        #endregion

        internal System.Windows.Forms.OpenFileDialog OpenFileDialog2;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.SaveFileDialog SaveFileDialog2;
        internal System.Windows.Forms.Button Button9;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.CheckBox CheckBox3;
        internal System.Windows.Forms.Panel Panel2;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Button Button7;
        internal System.Windows.Forms.Button Button6;
        internal System.Windows.Forms.CheckBox CheckBox2;
        internal System.Windows.Forms.Button Button8;
        internal System.Windows.Forms.Button Button5;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.CheckBox CheckBox1;
        internal System.Windows.Forms.Button Button4;
        internal System.Windows.Forms.Button Button3;
        internal System.Windows.Forms.ListBox ListBox1;
        internal System.Windows.Forms.NumericUpDown NumericUpDown1;
        internal HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.CheckBox CheckBox4;
        internal System.Windows.Forms.CheckBox CheckBox12;
        internal System.Windows.Forms.Button Button13;
        internal System.Windows.Forms.Button Button1;
        private NumericUpDown numChainTolerance;
        private Label label5;
        private Label label6;
        private Button btnReadPath;
        private ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private CheckBox cbRestrict;
        private NumericUpDown numRestrict;
    }
}