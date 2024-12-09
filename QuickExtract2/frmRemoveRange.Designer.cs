namespace QuickExtract2
{
    partial class frmRemoveRange
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
            this.btnOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numStIndx = new System.Windows.Forms.NumericUpDown();
            this.numEIndx = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numStIndx)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEIndx)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOK.Location = new System.Drawing.Point(249, 121);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 103;
            this.btnOK.Text = "Close";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 104;
            this.label1.Text = "StartPointIndex";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 105;
            this.label2.Text = "EndPointIndex";
            // 
            // numStIndx
            // 
            this.numStIndx.Location = new System.Drawing.Point(140, 22);
            this.numStIndx.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numStIndx.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numStIndx.Name = "numStIndx";
            this.numStIndx.Size = new System.Drawing.Size(75, 20);
            this.numStIndx.TabIndex = 106;
            this.numStIndx.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // numEIndx
            // 
            this.numEIndx.Location = new System.Drawing.Point(140, 55);
            this.numEIndx.Maximum = new decimal(new int[] {
            9999999,
            0,
            0,
            0});
            this.numEIndx.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.numEIndx.Name = "numEIndx";
            this.numEIndx.Size = new System.Drawing.Size(75, 20);
            this.numEIndx.TabIndex = 107;
            this.numEIndx.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(150, 81);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 23);
            this.button1.TabIndex = 108;
            this.button1.Text = "RemoveInBetween";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmRemoveRange
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 156);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.numEIndx);
            this.Controls.Add(this.numStIndx);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "frmRemoveRange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmRemoveRange";
            ((System.ComponentModel.ISupportInitialize)(this.numStIndx)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEIndx)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        internal System.Windows.Forms.NumericUpDown numStIndx;
        internal System.Windows.Forms.NumericUpDown numEIndx;
    }
}