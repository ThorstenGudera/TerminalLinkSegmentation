﻿namespace AvoidAGrabCutEasy
{
    partial class frmIGGVals
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
            label22 = new Label();
            numIGGDivisor = new NumericUpDown();
            label21 = new Label();
            label18 = new Label();
            numIGGAlpha = new NumericUpDown();
            numIGGKernel = new NumericUpDown();
            label1 = new Label();
            numOpacity = new NumericUpDown();
            numReplace = new NumericUpDown();
            label3 = new Label();
            groupBox1 = new GroupBox();
            cbReplaceBG = new CheckBox();
            rbIGG = new RadioButton();
            rbVariance = new RadioButton();
            groupBox2 = new GroupBox();
            numVarGamma = new NumericUpDown();
            label6 = new Label();
            cbVarLog = new CheckBox();
            label5 = new Label();
            label4 = new Label();
            numVarExpander = new NumericUpDown();
            label2 = new Label();
            numVarKernel = new NumericUpDown();
            numVarTolerance = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numIGGDivisor).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIGGAlpha).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numIGGKernel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOpacity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numReplace).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numVarGamma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVarExpander).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVarKernel).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numVarTolerance).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(246, 331);
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
            btnOK.Location = new Point(149, 331);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 735;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(10, 50);
            label22.Name = "label22";
            label22.Size = new Size(38, 15);
            label22.TabIndex = 741;
            label22.Text = "Alpha";
            // 
            // numIGGDivisor
            // 
            numIGGDivisor.Location = new Point(161, 47);
            numIGGDivisor.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numIGGDivisor.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numIGGDivisor.Name = "numIGGDivisor";
            numIGGDivisor.Size = new Size(52, 23);
            numIGGDivisor.TabIndex = 736;
            numIGGDivisor.Value = new decimal(new int[] { 8, 0, 0, 0 });
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(112, 50);
            label21.Name = "label21";
            label21.Size = new Size(43, 15);
            label21.TabIndex = 740;
            label21.Text = "Divisor";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(10, 19);
            label18.Name = "label18";
            label18.Size = new Size(115, 15);
            label18.TabIndex = 739;
            label18.Text = "InvGaussGrad Kernel";
            // 
            // numIGGAlpha
            // 
            numIGGAlpha.Location = new Point(55, 48);
            numIGGAlpha.Margin = new Padding(4, 3, 4, 3);
            numIGGAlpha.Maximum = new decimal(new int[] { 9999, 0, 0, 0 });
            numIGGAlpha.Name = "numIGGAlpha";
            numIGGAlpha.Size = new Size(52, 23);
            numIGGAlpha.TabIndex = 737;
            numIGGAlpha.Value = new decimal(new int[] { 101, 0, 0, 0 });
            // 
            // numIGGKernel
            // 
            numIGGKernel.Location = new Point(137, 17);
            numIGGKernel.Margin = new Padding(4, 3, 4, 3);
            numIGGKernel.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numIGGKernel.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numIGGKernel.Name = "numIGGKernel";
            numIGGKernel.Size = new Size(52, 23);
            numIGGKernel.TabIndex = 738;
            numIGGKernel.Value = new decimal(new int[] { 27, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 85);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 742;
            label1.Text = "Opacity";
            // 
            // numOpacity
            // 
            numOpacity.DecimalPlaces = 2;
            numOpacity.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numOpacity.Location = new Point(73, 83);
            numOpacity.Margin = new Padding(4, 3, 4, 3);
            numOpacity.Maximum = new decimal(new int[] { 20, 0, 0, 65536 });
            numOpacity.Name = "numOpacity";
            numOpacity.Size = new Size(52, 23);
            numOpacity.TabIndex = 738;
            numOpacity.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // numReplace
            // 
            numReplace.Location = new Point(161, 119);
            numReplace.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numReplace.Name = "numReplace";
            numReplace.Size = new Size(52, 23);
            numReplace.TabIndex = 744;
            numReplace.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(98, 121);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 743;
            label3.Text = "tolerance";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(cbReplaceBG);
            groupBox1.Controls.Add(label18);
            groupBox1.Controls.Add(numReplace);
            groupBox1.Controls.Add(numIGGKernel);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(numOpacity);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(numIGGAlpha);
            groupBox1.Controls.Add(label22);
            groupBox1.Controls.Add(label21);
            groupBox1.Controls.Add(numIGGDivisor);
            groupBox1.Location = new Point(12, 35);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(254, 153);
            groupBox1.TabIndex = 745;
            groupBox1.TabStop = false;
            groupBox1.Text = "IGG";
            // 
            // cbReplaceBG
            // 
            cbReplaceBG.AutoSize = true;
            cbReplaceBG.Location = new Point(10, 119);
            cbReplaceBG.Name = "cbReplaceBG";
            cbReplaceBG.Size = new Size(82, 19);
            cbReplaceBG.TabIndex = 745;
            cbReplaceBG.Text = "replace BG";
            cbReplaceBG.UseVisualStyleBackColor = true;
            // 
            // rbIGG
            // 
            rbIGG.AutoSize = true;
            rbIGG.Checked = true;
            rbIGG.Location = new Point(12, 10);
            rbIGG.Name = "rbIGG";
            rbIGG.Size = new Size(97, 19);
            rbIGG.TabIndex = 746;
            rbIGG.TabStop = true;
            rbIGG.Text = "InvGaussGrad";
            rbIGG.UseVisualStyleBackColor = true;
            rbIGG.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // rbVariance
            // 
            rbVariance.AutoSize = true;
            rbVariance.Location = new Point(115, 10);
            rbVariance.Name = "rbVariance";
            rbVariance.Size = new Size(69, 19);
            rbVariance.TabIndex = 746;
            rbVariance.Text = "Variance";
            rbVariance.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(numVarGamma);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(cbVarLog);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(numVarExpander);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(numVarKernel);
            groupBox2.Controls.Add(numVarTolerance);
            groupBox2.Location = new Point(12, 194);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(254, 120);
            groupBox2.TabIndex = 747;
            groupBox2.TabStop = false;
            groupBox2.Text = "Variance";
            // 
            // numVarGamma
            // 
            numVarGamma.DecimalPlaces = 4;
            numVarGamma.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numVarGamma.Location = new Point(161, 83);
            numVarGamma.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            numVarGamma.Name = "numVarGamma";
            numVarGamma.Size = new Size(69, 23);
            numVarGamma.TabIndex = 747;
            numVarGamma.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(103, 86);
            label6.Name = "label6";
            label6.Size = new Size(48, 15);
            label6.TabIndex = 746;
            label6.Text = "gamma";
            // 
            // cbVarLog
            // 
            cbVarLog.AutoSize = true;
            cbVarLog.Location = new Point(15, 85);
            cbVarLog.Name = "cbVarLog";
            cbVarLog.Size = new Size(87, 19);
            cbVarLog.TabIndex = 745;
            cbVarLog.Text = "logarithmic";
            cbVarLog.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(115, 19);
            label5.Name = "label5";
            label5.Size = new Size(56, 15);
            label5.TabIndex = 739;
            label5.Text = "Expander";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(10, 19);
            label4.Name = "label4";
            label4.Size = new Size(40, 15);
            label4.TabIndex = 739;
            label4.Text = "Kernel";
            // 
            // numVarExpander
            // 
            numVarExpander.Location = new Point(178, 17);
            numVarExpander.Margin = new Padding(4, 3, 4, 3);
            numVarExpander.Maximum = new decimal(new int[] { 256, 0, 0, 0 });
            numVarExpander.Name = "numVarExpander";
            numVarExpander.Size = new Size(52, 23);
            numVarExpander.TabIndex = 738;
            numVarExpander.Value = new decimal(new int[] { 64, 0, 0, 0 });
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 56);
            label2.Name = "label2";
            label2.Size = new Size(139, 15);
            label2.TabIndex = 743;
            label2.Text = "Set BG Transp    tolerance";
            // 
            // numVarKernel
            // 
            numVarKernel.Location = new Point(55, 17);
            numVarKernel.Margin = new Padding(4, 3, 4, 3);
            numVarKernel.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numVarKernel.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
            numVarKernel.Name = "numVarKernel";
            numVarKernel.Size = new Size(52, 23);
            numVarKernel.TabIndex = 738;
            numVarKernel.Value = new decimal(new int[] { 27, 0, 0, 0 });
            // 
            // numVarTolerance
            // 
            numVarTolerance.Location = new Point(161, 54);
            numVarTolerance.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numVarTolerance.Name = "numVarTolerance";
            numVarTolerance.Size = new Size(52, 23);
            numVarTolerance.TabIndex = 744;
            numVarTolerance.Value = new decimal(new int[] { 60, 0, 0, 0 });
            // 
            // frmIGGVals
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(347, 370);
            Controls.Add(groupBox2);
            Controls.Add(rbVariance);
            Controls.Add(rbIGG);
            Controls.Add(groupBox1);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmIGGVals";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmIGGVals";
            ((System.ComponentModel.ISupportInitialize)numIGGDivisor).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIGGAlpha).EndInit();
            ((System.ComponentModel.ISupportInitialize)numIGGKernel).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOpacity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numReplace).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numVarGamma).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVarExpander).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVarKernel).EndInit();
            ((System.ComponentModel.ISupportInitialize)numVarTolerance).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btnCancel;
        private Button btnOK;
        private Label label22;
        private Label label21;
        private Label label18;
        internal NumericUpDown numIGGAlpha;
        internal NumericUpDown numIGGKernel;
        internal NumericUpDown numIGGDivisor;
        private Label label1;
        internal NumericUpDown numOpacity;
        internal NumericUpDown numReplace;
        private Label label3;
        private GroupBox groupBox1;
        private Label label5;
        private Label label4;
        internal NumericUpDown numVarExpander;
        private Label label2;
        internal NumericUpDown numVarKernel;
        internal NumericUpDown numVarTolerance;
        internal RadioButton rbIGG;
        internal RadioButton rbVariance;
        internal CheckBox cbReplaceBG;
        internal NumericUpDown numVarGamma;
        private Label label6;
        internal CheckBox cbVarLog;
        internal GroupBox groupBox2;
    }
}