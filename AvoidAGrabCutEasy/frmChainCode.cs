using Cache;
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

namespace AvoidAGrabCutEasy
{
    public partial class frmChainCode : Form
    {
        private Bitmap? _bmpBU;
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
        private Bitmap? _chainsBmp;
        private GraphicsPath? _hChain;
        //private int _precisionForRounding = 4;
        //private GraphicsPath? _selectedFigure;
        private int _ix;
        private int _iy;
        private bool _tracking;
        private List<Point>? _eraseList;
        private Dictionary<int, GraphicsPath>? _pathList;
        private Dictionary<int, List<Tuple<List<Point>, int>>>? _allPoints;
        private int _eX2;
        private int _eY2;
        private static int[] CustomColors = new int[] { };
        private List<Point>? _drawList;
        private bool _tracking2;
        private Dictionary<int, GraphicsPath>? _pathList2;
        private Dictionary<int, List<Tuple<List<Point>, int>>>? _allPoints2;

        public List<ChainCode>? SelectedChains { get; private set; }

        public event EventHandler<string>? BoundaryError;

        public frmChainCode(Bitmap bmp, string basePathAddition)
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
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

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
                {
                    this._eraseList.Add(new Point(_ix, _iy));
                    this._tracking = true;
                }
            }

            if (this.cbDraw.Checked && e.Button == MouseButtons.Left)
            {
                if (this._drawList == null)
                    this._drawList = new List<Point>();

                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    this._drawList.Add(new Point(_ix, _iy));
                    this._tracking2 = true;
                }
            }
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

                    if (this._tracking2)
                    {
                        if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                            this._drawList?.Add(new Point(_ix, _iy));
                    }
                }

                if (this.cbErase.Checked || this.cbDraw.Checked)
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
                    this.AddPointsToPath();
                    this.DrawPointsToBitmap();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }

            if (this._tracking2 && e.Button == MouseButtons.Left)
            {
                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    this._drawList?.Add(new Point(_ix, _iy));
                    this.AddPointsToDrawPath();
                    this.DrawPoints2ToBitmap();
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }

            this._tracking = false;
            this._tracking2 = false;
        }

        //maybe consider using lines plus a pen instead of drawing rectangles
        private void DrawPoints2ToBitmap()
        {
            if (this._allPoints2 != null && this.helplineRulerCtrl1.Bmp != null)
            {
                using (Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp))
                {
                    foreach (int j in this._allPoints2.Keys)
                    {
                        Color c = this.label8.BackColor;
                        List<Tuple<List<Point>, int>> ll = this._allPoints2[j];

                        for (int i = 0; i < ll.Count; i++)
                        {
                            bool doRect = ll[i].Item1.Count == 1;

                            if (!doRect)
                            {
                                foreach (Point pt in ll[i].Item1)
                                {
                                    using (SolidBrush sb = new SolidBrush(c))
                                        gx.FillRectangle(sb, new Rectangle(
                                            (int)((int)(pt.X - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                            (int)((int)(pt.Y - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                            (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/),
                                            (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/)));
                                }
                            }
                            else
                            {
                                Point pt = ll[i].Item1[0];
                                using (SolidBrush sb = new SolidBrush(c))
                                    gx.FillRectangle(sb, new Rectangle(
                                        (int)((int)(pt.X - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                        (int)((int)(pt.Y - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                        (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/),
                                        (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/)));
                            }
                        }
                    }
                }

                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();
            }
        }

        private void AddPointsToDrawPath()
        {
            if (this._pathList2 == null)
                this._pathList2 = new Dictionary<int, GraphicsPath>();

            if (!this._pathList2.ContainsKey(1))
                this._pathList2.Add(1, new GraphicsPath());

            if (this._allPoints2 == null)
                this._allPoints2 = new Dictionary<int, List<Tuple<List<Point>, int>>>();

            if (!this._allPoints2.ContainsKey(1))
                this._allPoints2.Add(1, new List<Tuple<List<Point>, int>>());

            if (this._drawList != null)
            {
                GraphicsPath gp = this._pathList2[1];
                using (GraphicsPath gpTmp = new GraphicsPath())
                {
                    PointF[] z = this._drawList.Select(a => new PointF(a.X, a.Y)).ToArray();

                    if (z.Length > 1)
                    {
                        gpTmp.AddLines(z);
                        gp.AddPath((GraphicsPath)gpTmp.Clone(), false);

                        List<Point> ll = new List<Point>();
                        ll.AddRange(this._drawList.ToArray());
                        this._allPoints2[1].Add(Tuple.Create(ll, (int)this.numDraw.Value));
                    }
                    else if (z.Length == 1)
                    {
                        Point pt = this._drawList[0];
                        Rectangle r = new Rectangle(pt.X - (int)this.numDraw.Value / 2,
                            pt.Y - (int)this.numDraw.Value / 2,
                            (int)this.numDraw.Value,
                            (int)this.numDraw.Value);

                        gpTmp.AddRectangle(r);
                        gp.AddPath((GraphicsPath)gpTmp.Clone(), false);

                        List<Point> ll = new List<Point>();
                        ll.AddRange(this._drawList.ToArray());
                        this._allPoints2[1].Add(Tuple.Create(ll, (int)this.numDraw.Value));
                    }
                }
            }

            this._drawList?.Clear();
        }

        private void DrawPointsToBitmap()
        {
            if (this._allPoints != null && this.helplineRulerCtrl1.Bmp != null)
            {
                using (Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp))
                {
                    gx.CompositingMode = CompositingMode.SourceCopy;
                    foreach (int j in this._allPoints.Keys)
                    {
                        Color c = Color.FromArgb(0, 0, 0, 0);
                        List<Tuple<List<Point>, int>> ll = this._allPoints[j];

                        for (int i = 0; i < ll.Count; i++)
                        {
                            bool doRect = ll[i].Item1.Count == 1;

                            if (!doRect)
                            {
                                foreach (Point pt in ll[i].Item1)
                                {
                                    using (SolidBrush sb = new SolidBrush(c))
                                        gx.FillRectangle(sb, new Rectangle(
                                            (int)((int)(pt.X - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                            (int)((int)(pt.Y - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                            (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/),
                                            (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/)));
                                }
                            }
                            else
                            {
                                Point pt = ll[i].Item1[0];
                                using (SolidBrush sb = new SolidBrush(c))
                                    gx.FillRectangle(sb, new Rectangle(
                                        (int)((int)(pt.X - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                        (int)((int)(pt.Y - ll[i].Item2 / 2) /* * this.helplineRulerCtrl1.Zoom*/) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                        (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/),
                                        (int)(ll[i].Item2 /* * this.helplineRulerCtrl1.Zoom*/)));
                            }
                        }
                    }
                }

                _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();
            }
        }

        private void AddPointsToPath()
        {
            if (this._pathList == null)
                this._pathList = new Dictionary<int, GraphicsPath>();

            if (!this._pathList.ContainsKey(-1))
                this._pathList.Add(-1, new GraphicsPath());

            if (this._allPoints == null)
                this._allPoints = new Dictionary<int, List<Tuple<List<Point>, int>>>();

            if (!this._allPoints.ContainsKey(-1))
                this._allPoints.Add(-1, new List<Tuple<List<Point>, int>>());

            if (this._eraseList != null)
            {
                GraphicsPath gp = this._pathList[-1];
                using (GraphicsPath gpTmp = new GraphicsPath())
                {
                    PointF[] z = this._eraseList.Select(a => new PointF(a.X, a.Y)).ToArray();

                    if (z.Length > 1)
                    {
                        gpTmp.AddLines(z);
                        gp.AddPath((GraphicsPath)gpTmp.Clone(), false);

                        List<Point> ll = new List<Point>();
                        ll.AddRange(this._eraseList.ToArray());
                        this._allPoints[-1].Add(Tuple.Create(ll, (int)this.numErase.Value));
                    }
                    else if (z.Length == 1)
                    {
                        Point pt = this._eraseList[0];
                        Rectangle r = new Rectangle(pt.X - (int)this.numErase.Value / 2,
                            pt.Y - (int)this.numErase.Value / 2,
                            (int)this.numErase.Value,
                            (int)this.numErase.Value);

                        gpTmp.AddRectangle(r);
                        gp.AddPath((GraphicsPath)gpTmp.Clone(), false);

                        List<Point> ll = new List<Point>();
                        ll.AddRange(this._eraseList.ToArray());
                        this._allPoints[-1].Add(Tuple.Create(ll, (int)this.numErase.Value));
                    }
                }
            }

            this._eraseList?.Clear();
        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {
            if (this.checkedListBox1.Items.Count > 0 && this._chainsBmp != null && this.checkedListBox1.SelectedItems.Count > 0)
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

            if (this.cbErase.Checked)
            {
                if (this._eraseList != null && this.helplineRulerCtrl1.Bmp != null)
                {
                    e.Graphics.CompositingMode = CompositingMode.SourceCopy;

                    Color c = Color.Lime;
                    List<Point>? ll = this._eraseList;
                    int wh = (int)this.numErase.Value;

                    if (ll != null)
                    {
                        bool doRect = ll.Count == 1;

                        if (!doRect)
                        {
                            foreach (Point pt in ll)
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
                            Point pt = ll[0];
                            using (SolidBrush sb = new SolidBrush(c))
                                e.Graphics.FillRectangle(sb, new Rectangle(
                                    (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                    (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                    (int)(wh * this.helplineRulerCtrl1.Zoom),
                                    (int)(wh * this.helplineRulerCtrl1.Zoom)));
                        }
                    }
                }

                int wh2 = (int)this.numErase.Value;

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(95, 255, 0, 0)))
                    e.Graphics.FillRectangle(sb, new RectangleF(
                        this._eX2 - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                        this._eY2 - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                        (float)(wh2 * this.helplineRulerCtrl1.Zoom),
                        (float)(wh2 * this.helplineRulerCtrl1.Zoom)));
            }

            if (this.cbDraw.Checked)
            {
                if (this._drawList != null && this.helplineRulerCtrl1.Bmp != null)
                {
                    e.Graphics.CompositingMode = CompositingMode.SourceCopy;

                    Color c = this.label8.BackColor;
                    List<Point>? ll = this._drawList;
                    int wh = (int)this.numDraw.Value;

                    if (ll != null)
                    {
                        bool doRect = ll.Count == 1;

                        if (!doRect)
                        {
                            foreach (Point pt in ll)
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
                            Point pt = ll[0];
                            using (SolidBrush sb = new SolidBrush(c))
                                e.Graphics.FillRectangle(sb, new Rectangle(
                                    (int)((int)(pt.X - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                    (int)((int)(pt.Y - wh / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                    (int)(wh * this.helplineRulerCtrl1.Zoom),
                                    (int)(wh * this.helplineRulerCtrl1.Zoom)));
                        }
                    }
                }

                int wh2 = (int)this.numDraw.Value;

                using (SolidBrush sb = new SolidBrush(Color.FromArgb(95, 255, 0, 0)))
                    e.Graphics.FillRectangle(sb, new RectangleF(
                        this._eX2 - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                        this._eY2 - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                        (float)(wh2 * this.helplineRulerCtrl1.Zoom),
                        (float)(wh2 * this.helplineRulerCtrl1.Zoom)));
            }

            //if (this._selectedFigure != null)
            //{
            //    HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl1.dbPanel1;
            //    RectangleF r = _selectedFigure.GetBounds();

            //    if (this.checkedListBox1.CheckedItems.Count > 0)
            //    {
            //        using (GraphicsPath? gP = _selectedFigure.Clone() as GraphicsPath)
            //        {
            //            if (gP != null)
            //            {
            //                using (Matrix mx2 = new Matrix(1, 0, 0, 1, -r.X, -r.Y))
            //                    gP.Transform(mx2);

            //                using (Matrix mx2 = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, 0, 0))
            //                    gP.Transform(mx2);

            //                using (Matrix mx2 = new Matrix(1, 0, 0, 1,
            //                    r.X * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.X,
            //                    r.Y * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.Y))
            //                    gP.Transform(mx2);

            //                using (SolidBrush sb = new SolidBrush(Color.FromArgb(64, Color.Lime)))
            //                    e.Graphics.FillPath(sb, gP);
            //            }
            //        }

            //        using (Pen p = new Pen(Color.Lime, 2))
            //        {
            //            if (this.cbBGColor.Checked)
            //                p.Color = Color.OrangeRed;

            //            e.Graphics.DrawRectangle(p, r.X * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.X,
            //                r.Y * this.helplineRulerCtrl1.Zoom + pz.AutoScrollPosition.Y,
            //                r.Width * this.helplineRulerCtrl1.Zoom, r.Height * this.helplineRulerCtrl1.Zoom);
            //        }
            //    }
            //}
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
                                        b1 = new Bitmap(img);
                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                        b2 = new Bitmap(img);
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

        private void frmChainCode_Load(object sender, EventArgs e)
        {
            foreach (string z in System.Enum.GetNames(typeof(GradientMode)))
                this.cmbGradMode.Items.Add(z.ToString());

            this.cmbGradMode.SelectedIndex = 2;

            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());

            this.cmbZoom.SelectedIndex = 4;
        }

        private void frmChainCode_FormClosing(object sender, FormClosingEventArgs e)
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

                if (this._chainsBmp != null)
                    this._chainsBmp.Dispose();

                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();

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
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmChainCode");
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

        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                SetControls(false);

                int tolerance = (int)this.numReplace.Value;
                EdgeDetectionMethods.ReplaceColors(this.helplineRulerCtrl1.Bmp, 0, 0, 0, 0, tolerance, 255, 0, 0, 0);
                this._undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();

                SetControls(true);
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
                Bitmap bmp = new Bitmap(this._bmpBU);

                object[] o = (object[])e.Argument;

                GradientMode gradientMode = (GradientMode)o[0];
                double divisor = (double)o[1];

                EdgeDetectionMethods edgeDetectionMethods = new();
                edgeDetectionMethods.GetGradMag(bmp, gradientMode, divisor, this.backgroundWorker1);

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
                            _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                            this.btnUndo.Enabled = true;
                            CheckRedoButton();
                            this._pic_changed = true;

                            DrawPointsToBitmap();
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

                    Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
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
                    bmpTmp = new Bitmap(upperImg);
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
            for (int i = 0; i < this.checkedListBox1.Items.Count - 1; i++)
                this.checkedListBox1.SetItemChecked(i, true);

            checkedListBox1_SelectedIndexChanged(this.checkedListBox1, new EventArgs());
        }

        private void btnSelNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.checkedListBox1.Items.Count - 1; i++)
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

        private void btnRemLastScribbles_Click(object sender, EventArgs e)
        {
            this.btnUndo.PerformClick();

            if (this._allPoints != null && this._allPoints.Count > 0)
            {
                if (this._allPoints[-1] != null && this._allPoints[-1].Count > 0)
                    this._allPoints[-1].RemoveAt(this._allPoints[-1].Count - 1);
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

        private void btnRemLastScribbles2_Click(object sender, EventArgs e)
        {
            this.btnUndo.PerformClick();

            if (this._allPoints2 != null && this._allPoints2.Count > 0)
            {
                if (this._allPoints2[1] != null && this._allPoints2[1].Count > 0)
                    this._allPoints2[1].RemoveAt(this._allPoints2[1].Count - 1);
            }
        }
    }
}
