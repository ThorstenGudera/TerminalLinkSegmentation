using AvoidAGrabCutEasy;
using OutlineOperations;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace TerminalLinkSegmentation
{
    public partial class Form1 : Form
    {
        private string _basePathAddition = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Thorsten_Gudera");
        private bool _picChanged;
        private float _zoom;
        private Rectangle? _rc;
        private int _ix;
        private bool _dontDrawRect;
        private int _iy;
        private int _endX;
        private int _endY;
        private string _lastOpenedFile = "";

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

                if (bmp != null)
                {
                    this._lastOpenedFile = this.OpenFileDialog1.FileName;

                    Image iOld = this.pictureBox1.Image;
                    this.pictureBox1.Image = bmp;
                    if (iOld != null)
                        iOld.Dispose();

                    this.Text = this.OpenFileDialog1.FileName;

                    this._rc = GetImageRectangle();
                    this._ix = this._iy = this._endX = this._endY = 0;

                    this._dontDrawRect = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                if (this._ix < 0)
                    this._ix = 0;
                if (this._ix < 0)
                    this._ix = 0;
                if (this._endX - this._ix <= 0)
                    this._endX = this.pictureBox1.Image.Width;
                if (this._endY - this._iy <= 0)
                    this._endY = this.pictureBox1.Image.Height;

                int x = Math.Max(this._ix, 0);
                int y = Math.Max(this._iy, 0);
                int w = Math.Min(this._endX - this._ix, this.pictureBox1.Image.Width - x);
                int h = Math.Min(this._endY - this._iy, this.pictureBox1.Image.Height - y);

                using Bitmap bTmp = new Bitmap(this.pictureBox1.Image);
                using Bitmap b = bTmp.Clone(new Rectangle(x, y, w, h), PixelFormat.Format32bppArgb);
                frmAvoidAGrabCutEasy frm = new frmAvoidAGrabCutEasy(b, _basePathAddition);

                frm.SetupCache();
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.FBitmap != null)
                    {
                        Image iOld = this.pictureBox1.Image;

                        this.pictureBox1.Image = new Bitmap(frm.FBitmap);
                        if (iOld != null)
                            iOld.Dispose();

                        this.button4_Click(this.button4, new EventArgs());
                        this._picChanged = true;
                    }
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this._picChanged)
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

        // get position of pic in picturebox
        private Rectangle? GetImageRectangle()
        {
            Type pboxType = this.pictureBox1.GetType();

            if (pboxType != null)
            {
                System.Reflection.PropertyInfo? irProperty = pboxType.GetProperty("ImageRectangle", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                Rectangle? r = null;

                if (irProperty != null)
                    r = (Rectangle?)irProperty.GetValue(this.pictureBox1, null);

                _zoom = System.Convert.ToSingle(r?.Width / (double)this.pictureBox1.Image.Width);

                if (r != null)
                    return r.Value;
            }

            return null;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this._zoom != 0 && this.pictureBox1.Image != null && !this.pictureBox1.IsDisposed && this._rc != null)
            {
                int ix = (int)Math.Ceiling((double)(e.X - this._rc.Value.X) / (double)this._zoom) + this.splitContainer1.Panel2.AutoScrollMinSize.Width;
                int iy = (int)Math.Ceiling((double)(e.Y - this._rc.Value.Y) / (double)this._zoom) + this.splitContainer1.Panel2.AutoScrollMinSize.Height;

                if (ix >= 0 && ix < this.pictureBox1.Image.Width &&
                    iy >= 0 && iy < this.pictureBox1.Image.Height)
                {
                    this._ix = ix;
                    this._iy = iy;
                }

                this.pictureBox1.Capture = true;
            }

            if (this.pictureBox1.Image != null)
            {
                if (this._ix < 0)
                    this._ix = 0;
                if (this._ix < 0)
                    this._ix = 0;
                if (this._endX - this._ix <= 0)
                    this._endX = this.pictureBox1.Image.Width;
                if (this._endY - this._iy <= 0)
                    this._endY = this.pictureBox1.Image.Height;

                int x = Math.Max(this._ix, 0);
                int y = Math.Max(this._iy, 0);
                int w = Math.Min(this._endX - this._ix, this.pictureBox1.Image.Width - x);
                int h = Math.Min(this._endY - this._iy, this.pictureBox1.Image.Height - y);

                this.toolStripStatusLabel1.Text = new Rectangle(x, y, w, h).ToString();
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this._zoom != 0 && this.pictureBox1.Image != null && !this.pictureBox1.IsDisposed && this._rc != null)
            {
                int ix = (int)Math.Ceiling((double)(e.X - this._rc.Value.X) / (double)this._zoom) + this.splitContainer1.Panel2.AutoScrollMinSize.Width;
                int iy = (int)Math.Ceiling((double)(e.Y - this._rc.Value.Y) / (double)this._zoom) + this.splitContainer1.Panel2.AutoScrollMinSize.Height;

                if (ix >= 0 && e.X < this.pictureBox1.ClientSize.Width &&
                    iy >= 0 && e.Y < this.pictureBox1.ClientSize.Height)
                {
                    this._endX = ix;
                    this._endY = iy;
                }

                this.pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this._zoom != 0 && this.pictureBox1.Image != null && !this.pictureBox1.IsDisposed && this._rc != null)
            {
                int ix = (int)Math.Ceiling((double)(e.X - this._rc.Value.X) / (double)this._zoom) + this.splitContainer1.Panel2.AutoScrollMinSize.Width;
                int iy = (int)Math.Ceiling((double)(e.Y - this._rc.Value.Y) / (double)this._zoom) + this.splitContainer1.Panel2.AutoScrollMinSize.Height;

                if (ix >= 0 && e.X < this.pictureBox1.ClientSize.Width &&
                    iy >= 0 && e.Y < this.pictureBox1.ClientSize.Height &&
                    ix != this._ix && iy != this._iy)
                {
                    this._endX = Math.Min(ix, this.pictureBox1.Image.Width);
                    this._endY = Math.Min(iy, this.pictureBox1.Image.Height);
                }
                this.pictureBox1.Invalidate();
            }

            this.pictureBox1.Capture = false;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!this._dontDrawRect && this._zoom != 0 && this.pictureBox1.Image != null && !this.pictureBox1.IsDisposed && this._rc != null)
            {
                e.Graphics.DrawRectangle(Pens.Lime,
                    new RectangleF(this._ix * this._zoom + this._rc.Value.X + this.splitContainer1.Panel2.AutoScrollMinSize.Width,
                    this._iy * this._zoom + this._rc.Value.Y + this.splitContainer1.Panel2.AutoScrollMinSize.Height,
                        (this._endX - this._ix) * this._zoom, (this._endY - this._iy) * this._zoom));
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.pictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
            {
                this.pictureBox1.Dock = DockStyle.None;
                this.pictureBox1.Location = new Point(0, 0);
                this.pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            else
            {
                this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                this.pictureBox1.Dock = DockStyle.Fill;
            }

            this._rc = this.GetImageRectangle();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this._ix = this._iy = this._endX = this._endY = 0;

            if (this.pictureBox1.Image != null)
            {
                int x = 0;
                int y = 0;
                int w = this.pictureBox1.Image.Width;
                int h = this.pictureBox1.Image.Height;

                this.toolStripStatusLabel1.Text = new Rectangle(x, y, w, h).ToString();
            }

            this.pictureBox1.Invalidate();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
                pictureBox1_DoubleClick(this.pictureBox1, new EventArgs());
        }

        private void btnOutlineOperations_Click(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                using Bitmap bmp = new Bitmap(this.pictureBox1.Image);
                using (frnOutlineOperations frm = new frnOutlineOperations(bmp, this._basePathAddition))
                {
                    frm.SetupCache();

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (frm.FBitmap != null)
                        {
                            Image iOld = this.pictureBox1.Image;

                            this.pictureBox1.Image = new Bitmap(frm.FBitmap);
                            if (iOld != null)
                                iOld.Dispose();

                            this.button4_Click(this.button4, new EventArgs());
                            this._picChanged = true;
                        }
                    }
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string? s = Path.GetDirectoryName(Application.StartupPath);
            if (s != null && File.Exists(Path.Combine(s, "DemoLightWeight.exe")))
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = Path.Combine(s, "DemoLightWeight.exe");
                if (this.pictureBox1.Image != null)
                    p.StartInfo.Arguments = "\"" + this._lastOpenedFile + "\"";

                p.Start();
            }
        }
    }
}
