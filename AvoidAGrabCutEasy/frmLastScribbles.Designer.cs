﻿namespace AvoidAGrabCutEasy
{
    partial class frmLastScribbles
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
            cmbScribblesType = new ComboBox();
            label1 = new Label();
            listBox1 = new ListBox();
            listBox2 = new ListBox();
            btnDelete = new Button();
            btnOK = new Button();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // cmbScribblesType
            // 
            cmbScribblesType.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmbScribblesType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScribblesType.FormattingEnabled = true;
            cmbScribblesType.Location = new Point(422, 12);
            cmbScribblesType.Name = "cmbScribblesType";
            cmbScribblesType.Size = new Size(121, 23);
            cmbScribblesType.TabIndex = 0;
            cmbScribblesType.SelectedIndexChanged += cnbScribblesType_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(375, 15);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 1;
            label1.Text = "Type";
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(422, 41);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(120, 94);
            listBox1.TabIndex = 2;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // listBox2
            // 
            listBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new Point(569, 41);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(120, 94);
            listBox2.TabIndex = 2;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            // 
            // btnDelete
            // 
            btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnDelete.Location = new Point(603, 141);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(86, 27);
            btnDelete.TabIndex = 3;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.ForeColor = SystemColors.ControlText;
            btnOK.Location = new Point(611, 290);
            btnOK.Margin = new Padding(4, 3, 4, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(88, 27);
            btnOK.TabIndex = 650;
            btnOK.Text = "Close";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.AutoScroll = true;
            panel1.Controls.Add(pictureBox1);
            panel1.Location = new Point(12, 15);
            panel1.Name = "panel1";
            panel1.Size = new Size(344, 302);
            panel1.TabIndex = 651;
            // 
            // pictureBox1
            // 
            pictureBox1.Dock = DockStyle.Fill;
            pictureBox1.Location = new Point(0, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(344, 302);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.DoubleClick += pictureBox1_DoubleClick;
            // 
            // frmLastScribbles
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(712, 329);
            Controls.Add(panel1);
            Controls.Add(btnOK);
            Controls.Add(btnDelete);
            Controls.Add(listBox2);
            Controls.Add(listBox1);
            Controls.Add(label1);
            Controls.Add(cmbScribblesType);
            Name = "frmLastScribbles";
            StartPosition = FormStartPosition.CenterParent;
            Text = "frmLastScribbles";
            Load += frmLastScribbles_Load;
            Layout += frmLastScribbles_Layout;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ComboBox cmbScribblesType;
        private Label label1;
        private ListBox listBox1;
        private ListBox listBox2;
        private Button btnDelete;
        private Button btnOK;
        private Panel panel1;
        private PictureBox pictureBox1;
    }
}