using Cache;
using ChainCodeFinder;
using GetAlphaMatte;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AvoidAGrabCutEasy
{
    public partial class frmAlphaMatte : Form
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
                if (value != null)
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
        private bool _tracking4;
        private int _rX;
        private int _rY;
        private int _eX;
        private int _eY;
        private int _rW;
        private int _rH;
        private Rectangle _rect;
        private int _eW;
        private int _eH;

        private List<Point> _points = new List<Point>();
        private List<Point> _points2 = new List<Point>();
        //private int _currentDraw;
        private Color[] _colors = new Color[4];

        private object _lockObject = new object();
        //private bool _drawImgOverlay;
        //private int _eX2f;
        //private int _eY2f;
        //private List<int>? _lastDraw;
        private int _eX2;
        private int _eY2;

        private Dictionary<int, Dictionary<int, List<List<Point>>>>? _scribbles;

        private Point _ptHLC1BG = new Point(-1, -1);
        private Bitmap? _scribblesBitmap;
        private Point _ptHLC1FG = new Point(-1, -1);
        private BitArray? _bitsBG;
        private BitArray? _bitsFG;
        private Stopwatch? _sw;
        private ClosedFormMatteOp? _cfop;
        private int _lastRunNumber;
        private List<TrimapProblemInfo> _trimapProblemInfos = new List<TrimapProblemInfo>();
        private frmInfo? _frmInfo;
        private ClosedFormMatteOp[]? _cfopArray;
        private Bitmap? _bWork;
        private List<Tuple<int, int, int, bool, List<List<Point>>>> _scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();
        private Point _ptHLC1FGBG;
        private bool _hs;
        private Point? _ptSt;
        private List<Point>? _ptPrev;

        public frmAlphaMatte()
        {
            InitializeComponent();
        }

        public frmAlphaMatte(Bitmap bmp, string basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

            //HelperFunctions.HelperFunctions.SetFormSizeBig(this);
            this.CenterToScreen();

            // Me.helplineRulerCtrl1.SetZoomOnlyByMethodCall = True

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.helplineRulerCtrl1.Bmp = new Bitmap(bmp);
                _bmpBU = new Bitmap(bmp);
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

            //this.helplineRulerCtrl2.dbPanel1.MouseDown += helplineRulerCtrl2_MouseDown;
            //this.helplineRulerCtrl2.dbPanel1.MouseMove += helplineRulerCtrl2_MouseMove;
            //this.helplineRulerCtrl2.dbPanel1.MouseUp += helplineRulerCtrl2_MouseUp;

            //this.helplineRulerCtrl2.PostPaint += helplineRulerCtrl2_Paint;

            this._dontDoZoom = true;
            this.cmbZoom.SelectedIndex = 4;
            this._dontDoZoom = false;

            this._colors[0] = Color.Black;
            this._colors[1] = Color.White;
            this._colors[2] = Color.Black;
            this._colors[3] = Color.Yellow;

            //this._currentDraw = 0;

            this.helplineRulerCtrl2.AddDefaultHelplines();
            this.helplineRulerCtrl2.ResetAllHelpLineLabelsColor();

            //while developing...
            //AvailMem.AvailMem.NoMemCheck = true;
        }

        public void SetScribbles(Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles, List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq)
        {
            this._scribbles = scribbles;
            this._scribbleSeq = scribbleSeq;
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            int eX = e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
            int eY = e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;

            this.helplineRulerCtrl1.dbPanel1.Capture = true;

            if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                this._rX = ix;
                this._rY = iy;

                this._eX = eX;
                this._eY = eY;

                if (!this.cbRefPtBG.Checked && !this.cbRefPtFG.Checked && e.Button == MouseButtons.Left)
                {
                    this._points2.Add(new Point(ix, iy));
                    this._tracking4 = true;
                }

                if (e.Button == MouseButtons.Left && this.cbRefPtBG.Checked)
                {
                    this._ptHLC1BG = new Point(ix, iy);
                    this.btnFloodBG.Enabled = true;
                    this.cbRefPtBG.Checked = false;
                }

                if (e.Button == MouseButtons.Left && this.cbRefPtFG.Checked)
                {
                    this._ptHLC1FG = new Point(ix, iy);
                    this.btnFloodFG.Enabled = true;
                    this.cbRefPtFG.Checked = false;
                }

                if (e.Button == MouseButtons.Right)
                {
                    this._ptHLC1FGBG = new Point(ix, iy);
                    this.contextMenuStrip1.Show(this.helplineRulerCtrl1.dbPanel1, e.X, e.Y);
                }

                this._tracking = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            this._eX2 = e.X;
            this._eY2 = e.Y;

            if (ix >= this.helplineRulerCtrl1.Bmp.Width)
                ix = this.helplineRulerCtrl1.Bmp.Width - 1;
            if (iy >= this.helplineRulerCtrl1.Bmp.Height)
                iy = this.helplineRulerCtrl1.Bmp.Height - 1;

            if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                if (this._tracking)
                {
                    this._rW = Math.Abs(ix - this._rX);
                    this._rH = Math.Abs(iy - this._rY);

                    this._eW = Math.Abs(e.X - this._eX - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X);
                    this._eH = Math.Abs(e.Y - this._eY - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                }

                if (this._tracking4)
                    this._points2.Add(new Point(ix, iy));

                Color c = this.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                this.toolStripStatusLabel1.Text = ix.ToString() + "; " + iy.ToString();
                this.ToolStripStatusLabel2.BackColor = c;

                if (!this.cbRedrawOnMD.Checked || (this.cbRedrawOnMD.Checked && (_tracking || _tracking4)))
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            int eX = e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
            int eY = e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;

            if (ix >= this.helplineRulerCtrl1.Bmp.Width)
                ix = this.helplineRulerCtrl1.Bmp.Width - 1;
            if (iy >= this.helplineRulerCtrl1.Bmp.Height)
                iy = this.helplineRulerCtrl1.Bmp.Height - 1;

            if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                this._rW = Math.Abs(ix - this._rX);
                this._rH = Math.Abs(iy - this._rY);

                this._rect = new Rectangle(this._rX, this._rY, this._rW, this._rH);

                if (this._tracking4)
                {
                    AddPointsToScribblePath();
                    this._points2.Clear();
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }

            this._tracking = false;
            this._tracking4 = false;
            this.helplineRulerCtrl1.dbPanel1.Capture = false;
        }

        private void AddPointsToScribblePath()
        {
            if (this._scribbles != null)
            {
                int fgbg = this.rbFG.Checked ? 1 : this.rbBG.Checked ? 0 : 3;
                int wh = (int)this.numWHScribbles.Value;

                if (!this._scribbles.ContainsKey(fgbg))
                    this._scribbles.Add(fgbg, new Dictionary<int, List<List<Point>>>());

                if (!this._scribbles[fgbg].ContainsKey(wh))
                    this._scribbles[fgbg].Add(wh, new List<List<Point>>());

                if (this._scribbleSeq == null)
                    this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                if (this._points2 != null)
                {
                    Point? ptEnd = null;

                    if (this.cbClickMode.Checked && this._points2.Count == 1)
                    {
                        Point? ptSt = this._ptSt;
                        ptEnd = this._points2[0];
                        this._points2 = GetPointsSequence(ptSt, ptEnd, wh);
                    }

                    if (this._ptPrev == null)
                        this._ptPrev = new List<Point>();

                    this._ptSt = ptEnd;

                    if (ptEnd != null)
                        this._ptPrev.Add(ptEnd.Value);

                    if (this._points2.Count > 0)
                    {
                        List<List<Point>> whPts = this._scribbles[fgbg][wh];
                        whPts.Add(new List<Point>());
                        whPts[whPts.Count - 1].AddRange(this._points2.ToArray());
                        this._scribbleSeq.Add(Tuple.Create(fgbg, wh, this._scribbles[fgbg][wh].Count - 1, false, new List<List<Point>>()));
                    }
                }
            }
        }

        private List<Point> GetPointsSequence(Point? ptSt, Point? pt, int wh, double div = 10)
        {
            List<Point> pts = new List<Point>();

            if (ptSt != null && pt != null)
            {
                double dx = pt.Value.X - ptSt.Value.X;
                double dy = pt.Value.Y - ptSt.Value.Y;

                double nrm = Math.Sqrt(dx * dx + dy * dy);

                dx /= nrm;
                dy /= nrm;

                pts.Add(ptSt.Value);

                double dist = 0;
                double dstX = Math.Max(wh / div, 1) * dx;
                double dstY = Math.Max(wh / div, 1) * dy;
                double add = Math.Sqrt(dstX * dstX + dstY * dstY);
                double lastX = ptSt.Value.X;
                double lastY = ptSt.Value.Y;

                while (dist < nrm - add)
                {
                    double x = lastX + dstX;
                    double y = lastY + dstY;

                    pts.Add(new Point((int)Math.Round(x), (int)Math.Round(y)));
                    lastX = x;
                    lastY = y;

                    dist += add;
                }

                pts.Add(pt.Value);
            }

            return pts;
        }

        private void helplineRulerCtrl1_Paint(object? sender, PaintEventArgs e)
        {
            if (this._scribbles == null)
                this._scribbles = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

            if (this._scribbleSeq != null && this._scribbleSeq.Count > 0)
            {
                foreach (Tuple<int, int, int, bool, List<List<Point>>> f in this._scribbleSeq)
                {
                    int l = f.Item1;
                    int wh = f.Item2;
                    int listNo = f.Item3;

                    if (this._scribbles.ContainsKey(l) && this._scribbles[l].ContainsKey(wh))
                    {
                        List<List<Point>> ptsList = this._scribbles[l][wh];

                        if (ptsList != null && ptsList.Count > listNo)
                        {
                            bool doRect = ptsList[listNo].Count > 1;

                            Color c = l == 0 ? Color.Black : l == 1 ? Color.White : Color.Gray;

                            if (l == 3 && this.cbHighlight.Checked)
                                c = Color.Cyan;

                            if (doRect)
                            {
                                if (f.Item4 && f.Item5 != null && f.Item5.Count > 0)
                                {
                                    List<List<Point>> pts = f.Item5;
                                    using GraphicsPath gP = new GraphicsPath();
                                    foreach (List<Point> lPt in pts)
                                    {
                                        gP.StartFigure();
                                        gP.AddLines(lPt.Select(a => new PointF(a.X, a.Y)).ToArray());
                                        gP.CloseFigure();
                                    }

                                    using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                                    gP.Transform(mx);

                                    using SolidBrush sb = new SolidBrush(c);
                                    e.Graphics.FillPath(sb, gP);
                                }
                                else
                                    foreach (Point pt in ptsList[listNo])
                                    {
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                Math.Max((int)(wh * this.helplineRulerCtrl1.Zoom), 1),
                                                Math.Max((int)(wh * this.helplineRulerCtrl1.Zoom), 1)));
                                    }
                            }
                            else
                            {
                                if (ptsList[listNo].Count > 0)
                                {
                                    Point pt = ptsList[listNo][0];
                                    using (SolidBrush sb = new SolidBrush(c))
                                        e.Graphics.FillRectangle(sb, new Rectangle(
                                            (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                            (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                            (int)(wh * this.helplineRulerCtrl1.Zoom),
                                            (int)(wh * this.helplineRulerCtrl1.Zoom)));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (this._scribbles.ContainsKey(0)) //BG
                {
                    if (this._scribbles[0] != null)
                    {
                        foreach (int wh in this._scribbles[0].Keys)
                        {
                            List<List<Point>> whPts = this._scribbles[0][wh];

                            foreach (List<Point> pts in whPts)
                            {
                                bool doRect = pts.Count > 1;
                                Color c = Color.Black;

                                if (doRect)
                                {
                                    foreach (Point pt in pts)
                                    {
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)(wh * this.helplineRulerCtrl1.Zoom),
                                                (int)(wh * this.helplineRulerCtrl1.Zoom)));
                                    }
                                }
                                else
                                {
                                    if (pts.Count > 0)
                                    {
                                        Point pt = pts[0];
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)(wh * this.helplineRulerCtrl1.Zoom),
                                                (int)(wh * this.helplineRulerCtrl1.Zoom)));
                                    }
                                }
                            }
                        }
                    }
                }

                if (this._scribbles.ContainsKey(1)) //FG
                {
                    if (this._scribbles[1] != null)
                    {
                        foreach (int wh in this._scribbles[1].Keys)
                        {
                            List<List<Point>> whPts = this._scribbles[1][wh];

                            foreach (List<Point> pts in whPts)
                            {
                                bool doRect = pts.Count > 1;
                                Color c = Color.White;

                                if (doRect)
                                {
                                    foreach (Point pt in pts)
                                    {
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)(wh * this.helplineRulerCtrl1.Zoom),
                                                (int)(wh * this.helplineRulerCtrl1.Zoom)));
                                    }
                                }
                                else
                                {
                                    if (pts.Count > 0)
                                    {
                                        Point pt = pts[0];
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)(wh * this.helplineRulerCtrl1.Zoom),
                                                (int)(wh * this.helplineRulerCtrl1.Zoom)));
                                    }
                                }
                            }
                        }
                    }
                }

                if (this._scribbles.ContainsKey(3)) //Unknown
                {
                    if (this._scribbles[3] != null)
                    {
                        foreach (int wh in this._scribbles[3].Keys)
                        {
                            List<List<Point>> whPts = this._scribbles[3][wh];

                            foreach (List<Point> pts in whPts)
                            {
                                bool doRect = pts.Count > 1;
                                Color c = Color.Gray;

                                if (this.cbHighlight.Checked)
                                    c = Color.Cyan;

                                if (doRect)
                                {
                                    foreach (Point pt in pts)
                                    {
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)(wh * this.helplineRulerCtrl1.Zoom),
                                                (int)(wh * this.helplineRulerCtrl1.Zoom)));
                                    }
                                }
                                else
                                {
                                    if (pts.Count > 0)
                                    {
                                        Point pt = pts[0];
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)(wh * this.helplineRulerCtrl1.Zoom),
                                                (int)(wh * this.helplineRulerCtrl1.Zoom)));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this._points2.Count > 0)
            {
                Color c = this.rbFG.Checked ? Color.White : this.rbBG.Checked ? Color.Black : Color.Gray;

                if (this.cbHighlight.Checked && !this.rbFG.Checked && !this.rbBG.Checked)
                    c = Color.Lime;

                using (GraphicsPath gp = new GraphicsPath())
                {
                    if (this._points2.Count > 1)
                    {
                        foreach (Point pt in this._points2)
                        {
                            //using (SolidBrush sb = new SolidBrush(c))
                            using (HatchBrush sb = new HatchBrush(HatchStyle.Cross, c))
                                e.Graphics.FillRectangle(sb, new Rectangle(
                                    (int)((int)(pt.X - (int)this.numWHScribbles.Value / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                    (int)((int)(pt.Y - (int)this.numWHScribbles.Value / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                    (int)((int)this.numWHScribbles.Value * this.helplineRulerCtrl1.Zoom),
                                    (int)((int)this.numWHScribbles.Value * this.helplineRulerCtrl1.Zoom)));
                        }
                    }
                }
            }

            if (this.cbOverlay.Checked && this._scribbles.Count > 0)
            {
                HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl1.dbPanel1;

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = 0.75F;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);
                    e.Graphics.DrawImage(this.helplineRulerCtrl1.Bmp,
                        new Rectangle(0, 0, pz.ClientRectangle.Width, pz.ClientRectangle.Height),
                        -pz.AutoScrollPosition.X / this.helplineRulerCtrl1.Zoom,
                            -pz.AutoScrollPosition.Y / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Width / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Height / this.helplineRulerCtrl1.Zoom, GraphicsUnit.Pixel, ia);
                }
            }

            int wh2 = (int)this.numWHScribbles.Value;

            using (SolidBrush sb = new SolidBrush(Color.FromArgb(95, 255, 0, 0)))
                e.Graphics.FillRectangle(sb, new RectangleF(
                    this._eX2 - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                    this._eY2 - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                    (float)(wh2 * this.helplineRulerCtrl1.Zoom),
                    (float)(wh2 * this.helplineRulerCtrl1.Zoom)));

            if (this._ptHLC1BG.X > -1 && this._ptHLC1BG.Y > -1)
            {
                e.Graphics.DrawLine(Pens.Cyan, ((this._ptHLC1BG.X * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1BG.Y * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                    ((this._ptHLC1BG.X * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1BG.Y * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                e.Graphics.DrawLine(Pens.Cyan, ((this._ptHLC1BG.X * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1BG.Y * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                    ((this._ptHLC1BG.X * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1BG.Y * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
            }

            if (this._ptHLC1FG.X > -1 && this._ptHLC1FG.Y > -1)
            {
                e.Graphics.DrawLine(Pens.Red, ((this._ptHLC1FG.X * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1FG.Y * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                    ((this._ptHLC1FG.X * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1FG.Y * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                e.Graphics.DrawLine(Pens.Red, ((this._ptHLC1FG.X * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1FG.Y * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                    ((this._ptHLC1FG.X * this.helplineRulerCtrl1.Zoom - 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                    ((this._ptHLC1FG.Y * this.helplineRulerCtrl1.Zoom + 5)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
            }
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmCFM");
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
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
                                        b1 = new Bitmap((Bitmap)img.Clone());
                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                        b2 = new Bitmap((Bitmap)img.Clone());
                                        this.SetBitmap(ref this._bmpBU, ref b2);
                                    }
                                    else
                                        throw new Exception();
                                }

                                if (this.helplineRulerCtrl1.Bmp != null)
                                {
                                    this.numMaxSize.Value = (decimal)Math.Max(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

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
            MessageBox.Show("The goal is to achieve results as good as with a GrabCut, but only with the first (GMM_probabilities) estimation, to reduce the memory footprint. So keep \"QuickEstimation\" checked and play around with the other parameters.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache?.Processing == false)
            {
                Bitmap? bOut = _undoOPCache?.DoUndoAll();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                    this._pic_changed = false;
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
                else
                    MessageBox.Show("Error while resetting.");
            }

            this.numMaxSize.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //    if (_undoOPCache != null)
            //    {
            //        this.btnRedo.Enabled = false;
            //        this.btnRedo.Refresh();

            //        Bitmap bOut = _undoOPCache?.DoRedo();

            //        if (bOut != null)
            //        {
            //            this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");

            //            this._pic_changed = true;
            //            // Me.helplineRulerCtrl1.CalculateZoom()

            //            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            //            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            //            // Me.helplineRulerCtrl1.dbPanel1.Invalidate()

            //            if (_undoOPCache?.CurrentPosition > 1)
            //            {
            //                this.btnReset.Enabled = true;
            //                this.cbRectMode.Enabled = false;
            //            }
            //            else
            //                this.btnReset.Enabled = false;

            //            if (_undoOPCache?.CurrentPosition < _undoOPCache?.Count)
            //                this.btnRedo.Enabled = true;
            //            else
            //                this.btnRedo.Enabled = false;
            //        }
            //        this.helplineRulerCtrl1.dbPanel1.Invalidate();
            //    }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

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

                            // SetHRControlVars();

                            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                            this.helplineRulerCtrl1.dbPanel1.Invalidate();

                            _undoOPCache?.Reset(false);

                            //if (_undoOPCache?.Count > 1)
                            //    this.btnRedo.Enabled = true;
                            //else
                            //    this.btnRedo.Enabled = false;
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
                //this.btnReset2.PerformClick();
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
                    this.splitContainer1.BringToFront();
                    this.statusStrip1.SendToBack();

                    this.panel1.BringToFront();

                    this.panel1.AutoScroll = false;
                }
                else
                {
                    this._doubleClicked = false;
                    this.panel3.Dock = DockStyle.None;
                    this.panel1.Dock = DockStyle.Top;

                    this.panel3.Width = this.splitContainer1.Width = this.panel1.Width = this.ClientSize.Width;

                    this._isHoveringVertically = false;
                    this.panel1.Height = this._openPanelHeight;

                    this.statusStrip1.SendToBack();
                    this.panel1.SendToBack();
                    this.splitContainer1.BringToFront();

                    this.panel1.AutoScroll = true;
                    this.panel1.BringToFront();
                    this.splitContainer1.BringToFront();

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
                    this.splitContainer1.BringToFront();
                    this.statusStrip1.SendToBack();
                    this.panel1.Height = 24;
                }
                else
                {
                    this.panel3.Dock = DockStyle.Top;
                    this.panel1.Dock = DockStyle.None;
                    this._isHoveringVertically = true;
                    this.panel3.SendToBack();
                    this.splitContainer1.BringToFront();
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

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBGColor.Checked)
                this.helplineRulerCtrl2.dbPanel1.BackColor = this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl2.dbPanel1.BackColor = this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
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

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
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

            if (!e.Cancel)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();
                if (this.pictureBox1.Image != null)
                    this.pictureBox1.Image.Dispose();

                if (this._cfop != null)
                {
                    this._cfop.ShowProgess -= Cfop_UpdateProgress;
                    this._cfop.ShowInfo -= _cfop_ShowInfo;
                    this._cfop.Dispose();
                }
            }
        }

        private void btnRemLastScribbles_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this._points2.Clear();

                if (this.cbLastDrawn.Checked)
                {
                    int fg = this.rbFG.Checked ? 1 : this.rbBG.Checked ? 0 : 3;
                    int wh = (int)this.numWHScribbles.Value;

                    if (this._scribbles != null && this._scribbles.ContainsKey(fg) && this._scribbles[fg].ContainsKey(wh))
                    {
                        List<List<Point>> j = this._scribbles[fg][wh];

                        if (j != null && j.Count > 0)
                            j.RemoveAt(j.Count - 1);

                        if (this._scribbleSeq != null && j != null)
                        {
                            IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> l = this._scribbleSeq.Where(a => a.Item1 == fg);
                            if (l != null && l.Count() > 0)
                            {
                                IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> whL = l.Where(a => a.Item2 == wh);

                                if (whL != null && whL.Count() > 0)
                                {
                                    IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> listL = whL.Where(a => a.Item3 == j.Count);

                                    if (listL != null && listL.Count() > 0)
                                    {
                                        int indxt = this._scribbleSeq.IndexOf(listL.First());

                                        for (int j4 = indxt + 1; j4 < this._scribbleSeq.Count; j4++)
                                        {
                                            if (this._scribbleSeq[j4].Item1 == fg && this._scribbleSeq[j4].Item2 == wh)
                                                this._scribbleSeq[j4] = Tuple.Create(fg, wh, this._scribbleSeq[j4].Item3 - 1, false, new List<List<Point>>());
                                        }

                                        this._scribbleSeq.Remove(listL.First());
                                    }
                                }
                            }
                        }

                        //if (this._scribbleSeq != null)
                        //    this._scribbleSeq.RemoveAt(this._scribbleSeq.Count - 1);
                    }

                    if (this._ptPrev != null && this._ptPrev.Count > 0)
                    {
                        this._ptPrev.RemoveAt(this._ptPrev.Count - 1);
                        this._ptSt = this._ptPrev[this._ptPrev.Count - 1];
                    }
                    else
                        this._ptSt = null;
                }
                else
                    using (frmLastScribbles frm = new frmLastScribbles(this.helplineRulerCtrl1.Bmp, this._scribbles, this._scribbleSeq))
                        frm.ShowDialog();

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnClearScribbles_Click(object sender, EventArgs e)
        {
            this._points2.Clear();
            if (this._scribbles != null)
                this._scribbles.Clear();
            if (this._scribbleSeq != null)
                this._scribbleSeq.Clear();
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnLoadScribbles_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Scribbles-Files (*.tgScribbles)|*.tgScribbles";
            this.openFileDialog1.FileName = "File1.tgScribbles";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SavedScribbles? f = DeserializeF(this.openFileDialog1.FileName);

                if (f != null)
                {
                    this._scribbles = f.ToDictionary();

                    double resPic = 1.0;

                    if (f.BaseSize != null)
                        resPic = (double)Math.Max(f.BaseSize.Value.Width, f.BaseSize.Value.Height) /
                            (double)Math.Max(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                    if (this._scribbles != null && resPic != 1.0)
                    {
                        Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles2 = ResizeAllScribbles(this._scribbles, resPic);
                        this._scribbles = scribbles2;
                        List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = ResizeScribbleSeq(this._scribbleSeq, resPic, true);
                        this._scribbleSeq = scribbleSeq2;
                    }

                    if (f.Bmp != null && this.cbLSBmp.Checked)
                    {
                        Bitmap? b = ConvertFromBase64(f.Bmp);

                        if (b != null)
                        {
                            this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");
                            _undoOPCache?.Add(b);

                            this._pic_changed = true;

                            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                                (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                                (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                            this.cmbZoom_SelectedIndexChanged(this.cmbZoom, new EventArgs());
                        }
                    }

                    if (f.ScribbleSequence != null)
                        this._scribbleSeq = f.ScribbleSequence;

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    f.Dispose();
                }
            }
        }

        private void btnSaveScribbles_Click(object sender, EventArgs e)
        {
            if (this._scribbles != null)
            {
                SavedScribbles f = new SavedScribbles();

                if (this.helplineRulerCtrl1.Bmp != null)
                    f.BaseSize = this.helplineRulerCtrl1.Bmp.Size;

                if (this._scribbles.ContainsKey(0)) //BG
                {
                    if (this._scribbles[0] != null)
                    {
                        f.BGSizes = this._scribbles[0].Keys.ToArray();
                        f.BGPoints = new Point[f.BGSizes.Length][][];
                        int i = 0;

                        foreach (int wh in this._scribbles[0].Keys)
                        {
                            f.BGPoints[i] = new Point[this._scribbles[0][wh].Count][];

                            List<List<Point>> whPts = this._scribbles[0][wh];
                            int j = 0;

                            foreach (List<Point> pts in whPts)
                            {
                                f.BGPoints[i][j] = pts.ToArray();
                                j++;
                            }

                            i++;
                        }
                    }
                }

                if (this._scribbles.ContainsKey(1)) //FG
                {
                    if (this._scribbles[1] != null)
                    {
                        f.FGSizes = this._scribbles[1].Keys.ToArray();
                        f.FGPoints = new Point[f.FGSizes.Length][][];
                        int i = 0;

                        foreach (int wh in this._scribbles[1].Keys)
                        {
                            f.FGPoints[i] = new Point[this._scribbles[1][wh].Count][];

                            List<List<Point>> whPts = this._scribbles[1][wh];
                            int j = 0;

                            foreach (List<Point> pts in whPts)
                            {
                                f.FGPoints[i][j] = pts.ToArray();
                                j++;
                            }

                            i++;
                        }
                    }
                }

                if (this._scribbles.ContainsKey(3)) //Unknown
                {
                    if (this._scribbles[3] != null)
                    {
                        f.UknwnSizes = this._scribbles[3].Keys.ToArray();
                        f.UknwnPoints = new Point[f.UknwnSizes.Length][][];
                        int i = 0;

                        foreach (int wh in this._scribbles[3].Keys)
                        {
                            f.UknwnPoints[i] = new Point[this._scribbles[3][wh].Count][];

                            List<List<Point>> whPts = this._scribbles[3][wh];
                            int j = 0;

                            foreach (List<Point> pts in whPts)
                            {
                                f.UknwnPoints[i][j] = pts.ToArray();
                                j++;
                            }

                            i++;
                        }
                    }
                }

                if (this.helplineRulerCtrl1.Bmp != null && this.cbLSBmp.Checked)
                {
                    string? b64 = ConvertToBase64(this.helplineRulerCtrl1.Bmp);
                    if (b64 != null)
                        f.Bmp = b64;
                }

                if (this._scribbleSeq != null)
                    f.ScribbleSequence = this._scribbleSeq;

                WriteToFile(f);

                f.Dispose();
            }
        }

        private string? ConvertToBase64(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.GetBuffer());
            }
        }

        private Bitmap? ConvertFromBase64(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            Bitmap? bmp = null;
            Image? img = null;
            using (MemoryStream ms = new MemoryStream(bytes))
                img = Image.FromStream(ms);

            bmp = new Bitmap(img);
            img.Dispose();
            img = null;
            return bmp;
        }

        private void WriteToFile(SavedScribbles f)
        {
            this.saveFileDialog1.Filter = "Scribbles-Files (*.tgScribbles)|*.tgScribbles";
            this.saveFileDialog1.FileName = "File1.tgScribbles";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                SerializeF(f, this.saveFileDialog1.FileName);
        }

        private bool SerializeF(SavedScribbles f, string FileName)
        {
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

                using FileStream createStream = File.Create(FileName);
                JsonSerializer.Serialize(createStream, f, options);
            }
            catch (Exception ex)
            {
                bError = true;
                Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
            }

            return !bError;
        }

        private SavedScribbles? DeserializeF(string fileName)
        {
            Stream? stream = null;
            SavedScribbles? f = null;

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
                f = JsonSerializer.Deserialize<SavedScribbles>(stream, options);


            }
            catch (Exception ex)
            {
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

            return f;
        }

        private Dictionary<int, Dictionary<int, List<List<Point>>>> ResizeAllScribbles(Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles, double resWC)
        {
            Dictionary<int, Dictionary<int, List<List<Point>>>> res = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

            if (scribbles != null && resWC != 0)
                foreach (int j in scribbles.Keys)
                {
                    res.Add(j, new Dictionary<int, List<List<Point>>>());

                    Dictionary<int, List<List<Point>>> ptsWH = scribbles[j];

                    if (ptsWH != null)
                        foreach (int wh in ptsWH.Keys)
                        {
                            int newWH = (int)Math.Max(wh / resWC, 1);

                            if (res[j].ContainsKey(newWH))
                            {
                                List<List<Point>> ja = ptsWH[wh];

                                if (ja != null)
                                    foreach (List<Point> pts in ja)
                                    {
                                        List<Point> newPts = new List<Point>();
                                        Point[] transPts = pts.Select(a => new Point((int)(a.X / resWC), (int)(a.Y / resWC))).ToArray();
                                        newPts.AddRange(transPts);

                                        res[j][newWH].Add(newPts);
                                    }
                            }
                            else
                            {
                                res[j].Add(newWH, new List<List<Point>>());

                                List<List<Point>> ja = ptsWH[wh];

                                if (ja != null)
                                    foreach (List<Point> pts in ja)
                                    {
                                        List<Point> newPts = new List<Point>();
                                        Point[] transPts = pts.Select(a => new Point((int)(a.X / resWC), (int)(a.Y / resWC))).ToArray();
                                        newPts.AddRange(transPts);

                                        res[j][newWH].Add(newPts);
                                    }
                            }
                        }
                }

            return res;
        }

        private List<Tuple<int, int, int, bool, List<List<Point>>>> ResizeScribbleSeq(List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq, double resWC, bool verify)
        {
            List<Tuple<int, int, int, bool, List<List<Point>>>> res = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

            if (scribbleSeq != null)
                for (int i = 0; i < scribbleSeq.Count; i++)
                    res.Add(Tuple.Create(scribbleSeq[i].Item1, (int)(scribbleSeq[i].Item2 / resWC), scribbleSeq[i].Item3, false, new List<List<Point>>()));

            if (verify)
            {

            }

            return res;
        }


        private unsafe void btnFloodBG_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this._scribblesBitmap == null)
                    this._scribblesBitmap = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                if (this._bitsBG == null)
                    this._bitsBG = new BitArray(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height, false);

                Bitmap? bOld = this._scribblesBitmap;
                this._scribblesBitmap = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                if (bOld != null)
                    bOld.Dispose();
                bOld = null;

                this._bitsBG.SetAll(false);

                Bitmap? b = this._scribblesBitmap;

                Point ptSt = this._ptHLC1BG;

                if (ptSt.X != -1 && ptSt.Y != -1)
                {
                    Color startColor = b.GetPixel(ptSt.X, ptSt.Y);
                    Color replaceColor = Color.Black;

                    //first, draw the unknown curve(s)
                    using (Graphics gx = Graphics.FromImage(b))
                    {
                        if (this._scribbles != null)
                            if (this._scribbles.ContainsKey(3)) //Unknown
                            {
                                if (this._scribbles[3] != null)
                                {
                                    foreach (int wh in this._scribbles[3].Keys)
                                    {
                                        List<List<Point>> whPts = this._scribbles[3][wh];

                                        foreach (List<Point> pts in whPts)
                                        {
                                            bool doRect = pts.Count > 1;
                                            Color c = Color.Gray;

                                            List<Point> pts2 = pts.Distinct().ToList();

                                            if (doRect)
                                            {
                                                foreach (Point pt in pts2)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                        gx.FillRectangle(sb, new Rectangle(
                                                            (int)(pt.X - wh / 2),
                                                            (int)(pt.Y - wh / 2),
                                                            (int)wh,
                                                            (int)wh));
                                                    using (Pen pen = new Pen(c, 1))
                                                        gx.DrawRectangle(pen, new Rectangle(
                                                            (int)(pt.X - wh / 2),
                                                            (int)(pt.Y - wh / 2),
                                                            (int)wh,
                                                            (int)wh));
                                                }
                                            }
                                            else
                                            {
                                                Point pt = pts2[0];
                                                using (SolidBrush sb = new SolidBrush(c))
                                                    gx.FillRectangle(sb, new Rectangle(
                                                        (int)(pt.X - wh / 2),
                                                        (int)(pt.Y - wh / 2),
                                                        (int)wh,
                                                        (int)wh));
                                                using (Pen pen = new Pen(c, 1))
                                                    gx.DrawRectangle(pen, new Rectangle(
                                                        (int)(pt.X - wh / 2),
                                                        (int)(pt.Y - wh / 2),
                                                        (int)wh,
                                                        (int)wh));
                                            }
                                        }
                                    }
                                }
                            }
                    }

                    FloodFillMethods.floodfill(b, ptSt.X, ptSt.Y, 125, startColor, replaceColor,
                        Int32.MaxValue, false, false, 1.0, false, false);

                    List<Point> ll = new List<Point>();

                    int w = b.Width;
                    int h = b.Height;

                    BitmapData bmD = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    int stride = bmD.Stride;

                    byte* p = (byte*)bmD.Scan0;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            if (p[3] == 255 && p[0] == 0 && !this._bitsBG.Get(y * w + x))
                            {
                                ll.Add(new Point(x, y));
                                this._bitsBG.Set(y * w + x, true);
                            }

                            p += 4;
                        }
                    }

                    b.UnlockBits(bmD);

                    if (this._scribbles == null)
                        this._scribbles = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

                    if (!this._scribbles.ContainsKey(0))
                        this._scribbles.Add(0, new Dictionary<int, List<List<Point>>>());

                    if (!this._scribbles[0].ContainsKey(3))
                        this._scribbles[0].Add(3, new List<List<Point>>());

                    if (this._scribbleSeq == null)
                        this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                    this._scribbles[0][3].Add(ll.Distinct().ToList());
                    this._scribbleSeq.Add(Tuple.Create(0, 3, this._scribbles[0][3].Count - 1, true, GetBoundariesForScribbleFill(this._scribbles[0][3][this._scribbles[0][3].Count - 1], w, h)));

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private List<List<Point>> GetBoundariesForScribbleFill(List<Point> points, int w, int h)
        {
            List<List<Point>> res = new List<List<Point>>();

            using Bitmap bmp = new Bitmap(w, h);
            using Graphics gx = Graphics.FromImage(bmp);
            int wh = 3;
            foreach (Point pt in points)
            {
                gx.FillRectangle(Brushes.Black, new Rectangle(
                                            (int)(int)(pt.X - wh / 2),
                                            (int)(int)(pt.Y - wh / 2),
                                            (int)wh,
                                            (int)wh));
                using Pen pen = new(Color.Black, 2);
                gx.DrawRectangle(pen, new Rectangle(
                                            (int)(int)(pt.X - wh / 2),
                                            (int)(int)(pt.Y - wh / 2),
                                            (int)wh,
                                            (int)wh));
            }

            List<ChainCode>? c = this.GetBoundary(bmp);
            if (c != null)
            {
                foreach (ChainCode cc in c)
                {
                    List<Point> l = new List<Point>();
                    l.AddRange(cc.Coord.ToArray());
                    res.Add(l);
                }
            }

            return res;
        }

        private List<ChainCode>? GetBoundary(Bitmap? bmp)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, 0, false, 0, false, 0, false);
            return l;
        }

        private unsafe void btnFloodFG_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this._scribblesBitmap == null)
                    this._scribblesBitmap = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                if (this._bitsFG == null)
                    this._bitsFG = new BitArray(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height, false);

                Bitmap? bOld = this._scribblesBitmap;
                this._scribblesBitmap = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                if (bOld != null)
                    bOld.Dispose();
                bOld = null;

                this._bitsFG.SetAll(false);

                Bitmap? b = this._scribblesBitmap;

                Point ptSt = this._ptHLC1FG;
                if (ptSt.X != -1 && ptSt.Y != -1)
                {
                    Color startColor = b.GetPixel(ptSt.X, ptSt.Y);
                    Color replaceColor = Color.White;

                    //first, draw the unknown curve(s)
                    using (Graphics gx = Graphics.FromImage(b))
                    {
                        if (this._scribbles != null)
                            if (this._scribbles.ContainsKey(3)) //Unknown
                            {
                                if (this._scribbles[3] != null)
                                {
                                    foreach (int wh in this._scribbles[3].Keys)
                                    {
                                        List<List<Point>> whPts = this._scribbles[3][wh];

                                        foreach (List<Point> pts in whPts)
                                        {
                                            bool doRect = pts.Count > 1;
                                            Color c = Color.Gray;

                                            List<Point> pts2 = pts.Distinct().ToList();

                                            if (doRect)
                                            {
                                                foreach (Point pt in pts2)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                        gx.FillRectangle(sb, new Rectangle(
                                                            (int)(pt.X - wh / 2),
                                                            (int)(pt.Y - wh / 2),
                                                            (int)wh,
                                                            (int)wh));
                                                    using (Pen pen = new Pen(c, 1))
                                                        gx.DrawRectangle(pen, new Rectangle(
                                                            (int)(pt.X - wh / 2),
                                                            (int)(pt.Y - wh / 2),
                                                            (int)wh,
                                                            (int)wh));
                                                }
                                            }
                                            else
                                            {
                                                Point pt = pts2[0];
                                                using (SolidBrush sb = new SolidBrush(c))
                                                    gx.FillRectangle(sb, new Rectangle(
                                                        (int)(pt.X - wh / 2),
                                                        (int)(pt.Y - wh / 2),
                                                        (int)wh,
                                                        (int)wh));
                                                using (Pen pen = new Pen(c, 1))
                                                    gx.DrawRectangle(pen, new Rectangle(
                                                        (int)(pt.X - wh / 2),
                                                        (int)(pt.Y - wh / 2),
                                                        (int)wh,
                                                        (int)wh));
                                            }
                                        }
                                    }
                                }
                            }
                    }

                    FloodFillMethods.floodfill(b, ptSt.X, ptSt.Y, 125, startColor, replaceColor,
                        Int32.MaxValue, false, false, 1.0, false, false);

                    List<Point> ll = new List<Point>();

                    int w = b.Width;
                    int h = b.Height;

                    BitmapData bmD = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    int stride = bmD.Stride;

                    byte* p = (byte*)bmD.Scan0;

                    for (int y = 0; y < h; y++)
                    {
                        for (int x = 0; x < w; x++)
                        {
                            if (p[3] == 255 && p[0] == 255 && !this._bitsFG.Get(y * w + x))
                            {
                                ll.Add(new Point(x, y));
                                this._bitsFG.Set(y * w + x, true);
                            }

                            p += 4;
                        }
                    }

                    b.UnlockBits(bmD);

                    //cleanup
                    if (this._scribbles == null)
                        this._scribbles = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

                    if (!this._scribbles.ContainsKey(1))
                        this._scribbles.Add(1, new Dictionary<int, List<List<Point>>>());

                    if (!this._scribbles[1].ContainsKey(3))
                        this._scribbles[1].Add(3, new List<List<Point>>());

                    if (this._scribbleSeq == null)
                        this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                    this._scribbles[1][3].Add(ll.Distinct().ToList());
                    this._scribbleSeq.Add(Tuple.Create(1, 3, this._scribbles[1][3].Count - 1, true, GetBoundariesForScribbleFill(this._scribbles[1][3][this._scribbles[1][3].Count - 1], w, h)));

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private void cbRefPtBG_CheckedChanged(object sender, EventArgs e)
        {
            this.cbRefPtFG.Checked = false;
        }

        private void cbRefPtFG_CheckedChanged(object sender, EventArgs e)
        {
            this.cbRefPtBG.Checked = false;
        }

        private void btnChaincode_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.CachePathAddition != null)
            {
                //the idea is to get a rich derivative pic of hlc1.Bmp
                //replace all black by transp and get the chains
                using (Bitmap? b = new Bitmap(this.helplineRulerCtrl1.Bmp))
                {
                    using (frmChainCode frm = new frmChainCode(b, this.CachePathAddition))
                    {
                        frm.SetupCache();
                        frm.numWHScribbles.Value = this.numWHScribbles.Value;
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            int wh = (int)frm.numWHScribbles.Value;

                            //GraphicsPath? gP = null;

                            //if(frm.SelectedPath != null)
                            //    gP = (GraphicsPath?)frm.SelectedPath.Clone();

                            //if(gP != null && gP.PointCount > 1)
                            //{

                            //}

                            List<ChainCode>? c = frm.SelectedChains;

                            if (c != null)
                            {
                                if (this._scribbles == null)
                                    this._scribbles = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

                                if (!this._scribbles.ContainsKey(3))
                                    this._scribbles.Add(3, new Dictionary<int, List<List<Point>>>());

                                if (this._scribbleSeq == null)
                                    this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                                foreach (ChainCode cc in c)
                                {
                                    List<Point> pts = cc.Coord;

                                    if (pts.Count > 0)
                                    {
                                        if (!this._scribbles[3].ContainsKey(wh))
                                            this._scribbles[3].Add(wh, new List<List<Point>>());

                                        List<Point> points = new List<Point>();
                                        points.AddRange(pts.Distinct().ToArray());
                                        this._scribbles[3][wh].Add(points);
                                        this._scribbleSeq.Add(Tuple.Create(3, wh, this._scribbles[3][wh].Count - 1, false, new List<List<Point>>()));
                                    }
                                }
                            }

                            this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        }
                    }
                }
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null && this.pictureBox1.Image != null && this._bWork != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                this.btnGo.Text = "Cancel";
                this.btnGo.Enabled = true;

                this.numSleep.Enabled = this.label2.Enabled = true;

                if (_sw == null)
                    _sw = new Stopwatch();
                _sw.Reset();
                _sw.Start();

                this.btnOK.Enabled = this.btnCancel.Enabled = false;
                this._hs = this.cbHalfSize.Checked;

                Bitmap bWork = new Bitmap(this._bWork);
                Image img = this.pictureBox1.Image;
                Bitmap trWork = new Bitmap(img);

                ClosedFormMatteOp cfop = new ClosedFormMatteOp(bWork, trWork);
                BlendParameters bParam = new BlendParameters();
                bParam.MaxIterations = (int)this.numMaxRestarts.Value; //for GaussSeidel set this to 10000 or so
                bParam.InnerIterations = 35; //maybe 25 will do
                bParam.DesiredMaxLinearError = (double)this.numError.Value;
                bParam.Sleep = this.numSleep.Value > 0 ? true : false;
                bParam.SleepAmount = (int)this.numSleep.Value;
                bParam.BGW = this.backgroundWorker1;
                cfop.BlendParameters = bParam;

                this._cfop = cfop;
                this._cfop.ShowProgess += Cfop_UpdateProgress;
                this._cfop.ShowInfo += _cfop_ShowInfo;

                bool scalesPics = this.cbSlices.Checked;
                int scales = scalesPics ? rb4.Checked ? 4 : 16 : 0;
                int overlap = 32;
                bool interpolated = this.cbInterpolated.Checked;
                bool forceSerial = this.cbForceSerial.Checked;
                bool group = false;
                int groupAmountX = scalesPics ? 1 : 0; //we dont use grouping, so set it simply to 1
                int groupAmountY = scalesPics ? 1 : 0;
                int maxSize = bWork.Width * bWork.Height * 2;
                bool trySingleTile = scalesPics ? false : this.cbHalfSize.Checked ? true : false;
                bool verifyTrimaps = false;

                this.backgroundWorker1.RunWorkerAsync(new object[] { 1 /* GMRES_r; 0 is GaussSeidel */, scalesPics, scales, overlap,
                                interpolated, forceSerial, group, groupAmountX, groupAmountY, maxSize, bWork, trWork,
                                trySingleTile, verifyTrimaps });
            }
        }

        private void _cfop_ShowInfo(object? sender, string e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    this.toolStripStatusLabel4.Text = e;
                    if (e.StartsWith("pic "))
                        this.toolStripStatusLabel1.Text = e;
                    //if (e.StartsWith("outer pic-amount"))
                    //    this.label13.Text = e;
                    if (e.StartsWith("picOuter "))
                        this.toolStripStatusLabel1.Text = e;
                }));
            else
            {
                this.toolStripStatusLabel4.Text = e;
                if (e.StartsWith("pic "))
                    this.toolStripStatusLabel1.Text = e;
                //if (e.StartsWith("outer pic-amount"))
                //    this.label13.Text = e;
                if (e.StartsWith("picOuter "))
                    this.toolStripStatusLabel1.Text = e;
            }
        }

        private void Cfop_UpdateProgress(object? sender, ProgressEventArgs e)
        {
            this.backgroundWorker1.ReportProgress((int)(e.CurrentProgress / e.ImgWidthHeight * 100));
        }

        private void btnCache_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Cache.IsActive)
            {
                string folder = _undoOPCache.GetCachePath();

                using (frmCachedPictures frm = new frmCachedPictures(folder))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (frm.cbLoadTo.Checked)
                        {
                            bool hlc1 = frm.rbHlc1.Checked || frm.rbHlc12.Checked;
                            bool hlc2 = frm.rbHlc2.Checked || frm.rbHlc12.Checked;

                            if (hlc1)
                            {
                                Bitmap? bmp = null;
                                if (frm.listBox1.SelectedItem != null)
                                {
                                    string? d = frm.listBox1.SelectedItem.ToString();
                                    if (d != null)
                                    {
                                        FileInfo fi = new FileInfo(Path.Combine(folder, d));

                                        using (Image img = Image.FromFile(fi.FullName))
                                            bmp = new Bitmap(img);

                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                                        Bitmap bC = new Bitmap(bmp);
                                        this.SetBitmap(ref this._bmpBU, ref bC);
                                    }
                                }
                            }

                            if (hlc2)
                            {
                                Bitmap? bmp = null;
                                string? d = frm.listBox1.SelectedItem?.ToString();
                                if (d != null)
                                {
                                    FileInfo fi = new FileInfo(Path.Combine(folder, d));

                                    using (Image img = Image.FromFile(fi.FullName))
                                        bmp = new Bitmap(img);

                                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                                    //Bitmap bC = new Bitmap(bmp);
                                    //this.SetBitmap(ref this._b4Copy, ref bC);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void cmbZoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible && this.helplineRulerCtrl1.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(cmbZoom.SelectedItem?.ToString());
                this.helplineRulerCtrl1.Enabled = true;
                if (this.cmbZoom.SelectedIndex < 2)
                    this.helplineRulerCtrl1.ZoomSetManually = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
            if (this.Visible && this.helplineRulerCtrl2.Bmp != null && !this._dontDoZoom)
            {
                this.helplineRulerCtrl2.Enabled = false;
                this.helplineRulerCtrl2.Refresh();
                this.helplineRulerCtrl2.SetZoom(cmbZoom.SelectedItem?.ToString());
                this.helplineRulerCtrl2.Enabled = true;
                if (this.cmbZoom.SelectedIndex < 2)
                    this.helplineRulerCtrl2.ZoomSetManually = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }
        }

        private void btnGenerateTrimap_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                //closedFormMatte or bayesian Matte
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                this.btnGenerateTrimap.Text = "Cancel";
                this.btnGenerateTrimap.Enabled = true;

                this.numSleep.Enabled = this.label2.Enabled = true;

                this.btnOK.Enabled = this.btnCancel.Enabled = false;

                Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);

                double res = CheckWidthHeight(bWork, true, (double)this.numMaxSize.Value);
                this.toolStripStatusLabel1.Text = "resFactor: " + Math.Max(res, 1).ToString("N2");

                if (res > 1)
                {
                    Bitmap? bOld = bWork;
                    bWork = ResampleDown(bWork, res);
                    if (bOld != null)
                    {
                        bOld.Dispose();
                        bOld = null;
                    }
                }

                if (cbHalfSize.Checked && rbClosedForm.Checked)
                {
                    Bitmap bWork2 = ResampleBmp(bWork, 2);

                    Bitmap? bOld = bWork;
                    bWork = bWork2;
                    bOld.Dispose();
                    bOld = null;
                }

                double factor = (this.cbHalfSize.Checked && this.rbClosedForm.Checked) ? 2.0 : 1.0;
                factor *= (res > 1) ? res : 1.0;

                Bitmap bTrimap = new Bitmap(bWork.Width, bWork.Height);
                this.SetBitmap(ref _bWork, ref bWork);

                bool drawPaths = this.cbDrawPaths.Checked;
                float wFactor = (float)this.numScribblesWFactor.Value;

                this.backgroundWorker4.RunWorkerAsync(new object[] { factor, bTrimap, drawPaths, wFactor });
            }
        }

        private Bitmap ResampleBmp(Bitmap bmp, int n)
        {
            Bitmap bOut = new Bitmap(bmp.Width / n, bmp.Height / n);

            using (Graphics gx = Graphics.FromImage(bOut))
            {
                gx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gx.DrawImage(bmp, 0, 0, bOut.Width, bOut.Height);
            }

            return bOut;
        }

        private Bitmap? ResampleBack(Bitmap bmp)
        {
            if (this.helplineRulerCtrl1 != null && !this.IsDisposed && this.helplineRulerCtrl1.Bmp != null && bmp != null)
            {
                Bitmap bOut = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                using (Graphics gx = Graphics.FromImage(bOut))
                {
                    gx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    gx.DrawImage(bmp, 0, 0, bOut.Width, bOut.Height);
                }

                return bOut;
            }

            return null;
        }

        private double CheckWidthHeight(Bitmap bmp, bool fp, double maxSize)
        {
            double r = 1.0;
            if (fp)
            {
                r = (double)Math.Max(bmp.Width, bmp.Height) / maxSize;
                return r;
            }

            int res = 1;
            if (bmp.Width * bmp.Height > maxSize * maxSize * 256L)
            {
                res = 32;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize * 128L)
            {
                res = 24;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize * 64L)
            {
                res = 16;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize * 32L)
            {
                res = 12;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize * 16L)
            {
                res = 8;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize * 8L)
            {
                res = 6;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize * 4L)
            {
                res = 4;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize * 2L)
            {
                res = 3;
            }
            else if (bmp.Width * bmp.Height > maxSize * maxSize)
            {
                res = 2;
            }

            return res;
        }

        private Bitmap ResampleDown(Bitmap bWork, double resPic)
        {
            Bitmap bOut = new Bitmap((int)Math.Ceiling(bWork.Width / resPic), (int)Math.Ceiling(bWork.Height / resPic));

            using (Graphics gx = Graphics.FromImage(bOut))
                gx.DrawImage(bWork, 0, 0, bOut.Width, bOut.Height);

            return bOut;
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            _cfop_ShowInfo(this, "outer pic-amount " + "...");
            if (this._cfop != null && e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                int mode = (int)o[0];

                bool scalesPics = (bool)o[1];
                int scales = (int)o[2];

                int overlap = (int)o[3];
                bool interpolated = (bool)o[4];
                bool forceSerial = (bool)o[5];

                bool group = (bool)o[6];
                int groupAmountX = (int)o[7];
                int groupAmountY = (int)o[8];

                int maxSize = (int)o[9];

                Bitmap bWork = (Bitmap)o[10];
                Bitmap trWork = (Bitmap)o[11];

                bool trySingleTile = (bool)o[12];
                bool verifyTrimaps = (bool)o[13];

                string trimapProblemMessage = "In this configuration at least one trimap does not contain sufficient Information. " +
                    "Consider running the task again with a larger tileSize or less subtiles.\n\nYou could also rebuild the matte " +
                    "for selected rectangles by clicking on the \"RescanParts\" button.";
                int id = Environment.TickCount;
                this._lastRunNumber = id;

                if (!scalesPics)
                {
                    if (!AvailMem.AvailMem.checkAvailRam(bWork.Width * bWork.Height * 20L))
                        trySingleTile = false;

                    if (trySingleTile)
                    {
                        if (verifyTrimaps)
                        {
                            if (!CheckTrimap(trWork))
                            {
                                this.Invoke(new Action(() =>
                                {
                                    if (this._frmInfo == null || this._frmInfo.IsDisposed)
                                        this._frmInfo = new frmInfo();
                                    this._frmInfo.Show(trimapProblemMessage);
                                }));

                                this._trimapProblemInfos.Add(new TrimapProblemInfo(id, 1, 0, 0, trWork.Width, trWork.Height, 0));
                            }
                        }

                        _cfop_ShowInfo(this, "outer pic-amount " + 1.ToString());
                        this._cfop.GetMattingLaplacian(Math.Pow(10, -7));
                        Bitmap? b = null;

                        if (this._cfop.BlendParameters != null && this._cfop.BlendParameters.BGW != null && this._cfop.BlendParameters.BGW.WorkerSupportsCancellation && this._cfop.BlendParameters.BGW.CancellationPending)
                        {
                            e.Result = null;
                            return;
                        }

                        if (mode == 0)
                            b = this._cfop.SolveSystemGaussSeidel();
                        else if (mode == 1)
                            b = this._cfop.SolveSystemGMRES();

                        e.Result = b;
                    }
                    else
                    {
                        Bitmap result = new Bitmap(bWork.Width, bWork.Height);

                        int wh = bWork.Width * bWork.Height;
                        int n = 1;

                        while (wh > maxSize)
                        {
                            n += 1;
                            wh = bWork.Width / n * bWork.Height / n;
                        }
                        int n2 = n * n;

                        int h = bWork.Height / n;
                        int h2 = bWork.Height - h * (n - 1);

                        int w = bWork.Width / n;
                        int w2 = bWork.Width - w * (n - 1);

                        overlap = Math.Max(overlap, 1);

                        if (n2 == 1)
                            overlap = 0;

                        List<Bitmap> bmp = new List<Bitmap>();
                        List<Bitmap> bmp2 = new List<Bitmap>();

                        GetTiles(bWork, trWork, bmp, bmp2, w, w2, h, h2, overlap, n);

                        if (verifyTrimaps)
                            if (!CheckTrimaps(bmp2, w, h, n, id, overlap))
                                this.Invoke(new Action(() =>
                                {
                                    if (this._frmInfo == null || this._frmInfo.IsDisposed)
                                        this._frmInfo = new frmInfo();
                                    this._frmInfo.Show(trimapProblemMessage);
                                }));

                        Bitmap[]? bmp4 = new Bitmap[bmp.Count];

                        this._cfopArray = new ClosedFormMatteOp[bmp.Count];

                        if (_cfop.BlendParameters != null && AvailMem.AvailMem.checkAvailRam(w * h * bmp.Count * 20L) && !forceSerial)
                            Parallel.For(0, bmp.Count, i =>
                            {
                                _cfop_ShowInfo(this, "pic " + (i + 1).ToString());
                                ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                                BlendParameters bParam = new BlendParameters();
                                bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                                bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                                bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                                bParam.Sleep = _cfop.BlendParameters.Sleep;
                                bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                                bParam.BGW = this.backgroundWorker1;
                                cfop.BlendParameters = bParam;

                                cfop.ShowProgess += Cfop_UpdateProgress;
                                cfop.ShowInfo += _cfop_ShowInfo;

                                this._cfopArray[i] = cfop;

                                cfop.GetMattingLaplacian(Math.Pow(10, -7));
                                Bitmap? b = null;

                                if (mode == 0)
                                    b = cfop.SolveSystemGaussSeidel();
                                else if (mode == 1)
                                    b = cfop.SolveSystemGMRES();

                                //save and draw out later serially
                                if (b != null)
                                    bmp4[i] = b;

                                cfop.ShowProgess -= Cfop_UpdateProgress;
                                cfop.ShowInfo -= _cfop_ShowInfo;
                                cfop.Dispose();
                            });
                        else
                        {
                            if (_cfop.BlendParameters != null)
                                for (int i = 0; i < bmp.Count; i++)
                                {
                                    _cfop_ShowInfo(this, "pic " + (i + 1).ToString());
                                    ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                                    BlendParameters bParam = new BlendParameters();
                                    bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                                    bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                                    bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                                    bParam.Sleep = _cfop.BlendParameters.Sleep;
                                    bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                                    bParam.BGW = this.backgroundWorker1;
                                    cfop.BlendParameters = bParam;

                                    cfop.ShowProgess += Cfop_UpdateProgress;
                                    cfop.ShowInfo += _cfop_ShowInfo;

                                    this._cfopArray[i] = cfop;

                                    cfop.GetMattingLaplacian(Math.Pow(10, -7));
                                    Bitmap? b = null;

                                    if (mode == 0)
                                        b = cfop.SolveSystemGaussSeidel();
                                    else if (mode == 1)
                                        b = cfop.SolveSystemGMRES();

                                    //save and draw out later serially
                                    if (b != null)
                                        bmp4[i] = b;

                                    cfop.ShowProgess -= Cfop_UpdateProgress;
                                    cfop.ShowInfo -= _cfop_ShowInfo;
                                    cfop.Dispose();
                                }
                        }

                        if (this._cfop.BlendParameters != null && this._cfop.BlendParameters.BGW != null && this._cfop.BlendParameters.BGW.WorkerSupportsCancellation && this._cfop.BlendParameters.BGW.CancellationPending)
                        {
                            for (int i = bmp.Count - 1; i >= 0; i--)
                            {
                                if (bmp[i] != null)
                                    bmp[i].Dispose();
                                if (bmp2[i] != null)
                                    bmp2[i].Dispose();
                                if (bmp4[i] != null)
                                    bmp4[i].Dispose();
                            }
                            e.Result = null;
                            return;
                        }

                        for (int i = 0; i < bmp.Count; i++)
                        {
                            int x = i % n;
                            int y = i / n;

                            using (Graphics gx = Graphics.FromImage(result))
                                gx.DrawImage(bmp4[i], x * w - (x == 0 ? 0 : overlap), y * h - (y == 0 ? 0 : overlap));
                        }

                        for (int i = bmp.Count - 1; i >= 0; i--)
                        {
                            bmp[i].Dispose();
                            bmp2[i].Dispose();
                            bmp4[i].Dispose();
                        }

                        e.Result = result;
                    }
                }
                else
                {
                    if (trySingleTile)
                    {
                        bool wgth = bWork.Width > bWork.Height;
                        int xP = 2;
                        int yP = 2;

                        if (scales == 8)
                        {
                            xP = wgth ? 4 : 2;
                            yP = wgth ? 2 : 4;
                        }
                        if (scales == 16)
                        {
                            xP = 4;
                            yP = 4;
                        }
                        if (scales == 32)
                        {
                            xP = wgth ? 8 : 4;
                            yP = wgth ? 4 : 8;
                        }
                        if (scales == 64)
                        {
                            xP = 8;
                            yP = 8;
                        }
                        if (scales == 128)
                        {
                            xP = wgth ? 16 : 8;
                            yP = wgth ? 8 : 16;
                        }
                        if (scales == 256)
                        {
                            xP = 16;
                            yP = 16;
                        }

                        int w = bWork.Width;
                        int h = bWork.Height;

                        Bitmap cfopBmp = bWork;
                        Bitmap cfopTrimap = trWork;

                        if (interpolated)
                        {
                            w = (int)(w * 1.41);
                            h = (int)(h * 1.41);

                            cfopBmp = new Bitmap(w, h);
                            cfopTrimap = new Bitmap(w, h);

                            using (Graphics gx = Graphics.FromImage(cfopBmp))
                            {
                                gx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                gx.DrawImage(bWork, 0, 0, w, h);
                            }

                            using (Graphics gx = Graphics.FromImage(cfopTrimap))
                            {
                                gx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                gx.DrawImage(trWork, 0, 0, w, h);
                            }
                        }

                        Bitmap result = new Bitmap(w, h);

                        List<Bitmap> bmp = new List<Bitmap>();
                        List<Bitmap> bmp2 = new List<Bitmap>();
                        List<Size> sizes = new List<Size>();

                        int ww = 0;
                        int hh = 0;

                        int www = result.Width / xP;
                        int hhh = result.Height / yP;
                        int xAdd2 = result.Width - www * xP;
                        int yAdd2 = result.Height - hhh * yP;

                        for (int y = 0; y < yP; y++)
                        {
                            for (int x = 0; x < xP; x++)
                            {
                                Size sz = new Size(result.Width / xP, result.Height / yP);
                                int xAdd = result.Width - sz.Width * xP;
                                int yAdd = result.Height - sz.Height * yP;

                                if (y < yP - 1)
                                {
                                    if (x < xP - 1)
                                        sizes.Add(new Size(sz.Width, sz.Height));
                                    else
                                        sizes.Add(new Size(sz.Width + xAdd, sz.Height));
                                }
                                else
                                {
                                    if (x < xP - 1)
                                        sizes.Add(new Size(sz.Width, sz.Height + yAdd));
                                    else
                                        sizes.Add(new Size(sz.Width + xAdd, sz.Height + yAdd));
                                }

                                int xx = sizes[sizes.Count - 1].Width / groupAmountX;
                                int yy = sizes[sizes.Count - 1].Height / groupAmountY;

                                ww = sizes[sizes.Count - 1].Width - (xx * groupAmountX);
                                hh = sizes[sizes.Count - 1].Height - (yy * groupAmountY);

                                Bitmap b1 = new Bitmap(sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height);
                                Bitmap b2 = new Bitmap(sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height);

                                int wdth = result.Width;
                                int hght = result.Height;

                                BitmapData bmD = cfopBmp.LockBits(new Rectangle(0, 0, wdth, hght), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                BitmapData bmT = cfopTrimap.LockBits(new Rectangle(0, 0, wdth, hght), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                BitmapData b1D = b1.LockBits(new Rectangle(0, 0, sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                                BitmapData b2D = b2.LockBits(new Rectangle(0, 0, sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                                int strideD = bmD.Stride;
                                int strideB = b1D.Stride;

                                unsafe
                                {
                                    byte* p = (byte*)bmD.Scan0;
                                    byte* pT = (byte*)bmT.Scan0;
                                    byte* p1 = (byte*)b1D.Scan0;
                                    byte* p2 = (byte*)b2D.Scan0;

                                    if (group)
                                    {
                                        for (int y2 = y * groupAmountY, y4 = 0; y2 < hght + yP; y2 += yP * groupAmountY, y4 += groupAmountY)
                                        {
                                            for (int x2 = x * groupAmountX, x4 = 0; x2 < wdth + xP; x2 += xP * groupAmountX, x4 += groupAmountX)
                                            {
                                                for (int y7 = y2, y41 = y4, cntY = 0; y7 <= y2 + groupAmountY; y7++, y41++)
                                                {
                                                    for (int x7 = x2, x41 = x4, cntX = 0; x7 <= x2 + groupAmountX; x7++, x41++)
                                                    {
                                                        if (x7 < wdth - ww * xP && y7 < hght - hh * yP && x41 < b1.Width && y41 < b1.Height)
                                                        {
                                                            p1[y41 * strideB + x41 * 4] = p[y7 * strideD + x7 * 4];
                                                            p1[y41 * strideB + x41 * 4 + 1] = p[y7 * strideD + x7 * 4 + 1];
                                                            p1[y41 * strideB + x41 * 4 + 2] = p[y7 * strideD + x7 * 4 + 2];
                                                            p1[y41 * strideB + x41 * 4 + 3] = p[y7 * strideD + x7 * 4 + 3];

                                                            p2[y41 * strideB + x41 * 4] = pT[y7 * strideD + x7 * 4];
                                                            p2[y41 * strideB + x41 * 4 + 1] = pT[y7 * strideD + x7 * 4 + 1];
                                                            p2[y41 * strideB + x41 * 4 + 2] = pT[y7 * strideD + x7 * 4 + 2];
                                                            p2[y41 * strideB + x41 * 4 + 3] = pT[y7 * strideD + x7 * 4 + 3];
                                                        }
                                                        else
                                                        {
                                                            if (x7 > wdth)
                                                            {
                                                                x7 = wdth - 1 - (ww * (xP - x)) + cntX - 1;
                                                                x41 = b1.Width - 1 - ww + cntX;
                                                                cntX++;
                                                            }
                                                            if (y7 >= hght)
                                                            {
                                                                y7 = hght - 1 - (hh * (yP - y)) + cntY - 1;
                                                                y41 = b1.Height - 1 - hh + cntY;
                                                                cntY++;
                                                            }

                                                            if (x7 < wdth && y7 < hght && x41 < b1.Width && y41 < b1.Height)
                                                            {
                                                                p1[y41 * strideB + x41 * 4] = p[y7 * strideD + x7 * 4];
                                                                p1[y41 * strideB + x41 * 4 + 1] = p[y7 * strideD + x7 * 4 + 1];
                                                                p1[y41 * strideB + x41 * 4 + 2] = p[y7 * strideD + x7 * 4 + 2];
                                                                p1[y41 * strideB + x41 * 4 + 3] = p[y7 * strideD + x7 * 4 + 3];

                                                                p2[y41 * strideB + x41 * 4] = pT[y7 * strideD + x7 * 4];
                                                                p2[y41 * strideB + x41 * 4 + 1] = pT[y7 * strideD + x7 * 4 + 1];
                                                                p2[y41 * strideB + x41 * 4 + 2] = pT[y7 * strideD + x7 * 4 + 2];
                                                                p2[y41 * strideB + x41 * 4 + 3] = pT[y7 * strideD + x7 * 4 + 3];
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int y2 = y, y4 = 0; y2 < hght + yP; y2 += yP, y4++)
                                        {
                                            for (int x2 = x, x4 = 0; x2 < wdth + xP; x2 += xP, x4++)
                                            {
                                                if (x4 < b1.Width && y4 < b1.Height && x2 < wdth && y2 < hght)
                                                {
                                                    p1[y4 * strideB + x4 * 4] = p[y2 * strideD + x2 * 4];
                                                    p1[y4 * strideB + x4 * 4 + 1] = p[y2 * strideD + x2 * 4 + 1];
                                                    p1[y4 * strideB + x4 * 4 + 2] = p[y2 * strideD + x2 * 4 + 2];
                                                    p1[y4 * strideB + x4 * 4 + 3] = p[y2 * strideD + x2 * 4 + 3];

                                                    p2[y4 * strideB + x4 * 4] = pT[y2 * strideD + x2 * 4];
                                                    p2[y4 * strideB + x4 * 4 + 1] = pT[y2 * strideD + x2 * 4 + 1];
                                                    p2[y4 * strideB + x4 * 4 + 2] = pT[y2 * strideD + x2 * 4 + 2];
                                                    p2[y4 * strideB + x4 * 4 + 3] = pT[y2 * strideD + x2 * 4 + 3];
                                                }
                                                else if (x4 < b1.Width && y4 < b1.Height && (x2 >= wdth || y2 >= hght))
                                                {
                                                    if (x2 >= wdth)
                                                        x2 -= xP - (xP - x);
                                                    if (y2 >= hght)
                                                        y2 -= yP - (yP - y);

                                                    p1[y4 * strideB + x4 * 4] = p[y2 * strideD + x2 * 4];
                                                    p1[y4 * strideB + x4 * 4 + 1] = p[y2 * strideD + x2 * 4 + 1];
                                                    p1[y4 * strideB + x4 * 4 + 2] = p[y2 * strideD + x2 * 4 + 2];
                                                    p1[y4 * strideB + x4 * 4 + 3] = p[y2 * strideD + x2 * 4 + 3];

                                                    p2[y4 * strideB + x4 * 4] = pT[y2 * strideD + x2 * 4];
                                                    p2[y4 * strideB + x4 * 4 + 1] = pT[y2 * strideD + x2 * 4 + 1];
                                                    p2[y4 * strideB + x4 * 4 + 2] = pT[y2 * strideD + x2 * 4 + 2];
                                                    p2[y4 * strideB + x4 * 4 + 3] = pT[y2 * strideD + x2 * 4 + 3];
                                                }
                                            }
                                        }
                                    }
                                }

                                b2.UnlockBits(b2D);
                                b1.UnlockBits(b1D);
                                cfopTrimap.UnlockBits(bmT);
                                cfopBmp.UnlockBits(bmD);

                                bmp.Add(b1);
                                bmp2.Add(b2);

                                //try
                                //{
                                //    Form fff = new Form();
                                //    fff.BackgroundImage = b1;
                                //    fff.BackgroundImageLayout = ImageLayout.Zoom;
                                //    fff.ShowDialog();
                                //    fff.BackgroundImage = b2;
                                //    fff.BackgroundImageLayout = ImageLayout.Zoom;
                                //    fff.ShowDialog();
                                //}
                                //catch (Exception exc)
                                //{
                                //    Console.WriteLine(exc.ToString());
                                //}
                            }
                        }

                        if (verifyTrimaps)
                            if (!CheckTrimaps(bmp2, www, hhh, xP, id, xAdd2, yAdd2))  //no overlap, we dont have an outerArray and all inner pics resemble the whole pic
                                this.Invoke(new Action(() =>
                                {
                                    if (this._frmInfo == null || this._frmInfo.IsDisposed)
                                        this._frmInfo = new frmInfo();
                                    this._frmInfo.Show(trimapProblemMessage);
                                }));

                        Bitmap[] bmp4 = new Bitmap[bmp.Count];

                        _cfop_ShowInfo(this, "outer pic-amount " + bmp.Count().ToString());

                        this._cfopArray = new ClosedFormMatteOp[bmp.Count];

                        if (_cfop.BlendParameters != null && AvailMem.AvailMem.checkAvailRam(w * h * bmp.Count * 10L) && !forceSerial)
                            Parallel.For(0, bmp.Count, i =>
                            //for(int i = 0; i < bmp.Count; i++)
                            {
                                ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                                BlendParameters bParam = new BlendParameters();
                                bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                                bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                                bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                                bParam.Sleep = _cfop.BlendParameters.Sleep;
                                bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                                bParam.BGW = this.backgroundWorker1;
                                cfop.BlendParameters = bParam;

                                cfop.ShowProgess += Cfop_UpdateProgress;
                                cfop.ShowInfo += _cfop_ShowInfo;

                                this._cfopArray[i] = cfop;

                                cfop.GetMattingLaplacian(Math.Pow(10, -7));
                                Bitmap? b = null;

                                if (mode == 0)
                                    b = cfop.SolveSystemGaussSeidel();
                                else if (mode == 1)
                                    b = cfop.SolveSystemGMRES();

                                //save and draw out later serially
                                if (b != null)
                                    bmp4[i] = b;

                                cfop.ShowProgess -= Cfop_UpdateProgress;
                                cfop.ShowInfo -= _cfop_ShowInfo;
                                cfop.Dispose();
                            });
                        else
                            if (_cfop.BlendParameters != null)
                            for (int i = 0; i < bmp.Count; i++)
                            {
                                _cfop_ShowInfo(this, "pic " + (i + 1).ToString());
                                ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                                BlendParameters bParam = new BlendParameters();
                                bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                                bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                                bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                                bParam.Sleep = _cfop.BlendParameters.Sleep;
                                bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                                bParam.BGW = this.backgroundWorker1;
                                cfop.BlendParameters = bParam;

                                cfop.ShowProgess += Cfop_UpdateProgress;
                                cfop.ShowInfo += _cfop_ShowInfo;

                                this._cfopArray[i] = cfop;

                                cfop.GetMattingLaplacian(Math.Pow(10, -7));
                                Bitmap? b = null;

                                if (mode == 0)
                                    b = cfop.SolveSystemGaussSeidel();
                                else if (mode == 1)
                                    b = cfop.SolveSystemGMRES();

                                //save and draw out later serially
                                if (b != null)
                                    bmp4[i] = b;

                                cfop.ShowProgess -= Cfop_UpdateProgress;
                                cfop.ShowInfo -= _cfop_ShowInfo;
                                cfop.Dispose();
                            }

                        if (this._cfop.BlendParameters != null && this._cfop.BlendParameters.BGW != null && this._cfop.BlendParameters.BGW.WorkerSupportsCancellation && this._cfop.BlendParameters.BGW.CancellationPending)
                        {
                            for (int i = bmp.Count - 1; i >= 0; i--)
                            {
                                if (bmp[i] != null)
                                    bmp[i].Dispose();
                                if (bmp2[i] != null)
                                    bmp2[i].Dispose();
                                if (bmp4[i] != null)
                                    bmp4[i].Dispose();
                            }
                            e.Result = null;
                            return;
                        }

                        for (int y = 0; y < yP; y++)
                        {
                            for (int x = 0; x < xP; x++)
                            {
                                int indx = y * xP + x;
                                int wdth = result.Width;
                                int hght = result.Height;

                                BitmapData bmR = result.LockBits(new Rectangle(0, 0, wdth, hght), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                                BitmapData b4D = bmp4[indx].LockBits(new Rectangle(0, 0, sizes[indx].Width, sizes[indx].Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                                int strideR = bmR.Stride;
                                int strideB = b4D.Stride;

                                unsafe
                                {
                                    byte* p = (byte*)bmR.Scan0;
                                    byte* pB = (byte*)b4D.Scan0;

                                    if (group)
                                    {
                                        for (int y2 = y * groupAmountY, y4 = 0; y2 < hght + yP * groupAmountY; y2 += yP * groupAmountY, y4 += groupAmountY)
                                        {
                                            for (int x2 = x * groupAmountX, x4 = 0; x2 < wdth + xP * groupAmountX; x2 += xP * groupAmountX, x4 += groupAmountX)
                                            {
                                                for (int y7 = y2, y41 = y4, cntY = 0; y7 < y2 + groupAmountY; y7++, y41++)
                                                {
                                                    for (int x7 = x2, x41 = x4, cntX = 0; x7 < x2 + groupAmountX; x7++, x41++)
                                                    {
                                                        if (x7 < wdth - ww * xP && y7 < hght - hh * yP && x41 < b4D.Width && y41 < b4D.Height)
                                                        {
                                                            p[y7 * strideR + x7 * 4] = pB[y41 * strideB + x41 * 4];
                                                            p[y7 * strideR + x7 * 4 + 1] = pB[y41 * strideB + x41 * 4 + 1];
                                                            p[y7 * strideR + x7 * 4 + 2] = pB[y41 * strideB + x41 * 4 + 2];
                                                            p[y7 * strideR + x7 * 4 + 3] = pB[y41 * strideB + x41 * 4 + 3];
                                                        }
                                                        else
                                                        {
                                                            if (x7 > wdth)
                                                            {
                                                                x7 = wdth - 1 - (ww * (xP - x)) + cntX - 1;
                                                                x41 = b4D.Width - 1 - ww + cntX;
                                                                cntX++;
                                                            }
                                                            if (y7 >= hght)
                                                            {
                                                                y7 = hght - 1 - (hh * (yP - y)) + cntY - 1;
                                                                y41 = b4D.Height - 1 - hh + cntY;
                                                                cntY++;
                                                            }

                                                            if (x7 < wdth && y7 < hght && x41 < b4D.Width && y41 < b4D.Height)
                                                            {
                                                                p[y7 * strideR + x7 * 4] = pB[y41 * strideB + x41 * 4];
                                                                p[y7 * strideR + x7 * 4 + 1] = pB[y41 * strideB + x41 * 4 + 1];
                                                                p[y7 * strideR + x7 * 4 + 2] = pB[y41 * strideB + x41 * 4 + 2];
                                                                p[y7 * strideR + x7 * 4 + 3] = pB[y41 * strideB + x41 * 4 + 3];
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        for (int y2 = y, y4 = 0; y2 < hght + yP; y2 += yP, y4++)
                                        {
                                            for (int x2 = x, x4 = 0; x2 < wdth + xP; x2 += xP, x4++)
                                            {
                                                if (x4 < b4D.Width && y4 < b4D.Height && x2 < wdth && y2 < hght)
                                                {
                                                    p[y2 * strideR + x2 * 4] = pB[y4 * strideB + x4 * 4];
                                                    p[y2 * strideR + x2 * 4 + 1] = pB[y4 * strideB + x4 * 4 + 1];
                                                    p[y2 * strideR + x2 * 4 + 2] = pB[y4 * strideB + x4 * 4 + 2];
                                                    p[y2 * strideR + x2 * 4 + 3] = pB[y4 * strideB + x4 * 4 + 3];
                                                }
                                                else if (x4 < b4D.Width && y4 < b4D.Height && (x2 >= wdth || y2 >= hght))
                                                {
                                                    if (x2 >= wdth)
                                                        x2 -= xP - (xP - x);
                                                    if (y2 >= hght)
                                                        y2 -= yP - (yP - y);

                                                    p[y2 * strideR + x2 * 4] = pB[y4 * strideB + x4 * 4];
                                                    p[y2 * strideR + x2 * 4 + 1] = pB[y4 * strideB + x4 * 4 + 1];
                                                    p[y2 * strideR + x2 * 4 + 2] = pB[y4 * strideB + x4 * 4 + 2];
                                                    p[y2 * strideR + x2 * 4 + 3] = pB[y4 * strideB + x4 * 4 + 3];
                                                }
                                            }
                                        }
                                    }
                                }

                                bmp4[indx].UnlockBits(b4D);
                                result.UnlockBits(bmR);

                                //Form fff = new Form();
                                //fff.BackgroundImage = result;
                                //fff.BackgroundImageLayout = ImageLayout.Zoom;
                                //fff.ShowDialog();
                            }
                        }

                        for (int i = bmp.Count - 1; i >= 0; i--)
                        {
                            bmp[i].Dispose();
                            bmp2[i].Dispose();
                            bmp4[i].Dispose();
                        }

                        if (interpolated)
                        {
                            using (Bitmap resOld = result)
                            {
                                result = new Bitmap(bWork.Width, bWork.Height);

                                using (Graphics gx = Graphics.FromImage(result))
                                {
                                    gx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                    gx.DrawImage(resOld, 0, 0, result.Width, result.Height);
                                }
                            }

                            cfopBmp.Dispose();
                            cfopTrimap.Dispose();
                        }

                        e.Result = result;
                    }
                    else
                    {
                        int wh = bWork.Width * bWork.Height;
                        int n = 1;

                        while (wh > maxSize)
                        {
                            n += 1;
                            wh = bWork.Width / n * bWork.Height / n;
                        }
                        int n2 = n * n;

                        int hhh = bWork.Height / n;
                        int hhh2 = bWork.Height - hhh * (n - 1);

                        int www = bWork.Width / n;
                        int www2 = bWork.Width - www * (n - 1);

                        overlap = Math.Max(overlap, 1);

                        if (n2 == 1)
                            overlap = 0;

                        List<Bitmap> bmpF = new List<Bitmap>();
                        List<Bitmap> bmpF2 = new List<Bitmap>();

                        GetTiles(bWork, trWork, bmpF, bmpF2, www, www2, hhh, hhh2, overlap, n);

                        Bitmap[] bmpF4 = new Bitmap[bmpF.Count];

                        Bitmap bmpResult = new Bitmap(bWork.Width, bWork.Height);

                        for (int j = 0; j < bmpF.Count; j++)
                        {
                            bool wgth = bWork.Width > bWork.Height;
                            int xP = 2;
                            int yP = 2;

                            _cfop_ShowInfo(this, "picOuter " + (j + 1).ToString());

                            if (scales == 8)
                            {
                                xP = wgth ? 4 : 2;
                                yP = wgth ? 2 : 4;
                            }
                            if (scales == 16)
                            {
                                xP = 4;
                                yP = 4;
                            }
                            if (scales == 32)
                            {
                                xP = wgth ? 8 : 4;
                                yP = wgth ? 4 : 8;
                            }
                            if (scales == 64)
                            {
                                xP = 8;
                                yP = 8;
                            }
                            if (scales == 128)
                            {
                                xP = wgth ? 16 : 8;
                                yP = wgth ? 8 : 16;
                            }
                            if (scales == 256)
                            {
                                xP = 16;
                                yP = 16;
                            }

                            int w = bmpF[j].Width;
                            int h = bmpF[j].Height;

                            Bitmap cfopBmp = bmpF[j];
                            Bitmap cfopTrimap = bmpF2[j];

                            if (interpolated)
                            {
                                w = (int)(w * 1.41);
                                h = (int)(h * 1.41);

                                cfopBmp = new Bitmap(w, h);
                                cfopTrimap = new Bitmap(w, h);

                                using (Graphics gx = Graphics.FromImage(cfopBmp))
                                {
                                    gx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                    gx.DrawImage(bmpF[j], 0, 0, w, h);
                                }

                                using (Graphics gx = Graphics.FromImage(cfopTrimap))
                                {
                                    gx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                    gx.DrawImage(bmpF2[j], 0, 0, w, h);
                                }
                            }

                            Bitmap result = new Bitmap(w, h);

                            List<Bitmap> bmp = new List<Bitmap>();
                            List<Bitmap> bmp2 = new List<Bitmap>();
                            List<Size> sizes = new List<Size>();

                            int ww = 0;
                            int hh = 0;

                            for (int y = 0; y < yP; y++)
                            {
                                for (int x = 0; x < xP; x++)
                                {
                                    Size sz = new Size(result.Width / xP, result.Height / yP);
                                    int xAdd = result.Width - sz.Width * xP;
                                    int yAdd = result.Height - sz.Height * yP;

                                    if (y < yP - 1)
                                    {
                                        if (x < xP - 1)
                                            sizes.Add(new Size(sz.Width, sz.Height));
                                        else
                                            sizes.Add(new Size(sz.Width + xAdd, sz.Height));
                                    }
                                    else
                                    {
                                        if (x < xP - 1)
                                            sizes.Add(new Size(sz.Width, sz.Height + yAdd));
                                        else
                                            sizes.Add(new Size(sz.Width + xAdd, sz.Height + yAdd));
                                    }

                                    int xx = sizes[sizes.Count - 1].Width / groupAmountX;
                                    int yy = sizes[sizes.Count - 1].Height / groupAmountY;

                                    ww = sizes[sizes.Count - 1].Width - (xx * groupAmountX);
                                    hh = sizes[sizes.Count - 1].Height - (yy * groupAmountY);

                                    Bitmap b1 = new Bitmap(sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height);
                                    Bitmap b2 = new Bitmap(sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height);

                                    int wdth = result.Width;
                                    int hght = result.Height;

                                    BitmapData bmD = cfopBmp.LockBits(new Rectangle(0, 0, wdth, hght), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                    BitmapData bmT = cfopTrimap.LockBits(new Rectangle(0, 0, wdth, hght), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                                    BitmapData b1D = b1.LockBits(new Rectangle(0, 0, sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                                    BitmapData b2D = b2.LockBits(new Rectangle(0, 0, sizes[sizes.Count - 1].Width, sizes[sizes.Count - 1].Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                                    int strideD = bmD.Stride;
                                    int strideB = b1D.Stride;

                                    unsafe
                                    {
                                        byte* p = (byte*)bmD.Scan0;
                                        byte* pT = (byte*)bmT.Scan0;
                                        byte* p1 = (byte*)b1D.Scan0;
                                        byte* p2 = (byte*)b2D.Scan0;

                                        if (group)
                                        {
                                            for (int y2 = y * groupAmountY, y4 = 0; y2 < hght + yP; y2 += yP * groupAmountY, y4 += groupAmountY)
                                            {
                                                for (int x2 = x * groupAmountX, x4 = 0; x2 < wdth + xP; x2 += xP * groupAmountX, x4 += groupAmountX)
                                                {
                                                    for (int y7 = y2, y41 = y4, cntY = 0; y7 <= y2 + groupAmountY; y7++, y41++)
                                                    {
                                                        for (int x7 = x2, x41 = x4, cntX = 0; x7 <= x2 + groupAmountX; x7++, x41++)
                                                        {
                                                            if (x7 < wdth - ww * xP && y7 < hght - hh * yP && x41 < b1.Width && y41 < b1.Height)
                                                            {
                                                                p1[y41 * strideB + x41 * 4] = p[y7 * strideD + x7 * 4];
                                                                p1[y41 * strideB + x41 * 4 + 1] = p[y7 * strideD + x7 * 4 + 1];
                                                                p1[y41 * strideB + x41 * 4 + 2] = p[y7 * strideD + x7 * 4 + 2];
                                                                p1[y41 * strideB + x41 * 4 + 3] = p[y7 * strideD + x7 * 4 + 3];

                                                                p2[y41 * strideB + x41 * 4] = pT[y7 * strideD + x7 * 4];
                                                                p2[y41 * strideB + x41 * 4 + 1] = pT[y7 * strideD + x7 * 4 + 1];
                                                                p2[y41 * strideB + x41 * 4 + 2] = pT[y7 * strideD + x7 * 4 + 2];
                                                                p2[y41 * strideB + x41 * 4 + 3] = pT[y7 * strideD + x7 * 4 + 3];
                                                            }
                                                            else
                                                            {
                                                                if (x7 > wdth)
                                                                {
                                                                    x7 = wdth - 1 - (ww * (xP - x)) + cntX - 1;
                                                                    x41 = b1.Width - 1 - ww + cntX;
                                                                    cntX++;
                                                                }
                                                                if (y7 >= hght)
                                                                {
                                                                    y7 = hght - 1 - (hh * (yP - y)) + cntY - 1;
                                                                    y41 = b1.Height - 1 - hh + cntY;
                                                                    cntY++;
                                                                }

                                                                if (x7 < wdth && y7 < hght && x41 < b1.Width && y41 < b1.Height)
                                                                {
                                                                    p1[y41 * strideB + x41 * 4] = p[y7 * strideD + x7 * 4];
                                                                    p1[y41 * strideB + x41 * 4 + 1] = p[y7 * strideD + x7 * 4 + 1];
                                                                    p1[y41 * strideB + x41 * 4 + 2] = p[y7 * strideD + x7 * 4 + 2];
                                                                    p1[y41 * strideB + x41 * 4 + 3] = p[y7 * strideD + x7 * 4 + 3];

                                                                    p2[y41 * strideB + x41 * 4] = pT[y7 * strideD + x7 * 4];
                                                                    p2[y41 * strideB + x41 * 4 + 1] = pT[y7 * strideD + x7 * 4 + 1];
                                                                    p2[y41 * strideB + x41 * 4 + 2] = pT[y7 * strideD + x7 * 4 + 2];
                                                                    p2[y41 * strideB + x41 * 4 + 3] = pT[y7 * strideD + x7 * 4 + 3];
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int y2 = y, y4 = 0; y2 < hght + yP; y2 += yP, y4++)
                                            {
                                                for (int x2 = x, x4 = 0; x2 < wdth + xP; x2 += xP, x4++)
                                                {
                                                    if (x4 < b1.Width && y4 < b1.Height && x2 < wdth && y2 < hght)
                                                    {
                                                        p1[y4 * strideB + x4 * 4] = p[y2 * strideD + x2 * 4];
                                                        p1[y4 * strideB + x4 * 4 + 1] = p[y2 * strideD + x2 * 4 + 1];
                                                        p1[y4 * strideB + x4 * 4 + 2] = p[y2 * strideD + x2 * 4 + 2];
                                                        p1[y4 * strideB + x4 * 4 + 3] = p[y2 * strideD + x2 * 4 + 3];

                                                        p2[y4 * strideB + x4 * 4] = pT[y2 * strideD + x2 * 4];
                                                        p2[y4 * strideB + x4 * 4 + 1] = pT[y2 * strideD + x2 * 4 + 1];
                                                        p2[y4 * strideB + x4 * 4 + 2] = pT[y2 * strideD + x2 * 4 + 2];
                                                        p2[y4 * strideB + x4 * 4 + 3] = pT[y2 * strideD + x2 * 4 + 3];
                                                    }
                                                    else if (x4 < b1.Width && y4 < b1.Height && (x2 >= wdth || y2 >= hght))
                                                    {
                                                        if (x2 >= wdth)
                                                            x2 -= xP - (xP - x);
                                                        if (y2 >= hght)
                                                            y2 -= yP - (yP - y);

                                                        p1[y4 * strideB + x4 * 4] = p[y2 * strideD + x2 * 4];
                                                        p1[y4 * strideB + x4 * 4 + 1] = p[y2 * strideD + x2 * 4 + 1];
                                                        p1[y4 * strideB + x4 * 4 + 2] = p[y2 * strideD + x2 * 4 + 2];
                                                        p1[y4 * strideB + x4 * 4 + 3] = p[y2 * strideD + x2 * 4 + 3];

                                                        p2[y4 * strideB + x4 * 4] = pT[y2 * strideD + x2 * 4];
                                                        p2[y4 * strideB + x4 * 4 + 1] = pT[y2 * strideD + x2 * 4 + 1];
                                                        p2[y4 * strideB + x4 * 4 + 2] = pT[y2 * strideD + x2 * 4 + 2];
                                                        p2[y4 * strideB + x4 * 4 + 3] = pT[y2 * strideD + x2 * 4 + 3];
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    b2.UnlockBits(b2D);
                                    b1.UnlockBits(b1D);
                                    cfopTrimap.UnlockBits(bmT);
                                    cfopBmp.UnlockBits(bmD);

                                    bmp.Add(b1);
                                    bmp2.Add(b2);

                                    //try
                                    //{
                                    //    Form fff = new Form();
                                    //    fff.BackgroundImage = b1;
                                    //    fff.BackgroundImageLayout = ImageLayout.Zoom;
                                    //    fff.ShowDialog();
                                    //    fff.BackgroundImage = b2;
                                    //    fff.BackgroundImageLayout = ImageLayout.Zoom;
                                    //    fff.ShowDialog();
                                    //}
                                    //catch (Exception exc)
                                    //{
                                    //    Console.WriteLine(exc.ToString());
                                    //}
                                }
                            }

                            if (verifyTrimaps)
                            {
                                if (!CheckTrimaps(bmp2))
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        if (this._frmInfo == null || this._frmInfo.IsDisposed)
                                            this._frmInfo = new frmInfo();
                                        this._frmInfo.Show(trimapProblemMessage);
                                    }));

                                    int x = j % n * www;
                                    int y = j / n * hhh;

                                    //int x = i % n;
                                    //x * www - (x == 0 ? 0 : overlap)

                                    if (x > 0)
                                        x -= overlap;

                                    if (y > 0)
                                        y -= overlap;

                                    this._trimapProblemInfos.Add(new TrimapProblemInfo(id, j, x, y, bmpF[j].Width, bmpF[j].Height, overlap));
                                }
                            }

                            Bitmap[] bmp4 = new Bitmap[bmp.Count];

                            this._cfopArray = new ClosedFormMatteOp[bmp.Count];

                            if (_cfop.BlendParameters != null && AvailMem.AvailMem.checkAvailRam(w * h * bmp.Count * 10L) && !forceSerial)
                                Parallel.For(0, bmp.Count, i =>
                                //for(int i = 0; i < bmp.Count; i++)
                                {
                                    ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                                    BlendParameters bParam = new BlendParameters();
                                    bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                                    bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                                    bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                                    bParam.Sleep = _cfop.BlendParameters.Sleep;
                                    bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                                    bParam.BGW = this.backgroundWorker1;
                                    cfop.BlendParameters = bParam;

                                    cfop.ShowProgess += Cfop_UpdateProgress;
                                    cfop.ShowInfo += _cfop_ShowInfo;

                                    this._cfopArray[i] = cfop;

                                    cfop.GetMattingLaplacian(Math.Pow(10, -7));
                                    Bitmap? b = null;

                                    if (mode == 0)
                                        b = cfop.SolveSystemGaussSeidel();
                                    else if (mode == 1)
                                        b = cfop.SolveSystemGMRES();

                                    //save and draw out later serially
                                    if (b != null)
                                        bmp4[i] = b;

                                    cfop.ShowProgess -= Cfop_UpdateProgress;
                                    cfop.ShowInfo -= _cfop_ShowInfo;
                                    cfop.Dispose();
                                });
                            else
                                if (_cfop.BlendParameters != null)
                                for (int i = 0; i < bmp.Count; i++)
                                {
                                    _cfop_ShowInfo(this, "pic " + (i + 1).ToString());
                                    ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                                    BlendParameters bParam = new BlendParameters();
                                    bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                                    bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                                    bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                                    bParam.Sleep = _cfop.BlendParameters.Sleep;
                                    bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                                    bParam.BGW = this.backgroundWorker1;
                                    cfop.BlendParameters = bParam;

                                    cfop.ShowProgess += Cfop_UpdateProgress;
                                    cfop.ShowInfo += _cfop_ShowInfo;

                                    this._cfopArray[i] = cfop;

                                    cfop.GetMattingLaplacian(Math.Pow(10, -7));
                                    Bitmap? b = null;

                                    if (mode == 0)
                                        b = cfop.SolveSystemGaussSeidel();
                                    else if (mode == 1)
                                        b = cfop.SolveSystemGMRES();

                                    //save and draw out later serially
                                    if (b != null)
                                        bmp4[i] = b;

                                    cfop.ShowProgess -= Cfop_UpdateProgress;
                                    cfop.ShowInfo -= _cfop_ShowInfo;
                                    cfop.Dispose();
                                }

                            if (this._cfop.BlendParameters != null && this._cfop.BlendParameters.BGW != null && this._cfop.BlendParameters.BGW.WorkerSupportsCancellation && this._cfop.BlendParameters.BGW.CancellationPending)
                            {
                                for (int i = bmp.Count - 1; i >= 0; i--)
                                {
                                    if (bmp[i] != null)
                                        bmp[i].Dispose();
                                    if (bmp2[i] != null)
                                        bmp2[i].Dispose();
                                    if (bmp4[i] != null)
                                        bmp4[i].Dispose();
                                }
                                e.Result = null;
                                return;
                            }

                            for (int y = 0; y < yP; y++)
                            {
                                for (int x = 0; x < xP; x++)
                                {
                                    int indx = y * xP + x;
                                    int wdth = result.Width;
                                    int hght = result.Height;

                                    BitmapData bmR = result.LockBits(new Rectangle(0, 0, wdth, hght), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                                    BitmapData b4D = bmp4[indx].LockBits(new Rectangle(0, 0, sizes[indx].Width, sizes[indx].Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                                    int strideR = bmR.Stride;
                                    int strideB = b4D.Stride;

                                    unsafe
                                    {
                                        byte* p = (byte*)bmR.Scan0;
                                        byte* pB = (byte*)b4D.Scan0;

                                        if (group)
                                        {
                                            for (int y2 = y * groupAmountY, y4 = 0; y2 < hght + yP * groupAmountY; y2 += yP * groupAmountY, y4 += groupAmountY)
                                            {
                                                for (int x2 = x * groupAmountX, x4 = 0; x2 < wdth + xP * groupAmountX; x2 += xP * groupAmountX, x4 += groupAmountX)
                                                {
                                                    for (int y7 = y2, y41 = y4, cntY = 0; y7 < y2 + groupAmountY; y7++, y41++)
                                                    {
                                                        for (int x7 = x2, x41 = x4, cntX = 0; x7 < x2 + groupAmountX; x7++, x41++)
                                                        {
                                                            if (x7 < wdth - ww * xP && y7 < hght - hh * yP && x41 < b4D.Width && y41 < b4D.Height)
                                                            {
                                                                p[y7 * strideR + x7 * 4] = pB[y41 * strideB + x41 * 4];
                                                                p[y7 * strideR + x7 * 4 + 1] = pB[y41 * strideB + x41 * 4 + 1];
                                                                p[y7 * strideR + x7 * 4 + 2] = pB[y41 * strideB + x41 * 4 + 2];
                                                                p[y7 * strideR + x7 * 4 + 3] = pB[y41 * strideB + x41 * 4 + 3];
                                                            }
                                                            else
                                                            {
                                                                if (x7 > wdth)
                                                                {
                                                                    x7 = wdth - 1 - (ww * (xP - x)) + cntX - 1;
                                                                    x41 = b4D.Width - 1 - ww + cntX;
                                                                    cntX++;
                                                                }
                                                                if (y7 >= hght)
                                                                {
                                                                    y7 = hght - 1 - (hh * (yP - y)) + cntY - 1;
                                                                    y41 = b4D.Height - 1 - hh + cntY;
                                                                    cntY++;
                                                                }

                                                                if (x7 < wdth && y7 < hght && x41 < b4D.Width && y41 < b4D.Height)
                                                                {
                                                                    p[y7 * strideR + x7 * 4] = pB[y41 * strideB + x41 * 4];
                                                                    p[y7 * strideR + x7 * 4 + 1] = pB[y41 * strideB + x41 * 4 + 1];
                                                                    p[y7 * strideR + x7 * 4 + 2] = pB[y41 * strideB + x41 * 4 + 2];
                                                                    p[y7 * strideR + x7 * 4 + 3] = pB[y41 * strideB + x41 * 4 + 3];
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int y2 = y, y4 = 0; y2 < hght + yP; y2 += yP, y4++)
                                            {
                                                for (int x2 = x, x4 = 0; x2 < wdth + xP; x2 += xP, x4++)
                                                {
                                                    if (x4 < b4D.Width && y4 < b4D.Height && x2 < wdth && y2 < hght)
                                                    {
                                                        p[y2 * strideR + x2 * 4] = pB[y4 * strideB + x4 * 4];
                                                        p[y2 * strideR + x2 * 4 + 1] = pB[y4 * strideB + x4 * 4 + 1];
                                                        p[y2 * strideR + x2 * 4 + 2] = pB[y4 * strideB + x4 * 4 + 2];
                                                        p[y2 * strideR + x2 * 4 + 3] = pB[y4 * strideB + x4 * 4 + 3];
                                                    }
                                                    else if (x4 < b4D.Width && y4 < b4D.Height && (x2 >= wdth || y2 >= hght))
                                                    {
                                                        if (x2 >= wdth)
                                                            x2 -= xP - (xP - x);
                                                        if (y2 >= hght)
                                                            y2 -= yP - (yP - y);

                                                        p[y2 * strideR + x2 * 4] = pB[y4 * strideB + x4 * 4];
                                                        p[y2 * strideR + x2 * 4 + 1] = pB[y4 * strideB + x4 * 4 + 1];
                                                        p[y2 * strideR + x2 * 4 + 2] = pB[y4 * strideB + x4 * 4 + 2];
                                                        p[y2 * strideR + x2 * 4 + 3] = pB[y4 * strideB + x4 * 4 + 3];
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    bmp4[indx].UnlockBits(b4D);
                                    result.UnlockBits(bmR);
                                }
                            }

                            for (int i = bmp.Count - 1; i >= 0; i--)
                            {
                                bmp[i].Dispose();
                                bmp2[i].Dispose();
                                bmp4[i].Dispose();
                            }

                            if (interpolated)
                            {
                                using (Bitmap resOld = result)
                                {
                                    result = new Bitmap(bmpF[j].Width, bmpF[j].Height);

                                    using (Graphics gx = Graphics.FromImage(result))
                                    {
                                        gx.InterpolationMode = InterpolationMode.HighQualityBilinear;
                                        gx.DrawImage(resOld, 0, 0, result.Width, result.Height);
                                    }
                                }

                                cfopBmp.Dispose();
                                cfopTrimap.Dispose();
                            }

                            bmpF4[j] = result;
                        }

                        //draw to single pic
                        for (int i = 0; i < bmpF4.Length; i++)
                        {
                            int x = i % n;
                            int y = i / n;

                            using (Graphics gx = Graphics.FromImage(bmpResult))
                                gx.DrawImage(bmpF4[i], x * www - (x == 0 ? 0 : overlap), y * hhh - (y == 0 ? 0 : overlap));
                        }

                        for (int i = bmpF.Count - 1; i >= 0; i--)
                        {
                            if (bmpF[i] != null)
                                bmpF[i].Dispose();
                            if (bmpF2[i] != null)
                                bmpF2[i].Dispose();
                            if (bmpF4[i] != null)
                                bmpF4[i].Dispose();
                        }

                        e.Result = bmpResult;
                    }
                }
            }
            else
                e.Result = null;
        }

        private bool CheckTrimaps(List<Bitmap> bmp2, int www, int hhh, int n, int id, int overlap)
        {
            bool result = true;
            if (bmp2 != null && bmp2.Count > 0)
            {
                for (int i = 0; i < bmp2.Count; i++)
                {
                    if (!CheckTrimap(bmp2[i]))
                    {
                        result = false;
                        //}
                        int x = i % n * www;
                        int y = i / n * hhh;

                        //int x = i % n;
                        //x * www - (x == 0 ? 0 : overlap)

                        if (x > 0)
                            x -= overlap;

                        if (y > 0)
                            y -= overlap;

                        this._trimapProblemInfos.Add(new TrimapProblemInfo(id, i, x, y, bmp2[i].Width, bmp2[i].Height, overlap));
                    }
                }
            }

            return result;
        }

        //no overlap, we dont have an outerArray and all inner pics resemble the whole pic
        private bool CheckTrimaps(List<Bitmap> bmp2, int www, int hhh, int n, int id, int xAdd, int yAdd)
        {
            bool result = true;
            if (bmp2 != null && bmp2.Count > 0)
            {
                for (int i = 0; i < bmp2.Count; i++)
                {
                    if (!CheckTrimap(bmp2[i]))
                    {
                        result = false;
                        //}
                        int x = i % n * www;
                        int y = i / n * hhh;

                        this._trimapProblemInfos.Add(new TrimapProblemInfo(id, i, x, y, bmp2[i].Width, bmp2[i].Height, 0));
                    }
                }
            }

            return result;
        }

        private bool CheckTrimaps(List<Bitmap> bmp2)
        {
            if (bmp2 != null && bmp2.Count > 0)
            {
                foreach (Bitmap b in bmp2)
                {
                    if (!CheckTrimap(b))
                        return false;
                }
            }

            return true;
        }

        private unsafe bool CheckTrimap(Bitmap b)
        {
            int w = b.Width;
            int h = b.Height;
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmData.Stride;

            //if (this._rnd == null)
            //    this._rnd = new Random();

            bool unknownFound = false;
            int bgCount = 0;
            int fgCount = 0;

            byte* p = (byte*)bmData.Scan0;

            //first check, if unknown pixels are present
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                {
                    if (p[0] > 25 && p[0] < 230)
                        unknownFound = true;

                    if (p[0] <= 25)
                        bgCount++;

                    if (p[0] >= 230)
                        fgCount++;

                    p += 4;
                }

            //p = (byte*)bmData.Scan0;

            //if (unknownFound && (bgCount == 0 || fgCount == 0))
            //{
            //    //just a test, if this idea works at this stage of dev
            //    int cnt = 0;
            //    int iterations = 0;

            //    //todo: check pixels in orig img for determining fg/bg, or get a region for writing bg/fg, write pixels in clusters

            //    while(cnt < 128 && iterations < 10000)
            //    {
            //        int x = _rnd.Next(w);
            //        int y = _rnd.Next(h);

            //        if (p[x * 4 + y * stride] > 25 && p[x * 4 + y * stride] < 230) // only change unknown pixels
            //        {
            //            if ((cnt & 0x01) == 1)
            //                p[x * 4 + y * stride] = p[x * 4 + y * stride + 1] = p[x * 4 + y * stride + 2] = 0;
            //            else
            //                p[x * 4 + y * stride] = p[x * 4 + y * stride + 1] = p[x * 4 + y * stride + 2] = 255;

            //            cnt++;
            //        }

            //        iterations++;
            //    }
            //}

            b.UnlockBits(bmData);

            return !(unknownFound && (bgCount == 0 || fgCount == 0));
        }

        private void backgroundWorker1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (!this.toolStripProgressBar1.IsDisposed)
                if (InvokeRequired)
                {
                    try
                    {
                        this.toolStripProgressBar1.Value = e.ProgressPercentage;
                    }
                    catch
                    {

                    }
                }
                else
                {
                    try
                    {
                        this.toolStripProgressBar1.Value = e.ProgressPercentage;
                    }
                    catch
                    {

                    }
                }
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                {
                    bmp = (Bitmap)e.Result;

                    Bitmap? b2 = bmp;
                    bmp = ResampleBack(bmp);
                    b2.Dispose();
                    b2 = null;

                    if (bmp != null)
                    {
                        this._undoOPCache?.Add(bmp);
                        frmEdgePic frm4 = new frmEdgePic(bmp, this.helplineRulerCtrl1.Bmp.Size);
                        frm4.Text = "Alpha Matte";
                        frm4.ShowDialog();

                        if (this.helplineRulerCtrl1.Bmp != null)
                        {
                            b2 = bmp;
                            bmp = GetAlphaBoundsPic(this.helplineRulerCtrl1.Bmp, bmp);
                            b2.Dispose();
                            b2 = null;
                        }
                    }
                }

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                    //Bitmap? bC = new Bitmap(bmp);
                    //this.SetBitmap(ref _bmpRef, ref bC);

                    this.helplineRulerCtrl2.Zoom = this.helplineRulerCtrl1.Zoom;
                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl2.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    _undoOPCache?.Add(bmp);

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                if (this._cfop != null)
                {
                    this._cfop.ShowProgess -= Cfop_UpdateProgress;
                    this._cfop.ShowInfo -= _cfop_ShowInfo;
                    this._cfop.Dispose();
                }

                this.btnGo.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                rbClosedForm_CheckedChanged(this.rbClosedForm, new EventArgs());

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.Timer3.Start();

                this.backgroundWorker1.Dispose();
                this.backgroundWorker1 = new BackgroundWorker();
                this.backgroundWorker1.WorkerReportsProgress = true;
                this.backgroundWorker1.WorkerSupportsCancellation = true;
                this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
                this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
                this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

                this._sw?.Stop();
                this.Text = "frmProcOutline";
                if (this._sw != null)
                    this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
            }
        }

        private void GetTiles(Bitmap bWork, Bitmap trWork, List<Bitmap> bmp, List<Bitmap> bmp2,
            int w, int w2, int h, int h2, int overlap, int n)
        {
            for (int y = 0; y < n; y++)
            {
                for (int x = 0; x < n; x++)
                {
                    if (x < n - 1 && y < n - 1)
                    {
                        Bitmap b1 = new Bitmap(w + overlap * (x == 0 ? 1 : 2), h + overlap * (y == 0 ? 1 : 2));

                        using (Graphics gx = Graphics.FromImage(b1))
                            gx.DrawImage(bWork, -x * w + (x == 0 ? 0 : overlap), -y * h + (y == 0 ? 0 : overlap));

                        bmp.Add(b1);

                        Bitmap b3 = new Bitmap(w + overlap * (x == 0 ? 1 : 2), h + overlap * (y == 0 ? 1 : 2));

                        using (Graphics gx = Graphics.FromImage(b3))
                            gx.DrawImage(trWork, -x * w + (x == 0 ? 0 : overlap), -y * h + (y == 0 ? 0 : overlap));

                        bmp2.Add(b3);
                    }
                    else if (x == n - 1 && y < n - 1)
                    {
                        Bitmap b1 = new Bitmap(w2 + overlap, h + overlap * (y == 0 ? 1 : 2));

                        using (Graphics gx = Graphics.FromImage(b1))
                            gx.DrawImage(bWork, -x * w + (x == 0 ? 0 : overlap), -y * h + (y == 0 ? 0 : overlap));

                        bmp.Add(b1);

                        Bitmap b3 = new Bitmap(w2 + overlap, h + overlap * (y == 0 ? 1 : 2));

                        using (Graphics gx = Graphics.FromImage(b3))
                            gx.DrawImage(trWork, -x * w + (x == 0 ? 0 : overlap), -y * h + (y == 0 ? 0 : overlap));

                        bmp2.Add(b3);
                    }
                    else if (x < n - 1 && y == n - 1)
                    {
                        Bitmap b1 = new Bitmap(w + overlap * (x == 0 ? 1 : 2), h2 + overlap);

                        using (Graphics gx = Graphics.FromImage(b1))
                            gx.DrawImage(bWork, -x * w + (x == 0 ? 0 : overlap), -y * h + overlap);

                        bmp.Add(b1);

                        Bitmap b3 = new Bitmap(w + overlap * (x == 0 ? 1 : 2), h2 + overlap);

                        using (Graphics gx = Graphics.FromImage(b3))
                            gx.DrawImage(trWork, -x * w + (x == 0 ? 0 : overlap), -y * h + overlap);

                        bmp2.Add(b3);
                    }
                    else
                    {
                        Bitmap b1 = new Bitmap(w2 + overlap, h2 + overlap);

                        using (Graphics gx = Graphics.FromImage(b1))
                            gx.DrawImage(bWork, -x * w + overlap, -y * h + overlap);

                        bmp.Add(b1);

                        Bitmap b3 = new Bitmap(w2 + overlap, h2 + overlap);

                        using (Graphics gx = Graphics.FromImage(b3))
                            gx.DrawImage(trWork, -x * w + overlap, -y * h + overlap);

                        bmp2.Add(b3);
                    }
                }

                _cfop_ShowInfo(this, "outer pic-amount " + bmp.Count().ToString());
            }
        }

        private unsafe Bitmap? GetAlphaBoundsPic(Bitmap bmpIn, Bitmap bmpAlpha)
        {
            Bitmap? bmp = null;

            if (AvailMem.AvailMem.checkAvailRam(bmpAlpha.Width * bmpAlpha.Height * 16L))
            {
                int w = bmpAlpha.Width;
                int h = bmpAlpha.Height;

                bmp = new Bitmap(w, h);

                BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmIn = bmpIn.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                BitmapData bmA = bmpAlpha.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    p += y * stride;

                    byte* pIn = (byte*)bmIn.Scan0;
                    pIn += y * stride;

                    byte* pA = (byte*)bmA.Scan0;
                    pA += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        p[0] = pIn[0];
                        p[1] = pIn[1];
                        p[2] = pIn[2];

                        p[3] = pA[0];

                        p += 4;
                        pIn += 4;
                        pA += 4;
                    }
                });

                bmp.UnlockBits(bmD);
                bmpIn.UnlockBits(bmIn);
                bmpAlpha.UnlockBits(bmA);
            }

            return bmp;
        }

        private void numSleep_ValueChanged(object sender, EventArgs e)
        {
            if (this._cfop != null && this._cfop.BlendParameters != null)
            {
                if ((int)this.numSleep.Value == 0)
                    this._cfop.BlendParameters.Sleep = false;
                else
                    this._cfop.BlendParameters.Sleep = true;

                if (this._cfop != null && this._cfop.BlendParameters != null)
                    this._cfop.BlendParameters.SleepAmount = (int)this.numSleep.Value;

                if (this._cfopArray != null && _cfopArray.Length > 0)
                {
                    for (int i = 0; i < _cfopArray.Length; i++)
                        if (_cfopArray[i] != null)
                            try
                            {
                                BlendParameters? cb = _cfopArray[i].BlendParameters;
                                if (cb != null)
                                {
                                    bool b = cb.Sleep;
                                    if ((int)this.numSleep.Value == 0)
                                        b = false;
                                    else
                                        b = true;

                                    cb.SleepAmount = (int)this.numSleep.Value;
                                }
                            }
                            catch
                            {

                            }
                }
            }
        }

        private void btnResVals_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                double d = CheckWidthHeight(this.helplineRulerCtrl1.Bmp, true, (double)this.numMaxSize.Value);
                this.cbHalfSize.Checked = d > 1;
            }
        }

        private void frmClosedFormMatte_Load(object sender, EventArgs e)
        {
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
            rbClosedForm_CheckedChanged(this.rbClosedForm, new EventArgs());
            this._hs = this.cbHalfSize.Checked;

            if (this.helplineRulerCtrl1.Bmp != null)
                this.numMaxSize.Value = (decimal)Math.Max(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
        }

        private void floodBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point pt = this._ptHLC1BG;
            this._ptHLC1BG = this._ptHLC1FGBG;
            this.btnFloodBG.Enabled = true;
            this.btnFloodBG_Click(this.btnFloodBG, new EventArgs());
            this._ptHLC1BG = pt;
        }

        private void floodFGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Point pt = this._ptHLC1FG;
            this._ptHLC1FG = this._ptHLC1FGBG; //!
            this.btnFloodFG.Enabled = true;
            this.btnFloodFG_Click(this.btnFloodFG, new EventArgs());
            this._ptHLC1FG = pt;
        }

        private void cbOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._scribbles != null && this._scribbles.Count > 0)
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void rbClosedForm_CheckedChanged(object sender, EventArgs e)
        {
            this.panel2.Enabled = this.rbClosedForm.Checked;
            this.cbHalfSize_CheckedChanged(this.cbHalfSize, new EventArgs());
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            this.Timer3.Stop();

            if (!this.toolStripProgressBar1.IsDisposed)
            {
                this.toolStripProgressBar1.Visible = false;
                this.toolStripProgressBar1.Value = 0;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }
        }

        private void saveTrimapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
            this.saveFileDialog1.FileName = "Bild1.png";
            if (this.pictureBox1.Image != null && this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                this.pictureBox1.Image.Save(this.saveFileDialog1.FileName, ImageFormat.Png);
        }

        private void btnSetGamma_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker5.IsBusy)
            {
                this.backgroundWorker5.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl2.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnSetGamma.Enabled = true;
                this.btnSetGamma.Text = "Cancel";

                double gamma = (double)this.numGamma.Value;

                this.backgroundWorker5.RunWorkerAsync(new object[] { this.helplineRulerCtrl2.Bmp, gamma });
            }
        }

        private void backgroundWorker5_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = new Bitmap((Bitmap)o[0]);
                double gamma = (double)o[1];

                e.Result = GetAlphaPic(bmp, gamma);
            }
        }

        private unsafe Bitmap? GetAlphaPic(Bitmap bmp, double gamma)
        {
            Bitmap? bOut = null;

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                int w = bmp.Width;
                int h = bmp.Height;

                bOut = new Bitmap(w, h);

                BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmA = bOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    p += y * stride;

                    byte* pA = (byte*)bmA.Scan0;
                    pA += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        if (p[3] != 0)
                        {
                            pA[0] = p[0];
                            pA[1] = p[1];
                            pA[2] = p[2];

                            pA[3] = (byte)Math.Max(Math.Min(255.0 * Math.Pow((double)p[3] / 255.0, gamma), 255), 0);
                        }

                        p += 4;
                        pA += 4;
                    }
                });

                bmp.UnlockBits(bmD);
                bOut.UnlockBits(bmA);
            }

            return bOut;
        }

        private void backgroundWorker5_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                    bmp = (Bitmap)e.Result;

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    _undoOPCache?.Add(bmp);

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                this.btnSetGamma.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.Timer3.Start();

                this.backgroundWorker5.Dispose();
                this.backgroundWorker5 = new BackgroundWorker();
                this.backgroundWorker5.WorkerReportsProgress = true;
                this.backgroundWorker5.WorkerSupportsCancellation = true;
                this.backgroundWorker5.DoWork += backgroundWorker5_DoWork;
                //this.backgroundWorker5.ProgressChanged += backgroundWorker5_ProgressChanged;
                this.backgroundWorker5.RunWorkerCompleted += backgroundWorker5_RunWorkerCompleted;
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoUndo();

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");
                    if (_undoOPCache.CurrentPosition < 1)
                    {
                        this.btnUndo.Enabled = false;
                        this._pic_changed = false;
                    }

                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

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
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");
                    this._pic_changed = true;

                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

                    this.CheckRedoButton();

                    this.btnUndo.Enabled = true;
                }
                else
                    MessageBox.Show("Error while redoing.");
            }
        }

        private void backgroundWorker3_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = new Bitmap((Bitmap)o[0]);
                double alphaTh = (int)o[1];

                e.Result = GetAlphaZAndGainPic(bmp, alphaTh);
            }
        }

        private unsafe Bitmap? GetAlphaZAndGainPic(Bitmap bmp, double alphaTh)
        {
            Bitmap? bOut = null;

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                int w = bmp.Width;
                int h = bmp.Height;

                bOut = new Bitmap(w, h);

                BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmA = bOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                double slope = 255.0 / (255.0 - alphaTh);

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    p += y * stride;

                    byte* pA = (byte*)bmA.Scan0;
                    pA += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        if (p[3] != 0)
                        {
                            pA[0] = p[0];
                            pA[1] = p[1];
                            pA[2] = p[2];

                            pA[3] = (byte)Math.Max(Math.Min(slope * ((double)p[3] - alphaTh), 255), 0);
                        }

                        p += 4;
                        pA += 4;
                    }
                });

                bmp.UnlockBits(bmD);
                bOut.UnlockBits(bmA);
            }

            return bOut;
        }

        private void backgroundWorker3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                    bmp = (Bitmap)e.Result;

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    _undoOPCache?.Add(bmp);

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                this.btnAlphaZAndGain.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.Timer3.Start();

                this.backgroundWorker3.Dispose();
                this.backgroundWorker3 = new BackgroundWorker();
                this.backgroundWorker3.WorkerReportsProgress = true;
                this.backgroundWorker3.WorkerSupportsCancellation = true;
                this.backgroundWorker3.DoWork += backgroundWorker3_DoWork;
                //this.backgroundWorker3.ProgressChanged += backgroundWorker3_ProgressChanged;
                this.backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
            }
        }

        private void btnAlphaZAndGain_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl2.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnAlphaZAndGain.Enabled = true;
                this.btnAlphaZAndGain.Text = "Cancel";

                int alphaTh = (int)this.numAlphaZAndGain.Value;

                this.backgroundWorker3.RunWorkerAsync(new object[] { this.helplineRulerCtrl2.Bmp, alphaTh });
            }
        }

        private void cbHalfSize_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            this.btnGo.Enabled = c.Checked == this._hs && this.pictureBox1.Image != null;
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                double factor = (double)o[0];
                Bitmap bTrimap = (Bitmap)o[1];
                bool drawPaths = (bool)o[2];
                float wFactor = (float)o[3];

                using (Graphics gx = Graphics.FromImage(bTrimap))
                {
                    gx.SmoothingMode = SmoothingMode.None;
                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;

                    if (this._scribbles != null)
                    {
                        if (this.rbFullScribble.Checked)
                        {
                            gx.Clear(Color.Black);

                            if (this._scribbleSeq != null && this._scribbleSeq.Count > 0)
                            {
                                foreach (Tuple<int, int, int, bool, List<List<Point>>> f in this._scribbleSeq)
                                {
                                    int l = f.Item1;
                                    int wh = f.Item2;
                                    int listNo = f.Item3;

                                    if (this._scribbles.ContainsKey(l) && this._scribbles[l].ContainsKey(wh))
                                    {
                                        List<List<Point>> ptsList = this._scribbles[l][wh];

                                        if (ptsList != null && ptsList.Count > listNo)
                                        {
                                            bool doRect = ptsList[listNo].Count > 1;

                                            Color c = l == 0 ? Color.Black : l == 1 ? Color.White : Color.Gray;

                                            if (doRect)
                                            {
                                                foreach (Point pt in ptsList[listNo])
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                        gx.FillRectangle(sb, new Rectangle(
                                                            (int)((int)(pt.X - wh / 2) / factor),
                                                            (int)((int)(pt.Y - wh / 2) / factor),
                                                            (int)(wh / factor),
                                                            (int)(wh / factor)));
                                                    if (l == 1)
                                                        using (Pen pen = new Pen(c, (factor > 1) ? 2 : 1))
                                                            gx.DrawRectangle(pen, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2) / factor),
                                                                (int)((int)(pt.Y - wh / 2) / factor),
                                                                (int)(wh / factor),
                                                                (int)(wh / factor)));
                                                }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using SolidBrush sb = new SolidBrush(c);
                                                    using Pen pen = new Pen(c, (wh * wFactor) / (float)factor);
                                                    pen.LineJoin = LineJoin.Round;
                                                    using GraphicsPath gP = new GraphicsPath();
                                                    gP.AddLines(ptsList[listNo].Select(a => new PointF(a.X, a.Y)).ToArray());
                                                    using Matrix mx = new Matrix(1.0f / (float)factor, 0, 0, 1.0f / (float)factor, 0, 0);
                                                    gP.Transform(mx);
                                                    gx.DrawPath(pen, gP);
                                                }
                                            }
                                            else
                                            {
                                                if (ptsList[listNo].Count > 0)
                                                {
                                                    Point pt = ptsList[listNo][0];
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                        gx.FillRectangle(sb, new Rectangle(
                                                            (int)((int)(pt.X - wh / 2) / factor),
                                                            (int)((int)(pt.Y - wh / 2) / factor),
                                                            (int)(wh / factor),
                                                            (int)(wh / factor)));
                                                    if (l == 1)
                                                        using (Pen pen = new Pen(c, (factor > 1) ? 2 : 1))
                                                            gx.DrawRectangle(pen, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2) / factor),
                                                                (int)((int)(pt.Y - wh / 2) / factor),
                                                                (int)(wh / factor),
                                                                (int)(wh / factor)));
                                                }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using SolidBrush sb = new SolidBrush(c);
                                                    using GraphicsPath gP = new GraphicsPath();
                                                    gP.AddEllipse(ptsList[listNo][0].X - wh / 2f / (float)factor,
                                                        ptsList[listNo][0].Y - wh / 2f / (float)factor,
                                                        wh / (float)factor, wh / (float)factor);
                                                    gx.FillPath(sb, gP);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this._scribbles.ContainsKey(1))
                                {
                                    Dictionary<int, List<List<Point>>> z = this._scribbles[1];

                                    if (z != null)
                                    {
                                        foreach (int i in z.Keys)
                                        {
                                            List<List<Point>> list = z[i];
                                            int wh = (int)Math.Max((i / (factor == 0 ? 1 : factor)), 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X / factor), (int)(a.Y / factor))).ToList();

                                                foreach (Point pt in pts)
                                                {
                                                    gx.FillRectangle(Brushes.White, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                    using Pen pen = new(Color.White, (factor > 1) ? 2 : 1);
                                                    gx.DrawRectangle(pen, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                }
                                            }

                                        }
                                    }
                                }

                                if (this._scribbles.ContainsKey(3))
                                {
                                    Dictionary<int, List<List<Point>>> z = this._scribbles[3];

                                    if (z != null)
                                    {
                                        foreach (int i in z.Keys)
                                        {
                                            List<List<Point>> list = z[i];
                                            int wh = (int)Math.Max((i / (factor == 0 ? 1 : factor)), 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X / factor), (int)(a.Y / factor))).ToList();

                                                foreach (Point pt in pts)
                                                    gx.FillRectangle(Brushes.Gray, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (this.rbSparseScribble.Checked)
                        {
                            gx.Clear(Color.Gray);

                            if (this._scribbleSeq != null && this._scribbleSeq.Count > 0)
                            {
                                foreach (Tuple<int, int, int, bool, List<List<Point>>> f in this._scribbleSeq)
                                {
                                    int l = f.Item1;
                                    int wh = f.Item2;
                                    int listNo = f.Item3;

                                    if (this._scribbles.ContainsKey(l) && this._scribbles[l].ContainsKey(wh))
                                    {
                                        List<List<Point>> ptsList = this._scribbles[l][wh];

                                        if (ptsList != null && ptsList.Count > listNo)
                                        {
                                            bool doRect = ptsList[listNo].Count > 1;

                                            Color c = l == 0 ? Color.Black : l == 1 ? Color.White : Color.Gray;

                                            if (doRect)
                                            {
                                                foreach (Point pt in ptsList[listNo])
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                        gx.FillRectangle(sb, new Rectangle(
                                                            (int)((int)(pt.X - wh / 2) / factor),
                                                            (int)((int)(pt.Y - wh / 2) / factor),
                                                        (int)(wh / factor),
                                                            (int)(wh / factor)));
                                                    if (l == 1)
                                                        using (Pen pen = new Pen(c, (factor > 1) ? 2 : 1))
                                                            gx.DrawRectangle(pen, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2) / factor),
                                                                (int)((int)(pt.Y - wh / 2) / factor),
                                                                (int)(wh / factor),
                                                                (int)(wh / factor)));
                                                }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using SolidBrush sb = new SolidBrush(c);
                                                    using Pen pen = new Pen(c, (wh * wFactor) / (float)factor);
                                                    pen.LineJoin = LineJoin.Round;
                                                    using GraphicsPath gP = new GraphicsPath();
                                                    gP.AddLines(ptsList[listNo].Select(a => new PointF(a.X, a.Y)).ToArray());
                                                    using Matrix mx = new Matrix(1.0f / (float)factor, 0, 0, 1.0f / (float)factor, 0, 0);
                                                    gP.Transform(mx);
                                                    gx.DrawPath(pen, gP);
                                                }
                                            }
                                            else
                                            {
                                                if (ptsList[listNo].Count > 0)
                                                {
                                                    Point pt = ptsList[listNo][0];
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                        gx.FillRectangle(sb, new Rectangle(
                                                            (int)((int)(pt.X - wh / 2) / factor),
                                                            (int)((int)(pt.Y - wh / 2) / factor),
                                                            (int)(wh / factor),
                                                            (int)(wh / factor)));
                                                    if (l == 1)
                                                        using (Pen pen = new Pen(c, (factor > 1) ? 2 : 1))
                                                            gx.DrawRectangle(pen, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2) / factor),
                                                                (int)((int)(pt.Y - wh / 2) / factor),
                                                                (int)(wh / factor),
                                                                (int)(wh / factor)));
                                                }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using SolidBrush sb = new SolidBrush(c);
                                                    using GraphicsPath gP = new GraphicsPath();
                                                    gP.AddEllipse(ptsList[listNo][0].X - wh / 2f / (float)factor,
                                                        ptsList[listNo][0].Y - wh / 2f / (float)factor,
                                                        wh / (float)factor, wh / (float)factor);
                                                    gx.FillPath(sb, gP);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this._scribbles.ContainsKey(0))
                                {
                                    Dictionary<int, List<List<Point>>> z = this._scribbles[0];

                                    if (z != null)
                                    {
                                        foreach (int i in z.Keys)
                                        {
                                            List<List<Point>> list = z[i];
                                            int wh = (int)Math.Max((i / (factor == 0 ? 1 : factor)), 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X / factor), (int)(a.Y / factor))).ToList();

                                                foreach (Point pt in pts)
                                                    gx.FillRectangle(Brushes.Black, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                            }

                                        }
                                    }
                                }

                                if (this._scribbles.ContainsKey(1))
                                {
                                    Dictionary<int, List<List<Point>>> z = this._scribbles[1];

                                    if (z != null)
                                    {
                                        foreach (int i in z.Keys)
                                        {
                                            List<List<Point>> list = z[i];
                                            int wh = (int)Math.Max((i / (factor == 0 ? 1 : factor)), 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X / factor), (int)(a.Y / factor))).ToList();

                                                foreach (Point pt in pts)
                                                {
                                                    gx.FillRectangle(Brushes.White, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                    using Pen pen = new(Color.White, (factor > 1) ? 2 : 1);
                                                    gx.DrawRectangle(pen, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        //if (this._oldUnknownFT != null)
                        //    this.FillUnknownByBG(bTrimap, this._oldUnknownFT);
                    }
                }

                e.Result = bTrimap;
            }
        }

        private void backgroundWorker4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap bTrimap = (Bitmap)e.Result;
                if (bTrimap != null)
                {
                    Image? iOld = this.pictureBox1.Image;
                    this.pictureBox1.Image = bTrimap;
                    if (iOld != null)
                        iOld.Dispose();
                    iOld = null;

                    this.btnGo.Enabled = true;
                }
            }

            this.Cursor = Cursors.Default;
            this.SetControls(true);

            this._hs = this.cbHalfSize.Checked;
            rbClosedForm_CheckedChanged(this.rbClosedForm, new EventArgs());

            this.btnGenerateTrimap.Text = "gen trimap";

            this.btnOK.Enabled = this.btnCancel.Enabled = true;

            this.backgroundWorker4.Dispose();
            this.backgroundWorker4 = new BackgroundWorker();
            this.backgroundWorker4.WorkerReportsProgress = true;
            this.backgroundWorker4.WorkerSupportsCancellation = true;
            this.backgroundWorker4.DoWork += backgroundWorker4_DoWork;
            //this.backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
            this.backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
        }

        private void cbHighlight_CheckedChanged(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            GetAlphaMatte.frmEdgePic frm4 = new GetAlphaMatte.frmEdgePic(this.pictureBox1.Image, this.helplineRulerCtrl1.Bmp.Size);
            frm4.Text = "Trimap";
            frm4.ShowDialog();
        }

        private void btnCMNew_Click(object sender, EventArgs e)
        {
            this._ptSt = null;
        }
    }
}
