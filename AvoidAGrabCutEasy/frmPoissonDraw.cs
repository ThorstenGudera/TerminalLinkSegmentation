using Cache;
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

        private bool _dontDrawPath;

        private static int[] CustomColors = new int[] { };
        private bool _dontClose;

        private IBlendAlgorithm? _cAlg;
        private PoissonBlender? _pb;

        private TextureBrush? _tb = null;
        private Point _sourcePt;
        private Point _destPt;
        private bool _drawAll;
        private int _fp;
        private Bitmap? _bmpBUZoomed;
        private int _clicks;

        private Bitmap? _bmpOrg;
        private Bitmap? _bmpDraw;
        private int _ix2;
        private int _iy2;

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

                    toolStripStatusLabel4.Text = "x: " + _ix.ToString() + ", y: " + _iy.ToString() + " - " + "GrayValue (all channels): " + System.Convert.ToInt32(Math.Min(System.Convert.ToDouble(c.B) * 0.11 + System.Convert.ToDouble(c.G) * 0.59 + System.Convert.ToDouble(c.R) * 0.3, 255)).ToString() + " - RGB: " + c.R.ToString() + ";" + c.G.ToString() + ";" + c.B.ToString();

                    this.toolStripStatusLabel3.BackColor = c;

                    if (this.cbDraw.Checked && this._tracking && e.Button == MouseButtons.Left)
                    {
                        SetupCurPath();
                        this.CurPath?.Add(new PointF(this._ix, this._iy));
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
                if (this.CurPath != null && this.cbDraw.Checked && !ContainsPoint(this.CurPath, new PointF(this._ix, this._iy)))
                {
                    SetupCurPath();
                    this.CurPath?.Add(new PointF(this._ix, this._iy));
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

            if (this._bmpBUZoomed != null)
                using (TextureBrush tb = new TextureBrush(this._bmpBUZoomed))
                {
                    tb.WrapMode = WrapMode.TileFlipXY;
                    int dx = this._destPt.X - this._sourcePt.X;
                    int dy = this._destPt.Y - this._sourcePt.Y;
                    tb.TranslateTransform(dx * this.helplineRulerCtrl2.Zoom, dy * this.helplineRulerCtrl2.Zoom);

                    if (!this._dontDrawPath && this.cbDraw.Checked && this._tracking)
                    {
                        using (GraphicsPath gp = GetPath())
                        {
                            float w = (float)this.numPenSize.Value;
                            int x = this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.X;
                            int y = this.helplineRulerCtrl2.dbPanel1.AutoScrollPosition.Y;
                            tb.TranslateTransform(x, y);
                            using (Matrix m = new Matrix(1, 0, 0, 1, x, y))
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
                            }
                        }
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
                        if (this._drawAll)
                        {
                            if (this.Paths != null && this.Paths.Count > 0)
                            {
                                for (int i = 0; i <= this.Paths.Count - 1; i++)
                                {
                                    float w = this.Paths[i].DrawWidth;
                                    this._tb?.ResetTransform();
                                    this._tb?.TranslateTransform(this.Paths[i].Offset.X, this.Paths[i].Offset.Y);

                                    // Me.ComboBox1.SelectedIndex = Me.Paths(i).BVAlg
                                    // Me.NumericUpDown2.Value = CDec(Me.Paths(i).UpperWeight)
                                    // Me.NumericUpDown3.Value = CDec(Me.Paths(i).LowerWeight)

                                    if (this._tb != null)
                                        using (Pen pen = new Pen(this._tb, w))
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
                                                    gx.FillPath(this._tb, gp);
                                                }
                                                else
                                                {
                                                    gp.AddLines(this.Paths[i].ToArray());
                                                    gx.DrawPath(pen, gp);
                                                }
                                            }
                                        }
                                }
                            }
                        }

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
            object[] o = new object[] { bmpDrawTo, bmpDrawFrom, this.cmbAlg.SelectedIndex, this.numUpperWeight.Value, this.numLowerWeight.Value };
            //                Form fff = new Form();
            //                fff.BackgroundImage = bmpDrawTo;
            //                fff.BackgroundImageLayout = ImageLayout.Zoom;
            //                fff.ShowDialog();  
            //fff.BackgroundImage = bmpDrawFrom;
            //                fff.BackgroundImageLayout = ImageLayout.Zoom;
            //                fff.ShowDialog();
            this.backgroundWorker1.RunWorkerAsync(o);
        }

        private void backgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap bmpDrawTo = (Bitmap)o[0]; // bmpbu
                using (Bitmap bmpDrawFrom = (Bitmap)o[1]) //_bmpDraw
                {
                    if (AvailMem.AvailMem.checkAvailRam(bmpDrawTo.Width * bmpDrawTo.Height * 5L))
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

                                    PoissonBlender pb = new PoissonBlender(new Bitmap(bmpLw), new Bitmap(bmpU));
                                    PoissonBlend.ProgressEventArgs pe = new PoissonBlend.ProgressEventArgs(mI * 3, 0);
                                    pe.PrgInterval = mI / 20;

                                    IBVectorComputingAlgorithm bAlg = pb.BlendParameters.GetBVectorAlg(bVAlg); // addb etc
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
                                    b1 = new Bitmap((Bitmap)img.Clone());
                                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                    b2 = new Bitmap((Bitmap)img.Clone());
                                    this.SetBitmap(ref this._bmpBU, ref b2);
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
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                    this._pic_changed = false;
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

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
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bOut, this.helplineRulerCtrl1, "Bmp");
                    this._pic_changed = true;
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

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
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
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
                        if (this._bmpBU != null && AvailMem.AvailMem.checkAvailRam(this._bmpBU.Width * this._bmpBU.Height * 12L))
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

                        if (_undoOPCache?.Count > 1)
                            this.btnRedo.Enabled = true;
                        else
                            this.btnRedo.Enabled = false;
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

                    this.helplineRulerCtrl1.Enabled = e;

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

                this.helplineRulerCtrl1.Enabled = e;

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
                this.toolStripProgressBar1.Value = this.toolStripProgressBar1.Minimum;
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

            if (this._dontClose)
                e.Cancel = true;

            if (!e.Cancel)
            {
                if (this._cAlg != null)
                    this._cAlg.CancellationPending = true;
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();
                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
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

                //if (this.helplineRulerCtrl2.Bmp != null && this._bmpBU != null)
                //{
                //    Bitmap? bOld = this._bmpBU;
                //    this._bmpBU = new Bitmap(this.helplineRulerCtrl2.Bmp);
                //    if (bOld != null)
                //    {
                //        bOld.Dispose();
                //        bOld = null;
                //    }
                //}

                this.CurPath = new List<PointF>();

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
            // If Me.Paths IsNot Nothing Then
            // Me.Paths.Clear()
            // End If

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
                    bOld = null/* TODO Change to default(_) if this is not a reference type */;
                }
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
            this.numUpperWeight.Enabled = false;
            this.numLowerWeight.Enabled = false;

            if (this.cmbAlg.SelectedIndex == 3)
            {
                this.label3.Enabled = true;
                this.label4.Enabled = true;
                this.numUpperWeight.Enabled = true;
                this.numLowerWeight.Enabled = true;
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

                double faktor = System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height);
                double multiplier = System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height);
                if (multiplier >= faktor)
                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Width) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Width));
                else
                    this.helplineRulerCtrl1.Zoom = System.Convert.ToSingle(System.Convert.ToDouble(this.helplineRulerCtrl1.dbPanel1.Height) / System.Convert.ToDouble(this.helplineRulerCtrl1.Bmp.Height));

                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

                Bitmap? bC = new Bitmap(bmpHLC1);
                this.SetBitmap(ref this._bmpBU, ref bC);

                Bitmap? bD = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);      
                this.SetBitmap(ref this._bmpDraw, ref bD);

                if (this._bmpBUZoomed != null)
                    this._bmpBUZoomed.Dispose();
                this._bmpBUZoomed = null;
                if (this._bmpBUZoomed == null && this._bmpBU != null)
                    MakeBitmap(this._bmpBU, this.helplineRulerCtrl2.Zoom);

                this._sourcePt = new Point(0, 0);

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
    }
}
