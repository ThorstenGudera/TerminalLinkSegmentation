namespace QuickExtract2
{
    partial class frmSelectCrop
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
            Label2 = new Label();
            RadioButton2 = new RadioButton();
            RadioButton1 = new RadioButton();
            CheckBox1 = new CheckBox();
            Label1 = new Label();
            CheckedListBox1 = new CheckedListBox();
            Button2 = new Button();
            Button1 = new Button();
            SuspendLayout();
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Location = new Point(18, 129);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(76, 15);
            Label2.TabIndex = 293;
            Label2.Text = "keep content";
            // 
            // RadioButton2
            // 
            RadioButton2.AutoSize = true;
            RadioButton2.Location = new Point(110, 151);
            RadioButton2.Margin = new Padding(4, 3, 4, 3);
            RadioButton2.Name = "RadioButton2";
            RadioButton2.Size = new Size(91, 19);
            RadioButton2.TabIndex = 291;
            RadioButton2.Text = "outside path";
            RadioButton2.UseVisualStyleBackColor = true;
            // 
            // RadioButton1
            // 
            RadioButton1.AutoSize = true;
            RadioButton1.Checked = true;
            RadioButton1.Location = new Point(18, 151);
            RadioButton1.Margin = new Padding(4, 3, 4, 3);
            RadioButton1.Name = "RadioButton1";
            RadioButton1.Size = new Size(83, 19);
            RadioButton1.TabIndex = 292;
            RadioButton1.TabStop = true;
            RadioButton1.Text = "inside path";
            RadioButton1.UseVisualStyleBackColor = true;
            // 
            // CheckBox1
            // 
            CheckBox1.AutoSize = true;
            CheckBox1.Checked = true;
            CheckBox1.CheckState = CheckState.Checked;
            CheckBox1.Location = new Point(156, 10);
            CheckBox1.Margin = new Padding(4, 3, 4, 3);
            CheckBox1.Name = "CheckBox1";
            CheckBox1.Size = new Size(93, 19);
            CheckBox1.TabIndex = 290;
            CheckBox1.Text = "Current Path";
            CheckBox1.UseVisualStyleBackColor = true;
            CheckBox1.CheckedChanged += CheckBox1_CheckedChanged;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(14, 10);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(114, 15);
            Label1.TabIndex = 289;
            Label1.Text = "Select Paths to crop:";
            // 
            // CheckedListBox1
            // 
            CheckedListBox1.CheckOnClick = true;
            CheckedListBox1.FormattingEnabled = true;
            CheckedListBox1.Location = new Point(156, 37);
            CheckedListBox1.Margin = new Padding(4, 3, 4, 3);
            CheckedListBox1.Name = "CheckedListBox1";
            CheckedListBox1.Size = new Size(139, 94);
            CheckedListBox1.TabIndex = 288;
            CheckedListBox1.SelectedIndexChanged += CheckedListBox1_SelectedIndexChanged;
            // 
            // Button2
            // 
            Button2.DialogResult = DialogResult.Cancel;
            Button2.Location = new Point(290, 181);
            Button2.Margin = new Padding(4, 3, 4, 3);
            Button2.Name = "Button2";
            Button2.Size = new Size(88, 27);
            Button2.TabIndex = 287;
            Button2.Text = "Cancel";
            Button2.UseVisualStyleBackColor = true;
            // 
            // Button1
            // 
            Button1.DialogResult = DialogResult.OK;
            Button1.Location = new Point(196, 181);
            Button1.Margin = new Padding(4, 3, 4, 3);
            Button1.Name = "Button1";
            Button1.Size = new Size(88, 27);
            Button1.TabIndex = 286;
            Button1.Text = "OK";
            Button1.UseVisualStyleBackColor = true;
            // 
            // frmSelectCrop
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(393, 212);
            Controls.Add(Label2);
            Controls.Add(RadioButton2);
            Controls.Add(RadioButton1);
            Controls.Add(CheckBox1);
            Controls.Add(Label1);
            Controls.Add(CheckedListBox1);
            Controls.Add(Button2);
            Controls.Add(Button1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "frmSelectCrop";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmSelectCrop";
            Load += Form7_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Button Button1;
        public RadioButton RadioButton1;
        public CheckBox CheckBox1;
        public CheckedListBox CheckedListBox1;
        public RadioButton RadioButton2;
    }
}