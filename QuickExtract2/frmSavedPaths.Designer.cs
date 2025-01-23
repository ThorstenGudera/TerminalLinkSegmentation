namespace QuickExtract2
{
    partial class frmSavedPaths
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
            this.OpenFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.SaveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.Button9 = new System.Windows.Forms.Button();
            this.Label4 = new System.Windows.Forms.Label();
            this.CheckBox3 = new System.Windows.Forms.CheckBox();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.Label2 = new System.Windows.Forms.Label();
            this.Button7 = new System.Windows.Forms.Button();
            this.Button6 = new System.Windows.Forms.Button();
            this.CheckBox2 = new System.Windows.Forms.CheckBox();
            this.Button8 = new System.Windows.Forms.Button();
            this.Button5 = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.CheckBox1 = new System.Windows.Forms.CheckBox();
            this.Button4 = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.ListBox1 = new System.Windows.Forms.ListBox();
            this.NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.HelplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            this.Label3 = new System.Windows.Forms.Label();
            this.Button2 = new System.Windows.Forms.Button();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.CheckBox4 = new System.Windows.Forms.CheckBox();
            this.CheckBox12 = new System.Windows.Forms.CheckBox();
            this.Button13 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).BeginInit();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenFileDialog2
            // 
            this.OpenFileDialog2.FileName = "File1";
            this.OpenFileDialog2.Filter = "GraphicsPath-Files (*.tggrp)|*.tggrp";
            // 
            // ColorDialog1
            // 
            this.ColorDialog1.AnyColor = true;
            this.ColorDialog1.FullOpen = true;
            // 
            // SaveFileDialog2
            // 
            this.SaveFileDialog2.DefaultExt = "tggrp";
            this.SaveFileDialog2.FileName = "File1.tggrp";
            this.SaveFileDialog2.Filter = "GraphicsPath files (*.tggrp)|*.tggrp";
            this.SaveFileDialog2.RestoreDirectory = true;
            // 
            // Button9
            // 
            this.Button9.Location = new System.Drawing.Point(98, 8);
            this.Button9.Name = "Button9";
            this.Button9.Size = new System.Drawing.Size(94, 23);
            this.Button9.TabIndex = 11;
            this.Button9.Text = "unselect ListBox";
            this.Button9.UseVisualStyleBackColor = true;
            this.Button9.Click += new System.EventHandler(this.Button9_Click);
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Enabled = false;
            this.Label4.Location = new System.Drawing.Point(13, 143);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(98, 13);
            this.Label4.TabIndex = 10;
            this.Label4.Text = "Add CurPath to List";
            // 
            // CheckBox3
            // 
            this.CheckBox3.AutoSize = true;
            this.CheckBox3.Checked = true;
            this.CheckBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox3.Location = new System.Drawing.Point(119, 289);
            this.CheckBox3.Name = "CheckBox3";
            this.CheckBox3.Size = new System.Drawing.Size(75, 17);
            this.CheckBox3.TabIndex = 9;
            this.CheckBox3.Text = "save copy";
            this.CheckBox3.UseVisualStyleBackColor = true;
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.Button9);
            this.Panel2.Controls.Add(this.Label4);
            this.Panel2.Controls.Add(this.CheckBox3);
            this.Panel2.Controls.Add(this.Label2);
            this.Panel2.Controls.Add(this.Button7);
            this.Panel2.Controls.Add(this.Button6);
            this.Panel2.Controls.Add(this.CheckBox2);
            this.Panel2.Controls.Add(this.Button8);
            this.Panel2.Controls.Add(this.Button5);
            this.Panel2.Controls.Add(this.Label1);
            this.Panel2.Controls.Add(this.CheckBox1);
            this.Panel2.Controls.Add(this.Button4);
            this.Panel2.Controls.Add(this.Button3);
            this.Panel2.Controls.Add(this.ListBox1);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel2.Location = new System.Drawing.Point(727, 0);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(200, 487);
            this.Panel2.TabIndex = 14;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(37, 333);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(147, 52);
            this.Label2.TabIndex = 8;
            this.Label2.Text = "You need to set the path you \r\nwant to work with to CurPath\r\nCurPath will be disp" +
    "layed in \r\nthe Parent form. ";
            // 
            // Button7
            // 
            this.Button7.Location = new System.Drawing.Point(37, 422);
            this.Button7.Name = "Button7";
            this.Button7.Size = new System.Drawing.Size(75, 23);
            this.Button7.TabIndex = 7;
            this.Button7.Text = "Load Path";
            this.Button7.UseVisualStyleBackColor = true;
            this.Button7.Click += new System.EventHandler(this.Button7_Click);
            // 
            // Button6
            // 
            this.Button6.Location = new System.Drawing.Point(117, 422);
            this.Button6.Name = "Button6";
            this.Button6.Size = new System.Drawing.Size(75, 23);
            this.Button6.TabIndex = 7;
            this.Button6.Text = "Save Path";
            this.Button6.UseVisualStyleBackColor = true;
            this.Button6.Click += new System.EventHandler(this.Button6_Click);
            // 
            // CheckBox2
            // 
            this.CheckBox2.AutoSize = true;
            this.CheckBox2.Location = new System.Drawing.Point(37, 13);
            this.CheckBox2.Name = "CheckBox2";
            this.CheckBox2.Size = new System.Drawing.Size(64, 17);
            this.CheckBox2.TabIndex = 6;
            this.CheckBox2.Text = "CurPath";
            this.CheckBox2.UseVisualStyleBackColor = true;
            this.CheckBox2.CheckedChanged += new System.EventHandler(this.CheckBox2_CheckedChanged);
            // 
            // Button8
            // 
            this.Button8.Enabled = false;
            this.Button8.Location = new System.Drawing.Point(117, 138);
            this.Button8.Name = "Button8";
            this.Button8.Size = new System.Drawing.Size(75, 23);
            this.Button8.TabIndex = 5;
            this.Button8.Text = "Add";
            this.Button8.UseVisualStyleBackColor = true;
            this.Button8.Click += new System.EventHandler(this.Button8_Click);
            // 
            // Button5
            // 
            this.Button5.Location = new System.Drawing.Point(37, 284);
            this.Button5.Name = "Button5";
            this.Button5.Size = new System.Drawing.Size(75, 23);
            this.Button5.TabIndex = 5;
            this.Button5.Text = "go";
            this.Button5.UseVisualStyleBackColor = true;
            this.Button5.Click += new System.EventHandler(this.Button5_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(62, 310);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(95, 13);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "swap with CurPath";
            // 
            // CheckBox1
            // 
            this.CheckBox1.AutoSize = true;
            this.CheckBox1.Checked = true;
            this.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox1.Location = new System.Drawing.Point(118, 226);
            this.CheckBox1.Name = "CheckBox1";
            this.CheckBox1.Size = new System.Drawing.Size(74, 17);
            this.CheckBox1.TabIndex = 3;
            this.CheckBox1.Text = "shift 50 px";
            this.CheckBox1.UseVisualStyleBackColor = true;
            // 
            // Button4
            // 
            this.Button4.Location = new System.Drawing.Point(37, 222);
            this.Button4.Name = "Button4";
            this.Button4.Size = new System.Drawing.Size(75, 23);
            this.Button4.TabIndex = 2;
            this.Button4.Text = "Clone";
            this.Button4.UseVisualStyleBackColor = true;
            this.Button4.Click += new System.EventHandler(this.Button4_Click);
            // 
            // Button3
            // 
            this.Button3.Location = new System.Drawing.Point(37, 183);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(75, 23);
            this.Button3.TabIndex = 1;
            this.Button3.Text = "Delete";
            this.Button3.UseVisualStyleBackColor = true;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // ListBox1
            // 
            this.ListBox1.FormattingEnabled = true;
            this.ListBox1.Location = new System.Drawing.Point(37, 37);
            this.ListBox1.Name = "ListBox1";
            this.ListBox1.Size = new System.Drawing.Size(120, 95);
            this.ListBox1.TabIndex = 0;
            this.ListBox1.SelectedIndexChanged += new System.EventHandler(this.ListBox1_SelectedIndexChanged);
            // 
            // NumericUpDown1
            // 
            this.NumericUpDown1.DecimalPlaces = 2;
            this.NumericUpDown1.Location = new System.Drawing.Point(466, 17);
            this.NumericUpDown1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.NumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown1.Name = "NumericUpDown1";
            this.NumericUpDown1.Size = new System.Drawing.Size(50, 20);
            this.NumericUpDown1.TabIndex = 246;
            this.NumericUpDown1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NumericUpDown1.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // HelplineRulerCtrl1
            // 
            this.HelplineRulerCtrl1.Bmp = null;
            this.HelplineRulerCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HelplineRulerCtrl1.DontDoLayout = false;
            this.HelplineRulerCtrl1.DontHandleDoubleClick = false;
            this.HelplineRulerCtrl1.DontPaintBaseImg = false;
            this.HelplineRulerCtrl1.DontProcDoubleClick = false;
            this.HelplineRulerCtrl1.DrawModeClipped = false;
            this.HelplineRulerCtrl1.DrawPixelated = false;
            this.HelplineRulerCtrl1.IgnoreZoom = false;
            this.HelplineRulerCtrl1.Location = new System.Drawing.Point(0, 0);
            this.HelplineRulerCtrl1.MoveHelpLinesOnResize = false;
            this.HelplineRulerCtrl1.Name = "HelplineRulerCtrl1";
            this.HelplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            this.HelplineRulerCtrl1.Size = new System.Drawing.Size(927, 487);
            this.HelplineRulerCtrl1.TabIndex = 15;
            this.HelplineRulerCtrl1.Zoom = 1F;
            this.HelplineRulerCtrl1.ZoomSetManually = false;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(406, 20);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(57, 13);
            this.Label3.TabIndex = 247;
            this.Label3.Text = "Pen width:";
            // 
            // Button2
            // 
            this.Button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button2.Location = new System.Drawing.Point(835, 15);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(75, 23);
            this.Button2.TabIndex = 9;
            this.Button2.Text = "Cancel";
            this.Button2.UseVisualStyleBackColor = true;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.CheckBox4);
            this.Panel1.Controls.Add(this.CheckBox12);
            this.Panel1.Controls.Add(this.Button13);
            this.Panel1.Controls.Add(this.Label3);
            this.Panel1.Controls.Add(this.NumericUpDown1);
            this.Panel1.Controls.Add(this.Button1);
            this.Panel1.Controls.Add(this.Button2);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 487);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(927, 50);
            this.Panel1.TabIndex = 13;
            // 
            // CheckBox4
            // 
            this.CheckBox4.AutoSize = true;
            this.CheckBox4.Location = new System.Drawing.Point(608, 20);
            this.CheckBox4.Name = "CheckBox4";
            this.CheckBox4.Size = new System.Drawing.Size(104, 17);
            this.CheckBox4.TabIndex = 250;
            this.CheckBox4.Text = "color for Curpath";
            this.CheckBox4.UseVisualStyleBackColor = true;
            // 
            // CheckBox12
            // 
            this.CheckBox12.AutoSize = true;
            this.CheckBox12.Checked = true;
            this.CheckBox12.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox12.Location = new System.Drawing.Point(319, 18);
            this.CheckBox12.Name = "CheckBox12";
            this.CheckBox12.Size = new System.Drawing.Size(65, 17);
            this.CheckBox12.TabIndex = 249;
            this.CheckBox12.Text = "BG dark";
            this.CheckBox12.UseVisualStyleBackColor = true;
            this.CheckBox12.CheckedChanged += new System.EventHandler(this.CheckBox12_CheckedChanged);
            // 
            // Button13
            // 
            this.Button13.Location = new System.Drawing.Point(526, 15);
            this.Button13.Name = "Button13";
            this.Button13.Size = new System.Drawing.Size(75, 23);
            this.Button13.TabIndex = 248;
            this.Button13.Text = "Pen color";
            this.Button13.UseVisualStyleBackColor = true;
            this.Button13.Click += new System.EventHandler(this.Button13_Click);
            // 
            // Button1
            // 
            this.Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button1.Location = new System.Drawing.Point(754, 15);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(75, 23);
            this.Button1.TabIndex = 8;
            this.Button1.Text = "OK";
            this.Button1.UseVisualStyleBackColor = true;
            // 
            // frmSavedPaths
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(927, 537);
            this.Controls.Add(this.Panel2);
            this.Controls.Add(this.HelplineRulerCtrl1);
            this.Controls.Add(this.Panel1);
            this.MaximizeBox = false;
            this.Name = "frmSavedPaths";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmSavedPaths";
            this.Load += new System.EventHandler(this.Form8_Load);
            this.Panel2.ResumeLayout(false);
            this.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).EndInit();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.OpenFileDialog OpenFileDialog2;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.SaveFileDialog SaveFileDialog2;
        internal System.Windows.Forms.Button Button9;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.CheckBox CheckBox3;
        internal System.Windows.Forms.Panel Panel2;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Button Button7;
        internal System.Windows.Forms.Button Button6;
        internal System.Windows.Forms.CheckBox CheckBox2;
        internal System.Windows.Forms.Button Button8;
        internal System.Windows.Forms.Button Button5;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.CheckBox CheckBox1;
        internal System.Windows.Forms.Button Button4;
        internal System.Windows.Forms.Button Button3;
        internal System.Windows.Forms.ListBox ListBox1;
        internal System.Windows.Forms.NumericUpDown NumericUpDown1;
        internal HelplineRulerControl.HelplineRulerCtrl HelplineRulerCtrl1;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.CheckBox CheckBox4;
        internal System.Windows.Forms.CheckBox CheckBox12;
        internal System.Windows.Forms.Button Button13;
        internal System.Windows.Forms.Button Button1;
    }
}