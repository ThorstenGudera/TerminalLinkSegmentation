using AvoidAGrabCutEasy;

namespace TerminalLinkSegmentation
{
    public partial class Form1 : Form
    {
        private string _basePathAddition = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Thorsten_Gudera");
        private bool _picChanged;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this._picChanged)
            {
                DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dlg == DialogResult.Yes)
                    button3.PerformClick();
                else if (dlg == DialogResult.Cancel)
                    return;
            }

            Bitmap? bmp = null;
            if (this.OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (Image img = Image.FromFile(this.OpenFileDialog1.FileName))
                    bmp = new Bitmap(img);

                //test, if the BitmapData class is really back in .net 8.0
                //if(bmp != null)
                //{
                //    int w = bmp.Width;
                //    int h = bmp.Height;
                //    BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                //    int stride = bmD.Stride;

                //    unsafe
                //    {
                //        Parallel.For(0, h, y =>
                //        {
                //            byte* p = (byte*)bmD.Scan0;
                //            p += y * stride;

                //            for(int x = 0; x < w; x++)
                //            {
                //                p[0] = (byte)(255 - p[0]);      
                //                p[1] = (byte)(255 - p[1]); 
                //                p[2] = (byte)(255 - p[2]);

                //                p += 4;
                //            }
                //        });
                //    }

                //    bmp.UnlockBits(bmD);
                //}

                Image iOld = this.pictureBox1.Image;
                this.pictureBox1.Image = bmp;
                if (iOld != null)
                    iOld.Dispose();

                this.Text = this.OpenFileDialog1.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                frmAvoidAGrabCutEasy frm = new frmAvoidAGrabCutEasy(new Bitmap((Bitmap)this.pictureBox1.Image), _basePathAddition);
                frm.SetupCache();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.FBitmap != null)
                    {
                        Image iOld = this.pictureBox1.Image;

                        this.pictureBox1.Image = new Bitmap(frm.FBitmap);
                        if (iOld != null)
                            iOld.Dispose();

                        this._picChanged = true;
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(this._picChanged)
            {
                DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dlg == DialogResult.Yes)
                {
                    button3.PerformClick();
                    if (this._picChanged)
                        e.Cancel = true;
                }
                else if (dlg == DialogResult.Cancel)
                    e.Cancel = true;
            }

            if (!e.Cancel)
            {
                if (this.pictureBox1.Image != null)
                {
                    this.pictureBox1.Image.Dispose();
                    this.pictureBox1.Image = null;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

                try
                {
                    if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.pictureBox1.Image.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        _picChanged = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
