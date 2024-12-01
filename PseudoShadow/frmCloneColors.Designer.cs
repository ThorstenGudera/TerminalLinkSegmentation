namespace PseudoShadow
{
    partial class frmCloneColors
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
            if (this.FBitmap != null)
            {
                this.FBitmap.Dispose();
                this.FBitmap = null;
            }
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
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            btnClone = new Button();
            lblDestCol = new Label();
            lblSrcCol = new Label();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            btnRedo = new Button();
            cbBGColor = new CheckBox();
            numDestY = new NumericUpDown();
            numSrcY = new NumericUpDown();
            btnUndo = new Button();
            numDestX = new NumericUpDown();
            numSrcX = new NumericUpDown();
            label6 = new Label();
            button1 = new Button();
            button2 = new Button();
            label4 = new Label();
            label5 = new Label();
            label3 = new Label();
            cmbDest = new ComboBox();
            label2 = new Label();
            cmbSrc = new ComboBox();
            label1 = new Label();
            splitContainer2 = new SplitContainer();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            helplineRulerCtrl2 = new HelplineRulerControl.HelplineRulerCtrl();
            saveFileDialog1 = new SaveFileDialog();
            splitContainer4 = new SplitContainer();
            toolTip1 = new ToolTip(components);
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            btnCancel = new Button();
            btnOK = new Button();
            button3 = new Button();
            button4 = new Button();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDestY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSrcY).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDestX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numSrcX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer4).BeginInit();
            splitContainer4.Panel1.SuspendLayout();
            splitContainer4.Panel2.SuspendLayout();
            splitContainer4.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripStatusLabel3, toolStripStatusLabel4 });
            statusStrip1.Location = new Point(0, 821);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1233, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(118, 17);
            toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(118, 17);
            toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(118, 17);
            toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(118, 17);
            toolStripStatusLabel4.Text = "toolStripStatusLabel4";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(button3);
            splitContainer1.Panel1.Controls.Add(button4);
            splitContainer1.Panel1.Controls.Add(btnClone);
            splitContainer1.Panel1.Controls.Add(lblDestCol);
            splitContainer1.Panel1.Controls.Add(lblSrcCol);
            splitContainer1.Panel1.Controls.Add(Label20);
            splitContainer1.Panel1.Controls.Add(cmbZoom);
            splitContainer1.Panel1.Controls.Add(btnRedo);
            splitContainer1.Panel1.Controls.Add(cbBGColor);
            splitContainer1.Panel1.Controls.Add(numDestY);
            splitContainer1.Panel1.Controls.Add(numSrcY);
            splitContainer1.Panel1.Controls.Add(btnUndo);
            splitContainer1.Panel1.Controls.Add(numDestX);
            splitContainer1.Panel1.Controls.Add(numSrcX);
            splitContainer1.Panel1.Controls.Add(label6);
            splitContainer1.Panel1.Controls.Add(button1);
            splitContainer1.Panel1.Controls.Add(button2);
            splitContainer1.Panel1.Controls.Add(label4);
            splitContainer1.Panel1.Controls.Add(label5);
            splitContainer1.Panel1.Controls.Add(label3);
            splitContainer1.Panel1.Controls.Add(cmbDest);
            splitContainer1.Panel1.Controls.Add(label2);
            splitContainer1.Panel1.Controls.Add(cmbSrc);
            splitContainer1.Panel1.Controls.Add(label1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1233, 757);
            splitContainer1.SplitterDistance = 114;
            splitContainer1.TabIndex = 1;
            // 
            // btnClone
            // 
            btnClone.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnClone.Location = new Point(478, 23);
            btnClone.Name = "btnClone";
            btnClone.Size = new Size(88, 30);
            btnClone.TabIndex = 6;
            btnClone.Text = "CloneColors";
            btnClone.UseVisualStyleBackColor = true;
            btnClone.Click += btnClone_Click;
            // 
            // lblDestCol
            // 
            lblDestCol.BorderStyle = BorderStyle.FixedSingle;
            lblDestCol.Location = new Point(944, 8);
            lblDestCol.Name = "lblDestCol";
            lblDestCol.Size = new Size(100, 23);
            lblDestCol.TabIndex = 5;
            lblDestCol.Text = "    ";
            // 
            // lblSrcCol
            // 
            lblSrcCol.BorderStyle = BorderStyle.FixedSingle;
            lblSrcCol.Location = new Point(301, 7);
            lblSrcCol.Name = "lblSrcCol";
            lblSrcCol.Size = new Size(100, 23);
            lblSrcCol.TabIndex = 5;
            lblSrcCol.Text = "    ";
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(1071, 42);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 723;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(1133, 39);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 722;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // btnRedo
            // 
            btnRedo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRedo.Enabled = false;
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(740, 38);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 725;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // cbBGColor
            // 
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(1071, 11);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 721;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // numDestY
            // 
            numDestY.Location = new Point(866, 9);
            numDestY.Name = "numDestY";
            numDestY.Size = new Size(61, 23);
            numDestY.TabIndex = 4;
            numDestY.ValueChanged += numDestY_ValueChanged;
            // 
            // numSrcY
            // 
            numSrcY.Location = new Point(216, 7);
            numSrcY.Name = "numSrcY";
            numSrcY.Size = new Size(61, 23);
            numSrcY.TabIndex = 4;
            numSrcY.ValueChanged += numSrcY_ValueChanged;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUndo.Enabled = false;
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(644, 38);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 724;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // numDestX
            // 
            numDestX.Location = new Point(767, 9);
            numDestX.Name = "numDestX";
            numDestX.Size = new Size(61, 23);
            numDestX.TabIndex = 4;
            numDestX.ValueChanged += numDestX_ValueChanged;
            // 
            // numSrcX
            // 
            numSrcX.Location = new Point(121, 7);
            numSrcX.Name = "numSrcX";
            numSrcX.Size = new Size(61, 23);
            numSrcX.TabIndex = 4;
            numSrcX.ValueChanged += numSrcX_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(843, 12);
            label6.Name = "label6";
            label6.Size = new Size(17, 15);
            label6.TabIndex = 3;
            label6.Text = "Y:";
            // 
            // button1
            // 
            button1.FlatStyle = FlatStyle.System;
            button1.ForeColor = SystemColors.ControlText;
            button1.Location = new Point(956, 38);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(88, 27);
            button1.TabIndex = 719;
            button1.Text = "Save";
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(313, 37);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 719;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(193, 11);
            label4.Name = "label4";
            label4.Size = new Size(17, 15);
            label4.TabIndex = 3;
            label4.Text = "Y:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(744, 12);
            label5.Name = "label5";
            label5.Size = new Size(17, 15);
            label5.TabIndex = 3;
            label5.Text = "X:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(98, 11);
            label3.Name = "label3";
            label3.Size = new Size(17, 15);
            label3.TabIndex = 3;
            label3.Text = "X:";
            // 
            // cmbDest
            // 
            cmbDest.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDest.FormattingEnabled = true;
            cmbDest.Location = new Point(680, 8);
            cmbDest.Name = "cmbDest";
            cmbDest.Size = new Size(45, 23);
            cmbDest.TabIndex = 2;
            cmbDest.SelectedIndexChanged += cmbDest_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(644, 12);
            label2.Name = "label2";
            label2.Size = new Size(30, 15);
            label2.TabIndex = 1;
            label2.Text = "Dest";
            // 
            // cmbSrc
            // 
            cmbSrc.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSrc.FormattingEnabled = true;
            cmbSrc.Location = new Point(41, 8);
            cmbSrc.Name = "cmbSrc";
            cmbSrc.Size = new Size(45, 23);
            cmbSrc.TabIndex = 2;
            cmbSrc.SelectedIndexChanged += cmbSrc_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 11);
            label1.Name = "label1";
            label1.Size = new Size(23, 15);
            label1.TabIndex = 1;
            label1.Text = "Src";
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(helplineRulerCtrl1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(helplineRulerCtrl2);
            splitContainer2.Size = new Size(1233, 639);
            splitContainer2.SplitterDistance = 607;
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
            helplineRulerCtrl1.Margin = new Padding(4, 3, 4, 3);
            helplineRulerCtrl1.MoveHelpLinesOnResize = false;
            helplineRulerCtrl1.Name = "helplineRulerCtrl1";
            helplineRulerCtrl1.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl1.Size = new Size(607, 639);
            helplineRulerCtrl1.TabIndex = 0;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // helplineRulerCtrl2
            // 
            helplineRulerCtrl2.Bmp = null;
            helplineRulerCtrl2.Dock = DockStyle.Fill;
            helplineRulerCtrl2.DontDoLayout = false;
            helplineRulerCtrl2.DontHandleDoubleClick = false;
            helplineRulerCtrl2.DontPaintBaseImg = false;
            helplineRulerCtrl2.DontProcDoubleClick = false;
            helplineRulerCtrl2.DrawModeClipped = false;
            helplineRulerCtrl2.DrawPixelated = false;
            helplineRulerCtrl2.IgnoreZoom = false;
            helplineRulerCtrl2.Location = new Point(0, 0);
            helplineRulerCtrl2.Margin = new Padding(4, 3, 4, 3);
            helplineRulerCtrl2.MoveHelpLinesOnResize = false;
            helplineRulerCtrl2.Name = "helplineRulerCtrl2";
            helplineRulerCtrl2.SetZoomOnlyByMethodCall = false;
            helplineRulerCtrl2.Size = new Size(622, 639);
            helplineRulerCtrl2.TabIndex = 0;
            helplineRulerCtrl2.Zoom = 1F;
            helplineRulerCtrl2.ZoomSetManually = false;
            helplineRulerCtrl2.DBPanelDblClicked += helplineRulerCtrl2_DBPanelDblClicked;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // splitContainer4
            // 
            splitContainer4.Dock = DockStyle.Fill;
            splitContainer4.Location = new Point(0, 0);
            splitContainer4.Name = "splitContainer4";
            splitContainer4.Orientation = Orientation.Horizontal;
            // 
            // splitContainer4.Panel1
            // 
            splitContainer4.Panel1.Controls.Add(splitContainer1);
            // 
            // splitContainer4.Panel2
            // 
            splitContainer4.Panel2.Controls.Add(btnCancel);
            splitContainer4.Panel2.Controls.Add(btnOK);
            splitContainer4.Size = new Size(1233, 821);
            splitContainer4.SplitterDistance = 757;
            splitContainer4.TabIndex = 728;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(1136, 16);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 728;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(1043, 16);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 729;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.DialogResult = DialogResult.Cancel;
            button3.ForeColor = SystemColors.ControlText;
            button3.Location = new Point(1132, 81);
            button3.Margin = new Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new Size(88, 27);
            button3.TabIndex = 730;
            button3.Text = "Cancel";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.DialogResult = DialogResult.OK;
            button4.ForeColor = SystemColors.ControlText;
            button4.Location = new Point(1039, 81);
            button4.Margin = new Padding(4, 3, 4, 3);
            button4.Name = "button4";
            button4.Size = new Size(88, 27);
            button4.TabIndex = 731;
            button4.Text = "OK";
            button4.UseVisualStyleBackColor = true;
            button4.Click += btnOK_Click;
            // 
            // frmCloneColors
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1233, 843);
            Controls.Add(splitContainer4);
            Controls.Add(statusStrip1);
            Name = "frmCloneColors";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmCloneColors";
            FormClosing += frmCloneColors_FormClosing;
            Load += frmCloneColors_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)numDestY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSrcY).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDestX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numSrcX).EndInit();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            splitContainer4.Panel1.ResumeLayout(false);
            splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer4).EndInit();
            splitContainer4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private SplitContainer splitContainer1;
        private Label label4;
        private Label label3;
        private Label label2;
        private ComboBox cmbSrc;
        private Label label1;
        private SplitContainer splitContainer2;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl2;
        private Label lblDestCol;
        private Label lblSrcCol;
        private NumericUpDown numDestY;
        private NumericUpDown numSrcY;
        private NumericUpDown numDestX;
        private NumericUpDown numSrcX;
        private Label label6;
        private Label label5;
        private Button btnClone;
        private Button btnRedo;
        private Button btnUndo;
        internal Label Label20;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        internal Button button2;
        private SaveFileDialog saveFileDialog1;
        internal Button button1;
        private SplitContainer splitContainer4;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private ToolTip toolTip1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        internal ComboBox cmbDest;
        private Button button3;
        private Button button4;
        private Button btnCancel;
        private Button btnOK;
    }
}