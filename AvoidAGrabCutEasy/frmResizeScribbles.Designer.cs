namespace AvoidAGrabCutEasy
{
    partial class frmResizeScribbles
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
            numRScribblesFactor = new NumericUpDown();
            label23 = new Label();
            ((System.ComponentModel.ISupportInitialize)numRScribblesFactor).BeginInit();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(235, 71);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 730;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(138, 71);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 731;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // numRScribblesFactor
            // 
            numRScribblesFactor.DecimalPlaces = 4;
            numRScribblesFactor.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numRScribblesFactor.Location = new Point(142, 18);
            numRScribblesFactor.Margin = new Padding(4, 3, 4, 3);
            numRScribblesFactor.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numRScribblesFactor.Name = "numRScribblesFactor";
            numRScribblesFactor.Size = new Size(71, 23);
            numRScribblesFactor.TabIndex = 733;
            numRScribblesFactor.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(12, 21);
            label23.Name = "label23";
            label23.Size = new Size(123, 15);
            label23.TabIndex = 732;
            label23.Text = "Resize Scribbles factor";
            // 
            // frmResizeScribbles
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(336, 110);
            Controls.Add(numRScribblesFactor);
            Controls.Add(label23);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            MaximizeBox = false;
            Name = "frmResizeScribbles";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmResizeScribbles";
            ((System.ComponentModel.ISupportInitialize)numRScribblesFactor).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private Button btnOK;
        internal NumericUpDown numRScribblesFactor;
        private Label label23;
    }
}