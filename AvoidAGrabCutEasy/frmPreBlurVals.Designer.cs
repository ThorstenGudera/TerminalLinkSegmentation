namespace AvoidAGrabCutEasy
{
    partial class frmPreBlurVals
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
            if (this.pictureBox2.Image != null)
                this.pictureBox2.Image.Dispose();
            if (this.pictureBox3.Image != null)
                this.pictureBox3.Image.Dispose();
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
            splitContainer1 = new SplitContainer();
            numOpacity = new NumericUpDown();
            label1 = new Label();
            groupBox3 = new GroupBox();
            rbAfter = new RadioButton();
            rbBefore = new RadioButton();
            label8 = new Label();
            label7 = new Label();
            cbColors = new CheckBox();
            numValDst = new NumericUpDown();
            numValSrc = new NumericUpDown();
            cbBlur = new CheckBox();
            label16 = new Label();
            numKernel = new NumericUpDown();
            numDistWeight = new NumericUpDown();
            progressBar1 = new ProgressBar();
            btnPreview = new Button();
            pictureBox1 = new PictureBox();
            splitContainer2 = new SplitContainer();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numOpacity).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numValDst).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numValSrc).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numKernel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDistWeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(988, 166);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 734;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(891, 166);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 735;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(numOpacity);
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(groupBox3);
            splitContainer1.Panel1.Controls.Add(progressBar1);
            splitContainer1.Panel1.Controls.Add(btnPreview);
            splitContainer1.Panel1.Controls.Add(pictureBox1);
            splitContainer1.Panel1.Controls.Add(btnCancel);
            splitContainer1.Panel1.Controls.Add(btnOK);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1089, 713);
            splitContainer1.SplitterDistance = 202;
            splitContainer1.TabIndex = 748;
            // 
            // numOpacity
            // 
            numOpacity.DecimalPlaces = 2;
            numOpacity.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numOpacity.Location = new Point(401, 120);
            numOpacity.Margin = new Padding(4, 3, 4, 3);
            numOpacity.Maximum = new decimal(new int[] { 20, 0, 0, 65536 });
            numOpacity.Name = "numOpacity";
            numOpacity.Size = new Size(52, 23);
            numOpacity.TabIndex = 756;
            numOpacity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(338, 122);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 757;
            label1.Text = "Opacity";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(rbAfter);
            groupBox3.Controls.Add(rbBefore);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(cbColors);
            groupBox3.Controls.Add(numValDst);
            groupBox3.Controls.Add(numValSrc);
            groupBox3.Controls.Add(cbBlur);
            groupBox3.Controls.Add(label16);
            groupBox3.Controls.Add(numKernel);
            groupBox3.Controls.Add(numDistWeight);
            groupBox3.Location = new Point(12, 12);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(306, 125);
            groupBox3.TabIndex = 755;
            groupBox3.TabStop = false;
            // 
            // rbAfter
            // 
            rbAfter.AutoSize = true;
            rbAfter.Location = new Point(156, 47);
            rbAfter.Name = "rbAfter";
            rbAfter.Size = new Size(111, 19);
            rbAfter.TabIndex = 761;
            rbAfter.Text = "after proc colors";
            rbAfter.UseVisualStyleBackColor = true;
            // 
            // rbBefore
            // 
            rbBefore.AutoSize = true;
            rbBefore.Checked = true;
            rbBefore.Location = new Point(29, 47);
            rbBefore.Name = "rbBefore";
            rbBefore.Size = new Size(121, 19);
            rbBefore.TabIndex = 761;
            rbBefore.TabStop = true;
            rbBefore.Text = "before proc colors";
            rbBefore.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(220, 86);
            label8.Name = "label8";
            label8.Size = new Size(16, 15);
            label8.TabIndex = 760;
            label8.Text = "y:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(128, 86);
            label7.Name = "label7";
            label7.Size = new Size(16, 15);
            label7.TabIndex = 760;
            label7.Text = "x:";
            // 
            // cbColors
            // 
            cbColors.AutoSize = true;
            cbColors.Location = new Point(8, 85);
            cbColors.Name = "cbColors";
            cbColors.Size = new Size(100, 19);
            cbColors.TabIndex = 759;
            cbColors.Text = "ReMap Colors";
            cbColors.UseVisualStyleBackColor = true;
            cbColors.CheckedChanged += cbVarLog_CheckedChanged;
            // 
            // numValDst
            // 
            numValDst.Location = new Point(243, 83);
            numValDst.Margin = new Padding(4, 3, 4, 3);
            numValDst.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numValDst.Name = "numValDst";
            numValDst.Size = new Size(52, 23);
            numValDst.TabIndex = 757;
            numValDst.Value = new decimal(new int[] { 148, 0, 0, 0 });
            numValDst.ValueChanged += numIGGKernel_ValueChanged;
            // 
            // numValSrc
            // 
            numValSrc.Location = new Point(151, 83);
            numValSrc.Margin = new Padding(4, 3, 4, 3);
            numValSrc.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numValSrc.Name = "numValSrc";
            numValSrc.Size = new Size(52, 23);
            numValSrc.TabIndex = 758;
            numValSrc.Value = new decimal(new int[] { 62, 0, 0, 0 });
            numValSrc.ValueChanged += numIGGKernel_ValueChanged;
            // 
            // cbBlur
            // 
            cbBlur.AutoSize = true;
            cbBlur.Checked = true;
            cbBlur.CheckState = CheckState.Checked;
            cbBlur.Location = new Point(8, 22);
            cbBlur.Name = "cbBlur";
            cbBlur.Size = new Size(100, 19);
            cbBlur.TabIndex = 751;
            cbBlur.Text = "preBlur Kernel";
            cbBlur.UseVisualStyleBackColor = true;
            cbBlur.CheckedChanged += cbVarLog_CheckedChanged;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(172, 23);
            label16.Name = "label16";
            label16.Size = new Size(64, 15);
            label16.TabIndex = 754;
            label16.Text = "distWeight";
            toolTip1.SetToolTip(label16, "max color dist to keep blurring.\r\n\"Edge weight\" - bigger blurs more edges\r\nvalue range from 0 to 442");
            // 
            // numKernel
            // 
            numKernel.Location = new Point(113, 21);
            numKernel.Margin = new Padding(4, 3, 4, 3);
            numKernel.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numKernel.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numKernel.Name = "numKernel";
            numKernel.Size = new Size(52, 23);
            numKernel.TabIndex = 753;
            numKernel.Value = new decimal(new int[] { 127, 0, 0, 0 });
            numKernel.ValueChanged += numIGGKernel_ValueChanged;
            // 
            // numDistWeight
            // 
            numDistWeight.Location = new Point(243, 21);
            numDistWeight.Margin = new Padding(4, 3, 4, 3);
            numDistWeight.Maximum = new decimal(new int[] { 443, 0, 0, 0 });
            numDistWeight.Name = "numDistWeight";
            numDistWeight.Size = new Size(52, 23);
            numDistWeight.TabIndex = 752;
            toolTip1.SetToolTip(numDistWeight, "max color dist to keep blurring.\r\n\"Edge weight\" - bigger blurs more edges\r\nvalue range from 0 to 443");
            numDistWeight.Value = new decimal(new int[] { 101, 0, 0, 0 });
            numDistWeight.ValueChanged += numIGGKernel_ValueChanged;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 153);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(342, 23);
            progressBar1.TabIndex = 750;
            // 
            // btnPreview
            // 
            btnPreview.Location = new Point(378, 153);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(75, 23);
            btnPreview.TabIndex = 749;
            btnPreview.Text = "Preview";
            btnPreview.UseVisualStyleBackColor = true;
            btnPreview.Click += btnPreview_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(470, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(299, 181);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 748;
            pictureBox1.TabStop = false;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.AutoScroll = true;
            splitContainer2.Panel1.Controls.Add(pictureBox2);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.AutoScroll = true;
            splitContainer2.Panel2.Controls.Add(pictureBox3);
            splitContainer2.Size = new Size(1089, 507);
            splitContainer2.SplitterDistance = 543;
            splitContainer2.TabIndex = 0;
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(0, 0);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(543, 507);
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Location = new Point(0, 0);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(542, 507);
            pictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox3.TabIndex = 0;
            pictureBox3.TabStop = false;
            pictureBox3.Click += pictureBox3_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // frmPreBlurVals
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1089, 713);
            Controls.Add(splitContainer1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmPreBlurVals";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmPreBlurVals";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numOpacity).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numValDst).EndInit();
            ((System.ComponentModel.ISupportInitialize)numValSrc).EndInit();
            ((System.ComponentModel.ISupportInitialize)numKernel).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDistWeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel1.PerformLayout();
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button btnCancel;
        private Button btnOK;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Button btnPreview;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private ProgressBar progressBar1;
        internal PictureBox pictureBox1;
        private Label label16;
        internal NumericUpDown numDistWeight;
        internal NumericUpDown numKernel;
        private GroupBox groupBox3;
        private Label label8;
        private Label label7;
        internal NumericUpDown numValDst;
        internal NumericUpDown numValSrc;
        internal CheckBox cbBlur;
        internal CheckBox cbColors;
        internal RadioButton rbAfter;
        internal RadioButton rbBefore;
        internal NumericUpDown numOpacity;
        private Label label1;
        private ToolTip toolTip1;
    }
}