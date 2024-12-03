namespace AvoidAGrabCutEasy.ProcOutline
{
    partial class frmEditTrimap
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
            components = new System.ComponentModel.Container();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            label2 = new Label();
            label16 = new Label();
            btnLoadBasePic = new Button();
            numError = new NumericUpDown();
            label54 = new Label();
            cbClickDraw = new CheckBox();
            cbDraw = new CheckBox();
            label1 = new Label();
            cmbCurrentColor = new ComboBox();
            btnNew = new Button();
            btnRemStroke = new Button();
            label6 = new Label();
            numOpacity = new NumericUpDown();
            numWH = new NumericUpDown();
            cbImgOverlay = new CheckBox();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            button8 = new Button();
            button2 = new Button();
            btnCancel = new Button();
            btnOK = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            saveFileDialog1 = new SaveFileDialog();
            toolTip1 = new ToolTip(components);
            openFileDialog1 = new OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numError).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numOpacity).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numWH).BeginInit();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnCancel);
            splitContainer1.Panel2.Controls.Add(btnOK);
            splitContainer1.Panel2.Controls.Add(statusStrip1);
            splitContainer1.Size = new Size(1386, 981);
            splitContainer1.SplitterDistance = 862;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.FixedPanel = FixedPanel.Panel2;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Margin = new Padding(4, 3, 4, 3);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(helplineRulerCtrl1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(label2);
            splitContainer2.Panel2.Controls.Add(label16);
            splitContainer2.Panel2.Controls.Add(btnLoadBasePic);
            splitContainer2.Panel2.Controls.Add(numError);
            splitContainer2.Panel2.Controls.Add(label54);
            splitContainer2.Panel2.Controls.Add(cbClickDraw);
            splitContainer2.Panel2.Controls.Add(cbDraw);
            splitContainer2.Panel2.Controls.Add(label1);
            splitContainer2.Panel2.Controls.Add(cmbCurrentColor);
            splitContainer2.Panel2.Controls.Add(btnNew);
            splitContainer2.Panel2.Controls.Add(btnRemStroke);
            splitContainer2.Panel2.Controls.Add(label6);
            splitContainer2.Panel2.Controls.Add(numOpacity);
            splitContainer2.Panel2.Controls.Add(numWH);
            splitContainer2.Panel2.Controls.Add(cbImgOverlay);
            splitContainer2.Panel2.Controls.Add(Label20);
            splitContainer2.Panel2.Controls.Add(cmbZoom);
            splitContainer2.Panel2.Controls.Add(cbBGColor);
            splitContainer2.Panel2.Controls.Add(button8);
            splitContainer2.Panel2.Controls.Add(button2);
            splitContainer2.Size = new Size(1386, 862);
            splitContainer2.SplitterDistance = 1079;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 0;
            // 
            // helplineRulerCtrl1
            // 
            helplineRulerCtrl1.Bmp = null;
            helplineRulerCtrl1.Dock = DockStyle.Fill;
            helplineRulerCtrl1.DontDoLayout = false;
            helplineRulerCtrl1.DontHandleDoubleClick = false;
            helplineRulerCtrl1.DontPaintBaseImg = false;
            helplineRulerCtrl1.DontProcDoubleClick = false;
            helplineRulerCtrl1.DrawModeClipped = false;
            helplineRulerCtrl1.DrawPixelated = false;
            helplineRulerCtrl1.IgnoreZoom = false;
            helplineRulerCtrl1.Location = new Point(0, 0);
            helplineRulerCtrl1.Margin = new Padding(5, 3, 5, 3);
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(1079, 862);
            helplineRulerCtrl1.TabIndex = 0;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(133, 25);
            label2.Name = "label2";
            label2.Size = new Size(48, 15);
            label2.TabIndex = 670;
            label2.Text = "Opacity";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(34, 174);
            label16.Name = "label16";
            label16.Size = new Size(68, 15);
            label16.TabIndex = 669;
            label16.Text = "load trimap";
            // 
            // btnLoadBasePic
            // 
            btnLoadBasePic.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLoadBasePic.ForeColor = SystemColors.ControlText;
            btnLoadBasePic.Location = new Point(141, 168);
            btnLoadBasePic.Margin = new Padding(4, 3, 4, 3);
            btnLoadBasePic.Name = "btnLoadBasePic";
            btnLoadBasePic.Size = new Size(88, 27);
            btnLoadBasePic.TabIndex = 668;
            btnLoadBasePic.Text = "Load";
            btnLoadBasePic.UseVisualStyleBackColor = true;
            btnLoadBasePic.Click += btnLoadBasePic_Click;
            // 
            // numError
            // 
            numError.DecimalPlaces = 4;
            numError.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numError.Location = new Point(108, 683);
            numError.Margin = new Padding(4, 3, 4, 3);
            numError.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numError.Minimum = new decimal(new int[] { 1, 0, 0, 262144 });
            numError.Name = "numError";
            numError.Size = new Size(70, 23);
            numError.TabIndex = 666;
            numError.Value = new decimal(new int[] { 1, 0, 0, 131072 });
            numError.Visible = false;
            // 
            // label54
            // 
            label54.AutoSize = true;
            label54.Location = new Point(56, 685);
            label54.Margin = new Padding(4, 0, 4, 0);
            label54.Name = "label54";
            label54.Size = new Size(32, 15);
            label54.TabIndex = 667;
            label54.Text = "Error";
            label54.Visible = false;
            // 
            // cbClickDraw
            // 
            cbClickDraw.AutoSize = true;
            cbClickDraw.Location = new Point(105, 60);
            cbClickDraw.Margin = new Padding(4, 3, 4, 3);
            cbClickDraw.Name = "cbClickDraw";
            cbClickDraw.Size = new Size(79, 19);
            cbClickDraw.TabIndex = 665;
            cbClickDraw.Text = "ClickDraw";
            cbClickDraw.UseVisualStyleBackColor = true;
            cbClickDraw.CheckedChanged += cbClickDraw_CheckedChanged;
            // 
            // cbDraw
            // 
            cbDraw.AutoSize = true;
            cbDraw.Location = new Point(34, 60);
            cbDraw.Margin = new Padding(4, 3, 4, 3);
            cbDraw.Name = "cbDraw";
            cbDraw.Size = new Size(52, 19);
            cbDraw.TabIndex = 665;
            cbDraw.Text = "draw";
            cbDraw.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.Location = new Point(162, 84);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(58, 27);
            label1.TabIndex = 664;
            label1.Text = "    ";
            // 
            // cmbCurrentColor
            // 
            cmbCurrentColor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCurrentColor.FormattingEnabled = true;
            cmbCurrentColor.Items.AddRange(new object[] { "Background", "Foreground", "Unknown" });
            cmbCurrentColor.Location = new Point(34, 87);
            cmbCurrentColor.Margin = new Padding(4, 3, 4, 3);
            cmbCurrentColor.Name = "cmbCurrentColor";
            cmbCurrentColor.Size = new Size(117, 23);
            cmbCurrentColor.TabIndex = 663;
            cmbCurrentColor.SelectedIndexChanged += cmbCurrentColor_SelectedIndexChanged;
            // 
            // btnNew
            // 
            btnNew.Enabled = false;
            btnNew.Location = new Point(188, 55);
            btnNew.Margin = new Padding(4, 3, 4, 3);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(52, 27);
            btnNew.TabIndex = 662;
            btnNew.Text = "new ";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnNew_Click;
            // 
            // btnRemStroke
            // 
            btnRemStroke.Location = new Point(141, 123);
            btnRemStroke.Margin = new Padding(4, 3, 4, 3);
            btnRemStroke.Name = "btnRemStroke";
            btnRemStroke.Size = new Size(88, 27);
            btnRemStroke.TabIndex = 662;
            btnRemStroke.Text = "rem last";
            btnRemStroke.UseVisualStyleBackColor = true;
            btnRemStroke.Click += btnRemStroke_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(30, 129);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(37, 15);
            label6.TabIndex = 661;
            label6.Text = "width";
            // 
            // numOpacity
            // 
            numOpacity.DecimalPlaces = 2;
            numOpacity.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numOpacity.Location = new Point(188, 23);
            numOpacity.Margin = new Padding(4, 3, 4, 3);
            numOpacity.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            numOpacity.Name = "numOpacity";
            numOpacity.Size = new Size(52, 23);
            numOpacity.TabIndex = 660;
            numOpacity.Value = new decimal(new int[] { 2, 0, 0, 65536 });
            numOpacity.ValueChanged += numOpacity_ValueChanged;
            // 
            // numWH
            // 
            numWH.Location = new Point(75, 127);
            numWH.Margin = new Padding(4, 3, 4, 3);
            numWH.Maximum = new decimal(new int[] { 127, 0, 0, 0 });
            numWH.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numWH.Name = "numWH";
            numWH.Size = new Size(52, 23);
            numWH.TabIndex = 660;
            numWH.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // cbImgOverlay
            // 
            cbImgOverlay.AutoSize = true;
            cbImgOverlay.Location = new Point(19, 24);
            cbImgOverlay.Margin = new Padding(4, 3, 4, 3);
            cbImgOverlay.Name = "cbImgOverlay";
            cbImgOverlay.Size = new Size(107, 19);
            cbImgOverlay.TabIndex = 656;
            cbImgOverlay.Text = "overlay orig pic";
            cbImgOverlay.UseVisualStyleBackColor = true;
            cbImgOverlay.CheckedChanged += cbImgOverlay_CheckedChanged;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(54, 823);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 655;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(122, 819);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 654;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(65, 740);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 653;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // button8
            // 
            button8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(56, 775);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 652;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(150, 735);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 651;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(1284, 25);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 656;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(1191, 25);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 657;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripProgressBar1, toolStripStatusLabel4 });
            statusStrip1.Location = new Point(0, 70);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1386, 44);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(41, 39);
            toolStripStatusLabel1.Text = "    ";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.AutoSize = false;
            toolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(100, 39);
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(467, 38);
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Font = new Font("Segoe UI", 15.75F);
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(37, 39);
            toolStripStatusLabel4.Text = "    ";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png";
            // 
            // frmEditTrimap
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1386, 981);
            Controls.Add(splitContainer1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "frmEditTrimap";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmEditTrimap";
            Load += frmEditTrimap_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numError).EndInit();
            ((System.ComponentModel.ISupportInitialize)numOpacity).EndInit();
            ((System.ComponentModel.ISupportInitialize)numWH).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        internal System.Windows.Forms.Label Label20;
        internal System.Windows.Forms.ComboBox cmbZoom;
        internal System.Windows.Forms.CheckBox cbBGColor;
        private System.Windows.Forms.Button button8;
        internal System.Windows.Forms.Button button2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox cbImgOverlay;
        private System.Windows.Forms.Button btnRemStroke;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numWH;
        private System.Windows.Forms.ComboBox cmbCurrentColor;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbDraw;
        internal NumericUpDown numError;
        internal Label label54;
        private Label label16;
        private Button btnLoadBasePic;
        private OpenFileDialog openFileDialog1;
        private Label label2;
        private NumericUpDown numOpacity;
        private CheckBox cbClickDraw;
        private Button btnNew;
    }
}