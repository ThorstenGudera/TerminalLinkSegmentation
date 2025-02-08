namespace AvoidAGrabCutEasy
{
    partial class frmLoadScribblesSettings
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
            rbDynamic = new RadioButton();
            rbFixed = new RadioButton();
            label1 = new Label();
            panel4 = new Panel();
            panel2 = new Panel();
            numDivisor = new NumericUpDown();
            numNewWidth = new NumericUpDown();
            panel1 = new Panel();
            panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDivisor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numNewWidth).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(248, 131);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 645;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(151, 131);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 646;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // rbDynamic
            // 
            rbDynamic.AutoSize = true;
            rbDynamic.Checked = true;
            rbDynamic.Location = new Point(12, 19);
            rbDynamic.Name = "rbDynamic";
            rbDynamic.Size = new Size(74, 19);
            rbDynamic.TabIndex = 647;
            rbDynamic.TabStop = true;
            rbDynamic.Text = "dynamic ";
            rbDynamic.UseVisualStyleBackColor = true;
            // 
            // rbFixed
            // 
            rbFixed.AutoSize = true;
            rbFixed.Location = new Point(12, 81);
            rbFixed.Name = "rbFixed";
            rbFixed.Size = new Size(76, 19);
            rbFixed.TabIndex = 648;
            rbFixed.Text = "fixed, size";
            rbFixed.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(104, 11);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 649;
            label1.Text = "pen-width";
            // 
            // panel4
            // 
            panel4.BackColor = SystemColors.ControlDarkDark;
            panel4.Controls.Add(panel2);
            panel4.Location = new Point(100, 29);
            panel4.Name = "panel4";
            panel4.Size = new Size(70, 2);
            panel4.TabIndex = 697;
            // 
            // panel2
            // 
            panel2.BackColor = SystemColors.ControlDarkDark;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(70, 2);
            panel2.TabIndex = 697;
            // 
            // numDivisor
            // 
            numDivisor.Location = new Point(104, 37);
            numDivisor.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numDivisor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDivisor.Name = "numDivisor";
            numDivisor.Size = new Size(62, 23);
            numDivisor.TabIndex = 698;
            numDivisor.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // numNewWidth
            // 
            numNewWidth.Location = new Point(104, 79);
            numNewWidth.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            numNewWidth.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numNewWidth.Name = "numNewWidth";
            numNewWidth.Size = new Size(62, 23);
            numNewWidth.TabIndex = 698;
            numNewWidth.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ControlDarkDark;
            panel1.Location = new Point(100, 29);
            panel1.Name = "panel1";
            panel1.Size = new Size(70, 2);
            panel1.TabIndex = 697;
            // 
            // frmLoadScribblesSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(358, 172);
            Controls.Add(numNewWidth);
            Controls.Add(numDivisor);
            Controls.Add(panel1);
            Controls.Add(panel4);
            Controls.Add(label1);
            Controls.Add(rbFixed);
            Controls.Add(rbDynamic);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmLoadScribblesSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmLoadScribblesSettings";
            panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numDivisor).EndInit();
            ((System.ComponentModel.ISupportInitialize)numNewWidth).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private Button btnOK;
        private Label label1;
        private Panel panel4;
        internal RadioButton rbDynamic;
        internal RadioButton rbFixed;
        internal NumericUpDown numDivisor;
        internal NumericUpDown numNewWidth;
        private Panel panel2;
        private Panel panel1;
    }
}