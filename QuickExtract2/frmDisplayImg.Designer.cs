namespace QuickExtract2
{
    partial class frmDisplayImg
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
            this.HelplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            this.Label3 = new System.Windows.Forms.Label();
            this.Button1 = new System.Windows.Forms.Button();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.CheckBox12 = new System.Windows.Forms.CheckBox();
            this.Button13 = new System.Windows.Forms.Button();
            this.NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.Button2 = new System.Windows.Forms.Button();
            this.NumericUpDown7 = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown6 = new System.Windows.Forms.NumericUpDown();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.NumericUpDown5 = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown4 = new System.Windows.Forms.NumericUpDown();
            this.NumericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.Label4 = new System.Windows.Forms.Label();
            this.SaveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.ColorDialog1 = new System.Windows.Forms.ColorDialog();
            this.OpenFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown2)).BeginInit();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // HelplineRulerCtrl1
            // 
            this.HelplineRulerCtrl1.Bmp = null;
            this.HelplineRulerCtrl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.HelplineRulerCtrl1.DontDoLayout = false;
            this.HelplineRulerCtrl1.DontHandleDoubleClick = false;
            this.HelplineRulerCtrl1.DontPaintBaseImg = false;
            this.HelplineRulerCtrl1.DontProcDoubleClick = false;
            this.HelplineRulerCtrl1.IgnoreZoom = false;
            this.HelplineRulerCtrl1.Location = new System.Drawing.Point(0, 0);
            this.HelplineRulerCtrl1.MoveHelpLinesOnResize = false;
            this.HelplineRulerCtrl1.Name = "HelplineRulerCtrl1";
            this.HelplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            this.HelplineRulerCtrl1.Size = new System.Drawing.Size(731, 491);
            this.HelplineRulerCtrl1.TabIndex = 18;
            this.HelplineRulerCtrl1.Zoom = 1F;
            this.HelplineRulerCtrl1.ZoomSetManually = false;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(514, 20);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(57, 13);
            this.Label3.TabIndex = 247;
            this.Label3.Text = "Pen width:";
            // 
            // Button1
            // 
            this.Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Button1.Location = new System.Drawing.Point(558, 15);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(75, 23);
            this.Button1.TabIndex = 8;
            this.Button1.Text = "OK";
            this.Button1.UseVisualStyleBackColor = true;
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.CheckBox12);
            this.Panel1.Controls.Add(this.Button13);
            this.Panel1.Controls.Add(this.Label3);
            this.Panel1.Controls.Add(this.NumericUpDown1);
            this.Panel1.Controls.Add(this.Button1);
            this.Panel1.Controls.Add(this.Button2);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Panel1.Location = new System.Drawing.Point(0, 491);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(731, 50);
            this.Panel1.TabIndex = 16;
            // 
            // CheckBox12
            // 
            this.CheckBox12.AutoSize = true;
            this.CheckBox12.Checked = true;
            this.CheckBox12.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBox12.Location = new System.Drawing.Point(427, 18);
            this.CheckBox12.Name = "CheckBox12";
            this.CheckBox12.Size = new System.Drawing.Size(65, 17);
            this.CheckBox12.TabIndex = 249;
            this.CheckBox12.Text = "BG dark";
            this.CheckBox12.UseVisualStyleBackColor = true;
            this.CheckBox12.CheckedChanged += new System.EventHandler(this.CheckBox12_CheckedChanged);
            // 
            // Button13
            // 
            this.Button13.Location = new System.Drawing.Point(634, 15);
            this.Button13.Name = "Button13";
            this.Button13.Size = new System.Drawing.Size(75, 23);
            this.Button13.TabIndex = 248;
            this.Button13.Text = "Pen color";
            this.Button13.UseVisualStyleBackColor = true;
            this.Button13.Click += new System.EventHandler(this.Button13_Click);
            // 
            // NumericUpDown1
            // 
            this.NumericUpDown1.DecimalPlaces = 2;
            this.NumericUpDown1.Location = new System.Drawing.Point(574, 17);
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
            // Button2
            // 
            this.Button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Button2.Location = new System.Drawing.Point(639, 15);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(75, 23);
            this.Button2.TabIndex = 9;
            this.Button2.Text = "Cancel";
            this.Button2.UseVisualStyleBackColor = true;
            // 
            // NumericUpDown7
            // 
            this.NumericUpDown7.DecimalPlaces = 2;
            this.NumericUpDown7.Location = new System.Drawing.Point(92, 429);
            this.NumericUpDown7.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.NumericUpDown7.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.NumericUpDown7.Name = "NumericUpDown7";
            this.NumericUpDown7.Size = new System.Drawing.Size(75, 20);
            this.NumericUpDown7.TabIndex = 1;
            this.NumericUpDown7.ValueChanged += new System.EventHandler(this.NumericUpDown6_ValueChanged);
            // 
            // NumericUpDown6
            // 
            this.NumericUpDown6.DecimalPlaces = 2;
            this.NumericUpDown6.Location = new System.Drawing.Point(92, 397);
            this.NumericUpDown6.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.NumericUpDown6.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.NumericUpDown6.Name = "NumericUpDown6";
            this.NumericUpDown6.Size = new System.Drawing.Size(75, 20);
            this.NumericUpDown6.TabIndex = 1;
            this.NumericUpDown6.ValueChanged += new System.EventHandler(this.NumericUpDown6_ValueChanged);
            // 
            // Label7
            // 
            this.Label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(18, 289);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(149, 89);
            this.Label7.TabIndex = 2;
            this.Label7.Text = "  ";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label6
            // 
            this.Label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(18, 182);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(149, 89);
            this.Label6.TabIndex = 2;
            this.Label6.Text = "  ";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NumericUpDown5
            // 
            this.NumericUpDown5.DecimalPlaces = 2;
            this.NumericUpDown5.Location = new System.Drawing.Point(92, 138);
            this.NumericUpDown5.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.NumericUpDown5.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.NumericUpDown5.Name = "NumericUpDown5";
            this.NumericUpDown5.Size = new System.Drawing.Size(75, 20);
            this.NumericUpDown5.TabIndex = 1;
            this.NumericUpDown5.ValueChanged += new System.EventHandler(this.NumericUpDown2_ValueChanged);
            // 
            // NumericUpDown3
            // 
            this.NumericUpDown3.DecimalPlaces = 7;
            this.NumericUpDown3.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NumericUpDown3.Location = new System.Drawing.Point(92, 43);
            this.NumericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.NumericUpDown3.Name = "NumericUpDown3";
            this.NumericUpDown3.Size = new System.Drawing.Size(75, 20);
            this.NumericUpDown3.TabIndex = 1;
            this.NumericUpDown3.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown3.ValueChanged += new System.EventHandler(this.NumericUpDown2_ValueChanged);
            // 
            // NumericUpDown4
            // 
            this.NumericUpDown4.DecimalPlaces = 2;
            this.NumericUpDown4.Location = new System.Drawing.Point(92, 106);
            this.NumericUpDown4.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
            this.NumericUpDown4.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
            this.NumericUpDown4.Name = "NumericUpDown4";
            this.NumericUpDown4.Size = new System.Drawing.Size(75, 20);
            this.NumericUpDown4.TabIndex = 1;
            this.NumericUpDown4.ValueChanged += new System.EventHandler(this.NumericUpDown2_ValueChanged);
            // 
            // NumericUpDown2
            // 
            this.NumericUpDown2.DecimalPlaces = 7;
            this.NumericUpDown2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.NumericUpDown2.Location = new System.Drawing.Point(92, 11);
            this.NumericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.NumericUpDown2.Name = "NumericUpDown2";
            this.NumericUpDown2.Size = new System.Drawing.Size(75, 20);
            this.NumericUpDown2.TabIndex = 1;
            this.NumericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NumericUpDown2.ValueChanged += new System.EventHandler(this.NumericUpDown2_ValueChanged);
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Location = new System.Drawing.Point(18, 431);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(81, 13);
            this.Label9.TabIndex = 0;
            this.Label9.Text = "desired Height: ";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(18, 399);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(78, 13);
            this.Label8.TabIndex = 0;
            this.Label8.Text = "desired Width: ";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(18, 140);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(41, 13);
            this.Label5.TabIndex = 0;
            this.Label5.Text = "ShiftY: ";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(18, 45);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(47, 13);
            this.Label2.TabIndex = 0;
            this.Label2.Text = "ScaleY: ";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(18, 13);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(47, 13);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "ScaleX: ";
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.NumericUpDown7);
            this.Panel2.Controls.Add(this.NumericUpDown6);
            this.Panel2.Controls.Add(this.Label7);
            this.Panel2.Controls.Add(this.Label6);
            this.Panel2.Controls.Add(this.NumericUpDown5);
            this.Panel2.Controls.Add(this.NumericUpDown3);
            this.Panel2.Controls.Add(this.NumericUpDown4);
            this.Panel2.Controls.Add(this.NumericUpDown2);
            this.Panel2.Controls.Add(this.Label9);
            this.Panel2.Controls.Add(this.Label8);
            this.Panel2.Controls.Add(this.Label5);
            this.Panel2.Controls.Add(this.Label4);
            this.Panel2.Controls.Add(this.Label2);
            this.Panel2.Controls.Add(this.Label1);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.Panel2.Location = new System.Drawing.Point(731, 0);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(200, 541);
            this.Panel2.TabIndex = 17;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(18, 108);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(41, 13);
            this.Label4.TabIndex = 0;
            this.Label4.Text = "ShiftX: ";
            // 
            // SaveFileDialog2
            // 
            this.SaveFileDialog2.DefaultExt = "tggrp";
            this.SaveFileDialog2.FileName = "File1.tggrp";
            this.SaveFileDialog2.Filter = "GraphicsPath files (*.tggrp)|*.tggrp";
            this.SaveFileDialog2.RestoreDirectory = true;
            // 
            // ColorDialog1
            // 
            this.ColorDialog1.AnyColor = true;
            this.ColorDialog1.FullOpen = true;
            // 
            // OpenFileDialog2
            // 
            this.OpenFileDialog2.FileName = "File1";
            this.OpenFileDialog2.Filter = "GraphicsPath-Files (*.tggrp)|*.tggrp";
            // 
            // frmDisplayImg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 541);
            this.Controls.Add(this.HelplineRulerCtrl1);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Panel2);
            this.Name = "frmDisplayImg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "frmDisplayImg";
            this.Load += new System.EventHandler(this.Form8_Load);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NumericUpDown2)).EndInit();
            this.Panel2.ResumeLayout(false);
            this.Panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal HelplineRulerControl.HelplineRulerCtrl HelplineRulerCtrl1;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.CheckBox CheckBox12;
        internal System.Windows.Forms.Button Button13;
        internal System.Windows.Forms.NumericUpDown NumericUpDown1;
        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.NumericUpDown NumericUpDown7;
        internal System.Windows.Forms.NumericUpDown NumericUpDown6;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.NumericUpDown NumericUpDown5;
        internal System.Windows.Forms.NumericUpDown NumericUpDown3;
        internal System.Windows.Forms.NumericUpDown NumericUpDown4;
        internal System.Windows.Forms.NumericUpDown NumericUpDown2;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Panel Panel2;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.SaveFileDialog SaveFileDialog2;
        internal System.Windows.Forms.ColorDialog ColorDialog1;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialog2;
    }
}