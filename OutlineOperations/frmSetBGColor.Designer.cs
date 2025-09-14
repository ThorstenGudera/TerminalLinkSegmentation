namespace OutlineOperations
{
    partial class frmSetBGColor
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
            btnSelCol = new Button();
            colorDialog1 = new ColorDialog();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(236, 116);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 102;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(143, 116);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 103;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // btnSelCol
            // 
            btnSelCol.Location = new Point(106, 13);
            btnSelCol.Name = "btnSelCol";
            btnSelCol.Size = new Size(75, 23);
            btnSelCol.TabIndex = 104;
            btnSelCol.Text = "Go";
            btnSelCol.UseVisualStyleBackColor = true;
            btnSelCol.Click += btnSelCol_Click;
            // 
            // colorDialog1
            // 
            colorDialog1.AnyColor = true;
            colorDialog1.FullOpen = true;
            // 
            // label1
            // 
            label1.BorderStyle = BorderStyle.FixedSingle;
            label1.Location = new Point(200, 13);
            label1.Name = "label1";
            label1.Size = new Size(124, 77);
            label1.TabIndex = 105;
            label1.Text = "    ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 17);
            label2.Name = "label2";
            label2.Size = new Size(69, 15);
            label2.TabIndex = 106;
            label2.Text = "select Color";
            // 
            // frmSetBGColor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(343, 155);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(btnSelCol);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "frmSetBGColor";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmSetBGColor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private Button btnOK;
        private Button btnSelCol;
        private ColorDialog colorDialog1;
        private Label label2;
        internal Label label1;
    }
}