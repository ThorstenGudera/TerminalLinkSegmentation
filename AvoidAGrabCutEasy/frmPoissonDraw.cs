using Cache;
using ChainCodeFinder;
using ConvolutionLib;
using PoissonBlend;
using QuickExtract2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmPoissonDraw : Form
    {
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
                m_CachePathAddition = value;
            }
        }

        private string? m_CachePathAddition;

        public Bitmap FBitmap
        {
            get
            {
                return this.helplineRulerCtrl2.Bmp;
            }
        }

        private Bitmap? _bmpBU = null;
        private bool _pic_changed = false;

        private int _openPanelHeight;
        private bool _isHoveringVertically;
        private bool _doubleClicked;
        private bool _dontDoZoom;
        private bool _tracking;
        private int _ix;
        private int _iy;
        public List<PointF>? CurPath { get; set; }
        public List<PointF>? CurPathTmp { get; set; }
        public List<PathInfo>? Paths { get; set; }
        public List<PointF>? CurList { get; private set; }
        public string? AvoidAGrabCutCache { get; set; }

        private bool _dontDrawPath;

        private static int[] CustomColors = new int[] { };

        private IBlendAlgorithm? _cAlg;
        private PoissonBlender? _pb;

        private TextureBrush? _tb = null;
        private Point _sourcePt;
        private Point _destPt;
        //private bool _drawAll;
        private int _fp;
        private Bitmap? _bmpBUZoomed;
        private int _clicks;

        private Bitmap? _bmpOrg;
        private Bitmap? _bmpDraw;
        private int _ix2;
        private int _iy2;
        private bool _overlaySrc;
        private int _eX;
        private int _eY;

        private object _lockObject = new object();
        private bool _penStrokesDone;

        private Bitmap? _bmpOrigHLC1;
        private Bitmap? _bmpPenStrokes;
        private bool _customPicLoaded;

        public frmPoissonDraw(Bitmap bmp, string basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

            //HelperFunctions.HelperFunctions.SetFormSizeBig(this);
            this.CenterToScreen();

            // Me.helplineRulerCtrl1.SetZoomOnlyByMethodCall = True

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.helplineRulerCtrl1.Bmp = new Bitmap(bmp);
                this.helplineRulerCtrl2.Bmp = new Bitmap(bmp);
                _bmpBU = new Bitmap(bmp);
                _bmpOrg = new Bitmap(bmp);
                this._bmpDraw = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                this._tb = new TextureBrush(this._bmpBU);
                this._tb.WrapMode = WrapMode.TileFlipXY;
                this._bmpOrigHLC1 = new Bitmap(bmp);
            }
            else
            {
                MessageBox.Show("Not enough Memory");
                return;
            }

            double faktor = System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Height);
            double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Height);
            if (multiplier >= faktor)
                this.helplineRulerCtrl2.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Width));
            else
                this.helplineRulerCtrl2.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl2.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl2.Bmp.Height));

            this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
            this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

            this.helplineRulerCtrl2.AddDefaultHelplines();
            this.helplineRulerCtrl2.ResetAllHelpLineLabelsColor();

            faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
            multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
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

            this.helplineRulerCtrl2.dbPanel1.MouseDown += helplineRulerCtrl2_MouseDown;
            this.helplineRulerCtrl2.dbPanel1.MouseMove += helplineRulerCtrl2_MouseMove;
            this.helplineRulerCtrl2.dbPanel1.MouseUp += helplineRulerCtrl2_MouseUp;

            this.helplineRulerCtrl2.PostPaint += helplineRulerCtrl2_Paint;

            this._dontDoZoom = true;
            this.cmbZoom.SelectedIndex = 4;
            this._dontDoZoom = false;

            if (this._bmpBUZoomed == null)
                MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix2 = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy2 = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (this.cbSetPts.Checked && e.Button == MouseButtons.Right)
            {
                AddCurPath();
                if (this.rbSrc.Checked)
                    this._sourcePt = new Point(_ix2, _iy2);

                int dx = this._destPt.X - this._sourcePt.X;
                int dy = this._destPt.Y - this._sourcePt.Y;

                SetupTB();

                this._tb?.ResetTransform();
                this._tb?.TranslateTransform(dx, dy);

                if (this.cbAuto.Checked)
                {
                    if (this.rbSrc.Checked)
                        this.rbDest.Checked = true;
                    else
                        this.rbSrc.Checked = true;

                    this._clicks += 1;
                    if (this._clicks % 2 == 0)
                        this.cbSetPts.Checked = false;
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                if (ix < 0)
                    ix = 0;

                if (iy < 0)
                    iy = 0;

                if (ix > this.helplineRulerCtrl2.Bmp.Width - 1)
                    ix = this.helplineRulerCtrl2.Bmp.Width - 1;

                if (iy > this.helplineRulerCtrl2.Bmp.Height - 1)
                    iy = this.helplineRulerCtrl2.Bmp.Height - 1;

                Bitmap b = (Bitmap)this.helplineRulerCtrl1.Bmp;
                Color c = b.GetPixel(ix, iy);

                toolStripStatusLabel1.Text = "x: " + ix.ToString() + ", y: " + iy.ToString();
                this.ToolStripStatusLabel2.BackColor = c;
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {

        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {
            if (this._sourcePt.X >= 0 && this._sourcePt.Y >= 0 && this._sourcePt.X < this.helplineRulerCtrl1.Bmp.Width && this._sourcePt.Y < this.helplineRulerCtrl1.Bmp.Height)
            {
                e.Graphics.DrawLine(Pens.Yellow, this._sourcePt.X * this.helplineRulerCtrl1.Zoom - 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    this._sourcePt.Y * this.helplineRulerCtrl1.Zoom - 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                    this._sourcePt.X * this.helplineRulerCtrl1.Zoom + 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    this._sourcePt.Y * this.helplineRulerCtrl1.Zoom + 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                e.Graphics.DrawLine(Pens.Yellow, this._sourcePt.X * this.helplineRulerCtrl1.Zoom + 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    this._sourcePt.Y * this.helplineRulerCtrl1.Zoom - 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                    this._sourcePt.X * this.helplineRulerCtrl1.Zoom - 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    this._sourcePt.Y * this.helplineRulerCtrl1.Zoom + 5 + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);

                if (this.cbFindDestPoint.Checked)
                    e.Graphics.DrawRectangle(Pens.Red, (this._sourcePt.X - (int)this.numSrcPtSurround.Value) * this.helplineRulerCtrl1.Zoom + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                        (this._sourcePt.Y - (int)this.numSrcPtSurround.Value) * this.helplineRulerCtrl1.Zoom + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                        ((int)this.numSrcPtSurround.Value * 2 + 1) * this.helplineRulerCtrl1.Zoom,
                        ((int)this.numSrcPtSurround.Value * 2 + 1) * this.helplineRulerCtrl1.Zoom);
            }
        }

        private void helplineRulerCtrl2_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

            if (this.cbSetPts.Checked && e.Button == MouseButtons.Right)
            {
                AddCurPath();
                if (this.rbDest.Checked)
                    this._destPt = new Point(_ix, _iy);

                int dx = this._destPt.X - this._sourcePt.X;
                int dy = this._destPt.Y - this._sourcePt.Y;

                SetupTB();

                this._tb?.ResetTransform();
                this._tb?.TranslateTransform(dx, dy);

                if (this.cbAuto.Checked)
                {
                    if (this.rbSrc.Checked)
                        this.rbDest.Checked = true;
                    else
                        this.rbSrc.Checked = true;

                    this._clicks += 1;
                    if (this._clicks % 2 == 0)
                        this.cbSetPts.Checked = false;
                }

                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }

            if (this.cbDraw.Checked && e.Button == MouseButtons.Left)
            {
                this._dontDrawPath = false;
                SetupCurPath();
                this._tracking = true;
                if (!this.cbClickMode.Checked)
                    this.CurPath?.Add(new PointF(this._ix, this._iy));
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl2_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
                int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

                this._eX = e.X;
                this._eY = e.Y;

                if (ix != _ix || iy != _iy)
                {
                    _ix = ix;
                    _iy = iy;

                    if (_ix < 0)
                        _ix = 0;

                    if (_iy < 0)
                        _iy = 0;

                    if (_ix > this.helplineRulerCtrl2.Bmp.Width - 1)
                        _ix = this.helplineRulerCtrl2.Bmp.Width - 1;

                    if (_iy > this.helplineRulerCtrl2.Bmp.Height - 1)
                        _iy = this.helplineRulerCtrl2.Bmp.Height - 1;

                    Bitmap b = (Bitmap)this.helplineRulerCtrl2.Bmp;
                    Color c = b.GetPixel(_ix, _iy);

                    toolStripStatusLabel4.Text = "x: " + _ix.ToString() + ", y: " + _iy.ToString();
                    this.toolStripStatusLabel3.BackColor = c;

                    if (this.cbDraw.Checked)
                    {
                        if (this._tracking && e.Button == MouseButtons.Left)
                        {
                            SetupCurPath();
                            if (!this.cbClickMode.Checked)
                                this.CurPath?.Add(new PointF(this._ix, this._iy));
                        }

                        this.helplineRulerCtrl2.dbPanel1.Invalidate();
                    }
                }
            }
        }

        private void helplineRulerCtrl2_MouseUp(object? sender, MouseEventArgs e)
        {
            int ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
            int iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

            if (ix == _ix && iy == _iy)
            {
                if (this.CurPath != null && this.cbDraw.Checked && !this.cbClickMode.Checked && !ContainsPoint(this.CurPath, new PointF(this._ix, this._iy)))
                {
                    SetupCurPath();
                    this.CurPath?.Add(new PointF(this._ix, this._iy));
                }

                if (this.cbDraw.Checked && this.cbClickMode.Checked && e.Button == MouseButtons.Left)
                {
                    SetupCurList();
                    this.CurList?.Add(new PointF(this._ix, this._iy));
                }

                if (this.cbDraw.Checked && this.cbClickMode.Checked && e.Button == MouseButtons.Right)
                {
                    SetupCurList();
                    this.CurList?.Add(new PointF(this._ix, this._iy));

                    if (this.CurList?.Count > 0)
                    {
                        this.CurPath?.AddRange(this.CurList.ToArray());
                        this.CurList.Clear();

                        DrawPath();
                        this.helplineRulerCtrl2.dbPanel1.Invalidate();

                        this._penStrokesDone = true;
                    }
                }
            }

            if (this._tracking && this.CurPath != null && this.CurPath.Count > 0)
            {
                DrawPath();
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }

            this._tracking = false;
        }

        private void helplineRulerCtrl2_Paint(object sender, PaintEventArgs e)
        {
            SetupTB();
            float w = (float)this.numPenSize.Value;

            if (this._bmpBUZoomed != null)
                using (TextureBrush tb = new TextureBrush(this._bmpBUZoomed))
                {
                    tb.WrapMode = WrapMode.TileFlipXY;
                    int dx = this._destPt.X - this._sourcePt.X;
                    int dy = this._destPt.Y - this._sourcePt.Y;
                    tb.TranslateTransform(dx * helplineRulerCtrl2.Zoom, dy * helplineRulerCtrl2.Zoom);

                    if (!this._dontDrawPath && this.cbDraw.Checked && this._tracking)
                    {
                        using (GraphicsPath gp = GetPath())
                        {
                            int x = this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X;
                            int y = this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y;
                            tb.TranslateTransform(x, y);
                            using (Matrix m = new Matrix(this.helplineRulerCtrl2.Zoom, 0, 0, this.helplineRulerCtrl2.Zoom, x, y)) //for drawing hlc2
                            {
                                gp.Transform(m);
                                using (Pen pen = new Pen(tb, Math.Max(w * this.helplineRulerCtrl2.Zoom, 1.0f)))
                                {
                                    pen.LineJoin = LineJoin.Round;
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;

                                    if (this.CurPath?.Count == 1)
                                        e.Graphics.FillPath(tb, gp);
                                    else
                                        e.Graphics.DrawPath(pen, gp);
                                }

                                //test
                                using (Pen pen = new Pen(new SolidBrush(Color.FromArgb(this.cbOverlay.Checked ? 127 : 64, Color.Lime)),
                                    Math.Max(w * this.helplineRulerCtrl2.Zoom, 1.0f)))
                                {
                                    pen.LineJoin = LineJoin.Round;
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;

                                    if (this.CurPath?.Count == 1)
                                        e.Graphics.FillPath(tb, gp);
                                    else
                                        e.Graphics.DrawPath(pen, gp);
                                }
                            }
                        }
                    }
                }

            if (this.cbDraw.Checked)
            {
                float ww = Math.Max(w * this.helplineRulerCtrl2.Zoom, 1.0f);
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(this.cbOverlay.Checked ? 127 : 64, Color.Lime)))
                    e.Graphics.FillEllipse(sb, new RectangleF(_eX - ww / 2f, _eY - ww / 2f, ww, ww));
            }

            if (this.cbDraw.Checked && this.cbClickMode.Checked && this.CurList != null && this.CurList.Count > 0)
            {
                using GraphicsPath gPath = new GraphicsPath();
                gPath.AddLines(this.CurList.ToArray());

                using (Matrix m = new Matrix(this.helplineRulerCtrl2.Zoom, 0, 0, this.helplineRulerCtrl2.Zoom, this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y))
                {
                    gPath.Transform(m);

                    using (Pen pen = new Pen(new SolidBrush(Color.FromArgb(this.cbOverlay.Checked ? 127 : 64, Color.Red)),
                        Math.Max(w * this.helplineRulerCtrl2.Zoom, 1.0f)))
                    {
                        pen.LineJoin = LineJoin.Round;
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;

                        e.Graphics.DrawPath(pen, gPath);
                        if (gPath.PointCount > 0)
                            e.Graphics.DrawLine(pen, gPath.PathPoints[gPath.PathPoints.Length - 1], new PointF(_eX, _eY));
                    }
                }
            }

            if (this._overlaySrc)
            {
                //HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl1.dbPanel1;
                HelplineRulerControl.DBPanel pz2 = this.helplineRulerCtrl2.dbPanel1;

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = (float)this.numOpacity.Value;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);

                    int x = (int)((this._destPt.X - this._sourcePt.X) * this.helplineRulerCtrl2.Zoom + pz2.AutoScrollPosition.X);
                    int y = (int)((this._destPt.Y - this._sourcePt.Y) * this.helplineRulerCtrl2.Zoom + pz2.AutoScrollPosition.Y);

                    e.Graphics.DrawImage(this.helplineRulerCtrl1.Bmp,
                        new Rectangle(x, y,
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl2.Zoom)),
                            0, 0,
                            this.helplineRulerCtrl1.Bmp.Width,
                            this.helplineRulerCtrl1.Bmp.Height, GraphicsUnit.Pixel, ia);
                }
            }

            if (this.cbShowCustPic.Checked && this._bmpPenStrokes != null)
            {
                HelplineRulerControl.DBPanel pz2 = this.helplineRulerCtrl2.dbPanel1;

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.75f;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);

                    e.Graphics.DrawImage(this._bmpPenStrokes,
                        new Rectangle(0, 0,
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl2.Zoom)),
                            0, 0,
                            this.helplineRulerCtrl1.Bmp.Width,
                            this.helplineRulerCtrl1.Bmp.Height, GraphicsUnit.Pixel, ia);
                }
            }

            if (this._destPt.X >= 0 && this._destPt.Y >= 0 && this._destPt.X < this.helplineRulerCtrl2.Bmp.Width && this._destPt.Y < this.helplineRulerCtrl2.Bmp.Height)
            {
                e.Graphics.DrawLine(Pens.Red, this._destPt.X * this.helplineRulerCtrl2.Zoom - 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X,
                    this._destPt.Y * this.helplineRulerCtrl2.Zoom - 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y,
                    this._destPt.X * this.helplineRulerCtrl2.Zoom + 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X,
                    this._destPt.Y * this.helplineRulerCtrl2.Zoom + 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y);
                e.Graphics.DrawLine(Pens.Red, this._destPt.X * this.helplineRulerCtrl2.Zoom + 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X,
                    this._destPt.Y * this.helplineRulerCtrl2.Zoom - 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y,
                    this._destPt.X * this.helplineRulerCtrl2.Zoom - 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X,
                    this._destPt.Y * this.helplineRulerCtrl2.Zoom + 5 + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y);
            }
        }

        private void AddCurPath()
        {
            if (this.CurPath != null && this.CurPath.Count > 0)
            {
                if (this.Paths == null)
                    this.Paths = new List<PathInfo>();

                this.CurPath = new List<PointF>();
            }
        }

        private bool ContainsPoint(List<PointF> curPath, PointF pointF)
        {
            if (curPath != null)
            {
                for (int i = 0; i <= curPath.Count - 1; i++)
                {
                    if (curPath[i].X == pointF.X && curPath[i].Y == pointF.Y)
                        return true;
                }
            }
            return false;
        }

        private void SetupCurPath()
        {
            if (this.CurPath == null)
                this.CurPath = new List<PointF>();
        }

        private void SetupCurList()
        {
            if (this.CurList == null)
                this.CurList = new List<PointF>();
        }

        private void DrawPath()
        {
            if (this.IsDisposed == false && this.Visible)
            {
                if (this._bmpDraw != null && AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Bmp.Height * 10L))
                {
                    Bitmap bmpDrawFrom = new Bitmap(this._bmpDraw); //pen strokes drawn with hlc1_bmp

                    SetupTB();

                    if (this.Paths == null)
                        this.Paths = new List<PathInfo>();

                    using (Graphics gx = Graphics.FromImage(bmpDrawFrom))
                    {
                        // you could always start from bmpOrg and process each step to be able to change
                        // the parameters for each path individually, but this may take a long time to run...
                        // see commented-out sections below
                        //if (this._drawAll)
                        //{
                        //    if (this.Paths != null && this.Paths.Count > 0)
                        //    {
                        //        for (int i = 0; i <= this.Paths.Count - 1; i++)
                        //        {
                        //            float w = this.Paths[i].DrawWidth;
                        //            this._tb?.ResetTransform();
                        //            this._tb?.TranslateTransform(this.Paths[i].Offset.X, this.Paths[i].Offset.Y);

                        //            // Me.ComboBox1.SelectedIndex = Me.Paths(i).BVAlg
                        //            // Me.NumericUpDown2.Value = CDec(Me.Paths(i).UpperWeight)
                        //            // Me.NumericUpDown3.Value = CDec(Me.Paths(i).LowerWeight)

                        //            if (this._tb != null)
                        //                using (Pen pen = new Pen(this._tb, w))
                        //                {
                        //                    pen.LineJoin = LineJoin.Round;

                        //                    if (this.Paths[i].RoundCaps)
                        //                    {
                        //                        pen.StartCap = LineCap.Round;
                        //                        pen.EndCap = LineCap.Round;
                        //                    }

                        //                    using (GraphicsPath gp = new GraphicsPath())
                        //                    {
                        //                        if (this.Paths[i]?.Count() == 1)
                        //                        {
                        //                            gp.AddEllipse(this.Paths[i].Points[0].X - w, this.Paths[i].Points[0].Y - w, w * 2, w * 2);
                        //                            gx.FillPath(this._tb, gp);
                        //                        }
                        //                        else
                        //                        {
                        //                            gp.AddLines(this.Paths[i].ToArray());
                        //                            gx.DrawPath(pen, gp);
                        //                        }
                        //                    }
                        //                }
                        //        }
                        //    }
                        //}

                        using (GraphicsPath gp = GetPath())
                        {
                            float w = (float)this.numPenSize.Value;
                            int dx = this._destPt.X - this._sourcePt.X;
                            int dy = this._destPt.Y - this._sourcePt.Y;
                            this._tb?.ResetTransform();
                            this._tb?.TranslateTransform(dx, dy);

                            if (this._tb != null)
                                using (Pen pen = new Pen(this._tb, w))
                                {
                                    pen.LineJoin = LineJoin.Round;
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;

                                    if (this.CurPath?.Count == 1)
                                        gx.FillPath(this._tb, gp);
                                    else
                                        gx.DrawPath(pen, gp);
                                }

                            this.Paths?.RemoveRange(this._fp, this.Paths.Count - this._fp);
                            this.Paths?.Add(new PathInfo() { Points = this.CurPath, Offset = new Point(dx, dy), RoundCaps = true, DrawWidth = (float)this.numPenSize.Value, SourcePt = this._sourcePt, DestPt = this._destPt, BVAlg = this.cmbAlg.SelectedIndex, UpperWeight = System.Convert.ToDouble(this.numUpperWeight.Value), LowerWeight = System.Convert.ToDouble(this.numLowerWeight.Value) });
                            this._fp += 1;

                            if (this._bmpBU != null && AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 10L))
                            {
                                Bitmap bmpDrawTo = new Bitmap(this.helplineRulerCtrl2.Bmp); //hlc2 current image
                                ProcessImage(bmpDrawTo, bmpDrawFrom);
                            }
                        }
                    }
                }
            }
        }


        private void ProcessImage(Bitmap bmpDrawTo, Bitmap bmpDrawFrom)
        {
            this.SetControls(false);
            this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Minimum;
            this.toolStripProgressBar1.Visible = true;

            object[] o = new object[] { bmpDrawTo, bmpDrawFrom, this.cmbAlg.SelectedIndex,
                (double)this.numUpperWeight.Value, (double)this.numLowerWeight.Value,
                (int)this.numMaxPixelDist.Value, (double)this.numGamma.Value,
                this.cbOrdinaryDraw.Checked, (float)this.numOpacityOrdDraw.Value};

            if (!this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.RunWorkerAsync(o);
        }

        private void backgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                bool ordinaryDraw = (bool)o[7];
                float opacity = (float)o[8];

                Bitmap bmpDrawTo = (Bitmap)o[0]; // bmpbu
                using (Bitmap bmpDrawFrom = (Bitmap)o[1]) //_bmpDraw
                {
                    if (AvailMem.AvailMem.checkAvailRam(bmpDrawTo.Width * bmpDrawTo.Height * 5L))
                    {
                        if (ordinaryDraw)
                        {
                            using (Graphics gx = Graphics.FromImage(bmpDrawTo))
                            {
                                ColorMatrix cm = new ColorMatrix();
                                cm.Matrix33 = opacity;

                                using (ImageAttributes ia = new ImageAttributes())
                                {
                                    ia.SetColorMatrix(cm);

                                    gx.DrawImage(bmpDrawFrom,
                                        new Rectangle(0, 0,
                                            bmpDrawTo.Width,
                                            bmpDrawTo.Height),
                                            0, 0,
                                            bmpDrawFrom.Width,
                                            bmpDrawFrom.Height, GraphicsUnit.Pixel, ia);
                                }
                            }

                            e.Result = bmpDrawTo;
                        }
                        else
                        {
                            Rectangle rc = new Rectangle();
                            using (Bitmap? bmpU = ScanForPic(bmpDrawFrom, 0, ref rc))
                            {
                                if (bmpU != null)
                                    using (Bitmap bmpLw = bmpDrawTo.Clone(rc, bmpDrawTo.PixelFormat))
                                    {
                                        SetSolid(bmpLw, bmpU);

                                        int mI = 5000;
                                        bool preRel = true;
                                        int maxRstrs = 500;
                                        double desError = 0.0001;
                                        int minAlpha = 254;
                                        int innerItrts = 5;
                                        bool autoSOR = true;
                                        AutoSORMode autoSORMode = AutoSORMode.WidthRelated;
                                        //bool postProc = false;
                                        //double postProcMultiplier = 1.0;
                                        int bVAlg = System.Convert.ToInt32(o[2]);
                                        double upperWeight = System.Convert.ToDouble(o[3]);
                                        double lowerWeight = System.Convert.ToDouble(o[4]);
                                        int maxPixelDist = (int)o[5];
                                        double gamma = (double)o[6];

                                        PoissonBlender pb = new PoissonBlender(new Bitmap(bmpLw), new Bitmap(bmpU));
                                        PoissonBlend.ProgressEventArgs pe = new PoissonBlend.ProgressEventArgs(mI * 3, 0);
                                        pe.PrgInterval = mI / 20;
                                        //pb.BlendParameters.Gamma = ;
                                        //pb.BlendParameters.MaxPixelDist = ;

                                        IBVectorComputingAlgorithm bAlg = pb.BlendParameters.GetBVectorAlg(bVAlg); // addb etc

                                        if (bAlg.GetType().GetInterfaces().Contains(typeof(IExtendedBVectorComputingAlgorithm)))
                                        {
                                            IExtendedBVectorComputingAlgorithm iExtAlg = (IExtendedBVectorComputingAlgorithm)bAlg;
                                            using Bitmap bC = new Bitmap(bmpU);
                                            iExtAlg.Setup(bC, maxPixelDist, gamma);
                                        }

                                        IBlendAlgorithm cAlg = pb.BlendParameters.GetCalcAlg(1); // GMRES_r
                                        cAlg.ShowProgess += ShowProgress;
                                        this._cAlg = cAlg;
                                        this._pb = pb;

                                        Rectangle rcSmall = new Rectangle(0, 0, bmpU.Width, bmpU.Height);

                                        pb.SetParameters(minAlpha, mI, desError, innerItrts, preRel, new Rectangle(0, 0, rcSmall.Width, rcSmall.Height), maxRstrs, autoSOR, autoSORMode, 12, upperWeight, lowerWeight, cAlg, bAlg, true, pe);

                                        Bitmap? bmpRes = null;
                                        try
                                        {
                                            if (pb != null)
                                                bmpRes = pb.Apply();
                                        }
                                        catch (OutOfMemoryException exc)
                                        {
                                            MessageBox.Show(exc.Message);
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(exc.Message);
                                        }

                                        cAlg.ShowProgess -= ShowProgress;

                                        using (Graphics gx = Graphics.FromImage(bmpDrawTo))
                                            if (bmpRes != null)
                                                gx.DrawImage(bmpRes, rc);

                                        e.Result = bmpDrawTo;
                                    }
                            }
                        }
                    }
                }
            }
        }

        private void ShowProgress(object? sender, PoissonBlend.ProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    this.backgroundWorker1.ReportProgress(e.CurrentProgress / 50);
                }));
            }
            else
                this.backgroundWorker1.ReportProgress(e.CurrentProgress / 50);
        }

        private void SetSolid(Bitmap b, Bitmap b2)
        {
            BitmapData? bmData = null;
            BitmapData? bmSrc = null;
            if (AvailMem.AvailMem.checkAvailRam(b.Width * b.Height * 4L))
            {
                // GDI+ still lies to us - the return format is BGR, NOT RGB.

                bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                bmSrc = b2.LockBits(new Rectangle(0, 0, b2.Width, b2.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int scanline = bmData.Stride;

                int nWidth = b.Width;
                int nHeight = b.Height;

                byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                byte[] pSrc = new byte[(bmSrc.Stride * bmSrc.Height) - 1 + 1];
                Marshal.Copy(bmSrc.Scan0, pSrc, 0, pSrc.Length);

                b2.UnlockBits(bmSrc);

                Parallel.For(0, nHeight, y =>
                {
                    for (int x = 0; x <= nWidth - 1; x++)
                    {
                        if (p[x * 4 + y * scanline + 3] == 0)
                        {
                            p[x * 4 + y * scanline] = pSrc[x * 4 + y * scanline];
                            p[x * 4 + y * scanline + 1] = pSrc[x * 4 + y * scanline + 1];
                            p[x * 4 + y * scanline + 2] = pSrc[x * 4 + y * scanline + 2];
                        }
                        p[x * 4 + y * scanline + 3] = System.Convert.ToByte(255);
                    }
                });

                Marshal.Copy(p, 0, bmData.Scan0, p.Length);
                b.UnlockBits(bmData);
            }
            else
                throw new Exception("Not enough memory.");
        }

        public static Bitmap? ScanForPic(Bitmap bmp, int add, ref Rectangle rectangle)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
            {
                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData? bmData = null;

                try
                {
                    bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    int scanline = bmData.Stride;

                    System.IntPtr Scan0 = bmData.Scan0;
                    Point top = new Point();
                    Point left = new Point();
                    Point right = new Point();
                    Point bottom = new Point();
                    bool complete = false;

                    byte[] p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                    int pos = 0;

                    for (int y = 0; y <= bmp.Height - 1; y++)
                    {
                        for (int x = 0; x <= bmp.Width - 1; x++)
                        {
                            if (p[pos + 3] != 0)
                            {
                                top = new Point(x, y);
                                complete = true;
                                break;
                            }

                            pos += 4;
                        }
                        if (complete)
                            break;
                    }

                    pos = 0;
                    complete = false;

                    for (int y = bmp.Height - 1; y >= 0; y += -1)
                    {
                        for (int x = 0; x <= bmp.Width - 1; x++)
                        {
                            if (p[x * 4 + y * scanline + 3] != 0)
                            {
                                bottom = new Point(x + 1, y + 1);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }

                    pos = 0;
                    complete = false;

                    for (int x = 0; x <= bmp.Width - 1; x++)
                    {
                        for (int y = 0; y <= bmp.Height - 1; y++)
                        {
                            if (p[x * 4 + y * scanline + 3] != 0)
                            {
                                left = new Point(x, y);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }

                    pos = 0;
                    complete = false;

                    for (int x = bmp.Width - 1; x >= 0; x += -1)
                    {
                        for (int y = 0; y <= bmp.Height - 1; y++)
                        {
                            if (p[x * 4 + y * scanline + 3] != 0)
                            {
                                right = new Point(x + 1, y + 1);
                                complete = true;
                                break;
                            }
                        }
                        if (complete)
                            break;
                    }

                    bmp.UnlockBits(bmData);

                    rectangle = new Rectangle(left.X, top.Y, right.X - left.X, bottom.Y - top.Y);

                    if (rectangle.Width == 0)
                        rectangle.Width = bmp.Width;
                    if (rectangle.Height == 0)
                        rectangle.Height = bmp.Height;

                    Bitmap? b = null;
                    Graphics? g = null;

                    try
                    {
                        b = new Bitmap(rectangle.Width + add * 2, rectangle.Height + add * 2);
                        g = Graphics.FromImage(b);
                        g.DrawImage(bmp, add, add, rectangle, GraphicsUnit.Pixel);
                        g.Dispose();

                        return b;
                    }
                    catch
                    {
                        if (b != null)
                            b.Dispose();
                        if (g != null)
                            g.Dispose();

                        throw new Exception("Error at ScanForPic.");
                    }
                }
                catch
                {
                    try
                    {
                        if (bmData != null)
                            bmp.UnlockBits(bmData);
                    }
                    catch
                    {
                    }
                    throw new Exception("Error at ScanForPic.");
                }
            }

            return null;
        }

        private GraphicsPath GetPath()
        {
            GraphicsPath gp = new GraphicsPath();
            if (this.CurPath != null && this.CurPath.Count > 0)
            {
                gp.StartFigure();
                if (this.CurPath.Count > 1)
                    gp.AddLines(this.CurPath.ToArray());
                else
                {
                    float w = (float)Math.Max((double)this.numPenSize.Value / 2.0, 1.0);
                    gp.AddEllipse(this.CurPath[0].X - w, this.CurPath[0].Y - w, w * 2, w * 2);
                }
            }

            return gp;
        }

        private GraphicsPath GetPathTmp()
        {
            GraphicsPath gp = new GraphicsPath();
            if (this.CurPathTmp != null && this.CurPathTmp.Count > 0)
            {
                gp.StartFigure();
                if (this.CurPathTmp.Count > 1)
                    gp.AddLines(this.CurPathTmp.ToArray());
                else
                {
                    float w = (float)Math.Max((double)this.numPenSize.Value / 2.0, 1.0);
                    gp.AddEllipse(this.CurPathTmp[0].X - w, this.CurPathTmp[0].Y - w, w * 2, w * 2);
                }
            }
            return gp;
        }

        private void SetupTB()
        {
            if (this._tb == null && this._bmpBU != null)
            {
                this._tb = new TextureBrush(this._bmpBU);
                this._tb.WrapMode = WrapMode.TileFlipXY;
            }
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmPoissDraw");
            if (this.helplineRulerCtrl2.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl2.Bmp);
        }

        private void bitmappanel1_DragOver(object? sender, DragEventArgs e)
        {
            if ((e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)))
                e.Effect = DragDropEffects.Copy;
        }

        private void bitmappanel1_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
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

                        String[]? files = (String[]?)e.Data.GetData(DataFormats.FileDrop);
                        if (files != null)
                        {
                            using (Image img = Image.FromFile(files[0]))
                            {
                                if (AvailMem.AvailMem.checkAvailRam(img.Width * img.Height * 16L))
                                {
                                    b1 = new Bitmap(img);
                                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                    b2 = new Bitmap(img);
                                    this.SetBitmap(ref this._bmpBU, ref b2);
                                    Bitmap? bC2 = new Bitmap(b1);
                                    this.SetBitmap(ref this._bmpOrigHLC1, ref bC2);
                                }
                                else
                                    throw new Exception();
                            }
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
                        this.btnRedo.Enabled = false;

                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        if (files != null && files[0] != null)
                            this.Text = files[0] + " - frmQuickExtract";
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



            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
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

                        String[]? files = (String[]?)e.Data.GetData(DataFormats.FileDrop);
                        if (files != null)
                        {
                            using (Image img = Image.FromFile(files[0]))
                            {
                                if (AvailMem.AvailMem.checkAvailRam(img.Width * img.Height * 16L))
                                {
                                    b1 = new Bitmap(img);
                                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");

                                    b2 = new Bitmap(img);
                                    this.SetBitmap(ref this._bmpBU, ref b2);
                                }
                                else
                                    throw new Exception();
                            }

                            if (_bmpBU != null)
                            {
                                _bmpOrg = new Bitmap(_bmpBU);
                                this._bmpDraw = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                                TextureBrush? tOld = this._tb;
                                this._tb = new TextureBrush(this._bmpBU);
                                this._tb.WrapMode = WrapMode.TileFlipXY;
                                if (tOld != null)
                                    tOld.Dispose();
                                MakeBitmap(this._bmpBU, this.helplineRulerCtrl1.Zoom);
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
                            this.btnUndo.Enabled = this.btnRedo.Enabled = false;

                            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                            this.helplineRulerCtrl1.dbPanel1.Invalidate();

                            this.Text = files[0] + " - frmQuickExtract";
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

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("");
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoUndo();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");
                    this._pic_changed = false;
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

                    if (_undoOPCache.CurrentPosition > 1)
                    {
                        this.btnUndo.Enabled = true;
                    }
                    else
                        this.btnUndo.Enabled = false;

                    this.btnRedo.Enabled = true;
                }
                else
                    MessageBox.Show("Error while resetting.");
            }
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                this.btnRedo.Enabled = false;
                this.btnRedo.Refresh();

                Bitmap bOut = _undoOPCache.DoRedo();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");
                    this._pic_changed = true;
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    if (_undoOPCache.CurrentPosition > 1)
                    {
                        this.btnUndo.Enabled = true;
                    }
                    else
                        this.btnUndo.Enabled = false;

                    if (_undoOPCache.CurrentPosition < _undoOPCache.Count)
                        this.btnRedo.Enabled = true;
                    else
                        this.btnRedo.Enabled = false;
                }
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.helplineRulerCtrl2.Bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.helplineRulerCtrl2.Bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    _pic_changed = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                    Bitmap? bmpHLC1 = null;

                    try
                    {
                        if (this._bmpOrigHLC1 != null && AvailMem.AvailMem.checkAvailRam(this._bmpOrigHLC1.Width * this._bmpOrigHLC1.Height * 12L))
                            bmpHLC1 = new Bitmap(this._bmpOrigHLC1);
                        else
                            throw new Exception();

                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmpHLC1, this.helplineRulerCtrl1, "Bmp");

                        this.helplineRulerCtrl1.CalculateZoom();
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                    catch
                    {
                        if (bmpHLC1 != null)
                            bmpHLC1.Dispose();
                    }
                }
            }
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

            if (keyData == (Keys.Y | Keys.Control))
            {
                this.btnRedo.PerformClick();
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

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.helplineRulerCtrl1.dbPanel1.SuspendLayout();
                this.SuspendLayout();

                if (!this._doubleClicked)
                {
                    this._openPanelHeight = this.panel1.Height;
                    this._doubleClicked = true;
                    this.panel1.Dock = DockStyle.None;
                    this.panel3.Dock = DockStyle.Top;

                    this._isHoveringVertically = true;

                    this.panel3.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();
                    this.statusStrip1.SendToBack();

                    this.panel1.BringToFront();

                    this.panel1.AutoScroll = false;
                }
                else
                {
                    this._doubleClicked = false;
                    this.panel3.Dock = DockStyle.None;
                    this.panel1.Dock = DockStyle.Top;

                    this.panel3.Width = this.helplineRulerCtrl1.Width = this.panel1.Width = this.ClientSize.Width;

                    this._isHoveringVertically = false;
                    this.panel1.Height = this._openPanelHeight;

                    this.statusStrip1.SendToBack();
                    this.panel1.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();

                    this.panel1.AutoScroll = true;
                    this.panel1.BringToFront();
                    this.helplineRulerCtrl1.BringToFront();

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                }

                this.ResumeLayout();
                this.helplineRulerCtrl1.dbPanel1.ResumeLayout(true);
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this._doubleClicked && e.Button == MouseButtons.Right)
            {
                this.SuspendLayout();

                if (this._isHoveringVertically)
                {
                    this.panel1.Dock = DockStyle.Top;
                    this.panel3.Dock = DockStyle.None;
                    this._isHoveringVertically = false;
                    this.panel3.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();
                    this.statusStrip1.SendToBack();
                    this.panel1.Height = 24;
                }
                else
                {
                    this.panel3.Dock = DockStyle.Top;
                    this.panel1.Dock = DockStyle.None;
                    this._isHoveringVertically = true;
                    this.panel3.SendToBack();
                    this.helplineRulerCtrl1.BringToFront();
                    this.statusStrip1.SendToBack();

                    this.panel1.BringToFront();
                    this.panel1.Height = this._openPanelHeight;
                }

                this.ResumeLayout(true);
            }
        }

        private void helplineRulerCtrl1_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            this._dontDoZoom = true;

            if (e.Zoom == 1.0F)
                this.cmbZoom.SelectedIndex = 2;
            else if (e.ZoomWidth)
                this.cmbZoom.SelectedIndex = 3;
            else
                this.cmbZoom.SelectedIndex = 4;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            this._dontDoZoom = false;
        }

        private void helplineRulerCtrl2_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            this._dontDoZoom = true;

            if (e.Zoom == 1.0F)
                this.cmbZoom.SelectedIndex = 2;
            else if (e.ZoomWidth)
                this.cmbZoom.SelectedIndex = 3;
            else
                this.cmbZoom.SelectedIndex = 4;

            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            this._dontDoZoom = false;
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.helplineRulerCtrl1.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(cmbZoom?.SelectedItem?.ToString());
                this.helplineRulerCtrl1.Enabled = true;
                if (this.cmbZoom?.SelectedIndex < 2)
                    this.helplineRulerCtrl1.ZoomSetManually = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }

            if (this.Visible && this.helplineRulerCtrl2.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl2.Enabled = false;
                this.helplineRulerCtrl2.Refresh();
                this.helplineRulerCtrl2.SetZoom(cmbZoom?.SelectedItem?.ToString());
                this.helplineRulerCtrl2.Enabled = true;
                if (this.cmbZoom?.SelectedIndex < 2)
                    this.helplineRulerCtrl2.ZoomSetManually = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }

            if (this._bmpBUZoomed != null)
                this._bmpBUZoomed.Dispose();
            this._bmpBUZoomed = null;
            if (this._bmpBUZoomed == null && this._bmpBU != null)
                MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);
        }

        private void CheckBox12_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (this.cbBGColor.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = this.helplineRulerCtrl2.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = this.helplineRulerCtrl2.dbPanel1.BackColor = SystemColors.Control;
        }

        private void Button28_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.panel1.Controls)
                    {
                        if (ct.Name != "btnCancel" && !(ct is PictureBox))
                            ct.Enabled = e;
                    }

                    this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl2.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.panel1.Controls)
                {
                    if (ct.Name != "btnCancel" && !(ct is PictureBox))
                        ct.Enabled = e;
                }

                this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl2.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
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

        private void Timer3_Tick(object sender, EventArgs e)
        {
            this.Timer3.Stop();

            if (this.toolStripProgressBar1 != null && !this.toolStripProgressBar1.IsDisposed)
            {
                this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Minimum;
                this.toolStripProgressBar1.Visible = false;
            }
        }

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void frmPoissonDraw_Load(object sender, EventArgs e)
        {
            this.cmbAlg.SelectedIndex = 1;

            this.cmbZoom.Items.Add((0.75F).ToString());
            this.cmbZoom.Items.Add((0.5F).ToString());
            this.cmbZoom.Items.Add((0.25F).ToString());

            CheckBox12_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cmbZoom.SelectedIndex = 4;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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

            if (this._penStrokesDone)
            {
                DialogResult dlg = MessageBox.Show("Save penStrokes image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dlg == DialogResult.Yes)
                    this.btnSaveStrokes.PerformClick();
                else if (dlg == DialogResult.Cancel)
                    e.Cancel = true;
            }

            if (!e.Cancel)
            {
                if (this._cAlg != null)
                    this._cAlg.CancellationPending = true;
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();
                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                if (this._bmpOrigHLC1 != null)
                    this._bmpOrigHLC1.Dispose();
                if (this._bmpPenStrokes != null)
                    this._bmpPenStrokes.Dispose();
                if (this.pictureBox1.Image != null)
                    this.pictureBox1.Image.Dispose();
                //if (this._bmpOrg != null)
                //    this._bmpOrg.Dispose();    
                //if (this._bmpDraw != null)
                //    this._bmpDraw.Dispose();  //disposed in the designer file
            }
        }

        private void MakeBitmap(Bitmap bmpIn, float zoom)
        {
            if (zoom != 0.0)
            {
                Bitmap? bOLd = this._bmpBUZoomed;
                if (this._bmpBU != null)
                {
                    int w2 = System.Convert.ToInt32(Math.Ceiling(this._bmpBU.Width * zoom));
                    int h2 = System.Convert.ToInt32(Math.Ceiling(this._bmpBU.Height * zoom));

                    if (AvailMem.AvailMem.checkAvailRam(w2 * h2 * 4L))
                        this._bmpBUZoomed = new Bitmap(w2, h2);
                    else
                        return;

                    using (Graphics g = Graphics.FromImage(this._bmpBUZoomed))
                        g.DrawImage(bmpIn, 0, 0, System.Convert.ToInt32(Math.Ceiling(this._bmpBU.Width * zoom)), System.Convert.ToInt32(Math.Ceiling(this._bmpBU.Height * zoom)));

                    if (bOLd != null)
                        bOLd.Dispose();
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object? sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            if (this.toolStripProgressBar1 != null && !this.toolStripProgressBar1.IsDisposed)
                this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), this.toolStripProgressBar1.Minimum);
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Bitmap? bmp = null;

            if (e.Result != null)
            {
                bmp = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                this.SetControls(true);
                this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());

                this._pic_changed = true;

                this._pb?.BlendParameters.Dispose();
                this._cAlg = null;

                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                _undoOPCache?.Add(this.helplineRulerCtrl2.Bmp);
                // _undoOPCache2.Add(Me._bmpBUZoomed)

                this.btnUndo.Enabled = _undoOPCache?.CurrentPosition > 1;
                CheckRedoButton();

                this.CurPath = new List<PointF>();
                if (this._bmpPenStrokes != null)
                    this.cbShowCustPic.Checked = false;

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.Timer3.Start();

                //re init the bgw
                this.backgroundWorker1.Dispose();
                this.backgroundWorker1 = new BackgroundWorker();
                this.backgroundWorker1.WorkerReportsProgress = true;
                this.backgroundWorker1.WorkerSupportsCancellation = true;
                this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            }
        }

        private void btnNewPath_Click(object sender, EventArgs e)
        {
            if (this.CurPath != null)
                this.CurPath.Clear();

            if (this._bmpOrg != null)
            {
                Bitmap b = new Bitmap(this._bmpOrg);
                this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");
            }

            if (this.helplineRulerCtrl2.Bmp != null)
            {
                Bitmap? bOld = this._bmpBU;
                this._bmpBU = new Bitmap(this.helplineRulerCtrl2.Bmp);
                this._bmpDraw = new Bitmap(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                if (bOld != null)
                {
                    bOld.Dispose();
                    bOld = null;
                }

                this._penStrokesDone = false;
            }

            this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            _undoOPCache?.Reset(false);

            this.btnUndo.Enabled = false;

            if (_undoOPCache?.Count > 1)
                this.btnRedo.Enabled = true;
            else
                this.btnRedo.Enabled = false;

            this._fp = 0;
        }

        private void cmbAlg_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.label3.Enabled = false;
            this.label4.Enabled = false;
            this.label10.Enabled = false;
            this.label12.Enabled = false;
            this.numUpperWeight.Enabled = false;
            this.numLowerWeight.Enabled = false;
            this.numMaxPixelDist.Enabled = false;
            this.numGamma.Enabled = false;

            if (this.cmbAlg.SelectedIndex > 2)
            {
                this.label3.Enabled = true;
                this.label4.Enabled = true;
                this.numUpperWeight.Enabled = true;
                this.numLowerWeight.Enabled = true;

                if (this.cmbAlg.SelectedIndex > 3)
                {
                    this.label10.Enabled = true;
                    this.label12.Enabled = true;
                    this.numMaxPixelDist.Enabled = true;
                    this.numGamma.Enabled = true;
                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;
                using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                    bmp = new Bitmap(img);

                Bitmap? bmpHLC1 = new Bitmap(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                using TextureBrush tb = new TextureBrush(bmp);
                tb.WrapMode = WrapMode.TileFlipXY;
                using Graphics gx = Graphics.FromImage(bmpHLC1);
                gx.FillRectangle(tb, new RectangleF(0, 0, bmpHLC1.Width, bmpHLC1.Height));
                if (bmp != null)
                    bmp.Dispose();
                bmp = null;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmpHLC1, this.helplineRulerCtrl1, "Bmp");

                //double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                //double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                //if (multiplier >= faktor)
                //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                //else
                //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                Bitmap? bC = new Bitmap(bmpHLC1);
                this.SetBitmap(ref this._bmpBU, ref bC);

                Bitmap? bD = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                this.SetBitmap(ref this._bmpDraw, ref bD);

                Bitmap? bC2 = new Bitmap(bmpHLC1);
                this.SetBitmap(ref this._bmpOrigHLC1, ref bC2);

                if (this._bmpBUZoomed != null)
                    this._bmpBUZoomed.Dispose();
                this._bmpBUZoomed = null;
                if (this._bmpBUZoomed == null && this._bmpBU != null)
                    MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);

                //this._sourcePt = new Point(0, 0);

                int dx = this._destPt.X - this._sourcePt.X;
                int dy = this._destPt.Y - this._sourcePt.Y;

                if (this._tb != null)
                    this._tb.Dispose();
                this._tb = null;
                SetupTB();

                this._tb?.ResetTransform();
                this._tb?.TranslateTransform(dx, dy);

                this.btnLoadEdited.Enabled = true;
            }
        }

        private void cbOverlay_CheckedChanged(object sender, EventArgs e)
        {
            this._overlaySrc = cbOverlay.Checked;
            this.helplineRulerCtrl2.dbPanel1.Invalidate();
        }

        private void numOpacity_ValueChanged(object sender, EventArgs e)
        {
            if (this._overlaySrc)
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
        }

        internal void LoadOrigPic(Bitmap bmpBU)
        {
            Bitmap? bmp = new Bitmap(bmpBU);

            Bitmap? bmpHLC1 = new Bitmap(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
            using TextureBrush tb = new TextureBrush(bmp);
            tb.WrapMode = WrapMode.TileFlipXY;
            using Graphics gx = Graphics.FromImage(bmpHLC1);
            gx.FillRectangle(tb, new RectangleF(0, 0, bmpHLC1.Width, bmpHLC1.Height));
            if (bmp != null)
                bmp.Dispose();
            bmp = null;

            this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmpHLC1, this.helplineRulerCtrl1, "Bmp");

            //double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
            //double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
            //if (multiplier >= faktor)
            //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
            //else
            //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            Bitmap? bC = new Bitmap(bmpHLC1);
            this.SetBitmap(ref this._bmpBU, ref bC);

            Bitmap? bD = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
            this.SetBitmap(ref this._bmpDraw, ref bD);

            Bitmap? bC2 = new Bitmap(bmpHLC1);
            this.SetBitmap(ref this._bmpOrigHLC1, ref bC2);

            if (this._bmpBUZoomed != null)
                this._bmpBUZoomed.Dispose();
            this._bmpBUZoomed = null;
            if (this._bmpBUZoomed == null && this._bmpBU != null)
                MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);

            //this._sourcePt = new Point(0, 0);

            int dx = this._destPt.X - this._sourcePt.X;
            int dy = this._destPt.Y - this._sourcePt.Y;

            if (this._tb != null)
                this._tb.Dispose();
            this._tb = null;
            SetupTB();

            this._tb?.ResetTransform();
            this._tb?.TranslateTransform(dx, dy);

            this.btnLoadEdited.Enabled = true;
        }

        private void btnScreenBlend_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap b = new Bitmap(this.helplineRulerCtrl1.Bmp);
                ScreenBlend(b);

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                //double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                //double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                //if (multiplier >= faktor)
                //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                //else
                //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                Bitmap? bC = new Bitmap(b);
                this.SetBitmap(ref this._bmpBU, ref bC);

                Bitmap? bD = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                this.SetBitmap(ref this._bmpDraw, ref bD);

                if (this._bmpBUZoomed != null)
                    this._bmpBUZoomed.Dispose();
                this._bmpBUZoomed = null;
                if (this._bmpBUZoomed == null && this._bmpBU != null)
                    MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);

                //this._sourcePt = new Point(0, 0);

                int dx = this._destPt.X - this._sourcePt.X;
                int dy = this._destPt.Y - this._sourcePt.Y;

                if (this._tb != null)
                    this._tb.Dispose();
                this._tb = null;
                SetupTB();

                this._tb?.ResetTransform();
                this._tb?.TranslateTransform(dx, dy);
            }
        }

        private unsafe void ScreenBlend(Bitmap b)
        {
            int w = b.Width;
            int h = b.Height;
            BitmapData bmD = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    p[0] = (byte)Math.Max(Math.Min(Math.Sqrt((double)p[0] * (double)p[0] + (double)p[0] * (double)p[0]), 255), 0);
                    p[1] = (byte)Math.Max(Math.Min(Math.Sqrt((double)p[1] * (double)p[1] + (double)p[1] * (double)p[1]), 255), 0);
                    p[2] = (byte)Math.Max(Math.Min(Math.Sqrt((double)p[2] * (double)p[2] + (double)p[2] * (double)p[2]), 255), 0);

                    p += 4;
                }
            });

            b.UnlockBits(bmD);
        }

        private void btnSaveStrokes_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && this.helplineRulerCtrl1.Bmp != null && this.Paths != null)
            {
                using Bitmap bmp = new(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                using Graphics gx = Graphics.FromImage(bmp);
                using TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp);

                for (int i = 0; i < this.Paths.Count; i++)
                {
                    float w = this.Paths[i].DrawWidth;
                    tb?.ResetTransform();
                    tb?.TranslateTransform(this.Paths[i].Offset.X, this.Paths[i].Offset.Y);

                    // Me.ComboBox1.SelectedIndex = Me.Paths(i).BVAlg
                    // Me.NumericUpDown2.Value = CDec(Me.Paths(i).UpperWeight)
                    // Me.NumericUpDown3.Value = CDec(Me.Paths(i).LowerWeight)

                    if (tb != null)
                        using (Pen pen = new Pen(tb, w))
                        {
                            pen.LineJoin = LineJoin.Round;

                            if (this.Paths[i].RoundCaps)
                            {
                                pen.StartCap = LineCap.Round;
                                pen.EndCap = LineCap.Round;
                            }

                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                if (this.Paths[i]?.Count() == 1)
                                {
                                    gp.AddEllipse(this.Paths[i].Points[0].X - w, this.Paths[i].Points[0].Y - w, w * 2, w * 2);
                                    gx.FillPath(tb, gp);
                                }
                                else
                                {
                                    gp.AddLines(this.Paths[i].ToArray());
                                    gx.DrawPath(pen, gp);
                                }
                            }
                        }
                }

                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    bmp.Save(this.saveFileDialog1.FileName, ImageFormat.Png);
            }
        }

        private void numSrcPtSurround_ValueChanged(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void cbFindDestPoint_CheckedChanged(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnFindDestPoint_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.helplineRulerCtrl2.Bmp != null && this._bmpOrigHLC1 != null)
            {
                Bitmap? bmpToFind = null;
                try
                {
                    if (this.cbFindInOrig.Checked)
                    {
                        bmpToFind = this._bmpOrigHLC1.Clone(new Rectangle(
                           Math.Max(this._sourcePt.X - (int)this.numSrcPtSurround.Value, 0),
                           Math.Max(this._sourcePt.Y - (int)this.numSrcPtSurround.Value, 0),
                           Math.Min((int)this.numSrcPtSurround.Value * 2 + 1, this._bmpOrigHLC1.Width - 1 - this._sourcePt.X),
                           Math.Min((int)this.numSrcPtSurround.Value * 2 + 1, this._bmpOrigHLC1.Height - 1 - this._sourcePt.Y)),
                           this._bmpOrigHLC1.PixelFormat);

                        if (bmpToFind.Width > 1 && (bmpToFind.Width & 0x01) != 1)
                            bmpToFind = bmpToFind.Clone(new Rectangle(0, 0, bmpToFind.Width - 1, bmpToFind.Height), bmpToFind.PixelFormat);

                        if (bmpToFind.Height > 1 && (bmpToFind.Height & 0x01) != 1)
                            bmpToFind = bmpToFind.Clone(new Rectangle(0, 0, bmpToFind.Width, bmpToFind.Height - 1), bmpToFind.PixelFormat);
                    }
                    else
                    {
                        bmpToFind = this.helplineRulerCtrl1.Bmp.Clone(new Rectangle(
                           Math.Max(this._sourcePt.X - (int)this.numSrcPtSurround.Value, 0),
                           Math.Max(this._sourcePt.Y - (int)this.numSrcPtSurround.Value, 0),
                           Math.Min((int)this.numSrcPtSurround.Value * 2 + 1, this.helplineRulerCtrl1.Bmp.Width - 1 - this._sourcePt.X),
                           Math.Min((int)this.numSrcPtSurround.Value * 2 + 1, this.helplineRulerCtrl1.Bmp.Height - 1 - this._sourcePt.Y)),
                           this.helplineRulerCtrl1.Bmp.PixelFormat);

                        if (bmpToFind.Width > 1 && (bmpToFind.Width & 0x01) != 1)
                            bmpToFind = bmpToFind.Clone(new Rectangle(0, 0, bmpToFind.Width - 1, bmpToFind.Height), bmpToFind.PixelFormat);

                        if (bmpToFind.Height > 1 && (bmpToFind.Height & 0x01) != 1)
                            bmpToFind = bmpToFind.Clone(new Rectangle(0, 0, bmpToFind.Width, bmpToFind.Height - 1), bmpToFind.PixelFormat);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return;
                }

                if (bmpToFind != null && bmpToFind.Width > 18 && bmpToFind.Height > 18)
                {
                    this.SetControls(false);
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    //pad image
                    Bitmap bmpToSearch = new Bitmap(this.helplineRulerCtrl2.Bmp.Width + (int)this.numSrcPtSurround.Value * 2,
                        this.helplineRulerCtrl2.Bmp.Height + (int)this.numSrcPtSurround.Value * 2);
                    using Graphics gx = Graphics.FromImage(bmpToSearch);
                    gx.DrawImage(this.helplineRulerCtrl2.Bmp, (int)this.numSrcPtSurround.Value, (int)this.numSrcPtSurround.Value);

                    this.backgroundWorker2.RunWorkerAsync(new object[] { bmpToFind, bmpToSearch, (int)this.numSrcPtSurround.Value });
                }
            }
        }

        private bool LoG_PositiveValues(Bitmap b, int Length, bool doTransparency, Convolution conv,
            System.ComponentModel.BackgroundWorker bgw, double stddev, bool f_auto, double f, double divisor, bool grayscale)
        {
            if (grayscale)
                Grayscale(b);

            double[,] Kernel = new double[Length - 1 + 1, Length - 1 + 1];
            int Radius = Length / 2;

            stddev *= Radius;
            double stddev2 = stddev * stddev;
            double stddev4 = stddev2 * stddev2;

            double ff = f;
            if (f_auto)
                ff = -1.0 / (Math.PI * stddev4);

            double a = (2.0 * stddev2);

            double Sum = 0.0;

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                {
                    double dist = Math.Sqrt((x - Radius) * (x - Radius) + (y - Radius) * (y - Radius));
                    double nabla2 = 1.0 - (dist * dist / (2.0 * stddev2));
                    Kernel[x, y] = ff * nabla2 * Math.Exp(-dist * dist / a);

                    Sum += Kernel[x, y];
                }
            }

            for (int y = 0; y <= Kernel.GetLength(1) - 1; y++)
            {
                for (int x = 0; x <= Kernel.GetLength(0) - 1; x++)
                    Kernel[x, y] /= Sum;
            }

            double[,] AddVals = conv.CalculateStandardAddVals(Kernel);

            ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(b.Height, 20, 1);

            return conv.Convolve_par_Pos(b, Kernel, AddVals, 0, 255, false, true, pe, bgw, divisor);
        }

        private unsafe void Grayscale(Bitmap bmp)
        {
            BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;
            int nWidth = bmp.Width;
            int nHeight = bmp.Height;

            byte* p = (byte*)bmData.Scan0;

            int pos = 0;
            for (int y = 0; y < nHeight; y++)
            {
                pos = y * stride;
                for (int x = 0; x < nWidth; x++)
                {
                    int v = (int)Math.Max(Math.Min((double)p[pos] * 0.11 + (double)p[pos + 1] * 0.59 + (double)p[pos + 2] * 0.3, 255), 0);
                    p[pos] = p[pos + 1] = p[pos + 2] = (byte)v;

                    pos += 4;
                }
            }

            bmp.UnlockBits(bmData);
        }

        private void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap? bmpToFind = (Bitmap)o[0];
                Bitmap? bmpToSearch = (Bitmap)o[1];
                int wh = (int)o[2];
                Convolution conv = new Convolution();
                conv.CancelLoops = false;
                conv.ProgressPlus += Conv_ProgressPlus;

                //get an edge representation by convolving in log mode (positive) (grayscaled)
                LoG_PositiveValues(bmpToFind, 7, false, conv, this.backgroundWorker2, 0.3, false, 1.0, 1.0, true);

                this.backgroundWorker2.ReportProgress(0);
                conv.CancelLoops = false;

                //get an edge representation by convolving in log mode (positive) (grayscaled)
                LoG_PositiveValues(bmpToSearch, 7, false, conv, this.backgroundWorker2, 0.3, false, 1.0, 1.0, true);
                conv.ProgressPlus -= Conv_ProgressPlus;

                this.backgroundWorker2.ReportProgress(0);

                //read in bmpToFind (just one channel) as kernel
                double[,] kernel = ReadInBitmap(bmpToFind);

                if (kernel.GetLength(0) < 15 || FFT_related.CheckWH(new Size(kernel.GetLength(0), kernel.GetLength(1))) > 4096) //restrict size a bit
                {
                    ConvolutionLib.ProgressEventArgs pe = new ConvolutionLib.ProgressEventArgs(bmpToSearch.Height, 20, 1);
                    Correlate(bmpToSearch, kernel, pe, this.backgroundWorker2);
                }
                else
                {
                    using Bitmap bmpToSearchTmp = bmpToSearch;
                    bmpToSearch = FFT_related.Convolve(bmpToSearchTmp, MakeKernelSumOne(kernel), 0, this.backgroundWorker2);
                }

                if (bmpToFind != null)
                    bmpToFind.Dispose();
                bmpToFind = null;

                e.Result = FindPoint(bmpToSearch, wh);

                if (bmpToSearch != null)
                    bmpToSearch.Dispose();
                bmpToSearch = null;
            }
        }

        private double[,] MakeKernelSumOne(double[,] kernel)
        {
            double sum = kernel.Cast<double>().Sum();

            for (int y = 0; y < kernel.GetLength(1); y++)
                for (int x = 0; x < kernel.GetLength(0); x++)
                    kernel[x, y] /= sum;

            double[,] d = new double[kernel.GetLength(0), kernel.GetLength(1)];

            //rotate by 180°
            for (int y = 0; y < kernel.GetLength(1); y++)
                for (int x = 0; x < kernel.GetLength(0); x++)
                    d[x, y] = kernel[kernel.GetLength(0) - 1 - x, kernel.GetLength(1) - 1 - y];

            return d;
        }

        private unsafe double[,] ReadInBitmap(Bitmap bmp)
        {
            int xAdd = 0;
            int yAdd = 0;

            if ((bmp.Width & 0x1) != 1)
                xAdd = -1;
            if ((bmp.Height & 0x1) != 1)
                yAdd = -1;

            using (Bitmap bWork = new Bitmap(bmp.Width + xAdd, bmp.Height + yAdd))
            {
                using (Graphics gx = Graphics.FromImage(bWork))
                    gx.DrawImage(bmp, 0, 0);

                double[,] kernel = new double[bWork.Width, bWork.Height];

                BitmapData bmData = bWork.LockBits(new Rectangle(0, 0, bWork.Width, bWork.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmData.Stride;
                int nWidth = bWork.Width;
                int nHeight = bWork.Height;

                Parallel.For(0, nHeight, y =>
                {
                    byte* p = (byte*)bmData.Scan0;
                    for (int x = 0; x < nWidth; x++)
                        kernel[x, y] = p[y * stride + x * 4];
                });

                bWork.UnlockBits(bmData);

                return kernel;
            }
        }

        private unsafe Point FindPoint(Bitmap bmpToSearch, int wh)
        {
            int w = bmpToSearch.Width;
            int h = bmpToSearch.Height;
            int maxVal = 0;
            int xx = 0;
            int yy = 0;

            BitmapData bmData = bmpToSearch.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;

            byte* p = (byte*)bmData.Scan0;

            for (int y = 0; y < h; y++)
            {
                p = (byte*)bmData.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[0] > maxVal)
                    {
                        maxVal = p[0];
                        xx = x;
                        yy = y;
                    }

                    p += 4;
                }
            }

            bmpToSearch.UnlockBits(bmData);

            return new Point(xx - wh, yy - wh);
        }

        private void Conv_ProgressPlus(object sender, ConvolutionLib.ProgressEventArgs e)
        {
            this.backgroundWorker2.ReportProgress(Math.Min((int)e.CurrentProgress, 100));
        }

        private void backgroundWorker2_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => { this.toolStripProgressBar1.Value = Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum); }));
            else
                this.toolStripProgressBar1.Value = Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum);
        }

        private void backgroundWorker2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Point pt = (Point)e.Result;
                this._destPt = pt;
                if (this.cbAuto.Checked)
                    this.cbSetPts.Checked = false;

                this.toolStripStatusLabel6.Text = "Src " + this._sourcePt.ToString() + " Dest: " + this._destPt.ToString() +
                    "\n Diff: x: " + (this._destPt.X - this._sourcePt.X).ToString() + " y: " + (this._destPt.Y - this._sourcePt.Y).ToString();
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }

            this.SetControls(true);
            this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            this.Timer3.Start();

            //re init the bgw
            this.backgroundWorker2.Dispose();
            this.backgroundWorker2 = new BackgroundWorker();
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            this.backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            this.backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
        }

        // correlate, note: dont rotate bmpToFind
        public unsafe bool Correlate(Bitmap b, double[,] Kernel, ConvolutionLib.ProgressEventArgs pe, System.ComponentModel.BackgroundWorker bgw)
        {
            if (!AvailMem.AvailMem.checkAvailRam(b.Width * b.Height * 12L))
                throw new OutOfMemoryException("Not enough Memory.");
            if (Kernel.GetLength(0) != Kernel.GetLength(1))
                throw new Exception("Kernel must be quadratic.");
            if ((Kernel.GetLength(0) & 0x1) != 1)
                throw new Exception("Kernelrows Length must be Odd.");
            if (Kernel.GetLength(0) < 3)
                throw new Exception("Kernelrows Length must be in the range from 3 to" + (Math.Min(b.Width - 1, b.Height - 1)).ToString() + ".");
            if (Kernel.GetLength(0) > Math.Min(b.Width - 1, b.Height - 1))
                throw new Exception("Kernelrows Length must be in the range from 3 to" + (Math.Min(b.Width - 1, b.Height - 1)).ToString() + ".");

            int h = Kernel.GetLength(0) / 2;

            Bitmap? bSrc = null;
            BitmapData? bmData = null;
            BitmapData? bmSrc = null;

            try
            {
                bSrc = (Bitmap)b.Clone();
                bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmData.Stride;

                System.IntPtr Scan0 = bmData.Scan0;
                System.IntPtr SrcScan0 = bmSrc.Scan0;

                int nWidth = b.Width;
                int nHeight = b.Height;

                int llh = h * stride;
                int lh = h * 4;

                // #Region "Main Body"
                // For y As Integer = 0 To nHeight - Kernel.GetLength(1)
                Parallel.For(0, nHeight - Kernel.GetLength(1) + 1, (y, loopState) =>
                {
                    try
                    {
                        if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                            loopState.Break();
                    }
                    catch
                    {
                    }

                    double fSum = 0.0;
                    double KfSum = 0.0;
                    double fCount = 0.0;
                    double z = 1.0 / (Kernel.GetLength(0) * Kernel.GetLength(1));

                    int pos = 0;
                    int posSrc = 0;

                    // #Region "Standard"
                    z = 1.0 / (Kernel.GetLength(0) * Kernel.GetLength(1));

                    byte* p = (byte*)bmData.Scan0;
                    byte* pSrc = (byte*)bmSrc.Scan0;

                    for (int x = 0; x <= nWidth - Kernel.GetLength(0); x++)
                    {
                        fSum = 0.0;
                        KfSum = 0.0;
                        fCount = 0.0;

                        pos = 0;
                        pos += y * stride + x * 4;

                        posSrc = 0;
                        posSrc += y * stride + x * 4;

                        for (int row = 0; row <= Kernel.GetLength(1) - 1; row++)
                        {
                            int llr = row * stride;

                            for (int col = 0; col <= Kernel.GetLength(0) - 1; col++)
                            {
                                int lc = col * 4;

                                fSum += (System.Convert.ToDouble(pSrc[posSrc + llr + lc]) * Kernel[col, row]);
                                fCount += z;

                                KfSum += Kernel[col, row];
                            }
                        }

                        if (KfSum == 0.0)
                            KfSum = 1.0;

                        if (fCount == 0.0)
                            fCount = 1.0;

                        pos += llh + lh;
                        p[pos] = System.Convert.ToByte(Math.Max(Math.Min(System.Convert.ToInt32(Math.Max(Math.Min((fSum / KfSum), Int32.MaxValue), Int32.MinValue) / System.Convert.ToDouble(fCount)), 255), 0));
                        p[pos + 1] = System.Convert.ToByte(Math.Max(Math.Min(System.Convert.ToInt32(Math.Max(Math.Min((fSum / KfSum), Int32.MaxValue), Int32.MinValue) / System.Convert.ToDouble(fCount)), 255), 0));
                        p[pos + 2] = System.Convert.ToByte(Math.Max(Math.Min(System.Convert.ToInt32(Math.Max(Math.Min((fSum / KfSum), Int32.MaxValue), Int32.MinValue) / System.Convert.ToDouble(fCount)), 255), 0));
                    }
                    // #End Region

                    lock (this._lockObject)
                    {
                        if (pe != null)
                        {
                            if (pe.ImgWidthHeight < Int32.MaxValue)
                                pe.CurrentProgress += 1;
                            try
                            {
                                if (System.Convert.ToInt32(pe.CurrentProgress) % pe.PrgInterval == 0)
                                    this.backgroundWorker2.ReportProgress(Math.Min(
                                        (int)(((double)pe.CurrentProgress / (double)pe.ImgWidthHeight) * 100), 100));
                            }
                            catch
                            {
                            }
                        }
                    }
                });
                // Next
                // #End Region

                b.UnlockBits(bmData);
                bSrc.UnlockBits(bmSrc);
                bSrc.Dispose();

                return true;
            }
            catch
            {
                try
                {
                    if (bmData != null)
                        b.UnlockBits(bmData);
                }
                catch
                {
                }

                try
                {
                    if (bSrc != null && bmSrc != null)
                        bSrc.UnlockBits(bmSrc);
                }
                catch
                {
                }

                if (bSrc != null)
                {
                    bSrc.Dispose();
                    bSrc = null;
                }
            }
            return false;
        }

        private void cbDraw_CheckedChanged(object sender, EventArgs e)
        {
            this.cbClickMode.Enabled = this.cbDraw.Checked;
            this.cbOrdinaryDraw.Enabled = this.label14.Enabled = this.numOpacityOrdDraw.Enabled = this.cbDraw.Checked;
        }

        private void cbClickMode_CheckedChanged(object sender, EventArgs e)
        {
            this.panel2.Visible = this.cbClickMode.Checked;
        }

        private void btnReBlend_Click(object sender, EventArgs e)
        {
            Bitmap? bmpBlend = null;

            if (this.cbOrdinaryDraw.Checked)
                if (MessageBox.Show("Ordinary draw checkBox is checked. Maybe you want to uncheck it first, since else normal drawing will be done, no poisson blending.",
                    "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                    return;

            if (this.cbUseCustomReBlendPic.Checked)
            {
                if (this._bmpPenStrokes != null)
                {
                    Bitmap? bmp = new Bitmap(this._bmpPenStrokes);

                    SetPicToPB(bmp);

                    Bitmap bmpDrawTo = new Bitmap(this.helplineRulerCtrl2.Bmp);

                    this.SetControls(false);

                    ProcessImage(bmpDrawTo, bmp);
                }
                else
                {
                    this.SetControls(true);
                    this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                    this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                }
            }
            else
            {
                if (this.helplineRulerCtrl2.Bmp != null && this.helplineRulerCtrl1.Bmp != null)
                {
                    if (this.cbWholeRegionPic.Checked && this._bmpPenStrokes != null)
                    {
                        Bitmap? bmp = new Bitmap(this._bmpPenStrokes);
                        using Bitmap bSrc = new Bitmap(this.helplineRulerCtrl1.Bmp);

                        if (bSrc.Width != bmp.Width || bSrc.Height != bmp.Height)
                        {
                            Bitmap? bOld = bmp;
                            bmp = new Bitmap(bSrc.Width, bSrc.Height);
                            using Graphics g = Graphics.FromImage(bmp);
                            //g.DrawImage(bOld, 0, 0, bSrc.Width, bSrc.Height);
                            g.DrawImage(bOld, 0, 0);

                            if (bOld != null)
                                bOld.Dispose();
                            bOld = null;
                        }

                        bmpBlend = GetSurroundingRegion(bmp, bSrc, (int)this.numExtendRegion.Value);

                        if (bmpBlend != null)
                        {
                            SetPicToPB(bmpBlend);

                            Bitmap bmpDrawTo = new Bitmap(this.helplineRulerCtrl2.Bmp);

                            this.SetControls(false);
                            ProcessImage(bmpDrawTo, bmpBlend);
                        }
                        else
                        {
                            this.SetControls(true);
                            this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                        }

                        if (bmp != null)
                            bmp.Dispose();
                        bmp = null;
                    }
                    else if (this.helplineRulerCtrl2.Bmp != null && this.helplineRulerCtrl1.Bmp != null && this.Paths != null &&
                        ((!this.cbWholeRegionPic.Checked && !this.cbWholeRegionPic.Checked && !this.cbBlackBG.Checked) || (this._bmpPenStrokes == null && !this.cbBlackBG.Checked)))
                    {
                        using Bitmap bmp = new(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                        using Graphics gx = Graphics.FromImage(bmp);
                        using TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp);

                        for (int i = 0; i < this.Paths.Count; i++)
                        {
                            float w = this.Paths[i].DrawWidth;
                            tb?.ResetTransform();
                            tb?.TranslateTransform(this.Paths[i].Offset.X, this.Paths[i].Offset.Y);

                            if (tb != null)
                                using (Pen pen = new Pen(tb, w))
                                {
                                    pen.LineJoin = LineJoin.Round;

                                    if (this.Paths[i].RoundCaps)
                                    {
                                        pen.StartCap = LineCap.Round;
                                        pen.EndCap = LineCap.Round;
                                    }

                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        if (this.Paths[i]?.Count() == 1)
                                        {
                                            gp.AddEllipse(this.Paths[i].Points[0].X - w, this.Paths[i].Points[0].Y - w, w * 2, w * 2);
                                            gx.FillPath(tb, gp);
                                        }
                                        else
                                        {
                                            gp.AddLines(this.Paths[i].ToArray());
                                            gx.DrawPath(pen, gp);
                                        }
                                    }
                                }
                        }

                        bmpBlend = new Bitmap(bmp);

                        SetPicToPB(bmpBlend);

                        Bitmap bmpBlend2 = new Bitmap(bmpBlend);
                        this.SetBitmap(ref this._bmpPenStrokes, ref bmpBlend2);

                        Bitmap bmpDrawTo = new Bitmap(this.helplineRulerCtrl2.Bmp);

                        this.SetControls(false);

                        if (this.cbWholeRegionPic.Checked)
                        {
                            using Bitmap bSrc = new Bitmap(this.helplineRulerCtrl1.Bmp);

                            if (bSrc.Width != bmpBlend.Width || bSrc.Height != bmpBlend.Height)
                            {
                                Bitmap? bOld = bmpBlend;
                                bmpBlend = new Bitmap(bSrc.Width, bSrc.Height);
                                using Graphics g = Graphics.FromImage(bmpBlend);
                                //g.DrawImage(bOld, 0, 0, bSrc.Width, bSrc.Height);
                                g.DrawImage(bOld, 0, 0);

                                if (bOld != null)
                                    bOld.Dispose();
                                bOld = null;
                            }

                            bmpBlend = GetSurroundingRegion(bmp, bSrc, (int)this.numExtendRegion.Value);

                            if (bmpBlend != null)
                            {
                                SetPicToPB(bmpBlend);

                                ProcessImage(bmpDrawTo, bmpBlend);
                            }
                            else
                            {
                                this.SetControls(true);
                                this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                                this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                            }
                        }
                        else
                        {
                            ProcessImage(bmpDrawTo, bmpBlend);
                        }
                    }
                    else if (this.helplineRulerCtrl2.Bmp != null && this.helplineRulerCtrl1.Bmp != null && this.Paths != null &&
                        ((!this.cbWholeRegionPic.Checked && !this.cbWholeRegionPic.Checked && this.cbBlackBG.Checked) || (this._bmpPenStrokes == null && this.cbBlackBG.Checked)))
                    {
                        using Bitmap bmp = new(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                        using Graphics gx = Graphics.FromImage(bmp);
                        using TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp);

                        for (int i = 0; i < this.Paths.Count; i++)
                        {
                            float w = this.Paths[i].DrawWidth;
                            tb?.ResetTransform();
                            tb?.TranslateTransform(this.Paths[i].Offset.X, this.Paths[i].Offset.Y);

                            if (tb != null)
                                using (Pen pen = new Pen(tb, w))
                                {
                                    pen.LineJoin = LineJoin.Round;

                                    if (this.Paths[i].RoundCaps)
                                    {
                                        pen.StartCap = LineCap.Round;
                                        pen.EndCap = LineCap.Round;
                                    }

                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        if (this.Paths[i]?.Count() == 1)
                                        {
                                            gp.AddEllipse(this.Paths[i].Points[0].X - w, this.Paths[i].Points[0].Y - w, w * 2, w * 2);
                                            gx.FillPath(tb, gp);
                                        }
                                        else
                                        {
                                            gp.AddLines(this.Paths[i].ToArray());
                                            gx.DrawPath(pen, gp);
                                        }
                                    }
                                }
                        }

                        bmpBlend = new Bitmap(bmp);

                        SetPicToPB(bmpBlend);

                        Bitmap bmpBlend2 = new Bitmap(bmpBlend);
                        this.SetBitmap(ref this._bmpPenStrokes, ref bmpBlend2);

                        Bitmap bmpDrawTo = new Bitmap(this.helplineRulerCtrl2.Bmp);

                        this.SetControls(false);

                        using Bitmap bSrc = new Bitmap(this.helplineRulerCtrl1.Bmp);

                        if (bSrc.Width != bmpBlend.Width || bSrc.Height != bmpBlend.Height)
                        {
                            Bitmap? bOld = bmpBlend;
                            bmpBlend = new Bitmap(bSrc.Width, bSrc.Height);
                            using Graphics g = Graphics.FromImage(bmpBlend);
                            //g.DrawImage(bOld, 0, 0, bSrc.Width, bSrc.Height);
                            g.DrawImage(bOld, 0, 0);

                            if (bOld != null)
                                bOld.Dispose();
                            bOld = null;
                        }

                        using Bitmap bCopy = new Bitmap(bmp);
                        bmpBlend = GetSurroundingRegion(bmp, bSrc, (int)this.numExtendRegion.Value);

                        if (bmpBlend != null)
                        {
                            if (this.cbBlackBG.Checked)
                            {
                                this.colorDialog1.CustomColors = CustomColors;
                                if (this.cbDiffCol.Checked && this.colorDialog1.ShowDialog() == DialogResult.OK)
                                {
                                    Color c = this.colorDialog1.Color;
                                    CustomColors = this.colorDialog1.CustomColors;

                                    GetColoredShapeBGBmp(bmpBlend, bCopy, c);
                                }
                                else
                                    GetBlackShapeBGBmp(bmpBlend, bCopy);
                            }

                            SetPicToPB(bmpBlend);

                            ProcessImage(bmpDrawTo, bmpBlend);

                        }
                        else
                        {
                            this.SetControls(true);
                            this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                        }
                    }
                    else if (this.helplineRulerCtrl2.Bmp != null && this.helplineRulerCtrl1.Bmp != null && this.Paths == null)
                    {
                        MessageBox.Show("No penstrokes done so far.");
                        this.SetControls(true);
                        this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                        this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                    }
                }
            }
        }

        private void SetPicToPB(Bitmap bmp)
        {
            Image? iOld = this.pictureBox1.Image;
            this.pictureBox1.Image = new Bitmap(bmp);
            this.pictureBox1.Refresh();

            if (iOld != null)
                iOld.Dispose();
            iOld = null;
        }

        private void btnLoadCustomPenStrokesPic_Click(object sender, EventArgs e)
        {
            if (!this.cbUseCustomReBlendPic.Checked && this.cbWholeRegionPic.Checked)
            {
                Bitmap? bmpBlend = null;

                if (this.helplineRulerCtrl2.Bmp != null && this.helplineRulerCtrl1.Bmp != null && this.Paths != null)
                {
                    Bitmap? bmp = new(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                    using Graphics gx = Graphics.FromImage(bmp);
                    using TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp);

                    for (int i = 0; i < this.Paths.Count; i++)
                    {
                        float w = this.Paths[i].DrawWidth;
                        tb?.ResetTransform();
                        tb?.TranslateTransform(this.Paths[i].Offset.X, this.Paths[i].Offset.Y);

                        if (tb != null)
                            using (Pen pen = new Pen(tb, w))
                            {
                                pen.LineJoin = LineJoin.Round;

                                if (this.Paths[i].RoundCaps)
                                {
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;
                                }

                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    if (this.Paths[i]?.Count() == 1)
                                    {
                                        gp.AddEllipse(this.Paths[i].Points[0].X - w, this.Paths[i].Points[0].Y - w, w * 2, w * 2);
                                        gx.FillPath(tb, gp);
                                    }
                                    else
                                    {
                                        gp.AddLines(this.Paths[i].ToArray());
                                        gx.DrawPath(pen, gp);
                                    }
                                }
                            }
                    }

                    using Bitmap bSrc = new Bitmap(this.helplineRulerCtrl1.Bmp);

                    if (bSrc.Width != bmp.Width || bSrc.Height != bmp.Height)
                    {
                        Bitmap? bOld = bmp;
                        bmp = new Bitmap(bSrc.Width, bSrc.Height);
                        using Graphics g = Graphics.FromImage(bmp);
                        //g.DrawImage(bOld, 0, 0, bSrc.Width, bSrc.Height);
                        g.DrawImage(bOld, 0, 0);

                        if (bOld != null)
                            bOld.Dispose();
                        bOld = null;
                    }

                    bmpBlend = GetSurroundingRegion(bmp, bSrc, (int)this.numExtendRegion.Value);

                    if (bmp != null)
                        bmp.Dispose();
                    bmp = null;

                    if (bmpBlend != null)
                    {
                        Bitmap bmpBlend2 = new Bitmap(bmpBlend);
                        this.SetBitmap(ref this._bmpPenStrokes, ref bmpBlend2);
                        this.toolStripStatusLabel4.Text = "Custom Pic loaded";

                        SetPicToPB(bmpBlend);
                    }
                    else
                    {
                        MessageBox.Show("No surrounding region found");
                        this.SetControls(true);
                        this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                        this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                    }
                }
            }
            else if (!this.cbUseCustomReBlendPic.Checked && this.cbBlackBG.Checked)
            {
                Bitmap? bmpBlend = null;

                if (this.helplineRulerCtrl2.Bmp != null && this.helplineRulerCtrl1.Bmp != null && this.Paths != null)
                {
                    Bitmap? bmp = new(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                    using Graphics gx = Graphics.FromImage(bmp);
                    using TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp);

                    for (int i = 0; i < this.Paths.Count; i++)
                    {
                        float w = this.Paths[i].DrawWidth;
                        tb?.ResetTransform();
                        tb?.TranslateTransform(this.Paths[i].Offset.X, this.Paths[i].Offset.Y);

                        if (tb != null)
                            using (Pen pen = new Pen(tb, w))
                            {
                                pen.LineJoin = LineJoin.Round;

                                if (this.Paths[i].RoundCaps)
                                {
                                    pen.StartCap = LineCap.Round;
                                    pen.EndCap = LineCap.Round;
                                }

                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    if (this.Paths[i]?.Count() == 1)
                                    {
                                        gp.AddEllipse(this.Paths[i].Points[0].X - w, this.Paths[i].Points[0].Y - w, w * 2, w * 2);
                                        gx.FillPath(tb, gp);
                                    }
                                    else
                                    {
                                        gp.AddLines(this.Paths[i].ToArray());
                                        gx.DrawPath(pen, gp);
                                    }
                                }
                            }
                    }

                    using Bitmap bSrc = new Bitmap(this.helplineRulerCtrl1.Bmp);

                    if (bSrc.Width != bmp.Width || bSrc.Height != bmp.Height)
                    {
                        Bitmap? bOld = bmp;
                        bmp = new Bitmap(bSrc.Width, bSrc.Height);
                        using Graphics g = Graphics.FromImage(bmp);
                        //g.DrawImage(bOld, 0, 0, bSrc.Width, bSrc.Height);
                        g.DrawImage(bOld, 0, 0);

                        if (bOld != null)
                            bOld.Dispose();
                        bOld = null;
                    }

                    using Bitmap bCopy = new Bitmap(bmp);
                    bmpBlend = GetSurroundingRegion(bmp, bSrc, (int)this.numExtendRegion.Value);

                    if (bmpBlend != null)
                    {
                        this.colorDialog1.CustomColors = CustomColors;
                        if (this.cbDiffCol.Checked && this.colorDialog1.ShowDialog() == DialogResult.OK)
                        {
                            Color c = this.colorDialog1.Color;
                            CustomColors = this.colorDialog1.CustomColors;

                            GetColoredShapeBGBmp(bmpBlend, bCopy, c);
                        }
                        else
                            GetBlackShapeBGBmp(bmpBlend, bCopy);

                        Bitmap bmpBlend2 = new Bitmap(bmpBlend);
                        this.SetBitmap(ref this._bmpPenStrokes, ref bmpBlend2);
                        this.toolStripStatusLabel4.Text = "Custom Pic loaded";

                        SetPicToPB(bmpBlend);
                    }
                    else
                    {
                        MessageBox.Show("No surrounding region found");
                        this.SetControls(true);
                        this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                        this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                    }

                    if (bmp != null)
                        bmp.Dispose();
                    bmp = null;
                }
            }
            else if (this.cbUseCustomReBlendPic.Checked)
            {
                Bitmap? bmp = null;

                if (this.cbLoadFCache.Checked && Directory.Exists(this.AvoidAGrabCutCache))
                {
                    bool cancel = false;
                    string? s = Path.GetDirectoryName(this.openFileDialog1.FileName);
                    this.openFileDialog1.InitialDirectory = this.AvoidAGrabCutCache;
                    if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                            bmp = new Bitmap(img);
                    }
                    else
                        cancel = true;

                    this.openFileDialog1.InitialDirectory = s;

                    if (cancel)
                        return;
                }
                else
                {
                    if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                            bmp = new Bitmap(img);
                    }
                    else
                        return;
                }

                if (this.cbShiftImage.Checked && bmp != null)
                {
                    if (this.rbShiftMP.Checked)
                    {
                        int dx = this._destPt.X - this._sourcePt.X;
                        int dy = this._destPt.Y - this._sourcePt.Y;

                        Bitmap bC = new Bitmap(bmp.Width, bmp.Height);
                        using Graphics gx = Graphics.FromImage(bC);
                        gx.DrawImage(bmp, dx, dy);

                        Bitmap? bOld = bmp;
                        bmp = bC;
                        if (bOld != null)
                            bOld.Dispose();
                        bOld = null;
                    }
                    else if (this.rbShiftC.Checked)
                    {
                        using frmShiftPic frm = new frmShiftPic(this.helplineRulerCtrl2.Bmp, bmp);

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            int dx = (int)frm.numX.Value;
                            int dy = (int)frm.numY.Value;

                            Bitmap bC = new Bitmap(bmp.Width, bmp.Height);
                            using Graphics gx = Graphics.FromImage(bC);
                            gx.DrawImage(bmp, dx, dy);

                            Bitmap? bOld = bmp;
                            bmp = bC;
                            if (bOld != null)
                                bOld.Dispose();
                            bOld = null;
                        }
                    }
                }

                if (bmp != null)
                {
                    SetPicToPB(bmp);

                    Bitmap bmp2 = new Bitmap(bmp);
                    this.SetBitmap(ref this._bmpPenStrokes, ref bmp2);
                    this.toolStripStatusLabel4.Text = "Custom Pic loaded";

                    if (this.cbWholeRegionPic.Checked)
                    {
                        using Bitmap bSrc = new Bitmap(this.helplineRulerCtrl1.Bmp);

                        if (bSrc.Width != bmp.Width || bSrc.Height != bmp.Height)
                        {
                            Bitmap? bOld = bmp;
                            bmp = new Bitmap(bSrc.Width, bSrc.Height);
                            using Graphics g = Graphics.FromImage(bmp);
                            //g.DrawImage(bOld, 0, 0, bSrc.Width, bSrc.Height);
                            g.DrawImage(bOld, 0, 0);

                            if (bOld != null)
                                bOld.Dispose();
                            bOld = null;

                        }
                        Bitmap? bmpBlend = GetSurroundingRegion(bmp, bSrc, (int)this.numExtendRegion.Value);

                        if (bmpBlend != null)
                        {
                            Bitmap bmpBlend2 = new Bitmap(bmpBlend);
                            this.SetBitmap(ref this._bmpPenStrokes, ref bmpBlend2);
                            this.toolStripStatusLabel4.Text = "Custom Pic loaded";

                            SetPicToPB(bmpBlend);
                        }
                        else
                        {
                            MessageBox.Show("No surrounding region found");
                            this.SetControls(true);
                            this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                        }
                    }
                    else if (this.cbBlackBG.Checked)
                    {
                        using Bitmap bSrc = new Bitmap(this.helplineRulerCtrl1.Bmp);

                        if (bSrc.Width != bmp.Width || bSrc.Height != bmp.Height)
                        {
                            Bitmap? bOld = bmp;
                            bmp = new Bitmap(bSrc.Width, bSrc.Height);
                            using Graphics g = Graphics.FromImage(bmp);
                            //g.DrawImage(bOld, 0, 0, bSrc.Width, bSrc.Height);
                            g.DrawImage(bOld, 0, 0);

                            if (bOld != null)
                                bOld.Dispose();
                            bOld = null;
                        }

                        using Bitmap bCopy = new Bitmap(bmp);
                        Bitmap? bmpBlend = GetSurroundingRegion(bmp, bSrc, (int)this.numExtendRegion.Value);

                        if (bmpBlend != null)
                        {
                            this.colorDialog1.CustomColors = CustomColors;
                            if (this.cbDiffCol.Checked && this.colorDialog1.ShowDialog() == DialogResult.OK)
                            {
                                Color c = this.colorDialog1.Color;
                                CustomColors = this.colorDialog1.CustomColors;

                                GetColoredShapeBGBmp(bmpBlend, bCopy, c);
                            }
                            else
                                GetBlackShapeBGBmp(bmpBlend, bCopy);

                            Bitmap bmpBlend2 = new Bitmap(bmpBlend);
                            this.SetBitmap(ref this._bmpPenStrokes, ref bmpBlend2);
                            this.toolStripStatusLabel4.Text = "Custom Pic loaded";

                            SetPicToPB(bmpBlend);
                        }
                        else
                        {
                            MessageBox.Show("No surrounding region found");
                            this.SetControls(true);
                            this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                        }
                    }
                }

                if (bmp != null)
                    bmp.Dispose();
                bmp = null;
            }

            this._customPicLoaded = this.btnReBlend.Enabled = true;
        }

        private unsafe void GetBlackShapeBGBmp(Bitmap bmpBlend, Bitmap bmp)
        {
            int w = bmpBlend.Width;
            int h = bmpBlend.Height;

            BitmapData bmD = bmpBlend.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] > 0)
                        p[0] = p[1] = p[2] = 0;

                    p += 4;
                }
            });

            bmpBlend.UnlockBits(bmD);

            //maybe use this too
            //ExtendOutlineEx(bmpBlend, 12, true, true);

            using Graphics gx = Graphics.FromImage(bmpBlend);
            gx.DrawImage(bmp, 0, 0);
        }

        private unsafe void GetColoredShapeBGBmp(Bitmap bmpBlend, Bitmap bmp, Color c)
        {
            int w = bmpBlend.Width;
            int h = bmpBlend.Height;

            BitmapData bmD = bmpBlend.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] > 0)
                    {
                        p[0] = c.B;
                        p[1] = c.G;
                        p[2] = c.R;
                    }

                    p += 4;
                }
            });

            bmpBlend.UnlockBits(bmD);

            //if(extend > 0)
            //    ExtendOutlineEx(bmpBlend, extend, true, true);

            using Graphics gx = Graphics.FromImage(bmpBlend);
            gx.DrawImage(bmp, 0, 0);
        }

        private List<ChainCode> GetBoundary(Bitmap bmp)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, 0, false, 0, false, 0, false);
            return l;
        }

        private void ExtendOutlineEx(Bitmap bmp, int outerW, bool dontFill, bool drawPath)
        {
            List<ChainCode>? lInner = GetBoundary(bmp);

            using Bitmap? bSrc = new Bitmap(bmp.Width, bmp.Height);
            using Graphics g = Graphics.FromImage(bSrc);
            g.Clear(Color.Black);

            if (lInner?.Count > 0)
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
                                using (Graphics gx = Graphics.FromImage(bmp))
                                {
                                    gx.SmoothingMode = SmoothingMode.None;
                                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;

                                    using TextureBrush tb = new(bSrc);
                                    using Pen pen = new(tb, 1);
                                    gx.FillPath(tb, gp);
                                    gx.DrawPath(pen, gp);

                                    if (drawPath && outerW > 0)
                                    {
                                        try
                                        {
                                            using (Pen pen2 = new Pen(tb, outerW))
                                            {
                                                pen2.LineJoin = LineJoin.Round;
                                                gp.Widen(pen2);
                                                gx.DrawPath(pen2, gp);
                                            }
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(exc.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (dontFill)
                if (lInner?.Count > 0)
                {
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
                                        using (Graphics gx = Graphics.FromImage(bmp))
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
                }
        }

        private void ExtendOutlineEx(Bitmap bmp, Bitmap bSrc, int outerW, bool dontFill, bool drawPath)
        {
            List<ChainCode>? lInner = GetBoundary(bmp);

            //using Bitmap? bSrc = new Bitmap(bmp.Width, bmp.Height);
            //using Graphics g = Graphics.FromImage(bSrc);
            //g.Clear(Color.Black);

            if (lInner?.Count > 0)
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
                                using (Graphics gx = Graphics.FromImage(bmp))
                                {
                                    gx.SmoothingMode = SmoothingMode.None;
                                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;

                                    using TextureBrush tb = new(bSrc);
                                    using Pen pen = new(tb, 1);
                                    gx.FillPath(tb, gp);
                                    gx.DrawPath(pen, gp);

                                    if (drawPath && outerW > 0)
                                    {
                                        try
                                        {
                                            using (Pen pen2 = new Pen(tb, outerW))
                                            {
                                                pen2.LineJoin = LineJoin.Round;
                                                gp.Widen(pen2);
                                                gx.DrawPath(pen2, gp);
                                            }
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(exc.ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (dontFill)
                if (lInner?.Count > 0)
                {
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
                                        using (Graphics gx = Graphics.FromImage(bmp))
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
                }
        }

        private unsafe Bitmap? GetSurroundingRegion(Bitmap b, Bitmap bSrc, int extend)
        {
            Bitmap? bResult = null;

            if (b != null)
            {
                int w = b.Width;
                int h = b.Height;
                PrepareBWPic(b);

                //MorphologicalProcessing2.IMorphologicalOperation alg = new MorphologicalProcessing2.Algorithms.ConvexHull();
                //alg.BGW = null;
                //alg.ApplyGrayscale(b);
                //alg.Dispose();

                ChainFinder cf = new();
                List<ChainCode> c = cf.GetOutline(b, 0, true, 0, false, 0, false);

                for (int j = 0; j < c.Count; j++)
                {
                    List<Point> ptsCH = FindConvexHull(c[j].Coord);
                    using GraphicsPath gP2 = new();
                    gP2.AddLines(ptsCH.Select(a => new PointF(a.X, a.Y)).ToArray());
                    gP2.CloseFigure();

                    using Graphics gx = Graphics.FromImage(b);
                    gx.FillPath(Brushes.White, gP2);
                }

                bResult = new Bitmap(w, h);

                BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData bmMask = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData bW = bResult.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmMask.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* pMask = (byte*)bmMask.Scan0;
                    pMask += y * stride;
                    byte* p = (byte*)bW.Scan0;
                    p += y * stride;
                    byte* pRead = (byte*)bmSrc.Scan0;
                    pRead += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        if (pMask[0] >= 250 && pMask[1] >= 250 && pMask[2] >= 250)
                        {
                            p[0] = pRead[0];
                            p[1] = pRead[1];
                            p[2] = pRead[2];

                            p[3] = pRead[3];
                        }

                        pMask += 4;
                        p += 4;
                        pRead += 4;
                    }
                });

                b.UnlockBits(bmMask);
                bResult.UnlockBits(bW);
                bSrc.UnlockBits(bmSrc);
            }

            if (extend > 0 && bResult != null)
                ExtendOutlineEx(bResult, bSrc, extend, true, true);

            return bResult;
        }

        private unsafe void PrepareBWPic(Bitmap b)
        {
            int w = b.Width;
            int h = b.Height;

            BitmapData bD = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] == 0)
                    {
                        p[1] = p[2] = p[3] = 0;
                        p[3] = 255;
                    }
                    else
                    {
                        p[0] = p[1] = p[2] = 255;
                        p[3] = 255;
                    }
                    p += 4;
                }
            });

            b.UnlockBits(bD);
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                GetAlphaMatte.frmEdgePic frm4 = new GetAlphaMatte.frmEdgePic(this.pictureBox1.Image, this.helplineRulerCtrl1.Bmp.Size);
                frm4.Text = "Blending Src";
                frm4.ShowDialog();
            }
        }

        private void cbWholeRegionPic_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbWholeRegionPic.Checked)
            {
                this.cbBlackBG.Checked = false;
                this.label13.Enabled = this.numExtendRegion.Enabled = true;
                this.cbDiffCol.Enabled = false;
            }

            this.cbBlackBG.Enabled = !this.cbWholeRegionPic.Checked;

            if (this._customPicLoaded)
                this.btnReBlend.Enabled = this._customPicLoaded = false;
        }

        private void cbBlackBG_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbBlackBG.Checked)
            {
                this.cbWholeRegionPic.Checked = false;
                this.label13.Enabled = this.numExtendRegion.Enabled = true;
                this.cbDiffCol.Enabled = true;
            }

            this.cbWholeRegionPic.Enabled = !this.cbBlackBG.Checked;

            if (this._customPicLoaded)
                this.btnReBlend.Enabled = this._customPicLoaded = false;
        }

        private void btnColorsRGB_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap b = new Bitmap(this.helplineRulerCtrl1.Bmp);
                ColorCurves.frmColorCurves frm = new(this.helplineRulerCtrl1.Bmp, "", 0);
                this.SetControls(false);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    byte[] rr = frm.MappingsRed;
                    byte[] gg = frm.MappingsGreen;
                    byte[] bb = frm.MappingsBlue;

                    ColorCurves.fipbmp.GradColors(b, rr, gg, bb);

                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                    //double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                    //double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                    //if (multiplier >= faktor)
                    //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                    //else
                    //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    Bitmap? bC = new Bitmap(b);
                    this.SetBitmap(ref this._bmpBU, ref bC);

                    Bitmap? bD = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                    this.SetBitmap(ref this._bmpDraw, ref bD);

                    if (this._bmpBUZoomed != null)
                        this._bmpBUZoomed.Dispose();
                    this._bmpBUZoomed = null;
                    if (this._bmpBUZoomed == null && this._bmpBU != null)
                        MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);

                    //this._sourcePt = new Point(0, 0);

                    int dx = this._destPt.X - this._sourcePt.X;
                    int dy = this._destPt.Y - this._sourcePt.Y;

                    if (this._tb != null)
                        this._tb.Dispose();
                    this._tb = null;
                    SetupTB();

                    this._tb?.ResetTransform();
                    this._tb?.TranslateTransform(dx, dy);
                }

                this.SetControls(true);
            }
        }

        private void btnColorsHSL_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap b = new Bitmap(this.helplineRulerCtrl1.Bmp);
                ColorCurves.frmHSLRange frm = new(b);
                this.SetControls(false);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ColorCurves.HSLValues vals = frm.Vals;

                    ColorCurves.fipbmp.Bereich(b, vals.HueMin, vals.HueMax, vals.Hue, vals.Saturation, vals.Luminance,
                                        vals.AddSaturation, vals.AddLuminance, vals.SaturationMin, vals.SaturationMax,
                                        vals.LuminanceMin, vals.LuminanceMax, vals.DoAlpha, vals.Alpha, vals.AddAlpha,
                                                                  vals.UseRamp, vals.RampGamma);

                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                    //double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                    //double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                    //if (multiplier >= faktor)
                    //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                    //else
                    //    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    Bitmap? bC = new Bitmap(b);
                    this.SetBitmap(ref this._bmpBU, ref bC);

                    Bitmap? bD = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                    this.SetBitmap(ref this._bmpDraw, ref bD);

                    if (this._bmpBUZoomed != null)
                        this._bmpBUZoomed.Dispose();
                    this._bmpBUZoomed = null;
                    if (this._bmpBUZoomed == null && this._bmpBU != null)
                        MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);

                    //this._sourcePt = new Point(0, 0);

                    int dx = this._destPt.X - this._sourcePt.X;
                    int dy = this._destPt.Y - this._sourcePt.Y;

                    if (this._tb != null)
                        this._tb.Dispose();
                    this._tb = null;
                    SetupTB();

                    this._tb?.ResetTransform();
                    this._tb?.TranslateTransform(dx, dy);
                }

                this.SetControls(true);
            }
        }

        private void btnHSL2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                Bitmap b = new Bitmap(this.helplineRulerCtrl2.Bmp);
                ColorCurves.frmHSLRange frm = new(b);
                this.SetControls(false);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    ColorCurves.HSLValues vals = frm.Vals;

                    ColorCurves.fipbmp.Bereich(b, vals.HueMin, vals.HueMax, vals.Hue, vals.Saturation, vals.Luminance,
                                        vals.AddSaturation, vals.AddLuminance, vals.SaturationMin, vals.SaturationMax,
                                        vals.LuminanceMin, vals.LuminanceMax, vals.DoAlpha, vals.Alpha, vals.AddAlpha,
                                                                  vals.UseRamp, vals.RampGamma);

                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                    this._pic_changed = true;

                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

                    _undoOPCache?.Add(this.helplineRulerCtrl2.Bmp);

                    this.btnUndo.Enabled = _undoOPCache?.CurrentPosition > 1;
                    CheckRedoButton();

                    this.CurPath = new List<PointF>();

                    this.cmbAlg_SelectedIndexChanged(this.cmbAlg, new EventArgs());
                    this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                }

                this.SetControls(true);
            }
        }

        private void cbDiffCol_CheckedChanged(object sender, EventArgs e)
        {
            if (this._customPicLoaded)
                this.btnReBlend.Enabled = this._customPicLoaded = false;
        }

        private void numExtendRegion_ValueChanged(object sender, EventArgs e)
        {
            if (this._customPicLoaded)
                this.btnReBlend.Enabled = this._customPicLoaded = false;
        }

        private void cbUseCustomReBlendPic_CheckedChanged(object sender, EventArgs e)
        {
            if (!this.cbUseCustomReBlendPic.Checked)
            {
                this.btnReBlend.Enabled = true;
                this._customPicLoaded = false;
            }
        }

        private void btnLoadEdited_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;
                using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                    bmp = new Bitmap(img);

                Bitmap? b = new Bitmap(this.helplineRulerCtrl2.Bmp.Width, this.helplineRulerCtrl2.Bmp.Height);
                using TextureBrush tb = new TextureBrush(bmp);
                tb.WrapMode = WrapMode.TileFlipXY;
                using Graphics gx = Graphics.FromImage(b);
                gx.FillRectangle(tb, new RectangleF(0, 0, b.Width, b.Height));
                if (bmp != null)
                    bmp.Dispose();
                bmp = null;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                Bitmap? bC = new Bitmap(b);
                this.SetBitmap(ref this._bmpBU, ref bC);

                Bitmap? bD = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                this.SetBitmap(ref this._bmpDraw, ref bD);

                if (this._bmpBUZoomed != null)
                    this._bmpBUZoomed.Dispose();
                this._bmpBUZoomed = null;
                if (this._bmpBUZoomed == null && this._bmpBU != null)
                    MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);

                //this._sourcePt = new Point(0, 0);

                int dx = this._destPt.X - this._sourcePt.X;
                int dy = this._destPt.Y - this._sourcePt.Y;

                if (this._tb != null)
                    this._tb.Dispose();
                this._tb = null;
                SetupTB();

                this._tb?.ResetTransform();
                this._tb?.TranslateTransform(dx, dy);
            }
        }

        private void btnSaveHLC1_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.helplineRulerCtrl1.Bmp != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    this.helplineRulerCtrl1.Bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbShowCustPic_CheckedChanged(object sender, EventArgs e)
        {
            if (this._bmpPenStrokes != null)
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
        }

        // GrahamScan, from:
        //https://www.geeksforgeeks.org/dsa/convex-hull-using-graham-scan/
        public static List<Point> FindConvexHull(List<Point> pts)
        {
            List<Point> inputCopy = new();
            inputCopy.AddRange(pts);

            int n = inputCopy.Count;

            // Convex hull is not possible if there are fewer than 3 points
            if (n < 3)
                return new List<Point> { new Point(-1, -1) };

            // Find the point with the lowest y-coordinate (and leftmost in case of tie)
            Point p0 = inputCopy[0];
            foreach (Point p in inputCopy)
            {
                if (p.Y < p0.Y || (p.Y == p0.Y && p.X < p0.X))
                    p0 = p;
            }

            // Sort points based on polar angle with respect to the reference point p0
            inputCopy.Sort((a1, b1) => {
                int o = Orientation(p0, a1, b1);
                if (o == 0)
                    return DistSq(p0, a1).CompareTo(DistSq(p0, b1));

                return o < 0 ? -1 : 1;
            });

            // List to store the points on the convex hull
            List<Point> st = new List<Point>();

            // Process each point to build the hull
            foreach (Point pt in inputCopy)
            {
                // While last two points and current point make a non-left turn, remove the middle one
                while (st.Count > 1 && Orientation(st[st.Count - 2], st[st.Count - 1], pt) >= 0)
                {
                    st.RemoveAt(st.Count - 1);
                }
                // Add the current point to the hull
                st.Add(pt);
            }

            // If fewer than 3 points in the final hull, return [-1]
            if (st.Count < 3) return new List<Point> { new Point(-1, -1) };

            return st;
        }

        // Function to find orientation of the triplet (a, b, c)
        // Returns -1 if clockwise, 1 if counter-clockwise, 0 if collinear
        private static int Orientation(Point a, Point b, Point c)
        {
            double v = a.X * (b.Y - c.Y) +
                       b.X * (c.Y - a.Y) +
                       c.X * (a.Y - b.Y);
            if (v < 0) return -1;
            if (v > 0) return 1;
            return 0;
        }

        // Function to calculate the squared distance between two points
        static double DistSq(Point a, Point b)
        {
            return (a.X - b.X) * (a.X - b.X) +
                   (a.Y - b.Y) * (a.Y - b.Y);
        }
    }
}
