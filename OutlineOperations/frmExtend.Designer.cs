using System.Drawing;
using System.Windows.Forms;

namespace OutlineOperations
{
    partial class frmExtend
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
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            helplineRulerCtrl1 = new HelplineRulerControl.HelplineRulerCtrl();
            btnRedo = new Button();
            btnUndo = new Button();
            groupBox2 = new GroupBox();
            cbErase = new CheckBox();
            numErase = new NumericUpDown();
            label16 = new Label();
            btnLoadBasePic = new Button();
            pictureBox1 = new PictureBox();
            Label20 = new Label();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            button10 = new Button();
            button8 = new Button();
            button2 = new Button();
            groupBox1 = new GroupBox();
            rbRepeated = new RadioButton();
            rbOnce = new RadioButton();
            cbDiskShaped = new CheckBox();
            cbMorphological = new CheckBox();
            cbSetOpaque = new CheckBox();
            numExtendOrShrink = new NumericUpDown();
            label1 = new Label();
            btnExtendOrShrink = new Button();
            btnCancel = new Button();
            btnOK = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            saveFileDialog1 = new SaveFileDialog();
            Timer3 = new System.Windows.Forms.Timer(components);
            toolTip1 = new ToolTip(components);
            openFileDialog1 = new OpenFileDialog();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numErase).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numExtendOrShrink).BeginInit();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel2, toolStripProgressBar1, toolStripStatusLabel4 });
            statusStrip1.Location = new Point(0, 800);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1200, 50);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(41, 45);
            toolStripStatusLabel1.Text = "    ";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.AutoSize = false;
            toolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(100, 45);
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(467, 44);
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Font = new Font("Segoe UI", 15.75F);
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(37, 45);
            toolStripStatusLabel4.Text = "    ";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(helplineRulerCtrl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnRedo);
            splitContainer1.Panel2.Controls.Add(btnUndo);
            splitContainer1.Panel2.Controls.Add(groupBox2);
            splitContainer1.Panel2.Controls.Add(label16);
            splitContainer1.Panel2.Controls.Add(btnLoadBasePic);
            splitContainer1.Panel2.Controls.Add(pictureBox1);
            splitContainer1.Panel2.Controls.Add(Label20);
            splitContainer1.Panel2.Controls.Add(cmbZoom);
            splitContainer1.Panel2.Controls.Add(cbBGColor);
            splitContainer1.Panel2.Controls.Add(button10);
            splitContainer1.Panel2.Controls.Add(button8);
            splitContainer1.Panel2.Controls.Add(button2);
            splitContainer1.Panel2.Controls.Add(groupBox1);
            splitContainer1.Panel2.Controls.Add(btnCancel);
            splitContainer1.Panel2.Controls.Add(btnOK);
            splitContainer1.Size = new Size(1200, 800);
            splitContainer1.SplitterDistance = 901;
            splitContainer1.TabIndex = 3;
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
            helplineRulerCtrl1.Size = new Size(901, 800);
            helplineRulerCtrl1.TabIndex = 0;
            helplineRulerCtrl1.Zoom = 1F;
            helplineRulerCtrl1.ZoomSetManually = false;
            helplineRulerCtrl1.DBPanelDblClicked += helplineRulerCtrl1_DBPanelDblClicked;
            // 
            // btnRedo
            // 
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(138, 475);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 668;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // btnUndo
            // 
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(34, 475);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 667;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cbErase);
            groupBox2.Controls.Add(numErase);
            groupBox2.Location = new Point(15, 400);
            groupBox2.Margin = new Padding(4, 3, 4, 3);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(4, 3, 4, 3);
            groupBox2.Size = new Size(266, 58);
            groupBox2.TabIndex = 666;
            groupBox2.TabStop = false;
            // 
            // cbErase
            // 
            cbErase.AutoSize = true;
            cbErase.Location = new Point(6, 22);
            cbErase.Margin = new Padding(4, 3, 4, 3);
            cbErase.Name = "cbErase";
            cbErase.Size = new Size(107, 19);
            cbErase.TabIndex = 653;
            cbErase.Text = "Erase w. Mouse";
            cbErase.UseVisualStyleBackColor = true;
            // 
            // numErase
            // 
            numErase.Location = new Point(131, 21);
            numErase.Margin = new Padding(4, 3, 4, 3);
            numErase.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numErase.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numErase.Name = "numErase";
            numErase.Size = new Size(88, 23);
            numErase.TabIndex = 652;
            numErase.Value = new decimal(new int[] { 25, 0, 0, 0 });
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(93, 663);
            label16.Margin = new Padding(4, 0, 4, 0);
            label16.Name = "label16";
            label16.Size = new Size(73, 15);
            label16.TabIndex = 665;
            label16.Text = "load orig pic";
            // 
            // btnLoadBasePic
            // 
            btnLoadBasePic.ForeColor = SystemColors.ControlText;
            btnLoadBasePic.Location = new Point(181, 658);
            btnLoadBasePic.Margin = new Padding(4, 3, 4, 3);
            btnLoadBasePic.Name = "btnLoadBasePic";
            btnLoadBasePic.Size = new Size(88, 27);
            btnLoadBasePic.TabIndex = 664;
            btnLoadBasePic.Text = "Load";
            btnLoadBasePic.UseVisualStyleBackColor = true;
            btnLoadBasePic.Click += btnLoadBasePic_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            pictureBox1.Location = new Point(15, 12);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(266, 229);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 663;
            pictureBox1.TabStop = false;
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(19, 523);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 662;
            Label20.Text = "Set Zoom";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(88, 520);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 661;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(211, 522);
            cbBGColor.Margin = new Padding(4, 3, 4, 3);
            cbBGColor.Name = "cbBGColor";
            cbBGColor.Size = new Size(67, 19);
            cbBGColor.TabIndex = 660;
            cbBGColor.Text = "BG dark";
            cbBGColor.UseVisualStyleBackColor = true;
            cbBGColor.CheckedChanged += cbBGColor_CheckedChanged;
            // 
            // button10
            // 
            button10.ForeColor = SystemColors.ControlText;
            button10.Location = new Point(181, 591);
            button10.Margin = new Padding(4, 3, 4, 3);
            button10.Name = "button10";
            button10.Size = new Size(88, 27);
            button10.TabIndex = 659;
            button10.Text = "HowTo";
            button10.UseVisualStyleBackColor = true;
            button10.Click += button10_Click;
            // 
            // button8
            // 
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(86, 591);
            button8.Margin = new Padding(4, 3, 4, 3);
            button8.Name = "button8";
            button8.Size = new Size(88, 27);
            button8.TabIndex = 658;
            button8.Text = "Reload";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // button2
            // 
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(181, 558);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 657;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbRepeated);
            groupBox1.Controls.Add(rbOnce);
            groupBox1.Controls.Add(cbDiskShaped);
            groupBox1.Controls.Add(cbMorphological);
            groupBox1.Controls.Add(cbSetOpaque);
            groupBox1.Controls.Add(numExtendOrShrink);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(btnExtendOrShrink);
            groupBox1.Location = new Point(15, 247);
            groupBox1.Margin = new Padding(4, 3, 4, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 3, 4, 3);
            groupBox1.Size = new Size(267, 147);
            groupBox1.TabIndex = 651;
            groupBox1.TabStop = false;
            // 
            // rbRepeated
            // 
            rbRepeated.AutoSize = true;
            rbRepeated.Location = new Point(190, 48);
            rbRepeated.Margin = new Padding(4, 3, 4, 3);
            rbRepeated.Name = "rbRepeated";
            rbRepeated.Size = new Size(71, 19);
            rbRepeated.TabIndex = 654;
            rbRepeated.Text = "repeated";
            rbRepeated.UseVisualStyleBackColor = true;
            // 
            // rbOnce
            // 
            rbOnce.AutoSize = true;
            rbOnce.Checked = true;
            rbOnce.Location = new Point(131, 48);
            rbOnce.Margin = new Padding(4, 3, 4, 3);
            rbOnce.Name = "rbOnce";
            rbOnce.Size = new Size(51, 19);
            rbOnce.TabIndex = 654;
            rbOnce.TabStop = true;
            rbOnce.Text = "once";
            rbOnce.UseVisualStyleBackColor = true;
            // 
            // cbDiskShaped
            // 
            cbDiskShaped.AutoSize = true;
            cbDiskShaped.Checked = true;
            cbDiskShaped.CheckState = CheckState.Checked;
            cbDiskShaped.Location = new Point(27, 76);
            cbDiskShaped.Margin = new Padding(4, 3, 4, 3);
            cbDiskShaped.Name = "cbDiskShaped";
            cbDiskShaped.Size = new Size(88, 19);
            cbDiskShaped.TabIndex = 653;
            cbDiskShaped.Text = "disk shaped";
            cbDiskShaped.UseVisualStyleBackColor = true;
            // 
            // cbMorphological
            // 
            cbMorphological.AutoSize = true;
            cbMorphological.Checked = true;
            cbMorphological.CheckState = CheckState.Checked;
            cbMorphological.Location = new Point(9, 50);
            cbMorphological.Margin = new Padding(4, 3, 4, 3);
            cbMorphological.Name = "cbMorphological";
            cbMorphological.Size = new Size(113, 19);
            cbMorphological.TabIndex = 653;
            cbMorphological.Text = "morphologically";
            cbMorphological.UseVisualStyleBackColor = true;
            // 
            // cbSetOpaque
            // 
            cbSetOpaque.AutoSize = true;
            cbSetOpaque.Checked = true;
            cbSetOpaque.CheckState = CheckState.Checked;
            cbSetOpaque.Location = new Point(65, 110);
            cbSetOpaque.Margin = new Padding(4, 3, 4, 3);
            cbSetOpaque.Name = "cbSetOpaque";
            cbSetOpaque.Size = new Size(84, 19);
            cbSetOpaque.TabIndex = 652;
            cbSetOpaque.Text = "set opaque";
            cbSetOpaque.UseVisualStyleBackColor = true;
            // 
            // numExtendOrShrink
            // 
            numExtendOrShrink.Location = new Point(117, 17);
            numExtendOrShrink.Margin = new Padding(4, 3, 4, 3);
            numExtendOrShrink.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numExtendOrShrink.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numExtendOrShrink.Name = "numExtendOrShrink";
            numExtendOrShrink.Size = new Size(88, 23);
            numExtendOrShrink.TabIndex = 651;
            numExtendOrShrink.Value = new decimal(new int[] { 14, 0, 0, int.MinValue });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 18);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(95, 15);
            label1.TabIndex = 649;
            label1.Text = "Extend Or Shrink";
            // 
            // btnExtendOrShrink
            // 
            btnExtendOrShrink.Location = new Point(172, 105);
            btnExtendOrShrink.Margin = new Padding(4, 3, 4, 3);
            btnExtendOrShrink.Name = "btnExtendOrShrink";
            btnExtendOrShrink.Size = new Size(88, 27);
            btnExtendOrShrink.TabIndex = 650;
            btnExtendOrShrink.Text = "Go";
            btnExtendOrShrink.UseVisualStyleBackColor = true;
            btnExtendOrShrink.Click += btnExtendOrShrink_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(198, 766);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 647;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(106, 766);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 648;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // Timer3
            // 
            Timer3.Interval = 500;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png";
            // 
            // frmExtend
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(1200, 850);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "frmExtend";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmExtend";
            FormClosing += frmExtend_FormClosing;
            Load += frmExtend_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numErase).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numExtendOrShrink).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripStatusLabel toolStripStatusLabel4;
        private SplitContainer splitContainer1;
        private HelplineRulerControl.HelplineRulerCtrl helplineRulerCtrl1;
        private Button btnCancel;
        private Button btnOK;
        private Label label1;
        private GroupBox groupBox1;
        private NumericUpDown numExtendOrShrink;
        private Button btnExtendOrShrink;
        private PictureBox pictureBox1;
        internal Label Label20;
        internal ComboBox cmbZoom;
        internal CheckBox cbBGColor;
        private Button button10;
        private Button button8;
        internal Button button2;
        internal System.ComponentModel.BackgroundWorker backgroundWorker1;
        private SaveFileDialog saveFileDialog1;
        internal System.Windows.Forms.Timer Timer3;
        private ToolTip toolTip1;
        private CheckBox cbSetOpaque;
        private Label label16;
        private Button btnLoadBasePic;
        private OpenFileDialog openFileDialog1;
        private GroupBox groupBox2;
        private CheckBox cbErase;
        private NumericUpDown numErase;
        private Button btnRedo;
        private Button btnUndo;
        private RadioButton rbRepeated;
        private RadioButton rbOnce;
        private CheckBox cbDiskShaped;
        private CheckBox cbMorphological;
    }
}