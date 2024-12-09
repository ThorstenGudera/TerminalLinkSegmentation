namespace QuickExtract2
{
    partial class frmFloodClear
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
            this.components = new System.ComponentModel.Container();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.CheckBox4 = new System.Windows.Forms.CheckBox();
            this.CheckBox3 = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.numericUpDown7 = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.Button2 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.CheckBox15 = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.CheckBox9 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.ToolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.NumericUpDown8 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.SplitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label9 = new System.Windows.Forms.Label();
            this.BackgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).BeginInit();
            this.StatusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).BeginInit();
            this.SplitContainer1.Panel1.SuspendLayout();
            this.SplitContainer1.Panel2.SuspendLayout();
            this.SplitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // PictureBox1
            // 
            this.PictureBox1.Location = new System.Drawing.Point(0, 0);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(100, 50);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PictureBox1.TabIndex = 0;
            this.PictureBox1.TabStop = false;
            this.PictureBox1.Layout += new System.Windows.Forms.LayoutEventHandler(this.PictureBox1_Layout);
            this.PictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.contentPanel_MouseClick);
            this.PictureBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.PictureBox1_MouseDoubleClick);
            this.PictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bitmapPanel1_MouseDown);
            this.PictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.contentPanel_MouseMove);
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(35, 17);
            this.ToolStripStatusLabel1.Text = "Hello";
            // 
            // CheckBox4
            // 
            this.CheckBox4.AutoSize = true;
            this.CheckBox4.Enabled = false;
            this.CheckBox4.Location = new System.Drawing.Point(693, 37);
            this.CheckBox4.Name = "CheckBox4";
            this.CheckBox4.Size = new System.Drawing.Size(87, 17);
            this.CheckBox4.TabIndex = 246;
            this.CheckBox4.Text = "closed curve";
            this.CheckBox4.UseVisualStyleBackColor = true;
            this.CheckBox4.CheckedChanged += new System.EventHandler(this.CheckBox4_CheckedChanged);
            // 
            // CheckBox3
            // 
            this.CheckBox3.AutoSize = true;
            this.CheckBox3.Enabled = false;
            this.CheckBox3.Location = new System.Drawing.Point(608, 37);
            this.CheckBox3.Name = "CheckBox3";
            this.CheckBox3.Size = new System.Drawing.Size(79, 17);
            this.CheckBox3.TabIndex = 245;
            this.CheckBox3.Text = "use Curves";
            this.CheckBox3.UseVisualStyleBackColor = true;
            this.CheckBox3.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(561, 14);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(41, 13);
            this.Label1.TabIndex = 244;
            this.Label1.Text = "Epsilon";
            // 
            // numericUpDown7
            // 
            this.numericUpDown7.DecimalPlaces = 2;
            this.numericUpDown7.Location = new System.Drawing.Point(397, 37);
            this.numericUpDown7.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown7.Name = "numericUpDown7";
            this.numericUpDown7.Size = new System.Drawing.Size(67, 20);
            this.numericUpDown7.TabIndex = 227;
            this.numericUpDown7.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged_1);
            // 
            // NumericUpDown1
            // 
            this.NumericUpDown1.DecimalPlaces = 2;
            this.NumericUpDown1.Enabled = false;
            this.NumericUpDown1.Location = new System.Drawing.Point(684, 12);
            this.NumericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDown1.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.NumericUpDown1.Name = "NumericUpDown1";
            this.NumericUpDown1.Size = new System.Drawing.Size(69, 20);
            this.NumericUpDown1.TabIndex = 242;
            this.NumericUpDown1.Value = new decimal(new int[] {
            25,
            0,
            0,
            65536});
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Sqrt(2) / 2",
            "1",
            "Sqrt(2)",
            "2",
            "Custom"});
            this.comboBox1.Location = new System.Drawing.Point(608, 11);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(69, 21);
            this.comboBox1.TabIndex = 243;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(587, 62);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(75, 23);
            this.Button2.TabIndex = 241;
            this.Button2.Text = "Reload";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(213, 62);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(13, 13);
            this.label13.TabIndex = 240;
            this.label13.Text = "  ";
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Location = new System.Drawing.Point(12, 9);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(34, 71);
            this.label12.TabIndex = 239;
            this.label12.Text = " ";
            // 
            // CheckBox15
            // 
            this.CheckBox15.AutoSize = true;
            this.CheckBox15.Checked = true;
            this.CheckBox15.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox15.Location = new System.Drawing.Point(770, 16);
            this.CheckBox15.Name = "CheckBox15";
            this.CheckBox15.Size = new System.Drawing.Size(65, 17);
            this.CheckBox15.TabIndex = 238;
            this.CheckBox15.Text = "BG dark";
            this.CheckBox15.UseVisualStyleBackColor = true;
            this.CheckBox15.CheckedChanged += new System.EventHandler(this.CheckBox15_CheckedChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(485, 62);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 237;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // CheckBox9
            // 
            this.CheckBox9.AutoSize = true;
            this.CheckBox9.Checked = true;
            this.CheckBox9.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox9.Location = new System.Drawing.Point(485, 37);
            this.CheckBox9.Name = "CheckBox9";
            this.CheckBox9.Size = new System.Drawing.Size(82, 17);
            this.CheckBox9.TabIndex = 235;
            this.CheckBox9.Text = "comp Alpha";
            this.CheckBox9.UseVisualStyleBackColor = true;
            this.CheckBox9.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(485, 13);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(62, 17);
            this.checkBox1.TabIndex = 236;
            this.checkBox1.Text = "Smooth";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(326, 39);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(70, 13);
            this.label16.TabIndex = 232;
            this.label16.Text = "with Max Dist";
            this.ToolTip1.SetToolTip(this.label16, "Will be used with Tolerance2..\r\nRange 0 to 1000");
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Location = new System.Drawing.Point(326, 67);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(37, 13);
            this.Label17.TabIndex = 230;
            this.Label17.Text = "Edges";
            this.ToolTip1.SetToolTip(this.Label17, "Compares the difference between the *current* pixel and its neighbors.\r\nRange 0 t" +
        "o 255");
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(326, 12);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 231;
            this.label15.Text = "Tolerance2";
            this.ToolTip1.SetToolTip(this.label15, "Tolerance exceeding the \"normal\" Tolerance value.\r\nWill be used in combination wi" +
        "th maxDist.\r\nRange 0 to 255");
            // 
            // ToolStripProgressBar1
            // 
            this.ToolStripProgressBar1.Name = "ToolStripProgressBar1";
            this.ToolStripProgressBar1.Size = new System.Drawing.Size(200, 16);
            this.ToolStripProgressBar1.Visible = false;
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripStatusLabel1,
            this.ToolStripProgressBar1});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 579);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(843, 22);
            this.StatusStrip1.TabIndex = 3;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // NumericUpDown8
            // 
            this.NumericUpDown8.DecimalPlaces = 2;
            this.NumericUpDown8.Location = new System.Drawing.Point(397, 65);
            this.NumericUpDown8.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDown8.Name = "NumericUpDown8";
            this.NumericUpDown8.Size = new System.Drawing.Size(67, 20);
            this.NumericUpDown8.TabIndex = 228;
            this.NumericUpDown8.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDown8.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged_1);
            // 
            // numericUpDown6
            // 
            this.numericUpDown6.DecimalPlaces = 2;
            this.numericUpDown6.Location = new System.Drawing.Point(397, 10);
            this.numericUpDown6.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown6.Name = "numericUpDown6";
            this.numericUpDown6.Size = new System.Drawing.Size(67, 20);
            this.numericUpDown6.TabIndex = 229;
            this.numericUpDown6.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged_1);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Location = new System.Drawing.Point(216, 13);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(92, 17);
            this.checkBox2.TabIndex = 226;
            this.checkBox2.Text = "ColorDistance";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(62, 38);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 223;
            this.label7.Text = "Alpha";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(96, 9);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(28, 23);
            this.button4.TabIndex = 222;
            this.button4.Text = ">";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(59, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 221;
            this.label6.Text = "Color:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(59, 64);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 13);
            this.label11.TabIndex = 219;
            this.label11.Text = "Tolerance";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(62, 64);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(28, 13);
            this.label8.TabIndex = 220;
            this.label8.Text = "Blue";
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Location = new System.Drawing.Point(130, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 23);
            this.label2.TabIndex = 218;
            this.label2.Text = " ";
            // 
            // numericUpDown4
            // 
            this.numericUpDown4.Location = new System.Drawing.Point(132, 36);
            this.numericUpDown4.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown4.Name = "numericUpDown4";
            this.numericUpDown4.Size = new System.Drawing.Size(65, 20);
            this.numericUpDown4.TabIndex = 217;
            this.numericUpDown4.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged);
            // 
            // numericUpDown5
            // 
            this.numericUpDown5.DecimalPlaces = 2;
            this.numericUpDown5.Location = new System.Drawing.Point(130, 62);
            this.numericUpDown5.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDown5.Name = "numericUpDown5";
            this.numericUpDown5.Size = new System.Drawing.Size(67, 20);
            this.numericUpDown5.TabIndex = 216;
            this.numericUpDown5.Value = new decimal(new int[] {
            71,
            0,
            0,
            0});
            this.numericUpDown5.ValueChanged += new System.EventHandler(this.numericUpDown4_ValueChanged_1);
            // 
            // button7
            // 
            this.button7.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button7.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button7.Location = new System.Drawing.Point(760, 62);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(75, 23);
            this.button7.TabIndex = 102;
            this.button7.Text = "Cancel";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button6.Location = new System.Drawing.Point(680, 62);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(75, 23);
            this.button6.TabIndex = 103;
            this.button6.Text = "OK";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // colorDialog1
            // 
            this.colorDialog1.AnyColor = true;
            this.colorDialog1.FullOpen = true;
            // 
            // SplitContainer1
            // 
            this.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.SplitContainer1.Location = new System.Drawing.Point(0, 0);
            this.SplitContainer1.Name = "SplitContainer1";
            this.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // SplitContainer1.Panel1
            // 
            this.SplitContainer1.Panel1.Controls.Add(this.CheckBox4);
            this.SplitContainer1.Panel1.Controls.Add(this.CheckBox3);
            this.SplitContainer1.Panel1.Controls.Add(this.Label1);
            this.SplitContainer1.Panel1.Controls.Add(this.numericUpDown7);
            this.SplitContainer1.Panel1.Controls.Add(this.NumericUpDown1);
            this.SplitContainer1.Panel1.Controls.Add(this.comboBox1);
            this.SplitContainer1.Panel1.Controls.Add(this.Button2);
            this.SplitContainer1.Panel1.Controls.Add(this.label13);
            this.SplitContainer1.Panel1.Controls.Add(this.label12);
            this.SplitContainer1.Panel1.Controls.Add(this.CheckBox15);
            this.SplitContainer1.Panel1.Controls.Add(this.button1);
            this.SplitContainer1.Panel1.Controls.Add(this.CheckBox9);
            this.SplitContainer1.Panel1.Controls.Add(this.checkBox1);
            this.SplitContainer1.Panel1.Controls.Add(this.label16);
            this.SplitContainer1.Panel1.Controls.Add(this.Label17);
            this.SplitContainer1.Panel1.Controls.Add(this.label15);
            this.SplitContainer1.Panel1.Controls.Add(this.NumericUpDown8);
            this.SplitContainer1.Panel1.Controls.Add(this.numericUpDown6);
            this.SplitContainer1.Panel1.Controls.Add(this.checkBox2);
            this.SplitContainer1.Panel1.Controls.Add(this.label9);
            this.SplitContainer1.Panel1.Controls.Add(this.label7);
            this.SplitContainer1.Panel1.Controls.Add(this.button4);
            this.SplitContainer1.Panel1.Controls.Add(this.label6);
            this.SplitContainer1.Panel1.Controls.Add(this.label11);
            this.SplitContainer1.Panel1.Controls.Add(this.label8);
            this.SplitContainer1.Panel1.Controls.Add(this.label2);
            this.SplitContainer1.Panel1.Controls.Add(this.numericUpDown4);
            this.SplitContainer1.Panel1.Controls.Add(this.numericUpDown5);
            this.SplitContainer1.Panel1.Controls.Add(this.button7);
            this.SplitContainer1.Panel1.Controls.Add(this.button6);
            // 
            // SplitContainer1.Panel2
            // 
            this.SplitContainer1.Panel2.AutoScroll = true;
            this.SplitContainer1.Panel2.Controls.Add(this.PictureBox1);
            this.SplitContainer1.Size = new System.Drawing.Size(843, 601);
            this.SplitContainer1.SplitterDistance = 93;
            this.SplitContainer1.TabIndex = 2;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(59, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(34, 13);
            this.label9.TabIndex = 224;
            this.label9.Text = "Alpha";
            // 
            // BackgroundWorker1
            // 
            this.BackgroundWorker1.WorkerReportsProgress = true;
            this.BackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.BackgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.BackgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // frmFloodClear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 601);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.SplitContainer1);
            this.Name = "frmFloodClear";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmFloodClear";
            this.Shown += new System.EventHandler(this.frmFloodClear_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).EndInit();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown5)).EndInit();
            this.SplitContainer1.Panel1.ResumeLayout(false);
            this.SplitContainer1.Panel1.PerformLayout();
            this.SplitContainer1.Panel2.ResumeLayout(false);
            this.SplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitContainer1)).EndInit();
            this.SplitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
        internal System.Windows.Forms.CheckBox CheckBox4;
        internal System.Windows.Forms.CheckBox CheckBox3;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.NumericUpDown numericUpDown7;
        private System.Windows.Forms.NumericUpDown NumericUpDown1;
        private System.Windows.Forms.ComboBox comboBox1;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Label label13;
        internal System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox CheckBox15;
        public System.Windows.Forms.Button button1;
        internal System.Windows.Forms.CheckBox CheckBox9;
        internal System.Windows.Forms.CheckBox checkBox1;
        internal System.Windows.Forms.Label label16;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.Label Label17;
        internal System.Windows.Forms.Label label15;
        internal System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar1;
        internal System.Windows.Forms.StatusStrip StatusStrip1;
        internal System.Windows.Forms.NumericUpDown NumericUpDown8;
        internal System.Windows.Forms.NumericUpDown numericUpDown6;
        internal System.Windows.Forms.CheckBox checkBox2;
        internal System.Windows.Forms.Label label7;
        internal System.Windows.Forms.Button button4;
        internal System.Windows.Forms.Label label6;
        internal System.Windows.Forms.Label label11;
        internal System.Windows.Forms.Label label8;
        internal System.Windows.Forms.Label label2;
        internal System.Windows.Forms.NumericUpDown numericUpDown4;
        internal System.Windows.Forms.NumericUpDown numericUpDown5;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        public System.Windows.Forms.ColorDialog colorDialog1;
        internal System.Windows.Forms.SplitContainer SplitContainer1;
        internal System.Windows.Forms.Label label9;
        internal System.ComponentModel.BackgroundWorker BackgroundWorker1;
    }
}