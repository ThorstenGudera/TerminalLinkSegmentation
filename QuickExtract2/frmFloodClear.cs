using ChainCodeFinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickExtract2
{
    public partial class frmFloodClear : Form
    {
        private bool _Smooth;
        private Color _ReplaceColor;
        private bool _ColorDistance;
        private double _ToleranceEdges;
        private Point _Pt;
        private double _Tolerance;
        private Color _StartColor;
        private double _Tolerance2;
        private bool _CompAlpha;
        private int _IterationsMaximum = Int32.MaxValue;
        private double _MinVal;
        private double _Epsilon = Math.Sqrt(2.0);

        private static int[] CustomColors = new int[] { };

        private Bitmap? _bmp = null;
        private Rectangle? _r;
        private float _zoom = 1.0F;
        private bool _useCurves;
        private bool _useClosedCurve;

        public Bitmap FBitmap
        {
            get
            {
                return (Bitmap)this.PictureBox1.Image;
            }
        }

        public frmFloodClear(Bitmap bmp)
        {
            InitializeComponent();

            this._bmp = (Bitmap)bmp.Clone();
            this.PictureBox1.Image = (Bitmap)this._bmp.Clone();
            this._r = GetImageRectangle();

            this.comboBox1.SelectedIndex = 1;
        }

        public void SetValues()
        {
            this._Tolerance = System.Convert.ToDouble(this.numericUpDown5.Value); // the "normal" tolerance to compare the pixels with the start-color-pixel
            this._StartColor = this.label12.BackColor; // color, where we clicked into the image
            this._ReplaceColor = this.label2.BackColor; // color to fill in
            this._IterationsMaximum = Int32.MaxValue; // limit
            this._ColorDistance = this.checkBox2.Checked; // shall we compare the r,g,b values directly or shall we compute a color-distance in the rgb vectorspace (p2 norm of the vector)?
            this._Tolerance2 = System.Convert.ToDouble(this.numericUpDown6.Value); // maximum additional tolerance, tolerance exceeding the "normal" floodfill tolerance
            this._MinVal = 1000.0 - System.Convert.ToDouble(this.numericUpDown7.Value); // 0 to 1000, minimum weight for colors that are further away from the startcolor than the ordinary floodfill tolerance.
            this._ToleranceEdges = System.Convert.ToDouble(this.NumericUpDown8.Value); // max distance of neighbors of current pixel thats processed
            this._Smooth = this.checkBox1.Checked; // smooth output (edges)
            this._CompAlpha = this.CheckBox9.Checked; // if smooth set, the mode for the chaincode outline finder (grayscale- or alpha mode)

            // max allowed distance for approximating lines
            double epsilon = Math.Sqrt(2.0) / 2.0;

            switch (comboBox1.SelectedIndex)
            {
                case 1:
                    {
                        epsilon = 1.0;
                        break;
                    }

                case 2:
                    {
                        epsilon = Math.Sqrt(2.0);
                        break;
                    }

                case 3:
                    {
                        epsilon = 2.0;
                        break;
                    }

                case 4:
                    {
                        epsilon = System.Convert.ToDouble(NumericUpDown1.Value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            this._Epsilon = epsilon;
        }

        private void BackgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                Bitmap? b = null;
                //BitmapData bmData = null;

                this.BackgroundWorker1.ReportProgress(50);

                object[] o = (object[])e.Argument;

                try
                {
                    Bitmap b2 = (Bitmap)o[0]; // get the source pic
                    if (AvailMem.AvailMem.checkAvailRam(b2.Width * b2.Height * 4L))
                        b = b2;
                    else
                        throw new Exception();

                    bool bAlpha = this._Smooth && this._ReplaceColor.A == 0; // if we clear instead of fill (alpha = 0), we will smooth the results later
                    if (bAlpha)
                        this._Smooth = false;

                    // floodfill
                    if (_ColorDistance)
                    {
                        if (_ToleranceEdges == 255)
                            FloodFillMethods.floodfill(b, this._Pt.X, this._Pt.Y, this._Tolerance, this._StartColor, this._ReplaceColor, this._IterationsMaximum, _Tolerance2, _MinVal, this._Smooth, this._CompAlpha, this._Epsilon, this._useCurves, this._useClosedCurve);
                        else
                            FloodFillMethods.floodfill(b, this._Pt.X, this._Pt.Y, this._Tolerance, this._StartColor, this._ReplaceColor, this._IterationsMaximum, _Tolerance2, _MinVal, _ToleranceEdges, this._Smooth, this._CompAlpha, this._Epsilon, this._useCurves, this._useClosedCurve);
                    }
                    else if (_ToleranceEdges == 255)
                        FloodFillMethods.floodfill(b, this._Pt.X, this._Pt.Y, this._Tolerance, this._StartColor, this._ReplaceColor, this._IterationsMaximum, this._Smooth, this._CompAlpha, this._Epsilon, this._useCurves, this._useClosedCurve);
                    else
                        FloodFillMethods.floodfill(b, this._Pt.X, this._Pt.Y, this._Tolerance, this._StartColor, this._ReplaceColor, this._IterationsMaximum, _ToleranceEdges, this._Smooth, this._CompAlpha, this._Epsilon, this._useCurves, this._useClosedCurve);

                    if (bAlpha)
                    {
                        Bitmap? bmp = null;

                        if (AvailMem.AvailMem.checkAvailRam(b.Width * b.Height * 4L))
                        {
                            try
                            {
                                bmp = new Bitmap(b.Width, b.Height);

                                List<ChainCode> fList = new List<ChainCode>(); // get the outline of the "floodcleared" picture
                                ChainFinder fbmp = new ChainFinder();
                                fbmp.AllowNullCells = false;

                                if (_CompAlpha)
                                    fList = fbmp.GetOutline(b, 0, false, 0, false, 0, false);
                                else
                                    fList = fbmp.GetOutline(b, 0, true, 0, false, 0, false);

                                fbmp.RemoveOutlines(fList, 4);

                                if (fList.Count > 0)
                                {
                                    using (GraphicsPath fPath = FloodFillMethods.GetPath(fList, this._Epsilon, this._useCurves, this._useClosedCurve)) // create a GraphicsPath from the chains
                                    {
                                        if (fPath != null)
                                        {
                                            try
                                            {
                                                using (Graphics g = Graphics.FromImage(bmp))
                                                using (TextureBrush tb = new TextureBrush(b) // fill the path into a new bitmap
        )
                                                {
                                                    g.Clear(Color.Transparent);
                                                    g.SmoothingMode = SmoothingMode.AntiAlias;
                                                    g.FillPath(tb, fPath);
                                                }

                                                using (Graphics g = Graphics.FromImage(b)) // "insert" that new bitmap into the current working bitmap
                                                {
                                                    g.Clear(Color.Transparent);
                                                    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                            }
                            finally
                            {
                                if (bmp != null)
                                {
                                    bmp.Dispose();
                                    bmp = null;
                                }
                            }
                        }
                    }
                }
                catch
                {
                    if (b != null)
                    {
                        b.Dispose();
                        b = null;
                    }
                }

                this.BackgroundWorker1.ReportProgress(100);

                e.Result = b;
            }
        }

        private void BackgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            this.ToolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Image? iOld = this.PictureBox1.Image;

                this.PictureBox1.Image = (Bitmap)e.Result;

                if (iOld != null)
                {
                    iOld.Dispose();
                    iOld = null;
                }

                this.PictureBox1.Refresh();

                this.ToolStripProgressBar1.Value = 0;
                this.ToolStripProgressBar1.Visible = false;
            }

            this.SplitContainer1.Panel1.Enabled = true;
        }

        private void CheckBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox15.Checked)
                this.SplitContainer1.Panel2.BackColor = SystemColors.ControlDarkDark;
            else
                this.SplitContainer1.Panel2.BackColor = SystemColors.Control;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.colorDialog1.CustomColors = CustomColors;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color c = this.colorDialog1.Color;
                this.label2.BackColor = Color.FromArgb(System.Convert.ToInt32(this.numericUpDown4.Value), c.R, c.G, c.B);
                CustomColors = this.colorDialog1.CustomColors;

                SetValues();
            }
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Color c = this.label2.BackColor;
            this.label2.BackColor = Color.FromArgb(System.Convert.ToInt32(this.numericUpDown4.Value), c.R, c.G, c.B);
        }

        private void numericUpDown4_ValueChanged_1(object sender, EventArgs e)
        {
            SetValues();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckBox3.Enabled = this.checkBox1.Checked;
            SetValues();
        }

        private void PictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.PictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
            {
                this.PictureBox1.Location = new Point(0, 0);
                this.PictureBox1.Dock = DockStyle.None;
                this.PictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            else
            {
                this.PictureBox1.Dock = DockStyle.Fill;
                this.PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            this._r = GetImageRectangle();
        }

        private void bitmapPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_r != null && this._bmp != null && e.X >= _r.Value.X && e.Y >= _r.Value.Y && e.X < (_r.Value.X + _r.Value.Width) && e.Y < (_r.Value.Y + _r.Value.Height))
                {
                    int ix = System.Convert.ToInt32((e.X - _r.Value.X) / (double)_zoom);
                    int iy = System.Convert.ToInt32((e.Y - _r.Value.Y) / (double)_zoom);

                    if (_bmp != null)
                    {
                        if (ix > _bmp.Width - 1)
                            ix = _bmp.Width - 1;

                        if (iy > _bmp.Height - 1)
                            iy = _bmp.Height - 1;

                        ix = Math.Max(ix, 0);
                        iy = Math.Max(iy, 0);
                    }

                    this._Pt = new Point(ix, iy);
                }
            }
        }

        private void contentPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_r != null && this._bmp != null && e.X >= _r.Value.X && e.Y >= _r.Value.Y && e.X < (_r.Value.X + _r.Value.Width) && e.Y < (_r.Value.Y + _r.Value.Height))
            {
                int ix = System.Convert.ToInt32((e.X - _r.Value.X) / (double)_zoom);
                int iy = System.Convert.ToInt32((e.Y - _r.Value.Y) / (double)_zoom);

                if (_bmp != null)
                {
                    if (ix > _bmp.Width - 1)
                        ix = _bmp.Width - 1;

                    if (iy > _bmp.Height - 1)
                        iy = _bmp.Height - 1;

                    ix = Math.Max(ix, 0);
                    iy = Math.Max(iy, 0);


                    Color c = _bmp.GetPixel(ix, iy);
                    int g = System.Convert.ToInt32(c.R * 0.299 + c.G * 0.587 + c.B * 0.114);

                    ToolStripStatusLabel1.Text = "x: " + ix.ToString() + ", y: " + iy.ToString() + " " + "RGB: " + c.ToString() + " - Grayscale Value: " + g.ToString();

                    // for selection rect
                    if (this.checkBox1.Checked && e.Button == MouseButtons.Left)
                    {
                        this._Pt = new Point(ix, iy);
                        this.PictureBox1.Refresh();
                    }
                }
            }
        }

        private void contentPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (_r != null && this._bmp != null && e.X >= _r.Value.X && e.Y >= _r.Value.Y && e.X < (_r.Value.X + _r.Value.Width) && e.Y < (_r.Value.Y + _r.Value.Height))
            {
                try
                {
                    int x = System.Convert.ToInt32((e.X - _r.Value.X) / (double)_zoom);
                    int y = System.Convert.ToInt32((e.Y - _r.Value.Y) / (double)_zoom);
                    Color c = ((Bitmap)this.PictureBox1.Image).GetPixel(x, y);

                    this.label12.BackColor = c;
                    this.label13.Text = x.ToString() + "; " + y.ToString();
                    SetValues();

                    this.SplitContainer1.Panel2.Focus();
                }
                catch
                {
                }
            }
        }

        // get position of pic in piucturebox
        private Rectangle? GetImageRectangle()
        {
            Type pboxType = this.PictureBox1.GetType();

            if (pboxType != null)
            {
                System.Reflection.PropertyInfo? irProperty = pboxType.GetProperty("ImageRectangle", System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                Rectangle? r = null;

                if (irProperty != null)
                    r = (Rectangle?)irProperty.GetValue(this.PictureBox1, null);

                _zoom = System.Convert.ToSingle(r?.Width / (double)this.PictureBox1.Image.Width);

                if (r != null)
                    return r.Value;
            }

            return null;
        }

        private void PictureBox1_Layout(object sender, LayoutEventArgs e)
        {
            if (this.PictureBox1.Image != null)
                this._r = GetImageRectangle();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (this._bmp != null)
            {
                Image? iOld = this.PictureBox1.Image;

                this.PictureBox1.Image = (Bitmap)this._bmp.Clone();

                if (iOld != null)
                {
                    iOld.Dispose();
                    iOld = null;
                }

                this.PictureBox1.Refresh();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.PictureBox1.Image != null)
            {
                this.SplitContainer1.Panel1.Enabled = false;
                Bitmap bmp = (Bitmap)this.PictureBox1.Image.Clone();
                this.ToolStripProgressBar1.Visible = true;
                this.BackgroundWorker1.RunWorkerAsync(new object[] { bmp });
            }
        }

        private void frmFloodClear_Shown(object sender, EventArgs e)
        {
            Color c = this.colorDialog1.Color;
            this.label2.BackColor = Color.FromArgb(System.Convert.ToInt32(this.numericUpDown4.Value), c.R, c.G, c.B);

            SetValues();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.NumericUpDown1.Enabled = false;

            if (comboBox1.SelectedIndex == 4)
                this.NumericUpDown1.Enabled = true;

            SetValues();
        }

        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckBox4.Enabled = this.CheckBox3.Checked;
            this._useCurves = this.CheckBox3.Checked;
        }

        private void CheckBox4_CheckedChanged(object sender, EventArgs e)
        {
            this._useClosedCurve = this.CheckBox4.Checked;
        }
    }
}
