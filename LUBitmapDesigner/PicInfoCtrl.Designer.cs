namespace LUBitmapDesigner
{
    partial class PicInfoCtrl
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            GroupBox1 = new GroupBox();
            cmbMergeOP = new ComboBox();
            label5 = new Label();
            cbIntVals = new CheckBox();
            Button3 = new Button();
            Button1 = new Button();
            Label1 = new Label();
            cbLock = new CheckBox();
            Label2 = new Label();
            Label3 = new Label();
            btnOrigSize = new Button();
            Label4 = new Label();
            cbAspect = new CheckBox();
            numX = new NumericUpDown();
            numH = new NumericUpDown();
            numW = new NumericUpDown();
            numY = new NumericUpDown();
            numRot = new NumericUpDown();
            numOpacity = new NumericUpDown();
            GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numH).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numW).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOpacity).BeginInit();
            SuspendLayout();
            // 
            // GroupBox1
            // 
            GroupBox1.Controls.Add(cmbMergeOP);
            GroupBox1.Controls.Add(label5);
            GroupBox1.Controls.Add(cbIntVals);
            GroupBox1.Controls.Add(Button3);
            GroupBox1.Controls.Add(Button1);
            GroupBox1.Controls.Add(Label1);
            GroupBox1.Controls.Add(cbLock);
            GroupBox1.Controls.Add(Label2);
            GroupBox1.Controls.Add(Label3);
            GroupBox1.Controls.Add(btnOrigSize);
            GroupBox1.Controls.Add(Label4);
            GroupBox1.Controls.Add(cbAspect);
            GroupBox1.Controls.Add(numX);
            GroupBox1.Controls.Add(numH);
            GroupBox1.Controls.Add(numW);
            GroupBox1.Controls.Add(numY);
            GroupBox1.Controls.Add(numRot);
            GroupBox1.Controls.Add(numOpacity);
            GroupBox1.Location = new Point(15, -1);
            GroupBox1.Margin = new Padding(4, 3, 4, 3);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Padding = new Padding(4, 3, 4, 3);
            GroupBox1.Size = new Size(275, 294);
            GroupBox1.TabIndex = 38;
            GroupBox1.TabStop = false;
            GroupBox1.Text = "Settings";
            // 
            // cmbMergeOP
            // 
            cmbMergeOP.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMergeOP.DropDownWidth = 224;
            cmbMergeOP.FormattingEnabled = true;
            cmbMergeOP.Location = new Point(135, 263);
            cmbMergeOP.Name = "cmbMergeOP";
            cmbMergeOP.Size = new Size(121, 23);
            cmbMergeOP.TabIndex = 47;
            cmbMergeOP.SelectedIndexChanged += cmbMergeOP_SelectedIndexChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(53, 266);
            label5.Name = "label5";
            label5.Size = new Size(57, 15);
            label5.TabIndex = 46;
            label5.Text = "MergeOP";
            // 
            // cbIntVals
            // 
            cbIntVals.AutoSize = true;
            cbIntVals.Location = new Point(141, 123);
            cbIntVals.Margin = new Padding(4, 3, 4, 3);
            cbIntVals.Name = "cbIntVals";
            cbIntVals.Size = new Size(63, 19);
            cbIntVals.TabIndex = 43;
            cbIntVals.Text = "Int vals";
            cbIntVals.UseVisualStyleBackColor = true;
            cbIntVals.CheckedChanged += cbIntVals_CheckedChanged;
            // 
            // Button3
            // 
            Button3.Location = new Point(20, 404);
            Button3.Margin = new Padding(4, 3, 4, 3);
            Button3.Name = "Button3";
            Button3.Size = new Size(114, 27);
            Button3.TabIndex = 36;
            Button3.Text = "PasteSettings";
            Button3.UseVisualStyleBackColor = true;
            // 
            // Button1
            // 
            Button1.Location = new Point(156, 417);
            Button1.Margin = new Padding(4, 3, 4, 3);
            Button1.Name = "Button1";
            Button1.Size = new Size(88, 27);
            Button1.TabIndex = 36;
            Button1.Text = "Set";
            Button1.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            Label1.AutoSize = true;
            Label1.Location = new Point(19, 18);
            Label1.Margin = new Padding(4, 0, 4, 0);
            Label1.Name = "Label1";
            Label1.Size = new Size(56, 15);
            Label1.TabIndex = 23;
            Label1.Text = "Location:";
            // 
            // cbLock
            // 
            cbLock.AutoSize = true;
            cbLock.Location = new Point(20, 152);
            cbLock.Margin = new Padding(4, 3, 4, 3);
            cbLock.Name = "cbLock";
            cbLock.Size = new Size(51, 19);
            cbLock.TabIndex = 33;
            cbLock.Text = "Lock";
            cbLock.UseVisualStyleBackColor = true;
            cbLock.CheckedChanged += cbLock_CheckedChanged;
            // 
            // Label2
            // 
            Label2.AutoSize = true;
            Label2.Location = new Point(20, 71);
            Label2.Margin = new Padding(4, 0, 4, 0);
            Label2.Name = "Label2";
            Label2.Size = new Size(30, 15);
            Label2.TabIndex = 23;
            Label2.Text = "Size:";
            // 
            // Label3
            // 
            Label3.AutoSize = true;
            Label3.Location = new Point(20, 206);
            Label3.Margin = new Padding(4, 0, 4, 0);
            Label3.Name = "Label3";
            Label3.Size = new Size(55, 15);
            Label3.TabIndex = 23;
            Label3.Text = "Rotation:";
            // 
            // btnOrigSize
            // 
            btnOrigSize.Location = new Point(21, 174);
            btnOrigSize.Margin = new Padding(4, 3, 4, 3);
            btnOrigSize.Name = "btnOrigSize";
            btnOrigSize.Size = new Size(88, 27);
            btnOrigSize.TabIndex = 31;
            btnOrigSize.Text = "OrigSize";
            btnOrigSize.UseVisualStyleBackColor = true;
            btnOrigSize.Click += btnOrigSize_Click;
            // 
            // Label4
            // 
            Label4.AutoSize = true;
            Label4.Location = new Point(138, 206);
            Label4.Margin = new Padding(4, 0, 4, 0);
            Label4.Name = "Label4";
            Label4.Size = new Size(51, 15);
            Label4.TabIndex = 23;
            Label4.Text = "Opacity:";
            // 
            // cbAspect
            // 
            cbAspect.AutoSize = true;
            cbAspect.Checked = true;
            cbAspect.CheckState = CheckState.Checked;
            cbAspect.Location = new Point(21, 123);
            cbAspect.Margin = new Padding(4, 3, 4, 3);
            cbAspect.Name = "cbAspect";
            cbAspect.Size = new Size(88, 19);
            cbAspect.TabIndex = 32;
            cbAspect.Text = "keep aspect";
            cbAspect.UseVisualStyleBackColor = true;
            // 
            // numX
            // 
            numX.DecimalPlaces = 2;
            numX.Location = new Point(20, 40);
            numX.Margin = new Padding(4, 3, 4, 3);
            numX.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numX.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numX.Name = "numX";
            numX.Size = new Size(86, 23);
            numX.TabIndex = 30;
            numX.ValueChanged += numX_ValueChanged;
            // 
            // numH
            // 
            numH.DecimalPlaces = 2;
            numH.Location = new Point(141, 93);
            numH.Margin = new Padding(4, 3, 4, 3);
            numH.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numH.Name = "numH";
            numH.Size = new Size(86, 23);
            numH.TabIndex = 24;
            numH.ValueChanged += numH_ValueChanged;
            // 
            // numW
            // 
            numW.DecimalPlaces = 2;
            numW.Location = new Point(21, 93);
            numW.Margin = new Padding(4, 3, 4, 3);
            numW.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numW.Name = "numW";
            numW.Size = new Size(86, 23);
            numW.TabIndex = 29;
            numW.ValueChanged += numW_ValueChanged;
            // 
            // numY
            // 
            numY.DecimalPlaces = 2;
            numY.Location = new Point(140, 40);
            numY.Margin = new Padding(4, 3, 4, 3);
            numY.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numY.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numY.Name = "numY";
            numY.Size = new Size(86, 23);
            numY.TabIndex = 25;
            numY.ValueChanged += numY_ValueChanged;
            // 
            // numRot
            // 
            numRot.DecimalPlaces = 2;
            numRot.Location = new Point(24, 228);
            numRot.Margin = new Padding(4, 3, 4, 3);
            numRot.Maximum = new decimal(new int[] { 360, 0, 0, 0 });
            numRot.Minimum = new decimal(new int[] { 360, 0, 0, int.MinValue });
            numRot.Name = "numRot";
            numRot.Size = new Size(86, 23);
            numRot.TabIndex = 28;
            numRot.ValueChanged += numRot_ValueChanged;
            // 
            // numOpacity
            // 
            numOpacity.DecimalPlaces = 2;
            numOpacity.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numOpacity.Location = new Point(141, 228);
            numOpacity.Margin = new Padding(4, 3, 4, 3);
            numOpacity.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numOpacity.Name = "numOpacity";
            numOpacity.Size = new Size(86, 23);
            numOpacity.TabIndex = 27;
            numOpacity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numOpacity.ValueChanged += numOpacity_ValueChanged;
            // 
            // PicInfoCtrl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(GroupBox1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "PicInfoCtrl";
            Size = new Size(304, 300);
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numH).EndInit();
            ((System.ComponentModel.ISupportInitialize)numW).EndInit();
            ((System.ComponentModel.ISupportInitialize)numY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRot).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOpacity).EndInit();
            ResumeLayout(false);
        }

        #endregion

        internal System.Windows.Forms.GroupBox GroupBox1;
        public System.Windows.Forms.Button Button3;
        public System.Windows.Forms.Button Button1;
        public System.Windows.Forms.Label Label1;
        public System.Windows.Forms.CheckBox cbLock;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.Label Label3;
        public System.Windows.Forms.Button btnOrigSize;
        public System.Windows.Forms.Label Label4;
        public System.Windows.Forms.CheckBox cbAspect;
        public System.Windows.Forms.NumericUpDown numX;
        public System.Windows.Forms.NumericUpDown numH;
        public System.Windows.Forms.NumericUpDown numW;
        public System.Windows.Forms.NumericUpDown numY;
        public System.Windows.Forms.NumericUpDown numRot;
        public System.Windows.Forms.NumericUpDown numOpacity;
        public CheckBox cbIntVals;
        private Label label5;
        public ComboBox cmbMergeOP;
    }
}
