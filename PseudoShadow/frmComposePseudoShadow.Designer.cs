using System.Drawing.Printing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PseudoShadow
{
    partial class frmComposePseudoShadow
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
            if (this._bmpBU != null)
            {
                this._bmpBU.Dispose();
                this._bmpBU = null;
            }
            if (this._bmpBGOrig != null)
            {
                this._bmpBGOrig.Dispose();
                this._bmpBGOrig = null;
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
            toolTip1 = new ToolTip(components);
            toolStripStatusLabel4 = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            Label20 = new Label();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            btnOK = new Button();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel3 = new ToolStripStatusLabel();
            cmbZoom = new ComboBox();
            cbBGColor = new CheckBox();
            button8 = new Button();
            button2 = new Button();
            saveFileDialog1 = new SaveFileDialog();
            splitContainer2 = new SplitContainer();
            luBitmapDesignerCtrl1 = new LUBitmapDesigner.LUBitmapDesignerCtrl();
            cbExcludeFG = new CheckBox();
            btnFromCache = new Button();
            cbExcludeRegions = new CheckBox();
            btnCloneColors = new Button();
            label14 = new Label();
            panel3 = new Panel();
            cbAuto = new CheckBox();
            label13 = new Label();
            btnShear = new Button();
            label12 = new Label();
            label11 = new Label();
            label9 = new Label();
            label10 = new Label();
            label8 = new Label();
            numericUpDown6 = new NumericUpDown();
            numericUpDown4 = new NumericUpDown();
            numericUpDown5 = new NumericUpDown();
            numericUpDown2 = new NumericUpDown();
            numericUpDown3 = new NumericUpDown();
            numericUpDown1 = new NumericUpDown();
            panel2 = new Panel();
            label2 = new Label();
            btnGaussian = new Button();
            panel1 = new Panel();
            label7 = new Label();
            label3 = new Label();
            btnFloodfill = new Button();
            btnColor = new Button();
            numTolerance = new NumericUpDown();
            btnRedo = new Button();
            btnUndo = new Button();
            label6 = new Label();
            label5 = new Label();
            btnAlphaZAndGain = new Button();
            btnSetGamma = new Button();
            label4 = new Label();
            numGamma = new NumericUpDown();
            numAlphaZAndGain = new NumericUpDown();
            picInfoCtrl1 = new LUBitmapDesigner.PicInfoCtrl();
            btnLoad = new Button();
            label1 = new Label();
            btnClone = new Button();
            btnSwap = new Button();
            btnLoadUpper = new Button();
            btnMerge = new Button();
            btnRemove = new Button();
            btnCancel = new Button();
            splitContainer1 = new SplitContainer();
            openFileDialog1 = new OpenFileDialog();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
            colorDialog1 = new ColorDialog();
            backgroundWorker3 = new System.ComponentModel.BackgroundWorker();
            backgroundWorker4 = new System.ComponentModel.BackgroundWorker();
            openFileDialog2 = new OpenFileDialog();
            backgroundWorker6 = new System.ComponentModel.BackgroundWorker();
            label22 = new Label();
            btnAlphaCurve = new Button();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown6).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numTolerance).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numGamma).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numAlphaZAndGain).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripStatusLabel4
            // 
            toolStripStatusLabel4.Font = new Font("Segoe UI", 15.75F);
            toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            toolStripStatusLabel4.Size = new Size(37, 40);
            toolStripStatusLabel4.Text = "    ";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(467, 39);
            // 
            // Label20
            // 
            Label20.AutoSize = true;
            Label20.Location = new Point(82, 853);
            Label20.Margin = new Padding(4, 0, 4, 0);
            Label20.Name = "Label20";
            Label20.Size = new Size(58, 15);
            Label20.TabIndex = 655;
            Label20.Text = "Set Zoom";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.AutoSize = false;
            toolStripStatusLabel2.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel2.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(100, 40);
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Top | ToolStripStatusLabelBorderSides.Right | ToolStripStatusLabelBorderSides.Bottom;
            toolStripStatusLabel1.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(41, 40);
            toolStripStatusLabel1.Text = "    ";
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(1291, 8);
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
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusLabel3, toolStripStatusLabel2, toolStripProgressBar1, toolStripStatusLabel4 });
            statusStrip1.Location = new Point(0, 49);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 16, 0);
            statusStrip1.Size = new Size(1486, 45);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel3
            // 
            toolStripStatusLabel3.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            toolStripStatusLabel3.Size = new Size(37, 40);
            toolStripStatusLabel3.Text = "    ";
            // 
            // cmbZoom
            // 
            cmbZoom.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZoom.FormattingEnabled = true;
            cmbZoom.Items.AddRange(new object[] { "4", "2", "1", "Fit_Width", "Fit" });
            cmbZoom.Location = new Point(144, 850);
            cmbZoom.Margin = new Padding(4, 3, 4, 3);
            cmbZoom.Name = "cmbZoom";
            cmbZoom.Size = new Size(87, 23);
            cmbZoom.TabIndex = 654;
            cmbZoom.SelectedIndexChanged += cmbZoom_SelectedIndexChanged;
            // 
            // cbBGColor
            // 
            cbBGColor.AutoSize = true;
            cbBGColor.Checked = true;
            cbBGColor.CheckState = CheckState.Checked;
            cbBGColor.Location = new Point(16, 852);
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
            button8.ForeColor = SystemColors.ControlText;
            button8.Location = new Point(210, 813);
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
            button2.FlatStyle = FlatStyle.System;
            button2.ForeColor = SystemColors.ControlText;
            button2.Location = new Point(240, 846);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 651;
            button2.Text = "Save";
            button2.Click += button2_Click;
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
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
            splitContainer2.Panel1.Controls.Add(luBitmapDesignerCtrl1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.AutoScroll = true;
            splitContainer2.Panel2.Controls.Add(label22);
            splitContainer2.Panel2.Controls.Add(btnAlphaCurve);
            splitContainer2.Panel2.Controls.Add(cbExcludeFG);
            splitContainer2.Panel2.Controls.Add(btnFromCache);
            splitContainer2.Panel2.Controls.Add(cbExcludeRegions);
            splitContainer2.Panel2.Controls.Add(btnCloneColors);
            splitContainer2.Panel2.Controls.Add(label14);
            splitContainer2.Panel2.Controls.Add(panel3);
            splitContainer2.Panel2.Controls.Add(panel2);
            splitContainer2.Panel2.Controls.Add(panel1);
            splitContainer2.Panel2.Controls.Add(btnRedo);
            splitContainer2.Panel2.Controls.Add(btnUndo);
            splitContainer2.Panel2.Controls.Add(label6);
            splitContainer2.Panel2.Controls.Add(label5);
            splitContainer2.Panel2.Controls.Add(btnAlphaZAndGain);
            splitContainer2.Panel2.Controls.Add(btnSetGamma);
            splitContainer2.Panel2.Controls.Add(label4);
            splitContainer2.Panel2.Controls.Add(numGamma);
            splitContainer2.Panel2.Controls.Add(numAlphaZAndGain);
            splitContainer2.Panel2.Controls.Add(picInfoCtrl1);
            splitContainer2.Panel2.Controls.Add(btnLoad);
            splitContainer2.Panel2.Controls.Add(label1);
            splitContainer2.Panel2.Controls.Add(Label20);
            splitContainer2.Panel2.Controls.Add(cmbZoom);
            splitContainer2.Panel2.Controls.Add(cbBGColor);
            splitContainer2.Panel2.Controls.Add(btnClone);
            splitContainer2.Panel2.Controls.Add(btnSwap);
            splitContainer2.Panel2.Controls.Add(btnLoadUpper);
            splitContainer2.Panel2.Controls.Add(btnMerge);
            splitContainer2.Panel2.Controls.Add(btnRemove);
            splitContainer2.Panel2.Controls.Add(button8);
            splitContainer2.Panel2.Controls.Add(button2);
            splitContainer2.Size = new Size(1486, 882);
            splitContainer2.SplitterDistance = 1114;
            splitContainer2.SplitterWidth = 5;
            splitContainer2.TabIndex = 0;
            // 
            // luBitmapDesignerCtrl1
            // 
            luBitmapDesignerCtrl1.Dock = DockStyle.Fill;
            luBitmapDesignerCtrl1.Location = new Point(0, 0);
            luBitmapDesignerCtrl1.Margin = new Padding(5, 3, 5, 3);
            luBitmapDesignerCtrl1.Name = "luBitmapDesignerCtrl1";
            luBitmapDesignerCtrl1.SelectedShape = null;
            luBitmapDesignerCtrl1.ShadowMode = false;
            luBitmapDesignerCtrl1.ShapeList = null;
            luBitmapDesignerCtrl1.Size = new Size(1114, 882);
            luBitmapDesignerCtrl1.TabIndex = 0;
            // 
            // cbExcludeFG
            // 
            cbExcludeFG.AutoSize = true;
            cbExcludeFG.Location = new Point(140, 446);
            cbExcludeFG.Name = "cbExcludeFG";
            cbExcludeFG.Size = new Size(83, 19);
            cbExcludeFG.TabIndex = 726;
            cbExcludeFG.Text = "exclude R-FG";
            cbExcludeFG.UseVisualStyleBackColor = true;
            // 
            // btnFromCache
            // 
            btnFromCache.Enabled = false;
            btnFromCache.Location = new Point(262, 15);
            btnFromCache.Margin = new Padding(4, 3, 4, 3);
            btnFromCache.Name = "btnFromCache";
            btnFromCache.Size = new Size(88, 27);
            btnFromCache.TabIndex = 725;
            btnFromCache.Text = "FromCache";
            btnFromCache.UseVisualStyleBackColor = true;
            btnFromCache.Click += btnFromCache_Click;
            // 
            // cbExcludeRegions
            // 
            cbExcludeRegions.AutoSize = true;
            cbExcludeRegions.Location = new Point(26, 446);
            cbExcludeRegions.Name = "cbExcludeRegions";
            cbExcludeRegions.Size = new Size(108, 19);
            cbExcludeRegions.TabIndex = 724;
            cbExcludeRegions.Text = "exclude regions";
            cbExcludeRegions.UseVisualStyleBackColor = true;
            // 
            // btnCloneColors
            // 
            btnCloneColors.Enabled = false;
            btnCloneColors.Location = new Point(210, 343);
            btnCloneColors.Name = "btnCloneColors";
            btnCloneColors.Size = new Size(88, 27);
            btnCloneColors.TabIndex = 723;
            btnCloneColors.Text = "Clone Colors";
            btnCloneColors.UseVisualStyleBackColor = true;
            btnCloneColors.Click += btnCloneColors_Click;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(122, 750);
            label14.Name = "label14";
            label14.Size = new Size(98, 15);
            label14.TabIndex = 722;
            label14.Text = "load upper shape";
            // 
            // panel3
            // 
            panel3.BorderStyle = BorderStyle.FixedSingle;
            panel3.Controls.Add(cbAuto);
            panel3.Controls.Add(label13);
            panel3.Controls.Add(btnShear);
            panel3.Controls.Add(label12);
            panel3.Controls.Add(label11);
            panel3.Controls.Add(label9);
            panel3.Controls.Add(label10);
            panel3.Controls.Add(label8);
            panel3.Controls.Add(numericUpDown6);
            panel3.Controls.Add(numericUpDown4);
            panel3.Controls.Add(numericUpDown5);
            panel3.Controls.Add(numericUpDown2);
            panel3.Controls.Add(numericUpDown3);
            panel3.Controls.Add(numericUpDown1);
            panel3.Location = new Point(9, 598);
            panel3.Name = "panel3";
            panel3.Size = new Size(317, 139);
            panel3.TabIndex = 721;
            // 
            // cbAuto
            // 
            cbAuto.AutoSize = true;
            cbAuto.Checked = true;
            cbAuto.CheckState = CheckState.Checked;
            cbAuto.Location = new Point(30, 108);
            cbAuto.Name = "cbAuto";
            cbAuto.Size = new Size(50, 19);
            cbAuto.TabIndex = 715;
            cbAuto.Text = "auto";
            cbAuto.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(16, 11);
            label13.Name = "label13";
            label13.Size = new Size(36, 15);
            label13.TabIndex = 714;
            label13.Text = "Shear";
            // 
            // btnShear
            // 
            btnShear.Location = new Point(221, 106);
            btnShear.Margin = new Padding(4, 3, 4, 3);
            btnShear.Name = "btnShear";
            btnShear.Size = new Size(88, 27);
            btnShear.TabIndex = 714;
            btnShear.Text = "Go";
            btnShear.UseVisualStyleBackColor = true;
            btnShear.Click += btnShear_Click;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(184, 30);
            label12.Name = "label12";
            label12.Size = new Size(19, 15);
            label12.TabIndex = 713;
            label12.Text = "w:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(106, 30);
            label11.Name = "label11";
            label11.Size = new Size(16, 15);
            label11.TabIndex = 713;
            label11.Text = "y:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(4, 79);
            label9.Name = "label9";
            label9.Size = new Size(16, 15);
            label9.TabIndex = 713;
            label9.Text = "y:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(29, 30);
            label10.Name = "label10";
            label10.Size = new Size(15, 15);
            label10.TabIndex = 713;
            label10.Text = "x:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(4, 50);
            label8.Name = "label8";
            label8.Size = new Size(15, 15);
            label8.TabIndex = 713;
            label8.Text = "x:";
            // 
            // numericUpDown6
            // 
            numericUpDown6.DecimalPlaces = 2;
            numericUpDown6.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDown6.Location = new Point(185, 77);
            numericUpDown6.Margin = new Padding(4, 3, 4, 3);
            numericUpDown6.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numericUpDown6.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numericUpDown6.Name = "numericUpDown6";
            numericUpDown6.Size = new Size(70, 23);
            numericUpDown6.TabIndex = 712;
            // 
            // numericUpDown4
            // 
            numericUpDown4.DecimalPlaces = 2;
            numericUpDown4.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDown4.Location = new Point(107, 77);
            numericUpDown4.Margin = new Padding(4, 3, 4, 3);
            numericUpDown4.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numericUpDown4.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numericUpDown4.Name = "numericUpDown4";
            numericUpDown4.Size = new Size(70, 23);
            numericUpDown4.TabIndex = 712;
            numericUpDown4.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown4.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // numericUpDown5
            // 
            numericUpDown5.DecimalPlaces = 2;
            numericUpDown5.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDown5.Location = new Point(185, 48);
            numericUpDown5.Margin = new Padding(4, 3, 4, 3);
            numericUpDown5.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numericUpDown5.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numericUpDown5.Name = "numericUpDown5";
            numericUpDown5.Size = new Size(70, 23);
            numericUpDown5.TabIndex = 712;
            // 
            // numericUpDown2
            // 
            numericUpDown2.DecimalPlaces = 2;
            numericUpDown2.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDown2.Location = new Point(107, 48);
            numericUpDown2.Margin = new Padding(4, 3, 4, 3);
            numericUpDown2.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numericUpDown2.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numericUpDown2.Name = "numericUpDown2";
            numericUpDown2.Size = new Size(70, 23);
            numericUpDown2.TabIndex = 712;
            numericUpDown2.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // numericUpDown3
            // 
            numericUpDown3.DecimalPlaces = 2;
            numericUpDown3.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDown3.Location = new Point(29, 77);
            numericUpDown3.Margin = new Padding(4, 3, 4, 3);
            numericUpDown3.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numericUpDown3.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numericUpDown3.Name = "numericUpDown3";
            numericUpDown3.Size = new Size(70, 23);
            numericUpDown3.TabIndex = 712;
            numericUpDown3.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // numericUpDown1
            // 
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numericUpDown1.Location = new Point(29, 48);
            numericUpDown1.Margin = new Padding(4, 3, 4, 3);
            numericUpDown1.Maximum = new decimal(new int[] { 32767, 0, 0, 0 });
            numericUpDown1.Minimum = new decimal(new int[] { 32768, 0, 0, int.MinValue });
            numericUpDown1.Name = "numericUpDown1";
            numericUpDown1.Size = new Size(70, 23);
            numericUpDown1.TabIndex = 712;
            numericUpDown1.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.FixedSingle;
            panel2.Controls.Add(label2);
            panel2.Controls.Add(btnGaussian);
            panel2.Location = new Point(9, 559);
            panel2.Name = "panel2";
            panel2.Size = new Size(317, 33);
            panel2.TabIndex = 720;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 9);
            label2.Name = "label2";
            label2.Size = new Size(75, 15);
            label2.TabIndex = 0;
            label2.Text = "GaussianBlur";
            // 
            // btnGaussian
            // 
            btnGaussian.Location = new Point(221, 3);
            btnGaussian.Margin = new Padding(4, 3, 4, 3);
            btnGaussian.Name = "btnGaussian";
            btnGaussian.Size = new Size(88, 27);
            btnGaussian.TabIndex = 714;
            btnGaussian.Text = "Go";
            btnGaussian.UseVisualStyleBackColor = true;
            btnGaussian.Click += btnGaussian_Click;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(label7);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(btnFloodfill);
            panel1.Controls.Add(btnColor);
            panel1.Controls.Add(numTolerance);
            panel1.Location = new Point(9, 499);
            panel1.Name = "panel1";
            panel1.Size = new Size(317, 54);
            panel1.TabIndex = 719;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(105, 18);
            label7.Name = "label7";
            label7.Size = new Size(58, 15);
            label7.TabIndex = 715;
            label7.Text = "Tolerance";
            // 
            // label3
            // 
            label3.BackColor = Color.Black;
            label3.BorderStyle = BorderStyle.FixedSingle;
            label3.Location = new Point(63, 13);
            label3.Name = "label3";
            label3.Size = new Size(36, 25);
            label3.TabIndex = 715;
            label3.Text = "    ";
            // 
            // btnFloodfill
            // 
            btnFloodfill.Location = new Point(220, 13);
            btnFloodfill.Margin = new Padding(4, 3, 4, 3);
            btnFloodfill.Name = "btnFloodfill";
            btnFloodfill.Size = new Size(88, 27);
            btnFloodfill.TabIndex = 714;
            btnFloodfill.Text = "Floodfill";
            btnFloodfill.UseVisualStyleBackColor = true;
            btnFloodfill.Click += btnFloodfill_Click;
            // 
            // btnColor
            // 
            btnColor.Location = new Point(14, 12);
            btnColor.Margin = new Padding(4, 3, 4, 3);
            btnColor.Name = "btnColor";
            btnColor.Size = new Size(47, 27);
            btnColor.TabIndex = 714;
            btnColor.Text = "Color";
            btnColor.UseVisualStyleBackColor = true;
            btnColor.Click += btnColor_Click;
            // 
            // numTolerance
            // 
            numTolerance.Location = new Point(168, 16);
            numTolerance.Margin = new Padding(4, 3, 4, 3);
            numTolerance.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numTolerance.Name = "numTolerance";
            numTolerance.Size = new Size(47, 23);
            numTolerance.TabIndex = 710;
            numTolerance.Value = new decimal(new int[] { 254, 0, 0, 0 });
            // 
            // btnRedo
            // 
            btnRedo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRedo.Enabled = false;
            btnRedo.ForeColor = SystemColors.ControlText;
            btnRedo.Location = new Point(112, 813);
            btnRedo.Margin = new Padding(4, 3, 4, 3);
            btnRedo.Name = "btnRedo";
            btnRedo.Size = new Size(88, 27);
            btnRedo.TabIndex = 718;
            btnRedo.Text = "Redo";
            btnRedo.UseVisualStyleBackColor = true;
            btnRedo.Click += btnRedo_Click;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUndo.Enabled = false;
            btnUndo.ForeColor = SystemColors.ControlText;
            btnUndo.Location = new Point(16, 813);
            btnUndo.Margin = new Padding(4, 3, 4, 3);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(88, 27);
            btnUndo.TabIndex = 717;
            btnUndo.Text = "Undo";
            btnUndo.UseVisualStyleBackColor = true;
            btnUndo.Click += btnUndo_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(193, 389);
            label6.Name = "label6";
            label6.Size = new Size(27, 15);
            label6.TabIndex = 715;
            label6.Text = "to 0";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(24, 389);
            label5.Name = "label5";
            label5.Size = new Size(85, 15);
            label5.TabIndex = 716;
            label5.Text = "set alpha up to";
            // 
            // btnAlphaZAndGain
            // 
            btnAlphaZAndGain.Location = new Point(230, 383);
            btnAlphaZAndGain.Margin = new Padding(4, 3, 4, 3);
            btnAlphaZAndGain.Name = "btnAlphaZAndGain";
            btnAlphaZAndGain.Size = new Size(88, 27);
            btnAlphaZAndGain.TabIndex = 713;
            btnAlphaZAndGain.Text = "Go";
            btnAlphaZAndGain.UseVisualStyleBackColor = true;
            btnAlphaZAndGain.Click += btnAlphaZAndGain_Click;
            // 
            // btnSetGamma
            // 
            btnSetGamma.Location = new Point(230, 414);
            btnSetGamma.Margin = new Padding(4, 3, 4, 3);
            btnSetGamma.Name = "btnSetGamma";
            btnSetGamma.Size = new Size(88, 27);
            btnSetGamma.TabIndex = 714;
            btnSetGamma.Text = "Go";
            btnSetGamma.UseVisualStyleBackColor = true;
            btnSetGamma.Click += btnSetGamma_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(24, 420);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(98, 15);
            label4.TabIndex = 711;
            label4.Text = "set AlphaGamma";
            // 
            // numGamma
            // 
            numGamma.DecimalPlaces = 2;
            numGamma.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            numGamma.Location = new Point(132, 418);
            numGamma.Margin = new Padding(4, 3, 4, 3);
            numGamma.Minimum = new decimal(new int[] { 1, 0, 0, 131072 });
            numGamma.Name = "numGamma";
            numGamma.Size = new Size(70, 23);
            numGamma.TabIndex = 712;
            numGamma.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // numAlphaZAndGain
            // 
            numAlphaZAndGain.Location = new Point(116, 387);
            numAlphaZAndGain.Margin = new Padding(4, 3, 4, 3);
            numAlphaZAndGain.Maximum = new decimal(new int[] { 255, 0, 0, 0 });
            numAlphaZAndGain.Name = "numAlphaZAndGain";
            numAlphaZAndGain.Size = new Size(70, 23);
            numAlphaZAndGain.TabIndex = 710;
            numAlphaZAndGain.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // picInfoCtrl1
            // 
            picInfoCtrl1.Location = new Point(9, 45);
            picInfoCtrl1.Margin = new Padding(5, 3, 5, 3);
            picInfoCtrl1.Name = "picInfoCtrl1";
            picInfoCtrl1.Size = new Size(280, 300);
            picInfoCtrl1.TabIndex = 658;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(150, 15);
            btnLoad.Margin = new Padding(4, 3, 4, 3);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(88, 27);
            btnLoad.TabIndex = 657;
            btnLoad.Text = "Go";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(31, 20);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(87, 15);
            label1.TabIndex = 656;
            label1.Text = "Load BG image";
            // 
            // btnClone
            // 
            btnClone.ForeColor = SystemColors.ControlText;
            btnClone.Location = new Point(150, 780);
            btnClone.Margin = new Padding(4, 3, 4, 3);
            btnClone.Name = "btnClone";
            btnClone.Size = new Size(124, 27);
            btnClone.TabIndex = 652;
            btnClone.Text = "CloneUpperShape";
            btnClone.UseVisualStyleBackColor = true;
            btnClone.Click += btnClone_Click;
            // 
            // btnSwap
            // 
            btnSwap.ForeColor = SystemColors.ControlText;
            btnSwap.Location = new Point(16, 780);
            btnSwap.Margin = new Padding(4, 3, 4, 3);
            btnSwap.Name = "btnSwap";
            btnSwap.Size = new Size(124, 27);
            btnSwap.TabIndex = 652;
            btnSwap.Text = "SwapUpperShapes";
            btnSwap.UseVisualStyleBackColor = true;
            btnSwap.Click += btnSwap_Click;
            // 
            // btnLoadUpper
            // 
            btnLoadUpper.ForeColor = SystemColors.ControlText;
            btnLoadUpper.Location = new Point(231, 744);
            btnLoadUpper.Margin = new Padding(4, 3, 4, 3);
            btnLoadUpper.Name = "btnLoadUpper";
            btnLoadUpper.Size = new Size(88, 27);
            btnLoadUpper.TabIndex = 652;
            btnLoadUpper.Text = "Load";
            btnLoadUpper.UseVisualStyleBackColor = true;
            btnLoadUpper.Click += btnLoadUpper_Click;
            // 
            // btnMerge
            // 
            btnMerge.ForeColor = SystemColors.ControlText;
            btnMerge.Location = new Point(24, 343);
            btnMerge.Margin = new Padding(4, 3, 4, 3);
            btnMerge.Name = "btnMerge";
            btnMerge.Size = new Size(88, 27);
            btnMerge.TabIndex = 652;
            btnMerge.Text = "Merge";
            btnMerge.UseVisualStyleBackColor = true;
            btnMerge.Click += btnMerge_Click;
            // 
            // btnRemove
            // 
            btnRemove.ForeColor = SystemColors.ControlText;
            btnRemove.Location = new Point(16, 744);
            btnRemove.Margin = new Padding(4, 3, 4, 3);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(88, 27);
            btnRemove.TabIndex = 652;
            btnRemove.Text = "Remove";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.ForeColor = SystemColors.ControlText;
            btnCancel.Location = new Point(1384, 8);
            btnCancel.Margin = new Padding(4, 3, 4, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(88, 27);
            btnCancel.TabIndex = 656;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
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
            splitContainer1.Size = new Size(1486, 981);
            splitContainer1.SplitterDistance = 882;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png";
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            // 
            // backgroundWorker2
            // 
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
            backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
            // 
            // colorDialog1
            // 
            colorDialog1.AnyColor = true;
            colorDialog1.FullOpen = true;
            // 
            // backgroundWorker3
            // 
            backgroundWorker3.WorkerReportsProgress = true;
            backgroundWorker3.WorkerSupportsCancellation = true;
            backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
            // 
            // backgroundWorker4
            // 
            backgroundWorker4.WorkerReportsProgress = true;
            backgroundWorker4.WorkerSupportsCancellation = true;
            backgroundWorker4.DoWork += backgroundWorker4_DoWork;
            backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
            backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
            // 
            // openFileDialog2
            // 
            openFileDialog2.FileName = "openFileDialog1";
            openFileDialog2.Filter = "Images - (*.bmp;*.jpg;*.jpeg;*.jfif;*.png)|*.bmp;*.jpg;*.jpeg;*.jfif;*.png";
            // 
            // backgroundWorker6
            // 
            backgroundWorker6.WorkerReportsProgress = true;
            backgroundWorker6.WorkerSupportsCancellation = true;
            backgroundWorker6.DoWork += backgroundWorker6_DoWork;
            backgroundWorker6.ProgressChanged += backgroundWorker6_ProgressChanged;
            backgroundWorker6.RunWorkerCompleted += backgroundWorker6_RunWorkerCompleted;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(143, 473);
            label22.Name = "label22";
            label22.Size = new Size(69, 15);
            label22.TabIndex = 749;
            label22.Text = "AlphaCurve";
            // 
            // btnAlphaCurve
            // 
            btnAlphaCurve.Location = new Point(230, 466);
            btnAlphaCurve.Margin = new Padding(4, 3, 4, 3);
            btnAlphaCurve.Name = "btnAlphaCurve";
            btnAlphaCurve.Size = new Size(88, 27);
            btnAlphaCurve.TabIndex = 748;
            btnAlphaCurve.Text = "Go";
            btnAlphaCurve.UseVisualStyleBackColor = true;
            btnAlphaCurve.Click += btnAlphaCurve_Click;
            // 
            // frmComposePseudoShadow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1486, 981);
            Controls.Add(splitContainer1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "frmComposePseudoShadow";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmCompose";
            FormClosing += frmCompose_FormClosing;
            Load += frmCompose_Load;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDown6).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown4).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown5).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown3).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numTolerance).EndInit();
            ((System.ComponentModel.ISupportInitialize)numGamma).EndInit();
            ((System.ComponentModel.ISupportInitialize)numAlphaZAndGain).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        internal System.Windows.Forms.Label Label20;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.StatusStrip statusStrip1;
        internal System.Windows.Forms.ComboBox cmbZoom;
        internal System.Windows.Forms.CheckBox cbBGColor;
        private System.Windows.Forms.Button button8;
        internal System.Windows.Forms.Button button2;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private LUBitmapDesigner.LUBitmapDesignerCtrl luBitmapDesignerCtrl1;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private LUBitmapDesigner.PicInfoCtrl picInfoCtrl1;
        private Label label6;
        private Label label5;
        private Button btnAlphaZAndGain;
        private Button btnSetGamma;
        private Label label4;
        private NumericUpDown numGamma;
        private NumericUpDown numAlphaZAndGain;
        private Button btnRedo;
        private Button btnUndo;
        internal System.ComponentModel.BackgroundWorker backgroundWorker1;
        internal System.ComponentModel.BackgroundWorker backgroundWorker2;
        private Panel panel1;
        private Label label7;
        private Label label3;
        private Button btnFloodfill;
        private NumericUpDown numTolerance;
        private Button btnColor;
        private ColorDialog colorDialog1;
        internal System.ComponentModel.BackgroundWorker backgroundWorker3;
        internal System.ComponentModel.BackgroundWorker backgroundWorker4;
        private Button btnSwap;
        private Button btnClone;
        private Panel panel2;
        private Label label2;
        private Button btnGaussian;
        private Panel panel3;
        private NumericUpDown numericUpDown4;
        private NumericUpDown numericUpDown2;
        private NumericUpDown numericUpDown3;
        private NumericUpDown numericUpDown1;
        private Label label13;
        private Button btnShear;
        private Label label12;
        private Label label11;
        private Label label9;
        private Label label10;
        private Label label8;
        private NumericUpDown numericUpDown6;
        private NumericUpDown numericUpDown5;
        private CheckBox cbAuto;
        private Button btnRemove;
        private Label label14;
        private Button btnLoadUpper;
        private Button btnMerge;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private Button btnCloneColors;
        private CheckBox cbExcludeRegions;
        public Button btnFromCache;
        private OpenFileDialog openFileDialog2;
        private CheckBox cbExcludeFG;
        internal System.ComponentModel.BackgroundWorker backgroundWorker6;
        private Label label22;
        private Button btnAlphaCurve;
    }
}