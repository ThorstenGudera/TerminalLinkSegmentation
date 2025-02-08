namespace AvoidAGrabCutEasy
{
    partial class frmLastScribbles
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
            cmbScribblesType = new ComboBox();
            label1 = new Label();
            listBox1 = new ListBox();
            listBox2 = new ListBox();
            btnDelete = new Button();
            btnOK = new Button();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            btnSetWidth = new Button();
            label2 = new Label();
            numNewWidth = new NumericUpDown();
            rbAll = new RadioButton();
            rbOne = new RadioButton();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            rbTAll = new RadioButton();
            rbTOne = new RadioButton();
            btnSetPos = new Button();
            label4 = new Label();
            label3 = new Label();
            numTY = new NumericUpDown();
            numTX = new NumericUpDown();
            btnSmothenSettings = new Button();
            cbApproxLines = new CheckBox();
            btnApproxLines = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numNewWidth).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTX).BeginInit();
            SuspendLayout();
            // 
            // cmbScribblesType
            // 
            cmbScribblesType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbScribblesType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScribblesType.FormattingEnabled = true;
            cmbScribblesType.Location = new Point(551, 12);
            cmbScribblesType.Name = "cmbScribblesType";
            cmbScribblesType.Size = new Size(121, 23);
            cmbScribblesType.TabIndex = 0;
            cmbScribblesType.SelectedIndexChanged += cnbScribblesType_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(504, 15);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 1;
            label1.Text = "Type";
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(551, 41);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(120, 94);
            listBox1.TabIndex = 2;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // listBox2
            // 
            listBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(698, 41);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(120, 94);
            listBox2.TabIndex = 2;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Location = new Point(732, 141);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(86, 27);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(740, 428);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 650;
            btnOK.Text = "Close";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(12, 15);
            panel1.Name = "panel1";
            panel1.Size = new Size(473, 440);
            panel1.TabIndex = 651;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(473, 440);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.DoubleClick += pictureBox1_DoubleClick;
            // 
            // btnSetWidth
            // 
            btnSetWidth.Location = new Point(162, 56);
            btnSetWidth.Name = "btnSetWidth";
            btnSetWidth.Size = new Size(58, 25);
            btnSetWidth.TabIndex = 652;
            btnSetWidth.Text = "Go";
            btnSetWidth.UseVisualStyleBackColor = true;
            btnSetWidth.Click += btnSetWidth_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 61);
            label2.Name = "label2";
            label2.Size = new Size(83, 15);
            label2.TabIndex = 653;
            label2.Text = "resize width to";
            // 
            // numNewWidth
            // 
            numNewWidth.Location = new Point(98, 59);
            numNewWidth.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numNewWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numNewWidth.Name = "numNewWidth";
            numNewWidth.Size = new Size(58, 23);
            numNewWidth.TabIndex = 654;
            numNewWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // rbAll
            // 
            rbAll.AutoSize = true;
            rbAll.Checked = true;
            rbAll.Location = new Point(15, 22);
            rbAll.Name = "rbAll";
            rbAll.Size = new Size(69, 19);
            rbAll.TabIndex = 655;
            rbAll.TabStop = true;
            rbAll.Text = "all in lb2";
            rbAll.UseVisualStyleBackColor = true;
            // 
            // rbOne
            // 
            rbOne.AutoSize = true;
            rbOne.Location = new Point(98, 22);
            rbOne.Name = "rbOne";
            rbOne.Size = new Size(77, 19);
            rbOne.TabIndex = 655;
            rbOne.Text = "one in lb2";
            rbOne.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbAll);
            groupBox1.Controls.Add(rbOne);
            groupBox1.Controls.Add(btnSetWidth);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(numNewWidth);
            groupBox1.Location = new Point(537, 174);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(281, 100);
            groupBox1.TabIndex = 656;
            groupBox1.TabStop = false;
            groupBox1.Text = "resize";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(rbTAll);
            groupBox2.Controls.Add(rbTOne);
            groupBox2.Controls.Add(btnSetPos);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(numTY);
            groupBox2.Controls.Add(numTX);
            groupBox2.Location = new Point(537, 280);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(281, 100);
            groupBox2.TabIndex = 657;
            groupBox2.TabStop = false;
            groupBox2.Text = "translate";
            // 
            // rbTAll
            // 
            rbTAll.AutoSize = true;
            rbTAll.Checked = true;
            rbTAll.Location = new Point(14, 22);
            rbTAll.Name = "rbTAll";
            rbTAll.Size = new Size(69, 19);
            rbTAll.TabIndex = 655;
            rbTAll.TabStop = true;
            rbTAll.Text = "all in lb2";
            rbTAll.UseVisualStyleBackColor = true;
            // 
            // rbTOne
            // 
            rbTOne.AutoSize = true;
            rbTOne.Location = new Point(97, 22);
            rbTOne.Name = "rbTOne";
            rbTOne.Size = new Size(77, 19);
            rbTOne.TabIndex = 655;
            rbTOne.Text = "one in lb2";
            rbTOne.UseVisualStyleBackColor = true;
            // 
            // btnSetPos
            // 
            btnSetPos.Location = new Point(204, 57);
            btnSetPos.Name = "btnSetPos";
            btnSetPos.Size = new Size(58, 25);
            btnSetPos.TabIndex = 652;
            btnSetPos.Text = "Go";
            btnSetPos.UseVisualStyleBackColor = true;
            btnSetPos.Click += btnSetPos_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(106, 62);
            label4.Name = "label4";
            label4.Size = new Size(16, 15);
            label4.TabIndex = 653;
            label4.Text = "y:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 62);
            label3.Name = "label3";
            label3.Size = new Size(16, 15);
            label3.TabIndex = 653;
            label3.Text = "x:";
            // 
            // numTY
            // 
            numTY.Location = new Point(128, 59);
            numTY.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numTY.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numTY.Name = "numTY";
            numTY.Size = new Size(58, 23);
            numTY.TabIndex = 654;
            // 
            // numTX
            // 
            numTX.Location = new Point(36, 59);
            numTX.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numTX.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numTX.Name = "numTX";
            numTX.Size = new Size(58, 23);
            numTX.TabIndex = 654;
            // 
            // btnSmothenSettings
            // 
            btnSmothenSettings.Enabled = false;
            btnSmothenSettings.Location = new Point(627, 393);
            btnSmothenSettings.Name = "btnSmothenSettings";
            btnSmothenSettings.Size = new Size(35, 23);
            btnSmothenSettings.TabIndex = 730;
            btnSmothenSettings.Text = "set";
            btnSmothenSettings.UseVisualStyleBackColor = true;
            btnSmothenSettings.Click += btnSmothenSettings_Click;
            // 
            // cbApproxLines
            // 
            cbApproxLines.AutoSize = true;
            cbApproxLines.Enabled = false;
            cbApproxLines.Location = new Point(552, 396);
            cbApproxLines.Name = "cbApproxLines";
            cbApproxLines.Size = new Size(80, 19);
            cbApproxLines.TabIndex = 729;
            cbApproxLines.Text = "smoothen";
            cbApproxLines.UseVisualStyleBackColor = true;
            // 
            // btnApproxLines
            // 
            btnApproxLines.Enabled = false;
            btnApproxLines.Location = new Point(668, 392);
            btnApproxLines.Name = "btnApproxLines";
            btnApproxLines.Size = new Size(58, 25);
            btnApproxLines.TabIndex = 728;
            btnApproxLines.Text = "Go";
            btnApproxLines.UseVisualStyleBackColor = true;
            btnApproxLines.Click += btnApproxLines_Click;
            // 
            // frmLastScribbles
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(841, 467);
            Controls.Add(btnSmothenSettings);
            Controls.Add(cbApproxLines);
            Controls.Add(btnApproxLines);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(panel1);
            Controls.Add(btnOK);
            Controls.Add(btnDelete);
            Controls.Add(listBox2);
            Controls.Add(listBox1);
            Controls.Add(label1);
            Controls.Add(cmbScribblesType);
            Name = "frmLastScribbles";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmLastScribbles";
            Load += frmLastScribbles_Load;
            Layout += frmLastScribbles_Layout;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numNewWidth).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTX).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbScribblesType;
        private Label label1;
        private ListBox listBox1;
        private ListBox listBox2;
        private Button btnDelete;
        private Button btnOK;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Button btnSetWidth;
        private Label label2;
        private NumericUpDown numNewWidth;
        private RadioButton rbAll;
        private RadioButton rbOne;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private RadioButton rbTAll;
        private RadioButton rbTOne;
        private Button btnSetPos;
        private Label label4;
        private Label label3;
        private NumericUpDown numTY;
        private NumericUpDown numTX;
        private Button btnSmothenSettings;
        private CheckBox cbApproxLines;
        private Button btnApproxLines;
    }
}