namespace PseudoShadow
{
    partial class frmGaussianBlur
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
            pictureBox1 = new PictureBox();
            btnCancel = new Button();
            btnOK = new Button();
            panel1 = new Panel();
            rbLarge = new RadioButton();
            rbSmall = new RadioButton();
            tbVal = new TextBox();
            tbLast = new TextBox();
            label2 = new Label();
            trackBar1 = new TrackBar();
            cbZoom = new CheckBox();
            label1 = new Label();
            timer1 = new System.Windows.Forms.Timer(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(pictureBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnCancel);
            splitContainer1.Panel2.Controls.Add(btnOK);
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Panel2.Controls.Add(tbVal);
            splitContainer1.Panel2.Controls.Add(tbLast);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(trackBar1);
            splitContainer1.Panel2.Controls.Add(cbZoom);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Size = new Size(512, 661);
            splitContainer1.SplitterDistance = 512;
            splitContainer1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(512, 512);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.DoubleClick += pictureBox1_DoubleClick;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(411, 106);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 658;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(318, 106);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 659;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(rbLarge);
            panel1.Controls.Add(rbSmall);
            panel1.Location = new Point(158, 66);
            panel1.Name = "panel1";
            panel1.Size = new Size(149, 44);
            panel1.TabIndex = 5;
            // 
            // rbLarge
            // 
            rbLarge.AutoSize = true;
            rbLarge.Location = new Point(74, 11);
            rbLarge.Name = "rbLarge";
            rbLarge.Size = new Size(66, 19);
            rbLarge.TabIndex = 0;
            rbLarge.Text = "127-255";
            rbLarge.UseVisualStyleBackColor = true;
            // 
            // rbSmall
            // 
            rbSmall.AutoSize = true;
            rbSmall.Checked = true;
            rbSmall.Location = new Point(14, 12);
            rbSmall.Name = "rbSmall";
            rbSmall.Size = new Size(54, 19);
            rbSmall.TabIndex = 0;
            rbSmall.TabStop = true;
            rbSmall.Text = "0-127";
            rbSmall.UseVisualStyleBackColor = true;
            rbSmall.CheckedChanged += rbSmall_CheckedChanged;
            // 
            // tbVal
            // 
            tbVal.BackColor = SystemColors.ControlDark;
            tbVal.ForeColor = Color.White;
            tbVal.Location = new Point(450, 32);
            tbVal.Name = "tbVal";
            tbVal.ReadOnly = true;
            tbVal.Size = new Size(50, 23);
            tbVal.TabIndex = 4;
            // 
            // tbLast
            // 
            tbLast.Location = new Point(90, 77);
            tbLast.Name = "tbLast";
            tbLast.ReadOnly = true;
            tbLast.Size = new Size(50, 23);
            tbLast.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 80);
            label2.Name = "label2";
            label2.Size = new Size(72, 15);
            label2.TabIndex = 3;
            label2.Text = "Last Preview";
            // 
            // trackBar1
            // 
            trackBar1.BackColor = SystemColors.ControlDark;
            trackBar1.Location = new Point(12, 32);
            trackBar1.Maximum = 127;
            trackBar1.Minimum = 3;
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(432, 45);
            trackBar1.SmallChange = 2;
            trackBar1.TabIndex = 2;
            trackBar1.Value = 3;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // cbZoom
            // 
            cbZoom.AutoSize = true;
            cbZoom.Checked = true;
            cbZoom.CheckState = CheckState.Checked;
            cbZoom.Location = new Point(264, 9);
            cbZoom.Name = "cbZoom";
            cbZoom.Size = new Size(56, 19);
            cbZoom.TabIndex = 1;
            cbZoom.Text = "zoom";
            cbZoom.UseVisualStyleBackColor = true;
            cbZoom.CheckedChanged += cbZoom_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(326, 9);
            label1.Name = "label1";
            label1.Size = new Size(174, 15);
            label1.TabIndex = 0;
            label1.Text = "correct view only without zoom";
            // 
            // timer1
            // 
            timer1.Interval = 200;
            timer1.Tick += timer1_Tick;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // frmGaussianBlur
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(512, 661);
            Controls.Add(splitContainer1);
            Name = "frmGaussianBlur";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmGaussianBlur";
            FormClosing += frmGaussianBlur_FormClosing;
            Load += frmGaussianBlur_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private PictureBox pictureBox1;
        private Panel panel1;
        private RadioButton rbSmall;
        private TextBox tbLast;
        private Label label2;
        private CheckBox cbZoom;
        private Label label1;
        private RadioButton rbLarge;
        private TextBox tbVal;
        private Button btnCancel;
        private Button btnOK;
        private System.Windows.Forms.Timer timer1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        internal TrackBar trackBar1;
    }
}