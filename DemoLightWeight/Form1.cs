using AvoidAGrabCutEasy;
using Cache;
using ChainCodeFinder;
using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace DemoLightWeight
{
    public partial class Form1 : Form
    {
        private bool _picChanged;
        private int _rX;
        private int _rY;
        private int _eX;
        private int _eY;
        private bool _tracking;
        private int _rW;
        private int _rH;
        private int _eW;
        private int _eH;
        private Rectangle _rect;
        private bool _dontDoZoom;

        private Bitmap? _bmpBU;

        public ZoomEnum ZoomEnum { get; private set; }

        public Form1(string[] args)
        {
            InitializeComponent();
            this.helplineRulerCtrl1.AddDefaultHelplines();
            this.helplineRulerCtrl1.ResetAllHelpLineLabelsColor();

            if (args != null)
            {
                if (args.Length > 0 && File.Exists(args[0]))
                {
                    Bitmap? bmp = null;
                    using (Image img = Image.FromFile(args[0]))
                        bmp = (Bitmap)img.Clone();

                    if (bmp != null)
                    {
                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                        this.ZoomEnum = ZoomEnum.Fit;
                        this.helplineRulerCtrl1.Enabled = false;
                        this.helplineRulerCtrl1.Refresh();
                        this.helplineRulerCtrl1.SetZoom(ParseZE(this.ZoomEnum));
                        this.helplineRulerCtrl1.Enabled = true;

                        this._picChanged = false;
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        Bitmap? bC = (Bitmap)bmp.Clone();
                        this.SetBitmap(ref this._bmpBU, ref bC);
                    }

                    this.Text = args[0];
                }
            }

            this.helplineRulerCtrl1.dbPanel1.MouseDown += helplineRulerCtrl1_MouseDown;
            this.helplineRulerCtrl1.dbPanel1.MouseMove += helplineRulerCtrl1_MouseMove;
            this.helplineRulerCtrl1.dbPanel1.MouseUp += helplineRulerCtrl1_MouseUp;

            this.helplineRulerCtrl1.PostPaint += helplineRulerCtrl1_Paint;
        }

        private void helplineRulerCtrl1_DBPanelDblClicked(object sender, HelplineRulerControl.ZoomEventArgs e)
        {
            this._dontDoZoom = true;

            if (e.Zoom == 1.0F)
                this.ZoomEnum = ZoomEnum.One;
            else if (e.ZoomWidth)
                this.ZoomEnum = ZoomEnum.ZoomWidth;
            else
                this.ZoomEnum = ZoomEnum.Fit;

            if (this.Visible && this.helplineRulerCtrl1.Bmp != null)
            {
                this.helplineRulerCtrl1.Enabled = false;
                this.helplineRulerCtrl1.Refresh();
                this.helplineRulerCtrl1.SetZoom(ParseZE(this.ZoomEnum));
                this.helplineRulerCtrl1.Enabled = true;

                SetRectOrScribblesValues(this.helplineRulerCtrl1.Zoom);

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            if (this._dontDoZoom)
                this._dontDoZoom = false;
        }

        private void SetRectOrScribblesValues(float zoom)
        {
            Rectangle r = new Rectangle(this._rX, this._rY, this._rW, this._rH);
            this._eX = (int)(r.X * zoom);
            this._eY = (int)(r.Y * zoom);
            this._eW = (int)(r.Width * zoom);
            this._eH = (int)(r.Height * zoom);
        }

        private string ParseZE(ZoomEnum zoomEnum)
        {
            switch (zoomEnum)
            {
                case ZoomEnum.Fit:
                    return "fit";
                case ZoomEnum.ZoomWidth:
                    return "fit_width";
                case ZoomEnum.One:
                    return "1";
            }

            return "fit";
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (this._picChanged)
            {
                DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dlg == DialogResult.Yes)
                    btnSave.PerformClick();
                else if (dlg == DialogResult.Cancel)
                    return;
            }

            Bitmap? bmp = null;
            if (this.OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (Image img = Image.FromFile(this.OpenFileDialog1.FileName))
                    bmp = (Bitmap)img.Clone();

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                    this.ZoomEnum = ZoomEnum.Fit;
                    this.helplineRulerCtrl1.Enabled = false;
                    this.helplineRulerCtrl1.Refresh();
                    this.helplineRulerCtrl1.SetZoom(ParseZE(this.ZoomEnum));
                    this.helplineRulerCtrl1.Enabled = true;

                    this._picChanged = false;
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    Bitmap? bC = (Bitmap)bmp.Clone();
                    this.SetBitmap(ref this._bmpBU, ref bC);
                }

                this.Text = this.OpenFileDialog1.FileName;

                this.btnGo.Enabled = true;

                this._rX = this._rY = this._rW = this._eH = 0;
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

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_picChanged)
            {
                DialogResult dlg = MessageBox.Show("Save image?", "Unsaved data", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                if (dlg == DialogResult.Yes)
                {
                    this.btnSave.PerformClick();
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

                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
            }
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                int eX = e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                int eY = e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;

                this.helplineRulerCtrl1.dbPanel1.Capture = true;

                int ix2 = Math.Min(ix, this.helplineRulerCtrl1.Bmp.Width - 1);
                int iy2 = Math.Min(iy, this.helplineRulerCtrl1.Bmp.Height - 1);

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    this._rX = ix;
                    this._rY = iy;

                    this._eX = eX;
                    this._eY = eY;

                    this._tracking = true;

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

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

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
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

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }

                this._tracking = false;

                this.helplineRulerCtrl1.dbPanel1.Capture = false;
            }
        }

        private void helplineRulerCtrl1_Paint(object? sender, PaintEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
                using (Pen pen = new Pen(new SolidBrush(Color.Lime), 2))
                    e.Graphics.DrawRectangle(pen, new Rectangle(this._eX + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                        this._eY + this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y, this._eW, this._eH));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.saveFileDialog1.Filter = "Png-Images (*.png)|*.png";
                this.saveFileDialog1.FileName = "Bild1.png";

                try
                {
                    if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        this.helplineRulerCtrl1.Bmp.Save(this.saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                        _picChanged = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (this._bmpBU != null)
            {
                Bitmap bmp = (Bitmap)this._bmpBU.Clone();

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                    this._picChanged = false;
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();

                    this.btnGo.Enabled = this.numDispAmnt.Enabled = true;
                }

                //this._rX = this._rY = this._rW = this._eH = 0;
            }
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            //reset state
            if (this.backgroundWorker1.IsBusy)
            {
                if (this.backgroundWorker1.IsBusy)
                    this.backgroundWorker1.CancelAsync();

                return;
            }

            if (!this.backgroundWorker1.IsBusy && this.helplineRulerCtrl1.Bmp != null &&
                this.helplineRulerCtrl1.Bmp.Width > 10 && this.helplineRulerCtrl1.Bmp.Height > 10)
            {
                //this test is not corresponding to the real amount of RAM being used by the algorithms. It's more or less just an indicator, if to start at all.
                if (AvailMem.AvailMem.checkAvailRam(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Bmp.Height * 100L))
                {
                    //now get the needed parameters from the controls
                    this.SetControls(false);
                    this.btnGo.Text = "Cancel";
                    this.btnGo.Enabled = true;

                    this.toolStripProgressBar1.Value = 0;
                    this.toolStripProgressBar1.Visible = true;

                    this.toolStripStatusLabel1.Text = "1";

                    Rectangle r = new Rectangle(this._rX, this._rY, this._rW, this._rH);

                    //if no rect specified, setup with almost the whole image
                    if (r.Width == 0 || r.Height == 0)
                        r = new Rectangle(10, 10, this.helplineRulerCtrl1.Bmp.Width - 20, this.helplineRulerCtrl1.Bmp.Height - 20);

                    Bitmap bWork = (Bitmap)this.helplineRulerCtrl1.Bmp.Clone();

                    int displayAmnt = (int)this.numDispAmnt.Value;

                    //now start the work
                    this.backgroundWorker1.RunWorkerAsync(new object[] { bWork, r, displayAmnt });
                }
            }
        }

        private void backgroundWorker1_DoWork(object? sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap? bWork = (Bitmap)o[0];
                Bitmap bTmp = (Bitmap)bWork.Clone();
                Rectangle r = (Rectangle)o[1];
                int displayAmnt = (int)o[2];

                //create the operator for the GrabcutALike methods
                GrabCutOp _gc = new GrabCutOp()
                {
                    Bmp = bWork,
                    Gmm_comp = 2,
                    Gamma = 50,
                    NumIters = 1,
                    RectMode = true,
                    Rc = r,
                    BGW = this.backgroundWorker1,
                    QuickEstimation = true,
                    UseThreshold = true,
                    Threshold = 10.5,
                    MultCapacitiesForTLinks = true,
                    MultTLinkCapacity = 1,
                    CastIntCapacitiesForTLinks = true,
                    SelectionMode = AvoidAGrabCutEasy.ListSelectionMode.Min,
                    AssumeExpDist = true
                };

                _gc.ShowInfo += _gc_ShowInfo;

                //now do the initialisation
                //eg create the mask, preclassify the imagedata, compute the smootheness function and init the Gmms
                int it = _gc.Init();

                if (_gc.BGW != null && _gc.BGW.WorkerSupportsCancellation && _gc.BGW.CancellationPending)
                    it = -4;

                if (it != 0)
                {
                    ShowErrorMessage(it, 0);
                    if (bWork != null)
                        bWork.Dispose();
                    e.Result = (Bitmap)bTmp.Clone();
                    return;
                }

                //now do the work ...
                int l = _gc.Run();

                if (l != 0)
                {
                    ShowErrorMessage(l, 1);
                    if (bWork != null)
                        bWork.Dispose();
                    bWork = null;
                    e.Result = (Bitmap)bTmp.Clone();
                    return;
                }

                if (bWork != null)
                {
                    e.Result = ProcessResults(bTmp, _gc.Result, bTmp.Size, _gc.Mask, r, displayAmnt);

                    if (bTmp != null)
                        bTmp.Dispose();

                    if (bWork != null)
                        bWork.Dispose();
                }
            }
        }

        private unsafe Bitmap? ProcessResults(Bitmap bTmp, List<int>? result, Size sz, int[,]? mask, Rectangle r, int displayAmnt)
        {
            //... and get the result ...
            List<int>? res = result;

            //... and the result image
            Bitmap bRes = new Bitmap(sz.Width, sz.Height);

            int[,]? m = mask;
            int w = sz.Width;
            int h = sz.Height;

            //lock the bmps for fast processing
            BitmapData bmData = bRes.LockBits(new Rectangle(0, 0, bRes.Width, bRes.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmWork = bTmp.LockBits(new Rectangle(0, 0, bTmp.Width, bTmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int stride = bmData.Stride;

            //get the references to the pointer addresses
            byte* p = (byte*)bmData.Scan0;
            byte* pWork = (byte*)bmWork.Scan0;

            if (m != null)
            {
                //write the data
                int ww = m.GetLength(0);
                int hh = m.GetLength(1);

                for (int y = 0; y < h; y++)
                {
                    if (y > 0 && y < h)
                        for (int x = 0; x < w; x++)
                        {
                            if (x > 0 && x < w)
                                if (x < ww && y < hh && r.Contains(x, y) && (m[x, y] == 1 || m[x, y] == 3))
                                {
                                    p[x * 4 + y * stride] = pWork[x * 4 + y * stride];
                                    p[x * 4 + y * stride + 1] = pWork[x * 4 + y * stride + 1];
                                    p[x * 4 + y * stride + 2] = pWork[x * 4 + y * stride + 2];
                                    p[x * 4 + y * stride + 3] = pWork[x * 4 + y * stride + 3];
                                }
                        }
                }
            }

            //and unlock the bmps
            bTmp.UnlockBits(bmWork);
            bRes.UnlockBits(bmData);

            //use a ChainCode [that works on the - invisible - "cracks" between the pixels, because it is very fast aand reliable]
            List<ChainCode>? c = GetBoundary(bRes, 0, false);
            c = c?.OrderByDescending(x => x.Coord.Count).ToList();

            int? comp = c?.Count;

            if (comp.HasValue)
                comp = Math.Min(comp.Value, displayAmnt);

            if (c != null)
            {
                if (c.Count > 0)
                {
                    //if we have a very lot of components, restrict the amount
                    if (comp > 1000)
                        comp = 1000;

                    //now begin to redraw each component
                    using Graphics gx = Graphics.FromImage(bRes);

                    gx.Clear(Color.Transparent);

                    if (comp.HasValue)
                    {
                        int amnt = comp.Value;

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
                if (c.Count > 0 && comp.HasValue)
                {
                    int amnt = comp.Value;

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

                //our result pic
                return bRes;
            }

            return null;
        }

        private void ShowErrorMessage(int it, int part)
        {
            if (part == 0)
                switch (it)
                {
                    case -1:
                        this.Invoke(new Action(() => { MessageBox.Show("No BGPixels found. Cancelled operation."); }));
                        return;
                    case -2:
                        this.Invoke(new Action(() => { MessageBox.Show("No FGPixels found. Cancelled operation."); }));
                        return;
                    case -3:
                        this.Invoke(new Action(() => { MessageBox.Show("No Image passed to function. Cancelled operation."); }));
                        return;
                    case -4:
                        this.Invoke(new Action(() => { MessageBox.Show("Operation cancelled."); }));
                        return;
                    case -5:
                        this.Invoke(new Action(() => { MessageBox.Show("Mask is null. Cancelled operation."); }));
                        return;
                }
            if (part == 1)
                switch (it)
                {
                    case -1:
                        this.Invoke(new Action(() => { MessageBox.Show("Arrays-Length, or Graph-Length failed test. Cancelled operation."); }));
                        return;

                    case -25:
                        this.Invoke(new Action(() => { MessageBox.Show("Graph-Construction failed. Maybe the threshold is too big. Cancelled operation."); }));
                        return;

                    case 100:
                        this.Invoke(new Action(() => { MessageBox.Show("Bmp_width or Bmp_height = 0. Cancelled operation."); }));
                        return;

                    case 101:
                        this.Invoke(new Action(() => { MessageBox.Show("Operation cancelled."); }));
                        return;

                    case 102:
                        this.Invoke(new Action(() => { MessageBox.Show("At least one GMM is null. Cancelled operation."); }));
                        return;

                    case 103:
                        this.Invoke(new Action(() => { MessageBox.Show("This Mode only makes sense with RectMode."); }));
                        return;

                    case 104:
                        this.Invoke(new Action(() => { MessageBox.Show("This Mode only makes sense with ScribbleMode."); }));
                        return;
                }
        }

        private void backgroundWorker1_ProgressChanged(object? sender, System.ComponentModel.ProgressChangedEventArgs e)
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

        private void backgroundWorker1_RunWorkerCompleted(object? sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //if we have a result pic, display it
            if (e.Result != null)
            {
                Bitmap b = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
            }

            //re enable the UI
            this.SetControls(true);
            this._picChanged = true;
            this.btnGo.Text = "Go";
            this.btnGo.Enabled = this.numDispAmnt.Enabled = false;


            //re init the bgw
            this.backgroundWorker1.Dispose();
            this.backgroundWorker1 = new BackgroundWorker();
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            this.backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            this.backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;

            this.toolStripStatusLabel4.Text = "done";
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.splitContainer1.Panel1.Controls)
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
                foreach (Control ct in this.splitContainer1.Panel1.Controls)
                {
                    if (ct.Name != "btnCancel" && !(ct is PictureBox))
                        ct.Enabled = e;
                }

                this.helplineRulerCtrl1.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
            }
        }

        private void _gc_ShowInfo(object? sender, string e)
        {
            if (InvokeRequired)
                this.Invoke(new Action(() => this.toolStripStatusLabel4.Text = e));
            else
                this.toolStripStatusLabel4.Text = e;
        }

        private List<ChainCode>? GetBoundary(Bitmap? upperImg, int minAlpha, bool grayScale)
        {
            List<ChainCode>? l = null;
            Bitmap? bmpTmp = null;
            try
            {
                if (upperImg != null)
                    if (AvailMem.AvailMem.checkAvailRam(upperImg.Width * upperImg.Height * 4L))
                        bmpTmp = (Bitmap)upperImg.Clone();
                    else
                        throw new Exception("Not enough memory.");
                if (bmpTmp != null)
                {
                    int nWidth = bmpTmp.Width;
                    int nHeight = bmpTmp.Height;
                    ChainFinder cf = new ChainFinder();
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

        private void Form1_Load(object sender, EventArgs e)
        {
            this.helplineRulerCtrl1.BackColor = SystemColors.ControlDarkDark;
        }
    }
}
