using Cache;
using ChainCodeFinder;
using ConvolutionLib;
using MorphologicalProcessing2;
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

namespace OutlineOperations
{
    public partial class frnOutlineOperations : Form
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
                return this.helplineRulerCtrl1.Bmp;
            }
        }

        public List<PointF>? CurPath { get; private set; }

        private Bitmap? _bmpBU = null;
        private bool _pic_changed = false;

        private int _openPanelHeight;
        private bool _isHoveringVertically;
        private bool _doubleClicked;
        private bool _dontDoZoom;
        private bool _tracking;
        private Bitmap? _bmpMatte;
        private Bitmap? _bmpOrig;
        private object _lockObject = new object();
        private List<Point>? _points;
        private List<Bitmap>? _excludedRegions;
        private List<Point>? _exclLocations;
        private int _ix;
        private int _iy;
        private bool _dontDrawPath;
        private Color _fColor = Color.Lime;
        private int _eX;
        private int _eY;

        public frnOutlineOperations(Bitmap bmp, string basePathAddition)
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

            this._dontDoZoom = true;
            this.cmbZoom.SelectedIndex = 4;
            this._dontDoZoom = false;
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                int eX = e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                int eY = e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this.cbDraw.Checked && this._bmpOrig != null)
                    {
                        if (this._points == null)
                            this._points = new List<Point>();
                        this._points.Clear();
                        this._points.Add(new Point(ix, iy));

                        this._tracking = true;
                    }

                    if (this.cbWipeAlpha.Checked && e.Button == MouseButtons.Left)
                    {
                        this._dontDrawPath = false;

                        this._ix = ix;
                        this._iy = iy;
                        SetupCurPath();
                        this.CurPath?.Add(new PointF(this._ix, this._iy));

                        this._tracking = true;
                    }
                }
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                this._eX = e.X;
                this._eY = e.Y;

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this._tracking)
                    {
                        if (this.cbDraw.Checked && this._bmpOrig != null)
                            this._points?.Add(new Point(ix, iy));

                        if (this.cbWipeAlpha.Checked && e.Button == MouseButtons.Left && (ix != this._ix || iy != this._iy))
                        {
                            this._ix = ix;
                            this._iy = iy;
                            SetupCurPath();
                            this.CurPath?.Add(new PointF(this._ix, this._iy));
                        }
                    }

                    Color c = this.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                    this.toolStripStatusLabel1.Text = ix.ToString() + "; " + iy.ToString();
                    this.toolStripStatusLabel5.Text = c.ToString();
                    this.ToolStripStatusLabel2.BackColor = c;
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
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

                if (this._tracking)
                {
                    if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                    {
                        if (this.cbDraw.Checked && this._bmpOrig != null)
                        {
                            if (ix != this._points?[0].X || iy != this._points?[0].Y)
                                this._points?.Add(new Point(ix, iy));
                            DrawPointsToBitmap();
                        }
                    }

                    if (ix == _ix && iy == _iy && this.cbWipeAlpha.Checked && e.Button == MouseButtons.Left)
                    {
                        this._ix = ix;
                        this._iy = iy;

                        if (this.CurPath != null && !ContainsPoint(this.CurPath, new PointF(this._ix, this._iy)))
                        {
                            SetupCurPath();
                            this.CurPath.Add(new PointF(this._ix, this._iy));
                        }
                    }

                    if (this._tracking && this.CurPath != null && this.CurPath.Count > 0)
                        WipeAlpha();
                }
            }

            this._tracking = false;
        }

        private void DrawPointsToBitmap()
        {
            if (this._points != null && this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
            {
                if (this._points.Count > 1)
                {
                    using GraphicsPath gP = new GraphicsPath();
                    using Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp);
                    using TextureBrush tb = new TextureBrush(this._bmpOrig);
                    using Pen pen = new Pen(tb, (float)this.numDrawWidth.Value);
                    pen.LineJoin = LineJoin.Round;
                    gP.AddLines(this._points.Select(a => new PointF(a.X, a.Y)).ToArray());
                    gx.DrawPath(pen, gP);
                }
                else if (this._points.Count == 1)
                {
                    using GraphicsPath gP = new GraphicsPath();
                    using Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp);
                    using TextureBrush tb = new TextureBrush(this._bmpOrig);
                    float wh = (float)this.numDrawWidth.Value;
                    gP.AddEllipse(this._points[0].X - wh / 2f, this._points[0].Y - wh / 2f,
                        wh, wh);
                    gx.FillPath(tb, gP);
                }

                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.Invalidate();
                this._points.Clear();

                this._undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                this._pic_changed = true;
                this.btnUndo.Enabled = true;
                this.CheckRedoButton();
            }
        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {
            if (this._points != null && this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
            {
                if (this._points.Count > 1)
                {
                    using GraphicsPath gP = new GraphicsPath();
                    using TextureBrush tb = new TextureBrush(this._bmpOrig);
                    tb.ScaleTransform(this.helplineRulerCtrl1.Zoom, this.helplineRulerCtrl1.Zoom);
                    tb.TranslateTransform(this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y, MatrixOrder.Append);
                    using Pen pen = new Pen(tb, Math.Max((float)this.numDrawWidth.Value * this.helplineRulerCtrl1.Zoom, 1f));
                    pen.LineJoin = LineJoin.Round;
                    gP.AddLines(this._points.Select(a => new PointF(a.X, a.Y)).ToArray());
                    using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    gP.Transform(mx);
                    e.Graphics.DrawPath(pen, gP);
                }
                else if (this._points.Count == 1)
                {
                    using GraphicsPath gP = new GraphicsPath();
                    using TextureBrush tb = new TextureBrush(this._bmpOrig);
                    tb.ScaleTransform(this.helplineRulerCtrl1.Zoom, this.helplineRulerCtrl1.Zoom);
                    tb.TranslateTransform(this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y, MatrixOrder.Append);
                    float wh = (float)this.numDrawWidth.Value;
                    gP.AddEllipse(this._points[0].X - wh / 2f, this._points[0].Y - wh / 2f,
                        wh, wh);
                    using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X,
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                    gP.Transform(mx);
                    e.Graphics.FillPath(tb, gP);
                }
            }

            if (!this._dontDrawPath && this.cbWipeAlpha.Checked && this._tracking)
            {
                using (GraphicsPath gp = GetPath())
                {
                    float w = (float)this.numPenSize.Value;
                    int x = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                    int y = this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;
                    using (Matrix m = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom, x, y))
                    {
                        gp.Transform(m);
                        using (Pen pen = new Pen(new SolidBrush(this._fColor), Math.Max(w * this.helplineRulerCtrl1.Zoom, 1f)))
                        {
                            pen.LineJoin = LineJoin.Round;
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;

                            if (this.CurPath?.Count == 1)
                                e.Graphics.FillPath(new SolidBrush(this._fColor), gp);
                            else
                                e.Graphics.DrawPath(pen, gp);
                        }
                    }
                }
            }

            if (this.cbWipeAlpha.Checked)
            {
                float w = (float)this.numPenSize.Value;
                float ww = Math.Max(w * this.helplineRulerCtrl1.Zoom, 1f);
                using SolidBrush sb = new SolidBrush(Color.FromArgb(64, this._fColor));
                e.Graphics.FillEllipse(sb, new RectangleF(_eX - ww / 2f, _eY - ww / 2f, ww, ww));
            }
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmOutl");
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
        }

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("");
        }

        private void button4_Click(object sender, EventArgs e)
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

        private void button5_Click(object sender, EventArgs e)
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
        }

        private void CheckBox12_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (this.cbBGColor.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
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

        private void SetBitmap(ref Bitmap? bitmapToSet, ref Bitmap bitmapToBeSet)
        {
            Bitmap? bOld = bitmapToSet;

            bitmapToSet = bitmapToBeSet;

            if (bOld != null && bOld.Equals(bitmapToBeSet) == false)
                bOld.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_pic_changed && !_dontAskOnClosing)
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
                if (this._bmpBU != null)
                    this._bmpBU.Dispose();
                if (this._bmpMatte != null)
                    this._bmpMatte.Dispose();
                if (this._bmpOrig != null)
                    this._bmpOrig.Dispose();
                if (this._undoOPCache != null)
                    this._undoOPCache.Dispose();
            }
        }

        private void frmOutlineOperations_Load(object sender, EventArgs e)
        {
            this.CheckBox12_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cmbMorph.SelectedIndex = 0;

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.numRMatteW.Value = (decimal)this.helplineRulerCtrl1.Bmp.Width;
                this.numRMatteH.Value = (decimal)this.helplineRulerCtrl1.Bmp.Height;
            }
        }

        private void btnAlphaTh_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                using (frmAlphaThreshold frm = new frmAlphaThreshold(this.helplineRulerCtrl1.Bmp))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        this.SetControls(false);
                        this.btnAlphaTh.Text = "Cancel";
                        this.btnAlphaTh.Enabled = true;

                        this.backgroundWorker9.RunWorkerAsync(new object[] { frm.trackBar1.Value });
                    }
                }
            }
        }

        private void btnAlphaZAndGain_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker4.IsBusy)
            {
                this.backgroundWorker4.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnAlphaZAndGain.Enabled = true;
                this.btnAlphaZAndGain.Text = "Cancel";

                int alphaTh = (int)this.numAlphaZAndGain.Value;

                Bitmap b = new Bitmap(this.helplineRulerCtrl1.Bmp);
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
                                    SetTransp(b, rem, frm.Excluded.Location);

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
                    this._excludedRegions.Add(new Bitmap(excl));
                    this._exclLocations.Add(excludedRegions[j].Location);
                }
            }
        }

        private void btnSetGamma_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker5.IsBusy)
            {
                this.backgroundWorker5.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnSetGamma.Enabled = true;
                this.btnSetGamma.Text = "Cancel";

                double gamma = (double)this.numGamma.Value;

                Bitmap b = new Bitmap(this.helplineRulerCtrl1.Bmp);
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
                                    SetTransp(b, rem, frm.Excluded.Location);

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

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                using Bitmap bmp = new Bitmap((Bitmap)o[0]);
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
                    SetColorsToOrig(bmp, this.helplineRulerCtrl1.Bmp);
                }

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                        (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    _undoOPCache?.Add(bmp);

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                this.btnAlphaZAndGain.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

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

        private void backgroundWorker5_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = (Bitmap)o[0];
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
                }

                if (bmp != null)
                {
                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                    this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                        (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                        (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                    _undoOPCache?.Add(bmp);
                }

                this.btnSetGamma.Text = "Go";

                this.SetControls(true);
                this.Cursor = Cursors.Default;

                this.btnOK.Enabled = this.btnCancel.Enabled = true;

                this._pic_changed = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();

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

        private void backgroundWorker9_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null && this.helplineRulerCtrl1.Bmp != null)
            {
                bool b = false;
                object[] o = (object[])e.Argument;
                int th = (int)o[0];
                if (th < 0)
                    th = 0;
                if (th > 255)
                    th = 255;

                Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);

                if (bmp != null)
                {
                    ThresholdMethods thMthds = new ThresholdMethods();
                    thMthds.CancelLoops = false;

                    if (this.backgroundWorker4.CancellationPending)
                        thMthds.CancelLoops = true;

                    b = ThresholdMethods.ThresholdAlpha(bmp, th);
                }

                e.Result = bmp;
            }
        }

        private void backgroundWorker9_ProgressChanged(object? sender, ProgressChangedEventArgs e)
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

        private void backgroundWorker9_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            this.SetControls(true);
            this.btnAlphaTh.Text = "AlphaThreshold";

            this.btnOK.Enabled = this.btnCancel.Enabled = true;

            this._pic_changed = true;

            Bitmap? bmp = null;

            if (e.Result != null)
                bmp = (Bitmap)e.Result;

            if (this._undoOPCache != null && bmp != null)
            {
                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                this._undoOPCache?.Add(bmp);
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            this.backgroundWorker9.Dispose();
            this.backgroundWorker9 = new BackgroundWorker();
            this.backgroundWorker9.WorkerReportsProgress = true;
            this.backgroundWorker9.WorkerSupportsCancellation = true;
            this.backgroundWorker9.DoWork += backgroundWorker9_DoWork;
            this.backgroundWorker9.ProgressChanged += backgroundWorker9_ProgressChanged;
            this.backgroundWorker9.RunWorkerCompleted += backgroundWorker9_RunWorkerCompleted;
        }

        private void Timer3_Tick(object sender, EventArgs e)
        {
            this.Timer3.Stop();

            if (!this.toolStripProgressBar1.IsDisposed)
            {
                this.toolStripProgressBar1.Visible = false;
                this.toolStripProgressBar1.Value = 0;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            frmEdgePic frm4 = new frmEdgePic(this.pictureBox1.Image);
            frm4.Text = "Matte";
            frm4.ShowDialog();
        }

        private void btnLoadMatte_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;
                using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                    bmp = new Bitmap(img);

                if (bmp.Width != this.helplineRulerCtrl1.Bmp.Width || bmp.Height != this.helplineRulerCtrl1.Bmp.Height)
                {
                    Bitmap? bOld = bmp;
                    bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                    using Graphics g = Graphics.FromImage(bmp);
                    g.DrawImage(bOld, 0, 0, bmp.Width, bmp.Height);
                    if (bOld != null)
                        bOld.Dispose();
                    bOld = null;
                    MessageBox.Show("Matte resized.");
                }

                this.SetBitmap(ref this._bmpMatte, ref bmp);
                this.pictureBox1.Image = this._bmpMatte;
                this.pictureBox1.Refresh();

                _undoOPCache?.Add(bmp);

                this._pic_changed = true;
            }
        }

        private void btnApplyMatte_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._bmpMatte != null)
            {
                using (Bitmap b = new Bitmap(this.pictureBox1.Image))
                {
                    Bitmap? bmp = ApplyAlphaMatte(this.helplineRulerCtrl1.Bmp, b);

                    if (bmp != null)
                    {
                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                        //Bitmap bC = new Bitmap(bmp);
                        //this.SetBitmap(ref _bmpRef, ref bC);

                        this.helplineRulerCtrl1.Zoom = this.helplineRulerCtrl1.Zoom;
                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                        _undoOPCache?.Add(bmp);

                        this._pic_changed = true;
                    }
                }
            }
        }

        private unsafe Bitmap? ApplyAlphaMatte(Bitmap bmpIn, Bitmap bmpAlpha)
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

                        p[3] = (byte)Math.Max(Math.Min((double)pIn[3] / 255.0 * (double)pA[0], 255), 0);

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

        private void btnOrig_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Bitmap? bmp = null;
                using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                    bmp = new Bitmap(img);

                this.SetBitmap(ref this._bmpOrig, ref bmp);
                this.pictureBox2.Image = this._bmpOrig;
                this.pictureBox2.Refresh();

                _undoOPCache?.Add(bmp);

                this._pic_changed = true;

                this.btnMask.Enabled = true;

                if (MessageBox.Show("Also load to HLC1?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes &&
                    this._bmpOrig != null)
                {
                    Bitmap? bC = new Bitmap(this._bmpOrig);

                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, bC, this.helplineRulerCtrl1, "Bmp");
                    this._pic_changed = true;
                    this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                    this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                    _undoOPCache?.Add(bC);

                    if (_undoOPCache?.CurrentPosition > 1)
                    {
                        this.btnUndo.Enabled = true;
                    }
                    else
                        this.btnUndo.Enabled = false;

                    if (_undoOPCache?.CurrentPosition < _undoOPCache?.Count)
                        this.btnRedo.Enabled = true;
                    else
                        this.btnRedo.Enabled = false;

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
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

            if (this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);

                this.btnGo.Enabled = true;
                this.btnGo.Text = "Cancel";

                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                Bitmap bWork = new Bitmap(this.helplineRulerCtrl1.Bmp);
                Bitmap bOrig = new Bitmap(this._bmpOrig);
                int n = (int)this.numKernel.Value;
                int mode = this.cmbMorph.SelectedIndex;

                this.backgroundWorker1.RunWorkerAsync(new object[] { bWork, bOrig, mode, n });
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap? bWork = (Bitmap)o[0];
                Bitmap? bOrig = (Bitmap)o[1];
                int mode = (int)o[2];
                int kernel = (int)o[3];

                Bitmap? bMorph = GetMorphShape(bWork);

                switch (mode)
                {
                    case 0:
                        DoClosing(bMorph, kernel, true, true);
                        break;
                    case 1:
                        Dilate(bMorph, kernel, true, true);
                        break;
                    case 2:
                        Erode(bMorph, kernel, true, true);
                        break;
                    default:
                        break;
                }

                CropOrig(bOrig, bMorph);

                bWork.Dispose();
                bWork = null;
                bMorph.Dispose();
                bMorph = null;

                e.Result = bOrig;
            }
        }

        private unsafe void CropOrig(Bitmap bOrig, Bitmap bMorph)
        {
            int w = bOrig.Width;
            int h = bOrig.Height;

            BitmapData bmD = bOrig.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmRead = bMorph.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;
                byte* pR = (byte*)bmRead.Scan0;
                pR += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pR[0] == 0)
                        p[0] = p[1] = p[2] = p[3] = 0;

                    p += 4;
                    pR += 4;
                }
            });

            bOrig.UnlockBits(bmD);
            bMorph.UnlockBits(bmRead);
        }

        private void DoClosing(Bitmap bw, int wh, bool diskshaped, bool once)
        {
            IMorphologicalOperation alg = new MorphologicalProcessing2.Algorithms.Closing();

            alg.BGW = this.backgroundWorker1;

            if (once)
            {
                if (diskshaped)
                    alg.SetupEx(wh, wh);
                else
                    alg.Setup(wh, wh);

                alg.ApplyGrayscale(bw);
            }
            else
            {
                if (diskshaped)
                    alg.SetupEx(3, 3);
                else
                    alg.Setup(3, 3);

                int amount = Math.Max(wh / 3, 1);

                for (int i = 0; i < amount; i++)
                    alg.ApplyGrayscale(bw);
            }
        }

        private void Erode(Bitmap bw, int wh, bool diskshaped, bool once)
        {
            IMorphologicalOperation alg = new MorphologicalProcessing2.Algorithms.Erode();

            alg.BGW = this.backgroundWorker1;

            if (once)
            {
                if (diskshaped)
                    alg.SetupEx(wh, wh);
                else
                    alg.Setup(wh, wh);

                alg.ApplyGrayscale(bw);
            }
            else
            {
                if (diskshaped)
                    alg.SetupEx(3, 3);
                else
                    alg.Setup(3, 3);

                int amount = Math.Max(wh / 3, 1);

                for (int i = 0; i < amount; i++)
                    alg.ApplyGrayscale(bw);

            }
        }

        private void Dilate(Bitmap bw, int wh, bool diskshaped, bool once)
        {
            IMorphologicalOperation alg = new MorphologicalProcessing2.Algorithms.Dilate();

            alg.BGW = this.backgroundWorker1;

            if (once)
            {
                if (diskshaped)
                    alg.SetupEx(wh, wh);
                else
                    alg.Setup(wh, wh);

                alg.ApplyGrayscale(bw);
            }
            else
            {
                if (diskshaped)
                    alg.SetupEx(3, 3);
                else
                    alg.Setup(3, 3);

                int amount = Math.Max(wh / 3, 1);

                for (int i = 0; i < amount; i++)
                    alg.ApplyGrayscale(bw);

            }
        }

        private Bitmap GetMorphShape(Bitmap bWork)
        {
            int w = bWork.Width;
            int h = bWork.Height;

            Bitmap bmp = new Bitmap(w, h);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Black);

                List<ChainCode>? c = GetBoundary(bWork, 0, false);

                if (c != null && c.Count > 0)
                {
                    c = c.OrderByDescending(x => x.Coord.Count).ToList();
                    foreach (ChainCode cc in c)
                    {
                        if (!ChainFinder.IsInnerOutline(cc) && cc.Coord.Count > 4)
                        {
                            using (GraphicsPath gP = new GraphicsPath())
                            {
                                gP.AddLines(cc.Coord.Select(a => new PointF(a.X, a.Y)).ToArray());

                                g.FillPath(Brushes.White, gP);
                                g.DrawPath(Pens.Wheat, gP);
                            }
                        }
                    }
                }
            }

            return bmp;
        }

        private Point? GetClosestPointIndex(List<Point> pts, PointF pt)
        {
            Point? ptOut = null;
            int indx = -1;
            double dist = double.MaxValue;

            for (int i = 0; i < pts.Count; i++)
            {
                double dx = pts[i].X - pt.X;
                double dy = pts[i].Y - pt.Y;
                double d = Math.Sqrt(dx * dx + dy * dy);

                if (d < dist)
                {
                    indx = i;
                    dist = d;
                    ptOut = new Point(pts[i].X, pts[i].Y);
                }
            }

            return ptOut;
        }

        private List<ChainCode>? GetBoundary(Bitmap upperImg, int minAlpha, bool grayScale)
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
                lock (this._lockObject)
                    l = cf.GetOutline(bmpTmp, nWidth, nHeight, minAlpha, grayScale, 0, false, 0, false);
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
                Bitmap bRes = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bRes, this.helplineRulerCtrl1, "Bmp");

                //Bitmap bC = new Bitmap(bRes);
                //this.SetBitmap(ref _bmpRef, ref bC);

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(bRes);

            }

            this.btnGo.Text = "Go";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this._pic_changed = true;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

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
        }

        private void btnApproxLines_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
            {
                int w = this.helplineRulerCtrl1.Bmp.Width;
                int h = this.helplineRulerCtrl1.Bmp.Height;

                Bitmap b = new Bitmap(w, h);

                List<ChainCode>? c = GetBoundary(this.helplineRulerCtrl1.Bmp, 0, false);

                ChainFinder cf = new ChainFinder();
                double epsilon = (double)this.numEpsilon.Value;

                if (c != null && c.Count > 0)
                {
                    c = c.OrderByDescending(x => x.Coord.Count).ToList();
                    foreach (ChainCode cc in c)
                    {
                        if (cc.Coord.Count > 4)
                        {
                            List<Point> lList = new List<Point>();
                            lList.AddRange(cc.Coord);
                            lList = cf.RemoveColinearity(lList, true);
                            lList = cf.ApproximateLines(lList, epsilon);

                            using (Graphics g = Graphics.FromImage(b))
                            using (TextureBrush tb = new TextureBrush(this._bmpOrig))
                            using (Pen pen = new Pen(tb, 1))
                            using (GraphicsPath gP = new GraphicsPath())
                            {
                                if (this.cbCurves.Checked)
                                    gP.AddClosedCurve(lList.Select(a => new PointF(a.X, a.Y)).ToArray(), (float)this.numTension.Value);
                                else
                                {
                                    gP.AddLines(lList.Select(a => new PointF(a.X, a.Y)).ToArray());
                                    gP.CloseFigure();
                                }

                                g.FillPath(tb, gP);
                                g.DrawPath(pen, gP);
                            }
                        }
                    }
                }

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(b);

                this.btnUndo.Enabled = true;
                this.CheckRedoButton();
                this._pic_changed = true;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void btnShrExt_Click(object sender, EventArgs e)
        {
            if (this._bmpOrig != null && this.helplineRulerCtrl1.Bmp != null && this.CachePathAddition != null)
            {
                frmExtend frm = new frmExtend(this.helplineRulerCtrl1.Bmp, this._bmpOrig, this.CachePathAddition);
                frm.SetupCache();

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    Bitmap bmp = frm.FBitmap;

                    if (bmp != null)
                    {
                        Bitmap b = new Bitmap(bmp);

                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b, this.helplineRulerCtrl1, "Bmp");

                        //Bitmap bC = new Bitmap(b);
                        //this.SetBitmap(ref _bmpRef, ref bC);

                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                        this._undoOPCache?.Add(b);

                        this._pic_changed = true;
                    }
                }
            }
        }

        private void btnApplyMatte2_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null && this._bmpMatte != null)
            {
                using (Bitmap b = new Bitmap(this.pictureBox1.Image))
                using (Bitmap b2 = new Bitmap(b.Width, b.Height))
                {
                    using (Graphics gx = Graphics.FromImage(b2))
                    {
                        gx.Clear(Color.Black);
                        gx.DrawImage(b, (float)this.numshiftMatteX.Value, (float)this.numshiftMatteY.Value);
                    }

                    Bitmap? bmp = ApplyAlphaMatte(this.helplineRulerCtrl1.Bmp, b2);

                    if (bmp != null)
                    {
                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                        //Bitmap bC = new Bitmap(bmp);
                        //this.SetBitmap(ref _bmpRef, ref bC);

                        this.helplineRulerCtrl1.Zoom = this.helplineRulerCtrl1.Zoom;
                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                        _undoOPCache?.Add(bmp);

                        this._pic_changed = true;
                    }
                }
            }
        }

        private void btnResizeMatte_Click(object sender, EventArgs e)
        {
            if (this._bmpMatte != null)
            {
                Bitmap b = new Bitmap((int)this.numRMatteW.Value, (int)this.numRMatteW.Value);
                using Graphics g = Graphics.FromImage(b);
                g.DrawImage(this._bmpMatte, 0, 0, b.Width, b.Height);

                this.SetBitmap(ref this._bmpMatte, ref b);
                this.pictureBox1.Image = this._bmpMatte;
                this.pictureBox1.Refresh();

                _undoOPCache?.Add(b);

                this._pic_changed = true;
            }
        }

        public void SetOrig(Bitmap b)
        {
            Bitmap? bmp = new Bitmap(b);
            this.SetBitmap(ref this._bmpOrig, ref bmp);
            this.pictureBox2.Image = this._bmpOrig;
            this.pictureBox2.Refresh();

            _undoOPCache?.Add(bmp);

            this._pic_changed = true;
        }

        public void SetMatte(Bitmap b)
        {
            Bitmap? bmp = new Bitmap(b);
            if (bmp.Width != this.helplineRulerCtrl1.Bmp.Width || bmp.Height != this.helplineRulerCtrl1.Bmp.Height)
            {
                Bitmap? bOld = bmp;
                bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                using Graphics g = Graphics.FromImage(bmp);
                g.DrawImage(bOld, 0, 0, bmp.Width, bmp.Height);
                if (bOld != null)
                    bOld.Dispose();
                bOld = null;
                MessageBox.Show("Matte resized.");
            }

            this.SetBitmap(ref this._bmpMatte, ref bmp);
            this.pictureBox1.Image = this._bmpMatte;
            this.pictureBox1.Refresh();

            _undoOPCache?.Add(bmp);

            this._pic_changed = true;
        }

        private void btnMask_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker2.IsBusy)
            {
                this.backgroundWorker2.CancelAsync();
                return;
            }
            if (!this.backgroundWorker2.IsBusy && this.helplineRulerCtrl1.Bmp != null && this._bmpOrig != null)
            {
                this.btnMask.Text = "Cancel";

                this.SetControls(false);
                this.Cursor = Cursors.WaitCursor;

                Bitmap b = new Bitmap(this._bmpOrig);
                Bitmap bM = new Bitmap(this.helplineRulerCtrl1.Bmp);

                this.backgroundWorker2.RunWorkerAsync(new object[] { b, bM });
            }
        }

        private void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap b = (Bitmap)o[0];
                using Bitmap bM = (Bitmap)o[1];
                SetAlpha(b, bM);
                e.Result = b;
            }
        }

        private unsafe void SetAlpha(Bitmap b, Bitmap bM)
        {
            if (b.Width == bM.Width && b.Height == bM.Height)
            {
                int w = b.Width;
                int h = b.Height;

                BitmapData bmB = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                BitmapData bmM = bM.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                int stride = bmB.Stride;

                Parallel.For(0, h, y =>
                {
                    byte* p = (byte*)bmB.Scan0;
                    p += y * stride;
                    byte* pM = (byte*)bmM.Scan0;
                    pM += y * stride;

                    for (int x = 0; x < w; x++)
                    {
                        p[3] = pM[3];
                        p += 4;
                        pM += 4;
                    }
                });

                b.UnlockBits(bmB);
                bM.UnlockBits(bmM);
            }
        }

        private void backgroundWorker2_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                Bitmap bRes = (Bitmap)e.Result;

                this.SetBitmap(this.helplineRulerCtrl1.Bmp, bRes, this.helplineRulerCtrl1, "Bmp");

                //Bitmap bC = new Bitmap(bRes);
                //this.SetBitmap(ref _bmpRef, ref bC);

                this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                    (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                    (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                _undoOPCache?.Add(bRes);

            }

            this.btnMask.Text = "MaskOrig";

            this.SetControls(true);
            this.Cursor = Cursors.Default;

            this._pic_changed = true;

            this.helplineRulerCtrl1.dbPanel1.Invalidate();

            if (this.Timer3.Enabled)
                this.Timer3.Stop();

            this.Timer3.Start();

            this.backgroundWorker2.Dispose();
            this.backgroundWorker2 = new BackgroundWorker();
            this.backgroundWorker2.WorkerReportsProgress = true;
            this.backgroundWorker2.WorkerSupportsCancellation = true;
            this.backgroundWorker2.DoWork += backgroundWorker2_DoWork;
            //this.backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            this.backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
        }

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            frmEdgePic frm4 = new frmEdgePic(this.pictureBox2.Image);
            frm4.Text = "Orig";
            frm4.ShowDialog();
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

        private void WipeAlpha()
        {
            if (this.IsDisposed == false && this.Visible)
            {
                SetControls(false);
                this.Refresh();
                using (GraphicsPath gp = GetPath())
                {
                    using (Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp))
                    {
                        gx.PixelOffsetMode = PixelOffsetMode.Half;
                        gx.SmoothingMode = SmoothingMode.None;
                        gx.CompositingMode = CompositingMode.SourceCopy;

                        float w = (float)this.numPenSize.Value;

                        float[] f1 = new float[] { 1.0F, 0.0F, 0.0F, 0.0F, 0.0F };
                        float[] f2 = new float[] { 0.0F, 1.0F, 0.0F, 0.0F, 0.0F };
                        float[] f3 = new float[] { 0.0F, 0.0F, 1.0F, 0.0F, 0.0F };
                        float[] f4 = new float[] { 0.0F, 0.0F, 0.0F, (float)this.numAlpha.Value, 0.0F };
                        float[] f5 = new float[] { 0.0F, 0.0F, 0.0F, 0.0F, 1.0F };

                        float[][] arr = new float[][] { f1, f2, f3, f4, f5 };

                        ColorMatrix cm = new ColorMatrix(arr);
                        RectangleF r2; // = gp.GetBounds()
                        using (GraphicsPath gp2 = (GraphicsPath)gp.Clone()) // need a clone, else the path for drawing will be widened too and so being drawn too thick
                        {
                            using (Pen p2 = new Pen(Color.Black, w))
                            {
                                p2.LineJoin = LineJoin.Round;
                                p2.StartCap = LineCap.Round;
                                p2.EndCap = LineCap.Round;
                                gp2.Widen(p2);
                                r2 = gp2.GetBounds();
                            }
                        }

                        int w2 = Convert.ToInt32(Math.Ceiling(r2.Width));
                        int h2 = Convert.ToInt32(Math.Ceiling(r2.Height));

                        if (AvailMem.AvailMem.checkAvailRam(w2 * h2 * 4L))
                        {
                            using (Bitmap bmp = new Bitmap(w2, h2))
                            {
                                using (var ia = new ImageAttributes())
                                {
                                    ia.SetColorMatrix(cm);

                                    using (Graphics gx2 = Graphics.FromImage(bmp))
                                    {
                                        gx2.Clear(Color.Transparent);
                                        gx2.DrawImage(this.helplineRulerCtrl1.Bmp, new Rectangle(0, 0, bmp.Width, bmp.Height), r2.X, r2.Y, bmp.Width, bmp.Height, GraphicsUnit.Pixel, ia);
                                    }

                                    using (TextureBrush t = new TextureBrush(bmp))
                                    {
                                        t.TranslateTransform(r2.X, r2.Y);
                                        using (Pen p = new Pen(t, w))
                                        {
                                            p.LineJoin = LineJoin.Round;
                                            p.StartCap = LineCap.Round;
                                            p.EndCap = LineCap.Round;

                                            if (this.CurPath?.Count == 1)
                                                gx.FillPath(t, gp);
                                            else
                                                gx.DrawPath(p, gp);

                                            this.CurPath = new List<PointF>();

                                            _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                                            this.btnUndo.Enabled = true;

                                            CheckRedoButton();

                                            this._pic_changed = true;

                                            SetControls(true);
                                            // Me.HelplineRulerCtrl1.CalculateZoom()

                                            this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                                            this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                                            this.helplineRulerCtrl1.dbPanel1.Invalidate();

                                            this._dontDrawPath = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
                    float w = (float)((double)this.numPenSize.Value / (double)2.0);
                    gp.AddEllipse(this.CurPath[0].X - w, this.CurPath[0].Y - w, w * 2, w * 2);
                }
            }
            return gp;
        }

        private void btnSetAsMatte_Click(object sender, EventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);

                this.SetBitmap(ref this._bmpMatte, ref bmp);
                this.pictureBox1.Image = this._bmpMatte;
                this.pictureBox1.Refresh();

                _undoOPCache?.Add(bmp);

                this._pic_changed = true;
            }
        }

        private void backgroundWorker3_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;

                Bitmap b = (Bitmap)o[0];
                int krnl = (int)o[1];
                int maxVal = (int)o[2];
                bool matte = (bool)o[3];
                bool btnPic = (bool)o[4];

                DoBlur(b, krnl, maxVal, matte);

                e.Result = new object[] { b, matte, btnPic };
            }
        }

        private void DoBlur(Bitmap b, int krnl, int maxVal, bool matte)
        {
            Convolution conv = new();
            conv.ProgressPlus += Conv_ProgressPlus;
            conv.CancelLoops = false;

            FastZGaussian_Blur_NxN_SigmaAsDistance(b, krnl, 0.01, 255, false, conv, false, 1E-12, maxVal, matte, this.backgroundWorker3);
            conv.ProgressPlus -= Conv_ProgressPlus;
        }

        public bool FastZGaussian_Blur_NxN_SigmaAsDistance(Bitmap b, int Length, double Weight, int Sigma,
            bool SrcOnSigma, Convolution conv, bool logarithmic, double steepness2, int Radius2, bool matte, BackgroundWorker bgw)
        {
            int wh = Math.Min(b.Width, b.Height) - 1;
            if (Length > wh)
            {
                Length = wh;
                if ((Length & 0x1) != 1)
                    Length -= 1;
                Console.WriteLine("new length is: " + wh.ToString());
            }
            if ((Length & 0x1) != 1)
                return false;

            double[] KernelVector = new double[Length];

            int Radius = Length / 2;

            double a = -2.0 * Radius * Radius / Math.Log(Weight);
            double Sum = 0.0;

            for (int x = 0; x < KernelVector.Length; x++)
            {
                double dist = Math.Abs(x - Radius);
                KernelVector[x] = Math.Exp(-dist * dist / a);
                Sum += KernelVector[x];
            }

            for (int x = 0; x < KernelVector.Length; x++)
                KernelVector[x] /= Sum;

            double[] AddValVector = conv.CalculateStandardAddVals(KernelVector, Math.Min(255, b.Width - 1));

            double[] DistanceWeightsF = new double[System.Convert.ToInt32(255 * Math.Sqrt(3)) * 2];

            // Dim Radius2 As Integer = DistanceWeightsF.Length \ 2
            double a2 = -2.0 * Radius2 * Radius2 / Math.Log(steepness2);
            double Sum2 = 0.0;

            if (Radius2 < 444)
                for (int x = 0; x < DistanceWeightsF.Length; x++)
                {
                    double dist = Math.Abs(x - DistanceWeightsF.Length / 2);
                    DistanceWeightsF[x] = Math.Exp(-dist * dist / a2);
                    if (x >= DistanceWeightsF.Length / 2)
                        Sum2 += DistanceWeightsF[x];
                }

            double[] DistanceWeights = new double[DistanceWeightsF.Length / 2 + 1];

            // Dim s2 As Double = 0
            for (int x = DistanceWeightsF.Length / 2; x < DistanceWeightsF.Length; x++)
            {
                if (Radius2 == 444)
                    DistanceWeights[x - DistanceWeightsF.Length / 2] = 1;
                else
                    DistanceWeights[x - DistanceWeightsF.Length / 2] = DistanceWeightsF[x]; // / Sum2;
            }

            // MsgBox(s2) 'should be 1
            ProgressEventArgs pe = new ProgressEventArgs(b.Height + b.Width, 20, 1);

            try
            {
                if (matte)
                    conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, DistanceWeights, 0, Sigma, false, Math.Min(255, b.Width - 1), SrcOnSigma, pe, bgw, logarithmic);
                else
                    conv.ConvolveAlphaH_par_SigmaAsDistance(b, KernelVector, AddValVector, DistanceWeights, 0, Sigma, Math.Min(255, b.Width - 1), SrcOnSigma, pe, bgw, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate270FlipNone);

                if (matte)
                    conv.ConvolveH_par_SigmaAsDistance(b, KernelVector, AddValVector, DistanceWeights, 0, Sigma, false, Math.Min(255, b.Width - 1), SrcOnSigma, pe, bgw, logarithmic);
                else
                    conv.ConvolveAlphaH_par_SigmaAsDistance(b, KernelVector, AddValVector, DistanceWeights, 0, Sigma, Math.Min(255, b.Width - 1), SrcOnSigma, pe, bgw, logarithmic);

                b.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
                return false;
            }

            return true;
        }

        private void Conv_ProgressPlus(object sender, ProgressEventArgs e)
        {
            this.backgroundWorker3.ReportProgress((int)e.CurrentProgress);
        }

        private void backgroundWorker3_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage <= this.toolStripProgressBar1.Maximum)
                this.toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker3_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                object[] o = (object[])e.Result;
                Bitmap bmp = (Bitmap)o[0];
                bool matte = (bool)o[1];
                bool btnPic = (bool)o[2];

                if (bmp != null)
                {
                    if (matte && !btnPic)
                    {
                        this.SetBitmap(ref this._bmpMatte, ref bmp);
                        this.pictureBox1.Image = this._bmpMatte;
                        this.pictureBox1.Refresh();
                    }
                    else
                    {
                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, bmp, this.helplineRulerCtrl1, "Bmp");

                        //Bitmap bC = new Bitmap(bmp);
                        //this.SetBitmap(ref _bmpRef, ref bC);

                        this.helplineRulerCtrl1.Zoom = this.helplineRulerCtrl1.Zoom;
                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }

                    _undoOPCache?.Add(bmp);
                    this._pic_changed = true;
                    this.toolStripProgressBar1.Value = 0;

                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }

                this.backgroundWorker3.Dispose();
                this.backgroundWorker3 = new BackgroundWorker();
                this.backgroundWorker3.WorkerReportsProgress = true;
                this.backgroundWorker3.WorkerSupportsCancellation = true;
                this.backgroundWorker3.DoWork += backgroundWorker3_DoWork;
                this.backgroundWorker3.ProgressChanged += backgroundWorker3_ProgressChanged;
                this.backgroundWorker3.RunWorkerCompleted += backgroundWorker3_RunWorkerCompleted;
            }
        }

        private void btnBlurResult_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                Bitmap b = new Bitmap(this.helplineRulerCtrl1.Bmp);

                int krnl = (int)this.numKernel2.Value;
                int maxVal = (int)this.numDistWeight2.Value;

                object[] o = { b, krnl, maxVal, this.cbPicIsMatte.Checked ? true : false, true };

                this.backgroundWorker3.RunWorkerAsync(o);
            }
        }

        private void btnBlurMatte_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker3.IsBusy)
            {
                this.backgroundWorker3.CancelAsync();
                return;
            }

            if (this._bmpMatte != null)
            {
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                Bitmap b = new Bitmap(this._bmpMatte);

                int krnl = (int)this.numKernel2.Value;
                int maxVal = (int)this.numDistWeight2.Value;

                object[] o = { b, krnl, maxVal, true, false };

                this.backgroundWorker3.RunWorkerAsync(o);
            }
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
    }
}
