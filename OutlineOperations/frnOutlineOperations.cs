using Cache;
using ChainCodeFinder;
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
                }
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
        {
            if (this.helplineRulerCtrl1.Bmp != null)
            {
                int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
                int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

                int eX = e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X;
                int eY = e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y;

                if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
                {
                    if (this._tracking)
                    {
                        if (this.cbDraw.Checked && this._bmpOrig != null)
                        {
                            this._points?.Add(new Point(ix, iy));
                            this.helplineRulerCtrl1.dbPanel1.Invalidate();
                        }
                    }

                    Color c = this.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                    this.toolStripStatusLabel1.Text = ix.ToString() + "; " + iy.ToString();
                    this.toolStripStatusLabel5.Text = c.ToString();
                    this.ToolStripStatusLabel2.BackColor = c;
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

                this.backgroundWorker4.RunWorkerAsync(new object[] { this.helplineRulerCtrl1.Bmp, alphaTh });
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

                this.backgroundWorker5.RunWorkerAsync(new object[] { this.helplineRulerCtrl1.Bmp, gamma });
            }
        }

        private void backgroundWorker4_DoWork(object? sender, DoWorkEventArgs e)
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

        private void backgroundWorker4_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (!this.IsDisposed)
            {
                Bitmap? bmp = null;

                if (e.Result != null)
                    bmp = (Bitmap)e.Result;

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

        private void pictureBox2_DoubleClick(object sender, EventArgs e)
        {
            frmEdgePic frm4 = new frmEdgePic(this.pictureBox2.Image);
            frm4.Text = "Orig";
            frm4.ShowDialog();
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
                Bitmap bM = (Bitmap)o[1];
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
    }
}
