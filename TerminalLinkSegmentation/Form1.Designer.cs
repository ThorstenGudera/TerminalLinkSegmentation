namespace TerminalLinkSegmentation
{
    partial class Form1
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

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox1 = new PictureBox();
            splitContainer1 = new SplitContainer();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            OpenFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            ToolTip1 = new ToolTip(components);
            timer1 = new System.Windows.Forms.Timer(components);
            button5 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(933, 452);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.DoubleClick += pictureBox1_DoubleClick;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4, 3, 4, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(button5);
            splitContainer1.Panel1.Controls.Add(button4);
            splitContainer1.Panel1.Controls.Add(button3);
            splitContainer1.Panel1.Controls.Add(button2);
            splitContainer1.Panel1.Controls.Add(button1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.AutoScroll = true;
            splitContainer1.Panel2.BackColor = SystemColors.ControlDarkDark;
            splitContainer1.Panel2.Controls.Add(pictureBox1);
            splitContainer1.Size = new Size(933, 519);
            splitContainer1.SplitterDistance = 62;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 5;
            // 
            // button4
            // 
            button4.Location = new Point(641, 14);
            button4.Name = "button4";
            button4.Size = new Size(88, 27);
            button4.TabIndex = 3;
            button4.Text = "reset rect";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.Location = new Point(146, 14);
            button3.Name = "button3";
            button3.Size = new Size(88, 27);
            button3.TabIndex = 2;
            button3.Text = "save";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(816, 14);
            button2.Margin = new Padding(4, 3, 4, 3);
            button2.Name = "button2";
            button2.Size = new Size(88, 27);
            button2.TabIndex = 1;
            button2.Text = "Go";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(14, 14);
            button1.Margin = new Padding(4, 3, 4, 3);
            button1.Name = "button1";
            button1.Size = new Size(88, 27);
            button1.TabIndex = 0;
            button1.Text = "open";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // OpenFileDialog1
            // 
            OpenFileDialog1.FileName = "OpenFileDialog1";
            OpenFileDialog1.Filter = "Bilder (*.jpg;*.bmp;*.png)|*.jpg;*.bmp;*.png|Alle Dateien (*.*)|*.*";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.FileName = "Bild1.png";
            saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // button5
            // 
            button5.Location = new Point(537, 14);
            button5.Name = "button5";
            button5.Size = new Size(88, 27);
            button5.TabIndex = 3;
            button5.Text = "switch zoom";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(933, 519);
            Controls.Add(splitContainer1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            FormClosing += Form1_FormClosing;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        internal System.Windows.Forms.OpenFileDialog OpenFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        internal System.Windows.Forms.ToolTip ToolTip1;
        private Button button3;
        private System.Windows.Forms.Timer timer1;
        private Button button4;
        private Button button5;
    }
}
