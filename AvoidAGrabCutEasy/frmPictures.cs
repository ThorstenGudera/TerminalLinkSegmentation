using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmPictures : Form
    {
        private Bitmap? _bmpRef;
        private Bitmap? _bmpTrimap;
        private Bitmap? _bmpMatte;
        private Bitmap? _bmpWork;
        private Bitmap? _bmpOrig;

        public frmPictures(Bitmap? bmpRef, Bitmap? bmpTrimap, Bitmap? bmpMatte, Bitmap? bmpWork, Bitmap? bmpOrig)
        {
            InitializeComponent();

            if (bmpRef != null)
            {
                this._bmpRef = bmpRef;
                this.listBox1.Items.Add("Result");
            }
            if (bmpTrimap != null)
            {
                this._bmpTrimap = bmpTrimap;
                this.listBox1.Items.Add("Trimap");
            }
            if (bmpMatte != null)
            {
                this._bmpMatte = bmpMatte;
                this.listBox1.Items.Add("Matte");
            }
            if (bmpWork != null)
            {
                this._bmpWork = bmpWork;
                this.listBox1.Items.Add("BmpWork");
            }
            if (bmpOrig != null)
            {
                this._bmpOrig = bmpOrig;
                this.listBox1.Items.Add("Original");
            }
        }

        public frmPictures(Bitmap? bmpRef, Bitmap? bmpOrig)
        {
            InitializeComponent();

            if (bmpRef != null)
            {
                this._bmpRef = bmpRef;
                this.listBox1.Items.Add("Result");
            }
            if (bmpOrig != null)
            {
                this._bmpOrig = bmpOrig;
                this.listBox1.Items.Add("Original");
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > -1 && this._bmpTrimap != null && this._bmpMatte != null && this._bmpWork != null)
                switch (listBox1.SelectedIndex)
                {
                    case 0:
                        this.pictureBox1.Image = _bmpRef;
                        break;

                    case 1:
                        this.pictureBox1.Image = _bmpTrimap;
                        break;

                    case 2:
                        this.pictureBox1.Image = _bmpMatte;
                        break;

                    case 3:
                        this.pictureBox1.Image = _bmpWork;
                        break;

                    case 4:
                        this.pictureBox1.Image = _bmpOrig;
                        break;

                    default:
                        break;
                }
            else if (listBox1.SelectedIndex > -1 && this._bmpRef != null && this._bmpOrig != null)
                switch (listBox1.SelectedIndex)
                {
                    case 0:
                        this.pictureBox1.Image = _bmpRef;
                        break;

                    case 1:
                        this.pictureBox1.Image = _bmpOrig;
                        break;

                    default:
                        break;
                }

            if (this.pictureBox1.Image != null)
                this.label1.Text = "Size: " + this.pictureBox1.Image.Size.ToString();
        }

        private void frmPictures_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (this._bmpRef != null)
            //    this._bmpRef.Dispose();
            //if (this._bmpTrimap != null)
            //    this._bmpTrimap.Dispose();
            //if (this._bmpMatte != null)
            //    this._bmpMatte.Dispose();
            //if (this._bmpWork != null)
            //    this._bmpWork.Dispose();
            //if (this._bmpOrig != null)
            //    this._bmpOrig.Dispose();  // dont dispose, we use the "originals" and dispose them in the parent form
        }

        private void frmPictures_Load(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
                this.listBox1.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(this.pictureBox1.Image != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                this.pictureBox1.Image.Save(this.saveFileDialog1.FileName, ImageFormat.Png);
        }
    }
}
