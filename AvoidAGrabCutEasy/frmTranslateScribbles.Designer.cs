namespace AvoidAGrabCutEasy
{
    partial class frmTranslateScribbles
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
            label26 = new Label();
            label25 = new Label();
            numTY = new NumericUpDown();
            numTX = new NumericUpDown();
            btnCancel = new Button();
            btnOK = new Button();
            ((System.ComponentModel.ISupportInitialize)numTY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numTX).BeginInit();
            SuspendLayout();
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(95, 26);
            label26.Name = "label26";
            label26.Size = new Size(16, 15);
            label26.TabIndex = 726;
            label26.Text = "y:";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(12, 26);
            label25.Name = "label25";
            label25.Size = new Size(16, 15);
            label25.TabIndex = 727;
            label25.Text = "x:";
            // 
            // numTY
            // 
            numTY.Location = new Point(122, 24);
            numTY.Margin = new Padding(4, 3, 4, 3);
            numTY.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numTY.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numTY.Name = "numTY";
            numTY.Size = new Size(52, 23);
            numTY.TabIndex = 724;
            numTY.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // numTX
            // 
            numTX.Location = new Point(36, 24);
            numTX.Margin = new Padding(4, 3, 4, 3);
            numTX.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numTX.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numTX.Name = "numTX";
            numTX.Size = new Size(52, 23);
            numTX.TabIndex = 725;
            numTX.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(227, 77);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 728;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(130, 77);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 729;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // frmTranslateScribbles
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(328, 116);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            Controls.Add(label26);
            Controls.Add(label25);
            Controls.Add(numTY);
            Controls.Add(numTX);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmTranslateScribbles";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmTranslateScribbles";
            ((System.ComponentModel.ISupportInitialize)numTY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numTX).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label26;
        private Label label25;
        internal NumericUpDown numTY;
        internal NumericUpDown numTX;
        private Button btnCancel;
        private Button btnOK;
    }
}