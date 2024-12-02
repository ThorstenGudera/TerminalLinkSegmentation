using Cache;
using ChainCodeFinder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections;
using AvoidAGrabCutEasy.ProcOutline;
using GetAlphaMatte;
using System.Runtime.InteropServices;
using OutlineOperations;
using ConvolutionLib;
using SegmentsListLib;

namespace AvoidAGrabCutEasy
{
    public partial class frmAvoidAGrabCutEasy : Form
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

        private GrabCutOp? _gc;
        private bool _tracking2;
        private List<Point> _points = new List<Point>();
        private Dictionary<int, GraphicsPath>? _pathList;
        private List<Point> _points2 = new List<Point>();
        private int _currentDraw;
        private Color[] _colors = new Color[4];
        //private Dictionary<int, List<Tuple<List<Point>, int>>> _allPoints2;
        private Dictionary<int, List<Tuple<List<Point>, int>>>? _allPoints;
        private double _oldGamma;
        private double _oldXi;
        //private double _oldBeta;
        private object _lockObject = new object();
        private bool _drawImgOverlay;
        private int _eX2f;
        private int _eY2f;
        private List<int>? _lastDraw;
        private int _eX2;
        private int _eY2;

        private Dictionary<int, Dictionary<int, List<List<Point>>>>? _scribblesBU;
        private Dictionary<int, Dictionary<int, List<List<Point>>>>? _scribbles;
        private int _maxSize = 1200;
        private bool _finishedPart1;

        private double _items = 10;
        private double _correctItems = 5;
        private double _items2 = 10;
        private double _correctItems2 = 2;
        private List<ChainCode>? _allChains;
        private Bitmap? _bResCopy;
        private Bitmap? _bResCopyTransp;
        private bool _restartDiffInit;
        private Bitmap? _b4Copy;
        private double _KMeansInitW = 2;
        private double _KMeansInitH = 2;
        private ListSelectionMode _KMeansSelMode = 0;
        private int _KMeansInitIters = 10;
        private bool _kMInitRnd = false;
        private int _KMeansIters = 0;
        private int _numShiftX;
        private int _numShiftY;
        private Point _ptHLC1BG = new Point(-1, -1);
        private Bitmap? _scribblesBitmap;
        private Point _ptHLC1FG = new Point(-1, -1);
        private BitArray? _bitsBG;
        private BitArray? _bitsFG;
        private Point _ptHLC1FGBG;
        //It would be much easier to use a List of objects which hold all information about drawing instead of using a seperate sequence file...
        //Maybe I'll change it (as far as it is also easy to serialize in json)
        private List<Tuple<int, int, int, bool, List<List<Point>>>>? _scribbleSeqBU;
        private List<Tuple<int, int, int, bool, List<List<Point>>>> _scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();
        private List<ChainCode>? _chainsFromFrm;
        private List<Tuple<int, int, int, bool, List<List<Point>>>>? _pointsListSeq;
        private int _currentDrawOperation;
        private bool _dontUpdateNumComp;
        private bool _cancelledOp;
        private Bitmap? _bmpWork;
        private Bitmap? _bmpTrimap;
        private ClosedFormMatteOp? _cfop;
        private ClosedFormMatteOp[]? _cfopArray;
        private Bitmap? _bmpMatte;
        private Point? _ptSt;
        private List<Point>? _ptPrev;
        private bool _cbSetPFGToFG;
        private bool _cbSkipLearnEnabled;
        private bool _cbSkipLearnChecked;
        private float _opacityDraw;
        private Bitmap? _bmpOrig;
        private float _opacity;
        private frmPreBlurVals? _frm;
        private bool _frmValsChanged;
        private float[,]? _iggLuminanceMap;
        private float[,]? _iggLuminanceMap2;
        private LumMapApplicationSettings? _lmas = new LumMapApplicationSettings() { Factor1 = 1.4f, Threshold = 0.2, Factor2 = 1.4f, Exponent2 = 0.5 };
        private bool _useLumMapBasePic;
        private List<(int, int)>? _bgRelatedPointsAdded;
        private List<ChainCode>? _removedChains;

        public event EventHandler<string>? ShowInfo;
        //public event EventHandler<string> BoundaryError;

        public frmAvoidAGrabCutEasy()
        {
            InitializeComponent();
        }

        public frmAvoidAGrabCutEasy(Bitmap bmp, string basePathAddition)
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

            this._currentDraw = 0;

            this.helplineRulerCtrl2.AddDefaultHelplines();
            this.helplineRulerCtrl2.ResetAllHelpLineLabelsColor();

            //while developing...
            //AvailMem.AvailMem.NoMemCheck = true;
        }

        private void helplineRulerCtrl2_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

                if (this.cbDraw.Checked && ix >= 0 && ix < this.helplineRulerCtrl2.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl2.Bmp.Height)
                {
                    this._points.Add(new Point(ix, iy));
                    this._tracking2 = true;
                    this.helplineRulerCtrl2.dbPanel1.Capture = true;
                }
            }
        }

        private void helplineRulerCtrl2_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

                if (ix >= 0 && iy >= 0)
                {
                    if (this._tracking2)
                        this._points.Add(new Point(ix, iy));

                    if (ix < this.helplineRulerCtrl2.Bmp.Width && iy < this.helplineRulerCtrl2.Bmp.Height)
                    {
                        Color c = this.helplineRulerCtrl2.Bmp.GetPixel(ix, iy);
                        this.toolStripStatusLabel4.Text = ix.ToString() + "; " + iy.ToString();
                        this.toolStripStatusLabel3.BackColor = c;
                    }

                    this.helplineRulerCtrl2.dbPanel1.Invalidate();
                }
            }

            if (this._drawImgOverlay)
            {
                this._eX2f = e.X;
                this._eY2f = e.Y;
            }
        }

        private void helplineRulerCtrl2_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl2.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl2.Zoom);

                //if (ix >= 0 && ix < this.helplineRulerCtrl2.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl2.Bmp.Height)
                if (ix >= 0 && iy >= 0)
                {
                    if (this._tracking2)
                    {
                        if (this._points[this._points.Count - 1].X != ix || this._points[this._points.Count - 1].Y != iy)
                            this._points.Add(new Point(ix, iy));

                        AddPointsToPath();
                        this._points.Clear();
                        this.helplineRulerCtrl2.dbPanel1.Invalidate();
                    }
                }
            }

            this._tracking2 = false;
            this.helplineRulerCtrl2.dbPanel1.Capture = false;
        }

        private void AddPointsToPath()
        {
            if (this._pathList == null)
                this._pathList = new Dictionary<int, GraphicsPath>();

            if (!this._pathList.ContainsKey(this._currentDraw))
                this._pathList.Add(this._currentDraw, new GraphicsPath());

            if (this._allPoints == null)
                this._allPoints = new Dictionary<int, List<Tuple<List<Point>, int>>>();

            if (!this._allPoints.ContainsKey(this._currentDraw))
                this._allPoints.Add(this._currentDraw, new List<Tuple<List<Point>, int>>());

            if (this._pointsListSeq == null)
                this._pointsListSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

            if (this._bgRelatedPointsAdded == null)
                this._bgRelatedPointsAdded = new();

            GraphicsPath gp = this._pathList[this._currentDraw];
            using (GraphicsPath gpTmp = new GraphicsPath())
            {
                PointF[] z = this._points.Select(a => new PointF(a.X, a.Y)).ToArray();

                if (z.Length > 1)
                {
                    gpTmp.AddLines(z);
                    gp.AddPath((GraphicsPath)gpTmp.Clone(), false);

                    List<Point> ll = new List<Point>();
                    ll.AddRange(this._points.ToArray());
                    this._allPoints[this._currentDraw].Add(Tuple.Create(ll, (int)this.numWH.Value));

                    if (this._currentDraw == 0 || this._currentDraw == 2)
                        this._bgRelatedPointsAdded.Add((this._currentDraw, this._allPoints[this._currentDraw].Count - 1));
                }
                else if (z.Length == 1)
                {
                    Point pt = this._points[0];
                    Rectangle r = new Rectangle(pt.X - (int)this.numWH.Value / 2,
                        pt.Y - (int)this.numWH.Value / 2,
                        (int)this.numWH.Value,
                        (int)this.numWH.Value);

                    gpTmp.AddRectangle(r);
                    gp.AddPath((GraphicsPath)gpTmp.Clone(), false);

                    List<Point> ll = new List<Point>();
                    ll.AddRange(this._points.ToArray());
                    this._allPoints[this._currentDraw].Add(Tuple.Create(ll, (int)this.numWH.Value));

                    if (this._currentDraw == 0 || this._currentDraw == 2)
                        this._bgRelatedPointsAdded.Add((this._currentDraw, this._allPoints[this._currentDraw].Count - 1));
                }
            }

            this._pointsListSeq.Add(Tuple.Create(this._currentDraw, this._currentDrawOperation, this._allPoints[this._currentDraw].Count - 1, false, new List<List<Point>>()));

            this._currentDrawOperation++;

            if (this._lastDraw == null)
                this._lastDraw = new List<int>();

            this._lastDraw.Add(this._currentDraw);
        }

        private void helplineRulerCtrl2_Paint(object sender, PaintEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                if (this._allPoints != null)
                {
                    if (this._lastDraw != null && this._lastDraw.Count > 0 && this._pointsListSeq != null && this._pointsListSeq.Count > 0)
                    {
                        int curOp = 0;

                        foreach (Tuple<int, int, int, bool, List<List<Point>>> f in this._pointsListSeq)
                        {
                            IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> jj = this._pointsListSeq.Where(a => a.Item2 == curOp);

                            if (jj != null && jj.Count() > 0)
                            {
                                List<Tuple<List<Point>, int>> lpts = this._allPoints[jj.First().Item1].ToList();

                                Tuple<List<Point>, int> ptsList = this._allPoints[jj.First().Item1][jj.First().Item3]; //lpts = this._allPoints[jj.First().Item1]

                                if (ptsList != null && ptsList.Item1.Count() > 0)
                                {
                                    List<Point> pts = ptsList.Item1;
                                    int wh = ptsList.Item2;

                                    bool doRect = pts.Count > 1;

                                    Color c = this._colors[jj.First().Item1];

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
                            curOp++;
                        }
                    }
                    else
                    {
                        foreach (int j in this._allPoints.Keys)
                        {
                            Color c = this._colors[j];
                            List<Tuple<List<Point>, int>> ll = this._allPoints[j];

                            for (int i = 0; i < ll.Count; i++)
                            {
                                bool doRect = ll[i].Item1.Count == 1;

                                if (!doRect)
                                {
                                    foreach (Point pt in ll[i].Item1)
                                    {
                                        using (SolidBrush sb = new SolidBrush(c))
                                            e.Graphics.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - ll[i].Item2 / 2) * this.helplineRulerCtrl2.Zoom) + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - ll[i].Item2 / 2) * this.helplineRulerCtrl2.Zoom) + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y,
                                                (int)(ll[i].Item2 * this.helplineRulerCtrl2.Zoom),
                                                (int)(ll[i].Item2 * this.helplineRulerCtrl2.Zoom)));
                                    }
                                }
                                else
                                {
                                    Point pt = ll[i].Item1[0];
                                    using (SolidBrush sb = new SolidBrush(c))
                                        e.Graphics.FillRectangle(sb, new Rectangle(
                                            (int)((int)(pt.X - ll[i].Item2 / 2) * this.helplineRulerCtrl2.Zoom) + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X,
                                            (int)((int)(pt.Y - ll[i].Item2 / 2) * this.helplineRulerCtrl2.Zoom) + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y,
                                            (int)(ll[i].Item2 * this.helplineRulerCtrl2.Zoom),
                                            (int)(ll[i].Item2 * this.helplineRulerCtrl2.Zoom)));
                                }
                            }
                        }
                    }
                }

                if (this._points.Count > 0)
                {
                    Color c = this._colors[this._currentDraw];

                    using (GraphicsPath gp = new GraphicsPath())
                    {
                        if (this._points.Count > 1)
                        {
                            foreach (Point pt in this._points)
                            {
                                using (SolidBrush sb = new SolidBrush(c))
                                    e.Graphics.FillRectangle(sb, new Rectangle(
                                        (int)((int)(pt.X - (int)this.numWH.Value / 2) * this.helplineRulerCtrl2.Zoom) + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X,
                                        (int)((int)(pt.Y - (int)this.numWH.Value / 2) * this.helplineRulerCtrl2.Zoom) + this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y,
                                        (int)((int)this.numWH.Value * this.helplineRulerCtrl2.Zoom),
                                        (int)((int)this.numWH.Value * this.helplineRulerCtrl2.Zoom)));
                            }
                        }
                    }
                }

                if (this._drawImgOverlay)
                {
                    HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl2.dbPanel1;

                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = this._opacityDraw;

                    using (ImageAttributes ia = new ImageAttributes())
                    {
                        ia.SetColorMatrix(cm);
                        e.Graphics.DrawImage(this.helplineRulerCtrl1.Bmp,
                            new Rectangle(0, 0, pz.ClientRectangle.Width, pz.ClientRectangle.Height),
                            -pz.AutoScrollPosition.X / this.helplineRulerCtrl2.Zoom,
                                -pz.AutoScrollPosition.Y / this.helplineRulerCtrl2.Zoom,
                                pz.ClientRectangle.Width / this.helplineRulerCtrl2.Zoom,
                                pz.ClientRectangle.Height / this.helplineRulerCtrl2.Zoom, GraphicsUnit.Pixel, ia);
                    }

                    int wh2 = (int)this.numWH.Value;

                    using (SolidBrush sb = new SolidBrush(Color.FromArgb(95, 0, 255, 0)))
                        e.Graphics.FillRectangle(sb, new RectangleF(
                            this._eX2f - (float)(wh2 / 2f * this.helplineRulerCtrl2.Zoom) - 1f,
                            this._eY2f - (float)(wh2 / 2f * this.helplineRulerCtrl2.Zoom) - 1f,
                            (float)(wh2 * this.helplineRulerCtrl2.Zoom),
                            (float)(wh2 * this.helplineRulerCtrl2.Zoom)));
                }
            }
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
                if (!this.cbAllowRS.Checked)
                {
                    this._rX = ix;
                    this._rY = iy;

                    this._eX = eX;
                    this._eY = eY;
                }

                if (this.cbScribbleMode.Checked && !this.cbRefPtBG.Checked && !this.cbRefPtFG.Checked && e.Button == MouseButtons.Left && !this.cbClickMode.Checked)
                {
                    this._points2.Add(new Point(ix, iy));
                    this._tracking4 = true;
                }

                if (this.cbScribbleMode.Checked && !this.cbRefPtBG.Checked && !this.cbRefPtFG.Checked && e.Button == MouseButtons.Left && this.cbClickMode.Checked)
                    this._points2.Add(new Point(ix, iy));

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
                if (this._tracking && !this.cbAllowRS.Checked)
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
                if (!this.cbAllowRS.Checked)
                {
                    this._rW = Math.Abs(ix - this._rX);
                    this._rH = Math.Abs(iy - this._rY);

                    this._rect = new Rectangle(this._rX, this._rY, this._rW, this._rH);
                }

                if (this._tracking4 || (this.cbClickMode.Checked && e.Button == MouseButtons.Left))
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
            if (this.cbRectMode.Checked)
            {
                using (Pen pen = new Pen(new SolidBrush(Color.Lime), 2))
                    e.Graphics.DrawRectangle(pen, new Rectangle(this._eX + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                        this._eY + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y, this._eW, this._eH));
            }

            if (this.cbScribbleMode.Checked)
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

                            if (ptsList != null && ptsList.Count > 0 && ptsList.Count > listNo)
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
                                                    (int)(wh * this.helplineRulerCtrl1.Zoom),
                                                    (int)(wh * this.helplineRulerCtrl1.Zoom)));
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
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "testApp");
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
            this.btnCache.Enabled = true;
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
                                    this.btnReset2.Enabled = false;

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
            //_undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The goal is to achieve results as good as with a GrabCut, but only with the first (GMM_probabilities) estimation, to reduce the memory footprint. So keep \"QuickEstimation\" checked and play around with the other parameters.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_undoOPCache != null && _undoOPCache?.Processing == false)
            {
                this.btnReset2.Enabled = false;
                this.btnReset2.Refresh();

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

            this.cbRectMode.Enabled = true;
            this.cbRectMode.Checked = false;
            this.cbScribbleMode.Enabled = true;
            this.cbScribbleMode.Checked = false;
            this.btnReset2.Enabled = true;
            this.cbDraw.Checked = false;
            this.numMaxSize.Enabled = this.numGmmComp.Enabled = true;

            this.cbQuickEst_CheckedChanged(this.cbQuickEst, new EventArgs());
            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());

            if (this.numComponents2.Value > this.numMaxComponents.Value)
                this.numMaxComponents.Value = this.numComponents2.Value;

            this.btnGo.Enabled = true;

            this.cbAutoCropFromOrig.Enabled = this.btnCropFromOrig.Enabled = false;
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
                            this.btnReset2.Enabled = false;

                            //if (_undoOPCache?.Count > 1)
                            //    this.btnRedo.Enabled = true;
                            //else
                            //    this.btnRedo.Enabled = false;

                            this.cbAutoCropFromOrig.Enabled = this.btnCropFromOrig.Enabled = false;

                            this.btnReset2.Enabled = true;
                            this.cbScribbleMode.Checked = false;
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
                this.btnReset2.PerformClick();
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

                        if (ct.Name == "panel5")
                        {
                            foreach (Control ct2 in this.panel5.Controls)
                                if (ct2.Name != "btnDoAll")
                                    ct2.Enabled = e;

                            ct.Enabled = true;
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

                    if (ct.Name == "panel5")
                    {
                        foreach (Control ct2 in this.panel5.Controls)
                            if (ct2.Name != "btnDoAll")
                                ct2.Enabled = e;

                        ct.Enabled = true;
                    }
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
                {
                    this.backgroundWorker1.CancelAsync();
                    return;
                }

                if (this.backgroundWorker2.IsBusy)
                {
                    this.backgroundWorker2.CancelAsync();
                    return;
                }

                if (this._frm != null)
                    this._frm.Close();

                if (this._gc != null)
                {
                    this._gc.ShowInfo -= _gc_ShowInfo;
                    this._gc.ShowTHInfo -= _gc_ShowTHInfo;
                    this._gc.Dispose();
                }

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                if (this._b4Copy != null)
                    this._b4Copy.Dispose();
                if (this._bResCopy != null)
                    this._bResCopy.Dispose();
                if (this._bResCopyTransp != null)
                    this._bResCopyTransp.Dispose();
                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();
                if (this._scribblesBitmap != null)
                    this._scribblesBitmap.Dispose();
                if (this._bmpTrimap != null)
                    this._bmpTrimap.Dispose();
                if (this._bmpWork != null)
                    this._bmpWork.Dispose();
                if (this._bmpMatte != null)
                    this._bmpMatte.Dispose();
                if (this._bmpOrig != null)
                    this._bmpOrig.Dispose();

                if (this._pathList != null)
                {
                    foreach (int j in this._pathList.Keys)
                        this._pathList[j].Dispose();
                }
            }
        }

        private void _gc_ShowInfo(object? sender, string e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => this.toolStripStatusLabel4.Text = e));
            else
                this.toolStripStatusLabel4.Text = e;
        }

        private void frmGrabCut_Load(object sender, EventArgs e)
        {
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cmbCurrentColor.SelectedIndex = 0;

            this.cbQuickEst_CheckedChanged(this.cbQuickEst, new EventArgs());
            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.numMaxSize.Value = (decimal)Math.Max(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                btnCmpLMap_Click(this.btnCmpLMap, new EventArgs());
            }

            this.btnOK.Enabled = true;
        }

        private double CheckWidthHeight(Bitmap bmp, bool fp)
        {
            double r = 1.0;
            if (fp)
            {
                r = (double)Math.Max(bmp.Width, bmp.Height) / (double)_maxSize;
                return r;
            }

            int res = 1;
            if (bmp.Width * bmp.Height > _maxSize * _maxSize * 256L)
            {
                res = 32;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize * 128L)
            {
                res = 24;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize * 64L)
            {
                res = 16;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize * 32L)
            {
                res = 12;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize * 16L)
            {
                res = 8;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize * 8L)
            {
                res = 6;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize * 4L)
            {
                res = 4;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize * 2L)
            {
                res = 3;
            }
            else if (bmp.Width * bmp.Height > _maxSize * _maxSize)
            {
                res = 2;
            }

            return res;
        }

        private void cbQuickEst_CheckedChanged(object sender, EventArgs e)
        {
            this.btnMinCut.Enabled = !this.cbQuickEst.Checked;
        }

        private void btnRect_Click(object sender, EventArgs e)
        {
            Rectangle rc = new Rectangle(this._rX, this._rY, this._rW, this._rH);
            using (frmRect frm = new frmRect(rc))
            {
                frm.TopMost = true;

                frm.numX.ValueChanged += NumX_ValueChanged;
                frm.numY.ValueChanged += NumY_ValueChanged;
                frm.numW.ValueChanged += NumW_ValueChanged;
                frm.numH.ValueChanged += NumH_ValueChanged;

                frm.ShowDialog();
            }
        }

        private void NumH_ValueChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                NumericUpDown num = (NumericUpDown)sender;
                this._rH = (int)num.Value;
                this._eH = (int)((double)num.Value * (double)this.helplineRulerCtrl1.Zoom);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumW_ValueChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                NumericUpDown num = (NumericUpDown)sender;
                this._rW = (int)num.Value;
                this._eW = (int)((double)num.Value * (double)this.helplineRulerCtrl1.Zoom);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumY_ValueChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                NumericUpDown num = (NumericUpDown)sender;
                this._rY = (int)num.Value;
                this._eY = (int)((double)num.Value * (double)this.helplineRulerCtrl1.Zoom);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void NumX_ValueChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                NumericUpDown num = (NumericUpDown)sender;
                this._rX = (int)num.Value;
                this._eX = (int)((double)num.Value * (double)this.helplineRulerCtrl1.Zoom);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            //reset state
            if (this.backgroundWorker1.IsBusy || this.backgroundWorker2.IsBusy)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this.backgroundWorker2.IsBusy)
                    this.backgroundWorker2.CancelAsync();

                return;
            }

            //reset variable
            this._maxSize = (int)this.numMaxSize.Value;

            if (!this.backgroundWorker1.IsBusy && this.helplineRulerCtrl1.Bmp != null)
            {
                //get the resizeFactor
                double res = CheckWidthHeight(this.helplineRulerCtrl1.Bmp, true);
                //this.lblResPic.Text = res.ToString();

                //this test is not corresponding to the real amount of RAM being used by the algorithms. It's more or less just an indicator, if to start at all.
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 100L))
                {
                    //reset
                    if (this.cbRectMode.Enabled && this._gc != null)
                    {
                        this._gc.ShowInfo -= _gc_ShowInfo;
                        this._gc.ShowTHInfo -= _gc_ShowTHInfo;
                        this._gc.Dispose();
                        this._gc = null;
                    }

                    //ensure gammaChanged = false on reset
                    if (this.cbRectMode.Checked || this.cbScribbleMode.Checked)
                    {
                        this._oldGamma = (double)this.numGamma.Value;
                        this._oldXi = 1.0;
                    }

                    //now get the needed parameters from the controls
                    //get some of them before we disabel the UI-Controls
                    int intMult = 1;
                    double gamma = (double)this.numGamma.Value;
                    double xi = 1.0;
                    bool setPFGToFG = this._cbSetPFGToFG;

                    bool skipLearn = this._cbSkipLearnEnabled && this._cbSkipLearnChecked;

                    this.SetControls(false);
                    this.btnGo.Text = "Cancel";
                    this.btnGo.Enabled = true;
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    this.toolStripStatusLabel1.Text = "resFactor: " + Math.Max(res, 1).ToString("N2");

                    int gmm_comp = (int)this.numGmmComp.Value;

                    int num_Iters = 1;
                    bool rectMode = this.cbRectMode.Checked;
                    bool scribbleMode = this.cbScribbleMode.Checked;
                    if (this._rW == 0)
                        this._rW = this.helplineRulerCtrl1.Bmp.Width;
                    if (this._rH == 0)
                        this._rH = this.helplineRulerCtrl1.Bmp.Height;
                    Rectangle r = new Rectangle(this._rX, this._rY, this._rW, this._rH);
                    bool autoBias = false;
                    bool skipInit = !(rectMode || scribbleMode);
                    bool workOnPaths = !(rectMode || scribbleMode) && this.cbDraw.Checked;
                    int wh = (int)this.numWH.Value;
                    Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);
                    bool gammaChanged = gamma - this._oldGamma != 0;
                    gammaChanged |= xi - this._oldXi != 0;
                    this._oldGamma = gamma;
                    this._oldXi = xi;
                    bool useEightAdj = this.cbEightAdj.Checked;
                    bool useTh = this.cbUseTh.Checked;
                    double th = useTh ? (double)this.numDblMult.Value : 0;
                    bool initWKpp = true;

                    bool multCapacitiesForTLinks = this.cbMultTLCap.Checked;
                    double multTLinkCapacity = (double)this.numMultTLCap.Value;
                    bool castTLInt = cbCastTLInt.Checked;
                    bool getSourcePart = false;

                    ListSelectionMode selMode = this._KMeansSelMode;

                    double probMult1 = (double)this.numProbMult1.Value;

                    double numItems = this._items;
                    double numCorrect = this._correctItems;
                    double numItems2 = this._items2;
                    double numCorrect2 = this._correctItems2;

                    double kmInitW = this._KMeansInitW;
                    double kmInitH = this._KMeansInitH;

                    int KMeansInitIters = this._KMeansInitIters;
                    bool kMInitRnd = this._kMInitRnd;
                    int KMeansIters = this._KMeansIters;

                    int comp = (int)this.numMaxComponents.Value;
                    bool autoThreshold = this.cbAutoThreshold.Checked;

                    //testmode2
                    bool assumeExpDist = this.cbAssumeExpDist.Checked;

                    //now start the work
                    this.backgroundWorker1.RunWorkerAsync(new object[] { bWork, gmm_comp, gamma, num_Iters,
                        rectMode, r, autoBias, skipInit, workOnPaths,
                        wh, gammaChanged, intMult, useEightAdj, useTh, th, xi, res, initWKpp,
                        multCapacitiesForTLinks, multTLinkCapacity, castTLInt, getSourcePart, selMode,
                        scribbleMode, probMult1, kmInitW, kmInitH, setPFGToFG,
                        numItems, numCorrect, numItems2, numCorrect2, skipLearn, comp, autoThreshold,
                        KMeansInitIters, kMInitRnd, KMeansIters, assumeExpDist});
                }
            }
        }

        private unsafe void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap? bRes = null;
                int gmm_comp = (int)o[1];
                double gamma = (double)o[2];
                int numIters = (int)o[3];
                bool rectMode = (bool)o[4];
                Rectangle r = (Rectangle)o[5];
                bool autoBias = (bool)o[6];
                bool skipInit = (bool)o[7];
                bool workOnPaths = (bool)o[8];
                int wh = (int)o[9];
                bool gammaChanged = (bool)o[10];
                int intMult = (int)o[11];
                bool quick = true;
                bool useEightAdj = (bool)o[12];
                bool useTh = (bool)o[13];
                double th = (double)o[14];
                double xi = (double)o[15];
                double resPic = (double)o[16];
                bool initWKpp = (bool)o[17];
                bool multCapacitiesForTLinks = (bool)o[18];
                double multTLinkCapacity = (double)o[19];
                bool castTLInt = (bool)o[20];
                bool getSourcePart = (bool)o[21];
                ListSelectionMode selMode = (ListSelectionMode)o[22];
                bool scribbleMode = (bool)o[23];
                Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles = this._scribbles;
                List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq = this._scribbleSeq;
                double probMult1 = (double)o[24];
                double kmInitW = (double)o[25];
                double kmInitH = (double)o[26];

                bool setPFGToFG = (bool)o[27];
                double numItems = (double)o[28];
                double numCorrect = (double)o[29];
                double numItems2 = (double)o[30];
                double numCorrect2 = (double)o[31];
                bool skipLearn = (bool)o[32];

                if (scribbleMode && !rectMode)
                    r = new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                Rectangle clipRect = new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                bool dontFillPath = true;
                bool drawNumComp = true;
                int comp = (int)o[33];
                bool autoThreshold = (bool)o[34];
                int KMeansInitIters = (int)o[35];
                bool kMInitRnd = (bool)o[36];
                int KMeansIters = (int)o[37];

                bool assumeExpDist = (bool)o[38];

                //if we have a large pic that will be resized during these OPs
                //we need to resize also the scribbles, if present
                if (scribbleMode && resPic > 1)
                {
                    if (this._scribbles != null)
                    {
                        Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles2 = ResizeAllScribbles(this._scribbles, resPic);
                        this._scribblesBU = this._scribbles;
                        this._scribbles = scribbles2;
                        scribbles = this._scribbles;
                        List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = ResizeScribbleSeq(this._scribbleSeq, resPic, true);
                        this._scribbleSeqBU = this._scribbleSeq;
                        this._scribbleSeq = scribbleSeq2;
                        scribbleSeq = this._scribbleSeq;
                    }
                }

                //resize the input bmp
                Bitmap bWork = (Bitmap)o[0];
                Bitmap? bU2 = null;
                if (resPic > 1)
                {
                    Bitmap? bOld = bWork;
                    bU2 = new Bitmap(bWork);
                    bWork = ResampleDown(bWork, ref r, ref clipRect, resPic, scribbleMode, rectMode);
                    if (bOld != null)
                    {
                        bOld.Dispose();
                        bOld = null;
                    }
                }

                //do a check to ensure a correct initialisation of the GC_OP
                Bitmap bTmp = new Bitmap(bWork);
                if (r.Width == 0 && r.Height == 0)
                {
                    this.Invoke(new Action(() => { MessageBox.Show("No Image passed to function. Cancelled operation."); }));
                    if (bU2 != null)
                        bU2.Dispose();
                    e.Result = new Bitmap(bTmp);
                    return;
                }

                //create the operator for the GrabcutALike methods
                //if we have already a gc prrsent, we set its params later
                if (this._gc == null)
                {
                    this._gc = new GrabCutOp()
                    {
                        Bmp = bWork,
                        Gmm_comp = gmm_comp,
                        Gamma = gamma,
                        NumIters = numIters,
                        RectMode = rectMode,
                        ScribbleMode = scribbleMode,
                        Scribbles = scribbles,
                        Rc = r,
                        BGW = this.backgroundWorker1,
                        QuickEstimation = quick,
                        EightAdj = useEightAdj,
                        UseThreshold = useTh,
                        Threshold = th,
                        MultCapacitiesForTLinks = multCapacitiesForTLinks,
                        MultTLinkCapacity = multTLinkCapacity,
                        CastIntCapacitiesForTLinks = castTLInt,
                        SelectionMode = selMode,
                        ProbMult1 = probMult1,
                        KMInitW = kmInitW,
                        KMInitH = kmInitH,
                        NumItems = numItems,
                        NumCorrect = numCorrect,
                        NumItems2 = numItems2,
                        NumCorrect2 = numCorrect2,
                        AutoThreshold = autoThreshold,
                        KMeansInitIters = KMeansInitIters,
                        kMInitRnd = kMInitRnd,
                        KMeansIters = KMeansIters,
                        AssumeExpDist = assumeExpDist,
                        ScribbleSeq = scribbleSeq,
                        UseLumMap = this.cbCompLumMap.Checked //<- will be put into a param
                    };

                    this._gc.ShowInfo += _gc_ShowInfo;
                    this._gc.ShowTHInfo += _gc_ShowTHInfo;
                }

                //now do the initialisation
                //eg create the mask, preclassify the imagedata, compute the smootheness function and init the Gmms
                if (!skipInit)
                {
                    int it = this._gc.Init();

                    if (this._gc.BGW != null && this._gc.BGW.WorkerSupportsCancellation && this._gc.BGW.CancellationPending)
                        it = -4;

                    if (it != 0)
                    {
                        if (bU2 != null)
                            bU2.Dispose();

                        switch (it)
                        {
                            case -1:
                                this.Invoke(new Action(() => { MessageBox.Show("No BGPixels found. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -2:
                                this.Invoke(new Action(() => { MessageBox.Show("No FGPixels found. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -3:
                                this.Invoke(new Action(() => { MessageBox.Show("No Image passed to function. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -4:
                                this.Invoke(new Action(() => { MessageBox.Show("Operation cancelled."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -5:
                                this.Invoke(new Action(() => { MessageBox.Show("Mask is null. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                        }
                    }
                }
                else
                {
                    this._gc.Gamma = gamma;
                    this._gc.GammaChanged = gammaChanged;
                    this._gc.NumIters = numIters;
                    this._gc.Rc = r;
                    this._gc.QuickEstimation = quick;
                    this._gc.EightAdj = useEightAdj;
                    this._gc.UseThreshold = useTh;
                    this._gc.Threshold = th;
                    this._gc.MultCapacitiesForTLinks = multCapacitiesForTLinks;
                    this._gc.MultTLinkCapacity = multTLinkCapacity;
                    this._gc.CastIntCapacitiesForTLinks = castTLInt;
                    this._gc.SelectionMode = selMode;
                    this._gc.ProbMult1 = probMult1;
                    this._gc.KMInitW = kmInitW;
                    this._gc.KMInitH = kmInitH;
                    this._gc.NumItems = numItems;
                    this._gc.NumCorrect = numCorrect;
                    this._gc.NumItems2 = numItems2;
                    this._gc.NumCorrect2 = numCorrect2;
                    this._gc.AutoThreshold = autoThreshold;
                    this._gc.KMeansInitIters = KMeansInitIters;
                    this._gc.kMInitRnd = kMInitRnd;
                    this._gc.KMeansIters = KMeansIters;
                    this._gc.AssumeExpDist = assumeExpDist;
                    this._gc.ScribbleSeq = scribbleSeq;
                    this._gc.UseLumMap = this.cbCompLumMap.Checked;

                    if (!workOnPaths && this._gc.ScribbleMode && this._gc.Scribbles != null && this._gc.Scribbles.Count > 0)
                    {
                        if (!this._gc.RectMode)
                            r = new Rectangle(0, 0, bWork.Width, bWork.Height);
                        this._gc.ReInitScribbles();
                    }
                }

                //if we use scribbles on the previously computed result
                if (workOnPaths)
                {
                    if (this._allPoints != null && this._allPoints.Count > 0 && this._pointsListSeq != null)
                    {
                        this._gc.GammaChanged = gammaChanged;

                        Dictionary<int, List<Tuple<List<Point>, int>>> allPts2 = this._allPoints;

                        if (resPic > 1)
                            allPts2 = ResizeAllPoints(this._allPoints, resPic);

                        Dictionary<int, List<Tuple<List<Point>, int>>> allPts4 = allPts2;

                        this._gc.SetAllPointsInMask(allPts4, this._pointsListSeq, setPFGToFG);

                        this._gc.SkipLearn = skipLearn;
                    }
                }

                //test
                //since the MinCut in the GrabCut method uses neighborhood information,
                //let's try to use this information in our method too.
                //For this, we compute a gradient pic and from this, we get the inverted
                //luminance map. This may change, also the settings for the invGaussGrad
                //method for conputing the gradient pic may change.
                if (this._gc.UseLumMap)
                {
                    if (this._iggLuminanceMap != null || (this._iggLuminanceMap2 != null && this._useLumMapBasePic))
                    {
                        this._gc.IGGLuminanceMap = (this._useLumMapBasePic && this._iggLuminanceMap2 != null) ? this._iggLuminanceMap2 : this._iggLuminanceMap;
                        this._gc.LumMapSettings = this._lmas;
                    }
                    else if (this._iggLuminanceMap == null)
                    {
                        if (this._bmpBU != null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                this.toolStripStatusLabel4.Text = "Computing LumMap";
                                this.lblLumMap.Text = "computing...";
                            }));
                            LuminancMapOp lop = new LuminancMapOp();
                            lop.ProgressPlus += lop_ProgressPlus;
                            using Bitmap bmp = new Bitmap(this._bmpBU);
                            float[,]? lMap = lop.ComputeInvLuminanceMapSync(bmp);
                            this._iggLuminanceMap = lMap;
                            this._gc.IGGLuminanceMap = (this._useLumMapBasePic && this._iggLuminanceMap2 != null) ? this._iggLuminanceMap2 : lMap;
                            this._gc.LumMapSettings = this._lmas;
                            lop.ProgressPlus -= lop_ProgressPlus;
                            this.Invoke(new Action(() =>
                            {
                                this.toolStripStatusLabel4.Text = "LumMap done.";
                                this.lblLumMap.Text = "done.";
                            }));
                        }
                    }
                }

                //now do the work ...
                int l = this._gc.Run();

                if (l != 0)
                {
                    if (bU2 != null)
                        bU2.Dispose();

                    switch (l)
                    {
                        case -1:
                            this.Invoke(new Action(() => { MessageBox.Show("Arrays-Length, or Graph-Length failed test. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case -25:
                            this.Invoke(new Action(() => { MessageBox.Show("Graph-Construction failed. Maybe the threshold is too big. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 100:
                            this.Invoke(new Action(() => { MessageBox.Show("Bmp_width or Bmp_height = 0. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 101:
                            this.Invoke(new Action(() => { MessageBox.Show("Operation cancelled."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 102:
                            this.Invoke(new Action(() => { MessageBox.Show("At least one GMM is null. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 103:
                            this.Invoke(new Action(() => { MessageBox.Show("This Mode only makes sense with RectMode."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 104:
                            this.Invoke(new Action(() => { MessageBox.Show("This Mode only makes sense with ScribbleMode."); }));
                            e.Result = new Bitmap(bTmp);
                            return;
                    }
                }

                //... and get the result ...
                List<int>? res = this._gc.Result;

                //... and the result image
                bRes = new Bitmap(bWork.Width, bWork.Height);

                int[,]? m = this._gc.Mask;
                int w = bTmp.Width;
                int h = bTmp.Height;

                if ((scribbleMode && !rectMode) || workOnPaths)
                    r = new Rectangle(0, 0, bWork.Width, bWork.Height);

                //lock the bmps for fast processing
                BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, bRes.Width, bRes.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmWork = bTmp.LockBits(new Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;

                //get the references to the pointer addresses
                byte* p = (byte*)bmData.Scan0;
                byte* pWork = (byte*)bmWork.Scan0;

                //for (int i = 0; i < res.Count(); i++)
                //{
                //    int j = res[i];
                //    int x = j % w;
                //    int y = j / w;

                //    p[x * 4 + y * stride] = pWork[x * 4 + y * stride];
                //    p[x * 4 + y * stride + 1] = pWork[x * 4 + y * stride + 1];
                //    p[x * 4 + y * stride + 2] = pWork[x * 4 + y * stride + 2];
                //    p[x * 4 + y * stride + 3] = pWork[x * 4 + y * stride + 3];
                //}

                if (m != null)
                {
                    //write the data
                    int ww = m.GetLength(0);
                    int hh = m.GetLength(1);

                    for (int y = 0; y < h; y++)
                    {
                        if (y + this._numShiftY > 0 && y + this._numShiftY < h)
                            for (int x = 0; x < w; x++)
                            {
                                if (x + this._numShiftX > 0 && x + this._numShiftX < w)
                                    if (x < ww && y < hh && r.Contains(x, y) && (m[x, y] == 1 || m[x, y] == 3))
                                    {
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride] = pWork[x * 4 + y * stride];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 1] = pWork[x * 4 + y * stride + 1];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 2] = pWork[x * 4 + y * stride + 2];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 3] = pWork[x * 4 + y * stride + 3];
                                    }
                            }
                    }
                }

                //and unlock the bmps
                bTmp.UnlockBits(bmWork);
                bRes.UnlockBits(bmData);

                //now do some analysis of the result, to be able to redraw the resultpic with a different set of components
                Bitmap bResCopy = new Bitmap(bRes);
                Bitmap bCTransp = new Bitmap(bRes);

                //BU of the original result
                this.SetBitmap(ref this._bResCopy, ref bResCopy);

                //use a ChainCode [that works on the - invisible - "cracks" between the pixels, because it is very fast aand reliable]
                List<ChainCode>? c = GetBoundary(bRes, 0, false);
                c = c?.OrderByDescending(x => x.Coord.Count).ToList();

                if (c != null)
                {
                    int comp2 = c.Count;

                    if (c.Count > 0)
                    {
                        //if we have a very lot of components, allow the user to restrict the amount
                        if (c.Count > 1000 && (comp > 1000 || !drawNumComp))
                        {
                            using (frmDrawNumComp frm = new frmDrawNumComp(c.Count))
                            {
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    if (frm.checkBox1.Checked)
                                    {
                                        drawNumComp = true;
                                        comp = comp2 = (int)frm.numericUpDown1.Value;
                                    }
                                }
                            }
                        }

                        //now begin to redraw each component
                        using (Graphics gx = Graphics.FromImage(bRes))
                        {
                            gx.Clear(Color.Transparent);

                            int amnt = (!drawNumComp) ? c.Count : Math.Min(comp, c.Count);

                            for (int i = 0; i < amnt; i++)
                            {
                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    PointF[] pts = c[i].Coord.Select(pt => new PointF(pt.X, pt.Y)).ToArray();
                                    //make sure, each path is treated as an "outer-outline"
                                    gp.FillMode = FillMode.Winding;
                                    gp.AddLines(pts);
                                    gp.CloseAllFigures();

                                    //tmp try...catch
                                    try
                                    {
                                        using (TextureBrush tb = new TextureBrush(bTmp))
                                            gx.FillPath(tb, gp);

                                        //using (Pen pen = new Pen(Color.Red, 2))
                                        //    gx.DrawPath(pen, gp);
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                            }
                        }
                    }

                    //now redraw the inner outlines and "transparent" components
                    //we can do this in the easy way with setting graphics.CompositionMode to sourceCopy
                    //because the whole operations are full_pixel_wise (at least for the standard 4-connectivity)
                    if (dontFillPath && c.Count > 0)
                    {
                        int amnt = (!drawNumComp) ? c.Count : Math.Min(comp, c.Count);

                        for (int i = 0; i < amnt; i++)
                        {
                            ChainCode cc = c[i];
                            if (ChainFinder.IsInnerOutline(cc))
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Graphics gx = Graphics.FromImage(bRes))
                                        {
                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                            gx.FillPath(Brushes.Transparent, gP);
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                        }
                    }

                    //get a backup - for later use - of this bmp with the transparent paths
                    this.SetBitmap(ref this._bResCopyTransp, ref bCTransp);

                    bTmp.Dispose();

                    if (resPic > 1)
                    {
                        Bitmap bOld = bRes;

                        bRes = ResampleUp(bRes, resPic, bU2, dontFillPath, false);
                        Bitmap? bRCopy = ResampleUp(this._bResCopy, resPic, bU2, dontFillPath, false);
                        Bitmap? bRCopy2 = ResampleUp(this._bResCopyTransp, resPic, bU2, true, true);

                        if (bRCopy != null && bRCopy2 != null)
                        {
                            this.SetBitmap(ref this._bResCopy, ref bRCopy);
                            this.SetBitmap(ref this._bResCopyTransp, ref bRCopy2);
                        }
                        //bU2.Dispose();

                        if (bOld != null)
                            bOld.Dispose();
                    }
                    else //if (resPic == 1)
                    {
                        //set the list of all found paths [chains] to re_use later
                        List<ChainCode>? allChains = GetBoundary(this._bResCopyTransp, 0, false);
                        this._allChains = allChains;
                        if (this._removedChains != null)
                            this._removedChains.Clear();

                        if (allChains != null && allChains.Count > 0)
                        {
                            int area = allChains.Sum(a => a.Area);
                            int pxls = w * h;

                            int fc = pxls / area;

                            //if we have almost no output, maybe the initialization of the Gmms hasn't been good enough to receive a reasonable result
                            //so restart with some different KMeans initialization, if wanted
                            if (fc > 1000)
                                if (MessageBox.Show("Amount pixels to segmented area ratio is " + fc.ToString() + ". " +
                                    "Rerun with different Initialization of the Gmms?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                {
                                    this._restartDiffInit = true;
                                }
                        }
                    }

                    //reset the scribbles, if resized
                    if (scribbleMode && resPic > 1)
                    {
                        if (this._scribblesBU != null)
                            this._scribbles = this._scribblesBU;
                        if (this._scribbleSeqBU != null)
                            this._scribbleSeq = this._scribbleSeqBU;
                    }

                    //our result pic
                    e.Result = bRes;
                }
            }
        }

        private void lop_ProgressPlus(object? sender, ConvolutionLib.ProgressEventArgs e)
        {
            if (this.backgroundWorker1 != null)
                this.backgroundWorker1.ReportProgress(Math.Min((int)e.CurrentProgress, this.toolStripProgressBar1.Maximum));
        }

        private void _gc_ShowTHInfo(object? sender, string e)
        {
            if (!InvokeRequired)
            {
                this.lblTh.Text = e;
            }
            else
                this.Invoke(new Action(() =>
                {
                    this.lblTh.Text = e;
                }));
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
            //if we have a result pic, display it
            if (e.Result != null)
            {
                Bitmap b = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                this.SetBitmap(ref this._b4Copy, ref bC);

                //Bitmap bC2 = new Bitmap(this.helplineRulerCtrl1.Bmp);
                //this.SetBitmap(ref this._bmpBU, ref bC2);

                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                    (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                _undoOPCache?.Add(b);

                if (this.cbDraw.Checked && this._bgRelatedPointsAdded != null && this._bgRelatedPointsAdded.Count > 0)
                {
                    MessageBox.Show("You removed some BG pixels. Maybe increase the amount of displayed components, when those pixels are surrounded by FG pixels.");
                }

                //if (this._resWC > 1)
                //    ResampleAndTranslateBack();

                //if (this._bResCopyTransp != null)
                //{
                //    List<ChainCode> c2 = GetBoundary(this._bResCopyTransp, 0, false);
                //    c2 = c2.OrderByDescending(x => x.Coord.Count).ToList();

                //    //this.numRedrawComponents.Value = this.numRedrawComponents.Maximum = c2.Count;
                //}
            }

            //re enable the UI
            this.SetControls(true);

            btnReset2.Enabled = true;
            this._pic_changed = true;

            //reset the UI-drawn lists
            if (this._allPoints != null)
                this._allPoints.Clear();
            if (this._pathList != null)
                this._pathList.Clear();
            if (this._points != null)
                this._points.Clear();
            if (this._pointsListSeq != null)
                this._pointsListSeq.Clear();
            if (this._bgRelatedPointsAdded != null)
                this._bgRelatedPointsAdded.Clear();
            this._currentDrawOperation = 0;

            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            this.Timer3.Start();
            this.btnGo.Text = "Go";

            //re init the bgw
            this.backgroundWorker1.Dispose();
            this.backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

            if (this._gc != null)
                this._gc.BGW = this.backgroundWorker1;

            //do some UI settings
            this.cbQuickEst_CheckedChanged(this.cbQuickEst, new EventArgs());
            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());

            //if we didnt have a good result and set the auto restart with different KMeans settings
            if (this._restartDiffInit)
            {
                this._restartDiffInit = false;
                this._KMeansInitW += 1;
                this._KMeansInitH += 1;

                bool bR = this.cbRectMode.Checked;
                bool bS = this.cbScribbleMode.Checked;

                this.button4_Click(this.btnReset2, new EventArgs());

                if (bR)
                    this.cbRectMode.Checked = true;
                if (bS)
                    this.cbScribbleMode.Checked = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.btnGo_Click(this.btnGo, new EventArgs());
            }

            //do some UI settings
            this.cbRectMode.Checked = false;
            this.cbRectMode.Enabled = false;
            this.cbScribbleMode.Checked = false;
            this.cbScribbleMode.Enabled = false;
            this.label17.Enabled = this.numComponents2.Enabled = true;

            this.numMaxSize.Enabled = this.numGmmComp.Enabled = false;

            this.toolStripStatusLabel4.Text = "done";

            this.cbAutoCropFromOrig.Enabled = this.btnCropFromOrig.Enabled = true;

            if (this.timer1.Enabled)
                this.timer1.Stop();
            this.timer1.Start();
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


        private Bitmap ResampleDown(Bitmap bWork, double resPic)
        {
            Bitmap bOut = new Bitmap((int)Math.Ceiling(bWork.Width / resPic), (int)Math.Ceiling(bWork.Height / resPic));

            using (Graphics gx = Graphics.FromImage(bOut))
                gx.DrawImage(bWork, 0, 0, bOut.Width, bOut.Height);

            return bOut;
        }

        private Bitmap ResampleDown(Bitmap bWork, ref Rectangle r, ref Rectangle r2, double resPic, bool scribbleMode, bool rectMode)
        {
            Bitmap bOut = new Bitmap((int)Math.Ceiling(bWork.Width / resPic), (int)Math.Ceiling(bWork.Height / resPic));
            if (!scribbleMode || rectMode)
            {
                r.X = (int)(r.X / resPic);
                r.Y = (int)(r.Y / resPic);
                r.Width = (int)(r.Width / resPic);
                r.Height = (int)(r.Height / resPic);
            }
            r2.X = (int)(r2.X / resPic);
            r2.Y = (int)(r2.Y / resPic);
            r2.Width = (int)(r2.Width / resPic);
            r2.Height = (int)(r2.Height / resPic);
            using (Graphics gx = Graphics.FromImage(bOut))
                gx.DrawImage(bWork, 0, 0, bOut.Width, bOut.Height);

            return bOut;
        }

        private Bitmap? ResampleUp(Bitmap? bRes, double resPic, Bitmap? bOrig, bool dontFillPath, bool disposebOrig)
        {
            //take orig image,
            //get chains from result pic
            //"cut" (crop) orig image with chains as Mask

            if (bOrig != null)
            {
                Bitmap bOut = new Bitmap(bOrig.Width, bOrig.Height);

                using (Bitmap bTmp = new Bitmap(bOrig.Width, bOrig.Height))
                {
                    using (Graphics gx = Graphics.FromImage(bTmp))
                        if (bRes != null)
                            gx.DrawImage(bRes, 0, 0, bOut.Width, bOut.Height);

                    List<ChainCode>? allChains = GetBoundary(bTmp, 0, false);
                    this._allChains = allChains;
                    if (this._removedChains != null)
                        this._removedChains.Clear();

                    using (TextureBrush tb = new TextureBrush(bOrig))
                    {
                        if (allChains != null)
                            foreach (ChainCode c in allChains)
                            {
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Graphics gx = Graphics.FromImage(bOut))
                                            gx.FillPath(tb, gP);
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                            }
                    }

                    if (dontFillPath && allChains != null)
                        foreach (ChainCode c in allChains)
                        {
                            if (ChainFinder.IsInnerOutline(c))
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Graphics gx = Graphics.FromImage(bOut))
                                        {
                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                            gx.FillPath(Brushes.Transparent, gP);
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                        }
                }

                if (disposebOrig)
                    bOrig.Dispose();

                return bOut;
            }
            return null;
        }

        private List<ChainCode>? GetBoundary(Bitmap? bmp)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, 0, false, 0, false, 0, false);
            return l;
        }

        private Dictionary<int, List<Tuple<List<Point>, int>>> ShiftAllPoints(Dictionary<int, List<Tuple<List<Point>, int>>> allPoints, Rectangle clipRect)
        {
            Dictionary<int, List<Tuple<List<Point>, int>>> res = new Dictionary<int, List<Tuple<List<Point>, int>>>();
            foreach (int j in allPoints.Keys)
            {
                res.Add(j, new List<Tuple<List<Point>, int>>());

                List<Tuple<List<Point>, int>> points = allPoints[j];

                for (int i = 0; i < points.Count; i++)
                {
                    List<Point> pt = points[i].Item1;
                    int wh = points[i].Item2;
                    Tuple<List<Point>, int> ptRes = Tuple.Create(new List<Point>(), wh);

                    foreach (Point p in pt)
                        ptRes.Item1.Add(new Point(p.X - clipRect.X, p.Y - clipRect.Y));

                    res[j].Add(ptRes);
                }

            }
            return res;
        }

        private Dictionary<int, List<Tuple<List<Point>, int>>> ResizeAllPoints(Dictionary<int, List<Tuple<List<Point>, int>>> allPoints, double resPic)
        {
            Dictionary<int, List<Tuple<List<Point>, int>>> res = new Dictionary<int, List<Tuple<List<Point>, int>>>();
            foreach (int j in allPoints.Keys)
            {
                res.Add(j, new List<Tuple<List<Point>, int>>());

                List<Tuple<List<Point>, int>> points = allPoints[j];

                for (int i = 0; i < points.Count; i++)
                {
                    List<Point> pt = points[i].Item1;
                    int wh = (int)Math.Max(points[i].Item2 / resPic, 1);
                    Tuple<List<Point>, int> ptRes = Tuple.Create(new List<Point>(), wh);

                    foreach (Point p in pt)
                        ptRes.Item1.Add(new Point((int)(p.X / resPic), (int)(p.Y / resPic)));

                    res[j].Add(ptRes);
                }

            }
            return res;
        }


        private List<ChainCode>? GetBoundary(Bitmap? upperImg, int minAlpha, bool grayScale)
        {
            List<ChainCode>? l = null;
            Bitmap? bmpTmp = null;
            try
            {
                if (upperImg != null)
                    if (AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L))
                        bmpTmp = new Bitmap(upperImg);
                    else
                        throw new Exception("Not enough memory.");
                if (bmpTmp != null)
                {
                    int nWidth = bmpTmp.Width;
                    int nHeight = bmpTmp.Height;
                    ChainFinder cf = new ChainFinder();
                    lock (this._lockObject)
                        l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, grayScale, 0, false, 0, false);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                if (bmpTmp != null)
                {
                    bmpTmp.Dispose();
                    bmpTmp = null;
                }
            }
            return l;
        }

        private void cbRectMode_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbRectMode.Checked && !this.cbAllowRS.Checked)
                this.cbScribbleMode.Checked = false;

            SetRectOrScribblesValues(this.helplineRulerCtrl1.Zoom);
        }

        private void SetRectOrScribblesValues(float zoom)
        {
            if (this.cbRectMode.Checked)
            {
                Rectangle r = new Rectangle(this._rX, this._rY, this._rW, this._rH);
                this._eX = (int)(r.X * zoom);
                this._eY = (int)(r.Y * zoom);
                this._eW = (int)(r.Width * zoom);
                this._eH = (int)(r.Height * zoom);
            }

            //this._crX = 0;
            //this._crY = 0;
            //this._crW = this.helplineRulerCtrl1.Bmp.Width;
            //this._crH = this.helplineRulerCtrl1.Bmp.Height;

            this.btnRect.Enabled = this.cbRectMode.Checked;
        }

        private void cbScribbleMode_CheckedChanged(object sender, EventArgs e)
        {
            SetRectOrScribblesValues(this.helplineRulerCtrl1.Zoom);

            if (this.cbScribbleMode.Checked && !this.cbAllowRS.Checked)
                this.cbRectMode.Checked = false;

            this.btnLoadScribbles.Enabled = this.btnSaveScribbles.Enabled = this.cbScribbleMode.Checked;
        }

        private void cbMultTLCap_CheckedChanged(object sender, EventArgs e)
        {
            this.cbCastTLInt.Enabled = this.cbMultTLCap.Checked;
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

        private void cbDraw_CheckedChanged(object sender, EventArgs e)
        {
            this._drawImgOverlay = cbDraw.Checked;
            this._cbSkipLearnEnabled = cbDraw.Checked;
            this._cbSkipLearnChecked = cbDraw.Checked;
            this.helplineRulerCtrl2.dbPanel1.Invalidate();
        }

        private void cmbCurrentColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._currentDraw = this.cmbCurrentColor.SelectedIndex;
        }

        private void btnRemStroke_Click(object sender, EventArgs e)
        {
            if (this._pathList == null)
                this._pathList = new Dictionary<int, GraphicsPath>();

            if (this._allPoints == null)
                this._allPoints = new Dictionary<int, List<Tuple<List<Point>, int>>>();

            if (this._allPoints.Count > 0 && this._lastDraw != null && this._lastDraw.Count > 0 && this._allPoints.ContainsKey(this._lastDraw[this._lastDraw.Count - 1]) && this._allPoints[this._lastDraw[this._lastDraw.Count - 1]].Count > 0)
                this._allPoints[this._lastDraw[this._lastDraw.Count - 1]].RemoveAt(this._allPoints[this._lastDraw[this._lastDraw.Count - 1]].Count - 1);

            if (this._lastDraw != null && this._lastDraw.Count > 0 && this._pathList.ContainsKey(this._lastDraw[this._lastDraw.Count - 1]))
            {
                GraphicsPath gp = this._pathList[this._lastDraw[this._lastDraw.Count - 1]];
                gp.Reset();

                if (this._allPoints.ContainsKey(this._lastDraw[this._lastDraw.Count - 1]))
                {
                    foreach (Tuple<List<Point>, int> l in this._allPoints[this._lastDraw[this._lastDraw.Count - 1]])
                    {
                        PointF[] z = l.Item1.Select(a => new PointF(a.X, a.Y)).ToArray();
                        gp.AddLines(z);
                    }
                }

                int j = this._lastDraw[this._lastDraw.Count - 1];

                this._pathList[this._lastDraw[this._lastDraw.Count - 1]] = gp;

                if (this._lastDraw.Count > 0)
                    this._lastDraw.RemoveAt(this._lastDraw.Count - 1);

                this._currentDrawOperation--;

                if (this._pointsListSeq != null && this._pointsListSeq.Count > 0)
                {
                    IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> jj = this._pointsListSeq.Where(a => a.Item1 == j);
                    if (jj != null && jj.Count() > 0)
                    {
                        IEnumerable<Tuple<int, int, int, bool, List<List<Point>>>> jjj = jj.Where(a => a.Item2 == this._currentDrawOperation);
                        if (jjj != null && jjj.Count() > 0)
                            this._pointsListSeq.Remove(jjj.First());
                    }
                }

                if (this._bgRelatedPointsAdded != null && this._bgRelatedPointsAdded.Count > 0 &&
                    this._allPoints.ContainsKey(this._bgRelatedPointsAdded[this._bgRelatedPointsAdded.Count - 1].Item1))
                {
                    IEnumerable<(int, int)>? cv = this._bgRelatedPointsAdded.Where(a => a.Item1 == j);
                    if (cv != null && this._bgRelatedPointsAdded[this._bgRelatedPointsAdded.Count - 1].Item1 == j &&
                        cv.Count() > this._allPoints[this._bgRelatedPointsAdded[this._bgRelatedPointsAdded.Count - 1].Item1].Count)
                    {
                        //if (this._bgRelatedPointsAdded.Count > 0)
                            this._bgRelatedPointsAdded.RemoveAt(this._bgRelatedPointsAdded.Count - 1);
                    }
                }
            }

            this.helplineRulerCtrl2.dbPanel1.Invalidate();
        }

        private void btnRecut_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._allChains != null)
            {
                Bitmap bOut = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                Bitmap bC = new Bitmap(bOut);
                Bitmap bCT = new Bitmap(bOut);

                this.SetControls(false);
                this.Refresh();

                this._dontUpdateNumComp = true;

                List<ChainCode>? allChains = this._allChains;
                allChains = allChains?.OrderByDescending(x => x.Coord.Count).ToList();

                if (allChains != null && allChains.Count > 1000)
                    allChains = allChains.Take(1000).ToList();

                if (allChains != null)
                    this.numComponents2.Value = this.numComponents2.Maximum = allChains.Count;

                float wFactor = 1.0f; // ((float)bOut.Width + (float)this.numRCX.Value) / (float)bOut.Width;
                float hFactor = 1.0f; // ((float)bOut.Height + (float)this.numRCY.Value) / (float)bOut.Height;

                if (allChains != null)
                    using (TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp))
                    {
                        foreach (ChainCode c in allChains)
                        {
                            using (GraphicsPath gP = new GraphicsPath())
                            {
                                try
                                {
                                    gP.StartFigure();
                                    PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                    gP.AddLines(pts);

                                    using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                        gP.Transform(mx);

                                    using (Graphics gx = Graphics.FromImage(bOut), gx2 = Graphics.FromImage(bC), gx4 = Graphics.FromImage(bCT))
                                    {
                                        gx.FillPath(tb, gP);
                                        gx2.FillPath(tb, gP);
                                        gx4.FillPath(tb, gP);
                                    }
                                }
                                catch (Exception exc)
                                {
                                    Console.WriteLine(exc.ToString());
                                }
                            }
                        }

                        foreach (ChainCode c in allChains)
                        {
                            if (ChainFinder.IsInnerOutline(c))
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                            gP.Transform(mx);

                                        using (Graphics gx = Graphics.FromImage(bOut))
                                        {
                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                            gx.FillPath(Brushes.Transparent, gP);
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                        }

                        foreach (ChainCode c in allChains)
                        {
                            if (ChainFinder.IsInnerOutline(c))
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = c.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Matrix mx = new Matrix(wFactor, 0, 0, hFactor, 0, 0))
                                            gP.Transform(mx);

                                        using (Graphics gx = Graphics.FromImage(bCT))
                                        {
                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                            gx.FillPath(Brushes.Transparent, gP);
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                        }
                    }

                OnShowInfo("done");

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");
                this.SetBitmap(ref this._bResCopy, ref bC);
                this.SetBitmap(ref this._bResCopyTransp, ref bCT);

                _undoOPCache?.Add(bOut);

                this._pic_changed = true;

                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                    (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                this._dontUpdateNumComp = false;

                this.SetControls(true);

                this.cbRectMode.Enabled = false;
            }
        }

        private void OnShowInfo(string message)
        {
            ShowInfo?.Invoke(this, message);
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

                SetRectOrScribblesValues(this.helplineRulerCtrl1.Zoom);

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

        private void btnMinCut_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy || this.backgroundWorker2.IsBusy)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this.backgroundWorker2.IsBusy)
                    this.backgroundWorker2.CancelAsync();

                return;
            }

            this._maxSize = 300; //dont increase too much, running time grows exponentially

            if (MessageBox.Show("Reset Controls?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                this.cbUseTh.Checked = this.cbMultTLCap.Checked = false;
                this.numProbMult1.Value = (decimal)1.0;
            }

            if (!this.backgroundWorker2.IsBusy && this.helplineRulerCtrl1.Bmp != null)
            {
                double res = CheckWidthHeight(this.helplineRulerCtrl1.Bmp, true);
                //this.lblResPic.Text = res.ToString();

                //this test is not corresponding to the real amount of RAM being used by the algorithms. It's more or less just an indicator, if to start at all.
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 100L))
                {
                    if (this.cbRectMode.Enabled && this._gc != null)
                    {
                        this._gc.ShowInfo -= _gc_ShowInfo;
                        this._gc.ShowTHInfo -= _gc_ShowTHInfo;
                        this._gc.Dispose();
                        this._gc = null;
                    }

                    //ensure gammaChanged = false on reset
                    if (this.cbRectMode.Checked || this.cbScribbleMode.Checked)
                    {
                        this._oldGamma = (double)this.numGamma.Value;
                        this._oldXi = 1.0;
                    }

                    int intMult = 1;
                    double gamma = (double)this.numGamma.Value;
                    double xi = 1.0;
                    bool setPFGToFG = true;
                    bool skipLearn = this._cbSkipLearnEnabled && this._cbSkipLearnChecked;

                    this.SetControls(false);
                    this.btnMinCut.Text = "Cancel";
                    this.btnMinCut.Enabled = true;
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    this.toolStripStatusLabel1.Text = "resFactor: " + Math.Max(res, 1).ToString("N2");

                    int gmm_comp = 5;

                    int num_Iters = 1;
                    bool rectMode = this.cbRectMode.Checked;
                    bool scribbleMode = this.cbScribbleMode.Checked;
                    Rectangle r = new Rectangle(this._rX, this._rY, this._rW, this._rH);
                    bool autoBias = false;
                    bool skipInit = !(rectMode || scribbleMode);
                    bool workOnPaths = !(rectMode || scribbleMode) && this.cbDraw.Checked;
                    int wh = (int)this.numWH.Value;
                    Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);
                    bool gammaChanged = gamma - this._oldGamma != 0;
                    gammaChanged |= xi - this._oldXi != 0;
                    this._oldGamma = gamma;
                    this._oldXi = xi;
                    bool useEightAdj = this.cbEightAdj.Checked;
                    bool useTh = this.cbUseTh.Checked;
                    double th = useTh ? (double)this.numDblMult.Value : 0;
                    bool initWKpp = true;

                    bool multCapacitiesForTLinks = this.cbMultTLCap.Checked;
                    double multTLinkCapacity = (double)this.numMultTLCap.Value;
                    bool castTLInt = cbCastTLInt.Checked;
                    bool getSourcePart = false;

                    ListSelectionMode selMode = this._KMeansSelMode;

                    double probMult1 = (double)this.numProbMult1.Value;

                    double numItems = this._items;
                    double numCorrect = this._correctItems;
                    double numItems2 = this._items2;
                    double numCorrect2 = this._correctItems2;

                    double kmInitW = this._KMeansInitW;
                    double kmInitH = this._KMeansInitH;

                    int KMeansInitIters = this._KMeansInitIters;
                    bool kMInitRnd = this._kMInitRnd;
                    int KMeansIters = this._KMeansIters;

                    int comp = (int)this.numMaxComponents.Value;
                    bool autoThreshold = this.cbAutoThreshold.Checked;

                    //testmode2
                    bool assumeExpDist = this.cbAssumeExpDist.Checked;

                    this.backgroundWorker2.RunWorkerAsync(new object[] { bWork, gmm_comp, gamma, num_Iters,
                        rectMode, r, autoBias, skipInit, workOnPaths,
                        wh, gammaChanged, intMult, useEightAdj, useTh, th, xi, res, initWKpp,
                        multCapacitiesForTLinks, multTLinkCapacity, castTLInt, getSourcePart, selMode,
                        scribbleMode, probMult1, kmInitW, kmInitH, setPFGToFG,
                        numItems, numCorrect, numItems2, numCorrect2, skipLearn, comp, autoThreshold,
                        KMeansInitIters, kMInitRnd, KMeansIters, assumeExpDist});
                }
            }
        }

        private unsafe void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap? bRes = null;
                int gmm_comp = (int)o[1];
                double gamma = (double)o[2];
                int numIters = (int)o[3];
                bool rectMode = (bool)o[4];
                Rectangle r = (Rectangle)o[5];
                bool autoBias = (bool)o[6];
                bool skipInit = (bool)o[7];
                bool workOnPaths = (bool)o[8];
                int wh = (int)o[9];
                bool gammaChanged = (bool)o[10];
                int intMult = (int)o[11];
                bool quick = false;
                bool useEightAdj = (bool)o[12];
                bool useTh = (bool)o[13];
                double th = (double)o[14];
                double xi = (double)o[15];
                double resPic = (double)o[16];
                bool initWKpp = (bool)o[17];
                bool multCapacitiesForTLinks = (bool)o[18];
                double multTLinkCapacity = (double)o[19];
                bool castTLInt = (bool)o[20];
                bool getSourcePart = (bool)o[21];
                ListSelectionMode selMode = (ListSelectionMode)o[22];
                bool scribbleMode = (bool)o[23];
                Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles = this._scribbles;
                List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq = this._scribbleSeq;
                double probMult1 = (double)o[24];
                double kmInitW = (double)o[25];
                double kmInitH = (double)o[26];
                bool setPFGToFG = (bool)o[27];
                double numItems = (double)o[28];
                double numCorrect = (double)o[29];
                double numItems2 = (double)o[30];
                double numCorrect2 = (double)o[31];
                bool skipLearn = (bool)o[32];
                int comp = (int)o[33];
                bool autoThreshold = (bool)o[34];
                int KMeansInitIters = (int)o[35];
                bool kMInitRnd = (bool)o[36];
                int KMeansIters = (int)o[37];

                bool assumeExpDist = (bool)o[38];

                if (scribbleMode && !rectMode)
                    r = new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                Rectangle clipRect = new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                bool dontFillPath = true;
                bool drawNumComp = true;

                if (scribbleMode && resPic > 1)
                {
                    if (this._scribbles != null)
                    {
                        Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles2 = ResizeAllScribbles(this._scribbles, resPic);
                        this._scribblesBU = this._scribbles;
                        this._scribbles = scribbles2;
                        scribbles = this._scribbles;
                        List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = ResizeScribbleSeq(this._scribbleSeq, resPic, true);
                        this._scribbleSeqBU = this._scribbleSeq;
                        this._scribbleSeq = scribbleSeq2;
                        scribbleSeq = this._scribbleSeq;
                    }
                }

                Bitmap bWork = (Bitmap)o[0];
                Bitmap? bU2 = null;
                if (resPic > 1)
                {
                    Bitmap? bOld = bWork;
                    bU2 = new Bitmap(bWork);
                    bWork = ResampleDown(bWork, ref r, ref clipRect, resPic, scribbleMode, rectMode);
                    if (bOld != null)
                    {
                        bOld.Dispose();
                        bOld = null;
                    }
                }

                Bitmap bTmp = new Bitmap(bWork);
                if (r.Width == 0 && r.Height == 0)
                {
                    this.Invoke(new Action(() => { MessageBox.Show("No Image passed to function. Cancelled operation."); }));
                    if (bU2 != null)
                        bU2.Dispose();
                    e.Result = new Bitmap(bTmp);
                    return;
                }

                if (this._gc == null)
                {
                    this._gc = new GrabCutOp()
                    {
                        Bmp = bWork,
                        Gmm_comp = gmm_comp,
                        Gamma = gamma,
                        NumIters = numIters,
                        RectMode = rectMode,
                        ScribbleMode = scribbleMode,
                        Scribbles = scribbles,
                        Rc = r,
                        BGW = this.backgroundWorker2,
                        QuickEstimation = quick,
                        EightAdj = useEightAdj,
                        UseThreshold = useTh,
                        Threshold = th,
                        MultCapacitiesForTLinks = multCapacitiesForTLinks,
                        MultTLinkCapacity = multTLinkCapacity,
                        CastIntCapacitiesForTLinks = castTLInt,
                        SelectionMode = selMode,
                        ProbMult1 = probMult1,
                        KMInitW = kmInitW,
                        KMInitH = kmInitH,
                        NumItems = numItems,
                        NumCorrect = numCorrect,
                        NumItems2 = numItems2,
                        NumCorrect2 = numCorrect2,
                        AutoThreshold = autoThreshold,
                        KMeansInitIters = KMeansInitIters,
                        kMInitRnd = kMInitRnd,
                        KMeansIters = KMeansIters,
                        AssumeExpDist = assumeExpDist,
                        ScribbleSeq = scribbleSeq
                    };

                    this._gc.ShowInfo += _gc_ShowInfo;
                    this._gc.ShowTHInfo += _gc_ShowTHInfo;
                }

                if (!skipInit)
                {
                    int it = this._gc.Init();

                    if (this._gc.BGW != null && this._gc.BGW.WorkerSupportsCancellation && this._gc.BGW.CancellationPending)
                        it = -4;

                    if (it != 0)
                    {
                        if (bU2 != null)
                            bU2.Dispose();

                        switch (it)
                        {
                            case -1:
                                this.Invoke(new Action(() => { MessageBox.Show("No BGPixels found. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -2:
                                this.Invoke(new Action(() => { MessageBox.Show("No FGPixels found. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -3:
                                this.Invoke(new Action(() => { MessageBox.Show("No Image passed to function. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -4:
                                this.Invoke(new Action(() => { MessageBox.Show("Operation cancelled."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -5:
                                this.Invoke(new Action(() => { MessageBox.Show("Mask is null. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                        }
                    }
                }
                else
                {
                    this._gc.Gamma = gamma;
                    this._gc.GammaChanged = gammaChanged;
                    this._gc.NumIters = numIters;
                    this._gc.Rc = r;
                    this._gc.QuickEstimation = quick;
                    this._gc.EightAdj = useEightAdj;
                    this._gc.UseThreshold = useTh;
                    this._gc.Threshold = th;
                    this._gc.MultCapacitiesForTLinks = multCapacitiesForTLinks;
                    this._gc.MultTLinkCapacity = multTLinkCapacity;
                    this._gc.CastIntCapacitiesForTLinks = castTLInt;
                    this._gc.SelectionMode = selMode;
                    this._gc.ProbMult1 = probMult1;
                    this._gc.KMInitW = kmInitW;
                    this._gc.KMInitH = kmInitH;
                    this._gc.NumItems = numItems;
                    this._gc.NumCorrect = numCorrect;
                    this._gc.NumItems2 = numItems2;
                    this._gc.NumCorrect2 = numCorrect2;
                    this._gc.AutoThreshold = autoThreshold;
                    this._gc.KMeansInitIters = KMeansInitIters;
                    this._gc.kMInitRnd = kMInitRnd;
                    this._gc.KMeansIters = KMeansIters;
                    this._gc.AssumeExpDist = assumeExpDist;
                    this._gc.ScribbleSeq = scribbleSeq;

                    if (!workOnPaths && this._gc.ScribbleMode && this._gc.Scribbles != null && this._gc.Scribbles.Count > 0)
                    {
                        if (!this._gc.RectMode)
                            r = new Rectangle(0, 0, bWork.Width, bWork.Height);
                        this._gc.ReInitScribbles();
                    }
                }

                if (workOnPaths)
                {
                    if (this._allPoints != null && this._allPoints.Count > 0 && this._pointsListSeq != null)
                    {
                        this._gc.GammaChanged = gammaChanged;

                        Dictionary<int, List<Tuple<List<Point>, int>>> allPts2 = this._allPoints;

                        if (resPic > 1)
                            allPts2 = ResizeAllPoints(this._allPoints, resPic);

                        Dictionary<int, List<Tuple<List<Point>, int>>> allPts4 = allPts2;

                        this._gc.SetAllPointsInMask(allPts4, this._pointsListSeq, setPFGToFG);

                        this._gc.SkipLearn = skipLearn;
                    }
                }

                int l = this._gc.RunMinCut();

                if (l != 0)
                {
                    if (bU2 != null)
                        bU2.Dispose();

                    switch (l)
                    {
                        case -1:
                            this.Invoke(new Action(() => { MessageBox.Show("Arrays-Length, or Graph-Length failed test. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case -25:
                            this.Invoke(new Action(() => { MessageBox.Show("Graph-Construction failed. Maybe the threshold is too big. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 100:
                            this.Invoke(new Action(() => { MessageBox.Show("Bmp_width or Bmp_height = 0. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 101:
                            this.Invoke(new Action(() => { MessageBox.Show("Operation cancelled."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 102:
                            this.Invoke(new Action(() => { MessageBox.Show("At least one GMM is null. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 103:
                            this.Invoke(new Action(() => { MessageBox.Show("This Mode only makes sense with RectMode."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 104:
                            this.Invoke(new Action(() => { MessageBox.Show("This Mode only makes sense with ScribbleMode."); }));
                            e.Result = new Bitmap(bTmp);
                            return;
                    }
                }

                List<int>? res = this._gc.Result;

                bRes = new Bitmap(bWork.Width, bWork.Height);

                int[,]? m = this._gc.Mask;

                if ((scribbleMode && !rectMode) || workOnPaths)
                    r = new Rectangle(0, 0, bWork.Width, bWork.Height);

                BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, bRes.Width, bRes.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmWork = bTmp.LockBits(new Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                int w = bTmp.Width;
                int h = bTmp.Height;
                int stride = bmData.Stride;

                byte* p = (byte*)bmData.Scan0;
                byte* pWork = (byte*)bmWork.Scan0;

                //for (int i = 0; i < res.Count(); i++)
                //{
                //    int j = res[i];
                //    int x = j % w;
                //    int y = j / w;

                //    p[x * 4 + y * stride] = pWork[x * 4 + y * stride];
                //    p[x * 4 + y * stride + 1] = pWork[x * 4 + y * stride + 1];
                //    p[x * 4 + y * stride + 2] = pWork[x * 4 + y * stride + 2];
                //    p[x * 4 + y * stride + 3] = pWork[x * 4 + y * stride + 3];
                //}

                if (m != null)
                {
                    int ww = m.GetLength(0);
                    int hh = m.GetLength(1);

                    for (int y = 0; y < h; y++)
                    {
                        if (y + this._numShiftY > 0 && y + this._numShiftY < h)
                            for (int x = 0; x < w; x++)
                            {
                                if (x + this._numShiftX > 0 && x + this._numShiftX < w)
                                    if (x < ww && y < hh && r.Contains(x, y) && (m[x, y] == 1 || m[x, y] == 3))
                                    {
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride] = pWork[x * 4 + y * stride];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 1] = pWork[x * 4 + y * stride + 1];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 2] = pWork[x * 4 + y * stride + 2];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 3] = pWork[x * 4 + y * stride + 3];
                                    }
                            }
                    }
                }

                bTmp.UnlockBits(bmWork);
                bRes.UnlockBits(bmData);

                Bitmap bResCopy = new Bitmap(bRes);
                Bitmap bCTransp = new Bitmap(bRes);

                this.SetBitmap(ref this._bResCopy, ref bResCopy);

                List<ChainCode>? c = GetBoundary(bRes, 0, false);
                c = c?.OrderByDescending(x => x.Coord.Count).ToList();

                if (c != null)
                {
                    int comp2 = c.Count;

                    if (/*!dontFillPath &&*/ c.Count > 0)
                    {
                        if (c.Count > 1000 && (comp > 1000 || !drawNumComp))
                        {
                            using (frmDrawNumComp frm = new frmDrawNumComp(c.Count))
                            {
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    if (frm.checkBox1.Checked)
                                    {
                                        drawNumComp = true;
                                        comp = comp2 = (int)frm.numericUpDown1.Value;
                                    }
                                }
                            }
                        }

                        using (Graphics gx = Graphics.FromImage(bRes))
                        {
                            gx.Clear(Color.Transparent);

                            int amnt = (!drawNumComp) ? c.Count : Math.Min(comp, c.Count);

                            for (int i = 0; i < amnt; i++)
                            {
                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    PointF[] pts = c[i].Coord.Select(pt => new PointF(pt.X, pt.Y)).ToArray();
                                    gp.FillMode = FillMode.Winding;
                                    gp.AddLines(pts);
                                    gp.CloseAllFigures();

                                    //tmp try...catch
                                    try
                                    {
                                        using (TextureBrush tb = new TextureBrush(bTmp))
                                            gx.FillPath(tb, gp);

                                        //using (Pen pen = new Pen(Color.Red, 2))
                                        //    gx.DrawPath(pen, gp);
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                            }
                        }
                    }

                    if (dontFillPath && c.Count > 0)
                    {
                        int amnt = (!drawNumComp) ? c.Count : Math.Min(comp, c.Count);

                        for (int i = 0; i < amnt; i++)
                        {
                            ChainCode cc = c[i];
                            if (ChainFinder.IsInnerOutline(cc))
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Graphics gx = Graphics.FromImage(bRes))
                                        {
                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                            gx.FillPath(Brushes.Transparent, gP);
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                        }
                    }

                    this.SetBitmap(ref this._bResCopyTransp, ref bCTransp);

                    bTmp.Dispose();

                    if (resPic > 1)
                    {
                        Bitmap bOld = bRes;

                        bRes = ResampleUp(bRes, resPic, bU2, dontFillPath, false);
                        Bitmap? bRCopy = ResampleUp(this._bResCopy, resPic, bU2, dontFillPath, false);
                        Bitmap? bRCopy2 = ResampleUp(this._bResCopyTransp, resPic, bU2, true, true);

                        if (bRCopy != null && bRCopy2 != null)
                        {
                            this.SetBitmap(ref this._bResCopy, ref bRCopy);
                            this.SetBitmap(ref this._bResCopyTransp, ref bRCopy2);
                        }
                        //bU2.Dispose();

                        if (bOld != null)
                            bOld.Dispose();
                    }
                    else //if (resPic == 1)
                    {
                        List<ChainCode>? allChains = GetBoundary(this._bResCopyTransp, 0, false);
                        this._allChains = allChains;
                        if (this._removedChains != null)
                            this._removedChains.Clear();

                        if (allChains != null && allChains.Count > 0)
                        {
                            int area = allChains.Sum(a => a.Area);
                            int pxls = w * h;

                            int fc = pxls / area;

                            if (fc > 1000)
                                if (MessageBox.Show("Amount pixels to segmented area ratio is " + fc.ToString() + ". " +
                                    "Rerun with different Initialization of the Gmms?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                {
                                    this._restartDiffInit = true;
                                }
                        }
                    }

                    if (scribbleMode && resPic > 1)
                    {
                        if (this._scribblesBU != null)
                            this._scribbles = this._scribblesBU;
                        if (this._scribbleSeqBU != null)
                            this._scribbleSeq = this._scribbleSeqBU;
                    }

                    e.Result = bRes;
                }
            }
        }

        private void backgroundWorker2_ProgressChanged(object? sender, ProgressChangedEventArgs e)
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

        private void backgroundWorker2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap b = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                this.SetBitmap(ref this._b4Copy, ref bC);

                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                    (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                _undoOPCache?.Add(b);

                if (this.cbDraw.Checked && this._bgRelatedPointsAdded != null && this._bgRelatedPointsAdded.Count > 0)
                {
                    MessageBox.Show("You removed some BG pixels. Maybe increase the amount of displayed components, when those pixels are surrounded by FG pixels.");
                }

                //if (this._resWC > 1)
                //    ResampleAndTranslateBack();

                if (this._bResCopyTransp != null)
                {
                    List<ChainCode>? c2 = GetBoundary(this._bResCopyTransp, 0, false);
                    c2 = c2?.OrderByDescending(x => x.Coord.Count).ToList();

                    //this.numRedrawComponents.Value = this.numRedrawComponents.Maximum = c2.Count;
                }
            }

            this.SetControls(true);

            btnReset2.Enabled = true;
            this._pic_changed = true;

            if (this._allPoints != null)
                this._allPoints.Clear();
            if (this._pathList != null)
                this._pathList.Clear();
            if (this._points != null)
                this._points.Clear();
            if (this._pointsListSeq != null)
                this._pointsListSeq.Clear();
            if (this._bgRelatedPointsAdded != null)
                this._bgRelatedPointsAdded.Clear();
            this._currentDrawOperation = 0;

            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            this.Timer3.Start();
            this.btnMinCut.Text = "Go";

            this.backgroundWorker2.Dispose();
            this.backgroundWorker2 = new BackgroundWorker();
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            this.backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            this.backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;

            if (this._gc != null)
                this._gc.BGW = this.backgroundWorker2;

            this.cbQuickEst_CheckedChanged(this.cbQuickEst, new EventArgs());
            this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());

            if (this._restartDiffInit)
            {
                this._restartDiffInit = false;
                this._KMeansInitW += 1;
                this._KMeansInitH += 1;

                bool bR = this.cbRectMode.Checked;
                bool bS = this.cbScribbleMode.Checked;

                this.button4_Click(this.btnReset2, new EventArgs());

                if (bR)
                    this.cbRectMode.Checked = true;
                if (bS)
                    this.cbScribbleMode.Checked = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                this.btnGo_Click(this.btnGo, new EventArgs());
            }

            this.cbRectMode.Checked = false;
            this.cbRectMode.Enabled = false;
            this.cbScribbleMode.Checked = false;
            this.cbScribbleMode.Enabled = false;
            this.label17.Enabled = this.numComponents2.Enabled = true;

            this.toolStripStatusLabel4.Text = "done";

            this.cbAutoCropFromOrig.Enabled = this.btnCropFromOrig.Enabled = true;

            if (this.timer1.Enabled)
                this.timer1.Stop();
            this.timer1.Start();
        }

        private void btnLoadScribbles_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Filter = "Scribbles-Files (*.tgScribbles)|*.tgScribbles";
            this.openFileDialog1.FileName = "File1.tgScribbles";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                SavedScribbles? f = DeserializeF(this.openFileDialog1.FileName);
                btnClearScribbles.PerformClick();

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
                        this._scribblesBU = this._scribbles;
                        this._scribbles = scribbles2;
                        List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = ResizeScribbleSeq(this._scribbleSeq, resPic, true);
                        this._scribbleSeqBU = this._scribbleSeq;
                        this._scribbleSeq = scribbleSeq2;
                    }

                    if (this.cbScribbleMode.Enabled)
                    {
                        this.cbRectMode.Checked = false;
                        this.cbScribbleMode.Checked = true;
                    }

                    if (f.Bmp != null && this.cbLSBmp.Checked)
                    {
                        Bitmap? b = ConvertFromBase64(f.Bmp);

                        if (b != null)
                        {
                            this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");
                            _undoOPCache?.Add(b);

                            btnReset2.Enabled = true;
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

        //private bool SerializeF(SavedScribbles f, string FileName)
        //{
        //    //serialize data to binary
        //    IFormatter formatter = new BinaryFormatter();
        //    Stream stream = null;

        //    bool bError = false;

        //    try
        //    {
        //        //write the data to the file
        //        stream = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
        //        formatter.Serialize(stream, f);
        //    }
        //    catch (Exception ex)
        //    {
        //        bError = true;
        //        Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
        //    }

        //    try
        //    {
        //        //stream.Close()
        //        stream.Dispose();
        //        stream = null;
        //    }
        //    catch
        //    { }

        //    return !bError;
        //}

        //private SavedScribbles DeserializeF(string fileName)
        //{
        //    //deserialize from binary
        //    IFormatter formatter = new BinaryFormatter();
        //    formatter.Binder = new AvoidAGrabCut_To_Easy_Binder();
        //    Stream stream = null;
        //    SavedScribbles f = null;

        //    try
        //    {
        //        //write the data to the file
        //        stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

        //        f = (SavedScribbles)formatter.Deserialize(stream);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(DateTime.Now.ToString() + " " + ex.ToString());
        //    }

        //    try
        //    {
        //        //stream.Close()
        //        stream.Dispose();
        //        stream = null;
        //    }
        //    catch
        //    { }

        //    return f;
        //}

        //if you want to upgrade to .net 8.0
        //you then can remove the binder-file (AvoidAGrabCut_To_Easy_Binder)
        //if you want to use these in this .net-framework version, install the
        //System.Text.Json NuGet package
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

        private void btnOutline_Click(object sender, EventArgs e)
        {
            if ((this._b4Copy != null || this.helplineRulerCtrl2.Bmp != null) && this._bmpBU != null && this.CachePathAddition != null)
            {
                using (frmProcOutline frm = new frmProcOutline(this.helplineRulerCtrl2.Bmp, this._bmpBU, this.CachePathAddition))
                {
                    frm.SetupCache();

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        Bitmap b = new Bitmap(frm.FBitmap);

                        this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                        //Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                        //this.SetBitmap(ref this._b4Copy, ref bC);

                        this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                        this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                            (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                        _undoOPCache?.Add(b);
                    }
                }
            }

            this.btnRecut.Enabled = this.numComponents2.Enabled = false;
        }

        private void numShiftX_ValueChanged(object sender, EventArgs e)
        {
            this._numShiftX = (int)this.numShiftX.Value;
        }

        private void numShiftY_ValueChanged(object sender, EventArgs e)
        {
            this._numShiftY = (int)this.numShiftY.Value;
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

                                        //Bitmap bC = new Bitmap(bmp);
                                        //this.SetBitmap(ref this._bmpBU, ref bC);
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

        private void btnCompose_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                if (this.CachePathAddition != null)
                {
                    bool test = true;
                    if (test)
                    {
                        using (PseudoShadow.frmComposePseudoShadow frm = new PseudoShadow.frmComposePseudoShadow(new Bitmap(this.helplineRulerCtrl2.Bmp), this.CachePathAddition))
                        {
                            frm.SetupCache();
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                if (frm.FBitmap != null)
                                {
                                    Bitmap? bmp = new Bitmap(frm.FBitmap);

                                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");
                                    _undoOPCache?.Add(bmp);

                                    if (this.cmbZoom.SelectedItem != null)
                                        this.helplineRulerCtrl2.SetZoom(this.cmbZoom.SelectedItem.ToString());
                                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                                    //Bitmap bC = new Bitmap(bmp);
                                    //this.SetBitmap(ref this._bmpBU, ref bC);

                                    this._pic_changed = true;
                                    this.btnOK.Enabled = true;
                                }
                            }
                        }
                    }
                    else
                        using (frmCompose frm = new frmCompose(this.helplineRulerCtrl2.Bmp, this.CachePathAddition))
                        {
                            frm.SetupCache();
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                if (frm.FBitmap != null)
                                {
                                    Bitmap? bmp = new Bitmap(frm.FBitmap);

                                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");
                                    _undoOPCache?.Add(bmp);

                                    if (this.cmbZoom.SelectedItem != null)
                                        this.helplineRulerCtrl2.SetZoom(this.cmbZoom.SelectedItem.ToString());
                                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                                    //Bitmap bC = new Bitmap(bmp);
                                    //this.SetBitmap(ref this._bmpBU, ref bC);
                                }
                            }
                        }
                }
            }

            this.btnRecut.Enabled = this.numComponents2.Enabled = false;
        }

        private void btnInitSettings_Click(object sender, EventArgs e)
        {
            using (frmKMeansSettings frm = new frmKMeansSettings())
            {
                frm.cmbSelMode.SelectedIndex = (int)this._KMeansSelMode;
                frm.numKMeansIters.Value = (decimal)this._KMeansIters;
                frm.numInitIters.Value = (decimal)this._KMeansInitIters;
                frm.numInitW.Value = (decimal)this._KMeansInitW;
                frm.numInitH.Value = (decimal)this._KMeansInitH;
                frm.cbInitRnd.Checked = this._kMInitRnd;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    if (frm.cmbSelMode.SelectedItem?.ToString() != null)
                    {
                        string? d = frm.cmbSelMode.SelectedItem.ToString();
                        if (d != null)
                        {
                            ListSelectionMode selMode = (ListSelectionMode)System.Enum.Parse(typeof(ListSelectionMode), d);
                            this._KMeansSelMode = selMode;
                            this._KMeansIters = (int)frm.numKMeansIters.Value;
                            this._KMeansInitIters = (int)frm.numInitIters.Value;
                            this._KMeansInitW = (double)frm.numInitW.Value;
                            this._KMeansInitH = (double)frm.numInitH.Value;
                            this._kMInitRnd = frm.cbInitRnd.Checked;
                        }
                    }
                }
            }
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

                            this._chainsFromFrm = c;

                            this.cbScribbleMode.Checked = true;
                            this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        }
                    }
                }
            }
        }

        private void cbOverlay_CheckedChanged(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._scribbles != null && this._scribbles.Count > 0)
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private async void numComponents2_ValueChanged(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._allChains != null && !_dontUpdateNumComp)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl2.Enabled = false;
                this.helplineRulerCtrl2.Refresh();
                _dontUpdateNumComp = true; //the simple, but effective way of not getting an object_in_use_elsewhere error (I dont know, if this is good practice or not... (you could also create a new bitmap before starting the thread and pass it over))

                this.btnRecut.Enabled = false;

                Bitmap? bOut = await Task.Run(() =>
                {
                    Bitmap? bOut = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                    if (bOut != null)
                    {
                        List<ChainCode>? allChains = this._allChains;
                        int comp = (int)this.numComponents2.Value;

                        allChains = allChains?.OrderByDescending(x => x.Coord.Count).ToList();

                        if (allChains != null && allChains.Count > 1000)
                            allChains = allChains.Take(1000).ToList();

                        if (allChains != null && allChains.Count > 0)
                        {
                            //begin to redraw each component
                            using (Graphics gx = Graphics.FromImage(bOut))
                            {
                                gx.Clear(Color.Transparent);

                                int amnt = Math.Min(comp, allChains.Count);

                                for (int i = 0; i < amnt; i++)
                                {
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        PointF[] pts = allChains[i].Coord.Select(pt => new PointF(pt.X, pt.Y)).ToArray();
                                        //make sure, each path is treated as an "outer-outline"
                                        gp.FillMode = FillMode.Winding;
                                        gp.AddLines(pts);
                                        gp.CloseAllFigures();

                                        //tmp try...catch
                                        try
                                        {
                                            using (TextureBrush tb = new TextureBrush(this.helplineRulerCtrl1.Bmp))
                                                gx.FillPath(tb, gp);

                                            //using (Pen pen = new Pen(Color.Red, 2))
                                            //    gx.DrawPath(pen, gp);
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(exc.ToString());
                                        }
                                    }
                                }
                            }
                        }

                        //now redraw the inner outlines and "transparent" components
                        //we can do this in the easy way with setting graphics.CompositionMode to sourceCopy
                        //because the whole operations are full_pixel_wise (at least for the standard 4-connectivity)
                        if (allChains != null && allChains.Count > 0)
                        {
                            int amnt = Math.Min(comp, allChains.Count);

                            for (int i = 0; i < amnt; i++)
                            {
                                ChainCode cc = allChains[i];
                                if (ChainFinder.IsInnerOutline(cc))
                                    using (GraphicsPath gP = new GraphicsPath())
                                    {
                                        try
                                        {
                                            gP.StartFigure();
                                            PointF[] pts = cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                            gP.AddLines(pts);

                                            using (Graphics gx = Graphics.FromImage(bOut))
                                            {
                                                gx.CompositingMode = CompositingMode.SourceCopy;
                                                gx.FillPath(Brushes.Transparent, gP);
                                            }
                                        }
                                        catch (Exception exc)
                                        {
                                            Console.WriteLine(exc.ToString());
                                        }
                                    }
                            }
                        }

                        OnShowInfo("done");

                    }

                    return bOut;
                });

                if (bOut != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");

                    _undoOPCache?.Add(bOut);

                    this._pic_changed = true;

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                    this.helplineRulerCtrl2.dbPanel1.Invalidate();

                    _dontUpdateNumComp = false;
                }

                this.SetControls(true);
                this.helplineRulerCtrl1.Enabled = true;
                this.helplineRulerCtrl2.Enabled = true;

                this.cbRectMode.Enabled = false;
            }
        }

        private void numComponents2_DoubleClick(object sender, EventArgs e)
        {
            this.numMaxComponents.Value = this.numComponents2.Value;
        }

        private void btnCFM_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this.CachePathAddition != null)
            {
                using (frmAlphaMatte frm = new frmAlphaMatte(this.helplineRulerCtrl1.Bmp, this.CachePathAddition))
                {
                    frm.SetupCache();
                    if (this._scribbles != null)
                    {
                        Tuple<Dictionary<int, Dictionary<int, List<List<Point>>>>, List<Tuple<int, int, int, bool, List<List<Point>>>>> scr = this.CloneScribbles(this._scribbles, this._scribbleSeq);
                        frm.SetScribbles(scr.Item1, scr.Item2);
                    }

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (frm.FBitmap != null)
                        {
                            Bitmap b = new Bitmap(frm.FBitmap);

                            this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                            Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                            this.SetBitmap(ref this._b4Copy, ref bC);

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

            this.btnRecut.Enabled = this.numComponents2.Enabled = false;
        }

        private Tuple<Dictionary<int, Dictionary<int, List<List<Point>>>>, List<Tuple<int, int, int, bool, List<List<Point>>>>> CloneScribbles(
                                Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles, List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq)
        {
            Dictionary<int, Dictionary<int, List<List<Point>>>> dOut = new Dictionary<int, Dictionary<int, List<List<Point>>>>();
            List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSequence = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

            if (scribbles != null)
            {
                SavedScribbles f = new SavedScribbles();

                if (scribbles.ContainsKey(0)) //BG
                {
                    if (scribbles[0] != null)
                    {
                        f.BGSizes = scribbles[0].Keys.ToArray();
                        f.BGPoints = new Point[f.BGSizes.Length][][];
                        int i = 0;

                        foreach (int wh in scribbles[0].Keys)
                        {
                            f.BGPoints[i] = new Point[scribbles[0][wh].Count][];

                            List<List<Point>> whPts = scribbles[0][wh];
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

                if (scribbles.ContainsKey(1)) //FG
                {
                    if (scribbles[1] != null)
                    {
                        f.FGSizes = scribbles[1].Keys.ToArray();
                        f.FGPoints = new Point[f.FGSizes.Length][][];
                        int i = 0;

                        foreach (int wh in scribbles[1].Keys)
                        {
                            f.FGPoints[i] = new Point[scribbles[1][wh].Count][];

                            List<List<Point>> whPts = scribbles[1][wh];
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

                if (scribbles.ContainsKey(3)) //Unknown
                {
                    if (scribbles[3] != null)
                    {
                        f.UknwnSizes = scribbles[3].Keys.ToArray();
                        f.UknwnPoints = new Point[f.UknwnSizes.Length][][];
                        int i = 0;

                        foreach (int wh in scribbles[3].Keys)
                        {
                            f.UknwnPoints[i] = new Point[scribbles[3][wh].Count][];

                            List<List<Point>> whPts = scribbles[3][wh];
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

                if (scribbleSeq != null)
                    f.ScribbleSequence = scribbleSeq;

                if (f != null)
                {
                    dOut = f.ToDictionary();

                    if (this.cbScribbleMode.Enabled)
                    {
                        this.cbRectMode.Checked = false;
                        this.cbScribbleMode.Checked = true;
                    }

                    if (f.ScribbleSequence != null)
                        scribbleSequence = f.ScribbleSequence;

                    f.Dispose();
                }
            }

            return Tuple.Create(dOut, scribbleSequence);
        }

        private void cbHighlight_CheckedChanged(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        #region DoAll

        private void btnDoAll_Click(object sender, EventArgs e)
        {
            //reset state
            if (this.backgroundWorker1.IsBusy || this.backgroundWorker2.IsBusy)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                if (this.backgroundWorker2.IsBusy)
                    this.backgroundWorker2.CancelAsync();

                return;
            }

            if ((!this.cbRectMode.Checked && !this.cbScribbleMode.Checked && this._b4Copy == null) ||
                (this.cbRectMode.Checked && this._rW == 0) || (this.cbRectMode.Checked && this._rH == 0) ||
                (this.cbScribbleMode.Checked && this._scribbles == null) || this.btnDoAll.Text == "Cancel")
            {
                MessageBox.Show("No rect specified.");
                this.SetControls(true);
                this.btnDoAll.Text = "DoAll";
                this.button4_Click(this.btnReset2, new EventArgs());
                this.cbRectMode.Checked = true;
                return;
            }

            this._cancelledOp = false;

            if (this.bgwDoAll1.IsBusy || this.bgwDoAll3.IsBusy || this.bgwDoAll4.IsBusy || this.bgwDoAll2.IsBusy)
            {
                if (this.bgwDoAll1.IsBusy)
                    this.bgwDoAll1.CancelAsync();

                if (this.bgwDoAll3.IsBusy)
                    this.bgwDoAll3.CancelAsync();

                if (this.bgwDoAll4.IsBusy)
                    this.bgwDoAll4.CancelAsync();

                if (this.bgwDoAll2.IsBusy)
                    this.bgwDoAll2.CancelAsync();

                Cleanup();

                return;
            }

            //reset variable
            this._maxSize = (int)this.numMaxSize.Value;
            this._finishedPart1 = false;

            if (!this.backgroundWorker1.IsBusy && this.helplineRulerCtrl1.Bmp != null)
            {
                //get the resizeFactor
                double res = CheckWidthHeight(this.helplineRulerCtrl1.Bmp, true);
                //this.lblResPic.Text = res.ToString();

                //this test is not corresponding to the real amount of RAM being used by the algorithms. It's more or less just an indicator, if to start at all.
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 100L))
                {
                    //reset
                    if (this.cbRectMode.Enabled && this._gc != null)
                    {
                        this._gc.ShowInfo -= _gc_ShowInfo;
                        this._gc.ShowTHInfo -= _gc_ShowTHInfo;
                        this._gc.Dispose();
                        this._gc = null;
                    }

                    //ensure gammaChanged = false on reset
                    if (this.cbRectMode.Checked || this.cbScribbleMode.Checked)
                    {
                        this._oldGamma = (double)this.numGamma.Value;
                        this._oldXi = 1.0;
                    }

                    //now get the needed parameters from the controls
                    //get some of them before we disabel the UI-Controls
                    int intMult = 1;
                    double gamma = (double)this.numGamma.Value;
                    double xi = 1.0;
                    bool setPFGToFG = this._cbSetPFGToFG;

                    bool skipLearn = this._cbSkipLearnEnabled && this._cbSkipLearnChecked;

                    this.SetControls(false);
                    this.btnDoAll.Text = "Cancel";
                    this.btnDoAll.Enabled = true;
                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    this.toolStripStatusLabel1.Text = "resFactor: " + Math.Max(res, 1).ToString("N2");

                    int gmm_comp = (int)this.numGmmComp.Value;

                    int num_Iters = 2;
                    bool rectMode = this.cbRectMode.Checked;
                    bool scribbleMode = this.cbScribbleMode.Checked;
                    if (this._rW == 0)
                        this._rW = this.helplineRulerCtrl1.Bmp.Width;
                    if (this._rH == 0)
                        this._rH = this.helplineRulerCtrl1.Bmp.Height;
                    Rectangle r = new Rectangle(this._rX, this._rY, this._rW, this._rH);
                    bool autoBias = false;
                    bool skipInit = !(rectMode || scribbleMode);
                    bool workOnPaths = !(rectMode || scribbleMode) && this.cbDraw.Checked;
                    int wh = (int)this.numWH.Value;
                    Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);
                    bool gammaChanged = gamma - this._oldGamma != 0;
                    gammaChanged |= xi - this._oldXi != 0;
                    this._oldGamma = gamma;
                    this._oldXi = xi;
                    bool useEightAdj = this.cbEightAdj.Checked;
                    bool useTh = this.cbUseTh.Checked;
                    double th = useTh ? (double)this.numDblMult.Value : 0;
                    bool initWKpp = true;

                    bool multCapacitiesForTLinks = this.cbMultTLCap.Checked;
                    double multTLinkCapacity = (double)this.numMultTLCap.Value;
                    bool castTLInt = cbCastTLInt.Checked;
                    bool getSourcePart = false;

                    ListSelectionMode selMode = this._KMeansSelMode;

                    double probMult1 = (double)this.numProbMult1.Value;

                    double numItems = this._items;
                    double numCorrect = this._correctItems;
                    double numItems2 = this._items2;
                    double numCorrect2 = this._correctItems2;

                    double kmInitW = this._KMeansInitW;
                    double kmInitH = this._KMeansInitH;

                    int KMeansInitIters = this._KMeansInitIters;
                    bool kMInitRnd = this._kMInitRnd;
                    int KMeansIters = this._KMeansIters;

                    int comp = (int)this.numMaxComponents.Value;
                    bool autoThreshold = this.cbAutoThreshold.Checked;

                    //testmode2
                    bool assumeExpDist = this.cbAssumeExpDist.Checked;

                    this.ShowInfo += FrmAvoidAGrabCutEasy_ShowInfo;

                    //should already be stopped
                    if (this.bgwDoAll1.IsBusy || this.bgwDoAll3.IsBusy || this.bgwDoAll4.IsBusy || this.bgwDoAll2.IsBusy)
                    {
                        if (this.bgwDoAll1.IsBusy)
                            this.bgwDoAll1.CancelAsync();

                        if (this.bgwDoAll3.IsBusy)
                            this.bgwDoAll3.CancelAsync();

                        if (this.bgwDoAll4.IsBusy)
                            this.bgwDoAll4.CancelAsync();

                        if (this.bgwDoAll2.IsBusy)
                            this.bgwDoAll2.CancelAsync();

                        Cleanup();

                        return;
                    }

                    //now start the work
                    this.bgwDoAll1.RunWorkerAsync(new object[] { bWork, gmm_comp, gamma, num_Iters,
                        rectMode, r, autoBias, skipInit, workOnPaths, wh, gammaChanged, intMult, useEightAdj,
                        useTh, th, xi, res, initWKpp, multCapacitiesForTLinks, multTLinkCapacity, castTLInt,
                        getSourcePart, selMode, scribbleMode, probMult1, kmInitW, kmInitH, setPFGToFG,
                        numItems, numCorrect, numItems2, numCorrect2, skipLearn, comp, autoThreshold,
                        KMeansInitIters, kMInitRnd, KMeansIters, assumeExpDist});
                }
            }
        }

        private unsafe void bgwDoAll1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap? bRes = null;
                int gmm_comp = (int)o[1];
                double gamma = (double)o[2];
                int numIters = (int)o[3];
                bool rectMode = (bool)o[4];
                Rectangle r = (Rectangle)o[5];
                bool autoBias = (bool)o[6];
                bool skipInit = (bool)o[7];
                bool workOnPaths = (bool)o[8];
                int wh = (int)o[9];
                bool gammaChanged = (bool)o[10];
                int intMult = (int)o[11];
                bool quick = true;
                bool useEightAdj = (bool)o[12];
                bool useTh = (bool)o[13];
                double th = (double)o[14];
                double xi = (double)o[15];
                double resPic = (double)o[16];
                bool initWKpp = (bool)o[17];
                bool multCapacitiesForTLinks = (bool)o[18];
                double multTLinkCapacity = (double)o[19];
                bool castTLInt = (bool)o[20];
                bool getSourcePart = (bool)o[21];
                ListSelectionMode selMode = (ListSelectionMode)o[22];
                bool scribbleMode = (bool)o[23];
                Dictionary<int, Dictionary<int, List<List<Point>>>>? scribbles = this._scribbles;
                List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq = this._scribbleSeq;
                double probMult1 = (double)o[24];
                double kmInitW = (double)o[25];
                double kmInitH = (double)o[26];

                bool setPFGToFG = (bool)o[27];
                double numItems = (double)o[28];
                double numCorrect = (double)o[29];
                double numItems2 = (double)o[30];
                double numCorrect2 = (double)o[31];
                bool skipLearn = (bool)o[32];

                if (scribbleMode && !rectMode)
                    r = new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);

                Rectangle clipRect = new Rectangle(0, 0, this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                bool dontFillPath = true;
                bool drawNumComp = true;
                int comp = (int)o[33];
                bool autoThreshold = (bool)o[34];
                int KMeansInitIters = (int)o[35];
                bool kMInitRnd = (bool)o[36];
                int KMeansIters = (int)o[37];

                bool assumeExpDist = (bool)o[38];

                //if we have a large pic that will be resized during these OPs
                //we need to resize also the scribbles, if present
                if (scribbleMode && resPic > 1)
                {
                    if (this._scribbles != null)
                    {
                        Dictionary<int, Dictionary<int, List<List<Point>>>> scribbles2 = ResizeAllScribbles(this._scribbles, resPic);
                        this._scribblesBU = this._scribbles;
                        this._scribbles = scribbles2;
                        scribbles = this._scribbles;
                        List<Tuple<int, int, int, bool, List<List<Point>>>> scribbleSeq2 = ResizeScribbleSeq(this._scribbleSeq, resPic, true);
                        this._scribbleSeqBU = this._scribbleSeq;
                        this._scribbleSeq = scribbleSeq2;
                        scribbleSeq = this._scribbleSeq;
                    }
                }

                //resize the input bmp
                Bitmap bWork = (Bitmap)o[0];
                Bitmap? bU2 = null;
                if (resPic > 1)
                {
                    Bitmap? bOld = bWork;
                    bU2 = new Bitmap(bWork);
                    bWork = ResampleDown(bWork, ref r, ref clipRect, resPic, scribbleMode, rectMode);
                    if (bOld != null)
                    {
                        bOld.Dispose();
                        bOld = null;
                    }
                }

                //do a check to ensure a correct initialisation of the GC_OP
                Bitmap bTmp = new Bitmap(bWork);
                if (r.Width == 0 && r.Height == 0)
                {
                    this.Invoke(new Action(() => { OnShowInfo("No Image passed to function. Cancelled operation."); }));
                    if (bU2 != null)
                        bU2.Dispose();
                    e.Result = new Bitmap(bTmp);
                    return;
                }

                //create the operator for the GrabcutALike methods
                //if we have already a gc prrsent, we set its params later
                if (this._gc == null)
                {
                    this._gc = new GrabCutOp()
                    {
                        Bmp = bWork,
                        Gmm_comp = gmm_comp,
                        Gamma = gamma,
                        NumIters = numIters,
                        RectMode = rectMode,
                        ScribbleMode = scribbleMode,
                        Scribbles = scribbles,
                        Rc = r,
                        BGW = this.bgwDoAll1,
                        QuickEstimation = quick,
                        EightAdj = useEightAdj,
                        UseThreshold = useTh,
                        Threshold = th,
                        MultCapacitiesForTLinks = multCapacitiesForTLinks,
                        MultTLinkCapacity = multTLinkCapacity,
                        CastIntCapacitiesForTLinks = castTLInt,
                        SelectionMode = selMode,
                        ProbMult1 = probMult1,
                        KMInitW = kmInitW,
                        KMInitH = kmInitH,
                        NumItems = numItems,
                        NumCorrect = numCorrect,
                        NumItems2 = numItems2,
                        NumCorrect2 = numCorrect2,
                        AutoThreshold = autoThreshold,
                        KMeansInitIters = KMeansInitIters,
                        kMInitRnd = kMInitRnd,
                        KMeansIters = KMeansIters,
                        AssumeExpDist = assumeExpDist,
                        ScribbleSeq = scribbleSeq
                    };

                    this._gc.ShowInfo += _gc_ShowInfo;
                    this._gc.ShowTHInfo += _gc_ShowTHInfo;
                }

                //now do the initialisation
                //eg create the mask, preclassify the imagedata, compute the smootheness function and init the Gmms
                if (!skipInit)
                {
                    int it = this._gc.Init();

                    if (this._gc.BGW != null && this._gc.BGW.WorkerSupportsCancellation && this._gc.BGW.CancellationPending)
                        it = -4;

                    if (it != 0)
                    {
                        if (bU2 != null)
                            bU2.Dispose();

                        switch (it)
                        {
                            case -1:
                                this.Invoke(new Action(() => { OnShowInfo("No BGPixels found. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -2:
                                this.Invoke(new Action(() => { OnShowInfo("No FGPixels found. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -3:
                                this.Invoke(new Action(() => { OnShowInfo("No Image passed to function. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -4:
                                this.Invoke(new Action(() => { OnShowInfo("Operation cancelled."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                            case -5:
                                this.Invoke(new Action(() => { OnShowInfo("Mask is null. Cancelled operation."); }));
                                e.Result = new Bitmap(bTmp);
                                return;
                        }
                    }
                }
                else
                {
                    this._gc.Gamma = gamma;
                    this._gc.GammaChanged = gammaChanged;
                    this._gc.NumIters = numIters;
                    this._gc.Rc = r;
                    this._gc.QuickEstimation = quick;
                    this._gc.EightAdj = useEightAdj;
                    this._gc.UseThreshold = useTh;
                    this._gc.Threshold = th;
                    this._gc.MultCapacitiesForTLinks = multCapacitiesForTLinks;
                    this._gc.MultTLinkCapacity = multTLinkCapacity;
                    this._gc.CastIntCapacitiesForTLinks = castTLInt;
                    this._gc.SelectionMode = selMode;
                    this._gc.ProbMult1 = probMult1;
                    this._gc.KMInitW = kmInitW;
                    this._gc.KMInitH = kmInitH;
                    this._gc.NumItems = numItems;
                    this._gc.NumCorrect = numCorrect;
                    this._gc.NumItems2 = numItems2;
                    this._gc.NumCorrect2 = numCorrect2;
                    this._gc.AutoThreshold = autoThreshold;
                    this._gc.KMeansInitIters = KMeansInitIters;
                    this._gc.kMInitRnd = kMInitRnd;
                    this._gc.KMeansIters = KMeansIters;
                    this._gc.AssumeExpDist = assumeExpDist;
                    this._gc.ScribbleSeq = scribbleSeq;

                    if (!workOnPaths && this._gc.ScribbleMode && this._gc.Scribbles != null && this._gc.Scribbles.Count > 0)
                    {
                        if (!this._gc.RectMode)
                            r = new Rectangle(0, 0, bWork.Width, bWork.Height);
                        this._gc.ReInitScribbles();
                    }
                }

                //if we use scribbles on the previously computed result
                if (workOnPaths)
                {
                    if (this._allPoints != null && this._allPoints.Count > 0 && this._pointsListSeq != null)
                    {
                        this._gc.GammaChanged = gammaChanged;

                        Dictionary<int, List<Tuple<List<Point>, int>>> allPts2 = this._allPoints;

                        if (resPic > 1)
                            allPts2 = ResizeAllPoints(this._allPoints, resPic);

                        Dictionary<int, List<Tuple<List<Point>, int>>> allPts4 = allPts2;

                        this._gc.SetAllPointsInMask(allPts4, this._pointsListSeq, setPFGToFG);

                        this._gc.SkipLearn = skipLearn;
                    }
                }

                //test
                //since the MinCut in the GrabCut method uses neighborhood information,
                //let's try to use this information in our method too.
                //For this, we compute a gradient pic and from this, we get the inverted
                //luminance map. This may change, also the settings for the invGaussGrad
                //method for conputing the gradient pic may change.
                if (this.cbCompLumMap.Checked)
                {
                    if (this._iggLuminanceMap != null || (this._iggLuminanceMap2 != null && this._useLumMapBasePic))
                    {
                        this._gc.IGGLuminanceMap = (this._useLumMapBasePic && this._iggLuminanceMap2 != null) ? this._iggLuminanceMap2 : this._iggLuminanceMap;
                        this._gc.LumMapSettings = this._lmas;
                    }
                    else if (this._iggLuminanceMap == null)
                    {
                        if (this._bmpBU != null)
                        {
                            this.Invoke(new Action(() =>
                            {
                                this.toolStripStatusLabel4.Text = "Computing LumMap";
                                this.lblLumMap.Text = "computing...";
                            }));
                            LuminancMapOp lop = new LuminancMapOp();
                            lop.ProgressPlus += lop_ProgressPlus;
                            using Bitmap bmp = new Bitmap(this._bmpBU);
                            float[,]? lMap = lop.ComputeInvLuminanceMapSync(bmp);
                            this._iggLuminanceMap = lMap;
                            this._gc.IGGLuminanceMap = (this._useLumMapBasePic && this._iggLuminanceMap2 != null) ? this._iggLuminanceMap2 : lMap;
                            this._gc.LumMapSettings = this._lmas;
                            lop.ProgressPlus -= lop_ProgressPlus;
                            this.Invoke(new Action(() =>
                            {
                                this.toolStripStatusLabel4.Text = "LumMap done.";
                                this.lblLumMap.Text = "done.";
                            }));
                        }
                    }
                }

                //now do the work ...
                int l = this._gc.Run();

                if (l != 0)
                {
                    if (bU2 != null)
                        bU2.Dispose();

                    switch (l)
                    {
                        case -1:
                            this.Invoke(new Action(() => { OnShowInfo("Arrays-Length, or Graph-Length failed test. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case -25:
                            this.Invoke(new Action(() => { OnShowInfo("Graph-Construction failed. Maybe the threshold is too big. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 100:
                            this.Invoke(new Action(() => { OnShowInfo("Bmp_width or Bmp_height = 0. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 101:
                            this.Invoke(new Action(() => { OnShowInfo("Operation cancelled."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 102:
                            this.Invoke(new Action(() => { OnShowInfo("At least one GMM is null. Cancelled operation."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 103:
                            this.Invoke(new Action(() => { OnShowInfo("This Mode only makes sense with RectMode."); }));
                            e.Result = new Bitmap(bTmp);
                            return;

                        case 104:
                            this.Invoke(new Action(() => { OnShowInfo("This Mode only makes sense with ScribbleMode."); }));
                            e.Result = new Bitmap(bTmp);
                            return;
                    }
                }

                //... and get the result ...
                List<int>? res = this._gc.Result;

                //... and the result image
                bRes = new Bitmap(bWork.Width, bWork.Height);

                int[,]? m = this._gc.Mask;
                int w = bTmp.Width;
                int h = bTmp.Height;

                if ((scribbleMode && !rectMode) || workOnPaths)
                    r = new Rectangle(0, 0, bWork.Width, bWork.Height);

                //lock the bmps for fast processing
                BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, bRes.Width, bRes.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmWork = bTmp.LockBits(new Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                int stride = bmData.Stride;

                //get the references to the pointer addresses
                byte* p = (byte*)bmData.Scan0;
                byte* pWork = (byte*)bmWork.Scan0;

                //for (int i = 0; i < res.Count(); i++)
                //{
                //    int j = res[i];
                //    int x = j % w;
                //    int y = j / w;

                //    p[x * 4 + y * stride] = pWork[x * 4 + y * stride];
                //    p[x * 4 + y * stride + 1] = pWork[x * 4 + y * stride + 1];
                //    p[x * 4 + y * stride + 2] = pWork[x * 4 + y * stride + 2];
                //    p[x * 4 + y * stride + 3] = pWork[x * 4 + y * stride + 3];
                //}

                if (m != null)
                {
                    //write the data
                    int ww = m.GetLength(0);
                    int hh = m.GetLength(1);

                    for (int y = 0; y < h; y++)
                    {
                        if (y + this._numShiftY > 0 && y + this._numShiftY < h)
                            for (int x = 0; x < w; x++)
                            {
                                if (x + this._numShiftX > 0 && x + this._numShiftX < w)
                                    if (x < ww && y < hh && r.Contains(x, y) && (m[x, y] == 1 || m[x, y] == 3))
                                    {
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride] = pWork[x * 4 + y * stride];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 1] = pWork[x * 4 + y * stride + 1];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 2] = pWork[x * 4 + y * stride + 2];
                                        p[(x + this._numShiftX) * 4 + (y + this._numShiftY) * stride + 3] = pWork[x * 4 + y * stride + 3];
                                    }
                            }
                    }
                }

                //and unlock the bmps
                bTmp.UnlockBits(bmWork);
                bRes.UnlockBits(bmData);

                //now do some analysis of the result, to be able to redraw the resultpic with a different set of components
                Bitmap bResCopy = new Bitmap(bRes);
                Bitmap bCTransp = new Bitmap(bRes);

                //BU of the original result
                this.SetBitmap(ref this._bResCopy, ref bResCopy);

                //use a ChainCode [that works on the - invisible - "cracks" between the pixels, because it is very fast aand reliable]
                List<ChainCode>? c = GetBoundary(bRes, 0, false);
                c = c?.OrderByDescending(x => x.Coord.Count).ToList();

                if (c != null)
                {
                    int comp2 = c.Count;

                    if (c.Count > 0)
                    {
                        //if we have a very lot of components, allow the user to restrict the amount
                        if (c.Count > 1000 && (comp > 1000 || !drawNumComp))
                        {
                            using (frmDrawNumComp frm = new frmDrawNumComp(c.Count))
                            {
                                if (frm.ShowDialog() == DialogResult.OK)
                                {
                                    if (frm.checkBox1.Checked)
                                    {
                                        drawNumComp = true;
                                        comp = comp2 = (int)frm.numericUpDown1.Value;
                                    }
                                }
                            }
                        }

                        //now begin to redraw each component
                        using (Graphics gx = Graphics.FromImage(bRes))
                        {
                            gx.Clear(Color.Transparent);

                            int amnt = (!drawNumComp) ? c.Count : Math.Min(comp, c.Count);

                            for (int i = 0; i < amnt; i++)
                            {
                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    PointF[] pts = c[i].Coord.Select(pt => new PointF(pt.X, pt.Y)).ToArray();
                                    //make sure, each path is treated as an "outer-outline"
                                    gp.FillMode = FillMode.Winding;
                                    gp.AddLines(pts);
                                    gp.CloseAllFigures();

                                    //tmp try...catch
                                    try
                                    {
                                        using (TextureBrush tb = new TextureBrush(bTmp))
                                            gx.FillPath(tb, gp);

                                        //using (Pen pen = new Pen(Color.Red, 2))
                                        //    gx.DrawPath(pen, gp);
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                            }
                        }
                    }

                    //now redraw the inner outlines and "transparent" components
                    //we can do this in the easy way with setting graphics.CompositionMode to sourceCopy
                    //because the whole operations are full_pixel_wise (at least for the standard 4-connectivity)
                    if (dontFillPath && c.Count > 0)
                    {
                        int amnt = (!drawNumComp) ? c.Count : Math.Min(comp, c.Count);

                        for (int i = 0; i < amnt; i++)
                        {
                            ChainCode cc = c[i];
                            if (ChainFinder.IsInnerOutline(cc))
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    try
                                    {
                                        gP.StartFigure();
                                        PointF[] pts = cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray();
                                        gP.AddLines(pts);

                                        using (Graphics gx = Graphics.FromImage(bRes))
                                        {
                                            gx.CompositingMode = CompositingMode.SourceCopy;
                                            gx.FillPath(Brushes.Transparent, gP);
                                        }
                                    }
                                    catch (Exception exc)
                                    {
                                        Console.WriteLine(exc.ToString());
                                    }
                                }
                        }
                    }

                    //get a backup - for later use - of this bmp with the transparent paths
                    this.SetBitmap(ref this._bResCopyTransp, ref bCTransp);

                    bTmp.Dispose();

                    if (resPic > 1)
                    {
                        Bitmap bOld = bRes;

                        bRes = ResampleUp(bRes, resPic, bU2, dontFillPath, false);
                        Bitmap? bRCopy = ResampleUp(this._bResCopy, resPic, bU2, dontFillPath, false);
                        Bitmap? bRCopy2 = ResampleUp(this._bResCopyTransp, resPic, bU2, true, true);

                        if (bRCopy != null && bRCopy2 != null)
                        {
                            this.SetBitmap(ref this._bResCopy, ref bRCopy);
                            this.SetBitmap(ref this._bResCopyTransp, ref bRCopy2);
                        }
                        //bU2.Dispose();

                        if (bOld != null)
                            bOld.Dispose();
                    }
                    else //if (resPic == 1)
                    {
                        //set the list of all found paths [chains] to re_use later
                        List<ChainCode>? allChains = GetBoundary(this._bResCopyTransp, 0, false);
                        this._allChains = allChains;
                        if (this._removedChains != null)
                            this._removedChains.Clear();

                        if (allChains != null && allChains.Count > 0)
                        {
                            int area = allChains.Sum(a => a.Area);
                            int pxls = w * h;

                            int fc = pxls / area;

                            //if we have almost no output, maybe the initialization of the Gmms hasn't been good enough to receive a reasonable result
                            //so restart with some different KMeans initialization, if wanted
                            if (fc > 1000)
                                //if (MessageBox.Show("Amount pixels to segmented area ratio is " + fc.ToString() + ". " +
                                //    "Rerun with different Initialization of the Gmms?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                                //{
                                this._restartDiffInit = true;
                            //}
                        }
                    }

                    //reset the scribbles, if resized
                    if (scribbleMode && resPic > 1)
                    {
                        if (this._scribblesBU != null)
                            this._scribbles = this._scribblesBU;
                        if (this._scribbleSeqBU != null)
                            this._scribbleSeq = this._scribbleSeqBU;
                    }

                    //our result pic
                    e.Result = bRes;

                    this._finishedPart1 = true;
                }
            }
        }

        private void bgwDoAll1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
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

        private void bgwDoAll1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            //if we have a result pic, display it
            if (e.Result != null)
            {
                Bitmap b = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                this.SetBitmap(ref this._b4Copy, ref bC);

                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                    (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                _undoOPCache?.Add(b);

                //if (this._resWC > 1)
                //    ResampleAndTranslateBack();

                //if (this._bResCopyTransp != null)
                //{
                //    List<ChainCode> c2 = GetBoundary(this._bResCopyTransp, 0, false);
                //    c2 = c2.OrderByDescending(x => x.Coord.Count).ToList();

                //    //this.numRedrawComponents.Value = this.numRedrawComponents.Maximum = c2.Count;
                //}
            }

            this._pic_changed = true;

            //reset the UI-drawn lists
            if (this._allPoints != null)
                this._allPoints.Clear();
            if (this._pathList != null)
                this._pathList.Clear();
            if (this._points != null)
                this._points.Clear();
            if (this._pointsListSeq != null)
                this._pointsListSeq.Clear();
            if (this._bgRelatedPointsAdded != null)
                this._bgRelatedPointsAdded.Clear();
            this._currentDrawOperation = 0;

            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            //re init the bgw
            this.bgwDoAll1.Dispose();
            this.bgwDoAll1 = new BackgroundWorker();
            this.bgwDoAll1.WorkerReportsProgress = true;
            this.bgwDoAll1.WorkerSupportsCancellation = true;
            this.bgwDoAll1.DoWork += bgwDoAll1_DoWork;
            this.bgwDoAll1.ProgressChanged += bgwDoAll1_ProgressChanged;
            this.bgwDoAll1.RunWorkerCompleted += bgwDoAll1_RunWorkerCompleted;

            if (this._gc != null)
                this._gc.BGW = this.bgwDoAll1;

            if (!this._cancelledOp)
            {
                //do some UI settings
                this.cbQuickEst_CheckedChanged(this.cbQuickEst, new EventArgs());
                this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());

                //if we didnt have a good result and set the auto restart with different KMeans settings
                if (this._restartDiffInit)
                {
                    this._restartDiffInit = false;
                    this._KMeansInitW += 1;
                    this._KMeansInitH += 1;

                    bool bR = this.cbRectMode.Checked;
                    bool bS = this.cbScribbleMode.Checked;

                    this.button4_Click(this.btnReset2, new EventArgs());

                    if (bR)
                        this.cbRectMode.Checked = true;
                    if (bS)
                        this.cbScribbleMode.Checked = true;

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.btnGo_Click(this.btnGo, new EventArgs());
                }

                //do some UI settings
                this.cbRectMode.Checked = false;
                this.cbRectMode.Enabled = false;
                this.cbScribbleMode.Checked = false;
                this.cbScribbleMode.Enabled = false;
                this.label17.Enabled = this.numComponents2.Enabled = false;

                this.numMaxSize.Enabled = this.numGmmComp.Enabled = false;

                this.SetControls(false);
                this.btnDoAll.Text = "Cancel";
                this.btnDoAll.Enabled = true;
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                bool approxCurves = this.cbApproxC.Checked;

                if (this._finishedPart1)
                {
                    if (cbDraw.Checked)
                        cbDraw.Checked = false;

                    Bitmap? bPrev = new Bitmap(this.helplineRulerCtrl2.Bmp);

                    if (approxCurves)
                    {
                        Bitmap? bmp = null;

                        DefaultSmoothenOP dsOP = new DefaultSmoothenOP(this.helplineRulerCtrl2.Bmp, this.helplineRulerCtrl1.Bmp);
                        dsOP.ShowInfo += _gc_ShowInfo;

                        dsOP.BGW = this.backgroundWorker1;
                        dsOP.Init(1.5, 1.5, false, 0, 0, true);

                        dsOP.ComputeOutlineInfo();

                        bmp = dsOP.ComputePic(false, 0.0f, true, 0.5f, false, 1.0f);

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

                        dsOP.ShowInfo -= _gc_ShowInfo;
                    }
                    else
                    {
                        if (bPrev != null)
                            bPrev.Dispose();
                        bPrev = null;
                    }

                    OnShowInfo("restoring defects...");

                    //restore
                    double gamma2 = 1.0;
                    float opacity = 1.0f;
                    double wMin = 0.0;
                    double wMax = 180.0;
                    int whFactor = 4;

                    //should already be stopped
                    if (this.bgwDoAll1.IsBusy || this.bgwDoAll3.IsBusy || this.bgwDoAll4.IsBusy || this.bgwDoAll2.IsBusy)
                    {
                        if (this.bgwDoAll1.IsBusy)
                            this.bgwDoAll1.CancelAsync();

                        if (this.bgwDoAll3.IsBusy)
                            this.bgwDoAll3.CancelAsync();

                        if (this.bgwDoAll4.IsBusy)
                            this.bgwDoAll4.CancelAsync();

                        if (this.bgwDoAll2.IsBusy)
                            this.bgwDoAll2.CancelAsync();

                        Cleanup();

                        return;
                    }

                    if (bPrev != null)
                        this.bgwDoAll2.RunWorkerAsync(new object[] { this.helplineRulerCtrl2.Bmp, bPrev, gamma2, opacity, wMin, wMax, whFactor });
                    else
                        DoAlphaMatte((int)this.numBoundOuter.Value, (int)this.numBoundInner.Value, this.cbUnknownAuto.Checked);
                }
            }
        }

        private void Cleanup()
        {
            this._cancelledOp = true;

            this.btnDoAll.Text = "DoAll";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this.btnOK.Enabled = this.btnCancel.Enabled = true;

            this._pic_changed = true;

            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            this.Timer3.Start();
        }

        private void FrmAvoidAGrabCutEasy_ShowInfo(object? sender, string e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => this.toolStripStatusLabel4.Text = e));
            else
                this.toolStripStatusLabel4.Text = e;
        }

        private void _cfop_ShowInfo(object? sender, string e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    if (!e.StartsWith("outer pic-") && !e.StartsWith("pic "))
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
                if (!e.StartsWith("outer pic-") && !e.StartsWith("pic "))
                    this.toolStripStatusLabel4.Text = e;
                if (e.StartsWith("pic "))
                    this.toolStripStatusLabel1.Text = e;
                //if (e.StartsWith("outer pic-amount"))
                //    this.label13.Text = e;
                if (e.StartsWith("picOuter "))
                    this.toolStripStatusLabel1.Text = e;
            }
        }

        private void Cfop_UpdateProgress(object? sender, GetAlphaMatte.ProgressEventArgs e)
        {
            this.bgwDoAll3.ReportProgress((int)(e.CurrentProgress / e.ImgWidthHeight * 100));
        }

        private void bgwDoAll2_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap bWork = new Bitmap((Bitmap)o[0]);
                Bitmap? bPrevious = (Bitmap)o[1];
                double gamma = (double)o[2];
                float opacity = (float)o[3];
                double wMin = (double)o[4];
                double wMax = (double)o[5];
                int whFactor = (int)o[6];

                RevisitConvexDefects(bPrevious, bWork, gamma, opacity, wMax, whFactor, wMin);

                if (bPrevious != null)
                    bPrevious.Dispose();
                bPrevious = null;

                e.Result = bWork;
            }
        }

        private void bgwDoAll2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Bitmap? bmp = null;

            if (e.Result != null)
                bmp = (Bitmap)e.Result;

            if (bmp != null)
            {
                this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                this.SetBitmap(ref this._b4Copy, ref bC);

                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl2.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                    (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                _undoOPCache?.Add(bmp);
            }

            this._pic_changed = true;

            this.helplineRulerCtrl2.dbPanel1.Invalidate();

            //if (this.Timer3.Enabled)
            //    this.Timer3.Stop();

            //this.Timer3.Start();

            this.bgwDoAll2.Dispose();
            this.bgwDoAll2 = new BackgroundWorker();
            this.bgwDoAll2.WorkerReportsProgress = true;
            this.bgwDoAll2.WorkerSupportsCancellation = true;
            this.bgwDoAll2.DoWork += bgwDoAll2_DoWork;
            //this.bgwDoAll2.ProgressChanged += bgwDoAll2_ProgressChanged;
            this.bgwDoAll2.RunWorkerCompleted += bgwDoAll2_RunWorkerCompleted;

            //closedFormMatte Matte
            this.toolStripProgressBar1.Value = 0;
            this.toolStripProgressBar1.Visible = true;

            this.btnDoAll.Text = "Cancel";
            this.btnDoAll.Enabled = true;

            this.btnOK.Enabled = this.btnCancel.Enabled = false;

            if (this.helplineRulerCtrl1.Bmp != null && !this._cancelledOp)
                DoAlphaMatte((int)this.numBoundOuter.Value, (int)this.numBoundInner.Value, this.cbUnknownAuto.Checked);
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

            List<ChainCode>? lInner = GetBoundary(bOut);

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
                                using (Graphics gx = Graphics.FromImage(bOut))
                                {
                                    gx.SmoothingMode = SmoothingMode.None;
                                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    gx.FillPath(Brushes.White, gp);
                                    using (Pen p = new Pen(Color.White, 1))
                                        gx.DrawPath(p, gp);
                                }
                            }
                        }
                    }
                }
            }

            if (dontFill)
                for (int i = 0; i < lInner?.Count; i++)
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

        private Bitmap ExtendOutlineEx(Bitmap bmp, int outerW, bool dontFill, bool drawPath)
        {
            Bitmap bOut = new Bitmap(bmp.Width, bmp.Height);

            List<ChainCode>? lInner = GetBoundary(bmp);

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
                                using (Graphics gx = Graphics.FromImage(bOut))
                                {
                                    gx.SmoothingMode = SmoothingMode.None;
                                    gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    gx.FillPath(Brushes.Gray, gp);
                                    gx.DrawPath(Pens.Gray, gp);

                                    if (drawPath && outerW > 0)
                                    {
                                        try
                                        {
                                            using (Pen pen = new Pen(Color.Gray, outerW))
                                            {
                                                pen.LineJoin = LineJoin.Round;
                                                gp.Widen(pen);
                                                gx.DrawPath(pen, gp);
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
                }

            return bOut;
        }

        public Bitmap? RemOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
            {
                Bitmap? b = null;
                BitArray? fbits = null;

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

                        fbits = null;
                    }

                    return b;
                }
                catch
                {
                    if (fbits != null)
                        fbits = null;
                    if (b != null)
                        b.Dispose();

                    b = null;
                }
            }

            return null;
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

        private unsafe Bitmap? GetAlphaBoundsPic(Bitmap bmpIn, Bitmap bmpAlpha, bool procOrig)
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

                        if (procOrig || (!procOrig && pIn[3] > 0))
                            p[3] = pA[0];
                        //  maybe use
                        //  p[3] = (byte)Math.Max(Math.Min(255.0 * Math.Pow((double)pA[3] / 255.0, gamma), 255), 0);

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

        private unsafe Bitmap? GetAlphaBoundsPic(Bitmap bmpAlpha, double gamma)
        {
            Bitmap? bmp = null;

            if (AvailMem.AvailMem.checkAvailRam(bmpAlpha.Width * bmpAlpha.Height * 16L))
            {
                int w = bmpAlpha.Width;
                int h = bmpAlpha.Height;

                bmp = new Bitmap(w, h);

                BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmA = bmpAlpha.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmD.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmD.Scan0;
                    p += y * stride;

                    byte* pA = (byte*)bmA.Scan0;
                    pA += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        p[0] = pA[0];
                        p[1] = pA[1];
                        p[2] = pA[2];

                        p[3] = (byte)Math.Max(Math.Min(255.0 * Math.Pow((double)pA[3] / 255.0, gamma), 255), 0);

                        p += 4;
                        pA += 4;
                    }
                });

                bmp.UnlockBits(bmD);
                bmpAlpha.UnlockBits(bmA);
            }

            return bmp;
        }

        private void DoAlphaMatte(int oW, int iW, bool auto)
        {
            if (this.helplineRulerCtrl1.Bmp != null && !this._cancelledOp)
            {
                double d = CheckWidthHeight(this.helplineRulerCtrl1.Bmp, true, (double)this.numMaxSize.Value);

                if (auto)
                {
                    iW = 5;
                    oW = 5;

                    if (d > 1)
                    {
                        iW = (int)(5 * d * 2);
                        oW = (int)(5 * d * 2);
                    }
                }

                int innerW = iW;
                int outerW = oW;

                bool editTrimap = this.cbEditTrimap.Checked;
                Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);

                double res = CheckWidthHeight(bWork, true, (double)this.numMaxSize.Value);
                this.toolStripStatusLabel1.Text = "resFactor: " + Math.Max(res, 1).ToString("N2");

                if (res > 1)
                {
                    Bitmap? bOld2 = bWork;
                    bWork = ResampleDown(bWork, res);
                    if (bOld2 != null)
                    {
                        bOld2.Dispose();
                        bOld2 = null;
                    }
                }

                Bitmap bWork2 = ResampleBmp(bWork, 2);

                Bitmap? bOld = bWork;
                bWork = bWork2;
                bOld.Dispose();
                bOld = null;

                double factor = 2.0;
                factor *= (res > 1) ? res : 1.0;

                innerW = (int)Math.Max(Math.Ceiling(innerW / factor), 1);
                outerW = (int)Math.Max(Math.Ceiling(outerW / factor), 1);

                //Bitmap bTrimap = new Bitmap(bWork.Width, bWork.Height);

                //Bitmap? bW = new Bitmap(this.helplineRulerCtrl2.Bmp);
                //Bitmap? bOld4 = bW;
                //GetOpaqueParts(bW);
                //bW = ResampleDown(bW, factor);
                //if (bOld4 != null)
                //{
                //    bOld4.Dispose();
                //    bOld4 = null;
                //}

                //using (Bitmap bForeground = RemoveOutlineEx(bW, innerW, true))
                //using (Bitmap bUnknown = ExtendOutlineEx(bW, outerW, true, true))
                //{
                //    using (Graphics gx = Graphics.FromImage(bTrimap))
                //    {
                //        gx.SmoothingMode = SmoothingMode.None;
                //        gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                //        gx.Clear(Color.Black);
                //        gx.DrawImage(bUnknown, 0, 0);
                //        gx.DrawImage(bForeground, 0, 0);
                //    }
                //}

                bool redrawInner = this.cbRedrawInner.Checked;

                Bitmap bTrimap = new Bitmap(bWork.Width, bWork.Height);

                Bitmap? bW = new Bitmap(this.helplineRulerCtrl2.Bmp);

                GetOpaqueParts(bW);
                Bitmap? bOld4 = bW;
                bW = ResampleDown(bW, factor);

                if (bOld4 != null)
                {
                    bOld4.Dispose();
                    bOld4 = null;
                }

                if (redrawInner)
                {
                    Bitmap? bTrimapTmp = new Bitmap(bTrimap.Width, bTrimap.Height);
                    using (Bitmap bForeground = RemoveOutlineEx(bW, innerW, true))
                    using (Bitmap bUnknown = ExtendOutlineEx(bW, outerW, true, true))
                    {
                        using (Graphics gx = Graphics.FromImage(bTrimapTmp))
                        {
                            gx.SmoothingMode = SmoothingMode.None;
                            gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            gx.Clear(Color.Black);
                            gx.DrawImage(bUnknown, 0, 0);
                            gx.DrawImage(bForeground, 0, 0);
                        }

                        int tolerance = 95;
                        EdgeDetectionMethods.ReplaceColors(bTrimapTmp, 0, 0, 0, 0, tolerance, 255, 0, 0, 0);

                        //
                        List<ChainCode>? c = GetBoundary(bTrimapTmp);
                        ChainFinder cf = new ChainFinder();
                        cf.AllowNullCells = true;

                        if (c != null)
                        {
                            c = c.OrderByDescending(a => a.Coord.Count).ToList();

                            List<List<Point>> points = new List<List<Point>>();

                            foreach (ChainCode cc in c)
                            {
                                bool isInner = ChainFinder.IsInnerOutline(cc);

                                if (isInner)
                                {
                                    using (GraphicsPath gP = new GraphicsPath())
                                    {
                                        gP.StartFigure();
                                        gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());
                                        gP.CloseFigure();

                                        using (Graphics gx = Graphics.FromImage(bTrimapTmp))
                                        {
                                            using (Pen pen = new Pen(Color.Gray, innerW + outerW))
                                                gx.DrawPath(pen, gP);
                                        }
                                    }
                                }
                            }

                            foreach (List<Point> pts in points)
                            {
                                using (GraphicsPath gP = new GraphicsPath())
                                {
                                    gP.StartFigure();
                                    gP.AddLines(pts.Select(a => new PointF(a.X, a.Y)).ToArray());
                                    gP.CloseFigure();

                                    using (Graphics gx = Graphics.FromImage(bTrimapTmp))
                                    {
                                        using (Pen pen = new Pen(Color.Gray, innerW + outerW))
                                            gx.DrawPath(pen, gP);
                                    }
                                }
                            }
                        }
                        //
                    }

                    //Form fff = new Form();
                    //fff.BackgroundImage = bTrimapTmp;
                    //fff.BackgroundImageLayout = ImageLayout.Zoom;
                    //fff.ShowDialog();

                    if (bTrimap != null)
                    {
                        Bitmap? b = bTrimap;
                        bTrimap = new Bitmap(bTrimapTmp.Width, bTrimapTmp.Height);
                        using Graphics gx = Graphics.FromImage(bTrimap);
                        gx.Clear(Color.Black);
                        gx.DrawImage(bTrimapTmp, 0, 0);
                        if (b != null)
                            b.Dispose();
                        b = null;
                    }

                    bTrimapTmp.Dispose();
                    bTrimapTmp = null;
                }
                else
                {
                    using (Bitmap bForeground = RemoveOutlineEx(bW, innerW, true))
                    using (Bitmap bUnknown = ExtendOutlineEx(bW, outerW, true, true))
                    {
                        using (Graphics gx = Graphics.FromImage(bTrimap))
                        {
                            gx.SmoothingMode = SmoothingMode.None;
                            gx.InterpolationMode = InterpolationMode.NearestNeighbor;
                            gx.Clear(Color.Black);
                            gx.DrawImage(bUnknown, 0, 0);
                            gx.DrawImage(bForeground, 0, 0);
                        }
                    }
                }

                bW.Dispose();
                bW = null;

                if (bTrimap != null)
                {
                    Bitmap trWork = bTrimap;

                    if (editTrimap)
                    {
                        using (frmEditTrimap frm = new frmEditTrimap(trWork, bWork, factor))
                        {
                            Bitmap? bmp2 = null;

                            DialogResult dlg = frm.ShowDialog();

                            if (dlg == DialogResult.OK && frm.FBitmap != null)
                            {
                                bmp2 = new Bitmap(frm.FBitmap);

                                Bitmap? bOld2 = trWork;
                                trWork = bmp2;
                                if (bOld2 != null)
                                {
                                    bOld2.Dispose();
                                    bOld2 = null;
                                }
                            }
                            else if (dlg == DialogResult.Cancel)
                            {
                                bTrimap.Dispose();
                                bWork.Dispose();
                                this._cancelledOp = true;
                                Cleanup();
                                this.button4_Click(this.btnReset2, new EventArgs());
                                this.cbRectMode.Checked = true;
                                return;
                            }
                        }
                    }

                    ClosedFormMatteOp cfop = new ClosedFormMatteOp(bWork, trWork);
                    BlendParameters bParam = new BlendParameters();
                    bParam.MaxIterations = 35; //for GaussSeidel set this to 10000 or so
                    bParam.InnerIterations = 35; //maybe 25 will do
                    bParam.DesiredMaxLinearError = 0.01;
                    bParam.Sleep = true;
                    bParam.SleepAmount = 10;
                    bParam.BGW = this.bgwDoAll3;
                    cfop.BlendParameters = bParam;
                    this._cfop = cfop;

                    this._cfop.ShowProgess += Cfop_UpdateProgress;
                    this._cfop.ShowInfo += _cfop_ShowInfo;

                    //bool scalesPics = false;
                    //int scales = 0;
                    //int overlap = 32;
                    //bool interpolated = false;
                    //bool forceSerial = false;
                    //bool group = false;
                    //int groupAmountX = 0;
                    //int groupAmountY = 0;
                    //int maxSize = bWork.Width * bWork.Height;
                    //bool trySingleTile = false;
                    //bool verifyTrimaps = false;

                    bool scalesPics = true;
                    int scales = this.rb4.Checked ? 4 : 16;
                    int overlap = 32;
                    bool interpolated = false;
                    bool forceSerial = true;
                    bool group = false;
                    int groupAmountX = 1; //we dont use grouping, so set it simply to 1
                    int groupAmountY = 1;
                    int maxSize = bWork.Width * bWork.Height;
                    bool trySingleTile = false;
                    bool verifyTrimaps = false;

                    //should already be stopped
                    if (this.bgwDoAll1.IsBusy || this.bgwDoAll3.IsBusy || this.bgwDoAll4.IsBusy || this.bgwDoAll2.IsBusy)
                    {
                        if (this.bgwDoAll1.IsBusy)
                            this.bgwDoAll1.CancelAsync();

                        if (this.bgwDoAll3.IsBusy)
                            this.bgwDoAll3.CancelAsync();

                        if (this.bgwDoAll4.IsBusy)
                            this.bgwDoAll4.CancelAsync();

                        if (this.bgwDoAll2.IsBusy)
                            this.bgwDoAll2.CancelAsync();

                        Cleanup();

                        return;
                    }

                    Bitmap? tr = new Bitmap(trWork);
                    Bitmap? bWrk = new Bitmap(bWork);
                    if (tr != null && bWrk != null)
                    {
                        this.SetBitmap(ref this._bmpWork, ref bWrk);
                        this.SetBitmap(ref this._bmpTrimap, ref tr);
                    }

                    this.bgwDoAll3.RunWorkerAsync(new object[] { 1 /* GMRES_r; 0 is GaussSeidel */, scalesPics, scales, overlap,
                                interpolated, forceSerial, group, groupAmountX, groupAmountY, maxSize, bWork, trWork,
                                trySingleTile, verifyTrimaps });
                }
            }
        }

        private unsafe void GetOpaqueParts(Bitmap bW)
        {
            int w = bW.Width;
            int h = bW.Height;

            BitmapData bmD = bW.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] > 0 && p[3] < 255)
                        p[3] = 0;
                    p += 4;
                }
            });

            bW.UnlockBits(bmD);
        }

        //what we are doing here is to gather all angles that are smaller than a given value, because the "defects" are
        //visible after feathering at angles in the outline that are small. Then we simply get a small Bitmap from the
        //input image around that point, prcess this a bit and draw this in the result image. 
        private void RevisitConvexDefects(Bitmap bPrevious, Bitmap bmp, double gamma = 1.0, float opacity = 1.0f, double wMax = 180.0, int whFactor = 4, double wMin = 0)
        {
            //Get all connected components
            List<ChainCode>? c = GetBoundary(bPrevious);
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;

            if (c != null)
            {
                c = c.OrderByDescending(a => a.Coord.Count).ToList();

                //if (this._rectsList == null)
                //    this._rectsList = new List<List<Rectangle>>();
                //this._rectsList.Clear();

                foreach (ChainCode cc in c)
                {
                    bool isInner = ChainFinder.IsInnerOutline(cc);
                    List<Point> pts = cc.Coord;

                    List<Rectangle> rects = new List<Rectangle>();

                    //now use a bit of calculus and simple linear algebra
                    if (pts.Count > 4)
                    {
                        //make sure each point under consideration has not too close neighbors, since we have a chaincode detector in 4-adjacency
                        pts = cf.RemoveColinearity(pts, true, 4);
                        pts = cf.ApproximateLines(pts, 1.5);
                        pts = cf.RemoveColinearity(pts, true, 4);

                        cc.Coord = pts;
                        List<double> angles = new List<double>();
                        List<Point> distB = new List<Point>();
                        List<Point> distA = new List<Point>();

                        //get all angles as radians
                        for (int j = 0; j < pts.Count; j++)
                        {
                            PointF pt = pts[j];
                            PointF ptB = pts[(pts.Count + (j - 1)) % pts.Count];
                            PointF ptA = pts[(j + 1) % pts.Count];

                            if ((pt.X == ptB.X && pt.Y == ptB.Y) || ((pt.X == ptA.X && pt.Y == ptA.Y)))
                            {
                                angles.Add(0);
                                distB.Add(new Point(0, 0));
                                distA.Add(new Point(0, 0));
                                continue;
                            }

                            double distBX = pt.X - ptB.X;
                            double distBY = pt.Y - ptB.Y;

                            distB.Add(new Point((int)distBX, (int)distBY));

                            double dB = Math.Sqrt(distBX * distBX + distBY * distBY);

                            distBX /= dB;
                            distBY /= dB;

                            double aB = Math.Atan2(distBY, distBX);

                            double distAX = ptA.X - pt.X;
                            double distAY = ptA.Y - pt.Y;

                            distA.Add(new Point((int)distAX, (int)distAY));

                            double dA = Math.Sqrt(distAX * distAX + distAY * distAY);

                            distAX /= dA;
                            distAY /= dA;

                            double aA = Math.Atan2(distAY, distAX);

                            double a = aA - aB;

                            angles.Add(a);
                        }

                        List<Tuple<int, double>> d = new List<Tuple<int, double>>();
                        List<Tuple<int, Point, Point>> dp = new List<Tuple<int, Point, Point>>();

                        //convert angles to degrees and check, if they should be added
                        for (int j = 0; j < angles.Count; j++)
                        {
                            double w = angles[j] * 180.0 / Math.PI;
                            if (w < 0)
                                w += 360;
                            if (w > 360)
                                w -= 360;

                            double wMn = Math.Min(wMin, wMax);
                            double wMa = Math.Max(wMin, wMax);

                            if (w >= wMn && w < wMa)
                            {
                                d.Add(Tuple.Create(j, w));
                                dp.Add(Tuple.Create(j, distB[j], distA[j]));
                            }
                        }

                        //List<List<double[]>> l = GetAlphaTables(bmp, pts, d, dp);

                        //Instead of using a lot of math and arrays for alpha transitions to get the needed values, we
                        //use a trick and overdraw the parts around the angels with a circular_alpha_gradient_picture from the image
                        //passed to this method as input.

                        double dwh = CheckWidthHeight(this.helplineRulerCtrl1.Bmp, true, (double)this.numMaxSize.Value);
                        int iW = 5;
                        int oW = 5;

                        if (dwh > 1)
                        {
                            iW = (int)(5 * dwh * 2);
                            oW = (int)(5 * dwh * 2);
                        }

                        //now get a List of all Bitmaps to be used in this ChainCode, so we can draw all images (for the current ChainCode) in one pass
                        List<Tuple<Point, Bitmap>> bmps = new List<Tuple<Point, Bitmap>>();

                        //Size of bitmap
                        int wh = (oW + iW) * whFactor + 1;
                        if ((wh & 0x01) != 1)
                            wh++;
                        int wh2 = wh / 2;

                        for (int j = 0; j < dp.Count; j++)
                        {
                            Point pt = pts[dp[j].Item1];
                            int sx = Math.Max(pt.X - wh2, 0);
                            int sy = Math.Max(pt.Y - wh2, 0);
                            int ex = Math.Min(pt.X + wh2, bmp.Width);
                            int ey = Math.Min(pt.Y + wh2, bmp.Height);

                            //Get the picture
                            Bitmap b = bPrevious.Clone(new Rectangle(sx, sy, Math.Max(ex - sx, 1), Math.Max(ey - sy, 1)), PixelFormat.Format32bppArgb);
                            //Now get the alphaGradient and add the point_to_draw and the bitmap to the list
                            GetCircularAlphaGradient(b, gamma);

                            bmps.Add(Tuple.Create(new Point(sx, sy), b));
                        }

                        //draw out all bmps for this ChainCode
                        using (Graphics gx = Graphics.FromImage(bmp))
                        {
                            for (int j = 0; j < bmps.Count; j++)
                            {
                                if (opacity == 1.0f)
                                {
                                    //gx.FillRectangle(Brushes.Red, new Rectangle(bmps[j].Item1, new Size(wh, wh)));
                                    gx.DrawImage(bmps[j].Item2, bmps[j].Item1);
                                    rects.Add(new Rectangle(bmps[j].Item1, new Size(wh, wh)));
                                }
                                else
                                {
                                    ColorMatrix cm = new ColorMatrix();
                                    cm.Matrix33 = opacity;

                                    //gx.FillRectangle(Brushes.Red, new Rectangle(bmps[j].Item1, new Size(wh, wh)));

                                    using (ImageAttributes ia = new ImageAttributes())
                                    {
                                        ia.SetColorMatrix(cm);
                                        gx.DrawImage(bmps[j].Item2,
                                            new Rectangle(bmps[j].Item1, new Size(wh, wh)),
                                                0, 0, wh, wh, GraphicsUnit.Pixel, ia);
                                    }

                                    rects.Add(new Rectangle(bmps[j].Item1, new Size(wh, wh)));
                                }
                            }
                        }

                        //cleanup
                        for (int j = bmps.Count - 1; j >= 0; j--)
                        {
                            Bitmap? b = bmps[j].Item2;
                            bmps.RemoveAt(j);
                            b.Dispose();
                            b = null;
                        }
                    }

                    //this._rectsList.Add(rects);
                }
            }
            //#############################################################################
        }

        //This method is copied and telerik-translated from my PictureTextDesigner program, which is partly written in VB
        //so this still uses Marshal.Copy to get the Bitmap_Data, instead of using byte-pointers. This will be changed in the next days.
        private unsafe void GetCircularAlphaGradient(Bitmap bmp, double gamma = 1.0)
        {
            AlphaGradientMode gm = AlphaGradientMode.Elliptic;
            int valueFrom = 255;
            int valueTo = 0;

            BitmapData? bmData = null;

            try
            {
                if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 4L))
                {
                    Point mid = new Point(bmp.Width / 2, bmp.Height / 2);

                    bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

                    int scanline = bmData.Stride;
                    System.IntPtr Scan0 = bmData.Scan0;

                    int nWidth = bmp.Width;
                    int nHeight = bmp.Height;

                    byte[]? p = new byte[(bmData.Stride * bmData.Height) - 1 + 1];
                    Marshal.Copy(bmData.Scan0, p, 0, p.Length);

                    //to get some infos, how the ellipse relatedd values are computed, see Wikipedia.
                    //At least, in the german Wiki, there is a good description of using NumericalExcentricity
                    //and ellipse radiae, see: https://de.wikipedia.org/wiki/Ellipse#Formelsammlung_(Ellipsengleichungen)
                    if (gm.ToString().Contains("Elliptic") || gm.ToString().Contains("Irrsinn"))
                    {
                        double dist = valueFrom - valueTo;

                        if (bmp.Width > bmp.Height)
                        {
                            int l = mid.X;
                            int s = mid.Y;
                            double numEx = Math.Sqrt((l * l) - (s * s)) / l;

                            for (int y = 0; y <= nHeight - 1; y++)
                            {
                                for (int x = 0; x <= nWidth - 1; x++)
                                {
                                    switch (gm)
                                    {
                                        case AlphaGradientMode.Elliptic:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2(trueY, trueX);
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));

                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueFrom - (Math.Pow(radius / radiusMax, gamma) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.Elliptic2:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueY), (trueX));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueFrom - ((radius / radiusMax > 0.5 ? (Math.Pow(radius / radiusMax, gamma) - 0.5) * 2.0 : 0.0) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.EllipticRev:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueY), (trueX));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueTo + (Math.Pow(radius / radiusMax, gamma) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.EllipticRev2:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueY), (trueX));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueTo + ((radius / radiusMax > 0.5 ? (Math.Pow(radius / radiusMax, gamma) - 0.5) * 2.0 : 0.0) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.Irrsinn:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueY), (trueX));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = (dist / 255) * ((radius / radiusMax > 0.5 ? (Math.Pow(radius / radiusMax, gamma) - 0.5) * 2.0 : 0.0));

                                                p[x * 4 + y * scanline] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(Math.Abs((255.0 * val) - System.Convert.ToDouble(p[x * 4 + y * scanline]))), 0), 255));
                                                p[x * 4 + y * scanline + 1] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(Math.Abs((255.0 * val) - System.Convert.ToDouble(p[x * 4 + y * scanline + 1]))), 0), 255));
                                                p[x * 4 + y * scanline + 2] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(Math.Abs((255.0 * val) - System.Convert.ToDouble(p[x * 4 + y * scanline + 2]))), 0), 255));
                                                // p(x * 4 + y * scanline + 3) = CByte(Math.Min(Math.Max(CInt(CDbl(val) * CDbl(p(x * 4 + y * scanline + 3)) / 255.0), 0), 255))

                                                break;
                                            }

                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            int l = mid.Y;
                            int s = mid.X;
                            double numEx = Math.Sqrt((l * l) - (s * s)) / l;

                            for (int x = 0; x <= nWidth - 1; x++)
                            {
                                for (int y = 0; y <= nHeight - 1; y++)
                                {
                                    switch (gm)
                                    {
                                        case AlphaGradientMode.Elliptic:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueX), (trueY));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueFrom - (Math.Pow(radius / radiusMax, gamma) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.Elliptic2:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueX), (trueY));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueFrom - ((radius / radiusMax > 0.5 ? (Math.Pow(radius / radiusMax, gamma) - 0.5) * 2.0 : 0.0) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.EllipticRev:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueX), (trueY));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueTo + (Math.Pow(radius / radiusMax, gamma) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.EllipticRev2:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueX), (trueY));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = valueTo + ((radius / radiusMax > 0.5 ? (Math.Pow(radius / radiusMax, gamma) - 0.5) * 2.0 : 0.0) * dist);

                                                p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(val) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                                break;
                                            }

                                        case AlphaGradientMode.Irrsinn:
                                            {
                                                int trueX = x - mid.X;
                                                int trueY = y - mid.Y;
                                                double theta = Math.Atan2((trueX), (trueY));
                                                double xf = mid.X - x;
                                                double yf = mid.Y - y;
                                                double el = Math.Sqrt(1.0 - (numEx * numEx * Math.Cos(theta) * Math.Cos(theta)));
                                                double radiusMax = System.Convert.ToDouble(s) / el;
                                                double radius = Math.Sqrt((xf * xf) + (yf * yf));

                                                double val = (dist / 255) * ((radius / radiusMax > 0.5 ? (Math.Pow(radius / radiusMax, gamma) - 0.5) * 2.0 : 0.0));

                                                p[x * 4 + y * scanline] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(Math.Abs((255.0 * val) - System.Convert.ToDouble(p[x * 4 + y * scanline]))), 0), 255));
                                                p[x * 4 + y * scanline + 1] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(Math.Abs((255.0 * val) - System.Convert.ToDouble(p[x * 4 + y * scanline + 1]))), 0), 255));
                                                p[x * 4 + y * scanline + 2] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(Math.Abs((255.0 * val) - System.Convert.ToDouble(p[x * 4 + y * scanline + 2]))), 0), 255));
                                                // p(x * 4 + y * scanline + 3) = CByte(Math.Min(Math.Max(CInt(CDbl(val) * CDbl(p(x * 4 + y * scanline + 3)) / 255.0), 0), 255))

                                                break;
                                            }

                                        default:
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        double dist = valueFrom - valueTo;

                        for (int y = 0; y <= nHeight - 1; y++)
                        {
                            for (int x = 0; x <= nWidth - 1; x++)
                            {
                                switch (gm)
                                {
                                    case AlphaGradientMode.HorizontalDown:
                                        {
                                            int value = System.Convert.ToInt32(valueFrom - (dist * Math.Pow(System.Convert.ToDouble(x) / System.Convert.ToDouble(nWidth), gamma)));
                                            p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(value) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                            break;
                                        }

                                    case AlphaGradientMode.HorizontalUp:
                                        {
                                            int value = System.Convert.ToInt32(valueTo + (dist * Math.Pow(System.Convert.ToDouble(x) / System.Convert.ToDouble(nWidth), 1.0 / gamma)));
                                            p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(value) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                            break;
                                        }

                                    case AlphaGradientMode.VerticalalDown:
                                        {
                                            int value = System.Convert.ToInt32(valueFrom - (dist * Math.Pow(System.Convert.ToDouble(y) / System.Convert.ToDouble(nHeight), gamma)));
                                            p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(value) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                            break;
                                        }

                                    case AlphaGradientMode.VerticalUp:
                                        {
                                            int value = System.Convert.ToInt32(valueTo + (dist * Math.Pow(System.Convert.ToDouble(y) / System.Convert.ToDouble(nHeight), 1.0 / gamma)));
                                            p[x * 4 + y * scanline + 3] = System.Convert.ToByte(Math.Min(Math.Max(System.Convert.ToInt32(System.Convert.ToDouble(value) * System.Convert.ToDouble(p[x * 4 + y * scanline + 3]) / 255.0), 0), 255));

                                            break;
                                        }

                                    default:
                                        break;
                                }
                            }
                        }
                    }

                    Marshal.Copy(p, 0, bmData.Scan0, p.Length);
                    bmp.UnlockBits(bmData);

                    p = null;
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
            }
        }

        private void bgwDoAll4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && e.Argument != null)
            {
                Bitmap bmp = new Bitmap(this.helplineRulerCtrl2.Bmp);
                double gamma = (double)e.Argument;

                e.Result = GetAlphaBoundsPic(bmp, gamma);
            }
        }

        private void bgwDoAll4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                    bmp = (Bitmap)e.Result;

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                    Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                    this.SetBitmap(ref this._b4Copy, ref bC);

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl2.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    _undoOPCache?.Add(bmp);
                }

                this.btnDoAll.Text = "DoAll";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                btnReset2.Enabled = true;
                this._pic_changed = true;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.Timer3.Start();

                this.bgwDoAll4.Dispose();
                this.bgwDoAll4 = new BackgroundWorker();
                this.bgwDoAll4.WorkerReportsProgress = true;
                this.bgwDoAll4.WorkerSupportsCancellation = true;
                this.bgwDoAll4.DoWork += bgwDoAll4_DoWork;
                //this.bgwDoAll4.ProgressChanged += bgwDoAll4_ProgressChanged;
                this.bgwDoAll4.RunWorkerCompleted += bgwDoAll4_RunWorkerCompleted;

                this.ShowInfo -= FrmAvoidAGrabCutEasy_ShowInfo;

                //this._sw?.Stop();
                //this.Text = "frmProcOutline";
                //if (this._sw != null)
                //    this.Text += "        - ### -        " + TimeSpan.FromMilliseconds(this._sw.ElapsedMilliseconds).ToString();

                //do some UI settings
                this.cbQuickEst_CheckedChanged(this.cbQuickEst, new EventArgs());
                this.cbDraw_CheckedChanged(this.cbDraw, new EventArgs());
                this.cbRectMode.Checked = false;
                this.cbRectMode.Enabled = false;
                this.cbScribbleMode.Checked = false;
                this.cbScribbleMode.Enabled = false;
                this.label17.Enabled = this.numComponents2.Enabled = true;

                this.numMaxSize.Enabled = this.numGmmComp.Enabled = false;

                this.toolStripStatusLabel4.Text = "done";

                if (this.timer1.Enabled)
                    this.timer1.Stop();
                this.timer1.Start();

                this.cbAutoCropFromOrig.Enabled = this.btnCropFromOrig.Enabled = true;
            }
        }

        private void bgwDoAll3_DoWork(object? sender, DoWorkEventArgs e)
        {
            _cfop_ShowInfo(this, "outer pic-amount " + "...");
            OnShowInfo("Creating Matte.......");

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

                //string trimapProblemMessage = "In this configuration at least one trimap does not contain sufficient Information. " +
                //    "Consider running the task again with a larger tileSize or less subtiles.\n\nYou could also rebuild the matte " +
                //    "for selected rectangles by clicking on the \"RescanParts\" button.";
                int id = Environment.TickCount;

                //if (!scalesPics)
                //{
                //    if (!AvailMem.AvailMem.checkAvailRam(bWork.Width * bWork.Height * 20L))
                //        trySingleTile = false;

                //    if (trySingleTile)
                //    {
                //        if (verifyTrimaps)
                //        {
                //            if (!CheckTrimap(trWork))
                //            {
                //                this.Invoke(new Action(() =>
                //                {
                //                    //if (this._frmInfo == null || this._frmInfo.IsDisposed)
                //                    //    this._frmInfo = new frmInfo();
                //                    //this._frmInfo.Show(trimapProblemMessage);
                //                    OnShowInfo(trimapProblemMessage);
                //                }));

                //                //this._trimapProblemInfos.Add(new TrimapProblemInfo(id, 1, 0, 0, trWork.Width, trWork.Height, 0));
                //            }
                //        }

                //        _cfop_ShowInfo(this, "outer pic-amount " + 1.ToString());
                //        this._cfop.GetMattingLaplacian(Math.Pow(10, -7));
                //        Bitmap? b = null;

                //        OnShowInfo("Creating Matte............");

                //        if (this._cfop.BlendParameters != null && this._cfop.BlendParameters.BGW != null && this._cfop.BlendParameters.BGW.WorkerSupportsCancellation && this._cfop.BlendParameters.BGW.CancellationPending)
                //        {
                //            e.Result = null;
                //            return;
                //        }

                //        if (mode == 0)
                //            b = this._cfop.SolveSystemGaussSeidel();
                //        else if (mode == 1)
                //            b = this._cfop.SolveSystemGMRES();

                //        e.Result = b;
                //    }
                //    else
                //    {
                //        Bitmap result = new Bitmap(bWork.Width, bWork.Height);

                //        int wh = bWork.Width * bWork.Height;
                //        int n = 1;

                //        while (wh > maxSize)
                //        {
                //            n += 1;
                //            wh = bWork.Width / n * bWork.Height / n;
                //        }
                //        int n2 = n * n;

                //        int h = bWork.Height / n;
                //        int h2 = bWork.Height - h * (n - 1);

                //        int w = bWork.Width / n;
                //        int w2 = bWork.Width - w * (n - 1);

                //        overlap = Math.Max(overlap, 1);

                //        if (n2 == 1)
                //            overlap = 0;

                //        List<Bitmap> bmp = new List<Bitmap>();
                //        List<Bitmap> bmp2 = new List<Bitmap>();

                //        GetTiles(bWork, trWork, bmp, bmp2, w, w2, h, h2, overlap, n);

                //        OnShowInfo("Creating Matte............");

                //        //if (verifyTrimaps)
                //        //    if (!CheckTrimaps(bmp2, w, h, n, id, overlap))
                //        //        this.Invoke(new Action(() =>
                //        //        {
                //        //            if (this._frmInfo == null || this._frmInfo.IsDisposed)
                //        //                this._frmInfo = new frmInfo();
                //        //            this._frmInfo.Show(trimapProblemMessage);
                //        //        }));

                //        Bitmap[]? bmp4 = new Bitmap[bmp.Count];

                //        this._cfopArray = new ClosedFormMatteOp[bmp.Count];

                //        if (_cfop.BlendParameters != null && AvailMem.AvailMem.checkAvailRam(w * h * bmp.Count * 20L) && !forceSerial)
                //            Parallel.For(0, bmp.Count, i =>
                //            {
                //                _cfop_ShowInfo(this, "pic " + (i + 1).ToString());
                //                ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                //                BlendParameters bParam = new BlendParameters();
                //                bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                //                bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                //                bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                //                bParam.Sleep = _cfop.BlendParameters.Sleep;
                //                bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                //                bParam.BGW = this.backgroundWorker1;
                //                cfop.BlendParameters = bParam;

                //                cfop.ShowProgess += Cfop_UpdateProgress;
                //                cfop.ShowInfo += _cfop_ShowInfo;

                //                this._cfopArray[i] = cfop;

                //                cfop.GetMattingLaplacian(Math.Pow(10, -7));
                //                Bitmap? b = null;

                //                if (mode == 0)
                //                    b = cfop.SolveSystemGaussSeidel();
                //                else if (mode == 1)
                //                    b = cfop.SolveSystemGMRES();

                //                //save and draw out later serially
                //                if (b != null)
                //                    bmp4[i] = b;

                //                cfop.ShowProgess -= Cfop_UpdateProgress;
                //                cfop.ShowInfo -= _cfop_ShowInfo;
                //                cfop.Dispose();
                //            });
                //        else
                //        {
                //            if (_cfop.BlendParameters != null)
                //                for (int i = 0; i < bmp.Count; i++)
                //                {
                //                    _cfop_ShowInfo(this, "pic " + (i + 1).ToString());
                //                    ClosedFormMatteOp cfop = new ClosedFormMatteOp(bmp[i], bmp2[i]);
                //                    BlendParameters bParam = new BlendParameters();
                //                    bParam.MaxIterations = _cfop.BlendParameters.MaxIterations;
                //                    bParam.InnerIterations = _cfop.BlendParameters.InnerIterations;
                //                    bParam.DesiredMaxLinearError = _cfop.BlendParameters.DesiredMaxLinearError;
                //                    bParam.Sleep = _cfop.BlendParameters.Sleep;
                //                    bParam.SleepAmount = _cfop.BlendParameters.SleepAmount;
                //                    bParam.BGW = this.backgroundWorker1;
                //                    cfop.BlendParameters = bParam;

                //                    cfop.ShowProgess += Cfop_UpdateProgress;
                //                    cfop.ShowInfo += _cfop_ShowInfo;

                //                    this._cfopArray[i] = cfop;

                //                    cfop.GetMattingLaplacian(Math.Pow(10, -7));
                //                    Bitmap? b = null;

                //                    if (mode == 0)
                //                        b = cfop.SolveSystemGaussSeidel();
                //                    else if (mode == 1)
                //                        b = cfop.SolveSystemGMRES();

                //                    //save and draw out later serially
                //                    if (b != null)
                //                        bmp4[i] = b;

                //                    cfop.ShowProgess -= Cfop_UpdateProgress;
                //                    cfop.ShowInfo -= _cfop_ShowInfo;
                //                    cfop.Dispose();
                //                }
                //        }

                //        if (this._cfop.BlendParameters != null && this._cfop.BlendParameters.BGW != null && this._cfop.BlendParameters.BGW.WorkerSupportsCancellation && this._cfop.BlendParameters.BGW.CancellationPending)
                //        {
                //            for (int i = bmp.Count - 1; i >= 0; i--)
                //            {
                //                if (bmp[i] != null)
                //                    bmp[i].Dispose();
                //                if (bmp2[i] != null)
                //                    bmp2[i].Dispose();
                //                if (bmp4[i] != null)
                //                    bmp4[i].Dispose();
                //            }
                //            e.Result = null;
                //            return;
                //        }

                //        for (int i = 0; i < bmp.Count; i++)
                //        {
                //            int x = i % n;
                //            int y = i / n;

                //            using (Graphics gx = Graphics.FromImage(result))
                //                gx.DrawImage(bmp4[i], x * w - (x == 0 ? 0 : overlap), y * h - (y == 0 ? 0 : overlap));
                //        }

                //        for (int i = bmp.Count - 1; i >= 0; i--)
                //        {
                //            bmp[i].Dispose();
                //            bmp2[i].Dispose();
                //            bmp4[i].Dispose();
                //        }

                //        e.Result = result;
                //    }
                //}

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
                                //this.Invoke(new Action(() =>
                                //{
                                //    if (this._frmInfo == null || this._frmInfo.IsDisposed)
                                //        this._frmInfo = new frmInfo();
                                //    this._frmInfo.Show(trimapProblemMessage);
                                //}));

                                //this._trimapProblemInfos.Add(new TrimapProblemInfo(id, 1, 0, 0, trWork.Width, trWork.Height, 0));
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

                        //if (verifyTrimaps)
                        //    if (!CheckTrimaps(bmp2, w, h, n, id, overlap))
                        //        this.Invoke(new Action(() =>
                        //        {
                        //            if (this._frmInfo == null || this._frmInfo.IsDisposed)
                        //                this._frmInfo = new frmInfo();
                        //            this._frmInfo.Show(trimapProblemMessage);
                        //        }));

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

                        //if (verifyTrimaps)
                        //    if (!CheckTrimaps(bmp2, www, hhh, xP, id, xAdd2, yAdd2))  //no overlap, we dont have an outerArray and all inner pics resemble the whole pic
                        //        this.Invoke(new Action(() =>
                        //        {
                        //            if (this._frmInfo == null || this._frmInfo.IsDisposed)
                        //                this._frmInfo = new frmInfo();
                        //            this._frmInfo.Show(trimapProblemMessage);
                        //        }));

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

                            //if (verifyTrimaps)
                            //{
                            //    if (!CheckTrimaps(bmp2))
                            //    {
                            //        this.Invoke(new Action(() =>
                            //        {
                            //            if (this._frmInfo == null || this._frmInfo.IsDisposed)
                            //                this._frmInfo = new frmInfo();
                            //            this._frmInfo.Show(trimapProblemMessage);
                            //        }));

                            //        int x = j % n * www;
                            //        int y = j / n * hhh;

                            //        //int x = i % n;
                            //        //x * www - (x == 0 ? 0 : overlap)

                            //        if (x > 0)
                            //            x -= overlap;

                            //        if (y > 0)
                            //            y -= overlap;

                            //        this._trimapProblemInfos.Add(new TrimapProblemInfo(id, j, x, y, bmpF[j].Width, bmpF[j].Height, overlap));
                            //    }
                            //}

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
        }

        private void bgwDoAll3_ProgressChanged(object sender, ProgressChangedEventArgs e)
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

        private void bgwDoAll3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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
                        Bitmap? bmpMatte = new Bitmap(bmp);
                        if (bmpMatte != null)
                            this.SetBitmap(ref this._bmpMatte, ref bmpMatte);

                        GetAlphaMatte.frmEdgePic frm4 = new GetAlphaMatte.frmEdgePic(bmp, this.helplineRulerCtrl1.Bmp.Size);
                        frm4.Text = "Alpha Matte";
                        frm4.ShowDialog();

                        if (this.helplineRulerCtrl1.Bmp != null && bmp != null)
                        {
                            b2 = bmp;
                            bmp = GetAlphaBoundsPic(this.helplineRulerCtrl1.Bmp, bmp, true);
                            b2.Dispose();
                            b2 = null;
                        }

                        //if (this._bmpOrig != null)
                        //{
                        //    b2 = bmp;
                        //    bmp = GetAlphaBoundsPic(this._bmpOrig, bmp);
                        //    b2.Dispose();
                        //    b2 = null;
                        //}
                    }
                }

                if (this.helplineRulerCtrl2.Bmp != null && bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                    Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                    this.SetBitmap(ref this._b4Copy, ref bC);

                    this.toolStripDropDownButton1.Enabled = true;

                    this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                    this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                        (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                    _undoOPCache?.Add(bmp);
                }

                if (this._cfop != null)
                {
                    this._cfop.ShowProgess -= Cfop_UpdateProgress;
                    this._cfop.ShowInfo -= _cfop_ShowInfo;
                    this._cfop.Dispose();
                }

                this._pic_changed = true;

                this.helplineRulerCtrl2.dbPanel1.Invalidate();

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();

                this.bgwDoAll3.Dispose();
                this.bgwDoAll3 = new BackgroundWorker();
                this.bgwDoAll3.WorkerReportsProgress = true;
                this.bgwDoAll3.WorkerSupportsCancellation = true;
                this.bgwDoAll3.DoWork += bgwDoAll3_DoWork;
                //this.bgwDoAll3.ProgressChanged += bgwDoAll3_ProgressChanged;
                this.bgwDoAll3.RunWorkerCompleted += bgwDoAll3_RunWorkerCompleted;

                if (!this._cancelledOp)
                {
                    //should already be stopped
                    if (this.bgwDoAll1.IsBusy || this.bgwDoAll3.IsBusy || this.bgwDoAll4.IsBusy || this.bgwDoAll2.IsBusy)
                    {
                        if (this.bgwDoAll1.IsBusy)
                            this.bgwDoAll1.CancelAsync();

                        if (this.bgwDoAll3.IsBusy)
                            this.bgwDoAll3.CancelAsync();

                        if (this.bgwDoAll4.IsBusy)
                            this.bgwDoAll4.CancelAsync();

                        if (this.bgwDoAll2.IsBusy)
                            this.bgwDoAll2.CancelAsync();

                        Cleanup();

                        return;
                    }

                    //alphaGamma
                    double gamma = 2.0;
                    this.bgwDoAll4.RunWorkerAsync(gamma);
                }
            }
        }

        #endregion DoAll

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            //bmpRef, bmpTrimap, bmpMatte, bmpWork
            using frmPictures frm = new(_b4Copy, _bmpTrimap, _bmpMatte, _bmpWork, this.helplineRulerCtrl1.Bmp);
            frm.ShowDialog();
        }

        private void btnResScribbles_Click(object sender, EventArgs e)
        {
            if (this._scribblesBU != null && this._scribbleSeqBU != null)
            {
                this._scribbles = this._scribblesBU;
                this._scribbleSeq = this._scribbleSeqBU;
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnCMNew_Click(object sender, EventArgs e)
        {
            this._ptSt = null;
        }

        private void btnOutlineOperations_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && this.CachePathAddition != null)
            {
                using (frnOutlineOperations frm = new frnOutlineOperations(this.helplineRulerCtrl2.Bmp, this.CachePathAddition))
                {
                    frm.SetupCache();

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (frm.FBitmap != null)
                        {
                            Bitmap b = new Bitmap(frm.FBitmap);

                            this.SetBitmap(this.helplineRulerCtrl2.Bmp, b, this.helplineRulerCtrl2, "Bmp");

                            Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                            this.SetBitmap(ref this._b4Copy, ref bC);

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

            this.btnRecut.Enabled = this.numComponents2.Enabled = false;
        }

        private void btnDrawSettings_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null)
            {
                using frmDrawOnResultSettings frm = new();

                frm.cbSetPFGToFG.Checked = this._cbSetPFGToFG;
                frm.cbSkipLearn.Checked = this._cbSkipLearnEnabled && this._cbSkipLearnChecked;
                frm.numOpacity.Value = (decimal)this._opacityDraw;

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    this._cbSetPFGToFG = frm.cbSetPFGToFG.Checked;
                    this._cbSkipLearnEnabled = frm.cbSkipLearn.Checked;
                    this._cbSkipLearnChecked = frm.cbSkipLearn.Checked;
                    this._opacityDraw = (float)frm.numOpacity.Value;

                    this.helplineRulerCtrl2.dbPanel1.Invalidate();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Stop();
            try
            {
                this._dontUpdateNumComp = true;
                this.numComponents2.Value = this.numMaxComponents.Value;
                this._dontUpdateNumComp = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }

            if (this.cbAutoCropFromOrig.Checked)
                btnCropFromOrig_Click(this.btnCropFromOrig, new EventArgs());
        }

        private void btnCropFromOrig_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl2.Bmp != null && this._bmpOrig != null)
            {
                Bitmap bmp = new Bitmap(this._bmpOrig);
                SetImageAlpha(bmp, this.helplineRulerCtrl2.Bmp);

                this.SetBitmap(this.helplineRulerCtrl2.Bmp, bmp, this.helplineRulerCtrl2, "Bmp");

                Bitmap bC = new Bitmap(this.helplineRulerCtrl2.Bmp);
                this.SetBitmap(ref this._b4Copy, ref bC);

                //Bitmap bC2 = new Bitmap(this.helplineRulerCtrl1.Bmp);
                //this.SetBitmap(ref this._bmpBU, ref bC2);

                this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                    (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));

                _undoOPCache?.Add(bmp);
            }
        }

        private unsafe void SetImageAlpha(Bitmap bmp, Bitmap bmpRef)
        {
            int w = bmp.Width;
            int h = bmp.Height;

            BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmRead = bmpRef.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;
                byte* pRead = (byte*)bmRead.Scan0;
                pRead += y * stride;

                for (int x = 0; x < w; x++)
                {
                    p[3] = pRead[3];

                    p += 4;
                    pRead += 4;
                }
            });

            bmp.UnlockBits(bmD);
            bmpRef.UnlockBits(bmRead);
        }

        //Note: This is completely experimental, and bound to change a lot, or even might be removed
        private void btnPreBlur_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker4.IsBusy)
            {
                this.backgroundWorker4.CancelAsync();
                return;
            }

            if (!this.backgroundWorker4.IsBusy && this.helplineRulerCtrl1.Bmp != null && this._bmpBU != null)
            {
                if (_frm == null)
                    _frm = new frmPreBlurVals(new Bitmap(this._bmpBU));
                else
                {
                    Image? iOld = _frm.pictureBox1.Image;
                    _frm.pictureBox1.Image = new Bitmap(this._bmpBU);
                    if (iOld != null && !iOld.Equals(this.helplineRulerCtrl1.Bmp))
                    {
                        iOld.Dispose();
                        iOld = null;
                    }
                    _frm.ValsChanged = _frmValsChanged;
                }

                DialogResult dlg = _frm.ShowDialog();
                if (dlg == DialogResult.OK)
                {
                    if (_frm.FBitmap != null && !_frm.ValsChanged)
                    {
                        Bitmap bOrig = new Bitmap(this.helplineRulerCtrl1.Bmp);
                        this.SetBitmap(ref this._bmpOrig, ref bOrig);

                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, _frm.FBitmap, this.helplineRulerCtrl1, "Bmp");

                        this._undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);

                        this._pic_changed = true;

                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        this.CheckRedoButton();

                        this.btnPreBlur.Text = "preBlur";

                        this.cbAutoCropFromOrig.Enabled = this.btnCropFromOrig.Enabled = true;

                        this.btnReset2.Enabled = true;

                        _frmValsChanged = _frm.ValsChanged;

                        return;
                    }

                    SetControls(false);

                    this.btnPreBlur.Text = "Cancel";
                    this.btnPreBlur.Enabled = true;

                    this.toolStripProgressBar1.Value = 0;

                    bool blur = _frm.cbBlur.Checked;
                    bool colors = _frm.cbColors.Checked;
                    bool blurFirst = _frm.rbBefore.Checked;

                    int krnl = (int)_frm.numKernel.Value;
                    int maxVal = (int)_frm.numDistWeight.Value;
                    Point pt = new Point((int)_frm.numValSrc.Value, (int)_frm.numValDst.Value);

                    this._opacity = (float)_frm.numOpacity.Value;

                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    object[] o = { blur, colors, blurFirst, krnl, maxVal, pt };

                    this.backgroundWorker4.RunWorkerAsync(o);
                }
                else
                    _frmValsChanged = _frm.ValsChanged;

                _frmValsChanged = _frm.ValsChanged;
            }
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null && this.helplineRulerCtrl1.Bmp != null && this._bmpBU != null)
            {
                Bitmap bmp = new Bitmap(this._bmpBU);

                object[] o = (object[])e.Argument;

                bool blur = (bool)o[0];
                bool colors = (bool)o[1];
                bool blurFirst = (bool)o[2];

                int krnl = (int)o[3];
                int maxVal = (int)o[4];
                Point pt = (Point)o[5];

                Rectangle r = new Rectangle(0, 0, bmp.Width, bmp.Height);

                if (blur && !colors)
                    DoBlur(bmp, krnl, maxVal);
                if (!blur && colors)
                    DoColors(bmp, pt);
                if (blur && colors)
                {
                    if (blurFirst)
                    {
                        DoBlur(bmp, krnl, maxVal);
                        DoColors(bmp, pt);
                    }
                    else
                    {
                        DoColors(bmp, pt);
                        DoBlur(bmp, krnl, maxVal);
                    }
                }

                e.Result = bmp;
            }
        }

        private void DoBlur(Bitmap bmp, int krnl, int maxVal)
        {
            Convolution conv = new();
            conv.ProgressPlus += Conv_ProgressPlus;
            conv.CancelLoops = false;

            InvGaussGradOp igg = new InvGaussGradOp();
            igg.BGW = this.backgroundWorker1;

            igg.FastZGaussian_Blur_NxN_SigmaAsDistance(bmp, krnl, 0.01, 255, false, false, conv, false, 1E-12, maxVal);
            conv.ProgressPlus -= Conv_ProgressPlus;
        }

        private void DoColors(Bitmap bmp, Point pt)
        {
            byte[] rgb = new byte[256];
            List<Point> p = new();
            p.Add(new Point(0, 0));
            p.Add(pt);
            p.Add(new Point(255, 255));

            CurveSegment cuSgmt = new();
            List<BezierSegment> bz = cuSgmt.CalcBezierSegments(p.ToArray(), 0.5f);
            List<PointF> pts = cuSgmt.GetAllPoints(bz, 256, 0, 255);
            cuSgmt.MapPoints(pts, rgb);

            ColorCurves.fipbmp.GradColors(bmp, rgb, rgb, rgb);
        }

        private void Conv_ProgressPlus(object sender, ConvolutionLib.ProgressEventArgs e)
        {
            this.backgroundWorker4.ReportProgress((int)e.CurrentProgress);
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
                using Bitmap bmp = (Bitmap)e.Result;

                if (bmp != null && this._bmpBU != null)
                {
                    Bitmap bOrig = new Bitmap(this._bmpBU);
                    this.SetBitmap(ref this._bmpOrig, ref bOrig);

                    Bitmap b = new Bitmap(this._bmpBU);
                    using Graphics gx = Graphics.FromImage(b);

                    ColorMatrix cm = new ColorMatrix();
                    cm.Matrix33 = this._opacity;

                    using ImageAttributes ia = new ImageAttributes();

                    ia.SetColorMatrix(cm);
                    gx.DrawImage(bmp,
                                new Rectangle(0, 0, b.Width, b.Height),
                                0,
                                0,
                                b.Width,
                                b.Height, GraphicsUnit.Pixel, ia);

                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                    this._undoOPCache?.Add(b);

                    this._pic_changed = true;

                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    if (_frm != null)
                        _frmValsChanged = false;
                }

                this.SetControls(true);

                this.CheckRedoButton();

                this.btnPreBlur.Text = "preBlur";

                this.cbAutoCropFromOrig.Enabled = this.btnCropFromOrig.Enabled = true;

                if (this.Timer3.Enabled)
                    this.Timer3.Stop();
                this.Timer3.Start();

                this.backgroundWorker4.Dispose();
                this.backgroundWorker4 = new BackgroundWorker();
                this.backgroundWorker4.WorkerReportsProgress = true;
                this.backgroundWorker4.WorkerSupportsCancellation = true;
                this.backgroundWorker4.DoWork += backgroundWorker4_DoWork;
                this.backgroundWorker4.ProgressChanged += backgroundWorker4_ProgressChanged;
                this.backgroundWorker4.RunWorkerCompleted += backgroundWorker4_RunWorkerCompleted;
            }
        }

        private async void btnCmpLMap_Click(object sender, EventArgs e)
        {
            if (this._bmpBU != null)
            {
                this.lblLumMap.Text = "computing...";
                this.btnGo.Enabled = this.btnDoAll.Enabled = false;
                this.btnGo.Refresh();
                this.btnDoAll.Refresh();
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;
                LuminancMapOp lop = new LuminancMapOp();
                lop.ProgressPlus += lop_ProgressPlus;
                using Bitmap bmp = new Bitmap(this._bmpBU);
                float[,]? lMap = await lop.ComputeInvLuminanceMap(bmp);
                this._iggLuminanceMap = lMap;
                lop.ProgressPlus -= lop_ProgressPlus;
                this.btnGo.Enabled = this.btnDoAll.Enabled = true;
                this.lblLumMap.Text = "done";
                this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Maximum;
                this.toolStripProgressBar1.Visible = false;
            }
        }

        private void btnLumMapSettings_Click(object sender, EventArgs e)
        {
            if (this._lmas != null)
            {
                if (this._bmpBU != null && this.CachePathAddition != null)
                {
                    using frmLumMapSettings frm = new(this._bmpBU, this.CachePathAddition);
                    frm.SetupCache();

                    frm.numF1.Value = (decimal)this._lmas.Factor1;
                    frm.numTh.Value = (decimal)this._lmas.Threshold;
                    if (this._lmas.ValsLessThanTh)
                        frm.rbLessThan.Checked = true;
                    else
                        frm.rbGreaterThan.Checked = true;
                    frm.numF2.Value = (decimal)this._lmas.Factor2;
                    frm.numExp1.Value = (decimal)this._lmas.Exponent1;
                    frm.numExp2.Value = (decimal)this._lmas.Exponent2;
                    frm.numThMultiplier.Value = (decimal)-Math.Log10(this._lmas.ThMultiplier);
                    frm.cbAuto.Checked = this._lmas.MultAuto;
                    frm.cbAppSettingsOnly.Checked = true;
                    frm.cbDoSecondMult.Checked = this._lmas.DoSecondMultiplication;
                    frm.cbDoFirstMult.Checked = this._lmas.DoFirstMultiplication;

                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (frm.FBitmap != null && !frm.cbAppSettingsOnly.Checked)
                        {
                            using Bitmap? bmp = new Bitmap(frm.FBitmap);
                            this._iggLuminanceMap2 = LuminancMapOp.ComputeLuminanceMapFromPicSync(bmp);

                            this.cbUseLumMapBasePic.Enabled = true;
                        }

                        float f1 = (float)frm.numF1.Value;
                        double th = (double)frm.numTh.Value;
                        bool ltth = frm.rbLessThan.Checked;
                        float f2 = (float)frm.numF2.Value;
                        double e1 = (double)frm.numExp1.Value;
                        double e2 = (double)frm.numExp2.Value;
                        double m = Math.Pow(10, (double)-frm.numThMultiplier.Value);
                        bool auto = frm.cbAuto.Checked;
                        bool do2nd = frm.cbDoSecondMult.Checked;
                        bool do1st = frm.cbDoFirstMult.Checked;

                        LumMapApplicationSettings lmas = new()
                        {
                            Factor1 = f1,
                            Threshold = th,
                            ValsLessThanTh = ltth,
                            Factor2 = f2,
                            Exponent1 = e1,
                            Exponent2 = e2,
                            ThMultiplier = m,
                            MultAuto = auto,
                            DoSecondMultiplication = do2nd,
                            DoFirstMultiplication = do1st
                        };

                        this._lmas = lmas;
                    }
                }
            }
        }

        private void cbUseLumMapBasePic_CheckedChanged(object sender, EventArgs e)
        {
            this._useLumMapBasePic = this.cbUseLumMapBasePic.Checked;
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if (this._removedChains == null)
                this._removedChains = new List<ChainCode>();

            if (this._allChains != null && this._removedChains != null)
            {
                Bitmap? bOut = null;

                using frmReOrderChains frm = new(this.helplineRulerCtrl1.Bmp, this._allChains, this._removedChains);

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    bOut = new Bitmap(frm.FBitmap);
                    this._allChains = frm.AllChains;
                    this._removedChains = frm.RemovedChains;

                    if (bOut != null)
                    {
                        this.SetBitmap(this.helplineRulerCtrl2.Bmp, bOut, this.helplineRulerCtrl2, "Bmp");

                        _undoOPCache?.Add(bOut);

                        this._pic_changed = true;

                        this.helplineRulerCtrl2.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl2.MakeBitmap(this.helplineRulerCtrl2.Bmp);
                        this.helplineRulerCtrl2.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl2.Bmp.Width * this.helplineRulerCtrl2.Zoom),
                            (int)(this.helplineRulerCtrl2.Bmp.Height * this.helplineRulerCtrl2.Zoom));
                        this.helplineRulerCtrl2.dbPanel1.Invalidate();

                        if (this._allChains != null)
                            this.numComponents2.Value = this.numComponents2.Maximum = this._allChains.Count;
                    }
                }

                this.SetControls(true);

                this.cbRectMode.Enabled = false;
            }
        }

        private void buttonInfo_Click(object sender, EventArgs e)
        {
            using frmInfo frm = new();
            frm.Width += 200;
            frm.Height *= 2;

            StringBuilder info = new();

            info.Append("A normal GrabCut algorithm works with a Markov Random Field and a MinCut Algorithm. ");
            info.Append("Since we dont do a MinCut, we need to pass neighborhood information in a different way to our Algorithm. ");
            info.Append("So we create a Gradient related pic (InvGaussGrad, or Morphological Gradient), and create an inverted Luminance ");
            info.Append("Map from it, which we then multiply with the EstimationMaximized probabilities or the GaussianMixtureModels (\"Likelihood\"). ");
            info.Append("\n");
            info.Append("\n");
            info.Append("So, how to use it?");
            info.Append("\n");
            info.Append("Simply check the useLumMap checkbox, or do it manually by clicking the compInvLMap button.\n");
            info.Append("To change the parameters for multiplying the Map entries with the probabilities coming from the GMM, ");
            info.Append("click onto the \"settings\" button and change the values of the controls in the lower pane (\"Application settings\").\n");
            info.Append("You also could pass a custom edited image to the LumMap creation algorithm. ");
            info.Append("Click onto the \"Image\" Radiobutton in the settings form *and* uncheck the \"do Application settings only\" checkbox. ");
            info.Append("Only then a LuminanceMap will be computed from that picture. Also, this new LumMap will only be used, when you check the ");
            info.Append("\"UseLMBasePic\" checkbox in the main form. ");

            frm.ShowDialog(info.ToString());
        }
    }
}
