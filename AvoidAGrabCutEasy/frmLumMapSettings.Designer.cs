namespace AvoidAGrabCutEasy
{
    partial class frmLumMapSettings
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            numF1 = new NumericUpDown();
            numTh = new NumericUpDown();
            numF2 = new NumericUpDown();
            label4 = new Label();
            numExp1 = new NumericUpDown();
            label5 = new Label();
            numExp2 = new NumericUpDown();
            label6 = new Label();
            numThMultiplier = new NumericUpDown();
            cbAuto = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)numF1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTh).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numF2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numExp1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numExp2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numThMultiplier).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(314, 165);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 732;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(217, 165);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 733;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 17);
            label1.Name = "label1";
            label1.Size = new Size(44, 15);
            label1.TabIndex = 734;
            label1.Text = "factor1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 46);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 734;
            label2.Text = "threshold";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 107);
            label3.Name = "label3";
            label3.Size = new Size(44, 15);
            label3.TabIndex = 734;
            label3.Text = "factor2";
            // 
            // numF1
            // 
            numF1.DecimalPlaces = 4;
            numF1.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numF1.Location = new Point(89, 15);
            numF1.Name = "numF1";
            numF1.Size = new Size(88, 23);
            numF1.TabIndex = 735;
            numF1.Value = new decimal(new int[] { 25, 0, 0, 65536 });
            // 
            // numTh
            // 
            numTh.DecimalPlaces = 4;
            numTh.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numTh.Location = new Point(89, 44);
            numTh.Name = "numTh";
            numTh.Size = new Size(88, 23);
            numTh.TabIndex = 735;
            numTh.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // numF2
            // 
            numF2.DecimalPlaces = 4;
            numF2.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numF2.Location = new Point(89, 104);
            numF2.Name = "numF2";
            numF2.Size = new Size(88, 23);
            numF2.TabIndex = 735;
            numF2.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(202, 17);
            label4.Name = "label4";
            label4.Size = new Size(63, 15);
            label4.TabIndex = 734;
            label4.Text = "exponent1";
            // 
            // numExp1
            // 
            numExp1.DecimalPlaces = 4;
            numExp1.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numExp1.Location = new Point(292, 15);
            numExp1.Name = "numExp1";
            numExp1.Size = new Size(88, 23);
            numExp1.TabIndex = 735;
            numExp1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(202, 107);
            label5.Name = "label5";
            label5.Size = new Size(63, 15);
            label5.TabIndex = 734;
            label5.Text = "exponent2";
            // 
            // numExp2
            // 
            numExp2.DecimalPlaces = 4;
            numExp2.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numExp2.Location = new Point(292, 104);
            numExp2.Name = "numExp2";
            numExp2.Size = new Size(88, 23);
            numExp2.TabIndex = 735;
            numExp2.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(202, 46);
            label6.Name = "label6";
            label6.Size = new Size(86, 15);
            label6.TabIndex = 734;
            label6.Text = "multiplier 10^-";
            // 
            // numThMultiplier
            // 
            numThMultiplier.DecimalPlaces = 4;
            numThMultiplier.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numThMultiplier.Location = new Point(292, 44);
            numThMultiplier.Name = "numThMultiplier";
            numThMultiplier.Size = new Size(88, 23);
            numThMultiplier.TabIndex = 735;
            numThMultiplier.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // cbAuto
            // 
            cbAuto.AutoSize = true;
            cbAuto.Location = new Point(89, 73);
            cbAuto.Name = "cbAuto";
            cbAuto.Size = new Size(50, 19);
            cbAuto.TabIndex = 736;
            cbAuto.Text = "auto";
            cbAuto.UseVisualStyleBackColor = true;
            cbAuto.CheckedChanged += cbAuto_CheckedChanged;
            // 
            // frmLumMapSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(415, 204);
            Controls.Add(cbAuto);
            Controls.Add(numExp2);
            Controls.Add(numF2);
            Controls.Add(numThMultiplier);
            Controls.Add(numTh);
            Controls.Add(numExp1);
            Controls.Add(label5);
            Controls.Add(numF1);
            Controls.Add(label3);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmLumMapSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmLumMapSettings";
            ((System.ComponentModel.ISupportInitialize)numF1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTh).EndInit();
            ((System.ComponentModel.ISupportInitialize)numF2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numExp1).EndInit();
            ((System.ComponentModel.ISupportInitialize)numExp2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numThMultiplier).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private Button btnOK;
        private Label label1;
        private Label label2;
        private Label label3;
        internal NumericUpDown numF1;
        internal NumericUpDown numTh;
        internal NumericUpDown numF2;
        private Label label4;
        internal NumericUpDown numExp1;
        private Label label5;
        internal NumericUpDown numExp2;
        private Label label6;
        internal NumericUpDown numThMultiplier;
        internal CheckBox cbAuto;
    }
}