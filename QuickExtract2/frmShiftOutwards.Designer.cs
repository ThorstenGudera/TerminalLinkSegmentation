namespace QuickExtract2
{
    partial class frmShiftOutwards
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
            this.components = new System.ComponentModel.Container();
            this.NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.Label1 = new System.Windows.Forms.Label();
            this.Button13 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ComboBox2 = new System.Windows.Forms.ComboBox();
            this.ComboBox1 = new System.Windows.Forms.ComboBox();
            this.Button2 = new System.Windows.Forms.Button();
            this.Label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // NumericUpDown1
            // 
            this.NumericUpDown1.DecimalPlaces = 4;
            this.NumericUpDown1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.NumericUpDown1.Location = new System.Drawing.Point(49, 18);
            this.NumericUpDown1.Maximum = new decimal(new int[] {
            21500,
            0,
            0,
            0});
            this.NumericUpDown1.Minimum = new decimal(new int[] {
            21500,
            0,
            0,
            -2147483648});
            this.NumericUpDown1.Name = "NumericUpDown1";
            this.NumericUpDown1.Size = new System.Drawing.Size(75, 20);
            this.NumericUpDown1.TabIndex = 57;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Label1.Location = new System.Drawing.Point(16, 20);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(15, 15);
            this.Label1.TabIndex = 56;
            this.Label1.Text = ">";
            this.ToolTip1.SetToolTip(this.Label1, "shift outwards");
            // 
            // Button13
            // 
            this.Button13.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button13.Location = new System.Drawing.Point(306, 61);
            this.Button13.Name = "Button13";
            this.Button13.Size = new System.Drawing.Size(75, 23);
            this.Button13.TabIndex = 54;
            this.Button13.Text = "Cancel";
            this.Button13.UseVisualStyleBackColor = true;
            // 
            // Button1
            // 
            this.Button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button1.Location = new System.Drawing.Point(225, 61);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(75, 23);
            this.Button1.TabIndex = 55;
            this.Button1.Text = "OK";
            this.Button1.UseVisualStyleBackColor = true;
            // 
            // ComboBox2
            // 
            this.ComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox2.FormattingEnabled = true;
            this.ComboBox2.Location = new System.Drawing.Point(16, 50);
            this.ComboBox2.Name = "ComboBox2";
            this.ComboBox2.Size = new System.Drawing.Size(121, 21);
            this.ComboBox2.TabIndex = 61;
            // 
            // ComboBox1
            // 
            this.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox1.FormattingEnabled = true;
            this.ComboBox1.Location = new System.Drawing.Point(245, 17);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(121, 21);
            this.ComboBox1.TabIndex = 60;
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(153, 15);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(75, 23);
            this.Button2.TabIndex = 59;
            this.Button2.Text = "Reset";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(147, 66);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(58, 13);
            this.Label5.TabIndex = 58;
            this.Label5.Text = "Set Values";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 99);
            this.Controls.Add(this.NumericUpDown1);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Button13);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.ComboBox2);
            this.Controls.Add(this.ComboBox1);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Label5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Shift Path Coordinates outwards...";
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.NumericUpDown NumericUpDown1;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.ToolTip ToolTip1;
        internal System.Windows.Forms.Button Button13;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.ComboBox ComboBox2;
        internal System.Windows.Forms.ComboBox ComboBox1;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Label Label5;
    }
}