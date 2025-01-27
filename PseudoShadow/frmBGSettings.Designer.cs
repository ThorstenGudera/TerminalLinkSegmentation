namespace PseudoShadow
{
    partial class frmBGSettings
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
            btnCancel = new Button();
            btnOK = new Button();
            groupBox2 = new GroupBox();
            label3 = new Label();
            numFWidth = new NumericUpDown();
            numFHeight = new NumericUpDown();
            label4 = new Label();
            groupBox1 = new GroupBox();
            CheckBox2 = new CheckBox();
            label5 = new Label();
            numHeight = new NumericUpDown();
            numWidth = new NumericUpDown();
            label6 = new Label();
            textBox1 = new TextBox();
            label1 = new Label();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numFWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numFHeight).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numHeight).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numWidth).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(291, 264);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 658;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(195, 264);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 659;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(numFWidth);
            groupBox2.Controls.Add(numFHeight);
            groupBox2.Controls.Add(label4);
            groupBox2.Location = new Point(13, 147);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(192, 104);
            groupBox2.TabIndex = 664;
            groupBox2.TabStop = false;
            groupBox2.Text = "Factor";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 30);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(39, 15);
            label3.TabIndex = 0;
            label3.Text = "Width";
            // 
            // numFWidth
            // 
            numFWidth.DecimalPlaces = 4;
            numFWidth.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numFWidth.Location = new Point(76, 28);
            numFWidth.Margin = new Padding(4, 3, 4, 3);
            numFWidth.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numFWidth.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numFWidth.Name = "numFWidth";
            numFWidth.Size = new Size(93, 23);
            numFWidth.TabIndex = 1;
            numFWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numFWidth.ValueChanged += numFWidth_ValueChanged;
            // 
            // numFHeight
            // 
            numFHeight.DecimalPlaces = 4;
            numFHeight.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numFHeight.Location = new Point(76, 60);
            numFHeight.Margin = new Padding(4, 3, 4, 3);
            numFHeight.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numFHeight.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numFHeight.Name = "numFHeight";
            numFHeight.Size = new Size(93, 23);
            numFHeight.TabIndex = 3;
            numFHeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numFHeight.ValueChanged += numFHeight_ValueChanged;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 62);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(43, 15);
            label4.TabIndex = 2;
            label4.Text = "Height";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(CheckBox2);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(numHeight);
            groupBox1.Controls.Add(numWidth);
            groupBox1.Controls.Add(label6);
            groupBox1.Location = new Point(13, 12);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(192, 127);
            groupBox1.TabIndex = 663;
            groupBox1.TabStop = false;
            groupBox1.Text = "Pixelsize";
            // 
            // CheckBox2
            // 
            CheckBox2.AutoSize = true;
            CheckBox2.Checked = true;
            CheckBox2.CheckState = CheckState.Checked;
            CheckBox2.Location = new Point(43, 97);
            CheckBox2.Margin = new Padding(4, 3, 4, 3);
            CheckBox2.Name = "CheckBox2";
            CheckBox2.Size = new Size(121, 19);
            CheckBox2.TabIndex = 24;
            CheckBox2.Text = "Keep Aspect Ratio";
            CheckBox2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 30);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(39, 15);
            label5.TabIndex = 0;
            label5.Text = "Width";
            // 
            // numHeight
            // 
            numHeight.Location = new Point(76, 60);
            numHeight.Margin = new Padding(4, 3, 4, 3);
            numHeight.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numHeight.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numHeight.Name = "numHeight";
            numHeight.Size = new Size(93, 23);
            numHeight.TabIndex = 3;
            numHeight.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numHeight.ValueChanged += numHeight_ValueChanged;
            // 
            // numWidth
            // 
            numWidth.Location = new Point(76, 28);
            numWidth.Margin = new Padding(4, 3, 4, 3);
            numWidth.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numWidth.Name = "numWidth";
            numWidth.Size = new Size(93, 23);
            numWidth.TabIndex = 1;
            numWidth.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numWidth.ValueChanged += numWidth_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(8, 62);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(43, 15);
            label6.TabIndex = 2;
            label6.Text = "Height";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(232, 43);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(147, 23);
            textBox1.TabIndex = 665;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(232, 25);
            label1.Name = "label1";
            label1.Size = new Size(71, 15);
            label1.TabIndex = 666;
            label1.Text = "current Size:";
            // 
            // frmBGSettings
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(392, 303);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmBGSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmBGSettings";
            Shown += frmBGSettings_Shown;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numFWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)numFHeight).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numHeight).EndInit();
            ((System.ComponentModel.ISupportInitialize)numWidth).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private Button btnOK;
        private GroupBox groupBox2;
        private Label label3;
        private Label label4;
        private GroupBox groupBox1;
        internal CheckBox CheckBox2;
        private Label label5;
        private Label label6;
        internal NumericUpDown numFWidth;
        internal NumericUpDown numFHeight;
        internal NumericUpDown numHeight;
        internal NumericUpDown numWidth;
        public TextBox textBox1;
        private Label label1;
    }
}