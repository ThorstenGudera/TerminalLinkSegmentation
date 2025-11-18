using ChainCodeFinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace QuickExtract2
{
    public partial class frmOutlineShift : Form
    {
        private Bitmap? _bmp = null;
        private Bitmap? _bmpBU = null;
        private List<List<PointF>>? _cP = null;
        private Random _rnd = new Random();
        private List<ChainCode>? _fList;
        private Rectangle? _r;
        private float _zoom = 1.0F;
        private GraphicsPath? _aLF2 = null;
        private float _shiftOutwards;
        private int _precisionForRounding = 4;

        private bool _fPsf = false;
        private bool _addHoles = false;

        private const int _resampleAmount = 2;
        // prolongate the smaller array in ShiftCoords
        private bool _stretchPath = false;

        private int _ptX;
        private int _ptY;
        private int _ptx2;
        private int _ptY2;

        private ShiftPathMode _shiftPathMode = ShiftPathMode.FillPath;
        private ShiftFractionMode _shiftFractionMode = ShiftFractionMode.Closest;
        private bool _addHalfForDrawing = true;
        //private GraphicsPath _aLF2BU;
        private GraphicsPath? _curPathFigure;
        private GraphicsPath? _frmfPathClone;

        public GraphicsPath? ALF2
        {
            get
            {
                if (this._aLF2 != null)
                    return (GraphicsPath)this._aLF2.Clone();

                return null;
            }
        }

        public frmOutlineShift(Bitmap bmp)
        {
            InitializeComponent();

            this._bmp = bmp;
            this._bmpBU = (Bitmap)this._bmp.Clone();

            this.PictureBox1.Image = (Bitmap)this._bmp.Clone();
            this._r = GetImageRectangle();

            this.comboBox1.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
        }

        public frmOutlineShift(Bitmap bmp, List<List<PointF>> cP)
        {
            InitializeComponent();

            this._bmp = bmp;
            this._bmpBU = (Bitmap)this._bmp.Clone();
            this._cP = cP;

            if (this._cP != null && this._cP.Count > 0)
            {
                for (int i = 0; i <= this._cP.Count - 1; i++)
                {
                    if (this._cP[i].Count > 2)
                    {
                        this.Button15.Enabled = true;
                        break;
                    }
                }
            }

            this.PictureBox1.Image = (Bitmap)this._bmp.Clone();
            this._r = GetImageRectangle();

            this.comboBox1.SelectedIndex = 0;
            this.comboBox3.SelectedIndex = 0;
            this.comboBox4.SelectedIndex = 0;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.PictureBox1.Image != null)
            {
                this.PictureBox1.Image.Dispose();
                this.PictureBox1.Image = null;
            }
        }

        // create a new picture with some random ellipses in it
        private void ButtonNewImg_Click(object sender, EventArgs e)
        {
            Bitmap? bmp = null;
            Bitmap? bmp2 = null;

            if (AvailMem.AvailMem.checkAvailRam(this.PictureBox1.ClientSize.Width * this.PictureBox1.ClientSize.Height * 4L))
            {
                bmp = new Bitmap(this.PictureBox1.ClientSize.Width, this.PictureBox1.ClientSize.Height);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    int n = _rnd.Next(1001);

                    if ((n & 0x1) != 1)
                    {
                        Color bg = Color.FromArgb(255, _rnd.Next(256), _rnd.Next(256), _rnd.Next(256));
                        g.Clear(bg);
                    }

                    for (int i = 0; i <= 2; i++)
                    {
                        Rectangle r = new Rectangle(_rnd.Next(bmp.Width / 4), _rnd.Next(bmp.Width / 4), _rnd.Next(bmp.Width / 2), _rnd.Next(bmp.Width / 2));
                        Color c = Color.FromArgb(255, _rnd.Next(256), _rnd.Next(256), _rnd.Next(256));

                        using (SolidBrush sb = new SolidBrush(c))
                        {
                            g.FillEllipse(sb, r);
                        }

                        if (this._addHoles)
                        {
                            r.Inflate(-r.Width / 4, -r.Height / 4);

                            using (GraphicsPath gP = new GraphicsPath())
                            {
                                gP.AddEllipse(r);
                                g.SetClip(gP);
                                g.Clear(Color.Transparent);
                                g.ResetClip();
                            }
                        }
                    }
                }

                if (_fPsf)
                {
                    for (int i = 0; i <= _resampleAmount; i++)
                    {
                        Bitmap? bOld1 = bmp2;
                        if (AvailMem.AvailMem.checkAvailRam(System.Convert.ToInt64(this.PictureBox1.ClientSize.Width * this.PictureBox1.ClientSize.Height)))
                        {
                            bmp2 = new Bitmap(this.PictureBox1.ClientSize.Width / 2, this.PictureBox1.ClientSize.Height / 2);

                            using (Graphics g = Graphics.FromImage(bmp2))
                            {
                                g.InterpolationMode = InterpolationMode.Bilinear;
                                g.DrawImage(bmp, 0, 0, bmp2.Width, bmp2.Height);
                            }
                        }
                        if (bOld1 != null)
                        {
                            bOld1.Dispose();
                            bOld1 = null;
                        }

                        Bitmap? bOld2 = bmp;
                        if (AvailMem.AvailMem.checkAvailRam(this.PictureBox1.ClientSize.Width * this.PictureBox1.ClientSize.Height * 4L))
                        {
                            bmp = new Bitmap(this.PictureBox1.ClientSize.Width, this.PictureBox1.ClientSize.Height);

                            if (bmp2 != null)
                                using (Graphics g = Graphics.FromImage(bmp))
                                {
                                    g.InterpolationMode = InterpolationMode.Bilinear;
                                    g.DrawImage(bmp2, 0, 0, bmp.Width, bmp.Height);
                                }
                        }
                        if (bOld2 != null)
                        {
                            bOld2.Dispose();
                            bOld2 = null;
                        }
                    }
                    if (bmp2 != null)
                    {
                        bmp2.Dispose();
                        bmp2 = null;
                    }
                }

                Bitmap? bOld = this._bmp;
                _bmp = bmp; // CType(bmp.Clone(), Bitmap)
                if (bOld != null)
                {
                    bOld.Dispose();
                    bOld = null;
                }
                Bitmap? bOldBU = this._bmpBU;
                _bmpBU = (Bitmap)bmp.Clone();
                if (bOldBU != null)
                {
                    bOldBU.Dispose();
                    bOldBU = null;
                }
                Image? iOld = this.PictureBox1.Image;
                this.PictureBox1.Image = (Bitmap)bmp.Clone();
                this._r = GetImageRectangle();
                if (iOld != null)
                {
                    iOld.Dispose();
                    iOld = null;
                }

                _fList = new List<ChainCode>();
                //_outlineDone = false;

                GraphicsPath? alfOld = _aLF2;
                _aLF2 = new GraphicsPath();
                if (alfOld != null)
                {
                    alfOld.Dispose();
                    alfOld = null;
                }
                this.Button9.Enabled = false;

                Image? iOld2 = this.PictureBox2.Image;
                this.PictureBox2.Image = null;
                if (iOld2 != null)
                {
                    iOld2.Dispose();
                    iOld2 = null;
                }
                Image? iOld3 = this.PictureBox3.Image;
                this.PictureBox3.Image = null;
                if (iOld3 != null)
                {
                    iOld3.Dispose();
                    iOld3 = null;
                }

                this.ToolStripStatusLabel1.Text = "";
            }
        }

        // get the outline
        private void button6_Click(object sender, EventArgs e)
        {
            if (_bmp != null)
            {
                Bitmap? bmpTmp = null;

                try
                {
                    bool b = this.Button2.Enabled;
                    this.SetControls(false);

                    if (AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                        bmpTmp = (Bitmap)_bmp.Clone();
                    else
                    {
                        MessageBox.Show("Not enough Memory");
                        return;
                    }

                    //_outlineDone = false;

                    ChainFinder fbmp = new ChainFinder();
                    fbmp.ProgressPlus += fipbmp_ProgressPlus;
                    this.label3.Text = "working...";

                    int threshold = System.Convert.ToInt32(this.numericUpDown1.Value);

                    List<ChainCode> fList = new List<ChainCode>();

                    if (this.comboBox3.SelectedIndex == 1)
                        fbmp.GrayScaleImage(bmpTmp);

                    if (this.comboBox4.SelectedIndex == 0)
                        fList = fbmp.GetOutline(bmpTmp, threshold, (this.comboBox3.SelectedIndex == 1), 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), false);
                    else
                        fList = fbmp.GetOutline(bmpTmp, threshold, (this.comboBox3.SelectedIndex == 1), 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), true);

                    if (fList.Count > 0)
                    {
                        // remove 1px chains
                        for (int i = fList.Count - 1; i >= 0; i += -1)
                        {
                            if (fList[i].Coord.Count <= 4)
                                fList.RemoveAt(i);
                        }

                        fList = fList.OrderBy(a => a.Coord.Count).ToList();
                        fList.Reverse();

                        int maxChains = System.Convert.ToInt32(this.NumericUpDown2.Value);

                        // -1 is "no limit"
                        if (maxChains == -1)
                            maxChains = Int32.MaxValue;

                        if (fList.Count > maxChains)
                        {
                            if (MessageBox.Show("The outlineslist contains more than " + maxChains + " chains. Shall only the first " + maxChains + " chains be taken?", "Path may be slowly drawn...", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                fList.RemoveRange(maxChains, fList.Count - maxChains);
                        }
                    }

                    fbmp.ProgressPlus -= fipbmp_ProgressPlus;
                    this.label3.Text = "done.";

                    this._fList = fList;

                    //_outlineDone = true;

                    GetPicture2();

                    this.SetControls(true);
                    this.Button2.Enabled = b;
                    this.PictureBox1.Refresh();

                    this.Button12.Enabled = true;
                }
                finally
                {
                    if (bmpTmp != null)
                    {
                        bmpTmp.Dispose();
                        bmpTmp = null;
                    }
                }
            }
        }

        // picture for picbox2 -> unshifted path
        private void GetPicture2()
        {
            using (GraphicsPath fPath = GetPath(this.comboBox1.SelectedIndex, System.Convert.ToDouble(this.numericUpDown7.Value), this.CheckBox18.Checked, this.checkBox2.Checked, this.checkBox3.Checked, this.checkBox7.Checked, this.checkBox4.Checked, this.checkBox5.Checked))
            {
                if (fPath != null)
                {
                    GraphicsPath? alfOld = _aLF2;
                    _aLF2 = (GraphicsPath)fPath.Clone();
                    if (alfOld != null)
                    {
                        alfOld.Dispose();
                        alfOld = null;
                    }
                    this.Button9.Enabled = true;
                }
            }

            Image? iOld = this.PictureBox2.Image;

            if (this._bmp != null)
                this.PictureBox2.Image = GetFilledPathPicf(this._bmp);

            if (iOld != null)
            {
                iOld.Dispose();
                iOld = null;
            }
        }

        private Image? GetFilledPathPicf(Bitmap bmp)
        {
            Bitmap? bmpOut = null;
            if (_bmp != null && !this.IsDisposed && this._aLF2 != null && this._aLF2.PointCount > 0)
            {
                try
                {
                    if (AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                        bmpOut = new Bitmap(_bmp.Width, _bmp.Height);
                    else
                    {
                        MessageBox.Show("Not enough Memory");
                        return null;
                    }

                    using (Graphics g = Graphics.FromImage(bmpOut))
                    {
                        if (this.CheckBox13.Checked)
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                        using (TextureBrush tb = new TextureBrush(bmp))
                        {
                            if (_addHalfForDrawing)
                                tb.TranslateTransform(-0.5F, -0.5F);
                            g.FillPath(tb, this._aLF2);
                        }
                    }
                }
                catch
                {
                    if (bmpOut != null)
                    {
                        bmpOut.Dispose();
                        bmpOut = null;
                    }
                }
            }

            return bmpOut;
        }

        private void fipbmp_ProgressPlus(object sender, ProgEventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    IncrementLabelText(e);
                }));
            }
            else
                IncrementLabelText(e);
        }

        private void IncrementLabelText(ProgEventArgs e)
        {
            this.label3.Text = "Current y = " + e.Y.ToString() + " of " + e.Height.ToString();
            this.label3.Refresh();
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.SplitContainer1.Panel1.Controls)
                    {
                        if (ct is Button || ct is ComboBox)
                            ct.Enabled = e;
                    }

                    this.SplitContainer2.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.SplitContainer1.Panel1.Controls)
                {
                    if (ct is Button || ct is ComboBox)
                        ct.Enabled = e;
                }

                this.SplitContainer2.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void CheckBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckBox15.Checked)
            {
                this.SplitContainer2.Panel1.BackColor = SystemColors.ControlDarkDark;
                this.SplitContainer3.Panel1.BackColor = SystemColors.ControlDarkDark;
                this.SplitContainer3.Panel2.BackColor = SystemColors.ControlDarkDark;
            }
            else
            {
                this.SplitContainer2.Panel1.BackColor = SystemColors.Control;
                this.SplitContainer3.Panel1.BackColor = SystemColors.Control;
                this.SplitContainer3.Panel2.BackColor = SystemColors.Control;
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_bmp != null)
            {
                Graphics g = e.Graphics;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                if (this._curPathFigure != null && this._curPathFigure.PointCount > 1 && _r != null)
                {
                    using (Pen pen = new Pen(Color.Yellow, 2))
                    {
                        RectangleF r = this._curPathFigure.GetBounds();

                        using (GraphicsPath gP = (GraphicsPath)this._curPathFigure.Clone())
                        {
                            // zoom
                            using (Matrix mx = new Matrix(1, 0, 0, 1, -r.X, -r.Y))
                            {
                                gP.Transform(mx);
                            }
                            using (Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0))
                            {
                                gP.Transform(mx);
                            }
                            using (Matrix mx = new Matrix(1, 0, 0, 1, r.X * _zoom, r.Y * _zoom))
                            {
                                gP.Transform(mx);
                            }
                            // imageposition in picbox
                            using (Matrix mx = new Matrix(1, 0, 0, 1, _r.Value.X, _r.Value.Y))
                            {
                                gP.Transform(mx);
                            }
                            using (SolidBrush sb = new SolidBrush(Color.FromArgb(64, Color.Blue)))
                            {
                                e.Graphics.FillPath(sb, gP);
                                e.Graphics.DrawPath(Pens.White, gP);
                            }

                            e.Graphics.DrawPath(pen, gP);
                            return;
                        }
                    }
                }

                // draw Rect?
                if (this.CheckBox1.Checked)
                {
                    // if, draw the rect
                    using (Pen pen = new Pen(Color.Aqua, 2))
                    {
                        // e.Graphics.DrawImage(_bmp, _r)
                        using (GraphicsPath hChain = new GraphicsPath())
                        {
                            if (this._fList != null && _r != null)
                            {
                                for (int i = 0; i <= this._fList.Count - 1; i++)
                                {
                                    hChain.StartFigure();
                                    hChain.AddLines(this._fList[i].Coord.ToArray());
                                    hChain.CloseFigure();
                                }

                                RectangleF r = hChain.GetBounds();

                                using (GraphicsPath gP = (GraphicsPath)hChain.Clone())
                                {
                                    // zoom
                                    using (Matrix mx = new Matrix(1, 0, 0, 1, -r.X, -r.Y))
                                    {
                                        gP.Transform(mx);
                                    }
                                    using (Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0))
                                    {
                                        gP.Transform(mx);
                                    }
                                    using (Matrix mx = new Matrix(1, 0, 0, 1, r.X * _zoom, r.Y * _zoom))
                                    {
                                        gP.Transform(mx);
                                    }
                                    // imageposition in picbox
                                    using (Matrix mx = new Matrix(1, 0, 0, 1, _r.Value.X, _r.Value.Y))
                                    {
                                        gP.Transform(mx);
                                    }
                                    using (SolidBrush sb = new SolidBrush(Color.FromArgb(64, Color.Blue)))
                                    {
                                        e.Graphics.FillPath(sb, gP);
                                        e.Graphics.DrawPath(Pens.White, gP);
                                    }
                                }

                                if (this._aLF2 != null && this._aLF2.PointCount > 1)
                                {
                                    using (GraphicsPath gP = (GraphicsPath)this._aLF2.Clone())
                                    {
                                        // zoom
                                        using (Matrix mx = new Matrix(1, 0, 0, 1, -r.X, -r.Y))
                                        {
                                            gP.Transform(mx);
                                        }
                                        using (Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0))
                                        {
                                            gP.Transform(mx);
                                        }
                                        using (Matrix mx = new Matrix(1, 0, 0, 1, r.X * _zoom, r.Y * _zoom))
                                        {
                                            gP.Transform(mx);
                                        }
                                        // imageposition in picbox
                                        using (Matrix mx = new Matrix(1, 0, 0, 1, _r.Value.X, _r.Value.Y))
                                        {
                                            gP.Transform(mx);
                                        }
                                        using (Pen p = new Pen(Color.Aqua, 2))
                                        {
                                            e.Graphics.DrawPath(p, gP);
                                        }
                                    }
                                }
                            }
                        }
                        if (_r != null)
                            e.Graphics.DrawRectangle(pen, new Rectangle(System.Convert.ToInt32(_ptX * _zoom) + _r.Value.X, System.Convert.ToInt32(_ptY * _zoom) + _r.Value.Y, System.Convert.ToInt32((_ptx2 - _ptX) * _zoom), System.Convert.ToInt32((_ptY2 - _ptY) * _zoom)));
                    }
                }
                else
                    // else draw the current path and bounding rect (transform by zoom and translate also by position of pic in picturebox _> rectangle Me._r)
                    if (this._fList != null && this._fList.Count > 0)
                {
                    // e.Graphics.DrawImage(_bmp, _r)
                    using (GraphicsPath hChain = new GraphicsPath())
                    {
                        if (this._fList != null && _r != null)
                        {
                            for (int i = 0; i <= this._fList.Count - 1; i++)
                            {
                                hChain.StartFigure();
                                hChain.AddLines(this._fList[i].Coord.ToArray());
                                hChain.CloseFigure();
                            }

                            RectangleF r = hChain.GetBounds();

                            using (GraphicsPath gP = (GraphicsPath)hChain.Clone())
                            {
                                // zoom
                                using (Matrix mx = new Matrix(1, 0, 0, 1, -r.X, -r.Y))
                                {
                                    gP.Transform(mx);
                                }
                                using (Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0))
                                {
                                    gP.Transform(mx);
                                }
                                using (Matrix mx = new Matrix(1, 0, 0, 1, r.X * _zoom, r.Y * _zoom))
                                {
                                    gP.Transform(mx);
                                }
                                // imageposition in picbox
                                using (Matrix mx = new Matrix(1, 0, 0, 1, _r.Value.X, _r.Value.Y))
                                {
                                    gP.Transform(mx);
                                }
                                using (SolidBrush sb = new SolidBrush(Color.FromArgb(64, Color.Blue)))
                                {
                                    e.Graphics.FillPath(sb, gP);
                                    e.Graphics.DrawPath(Pens.White, gP);
                                }
                            }

                            using (Pen p = new Pen(Color.White))
                            {
                                e.Graphics.DrawRectangle(p, r.X * _zoom + _r.Value.X, r.Y * _zoom + _r.Value.Y, r.Width * _zoom, r.Height * _zoom);
                            }

                            if (this._aLF2 != null && this._aLF2.PointCount > 1)
                            {
                                using (GraphicsPath gP = (GraphicsPath)this._aLF2.Clone())
                                {
                                    // zoom
                                    using (Matrix mx = new Matrix(1, 0, 0, 1, -r.X, -r.Y))
                                    {
                                        gP.Transform(mx);
                                    }
                                    using (Matrix mx = new Matrix(_zoom, 0, 0, _zoom, 0, 0))
                                    {
                                        gP.Transform(mx);
                                    }
                                    using (Matrix mx = new Matrix(1, 0, 0, 1, r.X * _zoom, r.Y * _zoom))
                                    {
                                        gP.Transform(mx);
                                    }
                                    // imageposition in picbox
                                    using (Matrix mx = new Matrix(1, 0, 0, 1, _r.Value.X, _r.Value.Y))
                                    {
                                        gP.Transform(mx);
                                    }
                                    using (Pen p = new Pen(Color.Aqua, 2))
                                    {
                                        e.Graphics.DrawPath(p, gP);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // get position of pic in picturebox
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.numericUpDown7.Enabled = false;

            if (comboBox1.SelectedIndex == 4)
                this.numericUpDown7.Enabled = true;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
                comboBox1.Enabled = true;
            else
                comboBox1.Enabled = false;
        }

        private void Button17_Click(object sender, EventArgs e)
        {
            using (frmShiftOutwards frm = new frmShiftOutwards(this._shiftPathMode, this._shiftFractionMode))
            {
                frm.NumericUpDown1.Value = System.Convert.ToDecimal(this._shiftOutwards);
                frm.ComboBox1.SelectedIndex = System.Convert.ToInt32(this._shiftPathMode);
                frm.ComboBox2.SelectedIndex = System.Convert.ToInt32(this._shiftFractionMode);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    this._shiftOutwards = System.Convert.ToSingle(frm.NumericUpDown1.Value);
                    string? s = frm.ComboBox1.SelectedItem?.ToString();
                    if (s != null)
                        this._shiftPathMode = (ShiftPathMode)System.Enum.Parse(typeof(ShiftPathMode), s);
                    string? s2 = frm.ComboBox2.SelectedItem?.ToString();
                    if (s2 != null)
                        this._shiftFractionMode = (ShiftFractionMode)System.Enum.Parse(typeof(ShiftFractionMode), s2);
                }
            }
        }

        // picture for picbox3 -> unshifted path
        private void GetPicture3()
        {
            Image? iOld = this.PictureBox3.Image;

            this.PictureBox3.Image = GetFilledPathPic2f(this._bmp);

            this.Button2.Enabled = true;

            if (iOld != null)
            {
                iOld.Dispose();
                iOld = null;
            }
        }

        private Image? GetFilledPathPic2f(Bitmap? bmp)
        {
            Bitmap? bmpOut = null;
            if (!this.IsDisposed && this._aLF2 != null && this._aLF2.PointCount > 0)
            {
                try
                {
                    if (_bmp != null && AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                        bmpOut = new Bitmap(_bmp.Width, _bmp.Height);
                    else
                    {
                        MessageBox.Show("Not enough Memory");
                        return null;
                    }

                    using (Graphics g = Graphics.FromImage(bmpOut))
                    {
                        if (this.CheckBox13.Checked)
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                        if (bmp != null)
                            using (TextureBrush tb = new TextureBrush(bmp))
                            {
                                if (_addHalfForDrawing)
                                    tb.TranslateTransform(-0.5F, -0.5F);
                                g.FillPath(tb, this._aLF2);
                            }
                    }
                }
                catch
                {
                    if (bmpOut != null)
                    {
                        bmpOut.Dispose();
                        bmpOut = null;
                    }
                }
            }

            return bmpOut;
        }

        private void frm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            this.PictureBox1.Refresh();
        }

        // get a graphicsPath from our current chaincode
        // and process it as set by user
        private GraphicsPath GetPath(int comboBox1SelectedIndex, double numericUpDown7Value, bool CheckBox18Checked, bool checkBox2Checked, bool checkBox3Checked, bool checkBox7Checked, bool checkBox4Checked, bool checkBox5Checked)
        {
            GraphicsPath zPath = new GraphicsPath(); // large path
            ChainFinder fBmp = new ChainFinder();

            // max allowed distance for approximating lines
            double epsilon = Math.Sqrt(2.0) / 2.0;

            switch (comboBox1SelectedIndex)
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
                        epsilon = System.Convert.ToDouble(numericUpDown7Value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            int sign = -1;
            // loop over all chains

            for (int i = 0; i <= this._fList?.Count - 1; i++)
            {
                sign = -1;
                GraphicsPath? fPath = new GraphicsPath();
                using (GraphicsPath gPath = new GraphicsPath()) // path for current chain
                {
                    if (this._fList != null)
                    {
                        ChainCode c = (ChainCode)this._fList[i];
                        if (IsInnerOutline(c))
                            sign = 1;

                        List<Point> lList = c.Coord;

                        bool rlp2 = false;
                        if (lList.Count > 0 && checkBox4Checked && lList[lList.Count - 1] == lList[0])
                        {
                            lList.RemoveAt(lList.Count - 1);
                            rlp2 = true;
                        }

                        bool onlyLargest = false;
                        if (CheckBox18Checked)
                        {
                            this._fList = this._fList.OrderByDescending(a => a.Coord.Count).ToList();
                            onlyLargest = true;
                        }

                        // not shifted -> only apply cleaning, approximating lines etc.
                        if (CheckShift() == false)
                        {
                            if (checkBox2Checked)
                            {
                                if (checkBox3Checked)
                                {
                                    List<Point> fList = c.Coord;

                                    fList = fBmp.RemoveColinearity(fList, true, this._precisionForRounding);

                                    if (checkBox7Checked)
                                        fList = fBmp.ApproximateLines(fList, epsilon);

                                    // If fList.Count > 0 AndAlso checkBox4Checked AndAlso fList(fList.Count - 1) <> fList(0) Then 'close
                                    // fList.Add(fList(0))
                                    // End If

                                    if (checkBox5Checked)
                                    {
                                        if (fList.Count > 2)
                                        {
                                            bool rlp = false;
                                            if (fList[fList.Count - 1].X == fList[0].X && fList[fList.Count - 1].Y == fList[0].Y)
                                            {
                                                fList.RemoveAt(fList.Count - 1);
                                                rlp = true;
                                            }
                                            gPath.AddClosedCurve(fList.ToArray());
                                            if (rlp)
                                                fList.Add(fList[0]);
                                        }
                                        else if (fList.Count > 1)
                                            gPath.AddLines(fList.ToArray());
                                        else
                                        {
                                            Point p = fList[0];
                                            gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                    else if (fList.Count > 1)
                                        gPath.AddCurve(fList.ToArray());
                                    else
                                    {
                                        Point p = fList[0];
                                        gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                    }
                                }
                                else
                                {
                                    if (checkBox7Checked)
                                        lList = fBmp.ApproximateLines(lList, epsilon);

                                    // If lList.Count > 0 AndAlso checkBox4Checked AndAlso lList(lList.Count - 1) <> lList(0) Then
                                    // lList.Add(lList(0))
                                    // End If

                                    if (checkBox5Checked)
                                    {
                                        if (lList.Count > 2)
                                        {
                                            bool rlp = false;
                                            if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                                            {
                                                lList.RemoveAt(lList.Count - 1);
                                                rlp = true;
                                            }
                                            gPath.AddClosedCurve(lList.ToArray());
                                            if (rlp)
                                                lList.Add(lList[0]);
                                        }
                                        else if (lList.Count > 1)
                                            gPath.AddLines(lList.ToArray());
                                        else
                                        {
                                            Point p = lList[0];
                                            gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                    else if (lList.Count > 1)
                                        gPath.AddCurve(lList.ToArray());
                                    else
                                    {
                                        Point p = lList[0];
                                        gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                    }
                                }
                            }
                            else
                            {
                                if (checkBox3Checked)
                                {
                                    List<Point> fList = c.Coord;

                                    fList = fBmp.RemoveColinearity(fList, true, this._precisionForRounding);

                                    if (checkBox7Checked)
                                        fList = fBmp.ApproximateLines(fList, epsilon);

                                    gPath.AddLines(fList.ToArray());
                                }
                                else
                                {
                                    if (checkBox7Checked)
                                        lList = fBmp.ApproximateLines(lList, epsilon);

                                    gPath.AddLines(lList.ToArray());
                                }

                                if (checkBox4Checked)
                                    gPath.CloseFigure();
                            }
                        }
                        else
                            gPath.AddLines(lList.ToArray());

                        if (onlyLargest)
                            i = this._fList.Count;

                        if (gPath != null)
                        {
                            try
                            {
                                if (gPath.PathPoints.Length > 1)
                                {
                                    if (i > 0 && this.checkBox8.Checked)
                                        fPath.AddPath(gPath, true);
                                    else
                                        fPath.AddPath(gPath, false);
                                }
                                else
                                {
                                    PointF p = gPath.PathPoints[0];
                                    fPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                }
                            }
                            catch
                            {
                            }
                        }

                        if (rlp2)
                            lList.Add(lList[0]);
                    }

                }

                if (CheckShift())
                {
                    ChainFinder c2 = new ChainFinder();
                    GraphicsPathData gPath = c2.ShiftCoords(fPath, this._shiftOutwards * sign, this.CheckBox18.Checked, _stretchPath, _shiftFractionMode); // get shifted path
                    GraphicsPath gPath4 = new GraphicsPath(); // large path for current block

                    if (gPath != null && gPath.AllPoints != null && gPath.AllPoints.Count > 0)
                    {
                        for (int cnt = 0; cnt <= gPath.AllPoints.Count - 1; cnt++) // loop over each subpath
                        {
                            List<PointF> lList = gPath.AllPoints[cnt];

                            bool rlp2 = false;
                            if (lList.Count > 0 && checkBox4Checked && lList[lList.Count - 1] == lList[0])
                            {
                                lList.RemoveAt(lList.Count - 1);
                                rlp2 = true;
                            }

                            using (GraphicsPath gPath2 = new GraphicsPath()) // path for current subpath
                            {
                                // now process the shifted path just as above
                                if (checkBox2Checked)
                                {
                                    if (checkBox3Checked)
                                    {
                                        lList = fBmp.RemoveColinearity(lList, true, this._precisionForRounding);

                                        if (checkBox7Checked)
                                            lList = fBmp.ApproximateLines(lList, epsilon);

                                        // If lList.Count > 0 AndAlso checkBox4Checked AndAlso lList(lList.Count - 1) <> lList(0) Then
                                        // lList.Add(lList(0))
                                        // End If

                                        if (checkBox5Checked)
                                        {
                                            if (lList.Count > 2)
                                            {
                                                bool rlp = false;
                                                if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                                                {
                                                    lList.RemoveAt(lList.Count - 1);
                                                    rlp = true;
                                                }
                                                gPath2.AddClosedCurve(lList.ToArray());
                                                if (rlp)
                                                    lList.Add(lList[0]);
                                            }
                                            else if (lList.Count > 1)
                                                gPath2.AddLines(lList.ToArray());
                                            else
                                            {
                                                PointF p = lList[0];
                                                gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                            }
                                        }
                                        else if (lList.Count > 1)
                                            gPath2.AddCurve(lList.ToArray());
                                        else
                                        {
                                            PointF p = lList[0];
                                            gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                    else
                                    {
                                        if (checkBox7Checked)
                                            lList = fBmp.ApproximateLines(lList, epsilon);

                                        // If lList.Count > 0 AndAlso checkBox4Checked AndAlso lList(lList.Count - 1) <> lList(0) Then
                                        // lList.Add(lList(0))
                                        // End If

                                        if (checkBox5Checked)
                                        {
                                            if (lList.Count > 2)
                                            {
                                                bool rlp = false;
                                                if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                                                {
                                                    lList.RemoveAt(lList.Count - 1);
                                                    rlp = true;
                                                }
                                                gPath2.AddClosedCurve(lList.ToArray());
                                                if (rlp)
                                                    lList.Add(lList[0]);
                                            }
                                            else if (lList.Count > 1)
                                                gPath2.AddLines(lList.ToArray());
                                            else
                                            {
                                                PointF p = lList[0];
                                                gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                            }
                                        }
                                        else if (lList.Count > 1)
                                            gPath2.AddCurve(lList.ToArray());
                                        else
                                        {
                                            PointF p = lList[0];
                                            gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                }
                                else
                                {
                                    if (checkBox3Checked)
                                    {
                                        lList = fBmp.RemoveColinearity(lList, true, this._precisionForRounding);

                                        if (checkBox7Checked)
                                            lList = fBmp.ApproximateLines(lList, epsilon);

                                        gPath2.AddLines(lList.ToArray());
                                    }
                                    else
                                    {
                                        if (checkBox7Checked)
                                            lList = fBmp.ApproximateLines(lList, epsilon);

                                        gPath2.AddLines(lList.ToArray());
                                    }

                                    if (checkBox4Checked)
                                        gPath2.CloseFigure();
                                }

                                if (gPath2.PointCount > 1)
                                {
                                    gPath2.FillMode = FillMode.Alternate;
                                    gPath4.AddPath(gPath2, false); // add to large (shifted) path
                                }

                                if (rlp2)
                                {
                                }
                            }

                            if (rlp2)
                                lList.Add(lList[0]);
                        }

                        if (gPath4 != null && gPath4.PointCount > 1)
                        {
                            if (fPath != null)
                            {
                                fPath.Dispose();
                                fPath = null;
                            }

                            fPath = gPath4; // set as reult
                        }
                    }
                }

                if (fPath != null && fPath.PointCount > 1)
                    zPath.AddPath(fPath, false);
            }
            return zPath; // result
        }

        private bool IsInnerOutline(ChainCode c)
        {
            return c.Chain[c.Chain.Count - 1] == 0;
        }

        // same as above, just uses a different technique for shifting the path
        private GraphicsPath GetPath2(int comboBox1SelectedIndex, double numericUpDown7Value, bool CheckBox18Checked, bool checkBox2Checked, bool checkBox3Checked, bool checkBox7Checked, bool checkBox4Checked, bool checkBox5Checked)
        {
            GraphicsPath? fPath = new GraphicsPath();
            ChainFinder fBmp = new ChainFinder();

            double epsilon = Math.Sqrt(2.0) / 2.0;

            switch (comboBox1SelectedIndex)
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
                        epsilon = System.Convert.ToDouble(numericUpDown7Value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            GraphicsPath fP = new GraphicsPath();
            if (this._fList != null)
                for (int i = 0; i <= this._fList.Count - 1; i++)
                {
                    int sign = -1;
                    bool onlyLargest = false;
                    using (GraphicsPath gPath = new GraphicsPath())
                    {
                        ChainCode c = (ChainCode)this._fList[i];
                        if (IsInnerOutline(c))
                            sign = 1;
                        if (this.CheckBox18.Checked)
                        {
                            this._fList = this._fList.OrderByDescending(a => a.Coord.Count).ToList();
                            onlyLargest = true;
                        }
                        List<Point> lList = c.Coord;

                        bool rlp2 = false;
                        if (lList.Count > 0 && checkBox4Checked && lList[lList.Count - 1] == lList[0])
                        {
                            lList.RemoveAt(lList.Count - 1);
                            rlp2 = true;
                        }

                        if (CheckShift() == false)
                        {
                            if (checkBox2Checked)
                            {
                                if (checkBox3Checked)
                                {
                                    List<Point> fList = c.Coord;
                                    fList = fBmp.RemoveColinearity(fList, true, this._precisionForRounding);

                                    if (checkBox7Checked)
                                        fList = fBmp.ApproximateLines(fList, epsilon);

                                    // If fList.Count > 0 AndAlso checkBox4Checked AndAlso fList(fList.Count - 1) <> fList(0) Then
                                    // fList.Add(fList(0))
                                    // End If

                                    if (checkBox5Checked)
                                    {
                                        if (fList.Count > 2)
                                        {
                                            bool rlp = false;
                                            if (fList[fList.Count - 1].X == fList[0].X && fList[fList.Count - 1].Y == fList[0].Y)
                                            {
                                                fList.RemoveAt(fList.Count - 1);
                                                rlp = true;
                                            }
                                            gPath.AddClosedCurve(fList.ToArray());
                                            if (rlp)
                                                fList.Add(fList[0]);
                                        }
                                        else if (fList.Count > 1)
                                            gPath.AddLines(fList.ToArray());
                                        else
                                        {
                                            Point p = fList[0];
                                            gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                    else if (fList.Count > 1)
                                        gPath.AddCurve(fList.ToArray());
                                    else
                                    {
                                        Point p = fList[0];
                                        gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                    }
                                }
                                else
                                {
                                    if (checkBox7Checked)
                                        lList = fBmp.ApproximateLines(lList, epsilon);

                                    // If lList.Count > 0 AndAlso checkBox4Checked AndAlso lList(lList.Count - 1) <> lList(0) Then
                                    // lList.Add(lList(0))
                                    // End If

                                    if (checkBox5Checked)
                                    {
                                        if (lList.Count > 2)
                                        {
                                            bool rlp = false;
                                            if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                                            {
                                                lList.RemoveAt(lList.Count - 1);
                                                rlp = true;
                                            }
                                            gPath.AddClosedCurve(lList.ToArray());
                                            if (rlp)
                                                lList.Add(lList[0]);
                                        }
                                        else if (lList.Count > 1)
                                            gPath.AddLines(lList.ToArray());
                                        else
                                        {
                                            Point p = lList[0];
                                            gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                    else if (lList.Count > 1)
                                        gPath.AddCurve(lList.ToArray());
                                    else
                                    {
                                        Point p = lList[0];
                                        gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                    }
                                }
                            }
                            else
                            {
                                if (checkBox3Checked)
                                {
                                    List<Point> fList = c.Coord;

                                    fList = fBmp.RemoveColinearity(fList, true, this._precisionForRounding);

                                    if (checkBox7Checked)
                                        fList = fBmp.ApproximateLines(fList, epsilon);

                                    gPath.AddLines(fList.ToArray());
                                }
                                else
                                {
                                    if (checkBox7Checked)
                                        lList = fBmp.ApproximateLines(lList, epsilon);

                                    gPath.AddLines(lList.ToArray());
                                }

                                if (checkBox4Checked)
                                    gPath.CloseFigure();
                            }

                            if (gPath != null)
                            {
                                try
                                {
                                    if (gPath.PathPoints.Length > 1)
                                    {
                                        if (i > 0 && this.checkBox8.Checked)
                                            fPath.AddPath(gPath, true);
                                        else
                                            fPath.AddPath(gPath, false);
                                    }
                                    else
                                    {
                                        PointF p = gPath.PathPoints[0];
                                        fPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                        else
                        {
                            gPath.AddLines(lList.ToArray());
                            fPath = (GraphicsPath)gPath.Clone();
                        }

                        if (onlyLargest)
                            i = this._fList.Count;

                        if (rlp2)
                            lList.Add(lList[0]);
                    }

                    if (CheckShift())
                    {
                        ChainFinder c2 = new ChainFinder();
                        GraphicsPath? gPath = c2.ShiftCoords2(fPath, this._shiftOutwards * sign, CheckBox18Checked, this._shiftFractionMode);
                        GraphicsPath? gPath4 = new GraphicsPath();

                        if (gPath != null && gPath.PointCount > 1)
                        {
                            List<List<PointF>> lL = c2.SplitPath(gPath);

                            for (int j = 0; j <= lL.Count - 1; j++)
                            {
                                bool rlp2 = false;
                                if (lL[j].Count > 0 && checkBox4Checked && lL[j][lL[j].Count - 1] == lL[j][0])
                                {
                                    lL[j].RemoveAt(lL[j].Count - 1);
                                    rlp2 = true;
                                }

                                using (GraphicsPath gPath2 = new GraphicsPath())
                                {
                                    if (checkBox2Checked)
                                    {
                                        if (checkBox3Checked)
                                        {
                                            lL[j] = fBmp.RemoveColinearity(lL[j], true, this._precisionForRounding);

                                            if (checkBox7Checked)
                                                lL[j] = fBmp.ApproximateLines(lL[j], epsilon);

                                            // If lL(j).Count > 0 AndAlso checkBox4Checked AndAlso lL(j)(lL(j).Count - 1) <> lL(j)(0) Then
                                            // lL(j).Add(lL(j)(0))
                                            // End If

                                            if (checkBox5Checked)
                                            {
                                                if (lL[j].Count > 2)
                                                {
                                                    bool rlp = false;
                                                    if (lL[j][lL[j].Count - 1].X == lL[j][0].X && lL[j][lL[j].Count - 1].Y == lL[j][0].Y)
                                                    {
                                                        lL[j].RemoveAt(lL[j].Count - 1);
                                                        rlp = true;
                                                    }
                                                    gPath2.AddClosedCurve(lL[j].ToArray());
                                                    if (rlp)
                                                        lL[j].Add(lL[j][0]);
                                                }
                                                else if (lL[j].Count > 1)
                                                    gPath2.AddLines(lL[j].ToArray());
                                                else
                                                {
                                                    PointF p = lL[j][0];
                                                    gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                                }
                                            }
                                            else if (lL[j].Count > 1)
                                                gPath2.AddCurve(lL[j].ToArray());
                                            else
                                            {
                                                PointF p = lL[j][0];
                                                gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                            }
                                        }
                                        else
                                        {
                                            if (checkBox7Checked)
                                                lL[j] = fBmp.ApproximateLines(lL[j], epsilon);

                                            // If lL(j).Count > 0 AndAlso checkBox4Checked AndAlso lL(j)(lL(j).Count - 1) <> lL(j)(0) Then
                                            // lL(j).Add(lL(j)(0))
                                            // End If

                                            if (checkBox5Checked)
                                            {
                                                if (lL[j].Count > 2)
                                                {
                                                    bool rlp = false;
                                                    if (lL[j][lL[j].Count - 1].X == lL[j][0].X && lL[j][lL[j].Count - 1].Y == lL[j][0].Y)
                                                    {
                                                        lL[j].RemoveAt(lL[j].Count - 1);
                                                        rlp = true;
                                                    }
                                                    gPath2.AddClosedCurve(lL[j].ToArray());
                                                    if (rlp)
                                                        lL[j].Add(lL[j][0]);
                                                }
                                                else if (lL[j].Count > 1)
                                                    gPath2.AddLines(lL[j].ToArray());
                                                else
                                                {
                                                    PointF p = lL[j][0];
                                                    gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                                }
                                            }
                                            else if (lL[j].Count > 1)
                                                gPath2.AddCurve(lL[j].ToArray());
                                            else
                                            {
                                                PointF p = lL[j][0];
                                                gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (checkBox3Checked)
                                        {
                                            lL[j] = fBmp.RemoveColinearity(lL[j], true, this._precisionForRounding);

                                            if (checkBox7Checked)
                                                lL[j] = fBmp.ApproximateLines(lL[j], epsilon);

                                            gPath2.AddLines(lL[j].ToArray());
                                        }
                                        else
                                        {
                                            if (checkBox7Checked)
                                                lL[j] = fBmp.ApproximateLines(lL[j], epsilon);

                                            gPath2.AddLines(lL[j].ToArray());
                                        }

                                        if (checkBox4Checked)
                                            gPath2.CloseFigure();
                                    }

                                    if (gPath2.PointCount > 1)
                                    {
                                        gPath2.FillMode = FillMode.Alternate;
                                        gPath4.AddPath(gPath2, false);
                                    }
                                }

                                if (rlp2)
                                    lL[j].Add(lL[j][0]);
                            }
                        }

                        if (gPath != null)
                            gPath.Dispose();
                        gPath = null;

                        if (gPath4 != null && gPath4.PointCount > 1)
                            fP.AddPath(gPath4, false);
                    }
                }

            if (CheckShift())
            {
                if (fP != null && fP.PointCount > 1)
                {
                    if (fPath != null)
                    {
                        fPath.Dispose();
                        fPath = null;
                    }

                    fPath = fP;
                }
            }

            return fPath;
        }

        // shall the path be shifted?
        private bool CheckShift()
        {
            return this._shiftOutwards != 0;
        }

        // get a graphicsPath from our current chaincode
        // and process it as set by user
        private GraphicsPath GetPath4(int comboBox1SelectedIndex, double numericUpDown7Value, bool CheckBox18Checked, bool checkBox2Checked, bool checkBox3Checked, bool checkBox7Checked, bool checkBox4Checked, bool checkBox5Checked)
        {
            GraphicsPath zPath = new GraphicsPath(); // large path
            ChainFinder fBmp = new ChainFinder();

            // max allowed distance for approximating lines
            double epsilon = Math.Sqrt(2.0) / 2.0;

            switch (comboBox1SelectedIndex)
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
                        epsilon = System.Convert.ToDouble(numericUpDown7Value);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }

            int sign = -1;
            // loop over all chains
            if (this._fList != null)
                for (int i = 0; i <= this._fList.Count - 1; i++)
                {
                    sign = -1;
                    GraphicsPath? fPath = new GraphicsPath();
                    using (GraphicsPath gPath = new GraphicsPath()) // path for current chain
                    {
                        ChainCode c = (ChainCode)this._fList[i];
                        if (IsInnerOutline(c))
                            sign = 1;

                        List<Point> lList = c.Coord;

                        bool rlp2 = false;
                        if (lList.Count > 0 && checkBox4Checked && lList[lList.Count - 1] == lList[0])
                        {
                            lList.RemoveAt(lList.Count - 1);
                            rlp2 = true;
                        }

                        bool onlyLargest = false;
                        if (CheckBox18Checked)
                        {
                            this._fList = this._fList.OrderByDescending(a => a.Coord.Count).ToList();
                            onlyLargest = true;
                        }

                        // not shifted -> only apply cleaning, approximating lines etc.
                        if (CheckShift() == false)
                        {
                            if (checkBox2Checked)
                            {
                                if (checkBox3Checked)
                                {
                                    List<Point> fList = c.Coord;
                                    fList = fBmp.RemoveColinearity(fList, true, this._precisionForRounding);

                                    if (checkBox7Checked)
                                        fList = fBmp.ApproximateLines(fList, epsilon);

                                    // If fList.Count > 0 AndAlso checkBox4Checked AndAlso fList(fList.Count - 1) <> fList(0) Then 'close
                                    // fList.Add(fList(0))
                                    // End If

                                    if (checkBox5Checked)
                                    {
                                        if (fList.Count > 2)
                                        {
                                            bool rlp = false;
                                            if (fList[fList.Count - 1].X == fList[0].X && fList[fList.Count - 1].Y == fList[0].Y)
                                            {
                                                fList.RemoveAt(fList.Count - 1);
                                                rlp = true;
                                            }
                                            gPath.AddClosedCurve(fList.ToArray());
                                            if (rlp)
                                                fList.Add(fList[0]);
                                        }
                                        else if (fList.Count > 1)
                                            gPath.AddLines(fList.ToArray());
                                        else
                                        {
                                            Point p = fList[0];
                                            gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                    else if (fList.Count > 1)
                                        gPath.AddCurve(fList.ToArray());
                                    else
                                    {
                                        Point p = fList[0];
                                        gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                    }
                                }
                                else
                                {
                                    if (checkBox7Checked)
                                        lList = fBmp.ApproximateLines(lList, epsilon);

                                    // If lList.Count > 0 AndAlso checkBox4Checked AndAlso lList(lList.Count - 1) <> lList(0) Then
                                    // lList.Add(lList(0))
                                    // End If

                                    if (checkBox5Checked)
                                    {
                                        if (lList.Count > 2)
                                        {
                                            bool rlp = false;
                                            if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                                            {
                                                lList.RemoveAt(lList.Count - 1);
                                                rlp = true;
                                            }
                                            gPath.AddClosedCurve(lList.ToArray());
                                            if (rlp)
                                                lList.Add(lList[0]);
                                        }
                                        else if (lList.Count > 1)
                                            gPath.AddLines(lList.ToArray());
                                        else
                                        {
                                            Point p = lList[0];
                                            gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                        }
                                    }
                                    else if (lList.Count > 1)
                                        gPath.AddCurve(lList.ToArray());
                                    else
                                    {
                                        Point p = lList[0];
                                        gPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                    }
                                }
                            }
                            else
                            {
                                if (checkBox3Checked)
                                {
                                    List<Point> fList = c.Coord;

                                    fList = fBmp.RemoveColinearity(fList, true, this._precisionForRounding);

                                    if (checkBox7Checked)
                                        fList = fBmp.ApproximateLines(fList, epsilon);

                                    gPath.AddLines(fList.ToArray());
                                }
                                else
                                {
                                    if (checkBox7Checked)
                                        lList = fBmp.ApproximateLines(lList, epsilon);

                                    gPath.AddLines(lList.ToArray());
                                }

                                if (checkBox4Checked)
                                    gPath.CloseFigure();
                            }
                        }
                        else
                            gPath.AddLines(lList.ToArray());

                        if (onlyLargest)
                            i = this._fList.Count;

                        if (gPath != null)
                        {
                            try
                            {
                                if (gPath.PathPoints.Length > 1)
                                {
                                    if (i > 0 && this.checkBox8.Checked)
                                        fPath.AddPath(gPath, true);
                                    else
                                        fPath.AddPath(gPath, false);
                                }
                                else
                                {
                                    PointF p = gPath.PathPoints[0];
                                    fPath.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                }
                            }
                            catch
                            {
                            }
                        }

                        if (rlp2)
                            lList.Add(lList[0]);
                    }

                    if (CheckShift())
                    {
                        MorphologicalOperations m2 = new MorphologicalOperations();
                        GraphicsPathData gPath = m2.ShiftCoords4(fPath, this._shiftOutwards * sign, this.CheckBox18.Checked, _stretchPath, this._shiftFractionMode); // get shifted path
                        GraphicsPath gPath4 = new GraphicsPath(); // large path for current block

                        if (gPath != null && gPath.AllPoints != null && gPath.AllPoints.Count > 0)
                        {
                            for (int cnt = 0; cnt <= gPath.AllPoints.Count - 1; cnt++) // loop over each subpath
                            {
                                List<PointF> lList = gPath.AllPoints[cnt];

                                bool rlp2 = false;
                                if (lList.Count > 0 && checkBox4Checked && lList[lList.Count - 1] == lList[0])
                                {
                                    lList.RemoveAt(lList.Count - 1);
                                    rlp2 = true;
                                }

                                using (GraphicsPath gPath2 = new GraphicsPath()) // path for current subpath
                                {
                                    // now process the shifted path just as above
                                    if (checkBox2Checked)
                                    {
                                        if (checkBox3Checked)
                                        {
                                            lList = fBmp.RemoveColinearity(lList, true, this._precisionForRounding);

                                            if (checkBox7Checked)
                                                lList = fBmp.ApproximateLines(lList, epsilon);

                                            // If lList.Count > 0 AndAlso checkBox4Checked AndAlso lList(lList.Count - 1) <> lList(0) Then
                                            // lList.Add(lList(0))
                                            // End If

                                            if (checkBox5Checked)
                                            {
                                                if (lList.Count > 2)
                                                {
                                                    bool rlp = false;
                                                    if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                                                    {
                                                        lList.RemoveAt(lList.Count - 1);
                                                        rlp = true;
                                                    }
                                                    gPath2.AddClosedCurve(lList.ToArray());
                                                    if (rlp)
                                                        lList.Add(lList[0]);
                                                }
                                                else if (lList.Count > 1)
                                                    gPath2.AddLines(lList.ToArray());
                                                else
                                                {
                                                    PointF p = lList[0];
                                                    gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                                }
                                            }
                                            else if (lList.Count > 1)
                                                gPath2.AddCurve(lList.ToArray());
                                            else
                                            {
                                                PointF p = lList[0];
                                                gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                            }
                                        }
                                        else
                                        {
                                            if (checkBox7Checked)
                                                lList = fBmp.ApproximateLines(lList, epsilon);

                                            // If lList.Count > 0 AndAlso checkBox4Checked AndAlso lList(lList.Count - 1) <> lList(0) Then
                                            // lList.Add(lList(0))
                                            // End If

                                            if (checkBox5Checked)
                                            {
                                                if (lList.Count > 2)
                                                {
                                                    bool rlp = false;
                                                    if (lList[lList.Count - 1].X == lList[0].X && lList[lList.Count - 1].Y == lList[0].Y)
                                                    {
                                                        lList.RemoveAt(lList.Count - 1);
                                                        rlp = true;
                                                    }
                                                    gPath2.AddClosedCurve(lList.ToArray());
                                                    if (rlp)
                                                        lList.Add(lList[0]);
                                                }
                                                else if (lList.Count > 1)
                                                    gPath2.AddLines(lList.ToArray());
                                                else
                                                {
                                                    PointF p = lList[0];
                                                    gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                                }
                                            }
                                            else if (lList.Count > 1)
                                                gPath2.AddCurve(lList.ToArray());
                                            else
                                            {
                                                PointF p = lList[0];
                                                gPath2.AddRectangle(new RectangleF(p.X, p.Y, 1.0F, 1.0F));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (checkBox3Checked)
                                        {
                                            lList = fBmp.RemoveColinearity(lList, true, this._precisionForRounding);

                                            if (checkBox7Checked)
                                                lList = fBmp.ApproximateLines(lList, epsilon);

                                            gPath2.AddLines(lList.ToArray());
                                        }
                                        else
                                        {
                                            if (checkBox7Checked)
                                                lList = fBmp.ApproximateLines(lList, epsilon);

                                            gPath2.AddLines(lList.ToArray());
                                        }

                                        if (checkBox4Checked)
                                            gPath2.CloseFigure();
                                    }

                                    if (gPath2.PointCount > 1)
                                    {
                                        gPath2.FillMode = FillMode.Alternate;
                                        gPath4.AddPath(gPath2, false); // add to large (shifted) path
                                    }
                                }

                                if (rlp2)
                                    lList.Add(lList[0]);
                            }

                            if (gPath4 != null && gPath4.PointCount > 1)
                            {
                                if (fPath != null)
                                {
                                    fPath.Dispose();
                                    fPath = null;
                                }

                                fPath = gPath4; // set as reult
                            }
                        }
                    }

                    if (fPath != null && fPath.PointCount > 1)
                        zPath.AddPath(fPath, false);
                }
            return zPath; // result
        }

        // draw the current processed path to the form
        private void Button1_Click(object sender, EventArgs e)
        {
            if (this.BackgroundWorker1 != null && !this.BackgroundWorker1.IsBusy && this._fList != null && this._fList.Count > 0)
            {
                this.SetControls(false);
                this.BackgroundWorker1.RunWorkerAsync(new object[] { this._shiftPathMode, false, this.comboBox1.SelectedIndex, (double)this.numericUpDown7.Value, this.CheckBox18.Checked, this.checkBox2.Checked, this.checkBox3.Checked, this.checkBox7.Checked, this.checkBox4.Checked, this.checkBox5.Checked });
            }
            else
                MessageBox.Show("You need to \"outline\" the shapes first.");
        }

        // get processed path and preview path in extra form
        private void button8_Click(object sender, EventArgs e)
        {
            if (this.BackgroundWorker1 != null && !this.BackgroundWorker1.IsBusy && this._fList != null && this._fList.Count > 0)
            {
                this.SetControls(false);
                this.BackgroundWorker1.RunWorkerAsync(new object[] { this._shiftPathMode, true, this.comboBox1.SelectedIndex, (double)this.numericUpDown7.Value, this.CheckBox18.Checked, this.checkBox2.Checked, this.checkBox3.Checked, this.checkBox7.Checked, this.checkBox4.Checked, this.checkBox5.Checked });
            }
            else
                MessageBox.Show("You need to \"outline\" the shapes first.");
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._bmp != null && _r != null)
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
                    if (this.CheckBox1.Checked && e.Button == MouseButtons.Left)
                    {
                        this._ptx2 = ix;
                        this._ptY2 = iy;
                        this.PictureBox1.Invalidate();
                    }
                }
            }
        }

        private void PictureBox2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.PictureBox2.SizeMode == PictureBoxSizeMode.Zoom)
            {
                this.PictureBox2.Location = new Point(0, 0);
                this.PictureBox2.Dock = DockStyle.None;
                this.PictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            else
            {
                this.PictureBox2.Dock = DockStyle.Fill;
                this.PictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void PictureBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.PictureBox3.SizeMode == PictureBoxSizeMode.Zoom)
            {
                this.PictureBox3.Location = new Point(0, 0);
                this.PictureBox3.Dock = DockStyle.None;
                this.PictureBox3.SizeMode = PictureBoxSizeMode.AutoSize;
            }
            else
            {
                this.PictureBox3.Dock = DockStyle.Fill;
                this.PictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void PictureBox1_Layout(object sender, LayoutEventArgs e)
        {
            if (this.PictureBox1.Image != null)
                this._r = GetImageRectangle();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (this.PictureBox3.Image != null)
            {
                try
                {
                    if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                        this.PictureBox3.Image.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // show and edit rect specs
        private void Button14_Click(object sender, EventArgs e)
        {
            Rectangle r = new Rectangle(this._ptX, this._ptY, this._ptx2 - this._ptX, this._ptY2 - this._ptY);

            if (r.X < 0)
                r = new Rectangle(0, r.Y, r.Width + r.X, r.Height);
            if (r.Y < 0)
                r = new Rectangle(r.X, 0, r.Width, r.Height + r.Y);
            if (r.X + r.Width >= this.PictureBox1.Image.Width)
                r = new Rectangle(r.X, r.Y, this.PictureBox1.Image.Width - r.X - 1, r.Height);
            if (r.Y + r.Height >= this.PictureBox1.Image.Height)
                r = new Rectangle(r.X, r.Y, r.Width, this.PictureBox1.Image.Height - r.Y - 1);

            frmRect frm = new frmRect();
            frm.numX.Value = r.X;
            frm.numY.Value = r.Y;
            frm.numW.Value = r.Width;
            frm.numH.Value = r.Height;

            if (frm.ShowDialog() == DialogResult.OK)
            {
                r = new Rectangle(System.Convert.ToInt32(frm.numX.Value), System.Convert.ToInt32(frm.numY.Value), System.Convert.ToInt32(frm.numW.Value), System.Convert.ToInt32(frm.numH.Value));

                if (r.X < 0)
                    r = new Rectangle(0, r.Y, r.Width + r.X, r.Height);
                if (r.Y < 0)
                    r = new Rectangle(r.X, 0, r.Width, r.Height + r.Y);
                if (r.X + r.Width >= this.PictureBox1.Image.Width)
                    r = new Rectangle(r.X, r.Y, this.PictureBox1.Image.Width - r.X - 1, r.Height);
                if (r.Y + r.Height >= this.PictureBox1.Image.Height)
                    r = new Rectangle(r.X, r.Y, r.Width, this.PictureBox1.Image.Height - r.Y - 1);

                float factor = this._zoom;

                this._ptX = r.X;
                this._ptY = r.Y;
                this._ptx2 = r.X + r.Width;
                this._ptY2 = r.Y + r.Height;

                this.PictureBox1.Invalidate();
            }
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.CheckBox1.Checked && e.Button == MouseButtons.Left)
            {
                if (this._bmp != null && _r != null)
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

                    // for selection rect
                    this._ptX = ix;
                    this._ptY = iy;
                }
            }
            this.SplitContainer2.Panel1.Focus();
        }

        // if we have a valid selection rect, open a form to display and edit the selected part of the current path
        private void Button13_Click(object sender, EventArgs e)
        {
            if (this.PictureBox1.Image != null && this._bmp != null)
            {
                Rectangle r = new Rectangle(this._ptX, this._ptY, this._ptx2 - this._ptX, this._ptY2 - this._ptY);

                if (r.X < 0)
                    r = new Rectangle(0, r.Y, r.Width + r.X, r.Height);
                if (r.Y < 0)
                    r = new Rectangle(r.X, 0, r.Width, r.Height + r.Y);
                if (r.X + r.Width >= this.PictureBox1.Image.Width)
                    r = new Rectangle(r.X, r.Y, this.PictureBox1.Image.Width - r.X - 1, r.Height);
                if (r.Y + r.Height >= this.PictureBox1.Image.Height)
                    r = new Rectangle(r.X, r.Y, r.Width, this.PictureBox1.Image.Height - r.Y - 1);

                if (r.X >= 0 && r.Y >= 0 && r.Width > 0 && r.Height > 0 && r.X + r.Width < this.PictureBox1.Image.Width && r.Y + r.Height < this.PictureBox1.Image.Height)
                {
                    if (this._aLF2 == null)
                        this.Button1.PerformClick();

                    Bitmap? bmp = null;
                    try
                    {
                        if (AvailMem.AvailMem.checkAvailRam(r.Width * r.Height * 4L))
                        {
                            bmp = _bmp.Clone(r, _bmp.PixelFormat);

                            if (this._aLF2 != null && this._aLF2.PointCount > 1)
                            {
                                using (GraphicsPath gP = (GraphicsPath)this._aLF2.Clone())
                                {
                                    frmPathDetails frm = new frmPathDetails(bmp, gP, r);

                                    this.Button16.Enabled = false;

                                    bool b = this.Button2.Enabled;
                                    this.SetControls(false);

                                    frm.OrigFPathProvide += frm_OrigFPathProvide;

                                    if (frm.ShowDialog() == DialogResult.OK)
                                    {
                                        GraphicsPath? fP = frm.FPathComplete;

                                        // CompPaths(Me._aLF2, fP)

                                        ChainFinder c = new ChainFinder();

                                        // re scan outline with fP drawn
                                        Bitmap? bmpTmp = null;

                                        try
                                        {
                                            if (fP != null && AvailMem.AvailMem.checkAvailRam(this._bmp.Width * this._bmp.Height * 4L))
                                            {
                                                bmpTmp = new Bitmap(this._bmp.Width, this._bmp.Height);

                                                using (Graphics g = Graphics.FromImage(bmpTmp))
                                                {
                                                    g.SmoothingMode = SmoothingMode.AntiAlias;

                                                    // Using tb As New TextureBrush(Me._bmp)
                                                    using (SolidBrush sb = new SolidBrush(Color.Black))
                                                    {
                                                        fP.FillMode = FillMode.Winding;
                                                        g.FillPath(sb, fP);
                                                    }
                                                }

                                                //_outlineDone = false;

                                                c.ProgressPlus += fipbmp_ProgressPlus;
                                                this.label3.Text = "working...";

                                                int threshold = System.Convert.ToInt32(this.numericUpDown1.Value);

                                                List<ChainCode> fList = new List<ChainCode>();

                                                if (this.comboBox4.SelectedIndex == 0)
                                                    fList = c.GetOutline(bmpTmp, 0, false, 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), false);
                                                else
                                                    fList = c.GetOutline(bmpTmp, 0, false, 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), true);

                                                if (fList.Count > 0)
                                                {
                                                    // remove 1px chains
                                                    for (int i = fList.Count - 1; i >= 0; i += -1)
                                                    {
                                                        if (fList[i].Coord.Count <= 4)
                                                            fList.RemoveAt(i);
                                                    }

                                                    fList = fList.OrderBy(a => a.Coord.Count).ToList();
                                                    fList.Reverse();

                                                    for (int i = 0; i <= fList.Count - 1; i++)
                                                    {
                                                        if (fList[i].Coord.Count > 0 && fList[i].Coord[fList[i].Coord.Count - 1] == fList[i].Coord[0])
                                                        {
                                                            fList[i].Coord.RemoveAt(fList[i].Coord.Count - 1);
                                                            fList[i].Chain.RemoveAt(fList[i].Chain.Count - 1);
                                                        }
                                                    }
                                                }

                                                c.ProgressPlus -= fipbmp_ProgressPlus;
                                                this.label3.Text = "done.";

                                                this._fList = fList;

                                                //_outlineDone = true;
                                                this._shiftOutwards = 0;

                                                GetPicture2();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }
                                        finally
                                        {
                                            if (bmpTmp != null)
                                            {
                                                bmpTmp.Dispose();
                                                bmpTmp = null;
                                            }
                                        }

                                        GraphicsPath? pOld = this._aLF2;
                                        this._aLF2 = fP;
                                        if (pOld != null)
                                        {
                                            pOld.Dispose();
                                            pOld = null;
                                        }
                                        this.Button9.Enabled = true;

                                        this.CheckBox1.Checked = false;
                                        this.Button1.PerformClick();
                                    }

                                    frm.OrigFPathProvide -= frm_OrigFPathProvide;

                                    this.SetControls(true);
                                    this.Button2.Enabled = b;
                                    this.PictureBox1.Refresh();
                                }
                            }
                        }
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

        private void frm_OrigFPathProvide(object? sender, FPathEventArgs e)
        {
            if (e.Path != null && e.Path.PointCount > 1)
            {
                this._frmfPathClone = (GraphicsPath)e.Path.Clone();

                using (Matrix mx = new Matrix(1, 0, 0, 1, e.X, e.Y))
                {
                    this._frmfPathClone.Transform(mx);
                }

                this.Button16.Enabled = true;
            }
        }

        // Private Sub CompPaths(aLF2 As GraphicsPath, fP As GraphicsPath)
        // Dim t1(aLF2.PathTypes.Length - 1) As Byte
        // aLF2.PathTypes.CopyTo(t1, 0)
        // Dim t2(fP.PathTypes.Length - 1) As Byte
        // fP.PathTypes.CopyTo(t2, 0)

        // MessageBox.Show((t1.Length = t2.Length).ToString())

        // Dim l As New List(Of Integer)

        // For i As Integer = 0 To t1.Length - 1
        // If t1(i) <> t2(i) Then
        // l.Add(i)
        // End If
        // Next

        // l = l
        // End Sub

        private void Button3_Click(object sender, EventArgs e)
        {
            this.button6.PerformClick();
            this.Button1.PerformClick();
        }

        private void CheckBox6_CheckedChanged(object sender, EventArgs e)
        {
            this._fPsf = this.CheckBox6.Checked;
        }

        private void CheckBox9_CheckedChanged(object sender, EventArgs e)
        {
            this._addHoles = this.CheckBox9.Checked;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;

                if (bmp != null)
                    try
                    {
                        using (Image i = Image.FromFile(this.openFileDialog1.FileName))
                        {
                            if (AvailMem.AvailMem.checkAvailRam(i.Width * i.Height * 4))
                                bmp = (Bitmap)i.Clone();
                        }

                        Bitmap? bOld = this._bmp;
                        _bmp = bmp; // CType(bmp.Clone(), Bitmap)
                        if (bOld != null)
                        {
                            bOld.Dispose();
                            bOld = null;
                        }
                        Bitmap? bOldBU = this._bmpBU;
                        _bmpBU = (Bitmap)bmp.Clone();
                        if (bOldBU != null)
                        {
                            bOldBU.Dispose();
                            bOldBU = null;
                        }
                        Image? iOld = this.PictureBox1.Image;
                        this.PictureBox1.Image = (Bitmap)bmp.Clone();
                        this._r = GetImageRectangle();
                        if (iOld != null)
                        {
                            iOld.Dispose();
                            iOld = null;
                        }

                        _fList = new List<ChainCode>();
                        //_outlineDone = false;

                        GraphicsPath? alfOld = _aLF2;
                        _aLF2 = new GraphicsPath();
                        if (alfOld != null)
                        {
                            alfOld.Dispose();
                            alfOld = null;
                        }
                        this.Button9.Enabled = false;

                        Image? iOld2 = this.PictureBox2.Image;
                        this.PictureBox2.Image = null;
                        if (iOld2 != null)
                        {
                            iOld2.Dispose();
                            iOld2 = null;
                        }
                        Image? iOld3 = this.PictureBox3.Image;
                        this.PictureBox3.Image = null;
                        if (iOld3 != null)
                        {
                            iOld3.Dispose();
                            iOld3 = null;
                        }

                        this.ToolStripStatusLabel1.Text = "";
                    }
                    catch
                    {
                        if (bmp != null)
                            bmp.Dispose();
                    }
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (this._bmp != null)
            {
                frmFloodClear? frm = null;

                if (this.CheckBox11.Checked && this.PictureBox3.Image != null)
                    frm = new frmFloodClear((Bitmap)this.PictureBox3.Image);
                else
                    frm = new frmFloodClear((Bitmap)this.PictureBox1.Image);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = frm.FBitmap;

                    Image? iOld = this.PictureBox1.Image;
                    this.PictureBox1.Image = (Bitmap)bmp.Clone();
                    this._r = GetImageRectangle();
                    if (iOld != null)
                    {
                        iOld.Dispose();
                        iOld = null;
                    }

                    Bitmap? bOld = this._bmp;

                    this._bmp = (Bitmap)bmp.Clone();

                    if (bOld != null)
                    {
                        bOld.Dispose();
                        bOld = null;
                    }

                    _fList = new List<ChainCode>();
                    //_outlineDone = false;

                    GraphicsPath? alfOld = _aLF2;
                    _aLF2 = new GraphicsPath();
                    if (alfOld != null)
                    {
                        alfOld.Dispose();
                        alfOld = null;
                    }
                    this.Button9.Enabled = false;

                    Image? iOld2 = this.PictureBox2.Image;
                    this.PictureBox2.Image = null;
                    if (iOld2 != null)
                    {
                        iOld2.Dispose();
                        iOld2 = null;
                    }
                    Image? iOld3 = this.PictureBox3.Image;
                    this.PictureBox3.Image = null;
                    if (iOld3 != null)
                    {
                        iOld3.Dispose();
                        iOld3 = null;
                    }

                    this.ToolStripStatusLabel1.Text = "";
                }
            }
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (this.PictureBox1.Image != null && this._bmp != null)
            {
                Bitmap? bOld = this._bmp;

                if (this._bmpBU != null)
                    this._bmp = (Bitmap)this._bmpBU.Clone();

                if (bOld != null)
                {
                    bOld.Dispose();
                    bOld = null;
                }

                Image? iOld = this.PictureBox1.Image;
                this.PictureBox1.Image = (Bitmap)this._bmp.Clone();
                this._r = GetImageRectangle();
                if (iOld != null)
                {
                    iOld.Dispose();
                    iOld = null;
                }
            }
        }

        private void BackgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                if ((ShiftPathMode)o[0] == ShiftPathMode.DrawPath)
                {
                    using (GraphicsPath fPath = GetPath2(System.Convert.ToInt32(o[2]), System.Convert.ToDouble(o[3]), System.Convert.ToBoolean(o[4]), System.Convert.ToBoolean(o[5]), System.Convert.ToBoolean(o[6]), System.Convert.ToBoolean(o[7]), System.Convert.ToBoolean(o[8]), System.Convert.ToBoolean(o[9])))
                    {
                        if (fPath != null)
                        {
                            GraphicsPath? alfOld = _aLF2;
                            _aLF2 = (GraphicsPath)fPath.Clone();
                            if (alfOld != null)
                            {
                                alfOld.Dispose();
                                alfOld = null;
                            }
                        }
                    }
                }
                else if ((ShiftPathMode)o[0] == ShiftPathMode.FillPath)
                {
                    using (GraphicsPath fPath = GetPath(System.Convert.ToInt32(o[2]), System.Convert.ToDouble(o[3]), System.Convert.ToBoolean(o[4]), System.Convert.ToBoolean(o[5]), System.Convert.ToBoolean(o[6]), System.Convert.ToBoolean(o[7]), System.Convert.ToBoolean(o[8]), System.Convert.ToBoolean(o[9])))
                    {
                        if (fPath != null)
                        {
                            GraphicsPath? alfOld = _aLF2;
                            _aLF2 = (GraphicsPath)fPath.Clone();
                            if (alfOld != null)
                            {
                                alfOld.Dispose();
                                alfOld = null;
                            }
                        }
                    }
                }
                else
                    using (GraphicsPath fPath = GetPath4(System.Convert.ToInt32(o[2]), System.Convert.ToDouble(o[3]), System.Convert.ToBoolean(o[4]), System.Convert.ToBoolean(o[5]), System.Convert.ToBoolean(o[6]), System.Convert.ToBoolean(o[7]), System.Convert.ToBoolean(o[8]), System.Convert.ToBoolean(o[9])))
                    {
                        if (fPath != null)
                        {
                            GraphicsPath? alfOld = _aLF2;
                            _aLF2 = (GraphicsPath)fPath.Clone();
                            if (alfOld != null)
                            {
                                alfOld.Dispose();
                                alfOld = null;
                            }
                        }
                    }

                e.Result = (bool)o[1];
            }
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (this._aLF2 != null && this._aLF2.PointCount > 0)
            {
                if (System.Convert.ToBoolean(e.Result))
                {
                    try
                    {
                        Form2 frm = new Form2((GraphicsPath)this._aLF2.Clone());
                        frm.FormClosing -= frm_FormClosing;
                        frm.FormClosing += frm_FormClosing;
                        frm.Show();
                    }
                    catch
                    {
                    }
                }

                GetPicture3();
                this.SetControls(true);
                this.Button9.Enabled = true;
                this.PictureBox1.Refresh();
            }
        }

        private void CheckBox12_CheckedChanged(object sender, EventArgs e)
        {
            this._addHalfForDrawing = this.CheckBox12.Checked;

            if (this._fList != null && this._fList.Count > 0)
                GetPicture2();
            if (this._aLF2 != null && this._aLF2.PointCount > 0)
                GetPicture3();
        }

        private void Button9_Click(object sender, EventArgs e)
        {
            if (this._aLF2 != null && this._aLF2.PointCount > 1 && this.SaveFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.SaveFileDialog2.FileName;
                PointF[]? pts2 = this._aLF2.PathData.Points;
                byte[]? tps = this._aLF2.PathData.Types;

                if (pts2 != null && tps != null)
                {
                    object[] o = new object[] { pts2, tps };

                    try
                    {
                        JsonSerializerOptions options = new()
                        {
                            NumberHandling =
                                JsonNumberHandling.AllowReadingFromString |
                                JsonNumberHandling.WriteAsString,
                            WriteIndented = true,
                            ReadCommentHandling = JsonCommentHandling.Skip,
                            AllowTrailingCommas = true,
                        };

                        using FileStream createStream = File.Create(FileName);
                        JsonSerializer.Serialize(createStream, o, options);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
                    }
                }
            }
        }

        private void Button10_Click(object sender, EventArgs e)
        {
            if (this.OpenFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string fileName = this.OpenFileDialog2.FileName;
                if (File.Exists(fileName))
                {
                    this.SetControls(false);
                    object[]? o = null;
                    Stream? stream = null;
                    bool bError = false;

                    try
                    {
                        JsonSerializerOptions options = new()
                        {
                            NumberHandling =
                                JsonNumberHandling.AllowReadingFromString |
                                JsonNumberHandling.WriteAsString,
                            WriteIndented = true,
                            ReadCommentHandling = JsonCommentHandling.Skip,
                            AllowTrailingCommas = true,
                        };

                        stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        o = JsonSerializer.Deserialize<object[]>(stream, options);


                    }
                    catch (Exception ex)
                    {
                        bError = true;
                        Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
                    }

                    try
                    {
                        //stream.Close()
                        stream?.Dispose();
                        stream = null;
                    }
                    catch
                    { }

                    if (o != null && o.Length > 1)
                    {
                        PointF[] pts2 = (PointF[])o[0];
                        byte[] tps = (byte[])o[1];

                        GraphicsPath? gPath = new GraphicsPath(pts2, tps);

                        GraphicsPath? aOld = this._aLF2;

                        this._aLF2 = gPath;

                        if (aOld != null)
                        {
                            aOld.Dispose();
                            aOld = null;
                        }

                        GetFListFromNewPath();
                    }

                    if (bError)
                        MessageBox.Show("Errot");
                }
            }
        }

        private void GetFListFromNewPath()
        {
            // re scan outline with fP drawn
            Bitmap? bmpTmp = null;

            if (this._aLF2 != null && this._aLF2.PointCount > 1)
            {
                using (GraphicsPath fp = (GraphicsPath)this._aLF2.Clone())
                {
                    RectangleF rc = fp.GetBounds();
                    try
                    {
                        if (AvailMem.AvailMem.checkAvailRam(System.Convert.ToInt64(Math.Ceiling(rc.Width) * Math.Ceiling(rc.Height)) * 4L))
                        {
                            bmpTmp = new Bitmap(System.Convert.ToInt32(Math.Ceiling(rc.Width)), System.Convert.ToInt32(Math.Ceiling(rc.Height)));

                            using (Matrix mx = new Matrix(1, 0, 0, 1, -System.Convert.ToSingle(Math.Floor(rc.X)), -System.Convert.ToSingle(Math.Floor(rc.Y))))
                            {
                                fp.Transform(mx);
                            }

                            using (Graphics g = Graphics.FromImage(bmpTmp))
                            {
                                g.SmoothingMode = SmoothingMode.AntiAlias;

                                // Using tb As New TextureBrush(Me._bmp)
                                using (SolidBrush sb = new SolidBrush(Color.Black))
                                {
                                    g.FillPath(sb, fp);
                                }
                            }

                            bool b = this.Button2.Enabled;
                            this.SetControls(false);

                            //_outlineDone = false;

                            ChainFinder c = new ChainFinder();
                            c.ProgressPlus += fipbmp_ProgressPlus;
                            this.label3.Text = "working...";

                            int threshold = System.Convert.ToInt32(this.numericUpDown1.Value);

                            List<ChainCode> fList = new List<ChainCode>();

                            if (this.comboBox4.SelectedIndex == 0)
                                fList = c.GetOutline(bmpTmp, 0, false, 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), false);
                            else
                                fList = c.GetOutline(bmpTmp, 0, false, 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), true);

                            if (fList.Count > 0)
                            {
                                // remove 1px chains
                                for (int i = fList.Count - 1; i >= 0; i += -1)
                                {
                                    if (fList[i].Coord.Count <= 4)
                                        fList.RemoveAt(i);
                                }

                                fList = fList.OrderBy(a => a.Coord.Count).ToList();
                                fList.Reverse();

                                for (int i = 0; i <= fList.Count - 1; i++)
                                {
                                    List<Point> coords = fList[i].Coord;

                                    for (int j = 0; j <= coords.Count - 1; j++)
                                        coords[j] = new Point(coords[j].X + System.Convert.ToInt32(Math.Floor(rc.X)), coords[j].Y + System.Convert.ToInt32(Math.Floor(rc.Y)));
                                }
                            }

                            c.ProgressPlus -= fipbmp_ProgressPlus;
                            this.label3.Text = "done.";

                            this._fList = fList;

                            //_outlineDone = true;
                            this._shiftOutwards = 0;

                            GetPicture2();

                            this.SetControls(true);
                            this.Button2.Enabled = b;
                            this.PictureBox1.Refresh();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        if (bmpTmp != null)
                        {
                            bmpTmp.Dispose();
                            bmpTmp = null;
                        }
                    }
                }
            }
        }

        private void PictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.CheckBox1.Checked == false)
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

                _r = GetImageRectangle();
            }
        }

        private void CheckBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (this._fList != null && this._fList.Count > 0)
                GetPicture2();
            if (this._aLF2 != null && this._aLF2.PointCount > 0)
                GetPicture3();
        }

        private void Button12_Click(object sender, EventArgs e)
        {
            if (this._aLF2 != null && this._aLF2.PointCount > 0)
            {
                bool found = false;

                for (int i = 0; i <= this._aLF2.PathTypes.Length - 1; i++)
                {
                    if (_aLF2.PathTypes[i] != 1)
                        Console.WriteLine("At " + i + " -> " + _aLF2.PathTypes[i]);
                    // If _aLF2.PathTypes(i) = 129 OrElse (i > 0 AndAlso _aLF2.PathTypes(i) = 0) Then
                    if (i > 0 && _aLF2.PathTypes[i] == 0)
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    using (frmSelFig frm = new frmSelFig(this._aLF2))
                    {
                        frm.FigureChanged += frm_FigureChanged;
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (this._aLF2.Equals(this._curPathFigure) == false)
                            {
                                this._aLF2.Dispose();
                                this._aLF2 = null;
                            }
                            this._aLF2 = this._curPathFigure;
                        }
                        frm.FigureChanged -= frm_FigureChanged;
                    }
                }
            }
        }

        private void frm_FigureChanged(object? sender, FigureChangedEventArgs e)
        {
            if (this._curPathFigure != null && this._curPathFigure.Equals(this._aLF2) == false)
            {
                this._curPathFigure.Dispose();
                this._curPathFigure = null;
            }
            this._curPathFigure = GetFigure(e.FList);
            this.PictureBox1.Invalidate();
        }

        private GraphicsPath GetFigure(List<PointF> fList)
        {
            GraphicsPath gp = new GraphicsPath();

            if (fList != null && fList.Count > 1)
            {
                gp.AddLines(fList.ToArray());
                gp.CloseFigure();
            }

            return gp;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.PictureBox1 != null && this.SplitContainer2.Panel1.Focused)
            {
                if (keyData == Keys.Down)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-this.SplitContainer2.Panel1.AutoScrollPosition.X, -this.SplitContainer2.Panel1.AutoScrollPosition.Y + 4);
                    return true;
                }
                if (keyData == Keys.Up)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-this.SplitContainer2.Panel1.AutoScrollPosition.X, -this.SplitContainer2.Panel1.AutoScrollPosition.Y - 4);
                    return true;
                }
                if (keyData == Keys.Left)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-this.SplitContainer2.Panel1.AutoScrollPosition.X - 4, -this.SplitContainer2.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.Right)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-this.SplitContainer2.Panel1.AutoScrollPosition.X + 4, -this.SplitContainer2.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.PageDown)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-this.SplitContainer2.Panel1.AutoScrollPosition.X, -this.SplitContainer2.Panel1.AutoScrollPosition.Y + this.SplitContainer2.Panel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.PageUp)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-this.SplitContainer2.Panel1.AutoScrollPosition.X, -this.SplitContainer2.Panel1.AutoScrollPosition.Y - this.SplitContainer2.Panel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.Home)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-Int16.MaxValue, -this.SplitContainer2.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.End)
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(Int16.MaxValue, -this.SplitContainer2.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == (Keys.Home | Keys.Control))
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(-Int16.MaxValue, -Int16.MaxValue);
                    return true;
                }
                if (keyData == (Keys.End | Keys.Control))
                {
                    this.SplitContainer2.Panel1.AutoScrollPosition = new Point(Int16.MaxValue, Int16.MaxValue);
                    return true;
                }
            }
            if (this.PictureBox2 != null && this.SplitContainer3.Panel1.Focused)
            {
                if (keyData == Keys.Down)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-this.SplitContainer3.Panel1.AutoScrollPosition.X, -this.SplitContainer3.Panel1.AutoScrollPosition.Y + 4);
                    return true;
                }
                if (keyData == Keys.Up)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-this.SplitContainer3.Panel1.AutoScrollPosition.X, -this.SplitContainer3.Panel1.AutoScrollPosition.Y - 4);
                    return true;
                }
                if (keyData == Keys.Left)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-this.SplitContainer3.Panel1.AutoScrollPosition.X - 4, -this.SplitContainer3.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.Right)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-this.SplitContainer3.Panel1.AutoScrollPosition.X + 4, -this.SplitContainer3.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.PageDown)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-this.SplitContainer3.Panel1.AutoScrollPosition.X, -this.SplitContainer3.Panel1.AutoScrollPosition.Y + this.SplitContainer3.Panel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.PageUp)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-this.SplitContainer3.Panel1.AutoScrollPosition.X, -this.SplitContainer3.Panel1.AutoScrollPosition.Y - this.SplitContainer3.Panel1.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.Home)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-Int16.MaxValue, -this.SplitContainer3.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.End)
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(Int16.MaxValue, -this.SplitContainer3.Panel1.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == (Keys.Home | Keys.Control))
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(-Int16.MaxValue, -Int16.MaxValue);
                    return true;
                }
                if (keyData == (Keys.End | Keys.Control))
                {
                    this.SplitContainer3.Panel1.AutoScrollPosition = new Point(Int16.MaxValue, Int16.MaxValue);
                    return true;
                }
            }
            if (this.PictureBox3 != null && this.SplitContainer3.Panel2.Focused)
            {
                if (keyData == Keys.Down)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-this.SplitContainer3.Panel2.AutoScrollPosition.X, -this.SplitContainer3.Panel2.AutoScrollPosition.Y + 4);
                    return true;
                }
                if (keyData == Keys.Up)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-this.SplitContainer3.Panel2.AutoScrollPosition.X, -this.SplitContainer3.Panel2.AutoScrollPosition.Y - 4);
                    return true;
                }
                if (keyData == Keys.Left)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-this.SplitContainer3.Panel2.AutoScrollPosition.X - 4, -this.SplitContainer3.Panel2.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.Right)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-this.SplitContainer3.Panel2.AutoScrollPosition.X + 4, -this.SplitContainer3.Panel2.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.PageDown)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-this.SplitContainer3.Panel2.AutoScrollPosition.X, -this.SplitContainer3.Panel2.AutoScrollPosition.Y + this.SplitContainer3.Panel2.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.PageUp)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-this.SplitContainer3.Panel2.AutoScrollPosition.X, -this.SplitContainer3.Panel2.AutoScrollPosition.Y - this.SplitContainer3.Panel2.ClientSize.Height);
                    return true;
                }
                if (keyData == Keys.Home)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-Int16.MaxValue, -this.SplitContainer3.Panel2.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == Keys.End)
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(Int16.MaxValue, -this.SplitContainer3.Panel2.AutoScrollPosition.Y);
                    return true;
                }
                if (keyData == (Keys.Home | Keys.Control))
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(-Int16.MaxValue, -Int16.MaxValue);
                    return true;
                }
                if (keyData == (Keys.End | Keys.Control))
                {
                    this.SplitContainer3.Panel2.AutoScrollPosition = new Point(Int16.MaxValue, Int16.MaxValue);
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void PictureBox2_MouseDown(object sender, MouseEventArgs e)
        {
            this.SplitContainer3.Panel1.Focus();
        }

        private void PictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            this.SplitContainer3.Panel2.Focus();
        }

        private void Button15_Click(object sender, EventArgs e)
        {
            if (this._cP != null && this._cP.Count > 0)
            {
                this.SetControls(false);
                // load into _alf2
                if (this._cP.Count > 1)
                {
                    GraphicsPath? alf2Old = this._aLF2;
                    this._aLF2 = new GraphicsPath();

                    for (int i = 0; i <= this._cP.Count - 1; i++)
                    {
                        List<PointF> p = this._cP[0];
                        if (p.Count > 1)
                        {
                            List<byte> t = new List<byte>();
                            t.Add(0);
                            for (int j = 1; j <= p.Count - 2; j++)
                                t.Add(1);
                            t.Add(129);
                            GraphicsPath aLF = new GraphicsPath(p.ToArray(), t.ToArray());

                            this._aLF2.AddPath(aLF, false);
                        }
                    }

                    if (alf2Old != null)
                    {
                        alf2Old.Dispose();
                        alf2Old = null;
                    }
                }
                else
                {
                    GraphicsPath? alf2Old = this._aLF2;
                    List<PointF> p = this._cP[0];
                    if (p.Count > 1)
                    {
                        List<byte> t = new List<byte>();
                        t.Add(0);
                        for (int j = 1; j <= p.Count - 2; j++)
                            t.Add(1);
                        t.Add(129);
                        this._aLF2 = new GraphicsPath(p.ToArray(), t.ToArray());
                    }
                    if (alf2Old != null)
                    {
                        alf2Old.Dispose();
                        alf2Old = null;
                    }
                }

                SetUpFList2();

                GetPicture3();
                this.SetControls(true);
                this.Button9.Enabled = true;
                this.PictureBox1.Refresh();
            }
        }

        private void SetUpFList2()
        {
            if (_bmp != null && this._aLF2?.PathPoints != null)
            {
                Bitmap? bmpTmp = null;
                if (AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                    bmpTmp = (Bitmap)_bmp.Clone();
                else
                {
                    MessageBox.Show("Not enough Memory");
                    return;
                }

                //_outlineDone = false;

                ChainFinder fbmp = new ChainFinder();

                this.label3.Text = "working...";

                List<ChainCode> fList = new List<ChainCode>();

                ChainCode cc = new ChainCode();
                cc.Coord.AddRange(this._aLF2.PathPoints.ToList().ConvertAll(a => new Point(System.Convert.ToInt32(a.X), System.Convert.ToInt32(a.Y))).ToArray());
                cc.Chain.AddRange(ChainFinder.GetChain(cc.Coord));
                fList.Add(cc);

                this.label3.Text = "done.";

                this._fList = fList;

                //_outlineDone = true;

                GetPicture2();
            }
        }

        private void SetUpFList()
        {
            if (_bmp != null)
            {
                Bitmap? bmpTmp = null;
                if (AvailMem.AvailMem.checkAvailRam(_bmp.Width * _bmp.Height * 4L))
                    bmpTmp = (Bitmap)_bmp.Clone();
                else
                {
                    MessageBox.Show("Not enough Memory");
                    return;
                }

                //_outlineDone = false;

                ChainFinder fbmp = new ChainFinder();

                this.label3.Text = "working...";

                List<ChainCode> fList = new List<ChainCode>();

                fList = fbmp.GetOutline(bmpTmp, 0, false, 0, false, 0, false);

                if (fList.Count > 0)
                {
                    // remove 1px chains
                    for (int i = fList.Count - 1; i >= 0; i += -1)
                    {
                        if (fList[i].Coord.Count <= 4)
                            fList.RemoveAt(i);
                    }

                    fList = fList.OrderBy(a => a.Coord.Count).ToList();
                    fList.Reverse();

                    int maxChains = System.Convert.ToInt32(this.NumericUpDown2.Value);

                    // -1 is "no limit"
                    if (maxChains == -1)
                        maxChains = Int32.MaxValue;

                    if (fList.Count > maxChains)
                    {
                        if (MessageBox.Show("The outlineslist contains more than " + maxChains + " chains. Shall only the first " + maxChains + " chains be taken?", "Path may be slowly drawn...", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                            fList.RemoveRange(maxChains, fList.Count - maxChains);
                    }
                }

                this.label3.Text = "done.";

                this._fList = fList;

                //_outlineDone = true;

                GetPicture2();
            }
        }

        private void Button16_Click(object sender, EventArgs e)
        {
            GraphicsPath? fP = this._frmfPathClone;

            ChainFinder c = new ChainFinder();

            // re scan outline with fP drawn
            Bitmap? bmpTmp = null;

            if (this._bmp != null && fP != null)
                try
                {
                    if (AvailMem.AvailMem.checkAvailRam(this._bmp.Width * this._bmp.Height * 4L))
                    {
                        bmpTmp = new Bitmap(this._bmp.Width, this._bmp.Height);

                        using (Graphics g = Graphics.FromImage(bmpTmp))
                        {
                            g.SmoothingMode = SmoothingMode.AntiAlias;

                            // Using tb As New TextureBrush(Me._bmp)
                            using (SolidBrush sb = new SolidBrush(Color.Black))
                            {
                                fP.FillMode = FillMode.Winding;
                                g.FillPath(sb, fP);
                            }
                        }

                        bool b = this.Button2.Enabled;
                        this.SetControls(false);

                        //_outlineDone = false;

                        c.ProgressPlus += fipbmp_ProgressPlus;
                        this.label3.Text = "working...";

                        int threshold = System.Convert.ToInt32(this.numericUpDown1.Value);

                        List<ChainCode> fList = new List<ChainCode>();

                        if (this.comboBox4.SelectedIndex == 0)
                            fList = c.GetOutline(bmpTmp, 0, false, 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), false);
                        else
                            fList = c.GetOutline(bmpTmp, 0, false, 0, this.CheckBox10.Checked, System.Convert.ToInt32(numericUpDown8.Value), true);

                        if (fList.Count > 0)
                        {
                            // remove 1px chains
                            for (int i = fList.Count - 1; i >= 0; i += -1)
                            {
                                if (fList[i].Coord.Count <= 4)
                                    fList.RemoveAt(i);
                            }

                            fList = fList.OrderBy(a => a.Coord.Count).ToList();
                            fList.Reverse();

                            for (int i = 0; i <= fList.Count - 1; i++)
                            {
                                if (fList[i].Coord.Count > 0 && fList[i].Coord[fList[i].Coord.Count - 1] == fList[i].Coord[0])
                                {
                                    fList[i].Coord.RemoveAt(fList[i].Coord.Count - 1);
                                    fList[i].Chain.RemoveAt(fList[i].Chain.Count - 1);
                                }
                            }
                        }

                        c.ProgressPlus -= fipbmp_ProgressPlus;
                        this.label3.Text = "done.";

                        this._fList = fList;

                        //_outlineDone = true;
                        this._shiftOutwards = 0;

                        GetPicture2();

                        this.SetControls(true);
                        this.Button2.Enabled = b;
                        this.PictureBox1.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (bmpTmp != null)
                    {
                        bmpTmp.Dispose();
                        bmpTmp = null;
                    }
                }

            GraphicsPath? pOld = this._aLF2;
            this._aLF2 = fP;
            if (pOld != null)
            {
                pOld.Dispose();
                pOld = null;
            }
            this.Button9.Enabled = true;

            this.CheckBox1.Checked = false;
            this.Button1.PerformClick();
        }

        private void frmOutlineShift_Load(object sender, EventArgs e)
        {
            this.CheckBox15_CheckedChanged(this.CheckBox15, new EventArgs());
        }
    }
}
