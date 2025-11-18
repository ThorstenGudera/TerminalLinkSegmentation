using ConvolutionLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlineOperations
{
    public partial class frmAlphaThreshold : Form
    {
        private Bitmap? _bmp;
        private int _left;
        private int _top;
        private Bitmap? _bmpBU;
        //private bool _dontDoIt;
        private ThresholdMethods? _thresholdMethods;
        private bool _tracking;
        private int _x;
        private int _y;
        private float _zoom;

        public Bitmap? FBitmap { get; private set; }

        public frmAlphaThreshold(Bitmap bmp)
        {
            InitializeComponent();

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                _bmpBU = (Bitmap)bmp.Clone();
            else
            {
                MessageBox.Show("Not enough Memory");
                return;
            }

            Init();
        }

        private void Init()
        {
            this.pictureBox1.Dock = DockStyle.None;
            this.pictureBox1.Location = new Point(0, 0);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
            this.pictureBox1.Size = new Size(this.splitContainer1.Panel1.ClientSize.Width, this.splitContainer1.Panel1.ClientSize.Height);
            this.pictureBox1.Cursor = Cursors.Hand;

            _left = 0;
            _top = 0;

            Bitmap? b = this._bmp;

            if (AvailMem.AvailMem.checkAvailRam(this.pictureBox1.ClientSize.Width * this.pictureBox1.ClientSize.Height * 4L))
                _bmp = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
            else
            {
                if (b != null)
                {
                    b.Dispose();
                    b = null;
                }
                return;
            }

            if (this._bmpBU != null)
            {
                using (Graphics gx = Graphics.FromImage(_bmp))
                    gx.DrawImage(_bmpBU, new Rectangle(0, 0, _bmp.Width, _bmp.Height),
                        new Rectangle(_left, _top, this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel);
            }

            if (b != null)
            {
                b.Dispose();
                b = null;
            }

            if (this._bmp != null && AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                this.pictureBox1.Image = (Bitmap)this._bmp.Clone();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            this.cbZoom.Checked = !this.cbZoom.Checked;
        }

        private void cbZoom_CheckedChanged(object sender, EventArgs e)
        {
            if (this._bmpBU != null)
                if (this.cbZoom.Checked)
                {
                    this.pictureBox1.Dock = DockStyle.Fill;
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    this.pictureBox1.Cursor = Cursors.Default;

                    Bitmap? b = this._bmp;
                    _bmp = ResampleToFit(_bmpBU);
                    if (b != null)
                    {
                        b.Dispose();
                        b = null;
                    }
                }
                else
                {
                    this.pictureBox1.Dock = DockStyle.None;
                    this.pictureBox1.Location = new Point(0, 0);
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                    this.pictureBox1.Size = new Size(this.splitContainer1.Panel1.ClientSize.Width, this.splitContainer1.Panel1.ClientSize.Height);
                    this.pictureBox1.Cursor = Cursors.Hand;

                    Bitmap? b = this._bmp;

                    if (AvailMem.AvailMem.checkAvailRam(this.pictureBox1.ClientSize.Width * this.pictureBox1.ClientSize.Height * 4L))
                        _bmp = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
                    else
                    {
                        if (b != null)
                        {
                            b.Dispose();
                            b = null;
                        }
                        return;
                    }

                    if (this._bmpBU != null)
                    {
                        using (Graphics gx = Graphics.FromImage(_bmp))
                            gx.DrawImage(_bmpBU, new Rectangle(0, 0, _bmp.Width, _bmp.Height),
                                new Rectangle(_left, _top, this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel);
                    }

                    if (b != null)
                    {
                        b.Dispose();
                        b = null;
                    }
                }

            if (this._bmp != null && AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                this.pictureBox1.Image = (Bitmap)this._bmp.Clone();

            DoIt();
        }

        private void DoIt()
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            try
            {
                object[] o = new object[] { this.trackBar1.Value };
                this.backgroundWorker1.RunWorkerAsync(o);
            }
            catch
            {
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.timer1.Stop();

            if ((this.trackBar1.Value & 1) != 0x01)
                this.trackBar1.Value += 1;
            this.tbVal.Text = this.trackBar1.Value.ToString();
            this.timer1.Enabled = true;
            this.timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            DoIt();
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null && this._bmp != null)
            {
                bool b = false;
                object[] o = (object[])e.Argument;
                int th = (int)o[0];
                if (th < 0)
                    th = 0;
                if (th > 255)
                    th = 255;

                Bitmap bmp = (Bitmap)this._bmp.Clone();

                if (this._thresholdMethods == null)
                    this._thresholdMethods = new ThresholdMethods();
                _thresholdMethods.CancelLoops = false;

                if (this.backgroundWorker1.CancellationPending)
                    _thresholdMethods.CancelLoops = true;

                b = ThresholdMethods.ThresholdAlpha(bmp, th);

                bool bb = _thresholdMethods.CancelLoops;
                e.Result = new object[] { bmp, th, bb };
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                object[] o = (object[])e.Result;

                if ((bool)o[2] == false)
                {
                    Image? iOld = this.pictureBox1.Image;
                    this.pictureBox1.Image = (Bitmap)o[0];
                    if (iOld != null)
                    {
                        iOld.Dispose();
                        iOld = null;
                    }

                    if (this.cbZoom.Checked)
                    {
                        int blur = (int)o[1];
                        if (blur < 3)
                            blur = 3;
                        if ((blur & 1) != 0x01)
                            blur += 1;
                        if (blur > 255)
                            blur = 255;


                        this.tbLast.Text = blur.ToString();
                    }
                    else
                        this.tbLast.Text = o[1].ToString();
                }
                else if (o != null && (bool)o[2] == true)
                    this.tbLast.Text = "cancelled";
            }

            if (e.Result == null)
                MessageBox.Show("Fehler.");

            // create a new bgw, since it reported "complete"
            this.backgroundWorker1.Dispose();
            this.backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            //this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
        }

        private void rbSmall_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (this.rbSmall.Checked)
                {
                    this.trackBar1.Minimum = 3;
                    this.trackBar1.Maximum = 127;
                }
                else
                {
                    this.trackBar1.Minimum = 127;
                    this.trackBar1.Maximum = 255;
                }

                this.tbVal.Text = this.trackBar1.Value.ToString();
                DoIt();
            }
        }

        private Bitmap? ResampleToFit(Bitmap bmp)
        {
            Bitmap? b = null;

            try
            {
                if (bmp != null)
                {
                    float zoom = 1.0F;

                    double faktor1 = System.Convert.ToDouble(this.pictureBox1.ClientSize.Width) / System.Convert.ToDouble(this.pictureBox1.ClientSize.Height);
                    double faktor2 = System.Convert.ToDouble(bmp.Width) / System.Convert.ToDouble(bmp.Height);

                    if (faktor2 > faktor1)
                        zoom = (float)(this.pictureBox1.ClientSize.Width / (double)bmp.Width);
                    else
                        zoom = (float)(this.pictureBox1.ClientSize.Height / (double)bmp.Height);

                    int w2 = System.Convert.ToInt32(Math.Ceiling(bmp.Width * zoom));
                    int h2 = System.Convert.ToInt32(Math.Ceiling(bmp.Height * zoom));
                    if (AvailMem.AvailMem.checkAvailRam(w2 * h2 * 4L))
                        b = new Bitmap(w2, h2);
                    else
                    {
                        if (b != null)
                        {
                            b.Dispose();
                            b = null;
                        }
                        return null;
                    }

                    using (Graphics gx = Graphics.FromImage(b))
                        gx.DrawImage(bmp, 0, 0, bmp.Width * zoom, bmp.Height * zoom);

                    this._zoom = zoom;
                }
            }
            catch
            {
            }
            return b;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _x = e.X;
                _y = e.Y;

                // draw the orig pic to see the difference
                if (_bmp != null && this._bmpBU != null)
                {
                    Bitmap? b = this._bmp;

                    if (this.cbZoom.Checked == false)
                    {
                        if (AvailMem.AvailMem.checkAvailRam(this.pictureBox1.ClientSize.Width * this.pictureBox1.ClientSize.Height * 4L))
                            _bmp = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
                        else
                        {
                            if (b != null)
                            {
                                b.Dispose();
                                b = null;
                            }
                            return;
                        }

                        using (Graphics g = Graphics.FromImage(_bmp))
                            g.DrawImage(_bmpBU, new Rectangle(0, 0, _bmp.Width, _bmp.Height),
                                new Rectangle(_left, _top, this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel);
                    }
                    else
                        _bmp = ResampleToFit(_bmpBU);

                    if (b != null)
                    {
                        b.Dispose();
                        b = null;
                    }

                    _tracking = true;
                    if (_bmp != null && AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                        this.pictureBox1.Image = (Bitmap)_bmp.Clone();
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_tracking && this._bmpBU != null)
            {
                _left = Math.Min(Math.Max(_left + (_x - e.X), 0), _bmpBU.Width - this.pictureBox1.ClientSize.Width);
                _top = Math.Min(Math.Max(_top + (_y - e.Y), 0), _bmpBU.Height - this.pictureBox1.ClientSize.Height);
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._bmpBU != null && _tracking && (_bmpBU.Width > pictureBox1.Width || _bmpBU.Height > pictureBox1.Height))
            {
                Bitmap? b = this._bmp;

                if (this.cbZoom.Checked == false)
                {
                    if (AvailMem.AvailMem.checkAvailRam(this.pictureBox1.ClientSize.Width * this.pictureBox1.ClientSize.Height * 4L))
                        _bmp = new Bitmap(this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height);
                    else
                    {
                        if (b != null)
                        {
                            b.Dispose();
                            b = null;
                        }
                        return;
                    }

                    using (Graphics g = Graphics.FromImage(_bmp))
                        g.DrawImage(_bmpBU, new Rectangle(0, 0, _bmp.Width, _bmp.Height),
                            new Rectangle(_left, _top, this.pictureBox1.ClientSize.Width, this.pictureBox1.ClientSize.Height), GraphicsUnit.Pixel);
                }
                else
                    _bmp = ResampleToFit(_bmpBU);

                if (this._bmp != null && AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                    this.pictureBox1.Image = (Bitmap)_bmp.Clone();

                if (b != null)
                {
                    b.Dispose();
                    b = null;
                }

                DoIt();

                _tracking = false;
            }
        }

        private void frmGaussianBlur_Load(object sender, EventArgs e)
        {
            this.tbVal.Text = this.trackBar1.Value.ToString();
            this.cbZoom_CheckedChanged(this.cbZoom, new EventArgs());
        }

        private void frmGaussianBlur_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();
            if (this._bmp != null)
                this._bmp.Dispose();
            if (this._bmpBU != null)
                this._bmpBU.Dispose();
        }

        public void ComputeFBitmap(Bitmap b, int th)
        {
            if (th < 0)
                th = 0;
            if (th > 255)
                th = 255;

            Bitmap bmp = (Bitmap)b.Clone();

            if (bmp != null)
            {
                Convolution thMthds = new Convolution();
                thMthds.CancelLoops = false;

                //thMthds.ProgressPlus += ThMthds_ProgressPlus; 

                //if (this.backgroundWorker4.CancellationPending)
                //    thMthds.CancelLoops = true;

                ThresholdMethods.ThresholdAlpha(bmp, th);

                //thMthds.ProgressPlus -= ThMthds_ProgressPlus;
            }

            Bitmap? bOld = this.FBitmap;
            this.FBitmap = bmp;
            if(bOld != null)
                bOld.Dispose();
            bOld = null;
        }

        private void ThMthds_ProgressPlus(object sender, ConvolutionLib.ProgressEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
