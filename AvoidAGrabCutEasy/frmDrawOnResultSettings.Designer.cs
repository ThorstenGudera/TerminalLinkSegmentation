namespace AvoidAGrabCutEasy
{
    partial class frmDrawOnResultSettings
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
            cbSetPFGToFG = new CheckBox();
            cbSkipLearn = new CheckBox();
            label1 = new Label();
            numOpacity = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numOpacity).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(194, 85);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 736;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(97, 85);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 737;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // cbSetPFGToFG
            // 
            cbSetPFGToFG.AutoSize = true;
            cbSetPFGToFG.Location = new Point(97, 12);
            cbSetPFGToFG.Margin = new Padding(4, 3, 4, 3);
            cbSetPFGToFG.Name = "cbSetPFGToFG";
            cbSetPFGToFG.Size = new Size(73, 19);
            cbSetPFGToFG.TabIndex = 739;
            cbSetPFGToFG.Text = "PFGToFG";
            cbSetPFGToFG.UseVisualStyleBackColor = true;
            // 
            // cbSkipLearn
            // 
            cbSkipLearn.AutoSize = true;
            cbSkipLearn.Location = new Point(13, 12);
            cbSkipLearn.Margin = new Padding(4, 3, 4, 3);
            cbSkipLearn.Name = "cbSkipLearn";
            cbSkipLearn.Size = new Size(76, 19);
            cbSkipLearn.TabIndex = 738;
            cbSkipLearn.Text = "skipLearn";
            cbSkipLearn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 45);
            label1.Name = "label1";
            label1.Size = new Size(48, 15);
            label1.TabIndex = 744;
            label1.Text = "Opacity";
            // 
            // numOpacity
            // 
            numOpacity.DecimalPlaces = 2;
            numOpacity.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numOpacity.Location = new Point(76, 43);
            numOpacity.Margin = new Padding(4, 3, 4, 3);
            numOpacity.Maximum = new decimal(new int[] { 20, 0, 0, 65536 });
            numOpacity.Name = "numOpacity";
            numOpacity.Size = new Size(52, 23);
            numOpacity.TabIndex = 743;
            numOpacity.Value = new decimal(new int[] { 5, 0, 0, 65536 });
            // 
            // frmDrawOnResultSettings
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(295, 119);
            Controls.Add(label1);
            Controls.Add(numOpacity);
            Controls.Add(cbSetPFGToFG);
            Controls.Add(cbSkipLearn);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmDrawOnResultSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmDrawOnResultSettings";
            ((System.ComponentModel.ISupportInitialize)numOpacity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private Button btnOK;
        private Label label1;
        internal NumericUpDown numOpacity;
        internal CheckBox cbSetPFGToFG;
        internal CheckBox cbSkipLearn;
    }
}