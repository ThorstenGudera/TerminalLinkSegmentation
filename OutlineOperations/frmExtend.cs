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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutlineOperations
{
    public partial class frmExtend : Form
    {
        private bool _dontDoZoom;
        private bool _pic_changed;
        private bool _dontAskOnClosing;
        private Bitmap? _bmpBU;
        private Bitmap? _bmpOrig;
        private UndoOPCache? _undoOPCache;
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
        private int _ix;
        private int _iy;
        private bool _tracking;
        private List<Point>? _eraseList;
        private GraphicsPath? _erasePath;

        public Bitmap FBitmap
        {
            get
            {
                return this.helplineRulerCtrl1.Bmp;
            }
        }
        public frmExtend(Bitmap bmp, Bitmap bmpOrig, string basePathAddition)
        {
            InitializeComponent();

            CachePathAddition = basePathAddition;

            if (AvailMem.AvailMem.checkAvailRam(bmp.Width * bmp.Height * 16L))
            {
                this.helplineRulerCtrl1.Bmp = (Bitmap)bmp.Clone();
                _bmpBU = (Bitmap)bmp.Clone();
                if (bmpOrig != null)
                {
                    this._bmpOrig = new Bitmap(bmpOrig);
                    this.pictureBox1.Image = this._bmpOrig;
                }
                this.pictureBox1.Refresh();
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

            //while developing...
            //AvailMem.AvailMem.NoMemCheck = true;
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
                                    b1 = (Bitmap)img.Clone();
                                    this.SetBitmap(this.helplineRulerCtrl1.Bmp, b1, this.helplineRulerCtrl1, "Bmp");
                                    b2 = (Bitmap)img.Clone();
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
                            //this.btnReset2.Enabled = false;

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

        private void bitmappanel1_DragOver(object? sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void helplineRulerCtrl1_MouseDown(object? sender, MouseEventArgs e)
        {
            _ix = System.Convert.ToInt32((e.X - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X) / (double)this.helplineRulerCtrl1.Zoom);
            _iy = System.Convert.ToInt32((e.Y - this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y) / (double)this.helplineRulerCtrl1.Zoom);

            if (this.helplineRulerCtrl1.Bmp != null)
            {
                if (this.cbErase.Checked && e.Button == MouseButtons.Left)
                {
                    if (this._eraseList == null)
                        this._eraseList = new List<Point>();

                    if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                        this._eraseList.Add(new Point(_ix, _iy));

                    this._tracking = true;
                }
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

            _ix = ix;
            _iy = iy;

            if (ix >= 0 && ix < this.helplineRulerCtrl1.Bmp.Width && iy >= 0 && iy < this.helplineRulerCtrl1.Bmp.Height)
            {
                Color c = this.helplineRulerCtrl1.Bmp.GetPixel(ix, iy);
                this.toolStripStatusLabel1.Text = ix.ToString() + "; " + iy.ToString();
                this.toolStripStatusLabel2.BackColor = c;

                this.helplineRulerCtrl1.dbPanel1.Invalidate();
            }

            if (this._tracking)
            {
                if (_ix >= 0 && _iy >= 0 && _ix < this.helplineRulerCtrl1.Bmp.Width && _iy < this.helplineRulerCtrl1.Bmp.Height)
                    this._eraseList?.Add(new Point(_ix, _iy));

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
                    if (this._eraseList != null && this._eraseList.Count == 1 &&
                        _ix == this._eraseList[0].X && _iy == this._eraseList[0].Y)
                    {
                        this.AddPointsToErasePath();
                        ClearBmpRegions();
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }

                    if (this._eraseList != null && this._eraseList.Count > 1)
                    {
                        this._eraseList?.Add(new Point(_ix, _iy));
                        this.AddPointsToErasePath();
                        ClearBmpRegions();
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();
                    }
                }
            }

            this._tracking = false;
        }

        private void AddPointsToErasePath()
        {
            if (this._eraseList == null)
                this._eraseList = new List<Point>();
            if (this._erasePath == null)
                this._erasePath = new GraphicsPath();
            this._erasePath.Reset();

            if (this._eraseList != null)
            {
                if (this.cbErase.Checked)
                {
                    if (this._eraseList.Count > 1)
                        this._erasePath.AddLines(this._eraseList.ToArray());
                    else if (this._eraseList.Count == 1)
                    {
                        float wh = (float)this.numErase.Value;
                        this._erasePath.AddEllipse(new RectangleF(this._eraseList[0].X - wh / 4f,
                            this._eraseList[0].Y - wh / 4f,
                            wh / 2f, wh / 2f));
                    }
                }

                this._eraseList.Clear();
            }

            this.helplineRulerCtrl1.dbPanel1.Invalidate();
        }

        private void ClearBmpRegions()
        {
            if (this._erasePath != null && this._erasePath.PointCount > 1)
            {
                using (GraphicsPath gP = (GraphicsPath)this._erasePath.Clone())
                using (Graphics gx = Graphics.FromImage(this.helplineRulerCtrl1.Bmp))
                {
                    gx.CompositingMode = CompositingMode.SourceCopy;
                    gx.FillPath(Brushes.Transparent, gP);

                    using (Pen pen = new Pen(Color.Transparent, (float)this.numErase.Value))
                    {
                        pen.LineJoin = LineJoin.Round;
                        gx.DrawPath(pen, gP);
                    }

                    _undoOPCache?.Add(this.helplineRulerCtrl1.Bmp);
                    this._pic_changed = true;
                    this.btnUndo.Enabled = true;
                    this.CheckRedoButton();
                }
            }
        }

        private void helplineRulerCtrl1_Paint(object sender, PaintEventArgs e)
        {
            if (this._eraseList != null && this._eraseList.Count > 1)
            {
                using (GraphicsPath gP = new GraphicsPath())
                {
                    gP.AddLines(this._eraseList.ToArray());
                    using (Matrix mx = new Matrix(this.helplineRulerCtrl1.Zoom, 0, 0, this.helplineRulerCtrl1.Zoom,
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.X, this.helplineRulerCtrl1.dbPanel1.AutoScrollPosition.Y))
                        gP.Transform(mx);
                    using (Pen pen = new Pen(Color.Cyan, Math.Max((float)this.numErase.Value * this.helplineRulerCtrl1.Zoom, 1.0f)))
                    {
                        pen.LineJoin = LineJoin.Round;
                        e.Graphics.DrawPath(pen, gP);
                    }
                }
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

        private void cbBGColor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbBGColor.Checked)
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.ControlDarkDark;
            else
                this.helplineRulerCtrl1.dbPanel1.BackColor = SystemColors.Control;
        }

        private void button2_Click(object sender, EventArgs e)
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

                if (!_pic_changed && this._bmpBU != null)
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

                        this.helplineRulerCtrl1.SetZoom(this.helplineRulerCtrl1.Zoom.ToString());
                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);
                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(
                            (int)(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom),
                            (int)(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));

                        this._pic_changed = false;

                        //this.helplineRulerCtrl1.CalculateZoom();

                        this.helplineRulerCtrl1.MakeBitmap(this.helplineRulerCtrl1.Bmp);

                        // SetHRControlVars();

                        this.helplineRulerCtrl1.dbPanel1.AutoScrollMinSize = new Size(System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Width * this.helplineRulerCtrl1.Zoom), System.Convert.ToInt32(this.helplineRulerCtrl1.Bmp.Height * this.helplineRulerCtrl1.Zoom));
                        this.helplineRulerCtrl1.dbPanel1.Invalidate();

                        _undoOPCache?.Reset(false);
                        //this.btnReset2.Enabled = false;

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

        private void button10_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The goal is to achieve results as good as with a GrabCut, but only with the first (GMM_probabilities) estimation, to reduce the memory footprint. So keep \"QuickEstimation\" checked and play around with the other parameters.");
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

        private void frmExtend_FormClosing(object sender, FormClosingEventArgs e)
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
                if (this._bmpOrig != null)
                    this._bmpOrig.Dispose();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this._dontAskOnClosing = true;
        }

        public void SetupCache()
        {
            _undoOPCache = new Cache.UndoOPCache(this.GetType(), CachePathAddition, "tAppExt");
            if (this.helplineRulerCtrl1.Bmp != null)
                _undoOPCache.Add(this.helplineRulerCtrl1.Bmp);
        }

        private void btnExtendOrShrink_Click(object sender, EventArgs e)
        {
            if (this.backgroundWorker1.IsBusy)
            {
                this.backgroundWorker1.CancelAsync();
                return;
            }

            if (!this.backgroundWorker1.IsBusy && this._bmpOrig != null && this.helplineRulerCtrl1.Bmp != null)
            {
                int width = (int)this.numExtendOrShrink.Value;
                Bitmap bmp = new Bitmap(this.helplineRulerCtrl1.Bmp);
                Bitmap bOrig = new Bitmap(this._bmpOrig);
                bool opaque = this.cbSetOpaque.Checked;

                bool morph = this.cbMorphological.Checked;
                bool diskshaped = this.cbDiskShaped.Checked;
                bool once = this.rbOnce.Checked;

                this.Cursor = Cursors.WaitCursor;
                this.SetControls(false);
                this.btnExtendOrShrink.Text = "Cancel";
                this.btnExtendOrShrink.Enabled = true;
                this.toolStripProgressBar1.Value = 0;
                this.toolStripProgressBar1.Visible = true;

                this.backgroundWorker1.RunWorkerAsync(new object[] { bmp, bOrig, width, opaque, morph, diskshaped, once });
            }
        }

        private void frmExtend_Load(object sender, EventArgs e)
        {
            this.cmbZoom.SelectedIndex = 4;
            this.cbBGColor_CheckedChanged(this.cbBGColor, new EventArgs());
        }

        private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (e.Argument != null)
            {
                object[] o = (object[])e.Argument;
                Bitmap bmp = (Bitmap)o[0];
                Bitmap bOrig = (Bitmap)o[1];
                int width = (int)o[2];
                Bitmap? bResult = null;
                bool opaque = (bool)o[3];

                bool morph = (bool)o[4];
                bool diskshaped = (bool)o[5];
                bool once = (bool)o[6];

                if (opaque)
                    this.SetOpaque(bmp, bOrig, false);

                if (morph)
                {
                    Bitmap bw = this.GetBWMask(bmp);

                    if (width < 0)
                        this.Erode(bw, -width, diskshaped, once);
                    else
                        this.Dilate(bw, width, diskshaped, once);

                    this.GetCroppedPic(bw, bOrig);

                    bResult = bw;
                }
                else
                {
                    if (width < 0)
                        bResult = RemoveOutlineEx(bmp, -width, true);
                    else
                        bResult = ExtendOutlineEx(bmp, bOrig, width, true, true);
                }

                e.Result = bResult;
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

        private unsafe Bitmap GetBWMask(Bitmap bmp)
        {
            Bitmap bResult = new Bitmap(bmp);

            int w = bmp.Width;
            int h = bmp.Height;
            BitmapData bmD = bResult.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] == 0)
                        p[0] = p[1] = p[2] = 0;
                    else
                        p[0] = p[1] = p[2] = 255;

                    p[3] = 255;

                    p += 4;
                }
            });

            bResult.UnlockBits(bmD);

            return bResult;
        }

        private unsafe void GetCroppedPic(Bitmap bmp, Bitmap bOrig)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData bmRead = bOrig.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;
                byte* pRead = (byte*)bmRead.Scan0;
                pRead += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[0] == 0)
                        p[0] = p[1] = p[2] = p[3] = 0;
                    else
                    {
                        p[0] = pRead[0];
                        p[1] = pRead[1];
                        p[2] = pRead[2];
                        p[3] = pRead[3];
                    }

                    p += 4;
                    pRead += 4;
                }
            });

            bmp.UnlockBits(bmD);
            bOrig.UnlockBits(bmRead);
        }

        private unsafe void SetOpaque(Bitmap bmp, Bitmap bOrig, bool dontFill)
        {
            int w = bmp.Width;
            int h = bmp.Height;
            BitmapData bmD = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int stride = bmD.Stride;

            Parallel.For(0, h, y =>
            {
                byte* p = (byte*)bmD.Scan0;
                p += y * stride;

                for (int x = 0; x < w; x++)
                {
                    if (p[3] > 0)
                        p[3] = 255;

                    p += 4;
                }
            });

            bmp.UnlockBits(bmD);

            List<ChainCode> l = this.GetBoundary(bmp);

            if (l != null && l.Count > 0)
            {
                if (l.Count > 0)
                {
                    l = l.OrderByDescending(a => a.Coord.Count).ToList();

                    for (int i = 0; i < l.Count; i++)
                    {
                        List<PointF> pts = l[i].Coord.Select(a => new PointF(a.X, a.Y)).ToList();

                        if (pts.Count > 2)
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                gp.AddLines(pts.ToArray());

                                if (gp.PointCount > 0)
                                {
                                    using (Graphics gx = Graphics.FromImage(bmp))
                                    {
                                        gx.SmoothingMode = SmoothingMode.None;
                                        gx.InterpolationMode = InterpolationMode.NearestNeighbor;

                                        using (TextureBrush tb = new TextureBrush(bOrig))
                                        using (Pen pen = new Pen(tb, 1))
                                        {
                                            gx.FillPath(tb, gp);
                                            gx.DrawPath(pen, gp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (dontFill)
                    for (int i = 0; i < l.Count; i++)
                    {
                        if (ChainFinder.IsInnerOutline(l[i]))
                        {
                            List<PointF> pts = l[i].Coord.Select(a => new PointF(a.X, a.Y)).ToList();

                            if (pts.Count > 2)
                            {
                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    gp.AddLines(pts.ToArray());

                                    if (gp.PointCount > 0)
                                    {
                                        using (Graphics gx = Graphics.FromImage(bmp))
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
            }

            this.btnExtendOrShrink.Text = "Go";

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

        private void CheckRedoButton()
        {
            _undoOPCache?.CheckRedoButton(this.btnRedo);
        }

        private void SetControls(bool e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    foreach (Control ct in this.splitContainer1.Panel2.Controls)
                    {
                        if (ct != null)
                        {
                            if (ct.Name != "btnCancel" && !(ct is PictureBox))
                                ct.Enabled = e;

                            if (ct is GroupBox)
                            {
                                ct.Enabled = true;
                                GroupBox gb = (GroupBox)ct;

                                foreach (Control c in gb.Controls)
                                {
                                    if (!(c is Button))
                                        c.Enabled = e;
                                }
                            }
                        }
                    }

                    this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl1.Enabled = e;

                    this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
                }));
            }
            else
            {
                foreach (Control ct in this.splitContainer1.Panel2.Controls)
                {
                    if (ct.Name != "btnCancel" && !(ct is PictureBox))
                        ct.Enabled = e;
                    if (ct != null && ct is GroupBox)
                    {
                        ct.Enabled = true;
                        GroupBox gb = (GroupBox)ct;

                        foreach (Control c in gb.Controls)
                        {
                            if (!(c is Button))
                                c.Enabled = e;
                        }
                    }
                }

                this.helplineRulerCtrl1.Enabled = this.helplineRulerCtrl1.Enabled = e;

                this.Cursor = e ? Cursors.Default : Cursors.WaitCursor;
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

            List<ChainCode> lInner = GetBoundary(bOut);

            if (lInner.Count > 0)
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

                                    using (TextureBrush tb = new TextureBrush(bmp))
                                    using (Pen pen = new Pen(tb, 1))
                                    {
                                        gx.FillPath(tb, gp);
                                        gx.DrawPath(pen, gp);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (dontFill)
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

        private List<ChainCode> GetBoundary(Bitmap bmp)
        {
            ChainFinder cf = new ChainFinder();
            cf.AllowNullCells = true;
            List<ChainCode> l = cf.GetOutline(bmp, 0, false, 0, false, 0, false);
            return l;
        }

        public Bitmap? RemOutline(Bitmap bmp, int breite, System.ComponentModel.BackgroundWorker? bgw)
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

                        cf.RemoveOutline(b, fList);
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

        private Bitmap ExtendOutlineEx(Bitmap bmp, Bitmap bOrig, int outerW, bool dontFill, bool drawPath)
        {
            Bitmap bOut = new Bitmap(bmp.Width, bmp.Height);

            List<ChainCode> lInner = GetBoundary(bmp);

            if (lInner?.Count > 0 && bOrig != null)
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

                                    using (TextureBrush tb = new TextureBrush(bOrig))
                                    using (Pen pen = new Pen(tb, 1))
                                    {
                                        gx.FillPath(tb, gp);
                                        gx.DrawPath(pen, gp);

                                        if (drawPath && outerW > 0)
                                        {
                                            try
                                            {
                                                using (Pen pen2 = new Pen(tb, outerW))
                                                {
                                                    pen2.LineJoin = LineJoin.Round;
                                                    gp.Widen(pen2);
                                                    gx.DrawPath(pen2, gp);
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

        private void btnLoadBasePic_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK && this.helplineRulerCtrl1.Bmp != null)
            {
                Bitmap? bmp = null;
                using (Image img = Image.FromFile(this.openFileDialog1.FileName))
                    //if(img.Width >= this.helplineRulerCtrl1.Bmp.Width && img.Height >= this.helplineRulerCtrl1.Bmp.Height)
                    if (img.Width == this.helplineRulerCtrl1.Bmp.Width && img.Height == this.helplineRulerCtrl1.Bmp.Height)
                        bmp = new Bitmap(img);
                    else
                    {
                        bmp = new Bitmap(this.helplineRulerCtrl1.Bmp.Width, this.helplineRulerCtrl1.Bmp.Height);
                        using (Graphics gx = Graphics.FromImage(bmp))
                        {
                            gx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            gx.DrawImage(img, 0, 0, bmp.Width, bmp.Height);
                        }
                    }

                this.SetBitmap(ref this._bmpOrig, ref bmp);
                this.pictureBox1.Image = this._bmpOrig;
                this.pictureBox1.Refresh();
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
    }
}
