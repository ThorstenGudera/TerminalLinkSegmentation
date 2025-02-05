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

namespace PseudoShadow
{
    public partial class frmExcludeFromPic : Form
    {
        private UndoOPCache? _undoOPCache = null;

        //private bool _dontAskOnClosing;

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

        public List<ExcludedBmpRegion>? ExcludedBmpRegions { get; set; }

        private Bitmap? _bmpBU = null;
        private bool _pic_changed = false;
        private bool _dontDoZoom;
        private Bitmap? _remaining;
        private int _iX;
        private int _iY;
        private bool _tracking;
        private List<Point> _points2 = new List<Point>();
        private List<List<Point>>? _allPoints;
        private Rectangle _rc;

        public frmExcludeFromPic(Bitmap bmp, string basePathAddition)
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
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            int ix = (int)((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            int iy = (int)((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            this.helplineRulerCtrl1.dbPanel1.Capture = true;

            if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                this._iX = ix;
                this._iY = iy;

                if (this.cbDraw.Checked && e.Button == MouseButtons.Left)
                {
                    this._points2.Add(new Point(ix, iy));
                    this._tracking = true;
                }

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }
        }

        private void helplineRulerCtrl1_MouseMove(object? sender, MouseEventArgs e)
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
                    this._iX = ix;
                    this._iY = iy;

                    this._points2.Add(new Point(ix, iy));
                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                }

                Color c = this.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                this.toolStripStatusLabel1.Text = ix.ToString() + "; " + iy.ToString();
                this.toolStripStatusLabel2.BackColor = c;
            }
        }

        private void helplineRulerCtrl1_MouseUp(object? sender, MouseEventArgs e)
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
                    if (this._iX == ix && this._iY == iy)
                        AddPointsToPath(true);
                    else
                    {
                        this._iX = ix;
                        this._iY = iy;

                        AddPointsToPath(false);
                    }

                    this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    this._points2.Clear();
                }
            }

            this._tracking = false;
            this.helplineRulerCtrl1.dbPanel1.Capture = false;
        }

        private void AddPointsToPath(bool v)
        {
            if (this._allPoints == null)
                this._allPoints = new List<List<Point>>();

            if (v && this._allPoints != null && this._allPoints.Count > 0 && this._allPoints[this._allPoints.Count - 1] != null &&
                this._allPoints[this._allPoints.Count - 1].Count > 0 && this._points2.Count == 1)
            {
                this._allPoints.Add(new List<Point>());
                this._allPoints[this._allPoints.Count - 1].Add(new Point(this._points2[0].X, this._points2[0].Y));
            }
            else if (this._allPoints != null)
            {
                this._allPoints.Add(new List<Point>());
                this._allPoints[this._allPoints.Count - 1].AddRange(this._points2.ToArray());
            }
        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {
            if (this._allPoints != null)
            {
                using GraphicsPath gP = new GraphicsPath();

                for (int j = 0; j < this._allPoints.Count; j++)
                    gP.AddLines(this._allPoints[j].Select(a => new PointF(a.X, a.Y)).ToArray());

                using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                gP.Transform(mx);

                using Pen pen = new Pen(Color.Red, Math.Max((float)((double)this.numPenWidth.Value * this.helplineRulerCtrl1.Zoom), 1f));
                e.Graphics.DrawPath(pen, gP);
            }

            if (this._points2.Count > 0)
            {
                using GraphicsPath gP = new GraphicsPath();
                gP.AddLines(this._points2.Select(a => new PointF(a.X, a.Y)).ToArray());
                using Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                    this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y);
                gP.Transform(mx);

                using Pen pen = new Pen(Color.Cyan, Math.Max((float)((double)this.numPenWidth.Value * this.helplineRulerCtrl1.Zoom), 1f));
                e.Graphics.DrawPath(pen, gP);
            }
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "frmExcl");
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
                                        b1 = new Bitmap(img);
                                        this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                        b2 = new Bitmap(img);
                                        this.SetBitmap(ref this._bmpBU, ref b2);
                                    }
                                    else
                                        throw new Exception();
                                }

                                if (this.helplineRulerCtrl1.Bmp != null)
                                {
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
            MessageBox.Show("");
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
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void Button28_Click(object sender, EventArgs e)
        {
            //this._dontAskOnClosing = true;

            if (this.ExcludedBmpRegions == null)
                this.ExcludedBmpRegions = new List<ExcludedBmpRegion>();

            this.ExcludedBmpRegions.Clear();

            if (this.checkedListBox1.CheckedItems.Count > 0)
                for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                {
                    if (this.checkedListBox1.CheckedItems[i] != null)
                    {
                        ExcludedBmpRegion? r = (ExcludedBmpRegion?)this.checkedListBox1.CheckedItems[i];
                        if (r != null)
                            this.ExcludedBmpRegions.Add(r);
                    }
                }
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.splitContainer2.Panel1.Controls)
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

                    foreach (Control ct in this.splitContainer1.Panel2.Controls)
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

                    this.helplineRulerCtrl1.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.splitContainer2.Panel1.Controls)
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

                foreach (Control ct in this.splitContainer1.Panel2.Controls)
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

        private List<ChainCode>? GetBoundary(Bitmap? bmp)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, 0, false, 0, false, 0, false);
            return l;
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

            using Graphics g = Graphics.FromImage(bOut);
            g.Clear(Color.Transparent);

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
                                    using TextureBrush tb = new TextureBrush(bmp);
                                    gx.FillPath(tb, gp);
                                    using Pen p = new Pen(tb, 1);
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

            List<ChainCode>? lInner = GetBoundary(bOut);

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

        public Bitmap? RemOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
        {
            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 8L))
            {
                Bitmap? b = null;

                try
                {
                    b = (Bitmap)bmp.Clone();

                    int maxWidth = Math.Max(b.Width, b.Height);

                    for (int i = 0; i < Math.Min(breite, maxWidth); i++)
                    {
                        if (bgw != null && bgw.WorkerSupportsCancellation && bgw.CancellationPending)
                            break;

                        ChainFinder cf = new ChainFinder();
                        cf.AllowNullCells = true;

                        List<ChainCode> fList = cf.GetOutline(b, 0, false, 0, false);

                        if (fList.Count > 0)
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

        private void frmExcludeFromPic_Load(object sender, EventArgs e)
        {
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
            this.cmbZoom.SelectedIndex = 4;
        }

        private void frmExcludeFromPic_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
                this.backgroundWorker1.CancelAsync();

            if (this._bmpBU != null)
                this._bmpBU.Dispose();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.CancelAsync();
                return;
            }

            if (!this.backgroundWorker1.IsBusy && this._bmpBU != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);
                this.btnShow.Text = "Cancel";
                this.btnShow.Enabled = true;
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                Bitmap bOrig = new Bitmap(this._bmpBU);
                int width = (int)this.numExceptBounds.Value;

                this.backgroundWorker1.RunWorkerAsync(new object[] { bOrig, width });
            }
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                using Bitmap bOrig = (Bitmap)o[0];
                int width = (int)o[1];

                using Bitmap b = RemoveOutlineEx(bOrig, width, true);

                Bitmap b2 = GetRemainingRegion(bOrig, b);
                Bitmap remg = new Bitmap(b2);
                this.SetBitmap(ref this._remaining, ref remg);

                e.Result = b2;
            }
        }

        private unsafe Bitmap GetRemainingRegion(Bitmap bOrig, Bitmap b)
        {
            int w = bOrig.Width;
            int h = bOrig.Height;
            Bitmap result = new Bitmap(bOrig.Width, bOrig.Height);

            BitmapData bmD = result.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmRead = b.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmOrig = bOrig.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;
                byte* pR = (byte*)bmRead.Scan0;
                pR += y * stride;
                byte* pO = (byte*)bmOrig.Scan0;
                pO += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (pR[3] > 0)
                        p[3] = 0;
                    else
                    {
                        p[0] = pO[0];
                        p[1] = pO[1];
                        p[2] = pO[2];
                        p[3] = pO[3];
                    }
                    p += 4;
                    pR += 4;
                    pO += 4;
                }
            });

            result.UnlockBits(bmD);
            b.UnlockBits(bmRead);
            bOrig.UnlockBits(bmOrig);

            return result;
        }

        private void backgroundWorker1_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
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

                this._undoOPCache?.Add(bmp);

                this.btnUndo.Enabled = true;
                this._pic_changed = true;
                this.CheckRedoButton();

                this.btnAdd.Enabled = true;
            }

            this.btnShow.Text = "show";

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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (this._remaining != null)
            {
                ExcludedBmpRegion r = new(this._remaining);
                this.checkedListBox1.Items.Add(r, true);
                this.btnAdd.Enabled = false;
            }
        }

        private void btnNewPath_Click(object sender, EventArgs e)
        {
            if (this._allPoints != null)
                this._allPoints.Clear();

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnRemSeg_Click(object sender, EventArgs e)
        {
            if (this._allPoints != null && this._allPoints.Count > 0)
                this._allPoints.RemoveAt(this._allPoints.Count - 1);

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnRemPoint_Click(object sender, EventArgs e)
        {
            if (this._allPoints != null && this._allPoints.Count > 0)
            {
                List<Point> points = this._allPoints[this._allPoints.Count - 1];
                if (points.Count > 1)
                    points.RemoveAt(points.Count - 1);
                else
                    this._allPoints.RemoveAt(this._allPoints.Count - 1);
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnClosePath_Click(object sender, EventArgs e)
        {
            if (this._allPoints != null && this._allPoints.Count > 0)
            {
                List<Point> points = this._allPoints[this._allPoints.Count - 1];
                if (this._allPoints[0].Count > 0)
                {
                    if (points[points.Count - 1].X != this._allPoints[0][0].X || points[points.Count - 1].Y != this._allPoints[0][0].Y)
                        points.Add(this._allPoints[0][0]);
                }
                else
                    this._allPoints.RemoveAt(this._allPoints.Count - 1);
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void btnShow2_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker2.IsBusy)
            {
                this.backgroundWorker2.CancelAsync();
                return;
            }

            if (!this.backgroundWorker2.IsBusy && this._bmpBU != null)
            {
                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);
                this.btnShow.Text = "Cancel";
                this.btnShow2.Enabled = true;
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                Bitmap? bOrig = null;

                if (this._allPoints != null)
                {
                    using GraphicsPath gP = new GraphicsPath();

                    for (int j = 0; j < this._allPoints.Count; j++)
                        gP.AddLines(this._allPoints[j].Select(a => new PointF(a.X, a.Y)).ToArray());

                    gP.CloseFigure();

                    RectangleF r = gP.GetBounds();
                    _rc = new Rectangle((int)Math.Floor(r.X), (int)Math.Floor(r.Y), (int)Math.Ceiling(r.Width), (int)Math.Ceiling(r.Height));

                    using Bitmap bC = this._bmpBU.Clone(new RectangleF(_rc.X, _rc.Y, _rc.Width, _rc.Height), this._bmpBU.PixelFormat);
                    bOrig = new Bitmap(_rc.Width, _rc.Height);
                    using Graphics gx = Graphics.FromImage(bOrig);
                    gx.SmoothingMode = SmoothingMode.None;
                    using TextureBrush tb = new TextureBrush(bC);
                    using Matrix mx = new(1, 0, 0, 1, -r.X, -r.Y);
                    gP.Transform(mx);
                    gx.FillPath(tb, gP);
                    int width = (int)this.numExceptBounds2.Value;

                    this.backgroundWorker2.RunWorkerAsync(new object[] { bOrig, width });
                }
            }
        }

        private void backgroundWorker2_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                using Bitmap bOrig = (Bitmap)o[0];
                int width = (int)o[1];

                using Bitmap b = RemoveOutlineEx(bOrig, width, true);

                Bitmap b2 = GetRemainingRegion(bOrig, b);
                Bitmap remg = new Bitmap(b2);
                this.SetBitmap(ref this._remaining, ref remg);

                e.Result = b2;
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

                this._undoOPCache?.Add(bmp);

                this.btnUndo.Enabled = true;
                this._pic_changed = true;
                this.CheckRedoButton();

                this.btnAdd2.Enabled = true;
            }

            this.btnShow2.Text = "show";

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
            this.backgroundWorker2.ProgressChanged += backgroundWorker2_ProgressChanged;
            this.backgroundWorker2.RunWorkerCompleted += backgroundWorker2_RunWorkerCompleted;
        }

        private void btnAdd2_Click(object sender, EventArgs e)
        {
            if (this._remaining != null)
            {
                ExcludedBmpRegion r = new(this._remaining);
                r.Location = _rc.Location;
                this.checkedListBox1.Items.Add(r, true);
                this.btnAdd2.Enabled = false;
            }
        }
    }
}
