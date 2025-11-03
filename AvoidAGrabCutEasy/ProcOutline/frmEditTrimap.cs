using Cache;
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

namespace AvoidAGrabCutEasy.ProcOutline
{
    public partial class frmEditTrimap : Form
    {
        Bitmap? _bOrig = null;
        private Bitmap _bmpBU;
        private bool _pic_changed;
        private bool _drawImgOverlay;
        private float _overlayOpacity = 0.5f;
        private int _eX2f;
        private int _eY2f;
        private Color[] _colors = new Color[] { Color.Black, Color.White, Color.Gray };
        private bool _tracking;
        private List<Point> _points = new List<Point>();
        private Dictionary<int, GraphicsPath>? _pathList;
        private Dictionary<int, List<Tuple<List<Point>, int>>>? _allPoints;
        private int _currentDraw;
        private List<int>? _lastDraw;
        private bool _dontDoZoom;
        private bool _tracking2;
        private List<Point>? _points2;
        private Dictionary<int, Dictionary<int, List<List<Point>>>>? _scribbles;
        //It would be much easier to use a List of objects which hold all information about drawing instead of using a seperate sequence file...
        //Maybe I'll change it (as far as it is also easy to serialize in json)
        private List<Tuple<int, int, int, bool, List<List<Point>>>> _scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();
        private Point? _ptSt;
        private List<Point>? _ptPrev;

        public Bitmap? FBitmap
        {
            get { return this.helplineRulerCtrl1.Bmp; }
        }

        public frmEditTrimap(Bitmap trWork, Bitmap orig, double factor)
        {
            InitializeComponent();
            _bOrig = orig;

            this._bmpBU = new Bitmap(trWork);

            this.helplineRulerCtrl1.Bmp = new Bitmap(trWork);

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

            this.numWH.Value = (decimal)(int)Math.Max((double)this.numWH.Value / factor, 1);
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                if (this.cbDraw.Checked && !this.cbClickDraw.Checked && ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    this._points.Add(new Point(ix, iy));
                    this._tracking = true;
                }

                if (this.cbDraw.Checked && this.cbClickDraw.Checked && ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this._points2 == null)
                        this._points2 = new List<Point>();
                    if (this._points2.Count == 0)
                        this._points2.Add(new Point(ix, iy));
                    this._tracking2 = true;
                }
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this._tracking)
                    {
                        this._points.Add(new Point(ix, iy));
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }

                    Color c = this.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                    this.toolStripStatusLabel4.Text = ix.ToString() + "; " + iy.ToString();
                    this.toolStripStatusLabel2.BackColor = c;

                }
            }
            //if (this._drawImgOverlay)
            //{
            this._eX2f = e.X;
            this._eY2f = e.Y;
            //}

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this._tracking)
                    {
                        if (this._points[this._points.Count - 1].X != ix || this._points[this._points.Count - 1].Y != iy)
                            this._points.Add(new Point(ix, iy));

                        AddPointsToPath();
                        this._points.Clear();
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }

                    if (this._tracking2)
                    {
                        if (this._points2?.Count == 1 && (this._points2[this._points2.Count - 1].X != ix || this._points2[this._points2.Count - 1].Y != iy))
                            this._points2.Add(new Point(ix, iy));

                        AddPointsToScribblePath();
                        this._points2?.Clear();
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
            }

            this._tracking = this._tracking2 = false;
        }

        private void AddPointsToScribblePath()
        {
            if (this._scribbles != null)
            {
                int fgbg = this._currentDraw; //this.rbFG.Checked ? 1 : this.rbBG.Checked ? 0 : 3;
                int wh = (int)this.numWH.Value;

                if (!this._scribbles.ContainsKey(fgbg))
                    this._scribbles.Add(fgbg, new Dictionary<int, List<List<Point>>>());

                if (!this._scribbles[fgbg].ContainsKey(wh))
                    this._scribbles[fgbg].Add(wh, new List<List<Point>>());

                if (this._scribbleSeq == null)
                    this._scribbleSeq = new List<Tuple<int, int, int, bool, List<List<Point>>>>();

                if (this._points2 != null)
                {
                    Point? ptEnd = null;

                    if (this.cbClickDraw.Checked && this._points2.Count == 1)
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
                }
            }

            if (this._lastDraw == null)
                this._lastDraw = new List<int>();

            this._lastDraw.Add(this._currentDraw);
        }

        private void helplineRulerCtrl1_Paint(object? sender, PaintEventArgs e)
        {
            if (this._allPoints != null)
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
                                        (int)((int)(pt.X - ll[i].Item2 / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                        (int)((int)(pt.Y - ll[i].Item2 / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                        (int)(ll[i].Item2 * this.helplineRulerCtrl1.Zoom),
                                        (int)(ll[i].Item2 * this.helplineRulerCtrl1.Zoom)));
                            }
                        }
                        else
                        {
                            Point pt = ll[i].Item1[0];
                            using (SolidBrush sb = new SolidBrush(c))
                                e.Graphics.FillRectangle(sb, new Rectangle(
                                    (int)((int)(pt.X - ll[i].Item2 / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                    (int)((int)(pt.Y - ll[i].Item2 / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                    (int)(ll[i].Item2 * this.helplineRulerCtrl1.Zoom),
                                    (int)(ll[i].Item2 * this.helplineRulerCtrl1.Zoom)));
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
                                    (int)((int)(pt.X - (int)this.numWH.Value / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                    (int)((int)(pt.Y - (int)this.numWH.Value / 2) * this.helplineRulerCtrl1.Zoom) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                    (int)((int)this.numWH.Value * this.helplineRulerCtrl1.Zoom),
                                    (int)((int)this.numWH.Value * this.helplineRulerCtrl1.Zoom)));
                        }
                    }
                }
            }

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

                            //if (l == 3 && this.cbHighlight.Checked)
                            //    c = Color.Cyan;

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
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1),
                                                (int)Math.Max(wh * this.helplineRulerCtrl1.Zoom, 1)));
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

            if (this._bOrig != null && this._drawImgOverlay)
            {
                HelplineRulerControl.DBPanel pz = this.helplineRulerCtrl1.dbPanel1;

                ColorMatrix cm = new ColorMatrix();
                cm.Matrix33 = this._overlayOpacity;

                using (ImageAttributes ia = new ImageAttributes())
                {
                    ia.SetColorMatrix(cm);
                    e.Graphics.DrawImage(this._bOrig,
                        new Rectangle(0, 0, pz.ClientRectangle.Width, pz.ClientRectangle.Height),
                        -pz.AutoScrollPosition.X / this.helplineRulerCtrl1.Zoom,
                            -pz.AutoScrollPosition.Y / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Width / this.helplineRulerCtrl1.Zoom,
                            pz.ClientRectangle.Height / this.helplineRulerCtrl1.Zoom, GraphicsUnit.Pixel, ia);
                }
            }

            int wh2 = (int)this.numWH.Value;
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(95, 0, 255, 0)))
                e.Graphics.FillRectangle(sb, new RectangleF(
                    this._eX2f - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                    this._eY2f - (float)(wh2 / 2f * this.helplineRulerCtrl1.Zoom) - 1f,
                    (float)(wh2 * this.helplineRulerCtrl1.Zoom),
                    (float)(wh2 * this.helplineRulerCtrl1.Zoom)));
        }

        private void bitmappanel1_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
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
                        if (files != null && files[0] != null)
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

                            //_undoOPCache.Clear(this.helplineRulerCtrl1.Bmp);

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

        private void SetBitmap(ref Bitmap bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                DrawToBmp();

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

        private void DrawToBmp()
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._allPoints != null && this._allPoints.Count > 0)
            {
                Bitmap bmp = new Bitmap(this._bmpBU);
                using (Graphics gx = Graphics.FromImage(bmp))
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
                                        gx.FillRectangle(sb, new Rectangle(
                                            (int)((int)(pt.X - ll[i].Item2 / 2)),
                                            (int)((int)(pt.Y - ll[i].Item2 / 2)),
                                            (int)(ll[i].Item2),
                                            (int)(ll[i].Item2)));
                                }
                            }
                            else
                            {
                                Point pt = ll[i].Item1[0];
                                using (SolidBrush sb = new SolidBrush(c))
                                    gx.FillRectangle(sb, new Rectangle(
                                        (int)((int)(pt.X - ll[i].Item2 / 2)),
                                        (int)((int)(pt.Y - ll[i].Item2 / 2)),
                                        (int)(ll[i].Item2),
                                        (int)(ll[i].Item2)));
                            }
                        }
                    }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");
            }

            if (this.helplineRulerCtrl1.Bmp != null && this._scribbles != null && this._scribbleSeq != null && this._scribbleSeq.Count > 0)
            {
                Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                using (Graphics gx = Graphics.FromImage(bmp))
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

                                //if (l == 3 && this.cbHighlight.Checked)
                                //    c = Color.Cyan;

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

                                        using Matrix mx = new Matrix(1, 0, 0, 1,
                                            this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                            this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                                        gP.Transform(mx);

                                        using SolidBrush sb = new SolidBrush(c);
                                        gx.FillPath(sb, gP);
                                    }
                                    else
                                        foreach (Point pt in ptsList[listNo])
                                        {
                                            using (SolidBrush sb = new SolidBrush(c))
                                                gx.FillRectangle(sb, new Rectangle(
                                                    (int)((int)(pt.X - wh / 2)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                    (int)((int)(pt.Y - wh / 2)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                    (int)Math.Max(wh, 1),
                                                    (int)Math.Max(wh, 1)));
                                        }
                                }
                                else
                                {
                                    if (ptsList[listNo].Count > 0)
                                    {
                                        Point pt = ptsList[listNo][0];
                                        using (SolidBrush sb = new SolidBrush(c))
                                            gx.FillRectangle(sb, new Rectangle(
                                                (int)((int)(pt.X - wh / 2)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                                                (int)((int)(pt.Y - wh / 2)) + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y,
                                                (int)Math.Max(wh, 1),
                                                (int)Math.Max(wh, 1)));
                                    }
                                }
                            }
                        }
                    }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");
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

                        //_undoOPCache.Reset(false);

                        //if (_undoOPCache.Count > 1)
                        //    this.btnRedo.Enabled = true;
                        //else
                        //    this.btnRedo.Enabled = false;
                    }
                    catch
                    {
                        if (b1 != null)
                            b1.Dispose();
                    }
                }
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
        }

        private void cbImgOverlay_CheckedChanged(object sender, EventArgs e)
        {
            this._drawImgOverlay = cbImgOverlay.Checked;
            this._overlayOpacity = (float)this.numOpacity.Value;
            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void frmEditTrimap_Load(object sender, EventArgs e)
        {
            this.cmbCurrentColor.SelectedIndex = 2;
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cbDraw.Checked = true;
            this.cmbZoom.SelectedIndex = 4;
            this.numOpacity.Value = (decimal)this._overlayOpacity;
        }

        private void cmbCurrentColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.label1.BackColor = this._colors[this.cmbCurrentColor.SelectedIndex];
            this._currentDraw = this.cmbCurrentColor.SelectedIndex;
            this._ptSt = null;
        }

        private void btnRemStroke_Click(object sender, EventArgs e)
        {
            if (this._pathList == null)
                this._pathList = new Dictionary<int, GraphicsPath>();

            if (this._allPoints == null)
                this._allPoints = new Dictionary<int, List<Tuple<List<Point>, int>>>();

            if (!this.cbClickDraw.Checked)
            {
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

                    this._pathList[this._lastDraw[this._lastDraw.Count - 1]] = gp;

                    if (this._lastDraw.Count > 0)
                        this._lastDraw.RemoveAt(this._lastDraw.Count - 1);
                }
            }

            if (this.helplineRulerCtrl1.Bmp != null && this.cbClickDraw.Checked && this.cbDraw.Checked)
            {
                this._points2?.Clear();

                int fg = this._currentDraw; // this.rbFG.Checked ? 1 : this.rbBG.Checked ? 0 : 3;
                int wh = (int)this.numWH.Value;

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

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DrawToBmp();
        }

        private void btnLoadBasePic_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK && this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bmp = null;
                using Image img = Image.FromFile(this.openFileDialog1.FileName);
                //if(img.Width >= this.helplineRulerCtrl1.Bmp.Width && img.Height >= this.helplineRulerCtrl1.Bmp.Height)
                if (img.Width == this.helplineRulerCtrl1.Bmp.Width && img.Height == this.helplineRulerCtrl1.Bmp.Height)
                    bmp = new Bitmap(img);
                else
                {
                    bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                    using Graphics gx = Graphics.FromImage(bmp);
                    gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gx.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
                }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");
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

        private void numOpacity_ValueChanged(object sender, EventArgs e)
        {
            this._overlayOpacity = (float)this.numOpacity.Value;
            this.cbImgOverlay_CheckedChanged(this.cbImgOverlay, new EventArgs());
        }

        private void cbClickDraw_CheckedChanged(object sender, EventArgs e)
        {
            this.btnNew.Enabled = this.cbClickDraw.Checked;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            this._ptSt = null;
        }
    }
}
