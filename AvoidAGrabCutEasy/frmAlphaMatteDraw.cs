using Cache;
using ChainCodeFinder;
using OutlineOperations;
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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using GetAlphaMatte;
using ColorCurvesAlpha;

namespace AvoidAGrabCutEasy
{
    public partial class frmAlphaMatteDraw : Form
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

        public Dictionary<int, List<List<Point>>>? UnknownScibblesBU { get; set; }

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
        //private List<int> _lastDraw;
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
        private Bitmap? _bWork;
        private List<Tuple<int, int, int, bool, List<List<Point>>>> _scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();
        private Point _ptHLC1FGBG;
        //private frmInfo2 _frmInfo2;
        private bool _hs;
        private Point? _ptSt;
        private List<Point>? _ptPrev;
        private Bitmap? _bmpMatte;
        private Bitmap? _oldUnknownFT;
        private Bitmap? _bmpOutline;
        private Bitmap? _bmpMatteOrigSize;
        private Bitmap? _resMatte;
        private bool _dynamic = true;
        private int _divisor = 2;
        private int _newWidth = 10;
        private int _minSize = 84;
        private List<Point>? _exclLocations;
        private List<Bitmap>? _excludedRegions;

        private List<Point> _pointsDraw = new List<Point>();
        private Bitmap? _bWorkPart;
        private bool _lbGo;
        private List<Point>? _ptBU;
        private Bitmap? _fgMap;

        public frmAlphaMatteDraw()
        {
            InitializeComponent();
        }

        public frmAlphaMatteDraw(Bitmap bmp, string basePathAddition)
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

            this.helplineRulerCtrl2.dbPanel1.MouseDown += helplineRulerCtrl2_MouseDown;
            this.helplineRulerCtrl2.dbPanel1.MouseMove += helplineRulerCtrl2_MouseMove;
            this.helplineRulerCtrl2.dbPanel1.MouseUp += helplineRulerCtrl2_MouseUp;

            this.helplineRulerCtrl2.PostPaint += helplineRulerCtrl2_Paint;

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

        private void helplineRulerCtrl2_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && this.labelGo.Enabled)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

                this.helplineRulerCtrl2.dbPanel1.Capture = true;

                if (ix >= 0 && ix < this.helplineRulerCtrl2.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl2.Bmp.Height)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        this._pointsDraw.Clear();
                        this._pointsDraw.Add(new Point(ix, iy));
                        this._tracking4 = true;
                    }
                }
            }
        }

        private void helplineRulerCtrl2_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && this.labelGo.Enabled)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

                if (ix >= this.helplineRulerCtrl2.Bmp.Width)
                    ix = this.helplineRulerCtrl2.Bmp.Width - 1;
                if (iy >= this.helplineRulerCtrl2.Bmp.Height)
                    iy = this.helplineRulerCtrl2.Bmp.Height - 1;

                if (ix >= 0 && ix < this.helplineRulerCtrl2.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl2.Bmp.Height)
                {
                    this._eX2 = e.X;
                    this._eY2 = e.Y;

                    if (this._tracking4)
                        this._pointsDraw.Add(new Point(ix, iy));

                    Color c = this.helplineRulerCtrl2.Bmp.GetPixel(ix, iy);
                    this.toolStripStatusLabel1.Text = ix.ToString() + "; " + iy.ToString();
                    this.ToolStripStatusLabel2.BackColor = c;

                    //if (this._tracking4)
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();
                }
            }
        }

        private void helplineRulerCtrl2_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && this.labelGo.Enabled)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

                if (ix >= this.helplineRulerCtrl2.Bmp.Width)
                    ix = this.helplineRulerCtrl2.Bmp.Width - 1;
                if (iy >= this.helplineRulerCtrl2.Bmp.Height)
                    iy = this.helplineRulerCtrl2.Bmp.Height - 1;

                if (ix >= 0 && ix < this.helplineRulerCtrl2.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl2.Bmp.Height)
                {
                    if (this._tracking4)
                    {
                        this._pointsDraw.Add(new Point(ix, iy));
                        ProcessPicture();
                    }

                    this.helplineRulerCtrl2.dbPanel1.Invalidate();
                }
                else
                {
                    if (this._tracking4)
                        ProcessPicture();

                    this.helplineRulerCtrl2.dbPanel1.Invalidate();
                }
            }

            this._tracking4 = false;
            this.helplineRulerCtrl2.dbPanel1.Capture = false;
        }

        private void ProcessPicture()
        {
            if (this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.CancelAsync();
                return;
            }

            //if (this.rbFullScribble.Checked)
            {
                if (this.helplineRulerCtrl1.Bmp != null && this.pictureBox1.Image != null && this._bWork != null)
                {
                    this.Cursor = Cursors.WaitCursor;
                    this.SetControls(false);

                    this.btnCancelBGW.Visible = this.btnCancelBGW.Enabled = true;

                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    this.labelGo.Enabled = this.numDrawPenWidth.Enabled = this._lbGo = true;

                    this.numSleep.Enabled = this.label2.Enabled = this.numError.Enabled = this.label54.Enabled = true;

                    if (_sw == null)
                        _sw = new Stopwatch();
                    _sw.Reset();
                    _sw.Start();

                    this.btnOK.Enabled = this.btnCancel.Enabled = false;
                    bool hs = this.cbHalfSize.Checked;

                    //pointsDraw to path, get pictures
                    using GraphicsPath gP = new();
                    gP.AddLines(this._pointsDraw.Select(a => new PointF(a.X, a.Y)).ToArray());
                    RectangleF rTmp = gP.GetBounds();
                    Rectangle rClone = new Rectangle(
                        Math.Max((int)Math.Floor((rTmp.X - (float)this.numDrawPenWidth.Value / 2f) / (hs ? 2f : 1)), 0),
                        Math.Max((int)Math.Floor((rTmp.Y - (float)this.numDrawPenWidth.Value / 2f) / (hs ? 2f : 1)), 0),
                        Math.Min((int)Math.Ceiling((rTmp.Width + (float)this.numDrawPenWidth.Value) / (hs ? 2f : 1)), this.helplineRulerCtrl2.Bmp.Width - 1),
                        Math.Min((int)Math.Ceiling((rTmp.Height + (float)this.numDrawPenWidth.Value) / (hs ? 2f : 1)), this.helplineRulerCtrl2.Bmp.Height - 1));

                    Rectangle rClone2 = new Rectangle(
                        Math.Max((int)Math.Floor((rTmp.X - (float)this.numDrawPenWidth.Value / 2f)), 0),
                        Math.Max((int)Math.Floor((rTmp.Y - (float)this.numDrawPenWidth.Value / 2f)), 0),
                        Math.Min((int)Math.Ceiling((rTmp.Width + (float)this.numDrawPenWidth.Value)), this.helplineRulerCtrl2.Bmp.Width - 1),
                        Math.Min((int)Math.Ceiling((rTmp.Height + (float)this.numDrawPenWidth.Value)), this.helplineRulerCtrl2.Bmp.Height - 1));

                    using Bitmap bWorkTmp = (Bitmap)this._bWork.Clone();

                    //evtl für die folgenden Graphics-objekte pixeloffsetMode auf half setzen
                    Bitmap? bC = new Bitmap(rClone2.Width, rClone2.Height);
                    using Graphics gxC = Graphics.FromImage(bC);
                    gxC.DrawImage(this.helplineRulerCtrl1.Bmp, -rClone2.X, -rClone2.Y); //note we load hlc1.Bmp!

                    this.SetBitmap(ref this._bWorkPart, ref bC);

                    Image img = this.pictureBox1.Image;
                    using Bitmap trWorkTmp = (Bitmap)img.Clone();

                    Bitmap bWork = new Bitmap(rClone.Width, rClone.Height);
                    Bitmap trWork = new Bitmap(rClone.Width, rClone.Height);

                    if (this.rbSparseScribble.Checked)
                    {
                        Bitmap? bOld = bWork;
                        Bitmap? trOld = trWork;
                        bWork = new Bitmap(rClone.Width + 100, rClone.Height + 100);
                        trWork = new Bitmap(rClone.Width + 100, rClone.Height + 100);
                        if (bOld != null)
                            bOld.Dispose();
                        bOld = null;
                        if (trOld != null)
                            trOld.Dispose();
                        trOld = null;

                        //get some more trimap info in the working pic
                        using Bitmap fg = ExtractFG(trWorkTmp);
                        using Bitmap bg = ExtractBG(trWorkTmp);

                        DrawFGPixels(bWork, fg, this.helplineRulerCtrl1.Bmp, 100, 100);
                        DrawBGPixels(bWork, bg, this.helplineRulerCtrl1.Bmp, 100, 100);

                        DrawFGPixels(trWork, fg, trWorkTmp, 100, 100);
                        DrawBGPixels(trWork, bg, trWorkTmp, 100, 100);
                    }

                    using Graphics gxW = Graphics.FromImage(bWork);
                    gxW.DrawImage(bWorkTmp, -rClone.X, -rClone.Y);
                    using Graphics gxT = Graphics.FromImage(trWork);
                    gxT.DrawImage(trWorkTmp, -rClone.X, -rClone.Y);

                    if (this.cbWOScribbles.Checked)
                    {
                        //draw trimap and pic to process with a small added boudary
                        float scale = hs ? 0.5f : 1f;
                        int wh = Math.Max((int)((float)this.numDrawPenWidth.Value * scale), 1);

                        using Pen p4 = new Pen(Color.Gray, wh);
                        p4.LineJoin = LineJoin.Round;
                        p4.StartCap = LineCap.Round;
                        p4.EndCap = LineCap.Round;

                        using GraphicsPath gPU = (GraphicsPath)gP.Clone();
                        RectangleF rr = gPU.GetBounds();
                        using Matrix mxU = new Matrix(1, 0, 0, 1, -rTmp.X - rr.Width / 2, -rTmp.Y - rr.Height / 2);
                        gPU.Transform(mxU);
                        using Matrix mxU2 = new Matrix(scale, 0, 0, scale, 0, 0);
                        gPU.Transform(mxU2);
                        RectangleF rr2 = gPU.GetBounds();
                        using Matrix mxU3 = new Matrix(1, 0, 0, 1, rr2.Width / 2 + wh / 2, rr2.Height / 2 + wh / 2);
                        gPU.Transform(mxU3);

                        Bitmap? bOld = bWork;
                        bWork = new Bitmap(rClone.Width + 20, rClone.Height + 20);
                        using Bitmap trWorkI = new Bitmap(bWork.Width, bWork.Height);
                        if (bOld != null)
                            bOld.Dispose();
                        bOld = null;

                        using Graphics gxW2 = Graphics.FromImage(bWork);
                        gxW2.DrawImage(bWorkTmp, -rClone.X + 10, -rClone.Y + 10);

                        using Graphics gx4 = Graphics.FromImage(trWorkI);
                        gx4.SmoothingMode = SmoothingMode.None;
                        gx4.InterpolationMode = InterpolationMode.NearestNeighbor;
                        gx4.PixelOffsetMode = PixelOffsetMode.Half;
                        gx4.DrawPath(p4, gPU);

                        //show pic with path to click fg
                        Bitmap? trOld = trWork;
                        trWork = new Bitmap(rClone.Width + 20, rClone.Height + 20);
                        if (trOld != null)
                            trOld.Dispose();
                        trOld = null;

                        using Graphics gxIW = Graphics.FromImage(trWork);
                        gxIW.DrawImage(trWorkI, 10, 10);

                        using GetAlphaMatte.frmEdgePic frm = new GetAlphaMatte.frmEdgePic(trWork, bWork);
                        frm.SetupWithFrameAndOKButton();
                        Point fgPt = new Point(-1, -1);
                        frm.ClickedPoint = fgPt;
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            fgPt = frm.ClickedPoint;

                            //extend path with fg and bg parts
                            Bitmap bTrCopy = (Bitmap)trWork.Clone();
                            ChainFinder cf = new();
                            List<ChainCode> c = cf.GetOutline(bTrCopy, 0, false, 0, false, 0, false);

                            if (c.Count > 0)
                            {
                                c = c.OrderByDescending(a => a.Area).ToList();

                                PointF[] pPts = new PointF[gPU.PointCount];
                                gPU.PathPoints.CopyTo(pPts, 0);

                                //get path orientation etc
                                PointF pt0 = pPts[0];
                                PointF ptL = pPts[pPts.Length - 1];
                                double dx = ptL.X - pt0.X;
                                double dy = ptL.Y - pt0.Y;
                                double dist = Math.Sqrt(dx * dx + dy * dy);
                                dx /= dist;
                                dy /= dist;

                                PointF[] pTmp = new PointF[] { pt0, ptL };
                                PointF lP = pTmp.Where(a => a.X == pTmp.Min(a => a.X)).FirstOrDefault();
                                PointF rP = pTmp.Where(a => a.X == pTmp.Max(a => a.X)).FirstOrDefault();
                                PointF tmpG = new PointF(rP.X - lP.X, rP.Y - lP.Y);
                                double wG = Math.Atan2(tmpG.Y, tmpG.X);

                                float xx = fgPt.X - lP.X;
                                float yy = fgPt.Y - lP.Y;
                                double wL = Math.Atan2(yy, xx);

                                //angle between path and clicked point
                                double wLG = wL - wG;
                                double wD1 = wG + Math.PI / 2.0;
                                double wD2 = wG - Math.PI / 2.0;

                                //Points to compare in direction1 (perpendicular to path overall orientation)
                                double xStep1 = Math.Cos(wD1) * wh / 2;
                                double yStep1 = Math.Sin(wD1) * wh / 2;
                                PointF ptComp11 = new PointF((float)(lP.X + xStep1), (float)(lP.Y + yStep1));
                                PointF ptComp12 = new PointF((float)(rP.X + xStep1), (float)(rP.Y + yStep1));

                                //Points to compare in direction2
                                double xStep2 = Math.Cos(wD2) * wh / 2;
                                double yStep2 = Math.Sin(wD2) * wh / 2;
                                PointF ptComp21 = new PointF((float)(lP.X + xStep2), (float)(lP.Y + yStep2));
                                PointF ptComp22 = new PointF((float)(rP.X + xStep2), (float)(rP.Y + yStep2));

                                int indx11 = FindClosest(ptComp11, c[0].Coord);
                                int indx12 = FindClosest(ptComp12, c[0].Coord);
                                int indx21 = FindClosest(ptComp21, c[0].Coord);
                                int indx22 = FindClosest(ptComp22, c[0].Coord);

                                //get path segments for fg and bg
                                List<Point> cc = c[0].Coord;
                                List<Point> path1 = new List<Point>();
                                int st = Math.Min(indx11, indx12);
                                path1.AddRange(cc.Skip(st).Take(Math.Max(indx11, indx12) - st));
                                List<Point> path2 = new List<Point>();
                                int st2 = Math.Min(indx21, indx22);
                                path2.AddRange(cc.Skip(st2).Take(Math.Max(indx21, indx22) - st2));

                                if (path1.Count > 1 && path2.Count > 1)
                                {
                                    //easy check to determine correct paths
                                    if (path1.Contains(path2[0]))
                                    {
                                        path1 = cc.Except(path1).ToList();
                                        ReSortPath(path1);
                                    }
                                    if (path2.Contains(path1[0]))
                                    {
                                        path2 = cc.Except(path2).ToList();
                                        ReSortPath(path2);
                                    }

                                    if (pPts.Length > 1 && (path1.Count <= cc.Count / 2.0 || path2.Count <= cc.Count / 2.0))
                                    {
                                        List<Point> pPath1 = new List<Point>();
                                        List<Point> pPath2 = new List<Point>();

                                        int f = 0;
                                        int f2 = 0;

                                        for (int j = 0; j < pPts.Length; j++)
                                        {
                                            PointF ptComp1 = new PointF((float)(pPts[j].X + xStep1),
                                                (float)(pPts[j].Y + yStep1));

                                            int indx = FindClosest(ptComp1, cc);
                                            if (indx > -1)
                                            {
                                                if (f == 0)
                                                    pPath1.Add(cc[indx]);
                                                else
                                                {
                                                    pPath1.AddRange(cc.Skip(f).Take(indx - f));
                                                    f = indx;
                                                }
                                            }

                                            PointF ptComp2 = new PointF((float)(pPts[j].X + xStep2),
                                                (float)(pPts[j].Y + yStep2));

                                            int indx2 = FindClosest(ptComp2, cc);
                                            if (indx2 > -1)
                                            {
                                                if (f2 == 0)
                                                    pPath2.Add(cc[indx2]);
                                                else
                                                {
                                                    pPath2.AddRange(cc.Skip(f).Take(indx2 - f2));
                                                    f2 = indx2;
                                                }
                                            }
                                        }

                                        //Intersection of both paths
                                        List<Point> zl = pPath1.Intersect(pPath2).ToList();
                                        List<Point> zl2 = pPath2.Intersect(pPath1).ToList();
                                        List<Point> path1_1 = pPath1.Except(zl).ToList();
                                        List<Point> path2_1 = pPath2.Except(zl2).ToList();
                                        //Intersection with chain-coords
                                        path1_1 = cc.Intersect(path1_1).ToList();
                                        path2_1 = cc.Intersect(path2_1).ToList();
                                        //resort
                                        int d = Math.Max(bTrCopy.Width, bTrCopy.Height);
                                        ReSortPath(path1_1, d / 2); //distval may change;
                                                                    //maybe I change the whole method to collect all dists
                                                                    //greater that value and create a GraphicsPath with more
                                                                    //than one figure...
                                        ReSortPath(path2_1, d / 2);
                                        if (path1_1.Count > 1)
                                            path1 = path1_1;
                                        if (path2_1.Count > 1)
                                            path2 = path2_1;
                                    }

                                    //determine fg and bg paths
                                    List<Point> tmp = new();
                                    tmp.AddRange(path1);
                                    double distCP1 = FindClosestDist(fgPt, tmp);
                                    List<Point> tmp2 = new();
                                    tmp2.AddRange(path2);
                                    double distCP2 = FindClosestDist(fgPt, tmp2);
                                    int fgPath = 1;
                                    if (distCP2 < distCP1)
                                        fgPath = 2;

                                    //int indxCP = FindClosest(fgPt, cc);
                                    //Point pSearch = cc[indxCP];
                                    //int fgPath = 1;
                                    //if (path2.Contains(pSearch))
                                    //    fgPath = 2;
                                    //if (path1.Contains(pSearch))
                                    //    fgPath = 1;

                                    List<Point>[] paths = new List<Point>[] { path1, path2 };
                                    List<Point> fg = paths[fgPath - 1];
                                    List<Point> bg = (fgPath == 1) ? paths[1] : paths[0];

                                    //remove BG parts that are outside the gPU-bounds
                                    //and remove parts that are still in the area of the fg-path
                                    using GraphicsPath gPUC = (GraphicsPath)gPU.Clone();
                                    RectangleF rc = gPUC.GetBounds();
                                    IEnumerable<Point> zz = bg.Where(a => rc.Contains(a));
                                    if (zz != null && zz.Count() > 0)
                                        bg = zz.ToList();

                                    using GraphicsPath fgC = new();
                                    fgC.AddLines(fg.Select(a => new PointF(a.X, a.Y)).ToArray());
                                    fgC.CloseAllFigures();
                                    fgC.Widen(p4);

                                    for (int i = bg.Count - 1; i >= 0; i--)
                                        if (fgC.IsVisible(bg[i]) && bg.Count > 2)
                                            bg.RemoveAt(i);

                                    if (bg.Count > 1)
                                    {
                                        using GraphicsPath gPFG = new GraphicsPath();
                                        gPFG.StartFigure();
                                        gPFG.AddLines(fg.Select(a => new PointF(a.X, a.Y)).ToArray());
                                        using GraphicsPath gPBG = new GraphicsPath();
                                        gPBG.StartFigure();
                                        gPBG.AddLines(bg.Select(a => new PointF(a.X, a.Y)).ToArray());

                                        //draw to trWorkCopy
                                        using Graphics gxTrC = Graphics.FromImage(bTrCopy);
                                        gxTrC.PixelOffsetMode = PixelOffsetMode.Half;
                                        gxTrC.InterpolationMode = InterpolationMode.NearestNeighbor;
                                        gxTrC.SmoothingMode = SmoothingMode.None;

                                        using Pen pFG = new Pen(Color.White, wh / 2);
                                        pFG.LineJoin = LineJoin.Round;
                                        pFG.StartCap = LineCap.Round;
                                        pFG.EndCap = LineCap.Round;
                                        gxTrC.DrawPath(pFG, gPFG);
                                        using Pen pBG = new Pen(Color.Black, wh / 2);
                                        pBG.LineJoin = LineJoin.Round;
                                        pBG.StartCap = LineCap.Round;
                                        pBG.EndCap = LineCap.Round;
                                        gxTrC.DrawPath(pBG, gPBG);

                                        FillBlankPixelsUnknown(bTrCopy);

                                        Bitmap bC4 = (Bitmap)bTrCopy.Clone();
                                        Image? bPB = this.pictureBox2.Image;
                                        this.pictureBox2.Image = bC4;
                                        if (bPB != null)
                                            bPB.Dispose();
                                        bPB = null;
                                        this.pictureBox2.Refresh();

                                        if (this.cbAskTrimap.Checked)
                                        {
                                            using Bitmap bC5 = new Bitmap(bTrCopy.Width, bTrCopy.Height);
                                            using Graphics gx = Graphics.FromImage(bC5);
                                            gx.PixelOffsetMode = PixelOffsetMode.Half;
                                            gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                                            gx.SmoothingMode = SmoothingMode.None;
                                            gx.Clear(Color.Gray);
                                            gx.DrawPath(pFG, gPBG);
                                            gx.DrawPath(pBG, gPFG);
                                            FillBlankPixelsUnknown(bC5);
                                            using frmSelectTrimap frmST = new frmSelectTrimap(bC4, bC5, bWork);

                                            if (frmST.ShowDialog() == DialogResult.OK && frmST.FBitmap != null)
                                            {
                                                Bitmap bC44 = (Bitmap)frmST.FBitmap.Clone();
                                                Bitmap? bC45 = (Bitmap)frmST.FBitmap.Clone();
                                                Image? bPB4 = this.pictureBox2.Image;
                                                this.pictureBox2.Image = bC44;
                                                if (bPB4 != null)
                                                    bPB4.Dispose();
                                                bPB4 = null;
                                                this.pictureBox2.Refresh();

                                                Bitmap? bOld45 = bTrCopy;
                                                bTrCopy = bC45;
                                                if (bOld45 != null)
                                                    bOld45.Dispose();
                                                bOld45 = null;
                                            }
                                        }

                                        Bitmap? bTrOld = trWork;
                                        trWork = bTrCopy;
                                        if (bTrOld != null)
                                            bTrOld.Dispose();
                                        bTrOld = null;
                                    }
                                    else
                                    {
                                        Cleanup();
                                        return;
                                    }
                                }
                                else
                                {
                                    Cleanup();
                                    return;
                                }
                            }
                            else
                            {
                                Cleanup();
                                return;
                            }
                        }
                        else
                        {
                            Cleanup();
                            return;
                        }
                    }

                    this._hs = this.cbHalfSize.Checked;
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
                    this._cfop.ShowInfoOuter += Cfop_ShowInfoOuter;

                    bool scalesPics = this.cbSlices.Checked;
                    int scales = scalesPics ? !rb64.Checked ? rb4.Checked ? 4 : 16 : 64 : 0;
                    int overlap = 32;
                    bool interpolated = this.cbInterpolated.Checked;
                    bool forceSerial = this.cbForceSerial.Checked;
                    bool group = false;
                    int groupAmountX = scalesPics ? 1 : 0; //we dont use grouping, so set it simply to 1
                    int groupAmountY = scalesPics ? 1 : 0;
                    int maxSize = this.cbInterpolated.Checked ? (int)Math.Pow((double)this.numMaxSize.Value, 2) * 2 : (int)Math.Pow((double)this.numMaxSize.Value, 2);
                    bool trySingleTile = /*scalesPics ? false : this.cbHalfSize.Checked ? true :*/ bWork.Width * bWork.Height < maxSize ? true : false;
                    bool verifyTrimaps = false;

                    int maxW = this.cbInterpolated.Checked ? (int)this.numMaxSize.Value * 2 : (int)this.numMaxSize.Value;

                    if (maxW >= this._minSize)
                        this.backgroundWorker1.RunWorkerAsync(new object[] { 1 /* GMRES_r; 0 is GaussSeidel */, scalesPics, scales, overlap,
                                interpolated, forceSerial, group, groupAmountX, groupAmountY, maxSize, bWork, trWork,
                                trySingleTile, verifyTrimaps });
                    else
                    {
                        MessageBox.Show("MaxSize is too small for creating tiles with an overlap. Minimum is " + this._minSize.ToString());
                        Cleanup();
                        return;
                    }
                }
            }
        }

        private double FindClosestDist(Point ptComp, List<Point> coord)
        {
            double min = double.MaxValue;
            for (int j = 0; j < coord.Count; j++)
            {
                double dx = ptComp.X - coord[j].X;
                double dy = ptComp.Y - coord[j].Y;
                double dist = dx * dx + dy * dy;

                if (dist < min)
                    min = dist;
            }

            return min;
        }

        private void ReSortPath(List<Point> path, int breakDist)
        {
            int j = -1;
            for (int i = 1; i < path.Count; i++)
            {
                double dx = path[i].X - path[i - 1].X;
                double dy = path[i].Y - path[i - 1].Y;
                if (Math.Sqrt(dx * dx + dy * dy) > breakDist)
                {
                    j = i - 1;
                    break;
                }
            }

            List<Point> l = new();
            l.AddRange(path.Take(j + 1));
            path.RemoveRange(0, j + 1);
            path.AddRange(l);
        }

        private void ReSortPath(List<Point> path)
        {
            int j = -1;
            for (int i = 1; i < path.Count; i++)
            {
                double dx = path[i].X - path[i - 1].X;
                double dy = path[i].Y - path[i - 1].Y;
                if (dx * dx + dy * dy > 2)
                {
                    j = i - 1;
                    break;
                }
            }

            List<Point> l = new();
            l.AddRange(path.Take(j + 1));
            path.RemoveRange(0, j + 1);
            path.AddRange(l);
        }

        private unsafe void FillBlankPixelsUnknown(Bitmap bTrCopy)
        {
            int w = bTrCopy.Width;
            int h = bTrCopy.Height;
            BitmapData bmD = bTrCopy.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Color c = Color.Gray;
            byte val = c.R;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] < 255)
                    {
                        p[0] = p[1] = p[2] = val;
                        p[3] = 255;
                    }

                    p += 4;
                }
            });

            bTrCopy.UnlockBits(bmD);
        }

        private int FindClosest(PointF ptComp, List<Point> coord)
        {
            double min = double.MaxValue;
            int i = -1;
            for (int j = 0; j < coord.Count; j++)
            {
                double dx = ptComp.X - coord[j].X;
                double dy = ptComp.Y - coord[j].Y;
                double dist = dx * dx + dy * dy;

                if (dist < min)
                {
                    min = dist;
                    i = j;
                }
            }

            return i;
        }

        private int FindClosest(PointF ptComp, List<Point> coord, double dirDx, double dirDy)
        {
            int i = -1;
            List<Point> coord1 = coord.Where(a => Math.Sign(a.X - ptComp.X) == Math.Sign(dirDx) && Math.Sign(a.Y - ptComp.Y) == Math.Sign(dirDy)).ToList();
            if (coord1 != null)
            {
                double min = double.MaxValue;

                for (int j = 0; j < coord1.Count; j++)
                {
                    double dx = ptComp.X - coord1[j].X;
                    double dy = ptComp.Y - coord1[j].Y;
                    double dist = dx * dx + dy * dy;

                    if (dist < min)
                    {
                        min = dist;
                        i = j;
                    }
                }
            }

            return i;
        }

        private void DrawFGPixels(Bitmap bWork, Bitmap fg, Bitmap bmp, int ww, int hh)
        {
            List<Point> pts = new();
            int w = fg.Width;
            int h = fg.Height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (fg.GetPixel(x, y).A == 255 && pts.Count < (ww * hh) / 2)
                        pts.Add(new Point(x, y));

                    if (pts.Count >= (ww * hh) / 2)
                        break;
                }

                if (pts.Count >= (ww * hh) / 2)
                    break;
            }

            int wB = bWork.Width;
            int hB = bWork.Height;
            int cnt = 0;

            for (int y = hB - hh; y < hB - hh / 2; y++)
            {
                for (int x = wB - ww; x < wB; x++)
                {
                    if (cnt < pts.Count)
                    {
                        int xx = pts[cnt].X;
                        int yy = pts[cnt].Y;
                        bWork.SetPixel(x, y, bmp.GetPixel(xx, yy));
                    }
                }
            }
        }

        private void DrawBGPixels(Bitmap bWork, Bitmap bg, Bitmap bmp, int ww, int hh)
        {
            List<Point> pts = new();
            int w = bg.Width;
            int h = bg.Height;

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (bg.GetPixel(x, y).A == 255 && pts.Count < (ww * hh) / 2)
                        pts.Add(new Point(x, y));

                    if (pts.Count >= (ww * hh) / 2)
                        break;
                }

                if (pts.Count >= (ww * hh) / 2)
                    break;
            }

            int wB = bWork.Width;
            int hB = bWork.Height;
            int cnt = 0;

            for (int y = hB - hh / 2; y < hB; y++)
            {
                for (int x = wB - ww; x < wB; x++)
                {
                    if (cnt < pts.Count)
                    {
                        int xx = pts[cnt].X;
                        int yy = pts[cnt].Y;
                        bWork.SetPixel(x, y, bmp.GetPixel(xx, yy));
                    }
                }
            }
        }

        private unsafe Bitmap ExtractFG(Bitmap trWorkTmp)
        {
            int w = trWorkTmp.Width;
            int h = trWorkTmp.Height;
            Bitmap bOut = new Bitmap(w, h);

            BitmapData bmD = bOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmR = trWorkTmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                byte* pR = (byte*)bmR.Scan0;
                pR += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pR[0] > 252)
                    {
                        p[0] = p[1] = p[2] = 255;
                        p[3] = 255;
                    }
                    p += 4;
                    pR += 4;
                }
            });

            bOut.UnlockBits(bmD);
            trWorkTmp.UnlockBits(bmR);

            return bOut;
        }

        private unsafe Bitmap ExtractBG(Bitmap trWorkTmp)
        {
            int w = trWorkTmp.Width;
            int h = trWorkTmp.Height;
            Bitmap bOut = new Bitmap(w, h);

            BitmapData bmD = bOut.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmR = trWorkTmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                byte* pR = (byte*)bmR.Scan0;
                pR += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pR[0] < 4)
                    {
                        p[0] = p[1] = p[2] = 0;
                        p[3] = 255;
                    }
                    p += 4;
                    pR += 4;
                }
            });

            bOut.UnlockBits(bmD);
            trWorkTmp.UnlockBits(bmR);

            return bOut;
        }

        private void DrawOutFigures(List<List<Point>> figures1, List<List<Point>> figures2, Size size, List<Point> path1, List<Point> path2, List<Point> path11, List<Point> path21)
        {
            using Bitmap b = new(size.Width, size.Height);
            using Graphics g = Graphics.FromImage(b);
            g.Clear(Color.Green);

            //for (int i = 0; i < figures1.Count; i++)
            //{
            //    List<Point> pt = figures1[i];

            //    if (pt.Count > 1)
            //        foreach (Point p in pt)
            //            g.FillEllipse(Brushes.Red, new RectangleF(p.X - 1, p.Y - 1, 3, 3));
            //}

            //foreach (Point p in path1)
            //    g.FillEllipse(Brushes.Blue, new RectangleF(p.X - 1, p.Y - 1, 3, 3));      

            foreach (Point p in path11)
                g.FillEllipse(Brushes.White, new RectangleF(p.X - 1, p.Y - 1, 3, 3));

            //for (int i = 0; i < figures2.Count; i++)
            //{
            //    List<Point> pt = figures2[i];

            //    if (pt.Count > 1)
            //        foreach (Point p in pt)
            //        {
            //            using SolidBrush sb = new(Color.FromArgb(64, 255, 255, 0));
            //            g.FillEllipse(sb, new RectangleF(p.X - 1, p.Y - 1, 3, 3));
            //        }
            //}

            //foreach (Point p in path2)
            //{
            //    using SolidBrush sb = new(Color.FromArgb(127, 127, 100, 0));
            //    g.FillEllipse(sb, new RectangleF(p.X - 1, p.Y - 1, 3, 3));
            //}

            foreach (Point p in path21)
            {
                using SolidBrush sb = new(Color.FromArgb(127, 150, 150, 150));
                g.FillEllipse(sb, new RectangleF(p.X - 1, p.Y - 1, 3, 3));
            }
            Form fff = new Form();
            fff.BackgroundImage = b;
            fff.BackgroundImageLayout = ImageLayout.Zoom;
            fff.ShowDialog();
        }

        private void DrawOutFigures(List<Point> path1, List<Point> path2, Size size, GraphicsPath gPU, int wh)
        {
            using Bitmap b = new(size.Width, size.Height);
            using Graphics g = Graphics.FromImage(b);
            g.Clear(Color.Green);

            using GraphicsPath gP1 = new();
            gP1.AddLines(path2.Select(a => new PointF(a.X, a.Y)).ToArray());
            using Pen pen1 = new Pen(Color.FromArgb(255, 0, 0, 127), wh);
            g.DrawPath(pen1, gP1);

            foreach (Point p in path1)
                g.FillEllipse(Brushes.White, new RectangleF(p.X - 1, p.Y - 1, 3, 3));

            foreach (Point p in path2)
            {
                using SolidBrush sb = new(Color.FromArgb(127, 150, 150, 150));
                g.FillEllipse(sb, new RectangleF(p.X - 1, p.Y - 1, 3, 3));
            }

            using GraphicsPath gP = (GraphicsPath)gPU.Clone();
            //using Matrix mx = new Matrix(1, 0, 0, 1, wh / 2, wh / 2);
            //gP.Transform(mx);
            using Pen pen = new Pen(Color.FromArgb(64, 0, 0, 255), 1);
            g.DrawPath(pen, gP);

            Form fff = new Form();
            fff.BackgroundImage = b;
            fff.BackgroundImageLayout = ImageLayout.Zoom;
            fff.ShowDialog();
        }

        private List<List<Point>> CrossIntersectAndRemove(List<List<Point>> figures1, List<List<Point>> figures2)
        {
            List<List<Point>> res = new();
            for (int i = 0; i < figures1.Count; i++)
            {
                for (int j = 0; j < figures2.Count; j++)
                {
                    List<Point> zl = figures1[i].Intersect(figures2[j]).ToList();
                    res.Add(figures1[i].Except(zl).ToList());
                }
            }

            return res;
        }

        private List<List<Point>> ReSortPathTest(List<Point> path, int breakDist)
        {
            List<List<Point>> res = new();
            List<double> dists = new List<double>();
            dists.Add(0);
            int j = 0;
            for (int i = 1; i < path.Count; i++)
            {
                double dx = path[i].X - path[i - 1].X;
                double dy = path[i].Y - path[i - 1].Y;
                dists.Add(Math.Sqrt(dx * dx + dy * dy));
            }

            for (int i = 1; i < dists.Count; i++)
            {
                if (dists[i] > breakDist)
                {
                    List<Point> ll = new();
                    ll.AddRange(path.Skip(j).Take(i - j));
                    res.Add(ll);
                    j = i;
                }
            }

            List<Point> lll = new();
            lll.AddRange(path.Skip(j).Take(path.Count - j));
            res.Add(lll);

            return res;

            ////int j = -1;
            //for (int i = 1; i < path.Count; i++)
            //{
            //    double dx = path[i].X - path[i - 1].X;
            //    double dy = path[i].Y - path[i - 1].Y;
            //    if (Math.Sqrt(dx * dx + dy * dy) > breakDist)
            //    {
            //        j = i - 1;
            //        break;
            //    }
            //}

            //List<Point> l = new();
            //l.AddRange(path.Take(j + 1));
            //path.RemoveRange(0, j + 1);
            //path.AddRange(l);
        }

        private void helplineRulerCtrl2_Paint(object sender, PaintEventArgs e)
        {
            if (this._pointsDraw != null && this._pointsDraw.Count > 1)
            {
                using GraphicsPath gP = new();
                gP.AddLines(this._pointsDraw.Select(a => new PointF(a.X, a.Y)).ToArray());

                using Matrix mx = new Matrix(this.helplineRulerCtrl2.Zoom, 0, 0, this.helplineRulerCtrl2.Zoom,
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y);

                gP.Transform(mx);

                using Pen p = new Pen(Color.Red, Math.Max((float)this.numDrawPenWidth.Value * this.helplineRulerCtrl2.Zoom, 1));
                p.LineJoin = LineJoin.Round;
                p.StartCap = LineCap.Round;
                p.EndCap = LineCap.Round;

                e.Graphics.DrawPath(p, gP);
            }

            if (this.numDrawPenWidth.Enabled)
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(95, 255, 0, 0)))
                    e.Graphics.FillEllipse(sb, new RectangleF(
                        this._eX2 - (float)((float)this.numDrawPenWidth.Value / 2f * this.helplineRulerCtrl2.Zoom) - 1f,
                        this._eY2 - (float)((float)this.numDrawPenWidth.Value / 2f * this.helplineRulerCtrl2.Zoom) - 1f,
                        (float)((float)this.numDrawPenWidth.Value * this.helplineRulerCtrl2.Zoom),
                        (float)((float)this.numDrawPenWidth.Value * this.helplineRulerCtrl2.Zoom)));
        }

        public void SetScribbles(Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles, List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq)
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

            int ix2 = Math.Min(ix, this.helplineRulerCtrl1.Bmp.Width - 1);
            int iy2 = Math.Min(iy, this.helplineRulerCtrl1.Bmp.Height - 1);

            if (ix >= 0 && iy >= 0)
                if (!this.cbRefPtBG.Checked && !this.cbRefPtFG.Checked && e.Button == MouseButtons.Left && this.cbClickMode.Checked)
                    this._points2.Add(new Point(ix, iy));

            if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                this._rX = ix;
                this._rY = iy;

                this._eX = eX;
                this._eY = eY;

                if (!this.cbRefPtBG.Checked && !this.cbRefPtFG.Checked && e.Button == MouseButtons.Left && !this.cbClickMode.Checked)
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

                if (this._tracking4 && !this.cbClickMode.Checked)
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

                if (this._tracking4 || this.cbClickMode.Checked)
                {
                    AddPointsToScribblePath();
                    if (this._oldUnknownFT != null)
                        ClearOldUnknownFT(this._points2, (int)this.numWHScribbles.Value);
                    this._points2.Clear();
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }

            this._tracking = false;
            this._tracking4 = false;
            this.helplineRulerCtrl1.dbPanel1.Capture = false;
        }

        private void ClearOldUnknownFT(List<Point> pts, int wh)
        {
            if (this._oldUnknownFT != null)
            {
                using (Graphics gx = Graphics.FromImage(this._oldUnknownFT))
                {
                    gx.CompositingMode = CompositingMode.SourceCopy;
                    if (pts.Count > 1)
                    {
                        for (int i = 0; i < pts.Count; i++)
                        {
                            using (SolidBrush sb = new SolidBrush(Color.Transparent))
                                gx.FillRectangle(sb, new Rectangle(
                                    (int)(pts[i].X - wh / 2),
                                    (int)(pts[i].Y - wh / 2),
                                    (int)wh,
                                    (int)wh));
                            using (Pen pen = new Pen(Color.Transparent, 1))
                                gx.DrawRectangle(pen, new Rectangle(
                                    (int)(pts[i].X - wh / 2),
                                    (int)(pts[i].Y - wh / 2),
                                    (int)wh,
                                    (int)wh));
                        }
                    }
                    else
                    {
                        using (SolidBrush sb = new SolidBrush(Color.Transparent))
                            gx.FillRectangle(sb, new Rectangle(
                                (int)(pts[0].X - wh / 2),
                                (int)(pts[0].Y - wh / 2),
                                (int)wh,
                                (int)wh));
                        using (Pen pen = new Pen(Color.Transparent, 1))
                            gx.DrawRectangle(pen, new Rectangle(
                                (int)(pts[0].X - wh / 2),
                                (int)(pts[0].Y - wh / 2),
                                (int)wh,
                                (int)wh));
                    }
                }
            }
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
                        this._points2 = GetPointsSequence(ptSt, ptEnd, wh, 10);
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
                        if (this._scribbles[fgbg][wh].Count > 0)
                            this._scribbleSeq.Add(Tuple.Create(fgbg, wh, this._scribbles[fgbg][wh].Count - 1, false, new List<List<Point>>()));
                    }
                }
            }
        }

        private List<Point> GetPointsSequence(Point? ptSt, Point? pt, int wh, double div)
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

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
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

                    //test
                    if (!this.DesignMode && listNo < 0)
                    {
                        //MessageBox.Show("listNo is -1, hlc1_paint");

                        if (this._scribbles.ContainsKey(l) && this._scribbles[l].ContainsKey(wh))
                        {
                            List<List<Point>> j = this._scribbles[l][wh];

                            if (j != null && j.Count > 0)
                                j.RemoveAt(j.Count - 1);

                            if (this._scribbleSeq != null && j != null)
                            {
                                IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> ll = this._scribbleSeq.Where(a => a.Item1 == l);
                                if (ll != null && ll.Count() > 0)
                                {
                                    IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> whL = ll.Where(a => a.Item2 == wh);

                                    if (whL != null && whL.Count() > 0)
                                    {
                                        IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> listL = whL.Where(a => a.Item3 == j.Count);

                                        if (listL != null && listL.Count() > 0)
                                        {
                                            int indxt = this._scribbleSeq.IndexOf(listL.First());

                                            for (int j4 = indxt + 1; j4 < this._scribbleSeq.Count; j4++)
                                            {
                                                if (this._scribbleSeq[j4].Item1 == l && this._scribbleSeq[j4].Item2 == wh)
                                                    this._scribbleSeq[j4] = Tuple.Create(l, wh, this._scribbleSeq[j4].Item3 - 1, false, new List<List<Point>>());
                                            }

                                            this._scribbleSeq.Remove(listL.First());
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (this._scribbles.ContainsKey(l) && this._scribbles[l].ContainsKey(wh))
                    {
                        List<List<Point>> ptsList = this._scribbles[l][wh];

                        if (ptsList != null && ptsList.Count > listNo)
                        {
                            bool doRect = ptsList[listNo].Count > 1;

                            Color c = l == 0 ? Color.Black : l == 1 ? Color.White : Color.Cyan;

                            if (doRect)
                            {
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
                                            (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                            (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
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
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
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
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
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
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
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
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
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
                                Color c = Color.Cyan;

                                if (doRect)
                                {
                                    foreach (Point pt in pts)
                                    {
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
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
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this._points2.Count > 0)
            {
                Color c = this.rbFG.Checked ? Color.White : this.rbBG.Checked ? Color.Black : Color.Lime;

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

            //overlay for remOuter, will be changed
            if (this._oldUnknownFT != null)
            {
                e.Graphics.DrawImage(this._oldUnknownFT, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                            this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                            this._oldUnknownFT.Width * this.helplineRulerCtrl1.Zoom,
                            this._oldUnknownFT.Height * this.helplineRulerCtrl1.Zoom);
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
                                        b1 = (Bitmap)img.Clone();
                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                        b2 = (Bitmap)img.Clone();
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
            //_undoOPCache.CheckRedoButton(this.btnRedo);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The goal is to achieve results as good as with a GrabCut, but only with the first (GMM_probabilities) estimation, to reduce the memory footprint. So keep \"QuickEstimation\" checked and play around with the other parameters.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache.Processing == false)
            {
                Bitmap bOut = _undoOPCache.DoUndoAll();

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

            //        Bitmap bOut = _undoOPCache.DoRedo();

            //        if (bOut != null)
            //        {
            //            this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");

            //            this._pic_changed = true;
            //            // Me.helplineRulerCtrl1.CalculateZoom()

            //            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

            //            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            //            // Me.helplineRulerCtrl1.dbPanel1.Invalidate()

            //            if (_undoOPCache.CurrentPosition > 1)
            //            {
            //                this.btnReset.Enabled = true;
            //                this.cbRectMode.Enabled = false;
            //            }
            //            else
            //                this.btnReset.Enabled = false;

            //            if (_undoOPCache.CurrentPosition < _undoOPCache.Count)
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
                //string f = this.Text.Split(new String[] { " - " }, StringSplitOptions.None)[0];
                Bitmap? b1 = null;

                try
                {
                    if (this._bmpBU != null)
                    {
                        if (AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 12L))
                            b1 = (Bitmap)this._bmpBU.Clone();
                        else
                            throw new Exception();

                        Bitmap? bC = (Bitmap)b1.Clone();
                        this.SetBitmap(ref this._bmpBU, ref bC);

                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");

                        this._pic_changed = false;

                        this.helplineRulerCtrl1.CalculateZoom();

                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                        // SetHRControlVars();

                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        _undoOPCache?.Reset(false);

                        //if (_undoOPCache.Count > 1)
                        //    this.btnRedo.Enabled = true;
                        //else
                        //    this.btnRedo.Enabled = false;
                    }

                    if (this._bmpOutline != null && MessageBox.Show("Reset _bmpOutline?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        this._bmpOutline.Dispose();
                        this._bmpOutline = null;
                    }
                }
                catch
                {
                    if (b1 != null)
                        b1.Dispose();
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

                        if (ct is Panel)
                        {
                            ct.Enabled = true;
                            Panel pn = (Panel)ct;

                            foreach (Control c in pn.Controls)
                            {
                                if (!(c is Button) && !(c.Name == "numSleep") && !(c.Name == "numError"))
                                    c.Enabled = e;

                                if (c.Name == "numSleep")
                                    c.Enabled = true;

                                if (c.Name == "numError")
                                    c.Enabled = true;
                            }
                        }
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

                    if (ct is Panel)
                    {
                        ct.Enabled = true;
                        Panel pn = (Panel)ct;

                        foreach (Control c in pn.Controls)
                        {
                            if (!(c is Button) && !(c.Name == "numSleep"))
                                c.Enabled = e;

                            if (c.Name == "numSleep")
                                c.Enabled = true;
                        }
                    }
                }

                this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl2.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void SetBitmap(Bitmap? bitmapToSet, Bitmap? bitmapToBeSet, Control ct, string property)
        {
            Bitmap? bOld = bitmapToSet;

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

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap? bitmapToBeSet)
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
                if (this._bmpMatte != null)
                    this._bmpMatte.Dispose();
                if (this._oldUnknownFT != null)
                    this._oldUnknownFT.Dispose();
                if (this._bmpOutline != null)
                    this._bmpOutline.Dispose();
                if (this._bmpMatteOrigSize != null)
                    this._bmpMatteOrigSize.Dispose();
                if (this._resMatte != null)
                    this._resMatte.Dispose();
                if (this._fgMap != null)
                    this._fgMap.Dispose();

                if (this._cfop != null)
                {
                    this._cfop.ShowProgess -= Cfop_UpdateProgress;
                    this._cfop.ShowInfo -= _cfop_ShowInfo;
                    this._cfop.ShowInfoOuter -= Cfop_ShowInfoOuter;
                    this._cfop.Dispose();
                }
            }
        }

        private void Cfop_ShowInfoOuter(object? sender, string e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    this.toolStripStatusLabel1.Text = e;
                }));
            else
                this.toolStripStatusLabel1.Text = e;
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

                    if (this._ptPrev != null && this._ptPrev.Count > 1)
                    {
                        this._ptPrev.RemoveAt(this._ptPrev.Count - 1);
                        this._ptSt = this._ptPrev[this._ptPrev.Count - 1];
                    }
                    else
                    {
                        if (this._ptPrev != null && this._ptPrev.Count > 0)
                            this._ptPrev.RemoveAt(this._ptPrev.Count - 1);
                        this._ptSt = null;
                    }
                }
                else
                    using (frmLastScribbles frm = new frmLastScribbles(this.helplineRulerCtrl1.Bmp, this._scribbles, this._scribbleSeq))
                    {
                        frm.UnknownScibblesBU = this.UnknownScibblesBU;
                        frm.ShowDialog();
                        this.UnknownScibblesBU = frm.UnknownScibblesBU;
                    }

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

                    //double resPic = 1.0;

                    //if (f.BaseSize != null)
                    //    resPic = (double)Math.Max(f.BaseSize.Value.Width, f.BaseSize.Value.Height) /
                    //        (double)Math.Max(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                    //if (this._scribbles != null && resPic != 1.0)
                    //{
                    //    Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles2 = ResizeAllScribbles(this._scribbles, resPic);
                    //    this._scribbles = scribbles2;
                    //    List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = ResizeScribbleSeq(this._scribbleSeq, resPic, true);
                    //    this._scribbleSeq = scribbleSeq2;
                    //}

                    //test, this might help for scribbles that come from frmQuickExtract
                    if (this.cbApproxLines.Checked && this._scribbles.ContainsKey(3))
                    {
                        Dictionary<int, List<List<Point>>> j = this._scribbles[3];
                        for (int i = 0; i < j.Count; i++)
                        {
                            List<List<Point>> l = j.ElementAt(i).Value;
                            int w = j.ElementAt(i).Key;
                            ChainFinder cf = new ChainFinder();

                            for (int ii = 0; ii < l.Count; ii++)
                            {
                                List<Point> pts = l[ii];
                                if (this._dynamic)
                                    pts = cf.ApproximateLines(pts, Math.Max(w / this._divisor, 1));
                                else
                                    pts = cf.ApproximateLines(pts, this._newWidth);

                                List<Point> pts2 = new List<Point>();
                                for (int ll = 1; ll < pts.Count; ll++)
                                {
                                    double dx = pts[ll].X - pts[ll - 1].X;
                                    double dy = pts[ll].Y - pts[ll - 1].Y;
                                    double lngth = Math.Sqrt(dx * dx + dy * dy);
                                    dx /= lngth;
                                    dy /= lngth;

                                    for (int iii = 0; iii < (int)lngth; iii++)
                                    {
                                        Point pt = new Point(pts[ll - 1].X + (int)(iii * dx), pts[ll - 1].Y + (int)(iii * dy));

                                        if (pts[ll - 1].X != pt.X || pts[ll - 1].Y != pt.Y)
                                            pts2.Add(pt);
                                    }
                                }

                                l[ii] = pts2;
                            }

                            j[w] = l;
                        }
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

        private string ConvertToBase64(Bitmap bmp)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.GetBuffer());
            }
        }

        private Bitmap ConvertFromBase64(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);

            Bitmap? bmp = null;
            Image? img = null;
            using (MemoryStream ms = new MemoryStream(bytes))
                img = Image.FromStream(ms);

            bmp = (Bitmap)img.Clone();
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
                    res.Add(Tuple.Create(scribbleSeq[i].Item1, (int)Math.Max(scribbleSeq[i].Item2 / resWC, 1), scribbleSeq[i].Item3, false, new List<List<Point>>()));

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

                //if (this._scribbles != null && this._scribbles.ContainsKey(0) && this._scribbles[0].ContainsKey(3))
                //    if (this._scribbles[0][3].Count > 0)
                //        this._scribbles[0][3].RemoveAt(this._scribbles[0][3].Count - 1);

                this._bitsBG.SetAll(false);

                Bitmap b = this._scribblesBitmap;

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

                    if (!FloodFillMethods.Cancel)
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
                    if (this._scribbles[0][3].Count > 0)
                        this._scribbleSeq.Add(Tuple.Create(0, 3, this._scribbles[0][3].Count - 1, true, GetBoundariesForScribbleFill(this._scribbles[0][3][this._scribbles[0][3].Count - 1], w, h)));

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
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

                //if (this._scribbles != null && this._scribbles.ContainsKey(1) && this._scribbles[1].ContainsKey(2))
                //    this._scribbles[1].Remove(3);

                this._bitsFG.SetAll(false);

                Bitmap b = this._scribblesBitmap;

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

                    if (!FloodFillMethods.Cancel)
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
                    if (this._scribbles[1][3].Count > 0)
                        this._scribbleSeq.Add(Tuple.Create(1, 3, this._scribbles[1][3].Count - 1, true, GetBoundariesForScribbleFill(this._scribbles[1][3][this._scribbles[1][3].Count - 1], w, h)));

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private List<List<Point>> GetBoundariesForScribbleFill(List<Point> points, int w, int h)
        {
            List<List<Point>> res = new List<List<Point>>();

            using (Bitmap bmp = new Bitmap(w, h))
            using (Graphics gx = Graphics.FromImage(bmp))
            {
                int wh = 3;
                foreach (Point pt in points)
                {
                    gx.FillRectangle(Brushes.Black, new Rectangle(
                                                (int)(int)(pt.X - wh / 2),
                                                (int)(int)(pt.Y - wh / 2),
                                                (int)wh,
                                                (int)wh));
                    using (Pen pen = new Pen(Color.Black, 2))
                        gx.DrawRectangle(pen, new Rectangle(
                                                    (int)(int)(pt.X - wh / 2),
                                                    (int)(int)(pt.Y - wh / 2),
                                                    (int)wh,
                                                    (int)wh));
                }

                List<ChainCode> c = this.GetBoundary(bmp);
                if (c != null)
                {
                    foreach (ChainCode cc in c)
                    {
                        List<Point> l = new List<Point>();
                        l.AddRange(cc.Coord.ToArray());
                        res.Add(l);
                    }
                }

            }

            return res;
        }

        private List<ChainCode> GetBoundary(Bitmap bmp)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, 0, false, 0, false, 0, false);
            return l;
        }
        private List<ChainCode> GetBoundary(Bitmap? bmp, int threshold)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, threshold, false, 0, false, 0, false);
            return l;
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
                using (Bitmap b = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone())
                {
                    using (frmChainCode frm = new frmChainCode(b, this.CachePathAddition))
                    {
                        frm.SetupCache();
                        frm.numWHScribbles.Value = this.numWHScribbles.Value;
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            int wh = (int)frm.numWHScribbles.Value;

                            //GraphicsPath gP = null;

                            //if(frm.SelectedPath != null)
                            //    gP = (GraphicsPath)frm.SelectedPath.Clone();

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

        private void Cleanup()
        {
            if (this._cfop != null)
            {
                this._cfop.ShowProgess -= Cfop_UpdateProgress;
                this._cfop.ShowInfo -= _cfop_ShowInfo;
                this._cfop.ShowInfoOuter -= Cfop_ShowInfoOuter;
                this._cfop.Dispose();
            }

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            if (this._ptBU == null)
                this._ptBU = new List<Point>();
            this._ptBU.Clear();
            this._ptBU.AddRange(this._pointsDraw);
            this._pointsDraw.Clear();

            rbClosedForm_CheckedChanged(this.rbClosedForm, new EventArgs());

            this.btnOK.Enabled = this.btnCancel.Enabled = true;

            this._pic_changed = true;

            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            this.Timer3.Start();

            this._sw?.Stop();
            this.Text = "frmProcOutline";
            if (this._sw != null)
                this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
        }

        private void _cfop_ShowInfo(object? sender, string e)
        {
            if (!this.IsDisposed)
                if (InvokeRequired)
                    this.Invoke(new Action(() =>
                    {
                        this.toolStripStatusLabel4.Text = e;
                        if (e.StartsWith("pic "))
                            this.toolStripStatusLabel5.Text = e;
                        //if (e.StartsWith("outer pic-amount"))
                        //    this.label13.Text = e;
                        if (e.StartsWith("picOuter "))
                            this.toolStripStatusLabel1.Text = e;
                    }));
                else
                {
                    this.toolStripStatusLabel4.Text = e;
                    if (e.StartsWith("pic "))
                        this.toolStripStatusLabel5.Text = e;
                    //if (e.StartsWith("outer pic-amount"))
                    //    this.label13.Text = e;
                    if (e.StartsWith("picOuter "))
                        this.toolStripStatusLabel1.Text = e;
                }
        }

        private void Cfop_UpdateProgress(object? sender, GetAlphaMatte.ProgressEventArgs e)
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
                                            bmp = (Bitmap)img.Clone();

                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                                        Bitmap? bC = (Bitmap)bmp.Clone();
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
                                        bmp = (Bitmap)img.Clone();

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

                Bitmap? bWork = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();

                if (cbHalfSize.Checked && rbClosedForm.Checked)
                {
                    Bitmap bWork2 = ResampleBmp(bWork, 2);

                    Bitmap? bOld = bWork;
                    bWork = bWork2;
                    bOld.Dispose();
                    bOld = null;
                }

                // ((this.cbHalfSize.Checked && this.rbClosedForm.Checked) || (this.cbHalfSize2.Checked && this.rbBayes.Checked)) ? 2.0 : 1.0;

                Bitmap bTrimap = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                this.SetBitmap(ref _bWork, ref bWork);

                bool drawPaths = this.cbDrawPaths.Checked;
                float wFactor = (float)this.numScribblesWFactor.Value;
                bool drawBoth = this.cbDrawRectsAlso.Checked;
                bool roundCaps = this.cbRoundCaps.Checked;

                this.backgroundWorker3.RunWorkerAsync(new object[] { bTrimap, drawPaths, wFactor, drawBoth, roundCaps });
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

        private Bitmap? ResampleBack(Bitmap bmp, bool restoreFG)
        {
            if (this.helplineRulerCtrl1 != null && !this.IsDisposed && this.helplineRulerCtrl1.Bmp != null && bmp != null)
            {
                if (this._pointsDraw != null && this._pointsDraw.Count > 0)
                {
                    using GraphicsPath gP = new();
                    gP.AddLines(this._pointsDraw.Select(a => new PointF(a.X, a.Y)).ToArray());
                    RectangleF rTmp = gP.GetBounds();
                    Rectangle rClone = new Rectangle(
                        Math.Max((int)Math.Floor((rTmp.X - (float)this.numDrawPenWidth.Value / 2f)), 0),
                        Math.Max((int)Math.Floor((rTmp.Y - (float)this.numDrawPenWidth.Value / 2f)), 0),
                        Math.Min((int)Math.Ceiling((rTmp.Width + (float)this.numDrawPenWidth.Value)), this.helplineRulerCtrl2.Bmp.Width - 1),
                        Math.Min((int)Math.Ceiling((rTmp.Height + (float)this.numDrawPenWidth.Value)), this.helplineRulerCtrl2.Bmp.Height - 1));

                    Bitmap bOut = new Bitmap(rClone.Width, rClone.Height);

                    using (Graphics gx = Graphics.FromImage(bOut))
                    {
                        gx.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        gx.DrawImage(bmp, 0, 0, bOut.Width, bOut.Height);
                    }

                    if (restoreFG && this._fgMap != null)
                    {
                        using Bitmap bf = this._fgMap.Clone(rClone, bmp.PixelFormat);
                        using Bitmap bD = new Bitmap(rClone.Width, rClone.Height);
                        using Graphics gx2 = Graphics.FromImage(bD);
                        gx2.Clear(Color.White);
                        ChainFinder cf = new ChainFinder();
                        List<ChainCode> c = cf.GetOutline(bf, 0, true, 0, true, 0, false);

                        if (c.Count > 0)
                        {
                            using GraphicsPath gP2 = new GraphicsPath();

                            for (int i = 0; i < c.Count; i++)
                            {
                                if (c[i].Area > 0)
                                {
                                    gP2.AddLines(c[i].Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                                    gP2.CloseFigure();
                                }
                            }

                            using TextureBrush tb = new TextureBrush(bD);
                            using Graphics gx4 = Graphics.FromImage(bOut);
                            gx4.FillPath(tb, gP2);
                        }
                    }

                    return bOut;
                }
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

                int id = Environment.TickCount;
                this._lastRunNumber = id;

                e.Result = this._cfop.ProcessPicture(mode, scalesPics, scales, overlap, interpolated, forceSerial, group, groupAmountX,
                    groupAmountY, maxSize, bWork, trWork, trySingleTile, verifyTrimaps, Environment.TickCount, this.backgroundWorker1);
            }
            else
                e.Result = null;
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

        private async void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                {
                    using Bitmap bmpTmp = (Bitmap)e.Result;

                    int c = 0;
                    if (this.cbWOScribbles.Checked)
                        c = 10;
                    else if (this.rbSparseScribble.Checked)
                        c = 100;
                    bmp = bmpTmp.Clone(new Rectangle(this.rbSparseScribble.Checked ? 10 : 0, this.rbSparseScribble.Checked ? 10 : 0, bmpTmp.Width - c, bmpTmp.Height - c), bmpTmp.PixelFormat);

                    Bitmap? bC2 = (Bitmap)bmp.Clone();
                    this.SetBitmap(ref this._bmpMatteOrigSize, ref bC2);

                    Bitmap? b2 = bmp;
                    bmp = ResampleBack(bmp, this.cbRestore.Checked);

                    b2.Dispose();
                    b2 = null;

                    if (bmp != null)
                    {
                        Bitmap? bC = (Bitmap)bmp.Clone();
                        this.SetBitmap(ref _bmpMatte, ref bC);

                        if (this._bWorkPart != null)
                        {
                            if (this.cbHalfSize.Checked && this.cbRestrictMatte.Checked)
                            {
                                this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Minimum;
                                using (Bitmap? resMatte = await GetAlphaBoundsPic2((int)numMatteTh.Value))
                                {
                                    if (resMatte != null)
                                    {
                                        Bitmap? rMC = (Bitmap)resMatte.Clone();
                                        this.SetBitmap(ref this._resMatte, ref rMC);

                                        if (this._bmpMatteOrigSize != null)
                                        {
                                            int th = (int)this.numOTh.Value;
                                            using (Bitmap bMatteC = (Bitmap)this._bmpMatteOrigSize.Clone())
                                            {
                                                SetOpaqueGTh(bMatteC, th);
                                                SetFullLum2Shape(bMatteC, th);
                                                using (Graphics graphics = Graphics.FromImage(resMatte))
                                                    graphics.DrawImage(bMatteC, 0, 0, resMatte.Width, resMatte.Height);
                                            }
                                        }

                                        b2 = bmp;
                                        bmp = GetAlphaBoundsPic(this._bWorkPart, resMatte);
                                        b2.Dispose();
                                        b2 = null;
                                    }
                                    else
                                    {
                                        b2 = bmp;
                                        bmp = GetAlphaBoundsPic(this._bWorkPart, bmp);
                                        b2.Dispose();
                                        b2 = null;
                                    }
                                }
                            }
                            else
                            {
                                b2 = bmp;
                                bmp = GetAlphaBoundsPic(this._bWorkPart, bmp);
                                b2.Dispose();
                                b2 = null;
                            }
                        }
                    }
                }

                if (bmp != null && this._bWorkPart != null)
                {
                    using GraphicsPath gP = new();
                    gP.AddLines(this._pointsDraw.Select(a => new PointF(a.X, a.Y)).ToArray());

                    RectangleF rTmp = gP.GetBounds();

                    Rectangle rClone2 = new Rectangle(
                        Math.Max((int)Math.Floor((rTmp.X - (float)this.numDrawPenWidth.Value / 2f)), 0),
                        Math.Max((int)Math.Floor((rTmp.Y - (float)this.numDrawPenWidth.Value / 2f)), 0),
                        Math.Min((int)Math.Ceiling((rTmp.Width + (float)this.numDrawPenWidth.Value)), this.helplineRulerCtrl2.Bmp.Width - 1),
                        Math.Min((int)Math.Ceiling((rTmp.Height + (float)this.numDrawPenWidth.Value)), this.helplineRulerCtrl2.Bmp.Height - 1));

                    int fl = Math.Max((int)this.numDrawPenWidth.Value, 1);
                    using Bitmap b2 = new Bitmap(this._bWorkPart.Width + fl * 2, this._bWorkPart.Height + fl * 2);
                    using Graphics gx2 = Graphics.FromImage(b2);
                    gx2.PixelOffsetMode = PixelOffsetMode.Half;
                    gx2.CompositingMode = CompositingMode.SourceCopy;
                    gx2.TranslateTransform(-rClone2.X + fl, -rClone2.Y + fl);
                    gx2.DrawImage(this.helplineRulerCtrl2.Bmp, 0, 0);

                    using GraphicsPath gP2 = (GraphicsPath)gP.Clone();
                    using TextureBrush tb = new(bmp);
                    tb.TranslateTransform(rClone2.X, rClone2.Y);
                    using Pen p = new Pen(tb, Math.Max((int)this.numDrawPenWidth.Value, 1));
                    p.LineJoin = LineJoin.Round;
                    p.StartCap = LineCap.Round;
                    p.EndCap = LineCap.Round;
                    gx2.DrawPath(p, gP2);

                    using Graphics gx = Graphics.FromImage(this.helplineRulerCtrl2.Bmp);
                    gx.CompositingMode = CompositingMode.SourceCopy;
                    gx.PixelOffsetMode = PixelOffsetMode.Half;
                    gx.DrawImage(b2, rClone2.X - fl, rClone2.Y - fl);
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.Zoom = this.helplineRulerCtrl1.Zoom;
                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl2.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    if (this.cbWOScribbles.Checked)
                        this._undoOPCache?.Add(this.helplineRulerCtrl2.Bmp);
                    else
                        this._undoOPCache?.Add(bmp);

                    if (this._ptBU == null)
                        this._ptBU = new List<Point>();
                    this._ptBU.Clear();
                    this._ptBU.AddRange(this._pointsDraw);
                    this._pointsDraw.Clear();
                }

                if (this._cfop != null)
                {
                    this._cfop.ShowProgess -= Cfop_UpdateProgress;
                    this._cfop.ShowInfo -= _cfop_ShowInfo;
                    this._cfop.ShowInfoOuter -= Cfop_ShowInfoOuter;
                    this._cfop.Dispose();
                }

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                rbClosedForm_CheckedChanged(this.rbClosedForm, new EventArgs());

                this.btnOK.Enabled = this.btnCancel.Enabled = true;
                this.btnCancelBGW.Visible = this.btnCancelBGW.Enabled = false;
                this.btnRedoSameCoords.Enabled = true;

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

                        //if (doAlpha)
                        //p[3] = (byte)Math.Max(Math.Min(((double)pA[0] / 255.0 * (double)pIn[3]), 255), 0);
                        //else
                        //p[3] = pA[0];
                        p[3] = (byte)Math.Max(Math.Min(((double)pA[0] / 255.0 * (double)pIn[3]), 255), 0);

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

        private async Task<Bitmap?> GetAlphaBoundsPic2(int threshold)
        {
            if (this._bmpMatteOrigSize == null || this.helplineRulerCtrl1.Bmp == null)
                return null;

            using (Bitmap bMatteC = (Bitmap)this._bmpMatteOrigSize.Clone())
            using (Bitmap bmpFullLum = GetFullLumParts(bMatteC, threshold))
            {
                if (bmpFullLum != null)
                {
                    GetAlphaMatte.frmEdgePic frm = new GetAlphaMatte.frmEdgePic(bmpFullLum);
                    frm.Text = "Please check, if this picture should be used in the remaining processing steps. - MatteThreshold is " + threshold.ToString();
                    frm.btnClose.DialogResult = DialogResult.OK;
                    frm.btnClose.Text = "OK";
                    Point pt = frm.btnClose.Location;
                    frm.btnClose.Location = frm.btnCancel.Location;
                    frm.btnCancel.Location = pt;
                    frm.btnCancel.Visible = true;
                    frm.helplineRulerCtrl1.SetZoom("Fit_Width");
                    frm.BackColor = Color.Green;

                    if (frm.ShowDialog() == DialogResult.Cancel)
                        return null;
                }

                Bitmap? bmpRes = await Task.Run(() =>
                {
                    int w = 1;
                    int h = 1;

                    if (this._bWorkPart != null)
                    {
                        w = this._bWorkPart.Width;
                        h = this._bWorkPart.Height;
                    }

                    if (w < 2 || h < 2)
                        return null;

                    Bitmap bmpResult = new Bitmap(w, h);

                    if (this._bmpMatteOrigSize == null || this.helplineRulerCtrl1.Bmp == null)
                        return null;

                    if (this._bmpMatteOrigSize.Width == this.helplineRulerCtrl1.Bmp.Width &&
                        this._bmpMatteOrigSize.Height == this.helplineRulerCtrl1.Bmp.Height)
                        return null;

                    if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 16L))
                    {
                        //using (Bitmap bMatteC = new Bitmap(this._bmpMatteOrigSize))
                        {
                            if (bMatteC != null)
                            {
                                //using (Bitmap bmpFullLum = GetFullLumParts(bMatteC, threshold))
                                {
                                    //copy first opaque to target bmp
                                    using (Graphics gxF = Graphics.FromImage(bmpResult))
                                    {
                                        gxF.InterpolationMode = InterpolationMode.NearestNeighbor;
                                        gxF.PixelOffsetMode = PixelOffsetMode.Half;
                                        if (bmpFullLum != null)
                                            gxF.DrawImage(bmpFullLum, 0, 0, bmpResult.Width, bmpResult.Height);
                                    }

                                    CheckReversed(bmpResult, bMatteC);

                                    //to check the non opaque pixels
                                    //using (Bitmap bMatteC2 = new Bitmap(this._bmpMatteOrigSize))
                                    {
                                        int i = 0;
                                        int iMax = (int)Math.Sqrt(w * w + h * h) + 1;

                                        //while (CheckNonOpaquePixels(bMatteC2))   
                                        while (i < iMax)
                                        {
                                            using (Bitmap bmpMatteExcTargetSize = new Bitmap(w, h))
                                            {
                                                using (Bitmap bmpFullLumTargetSize = new Bitmap(w, h))
                                                {
                                                    using (Graphics gx = Graphics.FromImage(bmpFullLumTargetSize))
                                                    {
                                                        gx.InterpolationMode = InterpolationMode.HighQualityBicubic; //dont set pixeloffsetmode
                                                        if (bmpFullLum != null)
                                                            gx.DrawImage(bmpFullLum, 0, 0, bmpFullLumTargetSize.Width, bmpFullLumTargetSize.Height);
                                                    }

                                                    using (Bitmap? bSearch = GetOpaqueExcFullLum(bmpFullLum))
                                                    {
                                                        List<ChainCode> c = this.GetBoundary(bSearch, 254);

                                                        if (c != null && c.Count > 0)
                                                        {
                                                            //use a parameter to enable/disable this
                                                            //c = c.OrderByDescending(a => a.Area).ToList();
                                                            //if (c[0].Area > 0 && c[0].Area == bSearch.Width * bSearch.Height)
                                                            //    c.RemoveAt(0);

                                                            using (Bitmap bFindOMatte = FindInOMatte(bMatteC, c))
                                                            //resize found parts

                                                            using (Graphics gx2 = Graphics.FromImage(bmpMatteExcTargetSize))
                                                            {
                                                                gx2.Clear(Color.Transparent);
                                                                gx2.InterpolationMode = InterpolationMode.HighQualityBicubic; //dont set pixeloffsetmode
                                                                gx2.DrawImage(bFindOMatte, 0, 0, bmpMatteExcTargetSize.Width, bmpMatteExcTargetSize.Height);
                                                            }

                                                            //set sibling in checkPic to black
                                                            //SetCheckPicPartsBlack(bMatteC2, c);

                                                            //generate next level
                                                            if (bmpFullLum != null)
                                                                DrawFoundPartsOpaque(bmpFullLum, c);

                                                            //draw the sibling to the resized Matte
                                                            using (Graphics gx4 = Graphics.FromImage(bmpResult))
                                                                gx4.DrawImage(bmpMatteExcTargetSize, 0, 0, bmpFullLumTargetSize.Width, bmpFullLumTargetSize.Height);

                                                        }
                                                    }
                                                }

                                                if (CheckConvergence(bmpMatteExcTargetSize))
                                                    break;

                                                i++;

                                                this.Invoke(new Action(() =>
                                                {
                                                    this.toolStripStatusLabel4.Text = i.ToString();
                                                    if (i <= this.toolStripProgressBar1.Maximum)
                                                        this.toolStripProgressBar1.Value = i;
                                                    this.statusStrip1.Refresh();
                                                }));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return bmpResult;
                });

                return bmpRes;
            }
        }

        private unsafe void SetOpaqueGTh(Bitmap bMatteC, int threshold)
        {
            int w = bMatteC.Width;
            int h = bMatteC.Height;

            BitmapData bmD = bMatteC.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[0] > threshold)
                        p[0] = p[1] = p[2] = p[3] = 255;

                    p += 4;
                }
            });

            bMatteC.UnlockBits(bmD);
        }

        private unsafe void SetFullLum2Shape(Bitmap bmpFullLum2, int threshold)
        {
            int w = bmpFullLum2.Width;
            int h = bmpFullLum2.Height;

            BitmapData bmD = bmpFullLum2.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int stride = bmD.Stride;

            Parallel.For(0, h, (y, loopState) =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[0] <= threshold)
                        p[0] = p[1] = p[2] = p[3] = 0;

                    p += 4;
                }
            });

            bmpFullLum2.UnlockBits(bmD);
        }

        private unsafe void CheckReversed(Bitmap bmpMatteExcTargetSize, Bitmap matteOrig)
        {
            int w = bmpMatteExcTargetSize.Width;
            int h = bmpMatteExcTargetSize.Height;
            int w2 = matteOrig.Width;
            int h2 = matteOrig.Height;

            BitmapData bmD = bmpMatteExcTargetSize.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmSrc2 = matteOrig.LockBits(new Rectangle(0, 0, w2, h2), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int stride = bmD.Stride;
            int stride2 = bmSrc2.Stride;

            byte* pSrc2 = (byte*)bmSrc2.Scan0;

            Parallel.For(0, h, (y, loopState) =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[0] > 0 && pSrc2[(x / 2) * 4 + (y / 2) * stride2] > 0 && pSrc2[(x / 2) * 4 + (y / 2) * stride2] < p[0])
                        p[0] = p[1] = p[2] = p[3] = 0;

                    p += 4;
                }
            });

            matteOrig.UnlockBits(bmSrc2);
            bmpMatteExcTargetSize.UnlockBits(bmD);
        }

        private unsafe bool CheckConvergence(Bitmap bmpMatteExcTargetSize)
        {
            int w = bmpMatteExcTargetSize.Width;
            int h = bmpMatteExcTargetSize.Height;

            bool found = false;

            BitmapData bmD = bmpMatteExcTargetSize.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, (y, loopState) =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                if (found)
                    loopState.Break();

                for (int x = 0; x < w; x++)
                {
                    if (p[0] > 0)
                    {
                        found = true;
                        break;
                    }

                    p += 4;
                }
            });

            bmpMatteExcTargetSize.UnlockBits(bmD);

            return !found;
        }

        private unsafe void DrawImageByMin(Bitmap bmpResult, Bitmap bmpMatteExcTargetSize)
        {
            int w = bmpResult.Width;
            int h = bmpResult.Height;

            BitmapData bmD = bmpResult.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmSrc = bmpMatteExcTargetSize.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, (y, loopState) =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                byte* pSrc = (byte*)bmSrc.Scan0;
                pSrc += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (((pSrc[0] <= p[0]) && p[0] < 254) || ((pSrc[0] > p[0]) && p[0] == 0))
                        p[0] = p[1] = p[2] = pSrc[0];

                    p[3] = 255;

                    p += 4;
                    pSrc += 4;
                }
            });

            bmpMatteExcTargetSize.UnlockBits(bmSrc);
            bmpResult.UnlockBits(bmD);
        }

        private unsafe bool CheckNonOpaquePixels(Bitmap bMatteC2)
        {
            int w = bMatteC2.Width;
            int h = bMatteC2.Height;

            bool found = false;

            BitmapData bmD = bMatteC2.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, (y, loopState) =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                if (found)
                    loopState.Break();

                for (int x = 0; x < w; x++)
                {
                    if (p[0] < 255 && p[0] > 0)
                    {
                        found = true;
                        break;
                    }

                    p += 4;
                }
            });

            bMatteC2.UnlockBits(bmD);

            return found;
        }

        private unsafe void SetCheckPicPartsBlack(Bitmap bMatteC2, List<ChainCode> c)
        {
            int w = bMatteC2.Width;
            int h = bMatteC2.Height;

            BitmapData bmD = bMatteC2.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            byte* p = (byte*)bmD.Scan0;

            foreach (ChainCode cc in c)
            {
                if (cc.Coord.Count > 0)
                {
                    List<Point> points = cc.Coord;

                    foreach (Point pt in points)
                    {
                        int x = pt.X;
                        int y = pt.Y;

                        p[y * stride + x * 4] = 0;
                        p[y * stride + x * 4 + 1] = 0;
                        p[y * stride + x * 4 + 2] = 0;
                        p[y * stride + x * 4 + 3] = 255;
                    }
                }
            }

            bMatteC2.UnlockBits(bmD);
        }

        private unsafe void DrawFoundPartsOpaque(Bitmap bmpFullLum, List<ChainCode> c)
        {
            int w = bmpFullLum.Width;
            int h = bmpFullLum.Height;

            BitmapData bmD = bmpFullLum.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            byte* p = (byte*)bmD.Scan0;

            foreach (ChainCode cc in c)
            {
                if (cc.Coord.Count > 0)
                {
                    List<Point> points = cc.Coord;

                    foreach (Point pt in points)
                    {
                        int x = pt.X;
                        int y = pt.Y;

                        p[y * stride + x * 4] = 255;
                        p[y * stride + x * 4 + 1] = 255;
                        p[y * stride + x * 4 + 2] = 255;
                        p[y * stride + x * 4 + 3] = 255;
                    }
                }
            }

            bmpFullLum.UnlockBits(bmD);
        }

        private unsafe Bitmap FindInOMatte(Bitmap bMatteC, List<ChainCode> c)
        {
            int w = bMatteC.Width;
            int h = bMatteC.Height;

            Bitmap bWork = new Bitmap(bMatteC.Width, bMatteC.Height);

            BitmapData bmD = bWork.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmSrc = bMatteC.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            byte* p = (byte*)bmD.Scan0;
            byte* pSrc = (byte*)bmSrc.Scan0;

            foreach (ChainCode cc in c)
            {
                if (cc.Coord.Count > 0)
                {
                    List<Point> points = cc.Coord;

                    foreach (Point pt in points)
                    {
                        int x = pt.X;
                        int y = pt.Y;

                        p[y * stride + x * 4] = pSrc[y * stride + x * 4];
                        p[y * stride + x * 4 + 1] = pSrc[y * stride + x * 4 + 1];
                        p[y * stride + x * 4 + 2] = pSrc[y * stride + x * 4 + 2];
                        p[y * stride + x * 4 + 3] = pSrc[y * stride + x * 4 + 3];
                    }
                }
            }

            bMatteC.UnlockBits(bmSrc);
            bWork.UnlockBits(bmD);

            return bWork;
        }

        private unsafe Bitmap? GetOpaqueExcFullLum(Bitmap? bmpFullLum)
        {
            if (bmpFullLum != null)
            {
                int w = bmpFullLum.Width;
                int h = bmpFullLum.Height;

                Bitmap bWork = (Bitmap)bmpFullLum.Clone();

                BitmapData bmD = bWork.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    p += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        if (p[0] == 255)
                            p[0] = p[1] = p[2] = p[3] = 0;
                        else
                            p[0] = p[1] = p[2] = p[3] = 255;

                        p += 4;
                    }
                });

                bWork.UnlockBits(bmD);

                return bWork;
            }

            return null;
        }

        private unsafe Bitmap GetFullLumParts(Bitmap bMatteC, int threshold)
        {
            int w = bMatteC.Width;
            int h = bMatteC.Height;

            Bitmap bWork = (Bitmap)bMatteC.Clone();

            BitmapData bmD = bWork.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[0] < threshold)
                        p[0] = p[1] = p[2] = 0;

                    p += 4;
                }
            });

            bWork.UnlockBits(bmD);

            return bWork;
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

                if (this._cfop?.CfopArray != null && this._cfop?.CfopArray.Length > 0)
                {
                    for (int i = 0; i < this._cfop?.CfopArray.Length; i++)
                        if (this._cfop?.CfopArray[i] != null)
                            try
                            {
                                BlendParameters? cb = this._cfop?.CfopArray[i].BlendParameters;
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

            this.cbRestrictMatte.Checked = false;

            this.numScribblesWFactor.Value = (decimal)Math.Sqrt(2.0);

            this.cmbZoom.Items.Add((0.75F).ToString());
            this.cmbZoom.Items.Add((0.5F).ToString());
            this.cmbZoom.Items.Add((0.25F).ToString());

            this.cmbZoom.SelectedIndex = 4;
        }

        private async void floodBGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetControls(false);
            this.btnCancelBGW.Text = "Cancel";
            this.btnCancelBGW.Enabled = true;
            FloodFillMethods.Cancel = false;

            await Task.Run(() =>
            {
                Point pt = this._ptHLC1BG;
                this._ptHLC1BG = this._ptHLC1FGBG;
                this.btnFloodBG.Enabled = true;
                this.btnFloodBG_Click(this.btnFloodBG, new EventArgs());
                this._ptHLC1BG = pt;
            });

            this.SetControls(true);
            this.btnCancelBGW.Text = "Cancel";
        }

        private async void floodFGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetControls(false);
            this.btnCancelBGW.Text = "Cancel";
            this.btnCancelBGW.Enabled = true;
            FloodFillMethods.Cancel = false;

            await Task.Run(() =>
            {
                Point pt = this._ptHLC1FG;
                this._ptHLC1FG = this._ptHLC1FGBG; //!
                this.btnFloodFG.Enabled = true;
                this.btnFloodFG_Click(this.btnFloodFG, new EventArgs());
                this._ptHLC1FG = pt;
            });

            this.SetControls(true);
            this.btnCancelBGW.Text = "Cancel";
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

        private void backgroundWorker2_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (this._sw != null)
                if (!InvokeRequired)
                {
                    this.Text = "frmAvoidAGrabCut";
                    this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
                    if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                        this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
                }
                else
                    this.Invoke(new Action(() =>
                    {
                        this.Text = "frmAvoidAGrabCut";
                        if (!this.IsDisposed && this.Visible && !this.toolStripProgressBar1.IsDisposed)
                            this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();
                        this.toolStripProgressBar1.Value = Math.Max(Math.Min(e.ProgressPercentage, this.toolStripProgressBar1.Maximum), 0);
                    }));
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

                Bitmap b = (Bitmap)this.helplineRulerCtrl2.Bmp.Clone();
                bool redrawExcluded = false;

                string? c = this.CachePathAddition;
                if (c != null && this.cbExcludeRegions.Checked)
                {
                    bool show = true;
                    if (this._excludedRegions != null && this._excludedRegions.Count > 0)
                    {
                        if (MessageBox.Show("Use existing Exclusions?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            redrawExcluded = true;
                            show = false;
                        }
                    }

                    if (show)
                    {
                        using frmExcludeFromPic frm = new frmExcludeFromPic(b, c);
                        frm.SetupCache();

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (frm.ExcludedBmpRegions != null)
                            {
                                using Graphics gx = Graphics.FromImage(b);
                                for (int i = 0; i < frm.ExcludedBmpRegions.Count; i++)
                                {
                                    Bitmap? rem = frm.ExcludedBmpRegions[i].Remaining;
                                    if (rem != null)
                                        SetTransp(b, rem, frm.ExcludedBmpRegions[i].Location);
                                }

                                CopyRegions(frm.ExcludedBmpRegions);
                                redrawExcluded = true;
                            }
                        }
                    }
                }

                if (c != null && this.cbExcludeFG.Checked)
                {
                    bool show = true;
                    if (this._excludedRegions != null && this._excludedRegions.Count > 0)
                    {
                        if (MessageBox.Show("Use existing Exclusions?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            redrawExcluded = true;
                            show = false;
                        }
                    }

                    if (show)
                    {
                        using frmDefineFGPic frm = new frmDefineFGPic(b, c);
                        frm.SetupCache();

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (frm.Excluded != null)
                            {
                                using Graphics gx = Graphics.FromImage(b);

                                Bitmap? rem = frm.Excluded.Remaining;
                                if (rem != null)
                                {
                                    if (frm.cbSetOpaque.Checked)
                                    {
                                        SetOpaque(b, rem);
                                        SetOpaque(rem, rem);
                                    }
                                    SetTransp(b, rem, frm.Excluded.Location);
                                }

                                List<ExcludedBmpRegion> l = new();
                                l.Add(frm.Excluded);
                                CopyRegions(l);
                                redrawExcluded = true;
                            }
                        }
                    }
                }

                this.backgroundWorker5.RunWorkerAsync(new object[] { b, gamma, redrawExcluded });
            }
        }

        private unsafe void SetOpaque(Bitmap bWrite, Bitmap bRead)
        {
            int w = bWrite.Width;
            int h = bWrite.Height;

            if (bWrite.Equals(bRead))
            {
                BitmapData bmD = bWrite.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    p += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        if (p[3] > 0 && p[3] < 255)
                            p[3] = 255;
                        p += 4;
                    }
                });

                bWrite.UnlockBits(bmD);
            }
            else
            {
                BitmapData bmD = bWrite.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmR = bRead.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    p += y * stride;
                    byte* pR = (byte*)bmR.Scan0;
                    pR += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        if (pR[3] > 0 && pR[3] < 255)
                            p[3] = 255;
                        p += 4;
                        pR += 4;
                    }
                });

                bWrite.UnlockBits(bmD);
                bRead.UnlockBits(bmR);
            }
        }

        private void backgroundWorker5_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                using Bitmap bmp = (Bitmap)o[0];
                double gamma = (double)o[1];
                bool redrawExcluded = (bool)o[2];

                Bitmap? b = GetAlphaPic(bmp, gamma);
                if (b != null)
                    e.Result = new object[] { b, redrawExcluded };
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
                {
                    object[] o = (object[])e.Result;
                    bmp = (Bitmap)o[0];

                    if ((bool)o[1])
                    {
                        if (this._excludedRegions != null && this._exclLocations != null)
                        {
                            using Graphics gx = Graphics.FromImage(bmp);
                            for (int i = 0; i < this._excludedRegions.Count; i++)
                                gx.DrawImage(this._excludedRegions[i], this._exclLocations[i]);
                        }
                    }

                    //since ...
                    SetColorsToOrig(bmp, this.helplineRulerCtrl2.Bmp);
                }

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    _undoOPCache?.Add(bmp);
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

        private void cbHalfSize2_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            this.labelGo.Enabled = this.numDrawPenWidth.Enabled = this._lbGo = c.Checked == this._hs && this.pictureBox1.Image != null;
        }

        private void cbHalfSize_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            this.labelGo.Enabled = this.numDrawPenWidth.Enabled = this._lbGo = c.Checked == this._hs && this.pictureBox1.Image != null;

            this.cbRestrictMatte.Enabled = this.cbHalfSize.Checked;
            this.label7.Enabled = this.numOTh.Enabled = this.label17.Enabled = this.numMatteTh.Enabled = this.cbHalfSize.Checked && this.cbRestrictMatte.Checked;
        }

        private void backgroundWorker3_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap bTrimap = (Bitmap)o[0];
                bool drawPaths = (bool)o[1];
                float wFactor = (float)o[2];
                bool drawBoth = (bool)o[3];
                bool roundCaps = (bool)o[4];
                Bitmap? bFGMap = (Bitmap)((Bitmap)o[0]).Clone();

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
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    foreach (Point pt in ptsList[listNo])
                                                    {
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                                (int)Math.Max(wh, 1),
                                                                (int)Math.Max(wh, 1)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)Math.Max(wh, 1),
                                                                    (int)Math.Max(wh, 1)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (Pen pen = new Pen(c, (wh * wFactor)))
                                                    {
                                                        pen.LineJoin = LineJoin.Round;
                                                        if (roundCaps)
                                                        {
                                                            pen.StartCap = LineCap.Round;
                                                            pen.EndCap = LineCap.Round;
                                                        }
                                                        using (GraphicsPath gP = new GraphicsPath())
                                                        {
                                                            gP.AddLines(ptsList[listNo].Select(a => new PointF(a.X, a.Y)).ToArray());
                                                            using (Matrix mx = new Matrix(1.0f, 0, 0, 1.0f, 0, 0))
                                                                gP.Transform(mx);
                                                            gx.DrawPath(pen, gP);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    if (ptsList[listNo].Count > 0)
                                                    {
                                                        Point pt = ptsList[listNo][0];
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                                (int)Math.Max(wh, 1),
                                                                (int)Math.Max(wh, 1)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)Math.Max(wh, 1),
                                                                    (int)Math.Max(wh, 1)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (GraphicsPath gP = new GraphicsPath())
                                                    {
                                                        gP.AddEllipse(ptsList[listNo][0].X - wh / 2f,
                                                           ptsList[listNo][0].Y - wh / 2f,
                                                           wh, wh);
                                                        gx.FillPath(sb, gP);
                                                    }
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
                                            int wh = (int)Math.Max(i, 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X), (int)(a.Y))).ToList();

                                                foreach (Point pt in pts)
                                                {
                                                    gx.FillRectangle(Brushes.White, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                    using (Pen pen = new Pen(Color.White, 1))
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
                                            int wh = (int)Math.Max(i, 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X), (int)(a.Y))).ToList();

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
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    foreach (Point pt in ptsList[listNo])
                                                    {
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                            (int)(wh),
                                                                (int)(wh)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)(wh),
                                                                    (int)(wh)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (Pen pen = new Pen(c, (wh * wFactor)))
                                                    {
                                                        pen.LineJoin = LineJoin.Round;
                                                        if (roundCaps)
                                                        {
                                                            pen.StartCap = LineCap.Round;
                                                            pen.EndCap = LineCap.Round;
                                                        }
                                                        using (GraphicsPath gP = new GraphicsPath())
                                                        {
                                                            gP.AddLines(ptsList[listNo].Select(a => new PointF(a.X, a.Y)).ToArray());
                                                            using (Matrix mx = new Matrix(1.0f, 0, 0, 1.0f, 0, 0))
                                                                gP.Transform(mx);
                                                            gx.DrawPath(pen, gP);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    if (ptsList[listNo].Count > 0)
                                                    {
                                                        Point pt = ptsList[listNo][0];
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                                (int)(wh),
                                                                (int)(wh)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)(wh),
                                                                    (int)(wh)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (GraphicsPath gP = new GraphicsPath())
                                                    {
                                                        gP.AddEllipse(ptsList[listNo][0].X - wh / 2f,
                                                            ptsList[listNo][0].Y - wh / 2f,
                                                            wh, wh);
                                                        gx.FillPath(sb, gP);
                                                    }
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
                                            int wh = (int)Math.Max(i, 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X), (int)(a.Y))).ToList();

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
                                            int wh = (int)Math.Max(i, 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X), (int)(a.Y))).ToList();

                                                foreach (Point pt in pts)
                                                {
                                                    gx.FillRectangle(Brushes.White, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                    using (Pen pen = new Pen(Color.White, 1))
                                                        gx.DrawRectangle(pen, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        if (this._oldUnknownFT != null)
                            this.FillUnknownByBG(bTrimap, this._oldUnknownFT);
                    }
                }

                using (Graphics gx = Graphics.FromImage(bFGMap))
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
                                    int l = 1;
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
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    foreach (Point pt in ptsList[listNo])
                                                    {
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                                (int)Math.Max(wh, 1),
                                                                (int)Math.Max(wh, 1)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)Math.Max(wh, 1),
                                                                    (int)Math.Max(wh, 1)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (Pen pen = new Pen(c, (wh * wFactor)))
                                                    {
                                                        pen.LineJoin = LineJoin.Round;
                                                        if (roundCaps)
                                                        {
                                                            pen.StartCap = LineCap.Round;
                                                            pen.EndCap = LineCap.Round;
                                                        }
                                                        using (GraphicsPath gP = new GraphicsPath())
                                                        {
                                                            gP.AddLines(ptsList[listNo].Select(a => new PointF(a.X, a.Y)).ToArray());
                                                            using (Matrix mx = new Matrix(1.0f, 0, 0, 1.0f, 0, 0))
                                                                gP.Transform(mx);
                                                            gx.DrawPath(pen, gP);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    if (ptsList[listNo].Count > 0)
                                                    {
                                                        Point pt = ptsList[listNo][0];
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                                (int)Math.Max(wh, 1),
                                                                (int)Math.Max(wh, 1)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)Math.Max(wh, 1),
                                                                    (int)Math.Max(wh, 1)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (GraphicsPath gP = new GraphicsPath())
                                                    {
                                                        gP.AddEllipse(ptsList[listNo][0].X - wh / 2f,
                                                           ptsList[listNo][0].Y - wh / 2f,
                                                           wh, wh);
                                                        gx.FillPath(sb, gP);
                                                    }
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
                                            int wh = (int)Math.Max(i, 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X), (int)(a.Y))).ToList();

                                                foreach (Point pt in pts)
                                                {
                                                    gx.FillRectangle(Brushes.White, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                    using (Pen pen = new Pen(Color.White, 1))
                                                        gx.DrawRectangle(pen, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                }
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
                                    int l = 1;
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
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    foreach (Point pt in ptsList[listNo])
                                                    {
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                            (int)(wh),
                                                                (int)(wh)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)(wh),
                                                                    (int)(wh)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (Pen pen = new Pen(c, (wh * wFactor)))
                                                    {
                                                        pen.LineJoin = LineJoin.Round;
                                                        if (roundCaps)
                                                        {
                                                            pen.StartCap = LineCap.Round;
                                                            pen.EndCap = LineCap.Round;
                                                        }
                                                        using (GraphicsPath gP = new GraphicsPath())
                                                        {
                                                            gP.AddLines(ptsList[listNo].Select(a => new PointF(a.X, a.Y)).ToArray());
                                                            using (Matrix mx = new Matrix(1.0f, 0, 0, 1.0f, 0, 0))
                                                                gP.Transform(mx);
                                                            gx.DrawPath(pen, gP);
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if ((!drawPaths || f.Item4) || drawBoth)
                                                    if (ptsList[listNo].Count > 0)
                                                    {
                                                        Point pt = ptsList[listNo][0];
                                                        using (SolidBrush sb = new SolidBrush(c))
                                                            gx.FillRectangle(sb, new Rectangle(
                                                                (int)((int)(pt.X - wh / 2)),
                                                                (int)((int)(pt.Y - wh / 2)),
                                                                (int)(wh),
                                                                (int)(wh)));
                                                        if (l == 1)
                                                            using (Pen pen = new Pen(c, 1))
                                                                gx.DrawRectangle(pen, new Rectangle(
                                                                    (int)((int)(pt.X - wh / 2)),
                                                                    (int)((int)(pt.Y - wh / 2)),
                                                                    (int)(wh),
                                                                    (int)(wh)));
                                                    }

                                                if (drawPaths && !f.Item4)
                                                {
                                                    using (SolidBrush sb = new SolidBrush(c))
                                                    using (GraphicsPath gP = new GraphicsPath())
                                                    {
                                                        gP.AddEllipse(ptsList[listNo][0].X - wh / 2f,
                                                            ptsList[listNo][0].Y - wh / 2f,
                                                            wh, wh);
                                                        gx.FillPath(sb, gP);
                                                    }
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
                                            int wh = (int)Math.Max(i, 3);

                                            for (int j = 0; j < list.Count; j++)
                                            {
                                                List<Point> pts = list[j].Select(a => new Point((int)(a.X), (int)(a.Y))).ToList();

                                                foreach (Point pt in pts)
                                                {
                                                    gx.FillRectangle(Brushes.White, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                    using (Pen pen = new Pen(Color.White, 1))
                                                        gx.DrawRectangle(pen, new Rectangle(pt.X - wh / 2, pt.Y - wh / 2, wh, wh));
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }

                        if (this._oldUnknownFT != null)
                            this.FillUnknownByBG(bTrimap, this._oldUnknownFT);
                    }
                }

                this.SetBitmap(ref this._fgMap, ref bFGMap);

                e.Result = bTrimap;
            }
        }

        private void backgroundWorker3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap bTrimap = (Bitmap)e.Result;
                if (bTrimap != null)
                {
                    if (this.cbHalfSize.Checked && this.rbClosedForm.Checked)
                    {
                        if (this._bWork != null)
                        {
                            Bitmap bTrimapNew = ResampleTrimap(bTrimap, this._bWork.Width, this._bWork.Height);

                            Bitmap? tOld = bTrimap;
                            bTrimap = bTrimapNew;
                            if (tOld != null)
                                tOld.Dispose();
                            tOld = null;
                        }
                    }

                    Image? iOld = this.pictureBox1.Image;
                    this.pictureBox1.Image = bTrimap;
                    if (iOld != null)
                        iOld.Dispose();
                    iOld = null;

                    this.labelGo.Enabled = this.numDrawPenWidth.Enabled = this._lbGo = true;

                    Bitmap bC = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bC, this.helplineRulerCtrl2, "Bmp");
                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);

                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();
                }
            }

            this.Cursor = Cursors.Default;
            this.SetControls(true);

            this._hs = this.cbHalfSize.Checked;
            rbClosedForm_CheckedChanged(this.rbClosedForm, new EventArgs());

            this.btnGenerateTrimap.Text = "gen trimap";

            this.btnOK.Enabled = this.btnCancel.Enabled = true;

            this.backgroundWorker3.Dispose();
            this.backgroundWorker3 = new BackgroundWorker();
            this.backgroundWorker3.WorkerReportsProgress = true;
            this.backgroundWorker3.WorkerSupportsCancellation = true;
            this.backgroundWorker3.DoWork += backgroundWorker3_DoWork;
            //this.backgroundWorker3.ProgressChanged += backgroundWorker3_ProgressChanged;
            this.backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
        }

        private Bitmap ResampleTrimap(Bitmap trimap, int width, int height)
        {
            Bitmap bOut = new Bitmap(width, height);
            using (Graphics gx = Graphics.FromImage(bOut))
            {
                gx.SmoothingMode = SmoothingMode.None;
                gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                gx.DrawImage(trimap, 0, 0, bOut.Width, bOut.Height);
            }

            return bOut;
        }

        private void btnAlphaZAndGain_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker4.IsBusy)
            {
                this.backgroundWorker4.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl2.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnAlphaZAndGain.Enabled = true;
                this.btnAlphaZAndGain.Text = "Cancel";

                int alphaTh = (int)this.numAlphaZAndGain.Value;

                Bitmap b = (Bitmap)this.helplineRulerCtrl2.Bmp.Clone();
                bool redrawExcluded = false;

                string? c = this.CachePathAddition;
                if (c != null && this.cbExcludeRegions.Checked)
                {
                    bool show = true;
                    if (this._excludedRegions != null && this._excludedRegions.Count > 0)
                    {
                        if (MessageBox.Show("Use existing Exclusions?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            redrawExcluded = true;
                            show = false;
                        }
                    }

                    if (show)
                    {
                        using frmExcludeFromPic frm = new frmExcludeFromPic(b, c);
                        frm.SetupCache();

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (frm.ExcludedBmpRegions != null)
                            {
                                using Graphics gx = Graphics.FromImage(b);
                                for (int i = 0; i < frm.ExcludedBmpRegions.Count; i++)
                                {
                                    Bitmap? rem = frm.ExcludedBmpRegions[i].Remaining;
                                    if (rem != null)
                                        SetTransp(b, rem, frm.ExcludedBmpRegions[i].Location);
                                }

                                CopyRegions(frm.ExcludedBmpRegions);
                                redrawExcluded = true;
                            }
                        }
                    }
                }

                if (c != null && this.cbExcludeFG.Checked)
                {
                    bool show = true;
                    if (this._excludedRegions != null && this._excludedRegions.Count > 0)
                    {
                        if (MessageBox.Show("Use existing Exclusions?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            redrawExcluded = true;
                            show = false;
                        }
                    }

                    if (show)
                    {
                        using frmDefineFGPic frm = new frmDefineFGPic(b, c);
                        frm.SetupCache();

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (frm.Excluded != null)
                            {
                                using Graphics gx = Graphics.FromImage(b);

                                Bitmap? rem = frm.Excluded.Remaining;
                                if (rem != null)
                                {
                                    if (frm.cbSetOpaque.Checked)
                                    {
                                        SetOpaque(b, rem);
                                        SetOpaque(rem, rem);
                                    }
                                    SetTransp(b, rem, frm.Excluded.Location);
                                }

                                List<ExcludedBmpRegion> l = new();
                                l.Add(frm.Excluded);
                                CopyRegions(l);
                                redrawExcluded = true;
                            }
                        }
                    }
                }

                this.backgroundWorker4.RunWorkerAsync(new object[] { b, alphaTh, redrawExcluded });
            }
        }

        private unsafe void SetTransp(Bitmap bOrig, Bitmap rem, Point location)
        {
            int w = bOrig.Width;
            int h = bOrig.Height;
            int wR = rem.Width;
            int hR = rem.Height;

            int xx = location.X;
            int yy = location.Y;

            BitmapData bmRead = rem.LockBits(new Rectangle(0, 0, wR, hR), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmOrig = bOrig.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmOrig.Stride;
            int strideR = bmRead.Stride;

            Parallel.For(yy, yy + hR, y =>
            //for(int y = yy; y < yy+hR;y++)
            {
                byte* pR = (byte*)bmRead.Scan0;
                pR += (y - yy) * strideR;
                byte* pO = (byte*)bmOrig.Scan0;
                pO += y * stride + xx * 4;

                for (int x = 0; x < wR; x++)
                {
                    if (pR[3] > 0)
                        pO[3] = 0;
                    pR += 4;
                    pO += 4;
                }
            });

            rem.UnlockBits(bmRead);
            bOrig.UnlockBits(bmOrig);
        }

        private void CopyRegions(List<ExcludedBmpRegion> excludedRegions)
        {
            if (this._excludedRegions != null)
                for (int j = this._excludedRegions.Count - 1; j >= 0; j--)
                    this._excludedRegions[j].Dispose();
            else
                this._excludedRegions = new List<Bitmap>();
            this._excludedRegions.Clear();

            if (this._exclLocations == null)
                this._exclLocations = new List<Point>();

            this._exclLocations.Clear();

            for (int j = 0; j < excludedRegions.Count; j++)
            {
                Bitmap? excl = excludedRegions[j].Remaining;
                if (excl != null)
                {
                    this._excludedRegions.Add((Bitmap)excl.Clone());
                    this._exclLocations.Add(excludedRegions[j].Location);
                }
            }
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                using Bitmap bmp = (Bitmap)((Bitmap)o[0]).Clone();
                double alphaTh = (int)o[1];
                bool redrawExcluded = (bool)o[2];

                Bitmap? b = GetAlphaZAndGainPic(bmp, alphaTh);

                if (b != null)
                    e.Result = new object[] { b, redrawExcluded };
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

        private void backgroundWorker4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                {
                    object[] o = (object[])e.Result;
                    bmp = (Bitmap)o[0];

                    if ((bool)o[1])
                    {
                        if (this._excludedRegions != null && this._exclLocations != null)
                        {
                            using Graphics gx = Graphics.FromImage(bmp);
                            gx.SmoothingMode = SmoothingMode.None;
                            gx.InterpolationMode = InterpolationMode.NearestNeighbor;

                            for (int i = 0; i < this._excludedRegions.Count; i++)
                            {
                                SetTransp(bmp, this._excludedRegions[i], this._exclLocations[i]);
                                gx.DrawImage(this._excludedRegions[i], this._exclLocations[i]);
                            }
                        }
                    }

                    //since ...
                    SetColorsToOrig(bmp, this.helplineRulerCtrl2.Bmp);
                }

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

                this.backgroundWorker4.Dispose();
                this.backgroundWorker4 = new BackgroundWorker();
                this.backgroundWorker4.WorkerReportsProgress = true;
                this.backgroundWorker4.WorkerSupportsCancellation = true;
                this.backgroundWorker4.DoWork += backgroundWorker4_DoWork;
                //this.backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
                this.backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
            }
        }

        private unsafe void SetColorsToOrig(Bitmap bmpWrite, Bitmap bmpRead)
        {
            int w = bmpWrite.Width;
            int h = bmpRead.Height;

            BitmapData bmD = bmpWrite.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmR = bmpRead.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;
                byte* pR = (byte*)bmR.Scan0;
                pR += y * stride;

                for (int x = 0; x < w; x++)
                {
                    p[0] = pR[0];
                    p[1] = pR[1];
                    p[2] = pR[2];

                    p += 4;
                    pR += 4;
                }
            });

            bmpWrite.UnlockBits(bmD);
            bmpRead.UnlockBits(bmR);
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

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (this.pictureBox1.Image != null)
            {
                GetAlphaMatte.frmEdgePic frm4 = new GetAlphaMatte.frmEdgePic(this.pictureBox1.Image, this.helplineRulerCtrl1.Bmp.Size);
                frm4.Text = "Trimap";
                frm4.ShowDialog();
            }
        }

        private void btnCMNew_Click(object sender, EventArgs e)
        {
            this._ptSt = null;
        }

        private void btnRScribbles_Click(object sender, EventArgs e)
        {
            if (this._scribbles != null)
            {
                using (frmResizeScribbles frm = new frmResizeScribbles())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        double resPic = 1.0 / (double)frm.numRScribblesFactor.Value;

                        if (resPic != 1.0 && resPic > 0)
                        {
                            Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles2 = ResizeAllScribbles(this._scribbles, resPic);
                            this._scribbles = scribbles2;
                            List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = ResizeScribbleSeq(this._scribbleSeq, resPic, true);
                            this._scribbleSeq = scribbleSeq2;

                            this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        }
                    }
                }
            }
        }

        private void btnOutlineOperations_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && this.CachePathAddition != null)
            {
                using (frnOutlineOperations frm = new frnOutlineOperations(this.helplineRulerCtrl2.Bmp, this.CachePathAddition))
                {
                    frm.SetupCache();

                    if (this.helplineRulerCtrl1.Bmp != null)
                        frm.SetOrig(this.helplineRulerCtrl1.Bmp);
                    if (this._bmpMatte != null)
                        frm.SetMatte(this._bmpMatte);

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (frm.FBitmap != null)
                        {
                            Bitmap b = (Bitmap)frm.FBitmap.Clone();

                            this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                            //Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                            //this.SetBitmap(ref this._b4Copy, ref bC);

                            this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                            this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                            this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                                (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                                (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                            _undoOPCache?.Add(b);

                            this.btnOK.Enabled = true;
                        }
                    }
                }
            }
        }

        private async void floodFGRemOuterInnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SetControls(false);
            this.btnCancelBGW.Text = "Cancel";
            this.btnCancelBGW.Enabled = true;
            FloodFillMethods.Cancel = false;

            await Task.Run(() =>
            {
                Point pt = this._ptHLC1FG;
                this._ptHLC1FG = this._ptHLC1FGBG; //!
                this.btnFloodFG2.Enabled = true;
                this.btnFloodFG2_Click(this.btnFloodFG2, new EventArgs());
                this._ptHLC1FG = pt;
            });

            this.SetControls(true);
            this.btnCancelBGW.Text = "Cancel";
        }

        private unsafe void btnFloodFG2_Click(object sender, EventArgs e)
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

                Bitmap b = this._scribblesBitmap;

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

                    List<Point> ll = new List<Point>();
                    int w = b.Width;
                    int h = b.Height;

                    using (Bitmap bCurrentUnknown = (Bitmap)b.Clone())
                    {
                        //redraw a bit thicker when we halfsize the pic
                        if (this.cbHalfSize.Checked)
                            using (Graphics gx = Graphics.FromImage(bCurrentUnknown))
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
                                                            using (Pen pen = new Pen(c, 2))
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
                                                        using (Pen pen = new Pen(c, 2))
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

                        int remOuter = (int)this.numRemOuter.Value;
                        int remInner = (int)this.numRemInner.Value;

                        //remove inner part
                        int n = (int)this.numWHScribbles.Value;
                        using (Bitmap bForeground = remInner == 0 ? (Bitmap)b.Clone() : RemoveInnerOutlineEx(b, remInner, true))
                        {
                            //this.SubtractToZero(b, bForeground);

                            //remove outer part
                            using (Bitmap bForeground2 = remOuter == 0 ? (Bitmap)bForeground.Clone() : RemoveOutlineEx(bForeground, remOuter, true))
                            {
                                this.DrawUnKnown(bForeground2);

                                startColor = bForeground2.GetPixel(ptSt.X, ptSt.Y);

                                //floodfill fg
                                if (!FloodFillMethods.Cancel)
                                    FloodFillMethods.floodfill(bForeground2, ptSt.X, ptSt.Y, 125, startColor, replaceColor,
                                        Int32.MaxValue, false, false, 1.0, false, false);

                                //now get the outer (= removed) unkknown scribbles for the trimap
                                if (this._oldUnknownFT == null)
                                    this._oldUnknownFT = new Bitmap(b.Width, b.Height);

                                using (Bitmap bForeground4 = RemoveInnerOutlineEx(bForeground, 1, true))
                                using (Graphics g = Graphics.FromImage(this._oldUnknownFT))
                                    //g.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    //g.PixelOffsetMode = PixelOffsetMode.Half;
                                    g.DrawImage(bForeground4, 0, 0);

                                this.ClearForeground(this._oldUnknownFT, bForeground2);

                                Bitmap? bOld2 = b;
                                b = (Bitmap)bForeground2.Clone();
                                this._scribblesBitmap = b;
                                if (bOld2 != null)
                                    bOld2.Dispose();
                                bOld2 = null;
                            }
                        }

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
                    }
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
                    if (this._scribbles[1][3].Count > 0)
                        this._scribbleSeq.Add(Tuple.Create(1, 3, this._scribbles[1][3].Count - 1, true, GetBoundariesForScribbleFill(this._scribbles[1][3][this._scribbles[1][3].Count - 1], w, h)));

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private unsafe void ClearForeground(Bitmap oldUnknownFT, Bitmap b)
        {
            int w = oldUnknownFT.Width;
            int h = oldUnknownFT.Height;

            BitmapData bmD = oldUnknownFT.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmRead = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                byte* pRead = (byte*)bmRead.Scan0;
                p += y * stride;
                pRead += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pRead[3] > 0)
                        p[0] = p[1] = p[2] = p[3] = (byte)0;

                    p += 4;
                    pRead += 4;
                }
            });

            oldUnknownFT.UnlockBits(bmD);
            b.UnlockBits(bmRead);
        }

        private unsafe void DrawUnKnown(Bitmap b)
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
                    if (p[3] > 0)
                    {
                        p[0] = (byte)128;
                        p[1] = (byte)128;
                        p[2] = (byte)128;
                        p[3] = (byte)255;
                    }


                    p += 4;
                }
            });

            b.UnlockBits(bmD);
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

            if (lInner != null && lInner.Count > 0)
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

            if (lInner != null && dontFill)
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

        private Bitmap RemoveInnerOutlineEx(Bitmap bmp, int innerW, bool dontFill)
        {
            Bitmap bOut = new Bitmap(bmp.Width, bmp.Height);

            using (Bitmap? b = RemInnerOutline(bmp, innerW, null))
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

            if (lInner != null && lInner.Count > 0)
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
                                    using (Pen p = new Pen(Color.Black, 1))
                                        gx.DrawPath(p, gp);
                                }
                            }
                        }
                    }
                }
            }

            if (lInner != null && dontFill)
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

        public Bitmap? RemOutline(Bitmap? bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            if (bmp != null && AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
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

                        cf.RemoveOutline(b, fList, true);
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

        public Bitmap? RemInnerOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
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

                        cf.RemoveOutline(b, fList, false, true);
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

        private void numRemInner_ValueChanged(object sender, EventArgs e)
        {
            if (this._oldUnknownFT != null)
                this._oldUnknownFT.Dispose();
            this._oldUnknownFT = null;
        }

        private unsafe void FillUnknownByBG(Bitmap bTrimap, Bitmap oldUnknownFT)
        {
            int w = bTrimap.Width;
            int h = bTrimap.Height;

            Bitmap bRead = oldUnknownFT;

            if (oldUnknownFT.Width != w || oldUnknownFT.Height != h)
                bRead = ResizeOUFT(oldUnknownFT, w, h);

            BitmapData bmD = bTrimap.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmRead = bRead.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                byte* pRead = (byte*)bmRead.Scan0;
                p += y * stride;
                pRead += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pRead[3] > 0)
                    {
                        p[0] = p[1] = p[2] = (byte)0;
                        p[3] = (byte)255;
                    }

                    p += 4;
                    pRead += 4;
                }
            });

            bRead.UnlockBits(bmRead);
            bTrimap.UnlockBits(bmD);

            Form fff = new Form();
            fff.BackgroundImage = bTrimap;
            fff.BackgroundImageLayout = ImageLayout.Zoom;
            fff.ShowDialog();
        }

        private Bitmap ResizeOUFT(Bitmap oldUnknownFT, int w, int h)
        {
            Bitmap bOut = new Bitmap(w, h);
            using (Graphics gx = Graphics.FromImage(bOut))
            {
                gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gx.PixelOffsetMode = PixelOffsetMode.Half;
                gx.DrawImage(oldUnknownFT, -0.5f, -0.5f, w, h); //draw with a small offset, since we resample down by "integer" 2 --> so odd widths/heights of the structures may result in smaller widths/heights of the structures
            }
            //ClearArtefacts(bOut);

            return bOut;
        }

        private void btnGetOutline_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.CachePathAddition != null)
            {
                Bitmap bmp = this.helplineRulerCtrl1.Bmp;
                if (this._bmpOutline != null)
                    bmp = this._bmpOutline;

                using (frmOutline frm = new frmOutline(bmp, this.CachePathAddition))
                {
                    frm.SetupCache();

                    if (frm.ShowDialog() == DialogResult.OK && frm.FBitmap != null)
                    {
                        Bitmap? b = (Bitmap)frm.FBitmap.Clone();

                        List<ChainCode>? l4 = frm.SelectedChains;

                        if (l4 != null)
                        {
                            if (this._scribbles == null)
                                this._scribbles = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

                            for (int i = 0; i < l4.Count; i++)
                            {
                                int fgbg = 3;
                                int wh = (int)frm.numWHScribbles.Value;

                                if (!this._scribbles.ContainsKey(fgbg))
                                    this._scribbles.Add(fgbg, new Dictionary<int, List<List<Point>>>());

                                if (!this._scribbles[fgbg].ContainsKey(wh))
                                    this._scribbles[fgbg].Add(wh, new List<List<Point>>());

                                if (this._scribbleSeq == null)
                                    this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                                List<List<Point>> whPts = this._scribbles[fgbg][wh];
                                List<Point> chunk = new List<Point>();
                                int strt = 0;
                                int l = 0;

                                if (frm.cbSplitSegments.Checked)
                                    while (strt < l4[i].Coord.Count)
                                    {
                                        chunk.Clear();

                                        for (int j = strt; j < Math.Min(strt + (int)frm.numNewSegment.Value, l4[i].Coord.Count); j++)
                                            chunk.Add(l4[i].Coord[j]);

                                        whPts.Add(new List<Point>());
                                        whPts[whPts.Count - 1].AddRange(chunk.ToArray());
                                        this._scribbleSeq.Add(Tuple.Create(fgbg, wh, l, false, new List<List<Point>>()));
                                        strt += (int)frm.numNewSegment.Value;
                                        l++;
                                    }
                                else
                                {
                                    whPts.Add(new List<Point>());
                                    whPts[whPts.Count - 1].AddRange(l4[i].Coord);
                                    this._scribbleSeq.Add(Tuple.Create(fgbg, wh, i, false, new List<List<Point>>()));
                                }
                            }

                            this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        }

                        if (b != null)
                            this.SetBitmap(ref this._bmpOutline, ref b);
                    }
                }
            }
        }

        private void numError_ValueChanged(object sender, EventArgs e)
        {
            if (this._cfop != null && this._cfop.BlendParameters != null)
                this._cfop.BlendParameters.DesiredMaxLinearError = (double)this.numError.Value;

            if (this._cfop?.CfopArray != null)
            {
                for (int i = 0; i < this._cfop?.CfopArray.Length; i++)
                    if (this._cfop?.CfopArray[i] != null)
                        try
                        {
                            BlendParameters? cb = this._cfop?.CfopArray[i].BlendParameters;
                            if (cb != null)
                                cb.DesiredMaxLinearError = (double)this.numError.Value;
                        }
                        catch
                        {

                        }
            }
        }

        private void btnTScribbles_Click(object sender, EventArgs e)
        {
            if (this._scribbles != null)
            {
                using (frmTranslateScribbles frm = new frmTranslateScribbles())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        int x = (int)frm.numTX.Value;
                        int y = (int)frm.numTY.Value;

                        if (x != 0 || y != 0)
                        {
                            Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles2 = TranslateAllScibbles(this._scribbles, x, y);
                            this._scribbles = scribbles2;
                            List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = TranslateScribbleSeq(this._scribbleSeq, x, y);
                            this._scribbleSeq = scribbleSeq2;

                            this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        }
                    }
                }
            }
        }

        private List<Tuple<int, int, int, bool, List<List<Point>>>> TranslateScribbleSeq(List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq, int x, int y)
        {
            //List<Tuple<int, int, int, bool, List<List<Point>>>> result = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

            for (int i = 0; i < scribbleSeq.Count; i++)
            {
                if (scribbleSeq[i] != null && scribbleSeq[i].Item5 != null && scribbleSeq[i].Item5.Count > 0)
                {
                    for (int j = 0; j < scribbleSeq[i].Item5.Count; j++)
                    {
                        if (scribbleSeq[i].Item5[j] != null && scribbleSeq[i].Item5[j].Count > 0)
                        {
                            for (int l = 0; l < scribbleSeq[i].Item5[j].Count; l++)
                            {
                                scribbleSeq[i].Item5[j][l] = new Point(scribbleSeq[i].Item5[j][l].X + x, scribbleSeq[i].Item5[j][l].Y + y);
                            }
                        }
                    }
                }
            }

            return scribbleSeq;
        }

        private Dictionary<int, Dictionary<int, List<List<Point>>>> TranslateAllScibbles(Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles, int x, int y)
        {
            Dictionary<int, Dictionary<int, List<List<Point>>>> result = new Dictionary<int, Dictionary<int, List<List<Point>>>>();

            foreach (int w in scribbles.Keys)
            {
                result.Add(w, new Dictionary<int, List<List<Point>>>());
                Dictionary<int, List<List<Point>>> ptsWH = scribbles[w];

                foreach (int wh in ptsWH.Keys)
                {
                    result[w].Add(wh, new List<List<Point>>());
                    List<List<Point>> j = ptsWH[wh];

                    foreach (List<Point> pts in j)
                    {
                        List<Point> newPts = new List<Point>();
                        Point[] transPts = pts.Select(a => new Point(a.X + x, a.Y + y)).ToArray();
                        newPts.AddRange(transPts);

                        result[w][wh].Add(newPts);
                    }
                }
            }

            return result;
        }

        private void cbForceSerial_CheckedChanged(object sender, EventArgs e)
        {
            if (cbForceSerial.Checked)
                this.numSleep.Value = (decimal)0;
            else
                this.numSleep.Value = (decimal)10;
        }

        private void btnCheckArray_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this.rbClosedForm.Checked)
                {
                    int w = this.helplineRulerCtrl1.Bmp.Width;
                    int h = this.helplineRulerCtrl1.Bmp.Height;

                    if (this.cbHalfSize.Checked)
                    {
                        w /= 2;
                        h /= 2;
                    }

                    int wh = w * h;
                    int n = 1;

                    int maxSize = this.cbInterpolated.Checked ? (int)Math.Pow((double)this.numMaxSize.Value, 2) * 2 : (int)Math.Pow((double)this.numMaxSize.Value, 2);
                    int maxW = this.cbInterpolated.Checked ? (int)this.numMaxSize.Value * 2 : (int)this.numMaxSize.Value;

                    if (maxW < this._minSize)
                    {
                        MessageBox.Show("MaxSize is too small for creating tiles with an overlap. Minimum is " + this._minSize.ToString());
                        return;
                    }

                    while (wh > maxSize)
                    {
                        n += 1;
                        wh = w / n * h / n;
                    }

                    int n2 = n * n;

                    MessageBox.Show("Outer-Pic-Array will be of size: " + n2.ToString());
                }
                else
                    MessageBox.Show("Array will be of size: 1; whole Pic will be resized.");
            }
        }

        private void numMaxSize_ValueChanged(object sender, EventArgs e)
        {

        }

        private void cbRestrictMatte_CheckedChanged(object sender, EventArgs e)
        {
            this.label7.Enabled = this.numOTh.Enabled = this.label17.Enabled = this.numMatteTh.Enabled = this.cbRestrictMatte.Checked;
        }

        private void numOTh_ValueChanged(object sender, EventArgs e)
        {
            this.numOTh.Enabled = false;
            this.numOTh.Refresh();

            if (this._bmpMatteOrigSize != null && this._resMatte != null)
            {
                int th = (int)this.numOTh.Value;
                using (Bitmap bMatteC = (Bitmap)this._bmpMatteOrigSize.Clone())
                {
                    SetOpaqueGTh(bMatteC, th);
                    SetFullLum2Shape(bMatteC, th);
                    using (Bitmap resMatte = (Bitmap)this._resMatte.Clone())
                    {
                        using (Graphics graphics = Graphics.FromImage(resMatte))
                            graphics.DrawImage(bMatteC, 0, 0, resMatte.Width, resMatte.Height);


                        Bitmap? bmp = GetAlphaBoundsPic(this.helplineRulerCtrl1.Bmp, resMatte);

                        if (bmp != null)
                        {
                            this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                            this.helplineRulerCtrl2.Zoom = this.helplineRulerCtrl1.Zoom;
                            this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl2.Zoom.ToString());
                            this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                            this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                                (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                                (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                            _undoOPCache?.Add(bmp);

                            this.btnUndo.Enabled = true;
                            this.CheckRedoButton();

                            this.helplineRulerCtrl2.dbPanel1.Invalidate();
                        }
                    }
                }
            }

            this.numOTh.Enabled = true;
        }

        private void btnSmothenSettings_Click(object sender, EventArgs e)
        {
            using (frmLoadScribblesSettings frm = new frmLoadScribblesSettings())
            {
                frm.rbDynamic.Checked = this._dynamic;
                frm.numDivisor.Value = (decimal)this._divisor;
                frm.numNewWidth.Value = (decimal)this._newWidth;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    this._dynamic = frm.rbDynamic.Checked;
                    this._divisor = (int)frm.numDivisor.Value;
                    this._newWidth = (int)frm.numNewWidth.Value;
                }
            }
        }

        private void cbDrawRectsAlso_CheckedChanged(object sender, EventArgs e)
        {
            this.labelGo.Enabled = this.numDrawPenWidth.Enabled = this._lbGo = false;

            if (this.cbDrawRectsAlso.Checked)
                if (MessageBox.Show("Set w-Factor to default value for this set of operations?", "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                    this.numScribblesWFactor.Value = (decimal)Math.Sqrt(2.0);
        }

        private void cbExcludeRegions_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbExcludeRegions.Checked)
                this.cbExcludeFG.Checked = false;
        }

        private void cbExcludeFG_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbExcludeFG.Checked)
                this.cbExcludeRegions.Checked = false;
        }

        private void btnCancelBGW_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            if (this.btnCancelBGW.Text == "Cancel")
            {
                FloodFillMethods.Cancel = true;
                return;
            }

            if (this._ptBU == null)
                this._ptBU = new List<Point>();
            this._ptBU.Clear();
            this._ptBU.AddRange(this._pointsDraw);
            this._pointsDraw.Clear();
            this.helplineRulerCtrl2.dbPanel1.Invalidate();
        }

        private void cbWOScribbles_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbWOScribbles.Checked)
                this.labelGo.Enabled = this.numDrawPenWidth.Enabled = true;
            else
                this.labelGo.Enabled = this.numDrawPenWidth.Enabled = this._lbGo;

            if (this.helplineRulerCtrl2.Bmp == null)
            {
                Bitmap? bWork = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();

                if (cbHalfSize.Checked && rbClosedForm.Checked)
                {
                    Bitmap bWork2 = ResampleBmp(bWork, 2);

                    Bitmap? bOld = bWork;
                    bWork = bWork2;
                    bOld.Dispose();
                    bOld = null;
                }

                Bitmap bTrimap = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                this.pictureBox1.Image = bTrimap;
                using Graphics gx = Graphics.FromImage(bTrimap);
                gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                gx.PixelOffsetMode = PixelOffsetMode.Half;

                int wh = bTrimap.Height / 3;
                gx.FillRectangle(Brushes.Black, new Rectangle(0, 0, bTrimap.Width, wh));
                gx.FillRectangle(Brushes.Gray, new Rectangle(0, wh, bTrimap.Width, wh));
                gx.FillRectangle(Brushes.White, new Rectangle(0, wh * 2, bTrimap.Width, wh));

                this.SetBitmap(ref _bWork, ref bWork);

                Bitmap bC = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, bC, this.helplineRulerCtrl2, "Bmp");
                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                if (this.helplineRulerCtrl2.Bmp != null)
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                this.helplineRulerCtrl2.dbPanel1.Invalidate();
            }
        }

        private void btnRedoSameCoords_Click(object sender, EventArgs e)
        {
            if (this._ptBU != null && this.helplineRulerCtrl2.Bmp != null)
            {
                this._pointsDraw.Clear();
                this._pointsDraw.AddRange(this._ptBU);

                ProcessPicture();
            }
        }

        private void btnLoadBmp_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;
                Image img = Image.FromFile(this.openFileDialog1.FileName);
                bmp = (Bitmap)img.Clone();

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                    double faktor = System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height);
                    double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                    if (multiplier >= faktor)
                        this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                    else
                        this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                        (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    Bitmap? bC = (Bitmap)bmp.Clone();
                    this.SetBitmap(ref this._bmpBU, ref bC);

                    this._undoOPCache?.Add(bmp);
                }
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
        }

        private void btnAlphaCurve_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker6.IsBusy)
            {
                this.backgroundWorker6.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl2.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                int alphaTh = (int)this.numAlphaZAndGain.Value;

                Bitmap b = (Bitmap)this.helplineRulerCtrl2.Bmp.Clone();
                bool redrawExcluded = false;

                string? c = this.CachePathAddition;
                if (c != null && this.cbExcludeRegions.Checked)
                {
                    bool show = true;
                    if (this._excludedRegions != null && this._excludedRegions.Count > 0)
                    {
                        if (MessageBox.Show("Use existing Exclusions?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            redrawExcluded = true;
                            show = false;
                        }
                    }

                    if (show)
                    {
                        using frmExcludeFromPic frm = new frmExcludeFromPic(b, c);
                        frm.SetupCache();

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (frm.ExcludedBmpRegions != null)
                            {
                                using Graphics gx = Graphics.FromImage(b);
                                for (int i = 0; i < frm.ExcludedBmpRegions.Count; i++)
                                {
                                    Bitmap? rem = frm.ExcludedBmpRegions[i].Remaining;
                                    if (rem != null)
                                        SetTransp(b, rem, frm.ExcludedBmpRegions[i].Location);
                                }

                                CopyRegions(frm.ExcludedBmpRegions);
                                redrawExcluded = true;
                            }
                        }
                    }
                }

                if (c != null && this.cbExcludeFG.Checked)
                {
                    bool show = true;
                    if (this._excludedRegions != null && this._excludedRegions.Count > 0)
                    {
                        if (MessageBox.Show("Use existing Exclusions?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                        {
                            redrawExcluded = true;
                            show = false;
                        }
                    }

                    if (show)
                    {
                        using frmDefineFGPic frm = new frmDefineFGPic(b, c);
                        frm.SetupCache();

                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (frm.Excluded != null)
                            {
                                using Graphics gx = Graphics.FromImage(b);

                                Bitmap? rem = frm.Excluded.Remaining;
                                if (rem != null)
                                {
                                    if (frm.cbSetOpaque.Checked)
                                    {
                                        SetOpaque(b, rem);
                                        SetOpaque(rem, rem);
                                    }
                                    SetTransp(b, rem, frm.Excluded.Location);
                                }

                                List<ExcludedBmpRegion> l = new();
                                l.Add(frm.Excluded);
                                CopyRegions(l);
                                redrawExcluded = true;
                            }
                        }
                    }
                }

                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                using frmColorCurves frmA = new frmColorCurves(b, "255;127;127;127", 255);

                if (frmA.ShowDialog() == DialogResult.OK)
                {
                    int[] aalpha = new int[256];

                    Array.Copy(frmA.MappingsAlpha, aalpha, frmA.MappingsAlpha.Length);
                    this.backgroundWorker6.RunWorkerAsync(new object[] { b, aalpha, frmA.RadioButton1.Checked, frmA.CheckBox1.Checked, frmA.CheckBox4.Checked, redrawExcluded });
                }
            }
        }

        private void backgroundWorker6_DoWork(object? sender, DoWorkEventArgs e)
        {
            Bitmap? b = null;
            Bitmap? bRet = null;
            bool redrawExcluded = false;

            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                b = (Bitmap)o[0];

                this.backgroundWorker6.ReportProgress(50);

                int[] aalpha = (int[])o[1];

                if ((bool)o[2])
                    ColorCurvesAlpha.fipbmp.gradColors(b, aalpha, (bool)o[3]);
                else
                    ColorCurvesAlpha.fipbmp.gradLumToAlpha(b, aalpha, (bool)o[4], (bool)o[3]);

                this.backgroundWorker6.ReportProgress(100);

                bRet = b;
                redrawExcluded = (bool)o[5];
            }

            if (bRet != null)
                e.Result = new object[] { bRet, redrawExcluded };
        }

        private void backgroundWorker6_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (this.toolStripProgressBar1 != null && !this.toolStripProgressBar1.IsDisposed && this.toolStripProgressBar1.Visible)
                this.toolStripProgressBar1.Value = e.ProgressPercentage;
            else if (this.backgroundWorker6 != null && this.backgroundWorker6.IsBusy)
                this.backgroundWorker6.CancelAsync();
        }

        private void backgroundWorker6_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                {
                    object[] o = (object[])e.Result;
                    bmp = (Bitmap)o[0];

                    if ((bool)o[1])
                    {
                        if (this._excludedRegions != null && this._exclLocations != null)
                        {
                            using Graphics gx = Graphics.FromImage(bmp);
                            gx.SmoothingMode = SmoothingMode.None;
                            gx.InterpolationMode = InterpolationMode.NearestNeighbor;

                            for (int i = 0; i < this._excludedRegions.Count; i++)
                            {
                                SetTransp(bmp, this._excludedRegions[i], this._exclLocations[i]);
                                gx.DrawImage(this._excludedRegions[i], this._exclLocations[i]);
                            }
                        }
                    }

                    //since ...
                    SetColorsToOrig(bmp, this.helplineRulerCtrl2.Bmp);
                }

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    _undoOPCache?.Add(bmp);
                }

                this.btnAlphaCurve.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.Timer3.Start();

                this.backgroundWorker6.Dispose();
                this.backgroundWorker6 = new BackgroundWorker();
                this.backgroundWorker6.WorkerReportsProgress = true;
                this.backgroundWorker6.WorkerSupportsCancellation = true;
                this.backgroundWorker6.DoWork += backgroundWorker6_DoWork;
                this.backgroundWorker6.ProgressChanged += backgroundWorker6_ProgressChanged;
                this.backgroundWorker6.RunWorkerCompleted += backgroundWorker6_RunWorkerCompleted;
            }
        }
    }
}
