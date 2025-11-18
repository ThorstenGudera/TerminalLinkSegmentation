using Cache;
using ChainCodeFinder;
using ConvolutionLib;
using SegmentsListLib;
using System;
using System.Collections;
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
using static AvoidAGrabCutEasy.RGBLab;

namespace AvoidAGrabCutEasy
{
    public partial class frmOutline : Form
    {
        private Bitmap? _bmpBU;
        private Bitmap? _bmpOrig;
        private UndoOPCache? _undoOPCache = null;

        private bool _dontAskOnClosing;

        public string? CachePathAddition
        {
            get
            {
                return m_CachePathAddition;
            }
            set
            {
                if (value != null)
                    m_CachePathAddition = value;
            }
        }

        private string? m_CachePathAddition;

        public Bitmap FBitmap
        {
            get
            {
                return this.helplineRulerCtrl1.Bmp;
            }
        }

        private bool _pic_changed = false;
        //private bool _dontDoZoom;

        private object _lockObject = new object();
        private int _ix;
        private int _iy;
        private Point _startPt;
        private int _eX2;
        private int _eY2;
        private bool _tracking;
        private List<Point>? _eraseList;
        private List<Point>? _drawList;
        private bool _tracking2;
        private Bitmap? _chainsBmp;
        private GraphicsPath? _hChain;
        private GraphicsPath? _regionPath;
        private Point _aP;
        private Point _eP;
        private GraphicsPath? _erasePath;
        private Rectangle _rc;
        private Bitmap? _bmpFirstStrokes;
        private List<Point>? _drawList2;
        private bool _tracking4;
        private GraphicsPath? _drawPath2;
        private bool _mouseIsDown;
        private Bitmap? _bmpOutlineRef;
        private bool _overlay;
        private static int[] CustomColors = new int[] { };

        public List<ChainCode>? SelectedChains { get; private set; }

        public event EventHandler<string>? BoundaryError;

        public frmOutline(Bitmap bmp, string basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

            //HelperFunctions.HelperFunctions.SetFormSizeBig(this);
            this.CenterToScreen();

            // Me.helplineRulerCtrl1.SetZoomOnlyByMethodCall = True

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.helplineRulerCtrl1.Bmp = (Bitmap)bmp.Clone();
                _bmpBU = (Bitmap)bmp.Clone();
                this._bmpOrig = (Bitmap)bmp.Clone();
            }
            else
            {
                MessageBox.Show("Not enough Memory");
                return;
            }

            double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
            else
                this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            this.helplineRulerCtrl1.AddDefaultHelplines();
            this.helplineRulerCtrl1.ResetAllHelpLineLabelsColor();

            this.helplineRulerCtrl1.dbPanel1.DragOver += bitmappanel1_DragOver;
            this.helplineRulerCtrl1.dbPanel1.DragDrop += bitmappanel1_DragDrop;

            this.helplineRulerCtrl1.dbPanel1.MouseDown += helplineRulerCtrl1_MouseDown;
            this.helplineRulerCtrl1.dbPanel1.MouseMove += helplineRulerCtrl1_MouseMove;
            this.helplineRulerCtrl1.dbPanel1.MouseUp += helplineRulerCtrl1_MouseUp;

            this.helplineRulerCtrl1.PostPaint += helplineRulerCtrl1_Paint;
        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {
            if (!this._mouseIsDown && this.checkedListBox1.Items.Count > 0 && this._chainsBmp != null && this.checkedListBox1.SelectedItems.Count > 0)
            {
                HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl1.dbPanel1;

                ColorMatrix cm = new ColorMatrix(); //test draw opaque instead
                cm.Matrix33 = 0.25F;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);
                    e.Graphics.DrawImage(this._chainsBmp,
                        new Rectangle(0, 0, pz.ClientRectangle.Width, pz.ClientRectangle.Height),
                        new RectangleF(-pz.AutoScrollPosition.X / this.helplineRulerCtrl1.Zoom,
                            -pz.AutoScrollPosition.Y / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Width / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Height / this.helplineRulerCtrl1.Zoom), GraphicsUnit.Pixel);
                }

                if (this._hChain != null)
                {
                    RectangleF r = _hChain.GetBounds();

                    if (this.checkedListBox1.CheckedItems.Count > 0)
                    {
                        using (GraphicsPath? gP = _hChain.Clone() as GraphicsPath)
                        {
                            if (gP != null)
                            {
                                using (Matrix mx2 = new Matrix(1, 0, 0, 1, -r.X, -r.Y))
                                    gP.Transform(mx2);

                                using (Matrix mx2 = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, 0, 0))
                                    gP.Transform(mx2);

                                using (Matrix mx2 = new Matrix(1, 0, 0, 1,
                                    r.X * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.X,
                                    r.Y * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.Y))
                                    gP.Transform(mx2);

                                using (SolidBrush sb = new SolidBrush(Color.FromArgb(64, Color.Blue)))
                                    e.Graphics.FillPath(sb, gP);
                            }
                        }

                        using (Pen p = new Pen(Color.DarkRed, 2))
                        {
                            if (this.cbBGColor.Checked)
                                p.Color = Color.OrangeRed;

                            e.Graphics.DrawRectangle(p, r.X * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.X,
                                r.Y * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.Y,
                                r.Width * this.helplineRulerCtrl1.Zoom, r.Height * this.helplineRulerCtrl1.Zoom);
                        }
                    }
                }
            }

            if (!this._mouseIsDown && this._bmpBU != null && this._overlay)
            {
                HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl1.dbPanel1;

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.4F;

                if (this.cbIncOpacity.Checked)
                    cm.Matrix33 = 0.75f;

                using ImageAttributes ia = new ImageAttributes();

                ia.SetColorMatrix(cm);
                e.Graphics.DrawImage(this._bmpBU,
                            new Rectangle(0, 0, pz.ClientRectangle.Width, pz.ClientRectangle.Height),
                            -pz.AutoScrollPosition.X / this.helplineRulerCtrl1.Zoom,
                                -pz.AutoScrollPosition.Y / this.helplineRulerCtrl1.Zoom,
                                pz.ClientRectangle.Width / this.helplineRulerCtrl1.Zoom,
                                pz.ClientRectangle.Height / this.helplineRulerCtrl1.Zoom, GraphicsUnit.Pixel, ia);
            }

            if (this._regionPath != null && this._regionPath.PointCount > 1 && this._tracking2 == false)
            {
                using GraphicsPath gP = (GraphicsPath)this._regionPath.Clone();
                using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                gP.Transform(mx);
                using Pen pen = new Pen(Color.Cyan, 2);
                pen.LineJoin = LineJoin.Round;
                e.Graphics.DrawPath(pen, gP);
            }

            if (this._drawList != null && this._drawList.Count > 1 && !this.rbRect.Checked)
            {
                using GraphicsPath gP = new GraphicsPath();
                gP.AddLines(this._drawList.ToArray());
                using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                gP.Transform(mx);
                using Pen pen = new Pen(Color.Yellow, 2);
                pen.LineJoin = LineJoin.Round;
                e.Graphics.DrawPath(pen, gP);
            }

            if (this._drawList2 != null && this._drawList2.Count > 1 && !_tracking2)
            {
                using GraphicsPath gP = new GraphicsPath();
                gP.AddLines(this._drawList2.ToArray());
                using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                gP.Transform(mx);
                if (this.cbTB.Checked && this._bmpBU != null)
                {
                    using TextureBrush tb = new TextureBrush(this._bmpBU);
                    tb.TranslateTransform(this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    tb.ScaleTransform(this.helplineRulerCtrl1.Zoom, this.helplineRulerCtrl1.Zoom);
                    using Pen pen = new Pen(tb, Math.Max((float)this.numDraw.Value * this.helplineRulerCtrl1.Zoom, 1.0f));
                    pen.LineJoin = LineJoin.Round;
                    e.Graphics.DrawPath(pen, gP);
                }
                else
                {
                    using Pen pen = new Pen(Color.Yellow, Math.Max((float)this.numDraw.Value * this.helplineRulerCtrl1.Zoom, 1.0f));
                    pen.LineJoin = LineJoin.Round;
                    e.Graphics.DrawPath(pen, gP);
                }
            }

            if (this._tracking2 && this.cbSelect.Checked)
            {
                using Pen pen = new Pen(Color.Red, 2);
                e.Graphics.DrawRectangle(pen, new RectangleF(this._aP.X, this._aP.Y, this._eP.X - this._aP.X, this._eP.Y - this._aP.Y));
            }

            if (this._eraseList != null && this._eraseList.Count > 1)
            {
                using GraphicsPath gP = new GraphicsPath();
                gP.AddLines(this._eraseList.ToArray());
                using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                gP.Transform(mx);
                using Pen pen = new Pen(Color.Cyan, Math.Max((float)this.numErase.Value * this.helplineRulerCtrl1.Zoom, 1.0f));
                pen.LineJoin = LineJoin.Round;
                e.Graphics.DrawPath(pen, gP);
            }
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            this._startPt = new Point(_ix, _iy);

            this.helplineRulerCtrl1.dbPanel1.Capture = true;

            if (this.checkedListBox1.Items.Count > 0)
            {
                if (_ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    int ii = -1;

                    for (int i = 0; i <= this.checkedListBox1.Items.Count - 1; i++)
                    {
                        using (GraphicsPath gPath = new GraphicsPath())
                        {
                            ChainCode c = (ChainCode)this.checkedListBox1.Items[i];
                            gPath.AddLines(c.Coord.ToArray());

                            if (gPath.IsVisible(_ix, _iy) || gPath.IsOutlineVisible(_ix, _iy, Pens.Black))
                                ii = i;
                        }
                    }

                    if (ii >= 0)
                    {
                        this.checkedListBox1.SetItemChecked(ii, true);
                        this.checkedListBox1.SelectedItem = this.checkedListBox1.Items[ii];
                    }
                }
            }

            if (this.cbErase.Checked && e.Button == MouseButtons.Left)
            {
                if (this._eraseList == null)
                    this._eraseList = new List<Point>();

                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                    this._eraseList.Add(new Point(_ix, _iy));

                this._tracking = true;
            }

            if (this.cbDraw.Checked && e.Button == MouseButtons.Left)
            {
                if (this._drawList2 == null)
                    this._drawList2 = new List<Point>();

                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                    this._drawList2.Add(new Point(_ix, _iy));

                this._tracking4 = true;
            }

            if (this.cbSelect.Checked && e.Button == MouseButtons.Left)
            {
                if (this._drawList == null)
                    this._drawList = new List<Point>();

                this._drawList.Clear();

                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    this._drawList.Add(new Point(_ix, _iy));
                    if (this.rbRect.Checked)
                        this._aP = new Point(e.X, e.Y);
                    this._tracking2 = true;
                }
            }

            if (!this.cbDraw.Checked && !this.cbErase.Checked)
                this._mouseIsDown = true;
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                this._eX2 = e.X;
                this._eY2 = e.Y;

                if (ix != _ix || iy != _iy)
                {
                    _ix = ix;
                    _iy = iy;

                    if (_ix < 0)
                        _ix = 0;

                    if (_iy < 0)
                        _iy = 0;

                    if (_ix > this.helplineRulerCtrl1.Bmp.Width - 1)
                        _ix = this.helplineRulerCtrl1.Bmp.Width - 1;

                    if (_iy > this.helplineRulerCtrl1.Bmp.Height - 1)
                        _iy = this.helplineRulerCtrl1.Bmp.Height - 1;

                    Bitmap b = (Bitmap)this.helplineRulerCtrl1.Bmp;
                    Color c = b.GetPixel(_ix, _iy);

                    toolStripStatusLabel1.Text = "x: " + _ix.ToString() + ", y: " + _iy.ToString() + " - " + "GrayValue (all channels): " + System.Convert.ToInt32(Math.Min(System.Convert.ToDouble(c.B) * 0.11 + System.Convert.ToDouble(c.G) * 0.59 + System.Convert.ToDouble(c.R) * 0.3, 255)).ToString() + " - RGB: " + c.R.ToString() + ";" + c.G.ToString() + ";" + c.B.ToString();

                    this.toolStripStatusLabel2.BackColor = c;

                    if (this._tracking)
                    {
                        if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                            this._eraseList?.Add(new Point(_ix, _iy));
                    }

                    if (this._tracking4)
                    {
                        if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                            this._drawList2?.Add(new Point(_ix, _iy));
                    }

                    if (this._tracking2)
                    {
                        if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height && this.rbPath.Checked)
                            this._drawList?.Add(new Point(_ix, _iy));

                        if (this.rbRect.Checked)
                            this._eP = new Point(e.X, e.Y);
                    }
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (this._tracking && e.Button == MouseButtons.Left)
            {
                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    this._eraseList?.Add(new Point(_ix, _iy));
                    this.AddPointsToErasePath();
                    ClearBmpRegions();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }

            if (this._tracking4 && e.Button == MouseButtons.Left)
            {
                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    this._drawList2?.Add(new Point(_ix, _iy));
                    if (this.cbTB.Checked)
                        this.AddPointsToDrawPath2_2();
                    else
                        this.AddPointsToDrawPath2();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }

            if (this._tracking2 && e.Button == MouseButtons.Left)
            {
                if (_ix >= 0 && _iy >= 0 /*&& _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height*/)
                {
                    if (_ix >= this.helplineRulerCtrl1.Bmp.Width)
                        _ix = this.helplineRulerCtrl1.Bmp.Width - 1;
                    if (_iy >= this.helplineRulerCtrl1.Bmp.Height)
                        _iy = this.helplineRulerCtrl1.Bmp.Height - 1;
                    this._drawList?.Add(new Point(_ix, _iy));
                    this.AddPointsToDrawPath();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }

            this._tracking = false;
            this._tracking2 = false;
            this._tracking4 = false;
            this._mouseIsDown = false;
            this.helplineRulerCtrl1.dbPanel1.Capture = false;
        }

        private void ClearBmpRegions()
        {
            if (this._erasePath != null && this._erasePath.PointCount > 1)
            {
                using GraphicsPath gP = (GraphicsPath)this._erasePath.Clone();
                using Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp);
                gx.CompositingMode = CompositingMode.SourceCopy;
                gx.FillPath(Brushes.Transparent, gP);

                using Pen pen = new Pen(Color.Transparent, (float)this.numErase.Value);
                pen.LineJoin = LineJoin.Round;
                gx.DrawPath(pen, gP);

                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this._pic_changed = true;
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();

                Bitmap bC = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                if (this._bmpFirstStrokes != null)
                    this.SetBitmap(ref this._bmpFirstStrokes, ref bC);
            }
        }

        private void AddPointsToDrawPath()
        {
            if (this._drawList == null)
                this._drawList = new List<Point>();
            if (this._regionPath == null)
                this._regionPath = new GraphicsPath();
            this._regionPath.Reset();

            if (this._drawList != null)
            {
                if (this.cbSelect.Checked)
                {
                    if (rbRect.Checked && this._drawList.Count > 1)
                    {
                        this._regionPath.AddRectangle(new RectangleF(this._drawList[0].X,
                                                                     this._drawList[0].Y,
                                                                     this._drawList[1].X - this._drawList[0].X,
                                                                     this._drawList[1].Y - this._drawList[0].Y));
                    }
                    else if (rbPath.Checked)
                        this._regionPath.AddLines(this._drawList.ToArray());
                }
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void AddPointsToDrawPath2()
        {
            if (this._drawList2 == null)
                this._drawList2 = new List<Point>();
            if (this._drawPath2 == null)
                this._drawPath2 = new GraphicsPath();
            this._drawPath2.Reset();

            if (this._drawList2 != null)
            {
                if (this.cbDraw.Checked)
                    this._drawPath2.AddLines(this._drawList2.ToArray());

                this._drawList2.Clear();

                DrawPathToBmp();

                this.cbUseExisting.Checked = true;
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void AddPointsToDrawPath2_2()
        {
            if (this._drawList2 == null)
                this._drawList2 = new List<Point>();
            if (this._drawPath2 == null)
                this._drawPath2 = new GraphicsPath();
            this._drawPath2.Reset();

            if (this._drawList2 != null)
            {
                if (this.cbDraw.Checked)
                    this._drawPath2.AddLines(this._drawList2.ToArray());

                this._drawList2.Clear();

                DrawPathToBmpWTB();

                this.cbUseExisting.Checked = true;
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void DrawPathToBmp()
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this._drawPath2 != null && this._drawPath2.PointCount > 1)
                {
                    using GraphicsPath gP = (GraphicsPath)this._drawPath2.Clone();
                    using Pen pen = new Pen(this.label8.BackColor, Math.Max((float)this.numDraw.Value, 1.0f));
                    using Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp);
                    pen.LineJoin = LineJoin.Round;
                    gx.DrawPath(pen, gP);

                    _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                    this._pic_changed = true;
                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }
            }

            if (this._bmpFirstStrokes != null)
            {
                if (this._drawPath2 != null && this._drawPath2.PointCount > 1)
                {
                    using GraphicsPath gP = (GraphicsPath)this._drawPath2.Clone();
                    using Pen pen = new Pen(Color.White, 2);
                    using Graphics gx = Graphics.FromImage(this._bmpFirstStrokes);
                    pen.LineJoin = LineJoin.Round;
                    gx.DrawPath(pen, gP);

                    _undoOPCache?.Add(this._bmpFirstStrokes);
                    this._pic_changed = true;
                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }
            }
        }

        private void DrawPathToBmpWTB()
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this._drawPath2 != null && this._drawPath2.PointCount > 1 && this._bmpBU != null)
                {
                    using GraphicsPath gP = (GraphicsPath)this._drawPath2.Clone();
                    using TextureBrush tb = new TextureBrush(this._bmpBU);
                    using Pen pen = new Pen(tb, Math.Max((float)this.numDraw.Value, 1.0f));
                    using Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp);
                    pen.LineJoin = LineJoin.Round;
                    gx.DrawPath(pen, gP);

                    _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                    this._pic_changed = true;
                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }
            }

            if (this._bmpFirstStrokes != null)
            {
                if (this._drawPath2 != null && this._drawPath2.PointCount > 1)
                {
                    using GraphicsPath gP = (GraphicsPath)this._drawPath2.Clone();
                    using Pen pen = new Pen(Color.White, 2);
                    using Graphics gx = Graphics.FromImage(this._bmpFirstStrokes);
                    pen.LineJoin = LineJoin.Round;
                    gx.DrawPath(pen, gP);

                    _undoOPCache?.Add(this._bmpFirstStrokes);
                    this._pic_changed = true;
                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }
            }
        }

        private void AddPointsToErasePath()
        {
            if (this._eraseList == null)
                this._eraseList = new List<Point>();
            if (this._erasePath == null)
                this._erasePath = new GraphicsPath();
            this._erasePath.Reset();

            if (this._eraseList != null)
            {
                if (this.cbErase.Checked)
                    this._erasePath.AddLines(this._eraseList.ToArray());

                this._eraseList.Clear();
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void bitmappanel1_DragOver(object? sender, DragEventArgs e)
        {
            if ((e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)))
                e.Effect = DragDropEffects.Copy;
        }

        private void bitmappanel1_DragDrop(object? sender, DragEventArgs e)
        {
            if ((e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)))
            {
                if (e.Effect == DragDropEffects.Copy)
                {
                    Bitmap? b1 = null;
                    Bitmap? b2 = null;

                    try
                    {
                        if (_pic_changed)
                        {
                            DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                            if (dlg == DialogResult.Yes)
                                button2.PerformClick();
                            else if (dlg == DialogResult.No)
                                _pic_changed = false;
                        }

                        if (e.Data != null)
                        {
                            String[]? files = (String[]?)e.Data.GetData(DataFormats.FileDrop);

                            if (files != null)
                            {
                                using (Image img = Image.FromFile(files[0]))
                                {
                                    if (AvailMem.AvailMem.checkAvailRam(img.Width * img.Height * 16L))
                                    {
                                        b1 = (Bitmap)img.Clone();
                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                        b2 = (Bitmap)img.Clone();
                                        this.SetBitmap(ref this._bmpBU, ref b2);
                                    }
                                    else
                                        throw new Exception();
                                }

                                _undoOPCache?.Clear(this.helplineRulerCtrl1.Bmp);

                                _pic_changed = false;

                                double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                                double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                                if (multiplier >= faktor)
                                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                                else
                                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

                                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                                this.Text = files[0] + " - frmQuickExtract";
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Fehler beim Laden des Bildes");

                        if (b1 != null)
                            b1.Dispose();

                        if (b2 != null)
                            b2.Dispose();
                    }
                }
            }
        }

        private void frmOutline_Load(object sender, EventArgs e)
        {
            foreach (string z in System.Enum.GetNames(typeof(GradientMode)))
                this.cmbGradMode.Items.Add(z.ToString());

            this.cmbGradMode.SelectedIndex = 2;

            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());

            this.cmbZoom.SelectedIndex = 4;

            this.numDivisor.Value = 4;
        }

        private void frmOutline_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_pic_changed & !_dontAskOnClosing)
            {
                DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dlg == DialogResult.Yes)
                {
                    button2.PerformClick();
                    e.Cancel = true;
                }
                else if (dlg == DialogResult.Cancel)
                    e.Cancel = true;
            }

            if (!e.Cancel)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();
                if (this.backgroundWorker2.IsBusy)
                    this.backgroundWorker2.CancelAsync();

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                this._bmpBU = null;

                if (this._bmpOrig != null)
                    this._bmpOrig.Dispose();
                this._bmpOrig = null;

                if (this._regionPath != null)
                    this._regionPath.Dispose();

                if (this._erasePath != null)
                    this._erasePath.Dispose();

                if (this._chainsBmp != null)
                    this._chainsBmp.Dispose();

                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();

                if (this._bmpFirstStrokes != null)
                    this._bmpFirstStrokes.Dispose();

                if (this._drawPath2 != null)
                    this._drawPath2.Dispose();

                if (this._bmpOutlineRef != null)
                    this._bmpOutlineRef.Dispose();

                //if (this._selectedFigure != null)
                //    this._selectedFigure.Dispose();
                //this._selectedFigure = null;

                if (this._hChain != null)
                    this._hChain.Dispose();
                this._hChain = null;
            }
        }

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBGColor.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.helplineRulerCtrl1.Bmp != null /*&& !this._dontDoZoom*/)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(cmbZoom.SelectedItem?.ToString());
                this.helplineRulerCtrl1.Enabled = true;
                if (this.cmbZoom.SelectedIndex < 2)
                    this.helplineRulerCtrl1.ZoomSetManually = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

                try
                {
                    if (this.helplineRulerCtrl1.Bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.helplineRulerCtrl1.Bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        _pic_changed = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (_pic_changed)
                {
                    DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                    if (dlg == DialogResult.Yes)
                        button2.PerformClick();
                    else if (dlg == DialogResult.No)
                        _pic_changed = false;
                }

                if (!_pic_changed)
                {
                    string f = this.Text.Split(new String[] { " - " }, StringSplitOptions.None)[0];
                    Bitmap? b1 = null;

                    try
                    {
                        if (this._bmpBU != null)
                        {
                            if (AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 12L))
                                b1 = (Bitmap)this._bmpBU.Clone();
                            else
                                throw new Exception();

                            this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");

                            this._pic_changed = false;

                            this.helplineRulerCtrl1.CalculateZoom();

                            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                            this.helplineRulerCtrl1.dbPanel1.Invalidate();

                            _undoOPCache?.Reset(false);
                        }
                    }
                    catch
                    {
                        if (b1 != null)
                            b1.Dispose();
                    }
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("");
        }

        private void SetBitmap(Bitmap bitmapToSet, Bitmap bitmapToBeSet, Control ct, string property)
        {
            Bitmap bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();

            if (ct != null)
            {
                if (ct.GetType().GetProperties().Where((a) => a.Name == property).Count() > 0)
                {
                    System.Reflection.PropertyInfo? pi = ct.GetType().GetProperty(property);
                    pi?.SetValue(ct, bitmapToBeSet);
                }
            }
        }

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            //using (GraphicsPath? gP = GetGP())
            //{
            //    if (gP != null)
            //    {
            //        GraphicsPath? pOld = this._selectedPath;
            //        this._selectedPath = (GraphicsPath?)gP.Clone();
            //        if (pOld != null)
            //            pOld.Dispose();
            //        pOld = null;
            //    }
            //}

            if (this.helplineRulerCtrl1.Bmp != null && this.checkedListBox1.SelectedItems.Count > 0)
            {
                List<ChainCode>? c = GetCC();
                if (c != null)
                {
                    this.SelectedChains = c;
                }
            }

            this._dontAskOnClosing = true;
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmOutline");
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
        }

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoUndo();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                    if (_undoOPCache.CurrentPosition < 1)
                    {
                        this.btnUndo.Enabled = false;
                        this._pic_changed = false;
                    }

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.CheckRedoButton();
                }
                else
                    MessageBox.Show("Error while undoing.");
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoRedo();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                    this._pic_changed = true;

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.CheckRedoButton();

                    this.btnUndo.Enabled = true;
                }
                else
                    MessageBox.Show("Error while redoing.");
            }
        }

        private void cmbGradMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.cmbGradMode.SelectedIndex)
            {
                case 0:
                    this.numDivisor.Value = 1;
                    break;
                case 1:
                    this.numDivisor.Value = 4;
                    break;
                case 2:
                    this.numDivisor.Value = 16;
                    break;
                case 3:
                    this.numDivisor.Value = 256;
                    break;

                default:
                    break;
            }
        }

        private void btnGrad_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            if (this.helplineRulerCtrl1.Bmp != null && this.cmbGradMode.SelectedItem != null)
            {
                string? f = this.cmbGradMode.SelectedItem.ToString();
                if (f != null)
                {
                    SetControls(false);
                    this.btnGrad.Text = "Cancel";
                    this.btnGrad.Enabled = true;
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    GradientMode gradMode = (GradientMode)System.Enum.Parse(typeof(GradientMode), f);
                    double divisor = (double)this.numDivisor.Value;

                    object[] o = { gradMode, divisor };

                    this.backgroundWorker1.RunWorkerAsync(o);
                }
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (this._bmpBU != null && e.Argument != null)
            {
                Bitmap bmp = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();

                object[] o = (object[])e.Argument;

                GradientMode gradientMode = (GradientMode)o[0];
                double divisor = (double)o[1];

                EdgeDetectionMethods edgeDetectionMethods = new();
                edgeDetectionMethods.GetGradMag(bmp, gradientMode, divisor, _rc, this.backgroundWorker1);

                e.Result = bmp;
            }
        }

        private void backgroundWorker1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (!InvokeRequired)
            {
                if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                    this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
            }
            else
                this.Invoke(new Action(() =>
                {
                    if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                        this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
                }));
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (!this.IsDisposed && this.Visible)
                {
                    Bitmap? bmpNew = null;

                    try
                    {
                        bmpNew = (Bitmap)e.Result;

                        if (bmpNew != null)
                        {
                            this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmpNew, this.helplineRulerCtrl1, "Bmp");
                            Bitmap bC = (Bitmap)bmpNew.Clone();
                            this.SetBitmap(ref _bmpOutlineRef, ref bC);
                            _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                            this.btnUndo.Enabled = true;
                            CheckRedoButton();
                            this._pic_changed = true;
                        }
                    }
                    catch
                    {
                    }

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.CalculateZoom();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    SetControls(true);

                    this.Cursor = Cursors.Default;

                    if (this.Timer3.Enabled)
                        this.Timer3.Stop();
                    this.toolStripProgressBar1.Value = 100;
                    this.Timer3.Start();

                    this.toolStripProgressBar1.Visible = false;
                    this.btnGrad.Text = "Go";

                    // create a new bgw, since it reported "complete"
                    this.backgroundWorker1.Dispose();
                    this.backgroundWorker1 = new BackgroundWorker();
                    this.backgroundWorker1.WorkerReportsProgress = true;
                    this.backgroundWorker1.WorkerSupportsCancellation = true;
                    this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                    this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                    this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
                }
            }
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.splitContainer1.Panel1.Controls)
                    {
                        if (ct.Name != "btnCancel")
                            ct.Enabled = e;

                        if (ct is Panel)
                        {
                            ct.Enabled = true;
                            foreach (Control ct2 in ct.Controls)
                                ct2.Enabled = e;
                        }
                    }

                    this.helplineRulerCtrl1.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.splitContainer1.Panel1.Controls)
                {
                    if (ct.Name != "btnCancel")
                        ct.Enabled = e;

                    if (ct is Panel)
                    {
                        ct.Enabled = true;
                        foreach (Control ct2 in ct.Controls)
                            ct2.Enabled = e;
                    }
                }

                this.helplineRulerCtrl1.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.IsDisposed == false && this.Visible && this.helplineRulerCtrl1.Bmp != null)
            {
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                {
                    if (this.backgroundWorker2.IsBusy)
                        this.backgroundWorker2.CancelAsync();

                    SetControls(false);

                    Bitmap? bmp = null;

                    if (this.cbChainFromOrig.Checked && this._bmpFirstStrokes != null)
                        bmp = (Bitmap)this._bmpFirstStrokes.Clone();
                    else
                        bmp = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();

                    int mOpacity = (int)this.numChainTolerance.Value;

                    this.backgroundWorker2.RunWorkerAsync(new object[] { bmp, mOpacity });
                }
            }
        }

        private void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                if (o == null)
                    return;

                List<ChainCode>? l = new List<ChainCode>();

                using (Bitmap bmp = (Bitmap)o[0])
                {
                    bool transpMode = true;
                    int minOpacity = (int)o[1];
                    l = GetBoundary(bmp, minOpacity, transpMode);

                    if (l != null)
                    {
                        foreach (ChainCode c in l)
                            c.SetId();
                    }
                }

                e.Result = l;
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
                {
                    Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                    List<ChainCode> l = (List<ChainCode>)e.Result;

                    ChainFinder cf = new ChainFinder();
                    cf.DrawOutlineToBmp(bmp, this.helplineRulerCtrl1.Bmp, l);

                    if (l != null)
                    {
                        l = l.OrderByDescending((a) => a.Chain.Count).ToList();
                        this.checkedListBox1.Items.Clear();

                        this.checkedListBox1.SuspendLayout();
                        this.checkedListBox1.BeginUpdate();

                        int cnt = l.Count;
                        if (this.cbRestrict.Checked)
                            cnt = Math.Min(l.Count, (int)this.numRestrict.Value);

                        for (int i = 0; i < cnt; i++)
                            this.checkedListBox1.Items.Add(l[i], false);

                        this.checkedListBox1.EndUpdate();
                        this.checkedListBox1.ResumeLayout();

                        SetBitmap(ref this._chainsBmp, ref bmp);

                        if (l.Count > 0)
                        {
                            this.checkedListBox1.SetItemChecked(0, true);
                            this.checkedListBox1.SetSelected(0, true);
                        }
                    }
                }

                this.SetControls(true);

                this.backgroundWorker2.Dispose();
                this.backgroundWorker2 = new BackgroundWorker();
                this.backgroundWorker2.WorkerReportsProgress = true;
                this.backgroundWorker2.WorkerSupportsCancellation = true;
                this.backgroundWorker2.DoWork += backgroundWorker2_DoWork;
                //this.backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
                this.backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private List<ChainCode>? GetBoundary(Bitmap upperImg, int minAlpha, bool transpMode)
        {
            List<ChainCode>? l = null;
            Bitmap? bmpTmp = null;
            try
            {
                if (AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L))
                    bmpTmp = (Bitmap)upperImg.Clone();
                else
                    throw new Exception("Not enough memory.");

                int nWidth = bmpTmp.Width;
                int nHeight = bmpTmp.Height;
                ChainFinder cf = new ChainFinder();

                if (transpMode)
                    lock (this._lockObject)
                        l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, false, 0, false, 0, false);
                else
                    lock (this._lockObject)
                        l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, true, 0, false, 0, false);
            }
            catch (Exception exc)
            {
                OnBoundaryError(exc.Message);
            }
            finally
            {
                if (bmpTmp != null)
                {
                    bmpTmp.Dispose();
                    bmpTmp = null;
                }
            }

            if (l != null)
                return l;
            else
                return null;
        }

        private void OnBoundaryError(string message)
        {
            BoundaryError?.Invoke(this, message);
        }

        private void ClearExistingPaths()
        {
            this.checkedListBox1.Items.Clear();
            if (this._chainsBmp != null)
                this._chainsBmp.Dispose();
            this._chainsBmp = null;
            if (this._hChain != null)
                this._hChain.Dispose();
            this._hChain = null;
        }

        private void btnClearPaths_Click(object sender, EventArgs e)
        {
            ClearExistingPaths();
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void cbSelSingleClick_CheckedChanged(object sender, EventArgs e)
        {
            this.checkedListBox1.CheckOnClick = this.cbSelSingleClick.Checked;
        }

        private void btnSelAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
                this.checkedListBox1.SetItemChecked(i, true);

            checkedListBox1_SelectedIndexChanged(this.checkedListBox1, new EventArgs());
        }

        private void btnSelNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
                this.checkedListBox1.SetItemChecked(i, false);

            checkedListBox1_SelectedIndexChanged(this.checkedListBox1, new EventArgs());
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<ChainCode> fList = new List<ChainCode>();
            for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                if (this.checkedListBox1.CheckedItems[i] != null)
                {
                    ChainCode? f = this.checkedListBox1.CheckedItems[i] as ChainCode;
                    if (f != null)
                        fList.Add(f);
                }

            if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 5L))
            {
                Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                ChainFinder cf = new ChainFinder();
                cf.DrawOutlineToBmp(bmp, this.helplineRulerCtrl1.Bmp, fList);
                cf.HighLightOutlines(bmp, fList, this.cbBGColor.Checked);

                SetBitmap(ref this._chainsBmp, ref bmp);
            }

            if (this.checkedListBox1.SelectedIndex > -1)
            {
                ChainCode? c = this.checkedListBox1.SelectedItem as ChainCode;
                if (c != null)
                {
                    this.label6.Text = "Area: " + c.Area.ToString();
                    this.label7.Text = "Perimeter: " + c.Perimeter.ToString();

                    GraphicsPath? pOld = this._hChain;
                    _hChain = new GraphicsPath();
                    _hChain.AddLines(c.Coord.ToArray());

                    if (pOld != null)
                        pOld.Dispose();

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private GraphicsPath? GetGP()
        {
            GraphicsPath? gP = new GraphicsPath();

            if (this.checkedListBox1.SelectedIndex > -1)
            {
                for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                {
                    ChainFinder cf = new ChainFinder();
                    ChainCode? c = this.checkedListBox1.CheckedItems[i] as ChainCode;

                    if (c != null)
                    {
                        List<Point> l = c.Coord;

                        if (l.Count > 1)
                        {
                            gP.StartFigure();
                            gP.AddLines(l.ToArray());
                        }
                    }
                }
            }

            return gP;
        }

        private List<ChainCode>? GetCC()
        {
            List<ChainCode>? c = new List<ChainCode>();

            if (this.checkedListBox1.SelectedIndex > -1)
            {
                for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                {
                    ChainFinder cf = new ChainFinder();
                    ChainCode? cc = this.checkedListBox1.CheckedItems[i] as ChainCode;
                    if (cc != null)
                        c.Add(cc);
                }
            }

            return c;
        }

        private void helplineRulerCtrl1_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            //this._dontDoZoom = true;

            if (e.Zoom == 1.0F)
                this.cmbZoom.SelectedIndex = 2;
            else if (e.ZoomWidth)
                this.cmbZoom.SelectedIndex = 3;
            else
                this.cmbZoom.SelectedIndex = 4;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            //this._dontDoZoom = false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.I | Keys.Control | Keys.Shift))
            {
                if (this.helplineRulerCtrl1.Bmp != null)
                    MessageBox.Show(this.helplineRulerCtrl1.Bmp.Size.ToString());

                return true;
            }

            if (keyData == Keys.Enter)
            {
                this.helplineRulerCtrl1._contentPanel_MouseDoubleClick(this.helplineRulerCtrl1.dbPanel1, new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 2, 0, 0, 0));

                return true;
            }

            if (keyData == (Keys.Z | Keys.Control))
            {
                this.btnUndo.PerformClick();
                return true;
            }

            if (keyData == (Keys.Y | Keys.Control))
            {
                //this.btnRedo.PerformClick();
                return true;
            }

            //if (keyData == Keys.F3)
            //{
            //    if (this.FrmSetRamFreeable != null && this.FrmSetRamFreeable.Visible)
            //    {
            //        this.FrmSetRamFreeable.Show();
            //        this.FrmSetRamFreeable.BringToFront();
            //    }
            //    else
            //    {
            //        this.FrmSetRamFreeable = new frmSetRamFreeableAmount(this);
            //        this.FrmSetRamFreeable.Show();
            //    }
            //}

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnGetSelection_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._regionPath != null && this._regionPath.PointCount > 1)
            {
                Bitmap? bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                using Graphics gx = Graphics.FromImage(bmp);
                using TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp);
                gx.FillPath(tb, this._regionPath);

                RectangleF rc = this._regionPath.GetBounds();
                _rc = new Rectangle((int)rc.X, (int)rc.Y, (int)rc.Width, (int)rc.Height);

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                _undoOPCache?.Add(bmp);

                this.btnUndo.Enabled = true;
                CheckRedoButton();

                this._pic_changed = true;
                this._regionPath.Reset();
                this.cbSelect.Checked = false;

                this.panel1.Enabled = this.panel2.Enabled = this.panel3.Enabled =
                    this.panel4.Enabled = this.panel5.Enabled = this.panel6.Enabled = true;

                if (this._drawList != null)
                    this._drawList.Clear();

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                SetControls(false);

                int tolerance = (int)this.numReplace.Value;
                EdgeDetectionMethods.ReplaceColors(this.helplineRulerCtrl1.Bmp, 0, 0, 0, 0, tolerance, 255, 0, 0, 0);
                SetFGColorWhite(this.helplineRulerCtrl1.Bmp, 5);
                this._undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();

                SetControls(true);
            }
        }

        private unsafe void SetFGColorWhite(Bitmap bmp, int v)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] > 0 && (p[0] > v || p[1] > v || p[2] > v))
                        p[0] = p[1] = p[2] = p[3] = 255;

                    p += 4;
                }
            });

            bmp.UnlockBits(bmD);
        }

        private void cbErase_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbErase.Checked)
                this.cbDraw.Checked = false;
        }

        private void cbDraw_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbDraw.Checked)
                this.cbErase.Checked = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bmp = null;

                if (this.cbUseExisting.Checked)
                    if (this._bmpFirstStrokes == null)
                    {
                        bmp = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                        this.SetBitmap(ref this._bmpFirstStrokes, ref bmp);
                    }
                    else
                        bmp = (Bitmap)this._bmpFirstStrokes.Clone();
                else
                {
                    bmp = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                    this.SetBitmap(ref this._bmpFirstStrokes, ref bmp);
                }

                List<Point> pts = GetWhitePoints();
                Bitmap b = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                using Graphics gx = Graphics.FromImage(b);
                for (int i = 0; i < pts.Count; i++)
                    gx.FillRectangle(Brushes.White, pts[i].X - (int)this.numPrevWidth.Value / 2, pts[i].Y - (int)this.numPrevWidth.Value / 2, (int)this.numPrevWidth.Value, (int)this.numPrevWidth.Value);

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");
                this._undoOPCache?.Add(b);

                this._pic_changed = true;

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.CheckRedoButton();

                this.btnUndo.Enabled = true;
            }
        }

        private unsafe List<Point> GetWhitePoints()
        {
            List<Point> pts = new List<Point>();
            if (this._bmpFirstStrokes != null)
            {
                int w = this._bmpFirstStrokes.Width;
                int h = this._bmpFirstStrokes.Height;

                BitmapData bmD = this._bmpFirstStrokes.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                byte* p = (byte*)bmD.Scan0;

                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                        if (p[x * 4 + y * stride + 3] > 0)
                            pts.Add(new Point(x, y));
                }

                this._bmpFirstStrokes.UnlockBits(bmD);
            }

            return pts;
        }

        private void cbIncOpacity_CheckedChanged(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnCentralLine_Click(object sender, EventArgs e)
        {
            if (this.checkedListBox1.SelectedIndex > -1 && this.helplineRulerCtrl1.Bmp != null && this._bmpBU != null)
            {
                List<ChainCode> fList = new List<ChainCode>();
                for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                    if (this.checkedListBox1.CheckedItems[i] != null)
                    {
                        ChainCode? f = this.checkedListBox1.CheckedItems[i] as ChainCode;
                        if (f != null)
                            fList.Add(f);
                    }

                using Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                using Graphics gx = Graphics.FromImage(bmp);
                for (int i = 0; i < fList.Count; i++)
                {
                    GraphicsPath gP = new GraphicsPath();
                    gP.AddLines(fList[i].Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                    gx.FillPath(Brushes.Orange, gP);
                    using Pen pen = new Pen(Color.Green, Math.Max((float)this.numPrevWidth.Value, 1.0f));
                    gx.DrawPath(pen, gP);
                }

                using Bitmap bForeground = RemoveOutlineEx(bmp, (int)((float)this.numPrevWidth.Value * (float)this.numWidthFactor.Value), true);

                List<ChainCode> l4 = GetBoundary(bForeground);
                Bitmap bmp2 = new Bitmap(bForeground.Width, bForeground.Height);

                using Graphics gx4 = Graphics.FromImage(bmp2);
                using TextureBrush tb = new TextureBrush(this._bmpBU);
                for (int i = 0; i < l4.Count; i++)
                {
                    using GraphicsPath gP2 = new GraphicsPath();
                    gP2.AddLines(l4[i].Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                    gx4.FillPath(tb, gP2);
                }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp2, this.helplineRulerCtrl1, "Bmp");
                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.btnUndo.Enabled = true;
                CheckRedoButton();
                this._pic_changed = true;

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.CalculateZoom();
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                //this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                //this.helplineRulerCtrl1.dbPanel1.Invalidate();

                Form fff = new Form();
                fff.BackgroundImage = bmp2;
                fff.BackgroundImageLayout = ImageLayout.Zoom;
                fff.ShowDialog();
            }
        }

        private Bitmap RemoveOutlineEx(Bitmap bmp, int innerW, bool dontFill)
        {
            Bitmap bOut = new Bitmap(bmp.Width, bmp.Height);

            using (Bitmap? b = RemOutline(bmp, innerW, null))
            {
                using (Graphics gx = Graphics.FromImage(bOut))
                {
                    gx.SmoothingMode = SmoothingMode.None;
                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                    if (b != null)
                        gx.DrawImage(b, 0, 0);
                }
            }

            List<ChainCode> lInner = GetBoundary(bOut);

            if (lInner.Count > 0)
            {
                lInner = lInner.OrderByDescending(a => a.Coord.Count).ToList();

                for (int i = 0; i < lInner.Count; i++)
                {
                    List<PointF> pts = lInner[i].Coord.Select(a => new PointF(a.X, a.Y)).ToList();

                    if (pts.Count > 2)
                    {
                        using (GraphicsPath gp = new GraphicsPath())
                        {
                            gp.AddLines(pts.ToArray());

                            if (gp.PointCount > 0)
                            {
                                using (Graphics gx = Graphics.FromImage(bOut))
                                {
                                    gx.SmoothingMode = SmoothingMode.None;
                                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    gx.FillPath(Brushes.Black, gp);
                                    using (Pen p = new Pen(Color.Red, 1))
                                        gx.DrawPath(p, gp);
                                }
                            }
                        }
                    }
                }
            }

            if (dontFill)
                for (int i = 0; i < lInner.Count; i++)
                {
                    if (ChainFinder.IsInnerOutline(lInner[i]))
                    {
                        List<PointF> pts = lInner[i].Coord.Select(a => new PointF(a.X, a.Y)).ToList();

                        if (pts.Count > 2)
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                gp.AddLines(pts.ToArray());

                                if (gp.PointCount > 0)
                                {
                                    using (Graphics gx = Graphics.FromImage(bOut))
                                    {
                                        gx.SmoothingMode = SmoothingMode.None;
                                        gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                                        gx.CompositingMode = CompositingMode.SourceCopy;
                                        gx.FillPath(Brushes.Transparent, gp);
                                        gx.DrawPath(Pens.Transparent, gp);
                                    }
                                }
                            }
                        }
                    }
                }

            return bOut;
        }

        private List<ChainCode> GetBoundary(Bitmap bmp)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, 0, false, 0, false, 0, false);
            return l;
        }

        public Bitmap? RemOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
            {
                Bitmap? b = null;

                try
                {
                    b = (Bitmap)bmp.Clone();

                    for (int i = 0; i < breite; i++)
                    {
                        if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                            break;

                        ChainFinder cf = new ChainFinder();
                        cf.AllowNullCells = true;

                        List<ChainCode> fList = cf.GetOutline(b, 0, false, 0, false);

                        cf.RemoveOutline(b, fList);
                    }

                    return b;
                }
                catch
                {
                    if (b != null)
                        b.Dispose();

                    b = null;
                }
            }

            return null;
        }

        private void cbChainFromOrig_CheckedChanged(object sender, EventArgs e)
        {
            this.label11.Enabled = this.btnCentralLine.Enabled = !this.cbChainFromOrig.Checked;
        }

        private void btnNewPath_Click(object sender, EventArgs e)
        {
            if (this._drawList != null)
                this._drawList.Clear();
        }

        private void btnRemPt_Click(object sender, EventArgs e)
        {
            if (this._drawList != null && this._drawList.Count > 0)
            {
                this._drawList.RemoveAt(this._drawList.Count - 1);
                AddPointsToDrawPath();
            }
        }

        private void btnRem1Px_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._bmpOutlineRef != null)
            {
                Bitmap? bForeground = RemOutline(this._bmpOutlineRef, 2, null); //RemoveOutlineEx(this.helplineRulerCtrl1.Bmp, 1, false);
                SetPixelsFromRefPic(bForeground, this.helplineRulerCtrl1.Bmp);

                //if (bForeground != null)
                //    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bForeground, this.helplineRulerCtrl1, "Bmp");

                this._undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();
            }
        }

        private unsafe void SetPixelsFromRefPic(Bitmap? bOpaque, Bitmap bmp)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            if (bOpaque != null)
            {
                BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmOp = bOpaque.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    byte* pRead = (byte*)bmOp.Scan0;
                    p += y * stride;
                    pRead += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        if (pRead[3] == 0)
                            p[3] = 0;
                        p += 4;
                        pRead += 4;
                    }
                });

                bmp.UnlockBits(bmD);
                bOpaque.UnlockBits(bmOp);
            }
        }

        private void btnBlur_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }

            if (!this.backgroundWorker3.IsBusy)
            {
                int krnl = (int)this.numKernel.Value;
                int maxVal = (int)this.numDistWeight.Value;

                SetControls(false);

                this.btnBlur.Text = "Cancel";
                this.btnBlur.Enabled = true;

                if (this.helplineRulerCtrl1.Bmp != null)
                {
                    Bitmap bmp = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                    this.backgroundWorker3.RunWorkerAsync(new object[] { bmp, krnl, maxVal });
                }
            }
        }

        private void backgroundWorker3_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = (Bitmap)o[0];
                int krnl = (int)o[1];
                int maxVal = (int)o[2];

                Convolution conv = new();
                conv.ProgressPlus += Conv_ProgressPlus;
                conv.CancelLoops = false;

                InvGaussGradOp igg = new InvGaussGradOp();
                igg.BGW = this.backgroundWorker3;

                igg.FastZGaussian_Blur_NxN_SigmaAsDistance(bmp, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);
                conv.ProgressPlus -= Conv_ProgressPlus;

                e.Result = bmp;
            }
        }

        private void Conv_ProgressPlus(object sender, ProgressEventArgs e)
        {
            this.backgroundWorker3.ReportProgress(Math.Min((int)(((double)e.CurrentProgress / (double)e.ImgWidthHeight) * 100), 100));
        }

        private void backgroundWorker3_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (!InvokeRequired)
            {
                if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                    this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
            }
            else
                this.Invoke(new Action(() =>
                {
                    if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                        this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
                }));
        }

        private void backgroundWorker3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                if (!this.IsDisposed && this.Visible)
                {
                    Bitmap? bmpNew = null;

                    try
                    {
                        bmpNew = (Bitmap)e.Result;

                        if (bmpNew != null)
                        {
                            this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmpNew, this.helplineRulerCtrl1, "Bmp");
                            _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                            this.btnUndo.Enabled = true;
                            CheckRedoButton();
                            this._pic_changed = true;
                        }
                    }
                    catch
                    {
                    }

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.CalculateZoom();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    SetControls(true);

                    this.Cursor = Cursors.Default;

                    if (this.Timer3.Enabled)
                        this.Timer3.Stop();
                    this.toolStripProgressBar1.Value = 100;
                    this.Timer3.Start();

                    this.toolStripProgressBar1.Visible = false;
                    this.btnBlur.Text = "Blur";

                    // create a new bgw, since it reported "complete"
                    this.backgroundWorker3.Dispose();
                    this.backgroundWorker3 = new BackgroundWorker();
                    this.backgroundWorker3.WorkerReportsProgress = true;
                    this.backgroundWorker3.WorkerSupportsCancellation = true;
                    this.backgroundWorker3.DoWork += backgroundWorker3_DoWork;
                    this.backgroundWorker3.ProgressChanged += backgroundWorker3_ProgressChanged;
                    this.backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
                }
            }
        }

        private void btnColors_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                byte[] rgb = new byte[256];
                List<Point> p = new();
                p.Add(new Point(0, 0));
                p.Add(new Point((int)this.numValSrc.Value, (int)this.numValDst.Value));
                p.Add(new Point(255, 255));

                CurveSegment cuSgmt = new();
                List<BezierSegment> bz = cuSgmt.CalcBezierSegments(p.ToArray(), 0.5f);
                List<PointF> pts = cuSgmt.GetAllPoints(bz, 256, 0, 255);
                cuSgmt.MapPoints(pts, rgb);

                ColorCurves.fipbmp.GradColors(this.helplineRulerCtrl1.Bmp, rgb, rgb, rgb);

                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.btnUndo.Enabled = true;
                CheckRedoButton();
                this._pic_changed = true;

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.CalculateZoom();
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnColorCurves_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                ColorCurves.frmColorCurves frm = new(this.helplineRulerCtrl1.Bmp, "", 0);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    byte[] r = frm.MappingsRed;
                    byte[] g = frm.MappingsGreen;
                    byte[] b = frm.MappingsBlue;
                    ColorCurves.fipbmp.GradColors(this.helplineRulerCtrl1.Bmp, r, g, b);

                    _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                    this.btnUndo.Enabled = true;
                    CheckRedoButton();
                    this._pic_changed = true;

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.CalculateZoom();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private void btnInvGaussGrad_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker4.IsBusy)
            {
                this.backgroundWorker4.CancelAsync();
                return;
            }

            if (!this.backgroundWorker4.IsBusy && this.helplineRulerCtrl1.Bmp != null)
            {
                SetControls(false);

                this.btnInvGaussGrad.Text = "Cancel";
                this.btnInvGaussGrad.Enabled = true;

                this.toolStripProgressBar1.Value = 0;

                int kernelLength = (int)numIGGKernel.Value;
                double cornerWeight = 0.01;
                int sigma = 255;
                double steepness = 1E-12;
                int radius = 340;
                double alpha = (double)this.numIGGAlpha.Value * 255.0;
                GradientMode gradientMode = GradientMode.Scharr16;
                double divisor = (double)this.numIGGDivisor.Value;
                bool grayscale = false;
                bool stretchValues = true;
                int threshold = 127;

                object[] o = { kernelLength, cornerWeight, sigma, steepness,
                               radius, alpha, gradientMode, divisor, grayscale, stretchValues,
                               threshold };

                this.backgroundWorker4.RunWorkerAsync(o);
            }
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null && this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap bmp = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();

                object[] o = (object[])e.Argument;

                int kernelLength = (int)o[0];
                double cornerWeight = (double)o[1];
                int sigma = (int)o[2];
                double steepness = (double)o[3];
                int radius = (int)o[4];
                double alpha = (double)o[5];
                GradientMode gradientMode = (GradientMode)o[6];
                double divisor = (double)o[7];
                bool grayscale = (bool)o[8];
                bool stretchValues = (bool)o[9];
                int threshold = (int)o[10];

                Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (r.Width > 0 && r.Height > 0)
                    {
                        InvGaussGradOp igg = new InvGaussGradOp();
                        igg.BGW = this.backgroundWorker4;
                        Bitmap? iG = igg.Inv_InvGaussGrad(bmp, alpha, gradientMode, divisor, kernelLength, cornerWeight,
                            sigma, steepness, radius, stretchValues, threshold);
                        e.Result = iG;
                    }
                }
            }
        }

        private void backgroundWorker4_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= this.toolStripProgressBar1.Maximum)
                this.toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap bmp = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                this._undoOPCache?.Add(bmp);

                this._pic_changed = true;

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.SetControls(true);
                this.btnUndo.Enabled = true;

                this.CheckRedoButton();

                this.btnInvGaussGrad.Text = "Go";

                this.backgroundWorker4.Dispose();
                this.backgroundWorker4 = new BackgroundWorker();
                this.backgroundWorker4.WorkerReportsProgress = true;
                this.backgroundWorker4.WorkerSupportsCancellation = true;
                this.backgroundWorker4.DoWork += backgroundWorker4_DoWork;
                this.backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
                this.backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
            }
        }

        private void btnScanLWCW_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker5.IsBusy)
            {
                this.backgroundWorker5.CancelAsync();
                return;
            }

            if (!this.backgroundWorker5.IsBusy && this.helplineRulerCtrl1.Bmp != null)
            {
                SetControls(false);

                this.btnScanLWCW.Text = "Cancel";
                this.btnScanLWCW.Enabled = true;

                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                Bitmap? bWork = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                int dir = 0;
                if (this.rbR.Checked)
                    dir = 1;
                if (this.rbT.Checked)
                    dir = 2;
                if (this.rbB.Checked)
                    dir = 3;
                int tolerance = (int)this.numSCTol.Value;

                this.backgroundWorker5.RunWorkerAsync(new object[] { bWork, dir, tolerance });
            }
        }

        private void backgroundWorker5_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap? bWork = (Bitmap?)o[0];
                int dir = (int)o[1];
                int tolerance = (int)o[2];

                ScanLinesOrColumns(bWork, dir, tolerance);

                e.Result = bWork;
            }
        }

        private unsafe void ScanLinesOrColumns(Bitmap? bWork, int dir, int tolerance)
        {
            if (bWork != null)
            {
                int w = bWork.Width;
                int h = bWork.Height;

                BitmapData bmD = bWork.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = (int)bmD.Stride;

                if (dir == 0)
                {
                    Parallel.For(0, h, y =>
                    {
                        byte* p = (byte*)bmD.Scan0;
                        p += y * stride;

                        for (int x = 0; x < w; x++)
                        {
                            if (p[3] == 0 || ((int)p[0] + (int)p[1] + (int)p[2] <= tolerance))
                                p[3] = 0;
                            else if (p[3] != 0 && ((int)p[0] + (int)p[1] + (int)p[2] > tolerance))
                                break;

                            p += 4;
                        }
                    });
                }
                else if (dir == 1)
                {
                    Parallel.For(0, h, y =>
                    {
                        byte* p = (byte*)bmD.Scan0;
                        p += y * stride + (w - 1) * 4;

                        for (int x = w - 1; x >= 0; x--)
                        {
                            if (p[3] == 0 || ((int)p[0] + (int)p[1] + (int)p[2] <= tolerance))
                                p[3] = 0;
                            else if (p[3] != 0 && ((int)p[0] + (int)p[1] + (int)p[2] > tolerance))
                                break;

                            p -= 4;
                        }
                    });
                }
                else if (dir == 2)
                {
                    Parallel.For(0, w, x =>
                    {
                        byte* p = (byte*)bmD.Scan0;
                        p += x * 4;

                        for (int y = 0; y < h; y++)
                        {
                            if (p[3] == 0 || ((int)p[0] + (int)p[1] + (int)p[2] <= tolerance))
                                p[3] = 0;
                            else if (p[3] != 0 && ((int)p[0] + (int)p[1] + (int)p[2] > tolerance))
                                break;

                            p += stride;
                        }
                    });
                }
                else if (dir == 3)
                {
                    Parallel.For(0, w, x =>
                        {
                            byte* p = (byte*)bmD.Scan0;
                            p += x * 4 + (h - 1) * stride;

                            for (int y = h - 1; y >= 0; y--)
                            {
                                if (p[3] == 0 || ((int)p[0] + (int)p[1] + (int)p[2] <= tolerance))
                                    p[3] = 0;
                                else if (p[3] != 0 && ((int)p[0] + (int)p[1] + (int)p[2] > tolerance))
                                    break;

                                p -= stride;
                            }
                        });
                }

                bWork.UnlockBits(bmD);
            }
        }

        private void backgroundWorker5_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {

        }

        private void backgroundWorker5_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap bmp = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                this._undoOPCache?.Add(bmp);

                this._pic_changed = true;

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.SetControls(true);
                this.btnUndo.Enabled = true;

                this.CheckRedoButton();

                this.btnScanLWCW.Text = "Go";

                this.backgroundWorker5.Dispose();
                this.backgroundWorker5 = new BackgroundWorker();
                this.backgroundWorker5.WorkerReportsProgress = true;
                this.backgroundWorker5.WorkerSupportsCancellation = true;
                this.backgroundWorker5.DoWork += backgroundWorker5_DoWork;
                this.backgroundWorker5.ProgressChanged += backgroundWorker5_ProgressChanged;
                this.backgroundWorker5.RunWorkerCompleted += backgroundWorker5_RunWorkerCompleted;
            }
        }

        private void cbOverlay_CheckedChanged(object sender, EventArgs e)
        {
            this._overlay = this.cbOverlay.Checked;
            this.cbIncOpacity.Enabled = this.cbOverlay.Checked;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnFloodfill_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.cbSelect.Checked = false;
                int tol = (int)this.numFFTol.Value;
                Color startColor = this.helplineRulerCtrl1.Bmp.GetPixel(this._startPt.X, this._startPt.Y);
                Color replaceColor = Color.FromArgb(0, 0, 0, 0);
                FloodFillMethods.floodfill(this.helplineRulerCtrl1.Bmp, this._startPt.X, this._startPt.Y, tol, startColor, replaceColor,
                    Int32.MaxValue, false, false, 1.0, false, false);
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this._undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this._pic_changed = true;
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();
            }
        }

        private void btnShrExt_Click(object sender, EventArgs e)
        {
            if (this._bmpOrig != null && this.helplineRulerCtrl1.Bmp != null && this.CachePathAddition != null)
            {
                frmExtend frm = new frmExtend(this.helplineRulerCtrl1.Bmp, this._bmpOrig, this.CachePathAddition);
                frm.SetupCache();

                frm.numExtendOrShrink.Value = (decimal)-14;
                frm.cbMorphological.Checked = true;
                frm.cbDiskShaped.Checked = true;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Bitmap? bmp = frm.FBitmap;

                    if (bmp != null)
                    {
                        Bitmap b = (Bitmap)bmp.Clone();

                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                        //Bitmap? bC = new Bitmap(b);
                        //this.SetBitmap(ref _bmpRef, ref bC);

                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                        this._undoOPCache?.Add(b);
                    }
                }
            }
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            this.colorDialog1.CustomColors = CustomColors;
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.label8.BackColor = this.colorDialog1.Color;
                CustomColors = this.colorDialog1.CustomColors;
            }
        }
    }
}
